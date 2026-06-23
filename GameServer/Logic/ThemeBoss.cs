using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class ThemeBoss : Activity, IManager, IEventListener, ICmdProcessorEx, ICmdProcessor
	{
		public static ThemeBoss getInstance()
		{
			return ThemeBoss.instance;
		}

		public bool initialize()
		{
			this.InitConfig();
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(910, 1, 1, ThemeBoss.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource.getInstance().registerListener(10, ThemeBoss.getInstance());
			GlobalEventSource.getInstance().registerListener(11, ThemeBoss.getInstance());
			return true;
		}

		public bool showdown()
		{
			GlobalEventSource.getInstance().removeListener(10, ThemeBoss.getInstance());
			GlobalEventSource.getInstance().removeListener(11, ThemeBoss.getInstance());
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
			return nID != 910 || this.ProcessThemeBossStateCmd(client, nID, bytes, cmdParams);
		}

		public void processEvent(EventObject eventObject)
		{
			int eventType = eventObject.getEventType();
			if (eventType == 11)
			{
				MonsterDeadEventObject monsterDeadEventObject = eventObject as MonsterDeadEventObject;
				this.OnProcessMonsterDead(monsterDeadEventObject.getAttacker(), monsterDeadEventObject.getMonster());
			}
		}

		public bool InitConfig()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ThemeDataConst.ThemeActivityBoss);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null == xelement)
				{
					return false;
				}
				this.FromDate = "-1";
				this.ToDate = "-1";
				this.AwardStartDate = "-1";
				this.AwardEndDate = "-1";
				Dictionary<int, ThemeBossConfig> dictionary = new Dictionary<int, ThemeBossConfig>();
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					ThemeBossConfig themeBossConfig = new ThemeBossConfig();
					string[] array = Global.GetDefAttributeStr(xelement2, "MaxLevel", "0").Split(new char[]
					{
						'|'
					});
					if (array.Length >= 2)
					{
						themeBossConfig.ID = (int)Global.GetSafeAttributeLong(xelement2, "ID");
						themeBossConfig.MonstersID = (int)Global.GetSafeAttributeLong(xelement2, "MonstersID");
						themeBossConfig.MapCode = (int)Global.GetSafeAttributeLong(xelement2, "MapCode");
						themeBossConfig.PosX = (int)Global.GetSafeAttributeLong(xelement2, "X");
						themeBossConfig.PosY = (int)Global.GetSafeAttributeLong(xelement2, "Y");
						themeBossConfig.Radius = (int)Global.GetSafeAttributeLong(xelement2, "Radius");
						themeBossConfig.Num = (int)Global.GetSafeAttributeLong(xelement2, "Num");
						themeBossConfig.MaxUnionLevel = Global.GetUnionLevel2(Global.SafeConvertToInt32(array[0]), Global.SafeConvertToInt32(array[1]));
						if (!ConfigParser.ParserTimeRangeList(themeBossConfig.TimePoints, xelement2.Attribute("TimePoints").Value.ToString(), true, '|', '-'))
						{
							throw new Exception(string.Format("读取{0}时间配置(TimePoints)出错", text));
						}
						for (int i = 0; i < themeBossConfig.TimePoints.Count; i++)
						{
							TimeSpan timeSpan = new TimeSpan(themeBossConfig.TimePoints[i].Hours, themeBossConfig.TimePoints[i].Minutes, themeBossConfig.TimePoints[i].Seconds);
							themeBossConfig.SecondsOfDay.Add(timeSpan.TotalSeconds);
						}
						dictionary[themeBossConfig.ID] = themeBossConfig;
					}
				}
				List<int> broadGoodsIDList = new List<int>();
				string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ThemeActivityBOSSGoods");
				if (!string.IsNullOrEmpty(paramValueByName))
				{
					broadGoodsIDList = Array.ConvertAll<string, int>(paramValueByName.Split(new char[]
					{
						','
					}), (string _x) => Global.SafeConvertToInt32(_x)).ToList<int>();
				}
				lock (ThemeBoss.Mutex)
				{
					this.ThemeBossConfigDict = dictionary;
					this.BroadGoodsIDList = broadGoodsIDList;
				}
				this.ActivityType = 155;
				base.PredealDateTime();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
				return false;
			}
			return true;
		}

		public bool ProcessThemeBossStateCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int num2 = 0;
				int num3 = 0;
				if (this.SceneDict.Count != 0)
				{
					ThemeBossScene value = this.SceneDict.FirstOrDefault<KeyValuePair<int, ThemeBossScene>>().Value;
					if (value.State == BattleStates.StartFight)
					{
						if (value.AliveBossNum > 0)
						{
							num3 = 1;
						}
						else
						{
							num3 = 2;
						}
					}
					else
					{
						num3 = 0;
					}
					num2 = value.BossConfigInfo.ID;
				}
				client.sendCmd(nID, string.Format("{0}:{1}", num2, num3), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return true;
		}

		public void OnProcessMonsterDead(GameClient client, Monster monster)
		{
			if (ThemeBoss.getInstance().IsThemeBoss(monster))
			{
				ThemeBossScene themeBossScene = null;
				if (this.SceneDict.TryGetValue(monster.CurrentMapCode, out themeBossScene))
				{
					int attackerFromList = monster.GetAttackerFromList();
					if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
						if (null != gameClient)
						{
							client = gameClient;
						}
					}
					string msgText = string.Format(GLang.GetLang(4016, new object[0]), Global.FormatRoleNameWithZoneId2(client));
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, msgText, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
					themeBossScene.AliveBossNum--;
					if (themeBossScene.AliveBossNum <= 0)
					{
						themeBossScene.State = BattleStates.EndFight;
					}
				}
			}
		}

		private void GenerateThemeBossScene()
		{
			int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
			if (offsetDay != this.SceneDayID)
			{
				this.SceneDict.Clear();
				string cmd = StringUtil.substitute("{0}:{1}:{2}", new object[]
				{
					0,
					5,
					100
				});
				PaiHangData paiHangData = Global.sendToDB<PaiHangData, string>(269, cmd, 0);
				if (null != paiHangData)
				{
					long num = 0L;
					long num2 = 0L;
					List<PaiHangItemData> paiHangList = paiHangData.PaiHangList;
					if (null != paiHangList)
					{
						int num3 = Math.Min(paiHangList.Count, 100);
						for (int i = 0; i < num3; i++)
						{
							PaiHangItemData paiHangItemData = paiHangList[i];
							num += (long)paiHangItemData.Val1;
							num2 += (long)paiHangItemData.Val2;
						}
						int num4 = Global.GetUnionLevel2((int)num2, (int)num) / num3;
						num = (long)((num4 - 1) / 100);
						num2 = (long)((num4 - 1) % 100 + 1);
					}
					int unionLevel = Global.GetUnionLevel2((int)num2, (int)num);
					foreach (ThemeBossConfig themeBossConfig in this.ThemeBossConfigDict.Values)
					{
						if (unionLevel <= themeBossConfig.MaxUnionLevel)
						{
							ThemeBossScene themeBossScene = null;
							if (!this.SceneDict.TryGetValue(themeBossConfig.MapCode, out themeBossScene))
							{
								themeBossScene = new ThemeBossScene();
								themeBossScene.MapCode = themeBossConfig.MapCode;
								themeBossScene.BossConfigInfo = themeBossConfig;
								themeBossScene.State = BattleStates.NoBattle;
								this.SceneDict[themeBossConfig.MapCode] = themeBossScene;
							}
							if (themeBossScene.BossConfigInfo.MaxUnionLevel > themeBossConfig.MaxUnionLevel)
							{
								themeBossScene.BossConfigInfo = themeBossConfig;
							}
						}
					}
					this.SceneDayID = offsetDay;
				}
			}
		}

		private bool GetStartEndTime(int sceneId, out long StartTick, out long EndTick)
		{
			StartTick = 0L;
			EndTick = 0L;
			ThemeBossScene themeBossScene = null;
			DateTime dateTime = TimeUtil.NowDateTime();
			lock (ThemeBoss.Mutex)
			{
				if (!this.SceneDict.TryGetValue(sceneId, out themeBossScene))
				{
					return false;
				}
			}
			lock (ThemeBoss.Mutex)
			{
				for (int i = 0; i < themeBossScene.BossConfigInfo.TimePoints.Count - 1; i += 2)
				{
					if (dateTime.TimeOfDay.TotalSeconds >= themeBossScene.BossConfigInfo.SecondsOfDay[i] - 180.0 && dateTime.TimeOfDay.TotalSeconds <= themeBossScene.BossConfigInfo.SecondsOfDay[i + 1])
					{
						StartTick = dateTime.Date.AddSeconds(themeBossScene.BossConfigInfo.SecondsOfDay[i]).Ticks / 10000L;
						EndTick = dateTime.Date.AddSeconds(themeBossScene.BossConfigInfo.SecondsOfDay[i + 1]).Ticks / 10000L;
						return true;
					}
				}
			}
			return false;
		}

		public bool JudgeCanTriggerActivity(ThemeBossScene scene, DateTime now)
		{
			lock (ThemeBoss.Mutex)
			{
				for (int i = 0; i < scene.BossConfigInfo.TimePoints.Count - 1; i += 2)
				{
					if (now.TimeOfDay.TotalSeconds >= scene.BossConfigInfo.SecondsOfDay[i] - 180.0 && now.TimeOfDay.TotalSeconds <= scene.BossConfigInfo.SecondsOfDay[i])
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsThemeBossGoods(int goodsID)
		{
			bool result;
			lock (ThemeBoss.Mutex)
			{
				result = this.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		public bool IsThemeBossScene(int mapCode)
		{
			bool result;
			lock (ThemeBoss.Mutex)
			{
				result = this.SceneDict.ContainsKey(mapCode);
			}
			return result;
		}

		public bool IsThemeBoss(Monster monster)
		{
			bool result;
			if (401 != monster.MonsterType)
			{
				result = false;
			}
			else
			{
				ThemeBossScene themeBossScene = null;
				result = (this.SceneDict.TryGetValue(monster.CurrentMapCode, out themeBossScene) && themeBossScene.BossConfigInfo.MonstersID == monster.MonsterInfo.ExtensionID);
			}
			return result;
		}

		public void TimerProc()
		{
			if (!GameManager.IsKuaFuServer)
			{
				if (155 == this.ActivityType && this.InActivityTime())
				{
					DateTime now = TimeUtil.NowDateTime();
					long num = now.Ticks / 10000L;
					lock (ThemeBoss.Mutex)
					{
						if (Math.Abs(num - ThemeBoss.LastHeartBeatTicks) < 1000L)
						{
							return;
						}
						ThemeBoss.LastHeartBeatTicks = num;
						this.GenerateThemeBossScene();
					}
					foreach (KeyValuePair<int, ThemeBossScene> keyValuePair in this.SceneDict)
					{
						lock (ThemeBoss.Mutex)
						{
							switch (keyValuePair.Value.State)
							{
							case BattleStates.NoBattle:
								if (this.JudgeCanTriggerActivity(keyValuePair.Value, now))
								{
									if (this.GetStartEndTime(keyValuePair.Key, out keyValuePair.Value.StartTick, out keyValuePair.Value.EndTick))
									{
										keyValuePair.Value.State = BattleStates.WaitingFight;
									}
								}
								break;
							case BattleStates.WaitingFight:
								if (num >= keyValuePair.Value.StartTick)
								{
									ThemeBossConfig bossConfigInfo = keyValuePair.Value.BossConfigInfo;
									Monster monster = GameManager.MonsterZoneMgr.AddDynamicMonsters(keyValuePair.Value.MapCode, bossConfigInfo.MonstersID, -1, bossConfigInfo.Num, bossConfigInfo.PosX / 100, bossConfigInfo.PosY / 100, bossConfigInfo.Radius, 0, 0, null, null);
									if (null != monster)
									{
										keyValuePair.Value.State = BattleStates.StartFight;
										keyValuePair.Value.AliveBossNum = bossConfigInfo.Num;
										Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, GLang.GetLang(4013, new object[0]), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
									}
								}
								break;
							case BattleStates.StartFight:
								if (!keyValuePair.Value.BroadCast4014 && (keyValuePair.Value.EndTick - num) / 1000L <= 180L)
								{
									Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, GLang.GetLang(4014, new object[0]), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
									keyValuePair.Value.BroadCast4014 = true;
								}
								if (num >= keyValuePair.Value.EndTick)
								{
									keyValuePair.Value.State = BattleStates.EndFight;
									List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(keyValuePair.Value.MapCode);
									foreach (object obj in objectsByMap)
									{
										Monster monster2 = obj as Monster;
										if (monster2 != null && monster2.MonsterInfo.ExtensionID == keyValuePair.Value.BossConfigInfo.MonstersID)
										{
											GameManager.MonsterMgr.DeadMonsterImmediately(monster2);
										}
									}
								}
								break;
							case BattleStates.EndFight:
								keyValuePair.Value.State = BattleStates.NoBattle;
								keyValuePair.Value.BroadCast4014 = false;
								keyValuePair.Value.AliveBossNum = 0;
								break;
							}
						}
					}
				}
			}
		}

		public const int MaxActiveConditionDataNum = 100;

		private static object Mutex = new object();

		private static long LastHeartBeatTicks = 0L;

		private Dictionary<int, ThemeBossConfig> ThemeBossConfigDict = new Dictionary<int, ThemeBossConfig>();

		private List<int> BroadGoodsIDList = new List<int>();

		public ConcurrentDictionary<int, ThemeBossScene> SceneDict = new ConcurrentDictionary<int, ThemeBossScene>();

		public int SceneDayID;

		private static ThemeBoss instance = new ThemeBoss();
	}
}
