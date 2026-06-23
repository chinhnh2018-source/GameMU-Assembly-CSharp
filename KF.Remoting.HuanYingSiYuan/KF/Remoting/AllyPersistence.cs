using System;
using System.Collections.Generic;
using System.Xml.Linq;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;
using Tmsk.Tools.Tools;

namespace KF.Remoting
{
	public class AllyPersistence
	{
		private AllyPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				this.DataVersion = TimeUtil.NowDateTime().Ticks;
				XElement xelement = ConfigHelper.Load("config.xml");
				Consts.AllyNumMax = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "AllyNumMax", "value", 5L);
				Consts.AllyRequestClearSecond = (int)ConfigHelper.GetElementAttributeValueLong(xelement, "add", "key", "AllyRequestClearSecond", "value", 86400L);
				this.Initialized = true;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
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
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return result;
		}

		public List<AllyLogData> DBAllyLogList(int unionID)
		{
			List<AllyLogData> list = new List<AllyLogData>();
			try
			{
				string strSQL = string.Format("SELECT l.myUnionID,l.unionID,u.unionZoneID,u.unionName,l.logTime,l.logState\r\n                                                FROM t_ally_log l,t_ally_union u\r\n                                                WHERE l.unionID = u.unionID AND l.unionID='{0}'", unionID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					list.Add(new AllyLogData
					{
						MyUnionID = Convert.ToInt32(mySqlDataReader["myUnionID"]),
						UnionID = Convert.ToInt32(mySqlDataReader["unionID"]),
						LogTime = Convert.ToDateTime(mySqlDataReader["logTime"]),
						LogState = Convert.ToInt32(mySqlDataReader["logState"])
					});
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		public bool DBAllyLogDel(int unionID)
		{
			string sqlCmd = string.Format("DELETE FROM t_ally_log WHERE myUnionID='{0}'", unionID);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public bool DBAllyLogAdd(AllyLogData logData)
		{
			string sqlCmd = string.Format("INSERT INTO t_ally_log(myUnionID, unionID, logTime,logState) VALUES('{0}','{1}','{2}','{3}')", new object[]
			{
				logData.MyUnionID,
				logData.UnionID,
				logData.LogTime,
				logData.LogState
			});
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public KFAllyData DBUnionDataGet(int unionID)
		{
			KFAllyData kfallyData = null;
			try
			{
				string strSQL = string.Format("SELECT unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID\r\n                                                FROM t_ally_union\r\n                                                WHERE unionID='{0}'", unionID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					kfallyData = new KFAllyData();
					kfallyData.UnionID = Convert.ToInt32(mySqlDataReader["unionID"]);
					kfallyData.UnionZoneID = Convert.ToInt32(mySqlDataReader["unionZoneID"]);
					kfallyData.UnionName = mySqlDataReader["unionName"].ToString();
					kfallyData.UnionLevel = Convert.ToInt32(mySqlDataReader["unionLevel"]);
					kfallyData.UnionNum = Convert.ToInt32(mySqlDataReader["unionNum"]);
					kfallyData.LeaderID = Convert.ToInt32(mySqlDataReader["leaderID"]);
					kfallyData.LeaderZoneID = Convert.ToInt32(mySqlDataReader["leaderZoneID"]);
					kfallyData.LeaderName = mySqlDataReader["leaderName"].ToString();
					kfallyData.LogTime = Convert.ToDateTime(mySqlDataReader["logTime"]);
					kfallyData.ServerID = Convert.ToInt32(mySqlDataReader["serverID"]);
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return kfallyData;
		}

		public KFAllyData DBUnionDataGet(int unionZoneID, string unionName)
		{
			KFAllyData kfallyData = null;
			try
			{
				string strSQL = string.Format("SELECT unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID\r\n                                                FROM t_ally_union\r\n                                                WHERE unionZoneID='{0}' and unionName='{1}'", unionZoneID, unionName);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					kfallyData = new KFAllyData();
					kfallyData.UnionID = Convert.ToInt32(mySqlDataReader["unionID"]);
					kfallyData.UnionZoneID = Convert.ToInt32(mySqlDataReader["unionZoneID"]);
					kfallyData.UnionName = mySqlDataReader["unionName"].ToString();
					kfallyData.UnionLevel = Convert.ToInt32(mySqlDataReader["unionLevel"]);
					kfallyData.UnionNum = Convert.ToInt32(mySqlDataReader["unionNum"]);
					kfallyData.LeaderID = Convert.ToInt32(mySqlDataReader["leaderID"]);
					kfallyData.LeaderZoneID = Convert.ToInt32(mySqlDataReader["leaderZoneID"]);
					kfallyData.LeaderName = mySqlDataReader["leaderName"].ToString();
					kfallyData.LogTime = Convert.ToDateTime(mySqlDataReader["logTime"]);
					kfallyData.ServerID = Convert.ToInt32(mySqlDataReader["serverID"]);
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return kfallyData;
		}

		public bool DBUnionDataUpdate(KFAllyData data)
		{
			string sqlCmd = string.Format("REPLACE INTO t_ally_union(unionID,unionZoneID,unionName,unionLevel,unionNum,leaderID,leaderZoneID,leaderName,logTime,serverID) \r\n                                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}','{8}','{9}')", new object[]
			{
				data.UnionID,
				data.UnionZoneID,
				data.UnionName,
				data.UnionLevel,
				data.UnionNum,
				data.LeaderID,
				data.LeaderZoneID,
				data.LeaderName,
				data.LogTime,
				data.ServerID
			});
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public bool DBUnionDataDel(int unionID)
		{
			string sqlCmd = string.Format("DELETE FROM t_ally_union WHERE unionID={0}", unionID);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public List<int> DBAllyIDList(int unionID)
		{
			List<int> list = new List<int>();
			try
			{
				string strSQL = string.Format("SELECT DISTINCT(unionID2) uid from t_ally where unionID1='{0}' \r\n                                                UNION\r\n                                                SELECT DISTINCT(unionID1) uid from t_ally where unionID2='{0}' \r\n                                                ORDER BY uid", unionID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					list.Add(Convert.ToInt32(mySqlDataReader["uid"]));
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		public bool DBAllyAdd(int myUnionID, int unionID, DateTime logTime)
		{
			string sqlCmd = string.Format("INSERT INTO t_ally(unionID1, unionID2, logTime) VALUES('{0}','{1}','{2}')", myUnionID, unionID, logTime);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public bool DBAllyDel(int unionID, int targetID)
		{
			string sqlCmd = string.Format("DELETE FROM t_ally WHERE (unionID1='{0}' and unionID2='{1}') or(unionID1='{1}' and unionID2='{0}')", unionID, targetID);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public List<KFAllyData> DBAllyRequestList(int unionID)
		{
			List<KFAllyData> list = new List<KFAllyData>();
			try
			{
				string strSQL = string.Format("SELECT unionID,logTime,logState FROM t_ally_request WHERE myUnionID='{0}'", unionID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					list.Add(new KFAllyData
					{
						UnionID = Convert.ToInt32(mySqlDataReader["unionID"]),
						LogTime = Convert.ToDateTime(mySqlDataReader["logTime"]),
						LogState = Convert.ToInt32(mySqlDataReader["logState"])
					});
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		public bool DBAllyRequestAdd(int myUnionID, int unionID, DateTime logTime, int logState)
		{
			string sqlCmd = string.Format("INSERT INTO t_ally_request(myUnionID, unionID, logTime,logState) VALUES('{0}','{1}','{2}','{3}')", new object[]
			{
				myUnionID,
				unionID,
				logTime,
				logState
			});
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public bool DBAllyRequestDel(int myUnionID, int unionID)
		{
			string sqlCmd = string.Format("DELETE FROM t_ally_request WHERE myUnionID='{0}' and unionID='{1}'", myUnionID, unionID);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public List<KFAllyData> DBAllyAcceptList(int unionID)
		{
			List<KFAllyData> list = new List<KFAllyData>();
			try
			{
				string strSQL = string.Format("SELECT myUnionID,logTime,logState FROM t_ally_request WHERE unionID='{0}'", unionID);
				MySqlDataReader mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					list.Add(new KFAllyData
					{
						UnionID = Convert.ToInt32(mySqlDataReader["myUnionID"]),
						LogTime = Convert.ToDateTime(mySqlDataReader["logTime"]),
						LogState = Convert.ToInt32(mySqlDataReader["logState"])
					});
				}
				if (mySqlDataReader != null)
				{
					mySqlDataReader.Close();
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
			return list;
		}

		public static readonly AllyPersistence Instance = new AllyPersistence();

		public long DataVersion = 0L;

		public bool Initialized = false;
	}
}
