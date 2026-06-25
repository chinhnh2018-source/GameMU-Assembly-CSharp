# Hệ thống Event — Tổng hợp toàn bộ (SevenDay + SpecialActivity)

> Files phân tích:
> - `SevenDay/` (6 files): Login, ChongZhi, Goal, QiangGou, GoalType, ActivityType
> - `SpecialActivity/` (2 files): SpecialActivity.xml (188 dòng), SpecialActivityTime.xml (147 dòng)

---

## PHẦN 1: SevenDay — Hoạt động 7 ngày đầu tiên

### 1.1 Kiến trúc 4 Module

```
SevenDayActivityType.xml
├── ActivityType=1: 登录福利    → SevenDayLogin.xml
├── ActivityType=2: 充值福利    → SevenDayChongZhi.xml
├── ActivityType=3: 七日目标    → SevenDayGoal.xml  (GoalType.xml)
└── ActivityType=4: 七日抢购    → SevenDayQiangGou.xml
```

---

### 1.2 Module 1: 登录福利 (Login Rewards — 7 ngày)

`SevenDayLogin.xml` — 7 phần thưởng, 1 phần thưởng/ngày đăng nhập:

| ID | Ngày | Phần thưởng (GoodsOne) |
|----|------|------------------------|
| 1 | Ngày 1 | 1x 50160 (Bó kim cương) + 2x 5050 (?) |
| 2 | Ngày 2 | 1x 50160 + 30x 2016 (Tinh anh rune) |
| 3 | Ngày 3 | 1x 50160 + 1x 50017 (Binding Gold box) |
| 4 | Ngày 4 | 1x 50160 + 10x 2001 (Chúc phúc tinh thạch) |
| 5 | Ngày 5 | 1x 50160 + 20x 2003 (Sinh mệnh tinh thạch) |
| 6 | Ngày 6 | 1x 50160 + 10x 2017 (Thần ưng hỏa chủng) |
| 7 | Ngày 7 | 1x 50160 + **1x ID1030903** (giá trị = `2359296`) |

> **ID1030903** — Trang bị cuối với flag `2359296` — là chuỗi hex `0x240000` gợi ý đây là trang bị đặc biệt với lần thăng hạng cao.

---

### 1.3 Module 2: 充值福利 (Recharge Milestone Rewards)

`SevenDayChongZhi.xml` — Nạp đủ kim cương hàng ngày → nhận thưởng:

| ID | MinZhuanShi (KCương/ngày) | Phần thưởng |
|----|--------------------------|-------------|
| 1 | ≥ 300 | 1x 50160 + 1x 5079 (10万 MoJing) |
| 2 | ≥ 500 | 1x 50160 + 150x 2016 |
| 3 | ≥ 1,000 | 1x 50161 + 5x 50017 |
| 4 | ≥ 2,000 | 1x 50161 + 50x 2001 |
| 5 | ≥ 3,000 | 2x 50161 + 100x 2003 |
| 6 | ≥ 5,000 | 1x 50164 + 50x 2017 |
| 7 | ≥ **8,880** | 1x 50165 + **1x ID1030905** (flag `27525120`) |

> Tier 7 = nạp 8,880 KCương/ngày (≈88.8 USD) → nhận trang bị cực hiếm.

---

### 1.4 Module 3: 七日目标 (Seven-Day Goals)

`GoalType.xml` — 14 loại mục tiêu, mỗi loại gắn với 1-2 ngày:

| TypeID | Tên | Day | FunctionType range |
|--------|-----|-----|-------------------|
| 1 | 等级 (Level) | 1 | 1: 达到等级 |
| 2 | 魔晶 (MoJing) | 1 | 2: Kỹ năng LV, 3: Tích lũy MoJing, 4: Thu hồi MoJing, 5: Đổi vật phẩm |
| 3 | 竞技场 (Arena) | 2 | 6: Tham gia, 7: Thắng, 8: Đạt xếp hạng |
| 4 | 基础装备 (Basic Equip) | 2 | 9-16: Mặc trang bị, Thu hồi, Bán, Cầu phúc, Nhặt |
| 5 | 副本 (Dungeon) | 3 | ? |
| 6 | 合成转换 (Synthesis) | 3 | ? |
| 7 | 祝福晶石 | 4 | ? |
| 8 | 装备强化 (Forge) | 4 | ? |
| 9 | 生命晶石 | 5 | ? |
| 10 | 装备追加 (ZhuiJia) | 5 | ? |
| 11 | 神鹰火种 | 6 | ? |
| 12 | 翅膀 (Wings) | 6 | ? |
| 13 | 图鉴精魄 | 7 | ? |
| 14 | 装备进阶 | 7 | ? |

`SevenDayGoal.xml` — 194 dòng, sample các Goal ID:

| ID range | Day | GoalType | Mô tả | TypeGoal |
|----------|-----|---------|-------|---------|
| 1000-1016 | 1 | 1 | 等级达到X转Y级 | `转生,等级` |
| 2000-2003 | 1 | 2 | N kỹ năng lên Lv X | `số kỹ năng,cấp kỹ năng` |
| 3000-3001 | 1 | 2 | Tích lũy MoJing | `số MoJing` |
| 4000-4001 | 1 | 2 | Thu hồi MoJing | `số MoJing` |
| 5000-5003 | 1 | 2 | Đổi tinh thạch | `ID vật phẩm,số lượng` |
| 6000-6003 | 2 | 3 | Tham gia đấu trường N lần | `số lần` |
| 7000-7003 | 2 | 3 | Thắng đấu trường N lần | `số lần` |
| 8000-8006 | 2 | 3 | Xếp hạng đấu trường top N | `số thứ hạng` |
| 9000 | 2 | 4 | Mặc 3 trang bị xanh+ | `số lượng` |
| 10000-10003 | 2 | 4 | Mặc N trang bị tím+ | `số lượng` |
| 12000-12001 | 2 | 4 | Thu hồi N trang bị | `số lượng` |

---

### 1.5 Module 4: 七日抢购 (Flash Purchase — 50% OFF)

`SevenDayQiangGou.xml` — Mỗi ngày mở 1-2 item giảm giá 50%:

| ID | Day | Tên | Vật phẩm | Giá gốc | Giá KM | Số lần |
|----|-----|-----|----------|---------|--------|--------|
| 1 | Ngày 1 | 10万魔晶 | 5079×1 | 200 KCương | **100** | 5 |
| 2 | Ngày 2 | 祈福代劵 | 7004×1 | 200 | **100** | 5 |
| 3 | Ngày 3 | 188万绑金 | 50033×1 | 100 | **50** | 10 |
| 4 | Ngày 4 | 祝福晶石 | 2001×1 | 50 | **25** | 20 |
| 5 | Ngày 5 | 生命晶石×10 | 2003×10 | 100 | **50** | 4 |
| 6 | Ngày 5 | 成就印记 | 5030×1 | 60 | **30** | 10 |
| 7 | Ngày 6 | 神鹰火种 | 2017×1 | 20 | **10** | 20 |
| 8 | Ngày 6 | 10000星魂 | 5088×1 | 20 | **10** | 30 |
| 9 | Ngày 7 | 召唤代劵 | 7005×1 | 200 | **100** | 5 |

> Tất cả 9 item đều giảm **đúng 50%**. Ngày 5 có 2 item.

---

## PHẦN 2: SpecialActivity — Hoạt động theo lịch (3 ngày/đợt)

### 2.1 Cấu trúc XML đầy đủ

```xml
<Activity 
  ID="6763"
  GroupID="20180227"         <!-- = ngày bắt đầu đợt, liên kết SpecialActivityTime -->
  Name="冬日特惠"
  Day="1,1"                  <!-- minDay,maxDay → chỉ hiển thị ngày 1 của đợt -->
  NeedLevel="-1"             <!-- -1 = không giới hạn -->
  NeedVIP="-1"               <!-- -1 = không giới hạn VIP -->
  NeedChongZhi="-1"          <!-- -1 = không giới hạn nạp -->
  NeedWing="-1"              <!-- Wing cấp min-max -->
  NeedChengJiu="-1"          <!-- Achievement level min-max -->
  NeedJunXian="-1"           <!-- Rank min-max -->
  NeedMerlin="-1"            <!-- Merlin level min-max -->
  NeedShengWu="-1"           <!-- HolyItem total min-max -->
  NeedRing="-1"              <!-- Ring level min-max -->
  NeedShouHuShen="-1"        <!-- Guardian statue min-max -->
  Type="14"                  <!-- 14=直购(Direct Buy), 2=充值兑换(JiFen Exchange) -->
  Goal="-1"                  <!-- -1 = không yêu cầu mục tiêu -->
  GoodsOne="63220,1,1,0,0,0,0|2031,88,1,0,0,0,0"
  GoodsTwo=""
  GoodsThr=""
  EffectiveTime=""
  Price="1|11|7812"          <!-- currency_type|amount|shop_id -->
  PurchaseNum="5"            <!-- Số lần mua tối đa -->
/>
```

### 2.2 Giải mã trường `Price`

| Format | Ý nghĩa |
|--------|---------|
| `"1\|11\|7812"` | Type 1 = kim cương, 11 = số lượng, 7812 = ShopID |
| `"1000"` | Chỉ 1 số = JiFen (điểm nạp, Type=2) |
| `"680"` | JiFen trong EverydayActivity Shop |
| `""` | Miễn phí (tự động nhận) |

**Bảng Type 14 Price mapping (SpecialActivity):**
- `Price="1|11|7812"` → Mua bằng 11 kim cương, ShopID=7812
- `Price="1|14|7813"` → 14 kim cương, ShopID=7813
- `Price="1|16|7xxx"` → 16 kim cương
- `Price="1|19|7xxx"` → 19 kim cương
- `Price="1|20|7xxx"` → 20 kim cương

> **ShopID** là ID tham chiếu đến DB bảng `t_shop` để tracking lịch sử mua.

### 2.3 Phân loại Type

| Type | Tên | Cơ chế | Price format |
|------|-----|--------|-------------|
| 2 | 充值兑换 | Tiêu JiFen (điểm nạp) | `"1000"`, `"3000"` |
| 14 | 直购 (Direct Buy) | Mua trực tiếp bằng kim cương | `"1\|XX\|ShopID"` |

### 2.4 Cơ chế lọc người chơi (NeedXxx)

SpecialActivity dùng NeedXxx để lọc — **khác với EverydayActivity**:

| Attribute | Giá trị -1 | Giá trị có cấu hình |
|-----------|-----------|---------------------|
| NeedLevel | Tất cả | `"2,1\|7,100"` = CS2-7, Lv1-100 |
| NeedVIP | Tất cả | `"0\|6"` = VIP 0-6 |
| NeedMerlin | Tất cả | `"1,0\|11,10"` = Merlin Lv1-11 |
| NeedShengWu | Tất cả | `"1\|130"` = 1 đến 130 suit |
| NeedChongZhi | Tất cả | Số kim cương đã nạp |

**Ví dụ từ ID=6818-6821** (phân tầng theo Level):
```
ID=6818: NeedLevel="2,1|7,100"  → CS2-7  → Price="1000" JiFen → 100 5122 + 200 63149
ID=6819: NeedLevel="8,1|10,100" → CS8-10 → Price="3000" JiFen → 300 5122 + 300 63149
ID=6820: NeedLevel="11,1|12,100"→ CS11-12→ Price="5000" JiFen → 600 5122
ID=6821: NeedLevel="13,1|99,100"→ CS13+  → Price="6480" JiFen → 700 5122
```

**Ví dụ từ ID=6822-6825** (phân tầng theo VIP):
```
ID=6822: NeedVIP="0|6"   → VIP 0-6   → 40x 5271, Price=1000
ID=6823: NeedVIP="7|9"   → VIP 7-9   → 80x 5271, Price=3000
ID=6824: NeedVIP="10|11" → VIP 10-11 → 120x 5271, Price=5000
ID=6825: NeedVIP="12|99" → VIP 12+   → 140x 5271, Price=6480
```

> **Kết luận**: SpecialActivity tự hiển thị item **khác nhau** tùy người chơi mạnh/yếu, giàu/nghèo.

### 2.5 Cơ chế Day (ngày trong đợt)

```
Day="1,1" → Chỉ ngày 1 của đợt (3 ngày) mới thấy item này
Day="2,2" → Chỉ ngày 2
Day="3,3" → Chỉ ngày 3
Day="-1"  → Tất cả 3 ngày đều thấy
```

Mỗi đợt 3 ngày → ~20 item/ngày, tổng ~60 item/GroupID.

---

## PHẦN 3: So sánh đầy đủ 4 hệ thống Event

| Tiêu chí | SevenDay | EverydayActivity | SpecialActivity | DailyActive |
|---------|----------|-----------------|-----------------|-------------|
| **Kích hoạt** | 7 ngày đầu sau tạo nhân vật | Hàng ngày | Lịch cố định (SpecialActivityTime) | Hàng ngày |
| **DB storage** | `t_seven_day_act` | `t_everyday_act` | `t_special_act` | `t_role_daily_data` |
| **Điều kiện** | Day + Level/Forge/Arena... | GoalType (Level/Wing/Merlin...) | NeedXxx (Level/VIP/Wing...) | Bitmask flag |
| **Giá** | QiangGou: 50% OFF | JiFen (680/đợt) | JiFen (1000-6480) hoặc KCương trực tiếp | Miễn phí |
| **Lịch** | Ngày 1-7 (tuyến tính) | Random mỗi ngày | 3 ngày/đợt | Reset 0h hàng ngày |
| **Phân tầng** | Theo cấp độ nhân vật | Groups theo PaiHang ranking | NeedXxx (cực chi tiết) | Không |
| **XML files** | 6 files | 3 files | 2 files | 1 file |
| **Control switch** | Không rõ | `EveryDayActivityOpen` | `FromDate-ToDate` | Không rõ |
| **Số lượng item** | ~40 items (7 ngày × 6 loại) | ~263 activities | ~186 activities | ~19 activities |

---

## PHẦN 4: ID vật phẩm quan trọng xuất hiện nhiều

| GoodsID | Tên ước đoán | Xuất hiện trong |
|---------|-------------|----------------|
| 50160 | Bó kim cương (nhỏ) | Login, EverydayActivity rebate |
| 50161/50164/50165 | Bó kim cương (lớn hơn) | SevenDayChongZhi |
| 2016 | Rune tinh anh | SevenDayLogin, EverydayActivity |
| 2017 | Thần Ưng Hỏa Chủng (wing mat) | Nhiều chỗ |
| 2001 | Chúc Phúc Tinh Thạch | Nhiều chỗ |
| 2003 | Sinh Mệnh Tinh Thạch | Nhiều chỗ |
| 5079 | 10万 MoJing | SevenDayQiangGou |
| 5088 | 10000 Star Soul | SevenDayQiangGou |
| 7004 | Cầu Phúc Đại券| SevenDayQiangGou |
| 7005 | Triệu Hoán Đại券| SevenDayQiangGou |
| 5313 | Vật phẩm tiêu thụ (EDA) | EverydayActivity shop |
| 51275 | Awakening material | SpecialActivity |
| 63xxx | Trang bị/Thời trang đặc biệt | SpecialActivity |

---

## PHẦN 5: Kiến trúc tổng thể từ XML đến Server

```
[Config XMLs]                [Server RAM Cache]           [DB Tables]
                             
SevenDayActivityType.xml ──► SevenDayMgr.Init()     ──►  t_seven_day_act
SevenDayLogin.xml            _loginList[]                  (roleID, dayIndex, claimed)
SevenDayChongZhi.xml         _chongZhiList[]
SevenDayGoal.xml    ─────────_goalList[Day][GoalType]
SevenDayQiangGou.xml         _qiangGouList[]

EveryDayActivityType.xml ──► EverydayActivity.Init() ──►  t_everyday_act
EveryDayActivityGroup.xml    _typeList[]                   (groupID, actID, status)
EveryDayActivity.xml         _groupList{}
                             _actList{}
SystemParams (EveryDayActivityOpen=1,1,1,1)

SpecialActivity.xml ────────► SpecialActivityMgr   ──►  t_special_act
SpecialActivityTime.xml       _actList[GroupID]            (roleID, actID, buyCount)
                             (FilterByNeedXxx per player)

DailyActiveInfor.xml ──────► DailyActiveManager    ──►  t_role_daily_data
                             _activityList[]               (roleID, bitmask)
```

---

## PHẦN 6: Ưu tiên tích hợp vào GameMU.Manager

### Tầm quan trọng (cao → thấp)

| # | Hệ thống | REST API cần | Lý do |
|---|---------|-------------|-------|
| 1 | **EverydayActivity** | `GET /api/events/everyday/current` | GroupID thay đổi hàng ngày, cần monitor |
| 2 | **SpecialActivity** | `GET /api/events/special/active` | 3 ngày/đợt, cần biết đợt nào đang chạy |
| 3 | **SevenDay** | `GET /api/events/sevenday/config` | Cấu hình tĩnh, ít thay đổi |
| 4 | **DailyActive** | `GET /api/events/daily/config` | Cấu hình tĩnh |

### API thiết kế đề xuất (Phase 3)

```http
# EverydayActivity
GET  /api/events/everyday/current        → GroupID hôm nay + danh sách activities
GET  /api/events/everyday/groups         → Tất cả groups
GET  /api/events/everyday/types          → 9 TypeID

# SpecialActivity  
GET  /api/events/special/schedule        → Lịch sự kiện (SpecialActivityTime)
GET  /api/events/special/active          → Đợt đang chạy (GroupID theo ngày hôm nay)
GET  /api/events/special/activities/:groupId → Chi tiết activities của 1 đợt

# SevenDay
GET  /api/events/sevenday/login          → Login rewards (7 items)
GET  /api/events/sevenday/goals          → Goals theo ngày
GET  /api/events/sevenday/flash          → Flash purchase items
GET  /api/events/sevenday/recharge       → Recharge milestones

# SystemParams
GET  /api/config/params/:name            → Lấy giá trị 1 tham số
PUT  /api/config/params/:name            → Sửa tham số (cần restart server)
```
