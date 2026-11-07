namespace GreenNet.Core;

/// <summary>Đặt tiêu đề tiếng Việt và định dạng số cho DataGridView.</summary>
public static class GridHelper
{
    private static readonly Dictionary<string, (string Header, string Format)> Map = new()
    {
        ["Id"] = ("ID", null),
        ["CreatedAt"] = ("Thời gian (UTC)", "yyyy-MM-dd HH:mm:ss"),
        ["ServerName"] = ("Server", null),
        ["FileName"] = ("File", null),
        ["FileSizeBytes"] = ("Kích thước (B)", "N0"),
        ["BytesSent"] = ("Byte đã truyền", "N0"),
        ["Algo"] = ("Nén", null),
        ["Protocol"] = ("Giao thức", null),
        ["Encrypted"] = ("Mã hóa", null),
        ["DurationMs"] = ("Thời gian (ms)", "N0"),
        ["RttMs"] = ("RTT (ms)", "0.0"),
        ["CpuMs"] = ("CPU (ms)", "N0"),
        ["EnergyJ"] = ("Năng lượng (J)", "0.000"),
        ["GreenScore"] = ("Green Score", "0.0"),
        ["Sha256Ok"] = ("Toàn vẹn", null),
        // bảng so sánh
        ["Config"] = ("Cấu hình", null),
        ["Runs"] = ("Số lần chạy", null),
        ["AvgEnergyPerMB"] = ("Năng lượng/MB (J)", "0.000"),
        ["AvgDurationMs"] = ("Thời gian TB (ms)", "N0"),
        ["AvgRatio"] = ("Tỷ lệ byte truyền", "0.00"),
        ["AvgScore"] = ("Green Score TB", "0.0"),
        ["SavingPercent"] = ("Tiết kiệm so với mốc (%)", "0.0"),
        // bảng server của Load Balancer
        ["Name"] = ("Tên", null),
        ["Host"] = ("IP", null),
        ["Port"] = ("Cổng", null),
        ["Online"] = ("Online", null),
        ["Clients"] = ("Client", null),
        ["Score"] = ("Green Score", "0.0"),
        ["Assigned"] = ("Đã phân phối", null),
        ["Fingerprint"] = ("Vân tay khóa (SHA-256)", null),
    };

    public static void Apply(DataGridView grid)
    {
        foreach (DataGridViewColumn col in grid.Columns)
        {
            if (!Map.TryGetValue(col.DataPropertyName, out var m)) continue;
            col.HeaderText = m.Header;
            if (m.Format != null) col.DefaultCellStyle.Format = m.Format;
        }
    }
}
