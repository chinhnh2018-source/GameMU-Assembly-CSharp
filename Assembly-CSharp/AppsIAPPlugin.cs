using System;
using System.Collections;
using System.Globalization;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;
using UnityEngine;

public class AppsIAPPlugin
{
	public static string GetAPPSOrderIdHttpUrl()
	{
		return Global.PayServerURL + IOSSDKPlugin.PlatName + "pay/GetExchangeOrder.aspx";
	}

	public static string GetAPPSPaymentVerifyServerURL()
	{
		return Global.PayServerURL + IOSSDKPlugin.PlatName + "pay/ExchangeResult.aspx";
	}

	public static string GetAppealURL()
	{
		return Global.VerifyAccountServerURL + IOSSDKPlugin.PlatName + "Login/Appeal.aspx";
	}

	private static void PostLogInfoCallback(WWW w)
	{
		if (w == null || !string.IsNullOrEmpty(w.error))
		{
			if (w != null && !string.IsNullOrEmpty(w.error))
			{
				MUDebug.Log<string>(new string[]
				{
					"Post log fail with error msg:" + w.error
				});
			}
		}
	}

	public static void PostLogInfo(string info)
	{
	}

	public static void PostPayResultInfo(string resultCode)
	{
		MUDebug.Log<string>(new string[]
		{
			"Post Pay Result Code = " + resultCode
		});
	}

	public static void RequestProductInfo(int money, string productId)
	{
		Debug.Log(string.Concat(new object[]
		{
			"AppsIAPPlugin money =",
			money,
			" and product Id = ",
			productId
		}));
	}

	public static void RequestOrderInfo(string msg)
	{
		MUDebug.Log<string>(new string[]
		{
			"[AppsIAPPlugin] In RequestOrderInfo......"
		});
		PlayerPrefs.SetString("AppsOrderTime", (DateTime.Now.Ticks / 10000000L).ToString());
		MUDebug.Log<string>(new string[]
		{
			"IOSPlugin.APPS_KEY_ORDER_TIME == " + PlayerPrefs.GetString("AppsOrderTime")
		});
		Hashtable hashtable = (Hashtable)MUJson.jsonDecode(msg);
		IOSClientGetOrderData iosclientGetOrderData = new IOSClientGetOrderData();
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		iosclientGetOrderData.strPlatform = IOSSDKPlugin.PlatName;
		iosclientGetOrderData.strUserID = userID;
		iosclientGetOrderData.lTime = timeStamp;
		iosclientGetOrderData.strMoney = string.Empty + AppsIAPPlugin._payMoney;
		iosclientGetOrderData.nServerID = Global.Data.GameServerID;
		iosclientGetOrderData.NSLocaleCountryCode = hashtable["NSLocaleCountryCode"].ToString();
		iosclientGetOrderData.NSLocaleIdentifier = hashtable["NSLocaleIdentifier"].ToString();
		iosclientGetOrderData.NSLocaleLanguageCode = hashtable["NSLocaleLanguageCode"].ToString();
		iosclientGetOrderData.NSLocaleCurrencySymbol = hashtable["NSLocaleCurrencySymbol"].ToString();
		iosclientGetOrderData.NSLocaleCurrencyCode = hashtable["NSLocaleCurrencyCode"].ToString();
		iosclientGetOrderData.NSLocaleCollatorIdentifier = hashtable["NSLocaleCollatorIdentifier"].ToString();
		iosclientGetOrderData.Md5IdxValue = hashtable["IDXValue"].ToString();
		iosclientGetOrderData.Md5IdyValue = hashtable["IDYValue"].ToString();
		iosclientGetOrderData.IdxEqual = hashtable["IDXEqual"].ToString();
		iosclientGetOrderData.NotUseAgent = hashtable["NotUseAgent"].ToString();
		iosclientGetOrderData.VersionCode = "1.5.0";
		iosclientGetOrderData.roleName = Global.Data.RoleName;
		iosclientGetOrderData.roleId = Global.Data.RoleID;
		iosclientGetOrderData.ZhiGouID = string.Empty + AppsIAPPlugin.zhigouId;
		Debug.Log("apps iap zhigou id = " + AppsIAPPlugin.zhigouId);
		iosclientGetOrderData.Idfa = PlatSDKMgr.Idfa();
		string text = iosclientGetOrderData.Md5IdxValue.Substring(5, 13);
		iosclientGetOrderData.strMD5 = MD5Helper.get_md5_string(string.Concat(new object[]
		{
			text,
			IOSSDKPlugin.PlatName,
			userID,
			AppsIAPPlugin._payMoney,
			Global.Data.GameServerID,
			timeStamp,
			iosclientGetOrderData.NSLocaleCountryCode,
			iosclientGetOrderData.NSLocaleIdentifier,
			iosclientGetOrderData.NSLocaleLanguageCode,
			iosclientGetOrderData.NSLocaleCurrencyCode,
			iosclientGetOrderData.NSLocaleCollatorIdentifier,
			iosclientGetOrderData.Md5IdxValue,
			iosclientGetOrderData.Md5IdyValue,
			iosclientGetOrderData.IdxEqual,
			iosclientGetOrderData.NotUseAgent,
			iosclientGetOrderData.VersionCode
		}));
		iosclientGetOrderData.FirstId = Global.Data.ServerData.LastServer.nFirstLevelServerID;
		SimpleHttpTask.HttpPost(AppsIAPPlugin.GetAPPSOrderIdHttpUrl(), null, DataHelper.ObjectToBytes<IOSClientGetOrderData>(iosclientGetOrderData), new SimpleHttpTask.HttpCallback(AppsIAPPlugin.IOSGetOrderIdCallback), 10f);
	}

	public static void IOSGetOrderIdCallback(WWW w)
	{
		Super.HideNetWaiting();
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				ServerGetOrderData serverGetOrderData = DataHelper.BytesToObject<ServerGetOrderData>(bytes, 0, bytes.Length);
				if (serverGetOrderData != null && serverGetOrderData.strMD5 == MD5Helper.get_md5_string(string.Concat(new object[]
				{
					serverGetOrderData.strExchangeOrder,
					serverGetOrderData.strExtParam,
					serverGetOrderData.lTime,
					"tmsk_mu_06"
				})))
				{
					if (!(serverGetOrderData.strExchangeOrder == "-1"))
					{
						AppsIAPPlugin._orderId = serverGetOrderData.strExchangeOrder;
						if (!string.IsNullOrEmpty(serverGetOrderData.strExtParam))
						{
							AppsIAPPlugin._userInfo = serverGetOrderData.strExtParam;
						}
						MUDebug.Log<string>(new string[]
						{
							"userinfo=" + AppsIAPPlugin._userInfo
						});
						MUDebug.Log<string>(new string[]
						{
							"_orderId=" + AppsIAPPlugin._orderId
						});
						if (string.IsNullOrEmpty(AppsIAPPlugin._orderId))
						{
							AppsIAPPlugin._orderId = "BUDAN-" + Global.Data.UserID + DateTime.Now.ToString("yyyyMMddHHmmssff", DateTimeFormatInfo.CurrentInfo);
							MUDebug.Log<string>(new string[]
							{
								"Generate new orderid = " + AppsIAPPlugin._orderId
							});
						}
						OrderIDsStorage.VerifyInfo verifyInfo = new OrderIDsStorage.VerifyInfo();
						verifyInfo.verifyData = Convert.FromBase64String("NULLDATA");
						verifyInfo.money = AppsIAPPlugin._payMoney;
						verifyInfo.verNum = "NULLDATA";
						verifyInfo.TransID = "NULLDATA";
						verifyInfo.countryCode = "NULLDATA";
						verifyInfo.currencyCode = "NULLDATA";
						verifyInfo.verifyState = -1;
						OrderIDsStorage.Add(AppsIAPPlugin._orderId, verifyInfo);
						OrderIDsStorage.Save();
						AppsIAPPlugin.DoPay(serverGetOrderData.strExchangeOrder);
						PlayerPrefs.SetString("AppsOrderId", AppsIAPPlugin._orderId);
					}
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"returnBytes is null" + w.error
				});
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"************ w is empty"
			});
		}
	}

	public static void DoPay(string orderId)
	{
		if (AppsIAPPlugin._payMoney == 0)
		{
			MUDebug.Log<string>(new string[]
			{
				"DoPay return as [AppsIAPPlugin._payMoney == 0]"
			});
			return;
		}
		int payMoney = AppsIAPPlugin._payMoney;
		string userInfo = AppsIAPPlugin._userInfo;
	}

	public static void PaymentVerify(byte[] verifyData, string verNum, string transID, string countryCode, string currencyCode)
	{
		MUDebug.Log<string>(new string[]
		{
			"[AppsIAPPlugin] PaymentVerify..."
		});
		AppsIAPPlugin._verifyData = verifyData;
		MUDebug.Log<string>(new string[]
		{
			"Current Order id = [" + AppsIAPPlugin._orderId + "]"
		});
		if (AppsIAPPlugin._verifyData != null)
		{
			if (!(AppsIAPPlugin._orderId != string.Empty))
			{
				string orderId = string.Empty;
				OrderIDsStorage.VerifyInfo verifyInfo = null;
				OrderIDsStorage.GetOrderInfoWithEmptyData(out orderId, out verifyInfo);
				if (verifyInfo != null)
				{
					verifyInfo.verifyData = AppsIAPPlugin._verifyData;
					verifyInfo.money = ((AppsIAPPlugin._payMoney != 0) ? AppsIAPPlugin._payMoney : 6);
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
					verifyInfo.verifyData = AppsIAPPlugin._verifyData;
					verifyInfo.money = ((AppsIAPPlugin._payMoney != 0) ? AppsIAPPlugin._payMoney : 6);
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
			OrderIDsStorage.VerifyInfo verifyInfo2 = OrderIDsStorage.GetOrderInfo(AppsIAPPlugin._orderId);
			if (verifyInfo2 == null)
			{
				verifyInfo2 = new OrderIDsStorage.VerifyInfo();
				verifyInfo2.verifyData = AppsIAPPlugin._verifyData;
				verifyInfo2.money = AppsIAPPlugin._payMoney;
				verifyInfo2.verNum = verNum;
				verifyInfo2.TransID = transID;
				verifyInfo2.countryCode = countryCode;
				verifyInfo2.currencyCode = currencyCode;
				verifyInfo2.verifyState = -1;
				OrderIDsStorage.Add(AppsIAPPlugin._orderId, verifyInfo2);
				OrderIDsStorage.Save();
			}
			else
			{
				if (verifyInfo2.verifyData.Length > 0 && Convert.ToBase64String(verifyInfo2.verifyData) != "NULLDATA")
				{
					OrderIDsStorage.GetOrderInfoWithEmptyData(out AppsIAPPlugin._orderId, out verifyInfo2);
					if (verifyInfo2 == null)
					{
						AppsIAPPlugin._orderId = "BUDAN-" + Global.Data.UserID + DateTime.Now.ToString("yyyyMMddHHmmssff", DateTimeFormatInfo.CurrentInfo);
						verifyInfo2 = new OrderIDsStorage.VerifyInfo();
						verifyInfo2.money = ((AppsIAPPlugin._payMoney != 0) ? AppsIAPPlugin._payMoney : 6);
					}
				}
				if (verifyData != null)
				{
					verifyInfo2.verifyData = AppsIAPPlugin._verifyData;
					verifyInfo2.verNum = verNum;
					verifyInfo2.TransID = transID;
					verifyInfo2.countryCode = countryCode;
					verifyInfo2.currencyCode = currencyCode;
					verifyInfo2.verifyState = -1;
					OrderIDsStorage.Add(AppsIAPPlugin._orderId, verifyInfo2);
					OrderIDsStorage.Save();
				}
			}
		}
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = AppsIAPPlugin.GetAPPSPaymentVerifyServerURL();
		MUDebug.Log<string>(new string[]
		{
			voiceRequestParam.url
		});
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		int num = 57;
		int num2 = 49;
		string text = Convert.ToBase64String(verifyData);
		if (text.Length - num < 0)
		{
			Debug.Log("-----------b64VerifyData: " + text);
			Debug.Log("-----------b64VerifyData length: " + text.Length);
			AppsIAPPlugin.PostLogInfo("Illegal Verify Data:" + text);
		}
		else
		{
			ClientExchangeOrderResultData clientExchangeOrderResultData = new ClientExchangeOrderResultData();
			clientExchangeOrderResultData.strPlatform = IOSSDKPlugin.PlatName;
			clientExchangeOrderResultData.strUserID = userID;
			clientExchangeOrderResultData.lTime = timeStamp;
			clientExchangeOrderResultData.nMoney = AppsIAPPlugin._payMoney;
			clientExchangeOrderResultData.strExchangeOrder = AppsIAPPlugin._orderId;
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
				IOSSDKPlugin.PlatName,
				userID,
				AppsIAPPlugin._payMoney,
				AppsIAPPlugin._orderId,
				Global.Data.GameServerID,
				timeStamp,
				countryCode,
				currencyCode,
				clientExchangeOrderResultData.VersionCode
			}));
			clientExchangeOrderResultData.FirstId = Global.Data.ServerData.LastServer.nFirstLevelServerID + string.Empty;
			MUDebug.Log<string>(new string[]
			{
				"***** postdata.strMD5 = " + clientExchangeOrderResultData.strMD5
			});
			voiceRequestParam.timeout = 10;
			voiceRequestParam.postData = DataHelper.ObjectToBytes<ClientExchangeOrderResultData>(clientExchangeOrderResultData);
			SimpleHttpTask.HttpPost(voiceRequestParam.url, null, voiceRequestParam.postData, new SimpleHttpTask.HttpCallback(AppsIAPPlugin.PayResultCallback), 10f);
		}
	}

	public static void PayResultCallback(WWW w)
	{
		MUDebug.Log<string>(new string[]
		{
			"[AppsIAPPlugin] PayResultCallback..."
		});
		MUDebug.Log<string>(new string[]
		{
			"**************** Enter payment callback... ****************"
		});
		if (w != null && string.IsNullOrEmpty(w.error))
		{
			MUDebug.Log<string>(new string[]
			{
				"Get W and W is ok!"
			});
			byte[] bytes = w.bytes;
			if (bytes != null && bytes.Length > 0)
			{
				MUDebug.Log<string>(new string[]
				{
					"Got return bytes and length > 0"
				});
				ServerExchangeOrderResultData serverExchangeOrderResultData = DataHelper.BytesToObject<ServerExchangeOrderResultData>(bytes, 0, bytes.Length);
				if (serverExchangeOrderResultData == null)
				{
					MUDebug.Log<string>(new string[]
					{
						"responseData is null"
					});
				}
				else
				{
					MUDebug.Log<string>(new string[]
					{
						"responseData.strState = " + serverExchangeOrderResultData.strState
					});
				}
				if (serverExchangeOrderResultData != null && !string.IsNullOrEmpty(serverExchangeOrderResultData.strState))
				{
					string text = string.Empty;
					string orderId = string.Empty;
					if (serverExchangeOrderResultData.strState.Contains(":"))
					{
						string[] array = serverExchangeOrderResultData.strState.Split(new char[]
						{
							':'
						});
						if (array != null && array.Length >= 2)
						{
							text = array[0].Trim();
							orderId = array[1].Trim();
						}
						else
						{
							text = serverExchangeOrderResultData.strState;
							orderId = AppsIAPPlugin._orderId;
						}
					}
					else
					{
						text = serverExchangeOrderResultData.strState;
						orderId = AppsIAPPlugin._orderId;
					}
					if (text == "0")
					{
						MUDebug.Log<string>(new string[]
						{
							"Payment success!"
						});
						OrderIDsStorage.Del(orderId);
						OrderIDsStorage.Save();
					}
					else if (text == "-3" || text == "-2")
					{
						MUDebug.Log<string>(new string[]
						{
							"Illegal payment!"
						});
						OrderIDsStorage.Del(orderId);
						OrderIDsStorage.Save();
					}
					else if (text == "-4")
					{
						MUDebug.Log<string>(new string[]
						{
							"Illegal Verify-Data!"
						});
						OrderIDsStorage.VerifyInfo orderInfo = OrderIDsStorage.GetOrderInfo(AppsIAPPlugin._orderId);
						if (orderInfo != null)
						{
							orderInfo.verifyData = Convert.FromBase64String("NULLDATA");
							OrderIDsStorage.Add(AppsIAPPlugin._orderId, orderInfo);
							OrderIDsStorage.Save();
						}
					}
					else if (text == "-1")
					{
						if (AppsIAPPlugin._verifyData != null)
						{
							PlayZone.GlobalPlayZone.StartPaymentVerify();
						}
					}
					else if (text == "-6")
					{
						string[] buttons = new string[]
						{
							Global.GetLang("申诉"),
							Global.GetLang("取消")
						};
						string lang = Global.GetLang("尊敬的用户，由于您使用超过两个APP账号进行充值，当前账号已被锁；您可以通过申诉来解锁账号，我们将尽快为您解决!");
						Super.ShowMessageBoxGUI(Global.GetLang("提示"), lang, delegate(object s, DPSelectedItemEventArgs e)
						{
							MUDebug.Log<string>(new string[]
							{
								"************ the btn id=" + e.ID
							});
							if (e.ID == 0)
							{
								AppsIAPPlugin.AppealToUnlock();
							}
						}, buttons);
					}
					else if (text == "-7")
					{
						OrderIDsStorage.Del(orderId);
						OrderIDsStorage.Save();
						MUDebug.Log<string>(new string[]
						{
							"Need to refresh!"
						});
					}
					AppsIAPPlugin._verifyData = null;
				}
			}
			else
			{
				MUDebug.Log<string>(new string[]
				{
					"returnBytes is null" + w.error
				});
				AppsIAPPlugin.PostLogInfo("PaymentVerify callback return error [" + w.error + "]");
			}
		}
		else
		{
			MUDebug.Log<string>(new string[]
			{
				"Request Time out!"
			});
			AppsIAPPlugin.PostLogInfo("PaymentVerify callback return error [Request Time out!]");
			if (AppsIAPPlugin._verifyData != null)
			{
				PlayZone.GlobalPlayZone.StartPaymentVerify();
			}
			AppsIAPPlugin._verifyData = null;
		}
	}

	public static void AppealToUnlock()
	{
		MUDebug.Log<string>(new string[]
		{
			"Global.AppealURL = " + AppsIAPPlugin.GetAppealURL()
		});
		long timeStamp = Global.GetTimeStamp();
		string userID = Global.Data.UserID;
		ClientAppealData clientAppealData = new ClientAppealData();
		clientAppealData.strUserID = userID;
		clientAppealData.lTime = timeStamp;
		clientAppealData.strMD5 = MD5Helper.get_md5_string("tmsk_mu_06" + userID + timeStamp);
		VoiceRequestParam voiceRequestParam = new VoiceRequestParam();
		voiceRequestParam.url = AppsIAPPlugin.GetAppealURL();
		voiceRequestParam.timeout = 10;
		voiceRequestParam.postData = DataHelper.ObjectToBytes<ClientAppealData>(clientAppealData);
		SimpleHttpTask.HttpPost(voiceRequestParam.url, null, voiceRequestParam.postData, delegate(WWW w)
		{
			MUDebug.Log<string>(new string[]
			{
				"**************** Enter AppealToUnlock callback... ****************"
			});
			if (w != null && string.IsNullOrEmpty(w.error))
			{
				MUDebug.Log<string>(new string[]
				{
					"Get W and W is ok!"
				});
				byte[] bytes = w.bytes;
				if (bytes != null && bytes.Length > 0)
				{
					MUDebug.Log<string>(new string[]
					{
						"Got return bytes and length > 0"
					});
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

	public static int _payMoney;

	public static string _orderId = string.Empty;

	public static string _userInfo = string.Empty;

	public static byte[] _verifyData;

	public static string productId = string.Empty;

	public static int zhigouId;

	public static string loginfoUrl = string.Empty;
}
