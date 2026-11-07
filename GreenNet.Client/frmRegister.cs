using System.Text.RegularExpressions;
using GreenNet.Core;

namespace GreenNet.Client;

public partial class frmRegister : Form
{
    private static readonly Regex UserRx = new("^[A-Za-z0-9_]{3,30}$");

    public string RegisteredUser { get; private set; }

    public frmRegister(string suggestedUser = "")
    {
        InitializeComponent();
        txtUser.Text = suggestedUser;
    }

    private void SetStatus(string text, bool error)
    {
        lblStatus.ForeColor = error ? Color.Firebrick : Color.SeaGreen;
        lblStatus.Text = text;
    }

    private async void btnRegister_Click(object sender, EventArgs e)
    {
        var user = txtUser.Text.Trim();
        var pass = txtPass.Text;

        if (!UserRx.IsMatch(user))
        {
            SetStatus("Tên đăng nhập phải có 3–30 ký tự: chữ, số hoặc dấu _.", true);
            txtUser.Focus();
            return;
        }
        if (pass.Length < 6 || pass.Length > 40)
        {
            SetStatus("Mật khẩu phải có từ 6 đến 40 ký tự.", true);
            txtPass.Focus();
            return;
        }
        if (pass != txtConfirm.Text)
        {
            SetStatus("Mật khẩu nhập lại không khớp.", true);
            txtConfirm.Focus();
            return;
        }

        btnRegister.Enabled = false;
        SetStatus("Đang đăng ký…", false);
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var r = await ClientSession.RequestAsync(new Msg { Type = "REGISTER", User = user, Pass = pass }, cts.Token);
            if (r.Type == "OK")
            {
                RegisteredUser = user;
                DialogResult = DialogResult.OK;
            }
            else
            {
                SetStatus(r.Text ?? "Đăng ký thất bại.", true);
            }
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
            btnRegister.Enabled = true;
        }
    }
}
