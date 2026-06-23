using System;
using System.Collections.Generic;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Tools;
using KF.Contract.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class CopyWolfManager : IManager, IEventListener, IEventListenerEx
	{
		public static CopyWolfManager getInstance()
		{
			return CopyWolfManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			GlobalEventSource.getInstance().registerListener(11, CopyWolfManager.getInstance());
			GlobalEventSource.getInstance().registerListener(35, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 34, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 34, CopyWolfManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(11, CopyWolfManager.getInstance());
			GlobalEventSource.getInstance().removeListener(35, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 34, CopyWolfManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 34, CopyWolfManager.getInstance());
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				Monster monster = monsterDeadEventObject.getMonster();
				GameClient attacker = monsterDeadEventObject.getAttacker();
				this.MonsterDead(attacker, monster);
			}
			if (eventObject.getEventType() == 35)
			{
				MonsterToMonsterDeadEventObject monsterToMonsterDeadEventObject = eventObject as MonsterToMonsterDeadEventObject;
				Monster monsterAttack = monsterToMonsterDeadEventObject.getMonsterAttack();
				Monster monster = monsterToMonsterDeadEventObject.getMonster();
				CreateMonsterTagInfo createMonsterTagInfo = monster.Tag as CreateMonsterTagInfo;
				if (monster != null && monsterAttack != null && createMonsterTagInfo != null && monster.UniqueID != monsterAttack.UniqueID && createMonsterTagInfo.ManagerType == 34)
				{
					this.FortDead(monster);
				}
				else if (monster != null && monsterAttack != null && monster.UniqueID == monsterAttack.UniqueID)
				{
					this.MonsterDead(monster);
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			if (eventObject.EventType == 30)
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					CreateMonsterTagInfo createMonsterTagInfo = onCreateMonsterEventObject.Monster.Tag as CreateMonsterTagInfo;
					if (null != createMonsterTagInfo)
					{
						onCreateMonsterEventObject.Monster.AllwaySearchEnemy = true;
						if (createMonsterTagInfo.IsFort)
						{
							onCreateMonsterEventObject.Monster.Camp = this._runtimeData.CampID;
						}
						onCreateMonsterEventObject.Result = true;
						onCreateMonsterEventObject.Handled = true;
					}
				}
			}
			if (eventObject.EventType == 33)
			{
				PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
				if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 34)
				{
					Monster monster = preMonsterInjureEventObject.Attacker as Monster;
					Monster monster2 = preMonsterInjureEventObject.Monster;
					if (monster != null && monster2 != null)
					{
						CreateMonsterTagInfo createMonsterTagInfo = monster2.Tag as CreateMonsterTagInfo;
						if (createMonsterTagInfo != null && createMonsterTagInfo.IsFort)
						{
							int fuBenSeqId = createMonsterTagInfo.FuBenSeqId;
							if (fuBenSeqId > 0)
							{
								CopyWolfSceneInfo copyWolfSceneInfo = null;
								if (this._runtimeData.SceneDict.TryGetValue(fuBenSeqId, out copyWolfSceneInfo) && copyWolfSceneInfo != null)
								{
									int monsterHurt = this._runtimeData.GetMonsterHurt(monster.MonsterInfo.ExtensionID);
									int fortLifeNow = (int)Math.Max(0.0, monster2.VLife - (double)monsterHurt);
									copyWolfSceneInfo.ScoreData.FortLifeNow = fortLifeNow;
									copyWolfSceneInfo.ScoreData.FortLifeMax = (int)monster2.MonsterInfo.VLifeMax;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, copyWolfSceneInfo.ScoreData, copyWolfSceneInfo.CopyMapInfo);
									preMonsterInjureEventObject.Injure = monsterHurt;
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
			}
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (CopyWolfManager._mutex)
			{
				try
				{
					this._runtimeData.CopyWolfWaveDic.Clear();
					text = Global.GameResPath("Config/LangHunYaoSai.xml");
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
							CopyWolfWaveInfo copyWolfWaveInfo = new CopyWolfWaveInfo();
							copyWolfWaveInfo.WaveID = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "ID", "0"));
							copyWolfWaveInfo.MonsterList.Clear();
							string[] array = Global.GetDefAttributeStr(xelement2, "MonstersID", "0,0").Split(new char[]
							{
								'|'
							});
							foreach (string text2 in array)
							{
								string[] array3 = text2.Split(new char[]
								{
									','
								});
								copyWolfWaveInfo.MonsterList.Add(new int[]
								{
									int.Parse(array3[0]),
									int.Parse(array3[1])
								});
							}
							copyWolfWaveInfo.NextTime = Convert.ToInt32(Global.GetDefAttributeStr(xelement2, "NextTime", "60"));
							copyWolfWaveInfo.MonsterSiteDic.Clear();
							string[] array4 = Global.GetDefAttributeStr(xelement2, "Site", "0,0,0").Split(new char[]
							{
								'|'
							});
							foreach (string text3 in array4)
							{
								string[] array5 = text3.Split(new char[]
								{
									','
								});
								CopyWolfSiteInfo copyWolfSiteInfo = new CopyWolfSiteInfo();
								copyWolfSiteInfo.X = int.Parse(array5[0]);
								copyWolfSiteInfo.Y = int.Parse(array5[1]);
								copyWolfSiteInfo.Radius = int.Parse(array5[2]);
								copyWolfWaveInfo.MonsterSiteDic.Add(copyWolfSiteInfo);
							}
							this._runtimeData.CopyWolfWaveDic.Add(copyWolfWaveInfo.WaveID, copyWolfWaveInfo);
						}
					}
					string[] array6 = GameManager.systemParamsList.GetParamValueByName("LangHunYaoSaiMonstersHurt").Split(new char[]
					{
						'|'
					});
					foreach (string text4 in array6)
					{
						string[] array7 = text4.Split(new char[]
						{
							','
						});
						this._runtimeData.MonsterHurtDic.Add(int.Parse(array7[0]), int.Parse(array7[1]));
					}
					this._runtimeData.ScoreRateTime = (int)GameManager.systemParamsList.GetParamValueIntByName("LangHunYaoSaiTimeNum", -1);
					this._runtimeData.ScoreRateLife = (int)GameManager.systemParamsList.GetParamValueIntByName("LangHunYaoSaiLifeNum", -1);
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LangHunYaoSaiMonsters", ',');
					this._runtimeData.FortMonsterID = paramValueIntArrayByName[0];
					this._runtimeData.FortSite.X = (double)paramValueIntArrayByName[1];
					this._runtimeData.FortSite.Y = (double)paramValueIntArrayByName[2];
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		public bool AddCopyScene(GameClient client, CopyMap copyMap)
		{
			bool result;
			if (copyMap.MapCode == this._runtimeData.MapID)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (CopyWolfManager._mutex)
				{
					CopyWolfSceneInfo copyWolfSceneInfo = null;
					if (!this._runtimeData.SceneDict.TryGetValue(fuBenSeqID, out copyWolfSceneInfo))
					{
						copyWolfSceneInfo = new CopyWolfSceneInfo();
						copyWolfSceneInfo.CopyMapInfo = copyMap;
						copyWolfSceneInfo.CleanAllInfo();
						copyWolfSceneInfo.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						copyWolfSceneInfo.MapID = mapCode;
						copyWolfSceneInfo.CopyID = copyMap.CopyMapID;
						copyWolfSceneInfo.FuBenSeqId = fuBenSeqID;
						copyWolfSceneInfo.PlayerCount = 1;
						this._runtimeData.SceneDict[fuBenSeqID] = copyWolfSceneInfo;
					}
					else
					{
						copyWolfSceneInfo.PlayerCount++;
					}
					client.ClientData.BattleWhichSide = this._runtimeData.CampID;
					copyMap.IsKuaFuCopy = true;
					copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(this._runtimeData.TotalSecs * 1000));
					GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, copyWolfSceneInfo.ScoreData, copyWolfSceneInfo.CopyMapInfo);
				}
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 11);
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
				lock (CopyWolfManager._mutex)
				{
					CopyWolfSceneInfo copyWolfSceneInfo;
					this._runtimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out copyWolfSceneInfo);
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
			if (num >= CopyWolfManager._nextHeartBeatTicks)
			{
				CopyWolfManager._nextHeartBeatTicks = num + 1020L;
				long num2 = num / 1000L;
				foreach (CopyWolfSceneInfo copyWolfSceneInfo in this._runtimeData.SceneDict.Values)
				{
					lock (CopyWolfManager._mutex)
					{
						int fuBenSeqId = copyWolfSceneInfo.FuBenSeqId;
						int copyID = copyWolfSceneInfo.CopyID;
						int mapID = copyWolfSceneInfo.MapID;
						if (fuBenSeqId >= 0 && copyID >= 0 && mapID >= 0)
						{
							CopyMap copyMapInfo = copyWolfSceneInfo.CopyMapInfo;
							if (copyWolfSceneInfo.SceneStatus == 0)
							{
								copyWolfSceneInfo.PrepareTime = num2;
								copyWolfSceneInfo.BeginTime = num2 + (long)this._runtimeData.PrepareSecs;
								copyWolfSceneInfo.SceneStatus = 1;
								copyWolfSceneInfo.StateTimeData.GameType = 11;
								copyWolfSceneInfo.StateTimeData.State = copyWolfSceneInfo.SceneStatus;
								copyWolfSceneInfo.StateTimeData.EndTicks = num + (long)(this._runtimeData.PrepareSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, copyWolfSceneInfo.StateTimeData, copyWolfSceneInfo.CopyMapInfo);
							}
							else if (copyWolfSceneInfo.SceneStatus == 1)
							{
								if (num2 >= copyWolfSceneInfo.BeginTime)
								{
									copyWolfSceneInfo.SceneStatus = 2;
									copyWolfSceneInfo.EndTime = num2 + (long)this._runtimeData.FightingSecs;
									copyWolfSceneInfo.StateTimeData.GameType = 11;
									copyWolfSceneInfo.StateTimeData.State = copyWolfSceneInfo.SceneStatus;
									copyWolfSceneInfo.StateTimeData.EndTicks = num + (long)(this._runtimeData.FightingSecs * 1000);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, copyWolfSceneInfo.StateTimeData, copyWolfSceneInfo.CopyMapInfo);
								}
							}
							else if (copyWolfSceneInfo.SceneStatus == 2)
							{
								if (num2 >= copyWolfSceneInfo.EndTime)
								{
									copyWolfSceneInfo.SceneStatus = 3;
								}
								else
								{
									if (copyWolfSceneInfo.IsFortFlag <= 0)
									{
										this.CreateFort(copyWolfSceneInfo);
									}
									bool flag2 = false;
									lock (copyWolfSceneInfo)
									{
										CopyWolfWaveInfo waveConfig = this._runtimeData.GetWaveConfig(copyWolfSceneInfo.MonsterWave);
										if (waveConfig == null)
										{
											copyWolfSceneInfo.MonsterWaveOld = 0;
											copyWolfSceneInfo.MonsterWave = 0;
											copyWolfSceneInfo.SceneStatus = 3;
										}
										else
										{
											if (copyWolfSceneInfo.CreateMonsterTime > 0L && num2 - copyWolfSceneInfo.CreateMonsterTime >= (long)waveConfig.NextTime && waveConfig.NextTime > 0)
											{
												flag2 = true;
											}
											if (copyWolfSceneInfo.CreateMonsterTime > 0L && copyWolfSceneInfo.IsMonsterFlag == 0 && copyWolfSceneInfo.KilledMonsterHashSet.Count == copyWolfSceneInfo.MonsterCountCreate)
											{
												flag2 = true;
											}
											if (copyWolfSceneInfo.CreateMonsterTime <= 0L)
											{
												flag2 = true;
												copyWolfSceneInfo.MonsterWave = 0;
											}
											if (flag2)
											{
												this.CreateMonster(copyWolfSceneInfo, 1);
											}
										}
									}
								}
							}
							else if (copyWolfSceneInfo.SceneStatus == 3)
							{
								int leftSecond = 0;
								if (copyWolfSceneInfo.MonsterWave >= copyWolfSceneInfo.MonsterWaveTotal)
								{
									leftSecond = (int)Math.Max(0L, num2 - copyWolfSceneInfo.EndTime);
								}
								this.GiveAwards(copyWolfSceneInfo, leftSecond);
								copyWolfSceneInfo.SceneStatus = 4;
								copyWolfSceneInfo.EndTime = num2;
								copyWolfSceneInfo.LeaveTime = copyWolfSceneInfo.EndTime + (long)this._runtimeData.ClearRolesSecs;
								copyWolfSceneInfo.StateTimeData.GameType = 11;
								copyWolfSceneInfo.StateTimeData.State = 3;
								copyWolfSceneInfo.StateTimeData.EndTicks = num + (long)(this._runtimeData.ClearRolesSecs * 1000);
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, copyWolfSceneInfo.StateTimeData, copyWolfSceneInfo.CopyMapInfo);
							}
							else if (copyWolfSceneInfo.SceneStatus == 4)
							{
								if (num2 >= copyWolfSceneInfo.LeaveTime)
								{
									copyMapInfo.SetRemoveTicks(copyWolfSceneInfo.LeaveTime);
									copyWolfSceneInfo.SceneStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "【狼魂要塞】清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void CreateMonster(CopyWolfSceneInfo scene, int upWave = 1)
		{
			CopyMap copyMapInfo = scene.CopyMapInfo;
			CopyWolfWaveInfo copyWolfWaveInfo = null;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("【狼魂要塞】报错 地图配置 ID = {0}", scene.MapID), null, true);
			}
			else
			{
				long num = TimeUtil.NOW();
				long createMonsterTime = num / 1000L;
				lock (scene)
				{
					if (scene.MonsterWave >= scene.MonsterWaveTotal)
					{
						scene.MonsterWaveOld = scene.MonsterWave;
						scene.MonsterWave = 0;
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
						copyWolfWaveInfo = this._runtimeData.GetWaveConfig(num2);
						if (copyWolfWaveInfo == null)
						{
							LogManager.WriteLog(2, string.Format("【狼魂要塞】报错 刷怪波次 = {0}", num2), null, true);
						}
						else
						{
							scene.MonsterWave = num2;
							scene.CreateMonsterTime = createMonsterTime;
							int num3 = 0;
							int radius = 0;
							CreateMonsterTagInfo createMonsterTagInfo = new CreateMonsterTagInfo();
							createMonsterTagInfo.FuBenSeqId = scene.FuBenSeqId;
							createMonsterTagInfo.IsFort = false;
							createMonsterTagInfo.ManagerType = 34;
							foreach (CopyWolfSiteInfo copyWolfSiteInfo in copyWolfWaveInfo.MonsterSiteDic)
							{
								int gridX = gameMap.CorrectWidthPointToGridPoint(copyWolfSiteInfo.X + Global.GetRandomNumber(-copyWolfSiteInfo.Radius, copyWolfSiteInfo.Radius)) / gameMap.MapGridWidth;
								int gridY = gameMap.CorrectHeightPointToGridPoint(copyWolfSiteInfo.Y + Global.GetRandomNumber(-copyWolfSiteInfo.Radius, copyWolfSiteInfo.Radius)) / gameMap.MapGridHeight;
								foreach (int[] array in copyWolfWaveInfo.MonsterList)
								{
									int monsterID = array[0];
									int num4 = array[1];
									num3 += num4;
									GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, monsterID, scene.CopyMapInfo.CopyMapID, num4, gridX, gridY, radius, 0, 34, createMonsterTagInfo, null);
								}
							}
							scene.MonsterCountCreate += num3;
							scene.ScoreData.Wave = copyWolfWaveInfo.WaveID;
							scene.ScoreData.EndTime = num + (long)(copyWolfWaveInfo.NextTime * 1000);
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
						}
					}
				}
			}
		}

		public void CreateFort(CopyWolfSceneInfo scene)
		{
			CopyMap copyMapInfo = scene.CopyMapInfo;
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(scene.MapID, out gameMap))
			{
				LogManager.WriteLog(2, string.Format("【狼魂要塞】报错 地图配置 ID = {0}", scene.MapID), null, true);
			}
			else
			{
				lock (scene)
				{
					if (scene.IsFortFlag <= 0)
					{
						scene.IsFortFlag = 1;
						int gridX = gameMap.CorrectWidthPointToGridPoint((int)this._runtimeData.FortSite.X) / gameMap.MapGridWidth;
						int gridY = gameMap.CorrectHeightPointToGridPoint((int)this._runtimeData.FortSite.Y) / gameMap.MapGridHeight;
						CreateMonsterTagInfo createMonsterTagInfo = new CreateMonsterTagInfo();
						createMonsterTagInfo.FuBenSeqId = scene.FuBenSeqId;
						createMonsterTagInfo.IsFort = true;
						createMonsterTagInfo.ManagerType = 34;
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.MapID, this._runtimeData.FortMonsterID, scene.CopyMapInfo.CopyMapID, 1, gridX, gridY, 0, 0, 34, createMonsterTagInfo, null);
						XElement allMonstersXml = GameManager.MonsterZoneMgr.AllMonstersXml;
						if (allMonstersXml != null)
						{
							XElement safeXElement = Global.GetSafeXElement(allMonstersXml, "Monster", "ID", this._runtimeData.FortMonsterID.ToString());
							if (safeXElement != null)
							{
								int num = (int)Global.GetSafeAttributeLong(safeXElement, "MaxLife");
								scene.ScoreData.FortLifeNow = num;
								scene.ScoreData.FortLifeMax = num;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, scene.ScoreData, scene.CopyMapInfo);
							}
						}
					}
				}
			}
		}

		public bool IsCopyWolf(int fubenID)
		{
			return fubenID == this._runtimeData.CopyID;
		}

		public void MonsterDead(Monster monster)
		{
			CreateMonsterTagInfo createMonsterTagInfo = monster.Tag as CreateMonsterTagInfo;
			if (createMonsterTagInfo != null)
			{
				int fuBenSeqId = createMonsterTagInfo.FuBenSeqId;
				if (fuBenSeqId >= 0 && monster.CopyMapID >= 0 && this.IsCopyWolf(monster.CurrentMapCode))
				{
					CopyWolfSceneInfo copyWolfSceneInfo = null;
					if (this._runtimeData.SceneDict.TryGetValue(fuBenSeqId, out copyWolfSceneInfo) && copyWolfSceneInfo != null)
					{
						if (copyWolfSceneInfo.SceneStatus < 3)
						{
							if (copyWolfSceneInfo.AddKilledMonster(monster))
							{
								if (copyWolfSceneInfo.SceneStatus < 3)
								{
									lock (copyWolfSceneInfo)
									{
										if (copyWolfSceneInfo.IsMonsterFlag == 1 && copyWolfSceneInfo.KilledMonsterHashSet.Count == copyWolfSceneInfo.MonsterCountCreate)
										{
											copyWolfSceneInfo.MonsterWaveOld = copyWolfSceneInfo.MonsterWave;
											if (copyWolfSceneInfo.MonsterWave >= copyWolfSceneInfo.MonsterWaveTotal)
											{
												copyWolfSceneInfo.SceneStatus = 3;
											}
											else
											{
												copyWolfSceneInfo.IsMonsterFlag = 0;
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

		public void MonsterDead(GameClient client, Monster monster)
		{
			if (client.ClientData.FuBenSeqID >= 0 && client.ClientData.CopyMapID >= 0 && this.IsCopyWolf(client.ClientData.FuBenID))
			{
				CopyWolfSceneInfo copyWolfSceneInfo = null;
				if (this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out copyWolfSceneInfo) && copyWolfSceneInfo != null)
				{
					if (copyWolfSceneInfo.AddKilledMonster(monster))
					{
						if (copyWolfSceneInfo.SceneStatus < 3)
						{
							lock (copyWolfSceneInfo)
							{
								int num = copyWolfSceneInfo.AddMonsterScore(client.ClientData.RoleID, monster.MonsterInfo.WolfScore);
								copyWolfSceneInfo.ScoreData.RoleMonsterScore = copyWolfSceneInfo.RoleMonsterScore;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, copyWolfSceneInfo.ScoreData, copyWolfSceneInfo.CopyMapInfo);
								if (copyWolfSceneInfo.IsMonsterFlag == 1 && copyWolfSceneInfo.KilledMonsterHashSet.Count == copyWolfSceneInfo.MonsterCountCreate)
								{
									copyWolfSceneInfo.MonsterWaveOld = copyWolfSceneInfo.MonsterWave;
									if (copyWolfSceneInfo.MonsterWave >= copyWolfSceneInfo.MonsterWaveTotal)
									{
										copyWolfSceneInfo.SceneStatus = 3;
									}
									else
									{
										copyWolfSceneInfo.IsMonsterFlag = 0;
									}
								}
							}
						}
					}
				}
			}
		}

		public void FortDead(Monster fortMonster)
		{
			CreateMonsterTagInfo createMonsterTagInfo = fortMonster.Tag as CreateMonsterTagInfo;
			if (createMonsterTagInfo != null)
			{
				int fuBenSeqId = createMonsterTagInfo.FuBenSeqId;
				if (fuBenSeqId >= 0 && fortMonster.CopyMapID >= 0 && this.IsCopyWolf(fortMonster.CurrentMapCode))
				{
					CopyWolfSceneInfo copyWolfSceneInfo = null;
					if (this._runtimeData.SceneDict.TryGetValue(fuBenSeqId, out copyWolfSceneInfo) && copyWolfSceneInfo != null)
					{
						if (copyWolfSceneInfo.SceneStatus < 3)
						{
							lock (copyWolfSceneInfo)
							{
								copyWolfSceneInfo.SceneStatus = 3;
								copyWolfSceneInfo.ScoreData.FortLifeNow = 0;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<CopyWolfScoreData>(1025, copyWolfSceneInfo.ScoreData, copyWolfSceneInfo.CopyMapInfo);
							}
						}
					}
				}
			}
		}

		public void GiveAwards(CopyWolfSceneInfo scene, int leftSecond)
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
								int num2 = scene.MonsterWaveOld;
								if (num2 > scene.MonsterWaveTotal)
								{
									num2 = scene.MonsterWaveTotal;
								}
								int monsterScore = scene.GetMonsterScore(gameClient.ClientData.RoleID);
								int fortLifeNow = scene.ScoreData.FortLifeNow;
								int score = this.GetScore(monsterScore, leftSecond, fortLifeNow);
								long num3 = (long)this.AwardExp(fuBenMapItem.Experience, score);
								int num4 = this.AwardGoldBind(fuBenMapItem.Money1, score);
								int num5 = this.AwardWolfMoney(fuBenMapItem.WolfMoney, score);
								if (num3 > 0L)
								{
									GameManager.ClientMgr.ProcessRoleExperience(gameClient, num3, false, true, false, "none");
								}
								if (num4 > 0)
								{
									GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num4, string.Format("副本{0}通关奖励", scene.CopyID), false);
								}
								if (num5 > 0)
								{
									GameManager.ClientMgr.ModifyLangHunFenMoValue(gameClient, num5, "狼魂要塞", true, true);
								}
								CopyWolfAwardsData cmdData = new CopyWolfAwardsData
								{
									Wave = scene.MonsterWaveOld,
									Exp = num3,
									Money = num4,
									WolfMoney = num5,
									RoleScore = scene.GetMonsterScore(gameClient.ClientData.RoleID)
								};
								GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 1, 0, 11);
								gameClient.sendCmd<CopyWolfAwardsData>(1026, cmdData, false);
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
				DataHelper.WriteExceptionLogEx(ex, "【狼魂要塞】清场调度异常");
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool scoreInfo = true)
		{
			lock (CopyWolfManager._mutex)
			{
				CopyWolfSceneInfo copyWolfSceneInfo;
				if (this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out copyWolfSceneInfo))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, copyWolfSceneInfo.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<CopyWolfScoreData>(1025, copyWolfSceneInfo.ScoreData, false);
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			CopyWolfSceneInfo copyWolfSceneInfo = null;
			lock (this._runtimeData.SceneDict)
			{
				if (!this._runtimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out copyWolfSceneInfo))
				{
					return;
				}
			}
			lock (copyWolfSceneInfo)
			{
				copyWolfSceneInfo.PlayerCount--;
				if (copyWolfSceneInfo.SceneStatus != 3 && copyWolfSceneInfo.SceneStatus != 4 && copyWolfSceneInfo.SceneStatus != 5)
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
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

		public int GetScore(int monsterScore, int second, int life)
		{
			int num = this._runtimeData.ScoreRateTime * second;
			int num2 = this._runtimeData.ScoreRateLife * life;
			return Math.Max(0, monsterScore + num + num2);
		}

		public int AwardExp(int baseValue, int score)
		{
			return (int)((double)baseValue * (1.0 + Math.Pow((double)Math.Min(score, 1000000), 0.34) / 100.0));
		}

		public int AwardGoldBind(int baseValue, int score)
		{
			return (int)((double)baseValue * (1.0 + Math.Pow((double)Math.Min(score, 500000), 0.34) / 100.0));
		}

		public int AwardWolfMoney(int baseValue, int score)
		{
			return 200 + Math.Min(score, 100000) / 100;
		}

		public const GameTypes _gameType = 11;

		private static long _nextHeartBeatTicks = 0L;

		public static object _mutex = new object();

		public CopyWolfInfo _runtimeData = new CopyWolfInfo();

		private static CopyWolfManager instance = new CopyWolfManager();
	}
}
