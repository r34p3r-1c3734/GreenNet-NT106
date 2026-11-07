namespace GreenNet.Core;

/// <summary>
/// Thông điệp điều khiển (JSON) đi trong kênh TLS đã xác thực khóa.
/// Type: REGISTER, LOGIN, GETSERVER, STATUS, PING, PONG, SEND, DONE, REPORT, HISTORY, OK, ERR.
/// </summary>
public class Msg
{
    public string Type { get; set; }
    public string User { get; set; }
    public string Pass { get; set; }          // chỉ gửi trong kênh TLS; server băm PBKDF2 trước khi lưu
    public string Token { get; set; }
    public string Text { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string FileName { get; set; }
    public long FileSize { get; set; }
    public long BytesSent { get; set; }
    public string Algo { get; set; }          // None/Gzip/Brotli/Zstd hoặc Auto
    public string Protocol { get; set; }      // TCP hoặc UDP
    public bool Encrypted { get; set; }
    public string SessionKey { get; set; }    // khóa AES-256 (base64), gửi trong kênh TLS
    public ulong SessionId { get; set; }      // định danh phiên UDP (bí mật, cấp bởi server)
    public int UdpPort { get; set; }
    public string Sha256 { get; set; }
    public bool Ok { get; set; }
    public double DurationMs { get; set; }
    public double RttMs { get; set; }
    public double CpuMs { get; set; }
    public double EnergyJ { get; set; }
    public double Score { get; set; }
    public int Clients { get; set; }
    public List<SessionRow> Rows { get; set; }
}

public class SessionRow
{
    public int Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string ServerName { get; set; }
    public string FileName { get; set; }
    public long FileSizeBytes { get; set; }
    public long BytesSent { get; set; }
    public string Algo { get; set; }
    public string Protocol { get; set; }
    public bool Encrypted { get; set; }
    public double DurationMs { get; set; }
    public double RttMs { get; set; }
    public double CpuMs { get; set; }
    public double EnergyJ { get; set; }
    public double GreenScore { get; set; }
    public bool Sha256Ok { get; set; }
}
