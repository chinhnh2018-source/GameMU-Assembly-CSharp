using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using KF.Contract.Data;
using KF.Remoting.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;

namespace KF.Remoting
{
	internal class CoupleWishPersistence
	{
		private CoupleWishPersistence()
		{
		}

		public static CoupleWishPersistence getInstance()
		{
			return CoupleWishPersistence._Instance;
		}

		public int GetNextDbCoupleId()
		{
			if (this.CurrDbCoupleId == Global.UninitGameId)
			{
				string sqlstring = "SELECT IFNULL(MAX(couple_id),0) couple_id FROM t_couple_wish_group;";
				this.CurrDbCoupleId = (int)((long)DbHelperMySQL.GetSingle(sqlstring));
			}
			this.CurrDbCoupleId++;
			return this.CurrDbCoupleId;
		}

		public void UpdateRand2Db(List<CoupleWishCoupleDataK> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.AppendFormat("UPDATE t_couple_wish_group SET `be_wish_num`={0},`rank`={1} WHERE `couple_id`={2};", list[i].BeWishedNum, list[i].Rank, list[i].DbCoupleId);
				stringBuilder.AppendLine();
				if ((i % 50 == 0 || i == list.Count - 1) && stringBuilder.Length > 0)
				{
					try
					{
						string sqlstring = stringBuilder.ToString();
						stringBuilder.Clear();
						DbHelperMySQL.ExecuteSql(sqlstring);
					}
					catch (Exception ex)
					{
						LogManager.WriteException("CoupleWishPersistence.UpdateRank2Db " + ex.Message);
					}
				}
			}
		}

		public List<CoupleWishCoupleDataK> LoadRankFromDb(int week)
		{
			MySqlDataReader mySqlDataReader = null;
			List<CoupleWishCoupleDataK> list = new List<CoupleWishCoupleDataK>();
			try
			{
				string strSQL = string.Format("SELECT `couple_id`,`man_rid`,`man_zoneid`,`man_rname`,`man_selector`,`wife_rid`,`wife_zoneid`,`wife_rname`,`wife_selector`,`be_wish_num`,`rank` FROM t_couple_wish_group WHERE `week`={0} ORDER BY `rank`;", week);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					CoupleWishCoupleDataK coupleWishCoupleDataK = new CoupleWishCoupleDataK();
					coupleWishCoupleDataK.DbCoupleId = Convert.ToInt32(mySqlDataReader["couple_id"]);
					coupleWishCoupleDataK.Man = new KuaFuRoleMiniData();
					coupleWishCoupleDataK.Man.RoleId = Convert.ToInt32(mySqlDataReader["man_rid"]);
					coupleWishCoupleDataK.Man.ZoneId = Convert.ToInt32(mySqlDataReader["man_zoneid"]);
					coupleWishCoupleDataK.Man.RoleName = mySqlDataReader["man_rname"].ToString();
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("man_selector")))
					{
						coupleWishCoupleDataK.ManSelector = (byte[])mySqlDataReader["man_selector"];
					}
					coupleWishCoupleDataK.Wife = new KuaFuRoleMiniData();
					coupleWishCoupleDataK.Wife.RoleId = Convert.ToInt32(mySqlDataReader["wife_rid"]);
					coupleWishCoupleDataK.Wife.ZoneId = Convert.ToInt32(mySqlDataReader["wife_zoneid"]);
					coupleWishCoupleDataK.Wife.RoleName = mySqlDataReader["wife_rname"].ToString();
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("wife_selector")))
					{
						coupleWishCoupleDataK.WifeSelector = (byte[])mySqlDataReader["wife_selector"];
					}
					coupleWishCoupleDataK.BeWishedNum = Convert.ToInt32(mySqlDataReader["be_wish_num"]);
					coupleWishCoupleDataK.Rank = Convert.ToInt32(mySqlDataReader["rank"]);
					list.Add(coupleWishCoupleDataK);
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

		public bool ClearCoupleData(int db_coupleid)
		{
			bool result;
			try
			{
				string sqlstring = string.Format("DELETE FROM t_couple_wish_group WHERE `couple_id`={0}", db_coupleid);
				DbHelperMySQL.ExecuteSql(sqlstring);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = false;
			}
			return result;
		}

		public bool SaveWishRecord(int wishWeek, KuaFuRoleMiniData from, int wishType, string wishTxt, int toDbCoupleId, KuaFuRoleMiniData toMan, KuaFuRoleMiniData toWife)
		{
			bool result;
			try
			{
				string sqlstring = string.Format("INSERT INTO t_couple_wish_wish_log(`week`,`from_rid`,`from_zoneid`,`from_rname`,`to_couple_id`,`to_man_rid`,`to_man_zoneid`,`to_man_rname`,`to_wife_rid`,`to_wife_zoneid`,`to_wife_rname`,`time`,`wish_txt`,`wish_type`) VALUES({0},{1},{2},'{3}',{4},{5},{6},'{7}',{8},{9},'{10}','{11}','{12}',{13});", new object[]
				{
					wishWeek,
					from.RoleId,
					from.ZoneId,
					from.RoleName,
					toDbCoupleId,
					toMan.RoleId,
					toMan.ZoneId,
					toMan.RoleName,
					toWife.RoleId,
					toWife.ZoneId,
					toWife.RoleName,
					TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss"),
					wishTxt,
					wishType
				});
				DbHelperMySQL.ExecuteSql(sqlstring);
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
				result = false;
			}
			return result;
		}

		public bool WriteCoupleData(int week, CoupleWishCoupleDataK coupleData)
		{
			bool result;
			try
			{
				string strSQL = string.Format("INSERT INTO t_couple_wish_group(`couple_id`,`man_rid`,`man_zoneid`,`man_rname`,`wife_rid`,`wife_zoneid`,`wife_rname`,`be_wish_num`,`rank`,`week`,`man_selector`,`wife_selector`)  VALUES({0},{1},{2},'{3}',{4},{5},'{6}',{7},{8},{9},@man_selector,@wife_selector)  ON DUPLICATE KEY UPDATE `man_rname`='{3}',`wife_rname`='{6}',`be_wish_num`={7},`rank`={8},`man_selector`=@man_selector,`wife_selector`=@wife_selector;", new object[]
				{
					coupleData.DbCoupleId,
					coupleData.Man.RoleId,
					coupleData.Man.ZoneId,
					coupleData.Man.RoleName,
					coupleData.Wife.RoleId,
					coupleData.Wife.ZoneId,
					coupleData.Wife.RoleName,
					coupleData.BeWishedNum,
					coupleData.Rank,
					week
				});
				DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("man_selector", coupleData.ManSelector),
					new Tuple<string, byte[]>("wife_selector", coupleData.WifeSelector)
				});
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("CoupleWish FlushRandList2Db failed, couple_id={0},man={1},wife={2}", coupleData.DbCoupleId, coupleData.Man.RoleId, coupleData.Wife.RoleId), ex, true);
				result = false;
			}
			return result;
		}

		public void AddAdmireLog(int fromRole, int fromZone, int admireType, int toCoupleId, int week)
		{
			try
			{
				string sqlstring = string.Format("INSERT INTO t_couple_wish_admire_log(`from_rid`,`from_zoneid`,`admire_type`,`to_couple_id`,`week`,`time`) VALUES({0},{1},{2},{3},{4},'{5}');", new object[]
				{
					fromRole,
					fromZone,
					admireType,
					toCoupleId,
					week,
					TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
				});
				DbHelperMySQL.ExecuteSql(sqlstring);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		public CoupleWishSyncStatueData LoadCoupleStatue(int week)
		{
			CoupleWishSyncStatueData coupleWishSyncStatueData = new CoupleWishSyncStatueData();
			coupleWishSyncStatueData.ModifyTime = TimeUtil.NowDateTime();
			MySqlDataReader mySqlDataReader = null;
			try
			{
				string strSQL = string.Format("SELECT `couple_id`,`man_rid`,`man_zoneid`,`man_rname`,`man_statue`,`wife_rid`,`wife_zoneid`,`wife_rname`,`wife_statue`,`admire_cnt`,`is_divorced`,`yanhui_join_num` FROM t_couple_wish_statue WHERE `week`={0};", week);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				if (mySqlDataReader != null && mySqlDataReader.Read())
				{
					coupleWishSyncStatueData.DbCoupleId = Convert.ToInt32(mySqlDataReader["couple_id"]);
					coupleWishSyncStatueData.Man = new KuaFuRoleMiniData();
					coupleWishSyncStatueData.Man.RoleId = Convert.ToInt32(mySqlDataReader["man_rid"]);
					coupleWishSyncStatueData.Man.ZoneId = Convert.ToInt32(mySqlDataReader["man_zoneid"]);
					coupleWishSyncStatueData.Man.RoleName = mySqlDataReader["man_rname"].ToString();
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("man_statue")))
					{
						coupleWishSyncStatueData.ManRoleDataEx = (byte[])mySqlDataReader["man_statue"];
					}
					coupleWishSyncStatueData.Wife = new KuaFuRoleMiniData();
					coupleWishSyncStatueData.Wife.RoleId = Convert.ToInt32(mySqlDataReader["wife_rid"]);
					coupleWishSyncStatueData.Wife.ZoneId = Convert.ToInt32(mySqlDataReader["wife_zoneid"]);
					coupleWishSyncStatueData.Wife.RoleName = mySqlDataReader["wife_rname"].ToString();
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("wife_statue")))
					{
						coupleWishSyncStatueData.WifeRoleDataEx = (byte[])mySqlDataReader["wife_statue"];
					}
					coupleWishSyncStatueData.BeAdmireCount = Convert.ToInt32(mySqlDataReader["admire_cnt"]);
					coupleWishSyncStatueData.IsDivorced = Convert.ToInt32(mySqlDataReader["is_divorced"]);
					coupleWishSyncStatueData.YanHuiJoinNum = Convert.ToInt32(mySqlDataReader["yanhui_join_num"]);
					coupleWishSyncStatueData.Week = week;
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
			return coupleWishSyncStatueData;
		}

		public bool WriteStatueData(CoupleWishSyncStatueData statue)
		{
			bool result;
			try
			{
				string strSQL = string.Format("INSERT INTO t_couple_wish_statue(`couple_id`,`man_rid`,`man_zoneid`,`man_rname`,`wife_rid`,`wife_zoneid`,`wife_rname`,`admire_cnt`,`is_divorced`,`week`,`yanhui_join_num`,`man_statue`,`wife_statue`)  VALUES({0},{1},{2},'{3}',{4},{5},'{6}',{7},{8},{9},{10},@man_statue,@wife_statue)  ON DUPLICATE KEY UPDATE `admire_cnt`={7},`is_divorced`={8},`yanhui_join_num`={10},`man_statue`=@man_statue,`wife_statue`=@wife_statue;", new object[]
				{
					statue.DbCoupleId,
					statue.Man.RoleId,
					statue.Man.ZoneId,
					statue.Man.RoleName,
					statue.Wife.RoleId,
					statue.Wife.ZoneId,
					statue.Wife.RoleName,
					statue.BeAdmireCount,
					statue.IsDivorced,
					statue.Week,
					statue.YanHuiJoinNum
				});
				DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
				{
					new Tuple<string, byte[]>("man_statue", statue.ManRoleDataEx),
					new Tuple<string, byte[]>("wife_statue", statue.WifeRoleDataEx)
				});
				result = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("CoupleWish WriteStatueData failed, couple_id={0},man={1},wife={2}", statue.DbCoupleId, statue.Man.RoleId, statue.Wife.RoleId), ex, true);
				result = false;
			}
			return result;
		}

		public void AddYanHuiJoinLog(int fromRole, int fromZone, int toCoupleId, int week)
		{
			try
			{
				string sqlstring = string.Format("INSERT INTO t_couple_wish_join_yanhui_log(`from_rid`,`from_zoneid`,`to_couple_id`,`week`,`time`) VALUES({0},{1},{2},{3},'{4}');", new object[]
				{
					fromRole,
					fromZone,
					toCoupleId,
					week,
					TimeUtil.NowDateTime().ToString("yyyy-MM-dd HH:mm:ss")
				});
				DbHelperMySQL.ExecuteSql(sqlstring);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		private static CoupleWishPersistence _Instance = new CoupleWishPersistence();

		private int CurrDbCoupleId = Global.UninitGameId;

		private long CurrGameId = (long)Global.UninitGameId;
	}
}
