using System;
using System.Collections.Generic;
using Server.Data;
using Server.Protocol;
using Server.Tools;

namespace GameServer.Logic
{
	public class WaBaoManager
	{
		public static TCPOutPacket ProcessRandomWaBao(GameClient client, TCPOutPacketPool pool, int cmd)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = -1
			};
			TCPOutPacket result;
			if (null != client.ClientData.WaBaoGoodsData)
			{
				goodsData.Id = -1000;
				result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
			}
			else
			{
				int goodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("WaBaoGoodsID", -1);
				if (Global.GetTotalGoodsCountByID(client, goodsID) <= 0)
				{
					result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
				}
				else
				{
					bool flag = false;
					bool flag2 = false;
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, pool, client, goodsID, 1, false, out flag, out flag2, false))
					{
						result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
					}
					else
					{
						int randomNumber = Global.GetRandomNumber(1, 10001);
						Dictionary<int, SystemXmlItem> systemXmlItemDict = GameManager.systemWaBaoMgr.SystemXmlItemDict;
						List<int> list = new List<int>();
						foreach (SystemXmlItem systemXmlItem in systemXmlItemDict.Values)
						{
							if (randomNumber >= systemXmlItem.GetIntValue("StartValues", -1) && randomNumber <= systemXmlItem.GetIntValue("EndValues", -1))
							{
								list.Add(systemXmlItem.GetIntValue("ID", -1));
							}
						}
						if (list.Count <= 0)
						{
							goodsData.Id = -20;
							result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
						}
						else
						{
							int randomNumber2 = Global.GetRandomNumber(0, list.Count);
							int num = list[randomNumber2];
							SystemXmlItem systemXmlItem2 = null;
							if (!GameManager.systemWaBaoMgr.SystemXmlItemDict.TryGetValue(num, out systemXmlItem2))
							{
								goodsData.Id = -30;
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
							else
							{
								goodsData.Id = num;
								goodsData.GoodsID = systemXmlItem2.GetIntValue("GoodsID", -1);
								goodsData.Using = 0;
								goodsData.Forge_level = systemXmlItem2.GetIntValue("Level", -1);
								goodsData.Starttime = "1900-01-01 12:00:00";
								goodsData.Endtime = "1900-01-01 12:00:00";
								goodsData.Site = 0;
								goodsData.Quality = systemXmlItem2.GetIntValue("Quality", -1);
								goodsData.Props = "";
								goodsData.GCount = 1;
								goodsData.Binding = (flag ? 1 : 0);
								goodsData.Jewellist = "";
								goodsData.BagIndex = 0;
								goodsData.AddPropIndex = 0;
								goodsData.BornIndex = 0;
								goodsData.Lucky = 0;
								goodsData.Strong = 0;
								goodsData.ExcellenceInfo = 0;
								goodsData.AppendPropLev = 0;
								goodsData.ChangeLifeLevForEquip = 0;
								client.ClientData.WaBaoGoodsData = goodsData;
								Global.BroadcastWaBaoGoodsHint(client, goodsData);
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
						}
					}
				}
			}
			return result;
		}

		public static TCPOutPacket ProcessGetWaBaoGoodsData(GameClient client, TCPOutPacketPool pool, int cmd)
		{
			TCPOutPacket result;
			if (null == client.ClientData.WaBaoGoodsData)
			{
				string data = string.Format("{0}:{1}", -1, client.ClientData.RoleID);
				result = TCPOutPacket.MakeTCPOutPacket(pool, data, cmd);
			}
			else if (!Global.CanAddGoods(client, client.ClientData.WaBaoGoodsData.GoodsID, client.ClientData.WaBaoGoodsData.GCount, client.ClientData.WaBaoGoodsData.Binding, client.ClientData.WaBaoGoodsData.Endtime, true, false))
			{
				string data = string.Format("{0}:{1}", -10, client.ClientData.RoleID);
				result = TCPOutPacket.MakeTCPOutPacket(pool, data, cmd);
			}
			else
			{
				int num = Global.AddGoodsDBCommand(pool, client, client.ClientData.WaBaoGoodsData.GoodsID, client.ClientData.WaBaoGoodsData.GCount, client.ClientData.WaBaoGoodsData.Quality, client.ClientData.WaBaoGoodsData.Props, client.ClientData.WaBaoGoodsData.Forge_level, client.ClientData.WaBaoGoodsData.Binding, client.ClientData.WaBaoGoodsData.Site, client.ClientData.WaBaoGoodsData.Jewellist, true, 1, "挖宝获取道具", "1900-01-01 12:00:00", client.ClientData.WaBaoGoodsData.AddPropIndex, client.ClientData.WaBaoGoodsData.BornIndex, client.ClientData.WaBaoGoodsData.Lucky, client.ClientData.WaBaoGoodsData.Strong, 0, 0, 0, null, null, 0, true);
				if (num < 0)
				{
					string data = string.Format("{0}:{1}", -10, client.ClientData.RoleID);
					result = TCPOutPacket.MakeTCPOutPacket(pool, data, cmd);
				}
				else
				{
					client.ClientData.WaBaoGoodsData = null;
					string data = string.Format("{0}:{1}", 0, client.ClientData.RoleID);
					result = TCPOutPacket.MakeTCPOutPacket(pool, data, cmd);
				}
			}
			return result;
		}

		public static TCPOutPacket ProcessWaBaoByYaoShi(GameClient client, TCPOutPacketPool pool, int cmd, int idXiangZi, int idYaoShi, bool autoBuy)
		{
			GoodsData goodsData = new GoodsData
			{
				Id = -1
			};
			TCPOutPacket result;
			if ("1" != GameManager.GameConfigMgr.GetGameConfigItemStr("keydigtreasure", "1"))
			{
				goodsData.Id = -20;
				result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
			}
			else
			{
				Dictionary<int, int> yaoShiDiaoLuoForXiangZhi = Global.GetYaoShiDiaoLuoForXiangZhi(idXiangZi);
				if (yaoShiDiaoLuoForXiangZhi == null || yaoShiDiaoLuoForXiangZhi.Count <= 0)
				{
					goodsData.Id = -30;
					result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
				}
				else
				{
					bool flag = false;
					foreach (int num in yaoShiDiaoLuoForXiangZhi.Keys)
					{
						if (num == idYaoShi)
						{
							flag = true;
							break;
						}
					}
					if (!flag)
					{
						goodsData.Id = -50;
						result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
					}
					else
					{
						bool flag2 = true;
						bool flag3 = true;
						bool flag4 = true;
						bool flag5 = true;
						if (!Global.CanAddGoodsNum(client, 1))
						{
							goodsData.Id = -300;
							result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
						}
						else
						{
							Dictionary<int, int> dictionary = new Dictionary<int, int>();
							if (Global.GetTotalGoodsCountByID(client, idXiangZi) <= 0)
							{
								flag2 = false;
								dictionary.Add(idXiangZi, 1);
							}
							if (0 != idYaoShi)
							{
								if (Global.GetTotalGoodsCountByID(client, idYaoShi) <= 0)
								{
									flag3 = false;
									dictionary.Add(idYaoShi, 1);
								}
							}
							int userMoney = client.ClientData.UserMoney;
							int num2 = 0;
							if (dictionary.Count > 0)
							{
								if (autoBuy)
								{
									num2 = Global.SubUserMoneyForGoods(client, dictionary, "精雕细琢挖宝");
									if (num2 <= 0)
									{
										goodsData.Id = num2;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
									if (!flag2)
									{
										flag4 = false;
									}
									if (!flag3)
									{
										flag5 = false;
									}
								}
								else
								{
									if (!flag2)
									{
										goodsData.Id = -100;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
									if (!flag3)
									{
										goodsData.Id = -200;
										return DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
									}
								}
							}
							if (0 == idYaoShi)
							{
								flag5 = false;
							}
							GoodsData goodsData2 = null;
							int num3 = GoodsBaoXiang.ProcessFallByYaoShiWaBao(client, yaoShiDiaoLuoForXiangZhi[idYaoShi], flag5 ? idYaoShi : -1, flag4 ? idXiangZi : -1, out goodsData2, (idYaoShi == 0) ? 1 : 0, num2);
							if (num3 <= 0 || null == goodsData2)
							{
								goodsData.Id = num3;
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
							else
							{
								goodsData = goodsData2;
								Global.BroadcastYaoShiWaBaoGoodsHint(client, goodsData, idYaoShi, idXiangZi);
								Global.AddDigTreasureWithYaoShiEvent(client, idYaoShi, idXiangZi, flag5 ? 1 : 0, flag4 ? 1 : 0, num2, userMoney, client.ClientData.UserMoney, goodsData);
								result = DataHelper.ObjectToTCPOutPacket<GoodsData>(goodsData, pool, cmd);
							}
						}
					}
				}
			}
			return result;
		}
	}
}
