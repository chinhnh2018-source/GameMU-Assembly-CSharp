using System;
using System.Collections.Generic;
using System.Text;
using GameServer.Core.Executor;
using Maticsoft.DBUtility;
using MySql.Data.MySqlClient;
using Server.Tools;
using Tmsk.Contract.KuaFuData;

namespace KF.Remoting
{
	public class RankPersistence
	{
		private RankPersistence()
		{
		}

		public void InitConfig()
		{
			try
			{
				this.DataVersion = TimeUtil.NowDateTime().Ticks;
			}
			catch (Exception ex)
			{
				LogManager.WriteExceptionUseCache(ex.ToString());
			}
		}

		public List<KFRankData> DBRankLoad(int rankType, int max)
		{
			MySqlDataReader mySqlDataReader = null;
			List<KFRankData> list = new List<KFRankData>();
			try
			{
				string strSQL = string.Format("select rankType,rank,zoneID,roleID,roleName,grade,rankData,rankTime,serverID from t_rank where rankType='{0}' \r\n                            ORDER BY grade DESC, rankTime ASC, roleid ASC limit {1} ", rankType, max);
				mySqlDataReader = DbHelperMySQL.ExecuteReader(strSQL, false);
				while (mySqlDataReader != null && mySqlDataReader.Read())
				{
					KFRankData kfrankData = new KFRankData();
					kfrankData.RankType = Convert.ToInt32(mySqlDataReader["rankType"]);
					kfrankData.Rank = Convert.ToInt32(mySqlDataReader["rank"]);
					kfrankData.ZoneID = Convert.ToInt32(mySqlDataReader["zoneID"]);
					kfrankData.RoleID = Convert.ToInt32(mySqlDataReader["roleID"]);
					kfrankData.RoleName = mySqlDataReader["roleName"].ToString();
					kfrankData.Grade = Convert.ToInt32(mySqlDataReader["grade"]);
					if (!mySqlDataReader.IsDBNull(mySqlDataReader.GetOrdinal("rankData")))
					{
						kfrankData.RoleData = (byte[])mySqlDataReader["rankData"];
					}
					kfrankData.RankTime = Convert.ToDateTime(mySqlDataReader["rankTime"]);
					kfrankData.ServerID = Convert.ToInt32(mySqlDataReader["serverID"]);
					kfrankData.RankOld = kfrankData.Rank;
					list.Add(kfrankData);
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

		public bool DBRankDataUpdate(KFRankData data)
		{
			string strSQL = string.Format("REPLACE INTO t_rank(rankType,rank,zoneID,roleID,roleName,grade,rankTime,serverID,rankData) \r\n                                        VALUES('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}',@roleData)", new object[]
			{
				data.RankType,
				data.Rank,
				data.ZoneID,
				data.RoleID,
				data.RoleName,
				data.Grade,
				data.RankTime,
				data.ServerID
			});
			DbHelperMySQL.ExecuteSqlInsertImg(strSQL, new List<Tuple<string, byte[]>>
			{
				new Tuple<string, byte[]>("roleData", data.RoleData)
			});
			return true;
		}

		public void DBRankUpdate(List<KFRankData> list)
		{
			int num = 0;
			StringBuilder stringBuilder = new StringBuilder();
			foreach (KFRankData kfrankData in list)
			{
				if (kfrankData.Rank == kfrankData.RankOld)
				{
					num++;
				}
				else
				{
					stringBuilder.AppendFormat("UPDATE t_rank SET rank={0} WHERE rankType={1} AND roleID={2};", kfrankData.Rank, kfrankData.RankType, kfrankData.RoleID);
					stringBuilder.AppendLine();
					if ((num % 50 == 0 || num == list.Count - 1) && stringBuilder.Length > 0)
					{
						string sqlCmd = stringBuilder.ToString();
						stringBuilder.Clear();
						this.ExecuteSqlNoQuery(sqlCmd);
					}
					num++;
				}
			}
			if (stringBuilder.Length > 0)
			{
				string sqlCmd = stringBuilder.ToString();
				stringBuilder.Clear();
				this.ExecuteSqlNoQuery(sqlCmd);
			}
		}

		public bool DBRankDelMore(int rankType, int rankMax)
		{
			string sqlCmd = string.Format("delete from t_rank where rankType={0} and rank>{1}", rankType, rankMax);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
		}

		public bool DBRankDelByType(int rankType)
		{
			string sqlCmd = string.Format("delete from t_rank where rankType={0}", rankType);
			int num = this.ExecuteSqlNoQuery(sqlCmd);
			return num > 0;
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

		public long DataVersion = 0L;

		public static readonly RankPersistence Instance = new RankPersistence();
	}
}
