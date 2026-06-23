using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class VerifyPlayerRecallAccount
{
	public static void OnCentralServerCallback_Type0(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null)
		{
			string textMsg = string.Empty;
			if (!string.IsNullOrEmpty(w.error))
			{
				textMsg = Global.GetLang("验证老玩家失败，连接验证服务器失败，请检查网络");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
				return;
			}
			if (string.IsNullOrEmpty(w.text) || !w.text.Equals("OK"))
			{
				textMsg = Global.GetLang("验证老玩家失败，不是老玩家");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
			}
		}
	}

	public static void OnCentralServerCallback_Type1(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null)
		{
			string textMsg = string.Empty;
			if (!string.IsNullOrEmpty(w.error))
			{
				textMsg = Global.GetLang("验证老玩家推荐人失败，连接验证服务器失败，请检查网络");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
				return;
			}
			if (string.IsNullOrEmpty(w.text) || !w.text.Equals("OK"))
			{
				textMsg = Global.GetLang("验证老玩家推荐人失败");
				GGameInfocs.AddGameInfoMessage(GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, textMsg, 0, -1, -1, 0);
			}
		}
	}

	public static void PostRecallInfo(string userInputID, int requestType = 0)
	{
		VerifyPlayerRecallData verifyPlayerRecallData = new VerifyPlayerRecallData();
		verifyPlayerRecallData.userID = Global.Data.UserID;
		verifyPlayerRecallData.roleID = Global.Data.RoleID;
		verifyPlayerRecallData.serverID = Global.Data.GameServerID;
		verifyPlayerRecallData.userInputID = userInputID;
		verifyPlayerRecallData.roleLevel = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
		verifyPlayerRecallData.vipLevel = Global.GetVIPLeve();
		verifyPlayerRecallData.requestType = requestType;
		verifyPlayerRecallData.strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
		{
			"SMFCKO76fAJvX27f8v0Yu9EXZ3u3poFO4NPt12",
			verifyPlayerRecallData.userID,
			verifyPlayerRecallData.roleID,
			verifyPlayerRecallData.serverID,
			verifyPlayerRecallData.userInputID,
			verifyPlayerRecallData.roleLevel,
			verifyPlayerRecallData.vipLevel
		}));
		string url = Global.ServerListURLSecond + "UserHuiGui.aspx";
		if (requestType == 0)
		{
			SimpleHttpTask.HttpPost(url, null, DataHelper.ObjectToBytes<VerifyPlayerRecallData>(verifyPlayerRecallData), new SimpleHttpTask.HttpCallback(VerifyPlayerRecallAccount.OnCentralServerCallback_Type0), 10f);
		}
		else if (requestType == 1)
		{
			SimpleHttpTask.HttpPost(url, null, DataHelper.ObjectToBytes<VerifyPlayerRecallData>(verifyPlayerRecallData), new SimpleHttpTask.HttpCallback(VerifyPlayerRecallAccount.OnCentralServerCallback_Type1), 10f);
		}
	}
}
