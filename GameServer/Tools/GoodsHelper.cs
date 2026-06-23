using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Tools
{
	public class GoodsHelper
	{
		public static List<GoodsData> ParseGoodsDataList(string[] fields, string fileName)
		{
			int num = 7;
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < fields.Length; i++)
			{
				string[] array = fields[i].Split(new char[]
				{
					','
				});
				if (array.Length != num)
				{
					LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				}
				else
				{
					int[] array2 = Global.StringArray2IntArray(array);
					GoodsData newGoodsData = Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
					list.Add(newGoodsData);
				}
			}
			return list;
		}

		public static List<GoodsData> GetAwardPro(GameClient client, List<GoodsData> proGoodsList)
		{
			List<GoodsData> result;
			if (proGoodsList == null || proGoodsList.Count <= 0)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = new List<GoodsData>();
				foreach (GoodsData goodsData in proGoodsList)
				{
					if (Global.IsCanGiveRewardByOccupation(client, goodsData.GoodsID))
					{
						list.Add(goodsData);
					}
				}
				result = list;
			}
			return result;
		}

		public static GoodsData ParseGoodsData(string fields, string fileName)
		{
			int num = 7;
			string[] array = fields.Split(new char[]
			{
				','
			});
			GoodsData result;
			if (array.Length != num)
			{
				LogManager.WriteLog(1, string.Format("解析{0}文件中的奖励项时失败, 物品配置项个数错误", fileName), null, true);
				result = null;
			}
			else
			{
				int[] array2 = Global.StringArray2IntArray(array);
				result = Global.GetNewGoodsData(array2[0], array2[1], 0, array2[3], array2[2], 0, array2[5], 0, array2[6], array2[4], 0);
			}
			return result;
		}
	}
}
