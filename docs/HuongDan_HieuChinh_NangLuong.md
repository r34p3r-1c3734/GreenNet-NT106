# Hướng dẫn hiệu chỉnh mô hình năng lượng bằng phép đo thật và cách trích nguồn

Mục tiêu: thay các hằng số tạm trong `EnergyModel` (CpuWatts, JoulePerByte) bằng giá trị **do chính nhóm đo được**,
có sai số được báo cáo trung thực, để trả lời được câu hỏi "con số Joule này chính xác đến đâu?".

## 1. Xác định phạm vi (system boundary) trước khi đo

Mô hình của đồ án chỉ tính năng lượng **máy đầu cuối chạy Client**: phần CPU tăng thêm để nén/mã hóa/gửi và
phần giao diện mạng/ngăn xếp giao thức theo số byte. Nó **không** tính router, mạng lõi, trung tâm dữ liệu.

Điều này quan trọng khi so với tài liệu: các nghiên cứu về "điện năng trên mỗi GB truyền qua Internet" (ví dụ [1])
tính cho cả hạ tầng mạng, thường lớn hơn nhiều bậc so với chi phí ở một thiết bị đầu cuối. **Không đặt hai con số
này cạnh nhau như cùng một đại lượng**; hãy nêu rõ ranh giới hệ thống trong báo cáo.

Công thức:

```
E_tăng_thêm = P_cpu × t_cpu + e_byte × bytes
```
`e_byte` đã gồm cả chi phí theo gói (vì số gói ≈ bytes/1460 nên không tách riêng được; file mẫu đặt `JoulePerPacket = 0`).

## 2. Chọn cách đo

| Cách | Đo được gì | Ưu | Nhược |
|---|---|---|---|
| **A. Ổ cắm/đồng hồ đo điện** có ghi log ≥ 1 mẫu/giây | Công suất cả máy (W) | Đo thật ở ổ cắm, dễ làm, phù hợp sinh viên | Gồm cả màn hình, quạt, ổ đĩa; độ phân giải kém với phiên ngắn |
| **B. RAPL** (Intel; Linux `/sys/class/powercap`, công cụ đọc MSR trên Windows) | Năng lượng gói CPU/DRAM | Chi tiết, tần số cao [2] | Không gồm card mạng; cần kiểm tra CPU/HĐH của bạn có hỗ trợ; có sai số [2] |
| **C. Pin laptop** (rút sạc, đọc mức xả) | Năng lượng cả máy | Không cần thiết bị | Độ phân giải thấp, kém tin cậy, chỉ để kiểm tra thô |

Khuyến nghị: **A làm phép đo chính**, B (nếu có) để đối chiếu thành phần CPU. Ghi rõ model thiết bị đo và tần số lấy mẫu.

## 3. Quy trình đo

1. **Chuẩn bị:** cố định Ethernet hoặc Wi-Fi (không đổi giữa các lần), tắt cập nhật/antivirus quét, đóng ứng dụng nền,
   tắt màn hình hoặc đặt độ sáng cố định, cắm sạc (nếu đo bằng ổ cắm), chạy Server + Load Balancer trên **máy khác** qua LAN thật
   (không dùng localhost vì sẽ không có chi phí mạng thật).
2. **Đo công suất nghỉ `P_idle`:** để máy rảnh 5 phút với đúng cấu hình trên, lấy trung bình công suất (W).
3. **Thiết kế các lần chạy** sao cho CPU và byte **thay đổi độc lập** (nếu không, hồi quy không tách được hai hệ số):
   - 3 kích thước file (ví dụ 50, 200, 800 MB) × thuật toán (None, Gzip, Brotli, Zstd) × AES (tắt/bật).
   - Dùng cả file dễ nén (văn bản/CSV) lẫn khó nén (ZIP, video) để có cả "CPU cao – byte thấp" và "CPU thấp – byte cao".
   - Mỗi tổ hợp lặp ≥ 3–5 lần, **chạy theo thứ tự ngẫu nhiên**, bỏ lần đầu (làm nóng).
   - Mỗi lần chạy nên kéo dài ≥ 30–60 giây nếu dùng ổ cắm (để năng lượng vượt xa độ phân giải của thiết bị).
4. **Ghi cho mỗi lần chạy:** `total_j` (năng lượng cả máy trong lúc chạy, J) và `duration_s`.
   Nếu thiết bị chỉ cho log công suất: `total_j = Σ ((P_i + P_{i+1})/2) × Δt` (quy tắc hình thang).
   Từ ứng dụng lấy `CpuMs` và `BytesSent` (xuất CSV ở frmHistory).
5. **Tính năng lượng tăng thêm:** `measured_j = total_j − P_idle × duration_s`.

Tạo file CSV theo mẫu `tools/runs_template.csv`:

```
cpu_ms,bytes,measured_j
2300,104857600,38.4
...
```
(hoặc cung cấp `total_j`, `duration_s` rồi truyền `--idle-w`).

## 4. Hồi quy tìm hệ số

```
pip install numpy scipy
python tools/fit_energy_model.py runs.csv --idle-w 8.2 ^
       --device "Dell Latitude 5420, i5-1135G7, Ethernet 1 Gbps" ^
       --method "Smart plug 1 Hz, tru nang luong nghi, OLS" --out energy_calibration.json
```
Công cụ in `CpuWatts`, `JoulePerByte`, R², RMSE và **MAPE kiểm định chéo bỏ-một (LOO-CV)** – đây là sai số
trung thực nhất để đưa vào báo cáo vì tính trên dữ liệu không dùng để khớp. Cảnh báo "cpu_s và bytes gần tuyến tính"
nghĩa là thiết kế thí nghiệm chưa đủ đa dạng (xem bước 3).

Gợi ý đánh giá (kinh nghiệm, không phải chuẩn): R² > 0,9 và MAPE LOO-CV dưới ~10–15% là chấp nhận được cho một đồ án;
nếu cao hơn, hãy nêu nguyên nhân (nhiễu nền, độ phân giải thiết bị đo) thay vì che giấu.

## 5. Đưa vào ứng dụng và kiểm chứng

1. Chép `energy_calibration.json` cạnh `GreenNet.Client.exe`. Khi khởi động, nhật ký ở frmMain sẽ ghi
   "Mô hình năng lượng đã hiệu chỉnh: …". (File mẫu `energy_calibration.sample.json` được chép sẵn khi build.)
2. **Kiểm chứng độc lập:** chạy thêm 3 kịch bản **không dùng khi hồi quy**, đo bằng ổ cắm, rồi so sánh với giá trị ứng dụng báo.
   Báo cáo bảng: kịch bản – đo thật (J) – mô hình (J) – sai số %.
3. Lặp lại hiệu chỉnh khi đổi máy, card mạng hoặc loại kết nối: hệ số **không** chuyển được giữa các thiết bị.

## 6. Viết trong báo cáo

Nên có các ý sau:
- **Ranh giới hệ thống** (mục 1) và công thức.
- **Phương pháp đo:** thiết bị, tần số lấy mẫu, điều kiện cố định, cách trừ phần nghỉ, số lần lặp, thứ tự ngẫu nhiên.
- **Bảng hệ số** kèm R², RMSE, MAPE LOO-CV và kết quả kiểm chứng độc lập.
- **Kết luận theo hướng tương đối:** "Zstd tiết kiệm X% so với None trên file Y" (có độ lệch chuẩn), tránh khẳng định giá trị tuyệt đối.
- **Hạn chế (threats to validity):** thiết bị đo đo cả máy; `CpuMs` là CPU của cả tiến trình Client (gồm giao diện);
  chi phí TLS không tách riêng; với UDP, số byte thực tế được ước lượng (gồm header và gói gửi lại);
  hệ số chỉ đúng cho thiết bị và điều kiện đã đo; mô hình không gồm năng lượng nghỉ và hạ tầng mạng.

## 7. Cách trích nguồn

Nguyên tắc:
1. **Tham số của mô hình lấy từ phép đo của nhóm**, không chép hằng số từ bài báo đo trên phần cứng khác.
   Tài liệu dùng để (a) giải thích phương pháp, (b) đặt kết quả vào bối cảnh, (c) làm cơ sở thiết kế.
2. Mỗi con số lấy từ tài liệu phải kèm trích dẫn và **ghi đúng ranh giới hệ thống** của con số đó.
3. Mở từng tài liệu và đối chiếu lại số trang, DOI, số liệu trước khi nộp (danh sách dưới đây đã được tìm thấy trên
   các trang thư mục/nhà xuất bản, nhưng bạn nên tự kiểm tra lần cuối).

Danh sách gợi ý (định dạng IEEE):

[1] J. Aslan, K. Mayers, J. G. Koomey, and C. France, "Electricity intensity of Internet data transmission: Untangling the estimates," *J. Ind. Ecol.*, vol. 22, no. 4, pp. 785–798, 2018, doi: 10.1111/jiec.12630.
 → Dùng để nói về ranh giới hệ thống và vì sao con số theo GB của toàn mạng khác con số trên thiết bị đầu cuối.

[2] K. N. Khan, M. Hirki, T. Niemi, J. K. Nurminen, and Z. Ou, "RAPL in action: Experiences in using RAPL for power measurements," *ACM Trans. Model. Perform. Eval. Comput. Syst.*, vol. 3, no. 2, Art. 9, 2018, doi: 10.1145/3177754.
 → Dùng nếu bạn đo bằng RAPL: nêu ưu điểm và hạn chế của phương pháp.

[3] K. C. Barr and K. Asanović, "Energy-aware lossless data compression," *ACM Trans. Comput. Syst.*, vol. 24, no. 3, pp. 250–291, Aug. 2006.
 → Cơ sở cho ý tưởng chỉ nén khi tiết kiệm năng lượng (đánh đổi CPU và byte truyền) và chọn thuật toán nén theo ngữ cảnh. (Kiểm tra DOI trên trang ACM.)

[4] A. Carroll and G. Heiser, "An analysis of power consumption in a smartphone," in *Proc. USENIX Annu. Tech. Conf. (ATC)*, Boston, MA, USA, Jun. 2010, pp. 271–284.
 → Ví dụ cách phân rã công suất theo thành phần phần cứng và dùng mô hình tuyến tính theo mức sử dụng.

[5] E. Rescorla, "The Transport Layer Security (TLS) Protocol Version 1.3," IETF, RFC 8446, Aug. 2018.
[6] J. Postel, "User Datagram Protocol," IETF, RFC 768, Aug. 1980.
 → Cho phần bảo mật kênh điều khiển và giao thức UDP.

Cách viết trong văn bản: *"Chúng tôi đo công suất bằng ổ cắm đo điện có ghi log 1 Hz và trừ công suất nghỉ, tương tự cách
phân rã theo thành phần trong [4]; khác với các ước lượng cho toàn hạ tầng mạng [1], mô hình của chúng tôi chỉ tính thiết bị đầu cuối."*

## Danh sách kiểm tra trước khi nộp
- [ ] Nêu rõ ranh giới hệ thống và công thức
- [ ] Có công suất nghỉ, thiết bị đo, tần số lấy mẫu
- [ ] ≥ 20 lần chạy, thứ tự ngẫu nhiên, có lặp lại, CPU và byte thay đổi độc lập
- [ ] Bảng hệ số với R², RMSE, MAPE LOO-CV
- [ ] Kiểm chứng độc lập ≥ 3 kịch bản (đo thật so với mô hình)
- [ ] Mọi con số từ tài liệu có trích dẫn và ghi ranh giới hệ thống
- [ ] Phần hạn chế được nêu rõ
