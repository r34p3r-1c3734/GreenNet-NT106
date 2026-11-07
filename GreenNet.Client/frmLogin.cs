using GreenNet.Core;

namespace GreenNet.Client;

public partial class frmLogin : Form
{
    public frmLogin()
    {
        InitializeComponent();
        ClientSession.UiContext = SynchronizationContext.Current;
    }

    private bool TryReadEndpoint(out string host, out int port)
    {
        host = txtLbHost.Text.Trim();
        port = 0;
        if (!NetUtil.IsValidHost(host))
        {
            SetStatus("Địa chỉ Load Balancer không hợp lệ.", true);
            txtLbHost.Focus();
            return false;
        }
        if (!NetUtil.TryParsePort(txtLbPort.Text.Trim(), out port))
        {
            SetStatus("Cổng phải là số từ 1 đến 65535.", true);
            txtLbPort.Focus();
            return false;
        }
        return true;
    }

    private void SetStatus(string text, bool error)
    {
        lblStatus.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        lblStatus.Text = text;
    }

    private void SetBusy(bool busy)
    {
        btnLogin.Enabled = !busy;
        btnRegister.Enabled = !busy;
        UseWaitCursor = busy;
    }

    private async void btnLogin_Click(object sender, EventArgs e)
    {
        if (!TryReadEndpoint(out var host, out var port)) return;
        var user = txtUser.Text.Trim();
        var pass = txtPass.Text;
        if (user.Length == 0 || pass.Length == 0)
        {
            SetStatus("Vui lòng nhập tên đăng nhập và mật khẩu.", true);
            return;
        }

        SetBusy(true);
        SetStatus("Đang đăng nhập…", false);
        try
        {
            ClientSession.LbHost = host;
            ClientSession.LbPort = port;
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var r = await ClientSession.RequestAsync(new Msg { Type = "LOGIN", User = user, Pass = pass }, cts.Token);
            if (r.Type != "OK")
            {
                SetStatus(r.Text ?? "Đăng nhập thất bại.", true);
                return;
            }
            ClientSession.Username = user;
            ClientSession.Token = r.Token;
            var main = new frmMain();
            main.FormClosed += (s, a) => Application.Exit();
            main.Show();
            Hide();
        }
        catch (OperationCanceledException)
        {
            SetStatus("Hết thời gian chờ phản hồi.", true);
        }
        catch (Exception ex)
        {
            SetStatus("Không kết nối được: " + ex.Message, true);
        }
        finally
        {
            SetBusy(false);
        }
    }

    private void btnRegister_Click(object sender, EventArgs e)
    {
        if (!TryReadEndpoint(out var host, out var port)) return;
        ClientSession.LbHost = host;
        ClientSession.LbPort = port;
        using var f = new frmRegister(txtUser.Text.Trim());
        if (f.ShowDialog(this) == DialogResult.OK)
        {
            txtUser.Text = f.RegisteredUser;
            txtPass.Clear();
            txtPass.Focus();
            SetStatus("Đăng ký thành công. Hãy đăng nhập.", false);
        }
    }
}
