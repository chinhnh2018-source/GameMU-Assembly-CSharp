using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using AutoCSer.Net.TcpInternalServer;
using AutoCSer.Net.TcpStaticServer;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using Remoting;
using Server.Data;
using Server.Tools;

namespace KF.TcpCall
{
	[Server(Name = "KfCall", IsServer = true, IsAttribute = true, IsClientAwaiter = false, MemberFilters = 240, IsSegmentation = true, ClientSegmentationCopyPath = "GameServer\\Remoting\\")]
	public static class EscapeBattle_K
	{
		public static bool InitConfig()
		{
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.Initialize = false;
				bool flag2 = EscapeBattle_K._Config.Load(KuaFuServerManager.GetResourcePath("Config\\EscapeActivityRules.xml", KuaFuServerManager.ResourcePathTypes.GameRes));
				EscapeBattleMatchConfig escapeBattleMatchConfig = EscapeBattle_K._Config.MatchConfigList[0];
				EscapeBattleConsts.MinZhanDuiNumPerGame = escapeBattleMatchConfig.MatchTeamNum;
				EscapeBattleConsts.MinRoleNumPerGame = escapeBattleMatchConfig.EnterBattleNum;
				EscapeBattleConsts.BattleSignSecs = escapeBattleMatchConfig.BattleSignSecs;
				DateTime.TryParse(KuaFuServerManager.systemParamsList.GetParamValueByName("EscapeStartTime"), out EscapeBattleConsts.EscapeStartTime);
				if (!flag2)
				{
					LogManager.WriteLog(2, "EscapeBattle_K.InitConfig failed!", null, true);
				}
				EscapeBattle_K.Initialize = flag2;
			}
			return true;
		}

		public static void Update()
		{
			if (EscapeBattle_K.Initialize)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				TimeSpan timeSpan = TimeUtil.TimeOfWeek(dateTime);
				bool flag = false;
				lock (EscapeBattle_K.Mutex)
				{
					List<TimeSpan> timePoints = EscapeBattle_K._Config.MatchConfigList[0].TimePoints;
					for (int i = 0; i < timePoints.Count - 1; i += 2)
					{
						if (timePoints[i] <= timeSpan && timeSpan < timePoints[i + 1])
						{
							flag = true;
							break;
						}
					}
					if (EscapeBattle_K.SyncData.State != flag)
					{
						EscapeBattle_K.SyncData.State = flag;
					}
					int num = (int)dateTime.TimeOfDay.TotalSeconds;
					if (num / EscapeBattleConsts.BattleSignSecs != EscapeBattle_K.LastMatchMinute / EscapeBattleConsts.BattleSignSecs)
					{
						EscapeBattle_K.LastMatchMinute = num;
						EscapeBattle_K.PrepareMatchList();
					}
					EscapeBattle_K.PrepareGameFuBen(dateTime);
					if (flag)
					{
						if (!EscapeBattle_K.NeedUpdateRank)
						{
							EscapeBattle_K.NeedUpdateRank = true;
						}
					}
					else if ((EscapeBattle_K.NeedUpdateRank && EscapeBattle_K.ThisLoopPkLogs.Count == 0) || EscapeBattle_K.lastUpdateTime.Day != dateTime.Day)
					{
						EscapeBattle_K.NeedUpdateRank = false;
						EscapeBattle_K.LoadSyncData(dateTime, true);
					}
					EscapeBattle_K.ClearTimeOverGameFuBen(dateTime);
				}
				KFCallMsg[] array = null;
				lock (EscapeBattle_K.Mutex)
				{
					array = EscapeBattle_K.AsyncEvQ.ToArray();
					EscapeBattle_K.AsyncEvQ.Clear();
				}
				foreach (KFCallMsg msg in array)
				{
					ClientAgentManager.Instance().BroadCastMsg(msg, 0);
				}
				EscapeBattle_K.lastUpdateTime = dateTime;
			}
		}

		private static bool PrepareMatchList()
		{
			EscapeBattle_K.JoinList.Clear();
			foreach (EscapeBattle_K.JoinPkData joinPkData in EscapeBattle_K.JoinDict.Values.ToList<EscapeBattle_K.JoinPkData>())
			{
				EscapeBattle_K.JoinPkData joinPkData;
				if (joinPkData.ReadyState && joinPkData.ReadyNum >= EscapeBattleConsts.MinRoleNumPerGame && !joinPkData.InGame)
				{
					EscapeBattle_K.JoinList.Add(new EscapeBattle_K.JoinPkData
					{
						ZhanDuiID = joinPkData.ZhanDuiID,
						ZoneId = joinPkData.ZoneId,
						ZhanDuiName = joinPkData.ZhanDuiName,
						ReadyNum = joinPkData.ReadyNum,
						DuanWeiJiFen = joinPkData.DuanWeiJiFen
					});
				}
			}
			if (EscapeBattle_K.JoinList.Count >= EscapeBattleConsts.MinZhanDuiNumPerGame)
			{
				EscapeBattleMatchConfig escapeBattleMatchConfig = EscapeBattle_K._Config.MatchConfigList[0];
				EscapeBattle_K.JoinList.Sort((EscapeBattle_K.JoinPkData x, EscapeBattle_K.JoinPkData y) => x.DuanWeiJiFen - y.DuanWeiJiFen);
				List<int> notMatchList = new List<int>();
				int num = EscapeBattle_K.JoinList.Count % escapeBattleMatchConfig.MatchTeamNum;
				if (num != 0)
				{
					int num2 = 0;
					int randomNumber = Global.GetRandomNumber(0, EscapeBattle_K.JoinList.Count);
					for (int i = randomNumber; i < EscapeBattle_K.JoinList.Count + randomNumber; i++)
					{
						EscapeBattle_K.JoinPkData joinPkData = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
						if (EscapeBattle_K.LastMatchList.Contains(joinPkData.ZhanDuiID) && !notMatchList.Contains(joinPkData.ZhanDuiID))
						{
							notMatchList.Add(joinPkData.ZhanDuiID);
							num2++;
							if (num2 >= num)
							{
								break;
							}
						}
					}
					if (num2 < num)
					{
						for (int i = randomNumber; i < EscapeBattle_K.JoinList.Count + randomNumber; i++)
						{
							EscapeBattle_K.JoinPkData joinPkData = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
							if (!EscapeBattle_K.NotMatchList.Contains(joinPkData.ZhanDuiID) && !notMatchList.Contains(joinPkData.ZhanDuiID))
							{
								notMatchList.Add(joinPkData.ZhanDuiID);
								num2++;
								if (num2 >= num)
								{
									break;
								}
							}
						}
					}
					if (num2 < num)
					{
						for (int i = randomNumber; i < EscapeBattle_K.JoinList.Count + randomNumber; i++)
						{
							EscapeBattle_K.JoinPkData joinPkData = EscapeBattle_K.JoinList[i % EscapeBattle_K.JoinList.Count];
							if (!notMatchList.Contains(joinPkData.ZhanDuiID))
							{
								notMatchList.Add(joinPkData.ZhanDuiID);
								num2++;
								if (num2 >= num)
								{
									break;
								}
							}
						}
					}
					EscapeBattle_K.JoinList.RemoveAll((EscapeBattle_K.JoinPkData x) => notMatchList.Contains(x.ZhanDuiID));
				}
				EscapeBattle_K.NotMatchList = notMatchList;
				EscapeBattle_K.LastMatchList = EscapeBattle_K.JoinList.ConvertAll<int>((EscapeBattle_K.JoinPkData x) => x.ZhanDuiID);
				EscapeBattle_K.JoinList = ListExt.RandomSortList<EscapeBattle_K.JoinPkData>(EscapeBattle_K.JoinList);
			}
			return true;
		}

		private static void PrepareGameFuBen(DateTime now)
		{
			for (int i = 0; i < EscapeBattle_K.JoinList.Count - (EscapeBattleConsts.MinZhanDuiNumPerGame - 1); i += EscapeBattleConsts.MinZhanDuiNumPerGame)
			{
				if (EscapeBattleConsts.MinZhanDuiNumPerGame == 3)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i],
						EscapeBattle_K.JoinList[i + 1],
						EscapeBattle_K.JoinList[i + 2]
					});
				}
				else if (EscapeBattleConsts.MinZhanDuiNumPerGame == 2)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i],
						EscapeBattle_K.JoinList[i + 1]
					});
				}
				else if (EscapeBattleConsts.MinZhanDuiNumPerGame == 1)
				{
					EscapeBattle_K.CreateGameFuBen(now, new EscapeBattle_K.JoinPkData[]
					{
						EscapeBattle_K.JoinList[i]
					});
				}
			}
		}

		private static bool CreateGameFuBen(DateTime now, params EscapeBattle_K.JoinPkData[] joinArr)
		{
			for (int i = 1; i <= joinArr.Length; i++)
			{
				joinArr[i - 1].Side = i;
				if (joinArr[i - 1].InGame)
				{
					return true;
				}
			}
			int num = 0;
			int nextGameId = TianTiPersistence.Instance.GetNextGameId();
			if (ClientAgentManager.Instance().AssginKfFuben(EscapeBattle_K.GameType, (long)nextGameId, 10, out num))
			{
				EscapeBattleFuBenData escapeBattleFuBenData = new EscapeBattleFuBenData();
				escapeBattleFuBenData.GameID = (long)nextGameId;
				escapeBattleFuBenData.ServerID = num;
				EscapeBattleNtfEnterData escapeBattleNtfEnterData = new EscapeBattleNtfEnterData();
				escapeBattleNtfEnterData.ToServerId = num;
				escapeBattleNtfEnterData.GameId = nextGameId;
				EscapeBattlePkLogData escapeBattlePkLogData = new EscapeBattlePkLogData();
				escapeBattlePkLogData.Season = EscapeBattle_K.SyncData.Season;
				escapeBattlePkLogData.StartTime = now;
				escapeBattlePkLogData.GameID = nextGameId;
				escapeBattlePkLogData.ToServerID = num;
				EscapeBattleMatchConfig escapeBattleMatchConfig = EscapeBattle_K._Config.MatchConfigList[0];
				escapeBattlePkLogData.EndTime = now.AddSeconds((double)escapeBattleMatchConfig.TotalSecs);
				foreach (EscapeBattle_K.JoinPkData joinPkData in joinArr)
				{
					escapeBattleFuBenData.SideDict[(long)joinPkData.ZhanDuiID] = joinPkData.Side;
					escapeBattleFuBenData.RoleDict.AddRange(TianTi5v5Service.GetZhanDuiMemberIDs(joinPkData.ZhanDuiID));
					joinPkData.ToServerID = num;
					joinPkData.CurrGameID = nextGameId;
					joinPkData.CopyData = escapeBattleFuBenData;
					joinPkData.InGame = true;
					escapeBattleNtfEnterData.ZhanDuiIDList.Add(joinPkData.ZhanDuiID);
					escapeBattlePkLogData.ZhanDuiIDs.Add(joinPkData.ZhanDuiID);
					escapeBattlePkLogData.ZoneIDs.Add(joinPkData.ZoneId);
					escapeBattlePkLogData.ZhanDuiNames.Add(joinPkData.ZhanDuiName);
					EscapeBattle_K.JoinPkData joinPkData2;
					if (EscapeBattle_K.JoinDict.TryGetValue(joinPkData.ZhanDuiID, out joinPkData2))
					{
						joinPkData2.InGame = true;
						joinPkData2.CurrGameID = nextGameId;
						joinPkData2.Side = joinPkData.Side;
						joinPkData2.ToServerID = num;
						joinPkData2.CopyData = escapeBattleFuBenData;
						joinPkData2.State = 3;
						joinPkData2.ReadyState = false;
						joinPkData2.ReadyNum = 0;
					}
				}
				EscapeBattle_K.AsyncEvQ.Enqueue(KFCallMsg.New<EscapeBattleNtfEnterData>(10037, escapeBattleNtfEnterData));
				EscapeBattle_K.ThisLoopPkLogs[nextGameId] = escapeBattlePkLogData;
				if (!EscapeBattle_K.NeedUpdateRank)
				{
					EscapeBattle_K.NeedUpdateRank = true;
				}
				LogManager.WriteLog(1002, string.Format("大逃杀第{0}赛季战队成员通知入场,GameID={1},zhanduiIDs={2}", EscapeBattle_K.SyncData.Season, escapeBattleNtfEnterData.GameId, string.Join<int>(",", escapeBattlePkLogData.ZhanDuiIDs)), null, true);
				return true;
			}
			LogManager.WriteLog(1, string.Format("大逃杀第{0}赛季分配游戏服务器失败", EscapeBattle_K.SyncData.Season), null, true);
			return false;
		}

		private static void ClearTimeOverGameFuBen(DateTime now)
		{
			List<int> list = new List<int>();
			foreach (int num in EscapeBattle_K.ThisLoopPkLogs.Keys.ToList<int>())
			{
				EscapeBattlePkLogData escapeBattlePkLogData;
				if (EscapeBattle_K.ThisLoopPkLogs.TryGetValue(num, out escapeBattlePkLogData))
				{
					if (escapeBattlePkLogData.EndTime < now || escapeBattlePkLogData.State >= 7)
					{
						List<int> list2 = new List<int>();
						foreach (int item in escapeBattlePkLogData.ZhanDuiIDs)
						{
							list2.Add(item);
							list2.Add(int.MinValue);
						}
						try
						{
							EscapeBattle_K.GameResult(num, list2);
						}
						catch (Exception ex)
						{
							LogManager.WriteException(ex.ToString());
							list.Add(escapeBattlePkLogData.GameID);
						}
					}
				}
			}
			foreach (int key in list)
			{
				EscapeBattlePkLogData escapeBattlePkLogData;
				if (EscapeBattle_K.ThisLoopPkLogs.TryGetValue(key, out escapeBattlePkLogData))
				{
					EscapeBattle_K.ThisLoopPkLogs.Remove(key);
					ClientAgentManager.Instance().RemoveKfFuben(EscapeBattle_K.GameType, escapeBattlePkLogData.ToServerID, (long)escapeBattlePkLogData.GameID);
				}
			}
		}

		public static void LoadSyncData(DateTime now, bool rebuild = false)
		{
			lock (EscapeBattle_K.Mutex)
			{
				List<KFEscapeRankInfo> rankList = new List<KFEscapeRankInfo>();
				List<KFEscapeRankInfo> seasonRankList = new List<KFEscapeRankInfo>();
				if (KuaFuServerManager.IsGongNengOpened(115) && EscapeBattleConsts.CheckOpenState(now))
				{
					int weekStartDayIdNow = TimeUtil.GetWeekStartDayIdNow();
					int offsetDayNow = TimeUtil.GetOffsetDayNow();
					int num = EscapeBattle_K.Persistence.GetAsyncInt(14, weekStartDayIdNow);
					num = MathEx.Pack(offsetDayNow, num, EscapeBattleConsts.DaysPerSeason);
					int season = num - EscapeBattleConsts.DaysPerSeason;
					DateTime realDate = TimeUtil.GetRealDate(num);
					if (EscapeBattle_K.Persistence.BuildEscapeRankList(num, realDate))
					{
					}
					EscapeBattle_K.SyncData.Season = num;
					if (offsetDayNow == num)
					{
						EscapeBattle_K.Persistence.SetAsyncInt(14, EscapeBattle_K.SyncData.Season);
					}
					rankList = EscapeBattle_K.Persistence.LoadEscapeRankData(num);
					seasonRankList = EscapeBattle_K.Persistence.LoadEscapeRankData(season);
				}
				EscapeBattle_K.SyncData.RankList = rankList;
				EscapeBattle_K.SyncData.SeasonRankList = seasonRankList;
				EscapeBattle_K.SyncData.RankModTime = now;
			}
		}

		private static void ZhanDuiChangeState(EscapeBattle_K.JoinPkData pkData, int state)
		{
			int num = 0;
			if (state >= 5)
			{
				pkData.InGame = false;
				num = 0;
			}
			pkData.State = num;
			int serverIDFromZoneID = KuaFuServerManager.GetServerIDFromZoneID(pkData.ZoneId);
			ClientAgentManager.Instance().SendMsg(serverIDFromZoneID, KFCallMsg.New<int[]>(10038, new int[]
			{
				pkData.ZhanDuiID,
				pkData.CurrGameID,
				num
			}));
		}

		[Method]
		public static EscapeBattleSyncData SyncZhengBaData(EscapeBattleSyncData lastSyncData)
		{
			EscapeBattleSyncData escapeBattleSyncData = new EscapeBattleSyncData();
			lock (EscapeBattle_K.Mutex)
			{
				escapeBattleSyncData.Season = EscapeBattle_K.SyncData.Season;
				escapeBattleSyncData.State = EscapeBattle_K.SyncData.State;
				escapeBattleSyncData.CenterTime = TimeUtil.NowDateTime();
				escapeBattleSyncData.RankModTime = lastSyncData.RankModTime;
				if (EscapeBattle_K.SyncData.RankModTime != escapeBattleSyncData.RankModTime && EscapeBattle_K.SyncData.RankList != null)
				{
					escapeBattleSyncData.RankModTime = EscapeBattle_K.SyncData.RankModTime;
					escapeBattleSyncData.RankList = new List<KFEscapeRankInfo>(EscapeBattle_K.SyncData.RankList);
					escapeBattleSyncData.SeasonRankList = new List<KFEscapeRankInfo>(EscapeBattle_K.SyncData.SeasonRankList);
				}
			}
			return escapeBattleSyncData;
		}

		[Method]
		public static int GetZhanDuiState(int zhanDuiID)
		{
			int result = 0;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData joinPkData;
				if (EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinPkData))
				{
					if (joinPkData.State == 3 && !joinPkData.InGame)
					{
						return 0;
					}
					return joinPkData.State;
				}
			}
			return result;
		}

		[Method]
		public static int ZhanDuiJoin(int zhanDuiID, int jiFen, int readyNum)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			int result;
			if (!EscapeBattleConsts.CheckOpenState(dateTime))
			{
				result = -11004;
			}
			else
			{
				TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(zhanDuiID);
				if (null == zhanDuiData)
				{
					result = -4031;
				}
				else
				{
					lock (EscapeBattle_K.Mutex)
					{
						EscapeBattle_K.JoinPkData joinPkData;
						if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinPkData))
						{
							joinPkData = new EscapeBattle_K.JoinPkData
							{
								ZhanDuiID = zhanDuiID
							};
							EscapeBattle_K.JoinDict[zhanDuiID] = joinPkData;
						}
						joinPkData.DuanWeiJiFen = zhanDuiData.EscapeJiFen;
						joinPkData.ZhanDuiName = zhanDuiData.ZhanDuiName;
						joinPkData.ZoneId = zhanDuiData.ZoneID;
						joinPkData.ReadyNum = readyNum;
						if (readyNum > 0)
						{
							joinPkData.ReadyState = true;
							joinPkData.State = 2;
						}
						else if (readyNum == 0)
						{
							joinPkData.ReadyState = false;
						}
					}
					result = 0;
				}
			}
			return result;
		}

		[Method]
		public static int ZhengBaKuaFuLogin(ServerSocketSender socket, int zhanDuiID, int gameId, int srcServerID, out EscapeBattleFuBenData copyData)
		{
			copyData = null;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData joinPkData;
				if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinPkData) || joinPkData.CurrGameID == 0)
				{
					return -4006;
				}
				EscapeBattlePkLogData escapeBattlePkLogData = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(joinPkData.CurrGameID, out escapeBattlePkLogData))
				{
					return -4006;
				}
				copyData = joinPkData.CopyData;
			}
			KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(srcServerID);
			int result;
			if (null != kuaFuServerInfo)
			{
				copyData.IPs = new string[]
				{
					kuaFuServerInfo.DbIp,
					kuaFuServerInfo.DbIp
				};
				copyData.Ports = new int[]
				{
					kuaFuServerInfo.DbPort,
					kuaFuServerInfo.LogDbPort
				};
				result = 0;
			}
			else
			{
				result = -11000;
			}
			return result;
		}

		[Method]
		public static int ZhengBaRequestEnter(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
		{
			gameId = 0;
			kuaFuServerID = 0;
			ips = null;
			ports = null;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattle_K.JoinPkData joinPkData;
				if (!EscapeBattle_K.JoinDict.TryGetValue(zhanDuiID, out joinPkData) || joinPkData.CurrGameID == 0)
				{
					return -4006;
				}
				EscapeBattlePkLogData escapeBattlePkLogData = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(joinPkData.CurrGameID, out escapeBattlePkLogData))
				{
					return -4006;
				}
				if (escapeBattlePkLogData.State >= 3)
				{
					return -2008;
				}
				gameId = joinPkData.CurrGameID;
				kuaFuServerID = joinPkData.ToServerID;
			}
			KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(kuaFuServerID);
			int result;
			if (null != kuaFuServerInfo)
			{
				ips = new string[]
				{
					kuaFuServerInfo.Ip
				};
				ports = new int[]
				{
					kuaFuServerInfo.Port
				};
				result = 0;
			}
			else
			{
				result = -11001;
			}
			return result;
		}

		[Method]
		public static int GameState(int gameId, int state)
		{
			int result;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattlePkLogData escapeBattlePkLogData = null;
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameId, out escapeBattlePkLogData))
				{
					result = -11003;
				}
				else
				{
					escapeBattlePkLogData.State = state;
					foreach (int key in escapeBattlePkLogData.ZhanDuiIDs)
					{
						EscapeBattle_K.JoinPkData pkData;
						if (EscapeBattle_K.JoinDict.TryGetValue(key, out pkData))
						{
							EscapeBattle_K.ZhanDuiChangeState(pkData, state);
						}
					}
					result = 0;
				}
			}
			return result;
		}

		[Method]
		public static int GameResult(int gameId, List<int> zhanDuiScoreList)
		{
			int result;
			lock (EscapeBattle_K.Mutex)
			{
				EscapeBattlePkLogData escapeBattlePkLogData = null;
				LogManager.WriteLog(1002, string.Format("EscapeBattle::GameResult,gameid={0},scoreList={1}", gameId, string.Join<int>("_", zhanDuiScoreList)), null, true);
				if (!EscapeBattle_K.ThisLoopPkLogs.TryGetValue(gameId, out escapeBattlePkLogData))
				{
					result = 3;
				}
				else
				{
					DateTime fightTime = TimeUtil.NowDateTime();
					for (int i = 0; i < zhanDuiScoreList.Count - 1; i += 2)
					{
						int num = zhanDuiScoreList[i];
						int num2 = zhanDuiScoreList[i + 1];
						if (escapeBattlePkLogData.ZhanDuiIDs.Contains(num))
						{
							EscapeBattle_K.JoinPkData joinPkData;
							if (EscapeBattle_K.JoinDict.TryGetValue(num, out joinPkData))
							{
								if (joinPkData.InGame)
								{
									joinPkData.InGame = false;
									joinPkData.CurrGameID = 0;
									joinPkData.DuanWeiJiFen = TianTi5v5Service.UpdateEscapeZhanDui(joinPkData.ZhanDuiID, num2, fightTime);
								}
							}
							LogManager.WriteLog(1002, string.Format("EscapeBattle::GameResult,gameid={0},zhanduiid={1},score={2}", gameId, num, num2), null, true);
							EscapeBattle_K.ZhanDuiChangeState(joinPkData, 5);
						}
					}
					bool flag2 = true;
					foreach (int num in escapeBattlePkLogData.ZhanDuiIDs)
					{
						int num;
						EscapeBattle_K.JoinPkData joinPkData;
						if (EscapeBattle_K.JoinDict.TryGetValue(num, out joinPkData) && joinPkData.InGame)
						{
							flag2 = false;
						}
					}
					if (flag2)
					{
						EscapeBattle_K.ThisLoopPkLogs.Remove(gameId);
						ClientAgentManager.Instance().RemoveKfFuben(EscapeBattle_K.GameType, escapeBattlePkLogData.ToServerID, (long)escapeBattlePkLogData.GameID);
					}
					result = 0;
				}
			}
			return result;
		}

		private static object Mutex = new object();

		private static bool Initialize = false;

		private static GameTypes GameType = 37;

		private static EscapeBattleSyncData SyncData = new EscapeBattleSyncData();

		private static DateTime lastUpdateTime = TimeUtil.NowDateTime();

		private static Queue<KFCallMsg> AsyncEvQ = new Queue<KFCallMsg>();

		private static EscapeBattleConfig _Config = new EscapeBattleConfig();

		private static Dictionary<int, EscapeBattleZhanDuiData> ZhanDuiDataDict = new Dictionary<int, EscapeBattleZhanDuiData>();

		private static Dictionary<int, EscapeBattlePkLogData> ThisLoopPkLogs = new Dictionary<int, EscapeBattlePkLogData>();

		private static Dictionary<int, EscapeBattle_K.JoinPkData> JoinDict = new Dictionary<int, EscapeBattle_K.JoinPkData>();

		private static List<EscapeBattle_K.JoinPkData> JoinList = new List<EscapeBattle_K.JoinPkData>();

		private static List<int> LastMatchList = new List<int>();

		private static List<int> NotMatchList = new List<int>();

		private static int LastMatchMinute;

		private static bool NeedUpdateRank = false;

		private static TianTiPersistence Persistence = TianTiPersistence.Instance;

		private class JoinPkData
		{
			public int ZhanDuiID;

			public int ZoneId;

			public string ZhanDuiName;

			public int ReadyNum;

			public int DuanWeiJiFen;

			public int ToServerID;

			public int CurrGameID;

			public EscapeBattleFuBenData CopyData;

			public int Side;

			public bool InGame;

			public bool ReadyState;

			public int State;
		}

		internal static class TcpStaticServer
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M3(int gameId, List<int> zhanDuiScoreList)
			{
				return EscapeBattle_K.GameResult(gameId, zhanDuiScoreList);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M21(int gameId, int state)
			{
				return EscapeBattle_K.GameState(gameId, state);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M22(int zhanDuiID)
			{
				return EscapeBattle_K.GetZhanDuiState(zhanDuiID);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static EscapeBattleSyncData _M1(EscapeBattleSyncData lastSyncData)
			{
				return EscapeBattle_K.SyncZhengBaData(lastSyncData);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M20(int zhanDuiID, int jiFen, int readyNum)
			{
				return EscapeBattle_K.ZhanDuiJoin(zhanDuiID, jiFen, readyNum);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M2(ServerSocketSender _sender_, int zhanDuiID, int gameId, int srcServerID, out EscapeBattleFuBenData copyData)
			{
				return EscapeBattle_K.ZhengBaKuaFuLogin(_sender_, zhanDuiID, gameId, srcServerID, out copyData);
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			public static int _M4(int zhanDuiID, out int gameId, out int kuaFuServerID, out string[] ips, out int[] ports)
			{
				return EscapeBattle_K.ZhengBaRequestEnter(zhanDuiID, out gameId, out kuaFuServerID, out ips, out ports);
			}
		}
	}
}
