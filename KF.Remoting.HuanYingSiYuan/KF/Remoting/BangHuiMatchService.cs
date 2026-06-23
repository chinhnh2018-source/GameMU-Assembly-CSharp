using System;
using System.Collections.Generic;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	public class BangHuiMatchService
	{
		public static BangHuiMatchService Instance()
		{
			return BangHuiMatchService._instance;
		}

		private KuaFuData<Dictionary<int, List<BangHuiMatchRankInfo>>> BHMatchRankInfoDict
		{
			get
			{
				return this.Persistence.BHMatchRankInfoDict;
			}
			set
			{
				this.Persistence.BHMatchRankInfoDict = value;
			}
		}

		private KuaFuData<List<BangHuiMatchPKInfo>> BHMatchPKInfoList_Gold
		{
			get
			{
				return this.Persistence.BHMatchPKInfoList_Gold;
			}
			set
			{
				this.Persistence.BHMatchPKInfoList_Gold = value;
			}
		}

		private Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Gold
		{
			get
			{
				return this.Persistence.BHMatchBHDataDict_Gold;
			}
			set
			{
				this.Persistence.BHMatchBHDataDict_Gold = value;
			}
		}

		private Dictionary<int, KuaFuData<BHMatchBHData>> BHMatchBHDataDict_Rookie
		{
			get
			{
				return this.Persistence.BHMatchBHDataDict_Rookie;
			}
			set
			{
				this.Persistence.BHMatchBHDataDict_Rookie = value;
			}
		}

		private List<BHMatchBHData> BHMatchBHDataList_GoldJoin
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_GoldJoin;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_GoldJoin = value;
			}
		}

		private List<BHMatchBHData> BHMatchBHDataList_RookieJoin
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_RookieJoin;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_RookieJoin = value;
			}
		}

		private List<BHMatchBHData> BHMatchBHDataList_RookieJoinLast
		{
			get
			{
				return this.Persistence.BHMatchBHDataList_RookieJoinLast;
			}
			set
			{
				this.Persistence.BHMatchBHDataList_RookieJoinLast = value;
			}
		}

		public void InitConfig()
		{
			string paramValueByName = KuaFuServerManager.systemParamsList.GetParamValueByName("LeagueNewNum");
			string[] array = paramValueByName.Split(new char[]
			{
				','
			});
			if (array.Length == 2)
			{
				this.RuntimeData.RookieWinScoreFactor = Global.SafeConvertToInt32(array[0]);
				this.RuntimeData.RookieLoseScoreFactor = Global.SafeConvertToInt32(array[1]);
			}
			this.RuntimeData.RookiePromotionNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("LeagueUp", -1);
			if (!this.RuntimeData.Load(KuaFuServerManager.GetResourcePath("Config\\LeagueMatch.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueOpen.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueSuperList.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\LeagueNewRandom.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.platformType.ToString()))
			{
				LogManager.WriteLog(2, "BangHuiMatchService.InitConfig failed!", null, true);
			}
		}

		private int ComputeCurrentSeasonID(DateTime now)
		{
			int goldSeasonID = this.Persistence.GetGoldSeasonID();
			int currentSeasonIDByTime = this.GetCurrentSeasonIDByTime(now, true);
			int currentSeasonIDByTime2 = this.GetCurrentSeasonIDByTime(now, false);
			return (goldSeasonID == 0 || goldSeasonID != currentSeasonIDByTime) ? currentSeasonIDByTime2 : currentSeasonIDByTime;
		}

		private int GetCurrentSeasonIDByTime(DateTime now, bool calGold = true)
		{
			int result;
			lock (this.RuntimeData.MutexConfig)
			{
				if (!this.RuntimeData.CheckOpenState(now))
				{
					result = 0;
				}
				else
				{
					DateTime dateTime = this.RuntimeData.GetActivityOpenTm();
					TimeSpan timeSpan = TimeSpan.MinValue;
					foreach (BHMatchConfig bhmatchConfig in this.RuntimeData.BHMatchConfigDict.Values)
					{
						if (bhmatchConfig.ID != 1 || calGold)
						{
							int num = bhmatchConfig.TimePoints.Count / 2 - bhmatchConfig.RoundNum % (bhmatchConfig.TimePoints.Count / 2);
							for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
							{
								TimeSpan timeSpan2 = bhmatchConfig.TimePoints[i + 1];
								if (timeSpan2.Days == 0)
								{
									timeSpan2 += new TimeSpan(7, 0, 0, 0);
								}
								if (timeSpan2 > timeSpan && i / 2 < num)
								{
									timeSpan = timeSpan2;
								}
							}
						}
					}
					timeSpan -= new TimeSpan(1, 0, 0, 0);
					TimeSpan t = new TimeSpan(7, 0, 0, 0) - timeSpan;
					TimeSpan timeSpan3 = now + t - dateTime;
					int num2 = timeSpan3.Days - timeSpan3.Days % (7 * this.RuntimeData.GetSeasonWeaks());
					dateTime = dateTime.AddDays((double)num2);
					result = BangHuiMatchUtils.MakeSeason(dateTime);
				}
			}
			return result;
		}

		private int FixRound(BangHuiMatchType type, int round)
		{
			BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(type);
			return (round > bhmatchConfig.RoundNum) ? 1 : round;
		}

		private int GetCurrentRoundByTime(BangHuiMatchType type, DateTime now)
		{
			int result;
			lock (this.RuntimeData.MutexConfig)
			{
				if (!this.RuntimeData.CheckOpenState(now))
				{
					result = 0;
				}
				else
				{
					if (type == 1)
					{
						DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Gold);
						if (now < seasonDateTm)
						{
							return 1;
						}
					}
					else if (type == 2)
					{
						DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Rookie);
						if (now < seasonDateTm)
						{
							return 1;
						}
					}
					DateTime activityOpenTm = this.RuntimeData.GetActivityOpenTm();
					BHMatchConfig bhmatchConfig = null;
					if (!this.RuntimeData.BHMatchConfigDict.TryGetValue(type, out bhmatchConfig))
					{
						result = 0;
					}
					else
					{
						TimeSpan t = new TimeSpan((int)now.DayOfWeek, now.Hour, now.Minute, now.Second);
						if (t.Days == 0)
						{
							t += new TimeSpan(7, 0, 0, 0);
						}
						int num = 0;
						for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
						{
							TimeSpan timeSpan = bhmatchConfig.TimePoints[i + 1];
							if (timeSpan.Days == 0)
							{
								timeSpan += new TimeSpan(7, 0, 0, 0);
							}
							if (t > timeSpan)
							{
								num++;
							}
						}
						int num2 = (now - activityOpenTm).Days % (7 * this.RuntimeData.GetSeasonWeaks()) / 7;
						int val = num2 * bhmatchConfig.TimePoints.Count / 2 + num + 1;
						result = Math.Min(val, bhmatchConfig.RoundNum + 1);
					}
				}
			}
			return result;
		}

		private int ComputeLastSeasonID(int CurSeasonID)
		{
			int result;
			if (0 == CurSeasonID)
			{
				result = 0;
			}
			else
			{
				result = BangHuiMatchUtils.MakeSeason(BangHuiMatchUtils.GetSeasonDateTm(CurSeasonID).AddDays((double)(-7 * this.RuntimeData.GetSeasonWeaks())));
			}
			return result;
		}

		public void LoadDatabase(DateTime now)
		{
			try
			{
				lock (this.Mutex)
				{
					this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
					this.LastSeasonID_Gold = this.Persistence.LoadLastSeasonIDGold();
					this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
					this.LastSeasonID_Rookie = this.ComputeLastSeasonID(this.CurrentSeasonID_Rookie);
					this.Persistence.LoadDatabase(this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
					this.FixCurGoldRankInfo();
					this.InitFuBenManagerData(now);
					this.ReloadChampionRoleData_Gold();
					this.HandleBHMatchGoldAccident(now);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.LoadDatabase failed!", ex, true);
			}
		}

		private void InitFuBenManagerData(DateTime now)
		{
			this.LastUpdateTime = now;
			for (int i = 1; i <= 2; i++)
			{
				this.FuBenMgrDict[i] = new BHMatchFuBenMgrData();
				this.FuBenMgrDict[i].Round = (byte)this.GetCurrentRoundByTime(i, now);
				this.StateMachineDict[i] = new BHMatchStateMachine();
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
				this.StateMachineDict[i].Install(new BHMatchStateMachine.StateHandler(BHMatchStateMachine.StateType.RankAnalyse, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), null, null));
				this.StateMachineDict[i].SetCurrState(BHMatchStateMachine.StateType.Init, TimeUtil.NowDateTime(), i);
				this.StateMachineDict[i].Tick(now, i);
			}
		}

		private void HandleBHMatchGoldAccident(DateTime now)
		{
			if (this.CurrentSeasonID_Gold != 0 && 0 != this.BHMatchBHDataList_GoldJoin.Count)
			{
				int curRound = (int)this.FuBenMgrDict[1].Round;
				BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(1);
				if (curRound <= bhmatchConfig.RoundNum)
				{
					BangHuiMatchPKInfo bangHuiMatchPKInfo = this.BHMatchPKInfoList_Gold.V.Find((BangHuiMatchPKInfo x) => x.season == this.CurrentSeasonID_Gold && (int)x.round == curRound);
					if (null == bangHuiMatchPKInfo)
					{
						this.GenerateNextRoundPKInfo_Gold(now, curRound);
						LogManager.WriteLog(5, string.Format("BHMatch::HandleBHMatchGoldAccident SeasonID_Gold:{0} SeasonID_Rookie:{1} Round:{2}", this.CurrentSeasonID_Gold, this.CurrentSeasonID_Rookie, curRound), null, true);
					}
				}
			}
		}

		public void SwitchLastGoldBH_GM()
		{
			List<BangHuiMatchRankInfo> list = null;
			if (this.BHMatchRankInfoDict.V.TryGetValue(0, out list))
			{
				list.Reverse(0, list.Count);
				TimeUtil.AgeByNow(ref this.BHMatchRankInfoDict.Age);
				this.ReloadChampionRoleData_Gold();
			}
		}

		private void ReloadChampionRoleData_Gold()
		{
			List<BangHuiMatchRankInfo> list = null;
			if (!this.BHMatchRankInfoDict.V.TryGetValue(0, out list))
			{
				this.BHMatchChampionRoleData_Gold.Bytes0 = null;
				TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
			}
			else
			{
				KuaFuData<BHMatchBHData> kuaFuData = null;
				if (list.Count == 0 || !this.BHMatchBHDataDict_Gold.TryGetValue(list[0].Key, out kuaFuData))
				{
					this.BHMatchChampionRoleData_Gold.Bytes0 = null;
					TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
				}
				else
				{
					this.BHMatchChampionRoleData_Gold.Bytes0 = this.Persistence.LoadBHMatchRoleData(kuaFuData.V.type, kuaFuData.V.rid);
					TimeUtil.AgeByNow(ref this.BHMatchChampionRoleData_Gold.Age);
				}
			}
		}

		private void FixCurGoldRankInfo()
		{
			List<BangHuiMatchRankInfo> goldRankList = null;
			if (this.BHMatchRankInfoDict.V.TryGetValue(8, out goldRankList) && goldRankList.Count != 0)
			{
				Dictionary<int, List<BangHuiMatchRankInfo>> dictionary = new Dictionary<int, List<BangHuiMatchRankInfo>>();
				foreach (BangHuiMatchRankInfo bangHuiMatchRankInfo in goldRankList)
				{
					List<BangHuiMatchRankInfo> list = null;
					if (!dictionary.TryGetValue(bangHuiMatchRankInfo.Value, out list))
					{
						list = new List<BangHuiMatchRankInfo>();
						dictionary[bangHuiMatchRankInfo.Value] = list;
					}
					list.Add(bangHuiMatchRankInfo);
				}
				List<BangHuiMatchPKInfo> list2 = this.BHMatchPKInfoList_Gold.V.FindAll((BangHuiMatchPKInfo x) => x.season == this.CurrentSeasonID_Gold);
				foreach (KeyValuePair<int, List<BangHuiMatchRankInfo>> keyValuePair in dictionary)
				{
					List<BangHuiMatchRankInfo> list = keyValuePair.Value;
					foreach (BangHuiMatchRankInfo bangHuiMatchRankInfo in list)
					{
						bangHuiMatchRankInfo.RankValue = 0;
					}
					if (list.Count > 1)
					{
						for (int i = 0; i < list.Count; i++)
						{
							BangHuiMatchRankInfo left = list[i];
							for (int j = i + 1; j < list.Count; j++)
							{
								BangHuiMatchRankInfo right = list[j];
								BangHuiMatchPKInfo bangHuiMatchPKInfo = list2.Find((BangHuiMatchPKInfo x) => (x.bhid1 == left.Key && x.bhid2 == right.Key) || (x.bhid2 == left.Key && x.bhid1 == right.Key));
								if (null != bangHuiMatchPKInfo)
								{
									if (bangHuiMatchPKInfo.bhid1 == left.Key)
									{
										if (bangHuiMatchPKInfo.result == 1)
										{
											left.RankValue++;
										}
										else if (bangHuiMatchPKInfo.result == 2)
										{
											right.RankValue++;
										}
									}
									else if (bangHuiMatchPKInfo.result == 1)
									{
										right.RankValue++;
									}
									else if (bangHuiMatchPKInfo.result == 2)
									{
										left.RankValue++;
									}
								}
							}
						}
					}
				}
				List<BangHuiMatchRankInfo> list3 = new List<BangHuiMatchRankInfo>(goldRankList);
				foreach (BangHuiMatchRankInfo bangHuiMatchRankInfo in list3)
				{
					LogManager.WriteLog(5, string.Format("BHMatch::FixCurGoldRankInfo Before Key:{0} Param1:{1} Param2:{2} Value:{3} RankValue:{4}", new object[]
					{
						bangHuiMatchRankInfo.Key,
						bangHuiMatchRankInfo.Param1,
						bangHuiMatchRankInfo.Param2,
						bangHuiMatchRankInfo.Value,
						bangHuiMatchRankInfo.RankValue
					}), null, true);
				}
				list3.Sort(delegate(BangHuiMatchRankInfo left, BangHuiMatchRankInfo right)
				{
					int result;
					if (left.Value > right.Value)
					{
						result = -1;
					}
					else if (left.Value < right.Value)
					{
						result = 1;
					}
					else if (left.RankValue > right.RankValue)
					{
						result = -1;
					}
					else if (left.RankValue < right.RankValue)
					{
						result = 1;
					}
					else
					{
						int num = goldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == left.Key);
						int num2 = goldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == right.Key);
						if (num < num2)
						{
							result = -1;
						}
						else if (num > num2)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
					}
					return result;
				});
				foreach (BangHuiMatchRankInfo bangHuiMatchRankInfo in list3)
				{
					LogManager.WriteLog(5, string.Format("BHMatch::FixCurGoldRankInfo After Key:{0} Param1:{1} Param2:{2} Value:{3} RankValue:{4}", new object[]
					{
						bangHuiMatchRankInfo.Key,
						bangHuiMatchRankInfo.Param1,
						bangHuiMatchRankInfo.Param2,
						bangHuiMatchRankInfo.Value,
						bangHuiMatchRankInfo.RankValue
					}), null, true);
				}
				this.BHMatchRankInfoDict.V[8] = list3;
				TimeUtil.AgeByNow(ref this.BHMatchRankInfoDict.Age);
			}
		}

		private BangHuiMatchType GetMatchTypeByBhid(int bhid)
		{
			BangHuiMatchType result;
			lock (this.Mutex)
			{
				if (this.BHMatchBHDataList_GoldJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
				{
					result = 1;
				}
				else
				{
					result = 2;
				}
			}
			return result;
		}

		public void Update(DateTime now)
		{
			try
			{
				if (!GameFuncControlManager.IsGameFuncDisabled(15))
				{
					if ((now - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
					{
						this.UpdateFrameCount += 1U;
						foreach (KeyValuePair<int, BHMatchStateMachine> keyValuePair in this.StateMachineDict)
						{
							lock (this.Mutex)
							{
								keyValuePair.Value.Tick(now, keyValuePair.Key);
							}
						}
						this.Persistence.DelayWriteDataProc();
						this.LastUpdateTime = now;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.Update failed!", ex, true);
			}
		}

		private void MS_Init_Update(DateTime now, int param)
		{
			if (this.RuntimeData.CheckOpenState(now))
			{
				BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
				BHMatchStateMachine.StateType stateType = BHMatchStateMachine.StateType.SignUp;
				for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)bhmatchConfig.TimePoints[i].Days)
					{
						if (now.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i] - (double)bhmatchConfig.ApplyOverTime && now.TimeOfDay.TotalSeconds < bhmatchConfig.SecondsOfDay[i])
						{
							stateType = BHMatchStateMachine.StateType.PrepareGame;
						}
						else if (now.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i] && now.TimeOfDay.TotalSeconds < bhmatchConfig.SecondsOfDay[i + 1])
						{
							stateType = BHMatchStateMachine.StateType.GameStart;
						}
						else if (now.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i + 1] && now.TimeOfDay.TotalSeconds <= bhmatchConfig.SecondsOfDay[i + 1] + (double)bhmatchConfig.ApplyStartTime)
						{
							stateType = BHMatchStateMachine.StateType.RankAnalyse;
						}
					}
				}
				this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
				this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
				this.StateMachineDict[param].SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("BHMatch::MS_Init_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					param,
					stateType,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		private void MS_SignUp_Update(DateTime now, int param)
		{
			if (this.RuntimeData.CheckOpenState(now))
			{
				BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
				BHMatchStateMachine.StateType stateType = BHMatchStateMachine.StateType.None;
				for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
				{
					if (now.DayOfWeek == (DayOfWeek)bhmatchConfig.TimePoints[i].Days)
					{
						double num = bhmatchConfig.SecondsOfDay[i] - (double)bhmatchConfig.ApplyOverTime;
						if (this.LastUpdateTime.TimeOfDay.TotalSeconds < num && now.TimeOfDay.TotalSeconds >= num)
						{
							stateType = BHMatchStateMachine.StateType.PrepareGame;
						}
					}
				}
				if (stateType == BHMatchStateMachine.StateType.PrepareGame)
				{
					this.StateMachineDict[param].SetCurrState(stateType, now, param);
					LogManager.WriteLog(5, string.Format("BHMatch::MS_SignUp_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
					{
						param,
						stateType,
						this.CurrentSeasonID_Gold,
						this.CurrentSeasonID_Rookie,
						this.FuBenMgrDict[param].Round
					}), null, true);
				}
			}
		}

		private void MS_PrepareGame_Update(DateTime now, int param)
		{
			if (ClientAgentManager.Instance().IsAnyKfAgentAlive())
			{
				BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
				int currentRoundByTime = this.GetCurrentRoundByTime(param, now);
				this.FuBenMgrDict[param].Round = (byte)currentRoundByTime;
				if (param == 1)
				{
					if (this.BHMatchBHDataList_GoldJoin.Count < 8 || currentRoundByTime > bhmatchConfig.RoundNum)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
						LogManager.WriteLog(5, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							param,
							BHMatchStateMachine.StateType.NotifyEnter,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Gold);
					if (now < seasonDateTm)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.SignUp, now, param);
						LogManager.WriteLog(5, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							param,
							BHMatchStateMachine.StateType.SignUp,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					BHMatchGoldGroupConfig bhmatchGoldGroupConfig = this.RuntimeData.GetBHMatchGoldGroupConfig(currentRoundByTime);
					for (int i = 0; i < bhmatchGoldGroupConfig.GroupUnion.Length; i++)
					{
						int num;
						int num2;
						BangHuiMatchUtils.SplitUnionGroup(bhmatchGoldGroupConfig.GroupUnion[i], ref num, ref num2);
						BHMatchFuBenData bhmatchFuBenData = new BHMatchFuBenData();
						bhmatchFuBenData.Type = (byte)param;
						bhmatchFuBenData.bhid1 = this.BHMatchBHDataList_GoldJoin[num - 1].bhid;
						bhmatchFuBenData.bhid2 = this.BHMatchBHDataList_GoldJoin[num2 - 1].bhid;
						int serverId = 0;
						int nextGameId = TianTiPersistence.Instance.GetNextGameId();
						if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)nextGameId, bhmatchConfig.MaxEnterNum, out serverId))
						{
							bhmatchFuBenData.ServerId = serverId;
							bhmatchFuBenData.GameId = (long)nextGameId;
							this.FuBenMgrDict[param].FuBenDataDict[nextGameId] = bhmatchFuBenData;
							this.FuBenMgrDict[param].BHidVsGameId[bhmatchFuBenData.bhid1] = nextGameId;
							this.FuBenMgrDict[param].BHidVsGameId[bhmatchFuBenData.bhid2] = nextGameId;
							LogManager.WriteLog(5, string.Format("黄金赛分组 gameId:{0} bhid1:{1} bhname1:{2} bhid2:{3} bhname2:{4}", new object[]
							{
								nextGameId,
								bhmatchFuBenData.bhid1,
								this.BHMatchBHDataList_GoldJoin[num - 1].bhname,
								bhmatchFuBenData.bhid2,
								this.BHMatchBHDataList_GoldJoin[num2 - 1].bhname
							}), null, true);
						}
						else
						{
							LogManager.WriteLog(2, string.Format("黄金赛{0}赛季第{1}轮分配游戏服务器失败,bhid1={2},bhid2={3}", new object[]
							{
								this.CurrentSeasonID_Gold,
								currentRoundByTime,
								bhmatchFuBenData.bhid1,
								bhmatchFuBenData.bhid2
							}), null, true);
						}
					}
				}
				else
				{
					if (this.BHMatchBHDataList_RookieJoin.Count == 0 || currentRoundByTime > bhmatchConfig.RoundNum)
					{
						this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
						LogManager.WriteLog(5, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} SkipTo:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
						{
							param,
							BHMatchStateMachine.StateType.NotifyEnter,
							this.CurrentSeasonID_Gold,
							this.CurrentSeasonID_Rookie,
							this.FuBenMgrDict[param].Round
						}), null, true);
						return;
					}
					this.BHMatchBHDataList_RookieJoin.Sort(delegate(BHMatchBHData left, BHMatchBHData right)
					{
						int result;
						if (left.cur_score > right.cur_score)
						{
							result = -1;
						}
						else if (left.cur_score < right.cur_score)
						{
							result = 1;
						}
						else if (left.hist_score > right.hist_score)
						{
							result = -1;
						}
						else if (left.hist_score < right.hist_score)
						{
							result = 1;
						}
						else if (left.bhid > right.bhid)
						{
							result = -1;
						}
						else if (left.bhid < right.bhid)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					List<BHMatchRookieRandomConfig> list = null;
					lock (this.RuntimeData.MutexConfig)
					{
						list = this.RuntimeData.BHMatchRookieRandomConfigList;
					}
					int num3 = 0;
					while (num3 < list.Count && this.BHMatchBHDataList_RookieJoin.Count > 0)
					{
						BHMatchRookieRandomConfig bhmatchRookieRandomConfig = list[num3];
						if (bhmatchRookieRandomConfig.BeginNum <= this.BHMatchBHDataList_RookieJoin.Count)
						{
							int num4 = Math.Min(this.BHMatchBHDataList_RookieJoin.Count, bhmatchRookieRandomConfig.EndNum);
							int count = num4 - bhmatchRookieRandomConfig.BeginNum + 1;
							List<BHMatchBHData> range = this.BHMatchBHDataList_RookieJoin.GetRange(bhmatchRookieRandomConfig.BeginNum - 1, count);
							Random random = new Random((int)now.Ticks);
							int i = 0;
							while (range.Count > 0 && i < range.Count * 2)
							{
								int index = random.Next(0, range.Count);
								int index2 = random.Next(0, range.Count);
								BHMatchBHData value = range[index];
								range[index] = range[index2];
								range[index2] = value;
								i++;
							}
							int j = 0;
							for (i = 0; i < range.Count / 2; i++)
							{
								BHMatchBHData bhmatchBHData = range[j++];
								BHMatchBHData bhmatchBHData2 = range[j++];
								BHMatchFuBenData bhmatchFuBenData = new BHMatchFuBenData();
								bhmatchFuBenData.Type = (byte)param;
								bhmatchFuBenData.bhid1 = bhmatchBHData.bhid;
								bhmatchFuBenData.bhid2 = bhmatchBHData2.bhid;
								int serverId = 0;
								int nextGameId = TianTiPersistence.Instance.GetNextGameId();
								if (ClientAgentManager.Instance().AssginKfFuben(this.GameType, (long)nextGameId, bhmatchConfig.MaxEnterNum, out serverId))
								{
									bhmatchFuBenData.ServerId = serverId;
									bhmatchFuBenData.GameId = (long)nextGameId;
									this.FuBenMgrDict[param].FuBenDataDict[nextGameId] = bhmatchFuBenData;
									this.FuBenMgrDict[param].BHidVsGameId[bhmatchBHData.bhid] = nextGameId;
									this.FuBenMgrDict[param].BHidVsGameId[bhmatchBHData2.bhid] = nextGameId;
									LogManager.WriteLog(5, string.Format("新星赛分组 gameId:{0} bhid1:{1} bhname1:{2} bhid2:{3} bhname2:{4}", new object[]
									{
										nextGameId,
										bhmatchFuBenData.bhid1,
										bhmatchBHData.bhname,
										bhmatchFuBenData.bhid2,
										bhmatchBHData2.bhname
									}), null, true);
								}
								else
								{
									LogManager.WriteLog(2, string.Format("新星赛{0}赛季第{1}轮分配游戏服务器失败,bhid1={2},bhid2={3}", new object[]
									{
										this.CurrentSeasonID_Rookie,
										currentRoundByTime,
										bhmatchBHData.bhid,
										bhmatchBHData2.bhid
									}), null, true);
								}
							}
							while (j < range.Count)
							{
								BHMatchBHData bhmatchBHData3 = range[j++];
								bhmatchBHData3.hist_score += this.RuntimeData.RookieWinScoreFactor;
								bhmatchBHData3.cur_score += this.RuntimeData.RookieWinScoreFactor;
								this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, bhmatchBHData3, false, true);
							}
						}
						num3++;
					}
				}
				this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.NotifyEnter, now, param);
				LogManager.WriteLog(5, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					param,
					BHMatchStateMachine.StateType.NotifyEnter,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		private void MS_NotifyEnter_Update(DateTime now, int param)
		{
			BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
			BHMatchStateMachine.StateType stateType = BHMatchStateMachine.StateType.None;
			for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)bhmatchConfig.TimePoints[i].Days)
				{
					if (now.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i])
					{
						stateType = BHMatchStateMachine.StateType.GameStart;
					}
				}
			}
			if (stateType == BHMatchStateMachine.StateType.GameStart)
			{
				BHMatchFuBenMgrData bhmatchFuBenMgrData = this.FuBenMgrDict[param];
				foreach (BHMatchFuBenData bhmatchFuBenData in bhmatchFuBenMgrData.FuBenDataDict.Values)
				{
					BHMatchNtfEnterData bhmatchNtfEnterData = new BHMatchNtfEnterData();
					bhmatchNtfEnterData.bhid1 = bhmatchFuBenData.bhid1;
					bhmatchNtfEnterData.bhid2 = bhmatchFuBenData.bhid2;
					ClientAgentManager.Instance().BroadCastAsyncEvent(this.EvItemGameType, new AsyncDataItem(10028, new object[]
					{
						bhmatchNtfEnterData
					}), 0);
				}
				this.StateMachineDict[param].SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("BHMatch::MS_PrepareGame_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					param,
					stateType,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		private void MS_GameStart_Update(DateTime now, int param)
		{
			BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
			BHMatchStateMachine.StateType stateType = BHMatchStateMachine.StateType.None;
			for (int i = 0; i < bhmatchConfig.TimePoints.Count - 1; i += 2)
			{
				if (now.DayOfWeek == (DayOfWeek)bhmatchConfig.TimePoints[i].Days)
				{
					if (now.TimeOfDay.TotalSeconds >= bhmatchConfig.SecondsOfDay[i + 1] + (double)(bhmatchConfig.ApplyStartTime / 2))
					{
						stateType = BHMatchStateMachine.StateType.RankAnalyse;
					}
				}
			}
			if (stateType == BHMatchStateMachine.StateType.RankAnalyse)
			{
				this.StateMachineDict[param].SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("BHMatch::MS_GameStart_Update MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
				{
					param,
					stateType,
					this.CurrentSeasonID_Gold,
					this.CurrentSeasonID_Rookie,
					this.FuBenMgrDict[param].Round
				}), null, true);
			}
		}

		private void HandleUnCompleteFuBenData()
		{
			foreach (KeyValuePair<int, BHMatchFuBenMgrData> keyValuePair in this.FuBenMgrDict)
			{
				int key = keyValuePair.Key;
				foreach (KeyValuePair<int, BHMatchFuBenData> keyValuePair2 in keyValuePair.Value.FuBenDataDict)
				{
					BHMatchFuBenData value = keyValuePair2.Value;
					KuaFuData<BHMatchBHData> kuaFuData = null;
					KuaFuData<BHMatchBHData> kuaFuData2 = null;
					if (key == 1)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(value.bhid1, out kuaFuData);
						this.BHMatchBHDataDict_Gold.TryGetValue(value.bhid2, out kuaFuData2);
						if (kuaFuData != null && null != kuaFuData2)
						{
							kuaFuData.V.hist_play++;
							kuaFuData2.V.hist_play++;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							TimeUtil.AgeByNow(ref kuaFuData2.Age);
							this.Persistence.SaveBHMatchBHData(kuaFuData.V, false, false);
							this.Persistence.SaveBHMatchBHData(kuaFuData2.V, false, false);
						}
					}
					else
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(value.bhid1, out kuaFuData);
						this.BHMatchBHDataDict_Rookie.TryGetValue(value.bhid2, out kuaFuData2);
						if (kuaFuData != null && null != kuaFuData2)
						{
							kuaFuData.V.hist_score += this.RuntimeData.RookieLoseScoreFactor;
							kuaFuData2.V.hist_score += this.RuntimeData.RookieLoseScoreFactor;
							kuaFuData.V.cur_score += this.RuntimeData.RookieLoseScoreFactor;
							kuaFuData2.V.cur_score += this.RuntimeData.RookieLoseScoreFactor;
							kuaFuData.V.hist_play++;
							kuaFuData2.V.hist_play++;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							TimeUtil.AgeByNow(ref kuaFuData2.Age);
							this.Persistence.SaveBHMatchBHData(kuaFuData.V, false, false);
							this.Persistence.SaveBHMatchBHData(kuaFuData2.V, false, false);
							this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, kuaFuData.V, false, true);
							this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, kuaFuData2.V, false, true);
						}
					}
					if (kuaFuData != null && null != kuaFuData2)
					{
						this.GeneratePKInfo((byte)key, this.CurrentSeasonID_Rookie, (int)this.FuBenMgrDict[key].Round, kuaFuData.V, kuaFuData2.V, 0);
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, value.ServerId, value.GameId);
				}
				keyValuePair.Value.Clear();
			}
		}

		private void MS_RankAnalyse_Enter(DateTime now, int param)
		{
			this.HandleUnCompleteFuBenData();
			int currentRoundByTime = this.GetCurrentRoundByTime(param, now);
			if (this.CurrentSeasonID_Gold != this.ComputeCurrentSeasonID(now))
			{
				this.HandlePromotion();
				this.Persistence.SaveLastSeasonIDGold(this.CurrentSeasonID_Gold);
				this.LastSeasonID_Gold = this.CurrentSeasonID_Gold;
				this.CurrentSeasonID_Gold = this.ComputeCurrentSeasonID(now);
				for (int i = 0; i < this.BHMatchBHDataList_GoldJoin.Count; i++)
				{
					BHMatchBHData data = this.BHMatchBHDataList_GoldJoin[i];
					this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Gold, data, true, true);
				}
				this.Persistence.ReloadDatabasePerRound(1, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, true);
				this.ReloadChampionRoleData_Gold();
				this.GenerateNextRoundPKInfo_Gold(now, currentRoundByTime);
				this.FuBenMgrDict[1].Round = (byte)this.GetCurrentRoundByTime(1, now);
			}
			else if (param == 1)
			{
				this.Persistence.ReloadDatabasePerRound(param, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold, false);
				this.FixCurGoldRankInfo();
				BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(param);
				if (currentRoundByTime <= bhmatchConfig.RoundNum)
				{
					this.GenerateNextRoundPKInfo_Gold(now, currentRoundByTime);
				}
			}
			if (this.CurrentSeasonID_Rookie != this.GetCurrentSeasonIDByTime(now, false))
			{
				this.LastSeasonID_Rookie = this.CurrentSeasonID_Rookie;
				this.CurrentSeasonID_Rookie = this.GetCurrentSeasonIDByTime(now, false);
				this.Persistence.ReloadDatabasePerRound(2, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie, true);
			}
			else if (param == 2)
			{
				this.Persistence.ReloadDatabasePerRound(param, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie, false);
			}
			this.FuBenMgrDict[param].Round = (byte)this.GetCurrentRoundByTime(param, now);
			this.StateMachineDict[param].SetCurrState(BHMatchStateMachine.StateType.SignUp, now, param);
			LogManager.WriteLog(5, string.Format("BHMatch::MS_RankAnalyse_Enter MatchType:{0} To:{1} SeasonID_Gold:{2} SeasonID_Rookie:{3} Round:{4}", new object[]
			{
				param,
				BHMatchStateMachine.StateType.SignUp,
				this.CurrentSeasonID_Gold,
				this.CurrentSeasonID_Rookie,
				this.FuBenMgrDict[param].Round
			}), null, true);
		}

		private void GenerateNextRoundPKInfo_Gold(DateTime now, int round)
		{
			if (this.BHMatchBHDataList_GoldJoin.Count == 8)
			{
				round = this.FixRound(1, round);
				BHMatchGoldGroupConfig bhmatchGoldGroupConfig = this.RuntimeData.GetBHMatchGoldGroupConfig(round);
				for (int i = 0; i < bhmatchGoldGroupConfig.GroupUnion.Length; i++)
				{
					int num;
					int num2;
					BangHuiMatchUtils.SplitUnionGroup(bhmatchGoldGroupConfig.GroupUnion[i], ref num, ref num2);
					this.GeneratePKInfo(1, this.CurrentSeasonID_Gold, round, this.BHMatchBHDataList_GoldJoin[num - 1], this.BHMatchBHDataList_GoldJoin[num2 - 1], 0);
				}
			}
		}

		private void GeneratePKInfo(byte type, int season, int round, BHMatchBHData bh1Data, BHMatchBHData bh2Data, byte result)
		{
			BangHuiMatchPKInfo bangHuiMatchPKInfo = null;
			if (type == 1)
			{
				bangHuiMatchPKInfo = this.BHMatchPKInfoList_Gold.V.Find((BangHuiMatchPKInfo x) => x.season == season && (int)x.round == round && x.bhid1 == bh1Data.bhid);
			}
			if (null == bangHuiMatchPKInfo)
			{
				bangHuiMatchPKInfo = new BangHuiMatchPKInfo();
				bangHuiMatchPKInfo.type = type;
				bangHuiMatchPKInfo.season = season;
				bangHuiMatchPKInfo.round = (byte)round;
				bangHuiMatchPKInfo.bhid1 = bh1Data.bhid;
				bangHuiMatchPKInfo.bhname1 = KuaFuServerManager.FormatName(bh1Data.bhname, bh1Data.zoneid_bh);
				bangHuiMatchPKInfo.zoneid1 = bh1Data.zoneid_bh;
				bangHuiMatchPKInfo.bhid2 = bh2Data.bhid;
				bangHuiMatchPKInfo.bhname2 = KuaFuServerManager.FormatName(bh2Data.bhname, bh2Data.zoneid_bh);
				bangHuiMatchPKInfo.zoneid2 = bh2Data.zoneid_bh;
				if (type == 1)
				{
					this.BHMatchPKInfoList_Gold.V.Add(bangHuiMatchPKInfo);
				}
			}
			bangHuiMatchPKInfo.result = result;
			this.Persistence.SaveBHMatchPKInfo(bangHuiMatchPKInfo);
			if (type == 1)
			{
				TimeUtil.AgeByNow(ref this.BHMatchPKInfoList_Gold.Age);
			}
		}

		private void HandlePromotion()
		{
			List<BangHuiMatchRankInfo> list = null;
			if (this.CurrentSeasonID_Rookie == this.CurrentSeasonID_Gold)
			{
				this.Persistence.ReloadRankInfo(9, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
				if (!this.BHMatchRankInfoDict.V.TryGetValue(9, out list))
				{
					list = new List<BangHuiMatchRankInfo>();
				}
			}
			else
			{
				this.Persistence.ReloadRankInfo(1, this.CurrentSeasonID_Rookie, this.LastSeasonID_Rookie);
				if (!this.BHMatchRankInfoDict.V.TryGetValue(1, out list))
				{
					list = new List<BangHuiMatchRankInfo>();
				}
			}
			if (list.Count != 0)
			{
				KuaFuData<BHMatchBHData> kuaFuData = null;
				this.BHMatchBHDataDict_Rookie.TryGetValue(list[0].Key, out kuaFuData);
				kuaFuData.V.hist_champion++;
				TimeUtil.AgeByNow(ref kuaFuData.Age);
				this.Persistence.SaveBHMatchBHData(kuaFuData.V, true, false);
			}
			for (int i = 0; i < list.Count; i++)
			{
				KuaFuData<BHMatchBHData> kuaFuData2 = null;
				if (this.BHMatchBHDataDict_Rookie.TryGetValue(list[i].Key, out kuaFuData2))
				{
					if (kuaFuData2.V.best_record == 0 || kuaFuData2.V.best_record >= i + 1)
					{
						kuaFuData2.V.best_record = i + 1;
						TimeUtil.AgeByNow(ref kuaFuData2.Age);
						this.Persistence.SaveBHMatchBHData(kuaFuData2.V, false, false);
					}
				}
			}
			this.Persistence.ReloadRankInfo(8, this.CurrentSeasonID_Gold, this.LastSeasonID_Gold);
			this.FixCurGoldRankInfo();
			List<BangHuiMatchRankInfo> CurGoldRankList = null;
			if (!this.BHMatchRankInfoDict.V.TryGetValue(8, out CurGoldRankList))
			{
				CurGoldRankList = new List<BangHuiMatchRankInfo>();
			}
			for (int i = 0; i < CurGoldRankList.Count; i++)
			{
				KuaFuData<BHMatchBHData> kuaFuData2 = null;
				if (this.BHMatchBHDataDict_Gold.TryGetValue(CurGoldRankList[i].Key, out kuaFuData2))
				{
					if (kuaFuData2.V.best_record == 0 || kuaFuData2.V.best_record >= i + 1)
					{
						kuaFuData2.V.best_record = i + 1;
						TimeUtil.AgeByNow(ref kuaFuData2.Age);
						this.Persistence.SaveBHMatchBHData(kuaFuData2.V, false, false);
					}
				}
			}
			this.BHMatchBHDataList_GoldJoin.Sort(delegate(BHMatchBHData left, BHMatchBHData right)
			{
				int num3 = CurGoldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == left.bhid);
				int num4 = CurGoldRankList.FindIndex((BangHuiMatchRankInfo x) => x.Key == right.bhid);
				int result;
				if (num3 > num4)
				{
					result = 1;
				}
				else if (num3 < num4)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
				return result;
			});
			for (int j = 0; j < this.BHMatchBHDataList_GoldJoin.Count; j++)
			{
				this.BHMatchBHDataList_GoldJoin[j].group = j + 1;
				this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Gold, this.BHMatchBHDataList_GoldJoin[j], false, false);
			}
			foreach (BHMatchBHData bhmatchBHData in this.BHMatchBHDataList_RookieJoin)
			{
				bhmatchBHData.ChangeSeason();
			}
			foreach (BHMatchBHData bhmatchBHData in this.BHMatchBHDataList_GoldJoin)
			{
				bhmatchBHData.ChangeSeason();
			}
			int num = 0;
			if (this.BHMatchBHDataList_GoldJoin.Count == 0)
			{
				if (list.Count >= 8)
				{
					num = 8;
				}
			}
			else
			{
				this.BHMatchBHDataList_GoldJoin[0].hist_champion++;
				this.Persistence.SaveBHMatchBHData(this.BHMatchBHDataList_GoldJoin[0], true, false);
				num = Math.Min(this.RuntimeData.RookiePromotionNum, list.Count);
				if (num > 0 && this.BHMatchBHDataList_GoldJoin.Count > num)
				{
					int num2 = this.BHMatchBHDataList_GoldJoin.Count - num;
					for (int k = num2; k < this.BHMatchBHDataList_GoldJoin.Count; k++)
					{
						LogManager.WriteLog(5, string.Format("黄金赛淘汰,bhid={0} bhname={1}", this.BHMatchBHDataList_GoldJoin[k].bhid, this.BHMatchBHDataList_GoldJoin[k].bhname), null, true);
					}
					this.BHMatchBHDataList_GoldJoin.RemoveRange(num2, num);
				}
			}
			KuaFuData<BHMatchBHData> kuaFuData3 = null;
			for (int l = 0; l < num; l++)
			{
				KuaFuData<BHMatchBHData> kuaFuData4 = null;
				this.BHMatchBHDataDict_Rookie.TryGetValue(list[l].Key, out kuaFuData4);
				if (!this.BHMatchBHDataDict_Gold.TryGetValue(kuaFuData4.V.bhid, out kuaFuData3))
				{
					kuaFuData3 = new KuaFuData<BHMatchBHData>();
					kuaFuData3.V.type = 1;
					kuaFuData3.V.bhid = kuaFuData4.V.bhid;
					kuaFuData3.V.bhname = kuaFuData4.V.bhname;
					kuaFuData3.V.zoneid_bh = kuaFuData4.V.zoneid_bh;
					kuaFuData3.V.rid = kuaFuData4.V.rid;
					kuaFuData3.V.rname = kuaFuData4.V.rname;
					kuaFuData3.V.zoneid_r = kuaFuData4.V.zoneid_r;
					this.BHMatchBHDataDict_Gold[kuaFuData3.V.bhid] = kuaFuData3;
					this.Persistence.SaveBHMatchBHData(kuaFuData3.V, true, true);
				}
				kuaFuData3.V.group = this.BHMatchBHDataList_GoldJoin.Count + 1;
				this.BHMatchBHDataList_GoldJoin.Add(kuaFuData3.V);
				LogManager.WriteLog(5, string.Format("新星赛晋级,bhid={0} bhname={1} group={2}", kuaFuData3.V.bhid, kuaFuData3.V.bhname, kuaFuData3.V.group), null, true);
			}
			this.BHMatchBHDataList_RookieJoinLast = new List<BHMatchBHData>(this.BHMatchBHDataList_RookieJoin);
			this.BHMatchBHDataList_RookieJoin.Clear();
		}

		public BHMatchSyncData SyncData_BHMatch(long ageRank, long agePKInfo, long ageChampion)
		{
			BHMatchSyncData bhmatchSyncData = new BHMatchSyncData();
			try
			{
				lock (this.Mutex)
				{
					BHMatchFuBenMgrData bhmatchFuBenMgrData = null;
					BHMatchFuBenMgrData bhmatchFuBenMgrData2 = null;
					this.FuBenMgrDict.TryGetValue(1, out bhmatchFuBenMgrData);
					this.FuBenMgrDict.TryGetValue(2, out bhmatchFuBenMgrData2);
					bhmatchSyncData.LastSeasonID_Gold = this.LastSeasonID_Gold;
					bhmatchSyncData.CurSeasonID_Gold = this.CurrentSeasonID_Gold;
					bhmatchSyncData.LastSeasonID_Rookie = this.LastSeasonID_Rookie;
					bhmatchSyncData.CurSeasonID_Rookie = this.CurrentSeasonID_Rookie;
					bhmatchSyncData.RoundGoldReal = (int)bhmatchFuBenMgrData.Round;
					bhmatchSyncData.RoundRookieReal = (int)bhmatchFuBenMgrData2.Round;
					bhmatchSyncData.BHMatchRankInfoDict.Age = this.BHMatchRankInfoDict.Age;
					bhmatchSyncData.BHMatchPKInfoList_Gold.Age = this.BHMatchPKInfoList_Gold.Age;
					bhmatchSyncData.BHMatchChampionRoleData_Gold.Age = this.BHMatchChampionRoleData_Gold.Age;
					if (ageRank != this.BHMatchRankInfoDict.Age)
					{
						bhmatchSyncData.BHMatchRankInfoDict = this.BHMatchRankInfoDict;
					}
					if (agePKInfo != this.BHMatchPKInfoList_Gold.Age)
					{
						bhmatchSyncData.BHMatchPKInfoList_Gold = this.BHMatchPKInfoList_Gold;
					}
					if (ageChampion != this.BHMatchChampionRoleData_Gold.Age)
					{
						bhmatchSyncData.BHMatchChampionRoleData_Gold = this.BHMatchChampionRoleData_Gold;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.SyncData_BHMatch failed!", ex, true);
			}
			return bhmatchSyncData;
		}

		public string GetKuaFuGameState_BHMatch(int bhid)
		{
			string result = "";
			try
			{
				lock (this.Mutex)
				{
					BangHuiMatchType matchTypeByBhid = this.GetMatchTypeByBhid(bhid);
					int num = -4005;
					if (matchTypeByBhid == 2)
					{
						if (!this.BHMatchBHDataList_RookieJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
						{
							num = -4008;
						}
						else if (this.StateMachineDict[2].GetCurrState() == BHMatchStateMachine.StateType.NotifyEnter || this.StateMachineDict[2].GetCurrState() == BHMatchStateMachine.StateType.GameStart)
						{
							if (!this.FuBenMgrDict[2].BHidVsGameId.ContainsKey(bhid))
							{
								num = -4009;
							}
						}
					}
					result = string.Format("{0}:{1}", matchTypeByBhid, num);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.GetKuaFuGameState_BHMatch failed!", ex, true);
			}
			return result;
		}

		public bool CheckRookieJoinLast_BHMatch(int bhid)
		{
			try
			{
				lock (this.Mutex)
				{
					if (!this.BHMatchBHDataList_RookieJoinLast.Exists((BHMatchBHData x) => x.bhid == bhid))
					{
						return false;
					}
					return true;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.GetKuaFuGameState_BHMatch failed!", ex, true);
			}
			return false;
		}

		public int RookieSignUp_BHMatch(int bhid, int zoneid_bh, string bhname, int rid, string rname, int zoneid_r)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					if (!this.RuntimeData.CheckOpenState(dateTime))
					{
						result = -11004;
						return result;
					}
					BHMatchConfig bhmatchConfig = this.RuntimeData.GetBHMatchConfig(2);
					if (null == bhmatchConfig)
					{
						result = -3;
						return result;
					}
					DateTime activityCloseTm = this.RuntimeData.GetActivityCloseTm();
					int num = (int)(activityCloseTm - dateTime).TotalDays;
					if (num < 7)
					{
						int num2 = 0;
						for (int i = 0; i < num; i++)
						{
							bool flag2 = false;
							DateTime dateTime2 = dateTime.AddDays((double)i);
							for (int j = 0; j < bhmatchConfig.TimePoints.Count - 1; j += 2)
							{
								TimeSpan timeSpan = bhmatchConfig.TimePoints[j];
								if (j == 0 && dateTime2.DayOfWeek == (DayOfWeek)timeSpan.Days && dateTime2.TimeOfDay < timeSpan.Subtract(new TimeSpan(0, 0, bhmatchConfig.ApplyOverTime)))
								{
									flag2 = true;
									break;
								}
								if (dateTime2.DayOfWeek == (DayOfWeek)timeSpan.Days)
								{
									flag2 = true;
									break;
								}
							}
							if (flag2)
							{
								num2++;
							}
						}
						if (num2 == 0)
						{
							result = -2001;
							return result;
						}
					}
					DateTime seasonDateTm = BangHuiMatchUtils.GetSeasonDateTm(this.CurrentSeasonID_Rookie);
					if (dateTime < seasonDateTm || this.StateMachineDict[2].GetCurrState() != BHMatchStateMachine.StateType.SignUp)
					{
						result = -2001;
						return result;
					}
					BangHuiMatchType matchTypeByBhid = this.GetMatchTypeByBhid(bhid);
					if (matchTypeByBhid == 1)
					{
						result = -4005;
						return result;
					}
					if (this.BHMatchBHDataList_RookieJoin.Exists((BHMatchBHData x) => x.bhid == bhid))
					{
						result = -4005;
						return result;
					}
					KuaFuData<BHMatchBHData> kuaFuData = null;
					if (!this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out kuaFuData))
					{
						kuaFuData = new KuaFuData<BHMatchBHData>();
						kuaFuData.V.type = matchTypeByBhid;
						kuaFuData.V.bhid = bhid;
						kuaFuData.V.bhname = bhname;
						kuaFuData.V.zoneid_bh = zoneid_bh;
						kuaFuData.V.rid = rid;
						kuaFuData.V.rname = rname;
						kuaFuData.V.zoneid_r = zoneid_r;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						this.BHMatchBHDataDict_Rookie[bhid] = kuaFuData;
						this.Persistence.SaveBHMatchBHData(kuaFuData.V, true, true);
					}
					this.BHMatchBHDataList_RookieJoin.Add(kuaFuData.V);
					this.Persistence.SaveBHMatchBHSeasonData(this.CurrentSeasonID_Rookie, kuaFuData.V, true, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(2, "BangHuiMatchService.RookieSignUp_BHMatch failed!", ex, true);
			}
			return result;
		}

		public BHMatchFuBenData GetFuBenDataByBhid_BHMatch(int bhid)
		{
			BHMatchFuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					BangHuiMatchType matchTypeByBhid = this.GetMatchTypeByBhid(bhid);
					int key = 0;
					this.FuBenMgrDict[matchTypeByBhid].BHidVsGameId.TryGetValue(bhid, out key);
					this.FuBenMgrDict[matchTypeByBhid].FuBenDataDict.TryGetValue(key, out result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.GetFuBenDataByBhid_BHMatch failed!", ex, true);
			}
			return result;
		}

		public BHMatchFuBenData GetFuBenDataByGameId_BHMatch(int gameid)
		{
			BHMatchFuBenData bhmatchFuBenData = null;
			try
			{
				lock (this.Mutex)
				{
					for (int i = 1; i <= 2; i++)
					{
						this.FuBenMgrDict[i].FuBenDataDict.TryGetValue(gameid, out bhmatchFuBenData);
						if (null != bhmatchFuBenData)
						{
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.GetFuBenDataByGameId_BHMatch failed!", ex, true);
			}
			return bhmatchFuBenData;
		}

		public KuaFuCmdData GetBHDataByBhid_BHMatch(int type, int bhid, long age)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<BHMatchBHData> kuaFuData = null;
					if (type == 1)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(bhid, out kuaFuData);
					}
					if (type == 2)
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(bhid, out kuaFuData);
					}
					if (kuaFuData == null)
					{
						return null;
					}
					if (age != kuaFuData.Age)
					{
						return new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<BHMatchBHData>(kuaFuData.V)
						};
					}
					return new KuaFuCmdData
					{
						Age = kuaFuData.Age
					};
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "BangHuiMatchService.GetBHDataByBhid_BHMatch failed!", ex, true);
			}
			return null;
		}

		public int GameFuBenComplete_BHMatch(BangHuiMatchStatisticalData data)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<BHMatchBHData> kuaFuData = null;
					KuaFuData<BHMatchBHData> kuaFuData2 = null;
					BangHuiMatchType matchTypeByBhid = this.GetMatchTypeByBhid(data.bhid1);
					if (matchTypeByBhid == 1)
					{
						this.BHMatchBHDataDict_Gold.TryGetValue(data.bhid1, out kuaFuData);
						this.BHMatchBHDataDict_Gold.TryGetValue(data.bhid2, out kuaFuData2);
					}
					else
					{
						this.BHMatchBHDataDict_Rookie.TryGetValue(data.bhid1, out kuaFuData);
						this.BHMatchBHDataDict_Rookie.TryGetValue(data.bhid2, out kuaFuData2);
					}
					if (kuaFuData == null || null == kuaFuData2)
					{
						result = -2;
						return result;
					}
					BHMatchFuBenData bhmatchFuBenData = null;
					this.FuBenMgrDict[matchTypeByBhid].FuBenDataDict.TryGetValue(data.GameId, out bhmatchFuBenData);
					if (null == bhmatchFuBenData)
					{
						result = -4000;
						return result;
					}
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, bhmatchFuBenData.ServerId, (long)data.GameId);
					this.FuBenMgrDict[matchTypeByBhid].FuBenDataDict.Remove(data.GameId);
					kuaFuData.V.hist_play++;
					kuaFuData2.V.hist_play++;
					kuaFuData.V.hist_kill += data.kill1;
					kuaFuData2.V.hist_kill += data.kill2;
					kuaFuData.V.hist_score += data.score1;
					kuaFuData2.V.hist_score += data.score2;
					kuaFuData.V.cur_score += data.score1;
					kuaFuData2.V.cur_score += data.score2;
					if (!string.IsNullOrEmpty(data.bhname1))
					{
						kuaFuData.V.bhname = data.bhname1;
					}
					if (!string.IsNullOrEmpty(data.bhname2))
					{
						kuaFuData2.V.bhname = data.bhname2;
					}
					if (!string.IsNullOrEmpty(data.rname1))
					{
						kuaFuData.V.rid = data.rid1;
						kuaFuData.V.rname = data.rname1;
						kuaFuData.V.zoneid_r = data.zoneid_r1;
					}
					if (!string.IsNullOrEmpty(data.rname2))
					{
						kuaFuData2.V.rid = data.rid2;
						kuaFuData2.V.rname = data.rname2;
						kuaFuData2.V.zoneid_r = data.zoneid_r2;
					}
					if (data.result == 1)
					{
						kuaFuData.V.cur_win++;
						kuaFuData.V.hist_win++;
						if (data.bullshit)
						{
							kuaFuData.V.hist_bullshit++;
						}
					}
					else if (data.result == 2)
					{
						kuaFuData2.V.cur_win++;
						kuaFuData2.V.hist_win++;
						if (data.bullshit)
						{
							kuaFuData2.V.hist_bullshit++;
						}
					}
					if (null != data.bzroledata1)
					{
						this.Persistence.SaveBHMatchRolesData(matchTypeByBhid, data.rid1, data.rname1, data.zoneid_r1, data.bhid1, data.bzroledata1);
					}
					if (null != data.bzroledata2)
					{
						this.Persistence.SaveBHMatchRolesData(matchTypeByBhid, data.rid2, data.rname2, data.zoneid_r2, data.bhid2, data.bzroledata2);
					}
					TimeUtil.AgeByNow(ref kuaFuData.Age);
					TimeUtil.AgeByNow(ref kuaFuData2.Age);
					this.Persistence.SaveBHMatchBHData(kuaFuData.V, false, data.kill1 != 0);
					this.Persistence.SaveBHMatchBHData(kuaFuData2.V, false, data.kill2 != 0);
					int num = (matchTypeByBhid == 1) ? this.CurrentSeasonID_Gold : this.CurrentSeasonID_Rookie;
					this.Persistence.SaveBHMatchBHSeasonData(num, kuaFuData.V, data.result == 1, data.score1 != 0);
					this.Persistence.SaveBHMatchBHSeasonData(num, kuaFuData2.V, data.result == 2, data.score2 != 0);
					foreach (BHMatchRoleData bhmatchRoleData in data.roleStatisticalData.Values)
					{
						if (bhmatchRoleData.rid != 0 && !string.IsNullOrEmpty(bhmatchRoleData.rname))
						{
							bhmatchRoleData.type = matchTypeByBhid;
							this.Persistence.SaveBHMatchRolesData(matchTypeByBhid, bhmatchRoleData.rid, bhmatchRoleData.rname, bhmatchRoleData.zoneid, bhmatchRoleData.bhid, null);
							this.Persistence.SaveBHMatchRolesSeasonData(num, bhmatchRoleData, 0 != bhmatchRoleData.mvp, 0 != bhmatchRoleData.kill);
						}
					}
					this.GeneratePKInfo(matchTypeByBhid, num, (int)this.FuBenMgrDict[matchTypeByBhid].Round, kuaFuData.V, kuaFuData2.V, data.result);
					LogManager.WriteLog(5, string.Format("BHMatch::GameFuBenComplete_BHMatch GameID:{14} Type:{15} SeasonID:{0} Round:{1}\r\n                            bhid1:{2} bhname1:S{3}·{4} rcount1:{5} score1:{6} | bhid2:{7} bhname2:S{8}·{9} rcount2:{10} score2:{11} | result:{12} | templechg:{13}", new object[]
					{
						num,
						this.FuBenMgrDict[matchTypeByBhid].Round,
						data.bhid1,
						kuaFuData.V.zoneid_bh,
						kuaFuData.V.bhname,
						data.rolecount1,
						data.score1,
						data.bhid2,
						kuaFuData2.V.zoneid_bh,
						kuaFuData2.V.bhname,
						data.rolecount2,
						data.score2,
						data.result,
						data.templechg,
						data.GameId,
						matchTypeByBhid
					}), null, true);
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(2, "BangHuiMatchService.GameFuBenComplete_BHMatch failed!", ex, true);
			}
			return result;
		}

		public int RemoveBangHui_BHMatch(int bhid)
		{
			return 0;
		}

		private static BangHuiMatchService _instance = new BangHuiMatchService();

		public readonly GameTypes GameType = 24;

		public readonly GameTypes EvItemGameType = 2;

		private object Mutex = new object();

		private BangHuiMatchCommonData RuntimeData = new BangHuiMatchCommonData();

		public BangHuiMatchPersistence Persistence = BangHuiMatchPersistence.Instance;

		public KuaFuCmdData BHMatchChampionRoleData_Gold = new KuaFuCmdData();

		private int CurrentSeasonID_Gold = 0;

		private int LastSeasonID_Gold = 0;

		private int CurrentSeasonID_Rookie = 0;

		private int LastSeasonID_Rookie = 0;

		private Dictionary<int, BHMatchFuBenMgrData> FuBenMgrDict = new Dictionary<int, BHMatchFuBenMgrData>();

		private Dictionary<int, BHMatchStateMachine> StateMachineDict = new Dictionary<int, BHMatchStateMachine>();

		private uint UpdateFrameCount = 0U;

		private DateTime LastUpdateTime = DateTime.MinValue;
	}
}
