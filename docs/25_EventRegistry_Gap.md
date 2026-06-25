# 25_EventRegistry_Gap_Analysis.md
> EventRegistry.cs analysis + Gap + Code snippet
> Confidence: HIGH (đọc trực tiếp source)
> Date: 2026-06-25

---

## 1. EventRegistry.cs — Tổng quan

**Path**: `GameMU.Manager/Services/EventRegistry.cs`  
**Size**: 29KB / 23,271 chars  
**Total entries**: 35 files đã đăng ký  

### 1.1 Phân bố theo Toggle Strategy

| Strategy | Số files | Ý nghĩa |
|----------|----------|---------|
| Park | 20 | Không có flag → move ra sidecar khi tắt |
| None | 11 | Chỉ xem/sửa, không bật/tắt |
| DateWindow | 2 | Điều khiển bằng FromDate/ToDate |
| Flag | 2 | Có attribute IsOpen/Open = 1/0 |

### 1.2 Phân bố theo Category (12 nhóm)

| Category | Số files |
|----------|---------|
| Từ điển gốc (Goods, Monsters...) | 9 |
| Sự kiện chủ đề (ThemeActivity) | 5 |
| Tab hoạt động (HuoDongTab) | 4 |
| Hoạt động hằng ngày (EveryDay) | 3 |
| Sự kiện 7 ngày (SevenDay) | 3 |
| Hoạt động khác (Activity/) | 3 |
| Hoạt động đặc biệt (Special) | 2 |
| Mở hệ thống (SystemOpen) | 2 |
| Tham số hệ thống | 1 |
| Lịch sự kiện | 1 |
| Hồi quy / Gộp server | 1 |
| Ánh xạ vật phẩm | 1 |

### 1.3 Danh sách 35 files đã đăng ký

| Key | RelativePath | Toggle |
|-----|-------------|--------|
| system-params | SystemParams.xml | None |
| event-calendar | EventCalendar.xml | Park |
| special-activity | SpecialActivity/SpecialActivity.xml | Park |
| special-activity-time | SpecialActivity/SpecialActivityTime.xml | DateWindow |
| everyday-activity | EveryDayActivity/EveryDayActivity.xml | Park |
| everyday-activity-group | EveryDayActivity/EveryDayActivityGroup.xml | Park |
| everyday-activity-type | EveryDayActivity/EveryDayActivityType.xml | Park/Flag |
| version-system-open | VersionSystemOpen.xml | Flag |
| system-open | SystemOpen.xml | Park |
| theme-open | ThemeActivityOpen.xml | Flag |
| theme-type | ThemeActivityType.xml | Park |
| theme-zhigou | ThemeActivityZhiGou.xml | DateWindow |
| theme-boss | ThemeActivityBOSS.xml | Park |
| theme-zhuansheng | ThemeActivityZhuanSheng.xml | Park |
| huigui-huodong | HuiGuiHuoDong.xml | Park |
| sevenday-goal | SevenDay/SevenDayGoal.xml | Park |
| sevenday-type | SevenDay/SevenDayActivityType.xml | Park |
| sevenday-qianggou | SevenDay/SevenDayQiangGou.xml | Park |
| huodong-tab | HuoDongTab.xml | Park |
| kuafu-huodong-tab | KuaFuHuoDongTab.xml | Park |
| zhandui-huodong-tab | ZhanDuiHuoDongTab.xml | Park |
| zhanmeng-huodong-tab | ZhanMengHuoDongTab.xml | Park |
| activity-copy | Activity/Copy.xml | None |
| activity-boss | Activity/BossInfo.xml | None |
| activity-tip | Activity/ActivityTip.xml | None |
| get-goods | GetGoods.xml | None |
| goods | Goods.xml | None |
| monster | Monsters.xml | None |
| magic | Magics.xml | None |
| map | MapConfig.xml | None |
| npc | npcs.xml | None |
| fuben | FuBen.xml | None |
| era-task | EraTask.xml | None |
| legion-tasks | LegionTasks.xml | None |
| xilian | XiLianShuXing.xml | None |

---

## 2. GAP ANALYSIS — Files chưa đăng ký

### 2.1 JieRiGifts (45 files) — MISSING ❌

Tất cả 45 files trong `GameRes/GameRes/Config/JieRiGifts/` chưa có trong EventRegistry.

**Quan trọng nhất** (theo ActivityTypes mapping):
| ActivityType | File | Mô tả |
|-------------|------|-------|
| 9 | JieRiLiBao.xml | Lễ hội - Đại lễ bao |
| 10 | JieRiDengLu.xml | Đăng nhập 7 ngày |
| 11 | JieRiVip.xml | VIP đặc quyền |
| 12 | JieRiChongZhiSong.xml | Nạp tặng |
| 13 | JieRiLeiJi.xml | Tích lũy nạp |
| 14 | JieRiBaoXiang.xml | Hộp quà thẻ chữ |
| 15 | JieRiXiaoFeiKing.xml | Vua tiêu thụ |
| 16 | JieRiChongZhiKing.xml | Vua nạp |
| 17 | JieRiBOSS.xml | Boss công thành |
| 50+ | JieRiChongZhiQiangGou.xml (16KB) | Cướp mua |
| 70+ | JieRiMeiRiLeiJi.xml (7KB) | Tích lũy ngày |
| - | JieRiMeiRiChongZhiWang.xml (13KB) | Ngày nạp vương |
| - | MuJieRiType.xml | Type mapping mở rộng |

### 2.2 HeFuGifts (12 files) — MISSING ❌

| ActivityType | File | Mô tả |
|-------------|------|-------|
| 20 | HeFuLiBao.xml | Đăng nhập hào lễ |
| 21 | HeFuDengLu.xml | Tích lũy đăng nhập |
| 22 | HeFuQiangGou.xml | Shop giới hạn mua |
| 23 | HeFuFanLi.xml | Nạp hoàn tiền |
| 24 | PKJiangLi.xml | Vua PK |
| 25 | HeFuZhangChang.xml | Vì chiến mà sống |
| 26 | HeFuBOSS.xml | Boss chiến |
| 27 | HeFuLuoLan.xml | Luo Lan tranh bá |
| - | VIPLiBao.xml | VIP lễ bao |
| - | WangChengJiangLi.xml | Vương thành thưởng |
| - | ChongZhiSong.xml | Nạp tặng |
| - | HeFuType.xml | Type mapping |

### 2.3 Các files khác có thể còn thiếu

- `RiChangGifts/` folder — daily gifts không thấy trong scan
- `DailyActiveInfor.xml`, `DailyActiveAward.xml` — daily active config
- `GiftCodeNew.xml` (214KB) — gift codes
- `ThemeActivityZhiGouReward.xml`, `ThemeActivityMoYu.xml`, etc.
- `MuVip.xml`, `VipDailyAwards.xml`, `VipTab.xml`

---

## 3. XmlEventService.cs — Core Engine

**Size**: 15KB / 12,257 chars

### 3.1 Public API (11 methods)

| Method | Chức năng |
|--------|-----------|
| `FullPath(def)` | Resolve đường dẫn tuyệt đối |
| `FileExists(def)` | Kiểm tra file tồn tại |
| `CountRecords(def)` | Đếm số record trong file |
| `InvalidateCache(def)` | Xóa cache cho file cụ thể |
| `LoadRecords(def)` | Đọc + parse XML → List\<EventRecord\> |
| `UpdateRecord(def, id, attrs)` | Cập nhật record theo ID |
| `AddRecord(def, attrs)` | Thêm record mới |
| `DeleteRecord(def, id)` | Xóa record |
| `Toggle(def, id, enable)` | Bật/tắt theo strategy |
| `TriggerBackup(def)` | Tạo backup thủ công |
| `ListBackups()` | Liệt kê 50 backup gần nhất |

### 3.2 Design patterns

```csharp
// Config path from appsettings
"EventManager:GameResConfigPath" → _configRoot

// Backup path
Config/_EventManager/backups/{key}_{yyyyMMdd_HHmmss_fff}.bak

// Disable state
Config/_EventManager/state/{key}.disabled.xml  (Park OFF)
Config/_EventManager/state/{key}.window.json   (DateWindow OFF → save range)

// OffSentinel (tắt bằng cách set ToDate = past date)
const string OffSentinel = "2000-01-01 00:00:00";

// Date formats accepted
"yyyy-MM-dd HH:mm:ss", "yyyy-MM-dd HH:mm", "yyyy-MM-dd",
"yyyy/MM/dd HH:mm:ss", "yyyy/MM/dd"

// Write preserves UTF-8 BOM (new UTF8Encoding(true))
// Cache cleared on every write
```

### 3.3 Toggle implementation

```
Flag:   XElement.SetAttributeValue(FlagAttr, enable ? FlagOn : FlagOff)
        → Write XML

DateWindow ON:  Restore from {key}.window.json
DateWindow OFF: Save {FromAttr,ToAttr} to {key}.window.json
                Set ToDate = OffSentinel ("2000-01-01")
                → Write XML

Park ON:  Read {key}.disabled.xml → merge back into main XML
          Delete disabled file
Park OFF: Find record by ID → extract → write to {key}.disabled.xml
          Remove from main XML
```

---

## 4. Việc cần làm — EventRegistry.cs expansion

### 4.1 Snippet code đã sinh (file riêng)

File: `EventRegistry.JieRiHeFu.snippet.cs`  
Chứa 22 EventFileDef entries cho JieRiGifts (9 files chính) + HeFuGifts (10 files)

**Cách thêm vào EventRegistry.cs**:
```csharp
// Trong EventRegistry.cs, public static readonly List<EventFileDef> Files = new() { ... }
// Thêm các entries từ snippet vào sau phần HuiGuilHuoDong entry
```

### 4.2 Template EventFileDef cho DateWindow activity

```csharp
new() {
    Key="jieri-{name}",
    RelativePath="JieRiGifts/{filename}.xml",
    DisplayName="Lễ hội - {VietName} (ID {n})",
    Category="Quà lễ hội",
    ItemElement="Award",           // child element tag
    IdAttr="TimeOl",               // hoặc "Condition", "Rank", "ID" tùy file
    ListColumns=new[]{"TimeOl","GoodsOne","GoodsTwo","GoodsThr"},
    Description="Thưởng lễ hội theo ngày/tier",
    Toggle=ToggleStrategy.DateWindow,
    FromAttr="FromDate",
    ToAttr="ToDate"
},
```

### 4.3 Priority thêm trước

1. **JieRiType.xml** — type mapping (phải thêm trước để có reference)
2. **9 JieRi cơ bản** (ID 9-17) — dùng trong JieRiType.xml
3. **8 HeFu files** — dùng trong HeFuType.xml
4. **DailyActiveInfor.xml** — daily active config quan trọng
5. **GiftCodeNew.xml** — 214KB gift codes

---

## 5. Tóm tắt tiến độ

| Hạng mục | Trạng thái | Ghi chú |
|----------|-----------|---------|
| EventRegistry: core system files | ✅ 35 files | SystemParams, EventCalendar, Theme, SevenDay... |
| EventRegistry: JieRiGifts | ❌ 0/45 | Cần thêm snippet |
| EventRegistry: HeFuGifts | ❌ 0/12 | Cần thêm snippet |
| EventRegistry: DailyActive | ❌ 0/2 | DailyActiveInfor, DailyActiveAward |
| EventRegistry: GiftCode | ❌ 0/1 | GiftCodeNew.xml 214KB |
| XmlEventService: CRUD engine | ✅ | Complete |
| XmlEventService: Toggle (3 strategies) | ✅ | Flag/DateWindow/Park |
| XmlEventService: Backup | ✅ | Auto + manual |
| REST API | ❌ | Phase 3 |
