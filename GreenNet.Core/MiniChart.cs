using System.Drawing.Drawing2D;

namespace GreenNet.Core;

public enum ChartMode { Line, Bar }

/// <summary>Biểu đồ GDI+ đơn giản (không cần thư viện ngoài): đường realtime hoặc cột so sánh.</summary>
public class MiniChart : Control
{
    private readonly List<double> _vals = new();
    private List<string> _labels = new();

    public ChartMode Mode { get; set; } = ChartMode.Line;
    public string Title { get; set; } = "";
    public string Unit { get; set; } = "";
    public int MaxPoints { get; set; } = 120;

    public MiniChart()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = SystemColors.Window;
        ForeColor = SystemColors.WindowText;
    }

    public void AddPoint(double v)
    {
        _vals.Add(v);
        while (_vals.Count > MaxPoints) _vals.RemoveAt(0);
        Invalidate();
    }

    public void SetBars(IList<string> labels, IList<double> values)
    {
        _labels = labels.ToList();
        _vals.Clear();
        _vals.AddRange(values);
        Invalidate();
    }

    public void Clear()
    {
        _vals.Clear();
        _labels.Clear();
        Invalidate();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.SmoothingMode = SmoothingMode.AntiAlias;
        g.Clear(BackColor);

        using var fore = new SolidBrush(ForeColor);
        using var axis = new Pen(Color.FromArgb(90, ForeColor));
        string title = Unit.Length > 0 ? $"{Title} ({Unit})" : Title;
        g.DrawString(title, Font, fore, 6, 4);

        int bottomPad = Mode == ChartMode.Bar ? 40 : 18;
        var r = new Rectangle(46, 28, Width - 58, Height - 28 - bottomPad);
        if (r.Width < 30 || r.Height < 30) return;
        g.DrawRectangle(axis, r);

        if (_vals.Count == 0)
        {
            g.DrawString("Chưa có dữ liệu", Font, fore, r.X + 8, r.Y + 8);
            return;
        }

        double max = Math.Max(_vals.Max(), 1e-9) * 1.1;
        g.DrawString(max.ToString("0.##"), Font, fore, 2, r.Y - 6);
        g.DrawString("0", Font, fore, 30, r.Bottom - 12);

        using var green = new SolidBrush(Color.SeaGreen);
        if (Mode == ChartMode.Line)
        {
            if (_vals.Count == 1)
            {
                g.FillEllipse(green, r.X + 2, (float)(r.Bottom - _vals[0] / max * r.Height) - 3, 6, 6);
                return;
            }
            var pts = new PointF[_vals.Count];
            for (int i = 0; i < _vals.Count; i++)
            {
                float x = r.X + (float)i / (_vals.Count - 1) * r.Width;
                float y = r.Bottom - (float)(_vals[i] / max) * r.Height;
                pts[i] = new PointF(x, y);
            }
            using var pen = new Pen(Color.SeaGreen, 2f);
            g.DrawLines(pen, pts);
        }
        else
        {
            float slot = (float)r.Width / _vals.Count;
            float bw = Math.Min(slot * 0.6f, 90f);
            using var sf = new StringFormat { Alignment = StringAlignment.Center, Trimming = StringTrimming.EllipsisCharacter };
            for (int i = 0; i < _vals.Count; i++)
            {
                float h = (float)(_vals[i] / max) * r.Height;
                float x = r.X + slot * i + (slot - bw) / 2;
                g.FillRectangle(green, x, r.Bottom - h, bw, h);
                var valRect = new RectangleF(r.X + slot * i, Math.Max(r.Y, r.Bottom - h - 16), slot, 16);
                g.DrawString(_vals[i].ToString("0.##"), Font, fore, valRect, sf);
                if (i < _labels.Count)
                    g.DrawString(_labels[i], Font, fore, new RectangleF(r.X + slot * i, r.Bottom + 4, slot, 32), sf);
            }
        }
    }
}
