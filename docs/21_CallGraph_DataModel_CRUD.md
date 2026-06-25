# 21_CallGraph_DataModel_CRUD.md
> Luồng hoạt động đầy đủ + Data Model CRUD + Admin Design
> Confidence: HIGH (từ code thực tế)
> Date: 2026-06-25

---

## 1. GlobalServiceManager — Server Bootstrap & Timer

**Path**: `GameServer/Logic/GlobalServiceManager.cs`

### 1.1 Lifecycle
```
GlobalServiceManager.initialize()
    ↓
GlobalServiceManager.startup()      ← đăng ký tất cả Manager
    ↓
GlobalServiceManager.TimerProc()    ← gọi mỗi tick cho mọi IManager
    ↓
GlobalServiceManager.showdown()
    ↓
GlobalServiceManager.destroy()
```

### 1.2 Các module được register (30+ module)
```
ActivityNew (JieRi/HeFu/Theme/EverydayActivity)
AoYunDaTi         BangHui.ZhanMengShiJian
BocaiSys          BossAI
Building          CheatGuard
Copy              FluorescentGem
FuMo              GoldAuction
Goods             JingJiChang
KuaFuIPStatistics LiXianBaiTan / LiXianGuaJi
Marriage          MoRi
Olympics          OnePiece
Ornament          Spread / Talent / Tarot
Ten               Today
UnionAlly         UnionPalace
UserActivate      UserReturn
ZhuanPan          ActivityNew.SevenDay
```

### 1.3 IManager interface pattern
```csharp
interface IManager {
    void TimerProc();  // Called every server tick
}
```
→ Mỗi Manager tự quản lý timer nội bộ (không có central scheduler kiểu cron)

---

## 2. ServerEvents.cs — Event logging system

**Không phải** game event dispatch — đây là **log ghi file**.

```
EventsQueue (Queue<string>)
    ↓  (async flush)
Year_YYYY/Month_MM/DD/{prefix}.log
```

Log được group theo DayOfYear (tự xoay file mỗi ngày).

---

## 3. Call Graph đầy đủ — Event System

```
╔══════════════════════════════════════════════════════╗
║  ENTRY POINTS                                        ║
║  TCP Packet → TCPCmdHandler.cs (1.7MB switch/case)  ║
║  HTTP API   → (Phase 3, chưa implement)              ║
║  Timer      → GlobalServiceManager.TimerProc()       ║
║  GameEvent  → EventSource.FireEvent(EventTypes.xxx)  ║
╚══════════════════════════════════════════════════════╝
         │
         ▼
╔══════════════════════════════════════════════════════╗
║  ACTIVITY MANAGERS                                   ║
║                                                      ║
║  DailyActiveManager (hằng ngày - bitmask)            ║
║    → DailyActiveInfor.xml + DailyActiveAward.xml     ║
║    → DB: DailyActiveFlag, DailyActiveInfo1           ║
║                                                      ║
║  EverydayActivity (hằng ngày - event-driven)         ║
║    → EveryDayActivity.xml (52KB)                     ║
║    → EveryDayActivityGroup.xml (12KB)                ║
║    → EventTypes.OnClientChargeItem (type=36)         ║
║    → DB: EverydayActInfoDB, EverydayActGroupInfoDB   ║
║                                                      ║
║  RechargeRepayActiveMgr (nạp tiền)                   ║
║    → EventCalendar.xml → Activity subclass           ║
║    → ActivityTypes: TotalCharge, InputFirst, ...     ║
║    → DB: serialize string "0,1,2,1,..."              ║
║                                                      ║
║  JieRiActivity (lễ hội) [33 types]                   ║
║    → JieRiType.xml → JieRiXxx.xml                   ║
║    → HuodongCachingMgr.GetJieriActivityConfig()      ║
║                                                      ║
║  HeFuActivity (hợp phục) [8 types]                   ║
║    → HeFuType.xml → HeFuXxx.xml                     ║
║    → HuodongCachingMgr.GetHeFuActivityConfing()      ║
║                                                      ║
║  ThemeActivity [8 types: 150-157]                    ║
║    → ThemeActivityXxx.xml                           ║
║    → HuodongCachingMgr.GetThemeActivityConfig()      ║
║                                                      ║
║  ActivityManagerNew (Singleton)                      ║
║    → JieriPlatChargeKing, JieriPCKingEveryDay        ║
╚══════════════════════════════════════════════════════╝
         │
         ▼
╔══════════════════════════════════════════════════════╗
║  REWARD LAYER                                        ║
║  Activity.GiveAward(client, param)                   ║
║    → GoodsPackManager.GiveGoods(client, goodsID, n)  ║
║    → GoodsPackManager.AddGoods() → Inventory DB      ║
║                                                      ║
║  GoodsData format: 7 fields pipe-separated           ║
║  "GoodsID,GCount,Binding,Forge,Append,Lucky,Excel"   ║
╚══════════════════════════════════════════════════════╝
         │
         ▼
╔══════════════════════════════════════════════════════╗
║  DB LAYER                                            ║
║  Global.SaveRoleParamsXxxToDB()                      ║
║    → DailyActiveFlag (ulong[] bitmask)               ║
║    → DailyActiveInfo1 (uint[])                       ║
║    → DailyActiveDayID (int - DayOfYear)              ║
║    → DailyActiveAwardFlag (int)                      ║
║    → EverydayActXxx (string serialized)              ║
║    → Stored Proc: DBCmd 13173 (OnMoneyCharge)        ║
╚══════════════════════════════════════════════════════╝
```

---

## 4. Thiết kế Data Model CRUD (mục 10 trong prompt)

### 4.1 DailyEventConfig
```csharp
public class DailyEventConfig {
    public int Id { get; set; }           // DailyActiveID (100,200,...,1600)
    public int UiOrder { get; set; }      // Tab.ID (UI display order)
    public string Name { get; set; }      // Tên nhiệm vụ
    public int MinLevel { get; set; }     // Minleve
    public int MinRebirth { get; set; }   // MinZhuanshengleve
    public int? LoginCount { get; set; }  // Login condition
    public int? OnlineMinutes { get; set; }// Online condition
    public int? ConsumptionCount { get; set; }
    public int? DailyTaskCount { get; set; }
    public int? DungeonCount { get; set; }
    public int? EventCount { get; set; }
    public int? ForgeCount { get; set; }
    public int? AppendCount { get; set; }
    public int? KillMonsterCount { get; set; }
    public int? KillBossCount { get; set; }
    public string CraftItemCondition { get; set; } // "itemID,count"
    public int AwardPoints { get; set; }           // Award
    public bool IsActive { get; set; }
}
```
**XML**: DailyActiveInfor.xml, DailyActiveAward.xml

### 4.2 JieRiEventConfig
```csharp
public class JieRiEventConfig {
    public int Id { get; set; }              // ActivityType (9-17, 50-77)
    public string Name { get; set; }         // Tên lễ hội
    public string DataFile { get; set; }     // PeiZhi = JieRiXxx.xml
    public string FromDate { get; set; }     // Bắt đầu
    public string ToDate { get; set; }       // Kết thúc
    public string AwardStartDate { get; set; }
    public string AwardEndDate { get; set; }
    public bool IsEnabled { get; set; }      // Toggle on/off
    public List<JieRiAwardConfig> Awards { get; set; }
}

public class JieRiAwardConfig {
    public int DayOrTier { get; set; }       // TimeOl = ngày thứ N
    public string GoodsOne { get; set; }     // Pipe-separated GoodsData
    public string GoodsTwo { get; set; }
    public string GoodsThr { get; set; }
    public string EffectiveTime { get; set; }
}
```

### 4.3 RechargeEventConfig
```csharp
public class RechargeEventConfig {
    public int ActivityType { get; set; }    // ActivityTypes enum
    public string Name { get; set; }
    public string FromDate { get; set; }
    public string ToDate { get; set; }
    public List<RechargeMilestone> Milestones { get; set; }
}

public class RechargeMilestone {
    public int MinAmount { get; set; }       // Điều kiện nạp tối thiểu
    public string GoodsReward { get; set; }  // GoodsData format
    public bool IsRepeatable { get; set; }   // Có thể nhận lại không
}
```

### 4.4 ShopItemConfig
```csharp
public class ShopItemConfig {
    public int Id { get; set; }
    public int GoodsId { get; set; }
    public int Price { get; set; }
    public string Currency { get; set; }     // Gold/BindGold/Coin/Cash
    public int ForgeLevel { get; set; }
    public int AppendPropLev { get; set; }
    public int Lucky { get; set; }
    public int ExcellenceInfo { get; set; }
    public int? VipRequired { get; set; }
    public int? LevelRequired { get; set; }
    public int? DailyLimit { get; set; }
    public bool IsActive { get; set; }
}
```
**XML**: Mall.xml, QiZhenGe.xml, ZhanGongMall.xml

### 4.5 GiftCodeConfig
```csharp
public class GiftCodeConfig {
    public string Code { get; set; }
    public string GoodsReward { get; set; }
    public int? MaxUses { get; set; }
    public int UsedCount { get; set; }
    public string ExpireDate { get; set; }
    public bool IsActive { get; set; }
}
```
**XML**: GiftCodeNew.xml (214KB)

### 4.6 SystemParamConfig
```csharp
public class SystemParamConfig {
    public string ParamName { get; set; }    // Key
    public string Value { get; set; }        // Giá trị (dạng string)
    public string Category { get; set; }     // Nhóm
    public string Description { get; set; }
}
```
**XML**: SystemParams.xml (147KB)

---

## 5. Admin CRUD Design (mục 11 trong prompt)

### 5.1 Module map (GameMU.Manager)

| Module | URL | CRUD | Backup |
|--------|-----|------|--------|
| Daily Active | /daily-active | ✅ | ✅ |
| JieRi Events | /jieri | ✅ | ✅ |
| HeFu Events | /hefu | ✅ | ✅ |
| Theme Events | /theme | ✅ | ✅ |
| Recharge | /recharge | ✅ | ✅ |
| Shop/Mall | /mall | ✅ | ✅ |
| Gift Code | /giftcode | ✅ | ✅ |
| SystemParams | /params | ✅ | ✅ |
| EventCalendar | /calendar | ✅ | ✅ |
| EveryDay Activity | /everyday | ✅ | ✅ |
| Goods Audit | /goods-audit | Read | - |

### 5.2 Các operation cần có cho mỗi Event XML

```
[GET]    /api/events/{type}           → Load current XML config
[PUT]    /api/events/{type}           → Save XML config
[POST]   /api/events/{type}/toggle    → Enable/Disable
[POST]   /api/events/{type}/clone     → Clone từ event khác
[GET]    /api/events/{type}/backup    → List backup versions
[POST]   /api/events/{type}/restore   → Restore từ backup
[POST]   /api/events/import           → Import ZIP
[GET]    /api/events/export           → Export ZIP
```

### 5.3 GoodsData editor UI

Vì GoodsData dạng `"63262,50,1,0,0,0,0|9990001,200,1,0,0,0,0"` khó đọc:
→ Cần UI parser: split `|` → split `,` → form có 7 fields per row
→ FK validation với Goods.xml (GoodsID phải tồn tại)

---

## 6. ERD (Entity Relationship)

```
EventCalendar (ActivityType FK → Activity subclass)
    ↓ 1:N
ActivityConfig (FromDate, ToDate, AwardStartDate, AwardEndDate)
    ↓ 1:N  
AwardTier (TimeOl/Condition, GoodsOne, GoodsTwo, GoodsThr)
    ↓ N:M
GoodsItem (GoodsID → Goods.xml)

DailyActiveConfig (DailyActiveID, condition fields, AwardPoints)
    ↓ 1:N
DailyActiveAward (points threshold → GoodsReward)

Player (RoleID)
    ↓ 1:1
DailyActiveRecord (DailyActiveFlag bitmask, DayID, AwardFlag)
    ↓ 1:N
EverydayActRecord (ActID, progress, claimed)
    ↓ N:1
EverydayActivityConfig (ActID, Type, GoalValue, GoodsReward)
```

---

## 7. Sequence Diagram — JieRi Login Award

```
Client          TCPCmdHandler       JieRiActivity        HuodongCachingMgr     DB
  │                   │                   │                      │              │
  │──[TCP nID=xxx]──→ │                   │                      │              │
  │                   │──FindClient()──→  │                      │              │
  │                   │──GetActivity()──→ │                      │              │
  │                   │                   │──GetJieriConfig()──→ │              │
  │                   │                   │←──jieriConfig────────│              │
  │                   │                   │──InActivityTime()?   │              │
  │                   │                   │──CheckCondition()?   │              │
  │                   │                   │──HasBagSpace()?      │              │
  │                   │                   │──GiveAward()──────→  │              │
  │                   │                   │                      │──SaveDB()──→ │
  │                   │                   │←──success────────────│              │
  │←─[result:roleId]─ │                   │                      │              │
```

---

## 8. State Machine — Activity Lifecycle

```
[INACTIVE]
    ↓  (FromDate reached)
[ACTIVE]
    ↓  (player claims)
[CLAIMED] ← per player per tier
    ↓  (ToDate passed OR Toggle OFF)
[EXPIRED]
    ↓  (admin restores backup OR new season)
[INACTIVE]

Toggle pattern (XmlEventService.OffSentinel = "2000-01-01"):
    Admin sets ToDate = "2000-01-01" → server sees year <= 2001 → OFF
```

---

## 9. Kế hoạch migrate từng bước (hoàn chỉnh)

### Phase 1 — Read-Only Analysis (DONE ✅)
- [x] Đọc source code GameServer
- [x] Map XML ↔ Class
- [x] Identify hardcodes
- [x] Document schemas

### Phase 2 — GameMU.Manager (ĐANG LÀM 🔄)
- [x] EventRegistry.cs (50+ XML defs)
- [x] XmlEventService.cs (R/W/Cache/Backup)
- [x] EventModels.cs (DTO)
- [x] ModuleRegistry.cs (11 modules)
- [x] Pages: Index, File, Edit, Backups, Calendar, Features, GoodsAudit, Links
- [ ] REST API `/api/events/{key}` (Phase 3)
- [ ] Import/Export ZIP
- [ ] Performance test Goods.xml 15MB

### Phase 3 — Server Refactor (CẦN RESTART)
- [ ] Extract hardcode IDs → XML config
  - DailyActiveManager.InitDailyActiveFlagIndex() → DailyActiveInfor.xml
  - Packet 558 → TCPGameServerCmds const
  - EventType 36 → EventTypes.OnClientChargeItem const
  - DBCmd 13173 → DBCommandIDs.MoneyCharge const
- [ ] Replace TCPCmdHandler giant switch → dispatcher pattern
  - ActivityManagerNew pattern đã dùng Singleton → expand
- [ ] IEventConfigProvider interface
- [ ] IEventRepository interface
- [ ] EventCenter unified entry
- [ ] Unit tests

---

## 10. File tiếp theo cần đọc (theo priority)

| Priority | File | Size | Lý do |
|----------|------|------|-------|
| 🔴 HIGH | `GoodsPackManager.cs` | 119KB | Reward distribution engine |
| 🔴 HIGH | `EverydayActivity.cs` | 48KB | Hệ thống chính EverydayActivity |
| 🟡 MED | `ReloadXmlManager.cs` | 36KB | Hot-reload XML cơ chế |
| 🟡 MED | `GiftCodeNewManager.cs` | 10KB | Gift code flow |
| 🟡 MED | `SystemParams.xml` | 147KB | Config params inventory |
| 🟢 LOW | `SpecPriorityActivity.cs` | 71KB | Special activity complex |
| 🟢 LOW | `HuodongCachingMgr.cs` | ? | God Object cần split |
