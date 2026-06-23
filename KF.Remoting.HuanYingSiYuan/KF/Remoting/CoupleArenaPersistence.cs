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
	internal class CoupleArenaPersistence
	{
		private CoupleArenaPersistence()
		{
		}

		public static CoupleArenaPersistence getInstance()
		{
			return CoupleArenaPersistence._Instance;
		}

		public int GetNextDbCoupleId()
		{
			if (this.CurrDbCoupleId == Global.UninitGameId)
			{
				string sqlstring = "SELECT IFNULL(MAX(couple_id),0) couple_id  FROM(SELECT MAX(couple_id) couple_id FROM t_couple_arena_group UNION SELECT MAX(couple_id) couple_id FROM t_couple_arena_rank_history) couple_ids;";
				this.CurrDbCoupleId = (int)((long)DbHelperMySQL.GetSingle(sqlstring));
			}
			this.CurrDbCoupleId++;
			return this.CurrDbCoupleId;
		}

		public long GetNextGameId()
		{
			if (this.CurrGameId == (long)Global.UninitGameId)
			{
				string sqlstring = "SELECT IFNULL(MAX(id),0) id FROM t_couple_arena_pk_log;";
				this.CurrGameId = (long)DbHelperMySQL.GetSingle(sqlstring);
			}
			this.CurrGameId += 1L;
			return this.CurrGameId;
		}

		public bool CheckClearRank(int currWeek)
		{
			bool result;
			try
			{
				DbHelperMySQL.ExecuteSql(string.Format("INSERT IGNORE INTO t_async(`id`,`value`) VALUES({0},{1});", 5, 20151230));
				int num = (int)DbHelperMySQL.GetSingle("select value from t_async where id = " + 5);
				if (num == currWeek)
				{
					result = false;
				}
				else
				{
					string str = "INSERT INTO t_couple_arena_rank_history(`couple_id`,`man_rid`,`man_zoneid`,`wife_rid`,`wife_zoneid`,`total_pk_times`,`total_win_times`,`liansheng`,`jifen`,`duanwei_type`,`duanwei_level`,`rank`,`is_divorced`) (SELECT `couple_id`,`man_rid`,`man_zoneid`,`wife_rid`,`wife_zoneid`,`total_pk_times`,`total_win_times`,`liansheng`,`jifen`,`duanwei_type`,`duanwei_level`,`rank`,`is_divorced` FROM t_couple_arena_group);" + string.Format("UPDATE t_couple_arena_rank_history SET `week`={0} WHERE `week`=0;", num);
					string str2 = "DELETE FROM t_couple_arena_group;";
					string str3 = string.Format("REPLACE INTO t_async(`id`,`value`) VALUES({0},{1});", 5, currWeek);
					DbHelperMySQL.ExecuteSql(str + str2 + str3);
					result = true;
				}
			}
			catch (Exception ex)
			{
				result = false;
			}
			return result;
		}

		public void FlushRandList2Db(List<CoupleArenaCoupleDataK> list)
		{
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < list.Count; i++)
			{
				stringBuilder.AppendFormat("UPDATE t_couple_arena_group SET `total_pk_times`={0},`total_win_times`={1},`liansheng`={2},`jifen`={3},`duanwei_type`={4},`duanwei_level`={5},`rank`={6},`is_divorced`={7} WHERE `couple_id`={8};", new object[]
				{
					list[i].TotalFightTimes,
					list[i].WinFightTimes,
					list[i].LianShengTimes,
					list[i].JiFen,
					list[i].DuanWeiType,
					list[i].DuanWeiLevel,
					list[i].Rank,
					list[i].IsDivorced,
					list[i].Db_CoupleId
				});
				stringBuilder.AppendLine();
				if ((i % 50 == 0 || i == list.Count - 1) && stringBuilder.Length > 0)
				{
					try
					{
						DbHelperMySQL.ExecuteSql(stringBuilder.ToString());
					}
					catch (Exception ex)
					{
						LogManager.WriteException("CoupleArenaPersistence.FlushRandList2Db failed! " + ex.Message);
					}
					stringBuilder.Clear();
				}
			}
		}

		public List<CoupleArenaCoupleDataK> LoadRankFromDb()
		{
			MySqlDataReader mySqlDataReader = null;
			List<CoupleArenaCoupleDataK> list = new List<CoupleArenaCoupleDataK>();
			try
			{
				string strSQL = "SELECT `couple_id`,`man_rid`,`man_zoneid`,`man_data1`,`wife_rid`,`wife_zoneid`,`wife_data1`,`total_pk_times`,`total_win_times`,`liansheng`,`jifen`,`duanwei_type`,`duanwei_level`,`rank`,`is_divorced` FROM t_couple_arena_group ORDER BY `rank`;";
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					CoupleArenaCoupleDataK coupleArenaCoupleDataK = new CoupleArenaCoupleDataK();
					coupleArenaCoupleDataK.Db_CoupleId = Convert.ToInt32(mySqlDataReader["couple_id"]);
					coupleArenaCoupleDataK.ManRoleId = Convert.ToInt32(mySqlDataReader["man_rid"]);
					coupleArenaCoupleDataK.ManZoneId = Convert.ToInt32(mySqlDataReader["man_zoneid"]);
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("man_data1")))
					{
						coupleArenaCoupleDataK.ManSelectorData = (byte[])mySqlDataReader["man_data1"];
					}
					coupleArenaCoupleDataK.WifeRoleId = Convert.ToInt32(mySqlDataReader["wife_rid"]);
					coupleArenaCoupleDataK.WifeZoneId = Convert.ToInt32(mySqlDataReader["wife_zoneid"]);
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("wife_data1")))
					{
						coupleArenaCoupleDataK.WifeSelectorData = (byte[])mySqlDataReader["wife_data1"];
					}
					coupleArenaCoupleDataK.TotalFightTimes = Convert.ToInt32(mySqlDataReader["total_pk_times"]);
					coupleArenaCoupleDataK.WinFightTimes = Convert.ToInt32(mySqlDataReader["total_win_times"]);
					coupleArenaCoupleDataK.LianShengTimes = Convert.ToInt32(mySqlDataReader["liansheng"]);
					coupleArenaCoupleDataK.DuanWeiType = Convert.ToInt32(mySqlDataReader["duanwei_type"]);
					coupleArenaCoupleDataK.DuanWeiLevel = Convert.ToInt32(mySqlDataReader["duanwei_level"]);
					coupleArenaCoupleDataK.JiFen = Convert.ToInt32(mySqlDataReader["jifen"]);
					coupleArenaCoupleDataK.Rank = Convert.ToInt32(mySqlDataReader["rank"]);
					coupleArenaCoupleDataK.IsDivorced = Convert.ToInt32(mySqlDataReader["is_divorced"]);
					list.Add(coupleArenaCoupleDataK);
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
				string sqlstring = string.Format("DELETE FROM t_couple_arena_group WHERE `couple_id`={0}", db_coupleid);
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

		public void AddPkLog(long gameId, DateTime startTime, DateTime endTime, int man1, int wife1, int result1, int man2, int wife2, int result2)
		{
			try
			{
				string sqlstring = string.Format("INSERT INTO t_couple_arena_pk_log(`id`,`man_rid1`,`wife_rid1`,`result1`,`man_rid2`,`wife_rid2`,`result2`,`day`,`starttime`,`endtime`) VALUES({0},{1},{2},{3},{4},{5},{6},{7},'{8}','{9}');", new object[]
				{
					gameId,
					man1,
					wife1,
					result1,
					man2,
					wife2,
					result2,
					TimeUtil.MakeYearMonthDay(startTime),
					startTime.ToString("yyyy-MM-dd HH:mm:ss"),
					endTime.ToString("yyyy-MM-dd HH:mm:ss")
				});
				DbHelperMySQL.ExecuteSql(sqlstring);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.Message);
			}
		}

		public void WriteCoupleData(CoupleArenaCoupleDataK coupleData)
		{
			if (coupleData != null)
			{
				try
				{
					string strSQL = string.Format("INSERT INTO t_couple_arena_group(`couple_id`,`man_rid`,`man_zoneid`,`wife_rid`,`wife_zoneid`,`total_pk_times`,`total_win_times`,`liansheng`,`jifen`,`duanwei_type`,`duanwei_level`,`rank`,`is_divorced`,`man_data1`,`wife_data1`)  VALUES({0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},@man_data1,@wife_data1)  ON DUPLICATE KEY UPDATE `total_pk_times`={5},`total_win_times`={6},`liansheng`={7},`jifen`={8},`duanwei_type`={9},`duanwei_level`={10},`rank`={11},`is_divorced`={12},`man_data1`=@man_data1,`wife_data1`=@wife_data1;", new object[]
					{
						coupleData.Db_CoupleId,
						coupleData.ManRoleId,
						coupleData.ManZoneId,
						coupleData.WifeRoleId,
						coupleData.WifeZoneId,
						coupleData.TotalFightTimes,
						coupleData.WinFightTimes,
						coupleData.LianShengTimes,
						coupleData.JiFen,
						coupleData.DuanWeiType,
						coupleData.DuanWeiLevel,
						coupleData.Rank,
						coupleData.IsDivorced
					});
					DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
					{
						new Tuple<string, byte[]>("man_data1", coupleData.ManSelectorData),
						new Tuple<string, byte[]>("wife_data1", coupleData.WifeSelectorData)
					});
				}
				catch (Exception ex)
				{
					LogManager.WriteLog(2, string.Format("FlushRandList2Db failed, couple_id={0},man={1},wife={2}", coupleData.Db_CoupleId, coupleData.ManRoleId, coupleData.WifeRoleId), ex, true);
				}
			}
		}

		private static CoupleArenaPersistence _Instance = new CoupleArenaPersistence();

		private int CurrDbCoupleId = Global.UninitGameId;

		private long CurrGameId = (long)Global.UninitGameId;
	}
}
