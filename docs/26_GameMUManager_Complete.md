# 26_GameMUManager_Complete_Analysis.md
> Phân tích đầy đủ GameMU.Manager — GoodsAudit, Calendar, Schemas
> Confidence: HIGH (đọc trực tiếp source)
> Date: 2026-06-25

---

## 1. GoodsAuditService.cs — FK Validation Engine

**Path**: `GameMU.Manager/Services/GoodsAuditService.cs`  
**Chức năng**: Quét toàn bộ Config/*.xml, tìm tất cả attribute tham chiếu GoodsID, đối soát với Goods.xml

### 1.1 Thuật toán

```
1. Load Goods.xml → Set<goodsId>
2. Scan toàn bộ Config/**/*.xml (bỏ qua _EventManager/)
3. Với mỗi attribute trong GoodsFields:
   - Parse GoodsData format: split '|' → lấy field[0] = GoodsID
   - So sánh với Goods.xml IDs
4. Kết quả phân loại:
   - Clean    (Missing=0) → tất cả GoodsID tồn tại ✅
   - Broken   (MatchRate >= 90% nhưng còn thiếu) → vài mã sai 🟡
   - Suspect  (MatchRate < 90%) → có thể không phải FK Goods 🔴
```

### 1.2 GoodsFields — 50+ field names được scan

```csharp
// Goods reference fields:
"GoodsID","GoodsId","GoodsIDs","GoodsID1","GoodsIDOne","ItemID","ItemId"
// Multi-slot fields:
"GoodsOne","GoodsTwo","GoodsThr","GoodsFour","Goods","GoodsList"
// Special:
"GoodsCost","CostGoods","NeedGoods","LossItem","XiaoHuiGoods"
// Award/Reward:
"Award","Awards","AwardGoods","DayAward","EventAward","LiBaoAward","AchievementReward"
"Items","PetGoods","HorseGoods","OrnamentGoods","ProtectGoods","AddedGoods","ShowGoods"
// Win/Lose/Fight:
"WinGoods","LoseGoods","WinRewardItem","LoseRewardItem","FightAward","KillAward"
"Reward","RewardGoods","LeaderReward","MasterReward","FirstGoodsID"
"WinAward","LoseAward","ShowAward","SeasonReward"
// Others:
"GLGoods","VIPGoodsIDs","KillExtraAward","FirstWinRankReward"
```

### 1.3 GoodsData extraction

```csharp
// Input: "63262,50,1,0,0,0,0|9990001,200,1,0,0,0,0"
// Split by '|' → each chunk
// Take field[0] after split(',') = GoodsID
// Ignore 0 và -1 (đây là sentinel/null)
```

### 1.4 GoodsAuditResult categories

| Category | Điều kiện | Ý nghĩa |
|----------|-----------|---------|
| `Clean` | Missing == 0 | Tất cả GoodsID hợp lệ |
| `Broken` | MatchRate >= 90% AND Missing > 0 | Vài mã GoodsID không tồn tại |
| `Suspect` | MatchRate < 90% | Có thể attribute này không dùng Goods.xml IDs |

### 1.5 IsManaged flag

Mỗi `GoodsRefRow` có `IsManaged` = true nếu file đang được EventRegistry quản lý.  
→ Dễ link từ audit view đến File editor.

---

## 2. Calendar.cshtml.cs — Timeline View

**Chức năng**: Hiển thị timeline sự kiện theo thời gian (% width bar)

### 2.1 Logic

```
1. Lấy tất cả EventFileDef có Toggle = DateWindow
2. LoadRecords cho mỗi file
3. Parse FromDate/ToDate → DateTime?
4. Tính Status:
   - "unconfigured" → không có date
   - "off"          → ToDate.Year <= 2001 (OffSentinel)
   - "upcoming"     → FromDate > Now
   - "ended"        → ToDate < Now
   - "running"      → đang chạy

5. Phát hiện overlap (trùng giờ trong cùng 1 file) → OverlapWarning
6. Render timeline bar theo %:
   LeftPct  = (From - RangeStart) / TotalSeconds * 100
   WidthPct = (To - From) / TotalSeconds * 100
   NowPct   = (Now - RangeStart) / TotalSeconds * 100
```

### 2.2 Tác động của việc thêm JieRi/HeFu vào registry

**Hiện tại**: Calendar chỉ hiển thị 2 DateWindow files (SpecialActivityTime + HuiGuiHuoDong)  
**Sau khi thêm snippet**: Calendar sẽ hiển thị **57 thêm** DateWindow entries từ JieRi/HeFu  
→ Rất hữu ích để thấy lễ hội nào đang/sắp chạy và overlap nhau

### 2.3 OffSentinel detection

```csharp
// Phát hiện sự kiện đã tắt bằng DateWindow:
if (to != null && to.Value.Year <= 2001) item.Status = "off";
// Khớp với XmlEventService OffSentinel = "2000-01-01 00:00:00"
```

---

## 3. GiftCodeNew.xml Schema

Từ decode base64:
```xml
<Config>
  <GiftCode
    TypeID="S26X"           <!-- Mã code (4-5 ký tự) → ID chính -->
    TypeName="礼包奖励"     <!-- Tên hiển thị -->
    Note="统一礼包码-测试"  <!-- Ghi chú nội bộ -->
    Description="您的礼包码{0}使用成功..." <!-- Template thông báo, {0}=code -->
    Platform=""             <!-- Platform filter (rỗng = tất cả) -->
    Channel=""              <!-- Channel filter -->
    TimeBegin="2014-01-01 00:00:00"  <!-- Bắt đầu -->
    TimeEnd="2099-12-31 23:59:59"    <!-- Kết thúc (2099 = không hết hạn) -->
    Zone=""                 <!-- Zone filter -->
    UserType="1"            <!-- 1=player, 0=admin? -->
    UseCount="1"            <!-- Số lần có thể dùng per account -->
    GoodsOne="50161,1,1,0,0,0,0|2001,10,1,0,0,0,0|..."
    GoodsTwo=""
  />
</Config>
```

**EventRegistry entry** (đã thêm vào snippet):
```csharp
Key="giftcode", ItemElement="GiftCode", IdAttr="TypeID", NameAttr="TypeName"
Toggle=ToggleStrategy.Park
```

---

## 4. DailyActiveAward.xml Schema

```xml
<Config>
  <DailyActiveAward>
    <Award
      ID="1"               <!-- Thứ tự thưởng -->
      NeedhuoYue="20"      <!-- Điểm hoạt động cần tích lũy -->
      GoodsID="9990003,100,1,0,0,0,0|9990001,10,1,0,0,0,0"
    />
    <!-- 5 tiers: 20/60/100/110/130 điểm -->
  </DailyActiveAward>
</Config>
```

**EventRegistry entry** (đã thêm vào snippet):
```csharp
Key="daily-active-award", ItemElement="Award", IdAttr="ID"
Toggle=ToggleStrategy.None (chỉ xem/sửa, không bật/tắt)
```

---

## 5. Tổng hợp toàn bộ sessions

### 5.1 Files đã đọc trong session này

| File | Size | Key Findings |
|------|------|-------------|
| ActivityTypes.cs | 1.7KB | 100+ ActivityType enum |
| DailyActiveTypes.cs | 1KB | 19 DailyActive IDs |
| DailyActiveManager.cs | 31KB | DB fields, timer, methods |
| Activity.cs | 18KB | Base class, hardcode dates |
| RechargeRepayActiveMgr.cs | 48KB | TCP handlers, recharge flow |
| MallGoodsMgr.cs | 3KB | Mall+QiZhenGe loading |
| GlobalServiceManager.cs | 18KB | 30+ modules, TimerProc |
| ServerEvents.cs | 4KB | Log writer |
| EventTypes.cs | 1.4KB | 66 event types |
| ActivityManagerNew.cs | 8KB | New pattern + error codes |
| EverydayActivity.cs | 48KB | EventType 36, type 14, cmds 770/1507 |
| ReloadXmlManager.cs | 36KB | 60 hot-reload files, 86 Reset methods |
| EventRegistry.cs | 29KB | 35 registered files |
| XmlEventService.cs | 15KB | 11 CRUD methods, backup, toggle |
| GoodsAuditService.cs | 7KB | FK validation, 50+ GoodsFields |
| Calendar.cshtml.cs | 7KB | Timeline, OffSentinel detect, overlap |

### 5.2 XML files đã đọc schema

| File | Key Schema |
|------|-----------|
| DailyActiveInfor.xml | Tab: DailyActiveID, Name, conditions, Award |
| DailyActiveAward.xml | Award: ID, NeedhuoYue, GoodsID |
| JieRiType.xml | Type: ID, Name, PeiZhi |
| HeFuType.xml | Type: ID, Name, PeiZhi |
| JieRiDengLu.xml | Activities header + Award TimeOl+GoodsOne |
| JieRiChongZhiQiangGou.xml | Award: ID, Day, ZhiGouID, ChongZhiID, SinglePurchase |
| JieRiMeiRiLeiJi.xml | Award: ID, Day, MinYuanBao |
| GiftCodeNew.xml | GiftCode: TypeID, TimeBegin/End, UseCount, GoodsOne |
| EventCalendar.xml | EventCalendar: ID, EventName, Weekday, Level, LinkID |
| EveryDayActivity.xml | 52KB - not decoded (too large) |
| SystemParams.xml | 147KB - sample seen |

---

## 6. Actionable items cho GameMU.Manager

### Ngay bây giờ (no server restart)

1. **Thêm EventRegistry.COMPLETE.snippet.cs** vào `EventRegistry.cs`:
   ```bash
   # Mở Services/EventRegistry.cs
   # Tìm entry "huigui-huodong" 
   # Paste toàn bộ snippet sau đó
   ```
   → **+59 files** → Calendar sẽ hiển thị đầy đủ JieRi/HeFu timeline

2. **Test Calendar page** với entries mới để check overlap

3. **Chạy GoodsAudit** để phát hiện GoodsID bị thiếu trong JieRi/HeFu XMLs

### Phase 3 (cần implement)

1. **REST API** `/api/events/{key}`:
   ```csharp
   // In Program.cs hoặc một ApiController mới
   app.MapGet("/api/events/{key}", (string key, XmlEventService svc) => {
       var def = EventRegistry.Files.FirstOrDefault(f => f.Key == key);
       if (def == null) return Results.NotFound();
       var records = svc.LoadRecords(def);
       return Results.Ok(new { def.Key, def.DisplayName, records });
   });

   app.MapPut("/api/events/{key}/{id}", (string key, string id, 
       Dictionary<string,string> attrs, XmlEventService svc) => {
       var def = EventRegistry.Files.FirstOrDefault(f => f.Key == key);
       if (def == null) return Results.NotFound();
       svc.UpdateRecord(def, id, attrs);
       return Results.Ok();
   });
   ```

2. **GoodsData visual editor** (trong Edit.cshtml):
   ```javascript
   // Parse "63262,50,1,0,0,0,0|9990001,200,1,0,0,0,0"
   function parseGoodsData(raw) {
     return raw.split('|').filter(s => s).map(chunk => {
       const [GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo] 
         = chunk.split(',');
       return { GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo };
     });
   }
   function serializeGoodsData(items) {
     return items.map(i => 
       [i.GoodsID, i.GCount, i.Binding, i.Forge_level, i.AppendPropLev, i.Lucky, i.ExcellenceInfo]
       .join(',')
     ).join('|');
   }
   ```

3. **Reload trigger** sau khi save (gọi GameServer reload):
   ```csharp
   // Thêm vào XmlEventService sau WriteXml():
   await ReloadGameServer(def.RelativePath); // HTTP call to GameServer
   ```

---

## 7. Final Status

### Prompt tasks (Game_Event_Reverse_Engineering_FULL)

| Hạng mục | % | Ghi chú |
|----------|---|---------|
| 1. Phân tích hệ thống sự kiện | 100% ✅ | ActivityTypes 100+ enum, 7 hệ thống |
| 2. Phân tích luồng hoạt động | 100% ✅ | TCP→Handler→Manager→XML→DB |
| 3. Mapping XML | 95% ✅ | 35 đã map, 57 sắp thêm |
| 4. Reverse Schema XML | 100% ✅ | Tất cả pattern documented |
| 5. Phân tích thời gian | 100% ✅ | DayOfYear, OffSentinel, 3 reset types |
| 6. Phân tích Item | 100% ✅ | GoodsData 7-field format confirmed |
| 7. Phân tích Shop | 100% ✅ | Mall/QiZhenGe/ZhanGong |
| 8. Phân tích nạp tiền | 100% ✅ | Full flow documented |
| 9. Sinh tài liệu nghiệp vụ | 100% ✅ | 26 doc files |
| 10. Thiết kế Data Model CRUD | 100% ✅ | 6 models + ERD |
| 11. Admin CRUD (GameMU.Manager) | 85% 🔄 | Core done, need REST API + snippet |
| 12. Sinh sơ đồ tổng thể | 100% ✅ | Call Graph + ERD + Sequence + State |
| 13. Báo cáo cuối | 95% ✅ | File này + 25 docs khác |

### Docs tổng kết (session này)

| File | Mô tả |
|------|-------|
| 19_ActivitySystem_Analysis.md | ActivityTypes enum + hệ thống |
| 20_XMLSchema_Complete.md | Schema XML thực tế |
| 21_CallGraph_DataModel_CRUD.md | Call Graph + Data Model |
| 22_MigrationPlan_Complete.md | Kế hoạch migrate |
| 23_EverydayActivity_GameMUManager.md | EverydayActivity + Manager state |
| 24_HotReload_Complete.md | Hot-reload + 86 Reset methods |
| 25_EventRegistry_Gap.md | Gap analysis 57 files |
| 26_GameMUManager_Complete_Analysis.md | File này |
| **EventRegistry.JieRiHeFu.snippet.cs** | Snippet partial (22 entries) |
| **EventRegistry.COMPLETE.snippet.cs** | Snippet đầy đủ (59 entries) ← **DÙNG CÁI NÀY** |
