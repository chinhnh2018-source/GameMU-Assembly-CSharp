using System;
using System.Collections.Generic;
using GameDBServer.DB;

namespace GameDBServer.Logic.Rank
{
	public class UserRankValueCache
	{
		public void Init(int rid)
		{
			this.roleID = rid;
		}

		public void Clear()
		{
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				this.DictUserRankValue.Clear();
			}
			lock (this.UserChargeMoneyCountDicLock)
			{
				this.ChargeMoneyCountDic.Clear();
			}
		}

		public UserRankValue GetRankValueFromCache(RankDataKey key)
		{
			UserRankValue result = null;
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				if (this.DictUserRankValue.ContainsKey(key.GetKey()))
				{
					result = this.DictUserRankValue[key.GetKey()];
				}
			}
			return result;
		}

		public int AddUserRankValue(RankType ActType, int addValue)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				foreach (KeyValuePair<string, UserRankValue> keyValuePair in this.DictUserRankValue)
				{
					RankDataKey keyFromStr = RankDataKey.GetKeyFromStr(keyValuePair.Key.ToString());
					if (null != keyFromStr)
					{
						double offsetSecond2 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.StartDate));
						double offsetSecond3 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.EndDate));
						if (ActType == keyFromStr.rankType && offsetSecond >= offsetSecond2 && offsetSecond <= offsetSecond3)
						{
							keyValuePair.Value.RankValue += addValue;
						}
					}
				}
			}
			lock (this.UserChargeMoneyCountDicLock)
			{
				foreach (KeyValuePair<string, Dictionary<int, int>> keyValuePair2 in this.ChargeMoneyCountDic)
				{
					RankDataKey keyFromStr = RankDataKey.GetKeyFromStr(keyValuePair2.Key.ToString());
					if (null != keyFromStr)
					{
						double offsetSecond2 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.StartDate));
						double offsetSecond3 = Global.GetOffsetSecond(DateTime.Parse(keyFromStr.EndDate));
						if (ActType == keyFromStr.rankType && offsetSecond >= offsetSecond2 && offsetSecond <= offsetSecond3)
						{
							if (keyValuePair2.Value.ContainsKey(addValue))
							{
								Dictionary<int, int> value;
								(value = keyValuePair2.Value)[addValue] = value[addValue] + 1;
							}
							else
							{
								keyValuePair2.Value[addValue] = 1;
							}
						}
					}
				}
			}
			return 0;
		}

		private UserRankValue GetRankValueStruct(RankDataKey key)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			UserRankValue userRankValue = this.GetRankValueFromCache(key);
			if (null != userRankValue)
			{
				if (userRankValue.EndTime >= offsetSecond)
				{
					return userRankValue;
				}
				if (userRankValue.QueryFromDBTime > userRankValue.EndTime)
				{
					return userRankValue;
				}
			}
			UserRankValue result;
			lock (UserRankValueCache.UserRankValueDictLock)
			{
				userRankValue = this.InitRankValue(key);
				this.DictUserRankValue[key.GetKey()] = userRankValue;
				result = userRankValue;
			}
			return result;
		}

		public int GetRankValue(RankDataKey key)
		{
			UserRankValue rankValueStruct = this.GetRankValueStruct(key);
			return (rankValueStruct == null) ? 0 : rankValueStruct.RankValue;
		}

		public UserRankValue InitRankValue(RankDataKey key)
		{
			DBManager instance = DBManager.getInstance();
			UserRankValue result;
			if (null == instance)
			{
				result = null;
			}
			else
			{
				UserRankValue userRankValue = null;
				if (RankType.Charge == key.rankType)
				{
					DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref this.roleID);
					if (null != dbroleInfo)
					{
						userRankValue = this.GetUserInputRankVaule(instance, dbroleInfo.UserID, dbroleInfo.ZoneID, key.StartDate, key.EndDate);
					}
				}
				else if (RankType.Consume == key.rankType)
				{
					userRankValue = this.GetUserConsumeRankValue(instance, key.StartDate, key.EndDate);
				}
				result = userRankValue;
			}
			return result;
		}

		public UserRankValue GetUserInputRankVaule(DBManager dbMgr, string userid, int zoneid, string fromDate, string toDate)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			int num = DBQuery.GetUserInputMoney(dbMgr, userid, zoneid, fromDate, toDate);
			num = Global.TransMoneyToYuanBao(num);
			return new UserRankValue
			{
				QueryFromDBTime = offsetSecond,
				BeginTime = Global.GetOffsetSecond(DateTime.Parse(fromDate)),
				EndTime = Global.GetOffsetSecond(DateTime.Parse(toDate)),
				RankValue = num
			};
		}

		public Dictionary<int, int> GetUserInputMoneyCount(DBManager dbMgr, string userid, int zoneid, string fromDate, string toDate)
		{
			RankDataKey rankDataKey = new RankDataKey
			{
				rankType = RankType.Charge,
				StartDate = fromDate,
				EndDate = toDate
			};
			Dictionary<int, int> dictionary = null;
			lock (this.UserChargeMoneyCountDicLock)
			{
				if (this.ChargeMoneyCountDic.TryGetValue(rankDataKey.GetKey(), out dictionary))
				{
					return new Dictionary<int, int>(dictionary);
				}
				dictionary = DBQuery.GetUserDanBiInputMoneyCount(dbMgr, userid, zoneid, fromDate, toDate);
				this.ChargeMoneyCountDic[rankDataKey.GetKey()] = dictionary;
			}
			return new Dictionary<int, int>(dictionary);
		}

		public UserRankValue GetUserConsumeRankValue(DBManager dbMgr, string fromDate, string toDate)
		{
			double offsetSecond = Global.GetOffsetSecond(DateTime.Now);
			int userUsedMoney = DBQuery.GetUserUsedMoney(dbMgr, this.roleID, fromDate, toDate);
			return new UserRankValue
			{
				QueryFromDBTime = offsetSecond,
				BeginTime = Global.GetOffsetSecond(DateTime.Parse(fromDate)),
				EndTime = Global.GetOffsetSecond(DateTime.Parse(toDate)),
				RankValue = userUsedMoney
			};
		}

		private int roleID = 0;

		private static object UserRankValueDictLock = new object();

		private object UserChargeMoneyCountDicLock = new object();

		private Dictionary<string, UserRankValue> DictUserRankValue = new Dictionary<string, UserRankValue>();

		public Dictionary<string, Dictionary<int, int>> ChargeMoneyCountDic = new Dictionary<string, Dictionary<int, int>>();
	}
}
