using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class HuiGuiNetManager
{
	public static void ServeGetHuiGuiInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 6)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (int.Parse(e.fields[0]) != 1)
		{
			HuiGuiNetManager.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		string text = e.fields[1].Replace("$", ":");
		DateTime dateTime = DateTime.Parse(text);
		HuiGuiData.selfCreateTime = string.Format("{0}{1}{2}{3}{4}{5}", new object[]
		{
			dateTime.Year,
			Global.GetLang("年"),
			dateTime.Month,
			Global.GetLang("月"),
			dateTime.Day,
			Global.GetLang("日")
		});
		HuiGuiData.huiGuiId = e.fields[3].SafeToInt32(0);
		HuiGuiData.currentDay = e.fields[4].SafeToInt32(0);
		HuiGuiData.selfChongZhiNum = e.fields[5].SafeToInt32(0);
		MUEventManager.SendEvent("CMD_SPR_REGRESSACTIVE_GETFILE");
	}

	public static void ServeGetHuiGuiQianDaoInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		Dictionary<int, int> dictionary = DataHelper.BytesToObject<Dictionary<int, int>>(e.bytesData, 0, e.bytesData.Length);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, int>();
			return;
		}
		HuiGuiData.cacheLoginInfo = dictionary;
		MUEventManager.SendEvent<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSIGNINFO", dictionary);
	}

	public static void ServeHuiGuiQianDao(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 1)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (int.Parse(e.fields[0]) != 1)
		{
			HuiGuiNetManager.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		MUEventManager.SendEvent("CMD_SPR_REGRESSACTIVE_SING");
	}

	public static void ServeGetHuiGuiStoreInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		Dictionary<int, int> dictionary = DataHelper.BytesToObject<Dictionary<int, int>>(e.bytesData, 0, e.bytesData.Length);
		if (dictionary == null)
		{
			dictionary = new Dictionary<int, int>();
			return;
		}
		MUEventManager.SendEvent<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_GETSTOREINFO", dictionary);
	}

	public static void ServeHuiGuiBuy(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 1)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (int.Parse(e.fields[0]) != 1)
		{
			HuiGuiNetManager.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		MUEventManager.SendEvent("CMD_SPR_REGRESSACTIVE_STOREBUY");
	}

	public static void ServeGetHuiGuiChongZhiInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 3)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (int.Parse(e.fields[0]) != 1)
		{
			HuiGuiNetManager.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		int @params = e.fields[1].SafeToInt32(0);
		string text = e.fields[2];
		string[] array = text.Split(new char[]
		{
			'_'
		});
		List<int> list = new List<int>();
		for (int i = 0; i < array.Length; i++)
		{
			if (!(array[i] == string.Empty))
			{
				int num = array[i].SafeToInt32(0);
				if (num > 0)
				{
					list.Add(num);
				}
			}
		}
		MUEventManager.SendEvent<int, List<int>>("CMD_SPR_REGRESSACTIVE_INPUTINFO", @params, list);
	}

	public static void ServeHuiGuiLingQuZhiChong(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (e.fields.Length < 1)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (int.Parse(e.fields[0]) != 1)
		{
			HuiGuiNetManager.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		MUEventManager.SendEvent("CMD_SPR_REGRESSACTIVE_INPUT");
	}

	public static void ServeHuiGuiZhiGouInfo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		Dictionary<int, int> dictionary = DataHelper.BytesToObject<Dictionary<int, int>>(e.bytesData, 0, e.bytesData.Length);
		if (dictionary == null)
		{
			MUDebug.LogError<string>(new string[]
			{
				"is null"
			});
			dictionary = new Dictionary<int, int>();
			return;
		}
		MUEventManager.SendEvent<Dictionary<int, int>>("CMD_SPR_REGRESSACTIVE_ZHIGOU_QUERY", dictionary);
	}

	private static void ShowErrorMessage(int code)
	{
		string errorstring = HuiGuiNetManager.GetErrorstring(code);
		Super.HintMainText(errorstring, 10, 3);
	}

	private static string GetErrorstring(int code)
	{
		string chineseText = string.Empty;
		switch (code)
		{
		case 1:
			chineseText = Global.GetLang("成功");
			break;
		case 2:
			chineseText = Global.GetLang("获取活动开启状态错误");
			break;
		case 3:
			chineseText = Global.GetLang("不在活动范围内");
			break;
		case 4:
			chineseText = Global.GetLang("获取最早的注册时间出错");
			break;
		case 5:
			chineseText = Global.GetLang("获取活动归档失败");
			break;
		case 6:
			chineseText = Global.GetLang("用户信息出错");
			break;
		case 7:
			chineseText = Global.GetLang("签到验证失败");
			break;
		case 8:
			chineseText = Global.GetLang("签到记录查询失败");
			break;
		case 9:
			chineseText = Global.GetLang("获取签到配置出错");
			break;
		case 10:
			chineseText = Global.GetLang("当前活动过期");
			break;
		case 11:
			chineseText = Global.GetLang("获取发放奖励失败");
			break;
		case 12:
			chineseText = Global.GetLang("发放奖励失败");
			break;
		case 13:
			chineseText = Global.GetLang("重生背包空间不足");
			break;
		case 14:
			chineseText = Global.GetLang("普通背包空间不足");
			break;
		case 15:
			chineseText = Global.GetLang("记录领奖记录失败");
			break;
		case 16:
			chineseText = Global.GetLang("获取签到信息失败");
			break;
		case 17:
			chineseText = Global.GetLang("没有获取到数据");
			break;
		case 18:
			chineseText = Global.GetLang("检索类型出错");
			break;
		case 19:
			chineseText = Global.GetLang("天数计算出错");
			break;
		case 20:
			chineseText = Global.GetLang("已经签到了");
			break;
		case 21:
			chineseText = Global.GetLang("没到领奖时间");
			break;
		case 22:
			chineseText = Global.GetLang("获取商城配置出错");
			break;
		case 23:
			chineseText = Global.GetLang("商城验证失败");
			break;
		case 24:
			chineseText = Global.GetLang("商城购买物品失败");
			break;
		case 25:
			chineseText = Global.GetLang("商城获取天数验证失败");
			break;
		case 26:
			chineseText = Global.GetLang("商城验证物品失败");
			break;
		case 27:
			chineseText = Global.GetLang("商城验证参数出错");
			break;
		case 28:
			chineseText = Global.GetLang("钻石不足");
			break;
		case 29:
			chineseText = Global.GetLang("商城记录信息出错");
			break;
		case 30:
			chineseText = Global.GetLang("获取充值信息错误");
			break;
		case 31:
			chineseText = Global.GetLang("已经领取过充值奖励了");
			break;
		case 32:
			chineseText = Global.GetLang("累计充值配置错误");
			break;
		case 33:
			chineseText = Global.GetLang("检查奖励失败");
			break;
		case 34:
			chineseText = Global.GetLang("发放奖励失败");
			break;
		case 35:
			chineseText = Global.GetLang("获取直购信息错误");
			break;
		case 36:
			chineseText = Global.GetLang("获取签到信息错误");
			break;
		case 37:
			chineseText = Global.GetLang("获取签到信息错误");
			break;
		}
		return Global.GetLang(chineseText);
	}
}
