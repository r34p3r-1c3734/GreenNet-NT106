using System.ComponentModel;
using System.Globalization;
using System.Text;
using GreenNet.Core;

namespace GreenNet.Client;

public partial class frmHistory : Form
{
    private List<SessionRow> _all = new();
    private List<SessionRow> _view = new();

    public frmHistory()
    {
        InitializeComponent();
        dtpFrom.Value = DateTime.Today.AddDays(-30);
        dtpTo.Value = DateTime.Today;
        dtpFrom.ValueChanged += (s, e) => ApplyFilter();
        dtpTo.ValueChanged += (s, e) => ApplyFilter();
    }

    private async void frmHistory_Load(object sender, EventArgs e) => await LoadAsync();

    private async void btnLoad_Click(object sender, EventArgs e) => await LoadAsync();

    private async Task LoadAsync()
    {
        btnLoad.Enabled = false;
        lblCount.Text = "Đang tải…";
        try
        {
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(15));
            var r = await ClientSession.RequestAsync(new Msg { Type = "HISTORY", Token = ClientSession.Token }, cts.Token);
            if (r.Type != "OK")
            {
                MessageBox.Show(this, r.Text ?? "Không tải được lịch sử.", "Lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                lblCount.Text = "Lỗi";
                return;
            }
            _all = r.Rows ?? new List<SessionRow>();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            lblCount.Text = "Lỗi";
            MessageBox.Show(this, "Không tải được lịch sử: " + ex.Message, "Lịch sử", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            btnLoad.Enabled = true;
        }
    }

    private void ApplyFilter()
    {
        var from = dtpFrom.Value.Date;
        var to = dtpTo.Value.Date.AddDays(1);
        _view = _all.Where(x => x.CreatedAt.ToLocalTime() >= from && x.CreatedAt.ToLocalTime() < to).ToList();
        dgv.DataSource = new BindingList<SessionRow>(_view);
        GridHelper.Apply(dgv);
        lblCount.Text = $"{_view.Count} phiên";
    }

    private void btnExport_Click(object sender, EventArgs e)
    {
        if (_view.Count == 0)
        {
            MessageBox.Show(this, "Không có dữ liệu để xuất.", "Xuất CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }
        saveFileDialog.FileName = $"greennet_history_{DateTime.Now:yyyyMMdd_HHmm}.csv";
        if (saveFileDialog.ShowDialog(this) != DialogResult.OK) return;

        try
        {
            var inv = CultureInfo.InvariantCulture;
            var sb = new StringBuilder();
            sb.AppendLine("Id,CreatedAtUtc,Server,File,FileSizeBytes,BytesSent,Algo,Protocol,Encrypted,DurationMs,RttMs,CpuMs,EnergyJ,GreenScore,Sha256Ok");
            foreach (var r in _view)
            {
                sb.AppendLine(string.Join(",",
                    r.Id.ToString(inv), r.CreatedAt.ToString("o", inv), Csv(r.ServerName), Csv(r.FileName),
                    r.FileSizeBytes.ToString(inv), r.BytesSent.ToString(inv), r.Algo, r.Protocol, r.Encrypted ? "1" : "0",
                    r.DurationMs.ToString("0.###", inv), r.RttMs.ToString("0.###", inv), r.CpuMs.ToString("0.###", inv),
                    r.EnergyJ.ToString("0.#####", inv), r.GreenScore.ToString("0.#", inv), r.Sha256Ok ? "1" : "0"));
            }
            File.WriteAllText(saveFileDialog.FileName, sb.ToString(), new UTF8Encoding(true));
            MessageBox.Show(this, "Đã xuất file CSV.", "Xuất CSV", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Không ghi được file: " + ex.Message, "Xuất CSV", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private static string Csv(string s)
    {
        s ??= "";
        // chống CSV injection khi mở bằng Excel
        if (s.Length > 0 && "=+-@".IndexOf(s[0]) >= 0) s = "'" + s;
        return "\"" + s.Replace("\"", "\"\"") + "\"";
    }
}
