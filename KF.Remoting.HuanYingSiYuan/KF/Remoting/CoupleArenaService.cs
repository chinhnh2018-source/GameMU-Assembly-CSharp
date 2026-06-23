using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
	public class CoupleArenaService
	{
		private CoupleArenaService()
		{
		}

		public static CoupleArenaService getInstance()
		{
			return CoupleArenaService._Instance;
		}

		public void StartUp()
		{
			try
			{
				string fileName = "Config\\CoupleWar.xml";
				XElement xelement = XElement.Load(KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes));
				int i;
				using (IEnumerator<XElement> enumerator = xelement.Elements().GetEnumerator())
				{
					if (enumerator.MoveNext())
					{
						XElement xelement2 = enumerator.Current;
						string[] array = xelement2.Attribute("TimePoints").Value.Split(new char[]
						{
							',',
							'-',
							'|'
						});
						for (i = 0; i < array.Length; i += 3)
						{
							_CoupleArenaWarTimePoint coupleArenaWarTimePoint = new _CoupleArenaWarTimePoint();
							coupleArenaWarTimePoint.Weekday = Convert.ToInt32(array[i]);
							if (coupleArenaWarTimePoint.Weekday < 1 || coupleArenaWarTimePoint.Weekday > 7)
							{
								throw new Exception("weekday error!");
							}
							coupleArenaWarTimePoint.DayStartTicks = DateTime.Parse(array[i + 1]).TimeOfDay.Ticks;
							coupleArenaWarTimePoint.DayEndTicks = DateTime.Parse(array[i + 2]).TimeOfDay.Ticks;
							this._WarTimePointList.Add(coupleArenaWarTimePoint);
						}
						this._WarTimePointList.Sort((_CoupleArenaWarTimePoint _l, _CoupleArenaWarTimePoint _r) => _l.Weekday - _r.Weekday);
					}
				}
				fileName = "Config\\CoupleDuanWei.xml";
				xelement = XElement.Load(KuaFuServerManager.GetResourcePath(fileName, KuaFuServerManager.ResourcePathTypes.GameRes));
				foreach (XElement xelement2 in xelement.Elements())
				{
					_CoupleArenaDuanWeiCfg coupleArenaDuanWeiCfg = new _CoupleArenaDuanWeiCfg();
					XElement xelement2;
					coupleArenaDuanWeiCfg.NeedJiFen = Convert.ToInt32(xelement2.Attribute("NeedCoupleDuanWeiJiFen").Value.ToString());
					coupleArenaDuanWeiCfg.DuanWeiType = Convert.ToInt32(xelement2.Attribute("Type").Value.ToString());
					coupleArenaDuanWeiCfg.DuanWeiLevel = Convert.ToInt32(xelement2.Attribute("Level").Value.ToString());
					coupleArenaDuanWeiCfg.WinJiFen = Convert.ToInt32(xelement2.Attribute("WinJiFen").Value.ToString());
					coupleArenaDuanWeiCfg.LoseJiFen = Convert.ToInt32(xelement2.Attribute("LoseJiFen").Value.ToString());
					this._DuanWeiCfgList.Add(coupleArenaDuanWeiCfg);
				}
				this._DuanWeiCfgList.Sort((_CoupleArenaDuanWeiCfg _l, _CoupleArenaDuanWeiCfg _r) => _l.NeedJiFen - _r.NeedJiFen);
				DateTime dateTime = TimeUtil.NowDateTime();
				this.Persistence.CheckClearRank(this.CurrRankWeek(dateTime));
				this.SyncData.RankList = this.Persistence.LoadRankFromDb();
				this.SyncData.BuildRoleDict();
				this.SyncData.ModifyTime = dateTime;
				this.IsNeedSort = false;
				i = 1;
				while (i < this.SyncData.RankList.Count && !this.IsNeedSort)
				{
					this.IsNeedSort |= (this.SyncData.RankList[i].CompareTo(this.SyncData.RankList[i - 1]) < 0);
					this.IsNeedSort |= (this.SyncData.RankList[i].Rank != this.SyncData.RankList[i - 1].Rank + 1);
					i++;
				}
				this.CheckRebuildRank(dateTime);
				this.CheckFlushRank2Db();
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "CoupleArenaService.InitConfig failed!", ex, true);
			}
		}

		public int CoupleArenaJoin(int roleId1, int roleId2, int serverId)
		{
			int result;
			lock (this.Mutex)
			{
				if (!this.IsValidCoupleIfExist(roleId1, roleId2))
				{
					result = -11003;
				}
				else if (this.JoinDataUtil.GetJoinData(roleId1) != null || this.JoinDataUtil.GetJoinData(roleId2) != null)
				{
					result = -12;
				}
				else
				{
					CoupleArenaJoinData coupleArenaJoinData = this.JoinDataUtil.Create();
					coupleArenaJoinData.ServerId = serverId;
					coupleArenaJoinData.RoleId1 = roleId1;
					coupleArenaJoinData.RoleId2 = roleId2;
					coupleArenaJoinData.StartTime = TimeUtil.NowDateTime();
					CoupleArenaCoupleDataK coupleArenaCoupleDataK;
					if (this.SyncData.RoleDict.TryGetValue(roleId1, out coupleArenaCoupleDataK))
					{
						coupleArenaJoinData.DuanWeiLevel = coupleArenaCoupleDataK.DuanWeiLevel;
						coupleArenaJoinData.DuanWeiType = coupleArenaCoupleDataK.DuanWeiType;
					}
					else
					{
						coupleArenaJoinData.DuanWeiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
						coupleArenaJoinData.DuanWeiType = this._DuanWeiCfgList[0].DuanWeiType;
					}
					this.JoinDataUtil.AddJoinData(coupleArenaJoinData);
					result = 1;
				}
			}
			return result;
		}

		public int CoupleArenaQuit(int roleId1, int roleId2)
		{
			int result;
			lock (this.Mutex)
			{
				if (this.IsValidCoupleIfExist(roleId1, roleId2))
				{
					this.JoinDataUtil.DelJoinData(this.JoinDataUtil.GetJoinData(roleId1));
					this.JoinDataUtil.DelJoinData(this.JoinDataUtil.GetJoinData(roleId2));
				}
				result = 1;
			}
			return result;
		}

		public CoupleArenaSyncData CoupleArenaSync(DateTime lastSyncTime)
		{
			CoupleArenaSyncData result;
			lock (this.Mutex)
			{
				if (lastSyncTime == this.SyncData.ModifyTime)
				{
					result = null;
				}
				else if (!TimeUtil.RandomDispatchTime(lastSyncTime, TimeUtil.NowDateTime(), 180, 60, 10))
				{
					result = null;
				}
				else
				{
					result = new CoupleArenaSyncData
					{
						RankList = new List<CoupleArenaCoupleDataK>(this.SyncData.RankList),
						RoleDict = null,
						ModifyTime = this.SyncData.ModifyTime
					};
				}
			}
			return result;
		}

		public int CoupleArenaPreDivorce(int roleId1, int roleId2)
		{
			bool flag = false;
			int result;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				DateTime dateTime = TimeUtil.NowDateTime();
				if (!this.IsValidCoupleIfExist(roleId1, roleId2))
				{
					if (!this.IsInWeekRangeActTimes(dateTime))
					{
						CoupleArenaCoupleDataK coupleArenaCoupleDataK;
						this.SyncData.RoleDict.TryGetValue(roleId1, out coupleArenaCoupleDataK);
						CoupleArenaCoupleDataK coupleArenaCoupleDataK2;
						this.SyncData.RoleDict.TryGetValue(roleId2, out coupleArenaCoupleDataK2);
						if (coupleArenaCoupleDataK != null && coupleArenaCoupleDataK.IsDivorced == 1)
						{
							return 1;
						}
						if (coupleArenaCoupleDataK2 != null && coupleArenaCoupleDataK2.IsDivorced == 1)
						{
							return 1;
						}
					}
					result = -11003;
				}
				else
				{
					this.CoupleArenaQuit(roleId1, roleId2);
					CoupleArenaCoupleDataK data = null;
					if (!this.IsInWeekRangeActTimes(dateTime))
					{
						data = null;
						if (this.SyncData.RoleDict.TryGetValue(roleId1, out data))
						{
							data.IsDivorced = 1;
							this.Persistence.WriteCoupleData(data);
							if (data.Rank == 1)
							{
								this.SyncData.ModifyTime = dateTime;
							}
						}
						result = 1;
					}
					else
					{
						data = null;
						if (!this.SyncData.RoleDict.TryGetValue(roleId1, out data))
						{
							this.DivorceRecord.Add(roleId1, roleId2);
							result = 1;
						}
						else if (data == null)
						{
							result = 1;
						}
						else if (!this.Persistence.ClearCoupleData(data.Db_CoupleId))
						{
							result = -15;
						}
						else
						{
							if (data.Rank - 1 >= 0 && data.Rank - 1 < this.SyncData.RankList.Count && this.SyncData.RankList[data.Rank - 1].Db_CoupleId == data.Db_CoupleId)
							{
								this.SyncData.RankList.RemoveAt(data.Rank - 1);
							}
							else
							{
								this.SyncData.RankList.RemoveAll((CoupleArenaCoupleDataK _r) => _r.Db_CoupleId == data.Db_CoupleId);
							}
							this.DivorceRecord.Add(roleId1, roleId2);
							this.SyncData.BuildRoleDict();
							this.SyncData.ModifyTime = dateTime;
							this.IsNeedSort = true;
							result = 1;
						}
					}
				}
			}
			finally
			{
				if (flag)
				{
					object mutex;
					Monitor.Exit(mutex);
				}
			}
			return result;
		}

		public CoupleArenaFuBenData GetCoupleFuBenData(long gameId)
		{
			CoupleArenaFuBenData result;
			lock (this.Mutex)
			{
				CoupleArenaFuBenData coupleArenaFuBenData;
				if (!this.GameFuBenDict.TryGetValue(gameId, out coupleArenaFuBenData))
				{
					coupleArenaFuBenData = null;
				}
				result = coupleArenaFuBenData;
			}
			return result;
		}

		public CoupleArenaPkResultRsp CoupleArenaPkResult(CoupleArenaPkResultReq req)
		{
			CoupleArenaPkResultRsp result;
			if (req == null)
			{
				result = null;
			}
			else
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				lock (this.Mutex)
				{
					CoupleArenaFuBenData coupleArenaFuBenData;
					if (!this.IsValidCoupleIfExist(req.ManRole1, req.WifeRole1) || !this.IsValidCoupleIfExist(req.ManRole2, req.WifeRole2))
					{
						result = null;
					}
					else if (!this.GameFuBenDict.TryGetValue(req.GameId, out coupleArenaFuBenData))
					{
						result = null;
					}
					else
					{
						CoupleArenaPkResultRsp coupleArenaPkResultRsp = new CoupleArenaPkResultRsp();
						if (req.winSide == 0)
						{
							coupleArenaPkResultRsp.Couple1RetData.Result = 0;
							coupleArenaPkResultRsp.Couple2RetData.Result = 0;
						}
						else if (req.winSide == 1)
						{
							coupleArenaPkResultRsp.Couple1RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole1, req.WifeRole1) ? 0 : 1);
							coupleArenaPkResultRsp.Couple2RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole2, req.WifeRole2) ? 0 : 2);
						}
						else
						{
							coupleArenaPkResultRsp.Couple1RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole1, req.WifeRole1) ? 0 : 2);
							coupleArenaPkResultRsp.Couple2RetData.Result = (this.DivorceRecord.IsDivorce(req.ManRole2, req.WifeRole2) ? 0 : 1);
						}
						int duanWeiType = this._DuanWeiCfgList[0].DuanWeiType;
						int duanWeiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
						int duanWeiType2 = this._DuanWeiCfgList[0].DuanWeiType;
						int duanWeiLevel2 = this._DuanWeiCfgList[0].DuanWeiLevel;
						if (this.SyncData.RoleDict.ContainsKey(req.ManRole1))
						{
							duanWeiType = this.SyncData.RoleDict[req.ManRole1].DuanWeiType;
							duanWeiLevel = this.SyncData.RoleDict[req.ManRole1].DuanWeiLevel;
						}
						if (this.SyncData.RoleDict.ContainsKey(req.ManRole2))
						{
							duanWeiType2 = this.SyncData.RoleDict[req.ManRole2].DuanWeiType;
							duanWeiLevel2 = this.SyncData.RoleDict[req.ManRole2].DuanWeiLevel;
						}
						this.HandlePkResult(req.ManRole1, req.ManZoneId1, req.ManSelector1, req.WifeRole1, req.WifeZoneId1, req.WifeSelector1, duanWeiType2, duanWeiLevel2, coupleArenaPkResultRsp.Couple1RetData);
						this.HandlePkResult(req.ManRole2, req.ManZoneId2, req.ManSelector2, req.WifeRole2, req.WifeZoneId2, req.WifeSelector2, duanWeiType, duanWeiLevel, coupleArenaPkResultRsp.Couple2RetData);
						this.RemoveFuBen(req.GameId);
						this.Persistence.AddPkLog(coupleArenaFuBenData.GameId, coupleArenaFuBenData.StartTime, TimeUtil.NowDateTime(), req.ManRole1, req.WifeRole1, coupleArenaPkResultRsp.Couple1RetData.Result, req.ManRole2, req.WifeRole2, coupleArenaPkResultRsp.Couple2RetData.Result);
						result = coupleArenaPkResultRsp;
					}
				}
			}
			return result;
		}

		private void HandlePkResult(int man, int manzone, byte[] mandata, int wife, int wifezone, byte[] wifedata, int pkDuanWeiType, int pkDuanWeiLevel, CoupleArenaPkResultItem retData)
		{
			CoupleArenaCoupleDataK coupleData = null;
			if (!this.SyncData.RoleDict.TryGetValue(man, out coupleData))
			{
				coupleData = new CoupleArenaCoupleDataK();
				coupleData.Db_CoupleId = this.Persistence.GetNextDbCoupleId();
				coupleData.ManRoleId = man;
				coupleData.ManZoneId = manzone;
				coupleData.ManSelectorData = mandata;
				coupleData.WifeRoleId = wife;
				coupleData.WifeZoneId = wifezone;
				coupleData.WifeSelectorData = wifedata;
				coupleData.DuanWeiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
				coupleData.DuanWeiType = this._DuanWeiCfgList[0].DuanWeiType;
				coupleData.Rank = this.SyncData.RankList.Count + 1;
				if (retData.Result != 0)
				{
					this.SyncData.RankList.Add(coupleData);
					this.SyncData.RoleDict[coupleData.ManRoleId] = coupleData;
					this.SyncData.RoleDict[coupleData.WifeRoleId] = coupleData;
				}
			}
			else
			{
				coupleData.ManSelectorData = mandata;
				coupleData.WifeSelectorData = wifedata;
			}
			retData.OldDuanWeiType = coupleData.DuanWeiType;
			retData.OldDuanWeiLevel = coupleData.DuanWeiLevel;
			_CoupleArenaDuanWeiCfg coupleArenaDuanWeiCfg = this._DuanWeiCfgList.Find((_CoupleArenaDuanWeiCfg _d) => _d.DuanWeiLevel == coupleData.DuanWeiLevel && _d.DuanWeiType == coupleData.DuanWeiType);
			if (coupleArenaDuanWeiCfg == null)
			{
				LogManager.WriteLog(2, string.Format("couplearena.HandlePkResult can't find duanwei cfg ,type={0}, level={1}", coupleData.DuanWeiType, coupleData.DuanWeiLevel), null, true);
			}
			else
			{
				_CoupleArenaDuanWeiCfg coupleArenaDuanWeiCfg2 = this._DuanWeiCfgList.Find((_CoupleArenaDuanWeiCfg _d) => _d.DuanWeiLevel == pkDuanWeiLevel && _d.DuanWeiType == pkDuanWeiType);
				if (coupleArenaDuanWeiCfg2 == null)
				{
					LogManager.WriteLog(2, string.Format("couplearena.HandlePkResult can't find duanwei cfg ,type={0}, level={1}", pkDuanWeiType, pkDuanWeiLevel), null, true);
				}
				else
				{
					if (retData.Result == 0)
					{
						retData.NewDuanWeiType = coupleData.DuanWeiType;
						retData.NewDuanWeiLevel = coupleData.DuanWeiLevel;
					}
					else
					{
						coupleData.TotalFightTimes++;
						if (retData.Result == 1)
						{
							coupleData.WinFightTimes++;
							coupleData.LianShengTimes++;
							coupleData.JiFen += coupleArenaDuanWeiCfg2.WinJiFen;
							retData.GetJiFen = coupleArenaDuanWeiCfg2.WinJiFen;
						}
						else
						{
							coupleData.LianShengTimes = 0;
							coupleData.JiFen += coupleArenaDuanWeiCfg.LoseJiFen;
							coupleData.JiFen = Math.Max(coupleData.JiFen, 0);
							retData.GetJiFen = coupleArenaDuanWeiCfg.LoseJiFen;
						}
						this.ParseDuanweiByJiFen(coupleData.JiFen, out coupleData.DuanWeiType, out coupleData.DuanWeiLevel);
						this.SyncData.ModifyTime = TimeUtil.NowDateTime();
						retData.NewDuanWeiLevel = coupleData.DuanWeiLevel;
						retData.NewDuanWeiType = coupleData.DuanWeiType;
						this.IsNeedSort = true;
					}
					if (retData.Result != 0)
					{
						this.Persistence.WriteCoupleData(coupleData);
					}
				}
			}
		}

		private void ParseDuanweiByJiFen(int jifen, out int duanweiType, out int duanweiLevel)
		{
			duanweiLevel = this._DuanWeiCfgList[0].DuanWeiLevel;
			duanweiType = this._DuanWeiCfgList[0].DuanWeiType;
			for (int i = 0; i < this._DuanWeiCfgList.Count; i++)
			{
				if (jifen >= this._DuanWeiCfgList[i].NeedJiFen)
				{
					if (i == this._DuanWeiCfgList.Count - 1 || jifen < this._DuanWeiCfgList[i + 1].NeedJiFen)
					{
						duanweiType = this._DuanWeiCfgList[i].DuanWeiType;
						duanweiLevel = this._DuanWeiCfgList[i].DuanWeiLevel;
					}
				}
			}
		}

		public void Update()
		{
			try
			{
				lock (this.Mutex)
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					if ((dateTime - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
					{
						this.UpdateFrameCount += 1U;
						if (dateTime.DayOfYear != this.LastUpdateTime.DayOfYear)
						{
							this.MatchTimeLimiter.Reset();
							this.JoinDataUtil.Reset();
							this.DivorceRecord.Reset();
						}
						this.CheckRoleMatch(dateTime);
						if (this.UpdateFrameCount % 30U == 0U)
						{
							this.CheckTimeOutFuBen(dateTime);
						}
						if (this.LastUpdateTime.TimeOfDay.Ticks <= this._WarTimePointList.First<_CoupleArenaWarTimePoint>().DayStartTicks && dateTime.TimeOfDay.Ticks >= this._WarTimePointList.First<_CoupleArenaWarTimePoint>().DayStartTicks)
						{
							if (this.Persistence.CheckClearRank(this.CurrRankWeek(dateTime)))
							{
								lock (this.Mutex)
								{
									this.SyncData.RankList.Clear();
									this.SyncData.BuildRoleDict();
									this.SyncData.ModifyTime = dateTime;
								}
							}
						}
						if (this.UpdateFrameCount % 30U == 0U)
						{
							this.CheckRebuildRank(dateTime);
						}
						if (this.UpdateFrameCount % 300U == 0U)
						{
							this.CheckFlushRank2Db();
						}
						this.LastUpdateTime = dateTime;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache("CoupleArenaService.Update failed! " + ex.Message);
			}
		}

		private void CheckRoleMatch(DateTime now)
		{
			lock (this.Mutex)
			{
				List<CoupleArenaJoinData> joinList = this.JoinDataUtil.GetJoinList();
				if (joinList != null && joinList.Count > 0)
				{
					CoupleArenaJoinMatcher coupleArenaJoinMatcher = new CoupleArenaJoinMatcher();
					foreach (CoupleArenaJoinData coupleArenaJoinData in joinList)
					{
						if ((now - coupleArenaJoinData.StartTime).TotalSeconds >= 60.0 || coupleArenaJoinData.ToKfServerId > 0)
						{
							this.JoinDataUtil.DelJoinData(coupleArenaJoinData);
						}
						else if ((now - coupleArenaJoinData.StartTime).TotalSeconds >= 30.0)
						{
							coupleArenaJoinMatcher.AddGlobalJoinData(coupleArenaJoinData);
						}
						else
						{
							coupleArenaJoinMatcher.AddJoinData(coupleArenaJoinData.DuanWeiType, coupleArenaJoinData.DuanWeiLevel, coupleArenaJoinData);
						}
					}
					foreach (List<CoupleArenaJoinData> list in coupleArenaJoinMatcher.GetAllMatch())
					{
						int i = 0;
						while (i < list.Count - 1)
						{
							CoupleArenaJoinData coupleArenaJoinData2 = list[i];
							CoupleArenaJoinData coupleArenaJoinData3 = list[i + 1];
							if (this.MatchTimeLimiter.GetMatchTimes(coupleArenaJoinData2.RoleId1, coupleArenaJoinData2.RoleId2, coupleArenaJoinData3.RoleId1, coupleArenaJoinData3.RoleId2) >= TianTiPersistence.Instance.MaxRolePairFightCount)
							{
								i++;
							}
							else
							{
								CoupleArenaFuBenData coupleArenaFuBenData = new CoupleArenaFuBenData();
								coupleArenaFuBenData.GameId = this.Persistence.GetNextGameId();
								coupleArenaFuBenData.StartTime = now;
								coupleArenaFuBenData.RoleList = new List<KuaFuFuBenRoleData>();
								coupleArenaFuBenData.RoleList.Add(new KuaFuFuBenRoleData
								{
									ServerId = coupleArenaJoinData2.ServerId,
									RoleId = coupleArenaJoinData2.RoleId1,
									Side = 1
								});
								coupleArenaFuBenData.RoleList.Add(new KuaFuFuBenRoleData
								{
									ServerId = coupleArenaJoinData2.ServerId,
									RoleId = coupleArenaJoinData2.RoleId2,
									Side = 1
								});
								coupleArenaFuBenData.RoleList.Add(new KuaFuFuBenRoleData
								{
									ServerId = coupleArenaJoinData3.ServerId,
									RoleId = coupleArenaJoinData3.RoleId1,
									Side = 2
								});
								coupleArenaFuBenData.RoleList.Add(new KuaFuFuBenRoleData
								{
									ServerId = coupleArenaJoinData3.ServerId,
									RoleId = coupleArenaJoinData3.RoleId2,
									Side = 2
								});
								if (!ClientAgentManager.Instance().AssginKfFuben(this.GameType, coupleArenaFuBenData.GameId, 4, out coupleArenaFuBenData.KfServerId))
								{
									LogManager.WriteLog(2, "CoupleArena 没有跨服可以分配", null, true);
									return;
								}
								this.MatchTimeLimiter.AddMatchTimes(coupleArenaJoinData2.RoleId1, coupleArenaJoinData2.RoleId2, coupleArenaJoinData3.RoleId1, coupleArenaJoinData3.RoleId2, 1);
								this.GameFuBenDict[coupleArenaFuBenData.GameId] = coupleArenaFuBenData;
								i += 2;
								coupleArenaJoinData2.ToKfServerId = coupleArenaFuBenData.KfServerId;
								coupleArenaJoinData3.ToKfServerId = coupleArenaFuBenData.KfServerId;
								CoupleArenaCanEnterData coupleArenaCanEnterData = new CoupleArenaCanEnterData
								{
									GameId = coupleArenaFuBenData.GameId,
									KfServerId = coupleArenaFuBenData.KfServerId,
									RoleId1 = coupleArenaJoinData2.RoleId1,
									RoleId2 = coupleArenaJoinData2.RoleId2
								};
								ClientAgentManager.Instance().PostAsyncEvent(coupleArenaJoinData2.ServerId, this.EvItemGameType, new AsyncDataItem(10014, new object[]
								{
									coupleArenaCanEnterData
								}));
								CoupleArenaCanEnterData coupleArenaCanEnterData2 = new CoupleArenaCanEnterData
								{
									GameId = coupleArenaFuBenData.GameId,
									KfServerId = coupleArenaFuBenData.KfServerId,
									RoleId1 = coupleArenaJoinData3.RoleId1,
									RoleId2 = coupleArenaJoinData3.RoleId2
								};
								AsyncDataItem asyncDataItem = new AsyncDataItem(10014, new object[]
								{
									coupleArenaFuBenData.GameId,
									coupleArenaFuBenData.KfServerId,
									coupleArenaJoinData3.RoleId1,
									coupleArenaJoinData3.RoleId2
								});
								ClientAgentManager.Instance().PostAsyncEvent(coupleArenaJoinData3.ServerId, this.EvItemGameType, new AsyncDataItem(10014, new object[]
								{
									coupleArenaCanEnterData2
								}));
							}
						}
					}
				}
			}
		}

		private void CheckTimeOutFuBen(DateTime now)
		{
			lock (this.Mutex)
			{
				foreach (CoupleArenaFuBenData coupleArenaFuBenData in this.GameFuBenDict.Values.ToList<CoupleArenaFuBenData>())
				{
					if ((now - coupleArenaFuBenData.StartTime).TotalMinutes > 5.0)
					{
						this.RemoveFuBen(coupleArenaFuBenData.GameId);
					}
				}
			}
		}

		private void RemoveFuBen(long gameId)
		{
			lock (this.Mutex)
			{
				CoupleArenaFuBenData coupleArenaFuBenData;
				if (this.GameFuBenDict.TryGetValue(gameId, out coupleArenaFuBenData))
				{
					ClientAgentManager.Instance().RemoveKfFuben(this.GameType, coupleArenaFuBenData.KfServerId, coupleArenaFuBenData.GameId);
					this.GameFuBenDict.Remove(coupleArenaFuBenData.GameId);
				}
			}
		}

		private void CheckRebuildRank(DateTime now)
		{
			lock (this.Mutex)
			{
				if (this.IsNeedSort)
				{
					this.SyncData.RankList.Sort();
					for (int i = 0; i < this.SyncData.RankList.Count; i++)
					{
						this.SyncData.RankList[i].Rank = i + 1;
					}
					this.SyncData.BuildRoleDict();
					this.SyncData.ModifyTime = now;
					this.IsNeedSort = false;
					this.IsRankChanged = true;
				}
			}
		}

		private void CheckFlushRank2Db()
		{
			lock (this.Mutex)
			{
				if (this.IsRankChanged)
				{
					LogManager.WriteLog(2, "Persistence.FlushRandList2Db begin", null, true);
					this.Persistence.FlushRandList2Db(this.SyncData.RankList);
					LogManager.WriteLog(2, "Persistence.FlushRandList2Db end", null, true);
					this.IsRankChanged = false;
				}
			}
		}

		private int CurrRankWeek(DateTime time)
		{
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			_CoupleArenaWarTimePoint coupleArenaWarTimePoint = this._WarTimePointList.First<_CoupleArenaWarTimePoint>();
			int result;
			if (weekDay1To < coupleArenaWarTimePoint.Weekday || (weekDay1To == coupleArenaWarTimePoint.Weekday && time.TimeOfDay.Ticks < coupleArenaWarTimePoint.DayStartTicks))
			{
				result = TimeUtil.MakeFirstWeekday(time.AddDays(-7.0));
			}
			else
			{
				result = TimeUtil.MakeFirstWeekday(time);
			}
			return result;
		}

		private bool IsInWeekOnceActTimes(DateTime time)
		{
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			foreach (_CoupleArenaWarTimePoint coupleArenaWarTimePoint in this._WarTimePointList)
			{
				if (coupleArenaWarTimePoint.Weekday == weekDay1To && time.TimeOfDay.Ticks >= coupleArenaWarTimePoint.DayStartTicks && time.TimeOfDay.Ticks <= coupleArenaWarTimePoint.DayEndTicks)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsInWeekRangeActTimes(DateTime time)
		{
			_CoupleArenaWarTimePoint coupleArenaWarTimePoint = this._WarTimePointList.First<_CoupleArenaWarTimePoint>();
			_CoupleArenaWarTimePoint coupleArenaWarTimePoint2 = this._WarTimePointList.Last<_CoupleArenaWarTimePoint>();
			int weekDay1To = TimeUtil.GetWeekDay1To7(time);
			return ((weekDay1To == coupleArenaWarTimePoint.Weekday && time.TimeOfDay.Ticks > coupleArenaWarTimePoint.DayStartTicks) || weekDay1To > coupleArenaWarTimePoint.Weekday) && (weekDay1To < coupleArenaWarTimePoint2.Weekday || (weekDay1To == coupleArenaWarTimePoint2.Weekday && time.TimeOfDay.Ticks < coupleArenaWarTimePoint2.DayEndTicks));
		}

		private bool IsValidCoupleIfExist(int roleId1, int roleId2)
		{
			bool result;
			lock (this.Mutex)
			{
				CoupleArenaCoupleDataK coupleArenaCoupleDataK;
				this.SyncData.RoleDict.TryGetValue(roleId1, out coupleArenaCoupleDataK);
				CoupleArenaCoupleDataK coupleArenaCoupleDataK2;
				this.SyncData.RoleDict.TryGetValue(roleId2, out coupleArenaCoupleDataK2);
				if (coupleArenaCoupleDataK == null && coupleArenaCoupleDataK2 == null)
				{
					result = true;
				}
				else if (coupleArenaCoupleDataK == null || coupleArenaCoupleDataK2 == null)
				{
					result = false;
				}
				else if (!object.ReferenceEquals(coupleArenaCoupleDataK, coupleArenaCoupleDataK2))
				{
					result = false;
				}
				else if ((coupleArenaCoupleDataK.ManRoleId == roleId1 && coupleArenaCoupleDataK.WifeRoleId == roleId2) || (coupleArenaCoupleDataK.ManRoleId == roleId2 && coupleArenaCoupleDataK.WifeRoleId == roleId1))
				{
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		public void OnStopServer()
		{
			try
			{
				SysConOut.WriteLine("开始检测是否刷新情侣竞技数据到数据库...");
				lock (this.Mutex)
				{
					this.CheckRebuildRank(TimeUtil.NowDateTime());
					this.CheckFlushRank2Db();
				}
				SysConOut.WriteLine("结束检测是否刷新情侣竞技数据到数据库...");
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		private static CoupleArenaService _Instance = new CoupleArenaService();

		private List<_CoupleArenaDuanWeiCfg> _DuanWeiCfgList = new List<_CoupleArenaDuanWeiCfg>();

		private List<_CoupleArenaWarTimePoint> _WarTimePointList = new List<_CoupleArenaWarTimePoint>();

		public readonly GameTypes GameType = 13;

		public readonly GameTypes EvItemGameType = 2;

		private object Mutex = new object();

		private DateTime LastUpdateTime = DateTime.MinValue;

		private uint UpdateFrameCount = 0U;

		private CoupleArenaSyncData SyncData = new CoupleArenaSyncData();

		private bool IsNeedSort = false;

		private bool IsRankChanged = false;

		private Dictionary<long, CoupleArenaFuBenData> GameFuBenDict = new Dictionary<long, CoupleArenaFuBenData>();

		private CoupleArenaMatchTimeLimiter MatchTimeLimiter = new CoupleArenaMatchTimeLimiter();

		private CoupleArenaJoinDataUtil JoinDataUtil = new CoupleArenaJoinDataUtil();

		private CoupleArenaDivorceRecord DivorceRecord = new CoupleArenaDivorceRecord();

		private CoupleArenaPersistence Persistence = CoupleArenaPersistence.getInstance();
	}
}
