using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class MoYuDuoBaoNetManager
{
	public static void ServeGetBaseInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		ZorkBattleBaseData zorkBattleBaseData = DataHelper.BytesToObject<ZorkBattleBaseData>(e.bytesData, 0, e.bytesData.Length);
		if (zorkBattleBaseData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<ZorkBattleBaseData>("CMD_SPR_ZORK_BASE_DATA", zorkBattleBaseData);
	}

	public static void ServeEnter(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num == 1)
		{
			TeamCompeteDataManager.PopupEnterMoYuDuoBaoWindow();
		}
		else if (num < 0)
		{
			string chineseText = string.Empty;
			if (num == -4021)
			{
				chineseText = Global.GetLang("进入失败");
			}
			else
			{
				chineseText = StdErrorCode.GetErrMsg(num, true, false);
			}
			Super.HintMainText(Global.GetLang(chineseText), 10, 3);
		}
	}

	public static void ServeGetAwardImfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		ZorkBattleAwardsData zorkBattleAwardsData = DataHelper.BytesToObject<ZorkBattleAwardsData>(e.bytesData, 0, e.bytesData.Length);
		if (zorkBattleAwardsData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<ZorkBattleAwardsData>("CMD_SPR_ZORK_AWARD", zorkBattleAwardsData);
	}

	public static void ServeActivityState(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = int.Parse(e.fields[0]);
		ZorkBattleGameStates duoBaoState = num;
		MoYuDuoBaoData.DuoBaoState = duoBaoState;
		if (e.fields.Length > 1)
		{
			MoYuDuoBaoData.Round = int.Parse(e.fields[1]);
		}
		if (e.fields.Length > 2)
		{
			MoYuDuoBaoData.TimeOff = int.Parse(e.fields[2]) - 1;
		}
		if (e.fields.Length > 3)
		{
			MoYuDuoBaoData.BestTeam = e.fields[3];
		}
		MUEventManager.SendEvent<ZorkBattleGameStates>("CMD_SPR_ZORK_STATE", MoYuDuoBaoData.DuoBaoState);
	}

	public static void ServeSideScore(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		ZorkBattleSideScore zorkBattleSideScore = DataHelper.BytesToObject<ZorkBattleSideScore>(e.bytesData, 0, e.bytesData.Length);
		if (zorkBattleSideScore == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MoYuDuoBaoData.SetAppearTime(zorkBattleSideScore.MosterNextTimeDict);
		MUEventManager.SendEvent<ZorkBattleSideScore>("CMD_SPR_ZORK_SIDE_SCORE", zorkBattleSideScore);
	}

	public static void ServeRankInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		ZorkBattleRankInfo @params = DataHelper.BytesToObject<ZorkBattleRankInfo>(e.bytesData, 0, e.bytesData.Length);
		MUEventManager.SendEvent<ZorkBattleRankInfo>("CMD_SPR_ZORK_RANK_INFO", @params);
	}

	public static void ServeGetAward(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 2)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = int.Parse(e.fields[0]);
		int num2 = int.Parse(e.fields[1]);
		if (num >= 0)
		{
			MUEventManager.SendEvent<int>("CMD_SPR_ZORK_AWARD_GET", num);
			return;
		}
		if (num == -101)
		{
			Super.HintMainText(Global.GetLang("重生背包已满"), 10, 3);
			return;
		}
		if (num == -100)
		{
			Super.HintMainText(Global.GetLang("背包已满"), 10, 3);
			return;
		}
		string errMsg = StdErrorCode.GetErrMsg(num, true, false);
		Super.HintMainText(Global.GetLang(errMsg), 10, 3);
	}

	public static void ServeBaoMingResult(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			string chineseText = string.Empty;
			if (num == -4026)
			{
				chineseText = Global.GetLang("战队成员人数不满足报名条件");
			}
			else if (num == -19)
			{
				chineseText = Global.GetLang("战队成员重生等级不满足报名条件");
			}
			else
			{
				chineseText = StdErrorCode.GetErrMsg(num, true, false);
			}
			Super.HintMainText(Global.GetLang(chineseText), 10, 3);
		}
		else
		{
			Super.HintMainText(Global.GetLang("报名成功"), 10, 3);
			GameInstance.Game.SendDuoBaoGetActivityState();
		}
	}

	public static void ServeMiBaoShengJie(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		MazingerStore mazingerStore = DataHelper.BytesToObject<MazingerStore>(e.bytesData, 0, e.bytesData.Length);
		if (mazingerStore == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
		}
		else
		{
			int result = mazingerStore.result;
			if (result != 1)
			{
				string errorMessage = MoYuDuoBaoNetManager.GetErrorMessage(result);
				Super.HintMainText(errorMessage, 10, 3);
				MUEventManager.SendEvent<MazingerStore>("CMD_SPR_MAZINGERSTORE_UPGRADE ", mazingerStore);
			}
			else
			{
				MUEventManager.SendEvent<MazingerStore>("CMD_SPR_MAZINGERSTORE_UPGRADE ", mazingerStore);
			}
		}
	}

	public static string GetErrorMessage(int code)
	{
		string chineseText = string.Empty;
		switch (code)
		{
		case 2:
			chineseText = "参数错误";
			break;
		case 3:
			chineseText = Global.GetLang("操作错误");
			break;
		case 4:
			chineseText = Global.GetLang("服务器出错");
			break;
		case 6:
			chineseText = Global.GetLang("系统出错");
			break;
		case 7:
			chineseText = Global.GetLang("物品不足");
			break;
		case 8:
			chineseText = Global.GetLang("系统不存在物品");
			break;
		case 9:
			chineseText = Global.GetLang("使用道具出错");
			break;
		case 10:
			chineseText = Global.GetLang("数据保存出错");
			break;
		case 11:
			chineseText = Global.GetLang("星数不足，无法升阶");
			break;
		case 12:
			chineseText = Global.GetLang("功能未开启");
			break;
		}
		return Global.GetLang(chineseText);
	}

	public static void Test(MUSocketConnectEventArgs e)
	{
	}

	public enum MazingerStoreOpcode
	{
		Succ = 1,
		ParamErr,
		NotType,
		XmlErr,
		MaxStar,
		CatchErr,
		GoodNotEougth,
		NotGood,
		UseGood,
		DataSave,
		NotUpGrade,
		NotOpen
	}
}
