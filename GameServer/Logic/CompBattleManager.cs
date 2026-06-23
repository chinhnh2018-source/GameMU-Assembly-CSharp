using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
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
	public class CompBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static CompBattleManager getInstance()
		{
			return CompBattleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 2000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(2000, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2001, 2, 2, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2002, 2, 2, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2003, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2004, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2006, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(2009, 1, 1, CompBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(33, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 52, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(12, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompBattleManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(33, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 52, CompBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 52, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompBattleManager.getInstance());
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
			return GlobalNew.IsGongNengOpened(client, 98, hint) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen(120402);
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
				case 2000:
					return this.ProcessGetCompBattleBaseDataCmd(client, nID, bytes, cmdParams);
				case 2001:
					return this.ProcessGetCompBattleCityDataCmd(client, nID, bytes, cmdParams);
				case 2002:
					return this.ProcessCompBattleEnterCmd(client, nID, bytes, cmdParams);
				case 2003:
					return this.ProcessGetCompBattleAwardInfoCmd(client, nID, bytes, cmdParams);
				case 2004:
					return this.ProcessGetCompBattleStateCmd(client, nID, bytes, cmdParams);
				case 2006:
					return this.ProcessGetCompBattleSelfScoreCmd(client, nID, bytes, cmdParams);
				case 2009:
					return this.ProcessGetCompBattleAwardCmd(client, nID, bytes, cmdParams);
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
			if (num != 27)
			{
				if (num != 30)
				{
					if (num == 33)
					{
						PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
						if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 52)
						{
							Monster monster = preMonsterInjureEventObject.Monster;
							if (monster != null)
							{
								CompStrongholdConfig compStrongholdConfig = preMonsterInjureEventObject.Monster.Tag as CompStrongholdConfig;
								if (null != compStrongholdConfig)
								{
									bool flag = false;
									CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(preMonsterInjureEventObject.Monster.CopyMapID);
									CompBattleScene compBattleScene = null;
									if (copyMap != null && this.SceneDict.TryGetValue(copyMap.FuBenSeqID, out compBattleScene) && compBattleScene.m_eStatus == 2)
									{
										flag = true;
									}
									if (flag)
									{
										preMonsterInjureEventObject.Injure = this.RuntimeData.CompBattleFlagDamage;
									}
									else
									{
										preMonsterInjureEventObject.Injure = 0;
									}
									eventObject.Handled = true;
									eventObject.Result = true;
								}
							}
						}
					}
				}
				else
				{
					OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
					if (null != onCreateMonsterEventObject)
					{
						CompStrongholdConfig compStrongholdConfig = onCreateMonsterEventObject.Monster.Tag as CompStrongholdConfig;
						if (null != compStrongholdConfig)
						{
							onCreateMonsterEventObject.Monster.Camp = compStrongholdConfig.BattleWhichSide;
							onCreateMonsterEventObject.Result = true;
							onCreateMonsterEventObject.Handled = true;
						}
					}
				}
			}
			else
			{
				ProcessClickOnNpcEventObject processClickOnNpcEventObject = eventObject as ProcessClickOnNpcEventObject;
				if (null != processClickOnNpcEventObject)
				{
					if (null != processClickOnNpcEventObject.Npc)
					{
						int npcID = processClickOnNpcEventObject.Npc.NpcID;
					}
					if (this.OnSpriteClickOnNpc(processClickOnNpcEventObject.Client, processClickOnNpcEventObject.NpcId, processClickOnNpcEventObject.ExtensionID))
					{
						processClickOnNpcEventObject.Result = false;
						processClickOnNpcEventObject.Handled = true;
					}
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
					this.RuntimeData.CompBattleConfigDict.Clear();
					text = "Config/ForceCraft.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompBattleConfig compBattleConfig = new CompBattleConfig();
						compBattleConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compBattleConfig.Name = Global.GetSafeAttributeStr(xml, "Name");
						compBattleConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						compBattleConfig.ForceMax = (int)Global.GetSafeAttributeLong(xml, "ForceMax");
						compBattleConfig.MaxEnterNum = (int)Global.GetSafeAttributeLong(xml, "MaxEnterNum");
						compBattleConfig.EnterCD = (int)Global.GetSafeAttributeLong(xml, "EnterCD");
						compBattleConfig.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						compBattleConfig.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						compBattleConfig.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						compBattleConfig.DuiHuanType = (int)Global.GetSafeAttributeLong(xml, "DuiHuanType");
						if (!ConfigParser.ParserTimeRangeListWithDay(compBattleConfig.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < compBattleConfig.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(compBattleConfig.TimePoints[i].Hours, compBattleConfig.TimePoints[i].Minutes, compBattleConfig.TimePoints[i].Seconds);
							compBattleConfig.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						this.RuntimeData.CompBattleConfigDict[compBattleConfig.ID] = compBattleConfig;
					}
					this.RuntimeData.CompStrongholdConfigDict.Clear();
					text = "Config/ForceStronghold.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompStrongholdConfig compStrongholdConfig = new CompStrongholdConfig();
						compStrongholdConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compStrongholdConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						compStrongholdConfig.QiZhiID = Global.GetSafeAttributeIntArray(xml, "QiZhiID", -1, ',');
						compStrongholdConfig.Name = Global.GetSafeAttributeStr(xml, "Name");
						compStrongholdConfig.QiZuoID = (int)Global.GetSafeAttributeLong(xml, "QiZuoID");
						compStrongholdConfig.Point = (int)Global.GetSafeAttributeLong(xml, "Point");
						string[] array = Global.GetSafeAttributeStr(xml, "QiZuoSite").Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							compStrongholdConfig.PosX = Global.SafeConvertToInt32(array[0]);
							compStrongholdConfig.PosY = Global.SafeConvertToInt32(array[1]);
						}
						compStrongholdConfig.Rate = Global.GetSafeAttributeDouble(xml, "Rate");
						this.RuntimeData.CompStrongholdConfigDict[compStrongholdConfig.ID] = compStrongholdConfig;
					}
					this.RuntimeData.CompBattleBirthConfigDict.Clear();
					text = "Config/ForceCraftBirth.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompBattleBirthConfig compBattleBirthConfig = new CompBattleBirthConfig();
						compBattleBirthConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compBattleBirthConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						compBattleBirthConfig.ForceID = (int)Global.GetSafeAttributeLong(xml, "ForceID");
						compBattleBirthConfig.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						compBattleBirthConfig.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						compBattleBirthConfig.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.CompBattleBirthConfigDict[new KeyValuePair<int, int>(compBattleBirthConfig.MapCode, compBattleBirthConfig.ForceID)] = compBattleBirthConfig;
					}
					this.RuntimeData.CompBattleRewardConfigList.Clear();
					text = "Config/ForceCraftReward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompBattleRewardConfig compBattleRewardConfig = new CompBattleRewardConfig();
						compBattleRewardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compBattleRewardConfig.Rank = (int)Global.GetSafeAttributeLong(xml, "Rank");
						compBattleRewardConfig.RankRate = Global.GetSafeAttributeDouble(xml, "RankRate");
						compBattleRewardConfig.Grade = (int)Global.GetSafeAttributeLong(xml, "Grade");
						compBattleRewardConfig.Contribution = (int)Global.GetSafeAttributeLong(xml, "Contribution");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsOne"), ref compBattleRewardConfig.AwardsItemListOne, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsTwo"), ref compBattleRewardConfig.AwardsItemListTwo, '|', ',');
						this.RuntimeData.CompBattleRewardConfigList.Add(compBattleRewardConfig);
					}
					this.RuntimeData.CompBattleSingleIntegral = GameManager.systemParamsList.GetParamValueIntArrayByName("CraftSingleIntegral", ',');
					this.RuntimeData.CompBattleRewardRate = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CraftRewardRate", ',');
					this.RuntimeData.CompBattleFlagDamage = (int)GameManager.systemParamsList.GetParamValueIntByName("FlagDamage", -1);
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
					foreach (CompBattleScene compBattleScene in this.SceneDict.Values)
					{
						CompFuBenData compFuBenData = null;
						if (this.RuntimeData.FuBenItemData.TryGetValue(compBattleScene.GameId, out compFuBenData))
						{
							compFuBenData.Init();
							List<GameClient> clientsList = compBattleScene.CopyMap.GetClientsList();
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
							TianTiClient.getInstance().Comp_UpdateKuaFuMapClientCount(30, compFuBenData);
						}
					}
					foreach (KeyValuePair<KeyValuePair<int, int>, List<CompBattleWaitData>> keyValuePair in this.RuntimeData.CompBattleWaitQueueDict)
					{
						KeyValuePair<int, int> key = keyValuePair.Key;
						List<CompBattleWaitData> value = keyValuePair.Value;
						if (value.Count > 0)
						{
							int key2 = key.Key;
							int value2 = key.Value;
							CompBattleConfig compBattleConfig = null;
							if (this.RuntimeData.CompBattleConfigDict.TryGetValue(key2, out compBattleConfig))
							{
								KuaFuServerInfo kuaFuServerInfo = null;
								bool flag2 = false;
								CompFuBenData compFuBenData2 = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, key2);
								if (compFuBenData2 != null && compFuBenData2.GetRoleCountWithEnter(value2) < compBattleConfig.MaxEnterNum && KuaFuManager.getInstance().TryGetValue(compFuBenData2.ServerId, out kuaFuServerInfo))
								{
									flag2 = true;
								}
								List<CompBattleWaitData> list = new List<CompBattleWaitData>();
								if (flag2)
								{
									for (int j = 0; j < value.Count; j++)
									{
										CompBattleWaitData compBattleWaitData = value[j];
										GameClient gameClient2 = GameManager.ClientMgr.FindClient(compBattleWaitData.RoleId);
										if (null == gameClient2)
										{
											list.Add(compBattleWaitData);
										}
										else
										{
											int num = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, gameClient2.ServerId, key2, gameClient2.ClientData.RoleID, (int)gameClient2.ClientData.CompZhiWu, 4);
											if (num < 0)
											{
												break;
											}
											KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(gameClient2);
											if (null != clientKuaFuServerLoginData)
											{
												clientKuaFuServerLoginData.RoleId = gameClient2.ClientData.RoleID;
												clientKuaFuServerLoginData.GameId = (long)compFuBenData2.GameId;
												clientKuaFuServerLoginData.GameType = 30;
												clientKuaFuServerLoginData.EndTicks = compFuBenData2.EndTime.Ticks;
												clientKuaFuServerLoginData.ServerId = gameClient2.ServerId;
												clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
												clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
												clientKuaFuServerLoginData.FuBenSeqId = 0;
											}
											GlobalNew.RecordSwitchKuaFuServerLog(gameClient2);
											gameClient2.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(gameClient2), false);
											list.Add(compBattleWaitData);
										}
									}
								}
								for (int j = 0; j < list.Count; j++)
								{
									this.RemoveWait(list[j].RoleId);
								}
								for (int j = 0; j < value.Count; j++)
								{
									CompBattleWaitData compBattleWaitData = value[j];
									GameClient gameClient2 = GameManager.ClientMgr.FindClient(compBattleWaitData.RoleId);
									if (null != gameClient2)
									{
										int num = -22;
										gameClient2.sendCmd(2002, string.Format("{0}:{1}", num, j + 1), false);
									}
								}
							}
						}
					}
				}
			}
		}

		public CompBattleBaseData GetCompBattleBaseData(int compType)
		{
			CompBattleBaseData result = null;
			lock (this.RuntimeData.Mutex)
			{
				result = (CompBattleBaseData)this.RuntimeData.compBattleBaseData.Clone();
			}
			return result;
		}

		public void UpdateCompBattleBaseData(Dictionary<int, KFCompData> tempCompDataDict)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.compBattleBaseData.ClearAll();
				for (int i = 1; i <= 3; i++)
				{
					CompBattleOwnCity compBattleOwnCity = new CompBattleOwnCity();
					KFCompData kfcompData = null;
					if (tempCompDataDict.TryGetValue(i, out kfcompData))
					{
						foreach (KeyValuePair<int, CompStrongholdData> keyValuePair in kfcompData.StrongholdDict)
						{
							if (keyValuePair.Value.StrongholdSet.Count > 0 && 1 == keyValuePair.Value.Rank)
							{
								compBattleOwnCity.OwnCityList.Add(keyValuePair.Key);
							}
						}
					}
					this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Add(compBattleOwnCity);
				}
			}
		}

		public bool ProcessGetCompBattleBaseDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleBaseData cmdData = null;
				lock (this.RuntimeData.Mutex)
				{
					cmdData = (CompBattleBaseData)this.RuntimeData.compBattleBaseData.Clone();
				}
				client.sendCmd<CompBattleBaseData>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompBattleCityDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompBattleConfig compBattleConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(num2, out compBattleConfig))
					{
						return true;
					}
				}
				Dictionary<int, KFCompData> dictionary = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					dictionary = CompManager.getInstance().CompSyncDataCache.CompDataDict.V;
				}
				CompBattleCifyData compBattleCifyData = new CompBattleCifyData();
				compBattleCifyData.CityID = num2;
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(client, ref compBattleGameStates);
				if (1 == compBattleGameStates)
				{
					CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, num2);
					if (null != compFuBenData)
					{
						compBattleCifyData.RoleNum = compFuBenData.GetRoleCountWithEnter(client.ClientData.CompType);
						HashSet<int> hashSet = null;
						if (compFuBenData.ZhuJiangRoleDict.TryGetValue(client.ClientData.CompType, out hashSet) && null != hashSet)
						{
							foreach (int roleId in hashSet)
							{
								KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(roleId);
								if (kfcompRoleData != null && null != kfcompRoleData.RoleData4Selector)
								{
									RoleData4Selector roleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(kfcompRoleData.RoleData4Selector, 0, kfcompRoleData.RoleData4Selector.Length);
									if (null != roleData4Selector)
									{
										CompBattleZhuJiangInfo item = new CompBattleZhuJiangInfo
										{
											RoleID = roleData4Selector.RoleID,
											Name = roleData4Selector.RoleName,
											Level = roleData4Selector.Level,
											ZoneID = roleData4Selector.ZoneId,
											Occupation = roleData4Selector.Occupation,
											RoleSex = roleData4Selector.RoleSex,
											CompZhiWu = kfcompRoleData.ZhiWu
										};
										compBattleCifyData.ZhuJiangList.Add(item);
									}
								}
							}
						}
					}
				}
				lock (this.RuntimeData.Mutex)
				{
					foreach (KFCompData kfcompData in dictionary.Values)
					{
						CompStrongholdData compStrongholdData = null;
						if (kfcompData.StrongholdDict.TryGetValue(num2, out compStrongholdData))
						{
							foreach (int key in compStrongholdData.StrongholdSet)
							{
								compBattleCifyData.StrongholdDict[key] = kfcompData.CompType;
								if (1 == compStrongholdData.Rank)
								{
									compBattleCifyData.OwnCompType = kfcompData.CompType;
								}
							}
						}
					}
				}
				client.sendCmd<CompBattleCifyData>(nID, compBattleCifyData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
					CompBattleConfig compBattleConfig = null;
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(num2, out compBattleConfig))
					{
						this.RemoveWait(client.ClientData.RoleID);
						num3 = 0;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					KuaFuServerInfo kuaFuServerInfo = null;
					CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, num2);
					if (compFuBenData == null || !KuaFuManager.getInstance().TryGetValue(compFuBenData.ServerId, out kuaFuServerInfo))
					{
						num3 = -11000;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					if (compFuBenData.GetRoleCountWithEnter(client.ClientData.CompType) >= compBattleConfig.MaxEnterNum)
					{
						if (this.AddToWait(num2, client.ClientData.CompType, client.ClientData.RoleID))
						{
							num3 = -22;
							client.sendCmd(nID, string.Format("{0}:{1}", num3, this.GetWaitingCount(num2, client.ClientData.CompType)), false);
							return true;
						}
						num3 = -5;
						client.sendCmd(nID, string.Format("{0}:{1}", num3, 0), false);
						return true;
					}
					else
					{
						KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
						if (null != clientKuaFuServerLoginData)
						{
							clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
							clientKuaFuServerLoginData.GameId = (long)compFuBenData.GameId;
							clientKuaFuServerLoginData.GameType = 30;
							clientKuaFuServerLoginData.EndTicks = compFuBenData.EndTime.Ticks;
							clientKuaFuServerLoginData.ServerId = client.ServerId;
							clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
							clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
							clientKuaFuServerLoginData.FuBenSeqId = 0;
						}
					}
				}
				if (num3 >= 0)
				{
					num3 = TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, num2, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 4);
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

		public bool ProcessGetCompBattleAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == kfcompRoleData)
				{
					return true;
				}
				CompBattleAwardsData compBattleAwardsData = new CompBattleAwardsData();
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count == 3)
					{
						compBattleAwardsData.WinNum = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[kfcompRoleData.CompType - 1].OwnCityList.Count;
					}
				}
				if (kfcompRoleData.BattleJiFen > 0)
				{
					compBattleAwardsData.RankNum = kfcompRoleData.BattleRankNum;
					CompBattleRewardConfig compBattleRewardConfig = this.CalBattleRewardConfig(client, kfcompRoleData);
					compBattleAwardsData.AwardID = ((compBattleRewardConfig != null) ? compBattleRewardConfig.ID : 0);
				}
				client.sendCmd<CompBattleAwardsData>(2003, compBattleAwardsData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(client, ref compBattleGameStates);
				int num = compBattleGameStates;
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (num == 0 && kfcompRoleData != null && kfcompRoleData.CompType == kfcompRoleData.CompTypeBattle)
				{
					string roleParamByName = Global.GetRoleParamByName(client, "49");
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

		public bool ProcessGetCompBattleSelfScoreCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				CompBattleScene compBattleScene = client.SceneObject as CompBattleScene;
				if (null == compBattleScene)
				{
					return true;
				}
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == kfcompRoleData)
				{
					return true;
				}
				CompBattleSelfScore compBattleSelfScore = new CompBattleSelfScore();
				CompBattleRewardConfig compBattleRewardConfig = this.CalBattleRewardConfig(client, kfcompRoleData);
				compBattleSelfScore.RankNum = kfcompRoleData.BattleRankNum;
				compBattleSelfScore.AwardID = ((compBattleRewardConfig != null) ? compBattleRewardConfig.ID : 0);
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					CompManager.getInstance().CompSyncDataCache.CompRankBattleJiFenDict.V.TryGetValue(client.ClientData.CompType, out compBattleSelfScore.rankInfo2Client);
				}
				client.sendCmd<CompBattleSelfScore>(nID, compBattleSelfScore, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
				string roleParamByName = Global.GetRoleParamByName(client, "49");
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
						Global.SaveRoleParamsStringToDB(client, "49", "", true);
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

		public int GetWaitingCount(int cityId, int comptype)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(cityId, comptype);
				List<CompBattleWaitData> list = null;
				if (!this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(key, out list))
				{
					result = 0;
				}
				else
				{
					result = list.Count;
				}
			}
			return result;
		}

		public bool IsInWait(int rid)
		{
			try
			{
				if (rid <= 0)
				{
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompBattleWaitAllDict.ContainsKey(rid))
					{
						return true;
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::IsInWait roleID={0}", rid));
				return false;
			}
			return false;
		}

		public bool AddToWait(int cityId, int comptype, int rid)
		{
			try
			{
				if (this.IsInWait(rid))
				{
					return false;
				}
				KeyValuePair<int, int> keyValuePair = new KeyValuePair<int, int>(cityId, comptype);
				List<CompBattleWaitData> list = null;
				if (!this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(keyValuePair, out list))
				{
					list = new List<CompBattleWaitData>();
					this.RuntimeData.CompBattleWaitQueueDict[keyValuePair] = list;
				}
				list.Add(new CompBattleWaitData
				{
					CityID = cityId,
					CompType = comptype,
					RoleId = rid
				});
				this.RuntimeData.CompBattleWaitAllDict[rid] = keyValuePair;
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::AddToWait roleID={0}", rid));
				return false;
			}
			return true;
		}

		public void RemoveWait(int rid)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.IsInWait(rid))
					{
						KeyValuePair<int, int> key;
						this.RuntimeData.CompBattleWaitAllDict.TryGetValue(rid, out key);
						List<CompBattleWaitData> list = null;
						if (this.RuntimeData.CompBattleWaitQueueDict.TryGetValue(key, out list))
						{
							list.RemoveAll((CompBattleWaitData x) => x.RoleId == rid);
							this.RuntimeData.CompBattleWaitAllDict.Remove(rid);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, string.Format("CompBattleManager::RemoveWait roleID={0}", rid));
			}
		}

		public void OnCompBattleReset()
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient != null && gameClient.ClientData.CompType > 0)
				{
					int compBattleJiFenValue = GameManager.ClientMgr.GetCompBattleJiFenValue(gameClient);
					if (compBattleJiFenValue > 0)
					{
						GameManager.ClientMgr.ModifyCompBattleJiFenValue(gameClient, -compBattleJiFenValue, "势力战KF", true, true, false);
					}
				}
			}
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == null || mapSceneType == 48;
		}

		public int CheckCondition(GameClient client, ref CompBattleGameStates state)
		{
			int result = 0;
			CompBattleConfig compBattleConfig = null;
			if (client != null && !this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					compBattleConfig = this.RuntimeData.CompBattleConfigDict.Values.FirstOrDefault<CompBattleConfig>();
					if (null == compBattleConfig)
					{
						return -12;
					}
				}
				result = -2001;
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < compBattleConfig.TimePoints.Count - 1; i += 2)
					{
						if (dateTime.DayOfWeek == (DayOfWeek)compBattleConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= compBattleConfig.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= compBattleConfig.SecondsOfDay[i + 1])
						{
							if (dateTime.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i + 1] - (double)compBattleConfig.ClearRolesSecs)
							{
								state = 1;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < compBattleConfig.SecondsOfDay[i + 1])
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
			CompBattleConfig compBattleConfig = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(sceneId, out compBattleConfig))
				{
					goto IL_14B;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < compBattleConfig.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)compBattleConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= compBattleConfig.SecondsOfDay[i] && dateTime.TimeOfDay.TotalSeconds <= compBattleConfig.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(compBattleConfig.SecondsOfDay[i]);
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
			CompFuBenData compFuBenData = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, (int)kuaFuServerLoginData.GameId);
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
					CompBattleConfig compBattleConfig;
					if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(compFuBenData.GameId, out compBattleConfig) || (long)compBattleConfig.ID != kuaFuServerLoginData.GameId)
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
				CompFuBenData compFuBenData2 = TianTiClient.getInstance().Comp_GetKuaFuFuBenData(30, (int)clientKuaFuServerLoginData.GameId);
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
			CompBattleConfig compBattleConfig;
			lock (this.RuntimeData.Mutex)
			{
				clientKuaFuServerLoginData.FuBenSeqId = compFuBenData.SequenceId;
				if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(compFuBenData.GameId, out compBattleConfig))
				{
					return false;
				}
			}
			client.ClientData.BattleWhichSide = client.ClientData.CompType;
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(compBattleConfig.MapCode, client, out posX, out posY);
			bool result;
			if (birthPoint <= 0)
			{
				LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + birthPoint, null, true);
				result = false;
			}
			else
			{
				client.ClientData.MapCode = compBattleConfig.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		private void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
			this.RemoveWait(client.ClientData.RoleID);
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
				CompBattleBirthConfig compBattleBirthConfig = null;
				KeyValuePair<int, int> key = new KeyValuePair<int, int>(mapCode, battleWhichSide);
				if (this.RuntimeData.CompBattleBirthConfigDict.TryGetValue(key, out compBattleBirthConfig))
				{
					posX = compBattleBirthConfig.PosX;
					posY = compBattleBirthConfig.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public void UpdateBattleSideScoreRank(CompBattleScene scene, bool update = true)
		{
			try
			{
				if (scene.ScoreData.Count != 3)
				{
					LogManager.WriteLog(2, string.Format("势力战分数信息异常 CityID={0}", scene.GameId), null, true);
				}
				else
				{
					foreach (CompBattleSideScore compBattleSideScore in scene.ScoreData)
					{
						compBattleSideScore.Rate = 0.0;
						foreach (int key in compBattleSideScore.StrongholdSet)
						{
							CompStrongholdConfig compStrongholdConfig = null;
							if (this.RuntimeData.CompStrongholdConfigDict.TryGetValue(key, out compStrongholdConfig))
							{
								compBattleSideScore.Rate += compStrongholdConfig.Rate;
							}
						}
					}
					scene.ScoreData.Sort(delegate(CompBattleSideScore left, CompBattleSideScore right)
					{
						int result;
						if (left.Rate > right.Rate)
						{
							result = -1;
						}
						else if (left.Rate < right.Rate)
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
					List<CompStrongholdData> list = new List<CompStrongholdData>(new CompStrongholdData[3]);
					for (int i = 0; i < scene.ScoreData.Count; i++)
					{
						CompBattleSideScore compBattleSideScore2 = scene.ScoreData[i];
						compBattleSideScore2.Rank = i + 1;
						CompStrongholdData compStrongholdData = new CompStrongholdData();
						compStrongholdData.Rank = compBattleSideScore2.Rank;
						compStrongholdData.StrongholdSet = new HashSet<int>(compBattleSideScore2.StrongholdSet);
						list[compBattleSideScore2.CompType - 1] = compStrongholdData;
					}
					if (update)
					{
						TianTiClient.getInstance().Comp_UpdateStrongholdData(scene.SceneInfo.ID, list);
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, scene.ScoreData, scene.CopyMap);
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
		}

		public int CalTopOwnCityCompType()
		{
			int result = 0;
			int num = 0;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count != 3)
				{
					return result;
				}
				for (int i = 1; i <= 3; i++)
				{
					CompBattleOwnCity compBattleOwnCity = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[i - 1];
					if (compBattleOwnCity.OwnCityList.Count == num)
					{
						result = 0;
					}
					if (compBattleOwnCity.OwnCityList.Count > num)
					{
						result = i;
						num = compBattleOwnCity.OwnCityList.Count;
					}
				}
			}
			return result;
		}

		public bool CheckCompShopDuiHuanType(GameClient client, int nDuiHuanType)
		{
			try
			{
				CompBattleGameStates compBattleGameStates = 0;
				this.CheckCondition(null, ref compBattleGameStates);
				if (0 != compBattleGameStates)
				{
					return false;
				}
				if (nDuiHuanType == CompManager.getInstance().GetCompShopDuiHuanType(CompShopDHTypeIndex.CSDH_CompBattleTop))
				{
					if (client.ClientData.CompType == this.CalTopOwnCityCompType())
					{
						return true;
					}
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.compBattleBaseData.CompBattleOwnCityList.Count != 3)
						{
							return false;
						}
						CompBattleOwnCity compBattleOwnCity = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[client.ClientData.CompType - 1];
						foreach (int key in compBattleOwnCity.OwnCityList)
						{
							CompBattleConfig compBattleConfig = null;
							if (this.RuntimeData.CompBattleConfigDict.TryGetValue(key, out compBattleConfig))
							{
								if (compBattleConfig.DuiHuanType == nDuiHuanType)
								{
									return true;
								}
							}
						}
					}
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, "", false, false);
			}
			return false;
		}

		public void InstallJunQi(CompBattleScene scene, int comptype, CompStrongholdConfig item, bool updateScoreRank = true)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = comptype;
				int monsterID = item.QiZhiID[comptype - 1];
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, 52, item, null);
				CompBattleSideScore compBattleSideScore = scene.ScoreData.Find((CompBattleSideScore x) => x.CompType == comptype);
				if (null != compBattleSideScore)
				{
					compBattleSideScore.StrongholdSet.Add(item.ID);
				}
				if (updateScoreRank)
				{
					this.UpdateBattleSideScoreRank(scene, true);
				}
			}
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			CompStrongholdConfig compStrongholdConfig = null;
			bool flag = false;
			bool flag2 = false;
			CompBattleScene compBattleScene = client.SceneObject as CompBattleScene;
			bool result;
			if (null == compBattleScene)
			{
				result = flag;
			}
			else
			{
				Dictionary<int, CompConfig> dictionary = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					dictionary = CompManager.getInstance().RuntimeData.CompConfigDict;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (compBattleScene.CompStrongholdConfigDict.TryGetValue(npcExtentionID, out compStrongholdConfig))
					{
						flag = true;
						if (compStrongholdConfig.Alive)
						{
							return flag;
						}
						if (client.ClientData.BattleWhichSide != compStrongholdConfig.BattleWhichSide && Math.Abs(TimeUtil.NOW() - compStrongholdConfig.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (compBattleScene.m_eStatus == 2)
						{
							if (Math.Abs(client.ClientData.PosX - compStrongholdConfig.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - compStrongholdConfig.PosY) <= 1000)
							{
								flag2 = true;
							}
						}
					}
					if (flag2)
					{
						if (compStrongholdConfig.BattleWhichSideLast != client.ClientData.CompType)
						{
							CompConfig compConfig = null;
							CompConfig compConfig2 = null;
							dictionary.TryGetValue(compStrongholdConfig.BattleWhichSideLast, out compConfig);
							dictionary.TryGetValue(client.ClientData.CompType, out compConfig2);
							if (compConfig != null && null != compConfig2)
							{
								string bulletinText = string.Format(GLang.GetLang(5005, new object[0]), compConfig.CompName, compStrongholdConfig.Name, compConfig2.CompName);
								BulletinMsgData bulletinMsgData = new BulletinMsgData
								{
									MsgID = "comp-battle-install-junqi",
									PlayMinutes = -1,
									ToPlayNum = -1,
									BulletinText = bulletinText,
									BulletinTicks = TimeUtil.NOW(),
									playingNum = 0
								};
								List<GameClient> clientsList = compBattleScene.CopyMap.GetClientsList();
								if (clientsList != null && clientsList.Count > 0)
								{
									for (int i = 0; i < clientsList.Count; i++)
									{
										GameClient gameClient = clientsList[i];
										if (gameClient != null)
										{
											GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, bulletinMsgData);
										}
									}
								}
							}
						}
						this.InstallJunQi(compBattleScene, client.ClientData.CompType, compStrongholdConfig, true);
					}
				}
				result = flag;
			}
			return result;
		}

		private void InitScene(CompBattleScene scene, GameClient client, Dictionary<int, KFCompData> compDataDict)
		{
			foreach (CompStrongholdConfig compStrongholdConfig in this.RuntimeData.CompStrongholdConfigDict.Values)
			{
				if (scene.m_nMapCode == compStrongholdConfig.MapCode)
				{
					scene.CompStrongholdConfigDict.Add(compStrongholdConfig.QiZuoID, compStrongholdConfig.Clone() as CompStrongholdConfig);
				}
			}
			for (int i = 1; i <= 3; i++)
			{
				KFCompData kfcompData = null;
				CompStrongholdData compStrongholdData = null;
				CompBattleSideScore compBattleSideScore = new CompBattleSideScore();
				compBattleSideScore.CompType = i;
				if (compDataDict.TryGetValue(i, out kfcompData) && kfcompData.StrongholdDict.TryGetValue(scene.SceneInfo.ID, out compStrongholdData))
				{
					compBattleSideScore.Rank = compStrongholdData.Rank;
					compBattleSideScore.StrongholdSet = compStrongholdData.StrongholdSet;
					foreach (int key in compStrongholdData.StrongholdSet)
					{
						CompStrongholdConfig compStrongholdConfig2 = null;
						if (this.RuntimeData.CompStrongholdConfigDict.TryGetValue(key, out compStrongholdConfig2))
						{
							if (scene.CompStrongholdConfigDict.TryGetValue(compStrongholdConfig2.QiZuoID, out compStrongholdConfig2))
							{
								compStrongholdConfig2.BattleWhichSideLast = i;
								this.InstallJunQi(scene, i, compStrongholdConfig2, false);
							}
						}
					}
				}
				scene.ScoreData.Add(compBattleSideScore);
			}
			this.UpdateBattleSideScoreRank(scene, false);
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 52)
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
						CompBattleScene compBattleScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out compBattleScene))
						{
							CompBattleConfig compBattleConfig = null;
							CompFuBenData compFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out compFuBenData))
							{
								LogManager.WriteLog(2, "势力战场没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
							}
							if (!this.RuntimeData.CompBattleConfigDict.TryGetValue(compFuBenData.GameId, out compBattleConfig))
							{
								LogManager.WriteLog(2, "势力战场没有为副本找到对应的档位数据,ID:" + compFuBenData.GameId, null, true);
							}
							compBattleScene = new CompBattleScene();
							compBattleScene.CopyMap = copyMap;
							compBattleScene.CleanAllInfo();
							compBattleScene.GameId = num;
							compBattleScene.m_nMapCode = mapCode;
							compBattleScene.CopyMapId = copyMap.CopyMapID;
							compBattleScene.FuBenSeqId = fuBenSeqID;
							compBattleScene.SceneInfo = compBattleConfig;
							compBattleScene.MapGridWidth = gameMap.MapGridWidth;
							compBattleScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(compBattleConfig.ID));
							compBattleScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(compBattleScene, client, compDataDict);
							this.SceneDict[fuBenSeqID] = compBattleScene;
						}
						CompBattleClientContextData compBattleClientContextData;
						if (!compBattleScene.ClientContextDataDict.TryGetValue(roleID, out compBattleClientContextData))
						{
							compBattleClientContextData = new CompBattleClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID,
								CompZhiWu = (int)client.ClientData.CompZhiWu
							};
							compBattleScene.ClientContextDataDict[roleID] = compBattleClientContextData;
						}
						else
						{
							compBattleClientContextData.KillNum = 0;
						}
						client.SceneObject = compBattleScene;
						client.SceneGameId = (long)compBattleScene.GameId;
						client.SceneContextData2 = compBattleClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(compBattleScene.SceneInfo.TotalSecs * 1000));
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, compBattleScene.SceneInfo.ID, roleID, (int)client.ClientData.CompZhiWu, 5);
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
			if (sceneType == 52)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleScene compBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out compBattleScene);
					this.RuntimeData.FuBenItemData.Remove(compBattleScene.GameId);
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
			if (num >= CompBattleManager.NextHeartBeatTicks)
			{
				CompBattleManager.NextHeartBeatTicks = num + 1020L;
				foreach (CompBattleScene compBattleScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = compBattleScene.FuBenSeqId;
						int copyMapId = compBattleScene.CopyMapId;
						int nMapCode = compBattleScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = compBattleScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (compBattleScene.m_eStatus == 1 || compBattleScene.m_eStatus == 2)
							{
							}
							if (compBattleScene.m_eStatus == 0)
							{
								if (num2 >= compBattleScene.StartTimeTicks)
								{
									compBattleScene.m_lPrepareTime = compBattleScene.StartTimeTicks;
									compBattleScene.m_lBeginTime = compBattleScene.m_lPrepareTime + (long)(compBattleScene.SceneInfo.PrepareSecs * 1000);
									compBattleScene.m_eStatus = 1;
									compBattleScene.StateTimeData.GameType = 30;
									compBattleScene.StateTimeData.State = compBattleScene.m_eStatus;
									compBattleScene.StateTimeData.EndTicks = compBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, compBattleScene.StateTimeData, compBattleScene.CopyMap);
								}
							}
							else if (compBattleScene.m_eStatus == 1)
							{
								if (num2 >= compBattleScene.m_lBeginTime)
								{
									compBattleScene.m_eStatus = 2;
									compBattleScene.m_lEndTime = compBattleScene.m_lBeginTime + (long)(compBattleScene.SceneInfo.FightingSecs * 1000);
									compBattleScene.StateTimeData.GameType = 30;
									compBattleScene.StateTimeData.State = compBattleScene.m_eStatus;
									compBattleScene.StateTimeData.EndTicks = compBattleScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, compBattleScene.StateTimeData, compBattleScene.CopyMap);
									for (int i = 1; i <= 3; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (compBattleScene.m_eStatus == 2)
							{
								if (num2 >= compBattleScene.m_lEndTime)
								{
									this.ProcessEnd(compBattleScene, num2);
								}
							}
							else if (compBattleScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(compBattleScene.CopyMap);
								compBattleScene.m_eStatus = 4;
								TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, -1, compBattleScene.SceneInfo.ID, -1, -1, 6);
								this.GiveAwards(compBattleScene);
								CompFuBenData compFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(compBattleScene.GameId, out compFuBenData))
								{
									compFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("势力战场跨服副本GameID={0},战斗结束", compFuBenData.GameId), null, true);
								}
							}
							else if (compBattleScene.m_eStatus == 4)
							{
								if (num2 >= compBattleScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(compBattleScene.m_lLeaveTime);
									compBattleScene.m_eStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "势力战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene compBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compBattleScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, compBattleScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<List<CompBattleSideScore>>(2005, compBattleScene.ScoreData, false);
					}
				}
			}
		}

		private CompBattleRewardConfig CalBattleRewardConfig(GameClient client, KFCompRoleData compRoleData)
		{
			CompBattleRewardConfig result = null;
			double num = 0.0;
			lock (CompManager.getInstance().RuntimeData.Mutex)
			{
				if (CompManager.getInstance().CompSyncDataCache.CompBattleJoinRoleNum.Length != 0)
				{
					int num2 = CompManager.getInstance().CompSyncDataCache.CompBattleJoinRoleNum[client.ClientData.CompType - 1];
					num = ((num2 > 0) ? ((double)compRoleData.BattleRankNum / (double)num2) : 1.0);
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				foreach (CompBattleRewardConfig compBattleRewardConfig in this.RuntimeData.CompBattleRewardConfigList)
				{
					if (compBattleRewardConfig.Rank > 0 && compBattleRewardConfig.Rank == compRoleData.BattleRankNum)
					{
						result = compBattleRewardConfig;
						break;
					}
					if (compBattleRewardConfig.RankRate > 0.0 && num <= compBattleRewardConfig.RankRate)
					{
						result = compBattleRewardConfig;
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
				CompBattleRewardConfig compBattleRewardConfig = this.CalBattleRewardConfig(client, kfcompRoleData);
				if (null == compBattleRewardConfig)
				{
					result = -3;
				}
				else
				{
					int count = this.RuntimeData.compBattleBaseData.CompBattleOwnCityList[kfcompRoleData.CompType - 1].OwnCityList.Count;
					double num = 1.0;
					if (count > 0 && count <= this.RuntimeData.CompBattleRewardRate.Length)
					{
						lock (this.RuntimeData.Mutex)
						{
							num = this.RuntimeData.CompBattleRewardRate[count - 1];
						}
					}
					int num2 = (int)((double)compBattleRewardConfig.Grade * num);
					int num3 = (int)((double)compBattleRewardConfig.Contribution * num);
					List<AwardsItemData> items = compBattleRewardConfig.AwardsItemListOne.Items;
					List<AwardsItemData> items2 = compBattleRewardConfig.AwardsItemListTwo.Items;
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
							GameManager.ClientMgr.ModifyCompDonateValue(client, num3, "势力战场奖励", true, true, false);
						}
						if (items != null)
						{
							foreach (AwardsItemData awardsItemData in items)
							{
								int num5 = (int)((double)awardsItemData.GoodsNum * num);
								if (num5 > 0)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, num5, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "势力战场奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
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
										Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, num5, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "势力战场奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
									}
								}
							}
						}
						result = 1;
					}
				}
			}
			return result;
		}

		private void UpdateMapBuffer(CompBattleScene scene, GameClient client, bool login)
		{
			if (client.ClientData.CompType > 0)
			{
				List<CompLevelConfig> list = null;
				lock (CompManager.getInstance().RuntimeData.Mutex)
				{
					list = CompManager.getInstance().RuntimeData.CompLevelConfigList;
				}
				if (login)
				{
					CompFuBenData compFuBenData;
					if (!this.RuntimeData.FuBenItemData.TryGetValue(scene.GameId, out compFuBenData))
					{
						return;
					}
					HashSet<int> hashSet = null;
					if (compFuBenData.ZhuJiangRoleDict.TryGetValue(client.ClientData.CompType, out hashSet) && null != hashSet)
					{
						foreach (int roleID in hashSet)
						{
							GameClient zhujiangClient = GameManager.ClientMgr.FindClient(roleID);
							if (null != zhujiangClient)
							{
								CompLevelConfig compLevelConfig = list.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)zhujiangClient.ClientData.CompZhiWu);
								if (null != compLevelConfig)
								{
									BufferItemTypes bufferItem = BufferItemTypes.CompBattle_Self + (int)zhujiangClient.ClientData.CompZhiWu;
									CompManager.getInstance().UpdateBuff4GameClient(client, bufferItem, compLevelConfig.CraftBuffID, true);
								}
							}
						}
					}
				}
				if (client.ClientData.CompZhiWu > 0)
				{
					CompLevelConfig compLevelConfig = list.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
					if (null != compLevelConfig)
					{
						if (login)
						{
							CompManager.getInstance().UpdateBuff4GameClient(client, BufferItemTypes.CompBattle_Self, compLevelConfig.CraftSelfBuffID, true);
						}
						List<GameClient> clientsList = scene.CopyMap.GetClientsList();
						if (clientsList != null && clientsList.Count > 0)
						{
							for (int i = 0; i < clientsList.Count; i++)
							{
								GameClient gameClient = clientsList[i];
								if (gameClient != null && gameClient.ClientData.CompType == client.ClientData.CompType)
								{
									BufferItemTypes bufferItem = BufferItemTypes.CompBattle_Self + (int)client.ClientData.CompZhiWu;
									CompManager.getInstance().UpdateBuff4GameClient(gameClient, bufferItem, compLevelConfig.CraftBuffID, login);
								}
							}
						}
					}
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene compBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compBattleScene))
				{
					CompFuBenData compFuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(compBattleScene.GameId, out compFuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompBattleClientContextData compBattleClientContextData = client.SceneContextData2 as CompBattleClientContextData;
							compBattleClientContextData.CompZhiWu = (int)client.ClientData.CompZhiWu;
							compBattleClientContextData.Online = true;
							compFuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Add(client.ClientData.RoleID);
							CompBattleSideScore compBattleSideScore = compBattleScene.ScoreData.Find((CompBattleSideScore x) => x.CompType == client.ClientData.CompType);
							if (null != compBattleSideScore)
							{
								compBattleSideScore.ZhuJiangNum++;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, compBattleScene.ScoreData, compBattleScene.CopyMap);
						}
						this.UpdateMapBuffer(compBattleScene, client, true);
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
				CompBattleScene compBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compBattleScene))
				{
					CompFuBenData compFuBenData;
					if (this.RuntimeData.FuBenItemData.TryGetValue(compBattleScene.GameId, out compFuBenData))
					{
						if (client.ClientData.CompZhiWu > 0)
						{
							CompBattleClientContextData compBattleClientContextData = client.SceneContextData2 as CompBattleClientContextData;
							compBattleClientContextData.CompZhiWu = (int)client.ClientData.CompZhiWu;
							compBattleClientContextData.Online = false;
							bool flag2 = compFuBenData.ZhuJiangRoleDict[client.ClientData.CompType].Remove(client.ClientData.RoleID);
							CompBattleSideScore compBattleSideScore = compBattleScene.ScoreData.Find((CompBattleSideScore x) => x.CompType == client.ClientData.CompType);
							if (flag2 && null != compBattleSideScore)
							{
								compBattleSideScore.ZhuJiangNum--;
							}
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<CompBattleSideScore>>(2005, compBattleScene.ScoreData, compBattleScene.CopyMap);
						}
						this.UpdateMapBuffer(compBattleScene, client, false);
						List<int> roleCountSideList;
						int index;
						(roleCountSideList = compFuBenData.RoleCountSideList)[index = client.ClientData.CompType - 1] = roleCountSideList[index] - 1;
						TianTiClient.getInstance().Comp_GameFuBenRoleChangeState(30, client.ServerId, compBattleScene.SceneInfo.ID, client.ClientData.RoleID, (int)client.ClientData.CompZhiWu, 7);
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompBattleScene compBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out compBattleScene))
				{
					if (compBattleScene.m_eStatus == 2)
					{
						int num = this.RuntimeData.CompBattleSingleIntegral[0];
						TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 8, client.ClientData.RoleID, num);
						GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, num, "势力战杀人", true, true, false);
						CompBattleClientContextData compBattleClientContextData = client.SceneContextData2 as CompBattleClientContextData;
						CompBattleClientContextData compBattleClientContextData2 = other.SceneContextData2 as CompBattleClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						if (null != compBattleClientContextData)
						{
							compBattleClientContextData.KillNum++;
							compBattleClientContextData.BattleJiFen += num;
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(compBattleClientContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							if (compBattleClientContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != compBattleClientContextData2)
						{
							if (compBattleClientContextData2.KillNum >= 10)
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
							}
							compBattleClientContextData2.KillNum = 0;
						}
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(2007, huanYingSiYuanLianSha, compBattleScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(2008, huanYingSiYuanLianshaOver, compBattleScene.CopyMap);
						}
					}
				}
			}
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompBattleScene compBattleScene = client.SceneObject as CompBattleScene;
			CompStrongholdConfig tagInfo = monster.Tag as CompStrongholdConfig;
			if (tagInfo != null && null != compBattleScene)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleSideScore compBattleSideScore = compBattleScene.ScoreData.Find((CompBattleSideScore x) => x.CompType == tagInfo.BattleWhichSide);
					if (null != compBattleSideScore)
					{
						compBattleSideScore.StrongholdSet.Remove(tagInfo.ID);
					}
					tagInfo.DeadTicks = TimeUtil.NOW();
					tagInfo.Alive = false;
					tagInfo.BattleWhichSideLast = tagInfo.BattleWhichSide;
					tagInfo.BattleWhichSide = client.ClientData.BattleWhichSide;
					this.UpdateBattleSideScoreRank(compBattleScene, true);
				}
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			CompStrongholdConfig compStrongholdConfig = monster.Tag as CompStrongholdConfig;
			if (null != compStrongholdConfig)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompBattleScene compBattleScene = client.SceneObject as CompBattleScene;
					if (compBattleScene != null && compBattleScene.m_eStatus == 2)
					{
						CompBattleClientContextData compBattleClientContextData = client.SceneContextData2 as CompBattleClientContextData;
						if (null != compBattleClientContextData)
						{
							compBattleClientContextData.BattleJiFen += compStrongholdConfig.Point;
							TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 8, client.ClientData.RoleID, compStrongholdConfig.Point);
							GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, compStrongholdConfig.Point, "势力战打旗", true, true, false);
						}
					}
				}
			}
		}

		public void CompleteScene(CompBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		private void ProcessEnd(CompBattleScene scene, long nowTicks)
		{
			int successSide = (scene.ScoreData[0].Rate > 0.0) ? scene.ScoreData[0].CompType : 0;
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = 3;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 30;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void GiveAwards(CompBattleScene scene)
		{
			try
			{
				foreach (CompBattleClientContextData compBattleClientContextData in scene.ClientContextDataDict.Values)
				{
					if (compBattleClientContextData.BattleJiFen > 0)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(compBattleClientContextData.RoleId);
						string text = string.Format("{0}", scene.StartTimeTicks);
						if (gameClient != null)
						{
							Global.SaveRoleParamsStringToDB(gameClient, "49", text, true);
						}
						else
						{
							Global.UpdateRoleParamByNameOffline(compBattleClientContextData.RoleId, "49", text, compBattleClientContextData.ServerId);
						}
					}
				}
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "势力战场系统奖励异常");
			}
		}

		public const SceneUIClasses ManagerType = 52;

		private static CompBattleManager instance = new CompBattleManager();

		public CompBattleRuntimeData RuntimeData = new CompBattleRuntimeData();

		public ConcurrentDictionary<int, CompBattleScene> SceneDict = new ConcurrentDictionary<int, CompBattleScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
