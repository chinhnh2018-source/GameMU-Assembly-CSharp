# Dependency Graph - Hệ thống sự kiện

## Daily Active Flow
```
Player Action
    ↓
[Packet Handler - TCPCmdHandler.cs]
    ↓
[DailyActiveManager.Process*]
    ↓
[CheckDailyActiveCompleted]
    ├── IsFlagIsTrue (DayActiveFlag)
    ├── CheckLevCondition (DailyActive.xml)
    └── CheckSingleConditionForDailyActive
    ↓
[OnDailyActiveCompleted]
    ├── AddDailyActivePoints (từ SystemParams: VIPHuoYueAdd)
    └── NotifyClientDailyActiveData → Packet 558
    ↓
[GiveDailyActiveAward]
    ├── Parse GoodsIDs (7 trường)
    ├── AddGoodsDBCommand_Hook
    └── Global.UseMailGivePlayerAward (nếu túi đầy)
```

## Event Calendar Flow
```
[Timer: MonsterTask]
    ↓
[MonsterZone.BirthMonster]
    ↓
[CheckEventTime]
    ├── GetWeekDay/Time
    └── Check Level/VipLevel
    ↓
[PlayZone.ProcessGuideRequest] (hardcode switch LinkID)
    ↓
[Tạo/Clear CopyMap]
```

## Nạp tiền Flow
```
Payment Notify (KfCall.cs)
    ↓
[PlatChargeKingManager]
    ↓
[Check JieRiChongZhi.xml/LeiJiChongZhi.xml]
    ↓
Give GoodsIDs → Mail
```

## XML → DB Mapping
| XML | Key DB | Kiểu |
|-----|--------|------|
| EveryDayActivity.xml | DailyActiveInfo1 | uint[] |
| EveryDayActivity.xml | DailyActiveFlag | ulong[] (bitmask) |
| EveryDayActivity.xml | DailyActiveAwardFlag | int (bitmask) |
| SystemParams.xml | systemParamsList | key-value |
| Goods.xml | goodsCache | Dictionary<int,GoodsData> |