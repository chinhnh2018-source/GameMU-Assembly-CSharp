using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.Ornament;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Marriage.CoupleArena
{
	public class CoupleArenaManager : SingletonTemplate<CoupleArenaManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener
	{
		public void InitSystenParams()
		{
			try
			{
				this.ZhenAiBuffHoldWinSec = (int)GameManager.systemParamsList.GetParamValueIntByName("CoupleVictoryNeedTime", -1);
				this.YongQiBuff2ZhenAiBuffHurt = GameManager.systemParamsList.GetParamValueDoubleByName("CoupleBuffSpecificHurt", 0.0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				this.ZhenAiBuffHoldWinSec = 60;
				this.YongQiBuff2ZhenAiBuffHurt = 0.2;
			}
		}

		private bool LoadConfig()
		{
			bool result;
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath(CoupleAreanConsts.WarCfgFile));
				if (xelement.Elements().Count<XElement>() < 1)
				{
					throw new Exception(CoupleAreanConsts.WarCfgFile + " need at least 1 elements");
				}
				using (IEnumerator<XElement> enumerator = xelement.Elements().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						XElement xml = enumerator.Current;
						this.WarCfg.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						this.WarCfg.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						this.WarCfg.WaitSec = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						this.WarCfg.FightSec = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						this.WarCfg.ClearSec = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						this.WarCfg.TimePoints = new List<CoupleAreanWarCfg.TimePoint>();
						string[] array = Global.GetSafeAttributeStr(xml, "TimePoints").Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (int i = 0; i < array.Length; i += 3)
						{
							CoupleAreanWarCfg.TimePoint timePoint = new CoupleAreanWarCfg.TimePoint();
							timePoint.Weekday = Convert.ToInt32(array[i]);
							if (timePoint.Weekday < 1 || timePoint.Weekday > 7)
							{
								throw new Exception("weekday error!");
							}
							timePoint.DayStartTicks = DateTime.Parse(array[i + 1]).TimeOfDay.Ticks;
							timePoint.DayEndTicks = DateTime.Parse(array[i + 2]).TimeOfDay.Ticks;
							this.WarCfg.TimePoints.Add(timePoint);
						}
						this.WarCfg.TimePoints.Sort((CoupleAreanWarCfg.TimePoint _l, CoupleAreanWarCfg.TimePoint _r) => _l.Weekday - _r.Weekday);
					}
				}
				xelement = XElement.Load(Global.GameResPath(CoupleAreanConsts.DuanWeiCfgFile));
				foreach (XElement xml in xelement.Elements())
				{
					CoupleAreanDuanWeiCfg coupleAreanDuanWeiCfg = new CoupleAreanDuanWeiCfg();
					XElement xml;
					coupleAreanDuanWeiCfg.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					coupleAreanDuanWeiCfg.Type = (int)Global.GetSafeAttributeLong(xml, "Type");
					coupleAreanDuanWeiCfg.Level = (int)Global.GetSafeAttributeLong(xml, "Level");
					coupleAreanDuanWeiCfg.NeedJiFen = (int)Global.GetSafeAttributeLong(xml, "NeedCoupleDuanWeiJiFen");
					coupleAreanDuanWeiCfg.WinJiFen = (int)Global.GetSafeAttributeLong(xml, "WinJiFen");
					coupleAreanDuanWeiCfg.LoseJiFen = (int)Global.GetSafeAttributeLong(xml, "LoseJiFen");
					coupleAreanDuanWeiCfg.WeekGetRongYaoTimes = (int)Global.GetSafeAttributeLong(xml, "WeekRongYaoNum");
					coupleAreanDuanWeiCfg.WinRongYao = (int)Global.GetSafeAttributeLong(xml, "WinRongYu");
					coupleAreanDuanWeiCfg.LoseRongYao = (int)Global.GetSafeAttributeLong(xml, "LoseRongYu");
					this.DuanWeiCfgList.Add(coupleAreanDuanWeiCfg);
				}
				this.DuanWeiCfgList.Sort(delegate(CoupleAreanDuanWeiCfg _l, CoupleAreanDuanWeiCfg _r)
				{
					int result2;
					if (_l.Type < _r.Type)
					{
						result2 = -1;
					}
					else if (_l.Type > _r.Type)
					{
						result2 = 1;
					}
					else if (_l.Level > _r.Level)
					{
						result2 = -1;
					}
					else if (_l.Level < _r.Level)
					{
						result2 = 1;
					}
					else
					{
						result2 = 0;
					}
					return result2;
				});
				for (int i = 1; i < this.DuanWeiCfgList.Count; i++)
				{
					CoupleAreanDuanWeiCfg coupleAreanDuanWeiCfg2 = this.DuanWeiCfgList[i];
					CoupleAreanDuanWeiCfg coupleAreanDuanWeiCfg3 = this.DuanWeiCfgList[i - 1];
					if (coupleAreanDuanWeiCfg2.NeedJiFen <= coupleAreanDuanWeiCfg3.NeedJiFen)
					{
						throw new Exception(string.Format("段位积分配置有问题{0}", coupleAreanDuanWeiCfg2.Id));
					}
				}
				if (this.DuanWeiCfgList[0].NeedJiFen != 0)
				{
					throw new Exception(string.Format("段位积分配置有问题{0}", this.DuanWeiCfgList[0].Id));
				}
				xelement = XElement.Load(Global.GameResPath(CoupleAreanConsts.WeekRankAwardCfgFile));
				foreach (XElement xml in xelement.Elements())
				{
					CoupleAreanWeekRankAwardCfg coupleAreanWeekRankAwardCfg = new CoupleAreanWeekRankAwardCfg();
					XElement xml;
					coupleAreanWeekRankAwardCfg.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					coupleAreanWeekRankAwardCfg.Name = Global.GetSafeAttributeStr(xml, "Name");
					coupleAreanWeekRankAwardCfg.StartRank = (int)Global.GetSafeAttributeLong(xml, "StarRank");
					coupleAreanWeekRankAwardCfg.EndRank = (int)Global.GetSafeAttributeLong(xml, "EndRank");
					coupleAreanWeekRankAwardCfg.AwardGoods = GoodsHelper.ParseGoodsDataList(Global.GetSafeAttributeStr(xml, "Award").Split(new char[]
					{
						'|'
					}), CoupleAreanConsts.WeekRankAwardCfgFile);
					this.WeekAwardCfgList.Add(coupleAreanWeekRankAwardCfg);
				}
				xelement = XElement.Load(Global.GameResPath(CoupleAreanConsts.BuffCfgFile));
				foreach (XElement xml in xelement.Elements())
				{
					CoupleArenaBuffCfg coupleArenaBuffCfg = new CoupleArenaBuffCfg();
					XElement xml;
					coupleArenaBuffCfg.Type = (int)Global.GetSafeAttributeLong(xml, "TypeID");
					coupleArenaBuffCfg.Name = Global.GetSafeAttributeStr(xml, "Name");
					coupleArenaBuffCfg.MonsterId = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
					coupleArenaBuffCfg.RandPosList = new List<CoupleArenaBuffCfg.RandPos>();
					string[] array2 = Global.GetSafeAttributeStr(xml, "Site").Split(new char[]
					{
						'|',
						','
					});
					for (int i = 0; i < array2.Length - 2; i += 3)
					{
						CoupleArenaBuffCfg.RandPos randPos = new CoupleArenaBuffCfg.RandPos();
						randPos.X = Convert.ToInt32(array2[i]);
						randPos.Y = Convert.ToInt32(array2[i + 1]);
						randPos.R = Convert.ToInt32(array2[i + 2]);
						coupleArenaBuffCfg.RandPosList.Add(randPos);
					}
					coupleArenaBuffCfg.FlushSecList = new List<int>();
					string[] array3 = Global.GetSafeAttributeStr(xml, "Time").Split(new char[]
					{
						'|'
					});
					for (int i = 0; i < array3.Length; i++)
					{
						coupleArenaBuffCfg.FlushSecList.Add(Convert.ToInt32(array3[i]));
					}
					coupleArenaBuffCfg.ExtProps = new Dictionary<ExtPropIndexes, double>();
					string[] array4 = Global.GetSafeAttributeStr(xml, "Property").Split(new char[]
					{
						'|',
						','
					});
					for (int i = 0; i < array4.Length - 1; i += 2)
					{
						coupleArenaBuffCfg.ExtProps.Add((ExtPropIndexes)Enum.Parse(typeof(ExtPropIndexes), array4[i]), Convert.ToDouble(array4[i + 1]));
					}
					this.BuffCfgList.Add(coupleArenaBuffCfg);
				}
				xelement = XElement.Load(Global.GameResPath(CoupleAreanConsts.BirthPointCfgFile));
				foreach (XElement xml in xelement.Elements())
				{
					TianTiBirthPoint tianTiBirthPoint = new TianTiBirthPoint();
					XElement xml;
					tianTiBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					tianTiBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
					tianTiBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
					tianTiBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
					this.BirthPointList.Add(tianTiBirthPoint);
				}
				if (this.BirthPointList.Count != 2)
				{
					throw new Exception(CoupleAreanConsts.BirthPointCfgFile);
				}
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException("CoupleArenaManager loadconfig. " + ex.Message);
				result = false;
			}
			return result;
		}

		public bool initialize()
		{
			bool result;
			if (!this.LoadConfig())
			{
				result = false;
			}
			else
			{
				ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CoupleArenaManager.TimerProc", new EventHandler(this.TimerProc)), 20000, 10000);
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1370, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1372, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1371, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1373, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1374, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1377, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1375, 1, 1, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1380, 2, 2, SingletonTemplate<CoupleArenaManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10025, 38, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(10, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<CoupleArenaManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10025, 38, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(10, SingletonTemplate<CoupleArenaManager>.Instance());
			GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<CoupleArenaManager>.Instance());
			return true;
		}

		public bool destroy()
		{
			return true;
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
				case 1370:
					return this.HandleGetMainDataCommand(client, nID, bytes, cmdParams);
				case 1371:
					return this.HandleGetZhanBaoCommand(client, nID, bytes, cmdParams);
				case 1372:
					return this.HandleGetPaiHangCommand(client, nID, bytes, cmdParams);
				case 1373:
					return this.HandleSetReadyCommand(client, nID, bytes, cmdParams);
				case 1374:
					return this.HandleSingleJoinCommand(client, nID, bytes, cmdParams);
				case 1375:
					return this.HandleQuitCommand(client, nID, bytes, cmdParams);
				case 1377:
					return this.HandleEnterCommand(client, nID, bytes, cmdParams);
				case 1380:
					return this.HandleRegStateWatcherCommand(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 10025)
			{
				this.HandleCanEnterEvent((eventObject as CoupleArenaCanEnterEvent).Data);
			}
			eventObject.Handled = true;
		}

		private void HandleCanEnterEvent(CoupleArenaCanEnterData data)
		{
			string text;
			int num;
			string text2;
			int num2;
			string serverIp;
			int serverPort;
			if (!KuaFuManager.getInstance().GetKuaFuDbServerInfo(data.KfServerId, out text, out num, out text2, out num2, out serverIp, out serverPort))
			{
				LogManager.WriteLog(2, string.Format("夫妻竞技被分配到服务器ServerId={0}, 但是找不到该跨服活动服务器", data.KfServerId), null, true);
			}
			else
			{
				lock (this.Mutex)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(data.RoleId1);
					if (gameClient != null && this.GetMatchState(data.RoleId1) == ECoupleArenaMatchState.Ready)
					{
						gameClient.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId1;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.GameId = data.GameId;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.GameType = 13;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
						gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerIp = serverIp;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerPort = serverPort;
						gameClient.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
						gameClient.sendCmd(1376, "1", false);
						this.SetMatchState(data.RoleId1, ECoupleArenaMatchState.OnLine);
						this.NtfCoupleMatchState(gameClient.ClientData.RoleID);
						if (MarryLogic.IsMarried(gameClient.ClientData.RoleID))
						{
							this.NtfCoupleMatchState(gameClient.ClientData.MyMarriageData.nSpouseID);
						}
					}
					GameClient gameClient2 = GameManager.ClientMgr.FindClient(data.RoleId2);
					if (gameClient2 != null && this.GetMatchState(data.RoleId2) == ECoupleArenaMatchState.Ready)
					{
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.RoleId = data.RoleId2;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.GameId = data.GameId;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.GameType = 13;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerId = GameCoreInterface.getinstance().GetLocalServerId();
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerIp = serverIp;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.ServerPort = serverPort;
						gameClient2.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = 0;
						gameClient2.sendCmd(1376, "1", false);
						this.SetMatchState(data.RoleId2, ECoupleArenaMatchState.OnLine);
						this.NtfCoupleMatchState(gameClient2.ClientData.RoleID);
						if (MarryLogic.IsMarried(gameClient2.ClientData.RoleID))
						{
							this.NtfCoupleMatchState(gameClient2.ClientData.MyMarriageData.nSpouseID);
						}
					}
				}
			}
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				if (monsterDeadEventObject.getAttacker().ClientData.CopyMapID > 0 && monsterDeadEventObject.getAttacker().ClientData.FuBenSeqID > 0 && monsterDeadEventObject.getAttacker().ClientData.MapCode == this.WarCfg.MapCode && monsterDeadEventObject.getMonster().CurrentMapCode == this.WarCfg.MapCode)
				{
					lock (this.Mutex)
					{
						CoupleArenaCopyScene scene;
						if (this.FuBenSeq2CopyScenes.TryGetValue(monsterDeadEventObject.getAttacker().ClientData.FuBenSeqID, out scene))
						{
							this.OnMonsterDead(scene, monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
						}
					}
				}
			}
			else if (eventObject.getEventType() == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (playerDeadEventObject.getPlayer().ClientData.CopyMapID > 0 && playerDeadEventObject.getPlayer().ClientData.FuBenSeqID > 0 && playerDeadEventObject.getPlayer().ClientData.MapCode == this.WarCfg.MapCode)
				{
					lock (this.Mutex)
					{
						CoupleArenaCopyScene scene;
						if (this.FuBenSeq2CopyScenes.TryGetValue(playerDeadEventObject.getPlayer().ClientData.FuBenSeqID, out scene))
						{
							this.OnPlayerDead(scene, playerDeadEventObject.getPlayer(), playerDeadEventObject.getAttackerRole());
						}
					}
				}
			}
		}

		private bool HandleRegStateWatcherCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[0]);
			int num2 = Convert.ToInt32(cmdParams[1]);
			this.RegStateWatcher(client.ClientData.RoleID, num2 > 0);
			this.NtfCoupleMatchState(client.ClientData.RoleID);
			return true;
		}

		private bool HandleEnterCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[0]);
			int num2 = Convert.ToInt32(cmdParams[1]);
			if (num2 <= 0)
			{
				Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
				client.sendCmd(nID, 1.ToString(), false);
			}
			else
			{
				GlobalNew.RecordSwitchKuaFuServerLog(client);
				client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
			}
			return true;
		}

		private bool HandleQuitCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client) || !MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					if (this.GetMatchState(client.ClientData.RoleID) != ECoupleArenaMatchState.Ready)
					{
						client.sendCmd(nID, 1.ToString(), false);
						result = true;
					}
					else
					{
						TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
						this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
						if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
						{
							this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
							GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
							if (gameClient != null)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, GLang.GetLang(475, new object[0]), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
								gameClient.sendCmd(1375, 1.ToString(), false);
							}
						}
						this.NtfCoupleMatchState(client.ClientData.RoleID);
						this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						client.sendCmd(nID, 1.ToString(), false);
						result = true;
					}
				}
			}
			return result;
		}

		private bool HandleSingleJoinCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!client.ClientData.IsMainOccupation)
			{
				client.sendCmd(nID, -35.ToString(), false);
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					if (!this.IsGongNengOpened(client))
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else if (!this.IsInWeekOnceActTimes(TimeUtil.NowDateTime()))
					{
						client.sendCmd(nID, -2001.ToString(), false);
						result = true;
					}
					else if (MarryLogic.IsMarried(client.ClientData.RoleID) && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) != ECoupleArenaMatchState.Offline && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) != ECoupleArenaMatchState.NotOpen)
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else if (this.GetMatchState(client.ClientData.RoleID) != ECoupleArenaMatchState.OnLine)
					{
						client.sendCmd(nID, -12.ToString(), false);
						result = true;
					}
					else
					{
						int num = TianTiClient.getInstance().CoupleArenaJoin(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID, GameCoreInterface.getinstance().GetLocalServerId());
						if (num >= 0)
						{
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.Ready);
							this.NtfCoupleMatchState(client.ClientData.RoleID);
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
							GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 13);
						}
						client.sendCmd(nID, num.ToString(), false);
						result = true;
					}
				}
			}
			return result;
		}

		private bool HandleSetReadyCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[0]);
			bool flag = Convert.ToInt32(cmdParams[1]) > 0;
			bool result;
			if (!client.ClientData.IsMainOccupation)
			{
				client.sendCmd(nID, -35.ToString(), false);
				result = true;
			}
			else if (!this.IsGongNengOpened(client))
			{
				client.sendCmd(nID, -12.ToString(), false);
				result = true;
			}
			else if (!this.IsInWeekOnceActTimes(TimeUtil.NowDateTime()))
			{
				client.sendCmd(nID, -2001.ToString(), false);
				result = true;
			}
			else
			{
				lock (this.Mutex)
				{
					ECoupleArenaMatchState matchState = this.GetMatchState(client.ClientData.RoleID);
					ECoupleArenaMatchState ecoupleArenaMatchState = flag ? ECoupleArenaMatchState.Ready : ECoupleArenaMatchState.OnLine;
					if (matchState == ecoupleArenaMatchState)
					{
						result = true;
					}
					else
					{
						this.SetMatchState(client.ClientData.RoleID, ecoupleArenaMatchState);
						this.NtfCoupleMatchState(client.ClientData.RoleID);
						this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						if (matchState != ECoupleArenaMatchState.Ready && ecoupleArenaMatchState == ECoupleArenaMatchState.Ready && this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
						{
							CoupleArenaJoinData coupleArenaJoinData = new CoupleArenaJoinData();
							coupleArenaJoinData.RoleId1 = client.ClientData.RoleID;
							coupleArenaJoinData.RoleId2 = client.ClientData.MyMarriageData.nSpouseID;
							int num2 = TianTiClient.getInstance().CoupleArenaJoin(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID, GameCoreInterface.getinstance().GetLocalServerId());
							if (num2 >= 0)
							{
								client.sendCmd(1374, num2.ToString(), false);
								GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (gameClient != null)
								{
									gameClient.sendCmd(1374, num2.ToString(), false);
								}
							}
						}
						if (ecoupleArenaMatchState == ECoupleArenaMatchState.Ready)
						{
							GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 13);
						}
						result = true;
					}
				}
			}
			return result;
		}

		private bool HandleGetZhanBaoCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			List<CoupleArenaZhanBaoItemData> cmdData = null;
			if (this.IsGongNengOpened(client))
			{
				cmdData = Global.sendToDB<List<CoupleArenaZhanBaoItemData>, string>(nID, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID), client.ServerId);
			}
			client.sendCmd<List<CoupleArenaZhanBaoItemData>>(nID, cmdData, false);
			return true;
		}

		private bool HandleGetPaiHangCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			CoupleArenaPaiHangData coupleArenaPaiHangData = new CoupleArenaPaiHangData();
			lock (this.Mutex)
			{
				coupleArenaPaiHangData.PaiHang = this.SyncRankList.GetRange(0, Math.Min(10, this.SyncRankList.Count));
			}
			client.sendCmd<CoupleArenaPaiHangData>(nID, coupleArenaPaiHangData, false);
			return true;
		}

		private bool HandleGetMainDataCommand(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			DateTime time = TimeUtil.NowDateTime();
			CoupleArenaMainData coupleArenaMainData = new CoupleArenaMainData();
			coupleArenaMainData.JingJiData = null;
			coupleArenaMainData.WeekGetRongYaoTimes = 0;
			coupleArenaMainData.CanGetAwardId = 0;
			CoupleArenaCoupleJingJiData coupleArenaCoupleJingJiData = new CoupleArenaCoupleJingJiData();
			coupleArenaCoupleJingJiData.ManRoleId = client.ClientData.RoleID;
			coupleArenaCoupleJingJiData.ManZoneId = client.ClientData.ZoneID;
			coupleArenaCoupleJingJiData.ManSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.RoleID, client.ServerId);
			if (MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				coupleArenaCoupleJingJiData.WifeSelector = Global.sendToDB<RoleData4Selector, int>(10232, client.ClientData.MyMarriageData.nSpouseID, client.ServerId);
				if (coupleArenaCoupleJingJiData.WifeSelector != null)
				{
					coupleArenaCoupleJingJiData.WifeRoleId = coupleArenaCoupleJingJiData.WifeSelector.RoleID;
					coupleArenaCoupleJingJiData.WifeZoneId = coupleArenaCoupleJingJiData.WifeSelector.ZoneId;
				}
			}
			if ((!MarryLogic.IsMarried(client.ClientData.RoleID) && client.ClientData.RoleSex == 1) || client.ClientData.MyMarriageData.byMarrytype == 2)
			{
				int manRoleId = coupleArenaCoupleJingJiData.ManRoleId;
				int manZoneId = coupleArenaCoupleJingJiData.ManZoneId;
				RoleData4Selector manSelector = coupleArenaCoupleJingJiData.ManSelector;
				coupleArenaCoupleJingJiData.ManRoleId = coupleArenaCoupleJingJiData.WifeRoleId;
				coupleArenaCoupleJingJiData.ManZoneId = coupleArenaCoupleJingJiData.WifeZoneId;
				coupleArenaCoupleJingJiData.ManSelector = coupleArenaCoupleJingJiData.WifeSelector;
				coupleArenaCoupleJingJiData.WifeRoleId = manRoleId;
				coupleArenaCoupleJingJiData.WifeZoneId = manZoneId;
				coupleArenaCoupleJingJiData.WifeSelector = manSelector;
			}
			coupleArenaCoupleJingJiData.DuanWeiType = this.DuanWeiCfgList[0].Type;
			coupleArenaCoupleJingJiData.DuanWeiLevel = this.DuanWeiCfgList[0].Level;
			coupleArenaCoupleJingJiData.JiFen = 0;
			if (MarryLogic.IsMarried(client.ClientData.RoleID))
			{
				CoupleArenaCoupleJingJiData cachedCoupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
				if (cachedCoupleData != null)
				{
					coupleArenaCoupleJingJiData.TotalFightTimes = cachedCoupleData.TotalFightTimes;
					coupleArenaCoupleJingJiData.WinFightTimes = cachedCoupleData.WinFightTimes;
					coupleArenaCoupleJingJiData.LianShengTimes = cachedCoupleData.LianShengTimes;
					coupleArenaCoupleJingJiData.DuanWeiType = cachedCoupleData.DuanWeiType;
					coupleArenaCoupleJingJiData.DuanWeiLevel = cachedCoupleData.DuanWeiLevel;
					coupleArenaCoupleJingJiData.JiFen = cachedCoupleData.JiFen;
					coupleArenaCoupleJingJiData.Rank = cachedCoupleData.Rank;
				}
				GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_CoupleArenaDuanWei, new int[0]));
			}
			coupleArenaMainData.JingJiData = coupleArenaCoupleJingJiData;
			string roleParamByName = Global.GetRoleParamByName(client, "27");
			if (!string.IsNullOrEmpty(roleParamByName))
			{
				string[] array = roleParamByName.Split(new char[]
				{
					','
				});
				if (array != null && array.Length == 2 && Convert.ToInt32(array[0]) == this.CurrRankWeek(time))
				{
					coupleArenaMainData.WeekGetRongYaoTimes = Convert.ToInt32(array[1]);
				}
			}
			int num;
			if (this.IsInWeekAwardTime(time, out num))
			{
				CoupleArenaCoupleJingJiData cachedCoupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
				if (cachedCoupleData != null)
				{
					string roleParamByName2 = Global.GetRoleParamByName(client, "28");
					string[] array = (roleParamByName2 == null) ? null : roleParamByName2.Split(new char[]
					{
						','
					});
					if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != num)
					{
						foreach (CoupleAreanWeekRankAwardCfg coupleAreanWeekRankAwardCfg in this.WeekAwardCfgList)
						{
							if (cachedCoupleData.Rank >= coupleAreanWeekRankAwardCfg.StartRank && (coupleAreanWeekRankAwardCfg.EndRank == -1 || cachedCoupleData.Rank <= coupleAreanWeekRankAwardCfg.EndRank))
							{
								Global.SaveRoleParamsStringToDB(client, "28", string.Format("{0},{1}", num, coupleAreanWeekRankAwardCfg.Id), true);
								coupleArenaMainData.CanGetAwardId = coupleAreanWeekRankAwardCfg.Id;
								if (Global.CanAddGoodsDataList(client, coupleAreanWeekRankAwardCfg.AwardGoods))
								{
									foreach (GoodsData goodsData in coupleAreanWeekRankAwardCfg.AwardGoods)
									{
										Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "情侣竞技场", false, goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, goodsData.ExcellenceInfo, goodsData.AppendPropLev, goodsData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
									}
								}
								else
								{
									Global.UseMailGivePlayerAward3(client.ClientData.RoleID, coupleAreanWeekRankAwardCfg.AwardGoods, GLang.GetLang(476, new object[0]), string.Format(GLang.GetLang(477, new object[0]), cachedCoupleData.Rank), 0, 0, 0);
								}
								break;
							}
						}
					}
					this.CheckTipsIconState(client);
				}
			}
			client.sendCmd<CoupleArenaMainData>(nID, coupleArenaMainData, false);
			return true;
		}

		public void OnClientLogin(GameClient client)
		{
			if (client != null)
			{
				this.CheckFengHuoJiaRenChengHao(client);
				if (!client.ClientSocket.IsKuaFuLogin)
				{
					lock (this.Mutex)
					{
						this.RegStateWatcher(client.ClientData.RoleID, false);
						this.SetMatchState(client.ClientData.RoleID, (this.IsGongNengOpened(client) && MarryLogic.IsMarried(client.ClientData.RoleID)) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
						if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.OnLine)
						{
							if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
							{
								TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
								this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
								GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
								if (gameClient != null)
								{
									if (this.RoleStartReadyMs.ContainsKey(gameClient.ClientData.RoleID) && this.RoleStartReadyMs[gameClient.ClientData.RoleID] + 60000L > TimeUtil.NOW())
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, "情侣双方均在线，无法单人匹配", GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
									}
									gameClient.sendCmd(1375, 1.ToString(), false);
								}
							}
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						}
						this.CheckTipsIconState(client);
						GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_CoupleArenaDuanWei, new int[0]));
					}
				}
			}
		}

		public void OnClientLogout(GameClient client)
		{
			try
			{
				if (client != null)
				{
					if (!client.ClientSocket.IsKuaFuLogin)
					{
						lock (this.Mutex)
						{
							this.RegStateWatcher(client.ClientData.RoleID, false);
							if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.Ready || this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.OnLine)
							{
								TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
								if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
								{
									this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
									GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
									if (gameClient != null)
									{
										gameClient.sendCmd(1375, 1.ToString(), false);
									}
								}
							}
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.Offline);
							this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		public void CheckGongNengOpen(GameClient client)
		{
			lock (this.Mutex)
			{
				if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.NotOpen && this.IsGongNengOpened(client))
				{
					this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
					this.NtfCoupleMatchState(client.ClientData.RoleID);
					if (this.GetMatchState(client.ClientData.MyMarriageData.nSpouseID) == ECoupleArenaMatchState.Ready)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(client.ClientData.MyMarriageData.nSpouseID);
						if (gameClient != null)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(478, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, client.ClientData.MyMarriageData.nSpouseID);
						this.SetMatchState(client.ClientData.MyMarriageData.nSpouseID, ECoupleArenaMatchState.OnLine);
					}
					this.NtfCoupleMatchState(client.ClientData.MyMarriageData.nSpouseID);
				}
			}
		}

		public void OnMarry(GameClient client1, GameClient client2)
		{
			lock (this.Mutex)
			{
				this.SetMatchState(client1.ClientData.RoleID, this.IsGongNengOpened(client1) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
				this.SetMatchState(client2.ClientData.RoleID, this.IsGongNengOpened(client2) ? ECoupleArenaMatchState.OnLine : ECoupleArenaMatchState.NotOpen);
				this.NtfCoupleMatchState(client1.ClientData.RoleID);
				this.NtfCoupleMatchState(client2.ClientData.RoleID);
			}
		}

		public void OnDivorce(int roleId1, int roleId2)
		{
			lock (this.Mutex)
			{
				if (this.GetMatchState(roleId1) == ECoupleArenaMatchState.Ready || this.GetMatchState(roleId2) == ECoupleArenaMatchState.Ready)
				{
					TianTiClient.getInstance().CoupleArenaQuit(roleId1, roleId2);
				}
				ECoupleArenaMatchState matchState = this.GetMatchState(roleId1);
				ECoupleArenaMatchState matchState2 = this.GetMatchState(roleId2);
				this.SetMatchState(roleId1, (matchState == ECoupleArenaMatchState.OnLine || matchState == ECoupleArenaMatchState.Ready) ? ECoupleArenaMatchState.NotOpen : matchState);
				this.SetMatchState(roleId2, (matchState2 == ECoupleArenaMatchState.OnLine || matchState2 == ECoupleArenaMatchState.Ready) ? ECoupleArenaMatchState.NotOpen : matchState2);
				this.NtfCoupleMatchState(roleId1);
				this.NtfCoupleMatchState(roleId2);
				if (matchState == ECoupleArenaMatchState.Ready)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(roleId1);
					if (gameClient != null)
					{
						gameClient.sendCmd(1375, 1.ToString(), false);
					}
				}
				if (matchState2 == ECoupleArenaMatchState.Ready)
				{
					GameClient gameClient2 = GameManager.ClientMgr.FindClient(roleId2);
					if (gameClient2 != null)
					{
						gameClient2.sendCmd(1375, 1.ToString(), false);
					}
				}
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleId1));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleId2));
				Global.sendToDB<bool, string>(1383, string.Format("{0}:{1}", roleId1, roleId2), 0);
			}
		}

		public bool PreClearDivorceData(int roleId1, int roleId2)
		{
			return TianTiClient.getInstance().CoupleArenaPreDivorce(roleId1, roleId2) >= 0;
		}

		public void OnSpouseRequestDivorce(GameClient client, GameClient spouseClient)
		{
			if (client != null)
			{
				if (spouseClient != null)
				{
					lock (this.Mutex)
					{
						if (this.GetMatchState(client.ClientData.RoleID) == ECoupleArenaMatchState.Ready)
						{
							TianTiClient.getInstance().CoupleArenaQuit(client.ClientData.RoleID, spouseClient.ClientData.RoleID);
							this.SetMatchState(client.ClientData.RoleID, ECoupleArenaMatchState.OnLine);
							this.NtfCoupleMatchState(client.ClientData.RoleID);
							client.sendCmd(1375, 1.ToString(), false);
						}
					}
				}
			}
		}

		public double CalcBuffHurt(IObject obj, IObject objTarget)
		{
			double result;
			try
			{
				if (obj == null || objTarget == null)
				{
					result = 0.0;
				}
				else
				{
					GameClient gameClient = obj as GameClient;
					GameClient gameClient2 = objTarget as GameClient;
					if (gameClient == null || gameClient2 == null)
					{
						result = 0.0;
					}
					else
					{
						BufferData bufferDataByID = Global.GetBufferDataByID(gameClient, 2080011);
						BufferData bufferDataByID2 = Global.GetBufferDataByID(gameClient2, 2080010);
						if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L) && bufferDataByID2 != null && !Global.IsBufferDataOver(bufferDataByID2, 0L))
						{
							result = this.YongQiBuff2ZhenAiBuffHurt;
						}
						else
						{
							result = 0.0;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = 0.0;
			}
			return result;
		}

		public void SetFengHuoJiaRenCouple(int roleid1, int roleid2)
		{
			int num = Math.Min(roleid1, roleid2);
			int num2 = Math.Max(roleid1, roleid2);
			string[] array = GameManager.GameConfigMgr.GetGameConfigItemStr("CoupleArenaFengHuo", "0,0").Split(new char[]
			{
				','
			});
			if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != num || Convert.ToInt32(array[1]) != num2)
			{
				int roleID = Convert.ToInt32(array[0]);
				int roleID2 = Convert.ToInt32(array[1]);
				Global.UpdateDBGameConfigg("CoupleArenaFengHuo", string.Format("{0},{1}", num, num2));
				GameManager.GameConfigMgr.SetGameConfigItem("CoupleArenaFengHuo", string.Format("{0},{1}", num, num2));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleID));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleID2));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleid1));
				this.CheckFengHuoJiaRenChengHao(GameManager.ClientMgr.FindClient(roleid2));
			}
		}

		public void CheckFengHuoJiaRenChengHao(GameClient client)
		{
			if (client != null)
			{
				lock (this.Mutex)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					string[] array = GameManager.GameConfigMgr.GetGameConfigItemStr("CoupleArenaFengHuo", "0,0").Split(new char[]
					{
						','
					});
					bool flag2 = false;
					int num = 0;
					while (array != null && num < array.Length && !flag2)
					{
						flag2 = (Convert.ToInt32(array[num]) == client.ClientData.RoleID);
						num++;
					}
					flag2 &= MarryLogic.IsMarried(client.ClientData.RoleID);
					flag2 &= this.IsInFengHuoJiaRenChengHaoTime(dateTime);
					if (flag2)
					{
						CoupleAreanWarCfg.TimePoint timePoint = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
						int weekDay1To = TimeUtil.GetWeekDay1To7(dateTime);
						DateTime dateTime2 = dateTime.AddTicks(-dateTime.TimeOfDay.Ticks);
						dateTime2 = dateTime2.AddDays((double)(-(double)TimeUtil.GetWeekDay1To7(dateTime2)));
						dateTime2 = dateTime2.AddDays((double)timePoint.Weekday);
						dateTime2 = dateTime2.AddTicks(timePoint.DayStartTicks);
						if (weekDay1To > timePoint.Weekday || (weekDay1To == timePoint.Weekday && dateTime.TimeOfDay.Ticks > timePoint.DayStartTicks))
						{
							dateTime2 = dateTime2.AddDays(7.0);
						}
						FashionManager.getInstance().GetFashionByMagic(client, FashionIdConsts.CoupleArenaFengHuoJiaRen, dateTime2.ToString("yyyy-MM-dd HH:mm:ss"));
					}
					else
					{
						FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.CoupleArenaFengHuoJiaRen);
					}
				}
			}
		}

		public void CheckTipsIconState(GameClient client)
		{
			if (client != null)
			{
				bool bIconState = false;
				lock (this.Mutex)
				{
					int num = 0;
					if (this.IsInWeekAwardTime(TimeUtil.NowDateTime(), out num))
					{
						CoupleArenaCoupleJingJiData cachedCoupleData = this.GetCachedCoupleData(client.ClientData.RoleID);
						if (cachedCoupleData != null)
						{
							string roleParamByName = Global.GetRoleParamByName(client, "28");
							string[] array = (roleParamByName == null) ? null : roleParamByName.Split(new char[]
							{
								','
							});
							if (array == null || array.Length != 2 || Convert.ToInt32(array[0]) != num)
							{
								bIconState = true;
							}
						}
					}
				}
				if (client._IconStateMgr.AddFlushIconState(15011, bIconState))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		private bool IsGongNengOpened(GameClient client)
		{
			return client != null && GlobalNew.IsGongNengOpened(client, 58, false) && MarryLogic.IsMarried(client.ClientData.RoleID) && GlobalNew.IsGongNengOpened(client, 80, false);
		}

		private bool IsInFengHuoJiaRenChengHaoTime(DateTime now)
		{
			int num;
			return this.IsInWeekAwardTime(now, out num);
		}

		private bool IsInWeekOnceActTimes(DateTime time)
		{
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			foreach (CoupleAreanWarCfg.TimePoint timePoint in this.WarCfg.TimePoints)
			{
				if (timePoint.Weekday == weekDay1To && time.TimeOfDay.Ticks >= timePoint.DayStartTicks && time.TimeOfDay.Ticks <= timePoint.DayEndTicks)
				{
					return true;
				}
			}
			return false;
		}

		public bool IsNowCanDivorce(DateTime time)
		{
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			foreach (CoupleAreanWarCfg.TimePoint timePoint in this.WarCfg.TimePoints)
			{
				if (timePoint.Weekday == weekDay1To && time.TimeOfDay.Ticks >= timePoint.DayStartTicks - (long)((ulong)-1294967296) && time.TimeOfDay.Ticks <= timePoint.DayEndTicks + (long)((ulong)-1294967296))
				{
					return false;
				}
			}
			return true;
		}

		private bool IsInWeekAwardTime(DateTime time, out int week)
		{
			week = 0;
			CoupleAreanWarCfg.TimePoint timePoint = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
			CoupleAreanWarCfg.TimePoint timePoint2 = this.WarCfg.TimePoints.Last<CoupleAreanWarCfg.TimePoint>();
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			bool result;
			if (weekDay1To < timePoint.Weekday || (weekDay1To == timePoint.Weekday && time.TimeOfDay.Ticks < timePoint.DayStartTicks))
			{
				week = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
				result = true;
			}
			else if (weekDay1To < timePoint2.Weekday || (weekDay1To == timePoint2.Weekday && time.TimeOfDay.Ticks < timePoint2.DayEndTicks + 6000000000L))
			{
				week = 0;
				result = false;
			}
			else
			{
				week = TimeUtil.MakeFirstWeekday(time);
				result = true;
			}
			return result;
		}

		private int CurrRankWeek(DateTime time)
		{
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			CoupleAreanWarCfg.TimePoint timePoint = this.WarCfg.TimePoints.First<CoupleAreanWarCfg.TimePoint>();
			int result;
			if (weekDay1To < timePoint.Weekday || (weekDay1To == timePoint.Weekday && time.TimeOfDay.Ticks < timePoint.DayStartTicks))
			{
				result = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
			}
			else
			{
				result = TimeUtil.MakeFirstWeekday(time);
			}
			return result;
		}

		private bool IsStateWatcher(int roleId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.coupleStateWatchers.Contains(roleId);
			}
			return result;
		}

		private void RegStateWatcher(int roleId, bool bWatch)
		{
			lock (this.Mutex)
			{
				if (bWatch && !this.coupleStateWatchers.Contains(roleId))
				{
					this.coupleStateWatchers.Add(roleId);
				}
				if (!bWatch)
				{
					this.coupleStateWatchers.Remove(roleId);
				}
			}
		}

		private ECoupleArenaMatchState GetMatchState(int roleId)
		{
			ECoupleArenaMatchState result;
			lock (this.Mutex)
			{
				ECoupleArenaMatchState ecoupleArenaMatchState;
				if (!this.RoleMatchStateDict.TryGetValue(roleId, out ecoupleArenaMatchState))
				{
					ecoupleArenaMatchState = ECoupleArenaMatchState.Offline;
				}
				result = ecoupleArenaMatchState;
			}
			return result;
		}

		private void SetMatchState(int roleId, ECoupleArenaMatchState state)
		{
			lock (this.Mutex)
			{
				if (state == ECoupleArenaMatchState.Offline)
				{
					this.RoleMatchStateDict.Remove(roleId);
				}
				else
				{
					this.RoleMatchStateDict[roleId] = state;
				}
				if (state == ECoupleArenaMatchState.Ready)
				{
					this.RoleStartReadyMs[roleId] = TimeUtil.NOW();
				}
				else
				{
					this.RoleStartReadyMs.Remove(roleId);
				}
			}
		}

		private void NtfCoupleMatchState(int roleId)
		{
			if (this.IsStateWatcher(roleId))
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(roleId);
				if (gameClient != null)
				{
					CoupleArenaRoleStateData coupleArenaRoleStateData = new CoupleArenaRoleStateData
					{
						RoleId = gameClient.ClientData.RoleID
					};
					CoupleArenaRoleStateData coupleArenaRoleStateData2 = null;
					if (MarryLogic.IsMarried(gameClient.ClientData.RoleID))
					{
						coupleArenaRoleStateData2 = new CoupleArenaRoleStateData
						{
							RoleId = gameClient.ClientData.MyMarriageData.nSpouseID
						};
					}
					lock (this.Mutex)
					{
						coupleArenaRoleStateData.MatchState = (int)this.GetMatchState(coupleArenaRoleStateData.RoleId);
						if (coupleArenaRoleStateData2 != null)
						{
							coupleArenaRoleStateData2.MatchState = (int)this.GetMatchState(coupleArenaRoleStateData2.RoleId);
						}
					}
					List<CoupleArenaRoleStateData> list = new List<CoupleArenaRoleStateData>();
					list.Add(coupleArenaRoleStateData);
					if (coupleArenaRoleStateData2 != null)
					{
						list.Add(coupleArenaRoleStateData2);
					}
					gameClient.sendCmd<List<CoupleArenaRoleStateData>>(1379, list, false);
				}
			}
		}

		public CoupleArenaCoupleJingJiData GetCachedCoupleData(int roleId)
		{
			CoupleArenaCoupleJingJiData result;
			lock (this.Mutex)
			{
				CoupleArenaCoupleJingJiData coupleArenaCoupleJingJiData = null;
				if (!this.SyncRoleDict.TryGetValue(roleId, out coupleArenaCoupleJingJiData))
				{
					coupleArenaCoupleJingJiData = null;
				}
				result = coupleArenaCoupleJingJiData;
			}
			return result;
		}

		private CoupleArenaCoupleJingJiData ConvertToJiJiData(CoupleArenaCoupleDataK kData)
		{
			return new CoupleArenaCoupleJingJiData
			{
				ManRoleId = kData.ManRoleId,
				ManZoneId = kData.ManZoneId,
				ManSelector = ((kData.ManSelectorData != null) ? DataHelper.BytesToObject<RoleData4Selector>(kData.ManSelectorData, 0, kData.ManSelectorData.Length) : null),
				WifeRoleId = kData.WifeRoleId,
				WifeZoneId = kData.WifeZoneId,
				WifeSelector = ((kData.WifeSelectorData != null) ? DataHelper.BytesToObject<RoleData4Selector>(kData.WifeSelectorData, 0, kData.WifeSelectorData.Length) : null),
				TotalFightTimes = kData.TotalFightTimes,
				WinFightTimes = kData.WinFightTimes,
				LianShengTimes = kData.LianShengTimes,
				DuanWeiType = kData.DuanWeiType,
				DuanWeiLevel = kData.DuanWeiLevel,
				JiFen = kData.JiFen,
				Rank = kData.Rank,
				IsDivorced = kData.IsDivorced
			};
		}

		private void TimerProc(object sender, EventArgs e)
		{
			CoupleArenaSyncData coupleArenaSyncData = TianTiClient.getInstance().CoupleArenaSync(this.SyncDateTime);
			if (coupleArenaSyncData != null)
			{
				lock (this.Mutex)
				{
					this.SyncRankList.Clear();
					this.SyncRoleDict.Clear();
					if (coupleArenaSyncData.RankList != null)
					{
						this.SyncRankList.AddRange(from _r in coupleArenaSyncData.RankList
						select this.ConvertToJiJiData(_r));
						foreach (CoupleArenaCoupleJingJiData coupleArenaCoupleJingJiData in this.SyncRankList)
						{
							this.SyncRoleDict[coupleArenaCoupleJingJiData.ManRoleId] = coupleArenaCoupleJingJiData;
							this.SyncRoleDict[coupleArenaCoupleJingJiData.WifeRoleId] = coupleArenaCoupleJingJiData;
						}
					}
					if (this.SyncRankList.Count > 0 && this.SyncRankList[0].Rank == 1 && this.SyncRankList[0].IsDivorced == 0)
					{
						this.SetFengHuoJiaRenCouple(this.SyncRankList[0].ManRoleId, this.SyncRankList[0].WifeRoleId);
					}
					else
					{
						this.SetFengHuoJiaRenCouple(0, 0);
					}
					this.SyncDateTime = coupleArenaSyncData.ModifyTime;
				}
			}
		}

		public bool CanKuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			return true;
		}

		public bool KuaFuInitGame(GameClient client)
		{
			long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
			lock (this.Mutex)
			{
				CoupleArenaFuBenData coupleArenaFuBenData = null;
				if (!this.GameId2FuBenData.TryGetValue(gameId, out coupleArenaFuBenData))
				{
					coupleArenaFuBenData = TianTiClient.getInstance().GetFuBenData(gameId);
					if (coupleArenaFuBenData != null)
					{
						if (coupleArenaFuBenData.FuBenSeq == 0)
						{
							coupleArenaFuBenData.FuBenSeq = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						this.GameId2FuBenData.Add(gameId, coupleArenaFuBenData);
					}
				}
				if (coupleArenaFuBenData == null)
				{
					return false;
				}
				if (coupleArenaFuBenData.KfServerId != GameCoreInterface.getinstance().GetLocalServerId())
				{
					return false;
				}
				KuaFuFuBenRoleData kuaFuFuBenRoleData = null;
				bool flag2;
				if (coupleArenaFuBenData.RoleList != null)
				{
					flag2 = ((kuaFuFuBenRoleData = coupleArenaFuBenData.RoleList.Find((KuaFuFuBenRoleData _r) => _r.RoleId == client.ClientData.RoleID)) != null);
				}
				else
				{
					flag2 = false;
				}
				if (!flag2)
				{
					return false;
				}
				client.ClientData.MapCode = this.WarCfg.MapCode;
				client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				int posX = 0;
				int posY = 0;
				if (!this.GetBirthPoint(client.ClientData.MapCode, client.ClientData.BattleWhichSide, out posX, out posY))
				{
					LogManager.WriteLog(2, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
					return false;
				}
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				Global.GetClientKuaFuServerLoginData(client).FuBenSeqId = coupleArenaFuBenData.FuBenSeq;
				client.ClientData.FuBenSeqID = coupleArenaFuBenData.FuBenSeq;
			}
			return true;
		}

		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.WarCfg.MapCode)
			{
				int posX;
				int posY;
				if (!this.GetBirthPoint(this.WarCfg.MapCode, client.ClientData.BattleWhichSide, out posX, out posY))
				{
					result = false;
				}
				else
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, posX, posY, -1);
					Global.ClientRealive(client, posX, posY, -1);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
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
				int posX = this.BirthPointList[side % this.BirthPointList.Count].PosX;
				int posY = this.BirthPointList[side % this.BirthPointList.Count].PosY;
				int birthRadius = this.BirthPointList[side % this.BirthPointList.Count].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, posX, posY, birthRadius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				toPosX = posX;
				toPosY = posY;
				result = true;
			}
			return result;
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client)
		{
			lock (this.Mutex)
			{
				CoupleArenaCopyScene coupleArenaCopyScene;
				if (this.FuBenSeq2CopyScenes.TryGetValue(client.ClientData.FuBenSeqID, out coupleArenaCopyScene))
				{
					client.sendCmd<GameSceneStateTimeData>(827, coupleArenaCopyScene.StateTimeData, false);
				}
			}
		}

		public void OnLeaveFuBen(GameClient client)
		{
			lock (this.Mutex)
			{
				CoupleArenaCopyScene coupleArenaCopyScene = null;
				if (this.FuBenSeq2CopyScenes.TryGetValue(client.ClientData.FuBenSeqID, out coupleArenaCopyScene))
				{
					if (coupleArenaCopyScene.m_eStatus < 2)
					{
						coupleArenaCopyScene.EnterRoleSide.Remove(client.ClientData.RoleID);
					}
					else if (coupleArenaCopyScene.m_eStatus < 3)
					{
						CoupleArenaBuffCfg buffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
						CoupleArenaBuffCfg buffCfg2 = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
						this.ModifyBuff(coupleArenaCopyScene, client, BufferItemTypes.CoupleArena_YongQi_Buff, buffCfg2, false);
						this.ModifyBuff(coupleArenaCopyScene, client, BufferItemTypes.CoupleArena_ZhenAi_Buff, buffCfg, false);
						coupleArenaCopyScene.EnterRoleSide.Remove(client.ClientData.RoleID);
						List<int> list = coupleArenaCopyScene.EnterRoleSide.Values.ToList<int>();
						if (list.Count((int _s) => _s == client.ClientData.BattleWhichSide) <= 0)
						{
							coupleArenaCopyScene.WinSide = ((list.Count > 0) ? list[0] : 0);
							this.ProcessEnd(coupleArenaCopyScene, TimeUtil.NowDateTime(), TimeUtil.NOW());
						}
					}
				}
			}
		}

		public void AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (sceneType == 38)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.Mutex)
				{
					CoupleArenaCopyScene coupleArenaCopyScene = null;
					if (!this.FuBenSeq2CopyScenes.TryGetValue(fuBenSeqID, out coupleArenaCopyScene))
					{
						coupleArenaCopyScene = new CoupleArenaCopyScene();
						coupleArenaCopyScene.GameId = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
						coupleArenaCopyScene.FuBenSeq = fuBenSeqID;
						coupleArenaCopyScene.MapCode = mapCode;
						coupleArenaCopyScene.CopyMap = copyMap;
						this.FuBenSeq2CopyScenes[fuBenSeqID] = coupleArenaCopyScene;
					}
					coupleArenaCopyScene.EnterRoleSide[client.ClientData.RoleID] = client.ClientData.BattleWhichSide;
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((this.WarCfg.WaitSec + this.WarCfg.FightSec + this.WarCfg.ClearSec + 120) * 1000));
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 13);
				}
			}
		}

		public void RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (copyMap != null && sceneType == 38)
			{
				lock (this.Mutex)
				{
					CoupleArenaCopyScene coupleArenaCopyScene = null;
					if (this.FuBenSeq2CopyScenes.TryGetValue(copyMap.FuBenSeqID, out coupleArenaCopyScene))
					{
						this.FuBenSeq2CopyScenes.Remove(copyMap.FuBenSeqID);
						this.GameId2FuBenData.Remove((long)coupleArenaCopyScene.GameId);
					}
				}
			}
		}

		public void UpdateCopyScene()
		{
			DateTime now = TimeUtil.NowDateTime();
			long num = now.Ticks / 10000L;
			if (num >= CoupleArenaManager.NextHeartBeatTicks)
			{
				CoupleArenaManager.NextHeartBeatTicks = num + 1020L;
				lock (this.Mutex)
				{
					foreach (CoupleArenaCopyScene coupleArenaCopyScene in this.FuBenSeq2CopyScenes.Values.ToList<CoupleArenaCopyScene>())
					{
						coupleArenaCopyScene.m_lPrevUpdateTime = coupleArenaCopyScene.m_lCurrUpdateTime;
						coupleArenaCopyScene.m_lCurrUpdateTime = num;
						if (coupleArenaCopyScene.m_eStatus == 0)
						{
							this.NtfBuffHoldData(coupleArenaCopyScene);
							coupleArenaCopyScene.m_lPrepareTime = num;
							coupleArenaCopyScene.m_lBeginTime = num + (long)(this.WarCfg.WaitSec * 1000);
							coupleArenaCopyScene.m_eStatus = 1;
							coupleArenaCopyScene.StateTimeData.GameType = 13;
							coupleArenaCopyScene.StateTimeData.State = coupleArenaCopyScene.m_eStatus;
							coupleArenaCopyScene.StateTimeData.EndTicks = coupleArenaCopyScene.m_lBeginTime;
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, coupleArenaCopyScene.StateTimeData, coupleArenaCopyScene.CopyMap);
						}
						else if (coupleArenaCopyScene.m_eStatus == 1)
						{
							if (num >= coupleArenaCopyScene.m_lBeginTime)
							{
								if (coupleArenaCopyScene.EnterRoleSide.Values.ToList<int>().Distinct<int>().Count<int>() <= 1)
								{
									coupleArenaCopyScene.WinSide = 0;
									coupleArenaCopyScene.m_eStatus = 3;
								}
								else
								{
									this.NtfBuffHoldData(coupleArenaCopyScene);
									coupleArenaCopyScene.m_eStatus = 2;
									coupleArenaCopyScene.m_lEndTime = num + (long)(this.WarCfg.FightSec * 1000);
									coupleArenaCopyScene.StateTimeData.GameType = 13;
									coupleArenaCopyScene.StateTimeData.State = coupleArenaCopyScene.m_eStatus;
									coupleArenaCopyScene.StateTimeData.EndTicks = coupleArenaCopyScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, coupleArenaCopyScene.StateTimeData, coupleArenaCopyScene.CopyMap);
									coupleArenaCopyScene.CopyMap.AddGuangMuEvent(1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(coupleArenaCopyScene.CopyMap.MapCode, coupleArenaCopyScene.CopyMap.CopyMapID, 1, 0);
									coupleArenaCopyScene.CopyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(coupleArenaCopyScene.CopyMap.MapCode, coupleArenaCopyScene.CopyMap.CopyMapID, 2, 0);
								}
							}
						}
						else if (coupleArenaCopyScene.m_eStatus == 2)
						{
							if (num >= coupleArenaCopyScene.m_lEndTime)
							{
								coupleArenaCopyScene.m_eStatus = 3;
								if (!coupleArenaCopyScene.EnterRoleSide.TryGetValue(coupleArenaCopyScene.ZhenAiBuff_Role, out coupleArenaCopyScene.WinSide))
								{
									coupleArenaCopyScene.WinSide = 0;
								}
							}
							else if (coupleArenaCopyScene.EnterRoleSide.ContainsKey(coupleArenaCopyScene.ZhenAiBuff_Role) && num - coupleArenaCopyScene.ZhenAiBuff_StartMs >= (long)(this.ZhenAiBuffHoldWinSec * 1000))
							{
								coupleArenaCopyScene.m_eStatus = 3;
								coupleArenaCopyScene.WinSide = coupleArenaCopyScene.EnterRoleSide[coupleArenaCopyScene.ZhenAiBuff_Role];
							}
							else
							{
								this.CheckFlushZhenAiMonster(coupleArenaCopyScene);
								this.CheckFlushYongQiMonster(coupleArenaCopyScene);
							}
						}
						else if (coupleArenaCopyScene.m_eStatus == 3)
						{
							this.ProcessEnd(coupleArenaCopyScene, now, num);
						}
						else if (coupleArenaCopyScene.m_eStatus == 4)
						{
							if (num >= coupleArenaCopyScene.m_lLeaveTime)
							{
								coupleArenaCopyScene.m_eStatus = 5;
								coupleArenaCopyScene.CopyMap.SetRemoveTicks(coupleArenaCopyScene.m_lLeaveTime);
								try
								{
									List<GameClient> clientsList = coupleArenaCopyScene.CopyMap.GetClientsList();
									if (clientsList != null && clientsList.Count > 0)
									{
										for (int i = 0; i < clientsList.Count; i++)
										{
											GameClient gameClient = clientsList[i];
											if (gameClient != null)
											{
												KuaFuManager.getInstance().GotoLastMap(gameClient);
											}
										}
									}
								}
								catch (Exception ex)
								{
									DataHelper.WriteExceptionLogEx(ex, "情侣竞技系统清场调度异常");
								}
							}
						}
					}
				}
			}
		}

		private void CheckFlushZhenAiMonster(CoupleArenaCopyScene scene)
		{
			if (!scene.IsZhenAiMonsterExist && !scene.EnterRoleSide.ContainsKey(scene.ZhenAiBuff_Role))
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapCode, out gameMap))
				{
					LogManager.WriteLog(1000, string.Format("缺少情侣竞技地图 {0}", scene.MapCode), null, true);
				}
				else
				{
					CoupleArenaBuffCfg coupleArenaBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
					if (coupleArenaBuffCfg != null)
					{
						CoupleArenaBuffCfg.RandPos randPos = coupleArenaBuffCfg.RandPosList[Global.GetRandomNumber(0, coupleArenaBuffCfg.RandPosList.Count)];
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapCode, coupleArenaBuffCfg.MonsterId, scene.CopyMap.CopyMapID, 1, randPos.X / gameMap.MapGridWidth, randPos.Y / gameMap.MapGridHeight, randPos.R, 0, 38, null, null);
						scene.IsZhenAiMonsterExist = true;
					}
				}
			}
		}

		private void CheckFlushYongQiMonster(CoupleArenaCopyScene scene)
		{
			CoupleArenaBuffCfg coupleArenaBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
			if (coupleArenaBuffCfg != null)
			{
				bool flag = false;
				foreach (int num in coupleArenaBuffCfg.FlushSecList)
				{
					if (scene.m_lPrevUpdateTime - scene.m_lBeginTime <= (long)(num * 1000) && scene.m_lCurrUpdateTime - scene.m_lBeginTime >= (long)(num * 1000))
					{
						flag = true;
						break;
					}
				}
				if (flag && !scene.IsYongQiMonsterExist && !scene.EnterRoleSide.ContainsKey(scene.YongQiBuff_Role))
				{
					GameMap gameMap = null;
					if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapCode, out gameMap))
					{
						LogManager.WriteLog(1000, string.Format("缺少情侣竞技地图 {0}", scene.MapCode), null, true);
					}
					else
					{
						CoupleArenaBuffCfg.RandPos randPos = coupleArenaBuffCfg.RandPosList[Global.GetRandomNumber(0, coupleArenaBuffCfg.RandPosList.Count)];
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapCode, coupleArenaBuffCfg.MonsterId, scene.CopyMap.CopyMapID, 1, randPos.X / gameMap.MapGridWidth, randPos.Y / gameMap.MapGridHeight, randPos.R, 0, 38, null, null);
						scene.IsYongQiMonsterExist = true;
					}
				}
			}
		}

		private void ProcessEnd(CoupleArenaCopyScene scene, DateTime now, long nowTicks)
		{
			GameManager.CopyMapMgr.KillAllMonster(scene.CopyMap);
			scene.m_eStatus = 4;
			scene.m_lEndTime = nowTicks;
			scene.m_lLeaveTime = scene.m_lEndTime + (long)(this.WarCfg.ClearSec * 1000);
			scene.StateTimeData.GameType = 13;
			scene.StateTimeData.State = 3;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
			CoupleArenaFuBenData coupleArenaFuBenData = null;
			if (this.GameId2FuBenData.TryGetValue((long)scene.GameId, out coupleArenaFuBenData))
			{
				List<RoleData4Selector> list = new List<RoleData4Selector>();
				foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in coupleArenaFuBenData.RoleList)
				{
					RoleData4Selector roleData4Selector = Global.sendToDB<RoleData4Selector, int>(10232, kuaFuFuBenRoleData.RoleId, kuaFuFuBenRoleData.ServerId);
					if (roleData4Selector == null || roleData4Selector.RoleID <= 0)
					{
						LogManager.WriteLog(2, string.Format("加载RoleData4Selector失败, serverid={0}, roleid={1}", kuaFuFuBenRoleData.ServerId, kuaFuFuBenRoleData.RoleId), null, true);
						return;
					}
					list.Add(roleData4Selector);
				}
				if (!MarryLogic.SameSexMarry(false))
				{
					if (list[0].RoleSex == 1)
					{
						RoleData4Selector value = list[0];
						list[0] = list[1];
						list[1] = value;
					}
					if (list[2].RoleSex == 1)
					{
						RoleData4Selector value = list[2];
						list[2] = list[3];
						list[3] = value;
					}
				}
				CoupleArenaPkResultReq coupleArenaPkResultReq = new CoupleArenaPkResultReq();
				coupleArenaPkResultReq.GameId = (long)scene.GameId;
				coupleArenaPkResultReq.winSide = scene.WinSide;
				coupleArenaPkResultReq.ManRole1 = list[0].RoleID;
				coupleArenaPkResultReq.ManZoneId1 = list[0].ZoneId;
				coupleArenaPkResultReq.ManSelector1 = DataHelper.ObjectToBytes<RoleData4Selector>(list[0]);
				coupleArenaPkResultReq.WifeRole1 = list[1].RoleID;
				coupleArenaPkResultReq.WifeZoneId1 = list[1].ZoneId;
				coupleArenaPkResultReq.WifeSelector1 = DataHelper.ObjectToBytes<RoleData4Selector>(list[1]);
				coupleArenaPkResultReq.ManRole2 = list[2].RoleID;
				coupleArenaPkResultReq.ManZoneId2 = list[2].ZoneId;
				coupleArenaPkResultReq.ManSelector2 = DataHelper.ObjectToBytes<RoleData4Selector>(list[2]);
				coupleArenaPkResultReq.WifeRole2 = list[3].RoleID;
				coupleArenaPkResultReq.WifeZoneId2 = list[3].ZoneId;
				coupleArenaPkResultReq.WifeSelector2 = DataHelper.ObjectToBytes<RoleData4Selector>(list[3]);
				CoupleArenaPkResultRsp coupleArenaPkResultRsp = TianTiClient.getInstance().CoupleArenaPkResult(coupleArenaPkResultReq);
				if (coupleArenaPkResultRsp != null)
				{
					if (coupleArenaPkResultRsp.Couple1RetData != null)
					{
						if (coupleArenaPkResultRsp.Couple1RetData.Result != 0)
						{
							Global.sendToDB<bool, CoupleArenaZhanBaoSaveDbData>(1382, new CoupleArenaZhanBaoSaveDbData
							{
								FromMan = coupleArenaPkResultReq.ManRole1,
								FromWife = coupleArenaPkResultReq.WifeRole1,
								FirstWeekday = TimeUtil.MakeFirstWeekday(now),
								ZhanBao = new CoupleArenaZhanBaoItemData
								{
									TargetManZoneId = coupleArenaPkResultReq.ManZoneId2,
									TargetManRoleName = list[2].RoleName,
									TargetWifeZoneId = coupleArenaPkResultReq.WifeZoneId2,
									TargetWifeRoleName = list[3].RoleName,
									IsWin = (coupleArenaPkResultRsp.Couple1RetData.Result == 1),
									GetJiFen = coupleArenaPkResultRsp.Couple1RetData.GetJiFen
								}
							}, coupleArenaFuBenData.RoleList[0].ServerId);
						}
						this.NtfAwardData(coupleArenaPkResultReq.ManRole1, coupleArenaPkResultRsp.Couple1RetData);
						this.NtfAwardData(coupleArenaPkResultReq.WifeRole1, coupleArenaPkResultRsp.Couple1RetData);
					}
					if (coupleArenaPkResultRsp.Couple2RetData != null)
					{
						if (coupleArenaPkResultRsp.Couple2RetData.Result != 0)
						{
							Global.sendToDB<bool, CoupleArenaZhanBaoSaveDbData>(1382, new CoupleArenaZhanBaoSaveDbData
							{
								FirstWeekday = TimeUtil.MakeFirstWeekday(now),
								FromMan = coupleArenaPkResultReq.ManRole2,
								FromWife = coupleArenaPkResultReq.WifeRole2,
								ZhanBao = new CoupleArenaZhanBaoItemData
								{
									TargetManZoneId = coupleArenaPkResultReq.ManZoneId1,
									TargetManRoleName = list[0].RoleName,
									TargetWifeZoneId = coupleArenaPkResultReq.WifeZoneId1,
									TargetWifeRoleName = list[1].RoleName,
									IsWin = (coupleArenaPkResultRsp.Couple2RetData.Result == 1),
									GetJiFen = coupleArenaPkResultRsp.Couple2RetData.GetJiFen
								}
							}, coupleArenaFuBenData.RoleList[2].ServerId);
						}
						this.NtfAwardData(coupleArenaPkResultReq.ManRole2, coupleArenaPkResultRsp.Couple2RetData);
						this.NtfAwardData(coupleArenaPkResultReq.WifeRole2, coupleArenaPkResultRsp.Couple2RetData);
					}
				}
			}
		}

		private void NtfAwardData(int roleid, CoupleArenaPkResultItem retItem)
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleid);
			if (gameClient != null && gameClient.ClientData.MapCode == this.WarCfg.MapCode)
			{
				CoupleAreanDuanWeiCfg coupleAreanDuanWeiCfg = this.DuanWeiCfgList.Find((CoupleAreanDuanWeiCfg _d) => _d.Type == retItem.OldDuanWeiType && _d.Level == retItem.OldDuanWeiLevel);
				if (coupleAreanDuanWeiCfg == null)
				{
					LogManager.WriteLog(2, string.Format("NtfAwardData 段位配置找不到 type={0},level={1}", retItem.OldDuanWeiType, retItem.OldDuanWeiLevel), null, true);
				}
				else
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					int num = 0;
					bool flag = false;
					string roleParamByName = Global.GetRoleParamByName(gameClient, "27");
					if (!string.IsNullOrEmpty(roleParamByName))
					{
						string[] array = roleParamByName.Split(new char[]
						{
							','
						});
						if (array != null && array.Length == 2 && Convert.ToInt32(array[0]) == TimeUtil.MakeFirstWeekday(dateTime))
						{
							num = Convert.ToInt32(array[1]);
						}
					}
					CoupleArenaPkResultData coupleArenaPkResultData = new CoupleArenaPkResultData();
					coupleArenaPkResultData.PKResult = retItem.Result;
					coupleArenaPkResultData.DuanWeiType = retItem.NewDuanWeiType;
					coupleArenaPkResultData.DuanWeiLevel = retItem.NewDuanWeiLevel;
					coupleArenaPkResultData.GetJiFen = retItem.GetJiFen;
					if (retItem.Result != 0 && num < coupleAreanDuanWeiCfg.WeekGetRongYaoTimes)
					{
						coupleArenaPkResultData.GetRongYao = ((retItem.Result == 1) ? coupleAreanDuanWeiCfg.WinRongYao : coupleAreanDuanWeiCfg.LoseRongYao);
						flag = true;
					}
					if (flag)
					{
						GameManager.ClientMgr.ModifyTianTiRongYaoValue(gameClient, coupleArenaPkResultData.GetRongYao, "情侣竞技系统获得荣耀", true);
						Global.SaveRoleParamsStringToDB(gameClient, "27", string.Format("{0},{1}", TimeUtil.MakeFirstWeekday(dateTime), num + 1), true);
					}
					gameClient.sendCmd<CoupleArenaPkResultData>(1378, coupleArenaPkResultData, false);
				}
			}
		}

		private void OnMonsterDead(CoupleArenaCopyScene scene, GameClient client, Monster monster)
		{
			lock (this.Mutex)
			{
				if (scene.m_eStatus >= 2 && scene.m_eStatus < 3)
				{
					if (scene.EnterRoleSide.ContainsKey(client.ClientData.RoleID))
					{
						CoupleArenaBuffCfg coupleArenaBuffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
						CoupleArenaBuffCfg coupleArenaBuffCfg2 = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
						if (scene.IsZhenAiMonsterExist && monster.MonsterInfo.ExtensionID == coupleArenaBuffCfg.MonsterId)
						{
							scene.IsZhenAiMonsterExist = false;
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_YongQi_Buff, coupleArenaBuffCfg2, false);
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_ZhenAi_Buff, coupleArenaBuffCfg, true);
						}
						if (scene.IsYongQiMonsterExist && monster.MonsterInfo.ExtensionID == coupleArenaBuffCfg2.MonsterId)
						{
							scene.IsYongQiMonsterExist = false;
							this.ModifyBuff(scene, client, BufferItemTypes.CoupleArena_YongQi_Buff, coupleArenaBuffCfg2, true);
						}
					}
				}
			}
		}

		private void OnPlayerDead(CoupleArenaCopyScene scene, GameClient deader, GameClient killer)
		{
			lock (this.Mutex)
			{
				if (scene.m_eStatus >= 2 && scene.m_eStatus < 3)
				{
					if (scene.EnterRoleSide.ContainsKey(deader.ClientData.RoleID))
					{
						if (killer == null || scene.EnterRoleSide.ContainsKey(killer.ClientData.RoleID))
						{
							CoupleArenaBuffCfg buffCfg = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.ZhenAiBuffCfgType);
							CoupleArenaBuffCfg buffCfg2 = this.BuffCfgList.Find((CoupleArenaBuffCfg _b) => _b.Type == CoupleAreanConsts.YongQiBuffCfgType);
							if (scene.ZhenAiBuff_Role == deader.ClientData.RoleID)
							{
								this.ModifyBuff(scene, deader, BufferItemTypes.CoupleArena_ZhenAi_Buff, buffCfg, false);
								this.ModifyBuff(scene, killer, BufferItemTypes.CoupleArena_YongQi_Buff, buffCfg2, false);
								this.ModifyBuff(scene, killer, BufferItemTypes.CoupleArena_ZhenAi_Buff, buffCfg, true);
							}
							if (scene.YongQiBuff_Role == deader.ClientData.RoleID)
							{
								this.ModifyBuff(scene, deader, BufferItemTypes.CoupleArena_YongQi_Buff, buffCfg2, false);
							}
						}
					}
				}
			}
		}

		private void ModifyBuff(CoupleArenaCopyScene scene, GameClient client, BufferItemTypes buffType, CoupleArenaBuffCfg buffCfg, bool bAdd)
		{
			if (scene != null && client != null && buffCfg != null)
			{
				lock (this.Mutex)
				{
					bool flag2 = false;
					BufferData bufferDataByID = Global.GetBufferDataByID(client, (int)buffType);
					int bufferType = 1;
					if (bAdd && (bufferDataByID == null || Global.IsBufferDataOver(bufferDataByID, 0L)))
					{
						if (buffType != BufferItemTypes.CoupleArena_YongQi_Buff || scene.ZhenAiBuff_Role != client.ClientData.RoleID)
						{
							double[] actionParams = new double[]
							{
								1.0
							};
							Global.UpdateBufferData(client, buffType, actionParams, bufferType, true);
							foreach (KeyValuePair<ExtPropIndexes, double> keyValuePair in buffCfg.ExtProps)
							{
								client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
								{
									21,
									(int)keyValuePair.Key,
									keyValuePair.Value
								});
							}
							flag2 = true;
						}
					}
					if (!bAdd && bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
					{
						double[] array = new double[1];
						double[] actionParams = array;
						Global.UpdateBufferData(client, buffType, actionParams, bufferType, true);
						foreach (KeyValuePair<ExtPropIndexes, double> keyValuePair in buffCfg.ExtProps)
						{
							client.ClientData.PropsCacheManager.SetExtPropsSingle(new object[]
							{
								21,
								(int)keyValuePair.Key,
								0
							});
						}
						flag2 = true;
					}
					if (flag2)
					{
						if (buffType == BufferItemTypes.CoupleArena_ZhenAi_Buff)
						{
							if (bAdd)
							{
								scene.ZhenAiBuff_Role = client.ClientData.RoleID;
								scene.ZhenAiBuff_StartMs = TimeUtil.NOW();
							}
							else
							{
								scene.ZhenAiBuff_Role = 0;
							}
						}
						else if (buffType == BufferItemTypes.CoupleArena_YongQi_Buff)
						{
							if (bAdd)
							{
								scene.YongQiBuff_Role = client.ClientData.RoleID;
							}
							else
							{
								scene.YongQiBuff_Role = 0;
							}
						}
						this.NtfBuffHoldData(scene);
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					}
				}
			}
		}

		private void NtfBuffHoldData(CoupleArenaCopyScene scene)
		{
			lock (this.Mutex)
			{
				CoupleArenaFuBenData coupleArenaFuBenData;
				if (this.GameId2FuBenData.TryGetValue((long)scene.GameId, out coupleArenaFuBenData))
				{
					CoupleArenaBuffHoldData coupleArenaBuffHoldData = new CoupleArenaBuffHoldData();
					GameClient gameClient = GameManager.ClientMgr.FindClient(scene.ZhenAiBuff_Role);
					if (gameClient != null && scene.EnterRoleSide.ContainsKey(gameClient.ClientData.RoleID))
					{
						coupleArenaBuffHoldData.IsZhenAiBuffValid = true;
						coupleArenaBuffHoldData.ZhenAiHolderZoneId = gameClient.ClientData.ZoneID;
						coupleArenaBuffHoldData.ZhenAiHolderRname = gameClient.ClientData.RoleName;
					}
					else
					{
						coupleArenaBuffHoldData.IsZhenAiBuffValid = false;
					}
					gameClient = GameManager.ClientMgr.FindClient(scene.YongQiBuff_Role);
					if (gameClient != null && scene.EnterRoleSide.ContainsKey(scene.YongQiBuff_Role))
					{
						coupleArenaBuffHoldData.IsYongQiBuffValid = true;
						coupleArenaBuffHoldData.YongQiHolderZoneId = gameClient.ClientData.ZoneID;
						coupleArenaBuffHoldData.YongQiHolderRname = gameClient.ClientData.RoleName;
					}
					else
					{
						coupleArenaBuffHoldData.IsYongQiBuffValid = false;
					}
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<CoupleArenaBuffHoldData>(1381, coupleArenaBuffHoldData, scene.CopyMap);
				}
			}
		}

		private object Mutex = new object();

		private DateTime SyncDateTime = DateTime.MinValue;

		private List<CoupleArenaCoupleJingJiData> SyncRankList = new List<CoupleArenaCoupleJingJiData>();

		private Dictionary<int, CoupleArenaCoupleJingJiData> SyncRoleDict = new Dictionary<int, CoupleArenaCoupleJingJiData>();

		private Dictionary<int, ECoupleArenaMatchState> RoleMatchStateDict = new Dictionary<int, ECoupleArenaMatchState>();

		private Dictionary<int, long> RoleStartReadyMs = new Dictionary<int, long>();

		private Dictionary<int, int> RoleMatchKeyDict = new Dictionary<int, int>();

		private HashSet<int> coupleStateWatchers = new HashSet<int>();

		private CoupleAreanWarCfg WarCfg = new CoupleAreanWarCfg();

		private List<CoupleAreanDuanWeiCfg> DuanWeiCfgList = new List<CoupleAreanDuanWeiCfg>();

		private List<CoupleAreanWeekRankAwardCfg> WeekAwardCfgList = new List<CoupleAreanWeekRankAwardCfg>();

		private List<CoupleArenaBuffCfg> BuffCfgList = new List<CoupleArenaBuffCfg>();

		private List<TianTiBirthPoint> BirthPointList = new List<TianTiBirthPoint>();

		private int ZhenAiBuffHoldWinSec = 60;

		private double YongQiBuff2ZhenAiBuffHurt = 0.2;

		private Dictionary<long, CoupleArenaFuBenData> GameId2FuBenData = new Dictionary<long, CoupleArenaFuBenData>();

		private Dictionary<int, CoupleArenaCopyScene> FuBenSeq2CopyScenes = new Dictionary<int, CoupleArenaCopyScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
