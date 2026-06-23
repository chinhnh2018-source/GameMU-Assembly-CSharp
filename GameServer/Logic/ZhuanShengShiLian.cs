using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public static class ZhuanShengShiLian
	{
		public static void LoadZhuanShengShiLianXml()
		{
			string text = "";
			try
			{
				text = Global.GameResPath(ThemeDataConst.ThemeActivityZhuanSheng);
				XElement xelement = CheckHelper.LoadXml(text, true);
				if (null != xelement)
				{
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.FromDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ToDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.AwardStartDate = "-1";
					ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.AwardEndDate = "-1";
					Dictionary<int, ZhuanShengMapInfo> dictionary = new Dictionary<int, ZhuanShengMapInfo>();
					List<int> list = new List<int>();
					IEnumerable<XElement> enumerable = xelement.Elements();
					foreach (XElement xml in enumerable)
					{
						int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MapID", "0"));
						string[] array = Global.GetDefAttributeStr(xml, "MinLevel", "0").Split(new char[]
						{
							'|'
						});
						if (array.Length >= 2)
						{
							string[] array2 = Global.GetDefAttributeStr(xml, "MaxLevel", "0").Split(new char[]
							{
								'|'
							});
							if (array2.Length >= 2)
							{
								ZhuanShengMapInfo zhuanShengMapInfo = new ZhuanShengMapInfo
								{
									MonstersID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MonstersID", "0")),
									MapCode = num,
									ReadyTime = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ReadyTime", "0")),
									FightSecs = Convert.ToInt32(Global.GetDefAttributeStr(xml, "FightSecs", "0")),
									ClearRolesSecs = Convert.ToInt32(Global.GetDefAttributeStr(xml, "ClearRolesSecs", "0")),
									MaxEnterNum = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxEnterNum", "0")),
									BornX = Convert.ToInt32(Global.GetDefAttributeStr(xml, "X", "0")),
									BornY = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Y", "0")),
									MinZhuanSheng = Convert.ToInt32(array[0]),
									MinLevel = Convert.ToInt32(array[1]),
									MaxZhuanSheng = Convert.ToInt32(array2[0]),
									MaxLevel = Convert.ToInt32(array2[1])
								};
								List<string> list2 = new List<string>();
								string defAttributeStr = Global.GetDefAttributeStr(xml, "TimePoints", "0");
								if (!string.IsNullOrEmpty(defAttributeStr))
								{
									string[] array3 = defAttributeStr.Split(new char[]
									{
										'-'
									});
									for (int i = 0; i < array3.Length; i++)
									{
										list2.Add(array3[i].Trim());
									}
								}
								zhuanShengMapInfo.TimePoints = list2;
								dictionary[num] = zhuanShengMapInfo;
							}
						}
					}
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict = dictionary;
					}
					text = Global.GameResPath(ThemeDataConst.ThemeActivityZhuanShengReward);
					xelement = CheckHelper.LoadXml(text, true);
					if (null != xelement)
					{
						Dictionary<int, List<ShiLianReward>> dictionary2 = new Dictionary<int, List<ShiLianReward>>();
						enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MapID", "0"));
							List<ShiLianReward> list3;
							if (!dictionary2.TryGetValue(num, out list3))
							{
								list3 = new List<ShiLianReward>();
								dictionary2[num] = list3;
							}
							list3.Add(new ShiLianReward
							{
								MinRank = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MinRank", "0")),
								MaxRank = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MaxRank", "0")),
								WinRewardItem = Global.GetDefAttributeStr(xml, "WinRewardItem", ""),
								WinrewardExp = Convert.ToInt32(Global.GetDefAttributeStr(xml, "WinrewardExp", "0")),
								WinRewardMoney = Convert.ToInt32(Global.GetDefAttributeStr(xml, "WinRewardMoney", "0")),
								LoseRewardItem = Global.GetDefAttributeStr(xml, "LoseRewardItem", "0"),
								LoseRewardExp = Convert.ToInt32(Global.GetDefAttributeStr(xml, "LoseRewardExp", "0")),
								LoseRewardMoney = Convert.ToInt32(Global.GetDefAttributeStr(xml, "LoseRewardMoney", "0"))
							});
						}
						List<int> broadGoodsIDList = new List<int>();
						string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ThemeActivityZhuanShengGoods");
						if (!string.IsNullOrEmpty(paramValueByName))
						{
							broadGoodsIDList = Array.ConvertAll<string, int>(paramValueByName.Split(new char[]
							{
								','
							}), (string _x) => Convert.ToInt32(_x)).ToList<int>();
						}
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							ZhuanShengShiLian.ZhuanShengRunTimeData.ShiLianRewardDict = dictionary2;
							ZhuanShengShiLian.ZhuanShengRunTimeData.BroadGoodsIDList = broadGoodsIDList;
						}
						ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ActivityType = 157;
						ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.PredealDateTime();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public static int GetZhuanShengShiLianMapCodeIDForRole(GameClient client)
		{
			int result = -1;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				foreach (ZhuanShengMapInfo zhuanShengMapInfo in ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.Values)
				{
					int unionLevel = Global.GetUnionLevel2(zhuanShengMapInfo.MaxZhuanSheng, zhuanShengMapInfo.MaxLevel);
					int unionLevel2 = Global.GetUnionLevel2(zhuanShengMapInfo.MinZhuanSheng, zhuanShengMapInfo.MinLevel);
					int unionLevel3 = Global.GetUnionLevel2(client);
					if (unionLevel3 >= unionLevel2 && unionLevel3 <= unionLevel)
					{
						result = zhuanShengMapInfo.MapCode;
						break;
					}
				}
			}
			return result;
		}

		public static bool EnterSceneCopyScene(GameClient client, out int nSeqID, int mapCode)
		{
			nSeqID = -1;
			ZhuanShengMapInfo zhuanShengMapInfo;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				if (!ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.TryGetValue(mapCode, out zhuanShengMapInfo))
				{
					return false;
				}
			}
			bool result;
			if (!ZhuanShengShiLian.JudgeCanEnterOnTime(zhuanShengMapInfo))
			{
				result = false;
			}
			else
			{
				int zhuanShengShiLianMapCodeIDForRole = ZhuanShengShiLian.GetZhuanShengShiLianMapCodeIDForRole(client);
				if (zhuanShengShiLianMapCodeIDForRole <= 0 || mapCode != zhuanShengShiLianMapCodeIDForRole)
				{
					result = false;
				}
				else
				{
					ZSSLScene zsslscene = null;
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						foreach (KeyValuePair<int, ZSSLScene> keyValuePair in ZhuanShengShiLian.SceneDict)
						{
							if (keyValuePair.Value.SceneInfo.MapCode == mapCode)
							{
								zsslscene = keyValuePair.Value;
								nSeqID = keyValuePair.Key;
								break;
							}
						}
						if (null == zsslscene)
						{
							nSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
							zsslscene = new ZSSLScene();
							zsslscene.CleanAllInfo();
							zsslscene.SceneInfo = zhuanShengMapInfo;
							ZhuanShengShiLian.SceneDict[nSeqID] = zsslscene;
						}
					}
					if (null != zsslscene.m_CopyMap)
					{
						if (zsslscene.m_CopyMap.GetGameClientCount() >= zhuanShengMapInfo.MaxEnterNum)
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		public static void AddCopyScenes(int nSequenceID, int nFubenID, int nMapCodeID, CopyMap mapInfo)
		{
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZSSLScene zsslscene;
				if (ZhuanShengShiLian.SceneDict.TryGetValue(nSequenceID, out zsslscene))
				{
					zsslscene.m_CopyMap = mapInfo;
				}
			}
		}

		public static void RemoveCopyScenes(CopyMap cmInfo, int nSqeID, int nCopyID)
		{
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZSSLScene zsslscene;
				ZhuanShengShiLian.SceneDict.TryRemove(nSqeID, out zsslscene);
			}
		}

		public static void OnEnterScene(GameClient client)
		{
			ZhuanShengShiLian.SendTimeInfoToClient(client);
		}

		public static void SendTimeInfoToAll(ZSSLScene scene, long ticks)
		{
			List<GameClient> clientsList = scene.m_CopyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null)
					{
						int num;
						int state;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							num = (int)((scene.StatusEndTime - ticks) / 1000L);
							state = (int)scene.State;
						}
						string cmdData = string.Format("{0}:{1}", state, num);
						gameClient.sendCmd(1909, cmdData, false);
					}
				}
			}
		}

		public static void SendTimeInfoToClient(GameClient client)
		{
			ZSSLScene zsslscene;
			if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
			{
				long num = TimeUtil.NOW();
				int num2;
				int state;
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					num2 = (int)((zsslscene.StatusEndTime - num) / 1000L);
					state = (int)zsslscene.State;
				}
				string cmdData = string.Format("{0}:{1}", state, num2);
				client.sendCmd(1909, cmdData, false);
			}
		}

		public static void GiveGoodsAward(GameClient client, string goods)
		{
			string[] array = goods.Split(new char[]
			{
				'|'
			});
			List<GoodsData> list = new List<GoodsData>();
			for (int i = 0; i < array.Length; i++)
			{
				if (!(array[i] == ""))
				{
					string[] array2 = array[i].Split(new char[]
					{
						','
					});
					if (array2.Length == 7)
					{
						GoodsData goodsData = new GoodsData
						{
							Id = -1,
							GoodsID = Convert.ToInt32(array2[0]),
							Using = 0,
							Forge_level = Convert.ToInt32(array2[3]),
							Starttime = "1900-01-01 12:00:00",
							Endtime = "1900-01-01 12:00:00",
							Site = 0,
							GCount = Convert.ToInt32(array2[1]),
							Binding = Convert.ToInt32(array2[2]),
							BagIndex = 0,
							Lucky = Convert.ToInt32(array2[5]),
							ExcellenceInfo = Convert.ToInt32(array2[6]),
							AppendPropLev = Convert.ToInt32(array2[4])
						};
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData.GoodsID, out systemXmlItem))
						{
							string textMsg = string.Format("系统中不存在{0}", goodsData.GoodsID);
							GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg);
						}
						else
						{
							list.Add(goodsData);
						}
					}
				}
			}
			if (!Global.CanAddGoodsNum(client, array.Length))
			{
				Global.UseMailGivePlayerAward2(client, list, GLang.GetLang(4011, new object[0]), GLang.GetLang(4011, new object[0]), 0, 0, 0);
			}
			else
			{
				foreach (GoodsData goodsData2 in list)
				{
					SystemXmlItem systemXmlItem2 = null;
					GameManager.SystemGoods.SystemXmlItemDict.TryGetValue(goodsData2.GoodsID, out systemXmlItem2);
					string stringValue = systemXmlItem2.GetStringValue("Title");
					LogManager.WriteLog(3, string.Format("转生试炼奖励{0} {1}", client.ClientData.RoleID, stringValue), null, true);
					Global.AddGoodsDBCommand(Global._TCPManager.TcpOutPacketPool, client, goodsData2.GoodsID, goodsData2.GCount, goodsData2.Quality, "", goodsData2.Forge_level, goodsData2.Binding, goodsData2.Site, "", true, 1, "转生试炼奖励", "1900-01-01 12:00:00", 0, 0, goodsData2.Lucky, 0, goodsData2.ExcellenceInfo, goodsData2.AppendPropLev, 0, null, null, 0, true);
				}
			}
		}

		public static int KillerRid(GameClient client, Monster monster)
		{
			ZSSLScene zsslscene;
			int result;
			if (!ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
			{
				result = 0;
			}
			else
			{
				int num = 0;
				long num2 = 0L;
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					if (zsslscene.AttackLog.BHAttackRank == null || zsslscene.AttackLog.BHAttackRank.Count < 1)
					{
						return 0;
					}
					BHAttackLog bhattackLog = zsslscene.AttackLog.BHAttackRank[0];
					if (null == bhattackLog.RoleInjure)
					{
						return 0;
					}
					foreach (KeyValuePair<int, long> keyValuePair in bhattackLog.RoleInjure)
					{
						if (keyValuePair.Value > num2)
						{
							num = keyValuePair.Key;
							num2 = keyValuePair.Value;
						}
					}
				}
				result = num;
			}
			return result;
		}

		public static bool IsShiLianGoods(int goodsID)
		{
			bool result;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				result = ZhuanShengShiLian.ZhuanShengRunTimeData.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		public static void BroadMsg(int mapCode, string broadMsg)
		{
			int minZhuanSheng = 0;
			int minLevel = 0;
			int maxZhuanSheng = 100;
			int maxLevel = 100;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				ZhuanShengMapInfo zhuanShengMapInfo;
				if (!ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.TryGetValue(mapCode, out zhuanShengMapInfo))
				{
					return;
				}
				minZhuanSheng = zhuanShengMapInfo.MinZhuanSheng;
				minLevel = zhuanShengMapInfo.MinLevel;
				maxZhuanSheng = zhuanShengMapInfo.MaxZhuanSheng;
				maxLevel = zhuanShengMapInfo.MaxLevel;
			}
			Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, broadMsg, true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, minZhuanSheng, minLevel, maxZhuanSheng, maxLevel);
		}

		public static void BroadBossLife(ZSSLScene mapInfo, GameClient client, bool Top5Chg)
		{
			if (null != mapInfo.AttackLog)
			{
				BossLifeLog bossLifeLog = new BossLifeLog();
				bossLifeLog.InjureSum = mapInfo.AttackLog.InjureSum;
				if (null != mapInfo.AttackLog.BHAttackRank)
				{
					int count = Global.GMin(mapInfo.AttackLog.BHAttackRank.Count, 5);
					bossLifeLog.BHAttackRank = mapInfo.AttackLog.BHAttackRank.GetRange(0, count);
				}
				List<GameClient> clientsList = mapInfo.m_CopyMap.GetClientsList();
				if (clientsList != null && clientsList.Count > 0)
				{
					for (int i = 0; i < clientsList.Count; i++)
					{
						GameClient gameClient = clientsList[i];
						if (gameClient != null)
						{
							if (!Top5Chg && null != client)
							{
								if (client.ClientData.TeamID > 0 && client.ClientData.TeamID != gameClient.ClientData.TeamID)
								{
									goto IL_189;
								}
								if (gameClient.ClientData.RoleID != client.ClientData.RoleID)
								{
									goto IL_189;
								}
							}
							if (null != mapInfo.AttackLog.BHInjure)
							{
								long guid = ZhuanShengShiLian.GetGUID(gameClient.ClientData.TeamID, gameClient.ClientData.RoleID);
								mapInfo.AttackLog.BHInjure.TryGetValue(guid, out bossLifeLog.SelfBHAttack);
							}
							gameClient.sendCmd<BossLifeLog>(1906, bossLifeLog, false);
						}
						IL_189:;
					}
				}
			}
		}

		public static void ProcessAttack(GameClient client, Monster monster, int injure)
		{
			try
			{
				if (injure > 0)
				{
					long guid = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
					string roleName = client.ClientData.RoleName;
					TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
					if (null != teamData)
					{
						lock (teamData)
						{
							TeamMemberData leader = teamData.GetLeader();
							if (null != leader)
							{
								roleName = leader.RoleName;
							}
						}
					}
					ZSSLScene zsslscene;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
					{
						bool top5Chg = false;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog bossAttackLog = zsslscene.AttackLog;
							if (null == bossAttackLog)
							{
								bossAttackLog = new BossAttackLog
								{
									InjureSum = 0L,
									BHInjure = new Dictionary<long, BHAttackLog>(),
									BHAttackRank = new List<BHAttackLog>()
								};
							}
							BHAttackLog bhattackLog;
							if (!bossAttackLog.BHInjure.TryGetValue(guid, out bhattackLog))
							{
								bhattackLog = new BHAttackLog
								{
									BHName = roleName,
									BHInjure = 0L,
									RoleInjure = new Dictionary<int, long>()
								};
								bossAttackLog.BHInjure[guid] = bhattackLog;
							}
							if (!bhattackLog.RoleInjure.ContainsKey(client.ClientData.RoleID))
							{
								bhattackLog.RoleInjure[client.ClientData.RoleID] = 0L;
							}
							Dictionary<int, long> roleInjure;
							int roleID;
							(roleInjure = bhattackLog.RoleInjure)[roleID = client.ClientData.RoleID] = roleInjure[roleID] + (long)injure;
							bhattackLog.BHInjure += (long)injure;
							top5Chg = ZhuanShengShiLian.TrySortAttackRank(bossAttackLog, bhattackLog);
							bossAttackLog.InjureSum += (long)injure;
						}
						ZhuanShengShiLian.BroadBossLife(zsslscene, client, top5Chg);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZhuanShengShiLian :: 处理攻击boss异常。", new object[0]), ex, true);
			}
		}

		private static bool TrySortAttackRank(BossAttackLog bossAttackLog, BHAttackLog myAttackLog)
		{
			bool result;
			if (bossAttackLog == null || null == myAttackLog)
			{
				result = false;
			}
			else
			{
				BHAttackLog bhattackLog = null;
				bool flag = false;
				bool flag2 = false;
				if (bossAttackLog.BHAttackRank.Count >= 20)
				{
					int num = Global.GMin(bossAttackLog.BHAttackRank.Count, 20);
					bhattackLog = bossAttackLog.BHAttackRank[num - 1];
					if (myAttackLog.BHInjure > bhattackLog.BHInjure)
					{
						flag2 = true;
					}
				}
				else if (bossAttackLog.BHAttackRank.Count < 20)
				{
					flag2 = true;
				}
				if (bossAttackLog.BHAttackRank.Exists((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog)))
				{
					if (bhattackLog != null && myAttackLog.BHInjure > bhattackLog.BHInjure)
					{
						flag = true;
					}
					else
					{
						flag2 = true;
					}
				}
				int num2 = bossAttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog));
				bool flag3 = num2 >= 0 && num2 < 5;
				if (flag)
				{
					bossAttackLog.BHAttackRank.Sort((BHAttackLog x, BHAttackLog y) => (int)(y.BHInjure - x.BHInjure));
				}
				if (flag2)
				{
					bossAttackLog.BHAttackRank = bossAttackLog.BHInjure.Values.ToList<BHAttackLog>();
					bossAttackLog.BHAttackRank.Sort((BHAttackLog x, BHAttackLog y) => (int)(y.BHInjure - x.BHInjure));
					int num = Global.GMin(bossAttackLog.BHAttackRank.Count, 20);
					bossAttackLog.BHAttackRank = bossAttackLog.BHAttackRank.GetRange(0, num);
				}
				num2 = bossAttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, myAttackLog));
				flag3 |= (num2 >= 0 && num2 < 5);
				result = flag3;
			}
			return result;
		}

		public static void ProcessBossDie(GameClient client, Monster monster)
		{
			try
			{
				ZSSLScene zsslscene;
				if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
				{
					lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
					{
						zsslscene.BossDie = true;
						zsslscene.State = BattleStates.EndFight;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZhuanShengShiLian :: 处理boss死亡异常。", new object[0]), ex, true);
			}
		}

		public static long GetGUID(int teamID, int roleID)
		{
			long num = (long)((teamID > 0) ? teamID : 0);
			long num2 = (long)((teamID > 0) ? 0 : roleID);
			return num << 32 | num2;
		}

		public static int CheckInviteOrApplyTeam(GameClient client, GameClient otherClient)
		{
			int result;
			if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && !ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = 0;
			}
			else if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && !ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = -101;
			}
			else if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode) && ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(otherClient.ClientData.MapCode))
			{
				result = -102;
			}
			else if (client.ClientData.FuBenSeqID != otherClient.ClientData.FuBenSeqID)
			{
				result = -101;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static void OnCreateTeamCopyRoleLog(GameClient client)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene zsslscene;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
					{
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog attackLog = zsslscene.AttackLog;
							if (null != attackLog)
							{
								long guid = ZhuanShengShiLian.GetGUID(0, client.ClientData.RoleID);
								long guid2 = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
								BHAttackLog bhattackLog = null;
								if (attackLog.BHInjure.TryGetValue(guid, out bhattackLog))
								{
									attackLog.BHInjure.Remove(guid);
								}
								if (null != bhattackLog)
								{
									attackLog.BHInjure[guid2] = bhattackLog;
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZhuanShengShiLian :: 处理拷贝角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		public static void ProcessClearRoleLog(GameClient client)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene zsslscene;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
					{
						long guid = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
						int num = -1;
						TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
						if (null != teamData)
						{
							num = teamData.LeaderRoleID;
						}
						bool top5Chg = false;
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							BossAttackLog attackLog = zsslscene.AttackLog;
							if (null == attackLog)
							{
								return;
							}
							BHAttackLog bhattackLog;
							if (attackLog.BHInjure.TryGetValue(guid, out bhattackLog))
							{
								if (num == -1)
								{
									bhattackLog.BHInjure = 0L;
									attackLog.BHInjure.Remove(guid);
								}
								else
								{
									long num2 = 0L;
									if (bhattackLog.RoleInjure.TryGetValue(client.ClientData.RoleID, out num2))
									{
										bhattackLog.RoleInjure.Remove(client.ClientData.RoleID);
										bhattackLog.BHInjure -= num2;
										if (bhattackLog.BHInjure <= 0L)
										{
											attackLog.BHInjure.Remove(guid);
										}
									}
								}
								top5Chg = ZhuanShengShiLian.TrySortAttackRank(attackLog, bhattackLog);
							}
						}
						ZhuanShengShiLian.BroadBossLife(zsslscene, client, top5Chg);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZhuanShengShiLian :: 处理清除角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		public static void ProcessChangeTeamName(GameClient client, bool needBroad = false)
		{
			try
			{
				if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
				{
					ZSSLScene zsslscene;
					if (ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
					{
						TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
						if (null != teamData)
						{
							string bhname = "";
							long guid = ZhuanShengShiLian.GetGUID(client.ClientData.TeamID, client.ClientData.RoleID);
							lock (teamData)
							{
								if (teamData.LeaderRoleID == client.ClientData.RoleID)
								{
									return;
								}
								TeamMemberData leader = teamData.GetLeader();
								if (null == leader)
								{
									return;
								}
								bhname = leader.RoleName;
							}
							BHAttackLog tAttackLog;
							lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
							{
								if (null == zsslscene.AttackLog)
								{
									return;
								}
								if (!zsslscene.AttackLog.BHInjure.TryGetValue(guid, out tAttackLog))
								{
									return;
								}
								tAttackLog.BHName = bhname;
							}
							if (needBroad)
							{
								int num = zsslscene.AttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, tAttackLog));
								bool top5Chg = num >= 0 && num < 5;
								ZhuanShengShiLian.BroadBossLife(zsslscene, client, top5Chg);
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ZhuanShengShiLian :: 处理清除角色伤害记录异常。", new object[0]), ex, true);
			}
		}

		public static bool IsZhuanShengShiLianCopyScene(int mapCode)
		{
			bool result;
			lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
			{
				result = ZhuanShengShiLian.ZhuanShengRunTimeData.ZhuanShengMapDict.ContainsKey(mapCode);
			}
			return result;
		}

		public static bool JudgeCanEnterOnTime(ZhuanShengMapInfo mapInfo)
		{
			DateTime t = TimeUtil.NowDateTime();
			DateTime t2 = DateTime.Parse(mapInfo.TimePoints[0]);
			DateTime t3 = t2.AddMinutes((double)(mapInfo.ReadyTime / 60));
			return t > t2 && t <= t3;
		}

		public static bool CanFight(GameClient client)
		{
			ZSSLScene zsslscene;
			bool result;
			if (!ZhuanShengShiLian.SceneDict.TryGetValue(client.ClientData.FuBenSeqID, out zsslscene))
			{
				result = false;
			}
			else
			{
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					result = (zsslscene.State == BattleStates.StartFight);
				}
			}
			return result;
		}

		public static void TimerProc()
		{
			if (!GameManager.IsKuaFuServer)
			{
				long num = TimeUtil.NOW();
				lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
				{
					if (Math.Abs(num - ZhuanShengShiLian.LastHeartBeatTicks) < 1000L)
					{
						return;
					}
					ZhuanShengShiLian.LastHeartBeatTicks = num;
				}
				if (157 == ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.ActivityType && ZhuanShengShiLian.ZhuanShengRunTimeData.ThemeZSActivity.InActivityTime())
				{
					foreach (KeyValuePair<int, ZSSLScene> keyValuePair in ZhuanShengShiLian.SceneDict)
					{
						lock (ZhuanShengShiLian.ZhuanShengRunTimeData.Mutex)
						{
							switch (keyValuePair.Value.State)
							{
							case BattleStates.NoBattle:
							{
								DateTime dateTime = DateTime.Parse(keyValuePair.Value.SceneInfo.TimePoints[0]).AddSeconds((double)keyValuePair.Value.SceneInfo.ReadyTime);
								keyValuePair.Value.StartTick = dateTime.Ticks / 10000L;
								keyValuePair.Value.EndTick = dateTime.AddSeconds((double)keyValuePair.Value.SceneInfo.FightSecs).Ticks / 10000L;
								keyValuePair.Value.StatusEndTime = keyValuePair.Value.StartTick;
								ZhuanShengShiLian.BroadMsg(keyValuePair.Value.SceneInfo.MapCode, GLang.GetLang(4010, new object[0]));
								keyValuePair.Value.State = BattleStates.WaitingFight;
								break;
							}
							case BattleStates.WaitingFight:
								if (num >= keyValuePair.Value.StartTick && null != keyValuePair.Value.m_CopyMap)
								{
									GameManager.MonsterZoneMgr.AddDynamicMonsters(keyValuePair.Value.SceneInfo.MapCode, keyValuePair.Value.SceneInfo.MonstersID, keyValuePair.Value.m_CopyMap.FuBenSeqID, 1, keyValuePair.Value.SceneInfo.BornX / 100, keyValuePair.Value.SceneInfo.BornY / 100, 0, 0, 0, null, null);
									keyValuePair.Value.State = BattleStates.StartFight;
									keyValuePair.Value.StatusEndTime = keyValuePair.Value.EndTick;
									ZhuanShengShiLian.SendTimeInfoToAll(keyValuePair.Value, num);
								}
								break;
							case BattleStates.StartFight:
								if (num >= keyValuePair.Value.EndTick)
								{
									keyValuePair.Value.State = BattleStates.EndFight;
									keyValuePair.Value.StatusEndTime = keyValuePair.Value.EndTick;
									keyValuePair.Value.BossDie = false;
									List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(keyValuePair.Value.SceneInfo.MapCode);
									foreach (object obj in objectsByMap)
									{
										if (obj is Monster)
										{
											GameManager.MonsterMgr.DeadMonsterImmediately(obj as Monster);
										}
									}
								}
								break;
							case BattleStates.EndFight:
								try
								{
									List<ShiLianReward> list;
									if (ZhuanShengShiLian.ZhuanShengRunTimeData.ShiLianRewardDict.TryGetValue(keyValuePair.Value.SceneInfo.MapCode, out list))
									{
										List<BHAttackLog> bhAttackLogList = keyValuePair.Value.AttackLog.BHInjure.Values.ToList<BHAttackLog>();
										int i;
										for (i = 0; i < bhAttackLogList.Count; i++)
										{
											if (bhAttackLogList[i].BHInjure > 0L)
											{
												int rank = keyValuePair.Value.AttackLog.BHAttackRank.FindIndex((BHAttackLog x) => object.ReferenceEquals(x, bhAttackLogList[i]));
												rank++;
												ShiLianReward shiLianReward = list.Find((ShiLianReward _x) => _x.MinRank <= rank && (rank <= _x.MaxRank || _x.MaxRank < 0));
												if (null != shiLianReward)
												{
													int num2 = keyValuePair.Value.BossDie ? shiLianReward.WinrewardExp : shiLianReward.LoseRewardExp;
													int num3 = keyValuePair.Value.BossDie ? shiLianReward.WinRewardMoney : shiLianReward.LoseRewardMoney;
													string text = keyValuePair.Value.BossDie ? shiLianReward.WinRewardItem : shiLianReward.LoseRewardItem;
													foreach (KeyValuePair<int, long> keyValuePair2 in bhAttackLogList[i].RoleInjure)
													{
														GameClient gameClient = GameManager.ClientMgr.FindClient(keyValuePair2.Key);
														if (null != gameClient)
														{
															if (gameClient.ClientData.MapCode == keyValuePair.Value.SceneInfo.MapCode)
															{
																GameManager.ClientMgr.ProcessRoleExperience(gameClient, (long)num2, false, true, false, "none");
																GameManager.ClientMgr.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, gameClient, num3, "转生试炼添加绑金", true);
																ZhuanShengShiLian.GiveGoodsAward(gameClient, text);
																gameClient.sendCmd(1908, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
																{
																	keyValuePair.Value.BossDie ? 1 : 0,
																	text,
																	num2,
																	num3,
																	rank
																}), false);
															}
														}
													}
												}
											}
										}
									}
									keyValuePair.Value.AttackLog = null;
								}
								catch (Exception ex)
								{
									DataHelper.WriteExceptionLogEx(ex, "转生试炼调度异常");
								}
								keyValuePair.Value.ClearTick = num + (long)(keyValuePair.Value.SceneInfo.ClearRolesSecs * 1000);
								keyValuePair.Value.StatusEndTime = keyValuePair.Value.ClearTick;
								keyValuePair.Value.State = BattleStates.ClearBattle;
								ZhuanShengShiLian.SendTimeInfoToAll(keyValuePair.Value, num);
								break;
							case BattleStates.ClearBattle:
								if (num >= keyValuePair.Value.ClearTick)
								{
									List<GameClient> clientsList = keyValuePair.Value.m_CopyMap.GetClientsList();
									if (clientsList != null && clientsList.Count > 0)
									{
										for (int j = 0; j < clientsList.Count; j++)
										{
											GameClient gameClient = clientsList[j];
											if (gameClient != null)
											{
												int num4 = GameManager.MainMapCode;
												int maxX = -1;
												int mapY = -1;
												if (gameClient.ClientData.LastMapCode != -1 && gameClient.ClientData.LastPosX != -1 && gameClient.ClientData.LastPosY != -1)
												{
													if (MapTypes.Normal == Global.GetMapType(gameClient.ClientData.LastMapCode))
													{
														num4 = gameClient.ClientData.LastMapCode;
														maxX = gameClient.ClientData.LastPosX;
														mapY = gameClient.ClientData.LastPosY;
													}
												}
												GameMap gameMap = null;
												if (GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
												{
													GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num4, maxX, mapY, -1, 0);
												}
											}
										}
									}
									keyValuePair.Value.State = BattleStates.NoBattle;
								}
								break;
							}
						}
					}
				}
			}
		}

		private static long LastHeartBeatTicks = 0L;

		public static ZhuanShengRunData ZhuanShengRunTimeData = new ZhuanShengRunData();

		public static ConcurrentDictionary<int, ZSSLScene> SceneDict = new ConcurrentDictionary<int, ZSSLScene>();
	}
}
