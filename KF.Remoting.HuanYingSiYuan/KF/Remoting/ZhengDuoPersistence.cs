using System;
using System.Collections.Generic;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class ZhengDuoPersistence
	{
		private ZhengDuoPersistence()
		{
		}

		public int DBWeekAndStepGet(int type)
		{
			int num = 0;
			int result;
			try
			{
				object single = DbHelperMySQL.GetSingle("select value from t_async where id = " + type);
				if (single != null && single != DBNull.Value)
				{
					int.TryParse(single.ToString(), out num);
				}
				result = num;
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result = -1;
			}
			return result;
		}

		public bool DBWeekAndStepSet(int type, int value)
		{
			string sqlCmd = string.Format("REPLACE INTO t_async(id,value) \r\n                                        VALUES('{0}','{1}')", type, value);
			return this.ExecuteSqlNoQuery(sqlCmd) > 0;
		}

		public Dictionary<int, ZhengDuoRankData> DBRankList(int week)
		{
			Dictionary<int, ZhengDuoRankData> dictionary = new Dictionary<int, ZhengDuoRankData>();
			try
			{
				string strSQL = string.Format("SELECT bhid,zoneid,bhname,bhLevel,zhanli,rank1,rank2,state,millisecond,serverid,lose,enemy\r\n                                                FROM t_zhengduo_rank\r\n                                                WHERE week='{0}'\r\n                                                order by rank2,rank1,millisecond", week);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					ZhengDuoRankData zhengDuoRankData = new ZhengDuoRankData();
					zhengDuoRankData.Bhid = Convert.ToInt32(mySqlDataReader["bhid"]);
					zhengDuoRankData.ZoneId = Convert.ToInt32(mySqlDataReader["zoneid"]);
					zhengDuoRankData.BhName = mySqlDataReader["bhname"].ToString();
					zhengDuoRankData.BhLevel = Convert.ToInt32(mySqlDataReader["bhLevel"]);
					zhengDuoRankData.ZhanLi = (long)Convert.ToInt32(mySqlDataReader["zhanli"]);
					zhengDuoRankData.Rank1 = Convert.ToInt32(mySqlDataReader["rank1"]);
					zhengDuoRankData.Rank2 = Convert.ToInt32(mySqlDataReader["rank2"]);
					zhengDuoRankData.State = Convert.ToInt32(mySqlDataReader["state"]);
					zhengDuoRankData.UsedMillisecond = Convert.ToInt32(mySqlDataReader["millisecond"]);
					zhengDuoRankData.ServerID = Convert.ToInt32(mySqlDataReader["serverid"]);
					zhengDuoRankData.Lose = Convert.ToInt32(mySqlDataReader["lose"]);
					zhengDuoRankData.Enemy = Convert.ToInt32(mySqlDataReader["enemy"]);
					zhengDuoRankData.Week = week;
					dictionary[zhengDuoRankData.Rank1] = zhengDuoRankData;
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return null;
			}
			return dictionary;
		}

		public bool DBRankUpdata(ZhengDuoRankData data)
		{
			bool result;
			try
			{
				string sqlCmd = string.Format("REPLACE INTO t_zhengduo_rank(week,bhid,zoneid,bhname,bhLevel,zhanli,rank1,rank2,state,millisecond,serverid,lose,enemy) \r\n                                            VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}','{10}',{11},{12})", new object[]
				{
					data.Week,
					data.Bhid,
					data.ZoneId,
					data.BhName,
					data.BhLevel,
					data.ZhanLi,
					data.Rank1,
					data.Rank2,
					data.State,
					data.UsedMillisecond,
					data.ServerID,
					data.Lose,
					data.Enemy
				});
				result = (this.ExecuteSqlNoQuery(sqlCmd) >= 0);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				result = false;
			}
			return result;
		}

		public long CreateZhengDuoFuBen(int gametype, int serverId)
		{
			string sqlstring = string.Format("INSERT INTO t_game(gametype,serverid,createtime) VALUES({0},{1},NOW());", gametype, serverId);
			return DbHelperMySQL.ExecuteSqlGetIncrement(sqlstring, null);
		}

		private int ExecuteSqlNoQuery(string sqlCmd)
		{
			int result = 0;
			try
			{
				result = DbHelperMySQL.ExecuteSql(sqlCmd);
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
				return -1;
			}
			return result;
		}

		public static readonly ZhengDuoPersistence Instance = new ZhengDuoPersistence();
	}
}
