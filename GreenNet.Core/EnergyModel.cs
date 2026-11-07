using System.Text.Json;

namespace GreenNet.Core;

/// <summary>Nội dung file energy_calibration.json (kết quả hiệu chỉnh bằng phép đo thật).</summary>
public class EnergyCalibration
{
    public double CpuWatts { get; set; }
    public double JoulePerByte { get; set; }
    public double JoulePerPacket { get; set; }
    public double RefJoulePerMB { get; set; }
    public string Device { get; set; }
    public string MeasuredOn { get; set; }
    public string Method { get; set; }
    public double R2 { get; set; }
}

/// <summary>
/// Mô hình ước lượng năng lượng của MÁY ĐẦU CUỐI (CPU + giao diện mạng), không gồm mạng lõi/trung tâm dữ liệu:
///   E = P_cpu x t_cpu + E_byte x bytes + E_packet x packets
/// Giá trị mặc định chỉ là khởi đầu. Hãy hiệu chỉnh theo docs/HuongDan_HieuChinh_NangLuong.md
/// rồi đặt kết quả vào energy_calibration.json cạnh file .exe.
/// </summary>
public static class EnergyModel
{
    public static double CpuWatts = 15.0;          // W, phần công suất CPU tăng thêm so với nghỉ khi xử lý
    public static double JoulePerByte = 2.0e-7;    // J/byte (đã gồm chi phí theo gói nếu JoulePerPacket = 0)
    public static double JoulePerPacket = 1.0e-5;  // J/gói MSS ~1460 byte
    public static double RefJoulePerMB = 5.0;      // mốc quy đổi Green Score

    public static string CalibrationInfo { get; private set; } = "Mô hình năng lượng: giá trị mặc định (CHƯA hiệu chỉnh)";

    public static bool LoadCalibration(string path = null)
    {
        path ??= Path.Combine(AppContext.BaseDirectory, "energy_calibration.json");
        if (!File.Exists(path)) return false;

        var c = JsonSerializer.Deserialize<EnergyCalibration>(File.ReadAllText(path))
                ?? throw new InvalidDataException("energy_calibration.json rỗng.");
        static bool Pos(double v) => double.IsFinite(v) && v > 0;
        if (!Pos(c.CpuWatts) || !Pos(c.JoulePerByte) || !Pos(c.RefJoulePerMB) ||
            !double.IsFinite(c.JoulePerPacket) || c.JoulePerPacket < 0)
            throw new InvalidDataException("energy_calibration.json có giá trị không hợp lệ (phải là số dương hữu hạn).");

        CpuWatts = c.CpuWatts;
        JoulePerByte = c.JoulePerByte;
        JoulePerPacket = c.JoulePerPacket;
        RefJoulePerMB = c.RefJoulePerMB;
        CalibrationInfo = $"Mô hình năng lượng đã hiệu chỉnh: {c.Device} ({c.MeasuredOn}), phương pháp {c.Method}, R²={c.R2:0.000}";
        return true;
    }

    public static double Estimate(double cpuSeconds, long bytesOnWire)
    {
        double packets = Math.Ceiling(bytesOnWire / 1460.0);
        return CpuWatts * cpuSeconds + JoulePerByte * bytesOnWire + JoulePerPacket * packets;
    }

    /// <summary>Năng lượng ước tính trên mỗi byte dữ liệu gốc khi nén/mã hóa chunk (dùng cho tự chọn thuật toán nén).</summary>
    public static double CostPerInputByte(double cpuSeconds, int inputBytes, int outputBytes)
        => Estimate(cpuSeconds, outputBytes) / Math.Max(inputBytes, 1);

    /// <summary>Green Score 0–100: càng ít J/MB càng cao.</summary>
    public static double GreenScore(double energyJ, long fileSizeBytes)
    {
        double mb = Math.Max(fileSizeBytes / 1048576.0, 0.001);
        double perMb = energyJ / mb;
        return Math.Round(100.0 / (1.0 + perMb / RefJoulePerMB), 1);
    }
}
