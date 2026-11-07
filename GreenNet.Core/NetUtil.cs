using System.Net;
using System.Net.Sockets;

namespace GreenNet.Core;

public static class NetUtil
{
    public static async Task<TcpClient> ConnectAsync(string host, int port, int timeoutMs = 3000, CancellationToken ct = default)
    {
        var c = new TcpClient { NoDelay = true };
        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(timeoutMs);
            await c.ConnectAsync(host, port, cts.Token);
            return c;
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            c.Dispose();
            throw new TimeoutException($"Hết thời gian chờ kết nối tới {host}:{port}.");
        }
        catch
        {
            c.Dispose();
            throw;
        }
    }

    public static bool IsValidHost(string host)
        => !string.IsNullOrWhiteSpace(host)
           && (IPAddress.TryParse(host, out _) || Uri.CheckHostName(host) != UriHostNameType.Unknown);

    public static bool TryParsePort(string s, out int port)
        => int.TryParse(s, out port) && port >= 1 && port <= 65535;
}
