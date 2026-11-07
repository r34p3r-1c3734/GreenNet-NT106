using System.ComponentModel;
using GreenNet.Core;

namespace GreenNet.Client;

public class GroupRow
{
    public string Config { get; set; }
    public int Runs { get; set; }
    public double AvgEnergyPerMB { get; set; }
    public double AvgDurationMs { get; set; }
    public double AvgRatio { get; set; }
    public double AvgScore { get; set; }
    public double SavingPercent { get; set; }
}

public partial class frmCompare : Form
{
    private const string AllFiles = "(Tất cả file)";
    private List<SessionRow> _all = new();
    private bool _loading;

    public frmCompare()
    {
        InitializeComponent();
        _loading = true;
        cboMetric.SelectedIndex = 0;
        cboFile.Items.Add(AllFiles);
        cboFile.SelectedIndex = 0;
        _loading = false;
    }

    private async void frmCompare_Load(object sender, EventArgs e) => await LoadAsync();

    private async void btnReload_Click(object sender, EventArgs e) => await LoadAsync();

    private void Filter_Changed(object sender, EventArgs e) => Recompute();

    private async Task LoadAsync()
    {
        btnReload.Enabled = false;
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var r = await ClientSession.RequestAsync(new Msg { Type = "HISTORY", Token = ClientSession.Token }, cts.Token);
            if (r.Type != "OK")
            {
                MessageBox.Show(this, r.Text ?? "Không tải được dữ liệu.", "So sánh", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            _all = r.Rows ?? new List<SessionRow>();

            _loading = true;
            var selected = cboFile.SelectedItem as string;
            cboFile.Items.Clear();
            cboFile.Items.Add(AllFiles);
            foreach (var name in _all.Select(x => x.FileName).Distinct().OrderBy(x => x))
                cboFile.Items.Add(name);
            int idx = selected == null ? 0 : cboFile.Items.IndexOf(selected);
            cboFile.SelectedIndex = idx < 0 ? 0 : idx;
            _loading = false;

            Recompute();
        }
        catch (Exception ex)
        {
            _loading = false;
            MessageBox.Show(this, "Không tải được dữ liệu: " + ex.Message, "So sánh", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnReload.Enabled = true;
        }
    }

    private void Recompute()
    {
        if (_loading) return;

        IEnumerable<SessionRow> rows = _all;
        var file = cboFile.SelectedItem as string;
        if (!string.IsNullOrEmpty(file) && file != AllFiles)
            rows = rows.Where(r => r.FileName == file);

        var groups = rows
            .GroupBy(r => $"{r.Algo}{(r.Encrypted ? " + AES" : "")} ({r.Protocol})")
            .Select(g => new GroupRow
            {
                Config = g.Key,
                Runs = g.Count(),
                AvgEnergyPerMB = g.Average(r => r.EnergyJ / Math.Max(r.FileSizeBytes / 1048576.0, 0.001)),
                AvgDurationMs = g.Average(r => r.DurationMs),
                AvgRatio = g.Average(r => (double)r.BytesSent / Math.Max(r.FileSizeBytes, 1)),
                AvgScore = g.Average(r => r.GreenScore)
            })
            .OrderBy(x => x.Config)
            .ToList();

        var baseline = groups.FirstOrDefault(g => g.Config == "None (TCP)") ?? groups.FirstOrDefault();
        foreach (var g in groups)
        {
            g.SavingPercent = baseline == null || baseline.AvgEnergyPerMB <= 0
                ? 0
                : (baseline.AvgEnergyPerMB - g.AvgEnergyPerMB) / baseline.AvgEnergyPerMB * 100.0;
        }

        dgv.DataSource = new BindingList<GroupRow>(groups);
        GridHelper.Apply(dgv);

        Func<GroupRow, double> pick = cboMetric.SelectedIndex switch
        {
            1 => x => x.AvgDurationMs,
            2 => x => x.AvgRatio,
            3 => x => x.AvgScore,
            _ => x => x.AvgEnergyPerMB
        };
        chart.Title = cboMetric.Text;
        chart.SetBars(groups.Select(x => x.Config).ToList(), groups.Select(pick).ToList());

        lblBaseline.Text = baseline == null
            ? "Chưa có dữ liệu. Hãy gửi file với nhiều cấu hình khác nhau rồi nhấn Tải lại."
            : $"Mốc so sánh: {baseline.Config}. Cột 'Tiết kiệm' = chênh lệch năng lượng/MB so với mốc. Nên chọn một file cụ thể để so sánh công bằng.";
    }
}
