using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ElementWarManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IManager2, IEventListener
	{
		public static ElementWarManager getInstance()
		{
			return ElementWarManager.instance;
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
			TCPCmdDispatcher.getInstance().registerProcessorEx(1010, 1, 1, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1011, 1, 1, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1012, 2, 2, ElementWarManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10000, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10004, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10005, 28, ElementWarManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10000, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10004, 28, ElementWarManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10005, 28, ElementWarManager.getInstance());
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
			bool result;
			switch (nID)
			{
			case 1010:
				result = this.ProcessJoinCmd(client, nID, bytes, cmdParams);
				break;
			case 1011:
				result = this.ProcessQuitCmd(client, nID, bytes, cmdParams);
				break;
			case 1012:
				result = this.ProcessEnterCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
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
						gameClient.sendCmd<int>(1013, kuaFuFuBenRoleCountEvent.RoleCount, false);
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
								gameClient.sendCmd(1012, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, kuaFuNotifyEnterGameEvent.TeamCombatAvg), false);
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
					gameClient.sendCmd(1016, string.Format("{0}:{1}", kuaFuNotifyCopyCancelEvent.GameId, kuaFuNotifyCopyCancelEvent.Reason), false);
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
			lock (this._runtimeData.Mutex)
			{
				try
				{
					this._runtimeData.MonsterOrderConfigList.Clear();
					text = Global.GameResPath("Config/YuanSuShiLian.xml");
					XElement xelement = CheckHelper.LoadXml(text, true);
					if (null == xelement)
					{
						return false;
					}
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xelement2 in enumerable)
					{
						if (xelement2 != null)
						{
							ElementWarMonsterConfigInfo elementWarMonsterConfigInfo = new ElementWarMonsterConfigInfo();
							elementWarMonsterConfigInfo.OrderID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							elementWarMonsterConfigInfo.MonsterCount = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Num", "0"));
							string[] array = Global.GetDefAttributeStr(xelement2, "MonstersID", "0,0,0").Split(new char[]
							{
								'|'
							});
							elementWarMonsterConfigInfo.MonsterIDs = new List<int>();
							foreach (string s in array)
							{
								elementWarMonsterConfigInfo.MonsterIDs.Add(int.Parse(s));
							}
							elementWarMonsterConfigInfo.Up1 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UpOne", "0"));
							elementWarMonsterConfigInfo.Up2 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UpTwo", "0"));
							elementWarMonsterConfigInfo.Up3 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UpThree", "0"));
							elementWarMonsterConfigInfo.Up4 = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "UpFour", "0"));
							elementWarMonsterConfigInfo.X = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "X", "0"));
							elementWarMonsterConfigInfo.Y = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Y", "0"));
							elementWarMonsterConfigInfo.Radius = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "Radius", "0"));
							this._runtimeData.MonsterOrderConfigList.Add(elementWarMonsterConfigInfo.OrderID, elementWarMonsterConfigInfo);
						}
					}
					this._runtimeData.AwardLight = GameManager.systemParamsList.GetParamValueIntArrayByName("YuanSuShiLianAward", ',');
					this._runtimeData.YuanSuShiLianAward2 = GameManager.systemParamsList.GetParamValueIntArrayByName("YuanSuShiLianAward2", ',');
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
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(this._runtimeData.CopyID, out systemXmlItem))
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
				int elementWarCount = this.GetElementWarCount(client);
				if (elementWarCount >= systemXmlItem.GetIntValue("FinishNumber", -1))
				{
					client.sendCmd(nID, -16.ToString(), false);
					return true;
				}
				int num2 = 0;
				if (num2 > 0)
				{
					client.ClientData.SignUpGameType = 4;
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 4);
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
					client.sendCmd(1011, 0.ToString(), false);
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
			if (GameManager.MapMgr.DictMaps.TryGetValue(this._runtimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this._runtimeData.MapID, defaultBirthPosX, defaultBirthPosY, birthRadius);
				client.ClientData.MapCode = this._runtimeData.MapID;
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
			if (GameManager.MapMgr.DictMaps.TryGetValue(this._runtimeData.MapID, out gameMap))
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[this._runtimeData.MapID].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this._runtimeData.MapID, defaultBirthPosX, defaultBirthPosY, birthRadius);
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
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("ElementWar") && GlobalNew.IsGongNengOpened(client, 64, hint);
		}

		public int GetElementWarCount(GameClient client)
		{
			int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ElementWarDayId");
			int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "ElementWarCount");
			int offsetDayNow = Global.GetOffsetDayNow();
			int result;
			if (offsetDayNow == roleParamsInt32FromDB)
			{
				result = roleParamsInt32FromDB2;
			}
			else
			{
				Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarDayId", offsetDayNow, true);
				Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", 0, true);
				result = 0;
			}
			return result;
		}

		public void AddElementWarCount(GameClient client)
		{
			int elementWarCount = this.GetElementWarCount(client);
			Global.SaveRoleParamsInt32ValueToDB(client, "ElementWarCount", elementWarCount + 1, true);
		}

		public bool AddCopyScene(GameClient client, CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this._runtimeData.MapID)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (ElementWarManager._mutex)
				{
					ElementWarScene elementWarScene = null;
					if (!this._sceneDict.TryGetValue(fuBenSeqID, out elementWarScene))
					{
						elementWarScene = new ElementWarScene();
						elementWarScene.CopyMapInfo = copyMap;
						elementWarScene.CleanAllInfo();
						elementWarScene.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						elementWarScene.MapID = mapCode;
						elementWarScene.CopyID = copyMap.CopyMapID;
						elementWarScene.FuBenSeqId = fuBenSeqID;
						elementWarScene.PlayerCount = 1;
						this._sceneDict[fuBenSeqID] = elementWarScene;
					}
					else
					{
						elementWarScene.PlayerCount++;
					}
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this._runtimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, elementWarScene.ScoreData, elementWarScene.CopyMapInfo);
				}
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 4);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool RemoveCopyScene(CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this._runtimeData.MapID)
			{
				lock (ElementWarManager._mutex)
				{
					ElementWarScene elementWarScene;
					this._sceneDict.TryRemove(copyMap.FuBenSeqID, out elementWarScene);
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
			if (num >= ElementWarManager._nextHeartBeatTicks)
			{
				ElementWarManager._nextHeartBeatTicks = num + 1020L;
				long num2 = num / 1000L;
				foreach (ElementWarScene elementWarScene in this._sceneDict.Values)
				{
					lock (ElementWarManager._mutex)
					{
						int fuBenSeqId = elementWarScene.FuBenSeqId;
						int copyID = elementWarScene.CopyID;
						int mapID = elementWarScene.MapID;
						if (fuBenSeqId >= 0 && copyID >= 0 && mapID >= 0)
						{
							CopyMap copyMapInfo = elementWarScene.CopyMapInfo;
							if (elementWarScene.SceneStatus == 0)
							{
								elementWarScene.PrepareTime = num2;
								elementWarScene.BeginTime = num2 + (long)this._runtimeData.PrepareSecs;
								elementWarScene.SceneStatus = 1;
								elementWarScene.StateTimeData.GameType = 4;
								elementWarScene.StateTimeData.State = elementWarScene.SceneStatus;
								elementWarScene.StateTimeData.EndTicks = num + (long)(this._runtimeData.PrepareSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, elementWarScene.StateTimeData, elementWarScene.CopyMapInfo);
							}
							else if (elementWarScene.SceneStatus == 1)
							{
								if (num2 >= elementWarScene.BeginTime)
								{
									elementWarScene.SceneStatus = 2;
									elementWarScene.EndTime = num2 + (long)this._runtimeData.FightingSecs;
									elementWarScene.StateTimeData.GameType = 4;
									elementWarScene.StateTimeData.State = elementWarScene.SceneStatus;
									elementWarScene.StateTimeData.EndTicks = num + (long)(this._runtimeData.FightingSecs * 1000);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, elementWarScene.StateTimeData, elementWarScene.CopyMapInfo);
								}
							}
							else if (elementWarScene.SceneStatus == 2)
							{
								if (num2 >= elementWarScene.EndTime)
								{
									elementWarScene.SceneStatus = 3;
								}
								else
								{
									bool flag2 = false;
									int upWave = 0;
									lock (elementWarScene)
									{
										ElementWarMonsterConfigInfo orderConfig = this._runtimeData.GetOrderConfig(elementWarScene.MonsterWave);
										if (orderConfig == null)
										{
											elementWarScene.MonsterWaveOld = 1;
											elementWarScene.MonsterWave = 0;
											elementWarScene.SceneStatus = 3;
										}
										else if (elementWarScene.CreateMonsterTime > 0L && num2 - elementWarScene.CreateMonsterTime >= (long)orderConfig.Up1)
										{
											elementWarScene.MonsterWave = 0;
											elementWarScene.SceneStatus = 3;
										}
										else
										{
											if (elementWarScene.CreateMonsterTime > 0L && elementWarScene.IsMonsterFlag == 0 && elementWarScene.ScoreData.MonsterCount <= 0L)
											{
												if (elementWarScene.MonsterWave >= elementWarScene.MonsterWaveTotal)
												{
													elementWarScene.MonsterWaveOld = elementWarScene.MonsterWave;
													elementWarScene.MonsterWave = 0;
													elementWarScene.SceneStatus = 3;
													continue;
												}
												flag2 = true;
												if (num2 - elementWarScene.CreateMonsterTime <= (long)orderConfig.Up4)
												{
													upWave = 4;
												}
												else if (num2 - elementWarScene.CreateMonsterTime <= (long)orderConfig.Up3)
												{
													upWave = 3;
												}
												else if (num2 - elementWarScene.CreateMonsterTime <= (long)orderConfig.Up2)
												{
													upWave = 2;
												}
												else if (num2 - elementWarScene.CreateMonsterTime < (long)orderConfig.Up1)
												{
													upWave = 1;
												}
											}
											if (elementWarScene.CreateMonsterTime <= 0L)
											{
												flag2 = true;
											}
											if (flag2)
											{
												this.CreateMonster(elementWarScene, upWave);
											}
										}
									}
								}
							}
							else if (elementWarScene.SceneStatus == 3)
							{
								this.GiveAwards(elementWarScene);
								elementWarScene.SceneStatus = 4;
								elementWarScene.EndTime = num2;
								elementWarScene.LeaveTime = elementWarScene.EndTime + (long)this._runtimeData.ClearRolesSecs;
								elementWarScene.StateTimeData.GameType = 4;
								elementWarScene.StateTimeData.State = 3;
								elementWarScene.StateTimeData.EndTicks = num + (long)(this._runtimeData.ClearRolesSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, elementWarScene.StateTimeData, elementWarScene.CopyMapInfo);
							}
							else if (elementWarScene.SceneStatus == 4)
							{
								if (num2 >= elementWarScene.LeaveTime)
								{
									copyMapInfo.SetRemoveTicks(elementWarScene.LeaveTime);
									elementWarScene.SceneStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMapInfo.GetClientsList();
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
										DataHelper.WriteExceptionLogEx(ex, "【元素试炼】清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void CreateMonster(ElementWarScene scene, int upWave)
		{
			CopyMap copyMapInfo = scene.CopyMapInfo;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("元素试炼报错 地图配置 ID = {0}", scene.MapID), null, true);
			}
			else
			{
				long num = TimeUtil.NOW();
				long createMonsterTime = num / 1000L;
				lock (scene)
				{
					if (scene.MonsterWave >= scene.MonsterWaveTotal)
					{
						scene.SceneStatus = 3;
					}
					else
					{
						scene.IsMonsterFlag = 1;
						int num2 = scene.MonsterWave + upWave;
						if (num2 > scene.MonsterWaveTotal)
						{
							num2 = scene.MonsterWaveTotal;
						}
						ElementWarMonsterConfigInfo orderConfig = this._runtimeData.GetOrderConfig(num2);
						if (orderConfig == null)
						{
							LogManager.WriteLog(2, string.Format("元素试炼报错 刷怪波次 = {0}", num2), null, true);
						}
						else
						{
							scene.MonsterCountCreate = orderConfig.MonsterCount;
							scene.MonsterWave = num2;
							scene.CreateMonsterTime = createMonsterTime;
							for (int i = 0; i < orderConfig.MonsterCount; i++)
							{
								int monsterID = orderConfig.MonsterIDs[i % orderConfig.MonsterIDs.Count];
								int gridX = gameMap.CorrectWidthPointToGridPoint(orderConfig.X + Global.GetRandomNumber(-orderConfig.Radius, orderConfig.Radius)) / gameMap.MapGridWidth;
								int gridY = gameMap.CorrectHeightPointToGridPoint(orderConfig.Y + Global.GetRandomNumber(-orderConfig.Radius, orderConfig.Radius)) / gameMap.MapGridHeight;
								int radius = 0;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, radius, 0, 0, null, null);
							}
							scene.ScoreData.Wave = orderConfig.OrderID;
							scene.ScoreData.EndTime = num + (long)(orderConfig.Up1 * 1000);
							scene.ScoreData.MonsterCount = (long)orderConfig.MonsterCount;
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, scene.ScoreData, scene.CopyMapInfo);
						}
					}
				}
			}
		}

		public bool IsElementWarCopy(int FubenID)
		{
			return FubenID == this._runtimeData.CopyID;
		}

		public void KillMonster(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsElementWarCopy(client.ClientData.FuBenID))
			{
				ElementWarScene elementWarScene = null;
				if (this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out elementWarScene) && elementWarScene != null)
				{
					if (elementWarScene.AddKilledMonster(monster))
					{
						if (elementWarScene.SceneStatus < 3)
						{
							lock (elementWarScene)
							{
								elementWarScene.MonsterCountKill++;
								elementWarScene.ScoreData.MonsterCount -= 1L;
								elementWarScene.ScoreData.MonsterCount = ((elementWarScene.ScoreData.MonsterCount < 0L) ? 0L : elementWarScene.ScoreData.MonsterCount);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<ElementWarScoreData>(1014, elementWarScene.ScoreData, elementWarScene.CopyMapInfo);
								if (elementWarScene.IsMonsterFlag == 1 && elementWarScene.MonsterCountKill >= elementWarScene.MonsterCountCreate && elementWarScene.ScoreData.MonsterCount <= 0L)
								{
									elementWarScene.MonsterWaveOld = elementWarScene.MonsterWave;
									if (elementWarScene.MonsterWave >= elementWarScene.MonsterWaveTotal)
									{
										elementWarScene.SceneStatus = 3;
									}
									else
									{
										elementWarScene.IsMonsterFlag = 0;
										elementWarScene.MonsterCountKill = 0;
										elementWarScene.MonsterCountCreate = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		public void GiveAwards(ElementWarScene scene)
		{
			try
			{
				FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(scene.CopyMapInfo.FubenMapID, scene.MapID);
				if (fuBenMapItem != null)
				{
					int num = 0;
					List<GameClient> clientsList = scene.CopyMapInfo.GetClientsList();
					if (clientsList != null && clientsList.Count > 0)
					{
						for (int i = 0; i < clientsList.Count; i++)
						{
							GameClient gameClient = clientsList[i];
							if (gameClient != null && gameClient == GameManager.ClientMgr.FindClient(gameClient.ClientData.RoleID))
							{
								long num2 = (long)fuBenMapItem.Experience;
								int money = fuBenMapItem.Money1;
								int monsterWaveOld = scene.MonsterWaveOld;
								int num3 = fuBenMapItem.LightAward + this._runtimeData.AwardLight[monsterWaveOld];
								int num4 = fuBenMapItem.YuanSuFenMoaward + this._runtimeData.YuanSuShiLianAward2[monsterWaveOld];
								if (num2 > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(gameClient, num2, false, true, false, "none");
								}
								if (money > 0)
								{
									GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, money, string.Format("副本{0}通关奖励", scene.CopyID), false);
								}
								if (num3 > 0)
								{
									GameManager.FluorescentGemMgr.AddFluorescentPoint(gameClient, num3, "元素试炼", true);
								}
								if (num4 > 0)
								{
									GameManager.ClientMgr.ModifyYuanSuFenMoValue(gameClient, num4, "元素试炼", true, false);
								}
								ElementWarAwardsData cmdData = new ElementWarAwardsData
								{
									Wave = scene.MonsterWaveOld,
									Exp = num2,
									Money = money,
									Light = num3,
									ysfm = num4
								};
								this.AddElementWarCount(gameClient);
								GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 1, 0, 4);
								gameClient.sendCmd<ElementWarAwardsData>(1015, cmdData, false);
								num += gameClient.ClientData.CombatForce;
								Global.UpdateFuBenDataForQuickPassTimer(gameClient, scene.CopyMapInfo.FubenMapID, 0, 1);
							}
						}
					}
					if (clientsList != null && clientsList.Count > 0)
					{
						int count = clientsList.Count;
						num /= count;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "【元素试炼】清场调度异常");
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (ElementWarManager._mutex)
			{
				ElementWarScene elementWarScene;
				if (this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out elementWarScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, elementWarScene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<ElementWarScoreData>(1014, elementWarScene.ScoreData, false);
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			ElementWarScene elementWarScene = null;
			lock (this._sceneDict)
			{
				if (!this._sceneDict.TryGetValue(client.ClientData.FuBenSeqID, out elementWarScene))
				{
					return;
				}
			}
			lock (elementWarScene)
			{
				elementWarScene.PlayerCount--;
				if (elementWarScene.SceneStatus != 3 && elementWarScene.SceneStatus != 4 && elementWarScene.SceneStatus != 5)
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public const SceneUIClasses _sceneType = 28;

		public const GameTypes _gameType = 4;

		public ElementWarData _runtimeData = new ElementWarData();

		private static ElementWarManager instance = new ElementWarManager();

		public static object _mutex = new object();

		public ConcurrentDictionary<int, ElementWarScene> _sceneDict = new ConcurrentDictionary<int, ElementWarScene>();

		private static long _nextHeartBeatTicks = 0L;
	}
}
