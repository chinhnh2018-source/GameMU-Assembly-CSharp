using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using GameServer.Core.Executor;
using GameServer.Logic;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;

namespace KF.Remoting
{
	public class KuaFuLueDuoService
	{
		public static KuaFuLueDuoService Instance()
		{
			return KuaFuLueDuoService._instance;
		}

		private Dictionary<int, KuaFuLueDuoServerInfo> ServerInfoDict
		{
			get
			{
				return this.Persistence.ServerInfoDict;
			}
			set
			{
				this.Persistence.ServerInfoDict = value;
			}
		}

		private Dictionary<int, KuaFuData<KuaFuLueDuoBHData>> KuaFuLueDuoBHDataDict
		{
			get
			{
				return this.Persistence.KuaFuLueDuoBHDataDict;
			}
			set
			{
				this.Persistence.KuaFuLueDuoBHDataDict = value;
			}
		}

		private Dictionary<int, KuaFuLueDuoRankListData> KuaFuLueDuoRankInfoDict
		{
			get
			{
				return this.Persistence.KuaFuLueDuoRankInfoDict;
			}
			set
			{
				this.Persistence.KuaFuLueDuoRankInfoDict = value;
			}
		}

		private int SeasonCount
		{
			get
			{
				return this.Persistence.SeasonCount;
			}
			set
			{
				this.Persistence.SeasonCount = value;
			}
		}

		public bool InitConfig()
		{
			bool result;
			if (this.StateMachine.GetCurrState() >= KuaFuLueDuoStateMachine.StateType.SignUp)
			{
				LogManager.WriteLog(2, "因为跨服掠夺活动正在进行,本活动配置未重新加载!", null, true);
				result = true;
			}
			else
			{
				this.Initialized = false;
				this.RuntimeData.CrusadeSeason = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeSeason", 13);
				this.RuntimeData.CrusadeAttackerNum = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeAttackerNum", 3);
				this.RuntimeData.CrusadeOre = KuaFuServerManager.systemParamsList.GetParamValueIntArrayByName("CrusadeOre");
				this.RuntimeData.CrusadeMinApply = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeMinApply", 10000);
				this.RuntimeData.CrusadeApplyCD = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("CrusadeApplyCD", 300);
				this.RuntimeData.KuaFuLueDuoWaitRankSecs = (int)KuaFuServerManager.systemParamsList.GetParamValueIntByName("KuaFuLueDuoWaitRankSecs", 180);
				if (!this.RuntimeData.Load(KuaFuServerManager.GetResourcePath("Config\\CrusadeWar.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath("Config\\CrusadeGroup.xml", KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.platformType))
				{
					LogManager.WriteLog(2, "KuaFuLueDuoService.InitConfig failed!", null, true);
					throw new ConfigurationErrorsException("KuaFuLueDuoService.InitConfig failed!");
				}
				if (!this.LoadDatabase(TimeUtil.NowDateTime(), true))
				{
					LogManager.WriteLog(2, "从数据库加载跨服掠夺数据失败", null, true);
					result = false;
				}
				else
				{
					this.Initialized = true;
					result = true;
				}
			}
			return result;
		}

		private int ComputeCurrentSeasonID(DateTime now)
		{
			return TimeUtil.GetOffsetDay2(TimeUtil.NowDateTime());
		}

		public bool LoadDatabase(DateTime now, bool hist)
		{
			try
			{
				lock (this.Mutex)
				{
					int num = this.RuntimeData.ZoneGroupConfigDict.Keys.Max();
					this.Persistence.ZoneID2GroupIDs = new int[num + 1];
					this.Persistence.ZoneID2ServerIDs = new int[num + 1];
					foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> keyValuePair in this.RuntimeData.ZoneGroupConfigDict)
					{
						if (now >= keyValuePair.Value.StartTime)
						{
							this.Persistence.ZoneID2GroupIDs[keyValuePair.Key] = keyValuePair.Value.GroupID;
							this.Persistence.ZoneID2ServerIDs[keyValuePair.Key] = keyValuePair.Value.ServerNumber;
						}
					}
					this.SeasonCount = this.Persistence.LoadSeasonCount();
					this.SeasonID = this.ComputeCurrentSeasonID(now);
					int num2 = this.SeasonCount % this.RuntimeData.CrusadeSeason;
					if (num2 == 0)
					{
						num2 = this.RuntimeData.CrusadeSeason;
					}
					int[] histSeasonIDs = this.Persistence.GetHistSeasonIDs(num2);
					this.LastSeasonID = histSeasonIDs[0];
					this.MinSeasonID = histSeasonIDs[2];
					if (num2 == this.RuntimeData.CrusadeSeason && !hist)
					{
						this.MinSeasonID = this.SeasonID;
						this.LastSeasonID = this.SeasonID;
						this.Persistence.ClearLastSeasonData();
					}
					this.SyncDataDict.Clear();
					this.ServerInfoDict.Clear();
					this.KuaFuLueDuoBHDataDict.Clear();
					this.Persistence.JingJiaDict.Clear();
					this.KuaFuLueDuoRankInfoDict.Clear();
					foreach (KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem in this.RuntimeData.GroupConfigDict.Values)
					{
						if (now >= kuaFuLueDuoGroupItem.StartTime)
						{
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo = new KuaFuLueDuoServerInfo
							{
								ServerId = kuaFuLueDuoGroupItem.ServerNumber
							};
							kuaFuLueDuoServerInfo.ZhengFuList = new List<int>();
							kuaFuLueDuoServerInfo.ShiChouList = new List<int>();
							kuaFuLueDuoServerInfo.MingXingList = new List<KuaFuLueDuoRankInfo>();
							kuaFuLueDuoServerInfo.MingXingZhanMengList = "";
							this.ServerInfoDict[kuaFuLueDuoGroupItem.ServerNumber] = kuaFuLueDuoServerInfo;
						}
					}
					if (!this.Persistence.LoadDatabase(this.SeasonID, this.LastSeasonID, this.MinSeasonID))
					{
						LogManager.WriteLog(2, "加载跨服掠夺数据失败", null, true);
						return false;
					}
					this.InitGroupServerList();
					this.InitFuBenManagerData(now);
				}
				return true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.LoadDatabase failed!", ex, true);
			}
			return false;
		}

		public void OnStopServer()
		{
			this.Persistence.DelayWriteDataProc();
		}

		public void Update(DateTime now)
		{
			try
			{
				if (this.Initialized)
				{
					if ((now - this.LastUpdateTime).TotalMilliseconds >= 2000.0)
					{
						if (!GameFuncControlManager.IsGameFuncDisabled(15))
						{
							this.UpdateFrameCount += 1U;
							this.StateMachine.Tick(now, 0);
							this.Persistence.DelayWriteDataProc();
							this.LastUpdateTime = now;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.Update failed!", ex, true);
			}
		}

		private void InitFuBenManagerData(DateTime now)
		{
			if (this.StateMachine.GetCurrState() == KuaFuLueDuoStateMachine.StateType.None)
			{
				this.LastUpdateTime = now;
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.Init, null, new Action<DateTime, int>(this.MS_Init_Update), null));
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.SignUp, null, new Action<DateTime, int>(this.MS_SignUp_Update), null));
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.PrepareGame, null, new Action<DateTime, int>(this.MS_PrepareGame_Update), null));
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.NotifyEnter, null, new Action<DateTime, int>(this.MS_NotifyEnter_Update), null));
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.GameStart, null, new Action<DateTime, int>(this.MS_GameStart_Update), null));
				this.StateMachine.Install(new KuaFuLueDuoStateMachine.StateHandler(KuaFuLueDuoStateMachine.StateType.RankAnalyse, null, new Action<DateTime, int>(this.MS_RankAnalyse_Enter), null));
				this.StateMachine.SetCurrState(KuaFuLueDuoStateMachine.StateType.Init, TimeUtil.NowDateTime(), 0);
				this.StateMachine.Tick(now, 0);
			}
		}

		private int GetServerIdByZoneId(int zoneId)
		{
			KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
			int result;
			if (this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneId, out kuaFuLueDuoGroupItem))
			{
				result = kuaFuLueDuoGroupItem.ServerNumber;
			}
			else
			{
				LogManager.WriteLog(2, "跨服掠夺服务器分组配置表中未包含的区号:" + zoneId, null, true);
				result = zoneId;
			}
			return result;
		}

		private void InitGroupServerList()
		{
			DateTime t = TimeUtil.NowDateTime();
			lock (this.Mutex)
			{
				foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> keyValuePair in this.RuntimeData.GroupConfigDict)
				{
					int groupID = keyValuePair.Value.GroupID;
					int serverNumber = keyValuePair.Value.ServerNumber;
					if (!(t < keyValuePair.Value.StartTime))
					{
						KuaFuLueDuoSyncData kuaFuLueDuoSyncData;
						if (!this.SyncDataDict.TryGetValue(groupID, out kuaFuLueDuoSyncData))
						{
							kuaFuLueDuoSyncData = new KuaFuLueDuoSyncData();
							this.SyncDataDict[groupID] = kuaFuLueDuoSyncData;
						}
						kuaFuLueDuoSyncData.BhZiYuanDict.Clear();
						kuaFuLueDuoSyncData.ServerZiYuanDict.Clear();
						KuaFuLueDuoRankListData kuaFuLueDuoRankInfoDict;
						if (this.Persistence.KuaFuLueDuoRankInfoDict.TryGetValue(groupID, out kuaFuLueDuoRankInfoDict))
						{
							kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict = kuaFuLueDuoRankInfoDict;
							TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict.Age);
						}
						KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
						if (!kuaFuLueDuoSyncData.ServerInfoDict.TryGetValue(serverNumber, out kuaFuLueDuoServerInfo))
						{
							kuaFuLueDuoServerInfo = new KuaFuLueDuoServerInfo
							{
								ServerId = serverNumber
							};
							kuaFuLueDuoServerInfo.ZoneIdRangeList = keyValuePair.Value.ZoneNumber;
							kuaFuLueDuoSyncData.ServerInfoDict[serverNumber] = kuaFuLueDuoServerInfo;
							kuaFuLueDuoServerInfo.ZhengFuList = new List<int>();
							kuaFuLueDuoServerInfo.ShiChouList = new List<int>();
						}
						KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo2;
						if (this.ServerInfoDict.TryGetValue(serverNumber, out kuaFuLueDuoServerInfo2))
						{
							kuaFuLueDuoServerInfo.MingXingZhanMengList = kuaFuLueDuoServerInfo2.MingXingZhanMengList;
							kuaFuLueDuoServerInfo.ZiYuan = kuaFuLueDuoServerInfo2.ZiYuan;
							kuaFuLueDuoServerInfo.LastZiYuan = kuaFuLueDuoServerInfo2.LastZiYuan;
							kuaFuLueDuoServerInfo.ZhengFuList = kuaFuLueDuoServerInfo2.ZhengFuList.Distinct<int>().ToList<int>();
							kuaFuLueDuoServerInfo.ShiChouList = kuaFuLueDuoServerInfo2.ShiChouList.Distinct<int>().ToList<int>();
							for (int i = kuaFuLueDuoServerInfo.ShiChouList.Count - 1; i >= 0; i--)
							{
								KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
								if (!this.RuntimeData.GroupConfigDict.TryGetValue(kuaFuLueDuoServerInfo.ShiChouList[i], out kuaFuLueDuoGroupItem) || kuaFuLueDuoGroupItem.GroupID != groupID)
								{
									kuaFuLueDuoServerInfo.ShiChouList.RemoveAt(i);
								}
							}
						}
					}
				}
				foreach (KuaFuLueDuoSyncData kuaFuLueDuoSyncData in this.SyncDataDict.Values)
				{
					KuaFuLueDuoSyncData kuaFuLueDuoSyncData;
					KuaFuLueDuoServerInfo[] array = kuaFuLueDuoSyncData.ServerInfoDict.Values.ToArray<KuaFuLueDuoServerInfo>();
					Array.Sort<KuaFuLueDuoServerInfo>(array, delegate(KuaFuLueDuoServerInfo x, KuaFuLueDuoServerInfo y)
					{
						int num = y.ZiYuan - x.ZiYuan;
						int result;
						if (num != 0)
						{
							result = num;
						}
						else
						{
							result = x.ServerId - y.ServerId;
						}
						return result;
					});
					for (int j = 0; j < array.Length; j++)
					{
						KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo3 = array[j];
						KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState;
						if (!kuaFuLueDuoSyncData.StateList.TryGetValue(kuaFuLueDuoServerInfo3.ServerId, out kuaFuLueDuoServerJingJiaState))
						{
							if (!this.Persistence.JingJiaDict.TryGetValue(kuaFuLueDuoServerInfo3.ServerId, out kuaFuLueDuoServerJingJiaState))
							{
								kuaFuLueDuoServerJingJiaState = new KuaFuLueDuoServerJingJiaState
								{
									ServerId = kuaFuLueDuoServerInfo3.ServerId
								};
								kuaFuLueDuoServerJingJiaState.JingJiaList = new List<KuaFuLueDuoBangHuiJingJiaData>();
							}
							kuaFuLueDuoSyncData.StateList[kuaFuLueDuoServerInfo3.ServerId] = kuaFuLueDuoServerJingJiaState;
						}
						kuaFuLueDuoServerJingJiaState.ZiYuan = kuaFuLueDuoServerInfo3.ZiYuan;
						kuaFuLueDuoServerJingJiaState.Round = KuaFuLueDuoUtils.GetJingJiaRoundByIndex(j);
						kuaFuLueDuoServerJingJiaState.State = 0;
					}
					kuaFuLueDuoSyncData.ServerInfoDictAge = TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.StateAge);
				}
			}
		}

		private void MS_Init_Update(DateTime now, int param)
		{
			this.SeasonID = this.ComputeCurrentSeasonID(now);
			this.GameState = 0;
			if (this.LastDate != now.Date)
			{
				this.LastDate = now.Date;
				if (!this.LoadDatabase(now, true))
				{
					LogManager.WriteLog(2, "MS_Init_Update从数据库加载跨服掠夺数据失败", null, true);
					return;
				}
			}
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
			if (!(timeOfWeekNow < kuaFuLueDuoConfig.ApplyTimePoints[0]))
			{
				KuaFuLueDuoStateMachine.StateType stateType;
				if (timeOfWeekNow < kuaFuLueDuoConfig.ApplyTimePoints[kuaFuLueDuoConfig.ApplyTimePoints.Count - 1])
				{
					stateType = KuaFuLueDuoStateMachine.StateType.SignUp;
					this.GameState = 1;
				}
				else
				{
					int num = (int)(timeOfWeekNow - kuaFuLueDuoConfig.TimePoints[0]).TotalSeconds;
					if (Consts.TestMode && num >= 0 && num < kuaFuLueDuoConfig.TotalSecs)
					{
						this.GameState = 2;
						stateType = KuaFuLueDuoStateMachine.StateType.PrepareGame;
					}
					else
					{
						if (num >= 0)
						{
							return;
						}
						this.GameState = 2;
						stateType = KuaFuLueDuoStateMachine.StateType.PrepareGame;
					}
				}
				KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
				if (currState != stateType)
				{
					this.StateMachine.SetCurrState(stateType, now, param);
				}
				this.StateMachine.Tag1 = 0L;
				this.StateMachine.Tag2 = TimeSpan.MinValue;
				LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_Init_Update GameState:{0} To:{1} SeasonID:{2} SeasonIDLast:{3} Round:{4}", new object[]
				{
					currState,
					this.GameState,
					this.SeasonID,
					this.LastSeasonID,
					this.SignUpRound
				}), null, true);
			}
		}

		private void MS_SignUp_Update(DateTime now, int param)
		{
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
			int num = -1;
			for (int i = 0; i < kuaFuLueDuoConfig.ApplyTimePoints.Count - 1; i++)
			{
				if (timeOfWeekNow >= kuaFuLueDuoConfig.ApplyTimePoints[i] && timeOfWeekNow < kuaFuLueDuoConfig.ApplyTimePoints[i + 1])
				{
					num = i + 1;
					break;
				}
			}
			if (num < 0)
			{
				num = 5;
				KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
				KuaFuLueDuoStateMachine.StateType stateType = KuaFuLueDuoStateMachine.StateType.PrepareGame;
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_SignUp_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
				{
					currState,
					stateType,
					this.SeasonID,
					this.LastSeasonID,
					this.SignUpRound
				}), null, true);
			}
			if (num != this.SignUpRound)
			{
				this.SignUpRound = num;
				this.GameState = 1;
				int signUpRound = this.Persistence.GetSignUpRound();
				this.Persistence.SaveSignUpRound(this.SignUpRound);
				if (this.SignUpRound == 1 && signUpRound != this.SignUpRound)
				{
					lock (this.Mutex)
					{
						this.LoadDatabase(TimeUtil.NowDateTime(), false);
						foreach (KeyValuePair<int, KuaFuLueDuoGroupItem> keyValuePair in this.RuntimeData.GroupConfigDict)
						{
							int groupID = keyValuePair.Value.GroupID;
							int serverNumber = keyValuePair.Value.ServerNumber;
							KuaFuLueDuoSyncData kuaFuLueDuoSyncData;
							if (!this.SyncDataDict.TryGetValue(groupID, out kuaFuLueDuoSyncData))
							{
								kuaFuLueDuoSyncData = new KuaFuLueDuoSyncData();
								this.SyncDataDict[groupID] = kuaFuLueDuoSyncData;
							}
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (!kuaFuLueDuoSyncData.ServerInfoDict.TryGetValue(serverNumber, out kuaFuLueDuoServerInfo))
							{
								kuaFuLueDuoServerInfo = new KuaFuLueDuoServerInfo
								{
									ServerId = serverNumber
								};
								kuaFuLueDuoServerInfo.ZoneIdRangeList = keyValuePair.Value.ZoneNumber;
								kuaFuLueDuoSyncData.ServerInfoDict[serverNumber] = kuaFuLueDuoServerInfo;
							}
							int num2 = Math.Min(kuaFuLueDuoServerInfo.ZiYuan + this.RuntimeData.CrusadeOre[0], this.RuntimeData.CrusadeOre[1]);
							if (num2 != kuaFuLueDuoServerInfo.ZiYuan)
							{
								kuaFuLueDuoServerInfo.ZiYuan = num2;
								TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.ServerInfoDictAge);
								this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, kuaFuLueDuoServerInfo);
								KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState;
								if (kuaFuLueDuoSyncData.StateList.TryGetValue(serverNumber, out kuaFuLueDuoServerJingJiaState))
								{
									kuaFuLueDuoServerJingJiaState.ZiYuan = kuaFuLueDuoServerInfo.ZiYuan;
								}
							}
						}
					}
				}
				this.UpdateSignUpRound();
			}
		}

		private void UpdateSignUpRound()
		{
			if (this.SignUpRound > 0)
			{
				lock (this.Mutex)
				{
					foreach (KuaFuLueDuoSyncData kuaFuLueDuoSyncData in this.SyncDataDict.Values)
					{
						kuaFuLueDuoSyncData.SignUpRound = this.SignUpRound;
						foreach (KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState in kuaFuLueDuoSyncData.StateList.Values)
						{
							if (kuaFuLueDuoServerJingJiaState.Round < this.SignUpRound)
							{
								kuaFuLueDuoServerJingJiaState.State = 2;
							}
							else if (kuaFuLueDuoServerJingJiaState.Round == this.SignUpRound)
							{
								kuaFuLueDuoServerJingJiaState.State = 1;
							}
							else
							{
								kuaFuLueDuoServerJingJiaState.State = 0;
							}
						}
						TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.StateAge);
					}
				}
			}
		}

		private void MS_PrepareGame_Update(DateTime now, int param)
		{
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			this.GameState = 2;
			KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
			KuaFuLueDuoStateMachine.StateType stateType = KuaFuLueDuoStateMachine.StateType.PrepareGame;
			for (int i = 0; i < kuaFuLueDuoConfig.TimePoints.Count - 1; i += 2)
			{
				TimeSpan timeSpan = timeOfWeekNow - kuaFuLueDuoConfig.TimePoints[i];
				if (timeSpan.TotalSeconds < 0.0)
				{
					this.StateMachine.Tag2 = kuaFuLueDuoConfig.TimePoints[i];
					break;
				}
				if (timeSpan.TotalSeconds <= (double)kuaFuLueDuoConfig.GameSecs)
				{
					stateType = KuaFuLueDuoStateMachine.StateType.GameStart;
					break;
				}
				if (i < kuaFuLueDuoConfig.TimePoints.Count - 1 || timeOfWeekNow < kuaFuLueDuoConfig.TimePoints[i + 1])
				{
					stateType = KuaFuLueDuoStateMachine.StateType.GameStart;
					break;
				}
			}
			if (ClientAgentManager.Instance().IsAnyKfAgentAlive())
			{
				bool flag = false;
				foreach (KuaFuLueDuoSyncData kuaFuLueDuoSyncData in this.SyncDataDict.Values)
				{
					foreach (KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState in kuaFuLueDuoSyncData.StateList.Values)
					{
						KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData = kuaFuLueDuoServerJingJiaState.FuBenData;
						if (kuaFuLueDuoServerJingJiaState.FuBenData == null)
						{
							kuaFuLueDuoFuBenData = new KuaFuLueDuoFuBenData();
							kuaFuLueDuoFuBenData.GameId = (long)TianTiPersistence.Instance.GetNextGameId();
							kuaFuLueDuoFuBenData.DestServerId = kuaFuLueDuoServerJingJiaState.ServerId;
							kuaFuLueDuoFuBenData.LeftZiYuan = kuaFuLueDuoServerJingJiaState.ZiYuan;
							if (kuaFuLueDuoServerJingJiaState.ZiYuan > 0)
							{
								kuaFuLueDuoSyncData.ServerZiYuanDict[kuaFuLueDuoServerJingJiaState.ServerId] = new FightInfo
								{
									ZiYuan = kuaFuLueDuoServerJingJiaState.ZiYuan
								};
							}
							if (kuaFuLueDuoFuBenData.GameId > 0L)
							{
								kuaFuLueDuoServerJingJiaState.FuBenData = kuaFuLueDuoFuBenData;
								kuaFuLueDuoFuBenData.ServerIdList.Add(kuaFuLueDuoServerJingJiaState.ServerId);
								for (int i = 0; i < kuaFuLueDuoServerJingJiaState.JingJiaList.Count; i++)
								{
									KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData = kuaFuLueDuoServerJingJiaState.JingJiaList[i];
									kuaFuLueDuoFuBenData.BhDataList.Add(kuaFuLueDuoBangHuiJingJiaData.BhId);
									int serverIdByZoneId = this.GetServerIdByZoneId(kuaFuLueDuoBangHuiJingJiaData.ZoneId);
									if (!kuaFuLueDuoFuBenData.ServerIdList.Contains(serverIdByZoneId))
									{
										kuaFuLueDuoFuBenData.ServerIdList.Add(serverIdByZoneId);
									}
								}
								if (kuaFuLueDuoServerJingJiaState.JingJiaList.Count == 0)
								{
									kuaFuLueDuoServerJingJiaState.FuBenData.ServerId = kuaFuLueDuoServerJingJiaState.ServerId;
									this.FuBenMgr.FuBenDataDict[kuaFuLueDuoFuBenData.GameId] = kuaFuLueDuoFuBenData;
									flag = true;
								}
							}
						}
						if (kuaFuLueDuoServerJingJiaState.FuBenData != null && kuaFuLueDuoServerJingJiaState.FuBenData.ServerId == 0)
						{
							int num = 0;
							if (!ClientAgentManager.Instance().AssginKfFuben(25, kuaFuLueDuoFuBenData.GameId, 60, out num))
							{
								LogManager.WriteLog(2, string.Format("跨服掠夺分配副本分配游戏服务器失败,serverid={0},bhid={1}", kuaFuLueDuoFuBenData.DestServerId, string.Join<int>(",", kuaFuLueDuoFuBenData.BhDataList)), null, true);
								return;
							}
							kuaFuLueDuoFuBenData.ServerId = num;
							this.FuBenMgr.FuBenDataDict[kuaFuLueDuoFuBenData.GameId] = kuaFuLueDuoFuBenData;
							flag = true;
							LogManager.WriteLog(5, string.Format("跨服掠夺分配副本,gameId={0},serverid={1},dest={3},bhid={2}", new object[]
							{
								kuaFuLueDuoFuBenData.GameId,
								kuaFuLueDuoFuBenData.DestServerId,
								string.Join<int>(",", kuaFuLueDuoFuBenData.BhDataList),
								num
							}), null, true);
						}
					}
					if (flag)
					{
						TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.FuBenStateAge);
					}
				}
				KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
				if (stateType != currState)
				{
					this.StateMachine.SetCurrState(KuaFuLueDuoStateMachine.StateType.NotifyEnter, now, param);
					LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_PrepareGame_Update To:{0} SeasonID:{1}", KuaFuLueDuoStateMachine.StateType.NotifyEnter, this.SeasonID), null, true);
				}
			}
		}

		private void MS_NotifyEnter_Update(DateTime now, int param)
		{
			KuaFuLueDuoStateMachine.StateType stateType = KuaFuLueDuoStateMachine.StateType.GameStart;
			this.GameState = 2;
			KuaFuLueDuoFuBenMgrData fuBenMgr = this.FuBenMgr;
			foreach (KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData in fuBenMgr.FuBenDataDict.Values)
			{
				KuaFuLueDuoNtfEnterData kuaFuLueDuoNtfEnterData = new KuaFuLueDuoNtfEnterData();
				kuaFuLueDuoNtfEnterData.BhIdList.AddRange(kuaFuLueDuoFuBenData.BhDataList);
				ClientAgentManager.Instance().BroadCastAsyncEvent(1, new AsyncDataItem(10029, new object[]
				{
					kuaFuLueDuoNtfEnterData
				}), 0);
			}
			KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
			this.StateMachine.SetCurrState(stateType, now, param);
			LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_PrepareGame_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
			{
				currState,
				stateType,
				this.SeasonID,
				this.LastSeasonID,
				this.SignUpRound
			}), null, true);
		}

		private void MS_GameStart_Update(DateTime now, int param)
		{
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			if (!(timeOfWeekNow < this.StateMachine.Tag2))
			{
				this.GameState = 3;
				KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.GetKuaFuLueDuoConfig(param);
				KuaFuLueDuoStateMachine.StateType stateType = KuaFuLueDuoStateMachine.StateType.GameStart;
				for (int i = 0; i < kuaFuLueDuoConfig.TimePoints.Count - 1; i += 2)
				{
					if ((timeOfWeekNow - kuaFuLueDuoConfig.TimePoints[i]).TotalSeconds > (double)kuaFuLueDuoConfig.GameSecs && (i < kuaFuLueDuoConfig.TimePoints.Count - 1 || timeOfWeekNow < kuaFuLueDuoConfig.TimePoints[i + 1]))
					{
						stateType = KuaFuLueDuoStateMachine.StateType.RankAnalyse;
						break;
					}
				}
				KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
				if (stateType != currState)
				{
					this.StateMachine.Tag2 = TimeSpan.MaxValue;
					this.StateMachine.SetCurrState(stateType, now, param);
					LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_GameStart_Update GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
					{
						currState,
						stateType,
						this.SeasonID,
						this.LastSeasonID,
						this.SignUpRound
					}), null, true);
				}
			}
		}

		private void HandleUnCompleteFuBenData()
		{
			foreach (KeyValuePair<long, KuaFuLueDuoFuBenData> keyValuePair in this.FuBenMgr.FuBenDataDict)
			{
				KuaFuLueDuoFuBenData value = keyValuePair.Value;
				if (value.State < 3)
				{
					KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
					if (!this.RuntimeData.GroupConfigDict.TryGetValue(value.DestServerId, out kuaFuLueDuoGroupItem))
					{
						continue;
					}
					KuaFuLueDuoSyncData kuaFuLueDuoSyncData;
					if (!this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData))
					{
						continue;
					}
					KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
					if (kuaFuLueDuoSyncData.ServerInfoDict.TryGetValue(value.DestServerId, out kuaFuLueDuoServerInfo))
					{
						kuaFuLueDuoServerInfo.LastZiYuan = 0;
						this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, kuaFuLueDuoServerInfo);
					}
					KuaFuData<KuaFuLueDuoBHData> kuaFuData = null;
					foreach (int key in value.BhDataList)
					{
						if (this.KuaFuLueDuoBHDataDict.TryGetValue(key, out kuaFuData))
						{
							kuaFuData.V.last_ziyuan = 0;
							kuaFuData.V.season = this.SeasonID;
							TimeUtil.AgeByNow(ref kuaFuData.Age);
							this.Persistence.SaveKuaFuLueDuoBHData(kuaFuData.V);
						}
					}
				}
				ClientAgentManager.Instance().RemoveKfFuben(25, value.ServerId, value.GameId);
			}
			this.FuBenMgr.Clear();
		}

		private void MS_RankAnalyse_Enter(DateTime now, int param)
		{
			this.GameState = 4;
			TimeSpan timeOfWeekNow = TimeUtil.GetTimeOfWeekNow2();
			if (timeOfWeekNow < this.StateMachine.Tag2)
			{
				if (this.StateMachine.Tag2 == TimeSpan.MaxValue)
				{
					this.StateMachine.Tag2 = TimeUtil.GetTimeOfWeek2(TimeUtil.NowDateTime().AddSeconds((double)this.RuntimeData.KuaFuLueDuoWaitRankSecs));
				}
			}
			else
			{
				this.HandleUnCompleteFuBenData();
				this.Persistence.DelayWriteDataProc();
				this.SeasonCount++;
				this.Persistence.SaveSeasonID(this.SeasonID);
				this.LoadDatabase(TimeUtil.NowDateTime(), true);
				this.GameState = 0;
				KuaFuLueDuoStateMachine.StateType currState = this.StateMachine.GetCurrState();
				KuaFuLueDuoStateMachine.StateType stateType = KuaFuLueDuoStateMachine.StateType.Init;
				this.StateMachine.SetCurrState(stateType, now, param);
				LogManager.WriteLog(5, string.Format("KuaFuLueDuo::MS_RankAnalyse_Enter GameState:{0} To:{1} SeasonID:{2} LastSeasonID:{3} Round:{4}", new object[]
				{
					currState,
					stateType,
					this.SeasonID,
					this.LastSeasonID,
					this.SignUpRound
				}), null, true);
			}
		}

		public KuaFuLueDuoSyncData SyncData_KuaFuLueDuo(byte[] bytes)
		{
			try
			{
				KuaFuLueDuoSyncData kuaFuLueDuoSyncData = DataHelper2.BytesToObject<KuaFuLueDuoSyncData>(bytes, 0, bytes.Length);
				if (null == kuaFuLueDuoSyncData)
				{
					kuaFuLueDuoSyncData = new KuaFuLueDuoSyncData();
				}
				lock (this.Mutex)
				{
					KuaFuLueDuoSyncData kuaFuLueDuoSyncData2 = null;
					kuaFuLueDuoSyncData.SeasonID = this.SeasonID;
					kuaFuLueDuoSyncData.LastSeasonID = this.LastSeasonID;
					if (kuaFuLueDuoSyncData.ServerID < 9000)
					{
						KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
						if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(kuaFuLueDuoSyncData.ServerID, out kuaFuLueDuoGroupItem))
						{
							kuaFuLueDuoSyncData.ServerGameState = this.GameState;
							kuaFuLueDuoSyncData.GameState = -1039;
						}
						else if (!this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData2))
						{
							kuaFuLueDuoSyncData.GameState = 0;
						}
						else
						{
							kuaFuLueDuoSyncData.GroupID = kuaFuLueDuoSyncData2.GroupID;
							kuaFuLueDuoSyncData.GameState = this.GameState;
							kuaFuLueDuoSyncData.ServerGameState = this.GameState;
							if (kuaFuLueDuoSyncData.ServerInfoDictAge != kuaFuLueDuoSyncData2.ServerInfoDictAge)
							{
								kuaFuLueDuoSyncData.ServerInfoDictAge = kuaFuLueDuoSyncData2.ServerInfoDictAge;
								kuaFuLueDuoSyncData.ServerInfoDict = kuaFuLueDuoSyncData2.ServerInfoDict;
							}
							if (kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict.Age != kuaFuLueDuoSyncData2.KuaFuLueDuoRankInfoDict.Age)
							{
								kuaFuLueDuoSyncData.KuaFuLueDuoRankInfoDict = kuaFuLueDuoSyncData2.KuaFuLueDuoRankInfoDict;
							}
							if (kuaFuLueDuoSyncData.StateAge != kuaFuLueDuoSyncData2.StateAge)
							{
								kuaFuLueDuoSyncData.StateAge = kuaFuLueDuoSyncData2.StateAge;
								kuaFuLueDuoSyncData.StateList = kuaFuLueDuoSyncData2.StateList;
								kuaFuLueDuoSyncData.SignUpRound = kuaFuLueDuoSyncData2.SignUpRound;
							}
							if (kuaFuLueDuoSyncData.FuBenStateAge != kuaFuLueDuoSyncData2.FuBenStateAge)
							{
								kuaFuLueDuoSyncData.FuBenStateAge = kuaFuLueDuoSyncData2.FuBenStateAge;
								kuaFuLueDuoSyncData.ServerZiYuanDict = kuaFuLueDuoSyncData2.ServerZiYuanDict;
								kuaFuLueDuoSyncData.BhZiYuanDict = kuaFuLueDuoSyncData2.BhZiYuanDict;
							}
						}
					}
					else
					{
						kuaFuLueDuoSyncData.ServerGameState = this.GameState;
						kuaFuLueDuoSyncData.GameState = -1039;
						if (kuaFuLueDuoSyncData.FuBenStateAge == 1L)
						{
							if (null != kuaFuLueDuoSyncData.ServerZiYuanDict)
							{
								foreach (KeyValuePair<int, FightInfo> keyValuePair in kuaFuLueDuoSyncData.ServerZiYuanDict)
								{
									KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
									if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(keyValuePair.Key, out kuaFuLueDuoGroupItem))
									{
										kuaFuLueDuoSyncData.ServerGameState = this.GameState;
										kuaFuLueDuoSyncData.GameState = -1039;
										break;
									}
									if (!this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData2))
									{
										kuaFuLueDuoSyncData.GameState = 0;
										break;
									}
									kuaFuLueDuoSyncData2.ServerZiYuanDict[keyValuePair.Key] = keyValuePair.Value;
									TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData2.FuBenStateAge);
								}
							}
							if (null != kuaFuLueDuoSyncData.BhZiYuanDict)
							{
								foreach (KeyValuePair<int, FightInfo> keyValuePair in kuaFuLueDuoSyncData.BhZiYuanDict)
								{
									KuaFuData<KuaFuLueDuoBHData> kuaFuData;
									if (this.KuaFuLueDuoBHDataDict.TryGetValue(keyValuePair.Key, out kuaFuData))
									{
										KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
										if (this.RuntimeData.ZoneGroupConfigDict.TryGetValue(kuaFuData.V.zoneid, out kuaFuLueDuoGroupItem))
										{
											if (this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData2))
											{
												kuaFuLueDuoSyncData2.BhZiYuanDict[keyValuePair.Key] = keyValuePair.Value;
												TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData2.FuBenStateAge);
											}
										}
									}
								}
							}
						}
					}
					return kuaFuLueDuoSyncData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.SyncData_KuaFuLueDuo failed!", ex, true);
			}
			return new KuaFuLueDuoSyncData
			{
				GameState = -11003
			};
		}

		public KuaFuLueDuoJingJiaResult JingJia_KuaFuLueDuo(int bhid, int zoneid_bh, string bhname, int ziJin, int serverId, int oldZiJin)
		{
			KuaFuLueDuoJingJiaResult kuaFuLueDuoJingJiaResult = new KuaFuLueDuoJingJiaResult(0, 0, 0);
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				long num = TimeUtil.NOW();
				lock (this.Mutex)
				{
					KuaFuLueDuoConfig kuaFuLueDuoConfig = this.RuntimeData.GetKuaFuLueDuoConfig(0);
					if (null == kuaFuLueDuoConfig)
					{
						return -3;
					}
					KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
					KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem2;
					if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneid_bh, out kuaFuLueDuoGroupItem) || !this.RuntimeData.ZoneGroupConfigDict.TryGetValue(zoneid_bh, out kuaFuLueDuoGroupItem2) || kuaFuLueDuoGroupItem.GroupID != kuaFuLueDuoGroupItem2.GroupID)
					{
						return -1039;
					}
					KuaFuLueDuoSyncData kuaFuLueDuoSyncData = null;
					if (!this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData))
					{
						return -1039;
					}
					if (!Consts.TestMode)
					{
						if (!ClientAgentManager.Instance().IsAgentAlive(serverId))
						{
							return -11000;
						}
					}
					KuaFuData<KuaFuLueDuoBHData> kuaFuData;
					if (!this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out kuaFuData))
					{
						kuaFuData = new KuaFuData<KuaFuLueDuoBHData>();
						kuaFuData.V.bhid = bhid;
						kuaFuData.V.bhname = bhname;
						kuaFuData.V.zoneid = zoneid_bh;
						this.KuaFuLueDuoBHDataDict[bhid] = kuaFuData;
					}
					if (kuaFuData.V.jingjia_sid > 0 && kuaFuData.V.jingjia_sid != serverId)
					{
						return -1004;
					}
					kuaFuLueDuoJingJiaResult.ZiJin = kuaFuData.V.jingjia;
					KuaFuLueDuoServerJingJiaState kuaFuLueDuoServerJingJiaState;
					if (kuaFuData.V.jingjia > oldZiJin)
					{
						kuaFuLueDuoJingJiaResult.Result = -11000;
					}
					else if (!kuaFuLueDuoSyncData.StateList.TryGetValue(serverId, out kuaFuLueDuoServerJingJiaState) || kuaFuLueDuoServerJingJiaState.Round != kuaFuLueDuoSyncData.SignUpRound)
					{
						kuaFuLueDuoJingJiaResult.Result = -1042;
					}
					else if (num < kuaFuData.V.JingJiaTicks)
					{
						kuaFuLueDuoJingJiaResult.Result = -2007;
						kuaFuLueDuoJingJiaResult.CDTicks = kuaFuData.V.JingJiaTicks;
					}
					else
					{
						kuaFuData.V.JingJiaTicks = TimeUtil.NOW() + (long)(this.RuntimeData.CrusadeApplyCD * 1000);
						KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData = kuaFuLueDuoServerJingJiaState.JingJiaList.Find((KuaFuLueDuoBangHuiJingJiaData x) => x.BhId == bhid);
						if (null != kuaFuLueDuoBangHuiJingJiaData)
						{
							kuaFuLueDuoJingJiaResult.ZiJin = kuaFuLueDuoBangHuiJingJiaData.ZiJin;
							kuaFuLueDuoBangHuiJingJiaData.ZiJin = ziJin;
						}
						else
						{
							bool flag2;
							if (kuaFuLueDuoServerJingJiaState.JingJiaList.Count >= this.RuntimeData.CrusadeAttackerNum)
							{
								flag2 = !kuaFuLueDuoServerJingJiaState.JingJiaList.Any((KuaFuLueDuoBangHuiJingJiaData x) => x.ZiJin < ziJin);
							}
							else
							{
								flag2 = false;
							}
							if (flag2)
							{
								kuaFuLueDuoJingJiaResult = -1038;
								return kuaFuLueDuoJingJiaResult;
							}
							kuaFuLueDuoServerJingJiaState.JingJiaList.Add(new KuaFuLueDuoBangHuiJingJiaData
							{
								BhId = bhid,
								BhName = bhname,
								ZoneId = zoneid_bh,
								ServerId = serverId,
								ZiJin = ziJin
							});
							kuaFuLueDuoServerJingJiaState.JingJiaList.Sort();
							kuaFuLueDuoServerJingJiaState.JingJiaList.Reverse();
							for (int i = kuaFuLueDuoServerJingJiaState.JingJiaList.Count - 1; i >= this.RuntimeData.CrusadeAttackerNum; i--)
							{
								KuaFuLueDuoBangHuiJingJiaData kuaFuLueDuoBangHuiJingJiaData2 = kuaFuLueDuoServerJingJiaState.JingJiaList[i];
								kuaFuLueDuoServerJingJiaState.JingJiaList.RemoveAt(i);
								KuaFuData<KuaFuLueDuoBHData> kuaFuData2;
								if (this.KuaFuLueDuoBHDataDict.TryGetValue(kuaFuLueDuoBangHuiJingJiaData2.BhId, out kuaFuData2))
								{
									kuaFuData2.V.jingjia_sid = 0;
									kuaFuData2.V.jingjia = 0;
									kuaFuData2.V.last_jingjia = kuaFuData2.V.jingjia;
									this.Persistence.SaveKuaFuLueDuoBHSeasonData(this.SeasonID, kuaFuData2.V);
								}
							}
						}
						kuaFuData.V.season = this.SeasonID;
						kuaFuData.V.bhname = bhname;
						kuaFuData.V.jingjia = ziJin;
						kuaFuData.V.jingjia_sid = serverId;
						kuaFuData.V.group = kuaFuLueDuoGroupItem.GroupID;
						TimeUtil.AgeByNow(ref kuaFuData.Age);
						TimeUtil.AgeByNow(ref kuaFuLueDuoSyncData.StateAge);
						kuaFuData.V.last_ziyuan = 0;
						this.Persistence.SaveKuaFuLueDuoBHData(kuaFuData.V);
						this.Persistence.SaveKuaFuLueDuoBHSeasonData(this.SeasonID, kuaFuData.V);
					}
				}
				return kuaFuLueDuoJingJiaResult;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.RookieSignUp_KuaFuLueDuo failed!", ex, true);
			}
			return -11003;
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByServerId_KuaFuLueDuo(int serverId)
		{
			KuaFuLueDuoFuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					foreach (KeyValuePair<long, KuaFuLueDuoFuBenData> keyValuePair in this.FuBenMgr.FuBenDataDict)
					{
						if (keyValuePair.Value.DestServerId == serverId)
						{
							result = keyValuePair.Value;
							break;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.GetFuBenDataByServerId_KuaFuLueDuo failed!", ex, true);
			}
			return result;
		}

		public KuaFuLueDuoFuBenData GetFuBenDataByGameId_KuaFuLueDuo(long gameid)
		{
			KuaFuLueDuoFuBenData result = null;
			try
			{
				lock (this.Mutex)
				{
					this.FuBenMgr.FuBenDataDict.TryGetValue(gameid, out result);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.GetFuBenDataByGameId_KuaFuLueDuo failed!", ex, true);
			}
			return result;
		}

		public byte[] GetRoleData_KuaFuLueDuo(long rid)
		{
			byte[] result = null;
			try
			{
				int roleKillNum = this.Persistence.GetRoleKillNum(this.MinSeasonID, rid);
				result = DataHelper2.ObjectToBytes<int>(roleKillNum);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "KuaFuLueDuoService.GetRoleData_KuaFuLueDuo failed!", ex, true);
			}
			return result;
		}

		public KuaFuCmdData GetBHDataByBhid_KuaFuLueDuo(int bhid, long age)
		{
			try
			{
				lock (this.Mutex)
				{
					KuaFuData<KuaFuLueDuoBHData> kuaFuData = null;
					this.KuaFuLueDuoBHDataDict.TryGetValue(bhid, out kuaFuData);
					if (kuaFuData == null)
					{
						return null;
					}
					if (age != kuaFuData.Age)
					{
						return new KuaFuCmdData
						{
							Age = kuaFuData.Age,
							Bytes0 = DataHelper2.ObjectToBytes<KuaFuLueDuoBHData>(kuaFuData.V)
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
				LogManager.WriteLog(2, "KuaFuLueDuoService.GetBHDataByBhid_KuaFuLueDuo failed!", ex, true);
			}
			return null;
		}

		public int GameFuBenComplete_KuaFuLueDuo(KuaFuLueDuoStatisticalData data)
		{
			int result = 0;
			try
			{
				lock (this.Mutex)
				{
					KuaFuLueDuoGroupItem kuaFuLueDuoGroupItem;
					if (!this.RuntimeData.ZoneGroupConfigDict.TryGetValue(data.DestServerID, out kuaFuLueDuoGroupItem))
					{
						return -1039;
					}
					KuaFuLueDuoFuBenData kuaFuLueDuoFuBenData = null;
					if (data.GameId == -2L)
					{
						kuaFuLueDuoFuBenData = new KuaFuLueDuoFuBenData
						{
							ServerId = 9801,
							DestServerId = data.DestServerID
						};
					}
					else
					{
						this.FuBenMgr.FuBenDataDict.TryGetValue(data.GameId, out kuaFuLueDuoFuBenData);
						if (kuaFuLueDuoFuBenData == null || kuaFuLueDuoFuBenData.State == 3)
						{
							return 3;
						}
						kuaFuLueDuoFuBenData.State = 3;
					}
					KuaFuLueDuoSyncData kuaFuLueDuoSyncData;
					if (this.SyncDataDict.TryGetValue(kuaFuLueDuoGroupItem.GroupID, out kuaFuLueDuoSyncData))
					{
						if (data.DestServerID > 0)
						{
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (kuaFuLueDuoSyncData.ServerInfoDict.TryGetValue(data.DestServerID, out kuaFuLueDuoServerInfo))
							{
								kuaFuLueDuoServerInfo.LastZiYuan = Math.Max(kuaFuLueDuoServerInfo.ZiYuan - data.LeftZiYuan, 0);
								kuaFuLueDuoServerInfo.ZiYuan = data.LeftZiYuan;
								this.Persistence.SaveKuaFuLueDuoServerData(this.SeasonID, kuaFuLueDuoServerInfo);
							}
						}
						if (data.SuccessServerID > 0 && data.LeftZiYuan <= 0)
						{
							KuaFuLueDuoServerInfo kuaFuLueDuoServerInfo;
							if (kuaFuLueDuoSyncData.ServerInfoDict.TryGetValue(data.SuccessServerID, out kuaFuLueDuoServerInfo))
							{
								if (null == kuaFuLueDuoServerInfo.ZhengFuList)
								{
									kuaFuLueDuoServerInfo.ZhengFuList = new List<int>();
								}
								if (!kuaFuLueDuoServerInfo.ZhengFuList.Contains(data.DestServerID))
								{
									kuaFuLueDuoServerInfo.ZhengFuList.Add(kuaFuLueDuoFuBenData.DestServerId);
									this.Persistence.SaveKuaFuLueDuoServerRankData(this.SeasonID, kuaFuLueDuoGroupItem.GroupID, kuaFuLueDuoServerInfo, data.DestServerID, data.Percent);
								}
								this.Persistence.SaveKuaFuLueDuoServerZhengFuData(this.SeasonID, kuaFuLueDuoServerInfo);
							}
						}
						foreach (KuaFuLueDuoLueDuoResultData kuaFuLueDuoLueDuoResultData in data.LueDuoResultList)
						{
							KuaFuData<KuaFuLueDuoBHData> kuaFuData;
							if (this.KuaFuLueDuoBHDataDict.TryGetValue(kuaFuLueDuoLueDuoResultData.bhid, out kuaFuData))
							{
								kuaFuData.V.season = this.SeasonID;
								kuaFuData.V.sum_ziyuan += kuaFuLueDuoLueDuoResultData.ziyuan;
								kuaFuData.V.last_ziyuan = kuaFuLueDuoLueDuoResultData.ziyuan;
								if (string.IsNullOrEmpty(kuaFuLueDuoLueDuoResultData.bhname))
								{
									kuaFuLueDuoLueDuoResultData.bhname = kuaFuData.V.bhname;
								}
								else if (kuaFuData.V.bhname != kuaFuLueDuoLueDuoResultData.bhname)
								{
									kuaFuData.V.bhname = kuaFuLueDuoLueDuoResultData.bhname;
								}
								TimeUtil.AgeByNow(ref kuaFuData.Age);
								this.Persistence.SaveKuaFuLueDuoBHData(kuaFuData.V);
							}
						}
						foreach (KuaFuLueDuoRoleData kuaFuLueDuoRoleData in data.roleStatisticalData)
						{
							this.Persistence.SaveKuaFuLueDuoRoleData(this.SeasonID, kuaFuLueDuoRoleData.rid, kuaFuLueDuoRoleData.rname, kuaFuLueDuoRoleData.zoneid, kuaFuLueDuoRoleData.kill);
						}
					}
					return result;
				}
			}
			catch (Exception ex)
			{
				result = -11;
				LogManager.WriteLog(2, "KuaFuLueDuoService.GameFuBenComplete_KuaFuLueDuo failed!", ex, true);
			}
			return result;
		}

		public const GameTypes GameType = 25;

		public const GameTypes NotifyGameType = 1;

		private static KuaFuLueDuoService _instance = new KuaFuLueDuoService();

		private object Mutex = new object();

		private bool Initialized = false;

		private KuaFuLueDuoCommonData RuntimeData = new KuaFuLueDuoCommonData();

		public KuaFuLueDuoPersistence Persistence = KuaFuLueDuoPersistence.Instance;

		private int GameState;

		private DateTime LastDate;

		private Dictionary<int, KuaFuLueDuoSyncData> SyncDataDict = new Dictionary<int, KuaFuLueDuoSyncData>();

		private int SeasonID;

		private int LastSeasonID;

		private int MinSeasonID;

		private int SignUpRound;

		private KuaFuLueDuoFuBenMgrData FuBenMgr = new KuaFuLueDuoFuBenMgrData();

		private KuaFuLueDuoStateMachine StateMachine = new KuaFuLueDuoStateMachine();

		private uint UpdateFrameCount = 0U;

		private DateTime LastUpdateTime = DateTime.MinValue;
	}
}
