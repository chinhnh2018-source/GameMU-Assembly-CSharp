using System;
using System.Collections.Generic;
using GameDBServer.DB;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic.Rank
{
	public class RankCacheManager
	{
		public void PrintfRankData()
		{
			LogManager.WriteLog(LogTypes.Error, "RankDataDict开始输出", null, true);
			lock (this.RankDataDictLock)
			{
				foreach (KeyValuePair<string, RankData> keyValuePair in this.RankDataDict)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("RankDataKey = {0}", keyValuePair.Key), null, true);
					foreach (InputKingPaiHangData inputKingPaiHangData in keyValuePair.Value.RankDataList)
					{
						LogManager.WriteLog(LogTypes.Error, string.Format("rankData 名次={0}, UserID={1}, 数值={2}, 更新时间={3}", new object[]
						{
							inputKingPaiHangData.PaiHang,
							inputKingPaiHangData.UserID,
							inputKingPaiHangData.PaiHangValue,
							inputKingPaiHangData.PaiHangTime
						}), null, true);
					}
				}
			}
			LogManager.WriteLog(LogTypes.Error, "RankDataDict结束输出", null, true);
		}

		public void OnUserDoSomething(int roleID, RankType rankType, int value)
		{
			DBManager instance = DBManager.getInstance();
			if (null != instance)
			{
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleID);
				if (null != dbroleInfo)
				{
					double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
					dbroleInfo.RankValue.AddUserRankValue(rankType, value);
					lock (this.RankDataDictLock)
					{
						foreach (KeyValuePair<string, RankData> keyValuePair in this.RankDataDict)
						{
							RankDataKey keyFromStr = RankDataKey.GetKeyFromStr(keyValuePair.Key);
							if (null != keyFromStr)
							{
								if (rankType == keyFromStr.rankType)
								{
									double offsetSecond2 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.StartDate));
									double offsetSecond3 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.EndDate));
									if (offsetSecond >= offsetSecond2 && offsetSecond <= offsetSecond3)
									{
										bool flag2 = false;
										foreach (InputKingPaiHangData inputKingPaiHangData in keyValuePair.Value.RankDataList)
										{
											if ((RankType.Charge == rankType && inputKingPaiHangData.UserID == dbroleInfo.UserID) || (RankType.Consume == rankType && inputKingPaiHangData.UserID == dbroleInfo.RoleID.ToString()))
											{
												inputKingPaiHangData.PaiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
												inputKingPaiHangData.PaiHangValue += value;
												flag2 = true;
												break;
											}
										}
										if (!flag2)
										{
											int rankValue = dbroleInfo.RankValue.GetRankValue(keyFromStr);
											InputKingPaiHangData item = new InputKingPaiHangData
											{
												UserID = ((RankType.Charge == rankType) ? dbroleInfo.UserID : dbroleInfo.RoleID.ToString()),
												PaiHang = 0,
												PaiHangTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
												PaiHangValue = rankValue
											};
											keyValuePair.Value.RankDataList.Add(item);
										}
										this.BuildRank(keyValuePair.Value);
									}
								}
							}
						}
					}
				}
			}
		}

		public List<InputKingPaiHangData> GetRankDataList(RankData rankData)
		{
			List<InputKingPaiHangData> result;
			lock (this.RankDataDictLock)
			{
				byte[] array = DataHelper.ObjectToBytes<List<InputKingPaiHangData>>(rankData.RankDataList);
				result = DataHelper.BytesToObject<List<InputKingPaiHangData>>(array, 0, array.Length);
			}
			return result;
		}

		public RankData GetRankDataFromCache(RankDataKey key)
		{
			RankData result = null;
			lock (this.RankDataDictLock)
			{
				if (this.RankDataDict.ContainsKey(key.GetKey()))
				{
					result = this.RankDataDict[key.GetKey()];
				}
			}
			return result;
		}

		public RankData GetRankData(RankDataKey key, List<int> minGateValueList, int maxPaiHang)
		{
			DBManager instance = DBManager.getInstance();
			RankData result;
			if (null == instance)
			{
				result = null;
			}
			else
			{
				double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
				RankData rankData = this.GetRankDataFromCache(key);
				if (null != rankData)
				{
					double offsetSecond2 = Global.GetOffsetSecond(DateTime.Parse(key.EndDate));
					if (offsetSecond2 >= offsetSecond)
					{
						return rankData;
					}
					if (rankData.QueryFromDBTime > offsetSecond2)
					{
						return rankData;
					}
				}
				lock (this.RankDataDictLock)
				{
					rankData = this.InitRankData(key, minGateValueList, maxPaiHang);
					this.RankDataDict[key.GetKey()] = rankData;
					result = rankData;
				}
			}
			return result;
		}

		public RankData InitRankData(RankDataKey key, List<int> minGateValueList, int maxPaiHang)
		{
			DBManager instance = DBManager.getInstance();
			RankData result;
			if (null == instance)
			{
				result = null;
			}
			else
			{
				RankData rankData = null;
				if (RankType.Charge == key.rankType)
				{
					rankData = this.GetUserInputRank(instance, key.StartDate, key.EndDate, minGateValueList, maxPaiHang);
				}
				else if (RankType.Consume == key.rankType)
				{
					rankData = this.GetUserConsumeRank(instance, key.StartDate, key.EndDate, minGateValueList, maxPaiHang);
				}
				result = rankData;
			}
			return result;
		}

		private void BuildRank(RankData rankData)
		{
			if (null != rankData)
			{
				rankData.RankDataList.Sort(delegate(InputKingPaiHangData x, InputKingPaiHangData y)
				{
					int result;
					if (y.PaiHangValue == x.PaiHangValue)
					{
						double offsetSecond = Global.GetOffsetSecond(DateTime.Parse(x.PaiHangTime));
						double offsetSecond2 = Global.GetOffsetSecond(DateTime.Parse(y.PaiHangTime));
						result = (int)(offsetSecond - offsetSecond2);
					}
					else
					{
						result = y.PaiHangValue - x.PaiHangValue;
					}
					return result;
				});
				List<InputKingPaiHangData> list = new List<InputKingPaiHangData>();
				if (null != rankData.minGateValueList)
				{
					int num = 0;
					for (int i = 0; i < rankData.RankDataList.Count; i++)
					{
						InputKingPaiHangData inputKingPaiHangData = rankData.RankDataList[i];
						inputKingPaiHangData.PaiHang = -1;
						for (int j = num; j < rankData.minGateValueList.Count; j++)
						{
							if (inputKingPaiHangData.PaiHangValue >= rankData.minGateValueList[j])
							{
								inputKingPaiHangData.PaiHang = j + 1;
								list.Add(inputKingPaiHangData);
								num = inputKingPaiHangData.PaiHang;
								break;
							}
						}
						if (inputKingPaiHangData.PaiHang < 0 || inputKingPaiHangData.PaiHang >= rankData.minGateValueList.Count)
						{
							break;
						}
					}
					rankData.RankDataList = list;
				}
			}
		}

		public RankData GetUserInputRank(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			List<InputKingPaiHangData> userInputPaiHang = DBQuery.GetUserInputPaiHang(dbMgr, fromDate, toDate, maxPaiHang);
			RankData rankData = new RankData();
			rankData.QueryFromDBTime = offsetSecond;
			rankData.MaxRankCount = (double)maxPaiHang;
			rankData.minGateValueList = minGateValueList;
			rankData.RankDataList = userInputPaiHang;
			this.BuildRank(rankData);
			return rankData;
		}

		public RankData GetUserConsumeRank(DBManager dbMgr, string fromDate, string toDate, List<int> minGateValueList, int maxPaiHang)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			List<InputKingPaiHangData> userUsedMoneyPaiHang = DBQuery.GetUserUsedMoneyPaiHang(dbMgr, fromDate, toDate, maxPaiHang);
			RankData rankData = new RankData();
			rankData.QueryFromDBTime = offsetSecond;
			rankData.MaxRankCount = (double)maxPaiHang;
			rankData.minGateValueList = minGateValueList;
			rankData.RankDataList = userUsedMoneyPaiHang;
			this.BuildRank(rankData);
			return rankData;
		}

		private object RankDataDictLock = new object();

		private Dictionary<string, RankData> RankDataDict = new Dictionary<string, RankData>();
	}
}
