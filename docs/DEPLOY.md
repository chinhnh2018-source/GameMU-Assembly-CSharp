# DEPLOY.md — Hướng dẫn cài đặt Phase 3 (GameMU.Manager)
> Date: 2026-06-25

---

## Tổng quan các file cần thêm/sửa

| File | Action | Mô tả |
|------|--------|-------|
| `ApiEndpoints.cs` | **TẠO MỚI** | REST API 12 endpoints |
| `EventRegistry.COMPLETE.snippet.cs` | **MERGE VÀO** EventRegistry.cs | 59 JieRi/HeFu/Daily/Gift entries |
| `wwwroot/js/GoodsDataEditor.js` | **TẠO MỚI** | Visual editor GoodsData |
| `appsettings.json` | **CẬP NHẬT** | Thêm GameServer:ManageUrl |
| `Pages/Edit.cshtml` | **SỬA NHỎ** | Thêm script tag GoodsDataEditor |

---

## Bước 1: Thêm ApiEndpoints.cs

```bash
# Copy file ApiEndpoints.cs vào thư mục GameMU.Manager/
cp ApiEndpoints.cs GameMU.Manager/ApiEndpoints.cs
```

File này là extension method implement `app.MapApiEndpoints()` mà Program.cs đã gọi.  
Sau khi thêm, REST API tự động hoạt động tại `/api/*` + Swagger tại `/swagger`.

**Endpoints có sẵn sau khi thêm:**
```
GET    /api/files                    ← liệt kê tất cả XML files
GET    /api/events/{key}             ← load danh sách records
GET    /api/events/{key}/{id}        ← load 1 record
PUT    /api/events/{key}/{id}        ← lưu 1 record
POST   /api/events/{key}             ← thêm record mới
DELETE /api/events/{key}/{id}        ← xóa record
POST   /api/events/{key}/{id}/toggle?enable=true  ← bật/tắt
POST   /api/events/{key}/backup      ← backup thủ công
GET    /api/backups                  ← list backups
POST   /api/reload/{key}             ← trigger GameServer reload
GET    /api/params/{name}            ← đọc SystemParam
PUT    /api/params/{name}            ← sửa SystemParam
```

---

## Bước 2: Merge EventRegistry snippet vào EventRegistry.cs

```bash
# Mở file: GameMU.Manager/Services/EventRegistry.cs
# Tìm entry cuối cùng trong Files list:
#     new() { Key="huigui-huodong", ...
# Thêm toàn bộ nội dung EventRegistry.COMPLETE.snippet.cs ngay sau đó
```

### Sau khi merge, EventRegistry sẽ có:
- 35 files ban đầu
- **+59 files mới** (JieRi/HeFu/DailyActive/GiftCode)
- **= 94 files tổng**

### Effect ngay lập tức (không cần restart app):
1. **Calendar page** hiển thị timeline đầy đủ 45+ sự kiện JieRi/HeFu
2. **Index page** hiển thị nhóm "Quà lễ hội" và "Hợp phục / Gộp server"
3. **GoodsAudit** scan thêm JieRi/HeFu → phát hiện GoodsID broken/missing

---

## Bước 3: Thêm GoodsDataEditor.js

```bash
# Tạo thư mục nếu chưa có:
mkdir -p GameMU.Manager/wwwroot/js

# Copy file:
cp GoodsDataEditor.js GameMU.Manager/wwwroot/js/GoodsDataEditor.js
```

### Thêm vào Pages/Edit.cshtml (cuối file, trước `</body>`):
```html
<script src="/js/GoodsDataEditor.js"></script>
```

### Effect:
- Các field `GoodsOne`, `GoodsTwo`, `GoodsThr`, `GoodsID`... tự động render dạng bảng
- Mỗi row = 1 item với 7 input fields: GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo
- Click "+ Thêm item" để thêm row
- Click "✕" để xóa row
- Hidden input tự sync khi thay đổi → submit form bình thường

---

## Bước 4: Cập nhật appsettings.json

Thêm section `GameServer` vào `appsettings.json`:
```json
{
  "Logging": { ... },
  "AllowedHosts": "*",
  "EventManager": {
    "GameResConfigPath": "../GameRes/GameRes/Config"
  },
  "GameServer": {
    "ManageUrl": ""
  }
}
```

Nếu GameServer có management HTTP port (ví dụ port 9000):
```json
"GameServer": {
  "ManageUrl": "http://localhost:9000"
}
```

→ Khi đó `POST /api/reload/{key}` sẽ gọi GameServer reload XML không cần restart.

---

## Bước 5: Kiểm tra build

```bash
cd GameMU.Manager
dotnet build
dotnet run
```

Truy cập:
- `http://localhost:5000` — Web UI
- `http://localhost:5000/swagger` — REST API Swagger docs

---

## Checklist sau deploy

- [ ] Build thành công không lỗi
- [ ] `/swagger` hiển thị 12 endpoints
- [ ] `GET /api/files` trả về 94 files
- [ ] Calendar hiển thị JieRi/HeFu sự kiện
- [ ] Edit page: field GoodsOne render dạng bảng thay vì text thuần
- [ ] GoodsAudit chạy thành công

---

## Giải thích cấu trúc project sau khi deploy

```
GameMU.Manager/
├── ApiEndpoints.cs              ← MỚI: REST API 12 endpoints
├── Program.cs                   (không đổi, đã gọi app.MapApiEndpoints())
├── appsettings.json             ← SỬA: thêm GameServer:ManageUrl
├── Models/
│   └── EventModels.cs           (không đổi)
├── Services/
│   ├── EventRegistry.cs         ← SỬA: merge 59 entries mới
│   ├── XmlEventService.cs       (không đổi - đã có 11 CRUD methods)
│   ├── GoodsAuditService.cs     (không đổi)
│   ├── FeatureCatalog.cs        (không đổi)
│   ├── ModuleRegistry.cs        (không đổi)
│   └── ...
├── Pages/
│   ├── Edit.cshtml              ← SỬA NHỎ: thêm script tag
│   └── ...
└── wwwroot/
    └── js/
        └── GoodsDataEditor.js   ← MỚI: visual editor

Tổng: 3 file tạo mới + 2 file sửa nhỏ = có đủ Phase 3
```

---

## Notes quan trọng

1. **ApiEndpoints.cs** dùng `XmlEventService` đã được DI inject trong Program.cs → không cần đăng ký thêm.

2. **EventRegistry.COMPLETE.snippet.cs** chứa entries dùng `ToggleStrategy.DateWindow` với `FromAttr="FromDate"` và `ToAttr="ToDate"` — khớp đúng với schema XML của JieRiGifts và HeFuGifts đã đọc.

3. **GoodsDataEditor.js** dùng Bootstrap 5 classes → đảm bảo project đã include Bootstrap 5.

4. **Reload GameServer**: tính năng `/api/reload/{key}` cần biết GameServer management endpoint. Nếu GameServer không expose HTTP management port → để ManageUrl trống → admin tự restart server sau khi sửa XML.

5. **GiftCodeNew.xml (214KB)**: khi load `/api/events/giftcode` sẽ parse toàn bộ → có thể chậm. Recommend thêm `?page=1&pageSize=50` pagination trong tương lai.
