using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class RoleManager : SingletonTemplate<RoleManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(10230, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(10232, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(10233, SingletonTemplate<RoleManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(694, SingletonTemplate<RoleManager>.Instance());
			return true;
		}

		public bool showdown()
		{
			return true;
		}

		public bool destroy()
		{
			return true;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			if (nID != 694)
			{
				switch (nID)
				{
				case 10230:
					this.RoleCustomDataQuery(client, nID, cmdParams, count);
					break;
				case 10232:
					this.RoleDataSelectorQuery(client, nID, cmdParams, count);
					break;
				case 10233:
					this.RoleCustomDataUpdate(client, nID, cmdParams, count);
					break;
				}
			}
			else
			{
				this.FastCacheDataUpdate(client, nID, cmdParams, count);
			}
		}

		public RoleData4Selector GetRoleData4Selector(int roleID, bool mainOccupation = false)
		{
			DBManager instance = DBManager.getInstance();
			RoleData4Selector roleData4Selector = new RoleData4Selector();
			roleData4Selector.RoleID = -1;
			try
			{
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleID);
				if (null != dbroleInfo)
				{
					if (mainOccupation && dbroleInfo.OccupationList[0] != dbroleInfo.Occupation)
					{
						if (dbroleInfo.roleCustomData == null || null == dbroleInfo.roleCustomData.roleData4Selector)
						{
							dbroleInfo.roleCustomData = this.QueryRoleCustomData(roleID);
						}
						if (null != dbroleInfo.roleCustomData)
						{
							return dbroleInfo.roleCustomData.roleData4Selector;
						}
					}
					else
					{
						lock (dbroleInfo)
						{
							Global.DBRoleInfo2RoleData4Selector(instance, dbroleInfo, roleData4Selector);
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			return roleData4Selector;
		}

		private void RoleCustomDataQuery(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			DBManager instance = DBManager.getInstance();
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			RoleCustomData roleCustomData = new RoleCustomData();
			roleCustomData.roleId = roleId;
			try
			{
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleId);
				if (null != dbroleInfo)
				{
					if (null == dbroleInfo.roleCustomData)
					{
						dbroleInfo.roleCustomData = this.QueryRoleCustomData(roleId);
					}
					if (null != dbroleInfo.roleCustomData)
					{
						roleCustomData.customDataList = dbroleInfo.roleCustomData.customDataList;
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<RoleCustomData>(nID, roleCustomData);
		}

		private void RoleDataSelectorQuery(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleID = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			RoleData4Selector roleData4Selector = this.GetRoleData4Selector(roleID, true);
			client.sendCmd<RoleData4Selector>(nID, roleData4Selector);
		}

		private void RoleCustomDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			RoleCustomData roleCustomData = DataHelper.BytesToObject<RoleCustomData>(cmdParams, 0, count);
			if (roleCustomData != null)
			{
				using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
				{
					if (roleCustomData.customDataList != null && roleCustomData.roleData4Selector != null)
					{
						string arg = DataHelper.ObjectToHexString<RoleData4Selector>(roleCustomData.roleData4Selector);
						string arg2 = DataHelper.ObjectToHexString<List<RoleCustomDataItem>>(roleCustomData.customDataList);
						string sql = string.Format("insert into t_roledata(`rid`,`roledata4selector`,`occu_data`) VALUE({0},{1},{2}) on duplicate key update `roledata4selector`={1},`occu_data`={2}", roleCustomData.roleId, arg, arg2);
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
					else if (roleCustomData.roleData4Selector != null)
					{
						string arg = DataHelper.ObjectToHexString<RoleData4Selector>(roleCustomData.roleData4Selector);
						string sql = string.Format("INSERT INTO t_roledata(`rid`,`roledata4selector`) VALUE({0},{1}) on duplicate key update `roledata4selector`={1}", roleCustomData.roleId, arg);
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
					else if (roleCustomData.customDataList != null)
					{
						string arg = DataHelper.ObjectToHexString<List<RoleCustomDataItem>>(roleCustomData.customDataList);
						string sql = string.Format("INSERT INTO t_roledata(`rid`,`occu_data`) VALUE({0},{1}) on duplicate key update `occu_data`={1}", roleCustomData.roleId, arg);
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
				}
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref roleCustomData.roleId);
				if (null != dbroleInfo)
				{
					if (null == roleCustomData.roleData4Selector)
					{
						roleCustomData.roleData4Selector = dbroleInfo.roleCustomData.roleData4Selector;
					}
					dbroleInfo.roleCustomData = roleCustomData;
				}
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private RoleCustomData QueryRoleCustomData(int roleId)
		{
			RoleCustomData roleCustomData = null;
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				try
				{
					string sql = string.Format("select `occu_data`,`roledata4selector` from t_roledata where rid={0}", roleId);
					MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
					if (mySQLDataReader.Read())
					{
						byte[] array = mySQLDataReader[0] as byte[];
						byte[] array2 = mySQLDataReader[1] as byte[];
						roleCustomData = new RoleCustomData();
						roleCustomData.roleId = roleId;
						if (null != array)
						{
							roleCustomData.customDataList = DataHelper.BytesToObject<List<RoleCustomDataItem>>(array, 0, array.Length);
						}
						if (null != array2)
						{
							roleCustomData.roleData4Selector = DataHelper.BytesToObject<RoleData4Selector>(array2, 0, array2.Length);
						}
					}
				}
				catch (Exception ex)
				{
					LogManager.WriteException(ex.ToString());
				}
			}
			return roleCustomData;
		}

		private void FastCacheDataUpdate(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			FastCacheData fastCacheData = DataHelper.BytesToObject<FastCacheData>(cmdParams, 0, count);
			if (fastCacheData != null)
			{
				int id = fastCacheData.ID;
				DBRoleInfo dbroleInfo = DBManager.getInstance().GetDBRoleInfo(ref id);
				if (null != dbroleInfo)
				{
					if (fastCacheData.Flag_BaseInfo)
					{
						dbroleInfo.CombatForce = (int)fastCacheData.ZhanLi;
						dbroleInfo.Position = fastCacheData.Position;
					}
				}
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private const int ALLY_LOG_COUNT_MAX = 20;

		private object Mutex = new object();
	}
}
