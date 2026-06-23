using System;
using System.Collections.Generic;
using KF.Client;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class KuaFuWorldManager : SingletonTemplate<KuaFuWorldManager>
	{
		public bool IsTempRoleID(int roleID)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.TempRoleIDs.Contains(roleID);
			}
			return result;
		}

		public bool CheckPTKuaFuLoginSign(KuaFuServerLoginData data)
		{
			string str = null;
			string worldRoleID = ConstData.FormatWorldRoleID(data.RoleId, data.PTID);
			string[] ips;
			int[] ports;
			int num = KuaFuWorldClient.getInstance().CheckEnterWorldKuaFuSign(worldRoleID, data.SignToken, out str, out ips, out ports);
			bool result;
			if (num < 0)
			{
				LogManager.WriteLog(2, string.Format("CheckEnterWorldKuaFuSign faild,roleid={0},ptid={1},result={2}", data.RoleId, data.PTID, num), null, true);
				result = false;
			}
			else
			{
				string a = MD5Helper.get_md5_string(data.SignDataString() + str).ToLower();
				if (a != data.SignCode)
				{
					LogManager.WriteLog(2, string.Format("CheckEnterWorldKuaFuSign SignCode Error,roleid={0},ptid={1},SignCode={2}", data.RoleId, data.PTID, data.SignCode), null, true);
					result = false;
				}
				else
				{
					data.ips = ips;
					data.ports = ports;
					result = true;
				}
			}
			return result;
		}

		public KuaFuWorldRoleData GetWorldRoleData(int roleID, int serverID, string userID, int tempRoleID)
		{
			string channelNameByUserID = Data.GetChannelNameByUserID(userID);
			KuaFuWorldRoleData kuaFuWorldRoleData = null;
			lock (this.Mutex)
			{
				if (roleID != tempRoleID)
				{
					this.TempRoleIDs.Add(tempRoleID);
				}
				if (!this.WorldRoleDataDict.TryGetValue(roleID, out kuaFuWorldRoleData))
				{
					kuaFuWorldRoleData = new KuaFuWorldRoleData();
					kuaFuWorldRoleData.LocalRoleID = roleID;
					kuaFuWorldRoleData.TempRoleID = roleID;
					kuaFuWorldRoleData.UserID = userID;
					kuaFuWorldRoleData.ServerID = serverID;
					kuaFuWorldRoleData.Channel = channelNameByUserID;
					this.WorldRoleDataDict[roleID] = kuaFuWorldRoleData;
				}
				else
				{
					if (kuaFuWorldRoleData.TempRoleID == tempRoleID)
					{
						kuaFuWorldRoleData.ServerID = serverID;
						return kuaFuWorldRoleData;
					}
					kuaFuWorldRoleData = new KuaFuWorldRoleData();
					kuaFuWorldRoleData.LocalRoleID = roleID;
					kuaFuWorldRoleData.TempRoleID = tempRoleID;
					kuaFuWorldRoleData.UserID = userID;
					kuaFuWorldRoleData.ServerID = serverID;
					kuaFuWorldRoleData.Channel = channelNameByUserID;
					kuaFuWorldRoleData.UseTempRoleID = true;
				}
			}
			return kuaFuWorldRoleData;
		}

		private object Mutex = new object();

		private Dictionary<int, KuaFuWorldRoleData> WorldRoleDataDict = new Dictionary<int, KuaFuWorldRoleData>();

		private HashSet<int> TempRoleIDs = new HashSet<int>();
	}
}
