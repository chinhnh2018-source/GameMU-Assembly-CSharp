# 19_ActivitySystem_Analysis.md
> Phân tích từ code thực tế. Confidence: HIGH (đọc trực tiếp source .cs + XML)
> Date: 2026-06-25

---

## 1. Phân tích các hệ thống sự kiện

### 1.1 ActivityTypes enum — TẤT CẢ loại activity (từ ActivityTypes.cs)

| ID | Tên | Nhóm |
|----|-----|------|
| 0 | None | - |
| 1 | InputFirst | Nạp tiền lần đầu |
| 2 | InputFanLi | Nạp hoàn |
| 3 | InputJiaSong | Nạp tặng |
| 4 | InputKing | Vua nạp |
| 5 | LevelKing | Vua cấp độ |
| 6 | EquipKing | Vua trang bị |
| 7 | HorseKing | Vua ngựa |
| 8 | JingMaiKing | Vua kinh mạch |
| 9 | JieriDaLiBao | Lễ hội - Đại lễ bao |
| 10 | JieriDengLuHaoLi | Lễ hội - Đăng nhập hào lễ |
| 11 | JieriVIP | Lễ hội - VIP |
| 12 | JieriCZSong | Lễ hội - Nạp tặng |
| 13 | JieriLeiJiCZ | Lễ hội - Tích lũy nạp |
| 14 | JieriZiKa | Lễ hội - Zicard |
| 15 | JieriPTXiaoFeiKing | Lễ hội - Vua tiêu |
| 16 | JieriPTCZKing | Lễ hội - Vua nạp PT |
| 17 | JieriBossAttack | Lễ hội - Đánh Boss |
| 20 | HeFuLogin | Hợp phục - Đăng nhập |
| 21 | HeFuTotalLogin | Hợp phục - Tổng đăng nhập |
| 22 | HeFuShopLimit | Hợp phục - Shop giới hạn |
| 23 | HeFuRecharge | Hợp phục - Nạp tiền |
| 24 | HeFuPKKing | Hợp phục - Vua PK |
| 25 | HeFuAwardTime | Hợp phục - Thời gian thưởng |
| 26 | HeFuBossAttack | Hợp phục - Đánh Boss |
| 27 | MeiRiChongZhiHaoLi | Nạp tiền hằng ngày hào lễ |
| 28 | ChongJiLingQuShenZhuang | Nạp tích lấy Thần Trang |
| 29 | ShenZhuangJiQingHuiKui | Thần Trang hồi quỹ |
| 30 | XinCZFanLi | Nạp mới hoàn |
| 37 | TotalCharge | Tổng nạp |
| 38 | TotalConsume | Tổng tiêu |
| 39 | JieriTotalConsume | Lễ hội - Tổng tiêu |
| 40-42 | JieriDuoBei/QiangGou | Lễ hội - Đa bội/Cướp mua |
| 46 | EverydayActivity | Hoạt động hàng ngày (EveryDay system) |
| 48 | InputFanLiNew | Nạp hoàn mới |
| 50-77 | JieRi* | Lễ hội các loại |
| 100 | JieriPlatChargeKing | Vua nạp Platform |
| 110-114 | TriennialRegress* | Hồi quy 3 năm |
| 150-157 | Theme* | Theme (Chủ đề: ZhiGou/DaLiBao/JingYan/Boss/MoYu/ZS) |
| 999 | TenReturn | Trở về 10 ngày |
| 1000 | PlatFuLiUC | Phúc lợi Platform UC |

**Confidence: HIGH** — Đọc trực tiếp từ ActivityTypes.cs

---

### 1.2 DailyActive System (hằng ngày - bitmask)

19 IDs từ DailyActiveTypes.cs:

| ID | Tên | Mô tả |
|----|-----|-------|
| 100 | LoginGameCount | Đăng nhập |
| 200 | SeriesLogin | Đăng nhập liên tiếp |
| 300 | MallBuyCount | Mua Mall |
| 400 | CompleteDailyTaskCount1 | Hoàn thành nhiệm vụ ngày 1 |
| 401 | CompleteDailyTaskCount2 | Hoàn thành nhiệm vụ ngày 2 |
| 500 | CompleteNormalCopyMapCount1 | Hoàn thành bản đồ thường |
| 600 | CompleteHardCopyMapCount1 | Hoàn thành bản đồ khó |
| 700 | CompleteDifficltCopyMapCount1 | Hoàn thành bản đồ cực khó |
| 800 | CompleteBloodCastle | Blood Castle |
| 900 | CompleteDaimonSquare | Daimon Square |
| 1000 | CompleteBattle | Battle |
| 1100 | EquipForge | Rèn trang bị |
| 1200 | EquipAppend | Gắn phụ kiện trang bị |
| 1300 | KillMonster1 | Giết quái lv1 |
| 1301 | KillMonster2 | Giết quái lv2 |
| 1302 | KillMonster3 | Giết quái lv3 |
| 1400 | KillBoss | Giết Boss |
| 1500 | CompleteChangeLife | Đổi nghề |
| 1600 | MergeFruit | Hợp trái |

**DB fields**: DailyActiveFlag (ulong[] bitmask), DailyActiveInfo1 (uint[]), DailyActiveDayID (int), DailyActiveAwardFlag (int)

---

### 1.3 JieRi (Lễ hội) — 33 types được nhận diện

Từ Activity.cs `IsJieRiActivity()`:
Types: 9, 10, 12-17, 40-42, 50-62, 64, 66-70, 75-77

Config: `JieriActivityConfig` từ `HuodongCachingMgr.GetJieriActivityConfig()`

---

### 1.4 HeFu (Hợp phục) — types 20-25

Config: `HeFuActivityConfig` từ `HuodongCachingMgr.GetHeFuActivityConfing()`

---

### 1.5 Theme Activity — types 150-157

| ID | Tên |
|----|-----|
| 150 | ThemeZhiGou (Chỉ cấu) |
| 151 | ThemeDaLiBao |
| 152 | ThemeJingYan |
| 153 | ThemeSpec |
| 154 | ThemeDuiHuan |
| 155 | ThemeBoss |
| 156 | ThemeMoYu |
| 157 | ThemeZS |

Config: `ThemeActivityConfig`, check `ActivityOpenVavle > 0`

---

## 2. Phân tích luồng hoạt động

### 2.1 Entry Points (từ GameServer/Server/)

```
TCP Packet  →  TCPCmdHandler.cs (1.7MB !!!)
                ↓  switch(nID) / case
             Handler Method  (TCPProcessCmdResults Xxx(TCPManager, TMSKSocket, ...))
                ↓
             Manager/Service (DailyActiveManager, RechargeRepayActiveMgr, ...)
                ↓
             XML Config (HuodongCachingMgr → SystemXmlItem)
                ↓
             DB (Global.SaveRoleParamsXxx)
```

### 2.2 Recharge Flow (từ RechargeRepayActiveMgr.cs)

```
Packet → QueryAllRechargeRepayActiveInfo()
       → QueryRechargeRepayActive()        ← query current state
       → ProcessGetRepayAwardCmd()         ← claim reward
           ↓
         Activity.GetAwardMinConditionlist()
           ↓
         CheckRechargeReplay()
           ↓
         BuildWriteActiveRecordStr()       ← serialize to DB string
           ↓
         BroadcastActiveHint()             ← notify client (Packet 528/529/530)
```

**ActivityTypes trong Recharge**: TotalCharge, TotalConsume, InputFirst, InputFanLiNew, MeiRiChongZhiHaoLi, HeFuLogin, HeFuTotalLogin, HeFuPKKing, HeFuLuoLan, HeFuRecharge, OneDollarChongZhi

### 2.3 DailyActive Flow

```
Login → InitRoleDailyActiveData()
       → check DailyActiveDayID vs TimeUtil.NowDateTime().DayOfYear
       → if new day: CleanDailyActiveInfo() → reset all DB fields
       → NotifyClientDailyActiveData() [Packet 558 hardcoded]

In-game → ProcessDailyActiveKillMonster()
        → ProcessOnlineForDailyActive()
        → ProcessBuyItemInMallForDailyActive()
        → OnDailyActiveCompleted() → GiveDailyActiveAward()
```

---

## 3. Mapping XML → Class

| XML File | Loader/Manager | Class sử dụng | Confidence |
|----------|----------------|---------------|------------|
| DailyActiveInfor.xml | GameManager.systemDailyActiveInforMgr | DailyActiveManager | HIGH |
| DailyActiveAward.xml | GameManager.systemDailyActiveAwardMgr | DailyActiveManager | HIGH |
| EventCalendar.xml | HuodongCachingMgr | Activity (IsInActivityTime) | HIGH |
| EveryDayActivity.xml (52KB) | EveryDayActivity loader | EverydayActivity | HIGH |
| EveryDayActivityGroup.xml (12KB) | EveryDayActivity loader | EverydayActivity | HIGH |
| EveryDayActivityType.xml | EveryDayActivity loader | EverydayActivity | HIGH |
| Mall.xml | GameManager.systemMallMgr | MallGoodsMgr | HIGH |
| QiZhenGeGoods.xml | GameManager.systemQiZhenGeGoodsMgr | MallGoodsMgr | HIGH |
| SystemParams.xml (147KB) | GameManager.systemParamsMgr | GlobalServiceManager | HIGH |
| SystemOpen.xml (44KB) | VersionSystemOpenManager | System open check | HIGH |
| GiftCodeNew.xml (214KB) | GiftCodeNewManager | GiftCodeNewManager | HIGH |
| Activity/ActivityTip.xml | ActivityTipTypes | UI tips | HIGH |
| Activity/BossInfo.xml | MonsterBossManager | Boss spawn times | HIGH |
| Activity/Copy.xml | CopyMapManager | Dungeon config | HIGH |
| ThemeActivity*.xml (nhiều file) | HuodongCachingMgr | ThemeActivityConfig | HIGH |

---

## 4. Reverse Schema XML

### 4.1 Activity Base Fields (từ Activity.cs)

```xml
<Activity>
  <FromDate>2024-01-01 00:00:00</FromDate>   <!-- Required: start time -->
  <ToDate>2024-12-31 23:59:59</ToDate>       <!-- Required: end time -->
  <AwardStartDate>-1</AwardStartDate>        <!-- Nullable: "-1" = use FromDate -->
  <AwardEndDate>-1</AwardEndDate>            <!-- Nullable: "-1" = use ToDate -->
  <ActivityType>37</ActivityType>            <!-- Required: ActivityTypes enum -->
</Activity>
```

**Giải thích**:
- `FromDate/ToDate` → parse bằng `DateTime.TryParse()`, fail → fallback `2008-08-08 08:08:08`
- `AwardStartDate/AwardEndDate = "-1"` → dùng `2008-08-08` đến `2028-08-08` (hardcode!)
- `ActivityKeyStr` = `"{FromDate}_{ToDate}"` replace `:` → `$`

### 4.2 MallGoods Schema (từ MallGoodsMgr.cs)

```xml
<Item ID="1001">
  <GoodsID>10001</GoodsID>        <!-- Required -->
  <Price>500</Price>               <!-- Required: giá tính bằng Gold/Cash -->
  <Property>0,0,0,0</Property>    <!-- Required: "Forge_level,AppendPropLev,Lucky,ExcellenceInfo" -->
</Item>
```

---

## 5. Phân tích thời gian

### 5.1 Time APIs sử dụng

| Pattern | File | Mục đích |
|---------|------|---------|
| `TimeUtil.NowDateTime().DayOfYear` | DailyActiveManager.cs | Reset daily |
| `Timer` (NewTimerProc.cs) | TimedActionManager | Scheduled tasks |
| `DateTime.TryParse(FromDate)` | Activity.cs | Activity window |
| `DateTime.Now` | SpecailTimeManager.cs | Special time check |

### 5.2 Reset logic (DailyActive)

```csharp
int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
// Nếu dayOfYear != DailyActiveDayID → reset tất cả
Global.SaveRoleParamsInt32ValueToDB(client, "DailyActiveDayID", dayOfYear, true);
```
→ **Reset hàng ngày** theo DayOfYear (không phải datetime cụ thể)

### 5.3 Hardcode dates (cần chú ý)

| Class | Giá trị | Ý nghĩa |
|-------|---------|---------|
| Activity.cs | `"2008-08-08 08:08:08"` | Default StartTime khi parse fail |
| Activity.cs | `"2028-08-08 08:08:08"` | Default EndTime khi parse fail |
| Activity.cs | `"2000-01-01 00:00:00"` | Sentinel OFF value |
| FestivalManager | `to.Year <= 2001` | Check tắt festival |

---

## 6. Phân tích Item

### 6.1 Chuỗi phụ thuộc

```
EventCalendar.xml (ActivityType)
    ↓
Activity.GetAwardMinConditionlist()
    ↓
RewardID / GoodsID
    ↓
GoodsPackManager.GiveGoods(client, goodsID, count)
    ↓
GoodsPackManager.AddGoods() → Inventory
    ↓
SaveRoleDailyActiveData() → DB
```

### 6.2 MallGoods Item schema

```
GoodsID  →  Price (Gold/BindGold/Coin/Cash tùy loại mall)
Property = "Forge_level, AppendPropLev, Lucky, ExcellenceInfo"
```

---

## 7. Phân tích Shop

### 7.1 Mall loại

| Mall | Manager/XML | Currency |
|------|-------------|----------|
| systemMallMgr | Mall.xml | BindGold/Cash |
| systemQiZhenGeGoodsMgr | QiZhenGeMall.xml | Gold |
| ZhanGongMall | ZhanGongMall.xml | ZhanGong points |

### 7.2 MallGoodsCacheItem fields

```csharp
{
    Price,         // giá
    Forge_level,   // mức rèn
    AppendPropLev, // mức phụ kiện
    Lucky,         // lucky
    ExcellenceInfo // excellence
}
```

---

## 8. Phân tích nạp tiền

### 8.1 Recharge Flow

```
Client TCP Packet (nID=xxx)
    ↓
TCPCmdHandler.cs → RechargeRepayActiveMgr.QueryAllRechargeRepayActiveInfo()
    ↓
foreach ActivityTypes in [TotalCharge, TotalConsume, InputFirst, ...]
    activity = Global.GetActivity(type)
    awardList = activity.GetAwardMinConditionlist()
    state = GetBtnIndexState(money, minMoney, recode)
        → 0: chưa đủ
        → 1: đủ điều kiện, chưa nhận
        → 2: đã nhận
    ↓
BuildWriteActiveRecordStr() → serialize "0,1,2,1,..." → save DB
    ↓
ProcessGetRepayAwardCmd() → claim
    ↓
BroadcastActiveHint() → GLang.GetLang(528/529/530) → TCP broadcast
```

### 8.2 Hardcode Packet IDs trong Recharge

| GLang ID | Nội dung |
|----------|---------|
| 528 | Text: "TotalCharge" broadcast |
| 529 | Text: "TotalConsume" broadcast |
| 530 | Template: "{roleName} {text}" |

---

## 9. Phân tích ActivityNew folder

Các Activity classes mới (trong `GameServer/Logic/ActivityNew`):
- Kế thừa từ `Activity` base class
- Override `InActivityTime()`, `CheckCondition()`, `GetAwardMinConditionlist()`
- Config đọc từ `HuodongCachingMgr`

---

## 10. Hardcode phát hiện ✅

| Vị trí | Giá trị | Mức độ | Cần sửa |
|--------|---------|--------|---------|
| DailyActiveManager.InitDailyActiveFlagIndex() | IDs: 100,200,300,400-401,500,600,700,800,900,1000,1100,1200,1300-1302,1400,1500,1600 | 🔴 HIGH | → config XML |
| DailyActiveManager.NotifyClientDailyActiveData() | Packet 558 | 🔴 HIGH | → constant |
| Activity.cs | DateTime "2008-08-08 08:08:08" | 🟡 MEDIUM | → config |
| Activity.cs | DateTime "2028-08-08 08:08:08" | 🟡 MEDIUM | → config |
| RechargeRepayActiveMgr | GLang 528/529/530 | 🟡 MEDIUM | → resource |
| EverydayActivity.processEvent() | EventType 36 | 🔴 HIGH | → enum |
| EverydayActivity.OnMoneyChargeEvent() | DB Cmd 13173 | 🔴 HIGH | → constant |
| XmlEventService.OffSentinel | "2000-01-01 00:00:00" | 🟡 MEDIUM | → constant |
| FestivalManager | to.Year <= 2001 | 🔴 HIGH | → config |

---

## 11. Cây phụ thuộc

```
Player (TCP)
    │
    ▼
TCPCmdHandler.cs (1.7MB) ─────────── GameServer/Server/
    │
    ├── DailyActiveManager ──────── DailyActiveInfor.xml, DailyActiveAward.xml
    │       │
    │       └── GoodsPackManager ── Goods.xml (15MB)
    │
    ├── RechargeRepayActiveMgr ──── ActivityTypes enum
    │       │
    │       └── Activity subclasses ─ EventCalendar.xml (via HuodongCachingMgr)
    │                │
    │                └── JieRiActivity ─ JieRiGifts/
    │                └── HeFuActivity ─ HeFuGifts/
    │                └── ThemeActivity ─ ThemeActivity*.xml
    │
    ├── EverydayActivityManager ─── EveryDayActivity.xml (52KB)
    │                           ─── EveryDayActivityGroup.xml (12KB)
    │                           ─── EveryDayActivityType.xml
    │
    ├── MallGoodsMgr ────────────── Mall.xml, QiZhenGe.xml, ZhanGongMall.xml
    │
    ├── GiftCodeNewManager ────── GiftCodeNew.xml (214KB)
    │
    └── VersionSystemOpenManager ─ SystemOpen.xml (44KB)
                                 ─ SystemParams.xml (147KB)
```

---

## 12. Đề xuất EventService + Repository + ConfigProvider

### 12.1 Vấn đề hiện tại
- `HuodongCachingMgr` làm quá nhiều việc (God Object)
- `TCPCmdHandler.cs` 1.7MB = không tách được test
- Hardcode IDs rải rác
- Config đọc thẳng từ `SystemXmlItem` không qua interface

### 12.2 Đề xuất thiết kế

```csharp
// ConfigProvider — tách đọc XML
interface IEventConfigProvider {
    ActivityConfig GetActivity(ActivityTypes type);
    DailyActiveConfig GetDailyActive(int id);
    MallConfig GetMallGoods(int goodsId);
}

// Repository — tách DB  
interface IEventRepository {
    DailyActiveData GetDailyActive(int roleId);
    void SaveDailyActive(int roleId, DailyActiveData data);
    RechargeRecord GetRechargeRecord(int roleId, ActivityTypes type);
}

// EventService — business logic
class EventService {
    IEventConfigProvider _config;
    IEventRepository _repo;

    bool CheckDailyActive(int roleId, int activityId);
    void ClaimReward(int roleId, int activityId);
    RechargeStatus GetRechargeStatus(int roleId, ActivityTypes type);
}

// EventCenter — unified entry
class EventCenter {
    EventService _eventSvc;
    // Single entry point thay thế TCPCmdHandler chaos
}
```

---

## 13. Kế hoạch migrate từng bước

### Phase 1: Read-only (không ảnh hưởng server)
1. Extract `IEventConfigProvider` — wrap SystemXmlItem hiện tại
2. Extract `IEventRepository` — wrap Global.SaveRoleParams* 
3. Viết unit test cho logic

### Phase 2: GameMU.Manager UI (đang làm)
1. REST API `/api/events/{key}` — CRUD EventCalendar
2. REST API `/api/daily-active` — CRUD DailyActive config
3. REST API `/api/mall` — CRUD Mall
4. Backup/Restore XML

### Phase 3: Server refactor (cần restart server)
1. Replace hardcoded IDs → config XML
2. Replace TCPCmdHandler giant switch → dispatcher pattern
3. Extract EventCenter

---

*File tiếp theo cần đọc*:
- [ ] `GameServer/Logic/ActivityNew/` — các activity class mới
- [ ] `GameServer/Core/GameEvent/` — event dispatcher
- [ ] `GameServer/Logic/GoodsPackManager.cs` — reward system (119KB)
- [ ] `GameRes/GameRes/Config/JieRiGifts/` — XML lễ hội
- [ ] `GameRes/GameRes/Config/HeFuGifts/` — XML hợp phục
