using System.IO.Compression;

namespace GreenNet.Core;

public enum CompressAlgo { None, Gzip, Brotli, Zstd }

public static class CompressionHelper
{
    private const int MaxChunkOut = 4 * 1024 * 1024;   // chặn "zip bomb" cho mỗi chunk

    public static byte[] Compress(CompressAlgo algo, byte[] buf, int count)
    {
        switch (algo)
        {
            case CompressAlgo.None:
                return buf.AsSpan(0, count).ToArray();
            case CompressAlgo.Gzip:
            {
                using var ms = new MemoryStream();
                using (var z = new GZipStream(ms, CompressionLevel.Optimal, true)) z.Write(buf, 0, count);
                return ms.ToArray();
            }
            case CompressAlgo.Brotli:
            {
                using var ms = new MemoryStream();
                using (var z = new BrotliStream(ms, CompressionLevel.Optimal, true)) z.Write(buf, 0, count);
                return ms.ToArray();
            }
            case CompressAlgo.Zstd:
            {
                using var c = new ZstdSharp.Compressor(3);
                return c.Wrap(buf.AsSpan(0, count)).ToArray();
            }
            default:
                throw new NotSupportedException("Thuật toán nén không được hỗ trợ.");
        }
    }

    public static byte[] Decompress(CompressAlgo algo, byte[] data)
    {
        switch (algo)
        {
            case CompressAlgo.None:
                return data;
            case CompressAlgo.Gzip:
            {
                using var z = new GZipStream(new MemoryStream(data), CompressionMode.Decompress);
                return ReadLimited(z);
            }
            case CompressAlgo.Brotli:
            {
                using var z = new BrotliStream(new MemoryStream(data), CompressionMode.Decompress);
                return ReadLimited(z);
            }
            case CompressAlgo.Zstd:
            {
                using var d = new ZstdSharp.Decompressor();
                return d.Unwrap(data, MaxChunkOut).ToArray();
            }
            default:
                throw new NotSupportedException("Thuật toán nén không được hỗ trợ.");
        }
    }

    private static byte[] ReadLimited(Stream src)
    {
        using var output = new MemoryStream();
        var tmp = new byte[16 * 1024];
        int n;
        while ((n = src.Read(tmp, 0, tmp.Length)) > 0)
        {
            if (output.Length + n > MaxChunkOut) throw new InvalidDataException("Dữ liệu giải nén vượt giới hạn.");
            output.Write(tmp, 0, n);
        }
        return output.ToArray();
    }
}
