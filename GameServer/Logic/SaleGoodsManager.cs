using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class SaleGoodsManager
	{
		public static void AddSaleGoodsItem(SaleGoodsItem saleGoodsItem)
		{
			SaleManager.AddSaleGoodsItem(saleGoodsItem);
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				SaleGoodsManager._SaleGoodsDict[saleGoodsItem.GoodsDbID] = saleGoodsItem;
				SaleGoodsManager._SaleGoodsDataList = null;
			}
		}

		public static void AddSaleGoodsItems(GameClient client)
		{
			List<GoodsData> saleGoodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != saleGoodsDataList)
			{
				lock (saleGoodsDataList)
				{
					for (int i = 0; i < saleGoodsDataList.Count; i++)
					{
						if (saleGoodsDataList[i].Binding <= 0)
						{
							SaleGoodsItem saleGoodsItem = new SaleGoodsItem
							{
								GoodsDbID = saleGoodsDataList[i].Id,
								SalingGoodsData = saleGoodsDataList[i],
								Client = client
							};
							SaleGoodsManager.AddSaleGoodsItem(saleGoodsItem);
						}
					}
				}
			}
		}

		public static SaleGoodsItem RemoveSaleGoodsItem(int goodsDbID)
		{
			SaleManager.RemoveSaleGoodsItem(goodsDbID);
			SaleGoodsItem result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				SaleGoodsItem saleGoodsItem = null;
				if (SaleGoodsManager._SaleGoodsDict.TryGetValue(goodsDbID, out saleGoodsItem))
				{
					SaleGoodsManager._SaleGoodsDict.Remove(goodsDbID);
				}
				SaleGoodsManager._SaleGoodsDataList = null;
				result = saleGoodsItem;
			}
			return result;
		}

		public static void RemoveSaleGoodsItems(GameClient client)
		{
			List<GoodsData> saleGoodsDataList = client.ClientData.SaleGoodsDataList;
			if (null != saleGoodsDataList)
			{
				lock (saleGoodsDataList)
				{
					for (int i = 0; i < saleGoodsDataList.Count; i++)
					{
						SaleGoodsManager.RemoveSaleGoodsItem(saleGoodsDataList[i].Id);
					}
				}
			}
		}

		public static List<SaleGoodsData> GetSaleGoodsDataList()
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				if (null != SaleGoodsManager._SaleGoodsDataList)
				{
					result = SaleGoodsManager._SaleGoodsDataList;
				}
				else
				{
					List<SaleGoodsData> list = new List<SaleGoodsData>();
					foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
					{
						list.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (list.Count >= 250)
						{
							break;
						}
					}
					SaleGoodsManager._SaleGoodsDataList = list;
					result = list;
				}
			}
			return result;
		}

		public static List<SaleGoodsData> FindSaleGoodsDataList(Dictionary<int, bool> goodsIDDict)
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				List<SaleGoodsData> list = new List<SaleGoodsData>();
				foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
				{
					if (goodsIDDict.ContainsKey(saleGoodsItem.SalingGoodsData.GoodsID))
					{
						list.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (list.Count >= 250)
						{
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static List<SaleGoodsData> FindSaleGoodsDataListByRoleName(string searchText)
		{
			List<SaleGoodsData> result;
			lock (SaleGoodsManager._SaleGoodsDict)
			{
				List<SaleGoodsData> list = new List<SaleGoodsData>();
				foreach (SaleGoodsItem saleGoodsItem in SaleGoodsManager._SaleGoodsDict.Values)
				{
					if (-1 != saleGoodsItem.Client.ClientData.RoleName.IndexOf(searchText))
					{
						list.Add(new SaleGoodsData
						{
							GoodsDbID = saleGoodsItem.GoodsDbID,
							SalingGoodsData = saleGoodsItem.SalingGoodsData,
							RoleID = saleGoodsItem.Client.ClientData.RoleID,
							RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
							RoleLevel = saleGoodsItem.Client.ClientData.Level
						});
						if (list.Count >= 250)
						{
							break;
						}
					}
				}
				result = list;
			}
			return result;
		}

		public static int GetNewBaiTanJinBiID()
		{
			int result;
			lock (SaleGoodsManager.Mutex)
			{
				int baseBaiTanJinBiID = SaleGoodsManager.BaseBaiTanJinBiID;
				SaleGoodsManager.BaseBaiTanJinBiID--;
				result = baseBaiTanJinBiID;
			}
			return result;
		}

		private static List<SaleGoodsData> _SaleGoodsDataList = null;

		private static Dictionary<int, SaleGoodsItem> _SaleGoodsDict = new Dictionary<int, SaleGoodsItem>();

		private static object Mutex = new object();

		private static int BaseBaiTanJinBiID = -1;
	}
}
