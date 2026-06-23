using System;
using System.Collections.Generic;
using GameServer.Core.GameEvent;
using GameServer.Logic;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.Goods;
using GameServer.Logic.MUWings;
using Server.Data;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	public class WingUpStarCmdProcessor : ICmdProcessor
	{
		private WingUpStarCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(608, 2, this);
		}

		public static WingUpStarCmdProcessor getInstance()
		{
			return WingUpStarCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int roleID = Global.SafeConvertToInt32(cmdParams[0]);
			int num = Global.SafeConvertToInt32(cmdParams[1]);
			string strCostList = "";
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
				client.sendCmd<SCWingStarUp>(608, cmdData, false);
				result = true;
			}
			else
			{
				SystemXmlItem wingStarCacheItem = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel + 1);
				if (null == wingStarCacheItem)
				{
					SCWingStarUp cmdData = new SCWingStarUp(-23, roleID, 0, 0);
					client.sendCmd<SCWingStarUp>(608, cmdData, false);
					result = true;
				}
				else
				{
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("WingShengXing");
					if ("" == paramValueByName)
					{
						SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
						client.sendCmd<SCWingStarUp>(608, cmdData, false);
						result = true;
					}
					else
					{
						string[] array = paramValueByName.Split(new char[]
						{
							','
						});
						if (3 != array.Length)
						{
							SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
							client.sendCmd<SCWingStarUp>(608, cmdData, false);
							result = true;
						}
						else
						{
							int num2 = 0;
							int num3;
							if (0 == num)
							{
								num3 = (int)(Convert.ToDouble(array[0]) * 100.0);
								num2 = Convert.ToInt32(wingStarCacheItem.GetIntValue("GoodsExp", -1));
							}
							else
							{
								num3 = (int)(Convert.ToDouble(array[1]) * 100.0);
								num2 = Convert.ToInt32(wingStarCacheItem.GetIntValue("ZuanShiExp", -1));
							}
							int randomNumber = Global.GetRandomNumber(0, 100);
							if (randomNumber < num3)
							{
								num2 *= Convert.ToInt32(array[2]);
							}
							int intValue = wingStarCacheItem.GetIntValue("StarExp", -1);
							int forgeLevel = client.ClientData.MyWingData.ForgeLevel;
							int i = client.ClientData.MyWingData.ForgeLevel;
							int num4 = 0;
							if (client.ClientData.MyWingData.StarExp + num2 >= intValue)
							{
								if (i < MUWingsManager.MaxWingEnchanceLevel)
								{
									i++;
									num4 = client.ClientData.MyWingData.StarExp + num2 - intValue;
									while (i < MUWingsManager.MaxWingEnchanceLevel)
									{
										SystemXmlItem wingStarCacheItem2 = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, i + 1);
										if (null != wingStarCacheItem)
										{
											int intValue2 = wingStarCacheItem2.GetIntValue("StarExp", -1);
											if (num4 >= intValue2)
											{
												i++;
												num4 -= intValue2;
												continue;
											}
										}
										break;
									}
								}
								else
								{
									num4 = intValue;
								}
							}
							else
							{
								num4 = client.ClientData.MyWingData.StarExp + num2;
							}
							if (0 == num)
							{
								string stringValue = wingStarCacheItem.GetStringValue("NeedGoods");
								string[] array2 = stringValue.Split(new char[]
								{
									','
								});
								if (array2 == null || array2.Length != 2)
								{
									SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								int num5 = Convert.ToInt32(array2[0]);
								int num6 = Convert.ToInt32(array2[1]);
								if (num5 <= 0 || num6 <= 0)
								{
									SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								GoodsReplaceResult replaceResult = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, num5);
								if (replaceResult == null || replaceResult.TotalGoodsCnt() < num6)
								{
									SCWingStarUp cmdData = new SCWingStarUp(-4, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								List<GoodsReplaceResult.ReplaceItem> list = new List<GoodsReplaceResult.ReplaceItem>();
								list.AddRange(replaceResult.BindList);
								list.AddRange(replaceResult.UnBindList);
								list.Add(replaceResult.OriginBindGoods);
								list.Add(replaceResult.OriginUnBindGoods);
								int num7 = num6;
								foreach (GoodsReplaceResult.ReplaceItem replaceItem in list)
								{
									if (replaceItem.GoodsCnt > 0)
									{
										int num8 = Math.Min(num7, replaceItem.GoodsCnt);
										if (num8 <= 0)
										{
											break;
										}
										bool flag = false;
										bool flag2 = false;
										bool flag3 = false;
										if (replaceItem.IsBind)
										{
											if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num8, false, out flag, out flag2, false))
											{
												flag3 = true;
											}
										}
										else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num8, false, out flag, out flag2, false))
										{
											flag3 = true;
										}
										num7 -= num8;
										if (flag3)
										{
											SCWingStarUp cmdData = new SCWingStarUp(-5, roleID, 0, 0);
											client.sendCmd<SCWingStarUp>(608, cmdData, false);
											return true;
										}
										GoodsData goodsData = new GoodsData
										{
											GoodsID = replaceItem.GoodsID,
											GCount = num8
										};
										strCostList = EventLogManager.NewGoodsDataPropString(goodsData);
									}
								}
							}
							else
							{
								int intValue3 = wingStarCacheItem.GetIntValue("NeedZuanShi", -1);
								if (intValue3 <= 0)
								{
									SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								if (client.ClientData.UserMoney < intValue3)
								{
									SCWingStarUp cmdData = new SCWingStarUp(-6, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								int userMoney = client.ClientData.UserMoney;
								int gold = client.ClientData.Gold;
								if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, intValue3, "翅膀升星", true, true, false, DaiBiSySType.ChiBangShengXing))
								{
									SCWingStarUp cmdData = new SCWingStarUp(-7, roleID, 0, 0);
									client.sendCmd<SCWingStarUp>(608, cmdData, false);
									return true;
								}
								strCostList = EventLogManager.NewResPropString(ResLogType.FristBindZuanShi, new object[]
								{
									-intValue3,
									gold,
									client.ClientData.Gold,
									userMoney,
									client.ClientData.UserMoney
								});
							}
							GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingSuitStarTimes));
							int num9 = MUWingsManager.WingUpStarDBCommand(client, client.ClientData.MyWingData.DbID, i, num4);
							if (num9 < 0)
							{
								SCWingStarUp cmdData = new SCWingStarUp(-3, roleID, 0, 0);
								client.sendCmd<SCWingStarUp>(608, cmdData, false);
								result = true;
							}
							else
							{
								SCWingStarUp cmdData = new SCWingStarUp(0, roleID, i, num4);
								client.sendCmd<SCWingStarUp>(608, cmdData, false);
								client.ClientData.MyWingData.StarExp = num4;
								if (client.ClientData.MyWingData.ForgeLevel != i)
								{
									if (1 == client.ClientData.MyWingData.Using)
									{
										MUWingsManager.UpdateWingDataProps(client, false);
									}
									bool flag4 = GlobalNew.IsGongNengOpened(client, 50, false);
									client.ClientData.MyWingData.ForgeLevel = i;
									GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingLevel));
									bool flag5 = GlobalNew.IsGongNengOpened(client, 50, false);
									if (!flag4 && flag5)
									{
										LingYuManager.InitAsOpened(client);
									}
									if (1 == client.ClientData.MyWingData.Using)
									{
										MUWingsManager.UpdateWingDataProps(client, true);
										ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
										GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
										GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
									}
									if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckReborn(client))
									{
										client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
										client._IconStateMgr.SendIconStateToClient(client);
									}
								}
								EventLogManager.AddWingStarEvent(client, num, num2, forgeLevel, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, strCostList);
								ProcessTask.ProcessRoleTaskVal(client, TaskTypes.WingIDLevel, -1);
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private static WingUpStarCmdProcessor instance = new WingUpStarCmdProcessor();
	}
}
