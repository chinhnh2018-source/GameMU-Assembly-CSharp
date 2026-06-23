using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class HongBaoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static HongBaoManager getInstance()
		{
			return HongBaoManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("HongBaoManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1420, 0, 1, HongBaoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1421, 1, 3, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1422, 2, 3, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1423, 4, 4, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1424, 4, 4, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1428, 2, 2, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1429, 1, 1, HongBaoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(14, HongBaoManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(14, HongBaoManager.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1420:
				return this.ProcessFaHongBaoCmd(client, nID, bytes, cmdParams);
			case 1421:
				return this.GetHongBaoRankCmd(client, nID, bytes, cmdParams);
			case 1422:
				return this.ProcessGetHongBaoDataListCmd(client, nID, bytes, cmdParams);
			case 1423:
				return this.ShowHongBaoCmd(client, nID, bytes, cmdParams);
			case 1424:
				return this.GetHongBaoDetailCmd(client, nID, bytes, cmdParams);
			case 1428:
				return this.GetJirRiHongBaoBangAwardsCmd(client, nID, bytes, cmdParams);
			case 1429:
				return this.GetJirRiHongBaoBangDataCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 10001)
			{
				KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != kuaFuNotifyEnterGameEvent)
				{
					KuaFuServerLoginData kuaFuServerLoginData = kuaFuNotifyEnterGameEvent.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						lock (this.RuntimeData.Mutex)
						{
							LogManager.WriteLog(2, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						}
					}
					eventObject.Handled = true;
				}
			}
		}

		public bool InitConfig()
		{
			bool result = true;
			string arg = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.RedPacketsTime = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsTime", -1);
					this.RuntimeData.RedPacketsNumMax = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsNumMax", -1);
					this.RuntimeData.RedPacketsRight = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsRight", -1);
					this.RuntimeData.RedPacketsMessage = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsMessage", -1);
					this.RuntimeData.RedPacketsInfomationLimit = GameManager.systemParamsList.GetParamValueIntArrayByName("RedPacketsInfomationLimit", ',');
					this.RuntimeData.RedPacketsLimit = GameManager.systemParamsList.GetParamValueIntArrayByName("RedPacketsLimit", ',');
					this.RuntimeData.RedPacketsRequest = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsRequest", -1);
					this.RuntimeData.RedPacketsAutomaticRecordMax = (int)GameManager.systemParamsList.GetParamValueIntByName("RedPacketsAutomaticRecordMax", -1);
					this.RuntimeData.RedPacketsQuanMinMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsQuanMinMessage");
					this.RuntimeData.RedPacketsChongZhiMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsChongZhiMessage");
					this.RuntimeData.RedPacketsTeQuanMessage = GameManager.systemParamsList.GetParamValueByName("RedPacketsTeQuanMessage");
					JieRiChongZhiHongBaoActivity.getInstance().Init();
					JieRiHongBaoActivity.getInstance().Init();
					JieriHongBaoKingActivity.getInstance().Init();
					this.RuntimeData.Initialized = false;
					this.RuntimeData.ZhanMengHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoBangInitialized = false;
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", arg), ex, true);
				}
			}
			return result;
		}

		public bool ProcessGetHongBaoDataListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				MyHongBaoData myHongBaoData = new MyHongBaoData();
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				long num2 = Global.SafeConvertToInt64(cmdParams[2]);
				myHongBaoData.type = num;
				myHongBaoData.flag = 1L;
				if (this.IsGongNengOpened(client, false))
				{
					int roleID = client.ClientData.RoleID;
					int faction = client.ClientData.Faction;
					if (faction > 0 && num >= 0 && num <= 2)
					{
						if (this.RuntimeData.ZhanMengHongBaoInitialized)
						{
							int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
							long num3 = TimeUtil.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L;
							DateTime t = TimeUtil.NowDateTime();
							long num4 = TimeUtil.NOW();
							lock (this.RuntimeData.Mutex)
							{
								myHongBaoData.flag = TimeUtil.NOW();
								myHongBaoData.items = new List<HongBaoItemData>();
								ZhanMengHongBaoData zhanMengHongBaoData;
								if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
								{
									foreach (HongBaoSendData hongBaoSendData in zhanMengHongBaoData.HongBaoList)
									{
										if (num3 < hongBaoSendData.sendTime.Ticks)
										{
											HongBaoItemData hongBaoItemData = new HongBaoItemData();
											hongBaoItemData.sender = hongBaoSendData.sender;
											hongBaoItemData.beginTime = hongBaoSendData.sendTime;
											hongBaoItemData.endTime = hongBaoSendData.endTime;
											hongBaoItemData.diamondSumCount = hongBaoSendData.sumDiamondNum;
											hongBaoItemData.hongBaoID = hongBaoSendData.hongBaoID;
											hongBaoItemData.type = hongBaoSendData.type;
											hongBaoItemData.hongBaoStatus = hongBaoSendData.hongBaoStatus;
											HongBaoRecvData hongBaoRecvData = null;
											if (null != hongBaoSendData.RecvList)
											{
												hongBaoRecvData = hongBaoSendData.RecvList.Find((HongBaoRecvData x) => x.RoleId == client.ClientData.RoleID);
											}
											if (null != hongBaoRecvData)
											{
												hongBaoItemData.diamondCount = hongBaoRecvData.ZuanShi;
												hongBaoItemData.hongBaoStatus = 1;
											}
											if (num > 0 || hongBaoItemData.hongBaoStatus == 0)
											{
												if (hongBaoSendData.leftCount <= 0)
												{
													hongBaoItemData.hongBaoStatus = 3;
												}
												else if (hongBaoItemData.endTime <= t)
												{
													hongBaoItemData.hongBaoStatus = 2;
												}
											}
											if (num == 0)
											{
												if (hongBaoItemData.hongBaoStatus == 0)
												{
													myHongBaoData.items.Add(hongBaoItemData);
												}
											}
											else if (num == 1)
											{
												if (hongBaoSendData.senderID == roleID)
												{
													myHongBaoData.items.Add(hongBaoItemData);
												}
											}
											else
											{
												myHongBaoData.items.Add(hongBaoItemData);
											}
										}
									}
								}
							}
							myHongBaoData.items.Sort((HongBaoItemData x, HongBaoItemData y) => y.hongBaoID - x.hongBaoID);
							int num5 = this.RuntimeData.RedPacketsInfomationLimit[num];
							if (myHongBaoData.items.Count >= num5)
							{
								if (myHongBaoData.items.Count > num5)
								{
									myHongBaoData.items.RemoveRange(num5, myHongBaoData.items.Count - num5);
								}
							}
							else
							{
								if (client.ClientData.UpdateHongBaoLogTicks[num] <= num4)
								{
									List<string> cmd = new List<string>
									{
										num.ToString(),
										faction.ToString(),
										roleID.ToString(),
										this.RuntimeData.RedPacketsInfomationLimit[num].ToString()
									};
									client.ClientData.UpdateHongBaoLogTicks[num] = num4 + (long)(this.RuntimeData.RedPacketsRequest * 1000);
									client.ClientData.HongBaoLogLists[num] = Global.sendToDB<List<HongBaoItemData>, List<string>>(1438, cmd, client.ServerId);
								}
								List<HongBaoItemData> list = client.ClientData.HongBaoLogLists[num];
								if (null != list)
								{
									using (List<HongBaoItemData>.Enumerator enumerator2 = list.GetEnumerator())
									{
										while (enumerator2.MoveNext())
										{
											HongBaoItemData item = enumerator2.Current;
											if (myHongBaoData.items.Count >= this.RuntimeData.RedPacketsInfomationLimit[num])
											{
												break;
											}
											if (num3 <= item.beginTime.Ticks)
											{
												if (!myHongBaoData.items.Exists((HongBaoItemData x) => x.hongBaoID == item.hongBaoID))
												{
													lock (this.RuntimeData.Mutex)
													{
														HongBaoSendData hongBaoSendData2;
														if (!this.RuntimeData.OldHongBaoDict.TryGetValue(item.hongBaoID, out hongBaoSendData2))
														{
															hongBaoSendData2 = new HongBaoSendData();
															hongBaoSendData2.hongBaoID = item.hongBaoID;
															hongBaoSendData2.hongBaoStatus = -1;
															this.RuntimeData.OldHongBaoDict[item.hongBaoID] = hongBaoSendData2;
														}
													}
													myHongBaoData.items.Add(item);
													if (num == 0 && item.diamondCount > 0)
													{
														item.hongBaoStatus = 1;
													}
												}
											}
										}
									}
								}
							}
						}
					}
				}
				client.sendCmd<MyHongBaoData>(nID, myHongBaoData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool GetHongBaoRankCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				HongBaoRankData hongBaoRankData = new HongBaoRankData();
				TCPOutPacket tcpoutPacket = null;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				long num2 = Global.SafeConvertToInt64(cmdParams[2]);
				if (num == 0 || num == 1)
				{
					if (!this.IsGongNengOpened(client, false) || !this.RuntimeData.ZhanMengHongBaoInitialized)
					{
						tcpoutPacket = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(hongBaoRankData, TCPOutPacketPool.getInstance(), nID);
					}
					else
					{
						int faction = client.ClientData.Faction;
						hongBaoRankData.type = num;
						hongBaoRankData.flag = 1L;
						long num3 = TimeUtil.NOW();
						lock (this.RuntimeData.Mutex)
						{
							ZhanMengHongBaoData zhanMengHongBaoData;
							if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
							{
								if (num3 < zhanMengHongBaoData.LastUpdateTicks[num] + (long)this.RuntimeData.LoadFromDBInterval2)
								{
									if (num == 0)
									{
										hongBaoRankData.items = zhanMengHongBaoData.RecvRankList;
									}
									else
									{
										hongBaoRankData.items = zhanMengHongBaoData.SendRankList;
									}
									hongBaoRankData.flag = zhanMengHongBaoData.LastUpdateTicks[num];
									tcpoutPacket = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(hongBaoRankData, TCPOutPacketPool.getInstance(), nID);
									goto IL_25A;
								}
							}
						}
						List<HongBaoRankItemData> list = Global.sendToDB<List<HongBaoRankItemData>, ZhanMengHongBaoRankListQueryData>(1430, new ZhanMengHongBaoRankListQueryData
						{
							Bhid = faction,
							Count = this.RuntimeData.RedPacketsRankLimit,
							Type = num,
							StartTime = TimeUtil.GetWeekStartTimeNow()
						}, client.ServerId);
						lock (this.RuntimeData.Mutex)
						{
							ZhanMengHongBaoData zhanMengHongBaoData;
							if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
							{
								zhanMengHongBaoData = new ZhanMengHongBaoData();
								this.RuntimeData.ZhanMengHongBaoDict[faction] = zhanMengHongBaoData;
							}
							zhanMengHongBaoData.LastUpdateTicks[num] = num3;
							if (num == 0)
							{
								hongBaoRankData.items = (zhanMengHongBaoData.RecvRankList = list);
							}
							else
							{
								hongBaoRankData.items = (zhanMengHongBaoData.SendRankList = list);
							}
							hongBaoRankData.flag = zhanMengHongBaoData.LastUpdateTicks[num];
							tcpoutPacket = DataHelper.ObjectToTCPOutPacket<HongBaoRankData>(hongBaoRankData, TCPOutPacketPool.getInstance(), nID);
						}
					}
				}
				IL_25A:
				if (null != tcpoutPacket)
				{
					client.sendCmd(tcpoutPacket, true);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessFaHongBaoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (GameManager.IsKuaFuServer)
				{
					num = -12;
				}
				else
				{
					FaHongBaoData faHongBaoData = DataHelper.BytesToObject<FaHongBaoData>(bytes, 0, bytes.Length);
					if (null == faHongBaoData)
					{
						num = -18;
					}
					else if (!this.RuntimeData.Initialized)
					{
						num = -11000;
					}
					else
					{
						if (faHongBaoData.type == 0)
						{
							if (faHongBaoData.diamondNum < faHongBaoData.count || faHongBaoData.diamondNum % faHongBaoData.count != 0)
							{
								num = -18;
								goto IL_5B2;
							}
						}
						else
						{
							if (faHongBaoData.type != 1)
							{
								num = -18;
								goto IL_5B2;
							}
							if (faHongBaoData.diamondNum < faHongBaoData.count)
							{
								num = -18;
								goto IL_5B2;
							}
						}
						if (faHongBaoData.count < this.RuntimeData.RedPacketsLimit[0] || faHongBaoData.count > this.RuntimeData.RedPacketsLimit[1] || faHongBaoData.diamondNum < this.RuntimeData.RedPacketsLimit[2] || faHongBaoData.diamondNum > this.RuntimeData.RedPacketsLimit[3])
						{
							num = -18;
						}
						else
						{
							int faction = client.ClientData.Faction;
							if (faction <= 0)
							{
								num = -1033;
							}
							else
							{
								if (!string.IsNullOrEmpty(faHongBaoData.message))
								{
									if (faHongBaoData.message.Length > 60)
									{
										num = -18;
										goto IL_5B2;
									}
									num = NameServerNamager.CheckInvalidCharacters(faHongBaoData.message, false);
									if (num < 0)
									{
										num = -40;
										goto IL_5B2;
									}
								}
								DateTime sendTime = TimeUtil.NowDateTime();
								int num2 = TimeUtil.UnixSecondsNow() / 3600 % 10000;
								int num3 = Global.GetRoleParamsInt32FromDB(client, "10199");
								int num4 = num3 / 10000;
								int num5 = num3 % 10000;
								if (num4 != num2)
								{
									num4 = num2;
									num5 = 0;
								}
								if (num5 >= this.RuntimeData.RedPacketsNumMax)
								{
									num = -41;
								}
								else
								{
									num3 = num4 * 10000 + num5 + 1;
									if (!this.IsGongNengOpened(client, false))
									{
										num = -12;
									}
									else if (this.RuntimeData.RedPacketsRight > 0 && (!Global.CanTrade(client) || Global.TradeLevelLimit(client)))
									{
										num = -37;
									}
									else if (client.ClientData.UserMoney < faHongBaoData.diamondNum)
									{
										num = -10;
									}
									else if (!GameManager.ClientMgr.SubUserMoney(client, faHongBaoData.diamondNum, "发战盟红包", false, false, false, true, DaiBiSySType.None))
									{
										num = -10;
									}
									else
									{
										HongBaoSendData hongBaoSendData = new HongBaoSendData();
										hongBaoSendData.bhid = faction;
										hongBaoSendData.type = faHongBaoData.type;
										hongBaoSendData.senderID = client.ClientData.RoleID;
										hongBaoSendData.sender = Global.FormatRoleName4(client);
										hongBaoSendData.sendTime = sendTime;
										hongBaoSendData.endTime = sendTime.AddHours((double)this.RuntimeData.RedPacketsTime);
										hongBaoSendData.message = faHongBaoData.message;
										hongBaoSendData.sumDiamondNum = faHongBaoData.diamondNum;
										hongBaoSendData.sumCount = faHongBaoData.count;
										hongBaoSendData.leftZuanShi = faHongBaoData.diamondNum;
										hongBaoSendData.leftCount = faHongBaoData.count;
										int num6 = Global.sendToDB<int, HongBaoSendData>(1432, hongBaoSendData, client.ServerId);
										if (num6 <= 0)
										{
											if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, faHongBaoData.diamondNum, "发战盟红包失败退回", ActivityTypes.None, ""))
											{
												LogManager.WriteLog(15, string.Format("发战盟红包失败#退回钻石失败#zuanshi={0}", faHongBaoData.diamondNum), null, true);
											}
											num = -15;
										}
										else
										{
											lock (this.RuntimeData.Mutex)
											{
												hongBaoSendData.hongBaoID = num6;
												ZhanMengHongBaoData zhanMengHongBaoData;
												if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
												{
													zhanMengHongBaoData = new ZhanMengHongBaoData();
													this.RuntimeData.ZhanMengHongBaoDict[faction] = zhanMengHongBaoData;
												}
												zhanMengHongBaoData.HongBaoList.Add(hongBaoSendData);
												this.RuntimeData.HongBaoDict[hongBaoSendData.hongBaoID] = hongBaoSendData;
												this.AddFaHongBaoRank(client, zhanMengHongBaoData, 1, faHongBaoData.diamondNum);
											}
											Global.UpdateRoleParamByName(client, "10199", num3.ToString(), true);
											GameManager.ClientMgr.SendBangHuiCmd<string>(faction, 1425, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID), true, false);
											client.ClientData.UpdateHongBaoLogTicks[1] = 0L;
											client.ClientData.UpdateHongBaoLogTicks[2] = 0L;
											int diamondNum = faHongBaoData.diamondNum;
											int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_num_minamount", 5000);
											if (diamondNum >= gameConfigItemInt)
											{
												GameManager.logDBCmdMgr.AddTradeNumberInfo(3, diamondNum, 0, client.ClientData.RoleID, client.ServerId);
											}
											int num7 = Global.IncreaseTradeCount(client, "SaleTradeDayID", "SaleTradeCount", 1);
											int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("tradelog_freq_sale", 10);
											if (num7 >= gameConfigItemInt2)
											{
												GameManager.logDBCmdMgr.AddTradeFreqInfo(3, num7, client.ClientData.RoleID, 0);
											}
										}
									}
								}
							}
						}
					}
				}
				IL_5B2:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ShowHongBaoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = client.ClientData.RoleID;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int num2 = Global.SafeConvertToInt32(cmdParams[2]);
				int tips = Global.SafeConvertToInt32(cmdParams[3]);
				ShowHongBaoData showHongBaoData = new ShowHongBaoData();
				showHongBaoData.hongBaoID = num2;
				showHongBaoData.type = num;
				showHongBaoData.tips = tips;
				if (!this.RuntimeData.Initialized)
				{
					showHongBaoData.result = -11000;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					long num3 = TimeUtil.NOW();
					if (num == 101)
					{
						lock (this.RuntimeData.Mutex)
						{
							SystemHongBaoData systemHongBaoData;
							if (!this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(num2, out systemHongBaoData) || systemHongBaoData.LeftZuanShi <= 0 || systemHongBaoData.StartTime + (long)systemHongBaoData.DurationTime < num3)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsChongZhiMessage;
					}
					else if (num == 102)
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData hongBaoSendData;
							if (!this.RuntimeData.JieRiHongBaoDict.TryGetValue(num2, out hongBaoSendData) || hongBaoSendData.leftZuanShi <= 0 || hongBaoSendData.endTime < dateTime)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsQuanMinMessage;
					}
					else if (num == 103)
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData hongBaoSendData;
							if (!this.RuntimeData.SpecPHongBaoDict.TryGetValue(num2, out hongBaoSendData) || hongBaoSendData.leftZuanShi <= 0 || hongBaoSendData.endTime < dateTime)
							{
								showHongBaoData.result = 3;
							}
						}
						showHongBaoData.message = this.RuntimeData.RedPacketsTeQuanMessage;
					}
					else
					{
						lock (this.RuntimeData.Mutex)
						{
							HongBaoSendData hongBaoSendData;
							if (!this.RuntimeData.HongBaoDict.TryGetValue(num2, out hongBaoSendData))
							{
								showHongBaoData.result = 3;
							}
							else
							{
								showHongBaoData.type = hongBaoSendData.type;
								showHongBaoData.message = hongBaoSendData.message;
								showHongBaoData.sender = hongBaoSendData.sender;
								showHongBaoData.SumHongBaoNum = hongBaoSendData.sumCount;
								showHongBaoData.yiLingNum = hongBaoSendData.sumCount - hongBaoSendData.leftCount;
								if (hongBaoSendData.leftCount <= 0)
								{
									showHongBaoData.result = 3;
								}
								else if (dateTime >= hongBaoSendData.endTime)
								{
									showHongBaoData.result = 2;
								}
							}
						}
					}
				}
				client.sendCmd<ShowHongBaoData>(nID, showHongBaoData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool GetHongBaoDetailCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleId = client.ClientData.RoleID;
				int faction = client.ClientData.Faction;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int num2 = Global.SafeConvertToInt32(cmdParams[2]);
				int num3 = Global.SafeConvertToInt32(cmdParams[3]);
				HongBaoDeatilsData hongBaoDeatilsData = new HongBaoDeatilsData();
				hongBaoDeatilsData.hongBaoID = num2;
				hongBaoDeatilsData.type = num;
				if (!this.RuntimeData.Initialized)
				{
					hongBaoDeatilsData.hongBaoStatus = -11000;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					hongBaoDeatilsData.hongBaoStatus = -12;
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					long num4 = TimeUtil.NOW();
					lock (this.RuntimeData.Mutex)
					{
						if (num == 101)
						{
							hongBaoDeatilsData.message = this.RuntimeData.RedPacketsChongZhiMessage;
							SystemHongBaoData systemHongBaoData;
							if (!this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(num2, out systemHongBaoData) || systemHongBaoData.LeftZuanShi <= 0)
							{
								hongBaoDeatilsData.hongBaoStatus = 3;
							}
							else if (systemHongBaoData.StartTime > num4)
							{
								hongBaoDeatilsData.hongBaoStatus = 3;
							}
							else
							{
								List<HongBaoRecvData> list;
								if (!this.RuntimeData.ChongZhiHongBaoRecvDict.TryGetValue(num2, out list))
								{
									list = new List<HongBaoRecvData>();
									this.RuntimeData.ChongZhiHongBaoRecvDict[num2] = list;
								}
								HongBaoRecvData hongBaoRecvData = list.Find((HongBaoRecvData x) => x.RoleId == roleId);
								if (null == hongBaoRecvData)
								{
									int num5 = JunTuanClient.getInstance().OpenHongBao(systemHongBaoData.HongBaoId, roleId, client.ClientData.ZoneID, client.strUserID, client.ClientData.RoleName);
									if (systemHongBaoData.LeftZuanShi < num5)
									{
										num5 = systemHongBaoData.LeftZuanShi;
									}
									if (num5 > 0)
									{
										hongBaoRecvData = new HongBaoRecvData();
										hongBaoRecvData.HongBaoID = num2;
										hongBaoRecvData.RoleId = roleId;
										hongBaoRecvData.ZuanShi = num5;
										hongBaoRecvData.RecvTime = dateTime;
										list.Add(hongBaoRecvData);
										systemHongBaoData.LeftZuanShi -= num5;
										hongBaoDeatilsData.diamondNum = num5;
										if (!GameManager.ClientMgr.AddUserGold(client, num5, "领取充值红包"))
										{
											LogManager.WriteLog(15, string.Format("领取红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, num5), null, true);
										}
										JieriHongBaoKingActivity.getInstance().OnRecv(client, num5, "领取充值红包");
										hongBaoDeatilsData.hongBaoStatus = 1;
									}
									else
									{
										systemHongBaoData.LeftZuanShi = 0;
										if (num5 == 0)
										{
											hongBaoDeatilsData.hongBaoStatus = 3;
										}
										else
										{
											if (num5 == -200)
											{
												hongBaoRecvData = new HongBaoRecvData();
												hongBaoRecvData.HongBaoID = num2;
												hongBaoRecvData.RoleId = roleId;
												hongBaoRecvData.ZuanShi = 0;
												hongBaoRecvData.RecvTime = dateTime;
												list.Add(hongBaoRecvData);
											}
											hongBaoDeatilsData.hongBaoStatus = num5;
										}
									}
								}
								else
								{
									hongBaoDeatilsData.hongBaoStatus = 1;
								}
							}
						}
						else if (num == 102)
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							hongBaoDeatilsData.message = this.RuntimeData.RedPacketsQuanMinMessage;
							HongBaoSendData hongBaoSendData;
							if (!this.RuntimeData.JieRiHongBaoDict.TryGetValue(num2, out hongBaoSendData) || hongBaoSendData.leftZuanShi <= 0)
							{
								hongBaoDeatilsData.hongBaoStatus = 3;
							}
							else if (dateTime > hongBaoSendData.endTime)
							{
								hongBaoDeatilsData.hongBaoStatus = 2;
							}
							else
							{
								List<HongBaoRecvData> list = hongBaoSendData.RecvList;
								if (null == list)
								{
									list = new List<HongBaoRecvData>();
									hongBaoSendData.RecvList = list;
								}
								HongBaoRecvData hongBaoRecvData = list.Find((HongBaoRecvData x) => x.RoleId == roleId);
								if (null == hongBaoRecvData)
								{
									int num5 = JieRiHongBaoActivity.getInstance().OpenHongBao(hongBaoSendData.senderID);
									if (hongBaoSendData.leftZuanShi < num5)
									{
										num5 = hongBaoSendData.leftZuanShi;
									}
									if (num5 > 0)
									{
										hongBaoRecvData = new HongBaoRecvData();
										hongBaoRecvData.HongBaoID = num2;
										hongBaoRecvData.RoleId = roleId;
										hongBaoRecvData.ZuanShi = num5;
										hongBaoRecvData.RecvTime = dateTime;
										list.Add(hongBaoRecvData);
										hongBaoSendData.leftZuanShi -= num5;
										int num6 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData, client.ServerId);
										if (num6 < 0)
										{
											hongBaoSendData.leftZuanShi += num5;
											LogManager.WriteLog(2, "领取全民红包失败#更新红包数据失败", null, true);
											hongBaoDeatilsData.hongBaoStatus = -15;
										}
										else
										{
											hongBaoDeatilsData.diamondNum = num5;
											if (!GameManager.ClientMgr.AddUserGold(client, num5, "领取全民红包"))
											{
												LogManager.WriteLog(15, string.Format("领取红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, num5), null, true);
											}
											JieriHongBaoKingActivity.getInstance().OnRecv(client, num5, "领取全民红包");
											hongBaoDeatilsData.hongBaoStatus = 1;
										}
									}
									else
									{
										hongBaoDeatilsData.hongBaoStatus = 3;
									}
								}
								else
								{
									hongBaoDeatilsData.hongBaoStatus = 1;
								}
							}
						}
						else if (num == 103)
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							hongBaoDeatilsData.message = this.RuntimeData.RedPacketsQuanMinMessage;
							HongBaoSendData hongBaoSendData;
							if (!this.RuntimeData.SpecPHongBaoDict.TryGetValue(num2, out hongBaoSendData) || hongBaoSendData.leftZuanShi <= 0)
							{
								hongBaoDeatilsData.hongBaoStatus = 3;
							}
							else if (dateTime > hongBaoSendData.endTime)
							{
								hongBaoDeatilsData.hongBaoStatus = 2;
							}
							else
							{
								List<HongBaoRecvData> list = hongBaoSendData.RecvList;
								if (null == list)
								{
									list = new List<HongBaoRecvData>();
									hongBaoSendData.RecvList = list;
								}
								SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
								if (list.Find((HongBaoRecvData x) => x.RoleId == roleId) == null && null != specPriorityActivity)
								{
									int num5 = specPriorityActivity.OpenHongBao(hongBaoSendData.senderID);
									if (hongBaoSendData.leftZuanShi < num5)
									{
										num5 = hongBaoSendData.leftZuanShi;
									}
									if (num5 > 0)
									{
										HongBaoRecvData hongBaoRecvData = new HongBaoRecvData();
										hongBaoRecvData.HongBaoID = num2;
										hongBaoRecvData.RoleId = roleId;
										hongBaoRecvData.ZuanShi = num5;
										hongBaoRecvData.RecvTime = dateTime;
										list.Add(hongBaoRecvData);
										hongBaoSendData.leftZuanShi -= num5;
										int num6 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData, client.ServerId);
										if (num6 < 0)
										{
											hongBaoSendData.leftZuanShi += num5;
											LogManager.WriteLog(2, "领取特权红包失败#更新红包数据失败", null, true);
											hongBaoDeatilsData.hongBaoStatus = -15;
										}
										else
										{
											hongBaoDeatilsData.diamondNum = num5;
											if (!GameManager.ClientMgr.AddUserGold(client, num5, "领取特权红包"))
											{
												LogManager.WriteLog(15, string.Format("领取特权红包失败#无法给予领取者绑定钻石#rid={0},zuanshi={1}", client.ClientData.RoleID, num5), null, true);
											}
											specPriorityActivity.OnRecvHongBao(client, hongBaoSendData);
											hongBaoDeatilsData.hongBaoStatus = 1;
										}
									}
									else
									{
										hongBaoDeatilsData.hongBaoStatus = 3;
									}
								}
								else
								{
									hongBaoDeatilsData.hongBaoStatus = 1;
								}
							}
						}
						else
						{
							if (GameManager.IsKuaFuServer)
							{
								return false;
							}
							HongBaoSendData hongBaoSendData2;
							if (!this.RuntimeData.HongBaoDict.TryGetValue(num2, out hongBaoSendData2))
							{
								if (faction == 0)
								{
									hongBaoDeatilsData.hongBaoStatus = 2;
									goto IL_E50;
								}
								if (this.RuntimeData.OldHongBaoDict.TryGetValue(num2, out hongBaoSendData2))
								{
									if (hongBaoSendData2.hongBaoStatus < 0)
									{
										hongBaoSendData2 = Global.sendToDB<HongBaoSendData, int>(1439, num2, client.ServerId);
										if (null == hongBaoSendData2)
										{
											hongBaoDeatilsData.hongBaoStatus = -11000;
											goto IL_E50;
										}
										this.RuntimeData.OldHongBaoDict[num2] = hongBaoSendData2;
									}
								}
								else
								{
									if (num3 > 0)
									{
										hongBaoDeatilsData.hongBaoStatus = 3;
										goto IL_E50;
									}
									hongBaoDeatilsData.hongBaoStatus = -20;
									goto IL_E50;
								}
							}
							if (hongBaoDeatilsData.type != hongBaoSendData2.type)
							{
								hongBaoDeatilsData.hongBaoStatus = 2;
							}
							else if (faction != hongBaoSendData2.bhid)
							{
								hongBaoDeatilsData.hongBaoStatus = 2;
							}
							else
							{
								int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
								long num7 = TimeUtil.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L;
								if (num7 > hongBaoSendData2.sendTime.Ticks)
								{
									hongBaoDeatilsData.hongBaoStatus = -1009;
								}
								else
								{
									hongBaoDeatilsData.hongBaoID = num2;
									hongBaoDeatilsData.sender = hongBaoSendData2.sender;
									hongBaoDeatilsData.leftCount = hongBaoSendData2.leftCount;
									hongBaoDeatilsData.message = hongBaoSendData2.message;
									hongBaoDeatilsData.sendTime = hongBaoSendData2.sendTime;
									hongBaoDeatilsData.sumCount = hongBaoSendData2.sumCount;
									hongBaoDeatilsData.sumDiamondNum = hongBaoSendData2.sumDiamondNum;
									hongBaoDeatilsData.type = hongBaoSendData2.type;
									bool flag2 = true;
									List<HongBaoRecvData> list2 = hongBaoSendData2.RecvList;
									if (null != list2)
									{
										HongBaoRecvData hongBaoRecvData2 = list2.Find((HongBaoRecvData x) => x.RoleId == roleId);
										if (null != hongBaoRecvData2)
										{
											hongBaoDeatilsData.diamondNum = hongBaoRecvData2.ZuanShi;
											hongBaoDeatilsData.hongBaoStatus = 1;
											flag2 = false;
										}
									}
									else
									{
										list2 = new List<HongBaoRecvData>();
										hongBaoSendData2.RecvList = list2;
									}
									if (flag2)
									{
										if (hongBaoSendData2.leftCount <= 0)
										{
											hongBaoDeatilsData.hongBaoStatus = 3;
											hongBaoSendData2.hongBaoStatus = 2;
											flag2 = false;
										}
										else if (dateTime >= hongBaoSendData2.endTime)
										{
											hongBaoDeatilsData.hongBaoStatus = 2;
											hongBaoSendData2.hongBaoStatus = 2;
											flag2 = false;
										}
									}
									if (num3 > 0 && flag2)
									{
										int num5;
										if (hongBaoSendData2.type == 1)
										{
											num5 = hongBaoSendData2.leftZuanShi;
											if (hongBaoSendData2.leftCount > 1)
											{
												num5 = Global.GetRandomNumber(1, 2 * num5 / hongBaoSendData2.leftCount - 1);
											}
										}
										else
										{
											if (hongBaoSendData2.type != 0)
											{
												hongBaoDeatilsData.hongBaoStatus = 0;
												goto IL_E50;
											}
											num5 = hongBaoSendData2.sumDiamondNum / hongBaoSendData2.sumCount;
										}
										hongBaoSendData2.leftCount--;
										hongBaoSendData2.leftZuanShi -= num5;
										if (hongBaoSendData2.leftZuanShi <= 0)
										{
											hongBaoSendData2.hongBaoStatus = 3;
										}
										int num6 = Global.sendToDB<int, HongBaoSendData>(1432, hongBaoSendData2, client.ServerId);
										if (num6 < 0)
										{
											hongBaoSendData2.leftCount++;
											hongBaoSendData2.leftZuanShi += num5;
											LogManager.WriteLog(2, "领取战盟红包失败#更新红包数据失败", null, true);
											hongBaoDeatilsData.hongBaoStatus = -15;
											goto IL_E50;
										}
										HongBaoRecvData hongBaoRecvData = new HongBaoRecvData();
										hongBaoRecvData.HongBaoID = num2;
										hongBaoRecvData.RoleId = roleId;
										hongBaoRecvData.RoleName = Global.FormatRoleName4(client);
										hongBaoRecvData.ZuanShi = num5;
										hongBaoRecvData.RecvTime = dateTime;
										hongBaoRecvData.BhId = faction;
										list2.Add(hongBaoRecvData);
										hongBaoDeatilsData.leftCount = hongBaoSendData2.leftCount;
										hongBaoDeatilsData.diamondNum = num5;
										hongBaoDeatilsData.hongBaoStatus = 1;
										int num8 = Global.sendToDB<int, HongBaoRecvData>(1433, hongBaoRecvData, client.ServerId);
										if (num8 < 0)
										{
											LogManager.WriteLog(15, string.Format("领取战盟红包失败#红包钻石已扣减但无法记录领取者#rid={0},zuanshi={1},hongbaoid={2}", client.ClientData.RoleID, num5, hongBaoSendData2.hongBaoID), null, true);
										}
										if (!GameManager.ClientMgr.AddUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num5, "领取战盟红包", ActivityTypes.None, ""))
										{
											LogManager.WriteLog(15, string.Format("领取战盟红包失败#红包钻石已扣减但无法给予领取者钻石#rid={0},zuanshi={1},hongbaoid={2}", client.ClientData.RoleID, num5, hongBaoSendData2.hongBaoID), null, true);
										}
										ZhanMengHongBaoData zhanMengHongBaoData;
										if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
										{
											zhanMengHongBaoData = new ZhanMengHongBaoData();
											this.RuntimeData.ZhanMengHongBaoDict[faction] = zhanMengHongBaoData;
										}
										this.AddFaHongBaoRank(client, zhanMengHongBaoData, 0, hongBaoDeatilsData.diamondNum);
										client.ClientData.UpdateHongBaoLogTicks[0] = 0L;
										client.ClientData.UpdateHongBaoLogTicks[2] = 0L;
									}
									hongBaoDeatilsData.infos = new List<SingleHongBaoRankInfo>();
									foreach (HongBaoRecvData hongBaoRecvData3 in list2)
									{
										hongBaoDeatilsData.infos.Add(new SingleHongBaoRankInfo
										{
											roleName = hongBaoRecvData3.RoleName,
											diamondNum = hongBaoRecvData3.ZuanShi,
											recvTime = hongBaoRecvData3.RecvTime
										});
									}
								}
							}
						}
					}
				}
				IL_E50:
				client.sendCmd<HongBaoDeatilsData>(nID, hongBaoDeatilsData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool GetJirRiHongBaoBangDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				JieriHongBaoKingActivity.getInstance().QueryActivityInfo(client);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool GetJirRiHongBaoBangAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = client.ClientData.RoleID;
				int awardid = Global.SafeConvertToInt32(cmdParams[1]);
				JieriHongBaoKingActivity.getInstance().GetAward(client, awardid);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(14) || true;
		}

		public void OnInitGame(GameClient client)
		{
			this.NotifyHongBaoData(client);
		}

		public void AddChargeValue(long value)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (value > 0L)
				{
					this.RuntimeData.AddChargeValue += value;
				}
			}
		}

		public void UpdateChongZhiHongBaoDataList(KuaFuCmdData cmdData)
		{
			JieRiChongZhiHongBaoActivity jieRiChongZhiHongBaoActivity = JieRiChongZhiHongBaoActivity.getInstance();
			if (jieRiChongZhiHongBaoActivity.InActivityTime())
			{
				Dictionary<int, SystemHongBaoData> dictionary = null;
				long num = 0L;
				if (null != cmdData)
				{
					num = cmdData.Age;
					dictionary = DataHelper.BytesToObject<Dictionary<int, SystemHongBaoData>>(cmdData.Bytes0, 0, cmdData.Bytes0.Length);
				}
				lock (this.RuntimeData.Mutex)
				{
					if (dictionary == null)
					{
						dictionary = new Dictionary<int, SystemHongBaoData>();
					}
					foreach (KeyValuePair<int, SystemHongBaoData> keyValuePair in dictionary)
					{
						keyValuePair.Value.StartTime += TimeUtil.NOW() - num;
						SystemHongBaoData systemHongBaoData;
						if (this.RuntimeData.ChongZhiHongBaoDict.TryGetValue(keyValuePair.Key, out systemHongBaoData))
						{
							keyValuePair.Value.State = systemHongBaoData.State;
						}
						this.RuntimeData.ChongZhiHongBaoDict[keyValuePair.Key] = keyValuePair.Value;
					}
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, SystemHongBaoData> keyValuePair in this.RuntimeData.ChongZhiHongBaoDict)
					{
						if (!dictionary.ContainsKey(keyValuePair.Key))
						{
							list.Add(keyValuePair.Key);
						}
					}
					foreach (int key in list)
					{
						this.RuntimeData.ChongZhiHongBaoDict.Remove(key);
					}
				}
			}
		}

		private void TimerProc(object sender, EventArgs e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long num = TimeUtil.NOW();
			this.InitHongBaoData();
			if (this.RuntimeData.NextTicks1 < num)
			{
				this.RuntimeData.NextTicks1 = num + 1000L;
				lock (this.RuntimeData.Mutex)
				{
					foreach (SystemHongBaoData systemHongBaoData in this.RuntimeData.ChongZhiHongBaoDict.Values)
					{
						if (systemHongBaoData.State == 0)
						{
							if (systemHongBaoData.StartTime <= num)
							{
								systemHongBaoData.State = 1;
								GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", 101, systemHongBaoData.HongBaoId), true);
							}
						}
						else if (systemHongBaoData.State == 1)
						{
							if (systemHongBaoData.LeftZuanShi <= 0 || systemHongBaoData.StartTime + (long)systemHongBaoData.DurationTime < num)
							{
								systemHongBaoData.State = 2;
							}
						}
					}
				}
			}
			if (!GameManager.IsKuaFuServer)
			{
				if (this.RuntimeData.NextTicks3 < num)
				{
					this.RuntimeData.NextTicks3 = num + 3000L;
					JieRiHongBaoActivity jieRiHongBaoActivity = JieRiHongBaoActivity.getInstance();
					if (jieRiHongBaoActivity.InActivityTime())
					{
						lock (this.RuntimeData.Mutex)
						{
							List<HongBaoSendData> list = jieRiHongBaoActivity.SendHongBaoProc(dateTime, this.RuntimeData.JieRiHongBaoDict);
							if (null != list)
							{
								foreach (HongBaoSendData hongBaoSendData in list)
								{
									this.RuntimeData.JieRiHongBaoDict[hongBaoSendData.hongBaoID] = hongBaoSendData;
									GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID), true);
								}
							}
						}
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPriorityActivity.InActivityTime())
					{
						lock (this.RuntimeData.Mutex)
						{
							List<HongBaoSendData> list = specPriorityActivity.SendHongBaoProc(dateTime, this.RuntimeData.SpecPHongBaoDict);
							if (null != list)
							{
								foreach (HongBaoSendData hongBaoSendData in list)
								{
									this.RuntimeData.SpecPHongBaoDict[hongBaoSendData.hongBaoID] = hongBaoSendData;
									GameManager.ClientMgr.BroadcastServerCmd(1426, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID), true);
								}
							}
						}
					}
					JieRiChongZhiHongBaoActivity jieRiChongZhiHongBaoActivity = JieRiChongZhiHongBaoActivity.getInstance();
					if (jieRiChongZhiHongBaoActivity.InActivityTime())
					{
						long num2 = 0L;
						string keyStr = jieRiChongZhiHongBaoActivity.GetKeyStr();
						lock (this.RuntimeData.Mutex)
						{
							num2 = this.RuntimeData.AddChargeValue;
							this.RuntimeData.AddChargeValue = 0L;
						}
						if (num2 > 0L && !string.IsNullOrEmpty(keyStr))
						{
							if (!JunTuanClient.getInstance().AddChargeValue(keyStr, num2))
							{
								this.AddChargeValue(num2);
							}
						}
					}
				}
				if (this.RuntimeData.NextCheckExpireTicks < num)
				{
					this.RuntimeData.NextCheckExpireTicks = num + 30000L;
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> list2 = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoSendData2 in this.RuntimeData.JieRiHongBaoDict.Values)
						{
							if (hongBaoSendData2.leftZuanShi <= 0)
							{
								hongBaoSendData2.hongBaoStatus = 3;
								list2.Add(hongBaoSendData2);
							}
							else if (hongBaoSendData2.endTime < dateTime)
							{
								hongBaoSendData2.hongBaoStatus = 2;
								list2.Add(hongBaoSendData2);
							}
						}
						foreach (HongBaoSendData hongBaoSendData2 in list2)
						{
							int num3 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData2, 0);
							if (num3 >= 0)
							{
								this.RuntimeData.JieRiHongBaoDict.Remove(hongBaoSendData2.hongBaoID);
							}
						}
					}
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> list2 = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoSendData2 in this.RuntimeData.SpecPHongBaoDict.Values)
						{
							if (hongBaoSendData2.leftZuanShi <= 0)
							{
								hongBaoSendData2.hongBaoStatus = 3;
								list2.Add(hongBaoSendData2);
							}
							else if (hongBaoSendData2.endTime < dateTime)
							{
								hongBaoSendData2.hongBaoStatus = 2;
								list2.Add(hongBaoSendData2);
							}
						}
						foreach (HongBaoSendData hongBaoSendData2 in list2)
						{
							int num3 = Global.sendToDB<int, HongBaoSendData>(1435, hongBaoSendData2, 0);
							if (num3 >= 0)
							{
								this.RuntimeData.SpecPHongBaoDict.Remove(hongBaoSendData2.hongBaoID);
							}
						}
					}
					lock (this.RuntimeData.Mutex)
					{
						List<HongBaoSendData> list2 = new List<HongBaoSendData>();
						foreach (HongBaoSendData hongBaoSendData2 in this.RuntimeData.HongBaoDict.Values)
						{
							if (hongBaoSendData2.leftZuanShi <= 0)
							{
								hongBaoSendData2.hongBaoStatus = 3;
								list2.Add(hongBaoSendData2);
							}
							else if (hongBaoSendData2.endTime < dateTime || this.RuntimeData.DestoryBhIds.Contains(hongBaoSendData2.bhid))
							{
								hongBaoSendData2.endTime = dateTime;
								hongBaoSendData2.hongBaoStatus = 2;
								list2.Add(hongBaoSendData2);
								int leftZuanShi = hongBaoSendData2.leftZuanShi;
								if (leftZuanShi > 0)
								{
									hongBaoSendData2.leftZuanShi = 0;
									this.AddRoleZuanShiBySendMail(hongBaoSendData2.senderID, leftZuanShi, GLang.GetLang(2606, new object[0]), string.Format(GLang.GetLang(2607, new object[0]), leftZuanShi), "红包过期");
								}
							}
						}
						foreach (HongBaoSendData hongBaoSendData2 in list2)
						{
							int num3 = Global.sendToDB<int, HongBaoSendData>(1432, hongBaoSendData2, 0);
							if (num3 >= 0)
							{
								this.RuntimeData.HongBaoDict.Remove(hongBaoSendData2.hongBaoID);
								this.RuntimeData.OldHongBaoDict[hongBaoSendData2.hongBaoID] = hongBaoSendData2;
							}
						}
						this.RuntimeData.DestoryBhIds.Clear();
					}
				}
			}
		}

		public void OnDestoryZhanMeng(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.DestoryBhIds.Add(bhid);
			}
		}

		private bool AddRoleZuanShiBySendMail(int roleID, int zuanShi, string subject, string content, string msg)
		{
			string text = "";
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				-1,
				GLang.GetLang(112, new object[0]),
				roleID,
				"",
				subject,
				content,
				0,
				0,
				zuanShi,
				text
			});
			string[] array = Global.ExecuteDBCmd(10086, strcmd, GameManager.ServerId);
			bool flag = array == null;
			if (!flag)
			{
				LogManager.WriteLog(15, string.Format("邮件发钻石#{0}#失败#rid={1},zuanshi={2}", msg, roleID, zuanShi), null, true);
			}
			else
			{
				LogManager.WriteLog(15, string.Format("邮件发钻石#{0}#成功#rid={1},zuanshi={2}", msg, roleID, zuanShi), null, true);
			}
			return flag;
		}

		private int RankCompareProc(HongBaoRankItemData x, HongBaoRankItemData y)
		{
			int num = y.daimondNum - x.daimondNum;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				result = x.rankID - y.rankID;
			}
			return result;
		}

		private void AddFaHongBaoRank(GameClient client, ZhanMengHongBaoData data, int type, int addValue)
		{
			List<HongBaoRankItemData> list;
			if (type == 0)
			{
				list = data.RecvRankList;
			}
			else
			{
				list = data.SendRankList;
			}
			int num = list.FindIndex((HongBaoRankItemData x) => x.roleId == client.ClientData.RoleID);
			if (num >= 0)
			{
				HongBaoRankItemData hongBaoRankItemData = list[num];
				hongBaoRankItemData.daimondNum += addValue;
			}
			else
			{
				list.Add(new HongBaoRankItemData
				{
					roleName = Global.FormatRoleName4(client),
					roleId = client.ClientData.RoleID,
					daimondNum = addValue
				});
			}
			list.Sort(new Comparison<HongBaoRankItemData>(this.RankCompareProc));
			for (int i = 0; i < list.Count; i++)
			{
				list[i].rankID = i + 1;
			}
		}

		public void NotifyHongBaoData(GameClient client)
		{
			int faction = client.ClientData.Faction;
			int roleId = client.ClientData.RoleID;
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
			long num = TimeUtil.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L;
			DateTime t = TimeUtil.NowDateTime();
			long num2 = TimeUtil.NOW();
			lock (this.RuntimeData.Mutex)
			{
				if (faction > 0)
				{
					ZhanMengHongBaoData zhanMengHongBaoData;
					if (this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(faction, out zhanMengHongBaoData))
					{
						foreach (HongBaoSendData hongBaoSendData in zhanMengHongBaoData.HongBaoList)
						{
							if (num < hongBaoSendData.sendTime.Ticks && hongBaoSendData.leftCount > 0 && t < hongBaoSendData.endTime)
							{
								bool flag2;
								if (hongBaoSendData.RecvList != null)
								{
									flag2 = hongBaoSendData.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
								}
								else
								{
									flag2 = false;
								}
								if (!flag2)
								{
									client.SendCmdAfterStartPlayGame(1425, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID));
								}
							}
						}
					}
				}
				foreach (HongBaoSendData hongBaoSendData in this.RuntimeData.JieRiHongBaoDict.Values)
				{
					if (hongBaoSendData.leftZuanShi > 0 && t < hongBaoSendData.endTime)
					{
						bool flag3;
						if (hongBaoSendData.RecvList != null)
						{
							flag3 = hongBaoSendData.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag3 = false;
						}
						if (!flag3)
						{
							client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID));
						}
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				foreach (HongBaoSendData hongBaoSendData in this.RuntimeData.SpecPHongBaoDict.Values)
				{
					if (hongBaoSendData.leftZuanShi > 0 && t < hongBaoSendData.endTime)
					{
						bool flag4;
						if (hongBaoSendData.RecvList != null)
						{
							flag4 = hongBaoSendData.RecvList.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag4 = false;
						}
						if (!flag4)
						{
							if (specPriorityActivity != null && specPriorityActivity.CanGetHongBao(client, hongBaoSendData))
							{
								client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", hongBaoSendData.type, hongBaoSendData.hongBaoID));
							}
						}
					}
				}
				foreach (SystemHongBaoData systemHongBaoData in this.RuntimeData.ChongZhiHongBaoDict.Values)
				{
					if (systemHongBaoData.LeftZuanShi > 0 && num2 >= systemHongBaoData.StartTime && num2 < systemHongBaoData.StartTime + (long)systemHongBaoData.DurationTime)
					{
						List<HongBaoRecvData> list;
						bool flag5;
						if (this.RuntimeData.ChongZhiHongBaoRecvDict.TryGetValue(systemHongBaoData.HongBaoId, out list))
						{
							flag5 = list.Exists((HongBaoRecvData x) => x.RoleId == roleId);
						}
						else
						{
							flag5 = false;
						}
						if (!flag5)
						{
							client.SendCmdAfterStartPlayGame(1426, string.Format("{0}:{1}", 101, systemHongBaoData.HongBaoId));
						}
					}
				}
			}
		}

		private void InitHongBaoData()
		{
			bool flag = false;
			bool flag2 = false;
			bool flag3 = false;
			bool flag4 = false;
			lock (this.RuntimeData.Mutex)
			{
				if (GameManager.IsKuaFuServer)
				{
					this.RuntimeData.ZhanMengHongBaoInitialized = false;
					this.RuntimeData.JieRiHongBaoInitialized = true;
					this.RuntimeData.JieRiHongBaoBangInitialized = true;
					this.RuntimeData.SpecPHongBaoInitialized = true;
					this.RuntimeData.Initialized = true;
					return;
				}
				flag = this.RuntimeData.ZhanMengHongBaoInitialized;
				flag2 = this.RuntimeData.JieRiHongBaoInitialized;
				flag3 = this.RuntimeData.JieRiHongBaoBangInitialized;
				flag4 = this.RuntimeData.SpecPHongBaoInitialized;
			}
			if (!flag)
			{
				HongBaoListQueryData hongBaoListQueryData = new HongBaoListQueryData();
				hongBaoListQueryData = Global.sendToDB<HongBaoListQueryData, HongBaoListQueryData>(1434, hongBaoListQueryData, 0);
				if (hongBaoListQueryData != null && hongBaoListQueryData.Success > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (null != hongBaoListQueryData.List)
						{
							using (List<HongBaoSendData>.Enumerator enumerator = hongBaoListQueryData.List.GetEnumerator())
							{
								while (enumerator.MoveNext())
								{
									HongBaoSendData data = enumerator.Current;
									ZhanMengHongBaoData zhanMengHongBaoData;
									if (!this.RuntimeData.ZhanMengHongBaoDict.TryGetValue(data.bhid, out zhanMengHongBaoData))
									{
										zhanMengHongBaoData = new ZhanMengHongBaoData();
										this.RuntimeData.ZhanMengHongBaoDict[data.bhid] = zhanMengHongBaoData;
									}
									if (!zhanMengHongBaoData.HongBaoList.Exists((HongBaoSendData x) => x.hongBaoID == data.hongBaoID))
									{
										zhanMengHongBaoData.HongBaoList.Add(data);
									}
									if (!this.RuntimeData.HongBaoDict.ContainsKey(data.hongBaoID))
									{
										this.RuntimeData.HongBaoDict[data.hongBaoID] = data;
									}
								}
							}
						}
						this.RuntimeData.ZhanMengHongBaoInitialized = true;
						flag = true;
					}
				}
			}
			if (!flag2)
			{
				flag2 = true;
				HongBaoListQueryData hongBaoListQueryData = JieRiHongBaoActivity.getInstance().QueryHongBaoList();
				if (hongBaoListQueryData != null && hongBaoListQueryData.Success > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (null != hongBaoListQueryData.List)
						{
							foreach (HongBaoSendData hongBaoSendData in hongBaoListQueryData.List)
							{
								if (!this.RuntimeData.JieRiHongBaoDict.ContainsKey(hongBaoSendData.hongBaoID))
								{
									this.RuntimeData.JieRiHongBaoDict[hongBaoSendData.hongBaoID] = hongBaoSendData;
								}
							}
						}
						this.RuntimeData.JieRiHongBaoInitialized = true;
						flag2 = true;
					}
				}
			}
			if (!flag4)
			{
				flag4 = true;
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					HongBaoListQueryData hongBaoListQueryData = specPriorityActivity.QueryHongBaoList();
					if (hongBaoListQueryData != null && hongBaoListQueryData.Success > 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							if (null != hongBaoListQueryData.List)
							{
								foreach (HongBaoSendData hongBaoSendData in hongBaoListQueryData.List)
								{
									if (!this.RuntimeData.SpecPHongBaoDict.ContainsKey(hongBaoSendData.hongBaoID))
									{
										this.RuntimeData.SpecPHongBaoDict[hongBaoSendData.hongBaoID] = hongBaoSendData;
									}
								}
							}
							this.RuntimeData.SpecPHongBaoInitialized = true;
							flag4 = true;
						}
					}
				}
			}
			if (!flag3)
			{
				flag3 = JieriHongBaoKingActivity.getInstance().LoadRankFromDB();
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.JieRiHongBaoBangInitialized = flag3;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.JieRiHongBaoInitialized = flag2;
				this.RuntimeData.Initialized = (flag && flag2 && flag3);
			}
		}

		private static HongBaoManager instance = new HongBaoManager();

		public HongBaoRuntimeData RuntimeData = new HongBaoRuntimeData();
	}
}
