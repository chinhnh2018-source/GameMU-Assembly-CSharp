using System;
using System.Text;
using GameDBServer.DB;
using GameDBServer.Server;
using MySQLDriverCS;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class KingRoleDataManager : SingletonTemplate<KingRoleDataManager>, IManager, ICmdProcessor
	{
		private KingRoleDataManager()
		{
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(13230, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13231, SingletonTemplate<KingRoleDataManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(13232, SingletonTemplate<KingRoleDataManager>.Instance());
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
			if (nID == 13230)
			{
				this.HandleGet(client, nID, cmdParams, count);
			}
			else if (nID == 13231)
			{
				this.HandlePut(client, nID, cmdParams, count);
			}
			else if (nID == 13232)
			{
				this.HandleClr(client, nID, cmdParams, count);
			}
		}

		private void HandleClr(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			string @string = new UTF8Encoding().GetString(cmdParams, 0, count);
			string[] array = @string.Split(new char[]
			{
				':'
			});
			int num = Convert.ToInt32(array[0]);
			KingRoleGetData kingRoleGetData = DataHelper.BytesToObject<KingRoleGetData>(cmdParams, 0, count);
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("DELETE FROM t_king_role_data WHERE king_type={0}", num);
				client.sendCmd<bool>(nID, myDbConnection.ExecuteNonQueryBool(sql, 0));
			}
		}

		private void HandleGet(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			KingRoleGetData kingRoleGetData = DataHelper.BytesToObject<KingRoleGetData>(cmdParams, 0, count);
			string sql = string.Format("SELECT roledata_ex FROM t_king_role_data WHERE king_type={0}", kingRoleGetData.KingType);
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				MySQLDataReader mySQLDataReader = myDbConnection.ExecuteReader(sql, new MySQLParameter[0]);
				RoleDataEx cmdData = new RoleDataEx
				{
					RoleID = -1
				};
				if (mySQLDataReader.Read())
				{
					string @string = new ASCIIEncoding().GetString((byte[])mySQLDataReader["roledata_ex"]);
					byte[] array = Convert.FromBase64String(@string);
					cmdData = DataHelper.BytesToObject<RoleDataEx>(array, 0, array.Length);
				}
				client.sendCmd<RoleDataEx>(nID, cmdData);
			}
		}

		private void HandlePut(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			KingRolePutData kingRolePutData = DataHelper.BytesToObject<KingRolePutData>(cmdParams, 0, count);
			byte[] inArray = DataHelper.ObjectToBytes<RoleDataEx>(kingRolePutData.RoleDataEx);
			string text = Convert.ToBase64String(inArray);
			using (MyDbConnection3 myDbConnection = new MyDbConnection3(false))
			{
				string sql = string.Format("REPLACE INTO t_king_role_data(king_type,role_id,mod_time,roledata_ex) VALUES({0},{1},'{2}','{3}')", new object[]
				{
					kingRolePutData.KingType,
					kingRolePutData.RoleDataEx.RoleID,
					DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
					text
				});
				client.sendCmd<bool>(nID, myDbConnection.ExecuteNonQueryBool(sql, 0));
			}
		}
	}
}
