using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic.Goods
{
	public class CondJudger_EquipUsingAll : ICondJudger
	{
		public bool Judge(GameClient client, string arg, out string failedMsg)
		{
			failedMsg = "";
			bool flag = true;
			int forgelev = Global.SafeConvertToInt32(arg);
			List<int> list = new List<int>();
			for (int i = 0; i <= 6; i++)
			{
				list.Add(i);
			}
			List<GoodsData> goodsByCategoriyList = client.UsingEquipMgr.GetGoodsByCategoriyList(list);
			if (goodsByCategoriyList == null || goodsByCategoriyList.Count != list.Count + 1)
			{
				flag = false;
			}
			List<GoodsData> list2 = null;
			if (flag)
			{
				list.Clear();
				for (int i = 11; i <= 21; i++)
				{
					list.Add(i);
				}
				list2 = client.UsingEquipMgr.GetGoodsByCategoriyList(list);
				if (list2 == null || list2.Count == 0)
				{
					flag = false;
				}
				else if (list2.Count == 1)
				{
					SystemXmlItem systemXmlItem = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(list2[0].GoodsID, out systemXmlItem))
					{
						flag = false;
					}
					else
					{
						int intValue = systemXmlItem.GetIntValue("ActionType", -1);
						if (intValue == 1 || intValue == 4)
						{
							flag = false;
						}
					}
				}
			}
			if (!flag)
			{
				failedMsg = GLang.GetLang(8017, new object[0]);
			}
			else if (forgelev > 0)
			{
				bool flag2;
				if (goodsByCategoriyList.TrueForAll((GoodsData x) => x.Forge_level >= forgelev))
				{
					flag2 = !list2.TrueForAll((GoodsData x) => x.Forge_level >= forgelev);
				}
				else
				{
					flag2 = true;
				}
				if (!flag2)
				{
					flag = false;
					failedMsg = GLang.GetLang(8020, new object[0]);
				}
			}
			return flag;
		}
	}
}
