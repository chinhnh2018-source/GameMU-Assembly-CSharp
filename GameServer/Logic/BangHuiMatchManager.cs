using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.Ornament;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class BangHuiMatchManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static BangHuiMatchManager getInstance()
		{
			return BangHuiMatchManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("BangHuiMatchManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1165, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1166, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1167, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1168, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1169, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1170, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1171, 2, 2, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1172, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1174, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1175, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1176, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1177, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1195, 4, 4, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1196, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1197, 1, 1, BangHuiMatchManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(26, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10032, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().registerListener(55, BangHuiMatchManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(26, 10000, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(29, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10032, 45, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, BangHuiMatchManager.getInstance());
			GlobalEventSource.getInstance().removeListener(55, BangHuiMatchManager.getInstance());
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
			if (nID != 1165 && nID != 1168 && nID != 1169 && nID != 1166 && nID != 1195 && nID != 1196 && nID != 1197)
			{
				if (!this.IsGongNengOpened(client, false))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, "", GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			if (nID == 1195 || nID == 1196 || nID == 1197)
			{
				if (!GlobalNew.IsGongNengOpened(client, 120401, true))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			switch (nID)
			{
			case 1165:
				return this.ProcessGetBangHuiMatchMainInfoCmd(client, nID, bytes, cmdParams);
			case 1166:
				return this.ProcessGetBangHuiMatchRankInfoCmd(client, nID, bytes, cmdParams);
			case 1167:
				return this.ProcessGetBangHuiMatchAnalysisDataCmd(client, nID, bytes, cmdParams);
			case 1168:
				return this.ProcessGetBangHuiMatchAdmireDataCmd(client, nID, bytes, cmdParams);
			case 1169:
				return this.ProcessBangHuiMatchAdmireCmd(client, nID, bytes, cmdParams);
			case 1170:
				return this.ProcessBangHuiMatchJoinCmd(client, nID, bytes, cmdParams);
			case 1171:
				return this.ProcessBangHuiMatchEnterCmd(client, nID, bytes, cmdParams);
			case 1172:
				return this.ProcessGetBangHuiMatchStateCmd(client, nID, bytes, cmdParams);
			case 1173:
				break;
			case 1174:
				return this.ProcessGetBangHuiMatchAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1175:
				return this.ProcessGetBangHuiMatchAwardCmd(client, nID, bytes, cmdParams);
			case 1176:
				return this.ProcessGetBangHuiMatchRankAwardCmd(client, nID, bytes, cmdParams);
			case 1177:
				return this.ProcessGetBangHuiMatchRankInfoMiniCmd(client, nID, bytes, cmdParams);
			default:
				switch (nID)
				{
				case 1195:
					return this.ProcessBangHuiMatchGuess(client, nID, bytes, cmdParams);
				case 1196:
					return this.ProcessGetBangHuiMatchGuessInfo(client, nID, bytes, cmdParams);
				case 1197:
					return this.ProcessGetBangHuiMatchGuessAward(client, nID, bytes, cmdParams);
				}
				break;
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 28)
			{
				OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
				this.OnStartPlayGame(onStartPlayGameEventObject.Client);
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
				}
			}
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
			if (eventType == 55)
			{
				PlayerLogoutFinishEventObject playerLogoutFinishEventObject = eventObject as PlayerLogoutFinishEventObject;
				this.OnLogoutFinish(playerLogoutFinishEventObject.getPlayer());
			}
			if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			switch (num)
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
			case 25:
			{
				PreBangHuiChangeZhiWuEventObject preBangHuiChangeZhiWuEventObject = eventObject as PreBangHuiChangeZhiWuEventObject;
				if (preBangHuiChangeZhiWuEventObject != null && preBangHuiChangeZhiWuEventObject.TargetZhiWu == 1)
				{
					if ((long)preBangHuiChangeZhiWuEventObject.Player.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Gold)
					{
						preBangHuiChangeZhiWuEventObject.ErrorCode = -201;
						eventObject.Handled = true;
						eventObject.Result = false;
						GameManager.ClientMgr.NotifyImportantMsg(preBangHuiChangeZhiWuEventObject.Player, GLang.GetLang(2600, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
				break;
			}
			case 26:
			{
				PostBangHuiChangeEventObject postBangHuiChangeEventObject = eventObject as PostBangHuiChangeEventObject;
				if (postBangHuiChangeEventObject != null && null != postBangHuiChangeEventObject.Player)
				{
					this.UpdateChengHaoBuffer(postBangHuiChangeEventObject.Player);
				}
				break;
			}
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
					onClientChangeMapEventObject.Handled = (onClientChangeMapEventObject.Result = this.ClientChangeMap(onClientChangeMapEventObject.Client, ref onClientChangeMapEventObject.ToMapCode, ref onClientChangeMapEventObject.ToPosX, ref onClientChangeMapEventObject.ToPosY));
				}
				break;
			}
			case 30:
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					BHMatchQiZhiConfig bhmatchQiZhiConfig = onCreateMonsterEventObject.Monster.Tag as BHMatchQiZhiConfig;
					if (null != bhmatchQiZhiConfig)
					{
						onCreateMonsterEventObject.Monster.Camp = bhmatchQiZhiConfig.BattleWhichSide;
						onCreateMonsterEventObject.Result = true;
						onCreateMonsterEventObject.Handled = true;
					}
				}
				break;
			}
			case 33:
			{
				PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
				if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 45)
				{
					Monster monster = preMonsterInjureEventObject.Monster;
					if (monster != null)
					{
						if (this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
						{
							this.RuntimeData.MonsterIDVsDamage.TryGetValue(monster.MonsterInfo.ExtensionID, out preMonsterInjureEventObject.Injure);
							eventObject.Handled = true;
							eventObject.Result = true;
						}
					}
				}
				break;
			}
			default:
				if (num == 10032)
				{
					this.HandleNtfEnterEvent((eventObject as KFBHMatchNtfEnterData).Data);
					eventObject.Handled = true;
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
					if (!this.RuntimeData.CommonConfigData.Load(Global.GameResPath("Config\\LeagueMatch.xml"), Global.GameResPath("Config\\LeagueOpen.xml"), Global.GameResPath("Config\\LeagueSuperList.xml"), Global.GameResPath("Config\\LeagueNewRandom.xml"), GameCoreInterface.getinstance().GetPlatformType().ToString()))
					{
						LogManager.WriteLog(2, "BangHuiMatchService.InitConfig failed!", null, true);
					}
					foreach (BHMatchConfig bhmatchConfig in this.RuntimeData.CommonConfigData.BHMatchConfigDict.Values)
					{
						string str = (string)bhmatchConfig.WinAwardTag;
						string str2 = (string)bhmatchConfig.FailAwardTag;
						AwardsItemList winAwardTag = new AwardsItemList();
						AwardsItemList failAwardTag = new AwardsItemList();
						ConfigParser.ParseAwardsItemList(str, ref winAwardTag, '|', ',');
						ConfigParser.ParseAwardsItemList(str2, ref failAwardTag, '|', ',');
						bhmatchConfig.WinAwardTag = winAwardTag;
						bhmatchConfig.FailAwardTag = failAwardTag;
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					text = "Config\\LeagueWar.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BHMatchQiZhiConfig bhmatchQiZhiConfig = new BHMatchQiZhiConfig();
						bhmatchQiZhiConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "QiZuoID");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "QiZuoSite");
						string[] array = safeAttributeStr.Split(new char[]
						{
							'|'
						});
						if (array.Length == 2)
						{
							bhmatchQiZhiConfig.PosX = Global.SafeConvertToInt32(array[0]);
							bhmatchQiZhiConfig.PosY = Global.SafeConvertToInt32(array[1]);
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "RebirthSite");
						string[] array2 = safeAttributeStr2.Split(new char[]
						{
							'|'
						});
						if (array2.Length == 2)
						{
							bhmatchQiZhiConfig.RebirthSiteX = Global.SafeConvertToInt32(array2[0]);
							bhmatchQiZhiConfig.RebirthSiteY = Global.SafeConvertToInt32(array2[1]);
						}
						bhmatchQiZhiConfig.RebirthRadius = (int)Global.GetSafeAttributeLong(xml, "RebirthRadius");
						bhmatchQiZhiConfig.ProduceTime = (int)Global.GetSafeAttributeLong(xml, "ProduceTime");
						bhmatchQiZhiConfig.ProduceNum = (int)Global.GetSafeAttributeLong(xml, "ProduceNum");
						this.RuntimeData.NPCID2QiZhiConfigDict[bhmatchQiZhiConfig.NPCID] = bhmatchQiZhiConfig;
					}
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config\\LeagueBirthPoint.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BHMatchBirthPoint bhmatchBirthPoint = new BHMatchBirthPoint();
						bhmatchBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						bhmatchBirthPoint.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						bhmatchBirthPoint.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						bhmatchBirthPoint.BirthRadius = (int)Global.GetSafeAttributeLong(xml, "BirthRadius");
						this.RuntimeData.MapBirthPointDict[bhmatchBirthPoint.ID] = bhmatchBirthPoint;
					}
					this.RuntimeData.RankAwardConfigList_Gold.Clear();
					text = "Config\\LeagueSuperAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BHMatchRankAwardConfig bhmatchRankAwardConfig = new BHMatchRankAwardConfig();
						bhmatchRankAwardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						bhmatchRankAwardConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						bhmatchRankAwardConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsOne"), ref bhmatchRankAwardConfig.AwardsItemList, '|', ',');
						this.RuntimeData.RankAwardConfigList_Gold.Add(bhmatchRankAwardConfig);
					}
					this.RuntimeData.RankAwardConfigList_Rookie.Clear();
					text = "Config\\LeagueNewAward.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BHMatchRankAwardConfig bhmatchRankAwardConfig = new BHMatchRankAwardConfig();
						bhmatchRankAwardConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						bhmatchRankAwardConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						bhmatchRankAwardConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "GoodsOne"), ref bhmatchRankAwardConfig.AwardsItemList, '|', ',');
						this.RuntimeData.RankAwardConfigList_Rookie.Add(bhmatchRankAwardConfig);
					}
					this.RuntimeData.BHMatchGoldGuessConfigDict.Clear();
					text = "Config\\LeagueSustain.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						BHMatchGoldGuessConfig bhmatchGoldGuessConfig = new BHMatchGoldGuessConfig();
						bhmatchGoldGuessConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						bhmatchGoldGuessConfig.Round = (int)Global.GetSafeAttributeLong(xml, "Round");
						bhmatchGoldGuessConfig.Cost = (int)Global.GetSafeAttributeLong(xml, "Cost");
						bhmatchGoldGuessConfig.WinAward = (int)Global.GetSafeAttributeLong(xml, "WinAward");
						bhmatchGoldGuessConfig.FaillAward = (int)Global.GetSafeAttributeLong(xml, "FaillAward");
						int[] array3 = Global.String2IntArray(Global.GetSafeAttributeStr(xml, "MinLevel"), '|');
						bhmatchGoldGuessConfig.UnionLevLimit = Global.GetUnionLevel(array3[0], array3[1], false);
						this.RuntimeData.BHMatchGoldGuessConfigDict[bhmatchGoldGuessConfig.ID] = bhmatchGoldGuessConfig;
					}
					this.RuntimeData.BHMatchGodDamagePeriod = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueHurtTime", -1);
					this.RuntimeData.BHMatchGodDamagePctList.Clear();
					double[] paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LeagueHurt", ',');
					foreach (double item in paramValueDoubleArrayByName)
					{
						this.RuntimeData.BHMatchGodDamagePctList.Add(item);
					}
					paramValueDoubleArrayByName = GameManager.systemParamsList.GetParamValueDoubleArrayByName("LeagueDeHurt", ',');
					this.RuntimeData.BHMatchGodDebuffTemple = paramValueDoubleArrayByName[0];
					this.RuntimeData.BHMatchGodDebuffQiZhi = paramValueDoubleArrayByName[1];
					this.RuntimeData.GoldWingGoodsID = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueWing", -1);
					this.RuntimeData.MonsterIDVsDamage.Clear();
					string paramValueByName = GameManager.systemParamsList.GetParamValueByName("LeagueQiZhi");
					string[] array5 = paramValueByName.Split(new char[]
					{
						'|'
					});
					for (int j = 0; j < array5.Length; j++)
					{
						string[] array6 = array5[j].Split(new char[]
						{
							','
						});
						int num = Global.SafeConvertToInt32(array6[0]);
						int value = Global.SafeConvertToInt32(array6[1]);
						this.RuntimeData.MonsterIDVsDamage[num] = value;
						if (j == 0)
						{
							this.RuntimeData.BattleQiZhiMonsterID1 = num;
						}
						if (j == 1)
						{
							this.RuntimeData.BattleQiZhiMonsterID2 = num;
						}
					}
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueShenDian", ',');
					this.RuntimeData.TempleProduceTime = paramValueIntArrayByName[0];
					this.RuntimeData.TempleProduceNum = paramValueIntArrayByName[1];
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueMvpNum", ',');
					this.RuntimeData.MVPScoreFactorKill = paramValueIntArrayByName[0];
					this.RuntimeData.MVPScoreFactorQiZhi = paramValueIntArrayByName[1];
					this.RuntimeData.MVPScoreFactorTemple = paramValueIntArrayByName[2];
					paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("LeagueNewNum", ',');
					this.RuntimeData.CommonConfigData.RookieWinScoreFactor = paramValueIntArrayByName[0];
					this.RuntimeData.CommonConfigData.RookieLoseScoreFactor = paramValueIntArrayByName[1];
					this.RuntimeData.CommonConfigData.RookiePromotionNum = (int)GameManager.systemParamsList.GetParamValueIntByName("LeagueUp", -1);
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
			}
			return result;
		}

		private int GetCurrentBHMatchGoldRound()
		{
			int result = 0;
			BHMatchConfig bhmatchConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
			BHMatchConfig bhmatchConfig2 = this.RuntimeData.CommonConfigData.GetBHMatchConfig(2);
			if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold)
			{
				if (this.BHMatchSyncDataCache.RoundGoldReal > bhmatchConfig.RoundNum)
				{
					result = bhmatchConfig.RoundNum + 1;
				}
				else if (this.BHMatchSyncDataCache.RoundGoldReal <= bhmatchConfig.RoundNum)
				{
					result = this.BHMatchSyncDataCache.RoundGoldReal;
				}
			}
			else
			{
				result = this.BHMatchSyncDataCache.RoundGoldReal;
			}
			return result;
		}

		public bool ProcessGetBangHuiMatchMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				BangHuiMatchMainInfo mainInfo = new BangHuiMatchMainInfo();
				if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
				{
					client.sendCmd<BangHuiMatchMainInfo>(nID, mainInfo, false);
					return true;
				}
				mainInfo.round = this.GetCurrentBHMatchGoldRound();
				List<BangHuiMatchPKInfo> list = null;
				lock (this.RuntimeData.Mutex)
				{
					if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
					{
						list = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
					}
				}
				foreach (BangHuiMatchPKInfo bangHuiMatchPKInfo in list.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == mainInfo.round))
				{
					mainInfo.CurRoundPKInfo.Add((BangHuiMatchPKInfo)bangHuiMatchPKInfo.Clone());
				}
				foreach (BangHuiMatchPKInfo bangHuiMatchPKInfo in list.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == mainInfo.round - 1))
				{
					mainInfo.LastRoundPKInfo.Add((BangHuiMatchPKInfo)bangHuiMatchPKInfo.Clone());
				}
				if (mainInfo.CurRoundPKInfo != null && mainInfo.CurRoundPKInfo.Count != 0)
				{
					DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.BHMatchSyncDataCache.CurSeasonID_Gold);
					BHMatchConfig bhmatchConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
					BHMatchConfig bhmatchConfig2 = null;
					BangHuiMatchGameStates bangHuiMatchGameStates = 0;
					this.CheckCondition(client, 1, ref bhmatchConfig2, ref bangHuiMatchGameStates);
					if (TimeUtil.NowDateTime() < seasonDateTm)
					{
						bangHuiMatchGameStates = 2;
					}
					else if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > bhmatchConfig.RoundNum)
					{
						bangHuiMatchGameStates = 6;
					}
					else if (bangHuiMatchGameStates == 1 || bangHuiMatchGameStates == 2)
					{
						bangHuiMatchGameStates = 2;
					}
					mainInfo.seasonid = this.BHMatchSyncDataCache.CurSeasonID_Gold;
					mainInfo.timestate = bangHuiMatchGameStates;
				}
				List<BHMatchSupportData> list2 = client.ClientData.BHMatchSupportList.FindAll((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == mainInfo.round);
				using (List<BangHuiMatchPKInfo>.Enumerator enumerator = mainInfo.CurRoundPKInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BangHuiMatchPKInfo item = enumerator.Current;
						BHMatchSupportData bhmatchSupportData = list2.Find((BHMatchSupportData x) => x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
						if (null != bhmatchSupportData)
						{
							item.guess = (byte)bhmatchSupportData.guess;
						}
					}
				}
				list2 = client.ClientData.BHMatchSupportList.FindAll((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == mainInfo.round - 1);
				using (List<BangHuiMatchPKInfo>.Enumerator enumerator = mainInfo.LastRoundPKInfo.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BangHuiMatchPKInfo item = enumerator.Current;
						BHMatchSupportData bhmatchSupportData = list2.Find((BHMatchSupportData x) => x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
						if (null != bhmatchSupportData)
						{
							item.guess = (byte)bhmatchSupportData.guess;
						}
					}
				}
				client.sendCmd<BangHuiMatchMainInfo>(nID, mainInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private BangHuiMatchRankInfo SearchMyRankInfo(GameClient client, int type, List<BangHuiMatchRankInfo> rankInfoList)
		{
			BangHuiMatchRankInfo bangHuiMatchRankInfo = null;
			BangHuiMatchRankInfo result;
			if (rankInfoList.Count == 0)
			{
				result = bangHuiMatchRankInfo;
			}
			else
			{
				switch (type)
				{
				case 4:
				case 5:
				case 6:
				case 7:
				case 10:
				case 11:
				case 12:
				case 13:
					bangHuiMatchRankInfo = rankInfoList.Find((BangHuiMatchRankInfo x) => x.Key == client.ClientData.RoleID);
					goto IL_9D;
				}
				bangHuiMatchRankInfo = rankInfoList.Find((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				IL_9D:
				result = bangHuiMatchRankInfo;
			}
			return result;
		}

		private BangHuiMatchRankInfo GeneralRankInfo(GameClient client, int type)
		{
			BHMatchBHData bhmatchBHData = null;
			BangHuiMatchRankInfo bangHuiMatchRankInfo = new BangHuiMatchRankInfo();
			List<int> bhmatchRoleAnalysisData = this.GetBHMatchRoleAnalysisData(client);
			BangHuiMatchRankInfo result;
			if (null == bhmatchRoleAnalysisData)
			{
				result = null;
			}
			else
			{
				switch (type)
				{
				case 0:
				case 2:
				case 8:
					bhmatchBHData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, client.ClientData.Faction);
					if (null == bhmatchBHData)
					{
						return null;
					}
					if (type == 0)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.last_win;
					}
					else if (type == 2)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.hist_champion;
					}
					else if (type == 8)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.cur_win;
					}
					break;
				case 1:
				case 3:
				case 9:
					bhmatchBHData = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(2, client.ClientData.Faction);
					if (null == bhmatchBHData)
					{
						return null;
					}
					if (type == 1)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.last_score;
					}
					else if (type == 3)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.hist_champion;
					}
					else if (type == 9)
					{
						bangHuiMatchRankInfo.Value = bhmatchBHData.cur_score;
					}
					break;
				case 4:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[4];
					break;
				case 5:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[7];
					break;
				case 6:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[2];
					break;
				case 7:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[5];
					break;
				case 10:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[3];
					break;
				case 11:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[6];
					break;
				case 12:
				case 13:
					bangHuiMatchRankInfo.Value = bhmatchRoleAnalysisData[8];
					break;
				}
				if (null != bhmatchBHData)
				{
					bangHuiMatchRankInfo.Key = client.ClientData.Faction;
					bangHuiMatchRankInfo.Param1 = Global.FormatBangHuiNameWithZone(bhmatchBHData.zoneid_bh, bhmatchBHData.bhname);
					bangHuiMatchRankInfo.Param2 = Global.FormatRoleNameWithZoneId2(client);
					bangHuiMatchRankInfo.Key = client.ClientData.Faction;
				}
				else
				{
					bangHuiMatchRankInfo.Key = client.ClientData.RoleID;
					bangHuiMatchRankInfo.Param1 = Global.FormatRoleNameWithZoneId2(client);
				}
				result = bangHuiMatchRankInfo;
			}
			return result;
		}

		private int GetBangHuiMatchRankNum(GameClient client, int ranktype)
		{
			List<BangHuiMatchRankInfo> list = new List<BangHuiMatchRankInfo>();
			lock (this.RuntimeData.Mutex)
			{
				if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
				{
					this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(ranktype, out list);
				}
			}
			int result;
			if (list.Count == 0)
			{
				result = 0;
			}
			else
			{
				int num;
				switch (ranktype)
				{
				case 4:
				case 5:
				case 6:
				case 7:
				case 10:
				case 11:
				case 12:
				case 13:
					num = list.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.RoleID);
					goto IL_10B;
				}
				num = list.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				IL_10B:
				if (-1 == num)
				{
					result = 0;
				}
				else
				{
					result = num + 1;
				}
			}
			return result;
		}

		public bool ProcessGetBangHuiMatchRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				List<BangHuiMatchRankInfo> collection = new List<BangHuiMatchRankInfo>();
				lock (this.RuntimeData.Mutex)
				{
					if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
					{
						this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(num2, out collection);
					}
				}
				List<BangHuiMatchRankInfo> list = new List<BangHuiMatchRankInfo>(collection);
				if (list.Count != 0 && num2 > 7 && num2 != 8)
				{
					BangHuiMatchRankInfo bangHuiMatchRankInfo = this.SearchMyRankInfo(client, num2, list);
					if (null != bangHuiMatchRankInfo)
					{
						list.Add(bangHuiMatchRankInfo);
					}
					else
					{
						bangHuiMatchRankInfo = this.GeneralRankInfo(client, num2);
						if (null != bangHuiMatchRankInfo)
						{
							list.Add(bangHuiMatchRankInfo);
						}
					}
				}
				client.sendCmd<List<BangHuiMatchRankInfo>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchAnalysisDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
				if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
				{
					return true;
				}
				string[] array = kuaFuGameState_BHMatch.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					return true;
				}
				int num = Global.SafeConvertToInt32(array[0]);
				int num2 = Global.SafeConvertToInt32(array[1]);
				BHMatchBHData bhdataByBhid_BHMatch = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, client.ClientData.Faction);
				BHMatchBHData bhdataByBhid_BHMatch2 = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(2, client.ClientData.Faction);
				BangHuiAnalysisInfo bangHuiAnalysisInfo = new BangHuiAnalysisInfo();
				bangHuiAnalysisInfo.listAnalysisData = new List<int>(new int[24]);
				List<int> bhmatchRoleAnalysisData = this.GetBHMatchRoleAnalysisData(client);
				if (null == bhmatchRoleAnalysisData)
				{
					client.sendCmd<BangHuiAnalysisInfo>(nID, bangHuiAnalysisInfo, false);
					return true;
				}
				List<int> bhmatchRAnalysisExData = this.GetBHMatchRAnalysisExData(client);
				bangHuiAnalysisInfo.listAnalysisData[0] = bhmatchRAnalysisExData[0];
				bangHuiAnalysisInfo.listAnalysisData[1] = bhmatchRAnalysisExData[1];
				bangHuiAnalysisInfo.listAnalysisData[4] = bhmatchRAnalysisExData[2];
				bangHuiAnalysisInfo.listAnalysisData[5] = bhmatchRAnalysisExData[3];
				bangHuiAnalysisInfo.listAnalysisData[2] = bhmatchRoleAnalysisData[2];
				bangHuiAnalysisInfo.listAnalysisData[3] = this.GetBangHuiMatchRankNum(client, 6);
				bangHuiAnalysisInfo.listAnalysisData[6] = bhmatchRoleAnalysisData[5];
				bangHuiAnalysisInfo.listAnalysisData[7] = this.GetBangHuiMatchRankNum(client, 7);
				bangHuiAnalysisInfo.listAnalysisData[8] = bhmatchRoleAnalysisData[8];
				bangHuiAnalysisInfo.listAnalysisData[9] = bhmatchRoleAnalysisData[9];
				bangHuiAnalysisInfo.listAnalysisData[10] = bhmatchRoleAnalysisData[10];
				bangHuiAnalysisInfo.listAnalysisData[11] = 0;
				bangHuiAnalysisInfo.listAnalysisData[12] = ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.hist_champion : 0);
				bangHuiAnalysisInfo.listAnalysisData[13] = this.GetBangHuiMatchRankNum(client, 2);
				bangHuiAnalysisInfo.listAnalysisData[14] = ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.best_record : 0);
				bangHuiAnalysisInfo.listAnalysisData[15] = ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.hist_win : 0);
				bangHuiAnalysisInfo.listAnalysisData[16] = 0;
				bangHuiAnalysisInfo.listAnalysisData[17] = ((bhdataByBhid_BHMatch2 != null) ? bhdataByBhid_BHMatch2.hist_champion : 0);
				bangHuiAnalysisInfo.listAnalysisData[18] = this.GetBangHuiMatchRankNum(client, 3);
				bangHuiAnalysisInfo.listAnalysisData[19] = ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.best_record : 0);
				bangHuiAnalysisInfo.listAnalysisData[20] = ((bhdataByBhid_BHMatch2 != null) ? bhdataByBhid_BHMatch2.hist_win : 0);
				bangHuiAnalysisInfo.listAnalysisData[21] = 0;
				int num3 = 0;
				num3 += ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.hist_kill : 0);
				num3 += ((bhdataByBhid_BHMatch2 != null) ? bhdataByBhid_BHMatch2.hist_kill : 0);
				bangHuiAnalysisInfo.listAnalysisData[22] = num3;
				int num4 = 0;
				num4 += ((bhdataByBhid_BHMatch != null) ? bhdataByBhid_BHMatch.hist_bullshit : 0);
				num4 += ((bhdataByBhid_BHMatch2 != null) ? bhdataByBhid_BHMatch2.hist_bullshit : 0);
				bangHuiAnalysisInfo.listAnalysisData[23] = num4;
				client.sendCmd<BangHuiAnalysisInfo>(nID, bangHuiAnalysisInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				client.sendCmd<BHMatchKingShowData>(nID, new BHMatchKingShowData
				{
					AdmireCount = Global.GetBHMatchAdmireCount(client),
					RoleData4Selector = Global.RoleDataEx2RoleData4Selector(this.OwnerRoleData)
				}, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBangHuiMatchAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				MoBaiData moBaiData = null;
				string cmdData;
				if (!Data.MoBaiDataInfoList.TryGetValue(6, out moBaiData))
				{
					cmdData = string.Format("{0}", -2);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (client.ClientData.ChangeLifeCount < moBaiData.MinZhuanSheng || (client.ClientData.ChangeLifeCount == moBaiData.MinZhuanSheng && client.ClientData.Level < moBaiData.MinLevel))
				{
					cmdData = string.Format("{0}", -2);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				int num3 = moBaiData.AdrationMaxLimit;
				int bhmatchAdmireCount = Global.GetBHMatchAdmireCount(client);
				if (this.OwnerRoleData != null && client.ClientData.RoleID == this.OwnerRoleData.RoleID)
				{
					num3 += moBaiData.ExtraNumber;
				}
				int vipLevel = client.ClientData.VipLevel;
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("VIPMoBaiNum", ',');
				if (vipLevel > VIPEumValue.VIPENUMVALUE_MAXLEVEL || paramValueIntArrayByName.Length < 1)
				{
					cmdData = string.Format("{0}", -2);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				num3 += paramValueIntArrayByName[vipLevel];
				double num4 = 0.0;
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null != jieRiMultAwardActivity)
				{
					JieRiMultConfig config = jieRiMultAwardActivity.GetConfig(12);
					if (null != config)
					{
						num4 += config.GetMult();
					}
				}
				SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
				if (null != specPriorityActivity)
				{
					num4 += specPriorityActivity.GetMult(SpecPActivityBuffType.SPABT_Admire);
				}
				num4 = Math.Max(1.0, num4);
				num3 = (int)((double)num3 * num4);
				if (bhmatchAdmireCount >= num3)
				{
					cmdData = string.Format("{0}", -3);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				double num5;
				if (client.ClientData.ChangeLifeCount == 0)
				{
					num5 = 1.0;
				}
				else
				{
					num5 = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
				}
				if (num2 == 1)
				{
					if (!Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜战盟联赛盟主"))
					{
						cmdData = string.Format("{0}", -4);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					long num6 = (long)(num5 * (double)moBaiData.JinBiExpAward);
					if (num6 > 0L)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, num6, true, true, false, "none");
					}
					if (moBaiData.JinBiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.BHMatchMoBai, 0);
					}
					if (moBaiData.LingJingAwardByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜战盟联赛盟主", true, true, false);
					}
					if (moBaiData.ShenLiJingHuaByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByJinBi, "膜拜战盟联赛盟主", true, true);
					}
				}
				else if (num2 == 2)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜战盟联赛盟主", true, true, false, DaiBiSySType.None))
					{
						cmdData = string.Format("{0}", -5);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					int num7 = (int)(num5 * (double)moBaiData.ZuanShiExpAward);
					if (num7 > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)num7, true, true, false, "none");
					}
					if (moBaiData.ZuanShiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.BHMatchMoBai, 0);
					}
					if (moBaiData.LingJingAwardByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜战盟联赛盟主", true, true, false);
					}
					if (moBaiData.ShenLiJingHuaByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByZuanShi, "膜拜战盟联赛盟主", true, true);
					}
				}
				Global.ProcessIncreaseBHMatchAdmireCount(client);
				cmdData = string.Format("{0}", 1);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBangHuiMatchJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				if (this.IsGongNengOpened(client, false))
				{
					if (client.ClientData.BHZhiWu != 1)
					{
						num = -1002;
					}
					else
					{
						BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(client.ClientData.Faction, 0);
						if (null == bangHuiMiniData)
						{
							num = -1001;
						}
						else
						{
							string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
							if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
							{
								num = -11003;
							}
							else
							{
								string[] array = kuaFuGameState_BHMatch.Split(new char[]
								{
									':'
								});
								if (array.Length != 2)
								{
									num = -11003;
								}
								else
								{
									int num2 = Global.SafeConvertToInt32(array[0]);
									int num3 = Global.SafeConvertToInt32(array[1]);
									if (num2 != 2)
									{
										num = -5;
									}
									else
									{
										BHMatchConfig bhmatchConfig = null;
										BangHuiMatchGameStates bangHuiMatchGameStates = 0;
										if (!this.CheckMap(client))
										{
											num = -21;
										}
										else
										{
											num = this.CheckCondition(client, num2, ref bhmatchConfig, ref bangHuiMatchGameStates);
										}
										if (bangHuiMatchGameStates != 1)
										{
											num = -2001;
										}
										else if (num3 == -4005)
										{
											num = -12;
										}
										if (num >= 0)
										{
											num = TianTiClient.getInstance().RookieSignUp_BHMatch(client.ClientData.Faction, bangHuiMiniData.ZoneID, bangHuiMiniData.BHName, client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.ZoneID);
											if (num >= 0)
											{
												client.ClientData.SignUpGameType = 24;
											}
										}
									}
								}
							}
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

		public bool ProcessBangHuiMatchEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				if (client.ClientData.GuanZhanGM == 0)
				{
					num = client.ClientData.Faction;
				}
				int cmdData;
				if (!this.IsGongNengOpened(client, true))
				{
					cmdData = -13;
				}
				else
				{
					string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(num);
					if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
					{
						cmdData = -11003;
					}
					else
					{
						string[] array = kuaFuGameState_BHMatch.Split(new char[]
						{
							':'
						});
						if (array.Length != 2)
						{
							cmdData = -11003;
						}
						else
						{
							int matchType = Global.SafeConvertToInt32(array[0]);
							int num2 = Global.SafeConvertToInt32(array[1]);
							BHMatchConfig bhmatchConfig = null;
							BangHuiMatchGameStates bangHuiMatchGameStates = 0;
							if (!this.CheckMap(client))
							{
								cmdData = -21;
							}
							else
							{
								cmdData = this.CheckCondition(client, matchType, ref bhmatchConfig, ref bangHuiMatchGameStates);
								if (bangHuiMatchGameStates != 3)
								{
									cmdData = -2001;
								}
								else
								{
									KuaFuServerInfo kuaFuServerInfo = null;
									BHMatchFuBenData fuBenDataByBhid_BHMatch = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(num);
									if (fuBenDataByBhid_BHMatch == null || !KuaFuManager.getInstance().TryGetValue(fuBenDataByBhid_BHMatch.ServerId, out kuaFuServerInfo))
									{
										cmdData = -11000;
									}
									else
									{
										KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
										if (null != clientKuaFuServerLoginData)
										{
											clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
											clientKuaFuServerLoginData.GameId = fuBenDataByBhid_BHMatch.GameId;
											clientKuaFuServerLoginData.GameType = 24;
											clientKuaFuServerLoginData.EndTicks = 0L;
											clientKuaFuServerLoginData.ServerId = client.ServerId;
											clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
											clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
											clientKuaFuServerLoginData.FuBenSeqId = num;
										}
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
									}
								}
							}
						}
					}
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

		public bool ProcessGetBangHuiMatchStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = 51;
				if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						0,
						0,
						num,
						num2,
						this.BHMatchSyncDataCache.CurSeasonID_Gold
					}), false);
					return true;
				}
				string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
				if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						0,
						0,
						num,
						num2,
						this.BHMatchSyncDataCache.CurSeasonID_Gold
					}), false);
					return true;
				}
				string[] array = kuaFuGameState_BHMatch.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						0,
						0,
						num,
						num2,
						this.BHMatchSyncDataCache.CurSeasonID_Gold
					}), false);
					return true;
				}
				int num3 = Global.SafeConvertToInt32(array[0]);
				int num4 = Global.SafeConvertToInt32(array[1]);
				int num5 = (num3 == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
				DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(num5);
				BHMatchConfig bhmatchConfig = null;
				BangHuiMatchGameStates bangHuiMatchGameStates = 0;
				this.CheckCondition(client, num3, ref bhmatchConfig, ref bangHuiMatchGameStates);
				if (num4 == -4009)
				{
					bangHuiMatchGameStates = 7;
				}
				else if (num4 == -4005)
				{
					BHMatchConfig bhmatchConfig2 = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
					BHMatchConfig bhmatchConfig3 = this.RuntimeData.CommonConfigData.GetBHMatchConfig(2);
					if (num3 == 1 && TimeUtil.NowDateTime() < seasonDateTm)
					{
						bangHuiMatchGameStates = 2;
					}
					else if ((num3 == 1 && this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > bhmatchConfig2.RoundNum) || (num3 == 2 && this.BHMatchSyncDataCache.CurSeasonID_Rookie != this.BHMatchSyncDataCache.CurSeasonID_Gold))
					{
						bangHuiMatchGameStates = 6;
					}
					else if (bangHuiMatchGameStates == 1 || bangHuiMatchGameStates == 2)
					{
						bangHuiMatchGameStates = 2;
					}
				}
				else if (num3 == 2 && TimeUtil.NowDateTime() < seasonDateTm)
				{
					bangHuiMatchGameStates = 6;
				}
				else if (bangHuiMatchGameStates == 2 || bangHuiMatchGameStates == 3)
				{
					bangHuiMatchGameStates = 5;
				}
				List<BangHuiMatchRankInfo> list = null;
				List<BangHuiMatchRankInfo> list2 = null;
				lock (this.RuntimeData.Mutex)
				{
					this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(0, out list);
					this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(1, out list2);
				}
				int num6;
				if (list == null)
				{
					num6 = -1;
				}
				else
				{
					num6 = list.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				}
				int num7 = num6;
				int num8;
				if (list2 == null)
				{
					num8 = -1;
				}
				else
				{
					num8 = list2.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				}
				int num9 = num8;
				if (-1 != num7)
				{
					num = 1;
					num2 = num7 + 1;
				}
				else if (TianTiClient.getInstance().CheckRookieJoinLast_BHMatch(client.ClientData.Faction))
				{
					num = 2;
				}
				if (-1 != num9)
				{
					num = 2;
					num2 = num9 + 1;
				}
				if (0 != num)
				{
					int num10 = (num == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
					DateTime lastSeasonLastMatchEndTime = this.GetLastSeasonLastMatchEndTime(num);
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
					DateTime t = new DateTime(DataHelper.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L);
					if (t > lastSeasonLastMatchEndTime)
					{
						num = 0;
						num2 = 0;
					}
					else
					{
						List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "42");
						if (roleParamsIntListFromDB.Count == 2 && roleParamsIntListFromDB[num - 1] == num10)
						{
							num = 0;
							num2 = 0;
						}
					}
				}
				string roleParamByName = Global.GetRoleParamByName(client, "39");
				if (!string.IsNullOrEmpty(roleParamByName))
				{
					bangHuiMatchGameStates = 4;
				}
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					num3,
					bangHuiMatchGameStates,
					num,
					num2,
					this.BHMatchSyncDataCache.CurSeasonID_Gold
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				string text = Global.GetRoleParamByName(client, "39");
				if (!string.IsNullOrEmpty(text))
				{
					int num = 0;
					int success = 0;
					int mvprid = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 6 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(text, ref num, ref success, ref mvprid, '&');
					if (list.Count >= 6 && num > 0)
					{
						string mvpname = list[3];
						int mvpocc = Global.SafeConvertToInt32(list[4]);
						int mvpsex = Global.SafeConvertToInt32(list[5]);
						BHMatchConfig sceneInfo = null;
						if (this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(num, out sceneInfo))
						{
							this.NtfCanGetAward(client, success, sceneInfo, mvprid, mvpname, mvpocc, mvpsex, "");
						}
					}
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				string text = Global.GetRoleParamByName(client, "39");
				if (!string.IsNullOrEmpty(text))
				{
					int num2 = 0;
					int success = 0;
					int num3 = 0;
					List<string> list = Global.StringToList(text, '&');
					if (list.Count != 6 && !string.IsNullOrEmpty(text))
					{
						byte[] bytes2 = Convert.FromBase64String(text);
						text = new UTF8Encoding().GetString(bytes2);
						list = Global.StringToList(text, '&');
					}
					ConfigParser.ParseStrInt3(text, ref num2, ref success, ref num3, '&');
					bool flag = true;
					if (list.Count >= 6 && num2 > 0)
					{
						string text2 = list[3];
						int num4 = Global.SafeConvertToInt32(list[4]);
						int num5 = Global.SafeConvertToInt32(list[5]);
						BHMatchConfig sceneInfo = null;
						if (this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(num2, out sceneInfo))
						{
							num = this.GiveRoleAwards(client, success, sceneInfo);
							if (num < 0)
							{
								flag = false;
							}
						}
					}
					if (flag)
					{
						Global.SaveRoleParamsStringToDB(client, "39", "", true);
					}
					client.sendCmd<int>(nID, num, false);
				}
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private DateTime GetLastSeasonLastMatchEndTime(int lastMatchType)
		{
			bool flag = lastMatchType == 1;
			int num = (lastMatchType == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
			DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(num);
			TimeSpan timeSpan = TimeSpan.MinValue;
			lock (this.RuntimeData.Mutex)
			{
				foreach (BHMatchConfig bhmatchConfig in this.RuntimeData.CommonConfigData.BHMatchConfigDict.Values)
				{
					if (bhmatchConfig.ID != 1 || flag)
					{
						int num2 = bhmatchConfig.TimePoints.Count / 2 - bhmatchConfig.RoundNum % (bhmatchConfig.TimePoints.Count / 2);
						for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
						{
							TimeSpan timeSpan2 = bhmatchConfig.TimePoints[i + 1];
							if (timeSpan2.Days == 0)
							{
								timeSpan2 += new TimeSpan(7, 0, 0, 0);
							}
							if (timeSpan2 > timeSpan && i / 2 < num2)
							{
								timeSpan = timeSpan2;
							}
						}
					}
				}
			}
			timeSpan -= new TimeSpan(1, 0, 0, 0);
			TimeSpan t = new TimeSpan(7, 0, 0, 0) - timeSpan;
			return seasonDateTm - t;
		}

		public bool ProcessGetBangHuiMatchRankAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				List<BangHuiMatchRankInfo> list = null;
				List<BangHuiMatchRankInfo> list2 = null;
				lock (this.RuntimeData.Mutex)
				{
					this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(0, out list);
					this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(1, out list2);
				}
				int num;
				if (list == null)
				{
					num = -1;
				}
				else
				{
					num = list.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				}
				int num2 = num;
				int num3;
				if (list2 == null)
				{
					num3 = -1;
				}
				else
				{
					num3 = list2.FindIndex((BangHuiMatchRankInfo x) => x.Key == client.ClientData.Faction);
				}
				int num4 = num3;
				int num5 = 0;
				int rankNum = 51;
				if (-1 != num2)
				{
					num5 = 1;
					rankNum = num2 + 1;
				}
				else if (TianTiClient.getInstance().CheckRookieJoinLast_BHMatch(client.ClientData.Faction))
				{
					num5 = 2;
				}
				if (-1 != num4)
				{
					num5 = 2;
					rankNum = num4 + 1;
				}
				if (num5 == 0)
				{
					cmdData = -5;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				int num6 = (num5 == 1) ? this.BHMatchSyncDataCache.CurSeasonID_Gold : this.BHMatchSyncDataCache.CurSeasonID_Rookie;
				DateTime lastSeasonLastMatchEndTime = this.GetLastSeasonLastMatchEndTime(num5);
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
				DateTime t = new DateTime(DataHelper.UnixSecondsToTicks(roleParamsInt32FromDB) * 10000L);
				if (t > lastSeasonLastMatchEndTime)
				{
					cmdData = -2006;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "42");
				if (roleParamsIntListFromDB.Count != 2)
				{
					roleParamsIntListFromDB.Add(0);
					roleParamsIntListFromDB.Add(0);
				}
				if (roleParamsIntListFromDB[num5 - 1] == num6)
				{
					cmdData = -200;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				BHMatchRankAwardConfig bhmatchRankAwardConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					if (num5 == 1)
					{
						bhmatchRankAwardConfig = this.RuntimeData.RankAwardConfigList_Gold.Find((BHMatchRankAwardConfig x) => rankNum >= x.BeginNum && rankNum <= x.EndNum);
					}
					else
					{
						bhmatchRankAwardConfig = this.RuntimeData.RankAwardConfigList_Rookie.Find((BHMatchRankAwardConfig x) => rankNum >= x.BeginNum && (x.EndNum == -1 || rankNum <= x.EndNum));
					}
				}
				if (bhmatchRankAwardConfig != null && !Global.CanAddGoodsNum(client, bhmatchRankAwardConfig.AwardsItemList.Items.Count))
				{
					cmdData = -100;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				if (bhmatchRankAwardConfig != null)
				{
					foreach (AwardsItemData awardsItemData in bhmatchRankAwardConfig.AwardsItemList.Items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "战盟联赛排行榜奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				roleParamsIntListFromDB[num5 - 1] = num6;
				Global.SaveRoleParamsIntListToDB(client, roleParamsIntListFromDB, "42", true);
				List<int> bhmatchRAnalysisExData = this.GetBHMatchRAnalysisExData(client);
				if (null != bhmatchRAnalysisExData)
				{
					if (num5 == 1)
					{
						if (bhmatchRAnalysisExData[1] == 0 || bhmatchRAnalysisExData[1] > rankNum)
						{
							bhmatchRAnalysisExData[1] = rankNum;
						}
						if (rankNum == 1)
						{
							List<int> list3;
							(list3 = bhmatchRAnalysisExData)[0] = list3[0] + 1;
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_BHMatchGoldChampion, new int[0]));
						}
					}
					else
					{
						if (bhmatchRAnalysisExData[3] == 0 || bhmatchRAnalysisExData[3] > rankNum)
						{
							bhmatchRAnalysisExData[3] = rankNum;
						}
						if (rankNum == 1)
						{
							List<int> list3;
							(list3 = bhmatchRAnalysisExData)[2] = list3[2] + 1;
						}
					}
					this.SaveBHMatchRAnalysisExData(client, bhmatchRAnalysisExData);
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

		public bool ProcessGetBangHuiMatchRankInfoMiniCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<BangHuiMatchRankInfo> list = new List<BangHuiMatchRankInfo>();
				for (int i = 0; i <= 7; i++)
				{
					List<BangHuiMatchRankInfo> list2 = new List<BangHuiMatchRankInfo>();
					lock (this.RuntimeData.Mutex)
					{
						if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict.V)
						{
							this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(i, out list2);
						}
					}
					if (list2 == null || list2.Count == 0)
					{
						list.Add(new BangHuiMatchRankInfo());
					}
					else
					{
						list.Add(list2[0]);
					}
				}
				client.sendCmd<List<BangHuiMatchRankInfo>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessBangHuiMatchGuess(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int bhid1 = Convert.ToInt32(cmdParams[1]);
				int bhid2 = Convert.ToInt32(cmdParams[2]);
				int num3 = Convert.ToInt32(cmdParams[3]);
				if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
				{
					num = -2001;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				int goldRound = this.GetCurrentBHMatchGoldRound();
				BHMatchSupportData bhmatchSupportData = client.ClientData.BHMatchSupportList.Find((BHMatchSupportData x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && x.round == goldRound && x.bhid1 == bhid1 && x.bhid2 == bhid2);
				if (null != bhmatchSupportData)
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.BHMatchSyncDataCache.CurSeasonID_Gold);
				BHMatchConfig bhmatchConfig = this.RuntimeData.CommonConfigData.GetBHMatchConfig(1);
				BHMatchConfig bhmatchConfig2 = null;
				BangHuiMatchGameStates bangHuiMatchGameStates = 0;
				this.CheckCondition(client, 1, ref bhmatchConfig2, ref bangHuiMatchGameStates);
				if (TimeUtil.NowDateTime() < seasonDateTm)
				{
					bangHuiMatchGameStates = 2;
				}
				else if (this.BHMatchSyncDataCache.CurSeasonID_Rookie == this.BHMatchSyncDataCache.CurSeasonID_Gold && this.BHMatchSyncDataCache.RoundGoldReal > bhmatchConfig.RoundNum)
				{
					bangHuiMatchGameStates = 6;
				}
				if (1 != bangHuiMatchGameStates)
				{
					num = -2001;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				List<BangHuiMatchPKInfo> list = null;
				lock (this.RuntimeData.Mutex)
				{
					if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
					{
						list = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
					}
				}
				List<BangHuiMatchPKInfo> list2 = new List<BangHuiMatchPKInfo>(list.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == goldRound));
				BangHuiMatchPKInfo bangHuiMatchPKInfo = list2.Find((BangHuiMatchPKInfo x) => x.bhid1 == bhid1 && x.bhid2 == bhid2);
				if (null == bangHuiMatchPKInfo)
				{
					num = -5;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				BHMatchGoldGuessConfig bhmatchGoldGuessConfig;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(goldRound, out bhmatchGoldGuessConfig))
					{
						num = -3;
						client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							num,
							bhid1,
							bhid2,
							num3
						}), false);
						return true;
					}
				}
				if (Global.GetUnionLevel(client, false) < bhmatchGoldGuessConfig.UnionLevLimit)
				{
					num = -19;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				if (client.ClientData.Money1 + client.ClientData.YinLiang < bhmatchGoldGuessConfig.Cost)
				{
					num = -9;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				if (!Global.SubBindTongQianAndTongQian(client, bhmatchGoldGuessConfig.Cost, "战盟联赛竞猜"))
				{
					num = -9;
					client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						num,
						bhid1,
						bhid2,
						num3
					}), false);
					return true;
				}
				BHMatchSupportData bhmatchSupportData2 = new BHMatchSupportData();
				bhmatchSupportData2.season = this.BHMatchSyncDataCache.CurSeasonID_Gold;
				bhmatchSupportData2.round = goldRound;
				bhmatchSupportData2.bhid1 = bhid1;
				bhmatchSupportData2.bhid2 = bhid2;
				bhmatchSupportData2.guess = num3;
				bhmatchSupportData2.rid = client.ClientData.RoleID;
				client.ClientData.BHMatchSupportList.Add(bhmatchSupportData2);
				Global.SendToDB<BHMatchSupportData>(14021, bhmatchSupportData2, client.ServerId);
				client.sendCmd(nID, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					num,
					bhid1,
					bhid2,
					num3
				}), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchGuessInfo(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				List<BHMatchSupportData4Client> list = new List<BHMatchSupportData4Client>();
				List<BangHuiMatchPKInfo> list2 = null;
				lock (this.RuntimeData.Mutex)
				{
					if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
					{
						list2 = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
					}
				}
				int currentBHMatchGoldRound = this.GetCurrentBHMatchGoldRound();
				using (List<BHMatchSupportData>.Enumerator enumerator = client.ClientData.BHMatchSupportList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BHMatchSupportData item = enumerator.Current;
						if (item.isaward == 0 && (item.season != this.BHMatchSyncDataCache.CurSeasonID_Gold || item.round != currentBHMatchGoldRound))
						{
							BHMatchSupportData4Client bhmatchSupportData4Client = list.Find((BHMatchSupportData4Client x) => x.season == item.season && x.round == item.round);
							if (null == bhmatchSupportData4Client)
							{
								bhmatchSupportData4Client = new BHMatchSupportData4Client();
								bhmatchSupportData4Client.season = item.season;
								bhmatchSupportData4Client.round = item.round;
								list.Add(bhmatchSupportData4Client);
							}
							BHMatchGoldGuessConfig bhmatchGoldGuessConfig;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(item.round, out bhmatchGoldGuessConfig))
								{
									continue;
								}
							}
							BangHuiMatchPKInfo bangHuiMatchPKInfo = list2.Find((BangHuiMatchPKInfo x) => x.season == item.season && (int)x.round == item.round && x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
							if (bangHuiMatchPKInfo != null && (int)bangHuiMatchPKInfo.result == item.guess)
							{
								bhmatchSupportData4Client.right++;
								bhmatchSupportData4Client.jifen += bhmatchGoldGuessConfig.WinAward;
							}
							else
							{
								bhmatchSupportData4Client.jifen += bhmatchGoldGuessConfig.FaillAward;
							}
						}
					}
				}
				list.Sort(delegate(BHMatchSupportData4Client left, BHMatchSupportData4Client right)
				{
					int result;
					if (left.season < right.season)
					{
						result = -1;
					}
					else if (left.season > right.season)
					{
						result = 1;
					}
					else if (left.round < right.round)
					{
						result = -1;
					}
					else if (left.round > right.round)
					{
						result = 1;
					}
					else
					{
						result = 0;
					}
					return result;
				});
				client.sendCmd<List<BHMatchSupportData4Client>>(nID, list, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetBangHuiMatchGuessAward(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				if (!this.RuntimeData.CommonConfigData.CheckOpenState(TimeUtil.NowDateTime()))
				{
					cmdData = -2001;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				List<BangHuiMatchPKInfo> list = null;
				lock (this.RuntimeData.Mutex)
				{
					if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
					{
						list = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
					}
				}
				int num = 0;
				int currentBHMatchGoldRound = this.GetCurrentBHMatchGoldRound();
				using (List<BHMatchSupportData>.Enumerator enumerator = client.ClientData.BHMatchSupportList.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						BHMatchSupportData item = enumerator.Current;
						if (item.isaward == 0 && (item.season != this.BHMatchSyncDataCache.CurSeasonID_Gold || item.round != currentBHMatchGoldRound))
						{
							BHMatchGoldGuessConfig bhmatchGoldGuessConfig;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.BHMatchGoldGuessConfigDict.TryGetValue(item.round, out bhmatchGoldGuessConfig))
								{
									continue;
								}
							}
							BangHuiMatchPKInfo bangHuiMatchPKInfo = list.Find((BangHuiMatchPKInfo x) => x.season == item.season && (int)x.round == item.round && x.bhid1 == item.bhid1 && x.bhid2 == item.bhid2);
							if (bangHuiMatchPKInfo != null && (int)bangHuiMatchPKInfo.result == item.guess)
							{
								num += bhmatchGoldGuessConfig.WinAward;
							}
							else
							{
								num += bhmatchGoldGuessConfig.FaillAward;
							}
							item.isaward = 1;
							Global.SendToDB<BHMatchSupportData>(14021, item, client.ServerId);
						}
					}
				}
				GameManager.ClientMgr.ModifyBHMatchGuessJiFenValue(client, num, "战盟联赛竞猜奖励", true, true, false);
				client.sendCmd<int>(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
		{
			lock (this.RuntimeData.Mutex)
			{
				BangHuiMatchScene bangHuiMatchScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out bangHuiMatchScene))
				{
					gzData.SideName.Add(bangHuiMatchScene.GameStatisticalData.bhname1);
					gzData.SideName.Add(bangHuiMatchScene.GameStatisticalData.bhname2);
					foreach (KeyValuePair<int, BHMatchClientContextData> keyValuePair in bangHuiMatchScene.ClientContextDataDict)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(keyValuePair.Key);
						if (gameClient != null && gameClient.ClientData.HideGM <= 0)
						{
							SceneUIClasses mapSceneType = Global.GetMapSceneType(gameClient.ClientData.MapCode);
							if (45 == mapSceneType)
							{
								List<GuanZhanRoleMiniData> list = null;
								if (!gzData.RoleMiniDataDict.TryGetValue(keyValuePair.Value.BattleWhichSide, out list))
								{
									list = new List<GuanZhanRoleMiniData>();
									gzData.RoleMiniDataDict[keyValuePair.Value.BattleWhichSide] = list;
								}
								GuanZhanRoleMiniData guanZhanRoleMiniData = new GuanZhanRoleMiniData();
								guanZhanRoleMiniData.RoleID = gameClient.ClientData.RoleID;
								guanZhanRoleMiniData.Name = Global.FormatRoleNameWithZoneId2(gameClient);
								guanZhanRoleMiniData.Level = gameClient.ClientData.Level;
								guanZhanRoleMiniData.ChangeLevel = gameClient.ClientData.ChangeLifeCount;
								guanZhanRoleMiniData.Occupation = gameClient.ClientData.Occupation;
								guanZhanRoleMiniData.RoleSex = gameClient.ClientData.RoleSex;
								guanZhanRoleMiniData.BHZhiWu = gameClient.ClientData.BHZhiWu;
								guanZhanRoleMiniData.Param1 = keyValuePair.Value.Kill;
								list.Add(guanZhanRoleMiniData);
							}
						}
					}
					foreach (List<GuanZhanRoleMiniData> list2 in gzData.RoleMiniDataDict.Values)
					{
						list2.Sort(delegate(GuanZhanRoleMiniData left, GuanZhanRoleMiniData right)
						{
							int num = (left.BHZhiWu == 0) ? 5 : left.BHZhiWu;
							int num2 = (right.BHZhiWu == 0) ? 5 : right.BHZhiWu;
							int result;
							if (num < num2)
							{
								result = -1;
							}
							else if (num > num2)
							{
								result = 1;
							}
							else if (left.Param1 > right.Param1)
							{
								result = -1;
							}
							else if (left.Param1 < right.Param1)
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
				}
			}
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == 0;
		}

		private void OnInitGame(GameClient client)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (null != this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold)
				{
					List<BangHuiMatchPKInfo> v = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V;
				}
			}
			List<BHMatchSupportData> list = Global.sendToDB<List<BHMatchSupportData>, string>(14020, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, this.RuntimeData.BHMatchPKInfoMinSeasonID, this.RuntimeData.BHMatchPKInfoMinRound), client.ServerId);
			client.ClientData.BHMatchSupportList.Clear();
			if (list != null)
			{
				client.ClientData.BHMatchSupportList.AddRange(list);
			}
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			BHMatchFuBenData fuBenDataByGameId_BHMatch = TianTiClient.getInstance().GetFuBenDataByGameId_BHMatch((int)kuaFuServerLoginData.GameId);
			bool result;
			if (fuBenDataByGameId_BHMatch == null || fuBenDataByGameId_BHMatch.ServerId != GameManager.ServerId)
			{
				LogManager.WriteLog(2, string.Format("{0}不具有进入跨服地图{1}的资格", kuaFuServerLoginData.RoleId, kuaFuServerLoginData.GameId), null, true);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool OnInitGameKuaFu(GameClient client)
		{
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			BHMatchFuBenData bhmatchFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue((long)((int)clientKuaFuServerLoginData.GameId), out bhmatchFuBenData))
				{
					bhmatchFuBenData = null;
				}
			}
			if (null == bhmatchFuBenData)
			{
				BHMatchFuBenData bhmatchFuBenData2;
				if (client.ClientData.GuanZhanGM > 0)
				{
					bhmatchFuBenData2 = TianTiClient.getInstance().GetFuBenDataByGameId_BHMatch((int)clientKuaFuServerLoginData.GameId);
				}
				else
				{
					bhmatchFuBenData2 = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(client.ClientData.Faction);
				}
				if (bhmatchFuBenData2 == null)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + bhmatchFuBenData2 == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue(clientKuaFuServerLoginData.GameId, out bhmatchFuBenData))
					{
						bhmatchFuBenData = bhmatchFuBenData2;
						bhmatchFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[bhmatchFuBenData.GameId] = bhmatchFuBenData;
					}
				}
			}
			if (bhmatchFuBenData.bhid1 == client.ClientData.Faction)
			{
				client.ClientData.BattleWhichSide = 1;
			}
			else if (bhmatchFuBenData.bhid2 == client.ClientData.Faction)
			{
				client.ClientData.BattleWhichSide = 2;
			}
			else
			{
				client.ClientData.HideGM = 1;
			}
			BHMatchConfig bhmatchConfig;
			lock (this.RuntimeData.Mutex)
			{
				clientKuaFuServerLoginData.FuBenSeqId = bhmatchFuBenData.SequenceId;
				if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue((int)bhmatchFuBenData.Type, out bhmatchConfig))
				{
					return false;
				}
				client.SceneInfoObject = bhmatchConfig;
				client.ClientData.MapCode = bhmatchConfig.MapCode;
			}
			int num;
			int posX;
			int posY;
			bool result;
			if (!this.GetZhanMengBirthPoint(bhmatchConfig, client, client.ClientData.MapCode, out num, out posX, out posY, false))
			{
				LogManager.WriteLog(2, "无法获取有效的阵营和出生点,进入跨服失败,side=" + client.ClientData.BattleWhichSide, null, true);
				result = false;
			}
			else
			{
				client.ClientData.PosX = posX;
				client.ClientData.PosY = posY;
				client.ClientData.FuBenSeqID = clientKuaFuServerLoginData.FuBenSeqId;
				result = true;
			}
			return result;
		}

		public bool GetZhanMengBirthPoint(BHMatchConfig sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY, bool isLogin = false)
		{
			mapCode = sceneInfo.MapCode;
			posX = 0;
			posY = 0;
			double num = 0.0;
			int battleWhichSide = client.ClientData.BattleWhichSide;
			bool result;
			if (client.ClientData.HideGM > 0)
			{
				if (VideoLogic.getInstance().GetGuanZhanPos(toMapCode, ref posX, ref posY))
				{
					mapCode = toMapCode;
				}
				result = true;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					int num2 = 0;
					if (toMapCode == sceneInfo.MapCode_LongTa)
					{
						Point randomPoint;
						for (;;)
						{
							randomPoint = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa);
							if (!Global.InObs(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa, (int)randomPoint.X, (int)randomPoint.Y, 0, 0))
							{
								break;
							}
							if (num2++ >= 1000)
							{
								goto IL_103;
							}
						}
						mapCode = sceneInfo.MapCode_LongTa;
						posX = (int)randomPoint.X;
						posY = (int)randomPoint.Y;
						return true;
					}
					IL_103:
					BHMatchBirthPoint bhmatchBirthPoint = null;
					if (!this.RuntimeData.MapBirthPointDict.TryGetValue(battleWhichSide, out bhmatchBirthPoint))
					{
						return false;
					}
					posX = bhmatchBirthPoint.PosX;
					posY = bhmatchBirthPoint.PosY;
					num = Global.GetTwoPointDistance(new Point((double)posX, (double)posY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
					if (isLogin)
					{
						return true;
					}
				}
				BangHuiMatchScene bangHuiMatchScene = null;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out bangHuiMatchScene))
				{
					foreach (KeyValuePair<int, BHMatchQiZhiConfig> keyValuePair in bangHuiMatchScene.NPCID2QiZhiConfigDict)
					{
						if (keyValuePair.Value.BattleWhichSide == battleWhichSide && keyValuePair.Value.RebirthSiteX != 0 && keyValuePair.Value.RebirthSiteY != 0)
						{
							double twoPointDistance = Global.GetTwoPointDistance(new Point((double)keyValuePair.Value.PosX, (double)keyValuePair.Value.PosY), new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY));
							if (twoPointDistance < num)
							{
								num = twoPointDistance;
								Point mapPointByGridXY = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, keyValuePair.Value.RebirthSiteX / bangHuiMatchScene.MapGridWidth, keyValuePair.Value.RebirthSiteY / bangHuiMatchScene.MapGridHeight, keyValuePair.Value.RebirthRadius / bangHuiMatchScene.MapGridWidth, 0, false);
								posX = (int)mapPointByGridXY.X;
								posY = (int)mapPointByGridXY.Y;
							}
						}
					}
				}
				result = true;
			}
			return result;
		}

		private void HandleNtfEnterEvent(BHMatchNtfEnterData data)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (this.IsGongNengOpened(gameClient, false) && this.CheckMap(gameClient))
				{
					if (gameClient != null && (data.bhid1 == gameClient.ClientData.Faction || data.bhid2 == gameClient.ClientData.Faction))
					{
						gameClient.sendCmd<int>(1171, 1, false);
					}
				}
			}
			LogManager.WriteLog(2, string.Format("通知战盟ID={0}，ID={1}拥有进入战盟联赛资格", data.bhid1, data.bhid2), null, true);
		}

		public void SwitchLastGoldBH_GM()
		{
			TianTiClient.getInstance().SwitchLastGoldBH_GM();
		}

		private void TimerProc(object sender, EventArgs e)
		{
			lock (this.RuntimeData.Mutex)
			{
				BHMatchSyncData bhmatchSyncData = TianTiClient.getInstance().SyncData_BHMatch(this.BHMatchSyncDataCache.BHMatchRankInfoDict.Age, this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.Age, this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Age);
				this.BHMatchSyncDataCache.LastSeasonID_Gold = bhmatchSyncData.LastSeasonID_Gold;
				this.BHMatchSyncDataCache.CurSeasonID_Gold = bhmatchSyncData.CurSeasonID_Gold;
				this.BHMatchSyncDataCache.LastSeasonID_Rookie = bhmatchSyncData.LastSeasonID_Rookie;
				this.BHMatchSyncDataCache.CurSeasonID_Rookie = bhmatchSyncData.CurSeasonID_Rookie;
				this.BHMatchSyncDataCache.RoundGoldReal = bhmatchSyncData.RoundGoldReal;
				this.BHMatchSyncDataCache.RoundRookieReal = bhmatchSyncData.RoundRookieReal;
				if (this.BHMatchSyncDataCache.BHMatchRankInfoDict.Age != bhmatchSyncData.BHMatchRankInfoDict.Age)
				{
					this.BHMatchSyncDataCache.BHMatchRankInfoDict = bhmatchSyncData.BHMatchRankInfoDict;
					bool flag2 = false;
					flag2 |= this.RefreshBHMatchChampionBH(1);
					flag2 |= this.RefreshBHMatchChampionBH(0);
					if (flag2)
					{
						int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
						for (int i = 0; i < maxClientCount; i++)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
							if (null != gameClient)
							{
								this.UpdateChengHaoBuffer(gameClient);
							}
						}
					}
					this.SaveBHMatchBHListGoldJoin();
				}
				if (this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.Age != bhmatchSyncData.BHMatchPKInfoList_Gold.Age)
				{
					this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold = bhmatchSyncData.BHMatchPKInfoList_Gold;
					int goldRound = this.GetCurrentBHMatchGoldRound();
					List<BangHuiMatchPKInfo> list = this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V.FindAll((BangHuiMatchPKInfo x) => x.season == this.BHMatchSyncDataCache.CurSeasonID_Gold && (int)x.round == goldRound);
					if (null != list)
					{
						for (int i = 0; i < list.Count; i++)
						{
							BangHuiMatchPKInfo pkInfo = list[i];
							BHMatchBHData bhdataByBhid_BHMatch = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, pkInfo.bhid1);
							BHMatchBHData bhdataByBhid_BHMatch2 = TianTiClient.getInstance().GetBHDataByBhid_BHMatch(1, pkInfo.bhid2);
							List<BangHuiMatchRankInfo> list2 = new List<BangHuiMatchRankInfo>();
							if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
							{
								this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(8, out list2);
							}
							if (null != bhdataByBhid_BHMatch)
							{
								pkInfo.win1 = (byte)bhdataByBhid_BHMatch.cur_win;
								pkInfo.winpct1 = ((goldRound == 1) ? 0 : ((byte)((double)bhdataByBhid_BHMatch.cur_win / (double)(goldRound - 1) * 100.0)));
								pkInfo.rank1 = (byte)(list2.FindIndex((BangHuiMatchRankInfo x) => x.Key == pkInfo.bhid1) + 1);
							}
							if (null != bhdataByBhid_BHMatch2)
							{
								pkInfo.win2 = (byte)bhdataByBhid_BHMatch2.cur_win;
								pkInfo.winpct2 = ((goldRound == 1) ? 0 : ((byte)((double)bhdataByBhid_BHMatch2.cur_win / (double)(goldRound - 1) * 100.0)));
								pkInfo.rank2 = (byte)(list2.FindIndex((BangHuiMatchRankInfo x) => x.Key == pkInfo.bhid2) + 1);
							}
						}
					}
					foreach (BangHuiMatchPKInfo bangHuiMatchPKInfo in this.BHMatchSyncDataCache.BHMatchPKInfoList_Gold.V)
					{
						if (bangHuiMatchPKInfo.season < this.RuntimeData.BHMatchPKInfoMinSeasonID)
						{
							this.RuntimeData.BHMatchPKInfoMinSeasonID = bangHuiMatchPKInfo.season;
							this.RuntimeData.BHMatchPKInfoMinRound = (int)bangHuiMatchPKInfo.round;
						}
						else if (bangHuiMatchPKInfo.season == this.RuntimeData.BHMatchPKInfoMinSeasonID && (int)bangHuiMatchPKInfo.round < this.RuntimeData.BHMatchPKInfoMinRound)
						{
							this.RuntimeData.BHMatchPKInfoMinSeasonID = bangHuiMatchPKInfo.season;
							this.RuntimeData.BHMatchPKInfoMinRound = (int)bangHuiMatchPKInfo.round;
						}
					}
				}
				if (this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Age != bhmatchSyncData.BHMatchChampionRoleData_Gold.Age)
				{
					this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold = bhmatchSyncData.BHMatchChampionRoleData_Gold;
					if (null != this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0)
					{
						this.OwnerRoleData = DataHelper.BytesToObject<RoleDataEx>(this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0, 0, this.BHMatchSyncDataCache.BHMatchChampionRoleData_Gold.Bytes0.Length);
					}
					else
					{
						this.OwnerRoleData = null;
					}
					this.ReplaceBangHuiMatchNpc();
				}
			}
		}

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return !GameFuncControlManager.IsGameFuncDisabled(15) && GlobalNew.IsGongNengOpened(client, 91, hint);
		}

		private void NtfCanGetAward(GameClient client, int success, BHMatchConfig sceneInfo, int mvprid, string mvpname, int mvpocc, int mvpsex, string sucessBHName = "")
		{
			long num = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			int num2 = sceneInfo.BandJinBi;
			List<AwardsItemData> items;
			if (success > 0)
			{
				items = (sceneInfo.WinAwardTag as AwardsItemList).Items;
			}
			else
			{
				num = (long)((double)num * 0.8);
				num2 = (int)((double)num2 * 0.8);
				items = (sceneInfo.FailAwardTag as AwardsItemList).Items;
			}
			num -= num % 10000L;
			num2 -= num2 % 10000;
			client.sendCmd<BangHuiMatchAwardsData>(1174, new BangHuiMatchAwardsData
			{
				Exp = num,
				BindJinBi = num2,
				Success = success,
				AwardsItemDataList = items,
				SuccessBHName = sucessBHName,
				MvpRoleName = mvpname,
				MvpOccupation = mvpocc,
				MvpRoleSex = mvpsex
			}, false);
		}

		private int GiveRoleAwards(GameClient client, int success, BHMatchConfig sceneInfo)
		{
			long num = Global.GetExpMultiByZhuanShengExpXiShu(client, sceneInfo.Exp);
			int num2 = sceneInfo.BandJinBi;
			List<AwardsItemData> items;
			if (success > 0)
			{
				items = (sceneInfo.WinAwardTag as AwardsItemList).Items;
			}
			else
			{
				num = (long)((double)num * 0.8);
				num2 = (int)((double)num2 * 0.8);
				items = (sceneInfo.FailAwardTag as AwardsItemList).Items;
			}
			num -= num % 10000L;
			num2 -= num2 % 10000;
			int result;
			if (items != null && !Global.CanAddGoodsNum(client, items.Count))
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
					GameManager.ClientMgr.AddMoney1(client, num2, "战盟联赛奖励", true);
				}
				if (items != null)
				{
					foreach (AwardsItemData awardsItemData in items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "战盟联赛奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				result = 1;
			}
			return result;
		}

		public void GiveAwards(BangHuiMatchScene scene)
		{
			try
			{
				foreach (BHMatchClientContextData bhmatchClientContextData in scene.ClientContextDataDict.Values)
				{
					int num;
					if (bhmatchClientContextData.BattleWhichSide == scene.SuccessSide)
					{
						num = 1;
					}
					else
					{
						num = 0;
					}
					GameClient gameClient = GameManager.ClientMgr.FindClient(bhmatchClientContextData.RoleId);
					if (0 == bhmatchClientContextData.BattleWhichSide)
					{
						if (null != gameClient)
						{
							string sucessBHName = (scene.SuccessSide == 1) ? scene.GameStatisticalData.bhname1 : scene.GameStatisticalData.bhname2;
							this.NtfCanGetAward(gameClient, 1, scene.SceneInfo, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex, sucessBHName);
						}
					}
					else
					{
						string text = string.Format("{0}&{1}&{2}&{3}&{4}&{5}", new object[]
						{
							scene.SceneInfo.ID,
							num,
							scene.ClientContextMVP.RoleId,
							scene.ClientContextMVP.RoleName,
							scene.ClientContextMVP.Occupation,
							scene.ClientContextMVP.RoleSex
						});
						byte[] bytes = new UTF8Encoding().GetBytes(text);
						text = Convert.ToBase64String(bytes);
						if (gameClient != null)
						{
							int totalScore = bhmatchClientContextData.TotalScore;
							bhmatchClientContextData.TotalScore = 0;
							Global.SaveRoleParamsStringToDB(gameClient, "39", text, true);
							this.NtfCanGetAward(gameClient, num, scene.SceneInfo, scene.ClientContextMVP.RoleId, scene.ClientContextMVP.RoleName, scene.ClientContextMVP.Occupation, scene.ClientContextMVP.RoleSex, "");
						}
						else
						{
							Global.UpdateRoleParamByNameOffline(bhmatchClientContextData.RoleId, "39", text, bhmatchClientContextData.ServerId);
						}
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "战盟联赛系统清场调度异常");
			}
		}

		public void PushGameResultData(BangHuiMatchScene scene)
		{
			BHMatchFuBenData bhmatchFuBenData;
			if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out bhmatchFuBenData))
			{
				scene.GameStatisticalData.bhid1 = bhmatchFuBenData.bhid1;
				scene.GameStatisticalData.bhid2 = bhmatchFuBenData.bhid2;
				scene.GameStatisticalData.result = (byte)scene.SuccessSide;
				int num = scene.ScoreData.Score1 + scene.ScoreData.Score2;
				int num2 = (num == 0) ? 0 : ((int)((double)scene.ScoreData.Score1 / (double)num * 100.0));
				int num3 = (num == 0) ? 0 : ((int)((double)scene.ScoreData.Score2 / (double)num * 100.0));
				if (1 == scene.SuccessSide)
				{
					scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieWinScoreFactor + num2;
					scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + num3;
				}
				else if (2 == scene.SuccessSide)
				{
					scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + num2;
					scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieWinScoreFactor + num3;
				}
				else
				{
					scene.GameStatisticalData.score1 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + num2;
					scene.GameStatisticalData.score2 = this.RuntimeData.CommonConfigData.RookieLoseScoreFactor + num3;
				}
				foreach (BHMatchClientContextData bhmatchClientContextData in scene.ClientContextDataDict.Values)
				{
					if (0 != bhmatchClientContextData.BattleWhichSide)
					{
						if (1 == bhmatchClientContextData.BattleWhichSide)
						{
							scene.GameStatisticalData.rolecount1++;
						}
						else
						{
							scene.GameStatisticalData.rolecount2++;
						}
						BHMatchRoleData bhmatchRoleData = new BHMatchRoleData();
						bhmatchRoleData.rid = bhmatchClientContextData.RoleId;
						bhmatchRoleData.rname = bhmatchClientContextData.RoleName;
						bhmatchRoleData.zoneid = bhmatchClientContextData.ZoneID;
						bhmatchRoleData.kill = bhmatchClientContextData.Kill;
						bhmatchRoleData.mvp = ((bhmatchClientContextData.RoleId == scene.ClientContextMVP.RoleId) ? 1 : 0);
						bhmatchRoleData.bhid = ((bhmatchClientContextData.BattleWhichSide == 1) ? bhmatchFuBenData.bhid1 : bhmatchFuBenData.bhid2);
						if (bhmatchClientContextData.Kill != 0 || object.ReferenceEquals(bhmatchClientContextData, scene.ClientContextMVP))
						{
							scene.GameStatisticalData.roleStatisticalData[bhmatchRoleData.rid] = bhmatchRoleData;
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(bhmatchClientContextData.RoleId);
						List<int> bhmatchRoleAnalysisData;
						if (gameClient != null)
						{
							bhmatchRoleAnalysisData = this.GetBHMatchRoleAnalysisData(gameClient);
						}
						else
						{
							bhmatchRoleAnalysisData = this.GetBHMatchRoleAnalysisData(bhmatchClientContextData.RoleId, bhmatchClientContextData.ServerId);
						}
						if (null != bhmatchRoleAnalysisData)
						{
							List<int> list;
							(list = bhmatchRoleAnalysisData)[8] = list[8] + bhmatchClientContextData.Kill;
							(list = bhmatchRoleAnalysisData)[9] = list[9] + bhmatchClientContextData.Kill;
							(list = bhmatchRoleAnalysisData)[11] = list[11] + 1;
							(list = bhmatchRoleAnalysisData)[10] = list[10] + ((bhmatchClientContextData.BattleWhichSide == scene.SuccessSide) ? 1 : 0);
							if (bhmatchClientContextData.RoleId == scene.ClientContextMVP.RoleId)
							{
								if (bhmatchFuBenData.Type == 1)
								{
									(list = bhmatchRoleAnalysisData)[3] = list[3] + 1;
									(list = bhmatchRoleAnalysisData)[2] = list[2] + 1;
								}
								else
								{
									(list = bhmatchRoleAnalysisData)[6] = list[6] + 1;
									(list = bhmatchRoleAnalysisData)[5] = list[5] + 1;
								}
							}
						}
						if (gameClient != null)
						{
							this.SaveBHMatchRoleAnalysisData(gameClient, bhmatchRoleAnalysisData);
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_BHMatchJoin, new int[0]));
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_BHMatchGoldMVP, new int[0]));
							GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(gameClient, OrnamentGoalType.OGT_BHMatchWin, new int[0]));
						}
						else
						{
							this.SaveBHMatchRoleAnalysisDataOffline(bhmatchClientContextData.RoleId, bhmatchRoleAnalysisData, bhmatchClientContextData.ServerId);
						}
					}
				}
				RoleDataEx bhmatchBZRoleData = this.GetBHMatchBZRoleData(scene.GameStatisticalData.bhid1, scene.GameStatisticalData.serverid1);
				if (null != bhmatchBZRoleData)
				{
					scene.GameStatisticalData.rid1 = bhmatchBZRoleData.RoleID;
					scene.GameStatisticalData.rname1 = bhmatchBZRoleData.RoleName;
					scene.GameStatisticalData.zoneid_r1 = bhmatchBZRoleData.ZoneID;
					if (bhmatchFuBenData.Type == 1)
					{
						scene.GameStatisticalData.bzroledata1 = DataHelper.ObjectToBytes<RoleDataEx>(bhmatchBZRoleData);
					}
				}
				RoleDataEx bhmatchBZRoleData2 = this.GetBHMatchBZRoleData(scene.GameStatisticalData.bhid2, scene.GameStatisticalData.serverid2);
				if (null != bhmatchBZRoleData2)
				{
					scene.GameStatisticalData.rid2 = bhmatchBZRoleData2.RoleID;
					scene.GameStatisticalData.rname2 = bhmatchBZRoleData2.RoleName;
					scene.GameStatisticalData.zoneid_r2 = bhmatchBZRoleData2.ZoneID;
					if (bhmatchFuBenData.Type == 1)
					{
						scene.GameStatisticalData.bzroledata2 = DataHelper.ObjectToBytes<RoleDataEx>(bhmatchBZRoleData2);
					}
				}
				TianTiClient.getInstance().GameFuBenComplete_BHMatch(scene.GameStatisticalData);
			}
		}

		private RoleDataEx GetBHMatchBZRoleData(int bhid, int serverid)
		{
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, bhid, serverid);
			RoleDataEx result;
			if (null == bangHuiDetailData)
			{
				LogManager.WriteLog(1000, string.Format("无法获取帮会详细信息 BangHuiID={0} ServerID={1}", bhid, serverid), null, true);
				result = null;
			}
			else
			{
				RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, bangHuiDetailData.BZRoleID), serverid);
				if (roleDataEx == null || roleDataEx.RoleID <= 0)
				{
					LogManager.WriteLog(1000, string.Format("无法获取帮主详细信息 BangHuiID={0} BZRoleID={1} ServerID={2}", bhid, bangHuiDetailData.BZRoleID, serverid), null, true);
					result = null;
				}
				else
				{
					result = roleDataEx;
				}
			}
			return result;
		}

		public List<int> GetBHMatchRoleAnalysisData(int rid, int serverid)
		{
			List<int> result;
			if (0 == this.BHMatchSyncDataCache.CurSeasonID_Gold)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDBOffline = Global.GetRoleParamsIntListFromDBOffline(rid, "40", serverid);
				this.FilterBHMatchRoleAnalysisData(roleParamsIntListFromDBOffline);
				result = roleParamsIntListFromDBOffline;
			}
			return result;
		}

		public List<int> GetBHMatchRoleAnalysisData(GameClient client)
		{
			List<int> result;
			if (0 == this.BHMatchSyncDataCache.CurSeasonID_Gold)
			{
				result = null;
			}
			else
			{
				List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "40");
				this.FilterBHMatchRoleAnalysisData(roleParamsIntListFromDB);
				result = roleParamsIntListFromDB;
			}
			return result;
		}

		public List<int> GetBHMatchRAnalysisExData(GameClient client)
		{
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "41");
			if (roleParamsIntListFromDB.Count != 4)
			{
				for (int i = roleParamsIntListFromDB.Count; i < 4; i++)
				{
					roleParamsIntListFromDB.Add(0);
				}
			}
			return roleParamsIntListFromDB;
		}

		private void FilterBHMatchRoleAnalysisData(List<int> countList)
		{
			if (countList.Count != 14)
			{
				for (int i = countList.Count; i < 14; i++)
				{
					countList.Add(0);
				}
			}
			if (this.BHMatchSyncDataCache.CurSeasonID_Gold != countList[0])
			{
				countList[1] = countList[0];
				countList[0] = this.BHMatchSyncDataCache.CurSeasonID_Gold;
				countList[4] = countList[3];
				countList[3] = 0;
				countList[8] = 0;
			}
			if (this.BHMatchSyncDataCache.CurSeasonID_Rookie != countList[0])
			{
				countList[13] = countList[12];
				countList[12] = this.BHMatchSyncDataCache.CurSeasonID_Rookie;
				countList[7] = countList[6];
				countList[6] = 0;
				countList[8] = 0;
			}
			if (this.BHMatchSyncDataCache.LastSeasonID_Gold != countList[1])
			{
				countList[1] = this.BHMatchSyncDataCache.LastSeasonID_Gold;
				countList[4] = 0;
			}
			if (this.BHMatchSyncDataCache.LastSeasonID_Rookie != countList[13])
			{
				countList[13] = this.BHMatchSyncDataCache.LastSeasonID_Rookie;
				countList[7] = 0;
			}
		}

		private void SaveBHMatchRoleAnalysisDataOffline(int rid, List<int> countList, int serverid)
		{
			Global.SaveRoleParamsIntListToDBOffline(rid, countList, "40", serverid);
		}

		private void SaveBHMatchRoleAnalysisData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "40", true);
		}

		private void SaveBHMatchRAnalysisExData(GameClient client, List<int> countList)
		{
			Global.SaveRoleParamsIntListToDB(client, countList, "41", true);
		}

		public void RestoreBangHuiMatchNpc()
		{
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 141);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.BangHuiMatchBZ, false);
			}
		}

		public void ReplaceBangHuiMatchNpc()
		{
			if (null == this.OwnerRoleData)
			{
				this.RestoreBangHuiMatchNpc();
			}
			else
			{
				NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 141);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.BangHuiMatchBZ, false);
					FakeRoleManager.ProcessNewFakeRole(new SafeClientData
					{
						RoleData = this.OwnerRoleData
					}, npc.MapCode, FakeRoleTypes.BangHuiMatchBZ, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 141);
				}
			}
		}

		private void UpdateChengHaoBuffer(GameClient client)
		{
			if (this.RuntimeData.ChengHaoBHid_Rookie > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Rookie)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_RookieChengHao, actionParams, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_RookieChengHao, actionParams, 1, true);
				}
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_RookieChengHao, actionParams, 1, true);
				Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_RookieChengHao, actionParams, 1, true);
			}
			if (this.RuntimeData.ChengHaoBHid_Gold > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Gold)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_GoldChengHao, actionParams, 1, true);
					FashionManager.getInstance().GetFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_GoldChengHao, actionParams, 1, true);
					FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi);
				}
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchBZ_GoldChengHao, actionParams, 1, true);
				Global.UpdateBufferData(client, BufferItemTypes.BangHuiMatchCY_GoldChengHao, actionParams, 1, true);
				FashionManager.getInstance().DelFashionByMagic(client, FashionIdConsts.BangHuiMatchYuYi);
			}
		}

		public void SaveBHMatchBHListGoldJoin()
		{
			List<BangHuiMatchRankInfo> list = new List<BangHuiMatchRankInfo>();
			if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
			{
				this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(8, out list);
			}
			string text = "";
			int num = 0;
			while (list != null && num < list.Count)
			{
				text += string.Format("{0}|", list[num].Key);
				num++;
			}
			if (!string.IsNullOrEmpty(text) && text.Substring(text.Length - 1) == "|")
			{
				text = text.Substring(0, text.Length - 1);
			}
			GameManager.GameConfigMgr.UpdateGameConfigItem("bhmatch_goldjoin", text, true);
		}

		public bool RefreshBHMatchChampionBH(int rankType)
		{
			bool result = false;
			int num = 0;
			List<BangHuiMatchRankInfo> list = new List<BangHuiMatchRankInfo>();
			if (null != this.BHMatchSyncDataCache.BHMatchRankInfoDict)
			{
				this.BHMatchSyncDataCache.BHMatchRankInfoDict.V.TryGetValue(rankType, out list);
			}
			if (list != null && list.Count != 0)
			{
				num = list[0].Key;
			}
			if (rankType == 1)
			{
				if (this.RuntimeData.ChengHaoBHid_Rookie != (long)num)
				{
					result = true;
				}
				this.RuntimeData.ChengHaoBHid_Rookie = (long)num;
			}
			if (rankType == 0)
			{
				if (this.RuntimeData.ChengHaoBHid_Gold != (long)num)
				{
					result = true;
				}
				this.RuntimeData.ChengHaoBHid_Gold = (long)num;
			}
			return result;
		}

		public bool PreRemoveBangHui(GameClient client)
		{
			string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(client.ClientData.Faction);
			bool result;
			if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
			{
				result = true;
			}
			else
			{
				string[] array = kuaFuGameState_BHMatch.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					result = true;
				}
				else
				{
					int num = Global.SafeConvertToInt32(array[0]);
					int num2 = Global.SafeConvertToInt32(array[1]);
					result = (num != 1);
				}
			}
			return result;
		}

		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			BHMatchFuBenData fuBenDataByBhid_BHMatch = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(e.BHID);
			string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(e.BHID);
			bool result;
			if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
			{
				result = false;
			}
			else
			{
				string[] array = kuaFuGameState_BHMatch.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					result = false;
				}
				else
				{
					int matchType = Global.SafeConvertToInt32(array[0]);
					int num = Global.SafeConvertToInt32(array[1]);
					BHMatchConfig bhmatchConfig = null;
					BangHuiMatchGameStates bangHuiMatchGameStates = 0;
					this.CheckCondition(e.Player, matchType, ref bhmatchConfig, ref bangHuiMatchGameStates);
					if (bangHuiMatchGameStates == 3 && null != fuBenDataByBhid_BHMatch)
					{
						e.Result = false;
						GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2601, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			BHMatchFuBenData fuBenDataByBhid_BHMatch = TianTiClient.getInstance().GetFuBenDataByBhid_BHMatch(e.BHID);
			string kuaFuGameState_BHMatch = TianTiClient.getInstance().GetKuaFuGameState_BHMatch(e.BHID);
			bool result;
			if (string.IsNullOrEmpty(kuaFuGameState_BHMatch))
			{
				result = false;
			}
			else
			{
				string[] array = kuaFuGameState_BHMatch.Split(new char[]
				{
					':'
				});
				if (array.Length != 2)
				{
					result = false;
				}
				else
				{
					int matchType = Global.SafeConvertToInt32(array[0]);
					int num = Global.SafeConvertToInt32(array[1]);
					BHMatchConfig bhmatchConfig = null;
					BangHuiMatchGameStates bangHuiMatchGameStates = 0;
					this.CheckCondition(e.Player, matchType, ref bhmatchConfig, ref bangHuiMatchGameStates);
					if (bangHuiMatchGameStates == 3 && null != fuBenDataByBhid_BHMatch)
					{
						e.Result = false;
						GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(2601, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		private void InitScene(BangHuiMatchScene scene, GameClient client)
		{
			foreach (BHMatchQiZhiConfig bhmatchQiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(bhmatchQiZhiConfig.NPCID, bhmatchQiZhiConfig.Clone() as BHMatchQiZhiConfig);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 45)
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
					long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					lock (this.RuntimeData.Mutex)
					{
						BangHuiMatchScene bangHuiMatchScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out bangHuiMatchScene))
						{
							BHMatchConfig bhmatchConfig = null;
							BHMatchFuBenData bhmatchFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out bhmatchFuBenData))
							{
								LogManager.WriteLog(2, "战盟联赛没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue((int)bhmatchFuBenData.Type, out bhmatchConfig))
							{
								LogManager.WriteLog(2, "战盟联赛没有为副本找到对应的场景数据,ID:" + bhmatchFuBenData.Type, null, true);
							}
							bangHuiMatchScene = new BangHuiMatchScene();
							bangHuiMatchScene.CleanAllInfo();
							bangHuiMatchScene.GameId = (int)gameId;
							bangHuiMatchScene.FuBenSeqId = fuBenSeqID;
							bangHuiMatchScene.SceneInfo = bhmatchConfig;
							bangHuiMatchScene.MapGridWidth = gameMap.MapGridWidth;
							bangHuiMatchScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(bhmatchConfig.ID));
							bangHuiMatchScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							this.InitScene(bangHuiMatchScene, client);
							bangHuiMatchScene.GameStatisticalData.GameId = (int)gameId;
							this.SceneDict[fuBenSeqID] = bangHuiMatchScene;
						}
						bangHuiMatchScene.CopyMapDict[mapCode] = copyMap;
						BHMatchClientContextData bhmatchClientContextData;
						if (!bangHuiMatchScene.ClientContextDataDict.TryGetValue(roleID, out bhmatchClientContextData))
						{
							bhmatchClientContextData = new BHMatchClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID
							};
							bangHuiMatchScene.ClientContextDataDict[roleID] = bhmatchClientContextData;
						}
						if (client.ClientData.BattleWhichSide == 1)
						{
							bangHuiMatchScene.GameStatisticalData.bhname1 = client.ClientData.BHName;
							bangHuiMatchScene.GameStatisticalData.serverid1 = client.ServerId;
						}
						if (client.ClientData.BattleWhichSide == 2)
						{
							bangHuiMatchScene.GameStatisticalData.bhname2 = client.ClientData.BHName;
							bangHuiMatchScene.GameStatisticalData.serverid2 = client.ServerId;
						}
						client.SceneObject = bangHuiMatchScene;
						client.SceneGameId = (long)bangHuiMatchScene.GameId;
						client.SceneContextData2 = bhmatchClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(bangHuiMatchScene.SceneInfo.TotalSecs * 1000));
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
			if (sceneType == 45)
			{
				lock (this.RuntimeData.Mutex)
				{
					BangHuiMatchScene bangHuiMatchScene;
					this.SceneDict.TryRemove(copyMap.FuBenSeqID, out bangHuiMatchScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private int CheckCondition(GameClient client, int matchType, ref BHMatchConfig sceneItem, ref BangHuiMatchGameStates state)
		{
			int result = 0;
			sceneItem = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(matchType, out sceneItem))
				{
					return -12;
				}
			}
			state = 1;
			result = 0;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)sceneItem.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= sceneItem.SecondsOfDay[i] - (double)sceneItem.ApplyOverTime && dateTime.TimeOfDay.TotalSeconds <= sceneItem.SecondsOfDay[i + 1] + (double)sceneItem.ApplyStartTime)
					{
						if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i])
						{
							state = 2;
							result = 0;
						}
						else if (dateTime.TimeOfDay.TotalSeconds < sceneItem.SecondsOfDay[i + 1])
						{
							state = 3;
							result = 0;
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
			return result;
		}

		private TimeSpan GetStartTime(int sceneId)
		{
			BHMatchConfig bhmatchConfig = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CommonConfigData.BHMatchConfigDict.TryGetValue(sceneId, out bhmatchConfig))
				{
					goto IL_158;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.DayOfWeek == (DayOfWeek)bhmatchConfig.TimePoints[i].Days && dateTime.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i] - (double)bhmatchConfig.ApplyOverTime && dateTime.TimeOfDay.TotalSeconds <= bhmatchConfig.SecondsOfDay[i + 1])
					{
						timeSpan = TimeSpan.FromSeconds(bhmatchConfig.SecondsOfDay[i]);
						break;
					}
				}
			}
			IL_158:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		public bool ClientRelive(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			BHMatchConfig bhmatchConfig = client.SceneInfoObject as BHMatchConfig;
			if (null != bhmatchConfig)
			{
				int mapCode2;
				int num;
				int num2;
				if (this.GetZhanMengBirthPoint(bhmatchConfig, client, mapCode2 = bhmatchConfig.MapCode, out mapCode2, out num, out num2, false))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.MoveAndActionNum = 0;
					GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, num, num2, -1);
					if (mapCode2 != client.ClientData.MapCode)
					{
						GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, -1);
						client.ClientData.KuaFuChangeMapCode = mapCode2;
						GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, mapCode2, num, num2, -1, 1);
					}
					else
					{
						Global.ClientRealive(client, num, num2, -1);
					}
					return true;
				}
			}
			return false;
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (client != null && this.IsQiZhiExtensionID(monster.MonsterInfo.ExtensionID))
			{
				BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
				BHMatchQiZhiConfig bhmatchQiZhiConfig = monster.Tag as BHMatchQiZhiConfig;
				if (bangHuiMatchScene != null && null != bhmatchQiZhiConfig)
				{
					lock (this.RuntimeData.Mutex)
					{
						bhmatchQiZhiConfig.DeadTicks = TimeUtil.NOW();
						bhmatchQiZhiConfig.Alive = false;
						bhmatchQiZhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
						bhmatchQiZhiConfig.OwnTicks = 0L;
						bhmatchQiZhiConfig.OwnTicksDelta = 0L;
						this.UpdateQiZhiBangHuiOwnNum(bangHuiMatchScene);
						foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, bangHuiMatchScene.ScoreData, copyMap);
						}
					}
				}
			}
		}

		private void ProcessEnd(BangHuiMatchScene scene, int successSide, long nowTicks)
		{
			if (successSide != 0)
			{
				List<BHMatchClientContextData> list = new List<BHMatchClientContextData>();
				foreach (BHMatchClientContextData bhmatchClientContextData in scene.ClientContextDataDict.Values)
				{
					if (bhmatchClientContextData.BattleWhichSide == successSide)
					{
						list.Add(bhmatchClientContextData);
					}
				}
				list.Sort(delegate(BHMatchClientContextData left, BHMatchClientContextData right)
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
			scene.StateTimeData.GameType = 24;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, copyMap);
			}
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= BangHuiMatchManager.NextHeartBeatTicks)
			{
				BangHuiMatchManager.NextHeartBeatTicks = num + 1020L;
				foreach (BangHuiMatchScene bangHuiMatchScene in this.SceneDict.Values)
				{
					lock (this.RuntimeData.Mutex)
					{
						int fuBenSeqId = bangHuiMatchScene.FuBenSeqId;
						if (fuBenSeqId >= 0)
						{
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							if (bangHuiMatchScene.m_eStatus == 0)
							{
								if (num2 >= bangHuiMatchScene.StartTimeTicks)
								{
									bangHuiMatchScene.m_lPrepareTime = bangHuiMatchScene.StartTimeTicks;
									bangHuiMatchScene.m_lBeginTime = bangHuiMatchScene.m_lPrepareTime + (long)(bangHuiMatchScene.SceneInfo.PrepareSecs * 1000);
									bangHuiMatchScene.m_eStatus = 1;
									bangHuiMatchScene.StateTimeData.GameType = 24;
									bangHuiMatchScene.StateTimeData.State = bangHuiMatchScene.m_eStatus;
									bangHuiMatchScene.StateTimeData.EndTicks = bangHuiMatchScene.m_lBeginTime;
									foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, bangHuiMatchScene.StateTimeData, copyMap);
									}
								}
							}
							else if (bangHuiMatchScene.m_eStatus == 1)
							{
								if (num2 >= bangHuiMatchScene.m_lBeginTime)
								{
									bangHuiMatchScene.m_eStatus = 2;
									bangHuiMatchScene.m_lEndTime = bangHuiMatchScene.m_lBeginTime + (long)(bangHuiMatchScene.SceneInfo.FightingSecs * 1000);
									bangHuiMatchScene.StateTimeData.GameType = 24;
									bangHuiMatchScene.StateTimeData.State = bangHuiMatchScene.m_eStatus;
									bangHuiMatchScene.StateTimeData.EndTicks = bangHuiMatchScene.m_lEndTime;
									foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
									{
										GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, bangHuiMatchScene.StateTimeData, copyMap);
									}
									foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
									{
										if (copyMap.MapCode == bangHuiMatchScene.SceneInfo.MapCode)
										{
											for (int i = 1; i <= 2; i++)
											{
												GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
											}
										}
									}
								}
							}
							else if (bangHuiMatchScene.m_eStatus == 2)
							{
								if (num2 >= bangHuiMatchScene.m_lEndTime)
								{
									int num3 = 0;
									if (bangHuiMatchScene.ScoreData.Score1 > bangHuiMatchScene.ScoreData.Score2)
									{
										num3 = 1;
									}
									else if (bangHuiMatchScene.ScoreData.Score2 > bangHuiMatchScene.ScoreData.Score1)
									{
										num3 = 2;
									}
									if (0 != bangHuiMatchScene.LT_BattleWhichSide)
									{
										if (num3 != 0 && num3 != bangHuiMatchScene.LT_BattleWhichSide)
										{
											bangHuiMatchScene.GameStatisticalData.bullshit = true;
										}
										num3 = bangHuiMatchScene.LT_BattleWhichSide;
									}
									this.ProcessEnd(bangHuiMatchScene, num3, num2);
								}
								else
								{
									this.CheckSceneScoreTime(bangHuiMatchScene, num2);
									this.CheckSceneTempleDamage(bangHuiMatchScene, num2);
								}
							}
							else if (bangHuiMatchScene.m_eStatus == 3)
							{
								bangHuiMatchScene.m_eStatus = 4;
								this.GiveAwards(bangHuiMatchScene);
								foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
								{
									GameManager.CopyMapMgr.KillAllMonster(copyMap);
								}
								BHMatchFuBenData bhmatchFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue((long)bangHuiMatchScene.GameId, out bhmatchFuBenData))
								{
									LogManager.WriteLog(2, string.Format("战盟联赛跨服副本GameID={0},战斗结束", bhmatchFuBenData.GameId), null, true);
									this.RuntimeData.FuBenItemData.Remove((long)bangHuiMatchScene.GameId);
								}
							}
							else if (bangHuiMatchScene.m_eStatus == 4)
							{
								if (num2 >= bangHuiMatchScene.m_lLeaveTime)
								{
									foreach (CopyMap copyMap in bangHuiMatchScene.CopyMapDict.Values)
									{
										copyMap.SetRemoveTicks(bangHuiMatchScene.m_lLeaveTime);
										bangHuiMatchScene.m_eStatus = 5;
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
											DataHelper.WriteExceptionLogEx(ex, "战盟联赛系统清场调度异常");
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void CompleteScene(BangHuiMatchScene scene, int successSide)
		{
			scene.SuccessSide = successSide;
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				BangHuiMatchScene bangHuiMatchScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out bangHuiMatchScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, bangHuiMatchScene.StateTimeData, false);
					}
					if (sideScore)
					{
						client.sendCmd<BangHuiMatchScoreData>(1173, bangHuiMatchScene.ScoreData, false);
					}
				}
			}
		}

		public void RemoveBattleSceneBuffForRole(BangHuiMatchScene scene, GameClient client)
		{
			this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, 0);
			this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
				if (bangHuiMatchScene != null && bangHuiMatchScene.m_eStatus == 2)
				{
					if (client.ClientData.BattleWhichSide == 1)
					{
						bangHuiMatchScene.GameStatisticalData.kill1++;
					}
					if (client.ClientData.BattleWhichSide == 2)
					{
						bangHuiMatchScene.GameStatisticalData.kill2++;
					}
					BHMatchClientContextData bhmatchClientContextData = client.SceneContextData2 as BHMatchClientContextData;
					if (null != bhmatchClientContextData)
					{
						bhmatchClientContextData.TotalScore += this.CalMVPScore(bangHuiMatchScene, this.RuntimeData.MVPScoreFactorKill);
						bhmatchClientContextData.Kill++;
					}
					this.UpdateLongTaOwnInfo(bangHuiMatchScene);
				}
			}
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			BHMatchQiZhiConfig bhmatchQiZhiConfig = monster.Tag as BHMatchQiZhiConfig;
			if (null != bhmatchQiZhiConfig)
			{
				lock (this.RuntimeData.Mutex)
				{
					BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
					if (bangHuiMatchScene != null && bangHuiMatchScene.m_eStatus == 2)
					{
						BHMatchClientContextData bhmatchClientContextData = client.SceneContextData2 as BHMatchClientContextData;
						if (null != bhmatchClientContextData)
						{
							bhmatchClientContextData.TotalScore += this.CalMVPScore(bangHuiMatchScene, this.RuntimeData.MVPScoreFactorQiZhi);
						}
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
			if (null != bangHuiMatchScene)
			{
				this.RemoveBattleSceneBuffForRole(bangHuiMatchScene, client);
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public void OnLogoutFinish(GameClient client)
		{
			BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
			if (null != bangHuiMatchScene)
			{
				if (client.ClientData.MapCode == bangHuiMatchScene.SceneInfo.MapCode_LongTa)
				{
					this.UpdateLongTaOwnInfo(bangHuiMatchScene);
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
			if (null != bangHuiMatchScene)
			{
				if (client.ClientData.MapCode == bangHuiMatchScene.SceneInfo.MapCode_LongTa)
				{
					BHMatchClientContextData bhmatchClientContextData = client.SceneContextData2 as BHMatchClientContextData;
					bhmatchClientContextData.TempleEnterTicks = TimeUtil.NOW();
					bhmatchClientContextData.TempleDamageTimes = 0;
				}
				if (this.IsBangHuiMatchMap(client.ClientData.MapCode))
				{
					this.UpdateLongTaOwnInfo(bangHuiMatchScene);
				}
				this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
				if (bangHuiMatchScene.LT_BattleWhichSide == client.ClientData.BattleWhichSide)
				{
					this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 2080013);
				}
				else
				{
					this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
				}
				this.UpdateBuff4GameClient(client, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, this.GetQiZhiBuffGoodsIDBySide(bangHuiMatchScene, client.ClientData.BattleWhichSide));
			}
			this.UpdateChengHaoBuffer(client);
		}

		public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			BHMatchConfig bhmatchConfig = client.SceneInfoObject as BHMatchConfig;
			if (null != bhmatchConfig)
			{
				if (toNewMapCode == bhmatchConfig.MapCode || toNewMapCode == bhmatchConfig.MapCode_LongTa)
				{
					if (client.ClientData.MapCode != bhmatchConfig.MapCode_LongTa)
					{
						int num;
						int num2;
						int num3;
						if (this.GetZhanMengBirthPoint(bhmatchConfig, client, toNewMapCode, out num, out num2, out num3, false))
						{
							toNewMapCode = num;
							toNewPosX = num2;
							toNewPosY = num3;
						}
					}
				}
			}
			return true;
		}

		private void UpdateLongTaOwnInfo(BangHuiMatchScene scene)
		{
			if (scene.m_eStatus == 2)
			{
				lock (this.RuntimeData.Mutex)
				{
					CopyMap copyMap = null;
					if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMap))
					{
						this.RuntimeData.UpdateLongTaOwnInfoTimes += 1L;
						List<GameClient> list = copyMap.GetClientsList();
						list = Global.GetMapAliveClientsEx(list, scene.SceneInfo.MapCode_LongTa, true, this.RuntimeData.UpdateLongTaOwnInfoTimes);
						Dictionary<int, BHMatchRoleCountData> dictionary = new Dictionary<int, BHMatchRoleCountData>();
						for (int i = 0; i < list.Count; i++)
						{
							GameClient gameClient = list[i];
							if (gameClient != null && gameClient.ClientData.HideGM <= 0)
							{
								if (gameClient.ClientData.Faction > 0)
								{
									BHMatchRoleCountData bhmatchRoleCountData;
									if (!dictionary.TryGetValue(gameClient.ClientData.BattleWhichSide, out bhmatchRoleCountData))
									{
										bhmatchRoleCountData = new BHMatchRoleCountData
										{
											bhid = gameClient.ClientData.Faction,
											bhname = gameClient.ClientData.BHName,
											rolecount = 0,
											BattleWhichSide = gameClient.ClientData.BattleWhichSide,
											serverid = gameClient.ServerId
										};
										dictionary.Add(gameClient.ClientData.BattleWhichSide, bhmatchRoleCountData);
									}
									bhmatchRoleCountData.rolecount++;
								}
							}
						}
						int num = 0;
						int num2 = 0;
						BHMatchRoleCountData bhmatchRoleCountData2 = null;
						dictionary.TryGetValue(1, out bhmatchRoleCountData2);
						if (null != bhmatchRoleCountData2)
						{
							num = bhmatchRoleCountData2.rolecount;
						}
						BHMatchRoleCountData bhmatchRoleCountData3 = null;
						dictionary.TryGetValue(2, out bhmatchRoleCountData3);
						if (null != bhmatchRoleCountData3)
						{
							num2 = bhmatchRoleCountData3.rolecount;
						}
						bool flag2 = false;
						if (scene.ScoreData.PlayerNum1 != num || scene.ScoreData.PlayerNum2 != num2)
						{
							flag2 = true;
						}
						scene.ScoreData.PlayerNum1 = num;
						scene.ScoreData.PlayerNum2 = num2;
						if (dictionary.Count == 1)
						{
							BHMatchRoleCountData bhmatchRoleCountData4 = dictionary.Values.FirstOrDefault<BHMatchRoleCountData>();
							if (scene.LT_BattleWhichSide != bhmatchRoleCountData4.BattleWhichSide)
							{
								this.HandleTakeTemple(scene, list, bhmatchRoleCountData4.BattleWhichSide);
							}
							scene.ScoreData.LT_BHName = bhmatchRoleCountData4.bhname;
							scene.LT_BHServerID = bhmatchRoleCountData4.serverid;
							scene.LT_BattleWhichSide = bhmatchRoleCountData4.BattleWhichSide;
							scene.LT_OwnTicks = TimeUtil.NOW();
							scene.LT_OwnTicksDelta = 0L;
						}
						else if (dictionary.Count == 2)
						{
							if (0 == scene.LT_BattleWhichSide)
							{
								BHMatchRoleCountData bhmatchRoleCountData4;
								if (dictionary.TryGetValue(list[0].ClientData.BattleWhichSide, out bhmatchRoleCountData4))
								{
									this.HandleTakeTemple(scene, list, bhmatchRoleCountData4.BattleWhichSide);
									scene.ScoreData.LT_BHName = bhmatchRoleCountData4.bhname;
									scene.LT_BHServerID = bhmatchRoleCountData4.serverid;
									scene.LT_BattleWhichSide = bhmatchRoleCountData4.BattleWhichSide;
									scene.LT_OwnTicks = TimeUtil.NOW();
									scene.LT_OwnTicksDelta = 0L;
								}
							}
						}
						else
						{
							scene.ScoreData.LT_BHName = "";
							scene.LT_BattleWhichSide = 0;
							scene.LT_OwnTicks = 0L;
							scene.LT_OwnTicksDelta = 0L;
						}
						if (flag2)
						{
							foreach (CopyMap copyMap2 in scene.CopyMapDict.Values)
							{
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copyMap2);
							}
						}
					}
				}
			}
		}

		private void HandleTakeTemple(BangHuiMatchScene scene, List<GameClient> lsClients, int BattleWhichSide)
		{
			for (int i = 0; i < lsClients.Count; i++)
			{
				GameClient gameClient = lsClients[i];
				if (gameClient.ClientData.BattleWhichSide == BattleWhichSide)
				{
					BHMatchClientContextData bhmatchClientContextData = gameClient.SceneContextData2 as BHMatchClientContextData;
					if (null != bhmatchClientContextData)
					{
						bhmatchClientContextData.TotalScore += this.CalMVPScore(scene, this.RuntimeData.MVPScoreFactorTemple);
					}
					this.UpdateBuff4GameClient(gameClient, BufferItemTypes.BangHuiMatchDeHurt_Temple, 2080013);
				}
				else
				{
					this.UpdateBuff4GameClient(gameClient, BufferItemTypes.BangHuiMatchDeHurt_Temple, 0);
				}
			}
			scene.GameStatisticalData.templechg++;
		}

		private int GetQiZhiBuffGoodsIDBySide(BangHuiMatchScene scene, int side)
		{
			int result;
			switch ((side == 1) ? scene.ScoreData.QiZhi1 : scene.ScoreData.QiZhi2)
			{
			case 1:
				result = 2080014;
				break;
			case 2:
				result = 2080015;
				break;
			case 3:
				result = 2080016;
				break;
			case 4:
				result = 2080017;
				break;
			default:
				result = 0;
				break;
			}
			return result;
		}

		private double CalClientDehurtValue(BangHuiMatchScene scene, GameClient client)
		{
			double num = 0.0;
			int num2 = (client.ClientData.BattleWhichSide == 1) ? scene.ScoreData.QiZhi1 : scene.ScoreData.QiZhi2;
			int num3 = (client.ClientData.BattleWhichSide == scene.LT_BattleWhichSide) ? 1 : 0;
			num += this.RuntimeData.BHMatchGodDebuffQiZhi * (double)num2;
			return num + this.RuntimeData.BHMatchGodDebuffTemple * (double)num3;
		}

		private void UpdateQiZhiBangHuiOwnNum(BangHuiMatchScene scene)
		{
			int num = 0;
			int num2 = 0;
			lock (this.RuntimeData.Mutex)
			{
				foreach (BHMatchQiZhiConfig bhmatchQiZhiConfig in scene.NPCID2QiZhiConfigDict.Values)
				{
					if (bhmatchQiZhiConfig.BattleWhichSide == 1 && bhmatchQiZhiConfig.Alive)
					{
						num++;
					}
					if (bhmatchQiZhiConfig.BattleWhichSide == 2 && bhmatchQiZhiConfig.Alive)
					{
						num2++;
					}
				}
			}
			scene.ScoreData.QiZhi1 = num;
			scene.ScoreData.QiZhi2 = num2;
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				List<GameClient> clientsList = copyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null && gameClient.ClientData.CopyMapID == copyMap.CopyMapID)
						{
							this.UpdateBuff4GameClient(gameClient, BufferItemTypes.BangHuiMatchDeHurt_QiZhi, this.GetQiZhiBuffGoodsIDBySide(scene, gameClient.ClientData.BattleWhichSide));
						}
					}
				}
			}
		}

		private void CheckSceneTempleDamage(BangHuiMatchScene scene, long nowTicks)
		{
			CopyMap copyMap = null;
			if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMap))
			{
				List<GameClient> list = copyMap.GetClientsList();
				list = Global.GetMapAliveClientsEx(list, scene.SceneInfo.MapCode_LongTa, false, 0L);
				bool flag = false;
				for (int i = 0; i < list.Count; i++)
				{
					GameClient gameClient = list[i];
					if (gameClient != null && gameClient.ClientData.HideGM <= 0)
					{
						BHMatchClientContextData bhmatchClientContextData = gameClient.SceneContextData2 as BHMatchClientContextData;
						int num = (int)(nowTicks - bhmatchClientContextData.TempleEnterTicks) / 1000;
						int num2 = num / this.RuntimeData.BHMatchGodDamagePeriod;
						if (num2 > 0 && bhmatchClientContextData.TempleDamageTimes != num2)
						{
							bhmatchClientContextData.TempleDamageTimes = num2;
							double num3;
							if (num2 > this.RuntimeData.BHMatchGodDamagePctList.Count)
							{
								num3 = this.RuntimeData.BHMatchGodDamagePctList[this.RuntimeData.BHMatchGodDamagePctList.Count - 1];
							}
							else
							{
								num3 = this.RuntimeData.BHMatchGodDamagePctList[num2 - 1];
							}
							double num4 = num3 * (1.0 - this.CalClientDehurtValue(scene, gameClient));
							double num5 = (double)gameClient.ClientData.LifeV * num4;
							int currentLifeV = gameClient.ClientData.CurrentLifeV;
							gameClient.ClientData.CurrentLifeV -= (int)num5;
							num5 = (double)(currentLifeV - gameClient.ClientData.CurrentLifeV);
							if (num5 <= 0.0)
							{
								return;
							}
							GameManager.ClientMgr.SubSpriteLifeV(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num5);
							GameManager.ClientMgr.NotifySpriteInjured(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, gameClient.ClientData.RoleID, 0, (int)num5, (double)gameClient.ClientData.CurrentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							gameClient.sendCmd(1178, "", false);
							flag |= (gameClient.ClientData.CurrentLifeV <= 0);
						}
					}
				}
				if (flag)
				{
					this.UpdateLongTaOwnInfo(scene);
				}
			}
		}

		private void CheckSceneScoreTime(BangHuiMatchScene scene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool flag2 = false;
				foreach (KeyValuePair<int, BHMatchQiZhiConfig> keyValuePair in scene.NPCID2QiZhiConfigDict)
				{
					BHMatchQiZhiConfig value = keyValuePair.Value;
					if (value.BattleWhichSide != 0 && value.Alive)
					{
						value.OwnTicksDelta += nowTicks - value.OwnTicks;
						value.OwnTicks = nowTicks;
						if (value.OwnTicksDelta >= (long)(value.ProduceTime * 1000) && value.ProduceTime > 0)
						{
							int num = (int)(value.OwnTicksDelta / (long)(value.ProduceTime * 1000));
							value.OwnTicksDelta -= (long)(num * value.ProduceTime * 1000);
							if (value.BattleWhichSide == 1)
							{
								scene.ScoreData.Score1 += num * value.ProduceNum;
							}
							if (value.BattleWhichSide == 2)
							{
								scene.ScoreData.Score2 += num * value.ProduceNum;
							}
							flag2 = true;
						}
					}
				}
				if (scene.LT_BattleWhichSide != 0)
				{
					scene.LT_OwnTicksDelta += nowTicks - scene.LT_OwnTicks;
					scene.LT_OwnTicks = nowTicks;
					if (scene.LT_OwnTicksDelta >= (long)(this.RuntimeData.TempleProduceTime * 1000) && this.RuntimeData.TempleProduceTime > 0)
					{
						int num = (int)(scene.LT_OwnTicksDelta / (long)(this.RuntimeData.TempleProduceTime * 1000));
						scene.LT_OwnTicksDelta -= (long)(num * this.RuntimeData.TempleProduceTime * 1000);
						if (scene.LT_BattleWhichSide == 1)
						{
							scene.ScoreData.Score1 += num * this.RuntimeData.TempleProduceNum;
						}
						if (scene.LT_BattleWhichSide == 2)
						{
							scene.ScoreData.Score2 += num * this.RuntimeData.TempleProduceNum;
						}
						flag2 = true;
					}
				}
				if (flag2)
				{
					foreach (CopyMap copyMap in scene.CopyMapDict.Values)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, scene.ScoreData, copyMap);
					}
				}
			}
		}

		private int CalMVPScore(BangHuiMatchScene scene, int factor)
		{
			int num = (int)(TimeUtil.NOW() - scene.m_lBeginTime) / 1000;
			return (int)((1.0 + (double)num / 60.0 * 0.075) * (double)factor);
		}

		private void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int bufferGoodsID)
		{
			double[] actionParams = new double[]
			{
				(double)bufferGoodsID
			};
			Global.UpdateBufferData(client, bufferItem, actionParams, 1, false);
		}

		public bool IsQiZhiExtensionID(int QiZhiID)
		{
			return QiZhiID == this.RuntimeData.BattleQiZhiMonsterID1 || QiZhiID == this.RuntimeData.BattleQiZhiMonsterID2;
		}

		public bool IsBangHuiMatchMap(int MapCode)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (KeyValuePair<int, BHMatchConfig> keyValuePair in this.RuntimeData.CommonConfigData.BHMatchConfigDict)
				{
					if (keyValuePair.Value.MapCode == MapCode || keyValuePair.Value.MapCode_LongTa == MapCode)
					{
						return true;
					}
				}
			}
			return false;
		}

		public void InstallJunQi(BangHuiMatchScene scene, CopyMap copyMap, GameClient client, BHMatchQiZhiConfig item)
		{
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.SceneInfo.MapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				item.OwnTicks = TimeUtil.NOW();
				int monsterID = 0;
				if (client.ClientData.BattleWhichSide == 1)
				{
					monsterID = this.RuntimeData.BattleQiZhiMonsterID1;
				}
				else if (client.ClientData.BattleWhichSide == 2)
				{
					monsterID = this.RuntimeData.BattleQiZhiMonsterID2;
				}
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterID, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, 45, item, null);
			}
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			BHMatchQiZhiConfig bhmatchQiZhiConfig = null;
			bool flag = false;
			bool flag2 = false;
			CopyMap copyMap = null;
			BangHuiMatchScene bangHuiMatchScene = client.SceneObject as BangHuiMatchScene;
			bool result;
			if (bangHuiMatchScene == null || !bangHuiMatchScene.CopyMapDict.TryGetValue(client.ClientData.MapCode, out copyMap))
			{
				result = flag;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					if (bangHuiMatchScene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out bhmatchQiZhiConfig))
					{
						flag = true;
						if (bhmatchQiZhiConfig.Alive)
						{
							return flag;
						}
						if (client.ClientData.BattleWhichSide != bhmatchQiZhiConfig.BattleWhichSide && Math.Abs(TimeUtil.NOW() - bhmatchQiZhiConfig.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Math.Abs(client.ClientData.PosX - bhmatchQiZhiConfig.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - bhmatchQiZhiConfig.PosY) <= 1000)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						this.InstallJunQi(bangHuiMatchScene, copyMap, client, bhmatchQiZhiConfig);
						this.UpdateQiZhiBangHuiOwnNum(bangHuiMatchScene);
						foreach (CopyMap copyMap2 in bangHuiMatchScene.CopyMapDict.Values)
						{
							GameManager.ClientMgr.BroadSpecialCopyMapMessage<BangHuiMatchScoreData>(1173, bangHuiMatchScene.ScoreData, copyMap2);
						}
					}
				}
				result = flag;
			}
			return result;
		}

		public const SceneUIClasses ManagerType = 45;

		private static BangHuiMatchManager instance = new BangHuiMatchManager();

		public BangHuiMatchData RuntimeData = new BangHuiMatchData();

		public BHMatchSyncData BHMatchSyncDataCache = new BHMatchSyncData();

		private RoleDataEx OwnerRoleData = null;

		public ConcurrentDictionary<int, BangHuiMatchScene> SceneDict = new ConcurrentDictionary<int, BangHuiMatchScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
