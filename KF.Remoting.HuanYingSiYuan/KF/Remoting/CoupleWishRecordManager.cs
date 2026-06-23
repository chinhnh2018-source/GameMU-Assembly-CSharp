using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	internal class CoupleWishRecordManager
	{
		public CoupleWishRecordManager(int week)
		{
			this.UpdateWeek(week);
		}

		public void AddWishRecord(KuaFuRoleMiniData from, int wishType, string wishTxt, int toDbCoupleId, KuaFuRoleMiniData toMan, KuaFuRoleMiniData toWife)
		{
			lock (this.Mutex)
			{
				if (this.Persistence.SaveWishRecord(this.ThisWeek, from, wishType, wishTxt, toDbCoupleId, toMan, toWife))
				{
					this.AddCachedWishOther(from, wishType, wishTxt, toMan, toWife);
					this.AddCachedBeWished(toMan, toWife, wishType, wishTxt, from);
					this.AddCachedBeWished(toWife, toMan, wishType, wishTxt, from);
				}
			}
		}

		public List<CoupleWishWishRecordData> GetWishRecord(int roleId)
		{
			List<CoupleWishWishRecordData> list = null;
			MySqlDataReader mySqlDataReader = null;
			try
			{
				lock (this.Mutex)
				{
					Queue<CoupleWishWishRecordData> source = null;
					if (this.RoleWishRecords.TryGetValue(roleId, out source))
					{
						list = source.ToList<CoupleWishWishRecordData>();
					}
					if (list == null)
					{
						string strSQL = string.Format("SELECT `from_rid`,`from_zoneid`,`from_rname`,`to_man_rid`,`to_man_zoneid`,`to_man_rname`,`to_wife_rid`,`to_wife_zoneid`,`to_wife_rname`,`wish_type`,`wish_txt`  FROM t_couple_wish_wish_log WHERE `week`={0} AND (`from_rid`={1} OR `to_man_rid`={1} OR `to_wife_rid`={1}) ORDER BY `time` LIMIT {2};", this.ThisWeek, roleId, CoupleWishConsts.MaxWishRecordNum);
						mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
						source = (this.RoleWishRecords[roleId] = new Queue<CoupleWishWishRecordData>());
						while (mySqlDataReader != null && mySqlDataReader.Read())
						{
							CoupleWishWishRoleReq coupleWishWishRoleReq = new CoupleWishWishRoleReq();
							coupleWishWishRoleReq.From.RoleId = Convert.ToInt32(mySqlDataReader["from_rid"]);
							coupleWishWishRoleReq.From.ZoneId = Convert.ToInt32(mySqlDataReader["from_zoneid"]);
							coupleWishWishRoleReq.From.RoleName = mySqlDataReader["from_rname"].ToString();
							coupleWishWishRoleReq.ToMan.RoleId = Convert.ToInt32(mySqlDataReader["to_man_rid"]);
							coupleWishWishRoleReq.ToMan.ZoneId = Convert.ToInt32(mySqlDataReader["to_man_zoneid"]);
							coupleWishWishRoleReq.ToMan.RoleName = mySqlDataReader["to_man_rname"].ToString();
							coupleWishWishRoleReq.ToWife.RoleId = Convert.ToInt32(mySqlDataReader["to_wife_rid"]);
							coupleWishWishRoleReq.ToWife.ZoneId = Convert.ToInt32(mySqlDataReader["to_wife_zoneid"]);
							coupleWishWishRoleReq.ToWife.RoleName = mySqlDataReader["to_wife_rname"].ToString();
							coupleWishWishRoleReq.WishType = Convert.ToInt32(mySqlDataReader["wish_type"]);
							coupleWishWishRoleReq.WishTxt = mySqlDataReader["wish_txt"].ToString();
							if (coupleWishWishRoleReq.From.RoleId == roleId)
							{
								this.AddCachedWishOther(coupleWishWishRoleReq.From, coupleWishWishRoleReq.WishType, coupleWishWishRoleReq.WishTxt, coupleWishWishRoleReq.ToMan, coupleWishWishRoleReq.ToWife);
							}
							if (coupleWishWishRoleReq.ToMan.RoleId == roleId)
							{
								this.AddCachedBeWished(coupleWishWishRoleReq.ToMan, coupleWishWishRoleReq.ToWife, coupleWishWishRoleReq.WishType, coupleWishWishRoleReq.WishTxt, coupleWishWishRoleReq.From);
							}
							if (coupleWishWishRoleReq.ToWife.RoleId == roleId)
							{
								this.AddCachedBeWished(coupleWishWishRoleReq.ToWife, coupleWishWishRoleReq.ToMan, coupleWishWishRoleReq.WishType, coupleWishWishRoleReq.WishTxt, coupleWishWishRoleReq.From);
							}
						}
						list = source.ToList<CoupleWishWishRecordData>();
					}
					this.RoleLastReadMs[roleId] = TimeUtil.NOW();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.Message);
			}
			finally
			{
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			return list;
		}

		public void UpdateWeek(int week)
		{
			lock (this.Mutex)
			{
				if (this.ThisWeek != week)
				{
					this.RoleWishRecords.Clear();
					this.RoleLastReadMs.Clear();
					this.ThisWeek = week;
				}
			}
		}

		public void ClearUnActiveRecord()
		{
			bool flag = false;
			try
			{
				object mutex;
				Monitor.Enter(mutex = this.Mutex, ref flag);
				long nowMs = TimeUtil.NOW();
				int timeOutMs = 1800000;
				List<int> list = this.RoleLastReadMs.Keys.ToList<int>().FindAll((int _r) => this.RoleLastReadMs.ContainsKey(_r) && nowMs - this.RoleLastReadMs[_r] >= (long)timeOutMs);
				if (list != null)
				{
					foreach (int key in list)
					{
						this.RoleLastReadMs.Remove(key);
						this.RoleWishRecords.Remove(key);
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
		}

		private void AddCachedWishOther(KuaFuRoleMiniData from, int wishType, string wishTxt, KuaFuRoleMiniData toMan, KuaFuRoleMiniData toWife)
		{
			lock (this.Mutex)
			{
				Queue<CoupleWishWishRecordData> queue = null;
				if (this.RoleWishRecords.TryGetValue(from.RoleId, out queue))
				{
					CoupleWishWishRecordData coupleWishWishRecordData = new CoupleWishWishRecordData();
					coupleWishWishRecordData.FromRole = from;
					coupleWishWishRecordData.TargetRoles = new List<KuaFuRoleMiniData>();
					if (toMan.RoleId != from.RoleId)
					{
						coupleWishWishRecordData.TargetRoles.Add(toMan);
					}
					if (toWife.RoleId != from.RoleId)
					{
						coupleWishWishRecordData.TargetRoles.Add(toWife);
					}
					coupleWishWishRecordData.WishType = wishType;
					coupleWishWishRecordData.WishTxt = wishTxt;
					queue.Enqueue(coupleWishWishRecordData);
					while (queue.Count > CoupleWishConsts.MaxWishRecordNum)
					{
						queue.Dequeue();
					}
				}
			}
		}

		private void AddCachedBeWished(KuaFuRoleMiniData to, KuaFuRoleMiniData toSpouse, int wishType, string wishTxt, KuaFuRoleMiniData from)
		{
			lock (this.Mutex)
			{
				if (to.RoleId != from.RoleId)
				{
					Queue<CoupleWishWishRecordData> queue = null;
					if (this.RoleWishRecords.TryGetValue(to.RoleId, out queue))
					{
						CoupleWishWishRecordData coupleWishWishRecordData = new CoupleWishWishRecordData();
						coupleWishWishRecordData.FromRole = from;
						coupleWishWishRecordData.TargetRoles = new List<KuaFuRoleMiniData>();
						coupleWishWishRecordData.TargetRoles.Add(to);
						if (toSpouse.RoleId != from.RoleId)
						{
							coupleWishWishRecordData.TargetRoles.Add(toSpouse);
						}
						coupleWishWishRecordData.WishType = wishType;
						coupleWishWishRecordData.WishTxt = wishTxt;
						queue.Enqueue(coupleWishWishRecordData);
						while (queue.Count > CoupleWishConsts.MaxWishRecordNum)
						{
							queue.Dequeue();
						}
					}
				}
			}
		}

		private CoupleWishPersistence Persistence = CoupleWishPersistence.getInstance();

		private object Mutex = new object();

		private Dictionary<int, Queue<CoupleWishWishRecordData>> RoleWishRecords = new Dictionary<int, Queue<CoupleWishWishRecordData>>();

		private Dictionary<int, long> RoleLastReadMs = new Dictionary<int, long>();

		private int ThisWeek = 0;
	}
}
