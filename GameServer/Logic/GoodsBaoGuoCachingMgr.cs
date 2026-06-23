using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class GoodsBaoGuoCachingMgr
	{
		public static List<GoodsData> FindGoodsBaoGuoByID(int baoguoID)
		{
			List<GoodsData> result = null;
			GoodsBaoGuoCachingMgr._GoodsBaoGuoDict.TryGetValue(baoguoID, out result);
			return result;
		}

		public static int LoadGoodsBaoGuoDict()
		{
			try
			{
				Dictionary<int, List<GoodsData>> dictionary = new Dictionary<int, List<GoodsData>>();
				foreach (SystemXmlItem systemXmlItem in GameManager.systemGoodsBaoGuoMgr.SystemXmlItemDict.Values)
				{
					int intValue = systemXmlItem.GetIntValue("ID", -1);
					string stringValue = systemXmlItem.GetStringValue("Item");
					if (string.IsNullOrEmpty(stringValue))
					{
						LogManager.WriteLog(1, string.Format("加载物品包时, 读取物品列表错误, BaoguoID={0}", intValue), null, true);
					}
					else
					{
						string[] array = stringValue.Split(new char[]
						{
							'|'
						});
						if (array == null || array.Length <= 0)
						{
							LogManager.WriteLog(1, string.Format("加载物品包时, 物品列表格式错误, BaoguoID={0}, List={1}", intValue, array), null, true);
						}
						else
						{
							List<GoodsData> list = new List<GoodsData>();
							for (int i = 0; i < array.Length; i++)
							{
								string[] array2 = array[i].Trim().Split(new char[]
								{
									','
								});
								if (array2 == null || array2.Length != 7)
								{
									LogManager.WriteLog(1, string.Format("加载物品包时, 物品列表格中的物品配置项错误, BaoguoID={0}, GoodsItem={1}", intValue, array2), null, true);
								}
								else
								{
									GoodsData item = new GoodsData
									{
										Id = i,
										GoodsID = Global.SafeConvertToInt32(array2[0]),
										Using = 0,
										Forge_level = Global.SafeConvertToInt32(array2[3]),
										Starttime = "1900-01-01 12:00:00",
										Endtime = "1900-01-01 12:00:00",
										Site = 0,
										Quality = 0,
										Props = "",
										GCount = Global.SafeConvertToInt32(array2[1]),
										Binding = Global.SafeConvertToInt32(array2[2]),
										Jewellist = "",
										BagIndex = 0,
										AddPropIndex = 0,
										BornIndex = 0,
										Lucky = Global.SafeConvertToInt32(array2[5]),
										Strong = 0,
										ExcellenceInfo = Global.SafeConvertToInt32(array2[6]),
										AppendPropLev = Global.SafeConvertToInt32(array2[4])
									};
									list.Add(item);
								}
							}
							dictionary[intValue] = list;
						}
					}
				}
				GoodsBaoGuoCachingMgr._GoodsBaoGuoDict = dictionary;
				return 0;
			}
			catch (Exception)
			{
			}
			return -1;
		}

		private static Dictionary<int, List<GoodsData>> _GoodsBaoGuoDict = null;
	}
}
