using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class JingLingYuanSuJueXingManager : SingletonTemplate<JingLingYuanSuJueXingManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1452, SingletonTemplate<JingLingYuanSuJueXingManager>.Instance());
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
			if (nID == 1452)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		private void ProcessArmorLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int cmdData = 0;
			RoleDataCmdT<JingLingYuanSuJueXingData> roleDataCmdT = DataHelper.BytesToObject<RoleDataCmdT<JingLingYuanSuJueXingData>>(cmdParams, 0, count);
			if (roleDataCmdT != null && roleDataCmdT.RoleID > 0)
			{
				DBManager instance = DBManager.getInstance();
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleDataCmdT.RoleID);
				if (null != dbroleInfo)
				{
					dbroleInfo.JingLingYuanSuJueXingData = roleDataCmdT.Value;
					using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
					{
						string sql = string.Format("insert into t_juexing_jlys(rid,activetype,activeids) values({0},{1},'{2}') on duplicate key update activetype={1},activeids='{2}'", roleDataCmdT.RoleID, roleDataCmdT.Value.ActiveType, string.Join<int>(",", roleDataCmdT.Value.ActiveIDs));
						cmdData = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
					}
				}
			}
			client.sendCmd<int>(nID, cmdData);
		}

		private object Mutex = new object();
	}
}
