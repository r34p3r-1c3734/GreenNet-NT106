#!/usr/bin/env python3
"""
Hiệu chỉnh mô hình năng lượng GreenNet từ số đo thật.

  E_measured = CpuWatts * cpu_s + JoulePerByte * bytes        (E_measured = năng lượng TĂNG THÊM so với lúc nghỉ)

Đầu vào: file CSV, mỗi dòng là MỘT lần chạy, có các cột:
  cpu_ms      thời gian CPU của tiến trình (lấy từ cột CpuMs trong lịch sử/CSV xuất của frmHistory)
  bytes       số byte trên đường truyền (BytesSent trong lịch sử; với UDP nên dùng số byte thực tế nếu có)
  measured_j  năng lượng đo thật đã TRỪ phần nghỉ (J)
Hoặc thay measured_j bằng 2 cột total_j và duration_s kèm tham số --idle-w (công suất nghỉ, W):
  measured_j = total_j - idle_w * duration_s

Cách dùng:
  python fit_energy_model.py runs.csv --idle-w 8.2 --device "Dell Latitude, i5, Ethernet" --method "Smart plug 1 Hz" --out energy_calibration.json
"""
import argparse, csv, json, math, sys, datetime
import numpy as np

def load(path, idle_w):
    rows = list(csv.DictReader(open(path, encoding="utf-8-sig")))
    cpu, byt, e = [], [], []
    for i, r in enumerate(rows, 2):
        try:
            c = float(r["cpu_ms"]) / 1000.0
            b = float(r["bytes"])
            if "measured_j" in r and r["measured_j"].strip() != "":
                m = float(r["measured_j"])
            else:
                if idle_w is None:
                    sys.exit("Thiếu cột measured_j: hãy cung cấp total_j, duration_s và --idle-w.")
                m = float(r["total_j"]) - idle_w * float(r["duration_s"])
        except (KeyError, ValueError) as ex:
            sys.exit(f"Dòng {i} không hợp lệ ({ex}).")
        cpu.append(c); byt.append(b); e.append(m)
    return np.array(cpu), np.array(byt), np.array(e)

def fit(X, y):
    """Bình phương tối thiểu có ràng buộc hệ số >= 0 (dùng scipy.nnls nếu có)."""
    try:
        from scipy.optimize import nnls
        coef, _ = nnls(X, y)
        return coef
    except ImportError:
        coef, *_ = np.linalg.lstsq(X, y, rcond=None)
        if (coef < 0).any():
            print("CẢNH BÁO: hồi quy ra hệ số âm (cài scipy để ép không âm). Hãy kiểm tra lại dữ liệu đo.", file=sys.stderr)
        return np.clip(coef, 0, None)

def metrics(y, yhat):
    ss_res = float(np.sum((y - yhat) ** 2))
    ss_tot = float(np.sum((y - y.mean()) ** 2))
    r2 = 1 - ss_res / ss_tot if ss_tot > 0 else float("nan")
    rmse = math.sqrt(ss_res / len(y))
    mape = float(np.mean(np.abs((y - yhat) / y))) * 100 if (y != 0).all() else float("nan")
    return r2, rmse, mape

def main():
    ap = argparse.ArgumentParser(description=__doc__, formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("csv")
    ap.add_argument("--idle-w", type=float, default=None, help="công suất nghỉ đo được (W)")
    ap.add_argument("--device", default="(chưa ghi)")
    ap.add_argument("--method", default="(chưa ghi)")
    ap.add_argument("--out", default="energy_calibration.json")
    a = ap.parse_args()

    cpu, byt, e = load(a.csv, a.idle_w)
    n = len(e)
    if n < 8:
        sys.exit(f"Chỉ có {n} lần chạy. Cần tối thiểu ~8 (khuyến nghị 20+) với tải CPU và lượng byte thay đổi độc lập.")
    if np.corrcoef(cpu, byt)[0, 1] ** 2 > 0.95:
        print("CẢNH BÁO: cpu_s và bytes gần như tuyến tính với nhau → không tách được 2 hệ số. "
              "Hãy thêm các lần chạy CPU-cao/byte-thấp (nén mạnh) và CPU-thấp/byte-cao (None).", file=sys.stderr)

    X = np.column_stack([cpu, byt])
    coef = fit(X, e)
    yhat = X @ coef
    r2, rmse, mape = metrics(e, yhat)

    # Kiểm định chéo bỏ-một (leave-one-out): sai số trên dữ liệu KHÔNG dùng để khớp
    loo = []
    for i in range(n):
        m = np.ones(n, bool); m[i] = False
        c = fit(X[m], e[m])
        loo.append(abs(e[i] - X[i] @ c) / abs(e[i]) if e[i] != 0 else float("nan"))
    loo_mape = float(np.nanmean(loo)) * 100

    mb = np.maximum(byt / 1048576.0, 1e-3)
    ref = float(np.median(e / mb))

    print(f"Số lần chạy: {n}")
    print(f"CpuWatts      = {coef[0]:.4f} W")
    print(f"JoulePerByte  = {coef[1]:.4e} J/byte  ({coef[1]/8*1e9:.2f} nJ/bit)")
    print(f"R²            = {r2:.4f}")
    print(f"RMSE          = {rmse:.4f} J")
    print(f"MAPE (khớp)   = {mape:.1f} %")
    print(f"MAPE (LOO-CV) = {loo_mape:.1f} %   <-- con số trung thực để báo cáo")
    print(f"RefJoulePerMB = {ref:.4f} (trung vị J/MB đo được)")

    out = {
        "CpuWatts": round(float(coef[0]), 6),
        "JoulePerByte": float(f"{coef[1]:.6e}"),
        "JoulePerPacket": 0.0,   # chi phí theo gói đã gộp vào JoulePerByte (không tách được vì bytes ~ packets)
        "RefJoulePerMB": round(ref, 4),
        "Device": a.device,
        "MeasuredOn": datetime.date.today().isoformat(),
        "Method": a.method,
        "R2": round(r2, 4),
    }
    json.dump(out, open(a.out, "w", encoding="utf-8"), ensure_ascii=False, indent=2)
    print(f"Đã ghi {a.out}. Chép file này cạnh GreenNet.Client.exe (và đổi tên đúng là energy_calibration.json).")

if __name__ == "__main__":
    main()
