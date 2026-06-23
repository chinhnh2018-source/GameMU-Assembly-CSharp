using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleDataEx
	{
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
		public int MagicV;

		[ProtoMember(18)]
		public List<OldTaskData> OldTasks;

		[ProtoMember(19)]
		public int RolePic;

		[ProtoMember(20)]
		public int BagNum;

		[ProtoMember(21)]
		public List<TaskData> TaskDataList;

		[ProtoMember(22)]
		public List<GoodsData> GoodsDataList;

		[ProtoMember(23)]
		public string OtherName = string.Empty;

		[ProtoMember(24)]
		public string MainQuickBarKeys = string.Empty;

		[ProtoMember(25)]
		public string OtherQuickBarKeys = string.Empty;

		[ProtoMember(26)]
		public int LoginNum;

		[ProtoMember(27)]
		public int UserMoney;

		[ProtoMember(28)]
		public int LeftFightSeconds;

		[ProtoMember(29)]
		public List<FriendData> FriendDataList;

		[ProtoMember(30)]
		public List<HorseData> HorsesDataList;

		[ProtoMember(31)]
		public int HorseDbID;

		[ProtoMember(32)]
		public List<PetData> PetsDataList;

		[ProtoMember(33)]
		public int PetDbID;

		[ProtoMember(34)]
		public int InterPower;

		[ProtoMember(35)]
		public List<JingMaiData> JingMaiDataList;

		[ProtoMember(36)]
		public int DJPoint;

		[ProtoMember(37)]
		public int DJTotal;

		[ProtoMember(38)]
		public int DJWincnt;

		[ProtoMember(39)]
		public int TotalOnlineSecs;

		[ProtoMember(40)]
		public int AntiAddictionSecs;

		[ProtoMember(41)]
		public long LastOfflineTime;

		[ProtoMember(42)]
		public long BiGuanTime;

		[ProtoMember(43)]
		public int YinLiang;

		[ProtoMember(44)]
		public List<SkillData> SkillDataList;

		[ProtoMember(45)]
		public int TotalJingMaiExp;

		[ProtoMember(46)]
		public int JingMaiExpNum;

		[ProtoMember(47)]
		public long RegTime;

		[ProtoMember(48)]
		public int LastHorseID;

		[ProtoMember(49)]
		public List<GoodsData> SaleGoodsDataList;

		[ProtoMember(50)]
		public int DefaultSkillID = -1;

		[ProtoMember(51)]
		public int AutoLifeV;

		[ProtoMember(52)]
		public int AutoMagicV;

		[ProtoMember(53)]
		public List<BufferData> BufferDataList;

		[ProtoMember(54)]
		public List<DailyTaskData> MyDailyTaskDataList;

		[ProtoMember(55)]
		public DailyJingMaiData MyDailyJingMaiData;

		[ProtoMember(56)]
		public int NumSkillID;

		[ProtoMember(57)]
		public PortableBagData MyPortableBagData;

		[ProtoMember(58)]
		public HuodongData MyHuodongData;

		[ProtoMember(59)]
		public List<FuBenData> FuBenDataList;

		[ProtoMember(60)]
		public int MainTaskID;

		[ProtoMember(61)]
		public int PKPoint;

		[ProtoMember(62)]
		public int LianZhan;

		[ProtoMember(63)]
		public RoleDailyData MyRoleDailyData;

		[ProtoMember(64)]
		public int KillBoss;

		[ProtoMember(65)]
		public YaBiaoData MyYaBiaoData;

		[ProtoMember(66)]
		public long BattleNameStart;

		[ProtoMember(67)]
		public int BattleNameIndex;

		[ProtoMember(68)]
		public int CZTaskID;

		[ProtoMember(69)]
		public int BattleNum;

		[ProtoMember(70)]
		public int HeroIndex;

		[ProtoMember(71)]
		public int ZoneID;

		[ProtoMember(72)]
		public string BHName = string.Empty;

		[ProtoMember(73)]
		public int BHVerify;

		[ProtoMember(74)]
		public int BHZhiWu;

		[ProtoMember(75)]
		public int BGDayID1;

		[ProtoMember(76)]
		public int BGMoney;

		[ProtoMember(77)]
		public int BGDayID2;

		[ProtoMember(78)]
		public int BGGoods;

		[ProtoMember(80)]
		public int BangGong;

		[ProtoMember(81)]
		public int HuangHou;

		[ProtoMember(82)]
		public Dictionary<int, int> PaiHangPosDict;

		[ProtoMember(83)]
		public int JieBiaoDayID;

		[ProtoMember(84)]
		public int JieBiaoDayNum;

		[ProtoMember(85)]
		public int LastMailID;

		[ProtoMember(86)]
		public List<VipDailyData> VipDailyDataList;

		[ProtoMember(87)]
		public YangGongBKDailyJiFenData YangGongBKDailyJiFen;

		[ProtoMember(88)]
		public long OnceAwardFlag;

		[ProtoMember(89)]
		public int Gold;

		[ProtoMember(90)]
		public List<GoodsLimitData> GoodsLimitDataList;

		[ProtoMember(91)]
		public Dictionary<string, RoleParamsData> RoleParamsDict;

		[ProtoMember(92)]
		public int BanChat;

		[ProtoMember(93)]
		public int BanLogin;

		[ProtoMember(94)]
		public int IsFlashPlayer;

		[ProtoMember(95)]
		public int ChangeLifeCount;

		[ProtoMember(96)]
		public int AdmiredCount;

		[ProtoMember(97)]
		public int CombatForce;

		[ProtoMember(98)]
		public int AutoAssignPropertyPoint;

		[ProtoMember(99)]
		public string PushMessageID = string.Empty;

		[ProtoMember(100)]
		public WingData MyWingData;

		[ProtoMember(101)]
		public Dictionary<int, int> RolePictureJudgeReferInfo;

		[ProtoMember(102)]
		public Dictionary<int, int> RoleStarConstellationInfo;

		[ProtoMember(103)]
		public int VIPLevel;

		[ProtoMember(104)]
		public List<GoodsData> ElementhrtsList;

		[ProtoMember(105)]
		public List<GoodsData> UsingElementhrtsList;

		[ProtoMember(106)]
		public List<GoodsData> PetList;

		[ProtoMember(107)]
		public long Store_Yinliang;

		[ProtoMember(108)]
		public long Store_Money;

		[ProtoMember(109)]
		public Dictionary<int, LingYuData> LingYuDict;

		[ProtoMember(110)]
		public MarriageData MyMarriageData;

		[ProtoMember(111)]
		public Dictionary<int, int> MyMarryPartyJoinList;

		[ProtoMember(146)]
		public int CompType;

		[ProtoMember(147)]
		public byte CompZhiWu;

		[ProtoMember(148)]
		public List<GoodsData> MountStoreList;

		[ProtoMember(149)]
		public List<GoodsData> MountEquipList;

		[ProtoMember(150)]
		public int SubOccupation;

		[ProtoMember(151, IsRequired = true)]
		public RoleArmorData ArmorData;

		[ProtoMember(154)]
		public List<GoodsData> RebornGoodsDataList;

		[ProtoMember(156)]
		public int RebornCombatForce;

		[ProtoMember(157)]
		public int RebornCount;

		[ProtoMember(158)]
		public int RebornLevel;

		[ProtoMember(159)]
		public long RebornExperience;

		[ProtoMember(160)]
		public List<GoodsData> RebornGoodsStoreList;

		[ProtoMember(161)]
		public int RebornBagNum;

		[ProtoMember(162)]
		public RebornPortableBagData RebornGirdData;

		[ProtoMember(163)]
		public int RebornShowEquip;

		[ProtoMember(170)]
		public Dictionary<int, RebornEquipData> RebornEquipHoleData;

		[ProtoMember(250)]
		public int PTID;

		[ProtoMember(251)]
		public string WorldRoleID;

		[ProtoMember(252)]
		public string Channel;
	}
}
