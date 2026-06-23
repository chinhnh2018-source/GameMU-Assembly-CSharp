using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Server.Data;
using Server.Tools;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class GoodsUtil
	{
		public static void LoadConfig()
		{
			GoodsUtil.LoadGetGoodsXml();
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("HorseEquipBarOpen", ',');
			int[] paramValueIntArrayByName2 = GameManager.systemParamsList.GetParamValueIntArrayByName("HorseEquipMeltingOpen", ',');
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				for (int i = 40; i <= 45; i++)
				{
					GoodsTypeInfo goodsTypeInfo;
					if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(i, out goodsTypeInfo))
					{
						goodsTypeInfo.Categriory = 40;
						goodsTypeInfo.GoodsType = 2;
						int num = i - 40;
						if (paramValueIntArrayByName != null && num < paramValueIntArrayByName.Length && paramValueIntArrayByName[num] == 1)
						{
							goodsTypeInfo.IsEquip = true;
						}
						for (int j = 0; j < 15; j++)
						{
							if (paramValueIntArrayByName2 != null && j < paramValueIntArrayByName2.Length && paramValueIntArrayByName2[j] == 1)
							{
								goodsTypeInfo.Operations[j] = true;
								if (j == 10)
								{
									goodsTypeInfo.OperationsTypeList[j] = new List<int>();
									for (int k = 40; k <= 45; k++)
									{
										goodsTypeInfo.OperationsTypeList[j].Add(k);
									}
								}
								else
								{
									goodsTypeInfo.OperationsTypeList[j] = new List<int>
									{
										i
									};
								}
							}
						}
					}
				}
				for (int i = 11; i <= 21; i++)
				{
					GoodsTypeInfo goodsTypeInfo;
					if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(i, out goodsTypeInfo))
					{
						goodsTypeInfo.Categriory = i;
						goodsTypeInfo.GoodsType = 1;
						goodsTypeInfo.IsEquip = true;
						for (int j = 0; j < 15; j++)
						{
							goodsTypeInfo.Operations[j] = true;
							goodsTypeInfo.OperationsTypeList[j] = new List<int>();
							for (int k = 11; k <= 21; k++)
							{
								goodsTypeInfo.OperationsTypeList[j].Add(k);
							}
							if (j == 10)
							{
								for (int k = 0; k <= 6; k++)
								{
									goodsTypeInfo.OperationsTypeList[j].Add(k);
								}
							}
						}
					}
				}
				for (int i = 0; i <= 6; i++)
				{
					GoodsTypeInfo goodsTypeInfo;
					if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(i, out goodsTypeInfo))
					{
						goodsTypeInfo.Categriory = i;
						goodsTypeInfo.GoodsType = 1;
						goodsTypeInfo.IsEquip = true;
						for (int j = 0; j < 15; j++)
						{
							goodsTypeInfo.Operations[j] = true;
							if (j == 10)
							{
								goodsTypeInfo.OperationsTypeList[j] = new List<int>();
								for (int k = 0; k <= 6; k++)
								{
									goodsTypeInfo.OperationsTypeList[j].Add(k);
								}
								for (int k = 11; k <= 21; k++)
								{
									goodsTypeInfo.OperationsTypeList[j].Add(k);
								}
							}
							else
							{
								goodsTypeInfo.OperationsTypeList[j] = new List<int>
								{
									i
								};
							}
						}
					}
				}
				for (int i = 37; i <= 38; i++)
				{
					GoodsTypeInfo goodsTypeInfo;
					if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(i, out goodsTypeInfo))
					{
						goodsTypeInfo.Categriory = i;
						goodsTypeInfo.GoodsType = 1;
						goodsTypeInfo.IsEquip = true;
						for (int j = 0; j < 15; j++)
						{
							goodsTypeInfo.Operations[j] = true;
							goodsTypeInfo.OperationsTypeList[j] = new List<int>();
							for (int k = 37; k <= 38; k++)
							{
								goodsTypeInfo.OperationsTypeList[j].Add(k);
							}
						}
					}
				}
				for (int i = 30; i <= 36; i++)
				{
					GoodsTypeInfo goodsTypeInfo;
					if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(i, out goodsTypeInfo))
					{
						goodsTypeInfo.Categriory = i;
						goodsTypeInfo.GoodsType = 1;
						goodsTypeInfo.IsEquip = true;
						for (int j = 100; j < 101; j++)
						{
							goodsTypeInfo.Operations[j] = true;
						}
					}
				}
			}
		}

		public static void LoadGetGoodsXml()
		{
			string text = Global.GameResPath("Config/GetGoods.xml");
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			try
			{
				XElement xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					IEnumerable<XElement> xelements = ConfigHelper.GetXElements(xelement, "GetGoods");
					if (null != xelements)
					{
						foreach (XElement xelement2 in xelements)
						{
							int key = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "ID", 0L);
							int value = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Goods", 0L);
							dictionary[key] = value;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
			GoodsUtil.GetGoodsIdDict = dictionary;
		}

		public static bool CanEquip(int type, int site)
		{
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(type, out goodsTypeInfo) && goodsTypeInfo.UsingSite == site)
				{
					return goodsTypeInfo.IsEquip;
				}
			}
			return false;
		}

		public static bool IsEquip(int goodsId)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsId);
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(goodsCatetoriy, out goodsTypeInfo))
				{
					return goodsTypeInfo.IsEquip;
				}
			}
			return false;
		}

		public static GoodsTypeInfo GetGoodsTypeInfo(int type)
		{
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo result;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(type, out result))
				{
					return result;
				}
			}
			return GoodsTypeInfo.Empty;
		}

		public static GoodsTypeInfo GetGoodsTypeInfoByGoodsId(int goodsId)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsId);
			return GoodsUtil.GetGoodsTypeInfo(goodsCatetoriy);
		}

		public static bool CanUpgrade(int type, int op)
		{
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(type, out goodsTypeInfo))
				{
					return goodsTypeInfo.Operations[op];
				}
			}
			return false;
		}

		public static int CanUpgradeInhert(int type1, int type2, int op)
		{
			int result = -200;
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				GoodsTypeInfo goodsTypeInfo2;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(type1, out goodsTypeInfo) && GoodsUtil.GoodsTypeInfoDict.TryGetValue(type2, out goodsTypeInfo2))
				{
					if (goodsTypeInfo.Operations[op] && goodsTypeInfo2.Operations[op])
					{
						if (goodsTypeInfo.OperationsTypeList[op] == null || goodsTypeInfo.OperationsTypeList[op].Contains(type2))
						{
							return 0;
						}
						result = -201;
					}
				}
			}
			return result;
		}

		public static bool IsZuoQiEquip(int goodsId)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsId);
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(goodsCatetoriy, out goodsTypeInfo))
				{
					return goodsTypeInfo.GoodsType == 2;
				}
			}
			return false;
		}

		public static bool IsVisiableEquip(int goodsId)
		{
			int goodsCatetoriy = Global.GetGoodsCatetoriy(goodsId);
			lock (GoodsUtil.GoodsTypeInfoDict)
			{
				GoodsTypeInfo goodsTypeInfo;
				if (GoodsUtil.GoodsTypeInfoDict.TryGetValue(goodsCatetoriy, out goodsTypeInfo))
				{
					return goodsTypeInfo.GoodsType == 1;
				}
			}
			return true;
		}

		public static int GetResGoodsID(MoneyTypes type)
		{
			Dictionary<int, int> getGoodsIdDict = GoodsUtil.GetGoodsIdDict;
			if (null != getGoodsIdDict)
			{
				lock (getGoodsIdDict)
				{
					int result;
					if (getGoodsIdDict.TryGetValue((int)type, out result))
					{
						return result;
					}
				}
			}
			return 0;
		}

		public static string FormatUpdateDBGoodsStr(params object[] args)
		{
			string result;
			if (args.Length != 24)
			{
				LogManager.WriteLog(2, string.Format("FormatUpdateDBGoodsStr, 参数个数不对{0}", args.Length), null, true);
				result = null;
			}
			else
			{
				result = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}:{11}:{12}:{13}:{14}:{15}:{16}:{17}:{18}:{19}:{20}:{21}:{22}:{23}", args);
			}
			return result;
		}

		private static Dictionary<int, int> logItemDict
		{
			get
			{
				Dictionary<int, int> logItemDict_storage;
				lock (GoodsUtil._logItemLock)
				{
					logItemDict_storage = GoodsUtil._logItemDict_storage;
				}
				return logItemDict_storage;
			}
			set
			{
				lock (GoodsUtil._logItemLock)
				{
					GoodsUtil._logItemDict_storage = value;
				}
			}
		}

		public static void LoadItemLogMark()
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LogGoods", ',');
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
			{
				for (int i = 0; i < paramValueIntArrayByName.Length; i++)
				{
					dictionary.Add(paramValueIntArrayByName[i], 1);
				}
			}
			GoodsUtil.logItemDict = dictionary;
		}

		public static string ModifyGoodsLogName(GoodsData goodsData)
		{
			return Global.ModifyGoodsLogName(goodsData);
		}

		public static bool CheckHasGoodsList(GameClient client, List<List<int>> needGoods, bool notBind)
		{
			for (int i = 0; i < needGoods.Count; i++)
			{
				if (needGoods[i].Count >= 2)
				{
					int goodsID = needGoods[i][0];
					int num = needGoods[i][1];
					int num2 = notBind ? Global.GetTotalNotBindGoodsCountByID(client, goodsID) : Global.GetTotalGoodsCountByID(client, goodsID);
					if (num2 < num)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static bool CostGoodsList(GameClient client, List<List<int>> needGoods, bool notBind, ref string strCostList, string logMsg)
		{
			bool result = true;
			bool flag = false;
			bool flag2 = false;
			for (int i = 0; i < needGoods.Count; i++)
			{
				if (needGoods[i].Count >= 2)
				{
					int num = needGoods[i][0];
					int num2 = needGoods[i][1];
					if (notBind)
					{
						if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num2, false, out flag, out flag2, false))
						{
							LogManager.WriteLog(2, string.Format("{0},消耗{1}个GoodsID={2}的非绑定物品失败", logMsg, num2, num), null, true);
							result = false;
							goto IL_15A;
						}
					}
					else if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, num2, false, out flag, out flag2, false))
					{
						LogManager.WriteLog(2, string.Format("{0},消耗{1}个GoodsID={2}的物品失败", logMsg, num2, num), null, true);
						result = false;
						goto IL_15A;
					}
					if (strCostList != null)
					{
						GoodsData goodsData = new GoodsData
						{
							GoodsID = num,
							GCount = num2
						};
						strCostList += EventLogManager.NewGoodsDataPropString(goodsData);
					}
				}
				IL_15A:;
			}
			return result;
		}

		public static GoodsData GetGoodsDataBySite(GameClient client, int id, int site)
		{
			if (site == 1)
			{
				if (null == client.ClientData.MeditateGoodsDataList)
				{
					return null;
				}
				lock (client.ClientData.MeditateGoodsDataList)
				{
					return client.ClientData.MeditateGoodsDataList.FirstOrDefault((GoodsData x) => x.Id == id);
				}
			}
			return null;
		}

		public static bool RemoveGoodsDataBySite(GameClient client, GoodsData goodsData, int site)
		{
			if (site != 1)
			{
				if (site == 12000)
				{
					return ZuoQiManager.RemoveStoreGoodsData(client, goodsData);
				}
				if (site != 13000)
				{
					return false;
				}
			}
			else
			{
				if (null == client.ClientData.MeditateGoodsDataList)
				{
					return false;
				}
				lock (client.ClientData.MeditateGoodsDataList)
				{
					return client.ClientData.MeditateGoodsDataList.Remove(goodsData);
				}
			}
			return ZuoQiManager.RemoveEquipGoodsData(client, goodsData);
		}

		public static bool DestoryGoodsBySystem(GameClient client, GoodsData goodsData)
		{
			string cmdData;
			if (goodsData.Using > 0)
			{
				cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
				{
					client.ClientData.RoleID,
					2,
					goodsData.Id,
					goodsData.GoodsID,
					0,
					goodsData.Site,
					goodsData.GCount,
					goodsData.BagIndex,
					""
				});
				Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
			}
			cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				6,
				goodsData.Id,
				goodsData.GoodsID,
				0,
				goodsData.Site,
				goodsData.GCount,
				goodsData.BagIndex,
				""
			});
			Global.ModifyGoodsByCmdParams(client, cmdData, "客户端修改", null);
			return true;
		}

		public static int AddGoodsDBCommand(GameClient client, GoodsData goodsData, bool useOldGrid, int newHint, string goodsFromWhere, bool onLine = true)
		{
			int num = 0;
			int gcount = goodsData.GCount;
			int goodsID = goodsData.GoodsID;
			int num2 = Global.GetGoodsGridNumByID(goodsID);
			num2 = Global.GMax(num2, 1);
			int result;
			if (gcount <= 0)
			{
				result = 0;
			}
			else
			{
				int num3 = (gcount - 1) / num2 + 1;
				for (int i = 0; i < num3; i++)
				{
					int goodsNum = num2;
					if (i >= num3 - 1 && gcount % num2 > 0)
					{
						goodsNum = gcount % num2;
					}
					num = Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsID, goodsNum, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, goodsData.Site, goodsData.Jewellist, useOldGrid, newHint, goodsFromWhere, false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, false, goodsData.WashProps, goodsData.ElementhrtsProps, "1900-01-01 12:00:00", goodsData.JuHunID, onLine);
					if (num < 0)
					{
						return num;
					}
				}
				result = num;
			}
			return result;
		}

		public static GoodsData AddGoodsDataToBag(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string startTime, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife, int bagIndex = 0, List<int> washProps = null)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = startTime,
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = 1,
				Jewellist = jewelList,
				BagIndex = bagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife,
				WashProps = washProps
			};
			if (site == 1)
			{
				if (null == client.ClientData.MeditateGoodsDataList)
				{
					client.ClientData.MeditateGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.MeditateGoodsDataList)
				{
					client.ClientData.MeditateGoodsDataList.Add(goodsData);
				}
			}
			return goodsData;
		}

		public static int GetIdleSlotOfBag(GameClient client, int site)
		{
			int num = 0;
			List<GoodsData> list = null;
			if (site == 1)
			{
				list = client.ClientData.FuWenGoodsDataList;
			}
			int result;
			if (null == list)
			{
				result = num;
			}
			else
			{
				List<int> list2 = new List<int>();
				for (int i = 0; i < list.Count; i++)
				{
					if (list2.IndexOf(list[i].BagIndex) < 0)
					{
						list2.Add(list[i].BagIndex);
					}
				}
				int maxBagCount = GoodsUtil.GetMaxBagCount(site);
				for (int j = 0; j < maxBagCount; j++)
				{
					if (list2.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}

		public static bool CanAddGoodsNum(GameClient client, int num, int site)
		{
			return client != null && num > 0;
		}

		public static int GetMaxBagCount(int site)
		{
			return 1000;
		}

		public static GoodsData GetResGoodsData(MoneyTypes type, int gcount)
		{
			GoodsData result;
			if (gcount <= 0)
			{
				result = null;
			}
			else
			{
				int resGoodsID = GoodsUtil.GetResGoodsID(type);
				if (resGoodsID == 0)
				{
					result = null;
				}
				else
				{
					GoodsData goodsData = new GoodsData
					{
						GoodsID = resGoodsID,
						GCount = gcount
					};
					result = goodsData;
				}
			}
			return result;
		}

		public static List<GoodsData> GetGoodsListBySiteFromDB(GameClient client, int site)
		{
			List<GoodsData> result;
			if (null == client)
			{
				result = null;
			}
			else if (!Array.Exists<SaleGoodsConsts>(typeof(SaleGoodsConsts).GetEnumValues() as SaleGoodsConsts[], (SaleGoodsConsts _p) => _p == (SaleGoodsConsts)site))
			{
				result = null;
			}
			else
			{
				string cmd = StringUtil.substitute("{0}:{1}", new object[]
				{
					client.ClientData.RoleID,
					site
				});
				result = Global.sendToDB<List<GoodsData>, string>(204, cmd, client.ServerId);
			}
			return result;
		}

		public static int GetMeditateBagGoodsCnt(GameClient client)
		{
			if (null == client.ClientData.MeditateGoodsDataList)
			{
				client.ClientData.MeditateGoodsDataList = GoodsUtil.GetGoodsListBySiteFromDB(client, 1);
			}
			if (null == client.ClientData.MeditateGoodsDataList)
			{
				client.ClientData.MeditateGoodsDataList = new List<GoodsData>();
			}
			return client.ClientData.MeditateGoodsDataList.Count;
		}

		public static void ProcessMeditateGoods(GameClient client)
		{
			int mingXiangGoodsInterval = Global.GetMingXiangGoodsInterval(client);
			int num = GoodsUtil.GetMeditateBagGoodsCnt(client);
			int meditateTime = client.ClientData.MeditateTime;
			int num2 = Global.GMax(0, meditateTime - num * mingXiangGoodsInterval);
			if (num2 >= mingXiangGoodsInterval)
			{
				int num3 = num2 / mingXiangGoodsInterval;
				int num4 = 0;
				for (int i = 0; i < num3; i++)
				{
					if (num + 1 > Data.OfflineRW_ItemLimit)
					{
						LogManager.WriteLog(0, string.Format("角色冥想背包超过{2}了,角色ID = {0} ，角色roleid = {1}", client.strUserID, client.ClientData.RoleID, Data.OfflineRW_ItemLimit), null, true);
						break;
					}
					if (null != GoodsUtil.GiveOneMeditateGood(client))
					{
						num++;
						num4++;
					}
				}
				LogManager.WriteLog(0, string.Format("玩家登陆的时候,冥想背包物品添加{3}个,现有{2}个,角色ID = {0} ，角色roleid = {1}", new object[]
				{
					client.strUserID,
					client.ClientData.RoleID,
					num,
					num4
				}), null, true);
			}
		}

		public static long GetLastGiveMeditateTime(GameClient client)
		{
			return (long)(GoodsUtil.GetMeditateBagGoodsCnt(client) * Global.GetMingXiangGoodsInterval(client));
		}

		public static GoodsData GiveOneMeditateGood(GameClient client)
		{
			int mingXiangPackageID = Global.GetMingXiangPackageID(client);
			GoodsData result;
			if (0 == mingXiangPackageID)
			{
				result = null;
			}
			else
			{
				List<GoodsData> list = GoodsBaoXiang.FetchGoodListBaseFallPacketID(client, mingXiangPackageID, 1, FallAlgorithm.BaoXiang);
				if (list == null || list.Count == 0)
				{
					result = null;
				}
				else
				{
					GoodsData goodsData = list[0];
					goodsData.Site = 1;
					int id = GoodsUtil.AddGoodsDBCommand(client, goodsData, false, 0, "冥想", true);
					goodsData.Id = id;
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "MeditateTime");
					EventLogManager.AddRoleMeditateEvent(client, (long)(roleParamsInt32FromDB / 1000), GoodsUtil.GetMeditateBagGoodsCnt(client), Global.NewGoodsDataPropString(goodsData));
					result = goodsData;
				}
			}
			return result;
		}

		public static bool MoveGoodsSite(GameClient client, GoodsData goodsData, int newSite)
		{
			throw new NotImplementedException();
		}

		private static Dictionary<int, GoodsTypeInfo> GoodsTypeInfoDict = new Dictionary<int, GoodsTypeInfo>
		{
			{
				0,
				new GoodsTypeInfo()
			},
			{
				1,
				new GoodsTypeInfo()
			},
			{
				2,
				new GoodsTypeInfo()
			},
			{
				3,
				new GoodsTypeInfo()
			},
			{
				4,
				new GoodsTypeInfo()
			},
			{
				5,
				new GoodsTypeInfo()
			},
			{
				6,
				new GoodsTypeInfo()
			},
			{
				11,
				new GoodsTypeInfo()
			},
			{
				12,
				new GoodsTypeInfo()
			},
			{
				13,
				new GoodsTypeInfo()
			},
			{
				14,
				new GoodsTypeInfo()
			},
			{
				15,
				new GoodsTypeInfo()
			},
			{
				16,
				new GoodsTypeInfo()
			},
			{
				17,
				new GoodsTypeInfo()
			},
			{
				18,
				new GoodsTypeInfo()
			},
			{
				19,
				new GoodsTypeInfo()
			},
			{
				20,
				new GoodsTypeInfo()
			},
			{
				21,
				new GoodsTypeInfo()
			},
			{
				24,
				new GoodsTypeInfo
				{
					FashionGoods = true,
					GoodsType = 3,
					UsingSite = 6000
				}
			},
			{
				25,
				new GoodsTypeInfo
				{
					FashionGoods = true,
					GoodsType = 3,
					UsingSite = 6000
				}
			},
			{
				26,
				new GoodsTypeInfo
				{
					FashionGoods = true,
					GoodsType = 3,
					UsingSite = 6000
				}
			},
			{
				27,
				new GoodsTypeInfo
				{
					FashionGoods = true,
					GoodsType = 3,
					UsingSite = 6000
				}
			},
			{
				28,
				new GoodsTypeInfo
				{
					FashionGoods = true,
					GoodsType = 3,
					UsingSite = 6000
				}
			},
			{
				40,
				new GoodsTypeInfo()
			},
			{
				41,
				new GoodsTypeInfo()
			},
			{
				42,
				new GoodsTypeInfo()
			},
			{
				43,
				new GoodsTypeInfo()
			},
			{
				44,
				new GoodsTypeInfo()
			},
			{
				45,
				new GoodsTypeInfo()
			},
			{
				30,
				new GoodsTypeInfo()
			},
			{
				31,
				new GoodsTypeInfo()
			},
			{
				32,
				new GoodsTypeInfo()
			},
			{
				33,
				new GoodsTypeInfo()
			},
			{
				34,
				new GoodsTypeInfo()
			},
			{
				35,
				new GoodsTypeInfo()
			},
			{
				36,
				new GoodsTypeInfo()
			},
			{
				37,
				new GoodsTypeInfo()
			},
			{
				38,
				new GoodsTypeInfo()
			}
		};

		public static Dictionary<int, int> GetGoodsIdDict = new Dictionary<int, int>();

		private static object _logItemLock = new object();

		private static Dictionary<int, int> _logItemDict_storage = null;
	}
}
