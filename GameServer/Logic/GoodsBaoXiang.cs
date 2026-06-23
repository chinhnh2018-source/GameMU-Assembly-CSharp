using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class GoodsBaoXiang
	{
		public static void ProcessFallBaoXiang_StepOne(GameClient client, SystemXmlItem systemGoodsItem, GoodsData goodsData, ref int UseNum)
		{
			List<MagicActionItem> list = null;
			if (GameManager.SystemMagicActionMgr.GoodsActionsDict.TryGetValue(goodsData.GoodsID, out list) && null != list)
			{
				if (list.Count > 0)
				{
					MagicActionItem magicActionItem = list[0];
					if (MagicActionIDs.FALL_BAOXIANG == magicActionItem.MagicActionID || MagicActionIDs.FALL_BAOXIANG_2 == magicActionItem.MagicActionID)
					{
						int num = (int)magicActionItem.MagicActionParams[0];
						int maxFallCount = (int)magicActionItem.MagicActionParams[1];
						int num2 = 0;
						if (MagicActionIDs.FALL_BAOXIANG_2 == magicActionItem.MagicActionID)
						{
							num2 = (int)magicActionItem.MagicActionParams[2];
						}
						List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(num);
						if (null != fallGoodsItemList)
						{
							List<GoodsData> list2 = new List<GoodsData>();
							int i;
							for (i = 0; i < UseNum; i++)
							{
								List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 1, 1.0);
								if (fallGoodsItemByPercent.Count > 0)
								{
									List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
									if (null != goodsDataListFromFallGoodsItemList)
									{
										bool flag = true;
										foreach (GoodsData goodsData2 in goodsDataListFromFallGoodsItemList)
										{
											SystemXmlItem systemXmlItem = null;
											if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData2.GoodsID, out systemXmlItem))
											{
												LogManager.WriteLog(2, string.Format("掉落包配置出错 fallID={0} goodsID={1}", num, goodsData2.GoodsID), null, true);
												flag = false;
												break;
											}
											int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
											int siteByCategoriy = Global.GetSiteByCategoriy(intValue);
											if (siteByCategoriy != num2 && num2 != 15000)
											{
												LogManager.WriteLog(2, string.Format("掉落包配置出错 fallID={0} goodsID={1}", num, goodsData2.GoodsID), null, true);
												flag = false;
												break;
											}
											goodsData2.Site = siteByCategoriy;
											goodsData2.Binding = goodsData.Binding;
										}
										if (!flag)
										{
											break;
										}
										using (List<GoodsData>.Enumerator enumerator = goodsDataListFromFallGoodsItemList.GetEnumerator())
										{
											while (enumerator.MoveNext())
											{
												GoodsData item = enumerator.Current;
												GoodsData goodsData3 = list2.Find((GoodsData x) => x.GoodsID == item.GoodsID && x.ExcellenceInfo == item.ExcellenceInfo && x.GCount + item.GCount <= Global.GetGoodsGridNumByID(x.GoodsID));
												if (null == goodsData3)
												{
													list2.Add(item);
												}
												else
												{
													goodsData3.GCount += item.GCount;
												}
											}
										}
										flag = Global.CanAddGoodsNum(client, num2, list2.Count);
										if (!flag)
										{
											using (List<GoodsData>.Enumerator enumerator = goodsDataListFromFallGoodsItemList.GetEnumerator())
											{
												while (enumerator.MoveNext())
												{
													GoodsData item = enumerator.Current;
													GoodsData goodsData3 = list2.Find((GoodsData x) => x.GoodsID == item.GoodsID);
													if (null != goodsData3)
													{
														goodsData3.GCount -= item.GCount;
														if (goodsData3.GCount <= 0)
														{
															list2.Remove(goodsData3);
														}
													}
												}
											}
											break;
										}
									}
								}
							}
							UseNum = i;
							client.ClientData.FallBaoXiangGoodsList = list2;
						}
					}
				}
			}
		}

		public static void ProcessFallBaoXiang_StepTwo(GameClient client, int fallID, int maxFallCount, int binding, int actionGoodsID)
		{
			List<GoodsData> fallBaoXiangGoodsList = client.ClientData.FallBaoXiangGoodsList;
			if (null != fallBaoXiangGoodsList)
			{
				for (int i = 0; i < fallBaoXiangGoodsList.Count; i++)
				{
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(fallBaoXiangGoodsList[i].GoodsID, out systemXmlItem))
					{
						int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
						binding = Global.GetBindingByCategoriy(intValue, fallBaoXiangGoodsList[i].Binding);
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, fallBaoXiangGoodsList[i].GoodsID, fallBaoXiangGoodsList[i].GCount, fallBaoXiangGoodsList[i].Quality, fallBaoXiangGoodsList[i].Props, fallBaoXiangGoodsList[i].Forge_level, binding, fallBaoXiangGoodsList[i].Site, "", true, 1, "掉落宝箱获取", fallBaoXiangGoodsList[i].Endtime, fallBaoXiangGoodsList[i].AddPropIndex, fallBaoXiangGoodsList[i].BornIndex, fallBaoXiangGoodsList[i].Lucky, fallBaoXiangGoodsList[i].Strong, fallBaoXiangGoodsList[i].ExcellenceInfo, fallBaoXiangGoodsList[i].AppendPropLev, fallBaoXiangGoodsList[i].ChangeLifeLevForEquip, null, null, 0, true);
						Global.BroadcastFallBaoXiangGoodsHint(client, fallBaoXiangGoodsList[i], actionGoodsID);
						Global.BroadSelfFallBoxGoods(client, fallBaoXiangGoodsList[i]);
						if (client.ClientData.RoleAwardMsgType == RoleAwardMsg.RandomBaoXiang)
						{
							client.ClientData.AddAwardRecord(RoleAwardMsg.RandomBaoXiang, new GoodsData(fallBaoXiangGoodsList[i]), false);
						}
					}
				}
				client.ClientData.FallBaoXiangGoodsList = null;
			}
		}

		public static void ProcessFallBaoXiang(GameClient client, int fallID, int maxFallCount, int binding, int actionGoodsID)
		{
			if (fallID > 0)
			{
				List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null != fallGoodsItemList)
				{
					List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 1, 1.0);
					if (fallGoodsItemByPercent.Count > 0)
					{
						List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						if (Global.CanAddGoodsNum(client, goodsDataListFromFallGoodsItemList.Count))
						{
							for (int i = 0; i < goodsDataListFromFallGoodsItemList.Count; i++)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsDataListFromFallGoodsItemList[i].GoodsID, goodsDataListFromFallGoodsItemList[i].GCount, goodsDataListFromFallGoodsItemList[i].Quality, goodsDataListFromFallGoodsItemList[i].Props, goodsDataListFromFallGoodsItemList[i].Forge_level, binding, 0, "", true, 1, "掉落宝箱获取", goodsDataListFromFallGoodsItemList[i].Endtime, goodsDataListFromFallGoodsItemList[i].AddPropIndex, goodsDataListFromFallGoodsItemList[i].BornIndex, goodsDataListFromFallGoodsItemList[i].Lucky, goodsDataListFromFallGoodsItemList[i].Strong, goodsDataListFromFallGoodsItemList[i].ExcellenceInfo, goodsDataListFromFallGoodsItemList[i].AppendPropLev, goodsDataListFromFallGoodsItemList[i].ChangeLifeLevForEquip, null, null, 0, true);
								Global.BroadcastFallBaoXiangGoodsHint(client, goodsDataListFromFallGoodsItemList[i], actionGoodsID);
								if (client.ClientData.RoleAwardMsgType == RoleAwardMsg.RandomBaoXiang)
								{
									client.ClientData.AddAwardRecord(RoleAwardMsg.RandomBaoXiang, new GoodsData(goodsDataListFromFallGoodsItemList[i]), false);
								}
							}
						}
					}
				}
			}
		}

		public static List<GoodsData> FetchGoodListBaseFallPacketID(GameClient client, int fallID, int maxFallCount, FallAlgorithm fallAlgorithm)
		{
			List<GoodsData> list = null;
			List<GoodsData> result;
			if (fallID <= 0)
			{
				result = null;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == fallGoodsItemList)
				{
					result = null;
				}
				else
				{
					List<FallGoodsItem> fixedFallGoodsItemList = GameManager.GoodsPackMgr.GetFixedFallGoodsItemList(fallID);
					List<GoodsData> fixedGoodsDataList = GameManager.GoodsPackMgr.GetFixedGoodsDataList(fixedFallGoodsItemList, GameManager.GoodsPackMgr.GetFixedFallGoodsMaxCount(fallID));
					if (null != fixedGoodsDataList)
					{
						list = fixedGoodsDataList;
					}
					List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, (int)fallAlgorithm, 1.0);
					if (fallGoodsItemByPercent != null && fallGoodsItemByPercent.Count > 0)
					{
						List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						if (null != goodsDataListFromFallGoodsItemList)
						{
							if (null == list)
							{
								list = goodsDataListFromFallGoodsItemList;
							}
							else
							{
								list.AddRange(goodsDataListFromFallGoodsItemList);
							}
						}
					}
					result = list;
				}
			}
			return result;
		}

		public static int ProcessFallByYaoShiWaBao(GameClient client, int fallID, int idYaoShi, int idXiangZi, out GoodsData retGoodsData, int forceBinding, int subMoney)
		{
			retGoodsData = null;
			int result;
			if (fallID <= 0)
			{
				result = -3000;
			}
			else
			{
				int maxFallCount = 1;
				List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == fallGoodsItemList)
				{
					result = -3100;
				}
				else
				{
					List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 1, 1.0);
					if (fallGoodsItemByPercent.Count <= 0)
					{
						result = -3200;
					}
					else
					{
						List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						if (!Global.CanAddGoodsNum(client, goodsDataListFromFallGoodsItemList.Count))
						{
							result = -3300;
						}
						else if (1 == goodsDataListFromFallGoodsItemList.Count)
						{
							retGoodsData = goodsDataListFromFallGoodsItemList[0];
							bool flag = false;
							bool flag2 = false;
							bool flag3 = false;
							if (idXiangZi >= 0 && !GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, idXiangZi, 1, false, out flag, out flag2, false))
							{
								result = -400;
							}
							else if (idYaoShi >= 0 && !GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, idYaoShi, 1, false, out flag, out flag2, false))
							{
								result = -500;
							}
							else
							{
								if (!flag3)
								{
									flag3 = flag;
								}
								if (subMoney > 0)
								{
									flag3 = false;
								}
								retGoodsData.Binding = Math.Max(forceBinding, flag3 ? 1 : 0);
								int id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, retGoodsData.GoodsID, retGoodsData.GCount, retGoodsData.Quality, retGoodsData.Props, retGoodsData.Forge_level, retGoodsData.Binding, 0, "", true, 1, "精雕细琢挖宝获取", retGoodsData.Endtime, retGoodsData.AddPropIndex, retGoodsData.BornIndex, retGoodsData.Lucky, retGoodsData.Strong, 0, 0, 0, null, null, 0, true);
								retGoodsData.Id = id;
								result = 100;
							}
						}
						else
						{
							result = -3400;
						}
					}
				}
			}
			return result;
		}

		public static void CreateGoodsBaseFallID(GameClient client, int fallID, int nMaxCount)
		{
			if (fallID > 0)
			{
				List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null != fallGoodsItemList)
				{
					List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, nMaxCount, 1, 1.0);
					if (fallGoodsItemByPercent.Count > 0)
					{
						List<GoodsData> list = null;
						list = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						if (list != null)
						{
							List<GoodsPackItem> list2 = new List<GoodsPackItem>();
							Dictionary<string, bool> dict = new Dictionary<string, bool>();
							for (int i = 0; i < list.Count; i++)
							{
								List<GoodsData> list3 = new List<GoodsData>();
								list3.Add(list[i]);
								GoodsPackItem goodsPackItem = new GoodsPackItem
								{
									AutoID = GameManager.GoodsPackMgr.GetNextAutoID(),
									GoodsPackID = fallID,
									OwnerRoleID = client.ClientData.RoleID,
									OwnerRoleName = client.ClientData.RoleName,
									GoodsPackType = 0,
									ProduceTicks = TimeUtil.NOW(),
									LockedRoleID = -1,
									GoodsDataList = list3,
									TeamRoleIDs = null,
									MapCode = client.ClientData.MapCode,
									CopyMapID = client.ClientData.CopyMapID,
									KilledMonsterName = null,
									BelongTo = 1,
									FallLevel = 0,
									TeamID = -1
								};
								goodsPackItem.FallPoint = GameManager.GoodsPackMgr.GetFallGoodsPosition(ObjectTypes.OT_GOODSPACK, client.ClientData.MapCode, dict, new Point((double)((int)client.CurrentGrid.X), (double)((int)client.CurrentGrid.Y)), client.ClientData.CopyMapID, client);
								list2.Add(goodsPackItem);
								lock (GameManager.GoodsPackMgr.GoodsPackDict)
								{
									GameManager.GoodsPackMgr.GoodsPackDict[goodsPackItem.AutoID] = goodsPackItem;
								}
								for (int j = 0; j < list2.Count; j++)
								{
									GameManager.GoodsPackMgr.ProcessGoodsPackItem(client, client, list2[i], 1);
								}
							}
						}
					}
				}
			}
		}

		public static int ProcessActivityAward(GameClient client, int fallID, int maxFallCount, int binding, string sMsg, List<GoodsData> goodsDataList)
		{
			int result;
			if (fallID <= 0)
			{
				result = -10;
			}
			else
			{
				List<FallGoodsItem> fallGoodsItemList = GameManager.GoodsPackMgr.GetFallGoodsItemList(fallID);
				if (null == fallGoodsItemList)
				{
					result = -12;
				}
				else
				{
					List<FallGoodsItem> fallGoodsItemByPercent = GameManager.GoodsPackMgr.GetFallGoodsItemByPercent(fallGoodsItemList, maxFallCount, 1, 1.0);
					if (fallGoodsItemByPercent.Count <= 0)
					{
						result = -13;
					}
					else
					{
						List<GoodsData> goodsDataListFromFallGoodsItemList = GameManager.GoodsPackMgr.GetGoodsDataListFromFallGoodsItemList(fallGoodsItemByPercent);
						if (!Global.CanAddGoodsNum(client, goodsDataListFromFallGoodsItemList.Count))
						{
							result = -14;
						}
						else
						{
							for (int i = 0; i < goodsDataListFromFallGoodsItemList.Count; i++)
							{
								goodsDataList.Add(goodsDataListFromFallGoodsItemList[i]);
							}
							result = 1;
						}
					}
				}
			}
			return result;
		}
	}
}
