using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class PlatformAccountVerify
{
	private static string KupaiLoginVerifyCallback(WWW w)
	{
		string result = "-999";
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				ServerVerifySIDDataToken serverVerifySIDDataToken = DataHelper.BytesToObject<ServerVerifySIDDataToken>(bytes, 0, bytes.Length);
				if (serverVerifySIDDataToken != null)
				{
					result = serverVerifySIDDataToken.strPlatformUserID;
					Global.RootParams["uid"] = serverVerifySIDDataToken.strPlatformUserID;
					Global.RootParams["n"] = serverVerifySIDDataToken.strAccountName;
					Global.RootParams["t"] = string.Empty + serverVerifySIDDataToken.lTime;
					Global.RootParams["cm"] = serverVerifySIDDataToken.strCM;
					Global.RootParams["token"] = serverVerifySIDDataToken.strToken;
					PlatSDKMgr._platToken = serverVerifySIDDataToken.strPlatformToken;
				}
			}
			else
			{
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("连接验证服务器失败，请检查网络"), -1, -1, -1, -1, false);
			}
		}
		return result;
	}

	public static void GetPlatformUIDCallback(WWW w)
	{
		string strPlatUID = "-999";
		if (PlatSDKMgr.PlatName.Equals("KP"))
		{
			strPlatUID = PlatformAccountVerify.KupaiLoginVerifyCallback(w);
			PlatSDKMgr.PlatVerifyResult(strPlatUID);
			return;
		}
		if (PlatSDKMgr.PlatName.Equals("SHUN"))
		{
			strPlatUID = PlatformAccountVerify.KupaiLoginVerifyCallback(w);
			PlatSDKMgr.PlatVerifyResult(strPlatUID);
			return;
		}
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				ServerVerifySIDData serverVerifySIDData = DataHelper.BytesToObject<ServerVerifySIDData>(bytes, 0, bytes.Length);
				if (serverVerifySIDData != null)
				{
					strPlatUID = serverVerifySIDData.strPlatformUserID;
					MUDebug.Log<string>(new string[]
					{
						"strPlatformUserID:" + serverVerifySIDData.strPlatformUserID
					});
					Global.RootParams["uid"] = serverVerifySIDData.strPlatformUserID;
					Global.RootParams["n"] = serverVerifySIDData.strAccountName;
					Global.RootParams["t"] = string.Empty + serverVerifySIDData.lTime;
					Global.RootParams["cm"] = serverVerifySIDData.strCM;
					Global.RootParams["token"] = serverVerifySIDData.strToken;
					PlatSDKMgr.FangChenMi();
					PlatSDKMgr.TencentNotice(1);
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"returnBytes is null" + w.error
				});
				Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("连接验证服务器失败，请检查网络"), -1, -1, -1, -1, false);
			}
		}
		PlatSDKMgr.PlatVerifyResult(strPlatUID);
	}

	public static void GetPlatformUID(string strSession)
	{
		MUDebug.Log<string>(new string[]
		{
			"GetPlatformUID strSession:" + strSession
		});
		Super.ShowNetWaiting(Global.GetLang("正在进行帐号验证..."));
		ClientVerifySIDData clientVerifySIDData = new ClientVerifySIDData();
		clientVerifySIDData.strSID = strSession;
		clientVerifySIDData.lTime = Global.GetTimeStamp();
		clientVerifySIDData.strMD5 = MD5Helper.get_md5_string("tmsk_mu_06" + clientVerifySIDData.strSID + clientVerifySIDData.lTime);
		SimpleHttpTask.HttpPost(PlatSDKMgr.GetPlatVerifyURL(), null, DataHelper.ObjectToBytes<ClientVerifySIDData>(clientVerifySIDData), new SimpleHttpTask.HttpCallback(PlatformAccountVerify.GetPlatformUIDCallback), 10f);
	}
}
