using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Tmsk.Xml;
using UnityEngine;

public class AndroidSDKPlugin
{
	public static AndroidJavaObject UnityPlayerActivity
	{
		get
		{
			if (AndroidSDKPlugin.m_UnityActivity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				AndroidSDKPlugin.m_UnityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return AndroidSDKPlugin.m_UnityActivity;
		}
	}

	public static AndroidJavaClass TMUtils
	{
		get
		{
			if (AndroidSDKPlugin.m_TMUtils == null)
			{
				AndroidSDKPlugin.m_TMUtils = new AndroidJavaClass("com.tianmashikong.tool.TMUtils");
			}
			return AndroidSDKPlugin.m_TMUtils;
		}
	}

	public static string PlatName
	{
		get
		{
			if (string.IsNullOrEmpty(AndroidSDKPlugin.m_platName))
			{
				AndroidSDKPlugin.m_platName = AndroidSDKPlugin.TMUtils.CallStatic<string>("PlatName", new object[0]);
			}
			return AndroidSDKPlugin.m_platName;
		}
		set
		{
			AndroidSDKPlugin.m_platName = value;
		}
	}

	public static void SDKInit()
	{
		PlatSDKMgr.WXInit();
	}

	public static void Login(string args = "")
	{
		if (string.IsNullOrEmpty(args))
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call(AndroidSDKPlugin.androidLogin, new object[0]);
		}
		else
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call(AndroidSDKPlugin.androidLogin, new object[]
			{
				args
			});
		}
	}

	public static void LoginOut(string args)
	{
		Global.RootParams["uid"] = "-1";
		AndroidSDKPlugin.UnityPlayerActivity.Call(AndroidSDKPlugin.androidLoginOut, new object[0]);
	}

	public static void ReLogin()
	{
		Global.RootParams["uid"] = "-1";
		if (!PlatSDKEventListening.m_canchange)
		{
			return;
		}
		PlatSDKEventListening.ChangeAccountButtonWaiting();
		string platName = AndroidSDKPlugin.PlatName;
		if (platName != null)
		{
			if (AndroidSDKPlugin.<>f__switch$mapE == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(3);
				dictionary.Add("TM", 0);
				dictionary.Add("YYB", 1);
				dictionary.Add("QQ", 1);
				AndroidSDKPlugin.<>f__switch$mapE = dictionary;
			}
			int num;
			if (AndroidSDKPlugin.<>f__switch$mapE.TryGetValue(platName, ref num))
			{
				if (num == 0)
				{
					return;
				}
				if (num == 1)
				{
					Super.platformLogin.gameObject.SetActive(false);
					Super.ShowTencentLogin(MainGame._current.Stage);
					return;
				}
			}
		}
		AndroidSDKPlugin.UnityPlayerActivity.Call(AndroidSDKPlugin.androidReLogin, new object[0]);
	}

	public static void OnAppQuit()
	{
		AndroidSDKPlugin.UnityPlayerActivity.Call("OnAppQuit", new object[0]);
	}

	public static void DoPay(string orderId)
	{
		if (PlatSDKMgr._payMoney == 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"DoPay return as [PlatSDKMgr._payMoney == 0]"
			});
			return;
		}
		int payMoney = PlatSDKMgr._payMoney;
		if (string.IsNullOrEmpty(PlatSDKMgr._userInfo))
		{
			MUDebug.Log<string>(new string[]
			{
				"DoPay return as [string.IsNullOrEmpty (_userInfo)]"
			});
			return;
		}
		string userInfo = PlatSDKMgr._userInfo;
		string strServerName = Global.Data.ServerData.LastServer.strServerName;
		if (AndroidSDKPlugin.PlatName == "KP" && PlatSDKMgr.productId == "10000")
		{
			PlatSDKMgr.productId = "9";
		}
		if (AndroidSDKPlugin.PlatName == "TM")
		{
			return;
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add(GameInfoField.GAME_USER_ROLEID, Global.Data.RoleID.ToString());
		dictionary.Add(GameInfoField.GAME_USER_ROLE_NAME, Global.Data.RoleName);
		dictionary.Add(GameInfoField.GAME_USER_SERVER_ID, Global.Data.GameServerID + string.Empty);
		dictionary.Add(GameInfoField.GAME_USER_SERVER_NAME, strServerName);
		dictionary.Add(GameInfoField.GAME_USER_PARTY_NAME, Global.Data.roleData.BHName);
		dictionary.Add(GameInfoField.GAME_TOKEN, PlatSDKMgr._platToken);
		dictionary.Add(GameInfoField.GAME_PLAT_UID, Global.Data.UserID.Substring(AndroidSDKPlugin.PlatName.Length));
		dictionary.Add(GameInfoField.GAME_USER_GAMER_VIP, "VIP" + Global.Data.roleData.VIPLevel.ToString());
		dictionary.Add(GameInfoField.GAME_USER_LV, Global.Data.roleData.Level.ToString());
		dictionary.Add(GameInfoField.GAME_USER_BALANCE, Global.Data.roleData.UserMoney.ToString());
		dictionary.Add(GameInfoField.GAME_PAY_UERINFO, userInfo);
		dictionary.Add(GameInfoField.GAME_EXCHANGE_RATE, PlatSDKMgr._ExchageRate + string.Empty);
		dictionary.Add(GameInfoField.GAME_PAY_ORDER, PlatSDKMgr._orderId);
		dictionary.Add(GameInfoField.GAME_PAY_MONEY, string.Empty + payMoney);
		dictionary.Add(GameInfoField.GAME_GOOD_ID, PlatSDKMgr.productId);
		dictionary.Add(GameInfoField.GAME_GOOD_NAME, payMoney * PlatSDKMgr._ExchageRate + Global.GetLang("钻石"));
		dictionary.Add(GameInfoField.GAME_PAY_CALLBACK, PlatSDKMgr.GetPaymentVerifyServerURL());
		dictionary.Add(GameInfoField.GAME_PRODUCT_ID, PlatSDKMgr.productId);
		dictionary.Add(GameInfoField.GAME_PLAT_TOKEN, PlatSDKMgr._platToken);
		dictionary.Add(GameInfoField.GAME_CURRENCY, Global.GetLang("钻石"));
		dictionary.Add(GameInfoField.GAME_UID, Global.Data.UserID.Substring(AndroidSDKPlugin.PlatName.Length));
		dictionary.Add(GameInfoField.GAME_ITEM_ID, string.Empty + PlatSDKMgr.zhigouId);
		string text = MUJson.jsonEncode(dictionary);
		if (AndroidSDKPlugin.PlatName == "YNGW")
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("KunlunPay", new object[]
			{
				text
			});
			Debug.Log("YNGW充值");
		}
		else if (AndroidSDKPlugin.PlatName == "YNGoogle")
		{
			if (PlatSDKMgr.productId == "1")
			{
				AndroidSDKPlugin.UnityPlayerActivity.Call("KunlunPay", new object[]
				{
					text
				});
				Debug.Log("KunlunPay充值");
			}
			else
			{
				AndroidSDKPlugin.UnityPlayerActivity.Call("GooglePay", new object[]
				{
					text
				});
				Debug.Log("GooglePay充值");
			}
		}
	}

	public static void ShowUserCenter()
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("roleid", Global.Data.RoleID.ToString());
			dictionary.Add("serverid", Global.Data.GameServerID.ToString());
			string text = MUJson.jsonEncode(dictionary);
			AndroidSDKPlugin.UnityPlayerActivity.Call(AndroidSDKPlugin.androidShowUserCenter, new object[]
			{
				text
			});
		}
		catch (UnityException ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"sdk ShowUserCenter " + ex.ToString()
			});
		}
	}

	public static string GetAndriodPackName()
	{
		string result = "com.tianmashikong.qmqj";
		try
		{
			result = AndroidSDKPlugin.UnityPlayerActivity.Call<string>("getPackageName", new object[0]);
		}
		catch (UnityException ex)
		{
			MUDebug.Log<string>(new string[]
			{
				ex.ToString()
			});
		}
		return result;
	}

	public static void OnCreateRole(string rolename, string reqTime)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("roleId", Global.Data.RoleID.ToString());
			dictionary.Add("roleName", Global.Data.RoleName);
			dictionary.Add("lv", "1");
			dictionary.Add("rolePower", "0");
			dictionary.Add("vip", "VIP0");
			string text = MUJson.jsonEncode(dictionary);
			AndroidSDKPlugin.UnityPlayerActivity.Call("CreateRole", new object[]
			{
				text
			});
		}
		catch (UnityException ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"sdk OnCreateRole " + ex.ToString()
			});
		}
	}

	public static void EnterGameCallback()
	{
		try
		{
			if (!AndroidSDKPlugin.PlatName.Contains("TM"))
			{
				Dictionary<string, string> dictionary = new Dictionary<string, string>();
				dictionary.Add("roleId", Global.Data.RoleID.ToString());
				dictionary.Add("roleName", Global.Data.RoleName);
				dictionary.Add("lv", Global.Data.roleData.Level.ToString());
				dictionary.Add("rolePower", Global.Data.roleData.CombatForce.ToString());
				dictionary.Add("vip", "VIP" + Global.Data.roleData.VIPLevel.ToString());
				string text = MUJson.jsonEncode(dictionary);
				AndroidSDKPlugin.UnityPlayerActivity.Call("EnterGame", new object[]
				{
					text,
					Global.Data.GameServerID.ToString()
				});
			}
		}
		catch (UnityException ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"entergame " + ex.ToString()
			});
		}
	}

	public static void UserUpLevelCallBack(int newLevel)
	{
		try
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("roleId", Global.Data.RoleID.ToString());
			dictionary.Add("roleName", Global.Data.RoleName);
			dictionary.Add("lv", Global.Data.roleData.Level.ToString());
			dictionary.Add("rolePower", Global.Data.roleData.CombatForce.ToString());
			dictionary.Add("vip", "VIP" + Global.Data.roleData.VIPLevel.ToString());
			string text = MUJson.jsonEncode(dictionary);
			AndroidSDKPlugin.UnityPlayerActivity.Call("UserUpLevelListener", new object[]
			{
				text
			});
		}
		catch (UnityException ex)
		{
			MUDebug.Log<string>(new string[]
			{
				"sdk UserUpLevelCallBack " + ex.ToString()
			});
		}
	}

	private static string GetCreateRoleTime()
	{
		DateTime dateTime = Convert.ToDateTime(Global.Data.roleData.RegTime);
		DateTime dateTime2;
		dateTime2..ctor(1970, 1, 1, 8, 0, 0);
		return dateTime.Subtract(dateTime2).TotalSeconds.ToString();
	}

	public static string GameName
	{
		get
		{
			if (string.IsNullOrEmpty(AndroidSDKPlugin._gameName))
			{
				AndroidSDKPlugin._gameName = AndroidSDKPlugin.TMUtils.CallStatic<string>("GetAppMetaValue", new object[]
				{
					AndroidSDKPlugin.UnityPlayerActivity,
					"TMSK_GAMENAME"
				});
			}
			Debug.Log("gameName=" + AndroidSDKPlugin._gameName);
			return AndroidSDKPlugin._gameName;
		}
	}

	public static string TmskChannel
	{
		get
		{
			string result;
			try
			{
				if (string.IsNullOrEmpty(AndroidSDKPlugin._tmskchannel))
				{
					AndroidSDKPlugin._tmskchannel = AndroidSDKPlugin.TMUtils.CallStatic<string>("GetAppMetaValue", new object[]
					{
						AndroidSDKPlugin.UnityPlayerActivity,
						"tmsk_channel"
					});
				}
				result = AndroidSDKPlugin._tmskchannel;
			}
			catch (Exception ex)
			{
				result = PlatSDKMgr.PlatName;
			}
			return result;
		}
	}

	public static string TmskChnspecid
	{
		get
		{
			string result;
			try
			{
				if (string.IsNullOrEmpty(AndroidSDKPlugin._tmsk_chnspecid))
				{
					AndroidSDKPlugin._tmsk_chnspecid = AndroidSDKPlugin.TMUtils.CallStatic<string>("GetAppMetaValue", new object[]
					{
						AndroidSDKPlugin.UnityPlayerActivity,
						"tmsk_chnspecid"
					});
				}
				result = AndroidSDKPlugin._tmsk_chnspecid;
			}
			catch (Exception ex)
			{
				result = PlatSDKMgr.PlatName;
			}
			return result;
		}
	}

	public static string IMEI
	{
		get
		{
			return AndroidSDKPlugin._IMEI;
		}
	}

	public static string IMSI
	{
		get
		{
			if (string.IsNullOrEmpty(AndroidSDKPlugin._IMSI))
			{
				AndroidSDKPlugin._IMSI = AndroidSDKPlugin.TMUtils.CallStatic<string>("IMSI", new object[0]);
			}
			return AndroidSDKPlugin._IMSI;
		}
	}

	public static string WifiName
	{
		get
		{
			if (string.IsNullOrEmpty(AndroidSDKPlugin._wifi_SSID))
			{
				AndroidSDKPlugin._wifi_SSID = AndroidSDKPlugin.TMUtils.CallStatic<string>("Wifi_SSID", new object[0]);
			}
			return AndroidSDKPlugin._wifi_SSID;
		}
	}

	public static string CPU
	{
		get
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			string text = androidJavaClass.GetStatic<string>("HARDWARE") + androidJavaClass.GetStatic<string>("BOARD");
			return text.Replace(" ", string.Empty);
		}
	}

	public static string GPU
	{
		get
		{
			return SystemInfo.graphicsDeviceName.Replace(" ", string.Empty);
		}
	}

	public static string SystemVersion
	{
		get
		{
			return SystemInfo.operatingSystem.Replace(" ", string.Empty);
		}
	}

	public static string Brand
	{
		get
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			string @static = androidJavaClass.GetStatic<string>("BRAND");
			return @static.Replace(" ", string.Empty);
		}
	}

	public static string MODEL
	{
		get
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("android.os.Build");
			string @static = androidJavaClass.GetStatic<string>("MODEL");
			return @static.Replace(" ", string.Empty);
		}
	}

	public static string TMGameId
	{
		get
		{
			if (string.IsNullOrEmpty(AndroidSDKPlugin.m_tmGameId))
			{
				AndroidSDKPlugin.m_tmGameId = AndroidSDKPlugin.TMUtils.CallStatic<string>("TMGameId", new object[0]);
			}
			return AndroidSDKPlugin.m_tmGameId;
		}
		set
		{
			AndroidSDKPlugin.m_tmGameId = value;
		}
	}

	public static string TM_Chntrack()
	{
		return PlayerPrefs.GetString("chntrack", "0");
	}

	public static string LastVersion
	{
		get
		{
			return PlayerPrefs.GetString("lastversion", "0");
		}
		set
		{
			PlayerPrefs.SetString("lastversion", value);
		}
	}

	public static void SetChntrack()
	{
		if (AndroidSDKPlugin.LastVersion.Equals(PlatSDKMgr.Version))
		{
			return;
		}
		if (!PlayerPrefs.HasKey("chntrack"))
		{
			PlayerPrefs.SetString("chntrack", AndroidSDKPlugin.TmskChannel);
		}
		else
		{
			string @string = PlayerPrefs.GetString("chntrack", "0");
			PlayerPrefs.SetString("chntrack", @string + "," + AndroidSDKPlugin.TmskChannel);
		}
		AndroidSDKPlugin.LastVersion = PlatSDKMgr.Version;
	}

	public static void showAchievements()
	{
		AndroidSDKPlugin.UnityPlayerActivity.Call("showAchievements", new object[0]);
	}

	public static void PlatformTongJi(int chengJiuID)
	{
		XElement gameResXml = Global.GetGameResXml("Config/TongJiPlatform.Xml");
		List<XElement> xelementList = Global.GetXElementList(Global.GetXElement(gameResXml, "ChengJiu"), "*");
		foreach (XElement xelement in xelementList)
		{
			if (chengJiuID == Global.GetXElementAttributeInt(xelement, "ChengJiuID"))
			{
				int xelementAttributeInt = Global.GetXElementAttributeInt(xelement, "TjType");
				string xelementAttributeStr = Global.GetXElementAttributeStr(xelement, "TJID");
				int xelementAttributeInt2 = Global.GetXElementAttributeInt(xelement, "ChengJiu");
				string xelementAttributeStr2 = Global.GetXElementAttributeStr(xelement, "Description");
				AndroidSDKPlugin.PlatformTongJiCallback(xelementAttributeInt, xelementAttributeStr, xelementAttributeInt2, xelementAttributeStr2);
			}
		}
	}

	private static void PlatformTongJiCallback(int type, string tjid, int tjValue, string description)
	{
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"type == ",
				type,
				"        tjid ==  ",
				tjid,
				"    tjValue == ",
				tjValue,
				"description == ",
				description
			})
		});
		if (type == 1)
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("unlockAchievements", new object[]
			{
				tjid
			});
		}
		if (type == 2)
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("setLevelChengJiu", new object[]
			{
				description
			});
		}
		if (type == 3)
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("setPartyLevel", new object[0]);
		}
	}

	public static void ShowSubmitScore(int type, int value)
	{
		if (type == 1)
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("submitScore", new object[]
			{
				"CgkI1sXDxosUEAIQHw",
				value
			});
		}
		else if (type == 2)
		{
			AndroidSDKPlugin.UnityPlayerActivity.Call("submitScore", new object[]
			{
				"CgkI1sXDxosUEAIQIA",
				value
			});
		}
	}

	private static string androidLogin = "Login";

	private static string androidLoginOut = "LoginOut";

	private static string androidReLogin = "ReLogin";

	private static string androidOnAppQuit = "OnAppQuit";

	private static string androidPay = "Pay";

	private static string androidShowUserCenter = "ShowUserCenter";

	private static AndroidJavaObject m_UnityActivity;

	public static AndroidJavaClass m_TMUtils;

	public static string m_platName = string.Empty;

	private static string _gameName;

	private static string _tmskchannel = string.Empty;

	private static string _tmsk_chnspecid = string.Empty;

	private static string _IMEI = string.Empty;

	private static string _IMSI = string.Empty;

	private static string _wifi_SSID = string.Empty;

	public static string m_tmGameId = string.Empty;
}
