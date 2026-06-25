# 23_EverydayActivity_GameMUManager_Analysis.md
> EverydayActivity.cs deep dive + GameMU.Manager current state
> Confidence: HIGH (đọc trực tiếp source)
> Date: 2026-06-25

---

## 1. EverydayActivity.cs — Deep Dive

**Path**: `GameServer/Logic/ActivityNew/EverydayActivity.cs`  
**Size**: 48KB / 42,008 chars  
**Kế thừa**: `Activity`, `IEventListener`

### 1.1 Kiến trúc

```
EverydayActivity : Activity, IEventListener
    │
    ├── ActivityTypeConfigDict  Dict<int, EverydayActivityTypeConfig>
    ├── ActivityGroupConfigDict Dict<int, EverydayActivityGroupConfig>
    ├── ActivityConfigDict      Dict<int, EverydayActivityConfig>
    └── ActiveConditionDict     Dict<int, ActiveConditionData>
```

### 1.2 XML files nạp

| Const | Đường dẫn |
|-------|-----------|
| EverydayActivityTypeData_fileName | Config/EveryDayActivity/EveryDayActivityType.xml |
| EverydayActivityGroupData_fileName | Config/EveryDayActivity/EveryDayActivityGroup.xml |
| EverydayActivityData_fileName | Config/EveryDayActivity/EveryDayActivity.xml |

Load order: Type → Group → Activity

### 1.3 EverydayActivityType values dùng trong code

| Type | Mô tả |
|------|-------|
| 1 | Loại thứ nhất (giết quái, dungeon...) |
| 2 | Loại thứ hai |
| 3 | Loại thứ ba |
| 14 | **Charge Item** — nạp tiền item cụ thể (`ChargeItemID` check) |

**Type 14** đặc biệt: check `everydayActivityConfig.Price.ZhiGouID == chargeItemBaseEventObject.ChargeItemConfig.ChargeItemID`

### 1.4 Event subscription

```csharp
// IEventListener implementation
public void processEvent(EventObject eventObject) {
    if (eventObject.getEventType() == 36)  // EventTypes.OnClientChargeItem
    {
        // Tìm ActID có Type==14 và Price.ZhiGouID match ChargeItemID
        // Nếu tìm được → gửi cmd 1507 tới client
    }
}
```

**Confirm hardcode**: `getEventType() == 36` = `EventTypes.OnClientChargeItem`

### 1.5 SystemParams đọc

| Param | Ý nghĩa |
|-------|---------|
| `EveryDayActivityOpen` | Bật/tắt toàn bộ EverydayActivity |
| `EveryDayChongZhiDuiHuan` | Config đổi nạp: format `"exchangeRate:bonusRate"` |

### 1.6 TCP Commands

| Cmd ID | Hướng | Mô tả |
|--------|-------|-------|
| **1507** | Server → Client | Notify charge item match |
| **770** | Server → Client | EverydayActivity state notify |

### 1.7 Methods quan trọng

| Method | Chức năng |
|--------|-----------|
| `Init()` | Load 3 XML files, register event listener |
| `LoadEverydayActivityTypeData()` | Parse EveryDayActivityType.xml |
| `LoadEverydayActivityGroupData()` | Parse EveryDayActivityGroup.xml |
| `LoadEverydayActivityData()` | Parse EveryDayActivity.xml (52KB) |
| `OnRoleLogin(client)` | Gửi state tới client khi login |
| `GetEverydayActivityDataForClient()` | Build response data |
| `EverydayActCheckCondition(client, actID, extTag)` | Check đủ điều kiện |
| `EverydayActGiveAward(client, actID, extTag)` | Phát thưởng |
| `BuildFetchEverydayActAwardCmd(client, groupID, actID)` | Build TCP cmd string |
| `NotifyActivityState(client)` | Push state mới tới client |
| `CacheNeedCondition(config)` | Cache điều kiện cho từng activity |
| `Dispose()` | Cleanup |

### 1.8 Chuỗi phụ thuộc hoàn chỉnh

```
GameServer startup
    ↓
EverydayActivity.Init()
    ↓  load
EveryDayActivityType.xml + Group.xml + Activity.xml
    ↓  register
EventSource.RegisterListener(EventTypes.OnClientChargeItem, this)

IN-GAME:
Player charges item
    ↓
EventSource.FireEvent(EventTypes.OnClientChargeItem, chargeEventObj)
    ↓
EverydayActivity.processEvent(eventObj)  [type==36]
    ↓
Find ActID where Type==14 && ZhiGouID matches
    ↓
sendCmd(1507, buildCmd, false)  → client

Player claims award:
    ↓
TCP Packet → EverydayActCheckCondition()
    ↓
EverydayActGiveAward()
    ↓
GoodsPackManager.GiveGoods()
    ↓
DB save
```

---

## 2. GameMU.Manager — Current State (README.md)

**Project name**: `GameMU.EventManager` (csproj)  
**Framework**: ASP.NET Core 8 + Razor Pages  
**Language**: C# với giao diện **tiếng Việt có dấu**

### 2.1 Tính năng đã hoàn thành

| Tính năng | Mô tả |
|-----------|-------|
| Quản lý sự kiện XML | Xem/lọc/tìm kiếm/sửa/thêm/xóa |
| Bật/Tắt sự kiện | 3 cơ chế: Flag, DateWindow, Park |
| Auto backup | Trước mỗi lần ghi → `.bak` file |
| SystemParams manager | ~939 tham số, giữ nguyên comment/format |
| Feature Map | 186 class GameServer + XML mapping (scan source) |
| GoodsAudit | FK validation GoodsID |
| Links | Link Resolution Service |
| Calendar | EventCalendar view |

### 2.2 Ba cơ chế Toggle (phát hiện từ README + EventModels.cs)

| Cơ chế | Cách hoạt động | Files áp dụng |
|--------|----------------|---------------|
| **Flag** | Attribute `IsOpen`/`Open` = `1`/`0` | VersionSystemOpen.xml, ThemeActivityOpen.xml |
| **DateWindow** | `FromDate`/`ToDate` quyết định window | JieRiGifts/*, HeFuGifts/*, SpecialActivityTime.xml |
| **Park** | Không có flag → Tắt = move record ra sidecar file | EventCalendar.xml, SpecialActivity.xml, EveryDayActivity*.xml, SystemOpen.xml |

**Park mechanism**:
- Tắt → record chuyển vào `Config/_EventManager/state/{key}.disabled.xml`
- Bật lại → khôi phục về XML gốc
- DateWindow → lưu khung ngày cũ vào `state/{key}.window.json`

### 2.3 Cấu trúc thư mục hoàn chỉnh

```
GameMU.EventManager/
├── Program.cs
├── appsettings.json         ← GameResConfigPath setting
├── Models/
│   └── EventModels.cs       ← EventFileDef, ToggleStrategy, EventRecord, ForeignKeyRef
├── Services/
│   ├── EventRegistry.cs     (29KB) ← danh mục 50+ XML + toggle strategy
│   ├── EventRegistry.Masters.snippet.cs  ← snippet helper
│   ├── FeatureCatalog.cs    (30KB) ← 186 GameServer class → XML mapping
│   ├── GoodsAuditService.cs (7KB)  ← FK validation GoodsID
│   ├── LinkRegistry.cs      (4KB)  ← link definitions
│   ├── LinkResolutionService.cs (4.5KB)
│   ├── ModuleRegistry.cs    (4KB)  ← 11 sidebar modules
│   ├── XmlEventService.cs   (15KB) ← core R/W/Edit/Add/Delete/Toggle + backup
│   └── XmlValidationService.cs (6.5KB) ← XML validation
└── Pages/
    ├── Index.cshtml          ← trang chủ, nhóm danh mục, đếm
    ├── File.cshtml  (8.7KB)  ← danh sách 1 file, lọc, toggle, sửa, xóa
    ├── Edit.cshtml  (5KB)    ← form thêm/sửa attributes
    ├── Calendar.cshtml (6.5KB) ← EventCalendar view
    ├── Features.cshtml (3KB) ← Feature → XML map
    ├── GoodsAudit.cshtml (8KB) ← GoodsID FK audit
    ├── Links.cshtml (7.5KB)  ← link resolution
    ├── Backups.cshtml        ← backup list/restore
    └── Shared/               ← layout
```

### 2.4 appsettings.json config

```json
{
  "EventManager": {
    "GameResConfigPath": "../GameMU-Assembly-CSharp/GameRes/GameRes/Config"
  }
}
```

Có thể override bằng env var: `EventManager__GameResConfigPath=/path/to/Config`

### 2.5 Safety model

```
Write operation
    ↓
Auto backup: Config/_EventManager/backups/{key}_{timestamp}.bak
    ↓
Write new XML (UTF-8 BOM, giữ attribute order)
    ↓
Park OFF: record → state/{key}.disabled.xml
Park ON: state/{key}.disabled.xml → restore to XML
DateWindow OFF: lưu DateRange → state/{key}.window.json
DateWindow ON: khôi phục từ window.json
```

---

## 3. Những gì còn thiếu trong GameMU.Manager

Từ phân tích README + Services list:

### 3.1 Chưa có (Phase 3)
```
- REST API /api/events/{key}     ← công cụ external cần
- Import/Export ZIP              ← batch backup
- Diff viewer (trước/sau sửa)   ← nice-to-have
- Performance test Goods.xml 15MB cache
```

### 3.2 Đang có nhưng cần mở rộng
- `EventRegistry.cs` (29KB): cần thêm các file XML chưa map
- `FeatureCatalog.cs` (30KB): 186 class, có thể còn thiếu ActivityNew classes mới
- `XmlValidationService.cs`: cần thêm GoodsData format validation (7 fields pipe-separated)

---

## 4. EventFileDef — Model thiết kế thông minh

```csharp
public class EventFileDef {
    string Key;          // URL slug
    string RelativePath; // path từ GameResConfigPath
    string DisplayName;  // tiếng Việt
    string Category;     // nhóm sidebar
    string ItemElement;  // XML tag con (Activity, System, EventCalendar...)
    string IdAttr;       // attribute ID (default "ID")
    string? NameAttr;    // attribute Name (nếu có)
    string[] ListColumns;// cột hiển thị trong danh sách

    ToggleStrategy Toggle; // None | Flag | DateWindow | Park
    // Flag fields:
    string? FlagAttr;    // IsOpen, Open...
    string FlagOn;       // "1"
    string FlagOff;      // "0"
    // DateWindow fields:
    string FromAttr;     // "FromDate"
    string ToAttr;       // "ToDate"

    List<ForeignKeyRef> ForeignKeys; // FK validation
}

public class ForeignKeyRef {
    string Field;          // thuộc tính trong file này
    string TargetKey;      // key file đích trong EventRegistry
    string TargetIdAttr;   // ID attr bên file đích
    bool MultiValue;       // "8002,8014" = nhiều ID
    char MultiSeparator;   // ','
    bool ParseRewardList;  // GoodsData pipe format
}
```

→ **Design rất tốt**: chỉ cần thêm EventFileDef mới là có CRUD/Toggle tự động cho XML mới

---

## 5. Tóm tắt tiến độ GameMU.Manager

| Hạng mục | Trạng thái |
|----------|-----------|
| Core CRUD engine | ✅ XmlEventService.cs |
| 50+ XML definitions | ✅ EventRegistry.cs |
| 186 Feature catalog | ✅ FeatureCatalog.cs |
| 3 Toggle strategies | ✅ Flag/DateWindow/Park |
| Auto backup | ✅ |
| SystemParams (939 params) | ✅ |
| GoodsAudit FK | ✅ |
| Links resolution | ✅ |
| Calendar view | ✅ |
| UI Vietnamese | ✅ |
| REST API | ❌ Phase 3 |
| Import/Export ZIP | ❌ Phase 3 |
| GoodsData visual editor | ❌ cần làm |
| Performance Goods.xml 15MB | ❌ cần test |
