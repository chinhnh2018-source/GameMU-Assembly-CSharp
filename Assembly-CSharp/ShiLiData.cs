using System;
using System.Collections.Generic;
using GameServer.Logic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Contract.KuaFuData;
using Tmsk.Xml;
using UnityEngine;
using XMLCreater;

public class ShiLiData
{
	public static MUAllComp GetAllComp()
	{
		if (ShiLiData.m_allComp == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/Comp.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/Comp.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allComp = new MUAllComp(gameResXml);
		}
		return ShiLiData.m_allComp;
	}

	public static MUComp GetMUCompById(int id)
	{
		return ShiLiData.GetAllComp().GetCompByCompID(id);
	}

	public static MUComp GetCompByCompMoBaiID(int moBaiId)
	{
		return ShiLiData.GetAllComp().GetCompByCompMoBaiID(moBaiId);
	}

	public static MUComp GetCompByMapCode(int mapCode)
	{
		return ShiLiData.GetAllComp().GetCompByCompMapCode(mapCode);
	}

	public static string GetShiLiNameByType(ShiLiType type)
	{
		string empty = string.Empty;
		MUComp mucompById = ShiLiData.GetMUCompById((int)type);
		if (mucompById == null)
		{
			return string.Empty;
		}
		return mucompById.CompName;
	}

	public static MUAllCompLevel GetAllCompLevel()
	{
		if (ShiLiData.m_allCompLevel == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompLevel.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompLevel.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allCompLevel = new MUAllCompLevel(gameResXml);
		}
		return ShiLiData.m_allCompLevel;
	}

	public static MUCompLevel GetCompLevelByID(int id)
	{
		return ShiLiData.GetAllCompLevel().GetCompLevelByID(id);
	}

	public static MUCompLevel GetCompLevelByCompIDAndLevel(int compID, int level)
	{
		return ShiLiData.GetAllCompLevel().GetCompLevelByCompIDAndLevel(compID, level);
	}

	public static MUCompLevel GetSelfCompLevel()
	{
		return ShiLiData.GetCompLevelByCompIDAndLevel(Global.Data.roleData.CompType, (int)Global.Data.roleData.CompZhiWu);
	}

	public static MUAllCompResources GetAllCompResources()
	{
		if (ShiLiData.m_allResources == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompResources.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompResources.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allResources = new MUAllCompResources(gameResXml);
		}
		return ShiLiData.m_allResources;
	}

	public static MUCompResources GetCompResourcesByID(int id)
	{
		return ShiLiData.GetAllCompResources().GetCompResourcesByID(id);
	}

	public static int GetResourceGrowTime(int monstersID)
	{
		List<MUCompResources> compResources = ShiLiData.GetAllCompResources().CompResources;
		for (int i = 0; i < compResources.Count; i++)
		{
			if (monstersID == compResources[i].NormalMonstersID)
			{
				return compResources[i].GrowTime;
			}
		}
		MUDebug.Log<string>(new string[]
		{
			Global.GetLang("配置文件未找到初始态ID为") + monstersID + Global.GetLang("的资源")
		});
		return int.MaxValue;
	}

	public static MUALLCompNotice GetAllNotices()
	{
		if (ShiLiData.m_allNotices == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompNotice.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompNotice.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allNotices = new MUALLCompNotice(gameResXml);
		}
		return ShiLiData.m_allNotices;
	}

	public static MUCompNotice GetNoticeByID(int id)
	{
		return ShiLiData.GetAllNotices().GetCompNoticeByID(id);
	}

	public static ChangeableRulePart.RuleXml GetCompTaskHelpData()
	{
		if (ShiLiData.m_compTaskHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompIntroTask.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompIntroTask.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_compTaskHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return ShiLiData.m_compTaskHelpData;
	}

	public static ChangeableRulePart.RuleXml GetCompLevelHelpData()
	{
		if (ShiLiData.m_compLevelHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompIntroLevel.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompIntroLevel.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_compLevelHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return ShiLiData.m_compLevelHelpData;
	}

	public static ChangeableRulePart.RuleXml GetCompMapHelpData()
	{
		if (ShiLiData.m_compMapHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/CompIntroMap.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/CompIntroMap.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_compMapHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return ShiLiData.m_compMapHelpData;
	}

	public static ChangeableRulePart.RuleXml GetCompBattleHelpData()
	{
		if (ShiLiData.m_compBattleHelpData == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ForceCraftIntro.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ForceCraftIntro.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_compBattleHelpData = new ChangeableRulePart.RuleXml(gameResXml);
		}
		return ShiLiData.m_compBattleHelpData;
	}

	public static MUForceCraftAll GetAllForceCraft()
	{
		if (ShiLiData.m_allForceCraft == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ForceCraft.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ForceCraft.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allForceCraft = new MUForceCraftAll(gameResXml);
		}
		return ShiLiData.m_allForceCraft;
	}

	public static MUForceCraft GetForceCraftByID(int id)
	{
		return ShiLiData.GetAllForceCraft().GetForceCraftByID(id);
	}

	public static MUForceStrongholdAll GetAllForceStronghold()
	{
		if (ShiLiData.m_allForceStronghold == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ForceStronghold.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ForceStronghold.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allForceStronghold = new MUForceStrongholdAll(gameResXml);
		}
		return ShiLiData.m_allForceStronghold;
	}

	public static MUForceStronghold GetForceStronghold(int id)
	{
		return ShiLiData.GetAllForceStronghold().GetForceStrongholdByID(id);
	}

	public static MUForceStronghold GetForceStrongholdBuQiZuo(int qiZuoId)
	{
		return ShiLiData.GetAllForceStronghold().GetForceStrongholdByQiZuoID(qiZuoId);
	}

	public static MUForceCraftRewardAll GetAllCraftReward()
	{
		if (ShiLiData.m_allCraftReward == null)
		{
			XElement gameResXml = Global.GetGameResXml("Config/ForceCraftReward.xml");
			if (gameResXml == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"加载 Config/ForceCraftReward.xml 出现错误"
				});
				return null;
			}
			ShiLiData.m_allCraftReward = new MUForceCraftRewardAll(gameResXml, "ForceCraftReward");
		}
		return ShiLiData.m_allCraftReward;
	}

	public static void CleanShiLiXMLData()
	{
		ShiLiData.m_allComp = null;
		ShiLiData.m_allCompLevel = null;
		ShiLiData.m_allResources = null;
		ShiLiData.m_allNotices = null;
		ShiLiData.m_compTaskHelpData = null;
		ShiLiData.m_compLevelHelpData = null;
		ShiLiData.m_compMapHelpData = null;
		ShiLiData.m_compBattleHelpData = null;
		ShiLiData.m_allForceCraft = null;
		ShiLiData.m_allForceStronghold = null;
		ShiLiData.m_allCraftReward = null;
		ShiLiData.m_noticeInfos.Clear();
	}

	public static string GetSpecialTitleURL(MUCompLevel zhiwu)
	{
		return ShiLiData.GetSpecialTitleURL(ShiLiData.GetSpecialTitleByZhiWu(zhiwu));
	}

	public static string GetSpecialTitleURL(SpecialTitle specialTitle)
	{
		if (specialTitle == null)
		{
			return string.Empty;
		}
		return "NetImages/GameRes/Images/ChengHaoTeShu/" + specialTitle.IconCode + ".png";
	}

	public static SpecialTitle GetSpecialTitleByZhiWu(MUCompLevel zhiwu)
	{
		if (zhiwu == null)
		{
			return null;
		}
		int buff = zhiwu.Buff;
		return ShiLiData.GetSpecialTitleByBuffId(buff);
	}

	public static SpecialTitle GetSpecialTitleByBuffId(int buffId)
	{
		return Global.TeShuTitleListXml.Find((SpecialTitle info) => info.BuffID == buffId);
	}

	public static void SetselfCompData(CompData data)
	{
		ShiLiData.m_selfCompData = data;
		if (ShiLiData.m_selfCompData.BoomValueList == null)
		{
			ShiLiData.m_selfCompData.BoomValueList = new List<int>();
			ShiLiData.m_selfCompData.BoomValueList.Add(0);
			ShiLiData.m_selfCompData.BoomValueList.Add(0);
			ShiLiData.m_selfCompData.BoomValueList.Add(0);
		}
	}

	private static CompData MockCompData()
	{
		CompData compData = new CompData();
		compData.kfCompData = new KFCompData();
		compData.kfCompData.CompType = 1;
		compData.kfCompData.BoomValue = 5000;
		compData.kfCompData.EnemyCompType = 2;
		compData.kfCompData.EnemyCompTypeSet = 2;
		compData.kfCompData.Bulletin = Global.GetLang("欢迎大家加入");
		compData.kfCompData.Crystal = 100;
		compData.kfCompData.Boss = 200;
		compData.kfCompData.YestdCrystal = 300;
		compData.kfCompData.YestdBoss = 3200;
		compData.kfCompData.PlunderResList = new List<int>();
		compData.kfCompData.PlunderResList.Add(300);
		compData.kfCompData.PlunderResList.Add(200);
		compData.kfCompData.PlunderResList.Add(100);
		compData.kfCompData.YestdBossKillCompType = 1;
		compData.SelectData = new CompSelectData();
		compData.SelectData.RecommendCompList = new List<int>();
		compData.SelectData.RecommendCompList.Add(2);
		compData.SelectData.DaLingZhuNameList = new List<string>();
		compData.SelectData.DaLingZhuNameList.Add(Global.GetLang("天下第一"));
		compData.SelectData.DaLingZhuNameList.Add(Global.GetLang("天下第二"));
		compData.SelectData.DaLingZhuNameList.Add(Global.GetLang("天下第三"));
		compData.BoomValueList = new List<int>();
		compData.BoomValueList.Add(1000);
		compData.BoomValueList.Add(2000);
		compData.BoomValueList.Add(3000);
		return compData;
	}

	public static CompData GetSelfCompData()
	{
		if (ShiLiData.m_selfCompData == null)
		{
			ShiLiData.m_selfCompData = ShiLiData.MockCompData();
		}
		return ShiLiData.m_selfCompData;
	}

	public static int GetSelfCompType()
	{
		return GameInstance.Game.CurrentSession.roleData.CompType;
	}

	public static string GetRecommendRewardInfos()
	{
		return ConfigSystemParam.GetSystemParamByName("CompRecommend", true);
	}

	public static float GetCompReplaceAmerce()
	{
		return (float)ConfigSystemParam.GetSystemParamDoubleByName("CompReplaceAmerce");
	}

	public static int GetCompEnemyHurtNum()
	{
		float num = (float)ConfigSystemParam.GetSystemParamDoubleByName("CompEnemyHurtNum");
		return (int)(num * 100f);
	}

	public static int GetCompReplaceNeed()
	{
		return (int)ConfigSystemParam.GetSystemParamIntByName("CompReplaceNeed");
	}

	public static int GetBuffGoodsId(int compType)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CompEnemy", ',');
		int num = compType - 1;
		return systemParamIntArrayByName[num];
	}

	public static Dictionary<int, Dictionary<int, int>> GetAllBeginTasks()
	{
		if (ShiLiData.m_beginTasks == null)
		{
			ShiLiData.m_beginTasks = new Dictionary<int, Dictionary<int, int>>();
			ShiLiData.m_beginTasks[1] = new Dictionary<int, int>();
			ShiLiData.m_beginTasks[2] = new Dictionary<int, int>();
			ShiLiData.m_beginTasks[3] = new Dictionary<int, int>();
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("CompTaskBegin", '|');
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				if (array.Length > 3)
				{
					int num = array[0].SafeToInt32(0);
					int num2 = array[1].SafeToInt32(0);
					int num3 = array[2].SafeToInt32(0);
					int num4 = array[3].SafeToInt32(0);
					ShiLiData.m_beginTasks[1][num] = num2;
					ShiLiData.m_beginTasks[2][num] = num3;
					ShiLiData.m_beginTasks[3][num] = num4;
				}
				else
				{
					MUDebug.LogError<string>(new string[]
					{
						"CompTaskBegin num error"
					});
				}
			}
		}
		return ShiLiData.m_beginTasks;
	}

	public static int GetBeginTask(ShiLiType type, int taskClass)
	{
		Dictionary<int, int> dictionary = null;
		int result = -1;
		if (ShiLiData.GetAllBeginTasks().TryGetValue((int)type, ref dictionary) && dictionary.TryGetValue(taskClass, ref result))
		{
			return result;
		}
		return result;
	}

	public static bool IsShiLiNpcExenstionID(int exenstionID)
	{
		return ShiLiData.GetAllComp().GetCompByCompMoBaiID(exenstionID) != null;
	}

	public static int GetTaskIdByNum(ShiLiType type, int taskClass, int num)
	{
		return ShiLiData.GetBeginTask(type, taskClass) + num;
	}

	public static int GetTaskNum(int taskClass)
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CompTaskNum", ',');
		int num = taskClass - 100;
		if (systemParamIntArrayByName != null && num < systemParamIntArrayByName.Length)
		{
			return systemParamIntArrayByName[num];
		}
		return 0;
	}

	public static string GetBossAppearTime()
	{
		return ConfigSystemParam.GetSystemParamByName("CompBossTime", true);
	}

	public static List<DuiHuanNumSetting> GetDuiHuanInfos()
	{
		if (ShiLiData.m_duiHunNums == null)
		{
			ShiLiData.m_duiHunNums = new List<DuiHuanNumSetting>();
			string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName("CompShop", '|');
			for (int i = 0; i < systemParamStringArrayByName.Length; i++)
			{
				string[] array = systemParamStringArrayByName[i].Split(new char[]
				{
					','
				});
				if (array.Length < 2)
				{
					MUDebug.LogError<string>(new string[]
					{
						"势力争霸兑换参数出现问题"
					});
				}
				else
				{
					int f = array[0].SafeToInt32(0);
					float m = 1f;
					if (!float.TryParse(array[1], ref m))
					{
						MUDebug.LogError<string>(new string[]
						{
							"势力争霸兑换参数出现问题"
						});
					}
					DuiHuanNumSetting duiHuanNumSetting = new DuiHuanNumSetting(f, m);
					ShiLiData.m_duiHunNums.Add(duiHuanNumSetting);
				}
			}
			ShiLiData.m_duiHunNums.Sort((DuiHuanNumSetting x, DuiHuanNumSetting y) => x.fanRongDu.CompareTo(y.fanRongDu));
		}
		return ShiLiData.m_duiHunNums;
	}

	public static float GetDuiHuanNumMultiple(int fanRongdu)
	{
		List<DuiHuanNumSetting> duiHuanInfos = ShiLiData.GetDuiHuanInfos();
		if (duiHuanInfos.Count < 1)
		{
			return 1f;
		}
		for (int i = duiHuanInfos.Count - 1; i > -1; i--)
		{
			if (fanRongdu >= duiHuanInfos[i].fanRongDu)
			{
				return duiHuanInfos[i].multiple;
			}
		}
		return 1f;
	}

	public static int GetSelfTotalDuiHuanNum(int settingNum)
	{
		float duiHuanNumMultiple = ShiLiData.GetDuiHuanNumMultiple(ShiLiData.GetSelfCompData().kfCompData.BoomValue);
		if (ShiLiData.GetSelfCompData().kfCompData == null)
		{
			return 1;
		}
		return (int)(duiHuanNumMultiple * (float)settingNum);
	}

	public static long GetSelfCompPoint()
	{
		return Global.Data.roleData.MoneyData[138];
	}

	public static long GetSelfJunXian()
	{
		return (long)ShiLiData.GetSelfCompData().kfCompData.SelfJunXian;
	}

	public static KFCompRoleData GetCompRoleDataByZhiWu(List<KFCompRoleData> lstRoles, int zhiWu, int compType)
	{
		if (lstRoles.Count < zhiWu)
		{
			return null;
		}
		KFCompRoleData kfcompRoleData = lstRoles[zhiWu - 1];
		if (kfcompRoleData.RoleID <= 0)
		{
			return null;
		}
		return lstRoles[zhiWu - 1];
	}

	public static MUForceCraft GetBattleCity()
	{
		return ShiLiData.m_battleCity;
	}

	public static void SetBattleCity(MUForceCraft city)
	{
		ShiLiData.m_battleCity = city;
	}

	public static void ShowErrorMessage(int errorCode)
	{
		Super.HintMainText(StdErrorCode.GetErrMsg(errorCode, false, false), 10, 3);
	}

	public static float GetCraftRewardRate(int winNum)
	{
		if (winNum <= 0 || winNum > 5)
		{
			return 1f;
		}
		float[] systemParamFloatArrayByName = ConfigSystemParam.GetSystemParamFloatArrayByName("CraftRewardRate", ',');
		return systemParamFloatArrayByName[winNum - 1];
	}

	public static int[] GetCraftReward()
	{
		return ConfigSystemParam.GetSystemParamIntArrayByName("CraftReward", ',');
	}

	public static int GetBaseStoreId()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CraftStore", ',');
		return systemParamIntArrayByName[0];
	}

	public static int GetBastStoreId()
	{
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CraftStore", ',');
		return systemParamIntArrayByName[1];
	}

	public static bool IsCompBattleOpen()
	{
		return ConfigVersionSystemOpen.IsVersionSystemOpen(120402);
	}

	public static List<CompBattleSideScore> LastSideScore
	{
		get
		{
			return ShiLiData.m_lastSideScore;
		}
		set
		{
			ShiLiData.m_lastSideScore = value;
		}
	}

	public static bool IsQiZuo(int npcId)
	{
		return ShiLiData.GetForceStrongholdBuQiZuo(npcId) != null;
	}

	public static ShiLiType GetQiZuoCompType(int qiZuoID)
	{
		ShiLiType result = ShiLiType.None;
		MUForceStronghold forceStrongholdBuQiZuo = ShiLiData.GetForceStrongholdBuQiZuo(qiZuoID);
		if (forceStrongholdBuQiZuo == null)
		{
			return ShiLiType.None;
		}
		if (ShiLiData.LastSideScore == null)
		{
			return ShiLiType.None;
		}
		if (ShiLiData.LastSideScore.Count != 3)
		{
			return ShiLiType.None;
		}
		for (int i = 0; i < ShiLiData.LastSideScore.Count; i++)
		{
			CompBattleSideScore compBattleSideScore = ShiLiData.LastSideScore[i];
			if (compBattleSideScore.StrongholdSet != null)
			{
				if (compBattleSideScore.StrongholdSet.Contains(forceStrongholdBuQiZuo.ID))
				{
					return (ShiLiType)compBattleSideScore.CompType;
				}
			}
		}
		return result;
	}

	public static string GetMapMineQiZuoName(int qiZuoID)
	{
		string result = "none";
		switch (ShiLiData.GetQiZuoCompType(qiZuoID))
		{
		case ShiLiType.None:
			result = "qibai";
			break;
		case ShiLiType.ShenShengJiaoTuan:
			result = "qihong";
			break;
		case ShiLiType.ZiYouTongMeng:
			result = "qilan";
			break;
		case ShiLiType.ZhiMengXieHui:
			result = "qihuang";
			break;
		}
		return result;
	}

	public static int AddNotice(KFCompNotice notice)
	{
		ShiLiData.noticeId++;
		ShiLiData.m_noticeInfos[ShiLiData.noticeId] = notice;
		return ShiLiData.noticeId;
	}

	public static KFCompNotice GetSystemNoticeByID(int id)
	{
		KFCompNotice result = null;
		ShiLiData.m_noticeInfos.TryGetValue(id, ref result);
		return result;
	}

	public static void ClearAllNotice()
	{
		ShiLiData.m_noticeInfos.Clear();
	}

	public static void GoToNoticePlace(KFCompNotice notice)
	{
		if (notice == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"notice is null"
			});
			return;
		}
		Point pos = new Point(notice.toPosX, notice.toPosY);
		if (Global.IsInShiLiZhengBaMap())
		{
			Global.Data.GameScene.AutoFindRoad(notice.toMapCode, pos, 0, ExtActionTypes.EXTACTION_NONE);
		}
		else
		{
			GameInstance.Game.EnterCompMap(notice.toMapCode, notice.toPosX, notice.toPosY, 0, 0);
		}
	}

	public static bool BeShiLiTask(int taskClass)
	{
		return taskClass >= 100 && taskClass <= 150;
	}

	public static bool IsShiLiOpen()
	{
		return GongnengYugaoMgr.IsShiLiOpen();
	}

	public static void EnterSelfShiLiMap()
	{
		if (Global.IsInShiLiZhengBaMap())
		{
			Super.HintMainText(Global.GetLang("相同地图不可切换"), 10, 3);
			return;
		}
		MUComp mucompById = ShiLiData.GetMUCompById(GameInstance.Game.CurrentSession.roleData.CompType);
		if (mucompById != null)
		{
			GameInstance.Game.EnterCompMap(mucompById.MapCode, 0, 0, 0, 0);
		}
	}

	public static void OpenShiLiWindow()
	{
		if (GameInstance.Game.CurrentSession.roleData.CompType > 0)
		{
			ShiLiData.OpenShiLiMainWindow();
		}
		else
		{
			ShiLiData.OnShiLiSelectWindow();
		}
	}

	private static ChangeableRulePart.RuleXml GetRuleByType(ShiLiHelpType type)
	{
		ChangeableRulePart.RuleXml result = null;
		switch (type)
		{
		case ShiLiHelpType.HelpCompIntroTask:
			result = ShiLiData.GetCompTaskHelpData();
			break;
		case ShiLiHelpType.HelpCompIntroLevel:
			result = ShiLiData.GetCompLevelHelpData();
			break;
		case ShiLiHelpType.HelpCompIntroMap:
			result = ShiLiData.GetCompMapHelpData();
			break;
		}
		return result;
	}

	public static void OpenHelpWindow(ShiLiHelpType type)
	{
		ChangeableRulePart.RuleXml ruleByType = ShiLiData.GetRuleByType(type);
		if (ruleByType == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"未找到相关配置"
			});
			return;
		}
		if (ShiLiData.m_helpWindow == null)
		{
			ShiLiData.m_helpWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiData.m_helpWindow.IsShowModal = true;
			ShiLiData.m_helpWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiData.m_helpWindow, Global.GetLang("帮助界面"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiData.m_helpWindow);
		}
		if (ShiLiData.m_helpPart == null)
		{
			ShiLiData.m_helpPart = U3DUtils.NEW<CommonHelpWindow>();
			ShiLiData.m_helpPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiData.CloseHelpWindow();
			};
		}
		ShiLiData.m_helpWindow.SetContent(ShiLiData.m_helpWindow.BodyPresenter, ShiLiData.m_helpPart, 0.0, 0.0, true);
		ShiLiData.m_helpPart.SetHelpInfo(ruleByType.list);
	}

	private static void CloseHelpWindow()
	{
		if (null != ShiLiData.m_helpPart)
		{
			ShiLiData.m_helpPart.transform.parent = null;
			Object.Destroy(ShiLiData.m_helpPart.gameObject);
			ShiLiData.m_helpPart = null;
		}
		if (null != ShiLiData.m_helpWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShiLiData.m_helpWindow);
			ShiLiData.m_helpWindow = null;
		}
	}

	private static void OpenShiLiMainWindow()
	{
		if (ShiLiData.m_shiLiWindow == null)
		{
			ShiLiData.m_shiLiWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiData.m_shiLiWindow.IsShowModal = true;
			ShiLiData.m_shiLiWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiData.m_shiLiWindow, Global.GetLang("回收"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiData.m_shiLiWindow);
		}
		if (ShiLiData.m_shiLiPart == null)
		{
			ShiLiData.m_shiLiPart = U3DUtils.NEW<ShiLiPartMain>();
			ShiLiData.m_shiLiPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiData.CloseShiLiWindow();
			};
		}
		ShiLiData.m_shiLiWindow.SetContent(ShiLiData.m_shiLiWindow.BodyPresenter, ShiLiData.m_shiLiPart, 0.0, 0.0, true);
	}

	public static void ShowShiLiZhengDuo()
	{
		if (null == ShiLiData.m_shiLiWindow)
		{
			ShiLiData.OpenShiLiMainWindow();
		}
		ShiLiData.m_shiLiPart.OnOpenBattleWindowEX();
	}

	public static void CloseShiLiWindow()
	{
		if (null != ShiLiData.m_shiLiPart)
		{
			ShiLiData.m_shiLiPart.transform.parent = null;
			Object.Destroy(ShiLiData.m_shiLiPart.gameObject);
			ShiLiData.m_shiLiPart = null;
		}
		if (null != ShiLiData.m_shiLiWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShiLiData.m_shiLiWindow);
			ShiLiData.m_shiLiWindow = null;
		}
	}

	public static void OnShiLiSelectWindow()
	{
		if (ShiLiData.m_shiLiSelectWindow == null)
		{
			ShiLiData.m_shiLiSelectWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiData.m_shiLiSelectWindow.IsShowModal = true;
			ShiLiData.m_shiLiSelectWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiData.m_shiLiSelectWindow, Global.GetLang("势力商城"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiData.m_shiLiSelectWindow);
		}
		if (ShiLiData.m_shiLiSelectPart == null)
		{
			ShiLiData.m_shiLiSelectPart = U3DUtils.NEW<ShiLiPartSelect>();
			ShiLiData.m_shiLiSelectPart.transform.localScale = new Vector3(1f, 1f, 1f);
			ShiLiData.m_shiLiSelectPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiData.CloseShiLiSelectWindow();
			};
		}
		ShiLiData.m_shiLiSelectWindow.SetContent(ShiLiData.m_shiLiSelectWindow.BodyPresenter, ShiLiData.m_shiLiSelectPart, 0.0, 0.0, true);
	}

	public static void CloseShiLiSelectWindow()
	{
		if (null != ShiLiData.m_shiLiSelectPart)
		{
			ShiLiData.m_shiLiSelectPart.transform.parent = null;
			Object.Destroy(ShiLiData.m_shiLiSelectPart.gameObject);
			ShiLiData.m_shiLiSelectPart = null;
		}
		if (null != ShiLiData.m_shiLiSelectWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShiLiData.m_shiLiSelectWindow);
			ShiLiData.m_shiLiSelectWindow = null;
		}
	}

	public static void LingQuJiangLi(int taskId)
	{
		if (ShiLiData.m_renWuPart != null)
		{
			ShiLiData.m_renWuPart.JiangLiGet(taskId);
		}
	}

	public static void RefreshReWuWindow()
	{
		if (ShiLiData.m_renWuPart != null)
		{
			ShiLiData.m_renWuPart.Refresh();
		}
	}

	public static void OpenRenWuWindow()
	{
		if (ShiLiData.m_renWuWindow == null)
		{
			ShiLiData.m_renWuWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiData.m_renWuWindow.IsShowModal = true;
			ShiLiData.m_renWuWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiData.m_renWuWindow, Global.GetLang("任务大厅"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiData.m_renWuWindow);
		}
		if (ShiLiData.m_renWuPart == null)
		{
			ShiLiData.m_renWuPart = U3DUtils.NEW<ShiLiPartRenWu>();
			ShiLiData.m_renWuPart.CloseHandler = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiData.CloseRenWuWindow();
			};
		}
		ShiLiData.m_renWuWindow.SetContent(ShiLiData.m_renWuWindow.BodyPresenter, ShiLiData.m_renWuPart, 0.0, 0.0, true);
	}

	private static void CloseRenWuWindow()
	{
		if (null != ShiLiData.m_renWuPart)
		{
			ShiLiData.m_renWuPart.transform.parent = null;
			Object.Destroy(ShiLiData.m_renWuPart.gameObject);
			ShiLiData.m_renWuPart = null;
		}
		if (null != ShiLiData.m_renWuWindow)
		{
			Super.CloseChildWindow(Super.GData.GlobalPlayZone.Children, ShiLiData.m_renWuWindow);
			ShiLiData.m_renWuWindow = null;
		}
	}

	public static void OpenShopWindow()
	{
		if (ShiLiData.m_shopWindow == null)
		{
			ShiLiData.m_shopWindow = U3DUtils.NEW<GChildWindow>();
			ShiLiData.m_shopWindow.IsShowModal = true;
			ShiLiData.m_shopWindow.ModalType = ChildWindowModalType.Translucent;
			Super.InitChildWindow(ShiLiData.m_shopWindow, Global.GetLang("势力商城"));
			Super.GData.GlobalPlayZone.Children.Add(ShiLiData.m_shopWindow);
		}
		if (ShiLiData.m_shopPart == null)
		{
			ShiLiData.m_shopPart = U3DUtils.NEW<MUDuiHuanPart>();
			ShiLiData.m_shopPart.transform.localScale = new Vector3(1f, 1f, 1f);
			ShiLiData.m_shopPart.InitPartData(9, 0);
			ShiLiData.m_shopPart.DPSelectedItem = delegate(object s, DPSelectedItemEventArgs e)
			{
				ShiLiData.CloseShopWindow();
				return false;
			};
		}
		ShiLiData.m_shopWindow.SetContent(ShiLiData.m_shopWindow.BodyPresenter, ShiLiData.m_shopPart, 0.0, 0.0, true);
	}

	public static void CloseShopWindow()
	{
		if (null != ShiLiData.m_shopPart)
		{
			ShiLiData.m_shopPart.transform.parent = null;
			Object.Destroy(ShiLiData.m_shopPart.gameObject);
			ShiLiData.m_shopPart = null;
		}
		if (null != ShiLiData.m_shopWindow)
		{
			Super.GData.GlobalPlayZone.Children.Remove(ShiLiData.m_shopWindow, true);
			ShiLiData.m_shopWindow = null;
		}
	}

	public const int CompBattleID = 120402;

	private static MUAllComp m_allComp = null;

	private static MUAllCompLevel m_allCompLevel = null;

	private static MUAllCompResources m_allResources = null;

	private static MUALLCompNotice m_allNotices = null;

	private static ChangeableRulePart.RuleXml m_compTaskHelpData = null;

	private static ChangeableRulePart.RuleXml m_compLevelHelpData = null;

	private static ChangeableRulePart.RuleXml m_compMapHelpData = null;

	private static ChangeableRulePart.RuleXml m_compBattleHelpData = null;

	private static MUForceCraftAll m_allForceCraft = null;

	private static MUForceStrongholdAll m_allForceStronghold = null;

	private static MUForceCraftRewardAll m_allCraftReward = null;

	private static CompData m_selfCompData;

	private static Dictionary<int, Dictionary<int, int>> m_beginTasks = null;

	private static List<DuiHuanNumSetting> m_duiHunNums;

	private static MUForceCraft m_battleCity = null;

	private static List<CompBattleSideScore> m_lastSideScore;

	private static Dictionary<int, KFCompNotice> m_noticeInfos = new Dictionary<int, KFCompNotice>();

	private static int noticeId = 10000;

	protected static GChildWindow m_helpWindow = null;

	protected static CommonHelpWindow m_helpPart = null;

	protected static GChildWindow m_shiLiWindow = null;

	protected static ShiLiPartMain m_shiLiPart = null;

	protected static GChildWindow m_shiLiSelectWindow = null;

	protected static ShiLiPartSelect m_shiLiSelectPart = null;

	protected static GChildWindow m_renWuWindow = null;

	protected static ShiLiPartRenWu m_renWuPart = null;

	protected static GChildWindow m_shopWindow = null;

	protected static MUDuiHuanPart m_shopPart = null;
}
