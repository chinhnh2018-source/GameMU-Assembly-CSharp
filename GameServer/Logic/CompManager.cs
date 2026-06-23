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
using Server.Protocol;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic
{
	public class CompManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static CompManager getInstance()
		{
			return CompManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("CompManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool InitConfig()
		{
			bool result = true;
			string text = "";
			lock (this.RuntimeData.Mutex)
			{
				try
				{
					this.RuntimeData.CompConfigDict.Clear();
					text = "Config/Comp.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompConfig compConfig = new CompConfig();
						compConfig.ID = (int)Global.GetSafeAttributeLong(xml, "CompID");
						compConfig.CompName = Global.GetSafeAttributeStr(xml, "CompName");
						compConfig.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						compConfig.BossID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
						compConfig.MaxPlayer = (int)Global.GetSafeAttributeLong(xml, "MaxPlayer");
						compConfig.MoBai = (int)Global.GetSafeAttributeLong(xml, "MoBai");
						string safeAttributeStr = Global.GetSafeAttributeStr(xml, "JiaoTuanBirth");
						if (!string.IsNullOrEmpty(safeAttributeStr))
						{
							string[] array = safeAttributeStr.Split(new char[]
							{
								'|'
							});
							if (array.Length == 3)
							{
								compConfig.BirthPosX[0] = Global.SafeConvertToInt32(array[0]);
								compConfig.BirthPosY[0] = Global.SafeConvertToInt32(array[1]);
								compConfig.BirthRadius[0] = Global.SafeConvertToInt32(array[2]);
							}
						}
						string safeAttributeStr2 = Global.GetSafeAttributeStr(xml, "MengJunBirth");
						if (!string.IsNullOrEmpty(safeAttributeStr2))
						{
							string[] array = safeAttributeStr2.Split(new char[]
							{
								'|'
							});
							if (array.Length == 3)
							{
								compConfig.BirthPosX[1] = Global.SafeConvertToInt32(array[0]);
								compConfig.BirthPosY[1] = Global.SafeConvertToInt32(array[1]);
								compConfig.BirthRadius[1] = Global.SafeConvertToInt32(array[2]);
							}
						}
						string safeAttributeStr3 = Global.GetSafeAttributeStr(xml, "XieHuiBirth");
						if (!string.IsNullOrEmpty(safeAttributeStr3))
						{
							string[] array = safeAttributeStr3.Split(new char[]
							{
								'|'
							});
							if (array.Length == 3)
							{
								compConfig.BirthPosX[2] = Global.SafeConvertToInt32(array[0]);
								compConfig.BirthPosY[2] = Global.SafeConvertToInt32(array[1]);
								compConfig.BirthRadius[2] = Global.SafeConvertToInt32(array[2]);
							}
						}
						this.RuntimeData.CompConfigDict[compConfig.ID] = compConfig;
					}
					this.RuntimeData.CompResourcesConfigDict.Clear();
					text = "Config/CompResources.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompResourcesConfig compResourcesConfig = new CompResourcesConfig();
						compResourcesConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compResourcesConfig.MapCodeID = (int)Global.GetSafeAttributeLong(xml, "MapID");
						compResourcesConfig.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
						string[] array2 = Global.GetSafeAttributeStr(xml, "Site").Split(new char[]
						{
							'|'
						});
						if (array2.Length == 2)
						{
							compResourcesConfig.PosX = Global.SafeConvertToInt32(array2[0]);
							compResourcesConfig.PosY = Global.SafeConvertToInt32(array2[1]);
						}
						compResourcesConfig.GrowTime = (int)Global.GetSafeAttributeLong(xml, "GrowTime");
						compResourcesConfig.CollectTime = (int)Global.GetSafeAttributeLong(xml, "CollectTime");
						compResourcesConfig.AutoCollectTime = (int)Global.GetSafeAttributeLong(xml, "AutoCollectTime");
						array2 = Global.GetSafeAttributeStr(xml, "RefreshTime").Split(new char[]
						{
							'-'
						});
						if (array2.Length == 2)
						{
							TimeSpan.TryParse(array2[0], out compResourcesConfig.RefreshTimeBegin);
							TimeSpan.TryParse(array2[1], out compResourcesConfig.RefreshTimeEnd);
						}
						compResourcesConfig.BoomValue = (int)Global.GetSafeAttributeLong(xml, "CompNum");
						compResourcesConfig.CompDonate = (int)Global.GetSafeAttributeLong(xml, "CompHonor");
						compResourcesConfig.JunXian = (int)Global.GetSafeAttributeLong(xml, "CompFeast");
						this.RuntimeData.CompResourcesConfigDict[compResourcesConfig.ID] = compResourcesConfig;
					}
					this.RuntimeData.CompSolderSiteConfigList.Clear();
					text = "Config/CompSolderSite.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompSolderSiteConfig compSolderSiteConfig = new CompSolderSiteConfig();
						compSolderSiteConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						string[] array2 = Global.GetSafeAttributeStr(xml, "Site").Split(new char[]
						{
							'|'
						});
						if (array2.Length == 2)
						{
							compSolderSiteConfig.PosX = Global.SafeConvertToInt32(array2[0]);
							compSolderSiteConfig.PosY = Global.SafeConvertToInt32(array2[1]);
						}
						array2 = Global.GetSafeAttributeStr(xml, "RefreshTime").Split(new char[]
						{
							'-'
						});
						if (array2.Length == 2)
						{
							TimeSpan.TryParse(array2[0], out compSolderSiteConfig.RefreshTimeBegin);
							TimeSpan.TryParse(array2[1], out compSolderSiteConfig.RefreshTimeEnd);
						}
						this.RuntimeData.CompSolderSiteConfigList.Add(compSolderSiteConfig);
					}
					this.RuntimeData.CompSolderConfigDict.Clear();
					text = "Config/CompSolder.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompSolderConfig compSolderConfig = new CompSolderConfig();
						compSolderConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compSolderConfig.CompID = (int)Global.GetSafeAttributeLong(xml, "CompID");
						compSolderConfig.Rank = (int)Global.GetSafeAttributeLong(xml, "Level");
						compSolderConfig.MonsterID = (int)Global.GetSafeAttributeLong(xml, "MonstersID");
						compSolderConfig.AlarmTime = (int)Global.GetSafeAttributeLong(xml, "AlarmTime");
						this.RuntimeData.CompSolderConfigDict[new KeyValuePair<int, int>(compSolderConfig.CompID, compSolderConfig.Rank)] = compSolderConfig;
					}
					this.RuntimeData.CompNoticeConfigDict.Clear();
					text = "Config/CompNotice.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompNoticeConfig compNoticeConfig = new CompNoticeConfig();
						compNoticeConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compNoticeConfig.Goal = (int)Global.GetSafeAttributeLong(xml, "Goal");
						compNoticeConfig.CDTime = (int)Global.GetSafeAttributeLong(xml, "CDTime");
						compNoticeConfig.Range = (int)Global.GetSafeAttributeLong(xml, "Range");
						compNoticeConfig.OriginalMapOpen = Global.String2IntArray(Global.GetSafeAttributeStr(xml, "OriginalMapOpen"), '|');
						this.RuntimeData.CompNoticeConfigDict[compNoticeConfig.ID] = compNoticeConfig;
					}
					this.RuntimeData.CompLevelConfigList.Clear();
					text = "Config/CompLevel.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						CompLevelConfig compLevelConfig = new CompLevelConfig();
						compLevelConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						compLevelConfig.CompID = (int)Global.GetSafeAttributeLong(xml, "CompID");
						compLevelConfig.Level = (int)Global.GetSafeAttributeLong(xml, "Level");
						compLevelConfig.Enemy = (int)Global.GetSafeAttributeLong(xml, "Enemy");
						compLevelConfig.TalkCD = (int)Global.GetSafeAttributeLong(xml, "TalkCD");
						compLevelConfig.CraftSelfBuffID = (int)Global.GetSafeAttributeLong(xml, "CraftSelfBuffID");
						compLevelConfig.CraftBuffID = (int)Global.GetSafeAttributeLong(xml, "CraftBuffID");
						this.RuntimeData.CompLevelConfigList.Add(compLevelConfig);
					}
					ConfigParser.ParseAwardsItemList(GameManager.systemParamsList.GetParamValueByName("CompRecommend"), ref this.RuntimeData.CompRecommend, '|', ',');
					this.RuntimeData.CompRecommendRatio = GameManager.systemParamsList.GetParamValueDoubleByName("CompRecommendRatio", 0.0);
					this.RuntimeData.CompReplaceAmerce = GameManager.systemParamsList.GetParamValueDoubleByName("CompReplaceAmerce", 0.0);
					this.RuntimeData.CompReplaceNeed = (int)GameManager.systemParamsList.GetParamValueIntByName("CompReplaceNeed", -1);
					this.RuntimeData.CompSolderCD = GameManager.systemParamsList.GetParamValueIntArrayByName("CompSolderCD", ',');
					this.RuntimeData.CompBossCompNum = GameManager.systemParamsList.GetParamValueIntArrayByName("CompBossCompNum", ',');
					this.RuntimeData.CompBossCompHonor = GameManager.systemParamsList.GetParamValueIntArrayByName("CompBossCompHonor", ',');
					this.RuntimeData.CompBossRealive = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CompBossRealive", ',');
					this.RuntimeData.CompEnemy = GameManager.systemParamsList.GetParamValueIntArrayByName("CompEnemy", ',');
					this.RuntimeData.CompEnemyHurtNum = GameManager.systemParamsList.GetParamValueDoubleByName("CompEnemyHurtNum", 0.0);
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("CompShop", '|');
					foreach (string text2 in paramValueStringListByName)
					{
						string[] array2 = text2.Split(new char[]
						{
							','
						});
						int item = Global.SafeConvertToInt32(array2[0]);
						int item2 = Global.SafeConvertToInt32(array2[1]);
						this.RuntimeData.CompShop.Add(new Tuple<int, int>(item, item2));
					}
					this.RuntimeData.CompShopDuiHuanType = GameManager.systemParamsList.GetParamValueIntArrayByName("CraftStore", ',');
					this.RuntimeData.MaxDailyTaskNumDict.Clear();
					int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("CompTaskNum", ',');
					if (null != paramValueIntArrayByName)
					{
						for (int i = 0; i < paramValueIntArrayByName.Length; i++)
						{
							this.RuntimeData.MaxDailyTaskNumDict[100 + i] = paramValueIntArrayByName[i];
						}
					}
					this.RuntimeData.CompTaskBeginDict.Clear();
					List<string> paramValueStringListByName2 = GameManager.systemParamsList.GetParamValueStringListByName("CompTaskBegin", '|');
					foreach (string text2 in paramValueStringListByName2)
					{
						string[] array3 = text2.Split(new char[]
						{
							','
						});
						if (array3.Length == 4)
						{
							int key = Global.SafeConvertToInt32(array3[0]);
							List<int> list = null;
							if (!this.RuntimeData.CompTaskBeginDict.TryGetValue(key, out list))
							{
								list = new List<int>();
								this.RuntimeData.CompTaskBeginDict[key] = list;
							}
							list.Add(Global.SafeConvertToInt32(array3[1]));
							list.Add(Global.SafeConvertToInt32(array3[2]));
							list.Add(Global.SafeConvertToInt32(array3[3]));
						}
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

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1125, 1, 1, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1126, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1127, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1128, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1129, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1130, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1132, 1, 5, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1136, 1, 1, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1137, 2, 2, CompManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10002, 48, CompManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 48, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, CompManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, CompManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(10002, 48, CompManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 48, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, CompManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, CompManager.getInstance());
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
			return GlobalNew.IsGongNengOpened(client, 98, hint);
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
				case 1125:
					return this.ProcessCompDataCmd(client, nID, bytes, cmdParams);
				case 1126:
					return this.ProcessCompJoinCmd(client, nID, bytes, cmdParams);
				case 1127:
					return this.ProcessCompRankInfoCmd(client, nID, bytes, cmdParams);
				case 1128:
					return this.ProcessCompSetBulletinCmd(client, nID, bytes, cmdParams);
				case 1129:
					return this.ProcessCompSetEnemyCmd(client, nID, bytes, cmdParams);
				case 1130:
					return this.ProcessCompZhiWuCmd(client, nID, bytes, cmdParams);
				case 1132:
					return this.ProcessCompEnterCmd(client, nID, bytes, cmdParams);
				case 1136:
					return this.ProcessGetCompAdmireDataCmd(client, nID, bytes, cmdParams);
				case 1137:
					return this.ProcessCompAdmireCmd(client, nID, bytes, cmdParams);
				}
				result = true;
			}
			return result;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
			if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
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
				}
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			int eventType = eventObject.EventType;
			int num = eventType;
			if (num != 30)
			{
				if (num == 10002)
				{
					CaiJiEventObject caiJiEventObject = eventObject as CaiJiEventObject;
					if (null != caiJiEventObject)
					{
						GameClient client = caiJiEventObject.Source as GameClient;
						Monster monster = caiJiEventObject.Target as Monster;
						this.OnCaiJiFinish(client, monster);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
				}
			}
			else
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					CompSolderSiteConfig compSolderSiteConfig = onCreateMonsterEventObject.Monster.Tag as CompSolderSiteConfig;
					if (null != compSolderSiteConfig)
					{
						onCreateMonsterEventObject.Monster.Camp = compSolderSiteConfig.SolderConfig.CompID;
						onCreateMonsterEventObject.Result = true;
						onCreateMonsterEventObject.Handled = true;
					}
				}
			}
		}

		public bool ProcessCompDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				Dictionary<int, KFCompData> dictionary = null;
				Dictionary<int, List<KFCompRankInfo>> dictionary2 = null;
				lock (this.RuntimeData.Mutex)
				{
					dictionary = this.CompSyncDataCache.CompDataDict.V;
					dictionary2 = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				}
				CompData compData = new CompData();
				if (client.ClientData.CompType > 0)
				{
					dictionary.TryGetValue(client.ClientData.CompType, out compData.kfCompData);
					compData.kfCompData = (KFCompData)compData.kfCompData.Clone();
					compData.kfCompData.StrongholdDict = null;
					compData.kfCompData.compBattleBaseData = CompBattleManager.getInstance().GetCompBattleBaseData(client.ClientData.CompType);
					CompBattleGameStates compBattleStates = 0;
					CompBattleManager.getInstance().CheckCondition(client, ref compBattleStates);
					compData.kfCompData.CompBattleStates = compBattleStates;
					KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
					if (kfcompRoleData != null && null != compData.kfCompData)
					{
						compData.kfCompData.SelfJunXian = kfcompRoleData.JunXian;
						compData.kfCompData.CompTypeBattle = kfcompRoleData.CompTypeBattle;
						if (kfcompRoleData.CompType > 0 && kfcompRoleData.CompType != client.ClientData.CompType)
						{
							client.ClientData.CompType = kfcompRoleData.CompType;
							GameManager.ClientMgr.SetCompType(client, client.ClientData.CompType);
							return false;
						}
						int compBattleJiFenValue = GameManager.ClientMgr.GetCompBattleJiFenValue(client);
						if (compBattleJiFenValue != kfcompRoleData.BattleJiFen)
						{
							int addValue = kfcompRoleData.BattleJiFen - compBattleJiFenValue;
							GameManager.ClientMgr.ModifyCompBattleJiFenValue(client, addValue, "势力战KF", true, true, false);
						}
						int compMineJiFenValue = GameManager.ClientMgr.GetCompMineJiFenValue(client);
						if (compMineJiFenValue != kfcompRoleData.MineJiFen)
						{
							int addValue = kfcompRoleData.MineJiFen - compMineJiFenValue;
							GameManager.ClientMgr.ModifyCompMineJiFenValue(client, addValue, "势力矿洞KF", true, true, false);
						}
					}
					else if (kfcompRoleData == null && compData.kfCompData != null && client.ClientData.CompType > 0)
					{
						int num2 = TianTiClient.getInstance().Comp_JoinComp_Repair(client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, client.ClientData.CompType, GameManager.ClientMgr.GetCompBattleJiFenValue(client));
						if (num2 >= 0)
						{
							kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
							if (null != kfcompRoleData)
							{
								compData.kfCompData.SelfJunXian = kfcompRoleData.JunXian;
								compData.kfCompData.CompTypeBattle = kfcompRoleData.CompTypeBattle;
							}
						}
					}
				}
				for (int i = 1; i <= 3; i++)
				{
					KFCompData kfcompData = null;
					dictionary.TryGetValue(i, out kfcompData);
					if (null != kfcompData)
					{
						compData.YestdBoomValueList.Add(kfcompData.YestdBoomValue);
					}
					else
					{
						compData.YestdBoomValueList.Add(0);
					}
				}
				compData.SelectData.RecommendCompList = this.ComputerRecommendCompList(compData.YestdBoomValueList);
				for (int j = 1; j <= 3; j++)
				{
					List<KFCompRankInfo> list = null;
					if (dictionary2.TryGetValue(j, out list) && list != null && list.Count != 0)
					{
						KFCompRoleData kfcompRoleData2 = TianTiClient.getInstance().Comp_GetCompRoleData(list[0].Key);
						if (kfcompRoleData2 != null && kfcompRoleData2.CompType != j)
						{
							compData.SelectData.DaLingZhuNameList.Add("");
						}
						else
						{
							compData.SelectData.DaLingZhuNameList.Add(list[0].Param1);
						}
					}
					else
					{
						compData.SelectData.DaLingZhuNameList.Add("");
					}
				}
				client.sendCmd<CompData>(nID, compData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				if (num3 < 1 || num3 > 3)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				if (!this.CheckMap(client))
				{
					num = -21;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				if (num3 == client.ClientData.CompType)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				List<int> yestdBoomValueList = this.GetYestdBoomValueList();
				if (this.ComputerRecommendCompList(yestdBoomValueList).Contains(num3))
				{
					if (!Global.CanAddGoodsNum(client, this.RuntimeData.CompRecommend.Items.Count))
					{
						num = -100;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
						return true;
					}
				}
				bool flag = false;
				if (client.ClientData.CompType > 0)
				{
					flag = true;
					CompBattleGameStates compBattleGameStates = 0;
					CompBattleManager.getInstance().CheckCondition(client, ref compBattleGameStates);
					if (compBattleGameStates != 0)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
						return true;
					}
					CompMineManager.getInstance().CheckCondition(client, ref compBattleGameStates);
					if (compBattleGameStates != 0)
					{
						num = -12;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
						return true;
					}
					if (client.ClientData.UserMoney < this.RuntimeData.CompReplaceNeed)
					{
						num = -10;
						client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
						return true;
					}
				}
				num = TianTiClient.getInstance().Comp_JoinComp(client.ClientData.RoleID, client.ClientData.ZoneID, client.ClientData.RoleName, num3);
				if (num < 0)
				{
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				client.ClientData.CompType = num3;
				GameManager.ClientMgr.SetCompType(client, num3);
				if (flag)
				{
					GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.CompReplaceNeed, "势力争霸切换势力", true, true, false, DaiBiSySType.None);
					int compDonateValue = GameManager.ClientMgr.GetCompDonateValue(client);
					int addValue = (int)((double)compDonateValue * this.RuntimeData.CompReplaceAmerce) - compDonateValue;
					GameManager.ClientMgr.ModifyCompDonateValue(client, addValue, "势力争霸切换势力", true, true, false);
				}
				if (this.ComputerRecommendCompList(yestdBoomValueList).Contains(num3))
				{
					foreach (AwardsItemData awardsItemData in this.RuntimeData.CompRecommend.Items)
					{
						Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "势力争霸加入势力", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
					}
				}
				this.HandleCompTaskSomething(client, false);
				Global.SaveRoleParamsStringToDB(client, "49", "", true);
				client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
				{
					return true;
				}
				Dictionary<int, List<KFCompRankInfo>> dictionary = null;
				Dictionary<int, List<KFCompRankInfo>> dictionary2 = null;
				lock (this.RuntimeData.Mutex)
				{
					dictionary = this.CompSyncDataCache.CompRankJunXianDict.V;
					dictionary2 = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				}
				KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(client.ClientData.RoleID);
				if (null == kfcompRoleData)
				{
					return true;
				}
				List<KFCompRankInfo> list = null;
				if (num2 == 1)
				{
					dictionary.TryGetValue(client.ClientData.CompType, out list);
				}
				else if (num2 == 2)
				{
					dictionary2.TryGetValue(client.ClientData.CompType, out list);
				}
				if (null == list)
				{
					list = new List<KFCompRankInfo>();
				}
				List<KFCompRankInfo> list2 = new List<KFCompRankInfo>(list);
				if (null != list2)
				{
					if (num2 == 1)
					{
						list2.Add(new KFCompRankInfo
						{
							Key = client.ClientData.RoleID,
							Value = kfcompRoleData.JunXian,
							Param1 = Global.FormatRoleNameWithZoneId2(client)
						});
					}
					else if (num2 == 2)
					{
						list2.Add(new KFCompRankInfo
						{
							Key = client.ClientData.RoleID,
							Value = kfcompRoleData.JunXianLast,
							Param1 = Global.FormatRoleNameWithZoneId2(client)
						});
					}
				}
				client.sendCmd<List<KFCompRankInfo>>(nID, list2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompSetBulletinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				string text = cmdParams[1];
				if (client.ClientData.CompZhiWu != 1)
				{
					num = -12;
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				num = NameServerNamager.CheckInvalidCharacters(text, false);
				if (num < 0)
				{
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				num = TianTiClient.getInstance().Comp_SetBulletin(client.ClientData.CompType, text);
				if (num < 0)
				{
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				KFCompData kfcompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out kfcompData);
				}
				if (null != kfcompData)
				{
					kfcompData.Bulletin = text;
				}
				client.sendCmd<int>(nID, num, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompSetEnemyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				if (num3 < 1 || num3 > 3)
				{
					return true;
				}
				if (client.ClientData.CompType <= 0 || client.ClientData.CompZhiWu <= 0 || num3 == client.ClientData.CompType)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				List<CompLevelConfig> list = null;
				lock (this.RuntimeData.Mutex)
				{
					list = this.RuntimeData.CompLevelConfigList;
				}
				CompLevelConfig compLevelConfig = list.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
				if (null == compLevelConfig)
				{
					num = -3;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				if (0 == compLevelConfig.Enemy)
				{
					num = -12;
					client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
					return true;
				}
				TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 3, num3, 0);
				client.sendCmd(nID, string.Format("{0}:{1}", num, num3), false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompZhiWuCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				if (num2 < 1 || num2 > 3)
				{
					return true;
				}
				List<KFCompRankInfo> list = new List<KFCompRankInfo>();
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(num2, out list);
				}
				CompZhiWuData compZhiWuData = new CompZhiWuData();
				if (null != list)
				{
					List<KFCompRankInfo> list2 = new List<KFCompRankInfo>(list);
					if (list2.Count > 5)
					{
						list2 = list2.GetRange(0, 5);
					}
					foreach (KFCompRankInfo kfcompRankInfo in list2)
					{
						KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(kfcompRankInfo.Key);
						if (kfcompRoleData == null || kfcompRoleData.CompType != num2)
						{
							compZhiWuData.CompRoleData.Add(new KFCompRoleData());
						}
						else
						{
							compZhiWuData.CompRoleData.Add(kfcompRoleData);
						}
					}
				}
				client.sendCmd<CompZhiWuData>(nID, compZhiWuData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessCompEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int cmdData = 0;
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = 0;
				int num3 = 0;
				int num4 = 0;
				int num5 = 0;
				if (cmdParams.Length >= 2)
				{
					num2 = Global.SafeConvertToInt32(cmdParams[1]);
				}
				if (cmdParams.Length >= 3)
				{
					num3 = Global.SafeConvertToInt32(cmdParams[2]);
				}
				if (cmdParams.Length >= 4)
				{
					num4 = Global.SafeConvertToInt32(cmdParams[3]);
				}
				if (cmdParams.Length >= 5)
				{
					num5 = Global.SafeConvertToInt32(cmdParams[4]);
				}
				Dictionary<int, CompMapData> dictionary = null;
				List<int> list = null;
				Dictionary<int, CompConfig> dictionary2 = null;
				lock (this.RuntimeData.Mutex)
				{
					list = this.CompSyncDataCache.ServerLineList;
					dictionary2 = this.RuntimeData.CompConfigDict;
					dictionary = this.CompSyncDataCache.CompMapDataDict;
				}
				CompConfig compConfig = null;
				int num6 = 0;
				bool flag2 = false;
				bool flag3 = false;
				foreach (CompConfig compConfig2 in dictionary2.Values)
				{
					if (compConfig2.MapCode == num)
					{
						num6 = compConfig2.ID;
						flag2 = true;
						compConfig = compConfig2;
					}
					if (compConfig2.MapCode == client.ClientData.MapCode)
					{
						flag3 = true;
					}
				}
				if (!flag2)
				{
					cmdData = -12;
					client.sendCmd<int>(nID, cmdData, false);
					return true;
				}
				if (client.ClientData.CompType <= 0)
				{
					cmdData = -12;
				}
				else if (!Global.CanEnterMap(client, num) || num == client.ClientData.MapCode)
				{
					cmdData = -12;
				}
				else if (!flag3 && !this.CheckMap(client) && !KuaFuMapManager.getInstance().IsKuaFuMap(client.ClientData.MapCode))
				{
					cmdData = -21;
				}
				else
				{
					if (num2 > 0 && num3 > 0)
					{
						if (Global.InObs(ObjectTypes.OT_CLIENT, num, num2, num3, 0, 0))
						{
							cmdData = -12;
							goto IL_48A;
						}
					}
					int mapCode = client.ClientData.MapCode;
					if (num4 > 0)
					{
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
						{
							cmdData = -3;
							goto IL_48A;
						}
						MapTeleport mapTeleport = null;
						if (!gameMap.MapTeleportDict.TryGetValue(num4, out mapTeleport) || mapTeleport.ToMapID != num)
						{
							cmdData = -12;
							goto IL_48A;
						}
						if (Global.GetTwoPointDistance(client.CurrentPos, new Point((double)mapTeleport.X, (double)mapTeleport.Y)) > 800.0)
						{
							cmdData = -301;
							goto IL_48A;
						}
					}
					CompMapData compMapData = null;
					if (dictionary == null || !dictionary.TryGetValue(num, out compMapData) || compMapData.roleNum >= compConfig.MaxPlayer)
					{
						cmdData = -22;
					}
					else if (list == null || list.Count < 3)
					{
						cmdData = -11003;
					}
					else
					{
						int serverId = list[num6 - 1];
						KuaFuServerInfo kuaFuServerInfo = null;
						if (!KuaFuManager.getInstance().TryGetValue(serverId, out kuaFuServerInfo))
						{
							cmdData = -11000;
						}
						else
						{
							bool flag4 = 0 == 0;
							KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
							if (null != clientKuaFuServerLoginData)
							{
								clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
								clientKuaFuServerLoginData.GameId = (long)num;
								clientKuaFuServerLoginData.EndTicks = 0L;
								clientKuaFuServerLoginData.ServerId = client.ServerId;
								clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
								clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
								if (num6 == 1)
								{
									clientKuaFuServerLoginData.GameType = 27;
								}
								else if (num6 == 2)
								{
									clientKuaFuServerLoginData.GameType = 28;
								}
								else if (num6 == 3)
								{
									clientKuaFuServerLoginData.GameType = 29;
								}
							}
							GlobalNew.RecordSwitchKuaFuServerLog(client);
							Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
							{
								mapCode,
								num4,
								num5,
								num2,
								num3
							}), "EnterKuaFuMapFlag", true);
							client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
						}
					}
				}
				IL_48A:
				client.sendCmd<int>(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetCompAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompScene compScene = client.SceneObject as CompScene;
				if (null == compScene)
				{
					return true;
				}
				RoleData4Selector roleData4Selector = null;
				this.OwnerRoleDataDict.TryGetValue(compScene.m_nMapCode, out roleData4Selector);
				int num = Convert.ToInt32(cmdParams[0]);
				client.sendCmd<CompDaLingZhuShowData>(nID, new CompDaLingZhuShowData
				{
					AdmireCount = Global.GetCompAdmireCount(client),
					RoleData4Selector = roleData4Selector
				}, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessCompAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CompScene compScene = client.SceneObject as CompScene;
				if (null == compScene)
				{
					return true;
				}
				RoleData4Selector roleData4Selector = null;
				this.OwnerRoleDataDict.TryGetValue(compScene.m_nMapCode, out roleData4Selector);
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				MoBaiData moBaiData = null;
				string cmdData;
				if (!Data.MoBaiDataInfoList.TryGetValue(7, out moBaiData))
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
				int compAdmireCount = Global.GetCompAdmireCount(client);
				if (roleData4Selector != null && client.ClientData.RoleID == roleData4Selector.RoleID)
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
				if (compAdmireCount >= num3)
				{
					cmdData = string.Format("{0}", -3);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				double num4;
				if (client.ClientData.ChangeLifeCount == 0)
				{
					num4 = 1.0;
				}
				else
				{
					num4 = Data.ChangeLifeEverydayExpRate[client.ClientData.ChangeLifeCount];
				}
				if (num2 == 1)
				{
					if (!Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜势力争霸大领主"))
					{
						cmdData = string.Format("{0}", -4);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					long num5 = (long)(num4 * (double)moBaiData.JinBiExpAward);
					if (num5 > 0L)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, num5, true, true, false, "none");
					}
					if (moBaiData.JinBiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.CompMoBai, 0);
					}
					if (moBaiData.LingJingAwardByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜势力争霸大领主", true, true, false);
					}
					if (moBaiData.ShenLiJingHuaByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByJinBi, "膜拜势力争霸大领主", true, true);
					}
				}
				else if (num2 == 2)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜势力争霸大领主", true, true, false, DaiBiSySType.None))
					{
						cmdData = string.Format("{0}", -5);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
					int num6 = (int)(num4 * (double)moBaiData.ZuanShiExpAward);
					if (num6 > 0)
					{
						GameManager.ClientMgr.ProcessRoleExperience(client, (long)num6, true, true, false, "none");
					}
					if (moBaiData.ZuanShiZhanGongAward > 0)
					{
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.CompMoBai, 0);
					}
					if (moBaiData.LingJingAwardByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜势力争霸大领主", true, true, false);
					}
					if (moBaiData.ShenLiJingHuaByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyShenLiJingHuaPointsValue(client, moBaiData.ShenLiJingHuaByZuanShi, "膜拜势力争霸大领主", true, true);
					}
				}
				Global.ProcessIncreaseCompAdmireCount(client);
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

		public void OnChangeName(int roleId, string oldName, string newName)
		{
			if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
			{
				string roleParamsFromDBByRoleID = Global.GetRoleParamsFromDBByRoleID(roleId, "10203", 0);
				int num = Global.SafeConvertToInt32(roleParamsFromDBByRoleID);
				if (num > 0)
				{
					TianTiClient.getInstance().Comp_ChangeName(roleId, newName);
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			this.UpdateMapBuffer(client);
			List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
			if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count >= 5)
			{
				Global.SaveRoleParamsIntListToDB(client, new List<int>(new int[]
				{
					roleParamsIntListFromDB[0],
					0,
					0,
					0,
					0
				}), "EnterKuaFuMapFlag", true);
			}
		}

		private void UpdateMapBuffer(GameClient client)
		{
			int num = 0;
			CompMapClientContextData compMapClientContextData = client.SceneContextData2 as CompMapClientContextData;
			if (compMapClientContextData != null && client.ClientData.CompType > 0 && client.ClientData.CompType == compMapClientContextData.BattleWhichSide)
			{
				KFCompData kfcompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out kfcompData);
					num = ((kfcompData != null) ? kfcompData.EnemyCompType : 0);
				}
			}
			if (num < 1 || num > 3)
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.CompEnemy, actionParams, 1, false);
			}
			else
			{
				double[] actionParams = new double[]
				{
					(double)this.RuntimeData.CompEnemy[num - 1]
				};
				Global.UpdateBufferData(client, BufferItemTypes.CompEnemy, actionParams, 1, false);
			}
		}

		public int FilterCompEnemyInjure(GameClient client, GameClient enemy, int injure)
		{
			CompMapClientContextData compMapClientContextData = client.SceneContextData2 as CompMapClientContextData;
			int result;
			if (compMapClientContextData == null || client.ClientData.CompType <= 0 || client.ClientData.CompType != compMapClientContextData.BattleWhichSide)
			{
				result = injure;
			}
			else
			{
				KFCompData kfcompData = null;
				lock (this.RuntimeData.Mutex)
				{
					this.CompSyncDataCache.CompDataDict.V.TryGetValue(client.ClientData.CompType, out kfcompData);
				}
				if (kfcompData == null || kfcompData.EnemyCompType == 0 || enemy.ClientData.CompType != kfcompData.EnemyCompType)
				{
					result = injure;
				}
				else
				{
					result = (int)((double)injure * (1.0 + this.RuntimeData.CompEnemyHurtNum));
				}
			}
			return result;
		}

		private int GetZhiWuByRankJunXianLast(int compType, int rid)
		{
			List<KFCompRankInfo> list = new List<KFCompRankInfo>();
			lock (this.RuntimeData.Mutex)
			{
				this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(compType, out list);
			}
			int result;
			if (list == null || list.Count == 0)
			{
				result = 0;
			}
			else
			{
				int num = list.FindIndex((KFCompRankInfo x) => x.Key == rid) + 1;
				result = ((num > 5) ? 0 : num);
			}
			return result;
		}

		public void UpdateBuff4GameClient(GameClient client, BufferItemTypes bufferItem, int bufferGoodsID, bool add)
		{
			EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(bufferGoodsID);
			if (null != equipPropItem)
			{
				if (add)
				{
					double[] actionParams = new double[]
					{
						(double)bufferGoodsID
					};
					Global.UpdateBufferData(client, bufferItem, actionParams, 1, true);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferGoodsID,
						equipPropItem.ExtProps
					});
				}
				else
				{
					Global.RemoveBufferData(client, (int)bufferItem);
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferGoodsID,
						PropsCacheManager.ConstExtProps
					});
				}
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					default(DelayExecProcIds),
					2
				});
			}
		}

		private void UpdateChengHaoBuffer(GameClient client)
		{
			if (client.ClientData.CompType > 0)
			{
				List<KFCompRankInfo> list = null;
				lock (this.RuntimeData.Mutex)
				{
					list = this.CompSyncDataCache.CompRankBossDamageList.V;
				}
				int zhiWuByRankJunXianLast = this.GetZhiWuByRankJunXianLast(client.ClientData.CompType, client.ClientData.RoleID);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_1, 2000817, false);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_2, 2000818, false);
				this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_3, 2000819, false);
				if (client.ClientData.CompType > 0 && list.Count >= 3)
				{
					if (list[0].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_1, 2000817, true);
					}
					if (list[1].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_2, 2000818, true);
					}
					if (list[2].Value == client.ClientData.RoleID)
					{
						this.UpdateBuff4GameClient(client, BufferItemTypes.CompBossKiller_3, 2000819, true);
					}
				}
				if (client.ClientData.CompType > 0 && zhiWuByRankJunXianLast != (int)client.ClientData.CompZhiWu)
				{
					client.ClientData.CompZhiWu = (byte)zhiWuByRankJunXianLast;
					client.sendCmd(1135, string.Format("{0}:{1}", client.ClientData.RoleID, zhiWuByRankJunXianLast), false);
				}
				Global.RemoveBufferData(client, 9003);
				Global.RemoveBufferData(client, 9004);
				Global.RemoveBufferData(client, 9005);
				Global.RemoveBufferData(client, 9006);
				Global.RemoveBufferData(client, 9007);
				Global.RemoveBufferData(client, 9008);
				Global.RemoveBufferData(client, 9009);
				Global.RemoveBufferData(client, 9010);
				Global.RemoveBufferData(client, 9011);
				if (zhiWuByRankJunXianLast > 0 && client.ClientData.CompType > 0)
				{
					double[] actionParams = new double[]
					{
						1.0
					};
					if (client.ClientData.CompType == 1)
					{
						if (zhiWuByRankJunXianLast == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_1, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_2, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_1_3, actionParams, 1, false);
						}
					}
					if (client.ClientData.CompType == 2)
					{
						if (zhiWuByRankJunXianLast == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_1, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_2, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_2_3, actionParams, 1, false);
						}
					}
					if (client.ClientData.CompType == 3)
					{
						if (zhiWuByRankJunXianLast == 1)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_1, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 2)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_2, actionParams, 1, false);
						}
						if (zhiWuByRankJunXianLast == 3)
						{
							Global.UpdateBufferData(client, BufferItemTypes.CompJunXian_3_3, actionParams, 1, false);
						}
					}
				}
			}
		}

		public bool NoticeCoolDown(CompScene scene, int noticeID)
		{
			lock (this.RuntimeData.Mutex)
			{
				CoolDownItem coolDownItem = null;
				if (!scene.CompNoticeCoolDownDict.TryGetValue(noticeID, out coolDownItem))
				{
					return true;
				}
				long num = TimeUtil.NOW();
				if (num > coolDownItem.StartTicks + coolDownItem.CDTicks)
				{
					return true;
				}
			}
			return false;
		}

		public void AddNoticeCoolDown(CompScene scene, int noticeID)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompNoticeConfig compNoticeConfig = null;
				if (this.RuntimeData.CompNoticeConfigDict.TryGetValue(noticeID, out compNoticeConfig))
				{
					CoolDownItem coolDownItem = null;
					scene.CompNoticeCoolDownDict.TryGetValue(noticeID, out coolDownItem);
					long num = TimeUtil.NOW();
					long num2 = (long)(compNoticeConfig.CDTime * 1000);
					if (null == coolDownItem)
					{
						coolDownItem = new CoolDownItem
						{
							ID = noticeID,
							StartTicks = num,
							CDTicks = num2
						};
						scene.CompNoticeCoolDownDict[noticeID] = coolDownItem;
					}
					else if (num + num2 > coolDownItem.StartTicks + coolDownItem.CDTicks)
					{
						coolDownItem.StartTicks = num;
						coolDownItem.CDTicks = num2;
					}
				}
			}
		}

		public void CompChat(GameClient client, string text)
		{
			long num = TimeUtil.NOW();
			List<CompLevelConfig> list = null;
			lock (this.RuntimeData.Mutex)
			{
				list = this.RuntimeData.CompLevelConfigList;
			}
			CompLevelConfig compLevelConfig = list.Find((CompLevelConfig x) => x.CompID == client.ClientData.CompType && x.Level == (int)client.ClientData.CompZhiWu);
			if (null != compLevelConfig)
			{
				int num2 = Math.Max(compLevelConfig.TalkCD * 1000, 3000);
				if (num2 > 0 && num - client.ClientData.LastCompChatTicks < (long)num2)
				{
					long num3 = ((long)num2 - (num - client.ClientData.LastCompChatTicks)) / 1000L + 1L;
					GameManager.ClientMgr.NotifyHintMsg(client, string.Format(GLang.GetLang(4003, new object[0]), num3));
				}
				else
				{
					client.ClientData.LastCompChatTicks = num;
					KFCompChat kfcompChat = new KFCompChat(client.ClientData.ZoneID, client.ClientData.RoleName, text, client.ClientData.CompType);
					lock (this.RuntimeData.Mutex)
					{
						this.RuntimeData.CompChatList.Add(kfcompChat);
					}
					this.BroadcastCompChatMsg(kfcompChat);
				}
			}
		}

		public void BroadcastCompChatMsg(KFCompChat kfChat)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (gameClient != null && gameClient.ClientData.CompType == kfChat.CompType)
				{
					gameClient.sendCmd(157, kfChat.Text, false);
				}
			}
		}

		public void OnChatListData(byte[] data)
		{
			if (null != data)
			{
				List<KFCompChat> list = DataHelper.BytesToObject<List<KFCompChat>>(data, 0, data.Length);
				if (null != list)
				{
					foreach (KFCompChat kfChat in list)
					{
						this.BroadcastCompChatMsg(kfChat);
					}
				}
			}
		}

		public void CompNotice(CompScene scene, KFCompNotice notice)
		{
			if (this.NoticeCoolDown(scene, notice.NoticeID))
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.CompNoticeList.Add(notice);
				}
				this.BroadcastCompNoticeMsg(notice);
				this.AddNoticeCoolDown(scene, notice.NoticeID);
			}
		}

		public void OnNoticeListData(byte[] data)
		{
			if (null != data)
			{
				List<KFCompNotice> list = DataHelper.BytesToObject<List<KFCompNotice>>(data, 0, data.Length);
				if (null != list)
				{
					foreach (KFCompNotice kfNotice in list)
					{
						this.BroadcastCompNoticeMsg(kfNotice);
					}
				}
			}
		}

		public void BroadcastCompNoticeMsg(KFCompNotice kfNotice)
		{
			CompNoticeConfig compNoticeConfig = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CompNoticeConfigDict.TryGetValue(kfNotice.NoticeID, out compNoticeConfig))
				{
					return;
				}
			}
			if (!GameManager.IsKuaFuServer)
			{
				bool flag2 = false;
				if (compNoticeConfig.OriginalMapOpen != null && compNoticeConfig.OriginalMapOpen.Contains(1))
				{
					flag2 = true;
				}
				if (!flag2)
				{
					return;
				}
			}
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (compNoticeConfig.Range > 0)
				{
					gameClient.sendCmd<KFCompNotice>(1131, kfNotice, false);
				}
				else if ((kfNotice.CompType > 0 && gameClient.ClientData.CompType == kfNotice.CompType) || kfNotice.CompType <= 0)
				{
					gameClient.sendCmd<KFCompNotice>(1131, kfNotice, false);
				}
			}
		}

		public int GetDayDuiHuanNum(GameClient client, int DuiHuanNum)
		{
			int result;
			if (client.ClientData.CompType <= 0)
			{
				result = DuiHuanNum;
			}
			else
			{
				List<int> yestdBoomValueList = this.GetYestdBoomValueList();
				int num = yestdBoomValueList[client.ClientData.CompType - 1];
				int num2 = 1;
				foreach (Tuple<int, int> tuple in this.RuntimeData.CompShop)
				{
					if (num >= tuple.Item1 && tuple.Item2 > num2)
					{
						num2 = tuple.Item2;
					}
				}
				result = num2 * DuiHuanNum;
			}
			return result;
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			CompScene compScene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
			{
				CompResourcesConfig compResourcesConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					compResourcesConfig = (monster.Tag as CompResourcesConfig);
					if (compResourcesConfig == null)
					{
						return;
					}
					monster.Tag = null;
					DateTime dateTime = TimeUtil.NowDateTime();
					if (dateTime.TimeOfDay >= compResourcesConfig.RefreshTimeBegin && dateTime.TimeOfDay <= compResourcesConfig.RefreshTimeEnd)
					{
						compScene.ResourceGrowUpNum--;
						compResourcesConfig.ResourceState = 1;
						this.AddDelayCreateMonster(compScene, TimeUtil.NOW(), compResourcesConfig);
					}
					else
					{
						compScene.ResourceNum--;
						compScene.ResourceGrowUpNum--;
						compResourcesConfig.ResourceState = 0;
					}
				}
				if (compResourcesConfig.CompDonate > 0)
				{
					GameManager.ClientMgr.ModifyCompDonateValue(client, compResourcesConfig.CompDonate, "采集", true, true, false);
				}
				if (compResourcesConfig.JunXian > 0)
				{
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 1, client.ClientData.RoleID, compResourcesConfig.JunXian);
					string msgText = string.Format(GLang.GetLang(4017, new object[0]), compResourcesConfig.JunXian);
					GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				}
				if (compResourcesConfig.BoomValue > 0)
				{
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 0, compResourcesConfig.BoomValue, 0);
					TianTiClient.getInstance().Comp_CompOpt(client.ClientData.CompType, 6, compResourcesConfig.BoomValue, 0);
					string msgText = string.Format(GLang.GetLang(4018, new object[0]), compResourcesConfig.BoomValue);
					GameManager.ClientMgr.NotifyImportantMsg(client, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					if (compScene.CompSceneInfo.ID != client.ClientData.CompType)
					{
						TianTiClient.getInstance().Comp_CompOpt(compScene.CompSceneInfo.ID, 2, client.ClientData.CompType, compResourcesConfig.BoomValue);
						KFCompNotice notice = new KFCompNotice
						{
							NoticeID = 2,
							CompType = compScene.CompSceneInfo.ID,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							toMapCode = compScene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(compScene, notice);
					}
				}
				compScene.SaveCompSceneDBInfo();
			}
		}

		public void OnProcessBossRealive(Monster monster)
		{
			if (401 == monster.MonsterType)
			{
				lock (this.RuntimeData.Mutex)
				{
					CompScene compScene;
					if (this.SceneDict.TryGetValue(monster.MonsterZoneNode.MapCode, out compScene))
					{
						foreach (CompMapClientContextData compMapClientContextData in compScene.ClientContextDataDict.Values)
						{
							compMapClientContextData.TotalScore = 0L;
							compMapClientContextData.InjureBossDeltaDict.Remove(monster.MonsterInfo.ExtensionID);
						}
						MonsterZone dynamicMonsterZone = GameManager.MonsterZoneMgr.GetDynamicMonsterZone(monster.MonsterZoneNode.MapCode);
						if (dynamicMonsterZone != null && this.RuntimeData.CompBossRealive != null && this.RuntimeData.CompBossRealive.Length == 2)
						{
							Monster dynamicMonsterSeed = GameManager.MonsterZoneMgr.GetDynamicMonsterSeed(monster.MonsterInfo.ExtensionID);
							if (dynamicMonsterSeed != null && monster.LastDeadTicks > 0L)
							{
								MonsterStaticInfo monsterStaticInfo = dynamicMonsterSeed.MonsterInfo.Clone();
								double num = monster.MonsterInfo.VLifeMax / monsterStaticInfo.VLifeMax;
								double num2 = num;
								long num3 = monster.LastDeadTicks - monster.GetMonsterBirthTick();
								long num4 = (long)this.RuntimeData.CompBossRealive[0] * 10000000L;
								if (num3 > num4)
								{
									num = Math.Max(num * 0.9, 1.0);
								}
								else
								{
									num += (double)(num4 - num3) / 10000000.0 * this.RuntimeData.CompBossRealive[1];
								}
								string text = string.Format("势力Boss刷新 势力ID:{0} Before血量:{1} Before系数:{2} 存活时间:{3}s After系数:{4} After血量:{5}", new object[]
								{
									compScene.CompSceneInfo.ID,
									monster.MonsterInfo.VLifeMax,
									num2,
									num3 / 10000000L,
									num,
									monster.MonsterInfo.VLifeMax * num
								});
								LogManager.WriteLog(5, text, null, true);
								monsterStaticInfo.VLifeMax *= num;
								monster.MonsterInfo = monsterStaticInfo;
								compScene.BossMaxLifeFactor = num;
								compScene.SaveCompSceneDBInfo();
							}
							else if (null != dynamicMonsterSeed)
							{
								MonsterStaticInfo monsterStaticInfo = dynamicMonsterSeed.MonsterInfo.Clone();
								monsterStaticInfo.VLifeMax *= compScene.BossMaxLifeFactor;
								monster.MonsterInfo = monsterStaticInfo;
								string text = string.Format("势力Boss刷新 势力ID:{0} 血量:{1} 系数:{2}", compScene.CompSceneInfo.ID, monster.MonsterInfo.VLifeMax, compScene.BossMaxLifeFactor);
								LogManager.WriteLog(5, text, null, true);
							}
						}
						compScene.ScoreData.Score1 = 0L;
						compScene.ScoreData.Score2 = 0L;
						compScene.ScoreData.Score3 = 0L;
						compScene.ScoreData.BossMaxLifeV = (long)monster.MonsterInfo.VLifeMax;
						List<object> mapClients = GameManager.ClientMgr.GetMapClients(compScene.m_nMapCode);
						if (mapClients != null && mapClients.Count != 0)
						{
							for (int i = 0; i < mapClients.Count; i++)
							{
								GameClient gameClient = mapClients[i] as GameClient;
								if (gameClient != null)
								{
									this.NotifyScoreInfo(gameClient, true, true);
								}
							}
						}
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			CompScene compScene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
			{
				lock (this.RuntimeData.Mutex)
				{
					if (other.ClientData.CompType == compScene.CompSceneInfo.ID)
					{
						KFCompNotice notice = new KFCompNotice
						{
							NoticeID = 3,
							CompType = compScene.CompSceneInfo.ID,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							Param2 = Global.FormatRoleNameWithZoneId2(other),
							toMapCode = compScene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(compScene, notice);
					}
					if (other.ClientData.CompZhiWu == 1)
					{
						if (other.ClientData.CompType == compScene.CompSceneInfo.ID)
						{
							KFCompNotice notice = new KFCompNotice
							{
								NoticeID = 4,
								CompType = compScene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								Param2 = Global.FormatRoleNameWithZoneId2(other),
								toMapCode = compScene.m_nMapCode,
								toPosX = client.ClientData.PosX,
								toPosY = client.ClientData.PosY
							};
							this.CompNotice(compScene, notice);
						}
						KFCompNotice notice2 = new KFCompNotice
						{
							NoticeID = 6,
							CompType = client.ClientData.CompType,
							Param1 = Global.FormatRoleNameWithZoneId2(client),
							Param2 = Global.FormatRoleNameWithZoneId2(other),
							toMapCode = compScene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(compScene, notice2);
					}
				}
			}
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			CompScene compScene;
			if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
			{
				lock (this.RuntimeData.Mutex)
				{
					if (401 == monster.MonsterType && monster.MonsterInfo.ExtensionID == compScene.CompSceneInfo.BossID)
					{
						int num = this.RuntimeData.CompBossCompNum[0];
						int num2 = this.RuntimeData.CompBossCompNum[1];
						int num3 = this.RuntimeData.CompBossCompHonor[0];
						int num4 = this.RuntimeData.CompBossCompHonor[1];
						int num5 = this.RuntimeData.CompBossCompHonor[2];
						int num6 = this.RuntimeData.CompBossCompHonor[3];
						List<Tuple<int, long>> list = new List<Tuple<int, long>>();
						foreach (CompMapClientContextData compMapClientContextData in compScene.ClientContextDataDict.Values)
						{
							long num7 = 0L;
							if (compMapClientContextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out num7) && num7 > 0L)
							{
								list.Add(new Tuple<int, long>(compMapClientContextData.RoleId, num7));
								long num8 = Math.Min(num7 / (long)num, (long)num2);
								long num9 = Math.Min(num7 / (long)num3, (long)num5);
								long num10 = Math.Min(num7 / (long)num4, (long)num6);
								if (num8 > 0L)
								{
									TianTiClient.getInstance().Comp_CompOpt(compMapClientContextData.BattleWhichSide, 0, (int)num8, 0);
									TianTiClient.getInstance().Comp_CompOpt(compMapClientContextData.BattleWhichSide, 5, (int)num8, 0);
									if (compScene.CompSceneInfo.ID != compMapClientContextData.BattleWhichSide)
									{
										TianTiClient.getInstance().Comp_CompOpt(compScene.CompSceneInfo.ID, 2, compMapClientContextData.BattleWhichSide, (int)num8);
									}
								}
								GameClient gameClient = GameManager.ClientMgr.FindClient(compMapClientContextData.RoleId);
								if (null != gameClient)
								{
									if (num9 > 0L)
									{
										GameManager.ClientMgr.ModifyCompDonateValue(gameClient, (int)num9, "Boss", true, true, false);
									}
									if (num10 > 0L)
									{
										TianTiClient.getInstance().Comp_CompOpt(gameClient.ClientData.CompType, 1, compMapClientContextData.RoleId, (int)num10);
										string msgText = string.Format(GLang.GetLang(4017, new object[0]), num10);
										GameManager.ClientMgr.NotifyImportantMsg(gameClient, msgText, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
									}
								}
							}
						}
						TianTiClient.getInstance().Comp_CompOpt(compScene.CompSceneInfo.ID, 4, client.ClientData.CompType, 0);
						list.Sort(delegate(Tuple<int, long> left, Tuple<int, long> righit)
						{
							int result;
							if (left.Item2 < righit.Item2)
							{
								result = 1;
							}
							else if (left.Item2 > righit.Item2)
							{
								result = -1;
							}
							else
							{
								result = 0;
							}
							return result;
						});
						if (list.Count != 0)
						{
							TianTiClient.getInstance().Comp_CompOpt(compScene.CompSceneInfo.ID, 7, list[0].Item1, 0);
						}
						KFCompNotice notice = new KFCompNotice
						{
							NoticeID = 7,
							CompType = compScene.CompSceneInfo.ID,
							Param1 = compScene.CompSceneInfo.CompName,
							toMapCode = compScene.m_nMapCode,
							toPosX = client.ClientData.PosX,
							toPosY = client.ClientData.PosY
						};
						this.CompNotice(compScene, notice);
					}
					CompSolderSiteConfig compSolderSiteConfig = monster.Tag as CompSolderSiteConfig;
					if (null != compSolderSiteConfig)
					{
						monster.Tag = null;
						compSolderSiteConfig.MonsterState = 0;
						compScene.SolderNum--;
						compScene.SaveCompSceneDBInfo();
					}
				}
			}
		}

		public CompMapClientContextData GetBossTopDamageClientContext(Monster monster)
		{
			CompMapClientContextData compMapClientContextData = null;
			CompScene compScene;
			CompMapClientContextData result;
			if (!this.SceneDict.TryGetValue(monster.MonsterZoneNode.MapCode, out compScene))
			{
				result = compMapClientContextData;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					List<Tuple<int, long>> list = new List<Tuple<int, long>>();
					foreach (CompMapClientContextData compMapClientContextData2 in compScene.ClientContextDataDict.Values)
					{
						long num = 0L;
						if (compMapClientContextData2.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out num) && num > 0L)
						{
							list.Add(new Tuple<int, long>(compMapClientContextData2.RoleId, num));
						}
					}
					list.Sort(delegate(Tuple<int, long> left, Tuple<int, long> righit)
					{
						int result2;
						if (left.Item2 < righit.Item2)
						{
							result2 = 1;
						}
						else if (left.Item2 > righit.Item2)
						{
							result2 = -1;
						}
						else
						{
							result2 = 0;
						}
						return result2;
					});
					if (list.Count != 0)
					{
						compScene.ClientContextDataDict.TryGetValue(list[0].Item1, out compMapClientContextData);
					}
				}
				result = compMapClientContextData;
			}
			return result;
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			CompResourcesConfig compResourcesConfig = (monster != null) ? (monster.Tag as CompResourcesConfig) : null;
			int result;
			if (compResourcesConfig == null)
			{
				result = -200;
			}
			else if (client.ClientData.CompType <= 0)
			{
				result = -12;
			}
			else
			{
				bool flag = false;
				long num = TimeUtil.NOW();
				if (num >= monster.GetMonsterBirthTick() / 10000L + (long)(compResourcesConfig.GrowTime * 1000))
				{
					flag = true;
				}
				if (compResourcesConfig.ResourceState == 2)
				{
					flag = true;
				}
				if (!flag)
				{
					result = -2007;
				}
				else
				{
					result = compResourcesConfig.CollectTime;
				}
			}
			return result;
		}

		public void OnInjureMonster(GameClient client, Monster monster, long injure)
		{
			if (401 == monster.MonsterType)
			{
				CompMapClientContextData compMapClientContextData = client.SceneContextData2 as CompMapClientContextData;
				if (null != compMapClientContextData)
				{
					CompScene compScene;
					if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
					{
						lock (this.RuntimeData.Mutex)
						{
							long num = 0L;
							compMapClientContextData.InjureBossDeltaDict.TryGetValue(monster.MonsterInfo.ExtensionID, out num);
							num += injure;
							compMapClientContextData.InjureBossDeltaDict[monster.MonsterInfo.ExtensionID] = num;
							compMapClientContextData.TotalScore += injure;
							if (client.ClientData.CompType == 1)
							{
								compScene.ScoreData.Score1 += injure;
							}
							else if (client.ClientData.CompType == 2)
							{
								compScene.ScoreData.Score2 += injure;
							}
							else if (client.ClientData.CompType == 3)
							{
								compScene.ScoreData.Score3 += injure;
							}
						}
						List<object> mapClients = GameManager.ClientMgr.GetMapClients(compScene.m_nMapCode);
						if (mapClients != null && mapClients.Count != 0)
						{
							for (int i = 0; i < mapClients.Count; i++)
							{
								GameClient gameClient = mapClients[i] as GameClient;
								if (gameClient != null)
								{
									gameClient.sendCmd<CompBattleScoreData>(1133, compScene.ScoreData, false);
								}
							}
						}
						this.NotifyScoreInfo(client, true, true);
						if (compScene.CompSceneInfo.ID != client.ClientData.CompType)
						{
							KFCompNotice notice = new KFCompNotice
							{
								NoticeID = 5,
								CompType = compScene.CompSceneInfo.ID,
								Param1 = Global.FormatRoleNameWithZoneId2(client),
								toMapCode = compScene.m_nMapCode,
								toPosX = client.ClientData.PosX,
								toPosY = client.ClientData.PosY
							};
							this.CompNotice(compScene, notice);
						}
					}
				}
			}
		}

		public void NotifyScoreInfo(GameClient client, bool sideScore = true, bool selfScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				CompScene compScene;
				if (this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
				{
					if (sideScore)
					{
						client.sendCmd<CompBattleScoreData>(1133, compScene.ScoreData, false);
					}
					if (selfScore)
					{
						CompMapClientContextData compMapClientContextData = client.SceneContextData2 as CompMapClientContextData;
						if (null != compMapClientContextData)
						{
							client.sendCmd<long>(1134, compMapClientContextData.TotalScore, false);
						}
					}
				}
			}
		}

		public void HandleCompTaskSomething(GameClient client, bool login = false)
		{
			if (client.ClientData.CompType > 0)
			{
				List<TaskData> list = new List<TaskData>();
				foreach (TaskData taskData in client.ClientData.TaskDataList)
				{
					SystemXmlItem systemXmlItem = null;
					if (GameManager.SystemTasksMgr.SystemXmlItemDict.TryGetValue(taskData.DoingTaskID, out systemXmlItem))
					{
						int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
						if (intValue >= 100 && intValue <= 150)
						{
							int intValue2 = systemXmlItem.GetIntValue("CompID", -1);
							if (intValue2 > 0 && client.ClientData.CompType != intValue2)
							{
								list.Add(taskData);
							}
							string strB = new DateTime(taskData.AddDateTime * 10000L).ToString("yyyy-MM-dd");
							DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, intValue);
							if (dailyTaskData == null || string.Compare(dailyTaskData.RecTime, strB) > 0)
							{
								list.Add(taskData);
							}
						}
					}
				}
				foreach (TaskData taskData in list)
				{
					bool flag = Global.CancelTask(client, taskData.DbID, taskData.DoingTaskID);
					if (!login && flag)
					{
						client.sendCmd(154, string.Format("{0}:{1}:{2}:{3}", new object[]
						{
							client.ClientData.RoleID,
							taskData.DbID,
							taskData.DoingTaskID,
							0
						}), false);
					}
				}
				Dictionary<int, List<int>> dictionary = new Dictionary<int, List<int>>();
				List<int> list2 = new List<int>();
				foreach (int key in GameManager.SystemTasksMgr.SystemXmlItemDict.Keys)
				{
					SystemXmlItem systemXmlItem = GameManager.SystemTasksMgr.SystemXmlItemDict[key];
					int num = systemXmlItem.GetIntValue("ID", -1);
					if (-1 != num)
					{
						int intValue = systemXmlItem.GetIntValue("TaskClass", -1);
						if (intValue >= 100 && intValue <= 150)
						{
							if (Global.CanTaskPaoHuanTask(client, intValue))
							{
								List<int> list3 = null;
								if (!dictionary.TryGetValue(intValue, out list3))
								{
									list3 = new List<int>();
									dictionary[intValue] = list3;
								}
								list3.Add(num);
							}
						}
					}
				}
				foreach (KeyValuePair<int, List<int>> keyValuePair in dictionary)
				{
					DailyTaskData dailyTaskData = Global.FindDailyTaskDataByTaskClass(client, keyValuePair.Key);
					if (null != dailyTaskData)
					{
						List<int> list4 = null;
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CompTaskBeginDict.TryGetValue(keyValuePair.Key, out list4);
						}
						if (list4 != null && list4.Count >= 3)
						{
							int num = list4[client.ClientData.CompType - 1] + dailyTaskData.RecNum;
							if (keyValuePair.Value.Contains(num))
							{
								TCPOutPacket tcpoutPacket = null;
								TCPProcessCmdResults tcpprocessCmdResults = Global.TakeNewTask(TCPManager.getInstance(), client.ClientSocket, TCPManager.getInstance().tcpClientPool, TCPManager.getInstance().tcpRandKey, TCPManager.getInstance().TcpOutPacketPool, 125, client, client.ClientData.RoleID, num, -1, out tcpoutPacket);
								if (!login && tcpprocessCmdResults == TCPProcessCmdResults.RESULT_DATA && null != tcpoutPacket)
								{
									client.sendCmd(tcpoutPacket, true);
								}
							}
						}
					}
				}
			}
		}

		private bool CheckMap(GameClient client)
		{
			SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
			return mapSceneType == 0;
		}

		private List<int> ComputerRecommendCompList(List<int> YestdBoomValueList)
		{
			List<int> list = new List<int>();
			List<int> result;
			if (YestdBoomValueList == null || YestdBoomValueList.Count == 0)
			{
				result = list;
			}
			else
			{
				int num = YestdBoomValueList.Max();
				if (0 == num)
				{
					result = list;
				}
				else
				{
					for (int i = 1; i <= 3; i++)
					{
						if ((double)YestdBoomValueList[i - 1] < (double)num * this.RuntimeData.CompRecommendRatio)
						{
							list.Add(i);
						}
					}
					result = list;
				}
			}
			return result;
		}

		public bool IfInMyselfCompMap(GameClient client)
		{
			CompScene compScene = client.SceneObject as CompScene;
			bool result;
			if (null == compScene)
			{
				result = false;
			}
			else
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				result = (mapSceneType == 48 && compScene.CompSceneInfo.ID == client.ClientData.CompType);
			}
			return result;
		}

		public bool IfTopBoomCompType(GameClient client, int compType, bool self = true)
		{
			bool result;
			if (client.ClientData.CompType < 1 || client.ClientData.CompType > 3)
			{
				result = false;
			}
			else if (compType < 1 || compType > 3)
			{
				result = false;
			}
			else
			{
				List<int> yestdBoomValueList = this.GetYestdBoomValueList();
				int num = yestdBoomValueList[compType - 1];
				for (int i = 0; i < yestdBoomValueList.Count; i++)
				{
					if (self || i + 1 != client.ClientData.CompType)
					{
						if (num < yestdBoomValueList[i])
						{
							return false;
						}
					}
				}
				result = true;
			}
			return result;
		}

		public int GetTaskClassNum()
		{
			int count;
			lock (this.RuntimeData.Mutex)
			{
				count = this.RuntimeData.MaxDailyTaskNumDict.Count;
			}
			return count;
		}

		public int GetMaxDailyTaskNum(int taskClass)
		{
			int result = 0;
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.MaxDailyTaskNumDict.TryGetValue(taskClass, out result);
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int posX;
			int posY;
			int birthPoint = this.GetBirthPoint(client.SceneObject as CompScene, client, out posX, out posY);
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

		public int GetBirthPoint(CompScene sceneInfo, GameClient client, out int posX, out int posY)
		{
			int battleWhichSide = client.ClientData.BattleWhichSide;
			lock (this.RuntimeData.Mutex)
			{
				CompConfig compConfig = (sceneInfo != null) ? sceneInfo.CompSceneInfo : null;
				if (null != compConfig)
				{
					Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, compConfig.BirthPosX[battleWhichSide - 1], compConfig.BirthPosY[battleWhichSide - 1], compConfig.BirthRadius[battleWhichSide - 1]);
					posX = (int)mapPoint.X;
					posY = (int)mapPoint.Y;
					return battleWhichSide;
				}
			}
			posX = 0;
			posY = 0;
			return -1;
		}

		public bool OnInitGameKuaFu(GameClient client)
		{
			client.ClientData.CompType = GameManager.ClientMgr.GetCompType(client);
			bool result;
			if (client.ClientData.CompType <= 0)
			{
				result = false;
			}
			else
			{
				CompConfig compConfig = null;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.CompConfigDict.TryGetValue(client.ClientData.CompType, out compConfig))
					{
						return false;
					}
				}
				KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
				client.ClientData.MapCode = (int)clientKuaFuServerLoginData.GameId;
				client.ClientData.BattleWhichSide = client.ClientData.CompType;
				CompScene compScene;
				if (!this.SceneDict.TryGetValue(client.ClientData.MapCode, out compScene))
				{
					result = false;
				}
				else
				{
					bool flag2 = false;
					List<int> roleParamsIntListFromDB = Global.GetRoleParamsIntListFromDB(client, "EnterKuaFuMapFlag");
					if (roleParamsIntListFromDB != null && roleParamsIntListFromDB.Count >= 5)
					{
						int num = roleParamsIntListFromDB[0];
						int num2 = roleParamsIntListFromDB[1];
						int num3 = roleParamsIntListFromDB[2];
						int num4 = roleParamsIntListFromDB[3];
						int num5 = roleParamsIntListFromDB[4];
						if (compConfig.MapCode == client.ClientData.MapCode)
						{
							if (num4 > 0 && num5 > 0)
							{
								Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, num4, num5, 60);
								client.ClientData.PosX = (int)mapPoint.X;
								client.ClientData.PosY = (int)mapPoint.Y;
							}
							else if (num3 > 0)
							{
								int toX;
								int toY;
								int radius;
								if (GameManager.MonsterZoneMgr.GetMonsterBirthPoint(client.ClientData.MapCode, num3, out toX, out toY, out radius))
								{
									radius = 1;
									Point mapPoint2 = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, toX, toY, radius);
									client.ClientData.PosX = (int)mapPoint2.X;
									client.ClientData.PosY = (int)mapPoint2.Y;
								}
							}
							else
							{
								flag2 = true;
							}
						}
						else
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						int posX = 0;
						int posY = 0;
						if (this.GetBirthPoint(compScene, client, out posX, out posY) <= 0)
						{
							LogManager.WriteLog(2, string.Format("找不到出生点mapcode={0},side={1}", client.ClientData.MapCode, client.ClientData.BattleWhichSide), null, true);
							return false;
						}
						client.ClientData.PosX = posX;
						client.ClientData.PosY = posY;
					}
					lock (this.RuntimeData.Mutex)
					{
						CompMapClientContextData compMapClientContextData;
						if (!compScene.ClientContextDataDict.TryGetValue(client.ClientData.RoleID, out compMapClientContextData))
						{
							compMapClientContextData = new CompMapClientContextData();
							compScene.ClientContextDataDict[client.ClientData.RoleID] = compMapClientContextData;
						}
						compMapClientContextData.RoleId = client.ClientData.RoleID;
						compMapClientContextData.ServerId = client.ServerId;
						compMapClientContextData.BattleWhichSide = client.ClientData.BattleWhichSide;
						compMapClientContextData.RoleName = client.ClientData.RoleName;
						compMapClientContextData.Occupation = client.ClientData.Occupation;
						compMapClientContextData.RoleSex = client.ClientData.RoleSex;
						compMapClientContextData.ZoneID = client.ClientData.ZoneID;
						client.SceneContextData2 = compMapClientContextData;
						client.SceneObject = compScene;
					}
					result = true;
				}
			}
			return result;
		}

		public void OnInitGameAhead(GameClient client)
		{
			client.ClientData.CompType = GameManager.ClientMgr.GetCompType(client);
			client.ClientData.CompZhiWu = (byte)this.GetZhiWuByRankJunXianLast(client.ClientData.CompType, client.ClientData.RoleID);
		}

		public void OnInitGame(GameClient client)
		{
			this.HandleCompTaskSomething(client, true);
			this.UpdateChengHaoBuffer(client);
		}

		private void TimerProc(object sender, EventArgs e)
		{
			try
			{
				lock (this.RuntimeData.Mutex)
				{
					CompSyncData compSyncData = TianTiClient.getInstance().Comp_SyncData(this.CompSyncDataCache.CompDataDict.Age, this.CompSyncDataCache.CompRankJunXianDict.Age, this.CompSyncDataCache.CompRankJunXianLastDict.Age, this.CompSyncDataCache.CompRankBossDamageList.Age, this.CompSyncDataCache.CompRankBattleJiFenDict.Age, this.CompSyncDataCache.CompRankMineJiFenDict.Age);
					if (null == compSyncData)
					{
						return;
					}
					this.CompSyncDataCache.ServerLineList = compSyncData.ServerLineList;
					if (null != compSyncData.BytesCompMapDataDict)
					{
						this.CompSyncDataCache.CompMapDataDict = DataHelper2.BytesToObject<Dictionary<int, CompMapData>>(compSyncData.BytesCompMapDataDict, 0, compSyncData.BytesCompMapDataDict.Length);
					}
					if (this.CompSyncDataCache.CompRankBattleJiFenDict.Age != compSyncData.CompRankBattleJiFenDict.Age)
					{
						this.CompSyncDataCache.CompBattleJoinRoleNum = compSyncData.CompBattleJoinRoleNum;
						if (null == compSyncData.BytesCompRankBattleJiFenDict)
						{
							this.CompSyncDataCache.CompRankBattleJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
							this.CompSyncDataCache.CompRankBattleJiFenDict.Age = compSyncData.CompRankBattleJiFenDict.Age;
						}
						else
						{
							this.CompSyncDataCache.CompRankBattleJiFenDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(compSyncData.BytesCompRankBattleJiFenDict, 0, compSyncData.BytesCompRankBattleJiFenDict.Length);
						}
					}
					if (this.CompSyncDataCache.CompRankMineJiFenDict.Age != compSyncData.CompRankMineJiFenDict.Age)
					{
						this.CompSyncDataCache.CompMineJoinRoleNum = compSyncData.CompMineJoinRoleNum;
						if (null == compSyncData.BytesCompRankMineJiFenDict)
						{
							this.CompSyncDataCache.CompRankMineJiFenDict = new KuaFuData<Dictionary<int, List<KFCompRankInfo>>>();
							this.CompSyncDataCache.CompRankMineJiFenDict.Age = compSyncData.CompRankMineJiFenDict.Age;
						}
						else
						{
							this.CompSyncDataCache.CompRankMineJiFenDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(compSyncData.BytesCompRankMineJiFenDict, 0, compSyncData.BytesCompRankMineJiFenDict.Length);
						}
					}
					if (this.CompSyncDataCache.CompDataDict.Age != compSyncData.CompDataDict.Age && null != compSyncData.BytesCompDataDict)
					{
						Dictionary<int, KFCompData> dictionary = new Dictionary<int, KFCompData>(this.CompSyncDataCache.CompDataDict.V);
						this.CompSyncDataCache.CompDataDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, KFCompData>>>(compSyncData.BytesCompDataDict, 0, compSyncData.BytesCompDataDict.Length);
						bool flag2 = false;
						foreach (KFCompData kfcompData in dictionary.Values)
						{
							int enemyCompType = kfcompData.EnemyCompType;
							KFCompData kfcompData2;
							this.CompSyncDataCache.CompDataDict.V.TryGetValue(kfcompData.CompType, out kfcompData2);
							if (kfcompData2 != null && kfcompData2.EnemyCompType != enemyCompType)
							{
								flag2 = true;
							}
						}
						if (flag2)
						{
							int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
							for (int i = 0; i < maxClientCount; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
								if (null != gameClient)
								{
									this.UpdateMapBuffer(gameClient);
								}
							}
						}
					}
					if (this.CompSyncDataCache.CompRankJunXianDict.Age != compSyncData.CompRankJunXianDict.Age && null != compSyncData.BytesCompRankJunXianDict)
					{
						this.CompSyncDataCache.CompRankJunXianDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(compSyncData.BytesCompRankJunXianDict, 0, compSyncData.BytesCompRankJunXianDict.Length);
					}
					if (this.CompSyncDataCache.CompRankJunXianLastDict.Age != compSyncData.CompRankJunXianLastDict.Age && null != compSyncData.BytesCompRankJunXianLastDict)
					{
						this.CompSyncDataCache.CompRankJunXianLastDict = DataHelper2.BytesToObject<KuaFuData<Dictionary<int, List<KFCompRankInfo>>>>(compSyncData.BytesCompRankJunXianLastDict, 0, compSyncData.BytesCompRankJunXianLastDict.Length);
						int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
						for (int i = 0; i < maxClientCount; i++)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
							if (null != gameClient)
							{
								this.UpdateChengHaoBuffer(gameClient);
							}
						}
						if (!GameManager.IsKuaFuServer)
						{
							for (int j = 1; j <= 3; j++)
							{
								List<KFCompRankInfo> list = null;
								if (this.CompSyncDataCache.CompRankJunXianLastDict.V.TryGetValue(j, out list) && null != list)
								{
									List<KFCompRankInfo> list2 = new List<KFCompRankInfo>(list);
									if (list2.Count > 5)
									{
										list2 = list2.GetRange(0, 5);
									}
									foreach (KFCompRankInfo kfcompRankInfo in list2)
									{
										RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, kfcompRankInfo.Key), 0);
										if (roleDataEx != null && roleDataEx.RoleID > 0)
										{
											RoleData4Selector roleData4Selector = Global.RoleDataEx2RoleData4Selector(roleDataEx);
											roleData4Selector.CompType = j;
											roleData4Selector.CompZhiWu = (byte)this.GetZhiWuByRankJunXianLast(j, kfcompRankInfo.Key);
											TianTiClient.getInstance().Comp_SetRoleData4Selector(roleDataEx.RoleID, DataHelper.ObjectToBytes<RoleData4Selector>(roleData4Selector));
										}
									}
								}
							}
						}
						else
						{
							this.OnRefreshAllCompNpc(0);
						}
					}
					if (this.CompSyncDataCache.CompRankBossDamageList.Age != compSyncData.CompRankBossDamageList.Age && null != compSyncData.BytesCompRankBossDamageList)
					{
						List<KFCompRankInfo> list3 = new List<KFCompRankInfo>(this.CompSyncDataCache.CompRankBossDamageList.V);
						this.CompSyncDataCache.CompRankBossDamageList = DataHelper2.BytesToObject<KuaFuData<List<KFCompRankInfo>>>(compSyncData.BytesCompRankBossDamageList, 0, compSyncData.BytesCompRankBossDamageList.Length);
						for (int j = 0; j < this.CompSyncDataCache.CompRankBossDamageList.V.Count; j++)
						{
							int num = (list3.Count > j) ? list3[j].Value : 0;
							int value = this.CompSyncDataCache.CompRankBossDamageList.V[j].Value;
							if (num != value)
							{
								if (num > 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(num);
									if (null != gameClient)
									{
										this.UpdateChengHaoBuffer(gameClient);
									}
								}
								if (value > 0)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(value);
									if (null != gameClient)
									{
										this.UpdateChengHaoBuffer(gameClient);
									}
								}
							}
						}
					}
				}
				CompBattleManager.getInstance().UpdateCompBattleBaseData(this.CompSyncDataCache.CompDataDict.V);
				CompMineManager.getInstance().UpdateCompMineResourceData(this.CompSyncDataCache.CompDataDict.V);
				List<KFCompChat> list4 = null;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompChatList.Count > 0)
					{
						list4 = new List<KFCompChat>(this.RuntimeData.CompChatList);
						this.RuntimeData.CompChatList.Clear();
					}
				}
				if (null != list4)
				{
					TianTiClient.getInstance().Comp_CompChat(list4);
				}
				List<KFCompNotice> list5 = null;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CompNoticeList.Count > 0)
					{
						list5 = new List<KFCompNotice>(this.RuntimeData.CompNoticeList);
						this.RuntimeData.CompNoticeList.Clear();
					}
				}
				if (null != list5)
				{
					TianTiClient.getInstance().Comp_BroadCastCompNotice(list5);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
		}

		public void AddDelayCreateMonster(CompScene scene, long ticks, object monster)
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

		public void CheckCreateDynamicMonster(CompScene scene, long nowMs)
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
							if (obj is CompResourcesConfig)
							{
								CompResourcesConfig compResourcesConfig = obj as CompResourcesConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, compResourcesConfig.MonsterID, -1, 1, compResourcesConfig.PosX / 100, compResourcesConfig.PosY / 100, 0, 0, 48, compResourcesConfig, null);
							}
							else if (obj is CompSolderSiteConfig)
							{
								CompSolderSiteConfig compSolderSiteConfig = obj as CompSolderSiteConfig;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.m_nMapCode, compSolderSiteConfig.SolderConfig.MonsterID, -1, 1, compSolderSiteConfig.PosX / 100, compSolderSiteConfig.PosY / 100, 0, 0, 48, compSolderSiteConfig, null);
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

		private List<int> GetYestdBoomValueList()
		{
			Dictionary<int, KFCompData> dictionary = null;
			lock (this.RuntimeData.Mutex)
			{
				dictionary = this.CompSyncDataCache.CompDataDict.V;
			}
			List<int> list = new List<int>();
			for (int i = 1; i <= 3; i++)
			{
				KFCompData kfcompData = null;
				dictionary.TryGetValue(i, out kfcompData);
				if (null != kfcompData)
				{
					list.Add(kfcompData.YestdBoomValue);
				}
				else
				{
					list.Add(0);
				}
			}
			return list;
		}

		private int GetCompBoomRank(int compType)
		{
			List<int> yestdBoomValueList = this.GetYestdBoomValueList();
			int num = yestdBoomValueList[compType - 1];
			int num2 = 0;
			foreach (int num3 in yestdBoomValueList)
			{
				if (num3 > num)
				{
					num2++;
				}
			}
			return num2 + 1;
		}

		private CompSolderConfig GetCompSolderConfigByCompType(int compType)
		{
			Dictionary<KeyValuePair<int, int>, CompSolderConfig> dictionary = null;
			lock (this.RuntimeData.Mutex)
			{
				dictionary = this.RuntimeData.CompSolderConfigDict;
			}
			int compBoomRank = this.GetCompBoomRank(compType);
			CompSolderConfig result = null;
			dictionary.TryGetValue(new KeyValuePair<int, int>(compType, compBoomRank), out result);
			return result;
		}

		private void InitCreateDynamicMonster(CompScene scene, DateTime now)
		{
			long ticks = now.Ticks / 10000L;
			Dictionary<int, CompResourcesConfig> dictionary = null;
			List<CompSolderSiteConfig> list = null;
			lock (this.RuntimeData.Mutex)
			{
				dictionary = this.RuntimeData.CompResourcesConfigDict;
				list = this.RuntimeData.CompSolderSiteConfigList;
			}
			foreach (CompSolderSiteConfig compSolderSiteConfig in list)
			{
				scene.CompSolderSiteConfigList.Add(compSolderSiteConfig.Clone() as CompSolderSiteConfig);
			}
			if (scene.ResourceNum > 0)
			{
				int num = 0;
				int num2 = 0;
				foreach (CompResourcesConfig compResourcesConfig in dictionary.Values)
				{
					if (compResourcesConfig.MapCodeID == scene.m_nMapCode)
					{
						compResourcesConfig.ResourceState = ((num2 < scene.ResourceGrowUpNum) ? 2 : 1);
						this.AddDelayCreateMonster(scene, ticks, compResourcesConfig);
						if (2 == compResourcesConfig.ResourceState)
						{
							num2++;
						}
						if (++num >= scene.ResourceNum)
						{
							break;
						}
					}
				}
			}
		}

		public void CheckCreateResource(CompScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			Dictionary<int, CompResourcesConfig> dictionary = null;
			lock (this.RuntimeData.Mutex)
			{
				dictionary = this.RuntimeData.CompResourcesConfigDict;
			}
			bool flag2 = false;
			foreach (CompResourcesConfig compResourcesConfig in dictionary.Values)
			{
				if (compResourcesConfig.MapCodeID == scene.m_nMapCode)
				{
					if (!(now.TimeOfDay < compResourcesConfig.RefreshTimeBegin) && !(now.TimeOfDay > compResourcesConfig.RefreshTimeEnd))
					{
						if (compResourcesConfig.ResourceState == 0)
						{
							compResourcesConfig.ResourceState = 1;
							this.AddDelayCreateMonster(scene, num, compResourcesConfig);
							scene.ResourceNum++;
							flag2 = true;
						}
					}
				}
			}
			List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
			if (null != objectsByMap)
			{
				foreach (object obj in objectsByMap)
				{
					Monster monster = obj as Monster;
					if (monster.MonsterType == 1601 && monster.Alive)
					{
						CompResourcesConfig compResourcesConfig2 = monster.Tag as CompResourcesConfig;
						if (compResourcesConfig2 != null && compResourcesConfig2.ResourceState != 0)
						{
							if (compResourcesConfig2.ResourceState == 1 && num >= monster.GetMonsterBirthTick() / 10000L + (long)(compResourcesConfig2.GrowTime * 1000))
							{
								compResourcesConfig2.ResourceState = 2;
								monster.ResetMonsterBirthTick();
								scene.ResourceGrowUpNum++;
								flag2 = true;
							}
							else if (compResourcesConfig2.ResourceState == 2 && num >= monster.GetMonsterBirthTick() / 10000L + (long)(compResourcesConfig2.AutoCollectTime * 1000))
							{
								scene.ResourceGrowUpNum--;
								flag2 = true;
								scene.ResourceNum--;
								flag2 = true;
								monster.Tag = null;
								compResourcesConfig2.ResourceState = 0;
								GameManager.MonsterMgr.DeadMonsterImmediately(monster);
								if (compResourcesConfig2.BoomValue > 0)
								{
									TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 0, compResourcesConfig2.BoomValue, 0);
									TianTiClient.getInstance().Comp_CompOpt(scene.CompSceneInfo.ID, 6, compResourcesConfig2.BoomValue, 0);
								}
							}
						}
					}
				}
			}
			if (flag2)
			{
				scene.SaveCompSceneDBInfo();
			}
		}

		public void CheckCreateSolder(CompScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			CompSolderSiteConfig compSolderSiteConfig = scene.CompSolderSiteConfigList.First<CompSolderSiteConfig>();
			if (now.TimeOfDay < compSolderSiteConfig.RefreshTimeBegin || now.TimeOfDay > compSolderSiteConfig.RefreshTimeEnd)
			{
				List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
				if (null != objectsByMap)
				{
					foreach (object obj in objectsByMap)
					{
						Monster monster = obj as Monster;
						CompSolderSiteConfig compSolderSiteConfig2 = monster.Tag as CompSolderSiteConfig;
						if (compSolderSiteConfig2 != null && monster.Alive)
						{
							monster.Tag = null;
							compSolderSiteConfig2.MonsterState = 0;
							GameManager.MonsterMgr.DeadMonsterImmediately(monster);
						}
					}
				}
			}
			else
			{
				List<CompSolderSiteConfig> list = scene.CompSolderSiteConfigList.FindAll((CompSolderSiteConfig x) => x.MonsterState == 1);
				if (scene.SolderNum > list.Count)
				{
					int num2 = 0;
					foreach (CompSolderSiteConfig compSolderSiteConfig3 in scene.CompSolderSiteConfigList)
					{
						if (compSolderSiteConfig3.MonsterState != 1)
						{
							compSolderSiteConfig3.MonsterState = 1;
							compSolderSiteConfig3.SolderConfig = this.GetCompSolderConfigByCompType(scene.CompSceneInfo.ID);
							this.AddDelayCreateMonster(scene, num, compSolderSiteConfig3);
							if (++num2 >= scene.SolderNum - list.Count)
							{
								break;
							}
						}
					}
				}
				int compBoomRank = this.GetCompBoomRank(scene.CompSceneInfo.ID);
				int num3 = this.RuntimeData.CompSolderCD[compBoomRank - 1];
				DateTime dateTime = new DateTime(scene.SolderRefreshTimeMS * 10000L);
				if (Global.GetOffsetDay(dateTime) != Global.GetOffsetDay(now))
				{
					dateTime = now.Date + compSolderSiteConfig.RefreshTimeBegin;
				}
				if (num >= scene.SolderRefreshTimeMS)
				{
					int num4 = (now - dateTime).Seconds / num3 + 1;
					scene.SolderRefreshTimeMS = dateTime.Ticks / 10000L + (long)(num4 * num3 * 1000);
					List<CompSolderSiteConfig> list2 = new List<CompSolderSiteConfig>();
					foreach (CompSolderSiteConfig compSolderSiteConfig4 in scene.CompSolderSiteConfigList)
					{
						if (compSolderSiteConfig4.MonsterState == 0)
						{
							list2.Add(compSolderSiteConfig4);
						}
					}
					if (list2.Count != 0)
					{
						List<CompSolderSiteConfig> list3 = Global.RandomSortList<CompSolderSiteConfig>(list2);
						list3 = list3.GetRange(0, num4);
						foreach (CompSolderSiteConfig compSolderSiteConfig3 in list3)
						{
							compSolderSiteConfig3.MonsterState = 1;
							compSolderSiteConfig3.SolderConfig = this.GetCompSolderConfigByCompType(scene.CompSceneInfo.ID);
							this.AddDelayCreateMonster(scene, num, compSolderSiteConfig3);
							scene.SolderNum++;
						}
					}
					scene.SaveCompSceneDBInfo();
				}
			}
		}

		public void CheckSolderWarning(CompScene scene, DateTime now)
		{
			long num = now.Ticks / 10000L;
			List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(scene.m_nMapCode);
			if (null != objectsByMap)
			{
				foreach (object obj in objectsByMap)
				{
					Monster monster = obj as Monster;
					CompSolderSiteConfig compSolderSiteConfig = monster.Tag as CompSolderSiteConfig;
					if (monster.LockFocusTime != 0L && compSolderSiteConfig != null && monster.Alive)
					{
						if (num - monster.LockFocusTime >= (long)(compSolderSiteConfig.SolderConfig.AlarmTime * 1000))
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(monster.LockObject);
							if (null != gameClient)
							{
								KFCompNotice notice = new KFCompNotice
								{
									NoticeID = 1,
									CompType = scene.CompSceneInfo.ID,
									Param1 = Global.FormatRoleNameWithZoneId2(gameClient),
									toMapCode = scene.m_nMapCode,
									toPosX = gameClient.ClientData.PosX,
									toPosY = gameClient.ClientData.PosY
								};
								this.CompNotice(scene, notice);
							}
						}
					}
				}
			}
		}

		public void CheckMapRoleNum(CompScene scene, DateTime now)
		{
			int mapClientsCount = GameManager.ClientMgr.GetMapClientsCount(scene.m_nMapCode);
			TianTiClient.getInstance().Comp_UpdateMapRoleNum(scene.m_nMapCode, mapClientsCount);
		}

		public void TimerProc_fuBenWorker()
		{
			try
			{
				DateTime now = TimeUtil.NowDateTime();
				long num = now.Ticks / 10000L;
				if (this.RuntimeData.NextHeartBeatTicks <= num)
				{
					this.RuntimeData.NextHeartBeatTicks = num + 1020L;
					List<int> list = null;
					lock (this.RuntimeData.Mutex)
					{
						list = this.CompSyncDataCache.ServerLineList;
					}
					if (list.Exists((int x) => x == GameManager.ServerId))
					{
						Dictionary<int, CompConfig> dictionary = null;
						lock (this.RuntimeData.Mutex)
						{
							dictionary = this.RuntimeData.CompConfigDict;
						}
						for (int i = 1; i <= 3; i++)
						{
							if (list[i - 1] == GameManager.ServerId)
							{
								CompConfig compConfig = null;
								if (dictionary.TryGetValue(i, out compConfig))
								{
									CompScene compScene = null;
									if (!this.SceneDict.TryGetValue(compConfig.MapCode, out compScene))
									{
										compScene = new CompScene();
										compScene.m_nMapCode = compConfig.MapCode;
										compScene.CompSceneInfo = compConfig;
										this.SceneDict[compConfig.MapCode] = compScene;
										compScene.LoadCompSceneDBInfo();
										this.InitCreateDynamicMonster(compScene, now);
									}
								}
							}
						}
						foreach (CompScene scene in this.SceneDict.Values)
						{
							lock (this.RuntimeData.Mutex)
							{
								this.CheckCreateDynamicMonster(scene, num);
								this.CheckCreateResource(scene, now);
								this.CheckCreateSolder(scene, now);
								this.CheckSolderWarning(scene, now);
								this.CheckMapRoleNum(scene, now);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(9, ex.ToString(), null, true);
			}
		}

		public void RestoreCompNpc(CompConfig compConfig)
		{
			NPC npc = NPCGeneralManager.FindNPC(compConfig.MapCode, compConfig.MoBai);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, false);
			}
		}

		public void ReplaceCompNpc(CompConfig compConfig, RoleData4Selector OwnerRoleData)
		{
			if (null == OwnerRoleData)
			{
				this.RestoreCompNpc(compConfig);
			}
			else
			{
				NPC npc = NPCGeneralManager.FindNPC(compConfig.MapCode, compConfig.MoBai);
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
					FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, false);
					FakeRoleManager.ProcessNewFakeRole(OwnerRoleData, npc.MapCode, FakeRoleTypes.CompDaLingZhu_1 + compConfig.ID - 1, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, compConfig.MoBai);
				}
			}
		}

		public void OnRefreshAllCompNpc(int rid = 0)
		{
			lock (this.RuntimeData.Mutex)
			{
				Dictionary<int, CompConfig> compConfigDict = this.RuntimeData.CompConfigDict;
				Dictionary<int, List<KFCompRankInfo>> v = this.CompSyncDataCache.CompRankJunXianLastDict.V;
				for (int i = 1; i <= 3; i++)
				{
					CompConfig compConfig = null;
					if (compConfigDict.TryGetValue(i, out compConfig))
					{
						RoleData4Selector roleData4Selector = null;
						List<KFCompRankInfo> list = null;
						if (v.TryGetValue(i, out list) && list != null && list.Count != 0)
						{
							if (rid > 0 && list[0].Key != rid)
							{
								goto IL_10D;
							}
							int key = list[0].Key;
							KFCompRoleData kfcompRoleData = TianTiClient.getInstance().Comp_GetCompRoleData(key);
							if (kfcompRoleData != null && null != kfcompRoleData.RoleData4Selector)
							{
								roleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(kfcompRoleData.RoleData4Selector, 0, kfcompRoleData.RoleData4Selector.Length);
							}
						}
						this.OwnerRoleDataDict[compConfig.MapCode] = roleData4Selector;
						this.ReplaceCompNpc(compConfig, roleData4Selector);
					}
					IL_10D:;
				}
			}
		}

		public int GetCompShopDuiHuanType(CompShopDHTypeIndex idx)
		{
			int result;
			lock (this.RuntimeData.Mutex)
			{
				result = this.RuntimeData.CompShopDuiHuanType[(int)idx];
			}
			return result;
		}

		public bool CheckCanAddJunXian(long LastStartTimeTicks)
		{
			DateTime dateTime = TimeUtil.NowDateTime().AddMinutes(5.0);
			DateTime dateTime2 = new DateTime(LastStartTimeTicks * 10000L);
			DateTime weekStartTime = TimeUtil.GetWeekStartTime(dateTime);
			DateTime weekStartTime2 = TimeUtil.GetWeekStartTime(dateTime2);
			return weekStartTime.DayOfYear == weekStartTime2.DayOfYear;
		}

		public const SceneUIClasses ManagerType = 48;

		public CompRuntimeData RuntimeData = new CompRuntimeData();

		public CompSyncData CompSyncDataCache = new CompSyncData();

		public ConcurrentDictionary<int, CompScene> SceneDict = new ConcurrentDictionary<int, CompScene>();

		public Dictionary<int, RoleData4Selector> OwnerRoleDataDict = new Dictionary<int, RoleData4Selector>();

		private static CompManager instance = new CompManager();
	}
}
