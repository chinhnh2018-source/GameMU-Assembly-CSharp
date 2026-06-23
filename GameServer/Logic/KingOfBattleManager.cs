using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KingOfBattleManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static KingOfBattleManager getInstance()
		{
			return KingOfBattleManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KingOfBattleManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1180, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1181, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1183, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1188, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1182, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1190, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1191, 3, 3, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1192, 1, 1, KingOfBattleManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10001, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 39, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(31, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KingOfBattleManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10001, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 39, KingOfBattleManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 39, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(31, KingOfBattleManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KingOfBattleManager.getInstance());
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
			case 1180:
				return this.ProcessKingOfBattleJoinCmd(client, nID, bytes, cmdParams);
			case 1181:
				return this.ProcessKingOfBattleEnterCmd(client, nID, bytes, cmdParams);
			case 1182:
				return this.ProcessGetKingOfBattleAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1183:
				return this.ProcessGetKingOfBattleStateCmd(client, nID, bytes, cmdParams);
			case 1188:
				return this.ProcessGetKingOfBattleAwardCmd(client, nID, bytes, cmdParams);
			case 1190:
				return this.ProcessGetKingOfBattleMallDataCmd(client, nID, bytes, cmdParams);
			case 1191:
				return this.ProcessKingOfBattleMallBuyCmd(client, nID, bytes, cmdParams);
			case 1192:
				return this.ProcessKingOfBattleMallRefreshCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 31)
			{
				ClientRegionEventObject clientRegionEventObject = eventObject as ClientRegionEventObject;
				if (null != clientRegionEventObject)
				{
					if (clientRegionEventObject.EventType == 1 && clientRegionEventObject.Flag == 1)
					{
						this.SubmitCrystalBuff(clientRegionEventObject.Client, clientRegionEventObject.AreaLuaID);
					}
				}
			}
			if (eventType == 10)
			{
				PlayerDeadEventObject playerDeadEventObject = eventObject as PlayerDeadEventObject;
				if (null != playerDeadEventObject)
				{
					if (playerDeadEventObject.Type == PlayerDeadEventTypes.ByRole)
					{
						this.OnKillRole(playerDeadEventObject.getAttackerRole(), playerDeadEventObject.getPlayer());
					}
					GameClient player = playerDeadEventObject.getPlayer();
					if (null != player)
					{
						KingOfBattleScene scene;
						if (this.SceneDict.TryGetValue(player.ClientData.FuBenSeqID, out scene))
						{
							this.RemoveBattleSceneBuffForRole(scene, player);
						}
					}
				}
			}
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
			{
			case 27:
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
				break;
			}
			case 28:
			case 31:
			case 32:
				break;
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
					KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig = onCreateMonsterEventObject.Monster.Tag as KingOfBattleQiZhiConfig;
					if (null != kingOfBattleQiZhiConfig)
					{
						onCreateMonsterEventObject.Monster.Camp = kingOfBattleQiZhiConfig.BattleWhichSide;
						onCreateMonsterEventObject.Result = true;
						onCreateMonsterEventObject.Handled = true;
					}
					KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = onCreateMonsterEventObject.Monster.Tag as KingOfBattleDynamicMonsterItem;
					if (null != kingOfBattleDynamicMonsterItem)
					{
						if (kingOfBattleDynamicMonsterItem.MonsterType == 1)
						{
							onCreateMonsterEventObject.Monster.Camp = onCreateMonsterEventObject.Monster.MonsterInfo.Camp;
							onCreateMonsterEventObject.Result = true;
							onCreateMonsterEventObject.Handled = true;
						}
						else if (kingOfBattleDynamicMonsterItem.MonsterType == 2 || kingOfBattleDynamicMonsterItem.MonsterType == 3)
						{
							if (1301 == onCreateMonsterEventObject.Monster.MonsterType)
							{
								onCreateMonsterEventObject.Monster.Camp = 1;
							}
							else if (1302 == onCreateMonsterEventObject.Monster.MonsterType)
							{
								onCreateMonsterEventObject.Monster.Camp = 2;
							}
							else if (1303 == onCreateMonsterEventObject.Monster.MonsterType)
							{
								onCreateMonsterEventObject.Monster.Camp = 3;
							}
							onCreateMonsterEventObject.Result = true;
							onCreateMonsterEventObject.Handled = true;
						}
					}
				}
				break;
			}
			case 33:
			{
				PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
				if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 39)
				{
					Monster monster = preMonsterInjureEventObject.Monster;
					if (monster != null)
					{
						if (monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID1 || monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID2)
						{
							preMonsterInjureEventObject.Injure = this.RuntimeData.KingOfBattleDamageJunQi;
							eventObject.Handled = true;
							eventObject.Result = true;
						}
						KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = monster.Tag as KingOfBattleDynamicMonsterItem;
						if (kingOfBattleDynamicMonsterItem != null)
						{
							if (kingOfBattleDynamicMonsterItem.MonsterType == 1)
							{
								preMonsterInjureEventObject.Injure = this.RuntimeData.KingOfBattleDamageCenter;
								eventObject.Handled = true;
								eventObject.Result = true;
							}
							else if (kingOfBattleDynamicMonsterItem.MonsterType == 2 || kingOfBattleDynamicMonsterItem.MonsterType == 3)
							{
								preMonsterInjureEventObject.Injure = this.RuntimeData.KingOfBattleDamageTower;
								eventObject.Handled = true;
								eventObject.Result = true;
							}
						}
					}
				}
				break;
			}
			default:
				switch (num)
				{
				case 10001:
				{
					KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
					if (null != kuaFuNotifyEnterGameEvent)
					{
						KuaFuServerLoginData kuaFuServerLoginData = kuaFuNotifyEnterGameEvent.Arg as KuaFuServerLoginData;
						if (null != kuaFuServerLoginData)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.RuntimeData.RoleIdKuaFuLoginDataDict[kuaFuServerLoginData.RoleId] = kuaFuServerLoginData;
								LogManager.WriteLog(2, string.Format("通知角色ID={0}拥有进入王者战场资格,跨服GameID={1}", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
							}
						}
						eventObject.Handled = true;
					}
					break;
				}
				case 10002:
				{
					CaiJiEventObject caiJiEventObject = eventObject as CaiJiEventObject;
					if (null != caiJiEventObject)
					{
						GameClient client = caiJiEventObject.Source as GameClient;
						Monster monster2 = caiJiEventObject.Target as Monster;
						this.OnCaiJiFinish(client, monster2);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
					break;
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
					this.RuntimeData.BattleCrystalMonsterDict.Clear();
					text = "Config/KingOfBattleCrystalMonster.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BattleCrystalMonsterItem battleCrystalMonsterItem = new BattleCrystalMonsterItem();
						battleCrystalMonsterItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						battleCrystalMonsterItem.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						battleCrystalMonsterItem.GatherTime = (int)Global.GetSafeAttributeLong(xml, "GatherTime");
						battleCrystalMonsterItem.BattleJiFen = (int)Global.GetSafeAttributeLong(xml, "BattleJiFen");
						battleCrystalMonsterItem.PosX = (int)Global.GetSafeAttributeLong(xml, "X");
						battleCrystalMonsterItem.PosY = (int)Global.GetSafeAttributeLong(xml, "Y");
						battleCrystalMonsterItem.FuHuoTime = (int)Global.GetSafeAttributeLong(xml, "FuHuoTime") * 1000;
						battleCrystalMonsterItem.BuffGoodsID = (int)Global.GetSafeAttributeLong(xml, "GoodsID");
						battleCrystalMonsterItem.BuffTime = (int)Global.GetSafeAttributeLong(xml, "Time");
						this.RuntimeData.BattleCrystalMonsterDict[battleCrystalMonsterItem.Id] = battleCrystalMonsterItem;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config/KingOfBattleRebirth.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						YongZheZhanChangBirthPoint yongZheZhanChangBirthPoint = new YongZheZhanChangBirthPoint();
						yongZheZhanChangBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						yongZheZhanChangBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						yongZheZhanChangBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						yongZheZhanChangBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[yongZheZhanChangBirthPoint.ID] = yongZheZhanChangBirthPoint;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					text = "Config/KingOfBattle.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KingOfBattleSceneInfo kingOfBattleSceneInfo = new KingOfBattleSceneInfo();
						int num = (int)Global.GetSafeAttributeLong(xml, "Group");
						int num2 = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						kingOfBattleSceneInfo.Id = num;
						kingOfBattleSceneInfo.MapCode = num2;
						kingOfBattleSceneInfo.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
						kingOfBattleSceneInfo.MaxLevel = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
						kingOfBattleSceneInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
						kingOfBattleSceneInfo.MaxZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
						kingOfBattleSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						kingOfBattleSceneInfo.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						kingOfBattleSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						kingOfBattleSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						ConfigParser.ParseStrInt2(Global.GetSafeAttributeStr(xml, "ApplyTime"), ref kingOfBattleSceneInfo.SignUpStartSecs, ref kingOfBattleSceneInfo.SignUpEndSecs, ',');
						kingOfBattleSceneInfo.SignUpStartSecs += kingOfBattleSceneInfo.SignUpEndSecs;
						if (!ConfigParser.ParserTimeRangeListWithDay(kingOfBattleSceneInfo.TimePoints, Global.GetSafeAttributeStr(xml, "TimePoints"), true, '|', '-', ','))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("读取{0}时间配置(TimePoints)出错", text), null, true);
						}
						for (int i = 0; i < kingOfBattleSceneInfo.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(kingOfBattleSceneInfo.TimePoints[i].Hours, kingOfBattleSceneInfo.TimePoints[i].Minutes, kingOfBattleSceneInfo.TimePoints[i].Seconds);
							kingOfBattleSceneInfo.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(num2, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, num2), null, true);
						}
						RangeKey key = new RangeKey(Global.GetUnionLevel(kingOfBattleSceneInfo.MinZhuanSheng, kingOfBattleSceneInfo.MinLevel, false), Global.GetUnionLevel(kingOfBattleSceneInfo.MaxZhuanSheng, kingOfBattleSceneInfo.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[key] = kingOfBattleSceneInfo;
						this.RuntimeData.SceneDataDict[num] = kingOfBattleSceneInfo;
					}
					text = "Config/KingOfBattleAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						int num = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						KingOfBattleSceneInfo kingOfBattleSceneInfo;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out kingOfBattleSceneInfo))
						{
							kingOfBattleSceneInfo.Exp = Global.GetSafeAttributeLong(xml, "Exp");
							kingOfBattleSceneInfo.BandJinBi = (int)Global.GetSafeAttributeLong(xml, "BandJinBi");
							kingOfBattleSceneInfo.AwardMinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
							kingOfBattleSceneInfo.AwardMaxLevel = (int)Global.GetSafeAttributeLong(xml, "MaxLevel");
							kingOfBattleSceneInfo.AwardMinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
							kingOfBattleSceneInfo.AwardMaxZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MaxZhuanSheng");
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "WinGoods"), ref kingOfBattleSceneInfo.WinAwardsItemList, '|', ',');
							ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "LoseGoods"), ref kingOfBattleSceneInfo.LoseAwardsItemList, '|', ',');
						}
					}
					this.RuntimeData.SceneDynMonsterDict.Clear();
					text = "Config/KingOfBattleMonster.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					HashSet<int> hashSet = new HashSet<int>();
					foreach (XElement xml in enumerable)
					{
						KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = new KingOfBattleDynamicMonsterItem();
						kingOfBattleDynamicMonsterItem.Id = (int)Global.GetSafeAttributeLong(xml, "ID");
						kingOfBattleDynamicMonsterItem.MapCode = (int)Global.GetSafeAttributeLong(xml, "CodeID");
						kingOfBattleDynamicMonsterItem.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonsterID");
						kingOfBattleDynamicMonsterItem.MonsterType = (int)Global.GetSafeAttributeLong(xml, "MonsterType");
						kingOfBattleDynamicMonsterItem.RebornID = (int)Global.GetSafeAttributeLong(xml, "RebornID");
						kingOfBattleDynamicMonsterItem.PosX = (int)Global.GetSafeAttributeLong(xml, "X");
						kingOfBattleDynamicMonsterItem.PosY = (int)Global.GetSafeAttributeLong(xml, "Y");
						kingOfBattleDynamicMonsterItem.DelayBirthMs = (int)Global.GetSafeAttributeLong(xml, "Time") * 1000;
						kingOfBattleDynamicMonsterItem.PursuitRadius = (int)Global.GetSafeAttributeLong(xml, "PursuitRadius");
						kingOfBattleDynamicMonsterItem.BuffTime = (int)Global.GetSafeAttributeLong(xml, "BuffTime");
						string[] array = Global.GetSafeAttributeStr(xml, "JiFen").Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							kingOfBattleDynamicMonsterItem.JiFenDamage = Global.SafeConvertToInt32(array[0]);
							kingOfBattleDynamicMonsterItem.JiFenKill = Global.SafeConvertToInt32(array[1]);
						}
						string[] array2 = Global.GetSafeAttributeStr(xml, "Buff").Split(new char[]
						{
							'|'
						});
						for (int j = 0; j < array2.Length; j++)
						{
							string[] array3 = array2[j].Split(new char[]
							{
								','
							});
							if (array3.Length == 2)
							{
								KingOfBattleRandomBuff kingOfBattleRandomBuff = new KingOfBattleRandomBuff();
								kingOfBattleRandomBuff.GoodsID = Global.SafeConvertToInt32(array3[0]);
								kingOfBattleRandomBuff.Pct = Global.SafeConvertToDouble(array3[1]);
								kingOfBattleDynamicMonsterItem.RandomBuffList.Add(kingOfBattleRandomBuff);
							}
						}
						List<KingOfBattleDynamicMonsterItem> list = null;
						if (!this.RuntimeData.SceneDynMonsterDict.TryGetValue(kingOfBattleDynamicMonsterItem.MapCode, out list))
						{
							list = new List<KingOfBattleDynamicMonsterItem>();
							this.RuntimeData.SceneDynMonsterDict[kingOfBattleDynamicMonsterItem.MapCode] = list;
						}
						this.RuntimeData.DynMonsterDict[kingOfBattleDynamicMonsterItem.Id] = kingOfBattleDynamicMonsterItem;
						if (kingOfBattleDynamicMonsterItem.RebornID != -1)
						{
							hashSet.Add(kingOfBattleDynamicMonsterItem.RebornID);
						}
						list.Add(kingOfBattleDynamicMonsterItem);
					}
					foreach (KeyValuePair<int, KingOfBattleDynamicMonsterItem> keyValuePair in this.RuntimeData.DynMonsterDict)
					{
						if (hashSet.Contains(keyValuePair.Value.Id))
						{
							keyValuePair.Value.RebornBirth = true;
						}
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					text = "Config/KingOfBattleQiZuo.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig = new KingOfBattleQiZhiConfig();
						kingOfBattleQiZhiConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "NPCID");
						kingOfBattleQiZhiConfig.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						kingOfBattleQiZhiConfig.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						kingOfBattleQiZhiConfig.QiZhiMonsterID = (int)Global.GetSafeAttributeLong(xml, "Monster");
						this.RuntimeData.NPCID2QiZhiConfigDict[kingOfBattleQiZhiConfig.NPCID] = kingOfBattleQiZhiConfig;
					}
					this.RuntimeData.KingOfBattleStoreDict.Clear();
					this.RuntimeData.KingOfBattleStoreList.Clear();
					text = "Config/KingOfBattleStore.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KingOfBattleStoreConfig kingOfBattleStoreConfig = new KingOfBattleStoreConfig();
						kingOfBattleStoreConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						kingOfBattleStoreConfig.SaleData = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "GoodsID").Split(new char[]
						{
							','
						}), 0);
						kingOfBattleStoreConfig.JiFen = (int)Global.GetSafeAttributeLong(xml, "WangZheJiFen");
						kingOfBattleStoreConfig.SinglePurchase = (int)Global.GetSafeAttributeLong(xml, "SinglePurchase");
						kingOfBattleStoreConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						kingOfBattleStoreConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						kingOfBattleStoreConfig.RandNumMinus = kingOfBattleStoreConfig.EndNum - kingOfBattleStoreConfig.BeginNum + 1;
						this.RuntimeData.KingOfBattleStoreDict[kingOfBattleStoreConfig.ID] = kingOfBattleStoreConfig;
						this.RuntimeData.KingOfBattleStoreList.Add(kingOfBattleStoreConfig);
					}
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleAttackBuild", ',');
					if (paramValueIntArrayByName.Length == 3)
					{
						this.RuntimeData.KingOfBattleDamageJunQi = paramValueIntArrayByName[0];
						this.RuntimeData.KingOfBattleDamageTower = paramValueIntArrayByName[1];
						this.RuntimeData.KingOfBattleDamageCenter = paramValueIntArrayByName[2];
					}
					this.RuntimeData.KingOfBattleDie = (int)GameManager.systemParamsList.GetParamValueIntByName("KingOfBattleDie", -1);
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleUltraKill", ',');
					if (paramValueIntArrayByName.Length == 4)
					{
						this.RuntimeData.KingOfBattleUltraKillParam1 = paramValueIntArrayByName[0];
						this.RuntimeData.KingOfBattleUltraKillParam2 = paramValueIntArrayByName[1];
						this.RuntimeData.KingOfBattleUltraKillParam3 = paramValueIntArrayByName[2];
						this.RuntimeData.KingOfBattleUltraKillParam4 = paramValueIntArrayByName[3];
					}
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleShutDown", ',');
					if (paramValueIntArrayByName.Length == 4)
					{
						this.RuntimeData.KingOfBattleShutDownParam1 = paramValueIntArrayByName[0];
						this.RuntimeData.KingOfBattleShutDownParam2 = paramValueIntArrayByName[1];
						this.RuntimeData.KingOfBattleShutDownParam3 = paramValueIntArrayByName[2];
						this.RuntimeData.KingOfBattleShutDownParam4 = paramValueIntArrayByName[3];
					}
					this.RuntimeData.KingOfBattleLowestJiFen = (int)GameManager.systemParamsList.GetParamValueIntByName("KingOfBattleLowestJiFen", -1);
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("KingOfBattleStore", ',');
					if (paramValueIntArrayByName.Length == 3)
					{
						this.RuntimeData.KingOfBattleStoreRefreshTm = paramValueIntArrayByName[0];
						this.RuntimeData.KingOfBattleStoreRefreshNum = paramValueIntArrayByName[1];
						this.RuntimeData.KingOfBattleStoreRefreshCost = paramValueIntArrayByName[2];
					}
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
			bool flag = false;
			bool flag2 = false;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				bool flag4 = false;
				KingOfBattleSceneInfo kingOfBattleSceneInfo = this.RuntimeData.SceneDataDict.Values.FirstOrDefault<KingOfBattleSceneInfo>();
				for (int i = 0; i < kingOfBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)kingOfBattleSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= kingOfBattleSceneInfo.SecondsOfDay[i] - (double)kingOfBattleSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= kingOfBattleSceneInfo.SecondsOfDay[i + 1])
					{
						double num = kingOfBattleSceneInfo.SecondsOfDay[i] - dateTime.TimeOfDay.TotalSeconds;
						flag4 = true;
						if (!this.RuntimeData.PrepareGame)
						{
							if (num > 0.0 && num < (double)(kingOfBattleSceneInfo.SignUpEndSecs / 2))
							{
								LogManager.WriteLog(2, "报名截止5分钟时间过半,通知跨服中心开始分配所有报名玩家的活动场次", null, true);
								this.RuntimeData.PrepareGame = true;
								flag = true;
								break;
							}
						}
						else if (num < 0.0)
						{
							LogManager.WriteLog(2, "报名截止状态结束,可以通知已分配到场次的玩家进入游戏了", null, true);
							flag2 = true;
							this.RuntimeData.PrepareGame = false;
							break;
						}
					}
				}
				if (!flag4)
				{
					if (this.RuntimeData.RoleIdKuaFuLoginDataDict.Count > 0)
					{
						this.RuntimeData.RoleIdKuaFuLoginDataDict.Clear();
					}
					if (this.RuntimeData.RoleId2JoinGroup.Count > 0)
					{
						this.RuntimeData.RoleId2JoinGroup.Clear();
					}
				}
			}
			if (flag)
			{
				LogManager.WriteLog(2, "通知跨服中心开始分配所有报名玩家的活动场次", null, true);
				string cmd = string.Format("{0} {1} {2}", "GameState", 2, 15);
				YongZheZhanChangClient.getInstance().ExecuteCommand(cmd);
			}
			if (flag2)
			{
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuServerLoginData kuaFuServerLoginData in this.RuntimeData.RoleIdKuaFuLoginDataDict.Values)
					{
						this.RuntimeData.NotifyRoleEnterDict.Add(kuaFuServerLoginData.RoleId, kuaFuServerLoginData);
					}
				}
			}
			List<KuaFuServerLoginData> list = null;
			lock (this.RuntimeData.Mutex)
			{
				int count = this.RuntimeData.NotifyRoleEnterDict.Count;
				if (count > 0)
				{
					list = new List<KuaFuServerLoginData>();
					KuaFuServerLoginData kuaFuServerLoginData = this.RuntimeData.NotifyRoleEnterDict.First<KeyValuePair<int, KuaFuServerLoginData>>().Value;
					foreach (KeyValuePair<int, KuaFuServerLoginData> keyValuePair in this.RuntimeData.NotifyRoleEnterDict)
					{
						if (keyValuePair.Key % 15 == kuaFuServerLoginData.RoleId % 15)
						{
							list.Add(keyValuePair.Value);
						}
					}
					foreach (KuaFuServerLoginData kuaFuServerLoginData2 in list)
					{
						this.RuntimeData.NotifyRoleEnterDict.Remove(kuaFuServerLoginData2.RoleId);
					}
				}
			}
			if (null != list)
			{
				foreach (KuaFuServerLoginData kuaFuServerLoginData in list)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuServerLoginData.RoleId);
					if (null != gameClient)
					{
						gameClient.sendCmd<int>(1181, 1, false);
					}
				}
			}
		}

		public bool ProcessKingOfBattleJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (this.IsGongNengOpened(client, false))
				{
					KingOfBattleSceneInfo kingOfBattleSceneInfo = null;
					KingOfBattleGameStates kingOfBattleGameStates = KingOfBattleGameStates.None;
					if (!this.CheckMap(client))
					{
						num = -21;
					}
					else
					{
						num = this.CheckCondition(client, ref kingOfBattleSceneInfo, ref kingOfBattleGameStates);
					}
					if (kingOfBattleGameStates != KingOfBattleGameStates.SignUp)
					{
						num = -2001;
					}
					else if (this.RuntimeData.RoleId2JoinGroup.ContainsKey(client.ClientData.RoleID))
					{
						num = -12;
					}
					if (num >= 0)
					{
						int id = kingOfBattleSceneInfo.Id;
						num = YongZheZhanChangClient.getInstance().YongZheZhanChangSignUp(client.strUserID, client.ClientData.RoleID, client.ClientData.ZoneID, 15, id, client.ClientData.CombatForce);
						if (num > 0)
						{
							this.RuntimeData.RoleId2JoinGroup[client.ClientData.RoleID] = id;
							client.ClientData.SignUpGameType = 15;
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
			return false;
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == 0;
		}

		private int CheckCondition(GameClient client, ref KingOfBattleSceneInfo sceneItem, ref KingOfBattleGameStates state)
		{
			int result = 0;
			sceneItem = null;
			if (!this.IsGongNengOpened(client, true))
			{
				result = -13;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.LevelRangeSceneIdDict.TryGetValue(new RangeKey(Global.GetUnionLevel(client, false)), out sceneItem))
					{
						return -12;
					}
				}
				result = -2001;
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.RuntimeData.Mutex)
				{
					for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
					{
						if (dateTime.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1])
						{
							if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpStartSecs)
							{
								state = KingOfBattleGameStates.None;
								result = -2001;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i] - (double)sceneItem.SignUpEndSecs)
							{
								state = KingOfBattleGameStates.SignUp;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
							{
								state = KingOfBattleGameStates.Wait;
								result = 1;
							}
							else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
							{
								state = KingOfBattleGameStates.Start;
								result = 1;
							}
							else
							{
								state = KingOfBattleGameStates.None;
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
			KingOfBattleSceneInfo kingOfBattleSceneInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.SceneDataDict.TryGetValue(sceneId, out kingOfBattleSceneInfo))
				{
					goto IL_153;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < kingOfBattleSceneInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)kingOfBattleSceneInfo.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= kingOfBattleSceneInfo.SecondsOfDay[i] - (double)kingOfBattleSceneInfo.SignUpStartSecs && dateTime.TimeOfDay.TotalSeconds <= kingOfBattleSceneInfo.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(kingOfBattleSceneInfo.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_153:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		public bool ProcessGetKingOfBattleAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				string roleParamByName = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num2 = 0;
					int score = 0;
					int success = 0;
					string text = Global.GetRoleParamByName(client, "43");
					int num3 = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 4 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(roleParamByName, ref num2, ref success, ref score, ',');
					List<int> list2 = Global.StringToIntList(roleParamByName, ',');
					num2 = list2[0];
					bool flag = true;
					if (list2.Count >= 5 && num2 > 0)
					{
						success = list2[1];
						score = list2[2];
						int num4 = list2[3];
						int num5 = list2[4];
						if (list.Count >= 4)
						{
							num3 = Global.SafeConvertToInt32(list[0]);
							string text2 = list[1];
							int num6 = Global.SafeConvertToInt32(list[2]);
							int num7 = Global.SafeConvertToInt32(list[3]);
						}
						KingOfBattleSceneInfo sceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num2, out sceneInfo))
						{
							num = this.GiveRoleAwards(client, success, score, sceneInfo);
							if (num < 0)
							{
								flag = false;
							}
							if (client.ClientData.RoleID == num3)
							{
								GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KingOfBattleMVP, new int[0]));
							}
						}
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "43", "", true);
					}
					client.sendCmd<int>(nID, num, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool ProcessGetKingOfBattleAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				string roleParamByName = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num = 0;
					int num2 = 0;
					int success = 0;
					string text = Global.GetRoleParamByName(client, "43");
					int mvprid = 0;
					string mvpname = "";
					int mvpocc = 0;
					int mvpsex = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 4 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(roleParamByName, ref num, ref success, ref num2, ',');
					List<int> list2 = Global.StringToIntList(roleParamByName, ',');
					num = list2[0];
					bool flag = true;
					if (list2.Count >= 5 && num > 0)
					{
						success = list2[1];
						num2 = list2[2];
						int sideScore = list2[3];
						int sideScore2 = list2[4];
						if (list.Count >= 4)
						{
							mvprid = Global.SafeConvertToInt32(list[0]);
							mvpname = list[1];
							mvpocc = Global.SafeConvertToInt32(list[2]);
							mvpsex = Global.SafeConvertToInt32(list[3]);
						}
						KingOfBattleSceneInfo sceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out sceneInfo))
						{
							if (num2 >= this.RuntimeData.KingOfBattleLowestJiFen)
							{
								flag = false;
							}
							this.NtfCanGetAward(client, success, num2, sceneInfo, sideScore, sideScore2, mvprid, mvpname, mvpocc, mvpsex);
						}
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
						Global.SaveRoleParamsStringToDB(client, "43", "", true);
					}
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			client.sendCmd<int>(nID, 0, false);
			return false;
		}

		public bool ProcessGetKingOfBattleStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string roleParamByName = Global.GetRoleParamByName(client, "32");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					int num = 0;
					int num2 = 0;
					int num3 = 0;
					ConfigParser.ParseStrInt3(roleParamByName, ref num, ref num3, ref num2, ',');
					if (num > 0)
					{
						KingOfBattleSceneInfo kingOfBattleSceneInfo = null;
						if (this.RuntimeData.SceneDataDict.TryGetValue(num, out kingOfBattleSceneInfo))
						{
							client.sendCmd<int>(nID, 4, false);
							return true;
						}
					}
				}
				KingOfBattleSceneInfo kingOfBattleSceneInfo2 = null;
				KingOfBattleGameStates kingOfBattleGameStates = KingOfBattleGameStates.None;
				int cmdData = 0;
				int num4 = 0;
				this.RuntimeData.RoleId2JoinGroup.TryGetValue(client.ClientData.RoleID, out num4);
				this.CheckCondition(client, ref kingOfBattleSceneInfo2, ref kingOfBattleGameStates);
				if (num4 > 0)
				{
					if (kingOfBattleGameStates >= KingOfBattleGameStates.SignUp && kingOfBattleGameStates <= KingOfBattleGameStates.Wait)
					{
						int kuaFuRoleState = YongZheZhanChangClient.getInstance().GetKuaFuRoleState(client.ClientData.RoleID);
						if (kuaFuRoleState >= 1)
						{
							cmdData = 2;
						}
						else
						{
							cmdData = 5;
						}
					}
					else if (kingOfBattleGameStates == KingOfBattleGameStates.Start)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.ContainsKey(client.ClientData.RoleID))
						{
							cmdData = 3;
						}
					}
				}
				else if (kingOfBattleGameStates == KingOfBattleGameStates.SignUp)
				{
					cmdData = 1;
				}
				else if (kingOfBattleGameStates == KingOfBattleGameStates.Wait || kingOfBattleGameStates == KingOfBattleGameStates.Start)
				{
					cmdData = 5;
				}
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKingOfBattleEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (!this.IsGongNengOpened(client, false))
				{
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				KingOfBattleSceneInfo kingOfBattleSceneInfo = null;
				KingOfBattleGameStates kingOfBattleGameStates = KingOfBattleGameStates.None;
				if (!this.CheckMap(client))
				{
					num = -21;
				}
				else
				{
					num = this.CheckCondition(client, ref kingOfBattleSceneInfo, ref kingOfBattleGameStates);
				}
				if (kingOfBattleGameStates == KingOfBattleGameStates.Start)
				{
					KuaFuServerLoginData kuaFuServerLoginData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.RoleIdKuaFuLoginDataDict.TryGetValue(client.ClientData.RoleID, out kuaFuServerLoginData))
						{
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
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
							}
						}
						else
						{
							num = -11000;
						}
					}
					if (num >= 0)
					{
						num = YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 4, false);
						if (num >= 0)
						{
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
						else
						{
							Global.GetClientKuaFuServerLoginData(client).RoleId = 0;
						}
					}
				}
				else
				{
					num = -2001;
				}
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void RefreshKingOfBattleStoreData(KingOfBattleStoreData KOBattleStoreData, bool SetRefreshTm = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (SetRefreshTm)
				{
					KOBattleStoreData.LastRefTime = TimeUtil.NowDateTime();
				}
				KOBattleStoreData.SaleList.Clear();
				List<KingOfBattleStoreConfig> kingOfBattleStoreList = this.RuntimeData.KingOfBattleStoreList;
				int beginNum = kingOfBattleStoreList[0].BeginNum;
				int num = kingOfBattleStoreList[kingOfBattleStoreList.Count - 1].EndNum;
				for (int i = 0; i < this.RuntimeData.KingOfBattleStoreRefreshNum; i++)
				{
					int num2 = Global.GetRandomNumber(beginNum, num);
					for (int j = 0; j < kingOfBattleStoreList.Count; j++)
					{
						if (kingOfBattleStoreList[j].RandSkip)
						{
							num2 += kingOfBattleStoreList[j].RandNumMinus;
						}
						if (!kingOfBattleStoreList[j].RandSkip && num2 >= kingOfBattleStoreList[j].BeginNum && num2 <= kingOfBattleStoreList[j].EndNum)
						{
							kingOfBattleStoreList[j].RandSkip = true;
							num -= kingOfBattleStoreList[j].RandNumMinus;
							KingOfBattleStoreSaleData kingOfBattleStoreSaleData = new KingOfBattleStoreSaleData();
							kingOfBattleStoreSaleData.ID = kingOfBattleStoreList[j].ID;
							KOBattleStoreData.SaleList.Add(kingOfBattleStoreSaleData);
						}
					}
				}
				for (int j = 0; j < kingOfBattleStoreList.Count; j++)
				{
					kingOfBattleStoreList[j].RandSkip = false;
				}
			}
		}

		public KingOfBattleStoreData GetClientKingOfBattleStoreData(GameClient client)
		{
			KingOfBattleStoreData kobattleStoreData;
			if (null != client.ClientData.KOBattleStoreData)
			{
				kobattleStoreData = client.ClientData.KOBattleStoreData;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					client.ClientData.KOBattleStoreData = new KingOfBattleStoreData();
					client.ClientData.KOBattleStoreData.LastRefTime = Global.GetRoleParamsDateTimeFromDB(client, "10149");
					client.ClientData.KOBattleStoreData.SaleList = new List<KingOfBattleStoreSaleData>();
					List<ushort> roleParamsUshortListFromDB = Global.GetRoleParamsUshortListFromDB(client, "33");
					for (int i = 0; i < roleParamsUshortListFromDB.Count - 1; i += 2)
					{
						KingOfBattleStoreSaleData kingOfBattleStoreSaleData = new KingOfBattleStoreSaleData();
						kingOfBattleStoreSaleData.ID = (int)roleParamsUshortListFromDB[i];
						kingOfBattleStoreSaleData.Purchase = (int)roleParamsUshortListFromDB[i + 1];
						client.ClientData.KOBattleStoreData.SaleList.Add(kingOfBattleStoreSaleData);
					}
				}
				kobattleStoreData = client.ClientData.KOBattleStoreData;
			}
			return kobattleStoreData;
		}

		public void SaveKingOfBattleStoreData(GameClient client)
		{
			if (null != client.ClientData.KOBattleStoreData)
			{
				lock (this.RuntimeData.Mutex)
				{
					KingOfBattleStoreData kobattleStoreData = client.ClientData.KOBattleStoreData;
					Global.SaveRoleParamsDateTimeToDB(client, "10149", kobattleStoreData.LastRefTime, true);
					List<ushort> list = new List<ushort>();
					foreach (KingOfBattleStoreSaleData kingOfBattleStoreSaleData in kobattleStoreData.SaleList)
					{
						list.Add((ushort)kingOfBattleStoreSaleData.ID);
						list.Add((ushort)kingOfBattleStoreSaleData.Purchase);
					}
					Global.SaveRoleParamsUshortListToDB(client, list, "33", true);
				}
			}
		}

		public bool ProcessGetKingOfBattleMallDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				KingOfBattleStoreData clientKingOfBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				if ((TimeUtil.NowDateTime() - clientKingOfBattleStoreData.LastRefTime).TotalSeconds >= (double)(this.RuntimeData.KingOfBattleStoreRefreshTm * 3600))
				{
					this.RefreshKingOfBattleStoreData(clientKingOfBattleStoreData, true);
					this.SaveKingOfBattleStoreData(client);
				}
				client.sendCmd<KingOfBattleStoreData>(nID, clientKingOfBattleStoreData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKingOfBattleMallBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int num4 = Global.SafeConvertToInt32(cmdParams[2]);
				KingOfBattleStoreConfig kingOfBattleStoreConfig = null;
				string cmdData;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KingOfBattleStoreDict.TryGetValue(num3, out kingOfBattleStoreConfig))
					{
						num = 4;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				KingOfBattleStoreData clientKingOfBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				KingOfBattleStoreSaleData kingOfBattleStoreSaleData = null;
				foreach (KingOfBattleStoreSaleData kingOfBattleStoreSaleData2 in clientKingOfBattleStoreData.SaleList)
				{
					if (kingOfBattleStoreSaleData2.ID == num3)
					{
						kingOfBattleStoreSaleData = kingOfBattleStoreSaleData2;
						break;
					}
				}
				if (null == kingOfBattleStoreSaleData)
				{
					num = 3;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (kingOfBattleStoreConfig.SinglePurchase - kingOfBattleStoreSaleData.Purchase < num4)
				{
					num = 6;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (!Global.CanAddGoods(client, kingOfBattleStoreConfig.SaleData.GoodsID, kingOfBattleStoreConfig.SaleData.GCount * num4, kingOfBattleStoreConfig.SaleData.Binding, "1900-01-01 12:00:00", true, false))
				{
					num = 6;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int kingOfBattlePointValue = GameManager.ClientMgr.GetKingOfBattlePointValue(client);
				if (kingOfBattlePointValue < kingOfBattleStoreConfig.JiFen * num4)
				{
					num = 1;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				GameManager.ClientMgr.ModifyKingOfBattlePointValue(client, -kingOfBattleStoreConfig.JiFen * num4, "王者战场商店", true, true);
				GoodsData saleData = kingOfBattleStoreConfig.SaleData;
				Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, saleData.GoodsID, saleData.GCount * num4, saleData.Quality, saleData.Props, saleData.Forge_level, saleData.Binding, 0, saleData.Jewellist, true, 1, string.Format("王者战场商店", new object[0]), false, saleData.Endtime, saleData.AddPropIndex, saleData.BornIndex, saleData.Lucky, saleData.Strong, saleData.ExcellenceInfo, saleData.AppendPropLev, saleData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
				kingOfBattleStoreSaleData.Purchase += num4;
				this.SaveKingOfBattleStoreData(client);
				cmdData = string.Format("{0}:{1}:{2}", num, num3, kingOfBattleStoreSaleData.Purchase);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKingOfBattleMallRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpened(client, false))
				{
					return false;
				}
				int num = 0;
				string cmdData;
				if (client.ClientData.UserMoney < this.RuntimeData.KingOfBattleStoreRefreshCost)
				{
					num = 7;
					cmdData = string.Format("{0}", num);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.KingOfBattleStoreRefreshCost, "王者战场商店刷新", true, true, false, DaiBiSySType.None))
				{
					num = 7;
					cmdData = string.Format("{0}", num);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				KingOfBattleStoreData clientKingOfBattleStoreData = this.GetClientKingOfBattleStoreData(client);
				this.RefreshKingOfBattleStoreData(clientKingOfBattleStoreData, false);
				this.SaveKingOfBattleStoreData(client);
				client.sendCmd<KingOfBattleStoreData>(1190, clientKingOfBattleStoreData, false);
				cmdData = string.Format("{0}", num);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void OnStartPlayGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene kingOfBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					client.sendCmd<List<int>>(1189, kingOfBattleScene.SceneOpenTeleportList, false);
				}
			}
		}

		public bool OnInitGame(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out yongZheZhanChangFuBenData))
				{
					yongZheZhanChangFuBenData = null;
				}
				else if (yongZheZhanChangFuBenData.State >= 3)
				{
					return false;
				}
			}
			if (null == yongZheZhanChangFuBenData)
			{
				YongZheZhanChangFuBenData kuaFuFuBenData = YongZheZhanChangClient.getInstance().GetKuaFuFuBenData((int)clientKuaFuServerLoginData.GameId);
				if (kuaFuFuBenData == null || kuaFuFuBenData.State == 3)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + kuaFuFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue((int)clientKuaFuServerLoginData.GameId, out yongZheZhanChangFuBenData))
					{
						yongZheZhanChangFuBenData = kuaFuFuBenData;
						yongZheZhanChangFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[yongZheZhanChangFuBenData.GameId] = yongZheZhanChangFuBenData;
					}
				}
			}
			KuaFuFuBenRoleData kuaFuFuBenRoleData;
			bool result;
			if (!yongZheZhanChangFuBenData.RoleDict.TryGetValue(client.ClientData.RoleID, out kuaFuFuBenRoleData))
			{
				result = false;
			}
			else
			{
				client.ClientData.BattleWhichSide = kuaFuFuBenRoleData.Side;
				int posX;
				int posY;
				int birthPoint = this.GetBirthPoint(client, out posX, out posY);
				if (birthPoint <= 0)
				{
					LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + birthPoint, null, true);
					result = false;
				}
				else
				{
					lock (this.RuntimeData.Mutex)
					{
						clientKuaFuServerLoginData.FuBenSeqId = yongZheZhanChangFuBenData.SequenceId;
						KingOfBattleSceneInfo kingOfBattleSceneInfo;
						if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out kingOfBattleSceneInfo))
						{
							return false;
						}
						client.ClientData.MapCode = kingOfBattleSceneInfo.MapCode;
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

		public int GetBirthPoint(GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				YongZheZhanChangBirthPoint yongZheZhanChangBirthPoint = null;
				if (this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out yongZheZhanChangBirthPoint))
				{
					posX = yongZheZhanChangBirthPoint.PosX;
					posY = yongZheZhanChangBirthPoint.PosY;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(6) && !GameFuncControlManager.IsGameFuncDisabled(7) && GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("KingOfBattle") && GlobalNew.IsGongNengOpened(client, 78, hint);
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			BattleCrystalMonsterItem battleCrystalMonsterItem = (monster != null) ? (monster.Tag as BattleCrystalMonsterItem) : null;
			int result;
			if (battleCrystalMonsterItem == null)
			{
				result = -200;
			}
			else
			{
				result = battleCrystalMonsterItem.GatherTime;
			}
			return result;
		}

		private string BuildSceneBuffKey(GameClient client, int bufferGoodsID)
		{
			return string.Format("{0}_{1}", client.ClientData.RoleID, bufferGoodsID);
		}

		private void UpdateBuff4GameClient(GameClient client, int bufferGoodsID, object tagInfo, bool add)
		{
			try
			{
				BattleCrystalMonsterItem battleCrystalMonsterItem = tagInfo as BattleCrystalMonsterItem;
				KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = tagInfo as KingOfBattleDynamicMonsterItem;
				if (battleCrystalMonsterItem != null || null != kingOfBattleDynamicMonsterItem)
				{
					int num = 0;
					BufferItemTypes bufferItemTypes = BufferItemTypes.None;
					if (null != battleCrystalMonsterItem)
					{
						num = battleCrystalMonsterItem.BuffTime;
						bufferItemTypes = BufferItemTypes.KingOfBattleCrystal;
					}
					if (null != kingOfBattleDynamicMonsterItem)
					{
						num = kingOfBattleDynamicMonsterItem.BuffTime;
						bufferItemTypes = (BufferItemTypes)bufferGoodsID;
					}
					KingOfBattleScene kingOfBattleScene;
					if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
					{
						EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
						if (null != equipPropItem)
						{
							if (add)
							{
								double[] actionParams = new double[]
								{
									(double)num,
									(double)bufferGoodsID
								};
								Global.UpdateBufferData(client, bufferItemTypes, actionParams, 1, true);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									equipPropItem.ExtProps
								});
								string key = this.BuildSceneBuffKey(client, bufferGoodsID);
								kingOfBattleScene.SceneBuffDict[key] = new KingOfBattleSceneBuff
								{
									RoleID = client.ClientData.RoleID,
									BuffID = bufferGoodsID,
									EndTicks = TimeUtil.NOW() + (long)(num * 1000),
									tagInfo = tagInfo
								};
								if (bufferItemTypes == BufferItemTypes.KingOfBattleCrystal)
								{
									client.SceneContextData = tagInfo;
								}
							}
							else
							{
								Global.RemoveBufferData(client, (int)bufferItemTypes);
								client.ClientData.PropsCacheManager.SetExtProps(new object[]
								{
									PropsSystemTypes.BufferByGoodsProps,
									bufferGoodsID,
									PropsCacheManager.ConstExtProps
								});
								string key = this.BuildSceneBuffKey(client, bufferGoodsID);
								kingOfBattleScene.SceneBuffDict.Remove(key);
								if (bufferItemTypes == BufferItemTypes.KingOfBattleCrystal)
								{
									client.SceneContextData = null;
								}
							}
							GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void TryAddBossKillRandomBuff(GameClient client, KingOfBattleDynamicMonsterItem tagInfo)
		{
			int bufferGoodsID = -1;
			if (tagInfo.RandomBuffList.Count != 0)
			{
				double num = 0.0;
				double num2 = (double)Global.GetRandomNumber(1, 101) / 100.0;
				for (int i = 0; i < tagInfo.RandomBuffList.Count; i++)
				{
					num += tagInfo.RandomBuffList[i].Pct;
					if (num2 <= num)
					{
						bufferGoodsID = tagInfo.RandomBuffList[i].GoodsID;
						break;
					}
				}
				this.UpdateBuff4GameClient(client, bufferGoodsID, tagInfo, true);
			}
		}

		public void InstallJunQi(KingOfBattleScene scene, GameClient client, KingOfBattleQiZhiConfig item)
		{
			CopyMap copyMap = scene.CopyMap;
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.m_nMapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				int monsterID = 0;
				if (client.ClientData.BattleWhichSide == 1)
				{
					monsterID = this.RuntimeData.BattleQiZhiMonsterID1;
				}
				else if (client.ClientData.BattleWhichSide == 2)
				{
					monsterID = this.RuntimeData.BattleQiZhiMonsterID2;
				}
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, 39, item, null);
			}
		}

		public void CalculateTeleportGateState(KingOfBattleScene scene)
		{
			int num = -1;
			foreach (KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig in scene.NPCID2QiZhiConfigDict.Values)
			{
				if (num == -1 && kingOfBattleQiZhiConfig.Alive)
				{
					num = kingOfBattleQiZhiConfig.BattleWhichSide;
				}
				if (!kingOfBattleQiZhiConfig.Alive || kingOfBattleQiZhiConfig.BattleWhichSide != num)
				{
					num = -1;
					break;
				}
			}
			scene.SceneOpenTeleportList.Clear();
			if (-1 != num)
			{
				scene.SceneOpenTeleportList.Add(num + 10);
			}
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<int>>(1189, scene.SceneOpenTeleportList, scene.CopyMap);
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig = null;
			bool flag = false;
			bool flag2 = false;
			KingOfBattleScene kingOfBattleScene = client.SceneObject as KingOfBattleScene;
			bool result;
			if (null == kingOfBattleScene)
			{
				result = flag;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (kingOfBattleScene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out kingOfBattleQiZhiConfig))
					{
						flag = true;
						if (kingOfBattleQiZhiConfig.Alive)
						{
							return flag;
						}
						if (client.ClientData.BattleWhichSide != kingOfBattleQiZhiConfig.BattleWhichSide && Math.Abs(TimeUtil.NOW() - kingOfBattleQiZhiConfig.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Math.Abs(client.ClientData.PosX - kingOfBattleQiZhiConfig.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - kingOfBattleQiZhiConfig.PosY) <= 1000)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						this.InstallJunQi(kingOfBattleScene, client, kingOfBattleQiZhiConfig);
						this.CalculateTeleportGateState(kingOfBattleScene);
					}
				}
				result = flag;
			}
			return result;
		}

		private void InitScene(KingOfBattleScene scene, GameClient client)
		{
			foreach (KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(kingOfBattleQiZhiConfig.NPCID, kingOfBattleQiZhiConfig.Clone() as KingOfBattleQiZhiConfig);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 39)
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
					int num = (int)Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						KingOfBattleScene kingOfBattleScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out kingOfBattleScene))
						{
							KingOfBattleSceneInfo kingOfBattleSceneInfo = null;
							YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(num, out yongZheZhanChangFuBenData))
							{
								LogManager.WriteLog(2, "王者战场没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
							}
							if (!this.RuntimeData.SceneDataDict.TryGetValue(yongZheZhanChangFuBenData.GroupIndex, out kingOfBattleSceneInfo))
							{
								LogManager.WriteLog(2, "王者战场没有为副本找到对应的档位数据,ID:" + yongZheZhanChangFuBenData.GroupIndex, null, true);
							}
							kingOfBattleScene = new KingOfBattleScene();
							kingOfBattleScene.CopyMap = copyMap;
							kingOfBattleScene.CleanAllInfo();
							kingOfBattleScene.GameId = num;
							kingOfBattleScene.m_nMapCode = mapCode;
							kingOfBattleScene.CopyMapId = copyMap.CopyMapID;
							kingOfBattleScene.FuBenSeqId = fuBenSeqID;
							kingOfBattleScene.m_nPlarerCount = 1;
							kingOfBattleScene.SceneInfo = kingOfBattleSceneInfo;
							kingOfBattleScene.MapGridWidth = gameMap.MapGridWidth;
							kingOfBattleScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(kingOfBattleSceneInfo.Id));
							kingOfBattleScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(kingOfBattleScene, client);
							kingOfBattleScene.GameStatisticalData.GameId = num;
							this.SceneDict[fuBenSeqID] = kingOfBattleScene;
						}
						else
						{
							kingOfBattleScene.m_nPlarerCount++;
						}
						KingOfBattleClientContextData kingOfBattleClientContextData;
						if (!kingOfBattleScene.ClientContextDataDict.TryGetValue(roleID, out kingOfBattleClientContextData))
						{
							kingOfBattleClientContextData = new KingOfBattleClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							kingOfBattleScene.ClientContextDataDict[roleID] = kingOfBattleClientContextData;
						}
						else
						{
							kingOfBattleClientContextData.KillNum = 0;
						}
						client.SceneObject = kingOfBattleScene;
						client.SceneGameId = (long)kingOfBattleScene.GameId;
						client.SceneContextData2 = kingOfBattleClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(kingOfBattleScene.SceneInfo.TotalSecs * 1000));
					}
					YongZheZhanChangClient.getInstance().GameFuBenRoleChangeState(roleID, 5, 0, 0);
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
			if (sceneType == 39)
			{
				lock (this.RuntimeData.Mutex)
				{
					KingOfBattleScene kingOfBattleScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out kingOfBattleScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene kingOfBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					if (kingOfBattleScene.m_eStatus == 2)
					{
						BattleCrystalMonsterItem battleCrystalMonsterItem = monster.Tag as BattleCrystalMonsterItem;
						if (battleCrystalMonsterItem != null)
						{
							BattleCrystalMonsterItem battleCrystalMonsterItem2 = client.SceneContextData as BattleCrystalMonsterItem;
							if (null != battleCrystalMonsterItem2)
							{
								this.AddDelayCreateMonster(kingOfBattleScene, TimeUtil.NOW() + (long)battleCrystalMonsterItem2.FuHuoTime, battleCrystalMonsterItem2);
							}
							this.UpdateBuff4GameClient(client, battleCrystalMonsterItem.BuffGoodsID, battleCrystalMonsterItem, true);
						}
					}
				}
			}
		}

		public bool ClientChangeMap(GameClient client, int teleportID, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			KingOfBattleScene kingOfBattleScene = client.SceneObject as KingOfBattleScene;
			bool result;
			if (null == kingOfBattleScene)
			{
				result = false;
			}
			else
			{
				int num = teleportID % 10;
				result = (client.ClientData.BattleWhichSide == num && kingOfBattleScene.SceneOpenTeleportList.Contains(teleportID));
			}
			return result;
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (client != null && (monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID1 || monster.MonsterInfo.ExtensionID == this.RuntimeData.BattleQiZhiMonsterID2))
			{
				KingOfBattleScene kingOfBattleScene = client.SceneObject as KingOfBattleScene;
				KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig = monster.Tag as KingOfBattleQiZhiConfig;
				if (kingOfBattleScene != null && null != kingOfBattleQiZhiConfig)
				{
					lock (this.RuntimeData.Mutex)
					{
						kingOfBattleQiZhiConfig.DeadTicks = TimeUtil.NOW();
						kingOfBattleQiZhiConfig.Alive = false;
						kingOfBattleQiZhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
						this.CalculateTeleportGateState(kingOfBattleScene);
					}
				}
			}
			KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = monster.Tag as KingOfBattleDynamicMonsterItem;
			if (kingOfBattleDynamicMonsterItem != null && (kingOfBattleDynamicMonsterItem.MonsterType == 3 || kingOfBattleDynamicMonsterItem.MonsterType == 2))
			{
				KingOfBattleScene kingOfBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					CopyMap copyMap = kingOfBattleScene.CopyMap;
					string msg = string.Format(GLang.GetLang(397, new object[0]), Global.FormatRoleName4(client));
					if (client.ClientData.BattleWhichSide == 1 && !kingOfBattleScene.GuangMuNotify1)
					{
						kingOfBattleScene.GuangMuNotify1 = true;
						GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, client.ClientData.BattleWhichSide, 0);
						GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
					}
					else if (client.ClientData.BattleWhichSide == 2 && !kingOfBattleScene.GuangMuNotify2)
					{
						kingOfBattleScene.GuangMuNotify2 = true;
						GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, client.ClientData.BattleWhichSide, 0);
						GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
					}
					msg = string.Format(GLang.GetLang(398, new object[0]), Global.FormatRoleName4(client));
					GameManager.ClientMgr.BroadSpecialCopyMapMsg(copyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
				}
			}
			if (kingOfBattleDynamicMonsterItem != null && kingOfBattleDynamicMonsterItem.MonsterType == 1)
			{
				KingOfBattleScene kingOfBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					this.ProcessEnd(kingOfBattleScene, client.ClientData.BattleWhichSide, TimeUtil.NOW());
				}
			}
			if (kingOfBattleDynamicMonsterItem != null && kingOfBattleDynamicMonsterItem.MonsterType == 4)
			{
				KingOfBattleScene kingOfBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					string msg = string.Format(GLang.GetLang(399, new object[0]), Global.FormatRoleName4(client));
					GameManager.ClientMgr.BroadSpecialCopyMapMsg(kingOfBattleScene.CopyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
				}
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (monster.MonsterType == 401 || monster.MonsterType == 1301 || monster.MonsterType == 1302 || monster.MonsterType == 1303 || monster.MonsterType == 2000 || monster.MonsterType == 2001)
			{
				KingOfBattleClientContextData kingOfBattleClientContextData = client.SceneContextData2 as KingOfBattleClientContextData;
				if (null != kingOfBattleClientContextData)
				{
					KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = monster.Tag as KingOfBattleDynamicMonsterItem;
					if (null != kingOfBattleDynamicMonsterItem)
					{
						KingOfBattleScene kingOfBattleScene = null;
						int num = 0;
						if (monster.HandledDead && monster.WhoKillMeID == client.ClientData.RoleID)
						{
							num += kingOfBattleDynamicMonsterItem.JiFenKill;
						}
						double num2 = this.RuntimeData.KingBattleBossAttackPercent * monster.MonsterInfo.VLifeMax;
						lock (this.RuntimeData.Mutex)
						{
							if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
							{
								if (kingOfBattleScene.m_eStatus != 2)
								{
									return;
								}
								double num3 = 0.0;
								kingOfBattleClientContextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out num3);
								num3 += (double)injure;
								if (num3 >= num2 && num2 > 0.0)
								{
									int num4 = (int)(num3 / num2);
									num3 -= num2 * (double)num4;
									num += kingOfBattleDynamicMonsterItem.JiFenDamage * num4;
								}
								kingOfBattleClientContextData.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = num3;
								kingOfBattleClientContextData.TotalScore += num;
								if (monster.HandledDead)
								{
									KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem2 = null;
									if (kingOfBattleDynamicMonsterItem.RebornID != -1 && this.RuntimeData.DynMonsterDict.TryGetValue(kingOfBattleDynamicMonsterItem.RebornID, out kingOfBattleDynamicMonsterItem2))
									{
										long num5 = TimeUtil.NOW();
										this.AddDelayCreateMonster(kingOfBattleScene, num5 + (long)kingOfBattleDynamicMonsterItem2.DelayBirthMs, kingOfBattleDynamicMonsterItem2);
									}
									this.TryAddBossKillRandomBuff(client, kingOfBattleDynamicMonsterItem);
								}
								if (client.ClientData.BattleWhichSide == 1)
								{
									kingOfBattleScene.ScoreData.Score1 += num;
								}
								else if (client.ClientData.BattleWhichSide == 2)
								{
									kingOfBattleScene.ScoreData.Score2 += num;
								}
								kingOfBattleScene.GameStatisticalData.BossScore += num;
							}
						}
						if (num > 0 && kingOfBattleScene != null)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, kingOfBattleScene.ScoreData, kingOfBattleScene.CopyMap);
							this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						}
					}
				}
			}
		}

		private void ProcessEnd(KingOfBattleScene scene, int successSide, long nowTicks)
		{
			if (successSide != 0)
			{
				List<KingOfBattleClientContextData> list = new List<KingOfBattleClientContextData>();
				foreach (KingOfBattleClientContextData kingOfBattleClientContextData in scene.ClientContextDataDict.Values)
				{
					if (kingOfBattleClientContextData.BattleWhichSide == successSide)
					{
						list.Add(kingOfBattleClientContextData);
					}
				}
				list.Sort(delegate(KingOfBattleClientContextData left, KingOfBattleClientContextData right)
				{
					int result;
					if (left.TotalScore > right.TotalScore)
					{
						result = -1;
					}
					else if (left.TotalScore < right.TotalScore)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				if (list.Count != 0)
				{
					scene.ClientContextMVP = list[0];
				}
			}
			this.CompleteScene(scene, successSide);
			scene.m_eStatus = 3;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 15;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= KingOfBattleManager.NextHeartBeatTicks)
			{
				KingOfBattleManager.NextHeartBeatTicks = num + 1020L;
				foreach (KingOfBattleScene kingOfBattleScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = kingOfBattleScene.FuBenSeqId;
						int copyMapId = kingOfBattleScene.CopyMapId;
						int nMapCode = kingOfBattleScene.m_nMapCode;
						if (fuBenSeqId >= 0 && copyMapId >= 0 && nMapCode >= 0)
						{
							CopyMap copyMap = kingOfBattleScene.CopyMap;
							DateTime time = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (kingOfBattleScene.m_eStatus == 1 || kingOfBattleScene.m_eStatus == 2)
							{
								this.CheckCreateDynamicMonster(kingOfBattleScene, num2);
							}
							if (kingOfBattleScene.m_eStatus == 0)
							{
								if (num2 >= kingOfBattleScene.StartTimeTicks)
								{
									kingOfBattleScene.m_lPrepareTime = kingOfBattleScene.StartTimeTicks;
									kingOfBattleScene.m_lBeginTime = kingOfBattleScene.m_lPrepareTime + (long)(kingOfBattleScene.SceneInfo.PrepareSecs * 1000);
									kingOfBattleScene.m_eStatus = 1;
									kingOfBattleScene.StateTimeData.GameType = 15;
									kingOfBattleScene.StateTimeData.State = kingOfBattleScene.m_eStatus;
									kingOfBattleScene.StateTimeData.EndTicks = kingOfBattleScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kingOfBattleScene.StateTimeData, kingOfBattleScene.CopyMap);
									this.InitCreateDynamicMonster(kingOfBattleScene);
								}
							}
							else if (kingOfBattleScene.m_eStatus == 1)
							{
								if (num2 >= kingOfBattleScene.m_lBeginTime)
								{
									kingOfBattleScene.m_eStatus = 2;
									kingOfBattleScene.m_lEndTime = kingOfBattleScene.m_lBeginTime + (long)(kingOfBattleScene.SceneInfo.FightingSecs * 1000);
									kingOfBattleScene.StateTimeData.GameType = 15;
									kingOfBattleScene.StateTimeData.State = kingOfBattleScene.m_eStatus;
									kingOfBattleScene.StateTimeData.EndTicks = kingOfBattleScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kingOfBattleScene.StateTimeData, kingOfBattleScene.CopyMap);
									for (int i = 3; i <= 8; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (kingOfBattleScene.m_eStatus == 2)
							{
								if (num2 >= kingOfBattleScene.m_lEndTime)
								{
									int successSide = 0;
									if (kingOfBattleScene.ScoreData.Score1 > kingOfBattleScene.ScoreData.Score2)
									{
										successSide = 1;
									}
									else if (kingOfBattleScene.ScoreData.Score2 > kingOfBattleScene.ScoreData.Score1)
									{
										successSide = 2;
									}
									this.ProcessEnd(kingOfBattleScene, successSide, num2);
								}
								else
								{
									this.CheckSceneBufferTime(kingOfBattleScene, num2);
								}
							}
							else if (kingOfBattleScene.m_eStatus == 3)
							{
								GameManager.CopyMapMgr.KillAllMonster(kingOfBattleScene.CopyMap);
								kingOfBattleScene.m_eStatus = 4;
								YongZheZhanChangClient.getInstance().GameFuBenChangeState(kingOfBattleScene.GameId, 3, time);
								this.GiveAwards(kingOfBattleScene);
								YongZheZhanChangFuBenData yongZheZhanChangFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue(kingOfBattleScene.GameId, out yongZheZhanChangFuBenData))
								{
									yongZheZhanChangFuBenData.State = 3;
									LogManager.WriteLog(2, string.Format("王者战场跨服副本GameID={0},战斗结束", yongZheZhanChangFuBenData.GameId), null, true);
								}
							}
							else if (kingOfBattleScene.m_eStatus == 4)
							{
								if (num2 >= kingOfBattleScene.m_lLeaveTime)
								{
									copyMap.SetRemoveTicks(kingOfBattleScene.m_lLeaveTime);
									kingOfBattleScene.m_eStatus = 5;
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
										DataHelper.WriteExceptionLogEx(ex, "王者战场系统清场调度异常");
									}
								}
							}
						}
					}
				}
			}
		}

		private void AddDelayCreateMonster(KingOfBattleScene scene, long ticks, object monster)
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

		private void InitCreateDynamicMonster(KingOfBattleScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (KingOfBattleQiZhiConfig kingOfBattleQiZhiConfig in scene.NPCID2QiZhiConfigDict.Values)
				{
					kingOfBattleQiZhiConfig.Alive = true;
					if (kingOfBattleQiZhiConfig.QiZhiMonsterID == this.RuntimeData.BattleQiZhiMonsterID1)
					{
						kingOfBattleQiZhiConfig.BattleWhichSide = 1;
					}
					else if (kingOfBattleQiZhiConfig.QiZhiMonsterID == this.RuntimeData.BattleQiZhiMonsterID2)
					{
						kingOfBattleQiZhiConfig.BattleWhichSide = 2;
					}
					GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, kingOfBattleQiZhiConfig.QiZhiMonsterID, scene.CopyMapId, 1, kingOfBattleQiZhiConfig.PosX / scene.MapGridWidth, kingOfBattleQiZhiConfig.PosY / scene.MapGridHeight, 0, 0, 39, kingOfBattleQiZhiConfig, null);
				}
				foreach (BattleCrystalMonsterItem monster in this.RuntimeData.BattleCrystalMonsterDict.Values)
				{
					this.AddDelayCreateMonster(scene, scene.m_lPrepareTime, monster);
				}
				List<KingOfBattleDynamicMonsterItem> list = null;
				if (this.RuntimeData.SceneDynMonsterDict.TryGetValue(scene.m_nMapCode, out list))
				{
					foreach (KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem in list)
					{
						if (!kingOfBattleDynamicMonsterItem.RebornBirth)
						{
							this.AddDelayCreateMonster(scene, scene.m_lPrepareTime + (long)kingOfBattleDynamicMonsterItem.DelayBirthMs, kingOfBattleDynamicMonsterItem);
						}
					}
				}
			}
		}

		public void CheckCreateDynamicMonster(KingOfBattleScene scene, long nowMs)
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
							if (obj is KingOfBattleDynamicMonsterItem)
							{
								KingOfBattleDynamicMonsterItem kingOfBattleDynamicMonsterItem = obj as KingOfBattleDynamicMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, kingOfBattleDynamicMonsterItem.MonsterID, scene.CopyMapId, 1, kingOfBattleDynamicMonsterItem.PosX / scene.MapGridWidth, kingOfBattleDynamicMonsterItem.PosY / scene.MapGridHeight, 0, kingOfBattleDynamicMonsterItem.PursuitRadius, 39, kingOfBattleDynamicMonsterItem, null);
								if (kingOfBattleDynamicMonsterItem.MonsterType == 4)
								{
									string msg = string.Format(GLang.GetLang(400, new object[0]), Global.GetMonsterNameByID(kingOfBattleDynamicMonsterItem.MonsterID));
									GameManager.ClientMgr.BroadSpecialCopyMapMsg(scene.CopyMap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
								}
							}
							else if (obj is BattleCrystalMonsterItem)
							{
								BattleCrystalMonsterItem battleCrystalMonsterItem = obj as BattleCrystalMonsterItem;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, battleCrystalMonsterItem.MonsterID, scene.CopyMap.CopyMapID, 1, battleCrystalMonsterItem.PosX / scene.MapGridWidth, battleCrystalMonsterItem.PosY / scene.MapGridHeight, 0, 0, 39, battleCrystalMonsterItem, null);
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

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene kingOfBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, kingOfBattleScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<KingOfBattleScoreData>(1184, kingOfBattleScene.ScoreData, false);
					}
					if (selfScore)
					{
						KingOfBattleClientContextData kingOfBattleClientContextData = client.SceneContextData2 as KingOfBattleClientContextData;
						if (null != kingOfBattleClientContextData)
						{
							client.sendCmd<int>(1185, kingOfBattleClientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		public void CompleteScene(KingOfBattleScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		public void RemoveBattleSceneBuffForRole(KingOfBattleScene scene, GameClient client)
		{
			List<KingOfBattleSceneBuff> list = new List<KingOfBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (scene.SceneBuffDict.Count != 0)
				{
					foreach (KingOfBattleSceneBuff kingOfBattleSceneBuff in scene.SceneBuffDict.Values)
					{
						if (kingOfBattleSceneBuff.RoleID == client.ClientData.RoleID)
						{
							list.Add(kingOfBattleSceneBuff);
						}
					}
					if (list.Count != 0)
					{
						foreach (KingOfBattleSceneBuff kingOfBattleSceneBuff in list)
						{
							if (kingOfBattleSceneBuff.RoleID != 0)
							{
								this.UpdateBuff4GameClient(client, kingOfBattleSceneBuff.BuffID, kingOfBattleSceneBuff.tagInfo, false);
							}
							BattleCrystalMonsterItem battleCrystalMonsterItem = kingOfBattleSceneBuff.tagInfo as BattleCrystalMonsterItem;
							if (null != battleCrystalMonsterItem)
							{
								this.AddDelayCreateMonster(scene, TimeUtil.NOW() + (long)battleCrystalMonsterItem.FuHuoTime, kingOfBattleSceneBuff.tagInfo);
							}
						}
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene kingOfBattleScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					if (kingOfBattleScene.m_eStatus == 2)
					{
						int num = 0;
						int kingOfBattleDie = this.RuntimeData.KingOfBattleDie;
						KingOfBattleClientContextData kingOfBattleClientContextData = client.SceneContextData2 as KingOfBattleClientContextData;
						KingOfBattleClientContextData kingOfBattleClientContextData2 = other.SceneContextData2 as KingOfBattleClientContextData;
						HuanYingSiYuanLianSha huanYingSiYuanLianSha = null;
						HuanYingSiYuanLianshaOver huanYingSiYuanLianshaOver = null;
						HuanYingSiYuanAddScore huanYingSiYuanAddScore = new HuanYingSiYuanAddScore();
						huanYingSiYuanAddScore.Name = Global.FormatRoleName4(client);
						huanYingSiYuanAddScore.ZoneID = client.ClientData.ZoneID;
						huanYingSiYuanAddScore.Side = client.ClientData.BattleWhichSide;
						huanYingSiYuanAddScore.ByLianShaNum = 1;
						huanYingSiYuanAddScore.RoleId = client.ClientData.RoleID;
						huanYingSiYuanAddScore.Occupation = client.ClientData.Occupation;
						kingOfBattleScene.GameStatisticalData.KillScore += this.RuntimeData.KingOfBattleUltraKillParam1;
						if (null != kingOfBattleClientContextData)
						{
							kingOfBattleClientContextData.KillNum++;
							int num2 = this.RuntimeData.KingOfBattleUltraKillParam1 + kingOfBattleClientContextData.KillNum * this.RuntimeData.KingOfBattleUltraKillParam2;
							num2 = Math.Min(this.RuntimeData.KingOfBattleUltraKillParam4, Math.Max(this.RuntimeData.KingOfBattleUltraKillParam3, num2));
							huanYingSiYuanAddScore.ByLianShaNum = 1;
							huanYingSiYuanLianSha = new HuanYingSiYuanLianSha();
							huanYingSiYuanLianSha.Name = huanYingSiYuanAddScore.Name;
							huanYingSiYuanLianSha.ZoneID = huanYingSiYuanAddScore.ZoneID;
							huanYingSiYuanLianSha.Occupation = huanYingSiYuanAddScore.Occupation;
							huanYingSiYuanLianSha.LianShaType = Math.Min(kingOfBattleClientContextData.KillNum, 30) / 5;
							huanYingSiYuanLianSha.ExtScore = num2;
							huanYingSiYuanLianSha.Side = huanYingSiYuanAddScore.Side;
							num += num2;
							kingOfBattleScene.GameStatisticalData.LianShaScore += num2;
							if (kingOfBattleClientContextData.KillNum % 5 != 0)
							{
								huanYingSiYuanLianSha = null;
							}
						}
						if (null != kingOfBattleClientContextData2)
						{
							int num3 = this.RuntimeData.KingOfBattleShutDownParam1 + kingOfBattleClientContextData2.KillNum * this.RuntimeData.KingOfBattleShutDownParam2;
							num3 = Math.Min(this.RuntimeData.KingOfBattleShutDownParam4, Math.Max(this.RuntimeData.KingOfBattleShutDownParam3, num3));
							num += num3;
							kingOfBattleScene.GameStatisticalData.ZhongJieScore += num3;
							if (kingOfBattleClientContextData2.KillNum >= 10)
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
							kingOfBattleClientContextData2.KillNum = 0;
							kingOfBattleClientContextData2.TotalScore += kingOfBattleDie;
							kingOfBattleScene.GameStatisticalData.KillScore += kingOfBattleDie;
						}
						huanYingSiYuanAddScore.Score = num;
						if (client.ClientData.BattleWhichSide == 1)
						{
							kingOfBattleScene.ScoreData.Score1 += num;
							kingOfBattleScene.ScoreData.Score2 += kingOfBattleDie;
						}
						else
						{
							kingOfBattleScene.ScoreData.Score2 += num;
							kingOfBattleScene.ScoreData.Score1 += kingOfBattleDie;
						}
						if (null != kingOfBattleClientContextData)
						{
							kingOfBattleClientContextData.TotalScore += num;
						}
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, kingOfBattleScene.ScoreData, kingOfBattleScene.CopyMap);
						if (null != huanYingSiYuanLianSha)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianSha>(1186, huanYingSiYuanLianSha, kingOfBattleScene.CopyMap);
						}
						if (null != huanYingSiYuanLianshaOver)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<HuanYingSiYuanLianshaOver>(1187, huanYingSiYuanLianshaOver, kingOfBattleScene.CopyMap);
						}
						this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
						this.NotifyTimeStateInfoAndScoreInfo(other, false, false, true);
					}
				}
			}
		}

		public void SubmitCrystalBuff(GameClient client, int areaLuaID)
		{
			if (areaLuaID == client.ClientData.BattleWhichSide)
			{
				BattleCrystalMonsterItem battleCrystalMonsterItem = client.SceneContextData as BattleCrystalMonsterItem;
				if (null != battleCrystalMonsterItem)
				{
					lock (this.RuntimeData.Mutex)
					{
						KingOfBattleScene kingOfBattleScene;
						if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
						{
							KingOfBattleClientContextData kingOfBattleClientContextData = client.SceneContextData2 as KingOfBattleClientContextData;
							if (kingOfBattleClientContextData != null && kingOfBattleScene.m_eStatus == 2)
							{
								int battleJiFen = battleCrystalMonsterItem.BattleJiFen;
								kingOfBattleClientContextData.TotalScore += battleJiFen;
								kingOfBattleScene.GameStatisticalData.CaiJiScore += battleJiFen;
								if (client.ClientData.BattleWhichSide == 1)
								{
									kingOfBattleScene.ScoreData.Score1 += battleJiFen;
								}
								else if (client.ClientData.BattleWhichSide == 2)
								{
									kingOfBattleScene.ScoreData.Score2 += battleJiFen;
								}
								if (battleJiFen > 0)
								{
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<KingOfBattleScoreData>(1184, kingOfBattleScene.ScoreData, kingOfBattleScene.CopyMap);
									this.NotifyTimeStateInfoAndScoreInfo(client, false, false, true);
								}
							}
							this.UpdateBuff4GameClient(client, battleCrystalMonsterItem.BuffGoodsID, battleCrystalMonsterItem, false);
							this.AddDelayCreateMonster(kingOfBattleScene, TimeUtil.NOW() + (long)battleCrystalMonsterItem.FuHuoTime, battleCrystalMonsterItem);
						}
					}
				}
			}
		}

		public void GiveAwards(KingOfBattleScene scene)
		{
			try
			{
				Dictionary<int, int[]> dictionary = new Dictionary<int, int[]>();
				KingOfBattleStatisticalData gameStatisticalData = scene.GameStatisticalData;
				foreach (KingOfBattleClientContextData kingOfBattleClientContextData in scene.ClientContextDataDict.Values)
				{
					gameStatisticalData.AllRoleCount++;
					int num;
					if (kingOfBattleClientContextData.BattleWhichSide == scene.SuccessSide)
					{
						num = 1;
						gameStatisticalData.WinRoleCount++;
					}
					else
					{
						num = 0;
						gameStatisticalData.LoseRoleCount++;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(kingOfBattleClientContextData.RoleId);
					string text = string.Format("{0},{1},{2},{3},{4}", new object[]
					{
						scene.SceneInfo.Id,
						num,
						kingOfBattleClientContextData.TotalScore,
						scene.ScoreData.Score1,
						scene.ScoreData.Score2
					});
					string text2 = string.Format("{0}&{1}&{2}&{3}", new object[]
					{
						scene.ClientContextMVP.RoleId,
						scene.ClientContextMVP.RoleName,
						scene.ClientContextMVP.Occupation,
						scene.ClientContextMVP.RoleSex
					});
					byte[] bytes = new UTF8Encoding().GetBytes(text2);
					text2 = Convert.ToBase64String(bytes);
					if (gameClient != null)
					{
						int faction = gameClient.ClientData.Faction;
						int junTuanId = gameClient.ClientData.JunTuanId;
						if (faction > 0 && junTuanId > 0)
						{
							int[] array;
							if (!dictionary.TryGetValue(faction, out array))
							{
								int[] array2 = new int[2];
								array2[0] = junTuanId;
								array = array2;
								dictionary[faction] = array;
							}
							array[1]++;
						}
						int totalScore = kingOfBattleClientContextData.TotalScore;
						kingOfBattleClientContextData.TotalScore = 0;
						if (totalScore >= this.RuntimeData.KingOfBattleLowestJiFen)
						{
							Global.SaveRoleParamsStringToDB(gameClient, "32", text, true);
							Global.SaveRoleParamsStringToDB(gameClient, "43", text2, true);
						}
						else
						{
							Global.SaveRoleParamsStringToDB(gameClient, "32", this.RuntimeData.RoleParamsAwardsDefaultString, true);
							Global.SaveRoleParamsStringToDB(gameClient, "43", "", true);
						}
						this.NtfCanGetAward(gameClient, num, totalScore, scene.SceneInfo, scene.ScoreData.Score1, scene.ScoreData.Score2, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex);
					}
					else if (kingOfBattleClientContextData.TotalScore >= this.RuntimeData.KingOfBattleLowestJiFen)
					{
						Global.UpdateRoleParamByNameOffline(kingOfBattleClientContextData.RoleId, "32", text, kingOfBattleClientContextData.ServerId);
						Global.UpdateRoleParamByNameOffline(kingOfBattleClientContextData.RoleId, "43", text2, kingOfBattleClientContextData.ServerId);
					}
					else
					{
						Global.UpdateRoleParamByNameOffline(kingOfBattleClientContextData.RoleId, "32", this.RuntimeData.RoleParamsAwardsDefaultString, kingOfBattleClientContextData.ServerId);
						Global.UpdateRoleParamByNameOffline(kingOfBattleClientContextData.RoleId, "43", "", kingOfBattleClientContextData.ServerId);
					}
				}
				foreach (KeyValuePair<int, int[]> keyValuePair in dictionary)
				{
					JunTuanManager.getInstance().AddJunTuanTaskValue(keyValuePair.Key, keyValuePair.Value[0], Global.GetMapSceneType(scene.m_nMapCode), keyValuePair.Value[1]);
				}
				YongZheZhanChangClient.getInstance().PushGameResultData(gameStatisticalData);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "王者战场系统清场调度异常");
			}
		}

		private void NtfCanGetAward(GameClient client, int success, int score, KingOfBattleSceneInfo sceneInfo, int sideScore1, int sideScore2, int mvprid, string mvpname, int mvpocc, int mvpsex)
		{
			long num = 0L;
			int num2 = 0;
			List<AwardsItemData> awardsItemDataList = null;
			if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
			{
				num = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				num2 = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					awardsItemDataList = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					num = (long)((double)num * 0.8);
					num2 = (int)((double)num2 * 0.8);
					awardsItemDataList = sceneInfo.LoseAwardsItemList.Items;
				}
				num -= num % 10000L;
				num2 -= num2 % 10000;
			}
			client.sendCmd<KingOfBattleAwardsData>(1182, new KingOfBattleAwardsData
			{
				Exp = num,
				BindJinBi = num2,
				Success = success,
				AwardsItemDataList = awardsItemDataList,
				SideScore1 = sideScore1,
				SideScore2 = sideScore2,
				SelfScore = score,
				MvpRoleName = mvpname,
				MvpOccupation = mvpocc,
				MvpRoleSex = mvpsex
			}, false);
		}

		private int GiveRoleAwards(GameClient client, int success, int score, KingOfBattleSceneInfo sceneInfo)
		{
			long num = 0L;
			int num2 = 0;
			List<AwardsItemData> list = null;
			if (score >= this.RuntimeData.KingOfBattleLowestJiFen)
			{
				num = (long)((double)sceneInfo.Exp * (0.2 + Math.Min(0.8, Math.Pow((double)score, 0.5) / 100.0)));
				num2 = (int)((double)sceneInfo.BandJinBi * Math.Min(100.0, Math.Pow((double)score, 0.5)));
				if (success > 0)
				{
					list = sceneInfo.WinAwardsItemList.Items;
				}
				else
				{
					num = (long)((double)num * 0.8);
					num2 = (int)((double)num2 * 0.8);
					list = sceneInfo.LoseAwardsItemList.Items;
				}
				num -= num % 10000L;
				num2 -= num2 % 10000;
			}
			int result;
			if (list != null && !Global.CanAddGoodsNum(client, list.Count))
			{
				result = -100;
			}
			else
			{
				if (num > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, num, true, true, false, "none");
				}
				if (num2 > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, num2, "王者战场奖励", true);
				}
				if (list != null)
				{
					foreach (AwardsItemData awardsItemData in list)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "王者战场奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				if (score >= this.RuntimeData.KingOfBattleLowestJiFen && success > 0)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KingOfBattle, new int[0]));
				}
				result = 1;
			}
			return result;
		}

		public void LeaveFuBen(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				KingOfBattleScene kingOfBattleScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kingOfBattleScene))
				{
					kingOfBattleScene.m_nPlarerCount--;
					this.RemoveBattleSceneBuffForRole(kingOfBattleScene, client);
				}
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		private void CheckSceneBufferTime(KingOfBattleScene kingOfBattleScene, long nowTicks)
		{
			List<KingOfBattleSceneBuff> list = new List<KingOfBattleSceneBuff>();
			lock (this.RuntimeData.Mutex)
			{
				if (kingOfBattleScene.m_eStatus == 2)
				{
					if (kingOfBattleScene.SceneBuffDict.Count != 0)
					{
						foreach (KingOfBattleSceneBuff kingOfBattleSceneBuff in kingOfBattleScene.SceneBuffDict.Values)
						{
							if (kingOfBattleSceneBuff.EndTicks < nowTicks)
							{
								list.Add(kingOfBattleSceneBuff);
							}
						}
						if (list.Count != 0)
						{
							foreach (KingOfBattleSceneBuff kingOfBattleSceneBuff in list)
							{
								if (kingOfBattleSceneBuff.RoleID != 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(kingOfBattleSceneBuff.RoleID);
									if (null != gameClient)
									{
										this.UpdateBuff4GameClient(gameClient, kingOfBattleSceneBuff.BuffID, kingOfBattleSceneBuff.tagInfo, false);
									}
								}
								BattleCrystalMonsterItem battleCrystalMonsterItem = kingOfBattleSceneBuff.tagInfo as BattleCrystalMonsterItem;
								if (null != battleCrystalMonsterItem)
								{
									this.AddDelayCreateMonster(kingOfBattleScene, TimeUtil.NOW() + (long)battleCrystalMonsterItem.FuHuoTime, kingOfBattleSceneBuff.tagInfo);
								}
							}
						}
					}
				}
			}
		}

		public const SceneUIClasses ManagerType = 39;

		private static KingOfBattleManager instance = new KingOfBattleManager();

		public KingOfBattleData RuntimeData = new KingOfBattleData();

		public ConcurrentDictionary<int, KingOfBattleScene> SceneDict = new ConcurrentDictionary<int, KingOfBattleScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
