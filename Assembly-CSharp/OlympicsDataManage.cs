using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Tmsk.Xml;

public class OlympicsDataManage
{
	public static void Clear()
	{
		OlympicsDataManage.totalScore = 0;
		OlympicsDataManage.ownedScore = 0;
		OlympicsDataManage.totalRankCount = 100;
		OlympicsDataManage.shootTimes = 0;
		OlympicsDataManage.playBallTimes = 0;
		OlympicsDataManage.guessDataList.Clear();
		OlympicsDataManage.yesterdayGuessDataList.Clear();
		OlympicsDataManage.shopDataList.Clear();
		OlympicsDataManage.awardDictData.Clear();
		OlympicsDataManage.matchDictData.Clear();
		OlympicsDataManage.titleList.Clear();
	}

	public static Dictionary<int, OlympicsMatchData> GetMatchData()
	{
		if (OlympicsDataManage.matchDictData.Count > 0)
		{
			return OlympicsDataManage.matchDictData;
		}
		XElement gameResXml = Global.GetGameResXml("Config/AoYunMatch.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "AoYunMatch");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			OlympicsMatchData olympicsMatchData = new OlympicsMatchData();
			olympicsMatchData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			olympicsMatchData.FreeNum = Global.GetXElementAttributeInt(xelementList[i], "FreeNum");
			string[] array = Global.GetXElementAttributeStr(xelementList[i], "NeedZhuanShi").Split(new char[]
			{
				','
			});
			olympicsMatchData.needDiamondsList = new List<int>();
			for (int j = 0; j < array.Length; j++)
			{
				olympicsMatchData.needDiamondsList.Add(array[j].SafeToInt32(0));
			}
			olympicsMatchData.WinNum = Global.GetXElementAttributeInt(xelementList[i], "WinNum");
			olympicsMatchData.GameNum = Global.GetXElementAttributeInt(xelementList[i], "GameNum");
			olympicsMatchData.WinJiFen = Global.GetXElementAttributeInt(xelementList[i], "WinJiFen");
			olympicsMatchData.LoseJiFen = Global.GetXElementAttributeInt(xelementList[i], "LoseJiFen");
			if (!OlympicsDataManage.matchDictData.ContainsKey(olympicsMatchData.ID))
			{
				OlympicsDataManage.matchDictData.Add(olympicsMatchData.ID, olympicsMatchData);
			}
			i++;
		}
		return OlympicsDataManage.matchDictData;
	}

	private static List<string> ReloadTitileXml()
	{
		if (OlympicsDataManage.titleList.Count > 0)
		{
			return OlympicsDataManage.titleList;
		}
		XElement gameResXml = Global.GetGameResXml("Config/AoYunType.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "AoYunType");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			OlympicsDataManage.titleList.Add(Global.GetXElementAttributeStr(xelementList[i], "Name"));
			i++;
		}
		return OlympicsDataManage.titleList;
	}

	public static string GetTitleName(int index)
	{
		return OlympicsDataManage.ReloadTitileXml()[index];
	}

	public static void SetGuessData(List<OlympicsGuessData> tmpDataList, int dayId)
	{
		OlympicsDataManage.guessDataList.Clear();
		OlympicsDataManage.guessDayId = dayId;
		OlympicsDataManage.guessDataList = tmpDataList;
	}

	public static List<OlympicsGuessData> GetGuessData()
	{
		if (OlympicsDataManage.guessDataList == null || OlympicsDataManage.guessDataList.Count <= 0)
		{
			return new List<OlympicsGuessData>();
		}
		return OlympicsDataManage.guessDataList;
	}

	public static void SetYesterdayGuessData(List<OlympicsGuessData> tmpDataList)
	{
		OlympicsDataManage.yesterdayGuessDataList.Clear();
		OlympicsDataManage.yesterdayGuessDataList = tmpDataList;
	}

	public static List<OlympicsGuessData> GetYesterdayGuessData()
	{
		if (OlympicsDataManage.yesterdayGuessDataList == null || OlympicsDataManage.yesterdayGuessDataList.Count <= 0)
		{
			return new List<OlympicsGuessData>();
		}
		return OlympicsDataManage.yesterdayGuessDataList;
	}

	public static void SetRankData(List<KFRankData> tmpDataList)
	{
		OlympicsDataManage.rankDataList = tmpDataList;
	}

	public static List<KFRankData> GetRankData()
	{
		if (OlympicsDataManage.rankDataList == null || OlympicsDataManage.rankDataList.Count <= 0)
		{
			return new List<KFRankData>();
		}
		return OlympicsDataManage.rankDataList;
	}

	public static int GetCurrentPlayerIndexOfRank()
	{
		int result = 0;
		if (OlympicsDataManage.rankDataList == null || OlympicsDataManage.rankDataList.Count <= 0)
		{
			return 0;
		}
		int count = OlympicsDataManage.rankDataList.Count;
		if (count > OlympicsDataManage.totalRankCount)
		{
			int rank = OlympicsDataManage.rankDataList[OlympicsDataManage.totalRankCount].Rank;
			result = ((rank <= 50000) ? rank : -1);
		}
		else
		{
			for (int i = 0; i < count; i++)
			{
				KFRankData kfrankData = OlympicsDataManage.rankDataList[i];
				if (Global.Data.RoleID == kfrankData.RoleID)
				{
					result = kfrankData.Rank;
					break;
				}
				result = -1;
			}
		}
		return result;
	}

	public static Dictionary<int, OlympicsAwardData> GetAwardData()
	{
		if (OlympicsDataManage.awardDictData.Count > 0)
		{
			return OlympicsDataManage.awardDictData;
		}
		XElement gameResXml = Global.GetGameResXml("Config/AoYunAward.xml");
		if (gameResXml == null)
		{
			return null;
		}
		List<XElement> xelementList = Global.GetXElementList(gameResXml, "AoYunAward");
		int i = 0;
		int count = xelementList.Count;
		while (i < count)
		{
			OlympicsAwardData olympicsAwardData = new OlympicsAwardData();
			olympicsAwardData.ID = Global.GetXElementAttributeInt(xelementList[i], "ID");
			olympicsAwardData.Name = Global.GetXElementAttributeStr(xelementList[i], "Name");
			olympicsAwardData.StarRank = Global.GetXElementAttributeInt(xelementList[i], "BeginNum");
			olympicsAwardData.EndRank = Global.GetXElementAttributeInt(xelementList[i], "EngNum");
			olympicsAwardData.Award = Global.GetXElementAttributeStr(xelementList[i], "GoodsOne");
			if (!OlympicsDataManage.awardDictData.ContainsKey(olympicsAwardData.ID))
			{
				OlympicsDataManage.awardDictData.Add(olympicsAwardData.ID, olympicsAwardData);
			}
			i++;
		}
		return OlympicsDataManage.awardDictData;
	}

	public static void SetShopData(List<OlympicsShopData> tmpDataList)
	{
		OlympicsDataManage.shopDataList = tmpDataList;
	}

	public static List<OlympicsShopData> GetShopData()
	{
		if (OlympicsDataManage.shopDataList == null || OlympicsDataManage.shopDataList.Count <= 0)
		{
			return new List<OlympicsShopData>();
		}
		return OlympicsDataManage.shopDataList;
	}

	public static List<OlympicsShopData> RefreshShopData(int isSuccess, int id, int ownedBuyCount, int totalBuyCount)
	{
		List<OlympicsShopData> shopData = OlympicsDataManage.GetShopData();
		if (shopData == null || shopData.Count <= 0)
		{
			return new List<OlympicsShopData>();
		}
		for (int i = 0; i < shopData.Count; i++)
		{
			OlympicsShopData olympicsShopData = shopData[i];
			if (olympicsShopData.ID == id)
			{
				olympicsShopData.NumSingleBuy = ownedBuyCount;
				olympicsShopData.NumFullBuy = totalBuyCount;
				shopData[i] = olympicsShopData;
			}
		}
		return shopData;
	}

	public static bool MessageType(int operateType)
	{
		bool result = false;
		switch (operateType + 10)
		{
		case 0:
			Super.HintMainText(Global.GetLang("不能领取奖励"), 10, 3);
			break;
		case 1:
			Super.HintMainText(Global.GetLang("已经领取奖励"), 10, 3);
			break;
		case 2:
			Super.HintMainText(Global.GetLang("钻石不足"), 10, 3);
			break;
		case 3:
			Super.HintMainText(Global.GetLang("次数超过上限"), 10, 3);
			break;
		case 4:
			Super.HintMainText(Global.GetLang("游戏类型错误"), 10, 3);
			break;
		case 5:
			Super.HintMainText(Global.GetLang("背包不足"), 10, 3);
			break;
		case 6:
			Super.HintMainText(Global.GetLang("积分不足"), 10, 3);
			break;
		case 7:
			Super.HintMainText(Global.GetLang("剩余总数不足"), 10, 3);
			break;
		case 8:
			Super.HintMainText(Global.GetLang("个人限购次数不足"), 10, 3);
			break;
		case 9:
			Super.HintMainText(Global.GetLang("活动未开启"), 10, 3);
			break;
		case 10:
			Super.HintMainText(Global.GetLang("失败"), 10, 3);
			break;
		case 11:
			result = true;
			break;
		}
		return result;
	}

	public static int totalScore = 0;

	public static int ownedScore = 0;

	public static int totalRankCount = 100;

	public static int shootTimes = 0;

	public static int playBallTimes = 0;

	public static int guessDayId = 0;

	private static List<OlympicsGuessData> guessDataList = new List<OlympicsGuessData>();

	private static List<OlympicsGuessData> yesterdayGuessDataList = new List<OlympicsGuessData>();

	private static List<KFRankData> rankDataList = new List<KFRankData>();

	private static List<OlympicsShopData> shopDataList = new List<OlympicsShopData>();

	private static Dictionary<int, OlympicsAwardData> awardDictData = new Dictionary<int, OlympicsAwardData>();

	private static Dictionary<int, OlympicsMatchData> matchDictData = new Dictionary<int, OlympicsMatchData>();

	private static List<string> titleList = new List<string>();
}
