using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.MoRi
{
	public class MoRiJudgeManager : SingletonTemplate<MoRiJudgeManager>, IManager, ICmdProcessorEx, ICmdProcessor, IManager2, IEventListenerEx, IEventListener
	{
		private MoRiJudgeManager()
		{
		}

		public double[] AwardFactor { get; set; }

		public int MapCode { get; set; }

		public bool initialize()
		{
			return this.InitConfig();
		}

		private bool InitConfig()
		{
			try
			{
				XElement xelement = XElement.Load(Global.GameResPath("Config/MoRiShenPan.xml"));
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					MoRiMonster moRiMonster = new MoRiMonster();
					moRiMonster.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
					moRiMonster.Name = Global.GetSafeAttributeStr(xml, "Name");
					moRiMonster.MonsterId = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
					moRiMonster.BirthX = (int)Global.GetSafeAttributeLong(xml, "X");
					moRiMonster.BirthY = (int)Global.GetSafeAttributeLong(xml, "Y");
					moRiMonster.KillLimitSecond = (int)Global.GetSafeAttributeLong(xml, "Time");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Props");
					if (!string.IsNullOrEmpty(safeAttributeStr) && safeAttributeStr != "-1")
					{
						foreach (string text in safeAttributeStr.Split(new char[]
						{
							'|'
						}))
						{
							string[] array2 = text.Split(new char[]
							{
								','
							});
							if (array2 != null && array2.Length == 2)
							{
								moRiMonster.ExtPropDict.Add((int)Enum.Parse(typeof(ExtPropIndexes), array2[0]), (float)Convert.ToDouble(array2[1]));
							}
						}
					}
					this.BossConfigList.Add(moRiMonster);
				}
				this.BossConfigList.Sort(delegate(MoRiMonster left, MoRiMonster right)
				{
					int result;
					if (left.Id < right.Id)
					{
						result = -1;
					}
					else if (left.Id > right.Id)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				SystemXmlItem systemXmlItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(70000, out systemXmlItem))
				{
					LogManager.WriteLog(1000, string.Format("缺少末日审判副本配置 CopyID={0}", 70000), null, true);
					return false;
				}
				this.MapCode = systemXmlItem.GetIntValue("MapCode", -1);
				FuBenMapItem fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(70000, this.MapCode);
				if (fuBenMapItem == null)
				{
					LogManager.WriteLog(1000, string.Format("末日审判地图 {0} 配置错误", this.MapCode), null, true);
					return false;
				}
				this.CopyMaxAliveMinutes = fuBenMapItem.MaxTime;
				GameMap gameMap = null;
				if (!GameManager.MapMgr.DictMaps.TryGetValue(this.MapCode, out gameMap))
				{
					LogManager.WriteLog(1000, string.Format("缺少末日审判地图 {0}", this.MapCode), null, true);
					return false;
				}
				this.copyMapGirdWidth = gameMap.MapGridWidth;
				this.copyMapGirdHeight = gameMap.MapGridHeight;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", "Config/MoRiShenPan.xml"), ex, true);
				return false;
			}
			return true;
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1301, 1, 1, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1302, 1, 1, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1304, 2, 2, SingletonTemplate<MoRiJudgeManager>.Instance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10000, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10001, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10004, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10005, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource.getInstance().registerListener(11, SingletonTemplate<MoRiJudgeManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10000, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10001, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10004, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10005, 29, SingletonTemplate<MoRiJudgeManager>.Instance());
			GlobalEventSource.getInstance().removeListener(11, SingletonTemplate<MoRiJudgeManager>.Instance());
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
				if (monsterDeadEventObject.getAttacker().ClientData.CopyMapID > 0 && monsterDeadEventObject.getAttacker().ClientData.FuBenSeqID > 0 && monsterDeadEventObject.getAttacker().ClientData.MapCode == this.MapCode && monsterDeadEventObject.getMonster().CurrentMapCode == this.MapCode)
				{
					MoRiMonsterTag moRiMonsterTag = monsterDeadEventObject.getMonster().Tag as MoRiMonsterTag;
					if (moRiMonsterTag != null)
					{
						MoRiJudgeCopy moRiJudgeCopy = null;
						lock (this.copyDict)
						{
							if (!this.copyDict.TryGetValue(moRiMonsterTag.CopySeqId, out moRiJudgeCopy))
							{
								return;
							}
						}
						lock (moRiJudgeCopy)
						{
							if (moRiJudgeCopy.m_eStatus == 2)
							{
								if (moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].DeathMs <= 0L)
								{
									moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].DeathMs = TimeUtil.NOW();
									GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										2,
										this.BossConfigList[moRiMonsterTag.MonsterIdx].Id,
										moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].BirthMs,
										moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].DeathMs
									}), moRiJudgeCopy.MyCopyMap, false);
									this.CalcAwardRate(moRiJudgeCopy);
									if (moRiJudgeCopy.MonsterList.Count == this.BossConfigList.Count)
									{
										moRiJudgeCopy.Passed = true;
										moRiJudgeCopy.m_eStatus = 3;
									}
								}
							}
						}
					}
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
						gameClient.sendCmd(1303, kuaFuFuBenRoleCountEvent.RoleCount.ToString(), false);
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
								gameClient.sendCmd(1304, string.Format("{0}:{1}", kuaFuServerLoginData.GameId, kuaFuNotifyEnterGameEvent.TeamCombatAvg), false);
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
					gameClient.sendCmd(1306, string.Format("{0}:{1}", kuaFuNotifyCopyCancelEvent.GameId, kuaFuNotifyCopyCancelEvent.Reason), false);
					gameClient.ClientData.SignUpGameType = 0;
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
						GlobalNew.RecordSwitchKuaFuServerLog(gameClient);
						gameClient.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(gameClient), false);
						gameClient.ClientData.SignUpGameType = 0;
					}
				}
				eventObject.Handled = true;
				break;
			}
			}
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return false;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 1301:
				return this.ProcessMoRiJudgeJoin(client, nID, bytes, cmdParams);
			case 1302:
				return this.ProcessMoRiJudgeQuit(client, nID, bytes, cmdParams);
			case 1304:
				return this.ProcessMoRiJudgeEnter(client, nID, bytes, cmdParams);
			}
			return true;
		}

		private bool ProcessMoRiJudgeJoin(GameClient client, int nID, byte[] bytes, string[] cmdParams)
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
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(70000, out systemXmlItem))
				{
					client.sendCmd(nID, -3.ToString(), false);
					return true;
				}
				int intValue = systemXmlItem.GetIntValue("MinLevel", -1);
				int intValue2 = systemXmlItem.GetIntValue("MaxLevel", -1);
				int intValue3 = systemXmlItem.GetIntValue("MinZhuanSheng", -1);
				int intValue4 = systemXmlItem.GetIntValue("MaxZhuanSheng", -1);
				if (client.ClientData.ChangeLifeCount < intValue3 || (client.ClientData.ChangeLifeCount == intValue3 && client.ClientData.Level < intValue))
				{
					client.sendCmd(nID, -19.ToString(), false);
					return true;
				}
				if (client.ClientData.ChangeLifeCount > intValue4 || (client.ClientData.ChangeLifeCount == intValue4 && client.ClientData.Level > intValue2))
				{
					client.sendCmd(nID, -19.ToString(), false);
					return true;
				}
				FuBenData fuBenData = Global.GetFuBenData(client, 70000);
				if (fuBenData != null && fuBenData.FinishNum >= systemXmlItem.GetIntValue("FinishNumber", -1))
				{
					client.sendCmd(nID, -16.ToString(), false);
					return true;
				}
				int num = 0;
				if (num == 1)
				{
					client.ClientData.SignUpGameType = 3;
					GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 1, 0, 0, 0, 3);
				}
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessMoRiJudgeQuit(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, 0.ToString(), false);
					return true;
				}
				int num = 1;
				if (num >= 0)
				{
					num = 0;
					client.ClientData.SignUpGameType = 0;
				}
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private bool ProcessMoRiJudgeEnter(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, true))
				{
					client.sendCmd(nID, -2001.ToString() + ":0", false);
					return true;
				}
				int num = 1;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				if (num2 > 0)
				{
					if (num < 0)
					{
						num2 = 0;
					}
				}
				else
				{
					client.ClientData.SignUpGameType = 0;
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
				if (num2 <= 0)
				{
					Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
					client.sendCmd<int>(1302, 0, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private double CalcAwardRate(MoRiJudgeCopy judgeCopy)
		{
			double result = 1.0;
			judgeCopy.LimitKillCount = 0;
			for (int i = 0; i < judgeCopy.MonsterList.Count; i++)
			{
				if (judgeCopy.MonsterList[i].DeathMs > 0L && this.BossConfigList[i].KillLimitSecond > 0 && judgeCopy.MonsterList[i].DeathMs - judgeCopy.MonsterList[i].BirthMs <= (long)(this.BossConfigList[i].KillLimitSecond * 1000))
				{
					judgeCopy.LimitKillCount++;
				}
			}
			if (this.AwardFactor != null && judgeCopy.LimitKillCount - 1 >= 0 && judgeCopy.LimitKillCount - 1 < this.AwardFactor.Length)
			{
				result = this.AwardFactor[judgeCopy.LimitKillCount - 1];
			}
			return result;
		}

		private bool IsGongNengOpened(GameClient client, bool bHint = true)
		{
			return GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("MoRiShenPan") && GlobalNew.IsGongNengOpened(client, 63, bHint);
		}

		public bool OnInitGame(GameClient client)
		{
			int posX;
			int posY;
			bool result;
			if (!this.GetBirthPoint(out posX, out posY))
			{
				result = false;
			}
			else
			{
				client.ClientData.MapCode = this.MapCode;
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = Global.GetClientKuaFuServerLoginData(client).FuBenSeqId;
				result = true;
			}
			return result;
		}

		public void OnLogOut(GameClient client)
		{
			MoRiJudgeCopy moRiJudgeCopy = null;
			lock (this.copyDict)
			{
				if (!this.copyDict.TryGetValue(client.ClientData.FuBenSeqID, out moRiJudgeCopy))
				{
					return;
				}
			}
			lock (moRiJudgeCopy)
			{
				moRiJudgeCopy.RoleCount--;
				if (moRiJudgeCopy.m_eStatus != 3 && moRiJudgeCopy.m_eStatus != 4 && moRiJudgeCopy.m_eStatus != 5)
				{
					KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
				}
			}
		}

		public void AddCopyScene(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			if (copyMap.MapCode == this.MapCode)
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				int mapCode = copyMap.MapCode;
				lock (this.copyDict)
				{
					MoRiJudgeCopy moRiJudgeCopy = null;
					if (!this.copyDict.TryGetValue(fuBenSeqID, out moRiJudgeCopy))
					{
						moRiJudgeCopy = new MoRiJudgeCopy();
						moRiJudgeCopy.MyCopyMap = copyMap;
						moRiJudgeCopy.GameId = Global.GetClientKuaFuServerLoginData(client).GameId;
						moRiJudgeCopy.StateTimeData.GameType = 3;
						moRiJudgeCopy.StartTime = TimeUtil.NowDateTime();
						moRiJudgeCopy.EndTime = moRiJudgeCopy.StartTime.AddMinutes((double)this.CopyMaxAliveMinutes);
						moRiJudgeCopy.LimitKillCount = 0;
						moRiJudgeCopy.RoleCount = 1;
						moRiJudgeCopy.Passed = false;
						this.copyDict[fuBenSeqID] = moRiJudgeCopy;
					}
					else
					{
						moRiJudgeCopy.RoleCount++;
					}
				}
				FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
				copyMap.IsKuaFuCopy = true;
				copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)((this.CopyMaxAliveMinutes + 3) * 60000));
				GlobalNew.UpdateKuaFuRoleDayLogData(client.ServerId, client.ClientData.RoleID, TimeUtil.NowDateTime(), client.ClientData.ZoneID, 0, 1, 0, 0, 3);
			}
		}

		public void DelCopyScene(CopyMap copyMap)
		{
			if (copyMap != null && copyMap.MapCode == this.MapCode)
			{
				MoRiJudgeCopy moRiJudgeCopy = null;
				lock (this.copyDict)
				{
					if (!this.copyDict.TryGetValue(copyMap.FuBenSeqID, out moRiJudgeCopy))
					{
						return;
					}
				}
				lock (moRiJudgeCopy)
				{
					if (moRiJudgeCopy.m_eStatus < 3)
					{
						moRiJudgeCopy.m_eStatus = 3;
					}
				}
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= this.NextHeartBeatMs)
			{
				this.NextHeartBeatMs = num + 1020L;
				List<MoRiJudgeCopy> list = null;
				lock (this.copyDict)
				{
					list = this.copyDict.Values.ToList<MoRiJudgeCopy>();
				}
				if (list != null && list.Count > 0)
				{
					foreach (MoRiJudgeCopy moRiJudgeCopy in list)
					{
						lock (moRiJudgeCopy)
						{
							if (moRiJudgeCopy.m_eStatus == 0)
							{
								moRiJudgeCopy.m_eStatus = 1;
								moRiJudgeCopy.CurrStateBeginMs = num;
								moRiJudgeCopy.DeadlineMs = num + (long)(this.CopyMaxAliveMinutes * 60 * 1000);
								moRiJudgeCopy.StateTimeData.State = 2;
								moRiJudgeCopy.StateTimeData.EndTicks = moRiJudgeCopy.DeadlineMs;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, moRiJudgeCopy.StateTimeData, moRiJudgeCopy.MyCopyMap);
							}
							else if (moRiJudgeCopy.m_eStatus == 1)
							{
								if (num >= moRiJudgeCopy.CurrStateBeginMs + 1500L)
								{
									moRiJudgeCopy.m_eStatus = 2;
									moRiJudgeCopy.CurrStateBeginMs = num;
								}
							}
							else if (moRiJudgeCopy.m_eStatus == 2)
							{
								if (num >= moRiJudgeCopy.DeadlineMs || (num >= moRiJudgeCopy.CurrStateBeginMs + 90000L && moRiJudgeCopy.RoleCount <= 0))
								{
									moRiJudgeCopy.m_eStatus = 3;
									moRiJudgeCopy.CurrStateBeginMs = num;
									break;
								}
								int num2 = -1;
								if (moRiJudgeCopy.CurrMonsterIdx == -1)
								{
									num2 = 0;
								}
								else if (moRiJudgeCopy.MonsterList[moRiJudgeCopy.CurrMonsterIdx].DeathMs > 0L && num >= moRiJudgeCopy.MonsterList[moRiJudgeCopy.CurrMonsterIdx].DeathMs + 1300L)
								{
									num2 = moRiJudgeCopy.CurrMonsterIdx + 1;
								}
								if (num2 != -1)
								{
									if (num2 >= this.BossConfigList.Count)
									{
										moRiJudgeCopy.m_eStatus = 3;
										moRiJudgeCopy.CurrStateBeginMs = num;
									}
									else
									{
										this.FlushMonster(moRiJudgeCopy, num2);
									}
								}
							}
							else if (moRiJudgeCopy.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(moRiJudgeCopy.MyCopyMap);
								moRiJudgeCopy.EndTime = TimeUtil.NowDateTime();
								int num3 = 0;
								List<GameClient> clientsList = moRiJudgeCopy.MyCopyMap.GetClientsList();
								if (clientsList != null && clientsList.Count > 0)
								{
									int num4 = 0;
									foreach (GameClient gameClient in clientsList)
									{
										num3++;
										num4 += gameClient.ClientData.CombatForce;
										if (moRiJudgeCopy.Passed)
										{
											GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 1, 0, 3);
										}
										else
										{
											GlobalNew.UpdateKuaFuRoleDayLogData(gameClient.ServerId, gameClient.ClientData.RoleID, TimeUtil.NowDateTime(), gameClient.ClientData.ZoneID, 0, 0, 0, 1, 3);
										}
									}
									if (num3 > 0)
									{
										int num5 = num4 / num3;
									}
									if (moRiJudgeCopy.Passed)
									{
										GameManager.CopyMapMgr.CopyMapPassAwardForAll(clientsList[0], moRiJudgeCopy.MyCopyMap, false);
									}
								}
								moRiJudgeCopy.m_eStatus = 4;
								moRiJudgeCopy.CurrStateBeginMs = num;
								moRiJudgeCopy.StateTimeData.State = 3;
								moRiJudgeCopy.StateTimeData.EndTicks = num + 30000L;
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, moRiJudgeCopy.StateTimeData, moRiJudgeCopy.MyCopyMap);
							}
							else if (moRiJudgeCopy.m_eStatus == 4)
							{
								if (num >= moRiJudgeCopy.CurrStateBeginMs + 30000L)
								{
									lock (this.copyDict)
									{
										this.copyDict.Remove(moRiJudgeCopy.MyCopyMap.FuBenSeqID);
									}
									try
									{
										List<GameClient> clientsList = moRiJudgeCopy.MyCopyMap.GetClientsList();
										if (clientsList != null)
										{
											foreach (GameClient gameClient in clientsList)
											{
												KuaFuManager.getInstance().GotoLastMap(gameClient);
											}
										}
									}
									catch (Exception ex)
									{
										DataHelper.WriteExceptionLogEx(ex, "末日审判清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		private void FlushMonster(MoRiJudgeCopy judgeCopy, int nextMonsterIdx)
		{
			MoRiMonsterTag moRiMonsterTag = new MoRiMonsterTag();
			moRiMonsterTag.CopySeqId = judgeCopy.MyCopyMap.FuBenSeqID;
			moRiMonsterTag.MonsterIdx = nextMonsterIdx;
			moRiMonsterTag.ExtPropDict = null;
			if (nextMonsterIdx == this.BossConfigList.Count - 1)
			{
				moRiMonsterTag.ExtPropDict = new Dictionary<int, float>();
				int num = 0;
				while (num < judgeCopy.MonsterList.Count && num < judgeCopy.CurrMonsterIdx)
				{
					if (this.BossConfigList[num].KillLimitSecond != -1 && judgeCopy.MonsterList[num].DeathMs - judgeCopy.MonsterList[num].BirthMs <= (long)this.BossConfigList[num].KillLimitSecond * 1000L)
					{
						foreach (KeyValuePair<int, float> keyValuePair in this.BossConfigList[num].ExtPropDict)
						{
							if (moRiMonsterTag.ExtPropDict.ContainsKey(keyValuePair.Key))
							{
								Dictionary<int, float> extPropDict;
								int key;
								(extPropDict = moRiMonsterTag.ExtPropDict)[key = keyValuePair.Key] = extPropDict[key] + keyValuePair.Value;
							}
							else
							{
								moRiMonsterTag.ExtPropDict.Add(keyValuePair.Key, keyValuePair.Value);
							}
						}
					}
					num++;
				}
			}
			GameManager.MonsterZoneMgr.AddDynamicMonsters(this.MapCode, this.BossConfigList[nextMonsterIdx].MonsterId, judgeCopy.MyCopyMap.CopyMapID, 1, this.BossConfigList[nextMonsterIdx].BirthX / this.copyMapGirdWidth, this.BossConfigList[nextMonsterIdx].BirthY / this.copyMapGirdHeight, 0, 0, 29, moRiMonsterTag, null);
			judgeCopy.MonsterList.Add(new MoRiMonsterData
			{
				Id = this.BossConfigList[nextMonsterIdx].Id,
				BirthMs = TimeUtil.NOW(),
				DeathMs = -1L
			});
			judgeCopy.CurrMonsterIdx = nextMonsterIdx;
		}

		public void OnLoadDynamicMonsters(Monster monster)
		{
			MoRiMonsterTag moRiMonsterTag = null;
			if (monster != null && (moRiMonsterTag = (monster.Tag as MoRiMonsterTag)) != null)
			{
				MoRiJudgeCopy moRiJudgeCopy = null;
				lock (this.copyDict)
				{
					if (!this.copyDict.TryGetValue(moRiMonsterTag.CopySeqId, out moRiJudgeCopy))
					{
						return;
					}
				}
				GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					1,
					this.BossConfigList[moRiMonsterTag.MonsterIdx].Id,
					moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].BirthMs,
					moRiJudgeCopy.MonsterList[moRiMonsterTag.MonsterIdx].DeathMs
				}), moRiJudgeCopy.MyCopyMap, false);
				long toTicks = TimeUtil.NowDateTime().Ticks + 36000000000L;
				if (moRiMonsterTag.ExtPropDict != null)
				{
					foreach (KeyValuePair<int, float> keyValuePair in moRiMonsterTag.ExtPropDict)
					{
						monster.TempPropsBuffer.AddTempExtProp(keyValuePair.Key, (double)keyValuePair.Value, toTicks);
					}
				}
			}
		}

		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (client.ClientData.MapCode == this.MapCode)
			{
				int posX;
				int posY;
				if (!this.GetBirthPoint(out posX, out posY))
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

		private bool GetBirthPoint(out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(this.MapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[this.MapCode].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[this.MapCode].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[this.MapCode].BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, this.MapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		public void OnGMCommand(GameClient client, string[] cmdFields)
		{
			if (cmdFields[1] == "join")
			{
				this.ProcessMoRiJudgeJoin(client, 1301, null, null);
			}
			else if (cmdFields[1] == "quit")
			{
				this.ProcessMoRiJudgeQuit(client, 1302, null, null);
			}
		}

		public double GetCopyAwardRate(int copySeqId)
		{
			MoRiJudgeCopy judgeCopy = null;
			lock (this.copyDict)
			{
				if (!this.copyDict.TryGetValue(copySeqId, out judgeCopy))
				{
					return 1.0;
				}
			}
			return this.CalcAwardRate(judgeCopy);
		}

		public void NotifyTimeStateAndBossEvent(GameClient client)
		{
			MoRiJudgeCopy moRiJudgeCopy = null;
			lock (this.copyDict)
			{
				if (!this.copyDict.TryGetValue(client.ClientData.FuBenSeqID, out moRiJudgeCopy))
				{
					return;
				}
			}
			lock (moRiJudgeCopy)
			{
				client.sendCmd<GameSceneStateTimeData>(827, moRiJudgeCopy.StateTimeData, false);
				foreach (MoRiMonsterData moRiMonsterData in moRiJudgeCopy.MonsterList)
				{
					if (moRiMonsterData.BirthMs > 0L)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							1,
							moRiMonsterData.Id,
							moRiMonsterData.BirthMs,
							moRiMonsterData.DeathMs
						}), moRiJudgeCopy.MyCopyMap, false);
					}
					if (moRiMonsterData.DeathMs > 0L)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessageStr(1305, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							2,
							moRiMonsterData.Id,
							moRiMonsterData.BirthMs,
							moRiMonsterData.DeathMs
						}), moRiJudgeCopy.MyCopyMap, false);
					}
				}
			}
		}

		private List<MoRiMonster> BossConfigList = new List<MoRiMonster>();

		private Dictionary<int, MoRiJudgeCopy> copyDict = new Dictionary<int, MoRiJudgeCopy>();

		private long NextHeartBeatMs = 0L;

		private int copyMapGirdWidth;

		private int copyMapGirdHeight;

		private int CopyMaxAliveMinutes = 15;
	}
}
