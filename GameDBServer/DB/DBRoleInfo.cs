using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameDBServer.Data;
using GameDBServer.Data.Tarot;
using GameDBServer.Logic;
using GameDBServer.Logic.FluorescentGem;
using GameDBServer.Logic.Rank;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB
{
	public class DBRoleInfo
	{
		public int RoleID { get; set; }

		public string UserID { get; set; }

		public string RoleName { get; set; }

		public int RoleSex { get; set; }

		public int Occupation { get; set; }

		public int Level { get; set; }

		public int RolePic { get; set; }

		public int Faction { get; set; }

		public int Money1 { get; set; }

		public int Money2 { get; set; }

		public long Experience { get; set; }

		public int PKMode { get; set; }

		public int PKValue { get; set; }

		public string Position { get; set; }

		public string RegTime { get; set; }

		public long LastTime { get; set; }

		public int BagNum { get; set; }

		public int RebornBagNum { get; set; }

		public string OtherName { get; set; }

		public string MainQuickBarKeys { get; set; }

		public string OtherQuickBarKeys { get; set; }

		public int LoginNum { get; set; }

		public int LeftFightSeconds { get; set; }

		public int ServerLineID { get; set; }

		public int HorseDbID { get; set; }

		public int PetDbID { get; set; }

		public int InterPower { get; set; }

		public int TotalOnlineSecs { get; set; }

		public int AntiAddictionSecs { get; set; }

		public long LogOffTime { get; set; }

		public long BiGuanTime { get; set; }

		public int YinLiang { get; set; }

		public int TotalJingMaiExp { get; set; }

		public int JingMaiExpNum { get; set; }

		public int LastHorseID { get; set; }

		public int DefaultSkillID { get; set; }

		public int AutoLifeV { get; set; }

		public int AutoMagicV { get; set; }

		public int NumSkillID { get; set; }

		public int MainTaskID { get; set; }

		public int PKPoint { get; set; }

		public int LianZhan { get; set; }

		public int KillBoss { get; set; }

		public long BattleNameStart { get; set; }

		public int BattleNameIndex { get; set; }

		public int CZTaskID { get; set; }

		public int BattleNum { get; set; }

		public int HeroIndex { get; set; }

		public int LoginDayID { get; set; }

		public int LoginDayNum { get; set; }

		public int ZoneID { get; set; }

		public string BHName { get; set; }

		public int BHVerify { get; set; }

		public int BHZhiWu { get; set; }

		public int BGDayID1 { get; set; }

		public int BGMoney { get; set; }

		public int BGDayID2 { get; set; }

		public int BGGoods { get; set; }

		public int BangGong { get; set; }

		public int HuangHou { get; set; }

		public int JieBiaoDayID { get; set; }

		public int JieBiaoDayNum { get; set; }

		public string UserName { get; set; }

		public int LastMailID { get; set; }

		public long OnceAwardFlag { get; set; }

		public int Gold { get; set; }

		public int BanChat { get; set; }

		public int BanLogin { get; set; }

		public int IsFlashPlayer { get; set; }

		public int ChangeLifeCount { get; set; }

		public int AdmiredCount { get; set; }

		public int CombatForce { get; set; }

		public int AutoAssignPropertyPoint { get; set; }

		public string PushMsgID { get; set; }

		public int VipAwardFlag { get; set; }

		public int VIPLevel { get; set; }

		public long store_yinliang { get; set; }

		public long store_money { get; set; }

		public int MagicSwordParam { get; set; }

		public UserRankValueCache RankValue
		{
			get
			{
				return this.rankValue;
			}
			set
			{
				this.rankValue = value;
			}
		}

		public long UpdateDBPositionTicks { get; set; }

		public long UpdateDBTimeTicks { get; set; }

		public long UpdateDBInterPowerTimeTicks { get; set; }

		public List<OldTaskData> OldTasks { get; set; }

		public List<TaskData> DoingTaskList { get; set; }

		public List<GoodsData> GoodsDataList { get; set; }

		public List<GoodsData> RebornGoodsDataList { get; set; }

		public List<GoodsLimitData> GoodsLimitDataList { get; set; }

		public List<FriendData> FriendDataList { get; set; }

		public List<HorseData> HorsesDataList { get; set; }

		public List<PetData> PetsDataList { get; set; }

		public long LastDJPointDataTikcs { get; set; }

		public DJPointData RoleDJPointData { get; set; }

		public List<JingMaiData> JingMaiDataList { get; set; }

		public List<SkillData> SkillDataList { get; set; }

		public List<BufferData> BufferDataList { get; set; }

		public List<DailyTaskData> MyDailyTaskDataList { get; set; }

		public DailyJingMaiData MyDailyJingMaiData { get; set; }

		public PortableBagData MyPortableBagData { get; set; }

		public RebornPortableBagData RebornGirdData { get; set; }

		public int RebornShowEquip { get; set; }

		public int RebornShowModel { get; set; }

		public bool ExistsMyHuodongData { get; set; }

		public HuodongData MyHuodongData { get; set; }

		public List<FuBenData> FuBenDataList { get; set; }

		public MarriageData MyMarriageData { get; set; }

		public Dictionary<int, int> MyMarryPartyJoinList { get; set; }

		public Dictionary<sbyte, HolyItemData> MyHolyItemDataDic { get; set; }

		public RoleDailyData MyRoleDailyData { get; set; }

		public YaBiaoData MyYaBiaoData { get; set; }

		public long LastReferenceTicks
		{
			get
			{
				return this._LastReferenceTicks;
			}
			set
			{
				this._LastReferenceTicks = value;
			}
		}

		public Dictionary<int, int> PaiHangPosDict { get; set; }

		public List<VipDailyData> VipDailyDataList { get; set; }

		public YangGongBKDailyJiFenData YangGongBKDailyJiFen { get; set; }

		public WingData MyWingData { get; set; }

		public Dictionary<int, int> PictureJudgeReferInfo { get; set; }

		public Dictionary<int, int> StarConstellationInfo { get; set; }

		public Dictionary<int, LingYuData> LingYuDict { get; set; }

		public GuardStatueDetail MyGuardStatueDetail { get; set; }

		public TalentData MyTalentData { get; set; }

		public string LastIP { get; set; }

		public List<int> GroupMailRecordList { get; set; }

		public MerlinGrowthSaveDBData MerlinData { get; set; }

		public FluorescentGemData FluorescentGemData { get; set; }

		public int FluorescentPoint { get; set; }

		public List<BuildingData> BuildingDataList { get; set; }

		public Dictionary<int, OrnamentData> OrnamentDataDict { get; set; }

		public Dictionary<int, Dictionary<int, SevenDayItemData>> SevenDayActDict { get; set; }

		public long BanTradeToTicks { get; set; }

		public Dictionary<int, SpecActInfoDB> SpecActInfoDict { get; set; }

		public Dictionary<int, EverydayActInfoDB> EverydayActInfoDict { get; set; }

		public Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB> SpecPriorityActInfoDict { get; set; }

		public AlchemyDataDB AlchemyInfo { get; set; }

		public Dictionary<int, ShenJiFuWenData> ShenJiDict { get; set; }

		public TarotSystemData TarotData { get; set; }

		public List<FuWenTabData> FuWenTabList { get; set; }

		public List<TaoZhuangData> JueXingTaoZhuangList { get; set; }

		public List<MountData> MountList { get; set; }

		public RebornStampData RebornYinJi { get; set; }

		public static void DBTableRow2RoleInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, int index)
		{
			dbRoleInfo.RoleID = Convert.ToInt32(cmd.Table.Rows[index]["rid"]);
			dbRoleInfo.UserID = cmd.Table.Rows[index]["userid"].ToString();
			dbRoleInfo.RoleName = cmd.Table.Rows[index]["rname"].ToString();
			dbRoleInfo.RoleSex = Convert.ToInt32(cmd.Table.Rows[index]["sex"]);
			dbRoleInfo.Occupation = Convert.ToInt32(cmd.Table.Rows[index]["occupation"]);
			dbRoleInfo.Level = Convert.ToInt32(cmd.Table.Rows[index]["level"]);
			dbRoleInfo.RolePic = Convert.ToInt32(cmd.Table.Rows[index]["pic"]);
			dbRoleInfo.Faction = Convert.ToInt32(cmd.Table.Rows[index]["faction"]);
			dbRoleInfo.Money1 = Convert.ToInt32(cmd.Table.Rows[index]["money1"]);
			dbRoleInfo.Money2 = Convert.ToInt32(cmd.Table.Rows[index]["money2"]);
			dbRoleInfo.Experience = Convert.ToInt64(cmd.Table.Rows[index]["experience"]);
			dbRoleInfo.PKMode = Convert.ToInt32(cmd.Table.Rows[index]["pkmode"]);
			dbRoleInfo.PKValue = Convert.ToInt32(cmd.Table.Rows[index]["pkvalue"]);
			dbRoleInfo.Position = cmd.Table.Rows[index]["position"].ToString();
			dbRoleInfo.RegTime = cmd.Table.Rows[index]["regtime"].ToString();
			dbRoleInfo.LastTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["lasttime"].ToString());
			dbRoleInfo.BagNum = Convert.ToInt32(cmd.Table.Rows[index]["bagnum"]);
			dbRoleInfo.OtherName = cmd.Table.Rows[index]["othername"].ToString();
			dbRoleInfo.MainQuickBarKeys = cmd.Table.Rows[index]["main_quick_keys"].ToString();
			dbRoleInfo.OtherQuickBarKeys = cmd.Table.Rows[index]["other_quick_keys"].ToString();
			dbRoleInfo.LoginNum = Convert.ToInt32(cmd.Table.Rows[index]["loginnum"].ToString());
			dbRoleInfo.LeftFightSeconds = Convert.ToInt32(cmd.Table.Rows[index]["leftfightsecs"].ToString());
			dbRoleInfo.HorseDbID = Convert.ToInt32(cmd.Table.Rows[index]["horseid"].ToString());
			dbRoleInfo.PetDbID = Convert.ToInt32(cmd.Table.Rows[index]["petid"].ToString());
			dbRoleInfo.InterPower = Convert.ToInt32(cmd.Table.Rows[index]["interpower"].ToString());
			dbRoleInfo.TotalOnlineSecs = Convert.ToInt32(cmd.Table.Rows[index]["totalonlinesecs"].ToString());
			dbRoleInfo.AntiAddictionSecs = Convert.ToInt32(cmd.Table.Rows[index]["antiaddictionsecs"].ToString());
			dbRoleInfo.LogOffTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["logofftime"].ToString());
			dbRoleInfo.BiGuanTime = DataHelper.ConvertToTicks(cmd.Table.Rows[index]["biguantime"].ToString());
			dbRoleInfo.YinLiang = Convert.ToInt32(cmd.Table.Rows[index]["yinliang"].ToString());
			dbRoleInfo.TotalJingMaiExp = Convert.ToInt32(cmd.Table.Rows[index]["total_jingmai_exp"].ToString());
			dbRoleInfo.JingMaiExpNum = Convert.ToInt32(cmd.Table.Rows[index]["jingmai_exp_num"].ToString());
			dbRoleInfo.LastHorseID = Convert.ToInt32(cmd.Table.Rows[index]["lasthorseid"].ToString());
			dbRoleInfo.DefaultSkillID = Convert.ToInt32(cmd.Table.Rows[index]["skillid"].ToString());
			dbRoleInfo.AutoLifeV = Convert.ToInt32(cmd.Table.Rows[index]["autolife"].ToString());
			dbRoleInfo.AutoMagicV = Convert.ToInt32(cmd.Table.Rows[index]["automagic"].ToString());
			dbRoleInfo.NumSkillID = Convert.ToInt32(cmd.Table.Rows[index]["numskillid"].ToString());
			dbRoleInfo.MainTaskID = Convert.ToInt32(cmd.Table.Rows[index]["maintaskid"].ToString());
			dbRoleInfo.PKPoint = Convert.ToInt32(cmd.Table.Rows[index]["pkpoint"].ToString());
			dbRoleInfo.LianZhan = Convert.ToInt32(cmd.Table.Rows[index]["lianzhan"].ToString());
			dbRoleInfo.KillBoss = Convert.ToInt32(cmd.Table.Rows[index]["killboss"].ToString());
			dbRoleInfo.BattleNameStart = Convert.ToInt64(cmd.Table.Rows[index]["battlenamestart"].ToString());
			dbRoleInfo.BattleNameIndex = Convert.ToInt32(cmd.Table.Rows[index]["battlenameindex"].ToString());
			dbRoleInfo.CZTaskID = Convert.ToInt32(cmd.Table.Rows[index]["cztaskid"].ToString());
			dbRoleInfo.BattleNum = Convert.ToInt32(cmd.Table.Rows[index]["battlenum"].ToString());
			dbRoleInfo.HeroIndex = Convert.ToInt32(cmd.Table.Rows[index]["heroindex"].ToString());
			dbRoleInfo.LoginDayID = Convert.ToInt32(cmd.Table.Rows[index]["logindayid"].ToString());
			dbRoleInfo.LoginDayNum = Convert.ToInt32(cmd.Table.Rows[index]["logindaynum"].ToString());
			dbRoleInfo.ZoneID = Convert.ToInt32(cmd.Table.Rows[index]["zoneid"].ToString());
			dbRoleInfo.BHName = cmd.Table.Rows[index]["bhname"].ToString();
			dbRoleInfo.BHVerify = Convert.ToInt32(cmd.Table.Rows[index]["bhverify"].ToString());
			dbRoleInfo.BHZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["bhzhiwu"].ToString());
			dbRoleInfo.BGDayID1 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid1"].ToString());
			dbRoleInfo.BGMoney = Convert.ToInt32(cmd.Table.Rows[index]["bgmoney"].ToString());
			dbRoleInfo.BGDayID2 = Convert.ToInt32(cmd.Table.Rows[index]["bgdayid2"].ToString());
			dbRoleInfo.BGGoods = Convert.ToInt32(cmd.Table.Rows[index]["bggoods"].ToString());
			dbRoleInfo.BangGong = Convert.ToInt32(cmd.Table.Rows[index]["banggong"].ToString());
			dbRoleInfo.HuangHou = Convert.ToInt32(cmd.Table.Rows[index]["huanghou"].ToString());
			dbRoleInfo.JieBiaoDayID = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaodayid"].ToString());
			dbRoleInfo.JieBiaoDayNum = Convert.ToInt32(cmd.Table.Rows[index]["jiebiaonum"].ToString());
			dbRoleInfo.UserName = cmd.Table.Rows[index]["username"].ToString();
			dbRoleInfo.LastMailID = Convert.ToInt32(cmd.Table.Rows[index]["lastmailid"].ToString());
			dbRoleInfo.OnceAwardFlag = Convert.ToInt64(cmd.Table.Rows[index]["onceawardflag"].ToString());
			dbRoleInfo.Gold = Convert.ToInt32(cmd.Table.Rows[index]["money2"].ToString());
			dbRoleInfo.BanChat = Convert.ToInt32(cmd.Table.Rows[index]["banchat"].ToString());
			dbRoleInfo.BanLogin = Convert.ToInt32(cmd.Table.Rows[index]["banlogin"].ToString());
			dbRoleInfo.IsFlashPlayer = Convert.ToInt32(cmd.Table.Rows[index]["isflashplayer"].ToString());
			dbRoleInfo.ChangeLifeCount = Convert.ToInt32(cmd.Table.Rows[index]["changelifecount"].ToString());
			dbRoleInfo.AdmiredCount = Convert.ToInt32(cmd.Table.Rows[index]["admiredcount"].ToString());
			dbRoleInfo.CombatForce = Convert.ToInt32(cmd.Table.Rows[index]["combatforce"].ToString());
			dbRoleInfo.AutoAssignPropertyPoint = Convert.ToInt32(cmd.Table.Rows[index]["autoassignpropertypoint"].ToString());
			dbRoleInfo.store_yinliang = Convert.ToInt64(cmd.Table.Rows[index]["store_yinliang"]);
			dbRoleInfo.store_money = Convert.ToInt64(cmd.Table.Rows[index]["store_money"]);
			dbRoleInfo.MagicSwordParam = Convert.ToInt32(cmd.Table.Rows[index]["magic_sword_param"]);
			dbRoleInfo.FluorescentPoint = Convert.ToInt32(cmd.Table.Rows[index]["fluorescent_point"]);
			dbRoleInfo.BanTradeToTicks = Convert.ToInt64(cmd.Table.Rows[index]["ban_trade_to_ticks"].ToString());
			dbRoleInfo.JunTuanZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["juntuanzhiwu"]);
			dbRoleInfo.HuiJiData.huiji = Convert.ToInt32(cmd.Table.Rows[index]["huiji"]);
			dbRoleInfo.HuiJiData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["huijiexp"]);
			dbRoleInfo.ArmorData.Armor = Convert.ToInt32(cmd.Table.Rows[index]["armor"]);
			dbRoleInfo.ArmorData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["armorexp"]);
			dbRoleInfo.BianShenData.BianShen = Convert.ToInt32(cmd.Table.Rows[index]["bianshen"]);
			dbRoleInfo.BianShenData.Exp = Convert.ToInt32(cmd.Table.Rows[index]["bianshenexp"]);
			dbRoleInfo.RebornBagNum = Convert.ToInt32(cmd.Table.Rows[index]["reborn_bagnum"]);
			dbRoleInfo.RebornShowEquip = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow"]);
			dbRoleInfo.RebornShowModel = Convert.ToInt32(cmd.Table.Rows[index]["reborn_isshow_model"]);
			dbRoleInfo.ZhanDuiID = Convert.ToInt32(cmd.Table.Rows[index]["zhanduiid"]);
			dbRoleInfo.ZhanDuiZhiWu = Convert.ToInt32(cmd.Table.Rows[index]["zhanduizhiwu"]);
		}

		public static void DBTableRow2RoleInfo_Params(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd, bool normalOnly)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> roleParamsDict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					RoleParamsData roleParamsData = new RoleParamsData
					{
						ParamName = cmd.Table.Rows[i]["pname"].ToString(),
						ParamValue = cmd.Table.Rows[i]["pvalue"].ToString()
					};
					roleParamsData.ParamType = RoleParamNameInfo.GetRoleParamType(roleParamsData.ParamName, roleParamsData.ParamValue);
					if (roleParamsData.ParamType.Type <= 0 || !normalOnly)
					{
						roleParamsDict[roleParamsData.ParamName] = roleParamsData;
					}
				}
			}
		}

		public static void DBTableRow2RoleInfo_ParamsEx(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<string, RoleParamsData> roleParamsDict = dbRoleInfo.RoleParamsDict;
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int idx = Convert.ToInt32(cmd.Table.Rows[i]["idx"].ToString());
					int num = cmd.Table.Rows[i].ItemArray.Length;
					for (int j = 2; j < num; j++)
					{
						RoleParamType roleParamType = RoleParamNameInfo.GetRoleParamType(idx, j - 2);
						if (null != roleParamType)
						{
							RoleParamsData roleParamsData = new RoleParamsData
							{
								ParamName = roleParamType.ParamName,
								ParamValue = cmd.Table.Rows[i][j].ToString(),
								ParamType = roleParamType
							};
							roleParamsDict[roleParamsData.ParamName] = roleParamsData;
						}
					}
				}
			}
		}

		public static void InitFromRoleParams(DBRoleInfo dbRoleInfo)
		{
			string roleParamByName = Global.GetRoleParamByName(dbRoleInfo, "20017");
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					'$'
				});
				foreach (string s in array)
				{
					int item;
					if (int.TryParse(s, out item) && !dbRoleInfo.OccupationList.Contains(item))
					{
						dbRoleInfo.OccupationList.Add(item);
					}
				}
			}
			if (!dbRoleInfo.OccupationList.Contains(dbRoleInfo.Occupation))
			{
				dbRoleInfo.OccupationList.Insert(0, dbRoleInfo.Occupation);
			}
			roleParamByName = Global.GetRoleParamByName(dbRoleInfo, "10213");
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				dbRoleInfo.SubOccupation = Global.SafeConvertToInt32(roleParamByName, 10);
			}
		}

		public static void DBTableRow2RoleInfo_OldTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				List<OldTaskData> list = new List<OldTaskData>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					list.Add(new OldTaskData
					{
						TaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoCount = Convert.ToInt32(cmd.Table.Rows[i]["count"].ToString())
					});
				}
				dbRoleInfo.OldTasks = list;
			}
		}

		public static void DBTableRow2RoleInfo_DoingTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.DoingTaskList = new List<TaskData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.DoingTaskList.Add(new TaskData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString()),
						DoingTaskID = Convert.ToInt32(cmd.Table.Rows[i]["taskid"].ToString()),
						DoingTaskVal1 = Convert.ToInt32(cmd.Table.Rows[i]["value1"].ToString()),
						DoingTaskVal2 = Convert.ToInt32(cmd.Table.Rows[i]["value2"].ToString()),
						DoingTaskFocus = Convert.ToInt32(cmd.Table.Rows[i]["focus"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						StarLevel = Convert.ToInt32(cmd.Table.Rows[i]["starlevel"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Goods(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsDataList = new List<GoodsData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GoodsData goodsData = new GoodsData
					{
						Id = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						Using = Convert.ToInt32(cmd.Table.Rows[i]["isusing"].ToString()),
						Forge_level = Convert.ToInt32(cmd.Table.Rows[i]["forge_level"].ToString()),
						Starttime = cmd.Table.Rows[i]["starttime"].ToString(),
						Endtime = cmd.Table.Rows[i]["endtime"].ToString(),
						Site = Convert.ToInt32(cmd.Table.Rows[i]["site"].ToString()),
						Quality = Convert.ToInt32(cmd.Table.Rows[i]["quality"].ToString()),
						Props = cmd.Table.Rows[i]["Props"].ToString(),
						GCount = Convert.ToInt32(cmd.Table.Rows[i]["gcount"].ToString()),
						Binding = Convert.ToInt32(cmd.Table.Rows[i]["binding"].ToString()),
						Jewellist = cmd.Table.Rows[i]["jewellist"].ToString(),
						BagIndex = Convert.ToInt32(cmd.Table.Rows[i]["bagindex"].ToString()),
						SaleMoney1 = Convert.ToInt32(cmd.Table.Rows[i]["salemoney1"].ToString()),
						SaleYuanBao = Convert.ToInt32(cmd.Table.Rows[i]["saleyuanbao"].ToString()),
						SaleYinPiao = Convert.ToInt32(cmd.Table.Rows[i]["saleyinpiao"].ToString()),
						AddPropIndex = Convert.ToInt32(cmd.Table.Rows[i]["addpropindex"].ToString()),
						BornIndex = Convert.ToInt32(cmd.Table.Rows[i]["bornindex"].ToString()),
						Lucky = Convert.ToInt32(cmd.Table.Rows[i]["lucky"].ToString()),
						Strong = Convert.ToInt32(cmd.Table.Rows[i]["strong"].ToString()),
						ExcellenceInfo = Convert.ToInt32(cmd.Table.Rows[i]["excellenceinfo"].ToString()),
						AppendPropLev = Convert.ToInt32(cmd.Table.Rows[i]["appendproplev"].ToString()),
						ChangeLifeLevForEquip = Convert.ToInt32(cmd.Table.Rows[i]["equipchangelife"].ToString()),
						JuHunID = Convert.ToInt32(cmd.Table.Rows[i]["juhun"].ToString())
					};
					string text = cmd.Table.Rows[i]["washprops"].ToString();
					if (!string.IsNullOrEmpty(text))
					{
						try
						{
							byte[] array = Convert.FromBase64String(text);
							goodsData.WashProps = DataHelper.BytesToObject<List<int>>(array, 0, array.Length);
						}
						catch
						{
						}
					}
					string text2 = cmd.Table.Rows[i]["ehinfo"].ToString();
					if (!string.IsNullOrEmpty(text2))
					{
						try
						{
							byte[] array = Convert.FromBase64String(text2);
							goodsData.ElementhrtsProps = DataHelper.BytesToObject<List<int>>(array, 0, array.Length);
						}
						catch
						{
						}
					}
					dbRoleInfo.GoodsDataList.Add(goodsData);
				}
			}
		}

		public static void DBTableRow2RoleInfo_GoodsLimit(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GoodsLimitDataList = new List<GoodsLimitData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GoodsLimitDataList.Add(new GoodsLimitData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Friends(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FriendDataList = new List<FriendData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FriendDataList.Add(new FriendData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						OtherRoleID = Convert.ToInt32(cmd.Table.Rows[i]["otherid"].ToString()),
						FriendType = Convert.ToInt32(cmd.Table.Rows[i]["friendType"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Horses(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.HorsesDataList = new List<HorseData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.HorsesDataList.Add(new HorseData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						HorseID = Convert.ToInt32(cmd.Table.Rows[i]["horseid"].ToString()),
						BodyID = Convert.ToInt32(cmd.Table.Rows[i]["bodyid"].ToString()),
						PropsNum = cmd.Table.Rows[i]["propsNum"].ToString(),
						PropsVal = cmd.Table.Rows[i]["PropsVal"].ToString(),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[i]["failednum"].ToString()),
						JinJieTempTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["temptime"].ToString()),
						JinJieTempNum = Convert.ToInt32(cmd.Table.Rows[i]["tempnum"].ToString()),
						JinJieFailedDayID = Convert.ToInt32(cmd.Table.Rows[i]["faileddayid"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Pets(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.PetsDataList = new List<PetData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.PetsDataList.Add(new PetData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						PetID = Convert.ToInt32(cmd.Table.Rows[i]["petid"].ToString()),
						PetName = cmd.Table.Rows[i]["petname"].ToString(),
						PetType = Convert.ToInt32(cmd.Table.Rows[i]["pettype"].ToString()),
						FeedNum = Convert.ToInt32(cmd.Table.Rows[i]["feednum"].ToString()),
						ReAliveNum = Convert.ToInt32(cmd.Table.Rows[i]["realivenum"].ToString()),
						AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[i]["addtime"].ToString()),
						PetProps = cmd.Table.Rows[i]["props"].ToString(),
						Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_JingMais(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.JingMaiDataList = new List<JingMaiData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.JingMaiDataList.Add(new JingMaiData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						JingMaiID = Convert.ToInt32(cmd.Table.Rows[i]["jmid"].ToString()),
						JingMaiLevel = Convert.ToInt32(cmd.Table.Rows[i]["jmlevel"].ToString()),
						JingMaiBodyLevel = Convert.ToInt32(cmd.Table.Rows[i]["bodylevel"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Skills(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SkillDataList = new List<SkillData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.SkillDataList.Add(new SkillData
					{
						DbID = Convert.ToInt32(cmd.Table.Rows[i]["Id"].ToString()),
						SkillID = Convert.ToInt32(cmd.Table.Rows[i]["skillid"].ToString()),
						SkillLevel = Convert.ToInt32(cmd.Table.Rows[i]["skilllevel"].ToString()),
						UsedNum = Convert.ToInt32(cmd.Table.Rows[i]["usednum"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_Buffers(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BufferDataList = new List<BufferData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.BufferDataList.Add(new BufferData
					{
						BufferID = Convert.ToInt32(cmd.Table.Rows[i]["bufferid"].ToString()),
						StartTime = Convert.ToInt64(cmd.Table.Rows[i]["starttime"].ToString()),
						BufferSecs = Convert.ToInt32(cmd.Table.Rows[i]["buffersecs"].ToString()),
						BufferVal = Convert.ToInt64(cmd.Table.Rows[i]["bufferval"].ToString()),
						BufferType = 0
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_DailyTasks(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MyDailyTaskDataList)
			{
				dbRoleInfo.MyDailyTaskDataList = new List<DailyTaskData>();
			}
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				DailyTaskData item = new DailyTaskData
				{
					HuanID = Convert.ToInt32(cmd.Table.Rows[i]["huanid"].ToString()),
					RecTime = cmd.Table.Rows[i]["rectime"].ToString(),
					RecNum = Convert.ToInt32(cmd.Table.Rows[i]["recnum"].ToString()),
					TaskClass = Convert.ToInt32(cmd.Table.Rows[i]["taskClass"].ToString()),
					ExtDayID = Convert.ToInt32(cmd.Table.Rows[i]["extdayid"].ToString()),
					ExtNum = Convert.ToInt32(cmd.Table.Rows[i]["extnum"].ToString())
				};
				dbRoleInfo.MyDailyTaskDataList.Add(item);
			}
		}

		public static void DBTableRow2RoleInfo_DailyJingMai(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyDailyJingMaiData = new DailyJingMaiData
				{
					JmTime = cmd.Table.Rows[0]["jmtime"].ToString(),
					JmNum = Convert.ToInt32(cmd.Table.Rows[0]["jmnum"].ToString())
				};
			}
		}

		public static void DBTableRow2RoleInfo_PortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyPortableBagData = new PortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyPortableBagData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		public static void DBTableRow2RoleInfo_RebornPortableBag(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.RebornGirdData = new RebornPortableBagData
			{
				GoodsUsedGridNum = 0
			};
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.RebornGirdData.ExtGridNum = Convert.ToInt32(cmd.Table.Rows[0]["extgridnum"].ToString());
			}
		}

		public static void DBTableRow2RoleInfo_HuodongData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ExistsMyHuodongData = false;
			dbRoleInfo.MyHuodongData = new HuodongData();
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.ExistsMyHuodongData = true;
				dbRoleInfo.MyHuodongData.LastWeekID = cmd.Table.Rows[0]["loginweekid"].ToString();
				dbRoleInfo.MyHuodongData.LastDayID = cmd.Table.Rows[0]["logindayid"].ToString();
				dbRoleInfo.MyHuodongData.LoginNum = Convert.ToInt32(cmd.Table.Rows[0]["loginnum"].ToString());
				dbRoleInfo.MyHuodongData.NewStep = Convert.ToInt32(cmd.Table.Rows[0]["newstep"].ToString());
				dbRoleInfo.MyHuodongData.StepTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["steptime"].ToString());
				dbRoleInfo.MyHuodongData.LastMTime = Convert.ToInt32(cmd.Table.Rows[0]["lastmtime"].ToString());
				dbRoleInfo.MyHuodongData.CurMID = cmd.Table.Rows[0]["curmid"].ToString();
				dbRoleInfo.MyHuodongData.CurMTime = Convert.ToInt32(cmd.Table.Rows[0]["curmtime"].ToString());
				dbRoleInfo.MyHuodongData.SongLiID = Convert.ToInt32(cmd.Table.Rows[0]["songliid"].ToString());
				dbRoleInfo.MyHuodongData.LoginGiftState = Convert.ToInt32(cmd.Table.Rows[0]["logingiftstate"].ToString());
				dbRoleInfo.MyHuodongData.OnlineGiftState = Convert.ToInt32(cmd.Table.Rows[0]["onlinegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeHuoDongID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimehuodongid"].ToString());
				dbRoleInfo.MyHuodongData.LastLimitTimeDayID = Convert.ToInt32(cmd.Table.Rows[0]["lastlimittimedayid"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeLoginNum = Convert.ToInt32(cmd.Table.Rows[0]["limittimeloginnum"].ToString());
				dbRoleInfo.MyHuodongData.LimitTimeGiftState = Convert.ToInt32(cmd.Table.Rows[0]["limittimegiftstate"].ToString());
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["everydayonlineawardstep"].ToString());
				dbRoleInfo.MyHuodongData.GetEveryDayOnLineAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["geteverydayonlineawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginGetAwardStep = Convert.ToInt32(cmd.Table.Rows[0]["serieslogingetawardstep"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardDayID = Convert.ToInt32(cmd.Table.Rows[0]["seriesloginawarddayid"].ToString());
				dbRoleInfo.MyHuodongData.SeriesLoginAwardGoodsID = cmd.Table.Rows[0]["seriesloginawardgoodsid"].ToString();
				dbRoleInfo.MyHuodongData.EveryDayOnLineAwardGoodsID = cmd.Table.Rows[0]["everydayonlineawardgoodsid"].ToString();
			}
		}

		public static void DBTableRow2RoleInfo_FuBenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.FuBenDataList = new List<FuBenData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.FuBenDataList.Add(new FuBenData
					{
						FuBenID = Convert.ToInt32(cmd.Table.Rows[i]["fubenid"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						EnterNum = Convert.ToInt32(cmd.Table.Rows[i]["enternum"].ToString()),
						QuickPassTimer = Convert.ToInt32(cmd.Table.Rows[i]["quickpasstimer"].ToString()),
						FinishNum = Convert.ToInt32(cmd.Table.Rows[i]["finishnum"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_HolyItemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyHolyItemDataDic = new Dictionary<sbyte, HolyItemData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					HolyItemData holyItemData = null;
					sbyte b = Convert.ToSByte(cmd.Table.Rows[i]["shengwu_type"].ToString());
					sbyte key = Convert.ToSByte(cmd.Table.Rows[i]["part_slot"].ToString());
					bool flag = dbRoleInfo.MyHolyItemDataDic.TryGetValue(b, out holyItemData);
					if (!flag)
					{
						holyItemData = new HolyItemData();
					}
					holyItemData.m_sType = b;
					HolyItemPartData holyItemPartData = null;
					if (!holyItemData.m_PartArray.TryGetValue(key, out holyItemPartData))
					{
						holyItemPartData = new HolyItemPartData();
						holyItemData.m_PartArray.Add(key, holyItemPartData);
					}
					holyItemPartData.m_sSuit = Convert.ToSByte(cmd.Table.Rows[i]["part_suit"].ToString());
					holyItemPartData.m_nSlice = Convert.ToInt32(cmd.Table.Rows[i]["part_slice"].ToString());
					if (!flag)
					{
						dbRoleInfo.MyHolyItemDataDic.Add(b, holyItemData);
					}
				}
			}
		}

		public static void DBTableRow2RoleInfo_TarotData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int roleId)
		{
			dbRoleInfo.TarotData = new TarotSystemData();
			string text = string.Format("SELECT tarotinfo,kingbuff FROM t_tarot where roleid={0}", roleId);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
			try
			{
				MySQLCommand mySQLCommand = new MySQLCommand(text, connection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					string @string = Encoding.UTF8.GetString(mySQLDataReader["tarotinfo"] as byte[]);
					string string2 = Encoding.UTF8.GetString(mySQLDataReader["kingbuff"] as byte[]);
					string[] array = @string.Split(new char[]
					{
						';'
					}, StringSplitOptions.RemoveEmptyEntries);
					foreach (string data in array)
					{
						TarotCardData item = new TarotCardData(data);
						dbRoleInfo.TarotData.TarotCardDatas.Add(item);
					}
					dbRoleInfo.TarotData.KingData = new TarotKingData(string2);
				}
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static void DBTableRow2RoleInfo_MarriageData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyMarriageData = new MarriageData
				{
					nSpouseID = Convert.ToInt32(cmd.Table.Rows[0]["spouseid"].ToString()),
					byMarrytype = Convert.ToSByte(cmd.Table.Rows[0]["marrytype"].ToString()),
					nRingID = Convert.ToInt32(cmd.Table.Rows[0]["ringid"].ToString()),
					nGoodwillexp = Convert.ToInt32(cmd.Table.Rows[0]["goodwillexp"].ToString()),
					byGoodwillstar = Convert.ToSByte(cmd.Table.Rows[0]["goodwillstar"].ToString()),
					byGoodwilllevel = Convert.ToSByte(cmd.Table.Rows[0]["goodwilllevel"].ToString()),
					nGivenrose = Convert.ToInt32(cmd.Table.Rows[0]["givenrose"].ToString()),
					strLovemessage = cmd.Table.Rows[0]["lovemessage"].ToString(),
					byAutoReject = Convert.ToSByte(cmd.Table.Rows[0]["autoreject"].ToString()),
					ChangTime = cmd.Table.Rows[0]["changtime"].ToString()
				};
			}
			else
			{
				dbRoleInfo.MyMarriageData = new MarriageData();
			}
		}

		public static void DBTableRow2RoleInfo_MarryPartyJoinList(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MyMarryPartyJoinList = new Dictionary<int, int>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MyMarryPartyJoinList.Add(Convert.ToInt32(cmd.Table.Rows[i]["partyroleid"].ToString()), Convert.ToInt32(cmd.Table.Rows[i]["joincount"].ToString()));
				}
			}
		}

		public static void DBTableRow2RoleInfo_DailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyRoleDailyData = new RoleDailyData
				{
					ExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["expdayid"].ToString()),
					TodayExp = Convert.ToInt32(cmd.Table.Rows[0]["todayexp"].ToString()),
					LingLiDayID = Convert.ToInt32(cmd.Table.Rows[0]["linglidayid"].ToString()),
					TodayLingLi = Convert.ToInt32(cmd.Table.Rows[0]["todaylingli"].ToString()),
					KillBossDayID = Convert.ToInt32(cmd.Table.Rows[0]["killbossdayid"].ToString()),
					TodayKillBoss = Convert.ToInt32(cmd.Table.Rows[0]["todaykillboss"].ToString()),
					FuBenDayID = Convert.ToInt32(cmd.Table.Rows[0]["fubendayid"].ToString()),
					TodayFuBenNum = Convert.ToInt32(cmd.Table.Rows[0]["todayfubennum"].ToString()),
					WuXingDayID = Convert.ToInt32(cmd.Table.Rows[0]["wuxingdayid"].ToString()),
					WuXingNum = Convert.ToInt32(cmd.Table.Rows[0]["wuxingnum"].ToString()),
					RebornExpDayID = Convert.ToInt32(cmd.Table.Rows[0]["reborndayid"].ToString()),
					RebornExpMonster = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpmonster"].ToString()),
					RebornExpSale = Convert.ToInt32(cmd.Table.Rows[0]["rebornexpsale"].ToString())
				};
			}
		}

		public static void DBTableRow2RoleInfo_YaBiaoData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyYaBiaoData = new YaBiaoData
				{
					YaBiaoID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaoid"].ToString()),
					StartTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["starttime"].ToString()),
					State = Convert.ToInt32(cmd.Table.Rows[0]["state"].ToString()),
					LineID = Convert.ToInt32(cmd.Table.Rows[0]["lineid"].ToString()),
					TouBao = Convert.ToInt32(cmd.Table.Rows[0]["toubao"].ToString()),
					YaBiaoDayID = Convert.ToInt32(cmd.Table.Rows[0]["yabiaodayid"].ToString()),
					YaBiaoNum = Convert.ToInt32(cmd.Table.Rows[0]["yabiaonum"].ToString()),
					TakeGoods = Convert.ToInt32(cmd.Table.Rows[0]["takegoods"].ToString())
				};
			}
		}

		public static void DBTableRow2RoleInfo_VipDailyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.VipDailyDataList = new List<VipDailyData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.VipDailyDataList.Add(new VipDailyData
					{
						PriorityType = Convert.ToInt32(cmd.Table.Rows[i]["prioritytype"].ToString()),
						DayID = Convert.ToInt32(cmd.Table.Rows[i]["dayid"].ToString()),
						UsedTimes = Convert.ToInt32(cmd.Table.Rows[i]["usedtimes"].ToString())
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_YangGongBKDailyJiFenData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.YangGongBKDailyJiFen = new YangGongBKDailyJiFenData
				{
					JiFen = Convert.ToInt32(cmd.Table.Rows[0]["jifen"].ToString()),
					DayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString()),
					AwardHistory = Convert.ToInt64(cmd.Table.Rows[0]["awardhistory"].ToString())
				};
			}
		}

		public static void DBTableRow2RoleInfo_Wings(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyWingData = new WingData
				{
					DbID = Convert.ToInt32(cmd.Table.Rows[0]["Id"].ToString()),
					WingID = Convert.ToInt32(cmd.Table.Rows[0]["wingid"].ToString()),
					ForgeLevel = Convert.ToInt32(cmd.Table.Rows[0]["forgeLevel"].ToString()),
					AddDateTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addtime"].ToString()),
					JinJieFailedNum = Convert.ToInt32(cmd.Table.Rows[0]["failednum"].ToString()),
					Using = Convert.ToInt32(cmd.Table.Rows[0]["equiped"].ToString()),
					StarExp = Convert.ToInt32(cmd.Table.Rows[0]["starexp"].ToString()),
					ZhuLingNum = Convert.ToInt32(cmd.Table.Rows[0]["zhulingnum"].ToString()),
					ZhuHunNum = Convert.ToInt32(cmd.Table.Rows[0]["zhuhunnum"].ToString())
				};
			}
		}

		public static void DBTableRow2RoleInfo_picturejudgeinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int key = Convert.ToInt32(cmd.Table.Rows[i]["picturejudgeid"].ToString());
					int value = Convert.ToInt32(cmd.Table.Rows[i]["refercount"].ToString());
					dictionary[key] = value;
				}
				dbRoleInfo.PictureJudgeReferInfo = dictionary;
			}
		}

		public static void DBTableRow2RoleInfo_starconstellationinfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				Dictionary<int, int> dictionary = new Dictionary<int, int>(cmd.Table.Rows.Count);
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int key = Convert.ToInt32(cmd.Table.Rows[i]["starsiteid"].ToString());
					int value = Convert.ToInt32(cmd.Table.Rows[i]["starslotid"].ToString());
					dictionary[key] = value;
				}
				dbRoleInfo.StarConstellationInfo = dictionary;
			}
		}

		public static void DBTableRow2RoleInfo_LingYuInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.LingYuDict = new Dictionary<int, LingYuData>();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				LingYuData lingYuData = new LingYuData();
				lingYuData.Type = Convert.ToInt32(cmd.Table.Rows[i]["type"].ToString());
				lingYuData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
				lingYuData.Suit = Convert.ToInt32(cmd.Table.Rows[i]["suit"].ToString());
				dbRoleInfo.LingYuDict[lingYuData.Type] = lingYuData;
			}
		}

		public static void DBTableRow2RoleInfo_GuardStatue(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Level = Convert.ToInt32(cmd.Table.Rows[0]["level"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.Suit = Convert.ToInt32(cmd.Table.Rows[0]["suit"].ToString());
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.HasGuardPoint = Convert.ToInt32(cmd.Table.Rows[0]["total_guard_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.ActiveSoulSlot = Convert.ToInt32(cmd.Table.Rows[0]["slot_cnt"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverPoint = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_point"].ToString());
				dbRoleInfo.MyGuardStatueDetail.LastdayRecoverOffset = Convert.ToInt32(cmd.Table.Rows[0]["lastday_recover_offset"].ToString());
			}
		}

		public static void DBTableRow2RoleInfo_GuardSoul(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				if (dbRoleInfo.MyGuardStatueDetail == null)
				{
					dbRoleInfo.MyGuardStatueDetail = new GuardStatueDetail();
					dbRoleInfo.MyGuardStatueDetail.IsActived = true;
				}
				dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Clear();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					GuardSoulData guardSoulData = new GuardSoulData();
					guardSoulData.Type = Convert.ToInt32(cmd.Table.Rows[i]["soul_type"].ToString());
					guardSoulData.EquipSlot = Convert.ToInt32(cmd.Table.Rows[i]["equip_slot"].ToString());
					dbRoleInfo.MyGuardStatueDetail.GuardStatue.GuardSoulList.Add(guardSoulData);
				}
			}
		}

		public static int QueryRoleID_ByRolename(MySQLConnection conn, string strRoleName)
		{
			List<Tuple<int, string>> list = DBRoleInfo.QueryRoleIdList_ByRolename_IgnoreDbCmp(conn, strRoleName);
			int result = -1;
			if (list != null)
			{
				Tuple<int, string> tuple = list.Find((Tuple<int, string> _t) => _t.Item2 == strRoleName);
				result = ((tuple != null) ? tuple.Item1 : -1);
			}
			return result;
		}

		public static List<Tuple<int, string>> QueryRoleIdList_ByRolename_IgnoreDbCmp(MySQLConnection conn, string rolename)
		{
			List<Tuple<int, string>> list = new List<Tuple<int, string>>();
			string text = string.Format("SELECT rid,rname FROM t_roles where rname='{0}'", rolename);
			GameDBManager.SystemServerSQLEvents.AddEvent(string.Format("+SQL: {0}", text), EventLevels.Important);
			MySQLCommand mySQLCommand = new MySQLCommand(text, conn);
			MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
			while (mySQLDataReader.Read())
			{
				int item = Convert.ToInt32(mySQLDataReader["rid"].ToString());
				string item2 = mySQLDataReader["rname"].ToString();
				list.Add(new Tuple<int, string>(item, item2));
			}
			mySQLCommand.Dispose();
			return list;
		}

		public static void DBTableRow2RoleInfo_GMailInfo(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.GroupMailRecordList = new List<int>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.GroupMailRecordList.Add(Convert.ToInt32(cmd.Table.Rows[i]["gmailid"].ToString()));
				}
			}
		}

		public static void DBTableRow2RoleInfo_TalentBase(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MyTalentData = new TalentData
				{
					IsOpen = true,
					TotalCount = Convert.ToInt32(cmd.Table.Rows[0]["tatalCount"].ToString()),
					Exp = Convert.ToInt64(cmd.Table.Rows[0]["exp"].ToString())
				};
			}
			else
			{
				dbRoleInfo.MyTalentData = new TalentData();
			}
		}

		public static void DBTableRow2RoleInfo_TalentEffects(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			dictionary.Add(1, 0);
			dictionary.Add(2, 0);
			dictionary.Add(3, 0);
			dbRoleInfo.MyTalentData.EffectList = new List<TalentEffectItem>();
			if (dbRoleInfo.MyTalentData != null && cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					int num = Convert.ToInt32(cmd.Table.Rows[i]["talentType"].ToString());
					int id = Convert.ToInt32(cmd.Table.Rows[i]["effectID"].ToString());
					int num2 = Convert.ToInt32(cmd.Table.Rows[i]["effectLevel"].ToString());
					TalentEffectItem item = new TalentEffectItem
					{
						ID = id,
						Level = num2,
						TalentType = num
					};
					dbRoleInfo.MyTalentData.EffectList.Add(item);
					Dictionary<int, int> dictionary2;
					int key;
					(dictionary2 = dictionary)[key = num] = dictionary2[key] + num2;
				}
			}
			dbRoleInfo.MyTalentData.CountList = dictionary;
		}

		public static void DBTableRow2RoleInfo_TianTiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			RoleTianTiData roleTianTiData = new RoleTianTiData
			{
				RoleId = dbRoleInfo.RoleID
			};
			dbRoleInfo.TianTiData = roleTianTiData;
			if (cmd.Table.Rows.Count > 0)
			{
				roleTianTiData.DuanWeiId = Convert.ToInt32(cmd.Table.Rows[0]["duanweiid"].ToString());
				roleTianTiData.DuanWeiJiFen = Convert.ToInt32(cmd.Table.Rows[0]["duanweijifen"].ToString());
				roleTianTiData.DuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["duanweirank"].ToString());
				roleTianTiData.LianSheng = Convert.ToInt32(cmd.Table.Rows[0]["liansheng"].ToString());
				roleTianTiData.FightCount = Convert.ToInt32(cmd.Table.Rows[0]["fightcount"].ToString());
				roleTianTiData.SuccessCount = Convert.ToInt32(cmd.Table.Rows[0]["successcount"].ToString());
				roleTianTiData.TodayFightCount = Convert.ToInt32(cmd.Table.Rows[0]["todayfightcount"].ToString());
				roleTianTiData.LastFightDayId = Convert.ToInt32(cmd.Table.Rows[0]["lastfightdayid"].ToString());
				roleTianTiData.MonthDuanWeiRank = Convert.ToInt32(cmd.Table.Rows[0]["monthduanweirank"].ToString());
				DateTime.TryParse(cmd.Table.Rows[0]["fetchmonthawarddate"].ToString(), out roleTianTiData.FetchMonthDuanWeiRankAwardsTime);
				roleTianTiData.RongYao = Convert.ToInt32(cmd.Table.Rows[0]["rongyao"].ToString());
			}
		}

		public static void DBTableRow2RoleInfo_MerlinData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.MerlinData)
			{
				dbRoleInfo.MerlinData = new MerlinGrowthSaveDBData();
				for (int i = 0; i < 4; i++)
				{
					dbRoleInfo.MerlinData._ActiveAttr[i] = 0.0;
					dbRoleInfo.MerlinData._UnActiveAttr[i] = 0.0;
				}
			}
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.MerlinData._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[0]["roleID"].ToString(), 10);
				dbRoleInfo.MerlinData._Occupation = Global.SafeConvertToInt32(cmd.Table.Rows[0]["occupation"].ToString(), 10);
				dbRoleInfo.MerlinData._Level = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level"].ToString(), 10);
				dbRoleInfo.MerlinData._LevelUpFailNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["level_up_fail_num"].ToString(), 10);
				dbRoleInfo.MerlinData._StarNum = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starNum"].ToString(), 10);
				dbRoleInfo.MerlinData._StarExp = Global.SafeConvertToInt32(cmd.Table.Rows[0]["starExp"].ToString(), 10);
				dbRoleInfo.MerlinData._LuckyPoint = Global.SafeConvertToInt32(cmd.Table.Rows[0]["luckyPoint"].ToString(), 10);
				dbRoleInfo.MerlinData._ToTicks = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["toTicks"].ToString());
				dbRoleInfo.MerlinData._AddTime = DataHelper.ConvertToTicks(cmd.Table.Rows[0]["addTime"].ToString());
				dbRoleInfo.MerlinData._ActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._ActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["activeBlow"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[0] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveFrozen"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[1] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActivePalsy"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[2] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveSpeedDown"].ToString(), 10) / 100);
				dbRoleInfo.MerlinData._UnActiveAttr[3] = (double)(Global.SafeConvertToInt32(cmd.Table.Rows[0]["unActiveBlow"].ToString(), 10) / 100);
			}
		}

		public static void DBTableRow2RoleInfo_FluorescentGemData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (null == dbRoleInfo.FluorescentGemData)
			{
				dbRoleInfo.FluorescentGemData = new FluorescentGemData();
			}
			dbRoleInfo.FluorescentGemData.GemBagList.Clear();
			dbRoleInfo.FluorescentGemData.GemEquipList.Clear();
			HashSet<int> hashSet = new HashSet<int>();
			FluorescentGemSaveDBData fluorescentGemSaveDBData = new FluorescentGemSaveDBData();
			for (int i = 0; i < cmd.Table.Rows.Count; i++)
			{
				ulong id = Convert.ToUInt64(cmd.Table.Rows[i]["id"].ToString());
				fluorescentGemSaveDBData._RoleID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["roleid"].ToString(), 10);
				fluorescentGemSaveDBData._GoodsID = Global.SafeConvertToInt32(cmd.Table.Rows[i]["goodsid"].ToString(), 10);
				fluorescentGemSaveDBData._Position = Global.SafeConvertToInt32(cmd.Table.Rows[i]["position"].ToString(), 10);
				fluorescentGemSaveDBData._GemType = Global.SafeConvertToInt32(cmd.Table.Rows[i]["type"].ToString(), 10);
				fluorescentGemSaveDBData._Bind = Global.SafeConvertToInt32(cmd.Table.Rows[i]["bind"].ToString(), 10);
				int num = FluorescentGemManager.getInstance().GenerateBagIndex(fluorescentGemSaveDBData._Position, fluorescentGemSaveDBData._GemType);
				if (!hashSet.Contains(num))
				{
					GoodsData goodsData = new GoodsData();
					goodsData.GoodsID = fluorescentGemSaveDBData._GoodsID;
					goodsData.GCount = 1;
					goodsData.Binding = fluorescentGemSaveDBData._Bind;
					goodsData.Site = 7001;
					hashSet.Add(num);
					goodsData.BagIndex = num;
					dbRoleInfo.FluorescentGemData.GemEquipList.Add(goodsData);
				}
				else
				{
					FluorescentGemDBOperate.ForceUnEquipFluorescentGem(DBManager.getInstance(), id);
					LogManager.WriteLog(LogTypes.Error, string.Format("荧光宝石装备栏位置重复，强制删除，rid={0}, goodsid={1}, pos={2}, type={3}, bind={4}", new object[]
					{
						fluorescentGemSaveDBData._RoleID,
						fluorescentGemSaveDBData._GoodsID,
						fluorescentGemSaveDBData._Position,
						fluorescentGemSaveDBData._GemType,
						fluorescentGemSaveDBData._Bind
					}), null, true);
				}
			}
		}

		public static void DBTableRow2RoleInfo_BuildingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.BuildingDataList = new List<BuildingData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					BuildingData buildingData = new BuildingData();
					buildingData.BuildId = Convert.ToInt32(cmd.Table.Rows[i]["buildid"].ToString());
					buildingData.TaskID_1 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_1"].ToString());
					buildingData.TaskID_2 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_2"].ToString());
					buildingData.TaskID_3 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_3"].ToString());
					buildingData.TaskID_4 = Convert.ToInt32(cmd.Table.Rows[i]["taskid_4"].ToString());
					buildingData.BuildLev = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					buildingData.BuildExp = Convert.ToInt32(cmd.Table.Rows[i]["exp"].ToString());
					buildingData.BuildTime = cmd.Table.Rows[i]["developtime"].ToString();
					dbRoleInfo.BuildingDataList.Add(buildingData);
				}
			}
		}

		public static void DBTableRow2RoleInfo_OrnamentData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.OrnamentDataDict = new Dictionary<int, OrnamentData>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					OrnamentData ornamentData = new OrnamentData();
					ornamentData.ID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString());
					ornamentData.Param1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					ornamentData.Param2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					dbRoleInfo.OrnamentDataDict[ornamentData.ID] = ornamentData;
				}
			}
		}

		public static void DBTableRow2RoleInfo_SevenDayActData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.SevenDayActDict = new Dictionary<int, Dictionary<int, SevenDayItemData>>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SevenDayItemData sevenDayItemData = new SevenDayItemData();
					sevenDayItemData.AwardFlag = Convert.ToInt32(cmd.Table.Rows[i]["award_flag"].ToString());
					sevenDayItemData.Params1 = Convert.ToInt32(cmd.Table.Rows[i]["param1"].ToString());
					sevenDayItemData.Params2 = Convert.ToInt32(cmd.Table.Rows[i]["param2"].ToString());
					int num = Convert.ToInt32(cmd.Table.Rows[i]["roleid"].ToString());
					int key = Convert.ToInt32(cmd.Table.Rows[i]["act_type"].ToString());
					int key2 = Convert.ToInt32(cmd.Table.Rows[i]["id"].ToString());
					Dictionary<int, SevenDayItemData> dictionary = null;
					if (!dbRoleInfo.SevenDayActDict.TryGetValue(key, out dictionary))
					{
						dictionary = new Dictionary<int, SevenDayItemData>();
						dbRoleInfo.SevenDayActDict[key] = dictionary;
					}
					dictionary[key2] = sevenDayItemData;
				}
			}
		}

		public static void DBTableRow2RoleInfo_SpecialActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecActInfoDict = new Dictionary<int, SpecActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecActInfoDB specActInfoDB = new SpecActInfoDB();
					specActInfoDB.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					specActInfoDB.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					specActInfoDB.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					specActInfoDB.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					specActInfoDB.Active = Convert.ToInt16(cmd.Table.Rows[i]["active"].ToString());
					dbRoleInfo.SpecActInfoDict[specActInfoDB.ActID] = specActInfoDB;
				}
			}
		}

		public static void DBTableRow2RoleInfo_SpecialPriorityActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.SpecPriorityActInfoDict = new Dictionary<KeyValuePair<int, int>, SpecPriorityActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					SpecPriorityActInfoDB specPriorityActInfoDB = new SpecPriorityActInfoDB();
					specPriorityActInfoDB.TeQuanID = Convert.ToInt32(cmd.Table.Rows[i]["tequanid"].ToString());
					specPriorityActInfoDB.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					specPriorityActInfoDB.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					specPriorityActInfoDB.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					KeyValuePair<int, int> key = new KeyValuePair<int, int>(specPriorityActInfoDB.TeQuanID, specPriorityActInfoDB.ActID);
					dbRoleInfo.SpecPriorityActInfoDict[key] = specPriorityActInfoDB;
				}
			}
		}

		public static void DBTableRow2RoleInfo_EverydayActivityData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.EverydayActInfoDict = new Dictionary<int, EverydayActInfoDB>();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					EverydayActInfoDB everydayActInfoDB = new EverydayActInfoDB();
					everydayActInfoDB.GroupID = Convert.ToInt32(cmd.Table.Rows[i]["groupid"].ToString());
					everydayActInfoDB.ActID = Convert.ToInt32(cmd.Table.Rows[i]["actid"].ToString());
					everydayActInfoDB.PurNum = Convert.ToInt32(cmd.Table.Rows[i]["purchaseNum"].ToString());
					everydayActInfoDB.CountNum = Convert.ToInt32(cmd.Table.Rows[i]["countNum"].ToString());
					everydayActInfoDB.ActiveDay = (int)Convert.ToInt16(cmd.Table.Rows[i]["activeDay"].ToString());
					dbRoleInfo.EverydayActInfoDict[everydayActInfoDB.ActID] = everydayActInfoDB;
				}
			}
		}

		public static void DBTableRow2RoleInfo_AlchemyData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				dbRoleInfo.AlchemyInfo = new AlchemyDataDB();
				dbRoleInfo.AlchemyInfo.RoleID = dbRoleInfo.RoleID;
				dbRoleInfo.AlchemyInfo.BaseData.Element = Convert.ToInt32(cmd.Table.Rows[0]["element"].ToString());
				dbRoleInfo.AlchemyInfo.ElementDayID = Convert.ToInt32(cmd.Table.Rows[0]["dayid"].ToString());
				dbRoleInfo.AlchemyInfo.rollbackType = cmd.Table.Rows[0]["rollback"].ToString();
				string text = cmd.Table.Rows[0]["value"].ToString();
				if (!string.IsNullOrEmpty(text))
				{
					string[] array = text.Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						if (array3.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.AlchemyValue[Global.SafeConvertToInt32(array3[0], 10)] = Global.SafeConvertToInt32(array3[1], 10);
						}
					}
				}
				string text3 = cmd.Table.Rows[0]["todaycost"].ToString();
				if (!string.IsNullOrEmpty(text3))
				{
					string[] array4 = text3.Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array4)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						if (array3.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.BaseData.ToDayCost[Global.SafeConvertToInt32(array3[0], 10)] = Global.SafeConvertToInt32(array3[1], 10);
						}
					}
				}
				string text4 = cmd.Table.Rows[0]["histcost"].ToString();
				if (!string.IsNullOrEmpty(text4))
				{
					string[] array4 = text4.Split(new char[]
					{
						'|'
					});
					foreach (string text2 in array4)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						if (array3.Length == 2)
						{
							dbRoleInfo.AlchemyInfo.HistCost[Global.SafeConvertToInt32(array3[0], 10)] = Global.SafeConvertToInt32(array3[1], 10);
						}
					}
				}
			}
		}

		public static void DBTableRow2RoleInfo_ShenJiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.ShenJiDict = new Dictionary<int, ShenJiFuWenData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					ShenJiFuWenData shenJiFuWenData = new ShenJiFuWenData();
					shenJiFuWenData.ShenJiID = Convert.ToInt32(cmd.Table.Rows[i]["sjID"].ToString());
					shenJiFuWenData.Level = Convert.ToInt32(cmd.Table.Rows[i]["level"].ToString());
					dbRoleInfo.ShenJiDict[shenJiFuWenData.ShenJiID] = shenJiFuWenData;
				}
			}
		}

		public static void DBTableRow2RoleInfo_FuWenData(MySQLConnection connection, DBRoleInfo dbRoleInfo, int rid)
		{
			dbRoleInfo.FuWenTabList = new List<FuWenTabData>();
			string text = string.Format("SELECT * FROM t_fuwen where rid={0}", rid);
			try
			{
				MySQLCommand mySQLCommand = new MySQLCommand(text, connection);
				MySQLDataReader mySQLDataReader = mySQLCommand.ExecuteReaderEx();
				while (mySQLDataReader.Read())
				{
					string @string = Encoding.UTF8.GetString(mySQLDataReader["fuwenequip"] as byte[]);
					string text2 = mySQLDataReader["shenshiactive"].ToString();
					List<FuWenTabData> fuWenTabList = dbRoleInfo.FuWenTabList;
					FuWenTabData fuWenTabData = new FuWenTabData();
					fuWenTabData.TabID = Convert.ToInt32(mySQLDataReader["tabid"].ToString());
					fuWenTabData.Name = mySQLDataReader["name"].ToString();
					FuWenTabData fuWenTabData2 = fuWenTabData;
					List<int> fuWenEquipList;
					if (!(@string == ""))
					{
						fuWenEquipList = Array.ConvertAll<string, int>(@string.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						fuWenEquipList = new List<int>();
					}
					fuWenTabData2.FuWenEquipList = fuWenEquipList;
					FuWenTabData fuWenTabData3 = fuWenTabData;
					List<int> shenShiActiveList;
					if (!(text2 == ""))
					{
						shenShiActiveList = Array.ConvertAll<string, int>(text2.Split(new char[]
						{
							','
						}), (string x) => Convert.ToInt32(x)).ToList<int>();
					}
					else
					{
						shenShiActiveList = new List<int>();
					}
					fuWenTabData3.ShenShiActiveList = shenShiActiveList;
					fuWenTabData.SkillEquip = Convert.ToInt32(mySQLDataReader["skillequip"].ToString());
					fuWenTabData.OwnerID = Convert.ToInt32(mySQLDataReader["rid"].ToString());
					fuWenTabList.Add(fuWenTabData);
				}
				mySQLCommand.Dispose();
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static void DBTableRow2RoleInfo_JueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.JueXingTaoZhuangList = new List<TaoZhuangData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					List<TaoZhuangData> jueXingTaoZhuangList = dbRoleInfo.JueXingTaoZhuangList;
					TaoZhuangData taoZhuangData = new TaoZhuangData();
					taoZhuangData.ID = Convert.ToInt32(cmd.Table.Rows[i]["suitid"].ToString());
					taoZhuangData.ActiviteList = Array.ConvertAll<string, int>(cmd.Table.Rows[i]["activite"].ToString().Split(new char[]
					{
						','
					}), (string x) => Convert.ToInt32(x)).ToList<int>();
					jueXingTaoZhuangList.Add(taoZhuangData);
				}
			}
		}

		public static void DBTableRow2RoleInfo_ZuoQiData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			dbRoleInfo.MountList = new List<MountData>();
			if (cmd.Table.Rows.Count > 0)
			{
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					dbRoleInfo.MountList.Add(new MountData
					{
						GoodsID = Convert.ToInt32(cmd.Table.Rows[i]["goodsid"].ToString()),
						IsNew = (cmd.Table.Rows[i]["isnew"].ToString() == "1")
					});
				}
			}
		}

		public static void DBTableRow2RoleInfo_JingLingYuanSuJueXingData(DBRoleInfo dbRoleInfo, MySQLSelectCommand cmd)
		{
			if (cmd.Table.Rows.Count > 0)
			{
				JingLingYuanSuJueXingData jingLingYuanSuJueXingData = new JingLingYuanSuJueXingData();
				for (int i = 0; i < cmd.Table.Rows.Count; i++)
				{
					jingLingYuanSuJueXingData.ActiveType = Convert.ToInt32(cmd.Table.Rows[i]["activetype"].ToString());
					string text = cmd.Table.Rows[i]["activeids"].ToString();
					if (text.Length == 0)
					{
						return;
					}
					string[] array = text.Split(new char[]
					{
						','
					});
					jingLingYuanSuJueXingData.ActiveIDs = new int[array.Length];
					for (int j = 0; j < array.Length; j++)
					{
						int.TryParse(array[j], out jingLingYuanSuJueXingData.ActiveIDs[j]);
					}
				}
				dbRoleInfo.JingLingYuanSuJueXingData = jingLingYuanSuJueXingData;
			}
		}

		public bool Query(MySQLConnection conn, int roleID, bool bUseIsdel = true, int tempRoleID = 0)
		{
			LogManager.WriteLog(LogTypes.Info, string.Format("从数据库加载角色数据: {0}", roleID), null, true);
			MySQLSelectCommand mySQLSelectCommand;
			if (bUseIsdel)
			{
				string[] array = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] array2 = new string[]
				{
					"t_roles"
				};
				object[,] array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array, array2, array3, null, null);
			}
			else
			{
				string[] array4 = new string[]
				{
					"rid",
					"userid",
					"rname",
					"sex",
					"occupation",
					"level",
					"pic",
					"faction",
					"money1",
					"money2",
					"experience",
					"pkmode",
					"pkvalue",
					"position",
					"regtime",
					"lasttime",
					"bagnum",
					"othername",
					"main_quick_keys",
					"other_quick_keys",
					"loginnum",
					"leftfightsecs",
					"horseid",
					"petid",
					"interpower",
					"totalonlinesecs",
					"antiaddictionsecs",
					"logofftime",
					"biguantime",
					"yinliang",
					"total_jingmai_exp",
					"jingmai_exp_num",
					"lasthorseid",
					"skillid",
					"autolife",
					"automagic",
					"numskillid",
					"maintaskid",
					"pkpoint",
					"lianzhan",
					"killboss",
					"battlenamestart",
					"battlenameindex",
					"cztaskid",
					"battlenum",
					"heroindex",
					"logindayid",
					"logindaynum",
					"zoneid",
					"bhname",
					"bhverify",
					"bhzhiwu",
					"bgdayid1",
					"bgmoney",
					"bgdayid2",
					"bggoods",
					"banggong",
					"huanghou",
					"jiebiaodayid",
					"jiebiaonum",
					"username",
					"lastmailid",
					"onceawardflag",
					"banchat",
					"banlogin",
					"isflashplayer",
					"changelifecount",
					"admiredcount",
					"combatforce",
					"autoassignpropertypoint",
					"store_yinliang",
					"store_money",
					"magic_sword_param",
					"fluorescent_point",
					"ban_trade_to_ticks",
					"juntuanzhiwu",
					"huiji",
					"huijiexp",
					"armor",
					"armorexp",
					"bianshen",
					"bianshenexp",
					"reborn_bagnum",
					"reborn_isshow",
					"reborn_isshow_model",
					"zhanduiid",
					"zhanduizhiwu"
				};
				string[] array5 = new string[]
				{
					"t_roles"
				};
				object[,] array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array4, array5, array3, null, null);
			}
			bool result;
			if (mySQLSelectCommand.Table.Rows.Count <= 0)
			{
				result = false;
			}
			else
			{
				this.PTID = GameDBManager.PTID;
				this.WorldRoleID = string.Format("{0}@{1}", roleID, this.PTID);
				DBRoleInfo.DBTableRow2RoleInfo(this, mySQLSelectCommand, 0);
				object[,] array3;
				if (GameDBManager.Flag_Splite_RoleParams_Table == 0)
				{
					string[] array6 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] array7 = new string[]
					{
						"t_roleparams"
					};
					array3 = new object[1, 3];
					array3[0, 0] = "rid";
					array3[0, 1] = "=";
					array3[0, 2] = roleID;
					mySQLSelectCommand = new MySQLSelectCommand(conn, array6, array7, array3, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, mySQLSelectCommand, true);
				}
				else
				{
					string[] array8 = new string[]
					{
						"pname",
						"pvalue"
					};
					string[] array9 = new string[]
					{
						"t_roleparams_2"
					};
					array3 = new object[1, 3];
					array3[0, 0] = "rid";
					array3[0, 1] = "=";
					array3[0, 2] = roleID;
					mySQLSelectCommand = new MySQLSelectCommand(conn, array8, array9, array3, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_Params(this, mySQLSelectCommand, false);
					string[] array10 = new string[]
					{
						"*"
					};
					string[] array11 = new string[]
					{
						"t_roleparams_long"
					};
					array3 = new object[1, 3];
					array3[0, 0] = "rid";
					array3[0, 1] = "=";
					array3[0, 2] = roleID;
					mySQLSelectCommand = new MySQLSelectCommand(conn, array10, array11, array3, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, mySQLSelectCommand);
					string[] array12 = new string[]
					{
						"*"
					};
					string[] array13 = new string[]
					{
						"t_roleparams_char"
					};
					array3 = new object[1, 3];
					array3[0, 0] = "rid";
					array3[0, 1] = "=";
					array3[0, 2] = roleID;
					mySQLSelectCommand = new MySQLSelectCommand(conn, array12, array13, array3, null, null);
					DBRoleInfo.DBTableRow2RoleInfo_ParamsEx(this, mySQLSelectCommand);
				}
				DBRoleInfo.InitFromRoleParams(this);
				string[] array14 = new string[]
				{
					"rid",
					"taskid",
					"count"
				};
				string[] array15 = new string[]
				{
					"t_taskslog"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array14, array15, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OldTasks(this, mySQLSelectCommand);
				string[] array16 = new string[]
				{
					"Id",
					"rid",
					"taskid",
					"focus",
					"value1",
					"value2",
					"addtime",
					"starlevel"
				};
				string[] array17 = new string[]
				{
					"t_tasks"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array16, array17, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DoingTasks(this, mySQLSelectCommand);
				string[] array18 = new string[]
				{
					"id",
					"goodsid",
					"isusing",
					"forge_level",
					"starttime",
					"endtime",
					"site",
					"quality",
					"Props",
					"gcount",
					"binding",
					"jewellist",
					"bagindex",
					"salemoney1",
					"saleyuanbao",
					"saleyinpiao",
					"addpropindex",
					"bornindex",
					"lucky",
					"strong",
					"excellenceinfo",
					"appendproplev",
					"equipchangelife",
					"ehinfo",
					"washprops",
					"juhun"
				};
				string[] array19 = new string[]
				{
					"t_goods"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "gcount";
				array3[1, 1] = ">";
				array3[1, 2] = 0;
				object[,] array20 = array3;
				string[,] array21 = null;
				string[,] array22 = new string[1, 2];
				array22[0, 0] = "id";
				array22[0, 1] = "asc";
				mySQLSelectCommand = new MySQLSelectCommand(conn, array18, array19, array20, array21, array22);
				DBRoleInfo.DBTableRow2RoleInfo_Goods(this, mySQLSelectCommand);
				string[] array23 = new string[]
				{
					"goodsid",
					"dayid",
					"usednum"
				};
				string[] array24 = new string[]
				{
					"t_goodslimit"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array23, array24, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GoodsLimit(this, mySQLSelectCommand);
				string[] array25 = new string[]
				{
					"Id",
					"otherid",
					"friendType"
				};
				string[] array26 = new string[]
				{
					"t_friends"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "myid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array25, array26, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Friends(this, mySQLSelectCommand);
				string[] array27 = new string[]
				{
					"Id",
					"horseid",
					"bodyid",
					"propsNum",
					"PropsVal",
					"addtime",
					"failednum",
					"temptime",
					"tempnum",
					"faileddayid"
				};
				string[] array28 = new string[]
				{
					"t_horses"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array27, array28, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Horses(this, mySQLSelectCommand);
				string[] array29 = new string[]
				{
					"Id",
					"petid",
					"petname",
					"pettype",
					"feednum",
					"realivenum",
					"addtime",
					"props",
					"level"
				};
				string[] array30 = new string[]
				{
					"t_pets"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array29, array30, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Pets(this, mySQLSelectCommand);
				string[] array31 = new string[]
				{
					"Id",
					"jmid",
					"jmlevel",
					"bodylevel"
				};
				string[] array32 = new string[]
				{
					"t_jingmai"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array31, array32, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingMais(this, mySQLSelectCommand);
				string[] array33 = new string[]
				{
					"Id",
					"skillid",
					"skilllevel",
					"usednum"
				};
				string[] array34 = new string[]
				{
					"t_skills"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array33, array34, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Skills(this, mySQLSelectCommand);
				string[] array35 = new string[]
				{
					"bufferid",
					"starttime",
					"buffersecs",
					"bufferval"
				};
				string[] array36 = new string[]
				{
					"t_buffer"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array35, array36, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Buffers(this, mySQLSelectCommand);
				string[] array37 = new string[]
				{
					"huanid",
					"rectime",
					"recnum",
					"taskClass",
					"extdayid",
					"extnum"
				};
				string[] array38 = new string[]
				{
					"t_dailytasks"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array37, array38, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyTasks(this, mySQLSelectCommand);
				string[] array39 = new string[]
				{
					"jmtime",
					"jmnum"
				};
				string[] array40 = new string[]
				{
					"t_dailyjingmai"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array39, array40, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyJingMai(this, mySQLSelectCommand);
				string[] array41 = new string[]
				{
					"extgridnum"
				};
				string[] array42 = new string[]
				{
					"t_ptbag"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array41, array42, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_PortableBag(this, mySQLSelectCommand);
				string[] array43 = new string[]
				{
					"extgridnum"
				};
				string[] array44 = new string[]
				{
					"t_reborn_storage"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array43, array44, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_RebornPortableBag(this, mySQLSelectCommand);
				string[] array45 = new string[]
				{
					"loginweekid",
					"logindayid",
					"loginnum",
					"newstep",
					"steptime",
					"lastmtime",
					"curmid",
					"curmtime",
					"songliid",
					"logingiftstate",
					"onlinegiftstate",
					"lastlimittimehuodongid",
					"lastlimittimedayid",
					"limittimeloginnum",
					"limittimegiftstate",
					"everydayonlineawardstep",
					"geteverydayonlineawarddayid",
					"serieslogingetawardstep",
					"seriesloginawarddayid",
					"seriesloginawardgoodsid",
					"everydayonlineawardgoodsid"
				};
				string[] array46 = new string[]
				{
					"t_huodong"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array45, array46, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HuodongData(this, mySQLSelectCommand);
				string[] array47 = new string[]
				{
					"fubenid",
					"dayid",
					"enternum",
					"quickpasstimer",
					"finishnum"
				};
				string[] array48 = new string[]
				{
					"t_fuben"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array47, array48, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FuBenData(this, mySQLSelectCommand);
				string[] array49 = new string[]
				{
					"spouseid",
					"marrytype",
					"ringid",
					"goodwillexp",
					"goodwillstar",
					"goodwilllevel",
					"givenrose",
					"lovemessage",
					"autoreject",
					"changtime"
				};
				string[] array50 = new string[]
				{
					"t_marry"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array49, array50, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarriageData(this, mySQLSelectCommand);
				string[] array51 = new string[]
				{
					"partyroleid",
					"joincount"
				};
				string[] array52 = new string[]
				{
					"t_marryparty_join"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array51, array52, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MarryPartyJoinList(this, mySQLSelectCommand);
				string[] array53 = new string[]
				{
					"*"
				};
				string[] array54 = new string[]
				{
					"t_holyitem"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array53, array54, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_HolyItemData(this, mySQLSelectCommand);
				string[] array55 = new string[]
				{
					"*"
				};
				string[] array56 = new string[]
				{
					"t_dailydata"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array55, array56, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_DailyData(this, mySQLSelectCommand);
				string[] array57 = new string[]
				{
					"yabiaoid",
					"starttime",
					"state",
					"lineid",
					"toubao",
					"yabiaodayid",
					"yabiaonum",
					"takegoods"
				};
				string[] array58 = new string[]
				{
					"t_yabiao"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array57, array58, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YaBiaoData(this, mySQLSelectCommand);
				string[] array59 = new string[]
				{
					"rid",
					"prioritytype",
					"dayid",
					"usedtimes"
				};
				string[] array60 = new string[]
				{
					"t_vipdailydata"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array59, array60, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_VipDailyData(this, mySQLSelectCommand);
				string[] array61 = new string[]
				{
					"rid",
					"jifen",
					"dayid",
					"awardhistory"
				};
				string[] array62 = new string[]
				{
					"t_yangguangbkdailydata"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array61, array62, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_YangGongBKDailyJiFenData(this, mySQLSelectCommand);
				string[] array63 = new string[]
				{
					"Id",
					"wingid",
					"forgeLevel",
					"addtime",
					"failednum",
					"equiped",
					"starexp",
					"zhulingnum",
					"zhuhunnum"
				};
				string[] array64 = new string[]
				{
					"t_wings"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "isdel";
				array3[1, 1] = "=";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array63, array64, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_Wings(this, mySQLSelectCommand);
				string[] array65 = new string[]
				{
					"Id",
					"roleid",
					"picturejudgeid",
					"refercount"
				};
				string[] array66 = new string[]
				{
					"t_picturejudgeinfo"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array65, array66, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_picturejudgeinfo(this, mySQLSelectCommand);
				string[] array67 = new string[]
				{
					"Id",
					"roleid",
					"starsiteid",
					"starslotid"
				};
				string[] array68 = new string[]
				{
					"t_starconstellationinfo"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array67, array68, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_starconstellationinfo(this, mySQLSelectCommand);
				string[] array69 = new string[]
				{
					"roleid",
					"type",
					"level",
					"suit"
				};
				string[] array70 = new string[]
				{
					"t_lingyu"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array69, array70, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_LingYuInfo(this, mySQLSelectCommand);
				string[] array71 = new string[]
				{
					"roleid",
					"gmailid"
				};
				string[] array72 = new string[]
				{
					"t_rolegmail_record"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array71, array72, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GMailInfo(this, mySQLSelectCommand);
				string[] array73 = new string[]
				{
					"roleid",
					"slot_cnt",
					"level",
					"suit",
					"total_guard_point",
					"lastday_recover_point",
					"lastday_recover_offset"
				};
				string[] array74 = new string[]
				{
					"t_guard_statue"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array73, array74, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardStatue(this, mySQLSelectCommand);
				string[] array75 = new string[]
				{
					"roleid",
					"soul_type",
					"equip_slot"
				};
				string[] array76 = new string[]
				{
					"t_guard_soul"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array75, array76, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_GuardSoul(this, mySQLSelectCommand);
				string[] array77 = new string[]
				{
					"tatalCount",
					"exp"
				};
				string[] array78 = new string[]
				{
					"t_talent"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleID";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array77, array78, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentBase(this, mySQLSelectCommand);
				string[] array79 = new string[]
				{
					"talentType",
					"effectID",
					"effectLevel"
				};
				string[] array80 = new string[]
				{
					"t_talent_effect"
				};
				array3 = new object[2, 3];
				array3[0, 0] = "roleID";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				array3[1, 0] = "effectLevel";
				array3[1, 1] = ">";
				array3[1, 2] = 0;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array79, array80, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TalentEffects(this, mySQLSelectCommand);
				string[] array81 = new string[]
				{
					"duanweiid",
					"duanweijifen",
					"duanweirank",
					"liansheng",
					"fightcount",
					"successcount",
					"todayfightcount",
					"lastfightdayid",
					"monthduanweirank",
					"fetchmonthawarddate",
					"rongyao"
				};
				string[] array82 = new string[]
				{
					"t_kf_tianti_role"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array81, array82, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_TianTiData(this, mySQLSelectCommand);
				string[] array83 = new string[]
				{
					"roleID",
					"occupation",
					"level",
					"level_up_fail_num",
					"starNum",
					"starExp",
					"luckyPoint",
					"toTicks",
					"addTime",
					"activeFrozen",
					"activePalsy",
					"activeSpeedDown",
					"activeBlow",
					"unActiveFrozen",
					"unActivePalsy",
					"unActiveSpeedDown",
					"unActiveBlow"
				};
				string[] array84 = new string[]
				{
					"t_merlin_magic_book"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleID";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array83, array84, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_MerlinData(this, mySQLSelectCommand);
				string[] array85 = new string[]
				{
					"id",
					"roleid",
					"goodsid",
					"position",
					"type",
					"bind"
				};
				string[] array86 = new string[]
				{
					"t_fluorescent_gem_equip"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array85, array86, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_FluorescentGemData(this, mySQLSelectCommand);
				string[] array87 = new string[]
				{
					"buildid",
					"taskid_1",
					"taskid_2",
					"taskid_3",
					"taskid_4",
					"level",
					"exp",
					"developtime"
				};
				string[] array88 = new string[]
				{
					"t_building"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array87, array88, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_BuildingData(this, mySQLSelectCommand);
				string[] array89 = new string[]
				{
					"goodsid",
					"param1",
					"param2"
				};
				string[] array90 = new string[]
				{
					"t_ornament"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array89, array90, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_OrnamentData(this, mySQLSelectCommand);
				string[] array91 = new string[]
				{
					"roleid",
					"act_type",
					"id",
					"award_flag",
					"param1",
					"param2"
				};
				string[] array92 = new string[]
				{
					"t_seven_day_act"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "roleid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array91, array92, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SevenDayActData(this, mySQLSelectCommand);
				string[] array93 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"active"
				};
				string[] array94 = new string[]
				{
					"t_special_activity"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array93, array94, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialActivityData(this, mySQLSelectCommand);
				string[] array95 = new string[]
				{
					"rid",
					"tequanid",
					"actid",
					"purchaseNum",
					"countNum"
				};
				string[] array96 = new string[]
				{
					"t_special_priority_activity"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array95, array96, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_SpecialPriorityActivityData(this, mySQLSelectCommand);
				string[] array97 = new string[]
				{
					"rid",
					"sjID",
					"level"
				};
				string[] array98 = new string[]
				{
					"t_shenjifuwen"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array97, array98, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ShenJiData(this, mySQLSelectCommand);
				DBRoleInfo.DBTableRow2RoleInfo_TarotData(conn, this, roleID);
				string[] array99 = new string[]
				{
					"rid",
					"groupid",
					"actid",
					"purchaseNum",
					"countNum",
					"activeDay"
				};
				string[] array100 = new string[]
				{
					"t_everyday_activity"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array99, array100, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_EverydayActivityData(this, mySQLSelectCommand);
				DBRoleInfo.DBTableRow2RoleInfo_FuWenData(conn, this, roleID);
				string[] array101 = new string[]
				{
					"rid",
					"element",
					"dayid",
					"value",
					"todaycost",
					"histcost",
					"rollback"
				};
				string[] array102 = new string[]
				{
					"t_alchemy"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array101, array102, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_AlchemyData(this, mySQLSelectCommand);
				string[] array103 = new string[]
				{
					"rid",
					"suitid",
					"activite"
				};
				string[] array104 = new string[]
				{
					"t_juexing"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array103, array104, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JueXingData(this, mySQLSelectCommand);
				string[] array105 = new string[]
				{
					"rid",
					"goodsid",
					"isnew"
				};
				string[] array106 = new string[]
				{
					"t_zuoqi"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array105, array106, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_ZuoQiData(this, mySQLSelectCommand);
				string[] array107 = new string[]
				{
					"rid",
					"activetype",
					"activeids"
				};
				string[] array108 = new string[]
				{
					"t_juexing_jlys"
				};
				array3 = new object[1, 3];
				array3[0, 0] = "rid";
				array3[0, 1] = "=";
				array3[0, 2] = roleID;
				mySQLSelectCommand = new MySQLSelectCommand(conn, array107, array108, array3, null, null);
				DBRoleInfo.DBTableRow2RoleInfo_JingLingYuanSuJueXingData(this, mySQLSelectCommand);
				this.RankValue.Init(roleID);
				result = true;
			}
			return result;
		}

		public int PTID;

		public string WorldRoleID;

		public string Channel;

		public int SubOccupation;

		public int ZhanDuiID;

		public int ZhanDuiZhiWu;

		public int JunTuanZhiWu;

		public RoleHuiJiData HuiJiData = new RoleHuiJiData();

		public RoleBianShenData BianShenData = new RoleBianShenData();

		public RoleArmorData ArmorData = new RoleArmorData();

		public List<int> OccupationList = new List<int>();

		public RoleCustomData roleCustomData;

		private UserRankValueCache rankValue = new UserRankValueCache();

		public Dictionary<string, RoleParamsData> RoleParamsDict = new Dictionary<string, RoleParamsData>();

		private long _LastReferenceTicks = DateTime.Now.Ticks / 10000L;

		public RoleTianTiData TianTiData;

		public JingLingYuanSuJueXingData JingLingYuanSuJueXingData;
	}
}
