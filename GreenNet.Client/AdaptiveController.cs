using System.Diagnostics;
using GreenNet.Core;

namespace GreenNet.Client;

/// <summary>
/// Tự tối ưu trong lúc truyền:
///  1) Đổi thuật toán nén theo từng chunk: đo năng lượng ước tính trên mỗi byte của từng thuật toán
///     (CPU nén/mã hóa + năng lượng truyền số byte sau nén) rồi chọn cái thấp nhất; thỉnh thoảng thử lại.
///     Dữ liệu khó nén (ảnh, zip...) sẽ tự chuyển về None; dữ liệu văn bản sẽ nghiêng về Zstd/Brotli.
///  2) Giảm tốc độ gửi (throttle) khi công suất hoạt động ước tính vượt ngân sách PowerBudgetW;
///     khi vượt ngân sách còn ưu tiên thuật toán tốn ít CPU nhất.
/// Lưu ý: giảm tốc giúp giữ công suất đỉnh dưới ngưỡng (pin/nhiệt), không nhất thiết giảm tổng năng lượng.
/// </summary>
public class AdaptiveController
{
    private static readonly CompressAlgo[] All = { CompressAlgo.None, CompressAlgo.Gzip, CompressAlgo.Brotli, CompressAlgo.Zstd };
    private readonly double[] _energy = new double[4];   // J/byte đầu vào (EMA)
    private readonly double[] _cpu = new double[4];      // giây CPU/byte đầu vào (EMA)
    private readonly bool[] _seen = new bool[4];
    private int _chunk;
    private int _best = -1;
    private bool _overBudget;

    public double PowerBudgetW { get; }
    public int ThrottleMs { get; private set; }
    public CompressAlgo Current { get; private set; } = CompressAlgo.Zstd;
    public int[] Counts { get; } = new int[4];
    public event Action<string> Note;

    public AdaptiveController(double powerBudgetW) => PowerBudgetW = powerBudgetW;

    /// <summary>Chạy thử từng thuật toán một lần để tránh sai số do JIT/nạp thư viện ở các chunk đầu.</summary>
    public static void WarmUp()
    {
        var sample = new byte[16 * 1024];
        new Random(1).NextBytes(sample.AsSpan(0, 4096));
        foreach (var a in All)
            CompressionHelper.Compress(a, sample, sample.Length);
    }

    public CompressAlgo ChooseNext()
    {
        int idx;
        int unseen = Array.FindIndex(_seen, s => !s);
        if (unseen >= 0) idx = unseen;                                   // khám phá lần đầu
        else if (_chunk % 48 == 47) idx = (_chunk / 48) % 4;             // thử lại định kỳ
        else idx = ArgMin(_overBudget ? _cpu : _energy);                 // khai thác
        _chunk++;
        Current = All[idx];
        return Current;
    }

    private static int ArgMin(double[] v)
    {
        int b = 0;
        for (int i = 1; i < v.Length; i++) if (v[i] < v[b]) b = i;
        return b;
    }

    /// <param name="cpuSeconds">thời gian CPU dùng để nén và mã hóa chunk</param>
    public void Record(CompressAlgo algo, int inputBytes, int outputBytes, double cpuSeconds)
    {
        int i = (int)algo;
        double e = EnergyModel.CostPerInputByte(cpuSeconds, inputBytes, outputBytes);
        double c = cpuSeconds / Math.Max(inputBytes, 1);
        _energy[i] = _seen[i] ? 0.7 * _energy[i] + 0.3 * e : e;
        _cpu[i] = _seen[i] ? 0.7 * _cpu[i] + 0.3 * c : c;
        _seen[i] = true;
        Counts[i]++;

        if (_seen.All(s => s))
        {
            int best = ArgMin(_overBudget ? _cpu : _energy);
            if (best != _best)
            {
                string why = _overBudget
                    ? "vượt ngân sách công suất → ưu tiên CPU thấp nhất"
                    : $"ước tính {_energy[best] * 1048576.0:0.00} J/MB thấp nhất";
                Note?.Invoke(_best < 0
                    ? $"Tự chọn nén: {All[best]} ({why})"
                    : $"Đổi nén: {All[_best]} → {All[best]} ({why})");
                _best = best;
            }
        }
    }

    /// <summary>Cập nhật độ trễ giảm tốc theo công suất ước tính (W) của cửa sổ đo gần nhất.</summary>
    public int UpdateThrottle(double powerW)
    {
        if (PowerBudgetW <= 0)
        {
            ThrottleMs = 0;
            _overBudget = false;
            return 0;
        }
        int before = ThrottleMs;
        if (powerW > PowerBudgetW)
        {
            _overBudget = true;
            ThrottleMs = Math.Min(ThrottleMs == 0 ? 5 : (int)(ThrottleMs * 1.5) + 1, 500);
        }
        else if (powerW < PowerBudgetW * 0.8)
        {
            ThrottleMs = (int)(ThrottleMs * 0.7);
            if (ThrottleMs == 0) _overBudget = false;
        }
        if (before == 0 && ThrottleMs > 0)
            Note?.Invoke($"Công suất {powerW:0.0} W vượt ngưỡng {PowerBudgetW:0.0} W → bắt đầu giảm tốc");
        else if (before > 0 && ThrottleMs == 0)
            Note?.Invoke("Công suất đã xuống dưới ngưỡng → bỏ giảm tốc");
        return ThrottleMs;
    }

    public string Summary()
        => string.Join(", ", All.Select((a, i) => $"{a}: {Counts[i]}").Where((s, i) => Counts[i] > 0)) + " chunk";
}
