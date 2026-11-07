using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;
using GreenNet.Core;

namespace GreenNet.Server;

/// <summary>
/// Lõi server: mọi kết nối điều khiển đi qua TLS với chứng chỉ cố định (vân tay được client ghim).
/// File đi qua TCP (cùng kênh TLS) hoặc UDP tin cậy (cổng UDP trùng cổng TCP, dữ liệu mã hóa AES-GCM).
/// </summary>
public class ServerEngine
{
    private const long MaxFileSize = 2L * 1024 * 1024 * 1024;
    private static readonly Regex UserRx = new("^[A-Za-z0-9_]{3,30}$");

    private readonly Database _db;
    private readonly object _lock = new();
    private TcpListener _listener;
    private RudpHub _hub;
    private X509Certificate2 _cert;
    private Thread _thread;
    private CancellationTokenSource _cts;
    private int _clients;
    private double _energy;
    private double _score = 100;

    public string Name { get; }
    public int Port { get; }
    public string FingerprintText { get; private set; } = "";
    public event Action<string> Log;

    public bool IsRunning => _listener != null;
    public int Clients => Volatile.Read(ref _clients);
    public double EnergyJ { get { lock (_lock) return _energy; } }
    public double Score { get { lock (_lock) return _score; } }

    public ServerEngine(string name, int port, string connectionString)
    {
        Name = name;
        Port = port;
        _db = new Database(connectionString);
    }

    private void Write(string msg) => Log?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msg}");

    public void Start()
    {
        if (IsRunning) return;
        _cert = ServerIdentity.LoadOrCreate(Name);
        FingerprintText = Fingerprint.Of(_cert);
        _hub = new RudpHub(Port);
        TcpListener l;
        try
        {
            l = new TcpListener(IPAddress.Any, Port);
            l.Start();
        }
        catch
        {
            _hub.Dispose();
            _hub = null;
            throw;
        }
        _cts = new CancellationTokenSource();
        _listener = l;
        var ct = _cts.Token;
        _thread = new Thread(() => AcceptLoop(l, ct)) { IsBackground = true, Name = "Accept-" + Name };
        _thread.Start();
        Write($"Server '{Name}' lắng nghe TCP+UDP ở cổng {Port}.");
        Write($"Vân tay khóa SHA-256: {FingerprintText}");
    }

    public void Stop()
    {
        if (_listener == null) return;
        _cts.Cancel();
        try { _listener.Stop(); } catch { }
        _listener = null;
        _hub?.Dispose();
        _hub = null;
        _cert?.Dispose();
        _cert = null;
        Write("Đã dừng server.");
    }

    private void AcceptLoop(TcpListener l, CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            TcpClient c;
            try { c = l.AcceptTcpClient(); }
            catch (SocketException) { break; }
            catch (ObjectDisposedException) { break; }
            c.NoDelay = true;
            _ = Task.Run(() => HandleClientAsync(c, ct));
        }
    }

    private async Task HandleClientAsync(TcpClient tcp, CancellationToken ct)
    {
        Interlocked.Increment(ref _clients);
        var remote = tcp.Client.RemoteEndPoint?.ToString() ?? "?";
        try
        {
            using var client = tcp;
            var cert = _cert;
            if (cert == null) return;
            using var ns = await SecureChannel.ServerAsync(client.GetStream(), cert, ct);

            while (!ct.IsCancellationRequested)
            {
                Msg m;
                using (var idle = CancellationTokenSource.CreateLinkedTokenSource(ct))
                {
                    idle.CancelAfter(TimeSpan.FromMinutes(5));
                    m = await Framing.ReadJsonAsync<Msg>(ns, idle.Token);
                }

                switch (m.Type)
                {
                    case "PING":
                        await Framing.WriteJsonAsync(ns, new Msg { Type = "PONG" }, ct);
                        break;
                    case "STATUS":
                        await Framing.WriteJsonAsync(ns, new Msg { Type = "OK", Clients = Clients, EnergyJ = EnergyJ, Score = Score }, ct);
                        break;
                    case "REGISTER":
                        await Framing.WriteJsonAsync(ns, await RegisterAsync(remote, m), ct);
                        break;
                    case "LOGIN":
                        await Framing.WriteJsonAsync(ns, await LoginAsync(remote, m), ct);
                        break;
                    case "HISTORY":
                        await Framing.WriteJsonAsync(ns, await HistoryAsync(m), ct);
                        break;
                    case "REPORT":
                        await Framing.WriteJsonAsync(ns, await ReportAsync(m), ct);
                        break;
                    case "SEND":
                        await ReceiveFileAsync(ns, m, ct);
                        break;
                    default:
                        await Framing.WriteJsonAsync(ns, Err("Yêu cầu không hợp lệ."), ct);
                        break;
                }
            }
        }
        catch (EndOfStreamException) { }
        catch (OperationCanceledException) { }
        catch (IOException) { }
        catch (System.Security.Authentication.AuthenticationException ex)
        {
            Write($"[{remote}] Bắt tay TLS thất bại: {ex.Message}");
        }
        catch (Exception ex)
        {
            Write($"[{remote}] Lỗi: {ex.Message}");
        }
        finally
        {
            Interlocked.Decrement(ref _clients);
        }
    }

    private static Msg Err(string text) => new() { Type = "ERR", Text = text };

    private async Task<Msg> RegisterAsync(string remote, Msg m)
    {
        var user = (m.User ?? "").Trim();
        var pass = m.Pass ?? "";
        if (!UserRx.IsMatch(user)) return Err("Tên đăng nhập phải có 3–30 ký tự: chữ, số hoặc dấu _.");
        if (pass.Length < 6 || pass.Length > 40) return Err("Mật khẩu phải có từ 6 đến 40 ký tự.");
        try
        {
            bool created = await _db.CreateUserAsync(user, CryptoHelper.HashPassword(pass));
            if (!created) return Err("Tên đăng nhập đã tồn tại.");
            Write($"[{remote}] Đăng ký tài khoản '{user}'.");
            return new Msg { Type = "OK" };
        }
        catch (Exception ex)
        {
            Write($"Lỗi CSDL (đăng ký): {ex.Message}");
            return Err("Lỗi hệ thống, vui lòng thử lại sau.");
        }
    }

    private async Task<Msg> LoginAsync(string remote, Msg m)
    {
        var user = (m.User ?? "").Trim();
        var pass = m.Pass ?? "";
        if (user.Length == 0 || pass.Length == 0 || pass.Length > 40) return Err("Thiếu thông tin đăng nhập.");
        try
        {
            var hash = await _db.GetPasswordHashAsync(user);
            if (hash == null || !CryptoHelper.VerifyPassword(pass, hash))
            {
                await Task.Delay(500);   // làm chậm dò mật khẩu
                Write($"[{remote}] Đăng nhập thất bại: '{user}'.");
                return Err("Sai tên đăng nhập hoặc mật khẩu.");
            }
            var token = await _db.CreateTokenAsync(user);
            Write($"[{remote}] '{user}' đăng nhập.");
            return new Msg { Type = "OK", Token = token };
        }
        catch (Exception ex)
        {
            Write($"Lỗi CSDL (đăng nhập): {ex.Message}");
            return Err("Lỗi hệ thống, vui lòng thử lại sau.");
        }
    }

    private async Task<Msg> HistoryAsync(Msg m)
    {
        try
        {
            var user = await _db.GetUserByTokenAsync(m.Token);
            if (user == null) return Err("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");
            return new Msg { Type = "OK", Rows = await _db.GetSessionsAsync(user) };
        }
        catch (Exception ex)
        {
            Write($"Lỗi CSDL (lịch sử): {ex.Message}");
            return Err("Lỗi hệ thống, vui lòng thử lại sau.");
        }
    }

    private static double Clean(double v) => double.IsFinite(v) && v >= 0 ? v : 0;

    private async Task<Msg> ReportAsync(Msg m)
    {
        try
        {
            var user = await _db.GetUserByTokenAsync(m.Token);
            if (user == null) return Err("Phiên đăng nhập không hợp lệ hoặc đã hết hạn.");

            var name = Path.GetFileName(m.FileName ?? "");
            if (name.Length > 260) name = name[..260];
            string algo = m.Algo == "Auto" ? "Auto"
                : Enum.TryParse<CompressAlgo>(m.Algo, out var a) ? a.ToString() : "None";

            var row = new SessionRow
            {
                ServerName = Name,
                FileName = name,
                FileSizeBytes = Math.Max(0, m.FileSize),
                BytesSent = Math.Max(0, m.BytesSent),
                Algo = algo,
                Protocol = m.Protocol == "UDP" ? "UDP" : "TCP",
                Encrypted = m.Encrypted,
                DurationMs = Clean(m.DurationMs),
                RttMs = Clean(m.RttMs),
                CpuMs = Clean(m.CpuMs),
                EnergyJ = Clean(m.EnergyJ),
                GreenScore = Math.Min(Clean(m.Score), 100),
                Sha256Ok = m.Ok
            };
            await _db.InsertSessionAsync(user, row);

            lock (_lock)
            {
                _energy += row.EnergyJ;
                _score = 0.7 * _score + 0.3 * row.GreenScore;
            }
            Write($"Phiên của '{user}': {name} ({row.Protocol}, {row.Algo}), {row.EnergyJ:0.000} J, Green Score {row.GreenScore:0.0}.");
            return new Msg { Type = "OK" };
        }
        catch (Exception ex)
        {
            Write($"Lỗi CSDL (báo cáo): {ex.Message}");
            return Err("Lỗi hệ thống, vui lòng thử lại sau.");
        }
    }

    private async Task ReceiveFileAsync(Stream ns, Msg m, CancellationToken ct)
    {
        string user;
        try { user = await _db.GetUserByTokenAsync(m.Token); }
        catch (Exception ex)
        {
            Write($"Lỗi CSDL (xác thực): {ex.Message}");
            await Framing.WriteJsonAsync(ns, Err("Lỗi hệ thống, vui lòng thử lại sau."), ct);
            return;
        }
        if (user == null)
        {
            await Framing.WriteJsonAsync(ns, Err("Phiên đăng nhập không hợp lệ hoặc đã hết hạn."), ct);
            return;
        }
        if (m.FileSize <= 0 || m.FileSize > MaxFileSize)
        {
            await Framing.WriteJsonAsync(ns, Err("Kích thước file không hợp lệ."), ct);
            return;
        }

        bool udp = m.Protocol == "UDP";
        byte[] key = null;
        if (m.Encrypted)
        {
            try { key = Convert.FromBase64String(m.SessionKey ?? ""); }
            catch (FormatException) { key = null; }
            if (key == null || key.Length != 32)
            {
                await Framing.WriteJsonAsync(ns, Err("Khóa phiên AES không hợp lệ."), ct);
                return;
            }
        }
        if (udp && key == null)
        {
            await Framing.WriteJsonAsync(ns, Err("UDP yêu cầu bật mã hóa AES-256-GCM."), ct);
            return;
        }

        var safeName = string.Concat((Path.GetFileName(m.FileName) ?? "file")
            .Select(c => Path.GetInvalidFileNameChars().Contains(c) ? '_' : c));
        if (safeName.Length == 0) safeName = "file";
        var dir = Path.Combine(AppContext.BaseDirectory, "Received", Name);
        Directory.CreateDirectory(dir);
        var path = Path.Combine(dir, $"{DateTime.Now:yyyyMMdd_HHmmss_fff}_{safeName}");

        // nguồn dữ liệu: cùng kênh TLS (TCP) hoặc luồng UDP tin cậy
        Stream source = ns;
        RudpHub hub = _hub;
        ulong sid = 0;
        RudpReceiver rcv = null;
        if (udp)
        {
            if (hub == null) { await Framing.WriteJsonAsync(ns, Err("Server chưa mở UDP."), ct); return; }
            sid = BitConverter.ToUInt64(RandomNumberGenerator.GetBytes(8));
            rcv = hub.Register(sid);
            source = rcv;
        }

        bool complete = false;
        long total = 0;
        string hash = null;
        try
        {
            await Framing.WriteJsonAsync(ns, new Msg { Type = "OK", UdpPort = Port, SessionId = sid }, ct);

            using var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            using (var fs = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            {
                while (true)
                {
                    byte[] frame;
                    using (var fcts = CancellationTokenSource.CreateLinkedTokenSource(ct))
                    {
                        fcts.CancelAfter(TimeSpan.FromSeconds(60));
                        frame = await Framing.ReadAsync(source, fcts.Token);
                    }
                    if (frame.Length == 0) break;
                    if (key != null) frame = CryptoHelper.AesDecrypt(key, frame);
                    if (frame.Length < 1 || !Enum.IsDefined(typeof(CompressAlgo), (int)frame[0]))
                        throw new InvalidDataException("Chunk có mã thuật toán nén không hợp lệ.");
                    var algo = (CompressAlgo)frame[0];
                    var raw = CompressionHelper.Decompress(algo, frame.AsSpan(1).ToArray());
                    total += raw.Length;
                    if (total > m.FileSize) throw new InvalidDataException("Dữ liệu vượt kích thước đã khai báo.");
                    await fs.WriteAsync(raw, ct);
                    sha.AppendData(raw);
                }
            }
            if (total != m.FileSize) throw new InvalidDataException("Dữ liệu nhận được thiếu so với khai báo.");
            hash = Convert.ToHexString(sha.GetHashAndReset());
            complete = true;
        }
        catch (Exception ex) when (ex is InvalidDataException or CryptographicException or FormatException)
        {
            Write($"Từ chối file của '{user}': {ex.Message}");
            try { await Framing.WriteJsonAsync(ns, Err(ex.Message), ct); } catch { }
            throw;
        }
        finally
        {
            if (!complete)
            {
                try { File.Delete(path); } catch { }
            }
            if (rcv != null)
            {
                // giữ phiên UDP thêm một lúc để còn xác nhận (ACK) gói FIN bị gửi lại
                var h = hub; var id = sid;
                _ = Task.Delay(TimeSpan.FromSeconds(15)).ContinueWith(_ => h.Unregister(id));
            }
        }

        Write($"Nhận xong '{safeName}' ({total:N0} byte, {(udp ? "UDP" : "TCP")}) từ '{user}'.");
        await Framing.WriteJsonAsync(ns, new Msg { Type = "DONE", Sha256 = hash, BytesSent = total }, ct);
    }
}
