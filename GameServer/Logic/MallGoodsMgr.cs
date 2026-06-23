using System;
using System.Collections.Generic;
using Server.Tools;

namespace GameServer.Logic
{
	public class MallGoodsMgr
	{
		public static void InitMallGoodsPriceDict()
		{
			foreach (SystemXmlItem systemXmlItem in GameManager.systemMallMgr.SystemXmlItemDict.Values)
			{
				int intValue = systemXmlItem.GetIntValue("GoodsID", -1);
				int intValue2 = systemXmlItem.GetIntValue("Price", -1);
				string stringValue = systemXmlItem.GetStringValue("Property");
				if (string.IsNullOrEmpty(stringValue))
				{
					LogManager.WriteLog(2, string.Format("加载商城出售列表时, 物品配置属性错误，忽略。{0}", stringValue), null, true);
				}
				else
				{
					string[] array = stringValue.Split(new char[]
					{
						','
					});
					if (4 != array.Length)
					{
						LogManager.WriteLog(2, string.Format("加载加载商城出售列表时出售列表时, 物品配置项个数错误，忽略。{0}", systemXmlItem.GetIntValue("ID", -1)), null, true);
					}
					else
					{
						MallGoodsCacheItem value = new MallGoodsCacheItem
						{
							Price = intValue2,
							Forge_level = Math.Max(0, Global.SafeConvertToInt32(array[0])),
							AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(array[1])),
							Lucky = Math.Max(0, Global.SafeConvertToInt32(array[2])),
							ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(array[3]))
						};
						MallGoodsMgr.MallGoodsCacheDict[intValue] = value;
					}
				}
			}
			foreach (SystemXmlItem systemXmlItem in GameManager.systemQiZhenGeGoodsMgr.SystemXmlItemDict.Values)
			{
				int intValue = systemXmlItem.GetIntValue("GoodsID", -1);
				int intValue2 = systemXmlItem.GetIntValue("Price", -1);
				string stringValue = systemXmlItem.GetStringValue("Property");
				if (string.IsNullOrEmpty(stringValue))
				{
					LogManager.WriteLog(2, string.Format("加载商城出售列表时, 物品配置属性错误，忽略。{0}", stringValue), null, true);
				}
				else
				{
					string[] array = stringValue.Split(new char[]
					{
						','
					});
					if (4 != array.Length)
					{
						LogManager.WriteLog(2, string.Format("加载加载商城出售列表时出售列表时, 物品配置项个数错误，忽略。{0}", systemXmlItem.GetIntValue("ID", -1)), null, true);
					}
					else
					{
						MallGoodsCacheItem value = new MallGoodsCacheItem
						{
							Price = intValue2,
							Forge_level = Math.Max(0, Global.SafeConvertToInt32(array[0])),
							AppendPropLev = Math.Max(0, Global.SafeConvertToInt32(array[1])),
							Lucky = Math.Max(0, Global.SafeConvertToInt32(array[2])),
							ExcellenceInfo = Math.Max(0, Global.SafeConvertToInt32(array[3]))
						};
						MallGoodsMgr.MallGoodsCacheDict[intValue] = value;
					}
				}
			}
		}

		public static MallGoodsCacheItem GetMallGoodsCacheItem(int goodsID)
		{
			MallGoodsCacheItem mallGoodsCacheItem = null;
			MallGoodsCacheItem result;
			if (!MallGoodsMgr.MallGoodsCacheDict.TryGetValue(goodsID, out mallGoodsCacheItem))
			{
				result = null;
			}
			else
			{
				result = mallGoodsCacheItem;
			}
			return result;
		}

		private static Dictionary<int, MallGoodsCacheItem> MallGoodsCacheDict = new Dictionary<int, MallGoodsCacheItem>();
	}
}
