using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
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
using Tmsk.Tools.Tools;

namespace GameServer.Logic
{
	public class KuaFuLueDuoManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static KuaFuLueDuoManager getInstance()
		{
			return KuaFuLueDuoManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("KuaFuLueDuoManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 5000);
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1245, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1246, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1247, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1248, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1250, 3, 3, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1251, 2, 2, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1252, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsBinaryStreamParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1254, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1255, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1257, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1258, 3, 3, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1259, 1, 1, KuaFuLueDuoManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(26, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10003, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10002, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(10, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, KuaFuLueDuoManager.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(26, 10000, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10003, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10002, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(33, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(30, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(27, 47, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(10, KuaFuLueDuoManager.getInstance());
			GlobalEventSource.getInstance().removeListener(11, KuaFuLueDuoManager.getInstance());
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
			if (nID != 1252)
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(3, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					return true;
				}
			}
			switch (nID)
			{
			case 1245:
				return this.ProcessGetKuaFuLueDuoMainInfoCmd(client, nID, bytes, cmdParams);
			case 1246:
				return this.ProcessGetKuaFuLueDuoRankInfoCmd(client, nID, bytes, cmdParams);
			case 1247:
				return this.ProcessGetKuaFuLueDuoAnalysisDataCmd(client, nID, bytes, cmdParams);
			case 1248:
				return this.ProcessKuaFuLueDuoBuyEnterNumCmd(client, nID, bytes, cmdParams);
			case 1250:
				return this.ProcessKuaFuLueDuoJoinCmd(client, nID, bytes, cmdParams);
			case 1251:
				return this.ProcessKuaFuLueDuoEnterCmd(client, nID, bytes, cmdParams);
			case 1252:
				return this.ProcessGetKuaFuLueDuoStateCmd(client, nID, bytes, cmdParams);
			case 1254:
				return this.ProcessGetKuaFuLueDuoAwardInfoCmd(client, nID, bytes, cmdParams);
			case 1255:
				return this.ProcessGetKuaFuLueDuoAwardCmd(client, nID, bytes, cmdParams);
			case 1257:
				return this.ProcessGetKuaFuLueDuoMallDataCmd(client, nID, bytes, cmdParams);
			case 1258:
				return this.ProcessKuaFuLueDuoMallBuyCmd(client, nID, bytes, cmdParams);
			case 1259:
				return this.ProcessKuaFuLueDuoMallRefreshCmd(client, nID, bytes, cmdParams);
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
			else if (eventType == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
				if (null != playerInitGameEventObject)
				{
					this.OnInitGame(playerInitGameEventObject.getPlayer());
				}
			}
			else if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessJunQiDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
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
				break;
			case 26:
			{
				PostBangHuiChangeEventObject postBangHuiChangeEventObject = eventObject as PostBangHuiChangeEventObject;
				if (postBangHuiChangeEventObject != null && null != postBangHuiChangeEventObject.Player)
				{
					this.UpdateChengHaoBuffer(postBangHuiChangeEventObject.Player, true);
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
			case 29:
			case 31:
			case 32:
				break;
			case 30:
			{
				OnCreateMonsterEventObject onCreateMonsterEventObject = eventObject as OnCreateMonsterEventObject;
				if (null != onCreateMonsterEventObject)
				{
					QiZhiConfig qiZhiConfig = onCreateMonsterEventObject.Monster.Tag as QiZhiConfig;
					if (null != qiZhiConfig)
					{
						onCreateMonsterEventObject.Monster.Camp = qiZhiConfig.BattleWhichSide;
						onCreateMonsterEventObject.Result = true;
						onCreateMonsterEventObject.Handled = true;
					}
				}
				break;
			}
			case 33:
			{
				PreMonsterInjureEventObject preMonsterInjureEventObject = eventObject as PreMonsterInjureEventObject;
				if (preMonsterInjureEventObject != null && preMonsterInjureEventObject.SceneType == 47)
				{
					Monster monster = preMonsterInjureEventObject.Monster;
					if (monster != null)
					{
						QiZhiConfig qiZhiConfig2 = monster.Tag as QiZhiConfig;
						if (qiZhiConfig2 != null)
						{
							preMonsterInjureEventObject.Injure = qiZhiConfig2.Injure;
							eventObject.Handled = true;
							eventObject.Result = true;
						}
					}
				}
				break;
			}
			default:
				switch (num)
				{
				case 10002:
				{
					CaiJiEventObject caiJiEventObject = eventObject as CaiJiEventObject;
					if (null != caiJiEventObject)
					{
						GameClient gameClient = caiJiEventObject.Source as GameClient;
						Monster monster2 = caiJiEventObject.Target as Monster;
						this.OnCaiJiFinish(gameClient, monster2);
						eventObject.Handled = true;
						eventObject.Result = true;
					}
					break;
				}
				case 10003:
				{
					GetCaiJiTimeEventObject getCaiJiTimeEventObject = eventObject as GetCaiJiTimeEventObject;
					if (null != getCaiJiTimeEventObject)
					{
						GameClient gameClient = getCaiJiTimeEventObject.Source as GameClient;
						Monster monster2 = getCaiJiTimeEventObject.Target as Monster;
						if (gameClient != null && null != monster2)
						{
							getCaiJiTimeEventObject.GatherTime = this.GetCaiJiMonsterTime(gameClient, monster2);
							eventObject.Handled = true;
							eventObject.Result = true;
						}
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
					if (!this.RuntimeData.CommonConfigData.Load(Global.GameResPath("Config\\CrusadeWar.xml"), Global.GameResPath("Config\\CrusadeGroup.xml"), GameManager.PlatformType))
					{
						LogManager.WriteLog(2, "跨服掠夺 活动和分组配置 InitConfig failed!", null, true);
					}
					this.LoadCollectXml();
					this.RuntimeData.MapBirthPointDict.Clear();
					text = "Config\\CrusadeBirthPoint.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						MapBirthPoint mapBirthPoint = new MapBirthPoint();
						mapBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						mapBirthPoint.BirthPosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						mapBirthPoint.BirthPosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						mapBirthPoint.BirthRangeX = (mapBirthPoint.BirthRangeY = (int)Global.GetSafeAttributeLong(xml, "BirthRadius"));
						this.RuntimeData.MapBirthPointDict[mapBirthPoint.ID] = mapBirthPoint;
					}
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					text = "Config\\CrusadeQiZhi.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						QiZhiConfig qiZhiConfig = new QiZhiConfig();
						qiZhiConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "QiZuoID");
						qiZhiConfig.MonsterId = (int)Global.GetSafeAttributeLong(xml, "JunQiID");
						qiZhiConfig.PosX = ConfigHelper.String2IntArray(Global.GetSafeAttributeStr(xml, "QiZuoSite"), '|')[0];
						qiZhiConfig.PosY = ConfigHelper.String2IntArray(Global.GetSafeAttributeStr(xml, "QiZuoSite"), '|')[1];
						qiZhiConfig.Injure = (int)Global.GetSafeAttributeLong(xml, "Hurt");
						qiZhiConfig.RebirthRadius = (int)Global.GetSafeAttributeLong(xml, "RebirthRadius");
						this.RuntimeData.NPCID2QiZhiConfigDict[qiZhiConfig.NPCID] = qiZhiConfig;
					}
					this.RuntimeData.KingOfBattleStoreDict.Clear();
					this.RuntimeData.KingOfBattleStoreList.Clear();
					text = "Config\\CrusadeStore.xml";
					uri = Global.GameResPath(text);
					xelement = XElement.Load(uri);
					enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						KuaFuLueDuoStoreConfig kuaFuLueDuoStoreConfig = new KuaFuLueDuoStoreConfig();
						kuaFuLueDuoStoreConfig.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						kuaFuLueDuoStoreConfig.Type = (int)Global.GetSafeAttributeLong(xml, "Type");
						kuaFuLueDuoStoreConfig.SaleData = Global.ParseGoodsFromStr_7(Global.GetSafeAttributeStr(xml, "GoodsID").Split(new char[]
						{
							','
						}), 0);
						kuaFuLueDuoStoreConfig.ZuanShi = (int)Global.GetSafeAttributeLong(xml, "ZuanShiNum");
						kuaFuLueDuoStoreConfig.JueXingNum = (int)Global.GetSafeAttributeLong(xml, "JueXingNum");
						kuaFuLueDuoStoreConfig.SinglePurchase = (int)Global.GetSafeAttributeLong(xml, "SinglePurchase");
						kuaFuLueDuoStoreConfig.BeginNum = (int)Global.GetSafeAttributeLong(xml, "BeginNum");
						kuaFuLueDuoStoreConfig.EndNum = (int)Global.GetSafeAttributeLong(xml, "EndNum");
						kuaFuLueDuoStoreConfig.RandNumMinus = kuaFuLueDuoStoreConfig.EndNum - kuaFuLueDuoStoreConfig.BeginNum + 1;
						this.RuntimeData.KingOfBattleStoreDict[kuaFuLueDuoStoreConfig.ID] = kuaFuLueDuoStoreConfig;
						this.RuntimeData.KingOfBattleStoreList.Add(kuaFuLueDuoStoreConfig);
						if (kuaFuLueDuoStoreConfig.Type == 2)
						{
							this.RuntimeData.BeginNum = Math.Min(this.RuntimeData.BeginNum, kuaFuLueDuoStoreConfig.BeginNum);
							this.RuntimeData.EndNum = Math.Max(this.RuntimeData.EndNum, kuaFuLueDuoStoreConfig.EndNum);
						}
					}
					this.RuntimeData.CrusadeOrePercent = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeOrePercent", ',');
					this.RuntimeData.CrusadeUltraKill = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeUltraKill", ',');
					this.RuntimeData.CrusadeShutDown = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeShutDown", ',');
					this.RuntimeData.CrusadeAwardAttacker = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeAwardAttacker", ',');
					this.RuntimeData.CrusadeAwardDefender = GameManager.systemParamsList.GetParamValueDoubleArrayByName("CrusadeAwardDefender", ',');
					this.RuntimeData.CrusadeSeason = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeSeason", 13);
					this.RuntimeData.CrusadeOre = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeOre", ',');
					this.RuntimeData.CrusadeMinApply = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeMinApply", 10000);
					this.RuntimeData.CrusadeApplyCD = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeApplyCD", 300);
					this.RuntimeData.CrusadeEnterTime = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeEnterTime", ',');
					this.RuntimeData.CrusadeEnterPrice = GameManager.systemParamsList.GetParamValueIntArrayByName("CrusadeEnterPrice", ',');
					this.RuntimeData.CrusadePerfect = GameManager.systemParamsList.GetParamValueDoubleByName("CrusadePerfect", 0.0);
					this.RuntimeData.CrusadeStoreCD = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStoreCD", 86400);
					this.RuntimeData.CrusadeStorePrice = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStorePrice", 0);
					this.RuntimeData.CrusadeStoreRandomNum = (int)GameManager.systemParamsList.GetParamValueIntByName("CrusadeStoreRandomNum", 8);
					this.RuntimeData.ZhanMengZiJin = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengZiJin", ',');
					this.RuntimeData.HideRankList.Clear();
					List<string> paramValueStringListByName = GameManager.systemParamsList.GetParamValueStringListByName("KuaFuLueDuoHideRankList", '|');
					if (null != paramValueStringListByName)
					{
						foreach (string text2 in paramValueStringListByName)
						{
							List<int> list = ConfigHelper.String2IntList(text2, ',');
							if (list.Count > 1 && list[0] == GameManager.PlatformType)
							{
								for (int i = 1; i < list.Count; i++)
								{
									this.RuntimeData.HideRankList.Add(list[i]);
								}
							}
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

		public void LoadCollectXml()
		{
			string text = "";
			try
			{
				Dictionary<int, KuaFuLueDuoMonsterItem> dictionary = new Dictionary<int, KuaFuLueDuoMonsterItem>();
				text = Global.GameResPath("Config\\CrusadeCrystalMonster.xml");
				XElement xelement = ConfigHelper.Load(text);
				if (null != xelement)
				{
					foreach (XElement xml in xelement.Elements())
					{
						int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ID", "0"));
						int monsterID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MonsterID", "0"));
						dictionary[num] = new KuaFuLueDuoMonsterItem
						{
							ID = num,
							MonsterID = monsterID,
							Type = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Type", "0")),
							Name = Global.GetDefAttributeStr(xml, "Name", ""),
							GatherTime = Convert.ToInt32(Global.GetDefAttributeStr(xml, "GatherTime", "0")),
							FuHuoTime = Convert.ToInt32(Global.GetDefAttributeStr(xml, "FuHuoTime", "0")),
							ZiYuan = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Ore", "0")),
							JiFen = Convert.ToInt32(Global.GetDefAttributeStr(xml, "JiFen", "0")),
							X = Convert.ToInt32(Global.GetDefAttributeStr(xml, "X", "0")),
							Y = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Y", "0"))
						};
					}
				}
				this.RuntimeData.CollectMonsterDict = dictionary;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。ex:{1}", text, ex.Message), ex, true);
			}
		}

		public void RefreshKuaFuLueDuoStoreData(KuaFuLueDuoStoreData KuaFuLueDuoStoreData, bool SetRefreshTm = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (SetRefreshTm)
				{
					KuaFuLueDuoStoreData.LastRefTime = TimeUtil.NowDateTime();
				}
				KuaFuLueDuoStoreData.SaleList.Clear();
				List<KuaFuLueDuoStoreConfig> kingOfBattleStoreList = this.RuntimeData.KingOfBattleStoreList;
				int beginNum = this.RuntimeData.BeginNum;
				int num = this.RuntimeData.EndNum;
				for (int i = 0; i < this.RuntimeData.CrusadeStoreRandomNum; i++)
				{
					int num2 = Global.GetRandomNumber(beginNum, num);
					for (int j = 0; j < kingOfBattleStoreList.Count; j++)
					{
						KuaFuLueDuoStoreConfig kuaFuLueDuoStoreConfig = kingOfBattleStoreList[j];
						if (kuaFuLueDuoStoreConfig.Type == 1)
						{
							if (!kuaFuLueDuoStoreConfig.RandSkip)
							{
								kuaFuLueDuoStoreConfig.RandSkip = true;
								KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData = new KuaFuLueDuoStoreSaleData();
								kuaFuLueDuoStoreSaleData.ID = kuaFuLueDuoStoreConfig.ID;
								KuaFuLueDuoStoreData.SaleList.Add(kuaFuLueDuoStoreSaleData);
							}
						}
						else if (kuaFuLueDuoStoreConfig.Type == 2)
						{
							if (kuaFuLueDuoStoreConfig.RandSkip)
							{
								num2 += kuaFuLueDuoStoreConfig.RandNumMinus;
							}
							else if (num2 >= kuaFuLueDuoStoreConfig.BeginNum && num2 <= kuaFuLueDuoStoreConfig.EndNum)
							{
								kuaFuLueDuoStoreConfig.RandSkip = true;
								num -= kuaFuLueDuoStoreConfig.RandNumMinus;
								KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData = new KuaFuLueDuoStoreSaleData();
								kuaFuLueDuoStoreSaleData.ID = kuaFuLueDuoStoreConfig.ID;
								KuaFuLueDuoStoreData.SaleList.Add(kuaFuLueDuoStoreSaleData);
							}
						}
					}
				}
				for (int j = 0; j < kingOfBattleStoreList.Count; j++)
				{
					kingOfBattleStoreList[j].RandSkip = false;
				}
			}
		}

		public KuaFuLueDuoStoreData GetClientKuaFuLueDuoStoreData(GameClient client)
		{
			KuaFuLueDuoStoreData kuaFuLueDuoStoreData;
			if (null != client.ClientData.KuaFuLueDuoStoreData)
			{
				kuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
			}
			else
			{
				lock (this.RuntimeData.Mutex)
				{
					client.ClientData.KuaFuLueDuoStoreData = new KuaFuLueDuoStoreData();
					client.ClientData.KuaFuLueDuoStoreData.LastRefTime = Global.GetRoleParamsDateTimeFromDB(client, "10202");
					client.ClientData.KuaFuLueDuoStoreData.SaleList = new List<KuaFuLueDuoStoreSaleData>();
					List<ushort> roleParamsUshortListFromDB = Global.GetRoleParamsUshortListFromDB(client, "46");
					for (int i = 0; i < roleParamsUshortListFromDB.Count - 1; i += 2)
					{
						KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData = new KuaFuLueDuoStoreSaleData();
						kuaFuLueDuoStoreSaleData.ID = (int)roleParamsUshortListFromDB[i];
						kuaFuLueDuoStoreSaleData.Purchase = (int)roleParamsUshortListFromDB[i + 1];
						client.ClientData.KuaFuLueDuoStoreData.SaleList.Add(kuaFuLueDuoStoreSaleData);
					}
				}
				kuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
			}
			return kuaFuLueDuoStoreData;
		}

		public void SaveKuaFuLueDuoStoreData(GameClient client)
		{
			if (null != client.ClientData.KuaFuLueDuoStoreData)
			{
				lock (this.RuntimeData.Mutex)
				{
					KuaFuLueDuoStoreData kuaFuLueDuoStoreData = client.ClientData.KuaFuLueDuoStoreData;
					Global.SaveRoleParamsDateTimeToDB(client, "10202", kuaFuLueDuoStoreData.LastRefTime, true);
					List<ushort> list = new List<ushort>();
					foreach (KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData in kuaFuLueDuoStoreData.SaleList)
					{
						list.Add((ushort)kuaFuLueDuoStoreSaleData.ID);
						list.Add((ushort)kuaFuLueDuoStoreSaleData.Purchase);
					}
					Global.SaveRoleParamsUshortListToDB(client, list, "46", true);
				}
			}
		}

		public bool ProcessGetKuaFuLueDuoMallDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				KuaFuLueDuoStoreData clientKuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				if ((TimeUtil.NowDateTime() - clientKuaFuLueDuoStoreData.LastRefTime).TotalSeconds >= (double)this.RuntimeData.CrusadeStoreCD)
				{
					this.RefreshKuaFuLueDuoStoreData(clientKuaFuLueDuoStoreData, true);
					this.SaveKuaFuLueDuoStoreData(client);
				}
				client.sendCmd<KuaFuLueDuoStoreData>(nID, clientKuaFuLueDuoStoreData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKuaFuLueDuoMallBuyCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[0]);
				int num3 = Global.SafeConvertToInt32(cmdParams[1]);
				int num4 = Global.SafeConvertToInt32(cmdParams[2]);
				KuaFuLueDuoStoreConfig kuaFuLueDuoStoreConfig = null;
				string cmdData;
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.KingOfBattleStoreDict.TryGetValue(num3, out kuaFuLueDuoStoreConfig))
					{
						num = -20;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				KuaFuLueDuoStoreData clientKuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData = null;
				foreach (KuaFuLueDuoStoreSaleData kuaFuLueDuoStoreSaleData2 in clientKuaFuLueDuoStoreData.SaleList)
				{
					if (kuaFuLueDuoStoreSaleData2.ID == num3)
					{
						kuaFuLueDuoStoreSaleData = kuaFuLueDuoStoreSaleData2;
						break;
					}
				}
				if (null == kuaFuLueDuoStoreSaleData)
				{
					num = -20;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (kuaFuLueDuoStoreConfig.SinglePurchase - kuaFuLueDuoStoreSaleData.Purchase < num4)
				{
					num = -36;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (!Global.CanAddGoods(client, kuaFuLueDuoStoreConfig.SaleData.GoodsID, kuaFuLueDuoStoreConfig.SaleData.GCount * num4, kuaFuLueDuoStoreConfig.SaleData.Binding, "1900-01-01 12:00:00", true, false))
				{
					num = -100;
					cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				if (kuaFuLueDuoStoreConfig.JueXingNum > 0)
				{
					int jueXingPoint = client.ClientData.JueXingPoint;
					if (jueXingPoint < kuaFuLueDuoStoreConfig.JueXingNum * num4)
					{
						num = -45;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				else if (kuaFuLueDuoStoreConfig.ZuanShi > 0)
				{
					if (client.ClientData.UserMoney < kuaFuLueDuoStoreConfig.ZuanShi * num4)
					{
						num = -10;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				if (kuaFuLueDuoStoreConfig.JueXingNum > 0)
				{
					if (!GameManager.ClientMgr.ModifyJueXingValue(client, -kuaFuLueDuoStoreConfig.JueXingNum * num4, "觉醒商城购买", false))
					{
						num = -45;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				else if (kuaFuLueDuoStoreConfig.ZuanShi > 0)
				{
					if (!GameManager.ClientMgr.SubUserMoney(client, kuaFuLueDuoStoreConfig.ZuanShi * num4, "觉醒商城购买", true, true, true, true, DaiBiSySType.None))
					{
						num = -10;
						cmdData = string.Format("{0}:{1}:{2}", num, num3, 0);
						client.sendCmd(nID, cmdData, false);
						return true;
					}
				}
				GoodsData saleData = kuaFuLueDuoStoreConfig.SaleData;
				Global.AddGoodsDBCommand_Hook(Global._TCPManager.TcpOutPacketPool, client, saleData.GoodsID, saleData.GCount * num4, saleData.Quality, saleData.Props, saleData.Forge_level, saleData.Binding, 0, saleData.Jewellist, true, 1, string.Format("觉醒商城", new object[0]), false, saleData.Endtime, saleData.AddPropIndex, saleData.BornIndex, saleData.Lucky, saleData.Strong, saleData.ExcellenceInfo, saleData.AppendPropLev, saleData.ChangeLifeLevForEquip, true, null, null, "1900-01-01 12:00:00", 0, true);
				kuaFuLueDuoStoreSaleData.Purchase += num4;
				this.SaveKuaFuLueDuoStoreData(client);
				cmdData = string.Format("{0}:{1}:{2}", num, num3, kuaFuLueDuoStoreSaleData.Purchase);
				client.sendCmd(nID, cmdData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKuaFuLueDuoMallRefreshCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!this.IsGongNengOpenedEnter(client, false))
				{
					return false;
				}
				int num = 0;
				string cmdData;
				if (client.ClientData.UserMoney < this.RuntimeData.CrusadeStorePrice)
				{
					num = 7;
					cmdData = string.Format("{0}", num);
					client.sendCmd<int>(nID, num, false);
					return true;
				}
				if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.RuntimeData.CrusadeStorePrice, "觉醒商城刷新", true, true, false, DaiBiSySType.None))
				{
					num = 7;
					cmdData = string.Format("{0}", num);
					client.sendCmd(nID, cmdData, false);
					return true;
				}
				KuaFuLueDuoStoreData clientKuaFuLueDuoStoreData = this.GetClientKuaFuLueDuoStoreData(client);
				this.RefreshKuaFuLueDuoStoreData(clientKuaFuLueDuoStoreData, false);
				this.SaveKuaFuLueDuoStoreData(client);
				client.sendCmd<KuaFuLueDuoStoreData>(1257, clientKuaFuLueDuoStoreData, false);
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

		public bool ProcessGetKuaFuLueDuoMainInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int faction = client.ClientData.Faction;
				KuaFuLueDuoMainInfo kuaFuLueDuoMainInfo = DataHelper.BytesToObject<KuaFuLueDuoMainInfo>(bytes, 0, bytes.Length);
				if (null == kuaFuLueDuoMainInfo)
				{
					kuaFuLueDuoMainInfo = new KuaFuLueDuoMainInfo();
				}
				lock (this.RuntimeData.Mutex)
				{
					if (kuaFuLueDuoMainInfo.StateListAge != this.SyncDataCache.StateAge)
					{
						if (this.RuntimeData.JingJiaDict.TryGetValue(faction, out kuaFuLueDuoMainInfo.JingJiaData))
						{
						}
						kuaFuLueDuoMainInfo.StateListAge = this.SyncDataCache.StateAge;
						if (null != this.SyncDataCache.StateList)
						{
							kuaFuLueDuoMainInfo.StateList = this.SyncDataCache.StateList.Values.ToList<KuaFuLueDuoServerJingJiaState>();
							foreach (KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState in kuaFuLueDuoMainInfo.StateList)
							{
								if (null != kuaFuLueDuoServerJingJiaState.JingJiaList)
								{
									foreach (KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData in kuaFuLueDuoServerJingJiaState.JingJiaList)
									{
										kuaFuLueDuoBangHuiJingJiaData.ZiJin = 0;
									}
								}
							}
						}
					}
					if (kuaFuLueDuoMainInfo.ServerListAge != this.SyncDataCache.ServerInfoDictAge)
					{
						kuaFuLueDuoMainInfo.ServerListAge = this.SyncDataCache.ServerInfoDictAge;
						kuaFuLueDuoMainInfo.ServerList = this.SyncDataCache.ServerInfoDict.Values.ToList<KuaFuLueDuoServerInfo>();
					}
				}
				client.sendCmd<KuaFuLueDuoMainInfo>(nID, kuaFuLueDuoMainInfo, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private int GetKuaFuLueDuoRankNum(GameClient client, int ranktype)
		{
			int num = 0;
			switch (ranktype)
			{
			case 0:
			{
				int num2 = client.ServerId;
				KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
				if (this.SyncDataCache.ServerInfoDict.TryGetValue(num2, out kuaFuLueDuoServerInfo))
				{
					num = ((kuaFuLueDuoServerInfo.ZhengFuList == null) ? 0 : kuaFuLueDuoServerInfo.ZhengFuList.Count);
				}
				break;
			}
			case 2:
			{
				int num2 = client.ClientData.Faction;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CacheBh2LueDuoDict.TryGetValue(num2, out num))
					{
						return num;
					}
				}
				KuaFuLueDuoBHData bhdataByBhid_KuaFuLueDuo = HuanYingSiYuanClient.getInstance().GetBHDataByBhid_KuaFuLueDuo(num2);
				if (null != bhdataByBhid_KuaFuLueDuo)
				{
					num = bhdataByBhid_KuaFuLueDuo.sum_ziyuan;
					if (num >= 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CacheBh2LueDuoDict[num2] = num;
						}
					}
				}
				break;
			}
			case 4:
			{
				int num2 = client.ClientData.RoleID;
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.CacheRole2KillDict.TryGetValue(num2, out num))
					{
						return num;
					}
				}
				byte[] roleData_KuaFuLueDuo = HuanYingSiYuanClient.getInstance().GetRoleData_KuaFuLueDuo((long)num2);
				if (null != roleData_KuaFuLueDuo)
				{
					num = DataHelper.BytesToObject<int>(roleData_KuaFuLueDuo, 0, roleData_KuaFuLueDuo.Length);
					if (num >= 0)
					{
						lock (this.RuntimeData.Mutex)
						{
							this.RuntimeData.CacheRole2KillDict[num2] = num;
						}
					}
				}
				break;
			}
			}
			return num;
		}

		public bool ProcessGetKuaFuLueDuoRankInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoRankListCmdData kuaFuLueDuoRankListCmdData = DataHelper.BytesToObject<KuaFuLueDuoRankListCmdData>(bytes, 0, bytes.Length);
				if (kuaFuLueDuoRankListCmdData == null)
				{
					kuaFuLueDuoRankListCmdData = new KuaFuLueDuoRankListCmdData();
				}
				int rankType = kuaFuLueDuoRankListCmdData.RankType;
				KuaFuLueDuoRankListCmdData kuaFuLueDuoRankListCmdData2 = new KuaFuLueDuoRankListCmdData
				{
					RankType = rankType
				};
				int kuaFuLueDuoRankNum = this.GetKuaFuLueDuoRankNum(client, rankType);
				kuaFuLueDuoRankListCmdData2.SelfData = new KuaFuLueDuoRankInfo
				{
					Value = kuaFuLueDuoRankNum
				};
				lock (this.RuntimeData.Mutex)
				{
					KuaFuLueDuoRankListData kuaFuLueDuoRankInfoDict = this.SyncDataCache.KuaFuLueDuoRankInfoDict;
					if (kuaFuLueDuoRankInfoDict != null && !this.RuntimeData.HideRankList.Contains(rankType))
					{
						List<KuaFuLueDuoRankInfo> listRankList = new List<KuaFuLueDuoRankInfo>();
						if (kuaFuLueDuoRankInfoDict.ListDict != null && kuaFuLueDuoRankInfoDict.ListDict.TryGetValue(rankType, out listRankList))
						{
							kuaFuLueDuoRankListCmdData2.ListRankList = listRankList;
						}
						KuaFuLueDuoRankInfo lastData;
						if (kuaFuLueDuoRankInfoDict.LastInfoDict != null && kuaFuLueDuoRankInfoDict.LastInfoDict.TryGetValue(rankType, out lastData))
						{
							kuaFuLueDuoRankListCmdData2.LastData = lastData;
						}
					}
				}
				client.sendCmd<KuaFuLueDuoRankListCmdData>(nID, kuaFuLueDuoRankListCmdData2, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetKuaFuLueDuoAnalysisDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private int UpdateClientEnterNum(GameClient client)
		{
			int offsetDay = TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime());
			if (this.SyncDataCache.GameState == 3)
			{
				if (client.ClientData.KuaFuLueDuoEnterNumDayID < offsetDay)
				{
					GameManager.ClientMgr.ModifyKuaFuLueDuoBuyNumAndDayID(client, 0, offsetDay, "重置次数");
					int num = Math.Min(this.RuntimeData.CrusadeEnterTime[1], client.ClientData.KuaFuLueDuoEnterNum + this.RuntimeData.CrusadeEnterTime[0]) - client.ClientData.KuaFuLueDuoEnterNum;
					if (num > 0)
					{
						GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, num, "系统每轮补充", false);
					}
				}
			}
			return offsetDay;
		}

		public bool ProcessKuaFuLueDuoBuyEnterNumCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = -18;
				int i = Global.SafeConvertToInt32(cmdParams[1]);
				if (this.SyncDataCache.GameState != 3)
				{
					num = -2001;
				}
				else
				{
					int dayID = this.UpdateClientEnterNum(client);
					num = client.ClientData.KuaFuLueDuoEnterNum;
					if (num + i > this.RuntimeData.CrusadeEnterTime[1])
					{
						num = -36;
					}
					else
					{
						while (i > 0)
						{
							int num2 = Global.Clamp(client.ClientData.KuaFuLueDuoEnterNumBuyNum, 0, this.RuntimeData.CrusadeEnterPrice.Length - 1);
							int num3 = this.RuntimeData.CrusadeEnterPrice[num2];
							lock (client.ClientData.UserMoneyMutex)
							{
								if (client.ClientData.UserMoney < num3)
								{
									num = -10;
									break;
								}
								if (!GameManager.ClientMgr.SubUserMoney(client, num3, "购买跨服掠夺进入次数", true, true, true, true, DaiBiSySType.None))
								{
									num = -10;
									break;
								}
							}
							GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, 1, "购买跨服掠夺进入次数", false);
							GameManager.ClientMgr.ModifyKuaFuLueDuoBuyNumAndDayID(client, client.ClientData.KuaFuLueDuoEnterNumBuyNum + 1, dayID, "购买跨服掠夺进入次数");
							num = client.ClientData.KuaFuLueDuoEnterNum;
							i--;
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

		public bool ProcessKuaFuLueDuoJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				int num3 = Global.SafeConvertToInt32(cmdParams[2]);
				int faction = client.ClientData.Faction;
				int roleID = client.ClientData.RoleID;
				if (!this.IsGongNengOpenedJingJia(client, false))
				{
					num = -400;
				}
				else if (faction <= 0 || client.ClientData.BHZhiWu != 1)
				{
					num = -1002;
				}
				else if (this.SyncDataCache.GameState != 1)
				{
					num = -2001;
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, faction, 0);
					if (null == bangHuiDetailData)
					{
						num = -1001;
					}
					else if (num2 <= 0 || num2 == GameManager.ServerId)
					{
						num = -18;
					}
					else if (bangHuiDetailData.ZoneID == num2)
					{
						num = -18;
					}
					else
					{
						KuaFuLueDuoConfig kuaFuLueDuoConfig = null;
						KuaFuLueDuoGameStates kuaFuLueDuoGameStates = 0;
						int num4 = 0;
						num = this.CheckCondition(client, ref kuaFuLueDuoConfig, ref kuaFuLueDuoGameStates, ref num4);
						if (kuaFuLueDuoGameStates != 1)
						{
							num = -2001;
						}
						if (num >= 0)
						{
							KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData;
							lock (this.RuntimeData.Mutex)
							{
								if (!this.RuntimeData.JingJiaDict.TryGetValue(faction, out kuaFuLueDuoBangHuiJingJiaData))
								{
									kuaFuLueDuoBangHuiJingJiaData = new KuaFuLueDuoBangHuiJingJiaData
									{
										BhId = faction,
										ZoneId = bangHuiDetailData.ZoneID,
										BhName = bangHuiDetailData.BHName,
										ServerId = num2
									};
									this.RuntimeData.JingJiaDict[faction] = kuaFuLueDuoBangHuiJingJiaData;
								}
								if (kuaFuLueDuoBangHuiJingJiaData.ServerId > 0 && kuaFuLueDuoBangHuiJingJiaData.ServerId != num2 && kuaFuLueDuoBangHuiJingJiaData.ZiJin > 0)
								{
									num = -1004;
									goto IL_30F;
								}
								if (num3 < kuaFuLueDuoBangHuiJingJiaData.ZiJin + this.RuntimeData.CrusadeMinApply)
								{
									num = -1043;
									goto IL_30F;
								}
								if (bangHuiDetailData.TotalMoney < num3 - kuaFuLueDuoBangHuiJingJiaData.ZiJin + this.RuntimeData.ZhanMengZiJin[0])
								{
									num = -1041;
									goto IL_30F;
								}
							}
							KuaFuLueDuoJingJiaResult kuaFuLueDuoJingJiaResult = HuanYingSiYuanClient.getInstance().JingJia_KuaFuLueDuo(faction, bangHuiDetailData.ZoneID, bangHuiDetailData.BHName, num3, num2, kuaFuLueDuoBangHuiJingJiaData.ZiJin);
							num = kuaFuLueDuoJingJiaResult.Result;
							kuaFuLueDuoBangHuiJingJiaData.ZiJin = kuaFuLueDuoJingJiaResult.ZiJin;
							if (kuaFuLueDuoJingJiaResult.Result >= 0)
							{
								int num5 = num3 - kuaFuLueDuoJingJiaResult.ZiJin;
								int num6;
								if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num5, out num6))
								{
									LogManager.WriteLog(2, string.Format("帮会{0}资金扣除{1}失败", faction, num5), null, true);
								}
							}
						}
					}
				}
				IL_30F:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessKuaFuLueDuoEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				if (!this.IsGongNengOpenedEnter(client, true))
				{
					num = -13;
				}
				else if (client.ClientSocket.IsKuaFuLogin)
				{
					num = -21;
				}
				else
				{
					if (num2 == 0)
					{
						num2 = GameManager.ServerId;
					}
					if (faction <= 0 && num2 != GameManager.ServerId)
					{
						num = -1000;
					}
					else if (this.SyncDataCache.GameState != 3)
					{
						num = -2001;
					}
					else
					{
						KuaFuLueDuoConfig kuaFuLueDuoConfig = null;
						KuaFuLueDuoGameStates kuaFuLueDuoGameStates = 0;
						int num3 = 0;
						if (!this.CheckMap(client))
						{
							num = -21;
						}
						else
						{
							num = this.CheckCondition(client, ref kuaFuLueDuoConfig, ref kuaFuLueDuoGameStates, ref num3);
							if (num >= 0)
							{
								lock (this.RuntimeData.Mutex)
								{
									FightInfo fightInfo = null;
									if (this.SyncDataCache.ServerZiYuanDict != null && this.SyncDataCache.ServerZiYuanDict.TryGetValue(num2, out fightInfo))
									{
										int ziYuan = fightInfo.ZiYuan;
										if (ziYuan < 0)
										{
											num = -1044;
											goto IL_367;
										}
									}
									FightInfo fightInfo2;
									if (num2 == client.ServerId)
									{
										if (fightInfo != null && fightInfo.RoleNum >= kuaFuLueDuoConfig.DefenderMaxNum)
										{
											num = -1045;
											goto IL_367;
										}
									}
									else if (this.SyncDataCache.BhZiYuanDict.TryGetValue(faction, out fightInfo2) && fightInfo2.RoleNum >= kuaFuLueDuoConfig.AttackerMaxNum)
									{
										num = -1045;
										goto IL_367;
									}
								}
								KuaFuServerInfo kuaFuServerInfo = null;
								KuaFuLueDuoFuBenData fuBenDataByServerId_KuaFuLueDuo = HuanYingSiYuanClient.getInstance().GetFuBenDataByServerId_KuaFuLueDuo(num2);
								if (fuBenDataByServerId_KuaFuLueDuo == null || !KuaFuManager.getInstance().TryGetValue(fuBenDataByServerId_KuaFuLueDuo.ServerId, out kuaFuServerInfo))
								{
									num = -11000;
								}
								else if (fuBenDataByServerId_KuaFuLueDuo.BhDataList.Count == 0)
								{
									num = -1046;
								}
								else if (fuBenDataByServerId_KuaFuLueDuo.State >= 3 || fuBenDataByServerId_KuaFuLueDuo.LeftZiYuan <= 0)
								{
									num = -1044;
								}
								else
								{
									if (!VideoLogic.getInstance().IsGuanZhanGM(client))
									{
										if (fuBenDataByServerId_KuaFuLueDuo.DestServerId != GameManager.ServerId && !fuBenDataByServerId_KuaFuLueDuo.BhDataList.Contains(faction))
										{
											num = -4008;
											goto IL_367;
										}
										if (client.ClientData.KuaFuLueDuoEnterNum <= 0)
										{
											num = -16;
											goto IL_367;
										}
									}
									KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
									if (null != clientKuaFuServerLoginData)
									{
										clientKuaFuServerLoginData.RoleId = client.ClientData.RoleID;
										clientKuaFuServerLoginData.GameId = fuBenDataByServerId_KuaFuLueDuo.GameId;
										clientKuaFuServerLoginData.GameType = 25;
										clientKuaFuServerLoginData.EndTicks = 0L;
										clientKuaFuServerLoginData.ServerId = client.ServerId;
										clientKuaFuServerLoginData.ServerIp = kuaFuServerInfo.Ip;
										clientKuaFuServerLoginData.ServerPort = kuaFuServerInfo.Port;
									}
									GlobalNew.RecordSwitchKuaFuServerLog(client);
									client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
								}
							}
						}
					}
				}
				IL_367:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetKuaFuLueDuoStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int faction = client.ClientData.Faction;
				KuaFuLueDuoStateData kuaFuLueDuoStateData = DataHelper.BytesToObject<KuaFuLueDuoStateData>(bytes, 0, bytes.Length);
				if (null == kuaFuLueDuoStateData)
				{
					kuaFuLueDuoStateData = new KuaFuLueDuoStateData();
				}
				kuaFuLueDuoStateData.GameState = this.SyncDataCache.GameState;
				this.UpdateClientEnterNum(client);
				lock (this.RuntimeData.Mutex)
				{
					if (this.SyncDataCache.ServerInfoDict.ContainsKey(GameManager.ServerId))
					{
						kuaFuLueDuoStateData.ServerID = GameManager.ServerId;
						if (this.SyncDataCache.GameState >= 1 && this.SyncDataCache.GameState <= 4)
						{
							KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState;
							if (this.SyncDataCache.StateList != null && this.SyncDataCache.StateList.TryGetValue(GameManager.ServerId, out kuaFuLueDuoServerJingJiaState))
							{
								if (kuaFuLueDuoServerJingJiaState.JingJiaList != null && kuaFuLueDuoServerJingJiaState.JingJiaList.Count > 0)
								{
									kuaFuLueDuoStateData.AttackerList = new List<BangHuiMiniData>();
									foreach (KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData in kuaFuLueDuoServerJingJiaState.JingJiaList)
									{
										kuaFuLueDuoStateData.AttackerList.Add(new BangHuiMiniData
										{
											BHID = kuaFuLueDuoBangHuiJingJiaData.BhId,
											BHName = kuaFuLueDuoBangHuiJingJiaData.BhName,
											ZoneID = kuaFuLueDuoBangHuiJingJiaData.ZoneId
										});
									}
								}
							}
							if (this.SyncDataCache.ServerZiYuanDict != null)
							{
								FightInfo fightInfo;
								if (this.SyncDataCache.ServerZiYuanDict.TryGetValue(GameManager.ServerId, out fightInfo))
								{
									kuaFuLueDuoStateData.ZiYuan = fightInfo.ZiYuan;
								}
							}
							if (faction > 0)
							{
								KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData2;
								if (this.RuntimeData.JingJiaDict.TryGetValue(faction, out kuaFuLueDuoBangHuiJingJiaData2))
								{
									kuaFuLueDuoStateData.EnemyServerID = kuaFuLueDuoBangHuiJingJiaData2.ServerId;
								}
								if (this.SyncDataCache.ServerZiYuanDict != null)
								{
									FightInfo fightInfo;
									if (this.SyncDataCache.ServerZiYuanDict.TryGetValue(kuaFuLueDuoStateData.EnemyServerID, out fightInfo))
									{
										kuaFuLueDuoStateData.EnemyZiYuan = fightInfo.ZiYuan;
									}
								}
								if (this.SyncDataCache.BhZiYuanDict != null)
								{
									FightInfo fightInfo;
									if (this.SyncDataCache.BhZiYuanDict.TryGetValue(faction, out fightInfo))
									{
										kuaFuLueDuoStateData.LueDuoZiYuan = fightInfo.ZiYuan;
									}
								}
							}
						}
					}
				}
				KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				kuaFuLueDuoStateData.AwardsDataList = this.GetClientAwardsDataList(client, kuaFuLueDuoConfig);
				client.sendCmd<KuaFuLueDuoStateData>(nID, kuaFuLueDuoStateData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetKuaFuLueDuoAwardInfoCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				this.NtfCanGetAward(client, kuaFuLueDuoConfig);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessGetKuaFuLueDuoAwardCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				int cmdData = this.GiveRoleAwards(client, kuaFuLueDuoConfig);
				client.sendCmd<int>(nID, cmdData, false);
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

		private void OnInitGame(GameClient client)
		{
			this.UpdateChengHaoBuffer(client, false);
		}

		private KuaFuLueDuoFuBenData GetFuBenDataByGameId(long gameId)
		{
			KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData = null;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out kuaFuLueDuoFuBenData))
				{
					kuaFuLueDuoFuBenData = null;
				}
			}
			if (null == kuaFuLueDuoFuBenData)
			{
				KuaFuLueDuoFuBenData fuBenDataByGameId_KuaFuLueDuo = HuanYingSiYuanClient.getInstance().GetFuBenDataByGameId_KuaFuLueDuo(gameId);
				if (fuBenDataByGameId_KuaFuLueDuo == null)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + fuBenDataByGameId_KuaFuLueDuo == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return null;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out kuaFuLueDuoFuBenData))
					{
						kuaFuLueDuoFuBenData = fuBenDataByGameId_KuaFuLueDuo;
						kuaFuLueDuoFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenItemData[kuaFuLueDuoFuBenData.GameId] = kuaFuLueDuoFuBenData;
					}
				}
			}
			return kuaFuLueDuoFuBenData;
		}

		public bool KuaFuLogin(KuaFuServerLoginData kuaFuServerLoginData)
		{
			KuaFuLueDuoFuBenData fuBenDataByGameId = this.GetFuBenDataByGameId(kuaFuServerLoginData.GameId);
			bool result;
			if (fuBenDataByGameId == null || fuBenDataByGameId.ServerId != GameManager.ServerId)
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
			int faction = client.ClientData.Faction;
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			KuaFuLueDuoFuBenData fuBenDataByGameId = this.GetFuBenDataByGameId(clientKuaFuServerLoginData.GameId);
			bool result;
			if (null == fuBenDataByGameId)
			{
				LogManager.WriteLog(2, string.Format("角色({0})进入跨服活动({1})失败,未获取到活动副本({2})信息", client.ClientData.RoleID, 47, clientKuaFuServerLoginData.GameId), null, true);
				client.ClientData.PushMessageID = GLang.GetLang(2000, new object[0]);
				result = false;
			}
			else if (fuBenDataByGameId.State >= 3)
			{
				LogManager.WriteLog(2, string.Format("角色({0})进入跨服活动({1})失败,活动副本({2})已结束", client.ClientData.RoleID, 47, clientKuaFuServerLoginData.GameId), null, true);
				client.ClientData.PushMessageID = GLang.GetLang(2000, new object[0]);
				result = false;
			}
			else
			{
				int battleWhichSide = 0;
				if (client.ServerId == fuBenDataByGameId.DestServerId)
				{
					battleWhichSide = 1;
					client.ClientData.BirthSide = 1;
				}
				else
				{
					int num = fuBenDataByGameId.ServerIdList.IndexOf(client.ServerId);
					if (num < 0)
					{
						if (!VideoLogic.getInstance().IsGuanZhanGM(client))
						{
							LogManager.WriteLog(2, string.Format("角色({0})进入跨服活动({1})失败,角色服务器ID({2})无效", client.ClientData.RoleID, 47, client.ServerId), null, true);
							return false;
						}
						client.ClientData.HideGM = 1;
					}
					else
					{
						battleWhichSide = num + 1;
					}
					num = fuBenDataByGameId.BhDataList.IndexOf(client.ClientData.Faction);
					if (num < 0)
					{
						if (!VideoLogic.getInstance().IsGuanZhanGM(client))
						{
							LogManager.WriteLog(2, string.Format("角色({0})进入跨服活动({1})失败,角色战盟ID({2})无效", client.ClientData.RoleID, 47, client.ClientData.Faction), null, true);
							return false;
						}
						client.ClientData.HideGM = 1;
					}
					else
					{
						client.ClientData.BirthSide = num + 1 + 1;
					}
				}
				KuaFuLueDuoConfig kuaFuLueDuoConfig;
				lock (this.RuntimeData.Mutex)
				{
					clientKuaFuServerLoginData.FuBenSeqId = fuBenDataByGameId.SequenceId;
					kuaFuLueDuoConfig = this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.Values.FirstOrDefault<KuaFuLueDuoConfig>();
					if (null == kuaFuLueDuoConfig)
					{
						LogManager.WriteLog(2, string.Format("角色({0})进入跨服活动({1})失败,配置无效", client.ClientData.RoleID, 47), null, true);
						return false;
					}
					client.SceneInfoObject = kuaFuLueDuoConfig;
					client.ClientData.MapCode = kuaFuLueDuoConfig.MapCode;
					client.ClientData.BattleWhichSide = battleWhichSide;
				}
				int num2;
				int posX;
				int posY;
				if (!this.GetZhanMengBirthPoint(kuaFuLueDuoConfig, client, client.ClientData.MapCode, out num2, out posX, out posY, false))
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
			}
			return result;
		}

		public bool GetZhanMengBirthPoint(KuaFuLueDuoConfig sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY, bool isLogin = false)
		{
			mapCode = sceneInfo.MapCode;
			posX = 0;
			posY = 0;
			int birthSide = client.ClientData.BirthSide;
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
					KuaFuLueDuoScene kuaFuLueDuoScene = client.SceneObject as KuaFuLueDuoScene;
					if (null != kuaFuLueDuoScene)
					{
						QiZhiConfig qiZhiItem = kuaFuLueDuoScene.QiZhiItem;
						if (qiZhiItem != null && qiZhiItem.Alive && qiZhiItem.BattleWhichSide == client.ClientData.BattleWhichSide)
						{
							Point mapPointByGridXY = Global.GetMapPointByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, qiZhiItem.PosX / kuaFuLueDuoScene.MapGridWidth, qiZhiItem.PosY / kuaFuLueDuoScene.MapGridHeight, qiZhiItem.RebirthRadius / kuaFuLueDuoScene.MapGridWidth, 0, false);
							posX = (int)mapPointByGridXY.X;
							posY = (int)mapPointByGridXY.Y;
							return true;
						}
					}
					MapBirthPoint mapBirthPoint = null;
					if (!this.RuntimeData.MapBirthPointDict.TryGetValue(birthSide, out mapBirthPoint))
					{
						return false;
					}
					posX = mapBirthPoint.BirthPosX;
					posY = mapBirthPoint.BirthPosY;
				}
				result = true;
			}
			return result;
		}

		public void HandleNtfEnterEvent(KuaFuLueDuoNtfEnterData data)
		{
			foreach (GameClient gameClient in GameManager.ClientMgr.GetAllClients(true))
			{
				if (this.IsGongNengOpenedEnter(gameClient, false) && this.CheckMap(gameClient))
				{
					if (gameClient != null && data.BhIdList.Contains(gameClient.ClientData.Faction))
					{
						gameClient.sendCmd<int>(1256, 1, false);
					}
				}
			}
			LogManager.WriteLog(2, string.Format("通知战盟ID={0}拥有进入跨服掠夺资格", string.Join<int>(",", data.BhIdList)), null, true);
		}

		private void TimerProc(object sender, EventArgs e)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RequestSyncData.ServerID = GameManager.ServerId;
				this.RequestSyncData.StateAge = this.SyncDataCache.StateAge;
				this.RequestSyncData.ServerInfoDictAge = this.SyncDataCache.ServerInfoDictAge;
				this.RequestSyncData.KuaFuLueDuoRankInfoDict.Age = this.SyncDataCache.KuaFuLueDuoRankInfoDict.Age;
				this.RequestSyncData.FuBenStateAge = this.SyncDataCache.FuBenStateAge;
				if (this.RuntimeData.UpdateZiYuanData)
				{
					this.RequestSyncData.FuBenStateAge = 1L;
					this.RequestSyncData.BhZiYuanDict = new Dictionary<int, FightInfo>(this.RuntimeData.BhZiYuanDict);
					this.RequestSyncData.ServerZiYuanDict = new Dictionary<int, FightInfo>(this.RuntimeData.ServerZiYuanDict);
					this.RuntimeData.BhZiYuanDict.Clear();
					this.RuntimeData.ServerZiYuanDict.Clear();
					this.RuntimeData.UpdateZiYuanData = false;
				}
				KuaFuLueDuoSyncData kuaFuLueDuoSyncData = HuanYingSiYuanClient.getInstance().SyncData_KuaFuLueDuo(this.RequestSyncData);
				if (null != kuaFuLueDuoSyncData)
				{
					if (this.SyncDataCache.SeasonID != kuaFuLueDuoSyncData.SeasonID)
					{
						this.SyncDataCache.SeasonID = kuaFuLueDuoSyncData.SeasonID;
						this.RuntimeData.JingJiaDict.Clear();
					}
					this.SyncDataCache.LastSeasonID = kuaFuLueDuoSyncData.LastSeasonID;
					if (kuaFuLueDuoSyncData.GameState < 0)
					{
						this.SyncDataCache.GameState = (this.SyncDataCache.ServerGameState = kuaFuLueDuoSyncData.ServerGameState);
					}
					else
					{
						if (kuaFuLueDuoSyncData.GroupID != this.SyncDataCache.GroupID)
						{
						}
						if (kuaFuLueDuoSyncData.ServerInfoDictAge != this.RequestSyncData.ServerInfoDictAge)
						{
							this.SyncDataCache.ServerInfoDictAge = kuaFuLueDuoSyncData.ServerInfoDictAge;
							this.SyncDataCache.ServerInfoDict = kuaFuLueDuoSyncData.ServerInfoDict;
						}
						if (kuaFuLueDuoSyncData.FuBenStateAge != this.RequestSyncData.FuBenStateAge)
						{
							this.SyncDataCache.FuBenStateAge = kuaFuLueDuoSyncData.FuBenStateAge;
							this.SyncDataCache.ServerZiYuanDict = kuaFuLueDuoSyncData.ServerZiYuanDict;
							this.SyncDataCache.BhZiYuanDict = kuaFuLueDuoSyncData.BhZiYuanDict;
						}
						this.SyncDataCache.GameState = kuaFuLueDuoSyncData.GameState;
						if (kuaFuLueDuoSyncData.StateAge != this.RequestSyncData.StateAge)
						{
							this.SyncDataCache.StateAge = kuaFuLueDuoSyncData.StateAge;
							this.SyncDataCache.SignUpRound = kuaFuLueDuoSyncData.SignUpRound;
							this.SyncDataCache.StateList = kuaFuLueDuoSyncData.StateList;
							HashSet<int> hashSet = new HashSet<int>();
							Dictionary<int, KuaFuLueDuoBangHuiJingJiaData> dictionary = new Dictionary<int, KuaFuLueDuoBangHuiJingJiaData>();
							if (null != this.SyncDataCache.StateList)
							{
								foreach (KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState in this.SyncDataCache.StateList.Values)
								{
									if (null != kuaFuLueDuoServerJingJiaState.JingJiaList)
									{
										foreach (KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData in kuaFuLueDuoServerJingJiaState.JingJiaList)
										{
											KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData2;
											if (this.RuntimeData.JingJiaDict.TryGetValue(kuaFuLueDuoBangHuiJingJiaData.BhId, out kuaFuLueDuoBangHuiJingJiaData2))
											{
												kuaFuLueDuoBangHuiJingJiaData2.ZiJin = kuaFuLueDuoBangHuiJingJiaData.ZiJin;
												kuaFuLueDuoBangHuiJingJiaData2.ServerId = kuaFuLueDuoBangHuiJingJiaData.ServerId;
												kuaFuLueDuoBangHuiJingJiaData2.Age = this.SyncDataCache.StateAge;
											}
											else
											{
												kuaFuLueDuoBangHuiJingJiaData.Age = this.SyncDataCache.StateAge;
												this.RuntimeData.JingJiaDict[kuaFuLueDuoBangHuiJingJiaData.BhId] = kuaFuLueDuoBangHuiJingJiaData;
											}
										}
									}
								}
							}
							List<int> list = new List<int>();
							foreach (KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData2 in this.RuntimeData.JingJiaDict.Values)
							{
								KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData2;
								if (kuaFuLueDuoBangHuiJingJiaData2.Age < this.SyncDataCache.StateAge)
								{
									if (kuaFuLueDuoBangHuiJingJiaData2.ServerId > 0 && kuaFuLueDuoBangHuiJingJiaData2.ZiJin > 0)
									{
										int num = kuaFuLueDuoBangHuiJingJiaData2.BhId;
										int ziJin = kuaFuLueDuoBangHuiJingJiaData2.ZiJin;
										int serverId = kuaFuLueDuoBangHuiJingJiaData2.ServerId;
										kuaFuLueDuoBangHuiJingJiaData2.ServerId = 0;
										kuaFuLueDuoBangHuiJingJiaData2.ZiJin = 0;
										BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, num, 0);
										if (null != bangHuiDetailData)
										{
											if (!GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, null, num, ziJin))
											{
												LogManager.WriteLog(3, string.Format("跨服掠夺返还战盟竞价资金失败,bhid={0}, bidMoney={1}", num, ziJin), null, true);
											}
											else
											{
												Global.UseMailGivePlayerAward3(bangHuiDetailData.BZRoleID, null, GLang.GetLang(3000, new object[0]), string.Format(GLang.GetLang(3001, new object[0]), serverId), 0, 0, 0);
												list.Add(num);
											}
											GameClient gameClient = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
											if (null != gameClient)
											{
												gameClient.sendCmd<int>(1260, 1, false);
											}
										}
									}
									else
									{
										list.Add(kuaFuLueDuoBangHuiJingJiaData2.BhId);
									}
								}
							}
							foreach (int num in list)
							{
								int num;
								this.RuntimeData.JingJiaDict.Remove(num);
							}
						}
						if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.Age != kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict.Age)
						{
							this.RuntimeData.CacheRole2KillDict.Clear();
							this.RuntimeData.CacheBh2LueDuoDict.Clear();
							this.SyncDataCache.KuaFuLueDuoRankInfoDict = kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict;
							this.RefreshKuaFuLueDuoChampionBH();
						}
					}
				}
			}
		}

		public bool IsGongNengOpenedEnter(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 95, hint);
		}

		private bool IsGongNengOpenedJingJia(GameClient client, bool hint = false)
		{
			return GlobalNew.IsGongNengOpened(client, 94, hint);
		}

		public List<KuaFuLueDuoAwardsData> GetClientAwardsDataList(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> list = new List<KuaFuLueDuoAwardsData>();
			List<int> roleParamsIntListFromDBOffline = Global.GetRoleParamsIntListFromDBOffline(client, "47");
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline))
			{
				list.Add(new KuaFuLueDuoAwardsData
				{
					type = 1,
					ZiYuan = roleParamsIntListFromDBOffline[1],
					JiFen = roleParamsIntListFromDBOffline[2],
					Exp = (long)roleParamsIntListFromDBOffline[3] * sceneInfo.Exp,
					BindJinBi = roleParamsIntListFromDBOffline[4],
					JueXing = roleParamsIntListFromDBOffline[5]
				});
			}
			List<int> roleParamsIntListFromDBOffline2 = Global.GetRoleParamsIntListFromDBOffline(client, "48");
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline2))
			{
				list.Add(new KuaFuLueDuoAwardsData
				{
					type = 2,
					ZiYuan = roleParamsIntListFromDBOffline2[1],
					JiFen = roleParamsIntListFromDBOffline2[2],
					Exp = (long)roleParamsIntListFromDBOffline2[3] * sceneInfo.Exp,
					BindJinBi = roleParamsIntListFromDBOffline2[4],
					JueXing = roleParamsIntListFromDBOffline2[5]
				});
			}
			return list;
		}

		private void NtfCanGetAward(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> clientAwardsDataList = this.GetClientAwardsDataList(client, sceneInfo);
			client.sendCmd<List<KuaFuLueDuoAwardsData>>(1254, clientAwardsDataList, false);
		}

		private bool ValidateAwardsInfo(List<int> args)
		{
			if (args != null && args.Count >= 8 && args[3] > 0)
			{
				if (args[6] == this.SyncDataCache.SeasonID || (args[6] == this.SyncDataCache.LastSeasonID && this.SyncDataCache.GameState == 0))
				{
					return true;
				}
			}
			return false;
		}

		private int GiveRoleAwards(GameClient client, KuaFuLueDuoConfig sceneInfo)
		{
			List<KuaFuLueDuoAwardsData> list = new List<KuaFuLueDuoAwardsData>();
			List<int> roleParamsIntListFromDBOffline = Global.GetRoleParamsIntListFromDBOffline(client, "47");
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline))
			{
				list.Add(new KuaFuLueDuoAwardsData
				{
					Exp = (long)roleParamsIntListFromDBOffline[3] * sceneInfo.Exp,
					BindJinBi = roleParamsIntListFromDBOffline[4],
					JueXing = roleParamsIntListFromDBOffline[5]
				});
			}
			List<int> roleParamsIntListFromDBOffline2 = Global.GetRoleParamsIntListFromDBOffline(client, "48");
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline2))
			{
				list.Add(new KuaFuLueDuoAwardsData
				{
					Exp = (long)roleParamsIntListFromDBOffline2[3] * sceneInfo.Exp,
					BindJinBi = roleParamsIntListFromDBOffline2[4],
					JueXing = roleParamsIntListFromDBOffline2[5]
				});
			}
			foreach (KuaFuLueDuoAwardsData kuaFuLueDuoAwardsData in list)
			{
				if (kuaFuLueDuoAwardsData.Exp > 0L)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, kuaFuLueDuoAwardsData.Exp, true, true, false, "领取跨服掠夺奖励");
				}
				if (kuaFuLueDuoAwardsData.BindJinBi > 0)
				{
					GameManager.ClientMgr.AddMoney1(client, kuaFuLueDuoAwardsData.BindJinBi, "领取跨服掠夺奖励", true);
				}
				if (kuaFuLueDuoAwardsData.JueXing > 0)
				{
					GameManager.ClientMgr.ModifyJueXingValue(client, kuaFuLueDuoAwardsData.JueXing, "领取跨服掠夺奖励", false);
				}
			}
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline))
			{
				roleParamsIntListFromDBOffline[3] = 0;
				Global.SaveRoleParamsIntListToDBOffline(client.ClientData.RoleID, roleParamsIntListFromDBOffline, "47", client.ServerId);
			}
			if (this.ValidateAwardsInfo(roleParamsIntListFromDBOffline2))
			{
				roleParamsIntListFromDBOffline2[3] = 0;
				Global.SaveRoleParamsIntListToDBOffline(client.ClientData.RoleID, roleParamsIntListFromDBOffline2, "48", client.ServerId);
			}
			return 1;
		}

		public void GiveAwards(KuaFuLueDuoScene scene)
		{
			try
			{
				foreach (KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData in scene.ClientContextDataDict.Values)
				{
					if (0 != kuaFuLueDuoClientContextData.BattleWhichSide)
					{
						int totalScore = kuaFuLueDuoClientContextData.TotalScore;
						int num = kuaFuLueDuoClientContextData.BangHuiContextData.TotalScore + 1;
						if (kuaFuLueDuoClientContextData.BattleWhichSide == 1)
						{
							int num2 = scene.LeftZiYuan;
							kuaFuLueDuoClientContextData.AwardJueXing = (int)Math.Min((double)num2 * this.RuntimeData.CrusadeAwardDefender[0] * (double)totalScore / (double)num, this.RuntimeData.CrusadeAwardDefender[1]);
						}
						else
						{
							int num2 = kuaFuLueDuoClientContextData.BangHuiContextData.ZiYuan;
							kuaFuLueDuoClientContextData.AwardJueXing = (int)Math.Min((double)num2 * this.RuntimeData.CrusadeAwardAttacker[0] * (double)totalScore / (double)num, this.RuntimeData.CrusadeAwardAttacker[1]);
						}
						kuaFuLueDuoClientContextData.BangHuiContextData.TotalAwardJueXing += kuaFuLueDuoClientContextData.AwardJueXing;
						kuaFuLueDuoClientContextData.BangHuiContextData.TotalRoleNum++;
					}
				}
				foreach (KuaFuLueDuoBangHuiContextData kuaFuLueDuoBangHuiContextData in scene.BangHuiContextDataDict.Values)
				{
					kuaFuLueDuoBangHuiContextData.TotalRoleNum = Math.Max(kuaFuLueDuoBangHuiContextData.TotalRoleNum, 1);
					if (kuaFuLueDuoBangHuiContextData.ServerId == scene.ThisFuBenData.DestServerId)
					{
						kuaFuLueDuoBangHuiContextData.ZiYuan = scene.LeftZiYuan;
					}
					kuaFuLueDuoBangHuiContextData.BaoDi = kuaFuLueDuoBangHuiContextData.ZiYuan - kuaFuLueDuoBangHuiContextData.TotalAwardJueXing;
				}
				foreach (KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData in scene.ClientContextDataDict.Values)
				{
					if (0 != kuaFuLueDuoClientContextData.BattleWhichSide)
					{
						List<int> list = new List<int>();
						string roleParamsKey;
						if (kuaFuLueDuoClientContextData.BattleWhichSide == 1)
						{
							kuaFuLueDuoClientContextData.AwardJueXing += (int)Math.Max((double)(kuaFuLueDuoClientContextData.BangHuiContextData.BaoDi / kuaFuLueDuoClientContextData.BangHuiContextData.TotalRoleNum), this.RuntimeData.CrusadeAwardDefender[2]);
							kuaFuLueDuoClientContextData.AwardJueXing = (int)Math.Min((double)kuaFuLueDuoClientContextData.AwardJueXing, this.RuntimeData.CrusadeAwardDefender[3]) * scene.SceneInfo.JueXingNum;
							roleParamsKey = "47";
							list.Add(1);
							list.Add(scene.LeftZiYuan);
							list.Add(kuaFuLueDuoClientContextData.TotalScore);
							list.Add(kuaFuLueDuoClientContextData.AwardExpLevel);
							list.Add(kuaFuLueDuoClientContextData.AwardBindJinBi);
							list.Add(kuaFuLueDuoClientContextData.AwardJueXing);
							list.Add(this.SyncDataCache.SeasonID);
							list.Add(kuaFuLueDuoClientContextData.KillNum);
						}
						else
						{
							kuaFuLueDuoClientContextData.AwardJueXing += (int)Math.Max((double)(kuaFuLueDuoClientContextData.BangHuiContextData.BaoDi / kuaFuLueDuoClientContextData.BangHuiContextData.TotalRoleNum), this.RuntimeData.CrusadeAwardAttacker[2]);
							kuaFuLueDuoClientContextData.AwardJueXing = (int)Math.Min((double)kuaFuLueDuoClientContextData.AwardJueXing, this.RuntimeData.CrusadeAwardAttacker[3]) * scene.SceneInfo.JueXingNum;
							roleParamsKey = "48";
							list.Add(2);
							list.Add(kuaFuLueDuoClientContextData.ZiYuan);
							list.Add(kuaFuLueDuoClientContextData.TotalScore);
							list.Add(kuaFuLueDuoClientContextData.AwardExpLevel);
							list.Add(kuaFuLueDuoClientContextData.AwardBindJinBi);
							list.Add(kuaFuLueDuoClientContextData.AwardJueXing);
							list.Add(this.SyncDataCache.SeasonID);
							list.Add(kuaFuLueDuoClientContextData.KillNum);
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(kuaFuLueDuoClientContextData.RoleId);
						Global.SaveRoleParamsIntListToDBOffline(kuaFuLueDuoClientContextData.RoleId, list, roleParamsKey, kuaFuLueDuoClientContextData.ServerId);
						if (gameClient != null && gameClient.SceneObject == scene)
						{
							this.NtfCanGetAward(gameClient, scene.SceneInfo);
						}
					}
				}
				this.PushGameResultData(scene);
			}
			catch (Exception ex)
			{
				DataHelper.WriteExceptionLogEx(ex, "跨服掠夺系统清场调度异常");
			}
		}

		public void PushGameResultData(KuaFuLueDuoScene scene)
		{
			KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData;
			if (this.RuntimeData.FuBenItemData.TryGetValue((long)scene.GameId, out kuaFuLueDuoFuBenData))
			{
				foreach (KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData in scene.ClientContextDataDict.Values)
				{
					if (0 != kuaFuLueDuoClientContextData.BattleWhichSide)
					{
						KuaFuLueDuoRoleData kuaFuLueDuoRoleData = new KuaFuLueDuoRoleData();
						kuaFuLueDuoRoleData.rid = kuaFuLueDuoClientContextData.RoleId;
						kuaFuLueDuoRoleData.rname = kuaFuLueDuoClientContextData.RoleName;
						kuaFuLueDuoRoleData.zoneid = kuaFuLueDuoClientContextData.ZoneID;
						kuaFuLueDuoRoleData.kill = kuaFuLueDuoClientContextData.TotalKill;
						if (kuaFuLueDuoRoleData.kill > 0)
						{
							scene.GameStatisticalData.roleStatisticalData.Add(kuaFuLueDuoRoleData);
						}
					}
				}
				HuanYingSiYuanClient.getInstance().GameFuBenComplete_KuaFuLueDuo(scene.GameStatisticalData);
			}
		}

		private void UpdateChengHaoBuffer(GameClient client, bool notify = true)
		{
			if (this.RuntimeData.ChengHaoBHid > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				bool flag = 0 == 0;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_1_2, actionParams, 1, notify);
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_1_2, actionParams, 1, notify);
			}
			if (this.RuntimeData.ChengHaoBHid_Week > 0L && (long)client.ClientData.Faction == this.RuntimeData.ChengHaoBHid_Week)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_2_1, actionParams, 1, notify);
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.KuaFuLueDuo_2_1, actionParams, 1, notify);
			}
		}

		public bool RefreshKuaFuLueDuoChampionBH()
		{
			bool flag = false;
			int num = 0;
			int num2 = 0;
			if (null != this.SyncDataCache.KuaFuLueDuoRankInfoDict)
			{
				List<KuaFuLueDuoRankInfo> list;
				if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.ListDict != null && this.SyncDataCache.KuaFuLueDuoRankInfoDict.ListDict.TryGetValue(2, out list))
				{
					if (list != null && list.Count > 0)
					{
						num2 = list[0].Key;
					}
				}
				KuaFuLueDuoRankInfo kuaFuLueDuoRankInfo;
				if (this.SyncDataCache.KuaFuLueDuoRankInfoDict.LastInfoDict != null && this.SyncDataCache.KuaFuLueDuoRankInfoDict.LastInfoDict.TryGetValue(2, out kuaFuLueDuoRankInfo))
				{
					if (null != kuaFuLueDuoRankInfo)
					{
						num = kuaFuLueDuoRankInfo.Key;
					}
				}
			}
			if (this.RuntimeData.ChengHaoBHid_Week != (long)num)
			{
				flag = true;
			}
			this.RuntimeData.ChengHaoBHid_Week = (long)num;
			if (this.RuntimeData.ChengHaoBHid != (long)num2)
			{
				flag = true;
			}
			this.RuntimeData.ChengHaoBHid = (long)num2;
			if (flag)
			{
				int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
				for (int i = 0; i < maxClientCount; i++)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
					if (null != gameClient)
					{
						this.UpdateChengHaoBuffer(gameClient, true);
					}
				}
			}
			return flag;
		}

		private bool RefuseChangeBangHui(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (this.SyncDataCache.GameState >= 3 && this.SyncDataCache.GameState < 4)
				{
					KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData;
					if (this.RuntimeData.JingJiaDict.TryGetValue(bhid, out kuaFuLueDuoBangHuiJingJiaData) && kuaFuLueDuoBangHuiJingJiaData.ServerId > 0 && kuaFuLueDuoBangHuiJingJiaData.ZiJin > 0)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool PreRemoveBangHui(GameClient client)
		{
			bool result;
			if (this.RefuseChangeBangHui(client.ClientData.Faction))
			{
				GameManager.ClientMgr.NotifyImportantMsg(client, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = false;
			}
			else
			{
				result = true;
			}
			return result;
		}

		public bool OnPreBangHuiRemoveMember(PreBangHuiRemoveMemberEventObject e)
		{
			bool result;
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(3002, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private void InitScene(KuaFuLueDuoScene scene, GameClient client)
		{
			int num = 0;
			int num2 = 0;
			foreach (KuaFuLueDuoMonsterItem kuaFuLueDuoMonsterItem in this.RuntimeData.CollectMonsterDict.Values)
			{
				if (kuaFuLueDuoMonsterItem.Type == 1)
				{
					num = kuaFuLueDuoMonsterItem.ZiYuan;
				}
				if (kuaFuLueDuoMonsterItem.Type == 2)
				{
					num2 = kuaFuLueDuoMonsterItem.ZiYuan;
				}
				scene.CollectMonsterXml.Add(kuaFuLueDuoMonsterItem.ID, kuaFuLueDuoMonsterItem.Clone() as KuaFuLueDuoMonsterItem);
			}
			int num3 = (int)Math.Min(this.RuntimeData.CrusadeOrePercent[0] * (double)scene.TotalZiYuan, this.RuntimeData.CrusadeOrePercent[1]);
			if (num > 0)
			{
				scene.SmallZiYuanCount = (num3 - 1) / num + 1;
			}
			if (num2 > 0)
			{
				scene.BigZiYuanCount = (scene.TotalZiYuan - scene.SmallZiYuanCount * num - 1) / num2 + 1;
			}
			foreach (QiZhiConfig qiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.QiZhiItem = (qiZhiConfig.Clone() as QiZhiConfig);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 47)
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
					int faction = client.ClientData.Faction;
					long gameId = Global.GetClientKuaFuServerLoginData(client).GameId;
					DateTime dateTime = TimeUtil.NowDateTime();
					KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.Values.FirstOrDefault<KuaFuLueDuoConfig>();
					lock (this.RuntimeData.Mutex)
					{
						KuaFuLueDuoScene kuaFuLueDuoScene = null;
						if (!this.SceneDict.TryGetValue(fuBenSeqID, out kuaFuLueDuoScene))
						{
							KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData;
							if (!this.RuntimeData.FuBenItemData.TryGetValue(gameId, out kuaFuLueDuoFuBenData))
							{
								LogManager.WriteLog(2, "跨服掠夺没有为副本找到对应的跨服副本数据,GameID:" + gameId, null, true);
							}
							kuaFuLueDuoScene = new KuaFuLueDuoScene();
							kuaFuLueDuoScene.GameId = (int)gameId;
							kuaFuLueDuoScene.FuBenSeqId = fuBenSeqID;
							kuaFuLueDuoScene.ThisFuBenData = kuaFuLueDuoFuBenData;
							kuaFuLueDuoScene.SceneInfo = kuaFuLueDuoConfig;
							kuaFuLueDuoScene.MapGridWidth = gameMap.MapGridWidth;
							kuaFuLueDuoScene.MapGridHeight = gameMap.MapGridHeight;
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(kuaFuLueDuoConfig.ID));
							kuaFuLueDuoScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							kuaFuLueDuoScene.GameStatisticalData.GameId = gameId;
							kuaFuLueDuoScene.TotalZiYuan = kuaFuLueDuoFuBenData.LeftZiYuan;
							kuaFuLueDuoScene.LeftZiYuan = kuaFuLueDuoFuBenData.LeftZiYuan;
							kuaFuLueDuoScene.BangHuiContextDataDict[0] = new KuaFuLueDuoBangHuiContextData();
							for (int i = 0; i < kuaFuLueDuoFuBenData.BhDataList.Count; i++)
							{
								int num = kuaFuLueDuoFuBenData.BhDataList[i];
								kuaFuLueDuoScene.BangHuiContextDataDict[num] = new KuaFuLueDuoBangHuiContextData
								{
									BhId = num
								};
							}
							this.SceneDict[fuBenSeqID] = kuaFuLueDuoScene;
							this.InitScene(kuaFuLueDuoScene, client);
						}
						kuaFuLueDuoScene.CopyMap = copyMap;
						KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData;
						if (!kuaFuLueDuoScene.ClientContextDataDict.TryGetValue(roleID, out kuaFuLueDuoClientContextData))
						{
							kuaFuLueDuoClientContextData = new KuaFuLueDuoClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide,
								RoleName = client.ClientData.RoleName,
								Occupation = client.ClientData.Occupation,
								RoleSex = client.ClientData.RoleSex,
								ZoneID = client.ClientData.ZoneID,
								AwardExpLevel = (int)Global.GetExpMultiByZhuanShengExpXiShu(client, 1L),
								AwardBindJinBi = kuaFuLueDuoConfig.BandJinBi
							};
							kuaFuLueDuoScene.ClientContextDataDict[roleID] = kuaFuLueDuoClientContextData;
						}
						KuaFuLueDuoBangHuiContextData kuaFuLueDuoBangHuiContextData;
						if (kuaFuLueDuoScene.BangHuiContextDataDict.TryGetValue(faction, out kuaFuLueDuoBangHuiContextData))
						{
							kuaFuLueDuoClientContextData.BangHuiContextData = kuaFuLueDuoBangHuiContextData;
							if (kuaFuLueDuoBangHuiContextData.ZoneID == 0)
							{
								kuaFuLueDuoBangHuiContextData.ServerId = client.ServerId;
								if (faction > 0)
								{
									BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(faction, client.ServerId);
									kuaFuLueDuoBangHuiContextData.BhName = bangHuiMiniData.BHName;
									kuaFuLueDuoBangHuiContextData.ZoneID = bangHuiMiniData.ZoneID;
								}
								else
								{
									kuaFuLueDuoBangHuiContextData.ZoneID = client.ServerId;
								}
							}
						}
						else if (kuaFuLueDuoScene.BangHuiContextDataDict.TryGetValue(0, out kuaFuLueDuoBangHuiContextData))
						{
							kuaFuLueDuoClientContextData.BangHuiContextData = kuaFuLueDuoBangHuiContextData;
							kuaFuLueDuoBangHuiContextData.ServerId = client.ServerId;
						}
						kuaFuLueDuoClientContextData.Kill = 0;
						client.SceneObject = kuaFuLueDuoScene;
						client.SceneGameId = (long)kuaFuLueDuoScene.GameId;
						client.SceneContextData2 = kuaFuLueDuoClientContextData;
						copyMap.IsKuaFuCopy = true;
						copyMap.SetRemoveTicks(TimeUtil.NOW() + (long)(kuaFuLueDuoScene.SceneInfo.TotalSecs * 1000));
					}
					GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, -1, "进入跨服掠夺战场", false);
					result = true;
				}
			}
			else
			{
				result = false;
			}
			return result;
		}

		private int CheckCondition(GameClient client, ref KuaFuLueDuoConfig sceneItem, ref KuaFuLueDuoGameStates state, ref int signUpRound)
		{
			int result = 0;
			sceneItem = null;
			state = 0;
			lock (this.RuntimeData.Mutex)
			{
				sceneItem = this.RuntimeData.CommonConfigData.GetKuaFuLueDuoConfig(0);
				if (null == sceneItem)
				{
					return -4007;
				}
			}
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.ApplyTimePoints.Count - 1; i++)
				{
					TimeSpan timeSpan = timeOfWeekNow - sceneItem.ApplyTimePoints[i];
					TimeSpan timeSpan2 = timeOfWeekNow - sceneItem.ApplyTimePoints[i + 1];
					if (timeSpan.TotalSeconds >= 0.0 && timeSpan2.TotalSeconds < 0.0)
					{
						signUpRound = i + 1;
						state = 1;
						break;
					}
				}
				for (int i = 0; i < sceneItem.TimePoints.Count - 1; i += 2)
				{
					TimeSpan timeSpan = timeOfWeekNow - sceneItem.TimePoints[i];
					if (timeSpan.TotalSeconds >= 0.0 && timeSpan.TotalSeconds < (double)sceneItem.GameSecs)
					{
						state = 3;
						break;
					}
				}
			}
			return result;
		}

		private TimeSpan GetStartTime(int sceneId)
		{
			KuaFuLueDuoConfig kuaFuLueDuoConfig = null;
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			DateTime weekStartTimeNow = TimeUtil.GetWeekStartTimeNow();
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.CommonConfigData.KuaFuLueDuoConfigDict.TryGetValue(sceneId, out kuaFuLueDuoConfig))
				{
					for (int i = 0; i < kuaFuLueDuoConfig.TimePoints.Count - 1; i += 2)
					{
						TimeSpan timeSpan = kuaFuLueDuoConfig.TimePoints[i];
						if (timeOfWeekNow >= timeSpan)
						{
							return weekStartTimeNow.Add(timeSpan).TimeOfDay;
						}
					}
				}
			}
			return TimeUtil.NowDateTime().TimeOfDay;
		}

		public int GetCaiJiMonsterTime(GameClient client, Monster monster)
		{
			int result;
			if (client.ClientData.BattleWhichSide == 1)
			{
				result = -201;
			}
			else
			{
				KuaFuLueDuoMonsterItem kuaFuLueDuoMonsterItem = monster.Tag as KuaFuLueDuoMonsterItem;
				if (null != kuaFuLueDuoMonsterItem)
				{
					result = kuaFuLueDuoMonsterItem.GatherTime;
				}
				else
				{
					result = -4;
				}
			}
			return result;
		}

		public void GMCaiJi(GameClient client, int ziYuan)
		{
			int num = ziYuan * 2;
			KuaFuLueDuoScene kuaFuLueDuoScene;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuLueDuoScene))
				{
					return;
				}
				if (kuaFuLueDuoScene.m_eStatus != 2)
				{
					return;
				}
				KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
				if (null != kuaFuLueDuoClientContextData)
				{
					kuaFuLueDuoClientContextData.TotalScore += num;
					kuaFuLueDuoClientContextData.ZiYuan += ziYuan;
					kuaFuLueDuoClientContextData.BangHuiContextData.TotalScore += num;
					kuaFuLueDuoClientContextData.BangHuiContextData.ZiYuan += ziYuan;
					kuaFuLueDuoScene.LeftZiYuan = Math.Max(0, kuaFuLueDuoScene.LeftZiYuan - ziYuan);
				}
				this.SceneInfoChangeRole(kuaFuLueDuoScene, client, 0);
			}
			if (num > 0)
			{
				this.NotifyTimeStateInfoAndScoreInfo(kuaFuLueDuoScene, false, true);
			}
		}

		public void OnCaiJiFinish(GameClient client, Monster monster)
		{
			int num = 0;
			KuaFuLueDuoScene kuaFuLueDuoScene;
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoMonsterItem kuaFuLueDuoMonsterItem = monster.Tag as KuaFuLueDuoMonsterItem;
				if (kuaFuLueDuoMonsterItem == null)
				{
					return;
				}
				if (!this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuLueDuoScene))
				{
					return;
				}
				if (kuaFuLueDuoScene.m_eStatus != 2)
				{
					return;
				}
				KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
				if (null != kuaFuLueDuoClientContextData)
				{
					num = kuaFuLueDuoMonsterItem.JiFen;
					int ziYuan = kuaFuLueDuoMonsterItem.ZiYuan;
					kuaFuLueDuoClientContextData.TotalScore += num;
					kuaFuLueDuoClientContextData.ZiYuan += ziYuan;
					kuaFuLueDuoClientContextData.BangHuiContextData.TotalScore += num;
					kuaFuLueDuoClientContextData.BangHuiContextData.ZiYuan += ziYuan;
					kuaFuLueDuoScene.LeftZiYuan = Math.Max(0, kuaFuLueDuoScene.LeftZiYuan - ziYuan);
					kuaFuLueDuoMonsterItem.Alive = false;
					kuaFuLueDuoMonsterItem.FuHuoTicks = TimeUtil.NOW() + (long)(kuaFuLueDuoMonsterItem.FuHuoTime * 1000);
					monster.Tag = null;
				}
				this.SceneInfoChangeRole(kuaFuLueDuoScene, client, 0);
			}
			if (num > 0)
			{
				this.NotifyTimeStateInfoAndScoreInfo(kuaFuLueDuoScene, false, true);
			}
		}

		public void InstallJunQi(KuaFuLueDuoScene scene, CopyMap copyMap, GameClient client, QiZhiConfig item)
		{
			GameMap gameMap = GameManager.MapMgr.GetGameMap(scene.SceneInfo.MapCode);
			if (copyMap != null && null != gameMap)
			{
				item.Alive = true;
				item.BattleWhichSide = client.ClientData.BattleWhichSide;
				GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, item.MonsterId, copyMap.CopyMapID, 1, item.PosX / gameMap.MapGridWidth, item.PosY / gameMap.MapGridHeight, 0, 0, 47, item, null);
			}
		}

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			bool flag = false;
			bool flag2 = false;
			KuaFuLueDuoScene kuaFuLueDuoScene = client.SceneObject as KuaFuLueDuoScene;
			bool result;
			if (null == kuaFuLueDuoScene)
			{
				result = flag;
			}
			else
			{
				CopyMap copyMap = kuaFuLueDuoScene.CopyMap;
				lock (this.RuntimeData.Mutex)
				{
					QiZhiConfig qiZhiItem = kuaFuLueDuoScene.QiZhiItem;
					if (qiZhiItem != null && qiZhiItem.NPCID == npcExtentionID)
					{
						flag = true;
						if (qiZhiItem.Alive)
						{
							return flag;
						}
						if (client.ClientData.BattleWhichSide != qiZhiItem.BattleWhichSide && Math.Abs(TimeUtil.NOW() - qiZhiItem.DeadTicks) < 3000L)
						{
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(12, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (Math.Abs(client.ClientData.PosX - qiZhiItem.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - qiZhiItem.PosY) <= 1000)
						{
							flag2 = true;
						}
					}
					if (flag2)
					{
						this.InstallJunQi(kuaFuLueDuoScene, copyMap, client, qiZhiItem);
					}
				}
				result = flag;
			}
			return result;
		}

		public void OnProcessJunQiDead(GameClient client, Monster monster)
		{
			QiZhiConfig qiZhiConfig = monster.Tag as QiZhiConfig;
			if (null != qiZhiConfig)
			{
				lock (this.RuntimeData.Mutex)
				{
					qiZhiConfig.KillerBhid = (long)client.ClientData.Faction;
					qiZhiConfig.InstallBhName = "";
					qiZhiConfig.InstallBhid = 0L;
					qiZhiConfig.DeadTicks = TimeUtil.NOW();
					qiZhiConfig.Alive = false;
				}
			}
		}

		public bool ClientRelive(GameClient client)
		{
			bool result;
			if (!GameManager.ClientMgr.ModifyKuaFuLueDuoEnterNum(client, -1, "跨服掠夺复活", false))
			{
				KuaFuManager.getInstance().GotoLastMap(client);
				result = true;
			}
			else
			{
				int mapCode = client.ClientData.MapCode;
				KuaFuLueDuoConfig kuaFuLueDuoConfig = client.SceneInfoObject as KuaFuLueDuoConfig;
				if (null != kuaFuLueDuoConfig)
				{
					int mapCode2;
					int num;
					int num2;
					if (this.GetZhanMengBirthPoint(kuaFuLueDuoConfig, client, mapCode2 = kuaFuLueDuoConfig.MapCode, out mapCode2, out num, out num2, false))
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						client.ClientData.MoveAndActionNum = 0;
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
				result = false;
			}
			return result;
		}

		private void ProcessEnd(KuaFuLueDuoScene scene, long nowTicks)
		{
			Dictionary<int, int> dictionary = new Dictionary<int, int>();
			foreach (KuaFuLueDuoBangHuiContextData kuaFuLueDuoBangHuiContextData in scene.BangHuiContextDataDict.Values)
			{
				if (kuaFuLueDuoBangHuiContextData.ServerId != scene.ThisFuBenData.DestServerId)
				{
					int num;
					dictionary.TryGetValue(kuaFuLueDuoBangHuiContextData.ServerId, out num);
					dictionary[kuaFuLueDuoBangHuiContextData.ServerId] = num + kuaFuLueDuoBangHuiContextData.ZiYuan;
					scene.GameStatisticalData.LueDuoResultList.Add(new KuaFuLueDuoLueDuoResultData
					{
						bhid = kuaFuLueDuoBangHuiContextData.BhId,
						bhname = kuaFuLueDuoBangHuiContextData.BhName,
						zoneid = kuaFuLueDuoBangHuiContextData.ZoneID,
						ziyuan = kuaFuLueDuoBangHuiContextData.ZiYuan
					});
				}
			}
			foreach (KeyValuePair<int, int> keyValuePair in dictionary)
			{
				if ((double)keyValuePair.Value >= (double)scene.TotalZiYuan * this.RuntimeData.CrusadePerfect)
				{
					scene.GameStatisticalData.SuccessServerID = keyValuePair.Key;
					scene.GameStatisticalData.Percent = keyValuePair.Value * 100 / scene.TotalZiYuan;
					break;
				}
			}
			scene.GameStatisticalData.DestServerID = scene.ThisFuBenData.DestServerId;
			scene.GameStatisticalData.LeftZiYuan = scene.LeftZiYuan;
			scene.m_eStatus = 3;
			scene.m_lLeaveTime = nowTicks + (long)(scene.SceneInfo.ClearRolesSecs * 1000);
			scene.StateTimeData.GameType = 25;
			scene.StateTimeData.State = 5;
			scene.StateTimeData.EndTicks = scene.m_lLeaveTime;
			GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, scene.StateTimeData, scene.CopyMap);
		}

		public void TimerProc()
		{
			long num = TimeUtil.NOW();
			if (num >= KuaFuLueDuoManager.NextHeartBeatTicks)
			{
				KuaFuLueDuoManager.NextHeartBeatTicks = num + 1020L;
				List<KuaFuLueDuoScene> list = new List<KuaFuLueDuoScene>();
				lock (this.RuntimeData.Mutex)
				{
					foreach (KuaFuLueDuoScene kuaFuLueDuoScene in this.SceneDict.Values)
					{
						int fuBenSeqId = kuaFuLueDuoScene.FuBenSeqId;
						if (fuBenSeqId >= 0)
						{
							if (TimeUtil.NOW() - kuaFuLueDuoScene.StartTimeTicks > 86400000L)
							{
								list.Add(kuaFuLueDuoScene);
							}
							DateTime dateTime = TimeUtil.NowDateTime();
							long num2 = TimeUtil.NOW();
							CopyMap copyMap = kuaFuLueDuoScene.CopyMap;
							if (kuaFuLueDuoScene.m_eStatus == 0)
							{
								if (num2 >= kuaFuLueDuoScene.StartTimeTicks)
								{
									kuaFuLueDuoScene.m_lPrepareTime = kuaFuLueDuoScene.StartTimeTicks;
									kuaFuLueDuoScene.m_lBeginTime = kuaFuLueDuoScene.m_lPrepareTime + (long)(kuaFuLueDuoScene.SceneInfo.PrepareSecs * 1000);
									kuaFuLueDuoScene.m_eStatus = 1;
									kuaFuLueDuoScene.StateTimeData.GameType = 25;
									kuaFuLueDuoScene.StateTimeData.State = kuaFuLueDuoScene.m_eStatus;
									kuaFuLueDuoScene.StateTimeData.EndTicks = kuaFuLueDuoScene.m_lBeginTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kuaFuLueDuoScene.StateTimeData, copyMap);
								}
							}
							else if (kuaFuLueDuoScene.m_eStatus == 1)
							{
								if (num2 >= kuaFuLueDuoScene.m_lBeginTime)
								{
									kuaFuLueDuoScene.m_eStatus = 2;
									kuaFuLueDuoScene.m_lEndTime = kuaFuLueDuoScene.m_lBeginTime + (long)(kuaFuLueDuoScene.SceneInfo.FightingSecs * 1000);
									kuaFuLueDuoScene.StateTimeData.GameType = 25;
									kuaFuLueDuoScene.StateTimeData.State = kuaFuLueDuoScene.m_eStatus;
									kuaFuLueDuoScene.StateTimeData.EndTicks = kuaFuLueDuoScene.m_lEndTime;
									GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, kuaFuLueDuoScene.StateTimeData, copyMap);
									for (int i = 1; i <= 4; i++)
									{
										GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
									}
								}
							}
							else if (kuaFuLueDuoScene.m_eStatus == 2)
							{
								if (num2 >= kuaFuLueDuoScene.m_lEndTime || kuaFuLueDuoScene.LeftZiYuan <= 0)
								{
									this.ProcessEnd(kuaFuLueDuoScene, num2);
								}
								else
								{
									this.CheckSceneScoreTime(kuaFuLueDuoScene, num2);
									this.CheckCreateDynamicMonster(kuaFuLueDuoScene, num2);
								}
							}
							else if (kuaFuLueDuoScene.m_eStatus == 3)
							{
								kuaFuLueDuoScene.m_eStatus = 4;
								this.GiveAwards(kuaFuLueDuoScene);
								GameManager.CopyMapMgr.KillAllMonster(copyMap);
								KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData;
								if (this.RuntimeData.FuBenItemData.TryGetValue((long)kuaFuLueDuoScene.GameId, out kuaFuLueDuoFuBenData))
								{
									LogManager.WriteLog(2, string.Format("跨服掠夺跨服副本GameID={0},战斗结束", kuaFuLueDuoFuBenData.GameId), null, true);
									this.RuntimeData.FuBenItemData.Remove((long)kuaFuLueDuoScene.GameId);
								}
							}
							else if (kuaFuLueDuoScene.m_eStatus == 4)
							{
								if (num2 >= kuaFuLueDuoScene.m_lLeaveTime)
								{
									kuaFuLueDuoScene.ThisFuBenData.State = 3;
									copyMap.SetRemoveTicks(kuaFuLueDuoScene.m_lLeaveTime);
									kuaFuLueDuoScene.m_eStatus = 5;
									list.Add(kuaFuLueDuoScene);
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
										DataHelper.WriteExceptionLogEx(ex, "跨服掠夺系统清场调度异常");
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
						foreach (KuaFuLueDuoScene kuaFuLueDuoScene in list)
						{
							KuaFuLueDuoScene kuaFuLueDuoScene2;
							this.SceneDict.TryRemove(kuaFuLueDuoScene.FuBenSeqId, out kuaFuLueDuoScene2);
						}
					}
				}
			}
		}

		public void CheckCreateDynamicMonster(KuaFuLueDuoScene scene, long nowMs)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (scene.m_eStatus == 2)
				{
					foreach (KuaFuLueDuoMonsterItem kuaFuLueDuoMonsterItem in scene.CollectMonsterXml.Values)
					{
						try
						{
							if (!kuaFuLueDuoMonsterItem.Alive && kuaFuLueDuoMonsterItem.FuHuoTicks <= nowMs)
							{
								if (kuaFuLueDuoMonsterItem.Type == 1 && scene.SmallZiYuanCount > 0)
								{
									scene.SmallZiYuanCount--;
								}
								else
								{
									if (kuaFuLueDuoMonsterItem.Type != 2 || scene.BigZiYuanCount <= 0)
									{
										continue;
									}
									scene.BigZiYuanCount--;
								}
								kuaFuLueDuoMonsterItem.Alive = true;
								GameManager.MonsterZoneMgr.AddDynamicMonsters(scene.CopyMap.MapCode, kuaFuLueDuoMonsterItem.MonsterID, scene.CopyMap.CopyMapID, 1, kuaFuLueDuoMonsterItem.X / scene.MapGridWidth, kuaFuLueDuoMonsterItem.Y / scene.MapGridHeight, 0, 0, 47, kuaFuLueDuoMonsterItem, null);
							}
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool sideScore = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoScene kuaFuLueDuoScene;
				if (this.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out kuaFuLueDuoScene))
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, kuaFuLueDuoScene.StateTimeData, false);
					}
					if (sideScore)
					{
						KuaFuLueDuoScoreData kuaFuLueDuoScoreData = new KuaFuLueDuoScoreData();
						kuaFuLueDuoScoreData.LeftZiYuan = kuaFuLueDuoScene.LeftZiYuan;
						KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
						if (null != kuaFuLueDuoClientContextData)
						{
							kuaFuLueDuoScoreData.LueDuoZiYuan = kuaFuLueDuoClientContextData.BangHuiContextData.ZiYuan;
							kuaFuLueDuoScoreData.SelfScore = kuaFuLueDuoClientContextData.TotalScore;
							kuaFuLueDuoScoreData.LeftNum = kuaFuLueDuoClientContextData.LeftNum;
							client.sendCmd<KuaFuLueDuoScoreData>(1253, kuaFuLueDuoScoreData, false);
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(KuaFuLueDuoScene scene, bool timeState = true, bool sideScore = true)
		{
			List<GameClient> clientsList = scene.CopyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				KuaFuLueDuoScoreData kuaFuLueDuoScoreData = new KuaFuLueDuoScoreData();
				kuaFuLueDuoScoreData.LeftZiYuan = scene.LeftZiYuan;
				foreach (GameClient gameClient in clientsList)
				{
					if (timeState)
					{
						gameClient.sendCmd<GameSceneStateTimeData>(827, scene.StateTimeData, false);
					}
					KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = gameClient.SceneContextData2 as KuaFuLueDuoClientContextData;
					if (null != kuaFuLueDuoClientContextData)
					{
						kuaFuLueDuoScoreData.LueDuoZiYuan = kuaFuLueDuoClientContextData.BangHuiContextData.ZiYuan;
						kuaFuLueDuoScoreData.SelfScore = kuaFuLueDuoClientContextData.TotalScore;
						kuaFuLueDuoScoreData.LeftNum = kuaFuLueDuoClientContextData.LeftNum;
						gameClient.sendCmd<KuaFuLueDuoScoreData>(1253, kuaFuLueDuoScoreData, false);
					}
				}
			}
		}

		public void OnKillRole(GameClient client, GameClient other)
		{
			lock (this.RuntimeData.Mutex)
			{
				KuaFuLueDuoScene kuaFuLueDuoScene = client.SceneObject as KuaFuLueDuoScene;
				if (kuaFuLueDuoScene != null && kuaFuLueDuoScene.m_eStatus == 2)
				{
					int num = 0;
					KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
					KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData2 = other.SceneContextData2 as KuaFuLueDuoClientContextData;
					if (kuaFuLueDuoClientContextData != null && kuaFuLueDuoClientContextData2 != null)
					{
						kuaFuLueDuoClientContextData.KillNum++;
						kuaFuLueDuoClientContextData.Kill++;
						kuaFuLueDuoClientContextData.TotalKill++;
						int v = this.RuntimeData.CrusadeUltraKill[0] + this.RuntimeData.CrusadeUltraKill[1] * kuaFuLueDuoClientContextData.KillNum;
						num += Global.Clamp(v, this.RuntimeData.CrusadeUltraKill[2], this.RuntimeData.CrusadeUltraKill[3]);
						if (kuaFuLueDuoClientContextData2.KillNum >= 2)
						{
							v = this.RuntimeData.CrusadeShutDown[0] + this.RuntimeData.CrusadeShutDown[1] * kuaFuLueDuoClientContextData2.KillNum;
							num += Global.Clamp(v, this.RuntimeData.CrusadeShutDown[2], this.RuntimeData.CrusadeShutDown[3]);
						}
						kuaFuLueDuoClientContextData2.KillNum = 0;
						kuaFuLueDuoClientContextData.TotalScore += num;
						kuaFuLueDuoClientContextData.BangHuiContextData.TotalScore += num;
					}
				}
			}
		}

		public void LeaveFuBen(GameClient client)
		{
			KuaFuLueDuoScene kuaFuLueDuoScene = client.SceneObject as KuaFuLueDuoScene;
			if (null != kuaFuLueDuoScene)
			{
				this.SceneInfoChangeRole(kuaFuLueDuoScene, client, -1);
			}
			KuaFuLueDuoClientContextData kuaFuLueDuoClientContextData = client.SceneContextData2 as KuaFuLueDuoClientContextData;
			if (kuaFuLueDuoClientContextData.Kill > 0)
			{
				if (kuaFuLueDuoClientContextData.BattleWhichSide == 1)
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KuaFuLueDuo_Defender, new int[]
					{
						kuaFuLueDuoClientContextData.Kill
					}));
				}
				else
				{
					GlobalEventSource.getInstance().fireEvent(new OrnamentGoalEventObject(client, OrnamentGoalType.OGT_KuaFuLueDuo_Attacker, new int[]
					{
						kuaFuLueDuoClientContextData.Kill
					}));
				}
				kuaFuLueDuoClientContextData.Kill = 0;
			}
		}

		public void OnLogout(GameClient client)
		{
			this.LeaveFuBen(client);
		}

		public void OnStartPlayGame(GameClient client)
		{
			KuaFuLueDuoScene kuaFuLueDuoScene = client.SceneObject as KuaFuLueDuoScene;
			if (null != kuaFuLueDuoScene)
			{
				this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
				this.SceneInfoChangeRole(kuaFuLueDuoScene, client, 0);
			}
		}

		private void SceneInfoChangeRole(KuaFuLueDuoScene scene, GameClient client, int addNum = 0)
		{
			int faction = client.ClientData.Faction;
			int destServerId = scene.ThisFuBenData.DestServerId;
			if (scene != null && scene.CopyMap != null)
			{
				FightInfo fightInfo = new FightInfo
				{
					RoleNum = addNum
				};
				FightInfo fightInfo2 = new FightInfo
				{
					RoleNum = addNum
				};
				List<GameClient> clientsList = scene.CopyMap.GetClientsList();
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.UpdateZiYuanData = true;
					this.RuntimeData.ServerZiYuanDict[destServerId] = fightInfo;
					this.RuntimeData.BhZiYuanDict[faction] = fightInfo2;
					fightInfo.ZiYuan = scene.LeftZiYuan;
					foreach (GameClient gameClient in clientsList)
					{
						if (gameClient.ServerId == destServerId)
						{
							fightInfo.RoleNum++;
						}
						else if (faction == gameClient.ClientData.Faction)
						{
							fightInfo2.RoleNum++;
						}
					}
					foreach (KuaFuLueDuoBangHuiContextData kuaFuLueDuoBangHuiContextData in scene.BangHuiContextDataDict.Values)
					{
						if (kuaFuLueDuoBangHuiContextData.BhId == faction)
						{
							fightInfo2.ZiYuan = kuaFuLueDuoBangHuiContextData.ZiYuan;
							break;
						}
					}
				}
			}
		}

		private void CheckSceneScoreTime(KuaFuLueDuoScene scene, long nowTicks)
		{
			lock (this.RuntimeData.Mutex)
			{
				bool flag2 = true;
				if (flag2)
				{
					this.NotifyTimeStateInfoAndScoreInfo(scene, true, true);
				}
			}
		}

		private int CalMVPScore(KuaFuLueDuoScene scene, int factor)
		{
			int num = (int)(TimeUtil.NOW() - scene.m_lBeginTime) / 1000;
			return (int)((1.0 + (double)num / 60.0 * 0.075) * (double)factor);
		}

		public const SceneUIClasses ManagerType = 47;

		private static KuaFuLueDuoManager instance = new KuaFuLueDuoManager();

		public KuaFuLueDuoData RuntimeData = new KuaFuLueDuoData();

		public KuaFuLueDuoSyncData SyncDataCache = new KuaFuLueDuoSyncData();

		public KuaFuLueDuoSyncData RequestSyncData = new KuaFuLueDuoSyncData();

		private RoleDataEx OwnerRoleData = null;

		public ConcurrentDictionary<int, KuaFuLueDuoScene> SceneDict = new ConcurrentDictionary<int, KuaFuLueDuoScene>();

		private static long NextHeartBeatTicks = 0L;
	}
}
