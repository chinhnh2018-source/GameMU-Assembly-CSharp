# EveryDayActivity XML — Phân tích cấu trúc thực tế

> Files:
> - `GameRes/Config/EveryDayActivity/EveryDayActivityType.xml` (12 dòng)
> - `GameRes/Config/EveryDayActivity/EveryDayActivityGroup.xml` (95 dòng, 93 Groups)
> - `GameRes/Config/EveryDayActivity/EveryDayActivity.xml` (266 dòng, ~264 Activities)

---

## 1. EveryDayActivityType — 9 TypeID đang dùng

```xml
<EveryDayActivityType TypeID="1" Name="转生等级" OpenLevel="0|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="2" Name="翅膀"     OpenLevel="2|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="3" Name="成就"     OpenLevel="2|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="5" Name="军衔"     OpenLevel="3|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="6" Name="梅林之书" OpenLevel="5|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="7" Name="圣物等级" OpenLevel="5|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="8" Name="婚戒"     OpenLevel="3|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="9" Name="守护神"   OpenLevel="3|1"   CloseLevel="100|100" />
<EveryDayActivityType TypeID="10" Name="充值"    OpenLevel="2|1"   CloseLevel="100|100" />
```

> **Lưu ý**: TypeID 4 (không có XML) = Type.LevLimit trong code C#.  
> `OpenLevel` = `転生|等级` tối thiểu để loại này xuất hiện.

### Map TypeID → GoalType (C# code)

| TypeID (XML) | GoalType (C# case) | NeedType | Điều kiện xác định |
|---|---|---|---|
| 1 | 5 (EAT_Level) | 2 | `ChangeLifeCount + Level` |
| 2 | 6 (EAT_Wing) | 4 | `WingID + ForgeLevel` |
| 3 | 8 (EAT_ChengJiu) | 7 | `ChengJiuLevel` |
| 5 | 9 (EAT_JunXian) | 8 | `ShengWangLevel` |
| 6 | 10 (EAT_Merlin) | 6 | `MerlinLevel + StarNum` |
| 7 | 11 (EAT_ShengWu) | 10 | `Tổng HolyItemPart.Suit` |
| 8 | 12 (EAT_Ring) | 5 | `GoodwillLevel + Star` |
| 9 | 13 (EAT_ShouHuShen) | 9 | `GuardStatue.Suit + Level` |
| 10 | 2 (EAT_InputExchange) | 3 | `Điểm nạp hôm nay` |

---

## 2. EveryDayActivityGroup — 93 Groups, phân tầng theo sức mạnh

### Format XML
```xml
<EveryDayActivityGroup 
  GroupID="1001" 
  TypeID="1" 
  Name="转生等级" 
  NeedType="2"            <!-- Type điều kiện lọc server ranking -->
  NeedNum="0,1|2,100"     <!-- min转生,min等级 | max转生,max等级 -->
  ActivityID="1000|2000|3000"  <!-- Danh sách ActID trong group này -->
/>
```

### Phân bố Groups theo TypeID

| TypeID | Tên | Số Groups | GroupID range | NeedType |
|--------|-----|-----------|--------------|---------|
| 1 | 转生等级 | 16 | 1001-1016 | 2 (Level) |
| 2 | 翅膀 | 9 | 1101-1109 | 4 (Wing) |
| 3 | 成就 | 11 | 1201-1211 | 7 (ChengJiu) |
| 5 | 军衔 | 11 | 1401-1411 | 8 (ShengWang) |
| 6 | 梅林之书 | 18 | 1501-1518 | 6 (Merlin) |
| 7 | 圣物等级 | 8 | 1601-1608 | 10 (ShengWu) |
| 8 | 婚戒 | 7 | 1701-1707 | 5 (Ring) |
| 9 | 守护神 | 7 | 1801-1807 | 9 (GuardStatue) |
| 10 | 现有钻石 | 5 | 1901-1905 | 3 (UserMoney) |
| **Tổng** | | **92** | | |

### Chi tiết NeedNum của TypeID=10 (Hiện có Kim cương)

| GroupID | NeedNum (min\|max kim cương) | ActivityIDs |
|---------|------------------------------|------------|
| 1901 | 0 ~ 10,000 | 2087\|3087 |
| 1902 | 10,001 ~ 20,000 | 2088\|3088 |
| 1903 | 20,001 ~ 30,000 | 2089\|3089 |
| 1904 | 30,001 ~ 50,000 | 2090\|3090 |
| 1905 | 50,001+ | 2091\|3091 |

### Chi tiết NeedNum của TypeID=6 (Merlin) — 18 tầng

| GroupID | Level Merlin (min\|max) | ActivityIDs |
|---------|------------------------|------------|
| 1501 | 0-10 sao | 1400\|2047\|3047 |
| 1502 | 3 chiều, 0-10 sao | 1401\|2048\|3048 |
| ... | ... | ... |
| 1518 | Level 19, 0+ sao | 2064\|3064 |

---

## 3. EveryDayActivity.xml — Cấu trúc thực tế

### Format XML đầy đủ
```xml
<EveryDayActivity 
  ActivityID="1000"
  Name="等级返利"
  GoalType="5"            <!-- Loại điều kiện (map sang C# switch case) -->
  GoalNum="4,1"           <!-- Ngưỡng điều kiện: 転生=4, Level=1 -->
  GoodsOne="50160,2,1,0,0,0,0|2030,20,1,0,0,0,0"  <!-- Thưởng miễn phí -->
  GoodsTwo=""             <!-- Thưởng theo nghề (empty = tất cả) -->
  GoodsThr=""             <!-- Thưởng có thời hạn (empty = không) -->
  EffectiveTime=""        <!-- Thời gian hiệu lực (empty = vĩnh viễn) -->
  Price=""                <!-- Giá mua (empty = miễn phí/tự động nhận) -->
  PurchaseNum=""          <!-- Số lần mua: "" = tự động, "1" = 1 lần -->
/>
```

### Phân loại hoạt động theo dải ActivityID

| Dải ID | Loại | GoalType | Cơ chế |
|--------|------|---------|--------|
| 1000-1099 | 等级返利 (Lv Rebate) | 5 | Đạt chuyển sinh → tự nhận thưởng |
| 1100-1199 | 翅膀返利 (Wing Rebate) | 6 | Đạt cấp cánh → tự nhận |
| 1200-1299 | 成就返利 (Achievement) | 8 | Đạt cấp thành tựu → tự nhận |
| 1300-1399 | 军衔返利 (Rank Rebate) | 9 | Đạt cấp quân hàm → tự nhận |
| 1400-1499 | 梅林返利 (Merlin Rebate) | 10 | Đạt cấp Merlin → tự nhận |
| 1500-1599 | 圣物返利 (HolyItem) | 11 | Đạt tổng suit thần vật → tự nhận |
| 1600-1699 | 婚戒返利 (Ring Rebate) | 12 | Đạt cấp nhẫn → tự nhận |
| 1700-1799 | 守护神返利 (Guardian) | 13 | Đạt cấp thủ hộ thần → tự nhận |
| **2000-2099** | **充值兑换 (Charge Shop)** | **2** | **Tiêu điểm nạp, mua 680 điểm** |
| 3000-3099 | (Nhóm 3) | Nhiều loại | Mix cả rebate + shop |

### Mẫu 充值兑换 (ID 2000-2015) — Cửa hàng nạp

```
ActivityID=2000: Price=680 JiFen, PurchaseNum=1
  GoodsOne = "5180,1,1,0,0,0,0|5313,10,1,0,0,0,0"
  → Nhận 1x ID5180 + 10x ID5313

ActivityID=2001..2015: đồng giá 680 JiFen, đổi phần thưởng khác nhau
  (WingID, Merlin materials, StrongBox...)
```

### Mẫu 等级返利 (ID 1000) — Trả thưởng miễn phí

```
ActivityID=1000: GoalType=5, GoalNum="4,1" (CS4, Lv1)
  GoodsOne = "50160,2,1,0,0,0,0|2030,20,1,0,0,0,0"
  → Nhận 2x ID50160 (Binding Diamond) + 20x ID2030
  Price = "", PurchaseNum = "" → tự động nhận khi đạt điều kiện
```

---

## 4. SpecialActivityTime.xml — Cơ chế lịch sự kiện SpecialActivity

### Format
```xml
<Time 
  GroupID="20161101"        <!-- Ngày bắt đầu dạng YYYYMMDD = Group ID -->
  ServerOpenFromDate="-1"   <!-- -1 = không giới hạn ngày khai server -->
  ServerOpenToDate="-1"     <!-- -1 = không giới hạn -->
  FromDate="2016-11-01 00:00:00"
  ToDate="2016-11-03 23:59:59"
/>
```

### Đặc điểm SpecialActivityTime

- **Chu kỳ**: Mỗi 3 ngày 1 sự kiện, liên tục từ 2016-11 đến hiện tại
- **GroupID = YYYYMMDD** → dùng để map sang SpecialActivity.xml
- `ServerOpenFromDate=-1`: Áp dụng cho **tất cả** server, bất kể ngày khai server
- Hiện có **147 dòng** (~145 nhóm thời gian), tức khoảng **435 ngày** hoạt động đã được lên lịch

---

## 5. So sánh 3 hệ thống hoạt động

| Tiêu chí | EverydayActivity | SpecialActivity | DailyActiveManager |
|---------|-----------------|-----------------|---------------------|
| **Tần suất** | Hàng ngày, random 1 GroupID | Theo lịch cố định (3 ngày/lần) | Hàng ngày reset |
| **Cơ chế chọn** | Random dựa trên Server Ranking | GroupID = ngày tháng | Cố định 19 ID |
| **Lưu trữ** | DB `t_everyday_act` | DB `t_special_act` | DB `t_role_daily_data` |
| **Phần thưởng** | GoodsOne/Two/Thr | GoodsOne/Two/Thr | Awards XML |
| **Điều kiện** | GoalType 1-13 | GoalType 1-13 | DailyActive flag (bitmask) |
| **Giá mua** | Price (JiFen hoặc miễn phí) | Price (JiFen hoặc miễn phí) | Không có |
| **Số lần** | PurchaseNum (-1 hoặc N) | PurchaseNum | 1 lần/ngày |
| **Kích hoạt** | `EveryDayActivityOpen` | Thời gian FromDate-ToDate | Luôn bật |
| **XML root** | `EveryDayActivity/` dir | `SpecialActivity/` dir | `DailyActiveInfor.xml` |

---

## 6. GoalType trong XML vs GoalType trong Code C#

> **Phát hiện quan trọng**: GoalType trong XML **KHÁC** với TypeID trong XML.

```
XML GoalType = tham số trực tiếp trong <EveryDayActivity GoalType="5"/>
               ↔ C# switch(myActConfig.Type) case 5: → EAT_Level

XML TypeID   = tham số trong <EveryDayActivityType TypeID="1"/>
             ≠ GoalType (mapping phức tạp hơn)
```

### Mapping hoàn chỉnh (từ XML thực tế)

| XML GoalType | Tên hoạt động XML | C# case | Dữ liệu đo |
|---|---|---|---|
| 2 | 充值兑换 | EAT_InputExchange | Điểm nạp hôm nay (DB 13172) |
| 5 | 等级返利 | EAT_Level | ChangeLifeCount + Level |
| 6 | 翅膀返利 | EAT_Wing | WingID + ForgeLevel |
| 8 | 成就返利 | EAT_ChengJiu | ChengJiuManager.GetChengJiuLevel |
| 9 | 军衔返利 | EAT_JunXian | GetShengWangLevelValue |
| 10 | 梅林返利 | EAT_Merlin | MerlinData._Level + _StarNum |
| 11 | 圣物返利 | EAT_ShengWu | Tổng HolyItem Suit |
| 12 | 婚戒返利 | EAT_Ring | GoodwillLevel + Star |
| 13 | 守护神返利 | EAT_ShouHuShen | GuardStatue.Suit + Level |

---

## 7. Số lượng thực tế (EveryDayActivity.xml)

| Nhóm | Số Activities |
|------|--------------|
| 等级返利 (1000-1099) | 16 entries |
| 翅膀返利 (1100-1199) | 8 entries |
| 成就返利 (1200-1299) | 10 entries |
| 军衔返利 (1300-1399) | 10 entries |
| 梅林返利 (1400-1499) | 16 entries |
| 圣物返利 (1500-1599) | 7 entries |
| 婚戒返利 (1600-1699) | 6 entries |
| 守护神返利 (1700-1799) | 6 entries |
| 充值兑换 Group 2 (2000-2099) | 92 entries |
| Group 3 (3000-3099) | 92 entries |
| **Tổng** | **~263** |

---

## 8. Quan hệ giữa các file

```
EveryDayActivityType.xml
    └── TypeID → định nghĩa loại, OpenLevel

EveryDayActivityGroup.xml
    ├── GroupID → thuộc TypeID
    ├── NeedType, NeedNum → điều kiện lọc server PaiHang
    └── ActivityID list → các ActID trong nhóm

EveryDayActivity.xml
    ├── ActivityID → từng hoạt động cụ thể
    ├── GoalType, GoalNum → điều kiện nhân vật
    ├── Price, PurchaseNum → cơ chế mua
    └── GoodsOne/Two/Thr → phần thưởng

Server-side (C#):
    GenerateEverydayActGroupID():
        1. Query PaiHang top 100 → CacheNeedCondition
        2. Filter Groups theo NeedType+NeedNum vs PaiHang data
        3. Random 1 GroupID → save to DB "everydayact"
    
    CheckCondition():
        GetCurrentGoalNum() → compare vs GoalNum
        → Allow/Deny purchase
```
