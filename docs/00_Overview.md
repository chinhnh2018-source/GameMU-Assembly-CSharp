# Phân tích hệ thống sự kiện Game MU - Báo cáo

## 1. Kiến trúc tổng quan (High confidence)

### GameServer
- **Entry Point**: `Program.cs:Main()` → `InitServer()` → `GlobalServiceManager.initialize()`
- **TCP Listener**: `SocketListener.cs` - Xử lý client connections
- **Packet Router**: `TCPCmdHandler.cs` - Routing lệnh tới handler tương ứng

### KF.Remoting.HuanYingSiYuan
- **Remoting Service**: Liên server communication
- Services: `KuaFuWorldService`, `YongZheZhanChangService`, `ZhengBaService`...

### GameMU.Manager
- **Web Admin**: ASP.NET Core CRUD XML config
- `EventRegistry.cs`: 50+ file XML event definitions
- `XmlEventService.cs`: Cache, read/write XML service

## 2. Các loại sự kiện

| Loại | File XML | Manager | Tracking Data |
|------|--------|---------|---------------|
| Hoạt động hằng ngày | `EveryDayActivity/*.xml` | `DailyActiveManager` | `DailyActiveInfo1`, `DailyActiveFlag`, `DailyActiveAwardFlag` |
| Sự kiện 7 ngày | `SevenDay/*.xml` | - | - |
| Lịch sự kiện | `EventCalendar.xml` | - | Weekday, Time slots |
| Nạp tiền | `Gifts/DayChongZhi.xml` | - | MinYuanBao |
| Shop | `Mall.xml` | - | TabID, Price |

## 3. Daily Reset Mechanism

**Reset dựa vào**: `TimeUtil.NowDateTime().DayOfYear` (line 792, 27692...)

**Reset data**:
- `DailyActiveFlag` - bit flags (XOR 2-bit mỗi hoạt động)
- `DailyActiveInfo1` - counter values
- `DailyActiveAwardFlag` - phần thưởng đã nhận

## 4. Schema XML chính

**GoodsIDs (7 trường)**:
```
GoodsID,GCount,Binding,Forge_level,AppendPropLev,Lucky,ExcellenceInfo
```

**EveryDayActivity**:
```xml
<EveryDayActivity ActivityID="1000" Name="等级返利" GoalType="5" GoalNum="4,1" GoodsOne="..." />
```

**EventCalendar**:
```xml
<EventCalendar ID="1" Weekday="1" Level="1,50" Time="00:00-00:02|..." LinkID="301" EventAward="8002,8014" />
```

## 5. Hardcode phát hiện

- DailyActive flag: 100→300→400→500→600→700→800→900→1000→1100→1200→1300→1400→1500→1600
- Sentinel: `"2000-01-01 00:00:00"` (dùng cho toggle off)
- Packet ID: `558` cho DailyActiveData
- VersionSystemOpen ID: `>=100000` để mapping system