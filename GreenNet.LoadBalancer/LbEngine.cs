using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using System.Security.Authentication;
using GreenNet.Core;

namespace GreenNet.LoadBalancer;

public enum LbAlgo { RoundRobin, LeastConnections, EnergyAware }

public class ServerNode
{
    public string Name { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public bool Online { get; set; }
    public int Clients { get; set; }
    public double Score { get; set; } = 100;
    public int Assigned { get; set; }
    public string Fingerprint { get; set; } = "";
    public string LastError { get; set; } = "";
}

/// <summary>
/// Load Balancer: kiểm tra sức khỏe các server (STATUS qua TLS, ghim khóa theo kiểu TOFU)
/// và trả về địa chỉ server phù hợp cho client. Energy-aware chọn Score / (1 + Clients) lớn nhất.
/// </summary>
public class LbEngine
{
    private readonly object _pickLock = new();
    private TcpListener _listener;
    private Thread _thread;
    private CancellationTokenSource _cts;
    private int _rr = -1;

    public BindingList<ServerNode> Nodes { get; } = new();
    public LbAlgo Algo { get; set; } = LbAlgo.EnergyAware;
    public int Port { get; set; } = 9000;
    public bool IsRunning => _listener != null;
    public event Action<string> Log;

    public LbEngine()
    {
        TrustStore.FileName = "known_servers.lb.json";
        SecureChannel.AutoTrustOnFirstUse = true;
        SecureChannel.Info = Write;
    }

    private void Write(string msg) => Log?.Invoke($"[{DateTime.Now:HH:mm:ss}] {msg}");

    public bool AddNode(ServerNode n)
    {
        lock (_pickLock)
        {
            if (Nodes.Any(x => x.Name.Equals(n.Name, StringComparison.OrdinalIgnoreCase)
                            || (x.Host == n.Host && x.Port == n.Port)))
                return false;
            Nodes.Add(n);
            return true;
        }
    }

    public void RemoveNode(ServerNode n)
    {
        lock (_pickLock) Nodes.Remove(n);
    }

    public ServerNode[] Snapshot()
    {
        lock (_pickLock) return Nodes.ToArray();
    }

    /// <summary>Xóa vân tay đã ghim (dùng khi server đổi khóa hợp lệ).</summary>
    public static void ForgetKey(ServerNode n) => TrustStore.Remove($"{n.Host.ToLowerInvariant()}:{n.Port}");

    public void Start()
    {
        if (IsRunning) return;
        _cts = new CancellationTokenSource();
        var l = new TcpListener(IPAddress.Any, Port);
        l.Start();
        _listener = l;
        var ct = _cts.Token;
        _thread = new Thread(() => AcceptLoop(l, ct)) { IsBackground = true, Name = "LB-Accept" };
        _thread.Start();
        _ = Task.Run(() => HealthLoopAsync(ct));
        Write($"Load Balancer lắng nghe ở cổng {Port}.");
    }

    public void Stop()
    {
        if (_listener == null) return;
        _cts.Cancel();
        try { _listener.Stop(); } catch { }
        _listener = null;
        foreach (var n in Snapshot()) n.Online = false;
        Write("Đã dừng Load Balancer.");
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
            _ = Task.Run(() => HandleAsync(c, ct));
        }
    }

    private async Task HandleAsync(TcpClient tcp, CancellationToken ct)
    {
        try
        {
            using var client = tcp;
            var ns = client.GetStream();
            Msg m;
            using (var cts = CancellationTokenSource.CreateLinkedTokenSource(ct))
            {
                cts.CancelAfter(TimeSpan.FromSeconds(5));
                m = await Framing.ReadJsonAsync<Msg>(ns, cts.Token);
            }

            if (m.Type != "GETSERVER")
            {
                await Framing.WriteJsonAsync(ns, new Msg { Type = "ERR", Text = "Yêu cầu không hợp lệ." }, ct);
                return;
            }

            var node = Pick();
            if (node == null)
            {
                Write("Không có server nào online để phân phối.");
                await Framing.WriteJsonAsync(ns, new Msg { Type = "ERR", Text = "Hiện không có server khả dụng." }, ct);
                return;
            }
            await Framing.WriteJsonAsync(ns, new Msg { Type = "OK", Host = node.Host, Port = node.Port, Text = node.Name }, ct);
        }
        catch (OperationCanceledException) { }
        catch (EndOfStreamException) { }
        catch (IOException) { }
        catch (Exception ex)
        {
            Write($"Lỗi xử lý yêu cầu: {ex.Message}");
        }
    }

    private ServerNode Pick()
    {
        lock (_pickLock)
        {
            var online = Nodes.Where(n => n.Online).ToList();
            if (online.Count == 0) return null;

            ServerNode chosen = Algo switch
            {
                LbAlgo.RoundRobin => online[(_rr = (_rr + 1) % online.Count)],
                LbAlgo.LeastConnections => online.OrderBy(n => n.Clients).First(),
                _ => online.OrderByDescending(n => n.Score / (1.0 + n.Clients)).First()
            };
            chosen.Assigned++;
            chosen.Clients++;   // cộng tạm cho tới lần kiểm tra sức khỏe kế tiếp
            Write($"[{Algo}] -> {chosen.Name} ({chosen.Host}:{chosen.Port})");
            return chosen;
        }
    }

    private async Task HealthLoopAsync(CancellationToken ct)
    {
        try
        {
            using var timer = new PeriodicTimer(TimeSpan.FromSeconds(2));
            do
            {
                await Task.WhenAll(Snapshot().Select(n => CheckAsync(n, ct)));
            }
            while (await timer.WaitForNextTickAsync(ct));
        }
        catch (OperationCanceledException) { }
    }

    private async Task CheckAsync(ServerNode n, CancellationToken ct)
    {
        try
        {
            using var c = await NetUtil.ConnectAsync(n.Host, n.Port, 1500, ct);
            using var ssl = await SecureChannel.ClientAsync(c.GetStream(), n.Host, n.Port, ct);
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(2));
            await Framing.WriteJsonAsync(ssl, new Msg { Type = "STATUS" }, cts.Token);
            var r = await Framing.ReadJsonAsync<Msg>(ssl, cts.Token);
            if (r.Type != "OK") throw new InvalidDataException("Phản hồi STATUS không hợp lệ.");

            if (ssl.RemoteCertificate != null) n.Fingerprint = Fingerprint.Of(ssl.RemoteCertificate);
            n.Clients = Math.Max(0, r.Clients - 1);   // trừ chính kết nối kiểm tra này
            n.Score = r.Score;
            n.LastError = "";
            if (!n.Online) Write($"{n.Name} online.");
            n.Online = true;
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested)
        {
            throw;
        }
        catch (AuthenticationException ex)
        {
            if (n.LastError != ex.Message) Write($"{n.Name}: {ex.Message}");
            n.LastError = ex.Message;
            n.Online = false;
        }
        catch
        {
            if (n.Online) Write($"{n.Name} offline.");
            n.Online = false;
        }
    }
}
