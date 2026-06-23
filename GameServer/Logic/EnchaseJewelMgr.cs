using System;
using System.Collections.Generic;
using Server.Data;

namespace GameServer.Logic
{
	public class EnchaseJewelMgr
	{
		public static int ProcessEnchaseJewel(GameClient client, int actionType, int equipGoodsDbID, int jewelGoodsIDorDbID, out string jewellist, out int binding)
		{
			jewellist = "";
			binding = 0;
			GoodsData goodsByDbID = Global.GetGoodsByDbID(client, equipGoodsDbID);
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
						GoodsData goodsData = null;
						int num;
						if (0 == actionType)
						{
							goodsData = Global.GetGoodsByDbID(client, jewelGoodsIDorDbID);
							if (goodsData == null || goodsData.GCount <= 0)
							{
								return -100;
							}
							num = goodsData.GoodsID;
						}
						else
						{
							num = jewelGoodsIDorDbID;
						}
						if (!Global.CanEnchaseJewel(num))
						{
							result = -4;
						}
						else if (!Global.CanAddJewelIntoEquip(goodsByDbID.GoodsID, num))
						{
							result = -5;
						}
						else
						{
							if (0 == actionType)
							{
								if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsData, 1, false, false))
								{
									return -101;
								}
								if (!string.IsNullOrEmpty(goodsByDbID.Jewellist))
								{
									string[] array = goodsByDbID.Jewellist.Split(new char[]
									{
										','
									});
									if (array.Length >= 6)
									{
										return -110;
									}
								}
								jewellist = goodsByDbID.Jewellist;
								if (jewellist.Length > 0)
								{
									jewellist += ",";
								}
								jewellist += string.Format("{0}", num);
								binding = goodsByDbID.Binding;
								if (goodsByDbID.Binding != goodsData.Binding)
								{
									if (goodsData.Binding > 0)
									{
										binding = 1;
									}
								}
								if (Global.ModGoodsJewelDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsByDbID, jewellist, binding) < 0)
								{
									return -102;
								}
							}
							else
							{
								if (!Global.CanAddGoods(client, num, 1, 0, "1900-01-01 12:00:00", true, false))
								{
									return -200;
								}
								jewellist = goodsByDbID.Jewellist;
								if (string.IsNullOrEmpty(jewellist))
								{
									return -201;
								}
								string[] array2 = jewellist.Split(new char[]
								{
									','
								});
								List<string> list = new List<string>();
								for (int i = 0; i < array2.Length; i++)
								{
									list.Add(array2[i].Trim());
								}
								bool flag = false;
								for (int i = 0; i < list.Count; i++)
								{
									if (list[i] == num.ToString())
									{
										flag = true;
										list.RemoveAt(i);
										break;
									}
								}
								if (!flag)
								{
									return -300;
								}
								jewellist = "";
								for (int i = 0; i < list.Count; i++)
								{
									if (jewellist.Length > 0)
									{
										jewellist += ",";
									}
									jewellist += list[i];
								}
								if (Global.ModGoodsJewelDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsByDbID, jewellist, goodsByDbID.Binding) < 0)
								{
									return -202;
								}
								int num2 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num, 1, 0, "", 0, goodsByDbID.Binding, 0, "", true, 1, "宝石解镶嵌", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
								if (num2 < 0)
								{
									return -203;
								}
							}
							result = 0;
						}
					}
				}
			}
			return result;
		}
	}
}
