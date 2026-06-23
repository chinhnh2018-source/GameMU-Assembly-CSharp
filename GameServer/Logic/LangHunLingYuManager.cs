using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Logic.ActivityNew;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class LangHunLingYuManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, IManager2
	{
		public static LangHunLingYuManager getInstance()
		{
			return LangHunLingYuManager.instance;
		}

		public bool initialize()
		{
			return this.InitConfig();
		}

		public bool initialize(ICoreInterface coreInterface)
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1153, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1154, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1156, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1155, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1157, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1158, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1160, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1161, 1, 1, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1162, 2, 2, LangHunLingYuManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(12, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(23, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(24, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(25, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(26, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10015, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10016, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10017, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10018, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10019, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(30, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(33, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(29, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(27, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(11, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(28, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().registerListener(14, LangHunLingYuManager.getInstance());
			return true;
		}

		public bool startup()
		{
			ScheduleExecutor2.Instance.scheduleExecute(new NormalScheduleTask("LangHunLingYuManager.TimerProc", new EventHandler(this.TimerProc)), 15000, 1428);
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(11, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(12, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(23, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(24, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(25, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(26, 10000, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10001, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10015, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10016, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10017, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10018, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource4Scene.getInstance().removeListener(10019, 35, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(28, LangHunLingYuManager.getInstance());
			GlobalEventSource.getInstance().removeListener(14, LangHunLingYuManager.getInstance());
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
			case 1153:
				return this.ProcessLangHunLingYuJoinCmd(client, nID, bytes, cmdParams);
			case 1154:
				return this.ProcessLangHunLingYuRoleDataCmd(client, nID, bytes, cmdParams);
			case 1155:
				return this.ProcessLangHunLingYuCityDataCmd(client, nID, bytes, cmdParams);
			case 1156:
				return this.ProcessLangHunLingYuWorldDataCmd(client, nID, bytes, cmdParams);
			case 1157:
				return this.ProcessLangHunLingYuEnterCmd(client, nID, bytes, cmdParams);
			case 1158:
				return this.ProcessGetDailyAwardsCmd(client, nID, bytes, cmdParams);
			case 1160:
				return this.ProcessGetAdmireDataCmd(client, nID, bytes, cmdParams);
			case 1161:
				return this.ProcessGetAdmireHistoryCmd(client, nID, bytes, cmdParams);
			case 1162:
				return this.ProcessAdmireCmd(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			int num = eventType;
			if (num != 11)
			{
				if (num != 14)
				{
					if (num == 28)
					{
						OnStartPlayGameEventObject onStartPlayGameEventObject = eventObject as OnStartPlayGameEventObject;
						if (onStartPlayGameEventObject.Client.SceneType == 35)
						{
							YongZheZhanChangClient.getInstance().ChangeRoleState(onStartPlayGameEventObject.Client.ClientData.RoleID, 0, false);
							this.OnStartPlayGame(onStartPlayGameEventObject.Client);
						}
					}
				}
				else
				{
					PlayerInitGameEventObject playerInitGameEventObject = eventObject as PlayerInitGameEventObject;
					if (null != playerInitGameEventObject)
					{
						this.OnInitGame(playerInitGameEventObject.getPlayer());
					}
				}
			}
			else
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
			{
				PreBangHuiChangeZhiWuEventObject preBangHuiChangeZhiWuEventObject = eventObject as PreBangHuiChangeZhiWuEventObject;
				if (null != preBangHuiChangeZhiWuEventObject)
				{
					if ((long)preBangHuiChangeZhiWuEventObject.Player.ClientData.Faction == this.RuntimeData.ChengHaoBHid && preBangHuiChangeZhiWuEventObject.TargetZhiWu == 1)
					{
						LogManager.WriteLog(1, string.Format("圣域城主禁止委任首领职务", new object[0]), null, true);
						eventObject.Handled = true;
						eventObject.Result = false;
					}
				}
				break;
			}
			case 26:
			{
				PostBangHuiChangeEventObject postBangHuiChangeEventObject = eventObject as PostBangHuiChangeEventObject;
				if (postBangHuiChangeEventObject != null && null != postBangHuiChangeEventObject.Player)
				{
					this.UpdateChengHaoBuffer(postBangHuiChangeEventObject.Player, 0L, this.RuntimeData.ChengHaoBHid);
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
					QiZhiConfig qiZhiConfig = onCreateMonsterEventObject.Monster.Tag as QiZhiConfig;
					if (null != qiZhiConfig)
					{
						onCreateMonsterEventObject.Monster.MonsterName = qiZhiConfig.InstallBhName;
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
				if (null != preMonsterInjureEventObject)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.JunQiMonsterHashSet.Contains(preMonsterInjureEventObject.Monster.MonsterInfo.ExtensionID))
						{
							preMonsterInjureEventObject.Injure = this.RuntimeData.CutLifeV;
							preMonsterInjureEventObject.Result = true;
							preMonsterInjureEventObject.Handled = true;
						}
					}
				}
				break;
			}
			default:
				if (num != 10001)
				{
					switch (num)
					{
					case 10015:
					{
						NotifyLhlyBangHuiDataGameEvent notifyLhlyBangHuiDataGameEvent = eventObject as NotifyLhlyBangHuiDataGameEvent;
						if (null != notifyLhlyBangHuiDataGameEvent)
						{
							this.UpdateBangHuiDataEx(notifyLhlyBangHuiDataGameEvent.Arg as LangHunLingYuBangHuiDataEx);
							notifyLhlyBangHuiDataGameEvent.Handled = (notifyLhlyBangHuiDataGameEvent.Result = true);
						}
						break;
					}
					case 10016:
					{
						NotifyLhlyCityDataGameEvent notifyLhlyCityDataGameEvent = eventObject as NotifyLhlyCityDataGameEvent;
						if (null != notifyLhlyCityDataGameEvent)
						{
							this.UpdateCityDataEx(notifyLhlyCityDataGameEvent.Arg as LangHunLingYuCityDataEx);
							notifyLhlyCityDataGameEvent.Handled = (notifyLhlyCityDataGameEvent.Result = true);
						}
						break;
					}
					case 10017:
					{
						NotifyLhlyOtherCityListGameEvent notifyLhlyOtherCityListGameEvent = eventObject as NotifyLhlyOtherCityListGameEvent;
						if (null != notifyLhlyOtherCityListGameEvent)
						{
							this.UpdateOtherCityList(notifyLhlyOtherCityListGameEvent.Arg);
							notifyLhlyOtherCityListGameEvent.Handled = (notifyLhlyOtherCityListGameEvent.Result = true);
						}
						break;
					}
					case 10018:
					{
						NotifyLhlyCityOwnerHistGameEvent notifyLhlyCityOwnerHistGameEvent = eventObject as NotifyLhlyCityOwnerHistGameEvent;
						if (null != notifyLhlyCityOwnerHistGameEvent)
						{
							this.UpdateCityOwnerHist(notifyLhlyCityOwnerHistGameEvent.Arg);
							notifyLhlyCityOwnerHistGameEvent.Handled = (notifyLhlyCityOwnerHistGameEvent.Result = true);
						}
						break;
					}
					case 10019:
					{
						NotifyLhlyCityOwnerAdmireGameEvent notifyLhlyCityOwnerAdmireGameEvent = eventObject as NotifyLhlyCityOwnerAdmireGameEvent;
						if (null != notifyLhlyCityOwnerAdmireGameEvent)
						{
							this.UpdateCityOwnerAdmire(notifyLhlyCityOwnerAdmireGameEvent.RoleID, notifyLhlyCityOwnerAdmireGameEvent.AdmireCount);
							notifyLhlyCityOwnerAdmireGameEvent.Handled = (notifyLhlyCityOwnerAdmireGameEvent.Result = true);
						}
						break;
					}
					}
				}
				else
				{
					KuaFuNotifyEnterGameEvent kuaFuNotifyEnterGameEvent = eventObject as KuaFuNotifyEnterGameEvent;
					if (null != kuaFuNotifyEnterGameEvent)
					{
						LogManager.WriteLog(2, string.Format("通知角色ID={0}拥有进入勇者战场资格,跨服GameID={1}", 0, 0), null, true);
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
					this.RuntimeData.CutLifeV = (int)GameManager.systemParamsList.GetParamValueIntByName("CutLifeV", 10);
					this.RuntimeData.CityLevelInfoDict.Clear();
					text = "Config/City.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					int num = 0;
					foreach (XElement xml in enumerable)
					{
						CityLevelInfo cityLevelInfo = new CityLevelInfo();
						cityLevelInfo.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						cityLevelInfo.CityLevel = (int)Global.GetSafeAttributeLong(xml, "CityLevel");
						cityLevelInfo.CityNum = (int)Global.GetSafeAttributeLong(xml, "CityNum");
						cityLevelInfo.MaxNum = (int)Global.GetSafeAttributeLong(xml, "MaxNum");
						cityLevelInfo.ZhanMengZiJin = (int)Global.GetSafeAttributeLong(xml, "ZhanMengZiJin");
						cityLevelInfo.AttackWeekDay = Global.StringToIntList(Global.GetSafeAttributeStr(xml, "AttackWeekDay"), ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "Award"), ref cityLevelInfo.Award, '|', ',');
						ConfigParser.ParseAwardsItemList(Global.GetSafeAttributeStr(xml, "DayAward"), ref cityLevelInfo.DayAward, '|', ',');
						if (!ConfigParser.ParserTimeRangeListWithDay(cityLevelInfo.BaoMingTime, Global.GetSafeAttributeStr(xml, "BaoMingTime").Replace(';', '|'), true, '|', '-', ','))
						{
							LogManager.WriteLog(1000, string.Format("解析文件{0}的BaoMingTime出错", text), null, true);
							return false;
						}
						if (!ConfigParser.ParserTimeRangeList(cityLevelInfo.AttackTime, Global.GetSafeAttributeStr(xml, "AttackTime"), true, '|', '-'))
						{
							LogManager.WriteLog(1000, string.Format("解析文件{0}的BaoMingTime出错", text), null, true);
							return false;
						}
						this.RuntimeData.CityLevelInfoDict[cityLevelInfo.CityLevel] = cityLevelInfo;
						for (int i = 0; i < cityLevelInfo.CityNum; i++)
						{
							num++;
							this.RuntimeData.CityDataExDict.Add(num, new LangHunLingYuCityDataEx
							{
								CityId = num,
								CityLevel = cityLevelInfo.CityLevel
							});
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.MapBirthPointListDict.Clear();
					text = "Config/SiegeWarfareBirthPoint.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						MapBirthPoint mapBirthPoint = new MapBirthPoint();
						mapBirthPoint.ID = (int)Global.GetSafeAttributeLong(xml, "ID");
						mapBirthPoint.Type = (int)Global.GetSafeAttributeLong(xml, "Type");
						mapBirthPoint.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode");
						mapBirthPoint.BirthPosX = (int)Global.GetSafeAttributeLong(xml, "BirthPosX");
						mapBirthPoint.BirthPosY = (int)Global.GetSafeAttributeLong(xml, "BirthPosY");
						mapBirthPoint.BirthRangeX = (int)Global.GetSafeAttributeLong(xml, "BirthRangeX");
						mapBirthPoint.BirthRangeY = (int)Global.GetSafeAttributeLong(xml, "BirthRangeY");
						List<MapBirthPoint> list;
						if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(mapBirthPoint.Type, out list))
						{
							list = new List<MapBirthPoint>();
							this.RuntimeData.MapBirthPointListDict.Add(mapBirthPoint.Type, list);
						}
						list.Add(mapBirthPoint);
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					this.RuntimeData.NPCID2QiZhiConfigDict.Clear();
					this.RuntimeData.QiZhiBuffOwnerDataList.Clear();
					this.RuntimeData.QiZhiBuffDisableParamsDict.Clear();
					this.RuntimeData.QiZhiBuffEnableParamsDict.Clear();
					text = "Config/CityWarQiZuo.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						QiZhiConfig qiZhiConfig = new QiZhiConfig();
						qiZhiConfig.NPCID = (int)Global.GetSafeAttributeLong(xml, "NPCID");
						qiZhiConfig.BufferID = (int)Global.GetSafeAttributeLong(xml, "BufferID");
						qiZhiConfig.PosX = (int)Global.GetSafeAttributeLong(xml, "PosX");
						qiZhiConfig.PosY = (int)Global.GetSafeAttributeLong(xml, "PosY");
						qiZhiConfig.MonsterId = (int)Global.GetSafeAttributeLong(xml, "JuQiID");
						this.RuntimeData.JunQiMonsterHashSet.Add(qiZhiConfig.MonsterId);
						this.RuntimeData.NPCID2QiZhiConfigDict[qiZhiConfig.NPCID] = qiZhiConfig;
						this.RuntimeData.QiZhiBuffOwnerDataList.Add(new LangHunLingYuQiZhiBuffOwnerData
						{
							NPCID = qiZhiConfig.NPCID,
							OwnerBHName = ""
						});
						this.RuntimeData.QiZhiBuffDisableParamsDict[qiZhiConfig.BufferID] = new double[]
						{
							0.0,
							(double)qiZhiConfig.BufferID
						};
						this.RuntimeData.QiZhiBuffEnableParamsDict[qiZhiConfig.BufferID] = new double[]
						{
							0.0,
							(double)qiZhiConfig.BufferID
						};
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
				try
				{
					QiZhiConfig qiZhiConfig2;
					if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(this.RuntimeData.SuperQiZhiNpcId, out qiZhiConfig2))
					{
						this.RuntimeData.SuperQiZhiOwnerBirthPosX = qiZhiConfig2.PosX;
						this.RuntimeData.SuperQiZhiOwnerBirthPosY = qiZhiConfig2.PosY;
					}
					this.RuntimeData.SceneDataDict.Clear();
					this.RuntimeData.LevelRangeSceneIdDict.Clear();
					text = "Config/CityWar.xml";
					string uri = Global.GameResPath(text);
					XElement xelement = XElement.Load(uri);
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						LangHunLingYuSceneInfo langHunLingYuSceneInfo = new LangHunLingYuSceneInfo();
						int num2 = (int)Global.GetSafeAttributeLong(xml, "ID");
						langHunLingYuSceneInfo.Id = num2;
						langHunLingYuSceneInfo.MapCode = (int)Global.GetSafeAttributeLong(xml, "MapCode1");
						langHunLingYuSceneInfo.MapCode_LongTa = (int)Global.GetSafeAttributeLong(xml, "MapCode2");
						langHunLingYuSceneInfo.MinLevel = (int)Global.GetSafeAttributeLong(xml, "MinLevel");
						langHunLingYuSceneInfo.MaxLevel = 10000;
						langHunLingYuSceneInfo.MinZhuanSheng = (int)Global.GetSafeAttributeLong(xml, "MinZhuanSheng");
						langHunLingYuSceneInfo.MaxZhuanSheng = 10000;
						langHunLingYuSceneInfo.PrepareSecs = (int)Global.GetSafeAttributeLong(xml, "PrepareSecs");
						langHunLingYuSceneInfo.WaitingEnterSecs = (int)Global.GetSafeAttributeLong(xml, "WaitingEnterSecs");
						langHunLingYuSceneInfo.FightingSecs = (int)Global.GetSafeAttributeLong(xml, "FightingSecs");
						langHunLingYuSceneInfo.ClearRolesSecs = (int)Global.GetSafeAttributeLong(xml, "ClearRolesSecs");
						GameMap gameMap = null;
						if (!GameManager.MapMgr.DictMaps.TryGetValue(langHunLingYuSceneInfo.MapCode, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, langHunLingYuSceneInfo.MapCode), null, true);
						}
						if (!GameManager.MapMgr.DictMaps.TryGetValue(langHunLingYuSceneInfo.MapCode_LongTa, out gameMap))
						{
							result = false;
							LogManager.WriteLog(1000, string.Format("地图配置中缺少{0}所需的地图:{1}", text, langHunLingYuSceneInfo.MapCode_LongTa), null, true);
						}
						RangeKey key = new RangeKey(Global.GetUnionLevel(langHunLingYuSceneInfo.MinZhuanSheng, langHunLingYuSceneInfo.MinLevel, false), Global.GetUnionLevel(langHunLingYuSceneInfo.MaxZhuanSheng, langHunLingYuSceneInfo.MaxLevel, false), null);
						this.RuntimeData.LevelRangeSceneIdDict[key] = langHunLingYuSceneInfo;
						this.RuntimeData.SceneDataDict[num2] = langHunLingYuSceneInfo;
						this.RuntimeData.SceneInfoId = num2;
						this.RuntimeData.SceneDataList.Add(langHunLingYuSceneInfo);
					}
				}
				catch (Exception ex)
				{
					result = false;
					LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				}
				try
				{
					text = "Config/SiegeWarfareExp.xml";
					this._LevelAwardsMgr.LoadFromXMlFile(text, "", "ID", 0);
				}
				catch (Exception ex)
				{
					LogManager.WriteException(string.Format("加载xml配置文件:{0}, 失败。{1}", text, ex.ToString()));
					return false;
				}
			}
			this.RestoreLangHunLingYuNpc();
			return result;
		}

		public int GetCityLevelById(int cityId)
		{
			lock (this.RuntimeData.Mutex)
			{
				LangHunLingYuCityDataEx langHunLingYuCityDataEx;
				if (this.RuntimeData.CityDataExDict.TryGetValue(cityId, out langHunLingYuCityDataEx))
				{
					return langHunLingYuCityDataEx.CityLevel;
				}
			}
			return 0;
		}

		public int GetBangHuiCityLevel(int bhid)
		{
			lock (this.RuntimeData.Mutex)
			{
				LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
				if (this.RuntimeData.BangHuiDataExDict.TryGetValue((long)bhid, out langHunLingYuBangHuiDataEx))
				{
					return langHunLingYuBangHuiDataEx.Level;
				}
			}
			return 0;
		}

		public string GetBangHuiName(int bhid, out int zoneId)
		{
			zoneId = 0;
			LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
			string result;
			if (!this.RuntimeData.BangHuiDataExDict.TryGetValue((long)bhid, out langHunLingYuBangHuiDataEx))
			{
				result = GLang.GetLang(6, new object[0]);
			}
			else
			{
				zoneId = langHunLingYuBangHuiDataEx.ZoneId;
				result = langHunLingYuBangHuiDataEx.BhName;
			}
			return result;
		}

		private void UpdateCityDataEx(LangHunLingYuCityDataEx cityDataEx)
		{
			if (null != cityDataEx)
			{
				lock (this.RuntimeData.Mutex)
				{
					HashSet<long> hashSet = new HashSet<long>();
					HashSet<long> hashSet2 = new HashSet<long>();
					HashSet<long> hashSet3 = new HashSet<long>();
					HashSet<long> hashSet4 = new HashSet<long>();
					HashSet<long> hashSet5 = new HashSet<long>();
					hashSet = new HashSet<long>(cityDataEx.Site);
					hashSet5.UnionWith(cityDataEx.Site);
					LangHunLingYuCityDataEx langHunLingYuCityDataEx;
					if (this.RuntimeData.CityDataExDict.TryGetValue(cityDataEx.CityId, out langHunLingYuCityDataEx))
					{
						hashSet.ExceptWith(langHunLingYuCityDataEx.Site);
						hashSet2 = new HashSet<long>(langHunLingYuCityDataEx.Site);
						hashSet2.ExceptWith(cityDataEx.Site);
						for (int i = 1; i < langHunLingYuCityDataEx.Site.Length; i++)
						{
							long num = langHunLingYuCityDataEx.Site[i];
							if (num > 0L && !hashSet3.Contains(num))
							{
								hashSet3.Add(num);
							}
						}
						hashSet5.UnionWith(langHunLingYuCityDataEx.Site);
					}
					for (int i = 1; i < cityDataEx.Site.Length; i++)
					{
						long num = cityDataEx.Site[i];
						if (num > 0L && !hashSet4.Contains(num))
						{
							hashSet4.Add(num);
						}
					}
					this.RuntimeData.CityDataExDict[cityDataEx.CityId] = cityDataEx;
					LangHunLingYuCityData langHunLingYuCityData;
					if (!this.RuntimeData.CityDataDict.TryGetValue(cityDataEx.CityId, out langHunLingYuCityData))
					{
						langHunLingYuCityData = new LangHunLingYuCityData();
						langHunLingYuCityData.CityId = cityDataEx.CityId;
						langHunLingYuCityData.CityLevel = cityDataEx.CityLevel;
						this.RuntimeData.CityDataDict[cityDataEx.CityId] = langHunLingYuCityData;
					}
					LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
					if (this.RuntimeData.BangHuiDataExDict.TryGetValue(cityDataEx.Site[0], out langHunLingYuBangHuiDataEx))
					{
						langHunLingYuCityData.Owner = new BangHuiMiniData
						{
							BHID = langHunLingYuBangHuiDataEx.Bhid,
							BHName = langHunLingYuBangHuiDataEx.BhName,
							ZoneID = langHunLingYuBangHuiDataEx.ZoneId
						};
					}
					else
					{
						langHunLingYuCityData.Owner = null;
					}
					langHunLingYuCityData.AttackerList.Clear();
					for (int i = 1; i < cityDataEx.Site.Length; i++)
					{
						long num2 = cityDataEx.Site[i];
						if (num2 > 0L && this.RuntimeData.BangHuiDataExDict.TryGetValue(num2, out langHunLingYuBangHuiDataEx))
						{
							langHunLingYuCityData.AttackerList.Add(new BangHuiMiniData
							{
								BHID = langHunLingYuBangHuiDataEx.Bhid,
								BHName = langHunLingYuBangHuiDataEx.BhName,
								ZoneID = langHunLingYuBangHuiDataEx.ZoneId
							});
						}
					}
					foreach (long num2 in hashSet)
					{
						long num2;
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (!this.RuntimeData.BangHuiDataDict.TryGetValue(num2, out langHunLingYuBangHuiData))
						{
							langHunLingYuBangHuiData = new LangHunLingYuBangHuiData();
							this.RuntimeData.BangHuiDataDict[num2] = langHunLingYuBangHuiData;
						}
						langHunLingYuBangHuiData.SelfCityList.Add(cityDataEx.CityId);
						langHunLingYuBangHuiData.SelfCityList.Sort();
					}
					foreach (long num2 in hashSet2)
					{
						long num2;
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (!this.RuntimeData.BangHuiDataDict.TryGetValue(num2, out langHunLingYuBangHuiData))
						{
							langHunLingYuBangHuiData = new LangHunLingYuBangHuiData();
							this.RuntimeData.BangHuiDataDict[num2] = langHunLingYuBangHuiData;
						}
						langHunLingYuBangHuiData.SelfCityList.Remove(cityDataEx.CityId);
					}
					foreach (long num2 in hashSet4.Except(hashSet3))
					{
						long num2;
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (this.RuntimeData.BangHuiDataDict.TryGetValue(num2, out langHunLingYuBangHuiData))
						{
							langHunLingYuBangHuiData.SignUpState = 1;
						}
					}
					foreach (long num2 in hashSet3.Except(hashSet4))
					{
						long num2;
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (this.RuntimeData.BangHuiDataDict.TryGetValue(num2, out langHunLingYuBangHuiData))
						{
							langHunLingYuBangHuiData.SignUpState = 0;
						}
					}
					long num3 = 0L;
					if (this.RuntimeData.CityDataExDict.TryGetValue(1, out cityDataEx))
					{
						num3 = cityDataEx.Site[0];
						this.ReplaceLangHunLingYuNpc();
					}
					foreach (long num2 in hashSet5)
					{
						long num2;
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (this.RuntimeData.BangHuiDataDict.TryGetValue(num2, out langHunLingYuBangHuiData))
						{
							langHunLingYuBangHuiData.DayAwardFlags = 0;
							foreach (int key in langHunLingYuBangHuiData.SelfCityList)
							{
								if (this.RuntimeData.CityDataExDict.TryGetValue(key, out cityDataEx))
								{
									if (cityDataEx.Site[0] == num2)
									{
										langHunLingYuBangHuiData.DayAwardFlags = Global.SetIntSomeBit(cityDataEx.CityLevel, langHunLingYuBangHuiData.DayAwardFlags, true);
									}
								}
							}
						}
					}
					this.BroadcastBangHuiCityData(hashSet5, this.RuntimeData.ChengHaoBHid, num3);
					this.RuntimeData.ChengHaoBHid = num3;
				}
			}
		}

		private void UpdateBangHuiDataEx(LangHunLingYuBangHuiDataEx bangHuiDataEx)
		{
			if (null != bangHuiDataEx)
			{
				lock (this.RuntimeData.Mutex)
				{
					this.RuntimeData.BangHuiDataExDict[(long)bangHuiDataEx.Bhid] = bangHuiDataEx;
					LangHunLingYuBangHuiData value;
					if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)bangHuiDataEx.Bhid, out value))
					{
						value = new LangHunLingYuBangHuiData();
						this.RuntimeData.BangHuiDataDict[(long)bangHuiDataEx.Bhid] = value;
					}
				}
			}
		}

		private void UpdateOtherCityList(Dictionary<int, List<int>> list)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.OtherCityList = list;
			}
		}

		private void UpdateCityOwnerAdmire(int rid, int admirecount)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (null != this.RuntimeData.OwnerHistList)
				{
					foreach (LangHunLingYuKingHist langHunLingYuKingHist in this.RuntimeData.OwnerHistList)
					{
						if (langHunLingYuKingHist.rid == rid)
						{
							langHunLingYuKingHist.AdmireCount = admirecount;
						}
					}
				}
			}
		}

		private void UpdateCityOwnerHist(List<LangHunLingYuKingHist> list)
		{
			lock (this.RuntimeData.Mutex)
			{
				this.RuntimeData.OwnerHistList = list;
				this.ReplaceLangHunLingYuNpc();
			}
		}

		private void BroadcastBangHuiCityData(HashSet<long> newBangHuiIdHashSet, long oldBhid, long newBhid)
		{
			int maxClientCount = GameManager.ClientMgr.GetMaxClientCount();
			for (int i = 0; i < maxClientCount; i++)
			{
				GameClient gameClient = GameManager.ClientMgr.FindClientByNid(i);
				if (null != gameClient)
				{
					if (gameClient.ClientData.Faction > 0 && newBangHuiIdHashSet.Contains((long)gameClient.ClientData.Faction))
					{
						this.ProcessLangHunLingYuRoleDataCmd(gameClient, 1154, null, null);
					}
					if (oldBhid != newBhid)
					{
						this.UpdateChengHaoBuffer(gameClient, oldBhid, newBhid);
					}
				}
			}
		}

		private void OnInitGame(GameClient client)
		{
			this.UpdateChengHaoBuffer(client, 0L, this.RuntimeData.ChengHaoBHid);
		}

		private void UpdateChengHaoBuffer(GameClient client, long oldBhid, long newBhid)
		{
			if (newBhid > 0L && (long)client.ClientData.Faction == newBhid)
			{
				double[] actionParams = new double[]
				{
					1.0
				};
				if (client.ClientData.BHZhiWu == 1)
				{
					Global.UpdateBufferData(client, BufferItemTypes.ShengYuChengZhu_Title, actionParams, 1, true);
				}
				else
				{
					Global.UpdateBufferData(client, BufferItemTypes.LangHunLingYu_ChengHao, actionParams, 1, true);
				}
			}
			else
			{
				double[] array = new double[1];
				double[] actionParams = array;
				Global.UpdateBufferData(client, BufferItemTypes.ShengYuChengZhu_Title, actionParams, 1, true);
				Global.UpdateBufferData(client, BufferItemTypes.LangHunLingYu_ChengHao, actionParams, 1, true);
			}
		}

		public void OnLogin(GameClient client)
		{
			this.UpdateChengHaoBuffer(client, 0L, this.RuntimeData.ChengHaoBHid);
		}

		public bool CanGetAwardsByEnterTime(GameClient client)
		{
			int num = DataHelper.UnixSecondsNow() - Global.GetRoleParamsInt32FromDB(client, "EnterBangHuiUnixSecs");
			return (long)num >= GameManager.systemParamsList.GetParamValueIntByName("JiaRuTime", 0) * 60L * 60L;
		}

		public void CheckTipsIconState(GameClient client)
		{
			int num = 0;
			bool flag = false;
			bool bIconState = false;
			lock (this.RuntimeData.Mutex)
			{
				flag = this.CanGetAwardsByEnterTime(client);
				if (flag)
				{
					LangHunLingYuBangHuiData langHunLingYuBangHuiData;
					if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)client.ClientData.Faction, out langHunLingYuBangHuiData))
					{
						num = langHunLingYuBangHuiData.DayAwardFlags;
					}
					int offsetDayNow = Global.GetOffsetDayNow();
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
					int num2 = 0;
					if (roleParamsInt32FromDB == offsetDayNow)
					{
						num2 = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
					}
					int num3 = (num ^ num2) & num;
					if (num3 == 0)
					{
						flag = false;
					}
				}
				LangHunLingYuBangHuiData langHunLingYuBangHuiData2;
				if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)client.ClientData.Faction, out langHunLingYuBangHuiData2))
				{
					if (langHunLingYuBangHuiData2.SelfCityList.Count > 0)
					{
						bIconState = true;
					}
				}
			}
			client._IconStateMgr.AddFlushIconState(15002, flag);
			client._IconStateMgr.AddFlushIconState(15003, bIconState);
			client._IconStateMgr.SendIconStateToClient(client);
		}

		public bool ProcessLangHunLingYuJoinCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 0;
				int faction = client.ClientData.Faction;
				if (faction <= 0)
				{
					num = -1000;
				}
				else if (client.ClientData.BHZhiWu != 1)
				{
					num = -1002;
				}
				else if (!this.IsGongNengOpened(client, true))
				{
					num = -13;
				}
				else
				{
					BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(faction, 0);
					if (null == bangHuiMiniData)
					{
						num = -1001;
					}
					else
					{
						int num2 = 0;
						CityLevelInfo cityLevelInfo = null;
						LangHunLingYuGameStates langHunLingYuGameStates = LangHunLingYuGameStates.None;
						int num3 = this.GetBangHuiCityLevel(faction) + 1;
						if (num3 > 10)
						{
							num = -4004;
						}
						else
						{
							num = this.CheckSignUpCondition(num3, ref cityLevelInfo, ref langHunLingYuGameStates);
							if (langHunLingYuGameStates != LangHunLingYuGameStates.SignUp)
							{
								num = -2001;
							}
							else
							{
								LangHunLingYuBangHuiData langHunLingYuBangHuiData = null;
								lock (this.RuntimeData.Mutex)
								{
									if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)faction, out langHunLingYuBangHuiData))
									{
										if (null != langHunLingYuBangHuiData.SelfCityList)
										{
											foreach (int key in langHunLingYuBangHuiData.SelfCityList)
											{
												LangHunLingYuCityDataEx langHunLingYuCityDataEx;
												if (this.RuntimeData.CityDataExDict.TryGetValue(key, out langHunLingYuCityDataEx) && langHunLingYuCityDataEx.CityLevel >= num3)
												{
													num = -1004;
													break;
												}
											}
										}
										if (langHunLingYuBangHuiData.SignUpTime > TimeUtil.NOW() - 10000L)
										{
											num = -2005;
											goto IL_2E3;
										}
									}
									else
									{
										langHunLingYuBangHuiData = new LangHunLingYuBangHuiData();
										this.RuntimeData.BangHuiDataDict.Add((long)faction, langHunLingYuBangHuiData);
									}
									if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, cityLevelInfo.ZhanMengZiJin, out num2))
									{
										num = -9;
									}
									else
									{
										langHunLingYuBangHuiData.SignUpTime = TimeUtil.NOW();
										num = YongZheZhanChangClient.getInstance().LangHunLingYuSignUp(bangHuiMiniData.BHName, bangHuiMiniData.BHID, bangHuiMiniData.ZoneID, 10, 1, 0);
										if (num >= 0)
										{
											EventLogManager.AddGameEvent(LogRecordType.LangHunLingYu, new object[]
											{
												32,
												bangHuiMiniData.BHID,
												cityLevelInfo.ZhanMengZiJin
											});
										}
										else
										{
											GameManager.ClientMgr.AddBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, faction, cityLevelInfo.ZhanMengZiJin);
										}
									}
								}
							}
						}
					}
				}
				IL_2E3:
				client.sendCmd<int>(nID, num, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessLangHunLingYuEnterCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				CityLevelInfo cityLevelInfo = null;
				LangHunLingYuGameStates langHunLingYuGameStates = LangHunLingYuGameStates.None;
				int num = 0;
				int roleID = client.ClientData.RoleID;
				int num2 = Global.SafeConvertToInt32(cmdParams[1]);
				int faction = client.ClientData.Faction;
				bool flag = VideoLogic.getInstance().IsGuanZhanGM(client);
				if (num2 < 1 || num2 > 1023)
				{
					num = -18;
				}
				else
				{
					int unionLevel = Global.GetUnionLevel(client, false);
					if (unionLevel < Global.GetUnionLevel(this.RuntimeData.MinZhuanSheng, this.RuntimeData.MinLevel, false))
					{
						num = -19;
					}
					else if (!this.IsGongNengOpened(client, true))
					{
						num = -12;
					}
					else if (faction <= 0 && !flag)
					{
						num = -1000;
					}
					else
					{
						bool flag2 = false;
						bool flag3 = true;
						LangHunLingYuCityDataEx langHunLingYuCityDataEx;
						lock (this.RuntimeData.Mutex)
						{
							if (this.RuntimeData.CityDataExDict.TryGetValue(num2, out langHunLingYuCityDataEx))
							{
								if (flag)
								{
									flag2 = true;
									flag3 = false;
								}
								else
								{
									for (int i = 0; i < langHunLingYuCityDataEx.Site.Length; i++)
									{
										long num3 = langHunLingYuCityDataEx.Site[i];
										if (num3 == (long)faction)
										{
											flag2 = true;
										}
										if (i > 0 && num3 > 0L)
										{
											flag3 = false;
										}
									}
								}
							}
						}
						if (!flag2)
						{
							num = -1003;
						}
						else if (flag3)
						{
							num = -4006;
						}
						else if (!this.CheckMap(client))
						{
							num = -21;
						}
						else
						{
							num = this.CheckFightCondition(this.GetCityLevelById(num2), ref cityLevelInfo, ref langHunLingYuGameStates);
							if (num >= 0 && langHunLingYuGameStates == LangHunLingYuGameStates.Start)
							{
								lock (this.RuntimeData.Mutex)
								{
									KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
									if (null != clientKuaFuServerLoginData)
									{
										if (YongZheZhanChangClient.getInstance().LangHunLingYuKuaFuLoginData(roleID, num2, langHunLingYuCityDataEx.GameId, clientKuaFuServerLoginData))
										{
											num = 0;
										}
										else
										{
											num = -11000;
										}
									}
								}
								if (num >= 0)
								{
									if (num >= 0)
									{
										GlobalNew.RecordSwitchKuaFuServerLog(client);
										client.sendCmd<KuaFuServerLoginData>(14000, Global.GetClientKuaFuServerLoginData(client), false);
										EventLogManager.AddRoleEvent(client, OpTypes.Enter, OpTags.LangHunLingYu, LogRecordType.IntValue, new object[]
										{
											num2
										});
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
						}
					}
				}
				client.sendCmd<int>(nID, num, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessLangHunLingYuRoleDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = client.ClientData.RoleID;
				int faction = client.ClientData.Faction;
				LangHunLingYuRoleData langHunLingYuRoleData = new LangHunLingYuRoleData();
				if (faction > 0)
				{
					LangHunLingYuBangHuiData langHunLingYuBangHuiData = null;
					lock (this.RuntimeData.Mutex)
					{
						if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)faction, out langHunLingYuBangHuiData))
						{
							LangHunLingYuCityData langHunLingYuCityData;
							if (this.RuntimeData.CityDataDict.TryGetValue(1, out langHunLingYuCityData))
							{
								langHunLingYuRoleData.SelfCityList.Insert(0, langHunLingYuCityData);
							}
							goto IL_30B;
						}
						int num = 0;
						bool flag2 = false;
						langHunLingYuRoleData.SignUpState = langHunLingYuBangHuiData.SignUpState;
						foreach (int num2 in langHunLingYuBangHuiData.SelfCityList)
						{
							LangHunLingYuCityData langHunLingYuCityData;
							if (num2 > 0 && this.RuntimeData.CityDataDict.TryGetValue(num2, out langHunLingYuCityData))
							{
								if (num2 == 1)
								{
									flag2 = true;
								}
								langHunLingYuRoleData.SelfCityList.Add(langHunLingYuCityData);
								if (langHunLingYuCityData.Owner != null && langHunLingYuCityData.Owner.BHID == faction && langHunLingYuCityData.CityLevel > num)
								{
									num = langHunLingYuCityData.CityLevel;
								}
							}
						}
						if (!flag2)
						{
							LangHunLingYuCityData langHunLingYuCityData;
							if (this.RuntimeData.CityDataDict.TryGetValue(1, out langHunLingYuCityData))
							{
								langHunLingYuRoleData.SelfCityList.Insert(0, langHunLingYuCityData);
							}
						}
					}
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
					int resource = 0;
					int offsetDayNow = Global.GetOffsetDayNow();
					if (roleParamsInt32FromDB == offsetDayNow)
					{
						resource = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
					}
					List<int> list = new List<int>();
					lock (this.RuntimeData.Mutex)
					{
						if (null != langHunLingYuBangHuiData.SelfCityList)
						{
							foreach (int num2 in langHunLingYuBangHuiData.SelfCityList)
							{
								LangHunLingYuCityDataEx langHunLingYuCityDataEx;
								if (this.RuntimeData.CityDataExDict.TryGetValue(num2, out langHunLingYuCityDataEx))
								{
									if (langHunLingYuCityDataEx.Site[0] == (long)faction)
									{
										if (0 == Global.GetIntSomeBit(resource, langHunLingYuCityDataEx.CityLevel))
										{
											list.Add(langHunLingYuCityDataEx.CityLevel);
										}
									}
								}
							}
						}
					}
					langHunLingYuRoleData.GetDayAwardsState = new List<int>(list);
				}
				IL_30B:
				client.sendCmd<LangHunLingYuRoleData>(nID, langHunLingYuRoleData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessLangHunLingYuCityDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = client.ClientData.RoleID;
				int faction = client.ClientData.Faction;
				int num = Global.SafeConvertToInt32(cmdParams[1]);
				LangHunLingYuCityData cmdData = null;
				if ((faction > 0 && num >= 1) || num <= 1023)
				{
					lock (this.RuntimeData.Mutex)
					{
						if (this.RuntimeData.CityDataDict.TryGetValue(faction, out cmdData))
						{
						}
					}
				}
				client.sendCmd<LangHunLingYuCityData>(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetAdmireDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				client.sendCmd<LangHunLingYuKingShowData>(nID, new LangHunLingYuKingShowData
				{
					AdmireCount = Global.GetLHLYAdmireCount(client),
					RoleData4Selector = Global.RoleDataEx2RoleData4Selector(this.OwnerRoleData)
				}, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetAdmireHistoryCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				List<LangHunLingYuKingShowDataHist> list = new List<LangHunLingYuKingShowDataHist>();
				lock (this.RuntimeData.Mutex)
				{
					if (this.RuntimeData.OwnerHistList != null && this.RuntimeData.OwnerHistList.Count > 1)
					{
						for (int i = this.RuntimeData.OwnerHistList.Count - 1; i >= 0; i--)
						{
							LangHunLingYuKingHist langHunLingYuKingHist = this.RuntimeData.OwnerHistList[i];
							if (null != langHunLingYuKingHist.CityOwnerRoleData)
							{
								RoleDataEx roleDataEx = DataHelper.BytesToObject<RoleDataEx>(langHunLingYuKingHist.CityOwnerRoleData, 0, langHunLingYuKingHist.CityOwnerRoleData.Length);
								if (null != roleDataEx)
								{
									if (i != this.RuntimeData.OwnerHistList.Count - 1 || this.RuntimeData.ChengHaoBHid == 0L)
									{
										list.Add(new LangHunLingYuKingShowDataHist
										{
											AdmireCount = langHunLingYuKingHist.AdmireCount,
											CompleteTime = langHunLingYuKingHist.CompleteTime,
											RoleData4Selector = Global.RoleDataEx2RoleData4Selector(roleDataEx),
											BHName = roleDataEx.BHName
										});
										if (list.Count == 9)
										{
											break;
										}
									}
								}
							}
						}
					}
					client.sendCmd<List<LangHunLingYuKingShowDataHist>>(nID, list, false);
				}
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessAdmireCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Convert.ToInt32(cmdParams[0]);
				int num2 = Convert.ToInt32(cmdParams[1]);
				MoBaiData moBaiData = null;
				string cmdData;
				if (!Data.MoBaiDataInfoList.TryGetValue(1, out moBaiData))
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
				int lhlyadmireCount = Global.GetLHLYAdmireCount(client);
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
				if (lhlyadmireCount >= num3)
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
					if (!Global.SubBindTongQianAndTongQian(client, moBaiData.NeedJinBi, "膜拜圣域城主"))
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
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.JinBiZhanGongAward, AddBangGongTypes.LHLYMoBai, 0);
					}
					if (moBaiData.LingJingAwardByJinBi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByJinBi, "膜拜圣域城主", true, true, false);
					}
				}
				else if (num2 == 2)
				{
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, moBaiData.NeedZuanShi, "膜拜圣域城主", true, true, false, DaiBiSySType.None))
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
						GameManager.ClientMgr.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref moBaiData.ZuanShiZhanGongAward, AddBangGongTypes.LHLYMoBai, 0);
					}
					if (moBaiData.LingJingAwardByZuanShi > 0)
					{
						GameManager.ClientMgr.ModifyMUMoHeValue(client, moBaiData.LingJingAwardByZuanShi, "膜拜圣域城主", true, true, false);
					}
				}
				if (null != this.OwnerRoleData)
				{
					YongZheZhanChangClient.getInstance().LangHunLingYunAdmire(this.OwnerRoleData.RoleID);
				}
				Global.ProcessIncreaseLHLYAdmireCount(client);
				cmdData = string.Format("{0}", 1);
				client.sendCmd(nID, cmdData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessLangHunLingYuWorldDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int roleID = client.ClientData.RoleID;
				int faction = client.ClientData.Faction;
				LangHunLingYuWorldData langHunLingYuWorldData = new LangHunLingYuWorldData();
				if (faction > 0 || VideoLogic.getInstance().IsGuanZhanGM(client))
				{
					lock (this.RuntimeData.Mutex)
					{
						for (int i = 1; i <= 31; i++)
						{
							LangHunLingYuCityData item;
							if (this.RuntimeData.CityDataDict.TryGetValue(i, out item))
							{
								langHunLingYuWorldData.CityList.Add(item);
							}
						}
					}
				}
				client.sendCmd<LangHunLingYuWorldData>(nID, langHunLingYuWorldData, false);
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public bool ProcessGetDailyAwardsCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = 1;
				int num2 = Convert.ToInt32(cmdParams[0]);
				int faction = client.ClientData.Faction;
				if (faction <= 0 || client.ClientData.Faction != faction)
				{
					num = -12;
				}
				else if (!this.CanGetAwardsByEnterTime(client))
				{
					num = -2006;
				}
				else
				{
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwards");
					int num3 = 0;
					int offsetDayNow = Global.GetOffsetDayNow();
					if (roleParamsInt32FromDB == offsetDayNow)
					{
						num3 = Global.GetRoleParamsInt32FromDB(client, "LangHunLingYuDayAwardsFlags");
					}
					List<int> list = new List<int>();
					lock (this.RuntimeData.Mutex)
					{
						LangHunLingYuBangHuiData langHunLingYuBangHuiData;
						if (!this.RuntimeData.BangHuiDataDict.TryGetValue((long)faction, out langHunLingYuBangHuiData))
						{
							num = -20;
							goto IL_3E3;
						}
						if (null != langHunLingYuBangHuiData.SelfCityList)
						{
							foreach (int key in langHunLingYuBangHuiData.SelfCityList)
							{
								LangHunLingYuCityDataEx langHunLingYuCityDataEx;
								if (this.RuntimeData.CityDataExDict.TryGetValue(key, out langHunLingYuCityDataEx) && langHunLingYuCityDataEx.Site[0] == (long)faction)
								{
									if (0 == Global.GetIntSomeBit(num3, langHunLingYuCityDataEx.CityLevel))
									{
										list.Add(langHunLingYuCityDataEx.CityLevel);
									}
								}
							}
						}
					}
					bool flag2 = false;
					foreach (int num4 in list)
					{
						CityLevelInfo cityLevelInfo;
						if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(num4, out cityLevelInfo))
						{
							LogManager.WriteLog(2, "城池等级每日奖励未配置：Level=" + num4, null, true);
						}
						else
						{
							List<GoodsData> list2 = Global.ConvertToGoodsDataList(cityLevelInfo.DayAward.Items, -1);
							if (Global.CanAddGoodsDataList(client, list2))
							{
								for (int i = 0; i < list2.Count; i++)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, list2[i].GoodsID, list2[i].GCount, list2[i].Quality, "", list2[i].Forge_level, list2[i].Binding, 0, "", true, 1, "圣域争霸胜利战盟每日奖励", "1900-01-01 12:00:00", 0, list2[i].BornIndex, list2[i].Lucky, 0, list2[i].ExcellenceInfo, list2[i].AppendPropLev, 0, null, null, 0, true);
									GoodsData goodsData = list2[i];
									GameManager.logDBCmdMgr.AddDBLogInfo(goodsData.Id, Global.ModifyGoodsLogName(goodsData), "圣域争霸胜利战盟每日奖励", Global.GetMapName(client.ClientData.MapCode), client.ClientData.RoleName, "增加", goodsData.GCount, client.ClientData.ZoneID, client.strUserID, -1, client.ServerId, null);
								}
								num3 = Global.SetIntSomeBit(num4, num3, true);
								flag2 = true;
								Global.SaveRoleParamsInt32ValueToDB(client, "LangHunLingYuDayAwards", Global.GetOffsetDayNow(), true);
								Global.SaveRoleParamsInt32ValueToDB(client, "LangHunLingYuDayAwardsFlags", num3, true);
								EventLogManager.AddRoleEvent(client, OpTypes.GiveAwards, OpTags.LangHunLingYuDailyAwards, LogRecordType.IntValue, new object[]
								{
									num4
								});
							}
							else
							{
								num = -100;
							}
						}
					}
					if (flag2)
					{
						this.CheckTipsIconState(client);
					}
				}
				IL_3E3:
				client.sendCmd(nID, string.Format("{0}", num), false);
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

		private int CheckSignUpCondition(int cityLevel, ref CityLevelInfo sceneItem, ref LangHunLingYuGameStates state)
		{
			int result = 0;
			cityLevel = Math.Max(cityLevel, 1);
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out sceneItem))
				{
					return -12;
				}
			}
			result = -2001;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < sceneItem.BaoMingTime.Count - 1; i += 2)
				{
					TimeSpan t = dateTime.TimeOfDay.Add(TimeSpan.FromDays((double)dateTime.DayOfWeek));
					if (t >= sceneItem.BaoMingTime[i] && t <= sceneItem.BaoMingTime[i + 1])
					{
						state = LangHunLingYuGameStates.SignUp;
						result = 1;
						break;
					}
				}
			}
			return result;
		}

		private int CheckFightCondition(int cityLevel, ref CityLevelInfo sceneItem, ref LangHunLingYuGameStates state)
		{
			int result = 0;
			cityLevel = Math.Max(cityLevel, 1);
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out sceneItem))
				{
					return -12;
				}
			}
			result = -2001;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (sceneItem.AttackWeekDay.Contains((int)dateTime.DayOfWeek))
				{
					for (int i = 0; i < sceneItem.AttackTime.Count - 1; i += 2)
					{
						if (dateTime.TimeOfDay >= sceneItem.AttackTime[i] && dateTime.TimeOfDay <= sceneItem.AttackTime[i + 1])
						{
							state = LangHunLingYuGameStates.Start;
							result = 0;
							break;
						}
					}
				}
			}
			return result;
		}

		private TimeSpan GetStartTime(int cityLevel)
		{
			CityLevelInfo cityLevelInfo = null;
			TimeSpan timeSpan = TimeSpan.MinValue;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevel, out cityLevelInfo))
				{
					goto IL_108;
				}
			}
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < cityLevelInfo.AttackTime.Count - 1; i += 2)
				{
					if (dateTime.TimeOfDay >= cityLevelInfo.AttackTime[i] && dateTime.TimeOfDay <= cityLevelInfo.AttackTime[i + 1])
					{
						timeSpan = cityLevelInfo.AttackTime[i];
						break;
					}
				}
			}
			IL_108:
			if (timeSpan < TimeSpan.Zero)
			{
				timeSpan = dateTime.TimeOfDay;
			}
			return timeSpan;
		}

		private string GetBHName(int bangHuiID)
		{
			BangHuiMiniData bangHuiMiniData = Global.GetBangHuiMiniData(bangHuiID, 0);
			string result;
			if (null != bangHuiMiniData)
			{
				result = bangHuiMiniData.BHName;
			}
			else
			{
				result = GLang.GetLang(6, new object[0]);
			}
			return result;
		}

		private void ProcessTimeAddRoleExp(LangHunLingYuScene scene)
		{
			long num = TimeUtil.NOW();
			if (num - scene.LastAddBangZhanAwardsTicks >= 10000L)
			{
				scene.LastAddBangZhanAwardsTicks = num;
				this.NotifyQiZhiBuffOwnerDataList(scene);
				this.NotifyLongTaRoleDataList(scene);
				this.NotifyLongTaOwnerData(scene);
				foreach (CopyMap copyMap in scene.CopyMapDict.Values)
				{
					List<GameClient> clientsList = copyMap.GetClientsList();
					foreach (GameClient gameClient in clientsList)
					{
						if (null != gameClient)
						{
							this._LevelAwardsMgr.ProcessBangZhanAwards(gameClient);
						}
					}
				}
			}
		}

		public bool GetZhanMengBirthPoint(LangHunLingYuSceneInfo sceneInfo, GameClient client, int toMapCode, out int mapCode, out int posX, out int posY)
		{
			mapCode = sceneInfo.MapCode;
			posX = -1;
			posY = -1;
			bool result;
			if (client.ClientData.GuanZhanGM > 0)
			{
				if (VideoLogic.getInstance().GetGuanZhanPos(toMapCode, ref posX, ref posY))
				{
					mapCode = toMapCode;
				}
				result = true;
			}
			else
			{
				int faction = client.ClientData.Faction;
				lock (this.RuntimeData.Mutex)
				{
					int key = client.ClientData.BattleWhichSide - 1;
					int num = 0;
					if (sceneInfo.MapCode_LongTa == toMapCode)
					{
						Point randomPoint;
						for (;;)
						{
							randomPoint = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa);
							if (!Global.InObs(ObjectTypes.OT_CLIENT, sceneInfo.MapCode_LongTa, (int)randomPoint.X, (int)randomPoint.Y, 0, 0))
							{
								break;
							}
							if (num++ >= 1000)
							{
								goto IL_105;
							}
						}
						mapCode = sceneInfo.MapCode_LongTa;
						posX = (int)randomPoint.X;
						posY = (int)randomPoint.Y;
						return true;
					}
					IL_105:
					num = 0;
					LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
					if (langHunLingYuScene != null && client.ClientData.Faction == langHunLingYuScene.SuperQiZhiOwnerBhid && toMapCode == sceneInfo.MapCode)
					{
						for (;;)
						{
							mapCode = toMapCode;
							posX = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosX - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosX + 400);
							posY = Global.GetRandomNumber(this.RuntimeData.SuperQiZhiOwnerBirthPosY - 400, this.RuntimeData.SuperQiZhiOwnerBirthPosY + 400);
							if (!Global.InObs(ObjectTypes.OT_CLIENT, toMapCode, posX, posY, 0, 0))
							{
								break;
							}
							if (num++ >= 100)
							{
								goto IL_1D1;
							}
						}
						return true;
					}
					IL_1D1:
					List<MapBirthPoint> list;
					if (!this.RuntimeData.MapBirthPointListDict.TryGetValue(key, out list) || list.Count == 0)
					{
						return true;
					}
					num = 0;
					for (;;)
					{
						int randomNumber = Global.GetRandomNumber(0, list.Count);
						MapBirthPoint mapBirthPoint = list[randomNumber];
						posX = mapBirthPoint.BirthPosX + Global.GetRandomNumber(-mapBirthPoint.BirthRangeX, mapBirthPoint.BirthRangeX);
						posY = mapBirthPoint.BirthPosY + Global.GetRandomNumber(-mapBirthPoint.BirthRangeY, mapBirthPoint.BirthRangeY);
						if (!Global.InObs(ObjectTypes.OT_CLIENT, mapCode, posX, posY, 0, 0))
						{
							break;
						}
						if (num++ >= 1000)
						{
							goto Block_12;
						}
					}
					return true;
					Block_12:;
				}
				result = true;
			}
			return result;
		}

		public bool ClientRelive(GameClient client)
		{
			int mapCode = client.ClientData.MapCode;
			LangHunLingYuSceneInfo langHunLingYuSceneInfo = client.SceneInfoObject as LangHunLingYuSceneInfo;
			if (null != langHunLingYuSceneInfo)
			{
				int mapCode2;
				int num;
				int num2;
				if (this.GetZhanMengBirthPoint(langHunLingYuSceneInfo, client, mapCode2 = langHunLingYuSceneInfo.MapCode, out mapCode2, out num, out num2))
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

		public bool ClientChangeMap(GameClient client, ref int toNewMapCode, ref int toNewPosX, ref int toNewPosY)
		{
			LangHunLingYuSceneInfo langHunLingYuSceneInfo = client.SceneInfoObject as LangHunLingYuSceneInfo;
			if (null != langHunLingYuSceneInfo)
			{
				if (toNewMapCode == langHunLingYuSceneInfo.MapCode || toNewMapCode == langHunLingYuSceneInfo.MapCode_LongTa)
				{
					if (client.ClientData.MapCode != langHunLingYuSceneInfo.MapCode_LongTa)
					{
						int num;
						int num2;
						int num3;
						if (this.GetZhanMengBirthPoint(langHunLingYuSceneInfo, client, toNewMapCode, out num, out num2, out num3))
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

		public bool OnSpriteClickOnNpc(GameClient client, int npcID, int npcExtentionID)
		{
			bool result = false;
			LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
			CopyMap copyMap;
			if (langHunLingYuScene != null && langHunLingYuScene.CopyMapDict.TryGetValue(client.ClientData.MapCode, out copyMap))
			{
				lock (this.RuntimeData.Mutex)
				{
					QiZhiConfig qiZhiConfig;
					if (langHunLingYuScene.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out qiZhiConfig))
					{
						result = true;
						if (qiZhiConfig.Alive)
						{
							return result;
						}
						if (qiZhiConfig.KillerBhid <= 0L || (long)client.ClientData.Faction == qiZhiConfig.KillerBhid || Math.Abs(TimeUtil.NOW() - qiZhiConfig.DeadTicks) >= 10000L)
						{
							if (client.ClientData.Faction <= 0)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(45, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							}
							else if (Math.Abs(client.ClientData.PosX - qiZhiConfig.PosX) <= 1000 && Math.Abs(client.ClientData.PosY - qiZhiConfig.PosY) <= 1000)
							{
								qiZhiConfig.Alive = true;
								int zoneId;
								qiZhiConfig.InstallBhName = this.GetBangHuiName(client.ClientData.Faction, out zoneId);
								qiZhiConfig.BattleWhichSide = client.ClientData.BattleWhichSide;
								this.CreateMonster(copyMap, qiZhiConfig, qiZhiConfig.MonsterId);
								this.UpdateQiZhiBangHui(langHunLingYuScene, npcExtentionID, client.ClientData.Faction, client.ClientData.BHName, zoneId);
								Global.BroadcastBangHuiMsg(-1, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(48, new object[0]), new object[]
								{
									Global.FormatRoleName(client, client.ClientData.RoleName),
									Global.GetServerLineName2(),
									Global.GetMapName(client.ClientData.MapCode)
								}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlySysHint);
							}
						}
					}
				}
			}
			return result;
		}

		public void OnProcessJunQiDead(GameClient client, Monster monster)
		{
			if (client != null && this.RuntimeData.JunQiMonsterHashSet.Contains(monster.MonsterInfo.ExtensionID))
			{
				int num;
				string bangHuiName = this.GetBangHuiName(client.ClientData.Faction, out num);
				LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
				QiZhiConfig qiZhiConfig = monster.Tag as QiZhiConfig;
				if (langHunLingYuScene != null && null != qiZhiConfig)
				{
					lock (this.RuntimeData.Mutex)
					{
						qiZhiConfig.KillerBhid = (long)client.ClientData.Faction;
						qiZhiConfig.InstallBhName = "";
						qiZhiConfig.InstallBhid = 0L;
						qiZhiConfig.DeadTicks = TimeUtil.NOW();
						qiZhiConfig.Alive = false;
						this.UpdateQiZhiBangHui(langHunLingYuScene, qiZhiConfig.NPCID, 0, bangHuiName, 0);
					}
				}
			}
		}

		private void ResetQiZhiBuff(GameClient client)
		{
			if (!VideoLogic.getInstance().IsGuanZhanGM(client))
			{
				LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
				int mapCode = client.ClientData.MapCode;
				List<int> list = new List<int>();
				lock (this.RuntimeData.Mutex)
				{
					EquipPropItem equipPropItem = null;
					int num = 0;
					if (langHunLingYuScene != null && client.SceneType == 35)
					{
						for (int i = 0; i < langHunLingYuScene.QiZhiBuffOwnerDataList.Count; i++)
						{
							bool add = false;
							LangHunLingYuQiZhiBuffOwnerData langHunLingYuQiZhiBuffOwnerData = langHunLingYuScene.QiZhiBuffOwnerDataList[i];
							QiZhiConfig qiZhiConfig;
							if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(langHunLingYuQiZhiBuffOwnerData.NPCID, out qiZhiConfig))
							{
								num = qiZhiConfig.BufferID;
								equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(num);
								if (null != equipPropItem)
								{
									if (langHunLingYuQiZhiBuffOwnerData.OwnerBHID == client.ClientData.Faction)
									{
										add = true;
									}
								}
							}
							this.UpdateQiZhiBuff4GameClient(client, equipPropItem, num, add);
						}
					}
					else
					{
						foreach (QiZhiConfig qiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
						{
							this.UpdateQiZhiBuff4GameClient(client, equipPropItem, qiZhiConfig.BufferID, false);
						}
					}
				}
			}
		}

		public void OnStartPlayGame(GameClient client)
		{
			this.ResetQiZhiBuff(client);
			this.BroadcastLuoLanChengZhuLoginHint(client);
			LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
			if (null != langHunLingYuScene)
			{
				this.NotifyTimeStateInfoAndScoreInfo(client, true, true);
			}
		}

		public void BroadcastLuoLanChengZhuLoginHint(GameClient client)
		{
		}

		private void CreateMonster(CopyMap copyMap, QiZhiConfig qiZhiConfig, int monsterId)
		{
			GameManager.MonsterZoneMgr.AddDynamicMonsters(copyMap.MapCode, monsterId, copyMap.CopyMapID, 1, qiZhiConfig.PosX / this.RuntimeData.MapGridWidth, qiZhiConfig.PosY / this.RuntimeData.MapGridHeight, 0, 0, 35, qiZhiConfig, null);
		}

		private bool RefuseChangeBangHui(int bhid)
		{
			CityLevelInfo cityLevelInfo = null;
			LangHunLingYuGameStates langHunLingYuGameStates = LangHunLingYuGameStates.None;
			this.CheckFightCondition(1, ref cityLevelInfo, ref langHunLingYuGameStates);
			if (langHunLingYuGameStates == LangHunLingYuGameStates.Start)
			{
				lock (this.RuntimeData.Mutex)
				{
					LangHunLingYuBangHuiData langHunLingYuBangHuiData;
					if (this.RuntimeData.BangHuiDataDict.TryGetValue((long)bhid, out langHunLingYuBangHuiData))
					{
						if (langHunLingYuBangHuiData.SelfCityList.Count > 0)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		public bool OnPreBangHuiAddMember(PreBangHuiAddMemberEventObject e)
		{
			bool result;
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(402, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
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
			if (this.RefuseChangeBangHui(e.BHID))
			{
				e.Result = false;
				GameManager.ClientMgr.NotifyImportantMsg(e.Player, GLang.GetLang(403, new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private RoleDataEx OwnerRoleData
		{
			get
			{
				RoleDataEx ownerRoleData;
				lock (this.OwnerRoleDataMutex)
				{
					ownerRoleData = this._OwnerRoleData;
				}
				return ownerRoleData;
			}
			set
			{
				lock (this.OwnerRoleDataMutex)
				{
					this._OwnerRoleData = value;
				}
			}
		}

		public void ReplaceLangHunLingYuNpc()
		{
			if (this.RuntimeData.OwnerHistList == null || this.RuntimeData.OwnerHistList.Count == 0 || this.RuntimeData.ChengHaoBHid == 0L)
			{
				this.RestoreLangHunLingYuNpc();
			}
			else
			{
				LangHunLingYuKingHist langHunLingYuKingHist = this.RuntimeData.OwnerHistList[this.RuntimeData.OwnerHistList.Count - 1];
				RoleDataEx roleDataEx = DataHelper.BytesToObject<RoleDataEx>(langHunLingYuKingHist.CityOwnerRoleData, 0, langHunLingYuKingHist.CityOwnerRoleData.Length);
				if (roleDataEx == null || roleDataEx.RoleID <= 0)
				{
					this.RestoreLangHunLingYuNpc();
				}
				else
				{
					this.OwnerRoleData = roleDataEx;
					NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 134);
					if (null != npc)
					{
						npc.ShowNpc = false;
						GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
						FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang3, false);
						FakeRoleManager.ProcessNewFakeRole(new SafeClientData
						{
							RoleData = roleDataEx
						}, npc.MapCode, FakeRoleTypes.DiaoXiang3, 4, (int)npc.CurrentPos.X, (int)npc.CurrentPos.Y, 134);
					}
				}
			}
		}

		public void RestoreLangHunLingYuNpc()
		{
			this.OwnerRoleData = null;
			NPC npc = NPCGeneralManager.FindNPC(GameManager.MainMapCode, 134);
			if (null != npc)
			{
				npc.ShowNpc = true;
				GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				FakeRoleManager.ProcessDelFakeRoleByType(FakeRoleTypes.DiaoXiang3, false);
			}
		}

		public void LangHunLingYuBuildMaxCityOwnerInfo(LangHunLingYuStatisticalData statisticalData, int ServerID)
		{
			if (statisticalData.CityId == 1)
			{
				BangHuiDetailData bangHuiDetailDataAuto = this.GetBangHuiDetailDataAuto(statisticalData.SiteBhids[0], -1, ServerID);
				if (null != bangHuiDetailDataAuto)
				{
					statisticalData.rid = bangHuiDetailDataAuto.BZRoleID;
					RoleDataEx roleDataEx = Global.sendToDB<RoleDataEx, string>(275, string.Format("{0}:{1}", -1, statisticalData.rid), ServerID);
					if (roleDataEx != null && roleDataEx.RoleID > 0)
					{
						statisticalData.CityOwnerRoleData = DataHelper.ObjectToBytes<RoleDataEx>(roleDataEx);
					}
				}
			}
		}

		public BangHuiDetailData GetBangHuiDetailDataAuto(int bhid, int roleID = -1, int ServerID = 0)
		{
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, ServerID);
			if (null != bangHuiDetailData)
			{
				if (roleID <= 0 && bangHuiDetailData.BZRoleID > 0)
				{
					bangHuiDetailData = Global.GetBangHuiDetailData(bangHuiDetailData.BZRoleID, bhid, ServerID);
				}
			}
			return bangHuiDetailData;
		}

		public bool CanEnterKuaFuMap(KuaFuServerLoginData kuaFuServerLoginData)
		{
			int roleId = kuaFuServerLoginData.RoleId;
			LangHunLingYuFuBenData langHunLingYuFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)kuaFuServerLoginData.GameId, out langHunLingYuFuBenData))
				{
					langHunLingYuFuBenData = null;
				}
				else if (langHunLingYuFuBenData.State >= 3)
				{
					LogManager.WriteLog(2, string.Format("圣域争霸副本已结束,禁止角色{0}进入", roleId), null, true);
					return false;
				}
			}
			if (null == langHunLingYuFuBenData)
			{
				LangHunLingYuFuBenData langHunLingYuGameFuBenData = YongZheZhanChangClient.getInstance().GetLangHunLingYuGameFuBenData((int)kuaFuServerLoginData.GameId);
				if (langHunLingYuGameFuBenData == null || langHunLingYuGameFuBenData.State == 3)
				{
					LogManager.WriteLog(2, ("获取不到有效的副本数据," + langHunLingYuGameFuBenData == null) ? "fuBenData == null" : "fuBenData.State == GameFuBenState.End", null, true);
					return false;
				}
				if (langHunLingYuGameFuBenData.ServerId != GameManager.ServerId)
				{
					LogManager.WriteLog(2, string.Format("玩家请求进入的圣域争霸活动GameId={0}，不在本服务器{1}", langHunLingYuFuBenData.GameId, GameManager.ServerId), null, true);
					return false;
				}
				lock (this.RuntimeData.Mutex)
				{
					if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)kuaFuServerLoginData.GameId, out langHunLingYuFuBenData))
					{
						langHunLingYuFuBenData = langHunLingYuGameFuBenData;
						langHunLingYuFuBenData.SequenceId = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						this.RuntimeData.FuBenDataDict[langHunLingYuFuBenData.GameId] = langHunLingYuFuBenData;
					}
				}
			}
			return true;
		}

		public bool OnInitGameKuaFu(GameClient client)
		{
			int faction = client.ClientData.Faction;
			long num = (long)client.ClientData.RoleID;
			int num2 = 0;
			KuaFuServerLoginData clientKuaFuServerLoginData = Global.GetClientKuaFuServerLoginData(client);
			LangHunLingYuFuBenData langHunLingYuFuBenData;
			lock (this.RuntimeData.Mutex)
			{
				if (!this.RuntimeData.FuBenDataDict.TryGetValue((int)clientKuaFuServerLoginData.GameId, out langHunLingYuFuBenData))
				{
					LogManager.WriteLog(2, string.Format("玩家请求进入的圣域争霸活动GameId={0}，不在本服务器{1},角色{2}({3})", new object[]
					{
						langHunLingYuFuBenData.GameId,
						GameManager.ServerId,
						num,
						Global.FormatRoleName4(client)
					}), null, true);
					return false;
				}
				if (langHunLingYuFuBenData.State >= 3)
				{
					LogManager.WriteLog(2, string.Format("圣域争霸副本已结束,禁止角色{0}({1})进入", num, Global.FormatRoleName4(client)), null, true);
					return false;
				}
			}
			bool result;
			if (langHunLingYuFuBenData.CityDataEx == null)
			{
				result = false;
			}
			else
			{
				if (client.ClientData.GuanZhanGM == 0)
				{
					if (null != langHunLingYuFuBenData.CityDataEx.Site)
					{
						for (int i = 0; i < langHunLingYuFuBenData.CityDataEx.Site.Length; i++)
						{
							if (langHunLingYuFuBenData.CityDataEx.Site[i] == (long)faction)
							{
								num2 = i + 1;
							}
						}
					}
					if (num2 <= 0)
					{
						LogManager.WriteLog(2, string.Format("角色{0}({1})所在帮会({2})不在指定的圣域争霸活动GameId={3}", new object[]
						{
							num,
							Global.FormatRoleName4(client),
							faction,
							langHunLingYuFuBenData.GameId
						}), null, true);
						return false;
					}
				}
				LangHunLingYuSceneInfo langHunLingYuSceneInfo;
				lock (this.RuntimeData.Mutex)
				{
					int index = langHunLingYuFuBenData.GameId % this.RuntimeData.SceneDataList.Count;
					langHunLingYuSceneInfo = this.RuntimeData.SceneDataList[index];
					client.SceneInfoObject = langHunLingYuSceneInfo;
					client.ClientData.MapCode = langHunLingYuSceneInfo.MapCode;
				}
				client.ClientData.BattleWhichSide = num2;
				clientKuaFuServerLoginData.FuBenSeqId = langHunLingYuFuBenData.SequenceId;
				int num3;
				int posX;
				int posY;
				if (!this.GetZhanMengBirthPoint(langHunLingYuSceneInfo, client, client.ClientData.MapCode, out num3, out posX, out posY))
				{
					LogManager.WriteLog(2, string.Format("角色{0}({1})无法获取有效的阵营和出生点,进入跨服失败,side={2}", num, Global.FormatRoleName4(client), num2), null, true);
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

		public bool IsGongNengOpened(GameClient client, bool hint = false)
		{
			return client.ClientData.GuanZhanGM > 0 || (GameManager.VersionSystemOpenMgr.IsVersionSystemOpen("LangHunLingYu") && !GameFuncControlManager.IsGameFuncDisabled(8) && GlobalNew.IsGongNengOpened(client, 71, hint));
		}

		public void FillGuanZhanData(GameClient client, GuanZhanData gzData)
		{
			lock (this.RuntimeData.Mutex)
			{
				LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
				if (null != langHunLingYuScene)
				{
					gzData.SideName.Add(GLang.GetLang(5002, new object[0]));
					gzData.SideName.Add(GLang.GetLang(5003, new object[0]));
					foreach (KeyValuePair<int, LangHunLingYuClientContextData> keyValuePair in langHunLingYuScene.ClientContextDataDict)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(keyValuePair.Key);
						if (gameClient != null && gameClient.ClientData.HideGM <= 0)
						{
							SceneUIClasses mapSceneType = Global.GetMapSceneType(gameClient.ClientData.MapCode);
							if (35 == mapSceneType)
							{
								int key = (keyValuePair.Value.BattleWhichSide == 1) ? 1 : 2;
								List<GuanZhanRoleMiniData> list = null;
								if (!gzData.RoleMiniDataDict.TryGetValue(key, out list))
								{
									list = new List<GuanZhanRoleMiniData>();
									gzData.RoleMiniDataDict[key] = list;
								}
								GuanZhanRoleMiniData guanZhanRoleMiniData = new GuanZhanRoleMiniData();
								guanZhanRoleMiniData.RoleID = gameClient.ClientData.RoleID;
								guanZhanRoleMiniData.Name = Global.FormatRoleNameWithZoneId2(gameClient);
								guanZhanRoleMiniData.Level = gameClient.ClientData.Level;
								guanZhanRoleMiniData.ChangeLevel = gameClient.ClientData.ChangeLifeCount;
								guanZhanRoleMiniData.Occupation = gameClient.ClientData.Occupation;
								guanZhanRoleMiniData.RoleSex = gameClient.ClientData.RoleSex;
								guanZhanRoleMiniData.BHZhiWu = gameClient.ClientData.BHZhiWu;
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

		private void InitScene(LangHunLingYuScene scene, GameClient client)
		{
			foreach (LangHunLingYuQiZhiBuffOwnerData langHunLingYuQiZhiBuffOwnerData in this.RuntimeData.QiZhiBuffOwnerDataList)
			{
				scene.QiZhiBuffOwnerDataList.Add(new LangHunLingYuQiZhiBuffOwnerData
				{
					NPCID = langHunLingYuQiZhiBuffOwnerData.NPCID
				});
			}
			foreach (QiZhiConfig qiZhiConfig in this.RuntimeData.NPCID2QiZhiConfigDict.Values)
			{
				scene.NPCID2QiZhiConfigDict.Add(qiZhiConfig.NPCID, qiZhiConfig.Clone() as QiZhiConfig);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			bool result;
			if (sceneType == 35)
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
						LangHunLingYuScene langHunLingYuScene = null;
						if (!this.RuntimeData.SceneDict.TryGetValue(fuBenSeqID, out langHunLingYuScene))
						{
							LangHunLingYuFuBenData langHunLingYuFuBenData;
							if (!this.RuntimeData.FuBenDataDict.TryGetValue(num, out langHunLingYuFuBenData))
							{
								LogManager.WriteLog(2, "圣域争霸没有为副本找到对应的跨服副本数据,GameID:" + num, null, true);
								return false;
							}
							if (langHunLingYuFuBenData.State >= 3)
							{
								LogManager.WriteLog(2, "圣域争霸副本已经结束#GameID=" + num, null, true);
								return false;
							}
							langHunLingYuScene = new LangHunLingYuScene();
							langHunLingYuScene.CleanAllInfo();
							langHunLingYuScene.GameId = num;
							this.RuntimeData.MapGridWidth = gameMap.MapGridWidth;
							this.RuntimeData.MapGridHeight = gameMap.MapGridHeight;
							int cityLevelById = this.GetCityLevelById(langHunLingYuFuBenData.CityId);
							if (!this.RuntimeData.CityLevelInfoDict.TryGetValue(cityLevelById, out langHunLingYuScene.LevelInfo))
							{
								LogManager.WriteLog(2, "圣域争霸没有为副本找到对应的城池等级配置:CityId=" + langHunLingYuFuBenData.CityId, null, true);
							}
							langHunLingYuScene.SceneInfo = (client.SceneInfoObject as LangHunLingYuSceneInfo);
							DateTime dateTime2 = dateTime.Date.Add(this.GetStartTime(langHunLingYuScene.LevelInfo.ID));
							langHunLingYuScene.StartTimeTicks = dateTime2.Ticks / 10000L;
							langHunLingYuScene.m_lEndTime = langHunLingYuScene.StartTimeTicks + (long)((langHunLingYuScene.SceneInfo.PrepareSecs + langHunLingYuScene.SceneInfo.FightingSecs) * 1000);
							this.InitScene(langHunLingYuScene, client);
							this.RuntimeData.SceneDict[fuBenSeqID] = langHunLingYuScene;
							langHunLingYuScene.CityData.CityId = langHunLingYuFuBenData.CityDataEx.CityId;
							langHunLingYuScene.CityData.CityLevel = langHunLingYuFuBenData.CityDataEx.CityLevel;
							LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx;
							if (this.RuntimeData.BangHuiDataExDict.TryGetValue(langHunLingYuFuBenData.CityDataEx.Site[0], out langHunLingYuBangHuiDataEx))
							{
								langHunLingYuScene.LongTaOwnerData.OwnerBHid = langHunLingYuBangHuiDataEx.Bhid;
								langHunLingYuScene.LongTaOwnerData.OwnerBHName = langHunLingYuBangHuiDataEx.BhName;
								langHunLingYuScene.LongTaOwnerData.OwnerBHZoneId = langHunLingYuBangHuiDataEx.ZoneId;
							}
						}
						langHunLingYuScene.CopyMapDict[mapCode] = copyMap;
						int faction = client.ClientData.Faction;
						if (!this.RuntimeData.BangHuiMiniDataCacheDict.ContainsKey(faction))
						{
							this.RuntimeData.BangHuiMiniDataCacheDict[faction] = Global.GetBangHuiMiniData(faction, client.ServerId);
						}
						LangHunLingYuClientContextData langHunLingYuClientContextData;
						if (!langHunLingYuScene.ClientContextDataDict.TryGetValue(roleID, out langHunLingYuClientContextData))
						{
							langHunLingYuClientContextData = new LangHunLingYuClientContextData
							{
								RoleId = roleID,
								ServerId = client.ServerId,
								BattleWhichSide = client.ClientData.BattleWhichSide
							};
							langHunLingYuScene.ClientContextDataDict[roleID] = langHunLingYuClientContextData;
						}
						client.SceneObject = langHunLingYuScene;
						client.SceneGameId = (long)langHunLingYuScene.GameId;
						client.SceneContextData2 = langHunLingYuClientContextData;
						copyMap.SetRemoveTicks(langHunLingYuScene.StartTimeTicks + (long)(langHunLingYuScene.SceneInfo.TotalSecs * 1000));
						copyMap.IsKuaFuCopy = true;
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
			if (sceneType == 35)
			{
				lock (this.RuntimeData.Mutex)
				{
					LangHunLingYuScene langHunLingYuScene;
					this.RuntimeData.SceneDict.TryRemove(copyMap.FuBenSeqID, out langHunLingYuScene);
				}
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public void OnLogout(GameClient client)
		{
			YongZheZhanChangClient.getInstance().ChangeRoleState(client.ClientData.RoleID, 5, false);
		}

		public void TimerProc(object sender, EventArgs e)
		{
			lock (this.RuntimeData.Mutex)
			{
				if (this.RuntimeData.StatisticalDataQueue.Count > 0)
				{
					LangHunLingYuStatisticalData data = this.RuntimeData.StatisticalDataQueue.Peek();
					int num = YongZheZhanChangClient.getInstance().GameFuBenComplete(data);
					if (num >= 0)
					{
						this.RuntimeData.StatisticalDataQueue.Dequeue();
					}
				}
			}
			foreach (LangHunLingYuScene langHunLingYuScene in this.RuntimeData.SceneDict.Values)
			{
				lock (this.RuntimeData.Mutex)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					long num2 = TimeUtil.NOW();
					if (langHunLingYuScene.m_eStatus == 0)
					{
						if (num2 >= langHunLingYuScene.StartTimeTicks)
						{
							LangHunLingYuFuBenData langHunLingYuFuBenData;
							if (this.RuntimeData.FuBenDataDict.TryGetValue(langHunLingYuScene.GameId, out langHunLingYuFuBenData) && langHunLingYuFuBenData.State == 3)
							{
								langHunLingYuScene.m_eStatus = 4;
								langHunLingYuScene.m_lLeaveTime = TimeUtil.NOW();
							}
							langHunLingYuScene.m_lPrepareTime = langHunLingYuScene.StartTimeTicks;
							langHunLingYuScene.m_lBeginTime = langHunLingYuScene.m_lPrepareTime + (long)(langHunLingYuScene.SceneInfo.PrepareSecs * 1000);
							langHunLingYuScene.m_eStatus = 1;
							langHunLingYuScene.StateTimeData.GameType = 10;
							langHunLingYuScene.StateTimeData.State = langHunLingYuScene.m_eStatus;
							langHunLingYuScene.StateTimeData.EndTicks = langHunLingYuScene.m_lBeginTime;
							foreach (CopyMap copyMap in langHunLingYuScene.CopyMapDict.Values)
							{
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, langHunLingYuScene.StateTimeData, copyMap);
							}
						}
					}
					else if (langHunLingYuScene.m_eStatus == 1)
					{
						if (num2 >= langHunLingYuScene.m_lBeginTime)
						{
							langHunLingYuScene.m_eStatus = 2;
							langHunLingYuScene.m_lEndTime = langHunLingYuScene.m_lBeginTime + (long)(langHunLingYuScene.SceneInfo.FightingSecs * 1000);
							langHunLingYuScene.StateTimeData.GameType = 10;
							langHunLingYuScene.StateTimeData.State = langHunLingYuScene.m_eStatus;
							langHunLingYuScene.StateTimeData.EndTicks = langHunLingYuScene.m_lEndTime;
							foreach (CopyMap copyMap in langHunLingYuScene.CopyMapDict.Values)
							{
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, langHunLingYuScene.StateTimeData, copyMap);
							}
						}
					}
					else if (langHunLingYuScene.m_eStatus == 2)
					{
						if (num2 >= langHunLingYuScene.m_lEndTime)
						{
							langHunLingYuScene.m_eStatus = 3;
							langHunLingYuScene.m_lLeaveTime = langHunLingYuScene.m_lEndTime + (long)(langHunLingYuScene.SceneInfo.ClearRolesSecs * 1000);
							langHunLingYuScene.StateTimeData.GameType = 10;
							langHunLingYuScene.StateTimeData.State = 5;
							langHunLingYuScene.StateTimeData.EndTicks = langHunLingYuScene.m_lLeaveTime;
							foreach (CopyMap copyMap in langHunLingYuScene.CopyMapDict.Values)
							{
								GameManager.ClientMgr.BroadSpecialCopyMapMessage<GameSceneStateTimeData>(827, langHunLingYuScene.StateTimeData, copyMap);
							}
							this.ProcessWangChengZhanResult(langHunLingYuScene, true);
						}
						else
						{
							if (langHunLingYuScene.HasGuangMu)
							{
								foreach (CopyMap copyMap in langHunLingYuScene.CopyMapDict.Values)
								{
									if (copyMap.MapCode == langHunLingYuScene.SceneInfo.MapCode)
									{
										langHunLingYuScene.HasGuangMu = false;
										for (int i = 4; i < 9; i++)
										{
											GameManager.CopyMapMgr.AddGuangMuEvent(copyMap, i, 0);
										}
									}
								}
							}
							this.ProcessWangChengZhanResult(langHunLingYuScene, false);
						}
					}
					else if (langHunLingYuScene.m_eStatus == 3)
					{
						langHunLingYuScene.m_eStatus = 4;
						LangHunLingYuStatisticalData langHunLingYuStatisticalData = new LangHunLingYuStatisticalData();
						langHunLingYuStatisticalData.CompliteTime = TimeUtil.NowDateTime();
						langHunLingYuStatisticalData.CityId = langHunLingYuScene.CityData.CityId;
						langHunLingYuStatisticalData.GameId = langHunLingYuScene.GameId;
						langHunLingYuStatisticalData.SiteBhids[0] = langHunLingYuScene.LongTaOwnerData.OwnerBHid;
						this.LangHunLingYuBuildMaxCityOwnerInfo(langHunLingYuStatisticalData, langHunLingYuScene.LongTaOwnerData.OwnerBHServerId);
						this.RuntimeData.StatisticalDataQueue.Enqueue(langHunLingYuStatisticalData);
						LangHunLingYuFuBenData langHunLingYuFuBenData;
						if (this.RuntimeData.FuBenDataDict.TryGetValue(langHunLingYuScene.GameId, out langHunLingYuFuBenData))
						{
							langHunLingYuFuBenData.State = 3;
						}
						lock (this.RuntimeData.Mutex)
						{
							LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx = null;
							LangHunLingYuCityDataEx langHunLingYuCityDataEx = null;
							if (this.RuntimeData.CityDataExDict.TryGetValue(langHunLingYuStatisticalData.CityId, out langHunLingYuCityDataEx))
							{
								this.RuntimeData.BangHuiDataExDict.TryGetValue(langHunLingYuCityDataEx.Site[0], out langHunLingYuBangHuiDataEx);
							}
							LangHunLingYuBangHuiDataEx langHunLingYuBangHuiDataEx2 = null;
							this.RuntimeData.BangHuiDataExDict.TryGetValue((long)langHunLingYuStatisticalData.SiteBhids[0], out langHunLingYuBangHuiDataEx2);
							int oldZoneID = (langHunLingYuBangHuiDataEx == null) ? 0 : langHunLingYuBangHuiDataEx.ZoneId;
							int oldBHID = (langHunLingYuBangHuiDataEx == null) ? 0 : langHunLingYuBangHuiDataEx.Bhid;
							int oldLev = (langHunLingYuBangHuiDataEx == null) ? 0 : langHunLingYuBangHuiDataEx.Level;
							int newZoneID = (langHunLingYuBangHuiDataEx2 == null) ? 0 : langHunLingYuBangHuiDataEx2.ZoneId;
							int newBHID = (langHunLingYuBangHuiDataEx2 == null) ? 0 : langHunLingYuBangHuiDataEx2.Bhid;
							int newLev = (langHunLingYuBangHuiDataEx2 == null) ? 0 : langHunLingYuBangHuiDataEx2.Level;
							EventLogManager.AddLangHunLingYuEvent(langHunLingYuStatisticalData.GameId, langHunLingYuStatisticalData.CityId, oldZoneID, oldBHID, oldLev, newZoneID, newBHID, newLev);
						}
					}
					else if (langHunLingYuScene.m_eStatus == 4)
					{
						if (num2 >= langHunLingYuScene.m_lLeaveTime)
						{
							foreach (CopyMap copyMap in langHunLingYuScene.CopyMapDict.Values)
							{
								copyMap.SetRemoveTicks(langHunLingYuScene.m_lLeaveTime + 300000L);
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
									DataHelper.WriteExceptionLogEx(ex, "圣域争霸系统清场调度异常");
								}
							}
							langHunLingYuScene.m_eStatus = 5;
						}
					}
				}
			}
		}

		public void NotifyTimeStateInfoAndScoreInfo(GameClient client, bool timeState = true, bool otherInfo = true)
		{
			lock (this.RuntimeData.Mutex)
			{
				LangHunLingYuScene langHunLingYuScene = client.SceneObject as LangHunLingYuScene;
				if (langHunLingYuScene != null)
				{
					if (timeState)
					{
						client.sendCmd<GameSceneStateTimeData>(827, langHunLingYuScene.StateTimeData, false);
					}
					if (otherInfo)
					{
						client.sendCmd<List<LangHunLingYuQiZhiBuffOwnerData>>(1151, langHunLingYuScene.QiZhiBuffOwnerDataList, false);
						client.sendCmd<List<BangHuiRoleCountData>>(1150, langHunLingYuScene.LongTaBHRoleCountList, false);
						client.sendCmd<LangHunLingYuLongTaOwnerData>(1152, langHunLingYuScene.LongTaOwnerData, false);
					}
				}
			}
		}

		public void ProcessWangChengZhanResult(LangHunLingYuScene scene, bool finish)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				int num = (int)((scene.m_lEndTime - TimeUtil.NOW()) / 1000L);
				if (!finish)
				{
					if (num < 0)
					{
						num = 0;
					}
					this.UpdateQiZhiBuffParams(num);
					bool flag = this.TryGenerateNewHuangChengBangHui(scene);
					if (flag)
					{
						this.HandleHuangChengResultEx(scene, false);
					}
					else
					{
						this.ProcessTimeAddRoleExp(scene);
					}
				}
				else
				{
					this.TryGenerateNewHuangChengBangHui(scene);
					this.HandleHuangChengResultEx(scene, true);
					this.GiveLangHunLingYuAwards(scene);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		private void UpdateQiZhiBuffParams(int secs)
		{
			lock (this.RuntimeData.Mutex)
			{
				foreach (int key in this.RuntimeData.QiZhiBuffEnableParamsDict.Keys)
				{
					this.RuntimeData.QiZhiBuffEnableParamsDict[key][0] = (double)secs;
				}
			}
		}

		private void GiveLangHunLingYuAwards(LangHunLingYuScene scene)
		{
			LangHunLingYuAwardsData langHunLingYuAwardsData = new LangHunLingYuAwardsData();
			LangHunLingYuAwardsData langHunLingYuAwardsData2 = new LangHunLingYuAwardsData();
			langHunLingYuAwardsData.Success = 1;
			langHunLingYuAwardsData.AwardsItemDataList = scene.LevelInfo.Award.Items;
			langHunLingYuAwardsData.successBhName = (langHunLingYuAwardsData2.successBhName = ((scene.LongTaOwnerData.OwnerBHid > 0) ? scene.LongTaOwnerData.OwnerBHName : ""));
			HashSet<int> hashSet = new HashSet<int>();
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				List<GameClient> clientsList = copyMap.GetClientsList();
				foreach (GameClient gameClient in clientsList)
				{
					if (gameClient.ClientData.GuanZhanGM > 0)
					{
						gameClient.sendCmd<LangHunLingYuAwardsData>(1159, langHunLingYuAwardsData, false);
					}
					else
					{
						LangHunLingYuAwardsData langHunLingYuAwardsData3 = (gameClient.ClientData.Faction == scene.LongTaOwnerData.OwnerBHid) ? langHunLingYuAwardsData : langHunLingYuAwardsData2;
						if (langHunLingYuAwardsData3.AwardsItemDataList != null)
						{
							if (Global.CanAddGoodsNum(gameClient, langHunLingYuAwardsData3.AwardsItemDataList.Count))
							{
								foreach (AwardsItemData awardsItemData in langHunLingYuAwardsData3.AwardsItemDataList)
								{
									Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, gameClient, awardsItemData.GoodsID, awardsItemData.GoodsNum, 0, "", awardsItemData.Level, awardsItemData.Binding, 0, "", true, 1, "圣域争霸胜利奖励", "1900-01-01 12:00:00", 0, 0, awardsItemData.IsHaveLuckyProp, 0, awardsItemData.ExcellencePorpValue, awardsItemData.AppendLev, 0, null, null, 0, true);
								}
							}
							else
							{
								Global.UseMailGivePlayerAward2(gameClient, langHunLingYuAwardsData3.AwardsItemDataList, GLang.GetLang(404, new object[0]), GLang.GetLang(404, new object[0]), 0, 0, 0);
							}
						}
						EventLogManager.AddRoleEvent(gameClient, OpTypes.GiveAwards, OpTags.LangHunLingYu, LogRecordType.IntValue2, new object[]
						{
							gameClient.ClientData.Faction,
							langHunLingYuAwardsData3.Success
						});
						if (hashSet.Add(gameClient.ClientData.Faction))
						{
							EventLogManager.AddGameEvent(LogRecordType.LangHunLingYu, new object[]
							{
								35,
								gameClient.ClientData.Faction,
								langHunLingYuAwardsData3.Success
							});
						}
						gameClient.sendCmd<LangHunLingYuAwardsData>(1159, langHunLingYuAwardsData3, false);
					}
				}
			}
		}

		public bool TryGenerateNewHuangChengBangHui(LangHunLingYuScene scene)
		{
			int num = 0;
			int ownerBHServerId = 0;
			this.GetTheOnlyOneBangHui(scene, out num, out ownerBHServerId);
			lock (this.RuntimeData.Mutex)
			{
				if (num <= 0 || num == scene.LongTaOwnerData.OwnerBHid)
				{
					scene.LastTheOnlyOneBangHui = 0;
					return false;
				}
				if (scene.LastTheOnlyOneBangHui != num)
				{
					scene.LastTheOnlyOneBangHui = num;
					scene.BangHuiTakeHuangGongTicks = TimeUtil.NOW();
					return false;
				}
				if (scene.LastTheOnlyOneBangHui > 0)
				{
					long num2 = TimeUtil.NOW();
					EventLogManager.AddGameEvent(LogRecordType.LangHunLingYuLongTaOnlyBangHuiLog, new object[]
					{
						scene.CityData.CityId,
						num,
						num2 - scene.BangHuiTakeHuangGongTicks,
						"狼魂领域龙塔占领持续时间"
					});
					if (num2 - scene.BangHuiTakeHuangGongTicks > (long)this.RuntimeData.MaxTakingHuangGongSecs)
					{
						scene.LongTaOwnerData.OwnerBHid = scene.LastTheOnlyOneBangHui;
						scene.LongTaOwnerData.OwnerBHName = this.GetBangHuiName(num, out scene.LongTaOwnerData.OwnerBHZoneId);
						scene.LongTaOwnerData.OwnerBHServerId = ownerBHServerId;
						return true;
					}
				}
			}
			return false;
		}

		public void GetTheOnlyOneBangHui(LangHunLingYuScene scene, out int newBHid, out int newBHServerID)
		{
			newBHid = 0;
			newBHServerID = 0;
			CopyMap copyMap;
			if (scene.CopyMapDict.TryGetValue(scene.SceneInfo.MapCode_LongTa, out copyMap))
			{
				List<GameClient> list = copyMap.GetClientsList();
				list = Global.GetMapAliveClientsEx(list, scene.SceneInfo.MapCode_LongTa, true, 0L);
				lock (this.RuntimeData.Mutex)
				{
					Dictionary<int, BangHuiRoleCountData> dictionary = new Dictionary<int, BangHuiRoleCountData>();
					for (int i = 0; i < list.Count; i++)
					{
						GameClient gameClient = list[i];
						if (gameClient.ClientData.GuanZhanGM <= 0)
						{
							int faction = gameClient.ClientData.Faction;
							if (faction > 0)
							{
								BangHuiRoleCountData bangHuiRoleCountData;
								if (!dictionary.TryGetValue(faction, out bangHuiRoleCountData))
								{
									bangHuiRoleCountData = new BangHuiRoleCountData
									{
										BHID = faction,
										RoleCount = 0,
										ServerID = gameClient.ServerId
									};
									dictionary.Add(faction, bangHuiRoleCountData);
								}
								bangHuiRoleCountData.RoleCount++;
							}
						}
					}
					scene.LongTaBHRoleCountList = dictionary.Values.ToList<BangHuiRoleCountData>();
					if (scene.LongTaBHRoleCountList.Count == 1)
					{
						newBHid = scene.LongTaBHRoleCountList[0].BHID;
						newBHServerID = scene.LongTaBHRoleCountList[0].ServerID;
						EventLogManager.AddGameEvent(LogRecordType.LangHunLingYuLongTaOnlyBangHuiLog, new object[]
						{
							scene.CityData.CityId,
							newBHid,
							-1,
							"狼魂领域龙塔唯一帮会"
						});
					}
				}
			}
		}

		public void NotifyLongTaRoleDataList(LangHunLingYuScene scene)
		{
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<BangHuiRoleCountData>>(1150, scene.LongTaBHRoleCountList, copyMap);
			}
		}

		public void NotifyLongTaOwnerData(LangHunLingYuScene scene)
		{
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<LangHunLingYuLongTaOwnerData>(1152, scene.LongTaOwnerData, copyMap);
			}
		}

		public void UpdateQiZhiBangHui(LangHunLingYuScene scene, int npcExtentionID, int bhid, string bhName, int zoneId)
		{
			int num = 0;
			int num2 = 0;
			lock (this.RuntimeData.Mutex)
			{
				for (int i = 0; i < scene.QiZhiBuffOwnerDataList.Count; i++)
				{
					if (scene.QiZhiBuffOwnerDataList[i].NPCID == npcExtentionID)
					{
						num = scene.QiZhiBuffOwnerDataList[i].OwnerBHID;
						scene.QiZhiBuffOwnerDataList[i].OwnerBHID = bhid;
						scene.QiZhiBuffOwnerDataList[i].OwnerBHName = bhName;
						scene.QiZhiBuffOwnerDataList[i].OwnerBHZoneId = zoneId;
						break;
					}
				}
				QiZhiConfig qiZhiConfig;
				if (this.RuntimeData.NPCID2QiZhiConfigDict.TryGetValue(npcExtentionID, out qiZhiConfig))
				{
					num2 = qiZhiConfig.BufferID;
				}
			}
			if (bhid != num)
			{
				if (npcExtentionID == this.RuntimeData.SuperQiZhiNpcId)
				{
					scene.SuperQiZhiOwnerBhid = bhid;
				}
				try
				{
					EquipPropItem equipPropItem = GameManager.EquipPropsMgr.FindEquipPropItem(num2);
					if (null != equipPropItem)
					{
						foreach (CopyMap copyMap in scene.CopyMapDict.Values)
						{
							List<GameClient> clientsList = copyMap.GetClientsList();
							for (int i = 0; i < clientsList.Count; i++)
							{
								GameClient gameClient = clientsList[i];
								if (gameClient != null)
								{
									bool add = false;
									if (gameClient.ClientData.Faction == num)
									{
										add = false;
									}
									else if (gameClient.ClientData.Faction == bhid)
									{
										add = true;
									}
									this.UpdateQiZhiBuff4GameClient(gameClient, equipPropItem, num2, add);
								}
							}
						}
					}
					this.NotifyQiZhiBuffOwnerDataList(scene);
				}
				catch (Exception ex)
				{
					LogManager.WriteException("旗帜状态变化,设置旗帜Buff时发生异常:" + ex.ToString());
				}
			}
		}

		private void UpdateQiZhiBuff4GameClient(GameClient client, EquipPropItem item, int bufferID, bool add)
		{
			try
			{
				if (add && null != item)
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						item.ExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffEnableParamsDict[bufferID], 1, true);
				}
				else
				{
					client.ClientData.PropsCacheManager.SetExtProps(new object[]
					{
						PropsSystemTypes.BufferByGoodsProps,
						bufferID,
						PropsCacheManager.ConstExtProps
					});
					Global.UpdateBufferData(client, (BufferItemTypes)bufferID, this.RuntimeData.QiZhiBuffDisableParamsDict[bufferID], 1, true);
				}
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public void NotifyQiZhiBuffOwnerDataList(LangHunLingYuScene scene)
		{
			lock (this.RuntimeData.Mutex)
			{
				byte[] array = DataHelper.ObjectToBytes<List<LangHunLingYuQiZhiBuffOwnerData>>(scene.QiZhiBuffOwnerDataList);
			}
			foreach (CopyMap copyMap in scene.CopyMapDict.Values)
			{
				GameManager.ClientMgr.BroadSpecialCopyMapMessage<List<LangHunLingYuQiZhiBuffOwnerData>>(1151, scene.QiZhiBuffOwnerDataList, copyMap);
			}
		}

		private void HandleHuangChengResultEx(LangHunLingYuScene scene, bool isBattleOver = false)
		{
			int ownerBHid = scene.LongTaOwnerData.OwnerBHid;
			string ownerBHName = scene.LongTaOwnerData.OwnerBHName;
			if (isBattleOver)
			{
				if (ownerBHid <= 0)
				{
					string msg = StringUtil.substitute(GLang.GetLang(405, new object[0]), new object[0]);
					foreach (CopyMap copymap in scene.CopyMapDict.Values)
					{
						GameManager.ClientMgr.BroadSpecialCopyMapMsg(copymap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
					}
					return;
				}
			}
			if (scene.LastTheOnlyOneBangHui > 0)
			{
				string msg;
				if (!isBattleOver)
				{
					long num = (scene.m_lEndTime - TimeUtil.NOW()) / 1000L;
					msg = StringUtil.substitute(GLang.GetLang(406, new object[0]), new object[]
					{
						ownerBHName,
						num / 60L,
						num % 60L
					});
				}
				else
				{
					msg = StringUtil.substitute(GLang.GetLang(671, new object[0]), new object[]
					{
						ownerBHName
					});
				}
				foreach (CopyMap copymap in scene.CopyMapDict.Values)
				{
					GameManager.ClientMgr.BroadSpecialCopyMapMsg(copymap, msg, ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes.Hot, 0);
				}
			}
		}

		public const SceneUIClasses ManagerType = 35;

		private static LangHunLingYuManager instance = new LangHunLingYuManager();

		public LangHunLingYuData RuntimeData = new LangHunLingYuData();

		public LevelAwardsMgr _LevelAwardsMgr = new LevelAwardsMgr();

		private object OwnerRoleDataMutex = new object();

		private RoleDataEx _OwnerRoleData = null;
	}
}
