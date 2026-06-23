using System;
using System.Collections.Generic;
using System.Linq;
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
	public class ZhengDuoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx, IEventListener, ICopySceneManager
	{
		public static ZhengDuoManager getInstance()
		{
			return ZhengDuoManager.instance;
		}

		public bool initialize()
		{
			bool result;
			if (!this.ConfigData.Load(Global.GameResPath("Config\\PlunderLands.xml"), Global.GameResPath("Config\\PlunderLandsMonster.xml"), Global.GameResPath("Config\\PlunderLandsRebirth.xml")))
			{
				result = false;
			}
			else
			{
				this.ConfigData.AwardHurtMin = (int)GameManager.systemParamsList.GetParamValueIntByName("PlunderLandsLowest", -1);
				this.ConfigData.ZhengDuoMapList = StringUtil.StringToIntList(GameManager.systemParamsList.GetParamValueByName("PlunderLandsEnterMap"), ',');
				result = true;
			}
			return result;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1070, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1071, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1073, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1074, 1, 1, ZhengDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(17, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, ZhengDuoManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ZhengDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(17, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, ZhengDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, ZhengDuoManager.getInstance());
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
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = true;
			}
			else
			{
				switch (nID)
				{
				case 1070:
					return this.HandleZhengDuoData(client, nID, bytes, cmdParams);
				case 1071:
					return this.HandleZhengDuoSign(client, nID, bytes, cmdParams);
				case 1073:
					return this.HandleZhengDuoEnter(client, nID, bytes, cmdParams);
				case 1074:
					return this.HandleZhengDuoRankList(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		private bool HandleZhengDuoData(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			ZhengDuoData zhengDuoData = new ZhengDuoData();
			int faction = client.ClientData.Faction;
			long num = TimeUtil.NOW();
			if (faction > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					zhengDuoData.Step = this.RuntimeData.ZhengDuoStep;
					zhengDuoData.State = this.RuntimeData.State;
					client.ClientData.ZhengDuoDataAge = this.RuntimeData.Age;
					ZhengDuoRankData zhengDuoRankData;
					if (this.RuntimeData.ZhengDuoStep == 1 && this.RuntimeData.State == 1)
					{
						ZhengDuoSignUpData zhengDuoSignUpData;
						if (this.RuntimeData.FightDataDict.TryGetValue(client.ClientData.Faction, out zhengDuoSignUpData) && zhengDuoSignUpData.Week == this.RuntimeData.WeekDay)
						{
							zhengDuoData.SignUp = 1;
							if (num >= zhengDuoSignUpData.StartTicks + (long)zhengDuoSignUpData.UsedTicks)
							{
								zhengDuoData.State = 0;
							}
						}
					}
					else if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(faction, out zhengDuoRankData))
					{
						zhengDuoData.SignUp = 1;
						zhengDuoData.Lose = zhengDuoRankData.Lose;
						if (this.RuntimeData.ZhengDuoStep >= 1 && (this.RuntimeData.ZhengDuoStep < 5 || this.RuntimeData.State > 0))
						{
							zhengDuoData.State = (this.RuntimeData.State & zhengDuoRankData.State);
							if (zhengDuoRankData.Enemy > 0)
							{
								ZhengDuoRankData zhengDuoRankData2;
								if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(zhengDuoRankData.Enemy, out zhengDuoRankData2))
								{
									zhengDuoData.OtherName = zhengDuoRankData2.BhName;
									zhengDuoData.OtherZoneId = zhengDuoRankData2.ZoneId;
								}
							}
						}
						else if (this.RuntimeData.Rank1Bhid > 0)
						{
							ZhengDuoRankData zhengDuoRankData2;
							if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(this.RuntimeData.Rank1Bhid, out zhengDuoRankData2))
							{
								zhengDuoData.OtherName = zhengDuoRankData2.BhName;
								zhengDuoData.OtherZoneId = zhengDuoRankData2.ZoneId;
							}
						}
					}
					else
					{
						int[] array = Global.sendToDB<int[], int>(10224, faction, client.ServerId);
						if (array != null && array.Length >= 2 && array[0] == this.RuntimeData.WeekDay)
						{
							zhengDuoData.SignUp = 1;
							zhengDuoData.Lose = 1;
						}
						if (this.RuntimeData.Rank1Bhid > 0)
						{
							ZhengDuoRankData zhengDuoRankData2;
							if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(this.RuntimeData.Rank1Bhid, out zhengDuoRankData2))
							{
								zhengDuoData.OtherName = zhengDuoRankData2.BhName;
								zhengDuoData.OtherZoneId = zhengDuoRankData2.ZoneId;
							}
						}
					}
				}
			}
			client.sendCmd<ZhengDuoData>(nID, zhengDuoData, false);
			return true;
		}

		private bool HandleZhengDuoSign(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = -4007;
			if (!GlobalNew.IsGongNengOpened(client, 84, false))
			{
				cmdData = -4007;
			}
			else
			{
				int faction = client.ClientData.Faction;
				if (faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					cmdData = -1002;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.ZhengDuoStep == 1)
						{
							int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
							ZhengDuoSignUpData zhengDuoSignUpData;
							if (this.RuntimeData.FightDataDict.TryGetValue(faction, out zhengDuoSignUpData) && zhengDuoSignUpData.Week == weekStartDayIdNow)
							{
								cmdData = -4005;
							}
							else
							{
								zhengDuoSignUpData = new ZhengDuoSignUpData();
								zhengDuoSignUpData.Bhid = faction;
								zhengDuoSignUpData.StartTicks = TimeUtil.NOW();
								zhengDuoSignUpData.UsedTicks = 1800000;
								zhengDuoSignUpData.Week = weekStartDayIdNow;
								this.RuntimeData.FightDataDict[faction] = zhengDuoSignUpData;
								cmdData = 1;
								GameManager.ClientMgr.SendBangHuiCmd<int>(zhengDuoSignUpData.Bhid, 1072, this.RuntimeData.ZhengDuoStep, true, true);
								EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
								{
									0,
									weekStartDayIdNow,
									GameManager.ServerId,
									zhengDuoSignUpData.Bhid,
									"争夺之地报名"
								});
							}
						}
					}
				}
			}
			client.sendCmd<int>(nID, cmdData, false);
			return true;
		}

		private bool HandleZhengDuoEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int cmdData = 1;
			int faction = client.ClientData.Faction;
			lock (this.RuntimeData.Mutex)
			{
				if (!GlobalNew.IsGongNengOpened(client, 84, false))
				{
					cmdData = -4007;
				}
				else if (this.RuntimeData.ZhengDuoStep == 0 || this.RuntimeData.State == 0)
				{
					cmdData = -4007;
				}
				else
				{
					SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
					if (mapSceneType != 0)
					{
						cmdData = -21;
					}
					else
					{
						long num = TimeUtil.NOW();
						int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
						if (this.RuntimeData.ZhengDuoStep == 1)
						{
							ZhengDuoSignUpData zhengDuoSignUpData;
							ZhengDuoSceneInfo zhengDuoSceneInfo;
							if (!this.RuntimeData.FightDataDict.TryGetValue(faction, out zhengDuoSignUpData))
							{
								cmdData = -4008;
							}
							else if (zhengDuoSignUpData.Week == weekStartDayIdNow && num >= zhengDuoSignUpData.StartTicks + (long)zhengDuoSignUpData.UsedTicks)
							{
								cmdData = -4006;
							}
							else if (this.ConfigData.SceneDataDict.TryGetValue(1, out zhengDuoSceneInfo))
							{
								cmdData = 1;
								if (zhengDuoSignUpData.FuBenSeqId == 0)
								{
									zhengDuoSignUpData.FuBenSeqId = FuBenManager.GetFuBenSeqId(0);
								}
								client.ClientData.BattleWhichSide = 1;
								int maxX;
								int mapY;
								this.GetBirthPoint(client, out maxX, out mapY);
								client.SceneInfoObject = zhengDuoSceneInfo;
								client.ClientData.FuBenSeqID = zhengDuoSignUpData.FuBenSeqId;
								FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, zhengDuoSignUpData.FuBenSeqId, 0, 0);
								client.ClientData.WaitingChangeMapToMapCode = zhengDuoSceneInfo.MapCode;
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, zhengDuoSceneInfo.MapCode, maxX, mapY, -1, 0);
							}
							else
							{
								cmdData = -3;
							}
						}
						else
						{
							ZhengDuoFuBenData kuaFuFuBenDataByBhid = this.GetKuaFuFuBenDataByBhid(faction, weekStartDayIdNow);
							if (kuaFuFuBenDataByBhid != null && kuaFuFuBenDataByBhid.State == 2)
							{
								KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
								if (null != clientKuaFuServerLoginData)
								{
									clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
									clientKuaFuServerLoginData.GameId = kuaFuFuBenDataByBhid.GameId;
									clientKuaFuServerLoginData.GameType = 17;
									clientKuaFuServerLoginData.EndTicks = kuaFuFuBenDataByBhid.EndTime.Ticks;
									clientKuaFuServerLoginData.ServerId = GameManager.ServerId;
									KuaFuServerInfo kuaFuServerInfo;
									if (!KuaFuManager.getInstance().TryGetValue(kuaFuFuBenDataByBhid.ServerId, out kuaFuServerInfo))
									{
										LogManager.WriteLog(2, string.Format("进入争夺之地时找不到跨服服务器的IP和端口信息#serverid={0}", kuaFuFuBenDataByBhid.ServerId), null, true);
										cmdData = -11000;
									}
									else
									{
										clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
										clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
									}
								}
							}
							else
							{
								cmdData = -4006;
							}
						}
					}
				}
			}
			client.sendCmd<int>(nID, cmdData, false);
			return true;
		}

		private bool HandleZhengDuoRankList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			ZhengDuoRankList zhengDuoRankList = new ZhengDuoRankList();
			int faction = client.ClientData.Faction;
			if (faction > 0)
			{
				int[] array = Global.sendToDB<int[], int>(10224, faction, client.ServerId);
				lock (this.RuntimeData.Mutex)
				{
					if (array != null && array.Length >= 2 && array[0] == this.RuntimeData.WeekDay)
					{
						zhengDuoRankList.UsedMillisecond = array[1];
					}
					zhengDuoRankList.RankList = new List<ZhengDuoRankData>();
					foreach (ZhengDuoRankData zhengDuoRankData in this.RuntimeData.ZhengDuoRankDatas)
					{
						if (null != zhengDuoRankData)
						{
							zhengDuoRankList.RankList.Add(zhengDuoRankData);
						}
					}
				}
			}
			client.sendCmd<ZhengDuoRankList>(nID, zhengDuoRankList, false);
			return true;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 23:
			{
				PreBangHuiAddMemberEventObject preBangHuiAddMemberEventObject = eventObject as PreBangHuiAddMemberEventObject;
				if (null != preBangHuiAddMemberEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiAddMember(preBangHuiAddMemberEventObject);
				}
				break;
			}
			case 24:
			{
				PreBangHuiRemoveMemberEventObject preBangHuiRemoveMemberEventObject = eventObject as PreBangHuiRemoveMemberEventObject;
				if (null != preBangHuiRemoveMemberEventObject)
				{
					eventObject.Handled = this.OnPreBangHuiRemoveMember(preBangHuiRemoveMemberEventObject);
				}
				break;
			}
			}
		}

		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (this.IsActivityState(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(549, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (this.IsActivityState(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(550, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType != 11)
			{
				if (eventType == 17)
				{
					this.HandleMonsterInjured(((MonsterInjuredEventObject)eventObject).getAttacker(), ((MonsterInjuredEventObject)eventObject).getMonster(), ((MonsterInjuredEventObject)eventObject).injure);
				}
			}
			else
			{
				this.HandleMonsterDead(((MonsterDeadEventObject)eventObject).getAttacker(), ((MonsterDeadEventObject)eventObject).getMonster());
			}
		}

		private void HandleMonsterDead(GameClient player, Monster monster)
		{
			ZhengDuoScene zhengDuoScene = player.SceneObject as ZhengDuoScene;
			if (zhengDuoScene != null)
			{
				if (monster.Tag is ZhengDuoMonsterInfo)
				{
					zhengDuoScene.KillerId = player.ClientData.RoleID;
					zhengDuoScene.KillUsedTicks = (int)(TimeUtil.NOW() - zhengDuoScene.StartTimeTicks);
					zhengDuoScene.m_lEndTime = TimeUtil.NOW();
				}
			}
		}

		private void HandleMonsterInjured(GameClient client, Monster monster, int injure)
		{
			ZhengDuoScene zhengDuoScene = client.SceneObject as ZhengDuoScene;
			if (zhengDuoScene != null)
			{
				if (monster.Tag is ZhengDuoMonsterInfo)
				{
					lock (zhengDuoScene.ClientContextDataDict)
					{
						ZhengDuoScoreData zhengDuoScoreData;
						if (!zhengDuoScene.ClientContextDataDict.TryGetValue(client.ClientData.RoleID, out zhengDuoScoreData))
						{
							zhengDuoScoreData = new ZhengDuoScoreData(client.ClientData.RoleID, client.ClientData.RoleName, 0L);
						}
						zhengDuoScoreData.Score += (long)injure;
						if (zhengDuoScene.PreliminarisesMode)
						{
							bool flag2 = false;
							for (int i = 0; i < zhengDuoScene.ScoreData.Length; i++)
							{
								ZhengDuoScoreData zhengDuoScoreData2 = zhengDuoScene.ScoreData[i];
								if (zhengDuoScoreData2 != null && zhengDuoScoreData2.Id == client.ClientData.RoleID)
								{
									zhengDuoScoreData2.Score = zhengDuoScoreData.Score;
									flag2 = true;
								}
							}
							if (!flag2)
							{
								if (ZhengDuoScoreData.Compare_static(zhengDuoScene.ScoreData[2], zhengDuoScoreData) > 0)
								{
									zhengDuoScene.ScoreData[3] = new ZhengDuoScoreData(zhengDuoScoreData.Id, client.ClientData.RoleName, zhengDuoScoreData.Score);
									flag2 = true;
								}
							}
							if (flag2)
							{
								Array.Sort<ZhengDuoScoreData>(zhengDuoScene.ScoreData, new Comparison<ZhengDuoScoreData>(ZhengDuoScoreData.Compare_static));
							}
						}
						else if (zhengDuoScene.ScoreData[0].Id == client.ClientData.Faction)
						{
							zhengDuoScene.ScoreData[0].Score += (long)injure;
						}
						else
						{
							zhengDuoScene.ScoreData[1].Score += (long)injure;
						}
					}
				}
			}
		}

		private bool IsActivityState(int bhid)
		{
			bool result = false;
			if (bhid > 0)
			{
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.State == 0)
					{
						return false;
					}
					ZhengDuoSignUpData zhengDuoSignUpData;
					if (this.RuntimeData.FightDataDict.TryGetValue(bhid, out zhengDuoSignUpData) && zhengDuoSignUpData.Week == this.RuntimeData.WeekDay && TimeUtil.NOW() < zhengDuoSignUpData.StartTicks + (long)zhengDuoSignUpData.UsedTicks)
					{
						result = true;
					}
					ZhengDuoRankData zhengDuoRankData;
					if (this.RuntimeData.bhid2ZhengDuoRankDataDict.TryGetValue(bhid, out zhengDuoRankData))
					{
						if (zhengDuoRankData.Week == this.RuntimeData.WeekDay && zhengDuoRankData.State > 0)
						{
							result = true;
						}
					}
				}
			}
			return result;
		}

		public void CheckTipsIconState(GameClient client)
		{
			bool bIconState = this.IsActivityState(client.ClientData.Faction);
			if (client._IconStateMgr.AddFlushIconState(15004, bIconState))
			{
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 40)
			{
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
				{
					result = false;
				}
				else
				{
					int fuBenSeqID = copyMap.FuBenSeqID;
					int mapCode = copyMap.MapCode;
					int roleID = client.ClientData.RoleID;
					DateTime dateTime = TimeUtil.NowDateTime();
					int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					ZhengDuoRankData zhengDuoRankData = null;
					ZhengDuoScoreData zhengDuoScoreData = null;
					lock (this.RuntimeData.Mutex)
					{
						ZhengDuoScene zhengDuoScene = null;
						ZhengDuoFuBenData zhengDuoFuBenData = null;
						if (!this.RuntimeData.SceneDict.TryGetValue(fuBenSeqID, out zhengDuoScene))
						{
							ZhengDuoSceneInfo zhengDuoSceneInfo = client.SceneInfoObject as ZhengDuoSceneInfo;
							zhengDuoScene = new ZhengDuoScene();
							zhengDuoScene.CopyMap = copyMap;
							zhengDuoScene.CleanAllInfo();
							zhengDuoScene.GameId = num;
							zhengDuoScene.m_nMapCode = mapCode;
							zhengDuoScene.CopyMapId = copyMap.CopyMapID;
							zhengDuoScene.FuBenSeqId = fuBenSeqID;
							zhengDuoScene.SceneInfo = zhengDuoSceneInfo;
							zhengDuoScene.MapGridWidth = gameMap.MapGridWidth;
							zhengDuoScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime startTime = this.GetStartTime(zhengDuoSceneInfo.Id);
							zhengDuoScene.StartTimeTicks = startTime.Ticks / 10000L;
							zhengDuoScene.m_lEndTime = zhengDuoScene.StartTimeTicks + (long)((zhengDuoSceneInfo.SecondWait + zhengDuoSceneInfo.SecondFight) * 1000);
							zhengDuoScene.PreliminarisesMode = (zhengDuoSceneInfo.Id == 1);
							this.RuntimeData.SceneDict[fuBenSeqID] = zhengDuoScene;
							if (zhengDuoSceneInfo.Id > 1)
							{
								if (!this.RuntimeData.FuBenItemData.TryGetValue((long)num, out zhengDuoFuBenData))
								{
									LogManager.WriteLog(2, "没有为跨服副本找到对应的跨服副本数据,GameID:" + num, null, true);
									return false;
								}
								if (zhengDuoFuBenData.State == 3)
								{
									LogManager.WriteLog(2, "跨服副本已经结束#GameID=" + num, null, true);
									return false;
								}
								foreach (KeyValuePair<int, int> keyValuePair in zhengDuoFuBenData.PlayerDict)
								{
									zhengDuoScene.ScoreData[keyValuePair.Value - 1] = new ZhengDuoScoreData(keyValuePair.Key, null, 0L);
								}
							}
							else
							{
								ZhengDuoSignUpData zhengDuoSignUpData;
								if (!this.RuntimeData.FightDataDict.TryGetValue(client.ClientData.Faction, out zhengDuoSignUpData))
								{
									LogManager.WriteLog(2, "没有为跨服副本找到对应的跨服副本数据,GameID:" + num, null, true);
									return false;
								}
								if (zhengDuoSignUpData.StartTicks + (long)zhengDuoSignUpData.UsedTicks <= TimeUtil.NOW())
								{
									LogManager.WriteLog(2, "跨服副本已经结束#GameID=" + num, null, true);
									return false;
								}
								zhengDuoScene.StartTimeTicks = zhengDuoSignUpData.StartTicks;
								client.ClientData.BattleWhichSide = 1;
							}
							zhengDuoScene.Start();
							this.InitCreateDynamicMonster(zhengDuoScene);
						}
						ZhengDuoScoreData zhengDuoScoreData2;
						if (!zhengDuoScene.ClientContextDataDict.TryGetValue(roleID, out zhengDuoScoreData2))
						{
							zhengDuoScoreData2 = new ZhengDuoScoreData(roleID, Global.FormatRoleName4(client), 0L);
							zhengDuoScene.ClientContextDataDict[roleID] = zhengDuoScoreData2;
						}
						client.SceneObject = zhengDuoScene;
						client.SceneContextData2 = zhengDuoScoreData2;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((zhengDuoScene.SceneInfo.TotalSecs + 120) * 1000));
						for (int i = 0; i < zhengDuoScene.RankDatas.Length; i++)
						{
							if (null == zhengDuoScene.RankDatas[i])
							{
								zhengDuoRankData = (zhengDuoScene.RankDatas[i] = new ZhengDuoRankData());
								zhengDuoRankData.Bhid = client.ClientData.Faction;
								break;
							}
							if (zhengDuoScene.RankDatas[i].Bhid == client.ClientData.Faction)
							{
								break;
							}
						}
						zhengDuoScoreData = zhengDuoScene.ScoreData[client.ClientData.BattleWhichSide - 1];
					}
					if (zhengDuoRankData != null && string.IsNullOrEmpty(zhengDuoRankData.BhName))
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(client.ClientData.RoleID, client.ClientData.Faction, client.ServerId);
						if (bangHuiDetailData != null)
						{
							zhengDuoRankData.Bhid = bangHuiDetailData.BHID;
							zhengDuoRankData.BhName = bangHuiDetailData.BHName;
							zhengDuoRankData.ZoneId = bangHuiDetailData.ZoneID;
							zhengDuoRankData.BhLevel = bangHuiDetailData.QiLevel;
							zhengDuoRankData.ServerID = GameManager.ServerId;
							zhengDuoRankData.ZhanLi = bangHuiDetailData.TotalCombatForce;
							zhengDuoRankData.Week = TimeUtil.GetWeekStartDayIdNow();
							if (null != zhengDuoScoreData)
							{
								zhengDuoScoreData.Name = Global.FormatBangHuiNameWithZone(bangHuiDetailData.ZoneID, bangHuiDetailData.BHName);
							}
						}
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
			if (sceneType == 40)
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoScene zhengDuoScene;
					this.RuntimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out zhengDuoScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private DateTime GetStartTime(int sceneId)
		{
			ZhengDuoSceneInfo zhengDuoSceneInfo = null;
			TimeSpan timeSpan = TimeUtil.GetTimeOfWeekNow();
			DateTime weekStartTimeNow = TimeUtil.GetWeekStartTimeNow();
			lock (this.RuntimeData.Mutex)
			{
				if (this.ConfigData.SceneDataDict.TryGetValue(sceneId, out zhengDuoSceneInfo))
				{
					if (timeSpan >= zhengDuoSceneInfo.TimeBegin && timeSpan < zhengDuoSceneInfo.TimeEnd)
					{
						timeSpan = zhengDuoSceneInfo.TimeBegin;
					}
				}
			}
			return weekStartTimeNow.Add(timeSpan);
		}

		private void SyncData()
		{
			long num = 0L;
			ZhengDuoSyncData zhengDuoSyncData = TianTiClient.getInstance().ZhengDuoSync(this.RuntimeData.Age);
			lock (this.RuntimeData.Mutex)
			{
				if (zhengDuoSyncData != null && this.RuntimeData.Age != zhengDuoSyncData.Age)
				{
					num = zhengDuoSyncData.Age;
					this.RuntimeData.Age = zhengDuoSyncData.Age;
					Array.Clear(this.RuntimeData.ZhengDuoRankDatas, 0, this.RuntimeData.ZhengDuoRankDatas.Length);
					this.RuntimeData.bhid2ZhengDuoRankDataDict.Clear();
					this.RuntimeData.Rank1Bhid = 0;
					bool flag2 = this.RuntimeData.ZhengDuoStep != zhengDuoSyncData.ZhengDuoStep && zhengDuoSyncData.State > this.RuntimeData.State;
					this.RuntimeData.ZhengDuoStep = zhengDuoSyncData.ZhengDuoStep;
					this.RuntimeData.State = zhengDuoSyncData.State;
					this.RuntimeData.WeekDay = zhengDuoSyncData.WeekDay;
					if (null != zhengDuoSyncData.RankDatas)
					{
						foreach (ZhengDuoRankData zhengDuoRankData in zhengDuoSyncData.RankDatas)
						{
							if (zhengDuoRankData != null && zhengDuoRankData.Bhid > 0)
							{
								this.RuntimeData.bhid2ZhengDuoRankDataDict[zhengDuoRankData.Bhid] = zhengDuoRankData;
								this.RuntimeData.ZhengDuoRankDatas[zhengDuoRankData.Rank1] = zhengDuoRankData;
								if (flag2 && zhengDuoRankData.Lose == 0 && zhengDuoRankData.State > 0)
								{
									GameManager.ClientMgr.SendBangHuiCmd<int>(zhengDuoRankData.Bhid, 1072, zhengDuoSyncData.ZhengDuoStep, true, true);
								}
								if (zhengDuoRankData.Rank2 == 1)
								{
									this.RuntimeData.Rank1Bhid = zhengDuoRankData.Bhid;
								}
							}
						}
					}
				}
			}
			if (num > 0L)
			{
				int num2 = 0;
				GameClient nextClient;
				while ((nextClient = GameManager.ClientMgr.GetNextClient(ref num2, false)) != null)
				{
					if (nextClient.ClientData.ZhengDuoDataAge > 0L && nextClient.ClientData.ZhengDuoDataAge != num)
					{
						this.HandleZhengDuoData(nextClient, 1070, null, null);
					}
				}
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= this.RuntimeData.NextHeartBeatTicks)
			{
				this.RuntimeData.NextHeartBeatTicks = num + 1020L;
				this.SyncData();
				foreach (ZhengDuoScene zhengDuoScene in this.RuntimeData.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = zhengDuoScene.FuBenSeqId;
						int copyMapId = zhengDuoScene.CopyMapId;
						int nMapCode = zhengDuoScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = zhengDuoScene.CopyMap;
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							zhengDuoScene.Age++;
							if (zhengDuoScene.m_eStatus == 1)
							{
								if (num2 >= zhengDuoScene.m_lBeginTime)
								{
									zhengDuoScene.m_eStatus = 2;
									zhengDuoScene.StateTimeData.GameType = 17;
									zhengDuoScene.StateTimeData.State = zhengDuoScene.m_eStatus;
									zhengDuoScene.StateTimeData.EndTicks = zhengDuoScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengDuoScene.StateTimeData, zhengDuoScene.CopyMap);
									zhengDuoScene.CopyMap.AddGuangMuEvent(1, 0);
									zhengDuoScene.CopyMap.AddGuangMuEvent(2, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(zhengDuoScene.CopyMap.MapCode, zhengDuoScene.CopyMap.CopyMapID, 1, 0);
									GameManager.ClientMgr.BroadSpecialMapAIEvent(zhengDuoScene.CopyMap.MapCode, zhengDuoScene.CopyMap.CopyMapID, 2, 0);
								}
								else if (zhengDuoScene.Age % 3 == 0)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengDuoScene.StateTimeData, zhengDuoScene.CopyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(zhengDuoScene), zhengDuoScene.CopyMap);
								}
							}
							else if (zhengDuoScene.m_eStatus == 2)
							{
								if (num2 >= zhengDuoScene.m_lEndTime || zhengDuoScene.KillerId > 0)
								{
									this.CompleteScene(zhengDuoScene);
									zhengDuoScene.m_eStatus = 3;
									zhengDuoScene.m_lLeaveTime = zhengDuoScene.m_lEndTime + (long)(zhengDuoScene.SceneInfo.SecondClear * 1000);
									zhengDuoScene.StateTimeData.GameType = 17;
									zhengDuoScene.StateTimeData.State = 5;
									zhengDuoScene.StateTimeData.EndTicks = zhengDuoScene.m_lLeaveTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengDuoScene.StateTimeData, zhengDuoScene.CopyMap);
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(zhengDuoScene), zhengDuoScene.CopyMap);
								}
								else
								{
									this.CheckCreateDynamicMonster(zhengDuoScene, num);
									if (zhengDuoScene.Age % 3 == 0)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, zhengDuoScene.StateTimeData, zhengDuoScene.CopyMap);
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(zhengDuoScene), zhengDuoScene.CopyMap);
									}
								}
							}
							else if (zhengDuoScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(zhengDuoScene.CopyMap);
								zhengDuoScene.m_eStatus = 4;
								if (zhengDuoScene.PreliminarisesMode)
								{
									ZhengDuoRankData zhengDuoRankData = zhengDuoScene.RankDatas[0];
									zhengDuoRankData.UsedMillisecond = zhengDuoScene.KillUsedTicks;
									ZhengDuoSignUpData zhengDuoSignUpData;
									if (this.RuntimeData.FightDataDict.TryGetValue(zhengDuoRankData.Bhid, out zhengDuoSignUpData))
									{
										zhengDuoSignUpData.UsedTicks = zhengDuoScene.KillUsedTicks;
										EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
										{
											1,
											this.RuntimeData.WeekDay,
											GameManager.ServerId,
											zhengDuoSignUpData.Bhid,
											zhengDuoScene.KillUsedTicks,
											"争夺之地海选战斗结果"
										});
									}
									TianTiClient.getInstance().ZhengDuoSign(zhengDuoRankData.Bhid, zhengDuoRankData.UsedMillisecond, zhengDuoRankData.ZoneId, zhengDuoRankData.BhName, zhengDuoRankData.BhLevel, zhengDuoRankData.ZhanLi);
									this.GiveAwards(zhengDuoScene);
									int[] cmd = new int[]
									{
										zhengDuoRankData.Bhid,
										this.RuntimeData.WeekDay,
										zhengDuoScene.KillUsedTicks
									};
									Global.sendToDB<int, int[]>(10223, cmd, zhengDuoRankData.ServerID);
								}
								else
								{
									int[] array = new int[2];
									if (zhengDuoScene.ScoreData[0] != null)
									{
										array[0] = zhengDuoScene.ScoreData[0].Id;
									}
									if (zhengDuoScene.ScoreData[1] != null)
									{
										array[1] = zhengDuoScene.ScoreData[1].Id;
									}
									TianTiClient.getInstance().ZhengDuoResult(zhengDuoScene.SuccessSide, array);
									this.GiveAwards(zhengDuoScene);
									EventLogManager.AddGameEvent(LogRecordType.ZhengDuoZhiDi, new object[]
									{
										2,
										this.RuntimeData.WeekDay,
										GameManager.ServerId,
										zhengDuoScene.SuccessSide,
										array[0],
										array[1],
										"争夺之地淘汰赛战斗结果"
									});
								}
								ZhengDuoFuBenData zhengDuoFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue((long)zhengDuoScene.GameId, out zhengDuoFuBenData))
								{
									zhengDuoFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("争夺之地跨服副本GameID={0},战斗结束", zhengDuoFuBenData.GameId), null, true);
								}
							}
							else if (zhengDuoScene.m_eStatus == 4)
							{
								if (num2 >= zhengDuoScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(zhengDuoScene.m_lLeaveTime + 120000L);
									zhengDuoScene.m_eStatus = 5;
									try
									{
										List<GameClient> clientsList = copyMap.GetClientsList();
										if (clientsList != null && clientsList.Count > 0)
										{
											for (int i = 0; i < clientsList.Count; i++)
											{
												GameClient gameClient = clientsList[i];
												if (gameClient != null)
												{
													if (zhengDuoScene.PreliminarisesMode)
													{
														Global.GotoLastMap(gameClient, 1);
													}
													else
													{
														KuaFuManager.getInstance().GotoLastMap(gameClient);
													}
												}
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "争夺之地系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		public void CompleteScene(ZhengDuoScene scene)
		{
			int successSide = 0;
			if (scene.PreliminarisesMode)
			{
				if (scene.KillerId > 0)
				{
					successSide = scene.RankDatas[0].Bhid;
				}
			}
			else if (scene.ScoreData[0] != null && scene.ScoreData[1] != null)
			{
				if (scene.ScoreData[0].Score > scene.ScoreData[1].Score)
				{
					successSide = scene.ScoreData[0].Id;
				}
				else if (scene.ScoreData[1].Score > scene.ScoreData[0].Score)
				{
					successSide = scene.ScoreData[1].Id;
				}
			}
			else if (scene.ScoreData[0] != null)
			{
				successSide = scene.ScoreData[0].Id;
			}
			scene.SuccessSide = successSide;
		}

		public void CreateMonster(ZhengDuoScene scene, ZhengDuoMonsterInfo monsterInfo)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, monsterInfo.MonsterID, scene.CopyMapId, 1, monsterInfo.PosX / scene.MapGridWidth, monsterInfo.PosY / scene.MapGridHeight, 0, 0, 40, monsterInfo, null);
		}

		private void AddDelayCreateMonster(ZhengDuoScene scene, long ticks, object monster)
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

		private void InitCreateDynamicMonster(ZhengDuoScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoMonsterInfo monsterInfo = this.ConfigData.MonsterInfo;
				this.AddDelayCreateMonster(scene, scene.m_lBeginTime + (long)(monsterInfo.RefreshSecond * 1000), this.ConfigData.MonsterInfo);
			}
		}

		public void CheckCreateDynamicMonster(ZhengDuoScene scene, long nowMs)
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
							if (obj is ZhengDuoMonsterInfo)
							{
								ZhengDuoMonsterInfo zhengDuoMonsterInfo = obj as ZhengDuoMonsterInfo;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, zhengDuoMonsterInfo.MonsterID, scene.CopyMapId, 1, zhengDuoMonsterInfo.PosX / scene.MapGridWidth, zhengDuoMonsterInfo.PosY / scene.MapGridHeight, 0, 0, 40, zhengDuoMonsterInfo, MonsterFlags.AllFlags);
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

		public void GiveAwards(ZhengDuoScene scene)
		{
			try
			{
				int[] array = new int[2];
				List<GameClient> clientsList = scene.CopyMap.GetClientsList();
				foreach (GameClient gameClient in clientsList)
				{
					try
					{
						long num = 0L;
						ZhengDuoAwardData zhengDuoAwardData = new ZhengDuoAwardData();
						if (scene.KillerId > 0)
						{
							zhengDuoAwardData.Second = scene.KillUsedTicks / 1000;
						}
						if (scene.SuccessSide == gameClient.ClientData.Faction)
						{
							zhengDuoAwardData.State = 1;
						}
						AwardsItemList awardsItemList = new AwardsItemList();
						ZhengDuoScoreData zhengDuoScoreData;
						if (this.ConfigData.AwardHurtMin == 0 || (scene.ClientContextDataDict.TryGetValue(gameClient.ClientData.RoleID, out zhengDuoScoreData) && (num = zhengDuoScoreData.Score) >= (long)this.ConfigData.AwardHurtMin))
						{
							if (gameClient.ClientData.Faction == scene.SuccessSide)
							{
								long exp = Global.GetExpMultiByZhuanShengExpXiShu(gameClient, scene.SceneInfo.RateExp);
								int money = scene.SceneInfo.RateBindJinBi;
								zhengDuoAwardData.Exp = exp;
								zhengDuoAwardData.Money = money;
								ConfigParser.ParseAwardsItemList(scene.SceneInfo.AwardWin, ref awardsItemList, '|', ',');
							}
							else
							{
								long exp = (long)((double)Global.GetExpMultiByZhuanShengExpXiShu(gameClient, scene.SceneInfo.RateExp) * 0.8);
								int money = (int)((double)scene.SceneInfo.RateBindJinBi * 0.8);
								zhengDuoAwardData.Exp = exp;
								zhengDuoAwardData.Money = money;
								ConfigParser.ParseAwardsItemList(scene.SceneInfo.AwardLost, ref awardsItemList, '|', ',');
							}
							if (gameClient.ClientData.RoleID == scene.KillerId)
							{
								awardsItemList.Add(scene.SceneInfo.AwardKiller);
							}
						}
						else
						{
							LogManager.WriteLog(7, string.Format("争夺之地积分未达到最低奖励要求#rid={0},rname={1},score={2}", gameClient.ClientData.RoleID, gameClient.ClientData.RoleName, num), null, true);
						}
						zhengDuoAwardData.GoodsList = Global.ConvertToGoodsDataList(awardsItemList.Items, -1);
						if (zhengDuoAwardData.Money > 0)
						{
							GameManager.ClientMgr.AddMoney1(gameClient, zhengDuoAwardData.Money, "争夺之地奖励", true);
						}
						if (zhengDuoAwardData.Exp > 0L)
						{
							GameManager.ClientMgr.ProcessRoleExperience(gameClient, zhengDuoAwardData.Exp, true, true, false, "争夺之地奖励");
						}
						if (zhengDuoAwardData.GoodsList != null && zhengDuoAwardData.GoodsList.Count > 0)
						{
							if (!Global.CanAddGoodsDataList(gameClient, zhengDuoAwardData.GoodsList))
							{
								GameManager.ClientMgr.SendMailWhenPacketFull(gameClient, zhengDuoAwardData.GoodsList, GLang.GetLang(551, new object[0]), GLang.GetLang(552, new object[0]));
							}
							else
							{
								for (int i = 0; i < zhengDuoAwardData.GoodsList.Count; i++)
								{
									GoodsData goodsData = zhengDuoAwardData.GoodsList[i];
									if (null != goodsData)
									{
										goodsData.Id = Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, goodsData.GoodsID, goodsData.GCount, goodsData.Quality, goodsData.Props, goodsData.Forge_level, goodsData.Binding, 0, goodsData.Jewellist, true, 1, "争夺之地奖励", goodsData.Endtime, goodsData.AddPropIndex, goodsData.BornIndex, goodsData.Lucky, goodsData.Strong, 0, 0, 0, null, null, 0, true);
									}
								}
							}
						}
						gameClient.sendCmd<ZhengDuoAwardData>(1076, zhengDuoAwardData, false);
					}
					catch (Exception ex)
					{
						LogManager.WriteException(ex.ToString());
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState, bool scoreInfo = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoScene zhengDuoScene;
				if (this.RuntimeData.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zhengDuoScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, zhengDuoScene.StateTimeData, false);
					}
					if (scoreInfo)
					{
						client.sendCmd<ZhengDuoScoreInfo>(1075, this.MakeZhengDuoScoreInfo(zhengDuoScene), false);
					}
				}
			}
		}

		private ZhengDuoScoreInfo MakeZhengDuoScoreInfo(ZhengDuoScene scene)
		{
			ZhengDuoScoreInfo zhengDuoScoreInfo = new ZhengDuoScoreInfo
			{
				Step = scene.SceneInfo.Id
			};
			zhengDuoScoreInfo.ScoreRank = new List<ZhengDuoScoreData>();
			int num = 0;
			while (num < scene.ScoreData.Length && num < 3)
			{
				if (null != scene.ScoreData[num])
				{
					zhengDuoScoreInfo.ScoreRank.Add(scene.ScoreData[num]);
				}
				num++;
			}
			return zhengDuoScoreInfo;
		}

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				ZhengDuoBirthInfo zhengDuoBirthInfo;
				if (this.ConfigData.BirthInfoList.TryGetValue(battleWhichSide, out zhengDuoBirthInfo))
				{
					posX = zhengDuoBirthInfo.X;
					posY = zhengDuoBirthInfo.Y;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		private ZhengDuoFuBenData GetKuaFuFuBenDataByBhid(int bhid, int weekDay)
		{
			ZhengDuoFuBenData zhengDuoFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.FuBenItemDataByBhid.TryGetValue(bhid, out zhengDuoFuBenData) && zhengDuoFuBenData.WeekDay == weekDay && zhengDuoFuBenData.GroupIndex == this.RuntimeData.ZhengDuoStep)
				{
					return zhengDuoFuBenData;
				}
			}
			zhengDuoFuBenData = TianTiClient.getInstance().GetZhengDuoFuBenDataByBhid(bhid);
			if (zhengDuoFuBenData == null || zhengDuoFuBenData.WeekDay != weekDay || zhengDuoFuBenData.GroupIndex != this.RuntimeData.ZhengDuoStep)
			{
				LogManager.WriteLog(2, "获取不到有效的争夺之地副本数据,fuBenData == null", null, true);
				zhengDuoFuBenData = null;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoFuBenData zhengDuoFuBenData2;
					if (this.RuntimeData.FuBenItemDataByBhid.TryGetValue(bhid, out zhengDuoFuBenData2) && zhengDuoFuBenData2.WeekDay == weekDay && zhengDuoFuBenData2.GroupIndex == this.RuntimeData.ZhengDuoStep)
					{
						zhengDuoFuBenData = zhengDuoFuBenData2;
					}
					else
					{
						this.RuntimeData.FuBenItemDataByBhid[bhid] = zhengDuoFuBenData;
					}
				}
			}
			return zhengDuoFuBenData;
		}

		private ZhengDuoFuBenData GetKuaFuFuBenData(long gameId, int weekDay)
		{
			ZhengDuoFuBenData zhengDuoFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.FuBenItemData.TryGetValue(gameId, out zhengDuoFuBenData) && zhengDuoFuBenData.WeekDay == weekDay && zhengDuoFuBenData.GroupIndex == this.RuntimeData.ZhengDuoStep)
				{
					return zhengDuoFuBenData;
				}
			}
			zhengDuoFuBenData = TianTiClient.getInstance().GetZhengDuoFuBenData(gameId);
			if (zhengDuoFuBenData == null || zhengDuoFuBenData.WeekDay != weekDay)
			{
				LogManager.WriteLog(2, "获取不到有效的争夺之地副本数据,fuBenData == null", null, true);
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					ZhengDuoFuBenData zhengDuoFuBenData2;
					if (this.RuntimeData.FuBenItemData.TryGetValue(gameId, out zhengDuoFuBenData2) && zhengDuoFuBenData2.WeekDay == weekDay && zhengDuoFuBenData2.GroupIndex == this.RuntimeData.ZhengDuoStep)
					{
						zhengDuoFuBenData = zhengDuoFuBenData2;
					}
					else
					{
						this.RuntimeData.FuBenItemData[gameId] = zhengDuoFuBenData;
						zhengDuoFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
					}
				}
			}
			return zhengDuoFuBenData;
		}

		public bool OnInitGame(GameClient client)
		{
			int num = 0;
			bool flag = false;
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			ZhengDuoFuBenData kuaFuFuBenData = this.GetKuaFuFuBenData(clientKuaFuServerLoginData.GameId, this.RuntimeData.WeekDay);
			if (kuaFuFuBenData != null && kuaFuFuBenData.State < 3)
			{
				if (kuaFuFuBenData.ServerId == GameManager.ServerId)
				{
					if (kuaFuFuBenData.PlayerDict.TryGetValue(client.ClientData.Faction, out num))
					{
						flag = true;
					}
				}
			}
			bool result;
			if (!flag)
			{
				result = false;
			}
			else
			{
				client.ClientData.BattleWhichSide = num;
				int posX;
				int posY;
				num = this.GetBirthPoint(client, out posX, out posY);
				if (num <= 0)
				{
					LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + num, null, true);
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						clientKuaFuServerLoginData.FuBenSeqId = kuaFuFuBenData.SequenceId;
						ZhengDuoSceneInfo zhengDuoSceneInfo;
						if (!this.ConfigData.SceneDataDict.TryGetValue(kuaFuFuBenData.GroupIndex, out zhengDuoSceneInfo))
						{
							return false;
						}
						client.ClientData.MapCode = zhengDuoSceneInfo.MapCode;
						client.SceneInfoObject = zhengDuoSceneInfo;
					}
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
					result = true;
				}
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client, out posX, out posY);
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

		public const SceneUIClasses _sceneType = 40;

		public const GameTypes _gameType = 17;

		private static ZhengDuoManager instance = new ZhengDuoManager();

		private KFZhengDuoConfig ConfigData = new KFZhengDuoConfig();

		private ZhengDuoRuntimeData RuntimeData = new ZhengDuoRuntimeData();
	}
}
