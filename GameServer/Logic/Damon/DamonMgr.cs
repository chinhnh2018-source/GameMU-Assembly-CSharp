using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic.Damon
{
	internal class DamonMgr
	{
		public static GoodsData GetDamonGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.DamonGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Id == id)
						{
							return client.ClientData.DamonGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static void AddDamonGoodsData(GameClient client, GoodsData goodsData, bool refreshProps = true)
		{
			if (goodsData.Site == 0 || goodsData.Site == 10000)
			{
				if (null == client.ClientData.DamonGoodsDataList)
				{
					client.ClientData.DamonGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.DamonGoodsDataList)
				{
					client.ClientData.DamonGoodsDataList.Add(goodsData);
				}
				JingLingQiYuanManager.getInstance().RefreshProps(client, true);
			}
		}

		public static void AddOldDamonGoodsData(GameClient client)
		{
			if (null != client.ClientData.GoodsDataList)
			{
				List<GoodsData> list = new List<GoodsData>();
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					int goodsCatetoriy = Global.GetGoodsCatetoriy(client.ClientData.GoodsDataList[i].GoodsID);
					if (goodsCatetoriy >= 9 && goodsCatetoriy <= 10)
					{
						if (client.ClientData.GoodsDataList[i].Using > 0 && client.ClientData.GoodsDataList[i].Site == 0)
						{
							int idleSlotOfDamonGoods = Global.GetIdleSlotOfDamonGoods(client);
							string[] array = null;
							string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.GoodsDataList[i].Id,
								client.ClientData.GoodsDataList[i].Using,
								"*",
								"*",
								"*",
								5000,
								"*",
								"*",
								client.ClientData.GoodsDataList[i].GCount,
								"*",
								idleSlotOfDamonGoods,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(TCPClientPool.getInstance(), TCPOutPacketPool.getInstance(), 10006, strcmd, out array, client.ServerId);
							if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_FAILED)
							{
								if (array.Length > 0 && Convert.ToInt32(array[1]) >= 0)
								{
									DamonMgr.AddDamonGoodsData(client, client.ClientData.GoodsDataList[i], false);
									client.ClientData.GoodsDataList[i].Site = 5000;
									client.ClientData.GoodsDataList[i].BagIndex = idleSlotOfDamonGoods;
									list.Add(client.ClientData.GoodsDataList[i]);
								}
							}
						}
					}
					IL_2AC:
					i++;
					continue;
					goto IL_2AC;
				}
				for (i = 0; i < list.Count; i++)
				{
					Global.RemoveGoodsData(client, list[i]);
				}
				JingLingQiYuanManager.getInstance().RefreshProps(client, true);
			}
		}

		public static void InitDemonGoodsDataList(GameClient client)
		{
			if (null == client.ClientData.DamonGoodsDataList)
			{
				client.ClientData.DamonGoodsDataList = Global.sendToDB<List<GoodsData>, string>(204, string.Format("{0}:{1}", client.ClientData.RoleID, 5000), client.ServerId);
				if (client.ClientData.DamonGoodsDataList == null || client.ClientData.DamonGoodsDataList.Count == 0)
				{
					client.ClientData.DamonGoodsDataList = new List<GoodsData>();
					DamonMgr.AddOldDamonGoodsData(client);
				}
			}
			JingLingQiYuanManager.getInstance().RefreshProps(client, true);
		}

		public static GoodsData AddDamonGoodsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = id,
				GoodsID = goodsID,
				Using = 0,
				Forge_level = forgeLevel,
				Starttime = "1900-01-01 12:00:00",
				Endtime = endTime,
				Site = site,
				Quality = quality,
				Props = "",
				GCount = goodsNum,
				Binding = binding,
				Jewellist = jewelList,
				BagIndex = 0,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			DamonMgr.AddDamonGoodsData(client, goodsData, true);
			return goodsData;
		}

		public static void RemoveDamonGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.DamonGoodsDataList)
			{
				if (goodsData.Site == 5000)
				{
					lock (client.ClientData.DamonGoodsDataList)
					{
						client.ClientData.DamonGoodsDataList.Remove(goodsData);
					}
					JingLingQiYuanManager.getInstance().RefreshProps(client, true);
				}
			}
		}

		public static void ResetDamonBagAllGoods(GameClient client)
		{
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					Dictionary<string, GoodsData> dictionary = new Dictionary<string, GoodsData>();
					List<GoodsData> list = new List<GoodsData>();
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using <= 0)
						{
							client.ClientData.DamonGoodsDataList[i].BagIndex = 1;
							int goodsGridNumByID = Global.GetGoodsGridNumByID(client.ClientData.DamonGoodsDataList[i].GoodsID);
							if (goodsGridNumByID > 1)
							{
								GoodsData goodsData = null;
								string key = string.Format("{0}_{1}_{2}", client.ClientData.DamonGoodsDataList[i].GoodsID, client.ClientData.DamonGoodsDataList[i].Binding, Global.DateTimeTicks(client.ClientData.DamonGoodsDataList[i].Endtime));
								if (dictionary.TryGetValue(key, out goodsData))
								{
									int num = Global.GMin(goodsGridNumByID - goodsData.GCount, client.ClientData.DamonGoodsDataList[i].GCount);
									goodsData.GCount += num;
									client.ClientData.DamonGoodsDataList[i].GCount -= num;
									client.ClientData.DamonGoodsDataList[i].BagIndex = 1;
									goodsData.BagIndex = 1;
									if (!Global.ResetBagGoodsData(client, client.ClientData.DamonGoodsDataList[i]))
									{
										break;
									}
									if (goodsData.GCount >= goodsGridNumByID)
									{
										if (client.ClientData.DamonGoodsDataList[i].GCount > 0)
										{
											dictionary[key] = client.ClientData.DamonGoodsDataList[i];
										}
										else
										{
											dictionary.Remove(key);
											list.Add(client.ClientData.DamonGoodsDataList[i]);
										}
									}
									else if (client.ClientData.DamonGoodsDataList[i].GCount <= 0)
									{
										list.Add(client.ClientData.DamonGoodsDataList[i]);
									}
								}
								else
								{
									dictionary[key] = client.ClientData.DamonGoodsDataList[i];
								}
							}
						}
					}
					for (int i = 0; i < list.Count; i++)
					{
						client.ClientData.DamonGoodsDataList.Remove(list[i]);
					}
					client.ClientData.DamonGoodsDataList.Sort((GoodsData x, GoodsData y) => y.GoodsID - x.GoodsID);
					int num2 = 0;
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.DamonGoodsDataList[i].Using <= 0)
						{
							bool flag2 = 0 == 0;
							client.ClientData.DamonGoodsDataList[i].BagIndex = num2++;
							if (!Global.ResetBagGoodsData(client, client.ClientData.DamonGoodsDataList[i]))
							{
								break;
							}
						}
					}
				}
			}
			TCPOutPacket tcpOutPacket = null;
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.DamonGoodsDataList, Global._TCPManager.TcpOutPacketPool, 449);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.DamonGoodsDataList, Global._TCPManager.TcpOutPacketPool, 449);
			}
			Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		public static bool CanAddGoodsToDamonCangKu(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.DamonGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int num = Global.GetGoodsGridNumByID(goodsID);
				num = Global.GMax(num, 1);
				bool flag = false;
				int num2 = 0;
				lock (client.ClientData.DamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.DamonGoodsDataList.Count; i++)
					{
						num2++;
						if (canUseOld && num > 1)
						{
							if (client.ClientData.DamonGoodsDataList[i].GoodsID == goodsID && client.ClientData.DamonGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.DamonGoodsDataList[i].Endtime, endTime))
							{
								if (client.ClientData.DamonGoodsDataList[i].GCount + newGoodsNum <= num)
								{
									flag = true;
									break;
								}
							}
						}
					}
				}
				if (flag)
				{
					result = true;
				}
				else
				{
					int damonBagCapacity = DamonMgr.GetDamonBagCapacity(client);
					result = (num2 < damonBagCapacity);
				}
			}
			return result;
		}

		public static int GetDamonBagCapacity(GameClient client)
		{
			return Global.MaxDamonGridNum;
		}

		public static List<GoodsData> GetDemonGoodsDataList(GameClient client)
		{
			List<GoodsData> list = new List<GoodsData>();
			if (null != client.ClientData.DamonGoodsDataList)
			{
				lock (client.ClientData.DamonGoodsDataList)
				{
					list.AddRange(client.ClientData.DamonGoodsDataList);
				}
			}
			return list;
		}
	}
}
