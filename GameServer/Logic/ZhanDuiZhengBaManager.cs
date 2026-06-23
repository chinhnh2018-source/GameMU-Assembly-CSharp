using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using AutoCSer.Net.TcpServer;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Contract.Data;
using KF.Remoting;
using KF.TcpCall;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class ZhanDuiZhengBaManager : IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListener, IEventListenerEx, ICopySceneManager
	{
		public static ZhanDuiZhengBaManager getInstance()
		{
			return ZhanDuiZhengBaManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("ZhanDuiZhengBaManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1272, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1273, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1274, 2, 3, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1275, 2, 3, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1276, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1277, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1278, 1, 2, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1279, 0, 0, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1280, 1, 1, ZhanDuiZhengBaManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(13, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(64, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(61, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(62, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(60, 56, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(63, 10000, ZhanDuiZhengBaManager.getInstance());
			this.NotifyEnterHandler = new EventSourceEx<KFCallMsg>.HandlerData
			{
				ID = 56,
				EventType = 10034,
				Handler = new Func<KFCallMsg, bool>(this.KFCallMsgFunc)
			};
			KFCallManager.MsgSource.registerListener(10034, this.NotifyEnterHandler);
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().removeListener(13, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(61, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(62, 10007, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(60, 56, ZhanDuiZhengBaManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(63, 10000, ZhanDuiZhengBaManager.getInstance());
			KFCallManager.MsgSource.removeListener(10034, this.NotifyEnterHandler);
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
			case 1272:
				return this.ProcessGetMainInfoListCmd(client, nID, bytes, cmdParams);
			case 1273:
				return this.ProcessGetZhanDuiListCmd(client, nID, bytes, cmdParams);
			case 1275:
				return this.ProcessSupportCmd(client, nID, bytes, cmdParams);
			case 1276:
				return this.ProcessGetLogCmd(client, nID, bytes, cmdParams);
			case 1277:
				return this.ProcessZhanDuiZhengBaEnterCmd(client, nID, bytes, cmdParams);
			case 1280:
				return this.ProcessSupportListCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					if (playerDeadEventObject.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEventObject.getAttackerRole(), playerDeadEventObject.getPlayer());
					}
				}
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				if (null != onStartPlayGameEventObject)
				{
					this.UpdateChengHaoBuffer(onStartPlayGameEventObject.Client);
					this.GiveSupportAwards(onStartPlayGameEventObject.Client);
					this.GiveRankAwards(onStartPlayGameEventObject.Client);
				}
			}
			else if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject playerLeaveFuBenEventObject = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(playerLeaveFuBenEventObject.getPlayer());
			}
			else if (eventType == 64)
			{
				this.UpdateChengHaoBuffer(eventObject.Params[0] as GameClient);
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 60:
				this.NotifyTimeStateInfoAndScoreInfo(eventObject.Sender as GameClient);
				break;
			case 61:
			{
				EventObjectEx_I1 eventObjectEx_I = eventObject as EventObjectEx_I1;
				if (eventObjectEx_I != null && eventObjectEx_I.Param1 == 35)
				{
					eventObject.Handled = true;
					if (this.OnKuaFuLogin(eventObject.Sender as KuaFuServerLoginData))
					{
						eventObject.Result = true;
					}
				}
				break;
			}
			case 62:
			{
				EventObjectEx_I1 eventObjectEx_I = eventObject as EventObjectEx_I1;
				if (eventObjectEx_I != null && eventObjectEx_I.Param1 == 35)
				{
					eventObject.Handled = true;
					if (this.OnKuaFuInitGame(eventObject.Sender as GameClient))
					{
						eventObject.Handled = true;
						eventObject.Result = true;
					}
				}
				break;
			}
			case 63:
			{
				PreZhanDuiChangeMemberEventObject preZhanDuiChangeMemberEventObject = (PreZhanDuiChangeMemberEventObject)eventObject;
				preZhanDuiChangeMemberEventObject.Handled = this.OnPreZhanDuiChangeMember(preZhanDuiChangeMemberEventObject);
				break;
			}
			}
		}

		public bool KFCallMsgFunc(KFCallMsg msg)
		{
			int kuaFuEventType = msg.KuaFuEventType;
			if (kuaFuEventType == 10034)
			{
				ZhanDuiZhengBaNtfEnterData zhanDuiZhengBaNtfEnterData = msg.Get<ZhanDuiZhengBaNtfEnterData>();
				if (null != zhanDuiZhengBaNtfEnterData)
				{
					GameManager.ClientMgr.BroadZhanDuiMessage<int>(1281, 1, zhanDuiZhengBaNtfEnterData.ZhanDuiID1);
					if (zhanDuiZhengBaNtfEnterData.ZhanDuiID2 != zhanDuiZhengBaNtfEnterData.ZhanDuiID1)
					{
						GameManager.ClientMgr.BroadZhanDuiMessage<int>(1281, 1, zhanDuiZhengBaNtfEnterData.ZhanDuiID2);
					}
				}
			}
			return true;
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.TeamBattleName = GameManager.systemParamsList.GetParamValueIntArrayByName("TeamBattleName", ',');
					if (!this.RuntimeData.Config.Load(Global.GameResPath("Config\\TeamMatch.xml"), Global.GameResPath("Config\\TeamMatchBirthPoint.xml")))
					{
						return false;
					}
					List<ZhanDuiZhengBaAwardsConfig> list = new List<ZhanDuiZhengBaAwardsConfig>();
					text = "Config/TeamMatchAward.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						ZhanDuiZhengBaAwardsConfig zhanDuiZhengBaAwardsConfig = new ZhanDuiZhengBaAwardsConfig();
						zhanDuiZhengBaAwardsConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						zhanDuiZhengBaAwardsConfig.Rank = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "Rank", 0L);
						zhanDuiZhengBaAwardsConfig.TeamPoint = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "TeamPoint", 0L);
						string safeAttributeStr = Global.GetSafeAttributeStr(xelement2, "Award");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							ConfigParser.ParseAwardsItemList(safeAttributeStr, ref zhanDuiZhengBaAwardsConfig.Award, '|', ',');
						}
						list.Add(zhanDuiZhengBaAwardsConfig);
					}
					this.RuntimeData.AwardsConfig = list;
					this.RuntimeData.StartTime = this.RuntimeData.Config.MatchConfigList[0].TimePoints[0];
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public bool ProcessGetMainInfoListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				long num = 0L;
				if (cmdParams.Length >= 2)
				{
					num = Convert.ToInt64(cmdParams[1]);
				}
				AgeDataT<List<ZhanDuiZhengBaZhanDuiData>> ageDataT = new AgeDataT<List<ZhanDuiZhengBaZhanDuiData>>();
				lock (this.RuntimeData.Mutex)
				{
					if (num != this.RuntimeData.SyncData.RoleModTime.Ticks)
					{
						ageDataT.Age = this.RuntimeData.SyncData.RoleModTime.Ticks;
						if (this.RuntimeData.SyncData.RealActID >= 2 || this.RuntimeData.SyncData.HasSeasonEnd)
						{
							List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != zhanDuiList)
							{
								zhanDuiList.Sort(delegate(ZhanDuiZhengBaZhanDuiData x, ZhanDuiZhengBaZhanDuiData y)
								{
									int result;
									if (x.Grade < y.Grade)
									{
										result = -1;
									}
									else
									{
										result = 1;
									}
									return result;
								});
								ageDataT.V = zhanDuiList.TakeWhile((ZhanDuiZhengBaZhanDuiData x) => x.Grade <= 16).ToList<ZhanDuiZhengBaZhanDuiData>();
							}
						}
					}
				}
				client.sendCmd<AgeDataT<List<ZhanDuiZhengBaZhanDuiData>>>(nID, ageDataT, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetZhanDuiListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaZhanDuiData> cmdData = new List<ZhanDuiZhengBaZhanDuiData>();
				lock (this.RuntimeData.Mutex)
				{
					List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
					if (null != zhanDuiList)
					{
						cmdData = zhanDuiList.GetRange(0, zhanDuiList.Count);
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaZhanDuiData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSupportCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				int zhanDuiID = Global.SafeConvertToInt32(cmdParams[1]);
				DateTime dateTime = TimeUtil.NowDateTime();
				long num = Global.GetRoleParamsInt64FromDB(client, "10221");
				int num2 = (int)(num >> 32);
				int num3 = (int)(num & (long)((ulong)-1));
				int[] array = new int[2];
				if (num2 == this.RuntimeData.SyncData.Month)
				{
					long num4 = Global.GetRoleParamsInt64FromDB(client, "10222");
					array[0] = (int)(num4 >> 32);
					array[1] = (int)(num4 & (long)((ulong)-1));
				}
				else
				{
					num3 = 0;
				}
				if (array[0] == zhanDuiID || array[1] == zhanDuiID)
				{
					cmdData = -4032;
				}
				else if (array[0] > 0 && array[1] > 0)
				{
					cmdData = -4033;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						int month = this.RuntimeData.SyncData.Month;
						if (month <= 0)
						{
							cmdData = -400;
							goto IL_2DE;
						}
						List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
						if (zhanDuiList == null || zhanDuiList.Count == 0)
						{
							cmdData = -400;
							goto IL_2DE;
						}
						if (!zhanDuiList.Any((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == zhanDuiID))
						{
							cmdData = -4031;
							goto IL_2DE;
						}
						ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = this.RuntimeData.Config.MatchConfigList.First<ZhanDuiZhengBaMatchConfig>();
						TimeSpan t = new TimeSpan(dateTime.Day, dateTime.Hour, dateTime.Minute, dateTime.Second);
						if (t < zhanDuiZhengBaMatchConfig.LotteryTime[0] || t > zhanDuiZhengBaMatchConfig.LotteryTime[1])
						{
							cmdData = -2001;
							goto IL_2DE;
						}
						if (!MoneyUtil.CheckHasMoney(client, 8, zhanDuiZhengBaMatchConfig.LotteryMoney))
						{
							cmdData = -9;
							goto IL_2DE;
						}
						string text = "";
						MoneyUtil.CostMoney(client, 8, zhanDuiZhengBaMatchConfig.LotteryMoney, ref text, "战队争霸押注", true);
						if (array[0] == 0)
						{
							array[0] = zhanDuiID;
						}
						else if (array[1] == 0)
						{
							array[1] = zhanDuiID;
						}
						num = ((long)month << 32) + (long)num3;
					}
					long num4 = ((long)array[0] << 32) + (long)array[1];
					Global.SaveRoleParamsInt64ValueToDB(client, "10221", num, true);
					Global.SaveRoleParamsInt64ValueToDB(client, "10222", num4, true);
				}
				IL_2DE:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessSupportListCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaZhanDuiData> list = new List<ZhanDuiZhengBaZhanDuiData>();
				long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10221");
				int num = (int)(roleParamsInt64FromDB >> 32);
				int[] array = new int[2];
				if (num == this.RuntimeData.SyncData.Month)
				{
					long roleParamsInt64FromDB2 = Global.GetRoleParamsInt64FromDB(client, "10222");
					array[0] = (int)(roleParamsInt64FromDB2 >> 32);
					array[1] = (int)(roleParamsInt64FromDB2 & (long)((ulong)-1));
					lock (this.RuntimeData.Mutex)
					{
						List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
						if (null != zhanDuiList)
						{
							foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData in zhanDuiList)
							{
								if (zhanDuiZhengBaZhanDuiData.ZhanDuiID == array[0] || zhanDuiZhengBaZhanDuiData.ZhanDuiID == array[1])
								{
									list.Add(zhanDuiZhengBaZhanDuiData);
								}
							}
						}
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaZhanDuiData>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetLogCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<ZhanDuiZhengBaPkLogData> cmdData = new List<ZhanDuiZhengBaPkLogData>();
				lock (this.RuntimeData.Mutex)
				{
					List<ZhanDuiZhengBaPkLogData> pklogList = this.RuntimeData.SyncData.PKLogList;
					if (null != pklogList)
					{
						cmdData = pklogList.GetRange(0, pklogList.Count);
					}
				}
				client.sendCmd<List<ZhanDuiZhengBaPkLogData>>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessZhanDuiZhengBaEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (!this.IsGongNengOpened(client, false))
				{
					cmdData = -12;
				}
				else if (!this.CheckMap(client))
				{
					cmdData = -12;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.SyncData.ZhanDuiList.Any((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == client.ClientData.ZhanDuiID))
						{
							cmdData = -12;
							goto IL_1F3;
						}
					}
					int num;
					int targetServerID;
					string[] array;
					int[] array2;
					ReturnValue<int> returnValue = TcpCall.ZhanDuiZhengBa_K.ZhengBaRequestEnter(client.ClientData.ZhanDuiID, out num, out targetServerID, out array, out array2);
					if (returnValue.Type != 7 || returnValue.Value < 0)
					{
						cmdData = returnValue;
					}
					else
					{
						long nValue = (long)this.RuntimeData.SyncData.Month << 32;
						Global.SaveRoleParamsInt64ValueToDB(client, "10225", nValue, true);
						KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.GameType = 35;
						clientKuaFuServerLoginData.GameId = (long)num;
						clientKuaFuServerLoginData.EndTicks = TimeUtil.UTCTicks();
						clientKuaFuServerLoginData.TargetServerID = targetServerID;
						clientKuaFuServerLoginData.ServerIp = array[0];
						clientKuaFuServerLoginData.ServerPort = array2[0];
						clientKuaFuServerLoginData.Param1 = client.ClientData.ZhanDuiID;
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
				}
				IL_1F3:
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 113, false);
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == 0;
		}

		private bool GetBirthPoint(int mapCode, int side, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int x = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].X;
				int y = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].Y;
				int radius = this.RuntimeData.Config.BirthPointList[side % this.RuntimeData.Config.BirthPointList.Count].Radius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, x, y, radius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		private bool OnKuaFuLogin(KuaFuServerLoginData data)
		{
			ZhanDuiZhengBaFuBenData zhanDuiZhengBaFuBenData = null;
			int param = data.Param1;
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.KuaFuCopyDataDict.TryGetValue(data.GameId, out zhanDuiZhengBaFuBenData);
			}
			if (null == zhanDuiZhengBaFuBenData)
			{
				ReturnValue<int> returnValue = TcpCall.ZhanDuiZhengBa_K.ZhengBaKuaFuLogin(param, (int)data.GameId, data.ServerId, out zhanDuiZhengBaFuBenData);
				if (!returnValue.IsReturn || returnValue.Value < 0)
				{
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KuaFuCopyDataDict.ContainsKey(data.GameId))
					{
						this.RuntimeData.KuaFuCopyDataDict[data.GameId] = zhanDuiZhengBaFuBenData;
					}
				}
			}
			bool result;
			if (zhanDuiZhengBaFuBenData != null && GameManager.ServerId == zhanDuiZhengBaFuBenData.ServerID && zhanDuiZhengBaFuBenData.SideDict.ContainsKey((long)param))
			{
				data.ips = zhanDuiZhengBaFuBenData.IPs;
				data.ports = zhanDuiZhengBaFuBenData.Ports;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool OnKuaFuInitGame(GameClient client)
		{
			int zhanDuiID = client.ClientData.ZhanDuiID;
			int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
			bool result;
			if (num <= 0 || zhanDuiID <= 0)
			{
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig = this.RuntimeData.Config.MatchConfigList.First<ZhanDuiZhengBaMatchConfig>();
					ZhanDuiZhengBaFuBenData zhanDuiZhengBaFuBenData = null;
					if (!this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)num, out zhanDuiZhengBaFuBenData))
					{
						LogManager.WriteLog(2, string.Format("未找到活动KuaFuCopyData数据,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, num, zhanDuiZhengBaMatchConfig.MapCode), null, true);
						result = false;
					}
					else
					{
						if (zhanDuiZhengBaFuBenData.FuBenSeqID == 0)
						{
							zhanDuiZhengBaFuBenData.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						ZhanDuiZhengBaScene zhanDuiZhengBaScene = null;
						if (this.ZhanDuiZhengBaSceneDict.TryGetValue(zhanDuiZhengBaFuBenData.FuBenSeqID, out zhanDuiZhengBaScene) && zhanDuiZhengBaScene.m_eStatus >= 2)
						{
							LogManager.WriteLog(2, string.Format("当前场次战队争霸已经过准备时间,拒绝进入,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, num, zhanDuiZhengBaMatchConfig.MapCode), null, true);
							client.ClientData.PushMessageID = GLang.GetLang(8012, new object[0]);
							result = false;
						}
						else
						{
							int posX = 0;
							int posY = 0;
							int num2 = 0;
							if (!zhanDuiZhengBaFuBenData.SideDict.TryGetValue((long)client.ClientData.ZhanDuiID, out num2))
							{
								LogManager.WriteLog(2, string.Format("未找到活动阵营数据KuaFuCopyData,roleid={0},gameid={1},mapcode={2]", client.ClientData.RoleID, num, zhanDuiZhengBaMatchConfig.MapCode), null, true);
								result = false;
							}
							else if (!this.GetBirthPoint(zhanDuiZhengBaMatchConfig.MapCode, num2, out posX, out posY))
							{
								LogManager.WriteLog(2, string.Format("roleid={0},mapcode={1},side={2} 未找到出生点", client.ClientData.RoleID, zhanDuiZhengBaMatchConfig.MapCode, num2), null, true);
								result = false;
							}
							else
							{
								Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = zhanDuiZhengBaFuBenData.FuBenSeqID;
								client.ClientData.MapCode = zhanDuiZhengBaMatchConfig.MapCode;
								client.ClientData.PosX = posX;
								client.ClientData.PosY = posY;
								client.ClientData.FuBenSeqID = zhanDuiZhengBaFuBenData.FuBenSeqID;
								client.ClientData.BattleWhichSide = num2;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		public void TimerProc(object sender, EventArgs e)
		{
			long num = TimeUtil.NOW();
			bool flag = false;
			while (this.RuntimeData.SyncDataByTime.RunByInterval(num))
			{
				ZhanDuiZhengBaSyncData syncDataRequest = this.RuntimeData.SyncDataRequest;
				ReturnValue<ZhanDuiZhengBaSyncData> returnValue = TcpCall.ZhanDuiZhengBa_K.SyncZhengBaData(syncDataRequest);
				if (!returnValue.IsReturn)
				{
					break;
				}
				ZhanDuiZhengBaSyncData value = returnValue.Value;
				if (value == null)
				{
					break;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (syncDataRequest != this.RuntimeData.SyncDataRequest)
					{
						break;
					}
					this.RuntimeData.SyncData.Month = value.Month;
					this.RuntimeData.SyncData.RealActID = value.RealActID;
					this.RuntimeData.SyncData.IsThisMonthInActivity = value.IsThisMonthInActivity;
					this.RuntimeData.SyncData.CenterTime = value.CenterTime;
					this.RuntimeData.SyncData.TopZhanDui = value.TopZhanDui;
					if (!this.RuntimeData.SyncDataRequest.HasSeasonEnd && value.HasSeasonEnd)
					{
						flag = true;
					}
					this.RuntimeData.SyncData.HasSeasonEnd = value.HasSeasonEnd;
					this.RuntimeData.SyncDataRequest.HasSeasonEnd = value.HasSeasonEnd;
					if (value.RoleModTime != syncDataRequest.RoleModTime)
					{
						this.RuntimeData.SyncData.RoleModTime = value.RoleModTime;
						this.RuntimeData.SyncDataRequest.RoleModTime = value.RoleModTime;
						this.RuntimeData.SyncData.ZhanDuiList = value.ZhanDuiList;
					}
					if (value.PKLogModTime != syncDataRequest.PKLogModTime)
					{
						this.RuntimeData.SyncData.PKLogModTime = value.PKLogModTime;
						this.RuntimeData.SyncDataRequest.PKLogModTime = value.PKLogModTime;
						this.RuntimeData.SyncData.PKLogList = value.PKLogList;
					}
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < this.RuntimeData.PKResultQueue.Count; i++)
				{
					Tuple<int, int, int> tuple = this.RuntimeData.PKResultQueue.Peek();
					if (TcpCall.ZhanDuiZhengBa_K.ZhengBaPkResult(tuple.Item1, tuple.Item2).Type != 7)
					{
						break;
					}
					this.RuntimeData.PKResultQueue.Dequeue();
				}
			}
			if (flag)
			{
				this.GiveSupportAwards();
				foreach (GameClient client in GameManager.ClientMgr.GetAllClients(false))
				{
					this.UpdateChengHaoBuffer(client);
				}
			}
		}

		private void GiveSupportAwards()
		{
			foreach (GameClient client in GameManager.ClientMgr.GetAllClients(false))
			{
				this.GiveSupportAwards(client);
				this.GiveRankAwards(client);
			}
		}

		private void GiveSupportAwards(GameClient client)
		{
			try
			{
				if (this.RuntimeData.SyncData.HasSeasonEnd)
				{
					long num = Global.GetRoleParamsInt64FromDB(client, "10221");
					int num2 = (int)(num >> 32);
					int num3 = (int)(num & (long)((ulong)-1));
					int[] array = new int[2];
					if (num3 == 0 && num2 == this.RuntimeData.SyncData.Month)
					{
						long roleParamsInt64FromDB = Global.GetRoleParamsInt64FromDB(client, "10222");
						array[0] = (int)(roleParamsInt64FromDB >> 32);
						array[1] = (int)(roleParamsInt64FromDB & (long)((ulong)-1));
						int num4 = 0;
						lock (this.RuntimeData.Mutex)
						{
							List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != zhanDuiList)
							{
								using (List<ZhanDuiZhengBaZhanDuiData>.Enumerator enumerator = zhanDuiList.GetEnumerator())
								{
									while (enumerator.MoveNext())
									{
										ZhanDuiZhengBaZhanDuiData data = enumerator.Current;
										if (data.ZhanDuiID == array[0] || data.ZhanDuiID == array[1])
										{
											ZhanDuiZhengBaAwardsConfig zhanDuiZhengBaAwardsConfig = this.RuntimeData.AwardsConfig.Find((ZhanDuiZhengBaAwardsConfig x) => x.Rank == data.Grade);
											if (zhanDuiZhengBaAwardsConfig != null)
											{
												num4 += zhanDuiZhengBaAwardsConfig.TeamPoint;
											}
										}
									}
								}
							}
						}
						num3 = 1;
						num = ((long)num2 << 32) + (long)num3;
						Global.SaveRoleParamsInt64ValueToDB(client, "10221", num, true);
						GameManager.ClientMgr.ModifyTeamPointValue(client, num4, "战队争霸押注奖励", false);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		private void GiveRankAwards(GameClient client)
		{
			try
			{
				if (this.RuntimeData.SyncData.HasSeasonEnd)
				{
					ZhanDuiZhengBaAwardsConfig zhanDuiZhengBaAwardsConfig = null;
					long num = Global.GetRoleParamsInt64FromDB(client, "10225");
					int num2 = (int)(num >> 32);
					int num3 = (int)(num & (long)((ulong)-1));
					int[] array = new int[2];
					int grade = 0;
					if (num3 == 0 && num2 == this.RuntimeData.SyncData.Month)
					{
						lock (this.RuntimeData.Mutex)
						{
							List<ZhanDuiZhengBaZhanDuiData> zhanDuiList = this.RuntimeData.SyncData.ZhanDuiList;
							if (null != zhanDuiList)
							{
								foreach (ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData in zhanDuiList)
								{
									if (zhanDuiZhengBaZhanDuiData.ZhanDuiID == client.ClientData.ZhanDuiID)
									{
										grade = zhanDuiZhengBaZhanDuiData.Grade;
										break;
									}
								}
							}
							if (grade > 0)
							{
								zhanDuiZhengBaAwardsConfig = this.RuntimeData.AwardsConfig.Find((ZhanDuiZhengBaAwardsConfig x) => x.Rank == grade);
							}
						}
						if (zhanDuiZhengBaAwardsConfig != null && Global.CanAddGoodsNum(client, zhanDuiZhengBaAwardsConfig.Award.Items.Count))
						{
							foreach (AwardsItemData awardsItemData in zhanDuiZhengBaAwardsConfig.Award.Items)
							{
								Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "战盟联赛排行榜奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
							}
						}
						else
						{
							Global.UseMailGivePlayerAward2(client, zhanDuiZhengBaAwardsConfig.Award.Items, GLang.GetLang(8011, new object[0]), GLang.GetLang(8011, new object[0]), 0, 0, 0);
						}
						num3 = 1;
						num = ((long)num2 << 32) + (long)num3;
						Global.SaveRoleParamsInt64ValueToDB(client, "10225", num, true);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void UpdateChengHaoBuffer(GameClient client)
		{
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			int num = (int)(this.RuntimeData.StartTime - timeOfWeekNow).TotalSeconds % 604800;
			if (num < 0)
			{
				num += 604800;
			}
			int bufferIDBySpecialTitleID = FashionManager.getInstance().GetBufferIDBySpecialTitleID(this.RuntimeData.TeamBattleName[0]);
			if (this.RuntimeData.SyncData.TopZhanDui > 0 && client.ClientData.ZhanDuiID == this.RuntimeData.SyncData.TopZhanDui && num > 3)
			{
				Global.UpdateBufferDataTitle(client, bufferIDBySpecialTitleID, 1L, TimeUtil.NOW(), num);
			}
			else
			{
				Global.UpdateBufferDataTitle(client, bufferIDBySpecialTitleID, 0L, 0L, 0);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 56)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					long num = TimeUtil.NOW();
					ZhanDuiZhengBaScene zhanDuiZhengBaScene = null;
					if (!this.ZhanDuiZhengBaSceneDict.TryGetValue(fuBenSeqID, out zhanDuiZhengBaScene))
					{
						zhanDuiZhengBaScene = new ZhanDuiZhengBaScene();
						zhanDuiZhengBaScene.CopyMap = copyMap;
						zhanDuiZhengBaScene.CleanAllInfo();
						zhanDuiZhengBaScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						zhanDuiZhengBaScene.m_nMapCode = mapCode;
						zhanDuiZhengBaScene.CopyMapId = copyMap.CopyMapID;
						zhanDuiZhengBaScene.FuBenSeqId = fuBenSeqID;
						ZhanDuiZhengBaFuBenData fuBenData;
						if (this.RuntimeData.KuaFuCopyDataDict.TryGetValue((long)zhanDuiZhengBaScene.GameId, out fuBenData))
						{
							zhanDuiZhengBaScene.FuBenData = fuBenData;
						}
						zhanDuiZhengBaScene.SceneConfig = this.RuntimeData.Config.MatchConfigList.Find((ZhanDuiZhengBaMatchConfig x) => x.ID == fuBenData.ConfigID);
						this.ZhanDuiZhengBaSceneDict[fuBenSeqID] = zhanDuiZhengBaScene;
						zhanDuiZhengBaScene.m_lPrepareTime = num;
						zhanDuiZhengBaScene.m_lBeginTime = num + (long)(zhanDuiZhengBaScene.SceneConfig.WaitSeconds * 1000);
						zhanDuiZhengBaScene.m_eStatus = 1;
						zhanDuiZhengBaScene.StateTimeData.GameType = 35;
						zhanDuiZhengBaScene.StateTimeData.State = zhanDuiZhengBaScene.m_eStatus;
						zhanDuiZhengBaScene.StateTimeData.EndTicks = zhanDuiZhengBaScene.m_lBeginTime;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					zhanDuiZhengBaScene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.ZhanDuiID, true);
					zhanDuiZhengBaScene.ClientDict[client.ClientData.RoleID] = client;
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 56)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhanDuiZhengBaScene zhanDuiZhengBaScene;
					if (this.ZhanDuiZhengBaSceneDict.TryRemove(copyMap.FuBenSeqID, out zhanDuiZhengBaScene))
					{
					}
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= ZhanDuiZhengBaManager.NextHeartBeatTicks)
			{
				ZhanDuiZhengBaManager.NextHeartBeatTicks = num + 1020L;
				foreach (ZhanDuiZhengBaScene zhanDuiZhengBaScene in this.ZhanDuiZhengBaSceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = zhanDuiZhengBaScene.FuBenSeqId;
						int copyMapId = zhanDuiZhengBaScene.CopyMapId;
						int nMapCode = zhanDuiZhengBaScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = zhanDuiZhengBaScene.CopyMap;
							DateTime now = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							zhanDuiZhengBaScene.ScoreInfoData.Count1 = 0L;
							zhanDuiZhengBaScene.ScoreInfoData.Count2 = 0;
							List<KeyValuePair<int, Tuple<int, bool>>> list = new List<KeyValuePair<int, Tuple<int, bool>>>();
							foreach (KeyValuePair<int, Tuple<int, bool>> keyValuePair in zhanDuiZhengBaScene.RoleSideStateDict.ToList<KeyValuePair<int, Tuple<int, bool>>>())
							{
								if (keyValuePair.Value.Item2)
								{
									int key = keyValuePair.Key;
									Tuple<int, bool> value = keyValuePair.Value;
									GameClient gameClient = GameManager.ClientMgr.FindClient(key);
									if (null == gameClient)
									{
										if (value.Item2)
										{
											list.Add(new KeyValuePair<int, Tuple<int, bool>>(key, new Tuple<int, bool>(value.Item1, false)));
										}
									}
									else if (!gameClient.ClientData.FirstPlayStart)
									{
										int num3 = 0;
										if (zhanDuiZhengBaScene.FuBenData.SideDict.TryGetValue((long)value.Item1, out num3))
										{
											if (num3 == 1)
											{
												zhanDuiZhengBaScene.ScoreInfoData.Count1 += 1L;
											}
											else if (num3 == 2)
											{
												zhanDuiZhengBaScene.ScoreInfoData.Count2++;
											}
										}
									}
								}
							}
							foreach (KeyValuePair<int, Tuple<int, bool>> keyValuePair in list)
							{
								zhanDuiZhengBaScene.RoleSideStateDict[keyValuePair.Key] = keyValuePair.Value;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhanDuiZhengBaScoreInfoData>(1278, zhanDuiZhengBaScene.ScoreInfoData, copyMap);
							if (zhanDuiZhengBaScene.m_eStatus == 1)
							{
								bool flag2 = false;
								if (num2 >= zhanDuiZhengBaScene.m_lBeginTime)
								{
									flag2 = true;
								}
								else
								{
									bool flag3;
									if (zhanDuiZhengBaScene.RoleSideStateDict.Count >= zhanDuiZhengBaScene.FuBenData.RoleDict.Count)
									{
										flag3 = !copyMap.GetClientsList().All((GameClient x) => !x.ClientData.FirstPlayStart);
									}
									else
									{
										flag3 = true;
									}
									if (!flag3)
									{
										flag2 = true;
									}
								}
								if (flag2)
								{
									zhanDuiZhengBaScene.m_lBeginTime = num2;
									zhanDuiZhengBaScene.m_eStatus = 2;
									zhanDuiZhengBaScene.m_lEndTime = num2 + (long)(zhanDuiZhengBaScene.SceneConfig.FightSeconds * 1000);
									zhanDuiZhengBaScene.StateTimeData.GameType = 35;
									zhanDuiZhengBaScene.StateTimeData.State = zhanDuiZhengBaScene.m_eStatus;
									zhanDuiZhengBaScene.StateTimeData.EndTicks = zhanDuiZhengBaScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhanDuiZhengBaScene.StateTimeData, zhanDuiZhengBaScene.CopyMap);
									copyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 1, 0);
									copyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(copyMap.MapCode, copyMap.CopyMapID, 2, 0);
								}
							}
							else if (zhanDuiZhengBaScene.m_eStatus == 2)
							{
								if (num2 >= zhanDuiZhengBaScene.m_lEndTime)
								{
									this.CompleteZhanDuiZhengBaScene(zhanDuiZhengBaScene, zhanDuiZhengBaScene.FuBenData.BetterZhanDuiID);
								}
								else if (num2 - zhanDuiZhengBaScene.m_lBeginTime > 1000L)
								{
									this.SceneCheckComplete(zhanDuiZhengBaScene, true);
								}
							}
							else if (zhanDuiZhengBaScene.m_eStatus == 3)
							{
								this.ProcessEnd(zhanDuiZhengBaScene, now, num);
							}
							else if (zhanDuiZhengBaScene.m_eStatus == 4)
							{
								if (num2 >= zhanDuiZhengBaScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(zhanDuiZhengBaScene.m_lLeaveTime);
									zhanDuiZhengBaScene.m_eStatus = 5;
									KuaFuManager.getInstance().ClearCopyMapClients(copyMap);
								}
							}
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhanDuiZhengBaScene zhanDuiZhengBaScene;
				if (this.ZhanDuiZhengBaSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zhanDuiZhengBaScene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, zhanDuiZhengBaScene.StateTimeData, false);
				}
			}
		}

		public void CompleteZhanDuiZhengBaScene(ZhanDuiZhengBaScene scene, int successSide)
		{
			scene.m_eStatus = 3;
			scene.SuccessSide = successSide;
		}

		private int SceneCheckComplete(ZhanDuiZhengBaScene scene, bool complete = true)
		{
			int num = 0;
			if (scene.RoleSideStateDict.Count > 0)
			{
				foreach (Tuple<int, bool> tuple in scene.RoleSideStateDict.Values)
				{
					if (tuple.Item2)
					{
						if (num == 0)
						{
							num = tuple.Item1;
						}
						else if (num != tuple.Item1)
						{
							num = 0;
							break;
						}
					}
				}
			}
			else
			{
				num = scene.LastLeaveZhanDuiID;
			}
			if (num != 0 && complete)
			{
				this.CompleteZhanDuiZhengBaScene(scene, num);
			}
			return num;
		}

		private void SceneRemoveRole(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhanDuiZhengBaScene zhanDuiZhengBaScene;
				if (this.ZhanDuiZhengBaSceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zhanDuiZhengBaScene))
				{
					if (zhanDuiZhengBaScene.m_eStatus < 3)
					{
						zhanDuiZhengBaScene.RoleSideStateDict[client.ClientData.RoleID] = new Tuple<int, bool>(client.ClientData.ZhanDuiID, false);
						if (zhanDuiZhengBaScene.RoleSideStateDict.Count((KeyValuePair<int, Tuple<int, bool>> x) => x.Value.Item2) == 0)
						{
							zhanDuiZhengBaScene.LastLeaveZhanDuiID = client.ClientData.ZhanDuiID;
						}
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			if (client.SceneType == 56)
			{
				this.SceneRemoveRole(other);
				GameManager.ClientMgr.ChangePosition(TCPManager.getInstance().MySocketListener, TCPManager.getInstance().TcpOutPacketPool, other, this.RuntimeData.TeamBattleWatch[0], this.RuntimeData.TeamBattleWatch[1], 4, 159, 0);
			}
		}

		public void RoleLeaveFuBen(GameClient client)
		{
			if (client.SceneType == 56)
			{
				this.SceneRemoveRole(client);
			}
		}

		private void ProcessEnd(ZhanDuiZhengBaScene scene, DateTime now, long nowTicks)
		{
			scene.m_eStatus = 4;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(scene.SceneConfig.ClearSeconds * 1000);
			this.RuntimeData.PKResultQueue.Enqueue(new Tuple<int, int, int>(scene.GameId, scene.SuccessSide, 0));
			scene.StateTimeData.GameType = 35;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			this.GiveAwards(scene);
		}

		public void GiveAwards(ZhanDuiZhengBaScene scene)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				List<GameClient> list = scene.ClientDict.Values.ToList<GameClient>();
				HashSet<int> hashSet = new HashSet<int>();
				if (list != null && list.Count > 0)
				{
					int offsetDayNow = Global.GetOffsetDayNow();
					for (int i = 0; i < list.Count; i++)
					{
						GameClient gameClient = list[i];
						if (gameClient != null)
						{
							bool flag = false;
							GameClient gameClient2 = GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID);
							if (gameClient2 != null && gameClient2.SceneType == 56)
							{
								flag = true;
							}
							ZhanDuiZhengBaAwardsData zhanDuiZhengBaAwardsData = new ZhanDuiZhengBaAwardsData();
							if (gameClient.ClientData.ZhanDuiID == scene.SuccessSide)
							{
								zhanDuiZhengBaAwardsData.Success = 1;
								zhanDuiZhengBaAwardsData.NewGrade = scene.FuBenData.NewGrade;
							}
							else
							{
								zhanDuiZhengBaAwardsData.NewGrade = scene.FuBenData.JoinGrade;
							}
							if (flag)
							{
								gameClient2.sendCmd<ZhanDuiZhengBaAwardsData>(1279, zhanDuiZhengBaAwardsData, false);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "组队竞技清场调度异常");
			}
		}

		public bool OnPreZhanDuiChangeMember(PreZhanDuiChangeMemberEventObject e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long ticks = dateTime.TimeOfDay.Ticks;
			lock (this.RuntimeData.Mutex)
			{
				foreach (ZhanDuiZhengBaMatchConfig zhanDuiZhengBaMatchConfig in this.RuntimeData.Config.MatchConfigList)
				{
					if (dateTime.Day != zhanDuiZhengBaMatchConfig.TimePoints[0].Days)
					{
						return false;
					}
					if (ticks < zhanDuiZhengBaMatchConfig.DayBeginTick || ticks > zhanDuiZhengBaMatchConfig.DayEndTick)
					{
						return false;
					}
				}
				ZhanDuiZhengBaZhanDuiData zhanDuiZhengBaZhanDuiData = this.RuntimeData.SyncData.ZhanDuiList.Find((ZhanDuiZhengBaZhanDuiData x) => x.ZhanDuiID == e.ZhanDuiID);
				if (zhanDuiZhengBaZhanDuiData == null || zhanDuiZhengBaZhanDuiData.State == 2)
				{
					return false;
				}
				e.Result = false;
			}
			bool result;
			if (!e.Result)
			{
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(8001, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public const SceneUIClasses ManagerType = 56;

		public const GameTypes GameType = 35;

		private static ZhanDuiZhengBaManager instance = new ZhanDuiZhengBaManager();

		private EventSourceEx<KFCallMsg>.HandlerData NotifyEnterHandler = null;

		public ZhanDuiZhengBaData RuntimeData = new ZhanDuiZhengBaData();

		public ConcurrentDictionary<int, ZhanDuiZhengBaScene> ZhanDuiZhengBaSceneDict = new ConcurrentDictionary<int, ZhanDuiZhengBaScene>();

		public HashSet<int> CancledGameIdDict = new HashSet<int>();

		private static long NextHeartBeatTicks = 0L;
	}
}
