using System;
using System.Collections.Generic;
using GameDBServer.Logic.WanMoTa;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.DB.DBController
{
	public class WanMoTaDBController : DBController<WanMotaInfo>
	{
		private WanMoTaDBController()
		{
		}

		public static WanMoTaDBController getInstance()
		{
			return WanMoTaDBController.instance;
		}

		public WanMotaInfo getPlayerWanMoTaDataById(int Id)
		{
			string sql = string.Format("select * from t_wanmota where roleID = {0};", Id);
			return base.queryForObject(sql);
		}

		public List<WanMotaInfo> getPlayerWanMotaDataList()
		{
			string sql = string.Format("select * from t_wanmota order by passLayerCount desc, flushTime asc limit {0};", WanMoTaManager.RankingList_Max_Num);
			return base.queryForList(sql);
		}

		public static int updateWanMoTaData(DBManager dbMgr, int nRoleID, string[] fields, int startIndex)
		{
			int result = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = DBWriter.FormatUpdateSQL(nRoleID, fields, startIndex, WanMoTaDBController._fieldNames, "t_wanmota", WanMoTaDBController._fieldTypes, "roleID");
				result = myDbConnection.ExecuteNonQuery(sql, 0);
			}
			return result;
		}

		public int insertWanMoTaData(DBManager dbMgr, WanMotaInfo data)
		{
			int num = -1;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("INSERT INTO t_wanmota (roleID, roleName, flushTime, passLayerCount, sweepLayer, sweepReward, sweepBeginTime) VALUES({0}, '{1}', {2}, {3}, {4}, '{5}', {6})", new object[]
					{
						data.nRoleID,
						data.strRoleName,
						data.lFlushTime,
						data.nPassLayerCount,
						data.nSweepLayer,
						data.strSweepReward,
						data.lSweepBeginTime
					});
					num = myDbConnection.ExecuteNonQuery(sql, 0);
					if (num < 0)
					{
						return num;
					}
					num = myDbConnection.GetSingleInt("SELECT LAST_INSERT_ID() AS LastID", 0, new MySQLParameter[0]);
				}
				catch (MySQLException)
				{
					num = -2;
				}
			}
			return num;
		}

		internal void OnChangeName(int roleid, string oldName, string newName)
		{
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("UPDATE t_wanmota SET roleName='{0}' WHERE roleId={1}", newName, roleid);
				if (myDbConnection.ExecuteNonQuery(sql, 0) < 0)
				{
					LogManager.WriteLog(LogTypes.Error, string.Format("角色改名，更新t_wanmota失败, roleId={0}, oldName={1}, newName={2}", roleid, oldName, newName), null, true);
				}
			}
		}

		// Note: this type is marked as 'beforefieldinit'.
		static WanMoTaDBController()
		{
			byte[] array = new byte[5];
			array[3] = 1;
			WanMoTaDBController._fieldTypes = array;
		}

		private static WanMoTaDBController instance = new WanMoTaDBController();

		private static readonly string[] _fieldNames = new string[]
		{
			"flushTime",
			"passLayerCount",
			"sweepLayer",
			"sweepReward",
			"sweepBeginTime"
		};

		private static readonly byte[] _fieldTypes;
	}
}
