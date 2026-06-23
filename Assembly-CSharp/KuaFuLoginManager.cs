using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using Server.Tools;
using Tmsk.Contract;

public class KuaFuLoginManager
{
	public static void UpdateWebToken(int verSign, string userID, string userName, string lastTime, string isadult, string signCode)
	{
		WebLoginToken webLoginToken = new WebLoginToken
		{
			VerSign = verSign,
			UserID = userID,
			UserName = userName,
			LastTime = lastTime,
			Isadult = isadult,
			SignCode = signCode
		};
		KuaFuLoginManager.KuaFuServerLoginDataOriginal.WebLoginToken = webLoginToken;
		if (KuaFuLoginManager.KuaFuServerLoginData != null)
		{
			KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken = webLoginToken;
		}
	}

	public static bool LoginKuaFuServer(out string ip, out int port)
	{
		ip = string.Empty;
		port = 0;
		if (KuaFuLoginManager.KuaFuServerLoginData == KuaFuLoginManager.KuaFuServerLoginDataKuaFu && KuaFuLoginManager.KuaFuServerLoginDataKuaFu != null && KuaFuLoginManager.KuaFuServerLoginDataKuaFu.RoleId > 0)
		{
			ip = KuaFuLoginManager.KuaFuServerLoginData.ServerIp;
			port = KuaFuLoginManager.KuaFuServerLoginData.ServerPort;
			return true;
		}
		return false;
	}

	public static int GetKuaFuSeverLineNumber()
	{
		if (KuaFuLoginManager.KuaFuServerLoginData != null)
		{
			return KuaFuLoginManager.KuaFuServerLoginData.Line;
		}
		return 0;
	}

	public static bool ChangeToKuaFuServer(KuaFuServerLoginData kuaFuServerLoginData)
	{
		if (kuaFuServerLoginData != null)
		{
			KuaFuLoginManager.KuaFuServerLoginDataKuaFu = kuaFuServerLoginData;
			KuaFuLoginManager.KuaFuServerLoginData = KuaFuLoginManager.KuaFuServerLoginDataKuaFu;
			KuaFuLoginManager.NewRoleID = kuaFuServerLoginData.RoleId;
			if (kuaFuServerLoginData.RoleId != Global.Data.roleData.RoleID)
			{
				KuaFuLoginManager.ChangeRole = 1;
			}
			else
			{
				KuaFuLoginManager.ChangeRole = 0;
			}
			KuaFuLoginManager.KuaFuServerLoginDataOriginal.RoleId = GameInstance.Game.CurrentSession.roleData.RoleID;
			if (KuaFuLoginManager.KuaFuServerLoginDataKuaFu.ServerId != KuaFuLoginManager.KuaFuServerLoginDataOriginal.ServerId)
			{
				return true;
			}
		}
		return false;
	}

	public static void ChangeToOriginalServer()
	{
		KuaFuLoginManager.KuaFuServerLoginData = KuaFuLoginManager.KuaFuServerLoginDataOriginal;
	}

	public static void OnChangeServerComplete()
	{
		if (KuaFuLoginManager.KuaFuServerLoginData == KuaFuLoginManager.KuaFuServerLoginDataOriginal)
		{
			KuaFuLoginManager.KuaFuServerLoginDataOriginal.RoleId = 0;
			KuaFuLoginManager.KuaFuServerLoginDataKuaFu = null;
		}
	}

	public static void ClearLoginInfo()
	{
		KuaFuLoginManager.KuaFuServerLoginDataOriginal.RoleId = 0;
		KuaFuLoginManager.KuaFuServerLoginDataKuaFu = null;
		KuaFuLoginManager.KuaFuServerLoginData = null;
	}

	public static bool DirectLogin()
	{
		if (KuaFuLoginManager.KuaFuServerLoginData != null && KuaFuLoginManager.KuaFuServerLoginData.RoleId > 0)
		{
			GameInstance.Game.CurrentSession.RoleID = ((KuaFuLoginManager.ChangeRole != 1) ? KuaFuLoginManager.KuaFuServerLoginData.RoleId : KuaFuLoginManager.NewRoleID);
			GameInstance.Game.CurrentSession.LocalRoleID = ((KuaFuLoginManager.ChangeRole != 1) ? KuaFuLoginManager.KuaFuServerLoginData.RoleId : KuaFuLoginManager.NewRoleID);
			return true;
		}
		return false;
	}

	public static bool IsKuaFuLoginMode1(ref string uid, ref string name, ref string lastTime, ref string isadult, ref string token)
	{
		if (KuaFuLoginManager.KuaFuServerLoginData == KuaFuLoginManager.KuaFuServerLoginDataKuaFu && KuaFuLoginManager.KuaFuServerLoginDataKuaFu != null && KuaFuLoginManager.KuaFuServerLoginDataKuaFu.RoleId > 0)
		{
			uid = KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken.UserID;
			name = KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken.UserName;
			lastTime = KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken.LastTime;
			isadult = KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken.Isadult;
			token = KuaFuLoginManager.KuaFuServerLoginData.WebLoginToken.SignCode;
			return true;
		}
		return false;
	}

	public static string GetKuaFuLoginString(string normal)
	{
		int num = 0;
		if (Global.Data != null && Global.Data.ServerData != null)
		{
			num = Global.Data.ServerData.ClientInfo;
		}
		string result;
		if (KuaFuLoginManager.KuaFuServerLoginData == KuaFuLoginManager.KuaFuServerLoginDataKuaFu && KuaFuLoginManager.KuaFuServerLoginDataKuaFu != null && KuaFuLoginManager.KuaFuServerLoginDataKuaFu.RoleId > 0)
		{
			KuaFuLoginManager.KuaFuServerLoginDataKuaFu.Private = num;
			result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				normal,
				KuaFuLoginManager.KuaFuServerLoginData.RoleId,
				KuaFuLoginManager.KuaFuServerLoginData.GameId,
				KuaFuLoginManager.KuaFuServerLoginData.GameType,
				KuaFuLoginManager.KuaFuServerLoginData.ServerId,
				Convert.ToBase64String(DataHelper.ObjectToBytes<KuaFuServerLoginData>(KuaFuLoginManager.KuaFuServerLoginData)),
				KuaFuLoginManager.KuaFuServerLoginData.ServerPort
			});
		}
		else
		{
			KuaFuLoginManager.KuaFuServerLoginDataOriginal.Private = num;
			result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				normal,
				0,
				0,
				0,
				0,
				Convert.ToBase64String(DataHelper.ObjectToBytes<KuaFuServerLoginData>(KuaFuLoginManager.KuaFuServerLoginDataOriginal)),
				num
			});
		}
		return result;
	}

	public static KuaFuServerLoginData KuaFuServerLoginData = null;

	private static int NewRoleID = 0;

	private static byte ChangeRole = 0;

	public static KuaFuServerLoginData KuaFuServerLoginDataKuaFu = null;

	public static KuaFuServerLoginData KuaFuServerLoginDataOriginal = new KuaFuServerLoginData();
}
