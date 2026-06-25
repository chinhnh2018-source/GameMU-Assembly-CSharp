# EverydayActivity — GoalType GetCurrentGoalNum (Phân tích sâu)

> Nguồn: `GameServer/Logic/ActivityNew/EverydayActivity.cs` lines 982-1054
> Độ tin cậy: **HIGH** — đọc trực tiếp từ code

---

## 1. GetCurrentGoalNum — GoalType 1-13 (EverydayActivityType)

Function này trả về (NumOne, NumTwo) — giá trị hiện tại của nhân vật để so sánh với điều kiện hoạt động.

| Type | EverydayActivityType | NumOne | NumTwo | Field tương ứng |
|------|---------------------|--------|--------|----------------|
| 1 | EAT_QiangGou | `UserMoney` | - | Kim cương hiện có |
| 2 | EAT_InputExchange | `GetCurrentEverydayActJiFen()` | - | Điểm tích lũy hôm nay (DB Cmd 13172) |
| 3 | EAT_Consume | `mySaveData.CountNum` | - | CountNum từ EverydayActInfoDB (cộng dồn qua MoneyConst) |
| 4 | EAT_DirectAward | (không có case → default=0) | - | Chỉ kiểm tra điều kiện mua |
| 5 | EAT_Level | `ChangeLifeCount` | `Level` | Chuyển sinh + cấp |
| 6 | EAT_Wing | `WingData.WingID` | `WingData.ForgeLevel` | ID cánh + cấp forge cánh |
| 7 | EAT_Vip | `VipLevel` | - | Cấp VIP |
| 8 | EAT_ChengJiu | `ChengJiuManager.GetChengJiuLevel()` | - | Cấp thành tựu |
| 9 | EAT_JunXian | `GetShengWangLevelValue()` | - | Cấp danh vọng (ShengWang) |
| 10 | EAT_Merlin | `MerlinData._Level` | `MerlinData._StarNum` | Cấp Merlin + số sao |
| 11 | EAT_ShengWu | Tổng `HolyItemPartData.m_sSuit` | - | Tổng bộ trang bị thần vật |
| 12 | EAT_Ring | `MarriageData.byGoodwilllevel` | `MarriageData.byGoodwillstar` | Cấp nhẫn + số sao |
| 13 | EAT_ShouHuShen | `GuardStatue.Suit` | `CalMyGuardStatueLevel()` | Bộ thủ hộ thần + cấp |
| 14 | EAT_ZhiGou | `UserMoneyMgr.GetChargeItemDayPurchaseNum()` | - | Số lần mua ZhiGou hôm nay |

---

## 2. CheckCondition — Điều kiện mua

Error codes từ `EverydayActCheckCondition()`:
```
 0  → OK, có thể mua
-2  → ActID không tồn tại
-10 → Không đủ YuanBao (Type=1)
-12 → Chưa đủ điều kiện GoalData
-39 → Không đủ điểm tích lũy (Type=2)
-200 → Đã mua hết số lần (PurNum >= PurchaseNum)
-12 (Type=14) → ZhiGou không hợp lệ
```

---

## 3. GenerateEverydayActivity — Sinh hoạt động hàng ngày

```
OnRoleLogin(client) → GenerateEverydayActivity(client)
    → GetCurrentActGroupInfo()       // Từ GameConfig "everydayact": "day,groupID|day,groupID|..."
    → GenerateEverydayActGroupID()   // Sinh GroupID mới nếu chưa có hôm nay
         → PaiHangDB (cmd 269, top 100 mỗi PaiHangType) → CacheNeedCondition()
         → Lọc GroupID khả thi theo TypeID và LevLimit, random 1 GroupID
         → Lưu vào DB GameConfig "everydayact"
    → Sync EverydayActInfoDict: xóa hết groups hết hạn, thêm mới
```

**Lưu ý quan trọng**: Mỗi ngày tính theo `GetOffsetDay(DateTime)`, không phải DayOfYear. GroupID được **random** dựa trên điều kiện server (PaiHang top 100).

---

## 4. DB Commands (EverydayActivity)

| DB Cmd | Chức năng | Format |
|--------|----------|--------|
| 13170 | UpdateClientEverydayActData | `RoleID:GroupID:ActID:PurNum:CountNum:ActiveDay` |
| 13171 | DeleteClientEverydayActData | `RoleID:GroupID:ActID` |
| 13172 | GetCurrentEverydayActJiFen | `RoleID:dateFrom$:dateTo$` → trả về jifen |
| 13173 | UpdateEverydayJiFen (recharge) | `RoleID:delta:dateFrom$:dateTo$` |

---

## 5. GroupID chọn theo PaiHangTypes (Server Ranking)

```csharp
// ConvertPaiHangTypesToActNeedType()
PaiHangTypes.RoleLevel    → EANT_Level
PaiHangTypes.CombatForce  → EANT_CombatForce
PaiHangTypes.Wing         → EANT_Wing
PaiHangTypes.Ring         → EANT_Ring
PaiHangTypes.Merlin       → EANT_Merlin
PaiHangTypes.UserMoney    → EANT_UserMoney
PaiHangTypes.ChengJiu     → EANT_ChengJiu
PaiHangTypes.ShengWang    → EANT_ShengWang
PaiHangTypes.GuardStatue  → EANT_GuardStatue
PaiHangTypes.HolyItem     → EANT_HolyItem
```

Server lấy top 100 từ mỗi bảng xếp hạng, tính **trung bình**, rồi chọn GroupID phù hợp với mức đó. Kết quả: người chơi sẽ thấy hoạt động phù hợp với trình độ của server họ đang chơi.

---

## 6. Packet 770 (CMD_SPR_JIERIACT_STATE)

```csharp
// NotifyActivityState() — line 443
client.sendCmd(770, "12:1:::0:0");  // Open: type=12, flag=1
client.sendCmd(770, "12:0:::0:0");  // Close: type=12, flag=0
```

Điều kiện mở: `PlatformOpenStateVavle == 1 && !IsKuaFuLogin`

---

## 7. Tổng quan awards

```csharp
// EverydayActGiveAward() — 3 loại phần thưởng trong 1 ActID
GoodsDataListOne  → GiveAward(client, awardItem) — thường
GoodsDataListTwo  → GiveAward(client, awardItem) — theo nghề
GoodsDataListThr  → GiveEffectiveTimeAward() — có thời hạn
```

---

## 8. EveryDayActivity.xml Config Structure (từ code)

```xml
<EverydayActivityType TypeID="..." TypeID="..." LevLimit="min,max">
<EverydayActivityGroup GroupID="..." TypeID="..." NeedType="..." NeedNum="min1,min2,max1,max2">
    <ActivityID>...</ActivityID>
</EverydayActivityGroup>
<EverydayActivity ActID="..." Type="1..14" PurchaseNum="-1 hoặc N">
    <Price NumOne="..." ZhiGouID="..."/>
    <GoalData NumOne="..." NumTwo="..."/>
    <GoodsOne>id,cnt,bind,forge,ap,lk,ex</GoodsOne>
    <GoodsTwo>id,cnt,bind,forge,ap,lk,ex</GoodsTwo>
    <GoodsThr>id,cnt,bind,forge,ap,lk,ex:thời_hạn</GoodsThr>
</EverydayActivity>
```

**PurchaseNum = -1**: Mua 1 lần vĩnh viễn. `PurNum = 1` sau khi mua.
**PurchaseNum = N**: Giới hạn N lần. `PurNum++` sau mỗi lần.
