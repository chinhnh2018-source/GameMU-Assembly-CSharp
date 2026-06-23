# 05 — Kiến trúc Web App `GameMU.EventManager`

Web app C# (**ASP.NET Core 8 / Razor Pages**) đọc–ghi trực tiếp XML trong `GameRes/Config`,
cho phép xem, lọc, sửa, thêm, xoá và **BẬT/TẮT** từng sự kiện. Mọi thay đổi đều được **sao lưu tự động**.

## 5.1. Cấu trúc dự án
```
GameMU.EventManager/
├─ Program.cs                      # cấu hình ứng dụng
├─ appsettings.json                # EventManager:GameResConfigPath
├─ Models/EventModels.cs           # EventFileDef, ToggleStrategy, EventRecord
├─ Services/
│  ├─ EventRegistry.cs             # danh mục file sự kiện + nhãn tiếng Việt + chiến lược bật/tắt
│  └─ XmlEventService.cs           # lõi: load/save/edit/add/delete/toggle + backup + sidecar
└─ Pages/
   ├─ Index.cshtml                 # trang chủ (nhóm theo danh mục, đếm số mục)
   ├─ File.cshtml                  # danh sách 1 file: lọc, bật/tắt, sửa, xoá
   ├─ Edit.cshtml                  # form thêm/sửa thuộc tính
   └─ Backups.cshtml               # danh sách bản sao lưu gần đây
```

## 5.2. Thiết kế dạng "registry" (động, dễ mở rộng)
Thay vì viết model riêng cho từng file, app dùng một **engine tổng quát** + một **danh mục mô tả**
(`EventRegistry.Files`). Mỗi file sự kiện được mô tả bằng `EventFileDef`:

```csharp
new EventFileDef {
  Key="version-system-open",                 // định danh trên URL
  RelativePath="VersionSystemOpen.xml",
  DisplayName="Mo he thong theo phien ban (IsOpen)",
  Category="Mo he thong",
  ItemElement="Version", IdAttr="ID", NameAttr="SystemName",
  ListColumns=new[]{"ID","SystemName","IsOpen"},
  Toggle=ToggleStrategy.Flag, FlagAttr="IsOpen", FlagOn="1", FlagOff="0"
}
```

`ToggleStrategy` ∈ `{ Flag, DateWindow, Park, None }` (xem tài liệu 03).
⇒ Thêm một file sự kiện mới = thêm 1 `EventFileDef`; toàn bộ giao diện + CRUD + bật/tắt tự hoạt động.

## 5.3. Lõi xử lý `XmlEventService`
- `LoadRecords(def)` — đọc bản ghi (kèm bản ghi đang TẮT từ sidecar nếu là *Park*); tính trạng thái Bật/Tắt/Đang chạy/Hết hạn.
- `UpdateRecord` / `AddRecord` / `DeleteRecord` — sửa/thêm/xoá thuộc tính, giữ thứ tự thuộc tính.
- `Toggle(def, id, enable)` — bật/tắt theo đúng cơ chế (Flag/DateWindow/Park).
- Ghi file bằng `XmlWriter` với **UTF-8 BOM**, indent 2 dấu cách, giữ khai báo `<?xml ... encoding="utf-8"?>`.
- **Backup tự động** trước mọi lần ghi: `Config/_EventManager/backups/`.
- Trạng thái phụ trợ: `Config/_EventManager/state/` (`*.disabled.xml`, `*.window.json`).

## 5.4. Cách chạy
Yêu cầu **.NET SDK 8.0+**:
```bash
cd GameMU.EventManager
dotnet run
```
Cấu hình đường dẫn trong `appsettings.json`:
```json
"EventManager": { "GameResConfigPath": "../GameMU-Assembly-CSharp/GameRes/GameRes/Config" }
```
Hỗ trợ đường dẫn tuyệt đối/tương đối, hoặc biến môi trường `EventManager__GameResConfigPath`.

## 5.5. Đã kiểm thử
Toàn bộ thao tác đã chạy thử trực tiếp trên dữ liệu thật của repo:
- Flag (IsOpen) bật/tắt ✔
- DateWindow (JieRi) tắt → mốc quá khứ, bật → khôi phục khung cũ ✔
- Park (EventCalendar) tắt → chuyển sidecar, bật → khôi phục ✔
- Sửa thuộc tính (EveryDayActivity) — giữ nguyên các thuộc tính khác, không tạo trùng ✔
- Thêm + Xoá (EventCalendar) ✔
- File sau ghi vẫn **well-formed**, giữ **BOM** và khai báo XML ✔

## 5.6. Lưu ý vận hành
GameServer thường nạp cấu hình lúc khởi động. Sau khi chỉnh sửa qua web app, hãy
**reload cấu hình hoặc khởi động lại GameServer** để thay đổi có hiệu lực.
