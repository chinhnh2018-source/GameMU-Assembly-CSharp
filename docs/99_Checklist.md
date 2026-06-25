# Checklist Hoàn thành

## Phân tích (Completed)
| Task | Trạng thái | Output |
|------|-----------|--------|
| DailyActiveManager.cs flowchart | ✅ | docs/01_DailyActiveManager_FULL.md |
| DailyActiveTypes.cs — 19 ID đầy đủ | ✅ | docs/01_DailyActiveManager_FULL.md |
| DailyActiveDataField1.cs — 17 fields | ✅ | docs/01_DailyActiveManager_FULL.md |
| EverydayActivityType enum (14 types) | ✅ | docs/03_EverydayActivity_NEW.md |
| SevenDay 4 files + ESevenDayGoalFuncType (44 values) | ✅ | docs/03_EverydayActivity_NEW.md |
| EventCalendar.xml LinkID vs PacketID (đính chính) | ✅ | docs/05_PacketID_vs_LinkID.md |
| TCPCmdHandler case 301/302/303 = BangHui (không phải Activity) | ✅ | docs/05_PacketID_vs_LinkID.md |
| SystemParams.xml params | ✅ | docs/08_SystemParams.md |
| SevenDay/*.xml schema | ✅ | docs/10_SevenDayEvent.md |
| Remoting architecture | ✅ | docs/09_Remoting.md |
| Shop Mall.xml schema | ✅ | docs/06_Shop.md |
| Recharge configs (Festival) | ✅ | docs/04_Festival.md |
| KF.Remoting.HuanYingSiYuan — TcpCall dir | ✅ | docs/09_Remoting.md |
| RoleData 180 fields + GoodsData + DailyActiveData | ✅ | docs/07_DatabaseSchema.md |
| Summary tổng hợp toàn bộ phân tích | ✅ | docs/98_Summary.md |

## Chưa làm (còn mở)
- [ ] LinkID trong EventCalendar → UI window (client Unity, không có trong server code)
- [ ] REST API `/api/*` (Phase 3) trong GameMU.Manager
- [ ] Performance test Goods.xml cache 15MB

## Phát hiện key (tất cả sessions)
- **Hai hệ thống song song**: `DailyActiveManager` (ID 100–1600, bitmask) + `EverydayActivity` (ActID, event-driven)
- **SevenDayGoalFuncType** 44 loại mục tiêu | **EverydayActivityType** 14 loại
- **TCPCmdHandler case 301/302/303** = BangHui guild, KHÔNG phải EventCalendar
- **RoleData** = ProtoBuf 180 fields, 3 IsRequired: MoneyData, OpenData, ArmorData
- **GoodsData XML 7-field format** = GoodsID,GCount,Binding,Forge_level,AppendPropLev,Lucky,ExcellenceInfo
- **Sentinel tắt**: DateWindow dùng `2000-01-01`, XmlEventService.OffSentinel
- **GameMU.Manager** đã hoàn chỉnh: 50+ XML file, 11 module, CRUD + backup + FK + audit