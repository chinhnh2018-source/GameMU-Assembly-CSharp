using System;
using Server.Data;

namespace GameServer.Logic.Goods
{
	public class CondJudger_HuFuSuit : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			if (client != null && !string.IsNullOrEmpty(arg))
			{
				GoodsData goodsDataByCategoriy = client.UsingEquipMgr.GetGoodsDataByCategoriy(client, 22);
				if (goodsDataByCategoriy != null)
				{
					int num = -1;
					if (int.TryParse(arg, out num) && Global.GetEquipGoodsSuitID(goodsDataByCategoriy.GoodsID) >= num)
					{
						flag = true;
					}
				}
			}
			if (!flag)
			{
				failedMsg = string.Format(GLang.GetLang(140, new object[0]), string.Format(GLang.GetLang(681, new object[0]), arg));
			}
			return flag;
		}
	}
}
