# SystemParams.xml — Phân tích toàn diện

> File: `GameRes/Config/SystemParams.xml`  
> Kích thước: 148KB, 1871 dòng, ~350 tham số  
> Mục đích: Cấu hình toàn bộ tham số cân bằng game, không cần recompile server

---

## 1. Cơ chế trang bị (Forge / ZhuiJia / ZhuanSheng)

### 1.1 Tăng cường (Forge) — 21 cấp (0→20)
| Param | Mô tả | Giá trị key |
|-------|--------|-------------|
| `ForgeLevelAddAttackRates` | Tỷ lệ tăng attack theo cấp forge | 0→3.5 (21 items) |
| `ForgeLevelAddDefenseRates` | Tỷ lệ tăng defense theo cấp forge | 0→3.5 |
| `ForgeLevelAddMaxLifeVRates` | Tỷ lệ tăng MaxHP | 0→3.5 |
| `ForgeNeedGoodsIDs` | Vật liệu bắt buộc (pipe-separated per level) | ID 2000,2001,2002 |
| `ForgeNeedGoodsNum` | Số lượng vật liệu mỗi cấp | Tăng dần |
| `ForgeProtectStoneGoodsIDS` | Thần Hựu Tinh Thạch (ID,số) mỗi cấp | ID 2005 từ +8 |
| `ForgeGoodsRate` | Tỷ lệ thành công (%) | 100%→15% (cấp 19-20) |
| `ForgeLevelNeedYinLiang` | Kim tệ tiêu tốn mỗi cấp | 0→6,297,000 |
| `ForgeLuckyGoodsIDs` | Danh sách Thần Bùa may mắn | ID 35020-35030 (11 loại) |
| `ForgeLuckyGoodsRate` | Tỷ lệ tương ứng mỗi loại thần bùa | 5%→100% |
| `ForgeMaxOpen` | Mở giới hạn forge +20 | `2,2018-07-01,...` (theo thời gian) |
| `ForgeProtectOpen` | Bảo vệ không tụt quá 1 cấp (15→16) | `1` = bật |

### 1.2 Truy Gia (ZhuiJia) — 81 cấp (0→80)
| Param | Mô tả |
|-------|--------|
| `ZhuiJiaLevelAddAttackRates` | Tỷ lệ tăng attack 0→4.0 (81 items) |
| `ZhuiJiaLevelAddDefenseRates` | Tỷ lệ tăng defense 0→4.0 |
| `ZhuiJiaForgeGoodsIDs` | Vật liệu = ID 2003 (tất cả cấp) |
| `ZhuiJiaForgeGoodsNum` | Số lượng: 0→482 (tăng theo bảng) |
| `ZhuiJiaGoodsRate` | Tỷ lệ: 100% tất cả cấp |
| `ZhuiJiaXiaoHaoJinBi` | Kim tệ: 0→2,750,000 |

### 1.3 Truyền Thừa (ChuanCheng)
| Param | Mô tả | Value |
|-------|--------|-------|
| `ChuanChengGoodsRate` | Thành công % | 100% |
| `ChuanChengXiaoHaoJinBi` | Kim tệ: 21 items | 0→3,700,000 |
| `ChuanChengXiaoHaoZhuanShi` | Kim cương tiêu tốn | 0→182 |
| `XiLianChuanChengGoodsRate` | Truyền thừa tẩy luyện | 100% |
| `XiLianChuanChengXiaoHaoJinBi` | Kim tệ | 100,000 |
| `XiLianChuanChengXiaoHaoZhuanShi` | Kim cương | 3 |
| `JuHunChuanChengGoodsRate` | Tụ hồn truyền thừa | 100% |
| `JuHunChuanChengXiaoHaoZhuanShi` | Kim cương | 80 (tất cả 10 cấp) |

### 1.4 Chuyển Sinh Trang Bị (EquipZhuanSheng) — 11 cấp
| Param | Mô tả |
|-------|--------|
| `EquipZhuanShengNeedGoods` | Vật phẩm cần: ID 2050-2059 |
| `EquipZhuanShengNeedMoJing` | Mộ Tinh cần: 100→1000 |
| `EquipZhuanShengRate` | Thành công: 80%→50% |
| `EquipZhuanShengAddAttackRates` | Tăng attack: 0→3.0 |
| `EquipZhuanShengBoLiZhuanShi` | Bóc tách kim cương: 2→7776 |

### 1.5 Tẩy Luyện (XiLian)
- `TianshengXilianMoney` = 2,100,000,000 (tẩy thiên sinh)
- `TianshengJihuoGoodsID` = `1,2` (kích hoạt đá = thiên sinh)

---

## 2. VIP System

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `VIPBiaoShi` | Hiển thị nhãn VIP từ cấp | 1 |
| `VIPChuanSong` | VIP truyền tống vùng từ cấp | 2 |
| `VIPBossChuanSong` | VIP Boss truyền tống từ cấp | 4 |
| `VIPLiXianBaiTan` | Offline buôn từ cấp | 3 |
| `VIPSaoDang` | Quét sạch dungeon từ cấp | 0 |
| `VIPRiChangYiJianWanCheng` | VIP hoàn thành nhiệm vụ 1-click | 3 |
| `VIPJingJiCD` | Xóa CD đấu trường | Cấp 1 |
| `VIPQiangHuaAdd` | Tăng tỷ lệ forge theo VIP | 0→60 (% thêm) |
| `VIPZhuiJiaAdd` | Tăng tỷ lệ ZhuiJia | 0 (VIP0-10), 20→60 (VIP11-15) |
| `VIPJinBiLianZhi` | Số lần luyện kim tệ/ngày | 0→25 |
| `VIPBangZuanLianZhi` | Luyện kim cương bó/ngày | 0→25 |
| `VIPZuanShiLianZhi` | Luyện kim cương/ngày | 0→29 |
| `VIPJingJi` | Số lần mua thêm thách đấu/ngày | 0→75 |
| `VIPJinYanFuBenNum` | Mua thêm exp dungeon/ngày | 0→10 |
| `VIPResurrectionAdd` | Lần phục sinh miễn phí/ngày | 0→30 |
| `VIPHuoYueAdd` | Điểm hoạt động bonus | 0→15 |
| `VIPEnterBloodCastleCountAddValue` | Thêm lần Blood Castle | 0→3 |
| `VIPGouMaiJingJi` | Tiêu kim cương mua lần đấu | 10 |
| `VIPGoodsID` | ID vật phẩm VIP card | 30000,30001,30002 |

---

## 3. Nhiệm vụ Hàng ngày

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `DailyCircleTaskInfo` | Phạm vi nhiệm vụ theo cấp độ | 0转30级→20000-20020... |
| `CompleteTaskNeedYuanBao` | Kim cương hoàn thành 1 vòng | 10 |
| `DoubleExp` | Kim cương nhận 2x kinh nghiệm | 5 |
| `DailyTaskMaxNum0-10` | Số vòng tối đa mỗi giai đoạn | 10 |
| `PriceTaskInfo` | Nhiệm vụ thảo phạt theo cấp | 2转→16转 |
| `PriceTaskNum` | Số nhiệm vụ thảo phạt tối đa | 0 (tắt) |

---

## 4. Chiến Trận (PvP / Battles)

### 4.1 Đấu Trường (JingJi)
| Param | Mô tả |
|-------|--------|
| `JingJiFuBenID` | ID dungeon đấu trường | 9000 |
| `CDXiaoHaoZhuanShi` | Kim cương/giây CD | 0.1 |
| `JingJiBuff` | BUFF nhân vật gương | ID 2000900 |

### 4.2 Lãnh Địa Chiến (Siege Wars)
| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `LingDiZhanWeekDays` | Thứ tổ chức lãnh địa chiến | 7 (CN) |
| `LingDiZhanFightingDayTimes` | Khung giờ | 16:30-17:30 |
| `HuangChengZhanWeekDays` | Hoàng Thành chiến | -1 (tắt) |
| `WangChengZhanWeekDays` | Vương Thành chiến | -1 (tắt) |
| `ZhanMengNeed` | Điều kiện tạo chiến minh | 0\|50\|500,000\|-1 |
| `BangHuiFightingLineID` | Line ID tổ chức chiến | 1 |

### 4.3 PK System
| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `MaxFallRoleUsingRate` | Xác suất rơi đồ đang dùng | 0 |
| `MaxFallRoleBagRate` | Xác suất rơi đồ túi | 0 |
| `MaxFallRedRoleUsingRate` | Đỏ tên rơi đồ đang dùng | 300/1000 |
| `MaxFallRedRoleBagRate` | Đỏ tên rơi đồ túi | 500/1000 |
| `RedRoleDropGoodsNum` | Số đồ tối đa rơi khi chết đỏ | 1 |
| `OrdinaryRole` | Bạch danh chết có rơi đồ | 1 |
| `HongMingDebuff` | Debuff khi đỏ tên | attack-1%,def-1%,HP-1% |

### 4.4 Chiến Trường (Battles)
| Param | Mô tả |
|-------|--------|
| `WarriorBattleUltraKill` | Liên sát: cơ bản 27, hệ số 3, min 30, max 75 |
| `KingOfBattleUltraKill` | Vương giả chiến trường: tương tự |
| `CrusadeUltraKill` | Thập tự liên sát: 50,50,0,300 |
| `CrusadeEnterTime` | Lần vào/vòng: 30, giới hạn 60 |
| `CrusadeSeason` | Số vòng/mùa giải | 13 |

---

## 5. Activity Events (Hoạt Động)

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `EveryDayActivityOpen` | Bật/tắt EverydayActivity mỗi platform | `1,1\|2,1\|3,1\|4,1` (tất cả bật) |
| `EveryDayChongZhiDuiHuan` | Tỷ giá nạp → điểm hàng ngày | `20000:1` |
| `JieRiChongZhiDuiHuan` | Tỷ giá nạp → điểm lễ hội | `20000:1` |
| `SpecialChongZhiDuiHuan` | Tỷ giá nạp → điểm chuyên | `1:1` |
| `ZhouMoChongZhiTime` | Thời gian nạp cuối tuần | T7-CN |

> **Quan trọng**: `EveryDayActivityOpen` = **param điều khiển EverydayActivity bật/tắt theo platform**  
> Format: `platformType,switch|...` (1=iOS, 2=JB, 3=Android, 4=YYB)  
> Giá trị hiện tại: **tất cả 4 platform đều bật (1)**

---

## 6. Anticheat / Bot Detection

| Param | Mô tả |
|-------|--------|
| `BanOpenPlayform` | Bật phát hiện hack theo platform |
| `TimeOutType` | Loại timeout = 81 |
| `TimeOutCount` | Đếm trước ban = 3 |
| `RobotMaxRepeatLogCount` | Log lặp tối đa = 2000 |
| `RobotBanMinutes` | Thời gian ban: 10, 1440, -1 (vĩnh viễn) |
| `JiaoBenKick_APP` | iOS kick khi phát hiện script | 1 |
| `RobotKick_Android` | Android kick khi phát hiện emulator | 1 |
| `JiaoBenDropRateDown_APP` | Giảm drop rate map khi phát hiện script | 10% |
| `IPRestrict` | Giới hạn tạo nhân vật/IP | 13 |
| `DeviceIDRestrict` | Giới hạn tạo nhân vật/device | -1 (vô hạn) |

---

## 7. Server Limits & Login Queue

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `MaxServerNum` | Non-VIP: bắt đầu xếp hàng 700, tối đa 900, hàng tối đa 1000 |
| `MaxServerNum` (VIP) | VIP: 1000, 1000, 1000 (không xếp hàng) |
| `DeleteRoleNeedTime` | Thời gian chờ xóa nhân vật | 120 phút |
| `BagClearUpCD` | CD sắp xếp túi | 1000ms |
| `MinLevelForSendWorldMessage` | Cấp tối thiểu chat thế giới | 1 |
| `MinIntervalSecsForSendWorldMessage` | Khoảng cách giữa 2 lần chat thế giới | 5 giây |
| `MinLevelForMailSend` | Cấp tối thiểu gửi mail | 20 |

---

## 8. Game Economy

### 8.1 Luyện Kim (Lian Zhi)
| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `JinBiLianZhi` | Luyện kim tệ: 200,000 JB → 184,320 kinh nghiệm, 3 lần/ngày |
| `BangZuanLianZhi` | Luyện bó kim cương: 10→10,000 star soul, 3 lần/ngày |
| `ZuanShiLianZhi` | Luyện kim cương: 20 → exp + starsoul + 200,000 JB, 5 lần/ngày |

### 8.2 Tỷ giá Chuyển sinh kinh nghiệm
`ZhuanShengExpXiShu` = `1,2,4,7,11,16,22,29,37,46,56,...`  
→ 0 chuyển sinh = hệ số 1x, 1CS = 2x, ..., 5CS+ = 56x

### 8.3 Giao dịch
| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `JiaoYiShui` | Thuế giao dịch thị trường | 5% kim tệ + 5% kim cương |
| `ShangJiaNumber` | Số slot đăng bán tối đa | 16 |
| `ShangJiaTime` | Thời gian đăng bán tối đa | 43200 phút (~30 ngày) |
| `EmailMoney` | Phí gửi mail | 100 JB |
| `EmailMoneyWithAttachment` | Phí gửi mail kèm vật phẩm | 500 JB |

---

## 9. Chiến Minh / Quân Đoàn (ZhanMeng / Legion)

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `ZhanMengNeed` | Điều kiện tạo: 0转\|50 lv\|500,000 JB\|-1 (vật phẩm) |
| `ZhanMengChuShiZiJin` | Vốn khởi đầu chiến minh | 50,000 |
| `ZhanMengWeiHuFeiYong` | Phí duy trì/ngày | 10,000 |
| `AlignZhanMengLevel` | Cấp chiến minh tối thiểu kết đồng minh | 5 |
| `AlignNum` | Số đồng minh tối đa | 5 |
| `AlignCostMoney` | Phí kết đồng minh | 100,000 |
| `LegionsNeed` | Cấp chiến minh để tạo quân đoàn | 6 |
| `LegionsCastZuanShi` | Kim cương tạo quân đoàn | 2,000 |
| `LegionProsperityCost` | Phồn vinh: khởi đầu 100,000; trừ 3,000/ngày; cảnh báo 10,000; giải tán 0 |

---

## 10. Kết hôn / Hôn lễ

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `JieHunCost` | Chi phí kết hôn (kim cương) | 137 |
| `DivorceJinBiCost` | Ly hôn tự do (kim tệ) | 555,555 |
| `DivorceZuanShiCost` | Ly hôn cưỡng chế (kim cương) | 333 |
| `QiuHuiCD` | CD cầu hôn | 1800 giây (30') |
| `GoodWillXiShu` | Hệ số tăng thiện cảm | 0.1 |

---

## 11. Hệ thống Tinh Linh / Pets

| Param | Mô tả | Giá trị |
|-------|--------|---------|
| `CallPet` | Chi phí triệu hoán: 1 lần/10 lần | 200/1800 kim cương |
| `FreeCallPet` | Miễn phí mỗi | 36 giờ |
| `ConsumeCallPetJiFen` | Điểm tích lũy/1 kim cương | 0.1 |
| `PatSkillCostLingJing` | Tiêu Linh Tinh nắm bắt kỹ năng | 20→300 dần tăng |
| `PatSkillCostZuanShi` | Kim cương lock kỹ năng | 50 (1 kỹ năng), 200 (2 kỹ năng) |

---

## 12. Tham số nổi bật liên quan Event System

| Param | Vị trí | Liên quan |
|-------|--------|----------|
| `EveryDayActivityOpen` | Line 1457 | **Bật/tắt EverydayActivity** per platform |
| `EveryDayChongZhiDuiHuan` | Line 1455 | Tỷ giá nạp → EverydayAct JiFen |
| `JieRiChongZhiDuiHuan` | Line 1188 | Tỷ giá JieRi |
| `SpecialChongZhiDuiHuan` | Line 1278 | Tỷ giá SpecialActivity |
| `ZhouMoChongZhiTime` | Line 1194 | Thời gian nạp Weekend |
| `SuperChongZhiFanLiOpen` | Line 1401 | Siêu hoàn nạp bật theo platform |

---

## 13. Thống kê tổng quan

| Nhóm | Số tham số |
|------|-----------|
| Trang bị (Forge/ZhuiJia/ZhuanSheng) | ~35 |
| VIP System | ~20 |
| Nhiệm vụ hàng ngày | ~15 |
| PvP / Chiến trận | ~30 |
| Event Activities | ~8 |
| Economy (giá, tỷ giá) | ~20 |
| Anticheat / Server | ~25 |
| Chiến Minh / Quân đoàn | ~20 |
| Kết hôn / Xã hội | ~15 |
| Tinh linh / Pets | ~15 |
| Cơ sở hạ tầng (map, NPC...) | ~50 |
| Khác (Reborn, Escape...) | ~60 |
| **Tổng cộng** | **~313 tham số** |

---

## 14. Ghi chú quan trọng cho GameMU.Manager

> [!IMPORTANT]
> Một số tham số server-side **không thay đổi trực tiếp từ XML** mà được load vào RAM khi khởi động. Cần restart server hoặc reload config để có hiệu lực:
> - `ForgeGoodsRate`, `ForgeMaxOpen` → cần reload server
> - `EveryDayActivityOpen` → được đọc 1 lần khi `EverydayActivity.Init()`, cần restart GameServer

> [!NOTE]
> `EveryDayChongZhiDuiHuan` = `20000:1` nghĩa là: nạp 20,000 kim cương → 1 điểm EverydayAct JiFen. Đây là hệ số conversion trực tiếp với DB Cmd 13173.
