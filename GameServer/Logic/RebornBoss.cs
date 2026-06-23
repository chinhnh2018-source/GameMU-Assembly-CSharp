using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Reborn;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class RebornBoss : IManager, IEventListener, IEventListenerEx, ICmdProcessorEx, ICmdProcessor
	{
		public static RebornBoss getInstance()
		{
			return RebornBoss.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1715, 2, 2, RebornBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1717, 4, 4, RebornBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(30, 54, RebornBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(28, RebornBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(11, RebornBoss.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(30, 54, RebornBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(28, RebornBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(11, RebornBoss.getInstance());
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
			case 1715:
				return this.ProcessRebornBossDataCmd(client, nID, bytes, cmdParams);
			case 1717:
				return this.ProcessRebornBossGetAwardCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
			else if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(onStartPlayGameEventObject.Client);
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num == 30)
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					RebornBossConfig rebornBossConfig = onCreateMonsterEventObject.Monster.Tag as RebornBossConfig;
					if (null != rebornBossConfig)
					{
						RebornBossScene rebornBossScene;
						if (this.SceneDict.TryGetValue(onCreateMonsterEventObject.Monster.CurrentMapCode, out rebornBossScene))
						{
							lock (RebornBoss.Mutex)
							{
								rebornBossScene.scoreData.MonsterID = onCreateMonsterEventObject.Monster.RoleID;
								rebornBossScene.scoreData.VLife = onCreateMonsterEventObject.Monster.VLife;
								rebornBossScene.scoreData.VLifeMax = onCreateMonsterEventObject.Monster.MonsterInfo.VLifeMax;
								this.BroadCastScoreInfo(rebornBossScene);
							}
						}
					}
				}
			}
		}

		public bool ProcessRebornBossDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int mapCodeID = Convert.ToInt32(cmdParams[1]);
				Dictionary<KeyValuePair<int, int>, KFRebornBossRefreshData> dictionary = null;
				lock (RebornManager.getInstance().RebornSyncDataCache)
				{
					if (null != RebornManager.getInstance().RebornSyncDataCache.BossRefreshDict)
					{
						dictionary = RebornManager.getInstance().RebornSyncDataCache.BossRefreshDict.V;
					}
				}
				List<RebornBossData> list = new List<RebornBossData>();
				KFRebornRoleData kfrebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
				if (null == kfrebornRoleData)
				{
					client.sendCmd<List<RebornBossData>>(nID, list, false);
					return true;
				}
				List<KuaFuLineData> list2 = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(mapCodeID) as List<KuaFuLineData>;
				if (list2 != null && list2.Count > 0)
				{
					using (List<KuaFuLineData>.Enumerator enumerator = list2.GetEnumerator())
					{
						while (enumerator.MoveNext())
						{
							KuaFuLineData item = enumerator.Current;
							RebornBossData rebornBossData = new RebornBossData();
							KFRebornBossRefreshData kfrebornBossRefreshData;
							if (dictionary != null && dictionary.TryGetValue(new KeyValuePair<int, int>(mapCodeID, item.Line), out kfrebornBossRefreshData))
							{
								rebornBossData.ExtensionID = kfrebornBossRefreshData.ExtensionID;
								rebornBossData.NextTime = kfrebornBossRefreshData.NextTime;
							}
							List<KFRebornBossAwardData> rebornBossKillAwardList = this.GetRebornBossKillAwardList(client);
							KFRebornBossAwardData kfrebornBossAwardData = rebornBossKillAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == item.Line);
							KFRebornBossAwardData kfrebornBossAwardData2 = kfrebornRoleData.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == item.Line);
							if (null != kfrebornBossAwardData2)
							{
								rebornBossData.AwardExtensionID = kfrebornBossAwardData2.ExtensionID;
								rebornBossData.RankNum = kfrebornBossAwardData2.RankNum;
							}
							if (null != kfrebornBossAwardData)
							{
								if (null != kfrebornBossAwardData2)
								{
									if (kfrebornBossAwardData.ExtensionID == kfrebornBossAwardData2.ExtensionID)
									{
										rebornBossData.BossKill = 1;
									}
								}
								else
								{
									rebornBossData.AwardExtensionID = kfrebornBossAwardData.ExtensionID;
									rebornBossData.BossKill = 1;
								}
							}
							list.Add(rebornBossData);
						}
					}
				}
				client.sendCmd<List<RebornBossData>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public RebornBossAwardConfig GetKillAwardConfig(int ExtensionID)
		{
			RebornBossAwardConfig result = null;
			Dictionary<int, List<RebornBossAwardConfig>> rebornBossAwardConfigDict;
			lock (RebornBoss.Mutex)
			{
				rebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
			}
			List<RebornBossAwardConfig> list;
			if (rebornBossAwardConfigDict.TryGetValue(ExtensionID, out list))
			{
				result = list.Find((RebornBossAwardConfig x) => x.AwardType == 1);
			}
			return result;
		}

		public RebornBossAwardConfig GetRankAwardConfig(int ExtensionID, int RankNum)
		{
			RebornBossAwardConfig rebornBossAwardConfig = null;
			Dictionary<int, List<RebornBossAwardConfig>> rebornBossAwardConfigDict;
			lock (RebornBoss.Mutex)
			{
				rebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
			}
			List<RebornBossAwardConfig> list;
			RebornBossAwardConfig result;
			if (!rebornBossAwardConfigDict.TryGetValue(ExtensionID, out list))
			{
				result = rebornBossAwardConfig;
			}
			else
			{
				foreach (RebornBossAwardConfig rebornBossAwardConfig2 in list)
				{
					if (rebornBossAwardConfig2.AwardType <= 0)
					{
						if (rebornBossAwardConfig2.BeginNum <= 0 || RankNum <= 0 || RankNum >= rebornBossAwardConfig2.BeginNum)
						{
							if (rebornBossAwardConfig2.EndNum <= 0 || RankNum <= rebornBossAwardConfig2.EndNum)
							{
								rebornBossAwardConfig = rebornBossAwardConfig2;
								break;
							}
						}
					}
				}
				result = rebornBossAwardConfig;
			}
			return result;
		}

		public bool ProcessRebornBossGetAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int mapCodeID = Convert.ToInt32(cmdParams[1]);
				int lineID = Convert.ToInt32(cmdParams[2]);
				int ExtensionID = Convert.ToInt32(cmdParams[3]);
				KFRebornRoleData kfrebornRoleData = KuaFuWorldClient.getInstance().Reborn_GetRebornRoleData(client.ClientData.ServerPTID, client.ClientData.LocalRoleID);
				if (null == kfrebornRoleData)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				List<KFRebornBossAwardData> rebornBossKillAwardList = this.GetRebornBossKillAwardList(client);
				KFRebornBossAwardData kfrebornBossAwardData = rebornBossKillAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == lineID && x.ExtensionID == ExtensionID);
				KFRebornBossAwardData kfrebornBossAwardData2 = kfrebornRoleData.BossAwardList.Find((KFRebornBossAwardData x) => x.MapCodeID == mapCodeID && x.LineID == lineID && x.ExtensionID == ExtensionID);
				if (kfrebornBossAwardData == null && null == kfrebornBossAwardData2)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				RebornBossAwardConfig rebornBossAwardConfig = null;
				RebornBossAwardConfig rebornBossAwardConfig2 = null;
				if (null != kfrebornBossAwardData)
				{
					rebornBossAwardConfig = this.GetKillAwardConfig(kfrebornBossAwardData.ExtensionID);
				}
				if (null != kfrebornBossAwardData2)
				{
					rebornBossAwardConfig2 = this.GetRankAwardConfig(kfrebornBossAwardData2.ExtensionID, kfrebornBossAwardData2.RankNum);
				}
				if (rebornBossAwardConfig == null && null == rebornBossAwardConfig2)
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				List<AwardsItemData> list = new List<AwardsItemData>();
				List<AwardsItemData> list2 = new List<AwardsItemData>();
				if (null != rebornBossAwardConfig)
				{
					list.AddRange(rebornBossAwardConfig.AwardsItemListOne.Items);
					list2.AddRange(rebornBossAwardConfig.AwardsItemListTwo.Items);
				}
				if (null != rebornBossAwardConfig2)
				{
					list.AddRange(rebornBossAwardConfig2.AwardsItemListOne.Items);
					list2.AddRange(rebornBossAwardConfig2.AwardsItemListTwo.Items);
				}
				int num3 = 0;
				if (list != null)
				{
					num3 += list.Count;
				}
				if (list2 != null)
				{
					num3 += list2.Count((AwardsItemData goods) => Global.IsRoleOccupationMatchGoods(client, goods.GoodsID));
				}
				int num4;
				if (!RebornEquip.MoreIsCanIntoRebornOrBaseBagAward(client, list, out num4))
				{
					if (num4 == 1)
					{
						num = -101;
					}
					else
					{
						num = -100;
					}
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						mapCodeID,
						lineID,
						ExtensionID
					}), false);
					return true;
				}
				if (null != rebornBossAwardConfig)
				{
					this.GiveBossAward(client, rebornBossAwardConfig, "重生Boss最后一击奖励");
					rebornBossKillAwardList.Remove(kfrebornBossAwardData);
					this.SaveRebornBossKillAwardList(client, rebornBossKillAwardList);
				}
				if (null != rebornBossAwardConfig2)
				{
					this.GiveBossAward(client, rebornBossAwardConfig2, "重生Boss奖励");
					string param = string.Format("{0}", ExtensionID);
					KuaFuWorldClient.getInstance().Reborn_RebornOpt(client.ClientData.ServerPTID, client.ClientData.LocalRoleID, 5, mapCodeID, lineID, param);
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					mapCodeID,
					lineID,
					ExtensionID
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool InitConfig()
		{
			bool result;
			if (!this.LoadRebornBossConfigFile())
			{
				result = false;
			}
			else if (!this.LoadRebornBossAwardConfigFile())
			{
				result = false;
			}
			else
			{
				this.RebornBossRankClearSec = (int)GameManager.systemParamsList.GetParamValueIntByName("RebornBossResumeTime", 120);
				result = true;
			}
			return result;
		}

		public bool LoadRebornBossConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornBoss));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornBoss));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, List<RebornBossConfig>> dictionary = new Dictionary<int, List<RebornBossConfig>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					int key = (int)Global.GetSafeAttributeLong(xml, "MapID");
					List<RebornBossConfig> list;
					if (!dictionary.TryGetValue(key, out list))
					{
						list = new List<RebornBossConfig>();
						dictionary[key] = list;
					}
					RebornBossConfig rebornBossConfig = new RebornBossConfig();
					rebornBossConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					rebornBossConfig.MapID = (int)Global.GetSafeAttributeLong(xml, "MapID");
					rebornBossConfig.MonstersID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
					rebornBossConfig.RebornLevel = (int)Global.GetSafeAttributeLong(xml, "RebornLevel");
					rebornBossConfig.Site = Global.GetSafeAttributeIntArray(xml, "Site", -1, '|');
					rebornBossConfig.Radius = (int)Global.GetSafeAttributeLong(xml, "Radius");
					rebornBossConfig.PursuitRadius = (int)Global.GetSafeAttributeLong(xml, "PursuitRadius");
					string safeAttributeStr = Global.GetSafeAttributeStr(xml, "Time");
					string[] array = safeAttributeStr.Split(new char[]
					{
						'|'
					});
					foreach (string s in array)
					{
						TimeSpan item;
						if (TimeSpan.TryParse(s, out item))
						{
							rebornBossConfig.RefreshTimePoints.Add(item);
						}
					}
					rebornBossConfig.RefreshTimePoints.Sort(delegate(TimeSpan left, TimeSpan right)
					{
						int result;
						if (left < right)
						{
							result = -1;
						}
						else if (left > right)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					if (!ConfigParser.ParserTimeRangeList(rebornBossConfig.TimePoints, Global.GetSafeAttributeStr(xml, "EffectiveTime"), true, '|', '-'))
					{
						LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", RebornDataConst.RebornBoss), null, true);
					}
					list.Add(rebornBossConfig);
				}
				foreach (List<RebornBossConfig> list2 in dictionary.Values)
				{
					list2.Sort(delegate(RebornBossConfig left, RebornBossConfig right)
					{
						int result;
						if (left.ID < right.ID)
						{
							result = -1;
						}
						else if (left.ID > right.ID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
				}
				lock (RebornBoss.Mutex)
				{
					this.RebornBossConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornBoss, ex.Message), null, true);
				return false;
			}
			return true;
		}

		public bool LoadRebornBossAwardConfigFile()
		{
			try
			{
				GeneralCachingXmlMgr.RemoveCachingXml(Global.GameResPath(RebornDataConst.RebornBossAward));
				XElement xelement = GeneralCachingXmlMgr.GetXElement(Global.GameResPath(RebornDataConst.RebornBossAward));
				if (null == xelement)
				{
					return false;
				}
				Dictionary<int, List<RebornBossAwardConfig>> dictionary = new Dictionary<int, List<RebornBossAwardConfig>>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xml in enumerable)
				{
					RebornBossAwardConfig rebornBossAwardConfig = new RebornBossAwardConfig();
					rebornBossAwardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
					rebornBossAwardConfig.MonstersID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
					rebornBossAwardConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
					rebornBossAwardConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
					rebornBossAwardConfig.AwardType = (int)Global.GetSafeAttributeLong(xml, "Type");
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsOne"), ref rebornBossAwardConfig.AwardsItemListOne, '|', ',');
					ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsTwo"), ref rebornBossAwardConfig.AwardsItemListTwo, '|', ',');
					List<RebornBossAwardConfig> list;
					if (!dictionary.TryGetValue(rebornBossAwardConfig.MonstersID, out list))
					{
						list = new List<RebornBossAwardConfig>();
						dictionary[rebornBossAwardConfig.MonstersID] = list;
					}
					list.Add(rebornBossAwardConfig);
				}
				lock (RebornBoss.Mutex)
				{
					this.RebornBossAwardConfigDict = dictionary;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("{0}解析出现异常, {1}", RebornDataConst.RebornBossAward, ex.Message), null, true);
				return false;
			}
			return true;
		}

		private void OnStartPlayGame(GameClient client)
		{
			if (GameManager.IsKuaFuServer)
			{
				RebornBossScene rebornBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
				{
					this.NotifyScoreInfo(client);
				}
			}
		}

		public void AddDelayCreateMonster(RebornBossScene scene, long ticks, object monster)
		{
			lock (RebornBoss.Mutex)
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

		public void CheckCreateDynamicMonster(RebornBossScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			while (scene.CreateMonsterQueue.Count > 0)
			{
				KeyValuePair<long, List<object>> keyValuePair = scene.CreateMonsterQueue.First<KeyValuePair<long, List<object>>>();
				if (num < keyValuePair.Key)
				{
					break;
				}
				try
				{
					foreach (object obj in keyValuePair.Value)
					{
						if (obj is RebornBossConfig)
						{
							RebornBossConfig rebornBossConfig = obj as RebornBossConfig;
							GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, rebornBossConfig.MonstersID, -1, 1, rebornBossConfig.Site[0] / 100, rebornBossConfig.Site[1] / 100, rebornBossConfig.Radius / 100, rebornBossConfig.PursuitRadius, 54, rebornBossConfig, null);
						}
					}
				}
				finally
				{
					scene.CreateMonsterQueue.RemoveAt(0);
				}
			}
		}

		public void ResortBossAttackRank(RebornBossScene scene)
		{
			scene.BossRankList.Sort(delegate(RebornBossAttackLog left, RebornBossAttackLog right)
			{
				int result;
				if (left.InjureSum > right.InjureSum)
				{
					result = -1;
				}
				else if (left.InjureSum < right.InjureSum)
				{
					result = 1;
				}
				else if (left.RoleID > right.RoleID)
				{
					result = -1;
				}
				else if (left.RoleID < right.RoleID)
				{
					result = 1;
				}
				else
				{
					result = 0;
				}
				return result;
			});
			for (int i = 0; i < scene.BossRankList.Count; i++)
			{
				int rankNum = scene.BossRankList[i].RankNum;
				if (i >= RebornDataConst.RebornBossRankCal)
				{
					scene.BossRankList[i].RankNum = 0;
				}
				else
				{
					scene.BossRankList[i].RankNum = i + 1;
				}
				scene.BossRankList[i].NotifySelf |= (scene.BossRankList[i].RankNum != rankNum);
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType && monster.Tag is RebornBossConfig)
			{
				RebornBossScene rebornBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
				{
					bool flag = false;
					bool flag2 = false;
					bool flag3 = false;
					lock (RebornBoss.Mutex)
					{
						rebornBossScene.scoreData.VLife = monster.VLife;
						rebornBossScene.scoreData.BossBeAttackTm = TimeUtil.NowDateTime();
						RebornBossAttackLog rebornBossAttackLog;
						if (!rebornBossScene.BossRankDict.TryGetValue(client.ClientData.RoleID, out rebornBossAttackLog))
						{
							rebornBossAttackLog = new RebornBossAttackLog();
							rebornBossAttackLog.UserPtID = client.ClientData.UserPTID;
							rebornBossAttackLog.ServerPtID = client.ClientData.ServerPTID;
							rebornBossAttackLog.RoleID = client.ClientData.RoleID;
							rebornBossAttackLog.Param = client.ClientData.Channel;
							rebornBossAttackLog.RoleName = Global.FormatRoleNameWithZoneId2(client);
							rebornBossAttackLog.ServerID = client.ServerId;
							rebornBossAttackLog.LocalRoleID = client.ClientData.LocalRoleID;
							rebornBossScene.BossRankDict[rebornBossAttackLog.RoleID] = rebornBossAttackLog;
							rebornBossScene.BossRankList.Add(rebornBossAttackLog);
							flag3 = true;
						}
						rebornBossAttackLog.InjureSum += injure;
						int num = (int)((double)rebornBossAttackLog.InjureSum / rebornBossScene.scoreData.VLifeMax * 100.0);
						if (rebornBossAttackLog.DamagePct != num)
						{
							flag3 = true;
							flag2 = true;
						}
						rebornBossAttackLog.DamagePct = num;
						if (!flag3)
						{
							RebornBossAttackLog rebornBossAttackLog2 = rebornBossScene.BossRankList[Math.Min(RebornDataConst.RebornBossRankShow, rebornBossScene.BossRankList.Count) - 1];
							if (rebornBossAttackLog2.RoleID == rebornBossAttackLog.RoleID || rebornBossAttackLog.InjureSum > rebornBossAttackLog2.InjureSum)
							{
								flag3 = true;
							}
						}
						if (flag3)
						{
							this.ResortBossAttackRank(rebornBossScene);
						}
						if (rebornBossAttackLog.RankNum > 0 && rebornBossAttackLog.RankNum <= RebornDataConst.RebornBossRankShow)
						{
							flag = true;
						}
						if (flag2 || rebornBossAttackLog.NotifySelf)
						{
							this.NotifyScoreInfo(client);
						}
						if (flag)
						{
							this.BroadCastScoreInfo(rebornBossScene);
						}
					}
				}
			}
		}

		public void GiveBossAward(GameClient client, RebornBossAwardConfig awardConfig, string strFrom)
		{
			List<AwardsItemData> items = awardConfig.AwardsItemListOne.Items;
			List<AwardsItemData> items2 = awardConfig.AwardsItemListTwo.Items;
			if (items != null)
			{
				foreach (AwardsItemData awardsItemData in items)
				{
					if (RebornEquip.IsRebornType(awardsItemData.GoodsID))
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 15000, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
					else
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
			}
			if (items2 != null)
			{
				foreach (AwardsItemData awardsItemData in items2)
				{
					if (Global.IsCanGiveRewardByOccupation(client, awardsItemData.GoodsID))
					{
						if (RebornEquip.IsRebornType(awardsItemData.GoodsID))
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 15000, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
						}
						else
						{
							Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, strFrom, "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
						}
					}
				}
			}
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (401 == monster.MonsterType)
			{
				RebornBossScene rebornBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
				{
					lock (RebornBoss.Mutex)
					{
						Dictionary<int, List<RebornBossAwardConfig>> rebornBossAwardConfigDict = this.RebornBossAwardConfigDict;
					}
					RebornBossAwardConfig killAwardConfig = this.GetKillAwardConfig(monster.MonsterInfo.ExtensionID);
					if (null != killAwardConfig)
					{
						List<KFRebornBossAwardData> rebornBossKillAwardList = this.GetRebornBossKillAwardList(client);
						KFRebornBossAwardData item = new KFRebornBossAwardData
						{
							MapCodeID = rebornBossScene.m_nMapCode,
							LineID = rebornBossScene.m_nLineID,
							ExtensionID = monster.MonsterInfo.ExtensionID
						};
						rebornBossKillAwardList.Add(item);
						this.SaveRebornBossKillAwardList(client, rebornBossKillAwardList);
					}
					lock (RebornBoss.Mutex)
					{
						rebornBossScene.BossState = RebornBossState.RBS_Dead;
						this.ResortBossAttackRank(rebornBossScene);
						for (int i = 0; i < rebornBossScene.BossRankList.Count; i++)
						{
							RebornBossAttackLog rebornBossAttackLog = rebornBossScene.BossRankList[i];
							string param = string.Format("{0},{1}", monster.MonsterInfo.ExtensionID, rebornBossAttackLog.RankNum);
							KuaFuWorldClient.getInstance().Reborn_RebornOpt(rebornBossAttackLog.ServerPtID, rebornBossAttackLog.LocalRoleID, 4, rebornBossScene.m_nMapCode, rebornBossScene.m_nLineID, param);
						}
					}
					this.PrintBossInfoGM(client, 10, monster);
				}
			}
		}

		public RebornBossConfig GetBossConfigByExtensionID(RebornBossScene scene, bool peekNext = false)
		{
			RebornBossConfig rebornBossConfig = null;
			Dictionary<int, List<RebornBossConfig>> dictionary = null;
			lock (RebornBoss.Mutex)
			{
				dictionary = this.RebornBossConfigDict;
			}
			List<RebornBossConfig> list;
			RebornBossConfig result;
			if (!dictionary.TryGetValue(scene.m_nMapCode, out list) || list.Count == 0)
			{
				result = rebornBossConfig;
			}
			else
			{
				int num = list.FindIndex((RebornBossConfig x) => x.MonstersID == scene.scoreData.BossExtensionID);
				if (peekNext)
				{
					num++;
				}
				if (num >= 0 && num < list.Count)
				{
					rebornBossConfig = list[num];
				}
				else
				{
					num = 0;
					rebornBossConfig = list[num];
				}
				result = rebornBossConfig;
			}
			return result;
		}

		public DateTime CalBossRefreshTime(RebornBossScene scene, RebornBossConfig config, DateTime now)
		{
			DateTime result = DateTime.MinValue;
			if (now.TimeOfDay < config.RefreshTimePoints[0])
			{
				result = new DateTime(now.Year, now.Month, now.Day).Add(config.RefreshTimePoints[0]);
			}
			else if (now.TimeOfDay >= config.RefreshTimePoints[config.RefreshTimePoints.Count - 1])
			{
				result = new DateTime(now.Year, now.Month, now.Day).AddDays(1.0).Add(config.RefreshTimePoints[0]);
			}
			else
			{
				TimeSpan value = config.RefreshTimePoints.Find((TimeSpan x) => now.TimeOfDay < x);
				result = new DateTime(now.Year, now.Month, now.Day).Add(value);
			}
			return result;
		}

		public void CheckCreateBossState(RebornBossScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			lock (RebornBoss.Mutex)
			{
				if (Math.Abs(num - RebornBoss.LastHeartBeatTicks_Boss) < 3000L)
				{
					return;
				}
				RebornBoss.LastHeartBeatTicks_Boss = num;
			}
			string param;
			if (now >= scene.scoreData.BossRefreshTime)
			{
				param = string.Format("{0},{1}", scene.scoreData.BossExtensionID, "");
			}
			else
			{
				param = string.Format("{0},{1}", scene.scoreData.BossExtensionID, scene.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss"));
			}
			KuaFuWorldClient.getInstance().Reborn_RebornOpt(-1, -1, 6, scene.m_nMapCode, scene.m_nLineID, param);
		}

		public void CheckCreateBoss(RebornBossScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			if (!(now < scene.scoreData.BossRefreshTime))
			{
				bool flag = false;
				if (scene.BossState == RebornBossState.RBS_None)
				{
					RebornBossConfig bossConfigByExtensionID = this.GetBossConfigByExtensionID(scene, false);
					if (null != bossConfigByExtensionID)
					{
						scene.BossRankDict.Clear();
						scene.BossRankList.Clear();
						scene.BossState = RebornBossState.RBS_Init;
						scene.scoreData.BossExtensionID = bossConfigByExtensionID.MonstersID;
						scene.scoreData.BossRefreshTime = now;
						scene.scoreData.BossBeAttackTm = now;
						scene.SaveSceneDBInfo();
						GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, bossConfigByExtensionID.MonstersID, -1, 1, bossConfigByExtensionID.Site[0] / 100, bossConfigByExtensionID.Site[1] / 100, bossConfigByExtensionID.Radius / 100, bossConfigByExtensionID.PursuitRadius, 54, bossConfigByExtensionID, null);
						flag = true;
					}
				}
				else if (scene.BossState == RebornBossState.RBS_Init)
				{
					if ((now - scene.scoreData.BossBeAttackTm).TotalSeconds > (double)this.RebornBossRankClearSec)
					{
						scene.BossRankList.Clear();
						scene.BossRankDict.Clear();
						scene.scoreData.BossBeAttackTm = now;
						Monster monster = GameManager.MonsterMgr.FindMonster(scene.m_nMapCode, scene.scoreData.MonsterID);
						if (null != monster)
						{
							GameManager.MonsterMgr.AddSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, scene.scoreData.VLifeMax);
							scene.scoreData.VLife = monster.VLife;
						}
						flag = true;
					}
				}
				else if (scene.BossState == RebornBossState.RBS_Dead)
				{
					RebornBossConfig bossConfigByExtensionID = this.GetBossConfigByExtensionID(scene, true);
					if (null != bossConfigByExtensionID)
					{
						scene.BossState = RebornBossState.RBS_None;
						scene.scoreData.BossExtensionID = bossConfigByExtensionID.MonstersID;
						scene.scoreData.BossRefreshTime = this.CalBossRefreshTime(scene, bossConfigByExtensionID, now);
						scene.SaveSceneDBInfo();
						flag = true;
					}
				}
				if (flag)
				{
					this.BroadCastScoreInfo(scene);
				}
			}
		}

		public void NotifyScoreInfo(GameClient client)
		{
			lock (RebornBoss.Mutex)
			{
				RebornBossScene rebornBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
				{
					if (rebornBossScene.scoreData.VLife > 0.0 && rebornBossScene.scoreData.VLifeMax > 0.0)
					{
						rebornBossScene.scoreData.LeftLifePct = (int)(rebornBossScene.scoreData.VLife / rebornBossScene.scoreData.VLifeMax * 100.0);
						rebornBossScene.scoreData.NextTime = "";
					}
					else
					{
						rebornBossScene.scoreData.LeftLifePct = 0;
						rebornBossScene.scoreData.NextTime = rebornBossScene.scoreData.BossRefreshTime.ToString("yyyy-MM-dd HH:mm:ss");
					}
					rebornBossScene.scoreData.SelfRankNum = 0;
					rebornBossScene.scoreData.SelfDamagePct = 0;
					RebornBossAttackLog rebornBossAttackLog;
					if (rebornBossScene.BossRankDict.TryGetValue(client.ClientData.RoleID, out rebornBossAttackLog))
					{
						rebornBossScene.scoreData.SelfRankNum = rebornBossAttackLog.RankNum;
						rebornBossScene.scoreData.SelfDamagePct = rebornBossAttackLog.DamagePct;
						rebornBossAttackLog.NotifySelf = false;
					}
					rebornBossScene.scoreData.rankList = rebornBossScene.BossRankList.GetRange(0, Math.Min(rebornBossScene.BossRankList.Count, RebornDataConst.RebornBossRankShow));
					client.sendCmd<RebornBossScoreData>(1716, rebornBossScene.scoreData, false);
				}
			}
		}

		public void BroadCastScoreInfo(RebornBossScene scene)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(scene.m_nMapCode);
			if (mapClients != null && mapClients.Count != 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						this.NotifyScoreInfo(gameClient);
					}
				}
			}
		}

		public void BuildFakeBossInfoGM(GameClient client, int fakeNum)
		{
			lock (RebornBoss.Mutex)
			{
				RebornBossScene rebornBossScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
				{
					for (int i = 0; i < fakeNum; i++)
					{
						RebornBossAttackLog rebornBossAttackLog = new RebornBossAttackLog();
						do
						{
							rebornBossAttackLog.RoleID = Global.GetRandomNumber(-2000000000, -1);
						}
						while (rebornBossScene.BossRankDict.ContainsKey(rebornBossAttackLog.RoleID));
						rebornBossAttackLog.UserPtID = 1;
						rebornBossAttackLog.ServerPtID = 1;
						rebornBossAttackLog.Param = "FAKE";
						rebornBossAttackLog.RoleName = string.Format("FAKE{0}", rebornBossAttackLog.RoleID);
						rebornBossAttackLog.ServerID = -999;
						rebornBossAttackLog.LocalRoleID = rebornBossAttackLog.RoleID;
						rebornBossAttackLog.InjureSum = (long)Global.GetRandomNumber(1000000, 2000000);
						rebornBossScene.BossRankDict[rebornBossAttackLog.RoleID] = rebornBossAttackLog;
						rebornBossScene.BossRankList.Add(rebornBossAttackLog);
					}
					this.ResortBossAttackRank(rebornBossScene);
					this.BroadCastScoreInfo(rebornBossScene);
				}
			}
		}

		public void PrintBossInfoGM(GameClient client, int logNum = 2147483647, Monster deadBoss = null)
		{
			RebornBossScene rebornBossScene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out rebornBossScene))
			{
				lock (RebornBoss.Mutex)
				{
					int num = 0;
					foreach (RebornBossAttackLog rebornBossAttackLog in rebornBossScene.BossRankList)
					{
						if (++num > logNum)
						{
							break;
						}
						LogManager.WriteLog(5, string.Format("RebornBoss ranknum={0} userptid={1} channel={2} rname={3} localrid={4} rid={5} injure={6} serverptid={7}", new object[]
						{
							rebornBossAttackLog.RankNum,
							rebornBossAttackLog.UserPtID,
							rebornBossAttackLog.Param,
							rebornBossAttackLog.RoleName,
							rebornBossAttackLog.LocalRoleID,
							rebornBossAttackLog.RoleID,
							rebornBossAttackLog.InjureSum,
							rebornBossAttackLog.ServerPtID
						}), null, true);
					}
				}
				if (deadBoss != null && deadBoss.LastDeadTicks > 0L)
				{
					DateTime dateTime = new DateTime(deadBoss.LastDeadTicks);
					DateTime dateTime2 = new DateTime(deadBoss.GetMonsterBirthTick());
					long num2 = deadBoss.LastDeadTicks - deadBoss.GetMonsterBirthTick();
					LogManager.WriteLog(5, string.Format("RebornBoss birthtm={0} deadtm={1} aliveSeconds={2}", dateTime2, dateTime, num2 / 10000000L), null, true);
				}
			}
		}

		public List<KFRebornBossAwardData> GetRebornBossKillAwardList(GameClient client)
		{
			List<KFRebornBossAwardData> list = new List<KFRebornBossAwardData>();
			string roleParamByName = Global.GetRoleParamByName(client, "157");
			List<KFRebornBossAwardData> result;
			if (string.IsNullOrEmpty(roleParamByName))
			{
				result = list;
			}
			else
			{
				string[] array = roleParamByName.Split(new char[]
				{
					'|'
				});
				foreach (string text in array)
				{
					string[] array3 = text.Split(new char[]
					{
						','
					});
					if (array3.Length >= 3)
					{
						list.Add(new KFRebornBossAwardData
						{
							MapCodeID = Convert.ToInt32(array3[0]),
							LineID = Convert.ToInt32(array3[1]),
							ExtensionID = Convert.ToInt32(array3[2])
						});
					}
				}
				result = list;
			}
			return result;
		}

		public void SaveRebornBossKillAwardList(GameClient client, List<KFRebornBossAwardData> awardlist)
		{
			string text = "";
			foreach (KFRebornBossAwardData kfrebornBossAwardData in awardlist)
			{
				text += string.Format("{0},{1},{2}|", kfrebornBossAwardData.MapCodeID, kfrebornBossAwardData.LineID, kfrebornBossAwardData.ExtensionID);
			}
			if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
			{
				text = text.Substring(0, text.Length - 1);
			}
			Global.UpdateRoleParamByName(client, "157", text, true);
		}

		public void TimerProc_fuBenWorker()
		{
			if (GameManager.IsKuaFuServer)
			{
				DateTime now = TimeUtil.NowDateTime();
				long num = now.Ticks / 10000L;
				Dictionary<int, List<RebornBossConfig>> rebornBossConfigDict;
				lock (RebornBoss.Mutex)
				{
					if (Math.Abs(num - RebornBoss.LastHeartBeatTicks) < 1000L)
					{
						return;
					}
					RebornBoss.LastHeartBeatTicks = num;
					rebornBossConfigDict = this.RebornBossConfigDict;
				}
				foreach (KeyValuePair<int, List<RebornBossConfig>> keyValuePair in rebornBossConfigDict)
				{
					if (!this.KuaFuLineDataDict.ContainsKey(keyValuePair.Key))
					{
						List<KuaFuLineData> list = KuaFuWorldClient.getInstance().GetKuaFuLineDataList(keyValuePair.Key) as List<KuaFuLineData>;
						if (null != list)
						{
							this.KuaFuLineDataDict[keyValuePair.Key] = list;
						}
					}
				}
				foreach (KeyValuePair<int, List<KuaFuLineData>> keyValuePair2 in this.KuaFuLineDataDict)
				{
					KuaFuLineData kuaFuLineData = keyValuePair2.Value.Find((KuaFuLineData x) => x.ServerId == GameManager.KuaFuServerId);
					if (kuaFuLineData != null && !this.SceneDict.ContainsKey(keyValuePair2.Key))
					{
						RebornBossScene rebornBossScene = new RebornBossScene();
						rebornBossScene.m_nMapCode = keyValuePair2.Key;
						rebornBossScene.m_nLineID = kuaFuLineData.Line;
						rebornBossScene.LoadSceneDBInfo();
						this.SceneDict[keyValuePair2.Key] = rebornBossScene;
					}
				}
				foreach (RebornBossScene scene in this.SceneDict.Values)
				{
					lock (RebornBoss.Mutex)
					{
						this.CheckCreateDynamicMonster(scene, now);
						this.CheckCreateBoss(scene, now);
						this.CheckCreateBossState(scene, now);
					}
				}
			}
		}

		private static object Mutex = new object();

		private static long LastHeartBeatTicks = 0L;

		private static long LastHeartBeatTicks_Boss = 0L;

		public ConcurrentDictionary<int, RebornBossScene> SceneDict = new ConcurrentDictionary<int, RebornBossScene>();

		public Dictionary<int, List<RebornBossData>> BossDataDict = new Dictionary<int, List<RebornBossData>>();

		public Dictionary<int, List<KuaFuLineData>> KuaFuLineDataDict = new Dictionary<int, List<KuaFuLineData>>();

		public Dictionary<int, List<RebornBossConfig>> RebornBossConfigDict = new Dictionary<int, List<RebornBossConfig>>();

		public Dictionary<int, List<RebornBossAwardConfig>> RebornBossAwardConfigDict = new Dictionary<int, List<RebornBossAwardConfig>>();

		public int RebornBossRankClearSec = 120;

		private static RebornBoss instance = new RebornBoss();
	}
}
