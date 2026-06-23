using System;
using System.Collections.Generic;
using System.IO;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleData : ICloneable
	{
		public object Clone()
		{
			RoleData result = null;
			using (MemoryStream memoryStream = new MemoryStream())
			{
				Serializer.Serialize<RoleData>(memoryStream, this);
				memoryStream.Position = 0L;
				result = Serializer.Deserialize<RoleData>(memoryStream);
			}
			return result;
		}

		public RoleData4Selector ExtractRoleData4Selector()
		{
			return new RoleData4Selector
			{
				RoleID = this.RoleID,
				RoleName = this.RoleName,
				RoleSex = this.RoleSex,
				Occupation = this.Occupation,
				Level = this.Level,
				Faction = this.Faction,
				OtherName = this.OtherName,
				GoodsDataList = this.GoodsDataList,
				MyWingData = this.MyWingData,
				SubOccupation = this.SubOccupation
			};
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int RoleSex;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int Level = 1;

		[ProtoMember(6)]
		public int Faction;

		[ProtoMember(7)]
		public int Money1;

		[ProtoMember(8)]
		public int Money2;

		[ProtoMember(9)]
		public long Experience;

		[ProtoMember(10)]
		public int PKMode;

		[ProtoMember(11)]
		public int PKValue;

		[ProtoMember(12)]
		public int MapCode;

		[ProtoMember(13)]
		public int PosX;

		[ProtoMember(14)]
		public int PosY;

		[ProtoMember(15)]
		public int RoleDirection;

		[ProtoMember(16)]
		public int LifeV;

		[ProtoMember(17)]
		public int MaxLifeV;

		[ProtoMember(18)]
		public int MagicV;

		[ProtoMember(19)]
		public int MaxMagicV;

		[ProtoMember(20)]
		public int RolePic;

		[ProtoMember(21)]
		public int BagNum;

		[ProtoMember(22)]
		public List<TaskData> TaskDataList;

		[ProtoMember(23)]
		public List<GoodsData> GoodsDataList;

		[ProtoMember(24)]
		public int BodyCode;

		[ProtoMember(25)]
		public int WeaponCode;

		[ProtoMember(26)]
		public List<SkillData> SkillDataList;

		[ProtoMember(27)]
		public string OtherName;

		[ProtoMember(28)]
		public List<NPCTaskState> NPCTaskStateList;

		[ProtoMember(29)]
		public string MainQuickBarKeys = string.Empty;

		[ProtoMember(30)]
		public string OtherQuickBarKeys = string.Empty;

		[ProtoMember(31)]
		public int LoginNum;

		[ProtoMember(32)]
		public int UserMoney;

		[ProtoMember(33)]
		public string StallName;

		[ProtoMember(34)]
		public int TeamID;

		[ProtoMember(35)]
		public int LeftFightSeconds;

		[ProtoMember(36)]
		public int TotalHorseCount;

		[ProtoMember(37)]
		public int HorseDbID = -1;

		[ProtoMember(38)]
		public int TotalPetCount;

		[ProtoMember(39)]
		public int PetDbID = -1;

		[ProtoMember(40)]
		public int InterPower;

		[ProtoMember(41)]
		public int TeamLeaderRoleID;

		[ProtoMember(42)]
		public int YinLiang;

		[ProtoMember(43)]
		public int JingMaiBodyLevel;

		[ProtoMember(44)]
		public int JingMaiXueWeiNum;

		[ProtoMember(45)]
		public int LastHorseID;

		[ProtoMember(46)]
		public int DefaultSkillID = -1;

		[ProtoMember(47)]
		public int AutoLifeV;

		[ProtoMember(48)]
		public int AutoMagicV;

		[ProtoMember(49)]
		public List<BufferData> BufferDataList;

		[ProtoMember(50)]
		public List<DailyTaskData> MyDailyTaskDataList;

		[ProtoMember(51)]
		public int JingMaiOkNum;

		[ProtoMember(52)]
		public DailyJingMaiData MyDailyJingMaiData;

		[ProtoMember(53)]
		public int NumSkillID;

		[ProtoMember(54)]
		public PortableBagData MyPortableBagData;

		[ProtoMember(55)]
		public int NewStep;

		[ProtoMember(56)]
		public long StepTime;

		[ProtoMember(57)]
		public int BigAwardID;

		[ProtoMember(58)]
		public int SongLiID;

		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList;

		[ProtoMember(60)]
		public int TotalLearnedSkillLevelCount;

		[ProtoMember(61)]
		public int CompletedMainTaskID;

		[ProtoMember(62)]
		public int PKPoint;

		[ProtoMember(63)]
		public int LianZhan;

		[ProtoMember(64)]
		public long StartPurpleNameTicks;

		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData;

		[ProtoMember(66)]
		public long BattleNameStart;

		[ProtoMember(67)]
		public int BattleNameIndex;

		[ProtoMember(68)]
		public int CZTaskID;

		[ProtoMember(69)]
		public int HeroIndex;

		[ProtoMember(70)]
		public int AllQualityIndex;

		[ProtoMember(71)]
		public int AllForgeLevelIndex;

		[ProtoMember(72)]
		public int AllJewelLevelIndex;

		[ProtoMember(73)]
		public int HalfYinLiangPeriod;

		[ProtoMember(74)]
		public int ZoneID;

		[ProtoMember(75)]
		public string BHName = string.Empty;

		[ProtoMember(76)]
		public int BHVerify;

		[ProtoMember(77)]
		public int BHZhiWu;

		[ProtoMember(78)]
		public int BangGong;

		[ProtoMember(79)]
		public Dictionary<int, BangHuiLingDiItemData> BangHuiLingDiItemsDict;

		[ProtoMember(80)]
		public int HuangDiRoleID;

		[ProtoMember(81)]
		public int HuangHou;

		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict;

		[ProtoMember(83)]
		public int AutoFightingProtect;

		[ProtoMember(84)]
		public long FSHuDunStart;

		[ProtoMember(85)]
		public int BattleWhichSide = -1;

		[ProtoMember(86)]
		public int LastMailID;

		[ProtoMember(87)]
		public int IsVIP;

		[ProtoMember(88)]
		public long OnceAwardFlag;

		[ProtoMember(89)]
		public int Gold;

		[ProtoMember(90)]
		public long DSHideStart;

		[ProtoMember(91)]
		public List<int> RoleCommonUseIntPamams = new List<int>();

		[ProtoMember(92)]
		public int FSHuDunSeconds;

		[ProtoMember(93)]
		public long ZhongDuStart;

		[ProtoMember(94)]
		public int ZhongDuSeconds;

		[ProtoMember(95)]
		public string KaiFuStartDay = string.Empty;

		[ProtoMember(96)]
		public string RegTime = string.Empty;

		[ProtoMember(97)]
		public string JieriStartDay = string.Empty;

		[ProtoMember(98)]
		public int JieriDaysNum;

		[ProtoMember(99)]
		public string HefuStartDay = string.Empty;

		[ProtoMember(100)]
		public int JieriChengHao;

		[ProtoMember(101)]
		public string BuChangStartDay = string.Empty;

		[ProtoMember(102)]
		public long DongJieStart;

		[ProtoMember(103)]
		public int DongJieSeconds;

		public int DongJieMills;

		[ProtoMember(104)]
		public string YueduDazhunpanStartDay = string.Empty;

		[ProtoMember(105)]
		public int YueduDazhunpanStartDayNum;

		[ProtoMember(106)]
		public int RoleStrength;

		[ProtoMember(107)]
		public int RoleIntelligence;

		[ProtoMember(108)]
		public int RoleDexterity;

		[ProtoMember(109)]
		public int RoleConstitution;

		[ProtoMember(110)]
		public int ChangeLifeCount;

		[ProtoMember(111)]
		public int TotalPropPoint;

		[ProtoMember(112)]
		public int IsFlashPlayer;

		[ProtoMember(113)]
		public int AdmiredCount;

		[ProtoMember(114)]
		public int CombatForce;

		[ProtoMember(115)]
		public int AdorationCount;

		[ProtoMember(116)]
		public int DayOnlineSecond;

		[ProtoMember(117)]
		public int SeriesLoginNum;

		[ProtoMember(118)]
		public int AutoAssignPropertyPoint;

		[ProtoMember(119)]
		public int OnLineTotalTime;

		[ProtoMember(120)]
		public int AllZhuoYueNum;

		[ProtoMember(121)]
		public int VIPLevel;

		[ProtoMember(122)]
		public int OpenGridTime;

		[ProtoMember(123)]
		public int OpenPortableGridTime;

		[ProtoMember(124)]
		public WingData MyWingData;

		[ProtoMember(125)]
		public Dictionary<int, int> PictureJudgeReferInfo;

		[ProtoMember(126)]
		public int StarSoulValue;

		[ProtoMember(127)]
		public long StoreYinLiang;

		[ProtoMember(128)]
		public long StoreMoney;

		[ProtoMember(129)]
		public string PlayerRecallStartDay = string.Empty;

		[ProtoMember(130)]
		public string PlayerRecallDaysNum = string.Empty;

		[ProtoMember(131)]
		public TalentData MyTalentData;

		[ProtoMember(132)]
		public int TianTiRongYao;

		[ProtoMember(133)]
		public FluorescentGemData FluorescentDiamondData;

		[ProtoMember(134)]
		public int GMAuth;

		[ProtoMember(135)]
		public SoulStoneData soulStoneData;

		[ProtoMember(136)]
		public long SettingBitFlags;

		[ProtoMember(137)]
		public int SpouseId;

		[ProtoMember(138)]
		public List<ActivityData> ActivityList;

		[ProtoMember(139)]
		public List<int> OccupationList;

		[ProtoMember(140)]
		public int JunTuanId;

		[ProtoMember(141)]
		public string JunTuanName;

		[ProtoMember(142)]
		public int JunTuanZhiWu;

		[ProtoMember(143)]
		public int LingDi;

		[ProtoMember(144)]
		public Dictionary<int, ShenJiFuWenData> ShenJiDict;

		[ProtoMember(145)]
		public RoleHuiJiData HuiJiData;

		[ProtoMember(146)]
		public List<FuWenTabData> FuWenTabList;

		[ProtoMember(147)]
		public int HideGM;

		[ProtoMember(148)]
		public JueXingShiData JueXingData;

		[ProtoMember(149)]
		public LongCollection MoneyData;

		[ProtoMember(150)]
		public int CompType;

		[ProtoMember(151)]
		public byte CompZhiWu;

		[ProtoMember(152)]
		public List<GoodsData> MountStoreList;

		[ProtoMember(153)]
		public List<GoodsData> MountEquipList;

		[ProtoMember(154)]
		public List<GoodsLimitData> GoodsLimitDataList;

		[ProtoMember(155)]
		public SystemOpenData OpenData;

		[ProtoMember(156)]
		public int ThemeState;

		[ProtoMember(157)]
		public int SubOccupation;

		[ProtoMember(158, IsRequired = true)]
		public RoleArmorData ArmorData;

		[ProtoMember(159)]
		public int CurrentArmorV;

		[ProtoMember(160)]
		public int MaxArmorV;

		[ProtoMember(161)]
		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;

		[ProtoMember(162)]
		public RoleBianShenData BianShenData;

		public ZuoQiMainData ZuoQiMainData;

		[ProtoMember(163)]
		public int RebornCombatForce;

		[ProtoMember(164)]
		public int RebornCount;

		[ProtoMember(165)]
		public int RebornLevel;

		[ProtoMember(166)]
		public long RebornExperience;

		[ProtoMember(167)]
		public List<GoodsData> RebornGoodsDataList;

		[ProtoMember(168)]
		public List<GoodsData> RebornGoodsStoreList;

		[ProtoMember(169)]
		public int RebornBagNum;

		[ProtoMember(170)]
		public RebornPortableBagData RebornGirdData;

		[ProtoMember(171)]
		public int OpenRebornGridTime;

		[ProtoMember(172)]
		public int OpenRebornPortableGridTime;

		[ProtoMember(173)]
		public int RebornShowEquip;

		[ProtoMember(174)]
		public RebornStampData RebornYinJi;

		[ProtoMember(175)]
		public int RebornShowModel;

		[ProtoMember(176)]
		public int ZhanDuiID;

		[ProtoMember(177)]
		public int ZhanDuiZhiWu;

		[ProtoMember(179)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData;

		[ProtoMember(180)]
		public Dictionary<int, MazingerStoreData> MazingerStoreDataInfo;

		[ProtoMember(250)]
		public int PTID;

		[ProtoMember(251)]
		public string WorldRoleID;

		[ProtoMember(252)]
		public string Channel;

		[ProtoMember(178)]
		public List<GoodsData> HolyGoodsDataList;
	}
}
