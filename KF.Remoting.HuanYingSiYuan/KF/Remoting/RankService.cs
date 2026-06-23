using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class RankService
	{
		private RankService()
		{
		}

		public static RankService getInstance()
		{
			return RankService._Instance;
		}

		public long RankVersion(int serverID)
		{
			long result;
			if (!this.IsAgent(serverID))
			{
				result = 0L;
			}
			else
			{
				result = this._persistence.DataVersion;
			}
			return result;
		}

		public bool IsAgent(int serverID)
		{
			bool flag = ClientAgentManager.Instance().ExistAgent(serverID);
			if (!flag)
			{
			}
			return flag;
		}

		public int RankGradeUpdate(int serverID, KFRankData newData)
		{
			int result;
			lock (this._lockRank)
			{
				if (newData.RankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				RankInfo rankInfo;
				if (!this._rankInfoDic.TryGetValue(newData.RankType, out rankInfo))
				{
					result = 0;
				}
				else
				{
					if (!this._newRankDic.ContainsKey(newData.RankType))
					{
						this._newRankDic.TryAdd(newData.RankType, new List<KFRankData>());
					}
					if (!this._newRankIndexDic.ContainsKey(newData.RankType))
					{
						this._newRankIndexDic.TryAdd(newData.RankType, new Dictionary<int, int>());
					}
					List<KFRankData> list = this._newRankDic[newData.RankType];
					Dictionary<int, int> dictionary = this._newRankIndexDic[newData.RankType];
					List<KFRankData> list2 = this._newRankTopDic[newData.RankType];
					KFRankData kfrankData;
					if (!dictionary.ContainsKey(newData.RoleID))
					{
						kfrankData = newData;
						kfrankData.Rank = list.Count;
						list.Add(kfrankData);
						dictionary.Add(kfrankData.RoleID, kfrankData.Rank);
						if (rankInfo.RankTopCount > list2.Count)
						{
							list2.Add(kfrankData);
						}
					}
					else
					{
						int index = dictionary[newData.RoleID];
						kfrankData = list[index];
						kfrankData.GradeOld = kfrankData.Grade;
						kfrankData.Grade = newData.Grade;
						kfrankData.ServerID = newData.ServerID;
						kfrankData.RoleData = newData.RoleData;
					}
					this._persistence.DBRankDataUpdate(kfrankData);
					if (this._rankIsSortDic.ContainsKey(rankInfo.RankType))
					{
						this._rankIsSortDic[rankInfo.RankType] = true;
					}
					else
					{
						this._rankIsSortDic.TryAdd(rankInfo.RankType, true);
					}
					if (rankInfo.RankRefreshSpanType != 1)
					{
						ClientAgentManager.Instance().PostAsyncEvent(serverID, this._gameType, new AsyncDataItem(10026, new object[]
						{
							kfrankData
						}));
					}
					result = 1;
				}
			}
			return result;
		}

		public List<KFRankData> FilterRankTopList(List<KFRankData> _RankTopList, RankInfo info)
		{
			List<KFRankData> result;
			if (_RankTopList.Count <= info.RankRoleCount)
			{
				result = _RankTopList;
			}
			else
			{
				List<KFRankData> list = new List<KFRankData>();
				for (int i = 0; i < _RankTopList.Count; i++)
				{
					if (i < info.RankRoleCount)
					{
						list.Add(_RankTopList[i]);
					}
					else
					{
						KFRankData kfrankData = new KFRankData();
						kfrankData.Clone(_RankTopList[i]);
						kfrankData.RoleData = null;
						list.Add(kfrankData);
					}
				}
				result = list;
			}
			return result;
		}

		public List<KFRankData> RankTopList(int serverID, int rankType)
		{
			List<KFRankData> result;
			lock (this._lockRank)
			{
				if (rankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				RankInfo rankInfo;
				if (!this._rankInfoDic.TryGetValue(rankType, out rankInfo))
				{
					result = null;
				}
				else
				{
					List<KFRankData> rankTopList;
					if (rankInfo.RankListType == 1)
					{
						if (!this._oldRankTopDic.ContainsKey(rankType))
						{
							return null;
						}
						rankTopList = this._oldRankTopDic[rankType];
					}
					else
					{
						if (!this._newRankTopDic.ContainsKey(rankType))
						{
							return null;
						}
						rankTopList = this._newRankTopDic[rankType];
					}
					result = this.FilterRankTopList(rankTopList, rankInfo);
				}
			}
			return result;
		}

		public KFRankData RankRole(int serverID, int rankType, int roleID)
		{
			KFRankData result;
			lock (this._lockRank)
			{
				RankInfo rankInfo;
				if (!this._rankInfoDic.TryGetValue(rankType, out rankInfo))
				{
					result = null;
				}
				else
				{
					ConcurrentDictionary<int, List<KFRankData>> concurrentDictionary;
					ConcurrentDictionary<int, Dictionary<int, int>> concurrentDictionary2;
					if (rankInfo.RankListType == 1)
					{
						concurrentDictionary = this._oldRankDic;
						concurrentDictionary2 = this._oldRankIndexDic;
					}
					else
					{
						concurrentDictionary = this._newRankDic;
						concurrentDictionary2 = this._newRankIndexDic;
					}
					if (concurrentDictionary == null || !concurrentDictionary.ContainsKey(rankType))
					{
						result = null;
					}
					else if (concurrentDictionary2 == null || !concurrentDictionary2.ContainsKey(rankType))
					{
						result = null;
					}
					else
					{
						List<KFRankData> list = concurrentDictionary[rankType];
						Dictionary<int, int> dictionary = concurrentDictionary2[rankType];
						if (!dictionary.ContainsKey(roleID))
						{
							result = null;
						}
						else
						{
							int num = dictionary[roleID];
							if (num >= list.Count)
							{
								result = null;
							}
							else
							{
								result = list[num];
							}
						}
					}
				}
			}
			return result;
		}

		public void StartUp()
		{
			try
			{
				this.InitRankConfig();
				this.InitRankDB();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "RankService.StartUp failed!", ex, true);
			}
		}

		private void InitRankConfig()
		{
			this._rankInfoDic.Clear();
			this._rankInfoDic.TryAdd(1, new RankInfo
			{
				RankType = 1,
				RankMax = 50000,
				RankTopCount = 100,
				RankRoleCount = 3,
				RankListType = 1,
				RankRefreshSpanType = 1,
				RankRefreshTime = DateTime.Parse(string.Format("{0}-{1}-{2} {3}", new object[]
				{
					TimeUtil.NowDateTime().Year,
					TimeUtil.NowDateTime().Month,
					TimeUtil.NowDateTime().Day,
					"23:59:59"
				})),
				RankRefreshSecondTick = 0
			});
			string paramValueByName = KuaFuServerManager.systemParamsList.GetParamValueByName("AoYunTime");
			if (!string.IsNullOrEmpty(paramValueByName))
			{
				string[] array = paramValueByName.Split(new char[]
				{
					','
				});
				if (array != null && array.Length == 2)
				{
					DateTime.TryParse(array[0], out this.timeOlympicsBegin);
					DateTime.TryParse(array[1], out this.timeOlympicsEnd);
				}
			}
		}

		private void JudgeClearOlympicsActivityData()
		{
			DateTime t = TimeUtil.NowDateTime();
			if (!(DateTime.MinValue == this.timeOlympicsBegin) && !(t < this.timeOlympicsBegin.AddSeconds(-300.0)))
			{
				int num = this.timeOlympicsBegin.Year * 10000 + this.timeOlympicsBegin.Month * 100 + this.timeOlympicsBegin.Day;
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + 33);
				if (null == single)
				{
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 33, num));
				}
				else if ((int)single != num)
				{
					int num2 = 1;
					this._persistence.DBRankDelByType(num2);
					if (this._newRankDic.ContainsKey(num2))
					{
						this._newRankDic[num2].Clear();
					}
					else
					{
						this._newRankDic.TryAdd(num2, new List<KFRankData>());
					}
					if (this._newRankIndexDic.ContainsKey(num2))
					{
						this._newRankIndexDic[num2].Clear();
					}
					else
					{
						this._newRankIndexDic.TryAdd(num2, new Dictionary<int, int>());
					}
					if (this._newRankTopDic.ContainsKey(num2))
					{
						this._newRankTopDic[num2].Clear();
					}
					else
					{
						this._newRankTopDic.TryAdd(num2, new List<KFRankData>());
					}
					this.RankSort(num2);
					DbHelperMySQL.ExecuteSql(string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 33, num));
				}
			}
		}

		private void InitRankDB()
		{
			this._newRankDic.Clear();
			this._oldRankDic.Clear();
			this._newRankIndexDic.Clear();
			this._oldRankIndexDic.Clear();
			this._newRankTopDic.Clear();
			this._oldRankTopDic.Clear();
			foreach (RankInfo rankInfo in this._rankInfoDic.Values)
			{
				if (rankInfo.RankType == 1)
				{
					this.JudgeClearOlympicsActivityData();
				}
				List<KFRankData> value = this._persistence.DBRankLoad(rankInfo.RankType, rankInfo.RankMax);
				this._newRankDic.TryAdd(rankInfo.RankType, value);
				this._rankIsSortDic.TryAdd(rankInfo.RankType, true);
				this.RankSort(rankInfo.RankType);
			}
		}

		public void Update()
		{
			try
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (!this._isUpdate && (dateTime - this._lastUpdateTime).TotalMilliseconds >= 1000.0)
				{
					this._isUpdate = true;
					foreach (RankInfo rankInfo in this._rankInfoDic.Values)
					{
						if (!(dateTime < rankInfo.RankRefreshTime))
						{
							this.RankSort(rankInfo.RankType);
							if (rankInfo.RankRefreshSpanType == 1)
							{
								rankInfo.RankRefreshTime = rankInfo.RankRefreshTime.AddDays(1.0);
							}
							else if (rankInfo.RankRefreshSpanType == 2)
							{
								rankInfo.RankRefreshTime = rankInfo.RankRefreshTime.AddSeconds((double)rankInfo.RankRefreshSecondTick);
							}
						}
					}
					this._isUpdate = false;
					this._lastUpdateTime = dateTime;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "RankService.Update failed!", ex, true);
			}
		}

		private void RankSort(int rankType)
		{
			lock (this._lockRank)
			{
				bool flag2 = false;
				if (this._rankIsSortDic.TryGetValue(rankType, out flag2))
				{
					if (flag2)
					{
						this._rankIsSortDic[rankType] = false;
						RankInfo rankInfo;
						if (this._rankInfoDic.TryGetValue(rankType, out rankInfo))
						{
							List<KFRankData> list = null;
							if (this._newRankDic.TryGetValue(rankType, out list))
							{
								if (list != null)
								{
									list.Sort();
									List<KFRankData> list2 = null;
									Dictionary<int, int> dictionary = null;
									List<KFRankData> list3 = null;
									if (this._newRankIndexDic.ContainsKey(rankType))
									{
										this._newRankIndexDic[rankType].Clear();
									}
									else
									{
										this._newRankIndexDic.TryAdd(rankType, new Dictionary<int, int>());
									}
									if (this._newRankTopDic.ContainsKey(rankType))
									{
										this._newRankTopDic[rankType].Clear();
									}
									else
									{
										this._newRankTopDic.TryAdd(rankType, new List<KFRankData>());
									}
									if (rankInfo.RankListType == 1)
									{
										if (this._oldRankDic.ContainsKey(rankType))
										{
											this._oldRankDic[rankType].Clear();
										}
										else
										{
											this._oldRankDic.TryAdd(rankType, new List<KFRankData>());
										}
										if (this._oldRankIndexDic.ContainsKey(rankType))
										{
											this._oldRankIndexDic[rankType].Clear();
										}
										else
										{
											this._oldRankIndexDic.TryAdd(rankType, new Dictionary<int, int>());
										}
										if (this._oldRankTopDic.ContainsKey(rankType))
										{
											this._oldRankTopDic[rankType].Clear();
										}
										else
										{
											this._oldRankTopDic.TryAdd(rankType, new List<KFRankData>());
										}
										list2 = this._oldRankDic[rankType];
										dictionary = this._oldRankIndexDic[rankType];
										list3 = this._oldRankTopDic[rankType];
									}
									int num = 0;
									Dictionary<int, int> dictionary2 = this._newRankIndexDic[rankType];
									List<KFRankData> list4 = this._newRankTopDic[rankType];
									bool flag3 = false;
									List<KFRankData> list5 = new List<KFRankData>();
									foreach (KFRankData kfrankData in list)
									{
										kfrankData.RankOld = kfrankData.Rank;
										num = (kfrankData.Rank = num + 1);
										if (kfrankData.RankOld != kfrankData.Rank || kfrankData.GradeOld != kfrankData.Grade)
										{
											if (this.IsAgent(kfrankData.ServerID) && rankInfo.RankRefreshSpanType != 1)
											{
												ClientAgentManager.Instance().PostAsyncEvent(kfrankData.ServerID, this._gameType, new AsyncDataItem(10026, new object[]
												{
													kfrankData
												}));
											}
											if (num <= rankInfo.RankTopCount)
											{
												flag3 = true;
											}
										}
										if ((double)num > (double)rankInfo.RankMax * 1.2)
										{
											list5.Add(kfrankData);
										}
										else
										{
											dictionary2.Add(kfrankData.RoleID, num - 1);
											if (num <= rankInfo.RankTopCount)
											{
												list4.Add(kfrankData);
											}
											if (rankInfo.RankListType == 1)
											{
												KFRankData kfrankData2 = new KFRankData();
												kfrankData2.Clone(kfrankData);
												list2.Add(kfrankData2);
												dictionary.Add(kfrankData2.RoleID, num - 1);
												if (num <= rankInfo.RankTopCount)
												{
													list3.Add(kfrankData2);
												}
											}
										}
									}
									if (flag3)
									{
										List<KFRankData> list6 = this.FilterRankTopList(list4, rankInfo);
										ClientAgentManager.Instance().BroadCastAsyncEvent(this._gameType, new AsyncDataItem(10027, new object[]
										{
											rankType,
											rankInfo.RankRefreshSpanType,
											list6
										}), 0);
									}
									this._persistence.DBRankUpdate(list);
									if (list5.Count > 0)
									{
										foreach (KFRankData item in list5)
										{
											list.Remove(item);
										}
										this._persistence.DBRankDelMore(rankInfo.RankType, (int)((double)rankInfo.RankMax * 1.2));
									}
								}
							}
						}
					}
				}
			}
		}

		private static readonly RankService _Instance = new RankService();

		private object _lockRank = new object();

		public readonly GameTypes _gameType = 14;

		private DateTime timeOlympicsBegin = DateTime.MinValue;

		private DateTime timeOlympicsEnd = DateTime.MinValue;

		private RankPersistence _persistence = RankPersistence.Instance;

		private ConcurrentDictionary<int, List<KFRankData>> _oldRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		private ConcurrentDictionary<int, List<KFRankData>> _newRankDic = new ConcurrentDictionary<int, List<KFRankData>>();

		private ConcurrentDictionary<int, Dictionary<int, int>> _oldRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		private ConcurrentDictionary<int, Dictionary<int, int>> _newRankIndexDic = new ConcurrentDictionary<int, Dictionary<int, int>>();

		private ConcurrentDictionary<int, List<KFRankData>> _oldRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		private ConcurrentDictionary<int, List<KFRankData>> _newRankTopDic = new ConcurrentDictionary<int, List<KFRankData>>();

		private ConcurrentDictionary<int, bool> _rankIsSortDic = new ConcurrentDictionary<int, bool>();

		private ConcurrentDictionary<int, RankInfo> _rankInfoDic = new ConcurrentDictionary<int, RankInfo>();

		private DateTime _lastUpdateTime = DateTime.MinValue;

		private bool _isUpdate = false;
	}
}
