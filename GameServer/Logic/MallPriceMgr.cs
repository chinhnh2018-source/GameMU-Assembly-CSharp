using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class MallPriceMgr
	{
		public static void ClearCache()
		{
			lock (MallPriceMgr.PriceDict)
			{
				MallPriceMgr.PriceDict.Clear();
			}
		}

		public static int GetPriceByGoodsID(int goodsID)
		{
			int num = -1;
			if (!MallPriceMgr.PriceDict.TryGetValue(goodsID, out num))
			{
				num = MallPriceMgr.GetPriceByGoodsIDFromCfg(goodsID);
				if (num > 0)
				{
					lock (MallPriceMgr.PriceDict)
					{
						MallPriceMgr.PriceDict.Add(goodsID, num);
					}
				}
			}
			return num;
		}

		private static int GetPriceByGoodsIDFromCfg(int goodsID)
		{
			foreach (SystemXmlItem systemXmlItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int intValue = systemXmlItem.GetIntValue("GoodsID", -1);
				if (intValue == goodsID)
				{
					int intValue2 = systemXmlItem.GetIntValue("Price", -1);
					if (intValue2 <= 0)
					{
						return -1;
					}
					string stringValue = systemXmlItem.GetStringValue("PubStartTime");
					string stringValue2 = systemXmlItem.GetStringValue("PubEndTime");
					if (!string.IsNullOrEmpty(stringValue) && !string.IsNullOrEmpty(stringValue2))
					{
						long num = Global.SafeConvertToTicks(stringValue);
						long num2 = Global.SafeConvertToTicks(stringValue2);
						long num3 = TimeUtil.NOW();
						if (num3 < num || num3 > num2)
						{
							return -1;
						}
					}
					return intValue2;
				}
			}
			return -1;
		}

		public static int GetPriceTypeByGoodsIDFromCfg(int goodsID)
		{
			foreach (SystemXmlItem systemXmlItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int intValue = systemXmlItem.GetIntValue("GoodsID", -1);
				if (intValue == goodsID)
				{
					int intValue2 = systemXmlItem.GetIntValue("TabID", -1);
					if (10000 == intValue2)
					{
						return 1;
					}
				}
			}
			return 0;
		}

		private static Dictionary<int, int> PriceDict = new Dictionary<int, int>();
	}
}
