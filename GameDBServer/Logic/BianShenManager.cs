using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class BianShenManager : SingletonTemplate<BianShenManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1449, SingletonTemplate<BianShenManager>.Instance());
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
			if (nID == 1449)
			{
				this.ProcessBianShenLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		private void ProcessBianShenLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = 0;
			RoleDataCmdT<RoleBianShenData> roleDataCmdT = DataHelper.BytesToObject<RoleDataCmdT<RoleBianShenData>>(cmdParams, 0, count);
			if (roleDataCmdT != null && roleDataCmdT.RoleID > 0)
			{
				DBManager instance = DBManager.getInstance();
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleDataCmdT.RoleID);
				if (null != dbroleInfo)
				{
					if (dbroleInfo.BianShenData.BianShen != roleDataCmdT.Value.BianShen || dbroleInfo.BianShenData.Exp != roleDataCmdT.Value.Exp)
					{
						dbroleInfo.BianShenData.BianShen = roleDataCmdT.Value.BianShen;
						dbroleInfo.BianShenData.Exp = roleDataCmdT.Value.Exp;
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("update t_roles set bianshen={1},bianshenexp={2} where rid={0}", roleDataCmdT.RoleID, roleDataCmdT.Value.BianShen, roleDataCmdT.Value.Exp);
							num = myDbConnection.ExecuteSql(sql, new MySQLParameter[0]);
						}
					}
				}
			}
			client.sendCmd(nID, string.Format("{0}", num));
		}

		private object Mutex = new object();
	}
}
