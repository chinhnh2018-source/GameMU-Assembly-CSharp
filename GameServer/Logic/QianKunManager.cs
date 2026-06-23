using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew.SevenDay;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class QianKunManager
	{
		public static void LoadImpetrateItemsInfo()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataInfo = null;
				QianKunManager.m_ImpetrateDataInfo = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/NewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataInfo.Add(key, dictionary);
				}
			}
		}

		public static void LoadImpetrateItemsInfoFree()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataInfoFree = null;
				QianKunManager.m_ImpetrateDataInfoFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/FreeNewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataInfoFree.Add(key, dictionary);
				}
			}
		}

		public static void LoadImpetrateItemsInfoTeQuan()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataTeQuan = null;
				QianKunManager.m_ImpetrateDataTeQuan = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/TeQuanNewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataTeQuan.Add(key, dictionary);
				}
			}
		}

		public static void LoadImpetrateItemsInfoFreeTeQuan()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataTeQuanFree = null;
				QianKunManager.m_ImpetrateDataTeQuanFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/TeQuanFreeNewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataTeQuanFree.Add(key, dictionary);
				}
			}
		}

		public static void LoadImpetrateItemsInfoHuodong()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataHuoDong = null;
				QianKunManager.m_ImpetrateDataHuoDong = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/HuoDongNewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataHuoDong.Add(key, dictionary);
				}
			}
		}

		public static void LoadImpetrateItemsInfoFreeHuoDong()
		{
			lock (QianKunManager.m_mutex)
			{
				QianKunManager.m_ImpetrateDataHuoDongFree = null;
				QianKunManager.m_ImpetrateDataHuoDongFree = new Dictionary<int, Dictionary<int, SystemXmlItem>>();
				string text = "";
				XElement xelement = null;
				try
				{
					text = string.Format("Config/HuoDongFreeNewDig.xml", new object[0]);
					xelement = XElement.Load(Global.GameResPath(text));
					if (null == xelement)
					{
						throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
					}
				}
				catch (Exception)
				{
					throw new Exception(string.Format("加载系统xml配置文件:{0}, 失败。没有找到相关XML配置文件!", text));
				}
				IEnumerable<XElement> enumerable = xelement.Elements("Type");
				foreach (XElement xelement2 in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xelement2, "TypeID");
					Dictionary<int, SystemXmlItem> dictionary = new Dictionary<int, SystemXmlItem>();
					IEnumerable<XElement> enumerable2 = xelement2.Elements("Item");
					foreach (XElement xelement3 in enumerable2)
					{
						SystemXmlItem value = new SystemXmlItem
						{
							XMLNode = xelement3
						};
						int key2 = (int)Global.GetSafeAttributeLong(xelement3, "ID");
						dictionary[key2] = value;
					}
					QianKunManager.m_ImpetrateDataHuoDongFree.Add(key, dictionary);
				}
			}
		}

		public static void ProcessRandomWaBao(GameClient client, int binding, Dictionary<int, SystemXmlItem> SystemXmlItemDict, int nType)
		{
			int randomNumber = Global.GetRandomNumber(1, 10001);
			List<SystemXmlItem> list = new List<SystemXmlItem>();
			foreach (SystemXmlItem systemXmlItem in SystemXmlItemDict.Values)
			{
				if (randomNumber >= systemXmlItem.GetIntValue("StartValues", -1) && randomNumber <= systemXmlItem.GetIntValue("EndValues", -1))
				{
					list.Add(systemXmlItem);
				}
			}
			if (list.Count > 0)
			{
				List<string> list2 = new List<string>();
				int randomNumber2 = Global.GetRandomNumber(0, list.Count);
				SystemXmlItem systemXmlItem2 = list[randomNumber2];
				int intValue = systemXmlItem2.GetIntValue("GoodsID", -1);
				if (intValue > 0)
				{
					if (Global.CanAddGoods(client, intValue, 1, binding, "1900-01-01 12:00:00", true, false))
					{
						int num = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, intValue, 1, 0, "", 0, binding, 0, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, null, 0, true);
						if (num < 0)
						{
							LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
							{
								client.ClientData.RoleID,
								intValue,
								binding,
								num
							}), null, true);
						}
						else
						{
							Global.BroadcastQianKunDaiGoodsHint(client, intValue, nType);
							string msgText = string.Format(GLang.GetLang(519, new object[0]), Global.GetGoodsNameByID(intValue));
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
						}
					}
					else
					{
						LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, intValue, binding), null, true);
					}
				}
				int intValue2 = systemXmlItem2.GetIntValue("MinMoney", -1);
				int intValue3 = systemXmlItem2.GetIntValue("MaxMoney", -1);
				if (intValue2 >= 0 && intValue3 > intValue2)
				{
					int randomNumber3 = Global.GetRandomNumber(intValue2, intValue3);
					if (randomNumber3 > 0)
					{
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, randomNumber3, "开启乾坤袋一", false);
						string msgText = string.Format(GLang.GetLang(520, new object[0]), randomNumber3);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
				int intValue4 = systemXmlItem2.GetIntValue("MinBindYuanBao", -1);
				int intValue5 = systemXmlItem2.GetIntValue("MaxBindYuanBao", -1);
				if (intValue4 >= 0 && intValue5 > intValue4)
				{
					int randomNumber4 = Global.GetRandomNumber(intValue4, intValue5);
					if (randomNumber4 > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, randomNumber4, "开启乾坤袋");
						string msgText = string.Format(GLang.GetLang(521, new object[0]), randomNumber4);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
				int intValue6 = systemXmlItem2.GetIntValue("MinExp", -1);
				int intValue7 = systemXmlItem2.GetIntValue("MaxExp", -1);
				if (intValue6 >= 0 && intValue7 > intValue6)
				{
					int randomNumber5 = Global.GetRandomNumber(intValue6, intValue7);
					if (randomNumber5 > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)randomNumber5, false, true, false, "none");
						string msgText = string.Format(GLang.GetLang(522, new object[0]), randomNumber5);
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		public static string ProcessRandomWaBaoByZaDan(GameClient client, Dictionary<int, SystemXmlItem> SystemXmlItemDic, int nType, out string strRecord, int binding = 0, bool bMuProject = false)
		{
			strRecord = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int randomNumber = Global.GetRandomNumber(1, 100001);
			List<SystemXmlItem> list = new List<SystemXmlItem>();
			foreach (SystemXmlItem systemXmlItem in SystemXmlItemDic.Values)
			{
				if (randomNumber >= systemXmlItem.GetIntValue("StartValues", -1) && randomNumber <= systemXmlItem.GetIntValue("EndValues", -1))
				{
					list.Add(systemXmlItem);
					break;
				}
			}
			string result;
			if (list.Count <= 0)
			{
				result = "";
			}
			else
			{
				List<string> list2 = new List<string>();
				int randomNumber2 = Global.GetRandomNumber(0, list.Count);
				SystemXmlItem systemXmlItem2 = list[randomNumber2];
				int num6 = 0;
				int num7 = 0;
				int num8 = 0;
				int num9 = 0;
				int intValue = systemXmlItem2.GetIntValue("Num", -1);
				int intValue2 = systemXmlItem2.GetIntValue("GoodsID", -1);
				if (intValue2 > 0)
				{
					if (Global.CanAddGoodsToJinDanCangKu(client, intValue2, 1, binding, "1900-01-01 12:00:00", true))
					{
						int intValue3 = systemXmlItem2.GetIntValue("QiangHuaFallID", -1);
						if (intValue3 != -1)
						{
							num6 = GameManager.GoodsPackMgr.GetFallGoodsLevel(intValue3);
						}
						int intValue4 = systemXmlItem2.GetIntValue("ZhuiJiaFallID", -1);
						if (intValue4 != -1)
						{
							num7 = GameManager.GoodsPackMgr.GetZhuiJiaGoodsLevelID(intValue4);
						}
						int intValue5 = systemXmlItem2.GetIntValue("LckyProbability", -1);
						if (intValue5 != -1)
						{
							int luckyGoodsID = GameManager.GoodsPackMgr.GetLuckyGoodsID(intValue5);
							if (luckyGoodsID >= 1)
							{
								num8 = 1;
							}
						}
						int intValue6 = systemXmlItem2.GetIntValue("ZhuoYueFallID", -1);
						if (intValue6 != -1)
						{
							num9 = GameManager.GoodsPackMgr.GetExcellencePropertysID(intValue2, intValue6);
						}
						int num10 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, intValue2, intValue, 0, "", num6, binding, 2000, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, num8, 0, num9, num7, 0, null, null, 0, true);
						if (num10 < 0)
						{
							LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
							{
								client.ClientData.RoleID,
								intValue2,
								binding,
								num10
							}), null, true);
						}
						else
						{
							Global.BroadcastQianKunDaiGoodsHint(client, intValue2, nType);
							num = intValue2;
							num2 = 1;
							SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetEquipCountByQiFu);
							sevenDayGoalEventObject.Arg1 = intValue2;
							sevenDayGoalEventObject.Arg2 = num2;
							GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
						}
					}
					else
					{
						LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, intValue2, binding), null, true);
					}
				}
				int intValue7 = systemXmlItem2.GetIntValue("MinMoney", -1);
				int intValue8 = systemXmlItem2.GetIntValue("MaxMoney", -1);
				if (intValue7 >= 0 && intValue8 > intValue7)
				{
					int randomNumber3 = Global.GetRandomNumber(intValue7, intValue8);
					if (randomNumber3 > 0)
					{
						GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, randomNumber3, "开启乾坤袋二", false);
						num4 = randomNumber3;
					}
				}
				int intValue9 = systemXmlItem2.GetIntValue("MinBindYuanBao", -1);
				int intValue10 = systemXmlItem2.GetIntValue("MaxBindYuanBao", -1);
				if (intValue9 >= 0 && intValue10 > intValue9)
				{
					int randomNumber4 = Global.GetRandomNumber(intValue9, intValue10);
					if (randomNumber4 > 0)
					{
						GameManager.ClientMgr.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, randomNumber4, "开启乾坤袋二");
						num3 = randomNumber4;
					}
				}
				int intValue11 = systemXmlItem2.GetIntValue("MinExp", -1);
				int intValue12 = systemXmlItem2.GetIntValue("MaxExp", -1);
				if (intValue11 >= 0 && intValue12 > intValue11)
				{
					int randomNumber5 = Global.GetRandomNumber(intValue11, intValue12);
					if (randomNumber5 > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)randomNumber5, false, true, false, "none");
						num5 = randomNumber5;
					}
				}
				string text = string.Format("{0}|{1}|{2}|{3}", new object[]
				{
					num6,
					num7,
					num8,
					num9
				});
				string text2;
				if (bMuProject)
				{
					text2 = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
					{
						num,
						intValue,
						binding,
						num6,
						num7,
						num8,
						num9
					});
				}
				else
				{
					text2 = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
					{
						num,
						num2,
						num3,
						num4,
						num5,
						text
					});
				}
				strRecord = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
				{
					num,
					num2,
					num3,
					num4,
					num5,
					text
				});
				if (num > 0)
				{
					EventLogManager.AddRoleQiFuEvent(client, "【{0}】在祈福抽取中获得了【{1}】", new object[]
					{
						client.ClientData.RoleName,
						Global.GetGoodsLogName(new GoodsData
						{
							GoodsID = num,
							ExcellenceInfo = num9
						})
					});
				}
				result = text2;
			}
			return result;
		}

		public static string ProcessRandomWaBaoByZaDanSP(GameClient client, Dictionary<int, SystemXmlItem> SystemXmlItemDic, int nType, out string strRecord, int binding = 0, bool bMuProject = false)
		{
			strRecord = null;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int[] randomGoods = Global.GetRandomGoods(GameManager.systemParamsList.GetParamValueByName("QiFuTen"));
			int num10 = randomGoods[0];
			int num11 = randomGoods[1];
			if (Global.CanAddGoodsToJinDanCangKu(client, num10, 1, binding, "1900-01-01 12:00:00", true))
			{
				int fallLevelID = randomGoods[3];
				num6 = GameManager.GoodsPackMgr.GetFallGoodsLevel(fallLevelID);
				int zhuiJiaID = randomGoods[4];
				num7 = GameManager.GoodsPackMgr.GetZhuiJiaGoodsLevelID(zhuiJiaID);
				int luckyPercent = randomGoods[5];
				num8 = GameManager.GoodsPackMgr.GetLuckyGoodsID(luckyPercent);
				int excellencePropertyGroupID = randomGoods[6];
				num9 = GameManager.GoodsPackMgr.GetExcellencePropertysID(num10, excellencePropertyGroupID);
				int num12 = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num10, num11, 0, "", num6, binding, 2000, "", true, 1, "乾坤袋挖宝获取道具", "1900-01-01 12:00:00", 0, 0, num8, 0, num9, num7, 0, null, null, 0, true);
				if (num12 < 0)
				{
					LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，放入物品时错误, RoleID={0}, GoodsID={1}, Binding={2}, Ret={3}", new object[]
					{
						client.ClientData.RoleID,
						num10,
						binding,
						num12
					}), null, true);
				}
				else
				{
					Global.BroadcastQianKunDaiGoodsHint(client, num10, nType);
					num = num10;
					num2 = 1;
					SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.GetEquipCountByQiFu);
					sevenDayGoalEventObject.Arg1 = num10;
					sevenDayGoalEventObject.Arg2 = num2;
					GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
				}
			}
			else
			{
				LogManager.WriteLog(2, string.Format("使用乾坤袋挖宝时背包满，无法放入物品, RoleID={0}, GoodsID={1}, Binding={2}", client.ClientData.RoleID, num10, binding), null, true);
			}
			string text = string.Format("{0}|{1}|{2}|{3}", new object[]
			{
				num6,
				num7,
				num8,
				num9
			});
			string result;
			if (bMuProject)
			{
				result = string.Format("{0},{1},{2},{3},{4},{5},{6}", new object[]
				{
					num,
					num11,
					binding,
					num6,
					num7,
					num8,
					num9
				});
			}
			else
			{
				result = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
				{
					num,
					num2,
					num3,
					num4,
					num5,
					text
				});
			}
			strRecord = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", new object[]
			{
				num,
				num2,
				num3,
				num4,
				num5,
				text
			});
			if (num > 0)
			{
				EventLogManager.AddRoleQiFuEvent(client, "【{0}】在祈福抽取中获得了【{1}】", new object[]
				{
					client.ClientData.RoleName,
					Global.GetGoodsLogName(new GoodsData
					{
						GoodsID = num,
						ExcellenceInfo = num9
					})
				});
			}
			return result;
		}

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataInfo = null;

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataInfoFree = null;

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataHuoDong = null;

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataHuoDongFree = null;

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataTeQuan = null;

		public static Dictionary<int, Dictionary<int, SystemXmlItem>> m_ImpetrateDataTeQuanFree = null;

		public static object m_mutex = new object();
	}
}
