using System;
using System.Collections.Generic;
using System.Threading;
using GameServer.Core.Executor;
using Server.Data;

namespace GameServer.Logic
{
	public class DJRoomManager
	{
		public object Mutex
		{
			get
			{
				return this.mutex;
			}
		}

		public int GetNewRoomID()
		{
			int result = 1;
			lock (this.mutex)
			{
				result = this.BaseRoomID++;
			}
			return result;
		}

		public List<DJRoomData> CloneRoomDataList()
		{
			List<DJRoomData> result = null;
			lock (this.mutex)
			{
				result = this.DJRoomDataList.GetRange(0, this.DJRoomDataList.Count);
			}
			return result;
		}

		public DJRoomData FindRoomData(int roomID)
		{
			DJRoomData result = null;
			lock (this.mutex)
			{
				this.DJRoomDict.TryGetValue(roomID, out result);
			}
			return result;
		}

		public void AddRoomData(DJRoomData roomData)
		{
			lock (this.mutex)
			{
				this.DJRoomDict[roomData.RoomID] = roomData;
				this.DJRoomDataList.Add(roomData);
			}
		}

		public void RemoveRoomData(int roomID)
		{
			lock (this.mutex)
			{
				DJRoomData item = null;
				if (this.DJRoomDict.TryGetValue(roomID, out item))
				{
					this.DJRoomDict.Remove(roomID);
					this.DJRoomDataList.Remove(item);
				}
			}
		}

		public DJRoomData GetNextDJRoomData(int index)
		{
			DJRoomData result = null;
			lock (this.mutex)
			{
				if (index < this.DJRoomDataList.Count)
				{
					result = this.DJRoomDataList[index];
				}
			}
			return result;
		}

		public DJRoomRolesData FindRoomRolesData(int roomID)
		{
			DJRoomRolesData result = null;
			lock (this.mutex)
			{
				this.DJRoomRolesDict.TryGetValue(roomID, out result);
			}
			return result;
		}

		public void AddRoomRolesData(DJRoomRolesData djRoomRolesData)
		{
			lock (this.mutex)
			{
				this.DJRoomRolesDict[djRoomRolesData.RoomID] = djRoomRolesData;
			}
		}

		public void RemoveRoomRolesData(int roomID)
		{
			lock (this.mutex)
			{
				if (this.DJRoomRolesDict.ContainsKey(roomID))
				{
					this.DJRoomRolesDict.Remove(roomID);
				}
			}
		}

		public void SetRoomRolesDataRoleState(int roomID, int roleID, int state)
		{
			DJRoomRolesData djroomRolesData = this.FindRoomRolesData(roomID);
			if (null != djroomRolesData)
			{
				lock (this.mutex)
				{
					int num = 0;
					djroomRolesData.RoleStates.TryGetValue(roleID, out num);
					if (state > num)
					{
						djroomRolesData.RoleStates[roleID] = state;
					}
				}
			}
		}

		public void ProcessFighting()
		{
			int num = 0;
			DJRoomData nextDJRoomData = this.GetNextDJRoomData(num);
			while (null != nextDJRoomData)
			{
				this.ProcessRoomFighting(nextDJRoomData);
				num++;
				nextDJRoomData = this.GetNextDJRoomData(num);
			}
		}

		private bool CanGameOver(DJRoomRolesData djRoomRolesData)
		{
			bool flag = true;
			for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
			{
				int num = 0;
				djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out num);
				if (num == 1)
				{
					flag = false;
					break;
				}
			}
			bool flag2 = true;
			for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
			{
				int num = 0;
				djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out num);
				if (num == 1)
				{
					flag2 = false;
					break;
				}
			}
			return flag || flag2;
		}

		private int GetLoseTeam(DJRoomRolesData djRoomRolesData)
		{
			bool flag = true;
			for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
			{
				int num = 0;
				djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team1[i].RoleID, out num);
				if (num == 1)
				{
					flag = false;
					break;
				}
			}
			bool flag2 = true;
			for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
			{
				int num = 0;
				djRoomRolesData.RoleStates.TryGetValue(djRoomRolesData.Team2[i].RoleID, out num);
				if (num == 1)
				{
					flag2 = false;
					break;
				}
			}
			int result;
			if (flag)
			{
				result = 1;
			}
			else if (flag2)
			{
				result = 2;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private void ProcessRoomFighting(DJRoomData djRoomData)
		{
			bool flag = false;
			DJRoomData obj;
			try
			{
				obj = djRoomData;
				Monitor.Enter(djRoomData, ref flag);
				if (djRoomData.PKState <= 0)
				{
					return;
				}
			}
			finally
			{
				if (flag)
				{
					Monitor.Exit(obj);
				}
			}
			int num = 0;
			bool flag2 = false;
			try
			{
				obj = djRoomData;
				Monitor.Enter(djRoomData, ref flag2);
				num = djRoomData.DJFightState;
			}
			finally
			{
				if (flag2)
				{
					Monitor.Exit(obj);
				}
			}
			long num2 = 0L;
			bool flag3 = false;
			try
			{
				obj = djRoomData;
				Monitor.Enter(djRoomData, ref flag3);
				num2 = djRoomData.StartFightTicks;
			}
			finally
			{
				if (flag3)
				{
					Monitor.Exit(obj);
				}
			}
			long num3 = TimeUtil.NOW();
			if (num == 0)
			{
				GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 1, num3.ToString(), null);
				bool flag4 = false;
				try
				{
					obj = djRoomData;
					Monitor.Enter(djRoomData, ref flag4);
					djRoomData.DJFightState = 1;
					djRoomData.StartFightTicks = num3;
				}
				finally
				{
					if (flag4)
					{
						Monitor.Exit(obj);
					}
				}
			}
			else if (num == 1)
			{
				if (num3 >= num2 + 30000L)
				{
					GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 2, num3.ToString(), null);
					bool flag5 = false;
					try
					{
						obj = djRoomData;
						Monitor.Enter(djRoomData, ref flag5);
						djRoomData.PKState = 2;
						djRoomData.DJFightState = 2;
						djRoomData.StartFightTicks = num3;
					}
					finally
					{
						if (flag5)
						{
							Monitor.Exit(obj);
						}
					}
				}
			}
			else if (num == 2)
			{
				bool flag6 = false;
				DJRoomRolesData djroomRolesData = this.FindRoomRolesData(djRoomData.RoomID);
				if (null != djroomRolesData)
				{
					flag6 = this.CanGameOver(djroomRolesData);
				}
				if (flag6 || num3 >= num2 + 90000L)
				{
					GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 3, num3.ToString(), null);
					bool flag7 = false;
					try
					{
						obj = djRoomData;
						Monitor.Enter(djRoomData, ref flag7);
						djRoomData.PKState = 3;
						djRoomData.DJFightState = 3;
						djRoomData.StartFightTicks = num3;
					}
					finally
					{
						if (flag7)
						{
							Monitor.Exit(obj);
						}
					}
					GameManager.ClientMgr.NotifyDianJiangData(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
				}
			}
			else if (num == 3)
			{
				this.ProcessDJFightAwards(djRoomData);
				GameManager.ClientMgr.NotifyDianJiangFightCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData, 4, num3.ToString(), null);
				bool flag8 = false;
				try
				{
					obj = djRoomData;
					Monitor.Enter(djRoomData, ref flag8);
					djRoomData.DJFightState = 4;
					djRoomData.StartFightTicks = num3;
				}
				finally
				{
					if (flag8)
					{
						Monitor.Exit(obj);
					}
				}
			}
			else if (num == 4)
			{
				if (num3 >= num2 + 60000L)
				{
					GameManager.ClientMgr.NotifyDJFightRoomLeaveMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
					GameManager.ClientMgr.RemoveDianJiangRoom(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djRoomData);
				}
			}
		}

		private int GetTeamAvgDJPoint(List<DJRoomRoleData> team)
		{
			int result;
			if (team.Count <= 0)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				for (int i = 0; i < team.Count; i++)
				{
					num += team[i].DJPoint;
				}
				result = num / team.Count;
			}
			return result;
		}

		private int GetDJPointClass(int djPoint)
		{
			int result;
			if (djPoint <= 100)
			{
				result = 0;
			}
			else if (djPoint <= 200)
			{
				result = 1;
			}
			else if (djPoint <= 300)
			{
				result = 2;
			}
			else
			{
				result = 3;
			}
			return result;
		}

		private int GetRetPoint(int pointClass, bool isWinner)
		{
			int result;
			if (0 == pointClass)
			{
				if (isWinner)
				{
					result = 10;
				}
				else
				{
					result = -4;
				}
			}
			else if (1 == pointClass)
			{
				if (isWinner)
				{
					result = 9;
				}
				else
				{
					result = -5;
				}
			}
			else if (2 == pointClass)
			{
				if (isWinner)
				{
					result = 8;
				}
				else
				{
					result = -6;
				}
			}
			else if (isWinner)
			{
				result = 7;
			}
			else
			{
				result = -7;
			}
			return result;
		}

		private int GetTeamRolePoint(DJRoomRoleData djRoomRoleData, int otherTeamAvgDJPoint, bool isWinner)
		{
			int djpointClass = this.GetDJPointClass(djRoomRoleData.DJPoint);
			int djpointClass2 = this.GetDJPointClass(otherTeamAvgDJPoint);
			int num = Math.Abs(djpointClass - djpointClass2);
			int num2 = this.GetRetPoint(djpointClass, isWinner);
			if (0 != num)
			{
				if (1 == num)
				{
					if (djpointClass > djpointClass2)
					{
						if (!isWinner)
						{
							num2 -= 10;
						}
					}
					else if (isWinner)
					{
						num2 += 10;
					}
				}
				else if (2 == num)
				{
					if (djpointClass > djpointClass2)
					{
						if (!isWinner)
						{
							num2 -= 15;
						}
					}
					else if (isWinner)
					{
						num2 += 15;
					}
				}
				else if (3 == num)
				{
					if (djpointClass > djpointClass2)
					{
						if (!isWinner)
						{
							num2 -= 20;
						}
					}
					else if (isWinner)
					{
						num2 += 20;
					}
				}
			}
			return num2;
		}

		private void ProcessDJFightAwards(DJRoomData djRoomData)
		{
			DJRoomRolesData djroomRolesData = this.FindRoomRolesData(djRoomData.RoomID);
			if (null != djroomRolesData)
			{
				DJRoomRolesPoint djroomRolesPoint = new DJRoomRolesPoint
				{
					RoomID = djRoomData.RoomID,
					RoomName = djRoomData.RoomName,
					RolePoints = new List<DJRoomRolePoint>()
				};
				lock (djroomRolesData)
				{
					int loseTeam = this.GetLoseTeam(djroomRolesData);
					int teamAvgDJPoint = this.GetTeamAvgDJPoint(djroomRolesData.Team1);
					int teamAvgDJPoint2 = this.GetTeamAvgDJPoint(djroomRolesData.Team2);
					for (int i = 0; i < djroomRolesData.Team1.Count; i++)
					{
						djroomRolesPoint.RolePoints.Add(new DJRoomRolePoint
						{
							RoleID = djroomRolesData.Team1[i].RoleID,
							RoleName = djroomRolesData.Team1[i].RoleName,
							FightPoint = ((loseTeam > 0) ? this.GetTeamRolePoint(djroomRolesData.Team1[i], teamAvgDJPoint2, loseTeam != 1) : 0)
						});
					}
					for (int i = 0; i < djroomRolesData.Team2.Count; i++)
					{
						djroomRolesPoint.RolePoints.Add(new DJRoomRolePoint
						{
							RoleID = djroomRolesData.Team2[i].RoleID,
							RoleName = djroomRolesData.Team2[i].RoleName,
							FightPoint = ((loseTeam > 0) ? this.GetTeamRolePoint(djroomRolesData.Team1[i], teamAvgDJPoint, loseTeam != 2) : 0)
						});
					}
				}
				for (int i = 0; i < djroomRolesPoint.RolePoints.Count; i++)
				{
					if (djroomRolesPoint.RolePoints[i].FightPoint != 0)
					{
						GameManager.DBCmdMgr.AddDBCmd(10023, string.Format("{0}:{1}", djroomRolesPoint.RolePoints[i].RoleID, djroomRolesPoint.RolePoints[i].FightPoint), null, 0);
					}
				}
				GameManager.ClientMgr.NotifyDianJiangRoomRolesPoint(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, djroomRolesPoint);
			}
		}

		private object mutex = new object();

		private int BaseRoomID = 1;

		private Dictionary<int, DJRoomData> DJRoomDict = new Dictionary<int, DJRoomData>(100);

		private List<DJRoomData> DJRoomDataList = new List<DJRoomData>(100);

		private Dictionary<int, DJRoomRolesData> DJRoomRolesDict = new Dictionary<int, DJRoomRolesData>(100);
	}
}
