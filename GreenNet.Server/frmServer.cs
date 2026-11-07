using GreenNet.Core;

namespace GreenNet.Server;

public partial class frmServer : Form
{
    private ServerEngine _engine;

    public frmServer()
    {
        InitializeComponent();
    }

    private void AppendLog(string line)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => AppendLog(line)));
            return;
        }
        rtbLog.AppendText(line + Environment.NewLine);
        rtbLog.ScrollToCaret();
    }

    private void SetRunning(bool running)
    {
        btnStart.Enabled = !running;
        btnStop.Enabled = running;
        txtName.Enabled = !running;
        nudPort.Enabled = !running;
        txtConn.Enabled = !running;
    }

    private async void btnStart_Click(object sender, EventArgs e)
    {
        var name = txtName.Text.Trim();
        var cs = txtConn.Text.Trim();
        if (name.Length == 0 || name.Length > 50)
        {
            MessageBox.Show(this, "Tên server phải có 1–50 ký tự.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (cs.Length == 0)
        {
            MessageBox.Show(this, "Vui lòng nhập chuỗi kết nối SQL.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        btnStart.Enabled = false;
        lblStatus.Text = "Đang kiểm tra kết nối CSDL…";
        try
        {
            await new Database(cs).PingAsync();
            _engine = new ServerEngine(name, (int)nudPort.Value, cs);
            _engine.Log += AppendLog;
            _engine.Start();
            txtFingerprint.Text = _engine.FingerprintText;
            SetRunning(true);
            lblStatus.Text = $"Đang chạy – TCP+UDP cổng {nudPort.Value}";
        }
        catch (Exception ex)
        {
            _engine?.Stop();
            _engine = null;
            SetRunning(false);
            lblStatus.Text = "Không khởi động được";
            MessageBox.Show(this, "Không khởi động được server:\n" + ex.Message, "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        _engine?.Stop();
        _engine = null;
        SetRunning(false);
        lblStatus.Text = "Đã dừng";
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        if (_engine == null) return;
        lblClients.Text = $"Client đang kết nối: {_engine.Clients}";
        lblEnergy.Text = $"Năng lượng đã ghi nhận: {_engine.EnergyJ:0.000} J";
        lblScore.Text = $"Green Score: {_engine.Score:0.0}";
    }

    private void frmServer_FormClosing(object sender, FormClosingEventArgs e) => _engine?.Stop();
}
