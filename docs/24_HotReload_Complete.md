# 24_HotReload_EverydayActivity_Complete.md
> ReloadXmlManager + EverydayActivity complete analysis
> Confidence: HIGH (source thực tế)
> Date: 2026-06-25

---

## 1. ReloadXmlManager.cs — Hot-Reload XML

**Path**: `GameServer/Logic/ReloadXmlManager.cs`  
**Chức năng**: Server reload XML config mà KHÔNG cần restart  
**Entry**: `ReloadXmlManager.ReloadXmlFile(xmlFileName)` — gọi từ GM tool/console

### 1.1 Tất cả XML có hot-reload (60 files)

| Nhóm | XML Files |
|------|----------|
| **DailyActive** | DailyActiveInfor.xml, DailyActiveAward.xml |
| **Gifts** | biggift.xml, loginnumgift.xml, huodongloginnumgift.xml, newrolegift.xml, comateffectivenessgift.xml, fanli.xml, onlietimegift.xml, uplevelgift.xml, vipdailyawards.xml, chongzhiking.xml, chongzhisong.xml, bossking.xml, levelking.xml, jingmaiking.xml, wuxueking.xml |
| **JieRi** | jierigifts/jirriqianggou.xml |
| **HeFu** | hefugifts/hefuqianggou.xml |
| **Mall** | mall.xml, qizhengegoods.xml, qianggou.xml |
| **System** | systemparams.xml, systemopen.xml, systemoperations.xml, vip.xml |
| **Items** | goods.xml, goodspack.xml, goodsmergeitems.xml, monstergoodslist.xml |
| **Activity** | activity/activitytip.xml, specialtimes.xml |
| **Gameplay** | battle.xml, battleaward.xml, arenabattle.xml, equipupgrade.xml, equipborn.xml, jingmai.xml, lucky.xml, rebirth.xml, dig.xml, wuxue.xml, chengjiu.xml, chengjiubuff.xml |
| **Others** | AssConfig.xml, bornname.xml, broadcastinfos.xml, npcsalelist.xml, npcscripts.xml, ipwhitelist.xml, platconfig.xml, popupwin.xml, taskzhangjie.xml, zuanhuang.xml, DaiBiShiYong.xml, chongzhi_yueyu.xml, AngelTempleAuctionAward.xml |

### 1.2 Hot-reload mechanism

```csharp
// Admin triggers reload:
int result = ReloadXmlManager.ReloadXmlFile("config/systemparams.xml");
// → Calls HuodongCachingMgr.ResetXxx() or GameManager.systemXxxMgr.LoadConfig()
// → Returns 0 = success, -1 = fail
```

### 1.3 HuodongCachingMgr.Reset* — 86 methods

Tất cả activity có thể reload live:

| Nhóm | Các Reset method |
|------|-----------------|
| **EverydayActivity** | ResetEverydayActivity() |
| **JieRi (Lễ hội)** | ResetJieRiDengLuActivity, ResetJieriCZSongActivity, ResetJieRiLeiJiCZActivity, ResetJieRiZiKaLiaBaoActivity, ResetJieRiXiaoFeiKingActivity, ResetJieRiCZKingActivity, ResetJieriDaLiBaoActivity, ResetJieriFuLiActivity, ResetJieriGiveActivity, ResetJieRiGiveKingActivity, ResetJieriRecvActivity, ResetJieriRecvKingActivity, ResetJieriPlatChargeKingActivity, ResetJieriPCKingActivityEveryDay, ResetJieriLianXuChargeActivity, ResetJieriVIPActivity, ResetJieriVIPYouHuiAct, ResetJieriIPointsExchangeActivity, ResetJieriActivityConfig, ResetJieRiFanLiAwardActivity, ResetJieRiCZQGActivity, ResetJieRiMeiRiLeiJiActivity, ResetJieriSuperInputFanLiActivity, ResetJieRiTotalConsumeActivity, ResetJieriMultAwardActivity |
| **HeFu (Hợp phục)** | ResetHeFuActivityConfig, ResetHeFuLoginActivity, ResetHeFuTotalLoginActivity, ResetHeFuRechargeActivity, ResetHeFuPKKingActivity, ResetHeFuAwardTimeActivity, ResetHeFuLuoLanActivity |
| **Theme** | ResetThemeActivityConfig, ResetThemeZhiGouActivity, ResetThemeDaLiBaoActivity, ResetThemeDuiHuanActivity |
| **Recharge/Special** | ResetTotalChargeActivity, ResetTotalConsumeActivity, ResetMeiRiChongZhiActivity, ResetDanBiChongZhiActivity, ResetChongJiHaoLiActivity, ResetOneDollarBuyActivity, ResetOneDollarChongZhiActivity, ResetInputFanLiActivity, ResetInputFanLiNewActivity, ResetInputKingActivity, ResetInputSongActivity, ResetXinFanLiActivity, ResetXinXiaoFeiKingActivity, ResetYueDuZhuanPanActivity, ResetWeedEndInputActivity |
| **VIP/Level** | ResetLevelKingActivity, ResetHorseKingActivity, ResetEquipKingActivity, ResetJingMaiKingActivity |
| **Regress** | ResetRegressActiveOpen, ResetRegressActiveSignGift, ResetRegressActiveTotalRecharge, ResetRegressActiveDayBuy, ResetRegressActiveStore |
| **SpecialActivity** | ResetSpecialActivity, ResetSpecPriorityActivity |
| **Gifts/Items** | ResetBigAwardItem, ResetCombatAwardItem, ResetEveryDayOnLineAwardItem, ResetFirstChongZhiGift, ResetLimitTimeLoginItem, ResetMOnlineTimeItem, ResetNewStepItem, ResetSeriesLoginItem, ResetSongLiItem, ResetUpLevelItem, ResetWLoginItem, ResetShenZhuangJiQiHuiKuiHaoLiActivity |

### 1.4 Ý nghĩa với GameMU.Manager

> **KEY INSIGHT**: `ReloadXmlManager` = bridge giữa GameMU.Manager và GameServer!
>
> Khi admin sửa XML qua GameMU.Manager → có thể trigger reload bằng cách:
> 1. HTTP call đến GameServer management endpoint
> 2. Write trigger file (server watch)
> 3. GM console command

→ **Phase 3**: GameMU.Manager cần tích hợp `POST /api/reload/{xmlPath}` trigger tới GameServer sau khi save XML.

---

## 2. HuodongCachingMgr — God Object

Từ ReloadXmlManager ta biết HuodongCachingMgr có **86+ Reset methods** → đây là central cache cho tất cả Activity config.

### 2.1 Pattern chuẩn của mỗi Activity

```csharp
// Load (startup):
static T GetXxxActivity() {
    if (_xxxActivity == null) {
        _xxxActivity = LoadFromXml(...);
    }
    return _xxxActivity;
}

// Reset (hot-reload):
static int ResetXxxActivity() {
    _xxxActivity = null;      // clear cache
    _xxxActivity = LoadFromXml(...);  // reload
    return 0; // success
}
```

### 2.2 Đề xuất refactor

Hiện tại: 86+ pairs Get/Reset methods = ~172 methods trong 1 class  
→ Nên split thành:
```csharp
// IActivityConfigCache<T>
interface IActivityConfigCache<T> {
    T Get();
    void Reset();
}

// Concrete per activity
class JieRiDengLuCache : IActivityConfigCache<JieRiDengLuActivity> { ... }
```

---

## 3. Tổng hợp flow GameMU.Manager → Server

```
Admin sửa XML qua GameMU.Manager
    ↓
XmlEventService.SaveRecord(key, record)
    ├── Auto backup (Config/_EventManager/backups/)
    └── Write XML (UTF-8 BOM, preserve order)

    ↓ (Phase 3 — chưa có)
POST /manage/reload?file=config/systemparams.xml
    ↓
TCPCmdHandler nhận manage cmd
    ↓
ReloadXmlManager.ReloadXmlFile("config/systemparams.xml")
    ↓
HuodongCachingMgr.ResetXxx()
    ↓
Config có hiệu lực ngay (không restart server)
```

---

## 4. Checklist cập nhật cuối

### Source code đã đọc ✅
- [x] DailyActiveManager.cs (31KB)
- [x] Activity.cs (18KB)
- [x] ActivityTypes.cs (enum)
- [x] DailyActiveTypes.cs
- [x] RechargeRepayActiveMgr.cs (48KB)
- [x] MallGoodsMgr.cs
- [x] GlobalServiceManager.cs
- [x] ServerEvents.cs
- [x] EventTypes.cs
- [x] ActivityManagerNew.cs
- [x] EverydayActivity.cs (48KB) ✅ MỚI
- [x] ReloadXmlManager.cs (36KB) ✅ MỚI

### XML đã đọc ✅
- [x] DailyActiveInfor.xml (5.5KB) — schema đầy đủ
- [x] JieRiType.xml — type mapping
- [x] HeFuType.xml — type mapping
- [x] JieRiDengLu.xml — schema đầy đủ
- [x] EventCalendar.xml — (empty raw fetch, got schema từ docs)

### Còn lại (medium priority)
- [ ] `GoodsPackManager.cs` (119KB) — reward engine (lớn nhất)
- [ ] `GiftCodeNewManager.cs` (10KB)
- [ ] Sample of `SystemParams.xml` (147KB)
- [ ] `GameMU.Manager/Services/EventRegistry.cs` (29KB) — danh sách 50+ XML

### GameMU.Manager phase tiếp theo
- [ ] REST API `/api/events/{key}` CRUD
- [ ] Integrate với ReloadXmlManager (trigger reload after save)
- [ ] GoodsData visual editor (7-field parser)
- [ ] Import/Export ZIP

---

## 5. Docs index — tổng 26 files (22 trong repo + 4 session mới)

| # | File | Mô tả | Size |
|---|------|-------|------|
| 00 | 00_Overview.md | Kiến trúc 3 project | 2.1KB |
| 01-18 | (đã có) | Các phân tích trước | - |
| 19 | **19_ActivitySystem_Analysis.md** | ActivityTypes enum đầy đủ ✨ | 15.2KB |
| 20 | **20_XMLSchema_Complete.md** | XML schema thực tế ✨ | 11.0KB |
| 21 | **21_CallGraph_DataModel_CRUD.md** | Call Graph + Data Model ✨ | 14.2KB |
| 22 | **22_MigrationPlan_Complete.md** | Kế hoạch migrate ✨ | 8.0KB |
| 23 | **23_EverydayActivity_GameMUManager.md** | EA deep + Manager state ✨ | 10.0KB |
| 24 | **24_HotReload_EverydayActivity_Complete.md** | Hot-reload + HuodongCachingMgr ✨ | (file này) |
