using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using GameServer.Server;
using Server.Data;
using Server.Protocol;

namespace GameServer.Logic
{
	public class TeamManager
	{
		public int GetNextAutoID()
		{
			return (int)(Interlocked.Increment(ref this.BaseAutoID) & 2147483647L);
		}

		public void AddRoleID2TeamID(int roleID, int teamID)
		{
			lock (this._RoleID2TeamIDDict)
			{
				this._RoleID2TeamIDDict[roleID] = teamID;
			}
		}

		public void RemoveRoleID2TeamID(int roleID)
		{
			lock (this._RoleID2TeamIDDict)
			{
				if (this._RoleID2TeamIDDict.ContainsKey(roleID))
				{
					this._RoleID2TeamIDDict.Remove(roleID);
				}
			}
		}

		public int FindRoleID2TeamID(int roleID)
		{
			int result = -1;
			lock (this._RoleID2TeamIDDict)
			{
				if (this._RoleID2TeamIDDict.TryGetValue(roleID, out result))
				{
					return result;
				}
			}
			return result;
		}

		public void CreateTeam(TCPManager tcpMgr, TCPOutPacketPool pool, GameClient client)
		{
			int nextAutoID = GameManager.TeamMgr.GetNextAutoID();
			client.ClientData.TeamID = nextAutoID;
			this.AddRoleID2TeamID(client.ClientData.RoleID, client.ClientData.TeamID);
			TeamData teamData = new TeamData
			{
				TeamID = nextAutoID,
				LeaderRoleID = client.ClientData.RoleID,
				AddDateTime = TimeUtil.NOW(),
				GetThingOpt = 1
			};
			if (null == teamData.TeamRoles)
			{
				teamData.TeamRoles = new List<TeamMemberData>();
			}
			teamData.TeamRoles.Add(Global.ClientDataToTeamMemberData(client.ClientData));
			this.AddData(nextAutoID, teamData);
			GameManager.ClientMgr.NotifyTeamData(tcpMgr.MySocketListener, pool, teamData);
			GameManager.ClientMgr.NotifyOthersTeamIDChanged(tcpMgr.MySocketListener, pool, client);
		}

		public void AddData(int teamID, TeamData td)
		{
			lock (this._TeamDataDict)
			{
				this._TeamDataDict[teamID] = td;
			}
		}

		public void RemoveData(int teamID)
		{
			lock (this._TeamDataDict)
			{
				if (this._TeamDataDict.ContainsKey(teamID))
				{
					this._TeamDataDict.Remove(teamID);
				}
			}
		}

		public TeamData FindData(int teamID)
		{
			TeamData result = null;
			lock (this._TeamDataDict)
			{
				this._TeamDataDict.TryGetValue(teamID, out result);
			}
			return result;
		}

		public int GetTotalDataCount()
		{
			int result = 0;
			lock (this._TeamDataDict)
			{
				result = this._TeamDataDict.Count;
			}
			return result;
		}

		public List<TeamData> GetTeamDataList(int startIndex, int count)
		{
			int num = 0;
			List<TeamData> list = new List<TeamData>();
			lock (this._TeamDataDict)
			{
				foreach (TeamData item in this._TeamDataDict.Values)
				{
					if (num < startIndex)
					{
						num++;
					}
					else
					{
						list.Add(item);
						if (list.Count >= count)
						{
							break;
						}
						num++;
					}
				}
			}
			return list;
		}

		public bool CanAddToTeam(int roleID, int teamID, int requestType)
		{
			TeamRequestItem teamRequestItem = null;
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			lock (this._TeamRequestDict)
			{
				if (!this._TeamRequestDict.TryGetValue(key, out teamRequestItem))
				{
					return true;
				}
			}
			long num = TimeUtil.NOW();
			return num - teamRequestItem.RequestTicks >= 35000L;
		}

		public void AddTeamRequestItem(int roleID, int teamID, int requestType)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			TeamRequestItem value = new TeamRequestItem
			{
				ToRoleID = roleID,
				RequestTicks = TimeUtil.NOW()
			};
			lock (this._TeamRequestDict)
			{
				this._TeamRequestDict[key] = value;
			}
		}

		public void RemoveTeamRequestItem(int roleID, int teamID, int requestType)
		{
			string key = string.Format("{0}_{1}_{2}", roleID, teamID, requestType);
			lock (this._TeamRequestDict)
			{
				this._TeamRequestDict.Remove(key);
			}
		}

		private long BaseAutoID = 0L;

		private Dictionary<int, int> _RoleID2TeamIDDict = new Dictionary<int, int>();

		private Dictionary<int, TeamData> _TeamDataDict = new Dictionary<int, TeamData>();

		private Dictionary<string, TeamRequestItem> _TeamRequestDict = new Dictionary<string, TeamRequestItem>();
	}
}
