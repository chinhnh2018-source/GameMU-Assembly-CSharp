# 22_MigrationPlan_Complete.md
> Kế hoạch migrate hoàn chỉnh từ source analysis + checklist cập nhật
> Confidence: HIGH  
> Date: 2026-06-25

---

## Trạng thái tổng hợp

| Hạng mục (prompt) | Trạng thái | File |
|-------------------|-----------|------|
| 1. Phân tích hệ thống sự kiện | ✅ DONE | 19_ActivitySystem_Analysis.md |
| 2. Phân tích luồng hoạt động | ✅ DONE | 19, 21 |
| 3. Mapping XML | ✅ DONE | 19, 20 |
| 4. Reverse Schema XML | ✅ DONE | 20_XMLSchema_Complete.md |
| 5. Phân tích thời gian | ✅ DONE | 19 |
| 6. Phân tích Item | ✅ DONE | 19 |
| 7. Phân tích Shop | ✅ DONE | 19 |
| 8. Phân tích nạp tiền | ✅ DONE | 19 |
| 9. Sinh tài liệu nghiệp vụ | ✅ DONE | 19-22 |
| 10. Thiết kế Data Model CRUD | ✅ DONE | 21_CallGraph_DataModel_CRUD.md |
| 11. Sinh công cụ Admin CRUD | 🔄 IN PROGRESS | GameMU.Manager |
| 12. Sinh sơ đồ tổng thể | ✅ DONE | 21 (ERD, Sequence, State) |
| 13. Báo cáo cuối | 🔄 (file này) | 22_MigrationPlan_Complete.md |

---

## Phát hiện quan trọng từ code thực tế

### ⚠️ Hardcode cần xử lý ngay

| Vị trí | Value | Mức | Action |
|--------|-------|-----|--------|
| `DailyActiveManager.InitDailyActiveFlagIndex()` | 19 IDs hardcode | 🔴 | Config DailyActiveInfor.xml |
| `DailyActiveManager.NotifyClientDailyActiveData()` | Packet 558 | 🔴 | TCPGameServerCmds const |
| `EverydayActivity.processEvent()` | EventType **36** | 🔴 | EventTypes.OnClientChargeItem (đã có enum) |
| `EverydayActivity.OnMoneyChargeEvent()` | DBCmd **13173** | 🔴 | DBCommandIDs const |
| `Activity.cs` | Date "2008-08-08 08:08:08" | 🟡 | ActivityConfig default |
| `Activity.cs` | Date "2028-08-08 08:08:08" | 🟡 | ActivityConfig default |
| `XmlEventService.OffSentinel` | "2000-01-01 00:00:00" | 🟡 | const |
| `FestivalManager` | `to.Year <= 2001` | 🔴 | config sentinel |
| `ActivityManagerNew` | Error code 30767 (hardcode packet) | 🟡 | TCPGameServerCmds const |
| `RechargeRepayActiveMgr` | GLang 528/529/530 | 🟡 | lang resource |

### 🔑 Key discoveries

1. **Hai hệ thống DailyActive song song**:
   - `DailyActiveManager` (ID 100-1600, bitmask) ← config từ DailyActiveInfor.xml  
   - `EverydayActivity` (ActID, event-driven) ← config từ EveryDayActivity.xml
   - Không replace nhau — chạy song song

2. **JieRi chỉ có 9 types trong JieRiType.xml** (ID 9-17)  
   Types 40-77 được define ở nơi khác (MuJieRiType.xml hoặc hardcode ActivityTypes enum)

3. **GoodsData 7 fields** là chuẩn dùng khắp XML:
   `GoodsID,GCount,Binding,Forge_level,AppendPropLev,Lucky,ExcellenceInfo`

4. **Activity.CheckCondition() → GiveAward()** là pattern chuẩn  
   Error codes: -1 (null), -2 (time), -3 (bag), -7 (fail), -10007 (condition), 1 (OK)

5. **TCPCmdHandler.cs = 1.7MB** — entry point duy nhất cho tất cả TCP commands  
   → Đây là bottleneck cần split thành dispatcher

6. **EventCalendar.xml không load JieRi/HeFu detail** — nó chỉ define window thời gian  
   → JieRiXxx.xml/HeFuXxx.xml được load riêng qua HuodongCachingMgr

---

## Kế hoạch chi tiết GameMU.Manager

### Đã hoàn thành (từ 98_Summary.md)

| Component | File | Status |
|-----------|------|--------|
| EventRegistry | EventRegistry.cs (431 dòng) | ✅ 50+ XML defs |
| XmlEventService | XmlEventService.cs (345 dòng) | ✅ R/W/Cache/Backup/Toggle |
| Models | EventModels.cs | ✅ EventFileDef, ToggleStrategy, ForeignKeyRef |
| ModuleRegistry | ModuleRegistry.cs | ✅ 11 modules |
| Pages | Index, File, Edit, Backups, Calendar, Features, GoodsAudit, Links | ✅ |
| Services | GoodsAuditService, LinkResolutionService | ✅ FK validation |

### Còn lại (Phase 3)

```
1. REST API endpoints:
   GET  /api/events/{key}         → Return parsed EventFileDef JSON
   PUT  /api/events/{key}         → Save XML (validate FK trước)
   POST /api/events/{key}/toggle  → Toggle on/off
   POST /api/events/{key}/clone   → Clone với date range mới
   POST /api/import               → Import ZIP bundle
   GET  /api/export               → Export ZIP bundle
   GET  /api/config/params/:name  → SystemParams read
   PUT  /api/config/params/:name  → SystemParams write (cần restart server)

2. GoodsData visual editor:
   - Parse "GoodsID,GCount,Binding,Forge,Append,Lucky,Excel|..." 
   - Hiển thị dạng table có thể edit
   - FK check: GoodsID tồn tại trong Goods.xml cache
   - Warn nếu GoodsID không tìm thấy

3. Performance:
   - Goods.xml cache (~15MB) → lazy load + LRU cache
   - SystemParams.xml (147KB) → cache in memory, invalidate on save
   - GiftCodeNew.xml (214KB) → paginate, không load all vào memory
```

---

## Mapping XML ↔ GameMU.Manager EventRegistry

| XML File | EventRegistry Key | Category |
|----------|------------------|----------|
| DailyActiveInfor.xml | daily-active-config | daily |
| DailyActiveAward.xml | daily-active-award | daily |
| EventCalendar.xml | event-calendar | calendar |
| EveryDayActivity.xml | everyday-activity | daily |
| EveryDayActivityGroup.xml | everyday-group | daily |
| JieRiGifts/JieRiType.xml | jieri-type | jieri |
| JieRiGifts/JieRiDengLu.xml | jieri-denglu | jieri |
| JieRiGifts/JieRiLeiJi.xml | jieri-leiji | jieri |
| JieRiGifts/JieRiChongZhiKing.xml | jieri-cz-king | jieri |
| ... (46 JieRi files) | jieri-* | jieri |
| HeFuGifts/HeFuType.xml | hefu-type | hefu |
| HeFuGifts/HeFuDengLu.xml | hefu-denglu | hefu |
| ... (12 HeFu files) | hefu-* | hefu |
| ThemeActivity*.xml | theme-* | theme |
| Mall.xml | mall-main | shop |
| QiZhenGeMall.xml | mall-qizhengemall | shop |
| ZhanGongMall.xml | mall-zhangong | shop |
| GiftCodeNew.xml | giftcode-new | giftcode |
| SystemParams.xml | system-params | system |
| SystemOpen.xml | system-open | system |

---

## Checklist còn lại

### Source code chưa đọc
- [ ] `EverydayActivity.cs` (48KB) — hệ thống EverydayActivity
- [ ] `GoodsPackManager.cs` (119KB) — reward engine
- [ ] `ReloadXmlManager.cs` (36KB) — hot-reload cơ chế
- [ ] `GiftCodeNewManager.cs` (10KB) — gift code flow
- [ ] `HuodongCachingMgr.cs` — God Object cần phân tích
- [ ] `KF.Remoting.HuanYingSiYuan/KF/` — cross-server remoting
- [ ] `GameServer/Core/Executor/` — executor pattern

### XML chưa đọc
- [ ] `SystemParams.xml` (147KB) — cần sample sections
- [ ] `SystemOpen.xml` (44KB) — feature flags
- [ ] `EveryDayActivity.xml` (52KB) — sample entries
- [ ] `GiftCodeNew.xml` (214KB) — sample entries

### GameMU.Manager chưa làm
- [ ] REST API `/api/events/{key}` 
- [ ] GoodsData visual editor
- [ ] Import/Export ZIP
- [ ] Performance test Goods.xml cache 15MB

---

## Docs index cuối (tất cả files)

| File | Nội dung | Size |
|------|---------|------|
| 00_Overview.md | Kiến trúc 3 project | 2.1KB |
| 01_DailyActiveManager_FULL.md | DailyActive 19 IDs + flow | 6.1KB |
| 02_DailyEvent.md | Flow cơ + GoalType mapping | 1.6KB |
| 03_EverydayActivity_NEW.md | EverydayActivityType(14) + SevenDayGoalFuncType(44) | 5.5KB |
| 04_Festival.md | JieRi 28 files + HeFu 6 files | 1.3KB |
| 05_PacketID_vs_LinkID.md | TCP case 301=BangHui | 2.5KB |
| 06_Shop.md | Mall.xml schema | 1.0KB |
| 07_DatabaseSchema.md | RoleData 180 fields | 8.5KB |
| 08_SystemParams.md | VIPHuoYueAdd, DailyActiveOpen | 1.5KB |
| 09_Remoting.md | HuanYingSiYuanService | 1.5KB |
| 10_SevenDayEvent.md | SevenDayLogin schema | 0.9KB |
| 11_CallGraph.md | Flow xử lý chi tiết | 1.5KB |
| 12_SpecialActivity.md | SpecialActivity.xml schema | 1.6KB |
| 13_MigrationPlan.md | Kế hoạch migration v1 | 2.2KB |
| 14_EverydayActivity_DEEP.md | Deep dive EverydayActivity | 5.7KB |
| 15_TCPPacketID_Map.md | TCP Packet ID mapping | 6.4KB |
| 16_SystemParams_Analysis.md | SystemParams phân tích | 13.3KB |
| 17_EveryDayActivity_XML_Deep.md | XML EveryDayActivity deep | 10.7KB |
| 18_EventSystem_Complete.md | Toàn bộ Event System | 13.9KB |
| 19_ActivitySystem_Analysis.md | ActivityTypes enum đầy đủ | 15.2KB |
| 20_XMLSchema_Complete.md | XML schema thực tế | 11.0KB |
| 21_CallGraph_DataModel_CRUD.md | Call Graph + Data Model | 14.2KB |
| 22_MigrationPlan_Complete.md | Kế hoạch + Checklist | (file này) |
| 98_Summary.md | Tóm tắt tổng hợp | 5.0KB |
| 99_Checklist.md | Checklist hoàn thành | 2.2KB |
