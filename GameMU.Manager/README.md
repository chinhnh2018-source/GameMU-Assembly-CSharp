# GameMU Event Manager

Web app C# (ASP.NET Core 8 / Razor Pages) để **quản lý sự kiện** của GameServer MU,
đọc và ghi trực tiếp các file XML trong `GameRes/GameRes/Config`.

Cho phép: **xem danh sách sự kiện, lọc/tìm kiếm, sửa thuộc tính, thêm mới, xoá, và BẬT/TẮT từng sự kiện** —
mọi thay đổi đều **tự động sao lưu** trước khi ghi đè. Toàn bộ giao diện **tiếng Việt có dấu**.

Ngoài quản lý sự kiện, web app còn có:
- **Trình quản lý `SystemParams.xml`** (từ điển ~939 tham số điều khiển cường hóa/truy gia/truyền thừa/cánh...),
  có tìm kiếm và hiển thị **chú thích gốc**; khi lưu **giữ nguyên định dạng (tab) và toàn bộ comment**.
- **Bản đồ Tính năng → XML** (`/Features`): danh sách 186 lớp quản lý của GameServer và các file XML mà mỗi
  tính năng đọc — sinh từ việc **quét mã nguồn `GameServer`**.

---

## 1. Phân tích hệ thống sự kiện (tóm tắt)

GameServer không có một cờ "enabled" thống nhất cho mọi sự kiện. Sau khi phân tích `GameRes/Config`
và mã nguồn `GameServer/Logic/ActivityNew`, mỗi nhóm sự kiện được bật/tắt theo một trong các cách sau:

| Cơ chế | Ý nghĩa | File tiêu biểu |
|---|---|---|
| **Flag** (cờ) | Thuộc tính `IsOpen` / `Open` = `1`/`0` | `VersionSystemOpen.xml`, `ThemeActivityOpen.xml` |
| **DateWindow** (khung ngày) | `FromDate`/`ToDate` (hoặc `BeginTime`/`FinishTime`) quyết định thời gian chạy | `SpecialActivity/SpecialActivityTime.xml`, `JieRiGifts/*` (quà lễ hội), `HeFuGifts/*` (gộp server), `HuiGuiHuoDong.xml` |
| **Park** (gỡ tạm) | Không có cờ sẵn → tắt = chuyển bản ghi ra file sidecar, server không còn đọc; bật lại = đưa về | `EventCalendar.xml`, `SpecialActivity.xml`, `EveryDayActivity*.xml`, `SystemOpen.xml`, các `*HuoDongTab.xml`, `SevenDay/*`, `Activity/*`, `Theme*`… |

App tự chọn đúng cơ chế cho từng file (xem cột "bat/tat" trên giao diện).

### Các nhóm sự kiện được quản lý
- **Lịch sự kiện** — `EventCalendar.xml` (Blood Castle, Devil Square… theo thứ/khung giờ)
- **Hoạt động đặc biệt** — `SpecialActivity.xml` + `SpecialActivityTime.xml` (theo `GroupID`)
- **Hoạt động hằng ngày** — `EveryDayActivity.xml` / `Group` / `Type`
- **Mở hệ thống** — `VersionSystemOpen.xml` (cờ `IsOpen`), `SystemOpen.xml`
- **Sự kiện chủ đề (Theme)** — `ThemeActivityOpen/Type/ZhiGou/BOSS/ZhuanSheng`
- **Quà lễ hội (JieRi)** — toàn bộ `JieRiGifts/*` theo khung ngày
- **Hồi quy / Gộp server** — `HuiGuiHuoDong.xml`, `HeFuGifts/*`
- **Sự kiện 7 ngày** — `SevenDay/*`
- **Tab hoạt động** — `HuoDongTab`, `KuaFuHuoDongTab`, `ZhanDui`, `ZhanMeng`
- **Hoạt động khác** — `Activity/Copy`, `Activity/BossInfo`, `Activity/ActivityTip`

Danh mục đầy đủ và quy tắc bật/tắt nằm trong `Services/EventRegistry.cs` — dễ dàng thêm file mới.

---

## 2. Cài đặt & chạy

Yêu cầu: **.NET SDK 8.0+**.

```bash
cd GameMU.EventManager
# Trỏ tới thư mục Config của server (sửa trong appsettings.json hoặc qua biến môi trường)
dotnet run
```

Mặc định mở tại `http://localhost:5000` (hoặc cổng do `ASPNETCORE_URLS` quy định).

### Cấu hình đường dẫn
Sửa `appsettings.json`:
```json
"EventManager": {
  "GameResConfigPath": "../GameMU-Assembly-CSharp/GameRes/GameRes/Config"
}
```
- Có thể dùng đường dẫn **tuyệt đối** (vd `D:\\MuServer\\GameRes\\GameRes\\Config`) hoặc **tương đối** so với thư mục chạy app.
- Hoặc đặt qua biến môi trường: `EventManager__GameResConfigPath=/duong/dan/Config`.

---

## 3. An toàn dữ liệu

- Trước **mọi** thao tác ghi (sửa / thêm / xoá / bật / tắt), file gốc được sao lưu vào
  `Config/_EventManager/backups/<key>__<tên file>__<thời gian>.bak`.
- Bản ghi bị **TẮT** theo cơ chế *Park* được giữ trong `Config/_EventManager/state/<key>.disabled.xml`
  (không mất dữ liệu, bật lại là khôi phục).
- Cơ chế *DateWindow* khi TẮT sẽ lưu lại khung ngày cũ trong `state/<key>.window.json` để khôi phục khi BẬT.
- File XML ghi ra giữ **UTF-8 BOM** và khai báo `<?xml version="1.0" encoding="utf-8"?>` như bản gốc;
  thứ tự thuộc tính được giữ nguyên (chỉ sửa đúng thuộc tính cần đổi).

> Lưu ý vận hành: GameServer thường nạp cấu hình lúc khởi động. Sau khi chỉnh sửa, hãy **reload cấu hình
> hoặc khởi động lại GameServer** để thay đổi có hiệu lực, tuỳ theo cơ chế của bản server bạn dùng.

---

## 4. Cấu trúc mã nguồn

```
GameMU.EventManager/
├─ Program.cs                      # cấu hình ứng dụng
├─ appsettings.json                # đường dẫn GameRes/Config
├─ Models/EventModels.cs           # EventFileDef, ToggleStrategy, EventRecord
├─ Services/
│  ├─ EventRegistry.cs             # danh mục file sự kiện + nhãn tiếng Việt + chiến lược bật/tắt
│  └─ XmlEventService.cs           # đọc/ghi/sửa/thêm/xoá/bật-tắt + sao lưu (lõi)
└─ Pages/
   ├─ Index.cshtml                 # trang chủ: nhóm theo danh mục, đếm số mục
   ├─ File.cshtml                  # danh sách 1 file: lọc, bật/tắt, sửa, xoá
   ├─ Edit.cshtml                  # form thêm/sửa thuộc tính
   └─ Backups.cshtml               # danh sách bản sao lưu gần đây
```

### Mở rộng (thêm file sự kiện mới)
Thêm một `EventFileDef` vào `EventRegistry.Files` với `Key`, `RelativePath`, `ItemElement`,
`IdAttr`, `ListColumns` và `Toggle` phù hợp. Giao diện và toàn bộ thao tác CRUD/bật-tắt tự hoạt động.
