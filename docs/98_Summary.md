# Báo cáo tổng hợp — Phân tích hoàn chỉnh

> Cập nhật: 2026-06-25. Tổng hợp từ 3 session phân tích.

---

## Danh sách tài liệu docs/

| File | Nội dung | Confidence |
|------|---------|-----------|
| [00_Overview.md](00_Overview.md) | Kiến trúc 3 project | HIGH |
| [01_DailyActiveManager_FULL.md](01_DailyActiveManager_FULL.md) | 19 DailyActive ID + 17 DB fields + bitmask | HIGH |
| [02_DailyEvent.md](02_DailyEvent.md) | Flow cũ + GoalType mapping | HIGH |
| [03_EverydayActivity_NEW.md](03_EverydayActivity_NEW.md) | EverydayActivityType (14) + SevenDayGoalFuncType (44) | HIGH |
| [04_Festival.md](04_Festival.md) | JieRi 28 files + HeFu 6 files + Toggle | HIGH |
| [05_PacketID_vs_LinkID.md](05_PacketID_vs_LinkID.md) | Đính chính case 301 = BangHui (không phải Activity) | HIGH |
| [06_Shop.md](06_Shop.md) | Mall.xml schema + Currency mapping | HIGH |
| [07_DatabaseSchema.md](07_DatabaseSchema.md) | RoleData 180 fields + GoodsData + DailyActiveData | HIGH |
| [08_SystemParams.md](08_SystemParams.md) | VIPHuoYueAdd, DailyActiveOpen | HIGH |
| [09_Remoting.md](09_Remoting.md) | HuanYingSiYuanService + EventRegistry | HIGH |
| [10_SevenDayEvent.md](10_SevenDayEvent.md) | SevenDayLogin schema | HIGH |
| [11_CallGraph.md](11_CallGraph.md) | Flow xử lý chi tiết | HIGH |
| [12_SpecialActivity.md](12_SpecialActivity.md) | SpecialActivity.xml schema | HIGH |
| [13_MigrationPlan.md](13_MigrationPlan.md) | Kế hoạch migration | - |

---

## Phát hiện key từ code thực tế

### Hệ thống DailyActive (2 song song)

| Hệ thống | Class | DB | Kích hoạt |
|----------|-------|-----|----------|
| **Cũ** | `DailyActiveManager` | `DailyActiveFlag` (bitmask ulong[]), `DailyActiveInfo1` (uint[]) | ID 100–1600, check theo XML field name |
| **Mới** | `EverydayActivity` | `EverydayActInfoDB`, `EverydayActGroupInfoDB` | ActID từ XML, event-driven (EventObject type 36) |

Cả hai cùng chạy song song. Server cũ dùng cơ chế ID-based, server mới dùng cơ chế dict/event.

### RoleData — God Object (180 ProtoMember fields)
- Serialized qua **ProtoBuf** (không phải JSON/XML)  
- Truyền qua TCP client↔server  
- Key quan trọng nhất: `MoneyData` (IsRequired=true), `OpenData` (IsRequired=true), `ArmorData` (IsRequired=true)

### GoodsData — 7 fields trong XML = 7 fields trong code
```
XML:  GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo
Code: GoodsID, GCount, Binding, Forge_level, AppendPropLev, Lucky, ExcellenceInfo  ✅
```

---

## Trạng thái GameMU.Manager

### Đã có (hoàn chỉnh)
| Thành phần | File | Chức năng |
|-----------|------|----------|
| **Models** | `EventModels.cs` | EventFileDef, ToggleStrategy, EventRecord, ForeignKeyRef |
| **EventRegistry** | `EventRegistry.cs` (431 dòng) | 50+ file XML + FK metadata + JieRi/HeFu auto-gen |
| **XmlEventService** | `XmlEventService.cs` (345 dòng) | R/W + Cache + Backup + Toggle 3 strategy |
| **ModuleRegistry** | `ModuleRegistry.cs` | 11 module sidebar |
| **Pages** | Index, File, Edit, Backups, Calendar, Features, GoodsAudit, Links | Đầy đủ CRUD UI |
| **Services** | GoodsAuditService, LinkResolutionService | FK validation |

### Chưa có (Phase 3)
| Cần làm | Ưu tiên | Ghi chú |
|---------|---------|---------|
| REST API `/api/events/{key}` | Medium | Để tích hợp với tools khác |
| Import/Export ZIP | Medium | Backup toàn bộ Config |
| Diff viewer trước/sau sửa | Low | Nice-to-have |
| Preview XML schema inline | Low | Từ EventFileDef metadata |

---

## Module trong sidebar (11 modules)

| ID | Tên | Loại |
|----|-----|------|
| overview | Tổng quan | Page=/Index |
| calendar | Lịch sự kiện | Page=/Calendar |
| events | Sự kiện & Hoạt động | Category filter |
| festival | Lễ hội & Chủ đề | Category filter |
| system | Bật/Tắt hệ thống | Category filter |
| params | Tham số cân bằng | Category filter |
| refdata | Ánh xạ vật phẩm | Category filter |
| features | Bản đồ tính năng → XML | Page=/Features |
| links | Liên kết XML | Page=/Links |
| goods-audit | Đối soát mã vật phẩm | Page=/GoodsAudit |
| backups | Sao lưu & Khôi phục | Page=/Backups |

---

## Hardcode cần chú ý khi sửa

| Vị trí | Giá trị | Ảnh hưởng |
|--------|---------|----------|
| `DailyActiveManager.InitDailyActiveFlagIndex()` | 100, 200, 300, 400-401, 500, 600, 700, 800, 900, 1000, 1100, 1200, 1300-1302, 1400, 1500, 1600 | Thứ tự KHÔNG được thay đổi (index bitmask) |
| `DailyActiveManager.NotifyClientDailyActiveData()` | Packet 558 | Client hardcode parse |
| `EverydayActivity.processEvent()` | EventType 36 | Charge event type |
| `EverydayActivity.OnMoneyChargeEvent()` | DB Cmd 13173 | Stored proc ID |
| `XmlEventService.OffSentinel` | "2000-01-01 00:00:00" | Toggle off sentinel |
| `FestivalManager` | `to.Year <= 2001` | Check tắt |