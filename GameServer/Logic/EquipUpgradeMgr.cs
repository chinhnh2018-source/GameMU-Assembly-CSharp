using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class EquipUpgradeMgr
	{
		public static int ProcessUpgrade(GameClient client, int goodsDbID)
		{
			GoodsData goodsByDbID = Global.GetGoodsByDbID(client, goodsDbID);
			int result;
			if (null == goodsByDbID)
			{
				result = -1;
			}
			else if (goodsByDbID.Site != 0)
			{
				result = -9998;
			}
			else if (goodsByDbID.Using > 0)
			{
				result = -9999;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsByDbID.GoodsID, out systemXmlItem))
				{
					result = -2;
				}
				else
				{
					int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
					if (intValue < 0 || intValue >= 49)
					{
						result = -3;
					}
					else
					{
						int intValue2 = systemXmlItem.GetIntValue("SuitID", -1);
						int mainOccupationByGoodsID = Global.GetMainOccupationByGoodsID(goodsByDbID.GoodsID);
						int intValue3 = systemXmlItem.GetIntValue("ToSex", -1);
						int num = -1;
						Dictionary<int, SystemXmlItem> systemXmlItemDict = GameManager.SystemGoods.SystemXmlItemDict;
						foreach (SystemXmlItem systemXmlItem2 in systemXmlItemDict.Values)
						{
							int intValue4 = systemXmlItem2.GetIntValue("Categoriy", -1);
							int intValue5 = systemXmlItem2.GetIntValue("SuitID", -1);
							int intValue6 = systemXmlItem2.GetIntValue("ToSex", -1);
							int intValue7 = systemXmlItem2.GetIntValue("MainOccupation", -1);
							if (intValue4 == intValue && intValue5 == intValue2 + 1 && intValue7 == mainOccupationByGoodsID && intValue6 == intValue3)
							{
								num = systemXmlItem2.GetIntValue("ID", -1);
								break;
							}
						}
						if (num < 0)
						{
							result = -5;
						}
						else
						{
							int suitID = intValue2 + 1;
							SystemXmlItem equipUpgradeCacheItem = EquipUpgradeCacheMgr.GetEquipUpgradeCacheItem(intValue, suitID);
							if (null == equipUpgradeCacheItem)
							{
								result = -4;
							}
							else
							{
								int intValue8 = equipUpgradeCacheItem.GetIntValue("NeedGoodsID", -1);
								if (intValue8 < 0)
								{
									result = -6;
								}
								else
								{
									int intValue9 = equipUpgradeCacheItem.GetIntValue("GoodsNum", -1);
									if (intValue9 <= 0)
									{
										result = -7;
									}
									else
									{
										int intValue10 = equipUpgradeCacheItem.GetIntValue("JiFen", -1);
										if (intValue10 <= 0)
										{
											result = -8;
										}
										else
										{
											int num2 = equipUpgradeCacheItem.GetIntValue("Succeed", -1) * 100;
											if (num2 < 0)
											{
												result = -9;
											}
											else
											{
												int binding = goodsByDbID.Binding;
												bool flag = false;
												bool flag2 = false;
												if (Global.GetTotalGoodsCountByID(client, intValue8) < intValue9)
												{
													result = -10;
												}
												else
												{
													int zhuangBeiJiFenValue = GameManager.ClientMgr.GetZhuangBeiJiFenValue(client);
													if (zhuangBeiJiFenValue < intValue10)
													{
														result = -11;
													}
													else if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, intValue8, intValue9, false, out flag, out flag2, false))
													{
														result = -12;
													}
													else if (Global.GetRandomNumber(0, 10001) > num2)
													{
														result = -1000;
													}
													else
													{
														GameManager.ClientMgr.ModifyZhuangBeiJiFenValue(client, -intValue10, true, true);
														if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsDbID, false, false))
														{
															result = -14;
														}
														else
														{
															int strong = Math.Max(goodsByDbID.Strong, 0);
															int lucky = 0;
															int num3 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num, 1, goodsByDbID.Quality, "", goodsByDbID.Forge_level, goodsByDbID.Binding, 0, goodsByDbID.Jewellist, false, 1, "装备进阶", "1900-01-01 12:00:00", goodsByDbID.AddPropIndex, goodsByDbID.BornIndex, lucky, strong, goodsByDbID.ExcellenceInfo, goodsByDbID.AppendPropLev, goodsByDbID.ChangeLifeLevForEquip, goodsByDbID.WashProps, null, 0, true);
															if (num3 < 0)
															{
																result = -2000;
															}
															else
															{
																Global.BroadcastEquipUpgradeOk(client, goodsByDbID.GoodsID, num, goodsByDbID.Quality, goodsByDbID.Forge_level);
																result = num3;
															}
														}
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
			}
			return result;
		}
	}
}
