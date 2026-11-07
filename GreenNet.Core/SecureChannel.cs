using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

namespace GreenNet.Core;

/// <summary>Vân tay (SHA-256) của chứng chỉ, dạng AA:BB:CC…</summary>
public static class Fingerprint
{
    public static string Of(X509Certificate cert)
    {
        var hash = SHA256.HashData(cert.GetRawCertData());
        return string.Join(":", hash.Select(b => b.ToString("X2")));
    }
}

/// <summary>
/// Kho khóa tin cậy kiểu SSH known_hosts: "host:port" -> vân tay chứng chỉ đã ghim.
/// Lưu tại %LOCALAPPDATA%\GreenNet\&lt;FileName&gt;.
/// </summary>
public static class TrustStore
{
    private static readonly object L = new();
    public static string FileName = "known_servers.json";

    private static string PathOf()
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GreenNet");
        Directory.CreateDirectory(dir);
        return Path.Combine(dir, FileName);
    }

    private static Dictionary<string, string> Load()
    {
        try
        {
            var p = PathOf();
            if (File.Exists(p))
                return JsonSerializer.Deserialize<Dictionary<string, string>>(File.ReadAllText(p)) ?? new();
        }
        catch { }
        return new();
    }

    public static bool TryGet(string key, out string fingerprint)
    {
        lock (L) return Load().TryGetValue(key, out fingerprint);
    }

    public static void Set(string key, string fingerprint)
    {
        lock (L)
        {
            var d = Load();
            d[key] = fingerprint;
            File.WriteAllText(PathOf(), JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true }));
        }
    }

    public static void Remove(string key)
    {
        lock (L)
        {
            var d = Load();
            if (d.Remove(key))
                File.WriteAllText(PathOf(), JsonSerializer.Serialize(d, new JsonSerializerOptions { WriteIndented = true }));
        }
    }
}

/// <summary>Danh tính cố định của server: chứng chỉ tự ký RSA-2048, tạo một lần và lưu lại.</summary>
public static class ServerIdentity
{
    public static X509Certificate2 LoadOrCreate(string serverName)
    {
        var dir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "GreenNet", "identity");
        Directory.CreateDirectory(dir);
        var safe = new string(serverName.Select(c => char.IsLetterOrDigit(c) ? c : '_').ToArray());
        var path = Path.Combine(dir, safe + ".pfx");

        if (!File.Exists(path))
        {
            using var rsa = RSA.Create(2048);
            var req = new CertificateRequest($"CN=GreenNet-{safe}", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            req.CertificateExtensions.Add(new X509KeyUsageExtension(
                X509KeyUsageFlags.DigitalSignature | X509KeyUsageFlags.KeyEncipherment, false));
            req.CertificateExtensions.Add(new X509EnhancedKeyUsageExtension(
                new OidCollection { new Oid("1.3.6.1.5.5.7.3.1") }, false));
            using var tmp = req.CreateSelfSigned(DateTimeOffset.UtcNow.AddDays(-1), DateTimeOffset.UtcNow.AddYears(5));
            File.WriteAllBytes(path, tmp.Export(X509ContentType.Pfx));
        }
        // nạp lại từ PFX để SslStream trên Windows dùng được khóa riêng
        return new X509Certificate2(path);
    }
}

/// <summary>
/// Kênh TLS 1.2/1.3 với xác thực khóa công khai bằng cách GHIM VÂN TAY chứng chỉ (không dùng CA).
/// Lần đầu: hỏi người dùng đối chiếu vân tay (TOFU). Các lần sau: vân tay phải trùng, nếu khác → từ chối.
/// </summary>
public static class SecureChannel
{
    /// <summary>(host:port, vân tay) → true nếu người dùng tin cậy. UI của Client gán hàm này.</summary>
    public static Func<string, string, bool> ConfirmNewServer;
    /// <summary>Tự ghim ở lần đầu (dùng cho Load Balancer; admin phải đối chiếu vân tay thủ công).</summary>
    public static bool AutoTrustOnFirstUse;
    public static Action<string> Info;

    public static async Task<SslStream> ClientAsync(Stream inner, string host, int port, CancellationToken ct)
    {
        string failure = null;
        var key = $"{host.ToLowerInvariant()}:{port}";
        var ssl = new SslStream(inner, false, (sender, cert, chain, errors) =>
        {
            if (cert == null)
            {
                failure = "Server không gửi chứng chỉ.";
                return false;
            }
            var fp = Fingerprint.Of(cert);
            if (TrustStore.TryGet(key, out var pinned))
            {
                if (string.Equals(pinned, fp, StringComparison.OrdinalIgnoreCase)) return true;
                failure = $"CẢNH BÁO BẢO MẬT: khóa của {key} đã thay đổi so với lần trước (có thể đang bị tấn công man-in-the-middle). " +
                          $"Vân tay mới: {fp}. Kết nối bị chặn.";
                return false;
            }
            bool ok = AutoTrustOnFirstUse || (ConfirmNewServer?.Invoke(key, fp) ?? false);
            if (!ok)
            {
                failure = "Bạn đã từ chối tin cậy khóa của server này.";
                return false;
            }
            TrustStore.Set(key, fp);
            Info?.Invoke($"Đã ghim khóa của {key}: {fp}");
            return true;
        });

        try
        {
            using var hs = CancellationTokenSource.CreateLinkedTokenSource(ct);
            hs.CancelAfter(TimeSpan.FromSeconds(30));   // đủ thời gian để người dùng đối chiếu vân tay
            await ssl.AuthenticateAsClientAsync(new SslClientAuthenticationOptions
            {
                TargetHost = host,
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            }, hs.Token);
            return ssl;
        }
        catch (AuthenticationException ex)
        {
            ssl.Dispose();
            throw new AuthenticationException(failure ?? ex.Message, ex);
        }
        catch
        {
            ssl.Dispose();
            throw;
        }
    }

    public static async Task<SslStream> ServerAsync(Stream inner, X509Certificate2 cert, CancellationToken ct)
    {
        var ssl = new SslStream(inner, false);
        try
        {
            using var hs = CancellationTokenSource.CreateLinkedTokenSource(ct);
            hs.CancelAfter(TimeSpan.FromSeconds(30));
            await ssl.AuthenticateAsServerAsync(new SslServerAuthenticationOptions
            {
                ServerCertificate = cert,
                ClientCertificateRequired = false,
                EnabledSslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13
            }, hs.Token);
            return ssl;
        }
        catch
        {
            ssl.Dispose();
            throw;
        }
    }
}
