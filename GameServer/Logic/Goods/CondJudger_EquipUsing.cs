using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Goods
{
	public class CondJudger_EquipUsing : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = false;
			List<int> list = Global.StringToIntList(arg, '|');
			int itemCat = list[0];
			int num = 0;
			if (list.Count >= 2)
			{
				num = list[1];
			}
			List<int> list2 = new List<int>();
			if (1 <= itemCat && itemCat <= 5)
			{
				list2.Add(itemCat - 1);
			}
			else if (6 == itemCat || 7 == itemCat)
			{
				for (int i = 11; i <= 21; i++)
				{
					list2.Add(i);
				}
			}
			else if (8 == itemCat || 9 == itemCat)
			{
				list2.Add(6);
			}
			else if (10 == itemCat)
			{
				list2.Add(5);
			}
			GoodsData goodsData = null;
			List<GoodsData> goodsByCategoriyList = client.UsingEquipMgr.GetGoodsByCategoriyList(list2);
			if (goodsByCategoriyList == null || goodsByCategoriyList.Count == 0)
			{
				flag = false;
			}
			else if ((1 <= itemCat && itemCat <= 5) || 10 == itemCat)
			{
				flag = true;
				goodsData = goodsByCategoriyList[0];
			}
			else if (6 == itemCat || 7 == itemCat)
			{
				goodsData = goodsByCategoriyList.Find(delegate(GoodsData x)
				{
					int num2 = -1;
					int num3 = -1;
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(x.GoodsID, out systemXmlItem))
					{
						num2 = systemXmlItem.GetIntValue("HandType", -1);
						num3 = systemXmlItem.GetIntValue("ActionType", -1);
					}
					return (6 == itemCat && num2 == 1) || (6 == itemCat && num2 == 2 && x.BagIndex == 0) || (7 == itemCat && num2 == 0) || (7 == itemCat && num2 == 2 && x.BagIndex == 1) || (num3 != 1 && num3 != 4);
				});
				if (null != goodsData)
				{
					flag = true;
				}
			}
			else if (8 == itemCat || 9 == itemCat)
			{
				if (8 == itemCat)
				{
					goodsData = goodsByCategoriyList.Find((GoodsData x) => x.BagIndex == 0);
				}
				else
				{
					goodsData = goodsByCategoriyList.Find((GoodsData x) => x.BagIndex == 1);
				}
				if (null != goodsData)
				{
					flag = true;
				}
			}
			if (!flag || null == goodsData)
			{
				failedMsg = GLang.GetLang(8016, new object[0]);
			}
			else if (num > 0)
			{
				if (goodsData.Forge_level >= num)
				{
					flag = false;
					failedMsg = GLang.GetLang(8020, new object[0]);
				}
			}
			return flag;
		}
	}
}
