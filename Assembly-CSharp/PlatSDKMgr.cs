using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using HSGameEngine.JavaPlugins;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class PlatSDKMgr
{
	// Note: this type is marked as 'beforefieldinit'.
	static PlatSDKMgr()
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("TM", 0);
		dictionary.Add("UC", 1);
		dictionary.Add("MI", 2);
		dictionary.Add("PP", 3);
		dictionary.Add("KY", 4);
		dictionary.Add("QH360", 5);
		dictionary.Add("XY", 6);
		dictionary.Add("BD", 7);
		dictionary.Add("WDJ", 8);
		dictionary.Add("QQ", 9);
		dictionary.Add("APPS", 10);
		dictionary.Add("DL", 11);
		dictionary.Add("BD91", 12);
		dictionary.Add("I4", 13);
		dictionary.Add("iTools", 15);
		dictionary.Add("HM", 16);
		dictionary.Add("XYMU", 17);
		dictionary.Add("TM_TEST", 18);
		dictionary.Add("IPG", 19);
		dictionary.Add("TB", 20);
		dictionary.Add("OPPO", 21);
		dictionary.Add("PPS", 22);
		dictionary.Add("VIVO", 23);
		dictionary.Add("HW", 24);
		dictionary.Add("KP", 25);
		dictionary.Add("KuGou", 26);
		dictionary.Add("YK", 27);
		dictionary.Add("MZW", 28);
		dictionary.Add("PPTV", 29);
		dictionary.Add("BF", 30);
		dictionary.Add("JL", 31);
		dictionary.Add("LENOVO", 32);
		dictionary.Add("KUWO", 33);
		dictionary.Add("ANZHI", 34);
		dictionary.Add("BDZS", 35);
		dictionary.Add("QMQJ", 36);
		dictionary.Add("ITAPK", 37);
		dictionary.Add("SOGOU", 38);
		dictionary.Add("YYH", 39);
		dictionary.Add("WAN37", 40);
		dictionary.Add("MIAN", 41);
		dictionary.Add("YIWAN", 42);
		dictionary.Add("YOUYOU", 43);
		dictionary.Add("YYB", 44);
		dictionary.Add("CH", 45);
		dictionary.Add("TIYAN", 46);
		dictionary.Add("SHUN", 47);
		dictionary.Add("YYTY", 48);
		dictionary.Add("DIDI", 49);
		dictionary.Add("LESHI", 50);
		dictionary.Add("XYZS", 51);
		dictionary.Add("XXZS", 52);
		dictionary.Add("SINA", 53);
		dictionary.Add("KAOPU", 54);
		dictionary.Add("WAN51", 55);
		dictionary.Add("YNGW", 56);
		dictionary.Add("YNGoogle", 57);
		dictionary.Add("YNIOS", 58);
		PlatSDKMgr._PlatCodeDict = dictionary;
		PlatSDKMgr._uid = string.Empty;
		PlatSDKMgr._sid = string.Empty;
		PlatSDKMgr._token = string.Empty;
		PlatSDKMgr._name = string.Empty;
		PlatSDKMgr._lastTime = string.Empty;
		PlatSDKMgr._isadult = string.Empty;
		PlatSDKMgr._payMoney = 0;
		PlatSDKMgr._orderId = string.Empty;
		PlatSDKMgr._verifyData = null;
		PlatSDKMgr.productId = string.Empty;
		PlatSDKMgr.zhigouId = 0;
		PlatSDKMgr.m_toGame = null;
		PlatSDKMgr._UnityActivity = null;
		PlatSDKMgr.m_version = string.Empty;
		PlatSDKMgr.m_buildTime = string.Empty;
		PlatSDKMgr.m_platform = -1;
		PlatSDKMgr.m_platName = string.Empty;
		PlatSDKMgr.m_tmGameId = string.Empty;
	}

	public static Dictionary<string, int> PlatCodeDict
	{
		get
		{
			return PlatSDKMgr._PlatCodeDict;
		}
	}

	public static AndroidJavaObject UnityPlayerActivity
	{
		get
		{
			if (PlatSDKMgr._UnityActivity == null)
			{
				AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
				PlatSDKMgr._UnityActivity = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
			}
			return PlatSDKMgr._UnityActivity;
		}
	}

	public static string Version
	{
		get
		{
			if (string.IsNullOrEmpty(PlatSDKMgr.m_version))
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.tool.TMUtils"))
				{
					PlatSDKMgr.m_version = androidJavaClass.CallStatic<string>("GetVersionName", new object[0]);
				}
			}
			return PlatSDKMgr.m_version;
		}
	}

	public static string BuildTime
	{
		get
		{
			if (string.IsNullOrEmpty(PlatSDKMgr.m_buildTime))
			{
				PlatSDKMgr.ReadAndroidPlatcode();
			}
			return PlatSDKMgr.m_buildTime;
		}
	}

	public static void ReadAndroidPlatcode()
	{
		TextAsset textAsset = Resources.Load("platconfig") as TextAsset;
		if (null == textAsset)
		{
			PlatSDKMgr.m_platName = "TM";
			PlatSDKMgr.m_platform = 0;
			return;
		}
		XElement xml = XElement.Parse(textAsset.text);
		int.TryParse(Global.ReadXmlConfigStr(xml, "root", "platID"), ref PlatSDKMgr.m_platform);
		PlatSDKMgr.m_platName = Global.ReadXmlConfigStr(xml, "root", "platName");
		PlatSDKMgr.m_version = Global.ReadXmlConfigStr(xml, "root", "version");
		if (string.IsNullOrEmpty(PlatSDKMgr.m_version))
		{
			PlatSDKMgr.m_version = "1.0.1";
		}
		PlatSDKMgr.m_buildTime = Global.ReadXmlConfigStr(xml, "root", "buildTime");
		if (string.IsNullOrEmpty(PlatSDKMgr.m_buildTime))
		{
			PlatSDKMgr.m_buildTime = "2014-10-10";
		}
	}

	public static int Platform
	{
		get
		{
			if (PlatSDKMgr.m_platform == -1)
			{
				PlatSDKMgr.ReadAndroidPlatcode();
			}
			return PlatSDKMgr.m_platform;
		}
	}

	public static string PlatName
	{
		get
		{
			if (string.IsNullOrEmpty(PlatSDKMgr.m_platName))
			{
				PlatSDKMgr.m_platName = AndroidSDKPlugin.PlatName;
			}
			return PlatSDKMgr.m_platName;
		}
		set
		{
			PlatSDKMgr.m_platName = value;
		}
	}

	public static string TMGameId
	{
		get
		{
			if (string.IsNullOrEmpty(PlatSDKMgr.m_tmGameId))
			{
				PlatSDKMgr.m_tmGameId = AndroidSDKPlugin.TMGameId;
			}
			return PlatSDKMgr.m_tmGameId;
		}
		set
		{
			PlatSDKMgr.m_tmGameId = value;
		}
	}

	public static string GetAndriodPackName()
	{
		return PlatSDKMgr.UnityPlayerActivity.Call<string>("getPackageName", new object[0]);
	}

	public static void SDKInit()
	{
		AndroidSDKPlugin.SDKInit();
		PlatSDKMgr.InitBuglySDK();
	}

	public static bool Login(GameObject obj, string args = "")
	{
		PlatSDKMgr._LoginObj = obj;
		if (!PlatSDKEventListening.m_canchange)
		{
			return true;
		}
		PlatSDKEventListening.ChangeAccountButtonWaiting();
		AndroidSDKPlugin.Login(args);
		return true;
	}

	public static void LoginOut(string args)
	{
		Global.RootParams["uid"] = "-1";
		AndroidSDKPlugin.LoginOut(args);
	}

	public static void ReLogin()
	{
		Global.RootParams["uid"] = "-1";
		PlatSDKMgr._isEnterGame = false;
		AndroidSDKPlugin.ReLogin();
	}

	public static void DoPay(string orderId)
	{
		if (PlatSDKMgr._payMoney == 0)
		{
			return;
		}
		int payMoney = PlatSDKMgr._payMoney;
		string userInfo = PlatSDKMgr._userInfo;
		AndroidSDKPlugin.DoPay(orderId);
	}

	public static void ShowWeb()
	{
		if (PlatSDKMgr.PlatName == "YNGW" || PlatSDKMgr.PlatName == "YNGoogle")
		{
			PlatSDKMgr.UnityPlayerActivity.Call("ShowWeb", new object[0]);
		}
	}

	public static string LoginCmd()
	{
		return StringUtil.substitute("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
		{
			20140624,
			PlatSDKMgr._uid,
			PlatSDKMgr._name,
			PlatSDKMgr._lastTime,
			PlatSDKMgr._isadult,
			PlatSDKMgr._token
		});
	}

	public static string GetPlatVerifyURL()
	{
		string platName = PlatSDKMgr.PlatName;
		return Global.VerifyAccountServerURL + platName + "Login/SidInfo.aspx";
	}

	public static string GetAppealURL()
	{
		return Global.VerifyAccountServerURL + PlatSDKMgr.PlatName + "Login/Appeal.aspx";
	}

	public static void PlatVerifyResult(string strPlatUID)
	{
		Dictionary<int, string> jinDeng = Global.GetJinDeng();
		if (jinDeng.Count <= 0)
		{
			return;
		}
		Super.HideNetWaiting();
		string message = string.Empty;
		if ("-1" == strPlatUID)
		{
			message = jinDeng.GetValue(-1);
			Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, 0, false, default(Vector3));
		}
		else if ("-2" == strPlatUID)
		{
			message = jinDeng.GetValue(-2);
			Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, 0, false, default(Vector3));
		}
		else if (strPlatUID.StartsWith("-3"))
		{
			string[] array = strPlatUID.Split(new char[]
			{
				':'
			});
			int num = 0;
			if (array != null && array.Length >= 2)
			{
				num = ConvertExt.SafeConvertToInt32(array[1]);
			}
			if (num > 604800)
			{
				message = jinDeng.GetValue(-4);
				Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, 0, false, default(Vector3));
			}
			else
			{
				message = jinDeng.GetValue(-3);
				Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, num + 180, true, default(Vector3));
			}
		}
		else if ("-4" == strPlatUID)
		{
			message = jinDeng.GetValue(-5);
			Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, 0, false, default(Vector3));
		}
		else if (strPlatUID.StartsWith("-"))
		{
			message = Global.GetLang("网络状况不佳，错误码:") + strPlatUID;
			Super.ShowLoginYiChang(Global.MainStage, Global.GetLang("确  定"), message, 0, false, default(Vector3));
		}
		else
		{
			if (null != Super.tencentLogin)
			{
				Super.tencentLogin.DPSelectedItem(Super.tencentLogin, new DPSelectedItemEventArgs());
				if (Super.platformLogin == null)
				{
					Super.ShowPlatformUserLogin(MainGame._current.Stage, true);
				}
				else
				{
					Super.platformLogin.gameObject.SetActive(true);
				}
			}
			if (null != PlatSDKMgr.m_toGame)
			{
				PlatSDKMgr.m_toGame.NextStep(PlatSDKMgr.m_toGame, new NextStepEventArgs
				{
					StepType = 0
				});
				PlatSDKMgr.m_toGame = null;
			}
		}
	}

	private static void SelectServerTimer_Tick(object sender, object e)
	{
		if (PlatSDKMgr.timerSelectServer != null)
		{
			PlatSDKMgr.timerSelectServer.Stop();
			if (PlatSDKMgr.m_toGame != null)
			{
				PlatSDKMgr.m_toGame.NextStep(PlatSDKMgr.m_toGame, new NextStepEventArgs
				{
					StepType = 0
				});
				PlatSDKMgr.m_toGame = null;
			}
			PlatSDKMgr.timerSelectServer = null;
		}
	}

	public static void EnterGameCallback()
	{
		PlatSDKMgr._isEnterGame = true;
		AndroidSDKPlugin.EnterGameCallback();
		CheckSFAndWorker.SingleInstance.Init();
	}

	public static long ParseLoginMsgFromSDK(string msg)
	{
		if (PlatSDKMgr.PlatName == "TM")
		{
			return -1L;
		}
		if (PlatSDKMgr.PlatName == "M37")
		{
			Hashtable hashtable = (Hashtable)MUJson.jsonDecode(msg);
			PlatSDKMgr._chargeUid = hashtable["uid"].ToString();
		}
		if (PlatSDKMgr.PlatName == "JL")
		{
			Hashtable hashtable2 = (Hashtable)MUJson.jsonDecode(msg);
			PlatSDKMgr._chargeUid = hashtable2["userId"].ToString();
		}
		PlatformAccountVerify.GetPlatformUID(msg);
		return -1L;
	}

	public static void UserUpLevelCallBack(int newLevel)
	{
		AndroidSDKPlugin.UserUpLevelCallBack(newLevel);
	}

	public static void OnAppQuit()
	{
		MUDebug.Log<string>(new string[]
		{
			"PlatName========" + PlatSDKMgr.PlatName
		});
		AndroidSDKPlugin.OnAppQuit();
	}

	public static void ShowUserCenter()
	{
		AndroidSDKPlugin.ShowUserCenter();
	}

	public static void FangChenMi()
	{
		if (PlatSDKMgr.PlatName == "QH360")
		{
			PlatSDKMgr.UnityPlayerActivity.Call("AntiAddictionQuery", new object[]
			{
				Global.RootParams["uid"].Substring(PlatSDKMgr.PlatName.Length),
				Global.RootParams["token"]
			});
		}
	}

	public static string GetOrderIdHttpUrl()
	{
		string text = PlatSDKMgr.PlatName + "Login";
		string text2 = Global.PayServerURL + PlatSDKMgr.PlatName + "Login/GetExchangeOrder.aspx";
		MUDebug.Log<string>(new string[]
		{
			"YNAndroid_GetOrderIdHttpUrl()_strURL=" + text2
		});
		return text2;
	}

	public static string GetPaymentVerifyServerURL()
	{
		string text = PlatSDKMgr.PlatName;
		if (!PlatSDKMgr.PlatName.Equals("APPS"))
		{
			text = "QMQJ";
			string text2 = "001";
			return string.Concat(new string[]
			{
				Global.PayServerURL,
				text,
				"pay/ExchangeResult.aspx?tmchannel=",
				PlatSDKMgr.PlatName,
				text2
			});
		}
		text = PlatSDKMgr.PlatName;
		return Global.VerifyAccountServerURL + text + "pay/ExchangeResult.aspx";
	}

	public static string GetAPPSPaymentVerifyServerURL()
	{
		string text = PlatSDKMgr.PlatName + "Login";
		return Global.PayServerURL + PlatSDKMgr.PlatName + "pay/ExchangeResult.aspx";
	}

	public static string GetAPPSOrderIdHttpUrl()
	{
		string text = PlatSDKMgr.PlatName + "Login";
		string result = Global.PayServerURL + PlatSDKMgr.PlatName + "pay/GetExchangeOrder.aspx";
		if (PlatSDKMgr.PlatName == "KP")
		{
		}
		return result;
	}

	public static void TencentCallserverBack(WWW w)
	{
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			if (w.text.ToLower() == "ok")
			{
				PlayerPrefs.SetInt(PlatSDKMgr.m_tencentPayRecode, 0);
			}
		}
	}

	public static void OnTencentPayCallback(string msg, bool budan = true, bool login = false)
	{
		Hashtable hashtable = (Hashtable)MUJson.jsonDecode(msg);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		PlatSDKMgr.m_tencentPayRecode = hashtable["openid"].ToString();
		hashtable["zoneId"] = Global.Data.GameServerID + string.Empty;
		hashtable["roleName"] = PlatSDKMgr.UrlEncode(Global.Data.roleData.RoleName);
		if (Global.Data.RoleName.Contains("&") || Global.Data.RoleName.Contains("{") || Global.Data.RoleName.Contains("}"))
		{
			hashtable["roleName"] = string.Empty;
		}
		hashtable["roleid"] = Global.Data.roleData.RoleID + string.Empty;
		hashtable["firstId"] = Global.Data.ServerData.LastServer.nFirstLevelServerID + string.Empty;
		string text = "41459fd594d9ff2fa10b2608a143df4";
		string text2 = MD5Helper.get_md5_string(MUJson.jsonEncode(hashtable).ToString() + text).ToLower();
		dictionary.Add("data", MUJson.jsonEncode(hashtable).ToString());
		dictionary.Add("sig", text2);
		if (login)
		{
			dictionary.Add("login", "1");
		}
		else
		{
			dictionary.Add("login", "0");
		}
		SimpleHttpTask.HttpPost(PlatSDKMgr.GetPaymentVerifyServerURL(), dictionary, null, new SimpleHttpTask.HttpCallback(PlatSDKMgr.TencentCallserverBack), 10f);
	}

	public static void TencentReSubmitOrder(bool blogin = false)
	{
		if (PlatSDKMgr.PlatName == "YYB" || PlatSDKMgr.PlatName == "QQ")
		{
			MUDebug.Log<string>(new string[]
			{
				"进行补单处理。。。"
			});
			string andriodPackName = PlatSDKMgr.GetAndriodPackName();
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.sdkmgr.TencentPlugin"))
			{
				if (androidJavaClass != null)
				{
					string text = androidJavaClass.CallStatic<string>("GetUserInfo", new object[0]);
					MUDebug.Log<string>(new string[]
					{
						"补单。。。msg=" + text
					});
					PlatSDKMgr.OnTencentPayCallback(text, false, true);
				}
			}
		}
	}

	public static void QQPay(int money)
	{
		PlatSDKMgr._userInfo = "0";
		PlatSDKMgr._payMoney = money;
		PlatSDKMgr.DoPay("1");
		Super.HideNetWaiting();
	}

	public static void GetQQOrderCallback(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				ServerGetQQOrderData serverGetQQOrderData = DataHelper.BytesToObject<ServerGetQQOrderData>(bytes, 0, bytes.Length);
				if (serverGetQQOrderData != null)
				{
					PlatSDKMgr._orderId = serverGetQQOrderData.strURLParam;
					PlatSDKMgr._userInfo = serverGetQQOrderData.strExchangeOrder;
					PlatSDKMgr.DoPay(serverGetQQOrderData.strURLParam);
				}
			}
		}
	}

	public static void GetOrderIdCallback(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			MUDebug.Log<string>(new string[]
			{
				"YNAndroid************ w is not null and not empty......"
			});
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					"YNAndroid************ returnBytes.Length > 0......" + bytes.Length
				});
				ServerGetOrderData serverGetOrderData = DataHelper.BytesToObject<ServerGetOrderData>(bytes, 0, bytes.Length);
				if (serverGetOrderData != null)
				{
					PlatSDKMgr._orderId = serverGetOrderData.strExchangeOrder;
					if (!string.IsNullOrEmpty(serverGetOrderData.strExtParam))
					{
						PlatSDKMgr._userInfo = serverGetOrderData.strExtParam;
					}
					MUDebug.Log<string>(new string[]
					{
						"YNAndroid_userinfo=" + PlatSDKMgr._userInfo
					});
					MUDebug.Log<string>(new string[]
					{
						"YNAndroid_orderId=" + PlatSDKMgr._orderId
					});
					PlatSDKMgr.DoPay(serverGetOrderData.strExchangeOrder);
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"YNAndroid_returnBytes is null" + w.error
				});
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"YNAndroid************ w is empty"
			});
		}
	}

	public static void Pay(int money, string productId, int zhigouId = 0)
	{
		PlatSDKMgr._userInfo = string.Empty;
		PlatSDKMgr._payMoney = money;
		PlatSDKMgr.productId = productId;
		PlatSDKMgr.zhigouId = zhigouId;
		Super.ShowNetWaiting(null);
		if (PlatSDKMgr._Test)
		{
			PlatSDKMgr._payMoney = 1;
		}
		if (PlatSDKMgr.PlatName == "YYB" || PlatSDKMgr.PlatName == "QQ")
		{
			PlatSDKMgr.QQPay(PlatSDKMgr._payMoney);
			return;
		}
		ClientGetOrderData clientGetOrderData = new ClientGetOrderData();
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		clientGetOrderData.strPlatform = PlatSDKMgr.PlatName;
		if (PlatSDKMgr.PlatName == "KP" && PlatSDKMgr.productId == "10000")
		{
			PlatSDKMgr.productId = "9";
		}
		clientGetOrderData.ProductId = PlatSDKMgr.productId;
		clientGetOrderData.strUserID = userID;
		if (PlatSDKMgr.PlatName == "M37")
		{
			clientGetOrderData.strUserID = userID + "|" + PlatSDKMgr._chargeUid;
		}
		if (PlatSDKMgr.PlatName == "JL")
		{
			clientGetOrderData.strUserID = userID + "|" + PlatSDKMgr._chargeUid;
		}
		clientGetOrderData.lTime = timeStamp;
		clientGetOrderData.strMoney = string.Empty + PlatSDKMgr._payMoney;
		clientGetOrderData.roleName = Global.Data.RoleName;
		clientGetOrderData.roleId = Global.Data.RoleID;
		clientGetOrderData.nServerID = Global.Data.GameServerID;
		clientGetOrderData.firstId = Global.Data.ServerData.LastServer.nFirstLevelServerID + string.Empty;
		clientGetOrderData.strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
		{
			"tmsk_mu_06",
			clientGetOrderData.strPlatform,
			clientGetOrderData.strUserID,
			PlatSDKMgr._payMoney,
			Global.Data.GameServerID,
			timeStamp
		}));
		clientGetOrderData.ZhiGouID = string.Empty + zhigouId;
		clientGetOrderData.strAppId = PlatSDKMgr.TMGameId;
		SimpleHttpTask.HttpPost(PlatSDKMgr.GetOrderIdHttpUrl(), null, DataHelper.ObjectToBytes<ClientGetOrderData>(clientGetOrderData), new SimpleHttpTask.HttpCallback(PlatSDKMgr.GetOrderIdCallback), 10f);
	}

	public static void PaymentVerify(byte[] verifyData, string verNum, string transID, string countryCode, string currencyCode)
	{
		PlatSDKMgr._verifyData = verifyData;
		MUDebug.Log<string>(new string[]
		{
			"Current Order id = [" + PlatSDKMgr._orderId + "]"
		});
		if (PlatSDKMgr._verifyData != null)
		{
			if (!(PlatSDKMgr._orderId != string.Empty))
			{
				string orderId = string.Empty;
				OrderIDsStorage.VerifyInfo verifyInfo = null;
				OrderIDsStorage.GetOrderInfoWithEmptyData(out orderId, out verifyInfo);
				if (verifyInfo != null)
				{
					verifyInfo.verifyData = PlatSDKMgr._verifyData;
					verifyInfo.money = ((PlatSDKMgr._payMoney != 0) ? PlatSDKMgr._payMoney : 6);
					verifyInfo.verNum = verNum;
					verifyInfo.TransID = transID;
					verifyInfo.countryCode = countryCode;
					verifyInfo.currencyCode = currencyCode;
					verifyInfo.verifyState = -1;
					OrderIDsStorage.Add(orderId, verifyInfo);
					OrderIDsStorage.Save();
				}
				else
				{
					orderId = "BUDAN-" + Global.Data.UserID + DateTime.Now.ToString("yyyyMMddHHmmssff", DateTimeFormatInfo.CurrentInfo);
					verifyInfo = new OrderIDsStorage.VerifyInfo();
					verifyInfo.verifyData = PlatSDKMgr._verifyData;
					verifyInfo.money = ((PlatSDKMgr._payMoney != 0) ? PlatSDKMgr._payMoney : 6);
					verifyInfo.verNum = verNum;
					verifyInfo.TransID = transID;
					verifyInfo.countryCode = countryCode;
					verifyInfo.currencyCode = currencyCode;
					verifyInfo.verifyState = -1;
					OrderIDsStorage.Add(orderId, verifyInfo);
					OrderIDsStorage.Save();
				}
				return;
			}
			OrderIDsStorage.VerifyInfo verifyInfo2 = OrderIDsStorage.GetOrderInfo(PlatSDKMgr._orderId);
			if (verifyInfo2 == null)
			{
				verifyInfo2 = new OrderIDsStorage.VerifyInfo();
				verifyInfo2.verifyData = PlatSDKMgr._verifyData;
				verifyInfo2.money = PlatSDKMgr._payMoney;
				verifyInfo2.verNum = verNum;
				verifyInfo2.TransID = transID;
				verifyInfo2.countryCode = countryCode;
				verifyInfo2.currencyCode = currencyCode;
				verifyInfo2.verifyState = -1;
				OrderIDsStorage.Add(PlatSDKMgr._orderId, verifyInfo2);
				OrderIDsStorage.Save();
			}
			else
			{
				if (verifyInfo2.verifyData.Length > 0 && Convert.ToBase64String(verifyInfo2.verifyData) != "NULLDATA")
				{
					OrderIDsStorage.GetOrderInfoWithEmptyData(out PlatSDKMgr._orderId, out verifyInfo2);
					if (verifyInfo2 == null)
					{
						PlatSDKMgr._orderId = "BUDAN-" + Global.Data.UserID + DateTime.Now.ToString("yyyyMMddHHmmssff", DateTimeFormatInfo.CurrentInfo);
						verifyInfo2 = new OrderIDsStorage.VerifyInfo();
						verifyInfo2.money = ((PlatSDKMgr._payMoney != 0) ? PlatSDKMgr._payMoney : 6);
					}
				}
				if (verifyData != null)
				{
					verifyInfo2.verifyData = PlatSDKMgr._verifyData;
					verifyInfo2.verNum = verNum;
					verifyInfo2.TransID = transID;
					verifyInfo2.countryCode = countryCode;
					verifyInfo2.currencyCode = currencyCode;
					verifyInfo2.verifyState = -1;
					OrderIDsStorage.Add(PlatSDKMgr._orderId, verifyInfo2);
					OrderIDsStorage.Save();
				}
			}
		}
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = PlatSDKMgr.GetAPPSPaymentVerifyServerURL();
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		int num = 57;
		int num2 = 49;
		string text = Convert.ToBase64String(verifyData);
		if (text.Length - num < 0)
		{
			Debug.Log("-----------b64VerifyData: " + text);
			Debug.Log("-----------b64VerifyData length: " + text.Length);
		}
		else
		{
			ClientExchangeOrderResultData clientExchangeOrderResultData = new ClientExchangeOrderResultData();
			clientExchangeOrderResultData.strPlatform = PlatSDKMgr.PlatName;
			clientExchangeOrderResultData.strUserID = userID;
			clientExchangeOrderResultData.lTime = timeStamp;
			clientExchangeOrderResultData.nMoney = PlatSDKMgr._payMoney;
			clientExchangeOrderResultData.strExchangeOrder = PlatSDKMgr._orderId;
			clientExchangeOrderResultData.nServerID = Global.Data.GameServerID;
			clientExchangeOrderResultData.recipent_data = verifyData;
			clientExchangeOrderResultData.strIOSVer = verNum;
			clientExchangeOrderResultData.strTransactionId = transID;
			clientExchangeOrderResultData.strNSLocaleCountryCode = countryCode;
			clientExchangeOrderResultData.strNSLocaleCurrencyCode = currencyCode;
			clientExchangeOrderResultData.VersionCode = "1.5.0";
			string text2 = string.Concat(new object[]
			{
				string.Empty,
				timeStamp,
				Global.Data.GameServerID,
				text.Substring(text.Length - num, num2)
			});
			clientExchangeOrderResultData.strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
			{
				text2,
				PlatSDKMgr.PlatName,
				userID,
				PlatSDKMgr._payMoney,
				PlatSDKMgr._orderId,
				Global.Data.GameServerID,
				timeStamp,
				countryCode,
				currencyCode,
				clientExchangeOrderResultData.VersionCode
			}));
			clientExchangeOrderResultData.FirstId = Global.Data.ServerData.LastServer.nFirstLevelServerID + string.Empty;
			voiceRequestParam.timeout = 10;
			voiceRequestParam.postData = DataHelper.ObjectToBytes<ClientExchangeOrderResultData>(clientExchangeOrderResultData);
			SimpleHttpTask.HttpPost(voiceRequestParam.url, null, voiceRequestParam.postData, delegate(WWW w)
			{
				if (w != null && string.IsNullOrEmpty(w.error))
				{
					byte[] bytes = w.bytes;
					if (bytes != null && bytes.Length > 0)
					{
						ServerExchangeOrderResultData serverExchangeOrderResultData = DataHelper.BytesToObject<ServerExchangeOrderResultData>(bytes, 0, bytes.Length);
						if (serverExchangeOrderResultData == null)
						{
						}
						if (serverExchangeOrderResultData != null && !string.IsNullOrEmpty(serverExchangeOrderResultData.strState))
						{
							string text3 = string.Empty;
							string orderId2 = string.Empty;
							if (serverExchangeOrderResultData.strState.Contains(":"))
							{
								string[] array = serverExchangeOrderResultData.strState.Split(new char[]
								{
									':'
								});
								if (array != null && array.Length >= 2)
								{
									text3 = array[0].Trim();
									orderId2 = array[1].Trim();
								}
								else
								{
									text3 = serverExchangeOrderResultData.strState;
									orderId2 = PlatSDKMgr._orderId;
								}
							}
							else
							{
								text3 = serverExchangeOrderResultData.strState;
								orderId2 = PlatSDKMgr._orderId;
							}
							if (text3 == "0")
							{
								OrderIDsStorage.Del(orderId2);
								OrderIDsStorage.Save();
							}
							else if (text3 == "-3" || text3 == "-2")
							{
								OrderIDsStorage.Del(orderId2);
								OrderIDsStorage.Save();
							}
							else if (text3 == "-4")
							{
								OrderIDsStorage.VerifyInfo orderInfo = OrderIDsStorage.GetOrderInfo(PlatSDKMgr._orderId);
								if (orderInfo != null)
								{
									orderInfo.verifyData = Convert.FromBase64String("NULLDATA");
									OrderIDsStorage.Add(PlatSDKMgr._orderId, orderInfo);
									OrderIDsStorage.Save();
								}
							}
							else if (text3 == "-1")
							{
								if (PlatSDKMgr._verifyData != null)
								{
									PlayZone.GlobalPlayZone.StartPaymentVerify();
								}
							}
							else if (text3 == "-6")
							{
								string[] buttons = new string[]
								{
									Global.GetLang("申诉"),
									Global.GetLang("取消")
								};
								string lang = Global.GetLang("尊敬的用户，由于您使用超过两个APP账号进行充值，当前账号已被锁；您可以通过申诉来解锁账号，我们将尽快为您解决!");
								Super.ShowMessageBoxGUI(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
								{
									if (e.ID == 0)
									{
										PlatSDKMgr.AppealToUnlock();
									}
								}, buttons);
							}
							PlatSDKMgr._verifyData = null;
						}
					}
				}
				else
				{
					if (PlatSDKMgr._verifyData != null)
					{
						PlayZone.GlobalPlayZone.StartPaymentVerify();
					}
					PlatSDKMgr._verifyData = null;
				}
			}, 10f);
		}
	}

	public static void AppealToUnlock()
	{
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		ClientAppealData clientAppealData = new ClientAppealData();
		clientAppealData.strUserID = userID;
		clientAppealData.lTime = timeStamp;
		clientAppealData.strMD5 = MD5Helper.get_md5_string("tmsk_mu_06" + userID + timeStamp);
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = PlatSDKMgr.GetAppealURL();
		voiceRequestParam.timeout = 10;
		voiceRequestParam.postData = DataHelper.ObjectToBytes<ClientAppealData>(clientAppealData);
		SimpleHttpTask.HttpPost(voiceRequestParam.url, null, voiceRequestParam.postData, delegate(WWW w)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					ServerAppealData serverAppealData = DataHelper.BytesToObject<ServerAppealData>(bytes, 0, bytes.Length);
					if (serverAppealData != null)
					{
						if (serverAppealData.strState == "0")
						{
							if (!string.IsNullOrEmpty(serverAppealData.strResult))
							{
								string[] buttons = new string[]
								{
									Global.GetLang("确定")
								};
								string strResult = serverAppealData.strResult;
								Super.ShowMessageBoxGUI(Global.GetLang("提示"), strResult, delegate(object s, DPSelectedItemEventArgs e)
								{
								}, buttons);
							}
							else
							{
								string[] buttons2 = new string[]
								{
									Global.GetLang("确定")
								};
								string lang = Global.GetLang("申诉成功，请耐心等待处理结果!");
								Super.ShowMessageBoxGUI(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
								{
								}, buttons2);
							}
						}
						else if (serverAppealData.strState == "-1" && !string.IsNullOrEmpty(serverAppealData.strResult))
						{
							string[] buttons3 = new string[]
							{
								Global.GetLang("确定")
							};
							string strResult2 = serverAppealData.strResult;
							Super.ShowMessageBoxGUI(Global.GetLang("提示"), strResult2, delegate(object s, DPSelectedItemEventArgs e)
							{
							}, buttons3);
						}
					}
				}
			}
			else
			{
				string[] buttons4 = new string[]
				{
					Global.GetLang("确定")
				};
				string lang2 = Global.GetLang("请求超时，请联系客服进行处理!");
				Super.ShowMessageBoxGUI(Global.GetLang("提示"), lang2, delegate(object s, DPSelectedItemEventArgs e)
				{
				}, buttons4);
			}
		}, 10f);
	}

	public static void WXInit()
	{
		PlatSDKMgr._WXInit = false;
		if (PlatSDKMgr.PlatName == "TM")
		{
			return;
		}
		string text = PlatSDKMgr.GetAndriodPackName();
		string text2 = string.Empty;
		string platName = PlatSDKMgr.PlatName;
		if (platName != null)
		{
			if (PlatSDKMgr.<>f__switch$map11 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(30);
				dictionary.Add("WDJ", 0);
				dictionary.Add("UC", 1);
				dictionary.Add("MI", 2);
				dictionary.Add("QH360", 3);
				dictionary.Add("BD", 4);
				dictionary.Add("DL", 5);
				dictionary.Add("OPPO", 6);
				dictionary.Add("PPS", 7);
				dictionary.Add("VIVO", 8);
				dictionary.Add("HW", 9);
				dictionary.Add("KP", 10);
				dictionary.Add("KuGou", 11);
				dictionary.Add("YK", 12);
				dictionary.Add("MZW", 13);
				dictionary.Add("PPTV", 14);
				dictionary.Add("BF", 15);
				dictionary.Add("JL", 16);
				dictionary.Add("LENOVO", 17);
				dictionary.Add("ANZHI", 18);
				dictionary.Add("ITAPK", 19);
				dictionary.Add("YIWAN", 20);
				dictionary.Add("WAN37", 21);
				dictionary.Add("MIAN", 22);
				dictionary.Add("YYH", 23);
				dictionary.Add("SOGOU", 24);
				dictionary.Add("YOUYOU", 25);
				dictionary.Add("QMQJ", 26);
				dictionary.Add("CH", 27);
				dictionary.Add("YYB", 28);
				dictionary.Add("QQ", 29);
				PlatSDKMgr.<>f__switch$map11 = dictionary;
			}
			int num;
			if (PlatSDKMgr.<>f__switch$map11.TryGetValue(platName, ref num))
			{
				switch (num)
				{
				case 0:
					text2 = "wxd601bcb8ceab3ea3";
					goto IL_3E5;
				case 1:
					text2 = "wxcfaeb6fb1b7cbd30";
					goto IL_3E5;
				case 2:
					text2 = "wxb0fab53759246234";
					goto IL_3E5;
				case 3:
					text2 = "wx75e03f55dab229f8";
					goto IL_3E5;
				case 4:
					text2 = "wxe12d67db6b8b690d";
					goto IL_3E5;
				case 5:
					text2 = "wx1b4db25c57459edd";
					goto IL_3E5;
				case 6:
					text2 = "wx2d9eea6fcc7cc99c";
					goto IL_3E5;
				case 7:
					text2 = "wx87f0fed77a50fe78";
					goto IL_3E5;
				case 8:
					text2 = "wxf5e373d43e49f123";
					goto IL_3E5;
				case 9:
					text2 = "wx30fa1aa2d5f9486a";
					goto IL_3E5;
				case 10:
					text2 = "wx4114e2d55a713f45";
					goto IL_3E5;
				case 11:
					text2 = "wx6e9cd0a08ad213a8";
					goto IL_3E5;
				case 12:
					text2 = "wxab2645224daad99b";
					goto IL_3E5;
				case 13:
					text2 = "wxcec8e07ce307c901";
					goto IL_3E5;
				case 14:
					text2 = "wxec78dfa4b4e3fee0";
					goto IL_3E5;
				case 15:
					text2 = "wx962cb80ec3a643f3";
					goto IL_3E5;
				case 16:
					text2 = "wx1c7cad389b197e34";
					goto IL_3E5;
				case 17:
					text2 = "wx4fbde7f2a8fe6636";
					goto IL_3E5;
				case 18:
					text2 = "wxb3f338e947359c96";
					goto IL_3E5;
				case 19:
					text2 = "wxe7fa049a4d6a6837";
					goto IL_3E5;
				case 20:
					text2 = "wxfb1d7a42aad03f61";
					text = "com.tianmashikong.qmqj.ewan";
					goto IL_3E5;
				case 21:
					text2 = "wx63163b8f0e104903";
					text = "com.tianmashikong.qmqj.sq37";
					goto IL_3E5;
				case 22:
					text2 = "wx87bf0fea9230cc75";
					goto IL_3E5;
				case 23:
					text2 = "wx7ed3ffe0fbce1646";
					goto IL_3E5;
				case 24:
					text2 = "wx7cb6dcec7dfc9ddf";
					goto IL_3E5;
				case 25:
					text2 = "wx4389fa30c05ec374";
					goto IL_3E5;
				case 26:
					text2 = "wxb437a1a4e1c7df8e";
					text = "com.tmsk.qmqj.android.xy";
					goto IL_3E5;
				case 27:
					text2 = "wxc2e957d2f69103c6";
					goto IL_3E5;
				case 28:
					text2 = "wxe929408e19fa9969";
					goto IL_3E5;
				case 29:
					text2 = "wx7ed3ffe0fbce1646";
					goto IL_3E5;
				}
			}
		}
		text2 = string.Empty;
		IL_3E5:
		if (text2 == string.Empty)
		{
			return;
		}
		PlatSDKMgr._WXInit = true;
		if (PlatSDKMgr.UnityPlayerActivity == null)
		{
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(text + ".wxapi.WXSDKManager"))
		{
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("Init", new object[]
				{
					PlatSDKMgr.UnityPlayerActivity,
					text2
				});
			}
		}
	}

	public static int TencentShare(string type, string text, string imagepath)
	{
		string andriodPackName = PlatSDKMgr.GetAndriodPackName();
		if (PlatSDKMgr.UnityPlayerActivity == null)
		{
			return 0;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.sdkmgr.TencentPlugin"))
		{
			if (androidJavaClass != null)
			{
				return androidJavaClass.CallStatic<int>("TencentShare", new object[]
				{
					PlatSDKMgr.UnityPlayerActivity,
					type,
					text,
					imagepath
				});
			}
		}
		return 0;
	}

	public static void WXShareNosdk(string text, string path)
	{
		AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.tianmashikong.sdk.ShareManager");
		androidJavaClass.CallStatic("ShareTimeLine", new object[]
		{
			text,
			path
		});
		GameInstance.Game.UpdateShareStat();
	}

	public static void WXShareImage(string path)
	{
		string andriodPackName = PlatSDKMgr.GetAndriodPackName();
		if (PlatSDKMgr.PlatName == "BD" || PlatSDKMgr.PlatName == "M37" || PlatSDKMgr.PlatName == "SAM" || PlatSDKMgr.PlatName == "YYB" || PlatSDKMgr.PlatName == "XYZS" || PlatSDKMgr.PlatName == "MIAN" || PlatSDKMgr.PlatName == "LESHI" || PlatSDKMgr.PlatName == "SINA" || PlatSDKMgr.PlatName == "KAOPU" || PlatSDKMgr.PlatName == "WAN37" || PlatSDKMgr.PlatName == "QQ" || PlatSDKMgr.PlatName == "KYXY" || PlatSDKMgr.PlatName == "PYW" || PlatSDKMgr.PlatName == "GUOPAN")
		{
			PlatSDKMgr.WXShareNosdk(string.Empty, path);
			return;
		}
		if (!PlatSDKMgr._WXInit)
		{
			return;
		}
		if (PlatSDKMgr.UnityPlayerActivity == null)
		{
			return;
		}
		if (!PlatSDKMgr.GetWXInstalled())
		{
			Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("分享失败，请确认是否安装微信..."), -1, -1, -1, -1, false);
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(andriodPackName + ".wxapi.WXSDKManager"))
		{
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("SendImage", new object[]
				{
					PlatSDKMgr.UnityPlayerActivity,
					path,
					100,
					100
				});
			}
		}
	}

	public static void WXShareUrl(string path, string text)
	{
		string andriodPackName = PlatSDKMgr.GetAndriodPackName();
		if (!PlatSDKMgr._WXInit)
		{
			return;
		}
		if (PlatSDKMgr.UnityPlayerActivity == null)
		{
			return;
		}
		if (!PlatSDKMgr.GetWXInstalled())
		{
			Super.ShowMessageBoxEx(Global.MainStage, 0, Global.GetLang("错误"), Global.GetLang("分享失败，请确认是否安装微信..."), -1, -1, -1, -1, false);
			return;
		}
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(andriodPackName + ".wxapi.WXSDKManager"))
		{
			if (androidJavaClass != null)
			{
				androidJavaClass.CallStatic("SendImage", new object[]
				{
					PlatSDKMgr.UnityPlayerActivity,
					text,
					path
				});
			}
		}
	}

	public static bool GetWXInstalled()
	{
		if (!PlatSDKMgr._WXInit)
		{
			return false;
		}
		string andriodPackName = PlatSDKMgr.GetAndriodPackName();
		bool result;
		using (AndroidJavaClass androidJavaClass = new AndroidJavaClass(andriodPackName + ".wxapi.WXSDKManager"))
		{
			result = androidJavaClass.GetStatic<AndroidJavaObject>("api").Call<bool>("isWXAppInstalled", new object[0]);
		}
		return result;
	}

	private static string UrlEncode(string str)
	{
		byte[] bytes = Encoding.Default.GetBytes(str);
		string text = string.Empty;
		for (int i = 0; i < bytes.Length; i++)
		{
			text += "%";
			text += bytes[i].ToString("X2");
		}
		return text;
	}

	public static void OnCreateRole(string rolename, string reqTime)
	{
		AndroidSDKPlugin.OnCreateRole(rolename, reqTime);
		if (PlatSDKMgr.PlatName != "APPS")
		{
			return;
		}
		string text = "41459fd594d9ff2fa10b2608a143df4";
		string knetAdURL = LoadURLConfigManager.GetInstance().KnetAdURL;
		if (string.IsNullOrEmpty(knetAdURL))
		{
			return;
		}
		string text2 = WebUtility.UrlEncode(rolename);
		string text3 = Global.Data.GameServerID.ToString();
		long timeStamp = Global.GetTimeStamp();
		string text4 = Global.Data.UserID.Replace(PlatSDKMgr.PlatName, string.Empty);
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("key", "create");
		dictionary.Add("app", "1");
		dictionary.Add("req_time", reqTime);
		dictionary.Add("role", text2);
		dictionary.Add("server", text3);
		dictionary.Add("time", timeStamp.ToString());
		dictionary.Add("uid", text4);
		List<string> list = new List<string>();
		list.Add("app=1");
		list.Add("key=create");
		list.Add("req_time=" + reqTime);
		list.Add("role=" + text2);
		list.Add("server=" + text3);
		list.Add("time=" + timeStamp);
		list.Add("uid=" + text4);
		list.Sort();
		string text5 = string.Empty;
		for (int i = 0; i < list.Count; i++)
		{
			if (i == 0)
			{
				text5 = list[0];
			}
			else
			{
				text5 = text5 + "&" + list[i];
			}
		}
		string text6 = MD5Helper.get_md5_string(text5 + text).ToLower();
		dictionary.Add("token", text6);
		SimpleHttpTask.HttpGet(knetAdURL, dictionary, null, null, 10f);
	}

	public static void RequestProductInfo(int money, string productId, int zhigouId = 0)
	{
		PlatSDKMgr._payMoney = money;
		PlatSDKMgr.productId = productId;
	}

	public static void ToItunes()
	{
	}

	public static void TencentNotice(int type)
	{
	}

	public static void TencentHideNoticeRoll()
	{
	}

	public static void PlatAbort()
	{
	}

	public static bool NeedShowShareButton()
	{
		string platName = PlatSDKMgr.PlatName;
		if (platName != null)
		{
			if (PlatSDKMgr.<>f__switch$map12 == null)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>(4);
				dictionary.Add("I4", 0);
				dictionary.Add("PP", 0);
				dictionary.Add("TIYAN", 0);
				dictionary.Add("YYH", 0);
				PlatSDKMgr.<>f__switch$map12 = dictionary;
			}
			int num;
			if (PlatSDKMgr.<>f__switch$map12.TryGetValue(platName, ref num))
			{
				if (num == 0)
				{
					return false;
				}
			}
		}
		return true;
	}

	public static void LevelCDKey(string type)
	{
		int num = Global.Data.roleData.ChangeLifeCount * 100 + Global.Data.roleData.Level;
		if (type == "level")
		{
			int @int = PlayerPrefs.GetInt("lastlevel", 1);
			if (@int == num)
			{
				return;
			}
		}
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		dictionary.Add("uid", Global.Data.UserID);
		dictionary.Add("rid", Global.Data.roleData.RoleID.ToString());
		dictionary.Add("level", num.ToString());
		dictionary.Add("zoneid", Global.Data.GameServerID.ToString());
		dictionary.Add("type", type);
		SimpleHttpTask.HttpPost(Global.ServerListURLSecond + "WriteUserLevelYyb.aspx", dictionary, null, null, 10f);
		PlayerPrefs.SetInt("lastlevel", num);
	}

	public static string Idfa()
	{
		string empty = string.Empty;
		return AndroidSDKPlugin.IMEI;
	}

	public static string GameName()
	{
		return "QMQJ";
	}

	public static string VersionCode()
	{
		return "1.0.0";
	}

	public static string Channel()
	{
		return string.Empty;
	}

	public static string Area()
	{
		return string.Empty;
	}

	public static bool IsGuestk()
	{
		return false;
	}

	public static string DeviceType()
	{
		return string.Empty;
	}

	public static string DeviceId()
	{
		return QMQJJava.GetDeviceID();
	}

	public static string Account()
	{
		return Global.Data.UserID;
	}

	public static string DeviceToken()
	{
		return QMQJJava.GetPushServerClientID();
	}

	public static string AppID()
	{
		string empty = string.Empty;
		return "android.qmqj.tmsk";
	}

	public static float OSVersion()
	{
		return 0f;
	}

	public static string IpAddr()
	{
		return string.Empty;
	}

	public static void RoportLoginEvent()
	{
	}

	public static void AdReport(string postdata)
	{
		string text = "HWjKO26fJvZ2f8v0QuEGZ3k3hFO4Ct8A";
		string text2 = postdata + "&sign=" + MD5Helper.get_md5_string(text + postdata).ToLower();
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = Global.AdServerUrl + "adClientReport.aspx";
		string theKey = string.Empty + DateTime.Now.Ticks;
		voiceRequestParam.callback = delegate(WWW w, object obj)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					Hashtable hashtable = (Hashtable)MUJson.jsonDecode(Encoding.UTF8.GetString(bytes));
					if (hashtable["ret"].ToString() == "0")
					{
						HttpCache.Del(theKey);
					}
				}
			}
		};
		voiceRequestParam.postData = Encoding.UTF8.GetBytes(text2);
		voiceRequestParam.timeout = 20;
		HttpCache.HttpCacheInfo httpCacheInfo = new HttpCache.HttpCacheInfo();
		httpCacheInfo.postdata = voiceRequestParam.postData;
		httpCacheInfo.url = voiceRequestParam.url;
		HttpCache.Add(theKey, httpCacheInfo);
		HttpService.Instance.Load(voiceRequestParam.url, voiceRequestParam.callback, voiceRequestParam.postData, voiceRequestParam.param, 0);
	}

	public static void ActiveReport()
	{
		if (!PlayerPrefs.HasKey("ActiveReport"))
		{
			string text = "HWjKO26fJvZ2f8v0QuEGZ3k3hFO4Ct8A";
			string text2 = string.Empty;
			if (!PlatSDKMgr.PlatName.Equals("TMSK"))
			{
			}
			string text3 = string.Concat(new object[]
			{
				"r=",
				Global.GetTimeStamp(),
				"&tmskchannel=",
				AndroidSDKPlugin.TmskChannel,
				"&chnspecid=",
				AndroidSDKPlugin.TmskChnspecid,
				"&chntrack=",
				AndroidSDKPlugin.TM_Chntrack(),
				"&version=",
				PlatSDKMgr.Version,
				"&moniqi=",
				(!QMQJJava.checkNet()) ? "0" : "1",
				"&imei=",
				AndroidSDKPlugin.IMEI,
				"&imsi=",
				AndroidSDKPlugin.IMSI,
				"&wifiname=",
				AndroidSDKPlugin.WifiName.Replace("\"", string.Empty),
				"&cpu=",
				AndroidSDKPlugin.CPU,
				"&gpu=",
				AndroidSDKPlugin.GPU,
				"&brand=",
				AndroidSDKPlugin.Brand,
				"&model=",
				AndroidSDKPlugin.MODEL,
				"&systeminfo=",
				AndroidSDKPlugin.SystemVersion
			});
			text3 = QMQJJava.encrypt(PlatSDKMgr.DeviceId(), text3);
			text2 = string.Concat(new string[]
			{
				"appid=",
				PlatSDKMgr.AppID(),
				"&devid=",
				PlatSDKMgr.DeviceId(),
				"&idfa=",
				PlatSDKMgr.Idfa(),
				"&__tmsk__=",
				text3
			});
			string text4 = text2 + "&sign=" + MD5Helper.get_md5_string(text + text2).ToLower();
			VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
			voiceRequestParam.url = Global.AdServerUrl + "startup.aspx";
			string theKey = string.Empty + DateTime.Now.Ticks;
			voiceRequestParam.callback = delegate(WWW w, object obj)
			{
				if (w != null && string.IsNullOrEmpty(w.error))
				{
					byte[] bytes = w.bytes;
					if (bytes != null && bytes.Length > 0)
					{
						Hashtable hashtable = (Hashtable)MUJson.jsonDecode(Encoding.UTF8.GetString(bytes));
						if (hashtable["ret"].ToString() == "0")
						{
							PlayerPrefs.SetString("ActiveReport", string.Empty);
							HttpCache.Del(theKey);
						}
					}
				}
			};
			voiceRequestParam.postData = Encoding.UTF8.GetBytes(text4);
			voiceRequestParam.timeout = 20;
			HttpCache.HttpCacheInfo httpCacheInfo = new HttpCache.HttpCacheInfo();
			httpCacheInfo.postdata = voiceRequestParam.postData;
			httpCacheInfo.url = voiceRequestParam.url;
			HttpCache.Add(theKey, httpCacheInfo);
			HttpService.Instance.Load(voiceRequestParam.url, voiceRequestParam.callback, voiceRequestParam.postData, voiceRequestParam.param, 0);
		}
	}

	public static void CreateRoleReport()
	{
		string text = "HWjKO26fJvZ2f8v0QuEGZ3k3hFO4Ct8A";
		string text2 = string.Empty;
		if (!PlatSDKMgr.PlatName.Equals("TMSK"))
		{
		}
		string text3 = string.Concat(new object[]
		{
			"r=",
			Global.GetTimeStamp(),
			"&version=",
			PlatSDKMgr.Version,
			"&account=",
			Global.RootParams["uid"],
			"&roleid=",
			Global.Data.RoleID,
			"&zone=",
			Global.Data.GameServerID,
			"&rname=",
			PlatSDKMgr._lastCreateRole,
			"&tmskchannel=",
			AndroidSDKPlugin.TmskChannel,
			"&chnspecid=",
			AndroidSDKPlugin.TmskChnspecid,
			"&moniqi=",
			(!QMQJJava.checkNet()) ? "0" : "1",
			"&imei=",
			AndroidSDKPlugin.IMEI,
			"&imsi=",
			AndroidSDKPlugin.IMSI,
			"&wifiname=",
			AndroidSDKPlugin.WifiName.Replace("\"", string.Empty),
			"&cpu=",
			AndroidSDKPlugin.CPU,
			"&gpu=",
			AndroidSDKPlugin.GPU,
			"&brand=",
			AndroidSDKPlugin.Brand,
			"&model=",
			AndroidSDKPlugin.MODEL,
			"&systeminfo=",
			AndroidSDKPlugin.SystemVersion
		});
		text3 = QMQJJava.encrypt(PlatSDKMgr.DeviceId(), text3);
		text2 = string.Concat(new string[]
		{
			"appid=",
			PlatSDKMgr.AppID(),
			"&devid=",
			PlatSDKMgr.DeviceId(),
			"&idfa=",
			PlatSDKMgr.Idfa(),
			"&__tmsk__=",
			text3
		});
		text2 = text2 + "&sign=" + MD5Helper.get_md5_string(text + text2).ToLower();
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = Global.AdServerUrl + "createRole.aspx";
		string theKey = string.Empty + DateTime.Now.Ticks;
		voiceRequestParam.callback = delegate(WWW w, object obj)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					Hashtable hashtable = (Hashtable)MUJson.jsonDecode(Encoding.UTF8.GetString(bytes));
					if (hashtable["ret"].ToString() == "0")
					{
						HttpCache.Del(theKey);
					}
				}
			}
		};
		voiceRequestParam.postData = Encoding.UTF8.GetBytes(text2);
		voiceRequestParam.timeout = 20;
		HttpCache.HttpCacheInfo httpCacheInfo = new HttpCache.HttpCacheInfo();
		httpCacheInfo.postdata = voiceRequestParam.postData;
		httpCacheInfo.url = voiceRequestParam.url;
		HttpCache.Add(theKey, httpCacheInfo);
		HttpService.Instance.Load(voiceRequestParam.url, voiceRequestParam.callback, voiceRequestParam.postData, voiceRequestParam.param, 0);
	}

	public static void RegistReport(string uid, bool isVisitor = false)
	{
		string text = "HWjKO26fJvZ2f8v0QuEGZ3k3hFO4Ct8A";
		string text2 = string.Empty;
		if (!PlatSDKMgr.PlatName.Equals("TMSK"))
		{
		}
		string text3 = string.Concat(new object[]
		{
			"r=",
			Global.GetTimeStamp(),
			"&account=",
			Global.RootParams["uid"],
			"&tmskchannel=",
			AndroidSDKPlugin.TmskChannel,
			"&chnspecid=",
			AndroidSDKPlugin.TmskChnspecid,
			"&moniqi=",
			(!QMQJJava.checkNet()) ? "0" : "1",
			"&imei=",
			AndroidSDKPlugin.IMEI,
			"&imsi=",
			AndroidSDKPlugin.IMSI,
			"&wifiname=",
			AndroidSDKPlugin.WifiName.Replace("\"", string.Empty),
			"&cpu=",
			AndroidSDKPlugin.CPU,
			"&gpu=",
			AndroidSDKPlugin.GPU,
			"&brand=",
			AndroidSDKPlugin.Brand,
			"&model=",
			AndroidSDKPlugin.MODEL,
			"&systeminfo=",
			AndroidSDKPlugin.SystemVersion
		});
		text3 = QMQJJava.encrypt(PlatSDKMgr.DeviceId(), text3);
		text2 = string.Concat(new string[]
		{
			"appid=",
			PlatSDKMgr.AppID(),
			"&devid=",
			PlatSDKMgr.DeviceId(),
			"&idfa=",
			PlatSDKMgr.Idfa(),
			"&__tmsk__=",
			text3
		});
		string text4 = text2 + "&sign=" + MD5Helper.get_md5_string(text + text2).ToLower();
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = Global.AdServerUrl + "register.aspx";
		string theKey = string.Empty + DateTime.Now.Ticks;
		voiceRequestParam.callback = delegate(WWW w, object obj)
		{
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					Hashtable hashtable = (Hashtable)MUJson.jsonDecode(Encoding.UTF8.GetString(bytes));
					if (hashtable["ret"].ToString() == "0")
					{
						HttpCache.Del(theKey);
					}
				}
			}
		};
		voiceRequestParam.postData = Encoding.UTF8.GetBytes(text4);
		voiceRequestParam.timeout = 20;
		HttpCache.HttpCacheInfo httpCacheInfo = new HttpCache.HttpCacheInfo();
		httpCacheInfo.postdata = voiceRequestParam.postData;
		httpCacheInfo.url = voiceRequestParam.url;
		HttpCache.Add(theKey, httpCacheInfo);
		HttpService.Instance.Load(voiceRequestParam.url, voiceRequestParam.callback, voiceRequestParam.postData, voiceRequestParam.param, 0);
	}

	public static void InitBuglySDK()
	{
		BuglyAgent.ConfigDebugMode(true);
		BuglyAgent.InitWithAppId("507067e752");
		BuglyAgent.EnableExceptionHandler();
		BuglyAgent.PrintLog(2, "Init the bugly sdk", new object[0]);
	}

	public static void GotyeVideoInit()
	{
		string name = string.Empty;
		switch (Global.DevicePlatform())
		{
		case AppPlatform.Default:
		case AppPlatform.Android:
			name = "AndroidViedo";
			break;
		case AppPlatform.IOS:
			name = "APPViedo";
			break;
		case AppPlatform.IOS_Jailbreak:
			name = "YueYuViedo";
			break;
		}
		string[] systemParamStringArrayByName = ConfigSystemParam.GetSystemParamStringArrayByName(name, '|');
		if (systemParamStringArrayByName.Length == 0)
		{
			return;
		}
		try
		{
			PlatSDKMgr.UnityPlayerActivity.Call("GotyeVideoInit", new object[]
			{
				systemParamStringArrayByName[3],
				systemParamStringArrayByName[2],
				systemParamStringArrayByName[1]
			});
		}
		catch (UnityException ex)
		{
		}
	}

	private const string BuglyAppIDForiOS = "7d86bf1efb";

	private const string BuglyAppIDForAndroid = "507067e752";

	private static bool _Test = false;

	public static string _userInfo = string.Empty;

	public static int _ExchageRate = 10;

	public static string _lastCreateRole = string.Empty;

	private static bool _WXInit = false;

	public static string _platToken = "-1";

	public static GameObject _LoginObj = null;

	public static string m_tencentPayRecode = string.Empty;

	public static bool _hasEntergame = false;

	public static string _chargeUid = string.Empty;

	public static bool _isEnterGame = false;

	public static int _bReConnect = 0;

	private static Dictionary<string, int> _PlatCodeDict;

	public static string _uid;

	public static string _sid;

	public static string _token;

	public static string _name;

	public static string _lastTime;

	public static string _isadult;

	public static int _payMoney;

	public static string _orderId;

	public static byte[] _verifyData;

	public static string productId;

	public static int zhigouId;

	public static ToGame m_toGame;

	private static DispatcherTimer timerSelectServer;

	private static AndroidJavaObject _UnityActivity;

	private static string m_version;

	private static string m_buildTime;

	public static int m_platform;

	public static string m_platName;

	public static string m_tmGameId;
}
