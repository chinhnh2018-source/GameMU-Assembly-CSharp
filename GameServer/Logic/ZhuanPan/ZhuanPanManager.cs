using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Logic.Reborn;
using GameServer.Server;
using GameServer.Tools;
using Server.Tools;

namespace GameServer.Logic.ZhuanPan
{
	public class ZhuanPanManager : IManager, ICmdProcessorEx, ICmdProcessor
	{
		public static ZhuanPanManager getInstance()
		{
			return ZhuanPanManager.instance;
		}

		public bool initialize()
		{
			this.LoadConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1810, 1, 1, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1811, 2, 2, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1813, 1, 1, ZhuanPanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
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

		public bool LoadConfig()
		{
			try
			{
				if (!this.LoadSystemParams())
				{
					return false;
				}
				if (!this.LoadZhuanPan())
				{
					return false;
				}
				if (!this.LoadZhuanPanAward())
				{
					return false;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("转盘系统读取配置表出错", new object[0]), null, true);
			}
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1810:
					return this.ProcessZhuanPanInfoCmd(client, nID, bytes, cmdParams);
				case 1811:
					return this.ProcessZhuanPanChouJiangCmd(client, nID, bytes, cmdParams);
				case 1813:
					return this.ProcessZhuanPanLingJiangCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public bool ProcessZhuanPanInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				List<ZhuanPanItem> list = new List<ZhuanPanItem>();
				DateTime dateTime = Global.GetRoleParamsDateTimeFromDB(client, "10155");
				DateTime freeTime = DateTime.MaxValue;
				int num2 = Global.GetRoleParamsInt32FromDB(client, "10156");
				if (dateTime < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
				{
					dateTime = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
					Global.SaveRoleParamsDateTimeToDB(client, "10155", dateTime, true);
				}
				int num3 = Global.GetRoleParamsInt32FromDB(client, "10162") - 1;
				ZhuanPanMainData cmdData = null;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					list = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList;
					int zhuanPanFree = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree;
					int[] array = new int[ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count * 2];
					for (int i = 0; i < ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count; i++)
					{
						array[i * 2] = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[i][0];
						array[i * 2 + 1] = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[i][1];
					}
					if (zhuanPanFree > 0)
					{
						freeTime = dateTime.AddHours((double)zhuanPanFree);
					}
					if (num2 < 1 || num2 > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi)
					{
						num2 = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
						Global.SaveRoleParamsInt32ValueToDB(client, "10156", num2, true);
					}
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					ZhuanPanItem zhuanPanItem;
					if (roleParamsDateTimeFromDB < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						zhuanPanItem = null;
						num2 = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
						Global.SaveRoleParamsInt32ValueToDB(client, "10156", num2, true);
					}
					else
					{
						zhuanPanItem = ((num3 < 0 || num3 >= list.Count) ? null : list[num3]);
					}
					ZhuanPanItem goodsAward = null;
					if (zhuanPanItem != null)
					{
						int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10166");
						string[] array2 = zhuanPanItem.GoodsID.Split(new char[]
						{
							','
						});
						array2[2] = roleParamsInt32FromDB.ToString();
						goodsAward = new ZhuanPanItem
						{
							ID = zhuanPanItem.ID,
							GoodsID = string.Join(",", array2),
							AwardLevel = zhuanPanItem.AwardLevel,
							GongGao = zhuanPanItem.GongGao,
							AwardLabel = zhuanPanItem.AwardLabel
						};
					}
					cmdData = new ZhuanPanMainData
					{
						ZhuanPanAwardItemList = list,
						FreeTime = freeTime,
						LeftFuLiCount = num2,
						ZhuanPanCostArray = array,
						GoodsAward = goodsAward,
						GongGaoList = ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList
					};
				}
				client.sendCmd<ZhuanPanMainData>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessZhuanPanChouJiangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 2))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				DateTime dateTime = TimeUtil.NowDateTime();
				ZhuanPanChouJiangData zhuanPanChouJiangData = new ZhuanPanChouJiangData();
				List<ZhuanPanItem> list = new List<ZhuanPanItem>();
				int num3 = 0;
				int num4 = 1;
				zhuanPanChouJiangData.AwardType = num2;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					int zhuanPanFree = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree;
					int num5 = num2 - 1;
					if (num5 < 0 || num5 >= ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray.Count)
					{
						zhuanPanChouJiangData.Result = -200;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					int num6 = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[num5][0];
					int num7 = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray[num5][1];
					if (num6 <= 0 || num7 <= 0)
					{
						zhuanPanChouJiangData.Result = -200;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					int zhuanPanZuanShiFuLi = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
					Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> zhuanPanAwardXmlDict = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanAwardXmlDict;
					list = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList;
					if (dateTime < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime || dateTime > ZhuanPanManager.ZhuanPanRunTimeData.EndTime)
					{
						zhuanPanChouJiangData.Result = -100;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					DateTime dateTime2 = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					if (dateTime2 < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						dateTime2 = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
						Global.SaveRoleParamsDateTimeToDB(client, "10165", dateTime2, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10162", 0, true);
					}
					DateTime dateTime3 = DateTime.MaxValue;
					if (zhuanPanFree > 0)
					{
						dateTime3 = Global.GetRoleParamsDateTimeFromDB(client, "10155").AddHours((double)zhuanPanFree);
					}
					if (!Global.CanAddGoodsNum(client, 1) || !RebornEquip.CanAddGoodsNum(client, 1))
					{
						zhuanPanChouJiangData.Result = -4;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					num3 = Global.GetRoleParamsInt32FromDB(client, "10162");
					if (num3 > 0)
					{
						zhuanPanChouJiangData.Result = -202;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					Dictionary<int, ZhuanPanAwardItem> dictionary = null;
					if (!zhuanPanAwardXmlDict.TryGetValue(num2, out dictionary))
					{
						zhuanPanChouJiangData.Result = -101;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					int num8 = Global.GetRoleParamsInt32FromDB(client, "10156");
					zhuanPanChouJiangData.LeftFuLiCount = num8;
					zhuanPanChouJiangData.FreeTime = dateTime3;
					bool flag2 = false;
					if (num2 == 3)
					{
						if (!zhuanPanAwardXmlDict.ContainsKey(4))
						{
							zhuanPanChouJiangData.Result = -101;
							client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
							return true;
						}
						if (dateTime > dateTime3)
						{
							zhuanPanChouJiangData.FreeTime = dateTime.AddHours((double)zhuanPanFree);
							flag2 = true;
						}
					}
					if (!flag2)
					{
						if (!MoneyUtil.CheckHasMoney(client, num6, num7))
						{
							zhuanPanChouJiangData.Result = -num2;
							client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
							return true;
						}
						string text = "";
						if (!MoneyUtil.CostMoney(client, num6, num7, ref text, "转盘抽奖", true))
						{
							zhuanPanChouJiangData.Result = -num2;
							client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
							return true;
						}
					}
					if (num2 == 3)
					{
						if (dateTime > dateTime3)
						{
							Global.SaveRoleParamsDateTimeToDB(client, "10155", dateTime, true);
							zhuanPanChouJiangData.FreeTime = dateTime.AddHours((double)zhuanPanFree);
						}
						else
						{
							num4 = 0;
							num8--;
							if (num8 < 1)
							{
								if (!zhuanPanAwardXmlDict.TryGetValue(4, out dictionary))
								{
									zhuanPanChouJiangData.Result = -101;
									client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
									return true;
								}
								num8 = zhuanPanZuanShiFuLi;
								num2 = 4;
							}
							if (num8 > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi)
							{
								num8 = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi;
							}
							Global.SaveRoleParamsInt32ValueToDB(client, "10156", num8, true);
							zhuanPanChouJiangData.LeftFuLiCount = num8;
						}
					}
					int randomNumber = Global.GetRandomNumber(1, 100000);
					foreach (KeyValuePair<int, ZhuanPanAwardItem> keyValuePair in dictionary)
					{
						if (randomNumber >= keyValuePair.Value.StartValue && randomNumber <= keyValuePair.Value.EndValue)
						{
							num3 = keyValuePair.Key;
						}
					}
					if (list.Count < num3 || num3 <= 0)
					{
						LogManager.WriteLog(1000, string.Format("转盘抽奖随机出的awardID={0}找不到对应的奖励配置", num3), null, true);
						zhuanPanChouJiangData.Result = -201;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					zhuanPanChouJiangData.Result = 1;
					ZhuanPanItem zhuanPanItem = list[num3 - 1];
					SystemXmlItem systemXmlItem = null;
					int num9 = Convert.ToInt32(zhuanPanItem.GoodsID.Split(new char[]
					{
						','
					})[0]);
					if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num9, out systemXmlItem))
					{
						LogManager.WriteLog(1000, string.Format("转盘抽奖随机出的goodID={0}道具表中不存在", num9), null, true);
						string textMsg = string.Format("系统中不存在{0}", num9);
						GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
						zhuanPanChouJiangData.Result = -201;
						client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
						return true;
					}
					string stringValue = systemXmlItem.GetStringValue("Title");
					if (num2 == 3 && num4 > 0)
					{
						num2 = 4;
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, stringValue, "转盘抽奖_类型：" + num2, client.ClientData.RoleName, "系统", "修改", -1, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
					string[] array = zhuanPanItem.GoodsID.Split(new char[]
					{
						','
					});
					array[2] = num4.ToString();
					zhuanPanChouJiangData.GoodsItem = new ZhuanPanItem
					{
						ID = zhuanPanItem.ID,
						GoodsID = string.Join(",", array),
						AwardLevel = zhuanPanItem.AwardLevel,
						GongGao = zhuanPanItem.GongGao,
						AwardLabel = zhuanPanItem.AwardLevel
					};
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10162", num3, true);
				Global.SaveRoleParamsDateTimeToDB(client, "10165", dateTime, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "10166", num4, true);
				zhuanPanChouJiangData.AwardType = num2;
				client.sendCmd<ZhuanPanChouJiangData>(nID, zhuanPanChouJiangData, false);
				client._IconStateMgr.CheckFreeZhuanPanChouState(client);
				client._IconStateMgr.SendIconStateToClient(client);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessZhuanPanLingJiangCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				int num = Convert.ToInt32(cmdParams[0]);
				ZhuanPanItem zhuanPanItem = null;
				int num2 = Global.GetRoleParamsInt32FromDB(client, "10162") - 1;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "10165");
					if (roleParamsDateTimeFromDB < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime)
					{
						client.sendCmd(nID, "-100", false);
						return true;
					}
					if (num2 < 0 || num2 > ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList.Count)
					{
						client.sendCmd(nID, "-101", false);
						return true;
					}
					zhuanPanItem = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList[num2];
				}
				if (!Global.CanAddGoodsNum(client, 1))
				{
					client.sendCmd(nID, "-4", false);
					return true;
				}
				string[] array = zhuanPanItem.GoodsID.Split(new char[]
				{
					','
				});
				int num3 = Convert.ToInt32(array[0]);
				int goodsNum = Convert.ToInt32(array[1]);
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10166");
				int forgeLevel = Convert.ToInt32(array[3]);
				int nAppendPropLev = Convert.ToInt32(array[4]);
				int lucky = Convert.ToInt32(array[5]);
				int excellenceProperty = Convert.ToInt32(array[6]);
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(num3, out systemXmlItem))
				{
					string textMsg = string.Format("系统中不存在{0}", num3);
					GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
					client.sendCmd(nID, "-201", false);
					return true;
				}
				int site = 0;
				int intValue = systemXmlItem.GetIntValue("Categoriy", -1);
				if (intValue >= 800 && intValue < 816)
				{
					site = 3000;
				}
				else if (intValue == 901)
				{
					site = 7000;
				}
				else if (intValue >= 910 && intValue <= 928)
				{
					site = 8000;
				}
				else if (intValue == 940)
				{
					site = 11000;
				}
				else if (intValue >= 980 && intValue <= 981)
				{
					site = 16000;
				}
				Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, num3, goodsNum, 0, "", forgeLevel, roleParamsInt32FromDB, site, "", true, 1, "转盘抽奖", "1900-01-01 12:00:00", 0, 0, lucky, 0, excellenceProperty, nAppendPropLev, 0, null, null, 0, true);
				if (zhuanPanItem.GongGao == 1)
				{
					ZhuanPanGongGaoData zhuanPanGongGaoData = new ZhuanPanGongGaoData
					{
						ZoneId = client.ClientData.ZoneID,
						Rid = client.ClientData.RoleID,
						RoleName = client.ClientData.RoleName,
						GoodsId = zhuanPanItem.GoodsID,
						GoodsIndex = num2 + 1
					};
					int num4 = 0;
					GameClient nextClient;
					while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num4, false)) != null)
					{
						nextClient.sendCmd<ZhuanPanGongGaoData>(1812, zhuanPanGongGaoData, false);
					}
					lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
					{
						if (null == ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList = new List<ZhuanPanGongGaoData>();
						}
						while (ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Count >= 20)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.RemoveAt(0);
						}
						ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Add(zhuanPanGongGaoData);
					}
				}
				Global.SaveRoleParamsInt32ValueToDB(client, "10162", 0, true);
				client.sendCmd(nID, "1", false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool LoadSystemParams()
		{
			bool result;
			try
			{
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ZhuanPanCost");
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanConstArray = ConfigParser.ParserIntArrayList(paramValueByName, true, '|', ',');
					int zhuanPanFree = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanFree"));
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanFree = zhuanPanFree;
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanZuanShiFuLi = Convert.ToInt32(GameManager.systemParamsList.GetParamValueByName("ZhuanPanZuanShiFuLi"));
					string paramValueByName2 = GameManager.systemParamsList.GetParamValueByName("ZhuanPanTime");
					if (paramValueByName2 == "" || null == paramValueByName2)
					{
						result = false;
					}
					else
					{
						string[] array = paramValueByName2.Split(new char[]
						{
							','
						});
						if (!DateTime.TryParse(array[0], out ZhuanPanManager.ZhuanPanRunTimeData.BeginTime) || !DateTime.TryParse(array[1], out ZhuanPanManager.ZhuanPanRunTimeData.EndTime))
						{
							result = false;
						}
						else
						{
							ZhuanPanManager.ZhuanPanRunTimeData.EndTime.AddDays(1.0);
							DateTime t = TimeUtil.NowDateTime();
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = (t < ZhuanPanManager.ZhuanPanRunTimeData.BeginTime || t > ZhuanPanManager.ZhuanPanRunTimeData.EndTime);
							result = true;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("转盘系统读取配置表出错，出错文件 SystemParams.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		public bool LoadZhuanPan()
		{
			bool result;
			try
			{
				string filePath = Global.GameResPath("Config\\ZhuanPan.xml");
				XElement xelement = CheckHelper.LoadXml(filePath, true);
				if (null == xelement)
				{
					result = false;
				}
				else
				{
					List<ZhuanPanItem> list = new List<ZhuanPanItem>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					if (null == enumerable)
					{
						result = false;
					}
					else
					{
						foreach (XElement xelement2 in enumerable)
						{
							if (xelement2 != null)
							{
								ZhuanPanItem item = new ZhuanPanItem
								{
									ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0")),
									GoodsID = Global.GetDefAttributeStr(xelement2, "GoodsID", ""),
									AwardLevel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AwardLevel", "0")),
									GongGao = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "GongGao", "0")),
									AwardLabel = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "AwardLabel", "0"))
								};
								list.Add(item);
							}
						}
						lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanItemXmlList = list;
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("转盘系统读取配置表出错，出错文件 ZhuanPan.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		public bool LoadZhuanPanAward()
		{
			bool result;
			try
			{
				string filePath = Global.GameResPath("Config\\ZhuanPanAward.xml");
				XElement xelement = CheckHelper.LoadXml(filePath, true);
				if (null == xelement)
				{
					result = false;
				}
				else
				{
					Dictionary<int, Dictionary<int, ZhuanPanAwardItem>> dictionary = new Dictionary<int, Dictionary<int, ZhuanPanAwardItem>>();
					IEnumerable<XElement> enumerable = xelement.Elements("AwardLevel");
					if (null == enumerable)
					{
						result = false;
					}
					else
					{
						foreach (XElement xelement2 in enumerable)
						{
							int key = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "TypeID", "0"));
							IEnumerable<XElement> enumerable2 = xelement2.Elements("Award");
							if (null == enumerable2)
							{
								return false;
							}
							Dictionary<int, ZhuanPanAwardItem> dictionary2 = new Dictionary<int, ZhuanPanAwardItem>();
							foreach (XElement xml in enumerable2)
							{
								int key2 = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
								ZhuanPanAwardItem value = new ZhuanPanAwardItem
								{
									StartValue = Convert.ToInt32(Global.GetDefAttributeStr(xml, "StarValue", "0")),
									EndValue = Convert.ToInt32(Global.GetDefAttributeStr(xml, "EndValue", "0"))
								};
								dictionary2.Add(key2, value);
							}
							dictionary.Add(key, dictionary2);
						}
						lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
						{
							ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanAwardXmlDict = dictionary;
						}
						result = true;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("转盘系统读取配置表出错，出错文件 ZhuanPanAward.xml ex:" + ex.Message, new object[0]), null, true);
				result = false;
			}
			return result;
		}

		public void ZhuanPanTimer_Work()
		{
			DateTime t = TimeUtil.NowDateTime();
			bool flag = false;
			DateTime t2 = DateTime.MaxValue;
			DateTime t3 = DateTime.MinValue;
			lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
			{
				flag = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen;
				t2 = ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
				t3 = ZhuanPanManager.ZhuanPanRunTimeData.EndTime;
			}
			if (flag && (t > t3 || t < t2))
			{
				GameManager.ClientMgr.NotifyAllActivityState(10, 0, t2.ToString("yyyyMMddHHmmss"), t3.ToString("yyyyMMddHHmmss"), 0);
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = false;
					ZhuanPanManager.ZhuanPanRunTimeData.GongGaoList.Clear();
				}
			}
			else if (!flag && t > t2 && t < t3)
			{
				GameManager.ClientMgr.NotifyAllActivityState(10, 1, t2.ToString("yyyyMMddHHmmss"), t3.ToString("yyyyMMddHHmmss"), 0);
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen = true;
				}
			}
		}

		public void NotifyActivityState(GameClient client)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				bool flag = false;
				lock (ZhuanPanManager.ZhuanPanRunTimeData.Mutex)
				{
					flag = ZhuanPanManager.ZhuanPanRunTimeData.ZhuanPanOpen;
				}
				if (flag)
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						10,
						1,
						"",
						0,
						0
					});
					client.sendCmd(770, cmdData, false);
				}
			}
		}

		public DateTime GetBeginTime()
		{
			return ZhuanPanManager.ZhuanPanRunTimeData.BeginTime;
		}

		private static ZhuanPanData ZhuanPanRunTimeData = new ZhuanPanData();

		private static ZhuanPanManager instance = new ZhuanPanManager();
	}
}
