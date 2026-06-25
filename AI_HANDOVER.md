# AI Handover Document — GameMU-Assembly-CSharp

> **Mục đích**: Tài liệu này cho phép AI khác tiếp quản dự án mà không cần đọc lại toàn bộ codebase.
> **Repository**: https://github.com/chinhnh2018-source/GameMU-Assembly-CSharp
> **Cập nhật lần cuối**: 2026-06-25
> **Phiên làm việc**: 3 sessions phân tích + 2 sessions phát triển

---

## 1. Tổng quan dự án

### 1.1 Ba thành phần

| Project | Ngôn ngữ | Vai trò | Trạng thái |
|---------|----------|---------|-----------|
| `GameServer/` | C# | Game server production (reversed từ dnSpy) | **Chỉ đọc, KHÔNG sửa** |
| `KF.Remoting.HuanYingSiYuan/` | C# | Cross-server hub (AutoCSer RPC) | **Chỉ đọc, KHÔNG sửa** |
| `GameMU.Manager/` | C# ASP.NET Core 8 | Web admin tool — **đang phát triển** | **ĐANG LÀM** |
| `GameRes/GameRes/Config/` | XML | Tất cả config files game | Nguồn dữ liệu chính |

### 1.2 Mục tiêu dự án

Theo prompt `Game_Event_Reverse_Engineering_FULL.md` (file gốc trong repo root):
1. Reverse engineer GameServer C# source (đã hoàn thành 100%)
2. Tài liệu hóa toàn bộ hệ thống sự kiện (đã hoàn thành)
3. Xây dựng `GameMU.Manager` — web admin CRUD cho XML config (Phase 3 hoàn thành, Phase 4 đang chạy)

---

## 2. Trạng thái hiện tại — GameMU.Manager

### 2.1 Chạy ngay

```bash
cd GameMU.Manager
dotnet build   # cần NET SDK 8.0+
dotnet run
# → http://localhost:5000        (Web UI)
# → http://localhost:5000/swagger (REST API docs)
```

**Config**: `appsettings.json` → `EventManager:GameResConfigPath` trỏ đến `../GameRes/GameRes/Config`

### 2.2 Những gì đã có

| File/Tính năng | Mô tả | Trạng thái |
|----------------|-------|-----------|
| `Services/EventRegistry.cs` | **122 EventFileDef entries** — danh mục tất cả XML có thể quản lý | ✅ |
| `Services/ModuleRegistry.cs` | **19 sidebar modules** — điều hướng UI | ✅ |
| `Services/XmlEventService.cs` | Core CRUD + backup + toggle (3 strategies) | ✅ |
| `Services/ZipService.cs` | Import/Export ZIP bundle | ✅ |
| `Services/GoodsAuditService.cs` | FK validation GoodsID với Goods.xml | ✅ |
| `Services/LinkResolutionService.cs` | FK forward/back reference resolution | ✅ |
| `Services/XmlValidationService.cs` | Validate trước khi lưu (GoodsID, TimeWindow, %) | ✅ |
| `ApiEndpoints.cs` | **16 REST endpoints** (CRUD, Toggle, ZIP, Reload) | ✅ |
| `wwwroot/js/GoodsDataEditor.js` | Visual editor cho GoodsData 7-field format | ✅ |
| `Pages/File.cshtml` | Danh sách records, filter, toggle, xóa + **pagination** | ✅ |
| `Pages/Edit.cshtml` | Form thêm/sửa + GoodsDataEditor + validation | ✅ |
| `Pages/Calendar.cshtml` | Timeline view, detect overlap | ✅ |
| `Pages/Backups.cshtml` | ZIP Export/Import UI + backup list | ✅ |
| `Pages/GoodsAudit.cshtml` | FK audit với Goods.xml | ✅ |
| `Pages/Features.cshtml` | 186 GameServer classes ↔ XML map | ✅ |
| `Pages/Links.cshtml` | FK visualization giữa files | ✅ |

### 2.3 EventRegistry — 122 entries theo category

| Module ID | Category | Số files | Ví dụ |
|-----------|----------|---------|-------|
| `jieri` | Quà lễ hội | 44 | JieRiDengLu.xml, JieRiLeiJi.xml |
| `hefu` | Hợp phục / Gộp server | 12 | HeFuDengLu.xml, PKJiangLi.xml |
| `events` | Lịch/Hoạt động hằng ngày | 17 | EventCalendar.xml, EveryDayActivity.xml |
| `festival` | Sự kiện 7 ngày/Chủ đề | 11 | SevenDayGoal.xml, ThemeActivityZhiGou.xml |
| `arena` | Đấu trường | 5 | Arena.xml, ArenaAward.xml |
| `dungeon` | Đền & Dungeon | 9 | AngelTemple.xml, TempleMirage.xml |
| `richanggifts` | Quà hằng ngày | 6 | DayChongZhi.xml, LevelAward.xml |
| `vip` | VIP | 3 | MuVip.xml, VipDailyAwards.xml |
| `giftcode` | Gift Code & Quà | 2 | GiftCodeNew.xml (214KB!) |
| `shop` | Shop & Mall | 2 | ZhanGongMall.xml |
| `system` | Mở hệ thống | 2 | VersionSystemOpen.xml, SystemOpen.xml |
| `params` | Tham số hệ thống | 1 | SystemParams.xml (147KB, 939 params) |
| `refdata` | Từ điển gốc | 9 | Goods.xml (15MB!), Monsters.xml |

### 2.4 REST API Endpoints

```
GET    /api/files                      → Danh sách 122 XML files
GET    /api/events/{key}               → Load records của 1 file
GET    /api/events/{key}/{id}          → Load 1 record
PUT    /api/events/{key}/{id}          → Cập nhật 1 record
POST   /api/events/{key}              → Thêm record mới
DELETE /api/events/{key}/{id}          → Xóa record
POST   /api/events/{key}/{id}/toggle?enable=true
POST   /api/events/{key}/backup        → Backup thủ công
GET    /api/backups                    → List 50 backups gần nhất
POST   /api/reload/{key}              → Trigger GameServer hot-reload
GET    /api/params/{name}              → Đọc SystemParam
PUT    /api/params/{name}              → Sửa SystemParam
GET    /api/export?keys=k1,k2          → Download ZIP
POST   /api/import (multipart .zip)    → Upload ZIP (auto-backup trước)
```

---

## 3. GameServer — Phân tích đã hoàn thành

### 3.1 Kiến trúc tổng thể

```
TCP Packet → TCPCmdHandler.cs (1.7MB switch/case)
                ↓
        Manager/Service Layer
                ↓
        HuodongCachingMgr (Config cache)  +  XmlEventService (reload trigger)
                ↓
        XML Config (GameRes/Config/*.xml)
                ↓
        DB Layer: Global.SaveRoleParamsXxxToDB()
```

### 3.2 Hệ thống sự kiện — ActivityTypes enum

| Range | Nhóm | Ví dụ |
|-------|------|-------|
| 1-17 | Lễ hội (JieRi) | JieriDaLiBao(9), JieriDengLu(10) |
| 20-27 | Hợp phục (HeFu) | HeFuLogin(20), HeFuRecharge(23) |
| 27-46 | Hằng ngày/Nạp | MeiRiChongZhiHaoLi(27), EverydayActivity(46) |
| 50-77 | Lễ hội mở rộng | JieriGive(50), JieriWing(55), JieriFuLi(66) |
| 100 | Platform | JieriPlatChargeKing(100) |
| 110-114 | Hồi quy 3 năm | TriennialRegressOpen(110) |
| 150-157 | Theme | ThemeZhiGou(150) đến ThemeZS(157) |
| 999-1000 | Đặc biệt | TenReturn(999), PlatFuLiUC(1000) |

### 3.3 Key hardcodes cần biết

| File | Value | Ý nghĩa |
|------|-------|---------|
| `DailyActiveManager.cs` | IDs: 100,200,...,1600 | 19 DailyActive tasks hardcode |
| `DailyActiveManager.cs` | Packet 558 | Notify client DailyActive |
| `EverydayActivity.cs` | EventType **36** | = `OnClientChargeItem` |
| `EverydayActivity.cs` | DBCmd **13173** | Stored proc nạp tiền |
| `Activity.cs` | `"2008-08-08 08:08:08"` | Default start date |
| `Activity.cs` | `"2028-08-08 08:08:08"` | Default end date |
| `XmlEventService.cs` | `"2000-01-01 00:00:00"` | OffSentinel (tắt sự kiện) |
| `FestivalManager` | `to.Year <= 2001` | Check sự kiện tắt |

### 3.4 GoodsData format (quan trọng)

```
"GoodsID,GCount,Binding,Forge_level,AppendPropLev,Lucky,ExcellenceInfo|..."
Ví dụ: "63262,50,1,0,0,0,0|9990001,200,1,0,0,0,0"
```

- **7 fields**, pipe-separated, comma-separated per item
- Binding: 0=không ràng buộc, 1=ràng buộc
- Forge/Append/Lucky/Excellence = 0 nếu không có

### 3.5 Toggle strategies trong GameMU.Manager

| Strategy | Mechanism | Files điển hình |
|----------|-----------|----------------|
| `Flag` | Attribute `IsOpen`/`Open` = `1`/`0` | VersionSystemOpen.xml |
| `DateWindow` | `FromDate`/`ToDate` = OffSentinel khi tắt | JieRiGifts/*.xml |
| `Park` | Move record ra sidecar `.disabled.xml` | EventCalendar.xml |

---

## 4. KF.Remoting.HuanYingSiYuan

**Không cần sửa** — đây là cross-server hub riêng biệt.

- Framework: **AutoCSer** (RPC, không phải WCF/gRPC)
- 21 Services: JunTuanService, BangHuiMatchService, TianTiService...
- 16 Persistence files
- GameServer kết nối qua `HuanYingSiYuanManager.cs` → `KF.Client.HuanYingSiYuanClient`
- TCP cmds đăng ký: 820, 821, 822, 824, 826, 828

---

## 5. XML Files — Coverage

### 5.1 Config subdirs đã analyze

```
GameRes/GameRes/Config/
├── ✅ Activity/          (ActivityTip.xml, BossInfo.xml, Copy.xml)
├── ✅ Arena/             (5 files)
├── ✅ EveryDayActivity/  (EveryDayActivity.xml 52KB, Group.xml, Type.xml)
├── ✅ HeFuGifts/         (12 files: HeFuType.xml, ...)
├── ✅ JieRiGifts/        (46 files: JieRiType.xml, ...)
├── ✅ RiChangGifts/      (6 files)
├── ✅ SevenDay/          (6 files: Goal.xml 33KB, Login.xml...)
├── ✅ SpecialActivity/   (SpecialActivity.xml, SpecialActivityTime.xml)
├── ✅ (root 540 files)   (SystemParams.xml 147KB, EventCalendar.xml 20KB...)
├── ❓ BaoDian/           chưa khám phá
├── ❓ Fund/              chưa khám phá
├── ❓ Gem/               chưa khám phá
├── ❓ Horses/            chưa khám phá
├── ❓ HorseBodys/        chưa khám phá
├── ❓ JingMais/          chưa khám phá
├── ❓ Magics/            chưa khám phá
├── ❓ Manor/             chưa khám phá
├── ❓ ManorCrystal/      chưa khám phá
├── ❓ Merlin/            chưa khám phá
├── ❓ Roles/             chưa khám phá
├── ❓ SingleEquipAddProp/ chưa khám phá
├── ❓ Treasure/          chưa khám phá
├── ❓ Wing/              chưa khám phá
└── ❓ XingZuo/           chưa khám phá
```

### 5.2 Các file XML đặc biệt

| File | Size | Ghi chú |
|------|------|---------|
| `Goods.xml` | ~15MB | FK target cho GoodsAudit — **KHÔNG sửa** |
| `GiftCodeNew.xml` | 214KB | ~1000 gift codes, TypeID là string (vd "S26X") |
| `SystemParams.xml` | 147KB | 939 tham số, có comment → preserve khi ghi |
| `SystemOpen.xml` | 44KB | Feature flags |
| `EveryDayActivity.xml` | 52KB | Hoạt động hằng ngày event-driven |
| `SevenDayGoal.xml` | 33KB | 44 GoalFuncTypes |

---

## 6. Coding Conventions

### 6.1 GameMU.Manager — quy tắc quan trọng

```csharp
// EventFileDef — thêm mới vào EventRegistry.cs
new() {
    Key = "kebab-case-key",       // URL slug
    RelativePath = "SubDir/File.xml",
    DisplayName = "Tên tiếng Việt",
    Category = "Tên nhóm module",  // PHẢI khớp với ModuleRegistry.Categories
    ItemElement = "Award",          // XML child tag
    IdAttr = "ID",                  // attribute làm key
    NameAttr = "Name",              // optional: tên hiển thị
    ListColumns = new[]{"ID","Field1","Field2"},
    Toggle = ToggleStrategy.DateWindow,  // Park | DateWindow | Flag | None
    FromAttr = "FromDate",          // cho DateWindow
    ToAttr = "ToDate",
    ForeignKeys = new() {
        new() { Field="GoodsOne", TargetKey="goods", ParseRewardList=true }
    }
}
```

### 6.2 XmlEventService — Safety Model

```
Mọi write (Add/Update/Delete/Toggle) đều:
1. Auto-backup → Config/_EventManager/backups/{key}_{timestamp}.bak
2. Park OFF → Config/_EventManager/state/{key}.disabled.xml
3. DateWindow OFF → ToDate = "2000-01-01 00:00:00" (OffSentinel)
4. Ghi UTF-8 BOM, giữ nguyên attribute order
```

### 6.3 GoodsData Editor

```javascript
// GoodsDataEditor.js tự detect field có name="f_GoodsOne/Two/Thr/Four"
// Parse: "id,count,bind,forge,append,lucky,excel|..." → table 7 cols
// Serialize: rows → hidden input → form submit
```

### 6.4 ApiEndpoints.cs — pattern

```csharp
// Luôn tìm def trước, return 404 nếu không có
var def = FindDef(key);
if (def == null) return Results.NotFound(new { error = $"Key không tồn tại: {key}" });
// Error codes: -1=null, -2=time, -3=bag, -7=fail, -10007=condition, 1=OK
```

---

## 7. Những gì cần làm tiếp

### 7.1 Ngay lập tức (ưu tiên cao)

- [ ] **Khám phá 12 XML subdirs chưa phân tích**:
  `BaoDian/, Fund/, Gem/, Horses/, HorseBodys/, JingMais/, Magics/, Manor/, ManorCrystal/, Merlin/, Roles/, Treasure/, Wing/, XingZuo/`
  → Thêm vào EventRegistry nếu cần admin

- [ ] **Test build thực tế**: `cd GameMU.Manager && dotnet build && dotnet run`
  Kiểm tra `/swagger` trả về 16 endpoints, `/api/files` trả về 122 files

### 7.2 Medium priority

- [ ] **Performance: Goods.xml 15MB cache** — benchmark LoadRecords cho files lớn
- [ ] **GameServer reload integration** — nếu GameServer expose HTTP management port:
  ```json
  // Thêm vào appsettings.json:
  "GameServer": { "ManageUrl": "http://localhost:9000" }
  ```
- [ ] **Pagination cho SevenDayGoal.xml** — 33KB, nhiều records → tương tự GiftCode (50/page)

### 7.3 Nice-to-have

- [ ] **Diff viewer** — hiển thị before/after khi edit
- [ ] **Import conflict resolution** — khi import ZIP đè file đang mở
- [ ] **Batch toggle** — bật/tắt nhiều sự kiện cùng lúc

---

## 8. Files quan trọng để đọc ngay

```bash
# 1. Đọc prompt gốc (hiểu toàn bộ yêu cầu)
cat Game_Event_Reverse_Engineering_FULL.md

# 2. Đọc tóm tắt phân tích
cat docs/98_Summary.md
cat docs/99_Checklist.md

# 3. Đọc design system
cat docs/19_ActivitySystem_Analysis.md   # ActivityTypes enum đầy đủ
cat docs/20_XMLSchema_Complete.md         # XML schemas
cat docs/21_CallGraph_DataModel_CRUD.md  # Call graph + Data models

# 4. Đọc trạng thái Manager
cat GameMU.Manager/README.md
cat docs/DEPLOY.md
cat docs/26_GameMUManager_Complete.md    # GoodsAudit + Calendar

# 5. Đọc source code quan trọng nhất
# GameServer/Logic/ActivityTypes.cs       ← enum 100+ types
# GameServer/Logic/DailyActiveTypes.cs    ← 19 daily IDs
# GameServer/Logic/Activity.cs            ← base class + toggle logic
# GameServer/Logic/EverydayActivity.cs    ← event-driven EA system
# GameServer/Logic/ReloadXmlManager.cs    ← hot-reload mapping
```

---

## 9. Cấu trúc thư mục GameMU.Manager

```
GameMU.Manager/
├── ApiEndpoints.cs                   ← 16 REST endpoints (MapApiEndpoints)
├── Program.cs                        ← DI + middleware setup
├── appsettings.json                  ← GameResConfigPath + GameServer:ManageUrl
├── appsettings.template.json         ← template cho production
├── Models/
│   └── EventModels.cs               ← EventFileDef, ToggleStrategy, EventRecord, ForeignKeyRef
├── Services/
│   ├── EventRegistry.cs             ← 122 EventFileDef entries  ★ CHÍNH
│   ├── EventRegistry.COMPLETE.snippet.cs   ← snippet JieRi/HeFu (reference)
│   ├── EventRegistry.Expansion2.snippet.cs ← snippet SevenDay/Arena/VIP (reference)
│   ├── ModuleRegistry.cs            ← 19 sidebar modules  ★ CHÍNH
│   ├── XmlEventService.cs           ← Core R/W/Cache/Backup/Toggle  ★ CHÍNH
│   ├── XmlValidationService.cs      ← Validate trước khi ghi
│   ├── ZipService.cs                ← Import/Export ZIP
│   ├── GoodsAuditService.cs         ← FK validation với Goods.xml
│   ├── LinkResolutionService.cs     ← FK forward/back refs
│   ├── ModuleRegistry.cs            ← 19 modules
│   ├── FeatureCatalog.cs            ← 186 GameServer class → XML
│   ├── LinkRegistry.cs              ← Link definitions
│   └── LinkResolutionService.cs
├── Pages/
│   ├── Index.cshtml/.cs             ← Trang chủ: groups + counts
│   ├── File.cshtml/.cs              ← Danh sách records, pagination
│   ├── Edit.cshtml/.cs              ← Form thêm/sửa + validation
│   ├── Calendar.cshtml/.cs          ← Timeline DateWindow events
│   ├── Backups.cshtml/.cs           ← ZIP export/import + backup list
│   ├── GoodsAudit.cshtml/.cs        ← FK audit scan
│   ├── Features.cshtml/.cs          ← Class→XML map
│   ├── Links.cshtml/.cs             ← FK visualization
│   └── Shared/_Layout.cshtml        ← Sidebar auto-render từ ModuleRegistry
└── wwwroot/js/
    └── GoodsDataEditor.js           ← Visual editor GoodsData 7-field
```

---

## 10. Thông tin kỹ thuật quan trọng

### 10.1 EventRegistry — cách thêm file mới

```csharp
// Bước 1: Xác định schema bằng cách đọc XML file
// GET https://raw.githubusercontent.com/chinhnh2018-source/GameMU-Assembly-CSharp/main/GameRes/GameRes/Config/{path}

// Bước 2: Thêm entry vào Services/EventRegistry.cs, vị trí: theo category
new() {
    Key = "my-key",
    RelativePath = "SubDir/MyFile.xml",
    DisplayName = "Tên hiển thị tiếng Việt",
    Category = "Tên module đã có trong ModuleRegistry",  // QUAN TRỌNG
    ItemElement = "Award",   // tag XML child element
    IdAttr = "ID",
    ListColumns = new[]{"ID", "Field1", "Field2"},
    Toggle = ToggleStrategy.Park,
},

// Bước 3: Nếu Category mới, thêm module vào ModuleRegistry.cs
new() { Id="new-module", Name="Tên module", Icon="icon-name", // Bootstrap Icons
    Description="...", Categories=new[]{ "Tên category mới" } },
```

### 10.2 Backup & Safety

- Auto-backup trước MỌI thao tác ghi tại: `Config/_EventManager/backups/`
- Disabled records: `Config/_EventManager/state/{key}.disabled.xml`
- Window state: `Config/_EventManager/state/{key}.window.json`
- **KHÔNG BAO GIỜ xóa** thư mục `_EventManager/` — đây là state của tool

### 10.3 GitHub repo structure

```
repo root
├── Assembly-CSharp/          ← Unity client scripts (chỉ đọc)
├── Assembly-CSharp-firstpass/ ← Unity firstpass (chỉ đọc)
├── ClientTools/              ← Client tools
├── GameRes/GameRes/Config/   ← XML configs (540+ files)  ★ QUAN TRỌNG
│   ├── Activity/, Arena/, EveryDayActivity/, HeFuGifts/, JieRiGifts/, RiChangGifts/
│   ├── SevenDay/, SpecialActivity/, ThemeActivity*.xml
│   ├── SystemParams.xml, EventCalendar.xml, GiftCodeNew.xml...
│   ├── BaoDian/, Fund/, Gem/, Horses/, Magics/...  ← CHƯA phân tích
│   └── Goods.xml (15MB), Monsters.xml...
├── GameServer/               ← Server C# source  ★ QUAN TRỌNG
│   ├── Logic/ (1000+ files)  ← Business logic
│   ├── Core/GameEvent/       ← EventTypes, EventSource
│   ├── Server/TCPCmdHandler.cs (1.7MB) ← Entry point
│   └── KF/                   ← KF client side
├── KF.Remoting.HuanYingSiYuan/ ← Cross-server hub
├── GameMU.Manager/           ← Web admin tool  ★ ĐANG LÀM
└── docs/                     ← 28 analysis docs
```

### 10.4 Commit conventions

```
docs:   Thêm/sửa tài liệu phân tích
feat:   Tính năng mới cho GameMU.Manager
fix:    Sửa lỗi
feat(EventRegistry): Thêm entries mới
feat(ModuleRegistry): Thêm modules mới
```

---

## 11. Lỗi đã biết và workarounds

| Vấn đề | Nguyên nhân | Workaround |
|--------|------------|-----------|
| `DailyActiveInfor.xml` có wrapper `<DailyActive>` | XmlEventService tìm ItemElement ở bất kỳ đâu | Load OK, Add có thể sai vị trí — dùng Edit only |
| `GiftCodeNew.xml` TypeID là string | TypeID="S26X" không phải số | Pagination 50/page đã xử lý tốc độ |
| `Goods.xml` 15MB FK lookup chậm | File quá lớn | Có cache bởi GoodsAuditService |
| `SevenDayGoal.xml` 33KB | Nhiều records | Cần thêm PageSize đặc biệt tương tự GiftCode |

---

## 12. Người dùng và context

- **User**: chơi game (praovip6@gmail.com)
- **Timezone**: Asia/Bangkok (UTC+7)
- **Giao tiếp**: Tiếng Việt
- **Style**: Ngắn gọn, "tiếp" = tiếp tục, không cần giải thích dài
- **Workflow**: phân tích source → tạo doc → push GitHub → phát triển Manager

---

*Tài liệu này được tạo bởi AI Assistant tại Gumloop. Cập nhật lần cuối: 2026-06-25 20:37 ICT*
