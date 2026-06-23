using System;
using System.Collections.Generic;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class QiZhenGeManager
	{
		private static void InitQiZhenGeCachingItems()
		{
			if (QiZhenGeManager.QiZhenGeItemDataList.Count <= 0)
			{
				int num = 0;
				foreach (SystemXmlItem systemXmlItem in GameManager.systemQiZhenGeGoodsMgr.SystemXmlItemDict.Values)
				{
					int num2 = (int)(systemXmlItem.GetDoubleValue("Probability") * 10000.0);
					QiZhenGeManager.QiZhenGeItemDataList.Add(new QiZhenGeItemData
					{
						ItemID = systemXmlItem.GetIntValue("ID", -1),
						GoodsID = systemXmlItem.GetIntValue("GoodsID", -1),
						OrigPrice = systemXmlItem.GetIntValue("OrigPrice", -1),
						Price = systemXmlItem.GetIntValue("Price", -1),
						Description = systemXmlItem.GetStringValue("Description"),
						BaseProbability = num,
						SelfProbability = num2
					});
					num += num2;
				}
				if (num > 10000)
				{
					LogManager.WriteLog(2, string.Format("解析奇珍阁配置项时发生概率溢出10000错误", new object[0]), null, true);
				}
			}
		}

		public static void ClearQiZhenGeCachingItems()
		{
			lock (QiZhenGeManager.QiZhenMutex)
			{
				QiZhenGeManager.QiZhenGeItemDataList.Clear();
			}
		}

		private static QiZhenGeItemData PickUpQiZhenGeItemDataByPercent(List<QiZhenGeItemData> qiZhenGeItemDataList, int randPercent)
		{
			QiZhenGeItemData result = null;
			for (int i = 0; i < qiZhenGeItemDataList.Count; i++)
			{
				if (randPercent > qiZhenGeItemDataList[i].BaseProbability && randPercent <= qiZhenGeItemDataList[i].BaseProbability + qiZhenGeItemDataList[i].SelfProbability)
				{
					result = qiZhenGeItemDataList[i];
					break;
				}
			}
			return result;
		}

		public static List<QiZhenGeItemData> GetRandomQiZhenGeCachingItems(int maxNum)
		{
			List<QiZhenGeItemData> list = null;
			lock (QiZhenGeManager.QiZhenMutex)
			{
				QiZhenGeManager.InitQiZhenGeCachingItems();
				list = Global.RandomSortList<QiZhenGeItemData>(QiZhenGeManager.QiZhenGeItemDataList);
				QiZhenGeManager.QiZhenGeItemDataList = list;
			}
			List<QiZhenGeItemData> result;
			if (null == list)
			{
				result = null;
			}
			else
			{
				List<QiZhenGeItemData> list2 = new List<QiZhenGeItemData>();
				for (int i = 0; i < maxNum; i++)
				{
					int randomNumber = Global.GetRandomNumber(1, 10001);
					QiZhenGeItemData item = QiZhenGeManager.PickUpQiZhenGeItemDataByPercent(list, randomNumber);
					list2.Add(item);
				}
				result = list2;
			}
			return result;
		}

		public static List<QiZhenGeItemData> GetQiZhenGeGoodsList(GameClient client)
		{
			return QiZhenGeManager.GetRandomQiZhenGeCachingItems(Global.MaxNumPerRefreshQiZhenGe);
		}

		public static object QiZhenMutex = new object();

		private static List<QiZhenGeItemData> QiZhenGeItemDataList = new List<QiZhenGeItemData>();
	}
}
