# 03 — Cơ chế BẬT/TẮT sự kiện

Sau khi phân tích, có **3 cơ chế** bật/tắt. Web app tự gán đúng cơ chế cho từng file.

## 3.1. Flag — Cờ bật/tắt sẵn có
Một số file có thuộc tính boolean trực tiếp:
- `VersionSystemOpen.xml` → `IsOpen="1"` (mở) / `"0"` (đóng)
- `ThemeActivityOpen.xml` → `Open="1"` / `"0"`

**Bật/Tắt = đổi giá trị thuộc tính**. Không mất dữ liệu, an toàn nhất.

## 3.2. DateWindow — Khung thời gian
Sự kiện theo đợt được điều khiển bằng cặp ngày bắt đầu/kết thúc:
- `SpecialActivity/SpecialActivityTime.xml` → `FromDate` / `ToDate`
- `JieRiGifts/*` (quà lễ hội) → `FromDate` / `ToDate` (+ `AwardStartDate`/`AwardEndDate`)
- `HeFuGifts/*` → `FromDate` / `ToDate`
- `HuiGuiHuoDong.xml` → `BeginTime` / `FinishTime`

Trạng thái hiển thị được tính theo thời gian hiện tại:
- `now < From` → **Chưa bắt đầu**
- `From ≤ now ≤ To` → **Đang chạy**
- `now > To` → **Hết hạn**

**Khi TẮT:** app lưu lại khung ngày cũ (vào `state/<key>.window.json`) rồi đặt `From/To` về mốc quá khứ
(`2000-01-01`) ⇒ server không chạy sự kiện.
**Khi BẬT:** khôi phục khung ngày đã lưu; nếu không có thì đặt mặc định `now → now+7 ngày`.

## 3.3. Park — Gỡ tạm (cho file không có cờ)
Phần lớn file còn lại (EventCalendar, SpecialActivity, EveryDayActivity, SystemOpen, các Theme/SevenDay/
HuoDongTab/Activity…) **không có cờ enabled**. GameServer đọc tất cả bản ghi có trong file.

Vì vậy, muốn **TẮT một mục mà không mất dữ liệu**, app dùng cơ chế *Park*:
- **TẮT:** gỡ phần tử khỏi file XML gốc và chuyển sang file sidecar
  `Config/_EventManager/state/<key>.disabled.xml` (cùng cấu trúc). Server không còn đọc mục đó.
- **BẬT:** chuyển phần tử từ sidecar trở lại file gốc.

Đây là cách an toàn nhất với loại file không có cờ: dữ liệu được bảo toàn 100% và có thể đảo ngược.

## 3.4. Bảng quy ước cơ chế theo file (tóm tắt)

| Nhóm | File | Cơ chế |
|---|---|---|
| Lịch sự kiện | EventCalendar | Park |
| Hoạt động đặc biệt | SpecialActivity | Park |
| Hoạt động đặc biệt | SpecialActivityTime | DateWindow (From/To) |
| Hằng ngày | EveryDayActivity/Group/Type | Park |
| Mở hệ thống | VersionSystemOpen | Flag (IsOpen) |
| Mở hệ thống | SystemOpen | Park |
| Theme | ThemeActivityOpen | Flag (Open) |
| Theme | Type/ZhiGou/BOSS/ZhuanSheng | Park |
| Lễ hội | JieRiGifts/* | DateWindow (From/To) |
| Hồi quy | HuiGuiHuoDong | DateWindow (Begin/Finish) |
| Gộp server | HeFuGifts/* | DateWindow (From/To) |
| 7 ngày | SevenDay/* | Park |
| Tab hoạt động | *HuoDongTab | Park |
| Khác | Activity/* | Park |

## 3.5. An toàn
- Trước **mọi** thao tác ghi: sao lưu file gốc vào `Config/_EventManager/backups/`.
- Giữ nguyên **UTF-8 BOM**, khai báo XML, và **thứ tự thuộc tính** như bản gốc.
- Thư mục `_EventManager/` (backups + state) do app tạo, **không** ảnh hưởng tới GameServer.
