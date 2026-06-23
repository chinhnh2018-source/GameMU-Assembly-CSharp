# 01 — Tổng quan hệ thống sự kiện

## 1.1. "Sự kiện" trong GameServer là gì?

Trong source này, từ "sự kiện" (event / 活动 HuoDong) bao trùm nhiều loại nội dung được cấu hình
bằng XML và được GameServer/Client đọc lên:

- **Sự kiện theo lịch** (lặp lại theo thứ/khung giờ): Blood Castle, Devil Square… → `EventCalendar.xml`
- **Hoạt động đặc biệt theo đợt** (gói ưu đãi, nạp, mục tiêu): `SpecialActivity/*`
- **Hoạt động hằng ngày** (mục tiêu mỗi ngày): `EveryDayActivity/*`
- **Mở hệ thống/chức năng** theo phiên bản hoặc điều kiện: `VersionSystemOpen.xml`, `SystemOpen.xml`
- **Sự kiện chủ đề (Theme Activity)**: BOSS chủ đề, chuyển sinh, gói nạp theo chủ đề: `ThemeActivity*.xml`
- **Quà lễ hội (JieRi / 节日)**: nạp lễ, đăng nhập lễ, tích luỹ, hồng bao…: `JieRiGifts/*`
- **Hồi quy & Gộp server (HuiGui / HeFu)**: `HuiGuiHuoDong.xml`, `HeFuGifts/*`
- **Sự kiện 7 ngày (khai server)**: `SevenDay/*`
- **Tab hoạt động** (gom nhóm hiển thị): `*HuoDongTab.xml`
- **Hoạt động khác**: phụ bản, BOSS, gợi ý: `Activity/*`

## 1.2. Cách GameServer nạp cấu hình

- Các config được parse bằng `System.Xml.Linq` (xem `GameServer/Logic/ActivityNew/SpecialActivity.cs`).
- Lớp cấu hình tiêu biểu:
  - `SpecialActivityConfig` ⟷ `<Activity>` trong `SpecialActivity.xml`
  - `SpecialActivityTimeConfig` ⟷ `<Time>` trong `SpecialActivityTime.xml` (chứa `FromDate`/`ToDate`)
  - `EverydayActivityConfig`, `EverydayActivityTypeConfig`, …
- Thư mục logic: `GameServer/Logic/ActivityNew/` (Special/Everyday/Jieri/Theme/SevenDay…).

> **Quan trọng về vận hành:** server thường đọc cấu hình lúc khởi động (hoặc khi reload thủ công).
> Sau khi chỉnh sửa XML cần **reload cấu hình / khởi động lại GameServer** để có hiệu lực.

## 1.3. Vấn đề "bật/tắt" sự kiện

Không có một thuộc tính `Enabled` chung cho tất cả. Việc bật/tắt phụ thuộc loại file:
- Một số file **có cờ** (`IsOpen`, `Open`).
- Phần lớn sự kiện theo đợt **điều khiển bằng khung ngày** (`FromDate`/`ToDate`, `BeginTime`/`FinishTime`).
- Phần còn lại **không có cờ** → muốn tắt phải gỡ mục khỏi XML (cần cơ chế bảo toàn để khôi phục).

Chi tiết xem [03-Co-che-bat-tat-su-kien.md](03-Co-che-bat-tat-su-kien.md).
