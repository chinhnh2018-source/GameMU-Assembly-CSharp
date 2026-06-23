using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
	internal class CoupleWishService
	{
		private CoupleWishService()
		{
		}

		public static CoupleWishService getInstance()
		{
			return CoupleWishService._Instance;
		}

		public void StartUp()
		{
			try
			{
				this._Config.Load(KuaFuServerManager.GetResourcePath(CoupleWishConsts.RankAwardCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath(CoupleWishConsts.WishTypeCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes), KuaFuServerManager.GetResourcePath(CoupleWishConsts.YanHuiCfgFile, KuaFuServerManager.ResourcePathTypes.GameRes));
				this.ReloadSyncData();
				this.WishRecordMgr = new CoupleWishRecordManager(this.SyncData.ThisWeek.Week);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, "CoupleWishService.StartUp failed!", ex, true);
			}
		}

		private void ReloadSyncData()
		{
			lock (this.Mutex)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				this.SyncData.ThisWeek.ModifyTime = dateTime;
				this.SyncData.ThisWeek.Week = this.CurrRankWeek(dateTime);
				this.SyncData.ThisWeek.RankList = this.Persistence.LoadRankFromDb(this.SyncData.ThisWeek.Week);
				this.SyncData.ThisWeek.BuildIndex();
				int num = 1;
				while (num < this.SyncData.ThisWeek.RankList.Count && !this.IsNeedSort)
				{
					this.IsNeedSort = (this.SyncData.ThisWeek.RankList[num].CompareTo(this.SyncData.ThisWeek.RankList[num - 1]) < 0);
					num++;
				}
				this.CheckSortRank();
				this.CheckSaveRank();
				this.SyncData.LastWeek.ModifyTime = dateTime;
				this.SyncData.LastWeek.Week = this.CurrRankWeek(dateTime.AddDays(-7.0));
				this.SyncData.LastWeek.RankList = this.Persistence.LoadRankFromDb(this.SyncData.LastWeek.Week);
				this.SyncData.LastWeek.BuildIndex();
				this.SyncData.Statue = this.Persistence.LoadCoupleStatue(this.SyncData.LastWeek.Week);
				if (this.SyncData.LastWeek.RankList.Count > 0 && this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Rank == 1 && this.SyncData.Statue.DbCoupleId != this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().DbCoupleId)
				{
					this.SyncData.Statue = new CoupleWishSyncStatueData();
					this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
					this.SyncData.Statue.Week = this.SyncData.LastWeek.Week;
					this.SyncData.Statue.DbCoupleId = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().DbCoupleId;
					this.SyncData.Statue.Man = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Man;
					this.SyncData.Statue.Wife = this.SyncData.LastWeek.RankList.First<CoupleWishCoupleDataK>().Wife;
					this.Persistence.WriteStatueData(this.SyncData.Statue);
				}
			}
		}

		public void Update()
		{
			if (this.WishRecordMgr != null)
			{
				try
				{
					DateTime dateTime = TimeUtil.NowDateTime();
					if ((dateTime - this.LastUpdateTime).TotalMilliseconds >= 1000.0)
					{
						this.UpdateFrameCount += 1U;
						if (this.LastUpdateTime.DayOfYear != dateTime.DayOfYear && TimeUtil.GetWeekDay1To7(dateTime) == 1)
						{
							lock (this.Mutex)
							{
								this.CheckSortRank();
								this.CheckSaveRank();
								this.ReloadSyncData();
								this.WishRecordMgr.UpdateWeek(this.SyncData.ThisWeek.Week);
							}
						}
						if (this.UpdateFrameCount % 30U == 0U)
						{
							this.CheckSortRank();
						}
						if (this.UpdateFrameCount % 600U == 0U)
						{
							this.CheckSaveRank();
							this.WishRecordMgr.ClearUnActiveRecord();
						}
						this.LastUpdateTime = dateTime;
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, "CoupleWishService.Update failed!", ex, true);
				}
			}
		}

		public int CoupleWishWishRole(CoupleWishWishRoleReq req)
		{
			DateTime time = TimeUtil.NowDateTime();
			long num = time.Ticks / 10000L;
			int result;
			try
			{
				lock (this.Mutex)
				{
					if (this.SyncData.ThisWeek.Week != this.CurrRankWeek(time))
					{
						result = -11000;
					}
					else
					{
						CoupleWishTypeConfig coupleWishTypeConfig = this._Config.WishTypeCfgList.Find((CoupleWishTypeConfig _w) => _w.WishType == req.WishType);
						if (coupleWishTypeConfig == null)
						{
							result = -3;
						}
						else
						{
							if (coupleWishTypeConfig.CooldownTime > 0)
							{
								if (this.WishCdControls.ContainsKey(req.WishType) && num - this.WishCdControls[req.WishType] < (long)(coupleWishTypeConfig.CooldownTime * 1000))
								{
									return -30;
								}
							}
							CoupleWishCoupleDataK coupleWishCoupleDataK;
							if (req.IsWishRank)
							{
								int index;
								if (!this.SyncData.ThisWeek.CoupleIdex.TryGetValue(req.ToCoupleId, out index))
								{
									return -11;
								}
								coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[index];
								coupleWishCoupleDataK.BeWishedNum += coupleWishTypeConfig.GetWishNum;
								if (req.ToManSelector != null && req.ToWifeSelector != null)
								{
									coupleWishCoupleDataK.Man = req.ToMan;
									coupleWishCoupleDataK.ManSelector = req.ToManSelector;
									coupleWishCoupleDataK.Wife = req.ToWife;
									coupleWishCoupleDataK.WifeSelector = req.ToWifeSelector;
									this.Persistence.WriteCoupleData(this.SyncData.ThisWeek.Week, coupleWishCoupleDataK);
								}
							}
							else
							{
								if (req.ToManSelector == null || req.ToWifeSelector == null)
								{
									return -11003;
								}
								if (!this.IsValidCoupleIfExist(req.ToMan.RoleId, req.ToWife.RoleId))
								{
									return -11003;
								}
								bool flag2 = false;
								int index;
								if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(req.ToMan.RoleId, out index))
								{
									flag2 = true;
									coupleWishCoupleDataK = new CoupleWishCoupleDataK();
									coupleWishCoupleDataK.DbCoupleId = this.Persistence.GetNextDbCoupleId();
									coupleWishCoupleDataK.Rank = this.SyncData.ThisWeek.RankList.Count + 1;
								}
								else
								{
									coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[index];
								}
								coupleWishCoupleDataK.Man = req.ToMan;
								coupleWishCoupleDataK.ManSelector = req.ToManSelector;
								coupleWishCoupleDataK.Wife = req.ToWife;
								coupleWishCoupleDataK.WifeSelector = req.ToWifeSelector;
								coupleWishCoupleDataK.BeWishedNum += coupleWishTypeConfig.GetWishNum;
								if (!this.Persistence.WriteCoupleData(this.SyncData.ThisWeek.Week, coupleWishCoupleDataK))
								{
									coupleWishCoupleDataK.BeWishedNum -= coupleWishTypeConfig.GetWishNum;
									return -15;
								}
								if (flag2)
								{
									this.SyncData.ThisWeek.RankList.Add(coupleWishCoupleDataK);
									this.SyncData.ThisWeek.BuildIndex();
								}
							}
							this.IsNeedSort = true;
							if (this.SyncData.ThisWeek.RankList.Count <= CoupleWishConsts.MaxRankNum || this.SyncData.ThisWeek.RankList.Last<CoupleWishCoupleDataK>().Rank <= CoupleWishConsts.MaxRankNum)
							{
								this.CheckSortRank();
							}
							this.WishCdControls[req.WishType] = num;
							this.WishRecordMgr.AddWishRecord(req.From, req.WishType, req.WishTxt, coupleWishCoupleDataK.DbCoupleId, coupleWishCoupleDataK.Man, coupleWishCoupleDataK.Wife);
							result = 1;
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = -11003;
			}
			return result;
		}

		public List<CoupleWishWishRecordData> CoupleWishGetWishRecord(int roleId)
		{
			List<CoupleWishWishRecordData> result;
			try
			{
				lock (this.Mutex)
				{
					result = this.WishRecordMgr.GetWishRecord(roleId);
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = null;
			}
			return result;
		}

		public CoupleWishSyncData CoupleWishSyncCenterData(DateTime oldThisWeek, DateTime oldLastWeek, DateTime oldStatue)
		{
			CoupleWishSyncData result;
			try
			{
				lock (this.Mutex)
				{
					CoupleWishSyncData coupleWishSyncData = new CoupleWishSyncData();
					if (oldThisWeek != this.SyncData.ThisWeek.ModifyTime && TimeUtil.RandomDispatchTime(oldThisWeek, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						coupleWishSyncData.ThisWeek = this.SyncData.ThisWeek.SimpleClone();
					}
					else
					{
						coupleWishSyncData.ThisWeek.ModifyTime = oldThisWeek;
					}
					if (oldLastWeek != this.SyncData.LastWeek.ModifyTime && TimeUtil.RandomDispatchTime(oldLastWeek, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						coupleWishSyncData.LastWeek = this.SyncData.LastWeek.SimpleClone();
					}
					else
					{
						coupleWishSyncData.LastWeek.ModifyTime = oldLastWeek;
					}
					if (oldStatue != this.SyncData.Statue.ModifyTime && TimeUtil.RandomDispatchTime(oldStatue, TimeUtil.NowDateTime(), 180, 60, 10))
					{
						coupleWishSyncData.Statue = this.SyncData.Statue.SimpleClone();
					}
					else
					{
						coupleWishSyncData.Statue.ModifyTime = oldStatue;
					}
					result = coupleWishSyncData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
				result = null;
			}
			return result;
		}

		public int CoupleWishPreDivorce(int man, int wife)
		{
			int result;
			lock (this.Mutex)
			{
				DateTime dateTime = TimeUtil.NowDateTime();
				if (!this.IsValidCoupleIfExist(man, wife))
				{
					int num;
					if (!this._Config.IsInWishTime(dateTime, ref num))
					{
						result = 1;
					}
					else
					{
						result = -11003;
					}
				}
				else
				{
					int num;
					if (this._Config.IsInWishTime(dateTime, ref num))
					{
						int index;
						if (this.SyncData.ThisWeek.RoleIndex.TryGetValue(man, out index))
						{
							CoupleWishCoupleDataK coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[index];
							if (!this.Persistence.ClearCoupleData(coupleWishCoupleDataK.DbCoupleId))
							{
								return -15;
							}
							this.SyncData.ThisWeek.RankList.RemoveAt(index);
							this.SyncData.ThisWeek.BuildIndex();
							this.IsNeedSort = true;
						}
					}
					if (this.SyncData.Statue.DbCoupleId > 0 && this.SyncData.Statue.Man != null && this.SyncData.Statue.Wife != null)
					{
						if (this.SyncData.Statue.Man.RoleId == man && this.SyncData.Statue.Wife.RoleId == wife && this.SyncData.Statue.IsDivorced != 1)
						{
							int isDivorced = this.SyncData.Statue.IsDivorced;
							this.SyncData.Statue.IsDivorced = 1;
							if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
							{
								this.SyncData.Statue.IsDivorced = isDivorced;
								return -15;
							}
							this.SyncData.Statue.ModifyTime = dateTime;
						}
					}
					result = 1;
				}
			}
			return result;
		}

		public int CoupleWishAdmire(int fromRole, int fromZone, int admireType, int toCoupleId)
		{
			int result;
			lock (this.Mutex)
			{
				if (this.SyncData.Statue.DbCoupleId > 0 && this.SyncData.Statue.DbCoupleId == toCoupleId && this.SyncData.Statue.ManRoleDataEx != null && this.SyncData.Statue.WifeRoleDataEx != null)
				{
					this.SyncData.Statue.BeAdmireCount++;
					this.Persistence.WriteStatueData(this.SyncData.Statue);
				}
				this.Persistence.AddAdmireLog(fromRole, fromZone, admireType, toCoupleId, this.SyncData.LastWeek.Week);
				this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
				result = 1;
			}
			return result;
		}

		public int CoupleWishJoinParty(int fromRole, int fromZone, int toCoupleId)
		{
			int result;
			lock (this.Mutex)
			{
				if (this.SyncData.Statue.DbCoupleId != toCoupleId)
				{
					result = -12;
				}
				else if (this.SyncData.Statue.YanHuiJoinNum >= this._Config.YanHuiCfg.TotalMaxJoinNum)
				{
					result = -16;
				}
				else
				{
					this.SyncData.Statue.YanHuiJoinNum++;
					if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
					{
						this.SyncData.Statue.YanHuiJoinNum--;
						result = -15;
					}
					else
					{
						this.Persistence.AddYanHuiJoinLog(fromRole, fromZone, toCoupleId, this.SyncData.LastWeek.Week);
						this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
						result = 1;
					}
				}
			}
			return result;
		}

		public void CoupleWishReportCoupleStatue(CoupleWishReportStatueData req)
		{
			if (req != null && req.ManStatue != null && req.WifeStatue != null)
			{
				lock (this.Mutex)
				{
					if (this.SyncData.Statue.DbCoupleId == req.DbCoupleId)
					{
						byte[] manRoleDataEx = this.SyncData.Statue.ManRoleDataEx;
						byte[] wifeRoleDataEx = this.SyncData.Statue.WifeRoleDataEx;
						this.SyncData.Statue.ManRoleDataEx = req.ManStatue;
						this.SyncData.Statue.WifeRoleDataEx = req.WifeStatue;
						if (!this.Persistence.WriteStatueData(this.SyncData.Statue))
						{
							this.SyncData.Statue.ManRoleDataEx = manRoleDataEx;
							this.SyncData.Statue.WifeRoleDataEx = wifeRoleDataEx;
						}
					}
					this.SyncData.Statue.ModifyTime = TimeUtil.NowDateTime();
				}
			}
		}

		private void CheckSortRank()
		{
			lock (this.Mutex)
			{
				if (this.IsNeedSort)
				{
					this.IsNeedSort = false;
					this.SyncData.ThisWeek.RankList.Sort();
					List<int> list = new List<int>();
					foreach (CoupleWishRankAwardConfig coupleWishRankAwardConfig in this._Config.RankAwardCfgList)
					{
						if (coupleWishRankAwardConfig.EndRank <= 0)
						{
							break;
						}
						for (int i = coupleWishRankAwardConfig.StartRank; i <= coupleWishRankAwardConfig.EndRank; i++)
						{
							list.Add(coupleWishRankAwardConfig.MinWishNum);
						}
					}
					int num = 1;
					int j = 0;
					while (j < this.SyncData.ThisWeek.RankList.Count)
					{
						if (num - 1 >= 0 && num - 1 < list.Count)
						{
							if (this.SyncData.ThisWeek.RankList[j].BeWishedNum >= list[num - 1])
							{
								this.SyncData.ThisWeek.RankList[j].Rank = num;
								num++;
								j++;
							}
							else
							{
								num++;
							}
						}
						else
						{
							this.SyncData.ThisWeek.RankList[j].Rank = num;
							num++;
							j++;
						}
					}
					this.SyncData.ThisWeek.ModifyTime = TimeUtil.NowDateTime();
					this.SyncData.ThisWeek.BuildIndex();
					this.IsNeedSaveRank = true;
				}
			}
		}

		private void CheckSaveRank()
		{
			lock (this.Mutex)
			{
				if (this.IsNeedSaveRank)
				{
					LogManager.WriteLog(2, "CoupleWishService.CheckSaveRank begin", null, true);
					this.Persistence.UpdateRand2Db(this.SyncData.ThisWeek.RankList);
					LogManager.WriteLog(2, "CoupleWishService.CheckSaveRank end", null, true);
					this.IsNeedSaveRank = false;
				}
			}
		}

		private bool IsValidCoupleIfExist(int man, int wife)
		{
			bool result;
			lock (this.Mutex)
			{
				int num;
				if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(man, out num))
				{
					num = -1;
				}
				int num2;
				if (!this.SyncData.ThisWeek.RoleIndex.TryGetValue(wife, out num2))
				{
					num2 = -1;
				}
				if (num != num2)
				{
					result = false;
				}
				else
				{
					if (num != -1)
					{
						CoupleWishCoupleDataK coupleWishCoupleDataK = this.SyncData.ThisWeek.RankList[num];
						if ((coupleWishCoupleDataK.Man.RoleId != man || coupleWishCoupleDataK.Wife.RoleId != wife) && (coupleWishCoupleDataK.Man.RoleId != wife || coupleWishCoupleDataK.Wife.RoleId != man))
						{
							return false;
						}
					}
					result = true;
				}
			}
			return result;
		}

		private int CurrRankWeek(DateTime time)
		{
			return TimeUtil.MakeFirstWeekday(time);
		}

		public void OnStopServer()
		{
			try
			{
				SysConOut.WriteLine("开始检测是否刷新情侣排行榜到数据库...");
				lock (this.Mutex)
				{
					this.CheckSortRank();
					this.CheckSaveRank();
				}
				SysConOut.WriteLine("结束检测是否刷新情侣排行榜到数据库...");
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		private static readonly CoupleWishService _Instance = new CoupleWishService();

		private object Mutex = new object();

		private CoupleWishSyncData SyncData = new CoupleWishSyncData();

		private DateTime LastUpdateTime = DateTime.MinValue;

		private uint UpdateFrameCount = 0U;

		private CoupleWishRecordManager WishRecordMgr = null;

		private CoupleWishConfig _Config = new CoupleWishConfig();

		private CoupleWishPersistence Persistence = CoupleWishPersistence.getInstance();

		private Dictionary<int, long> WishCdControls = new Dictionary<int, long>();

		private bool IsNeedSort = false;

		private bool IsNeedSaveRank = false;
	}
}
