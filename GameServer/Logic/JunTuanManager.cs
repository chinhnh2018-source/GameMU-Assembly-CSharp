using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class JunTuanManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static JunTuanManager getInstance()
		{
			return JunTuanManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("JunTuanManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1800);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1230, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1231, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1232, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1233, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1234, 3, 3, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1235, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1236, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1237, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1238, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1239, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1224, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1221, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1223, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1220, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1226, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1227, 2, 2, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1228, 1, 1, JunTuanManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, JunTuanManager.getInstance());
			GameManager.GameConfigMgr.UpdateGameConfigItem("juntuanbanghuimax", this.RuntimeData.LegionsNeed.ToString(), false);
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, JunTuanManager.getInstance());
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
			case 1220:
				return this.ProcessGetJunTuanRankListCmd(client, nID, bytes, cmdParams);
			case 1221:
				return this.ProcessGetJunTuanTaskListCmd(client, nID, bytes, cmdParams);
			case 1223:
				return this.ProcessGetJunTuanTaskAwardCmd(client, nID, bytes, cmdParams);
			case 1224:
				return this.ProcessJunTuanCreateCmd(client, nID, bytes, cmdParams);
			case 1226:
				return this.ProcessGetJunTuanRoleListCmd(client, nID, bytes, cmdParams);
			case 1227:
				return this.ProcessQuitJunTuanCmd(client, nID, bytes, cmdParams);
			case 1228:
				return this.ProcessDestroyJunTuanCmd(client, nID, bytes, cmdParams);
			case 1230:
				return this.ProcessGetJunTuanListCmd(client, nID, bytes, cmdParams);
			case 1231:
				return this.ProcessGetJunTuanDataCmd(client, nID, bytes, cmdParams);
			case 1232:
				return this.ProcessGetJunTuanBangHuiListCmd(client, nID, bytes, cmdParams);
			case 1233:
				return this.ProcessJunTuanJoinCmd(client, nID, bytes, cmdParams);
			case 1234:
				return this.ProcessJunTuanJoinResponseCmd(client, nID, bytes, cmdParams);
			case 1235:
				return this.ProcessGetJunTuanRequestListCmd(client, nID, bytes, cmdParams);
			case 1236:
				return this.ProcessGetJunTuanLogListCmd(client, nID, bytes, cmdParams);
			case 1237:
				return this.ProcessJunTuanBulltinCmd(client, nID, bytes, cmdParams);
			case 1238:
				return this.ProcessJunTuanBangHuiZhiWuCmd(client, nID, bytes, cmdParams);
			case 1239:
				return this.ProcessJunTuanRoleZhiWuCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num != 10)
			{
				if (num == 14)
				{
					PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
					if (null != playerInitGameEventObject)
					{
						this.OnInitGame(playerInitGameEventObject.getPlayer());
					}
				}
			}
			else
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject != null && null != playerDeadEventObject.getAttackerRole())
				{
					this.AddJunTuanTaskValue(playerDeadEventObject.getAttackerRole(), 4, 1);
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
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.LegionsNeed = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsNeed", -1);
					this.RuntimeData.LegionsCreateCD = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsCreateCD", -1);
					this.RuntimeData.LegionsCastZuanShi = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsCastZuanShi", -1);
					this.RuntimeData.LegionsJionCD = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsJionCD", -1);
					this.RuntimeData.LegionsEliteNum = (int)GameManager.systemParamsList.GetParamValueIntByName("LegionsEliteNum", -1);
					this.RuntimeData.LegionProsperityCost = GameManager.systemParamsList.GetParamValueIntArrayByName("LegionProsperityCost", ',');
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("LegionTasksTime");
					if (!ConfigHelper.ParserTimeRangeListWithDay2(this.RuntimeData.TaskStartEndTimeList, paramValueByName, true, '|', '-', ',') || this.RuntimeData.TaskStartEndTimeList.Count != 2)
					{
						LogManager.WriteLog(1000, string.Format("解析systemparams.xml的LegionTasksTime出错", text), null, true);
					}
					text = "Config/LegionsManager.xml";
					string text2 = Global.GameResPath(text);
					this.RuntimeData.RolePermissionDict.Load(text2, null);
					text = "Config/LegionTasks.xml";
					text2 = Global.GameResPath(text);
					this.RuntimeData.TaskList.Load(text2, null);
					this.RuntimeData.TaskCount = 0;
					this.RuntimeData.Task2IdxDict.Clear();
					this.RuntimeData.KillMonsterIds.Clear();
					foreach (KeyValuePair<int, JunTuanTaskInfo> keyValuePair in this.RuntimeData.TaskList.Value)
					{
						this.RuntimeData.Task2IdxDict[keyValuePair.Key] = this.RuntimeData.TaskCount++;
						if (keyValuePair.Value.CompleteType == 1)
						{
							foreach (int item in keyValuePair.Value.TypeID)
							{
								this.RuntimeData.KillMonsterIds.Add(item);
							}
						}
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public bool ProcessGetJunTuanListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanMiniData> cmdData = null;
				if (this.IsGongNengOpened(client, false))
				{
					int faction = client.ClientData.Faction;
					cmdData = JunTuanClient.getInstance().GetJunTuanList(faction);
				}
				client.sendCmd<List<JunTuanMiniData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				JunTuanData junTuanData = null;
				if (this.IsGongNengOpened(client, false))
				{
					int faction = client.ClientData.Faction;
					int junTuanId = client.ClientData.JunTuanId;
					if (faction > 0 && junTuanId > 0)
					{
						junTuanData = JunTuanClient.getInstance().GetJunTuanData(faction, junTuanId, true);
						bool bIconState = junTuanData != null && client.ClientData.JunTuanZhiWu == 1 && junTuanData.RequestCount > 0;
						if (client._IconStateMgr.AddFlushIconState(15005, bIconState))
						{
							client._IconStateMgr.SendIconStateToClient(client);
						}
					}
				}
				client.sendCmd<JunTuanData>(nID, junTuanData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanCreateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string text = cmdParams[1];
				int num = NameServerNamager.CheckInvalidCharacters(text, false);
				if (num < 0)
				{
					num = -1027;
				}
				else
				{
					int faction = client.ClientData.Faction;
					if (faction <= 0 || client.ClientData.BHZhiWu != 1)
					{
						num = -1002;
					}
					else if (client.ClientData.JunTuanId != 0)
					{
						num = -1020;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						num = -12;
					}
					else if (client.ClientData.UserMoney < this.RuntimeData.LegionsCastZuanShi)
					{
						num = -10;
					}
					else
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(client.ClientData.RoleID, faction, client.ServerId);
						if (null == bangHuiDetailData)
						{
							num = -15;
						}
						else if (bangHuiDetailData.QiLevel < this.RuntimeData.LegionsNeed)
						{
							num = -1008;
						}
						else
						{
							JunTuanRequestData junTuanRequestData = new JunTuanRequestData();
							junTuanRequestData.BhId = faction;
							junTuanRequestData.BhName = bangHuiDetailData.BHName;
							junTuanRequestData.BhZoneId = bangHuiDetailData.ZoneID;
							junTuanRequestData.LeaderRoleId = client.ClientData.RoleID;
							junTuanRequestData.LeaderName = client.ClientData.RoleName;
							junTuanRequestData.LeaderZoneId = client.ClientData.ZoneID;
							junTuanRequestData.ZhanLi = bangHuiDetailData.TotalCombatForce;
							junTuanRequestData.RoleNum = bangHuiDetailData.TotalNum;
							junTuanRequestData.JunTuanName = text;
							junTuanRequestData.Occupation = client.ClientData.Occupation;
							num = JunTuanClient.getInstance().CreateJunTuan(junTuanRequestData);
							if (num >= 0)
							{
								int num2 = num;
								num = this.UpdateJunTuanRoleList(faction, client.ServerId);
								if (num >= 0)
								{
									int userMoney = client.ClientData.UserMoney;
									if (!GameManager.ClientMgr.SubUserMoney(client, this.RuntimeData.LegionsCastZuanShi, "创建军团", true, true, true, true, DaiBiSySType.None))
									{
										num = -10;
										JunTuanClient.getInstance().DestroyJunTuan(faction, num2);
									}
									else
									{
										string strCostList = EventLogManager.NewResPropString(ResLogType.ZuanShi, new object[]
										{
											-this.RuntimeData.LegionsCastZuanShi,
											userMoney,
											client.ClientData.UserMoney
										});
										EventLogManager.AddJunTuanCreateEvent(client, num2, faction, strCostList);
										JunTuanBangHuiMiniData data = new JunTuanBangHuiMiniData
										{
											BhId = faction,
											JunTuanId = num2,
											JunTuanName = text,
											JunTuanZhiWu = 1
										};
										this.UpdateBhJunTuan(data);
										num = num2;
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanBulltinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string text = cmdParams[1];
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int num;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					num = -1024;
				}
				else
				{
					num = NameServerNamager.CheckInvalidCharacters(text, false);
					if (num >= 0)
					{
						if (!this.IsGongNengOpened(client, false))
						{
							num = -12;
						}
						else
						{
							long num2 = TimeUtil.NOW();
							JunTuanRolePermissionInfo rolePermitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
							if (rolePermitionInfo.BulletinCD <= 0 || client.ClientData.LastJunTuanBulletinTicks > num2)
							{
								num = -2007;
							}
							else
							{
								client.ClientData.LastJunTuanBulletinTicks = num2 + (long)(rolePermitionInfo.BulletinCD * 1000);
								num = JunTuanClient.getInstance().ChangeJunTuanBulltin(faction, client.ClientData.JunTuanId, text);
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessQuitJunTuanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int cmdData;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					cmdData = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					cmdData = -12;
				}
				else if (KarenBattleManager.getInstance().InActivityTime())
				{
					cmdData = -2002;
				}
				else
				{
					JunTuanRolePermissionInfo rolePermitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
					if (faction != num)
					{
						if (rolePermitionInfo.Manager <= 0)
						{
							cmdData = -12;
							goto IL_F1;
						}
					}
					else if (rolePermitionInfo.Quit <= 0)
					{
						cmdData = -12;
						goto IL_F1;
					}
					cmdData = JunTuanClient.getInstance().QuitJunTuan(faction, client.ClientData.JunTuanId, num);
				}
				IL_F1:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessDestroyJunTuanCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				int num;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
				{
					num = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					num = -12;
				}
				else
				{
					JunTuanRolePermissionInfo rolePermitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
					if (rolePermitionInfo.Dissolution <= 0)
					{
						num = -12;
					}
					else if (KarenBattleManager.getInstance().InActivityTime())
					{
						num = -2002;
					}
					else
					{
						num = JunTuanClient.getInstance().DestroyJunTuan(faction, client.ClientData.JunTuanId);
						if (0 == num)
						{
							EventLogManager.AddJunTuanDestroyEvent(client, junTuanId, faction);
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanBangHuiZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (num2 > 0 && num2 != faction)
				{
					if (faction <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
					{
						num = -1024;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						num = -12;
					}
					else if (KarenBattleManager.getInstance().InActivityTime())
					{
						num = -2002;
					}
					else
					{
						num = JunTuanClient.getInstance().JunTuanChangeBangHuiZhiWu(faction, client.ClientData.JunTuanId, num2, 1);
						if (0 == num)
						{
							JunTuanData junTuanData = JunTuanClient.getInstance().GetJunTuanData(client.ClientData.Faction, client.ClientData.JunTuanId, true);
							if (null != junTuanData)
							{
								EventLogManager.AddJunTuanZhiWuEvent(client, 1, junTuanData.LeaderRoleId, 1);
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanRoleZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				List<int> list = DataHelper.BytesToObject<List<int>>(bytes, 0, bytes.Length);
				if (list.Count < 2 || list[0] != client.ClientData.RoleID || list[1] != 3)
				{
					num = -18;
				}
				else if (list.Count > this.RuntimeData.LegionsEliteNum + 2)
				{
					num = -1034;
				}
				else
				{
					int faction = client.ClientData.Faction;
					int junTuanId = client.ClientData.JunTuanId;
					if (faction <= 0 || client.ClientData.BHZhiWu != 1 || junTuanId <= 0)
					{
						num = -1024;
					}
					else if (this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu).AppointElite <= 0)
					{
						num = -12;
					}
					else if (!this.IsGongNengOpened(client, false))
					{
						num = -12;
					}
					else
					{
						List<JunTuanRoleData> junTuanRoleList = this.GetJunTuanRoleList(faction, client.ServerId);
						HashSet<int> hashSet = new HashSet<int>();
						HashSet<int> hashSet2 = new HashSet<int>();
						for (int i = 0; i < junTuanRoleList.Count; i++)
						{
							hashSet.Add(junTuanRoleList[i].RoleId);
						}
						for (int i = 2; i < list.Count; i++)
						{
							hashSet2.Add(list[i]);
							if (!hashSet.Contains(list[i]))
							{
								num = -1000;
							}
						}
						if (num >= 0)
						{
							foreach (JunTuanRoleData junTuanRoleData in junTuanRoleList)
							{
								if (junTuanRoleData.RoleId == client.ClientData.RoleID)
								{
									junTuanRoleData.JuTuanZhiWu = 2;
								}
								else
								{
									junTuanRoleData.JuTuanZhiWu = (hashSet2.Contains(junTuanRoleData.RoleId) ? 3 : 0);
								}
								EventLogManager.AddJunTuanZhiWuEvent(client, client.ClientData.JunTuanZhiWu, junTuanRoleData.RoleId, junTuanRoleData.JuTuanZhiWu);
							}
							Global.SendToDB<List<int>>(1240, list, client.ServerId);
							num = JunTuanClient.getInstance().UpdateRoleDataList(faction, junTuanRoleList);
							foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
							{
								if (gameClient.ClientData.JunTuanId == junTuanId && gameClient.ClientData.BHZhiWu != 1)
								{
									int num2 = hashSet2.Contains(gameClient.ClientData.RoleID) ? 3 : 0;
									if (num2 != gameClient.ClientData.JunTuanZhiWu)
									{
										JunTuanBangHuiMiniData data = new JunTuanBangHuiMiniData(gameClient.ClientData.Faction, client.ClientData.JunTuanId, gameClient.ClientData.JunTuanName, num2, gameClient.ClientData.LingDi);
										this.JunTuanZhiWuChanged(gameClient, data);
									}
								}
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanRequestListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRequestData> cmdData = null;
				int faction = client.ClientData.Faction;
				if (faction > 0 && client.ClientData.BHZhiWu == 1)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							cmdData = JunTuanClient.getInstance().GetJunTuanRequestList(faction);
						}
					}
				}
				client.sendCmd<List<JunTuanRequestData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanLogListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanEventLog> cmdData = null;
				int faction = client.ClientData.Faction;
				if (faction > 0)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							cmdData = JunTuanClient.getInstance().GetJunTuanLogList(faction);
						}
					}
				}
				client.sendCmd<List<JunTuanEventLog>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanRoleListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRoleData> list = new List<JunTuanRoleData>();
				int faction = client.ClientData.Faction;
				if (faction > 0)
				{
					int junTuanId = client.ClientData.JunTuanId;
					if (junTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							Dictionary<int, JunTuanRoleData> dictionary = new Dictionary<int, JunTuanRoleData>();
							List<JunTuanRoleData> junTuanRoleList = this.GetJunTuanRoleList(faction, client.ServerId);
							List<JunTuanRoleData> junTuanRoleList2 = JunTuanClient.getInstance().GetJunTuanRoleList(faction, junTuanId);
							if (junTuanRoleList2 != null && junTuanRoleList2.Count > 0)
							{
								foreach (JunTuanRoleData junTuanRoleData in junTuanRoleList2)
								{
									if (junTuanRoleData.BhId != faction)
									{
										list.Add(junTuanRoleData);
									}
									else
									{
										dictionary[junTuanRoleData.RoleId] = junTuanRoleData;
									}
								}
							}
							foreach (JunTuanRoleData junTuanRoleData in junTuanRoleList)
							{
								list.Add(junTuanRoleData);
								JunTuanRoleData junTuanRoleData2;
								if (dictionary.TryGetValue(junTuanRoleData.RoleId, out junTuanRoleData2) && (junTuanRoleData2.JuTuanZhiWu == 2 || junTuanRoleData2.JuTuanZhiWu == 1))
								{
									junTuanRoleData.JuTuanZhiWu = junTuanRoleData2.JuTuanZhiWu;
								}
							}
						}
					}
				}
				client.sendCmd<List<JunTuanRoleData>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanRankListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanRankData> cmdData = null;
				int faction = client.ClientData.Faction;
				if (faction > 0)
				{
					if (client.ClientData.JunTuanId != 0)
					{
						if (this.IsGongNengOpened(client, false))
						{
							cmdData = JunTuanClient.getInstance().GetJunTuanRankingData();
						}
					}
				}
				client.sendCmd<List<JunTuanRankData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int junTuanId = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					num = -1002;
				}
				else if (client.ClientData.JunTuanId != 0)
				{
					num = -1020;
				}
				else if (this.IsGongNengOpened(client, false))
				{
					if (KarenBattleManager.getInstance().InActivityTime())
					{
						num = -2002;
					}
					else
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(client.ClientData.RoleID, faction, client.ServerId);
						if (null == bangHuiDetailData)
						{
							num = -15;
						}
						else if (bangHuiDetailData.QiLevel < this.RuntimeData.LegionsNeed)
						{
							num = -1008;
						}
						else
						{
							JunTuanRequestData junTuanRequestData = new JunTuanRequestData();
							junTuanRequestData.BhId = faction;
							junTuanRequestData.BhName = bangHuiDetailData.BHName;
							junTuanRequestData.BhZoneId = bangHuiDetailData.ZoneID;
							junTuanRequestData.LeaderRoleId = client.ClientData.RoleID;
							junTuanRequestData.LeaderName = client.ClientData.RoleName;
							junTuanRequestData.LeaderZoneId = client.ClientData.ZoneID;
							junTuanRequestData.ZhanLi = bangHuiDetailData.TotalCombatForce;
							junTuanRequestData.RoleNum = bangHuiDetailData.TotalNum;
							junTuanRequestData.JunTuanId = junTuanId;
							junTuanRequestData.Occupation = client.ClientData.Occupation;
							num = this.UpdateJunTuanRoleList(faction, client.ServerId);
							if (num >= 0)
							{
								num = JunTuanClient.getInstance().JoinJunTuan(junTuanRequestData);
							}
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessJunTuanJoinResponseCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int otherBhid = Global.SafeConvertToInt32(cmdParams[1]);
				int num = Global.SafeConvertToInt32(cmdParams[2]);
				int faction = client.ClientData.Faction;
				int cmdData;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					cmdData = -1024;
				}
				else if (client.ClientData.JunTuanId == 0 || this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu).Manager <= 0)
				{
					cmdData = -1024;
				}
				else if (!this.IsGongNengOpened(client, false))
				{
					cmdData = -12;
				}
				else if (KarenBattleManager.getInstance().InActivityTime() && num > 0)
				{
					cmdData = -2002;
				}
				else
				{
					cmdData = JunTuanClient.getInstance().JoinJunTuanResponse(faction, client.ClientData.JunTuanId, otherBhid, num > 0);
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanBangHuiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanBangHuiData> cmdData = null;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				if (faction <= 0)
				{
				}
				int junTuanId = client.ClientData.JunTuanId;
				if (junTuanId <= 0)
				{
				}
				if (this.IsGongNengOpened(client, false))
				{
					if (faction > 0 && num == junTuanId)
					{
						cmdData = JunTuanClient.getInstance().GetJunTuanBangHuiList(faction, junTuanId);
					}
					else
					{
						cmdData = JunTuanClient.getInstance().GetJunTuanBangHuiList(num);
					}
				}
				client.sendCmd<List<JunTuanBangHuiData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetJunTuanTaskAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int taskId = Global.SafeConvertToInt32(cmdParams[1]);
				if (GameFuncControlManager.IsGameFuncDisabled(13))
				{
					return true;
				}
				if (!this.IsGongNengOpened(client, false))
				{
					num = -12;
				}
				else
				{
					bool flag = false;
					JunTuanTaskAllData junTuanTaskAllData;
					num = JunTuanClient.getInstance().GetJunTuanTaskAllData(client.ClientData.Faction, client.ClientData.JunTuanId, out junTuanTaskAllData);
					if (num >= 0)
					{
						if (junTuanTaskAllData != null && junTuanTaskAllData.TaskList != null)
						{
							JunTuanTaskData junTuanTaskData = junTuanTaskAllData.TaskList.Find((JunTuanTaskData x) => x.TaskId == taskId);
							if (junTuanTaskData != null && junTuanTaskData.TaskState == 1L)
							{
								flag = true;
							}
						}
						if (!flag)
						{
							num = -1028;
						}
						else
						{
							string text = Global.GetRoleParamByName(client, "20018");
							JunTuanTaskInfo junTuanTaskInfo;
							int num2;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.TaskList.Value.TryGetValue(taskId, out junTuanTaskInfo))
								{
									num = -1028;
									goto IL_463;
								}
								if (!this.RuntimeData.Task2IdxDict.TryGetValue(taskId, out num2) || num2 >= this.RuntimeData.TaskCount)
								{
									num = -1028;
									goto IL_463;
								}
							}
							int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
							int[] array = new int[this.RuntimeData.TaskCount];
							if (!string.IsNullOrEmpty(text))
							{
								string[] array2 = text.Split(new char[]
								{
									','
								});
								int num3;
								if (array2.Length == 2 && int.TryParse(array2[0], out num3))
								{
									if (num3 >= weekStartDayIdNow)
									{
										int i = 0;
										while (i < array2[1].Length && i < this.RuntimeData.TaskCount)
										{
											array[i] = ((array2[1][i] == '0') ? 0 : 1);
											i++;
										}
									}
								}
							}
							if (array[num2] == 1)
							{
								num = -200;
							}
							else
							{
								array[num2] = 1;
								List<GoodsData> list = Global.ConvertToGoodsDataList(junTuanTaskInfo.Item.Items, -1);
								if (!Global.CanAddGoodsDataList(client, list))
								{
									num = -100;
								}
								else
								{
									text = string.Format("{0},{1}", weekStartDayIdNow, string.Join<int>("", array));
									Global.SaveRoleParamsStringToDB(client, "20018", text, true);
									if (junTuanTaskInfo.Exp > 0)
									{
										long expMultiByZhuanShengExpXiShu = Global.GetExpMultiByZhuanShengExpXiShu(client, (long)junTuanTaskInfo.Exp);
										GameManager.ClientMgr.ProcessRoleExperience(client, expMultiByZhuanShengExpXiShu, true, false, false, "none");
									}
									if (junTuanTaskInfo.ZhanGong > 0)
									{
										int zhanGong = junTuanTaskInfo.ZhanGong;
										GameManager.ClientMgr.AddBangGong(client, ref zhanGong, AddBangGongTypes.JunTuanTaskAwards, 0);
									}
									if (!Global.CanAddGoodsDataList(client, list))
									{
										GameManager.ClientMgr.SendMailWhenPacketFull(client, list, GLang.GetLang(2608, new object[0]), GLang.GetLang(2608, new object[0]));
									}
									else
									{
										for (int i = 0; i < list.Count; i++)
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list[i].GoodsID, list[i].GCount, list[i].Quality, "", list[i].Forge_level, list[i].Binding, 0, "", true, 1, "阵营战排名奖励", "1900-01-01 12:00:00", 0, list[i].BornIndex, list[i].Lucky, 0, list[i].ExcellenceInfo, list[i].AppendPropLev, 0, null, null, 0, true);
										}
									}
									num = 0;
								}
							}
						}
					}
				}
				IL_463:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool ProcessGetJunTuanTaskListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<JunTuanTaskData> list = new List<JunTuanTaskData>();
				if (GameFuncControlManager.IsGameFuncDisabled(13))
				{
					return true;
				}
				int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
				string roleParamByName = Global.GetRoleParamByName(client, "20018");
				int[] array = new int[this.RuntimeData.TaskCount];
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					string[] array2 = roleParamByName.Split(new char[]
					{
						','
					});
					int num;
					if (array2.Length == 2 && int.TryParse(array2[0], out num))
					{
						if (num >= weekStartDayIdNow)
						{
							int num2 = 0;
							while (num2 < array2[1].Length && num2 < this.RuntimeData.TaskCount)
							{
								array[num2] = ((array2[1][num2] == '0') ? 0 : 1);
								num2++;
							}
						}
					}
				}
				JunTuanTaskAllData junTuanTaskAllData;
				if (JunTuanClient.getInstance().GetJunTuanTaskAllData(client.ClientData.Faction, client.ClientData.JunTuanId, out junTuanTaskAllData) >= 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (junTuanTaskAllData != null && junTuanTaskAllData.TaskList != null)
						{
							foreach (JunTuanTaskData junTuanTaskData in junTuanTaskAllData.TaskList)
							{
								JunTuanTaskData junTuanTaskData2 = junTuanTaskData.Clone();
								list.Add(junTuanTaskData2);
								int num3;
								if (this.RuntimeData.Task2IdxDict.TryGetValue(junTuanTaskData2.TaskId, out num3) && num3 < array.Length && array[num3] == 1)
								{
									junTuanTaskData2.HasGet = 1;
								}
								if (junTuanTaskData2.TaskState == 1L && junTuanTaskData2.HasGet == 0)
								{
								}
							}
						}
					}
				}
				client.sendCmd<List<JunTuanTaskData>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(13) && GlobalNew.IsGongNengOpened(client, 88, hint);
		}

		public JunTuanRolePermissionInfo GetRolePermitionInfo(int zhiwu)
		{
			if (zhiwu == 0)
			{
				zhiwu = 4;
			}
			JunTuanRolePermissionInfo junTuanRolePermissionInfo = null;
			JunTuanRolePermissionInfo result;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.RolePermissionDict.Value.TryGetValue(zhiwu, out junTuanRolePermissionInfo))
				{
					result = junTuanRolePermissionInfo;
				}
				else
				{
					result = new JunTuanRolePermissionInfo();
				}
			}
			return result;
		}

		public void UpdateJunTuanData(KuaFuData<JunTuanData> data)
		{
		}

		public void UpdateBhJunTuan(JunTuanBangHuiMiniData data)
		{
			GameManager.ClientMgr.BroadcastServerCmd<JunTuanBangHuiMiniData>(1229, data, true);
			int junTuanZhiWu = data.JunTuanZhiWu;
			DateTime dateTime = TimeUtil.NowDateTime();
			List<BangHuiMemberData> list = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", 0, data.BhId), 0);
			foreach (BangHuiMemberData bangHuiMemberData in list)
			{
				int roleID = bangHuiMemberData.RoleID;
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
				if (null == gameClient)
				{
					if (data.JunTuanChanged > 0)
					{
						GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", roleID, "10182", dateTime.Ticks.ToString()), null, 0);
					}
				}
				else
				{
					if (data.JunTuanChanged > 0)
					{
						Global.SaveRoleParamsDateTimeToDB(gameClient, "10182", dateTime, true);
					}
					if (gameClient.ClientData.Faction == data.BhId && (gameClient.ClientData.JunTuanId != data.JunTuanId || gameClient.ClientData.JunTuanName != data.JunTuanName || gameClient.ClientData.JunTuanZhiWu != data.JunTuanZhiWu))
					{
						gameClient.ClientData.JunTuanId = data.JunTuanId;
						gameClient.ClientData.JunTuanName = data.JunTuanName;
						data.JunTuanZhiWu = this.GetJunTuanZhiWu(gameClient, junTuanZhiWu);
						data.RoleId = gameClient.ClientData.RoleID;
						GameManager.ClientMgr.BroadcastOthersCmdData<JunTuanBangHuiMiniData>(gameClient, 1225, data, true);
						LingDiCaiJiManager.getInstance().UpdateChengHaoBuff(gameClient);
						EraManager.getInstance().OnJunTuanZhiWuChanged(gameClient);
					}
				}
			}
		}

		public void UpdateJunTuanTaskData(JunTuanTaskData data)
		{
		}

		public JunTuanBaseData GetJunTuanBaseDataByJunTuanID(int junTuanID)
		{
			JunTuanBaseData result;
			lock (this.RuntimeData.Mutex)
			{
				JunTuanBaseData junTuanBaseData;
				this.RuntimeData.JunTuanBaseDict.TryGetValue(junTuanID, out junTuanBaseData);
				result = junTuanBaseData;
			}
			return result;
		}

		public void OnInitGame(RoleDataEx dbRd)
		{
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
				int bhid = dbRd.Faction;
				int key;
				if (this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
				{
					JunTuanBaseData junTuanBaseData;
					if (this.RuntimeData.JunTuanBaseDict.TryGetValue(key, out junTuanBaseData))
					{
						dbRd.JunTuanId = junTuanBaseData.JunTuanId;
						dbRd.JunTuanName = junTuanBaseData.JunTuanName;
						dbRd.JunTuanZhiWu = this.GetJunTuanZhiWu(dbRd, (junTuanBaseData.BhList.FindIndex((int x) => x == bhid) == 0) ? 1 : 0);
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
		}

		public void OnInitGame(GameClient client)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.RuntimeData.Mutex, ref flag);
					int bhid = client.ClientData.Faction;
					int key;
					if (!this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(bhid, out key))
					{
						return;
					}
					JunTuanBaseData junTuanBaseData;
					if (this.RuntimeData.JunTuanBaseDict.TryGetValue(key, out junTuanBaseData))
					{
						client.ClientData.JunTuanId = junTuanBaseData.JunTuanId;
						client.ClientData.JunTuanName = junTuanBaseData.JunTuanName;
						client.ClientData.JunTuanZhiWu = this.GetJunTuanZhiWu(client, (junTuanBaseData.BhList.FindIndex((int x) => x == bhid) == 0) ? 1 : 0);
					}
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
				if (client.ClientData.JunTuanId > 0)
				{
					JunTuanData junTuanData = JunTuanClient.getInstance().GetJunTuanData(client.ClientData.Faction, client.ClientData.JunTuanId, false);
					if (null != junTuanData)
					{
						if (junTuanData.Point < this.RuntimeData.LegionProsperityCost[2])
						{
							GameManager.ClientMgr.NotifyHintMsgDelay(client, GLang.GetLang(2609, new object[0]));
						}
					}
				}
			}
		}

		public void JunTuanChat(GameClient client, string text)
		{
			long num = TimeUtil.NOW();
			JunTuanRolePermissionInfo rolePermitionInfo = this.GetRolePermitionInfo(client.ClientData.JunTuanZhiWu);
			if (null != rolePermitionInfo)
			{
				if (Global.GetUnionLevel(client, false) < Global.GetUnionLevel(rolePermitionInfo.TalkLevel[0], rolePermitionInfo.TalkLevel[1], false))
				{
					GameManager.ClientMgr.NotifyHintMsg(client, GLang.GetLang(2610, new object[0]));
				}
				else
				{
					int num2 = Math.Max(rolePermitionInfo.TalkCD * 1000, 3000);
					if (num - client.ClientData.LastJunTuanChatTicks < (long)num2)
					{
						long num3 = ((long)num2 - num - client.ClientData.LastJunTuanChatTicks) / 1000L + 1L;
						GameManager.ClientMgr.NotifyHintMsg(client, string.Format(GLang.GetLang(2611, new object[0]), num3));
					}
					else
					{
						client.ClientData.LastJunTuanChatTicks = num;
						KFChat kfchat = null;
						lock (this.RuntimeData.Mutex)
						{
							int faction = client.ClientData.Faction;
							int num4;
							if (!this.RuntimeData.BangHuiJunTuanIdDict.TryGetValue(faction, out num4) || num4 <= 0)
							{
								return;
							}
							kfchat = new KFChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, num4);
							this.RuntimeData.JunTuanChatList.Add(kfchat);
						}
						this.BroadcastJunTuanChatMsg(kfchat);
					}
				}
			}
		}

		private int GetJunTuanZhiWu(RoleDataEx dbRd, int bhZhiWu)
		{
			int faction = dbRd.Faction;
			int result;
			if (dbRd.BHZhiWu == 1)
			{
				if (bhZhiWu > 0)
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = dbRd.JunTuanZhiWu;
			}
			return result;
		}

		private int GetJunTuanZhiWu(GameClient client, int bhZhiWu)
		{
			int faction = client.ClientData.Faction;
			int result;
			if (client.ClientData.BHZhiWu == 1)
			{
				if (bhZhiWu > 0)
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = client.ClientData.JunTuanZhiWu;
			}
			return result;
		}

		private List<JunTuanRoleData> GetJunTuanRoleList(int bhid, int serverId)
		{
			List<JunTuanRoleData> list = new List<JunTuanRoleData>();
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bhid, serverId);
			List<BangHuiMemberData> list2 = Global.sendToDB<List<BangHuiMemberData>, string>(299, string.Format("{0}:{1}", 0, bhid), serverId);
			foreach (BangHuiMemberData bangHuiMemberData in list2)
			{
				int juTuanZhiWu = (bangHuiMemberData.BHZhiwu == 1) ? 2 : bangHuiMemberData.JunTuanZhiWu;
				JunTuanRoleData item = new JunTuanRoleData
				{
					BhId = bhid,
					RoleId = bangHuiMemberData.RoleID,
					RoleName = bangHuiMemberData.RoleName,
					ZoneId = bangHuiMemberData.ZoneID,
					BhName = bangHuiMiniData.BHName,
					BhZoneId = bangHuiMemberData.ZoneID,
					ZhanLi = bangHuiMemberData.BangHuiMemberCombatForce,
					ChangeLifeCount = bangHuiMemberData.BangHuiMemberChangeLifeLev,
					Level = bangHuiMemberData.Level,
					JuTuanZhiWu = juTuanZhiWu,
					Occu = bangHuiMemberData.Occupation,
					OnlineState = bangHuiMemberData.OnlineState
				};
				list.Add(item);
			}
			return list;
		}

		public void DelayUpdateJunTuanRoleList(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.HasUpdateRoleDataHashSet.Add(bhid);
			}
		}

		public int UpdateJunTuanRoleList(int bhid, int serverId)
		{
			List<JunTuanRoleData> junTuanRoleList = this.GetJunTuanRoleList(bhid, serverId);
			return JunTuanClient.getInstance().UpdateRoleDataList(bhid, junTuanRoleList);
		}

		public void AddJunTuanTaskValue(int bhid, int junTuanId, int sceneType, int taskValue)
		{
			if (bhid > 0 && junTuanId > 0)
			{
				TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow();
				if (!(timeOfWeekNow < this.RuntimeData.TaskStartEndTimeList[0]) && !(timeOfWeekNow > this.RuntimeData.TaskStartEndTimeList[1]))
				{
					lock (this.RuntimeData.Mutex)
					{
						foreach (JunTuanTaskInfo junTuanTaskInfo in this.RuntimeData.TaskList.Value.Values)
						{
							if (junTuanTaskInfo.CompleteType == 2)
							{
								if (junTuanTaskInfo.TypeID.Contains(sceneType))
								{
									this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(bhid, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
								}
							}
						}
					}
				}
			}
		}

		public void AddJunTuanTaskValue(GameClient client, int taskType, int taskValue)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (faction > 0 && junTuanId > 0)
				{
					TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow();
					if (!(timeOfWeekNow < this.RuntimeData.TaskStartEndTimeList[0]) && !(timeOfWeekNow > this.RuntimeData.TaskStartEndTimeList[1]))
					{
						lock (this.RuntimeData.Mutex)
						{
							foreach (JunTuanTaskInfo junTuanTaskInfo in this.RuntimeData.TaskList.Value.Values)
							{
								if (junTuanTaskInfo.CompleteType == taskType)
								{
									switch (taskType)
									{
									case 1:
									case 2:
										this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(faction, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										break;
									case 3:
										this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(faction, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										break;
									case 4:
									{
										int mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
										if (junTuanTaskInfo.TypeID.Contains(mapSceneType))
										{
											this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(faction, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										}
										break;
									}
									case 5:
										if (junTuanTaskInfo.TypeID.Contains(client.ClientData.FuBenID))
										{
											this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(faction, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
										}
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void AddJunTuanTaskValue(GameClient client, Monster monter, int taskType, int taskValue)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				int faction = client.ClientData.Faction;
				int junTuanId = client.ClientData.JunTuanId;
				if (faction > 0 || junTuanId > 0)
				{
					TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow();
					if (!(timeOfWeekNow < this.RuntimeData.TaskStartEndTimeList[0]) && !(timeOfWeekNow > this.RuntimeData.TaskStartEndTimeList[1]))
					{
						lock (this.RuntimeData.Mutex)
						{
							if (this.RuntimeData.KillMonsterIds.Contains(monter.MonsterInfo.ExtensionID))
							{
								foreach (JunTuanTaskInfo junTuanTaskInfo in this.RuntimeData.TaskList.Value.Values)
								{
									if (junTuanTaskInfo.CompleteType == taskType)
									{
										if (taskType == 1)
										{
											if (junTuanTaskInfo.TypeID.Contains(monter.MonsterInfo.ExtensionID))
											{
												this.RuntimeData.JunTuanTaskQueue.Enqueue(new Tuple<int, int, int, int, long>(faction, junTuanId, junTuanTaskInfo.ID, taskValue, TimeUtil.NowDateTime().Ticks));
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

		private void TimerProc(object sender, EventArgs e)
		{
			bool flag = false;
			long num = TimeUtil.NOW();
			Dictionary<int, List<GameClient>> dictionary = new Dictionary<int, List<GameClient>>();
			lock (this.RuntimeData.Mutex)
			{
				if (num > this.RuntimeData.NextUpdateTicks)
				{
					this.RuntimeData.NextUpdateTicks = num + 2200L;
					flag = true;
				}
			}
			if (flag)
			{
				foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
				{
					int num2 = gameClient.ClientData.Faction;
					if (num2 > 0)
					{
						List<GameClient> list;
						if (!dictionary.TryGetValue(num2, out list))
						{
							list = new List<GameClient>();
							dictionary[num2] = list;
						}
						list.Add(gameClient);
						if (gameClient.ClientData.JunTuanId > 0 && !gameClient.ClientSocket.IsKuaFuLogin && this.RuntimeData.HasUpdateRoleDataHashSet.Contains(num2))
						{
							this.RuntimeData.HasUpdateRoleDataHashSet.Remove(num2);
							this.UpdateJunTuanRoleList(num2, gameClient.ServerId);
						}
					}
					else if (gameClient.ClientData.JunTuanId != 0 || gameClient.ClientData.JunTuanName != null)
					{
						JunTuanBangHuiMiniData data = new JunTuanBangHuiMiniData();
						this.JunTuanZhiWuChanged(gameClient, data);
					}
				}
			}
			List<JunTuanBaseData> junTuanBaseDataList = JunTuanClient.getInstance().GetJunTuanBaseDataList(true);
			if (null != junTuanBaseDataList)
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.BangHuiJunTuanIdDict.Clear();
					this.RuntimeData.JunTuanBaseDict.Clear();
					foreach (JunTuanBaseData junTuanBaseData in junTuanBaseDataList)
					{
						if (null != junTuanBaseData.BhList)
						{
							int bhZhiWu = 1;
							foreach (int num2 in junTuanBaseData.BhList)
							{
								this.RuntimeData.BangHuiJunTuanIdDict[num2] = junTuanBaseData.JunTuanId;
								List<GameClient> list;
								if (dictionary.TryGetValue(num2, out list) && list != null)
								{
									foreach (GameClient gameClient2 in list)
									{
										int junTuanZhiWu = this.GetJunTuanZhiWu(gameClient2, bhZhiWu);
										if (gameClient2.ClientData.JunTuanId != junTuanBaseData.JunTuanId || gameClient2.ClientData.JunTuanName != junTuanBaseData.JunTuanName || gameClient2.ClientData.JunTuanZhiWu != junTuanZhiWu || gameClient2.ClientData.LingDi != junTuanBaseData.LingDi)
										{
											JunTuanBangHuiMiniData data = new JunTuanBangHuiMiniData(num2, junTuanBaseData.JunTuanId, junTuanBaseData.JunTuanName, junTuanZhiWu, junTuanBaseData.LingDi);
											this.JunTuanZhiWuChanged(gameClient2, data);
										}
									}
								}
								bhZhiWu = 0;
							}
							this.RuntimeData.JunTuanBaseDict[junTuanBaseData.JunTuanId] = junTuanBaseData;
						}
					}
					foreach (KeyValuePair<int, List<GameClient>> keyValuePair in dictionary)
					{
						int num2 = keyValuePair.Key;
						if (!this.RuntimeData.BangHuiJunTuanIdDict.ContainsKey(num2))
						{
							List<GameClient> list = keyValuePair.Value;
							JunTuanBangHuiMiniData junTuanBangHuiMiniData = new JunTuanBangHuiMiniData(num2, 0, null, 0, 0);
							foreach (GameClient gameClient2 in list)
							{
								junTuanBangHuiMiniData.JunTuanZhiWu = 0;
								if (gameClient2.ClientData.JunTuanId != junTuanBangHuiMiniData.JunTuanId || gameClient2.ClientData.JunTuanName != junTuanBangHuiMiniData.JunTuanName || (gameClient2.ClientData.JunTuanZhiWu != junTuanBangHuiMiniData.JunTuanZhiWu | gameClient2.ClientData.LingDi != junTuanBangHuiMiniData.LingDi))
								{
									this.JunTuanZhiWuChanged(gameClient2, junTuanBangHuiMiniData);
								}
							}
						}
					}
				}
			}
			List<Tuple<int, int, int, int, long>> list2 = null;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.JunTuanTaskQueue.Count > 0)
				{
					list2 = new List<Tuple<int, int, int, int, long>>();
					list2.AddRange(this.RuntimeData.JunTuanTaskQueue);
					this.RuntimeData.JunTuanTaskQueue.Clear();
				}
			}
			if (null != list2)
			{
				foreach (Tuple<int, int, int, int, long> tuple in list2)
				{
					if (!JunTuanClient.getInstance().IsTaskComplete(tuple.Item1, tuple.Item2, tuple.Item3))
					{
						int num3 = JunTuanClient.getInstance().JunTuanChangeTaskValue(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5);
						if (num3 == -11000)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.RuntimeData.JunTuanTaskQueue.Enqueue(tuple);
							}
						}
						else if (num3 == 1)
						{
						}
					}
				}
			}
			List<KFChat> list3 = null;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.JunTuanChatList.Count > 0)
				{
					list3 = new List<KFChat>(this.RuntimeData.JunTuanChatList);
					this.RuntimeData.JunTuanChatList.Clear();
				}
			}
			if (null != list3)
			{
				JunTuanClient.getInstance().JunTuanChat(list3);
			}
		}

		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFChat> list = DataHelper.BytesToObject<List<KFChat>>(data, 0, data.Length);
				if (null != list)
				{
					foreach (KFChat kfChat in list)
					{
						this.BroadcastJunTuanChatMsg(kfChat);
					}
				}
			}
		}

		public void BroadcastJunTuanChatMsg(KFChat kfChat)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient.ClientData.JunTuanId == kfChat.JunTuanId)
				{
					gameClient.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		public void JunTuanZhiWuChanged(GameClient client, JunTuanBangHuiMiniData data)
		{
			client.ClientData.JunTuanId = data.JunTuanId;
			client.ClientData.JunTuanName = data.JunTuanName;
			client.ClientData.JunTuanZhiWu = data.JunTuanZhiWu;
			client.ClientData.LingDi = data.LingDi;
			data.RoleId = client.ClientData.RoleID;
			GameManager.ClientMgr.BroadcastOthersCmdData<JunTuanBangHuiMiniData>(client, 1225, data, true);
			LingDiCaiJiManager.getInstance().UpdateChengHaoBuff(client);
			EraManager.getInstance().OnJunTuanZhiWuChanged(client);
		}

		public bool OnBangHuiMemberChanged(GameClient client, int bhid)
		{
			int serverId = client.ServerId;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(client.ClientData.RoleID, bhid, serverId);
			bool result;
			if (null == bangHuiDetailData)
			{
				result = true;
			}
			else if (bangHuiDetailData.QiLevel < this.RuntimeData.LegionsNeed)
			{
				result = true;
			}
			else
			{
				this.UpdateJunTuanRoleList(bhid, serverId);
				result = true;
			}
			return result;
		}

		public bool PreRemoveBangHui(GameClient client)
		{
			int faction = client.ClientData.Faction;
			int serverId = client.ServerId;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(client.ClientData.RoleID, faction, serverId);
			return null == bangHuiDetailData || bangHuiDetailData.QiLevel < this.RuntimeData.LegionsNeed || JunTuanClient.getInstance().RemoveBangHui(faction) >= 0;
		}

		public void OnRoleChangName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleId);
				if (safeClientDataFromLocalOrDB != null && safeClientDataFromLocalOrDB.Faction > 0)
				{
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, safeClientDataFromLocalOrDB.Faction, GameManager.ServerId);
					if (bangHuiDetailData != null && bangHuiDetailData.QiLevel >= this.RuntimeData.LegionsNeed)
					{
						this.UpdateJunTuanRoleList(safeClientDataFromLocalOrDB.Faction, GameManager.ServerId);
					}
				}
			}
		}

		public bool PreRemoveRole(int roleId)
		{
			SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleId);
			bool result;
			if (safeClientDataFromLocalOrDB == null || safeClientDataFromLocalOrDB.Faction <= 0)
			{
				result = true;
			}
			else
			{
				BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, safeClientDataFromLocalOrDB.Faction, GameManager.ServerId);
				if (bangHuiDetailData == null || bangHuiDetailData.QiLevel < this.RuntimeData.LegionsNeed)
				{
					result = true;
				}
				else
				{
					List<JunTuanRoleData> junTuanRoleList = this.GetJunTuanRoleList(safeClientDataFromLocalOrDB.Faction, GameManager.ServerId);
					foreach (JunTuanRoleData junTuanRoleData in junTuanRoleList)
					{
						if (junTuanRoleData.RoleId == roleId)
						{
							if (junTuanRoleData.JuTuanZhiWu == 1 || junTuanRoleData.JuTuanZhiWu == 2)
							{
								return false;
							}
						}
					}
					result = (JunTuanClient.getInstance().UpdateRoleDataList(safeClientDataFromLocalOrDB.Faction, junTuanRoleList) >= 0);
				}
			}
			return result;
		}

		public void OnBangHuiChangeName(int bhid, string newName)
		{
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, bhid, GameManager.ServerId);
			if (bangHuiDetailData != null && bangHuiDetailData.QiLevel >= this.RuntimeData.LegionsNeed)
			{
				JunTuanClient.getInstance().ChangeBangHuiName(bhid, newName);
			}
		}

		public void NotifyJunTuanRequest(int junTuanId, bool icon)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient.ClientData.JunTuanId == junTuanId && gameClient.ClientData.JunTuanZhiWu == 1)
				{
					if (gameClient._IconStateMgr.AddFlushIconState(15005, icon))
					{
						gameClient._IconStateMgr.SendIconStateToClient(gameClient);
					}
				}
			}
		}

		private static JunTuanManager instance = new JunTuanManager();

		public JunTuanRuntimeData RuntimeData = new JunTuanRuntimeData();
	}
}
