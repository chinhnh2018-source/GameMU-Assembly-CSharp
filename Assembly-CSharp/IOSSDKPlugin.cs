using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using Tmsk.Xml;
using UnityEngine;

public class IOSSDKPlugin
{
	public static int Platform
	{
		get
		{
			if (IOSSDKPlugin.m_platform == -1)
			{
				IOSSDKPlugin.ReadAndroidPlatcode();
			}
			return IOSSDKPlugin.m_platform;
		}
	}

	public static string PlatName
	{
		get
		{
			if (string.IsNullOrEmpty(IOSSDKPlugin.m_platName))
			{
			}
			return IOSSDKPlugin.m_platName;
		}
		set
		{
			IOSSDKPlugin.m_platName = value;
		}
	}

	public static string Version
	{
		get
		{
			if (string.IsNullOrEmpty(IOSSDKPlugin.m_version))
			{
			}
			return IOSSDKPlugin.m_version;
		}
	}

	public static string BuildTime
	{
		get
		{
			if (string.IsNullOrEmpty(IOSSDKPlugin.m_buildTime))
			{
				IOSSDKPlugin.ReadAndroidPlatcode();
			}
			return IOSSDKPlugin.m_buildTime;
		}
	}

	public static void ReadAndroidPlatcode()
	{
		TextAsset textAsset = Resources.Load("platconfig") as TextAsset;
		if (null == textAsset)
		{
			IOSSDKPlugin.m_platName = "TM";
			IOSSDKPlugin.m_platform = 0;
			return;
		}
		XElement xml = XElement.Parse(textAsset.text);
		int.TryParse(Global.ReadXmlConfigStr(xml, "root", "platID"), ref IOSSDKPlugin.m_platform);
		IOSSDKPlugin.m_platName = Global.ReadXmlConfigStr(xml, "root", "platName");
		IOSSDKPlugin.m_version = Global.ReadXmlConfigStr(xml, "root", "version");
		if (string.IsNullOrEmpty(IOSSDKPlugin.m_version))
		{
			IOSSDKPlugin.m_version = "1.0.1";
		}
		IOSSDKPlugin.m_buildTime = Global.ReadXmlConfigStr(xml, "root", "buildTime");
		if (string.IsNullOrEmpty(IOSSDKPlugin.m_buildTime))
		{
			IOSSDKPlugin.m_buildTime = "2014-10-10";
		}
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"platID = ",
				IOSSDKPlugin.m_platform,
				" name = ",
				IOSSDKPlugin.m_platName,
				" version = ",
				IOSSDKPlugin.m_version,
				" buildTime = ",
				IOSSDKPlugin.m_buildTime
			})
		});
	}

	public static string GetOrderIdHttpUrl()
	{
		string text = IOSSDKPlugin.PlatName + "Login";
		string text2 = Global.PayServerURL + IOSSDKPlugin.PlatName + "Login/GetExchangeOrder.aspx";
		MUDebug.Log<string>(new string[]
		{
			"YNIOS_GetOrderIdHttpUrl()_strURL=" + text2
		});
		return text2;
	}

	public static string GetPaymentVerifyServerURL()
	{
		string text = IOSSDKPlugin.PlatName + "Login";
		string text2 = Global.PayServerURL + IOSSDKPlugin.PlatName + "Login/ExchangeResult.aspx";
		MUDebug.Log<string>(new string[]
		{
			"YNIOS_GetPaymentVerifyServerURL()_strURL=" + text2
		});
		return text2;
	}

	public static void SDKInit()
	{
	}

	public static bool Login(GameObject obj, string args = "")
	{
		MUDebug.Log<string>(new string[]
		{
			"start sdk login platname = " + IOSSDKPlugin.PlatName
		});
		PlatSDKEventListening.ChangeAccountButtonWaiting();
		return true;
	}

	public static void LoginOut(string args)
	{
		Global.RootParams["uid"] = "-1";
	}

	public static void ReLogin()
	{
		Global.RootParams["uid"] = "-1";
	}

	public static void Pay(int money, string productId, int zhigouId = 0)
	{
		IOSSDKPlugin._userInfo = string.Empty;
		IOSSDKPlugin._payMoney = money;
		IOSSDKPlugin.productId = productId;
		IOSSDKPlugin.zhigouId = zhigouId;
		Super.ShowNetWaiting(null);
		ClientGetOrderData clientGetOrderData = new ClientGetOrderData();
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		clientGetOrderData.strPlatform = IOSSDKPlugin.PlatName;
		clientGetOrderData.strUserID = userID;
		if (IOSSDKPlugin.PlatName == "HM")
		{
			clientGetOrderData.strUserID = userID + "|1";
		}
		clientGetOrderData.lTime = timeStamp;
		clientGetOrderData.strMoney = string.Empty + IOSSDKPlugin._payMoney;
		clientGetOrderData.roleName = Global.Data.RoleName;
		clientGetOrderData.roleId = Global.Data.RoleID;
		clientGetOrderData.nServerID = Global.Data.GameServerID;
		clientGetOrderData.firstId = Global.Data.ServerData.LastServer.nFirstLevelServerID + string.Empty;
		clientGetOrderData.strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
		{
			"tmsk_mu_06",
			clientGetOrderData.strPlatform,
			clientGetOrderData.strUserID,
			IOSSDKPlugin._payMoney,
			Global.Data.GameServerID,
			timeStamp
		}));
		clientGetOrderData.ZhiGouID = string.Empty + zhigouId;
		Debug.Log("YNIOS_zhigou id = " + zhigouId);
		clientGetOrderData.ProductId = productId;
		Debug.Log("YNIOS_product id = " + productId);
		SimpleHttpTask.HttpPost(IOSSDKPlugin.GetOrderIdHttpUrl(), null, DataHelper.ObjectToBytes<ClientGetOrderData>(clientGetOrderData), new SimpleHttpTask.HttpCallback(IOSSDKPlugin.GetOrderIdCallback), 10f);
	}

	public static void GetOrderIdCallback(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			MUDebug.Log<string>(new string[]
			{
				"YNIOS************ w is not null and not empty......"
			});
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					"YNIOS************ returnBytes.Length > 0......" + bytes.Length
				});
				ServerGetOrderData serverGetOrderData = DataHelper.BytesToObject<ServerGetOrderData>(bytes, 0, bytes.Length);
				if (serverGetOrderData != null)
				{
					IOSSDKPlugin._orderId = serverGetOrderData.strExchangeOrder;
					if (!string.IsNullOrEmpty(serverGetOrderData.strExtParam))
					{
						IOSSDKPlugin._userInfo = serverGetOrderData.strExtParam;
					}
					MUDebug.Log<string>(new string[]
					{
						"YNIOS_userinfo=" + IOSSDKPlugin._userInfo
					});
					MUDebug.Log<string>(new string[]
					{
						"YNIOS_orderId=" + IOSSDKPlugin._orderId
					});
					IOSSDKPlugin.DoPay(serverGetOrderData.strExchangeOrder);
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"YNIOS_returnBytes is null" + w.error
				});
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"YNIOS_************ w is empty"
			});
		}
	}

	public static void DoPay(string orderId)
	{
		if (IOSSDKPlugin._payMoney == 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"DoPay return as [IOSSDKPlugin._payMoney == 0]"
			});
			return;
		}
		int payMoney = IOSSDKPlugin._payMoney;
		string userInfo = IOSSDKPlugin._userInfo;
		string text = Global.GetLang("全民奇迹");
		if (Global.Data != null && Global.Data.ServerData != null && Global.Data.ServerData.LastServer != null)
		{
			text = Global.Data.ServerData.LastServer.strServerName;
		}
	}

	public static void ShowUserCenter()
	{
	}

	public static void PlatAbort()
	{
	}

	public static void VideoLogin(string jsonstr, string roomid, string psw)
	{
	}

	public static void VideoClose()
	{
	}

	public static UserInfoSdkData UserInfoParse()
	{
		return new UserInfoSdkData();
	}

	public static string _userInfo = string.Empty;

	public static string _orderId = string.Empty;

	public static int _payMoney;

	public static string productId = string.Empty;

	public static string _platToken = string.Empty;

	public static int _ExchageRate = 10;

	public static int zhigouId;

	public static int m_platform = -1;

	public static string m_platName = string.Empty;

	private static string m_version = string.Empty;

	private static string m_buildTime = string.Empty;
}
