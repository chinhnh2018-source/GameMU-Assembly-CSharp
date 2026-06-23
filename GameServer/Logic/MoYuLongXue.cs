using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Tools;
using Server.Data;
using Server.Tools;

namespace GameServer.Logic
{
	public class MoYuLongXue
	{
		public static void LoadMoYuXml()
		{
			string text = "";
			try
			{
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					text = Global.GameResPath(ThemeDataConst.ThemeActivityMoYu);
					XElement xelement = CheckHelper.LoadXml(text, true);
					if (null != xelement)
					{
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.FromDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.ToDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.AwardStartDate = "-1";
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.AwardEndDate = "-1";
						Dictionary<int, MoYuMonsterInfo> dictionary = new Dictionary<int, MoYuMonsterInfo>();
						List<int> list = new List<int>();
						IEnumerable<XElement> enumerable = xelement.Elements();
						foreach (XElement xml in enumerable)
						{
							int num = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MonstersID", "0"));
							int num2 = Convert.ToInt32(Global.GetDefAttributeStr(xml, "MapId", "0"));
							dictionary[num] = new MoYuMonsterInfo
							{
								MonstersID = num,
								NpcID = Convert.ToInt32(Global.GetDefAttributeStr(xml, "NpcID", "0")),
								MapCode = num2,
								Chengjiu = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Chengjiu", "0")),
								Shengwang = Convert.ToInt32(Global.GetDefAttributeStr(xml, "Shengwang", "0")),
								HurtMin = Convert.ToInt32(Global.GetDefAttributeStr(xml, "HurtMin", "0"))
							};
							if (!list.Contains(num2))
							{
								list.Add(num2);
							}
						}
						List<int> broadGoodsIDList = new List<int>();
						string paramValueByName = GameManager.systemParamsList.GetParamValueByName("ThemeActivityMoYuGoods");
						if (!string.IsNullOrEmpty(paramValueByName))
						{
							broadGoodsIDList = Array.ConvertAll<string, int>(paramValueByName.Split(new char[]
							{
								','
							}), (string _x) => Convert.ToInt32(_x)).ToList<int>();
						}
						MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.Clear();
						MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList.Clear();
						MoYuLongXue.MoYuRunTimeData.MapCodeList.Clear();
						MoYuLongXue.MoYuRunTimeData.MonsterXmlDict = dictionary;
						MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList = broadGoodsIDList;
						MoYuLongXue.MoYuRunTimeData.MapCodeList = list;
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.ActivityType = 156;
						MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.PredealDateTime();
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, string.Format("加载xml配置文件:{0}, 失败。", text), ex, true);
			}
		}

		public static bool InActivityTime()
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				if (null == MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity)
				{
					result = false;
				}
				else
				{
					result = MoYuLongXue.MoYuRunTimeData.ThemeMoYuActivity.InActivityTime();
				}
			}
			return result;
		}

		public static bool InMoYuMap(int mapCode)
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				result = MoYuLongXue.MoYuRunTimeData.MapCodeList.Contains(mapCode);
			}
			return result;
		}

		public static bool IsBHGoods(int goodsID)
		{
			bool result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				result = MoYuLongXue.MoYuRunTimeData.BroadGoodsIDList.Contains(goodsID);
			}
			return result;
		}

		public static void OnBangHuiDestroy(int faction)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				foreach (BossAttackLog bossAttackLog in MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Values)
				{
					if (null != bossAttackLog.BHInjure)
					{
						BHAttackLog bhattackLog = null;
						if (bossAttackLog.BHInjure.TryGetValue((long)faction, out bhattackLog))
						{
							bhattackLog.RoleInjure.Clear();
						}
					}
				}
			}
		}

		public static void OnClientLeaveBangHui(int faction, int rid)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				foreach (BossAttackLog bossAttackLog in MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Values)
				{
					if (null != bossAttackLog.BHInjure)
					{
						BHAttackLog bhattackLog = null;
						if (bossAttackLog.BHInjure.TryGetValue((long)faction, out bhattackLog))
						{
							bhattackLog.RoleInjure.Remove(rid);
						}
					}
				}
			}
		}

		public static int KillerRid(Monster monster)
		{
			int result = 0;
			long num = 0L;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				BossAttackLog bossAttackLog;
				if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out bossAttackLog))
				{
					return 0;
				}
				if (bossAttackLog.BHAttackRank == null || bossAttackLog.BHAttackRank.Count < 1)
				{
					return 0;
				}
				BHAttackLog bhattackLog = bossAttackLog.BHAttackRank[0];
				if (null == bhattackLog.RoleInjure)
				{
					return 0;
				}
				foreach (KeyValuePair<int, long> keyValuePair in bhattackLog.RoleInjure)
				{
					if (keyValuePair.Value > num)
					{
						result = keyValuePair.Key;
						num = keyValuePair.Value;
					}
				}
			}
			return result;
		}

		public static int GetBossLeftCount()
		{
			int result;
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				if (null == MoYuLongXue.MoYuRunTimeData.BossAttackLogDict)
				{
					result = 0;
				}
				else
				{
					result = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
				}
			}
			return result;
		}

		public static void ProcessAddMonster(Monster monster)
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if ((dateTime - MoYuLongXue.MoYuRunTimeData.LastBirthTimePoint).TotalMinutes > 1.0)
				{
					Global.BroadcastRoleActionMsg(null, RoleActionsMsgTypes.HintMsg, string.Format(GLang.GetLang(4004, new object[0]), monster.MonsterInfo.VSName), true, GameInfoTypeIndexes.Hot, ShowGameInfoTypes.HintAndBox, 0, 0, 100, 100);
					MoYuLongXue.MoYuRunTimeData.LastBirthTimePoint = dateTime;
				}
				NPC npc = null;
				int mapCode = 0;
				int cmdData = 0;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					MoYuMonsterInfo moYuMonsterInfo;
					if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.TryGetValue(monster.MonsterInfo.ExtensionID, out moYuMonsterInfo))
					{
						return;
					}
					mapCode = moYuMonsterInfo.MapCode;
					MoYuLongXue.MoYuRunTimeData.BossAttackLogDict[monster.RoleID] = new BossAttackLog
					{
						InjureSum = 0L,
						BHInjure = new Dictionary<long, BHAttackLog>(),
						BHAttackRank = new List<BHAttackLog>()
					};
					cmdData = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
					npc = NPCGeneralManager.FindNPC(moYuMonsterInfo.MapCode, moYuMonsterInfo.NpcID);
				}
				if (null != npc)
				{
					npc.ShowNpc = false;
					GameManager.ClientMgr.NotifyMySelfDelNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				}
				List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(mapCode);
				foreach (GameClient gameClient in mapGameClients)
				{
					gameClient.sendCmd<int>(1907, cmdData, false);
				}
				MoYuLongXue.NotifyBossLogBy9Grid(monster);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("MoYuLongXue :: 处理场景刷新怪物异常。", new object[0]), ex, true);
			}
		}

		public static void ProcessAttack(GameClient client, Monster monster, int injure)
		{
			try
			{
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.ContainsKey(monster.MonsterInfo.ExtensionID))
					{
						return;
					}
					BossAttackLog bossAttackLog;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out bossAttackLog))
					{
						bossAttackLog = new BossAttackLog
						{
							InjureSum = 0L,
							BHInjure = new Dictionary<long, BHAttackLog>(),
							BHAttackRank = new List<BHAttackLog>()
						};
						MoYuLongXue.MoYuRunTimeData.BossAttackLogDict[monster.RoleID] = bossAttackLog;
					}
					if (client.ClientData.Faction > 0)
					{
						BHAttackLog bhattackLog;
						if (!bossAttackLog.BHInjure.TryGetValue((long)client.ClientData.Faction, out bhattackLog))
						{
							bhattackLog = new BHAttackLog
							{
								BHID = client.ClientData.Faction,
								BHName = client.ClientData.BHName,
								BHInjure = 0L,
								RoleInjure = new Dictionary<int, long>()
							};
							bossAttackLog.BHInjure[(long)client.ClientData.Faction] = bhattackLog;
							bossAttackLog.BHAttackRank.Add(bhattackLog);
						}
						if (!bhattackLog.RoleInjure.ContainsKey(client.ClientData.RoleID))
						{
							bhattackLog.RoleInjure[client.ClientData.RoleID] = 0L;
						}
						Dictionary<int, long> roleInjure;
						int roleID;
						(roleInjure = bhattackLog.RoleInjure)[roleID = client.ClientData.RoleID] = roleInjure[roleID] + (long)injure;
						bhattackLog.BHInjure += (long)injure;
						bossAttackLog.BHAttackRank.Sort(delegate(BHAttackLog x, BHAttackLog y)
						{
							int result;
							if (x.BHInjure > y.BHInjure)
							{
								result = -1;
							}
							else if (x.BHInjure < y.BHInjure)
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
					bossAttackLog.InjureSum += (long)injure;
				}
				MoYuLongXue.NotifyBossLogBy9Grid(monster);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("MoYuLongXue :: 处理攻击boss异常。", new object[0]), ex, true);
			}
		}

		public static bool ProcessMonsterDie(Monster monster)
		{
			try
			{
				NPC npc = null;
				bool result = true;
				int mapCode = 0;
				int cmdData = 0;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					BossAttackLog bossAttackLog;
					MoYuMonsterInfo moYuMonsterInfo;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monster.RoleID, out bossAttackLog))
					{
						result = false;
					}
					else if (!MoYuLongXue.MoYuRunTimeData.MonsterXmlDict.TryGetValue(monster.MonsterInfo.ExtensionID, out moYuMonsterInfo))
					{
						result = false;
					}
					else
					{
						mapCode = moYuMonsterInfo.MapCode;
						List<BHAttackLog> bhattackRank = bossAttackLog.BHAttackRank;
						int num = Global.GMin(bhattackRank.Count, 5);
						for (int i = 0; i < num; i++)
						{
							foreach (KeyValuePair<int, long> keyValuePair in bhattackRank[i].RoleInjure)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(keyValuePair.Key);
								if (null != gameClient)
								{
									if (gameClient.ClientData.MapCode == moYuMonsterInfo.MapCode)
									{
										if (keyValuePair.Value >= (long)moYuMonsterInfo.HurtMin)
										{
											GameManager.ClientMgr.ModifyChengJiuPointsValue(gameClient, moYuMonsterInfo.Chengjiu, "魔域龙穴boss奖励", true, true);
											GameManager.ClientMgr.ModifyShengWangValue(gameClient, moYuMonsterInfo.Shengwang, "魔域龙穴boss奖励", true, true);
										}
									}
								}
							}
						}
						npc = NPCGeneralManager.FindNPC(moYuMonsterInfo.MapCode, moYuMonsterInfo.NpcID);
					}
					MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Remove(monster.RoleID);
					cmdData = MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.Count;
				}
				List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(mapCode);
				foreach (GameClient gameClient in mapGameClients)
				{
					GameClient gameClient;
					gameClient.sendCmd<int>(1907, cmdData, false);
				}
				if (null != npc)
				{
					npc.ShowNpc = true;
					GameManager.ClientMgr.NotifyMySelfNewNPCBy9Grid(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, npc);
				}
				return result;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("MoYuLongXue :: 处理boss被击杀异常。", new object[0]), ex, true);
			}
			return false;
		}

		public static BossLifeLog GetBossAttackLog(int factionID, int monsterID)
		{
			try
			{
				BossLifeLog bossLifeLog;
				lock (MoYuLongXue.MoYuRunTimeData.Mutex)
				{
					BossAttackLog bossAttackLog;
					if (!MoYuLongXue.MoYuRunTimeData.BossAttackLogDict.TryGetValue(monsterID, out bossAttackLog))
					{
						return null;
					}
					int count = Global.GMin(bossAttackLog.BHAttackRank.Count, 5);
					bossLifeLog = new BossLifeLog
					{
						InjureSum = bossAttackLog.InjureSum,
						BHAttackRank = bossAttackLog.BHAttackRank.GetRange(0, count)
					};
					bossAttackLog.BHInjure.TryGetValue((long)factionID, out bossLifeLog.SelfBHAttack);
				}
				return bossLifeLog;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("MoYuLongXue :: 处理获取boss攻打记录异常。", new object[0]), ex, true);
			}
			return null;
		}

		public static void NotifyBossLogBy9Grid(Monster monster)
		{
			lock (MoYuLongXue.MoYuRunTimeData.Mutex)
			{
				List<object> all9GridObjects = Global.GetAll9GridObjects(monster);
				for (int i = 0; i < all9GridObjects.Count; i++)
				{
					GameClient gameClient = all9GridObjects[i] as GameClient;
					if (null != gameClient)
					{
						BossLifeLog bossAttackLog = MoYuLongXue.GetBossAttackLog(gameClient.ClientData.Faction, monster.RoleID);
						if (null != bossAttackLog)
						{
							(all9GridObjects[i] as GameClient).sendCmd<BossLifeLog>(1906, bossAttackLog, false);
						}
					}
				}
			}
		}

		public static MoYuRunData MoYuRunTimeData = new MoYuRunData();
	}
}
