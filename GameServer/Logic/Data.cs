using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public static class Data
	{
		public static int GetTotalLoginInfoNum()
		{
			int count;
			lock (Data.TotalLoginDataInfoListLock)
			{
				count = Data.TotalLoginDataInfoList.Count;
			}
			return count;
		}

		public static TotalLoginDataInfo GetTotalLoginDataInfo(int nIdnex)
		{
			lock (Data.TotalLoginDataInfoListLock)
			{
				if (Data.TotalLoginDataInfoList.ContainsKey(nIdnex))
				{
					return Data.TotalLoginDataInfoList[nIdnex];
				}
			}
			return null;
		}

		public static SingleChargeData ChargeData
		{
			get
			{
				SingleChargeData chargeData;
				lock (Data.SingleChargeDataMutex)
				{
					chargeData = Data._ChargeData;
				}
				return chargeData;
			}
			set
			{
				lock (Data.SingleChargeDataMutex)
				{
					Data._ChargeData = value;
				}
			}
		}

		public static Dictionary<int, ChargeItemData> ChargeItemDict
		{
			get
			{
				Dictionary<int, ChargeItemData> chargeItemDict;
				lock (Data.ChargeItemDataMutex)
				{
					chargeItemDict = Data._ChargeItemDict;
				}
				return chargeItemDict;
			}
			set
			{
				lock (Data.ChargeItemDataMutex)
				{
					Data._ChargeItemDict = value;
				}
			}
		}

		public static List<ChannelName> ChannelNameConfigList
		{
			get
			{
				return Data._ChannelNameConfigList.Value;
			}
		}

		public static List<LianZhanConfig> LianZhanConfigList
		{
			get
			{
				return Data._LianZhanConfigList.Value;
			}
		}

		public static void LoadConfig()
		{
			try
			{
				double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("ZhanDouLiAlertLog", ',');
				if (paramValueDoubleArrayByName != null && paramValueDoubleArrayByName.Length == 2 && paramValueDoubleArrayByName[0] >= 100000.0 && paramValueDoubleArrayByName[1] >= 0.1)
				{
					Data.CombatForceLogMinValue = (long)paramValueDoubleArrayByName[0];
					Data.CombatForceLogPercent = paramValueDoubleArrayByName[1];
				}
				Data.LoadUseridWhiteList();
				Data.LoadSystemParamsValues();
				Data.SettingsDict.Load(Global.GameResPath("Config/Settings.xml"), "Maps");
				int num = (int)GameManager.systemParamsList.GetParamValueIntByName("LogWithRname", -1);
				Global.WithRname = (num == 1);
				Data.ZhuTiID = GameManager.GameConfigMgr.GetGameConfigItemInt("zhuti", 0);
				Data.FightStateTime = (long)((int)GameManager.systemParamsList.GetParamValueIntByName("FightStateTime", 6000));
				Data.CheckTimeBoost = (GameManager.systemParamsList.GetParamValueIntByName("CheckTimeBoost", 1) > 0L);
				Data.CheckPositionCheat = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 1L) != 0L);
				Data.CheckPositionCheatSpeed = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 2L) != 0L);
				Data.IgnoreClientPos = ((GameManager.systemParamsList.GetParamValueIntByName("CheckPositionCheat", 1) & 4L) != 0L);
				LogManager.DisableLogTypes(GameManager.systemParamsList.GetParamValueIntArrayByName("DisableLogTypes", ','));
				Data.SyncTimeByClient = GameManager.systemParamsList.GetParamValueIntByName("SyncTimeByClient", 750);
				Data.MaxServerClientTimeDiff = GameManager.systemParamsList.GetParamValueIntByName("MaxServerClientTimeDiff", 300);
				Data.RoleOccupationMaxCount = (int)GameManager.systemParamsList.GetParamValueIntByName("PurchaseOccupationNum", 1);
				Data.OpChangeLifeCount = (int)GameManager.systemParamsList.GetParamValueIntByName("OpChangeLifeCount", 100);
				Data.NotifyLiXianAwardMin = (int)GameManager.systemParamsList.GetParamValueIntByName("OfflineRW_Auto", 1440);
				Data.OfflineRW_ItemLimit = (int)GameManager.systemParamsList.GetParamValueIntByName("OfflineRW_ItemLimit", 30);
				Data.OpenData.paimaihangjinbi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangjinbi", 1);
				Data.OpenData.paimaihangzuanshi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangzuanshi", 1);
				Data.OpenData.paimaihangmobi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaihangmobi", 0);
				Data.OpenData.bangzuan = (int)GameManager.systemParamsList.GetParamValueIntByName("bangzuan", 1);
				Data.OpenData.zuanshi = (int)GameManager.systemParamsList.GetParamValueIntByName("zuanshi", 1);
				Data.OpenData.mobi = (int)GameManager.systemParamsList.GetParamValueIntByName("mobi", 0);
				Data.OpenData.paimaijiemianmobi = (int)GameManager.systemParamsList.GetParamValueIntByName("paimaijiemianmobi", 0);
				Data.LuoLanKingGongGaoCD = (long)((int)GameManager.systemParamsList.GetParamValueIntByName("LuoLanKingGongGaoCD", 120));
				int platformType = GameCoreInterface.getinstance().GetPlatformType();
				List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("LogLifeRecoverOpen", '|');
				if (paramValueStringListByName != null)
				{
					ClientCmdCheck.MinLogAddLifeV = 2147483647L;
					ClientCmdCheck.MinLogAddLifePercent = 100L;
					ClientCmdCheck.MapCodes.Clear();
					foreach (string str in paramValueStringListByName)
					{
						int[] array = Global.String2IntArray(str, ',');
						lock (ClientCmdCheck.MapCodes)
						{
							if (array[0] == platformType && array[1] > 0)
							{
								double[] paramValueDoubleArrayByName2 = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LogLifeRecoverNum", ',');
								int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LogLifeRecoverMap", ',');
								if (paramValueDoubleArrayByName2 != null && paramValueDoubleArrayByName2.Length >= 2)
								{
									ClientCmdCheck.MinLogAddLifeV = (long)((int)Math.Max(paramValueDoubleArrayByName2[0], 10000.0));
									ClientCmdCheck.MinLogAddLifePercent = (long)((int)Math.Max(paramValueDoubleArrayByName2[1] * 100.0, 16.0));
								}
								if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length >= 1)
								{
									foreach (int item in paramValueIntArrayByName)
									{
										ClientCmdCheck.MapCodes.Add(item);
									}
								}
								break;
							}
						}
					}
				}
				Data.LoadEquipDelay = (GameManager.systemParamsList.GetParamValueIntByName("LoadEquipDelay", 1) != 0L);
				Data.LoadExtPropThreshold();
				GoodsUtil.LoadConfig();
				Data.LoadChannelNameConfig();
				Data.LoadKuaFuWorldCmds();
				Data.LoadLianZhanConfig();
				Data.LoadMapOptimizeFlags();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			GameManager.OnceDestroyCopyMapNum = (int)GameManager.systemParamsList.GetParamValueIntByName("OnceDestroyCopyMapNum", 100);
		}

		public static void LoadMapOptimizeFlags()
		{
			string text = GameManager.systemParamsList.GetParamValueByName("MapOptimizeFlags");
			if (null == text)
			{
				text = "96000";
			}
			int[] source = ConfigHelper.String2IntArray(text, ',');
			foreach (MapGrid mapGrid in GameManager.MapGridMgr.DictGrids.Values)
			{
				mapGrid.FlagOptimizeFindObjects = source.Contains(mapGrid._GameMap.MapCode);
			}
		}

		public static void LoadExtPropThreshold()
		{
			string text = Global.GameResPath("Config/ExtPropThreshold.xml");
			try
			{
				Data.ExtPropThreshold[24] = new Tuple<double, double>(0.0, 0.8);
				XElement xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "ExtPropThreshold");
					if (null != xelements)
					{
						foreach (XElement xelement2 in xelements)
						{
							int num = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ExtPropID1", 0L);
							double elementAttributeValueDouble = ConfigHelper.GetElementAttributeValueDouble(xelement2, "Min", 0.0);
							double elementAttributeValueDouble2 = ConfigHelper.GetElementAttributeValueDouble(xelement2, "Max", 0.8);
							if (num > 0 && num < 177)
							{
								Data.ExtPropThreshold[num] = new Tuple<double, double>(elementAttributeValueDouble, elementAttributeValueDouble2);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
		}

		public static void LoadUseridWhiteList()
		{
			string text = Global.IsolateResPath("Config/LoginUserWhiteList.xml");
			try
			{
				Data.readWriteLock.EnterWriteLock();
				Data.UseridWhiteList.Clear();
				XElement xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "WhiteList");
					if (null != xelements)
					{
						foreach (XElement xelement2 in xelements)
						{
							string elementAttributeValue = ConfigHelper.GetElementAttributeValue(xelement2, "PinTai", "");
							if (0 == string.Compare(elementAttributeValue, GameCoreInterface.getinstance().GetPlatformType().ToString(), true))
							{
								string elementAttributeValue2 = ConfigHelper.GetElementAttributeValue(xelement2, "UserID", "");
								Data.UseridWhiteList.Add(elementAttributeValue2.ToLower());
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
			finally
			{
				Data.readWriteLock.ExitWriteLock();
			}
		}

		public static bool? InUserWriteList(string userId)
		{
			try
			{
				Data.readWriteLock.EnterReadLock();
				if (Data.UseridWhiteList.Count > 0)
				{
					return new bool?(Data.UseridWhiteList.Contains(userId.ToLower()));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
				return new bool?(false);
			}
			finally
			{
				Data.readWriteLock.ExitReadLock();
			}
			return null;
		}

		private static void LoadSystemParamsValues()
		{
			try
			{
				Data.readWriteLock.EnterWriteLock();
				Data.DaTianShiGoodsIdList.Clear();
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("DaTianShi", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
				{
					foreach (int item in paramValueIntArrayByName)
					{
						Data.DaTianShiGoodsIdList.Add(item);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
			finally
			{
				Data.readWriteLock.ExitWriteLock();
			}
		}

		public static bool IsDaTianShiGoods(int goodsId)
		{
			bool result;
			try
			{
				Data.readWriteLock.EnterReadLock();
				result = Data.DaTianShiGoodsIdList.Contains(goodsId);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
				result = false;
			}
			finally
			{
				Data.readWriteLock.ExitReadLock();
			}
			return result;
		}

		private static void LoadChannelNameConfig()
		{
			try
			{
				string text = Global.GameResPath("Config/ChannelName.xml");
				Data._ChannelNameConfigList.Load(text, null);
				if (null != Data.ChannelNameConfigList)
				{
					Data.ChannelNameConfigList.Sort((ChannelName x, ChannelName y) => StringComparer.Ordinal.Compare(y.Channel, x.Channel));
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static string GetChannelNameByUserID(string userid)
		{
			string result = "IOS";
			List<ChannelName> channelNameConfigList = Data.ChannelNameConfigList;
			if (null != channelNameConfigList)
			{
				foreach (ChannelName channelName in channelNameConfigList)
				{
					if (userid.StartsWith(channelName.Channel))
					{
						result = channelName.Name;
						break;
					}
				}
			}
			return result;
		}

		public static int GetUserPtIDByUserID(string userid)
		{
			PlatformTypes result = 1;
			List<ChannelName> channelNameConfigList = Data.ChannelNameConfigList;
			if (null != channelNameConfigList)
			{
				foreach (ChannelName channelName in channelNameConfigList)
				{
					if (userid.StartsWith(channelName.Channel))
					{
						result = channelName.PTID;
						break;
					}
				}
			}
			return result;
		}

		public static string GetPtNameByPtID(int ptId)
		{
			string result = "";
			List<ChannelName> channelNameConfigList = Data.ChannelNameConfigList;
			if (null != channelNameConfigList)
			{
				ChannelName channelName = channelNameConfigList.Find((ChannelName x) => x.PTID == ptId);
				if (null != channelName)
				{
					result = channelName.PTName;
				}
			}
			return result;
		}

		private static void LoadLianZhanConfig()
		{
			try
			{
				Data.LianZhanTimes = ConfigParser.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("RebornLianZhan"), true, '|', ',');
				Data.LianZhanMaps = GameManager.systemParamsList.GetParamValueIntArrayByName("RebornLianZhanMap", '|');
				string text = Global.GameResPath("Config/RebornLianZhan.xml");
				Data._LianZhanConfigList.Load(text, null);
				if (Data._LianZhanConfigList.Value.Count > 0)
				{
					Data.MinLianZhanNum = Data.LianZhanConfigList.Min((LianZhanConfig x) => x.Num);
					Data.MaxLianZhanNum = Data.LianZhanConfigList.Max((LianZhanConfig x) => x.Num);
					Data.MinLianZhanNum = MathEx.GCD(Data.MinLianZhanNum, Data.MaxLianZhanNum);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static bool IsLianZhanMap(int mapCode)
		{
			int[] lianZhanMaps = Data.LianZhanMaps;
			if (null != lianZhanMaps)
			{
				for (int i = 0; i < lianZhanMaps.Length; i++)
				{
					if (lianZhanMaps[i] == mapCode)
					{
						return true;
					}
				}
			}
			return false;
		}

		public static int LianZhanContinueTime(int num)
		{
			int num2 = 5;
			List<List<int>> lianZhanTimes = Data.LianZhanTimes;
			int result;
			if (lianZhanTimes == null || lianZhanTimes.Count == 0)
			{
				result = num2;
			}
			else
			{
				num2 = lianZhanTimes[lianZhanTimes.Count - 1][1];
				for (int i = 0; i < lianZhanTimes.Count; i++)
				{
					if (num <= lianZhanTimes[i][0])
					{
						num2 = lianZhanTimes[i][1];
						break;
					}
				}
				result = num2;
			}
			return result;
		}

		public static MapSettingItem GetMapSettingInfo(int mapCode)
		{
			Dictionary<int, MapSettingItem> value = Data.SettingsDict.Value;
			MapSettingItem result;
			if (value == null || !value.TryGetValue(mapCode, out result))
			{
				result = new MapSettingItem();
			}
			return result;
		}

		public static void ClearMiniBufferDataIds()
		{
			lock (Data.MiniBufferDataIds)
			{
				List<KeyValuePair<int, int>> list = Data.MiniBufferDataIds.ToList<KeyValuePair<int, int>>();
				foreach (KeyValuePair<int, int> keyValuePair in list)
				{
					if (keyValuePair.Value != 0)
					{
						Data.MiniBufferDataIds.Remove(keyValuePair.Key);
					}
				}
			}
		}

		public static void AddMiniBufferDataIds(params int[] args)
		{
			lock (Data.MiniBufferDataIds)
			{
				foreach (int key in args)
				{
					if (!Data.MiniBufferDataIds.ContainsKey(key))
					{
						Data.MiniBufferDataIds[key] = 1;
					}
				}
			}
		}

		public static bool IsMiniBufferDataId(int id)
		{
			bool result;
			lock (Data.MiniBufferDataIds)
			{
				result = Data.MiniBufferDataIds.ContainsKey(id);
			}
			return result;
		}

		public static bool KuaFuWorldCmdEnabled(int cmd)
		{
			HashSet<int> kuaFuWorldCmds = Data.KuaFuWorldCmds;
			return kuaFuWorldCmds == null || !kuaFuWorldCmds.Contains(cmd);
		}

		private static void LoadKuaFuWorldCmds()
		{
			HashSet<int> hashSet = new HashSet<int>(Data.DisabledKuaFuWorldCmds);
			try
			{
				string text = Global.GameResPath("Config/KuaFuWorldCmds.xml");
				XElement xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					foreach (XElement xelement2 in ConfigHelper.GetXElements(xelement, "KuaFuWorldCmd"))
					{
						int num = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
						int num2 = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Enable", 0L);
						if (num > 0)
						{
							if (num2 > 0)
							{
								hashSet.Add(num);
							}
							else
							{
								hashSet.Remove(num);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			finally
			{
				Data.KuaFuWorldCmds = hashSet;
			}
		}

		public const int ConstSubPKPointPerMin = 10;

		public const string GAME_CONFIG_USERWHITELIST_FILE = "Config/LoginUserWhiteList.xml";

		public const int RoleMaxLevel = 100;

		public const int GlobalData = 0;

		public static int WalkUnitCost;

		public static int RunUnitCost;

		public static int[] SpeedTickList;

		public static int WalkStepWidth;

		public static int RunStepWidth;

		public static int MaxAttackDistance;

		public static int MinAttackDistance;

		public static int MaxMagicDistance;

		public static int MaxAttackSlotTick;

		public static int LifeTotalWidth;

		public static int HoldWidth;

		public static int HoldHeight;

		public static int GoodsPackOvertimeTick = 90;

		public static int PackDestroyTimeTick = 90;

		public static int TaskMaxFocusCount = 400;

		public static int AliveGoodsID = -1;

		public static int AliveMaxLevel = 10;

		public static int AutoGetThing = 0;

		public static long[] LevelUpExperienceList = null;

		public static RoleSitExpItem[] RoleSitExpList = null;

		public static List<RoleBasePropItem[]> RoleBasePropList = new List<RoleBasePropItem[]>();

		public static List<MapStallItem> MapStallList = new List<MapStallItem>(10);

		public static Dictionary<int, string> MapNamesDict = new Dictionary<int, string>(100);

		public static Dictionary<int, ChangeOccupInfo> ChangeOccupInfoList = new Dictionary<int, ChangeOccupInfo>();

		public static Dictionary<int, double> ChangeLifeEverydayExpRate = new Dictionary<int, double>();

		public static Dictionary<int, OccupationAddPointInfo> OccupationAddPointInfoList = new Dictionary<int, OccupationAddPointInfo>();

		public static Dictionary<int, ChangeLifeAddPointInfo> ChangeLifeAddPointInfoList = new Dictionary<int, ChangeLifeAddPointInfo>();

		public static Dictionary<int, BloodCastleDataInfo> BloodCastleDataInfoList = new Dictionary<int, BloodCastleDataInfo>();

		public static Dictionary<int, MoBaiData> MoBaiDataInfoList = new Dictionary<int, MoBaiData>();

		public static Dictionary<int, List<CopyScoreDataInfo>> CopyScoreDataInfoList = new Dictionary<int, List<CopyScoreDataInfo>>();

		public static FreshPlayerCopySceneInfo FreshPlayerSceneInfo = new FreshPlayerCopySceneInfo();

		public static List<TaskStarDataInfo> TaskStarInfo = new List<TaskStarDataInfo>();

		public static List<DailyCircleTaskAwardInfo> DailyCircleTaskAward = new List<DailyCircleTaskAwardInfo>();

		public static TaofaTaskAwardInfo TaofaTaskExAward = new TaofaTaskAwardInfo();

		public static Dictionary<int, CombatForceInfo> CombatForceDataInfo = new Dictionary<int, CombatForceInfo>();

		public static Dictionary<int, DaimonSquareDataInfo> DaimonSquareDataInfoList = new Dictionary<int, DaimonSquareDataInfo>();

		public static double[] WingForgeLevelAddShangHaiJiaCheng = null;

		public static double[] WingForgeLevelAddDefenseRates = null;

		public static double[] WingForgeLevelAddShangHaiXiShou = null;

		public static double[] WingZhuiJiaLevelAddDefenseRates = null;

		public static double[] ForgeLevelAddAttackRates = null;

		public static double[] ForgeLevelAddDefenseRates = null;

		public static double[] ZhuiJiaLevelAddAttackRates = null;

		public static double[] ZhuiJiaLevelAddDefenseRates = null;

		public static double[] ForgeLevelAddMaxLifeVRates = null;

		public static double[] ZhuoYueAddAttackRates = null;

		public static double[] ZhuoYueAddDefenseRates = null;

		public static double[] RebornZhuoYueAddRates = null;

		public static int[] ForgeProtectStoneGoodsID = null;

		public static int[] ForgeProtectStoneGoodsNum = null;

		public static int DiamondToVipExpValue = 0;

		public static double[] RedNameDebuffInfo = null;

		public static string[] ForgeNeedGoodsID = new string[21];

		public static string[] ForgeNeedGoodsNum = new string[21];

		public static Dictionary<int, int> MapTransNeedMoneyDict = new Dictionary<int, int>();

		public static double[] EquipChangeLifeAddAttackRates = null;

		public static double[] EquipChangeLifeAddDefenseRates = null;

		public static int[] KillBossCountForChengJiu = null;

		public static int InsertAwardtPortableBagTaskID = 0;

		public static string InsertAwardtPortableBagGoodsInfo = null;

		public static int PaihangbangAdration = 0;

		public static int[] StoryCopyMapID = null;

		public static int FreeImpetrateIntervalTime = 0;

		public static Dictionary<int, TotalLoginDataInfo> TotalLoginDataInfoList = new Dictionary<int, TotalLoginDataInfo>();

		public static object TotalLoginDataInfoListLock = new object();

		public static Dictionary<int, VIPDataInfo> VIPDataInfoList = new Dictionary<int, VIPDataInfo>();

		public static Dictionary<int, VIPLevAwardAndExpInfo> VIPLevAwardAndExpInfoList = new Dictionary<int, VIPLevAwardAndExpInfo>();

		public static Dictionary<int, MeditateData> MeditateInfoList = new Dictionary<int, MeditateData>();

		public static Dictionary<int, ExperienceCopyMapDataInfo> ExperienceCopyMapDataInfoList = new Dictionary<int, ExperienceCopyMapDataInfo>();

		public static PKKingAdrationData PKkingadrationData = new PKKingAdrationData();

		public static PKKingAdrationData LLCZadrationData = new PKKingAdrationData();

		public static BossHomeData BosshomeData = new BossHomeData();

		public static GoldTempleData GoldtempleData = new GoldTempleData();

		public static Dictionary<int, PictureJudgeData> PicturejudgeData = new Dictionary<int, PictureJudgeData>();

		public static Dictionary<int, PictureJudgeTypeData> PicturejudgeTypeData = new Dictionary<int, PictureJudgeTypeData>();

		public static Dictionary<int, Dictionary<int, MuEquipUpgradeData>> EquipUpgradeData = new Dictionary<int, Dictionary<int, MuEquipUpgradeData>>();

		public static GoldCopySceneData Goldcopyscenedata = new GoldCopySceneData();

		public static Dictionary<int, EquipJuHunXmlData> EquipJuHunDataDict = new Dictionary<int, EquipJuHunXmlData>();

		public static Dictionary<int, BagTypeXmlData> BagTypeDict = new Dictionary<int, BagTypeXmlData>();

		private static SingleChargeData _ChargeData = null;

		private static object SingleChargeDataMutex = new object();

		private static object ChargeItemDataMutex = new object();

		public static Dictionary<int, ChargeItemData> _ChargeItemDict = null;

		public static Dictionary<int, int> LingYuMaterialZuanshiDict = new Dictionary<int, int>();

		public static Dictionary<int, int> FuBenNeedDict = new Dictionary<int, int>();

		public static long CombatForceLogMinValue = 1800000L;

		public static double CombatForceLogPercent = 0.25;

		private static ReaderWriterLockSlim readWriteLock = new ReaderWriterLockSlim();

		public static HashSet<string> UseridWhiteList = new HashSet<string>();

		public static HashSet<int> DaTianShiGoodsIdList = new HashSet<int>();

		public static HashSet<int> CanTeleportMapHashSet = new HashSet<int>
		{
			5,
			6,
			7,
			12,
			13,
			24,
			21,
			20,
			14,
			15,
			25,
			26,
			27,
			31,
			35,
			36,
			38,
			39,
			40,
			43,
			50,
			51
		};

		public static long FightStateTime = 6000L;

		public static int NotifyLiXianAwardMin = 0;

		public static int OfflineRW_ItemLimit = 0;

		public static bool CheckTimeBoost = true;

		public static bool CheckPositionCheat = true;

		public static bool CheckPositionCheatSpeed = true;

		public static long SyncTimeByClient = 0L;

		public static bool IgnoreClientPos = false;

		public static long MaxServerClientTimeDiff;

		public static int RoleOccupationMaxCount = 2;

		public static int OpChangeLifeCount = 100;

		public static SystemOpenData OpenData = new SystemOpenData();

		public static int ZhuTiID;

		public static int ThemeActivityState;

		public static TemplateLoader<Dictionary<int, MapSettingItem>> SettingsDict = new TemplateLoader<Dictionary<int, MapSettingItem>>();

		public static int MinLianZhanNum;

		public static int MaxLianZhanNum;

		private static TemplateLoader<List<ChannelName>> _ChannelNameConfigList = new TemplateLoader<List<ChannelName>>();

		private static TemplateLoader<List<LianZhanConfig>> _LianZhanConfigList = new TemplateLoader<List<LianZhanConfig>>();

		public static List<List<int>> LianZhanTimes = null;

		public static int[] LianZhanMaps = null;

		private static HashSet<int> KuaFuWorldCmds = new HashSet<int>();

		public static bool LoadEquipDelay = true;

		public static long LuoLanKingGongGaoCD = 120L;

		public static LongCollection NextBroadCastTickDict = new LongCollection();

		private static Dictionary<int, int> MiniBufferDataIds = new Dictionary<int, int>
		{
			{
				81,
				0
			},
			{
				83,
				0
			},
			{
				84,
				0
			},
			{
				102,
				0
			},
			{
				39,
				0
			},
			{
				103,
				0
			},
			{
				111,
				0
			},
			{
				101,
				0
			},
			{
				2080011,
				0
			},
			{
				2080010,
				0
			},
			{
				2080001,
				0
			},
			{
				2080007,
				0
			},
			{
				2080008,
				0
			},
			{
				2080009,
				0
			},
			{
				2080002,
				0
			},
			{
				116,
				0
			},
			{
				121,
				0
			},
			{
				2000853,
				0
			},
			{
				2000854,
				0
			},
			{
				2000855,
				0
			},
			{
				2000856,
				0
			},
			{
				2000857,
				0
			},
			{
				10013,
				0
			},
			{
				10020,
				0
			},
			{
				10022,
				0
			},
			{
				10023,
				0
			},
			{
				10012,
				0
			},
			{
				10011,
				0
			},
			{
				10010,
				0
			},
			{
				10009,
				0
			},
			{
				10008,
				0
			},
			{
				10007,
				0
			},
			{
				10001,
				0
			},
			{
				10002,
				0
			},
			{
				10003,
				0
			},
			{
				10004,
				0
			},
			{
				9000,
				0
			},
			{
				9001,
				0
			},
			{
				9002,
				0
			},
			{
				9003,
				0
			},
			{
				9004,
				0
			},
			{
				9005,
				0
			},
			{
				9006,
				0
			},
			{
				9007,
				0
			},
			{
				9008,
				0
			},
			{
				9009,
				0
			},
			{
				9010,
				0
			},
			{
				9011,
				0
			},
			{
				9012,
				0
			},
			{
				9051,
				0
			},
			{
				9052,
				0
			}
		};

		public static Tuple<double, double>[] ExtPropThreshold = new Tuple<double, double>[177];

		private static HashSet<int> DisabledKuaFuWorldCmds = new HashSet<int>();
	}
}
