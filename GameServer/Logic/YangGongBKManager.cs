using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class YangGongBKManager
	{
		public static YangGongBKItem OpenYangGongBK(GameClient client, bool isBaoWuBinding)
		{
			YangGongBKItem yangGongBKItem = null;
			int num = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID1", -1);
			int num2 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID2", -1);
			int num3 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID3", -1);
			int num4 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID4", -1);
			int num5 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID5", -1);
			int num6 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID6", -1);
			int num7 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID7", -1);
			int num8 = (int)GameManager.systemParamsList.GetParamValueIntByName("YangGongFallID8", -1);
			YangGongBKItem result;
			if (num <= 0 || num2 <= 0 || num3 <= 0 || num4 <= 0)
			{
				result = yangGongBKItem;
			}
			else if (num5 <= 0 || num6 <= 0 || num7 <= 0 || num8 <= 0)
			{
				result = yangGongBKItem;
			}
			else
			{
				List<FallGoodsItem> list = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num, 1, false);
				List<FallGoodsItem> randomFallGoodsItemList = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num2, 1, false);
				List<FallGoodsItem> randomFallGoodsItemList2 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num3, 1, false);
				List<FallGoodsItem> randomFallGoodsItemList3 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num4, 1, false);
				List<FallGoodsItem> randomFallGoodsItemList4 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num5, 1, true);
				List<FallGoodsItem> randomFallGoodsItemList5 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num6, 1, true);
				List<FallGoodsItem> randomFallGoodsItemList6 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num7, 1, true);
				List<FallGoodsItem> randomFallGoodsItemList7 = GameManager.GoodsPackMgr.GetRandomFallGoodsItemList(num8, 1, true);
				if (list == null || randomFallGoodsItemList == null || randomFallGoodsItemList2 == null || null == randomFallGoodsItemList3)
				{
					result = yangGongBKItem;
				}
				else if (randomFallGoodsItemList4 == null || randomFallGoodsItemList5 == null || randomFallGoodsItemList6 == null || null == randomFallGoodsItemList7)
				{
					result = yangGongBKItem;
				}
				else if (1 != list.Count || 1 != randomFallGoodsItemList.Count || 1 != randomFallGoodsItemList2.Count || 1 != randomFallGoodsItemList3.Count)
				{
					result = yangGongBKItem;
				}
				else if (1 != randomFallGoodsItemList4.Count || 1 != randomFallGoodsItemList5.Count || 1 != randomFallGoodsItemList6.Count || 1 != randomFallGoodsItemList7.Count)
				{
					result = yangGongBKItem;
				}
				else
				{
					list.AddRange(randomFallGoodsItemList);
					list.AddRange(randomFallGoodsItemList2);
					list.AddRange(randomFallGoodsItemList3);
					list.AddRange(randomFallGoodsItemList4);
					list.AddRange(randomFallGoodsItemList5);
					list.AddRange(randomFallGoodsItemList6);
					list.AddRange(randomFallGoodsItemList7);
					list = Global.RandomSortList<FallGoodsItem>(list);
					List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(list);
					List<GoodsData> list2 = new List<GoodsData>();
					for (int i = 0; i < goodsDataListFromFallGoodsItemList.Count; i++)
					{
						list2.Add(goodsDataListFromFallGoodsItemList[i]);
					}
					yangGongBKItem = new YangGongBKItem
					{
						FallGoodsItemList = list,
						GoodsDataList = goodsDataListFromFallGoodsItemList,
						IsBaoWuBinding = isBaoWuBinding,
						TempGoodsDataList = list2
					};
					result = yangGongBKItem;
				}
			}
			return result;
		}

		public static int ClickYangGongBK(GameClient client, YangGongBKItem yangGongBKItem, out GoodsData goodsData)
		{
			goodsData = null;
			if (null == YangGongBKManager.YangGongBKNumPercents)
			{
				YangGongBKManager.YangGongBKNumPercents = GameManager.systemParamsList.GetParamValueDoubleArrayByName("YangGongBKNumPercents", ',');
			}
			int result;
			if (YangGongBKManager.YangGongBKNumPercents == null || YangGongBKManager.YangGongBKNumPercents.Length != 4)
			{
				result = -1000;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = yangGongBKItem.FallGoodsItemList;
				List<GoodsData> goodsDataList = yangGongBKItem.GoodsDataList;
				if (fallGoodsItemList == null || null == goodsDataList)
				{
					result = -200;
				}
				else if (fallGoodsItemList.Count != goodsDataList.Count)
				{
					result = -200;
				}
				else
				{
					double num = YangGongBKManager.YangGongBKNumPercents[yangGongBKItem.ClickBKNum];
					int num2 = -1;
					for (int i = 0; i < fallGoodsItemList.Count; i++)
					{
						int randomNumber = Global.GetRandomNumber(1, 10001);
						int num3 = fallGoodsItemList[i].SelfPercent;
						if (fallGoodsItemList[i].IsGood)
						{
							num3 = (int)((double)num3 * num);
						}
						if (randomNumber <= num3)
						{
							if (!yangGongBKItem.PickUpDict.ContainsKey(goodsDataList[i].Id))
							{
								num2 = i;
								break;
							}
						}
					}
					if (-1 == num2)
					{
						int num4 = -1;
						int num5 = 0;
						for (int i = 0; i < fallGoodsItemList.Count; i++)
						{
							if (!yangGongBKItem.PickUpDict.ContainsKey(goodsDataList[i].Id))
							{
								if (fallGoodsItemList[i].SelfPercent > num5)
								{
									num4 = i;
									num5 = fallGoodsItemList[i].SelfPercent;
								}
							}
						}
						num2 = num4;
					}
					if (num2 < 0)
					{
						result = -300;
					}
					else
					{
						goodsData = goodsDataList[num2];
						result = num2;
					}
				}
			}
			return result;
		}

		private static double[] YangGongBKNumPercents = null;
	}
}
