# 02 — Danh mục file XML sự kiện (lược đồ)

Thư mục gốc: `GameRes/GameRes/Config`. Bảng dưới liệt kê các file sự kiện chính,
the gốc (root), the lặp (item), thuộc tính định danh và số bản ghi mẫu.

## Lịch sự kiện
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `EventCalendar.xml` | `Config → EventCalendar` (67) | `ID` | ID, Weekday, Level, CompletedTaskID, VipLevel, Time, LinkID, EventName, EventAward |

## Hoạt động đặc biệt
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `SpecialActivity/SpecialActivity.xml` | `SpecialActivity → Activity` (185) | `ID` | ID, GroupID, Name, Day, NeedLevel, NeedVIP, NeedChongZhi, NeedWing, NeedChengJiu, NeedJunXian, NeedMerlin, NeedShengWu, NeedRing, NeedShouHuShen, Type, Goal, GoodsOne/Two/Thr, EffectiveTime, Price, PurchaseNum |
| `SpecialActivity/SpecialActivityTime.xml` | `SpecialActivityTime → Time` (144) | `GroupID` | GroupID, ServerOpenFromDate, ServerOpenToDate, FromDate, ToDate |

## Hoạt động hằng ngày
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `EveryDayActivity/EveryDayActivity.xml` | `Config → EveryDayActivity` (263) | `ActivityID` | ActivityID, Name, GoalType, GoalNum, GoodsOne/Two/Thr, EffectiveTime, Price, PurchaseNum |
| `EveryDayActivity/EveryDayActivityGroup.xml` | `Config → EveryDayActivityGroup` (92) | `GroupID` | GroupID, TypeID, Name, NeedType, NeedNum, ActivityID |
| `EveryDayActivity/EveryDayActivityType.xml` | `Config → EveryDayActivityType` (9) | `TypeID` | TypeID, Name, OpenLevel, CloseLevel |

## Mở hệ thống
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `VersionSystemOpen.xml` | `Config → Version` (50) | `ID` | ID, SystemName, **IsOpen** |
| `SystemOpen.xml` | `Config → System` (134) | `ID` | Order, ID, Cartoon, Name, occupation, TriggerCondition, TimeParameters, PostWizardID, PostTaskPlotID, SpecialOpenType, ImageOne, ImageTwo, Description, Music, NotOpenShow, DongHua |

## Sự kiện chủ đề (Theme)
| File | root → item | ID | Thuộc tính chính |
|---|---|---|---|
| `ThemeActivityOpen.xml` | `Config → ThemeActivityOpen` (1) | `ID` | ID, **Open**, Logo, HuanYing, Loading, Title |
| `ThemeActivityType.xml` | `Config → ThemeActivityType` (5) | `ID` | ID, Type, Name, PeiZhi, EndData |
| `ThemeActivityZhiGou.xml` | `Config → ThemeActivityZhiGou` (96) | `ID` | ID, Day, ZhiGouID, ChongZhiID, GoodsOne/Two, SinglePurchase, TitlePic, Background |
| `ThemeActivityBOSS.xml` | `Config → ThemeActivityBOSS` (9) | `ID` | ID, MonstersID, MaxLevel, ZhanLi, GoodsList, Scale, MapCode, X, Y, Radius, Num, TimePoints |
| `ThemeActivityZhuanSheng.xml` | `Config → ThemeActivityZhuanSheng` (4) | `ID` | ID, MonstersID, MapID, X, Y, MinLevel, MaxLevel, ZhanLi, GoodsList, Scale, TimePoints, ReadyTime, FightSecs, ClearRolesSecs, MaxEnterNum |

## Quà lễ hội (JieRiGifts) — điều khiển bằng khung ngày
Hầu hết các file có dạng `Config → Activities` (1 bản ghi) với thuộc tính:
`ActivityType, FromDate, ToDate, AwardStartDate, AwardEndDate`.

Danh sách: `JieRiBOSS, JieRiBaoXiang, JieRiChongZhiFanLi, JieRiChongZhiHongBao, JieRiChongZhiKing,
JieRiChongZhiQiangGou, JieRiChongZhiSong, JieRiDanBiChongZhi, JieRiDayChongZhi, JieRiDayXiaoFei,
JieRiDengLu, JieRiDuoBei, JieRiFuLi, JieRiFuWenFanLi, JieRiHongBaoBang, JieRiHuiJiFanLi, JieRiLeiJi,
JieRiLeiJiXiaoFei, JieRiLiBao, JieRiLianXu, JieRiMeiRiChongZhiWang, JieRiMeiRiLeiJi, JieRiQuanMinHongBao,
JieRiShouQu, JieRiShouQuKing, JieRiVip, JieRiXiaoFeiKing, JieRiZengSong, JieRiZengSongKing`.

Ngoài ra: `JieRiType.xml` (Config → Type), `MuJieRiType.xml` (Config → Type), `JieRiQiangGou.xml`
(Xiangou → Goods), `JieRiLv.xml`…

## Hồi quy / Gộp server
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `HuiGuiHuoDong.xml` | `Config → HuiGuiHuoDong` (3) | `ID` | ID, HuoDongLevel, **BeginTime, FinishTime**, RegisterBegin, RegisterFinish |
| `HeFuGifts/HeFuBOSS.xml` | `Config → Activities` (1) | `ActivityType` | ActivityType, FromDate, ToDate, AwardStartDate, AwardEndDate |
| `HeFuGifts/HeFu*.xml` | `Config → Activities` | `ActivityType` | (một số chỉ có ActivityType) |
| `HeFuGifts/HeFuQiangGou.xml` | `Xiangou → Goods` (20) | `ID` | Group, Random, ID, GoodsID, OrigPrice, Price, SinglePurchase, FullPurchase, DaysTime |

## Sự kiện 7 ngày
| File | root → item | ID | Thuộc tính |
|---|---|---|---|
| `SevenDay/SevenDayGoal.xml` | `config → Goal` (191) | `ID` | ID, Day, GoalType, FunctionType, Describe, TypeGoal, Award, ShowNum |
| `SevenDay/SevenDayActivityType.xml` | `Config → ActivityType` (4) | `ActivityType` | ActivityType, Name, Tiptype, XML |
| `SevenDay/SevenDayQiangGou.xml` | `Xiangou → Goods` (9) | `ID` | ID, Day, Name, GoodsID, OrigPrice, Price, Purchase |

## Tab hoạt động
| File | root → item | Thuộc tính |
|---|---|---|
| `HuoDongTab.xml` | `HuoDongList → HuoDong` (7) | ID, Name, Preview, RewardExplain, GLXml |
| `KuaFuHuoDongTab.xml` | `HuoDongList → HuoDong` (10) | ID, Name, Preview, RewardExplain, GLXml |
| `ZhanDuiHuoDongTab.xml` | `HuoDongList → HuoDong` (4) | ID, Name, Preview, RewardExplain, GLXml |
| `ZhanMengHuoDongTab.xml` | `HuoDongList → HuoDong` (5) | ID, Name |

## Hoạt động khác (Activity/)
| File | root → item | Thuộc tính |
|---|---|---|
| `Activity/Copy.xml` | `Copys → Copy` (28) | Type, ID, Name, Level, MaxLevel, Information, PublishID, Description1/2 |
| `Activity/BossInfo.xml` | `BossList → Boss` (20) | Type, ID, Level, Description, NpcID, ZhanLi, GoodsList, Scale, Show |
| `Activity/ActivityTip.xml` | `ActivityList → Tip` (22) | ID, Name, MinZhuanSheng, MinLevel, Task, WeekDays, TimeType, StartDay, ShowType, ShowTimes, ToMapID, ToX, ToY, List, Hint, Intro, OpenDay |
