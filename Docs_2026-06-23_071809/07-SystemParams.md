# 07 — Phân tích SystemParams.xml (từ điển tham số hệ thống)

`SystemParams.xml` là **từ điển tham số** lớn nhất của server: **939 tham số** (`<Param>`),
trong đó **914 tham số có chú thích tiếng Trung** đi kèm, và **173 tham số được mã nguồn gọi trực tiếp**
qua hàm `GetParamValueByName(...)`. Đây là nơi tinh chỉnh hành vi của **rất nhiều chức năng** mà không cần
sửa code.

## 7.1. Cấu trúc
```xml
<Config>
  <Params>
    <!--MU không cánh: hệ số cộng tấn công theo cấp cường hóa, phải 16 mục...-->
    <Param Name="ForgeLevelAddAttackRates" Value="0,0.05,0.1,0.15,0.2,..."/>
    <Param Name="ForgeGoodsRate" Value="0,100,100,90,80,70,..."/>
    ...
  </Params>
</Config>
```
- Mỗi `<Param>` gồm `Name` (khóa) và `Value` (giá trị, thường là **danh sách** ngăn bằng dấu `,` hoặc `|`).
- Comment `<!-- ... -->` ngay phía trên mô tả ý nghĩa và quy tắc (số mục bắt buộc, đơn vị, %, ...).
- **Lưu ý:** nhiều tham số yêu cầu **đúng số phần tử** (vd "phải 16 mục", "phải 80 mục"); nhập sai số
  phần tử có thể gây lỗi khi server tính toán. Khi sửa, đọc kỹ chú thích.

## 7.2. Server đọc tham số như thế nào
Giá trị được truy xuất qua:
```csharp
string v = GameManager.systemParamsList.GetParamValueByName("TenThamSo");
```
Ví dụ thực tế trong `Logic/ActivityNew/SpecialActivity.cs`:
```csharp
string paramValueByName = GameManager.systemParamsList.GetParamValueByName("SpecialChongZhiDuiHuan");
```
Có **173** tham số được gọi kiểu này rải khắp các manager (cường hóa, đổi nạp, đào, thờ cúng...).
Phần còn lại được nạp theo cụm trong các hệ thống tương ứng.

## 7.3. Các họ tham số lớn (theo tiền tố)
| Tiền tố | Số lượng | Chức năng liên quan |
|---|---|---|
| `Manor*` | 22 | Trang viên / Lãnh địa |
| `Zhan*` | 21 | Chiến đấu / Chiến trường / Đội |
| `Comp*` | 21 | Liên minh (Comp) |
| `Horse*` | 20 | Ngựa / Tọa kỵ |
| `Crusade*` | 19 | Thập tự quân (liên server) |
| `Max*` | 14 | Giới hạn tối đa các hệ thống |
| `Daily*` | 13 | Hoạt động/độ tích cực hằng ngày |
| `Red*` | 13 | Hồng bao / điểm đỏ |
| `Forge*` | 12 | **Cường hóa** trang bị |
| `Jing*` | 12 | Kinh mạch / Đấu trường (JingJi) |
| `Equip*` | 11 | Trang bị |
| `Temple*` | 11 | Ảo ảnh tự viện / Đền |
| `ZhuiJia*` | 10 | **Truy gia** thuộc tính |
| `Ling*` | 10 | Linh ngọc / Linh khí |
| `Yang*` | 10 | (Dưỡng thành...) |
| `Fu*` | 10 | Phù văn / Phụ ma |
| `Team*` | 10 | Tổ đội / Đấu đội |
| `League*` | 9 | Giải đấu bang hội |
| `Wing*` | 7 | **Cánh** (cường hóa/truy gia cánh) |

## 7.4. Một số nhóm tham số quan trọng (ví dụ)
- **Cường hóa (Forge):** `ForgeLevelAddAttackRates`, `ForgeLevelAddDefenseRates`, `ForgeGoodsRate`
  (tỉ lệ thành công theo cấp), `ForgeNeedGoodsIDs`/`ForgeNeedGoodsNum` (vật phẩm cần),
  `ForgeProtectStoneGoodsIDS` (đá bảo vệ), `ForgeLevelNeedYinLiang` (tiền tiêu hao).
- **Truy gia (ZhuiJia):** `ZhuiJiaLevelAddAttackRates`, `ZhuiJiaGoodsRate`, `ZhuiJiaForgeGoodsIDs`,
  `ZhuiJiaXiaoHaoJinBi` (80 mục tương ứng 80 cấp truy gia).
- **Truyền thừa (ChuanCheng):** `ChuanChengGoodsRate`, `ChuanChengXiaoHaoJinBi`, `ChuanChengXiaoHaoZhuanShi`.
- **Thờ cúng (MoBai):** `MoBaiNumber`, `JiBiMoBai`, `ZuanShiMoBai` và biến thể `LuoLan*`.
- **Đổi nạp đặc biệt:** `SpecialChongZhiDuiHuan` (dùng trong hoạt động đặc biệt).
- **Dịch chuyển thế giới:** `ShiJieChuanSong`.

> Đây chỉ là phần nổi bật; toàn bộ 939 tham số đều xem/sửa được trong web app.

## 7.5. Quản lý trong web app
- Mở thẻ **"Tham số hệ thống (SystemParams)"** trên trang chủ.
- Danh sách hiển thị `Name`, `Value` và **chú thích gốc** (cột "Chú thích").
- Có **ô tìm kiếm** theo tên/giá trị/chú thích — rất cần thiết với 939 dòng.
- Khi sửa: trang Edit hiện **"Chú thích gốc"** để bạn hiểu ý nghĩa trước khi đổi `Value`.
- **An toàn:** lưu giữ nguyên định dạng gốc (tab thụt lề, BOM UTF-8) và **toàn bộ comment** —
  chỉ thay đúng giá trị bạn sửa; file gốc được sao lưu trước khi ghi đè.
- SystemParams là từ điển cấu hình nên **không có bật/tắt** (chỉ Xem/Sửa).

> Cảnh báo: nhiều tham số yêu cầu đúng số phần tử và đúng đơn vị (xem chú thích). Sửa sai có thể khiến
> chức năng liên quan tính toán lỗi. Luôn kiểm thử trên server test trước.
