using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using UnityEngine;

namespace HSGameEngine.GameEngine.Data
{
	public class GData
	{
		public string UserID
		{
			get
			{
				return GameInstance.Game.CurrentSession.UserID;
			}
		}

		public string UserName
		{
			get
			{
				return GameInstance.Game.CurrentSession.UserName;
			}
		}

		public string UserToken
		{
			get
			{
				return GameInstance.Game.CurrentSession.UserToken;
			}
		}

		public int UserIsAdult
		{
			get
			{
				return GameInstance.Game.CurrentSession.UserIsAdult;
			}
		}

		public int RoleRandToken
		{
			get
			{
				return GameInstance.Game.CurrentSession.RoleRandToken;
			}
		}

		public int RoleID
		{
			get
			{
				return GameInstance.Game.CurrentSession.RoleID;
			}
		}

		public int RoleSex
		{
			get
			{
				return GameInstance.Game.CurrentSession.RoleSex;
			}
		}

		public string RoleName
		{
			get
			{
				return GameInstance.Game.CurrentSession.RoleName;
			}
		}

		public bool PlayGame
		{
			get
			{
				return GameInstance.Game.CurrentSession.PlayGame;
			}
		}

		public RoleData roleData
		{
			get
			{
				return GameInstance.ValidRoleData;
			}
		}

		public MarriageData MarryData
		{
			get
			{
				return GameInstance.Game.CurrentSession.MarriageData;
			}
		}

		public MarriageData_EX MarryOtherData
		{
			get
			{
				return GameInstance.Game.CurrentSession.OtherMarriageData;
			}
		}

		public string RolePathString
		{
			get
			{
				return GameInstance.Game.CurrentSession.RolePathString;
			}
		}

		public int GameServerID
		{
			get
			{
				int result = 1;
				if (GData.LockedServerID > 0)
				{
					return GData.LockedServerID;
				}
				int @int = PlayerPrefs.GetInt("NewLastServerInfoID");
				if (@int != 0)
				{
					result = @int;
				}
				return result;
			}
		}

		public string GamePingTaiID
		{
			get
			{
				return PlatformUserLogin.GamePingTaiID;
			}
		}

		public List<GoodsData> equipPet
		{
			get
			{
				return this.mEquipPet;
			}
			set
			{
				this.mEquipPet = value;
			}
		}

		public void AddPetToEquipPet(GoodsData pet)
		{
			if (pet != null)
			{
				if (this.mEquipPet == null)
				{
					this.mEquipPet = new List<GoodsData>();
				}
				byte b = 0;
				for (int i = 0; i < this.mEquipPet.Count; i++)
				{
					if (this.mEquipPet[i].Id == pet.Id)
					{
						this.mEquipPet[i] = pet;
						b = 1;
					}
				}
				if (b == 0)
				{
					this.mEquipPet.Add(pet);
				}
			}
		}

		public int BufferDataId
		{
			get
			{
				if (Global.Data.roleData.RoleCommonUseIntPamams.Count >= 40)
				{
					return Global.Data.roleData.RoleCommonUseIntPamams[40];
				}
				return -1;
			}
		}

		public int GameCursorImageID { get; set; }

		public GRadarMap GameRadarMap
		{
			get
			{
				return this._GameRadarMap;
			}
			set
			{
				this._GameRadarMap = value;
			}
		}

		public WorldNavigationPart GamePartMap
		{
			get
			{
				return this._GamePartMap;
			}
			set
			{
				this._GamePartMap = value;
			}
		}

		public JunTuanBangHuiMiniData RoleJunTuanMiniData
		{
			get
			{
				return this.m_RoleJunTuanMiniData;
			}
			set
			{
				this.m_RoleJunTuanMiniData = value;
			}
		}

		public GScene GameScene;

		public static int LockedServerID;

		public long OnLineTimeBagGrid;

		public long OnLineTimePortableGrid;

		public long OnLineTimeRebornBagGrid;

		public long OnLineTimeRebornPortableGrid;

		public XuanFuServerData ServerData;

		public int WalkUnitCost;

		public int RunUnitCost;

		public int[] SpeedTickList;

		public int MaxAttackSlotTick = 1400;

		public int WalkStepWidth;

		public int RunStepWidth;

		public int MaxAttackDistance;

		public int MinAttackDistance;

		public int MaxMagicDistance;

		public int MaxUnWatchDistance;

		public int LifeTotalWidth;

		public int HoldWidth;

		public int HoldHeight;

		public uint FactionBrushColor;

		public uint OtherNameBrushColor;

		public uint SnameBrushColor;

		public uint LifeBrushColor;

		public long[] LevelUpExperienceList;

		public Dictionary<int, double> LevelUpExpProportionList;

		public int ShowObstruction;

		public int ShowObsIndex;

		public int ShowCPUUsage;

		public int ShowFindWay;

		public int ShowPunish;

		public int ShowKeyPoints;

		public int AliveGoodsID = -1;

		public int AliveMaxLevel = 10;

		public int GoodsPackOvertimeTick = 90;

		public int GoodsDestroytimeTick = 90;

		public string WatchSprite;

		public string ViewSprite;

		public NPCData npcData;

		public int nAttackRoleID;

		public string strAttackName = string.Empty;

		public Dictionary<int, RoleData> OtherRoles = new Dictionary<int, RoleData>(500);

		public Dictionary<string, RoleData> OtherRolesByName = new Dictionary<string, RoleData>(500);

		public Dictionary<int, MonsterData> SystemMonsters = new Dictionary<int, MonsterData>(500);

		public Dictionary<int, BiaoCheData> BiaoChes = new Dictionary<int, BiaoCheData>(500);

		public Dictionary<int, JunQiData> JunQis = new Dictionary<int, JunQiData>(100);

		public Dictionary<int, FakeRoleData> FakeRoles = new Dictionary<int, FakeRoleData>(100);

		public int TargetNpcID = -1;

		public Dictionary<int, MapTeleports> MapTeleportsDict = new Dictionary<int, MapTeleports>();

		public List<MapStallItem> MapStallList = new List<MapStallItem>(10);

		public List<RoadItem> AutoRoadItemsList;

		public int AutoRoadOffset;

		public ExtActionTypes AutoRoadExtActionType;

		public int AutoRoadCancelNum;

		public LocalAutoFightData AutoFightData = new LocalAutoFightData();

		public long LastAddLifeTicks;

		public long LastAddMagicTicks;

		public int TempWaitingSkillID = -1;

		public long TempWaitingSkillIDTicks;

		public int[] JingMaiUpDecoIDs;

		public SystemSettingData SysSetting = new SystemSettingData();

		public ZhuanShengPromptingData ZSPSetting = new ZhuanShengPromptingData();

		public List<FriendData> FriendDataList;

		public List<OneKeyFindFriendData> FriendRecommendDataList;

		public int ExchangeID = -1;

		public ExchangeData ExchangeDataItem;

		public StallData StallDataItem;

		public StallData OtherStallDataItem;

		public TeamData CurrentTeamData;

		public List<DJRoomData> DJRoomDataList;

		public int RoleSalesWindowCount;

		public List<int> FirstNewGoodsIDList = new List<int>();

		public List<int> LastedNewTaskIDList = new List<int>();

		public List<int> LastedCompTaskIDList = new List<int>();

		public List<HorseData> HorsesDataList;

		public HorseData CurrentSelectedHorseData;

		public List<HorseData> OtherHorsesDataList;

		public HorseData CurrentOtherSelectedHorseData;

		public List<PetData> PetsDataList;

		public List<GoodsData> PortableGoodsDataList;

		public List<GoodsData> JinDanGoodsDataList;

		public List<GoodsData> YuansuGoodsDataList;

		public List<GoodsData> YuansuGoodsDataListByUseing;

		public List<GoodsData> JingLingGoodsDataList;

		public DJRoomData CurrentDJRoomData;

		public DJRoomRolesData CurrentDJRoomRolesData;

		public List<JingMaiData> JingMaiDataList;

		public List<JingMaiData> OtherJingMaiDataList;

		public int GMAuth;

		public List<BulletinMsgData> BulletinMsgDataList = new List<BulletinMsgData>();

		public List<GoodsData> SaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> OtherSaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> ViewGoodsPackDataList;

		public double[] CurrentRolePropFields;

		public GoodsData WaBaoGoodsData;

		public HuodongData MyHuoDongData;

		public RoleData HuangDiRoleData;

		public List<GoodsData> GiftsGoodsDataList = new List<GoodsData>();

		public List<GoodsData> EmailFujianGoodsDataList = new List<GoodsData>();

		public List<GoodsData> NpcSaleGoodsDataList = new List<GoodsData>();

		public List<GoodsData> BaoKuJiangLiGoodsDataList = new List<GoodsData>();

		public DailyActiveData DailyActiveInfor = new DailyActiveData();

		public Dictionary<int, MeditateData> MeditateInfoList = new Dictionary<int, MeditateData>();

		public ChengJiuData ChengJiuData;

		public AchievementRuneData ChengjiuFuWen;

		public PrestigeMedalData adendaData;

		public List<LangHunLingYuKingShowDataHist> langHunLingYuKingShowDataHist = new List<LangHunLingYuKingShowDataHist>();

		public int MoveMode = 1;

		public MallSaleData MallData;

		public JieriXmlData JieriData;

		public JieriXmlData ZhuTiFu;

		public bool IsDoingZaJinDan;

		public bool IsDoingYuanSuTilian;

		public bool IsDoingJingLingZhaoHuan;

		public ActivitiesData ActivitData;

		public bool IsArenaBattling;

		public bool IsYanHuangBattling;

		public bool DisableAutoRoad;

		public bool DisableRoleInfo;

		public JieriXmlData ZhuanXiangData;

		public JieriXmlData everyDayData;

		public float ScreenScaleX = (float)Screen.width / 960f;

		public float ScreenScaleY = (float)Screen.height / 540f;

		public bool WaitingForMapChange;

		public bool WaitingForSystemHelp;

		public bool ShowingGamePartMap;

		public int TaskMaxFocusCount = 4;

		public Dictionary<int, long> TalkDict = new Dictionary<int, long>();

		public string ReportStatURL = string.Empty;

		public Dictionary<int, BossData> BossInfoDict = new Dictionary<int, BossData>();

		public HashSet<int> ShiJieBossSet;

		public HashSet<int> HuangJinBossSet;

		public DateTime BossInfoLastRefreshTime = default(DateTime);

		public int AlivedBossCount;

		public int AlivedHuangJinBossCount;

		public Dictionary<int, List<CopyScoreDataInfo>> CopyScoreDataInfoList = new Dictionary<int, List<CopyScoreDataInfo>>();

		public Dictionary<int, int[]> fuBenNeedDict;

		public Dictionary<int, int[]> fuBenNeedLevelDict;

		private List<GoodsData> mEquipPet;

		public List<GoodsData> fashionAndTitleList;

		public int taLuoPaiLevel;

		public List<BufferData> teshuTitileBufferLst = new List<BufferData>();

		public Dictionary<int, Dictionary<int, EventCalendar>> HuoDongRiLiMap = new Dictionary<int, Dictionary<int, EventCalendar>>();

		public int teshuTitileTipOne;

		public List<GoodsData> DecorationList;

		private GRadarMap _GameRadarMap;

		private WorldNavigationPart _GamePartMap;

		public bool TriggerByCancel;

		public int MeditateState;

		public int MeditateSecs1;

		public int MeditateSecs2;

		public int MapCodeAtDeath;

		public CopyTeamData CurrentCopyTeamData;

		public bool IsTtsEnabled;

		public bool IsVoictToTextEnable;

		public Dictionary<int, int> HunYanJointTimes = new Dictionary<int, int>();

		public Dictionary<int, MarryPartyData> HunYanListDatas = new Dictionary<int, MarryPartyData>();

		public TalentData otherPlayerTalentData;

		public byte IsZhaoHuanShouLiving;

		public Dictionary<int, Color> RoleNameColor = new Dictionary<int, Color>();

		public int ShenLiJingHuaCount;

		private JunTuanBangHuiMiniData m_RoleJunTuanMiniData;

		public int CurrentJunTaiCaiJiType;

		public List<GoodsData> PaiZhuPetList;

		public List<HongBaoTipData> mZhanMengHongBaoTipsData = new List<HongBaoTipData>();

		public Dictionary<int, List<HongBaoTipData>> mSystemHongBaoTipsDataDict = new Dictionary<int, List<HongBaoTipData>>();

		public YaoSaiBossMainData mYaoSaiBossMainData;

		public Dictionary<int, MyHongBaoData> HongBaoFlagDict = new Dictionary<int, MyHongBaoData>();

		public Dictionary<int, HongBaoRankData> HongBaoRankDict = new Dictionary<int, HongBaoRankData>();

		public int mCurrentZhanMengMemberNum;

		public YaoSaiData yaosaiData;

		public bool mIsChanKanHongBao;

		public bool IsRecordVedio;

		public List<BangHuiMemberData> bangHuiMemberDataList = new List<BangHuiMemberData>();

		public EmblemCoolDownItem RoleEmblemData;

		public AlchemyData MyAlchemyData;

		public List<GoodsData> MyFuWenData = new List<GoodsData>();

		public List<FuWenTabData> MyFuWenTabData = new List<FuWenTabData>();

		public ShenShiMainData FuWenFanLiMainData;

		public int CoolDownSkillID;

		public double CoolDownSkillPercent;

		public int CurrentClickZhiXianTaskID;

		public int RoleUseHorseSkillId;

		public byte RoleFightState;
	}
}
