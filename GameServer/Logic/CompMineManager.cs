using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class CompMineManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static CompMineManager getInstance()
		{
			return CompMineManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompMineManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2010, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2011, 2, 2, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2012, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2013, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2015, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2018, 1, 1, CompMineManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(34, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 53, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompMineManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompMineManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(34, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 53, CompMineManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 53, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompMineManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompMineManager.getInstance());
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

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 98, hint) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(120403);
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			if (!this.IsGongNengOpened(client, false))
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 2010:
					return this.ProcessGetCompMineBaseDataCmd(client, nID, bytes, cmdParams);
				case 2011:
					return this.ProcessCompMineEnterCmd(client, nID, bytes, cmdParams);
				case 2012:
					return this.ProcessGetCompMineAwardInfoCmd(client, nID, bytes, cmdParams);
				case 2013:
					return this.ProcessGetCompMineStateCmd(client, nID, bytes, cmdParams);
				case 2015:
					return this.ProcessGetCompMineSelfScoreCmd(client, nID, bytes, cmdParams);
				case 2018:
					return this.ProcessGetCompMineAwardCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(onStartPlayGameEventObject.Client);
			}
			else if (eventType == 10)
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
			else if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
			else if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				this.OnLogout(playerLogoutEventObject.getPlayer());
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
			{
			case 29:
			{
				OnClientChangeMapEventObject onClientChangeMapEventObject = eventObject as OnClientChangeMapEventObject;
				if (null != onClientChangeMapEventObject)
				{
					onClientChangeMapEventObject.Result = this.ClientChangeMap(onClientChangeMapEventObject.Client, onClientChangeMapEventObject.TeleportID, ref onClientChangeMapEventObject.ToMapCode, ref onClientChangeMapEventObject.ToPosX, ref onClientChangeMapEventObject.ToPosY);
					onClientChangeMapEventObject.Handled = true;
				}
				break;
			}
			case 30:
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					CompMineScene compMineScene = null;
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(onCreateMonsterEventObject.Monster.CopyMapID);
					if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out compMineScene))
					{
						CompMineTruckConfig compMineTruckConfig = onCreateMonsterEventObject.Monster.Tag as CompMineTruckConfig;
						if (null != compMineTruckConfig)
						{
							onCreateMonsterEventObject.Monster.Camp = compMineScene.GameId;
							onCreateMonsterEventObject.Result = true;
							onCreateMonsterEventObject.Handled = true;
						}
					}
				}
				break;
			}
			default:
				if (num == 34)
				{
					AfterMonsterInjureEventObject afterMonsterInjureEventObject = eventObject as AfterMonsterInjureEventObject;
					if (afterMonsterInjureEventObject != null && afterMonsterInjureEventObject.SceneType == 53)
					{
						Monster monster = afterMonsterInjureEventObject.Monster;
						if (monster != null)
						{
							CompMineTruckConfig compMineTruckConfig = afterMonsterInjureEventObject.Monster.Tag as CompMineTruckConfig;
							if (null != compMineTruckConfig)
							{
								bool flag = false;
								CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(afterMonsterInjureEventObject.Monster.CopyMapID);
								CompMineScene compMineScene = null;
								if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out compMineScene) && compMineScene.m_eStatus == 2)
								{
									flag = true;
								}
								if (flag)
								{
									afterMonsterInjureEventObject.Injure = Math.Min(afterMonsterInjureEventObject.Injure + afterMonsterInjureEventObject.MerlinInjure, compMineTruckConfig.MaxHurt);
									afterMonsterInjureEventObject.MerlinInjure = 0;
								}
								else
								{
									afterMonsterInjureEventObject.Injure = 0;
									afterMonsterInjureEventObject.MerlinInjure = 0;
								}
								eventObject.Handled = true;
								eventObject.Result = true;
							}
						}
					}
				}
				break;
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
					this.RuntimeData.CompMineConfigDict.Clear();
					text = "Config/CompMineWar.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompMineConfig compMineConfig = new CompMineConfig();
						compMineConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compMineConfig.Name = Global.GetSafeAttributeStr(xml, "Name");
						compMineConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						compMineConfig.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
						compMineConfig.EnterCD = (int)Global.GetSafeAttributeLong(xml, "EnterCD");
						compMineConfig.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						compMineConfig.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						compMineConfig.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						compMineConfig.ExpNum = (int)Global.GetSafeAttributeLong(xml, "Exp");
						compMineConfig.BandJinBiNum = (int)Global.GetSafeAttributeLong(xml, "BandJinBi");
						if (!ConfigParser.ParserTimeRangeListWithDay(compMineConfig.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < compMineConfig.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(compMineConfig.TimePoints[i].Hours, compMineConfig.TimePoints[i].Minutes, compMineConfig.TimePoints[i].Seconds);
							compMineConfig.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						this.RuntimeData.CompMineConfigDict[compMineConfig.ID] = compMineConfig;
					}
					this.RuntimeData.CompMineTruckConfigList.Clear();
					text = "Config/CompMineTruck.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompMineTruckConfig compMineTruckConfig = new CompMineTruckConfig();
						compMineTruckConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compMineTruckConfig.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
						compMineTruckConfig.TimePoints = (int)Global.GetSafeAttributeLong(xml, "TimePoints");
						compMineTruckConfig.Speed = Global.GetSafeAttributeDouble(xml, "Speed");
						compMineTruckConfig.MaxHurt = (int)Global.GetSafeAttributeLong(xml, "MaxHurt");
						compMineTruckConfig.AddLIfe = Global.GetSafeAttributeIntArray(xml, "AddLIfe", -1, '|');
						compMineTruckConfig.FinishNum = Global.GetSafeAttributeDoubleArray(xml, "FinishNum", -1, '|');
						compMineTruckConfig.BrokenNum = Global.GetSafeAttributeDoubleArray(xml, "BrokenNum", -1, '|');
						compMineTruckConfig.CompBoomNum = (int)Global.GetSafeAttributeLong(xml, "CompNum");
						compMineTruckConfig.CompMineNum = (int)Global.GetSafeAttributeLong(xml, "CompMineNum");
						this.RuntimeData.CompMineTruckConfigList.Add(compMineTruckConfig);
					}
					this.RuntimeData.CompMineLinkConfigList.Clear();
					text = "Config/CompMineLink.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompMineLinkConfig compMineLinkConfig = new CompMineLinkConfig();
						compMineLinkConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						string[] array = Global.GetSafeAttributeStr(xml, "TargetSide").Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							compMineLinkConfig.PosX = Global.SafeConvertToInt32(array[0]);
							compMineLinkConfig.PosY = Global.SafeConvertToInt32(array[1]);
						}
						compMineLinkConfig.Supplies = (Global.GetSafeAttributeLong(xml, "Supplies") > 0L);
						compMineLinkConfig.End = (Global.GetSafeAttributeLong(xml, "End") > 0L);
						this.RuntimeData.CompMineLinkConfigList.Add(compMineLinkConfig);
					}
					this.RuntimeData.CompMineBirthConfigDict.Clear();
					text = "Config/CompMineBirthPoint.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompMineBirthConfig compMineBirthConfig = new CompMineBirthConfig();
						compMineBirthConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compMineBirthConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						string[] array = Global.GetSafeAttributeStr(xml, "JiaoTuanBirth").Split(new char[]
						{
							'|'
						});
						if (array.Length == 3)
						{
							CMBirthPoint cmbirthPoint = new CMBirthPoint();
							cmbirthPoint.PosX = Global.SafeConvertToInt32(array[0]);
							cmbirthPoint.PosY = Global.SafeConvertToInt32(array[1]);
							cmbirthPoint.BirthRadius = Global.SafeConvertToInt32(array[2]);
							compMineBirthConfig.BirthPoints[0] = cmbirthPoint;
						}
						array = Global.GetSafeAttributeStr(xml, "MengJunBirth").Split(new char[]
						{
							'|'
						});
						if (array.Length == 3)
						{
							CMBirthPoint cmbirthPoint = new CMBirthPoint();
							cmbirthPoint.PosX = Global.SafeConvertToInt32(array[0]);
							cmbirthPoint.PosY = Global.SafeConvertToInt32(array[1]);
							cmbirthPoint.BirthRadius = Global.SafeConvertToInt32(array[2]);
							compMineBirthConfig.BirthPoints[1] = cmbirthPoint;
						}
						array = Global.GetSafeAttributeStr(xml, "XieHuiBirth").Split(new char[]
						{
							'|'
						});
						if (array.Length == 3)
						{
							CMBirthPoint cmbirthPoint = new CMBirthPoint();
							cmbirthPoint.PosX = Global.SafeConvertToInt32(array[0]);
							cmbirthPoint.PosY = Global.SafeConvertToInt32(array[1]);
							cmbirthPoint.BirthRadius = Global.SafeConvertToInt32(array[2]);
							compMineBirthConfig.BirthPoints[2] = cmbirthPoint;
						}
						this.RuntimeData.CompMineBirthConfigDict[compMineBirthConfig.MapCode] = compMineBirthConfig;
					}
					this.RuntimeData.CompMineRewardConfigList.Clear();
					text = "Config/CompMineAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompMineRewardConfig compMineRewardConfig = new CompMineRewardConfig();
						compMineRewardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compMineRewardConfig.Rank = (int)Global.GetSafeAttributeLong(xml, "Rank");
						compMineRewardConfig.RankRate = Global.GetSafeAttributeDouble(xml, "RankRate");
						compMineRewardConfig.Grade = (int)Global.GetSafeAttributeLong(xml, "CompFeast");
						compMineRewardConfig.Contribution = (int)Global.GetSafeAttributeLong(xml, "CompHonor");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsOne"), ref compMineRewardConfig.AwardsItemListOne, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsTwo"), ref compMineRewardConfig.AwardsItemListTwo, '|', ',');
						this.RuntimeData.CompMineRewardConfigList.Add(compMineRewardConfig);
					}
					this.RuntimeData.CompMineAttackKill = GameManager.systemParamsList.GetParamValueIntArrayByName("CompMineAttackKill", ',');
					this.RuntimeData.CompMineAttackShutDown = GameManager.systemParamsList.GetParamValueIntArrayByName("CompMineAttackShutDown", ',');
					this.RuntimeData.CompMineDie = (int)GameManager.systemParamsList.GetParamValueIntByName("CompMineDie", -1);
					this.RuntimeData.CompMineAwardNum = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CompMineAwardNum", '|');
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private void TimerProc(object sender, EventArgs e)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(null, ref compBattleGameStates);
				if (0 != compBattleGameStates)
				{
					foreach (CompMineScene compMineScene in this.SceneDict.Values)
					{
						CompFuBenData compFuBenData = null;
						if (this.RuntimeData.FuBenItemData.TryGetValue(compMineScene.GameId, out compFuBenData))
						{
							compFuBenData.Init();
							List<GameClient> clientsList = compMineScene.CopyMap.GetClientsList();
							if (clientsList != null && clientsList.Count > 0)
							{
								for (int i = 0; i < clientsList.Count; i++)
								{
									GameClient gameClient = clientsList[i];
									if (gameClient != null)
									{
										int battleWhichSide = gameClient.ClientData.BattleWhichSide;
										if (battleWhichSide > 0 && battleWhichSide <= compFuBenData.RoleCountSideList.Count)
										{
											List<int> roleCountSideList;
											int index;
											(roleCountSideList = compFuBenData.RoleCountSideList)[index = battleWhichSide - 1] = roleCountSideList[index] + 1;
											if (gameClient.ClientData.CompZhiWu > 0)
											{
												compFuBenData.ZhuJiangRoleDict[gameClient.ClientData.CompType].Add(gameClient.ClientData.RoleID);
											}
										}
									}
								}
							}
							compFuBenData.MineTruckGo = compMineScene.ScoreData.MineTruckGo;
							compFuBenData.MineSafeArrived = compMineScene.ScoreData.SafeArrived;
							TianTiClient.getInstance().Comp_UpdateKuaFuMapClientCount(31, compFuBenData);
						}
					}
				}
			}
		}

		public bool ProcessGetCompMineBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(client, ref compBattleGameStates);
				List<CompMineBaseData> list = new List<CompMineBaseData>();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 1; i <= 3; i++)
					{
						CompMineBaseData compMineBaseData = new CompMineBaseData();
						if (0 != compBattleGameStates)
						{
							CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, i);
							if (null != compFuBenData)
							{
								compMineBaseData.MineTruckGo = compFuBenData.MineTruckGo;
								compMineBaseData.SafeArrived = compFuBenData.MineSafeArrived;
							}
						}
						list.Add(compMineBaseData);
					}
				}
				client.sendCmd<List<CompMineBaseData>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompMineEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				int num3 = 0;
				CompBattleGameStates compBattleGameStates = 0;
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				if (!this.CheckMap(client))
				{
					num3 = -21;
				}
				else
				{
					num3 = this.CheckCondition(client, ref compBattleGameStates);
					if (compBattleGameStates != 1)
					{
						num3 = -2001;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					CompMineConfig compMineConfig = null;
					if (!this.RuntimeData.CompMineConfigDict.TryGetValue(num2, out compMineConfig))
					{
						num3 = -3;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					DateTime roleParamsDateTimeFromDB = Global.GetRoleParamsDateTimeFromDB(client, "20022");
					if (TimeUtil.NowDateTime().Ticks - roleParamsDateTimeFromDB.Ticks < 10000000L * (long)compMineConfig.EnterCD)
					{
						GameManager.ClientMgr.NotifyImportantMsg(client, string.Format(GLang.GetLang(2615, new object[0]), compMineConfig.EnterCD), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						num3 = -2007;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					KuaFuServerInfo kuaFuServerInfo = null;
					CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, num2);
					if (compFuBenData == null || !KuaFuManager.getInstance().TryGetValue(compFuBenData.ServerId, out kuaFuServerInfo))
					{
						num3 = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					if (compFuBenData.GetRoleCountWithEnter(client.ClientData.CompType) >= compMineConfig.MaxEnterNum)
					{
						num3 = -22;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
					if (null != clientKuaFuServerLoginData)
					{
						clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
						clientKuaFuServerLoginData.GameId = (long)compFuBenData.GameId;
						clientKuaFuServerLoginData.GameType = 31;
						clientKuaFuServerLoginData.EndTicks = compFuBenData.EndTime.Ticks;
						clientKuaFuServerLoginData.ServerId = client.ServerId;
						clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
						clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
						clientKuaFuServerLoginData.FuBenSeqId = 0;
					}
				}
				if (num3 >= 0)
				{
					num3 = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, num2, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 4);
					if (num3 >= 0)
					{
						GlobalNew.RecordSwitchKuaFuServerLog(client);
						client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
					}
					else
					{
						Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					}
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompMineAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == kfcompRoleData)
				{
					return true;
				}
				CompMineAwardsData compMineAwardsData = new CompMineAwardsData();
				if (kfcompRoleData.MineJiFen > 0)
				{
					compMineAwardsData.RankNum = kfcompRoleData.MineRankNum;
					CompMineRewardConfig compMineRewardConfig = this.CalMineRewardConfig(client, kfcompRoleData);
					compMineAwardsData.AwardID = ((compMineRewardConfig != null) ? compMineRewardConfig.ID : 0);
				}
				client.sendCmd<CompMineAwardsData>(nID, compMineAwardsData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompMineStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(client, ref compBattleGameStates);
				int num = compBattleGameStates;
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (num == 0 && kfcompRoleData != null && kfcompRoleData.CompType == kfcompRoleData.CompTypeMine)
				{
					string roleParamByName = Global.GetRoleParamByName(client, "150");
					if (!string.IsNullOrEmpty(roleParamByName))
					{
						long num2 = Global.SafeConvertToInt64(roleParamByName);
						if (TimeUtil.NOW() - num2 < 604800000L)
						{
							client.sendCmd<int>(nID, 2, false);
							return true;
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
			return true;
		}

		public bool ProcessGetCompMineSelfScoreCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompMineScene compMineScene = client.SceneObject as CompMineScene;
				if (null == compMineScene)
				{
					return true;
				}
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == kfcompRoleData)
				{
					return true;
				}
				CompMineSelfScore compMineSelfScore = new CompMineSelfScore();
				CompMineRewardConfig compMineRewardConfig = this.CalMineRewardConfig(client, kfcompRoleData);
				compMineSelfScore.RankNum = kfcompRoleData.MineRankNum;
				compMineSelfScore.AwardID = ((compMineRewardConfig != null) ? compMineRewardConfig.ID : 0);
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					CompManager.getInstance().CompSyncDataCache.CompRankMineJiFenDict.V.TryGetValue(client.ClientData.CompType, out compMineSelfScore.rankInfo2Client);
				}
				client.sendCmd<CompMineSelfScore>(nID, compMineSelfScore, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompMineAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(client, ref compBattleGameStates);
				if (0 != compBattleGameStates)
				{
					int num = -2001;
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				string roleParamByName = Global.GetRoleParamByName(client, "150");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					bool flag = true;
					long num2 = Global.SafeConvertToInt64(roleParamByName);
					int num;
					if (TimeUtil.NOW() - num2 < 604800000L)
					{
						num = this.GiveRoleAwards(client, num2);
						if (num < 0)
						{
							flag = false;
						}
					}
					else
					{
						num = -5;
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "150", "", true);
					}
					client.sendCmd<int>(nID, num, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public void OnCompMineReset()
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient != null && gameClient.ClientData.CompType > 0)
				{
					int compMineJiFenValue = GameManager.ClientMgr.GetCompMineJiFenValue(gameClient);
					if (compMineJiFenValue > 0)
					{
						GameManager.ClientMgr.ModifyCompMineJiFenValue(gameClient, -compMineJiFenValue, "势力矿洞KF", true, true, false);
					}
				}
			}
		}

		public bool IsCompMineMap(int mapCodeID)
		{
			bool result;
			lock (this.RuntimeData.Mutex)
			{
				foreach (KeyValuePair<int, CompMineConfig> keyValuePair in this.RuntimeData.CompMineConfigDict)
				{
					if (keyValuePair.Value.MapCode == mapCodeID)
					{
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == null || mapSceneType == 48;
		}

		public int CheckCondition(GameClient client, ref CompBattleGameStates state)
		{
			int result = 0;
			CompMineConfig compMineConfig = null;
			if (client != null && !this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					compMineConfig = this.RuntimeData.CompMineConfigDict.Values.FirstOrDefault<CompMineConfig>();
					if (null == compMineConfig)
					{
						return -12;
					}
				}
				result = -2001;
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < compMineConfig.TimePoints.Count - 1; i += 2)
					{
						if (dateTime.DayOfWeek == (DayOfWeek)compMineConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= compMineConfig.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= compMineConfig.SecondsOfDay[i + 1])
						{
							if (dateTime.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i + 1] - (double)compMineConfig.ClearRolesSecs)
							{
								state = 1;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < compMineConfig.SecondsOfDay[i + 1])
							{
								state = 4;
								result = 1;
							}
							else
							{
								state = 0;
								result = -2001;
							}
							break;
						}
					}
				}
			}
			return result;
		}

		private TimeSpan GetStartTime(int sceneId)
		{
			CompMineConfig compMineConfig = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompMineConfigDict.TryGetValue(sceneId, out compMineConfig))
				{
					goto IL_14B;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < compMineConfig.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)compMineConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= compMineConfig.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= compMineConfig.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(compMineConfig.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_14B:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, (int)kuaFuServerLoginData.GameId);
			bool result;
			if (compFuBenData == null || compFuBenData.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineConfig compMineConfig;
					if (!this.RuntimeData.CompMineConfigDict.TryGetValue(compFuBenData.GameId, out compMineConfig) || (long)compMineConfig.ID != kuaFuServerLoginData.GameId)
					{
						LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
						return false;
					}
				}
				result = true;
			}
			return result;
		}

		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			CompFuBenData compFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out compFuBenData))
				{
					compFuBenData = null;
				}
				else if (compFuBenData.State >= 3)
				{
					return false;
				}
			}
			if (null == compFuBenData)
			{
				CompFuBenData compFuBenData2 = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(31, (int)clientKuaFuServerLoginData.GameId);
				if (compFuBenData2 == null || compFuBenData2.State == 3)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + compFuBenData2 == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out compFuBenData))
					{
						compFuBenData = compFuBenData2;
						compFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						compFuBenData.Init();
						this.RuntimeData.FuBenItemData[compFuBenData.GameId] = compFuBenData;
					}
				}
			}
			CompMineConfig compMineConfig;
			lock (this.RuntimeData.Mutex)
			{
				clientKuaFuServerLoginData.FuBenSeqId = compFuBenData.SequenceId;
				if (!this.RuntimeData.CompMineConfigDict.TryGetValue(compFuBenData.GameId, out compMineConfig))
				{
					return false;
				}
			}
			client.ClientData.BattleWhichSide = client.ClientData.CompType;
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(compMineConfig.MapCode, client, out posX, out posY);
			bool result;
			if (birthPoint <= 0)
			{
				LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + birthPoint, null, true);
				result = false;
			}
			else
			{
				client.ClientData.MapCode = compMineConfig.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				Global.SaveRoleParamsDateTimeToDB(client, "20022", TimeUtil.NowDateTime(), true);
				result = true;
			}
			return result;
		}

		private void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client.ClientData.MapCode, client, out posX, out posY);
			bool result;
			if (birthPoint <= 0)
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
			return result;
		}

		public int GetBirthPoint(int mapCode, GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				CompMineBirthConfig compMineBirthConfig = null;
				if (this.RuntimeData.CompMineBirthConfigDict.TryGetValue(mapCode, out compMineBirthConfig))
				{
					CMBirthPoint cmbirthPoint = compMineBirthConfig.BirthPoints[battleWhichSide - 1];
					CompMineScene compMineScene = client.SceneObject as CompMineScene;
					if (null != compMineScene)
					{
						Point mapPointByGridXY = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, cmbirthPoint.PosX / compMineScene.MapGridWidth, cmbirthPoint.PosY / compMineScene.MapGridHeight, cmbirthPoint.BirthRadius / compMineScene.MapGridWidth, 0, false);
						posX = (int)mapPointByGridXY.X;
						posY = (int)mapPointByGridXY.Y;
						return battleWhichSide;
					}
					posX = cmbirthPoint.PosX;
					posY = cmbirthPoint.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public void UpdateCompMineResourceData(Dictionary<int, KFCompData> CompDataDict)
		{
			if (null != CompDataDict)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (CompMineScene compMineScene in this.SceneDict.Values)
					{
						for (int i = 0; i < compMineScene.ScoreData.ResJiFenList.Count; i++)
						{
							CompMineResData compMineResData = compMineScene.ScoreData.ResJiFenList[i];
							KFCompData kfcompData;
							if (CompDataDict.TryGetValue(compMineResData.CompType, out kfcompData))
							{
								compMineResData.MineRes = kfcompData.MineRes;
								compMineResData.Rank = kfcompData.MineRank;
							}
						}
						this.UpdateBattleSideScoreRank(compMineScene, true);
					}
				}
			}
		}

		public void UpdateBattleSideScoreRank(CompMineScene scene, bool sync = true)
		{
			try
			{
				if (scene.ScoreData.ResJiFenList.Count != 3)
				{
					LogManager.WriteLog(2, string.Format("势力矿洞分数信息异常 CityID={0}", scene.GameId), null, true);
				}
				else
				{
					scene.ScoreData.ResJiFenList.Sort(delegate(CompMineResData left, CompMineResData right)
					{
						int result;
						if (left.MineRes > right.MineRes)
						{
							result = -1;
						}
						else if (left.MineRes < right.MineRes)
						{
							result = 1;
						}
						else if (left.Rank < right.Rank)
						{
							result = -1;
						}
						else if (left.Rank > right.Rank)
						{
							result = 1;
						}
						else if (left.CompType < right.CompType)
						{
							result = -1;
						}
						else if (left.CompType > right.CompType)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int i = 0; i < scene.ScoreData.ResJiFenList.Count; i++)
					{
						CompMineResData compMineResData = scene.ScoreData.ResJiFenList[i];
						compMineResData.Rank = i + 1;
					}
					if (sync)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		private void InitScene(CompMineScene scene, Dictionary<int, KFCompData> compDataDict)
		{
			for (int i = 1; i <= 3; i++)
			{
				CompMineResData compMineResData = new CompMineResData();
				compMineResData.CompType = i;
				KFCompData kfcompData = null;
				if (compDataDict.TryGetValue(i, out kfcompData))
				{
					compMineResData.Rank = kfcompData.MineRank;
					compMineResData.MineRes = kfcompData.MineRes;
				}
				scene.ScoreData.ResJiFenList.Add(compMineResData);
			}
			this.UpdateBattleSideScoreRank(scene, false);
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 53)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					Dictionary<int, KFCompData> compDataDict = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						compDataDict = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
					}
					int fuBenSeqID = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleID = client.ClientData.RoleID;
					int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						CompMineScene compMineScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out compMineScene))
						{
							CompMineConfig compMineConfig = null;
							CompFuBenData compFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out compFuBenData))
							{
								LogManager.WriteLog(2, "势力矿洞没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
							}
							if (!this.RuntimeData.CompMineConfigDict.TryGetValue(compFuBenData.GameId, out compMineConfig))
							{
								LogManager.WriteLog(2, "势力矿洞没有为副本找到对应的档位数据,ID:" + compFuBenData.GameId, null, true);
							}
							compMineScene = new CompMineScene();
							compMineScene.CopyMap = copyMap;
							compMineScene.CleanAllInfo();
							compMineScene.GameId = num;
							compMineScene.m_nMapCode = mapCode;
							compMineScene.CopyMapId = copyMap.CopyMapID;
							compMineScene.FuBenSeqId = fuBenSeqID;
							compMineScene.SceneInfo = compMineConfig;
							compMineScene.MapGridWidth = gameMap.MapGridWidth;
							compMineScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(compMineConfig.ID));
							compMineScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(compMineScene, compDataDict);
							this.SceneDict[fuBenSeqID] = compMineScene;
						}
						CompMineClientContextData compMineClientContextData;
						if (!compMineScene.ClientContextDataDict.TryGetValue(roleID, out compMineClientContextData))
						{
							compMineClientContextData = new CompMineClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							compMineScene.ClientContextDataDict[roleID] = compMineClientContextData;
						}
						else
						{
							compMineClientContextData.KillNum = 0;
						}
						client.SceneObject = compMineScene;
						client.SceneGameId = (long)compMineScene.GameId;
						client.SceneContextData2 = compMineClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(compMineScene.SceneInfo.TotalSecs * 1000));
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, compMineScene.SceneInfo.ID, roleID, (int)client.ClientData.CompZhiWu, 5);
					}
					result = true;
				}
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
			if (sceneType == 53)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineScene compMineScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out compMineScene);
					this.RuntimeData.FuBenItemData.Remove(compMineScene.GameId);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			CompMineTruckConfig compMineTruckConfig = monster.Tag as CompMineTruckConfig;
			if (null != compMineTruckConfig)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompMineScene compMineScene = client.SceneObject as CompMineScene;
					if (compMineScene != null && compMineScene.m_eStatus == 2)
					{
						CompMineClientContextData compMineClientContextData = client.SceneContextData2 as CompMineClientContextData;
						if (null != compMineClientContextData)
						{
							compMineClientContextData.TruckInjure += injure;
						}
					}
				}
			}
		}

		private void ProcessEnd(CompMineScene scene, long nowTicks)
		{
			scene.m_eStatus = 3;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 31;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void GiveAwards(CompMineScene scene)
		{
			try
			{
				foreach (CompMineClientContextData compMineClientContextData in scene.ClientContextDataDict.Values)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(compMineClientContextData.RoleId);
					if (null != gameClient)
					{
						long expMultiByZhuanShengExpXiShu = Global.GetExpMultiByZhuanShengExpXiShu(gameClient, (long)scene.SceneInfo.ExpNum);
						int bandJinBiNum = scene.SceneInfo.BandJinBiNum;
						if (expMultiByZhuanShengExpXiShu > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(gameClient, expMultiByZhuanShengExpXiShu, true, true, false, "none");
						}
						if (bandJinBiNum > 0)
						{
							GameManager.ClientMgr.AddMoney1(gameClient, bandJinBiNum, "势力矿洞基础奖励", true);
						}
					}
					if (compMineClientContextData.BattleJiFen > 0)
					{
						string text = string.Format("{0}", scene.StartTimeTicks);
						if (gameClient != null)
						{
							Global.SaveRoleParamsStringToDB(gameClient, "150", text, true);
						}
						else
						{
							Global.UpdateRoleParamByNameOffline(compMineClientContextData.RoleId, "150", text, compMineClientContextData.ServerId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "势力矿洞系统奖励异常");
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene compMineScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compMineScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, compMineScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<CompMineSideScore>(2014, compMineScene.ScoreData, false);
					}
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene compMineScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compMineScene))
				{
					CompFuBenData compFuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(compMineScene.GameId, out compFuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompMineClientContextData compMineClientContextData = client.SceneContextData2 as CompMineClientContextData;
							compFuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Add(client.ClientData.RoleID);
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, compMineScene.ScoreData, compMineScene.CopyMap);
						}
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = compFuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] + 1;
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene compMineScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compMineScene))
				{
					CompFuBenData compFuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(compMineScene.GameId, out compFuBenData))
					{
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = compFuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] - 1;
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, client.ServerId, compMineScene.SceneInfo.ID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 7);
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineScene compMineScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compMineScene))
				{
					if (compMineScene.m_eStatus == 2)
					{
						int num = 0;
						int compMineDie = this.RuntimeData.CompMineDie;
						CompMineClientContextData compMineClientContextData = client.SceneContextData2 as CompMineClientContextData;
						CompMineClientContextData compMineClientContextData2 = other.SceneContextData2 as CompMineClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						if (compMineScene.GameId == compMineClientContextData.BattleWhichSide || compMineScene.GameId == compMineClientContextData2.BattleWhichSide)
						{
							huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
							huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
							huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
							huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
							if (null != compMineClientContextData)
							{
								compMineClientContextData.KillNum++;
								if (compMineScene.GameId == compMineClientContextData.BattleWhichSide)
								{
									compMineClientContextData.KillNumLocal++;
								}
								int num2 = this.RuntimeData.CompMineAttackKill[0] + compMineClientContextData.KillNum * this.RuntimeData.CompMineAttackKill[1];
								num2 = Math.Min(this.RuntimeData.CompMineAttackKill[3], Math.Max(this.RuntimeData.CompMineAttackKill[2], num2));
								huanYingSiYuanAddScore.ByLianShaNum = 1;
								huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
								huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
								huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
								huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
								huanYingSiYuanLianSha.LianShaType = Math.Min(compMineClientContextData.KillNum, 30) / 5;
								huanYingSiYuanLianSha.ExtScore = num2;
								huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
								num += num2;
								if (compMineClientContextData.KillNum % 5 != 0)
								{
									huanYingSiYuanLianSha = null;
								}
							}
							if (null != compMineClientContextData2)
							{
								int num3 = this.RuntimeData.CompMineAttackShutDown[0] + compMineClientContextData2.KillNum * this.RuntimeData.CompMineAttackShutDown[1];
								num3 = Math.Min(this.RuntimeData.CompMineAttackShutDown[3], Math.Max(this.RuntimeData.CompMineAttackShutDown[2], num3));
								num += num3;
								if (compMineClientContextData2.KillNum >= 10)
								{
									huanYingSiYuanLianshaOver = new HuanYingSiYuanLianshaOver();
									huanYingSiYuanLianshaOver.KillerName = huanYingSiYuanAddScore.Name;
									huanYingSiYuanLianshaOver.KillerZoneID = huanYingSiYuanAddScore.ZoneID;
									huanYingSiYuanLianshaOver.KillerOccupation = client.ClientData.Occupation;
									huanYingSiYuanLianshaOver.KillerSide = huanYingSiYuanAddScore.Side;
									huanYingSiYuanLianshaOver.KilledName = Global.FormatRoleName4(other);
									huanYingSiYuanLianshaOver.KilledZoneID = other.ClientData.ZoneID;
									huanYingSiYuanLianshaOver.KilledOccupation = other.ClientData.Occupation;
									huanYingSiYuanLianshaOver.KilledSide = other.ClientData.BattleWhichSide;
									huanYingSiYuanLianshaOver.ExtScore = num3;
								}
								compMineClientContextData2.KillNum = 0;
							}
						}
						if (num > 0)
						{
							compMineClientContextData.BattleJiFen += num;
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 10, client.ClientData.RoleID, num);
							GameManager.ClientMgr.ModifyCompMineJiFenValue(client, num, "杀人", true, true, false);
						}
						if (compMineDie > 0)
						{
							compMineClientContextData2.BattleJiFen += compMineDie;
							TianTiClient.getInstance().Comp_CompOpt(other.ClientData.CompType, 10, other.ClientData.RoleID, compMineDie);
							GameManager.ClientMgr.ModifyCompMineJiFenValue(other, compMineDie, "被杀", true, true, false);
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2016, huanYingSiYuanLianSha, compMineScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2017, huanYingSiYuanLianshaOver, compMineScene.CopyMap);
						}
					}
				}
			}
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompMineScene compMineScene = client.SceneObject as CompMineScene;
			CompMineTruckConfig compMineTruckConfig = monster.Tag as CompMineTruckConfig;
			if (compMineTruckConfig != null && null != compMineScene)
			{
				Dictionary<int, CompConfig> dictionary = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					dictionary = CompManager.getInstance().RuntimeData.CompConfigDict;
				}
				lock (this.RuntimeData.Mutex)
				{
					long num = 0L;
					long[] array = new long[3];
					foreach (KeyValuePair<int, CompMineClientContextData> keyValuePair in compMineScene.ClientContextDataDict)
					{
						CompMineClientContextData value = keyValuePair.Value;
						GameClient gameClient = GameManager.ClientMgr.FindClient(value.RoleId);
						if (null != gameClient)
						{
							if (value.BattleWhichSide != compMineScene.GameId)
							{
								int num2 = (int)((double)value.TruckInjure / compMineTruckConfig.BrokenNum[0]);
								num2 = Math.Max(num2, (int)compMineTruckConfig.BrokenNum[1]);
								num2 = Math.Min(num2, (int)compMineTruckConfig.BrokenNum[2]);
								if (num2 > 0)
								{
									value.BattleJiFen += num2;
									TianTiClient.getInstance().Comp_CompOpt(gameClient.ClientData.CompType, 10, gameClient.ClientData.RoleID, num2);
									GameManager.ClientMgr.ModifyCompMineJiFenValue(gameClient, num2, "矿车击碎", true, true, false);
								}
							}
						}
					}
					foreach (KeyValuePair<int, CompMineClientContextData> keyValuePair in compMineScene.ClientContextDataDict)
					{
						CompMineClientContextData value = keyValuePair.Value;
						num += value.TruckInjure;
						array[value.BattleWhichSide - 1] += value.TruckInjure;
						value.TruckInjure = 0L;
					}
					for (int i = 1; i <= 3; i++)
					{
						if (i != compMineScene.GameId && array[i - 1] > 0L)
						{
							int num3 = (int)((double)compMineTruckConfig.CompMineNum * (double)array[i - 1] / (double)num);
							if (num3 > 0)
							{
								TianTiClient.getInstance().Comp_CompOpt(i, 9, num3, 0);
								CompConfig compConfig;
								if (dictionary.TryGetValue(i, out compConfig))
								{
									string msg = string.Format(GLang.GetLang(6002, new object[0]), compConfig.CompName, num3);
									this.BroadCastImportantMsg(compMineScene, 0, msg);
								}
							}
							int num4 = (int)((double)compMineTruckConfig.CompBoomNum * (double)array[i - 1] / (double)num);
							if (num4 > 0)
							{
								TianTiClient.getInstance().Comp_CompOpt(i, 0, num4, 0);
								string msg = string.Format(GLang.GetLang(4018, new object[0]), num4);
								this.BroadCastImportantMsg(compMineScene, i, msg);
							}
						}
					}
					compMineScene.ScoreData.MineTruckProcess = 0;
					compMineScene.ScoreData.SuppliesNum = 0;
					compMineScene.ScoreData.SuppliesStep = 0;
					this.UpdateBattleSideScoreRank(compMineScene, true);
					if (compMineScene.ScoreData.MineTruckGo < this.RuntimeData.CompMineTruckConfigList.Count)
					{
						CompMineTruckConfig compMineTruckConfig2 = this.RuntimeData.CompMineTruckConfigList[compMineScene.ScoreData.MineTruckGo];
						long num5 = TimeUtil.NOW() + (long)(compMineTruckConfig2.TimePoints * 1000);
						if (num5 < compMineScene.m_lEndTime)
						{
							this.AddDelayCreateMonster(compMineScene, num5, compMineTruckConfig2);
						}
					}
				}
			}
		}

		public void AddDelayCreateMonster(CompMineScene scene, long ticks, object monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				List<object> list = null;
				if (!scene.CreateMonsterQueue.TryGetValue(ticks, out list))
				{
					list = new List<object>();
					scene.CreateMonsterQueue.Add(ticks, list);
				}
				list.Add(monster);
			}
		}

		public void CheckCreateDynamicMonster(CompMineScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				while (scene.CreateMonsterQueue.Count > 0)
				{
					KeyValuePair<long, List<object>> keyValuePair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
					if (nowMs < keyValuePair.Key)
					{
						break;
					}
					try
					{
						foreach (object obj in keyValuePair.Value)
						{
							if (obj is CompMineTruckConfig)
							{
								CompMineLinkConfig compMineLinkConfig = this.RuntimeData.CompMineLinkConfigList[0];
								CompMineTruckConfig compMineTruckConfig = obj as CompMineTruckConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, compMineTruckConfig.MonsterID, scene.CopyMapId, 1, compMineLinkConfig.PosX / 100, compMineLinkConfig.PosY / 100, 0, 0, 53, compMineTruckConfig, null);
								scene.ScoreData.MineTruckProcess = 0;
								scene.ScoreData.MineTruckGo++;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, scene.ScoreData, scene.CopyMap);
							}
						}
					}
					finally
					{
						scene.CreateMonsterQueue.RemoveAt(0);
					}
				}
			}
		}

		private void InitCreateDynamicMonster(CompMineScene scene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompMineTruckConfig compMineTruckConfig = this.RuntimeData.CompMineTruckConfigList[0];
				long ticks = scene.m_lBeginTime + (long)(compMineTruckConfig.TimePoints * 1000);
				this.AddDelayCreateMonster(scene, ticks, compMineTruckConfig);
			}
		}

		public bool ClientChangeMap(GameClient client, int teleportID, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			CompMineScene compMineScene = client.SceneObject as CompMineScene;
			bool result;
			if (null == compMineScene)
			{
				result = false;
			}
			else if (compMineScene.GameId != client.ClientData.CompType)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(6004, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else if (compMineScene.m_eStatus == 1)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(6000, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public void MonsterMoveStep(Monster monster)
		{
			long num = TimeUtil.NOW();
			CompMineTruckConfig compMineTruckConfig = monster.Tag as CompMineTruckConfig;
			if (null != compMineTruckConfig)
			{
				if ((double)(num - monster.MoveTime) >= compMineTruckConfig.Speed * 500.0)
				{
					Dictionary<int, CompConfig> dictionary = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						dictionary = CompManager.getInstance().RuntimeData.CompConfigDict;
					}
					lock (this.RuntimeData.Mutex)
					{
						CompMineScene compMineScene = null;
						CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(monster.CopyMapID);
						if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out compMineScene))
						{
							int step = monster.Step;
							int num2 = this.RuntimeData.CompMineLinkConfigList.Count<CompMineLinkConfig>() - 1;
							int num3 = step + 1;
							int mineTruckProcess = compMineScene.ScoreData.MineTruckProcess;
							compMineScene.ScoreData.MineTruckProcess = (int)((double)num3 / (double)num2 * 100.0);
							if (num3 >= num2)
							{
								compMineScene.ScoreData.SafeArrived++;
								compMineScene.ScoreData.MineTruckProcess = 0;
								compMineScene.ScoreData.SuppliesNum = 0;
								compMineScene.ScoreData.SuppliesStep = 0;
								foreach (KeyValuePair<int, CompMineClientContextData> keyValuePair in compMineScene.ClientContextDataDict)
								{
									CompMineClientContextData value = keyValuePair.Value;
									GameClient gameClient = GameManager.ClientMgr.FindClient(value.RoleId);
									if (null != gameClient)
									{
										if (value.BattleWhichSide == compMineScene.GameId && gameClient.ClientData.MapCode == compMineScene.m_nMapCode)
										{
											int num4 = (int)((double)value.KillNumLocal * compMineTruckConfig.FinishNum[0]);
											num4 = Math.Max(num4, (int)compMineTruckConfig.FinishNum[1]);
											num4 = Math.Min(num4, (int)compMineTruckConfig.FinishNum[2]);
											if (num4 > 0)
											{
												value.BattleJiFen += num4;
												TianTiClient.getInstance().Comp_CompOpt(gameClient.ClientData.CompType, 10, gameClient.ClientData.RoleID, num4);
												GameManager.ClientMgr.ModifyCompMineJiFenValue(gameClient, num4, "矿车到达", true, true, false);
											}
										}
									}
								}
								foreach (KeyValuePair<int, CompMineClientContextData> keyValuePair in compMineScene.ClientContextDataDict)
								{
									CompMineClientContextData value = keyValuePair.Value;
									value.TruckInjure = 0L;
								}
								TianTiClient.getInstance().Comp_CompOpt(compMineScene.GameId, 9, compMineTruckConfig.CompMineNum, 0);
								CompConfig compConfig;
								if (dictionary.TryGetValue(compMineScene.GameId, out compConfig))
								{
									string msg = string.Format(GLang.GetLang(6001, new object[0]), compConfig.CompName, compMineTruckConfig.CompMineNum);
									this.BroadCastImportantMsg(compMineScene, 0, msg);
								}
								if (compMineTruckConfig.CompBoomNum > 0)
								{
									TianTiClient.getInstance().Comp_CompOpt(compMineScene.GameId, 0, compMineTruckConfig.CompBoomNum, 0);
									string msg = string.Format(GLang.GetLang(4018, new object[0]), compMineTruckConfig.CompBoomNum);
									this.BroadCastImportantMsg(compMineScene, compMineScene.GameId, msg);
								}
								this.UpdateBattleSideScoreRank(compMineScene, true);
								GameManager.MonsterMgr.AddDelayDeadMonster(monster);
								monster.Tag = null;
								if (compMineScene.ScoreData.MineTruckGo < this.RuntimeData.CompMineTruckConfigList.Count)
								{
									CompMineTruckConfig compMineTruckConfig2 = this.RuntimeData.CompMineTruckConfigList[compMineScene.ScoreData.MineTruckGo];
									long num5 = num + (long)(compMineTruckConfig2.TimePoints * 1000);
									if (num5 < compMineScene.m_lEndTime)
									{
										this.AddDelayCreateMonster(compMineScene, num5, compMineTruckConfig2);
									}
								}
							}
							else
							{
								int posX = this.RuntimeData.CompMineLinkConfigList[num3].PosX;
								int posY = this.RuntimeData.CompMineLinkConfigList[num3].PosY;
								int num6 = posX / compMineScene.MapGridWidth;
								int num7 = posY / compMineScene.MapGridHeight;
								Point start = new Point((double)num6, (double)num7);
								Point currentGrid = monster.CurrentGrid;
								int currentX = (int)currentGrid.X;
								int currentY = (int)currentGrid.Y;
								double directionByAspect = Global.GetDirectionByAspect(num6, num7, currentX, currentY);
								ChuanQiUtils.WalkTo(monster, (Dircetions)directionByAspect);
								monster.MoveTime = num;
								if (Global.GetTwoPointDistance(start, currentGrid) < 2.0)
								{
									if (num3 < this.RuntimeData.CompMineLinkConfigList.Count && this.RuntimeData.CompMineLinkConfigList[num3].Supplies && compMineScene.ScoreData.SuppliesNum < compMineTruckConfig.AddLIfe.Length && num3 > compMineScene.ScoreData.SuppliesStep)
									{
										compMineScene.ScoreData.SuppliesStep = num3;
										double num8 = (double)compMineTruckConfig.AddLIfe[compMineScene.ScoreData.SuppliesNum];
										GameManager.MonsterMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, num8);
										LogManager.WriteLog(5, string.Format("势力矿洞跨服副本GameID={0}, 矿车Step={1}, 次数={2}, 回血={3}", new object[]
										{
											compMineScene.GameId,
											num3,
											compMineScene.ScoreData.SuppliesNum + 1,
											num8
										}), null, true);
										compMineScene.ScoreData.SuppliesNum++;
									}
									monster.Step = num3;
									LogManager.WriteLog(5, string.Format("势力矿洞跨服副本GameID={0}, 矿车Step={1}", compMineScene.GameId, num3), null, true);
								}
								if (mineTruckProcess != compMineScene.ScoreData.MineTruckProcess)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<CompMineSideScore>(2014, compMineScene.ScoreData, compMineScene.CopyMap);
								}
							}
						}
					}
				}
			}
		}

		private CompMineRewardConfig CalMineRewardConfig(GameClient client, KFCompRoleData compRoleData)
		{
			CompMineRewardConfig result = null;
			double num = 0.0;
			lock (CompManager.getInstance().RuntimeData.Mutex)
			{
				if (CompManager.getInstance().CompSyncDataCache.CompMineJoinRoleNum.Length != 0)
				{
					int num2 = CompManager.getInstance().CompSyncDataCache.CompMineJoinRoleNum[client.ClientData.CompType - 1];
					num = ((num2 > 0) ? ((double)compRoleData.MineRankNum / (double)num2) : 1.0);
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				foreach (CompMineRewardConfig compMineRewardConfig in this.RuntimeData.CompMineRewardConfigList)
				{
					if (compMineRewardConfig.Rank > 0 && compMineRewardConfig.Rank == compRoleData.MineRankNum)
					{
						result = compMineRewardConfig;
						break;
					}
					if (compMineRewardConfig.RankRate > 0.0 && num <= compMineRewardConfig.RankRate)
					{
						result = compMineRewardConfig;
						break;
					}
				}
			}
			return result;
		}

		private int GiveRoleAwards(GameClient client, long LastStartTimeTicks)
		{
			KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
			int result;
			if (null == kfcompRoleData)
			{
				result = -11003;
			}
			else
			{
				CompMineRewardConfig compMineRewardConfig = this.CalMineRewardConfig(client, kfcompRoleData);
				if (null == compMineRewardConfig)
				{
					result = -3;
				}
				else
				{
					Dictionary<int, KFCompData> dictionary = null;
					lock (CompManager.getInstance().RuntimeData.Mutex)
					{
						dictionary = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
					}
					KFCompData kfcompData;
					if (!dictionary.TryGetValue(client.ClientData.CompType, out kfcompData))
					{
						result = -3;
					}
					else
					{
						double num = 1.0;
						if (kfcompData.MineRank > 0 && kfcompData.MineRank <= this.RuntimeData.CompMineAwardNum.Length)
						{
							lock (this.RuntimeData.Mutex)
							{
								num = this.RuntimeData.CompMineAwardNum[kfcompData.MineRank - 1];
							}
						}
						int num2 = (int)((double)compMineRewardConfig.Grade * num);
						int num3 = (int)((double)compMineRewardConfig.Contribution * num);
						List<AwardsItemData> items = compMineRewardConfig.AwardsItemListOne.Items;
						List<AwardsItemData> items2 = compMineRewardConfig.AwardsItemListTwo.Items;
						int num4 = 0;
						if (items != null)
						{
							num4 += items.Count;
						}
						if (items2 != null)
						{
							num4 += items2.Count((AwardsItemData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
						}
						if (!Global.CanAddGoodsNum(client, num4))
						{
							result = -100;
						}
						else
						{
							if (num2 > 0 && CompManager.getInstance().CheckCanAddJunXian(LastStartTimeTicks))
							{
								TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, num2);
								string msgText = string.Format(GLang.GetLang(4017, new object[0]), num2);
								GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							if (num3 > 0)
							{
								GameManager.ClientMgr.ModifyCompDonateValue(client, num3, "势力矿洞奖励", true, true, false);
							}
							if (items != null)
							{
								foreach (AwardsItemData awardsItemData in items)
								{
									int num5 = (int)((double)awardsItemData.GoodsNum * num);
									if (num5 > 0)
									{
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, num5, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "势力矿洞奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
									}
								}
							}
							if (items2 != null)
							{
								foreach (AwardsItemData awardsItemData in items2)
								{
									if (Global.IsCanGiveRewardByOccupation(client, awardsItemData.GoodsID))
									{
										int num5 = (int)((double)awardsItemData.GoodsNum * num);
										if (num5 > 0)
										{
											Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, num5, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "势力矿洞奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
										}
									}
								}
							}
							result = 1;
						}
					}
				}
			}
			return result;
		}

		private void BroadCastImportantMsg(CompMineScene scene, int compType, string msg)
		{
			List<GameClient> clientsList = scene.CopyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null && (compType <= 0 || gameClient.ClientData.CompType == compType))
					{
						GameManager.ClientMgr.NotifyImportantMsg(gameClient, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= CompMineManager.NextHeartBeatTicks)
			{
				CompMineManager.NextHeartBeatTicks = num + 1020L;
				foreach (CompMineScene compMineScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = compMineScene.FuBenSeqId;
						int copyMapId = compMineScene.CopyMapId;
						int nMapCode = compMineScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = compMineScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (compMineScene.m_eStatus == 1 || compMineScene.m_eStatus == 2)
							{
								this.CheckCreateDynamicMonster(compMineScene, num2);
							}
							if (compMineScene.m_eStatus == 0)
							{
								if (num2 >= compMineScene.StartTimeTicks)
								{
									compMineScene.m_lPrepareTime = compMineScene.StartTimeTicks;
									compMineScene.m_lBeginTime = compMineScene.m_lPrepareTime + (long)(compMineScene.SceneInfo.PrepareSecs * 1000);
									compMineScene.m_eStatus = 1;
									compMineScene.StateTimeData.GameType = 31;
									compMineScene.StateTimeData.State = compMineScene.m_eStatus;
									compMineScene.StateTimeData.EndTicks = compMineScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, compMineScene.StateTimeData, compMineScene.CopyMap);
									this.InitCreateDynamicMonster(compMineScene, num2);
								}
							}
							else if (compMineScene.m_eStatus == 1)
							{
								if (num2 >= compMineScene.m_lBeginTime)
								{
									compMineScene.m_eStatus = 2;
									compMineScene.m_lEndTime = compMineScene.m_lBeginTime + (long)(compMineScene.SceneInfo.FightingSecs * 1000);
									compMineScene.StateTimeData.GameType = 31;
									compMineScene.StateTimeData.State = compMineScene.m_eStatus;
									compMineScene.StateTimeData.EndTicks = compMineScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, compMineScene.StateTimeData, compMineScene.CopyMap);
									for (int i = 1; i <= 3; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (compMineScene.m_eStatus == 2)
							{
								if (num2 >= compMineScene.m_lEndTime)
								{
									this.ProcessEnd(compMineScene, num2);
								}
							}
							else if (compMineScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(compMineScene.CopyMap);
								compMineScene.m_eStatus = 4;
								TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(31, -1, compMineScene.SceneInfo.ID, -1, -1, 6);
								this.GiveAwards(compMineScene);
								CompFuBenData compFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(compMineScene.GameId, out compFuBenData))
								{
									compFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("势力矿洞跨服副本GameID={0},战斗结束", compFuBenData.GameId), null, true);
								}
							}
							else if (compMineScene.m_eStatus == 4)
							{
								if (num2 >= compMineScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(compMineScene.m_lLeaveTime);
									compMineScene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int j = 0; j < clientsList.Count; j++)
											{
												GameClient gameClient = clientsList[j];
												if (gameClient != null)
												{
													KuaFuManager.getInstance().GotoLastMap(gameClient);
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "势力矿洞系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public const SceneUIClasses ManagerType = 53;

		private static CompMineManager instance = new CompMineManager();

		public CompMineRuntimeData RuntimeData = new CompMineRuntimeData();

		public ConcurrentDictionary<int, CompMineScene> SceneDict = new ConcurrentDictionary<int, CompMineScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
