using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting;
using KF.Remoting.Data;
using KF.TcpCall;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace Remoting
{
	public class TianTi5v5Service : IDisposable
	{
		public void Dispose()
		{
		}

		private static int ZhanDuiDataSortCompare(TianTi5v5ZhanDuiData x, TianTi5v5ZhanDuiData y)
		{
			int num = y.DuanWeiJiFen - x.DuanWeiJiFen;
			int result;
			if (num != 0)
			{
				result = num;
			}
			else
			{
				result = x.ZhanDuiID - y.ZhanDuiID;
			}
			return result;
		}

		public static bool InitConfig()
		{
			TianTi5v5Service.RankData.MaxPaiMingRank = 201;
			try
			{
				string fileName = "Config/TeamDuanWeiAward.xml";
				string resourcePath = KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes);
				XElement xelement = ConfigHelper.Load(resourcePath);
				IEnumerable<XElement> enumerable = xelement.Elements();
				foreach (XElement xelement2 in enumerable)
				{
					int val = (int)ConfigHelper.GetElementAttributeValueLong(xelement2, "StarRank", 201L);
					TianTi5v5Service.RankData.MaxPaiMingRank = Math.Max(TianTi5v5Service.RankData.MaxPaiMingRank, val);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			Dictionary<int, TianTi5v5ZhanDuiData> dictionary = new Dictionary<int, TianTi5v5ZhanDuiData>();
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.LoadZhanDuiRankData(TianTi5v5Service.RankData, TimeUtil.NowDateTime());
				if (!TianTi5v5Service.Persistence.LoadZhanDuiData(dictionary))
				{
					return false;
				}
				TianTi5v5Service.ZhanDuiDict = dictionary;
			}
			TianTi5v5Service.pTianTiPiPeiCfg = new List<TianTi5v5Service.PiPeiCfg>
			{
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 0,
					MinTime = 0,
					MaxTime = 5
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 1,
					MinTime = 5,
					MaxTime = 10
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 3,
					MinTime = 10,
					MaxTime = 15
				},
				new TianTi5v5Service.PiPeiCfg
				{
					ID = 25,
					MinTime = 15,
					MaxTime = 60
				}
			};
			return true;
		}

		public static void LoadZhanDuiRankData(TianTi5v5RankData rankData, DateTime now)
		{
			try
			{
				DateTime modifyTime = now;
				List<TianTi5v5ZhanDuiData> list = new List<TianTi5v5ZhanDuiData>();
				List<TianTi5v5ZhanDuiData> list2 = new List<TianTi5v5ZhanDuiData>();
				int offsetDay = TimeUtil.GetOffsetDay(now);
				int offsetMonthDayID = TimeUtil.GetOffsetMonthDayID(now);
				TianTi5v5Service.Persistence.LoadZhanDuiRankList(list2, offsetMonthDayID);
				if (now.Day != 1)
				{
					TianTi5v5Service.Persistence.LoadZhanDuiRankList(list, offsetDay);
				}
				lock (TianTi5v5Service.Mutex)
				{
					rankData.ModifyTime = modifyTime;
					rankData.DayPaiHangList = list;
					rankData.MonthPaiHangList = list2;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
		}

		public static void UpdateZhanDuiRankData(DateTime now, bool forceUpdateMonthRank = false)
		{
			bool flag = now.Day == 1 || forceUpdateMonthRank;
			int offsetDay = TimeUtil.GetOffsetDay(now);
			DateTime monthStartDateTime = new DateTime(now.Year, now.Month, 1);
			if (now.Day == 1)
			{
				monthStartDateTime = monthStartDateTime.AddMonths(-1);
			}
			List<TianTi5v5ZhanDuiData> list = TianTi5v5Service.ZhanDuiDict.Values.ToList<TianTi5v5ZhanDuiData>();
			list.RemoveAll((TianTi5v5ZhanDuiData x) => x.LastFightTime < monthStartDateTime || x.DuanWeiJiFen == 0);
			list.Sort(new Comparison<TianTi5v5ZhanDuiData>(TianTi5v5Service.ZhanDuiDataSortCompare));
			int maxPaiMingRank = TianTi5v5Service.RankData.MaxPaiMingRank;
			List<TianTi5v5ZhanDuiData> list2 = new List<TianTi5v5ZhanDuiData>();
			for (int i = 0; i < list.Count; i++)
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData = list[i];
				if (i < maxPaiMingRank)
				{
					tianTi5v5ZhanDuiData.DuanWeiRank = i + 1;
					if (flag)
					{
						tianTi5v5ZhanDuiData.MonthDuanWeiRank = tianTi5v5ZhanDuiData.DuanWeiRank;
					}
					list2.Add(tianTi5v5ZhanDuiData.Clone());
					if (flag)
					{
						tianTi5v5ZhanDuiData.DuanWeiRank = maxPaiMingRank + 1;
					}
				}
				else
				{
					tianTi5v5ZhanDuiData.DuanWeiRank = maxPaiMingRank + 1;
					if (flag)
					{
						tianTi5v5ZhanDuiData.MonthDuanWeiRank = tianTi5v5ZhanDuiData.DuanWeiRank;
					}
				}
			}
			TianTi5v5Service.Persistence.UpdateZhanDuiRankData(list, offsetDay, TianTi5v5Service.RankData.MaxPaiMingRank, flag);
			TianTi5v5Service.Persistence.UpdateZhanDuiDayRank(list2, offsetDay, TianTi5v5Service.RankData.MaxPaiMingRank, flag);
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.RankData.ModifyTime = now;
				if (now.Day == 1)
				{
					TianTi5v5Service.RankData.MonthPaiHangList = list2;
					TianTi5v5Service.RankData.DayPaiHangList = new List<TianTi5v5ZhanDuiData>();
				}
				else
				{
					TianTi5v5Service.RankData.DayPaiHangList = list;
				}
			}
		}

		public static void PaiHangCopy(DateTime now)
		{
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.RankData.ModifyTime = now;
				TianTi5v5Service.RankData.MonthPaiHangList = TianTi5v5Service.RankData.DayPaiHangList;
			}
		}

		public static int CreateZhanDui(int serverID, TianTi5v5ZhanDuiData pData)
		{
			int num = pData.ZhanDuiID;
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -3;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					if (pData.ZhanDuiID == 0)
					{
						if (TianTi5v5Service.ZhanDuiDict.Values.Any((TianTi5v5ZhanDuiData x) => StringComparer.CurrentCultureIgnoreCase.Compare(x.ZhanDuiName, pData.ZhanDuiName) == 0))
						{
							return -4023;
						}
						num = TianTi5v5Service.Persistence.InitZhanDui(pData);
						if (num <= 0)
						{
							return -15;
						}
						pData.ZhanDuiID = num;
						TianTi5v5Service.ZhanDuiDict[pData.ZhanDuiID] = pData;
						return num;
					}
					else
					{
						TianTi5v5Service.Persistence.UpdateZhanDui(pData);
						TianTi5v5Service.ZhanDuiDict[pData.ZhanDuiID] = pData;
					}
				}
				result = num;
			}
			return result;
		}

		public static int UpdateZhanDuiXuanYan(long teamID, string xuanYan)
		{
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -1;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
					if (!TianTi5v5Service.ZhanDuiDict.TryGetValue((int)teamID, out tianTi5v5ZhanDuiData))
					{
						return -15;
					}
					tianTi5v5ZhanDuiData.XuanYan = xuanYan;
				}
				TianTi5v5Service.Persistence.UpdateZhanDuiXuanYan(teamID, xuanYan);
				result = 0;
			}
			return result;
		}

		public static int UpdateZhanDuiData(TianTi5v5ZhanDuiData data, ZhanDuiDataModeTypes modeType)
		{
			int result = 0;
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(data.ZhanDuiID, out tianTi5v5ZhanDuiData))
				{
					if (modeType == 1)
					{
						tianTi5v5ZhanDuiData.ZhanDuiID = data.ZhanDuiID;
						tianTi5v5ZhanDuiData.XuanYan = data.XuanYan;
						tianTi5v5ZhanDuiData.ZhanDuiName = data.ZhanDuiName;
						tianTi5v5ZhanDuiData.LeaderRoleID = data.LeaderRoleID;
						tianTi5v5ZhanDuiData.ZhanDouLi = data.ZhanDouLi;
						tianTi5v5ZhanDuiData.teamerList = data.teamerList;
						tianTi5v5ZhanDuiData.TeamerRidList = data.TeamerRidList;
						tianTi5v5ZhanDuiData.LeaderRoleName = data.LeaderRoleName;
						tianTi5v5ZhanDuiData.ZoneID = data.ZoneID;
					}
					else if (modeType == 0)
					{
						tianTi5v5ZhanDuiData.DuanWeiId = data.DuanWeiId;
						tianTi5v5ZhanDuiData.DuanWeiJiFen = data.DuanWeiJiFen;
						tianTi5v5ZhanDuiData.DuanWeiRank = data.DuanWeiRank;
						tianTi5v5ZhanDuiData.ZhanDouLi = data.ZhanDouLi;
						tianTi5v5ZhanDuiData.LianSheng = data.LianSheng;
						tianTi5v5ZhanDuiData.SuccessCount = data.SuccessCount;
						tianTi5v5ZhanDuiData.FightCount = data.FightCount;
						tianTi5v5ZhanDuiData.MonthDuanWeiRank = data.MonthDuanWeiRank;
						tianTi5v5ZhanDuiData.LastFightTime = data.LastFightTime;
						using (List<TianTi5v5ZhanDuiRoleData>.Enumerator enumerator = tianTi5v5ZhanDuiData.teamerList.GetEnumerator())
						{
							while (enumerator.MoveNext())
							{
								TianTi5v5ZhanDuiRoleData role = enumerator.Current;
								TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData = data.teamerList.Find((TianTi5v5ZhanDuiRoleData x) => x.RoleID == role.RoleID);
								if (null != tianTi5v5ZhanDuiRoleData)
								{
									role.MonthFightCounts = tianTi5v5ZhanDuiRoleData.MonthFightCounts;
									role.TodayFightCount = tianTi5v5ZhanDuiRoleData.TodayFightCount;
									role.MonthFigntCount = tianTi5v5ZhanDuiRoleData.MonthFigntCount;
									role.ZhanLi = tianTi5v5ZhanDuiRoleData.ZhanLi;
									role.RoleOcc = tianTi5v5ZhanDuiRoleData.RoleOcc;
									role.ZhuanSheng = tianTi5v5ZhanDuiRoleData.ZhuanSheng;
									role.Level = tianTi5v5ZhanDuiRoleData.Level;
									role.RebornLevel = tianTi5v5ZhanDuiRoleData.RebornLevel;
									role.ModelData = tianTi5v5ZhanDuiRoleData.ModelData;
								}
							}
						}
					}
					result = TianTi5v5Service.Persistence.UpdateZhanDui(tianTi5v5ZhanDuiData);
				}
			}
			return result;
		}

		public static void UpdateZorkZhanDuiData(TianTi5v5ZhanDuiData data)
		{
			TianTi5v5Service.Persistence.UpdateZorkZhanDui(data);
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(data.ZhanDuiID, out tianTi5v5ZhanDuiData))
				{
					tianTi5v5ZhanDuiData.ZorkJiFen = data.ZorkJiFen;
					tianTi5v5ZhanDuiData.ZorkLastFightTime = data.ZorkLastFightTime;
				}
			}
		}

		public static TianTi5v5ZhanDuiData GetZhanDuiData(int zhanDuiID)
		{
			TianTi5v5ZhanDuiData result = null;
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out result);
			}
			return result;
		}

		public static void CalZorkBattleRankTeamJiFen(List<KFZorkRankInfo> rankList)
		{
			lock (TianTi5v5Service.Mutex)
			{
				List<TianTi5v5ZhanDuiData> list = TianTi5v5Service.ZhanDuiDict.Values.ToList<TianTi5v5ZhanDuiData>();
				if (list.Count != 0)
				{
					list.RemoveAll((TianTi5v5ZhanDuiData x) => x.ZorkJiFen == 0);
					list.Sort(delegate(TianTi5v5ZhanDuiData left, TianTi5v5ZhanDuiData right)
					{
						int result;
						if (left.ZorkJiFen > right.ZorkJiFen)
						{
							result = -1;
						}
						else if (left.ZorkJiFen < right.ZorkJiFen)
						{
							result = 1;
						}
						else if (left.ZorkLastFightTime > right.ZorkLastFightTime)
						{
							result = 1;
						}
						else if (left.ZorkLastFightTime < right.ZorkLastFightTime)
						{
							result = -1;
						}
						else if (left.ZhanDuiID > right.ZhanDuiID)
						{
							result = -1;
						}
						else if (left.ZhanDuiID < right.ZhanDuiID)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					list = list.GetRange(0, Math.Min(30, list.Count));
					foreach (TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData in list)
					{
						KFZorkRankInfo item = new KFZorkRankInfo
						{
							Key = tianTi5v5ZhanDuiData.ZhanDuiID,
							Value = tianTi5v5ZhanDuiData.ZorkJiFen,
							StrParam1 = KuaFuServerManager.FormatName(tianTi5v5ZhanDuiData.ZoneID, tianTi5v5ZhanDuiData.ZhanDuiName)
						};
						rankList.Add(item);
					}
				}
			}
		}

		public static void ClearAllZhanDuiZorkData()
		{
			lock (TianTi5v5Service.Mutex)
			{
				foreach (TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData in TianTi5v5Service.ZhanDuiDict.Values)
				{
					if (tianTi5v5ZhanDuiData.ZorkJiFen > 0)
					{
						tianTi5v5ZhanDuiData.ZorkJiFen = 0;
						TianTi5v5Service.Persistence.UpdateZorkZhanDui(tianTi5v5ZhanDuiData);
					}
				}
				TianTi5v5Service.Persistence.ClearZorkBattleRoleData();
			}
		}

		public static int DeleteZhanDui(int serverID, int roleid, int zhanDuiID)
		{
			int result;
			if (!TianTi5v5Service.ThreadInit)
			{
				result = -100;
			}
			else
			{
				lock (TianTi5v5Service.Mutex)
				{
					TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
					if (!TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out tianTi5v5ZhanDuiData))
					{
						result = 0;
					}
					else
					{
						int num = TianTi5v5Service.Persistence.DeleteZhanDui((long)zhanDuiID);
						if (num >= 0)
						{
							LogManager.WriteLog(0, string.Format("DeleteZhanDui,zhanduiid={0},roleid={1},teamName={2}", zhanDuiID, roleid, tianTi5v5ZhanDuiData.ZhanDuiName), null, true);
							TianTi5v5Service.ZhanDuiDict.Remove(zhanDuiID);
						}
						result = num;
					}
				}
			}
			return result;
		}

		public static TianTi5v5RankData ZhanDuiGetRankingData(DateTime modifyTime)
		{
			TianTi5v5RankData tianTi5v5RankData = new TianTi5v5RankData();
			lock (TianTi5v5Service.Mutex)
			{
				tianTi5v5RankData.ModifyTime = TianTi5v5Service.RankData.ModifyTime;
				tianTi5v5RankData.MaxPaiMingRank = TianTi5v5Service.RankData.MaxPaiMingRank;
				if (modifyTime < TianTi5v5Service.RankData.ModifyTime && null != TianTi5v5Service.RankData.DayPaiHangList)
				{
					tianTi5v5RankData.DayPaiHangList = new List<TianTi5v5ZhanDuiData>(TianTi5v5Service.RankData.DayPaiHangList);
				}
				if (modifyTime < TianTi5v5Service.RankData.ModifyTime && null != TianTi5v5Service.RankData.MonthPaiHangList)
				{
					tianTi5v5RankData.MonthPaiHangList = new List<TianTi5v5ZhanDuiData>(TianTi5v5Service.RankData.MonthPaiHangList);
				}
			}
			return tianTi5v5RankData;
		}

		public static List<int> GetZhanDuiMemberIDs(int zhanDuiID)
		{
			List<int> result = new List<int>();
			lock (TianTi5v5Service.Mutex)
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(zhanDuiID, out tianTi5v5ZhanDuiData))
				{
					result = tianTi5v5ZhanDuiData.teamerList.ConvertAll<int>((TianTi5v5ZhanDuiRoleData x) => x.RoleID);
				}
			}
			return result;
		}

		public static int ZhanDuiRoleSignUp(int serverId, int gameType, int zhanDuiID, long zhanLi = 1L, int groupIndex = 1)
		{
			int result = 1;
			Lazy<KF5v5PiPeiTeam> lazy = new Lazy<KF5v5PiPeiTeam>(() => new KF5v5PiPeiTeam
			{
				TeamID = zhanDuiID,
				GroupIndex = groupIndex,
				ServerID = serverId,
				ZhanDouLi = zhanLi,
				StateEndTicks = TimeUtil.NOW()
			});
			KF5v5PiPeiTeam orAdd = TianTi5v5Service.PiPeiDict.GetOrAdd(zhanDuiID, (int x) => lazy.Value);
			int num = 0;
			lock (orAdd)
			{
				num = orAdd.GameId;
				orAdd.GameId = 0;
				orAdd.State = 1;
				orAdd.ServerID = serverId;
				orAdd.ZhanDouLi = zhanLi;
				orAdd.GroupIndex = groupIndex;
				orAdd.StateEndTicks = Global.NowTime.Ticks;
				int randomNumber = Global.GetRandomNumber(0, 10);
				LogManager.WriteLog(0, string.Format("组队竞技战队随机延时,zhanduiid={0}, duanwei={1},time={2}", zhanDuiID, groupIndex, randomNumber), null, true);
			}
			if (num > 0)
			{
				TianTi5v5Service.RemoveRoleFromFuBen(num, zhanDuiID);
			}
			return result;
		}

		public static int ZhanDuiRoleChangeState(int serverId, int zhanDuiID, int roleId, int state, int gameID)
		{
			KF5v5PiPeiTeam kf5v5PiPeiTeam;
			if (TianTi5v5Service.PiPeiDict.TryGetValue(zhanDuiID, out kf5v5PiPeiTeam))
			{
				if (state == 6 || state == 0)
				{
					int num = 0;
					lock (kf5v5PiPeiTeam)
					{
						num = kf5v5PiPeiTeam.GameId;
						kf5v5PiPeiTeam.GameId = 0;
						kf5v5PiPeiTeam.State = 0;
						kf5v5PiPeiTeam.ServerID = serverId;
						kf5v5PiPeiTeam.StateEndTicks = Global.NowTime.Ticks;
					}
					if (num > 0)
					{
						TianTi5v5Service.RemoveRoleFromFuBen(num, zhanDuiID);
					}
				}
			}
			return 0;
		}

		public static KuaFu5v5FuBenData ZhanDuiGetFuBenData(int gameId)
		{
			KuaFu5v5FuBenData kuaFu5v5FuBenData = null;
			KuaFu5v5FuBenData result;
			if (TianTi5v5Service.FuBenDataDict.TryGetValue(gameId, out kuaFu5v5FuBenData) && kuaFu5v5FuBenData.State < 3)
			{
				result = kuaFu5v5FuBenData;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public static void ClearRolePairFightCount()
		{
			lock (TianTi5v5Service.RolePairFightCountDict)
			{
				TianTi5v5Service.RolePairFightCountDict.Clear();
			}
		}

		public static void AddRolePairFightCount(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			int num = 0;
			int num2 = 0;
			if (KuaFu5v5FuBenData.ZhanDuiDict.Count >= 2)
			{
				foreach (int num3 in KuaFu5v5FuBenData.ZhanDuiDict.Keys)
				{
					if (num == 0)
					{
						num = num3;
					}
					else
					{
						num2 = num3;
					}
				}
				long key = ListExt.MakeRolePairKey(num, num2);
				lock (TianTi5v5Service.RolePairFightCountDict)
				{
					int num4;
					if (!TianTi5v5Service.RolePairFightCountDict.TryGetValue(key, out num4))
					{
						TianTi5v5Service.RolePairFightCountDict[key] = 1;
					}
					else
					{
						TianTi5v5Service.RolePairFightCountDict[key] = num4 + 1;
					}
				}
			}
		}

		public static bool CanAddFuBenRole(int x, int y)
		{
			long key = ListExt.MakeRolePairKey(x, y);
			lock (TianTi5v5Service.RolePairFightCountDict)
			{
				int num;
				if (!TianTi5v5Service.RolePairFightCountDict.TryGetValue(key, out num) || num < TianTi5v5Service.Persistence.MaxRolePairFightCount)
				{
					return true;
				}
			}
			return false;
		}

		public static RangeKey GetAssignRange(int baseValue, long startTicks, long waitTicks1, long waitTicks3, long waitTicksAll)
		{
			int num;
			if (startTicks > waitTicks3)
			{
				if (startTicks > waitTicks1)
				{
					num = 0;
				}
				else
				{
					num = 1;
				}
			}
			else if (startTicks > waitTicksAll)
			{
				num = 2;
			}
			else
			{
				num = 3;
			}
			int num2 = TianTi5v5Service.AssignRangeArray[num];
			return new RangeKey(baseValue - num2, baseValue + num2, null);
		}

		public static void ThreadProc(object state)
		{
			if (TianTi5v5Service.ThreadInit)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					Global.UpdateNowTime(dateTime);
					if (dateTime > TianTi5v5Service.CheckRoleTimerProcTime)
					{
						TianTi5v5Service.CheckRoleTimerProcTime = dateTime.AddSeconds(1.428);
						int signUpCount;
						int startCount;
						TianTi5v5Service.CheckRoleTimerProc(dateTime, out signUpCount, out startCount);
						ClientAgentManager.Instance().SetGameTypeLoad(TianTi5v5Service.GameType, signUpCount, startCount);
					}
					if (dateTime > TianTi5v5Service.SaveServerStateProcTime)
					{
						TianTi5v5Service.SaveServerStateProcTime = dateTime.AddSeconds(30.0);
						if (dateTime.Hour >= 3 && dateTime.Hour < 4)
						{
							TianTi5v5Service.ClearRolePairFightCount();
							TianTi5v5Service.UpdateZhanDuiRankData(dateTime, false);
							ZhanDuiZhengBa_K.LoadSyncData(dateTime, false);
						}
					}
					if (dateTime > TianTi5v5Service.CheckGameFuBenTime)
					{
						TianTi5v5Service.CheckGameFuBenTime = dateTime.AddSeconds(1000.0);
						TianTi5v5Service.CheckGameFuBenTimerProc(dateTime);
					}
					TianTi5v5Service.Persistence.DelayWriteDataProc();
				}
				catch (Exception ex)
				{
					LogManager.WriteExceptionUseCache(ex.ToString());
				}
			}
		}

		private static void CheckRoleTimerProc(DateTime now, out int signUpCnt, out int startCount)
		{
			signUpCnt = 0;
			startCount = 0;
			bool flag = true;
			long ticks = now.AddHours(-1.0).Ticks;
			long ticks2 = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecs1)).Ticks;
			long ticks3 = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecs3)).Ticks;
			long ticks4 = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.SignUpWaitSecsAll)).Ticks;
			long ticks5 = now.AddSeconds((double)(-(double)TianTi5v5Service.Persistence.WaitForJoinMaxSecs)).Ticks;
			TianTi5v5Service.ProcessPiPeiList.Clear();
			List<int> list = new List<int>();
			foreach (KF5v5PiPeiTeam kf5v5PiPeiTeam in TianTi5v5Service.PiPeiDict.Values)
			{
				int num = 0;
				lock (kf5v5PiPeiTeam)
				{
					if (kf5v5PiPeiTeam.State == 0 || kf5v5PiPeiTeam.State > 5)
					{
						if (kf5v5PiPeiTeam.StateEndTicks < ticks)
						{
							kf5v5PiPeiTeam.State = 0;
							list.Add(kf5v5PiPeiTeam.TeamID);
							continue;
						}
					}
					else if (kf5v5PiPeiTeam.State == 3 || kf5v5PiPeiTeam.State == 4)
					{
						if (kf5v5PiPeiTeam.StateEndTicks < now.Ticks)
						{
							kf5v5PiPeiTeam.State = 0;
							list.Add(kf5v5PiPeiTeam.TeamID);
							num = kf5v5PiPeiTeam.GameId;
						}
					}
					else if (kf5v5PiPeiTeam.State == 1)
					{
						if (kf5v5PiPeiTeam.StateEndTicks < ticks5)
						{
							kf5v5PiPeiTeam.State = 0;
							list.Add(kf5v5PiPeiTeam.TeamID);
							continue;
						}
					}
				}
				if (kf5v5PiPeiTeam.State == 1)
				{
					signUpCnt++;
					if (flag)
					{
						RangeKey assignRange = TianTi5v5Service.GetAssignRange(kf5v5PiPeiTeam.GroupIndex, kf5v5PiPeiTeam.StateEndTicks, ticks2, ticks3, ticks4);
						flag = TianTi5v5Service.AssignGameFuben(kf5v5PiPeiTeam, assignRange, now);
					}
				}
				else if (kf5v5PiPeiTeam.State == 2)
				{
					signUpCnt++;
				}
				else if (kf5v5PiPeiTeam.State == 5)
				{
					startCount++;
				}
				if (num > 0)
				{
					TianTi5v5Service.RemoveRoleFromFuBen(num, kf5v5PiPeiTeam.TeamID);
				}
			}
			foreach (int key in list)
			{
				KF5v5PiPeiTeam kf5v5PiPeiTeam2;
				if (TianTi5v5Service.PiPeiDict.TryRemove(key, out kf5v5PiPeiTeam2))
				{
					if (kf5v5PiPeiTeam2.State == 1)
					{
						TianTi5v5Service.PiPeiDict.TryAdd(key, kf5v5PiPeiTeam2);
					}
				}
			}
		}

		public static bool AssignGameFuben(KF5v5PiPeiTeam kuaFuRoleData, RangeKey range, DateTime now)
		{
			int num = 0;
			DateTime dateTime = now.AddSeconds((double)TianTi5v5Service.EnterGameSecs);
			List<KuaFuFuBenRoleData> list = new List<KuaFuFuBenRoleData>();
			KuaFu5v5FuBenData kuaFu5v5FuBenData = new KuaFu5v5FuBenData();
			int side = 0;
			int removeZhanDuiIDFromPiPeiList = 0;
			KF5v5PiPeiTeam kf5v5PiPeiTeam = kuaFuRoleData;
			if (Consts.TianTiRoleCountTotal > 1)
			{
				foreach (Tuple<KF5v5PiPeiTeam, int, int, int> tuple in TianTi5v5Service.ProcessPiPeiList)
				{
					if (tuple.Item2 >= range.Left && tuple.Item2 <= range.Right)
					{
						if (kuaFuRoleData.GroupIndex >= tuple.Item3 && kuaFuRoleData.GroupIndex <= tuple.Item4)
						{
							if (TianTi5v5Service.CanAddFuBenRole(kuaFuRoleData.TeamID, tuple.Item1.TeamID))
							{
								removeZhanDuiIDFromPiPeiList = tuple.Item1.TeamID;
								kf5v5PiPeiTeam = tuple.Item1;
								if (kuaFu5v5FuBenData.AddZhanDui(kf5v5PiPeiTeam.TeamID, ref num, ref side))
								{
									TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
									if (TianTi5v5Service.ZhanDuiDict.TryGetValue(kf5v5PiPeiTeam.TeamID, out tianTi5v5ZhanDuiData))
									{
										foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in tianTi5v5ZhanDuiData.teamerList)
										{
											KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
											{
												ServerId = kf5v5PiPeiTeam.ServerID,
												RoleId = tianTi5v5ZhanDuiRoleData.RoleID,
												Side = side
											};
											kuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, kf5v5PiPeiTeam.TeamID);
										}
									}
								}
							}
						}
					}
				}
				if (removeZhanDuiIDFromPiPeiList == 0)
				{
					TianTi5v5Service.ProcessPiPeiList.Add(new Tuple<KF5v5PiPeiTeam, int, int, int>(kuaFuRoleData, kuaFuRoleData.GroupIndex, range.Left, range.Right));
					return true;
				}
				TianTi5v5Service.ProcessPiPeiList.RemoveAll((Tuple<KF5v5PiPeiTeam, int, int, int> x) => x.Item1.TeamID == removeZhanDuiIDFromPiPeiList);
				TianTi5v5Service.ProcessPiPeiList.RemoveAll((Tuple<KF5v5PiPeiTeam, int, int, int> x) => x.Item1.TeamID == kuaFuRoleData.TeamID);
			}
			kf5v5PiPeiTeam = kuaFuRoleData;
			if (kuaFu5v5FuBenData.AddZhanDui(kf5v5PiPeiTeam.TeamID, ref num, ref side))
			{
				TianTi5v5ZhanDuiData tianTi5v5ZhanDuiData;
				if (TianTi5v5Service.ZhanDuiDict.TryGetValue(kf5v5PiPeiTeam.TeamID, out tianTi5v5ZhanDuiData))
				{
					foreach (TianTi5v5ZhanDuiRoleData tianTi5v5ZhanDuiRoleData in tianTi5v5ZhanDuiData.teamerList)
					{
						KuaFuFuBenRoleData kuaFuFuBenRoleData = new KuaFuFuBenRoleData
						{
							ServerId = kf5v5PiPeiTeam.ServerID,
							RoleId = tianTi5v5ZhanDuiRoleData.RoleID,
							Side = side
						};
						kuaFu5v5FuBenData.AddKuaFuFuBenRoleData(kuaFuFuBenRoleData, kf5v5PiPeiTeam.TeamID);
					}
				}
			}
			try
			{
				int num2 = 0;
				int nextGameId = TianTi5v5Service.Persistence.GetNextGameId();
				bool flag = ClientAgentManager.Instance().AssginKfFuben(TianTi5v5Service.GameType, (long)nextGameId, num, out num2);
				if (flag)
				{
					kuaFu5v5FuBenData.ServerId = num2;
					kuaFu5v5FuBenData.GameId = nextGameId;
					kuaFu5v5FuBenData.GameType = TianTi5v5Service.GameType;
					kuaFu5v5FuBenData.EndTime = Global.NowTime.AddMinutes(8.0);
					kuaFu5v5FuBenData.LoginInfo = KuaFuServerManager.GetKuaFuLoginInfo(kuaFuRoleData.ServerID, num2);
					TianTi5v5Service.AddGameFuBen(kuaFu5v5FuBenData);
					TianTi5v5Service.Persistence.LogCreateTianTiFuBen(kuaFu5v5FuBenData.GameId, kuaFu5v5FuBenData.ServerId, 0, num);
					foreach (int key in kuaFu5v5FuBenData.ZhanDuiDict.Keys)
					{
						KF5v5PiPeiTeam kf5v5PiPeiTeam2;
						if (TianTi5v5Service.PiPeiDict.TryGetValue(key, out kf5v5PiPeiTeam2))
						{
							kf5v5PiPeiTeam2.State = 3;
							kf5v5PiPeiTeam2.StateEndTicks = dateTime.Ticks;
							kf5v5PiPeiTeam2.GameId = kuaFu5v5FuBenData.GameId;
						}
					}
					kuaFu5v5FuBenData.State = 2;
					TianTi5v5Service.NotifyFuBenRoleEnterGame(kuaFu5v5FuBenData);
					TianTi5v5Service.AddRolePairFightCount(kuaFu5v5FuBenData);
					return true;
				}
				return false;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return false;
		}

		public static void NotifyFuBenRoleEnterGame(KuaFu5v5FuBenData fuBenData)
		{
			try
			{
				lock (fuBenData)
				{
					List<int> list = new List<int>();
					foreach (KuaFuFuBenRoleData kuaFuFuBenRoleData in fuBenData.RoleDict.Values)
					{
						if (!list.Contains(kuaFuFuBenRoleData.ServerId))
						{
							list.Add(kuaFuFuBenRoleData.ServerId);
							AsyncDataItem evItem = new AsyncDataItem(10033, new object[]
							{
								fuBenData
							});
							ClientAgentManager.Instance().PostAsyncEvent(kuaFuFuBenRoleData.ServerId, TianTi5v5Service.ChannelGameType, evItem);
						}
					}
				}
			}
			catch (Exception ex)
			{
			}
		}

		public static void AddGameFuBen(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			TianTi5v5Service.FuBenDataDict[KuaFu5v5FuBenData.GameId] = KuaFu5v5FuBenData;
		}

		public static void RemoveGameFuBen(KuaFu5v5FuBenData KuaFu5v5FuBenData)
		{
			int gameId = KuaFu5v5FuBenData.GameId;
			KuaFu5v5FuBenData kuaFu5v5FuBenData;
			if (TianTi5v5Service.FuBenDataDict.TryRemove(gameId, out kuaFu5v5FuBenData))
			{
				kuaFu5v5FuBenData.State = 3;
			}
			ClientAgentManager.Instance().RemoveKfFuben(TianTi5v5Service.GameType, KuaFu5v5FuBenData.ServerId, (long)KuaFu5v5FuBenData.GameId);
			lock (KuaFu5v5FuBenData)
			{
				foreach (KeyValuePair<int, int> keyValuePair in KuaFu5v5FuBenData.ZhanDuiDict)
				{
					KF5v5PiPeiTeam kf5v5PiPeiTeam;
					if (TianTi5v5Service.PiPeiDict.TryGetValue(keyValuePair.Key, out kf5v5PiPeiTeam))
					{
						if (kf5v5PiPeiTeam.GameId == gameId && kf5v5PiPeiTeam.State >= 3)
						{
							kf5v5PiPeiTeam.State = 0;
						}
					}
				}
			}
		}

		public static void RemoveRoleFromFuBen(int gameId, int zhanDuiID)
		{
			if (gameId > 0)
			{
				KuaFu5v5FuBenData kuaFu5v5FuBenData;
				if (TianTi5v5Service.FuBenDataDict.TryGetValue(gameId, out kuaFu5v5FuBenData))
				{
					lock (kuaFu5v5FuBenData)
					{
						kuaFu5v5FuBenData.State = 3;
						int num = kuaFu5v5FuBenData.RemoveKuaFuFuBenZhanDui(zhanDuiID);
						if (kuaFu5v5FuBenData.CanRemove())
						{
							TianTi5v5Service.RemoveGameFuBen(kuaFu5v5FuBenData);
						}
					}
				}
			}
		}

		public static int GetZhanLiAssignRangeID(long startTicks, DateTime now)
		{
			long num = Math.Max(0L, now.Ticks - startTicks);
			int result;
			if (null == TianTi5v5Service.pTianTiPiPeiCfg)
			{
				result = -1;
			}
			else
			{
				foreach (TianTi5v5Service.PiPeiCfg piPeiCfg in TianTi5v5Service.pTianTiPiPeiCfg)
				{
					if (num >= (long)(piPeiCfg.MinTime * 1000) * 10000L && num < (long)(piPeiCfg.MaxTime * 1000) * 10000L)
					{
						return piPeiCfg.ID;
					}
				}
				result = -1;
			}
			return result;
		}

		private static void CheckGameFuBenTimerProc(DateTime now)
		{
			if (TianTi5v5Service.FuBenDataDict.Count > 0)
			{
				DateTime t = now.AddMinutes(-40.0);
				foreach (KuaFu5v5FuBenData kuaFu5v5FuBenData in TianTi5v5Service.FuBenDataDict.Values)
				{
					lock (kuaFu5v5FuBenData)
					{
						if (kuaFu5v5FuBenData.CanRemove())
						{
							TianTi5v5Service.RemoveGameFuBen(kuaFu5v5FuBenData);
						}
						else if (kuaFu5v5FuBenData.EndTime < t)
						{
							TianTi5v5Service.RemoveGameFuBen(kuaFu5v5FuBenData);
						}
					}
				}
			}
		}

		public static int UpdateEscapeZhanDui(int zhanDuiID, int jiFen, DateTime fightTime)
		{
			TianTi5v5ZhanDuiData zhanDuiData = TianTi5v5Service.GetZhanDuiData(zhanDuiID);
			int result;
			if (zhanDuiData != null)
			{
				if (jiFen != -2147483648)
				{
					zhanDuiData.EscapeJiFen = jiFen;
				}
				zhanDuiData.EscapeLastFightTime = fightTime;
				TianTi5v5Service.Persistence.UpdateEscapeZhanDui(zhanDuiData);
				result = zhanDuiData.EscapeJiFen;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private const double CheckGameFuBenInterval = 1000.0;

		private const double CheckRoleTimerProcInterval = 1.428;

		private const double SaveServerStateProcInterval = 30.0;

		private static object Mutex = new object();

		private static bool ThreadInit = true;

		private static GameTypes GameType = 34;

		private static GameTypes ChannelGameType = 2;

		private static int EnterGameSecs = 30;

		private static DateTime CheckGameFuBenTime;

		private static DateTime CheckRoleTimerProcTime;

		private static DateTime SaveServerStateProcTime;

		private static List<Tuple<KF5v5PiPeiTeam, int, int, int>> ProcessPiPeiList = new List<Tuple<KF5v5PiPeiTeam, int, int, int>>();

		public static int[] AssignRangeArray = new int[]
		{
			0,
			1,
			2,
			100
		};

		public static int[][] AssignRangeArray2 = new int[][]
		{
			new int[]
			{
				0,
				1,
				2,
				100
			}
		};

		public static List<TianTi5v5Service.PiPeiCfg> pTianTiPiPeiCfg = new List<TianTi5v5Service.PiPeiCfg>();

		private static SortedList<long, int> RolePairFightCountDict = new SortedList<long, int>();

		public static ConcurrentDictionary<int, KF5v5PiPeiTeam> PiPeiDict = new ConcurrentDictionary<int, KF5v5PiPeiTeam>();

		private static ConcurrentDictionary<int, KuaFu5v5FuBenData> FuBenDataDict = new ConcurrentDictionary<int, KuaFu5v5FuBenData>(1, 4096);

		private static Dictionary<int, TianTi5v5ZhanDuiData> ZhanDuiDict = new Dictionary<int, TianTi5v5ZhanDuiData>();

		private static Dictionary<int, TianTi5v5ZhanDuiRoleData> ZhanDuiRoleDataDict = new Dictionary<int, TianTi5v5ZhanDuiRoleData>();

		private static TianTi5v5RankData RankData = new TianTi5v5RankData();

		public static TianTiPersistence Persistence = TianTiPersistence.Instance;

		public class PiPeiCfg
		{
			public int ID;

			public int MinTime;

			public int MaxTime;

			public int Zhanli;

			public int PiPeiType;

			public double dPiPeiPercent;
		}
	}
}
