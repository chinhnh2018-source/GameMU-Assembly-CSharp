using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using GameServer.Server;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	internal class ElementhrtsManager
	{
		public static ElementhrtsManager.RefineType GetRefineType(int Grade)
		{
			ElementhrtsManager.RefineType result = null;
			lock (ElementhrtsManager.RefineTypeDict)
			{
				if (ElementhrtsManager.RefineTypeDict.ContainsKey(Grade))
				{
					result = ElementhrtsManager.RefineTypeDict[Grade];
				}
			}
			return result;
		}

		public static void LoadRefineType()
		{
			string uri = "Config/RefineType.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/RefineType.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.RefineTypeDict)
					{
						ElementhrtsManager.RefineTypeDict.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (null != xelement2)
							{
								ElementhrtsManager.RefineType refineType = new ElementhrtsManager.RefineType();
								refineType.Grade = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								refineType.MinZhuanSheng = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinZhuanSheng", "0"));
								refineType.MinLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MinLevel", "0"));
								refineType.MaxZhuanSheng = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MaxZhuanSheng", "0"));
								refineType.MaxLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MaxLevel", "0"));
								refineType.RefineCost = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RefineCost", "0"));
								refineType.ZuanShiCost = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ZuanShiCost", "0"));
								refineType.SuccessRate = Global.GetSafeAttributeDouble(xelement2, "SuccessRate");
								refineType.RefineLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RefineLevel", "0"));
								ElementhrtsManager.RefineTypeDict[refineType.Grade] = refineType;
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/RefineType.xml时文件出错", ex, true);
				}
			}
		}

		public static List<ElementhrtsManager.ElementHrtsBase> GetElementHrtsBase(int Grade)
		{
			List<ElementhrtsManager.ElementHrtsBase> result = null;
			lock (ElementhrtsManager.ElementHrtsBaseDict)
			{
				if (ElementhrtsManager.ElementHrtsBaseDict.ContainsKey(Grade))
				{
					result = ElementhrtsManager.ElementHrtsBaseDict[Grade];
				}
			}
			return result;
		}

		public static void LoadElementHrtsBase()
		{
			string uri = "Config/Refine.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/RefineType.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.ElementHrtsBaseDict)
					{
						ElementhrtsManager.ElementHrtsBaseDict.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (null != xelement2)
							{
								int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TypeID", "0"));
								List<ElementhrtsManager.ElementHrtsBase> list = new List<ElementhrtsManager.ElementHrtsBase>();
								IEnumerable<XElement> enumerable2 = xelement2.Elements();
								foreach (XElement xml in enumerable2)
								{
									list.Add(new ElementhrtsManager.ElementHrtsBase
									{
										ID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0")),
										GoodsID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "GoodsID", "0")),
										StartValues = Convert.ToInt32(Global.GetDefAttributeStr(xml, "StartValues", "0")),
										EndValues = Convert.ToInt32(Global.GetDefAttributeStr(xml, "EndValues", "0"))
									});
								}
								ElementhrtsManager.ElementHrtsBaseDict[key] = list;
							}
						}
					}
				}
				catch (Exception)
				{
					LogManager.WriteLog(1000, "加载Config/RefineType.xml时出现异常!!!", null, true);
				}
			}
		}

		public static ElementhrtsManager.ElementHrtsLevelInfo GetElementHrtsLevelInfo(int grade, int level)
		{
			string key = grade.ToString() + "|" + level.ToString();
			ElementhrtsManager.ElementHrtsLevelInfo result = null;
			lock (ElementhrtsManager.ElementHrtsLevelDict)
			{
				if (ElementhrtsManager.ElementHrtsLevelDict.ContainsKey(key))
				{
					result = ElementhrtsManager.ElementHrtsLevelDict[key];
				}
			}
			return result;
		}

		public static void LoadElementHrtsLevelInfo()
		{
			string uri = "Config/ElementsHeart.xml";
			GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(uri));
			XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(uri));
			if (null == xelement)
			{
				LogManager.WriteLog(1000, "加载Config/ElementsHeart.xml时出错!!!文件不存在", null, true);
			}
			else
			{
				try
				{
					lock (ElementhrtsManager.ElementHrtsLevelDict)
					{
						ElementhrtsManager.ElementHrtsLevelDict.Clear();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xelement2 in enumerable)
						{
							if (null != xelement2)
							{
								int num = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
								int num2 = 0;
								int num3 = 0;
								IEnumerable<XElement> enumerable2 = xelement2.Elements();
								foreach (XElement xml in enumerable2)
								{
									ElementhrtsManager.ElementHrtsLevelInfo elementHrtsLevelInfo = new ElementhrtsManager.ElementHrtsLevelInfo();
									elementHrtsLevelInfo.Level = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
									if (num2 + 1 != elementHrtsLevelInfo.Level)
									{
										LogManager.WriteLog(1000, string.Format("加载Config/ElementsHeart.xml时出错!!!，{0}, {1}", num, num2), null, true);
										return;
									}
									num2 = elementHrtsLevelInfo.Level;
									elementHrtsLevelInfo.NeedExp = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NeedExp", "0"));
									num3 += elementHrtsLevelInfo.NeedExp;
									elementHrtsLevelInfo.TotalExp = num3;
									string key = num.ToString() + "|" + num2.ToString();
									ElementhrtsManager.ElementHrtsLevelDict[key] = elementHrtsLevelInfo;
								}
							}
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(1000, "加载Config/ElementsHeart.xml时出现异常!!!", ex, true);
				}
			}
		}

		public static int GetSpecialElementHrtsExp(int GoodsID)
		{
			int result = 0;
			lock (ElementhrtsManager.SpecialExpDict)
			{
				if (ElementhrtsManager.SpecialExpDict.ContainsKey(GoodsID))
				{
					result = ElementhrtsManager.SpecialExpDict[GoodsID];
				}
			}
			return result;
		}

		public static void LoadSpecialElementHrtsExp()
		{
			lock (ElementhrtsManager.SpecialExpDict)
			{
				ElementhrtsManager.SpecialExpDict.Clear();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("SpecialElementsHeart");
				if (null == paramValueByName)
				{
					SysConOut.WriteLine("SpecialElementsHeart 不存在，加载失败");
				}
				else
				{
					string[] array = paramValueByName.Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						if (2 != array2.Length)
						{
							SysConOut.WriteLine("加载SpecialElementsHeart时出现异常!!!");
						}
						int key = Convert.ToInt32(array2[0]);
						int value = Convert.ToInt32(array2[1]);
						ElementhrtsManager.SpecialExpDict[key] = value;
					}
				}
			}
		}

		public static bool IsElementHrt(int categoriy)
		{
			return categoriy >= 800 && categoriy < 816;
		}

		public static GoodsData GetElementhrtsByDbID(GameClient client, int Site, int id)
		{
			List<GoodsData> list = null;
			if (3000 == Site)
			{
				list = client.ClientData.ElementhrtsList;
			}
			else if (3001 == Site)
			{
				list = client.ClientData.UsingElementhrtsList;
			}
			GoodsData result;
			if (null == list)
			{
				result = null;
			}
			else
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i].Id == id)
					{
						return list[i];
					}
				}
				result = null;
			}
			return result;
		}

		public static GoodsData GetElementhrtsByDbID(GameClient client, int id)
		{
			if (null != client.ClientData.ElementhrtsList)
			{
				for (int i = 0; i < client.ClientData.ElementhrtsList.Count; i++)
				{
					if (client.ClientData.ElementhrtsList[i].Id == id)
					{
						return client.ClientData.ElementhrtsList[i];
					}
				}
			}
			if (null != client.ClientData.UsingElementhrtsList)
			{
				for (int i = 0; i < client.ClientData.UsingElementhrtsList.Count; i++)
				{
					if (client.ClientData.UsingElementhrtsList[i].Id == id)
					{
						return client.ClientData.UsingElementhrtsList[i];
					}
				}
			}
			return null;
		}

		public static void AddElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 3000)
			{
				if (null == client.ClientData.ElementhrtsList)
				{
					client.ClientData.ElementhrtsList = new List<GoodsData>();
				}
				lock (client.ClientData.ElementhrtsList)
				{
					client.ClientData.ElementhrtsList.Add(goodsData);
				}
			}
		}

		public static void AddUsingElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (goodsData.Site == 3001)
			{
				if (null == client.ClientData.UsingElementhrtsList)
				{
					client.ClientData.UsingElementhrtsList = new List<GoodsData>();
				}
				lock (client.ClientData.UsingElementhrtsList)
				{
					client.ClientData.UsingElementhrtsList.Add(goodsData);
				}
			}
		}

		public static GoodsData AddElementhrtsData(GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewelList, int idelBagIndex, string endTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int nEquipChangeLife)
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
				BagIndex = idelBagIndex,
				AddPropIndex = addPropIndex,
				BornIndex = bornIndex,
				Lucky = lucky,
				Strong = strong,
				ExcellenceInfo = ExcellenceProperty,
				AppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = nEquipChangeLife
			};
			if (3000 == goodsData.Site)
			{
				ElementhrtsManager.AddElementhrtsData(client, goodsData);
			}
			if (3001 == goodsData.Site)
			{
				ElementhrtsManager.AddUsingElementhrtsData(client, goodsData);
			}
			return goodsData;
		}

		public static void RemoveElementhrtsData(GameClient client, GoodsData goodsData)
		{
			if (3000 == goodsData.Site)
			{
				lock (client.ClientData.ElementhrtsList)
				{
					if (null != client.ClientData.ElementhrtsList)
					{
						client.ClientData.ElementhrtsList.Remove(goodsData);
					}
				}
			}
			if (3001 == goodsData.Site)
			{
				lock (client.ClientData.UsingElementhrtsList)
				{
					if (null != client.ClientData.UsingElementhrtsList)
					{
						client.ClientData.UsingElementhrtsList.Remove(goodsData);
					}
				}
			}
		}

		public static int GetIdleSlotOfBag(GameClient client)
		{
			int num = -1;
			int result;
			if (null == client.ClientData.ElementhrtsList)
			{
				result = 0;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.ElementhrtsList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.ElementhrtsList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.ElementhrtsList[i].BagIndex);
					}
				}
				for (int j = 0; j < ElementhrtsManager.GetMaxElementhrtsCount(); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						return j;
					}
				}
				result = num;
			}
			return result;
		}

		public static bool CanAddGoodsNum(GameClient client, int num)
		{
			return client != null && num > 0 && num + client.ClientData.ElementhrtsList.Count <= ElementhrtsManager.GetMaxElementhrtsCount();
		}

		public static int GetIdleSlotOfUsing(GameClient client)
		{
			int num = -1;
			int result;
			if (null == client.ClientData.UsingElementhrtsList)
			{
				result = 0;
			}
			else
			{
				List<int> list = new List<int>();
				for (int i = 0; i < client.ClientData.UsingElementhrtsList.Count; i++)
				{
					if (list.IndexOf(client.ClientData.UsingElementhrtsList[i].BagIndex) < 0)
					{
						list.Add(client.ClientData.UsingElementhrtsList[i].BagIndex);
					}
				}
				for (int j = 0; j < ElementhrtsManager.GetMaxUsingElementhrtsCount(client); j++)
				{
					if (list.IndexOf(j) < 0)
					{
						return j;
					}
				}
				result = num;
			}
			return result;
		}

		public static int GetElementhrtsListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.ElementhrtsList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.ElementhrtsList.Count;
			}
			return result;
		}

		public static int GetUsingElementhrtsListCount(GameClient client)
		{
			int result;
			if (null == client.ClientData.UsingElementhrtsList)
			{
				result = 0;
			}
			else
			{
				result = client.ClientData.UsingElementhrtsList.Count;
			}
			return result;
		}

		public static int GetMaxElementhrtsCount()
		{
			return ElementhrtsManager.MaxElementhrtsGridNum;
		}

		public static int GetMaxUsingElementhrtsCount(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10160");
			return Math.Max(ElementhrtsManager.MaxUsingElementhrtsGridNum, roleParamsInt32FromDB);
		}

		public static void SortElementhrtsList(GameClient client)
		{
			if (null != client.ClientData.ElementhrtsList)
			{
				client.ClientData.ElementhrtsList.Sort(delegate(GoodsData x, GoodsData y)
				{
					int result;
					if (Global.GetEquipGoodsSuitID(y.GoodsID) - Global.GetEquipGoodsSuitID(x.GoodsID) == 0)
					{
						if (x.GoodsID - y.GoodsID == 0)
						{
							result = x.Id - y.Id;
						}
						else
						{
							result = x.GoodsID - y.GoodsID;
						}
					}
					else
					{
						result = Global.GetEquipGoodsSuitID(y.GoodsID) - Global.GetEquipGoodsSuitID(x.GoodsID);
					}
					return result;
				});
				bool flag = false;
				int num = 0;
				foreach (GoodsData goodsData in client.ClientData.ElementhrtsList)
				{
					if (goodsData.BagIndex != num)
					{
						goodsData.BagIndex = num;
						bool flag2 = 0 == 0;
						flag = true;
					}
					num++;
				}
				if (!flag)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(null, Global._TCPManager.TcpOutPacketPool, 725);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
				else
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<GoodsData>>(client.ClientData.ElementhrtsList, Global._TCPManager.TcpOutPacketPool, 725);
					Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true);
				}
			}
		}

		private static void RequestElementHrtList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, GameClient client, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string s = StringUtil.substitute("{0}:{1}", new object[]
			{
				client.ClientData.RoleID,
				3000
			});
			byte[] bytes = new UTF8Encoding().GetBytes(s);
			TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 204, bytes, bytes.Length, out tcpOutPacket, client.ServerId);
			if (TCPProcessCmdResults.RESULT_FAILED != tcpprocessCmdResults && null != tcpOutPacket)
			{
				List<GoodsData> elementhrtsList = DataHelper.BytesToObject<List<GoodsData>>(tcpOutPacket.GetPacketBytes(), 6, tcpOutPacket.PacketDataSize - 6);
				client.ClientData.ElementhrtsList = elementhrtsList;
				Global.PushBackTcpOutPacket(tcpOutPacket);
			}
			if (null == client.ClientData.ElementhrtsList)
			{
				client.ClientData.ElementhrtsList = new List<GoodsData>();
			}
		}

		public static TCPProcessCmdResults RequestElementExtend(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (2 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int cmdData = -3;
				int num2 = Convert.ToInt32(array[1]);
				if (GameFuncControlManager.IsGameFuncDisabled(13))
				{
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int maxUsingElementhrtsCount = ElementhrtsManager.GetMaxUsingElementhrtsCount(gameClient);
				if (num2 != maxUsingElementhrtsCount + 1)
				{
					cmdData = -2;
				}
				else
				{
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("ElementsHeartSlots", '|');
					if (paramValueStringListByName != null && paramValueStringListByName.Count > 0)
					{
						foreach (string str in paramValueStringListByName)
						{
							List<int> list = Global.StringToIntList(str, ',');
							if (list != null && list.Count == 2)
							{
								if (num2 == list[0])
								{
									int num3 = list[1];
									if (num3 > 0)
									{
										if (gameClient.ClientData.UserMoney >= num3)
										{
											if (GameManager.ClientMgr.SubUserMoney(gameClient, num3, "扩展元素之心槽位", true, true, true, true, DaiBiSySType.None))
											{
												EventLogManager.AddRoleEvent(gameClient, OpTypes.ElementhrtsSlotExtend, OpTags.Trace, LogRecordType.IntValue, new object[]
												{
													num2,
													"扩展元素之心槽位"
												});
												cmdData = 0;
												Global.SaveRoleParamsInt32ValueToDB(gameClient, "10160", num2, true);
											}
										}
										else
										{
											cmdData = -10;
										}
									}
									else
									{
										cmdData = -3;
									}
								}
							}
						}
					}
				}
				gameClient.sendCmd<int>(nID, cmdData, false);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_OK;
		}

		public static TCPProcessCmdResults ProcessGetElementHrtList(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (2 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Convert.ToInt32(array[1]);
				if (num2 == 3000)
				{
					byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(gameClient.ClientData.ElementhrtsList);
					GameManager.ClientMgr.SendToClient(gameClient, buffer, nID);
				}
				else if (num2 == 3001)
				{
					byte[] buffer = DataHelper.ObjectToBytes<List<GoodsData>>(gameClient.ClientData.UsingElementhrtsList);
					GameManager.ClientMgr.SendToClient(gameClient, buffer, nID);
				}
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetElementHrtList", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessGetElementHrtsInfo(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (1 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (num2 <= 0)
				{
					num2 = 1;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				int maxUsingElementhrtsCount = ElementhrtsManager.GetMaxUsingElementhrtsCount(client);
				string data2 = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					roleParamsInt32FromDB,
					num2,
					maxUsingElementhrtsCount
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetElementHrtsInfo", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessUseElementHrt(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (3 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				bool flag = num3 > 0;
				GoodsData goodsData = null;
				int num4 = 0;
				int num5 = 0;
				string text2;
				if (flag)
				{
					goodsData = ElementhrtsManager.GetElementhrtsByDbID(gameClient, 3000, num2);
					if (null == goodsData)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (3001 == goodsData.Site)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							3,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (ElementhrtsManager.GetUsingElementhrtsListCount(gameClient) >= ElementhrtsManager.GetMaxUsingElementhrtsCount(gameClient))
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							5,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					ElementhrtsError elementhrtsError = ElementhrtsManager.CheckCanEquipElementHrt(gameClient, goodsData.GoodsID);
					if (ElementhrtsError.Success != elementhrtsError)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							(int)elementhrtsError,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num6 = ElementhrtsManager.GetIdleSlotOfUsing(gameClient);
					if (num6 < 0)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							5,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num4 = 3001;
					num5 = num6;
				}
				else
				{
					goodsData = ElementhrtsManager.GetElementhrtsByDbID(gameClient, 3001, num2);
					if (null == goodsData)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							1,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (3001 != goodsData.Site)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							2,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (ElementhrtsManager.GetElementhrtsListCount(gameClient) >= ElementhrtsManager.GetMaxElementhrtsCount())
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							4,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num6 = ElementhrtsManager.GetIdleSlotOfBag(gameClient);
					if (num6 < 0)
					{
						text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							4,
							num,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					num4 = 3000;
					num5 = num6;
				}
				string[] array2 = null;
				text2 = Global.FormatUpdateDBGoodsStr(new object[]
				{
					num,
					num2,
					"*",
					"*",
					"*",
					"*",
					num4,
					"*",
					"*",
					1,
					"*",
					num5,
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
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10006, text2, out array2, gameClient.ServerId);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						16,
						num2,
						goodsData.Site,
						goodsData.BagIndex,
						0
					});
					GameManager.ClientMgr.SendToClient(gameClient, text2, nID);
					return TCPProcessCmdResults.RESULT_OK;
				}
				if (array2.Length <= 0 || Convert.ToInt32(array2[1]) < 0)
				{
					text2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						16,
						num2,
						goodsData.Site,
						goodsData.BagIndex,
						0
					});
					GameManager.ClientMgr.SendToClient(gameClient, text2, nID);
					return TCPProcessCmdResults.RESULT_OK;
				}
				GoodsData goodsData2 = null;
				if (null != gameClient.ClientData.DamonGoodsDataList)
				{
					lock (gameClient.ClientData.DamonGoodsDataList)
					{
						for (int i = 0; i < gameClient.ClientData.DamonGoodsDataList.Count; i++)
						{
							GoodsData goodsData3 = gameClient.ClientData.DamonGoodsDataList[i];
							if (goodsData3.Using > 0)
							{
								goodsData2 = goodsData3;
								break;
							}
						}
					}
				}
				ElementhrtsManager.RemoveElementhrtsData(gameClient, goodsData);
				goodsData.Site = num4;
				goodsData.BagIndex = num5;
				if (flag)
				{
					ElementhrtsManager.AddUsingElementhrtsData(gameClient, goodsData);
				}
				else
				{
					ElementhrtsManager.AddElementhrtsData(gameClient, goodsData);
				}
				if (null != goodsData2)
				{
					if (Global.RefreshEquipProp(gameClient, goodsData))
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, gameClient);
						GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, gameClient, true, false, 7);
					}
				}
				GameManager.ClientMgr.NotifyModGoods(Global._TCPManager.MySocketListener, pool, gameClient, 3, goodsData.Id, goodsData.Using, goodsData.Site, goodsData.GCount, goodsData.BagIndex, 1);
				string data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					num,
					num2,
					num3,
					num5
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessUseElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static ElementhrtsError CheckCanEquipElementHrt(GameClient client, int GoodsID)
		{
			SystemXmlItem systemXmlItem = null;
			ElementhrtsError result;
			if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodsID, out systemXmlItem))
			{
				result = ElementhrtsError.ErrorConfig;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
				if (!ElementhrtsManager.IsElementHrt(intValue))
				{
					result = ElementhrtsError.CantEquip;
				}
				else if (intValue == 810)
				{
					result = ElementhrtsError.CantEquip;
				}
				else if (null == client.ClientData.UsingElementhrtsList)
				{
					result = ElementhrtsError.Success;
				}
				else
				{
					foreach (GoodsData goodsData in client.ClientData.UsingElementhrtsList)
					{
						if (GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
						{
							if (intValue == systemXmlItem.GetIntValue("Categoriy", -1))
							{
								return ElementhrtsError.SameCategoriy;
							}
						}
					}
					result = ElementhrtsError.Success;
				}
			}
			return result;
		}

		public static TCPProcessCmdResults ProcessGetSomeElementHrts(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (3 != array.Length)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Convert.ToInt32(array[1]);
				bool flag = Convert.ToInt32(array[2]) > 0;
				string data2;
				if (1 != num2 && 10 != num2)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						6,
						num,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (ElementhrtsManager.GetElementhrtsListCount(client) + num2 > ElementhrtsManager.GetMaxElementhrtsCount())
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						7,
						num,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (num2 > 1 && flag)
				{
					data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						9,
						num,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				string text2 = "";
				int num3 = 0;
				for (int i = 0; i < num2; i++)
				{
					int num4 = 0;
					int num5 = 0;
					ElementhrtsError someElementHrts = ElementhrtsManager.GetSomeElementHrts(client, flag, out num4, out num5);
					if (ElementhrtsError.Success != someElementHrts)
					{
						data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							(int)someElementHrts,
							num,
							0,
							0,
							text2
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					text2 += num4;
					text2 += ",";
					text2 += num5;
					text2 += "|";
					num3++;
				}
				LogManager.WriteLog(0, string.Format("玩家抽取获取元素之心 times = {0}, count = {1}", num2, num3), null, true);
				int num6 = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (num6 <= 0)
				{
					num6 = 1;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				data2 = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					num,
					roleParamsInt32FromDB,
					num6,
					text2
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessGetSomeElementHrts", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static ElementhrtsError GetSomeElementHrts(GameClient client, bool bUseMoney, out int GoodsID, out int EhtLevel)
		{
			GoodsID = 0;
			EhtLevel = 0;
			try
			{
				int num = Global.GetRoleParamsInt32FromDB(client, "ElementGrade");
				if (num <= 0)
				{
					num = 1;
				}
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				if (bUseMoney)
				{
					num = ElementhrtsManager.ZhuanShiGrade;
				}
				ElementhrtsManager.RefineType refineType = ElementhrtsManager.GetRefineType(num);
				if (null == refineType)
				{
					return ElementhrtsError.ErrorConfig;
				}
				if (client.ClientData.Level < refineType.MinLevel)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.Level > refineType.MaxLevel)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.ChangeLifeCount < refineType.MinZhuanSheng)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (client.ClientData.ChangeLifeCount > refineType.MaxZhuanSheng)
				{
					return ElementhrtsError.ErrorLevel;
				}
				if (ElementhrtsManager.GetElementhrtsListCount(client) >= ElementhrtsManager.GetMaxElementhrtsCount())
				{
					return ElementhrtsError.BagNotEnough;
				}
				if (refineType.RefineCost > 0)
				{
					if (roleParamsInt32FromDB < refineType.RefineCost)
					{
						return ElementhrtsError.PowderNotEnough;
					}
				}
				if (refineType.ZuanShiCost > 0)
				{
					if (client.ClientData.UserMoney < refineType.ZuanShiCost)
					{
						return ElementhrtsError.MoneyNotEnough;
					}
				}
				if (refineType.ZuanShiCost > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, refineType.ZuanShiCost, "获取元素之心", true, true, false, DaiBiSySType.None))
					{
						return ElementhrtsError.MoneyNotEnough;
					}
				}
				if (refineType.RefineCost > 0)
				{
					GameManager.ClientMgr.ModifyYuanSuFenMoValue(client, -refineType.RefineCost, "获取元素", true, false);
				}
				List<ElementhrtsManager.ElementHrtsBase> elementHrtsBase = ElementhrtsManager.GetElementHrtsBase(num);
				if (elementHrtsBase == null || elementHrtsBase.Count <= 0)
				{
					return ElementhrtsError.ErrorConfig;
				}
				int randomNumber = Global.GetRandomNumber(1, 100001);
				foreach (ElementhrtsManager.ElementHrtsBase elementHrtsBase2 in elementHrtsBase)
				{
					if (randomNumber >= elementHrtsBase2.StartValues && randomNumber <= elementHrtsBase2.EndValues)
					{
						GoodsID = elementHrtsBase2.GoodsID;
						break;
					}
				}
				LogManager.WriteLog(0, string.Format("获取元素之心随机数: grade = {0}, random = {1}, GoodsID = {2}", num, randomNumber, GoodsID), null, true);
				if (0 == GoodsID)
				{
					GoodsID = elementHrtsBase[0].GoodsID;
					LogManager.WriteLog(2, string.Format("获取元素之心获得配置异常: grade = {0}, random = {1}", num, randomNumber), null, true);
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(GoodsID, out systemXmlItem))
				{
					return ElementhrtsError.ErrorConfig;
				}
				if (null == systemXmlItem)
				{
					LogManager.WriteLog(2, string.Format("GetSomeElementHrts: (null == systemGoods) GoodsID={0}", GoodsID), null, true);
					return ElementhrtsError.ErrorConfig;
				}
				string stringValue = systemXmlItem.GetStringValue("EquipProps");
				int intValue = systemXmlItem.GetIntValue("SuitID", -1);
				int num2 = 1;
				int intValue2 = systemXmlItem.GetIntValue("Categoriy", -1);
				List<int> list = new List<int>();
				list.Add(num2);
				list.Add(0);
				Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, GoodsID, 1, 0, "", 0, 1, 3000, "", false, 1, "获取元素之心", "1900-01-01 12:00:00", 0, 0, 0, 0, 0, 0, 0, null, list, 0, true);
				EhtLevel = num2;
				int randomNumber2 = Global.GetRandomNumber(0, 100);
				if ((double)randomNumber2 <= refineType.SuccessRate * 100.0)
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "ElementGrade", refineType.RefineLevel, true);
				}
				else
				{
					Global.SaveRoleParamsInt32ValueToDB(client, "ElementGrade", 1, true);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "GetSomeElementHrts", false, false);
			}
			return ElementhrtsError.Success;
		}

		public static TCPProcessCmdResults ProcessPowerElementHrt(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 3)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient gameClient = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, gameClient, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num2 = Convert.ToInt32(array[1]);
				int num3 = Convert.ToInt32(array[2]);
				List<int> list = new List<int>();
				string[] array2 = array[3].Split(new char[]
				{
					'|'
				});
				for (int i = 0; i < array2.Length; i++)
				{
					if (array2[i] != "")
					{
						list.Add(Convert.ToInt32(array2[i]));
					}
				}
				if (list.Count <= 0)
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (list.IndexOf(num2) >= 0)
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				GoodsData elementhrtsByDbID = ElementhrtsManager.GetElementhrtsByDbID(gameClient, num3, num2);
				if (null == elementhrtsByDbID)
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(elementhrtsByDbID.GoodsID, out systemXmlItem))
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						15,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
				if (!ElementhrtsManager.IsElementHrt(intValue))
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						9,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				if (intValue == 810)
				{
					string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
					{
						13,
						num,
						0,
						0,
						0,
						0
					});
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
					return TCPProcessCmdResults.RESULT_DATA;
				}
				int j = 0;
				foreach (int id in list)
				{
					GoodsData elementhrtsByDbID2 = ElementhrtsManager.GetElementhrtsByDbID(gameClient, id);
					if (null == elementhrtsByDbID2)
					{
						string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							num,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int num4 = 0;
					SystemXmlItem systemXmlItem2 = null;
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(elementhrtsByDbID2.GoodsID, out systemXmlItem2))
					{
						string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							num,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					int intValue2 = systemXmlItem2.GetIntValue("SuitID", -1);
					if (systemXmlItem2.GetIntValue("Categoriy", -1) == 810)
					{
						num4 = ElementhrtsManager.GetSpecialElementHrtsExp(elementhrtsByDbID2.GoodsID);
					}
					else if (elementhrtsByDbID2.ElementhrtsProps != null && elementhrtsByDbID2.ElementhrtsProps.Count >= 2)
					{
						ElementhrtsManager.ElementHrtsLevelInfo elementHrtsLevelInfo = ElementhrtsManager.GetElementHrtsLevelInfo(systemXmlItem2.GetIntValue("SuitID", -1), elementhrtsByDbID2.ElementhrtsProps[0]);
						if (null != elementHrtsLevelInfo)
						{
							num4 = elementHrtsLevelInfo.TotalExp + elementhrtsByDbID2.ElementhrtsProps[1];
						}
					}
					j += num4;
				}
				foreach (int id in list)
				{
					GoodsData elementhrtsByDbID2 = ElementhrtsManager.GetElementhrtsByDbID(gameClient, id);
					if (null == elementhrtsByDbID2)
					{
						string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							num,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
					if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, tcpClientPool, pool, gameClient, elementhrtsByDbID2, 1, false, false))
					{
						string data2 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
						{
							15,
							num,
							0,
							0,
							0,
							0
						});
						tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data2, nID);
						return TCPProcessCmdResults.RESULT_DATA;
					}
				}
				int intValue3 = systemXmlItem.GetIntValue("SuitID", -1);
				int num5 = 1;
				int num6 = 0;
				if (elementhrtsByDbID.ElementhrtsProps != null && elementhrtsByDbID.ElementhrtsProps.Count >= 2)
				{
					num5 = elementhrtsByDbID.ElementhrtsProps[0];
					num6 = elementhrtsByDbID.ElementhrtsProps[1];
				}
				while (j > 0)
				{
					ElementhrtsManager.ElementHrtsLevelInfo elementHrtsLevelInfo2 = ElementhrtsManager.GetElementHrtsLevelInfo(intValue3, num5 + 1);
					if (null == elementHrtsLevelInfo2)
					{
						break;
					}
					int num7 = Global.GMax(0, elementHrtsLevelInfo2.NeedExp - num6);
					if (num7 < 0)
					{
						break;
					}
					if (num7 > j)
					{
						num6 += j;
						j = 0;
					}
					else
					{
						num5++;
						num6 = 0;
						j -= num7;
					}
				}
				UpdateGoodsArgs updateGoodsArgs = new UpdateGoodsArgs
				{
					RoleID = gameClient.ClientData.RoleID,
					DbID = num2,
					WashProps = null
				};
				updateGoodsArgs.ElementhrtsProps = new List<int>();
				updateGoodsArgs.ElementhrtsProps.Add(num5);
				updateGoodsArgs.ElementhrtsProps.Add(num6);
				GoodsData goodsData = null;
				if (null != gameClient.ClientData.DamonGoodsDataList)
				{
					lock (gameClient.ClientData.DamonGoodsDataList)
					{
						for (int i = 0; i < gameClient.ClientData.DamonGoodsDataList.Count; i++)
						{
							GoodsData goodsData2 = gameClient.ClientData.DamonGoodsDataList[i];
							if (goodsData2.Using > 0)
							{
								goodsData = goodsData2;
								break;
							}
						}
					}
				}
				bool flag2 = false;
				int site = elementhrtsByDbID.Site;
				if (null != goodsData)
				{
					if (3001 == elementhrtsByDbID.Site)
					{
						elementhrtsByDbID.Site = 3000;
						flag2 = Global.RefreshEquipProp(gameClient, elementhrtsByDbID);
						elementhrtsByDbID.Site = site;
					}
				}
				Global.UpdateGoodsProp(gameClient, elementhrtsByDbID, updateGoodsArgs, true);
				if (null != goodsData)
				{
					if (flag2 && Global.RefreshEquipProp(gameClient, elementhrtsByDbID))
					{
						GameManager.ClientMgr.NotifyUpdateEquipProps(tcpMgr.MySocketListener, pool, gameClient);
						GameManager.ClientMgr.NotifyOthersLifeChanged(tcpMgr.MySocketListener, pool, gameClient, true, false, 7);
					}
				}
				string data3 = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					0,
					num,
					num2,
					num3,
					num5,
					num6
				});
				tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data3, nID);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessPowerElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static TCPProcessCmdResults ProcessResetElementHrtBag(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPRandKey tcpRandKey, TCPOutPacketPool pool, int nID, byte[] data, int count, out TCPOutPacket tcpOutPacket)
		{
			tcpOutPacket = null;
			string text = null;
			try
			{
				text = new UTF8Encoding().GetString(data, 0, count);
			}
			catch (Exception)
			{
				LogManager.WriteLog(2, string.Format("解析指令字符串错误, CMD={0}, Client={1}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false)), null, true);
				return TCPProcessCmdResults.RESULT_FAILED;
			}
			try
			{
				string[] array = text.Split(new char[]
				{
					':'
				});
				if (array.Length < 1)
				{
					LogManager.WriteLog(2, string.Format("指令参数个数错误, CMD={0}, Client={1}, Recv={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), array.Length), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				int num = Convert.ToInt32(array[0]);
				GameClient client = GameManager.ClientMgr.FindClient(socket);
				if (KuaFuManager.getInstance().ClientCmdCheckFaild(nID, client, ref num))
				{
					LogManager.WriteLog(2, string.Format("根据RoleID定位GameClient对象失败, CMD={0}, Client={1}, RoleID={2}", (TCPGameServerCmds)nID, Global.GetSocketRemoteEndPoint(socket, false), num), null, true);
					return TCPProcessCmdResults.RESULT_FAILED;
				}
				ElementhrtsManager.SortElementhrtsList(client);
				return TCPProcessCmdResults.RESULT_OK;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "ProcessPowerElementHrt", false, false);
			}
			return TCPProcessCmdResults.RESULT_DATA;
		}

		public static int MaxElementhrtsGridNum = 100;

		public static int MaxUsingElementhrtsGridNum = 8;

		public static int ZhuanShiGrade = 6;

		private static Dictionary<int, ElementhrtsManager.RefineType> RefineTypeDict = new Dictionary<int, ElementhrtsManager.RefineType>();

		private static Dictionary<int, List<ElementhrtsManager.ElementHrtsBase>> ElementHrtsBaseDict = new Dictionary<int, List<ElementhrtsManager.ElementHrtsBase>>();

		private static Dictionary<string, ElementhrtsManager.ElementHrtsLevelInfo> ElementHrtsLevelDict = new Dictionary<string, ElementhrtsManager.ElementHrtsLevelInfo>();

		private static Dictionary<int, int> SpecialExpDict = new Dictionary<int, int>();

		public class RefineType
		{
			public int Grade;

			public int MinZhuanSheng;

			public int MinLevel;

			public int MaxZhuanSheng;

			public int MaxLevel;

			public int RefineCost;

			public int ZuanShiCost;

			public double SuccessRate;

			public int RefineLevel;
		}

		public class ElementHrtsBase
		{
			public int ID;

			public int GoodsID;

			public int StartValues;

			public int EndValues;
		}

		public class ElementHrtsLevelInfo
		{
			public int Level;

			public int NeedExp;

			public int TotalExp;
		}
	}
}
