using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ShiLiNetManager
{
	public static void ServeGetCompData(MUSocketConnectEventArgs e)
	{
		CompData compData = DataHelper.BytesToObject<CompData>(e.bytesData, 0, e.bytesData.Length);
		if (compData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		Super.HideNetWaiting();
		ShiLiData.SetselfCompData(compData);
		GameInstance.Game.CurrentSession.roleData.CompType = compData.kfCompData.CompType;
		MUEventManager.SendEvent<CompData>("CMD_SPR_COMP_DATA", compData);
	}

	public static void ServerJoinComp(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			if (int.Parse(e.fields[0]) == -12)
			{
				Super.HintMainText(Global.GetLang("势力争霸战期间不能转换势力"), 10, 3);
			}
			else
			{
				ShiLiData.ShowErrorMessage(int.Parse(e.fields[0]));
			}
			return;
		}
		if (e.fields.Length < 2)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		GameInstance.Game.CurrentSession.roleData.CompType = num;
		MUEventManager.SendEvent<int>("CMD_SPR_COMP_JOIN", num);
	}

	public static void ServerGetRank(MUSocketConnectEventArgs e)
	{
		List<CompRankInfo> list = DataHelper.BytesToObject<List<CompRankInfo>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		Super.HideNetWaiting();
		MUEventManager.SendEvent<List<CompRankInfo>>("CMD_SPR_COMP_RANK_INFO", list);
	}

	public static void ServerSetBulltin(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num >= 0)
		{
			MUEventManager.SendEvent("CMD_SPR_COMP_SET_BULLETIN");
			return;
		}
		if (num == -12)
		{
			Super.HintMainText(Global.GetLang("权限不足"), 10, 3);
			return;
		}
		ShiLiData.ShowErrorMessage(num);
	}

	public static void ServerSetEnemy(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			ShiLiData.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		if (e.fields.Length < 2)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		ShiLiData.GetSelfCompData().kfCompData.EnemyCompTypeSet = num;
		MUEventManager.SendEvent<int>("CMD_SPR_COMP_SET_ENEMY", num);
	}

	public static void ServerZhiWu(MUSocketConnectEventArgs e)
	{
		CompZhiWuData @params = DataHelper.BytesToObject<CompZhiWuData>(e.bytesData, 0, e.bytesData.Length);
		Super.HideNetWaiting();
		MUEventManager.SendEvent<CompZhiWuData>("CMD_SPR_COMP_ZHIWU", @params);
	}

	public static void ServerMapEnter(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			Global.Data.WaitingForMapChange = false;
			ShiLiData.ShowErrorMessage(num);
			return;
		}
	}

	public static void ServerCompNotice(MUSocketConnectEventArgs e)
	{
		KFCompNotice kfcompNotice = DataHelper.BytesToObject<KFCompNotice>(e.bytesData, 0, e.bytesData.Length);
		Super.HideNetWaiting();
		if (kfcompNotice == null)
		{
			return;
		}
		(Super.GData.PlayZoneRoot as PlayZone).ShowCompNotice(kfcompNotice);
	}

	public static void ServerSidSocre(MUSocketConnectEventArgs e)
	{
		CompBattleScoreData @params = DataHelper.BytesToObject<CompBattleScoreData>(e.bytesData, 0, e.bytesData.Length);
		Super.HideNetWaiting();
		MUEventManager.SendEvent<CompBattleScoreData>("CMD_SPR_COMP_SIDE_SCORE", @params);
	}

	public static void ServerSelfSocre(MUSocketConnectEventArgs e)
	{
		long @params = DataHelper.BytesToObject<long>(e.bytesData, 0, e.bytesData.Length);
		Super.HideNetWaiting();
		MUEventManager.SendEvent<long>("CMD_SPR_COMP_SELF_SCORE", @params);
	}

	public static void ServerSelfZhiWuChange(MUSocketConnectEventArgs e)
	{
		if (int.Parse(e.fields[0]) < 0)
		{
			ShiLiData.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		if (e.fields.Length < 2)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		GameInstance.Game.CurrentSession.roleData.CompZhiWu = (byte)num;
		MUEventManager.SendEvent<int>("CMD_SPR_COMP_CHANGE_ZHIWU", num);
	}

	public static void ServerCompPointChange()
	{
		MUEventManager.SendEvent("CMD_SPR_COMPPOINT_CHANGE");
	}

	public static void ServerGetCompBattleData(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		CompBattleBaseData compBattleBaseData = DataHelper.BytesToObject<CompBattleBaseData>(e.bytesData, 0, e.bytesData.Length);
		if (compBattleBaseData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<CompBattleBaseData>("CMD_SPR_COMP_BATTLE_BASE_DATA", compBattleBaseData);
	}

	public static void ServerGetCompBattleCityData(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		CompBattleCifyData compBattleCifyData = DataHelper.BytesToObject<CompBattleCifyData>(e.bytesData, 0, e.bytesData.Length);
		if (compBattleCifyData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<CompBattleCifyData>("CMD_SPR_COMP_BATTLE_CITY_DATA", compBattleCifyData);
	}

	public static void ServerGetCompBattleEnterData(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = int.Parse(e.fields[0]);
		if (num == -22)
		{
			if (e.fields.Length < 2)
			{
				Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
				return;
			}
			int @params = int.Parse(e.fields[1]);
			MUEventManager.SendEvent<int>("CMD_SPR_COMP_BATTLE_ENTER_PAIDUI", @params);
			return;
		}
		else
		{
			if (int.Parse(e.fields[0]) < 0)
			{
				ShiLiData.ShowErrorMessage(int.Parse(e.fields[0]));
				MUDebug.LogError<string>(new string[]
				{
					"CMD_SPR_COMP_BATTLE_ENTER ErrorCode  " + num
				});
				return;
			}
			MUEventManager.SendEvent<int>("CMD_SPR_COMP_BATTLE_ENTER", num);
			return;
		}
	}

	public static void ServerGetCompBattleAwardData(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		CompBattleAwardsData compBattleAwardsData = DataHelper.BytesToObject<CompBattleAwardsData>(e.bytesData, 0, e.bytesData.Length);
		if (compBattleAwardsData == null)
		{
			compBattleAwardsData = new CompBattleAwardsData();
			compBattleAwardsData.WinNum = 0;
			compBattleAwardsData.RankNum = 0;
			compBattleAwardsData.AwardID = 0;
		}
		MUEventManager.SendEvent<CompBattleAwardsData>("CMD_SPR_COMP_BATTLE_AWARD", compBattleAwardsData);
	}

	public static void ServerGetCompBattleState(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			ShiLiData.ShowErrorMessage(num);
			return;
		}
		MUEventManager.SendEvent<CompBattleGameStates>("CMD_SPR_COMP_BATTLE_STATE", (CompBattleGameStates)num);
	}

	public static void ServerGetCompBattleSideScore(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		List<CompBattleSideScore> list = DataHelper.BytesToObject<List<CompBattleSideScore>>(e.bytesData, 0, e.bytesData.Length);
		if (list == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		ShiLiData.LastSideScore = list;
		MUEventManager.SendEvent<List<CompBattleSideScore>>("CMD_SPR_COMP_BATTLE_SIDE_SCORE", list);
	}

	public static void ServerGetCompBattleSelfScore(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		CompBattleSelfScore compBattleSelfScore = DataHelper.BytesToObject<CompBattleSelfScore>(e.bytesData, 0, e.bytesData.Length);
		if (compBattleSelfScore == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		MUEventManager.SendEvent<CompBattleSelfScore>("CMD_SPR_COMP_BATTLE_SELF_SCORE", compBattleSelfScore);
	}

	public static void ServerGetCompBattleAwardGet(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = DataHelper.BytesToObject<int>(e.bytesData, 0, e.bytesData.Length);
		if (num < 0)
		{
			ShiLiData.ShowErrorMessage(num);
			return;
		}
		MUEventManager.SendEvent<int>("CMD_SPR_COMP_BATTLE_AWARD_GET", num);
	}
}
