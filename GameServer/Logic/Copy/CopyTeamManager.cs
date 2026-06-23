using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Server;
using KF.Client;
using KF.Contract.Data;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic.Copy
{
	public class CopyTeamManager : SingletonTemplate<CopyTeamManager>, IManager, ICmdProcessorEx, ICmdProcessor, IEventListener, IEventListenerEx, ICopySceneManager
	{
		public bool IsKuaFuCopy(int copyId)
		{
			SystemXmlItem systemXmlItem = null;
			bool result;
			if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out systemXmlItem))
			{
				result = false;
			}
			else
			{
				int intValue = systemXmlItem.GetIntValue("MapCode", -1);
				SceneUIClasses mapSceneType = Global.GetMapSceneType(intValue);
				SceneUIClasses sceneUIClasses = mapSceneType;
				if (sceneUIClasses <= 29)
				{
					if (sceneUIClasses != 10)
					{
						switch (sceneUIClasses)
						{
						case 28:
						case 29:
							break;
						default:
							goto IL_72;
						}
					}
				}
				else if (sceneUIClasses != 34 && sceneUIClasses != 49 && sceneUIClasses != 1003)
				{
					goto IL_72;
				}
				return true;
				IL_72:
				result = false;
			}
			return result;
		}

		public bool HandleKuaFuLogin(KuaFuServerLoginData data)
		{
			bool result;
			if (data == null)
			{
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(data.GameId, out copyTeamData) || copyTeamData.StartTime <= 0L)
					{
						copyTeamData = KFCopyRpcClient.getInstance().GetTeamData(data.GameId);
						if (copyTeamData == null)
						{
							return false;
						}
						this.CopyTeamDict[copyTeamData.TeamID] = copyTeamData;
						HashSet<long> hashSet = null;
						if (this.FuBenId2Teams.TryGetValue(copyTeamData.FuBenId, out hashSet) && !hashSet.Contains(copyTeamData.TeamID))
						{
							hashSet.Add(copyTeamData.TeamID);
						}
					}
					if (copyTeamData == null)
					{
						result = false;
					}
					else if (copyTeamData.KFServerId != this.ThisServerId)
					{
						result = false;
					}
					else if (copyTeamData.StartTime <= 0L)
					{
						result = false;
					}
					else if (!copyTeamData.TeamRoles.Exists((CopyTeamMemberData _role) => _role.RoleID == data.RoleId))
					{
						result = false;
					}
					else
					{
						if (copyTeamData.FuBenSeqID <= 0)
						{
							copyTeamData.FuBenSeqID = GameCoreInterface.getinstance().GetNewFuBenSeqId();
						}
						data.FuBenSeqId = copyTeamData.FuBenSeqID;
						this.FuBenSeq2TeamId[copyTeamData.FuBenSeqID] = copyTeamData.TeamID;
						result = true;
					}
				}
			}
			return result;
		}

		public bool HandleKuaFuInitGame(GameClient client)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (!this.CopyTeamDict.TryGetValue(client.ClientSocket.ClientKuaFuServerLoginData.GameId, out copyTeamData))
					{
						result = false;
					}
					else
					{
						SystemXmlItem systemXmlItem = null;
						if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyTeamData.FuBenId, out systemXmlItem))
						{
							result = false;
						}
						else
						{
							int intValue = systemXmlItem.GetIntValue("MapCode", -1);
							int posX;
							int posY;
							if (!this.GetBirthPoint(intValue, out posX, out posY))
							{
								LogManager.WriteLog(2, string.Format("rolename={0} 跨服登录副本copyid={1}, 找不到出生点", client.ClientData.RoleName, copyTeamData.FuBenId), null, true);
								result = false;
							}
							else
							{
								client.ClientData.MapCode = intValue;
								client.ClientData.PosX = posX;
								client.ClientData.PosY = posY;
								client.ClientData.FuBenSeqID = client.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId;
								this.RoleId2JoinedTeam[client.ClientData.RoleID] = copyTeamData.TeamID;
								result = true;
							}
						}
					}
				}
			}
			return result;
		}

		private bool GetBirthPoint(int mapCode, out int toPosX, out int toPosY)
		{
			toPosX = -1;
			toPosY = -1;
			GameMap gameMap = null;
			bool result;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(mapCode, out gameMap))
			{
				result = false;
			}
			else
			{
				int defaultBirthPosX = gameMap.DefaultBirthPosX;
				int defaultBirthPosY = gameMap.DefaultBirthPosY;
				int birthRadius = gameMap.BirthRadius;
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, mapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
				toPosX = (int)mapPoint.X;
				toPosY = (int)mapPoint.Y;
				result = true;
			}
			return result;
		}

		public void OnCopyRemove(int FuBenSeqId)
		{
			long num = -1L;
			lock (this.Mutex)
			{
				if (this.FuBenSeq2TeamId.TryGetValue(FuBenSeqId, out num))
				{
					this.FuBenSeq2TeamId.Remove(FuBenSeqId);
					CopyTeamData copyTeamData;
					if (this.CopyTeamDict.TryGetValue(num, out copyTeamData))
					{
						this.OnTeamDestroy(new CopyTeamDestroyData
						{
							TeamId = num
						});
						if (this.IsKuaFuCopy(copyTeamData.FuBenId))
						{
							KFCopyRpcClient.getInstance().KFCopyTeamRemove(num);
						}
					}
				}
			}
		}

		private void OnTeamCreate(CopyTeamCreateData data)
		{
			if (data != null)
			{
				bool flag = this.IsKuaFuCopy(data.CopyId);
				CopyTeamData copyTeamData = new CopyTeamData();
				copyTeamData.TeamID = data.TeamId;
				copyTeamData.LeaderRoleID = data.Member.RoleID;
				copyTeamData.RoleSex = data.Member.RoleSex;
				copyTeamData.Occupation = data.Member.Occupation;
				copyTeamData.FuBenId = data.CopyId;
				copyTeamData.MinZhanLi = data.MinCombat;
				copyTeamData.AutoStart = (data.AutoStart > 0);
				copyTeamData.TeamRoles.Add(data.Member);
				copyTeamData.TeamRoles[0].IsReady = true;
				copyTeamData.TeamName = copyTeamData.TeamRoles[0].RoleName;
				copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
				copyTeamData.AutoKick = data.AutoKick;
				lock (this.Mutex)
				{
					this.CopyTeamDict[copyTeamData.TeamID] = copyTeamData;
					HashSet<long> hashSet = null;
					if (this.FuBenId2Teams.TryGetValue(copyTeamData.FuBenId, out hashSet) && !hashSet.Contains(copyTeamData.TeamID))
					{
						hashSet.Add(copyTeamData.TeamID);
					}
					if (data.Member.ServerId == this.ThisServerId)
					{
						this.RoleId2JoinedTeam[data.Member.RoleID] = copyTeamData.TeamID;
						GameClient gameClient = GameManager.ClientMgr.FindClient(data.Member.RoleID);
						if (gameClient != null)
						{
							this.NotifyTeamCmd(gameClient, 1, 1, copyTeamData.TeamID, copyTeamData.TeamName, -1, -1, -1);
						}
					}
					this.NotifyTeamData(copyTeamData);
					this.NotifyTeamListChange(copyTeamData);
				}
			}
		}

		private void OnTeamJoin(CopyTeamJoinData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						if (copyTeamData.TeamRoles.Count < ConstData.CopyRoleMax(copyTeamData.FuBenId))
						{
							copyTeamData.TeamRoles.Add(data.Member);
							copyTeamData.MemberCount = copyTeamData.TeamRoles.Count<CopyTeamMemberData>();
							if (data.Member.ServerId == this.ThisServerId)
							{
								this.RoleId2JoinedTeam[data.Member.RoleID] = copyTeamData.TeamID;
								GameClient gameClient = GameManager.ClientMgr.FindClient(data.Member.RoleID);
								if (gameClient != null)
								{
									this.NotifyTeamCmd(gameClient, 1, 4, copyTeamData.TeamID, copyTeamData.TeamName, -1, -1, -1);
								}
							}
							this.NotifyTeamData(copyTeamData);
							this.NotifyTeamListChange(copyTeamData);
						}
					}
				}
			}
		}

		private void OnTeamKickout(CopyTeamKickoutData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.ToRoleId);
						if (copyTeamMemberData != null)
						{
							copyTeamData.TeamRoles.Remove(copyTeamMemberData);
							copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
							if (copyTeamMemberData.ServerId == this.ThisServerId)
							{
								this.RoleId2JoinedTeam.Remove(copyTeamMemberData.RoleID);
								GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
								if (gameClient != null)
								{
									this.NotifyTeamStateChanged(gameClient, -12L, copyTeamMemberData.RoleID, 0);
								}
							}
							this.NotifyTeamData(copyTeamData);
							this.NotifyTeamListChange(copyTeamData);
						}
					}
				}
			}
		}

		private void OnTeamLeave(CopyTeamLeaveData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.RoleId);
						if (copyTeamMemberData != null)
						{
							copyTeamData.TeamRoles.Remove(copyTeamMemberData);
							copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
							if (copyTeamData.MemberCount <= 0)
							{
								copyTeamData.LeaderRoleID = -1;
								this.OnTeamDestroy(new CopyTeamDestroyData
								{
									TeamId = copyTeamData.TeamID
								});
							}
							else if (copyTeamData.LeaderRoleID == copyTeamMemberData.RoleID)
							{
								copyTeamData.LeaderRoleID = copyTeamData.TeamRoles[0].RoleID;
								copyTeamData.TeamRoles[0].IsReady = true;
								copyTeamData.TeamName = copyTeamData.TeamRoles[0].RoleName;
							}
							if (copyTeamMemberData.ServerId == this.ThisServerId)
							{
								this.RoleId2JoinedTeam.Remove(copyTeamMemberData.RoleID);
								GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
								if (gameClient != null)
								{
									this.NotifyTeamStateChanged(gameClient, -11L, copyTeamMemberData.RoleID, 0);
								}
							}
							this.NotifyTeamData(copyTeamData);
							this.NotifyTeamListChange(copyTeamData);
						}
					}
				}
			}
		}

		private void OnTeamSetReady(CopyTeamReadyData data)
		{
			if (data != null)
			{
				bool flag = false;
				try
				{
					object mutex;
					Monitor.Enter(mutex = this.Mutex, ref flag);
					CopyTeamData td = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out td))
					{
						CopyTeamMemberData copyTeamMemberData = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == data.RoleId);
						if (copyTeamMemberData != null)
						{
							copyTeamMemberData.IsReady = (data.Ready > 0);
							if (!copyTeamMemberData.IsReady)
							{
								copyTeamMemberData.NoReadyTicks = TimeUtil.NOW();
							}
							if (copyTeamMemberData.ServerId == this.ThisServerId)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
								if (gameClient != null)
								{
									this.NotifyTeamStateChanged(gameClient, td.TeamID, copyTeamMemberData.RoleID, data.Ready);
								}
							}
							this.NotifyTeamData(td);
							bool flag2;
							if (copyTeamMemberData.IsReady && td.AutoStart && td.MemberCount >= ConstData.CopyRoleMax(td.FuBenId))
							{
								flag2 = !td.TeamRoles.All((CopyTeamMemberData _role) => _role.IsReady);
							}
							else
							{
								flag2 = true;
							}
							if (!flag2)
							{
								CopyTeamMemberData copyTeamMemberData2 = td.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == td.LeaderRoleID);
								if (copyTeamMemberData2 != null && copyTeamMemberData2.ServerId == this.ThisServerId)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData2.RoleID);
									if (gameClient != null)
									{
										this.NotifyTeamCmd(gameClient, 1, 14, 0L, "", -1, -1, -1);
									}
								}
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
		}

		private void OnTeamSetFlag(CopyTeamFlagData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						if ((copyTeamData.AutoStart ? 1 : 0) != data.AutoStart)
						{
							copyTeamData.AutoStart = (data.AutoStart > 0);
							this.NotifyTeamCmds(copyTeamData, 1, 16, copyTeamData.AutoStart ? 1L : 0L, "");
						}
						if (copyTeamData.AutoKick != data.AutoKick)
						{
							copyTeamData.AutoKick = data.AutoKick;
							this.NotifyTeamCmds(copyTeamData, 1, 15, (long)copyTeamData.AutoKick, "");
						}
					}
				}
			}
		}

		private void OnTeamDestroy(CopyTeamDestroyData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						this.CopyTeamDict.Remove(data.TeamId);
						this.FuBenSeq2TeamId.Remove(copyTeamData.FuBenSeqID);
						HashSet<long> hashSet = null;
						if (this.FuBenId2Teams.TryGetValue(copyTeamData.FuBenId, out hashSet))
						{
							hashSet.Remove(copyTeamData.TeamID);
						}
						foreach (CopyTeamMemberData copyTeamMemberData in copyTeamData.TeamRoles)
						{
							this.RoleId2JoinedTeam.Remove(copyTeamMemberData.RoleID);
							if (copyTeamMemberData.ServerId == this.ThisServerId)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
								if (gameClient != null)
								{
									this.NotifyTeamStateChanged(gameClient, -11L, copyTeamMemberData.RoleID, 0);
								}
							}
						}
						copyTeamData.LeaderRoleID = -1;
						this.NotifyTeamData(copyTeamData);
						copyTeamData.TeamRoles.Clear();
						copyTeamData.MemberCount = copyTeamData.TeamRoles.Count;
						this.NotifyTeamListChange(copyTeamData);
					}
				}
			}
		}

		private void OnTeamStart(CopyTeamStartData data)
		{
			if (data != null)
			{
				lock (this.Mutex)
				{
					CopyTeamData copyTeamData = null;
					if (this.CopyTeamDict.TryGetValue(data.TeamId, out copyTeamData))
					{
						copyTeamData.StartTime = data.StartMs;
						copyTeamData.KFServerId = data.ToServerId;
						copyTeamData.FuBenSeqID = data.FuBenSeqId;
						bool flag2 = this.IsKuaFuCopy(copyTeamData.FuBenId);
						string empty = string.Empty;
						int serverPort = 0;
						if (flag2)
						{
							if (!KFCopyRpcClient.getInstance().GetKuaFuGSInfo(data.ToServerId, out empty, out serverPort))
							{
								LogManager.WriteLog(2, string.Format("跨服副本CopyType={0}, RoomId={1}被分配到服务器ServerId={2}, 但是找不到该跨服活动服务器", copyTeamData.FuBenId, data.TeamId, data.ToServerId), null, true);
								return;
							}
						}
						else
						{
							this.FuBenSeq2TeamId[copyTeamData.FuBenSeqID] = copyTeamData.TeamID;
						}
						foreach (CopyTeamMemberData copyTeamMemberData in copyTeamData.TeamRoles)
						{
							if (copyTeamMemberData.ServerId == this.ThisServerId)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
								if (gameClient != null)
								{
									if (flag2)
									{
										gameClient.ClientSocket.ClientKuaFuServerLoginData.RoleId = copyTeamMemberData.RoleID;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.GameId = copyTeamData.TeamID;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.GameType = 8;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.EndTicks = 0L;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerId = this.ThisServerId;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerIp = empty;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.ServerPort = serverPort;
										gameClient.ClientSocket.ClientKuaFuServerLoginData.FuBenSeqId = data.FuBenSeqId;
									}
									GameManager.ClientMgr.NotifyTeamMemberFuBenEnterMsg(gameClient, copyTeamData.LeaderRoleID, copyTeamData.FuBenId, copyTeamData.FuBenSeqID);
								}
							}
						}
						this.NotifyTeamListChange(copyTeamData);
					}
				}
			}
		}

		private CopyTeamManager()
		{
		}

		public bool initialize()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(621, 6, SingletonTemplate<CopyTeamManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(624, 4, SingletonTemplate<CopyTeamManager>.Instance());
			TCPCmdDispatcher.getInstance().registerProcessor(620, 4, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource.getInstance().registerListener(13, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource.getInstance().registerListener(12, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource.getInstance().registerListener(14, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10006, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10007, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10008, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10009, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10010, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10011, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10012, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().registerListener(10013, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			this.ThisServerId = GameManager.ServerId;
			foreach (SystemXmlItem systemXmlItem in GameManager.systemFuBenMgr.SystemXmlItemDict.Values)
			{
				int num = systemXmlItem.GetIntValue("CopyType", -1);
				int intValue = systemXmlItem.GetIntValue("ID", -1);
				if (1 == num)
				{
					this.FuBenId2Watchers.Add(intValue, new HashSet<int>());
					this.FuBenId2Teams.Add(intValue, new HashSet<long>());
				}
				int intValue2 = systemXmlItem.GetIntValue("RecordDamage", -1);
				if (1 == intValue2)
				{
					this.RecordDamagesFuBenIDHashSet.Add(intValue);
				}
			}
			List<FuBenMapItem> allFubenMapItem = FuBenManager.GetAllFubenMapItem();
			foreach (FuBenMapItem fuBenMapItem in allFubenMapItem)
			{
				int num = Global.GetFuBenCopyType(fuBenMapItem.FuBenID);
				if (1 == num)
				{
					this.MapCode2ToFubenId.Add(fuBenMapItem.MapCode, fuBenMapItem.FuBenID);
					if (!this.FuBenId2MapCodes.ContainsKey(fuBenMapItem.FuBenID))
					{
						this.FuBenId2MapCodes.Add(fuBenMapItem.FuBenID, new List<int>());
					}
					this.FuBenId2MapCodes[fuBenMapItem.FuBenID].Add(fuBenMapItem.MapCode);
				}
			}
			SingletonTemplate<UniqueTeamId>.Instance().Init();
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
			GlobalEventSource.getInstance().removeListener(13, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource.getInstance().removeListener(12, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource.getInstance().removeListener(14, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10006, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10007, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10008, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10009, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10010, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10011, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10012, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			GlobalEventSource4Scene.getInstance().removeListener(10013, 10001, SingletonTemplate<CopyTeamManager>.Instance());
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			switch (nID)
			{
			case 620:
				return this.HandleNetCmd_GetRoomList(client, nID, bytes, cmdParams);
			case 621:
				return this.HandleNetCmd_CopyTeam(client, nID, bytes, cmdParams);
			case 624:
				return this.HandleNetCmd_RegRoomNotify(client, nID, bytes, cmdParams);
			}
			return true;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public void processEvent(EventObject eventObject)
		{
			if (eventObject.getEventType() == 13)
			{
				PlayerLeaveFuBenEventObject playerLeaveFuBenEventObject = (PlayerLeaveFuBenEventObject)eventObject;
				this.RoleLeaveFuBen(playerLeaveFuBenEventObject.getPlayer());
			}
			else if (eventObject.getEventType() == 14)
			{
				PlayerInitGameEventObject playerInitGameEventObject = (PlayerInitGameEventObject)eventObject;
				this.OnPlayerLogin(playerInitGameEventObject.getPlayer());
			}
			else if (eventObject.getEventType() == 12)
			{
				PlayerLogoutEventObject playerLogoutEventObject = (PlayerLogoutEventObject)eventObject;
				this.OnPlayerLogout(playerLogoutEventObject.getPlayer());
			}
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10006:
				this.OnTeamCreate((eventObject as KFCopyRoomCreateEvent).Data);
				break;
			case 10007:
				this.OnTeamJoin((eventObject as KFCopyRoomJoinEvent).Data);
				break;
			case 10008:
				this.OnTeamSetReady((eventObject as KFCopyRoomReadyEvent).Data);
				break;
			case 10009:
				this.OnTeamKickout((eventObject as KFCopyRoomKickoutEvent).Data);
				break;
			case 10010:
				this.OnTeamLeave((eventObject as KFCopyRoomLeaveEvent).Data);
				break;
			case 10011:
				this.OnTeamDestroy((eventObject as KFCopyTeamDestroyEvent).Data);
				break;
			case 10012:
				this.OnTeamStart((eventObject as KFCopyRoomStartEvent).Data);
				break;
			case 10013:
				this.OnTeamSetFlag((eventObject as KFCopyRoomSetFlagEvent).Data);
				break;
			}
			eventObject.Handled = true;
		}

		private bool HandleNetCmd_CopyTeam(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[1]);
			if (num == 1)
			{
				int copyId = Convert.ToInt32(cmdParams[2]);
				int minCombat = Convert.ToInt32(cmdParams[3]);
				int autoStart = Convert.ToInt32(cmdParams[4]);
				int kickNoReady = Convert.ToInt32(cmdParams[5]);
				this.HandleCreateCopyTeam(client, copyId, minCombat, autoStart, kickNoReady);
			}
			else if (num == 4)
			{
				long teamId = Convert.ToInt64(cmdParams[2]);
				this.HandleApplyCopyTeam(client, teamId);
			}
			else if (num == 8)
			{
				int otherRoleId = Convert.ToInt32(cmdParams[2]);
				this.HandleKickoutCopyTeam(client, otherRoleId);
			}
			else if (num == 9)
			{
				this.HandleQuitFromTeam(client, true);
			}
			else if (num == 12)
			{
				int ready = Convert.ToInt32(cmdParams[2]);
				this.HandleSetReady(client, ready);
			}
			else if (num == 13)
			{
				int copyId = Convert.ToInt32(cmdParams[2]);
				this.HandleQuickJoinTeam(client, copyId);
			}
			else if (num == 15)
			{
				int flag = Convert.ToInt32(cmdParams[5]);
				this.HandleModKickFlag(client, flag);
			}
			else if (num == 16)
			{
				int flag = Convert.ToInt32(cmdParams[4]);
				this.HandleModAutoStart(client, flag);
			}
			return true;
		}

		private bool HandleNetCmd_RegRoomNotify(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			int copyId = Convert.ToInt32(cmdParams[1]);
			int num = Convert.ToInt32(cmdParams[2]);
			if (num > 0)
			{
				this.RegisterCopyTeamListNotify(client, copyId);
			}
			else
			{
				this.UnRegisterCopyTeamListNotify(client);
			}
			return true;
		}

		private bool HandleNetCmd_GetRoomList(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			throw new NotImplementedException();
		}

		private void HandleCreateCopyTeam(GameClient client, int copyId, int minCombat, int autoStart, int kickNoReady)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				SystemXmlItem fubenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleCreateCopyTeam Faild copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.HasFinishedPreTask(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleCreateCopyTeam Faild !HasFinishedPreTask copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.HasPassedPreCopy(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleCreateCopyTeam Faild !HasPassedPreCopy copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.IsInCopyLevelLimit(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleCreateCopyTeam Faild !IsInCopyLevelLimit copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.IsInCopyTimesLimit(client, fubenItem))
				{
					this.NotifyTeamCmd(client, -21, 1, 0L, "", -1, -1, -1);
				}
				else
				{
					lock (this.Mutex)
					{
						long num;
						if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out num))
						{
							this.NotifyTeamCmd(client, -3, 1, 0L, "", -1, -1, -1);
							return;
						}
					}
					if (this.FuBenId2Watchers.ContainsKey(copyId))
					{
						if (!this.IsKuaFuCopy(copyId))
						{
							this.OnTeamCreate(new CopyTeamCreateData
							{
								Member = this.ClientDataToTeamMemberData(client.ClientData),
								MinCombat = minCombat,
								TeamId = SingletonTemplate<UniqueTeamId>.Instance().Create(),
								CopyId = copyId,
								AutoStart = autoStart,
								AutoKick = kickNoReady
							});
							this.HandleSetReady(client, 1);
						}
						else
						{
							KFCopyTeamCreateReq kfcopyTeamCreateReq = new KFCopyTeamCreateReq();
							kfcopyTeamCreateReq.Member = this.ClientDataToTeamMemberData(client.ClientData);
							kfcopyTeamCreateReq.Member.RoleName = Global.FormatNameWithZoneId(kfcopyTeamCreateReq.Member.ZoneId, kfcopyTeamCreateReq.Member.RoleName);
							kfcopyTeamCreateReq.CopyId = copyId;
							kfcopyTeamCreateReq.MinCombat = minCombat;
							kfcopyTeamCreateReq.AutoStart = autoStart;
							kfcopyTeamCreateReq.AutoKick = kickNoReady;
							kfcopyTeamCreateReq.TeamId = SingletonTemplate<UniqueTeamId>.Instance().Create();
							KFCopyTeamCreateRsp kfcopyTeamCreateRsp = KFCopyRpcClient.getInstance().CreateTeam(kfcopyTeamCreateReq);
							if (kfcopyTeamCreateRsp == null)
							{
								LogManager.WriteLog(2, string.Format("KF 创建队伍RPC调用失败 roleid={0}, rolename={1}, copyid={2}", client.ClientData.RoleID, client.ClientData.RoleName, copyId), null, true);
								this.NotifyTeamCmd(client, -13, 1, 0L, "", -1, -1, -1);
							}
							else if (kfcopyTeamCreateRsp.ErrorCode == 1)
							{
								this.OnTeamCreate(kfcopyTeamCreateRsp.Data);
							}
							else
							{
								this.NotifyTeamCmd(client, kfcopyTeamCreateRsp.ErrorCode, 1, 0L, "", -1, -1, -1);
								LogManager.WriteLog(2, string.Format("KF 创建队伍失败 roleid={0}, rolename={1}, copyid={2}, errorcode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									copyId,
									kfcopyTeamCreateRsp.ErrorCode
								}), null, true);
							}
						}
					}
				}
			}
		}

		public void HandleApplyCopyTeam(GameClient client, long teamId)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				lock (this.Mutex)
				{
					long num;
					if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out num))
					{
						this.NotifyTeamCmd(client, -3, 4, 0L, "", -1, -1, -1);
					}
					else
					{
						CopyTeamData copyTeamData = null;
						if (!this.CopyTeamDict.TryGetValue(teamId, out copyTeamData))
						{
							this.NotifyListTeamRemove(client, teamId, -1);
						}
						else
						{
							SystemXmlItem fubenItem = null;
							if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyTeamData.FuBenId, out fubenItem))
							{
								LogManager.WriteLog(2, string.Format("HandleApplyCopyTeam Faild copyId={0}", copyTeamData.FuBenId), null, true);
							}
							else if (!FuBenChecker.HasFinishedPreTask(client, fubenItem))
							{
								LogManager.WriteLog(2, string.Format("HandleApplyCopyTeam Faild !HasFinishedPreTask copyId={0}", copyTeamData.FuBenId), null, true);
							}
							else if (!FuBenChecker.HasPassedPreCopy(client, fubenItem))
							{
								LogManager.WriteLog(2, string.Format("HandleApplyCopyTeam Faild !HasPassedPreCopy copyId={0}", copyTeamData.FuBenId), null, true);
							}
							else if (!FuBenChecker.IsInCopyLevelLimit(client, fubenItem))
							{
								LogManager.WriteLog(2, string.Format("HandleApplyCopyTeam Faild !IsInCopyLevelLimit copyId={0}", copyTeamData.FuBenId), null, true);
							}
							else if (!FuBenChecker.IsInCopyTimesLimit(client, fubenItem))
							{
								this.NotifyTeamCmd(client, -21, 4, 0L, "", -1, -1, -1);
							}
							else if (copyTeamData.StartTime > 0L)
							{
								this.NotifyTeamCmd(client, -15, 4, 0L, "", -1, -1, -1);
							}
							else if (copyTeamData.TeamRoles.Count >= ConstData.CopyRoleMax(copyTeamData.FuBenId))
							{
								this.NotifyTeamCmd(client, -5, 4, 0L, "", -1, -1, -1);
							}
							else if (client.ClientData.CombatForce < copyTeamData.MinZhanLi)
							{
								this.NotifyTeamCmd(client, -6, 4, 0L, "", -1, -1, -1);
							}
							else if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
							{
								this.OnTeamJoin(new CopyTeamJoinData
								{
									Member = this.ClientDataToTeamMemberData(client.ClientData),
									TeamId = teamId,
									Member = 
									{
										NoReadyTicks = TimeUtil.NOW()
									}
								});
							}
							else
							{
								KFCopyTeamJoinReq kfcopyTeamJoinReq = new KFCopyTeamJoinReq();
								kfcopyTeamJoinReq.Member = this.ClientDataToTeamMemberData(client.ClientData);
								kfcopyTeamJoinReq.Member.RoleName = Global.FormatNameWithZoneId(kfcopyTeamJoinReq.Member.ZoneId, kfcopyTeamJoinReq.Member.RoleName);
								kfcopyTeamJoinReq.CopyId = copyTeamData.FuBenId;
								kfcopyTeamJoinReq.TeamId = copyTeamData.TeamID;
								KFCopyTeamJoinRsp kfcopyTeamJoinRsp = KFCopyRpcClient.getInstance().JoinTeam(kfcopyTeamJoinReq);
								if (kfcopyTeamJoinRsp == null)
								{
									LogManager.WriteLog(2, string.Format("KF 加入队伍RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, teamId), null, true);
									this.NotifyTeamCmd(client, -13, 4, 0L, "", -1, -1, -1);
								}
								else if (kfcopyTeamJoinRsp.ErrorCode == 1)
								{
									this.OnTeamJoin(kfcopyTeamJoinRsp.Data);
								}
								else
								{
									LogManager.WriteLog(2, string.Format("KF 加入队伍失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										teamId,
										kfcopyTeamJoinRsp.ErrorCode
									}), null, true);
									this.NotifyTeamCmd(client, kfcopyTeamJoinRsp.ErrorCode, 4, 0L, "", -1, -1, -1);
									if (kfcopyTeamJoinRsp.ErrorCode == -2)
									{
										LogManager.WriteLog(2, string.Format("KF 加入队伍, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, teamId), null, true);
										this.OnTeamDestroy(new CopyTeamDestroyData
										{
											TeamId = kfcopyTeamJoinReq.TeamId
										});
									}
								}
							}
						}
					}
				}
			}
		}

		public void HandleKickoutCopyTeam(GameClient client, int otherRoleId)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				lock (this.Mutex)
				{
					long key;
					CopyTeamData copyTeamData;
					if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						this.NotifyTeamCmd(client, -1, 8, 0L, "", -1, -1, -1);
					}
					else if (!this.CopyTeamDict.TryGetValue(key, out copyTeamData))
					{
						this.RoleId2JoinedTeam.Remove(client.ClientData.RoleID);
						this.NotifyTeamCmd(client, -2, 8, 0L, "", -1, -1, -1);
					}
					else if (copyTeamData.LeaderRoleID != client.ClientData.RoleID || client.ClientData.RoleID == otherRoleId)
					{
						this.NotifyTeamCmd(client, -4, 8, 0L, "", -1, -1, -1);
					}
					else if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
					{
						this.OnTeamKickout(new CopyTeamKickoutData
						{
							FromRoleId = client.ClientData.RoleID,
							ToRoleId = otherRoleId,
							TeamId = copyTeamData.TeamID
						});
					}
					else
					{
						KFCopyTeamKickoutReq kfcopyTeamKickoutReq = new KFCopyTeamKickoutReq();
						kfcopyTeamKickoutReq.FromRoleId = client.ClientData.RoleID;
						kfcopyTeamKickoutReq.ToRoleId = otherRoleId;
						kfcopyTeamKickoutReq.TeamId = copyTeamData.TeamID;
						KFCopyTeamKickoutRsp kfcopyTeamKickoutRsp = KFCopyRpcClient.getInstance().KickoutTeam(kfcopyTeamKickoutReq);
						if (kfcopyTeamKickoutRsp == null)
						{
							LogManager.WriteLog(2, string.Format("KF 队伍踢人RPC调用失败 roleid={0}, rolename={1}, otherrid={2}", client.ClientData.RoleID, client.ClientData.RoleName, otherRoleId), null, true);
							this.NotifyTeamCmd(client, -13, 8, 0L, "", -1, -1, -1);
						}
						else if (kfcopyTeamKickoutRsp.ErrorCode == 1)
						{
							this.OnTeamKickout(kfcopyTeamKickoutRsp.Data);
						}
						else
						{
							this.NotifyTeamCmd(client, kfcopyTeamKickoutRsp.ErrorCode, 8, 0L, "", -1, -1, -1);
							LogManager.WriteLog(2, string.Format("KF 队伍踢人失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								kfcopyTeamKickoutReq.TeamId,
								kfcopyTeamKickoutRsp.ErrorCode
							}), null, true);
							if (kfcopyTeamKickoutRsp.ErrorCode == -2)
							{
								LogManager.WriteLog(2, string.Format("KF 队伍踢人, 队伍在中心已销毁 roleid={0}, rolename={1}, otherrid={2}", client.ClientData.RoleID, client.ClientData.RoleName, otherRoleId), null, true);
								this.OnTeamDestroy(new CopyTeamDestroyData
								{
									TeamId = kfcopyTeamKickoutReq.TeamId
								});
							}
						}
					}
				}
			}
		}

		public void HandleQuitFromTeam(GameClient client, bool notifyOther = true)
		{
			lock (this.Mutex)
			{
				long key;
				if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
				{
					CopyTeamData copyTeamData;
					if (!this.CopyTeamDict.TryGetValue(key, out copyTeamData))
					{
						this.RoleId2JoinedTeam.Remove(client.ClientData.RoleID);
					}
					else
					{
						CopyTeamMemberData copyTeamMemberData = copyTeamData.TeamRoles.Find((CopyTeamMemberData _role) => _role.RoleID == client.ClientData.RoleID);
						if (copyTeamMemberData != null)
						{
							if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
							{
								this.OnTeamLeave(new CopyTeamLeaveData
								{
									TeamId = copyTeamData.TeamID,
									RoleId = client.ClientData.RoleID
								});
							}
							else
							{
								KFCopyTeamLeaveReq kfcopyTeamLeaveReq = new KFCopyTeamLeaveReq();
								kfcopyTeamLeaveReq.ReqServerId = this.ThisServerId;
								kfcopyTeamLeaveReq.RoleId = client.ClientData.RoleID;
								kfcopyTeamLeaveReq.TeamId = copyTeamData.TeamID;
								KFCopyTeamLeaveRsp kfcopyTeamLeaveRsp = KFCopyRpcClient.getInstance().LeaveTeam(kfcopyTeamLeaveReq);
								if (kfcopyTeamLeaveRsp == null)
								{
									LogManager.WriteLog(2, string.Format("KF 离开队伍RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamLeaveReq.TeamId), null, true);
									this.NotifyTeamCmd(client, -13, 9, 0L, "", -1, -1, -1);
								}
								else if (kfcopyTeamLeaveRsp.ErrorCode == 1)
								{
									this.OnTeamLeave(kfcopyTeamLeaveRsp.Data);
								}
								else
								{
									this.NotifyTeamCmd(client, kfcopyTeamLeaveRsp.ErrorCode, 9, 0L, "", -1, -1, -1);
									LogManager.WriteLog(2, string.Format("KF 离开队伍失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
									{
										client.ClientData.RoleID,
										client.ClientData.RoleName,
										kfcopyTeamLeaveReq.TeamId,
										kfcopyTeamLeaveRsp.ErrorCode
									}), null, true);
									if (kfcopyTeamLeaveRsp.ErrorCode == -2)
									{
										LogManager.WriteLog(2, string.Format("KF 离开队伍, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamLeaveReq.TeamId), null, true);
										this.OnTeamDestroy(new CopyTeamDestroyData
										{
											TeamId = kfcopyTeamLeaveReq.TeamId
										});
									}
								}
							}
						}
					}
				}
			}
		}

		public void HandleSetReady(GameClient client, int ready)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				lock (this.Mutex)
				{
					long key;
					CopyTeamData copyTeamData;
					if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
					}
					else if (this.CopyTeamDict.TryGetValue(key, out copyTeamData))
					{
						if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
						{
							this.OnTeamSetReady(new CopyTeamReadyData
							{
								RoleId = client.ClientData.RoleID,
								TeamId = copyTeamData.TeamID,
								Ready = ready
							});
						}
						else
						{
							KFCopyTeamSetReadyReq kfcopyTeamSetReadyReq = new KFCopyTeamSetReadyReq();
							kfcopyTeamSetReadyReq.RoleId = client.ClientData.RoleID;
							kfcopyTeamSetReadyReq.TeamId = copyTeamData.TeamID;
							kfcopyTeamSetReadyReq.Ready = ready;
							KFCopyTeamSetReadyRsp kfcopyTeamSetReadyRsp = KFCopyRpcClient.getInstance().SetReady(kfcopyTeamSetReadyReq);
							if (kfcopyTeamSetReadyRsp == null)
							{
								LogManager.WriteLog(2, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetReadyReq.TeamId), null, true);
								this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
							}
							else if (kfcopyTeamSetReadyRsp.ErrorCode == 1)
							{
								this.OnTeamSetReady(kfcopyTeamSetReadyRsp.Data);
							}
							else
							{
								this.NotifyTeamStateChanged(client, kfcopyTeamSetReadyRsp.ErrorCode, client.ClientData.RoleID, 0);
								LogManager.WriteLog(2, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									kfcopyTeamSetReadyReq.TeamId,
									kfcopyTeamSetReadyRsp.ErrorCode
								}), null, true);
								if (kfcopyTeamSetReadyRsp.ErrorCode == -2)
								{
									LogManager.WriteLog(2, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetReadyReq.TeamId), null, true);
									this.OnTeamDestroy(new CopyTeamDestroyData
									{
										TeamId = kfcopyTeamSetReadyReq.TeamId
									});
								}
							}
						}
					}
				}
			}
		}

		public void HandleQuickJoinTeam(GameClient client, int copyId)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				SystemXmlItem fubenItem = null;
				if (!GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyId, out fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleQuickJoinTeam Faild copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.HasFinishedPreTask(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleQuickJoinTeam Faild !HasFinishedPreTask copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.HasPassedPreCopy(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleQuickJoinTeam Faild !HasPassedPreCopy copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.IsInCopyLevelLimit(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleQuickJoinTeam Faild !IsInCopyLevelLimit copyId={0}", copyId), null, true);
				}
				else if (!FuBenChecker.IsInCopyTimesLimit(client, fubenItem))
				{
					LogManager.WriteLog(2, string.Format("HandleQuickJoinTeam Faild !IsInCopyTimesLimit copyId={0}", copyId), null, true);
				}
				else
				{
					lock (this.Mutex)
					{
						if (this.RoleId2JoinedTeam.ContainsKey(client.ClientData.RoleID))
						{
							this.NotifyTeamCmd(client, -3, 13, 0L, "", -1, -1, -1);
						}
						else
						{
							int combatForce = client.ClientData.CombatForce;
							HashSet<long> hashSet = null;
							if (!this.FuBenId2Teams.TryGetValue(copyId, out hashSet) || hashSet.Count <= 0)
							{
								this.NotifyTeamCmd(client, -7, 13, -1L, "", -1, -1, -1);
							}
							else
							{
								CopyTeamData copyTeamData = null;
								foreach (long num in hashSet.ToList<long>())
								{
									CopyTeamData copyTeamData2 = null;
									if (!this.CopyTeamDict.TryGetValue(num, out copyTeamData2))
									{
										hashSet.Remove(num);
									}
									else if (copyTeamData2.StartTime <= 0L && combatForce >= copyTeamData2.MinZhanLi && copyTeamData2.MemberCount < ConstData.CopyRoleMax(copyTeamData2.FuBenId))
									{
										copyTeamData = copyTeamData2;
										break;
									}
								}
								if (copyTeamData == null)
								{
									this.NotifyTeamCmd(client, -7, 13, -1L, "", -1, -1, -1);
								}
								else
								{
									this.HandleApplyCopyTeam(client, copyTeamData.TeamID);
								}
							}
						}
					}
				}
			}
		}

		public void HandleModKickFlag(GameClient client, int flag)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				lock (this.Mutex)
				{
					long key;
					CopyTeamData copyTeamData;
					if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
					}
					else if (this.CopyTeamDict.TryGetValue(key, out copyTeamData))
					{
						if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
						{
							this.OnTeamSetFlag(new CopyTeamFlagData
							{
								RoleId = client.ClientData.RoleID,
								TeamId = copyTeamData.TeamID,
								AutoKick = flag,
								AutoStart = (copyTeamData.AutoStart ? 1 : 0)
							});
						}
						else
						{
							KFCopyTeamSetFlagReq kfcopyTeamSetFlagReq = new KFCopyTeamSetFlagReq();
							kfcopyTeamSetFlagReq.RoleId = client.ClientData.RoleID;
							kfcopyTeamSetFlagReq.TeamId = copyTeamData.TeamID;
							kfcopyTeamSetFlagReq.AutoStart = (copyTeamData.AutoStart ? 1 : 0);
							kfcopyTeamSetFlagReq.AutoKick = flag;
							KFCopyTeamSetFlagRsp kfcopyTeamSetFlagRsp = KFCopyRpcClient.getInstance().SetFlag(kfcopyTeamSetFlagReq);
							if (kfcopyTeamSetFlagRsp == null)
							{
								LogManager.WriteLog(2, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetFlagReq.TeamId), null, true);
								this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
							}
							else if (kfcopyTeamSetFlagRsp.ErrorCode == 1)
							{
								this.OnTeamSetFlag(kfcopyTeamSetFlagRsp.Data);
							}
							else
							{
								this.NotifyTeamCmd(client, kfcopyTeamSetFlagRsp.ErrorCode, 15, (long)copyTeamData.AutoKick, "", -1, -1, -1);
								LogManager.WriteLog(2, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									kfcopyTeamSetFlagReq.TeamId,
									kfcopyTeamSetFlagRsp.ErrorCode
								}), null, true);
								if (kfcopyTeamSetFlagRsp.ErrorCode == -2)
								{
									LogManager.WriteLog(2, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetFlagReq.TeamId), null, true);
									this.OnTeamDestroy(new CopyTeamDestroyData
									{
										TeamId = kfcopyTeamSetFlagReq.TeamId
									});
								}
							}
						}
					}
				}
			}
		}

		public void HandleModAutoStart(GameClient client, int flag)
		{
			if (!client.ClientSocket.IsKuaFuLogin)
			{
				lock (this.Mutex)
				{
					long key;
					CopyTeamData copyTeamData;
					if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
					}
					else if (this.CopyTeamDict.TryGetValue(key, out copyTeamData))
					{
						if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
						{
							this.OnTeamSetFlag(new CopyTeamFlagData
							{
								RoleId = client.ClientData.RoleID,
								TeamId = copyTeamData.TeamID,
								AutoKick = copyTeamData.AutoKick,
								AutoStart = flag
							});
						}
						else
						{
							KFCopyTeamSetFlagReq kfcopyTeamSetFlagReq = new KFCopyTeamSetFlagReq();
							kfcopyTeamSetFlagReq.RoleId = client.ClientData.RoleID;
							kfcopyTeamSetFlagReq.TeamId = copyTeamData.TeamID;
							kfcopyTeamSetFlagReq.AutoStart = flag;
							kfcopyTeamSetFlagReq.AutoKick = copyTeamData.AutoKick;
							KFCopyTeamSetFlagRsp kfcopyTeamSetFlagRsp = KFCopyRpcClient.getInstance().SetFlag(kfcopyTeamSetFlagReq);
							if (kfcopyTeamSetFlagRsp == null)
							{
								LogManager.WriteLog(2, string.Format("KF 设置准备状态RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetFlagReq.TeamId), null, true);
								this.NotifyTeamStateChanged(client, -13L, client.ClientData.RoleID, 0);
							}
							else if (kfcopyTeamSetFlagRsp.ErrorCode == 1)
							{
								this.OnTeamSetFlag(kfcopyTeamSetFlagRsp.Data);
							}
							else
							{
								this.NotifyTeamCmd(client, kfcopyTeamSetFlagRsp.ErrorCode, 16, copyTeamData.AutoStart ? 1L : 0L, "", -1, -1, -1);
								LogManager.WriteLog(2, string.Format("KF 设置准备状态失败 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
								{
									client.ClientData.RoleID,
									client.ClientData.RoleName,
									kfcopyTeamSetFlagReq.TeamId,
									kfcopyTeamSetFlagRsp.ErrorCode
								}), null, true);
								if (kfcopyTeamSetFlagRsp.ErrorCode == -2)
								{
									LogManager.WriteLog(2, string.Format("KF 设置准备状态, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamSetFlagReq.TeamId), null, true);
									this.OnTeamDestroy(new CopyTeamDestroyData
									{
										TeamId = kfcopyTeamSetFlagReq.TeamId
									});
								}
							}
						}
					}
				}
			}
		}

		public void HandleClickStart(GameClient client, int fubenSeqId)
		{
			lock (this.Mutex)
			{
				CopyTeamData copyTeamData = null;
				if (!this.CanEnterScene(client, out copyTeamData))
				{
					client.sendCmd(253, string.Format("{0}:{1}", -100, client.ClientData.RoleID), false);
				}
				else if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
				{
					this.OnTeamStart(new CopyTeamStartData
					{
						TeamId = copyTeamData.TeamID,
						StartMs = TimeUtil.NOW(),
						ToServerId = 0,
						FuBenSeqId = fubenSeqId
					});
				}
				else
				{
					SystemXmlItem systemXmlItem = null;
					FuBenMapItem fuBenMapItem = null;
					if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyTeamData.FuBenId, out systemXmlItem) && (fuBenMapItem = FuBenManager.FindMapCodeByFuBenID(copyTeamData.FuBenId, systemXmlItem.GetIntValue("MapCode", -1))) != null)
					{
						KFCopyTeamStartReq kfcopyTeamStartReq = new KFCopyTeamStartReq();
						kfcopyTeamStartReq.RoleId = client.ClientData.RoleID;
						kfcopyTeamStartReq.TeamId = copyTeamData.TeamID;
						kfcopyTeamStartReq.LastMs = fuBenMapItem.MaxTime * 60 * 1000;
						KFCopyTeamStartRsp kfcopyTeamStartRsp = KFCopyRpcClient.getInstance().StartGame(kfcopyTeamStartReq);
						if (kfcopyTeamStartRsp == null)
						{
							LogManager.WriteLog(2, string.Format("KF 开始游戏RPC调用失败 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamStartReq.TeamId), null, true);
						}
						else if (kfcopyTeamStartRsp.ErrorCode == 1)
						{
							this.OnTeamStart(kfcopyTeamStartRsp.Data);
						}
						else
						{
							LogManager.WriteLog(2, string.Format("KF 开始游戏 roleid={0}, rolename={1}, teamid={2}, errorcode={3}", new object[]
							{
								client.ClientData.RoleID,
								client.ClientData.RoleName,
								kfcopyTeamStartReq.TeamId,
								kfcopyTeamStartRsp.ErrorCode
							}), null, true);
							if (kfcopyTeamStartRsp.ErrorCode == -2)
							{
								LogManager.WriteLog(2, string.Format("KF 开始游戏, 队伍在中心已销毁 roleid={0}, rolename={1}, teamid={2}", client.ClientData.RoleID, client.ClientData.RoleName, kfcopyTeamStartReq.TeamId), null, true);
								this.OnTeamDestroy(new CopyTeamDestroyData
								{
									TeamId = kfcopyTeamStartReq.TeamId
								});
							}
						}
						client.sendCmd(253, string.Format("{0}:{1}", 1000, client.ClientData.RoleID), false);
					}
				}
			}
		}

		private void RegisterCopyTeamListNotify(GameClient client, int copyId)
		{
			lock (this.Mutex)
			{
				int roleID = client.ClientData.RoleID;
				foreach (KeyValuePair<int, HashSet<int>> keyValuePair in this.FuBenId2Watchers)
				{
					int key = keyValuePair.Key;
					HashSet<int> value = keyValuePair.Value;
					if (key == copyId)
					{
						if (!value.Contains(roleID))
						{
							value.Add(roleID);
						}
					}
					else
					{
						value.Remove(roleID);
					}
				}
			}
			this.SendTeamList(client, 0, copyId);
		}

		public void UnRegisterCopyTeamListNotify(GameClient client)
		{
			lock (this.Mutex)
			{
				foreach (KeyValuePair<int, HashSet<int>> keyValuePair in this.FuBenId2Watchers)
				{
					HashSet<int> value = keyValuePair.Value;
					value.Remove(client.ClientData.RoleID);
				}
			}
		}

		public bool CanEnterScene(GameClient client, out CopyTeamData td)
		{
			td = null;
			MarriageInstance marriageInstanceEX = MarryFuBenMgr.getInstance().GetMarriageInstanceEX(client);
			if (marriageInstanceEX != null)
			{
				if (MapTypes.MarriageCopy == Global.GetMapType(marriageInstanceEX.nHusband_FuBenID))
				{
					if (MarryFuBenMgr.getInstance().CanEnterSceneEX(client))
					{
						td = null;
						return true;
					}
					td = null;
					return false;
				}
			}
			bool result;
			lock (this.Mutex)
			{
				long key;
				if (!this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
				{
					this.NotifyTeamStateChanged(client, -1L, client.ClientData.RoleID, 0);
					result = false;
				}
				else if (this.CopyTeamDict.TryGetValue(key, out td) && td.LeaderRoleID == client.ClientData.RoleID)
				{
					if (this.IsKuaFuCopy(td.FuBenId))
					{
						result = true;
					}
					else
					{
						foreach (CopyTeamMemberData copyTeamMemberData in td.TeamRoles)
						{
							if (!copyTeamMemberData.IsReady)
							{
								return false;
							}
							GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
							if (gameClient == null)
							{
								copyTeamMemberData.IsReady = false;
								copyTeamMemberData.NoReadyTicks = TimeUtil.NOW();
								this.NotifyTeamData(td);
								return false;
							}
						}
						result = true;
					}
				}
				else
				{
					td = null;
					result = false;
				}
			}
			return result;
		}

		public bool CanEnterOtherScene(GameClient client)
		{
			lock (this.Mutex)
			{
				long num;
				if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out num))
				{
					return false;
				}
			}
			return true;
		}

		public void Update()
		{
			long num = TimeUtil.NOW();
			lock (this.Mutex)
			{
				if (num >= this.TimeLimitMemberNoReadyMs + 1000L)
				{
					foreach (CopyTeamData copyTeamData in this.CopyTeamDict.Values)
					{
						if (copyTeamData.AutoKick > 0)
						{
							List<CopyTeamLeaveData> list = new List<CopyTeamLeaveData>();
							foreach (CopyTeamMemberData copyTeamMemberData in copyTeamData.TeamRoles)
							{
								if (!copyTeamMemberData.IsReady && num > copyTeamMemberData.NoReadyTicks + 30000L)
								{
									list.Add(new CopyTeamLeaveData
									{
										TeamId = copyTeamData.TeamID,
										RoleId = copyTeamMemberData.RoleID
									});
								}
							}
							foreach (CopyTeamLeaveData data in list)
							{
								this.OnTeamLeave(data);
							}
						}
					}
				}
			}
		}

		public List<CopyTeamData> GetTeamDataList(int startIndex, int count, int sceneIndex, int zhanLi)
		{
			int num = 0;
			List<CopyTeamData> list = new List<CopyTeamData>();
			lock (this.CopyTeamDict)
			{
				foreach (CopyTeamData copyTeamData in this.CopyTeamDict.Values)
				{
					if (num >= startIndex && sceneIndex == copyTeamData.FuBenId && copyTeamData.StartTime == 0L && zhanLi >= copyTeamData.MinZhanLi && copyTeamData.MemberCount < ConstData.CopyRoleMax(sceneIndex))
					{
						list.Add(copyTeamData.SimpleClone());
						if (list.Count >= count)
						{
							break;
						}
					}
					num++;
				}
			}
			return list;
		}

		public List<CopyTeamData> GetTeamDataListInCopyMap(int sceneIndex = -1)
		{
			List<CopyTeamData> list = new List<CopyTeamData>();
			lock (this.CopyTeamDict)
			{
				foreach (CopyTeamData copyTeamData in this.CopyTeamDict.Values)
				{
					if (sceneIndex < 0 || sceneIndex == copyTeamData.FuBenId)
					{
						if (copyTeamData.StartTime > 0L && copyTeamData.FuBenSeqID > 0)
						{
							list.Add(copyTeamData);
						}
					}
				}
			}
			return list;
		}

		public CopyTeamMemberData ClientDataToTeamMemberData(SafeClientData clientData)
		{
			return new CopyTeamMemberData
			{
				RoleID = clientData.RoleID,
				RoleName = Global.FormatRoleName2(clientData, clientData.RoleName),
				RoleSex = clientData.RoleSex,
				Level = clientData.Level,
				Occupation = clientData.Occupation,
				RolePic = clientData.RolePic,
				MapCode = clientData.MapCode,
				OnlineState = 1,
				MaxLifeV = clientData.LifeV,
				CurrentLifeV = clientData.CurrentLifeV,
				MaxMagicV = clientData.MagicV,
				CurrentMagicV = clientData.CurrentMagicV,
				PosX = clientData.PosX,
				PosY = clientData.PosY,
				CombatForce = clientData.CombatForce,
				ChangeLifeLev = clientData.ChangeLifeCount,
				ServerId = this.ThisServerId,
				ZoneId = clientData.ZoneID
			};
		}

		public void RoleLeaveFuBen(GameClient client)
		{
			this.HandleQuitFromTeam(client, true);
		}

		public void OnPlayerLogin(GameClient client)
		{
			if (client != null)
			{
				lock (this.Mutex)
				{
					long key;
					if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						CopyTeamData copyTeamData;
						if (this.CopyTeamDict.TryGetValue(key, out copyTeamData))
						{
							if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
							{
								this.HandleQuitFromTeam(client, true);
							}
							else if (copyTeamData.StartTime > 0L && (copyTeamData.KFServerId == 0 || copyTeamData.KFServerId != this.ThisServerId))
							{
								this.HandleQuitFromTeam(client, false);
							}
						}
					}
				}
			}
		}

		public void OnPlayerLogout(GameClient client)
		{
			if (client != null)
			{
				this.UnRegisterCopyTeamListNotify(client);
				lock (this.Mutex)
				{
					long key;
					if (this.RoleId2JoinedTeam.TryGetValue(client.ClientData.RoleID, out key))
					{
						CopyTeamData copyTeamData;
						if (this.CopyTeamDict.TryGetValue(key, out copyTeamData))
						{
							if (!this.IsKuaFuCopy(copyTeamData.FuBenId))
							{
								this.HandleQuitFromTeam(client, true);
							}
							else if (copyTeamData.StartTime <= 0L)
							{
								this.HandleQuitFromTeam(client, true);
							}
						}
					}
				}
			}
		}

		public bool IsTeamCopyMapCode(int mapCode)
		{
			return this.MapCode2ToFubenId.ContainsKey(mapCode);
		}

		public List<int> GetTeamCopyMapCodes(int fubenId)
		{
			List<int> list = null;
			List<int> result;
			if (!this.FuBenId2MapCodes.TryGetValue(fubenId, out list))
			{
				result = null;
			}
			else
			{
				result = list;
			}
			return result;
		}

		public bool NeedRecordDamageInfoFuBenID(int fuBenID)
		{
			return this.RecordDamagesFuBenIDHashSet.Contains(fuBenID) || GameManager.GuildCopyMapMgr.IsGuildCopyMap(fuBenID);
		}

		public bool IsInRoleId2JoinedTeam(int nRid)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.RoleId2JoinedTeam.ContainsKey(nRid);
			}
			return result;
		}

		public void NotifyTeamListChange(CopyTeamData td)
		{
			if (td != null)
			{
				lock (this.Mutex)
				{
					HashSet<int> hashSet = null;
					if (this.FuBenId2Watchers.TryGetValue(td.FuBenId, out hashSet))
					{
						List<int> list = hashSet.ToList<int>();
						if (list != null && list.Count<int>() > 0)
						{
							foreach (int num in list)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(num);
								if (gameClient == null)
								{
									hashSet.Remove(num);
								}
								else if (td.MemberCount <= 0 || td.MinZhanLi <= gameClient.ClientData.CombatForce || td.StartTime > 0L)
								{
									this.NotifyListTeamData(gameClient, td);
								}
							}
						}
					}
				}
			}
		}

		public void NotifyListTeamData(GameClient client, CopyTeamData ctd)
		{
			int num = (ctd.StartTime > 0L) ? 0 : ctd.MemberCount;
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				ctd.FuBenId,
				ctd.TeamID,
				ctd.TeamName,
				num,
				ctd.MinZhanLi
			});
			client.sendCmd(625, cmdData, false);
		}

		public void NotifyListTeamRemove(GameClient client, long teamID, int sceneIndex = -1)
		{
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				sceneIndex,
				teamID,
				"",
				0,
				0
			});
			client.sendCmd(625, cmdData, false);
		}

		public void NotifyTeamCmd(GameClient client, CopyTeamErrorCodes status, int teamType, long extTag1, string extTag2, int nOccu = -1, int nLev = -1, int nChangeLife = -1)
		{
			string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
			{
				status,
				client.ClientData.RoleID,
				teamType,
				extTag1,
				extTag2,
				nOccu,
				nLev,
				nChangeLife
			});
			client.sendCmd(621, cmdData, false);
		}

		public void NotifyTeamData(CopyTeamData td)
		{
			if (td != null)
			{
				lock (this.Mutex)
				{
					foreach (CopyTeamMemberData copyTeamMemberData in td.TeamRoles)
					{
						if (copyTeamMemberData.ServerId == this.ThisServerId)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
							if (null != gameClient)
							{
								gameClient.sendCmd<CopyTeamData>(622, td, false);
							}
						}
					}
				}
			}
		}

		public void NotifyTeamCmds(CopyTeamData td, CopyTeamErrorCodes status, int teamType, long extTag1, string extTag2)
		{
			if (td != null)
			{
				lock (this.Mutex)
				{
					foreach (CopyTeamMemberData copyTeamMemberData in td.TeamRoles)
					{
						if (copyTeamMemberData.ServerId == this.ThisServerId)
						{
							GameClient gameClient = GameManager.ClientMgr.FindClient(copyTeamMemberData.RoleID);
							if (null != gameClient)
							{
								this.NotifyTeamCmd(gameClient, status, teamType, extTag1, extTag2, -1, -1, -1);
							}
						}
					}
				}
			}
		}

		public void NotifyTeamStateChanged(GameClient client, long teamID, int roleID, int isReady)
		{
			string cmdData = string.Format("{0}:{1}:{2}", roleID, teamID, isReady);
			client.sendCmd(623, cmdData, false);
		}

		public void NotifyTeamFuBenEnterMsg(List<int> roleIDsList, int minLevel, int maxLevel, int leaderMapCode, int leaderRoleID, int fuBenID, int fuBenSeqID, int enterNumber, int maxFinishNum, bool igoreNumLimit = false)
		{
			if (roleIDsList != null && roleIDsList.Count > 0)
			{
				for (int i = 0; i < roleIDsList.Count; i++)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(roleIDsList[i]);
					if (null != gameClient)
					{
						int unionLevel = Global.GetUnionLevel(gameClient.ClientData.ChangeLifeCount, gameClient.ClientData.Level, false);
						if (unionLevel >= minLevel && unionLevel <= maxLevel)
						{
							if (!igoreNumLimit)
							{
								FuBenData fuBenData = Global.GetFuBenData(gameClient, fuBenID);
								int num;
								int fuBenEnterNum = Global.GetFuBenEnterNum(fuBenData, out num);
								if ((enterNumber >= 0 && fuBenEnterNum >= enterNumber) || (maxFinishNum >= 0 && num >= maxFinishNum))
								{
									goto IL_D4;
								}
							}
							GameManager.ClientMgr.NotifyTeamMemberFuBenEnterMsg(gameClient, leaderRoleID, fuBenID, fuBenSeqID);
						}
					}
					IL_D4:;
				}
			}
		}

		public void SendTeamList(GameClient client, int startIndex, int copyId)
		{
			CopySearchTeamData copySearchTeamData = new CopySearchTeamData
			{
				StartIndex = startIndex,
				TotalTeamsCount = 0,
				PageTeamsCount = 100,
				TeamDataList = null
			};
			lock (this.Mutex)
			{
				copySearchTeamData.TotalTeamsCount = this.CopyTeamDict.Count<KeyValuePair<long, CopyTeamData>>();
				startIndex = ((startIndex >= this.CopyTeamDict.Count) ? 0 : startIndex);
				if (this.CopyTeamDict.Count > 0)
				{
					copySearchTeamData.TeamDataList = new List<CopyTeamData>();
					int num = 0;
					foreach (CopyTeamData copyTeamData in this.CopyTeamDict.Values)
					{
						if (num >= startIndex && copyId == copyTeamData.FuBenId && copyTeamData.StartTime == 0L && client.ClientData.CombatForce >= copyTeamData.MinZhanLi && copyTeamData.MemberCount < ConstData.CopyRoleMax(copyId))
						{
							copySearchTeamData.TeamDataList.Add(copyTeamData.SimpleClone());
							if (copySearchTeamData.TeamDataList.Count<CopyTeamData>() >= copySearchTeamData.PageTeamsCount)
							{
								break;
							}
						}
						num++;
					}
				}
			}
			client.sendCmd<CopySearchTeamData>(620, copySearchTeamData, false);
		}

		public void OnLeaveFuBen(GameClient client, SceneUIClasses sceneType)
		{
			CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
			if (!copyMap.CopyMapPassAwardFlag)
			{
				KuaFuManager.getInstance().SetCannotJoinKuaFu_UseAutoEndTicks(client);
			}
		}

		public bool AddCopyScenes(GameClient client, CopyMap copyMap, SceneUIClasses sceneType)
		{
			int fuBenSeqID = copyMap.FuBenSeqID;
			int mapCode = copyMap.MapCode;
			FuBenManager.AddFuBenSeqID(client.ClientData.RoleID, copyMap.FuBenSeqID, 0, copyMap.FubenMapID);
			return true;
		}

		public bool RemoveCopyScene(CopyMap copyMap, SceneUIClasses sceneType)
		{
			return true;
		}

		public void TimerProc()
		{
		}

		public const int ConstCopyType = 1;

		private HashSet<int> RecordDamagesFuBenIDHashSet = new HashSet<int>();

		private Dictionary<int, HashSet<int>> FuBenId2Watchers = new Dictionary<int, HashSet<int>>();

		private Dictionary<int, HashSet<long>> FuBenId2Teams = new Dictionary<int, HashSet<long>>();

		private Dictionary<int, int> MapCode2ToFubenId = new Dictionary<int, int>();

		private Dictionary<int, List<int>> FuBenId2MapCodes = new Dictionary<int, List<int>>();

		private Dictionary<long, CopyTeamData> CopyTeamDict = new Dictionary<long, CopyTeamData>();

		public long TimeLimitMemberNoReadyMs = 0L;

		private Dictionary<int, long> CreatKuaFuCopyLinkIntervalDic = new Dictionary<int, long>();

		private int ThisServerId;

		private Dictionary<int, long> RoleId2JoinedTeam = new Dictionary<int, long>();

		private Dictionary<int, long> FuBenSeq2TeamId = new Dictionary<int, long>();

		private object Mutex = new object();
	}
}
