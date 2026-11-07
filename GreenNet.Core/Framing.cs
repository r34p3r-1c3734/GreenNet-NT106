using System.Buffers.Binary;
using System.Text.Json;

namespace GreenNet.Core;

/// <summary>Gói tin: 4 byte độ dài (big-endian) + dữ liệu. Độ dài 0 = gói kết thúc.</summary>
public static class Framing
{
    public const int MaxFrame = 16 * 1024 * 1024;

    public static async Task WriteAsync(Stream s, byte[] data, CancellationToken ct = default)
    {
        var buf = new byte[4 + data.Length];
        BinaryPrimitives.WriteInt32BigEndian(buf, data.Length);
        Buffer.BlockCopy(data, 0, buf, 4, data.Length);
        await s.WriteAsync(buf, ct);
        await s.FlushAsync(ct);
    }

    public static async Task<byte[]> ReadAsync(Stream s, CancellationToken ct = default)
    {
        var head = new byte[4];
        await s.ReadExactlyAsync(head, ct);
        int len = BinaryPrimitives.ReadInt32BigEndian(head);
        if (len < 0 || len > MaxFrame)
            throw new InvalidDataException("Kích thước gói tin không hợp lệ.");
        var data = new byte[len];
        if (len > 0) await s.ReadExactlyAsync(data, ct);
        return data;
    }

    public static Task WriteJsonAsync<T>(Stream s, T obj, CancellationToken ct = default)
        => WriteAsync(s, JsonSerializer.SerializeToUtf8Bytes(obj), ct);

    public static async Task<T> ReadJsonAsync<T>(Stream s, CancellationToken ct = default)
    {
        var bytes = await ReadAsync(s, ct);
        return JsonSerializer.Deserialize<T>(bytes)
               ?? throw new InvalidDataException("Gói JSON rỗng.");
    }
}
