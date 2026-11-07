namespace GreenNet.Client;

public class MetricSample
{
    public double Percent;
    public double ThroughputKBps;
    public double CpuPercent;
    public double EnergyJ;
    public double PowerW;         // công suất hoạt động ước tính trong cửa sổ đo
    public double RttMs;
    public double GreenScore;
    public long Bytes;
    public bool Final;
    public string Algo;           // thuật toán nén đang dùng
    public int ThrottleMs;        // độ trễ giảm tốc giữa các chunk
    public string Note;           // quyết định thích ứng mới (nếu có)
}

/// <summary>Kênh phát số liệu realtime từ frmMain sang frmDashboard.</summary>
public static class MetricsHub
{
    public static event Action<MetricSample> Sample;
    public static void Publish(MetricSample s) => Sample?.Invoke(s);
}
