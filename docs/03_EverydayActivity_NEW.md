# EverydayActivity (ActivityNew) — Reverse Engineering

> Nguồn: `GameServer/Logic/ActivityNew/EverydayActivity.cs` (1398 dòng) + `EverydayActivityType.cs`
> Độ tin cậy: **HIGH**

---

## 1. EverydayActivityType Enum (Type trong EveryDayActivity.xml)

```csharp
enum EverydayActivityType {
    EAT_QiangGou      = 1,  // Mua nhanh (ZhiGou)
    EAT_InputExchange = 2,  // Nạp đổi (ChongZhiDuiHuan)
    EAT_Consume       = 3,  // Tiêu tiền
    EAT_DirectAward   = 4,  // Phần thưởng trực tiếp (điều kiện level/vip)
    EAT_Level         = 5,  // Cấp độ nhân vật
    EAT_Wing          = 6,  // Cấp cánh
    EAT_Vip           = 7,  // Cấp VIP
    EAT_ChengJiu      = 8,  // Cấp thành tựu
    EAT_JunXian       = 9,  // Cấp quân hàm
    EAT_Merlin        = 10, // Cấp Merlin (thần pháp)
    EAT_ShengWu       = 11, // Cấp Thần Vật
    EAT_Ring          = 12, // Cấp nhẫn (hôn lễ)
    EAT_ShouHuShen    = 13, // Cấp Thủ Hộ Thần
    EAT_ZhiGou        = 14  // Mua trực tiếp (ZhiGou khác QiangGou)
}
```

**Ghi chú**: `EAT_QiangGou` (1) và `EAT_ZhiGou` (14) đều liên quan ZhiGou.xml nhưng khác nhau ở logic.

---

## 2. Flow xử lý EverydayActivity

```
ChargeItemBaseEventObject (EventType=36) → processEvent()
    → Tìm ActID có Type==14 và Price.ZhiGouID == chargeItemConfig.ChargeItemID
    → BuildFetchEverydayActAwardCmd() → sendCmd(1507)
```

**Recharge event**:
```
OnMoneyChargeEvent(userid, roleid, addMoney)
    → Đọc EveryDayChongZhiDuiHuan từ SystemParams
    → Format: "20000:1" = 20000 YuanBao → 1 lần đổi
    → ExecuteDBCmd(13173, ...) → DB ghi record
```

---

## 3. GoalType cũ vs Type mới

| Hệ thống cũ (DailyActiveManager) | Hệ thống mới (EverydayActivity) |
|----------------------------------|----------------------------------|
| ID-based (100, 200, 300...) | ActID từ XML |
| GoalType không có | Type = EverydayActivityType enum |
| Check field name ("Login", "Online"...) | GoalData struct |
| Reset theo DayOfYear | FromDate/ToDate per GroupID |

**Hai hệ thống song song**: DailyActiveManager (cũ, bitmask) và EverydayActivity (mới, dict-based).

---

## 4. Cấu trúc data DB (EverydayActivity mới)

```
EverydayActInfoDB:
  - ActID: int
  - GroupID: int
  - FinishCount: int   // Số lần hoàn thành
  - AwardCount: int    // Số lần nhận thưởng
  - LastTime: DateTime

EverydayActGroupInfoDB:
  - GroupID: int
  - FromDate: DateTime
  - ToDate: DateTime
```

---

## 5. SevenDay Activity

### ESevenDayActType
```csharp
enum ESevenDayActType {
    Login  = 1,  // SevenDayLogin.xml
    Charge = 2,  // SevenDayChongZhi.xml
    Goal   = 3,  // SevenDayGoal.xml
    Buy    = 4   // SevenDayQiangGou.xml
}
```

### Tất cả GoalFuncType (ESevenDayGoalFuncType) — từ SevenDayGoal.xml

| Value | Tên | Mô tả |
|-------|-----|-------|
| 1 | RoleLevelUp | Lên cấp nhân vật |
| 2 | SkillLevelUp | Lên cấp kỹ năng |
| 3 | MoJingCntInBag | Số ma tinh trong túi |
| 4 | RecoverMoJing | Khôi phục ma tinh |
| 5 | ExchangeJinHuaJingShiByMoJing | Đổi tinh thạch bằng ma tinh |
| 6 | JoinJingJiChangTimes | Tham gia đấu trường |
| 7 | WinJingJiChangTimes | Thắng đấu trường |
| 8 | JingJiChangRank | Xếp hạng đấu trường |
| 9 | PeiDaiBlueUp | Đeo trang bị xanh+ |
| 10 | PeiDaiPurpleUp | Đeo trang bị tím+ |
| 12 | RecoverEquipBlueUp | Khôi phục trang bị xanh+ |
| 13 | MallInSaleCount | Số sản phẩm bán trên chợ |
| 14 | GetEquipCountByQiFu | Nhặt trang bị từ kỳ phú |
| 15 | PickUpEquipCount | Nhặt trang bị |
| 16 | EquipChuanChengTimes | Số lần truyền thừa |
| 17 | EnterFuBenTimes | Số lần vào dungeon |
| 18 | KillMonsterInMap | Số quái giết trong map |
| 19 | JoinActivityTimes | Tham gia hoạt động |
| 20 | HeChengTimes | Số lần hợp thành |
| 21 | UseGoodsCount | Số lượng vật phẩm dùng |
| 22 | JinBiZhuanHuanTimes | Số lần đổi vàng |
| 23 | BangZuanZhuanHuanTimes | Số lần đổi bang kim cương |
| 24 | ZuanShiZhuanHuanTimes | Số lần đổi kim cương |
| 25 | ExchangeJinHuaJingShiByQiFuScore | Đổi tinh thạch |
| 26 | CombatChange | Thay đổi chiến lực |
| 27 | PeiDaiForgeEquip | Đeo trang bị cường hóa |
| 28 | ForgeEquipLevel | Cấp cường hóa |
| 29 | ForgeEquipTimes | Số lần cường hóa |
| 30 | CompleteChengJiu | Hoàn thành thành tựu |
| 31 | ChengJiuLevel | Cấp thành tựu |
| 32 | JunXianLevel | Cấp quân hàm |
| 33 | PeiDaiAppendEquip | Đeo trang bị truy gia |
| 34 | AppendEquipLevel | Cấp truy gia |
| 35 | AppendEquipTimes | Số lần truy gia |
| 36 | ActiveXingZuo | Kích hoạt tinh tọa |
| 37 | GetSpriteCountBuleUp | Nhặt linh hồn xanh+ |
| 38 | GetSpriteCountPurpleUp | Nhặt linh hồn tím+ |
| 39 | WingLevel | Cấp cánh |
| 40 | WingSuitStarTimes | Số lần star cánh |
| 41 | CompleteTuJian | Hoàn thành đồ giám |
| 42 | PeiDaiSuitEquipCount | Số trang bị bộ đang đeo |
| 43 | PeiDaiSuitEquipLevel | Cấp bộ trang bị đeo |
| 44 | EquipSuitUpTimes | Số lần thăng cấp bộ |

### SevenDay Config paths
```
Config/SevenDay/SevenDayLogin.xml     → Login ngày 1-7
Config/SevenDay/SevenDayGoal.xml      → Mục tiêu (GoalFuncType)
Config/SevenDay/SevenDayQiangGou.xml  → Mua nhanh
Config/SevenDay/SevenDayChongZhi.xml  → Nạp tiền
```
