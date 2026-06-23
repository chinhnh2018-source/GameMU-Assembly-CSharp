using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using GameServer.Core.Executor;
using KF.Contract.Data;
using Server.Tools;

namespace KF.Remoting
{
	public class KFCopyTeamManager
	{
		public void SetService(KuaFuCopyService service)
		{
			this._KFCopyService = service;
		}

		public KFCopyTeamCreateRsp CreateTeam(KFCopyTeamCreateReq req)
		{
			KFCopyTeamCreateRsp kfcopyTeamCreateRsp = new KFCopyTeamCreateRsp();
			try
			{
				lock (this.Mutex)
				{
					this.ForceLeaveRoom(req.Member.RoleID);
					if (!ClientAgentManager.Instance().IsAnyKfAgentAlive())
					{
						kfcopyTeamCreateRsp.ErrorCode = -18;
						return kfcopyTeamCreateRsp;
					}
					KFTeamCountControl teamControl = this._KFCopyService.dbMgr.TeamControl;
					if (teamControl == null)
					{
						LogManager.WriteLog(2, string.Format("跨服队伍创建失败,  丢失副本上线控制的配置文件 KFTeamCountControl", new object[0]), null, true);
						kfcopyTeamCreateRsp.ErrorCode = -13;
						return kfcopyTeamCreateRsp;
					}
					HashSet<long> hashSet = null;
					if (!this.CopyId2Teams.TryGetValue(req.CopyId, out hashSet))
					{
						hashSet = new HashSet<long>();
						this.CopyId2Teams[req.CopyId] = hashSet;
					}
					CopyTeamData copyTeamData = new CopyTeamData();
					copyTeamData.TeamID = req.TeamId;
					copyTeamData.LeaderRoleID = req.Member.RoleID;
					copyTeamData.FuBenId = req.CopyId;
					copyTeamData.MinZhanLi = req.MinCombat;
					copyTeamData.AutoStart = (req.AutoStart > 0);
					copyTeamData.AutoKick = req.AutoKick;
					copyTeamData.TeamRoles.Add(req.Member);
					copyTeamData.TeamRoles[0].IsReady = true;
					copyTeamData.TeamName = copyTeamData.TeamRoles[0].RoleName;
					copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
					this.CopyTeamDict.Add(copyTeamData.TeamID, copyTeamData);
					hashSet.Add(copyTeamData.TeamID);
					this.TimeLimitCopy.Add(copyTeamData.TeamID, TimeUtil.NOW() + (long)(teamControl.TeamMaxWaitMinutes * 60 * 1000));
					this.RoleId2JoinedTeam[req.Member.RoleID] = copyTeamData.TeamID;
					CopyTeamCreateData copyTeamCreateData = new CopyTeamCreateData();
					copyTeamCreateData.Member = req.Member;
					copyTeamCreateData.MinCombat = req.MinCombat;
					copyTeamCreateData.CopyId = req.CopyId;
					copyTeamCreateData.TeamId = copyTeamData.TeamID;
					copyTeamCreateData.AutoStart = req.AutoStart;
					copyTeamCreateData.AutoKick = req.AutoKick;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10000,
						Args = new object[]
						{
							req.Member.ServerId,
							copyTeamCreateData
						}
					});
					kfcopyTeamCreateRsp.ErrorCode = 1;
					kfcopyTeamCreateRsp.Data = copyTeamCreateData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("跨服队伍创建异常, serverid={0}, role={1}, copyid={2}", req.Member.ServerId, req.Member.RoleID, req.CopyId), ex, true);
				kfcopyTeamCreateRsp.ErrorCode = -14;
			}
			return kfcopyTeamCreateRsp;
		}

		public KFCopyTeamJoinRsp JoinTeam(KFCopyTeamJoinReq req)
		{
			KFCopyTeamJoinRsp kfcopyTeamJoinRsp = new KFCopyTeamJoinRsp();
			try
			{
				lock (this.Mutex)
				{
					this.ForceLeaveRoom(req.Member.RoleID);
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out copyTeamData))
					{
						kfcopyTeamJoinRsp.ErrorCode = -2;
						return kfcopyTeamJoinRsp;
					}
					if (copyTeamData.StartTime > 0L)
					{
						kfcopyTeamJoinRsp.ErrorCode = -15;
						return kfcopyTeamJoinRsp;
					}
					if (copyTeamData.MemberCount >= ConstData.CopyRoleMax(req.CopyId))
					{
						kfcopyTeamJoinRsp.ErrorCode = -5;
						return kfcopyTeamJoinRsp;
					}
					req.Member.IsReady = false;
					copyTeamData.TeamRoles.Add(req.Member);
					copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
					if (!req.Member.IsReady)
					{
						req.Member.NoReadyTicks = TimeUtil.NOW();
					}
					this.RoleId2JoinedTeam[req.Member.RoleID] = copyTeamData.TeamID;
					CopyTeamJoinData copyTeamJoinData = new CopyTeamJoinData();
					copyTeamJoinData.Member = req.Member;
					copyTeamJoinData.TeamId = req.TeamId;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10001,
						Args = new object[]
						{
							req.Member.ServerId,
							copyTeamJoinData
						}
					});
					kfcopyTeamJoinRsp.ErrorCode = 1;
					kfcopyTeamJoinRsp.Data = copyTeamJoinData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("加入跨服副本队伍异常, serverid={0}, role={1}, teamid={2}", req.Member.ServerId, req.Member.RoleID, req.TeamId), ex, true);
				kfcopyTeamJoinRsp.ErrorCode = -14;
			}
			return kfcopyTeamJoinRsp;
		}

		public KFCopyTeamKickoutRsp KickoutTeam(KFCopyTeamKickoutReq req)
		{
			KFCopyTeamKickoutRsp kfcopyTeamKickoutRsp = new KFCopyTeamKickoutRsp();
			try
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.Mutex, ref flag);
					CopyTeamData td = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
					{
						kfcopyTeamKickoutRsp.ErrorCode = -2;
						return kfcopyTeamKickoutRsp;
					}
					if (td.StartTime > 0L)
					{
						kfcopyTeamKickoutRsp.ErrorCode = -15;
						return kfcopyTeamKickoutRsp;
					}
					if (td.LeaderRoleID != req.FromRoleId)
					{
						kfcopyTeamKickoutRsp.ErrorCode = -4;
						return kfcopyTeamKickoutRsp;
					}
					CopyTeamMemberData copyTeamMemberData = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
					if (copyTeamMemberData == null || copyTeamMemberData.RoleID != req.FromRoleId)
					{
						kfcopyTeamKickoutRsp.ErrorCode = -4;
						return kfcopyTeamKickoutRsp;
					}
					CopyTeamMemberData copyTeamMemberData2 = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.ToRoleId);
					if (copyTeamMemberData2 == null)
					{
						kfcopyTeamKickoutRsp.ErrorCode = -16;
						return kfcopyTeamKickoutRsp;
					}
					td.TeamRoles.Remove(copyTeamMemberData2);
					td.MemberCount = td.TeamRoles.Count;
					this.RoleId2JoinedTeam.Remove(req.ToRoleId);
					CopyTeamKickoutData copyTeamKickoutData = new CopyTeamKickoutData();
					copyTeamKickoutData.FromRoleId = req.FromRoleId;
					copyTeamKickoutData.ToRoleId = req.ToRoleId;
					copyTeamKickoutData.TeamId = req.TeamId;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10002,
						Args = new object[]
						{
							copyTeamMemberData.ServerId,
							copyTeamKickoutData
						}
					});
					kfcopyTeamKickoutRsp.ErrorCode = 1;
					kfcopyTeamKickoutRsp.Data = copyTeamKickoutData;
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("踢出跨服副本队伍异常, role={0}, teamid={1}", req.FromRoleId, req.TeamId), ex, true);
				kfcopyTeamKickoutRsp.ErrorCode = -14;
			}
			return kfcopyTeamKickoutRsp;
		}

		public KFCopyTeamLeaveRsp LeaveTeam(KFCopyTeamLeaveReq req)
		{
			KFCopyTeamLeaveRsp kfcopyTeamLeaveRsp = new KFCopyTeamLeaveRsp();
			try
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out copyTeamData))
					{
						kfcopyTeamLeaveRsp.ErrorCode = -2;
						return kfcopyTeamLeaveRsp;
					}
					if (copyTeamData.StartTime > 0L)
					{
					}
					CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
					if (copyTeamMemberData == null)
					{
						kfcopyTeamLeaveRsp.ErrorCode = -16;
						return kfcopyTeamLeaveRsp;
					}
					this.RoleId2JoinedTeam.Remove(copyTeamMemberData.RoleID);
					copyTeamData.TeamRoles.Remove(copyTeamMemberData);
					copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
					if (copyTeamData.MemberCount <= 0)
					{
						this.RemoveTeam(copyTeamData.TeamID);
					}
					else if (copyTeamData.LeaderRoleID == copyTeamMemberData.RoleID)
					{
						copyTeamData.LeaderRoleID = copyTeamData.TeamRoles[0].RoleID;
						copyTeamData.TeamRoles[0].IsReady = true;
						copyTeamData.TeamName = copyTeamData.TeamRoles[0].RoleName;
					}
					CopyTeamLeaveData copyTeamLeaveData = new CopyTeamLeaveData();
					copyTeamLeaveData.TeamId = req.TeamId;
					copyTeamLeaveData.RoleId = req.RoleId;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10003,
						Args = new object[]
						{
							req.ReqServerId,
							copyTeamLeaveData
						}
					});
					kfcopyTeamLeaveRsp.ErrorCode = 1;
					kfcopyTeamLeaveRsp.Data = copyTeamLeaveData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("离开跨服副本队伍异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
				kfcopyTeamLeaveRsp.ErrorCode = -14;
			}
			return kfcopyTeamLeaveRsp;
		}

		public KFCopyTeamSetReadyRsp TeamSetReady(KFCopyTeamSetReadyReq req)
		{
			KFCopyTeamSetReadyRsp kfcopyTeamSetReadyRsp = new KFCopyTeamSetReadyRsp();
			try
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out copyTeamData))
					{
						kfcopyTeamSetReadyRsp.ErrorCode = -2;
						return kfcopyTeamSetReadyRsp;
					}
					if (req.Ready <= 0)
					{
						if (req.RoleId == copyTeamData.LeaderRoleID)
						{
							kfcopyTeamSetReadyRsp.ErrorCode = -20;
							return kfcopyTeamSetReadyRsp;
						}
					}
					CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
					if (copyTeamMemberData == null)
					{
						kfcopyTeamSetReadyRsp.ErrorCode = -16;
						return kfcopyTeamSetReadyRsp;
					}
					if (copyTeamData.StartTime > 0L)
					{
						kfcopyTeamSetReadyRsp.ErrorCode = -15;
						return kfcopyTeamSetReadyRsp;
					}
					copyTeamMemberData.IsReady = (req.Ready > 0);
					if (!copyTeamMemberData.IsReady)
					{
						copyTeamMemberData.NoReadyTicks = TimeUtil.NOW();
					}
					CopyTeamReadyData copyTeamReadyData = new CopyTeamReadyData();
					copyTeamReadyData.RoleId = req.RoleId;
					copyTeamReadyData.TeamId = req.TeamId;
					copyTeamReadyData.Ready = req.Ready;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10004,
						Args = new object[]
						{
							copyTeamMemberData.ServerId,
							copyTeamReadyData
						}
					});
					kfcopyTeamSetReadyRsp.ErrorCode = 1;
					kfcopyTeamSetReadyRsp.Data = copyTeamReadyData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("更新跨服副本队伍准备状态异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
				kfcopyTeamSetReadyRsp.ErrorCode = -14;
			}
			return kfcopyTeamSetReadyRsp;
		}

		public KFCopyTeamSetFlagRsp TeamSetFlag(KFCopyTeamSetFlagReq req)
		{
			KFCopyTeamSetFlagRsp kfcopyTeamSetFlagRsp = new KFCopyTeamSetFlagRsp();
			try
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out copyTeamData))
					{
						kfcopyTeamSetFlagRsp.ErrorCode = -2;
						return kfcopyTeamSetFlagRsp;
					}
					if (req.RoleId != copyTeamData.LeaderRoleID)
					{
						kfcopyTeamSetFlagRsp.ErrorCode = -4;
						return kfcopyTeamSetFlagRsp;
					}
					CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == req.RoleId);
					if (copyTeamMemberData == null)
					{
						kfcopyTeamSetFlagRsp.ErrorCode = -16;
						return kfcopyTeamSetFlagRsp;
					}
					if (copyTeamData.StartTime > 0L)
					{
						kfcopyTeamSetFlagRsp.ErrorCode = 1;
						return kfcopyTeamSetFlagRsp;
					}
					if (req.AutoStart >= 0)
					{
						copyTeamData.AutoStart = (req.AutoStart > 0);
					}
					if (req.AutoKick >= 0)
					{
						copyTeamData.AutoKick = req.AutoKick;
					}
					CopyTeamFlagData copyTeamFlagData = new CopyTeamFlagData();
					copyTeamFlagData.RoleId = req.RoleId;
					copyTeamFlagData.TeamId = req.TeamId;
					copyTeamFlagData.AutoStart = (copyTeamData.AutoStart ? 1 : 0);
					copyTeamFlagData.AutoKick = copyTeamData.AutoKick;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10007,
						Args = new object[]
						{
							copyTeamMemberData.ServerId,
							copyTeamFlagData
						}
					});
					kfcopyTeamSetFlagRsp.ErrorCode = 1;
					kfcopyTeamSetFlagRsp.Data = copyTeamFlagData;
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("更新跨服副本队伍准备状态异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
				kfcopyTeamSetFlagRsp.ErrorCode = -14;
			}
			return kfcopyTeamSetFlagRsp;
		}

		public KFCopyTeamStartRsp StartGame(KFCopyTeamStartReq req)
		{
			KFCopyTeamStartRsp kfcopyTeamStartRsp = new KFCopyTeamStartRsp();
			try
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.Mutex, ref flag);
					CopyTeamData td = null;
					if (!this.CopyTeamDict.TryGetValue(req.TeamId, out td))
					{
						kfcopyTeamStartRsp.ErrorCode = -2;
						return kfcopyTeamStartRsp;
					}
					if (td.StartTime > 0L)
					{
						kfcopyTeamStartRsp.ErrorCode = -15;
						return kfcopyTeamStartRsp;
					}
					if (td.LeaderRoleID != req.RoleId)
					{
						kfcopyTeamStartRsp.ErrorCode = -4;
						return kfcopyTeamStartRsp;
					}
					CopyTeamMemberData copyTeamMemberData = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
					if (copyTeamMemberData == null || copyTeamMemberData.RoleID != req.RoleId)
					{
						kfcopyTeamStartRsp.ErrorCode = -4;
						return kfcopyTeamStartRsp;
					}
					if (td.TeamRoles.Exists((CopyTeamMemberData _role) => !_role.IsReady))
					{
						kfcopyTeamStartRsp.ErrorCode = -17;
						return kfcopyTeamStartRsp;
					}
					int num;
					if (!ClientAgentManager.Instance().AssginKfFuben(8, td.TeamID, td.TeamRoles.Count, out num))
					{
						kfcopyTeamStartRsp.ErrorCode = -18;
						return kfcopyTeamStartRsp;
					}
					td.StartTime = TimeUtil.NOW();
					td.KFServerId = num;
					td.FuBenSeqID = 0;
					CopyTeamStartData copyTeamStartData = new CopyTeamStartData();
					copyTeamStartData.TeamId = req.TeamId;
					copyTeamStartData.StartMs = td.StartTime;
					copyTeamStartData.ToServerId = num;
					copyTeamStartData.FuBenSeqId = td.FuBenSeqID;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10005,
						Args = new object[]
						{
							copyTeamMemberData.ServerId,
							copyTeamStartData
						}
					});
					this.TimeLimitCopy[td.TeamID] = td.StartTime + (long)req.LastMs + 180000L;
					kfcopyTeamStartRsp.ErrorCode = 1;
					kfcopyTeamStartRsp.Data = copyTeamStartData;
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("开始跨服副本队伍异常, role={0}, teamid={1}", req.RoleId, req.TeamId), ex, true);
				kfcopyTeamStartRsp.ErrorCode = -14;
			}
			return kfcopyTeamStartRsp;
		}

		private void ForceLeaveRoom(int roleId)
		{
			lock (this.Mutex)
			{
				long teamId = -1L;
				if (this.RoleId2JoinedTeam.TryGetValue(roleId, out teamId))
				{
					this.LeaveTeam(new KFCopyTeamLeaveReq
					{
						ReqServerId = int.MaxValue,
						RoleId = roleId,
						TeamId = teamId
					});
					this.RoleId2JoinedTeam.Remove(roleId);
				}
			}
		}

		private void AddAsyncEvent(AsyncDataItem evItem)
		{
			if (evItem != null)
			{
				lock (this.RoomEventQ)
				{
					this.RoomEventQ.Enqueue(evItem);
				}
			}
		}

		public AsyncDataItem[] PopAsyncEvent()
		{
			AsyncDataItem[] result = null;
			lock (this.RoomEventQ)
			{
				result = this.RoomEventQ.ToArray();
				this.RoomEventQ.Clear();
			}
			return result;
		}

		public void Update()
		{
			long num = TimeUtil.NOW();
			lock (this.Mutex)
			{
				if (num >= this.TimeLimitCopyLastCheckMs + 30000L)
				{
					this.TimeLimitCopyLastCheckMs = num;
					List<KeyValuePair<long, long>> list = this.TimeLimitCopy.ToList<KeyValuePair<long, long>>();
					list.Sort(delegate(KeyValuePair<long, long> _left, KeyValuePair<long, long> _right)
					{
						int result;
						if (_left.Value - _right.Value < 0L)
						{
							result = -1;
						}
						else if (_left.Value - _right.Value > 0L)
						{
							result = 1;
						}
						else
						{
							result = 0;
						}
						return result;
					});
					for (int i = 0; i < list.Count; i++)
					{
						long key = list[i].Key;
						long value = list[i].Value;
						if (value > num)
						{
							break;
						}
						this.RemoveTeam(key);
						this.TimeLimitCopy.Remove(key);
					}
				}
				if (num >= this.TimeLimitMemberNoReadyMs + 1000L)
				{
					foreach (CopyTeamData copyTeamData in this.CopyTeamDict.Values)
					{
						if (copyTeamData.StartTime <= 0L)
						{
							if (copyTeamData.AutoKick > 0)
							{
								List<int> list2 = new List<int>();
								foreach (CopyTeamMemberData copyTeamMemberData in copyTeamData.TeamRoles)
								{
									if (!copyTeamMemberData.IsReady && num > copyTeamMemberData.NoReadyTicks + 30000L)
									{
										list2.Add(copyTeamMemberData.RoleID);
									}
								}
								foreach (int roleid in list2)
								{
									this.RemoveMember(copyTeamData.TeamID, roleid);
								}
							}
						}
					}
				}
			}
		}

		public void RemoveTeam(long teamId)
		{
			lock (this.Mutex)
			{
				CopyTeamData copyTeamData = null;
				if (this.CopyTeamDict.TryGetValue(teamId, out copyTeamData))
				{
					this.CopyTeamDict.Remove(teamId);
					HashSet<long> hashSet = null;
					if (this.CopyId2Teams.TryGetValue(copyTeamData.FuBenId, out hashSet))
					{
						hashSet.Remove(teamId);
					}
					this.TimeLimitCopy.Remove(copyTeamData.TeamID);
					if (copyTeamData.KFServerId > 0)
					{
						this._KFCopyService.RemoveGameTeam(copyTeamData.KFServerId, copyTeamData.TeamID);
					}
					foreach (CopyTeamMemberData copyTeamMemberData in copyTeamData.TeamRoles)
					{
						this.RoleId2JoinedTeam.Remove(copyTeamMemberData.RoleID);
					}
					CopyTeamDestroyData copyTeamDestroyData = new CopyTeamDestroyData();
					copyTeamDestroyData.TeamId = teamId;
					this.AddAsyncEvent(new AsyncDataItem
					{
						EventType = 10006,
						Args = new object[]
						{
							copyTeamDestroyData
						}
					});
				}
			}
		}

		public CopyTeamErrorCodes RemoveMember(long teamId, int roleid)
		{
			CopyTeamErrorCodes result;
			try
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.Mutex, ref flag);
					CopyTeamData td = null;
					if (!this.CopyTeamDict.TryGetValue(teamId, out td))
					{
						result = -2;
					}
					else if (td.StartTime > 0L)
					{
						result = -15;
					}
					else
					{
						CopyTeamMemberData copyTeamMemberData = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
						if (copyTeamMemberData == null)
						{
							result = -4;
						}
						else
						{
							CopyTeamMemberData copyTeamMemberData2 = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == roleid);
							if (copyTeamMemberData2 == null)
							{
								result = -16;
							}
							else
							{
								td.TeamRoles.Remove(copyTeamMemberData2);
								td.MemberCount = td.TeamRoles.Count;
								this.RoleId2JoinedTeam.Remove(roleid);
								CopyTeamKickoutData copyTeamKickoutData = new CopyTeamKickoutData();
								copyTeamKickoutData.FromRoleId = td.LeaderRoleID;
								copyTeamKickoutData.ToRoleId = roleid;
								copyTeamKickoutData.TeamId = teamId;
								this.AddAsyncEvent(new AsyncDataItem
								{
									EventType = 10002,
									Args = new object[]
									{
										0,
										copyTeamKickoutData
									}
								});
								result = 1;
							}
						}
					}
				}
				finally
				{
					if (flag)
					{
						object mutex;
						Monitor.Exit(mutex);
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("系统踢出跨服副本队伍异常, role={0}, teamid={1}", roleid, teamId), ex, true);
				result = -14;
			}
			return result;
		}

		public CopyTeamData GetTeamData(long teamid)
		{
			CopyTeamData result;
			lock (this.Mutex)
			{
				CopyTeamData copyTeamData = null;
				if (!this.CopyTeamDict.TryGetValue(teamid, out copyTeamData))
				{
					result = null;
				}
				else
				{
					result = copyTeamData;
				}
			}
			return result;
		}

		public KFCopyTeamAnalysis BuildAnalysisData()
		{
			KFCopyTeamAnalysis kfcopyTeamAnalysis = new KFCopyTeamAnalysis();
			lock (this.Mutex)
			{
				foreach (KeyValuePair<long, CopyTeamData> keyValuePair in this.CopyTeamDict)
				{
					long key = keyValuePair.Key;
					CopyTeamData value = keyValuePair.Value;
					KFCopyTeamAnalysis.Item item = null;
					if (!kfcopyTeamAnalysis.AnalysisDict.TryGetValue(value.FuBenId, out item))
					{
						item = new KFCopyTeamAnalysis.Item();
						kfcopyTeamAnalysis.AnalysisDict[value.FuBenId] = item;
					}
					item.TotalCopyCount++;
					item.TotalRoleCount += value.TeamRoles.Count;
					if (value.StartTime > 0L)
					{
						item.StartCopyCount++;
						item.StartRoleCount += value.TeamRoles.Count;
					}
					else
					{
						item.UnStartCopyCount++;
						item.UnStartRoleCount += value.TeamRoles.Count;
					}
				}
			}
			return kfcopyTeamAnalysis;
		}

		private object Mutex = new object();

		private Dictionary<int, long> RoleId2JoinedTeam = new Dictionary<int, long>();

		private Dictionary<long, CopyTeamData> CopyTeamDict = new Dictionary<long, CopyTeamData>();

		private Dictionary<int, HashSet<long>> CopyId2Teams = new Dictionary<int, HashSet<long>>();

		private long TimeLimitCopyLastCheckMs = 0L;

		private Dictionary<long, long> TimeLimitCopy = new Dictionary<long, long>();

		private Queue<AsyncDataItem> RoomEventQ = new Queue<AsyncDataItem>();

		private KuaFuCopyService _KFCopyService = null;

		public long TimeLimitMemberNoReadyMs = 0L;
	}
}
