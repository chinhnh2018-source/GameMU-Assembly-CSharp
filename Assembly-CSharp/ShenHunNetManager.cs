using System;
using GameServer.Logic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameFramework.Logic;
using Server.Data;
using Server.Tools;

public class ShenHunNetManager
{
	public static void ServeBianChengLevelUp(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		BianShenUpdateResultData bianShenUpdateResultData = DataHelper.BytesToObject<BianShenUpdateResultData>(e.bytesData, 0, e.bytesData.Length);
		if (bianShenUpdateResultData == null)
		{
			Super.HintMainText(Global.GetLang("参数错误"), 10, 3);
			return;
		}
		if (bianShenUpdateResultData.Result < 0)
		{
			ShiLiData.ShowErrorMessage(bianShenUpdateResultData.Result);
		}
		if (Global.Data.roleData.BianShenData == null)
		{
			Global.Data.roleData.BianShenData = new RoleBianShenData();
		}
		Global.Data.roleData.BianShenData.BianShen = bianShenUpdateResultData.BianShen;
		Global.Data.roleData.BianShenData.Exp = bianShenUpdateResultData.Exp;
		MUEventManager.SendEvent<BianShenUpdateResultData>("CMD_DB_BIANSHEN_LEVEL_UP", bianShenUpdateResultData);
	}

	public static void ServeFashionForge(MUSocketConnectEventArgs e)
	{
		Super.HideNetWaiting();
		int num = int.Parse(e.fields[0]);
		if (num < 0)
		{
			string errMsg = StdErrorCode.GetErrMsg(num, true, false);
			Super.HintMainText(Global.GetLang(errMsg), 10, 3);
			return;
		}
		int @params = int.Parse(e.fields[2]);
		int params2 = int.Parse(e.fields[3]);
		MUEventManager.SendEvent<int, int>("CMD_SPR_FASHION_FORGE", @params, params2);
	}

	public static void ServerFashionEquip(SCModGoods ModGoods)
	{
		Super.HideNetWaiting();
		GoodsData goodsData = Global.Data.fashionAndTitleList.Find((GoodsData e) => e.Id == ModGoods.ID);
		if (goodsData != null && Global.GetGoodsCatetoriy(goodsData.GoodsID) == 28)
		{
			MUEventManager.SendEvent<int, bool>("CMD_SPR_FASHION_EQUIP", goodsData.GoodsID, goodsData.Using == 1);
		}
	}
}
