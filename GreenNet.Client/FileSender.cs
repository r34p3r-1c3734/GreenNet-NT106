using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using GreenNet.Core;

namespace GreenNet.Client;

public class SendOptions
{
    public string Path;
    public CompressAlgo Algo = CompressAlgo.None;
    public bool Encrypt;
    public bool Udp;
    public bool Auto;               // tự đổi thuật toán nén và giảm tốc
    public double PowerBudgetW;     // 0 = không giới hạn
}

public class RunResult
{
    public string ServerName;
    public string Protocol;
    public string AlgoLabel;
    public long FileSize;
    public long BytesSent;
    public long WireBytes;
    public double DurationMs;
    public double RttMs;
    public double CpuMs;
    public double EnergyJ;
    public double GreenScore;
    public long Retransmissions;
    public string AdaptiveSummary;
    public bool Sha256Ok;
    public bool ReportSaved;
}

/// <summary>Logic truyền file (không có giao diện): nén từng chunk, mã hóa tùy chọn, đo số liệu, tự thích ứng.</summary>
public static class FileSender
{
    public const int ChunkSize = 64 * 1024;

    public static async Task<RunResult> SendAsync(SendOptions opt, IProgress<MetricSample> progress, CancellationToken ct)
    {
        var fi = new FileInfo(opt.Path);
        if (!fi.Exists) throw new FileNotFoundException("Không tìm thấy file.", opt.Path);
        if (fi.Length == 0) throw new InvalidOperationException("File rỗng, không thể gửi.");
        if (opt.Udp && !opt.Encrypt) throw new InvalidOperationException("UDP bắt buộc bật mã hóa AES-256-GCM.");

        AdaptiveController ctrl = null;
        var notes = new ConcurrentQueue<string>();
        if (opt.Auto)
        {
            AdaptiveController.WarmUp();
            ctrl = new AdaptiveController(opt.PowerBudgetW);
            ctrl.Note += notes.Enqueue;
        }

        using var link = await ClientSession.ConnectToServerAsync(ct);
        var ns = link.Stream;
        string host = link.Info.Host;
        string serverName = link.Info.Text;

        double rtt = await MeasureRttAsync(ns, ct);

        byte[] key = opt.Encrypt ? CryptoHelper.NewAesKey() : null;
        string algoLabel = opt.Auto ? "Auto" : opt.Algo.ToString();
        await Framing.WriteJsonAsync(ns, new Msg
        {
            Type = "SEND",
            Token = ClientSession.Token,
            FileName = fi.Name,
            FileSize = fi.Length,
            Algo = algoLabel,
            Protocol = opt.Udp ? "UDP" : "TCP",
            Encrypted = opt.Encrypt,
            SessionKey = key == null ? null : Convert.ToBase64String(key)
        }, ct);
        var reply = await Framing.ReadJsonAsync<Msg>(ns, ct);
        if (reply.Type != "OK") throw new InvalidOperationException(reply.Text ?? "Server từ chối yêu cầu.");

        Stream data = ns;
        RudpSender udp = null;
        try
        {
            if (opt.Udp)
            {
                var ip = await ResolveIPv4Async(host, ct);
                udp = new RudpSender(new IPEndPoint(ip, reply.UdpPort), reply.SessionId);
                data = udp;
            }

            using var proc = Process.GetCurrentProcess();
            var cpu0 = proc.TotalProcessorTime;
            var cpuLast = cpu0;
            var sw = Stopwatch.StartNew();
            var lastTick = TimeSpan.Zero;
            long sent = 0, lastWire = 0, read = 0;
            double lastEnergy = 0;
            using var sha = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            var buf = new byte[ChunkSize];
            string curAlgo = opt.Auto ? "(đang thăm dò)" : opt.Algo.ToString();

            long Wire() => udp != null ? udp.WireBytes : sent;

            using (var fs = new FileStream(opt.Path, FileMode.Open, FileAccess.Read, FileShare.Read, 81920, true))
            {
                int n;
                while ((n = await fs.ReadAsync(buf.AsMemory(0, buf.Length), ct)) > 0)
                {
                    sha.AppendData(buf, 0, n);

                    var algo = ctrl != null ? ctrl.ChooseNext() : opt.Algo;
                    long t0 = Stopwatch.GetTimestamp();
                    var comp = CompressionHelper.Compress(algo, buf, n);
                    var payload = new byte[comp.Length + 1];
                    payload[0] = (byte)algo;                         // gắn thuật toán vào từng chunk
                    Buffer.BlockCopy(comp, 0, payload, 1, comp.Length);
                    if (key != null) payload = CryptoHelper.AesEncrypt(key, payload);
                    double cpuSec = Stopwatch.GetElapsedTime(t0).TotalSeconds;
                    ctrl?.Record(algo, n, payload.Length, cpuSec);
                    curAlgo = algo.ToString();

                    await Framing.WriteAsync(data, payload, ct);
                    sent += payload.Length + 4;
                    read += n;

                    var now = sw.Elapsed;
                    if (now - lastTick >= TimeSpan.FromMilliseconds(250))
                    {
                        var cpuNow = proc.TotalProcessorTime;
                        double dt = (now - lastTick).TotalSeconds;
                        long wire = Wire();
                        double energyNow = EnergyModel.Estimate((cpuNow - cpu0).TotalSeconds, wire);
                        double powerW = (energyNow - lastEnergy) / dt;
                        int throttle = ctrl?.UpdateThrottle(powerW) ?? 0;

                        var noteList = new List<string>();
                        while (notes.TryDequeue(out var nt)) noteList.Add(nt);

                        progress?.Report(new MetricSample
                        {
                            Percent = read * 100.0 / fi.Length,
                            ThroughputKBps = (wire - lastWire) / 1024.0 / dt,
                            CpuPercent = (cpuNow - cpuLast).TotalSeconds / dt / Environment.ProcessorCount * 100.0,
                            EnergyJ = energyNow,
                            PowerW = powerW,
                            RttMs = rtt,
                            Bytes = sent,
                            Algo = curAlgo,
                            ThrottleMs = throttle,
                            Note = noteList.Count > 0 ? string.Join("; ", noteList) : null
                        });
                        lastTick = now;
                        lastWire = wire;
                        lastEnergy = energyNow;
                        cpuLast = cpuNow;
                    }

                    if (ctrl != null && ctrl.ThrottleMs > 0)
                        await Task.Delay(ctrl.ThrottleMs, ct);
                }
            }

            await Framing.WriteAsync(data, Array.Empty<byte>(), ct);
            sent += 4;
            if (udp != null) await udp.CompleteAsync(ct);

            var done = await Framing.ReadJsonAsync<Msg>(ns, ct);
            if (done.Type != "DONE") throw new InvalidOperationException(done.Text ?? "Server báo lỗi khi nhận file.");
            sw.Stop();

            string localHash = Convert.ToHexString(sha.GetHashAndReset());
            bool ok = string.Equals(done.Sha256, localHash, StringComparison.OrdinalIgnoreCase);
            double cpuSecTotal = (proc.TotalProcessorTime - cpu0).TotalSeconds;
            long wireTotal = Wire();
            double energy = EnergyModel.Estimate(cpuSecTotal, wireTotal);
            double score = EnergyModel.GreenScore(energy, fi.Length);

            var result = new RunResult
            {
                ServerName = serverName,
                Protocol = opt.Udp ? "UDP" : "TCP",
                AlgoLabel = algoLabel,
                FileSize = fi.Length,
                BytesSent = sent,
                WireBytes = wireTotal,
                DurationMs = sw.Elapsed.TotalMilliseconds,
                RttMs = rtt,
                CpuMs = cpuSecTotal * 1000.0,
                EnergyJ = energy,
                GreenScore = score,
                Retransmissions = udp?.Retransmissions ?? 0,
                AdaptiveSummary = ctrl?.Summary(),
                Sha256Ok = ok
            };

            try
            {
                await Framing.WriteJsonAsync(ns, new Msg
                {
                    Type = "REPORT",
                    Token = ClientSession.Token,
                    FileName = fi.Name,
                    FileSize = fi.Length,
                    BytesSent = sent,
                    Algo = algoLabel,
                    Protocol = result.Protocol,
                    Encrypted = opt.Encrypt,
                    DurationMs = result.DurationMs,
                    RttMs = rtt,
                    CpuMs = result.CpuMs,
                    EnergyJ = energy,
                    Score = score,
                    Ok = ok
                }, ct);
                var rep = await Framing.ReadJsonAsync<Msg>(ns, ct);
                result.ReportSaved = rep.Type == "OK";
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                result.ReportSaved = false;
            }
            return result;
        }
        finally
        {
            udp?.Dispose();
        }
    }

    private static async Task<IPAddress> ResolveIPv4Async(string host, CancellationToken ct)
    {
        if (IPAddress.TryParse(host, out var ip))
        {
            if (ip.AddressFamily != AddressFamily.InterNetwork)
                throw new NotSupportedException("UDP hiện chỉ hỗ trợ địa chỉ IPv4.");
            return ip;
        }
        var all = await Dns.GetHostAddressesAsync(host, ct);
        return all.FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)
               ?? throw new NotSupportedException("Không phân giải được địa chỉ IPv4 của server.");
    }

    private static async Task<double> MeasureRttAsync(Stream ns, CancellationToken ct)
    {
        const int rounds = 3;
        double total = 0;
        for (int i = 0; i < rounds; i++)
        {
            var sw = Stopwatch.StartNew();
            await Framing.WriteJsonAsync(ns, new Msg { Type = "PING" }, ct);
            var r = await Framing.ReadJsonAsync<Msg>(ns, ct);
            sw.Stop();
            if (r.Type != "PONG") throw new InvalidOperationException("Phản hồi PING không hợp lệ.");
            total += sw.Elapsed.TotalMilliseconds;
        }
        return total / rounds;
    }
}
