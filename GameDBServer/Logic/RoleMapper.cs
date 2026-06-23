using System;
using System.Collections.Generic;
using GameDBServer.DB;
using GameDBServer.Server;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Logic
{
	public class RoleMapper : SingletonTemplate<RoleMapper>, IManager, ICmdProcessor
	{
		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
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
			if (nID == 10230)
			{
				this.RoleCustomDataQuery(client, nID, cmdParams, count);
			}
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
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteException(ex.ToString());
			}
			client.sendCmd<RoleCustomData>(nID, roleCustomData);
		}

		public void SetTempRoleID(int localRoleID, int tempRoleID)
		{
			lock (this.Mutex)
			{
				KuaFuWorldRoleData value = new KuaFuWorldRoleData
				{
					LocalRoleID = localRoleID,
					TempRoleID = tempRoleID
				};
				this.KuaFuWorldRoleDataDict[tempRoleID] = value;
			}
		}

		public int GetLocalRoleIDByTempID(int tempRoleID)
		{
			lock (this.Mutex)
			{
				KuaFuWorldRoleData kuaFuWorldRoleData;
				if (this.KuaFuWorldRoleDataDict.TryGetValue(tempRoleID, out kuaFuWorldRoleData))
				{
					return kuaFuWorldRoleData.LocalRoleID;
				}
			}
			return 0;
		}

		public const int MinLocalRoleID = 200000;

		private object Mutex = new object();

		private Dictionary<int, KuaFuWorldRoleData> KuaFuWorldRoleDataDict = new Dictionary<int, KuaFuWorldRoleData>();
	}
}
