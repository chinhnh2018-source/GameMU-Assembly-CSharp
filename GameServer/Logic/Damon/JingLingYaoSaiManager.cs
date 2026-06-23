using System;
using System.Collections.Generic;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic.Damon
{
	internal class JingLingYaoSaiManager
	{
		public static GoodsData GetPaiZhuDamonGoodsDataByDbID(GameClient client, int id)
		{
			GoodsData result;
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				result = null;
			}
			else
			{
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
					{
						if (client.ClientData.PaiZhuDamonGoodsDataList[i].Id == id)
						{
							return client.ClientData.PaiZhuDamonGoodsDataList[i];
						}
					}
				}
				result = null;
			}
			return result;
		}

		public static void AddPaiZhuDamonGoodsData(GameClient client, GoodsData goodsData, bool refreshProps = true)
		{
			if (goodsData.Site == 0)
			{
				if (null == client.ClientData.PaiZhuDamonGoodsDataList)
				{
					client.ClientData.PaiZhuDamonGoodsDataList = new List<GoodsData>();
				}
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					client.ClientData.PaiZhuDamonGoodsDataList.Add(goodsData);
				}
			}
		}

		public static void AddOldPaiZhuDamonGoodsData(GameClient client)
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
							int idleSlotOfPaiZhuDamonGoods = JingLingYaoSaiManager.GetIdleSlotOfPaiZhuDamonGoods(client);
							string[] array = null;
							string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.GoodsDataList[i].Id,
								client.ClientData.GoodsDataList[i].Using,
								"*",
								"*",
								"*",
								10000,
								"*",
								"*",
								client.ClientData.GoodsDataList[i].GCount,
								"*",
								idleSlotOfPaiZhuDamonGoods,
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
									JingLingYaoSaiManager.AddPaiZhuDamonGoodsData(client, client.ClientData.GoodsDataList[i], false);
									client.ClientData.GoodsDataList[i].Site = 10000;
									client.ClientData.GoodsDataList[i].BagIndex = idleSlotOfPaiZhuDamonGoods;
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
			}
		}

		public static void InitPaiZhuDemonGoodsDataList(GameClient client)
		{
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				client.ClientData.PaiZhuDamonGoodsDataList = Global.sendToDB<List<GoodsData>, string>(20314, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, 10000, 10001), client.ServerId);
				if (client.ClientData.PaiZhuDamonGoodsDataList == null || client.ClientData.PaiZhuDamonGoodsDataList.Count == 0)
				{
					client.ClientData.PaiZhuDamonGoodsDataList = new List<GoodsData>();
					JingLingYaoSaiManager.AddOldPaiZhuDamonGoodsData(client);
				}
			}
		}

		public static void RemovePaiZhuDamonGoodsData(GameClient client, GoodsData goodsData)
		{
			if (null != client.ClientData.PaiZhuDamonGoodsDataList)
			{
				if (goodsData.Site == 10000)
				{
					lock (client.ClientData.PaiZhuDamonGoodsDataList)
					{
						client.ClientData.PaiZhuDamonGoodsDataList.Remove(goodsData);
					}
				}
			}
		}

		public static bool CanAddGoodsToPaiZhuDamonCangKu(GameClient client, int goodsID, int newGoodsNum, int binding, string endTime = "1900-01-01 12:00:00", bool canUseOld = true)
		{
			bool result;
			if (client.ClientData.PaiZhuDamonGoodsDataList == null)
			{
				result = true;
			}
			else
			{
				int num = Global.GetGoodsGridNumByID(goodsID);
				num = Global.GMax(num, 1);
				bool flag = false;
				int num2 = 0;
				lock (client.ClientData.PaiZhuDamonGoodsDataList)
				{
					for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
					{
						num2++;
						if (canUseOld && num > 1)
						{
							if (client.ClientData.PaiZhuDamonGoodsDataList[i].GoodsID == goodsID && client.ClientData.PaiZhuDamonGoodsDataList[i].Binding == binding && Global.DateTimeEqual(client.ClientData.PaiZhuDamonGoodsDataList[i].Endtime, endTime))
							{
								if (client.ClientData.PaiZhuDamonGoodsDataList[i].GCount + newGoodsNum <= num)
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
					int paiZhuDamonBagCapacity = JingLingYaoSaiManager.GetPaiZhuDamonBagCapacity(client);
					result = (num2 < paiZhuDamonBagCapacity);
				}
			}
			return result;
		}

		public static int GetPaiZhuDamonBagCapacity(GameClient client)
		{
			return Global.ManorPetMax;
		}

		public static int GetIdleSlotOfPaiZhuDamonGoods(GameClient client)
		{
			int num = 0;
			int result;
			if (null == client.ClientData.PaiZhuDamonGoodsDataList)
			{
				result = num;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.PaiZhuDamonGoodsDataList.Count; i++)
				{
					if (client.ClientData.PaiZhuDamonGoodsDataList[i].Site == 10000)
					{
						if (list.IndexOf(client.ClientData.PaiZhuDamonGoodsDataList[i].BagIndex) < 0)
						{
							list.Add(client.ClientData.PaiZhuDamonGoodsDataList[i].BagIndex);
						}
					}
				}
				int paiZhuDamonBagCapacity = JingLingYaoSaiManager.GetPaiZhuDamonBagCapacity(client);
				for (int j = 0; j < paiZhuDamonBagCapacity; j++)
				{
					if (list.IndexOf(j) < 0)
					{
						num = j;
						break;
					}
				}
				result = num;
			}
			return result;
		}
	}
}
