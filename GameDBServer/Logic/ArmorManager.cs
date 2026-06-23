using System;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class ArmorManager : SingletonTemplate<ArmorManager>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(1447, SingletonTemplate<ArmorManager>.Instance());
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
			if (nID == 1447)
			{
				this.ProcessArmorLevelStarUpCmd(client, nID, cmdParams, count);
			}
		}

		private void ProcessArmorLevelStarUpCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int num = 0;
			RoleDataCmdT<RoleArmorData> roleDataCmdT = DataHelper.BytesToObject<RoleDataCmdT<RoleArmorData>>(cmdParams, 0, count);
			if (roleDataCmdT != null && roleDataCmdT.RoleID > 0)
			{
				DBManager instance = DBManager.getInstance();
				DBRoleInfo dbroleInfo = instance.GetDBRoleInfo(ref roleDataCmdT.RoleID);
				if (null != dbroleInfo)
				{
					if (dbroleInfo.ArmorData.Armor != roleDataCmdT.Value.Armor || dbroleInfo.ArmorData.Exp != roleDataCmdT.Value.Exp)
					{
						dbroleInfo.ArmorData.Armor = roleDataCmdT.Value.Armor;
						dbroleInfo.ArmorData.Exp = roleDataCmdT.Value.Exp;
						using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
						{
							string sql = string.Format("update t_roles set armor={1},armorexp={2} where rid={0}", roleDataCmdT.RoleID, roleDataCmdT.Value.Armor, roleDataCmdT.Value.Exp);
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
