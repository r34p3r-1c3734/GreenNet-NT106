using System.Net.Sockets;
using GreenNet.Core;

namespace GreenNet.Client;

/// <summary>Kết nối TLS tới server do Load Balancer chọn.</summary>
public sealed class ServerLink : IDisposable
{
    public TcpClient Tcp;
    public Stream Stream;      // SslStream đã xác thực khóa server
    public Msg Info;           // phản hồi của Load Balancer (Host, Port, Text = tên server)

    public void Dispose()
    {
        Stream?.Dispose();
        Tcp?.Dispose();
    }
}

/// <summary>Trạng thái phiên và các hàm gọi Load Balancer / Server dùng chung cho các Form.</summary>
public static class ClientSession
{
    public static string LbHost = "127.0.0.1";
    public static int LbPort = 9000;
    public static string Username;
    public static string Token;
    public static SynchronizationContext UiContext;

    /// <summary>Hỏi người dùng có tin cậy khóa của server lần đầu không (chạy trên luồng giao diện).</summary>
    public static bool ConfirmNewServer(string endpoint, string fingerprint)
    {
        bool ok = false;
        void Ask()
        {
            var text =
                $"Đây là lần đầu bạn kết nối tới {endpoint}.\n\n" +
                $"Vân tay khóa (SHA-256):\n{fingerprint}\n\n" +
                "Hãy so sánh với vân tay hiển thị trên màn hình frmServer của server đó.\n" +
                "Chỉ chọn Yes nếu hai chuỗi TRÙNG KHỚP hoàn toàn. Nếu không chắc, chọn No.";
            ok = MessageBox.Show(Form.ActiveForm, text, "Xác thực server lần đầu",
                     MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2) == DialogResult.Yes;
        }
        if (UiContext != null) UiContext.Send(_ => Ask(), null);
        else Ask();
        return ok;
    }

    /// <summary>Hỏi Load Balancer nên kết nối server nào. Kênh này không bí mật; khóa server được xác thực ở bước TLS.</summary>
    public static async Task<Msg> AskLoadBalancerAsync(CancellationToken ct = default)
    {
        using var c = await NetUtil.ConnectAsync(LbHost, LbPort, 3000, ct);
        var ns = c.GetStream();
        await Framing.WriteJsonAsync(ns, new Msg { Type = "GETSERVER" }, ct);
        var r = await Framing.ReadJsonAsync<Msg>(ns, ct);
        if (r.Type != "OK")
            throw new InvalidOperationException(r.Text ?? "Load Balancer không có server khả dụng.");
        return r;
    }

    public static async Task<ServerLink> ConnectToServerAsync(CancellationToken ct = default)
    {
        var srv = await AskLoadBalancerAsync(ct);
        var tcp = await NetUtil.ConnectAsync(srv.Host, srv.Port, 3000, ct);
        try
        {
            var ssl = await SecureChannel.ClientAsync(tcp.GetStream(), srv.Host, srv.Port, ct);
            return new ServerLink { Tcp = tcp, Stream = ssl, Info = srv };
        }
        catch
        {
            tcp.Dispose();
            throw;
        }
    }

    /// <summary>Gửi một yêu cầu đơn tới server (qua TLS đã ghim khóa) và đọc phản hồi.</summary>
    public static async Task<Msg> RequestAsync(Msg m, CancellationToken ct = default)
    {
        using var link = await ConnectToServerAsync(ct);
        await Framing.WriteJsonAsync(link.Stream, m, ct);
        return await Framing.ReadJsonAsync<Msg>(link.Stream, ct);
    }
}
