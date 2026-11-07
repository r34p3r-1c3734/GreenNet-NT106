using GreenNet.Core;

namespace GreenNet.Client;

public partial class frmMain : Form
{
    private CancellationTokenSource _cts;
    private bool _busy;
    private frmDashboard _dash;
    private frmHistory _hist;
    private frmCompare _cmp;

    public frmMain()
    {
        InitializeComponent();
        lblWelcome.Text = $"Xin chào, {ClientSession.Username}";
        cboProto.SelectedIndex = 0;
        cboAlgo.SelectedIndex = 0;
        Log("Sẵn sàng. Chọn file, cấu hình rồi nhấn Gửi file.");
        Log(EnergyModel.CalibrationInfo);
    }

    private void Log(string message)
    {
        rtbLog.AppendText($"[{DateTime.Now:HH:mm:ss}] {message}{Environment.NewLine}");
        rtbLog.ScrollToCaret();
    }

    private bool IsUdp => cboProto.Text == "UDP";

    private void UpdateControls()
    {
        btnSend.Enabled = !_busy;
        btnBrowse.Enabled = !_busy;
        cboProto.Enabled = !_busy;
        chkAuto.Enabled = !_busy;
        cboAlgo.Enabled = !_busy && !chkAuto.Checked;
        nudPower.Enabled = !_busy && chkAuto.Checked;
        chkEncrypt.Enabled = !_busy && !IsUdp;       // UDP luôn mã hóa AES
        btnCancel.Enabled = _busy;
    }

    private void cboProto_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (IsUdp) chkEncrypt.Checked = true;
        UpdateControls();
    }

    private void chkAuto_CheckedChanged(object sender, EventArgs e) => UpdateControls();

    private void btnBrowse_Click(object sender, EventArgs e)
    {
        if (openFileDialog.ShowDialog(this) == DialogResult.OK)
            txtFile.Text = openFileDialog.FileName;
    }

    private async void btnSend_Click(object sender, EventArgs e)
    {
        var path = txtFile.Text.Trim();
        if (path.Length == 0 || !File.Exists(path))
        {
            MessageBox.Show(this, "Vui lòng chọn một file hợp lệ.", "Thiếu file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!Enum.TryParse(cboAlgo.Text, out CompressAlgo algo)) algo = CompressAlgo.None;

        var opt = new SendOptions
        {
            Path = path,
            Algo = algo,
            Udp = IsUdp,
            Encrypt = IsUdp || chkEncrypt.Checked,
            Auto = chkAuto.Checked,
            PowerBudgetW = (double)nudPower.Value
        };

        _cts = new CancellationTokenSource();
        var ct = _cts.Token;
        var progress = new Progress<MetricSample>(OnProgress);
        _busy = true;
        UpdateControls();
        progressBar.Value = 0;
        lblProgress.Text = "Đang kết nối và xác thực khóa server…";
        string algoText = opt.Auto ? "Auto" : opt.Algo.ToString();
        Log($"Gửi {Path.GetFileName(path)} ({(opt.Udp ? "UDP" : "TCP")}, nén: {algoText}{(opt.Encrypt ? ", AES-256-GCM" : "")}" +
            $"{(opt.Auto && opt.PowerBudgetW > 0 ? $", ngưỡng {opt.PowerBudgetW:0} W" : "")})");

        try
        {
            var r = await Task.Run(() => FileSender.SendAsync(opt, progress, ct));
            ShowResult(r);
        }
        catch (OperationCanceledException)
        {
            lblProgress.Text = "Đã hủy";
            Log("Đã hủy phiên truyền.");
        }
        catch (Exception ex)
        {
            lblProgress.Text = "Lỗi";
            Log("Lỗi: " + ex.Message);
            MessageBox.Show(this, ex.Message, "Gửi file thất bại", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            _busy = false;
            UpdateControls();
            _cts.Dispose();
            _cts = null;
        }
    }

    private void btnCancel_Click(object sender, EventArgs e) => _cts?.Cancel();

    private void OnProgress(MetricSample s)
    {
        progressBar.Value = Math.Clamp((int)s.Percent, 0, 100);
        lblProgress.Text = $"Đang gửi… {s.Percent:0}% – {s.ThroughputKBps:0} KB/s";
        lblRtt.Text = $"RTT: {s.RttMs:0.0} ms";
        lblEnergy.Text = $"Năng lượng ước tính: {s.EnergyJ:0.000} J";
        lblAdaptive.Text = $"Nén: {s.Algo} | Công suất ước tính: {s.PowerW:0.0} W | Giảm tốc: {s.ThrottleMs} ms";
        if (s.Note != null) Log("Tự tối ưu: " + s.Note);
        MetricsHub.Publish(s);
    }

    private void ShowResult(RunResult r)
    {
        progressBar.Value = 100;
        lblProgress.Text = "Hoàn tất";
        lblRtt.Text = $"RTT: {r.RttMs:0.0} ms";
        lblEnergy.Text = $"Năng lượng ước tính: {r.EnergyJ:0.000} J";
        lblScore.Text = $"Green Score: {r.GreenScore:0.0}/100";
        lblRatio.Text = $"Tỷ lệ byte truyền: {r.BytesSent * 100.0 / r.FileSize:0.0}%";
        lblServer.Text = $"Server: {r.ServerName} ({r.Protocol})";
        lblIntegrity.ForeColor = r.Sha256Ok ? Color.SeaGreen : Color.Firebrick;
        lblIntegrity.Text = r.Sha256Ok ? "Toàn vẹn (SHA-256): khớp" : "Toàn vẹn (SHA-256): KHÔNG khớp";
        if (r.AdaptiveSummary != null) lblAdaptive.Text = "Đã dùng: " + r.AdaptiveSummary;

        Log($"Hoàn tất trong {r.DurationMs:0} ms, {r.BytesSent:N0} byte, {r.EnergyJ:0.000} J, Green Score {r.GreenScore:0.0}.");
        if (r.Protocol == "UDP") Log($"UDP: {r.Retransmissions} gói phải gửi lại.");
        if (r.AdaptiveSummary != null) Log("Phân bổ thuật toán nén: " + r.AdaptiveSummary);
        if (!r.Sha256Ok) Log("CẢNH BÁO: mã băm file nhận được không khớp.");
        if (!r.ReportSaved) Log("Cảnh báo: không lưu được lịch sử phiên vào CSDL.");

        MetricsHub.Publish(new MetricSample
        {
            Percent = 100,
            EnergyJ = r.EnergyJ,
            RttMs = r.RttMs,
            GreenScore = r.GreenScore,
            Bytes = r.BytesSent,
            Final = true
        });
    }

    private static void ShowSingle<T>(ref T form, Func<T> create, IWin32Window owner) where T : Form
    {
        if (form == null || form.IsDisposed)
        {
            form = create();
            form.Show(owner);
        }
        else
        {
            form.Activate();
        }
    }

    private void btnDashboard_Click(object sender, EventArgs e) => ShowSingle(ref _dash, () => new frmDashboard(), this);
    private void btnHistory_Click(object sender, EventArgs e) => ShowSingle(ref _hist, () => new frmHistory(), this);
    private void btnCompare_Click(object sender, EventArgs e) => ShowSingle(ref _cmp, () => new frmCompare(), this);

    private void btnLogout_Click(object sender, EventArgs e)
    {
        ClientSession.Username = null;
        ClientSession.Token = null;
        Application.Restart();
    }

    private void frmMain_FormClosing(object sender, FormClosingEventArgs e) => _cts?.Cancel();
}
