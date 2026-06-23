using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Common;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class JueXingNetManager
{
	public static void ServerJueXingGetInfo(MUSocketConnectEventArgs e)
	{
		JueXingShiData jueXingShiData = DataHelper.BytesToObject<JueXingShiData>(e.bytesData, 0, e.bytesData.Length);
		Super.HideNetWaiting();
		JueXingShiData jueXingShiData2;
		if (jueXingShiData == null)
		{
			jueXingShiData2 = new JueXingShiData();
			jueXingShiData2.AttackEquip = 0;
			jueXingShiData2.DefenseEquip = 0;
			jueXingShiData2.JueXingJi = 0;
			jueXingShiData2.JueXingJie = 1;
			jueXingShiData2.TaoZhuangList = new List<TaoZhuangData>();
		}
		else
		{
			jueXingShiData2 = jueXingShiData;
			if (jueXingShiData2.TaoZhuangList == null)
			{
				jueXingShiData2.TaoZhuangList = new List<TaoZhuangData>();
			}
		}
		JueXingData.SetSelfJueXingData(jueXingShiData2);
		MUEventManager.SendEvent<JueXingShiData>("CMD_SPR_JUEXING_INFO", jueXingShiData2);
	}

	public static void ServerJueXingShiJiHuo(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			JueXingData.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		if (e.fields.Length < 3)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		int num2 = ConvertExt.SafeConvertToInt32(e.fields[2]);
		JueXingData.JueXingJiHuo(num, num2);
		if (num == JueXingData.GetSelfJueXingData().AttackEquip || num == JueXingData.GetSelfJueXingData().DefenseEquip)
		{
			Global.Data.GameScene.ResetLeaderJueXingTeXiao();
		}
		JueXingNetManager.UpdateJueXingSuiPian(null);
		MUEventManager.SendEvent<int, int>("CMD_SPR_JUEXING_JIHUO", num, num2);
	}

	public static void ServerJueXingTaoZhuangSelect(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			JueXingData.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		if (e.fields.Length < 3)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		int num2 = ConvertExt.SafeConvertToInt32(e.fields[2]);
		JueXingData.SetEquipTaoZhuangId((JueXingTaoZhuangType)num, num2);
		Global.Data.GameScene.ResetLeaderJueXingTeXiao();
		MUEventManager.SendEvent<int, int>("CMD_SPR_JUEXING_TAOCHANGE", num, num2);
	}

	public static void ServerJueXingMoHua(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			JueXingData.ShowErrorMessage(int.Parse(e.fields[0]));
			return;
		}
		if (e.fields.Length < 3)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		int num = ConvertExt.SafeConvertToInt32(e.fields[1]);
		int num2 = ConvertExt.SafeConvertToInt32(e.fields[2]);
		JueXingData.GetSelfJueXingData().JueXingJie = num;
		JueXingData.GetSelfJueXingData().JueXingJi = num2;
		MUEventManager.SendEvent<int, int>("CMD_SPR_JUEXING_MOHUA", num, num2);
	}

	public static void ServerJueXingHuiShou(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		if (int.Parse(e.fields[0]) < 0)
		{
			JueXingData.ShowErrorMessage(int.Parse(e.fields[0]));
			MUEventManager.SendEvent("CMD_SPR_JUEXING_HUISHOU");
			return;
		}
		MUEventManager.SendEvent("CMD_SPR_JUEXING_HUISHOU");
	}

	public static void UpdateJueXingSuiPian(GoodsData goodsData)
	{
		bool active = JueXingData.BeHaveCanJuHuo();
		ActivityTipManager.SetActivityTipItemActive(18003, active);
	}
}
