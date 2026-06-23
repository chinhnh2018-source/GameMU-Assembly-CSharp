using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Server;
using GameServer.Server.CmdProcesser;
using Server.Data;

namespace GameServer.Logic
{
	public class SaleManager : IManager
	{
		public static SaleManager getInstance()
		{
			return SaleManager.instance;
		}

		public bool initialize()
		{
			SaleManager.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(652, 3, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_OPENMARKET2));
			TCPCmdDispatcher.getInstance().registerProcessor(653, 3, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETSALEMONEY2));
			TCPCmdDispatcher.getInstance().registerProcessor(654, 7, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_SALEGOODS2));
			TCPCmdDispatcher.getInstance().registerProcessor(655, 1, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_SELFSALEGOODSLIST2));
			TCPCmdDispatcher.getInstance().registerProcessor(656, 2, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_OTHERSALEGOODSLIST2));
			TCPCmdDispatcher.getInstance().registerProcessor(657, 1, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETROLELIST2));
			TCPCmdDispatcher.getInstance().registerProcessor(658, 5, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETGOODSLIST2));
			TCPCmdDispatcher.getInstance().registerProcessor(659, 5, SaleCmdsProcessor.getInstance(TCPGameServerCmds.CMD_SPR_MARKETBUYGOODS2));
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public static double JiaoYiShuiJinBi { get; private set; }

		public static double JiaoYiShuiZuanShi { get; private set; }

		public static double JiaoYiShuiMoBi { get; private set; }

		public static int MaxSaleNum { get; private set; }

		public static void InitConfig()
		{
			SaleManager.MaxSaleNum = (int)GameManager.systemParamsList.GetParamValueIntByName("ShangJiaNumber", -1);
			double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("JiaoYiShui", ',');
			if (null != paramValueDoubleArrayByName)
			{
				if (paramValueDoubleArrayByName.Length >= 2)
				{
					SaleManager.JiaoYiShuiJinBi = paramValueDoubleArrayByName[0];
					SaleManager.JiaoYiShuiZuanShi = paramValueDoubleArrayByName[1];
				}
				if (paramValueDoubleArrayByName.Length >= 3)
				{
					SaleManager.JiaoYiShuiMoBi = paramValueDoubleArrayByName[2];
				}
			}
			long num = GameManager.systemParamsList.GetParamValueIntByName("ShangJiaTime", -1) * 1000L;
			if (num > 0L)
			{
				SaleManager.MaxSaleGoodsTime = num;
			}
			foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.JiaoYiType.SystemXmlItemDict)
			{
				int key = keyValuePair.Key;
				int intValue = keyValuePair.Value.GetIntValue("Type", -1);
				Dictionary<int, List<SaleGoodsData>> dictionary;
				if (!SaleManager._SaleGoodsDict2.TryGetValue(intValue, out dictionary))
				{
					dictionary = new Dictionary<int, List<SaleGoodsData>>();
					SaleManager._SaleGoodsDict2.Add(intValue, dictionary);
					SaleManager._TypeHashSet.Add(intValue);
				}
				if (!dictionary.ContainsKey(key))
				{
					dictionary.Add(key, new List<SaleGoodsData>());
				}
				SaleManager._IDHashSet.Add(key);
				int[] intArrayValue = keyValuePair.Value.GetIntArrayValue("GoodsID", '|');
				if (null == intArrayValue)
				{
					int[] intArrayValue2 = keyValuePair.Value.GetIntArrayValue("Categoriy", '|');
					int intValue2 = keyValuePair.Value.GetIntValue("Occupation", -1);
					if (null != intArrayValue2)
					{
						foreach (int num2 in intArrayValue2)
						{
							if (!SaleManager._Categoriy2TabIDDict.ContainsKey((long)intValue2 * 100000000L + (long)num2))
							{
								SaleManager._Categoriy2TabIDDict.Add((long)intValue2 * 100000000L + (long)num2, new int[]
								{
									intValue,
									key
								});
							}
						}
					}
					else
					{
						SaleManager.OthersGoodsTypeAndID = new int[]
						{
							intValue,
							key
						};
					}
				}
				else
				{
					foreach (int key2 in intArrayValue)
					{
						if (!SaleManager._GoodsID2TabIDDict.ContainsKey(key2))
						{
							SaleManager._GoodsID2TabIDDict.Add(key2, new int[]
							{
								intValue,
								key
							});
						}
					}
				}
			}
		}

		public static int[] GetTypeAndID(int goodsID)
		{
			int[] array = null;
			lock (SaleManager._GoodsID2TabIDDict)
			{
				if (!SaleManager._GoodsID2TabIDDict.TryGetValue(goodsID, out array))
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsID);
					int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsID);
					if (SaleManager._Categoriy2TabIDDict.TryGetValue((long)(mainOccupationByGoodsID * 100000000 + goodsCatetoriy), out array))
					{
						SaleManager._GoodsID2TabIDDict.Add(goodsID, array);
						return array;
					}
				}
			}
			int[] result;
			if (null == array)
			{
				result = SaleManager.OthersGoodsTypeAndID;
			}
			else
			{
				result = array;
			}
			return result;
		}

		public static bool IsValidType(int type, int id)
		{
			return SaleManager._TypeHashSet.Contains(type) && (id <= 0 || SaleManager._IDHashSet.Contains(id));
		}

		public static void AddSaleGoodsData(SaleGoodsData saleGoodsData)
		{
			int goodsID = saleGoodsData.SalingGoodsData.GoodsID;
			int[] typeAndID = SaleManager.GetTypeAndID(goodsID);
			if (null != typeAndID)
			{
				lock (SaleManager.Mutex_SaleGoodsDict)
				{
					List<SaleGoodsData> list = SaleManager._SaleGoodsDict2[typeAndID[0]][typeAndID[1]];
					SaleManager.UpdateOrderdList(list, saleGoodsData, true, true, SearchOrderTypes.OrderByMoney);
					SaleManager._SaleGoodsDict[saleGoodsData.GoodsDbID] = saleGoodsData;
					SaleManager.UpdateCachedListForSaleGoodsData(saleGoodsData, typeAndID, true);
				}
			}
		}

		public static void AddSaleGoodsItem(SaleGoodsItem saleGoodsItem)
		{
			SaleGoodsData saleGoodsData = new SaleGoodsData
			{
				GoodsDbID = saleGoodsItem.GoodsDbID,
				SalingGoodsData = saleGoodsItem.SalingGoodsData,
				RoleID = saleGoodsItem.Client.ClientData.RoleID,
				RoleName = Global.FormatRoleName(saleGoodsItem.Client, saleGoodsItem.Client.ClientData.RoleName),
				RoleLevel = saleGoodsItem.Client.ClientData.Level
			};
			SaleManager.AddSaleGoodsData(saleGoodsData);
		}

		public static void AddLiXianSaleGoodsItem(LiXianSaleGoodsItem liXianSaleGoodsItem)
		{
			SaleGoodsData saleGoodsData = new SaleGoodsData
			{
				GoodsDbID = liXianSaleGoodsItem.GoodsDbID,
				SalingGoodsData = liXianSaleGoodsItem.SalingGoodsData,
				RoleID = liXianSaleGoodsItem.RoleID,
				RoleName = liXianSaleGoodsItem.RoleName,
				RoleLevel = liXianSaleGoodsItem.RoleLevel
			};
			SaleManager.AddSaleGoodsData(saleGoodsData);
		}

		public static void RemoveSaleGoodsItem(int goodsDbID)
		{
			lock (SaleManager.Mutex_SaleGoodsDict)
			{
				SaleGoodsData saleGoodsData;
				if (SaleManager._SaleGoodsDict.TryGetValue(goodsDbID, out saleGoodsData))
				{
					int goodsID = saleGoodsData.SalingGoodsData.GoodsID;
					int[] typeAndID = SaleManager.GetTypeAndID(goodsID);
					if (null != typeAndID)
					{
						List<SaleGoodsData> list = SaleManager._SaleGoodsDict2[typeAndID[0]][typeAndID[1]];
						SaleManager.UpdateOrderdList(list, saleGoodsData, true, false, SearchOrderTypes.OrderByMoney);
					}
					SaleManager._SaleGoodsDict.Remove(goodsDbID);
					SaleManager.UpdateCachedListForSaleGoodsData(saleGoodsData, typeAndID, false);
				}
			}
		}

		public static List<SaleGoodsData> GetSaleGoodsDataList(int type, int id = 0)
		{
			List<SaleGoodsData> list = null;
			lock (SaleManager.Mutex_SaleGoodsDict)
			{
				Dictionary<int, List<SaleGoodsData>> dictionary;
				if (type == 1)
				{
					list = new List<SaleGoodsData>();
					foreach (KeyValuePair<int, Dictionary<int, List<SaleGoodsData>>> keyValuePair in SaleManager._SaleGoodsDict2)
					{
						if (keyValuePair.Value != null && keyValuePair.Value.Count > 0)
						{
							foreach (KeyValuePair<int, List<SaleGoodsData>> keyValuePair2 in keyValuePair.Value)
							{
								if (keyValuePair2.Value != null && keyValuePair2.Value.Count > 0)
								{
									ListExt.BinaryCombineDesc<SaleGoodsData>(list, keyValuePair2.Value, SaleGoodsMoneyCompare.Instance);
								}
							}
						}
					}
				}
				else if (SaleManager._SaleGoodsDict2.TryGetValue(type, out dictionary))
				{
					if (!dictionary.TryGetValue(id, out list))
					{
						if (id <= 0 && list == null)
						{
							list = new List<SaleGoodsData>();
							foreach (KeyValuePair<int, List<SaleGoodsData>> keyValuePair3 in dictionary)
							{
								if (keyValuePair3.Key > 0 && null != keyValuePair3.Value)
								{
									ListExt.BinaryCombineDesc<SaleGoodsData>(list, keyValuePair3.Value, SaleGoodsMoneyCompare.Instance);
								}
							}
						}
					}
				}
			}
			if (null == list)
			{
				list = new List<SaleGoodsData>();
			}
			return list;
		}

		private static List<SaleGoodsData> GetCachedSaleGoodsList(SearchArgs searchArgs)
		{
			List<SaleGoodsData> list = null;
			SearchArgs searchArgs2 = new SearchArgs(searchArgs);
			int num = -1;
			int moneyFlags = -1;
			int num2 = -1;
			int num3 = -1;
			lock (SaleManager.Mutex_SaleGoodsDict)
			{
				while (!SaleManager._OrderdSaleGoodsListDict.TryGetValue(searchArgs, out list))
				{
					if (searchArgs.ColorFlags < 63)
					{
						num = searchArgs.ColorFlags;
						searchArgs.ColorFlags = 63;
					}
					else if (searchArgs.MoneyFlags < 7)
					{
						moneyFlags = searchArgs.MoneyFlags;
						searchArgs.MoneyFlags = 7;
					}
					else if (searchArgs.OrderBy > 0)
					{
						num2 = searchArgs.OrderBy;
						searchArgs.OrderBy = 0;
					}
					else
					{
						if (searchArgs.OrderTypeFlags <= 1)
						{
							IL_15F:
							if (null == list)
							{
								list = SaleManager.GetSaleGoodsDataList(searchArgs.Type, searchArgs.ID);
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), new List<SaleGoodsData>(list));
							}
							if (num3 > 0)
							{
								list = new List<SaleGoodsData>(list);
								list.Sort(SaleManager.GetComparerFor(true, true, (SearchOrderTypes)num3));
								searchArgs.OrderTypeFlags = num3;
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), list);
							}
							if (num2 > 0)
							{
								list = new List<SaleGoodsData>(list);
								searchArgs.OrderBy = num2;
								list.Reverse();
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), list);
							}
							if (moneyFlags > 0)
							{
								searchArgs.MoneyFlags = moneyFlags;
								list = new List<SaleGoodsData>(list);
								list.RemoveAll(delegate(SaleGoodsData x)
								{
									bool result;
									if (x.SalingGoodsData.SaleMoney1 > 0)
									{
										result = ((moneyFlags & 1) == 0);
									}
									else if (x.SalingGoodsData.SaleYuanBao > 0)
									{
										result = ((moneyFlags & 2) == 0);
									}
									else
									{
										result = (x.SalingGoodsData.SaleYinPiao <= 0 || (moneyFlags & 4) == 0);
									}
									return result;
								});
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), list);
							}
							if (num > 0)
							{
								list = new List<SaleGoodsData>(list);
								searchArgs.ColorFlags = num;
								list.RemoveAll(delegate(SaleGoodsData x)
								{
									int equipColor = Global.GetEquipColor(x.SalingGoodsData);
									return (1 << equipColor - 1 & searchArgs.ColorFlags) == 0;
								});
								SaleManager._OrderdSaleGoodsListDict.Add(searchArgs.Clone(), list);
							}
							return list;
						}
						num3 = searchArgs.OrderTypeFlags;
						searchArgs.OrderTypeFlags = 0;
					}
				}
				goto IL_15F;
			}
			return list;
		}

		public static int GetSaleMoneyType(GoodsData goodsData)
		{
			int result;
			if (goodsData.SaleMoney1 > 0)
			{
				result = 1;
			}
			else if (goodsData.SaleYuanBao > 0)
			{
				result = 2;
			}
			else
			{
				result = 4;
			}
			return result;
		}

		public static void UpdateCachedListForSaleGoodsData(SaleGoodsData saleGoodsData, int[] typeAndID, bool add)
		{
			lock (SaleManager.Mutex_SaleGoodsDict)
			{
				if (SaleManager._OrderdSaleGoodsListDict.Count != 0)
				{
					if (null != typeAndID)
					{
						List<SearchArgs> list = new List<SearchArgs>();
						int equipColor = Global.GetEquipColor(saleGoodsData.SalingGoodsData);
						int saleMoneyType = SaleManager.GetSaleMoneyType(saleGoodsData.SalingGoodsData);
						List<SearchArgs> list2 = SaleManager._OrderdSaleGoodsListDict.Keys.ToList<SearchArgs>();
						foreach (SearchArgs searchArgs in list2)
						{
							if ((searchArgs.ID == 0 && searchArgs.Type == typeAndID[0]) || searchArgs.ID == typeAndID[1] || searchArgs.Type == 1)
							{
								if ((1 << equipColor - 1 & searchArgs.ColorFlags) != 0)
								{
									if ((searchArgs.MoneyFlags & saleMoneyType) != 0)
									{
										SaleManager.UpdateOrderdList(SaleManager._OrderdSaleGoodsListDict[searchArgs], saleGoodsData, searchArgs.OrderBy == 0, add, (SearchOrderTypes)searchArgs.OrderTypeFlags);
									}
								}
							}
						}
					}
				}
			}
		}

		private static IComparer<SaleGoodsData> GetComparerFor(bool desc, bool add, SearchOrderTypes searchOrderType)
		{
			IComparer<SaleGoodsData> result;
			switch (searchOrderType)
			{
			case SearchOrderTypes.OrderByMoney:
				if (desc && !add)
				{
					result = SaleGoodsMoneyCompare2.Instance;
				}
				else
				{
					result = SaleGoodsMoneyCompare.Instance;
				}
				return result;
			case SearchOrderTypes.OrderByMoneyPerItem:
				if (desc && !add)
				{
					result = SaleGoodsMoneyPerItemCompare.DescInstance;
				}
				else
				{
					result = SaleGoodsMoneyPerItemCompare.AscInstance;
				}
				return result;
			case (SearchOrderTypes)3:
				break;
			case SearchOrderTypes.OrderBySuit:
				if (desc && !add)
				{
					result = SaleGoodsSuitCompare.DescInstance;
				}
				else
				{
					result = SaleGoodsSuitCompare.AscInstance;
				}
				return result;
			default:
				if (searchOrderType == SearchOrderTypes.OrderByNameAndColor)
				{
					if (desc && !add)
					{
						result = SaleGoodsNameAndColorCompare.DescInstance;
					}
					else
					{
						result = SaleGoodsNameAndColorCompare.AscInstance;
					}
					return result;
				}
				break;
			}
			result = SaleGoodsMoneyCompare.Instance;
			return result;
		}

		private static void UpdateOrderdList(List<SaleGoodsData> list, SaleGoodsData saleGoodsData, bool desc, bool add, SearchOrderTypes searchOrderType)
		{
			if (add)
			{
				if (desc)
				{
					ListExt.BinaryInsertDesc<SaleGoodsData>(list, saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
				}
				else
				{
					ListExt.BinaryInsertAsc<SaleGoodsData>(list, saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
				}
			}
			else
			{
				int num = list.BinarySearch(saleGoodsData, SaleManager.GetComparerFor(desc, add, searchOrderType));
				if (num < 0)
				{
					for (int i = 0; i < list.Count; i++)
					{
						if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
						{
							list.RemoveAt(i);
							break;
						}
					}
				}
				else
				{
					for (int i = num; i >= 0; i--)
					{
						if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
						{
							list.RemoveAt(i);
							return;
						}
					}
					for (int i = num + 1; i < list.Count; i++)
					{
						if (list[i].GoodsDbID == saleGoodsData.GoodsDbID)
						{
							list.RemoveAt(i);
							break;
						}
					}
				}
			}
		}

		private static void FixSearchArgs(SearchArgs searchArgs)
		{
			if (!SaleManager.IsValidType(searchArgs.Type, searchArgs.ID))
			{
				searchArgs.Type = 1;
				searchArgs.ID = 1;
			}
			int orderTypeFlags = searchArgs.OrderTypeFlags;
			for (int i = 0; i < 32; i++)
			{
				int num = 1 << i;
				if ((orderTypeFlags & num) != 0)
				{
					searchArgs.OrderTypeFlags &= num;
					if (orderTypeFlags > 8)
					{
					}
					break;
				}
			}
		}

		public static List<SaleGoodsData> GetSaleGoodsDataList(SearchArgs searchArgs, List<int> GoodsIds)
		{
			SaleManager.FixSearchArgs(searchArgs);
			List<SaleGoodsData> list = SaleManager.GetCachedSaleGoodsList(searchArgs);
			if (GoodsIds != null && GoodsIds.Count > 0)
			{
				list = list.FindAll((SaleGoodsData x) => GoodsIds.Contains(x.SalingGoodsData.GoodsID));
			}
			return list;
		}

		public const int ConstAllColorFlags = 63;

		public const int ConstAllMoneyFlags = 7;

		private static SaleManager instance = new SaleManager();

		private static object Mutex_SaleGoodsDict = new object();

		private static Dictionary<int, SaleGoodsData> _SaleGoodsDict = new Dictionary<int, SaleGoodsData>();

		private static Dictionary<int, Dictionary<int, List<SaleGoodsData>>> _SaleGoodsDict2 = new Dictionary<int, Dictionary<int, List<SaleGoodsData>>>();

		private static Dictionary<SearchArgs, List<SaleGoodsData>> _OrderdSaleGoodsListDict = new Dictionary<SearchArgs, List<SaleGoodsData>>(SearchArgs.Compare);

		private static Dictionary<int, int[]> _GoodsID2TabIDDict = new Dictionary<int, int[]>();

		private static Dictionary<long, int[]> _Categoriy2TabIDDict = new Dictionary<long, int[]>();

		private static HashSet<int> _TypeHashSet = new HashSet<int>();

		private static HashSet<int> _IDHashSet = new HashSet<int>();

		private static int[] OthersGoodsTypeAndID = null;

		public static long MaxSaleGoodsTime = 43200000L;

		private static Dictionary<string, Dictionary<int, int>> _SearchText2GoodsIDDict = new Dictionary<string, Dictionary<int, int>>();

		private static Dictionary<string, List<int>> _SearchText2GoodsIDDict2 = new Dictionary<string, List<int>>();
	}
}
