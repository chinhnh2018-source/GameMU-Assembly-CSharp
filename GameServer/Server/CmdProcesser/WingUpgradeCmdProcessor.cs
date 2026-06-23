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
	public class WingUpgradeCmdProcessor : ICmdProcessor
	{
		private WingUpgradeCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(609, 2, this);
		}

		public static WingUpgradeCmdProcessor getInstance()
		{
			return WingUpgradeCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdId = 609;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					-3,
					num,
					0,
					0
				});
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else if (client.ClientData.MyWingData.WingID >= MUWingsManager.MaxWingID)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					-8,
					num,
					0,
					0
				});
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else
			{
				SystemXmlItem wingUPCacheItem = MUWingsManager.GetWingUPCacheItem(client.ClientData.MyWingData.WingID + 1);
				if (null == wingUPCacheItem)
				{
					string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						-3,
						num,
						0,
						0
					});
					client.sendCmd(cmdId, cmdData, false);
					result = true;
				}
				else
				{
					SystemXmlItem wingStarCacheItem = WingStarCacheManager.GetWingStarCacheItem(Global.CalcOriginalOccupationID(client), client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel + 1);
					if (null != wingStarCacheItem)
					{
						string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							-3,
							num,
							0,
							0
						});
						client.sendCmd(cmdId, cmdData, false);
						result = true;
					}
					else
					{
						string strCostList = "";
						if (0 == num2)
						{
							string stringValue = wingUPCacheItem.GetStringValue("NeedGoods");
							string[] array = stringValue.Split(new char[]
							{
								','
							});
							if (array == null || array.Length != 2)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							int num3 = Convert.ToInt32(array[0]);
							int num4 = Convert.ToInt32(array[1]);
							if (num3 <= 0 || num4 <= 0)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							GoodsReplaceResult replaceResult = SingletonTemplate<GoodsReplaceManager>.Instance().GetReplaceResult(client, num3);
							if (replaceResult == null || replaceResult.TotalGoodsCnt() < num4)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-4,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							List<GoodsReplaceResult.ReplaceItem> list = new List<GoodsReplaceResult.ReplaceItem>();
							list.AddRange(replaceResult.BindList);
							list.AddRange(replaceResult.UnBindList);
							list.Add(replaceResult.OriginBindGoods);
							list.Add(replaceResult.OriginUnBindGoods);
							int num5 = num4;
							foreach (GoodsReplaceResult.ReplaceItem replaceItem in list)
							{
								if (replaceItem.GoodsCnt > 0)
								{
									int num6 = Math.Min(num5, replaceItem.GoodsCnt);
									if (num6 <= 0)
									{
										break;
									}
									bool flag = false;
									bool flag2 = false;
									bool flag3 = false;
									if (replaceItem.IsBind)
									{
										if (!GameManager.ClientMgr.NotifyUseBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num6, false, out flag, out flag2, false))
										{
											flag3 = true;
										}
									}
									else if (!GameManager.ClientMgr.NotifyUseNotBindGoods(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, replaceItem.GoodsID, num6, false, out flag, out flag2, false))
									{
										flag3 = true;
									}
									num5 -= num6;
									if (flag3)
									{
										string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
										{
											-5,
											num,
											0,
											0
										});
										client.sendCmd(cmdId, cmdData, false);
										return true;
									}
									GoodsData goodsData = new GoodsData
									{
										GoodsID = replaceItem.GoodsID,
										GCount = num6
									};
									strCostList = EventLogManager.NewGoodsDataPropString(goodsData);
								}
							}
						}
						else
						{
							int intValue = wingUPCacheItem.GetIntValue("NeedZuanShi", -1);
							if (intValue <= 0)
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-3,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							int userMoney = client.ClientData.UserMoney;
							int gold = client.ClientData.Gold;
							if (client.ClientData.UserMoney < intValue && !HuanLeDaiBiManager.GetInstance().HuanledaibiEnough(client, intValue))
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-6,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, intValue, "翅膀进阶", true, true, false, DaiBiSySType.ChiBangShengJie))
							{
								string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
								{
									-7,
									num,
									0,
									0
								});
								client.sendCmd(cmdId, cmdData, false);
								return true;
							}
							strCostList = EventLogManager.NewResPropString(ResLogType.FristBindZuanShi, new object[]
							{
								-intValue,
								gold,
								client.ClientData.Gold,
								userMoney,
								client.ClientData.UserMoney
							});
						}
						int intValue2 = wingUPCacheItem.GetIntValue("LuckyOne", -1);
						int intValue3 = wingUPCacheItem.GetIntValue("LuckyTwo", -1);
						int num7 = (int)(wingUPCacheItem.GetDoubleValue("LuckyTwoRate") * 100.0);
						int num8 = client.ClientData.MyWingData.WingID;
						int num9 = client.ClientData.MyWingData.JinJieFailedNum;
						int nStarLevel = client.ClientData.MyWingData.ForgeLevel;
						int nStarExp = client.ClientData.MyWingData.StarExp;
						int wingID = client.ClientData.MyWingData.WingID;
						int jinJieFailedNum = client.ClientData.MyWingData.JinJieFailedNum;
						int forgeLevel = client.ClientData.MyWingData.ForgeLevel;
						int starExp = client.ClientData.MyWingData.StarExp;
						if (intValue2 + client.ClientData.MyWingData.JinJieFailedNum < intValue3)
						{
							num9++;
						}
						else if (intValue2 + client.ClientData.MyWingData.JinJieFailedNum < 110000)
						{
							int randomNumber = Global.GetRandomNumber(0, 100);
							if (randomNumber < num7)
							{
								num8++;
								num9 = 0;
								nStarLevel = 0;
								nStarExp = 0;
							}
							else
							{
								num9++;
							}
						}
						else
						{
							num8++;
							num9 = 0;
							nStarLevel = 0;
							nStarExp = 0;
						}
						GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.WingSuitStarTimes));
						int num10 = MUWingsManager.WingUpDBCommand(client, client.ClientData.MyWingData.DbID, num8, num9, nStarLevel, nStarExp, client.ClientData.MyWingData.ZhuLingNum, client.ClientData.MyWingData.ZhuHunNum);
						if (num10 < 0)
						{
							string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								-3,
								num,
								0,
								0
							});
							client.sendCmd(cmdId, cmdData, false);
							result = true;
						}
						else
						{
							string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
							{
								0,
								num,
								num8,
								num9
							});
							client.sendCmd(cmdId, cmdData, false);
							client.ClientData.MyWingData.JinJieFailedNum = num9;
							if (client.ClientData.MyWingData.WingID != num8)
							{
								if (1 == client.ClientData.MyWingData.Using)
								{
									MUWingsManager.UpdateWingDataProps(client, false);
								}
								bool flag4 = GlobalNew.IsGongNengOpened(client, 50, false);
								client.ClientData.MyWingData.WingID = num8;
								client.ClientData.MyWingData.ForgeLevel = 0;
								client.ClientData.MyWingData.StarExp = 0;
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
								if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing) || client._IconStateMgr.CheckReborn(client))
								{
									client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
									client._IconStateMgr.SendIconStateToClient(client);
								}
								EventLogManager.AddWingStarEvent(client, 2, 0, forgeLevel, client.ClientData.MyWingData.WingID, client.ClientData.MyWingData.ForgeLevel, client.ClientData.MyWingData.StarExp, strCostList);
							}
							EventLogManager.AddWingUpgradeEvent(client, num2, jinJieFailedNum, client.ClientData.MyWingData.JinJieFailedNum, wingID, client.ClientData.MyWingData.WingID, forgeLevel, client.ClientData.MyWingData.ForgeLevel, starExp, client.ClientData.MyWingData.StarExp, strCostList);
							ProcessTask.ProcessRoleTaskVal(client, TaskTypes.WingIDLevel, -1);
							result = true;
						}
					}
				}
			}
			return result;
		}

		private static WingUpgradeCmdProcessor instance = new WingUpgradeCmdProcessor();
	}
}
