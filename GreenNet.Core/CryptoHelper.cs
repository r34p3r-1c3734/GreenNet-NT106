using System.Security.Cryptography;

namespace GreenNet.Core;

public static class CryptoHelper
{
    private const int Iterations = 100_000;

    // ---- Mật khẩu: PBKDF2-SHA256 + salt ----
    public static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(16);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, HashAlgorithmName.SHA256, 32);
        return $"{Iterations}.{Convert.ToBase64String(salt)}.{Convert.ToBase64String(hash)}";
    }

    public static bool VerifyPassword(string password, string stored)
    {
        var p = stored.Split('.');
        if (p.Length != 3 || !int.TryParse(p[0], out int iter)) return false;
        var salt = Convert.FromBase64String(p[1]);
        var expected = Convert.FromBase64String(p[2]);
        var actual = Rfc2898DeriveBytes.Pbkdf2(password, salt, iter, HashAlgorithmName.SHA256, expected.Length);
        return CryptographicOperations.FixedTimeEquals(actual, expected);
    }

    // ---- AES-256-GCM: kết quả = nonce(12) | tag(16) | ciphertext ----
    public static byte[] NewAesKey() => RandomNumberGenerator.GetBytes(32);

    public static byte[] AesEncrypt(byte[] key, byte[] plain)
    {
        var nonce = RandomNumberGenerator.GetBytes(12);
        var tag = new byte[16];
        var cipher = new byte[plain.Length];
        using var aes = new AesGcm(key, 16);
        aes.Encrypt(nonce, plain, cipher, tag);
        var res = new byte[28 + cipher.Length];
        Buffer.BlockCopy(nonce, 0, res, 0, 12);
        Buffer.BlockCopy(tag, 0, res, 12, 16);
        Buffer.BlockCopy(cipher, 0, res, 28, cipher.Length);
        return res;
    }

    public static byte[] AesDecrypt(byte[] key, byte[] data)
    {
        if (data.Length < 28) throw new CryptographicException("Dữ liệu mã hóa không hợp lệ.");
        var nonce = data.AsSpan(0, 12);
        var tag = data.AsSpan(12, 16);
        var cipher = data.AsSpan(28);
        var plain = new byte[cipher.Length];
        using var aes = new AesGcm(key, 16);
        aes.Decrypt(nonce, cipher, tag, plain);
        return plain;
    }
}
