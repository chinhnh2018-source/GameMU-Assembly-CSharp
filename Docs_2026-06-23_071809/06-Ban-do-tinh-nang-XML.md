# 06 — Bản đồ Tính năng → File XML (quét từ mã nguồn GameServer)

Bảng dưới được sinh tự động bằng cách **quét toàn bộ mã nguồn `GameServer`** (thư mục `Logic/`, `Server/Data/`), đối chiếu các chuỗi `*.xml` được tham chiếu với file thật trong `GameRes/Config`. Tổng cộng **186 lớp quản lý (manager)** có dùng file cấu hình.

> Riêng `Program.cs` là bộ nạp gốc, tham chiếu ~89 file cấu hình lõi (Goods, Magics, Monsters, LevelUp, FuBen, SystemOpen, SystemParams...) nên không liệt kê từng dòng ở đây.

Trang **Bản đồ tính năng → XML** trong web app hiển thị đúng dữ liệu này, có tìm kiếm và liên kết tới các file đang được quản lý.

## Bang hội

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Giải đấu bang hội (League) | `Logic/BangHuiMatchManager.cs` | `LeagueBirthPoint.xml`, `LeagueMatch.xml`, `LeagueNewAward.xml`, `LeagueNewRandom.xml`, `LeagueOpen.xml`, `LeagueSuperAward.xml`, `LeagueSuperList.xml`, `LeagueSustain.xml`, `LeagueWar.xml` |
| Quân đoàn (Legion) | `Logic/JunTuanManager.cs` | `LegionTasks.xml`, `LegionsManager.xml`, `systemparams.xml` |
| Thần điện liên minh | `Logic/UnionPalace/UnionPalaceManager.cs` | `ShenDianExtra.xml`, `ShenDianLevelUp.xml`, `ShenDianScale.xml` |
| Đại chiến quân đoàn (Karen) | `Logic/KarenBattleManager.cs` | `LegionsWar.xml` |

## Bản đồ

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Bản đồ / Cổng dịch chuyển / Vùng an toàn | `Logic/GameMap.cs` | `MapConfig.xml` |

## Chuyển sinh

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Thánh ấn chuyển sinh | `Logic/Reborn/RebornStampConsts.cs` | `ShenShengYinJiZhu.xml`, `ShenShengYinJiZi.xml` |
| Trang bị chuyển sinh | `Logic/Reborn/RebornEquipConst.cs` | `EquipQuenching.xml`, `RebornEquip.xml`, `RebornEquipEvolution.xml`, `RebornSuperiorDrop.xml`, `RebornSuperiorType.xml` |

## Cánh

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Chú linh chú hồn (MU Wings) | `Logic/MUWings/ZhuLingZhuHunManager.cs` | `MaxWinZhuLing.xml`, `WinZhuLing.xml`, `ZhuLingType.xml` |

## Công thành

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Công thành chiến (LuoLan) | `Logic/LuoLanChengZhanManager.cs` | `QiZuoConfig.xml`, `SiegeWarfare.xml`, `SiegeWarfareBirthPoint.xml`, `SiegeWarfareEveryDayAward.xml`, `SiegeWarfareExp.xml` |
| Lãnh địa / Thành chiến | `Logic/LangHunLingYuManager.cs` | `City.xml`, `CityWar.xml`, `CityWarQiZuo.xml`, `SiegeWarfareBirthPoint.xml`, `SiegeWarfareExp.xml` |

## Hoạt động

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Hoạt động hằng ngày | `Logic/ActivityNew/EverydayActivity.cs` | `EveryDayActivity.xml`, `EveryDayActivityGroup.xml`, `EveryDayActivityType.xml` |
| Hoạt động đặc biệt (gói ưu đãi theo đợt) | `Logic/ActivityNew/SpecialActivity.cs` | `SpecialActivity.xml`, `SpecialActivityTime.xml` |
| Trung tâm nạp hoạt động/sự kiện (lễ hội, nạp, gộp server...) | `Logic/HuodongCachingMgr.cs` | `ChengJiuFanLi.xml`, `ChongZhiDuiHuan.xml`, `ChongZhiSong.xml`, `DaTianShiFanLi.xml`, `DayChongZhi.xml`, `HeFuDengLu.xml`, `HeFuFanLi.xml`, `HeFuLiBao.xml`, `HeFuLuoLan.xml`, `HeFuType.xml`, `HeFuZhangChang.xml`, `HuShenFuFanLi.xml`, `HunYinFanLi.xml`, `JieRiBaoXiang.xml`, `JieRiChongZhiKing.xml`, `JieRiDanBiChongZhi.xml`, `JieRiDayChongZhi.xml`, `JieRiDengLu.xml`, `JieRiDuoBei.xml`, `JieRiFuWenFanLi.xml`, `JieRiHuiJiFanLi.xml`, `JieRiLeiJi.xml`, `JieRiLeiJiXiaoFei.xml`, `JieRiLiBao.xml`, `JieRiMeiRiLeiJi.xml`, `JieRiVip.xml`, `JieRiXiaoFeiKing.xml`, `JunXianFanLi.xml`, `MuJieRiType.xml`, `NewDig2.xml`, `PKJiangLi.xml`, `QiangHuaFanLi.xml`, `ShenZhuangAward.xml`, `ThemeActivityOpen.xml`, `ThemeActivityType.xml`, `VIPFanLi.xml`, `WangChengJiangLi.xml`, `WingFanLi.xml`, `ZhuiJiaFanLi.xml` |

## Hằng ngày

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Hôm nay (JianFu) | `Logic/Today/TodayManager.cs` | `JianFu.xml` |

## Hệ thống

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Giftcode | `Logic/GiftCodeNewManager.cs` | `GiftCodeNew.xml` |
| Mở hệ thống theo phiên bản (IsOpen) | `Logic/VersionSystemOpenManager.cs` | `VersionSystemOpen.xml` |

## Hồi quy

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Hoạt động hồi quy - Mở | `Logic/RegressActiveOpen.cs` | `HuiGuiHuoDong.xml` |

## Khác

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| AlchemyManager | `Logic/AlchemyManager.cs` | `CurrencyConversion.xml` |
| AllThingsCalcItem | `Logic/AllThingsCalcItem.cs` | `QiangHuaFuJia.xml` |
| CGetOldResourceManager | `Logic/CGetOldResourceManager.cs` | `ZiYuanZhaoHui.xml` |
| ChuanQiQianHua | `Logic/ChuanQiQianHua.cs` | `QiangHua.xml` |
| CompBattleRuntimeData | `Logic/CompBattleRuntimeData.cs` | `ForceCraft.xml`, `ForceCraftBirth.xml`, `ForceCraftReward.xml`, `ForceStronghold.xml` |
| CompMineRuntimeData | `Logic/CompMineRuntimeData.cs` | `CompMineAward.xml`, `CompMineBirthPoint.xml`, `CompMineLink.xml`, `CompMineTruck.xml`, `CompMineWar.xml` |
| CompRuntimeData | `Logic/CompRuntimeData.cs` | `Comp.xml`, `CompLevel.xml`, `CompNotice.xml`, `CompResources.xml`, `CompSolder.xml`, `CompSolderSite.xml` |
| DanBiChongZhiActivity | `Logic/DanBiChongZhiActivity.cs` | `JieRiDanBiChongZhi.xml` |
| Data | `Logic/Data.cs` | `ChannelName.xml`, `ExtPropThreshold.xml`, `RebornLianZhan.xml`, `Settings.xml` |
| DeControl | `Logic/DeControl.cs` | `DeControl.xml` |
| EquipUpgradeCacheMgr | `Logic/EquipUpgradeCacheMgr.cs` | `EquipUpgrade.xml` |
| FaceBookManager | `Logic/FaceBookManager.cs` | `FacebookAward.xml` |
| FluorescentGemDefine | `Server/Data/FluorescentGemDefine.cs` | `GemDig.xml`, `GemDigType.xml`, `GemLevelup.xml` |
| GMCommands | `Logic/GMCommands.cs` | `TeQuanTiaoJian.xml` |
| Global | `Logic/Global.cs` | `EveryDayActivity.xml`, `Goods.xml`, `GoodsQuality.xml`, `HorseBaseProp.xml`, `HorseJiFen.xml`, `HorsePropLimit.xml`, `HorseUp.xml`, `JingMai.xml`, `LevelUp.xml`, `MuJieRiType.xml`, `QualityUp.xml`, `Settings.xml`, `SpecialActivity.xml`, `SpecialActivityTime.xml`, `TeQuanBoss.xml`, `TeQuanBuff.xml`, `TeQuanChouJiang.xml`, `TeQuanHongBao.xml`, `TeQuanJiHuo.xml`, `TeQuanJiangLi.xml`, `TeQuanShangCheng.xml`, `TeQuanTiaoJian.xml`, `TeQuanZhiGou.xml`, `ThemeActivityType.xml`, `VIP.xml`, `npcs.xml` |
| GoldAuctionConfigModel | `Logic/GoldAuction/GoldAuctionConfigModel.cs` | `AngelTempleAuctionAward.xml`, `Auction.xml` |
| GongGaoDataManager | `Logic/GongGaoDataManager.cs` | `Gonggao.xml` |
| GoodsPackManager | `Logic/GoodsPackManager.cs` | `EraDropLimit.xml` |
| GoodsReplaceManager | `Logic/Goods/GoodsReplaceManager.cs` | `ReplaceGoods.xml` |
| GoodsUtil | `Logic/GoodsUtil.cs` | `GetGoods.xml` |
| GuardStatueConst | `Logic/TuJian/GuardStatueConst.cs` | `JingPoShouHu.xml`, `ShouHuLevelUp.xml`, `ShouHuSuitUp.xml`, `TuJianShouHuType.xml` |
| HorseCachingManager | `Logic/HorseCachingManager.cs` | `HorseEnchance.xml` |
| InputFanLiNew | `Logic/ActivityNew/InputFanLiNew.cs` | `SanZhouNian_ChongZhiFanLi.xml` |
| JieRiCZQGActivity | `Logic/ActivityNew/JieRiCZQGActivity.cs` | `JieRiChongZhiQiangGou.xml` |
| JieRiChongZhiHongBaoActivity | `Logic/ActivityNew/JieRiChongZhiHongBaoActivity.cs` | `JieRiChongZhiHongBao.xml` |
| JieRiFuLiActivity | `Logic/ActivityNew/JieRiFuLiActivity.cs` | `JieRiFuLi.xml` |
| JieRiGiveKingActivity | `Logic/ActivityNew/JieRiGiveKingActivity.cs` | `JieRiZengSongKing.xml` |
| JieRiHongBaoActivity | `Logic/ActivityNew/JieRiHongBaoActivity.cs` | `JieRiQuanMinHongBao.xml` |
| JieRiRecvKingActivity | `Logic/ActivityNew/JieRiRecvKingActivity.cs` | `JieRiShouQuKing.xml` |
| JieriGiveActivity | `Logic/ActivityNew/JieriGiveActivity.cs` | `JieRiZengSong.xml` |
| JieriHongBaoKingActivity | `Logic/ActivityNew/JieriHongBaoKingActivity.cs` | `JieRiHongBaoBang.xml` |
| JieriIPointsExchgActivity | `Logic/ActivityNew/JieriIPointsExchgActivity.cs` | `ChongZhiDuiHuan.xml` |
| JieriLianXuChargeActivity | `Logic/ActivityNew/JieriLianXuChargeActivity.cs` | `JieRiLianXu.xml` |
| JieriPlatChargeKing | `Logic/ActivityNew/JieriPlatChargeKing.cs` | `PingTaiChongZhiKing.xml` |
| JieriPlatChargeKingEveryDay | `Logic/ActivityNew/JieriPlatChargeKingEveryDay.cs` | `JieRiMeiRiChongZhiWang.xml` |
| JieriRecvActivity | `Logic/ActivityNew/JieriRecvActivity.cs` | `JieRiShouQu.xml` |
| JieriSuperInputActivity | `Logic/ActivityNew/JieriSuperInputActivity.cs` | `MU_ChongZhiFanLi.xml` |
| JieriVIPYouHuiActivity | `Logic/ActivityNew/JieriVIPYouHuiActivity.cs` | `ChongZhiDuiHuan.xml`, `VIPYouHuiLiBao.xml` |
| JueXingConsts | `Server/Data/JueXingConsts.cs` | `AwakenActivation.xml`, `AwakenLevel.xml`, `AwakenRecovery.xml`, `AwakenSuit.xml` |
| JueXingManager | `Logic/JueXingManager.cs` | `SystemParams.xml` |
| KarenBattleManager_MapEast | `Logic/KarenBattleManager_MapEast.cs` | `LegionsEast.xml`, `LegionsEastBirthPoint.xml`, `LegionsEastFlag.xml` |
| KarenBattleManager_MapWest | `Logic/KarenBattleManager_MapWest.cs` | `LegionsWest.xml`, `LegionsWestBirthPoint.xml` |
| KuaFuMapManager | `Logic/KuaFuMapManager.cs` | `MapLine.xml` |
| LingDiCaiJiConsts | `Server/Data/LingDiCaiJiConsts.cs` | `ManorCollectMonster.xml` |
| MagicSwordManager | `Logic/MagicSword/MagicSwordManager.cs` | `SystemParams.xml` |
| MarriageOtherLogic | `Logic/MarriageOtherLogic.cs` | `GiveRose.xml`, `GoodWill.xml`, `WeddingRing.xml` |
| MarryPartyLogic | `Logic/MarryPartyLogic.cs` | `WeddingFeasttAward.xml` |
| MazingerStoreConst | `Logic/MazingerStoreConst.cs` | `MoShenMiBaoJie.xml`, `MoShenMiBaoXing.xml` |
| MerlinMagicBookDefine | `Server/Data/MerlinMagicBookDefine.cs` | `MagicBook.xml`, `MagicBookStar.xml`, `MagicWord.xml` |
| MoRiJudgeConsts | `Logic/MoRi/MoRiJudgeConsts.cs` | `MoRiShenPan.xml` |
| MonsterZone | `Logic/MonsterZone.cs` | `Monsters.xml` |
| MonsterZoneManager | `Logic/MonsterZoneManager.cs` | `Monsters.xml` |
| MountHolyStampConst | `Logic/MountHolyStampConst.cs` | `ShengYinShengJi.xml`, `ShengYinTaoZhuang.xml`, `ShengYinZhuoYue.xml` |
| NPCGeneralManager | `Logic/NPCGeneralManager.cs` | `npcs.xml` |
| NPCSaleList | `Logic/NPCSaleList.cs` | `NPCSaleList.xml` |
| OneDollarBuyActivity | `Logic/ActivityNew/OneDollarBuyActivity.cs` | `OneDollarBuy.xml` |
| OneDollarChongZhi | `Logic/ActivityNew/OneDollarChongZhi.cs` | `YiYuanChongZhi.xml` |
| PetSkillManager | `Logic/Goods/PetSkillManager.cs` | `PetSkill.xml`, `PetSkillLevelup.xml` |
| PlatConfig | `Logic/PlatConfig.cs` | `PlatConfig.xml` |
| PopupWinMgr | `Logic/PopupWinMgr.cs` | `PopupWin.xml` |
| PrestigeMedalManager | `Logic/JingJiChang/PrestigeMedalManager.cs` | `ShengWangSpecialAttribute.xml`, `ShengWangXunZhang.xml` |
| RebornDataConst | `Server/Data/RebornDataConst.cs` | `RebornBoss.xml`, `RebornBossAward.xml`, `RebornCombatForce.xml`, `RebornLevel.xml`, `RebornStage.xml` |
| RebornStoneConst | `Logic/RebornStoneConst.cs` | `ChongShengBaoShi.xml`, `XuanCaiHeCheng.xml`, `XuanCaiShuXing.xml`, `ZhuangBeiDaKong.xml` |
| RegressActiveDayBuy | `Logic/RegressActiveDayBuy.cs` | `HuiGuiDayZhiGou.xml` |
| RegressActiveSignGift | `Logic/RegressActiveSignGift.cs` | `HuiGuiLoginNumGift.xml` |
| RegressActiveStore | `Logic/RegressActiveStore.cs` | `HuiGuiStore.xml` |
| RegressActiveTotalRecharge | `Logic/RegressActiveTotalRecharge.cs` | `HuiGuiChongZhiGift.xml` |
| ReloadXmlManager | `Logic/ReloadXmlManager.cs` | `ActivityTip.xml`, `AngelTempleAuctionAward.xml`, `AssConfig.xml`, `AssInfo.xml`, `AssList.xml`, `Auction.xml`, `CaiDaXiao.xml`, `CaiShuZi.xml`, `ChengJiu.xml`, `ChengJiuBuff.xml`, `DaiBiShiYong.xml`, `DailyActiveAward.xml`, `DailyActiveInfor.xml`, `DuiHuanShangCheng.xml`, `JieRiDanBiChongZhi.xml`, `Lucky.xml`, `LuckyAward.xml`, `LuckyAward2.xml`, `VipDailyAwards.xml`, `activitytip.xml`, `arenabattle.xml`, `battle.xml`, `battleaward.xml`, `battleexp.xml`, `bornname.xml`, `chengjiu.xml`, `chengjiubuff.xml`, `chongzhi_andrid.xml`, `chongzhi_app.xml`, `chongzhi_yueyu.xml`, `chongzhisong.xml`, `dig.xml`, `equipborn.xml`, `equipupgrade.xml`, `goods.xml`, `goodspack.xml`, `hefuqianggou.xml`, `jingmai.xml`, `lucky.xml`, `luckyaward.xml`, `npcsalelist.xml`, `npcscripts.xml`, `platconfig.xml`, `popupwin.xml`, `qizhengegoods.xml`, `rebirth.xml`, `systemopen.xml`, `systemoperations.xml`, `systemparams.xml`, `vip.xml`, `vipdailyawards.xml`, `wuxue.xml`, `zuanhuang.xml` |
| RobotTaskValidator | `Logic/RobotTaskValidator.cs` | `AssConfig.xml`, `AssInfo.xml`, `AssList.xml` |
| RoleManager | `Logic/RoleManager.cs` | `Settings.xml` |
| SevenDayBuyAct | `Logic/ActivityNew/SevenDay/SevenDayBuyAct.cs` | `SevenDayQiangGou.xml` |
| SevenDayChargeAct | `Logic/ActivityNew/SevenDay/SevenDayChargeAct.cs` | `SevenDayChongZhi.xml` |
| SevenDayConsts | `Logic/ActivityNew/SevenDay/SevenDayConsts.cs` | `SevenDayChongZhi.xml`, `SevenDayGoal.xml`, `SevenDayLogin.xml`, `SevenDayQiangGou.xml` |
| SevenDayGoalAct | `Logic/ActivityNew/SevenDay/SevenDayGoalAct.cs` | `SevenDayGoal.xml` |
| SevenDayLoginAct | `Logic/ActivityNew/SevenDay/SevenDayLoginAct.cs` | `SevenDayLogin.xml` |
| ShenQiConsts | `Server/Data/ShenQiConsts.cs` | `Artifact.xml`, `God.xml`, `Toughness.xml` |
| ShenShiConsts | `Server/Data/ShenShiConsts.cs` | `FuWen.xml`, `FuWenGod.xml`, `FuWenHole.xml`, `FuWenPayRandom.xml`, `FuWenRandom.xml`, `HuoDongFuWenPayRandom.xml`, `HuoDongFuWenRandom.xml` |
| ShenShiManager | `Logic/ShenShiManager.cs` | `SystemParams.xml` |
| SoulStoneConsts | `Logic/FluorescentGem/SoulStoneConsts.cs` | `HunShi.xml`, `HunShiExp.xml`, `HunShiGoodsType.xml`, `HunShiGroup.xml`, `HunShiType.xml` |
| SpecPriorityActivity | `Logic/ActivityNew/SpecPriorityActivity.cs` | `TeQuanBoss.xml`, `TeQuanBuff.xml`, `TeQuanChouJiang.xml`, `TeQuanHongBao.xml`, `TeQuanJiHuo.xml`, `TeQuanJiangLi.xml`, `TeQuanShangCheng.xml`, `TeQuanTiaoJian.xml`, `TeQuanZhiGou.xml` |
| SummonerManager | `Logic/Summoner/SummonerManager.cs` | `SystemParams.xml` |
| SystemParamsList | `Logic/SystemParamsList.cs` | `SystemParams.xml` |
| ThemeActivityConfig | `Logic/ThemeActivityConfig.cs` | `ThemeActivityType.xml` |
| ThemeDaLiBaoActivity | `Logic/ActivityNew/ThemeDaLiBaoActivity.cs` | `ThemeActivityLiBao.xml` |
| ThemeDataConst | `Server/Data/ThemeDataConst.cs` | `ThemeActivityBOSS.xml`, `ThemeActivityMoYu.xml`, `ThemeActivityZhuanSheng.xml`, `ThemeActivityZhuanShengReward.xml` |
| ThemeDuiHuanActivity | `Logic/ActivityNew/ThemeDuiHuanActivity.cs` | `ThemeActivityDuiHuan.xml` |
| ThemeZhiGouActivity | `Logic/ActivityNew/ThemeZhiGouActivity.cs` | `OneDollarBuy.xml`, `ThemeActivityZhiGou.xml` |
| TimerBossManager | `Logic/RefreshIconState/TimerBossManager.cs` | `BossInfo.xml`, `HuangJin.xml` |
| TradeBlackManager | `Logic/CheatGuard/TradeBlackManager.cs` | `Blacklist.xml`, `TradeConfig.xml` |
| UserActivateManager | `Logic/UserActivate/UserActivateManager.cs` | `SystemParams.xml` |
| VideoLogic | `Logic/VideoLogic.cs` | `Viedo.xml` |
| WebOldPlayerManager | `Logic/WebOldPlayerManager.cs` | `WebOldPlayer.xml` |
| WuXingMapMgr | `Logic/WuXingMapMgr.cs` | `WuXing.xml`, `WuXingAwards.xml` |
| YaoSaiBossConsts | `Server/Data/YaoSaiBossConsts.cs` | `PetBoss.xml` |
| YaoSaiMissionConsts | `Server/Data/YaoSaiMissionConsts.cs` | `PetMission.xml` |
| YueKaManager | `Logic/YueKa/YueKaManager.cs` | `Card.xml` |
| ZhuanPanConsts | `Logic/ZhuanPan/ZhuanPanConsts.cs` | `ZhuanPan.xml`, `ZhuanPanAward.xml` |
| ZuoQiConsts | `Server/Data/ZuoQiConsts.cs` | `HorseAdvanced.xml`, `HorseArrayAddition.xml`, `HorseEquipAddition.xml`, `HorseFreeRandom.xml`, `HorseLevelUp.xml`, `HorsePayRandom.xml`, `HorsePokedex.xml`, `HorseRandom.xml`, `HorseSuit.xml`, `HorseSuperiorDrop.xml`, `HorseSuperiorType.xml`, `TeQuanHorseFreeRandom.xml`, `TeQuanHorsePayRandom.xml`, `TeQuanHorseRandom.xml` |
| ZuoQiManager | `Logic/ZuoQiManager.cs` | `SystemParams.xml` |

## Kết hôn

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Phụ bản phu thê (BOSS) | `Logic/MarryFuBenMgr.cs` | `ManAndWifeBoss.xml` |
| Đấu trường tình lữ | `Logic/Marriage/CoupleArena/CoupleAreanConsts.cs` | `CoupleBirthPoint.xml`, `CoupleBuff.xml`, `CoupleDuanWei.xml`, `CoupleWar.xml`, `CoupleWarAward.xml` |

## Liên minh

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Chiến trường liên minh / Cứ điểm | `Logic/CompBattleManager.cs` | `ForceCraft.xml`, `ForceCraftBirth.xml`, `ForceCraftReward.xml`, `ForceStronghold.xml` |
| Liên minh (Comp) | `Logic/CompManager.cs` | `Comp.xml`, `CompLevel.xml`, `CompNotice.xml`, `CompResources.xml`, `CompSolder.xml`, `CompSolderSite.xml` |
| Mỏ liên minh | `Logic/CompMineManager.cs` | `CompMineAward.xml`, `CompMineBirthPoint.xml`, `CompMineLink.xml`, `CompMineTruck.xml`, `CompMineWar.xml` |

## Liên server

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| BOSS liên server | `Logic/KuaFuBossManager.cs` | `ThroughServiceBoss.xml`, `ThroughServiceBossMonster.xml`, `ThroughServiceBossRebirth.xml` |
| Chiến trường dũng giả (xuyên server) | `Logic/YongZheZhanChangManager.cs` | `BattleCrystalMonster.xml`, `BattleMonster.xml`, `ThroughServiceBattle.xml`, `ThroughServiceBattleAward.xml`, `ThroughServiceRebirth.xml` |
| Thập tự quân / Cướp tài nguyên liên server | `Logic/KuaFuLueDuoManager.cs` | `CrusadeBirthPoint.xml`, `CrusadeCrystalMonster.xml`, `CrusadeGroup.xml`, `CrusadeQiZhi.xml`, `CrusadeStore.xml`, `CrusadeWar.xml` |
| Tranh đoạt lãnh thổ | `Logic/ZhengDuoManager.cs` | `PlunderLands.xml`, `PlunderLandsMonster.xml`, `PlunderLandsRebirth.xml` |

## Lãnh địa

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Thu thập lãnh địa | `Logic/LingDiCaiJiManager.cs` | `ShouWeiMonster.xml` |
| Xây dựng lãnh địa | `Logic/Building/BuildingManager.cs` | `Build.xml`, `BuildLevel.xml`, `BuildLevelAward.xml`, `BuildTask.xml` |
| Yêu tái - Trang viên | `Logic/YaoSaiJianYuManager.cs` | `ManorCommand.xml`, `ManorLevel.xml` |

## Ngoại trang

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Biến thân (Transfiguration) | `Logic/BianShenManager.cs` | `TransfigurationFashionEffect.xml`, `TransfigurationLevel.xml` |
| Thời trang / Cánh thời trang / Vũ khí ngoại trang | `Logic/FashionManager.cs` | `Fashion.xml`, `FashionTab.xml`, `FashionWings.xml`, `HorseFashion.xml`, `JiaoYinShiZhuangShengJi.xml`, `ShiZhuangLevelup.xml`, `SpecialTitle.xml`, `TransfigurationFashion.xml`, `WuQiShiZhuangShengJi.xml` |

## Nhân vật

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Chòm sao (XingZuo) | `Logic/StarConstellationManager.cs` | `XingZuoType.xml` |
| Thiên phú (Talent) | `Logic/Talent/TalentManager.cs` | `TianFuDian.xml`, `TianFuGroupProperty.xml` |
| Thành tựu | `Logic/ChengJiuManager.cs` | `ChengJiuFuWen.xml`, `ChengJiuSpecialAttribute.xml` |
| Tượng thủ hộ | `Logic/TuJian/GuardStatueManager.cs` | `JingPoShouHu.xml`, `ShouHuLevelUp.xml`, `ShouHuSuitUp.xml`, `TuJianShouHuType.xml` |

## Nạp/Quỹ

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Nạp tiền / Trực mua (ZhiGou) | `Logic/UserMoneyCharge/UserMoneyMgr.cs` | `MU_ChongZhi.xml`, `ZhiGou.xml` |
| Quỹ (Fund) | `Logic/FundManager.cs` | `Fund.xml`, `FundSet.xml` |

## Phụ bản

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Bản đồ phụ bản | `Logic/FuBenManager.cs` | `FuBenMap.xml` |
| Lang Hồn Yêu Tái | `Logic/CopyWolfManager.cs` | `LangHunYaoSai.xml` |
| Mục tiêu phụ bản | `Logic/CopyTargetManager.cs` | `FuBenMuBiao.xml` |
| Phụ bản Trận pháp LuoLan | `Logic/LuoLanFaZhenCopySceneManager.cs` | `LuoLanFaZhen.xml` |
| Phụ bản Ác ma Laixi | `Logic/EMoLaiXiCopySceneManager.cs` | `EMoLaiXi.xml`, `EMoLaiXiLuXian.xml`, `JinBiFuBen.xml` |
| Vạn ma hạp cốc | `Logic/WanMoXiaGuManager.cs` | `WanMoXiaGu.xml` |
| Ảo ảnh tự viện / Đền thiên thần | `Logic/HuanYingSiYuanManager.cs` | `ContinuityKillAward.xml`, `HolyGrail.xml`, `TempleMirage.xml`, `TempleMirageRebirth.xml` |

## Quái vật

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Thông tin quái vật | `Logic/MonsterStaticInfoMgr.cs` | `Monsters.xml` |

## Sự kiện

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Bài Tarot | `Logic/Tarot/TarotManager.cs` | `Tarot.xml` |
| Hoàn trả Mười | `Logic/TenRetutnManager.cs` | `TenRetutnAward.xml` |
| Hoạt động Mười (Ten) | `Logic/Ten/TenManager.cs` | `TenAward.xml` |
| Mini game (đoán số/lớn nhỏ) | `Logic/BocaiSys/BoCaiConfigMgr.cs` | `CaiDaXiao.xml`, `CaiShuZi.xml`, `DaiBiShiYong.xml`, `DuiHuanShangCheng.xml` |
| Nguyên tố thí luyện | `Logic/ElementWarManager.cs` | `YuanSuShiLian.xml` |
| Phán xét tận thế (MoRi) | `Logic/MoRi/MoRiJudgeManager.cs` | `MoRiShenPan.xml` |
| Tiệc khánh công | `Logic/QingGongYanManager.cs` | `GleeFeastAward.xml` |
| Triệu hồi thú (đào/triệu hồi) | `Logic/CallPetManager.cs` | `CallPet.xml`, `CallPetType.xml`, `FreeCallPet.xml`, `HuoDongCallPet.xml`, `SystemParams.xml`, `TeQuanCallPet.xml` |
| Truy tìm kho báu (One Piece) | `Logic/OnePiece/OnePieceManager.cs` | `TreasureBox.xml`, `TreasureEvent.xml`, `TreasureMap.xml` |
| Vòng quay may mắn | `Logic/ZhuanPan/ZhuanPanManager.cs` | `SystemParams.xml`, `ZhuanPan.xml`, `ZhuanPanAward.xml` |
| Đào càn khôn (New Dig) | `Logic/QianKunManager.cs` | `FreeNewDig.xml`, `HuoDongFreeNewDig.xml`, `HuoDongNewDig.xml`, `NewDig.xml`, `TeQuanFreeNewDig.xml`, `TeQuanNewDig.xml` |

## Sự kiện chiến trường

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Chiến trường Zork (sinh tồn) | `Logic/ZorkBattleManager.cs` | `ZorkAchievement.xml`, `ZorkActivityRules.xml`, `ZorkDanAward.xml`, `ZorkMonster.xml`, `ZorkPlayPoint.xml`, `ZorkScene.xml` |
| Chiến trường Đào thoát (Escape) | `Logic/EscapeBattleManager.cs` | `EscapeActivityRules.xml`, `EscapeDanList.xml`, `EscapeMapCollection.xml`, `EscapeMapSafeArea.xml`, `EscapePlayPoint.xml`, `EscapeRankAward.xml` |

## Sự kiện thi đấu

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Thế vận hội (đáp đề, đấu giá, mua nhanh) | `Logic/Olympics/OlympicsManager.cs` | `AoYunAward.xml`, `AoYunMatch.xml`, `AoYunQiangGou.xml`, `AoYunQuestion.xml` |
| Thế vận hội - Trả lời câu hỏi | `Logic/AoYunDaTi/AoyunDaTiConsts.cs` | `QuestionAward.xml`, `QuestionBank.xml`, `QuestionTime.xml` |
| Vua chiến trường (King Of Battle) | `Logic/KingOfBattleManager.cs` | `KingOfBattle.xml`, `KingOfBattleAward.xml`, `KingOfBattleCrystalMonster.xml`, `KingOfBattleMonster.xml`, `KingOfBattleQiZuo.xml`, `KingOfBattleRebirth.xml`, `KingOfBattleStore.xml` |

## Thú cưng

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Tinh linh khởi nguyên (Pet) | `Logic/JingLingQiYuanManager.cs` | `PetGroupProperty.xml`, `PetLevelAward.xml`, `PetSkillGroupProperty.xml`, `PetSkillLevelAward.xml`, `PetTianFuAward.xml` |
| Tinh linh nguyên tố giác tỉnh | `Logic/JingLingYuanSuJueXingManager.cs` | `JingLingYuanSu.xml`, `JingLingYuanSuShuXing.xml`, `SystemParams.xml` |
| Yêu tái - BOSS | `Logic/YaoSaiBossManager.cs` | `PetBoss.xml` |
| Yêu tái - Nhiệm vụ | `Logic/YaoSaiMissionManager.cs` | `PetMission.xml` |

## Thế giới

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Cấp thế giới / Lễ hội theo cấp | `Logic/WorldLevelManager.cs` | `JieRiLv.xml`, `JieRiLvType.xml` |
| Hệ thống Kỷ nguyên (Era): nhiệm vụ, drop, thưởng, cống hiến | `Logic/EraManager.cs` | `EraContribution.xml`, `EraDrop.xml`, `EraReward.xml`, `EraTask.xml`, `EraUI.xml` |

## Trang bị

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Huy hiệu (Emblem) | `Logic/HuiJiManager.cs` | `EmblemStar.xml`, `EmblemUp.xml` |
| Hồn thạch (HunShi) | `Logic/FluorescentGem/SoulStoneManager.cs` | `HunShi.xml`, `HunShiExp.xml`, `HunShiGoodsType.xml`, `HunShiGroup.xml`, `HunShiType.xml` |
| Linh ngọc (LingYu) | `Logic/LingYuManager.cs` | `LingYuLevelUp.xml`, `LingYuSuitUp.xml`, `LingYucollect.xml`, `LingyuType.xml` |
| Nguyên tố tim / Tinh luyện (Refine) | `Logic/ElementhrtsManager.cs` | `ElementsHeart.xml`, `Refine.xml`, `RefineType.xml` |
| Phụ ma trang bị (Enchant) | `Logic/FuMo/FuMoManager.cs` | `EquipEnchantmentExtra.xml`, `EquipEnchantmentRandom.xml` |
| Sách phép Merlin | `Logic/MerlinMagicBook/MerlinMagicBookManager.cs` | `MagicBook.xml`, `MagicBookStar.xml`, `MagicWord.xml`, `SystemParams.xml` |
| Thánh thuẫn (Shenshenghudun) | `Logic/ArmorManager.cs` | `ShenshenghudunJie.xml`, `ShenshenghudunXing.xml` |
| Thần khí / Tái tạo (ZaiZao) | `Logic/ArtifactManager.cs` | `TaoZhuangProps.xml`, `ZaiZao.xml` |
| Thần ký phù văn | `Logic/ShenJiFuWenManager.cs` | `ShenJiDian.xml`, `ShenJiFuWen.xml` |
| Trang sức (Ornament) | `Logic/Ornament/OrnamentManager.cs` | `Ornament.xml`, `OrnamentGroup.xml`, `OrnamentSite.xml` |
| Trang sức vũ khí | `Logic/WeaponAdornManager.cs` | `WeaponAdorn.xml` |
| Tẩy luyện thuộc tính | `Logic/WashPropsManager.cs` | `XiLianShuXing.xml`, `XiLianType.xml` |
| Vũ khí đại sư | `Logic/WeaponMaster.cs` | `WeaponMaster.xml` |
| Vật phẩm thánh (BuJian/ShengWu) | `Logic/HolyItemManager.cs` | `BuJian.xml`, `ShengWu.xml` |
| Đào ngọc huỳnh quang | `Logic/FluorescentGem/FluorescentGemManager.cs` | `GemDig.xml`, `GemDigType.xml`, `GemLevelup.xml`, `SystemParams.xml` |

## Đấu xếp hạng

| Tính năng | Lớp quản lý | File XML |
|---|---|---|
| Thiên thê (đấu xếp hạng đơn) | `Logic/TianTiManager.cs` | `DuanWei.xml`, `DuanWeiRankAward.xml`, `TianTi.xml`, `TianTiBirthPoint.xml` |
| Thiên thê 5v5 (đội) | `Logic/TianTi5v5Manager.cs` | `TeamBattle.xml`, `TeamBattleBirthPoint.xml`, `TeamDuanWei.xml`, `TeamDuanWeiAward.xml` |
| Tranh bá (Match) | `Logic/ZhengBaManager.cs` | `Match.xml`, `MatchAward.xml`, `MatchBirthPoint.xml`, `Sustain.xml` |
| Tranh bá theo đội | `Logic/ZhanDuiZhengBaManager.cs` | `TeamMatch.xml`, `TeamMatchAward.xml`, `TeamMatchBirthPoint.xml` |
| Đấu trường / Quân hàm | `Logic/JingJiChang/JingJiChangManager.cs` | `JingJi.xml`, `JunXian.xml` |
