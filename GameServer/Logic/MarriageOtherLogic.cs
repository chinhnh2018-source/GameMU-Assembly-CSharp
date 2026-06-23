using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	internal class MarriageOtherLogic : ICmdProcessorEx, ICmdProcessor, IEventListener
	{
		public static MarriageOtherLogic getInstance()
		{
			return MarriageOtherLogic.instance;
		}

		public void init()
		{
			try
			{
				this.dNeedGam = GameManager.systemParamsList.GetParamValueDoubleArrayByName("XianHuaCost", ',');
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic XianHuaCost", false, false);
			}
			try
			{
				this.dRingmodulus = GameManager.systemParamsList.GetParamValueDoubleByName("GoodWillXiShu", 0.0);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic GoodWillXiShu", false, false);
			}
			try
			{
				this.dOtherRingmodulus = GameManager.systemParamsList.GetParamValueDoubleByName("BanLvXiShu", 0.0);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic BanLvXiShu", false, false);
			}
			try
			{
				this.WeddingRingDic.LoadFromXMlFile("Config/WeddingRing.xml", "", "GoodsID", 0);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic WeddingRing.xml", false, false);
			}
			try
			{
				SystemXmlItems systemXmlItems = new SystemXmlItems();
				systemXmlItems.LoadFromXMlFile("Config/GiveRose.xml", "", "GoodsID", 0);
				foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in systemXmlItems.SystemXmlItemDict)
				{
					MarriageRoseData marriageRoseData = new MarriageRoseData();
					marriageRoseData.nBaseAddGoodWill = keyValuePair.Value.GetIntValue("GoodWill", -1);
					string[] array = keyValuePair.Value.GetStringValue("MultiplyingPower").Split(new char[]
					{
						'|'
					});
					int num = 0;
					for (int i = 0; i < array.Length; i++)
					{
						string[] array2 = array[i].Split(new char[]
						{
							','
						});
						num += (int)(Convert.ToDouble(array2[1]) * 100.0);
						marriageRoseData.modulusList.Add(Convert.ToInt32(array2[0]));
						marriageRoseData.rateList.Add(num);
					}
					this.RoseDataDic.Add(Convert.ToInt32(keyValuePair.Value.GetIntValue("GoodsID", -1)), marriageRoseData);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic GiveRose.xml", false, false);
			}
			try
			{
				SystemXmlItems systemXmlItems2 = new SystemXmlItems();
				systemXmlItems2.LoadFromXMlFile("Config/GoodWill.xml", "", "Type", 0);
				sbyte b = 0;
				int num2 = 0;
				this.GoodwillAllExpList.Add(0);
				foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in systemXmlItems2.SystemXmlItemDict)
				{
					b = 0;
					foreach (XElement xelement in keyValuePair.Value.XMLNode.Descendants())
					{
						num2 += Convert.ToInt32(xelement.Attribute("NeedGoodWill").Value);
						this.GoodwillAllExpList.Add(num2);
						b += 1;
					}
				}
				this.byMaxGoodwillLv = (sbyte)((this.GoodwillAllExpList.Count - 1) / (int)b);
				this.byMaxGoodwillStar = b;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "init marryotherlogic GoodWill.xml", false, false);
			}
			TCPCmdDispatcher.getInstance().registerProcessorEx(871, 1, 1, MarriageOtherLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(872, 1, 1, MarriageOtherLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(873, 1, 1, MarriageOtherLogic.getInstance(), TCPCmdFlags.IsStringArrayParams);
		}

		public void destroy()
		{
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 871:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				if (!MarryLogic.IsVersionSystemOpenOfMarriage())
				{
					client.sendCmd<int>(nID, 11, false);
				}
				else
				{
					try
					{
						int nGoodsDBId = Global.SafeConvertToInt32(cmdParams[0]);
						int cmdData = (int)this.GiveRose(client, nGoodsDBId);
						client.sendCmd<int>(nID, cmdData, false);
					}
					catch (Exception e)
					{
						DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_MARRY_ROSE", false, false);
					}
				}
				break;
			case 872:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				if (!MarryLogic.IsVersionSystemOpenOfMarriage())
				{
					client.sendCmd<int>(nID, 11, false);
				}
				else
				{
					try
					{
						int nRingID = Global.SafeConvertToInt32(cmdParams[0]);
						int cmdData = (int)this.ChangeRing(client, nRingID);
						client.sendCmd<int>(nID, cmdData, false);
					}
					catch (Exception e)
					{
						DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_MARRY_RING", false, false);
					}
				}
				break;
			case 873:
				if (cmdParams == null || cmdParams.Length != 1)
				{
					return false;
				}
				if (!MarryLogic.IsVersionSystemOpenOfMarriage())
				{
					client.sendCmd<int>(nID, 11, false);
				}
				else
				{
					try
					{
						int cmdData = (int)this.ChangeMarriageMessage(client, cmdParams[0]);
						client.sendCmd<int>(nID, cmdData, false);
					}
					catch (Exception e)
					{
						DataHelper.WriteFormatExceptionLog(e, "CMD_SPR_MARRY_MESSAGE", false, false);
					}
				}
				break;
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
		}

		public void PlayGameAfterSend(GameClient client)
		{
			this.SendMarriageDataToClient(client, true);
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				if (null != gameClient)
				{
					string msgText = string.Format(GLang.GetLang(489, new object[0]), client.ClientData.RoleName);
					GameManager.ClientMgr.NotifyImportantMsg(gameClient, msgText, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.OnlySysHint, 0);
				}
			}
		}

		public MarryOtherResult ChangeMarriageMessage(GameClient client, string strMessage)
		{
			MarryOtherResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryOtherResult.NotMarriaged;
			}
			else if (strMessage.Length >= 64)
			{
				result = MarryOtherResult.MessageLimit;
			}
			else
			{
				client.ClientData.MyMarriageData.strLovemessage = strMessage;
				MarryFuBenMgr.UpdateMarriageData2DB(client);
				this.SendMarriageDataToClient(client, true);
				GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
				if (null != gameClient)
				{
					gameClient.ClientData.MyMarriageData.strLovemessage = strMessage;
					MarryFuBenMgr.UpdateMarriageData2DB(gameClient);
					this.SendMarriageDataToClient(gameClient, true);
				}
				else
				{
					string cmd = string.Format("{0}", client.ClientData.MyMarriageData.nSpouseID);
					MarriageData marriageData = Global.sendToDB<MarriageData, string>(10186, cmd, client.ServerId);
					if (marriageData == null || -1 == marriageData.byMarrytype)
					{
						return MarryOtherResult.Error;
					}
					MarryFuBenMgr.UpdateMarriageData2DB(client.ClientData.MyMarriageData.nSpouseID, marriageData, client);
				}
				result = MarryOtherResult.Success;
			}
			return result;
		}

		public MarryOtherResult ChangeRing(GameClient client, int nRingID)
		{
			MarryOtherResult result;
			if (-1 == client.ClientData.MyMarriageData.byMarrytype)
			{
				result = MarryOtherResult.NotMarriaged;
			}
			else if (nRingID - client.ClientData.MyMarriageData.nRingID != 1)
			{
				result = MarryOtherResult.NotNexRise;
			}
			else
			{
				SystemXmlItem systemXmlItem = null;
				if (!this.WeddingRingDic.SystemXmlItemDict.TryGetValue(nRingID, out systemXmlItem) || null == systemXmlItem)
				{
					result = MarryOtherResult.NotRing;
				}
				else
				{
					SystemXmlItem systemXmlItem2 = null;
					if (!this.WeddingRingDic.SystemXmlItemDict.TryGetValue(client.ClientData.MyMarriageData.nRingID, out systemXmlItem2) || null == systemXmlItem2)
					{
						result = MarryOtherResult.NotRing;
					}
					else
					{
						string text = "";
						int userMoney = client.ClientData.UserMoney;
						int gold = client.ClientData.Gold;
						int intValue = systemXmlItem.GetIntValue("NeedZuanShi", -1);
						int intValue2 = systemXmlItem2.GetIntValue("NeedZuanShi", -1);
						int num = intValue - intValue2;
						if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "更换婚戒扣除", true, true, false, DaiBiSySType.None))
						{
							result = MarryOtherResult.NeedGam;
						}
						else
						{
							text += EventLogManager.NewResPropString(ResLogType.FristBindZuanShi, new object[]
							{
								-num,
								gold,
								client.ClientData.Gold,
								userMoney,
								client.ClientData.UserMoney
							});
							client.ClientData.MyMarriageData.nRingID = nRingID;
							EventLogManager.AddRingBuyEvent(client, 1, nRingID, (int)client.ClientData.MyMarriageData.byGoodwilllevel, (int)client.ClientData.MyMarriageData.byGoodwilllevel, (int)client.ClientData.MyMarriageData.byGoodwillstar, (int)client.ClientData.MyMarriageData.byGoodwillstar, text);
							this.UpdateRingAttr(client, true, false);
							MarryFuBenMgr.UpdateMarriageData2DB(client);
							this.SendMarriageDataToClient(client, true);
							result = MarryOtherResult.Success;
						}
					}
				}
			}
			return result;
		}

		public void UpdateRingAttr(GameClient client, bool bNeedUpdateSpouse = false, bool bIsLogin = false)
		{
			if (MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				if (-1 != client.ClientData.MyMarriageData.nRingID)
				{
					if (-1 != client.ClientData.MyMarriageData.byMarrytype && -1 != client.ClientData.MyMarriageData.nSpouseID)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
						MarriageData marriageData;
						if (null != gameClient)
						{
							marriageData = gameClient.ClientData.MyMarriageData;
						}
						else
						{
							string cmd = string.Format("{0}", client.ClientData.MyMarriageData.nSpouseID);
							marriageData = Global.sendToDB<MarriageData, string>(10186, cmd, client.ServerId);
						}
						if (marriageData != null && -1 != marriageData.nRingID)
						{
							EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(client.ClientData.MyMarriageData.nRingID);
							EquipPropItem equipPropItem2 = new EquipPropItem();
							EquipPropItem equipPropItem3 = GameManager.EquipPropsMgr.FindEquipPropItem(marriageData.nRingID);
							EquipPropItem equipPropItem4 = new EquipPropItem();
							for (int i = 0; i < equipPropItem2.ExtProps.Length; i++)
							{
								equipPropItem2.ExtProps[i] = this.RingAttrJiSuan(client.ClientData.MyMarriageData.byGoodwilllevel, client.ClientData.MyMarriageData.byGoodwillstar, equipPropItem.ExtProps[i]);
								equipPropItem4.ExtProps[i] = this.RingAttrJiSuan(marriageData.byGoodwilllevel, marriageData.byGoodwillstar, equipPropItem3.ExtProps[i]);
								equipPropItem2.ExtProps[i] += equipPropItem4.ExtProps[i] * this.dOtherRingmodulus;
							}
							client.ClientData.PropsCacheManager.SetExtProps(new object[]
							{
								PropsSystemTypes.MarriageRing,
								equipPropItem2.ExtProps
							});
							if (!bIsLogin)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
							}
							if (bNeedUpdateSpouse)
							{
								if (null != gameClient)
								{
									this.UpdateRingAttr(gameClient, false, false);
								}
							}
						}
					}
				}
			}
		}

		private double RingAttrJiSuan(sbyte level, sbyte star, double ExpProp)
		{
			return ExpProp * ((double)(1 + (level - 1) * 2) + (double)star * this.dRingmodulus);
		}

		public void ResetRingAttr(GameClient client)
		{
			if (-1 != client.ClientData.MyMarriageData.nRingID)
			{
				EquipPropItem equipPropItem = new EquipPropItem();
				client.ClientData.PropsCacheManager.SetExtProps(new object[]
				{
					PropsSystemTypes.MarriageRing,
					equipPropItem.ExtProps
				});
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
			}
		}

		public int GetMaxGoodwillStar()
		{
			return (int)this.byMaxGoodwillStar;
		}

		public MarryOtherResult GiveRose(GameClient client, int nGoodsDBId)
		{
			MarryOtherResult result;
			if (client.ClientData.MyMarriageData.byMarrytype == -1)
			{
				result = MarryOtherResult.NotMarriaged;
			}
			else if (client.ClientData.MyMarriageData.byGoodwilllevel == this.byMaxGoodwillLv && client.ClientData.MyMarriageData.byGoodwillstar == this.byMaxGoodwillStar)
			{
				result = MarryOtherResult.MaxLimit;
			}
			else
			{
				GoodsData goodsByID = Global.GetGoodsByID(client, nGoodsDBId);
				if (null == goodsByID)
				{
					result = MarryOtherResult.NotFindItem;
				}
				else
				{
					lock (this.RoseDataDic)
					{
						MarriageRoseData marriageRoseData = null;
						if (!this.RoseDataDic.TryGetValue(goodsByID.GoodsID, out marriageRoseData))
						{
							return MarryOtherResult.ItemNotRose;
						}
						int num;
						if (client.ClientData.MyMarriageData.nGivenrose < this.dNeedGam.Length)
						{
							num = Convert.ToInt32(this.dNeedGam[client.ClientData.MyMarriageData.nGivenrose]);
						}
						else
						{
							num = Convert.ToInt32(this.dNeedGam[this.dNeedGam.Length - 1]);
						}
						if (num != 0 && !GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "结婚献花", true, true, false, DaiBiSySType.None))
						{
							return MarryOtherResult.NeedGam;
						}
						if (!GameManager.ClientMgr.NotifyUseGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, goodsByID, 1, false, false))
						{
							return MarryOtherResult.NeedRose;
						}
						client.ClientData.MyMarriageData.nGivenrose++;
						int randomNumber = Global.GetRandomNumber(0, 100);
						int num2 = 1;
						for (int i = 0; i < marriageRoseData.rateList.Count; i++)
						{
							if (randomNumber < marriageRoseData.rateList[i])
							{
								num2 = marriageRoseData.modulusList[i];
								break;
							}
						}
						this.UpdateMarriageGoodWill(client, marriageRoseData.nBaseAddGoodWill * num2);
						if (num2 != 1)
						{
							return MarryOtherResult.CirEffect;
						}
					}
					result = MarryOtherResult.Success;
				}
			}
			return result;
		}

		public bool CanAddMarriageGoodWill(GameClient client)
		{
			bool result;
			if (!MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				result = false;
			}
			else if (client.ClientData.MyMarriageData.byMarrytype == -1)
			{
				result = false;
			}
			else
			{
				sbyte byGoodwilllevel = client.ClientData.MyMarriageData.byGoodwilllevel;
				sbyte byGoodwillstar = client.ClientData.MyMarriageData.byGoodwillstar;
				result = (byGoodwilllevel != this.byMaxGoodwillLv || byGoodwillstar != this.byMaxGoodwillStar);
			}
			return result;
		}

		public void UpdateMarriageGoodWill(GameClient client, int addGoodwillValue)
		{
			if (MarryLogic.IsVersionSystemOpenOfMarriage())
			{
				if (client.ClientData.MyMarriageData.byMarrytype != -1)
				{
					if (addGoodwillValue != 0)
					{
						sbyte byGoodwilllevel = client.ClientData.MyMarriageData.byGoodwilllevel;
						sbyte byGoodwillstar = client.ClientData.MyMarriageData.byGoodwillstar;
						if (byGoodwilllevel != this.byMaxGoodwillLv || byGoodwillstar != this.byMaxGoodwillStar)
						{
							int byGoodwilllevel2 = (int)client.ClientData.MyMarriageData.byGoodwilllevel;
							int byGoodwillstar2 = (int)client.ClientData.MyMarriageData.byGoodwillstar;
							client.ClientData.MyMarriageData.nGoodwillexp += addGoodwillValue;
							int num = this.GoodwillAllExpList[(int)((byGoodwilllevel - 1) * this.byMaxGoodwillStar + byGoodwillstar)];
							client.ClientData.MyMarriageData.nGoodwillexp += num;
							bool flag = false;
							bool flag2 = false;
							for (int i = 1; i < this.GoodwillAllExpList.Count; i++)
							{
								if (i == this.GoodwillAllExpList.Count - 1 && client.ClientData.MyMarriageData.nGoodwillexp >= this.GoodwillAllExpList[i])
								{
									client.ClientData.MyMarriageData.byGoodwilllevel = this.byMaxGoodwillLv;
									client.ClientData.MyMarriageData.byGoodwillstar = this.byMaxGoodwillStar;
									flag2 = true;
									client.ClientData.MyMarriageData.nGoodwillexp = this.GoodwillAllExpList[i] - this.GoodwillAllExpList[i - 1];
								}
								else if (client.ClientData.MyMarriageData.nGoodwillexp < this.GoodwillAllExpList[i])
								{
									int num2;
									int num3;
									if (i <= (int)(this.byMaxGoodwillStar + 1))
									{
										num2 = 1;
										num3 = i - 1;
									}
									else
									{
										num2 = (i - 2) / (int)this.byMaxGoodwillStar + 1;
										num3 = (i - 1) % (int)this.byMaxGoodwillStar;
										if (num3 == 0)
										{
											num3 = 10;
										}
									}
									if (num2 != (int)byGoodwilllevel)
									{
										flag = true;
									}
									if (num3 != (int)byGoodwillstar)
									{
										flag2 = true;
									}
									client.ClientData.MyMarriageData.byGoodwilllevel = (sbyte)num2;
									client.ClientData.MyMarriageData.byGoodwillstar = (sbyte)num3;
									client.ClientData.MyMarriageData.nGoodwillexp -= this.GoodwillAllExpList[i - 1];
									break;
								}
							}
							if (flag || flag2)
							{
								client.ClientData.MyMarriageData.ChangTime = TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss");
							}
							MarryFuBenMgr.UpdateMarriageData2DB(client);
							if (flag || flag2)
							{
								this.UpdateRingAttr(client, true, false);
							}
							this.SendMarriageDataToClient(client, flag || flag2);
							if (flag)
							{
								if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMarriage) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
								{
									client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							if (addGoodwillValue > 0)
							{
								string msgText = StringUtil.substitute(GLang.GetLang(490, new object[0]), new object[]
								{
									addGoodwillValue
								});
								GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.PiaoFuZi, 0);
							}
							EventLogManager.AddRingStarSuitEvent(client, client.ClientData.MyMarriageData.nRingID, byGoodwilllevel2, (int)client.ClientData.MyMarriageData.byGoodwilllevel, byGoodwillstar2, (int)client.ClientData.MyMarriageData.byGoodwillstar, "");
						}
					}
				}
			}
		}

		public void ChangeDayUpdate(GameClient client, bool bIsFirstLogin = true)
		{
			if (bIsFirstLogin && client.ClientData.MyMarriageData.nGivenrose != 0)
			{
				client.ClientData.MyMarriageData.nGivenrose = 0;
				MarryFuBenMgr.UpdateMarriageData2DB(client);
				this.SendMarriageDataToClient(client, false);
			}
		}

		public void SendMarriageDataToClient(GameClient client, bool bSendSpouseData = true)
		{
			if (null != client.ClientData.MyMarriageData)
			{
				client.sendCmd<MarriageData>(895, client.ClientData.MyMarriageData, false);
				if (bSendSpouseData)
				{
					this.SendSpouseDataToClient(client);
				}
			}
		}

		public void SendSpouseDataToClient(GameClient client)
		{
			try
			{
				if (-1 != client.ClientData.MyMarriageData.nSpouseID)
				{
					MarriageData_EX marriageData_EX = new MarriageData_EX();
					GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
					if (null != gameClient)
					{
						marriageData_EX.myMarriageData = gameClient.ClientData.MyMarriageData;
						marriageData_EX.roleName = gameClient.ClientData.RoleName;
						marriageData_EX.Occupation = gameClient.ClientData.OccupationList[0];
						client.sendCmd<MarriageData_EX>(896, marriageData_EX, false);
					}
					else
					{
						RoleDataEx offlineRoleData = MarryLogic.GetOfflineRoleData(client.ClientData.MyMarriageData.nSpouseID);
						if (offlineRoleData != null)
						{
							marriageData_EX.roleName = offlineRoleData.RoleName;
							marriageData_EX.Occupation = offlineRoleData.OccupationList[0];
							marriageData_EX.myMarriageData = offlineRoleData.MyMarriageData;
							client.sendCmd<MarriageData_EX>(896, marriageData_EX, false);
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "SendSpouseDataToClient", false, false);
			}
		}

		private static MarriageOtherLogic instance = new MarriageOtherLogic();

		private Dictionary<int, MarriageRoseData> RoseDataDic = new Dictionary<int, MarriageRoseData>();

		private Dictionary<sbyte, Dictionary<sbyte, int>> GoodwillLvDic = new Dictionary<sbyte, Dictionary<sbyte, int>>();

		private List<int> GoodwillAllExpList = new List<int>();

		public SystemXmlItems WeddingRingDic = new SystemXmlItems();

		private sbyte byMaxGoodwillStar = 0;

		private sbyte byMaxGoodwillLv = 0;

		private double[] dNeedGam;

		private double dRingmodulus = 0.0;

		private double dOtherRingmodulus = 0.0;
	}
}
