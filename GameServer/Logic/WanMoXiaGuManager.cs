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
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class WanMoXiaGuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2, ICopySceneManager
	{
		public static WanMoXiaGuManager getInstance()
		{
			return WanMoXiaGuManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			GlobalEventSource4Scene.getInstance().registerListener(10001, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10000, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10004, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10005, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, WanMoXiaGuManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10000, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10004, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10005, 49, WanMoXiaGuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, WanMoXiaGuManager.getInstance());
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
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				Monster monster = monsterDeadEventObject.getMonster();
				GameClient attacker = monsterDeadEventObject.getAttacker();
				if (attacker != null && null != monster)
				{
					this.OnInjureMonster(attacker, monster, 0L);
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10000:
			{
				KuaFuFuBenRoleCountEvent kuaFuFuBenRoleCountEvent = eventObject as KuaFuFuBenRoleCountEvent;
				if (null != kuaFuFuBenRoleCountEvent)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuFuBenRoleCountEvent.RoleId);
					if (null != gameClient)
					{
						gameClient.sendCmd<int>(1264, kuaFuFuBenRoleCountEvent.RoleCount, false);
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10001:
			{
				KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
				if (null != kuaFuNotifyEnterGameEvent)
				{
					KuaFuServerLoginData kuaFuServerLoginData = kuaFuNotifyEnterGameEvent.Arg as KuaFuServerLoginData;
					if (null != kuaFuServerLoginData)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
						if (null != gameClient)
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(gameClient);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = kuaFuServerLoginData.RoleId;
								clientKuaFuServerLoginData.GameId = kuaFuServerLoginData.GameId;
								clientKuaFuServerLoginData.GameType = kuaFuServerLoginData.GameType;
								clientKuaFuServerLoginData.EndTicks = kuaFuServerLoginData.EndTicks;
								clientKuaFuServerLoginData.ServerId = kuaFuServerLoginData.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerLoginData.ServerIp;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerLoginData.ServerPort;
								clientKuaFuServerLoginData.FuBenSeqId = kuaFuServerLoginData.FuBenSeqId;
								gameClient.sendCmd(1263, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, kuaFuNotifyEnterGameEvent.TeamCombatAvg), false);
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10004:
			{
				KuaFuNotifyCopyCancelEvent kuaFuNotifyCopyCancelEvent = eventObject as KuaFuNotifyCopyCancelEvent;
				GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuNotifyCopyCancelEvent.RoleId);
				if (gameClient != null)
				{
					gameClient.ClientData.SignUpGameType = 0;
					gameClient.sendCmd(1265, string.Format("{0}:{1}", kuaFuNotifyCopyCancelEvent.GameId, kuaFuNotifyCopyCancelEvent.Reason), false);
				}
				eventObject.Handled = true;
				break;
			}
			case 10005:
			{
				KuaFuNotifyRealEnterGameEvent kuaFuNotifyRealEnterGameEvent = eventObject as KuaFuNotifyRealEnterGameEvent;
				if (kuaFuNotifyRealEnterGameEvent != null)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuNotifyRealEnterGameEvent.RoleId);
					if (gameClient != null)
					{
						gameClient.ClientData.SignUpGameType = 0;
						GlobalNew.RecordSwitchKuaFuServerLog(gameClient);
						gameClient.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(gameClient), false);
					}
				}
				eventObject.Handled = true;
				break;
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
					Dictionary<int, WanMoXiaGuMonsterConfigInfo> dictionary = new Dictionary<int, WanMoXiaGuMonsterConfigInfo>();
					text = Global.GameResPath("Config/WanMoXiaGu.xml");
					XElement xelement = CheckHelper.LoadXml(text, true);
					if (null == xelement)
					{
						return false;
					}
					foreach (XElement xelement2 in xelement.Elements())
					{
						if (xelement2 != null)
						{
							WanMoXiaGuMonsterConfigInfo wanMoXiaGuMonsterConfigInfo = new WanMoXiaGuMonsterConfigInfo();
							wanMoXiaGuMonsterConfigInfo.ID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							wanMoXiaGuMonsterConfigInfo.MonsterID = ConfigHelper.GetElementAttributeValueIntArray(xelement2, "MonstersID", null);
							wanMoXiaGuMonsterConfigInfo.MonstersNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "MonstersNum", "0"));
							wanMoXiaGuMonsterConfigInfo.BeginNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "BeginNum", "0"));
							wanMoXiaGuMonsterConfigInfo.EndNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "EndNum", "0"));
							this.RuntimeData.BeginNum = Math.Min(this.RuntimeData.BeginNum, wanMoXiaGuMonsterConfigInfo.BeginNum);
							this.RuntimeData.EndNum = Math.Max(this.RuntimeData.EndNum, wanMoXiaGuMonsterConfigInfo.EndNum);
							string[] array = Global.GetDefAttributeStr(xelement2, "Site", "0,0").Split(new char[]
							{
								'|'
							});
							wanMoXiaGuMonsterConfigInfo.Site = new List<int>();
							foreach (string text2 in array)
							{
								wanMoXiaGuMonsterConfigInfo.Site.AddRange(ConfigHelper.String2IntList(text2, ','));
							}
							wanMoXiaGuMonsterConfigInfo.Props = Global.GetDefAttributeStr(xelement2, "Props", "");
							wanMoXiaGuMonsterConfigInfo.Intro = Global.GetDefAttributeStr(xelement2, "Intro", "");
							wanMoXiaGuMonsterConfigInfo.RecoverTime = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RecoverTime", "0"));
							wanMoXiaGuMonsterConfigInfo.RecoverNum = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "RecoverNum", "0"));
							wanMoXiaGuMonsterConfigInfo.Decorations = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Decorations", "0"));
							dictionary.Add(wanMoXiaGuMonsterConfigInfo.ID, wanMoXiaGuMonsterConfigInfo);
						}
					}
					this.RuntimeData.MonsterOrderConfigList = dictionary;
					this.RuntimeData.AwardList = ConfigHelper.ParserIntArrayList(GameManager.systemParamsList.GetParamValueByName("WanMoXiaGuAward"), true, '|', ',');
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("WanMoXiaGuCall", ',');
					this.RuntimeData.BossMonsterID = (int)paramValueDoubleArrayByName[0];
					this.RuntimeData.WanMoXiaGuCall = paramValueDoubleArrayByName[1];
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public bool ProcessJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				if (mapSceneType != 0)
				{
					client.sendCmd(nID, -21.ToString(), false);
					return true;
				}
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, -2001.ToString(), false);
					return true;
				}
				if (client.ClientData.SignUpGameType != 0)
				{
					client.sendCmd(nID, -2002.ToString(), false);
					return true;
				}
				if (KuaFuManager.getInstance().IsInCannotJoinKuaFuCopyTime(client))
				{
					client.sendCmd(nID, -2004.ToString(), false);
					return true;
				}
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this.RuntimeData.CopyID, out systemXmlItem))
				{
					client.sendCmd(nID, -3.ToString(), false);
					return true;
				}
				int intValue = systemXmlItem.GetIntValue("MinLevel", -1);
				int intValue2 = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
				int num = intValue2 * 100 + intValue;
				if (client.ClientData.ChangeLifeCount * 100 + client.ClientData.Level < num)
				{
					client.sendCmd(nID, -19.ToString(), false);
					return true;
				}
				int wanMoXiaGuCount = this.GetWanMoXiaGuCount(client);
				if (wanMoXiaGuCount >= systemXmlItem.GetIntValue("FinishNumber", -1))
				{
					client.sendCmd(nID, -16.ToString(), false);
					return true;
				}
				int num2 = 0;
				if (num2 > 0)
				{
					client.ClientData.SignUpGameType = 8;
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 8);
				}
				client.sendCmd<int>(nID, num2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessQuitCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd(nID, 0.ToString(), false);
					return true;
				}
				client.ClientData.SignUpGameType = 0;
				int cmdData = 0;
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd(nID, 0.ToString(), false);
					return true;
				}
				client.ClientData.SignUpGameType = 0;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				if (num > 0)
				{
					int num2 = 0;
					if (num2 < 0)
					{
						num = 0;
					}
				}
				else
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
				if (num <= 0)
				{
					Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					client.sendCmd(1262, 0.ToString(), false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool OnInitGame(GameClient client)
		{
			GameMap gameMap = null;
			bool result;
			if (GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapID, defaultBirthPosX, defaultBirthPosY, birthRadius);
				client.ClientData.MapCode = this.RuntimeData.MapID;
				client.ClientData.PosX = (int)mapPoint.X;
				client.ClientData.PosY = (int)mapPoint.Y;
				client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			GameMap gameMap = null;
			bool result;
			if (GameManager.MapMgr.DictMaps.TryGetValue(this.RuntimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[this.RuntimeData.MapID].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.RuntimeData.MapID, defaultBirthPosX, defaultBirthPosY, birthRadius);
				client.ClientData.CurrentLifeV = client.ClientData.LifeV;
				client.ClientData.CurrentMagicV = client.ClientData.MagicV;
				client.ClientData.MoveAndActionNum = 0;
				GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, (int)mapPoint.X, (int)mapPoint.Y, -1);
				Global.ClientRealive(client, (int)mapPoint.X, (int)mapPoint.Y, -1);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 96, hint);
		}

		public int GetWanMoXiaGuCount(GameClient client)
		{
			int num = 0;
			foreach (int fuBenID in this.RuntimeData.FuBenIds)
			{
				FuBenData fuBenData = Global.GetFuBenData(client, fuBenID);
				int num2;
				Global.GetFuBenEnterNum(fuBenData, out num2);
				num += num2;
			}
			return num;
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (copyMap.MapCode == this.RuntimeData.MapID)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.RuntimeData.Mutex)
				{
					WanMoXiaGuScene wanMoXiaGuScene = null;
					if (!this.SceneDict.TryGetValue(fuBenSeqID, out wanMoXiaGuScene))
					{
						wanMoXiaGuScene = new WanMoXiaGuScene();
						wanMoXiaGuScene.CopyMapInfo = copyMap;
						wanMoXiaGuScene.CleanAllInfo();
						wanMoXiaGuScene.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						wanMoXiaGuScene.MapID = mapCode;
						wanMoXiaGuScene.CopyMapID = copyMap.CopyMapID;
						wanMoXiaGuScene.FuBenSeqId = fuBenSeqID;
						wanMoXiaGuScene.PlayerCount = 1;
						wanMoXiaGuScene.BossLifePercent = 1.0;
						this.SceneDict[fuBenSeqID] = wanMoXiaGuScene;
					}
					else
					{
						wanMoXiaGuScene.PlayerCount++;
					}
					client.ClientData.BattleWhichSide = 1;
					client.SceneObject = wanMoXiaGuScene;
					copyMap.IsKuaFuCopy = true;
					copyMap.CustomPassAwards = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this.RuntimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, wanMoXiaGuScene.ScoreData, wanMoXiaGuScene.CopyMapInfo);
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
			return true;
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= WanMoXiaGuManager._nextHeartBeatTicks)
			{
				WanMoXiaGuManager._nextHeartBeatTicks = num + 1020L;
				long num2 = num / 1000L;
				List<WanMoXiaGuScene> list = new List<WanMoXiaGuScene>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (WanMoXiaGuScene wanMoXiaGuScene in this.SceneDict.Values)
					{
						int fuBenSeqId = wanMoXiaGuScene.FuBenSeqId;
						int copyMapID = wanMoXiaGuScene.CopyMapID;
						int mapID = wanMoXiaGuScene.MapID;
						if (fuBenSeqId >= 0 && copyMapID >= 0 && mapID >= 0)
						{
							CopyMap copyMapInfo = wanMoXiaGuScene.CopyMapInfo;
							if (wanMoXiaGuScene.SceneStatus == 0)
							{
								wanMoXiaGuScene.PrepareTime = num2;
								wanMoXiaGuScene.BeginTime = num2 + (long)this.RuntimeData.PrepareSecs;
								wanMoXiaGuScene.SceneStatus = 1;
								wanMoXiaGuScene.StateTimeData.GameType = 8;
								wanMoXiaGuScene.StateTimeData.State = wanMoXiaGuScene.SceneStatus;
								wanMoXiaGuScene.StateTimeData.EndTicks = num + (long)(this.RuntimeData.PrepareSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, wanMoXiaGuScene.StateTimeData, wanMoXiaGuScene.CopyMapInfo);
							}
							else if (wanMoXiaGuScene.SceneStatus == 1)
							{
								if (num2 >= wanMoXiaGuScene.BeginTime)
								{
									wanMoXiaGuScene.SceneStatus = 2;
									wanMoXiaGuScene.EndTime = num2 + (long)this.RuntimeData.FightingSecs;
									wanMoXiaGuScene.StateTimeData.GameType = 8;
									wanMoXiaGuScene.StateTimeData.State = wanMoXiaGuScene.SceneStatus;
									wanMoXiaGuScene.StateTimeData.EndTicks = num + (long)(this.RuntimeData.FightingSecs * 1000);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, wanMoXiaGuScene.StateTimeData, wanMoXiaGuScene.CopyMapInfo);
								}
							}
							else if (wanMoXiaGuScene.SceneStatus == 2)
							{
								if (num2 >= wanMoXiaGuScene.EndTime)
								{
									wanMoXiaGuScene.SceneStatus = 3;
								}
								else if (null == wanMoXiaGuScene.Boss)
								{
									wanMoXiaGuScene.Boss = (GameManager.MonsterMgr.FindMonsterByExtensionID(copyMapInfo.CopyMapID, this.RuntimeData.BossMonsterID).FirstOrDefault<object>() as Monster);
									if (null != wanMoXiaGuScene.Boss)
									{
										wanMoXiaGuScene.ScoreData.MonsterID = wanMoXiaGuScene.Boss.RoleID;
										wanMoXiaGuScene.ScoreData.BossLifePercent = 1.0;
									}
								}
								else if (wanMoXiaGuScene.Boss != null && wanMoXiaGuScene.Boss.Alive)
								{
									if (!wanMoXiaGuScene.MonsterCreated && wanMoXiaGuScene.BossLifePercent < this.RuntimeData.WanMoXiaGuCall)
									{
										int randomNumber = Global.GetRandomNumber(this.RuntimeData.BeginNum, this.RuntimeData.EndNum);
										foreach (WanMoXiaGuMonsterConfigInfo wanMoXiaGuMonsterConfigInfo in this.RuntimeData.MonsterOrderConfigList.Values)
										{
											if (randomNumber >= wanMoXiaGuMonsterConfigInfo.BeginNum && randomNumber <= wanMoXiaGuMonsterConfigInfo.EndNum)
											{
												wanMoXiaGuScene.ScoreData.Intro = wanMoXiaGuMonsterConfigInfo.Intro;
												wanMoXiaGuScene.ScoreData.Decorations = wanMoXiaGuMonsterConfigInfo.Decorations;
												wanMoXiaGuScene.ZuoQiInfo = wanMoXiaGuMonsterConfigInfo;
												this.CreateMonster(wanMoXiaGuScene, wanMoXiaGuScene.ZuoQiInfo);
												wanMoXiaGuScene.MonsterCreated = true;
												wanMoXiaGuScene.NextRelifeTicks = num + (long)(wanMoXiaGuScene.ZuoQiInfo.RecoverTime * 1000);
											}
										}
									}
									if (wanMoXiaGuScene.ZuoQiInfo != null)
									{
										if (wanMoXiaGuScene.MonsterCount != wanMoXiaGuScene.ScoreData.MonsterCount)
										{
											wanMoXiaGuScene.MonsterCount = wanMoXiaGuScene.ScoreData.MonsterCount;
											foreach (string text in wanMoXiaGuScene.ZuoQiInfo.Props.Split(new char[]
											{
												'|'
											}))
											{
												string[] array2 = text.Split(new char[]
												{
													','
												});
												ExtPropIndexes index;
												double num3;
												if (array2.Length == 2 && Enum.TryParse<ExtPropIndexes>(array2[0], out index) && double.TryParse(array2[1], out num3))
												{
													wanMoXiaGuScene.Boss.TempPropsBuffer.AddTempExtProp((int)index, num3 * (double)wanMoXiaGuScene.MonsterCount, long.MaxValue);
												}
											}
										}
										if (num >= wanMoXiaGuScene.NextRelifeTicks)
										{
											wanMoXiaGuScene.NextRelifeTicks = num + (long)(wanMoXiaGuScene.ZuoQiInfo.RecoverTime * 1000);
											if (wanMoXiaGuScene.MonsterCount > 0)
											{
												Monster boss = wanMoXiaGuScene.Boss;
												boss.AddLife((long)(wanMoXiaGuScene.ZuoQiInfo.RecoverNum * wanMoXiaGuScene.MonsterCount));
												List<object> all9Clients = Global.GetAll9Clients(boss);
												GameManager.ClientMgr.NotifyOthersRelife(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, boss, boss.MonsterZoneNode.MapCode, boss.CopyMapID, boss.RoleID, (int)boss.SafeCoordinate.X, (int)boss.SafeCoordinate.Y, (int)boss.SafeDirection, boss.VLife, boss.VMana, 120, all9Clients, 0);
											}
											wanMoXiaGuScene.BossLifePercent = (wanMoXiaGuScene.ScoreData.BossLifePercent = wanMoXiaGuScene.Boss.VLife / wanMoXiaGuScene.Boss.MonsterInfo.VLifeMax);
											GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, wanMoXiaGuScene.ScoreData, wanMoXiaGuScene.CopyMapInfo);
										}
									}
								}
								else
								{
									wanMoXiaGuScene.Success = 1;
									wanMoXiaGuScene.SceneStatus = 3;
								}
							}
							else if (wanMoXiaGuScene.SceneStatus == 3)
							{
								wanMoXiaGuScene.SceneStatus = 4;
								wanMoXiaGuScene.EndTime = num2;
								wanMoXiaGuScene.LeaveTime = wanMoXiaGuScene.EndTime + (long)this.RuntimeData.ClearRolesSecs;
								if (wanMoXiaGuScene.Success > 0)
								{
									this.GiveAwards(wanMoXiaGuScene);
								}
								wanMoXiaGuScene.StateTimeData.GameType = 8;
								wanMoXiaGuScene.StateTimeData.State = 3;
								wanMoXiaGuScene.StateTimeData.EndTicks = num + (long)(this.RuntimeData.ClearRolesSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, wanMoXiaGuScene.StateTimeData, wanMoXiaGuScene.CopyMapInfo);
							}
							else if (wanMoXiaGuScene.SceneStatus == 4)
							{
								if (num2 >= wanMoXiaGuScene.LeaveTime)
								{
									list.Add(wanMoXiaGuScene);
									copyMapInfo.SetRemoveTicks(wanMoXiaGuScene.LeaveTime);
									wanMoXiaGuScene.SceneStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMapInfo.GetClientsList();
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
										DataHelper.WriteExceptionLogEx(ex, "【万魔峡谷】清场调度异常");
									}
								}
							}
						}
					}
				}
				if (list.Count > 0)
				{
					lock (this.RuntimeData.Mutex)
					{
						foreach (WanMoXiaGuScene wanMoXiaGuScene in list)
						{
							WanMoXiaGuScene wanMoXiaGuScene2;
							this.SceneDict.TryRemove(wanMoXiaGuScene.FuBenSeqId, out wanMoXiaGuScene2);
						}
					}
				}
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.VLife <= 0.0)
			{
			}
			lock (this.RuntimeData.Mutex)
			{
				WanMoXiaGuScene wanMoXiaGuScene = client.SceneObject as WanMoXiaGuScene;
				if (wanMoXiaGuScene != null && wanMoXiaGuScene.SceneStatus == 2)
				{
					if (monster.GetMonsterData().ExtensionID == this.RuntimeData.BossMonsterID)
					{
						wanMoXiaGuScene.BossLifePercent = monster.VLife / monster.MonsterInfo.VLifeMax;
						if (wanMoXiaGuScene.BossLifePercent <= 0.0)
						{
							wanMoXiaGuScene.Success = 1;
							wanMoXiaGuScene.SceneStatus = 3;
						}
						if (wanMoXiaGuScene.BossLifePercent <= 0.0 || Math.Round(wanMoXiaGuScene.BossLifePercent, 2) != Math.Round(wanMoXiaGuScene.ScoreData.BossLifePercent, 2))
						{
							wanMoXiaGuScene.ScoreData.BossLifePercent = wanMoXiaGuScene.BossLifePercent;
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, wanMoXiaGuScene.ScoreData, wanMoXiaGuScene.CopyMapInfo);
						}
						else
						{
							wanMoXiaGuScene.ScoreData.BossLifePercent = wanMoXiaGuScene.BossLifePercent;
						}
					}
					else if (monster.VLife <= 0.0 && wanMoXiaGuScene.AddKilledMonster(monster))
					{
						wanMoXiaGuScene.ScoreData.MonsterCount--;
						wanMoXiaGuScene.ScoreData.MonsterCount = ((wanMoXiaGuScene.ScoreData.MonsterCount < 0) ? 0 : wanMoXiaGuScene.ScoreData.MonsterCount);
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, wanMoXiaGuScene.ScoreData, wanMoXiaGuScene.CopyMapInfo);
					}
				}
			}
		}

		public void CreateMonster(WanMoXiaGuScene scene, object tag)
		{
			CopyMap copyMapInfo = scene.CopyMapInfo;
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				WanMoXiaGuMonsterConfigInfo wanMoXiaGuMonsterConfigInfo = tag as WanMoXiaGuMonsterConfigInfo;
				if (wanMoXiaGuMonsterConfigInfo != null)
				{
					for (int i = 0; i < wanMoXiaGuMonsterConfigInfo.MonstersNum; i++)
					{
						int monsterID = wanMoXiaGuMonsterConfigInfo.MonsterID[i];
						int gridX = wanMoXiaGuMonsterConfigInfo.Site[i * 2] / gameMap.MapGridWidth;
						int gridY = wanMoXiaGuMonsterConfigInfo.Site[i * 2 + 1] / gameMap.MapGridHeight;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, 49, wanMoXiaGuMonsterConfigInfo, null);
					}
					scene.ScoreData.MonsterCount = wanMoXiaGuMonsterConfigInfo.MonstersNum;
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<WanMoXiaGuScoreData>(1266, scene.ScoreData, scene.CopyMapInfo);
				}
			}
		}

		public void GiveAwards(WanMoXiaGuScene scene)
		{
			try
			{
				FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(scene.CopyMapInfo.FubenMapID, scene.MapID);
				if (fuBenMapItem != null)
				{
					int num = (int)(scene.EndTime - scene.BeginTime);
					int num2 = 0;
					List<GameClient> list = scene.CopyMapInfo.GetClientsList().Distinct<GameClient>().ToList<GameClient>();
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							GameClient gameClient = list[i];
							if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
							{
								num2 += gameClient.ClientData.CombatForce;
								long num3 = (long)fuBenMapItem.Experience;
								int money = fuBenMapItem.Money1;
								int num4 = this.RuntimeData.AwardList.Count - 1;
								int num5 = this.RuntimeData.AwardList[num4][1];
								for (int j = 0; j <= num4; j++)
								{
									if (num <= this.RuntimeData.AwardList[j][0])
									{
										num5 = this.RuntimeData.AwardList[j][1];
										break;
									}
								}
								if (num3 > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(gameClient, num3, false, true, false, "万魔峡谷通关奖励");
								}
								if (money > 0)
								{
									GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, money, "万魔峡谷通关奖励", true);
								}
								List<GoodsData> list2 = new List<GoodsData>();
								if (null != fuBenMapItem.GoodsDataList)
								{
									for (int k = 0; k < fuBenMapItem.GoodsDataList.Count; k++)
									{
										GoodsData goodsData = new GoodsData(fuBenMapItem.GoodsDataList[k]);
										goodsData.GCount *= num5;
										list2.Add(goodsData);
									}
								}
								if (list2.Count > 0)
								{
									if (Global.CanAddGoodsDataList(gameClient, list2))
									{
										foreach (GoodsData goodsData in list2)
										{
											GoodsUtil.AddGoodsDBCommand(gameClient, goodsData, true, 1, "万魔峡谷通关奖励", true);
										}
									}
									else
									{
										Global.UseMailGivePlayerAward2(gameClient, list2, GLang.GetLang(4000, new object[0]), GLang.GetLang(4001, new object[0]), 0, 0, 0);
									}
								}
								WanMoXiaGuAwardsData cmdData = new WanMoXiaGuAwardsData
								{
									Success = scene.Success,
									UsedSecs = num,
									Exp = num3,
									Money = money,
									AwardsGoods = list2
								};
								gameClient.sendCmd<WanMoXiaGuAwardsData>(1267, cmdData, false);
								Global.UpdateFuBenDataForQuickPassTimer(gameClient, scene.CopyMapInfo.FubenMapID, 0, 1);
							}
						}
					}
					if (list != null && list.Count > 0)
					{
						int count = list.Count;
						num2 /= count;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "【万魔峡谷】清场调度异常");
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				WanMoXiaGuScene wanMoXiaGuScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out wanMoXiaGuScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, wanMoXiaGuScene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<WanMoXiaGuScoreData>(1266, wanMoXiaGuScene.ScoreData, false);
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			WanMoXiaGuScene wanMoXiaGuScene = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out wanMoXiaGuScene))
				{
					return;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				wanMoXiaGuScene.PlayerCount--;
				if (wanMoXiaGuScene.SceneStatus != 3 && wanMoXiaGuScene.SceneStatus != 4 && wanMoXiaGuScene.SceneStatus != 5)
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses _sceneType = 49;

		public const GameTypes GameType = 8;

		public ConcurrentDictionary<int, WanMoXiaGuScene> SceneDict = new ConcurrentDictionary<int, WanMoXiaGuScene>();

		private static long _nextHeartBeatTicks = 0L;

		public WanMoXiaGuData RuntimeData = new WanMoXiaGuData();

		private static WanMoXiaGuManager instance = new WanMoXiaGuManager();
	}
}
