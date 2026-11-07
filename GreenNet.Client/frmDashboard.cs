namespace GreenNet.Client;

public partial class frmDashboard : Form
{
    public frmDashboard()
    {
        InitializeComponent();
        MetricsHub.Sample += OnSample;
    }

    private void OnSample(MetricSample s)
    {
        if (IsDisposed || !IsHandleCreated) return;
        if (InvokeRequired)
        {
            BeginInvoke(new Action(() => OnSample(s)));
            return;
        }

        if (s.Final)
        {
            lblScore.Text = $"Green Score: {s.GreenScore:0.0}/100";
        }
        else
        {
            chartThroughput.AddPoint(s.ThroughputKBps);
            chartCpu.AddPoint(s.CpuPercent);
            chartEnergy.AddPoint(s.EnergyJ);
        }
        lblRtt.Text = $"RTT: {s.RttMs:0.0} ms";
        lblEnergy.Text = $"Năng lượng ước tính: {s.EnergyJ:0.000} J";
        if (!s.Final)
            lblAdaptive.Text = $"Nén hiện tại: {s.Algo} | Công suất ước tính: {s.PowerW:0.0} W | Giảm tốc: {s.ThrottleMs} ms" +
                               (s.Note != null ? $" | {s.Note}" : "");

        if (s.EnergyJ > (double)nudThreshold.Value)
        {
            lblAlert.ForeColor = Color.Firebrick;
            lblAlert.Text = "CẢNH BÁO: năng lượng vượt ngưỡng đã đặt";
        }
        else
        {
            lblAlert.ForeColor = Color.SeaGreen;
            lblAlert.Text = "Bình thường";
        }
    }

    private void btnClear_Click(object sender, EventArgs e)
    {
        chartThroughput.Clear();
        chartCpu.Clear();
        chartEnergy.Clear();
        lblRtt.Text = "RTT: --";
        lblEnergy.Text = "Năng lượng ước tính: --";
        lblScore.Text = "Green Score: --";
        lblAdaptive.Text = "Chưa có phiên truyền nào.";
        lblAlert.ForeColor = SystemColors.ControlText;
        lblAlert.Text = "Bình thường";
    }

    private void frmDashboard_FormClosed(object sender, FormClosedEventArgs e) => MetricsHub.Sample -= OnSample;
}
