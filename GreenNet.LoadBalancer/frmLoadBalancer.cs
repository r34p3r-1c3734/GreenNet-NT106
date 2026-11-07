using System.Net.Sockets;
using GreenNet.Core;

namespace GreenNet.LoadBalancer;

public partial class frmLoadBalancer : Form
{
    private readonly LbEngine _engine = new();

    public frmLoadBalancer()
    {
        InitializeComponent();
        _engine.Log += AppendLog;
        _engine.AddNode(new ServerNode { Name = "Server A", Host = "127.0.0.1", Port = 9001 });
        _engine.AddNode(new ServerNode { Name = "Server B", Host = "127.0.0.1", Port = 9002 });
        bindingSource1.DataSource = _engine.Nodes;
        dgvServers.DataSource = bindingSource1;
        GridHelper.Apply(dgvServers);
        if (dgvServers.Columns["LastError"] != null) dgvServers.Columns["LastError"].Visible = false;
        cboAlgo.SelectedIndex = (int)LbAlgo.EnergyAware;
        timer1.Start();
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

    private void cboAlgo_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (cboAlgo.SelectedIndex >= 0) _engine.Algo = (LbAlgo)cboAlgo.SelectedIndex;
    }

    private void btnStart_Click(object sender, EventArgs e)
    {
        try
        {
            _engine.Port = (int)nudPort.Value;
            _engine.Start();
            btnStart.Enabled = false;
            btnStop.Enabled = true;
            nudPort.Enabled = false;
            lblStatus.Text = $"Đang chạy – cổng {_engine.Port}";
        }
        catch (SocketException ex)
        {
            MessageBox.Show(this, $"Không mở được cổng {nudPort.Value}: {ex.Message}", "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void btnStop_Click(object sender, EventArgs e)
    {
        _engine.Stop();
        btnStart.Enabled = true;
        btnStop.Enabled = false;
        nudPort.Enabled = true;
        lblStatus.Text = "Đã dừng";
    }

    private void btnAdd_Click(object sender, EventArgs e)
    {
        var name = txtSrvName.Text.Trim();
        var host = txtSrvHost.Text.Trim();
        if (name.Length == 0 || name.Length > 50)
        {
            MessageBox.Show(this, "Tên server phải có 1–50 ký tự.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        if (!NetUtil.IsValidHost(host))
        {
            MessageBox.Show(this, "Địa chỉ IP/tên máy không hợp lệ.", "Thiếu thông tin", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        var node = new ServerNode { Name = name, Host = host, Port = (int)nudSrvPort.Value };
        if (!_engine.AddNode(node))
            MessageBox.Show(this, "Tên hoặc địa chỉ server này đã có trong danh sách.", "Trùng lặp", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    private void btnRemove_Click(object sender, EventArgs e)
    {
        if (bindingSource1.Current is ServerNode n) _engine.RemoveNode(n);
    }

    private void btnForget_Click(object sender, EventArgs e)
    {
        if (bindingSource1.Current is not ServerNode n) return;
        var ok = MessageBox.Show(this,
            $"Xóa khóa đã ghim của {n.Name} ({n.Host}:{n.Port})?\n\nChỉ làm khi bạn biết chắc server đã đổi khóa hợp lệ " +
            "(ví dụ cài lại). Khóa mới sẽ được ghim ở lần kiểm tra kế tiếp; hãy đối chiếu vân tay với màn hình frmServer.",
            "Quên khóa server", MessageBoxButtons.YesNo, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
        if (ok == DialogResult.Yes) LbEngine.ForgetKey(n);
    }

    private void timer1_Tick(object sender, EventArgs e)
    {
        bindingSource1.ResetBindings(false);
        var nodes = _engine.Snapshot();
        chartAssigned.SetBars(nodes.Select(n => n.Name).ToList(), nodes.Select(n => (double)n.Assigned).ToList());
    }

    private void frmLoadBalancer_FormClosing(object sender, FormClosingEventArgs e) => _engine.Stop();
}
