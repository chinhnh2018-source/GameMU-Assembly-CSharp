# Hệ thống Hoạt động hằng ngày

## Flow xử lý
```
Player Action (Login/KillMonster/Online/...)
    ↓
Handler (TCPCmdHandler.cs)
    ↓
DailyActiveManager.Process*()
    ↓
CheckDailyActive*() → IsDailyActiveCompleted()
    ↓
OnDailyActiveCompleted() → AddDailyActivePoints()
    ↓
NotifyClientDailyActiveData() → Packet 558
```

## GoalType Mapping (từ EveryDayActivityType.xml)
| GoalType | Tên | CheckLevCondition |
|--------|-----|-----------------|
| 1 | Đổi cấp | MinZhuanshengleve |
| 2 | Login | 100 |
| 5 | Level (Cấp độ) | MinZhuanshengleve + Minleve |
| 6 | Wing (Cánh) | MinZhuanshengleve |
| 8 | Achievement (Thành tựu) | MinZhuanshengleve |
| 9 | JunHuan (Quân hàm) | MinZhuanshengleve |
| 10 | Recharge (Nạp) | MinZhuanshengleve |
| 11 | ShenShi (Thần pháp) | MinZhuanshengleve |
| 12 | Marriage (Hôn lễ) | MinZhuanshengleve |
| 13 | DivineBeast (Thủy thần) | MinZhuanshengleve |

## Hardcode Flag Mapping (DailyActiveManager.cs)
```csharp
m_DailyActiveInfo.Add(100, num);   // Login
m_DailyActiveInfo.Add(200, num);   // Online
m_DailyActiveInfo.Add(300, num);   // BuyMall

// GoalType ranges:
1000-1016: Level rebate
1100-1107: Wing rebate
1200-1209: Achievement rebate
1300-1309: JunHuan rebate
1400-1416: Merlin rebate
1500-1506: DivineBeast rebate
1600-1605: Marriage rebate
1700-1705: DivineBeast rebate
2000-2091: Recharge exchange
3000-3086: Daily limit purchase
```

## Database Keys
- `DailyActiveFlag`: Bit flags (ulong[]), mỗi 2 bit 1 hoạt động
- `DailyActiveInfo1`: Counter values (uint[])
- `DailyActiveAwardFlag`: Phần thưởng đã nhận (int bitmask)