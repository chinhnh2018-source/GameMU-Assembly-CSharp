using System;
using System.Collections.Generic;
using System.Linq;
using GameServer.Core.GameEvent;
using GameServer.Server;
using GameServer.Tools;
using KF.Client;
using Server.Data;
using Server.Tools;
using Tmsk.Contract;
using Tmsk.Contract.KuaFuData;

namespace GameServer.Logic.UnionAlly
{
	public class AllyManager : IManager, ICmdProcessorEx, ICmdProcessor, IEventListenerEx
	{
		public static AllyManager getInstance()
		{
			return AllyManager.instance;
		}

		public bool initialize()
		{
			return true;
		}

		public bool startup()
		{
			TCPCmdDispatcher.getInstance().registerProcessorEx(1042, 2, 2, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1043, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1044, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1045, 2, 2, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1046, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1047, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			TCPCmdDispatcher.getInstance().registerProcessorEx(1048, 1, 1, AllyManager.getInstance(), TCPCmdFlags.IsStringArrayParams);
			GlobalEventSource4Scene.getInstance().registerListener(10026, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10027, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10028, 10004, AllyManager.getInstance());
			GlobalEventSource4Scene.getInstance().registerListener(10029, 10004, AllyManager.getInstance());
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

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			return true;
		}

		public bool processCmdEx(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			bool result;
			switch (nID)
			{
			case 1042:
				result = this.ProcessAllyRequestCmd(client, nID, bytes, cmdParams);
				break;
			case 1043:
				result = this.ProcessAllyCancelCmd(client, nID, bytes, cmdParams);
				break;
			case 1044:
				result = this.ProcessAllyRemoveCmd(client, nID, bytes, cmdParams);
				break;
			case 1045:
				result = this.ProcessAllyAgreeCmd(client, nID, bytes, cmdParams);
				break;
			case 1046:
				result = this.ProcessAllyDataCmd(client, nID, bytes, cmdParams);
				break;
			case 1047:
				result = this.ProcessAllyLogDataCmd(client, nID, bytes, cmdParams);
				break;
			default:
				result = true;
				break;
			}
			return result;
		}

		public void processEvent(EventObjectEx eventObject)
		{
			switch (eventObject.EventType)
			{
			case 10026:
			{
				KFNotifyAllyGameEvent kfnotifyAllyGameEvent = eventObject as KFNotifyAllyGameEvent;
				int unionID = kfnotifyAllyGameEvent.UnionID;
				List<AllyData> allyList = AllyClient.getInstance().HAllyDataList(unionID, 1);
				int num = 0;
				GameClient gameClient;
				while ((gameClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
				{
					lock (AllyClient.getInstance()._Mutex)
					{
						if (gameClient.ClientData.Faction == unionID)
						{
							gameClient.ClientData.AllyList = allyList;
						}
					}
				}
				break;
			}
			case 10027:
			{
				KFNotifyAllyLogGameEvent kfnotifyAllyLogGameEvent = eventObject as KFNotifyAllyLogGameEvent;
				if (null != kfnotifyAllyLogGameEvent)
				{
					List<AllyLogData> list = (List<AllyLogData>)kfnotifyAllyLogGameEvent.LogList;
					if (list != null && list.Count > 0)
					{
						foreach (AllyLogData logData in list)
						{
							this.DBAllyLogAdd(logData, 0);
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10028:
			{
				KFNotifyAllyTipGameEvent kfnotifyAllyTipGameEvent = eventObject as KFNotifyAllyTipGameEvent;
				if (null != kfnotifyAllyTipGameEvent)
				{
					int unionID = kfnotifyAllyTipGameEvent.UnionID;
					int tipID = kfnotifyAllyTipGameEvent.TipID;
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, unionID, GameManager.ServerId);
					if (bangHuiDetailData != null && this.IsAllyOpen(bangHuiDetailData.QiLevel))
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
						if (gameClient == null)
						{
							break;
						}
						lock (AllyClient.getInstance()._Mutex)
						{
							if (tipID == 14112)
							{
								int num2 = AllyClient.getInstance().AllyCount(unionID);
								if (num2 > 0 && this.IsAllyMax(num2))
								{
									break;
								}
							}
							gameClient._IconStateMgr.AddFlushIconState(14111, false);
							gameClient._IconStateMgr.AddFlushIconState((ushort)tipID, false);
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							gameClient._IconStateMgr.AddFlushIconState(14111, true);
							gameClient._IconStateMgr.AddFlushIconState((ushort)tipID, true);
							gameClient._IconStateMgr.SendIconStateToClient(gameClient);
							switch (tipID)
							{
							case 14112:
								gameClient.AllyTip[0] = 1;
								break;
							case 14113:
								gameClient.AllyTip[1] = 1;
								break;
							}
						}
					}
					eventObject.Handled = true;
				}
				break;
			}
			case 10029:
			{
				int num = 0;
				GameClient gameClient;
				while ((gameClient = GameManager.ClientMgr.GetNextClient(ref num, false)) != null)
				{
					lock (AllyClient.getInstance()._Mutex)
					{
						gameClient.ClientData.AllyList = null;
						this.UnionAllyInit(gameClient);
						AllyClient.getInstance().HRankClear();
						AllyClient.getInstance().HRankTopList(1);
					}
				}
				break;
			}
			}
		}

		public bool ProcessAllyDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				EAllyDataType dataType = Convert.ToInt32(cmdParams[0]);
				List<AllyData> allyData = this.GetAllyData(client, dataType);
				client.sendCmd<List<AllyData>>(nID, allyData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAllyLogDataCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLengthAndRole(client, nID, cmdParams, 1))
				{
					return false;
				}
				List<AllyLogData> allyLogData = this.GetAllyLogData(client);
				client.sendCmd<List<AllyLogData>>(nID, allyLogData, false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAllyRequestCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				int zoneID = Convert.ToInt32(cmdParams[0]);
				string unionName = cmdParams[1];
				EAlly eally = this.AllyRequest(client, zoneID, unionName);
				int num = eally;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAllyCancelCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int targetID = Convert.ToInt32(cmdParams[0]);
				EAlly eally = this.AllyOperate(client, targetID, 4);
				int num = eally;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAllyRemoveCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 1))
				{
					return false;
				}
				int targetID = Convert.ToInt32(cmdParams[0]);
				EAlly eally = this.AllyOperate(client, targetID, 3);
				int num = eally;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		public bool ProcessAllyAgreeCmd(GameClient client, int nID, byte[] bytes, string[] cmdParams)
		{
			try
			{
				if (!CheckHelper.CheckCmdLength(client, nID, cmdParams, 2))
				{
					return false;
				}
				int targetID = Convert.ToInt32(cmdParams[0]);
				EAllyOperate operateType = Convert.ToInt32(cmdParams[1]);
				EAlly eally = this.AllyOperate(client, targetID, operateType);
				int num = eally;
				client.sendCmd(nID, num.ToString(), false);
				return true;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return false;
		}

		private List<AllyData> GetAllyData(GameClient client, EAllyDataType dataType)
		{
			List<AllyData> list = new List<AllyData>();
			int faction = client.ClientData.Faction;
			List<AllyData> result;
			if (faction <= 0)
			{
				result = list;
			}
			else
			{
				BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, faction, client.ServerId);
				if (bangHuiDetailData == null || !this.IsAllyOpen(bangHuiDetailData.QiLevel))
				{
					result = list;
				}
				else
				{
					switch (dataType)
					{
					case 1:
					{
						List<AllyData> list2 = AllyClient.getInstance().HAllyDataList(faction, 1);
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2);
						}
						list2 = AllyClient.getInstance().HAllyDataList(faction, 3);
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2);
						}
						break;
					}
					case 2:
					{
						List<AllyData> list2 = AllyClient.getInstance().HAllyDataList(faction, 2);
						if (list2 != null && list2.Count > 0)
						{
							list.AddRange(list2);
						}
						client.AllyTip[0] = 0;
						if (client.AllyTip[1] <= 0)
						{
							client._IconStateMgr.AddFlushIconState(14111, false);
						}
						client._IconStateMgr.AddFlushIconState(14112, false);
						client._IconStateMgr.SendIconStateToClient(client);
						break;
					}
					}
					result = list;
				}
			}
			return result;
		}

		private EAlly AllyRequest(GameClient client, int zoneID, string unionName)
		{
			EAlly result;
			if (zoneID <= 0)
			{
				result = -2;
			}
			else if (string.IsNullOrEmpty(unionName))
			{
				result = -3;
			}
			else
			{
				int faction = client.ClientData.Faction;
				if (faction <= 0)
				{
					result = -4;
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, faction, client.ServerId);
					if (bangHuiDetailData == null)
					{
						result = -4;
					}
					else if (!this.IsAllyOpen(bangHuiDetailData.QiLevel))
					{
						result = -5;
					}
					else if (bangHuiDetailData.ZoneID == zoneID && bangHuiDetailData.BHName == unionName)
					{
						result = -6;
					}
					else if (bangHuiDetailData.BZRoleID != client.ClientData.RoleID)
					{
						result = -7;
					}
					else if (!this.UnionMoneyIsMore(bangHuiDetailData.TotalMoney))
					{
						result = -8;
					}
					else if (AllyClient.getInstance().UnionIsAlly(faction, zoneID, unionName))
					{
						result = -9;
					}
					else if (AllyClient.getInstance().UnionIsRequest(faction, zoneID, unionName))
					{
						result = -10;
					}
					else if (AllyClient.getInstance().UnionIsAccept(faction, zoneID, unionName))
					{
						result = -10;
					}
					else
					{
						int num = AllyClient.getInstance().AllyCount(faction);
						int num2 = AllyClient.getInstance().AllyRequestCount(faction);
						if (num > 0 && this.IsAllyMax(num))
						{
							result = -11;
						}
						else
						{
							int num3 = num + num2;
							if (num3 > 0 && this.IsAllyMax(num3))
							{
								result = -12;
							}
							else
							{
								EAlly eally = AllyClient.getInstance().HAllyRequest(faction, zoneID, unionName);
								if (eally == 1)
								{
									int num4 = 0;
									if (!GameManager.ClientMgr.SubBangHuiTongQian(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, this.AllyCostMoney, out num4))
									{
										LogManager.WriteLog(2, "战盟结盟 申请 扣除战盟资金失败", null, true);
									}
								}
								result = eally;
							}
						}
					}
				}
			}
			return result;
		}

		private EAlly AllyOperate(GameClient client, int targetID, EAllyOperate operateType)
		{
			EAlly result;
			if (targetID <= 0)
			{
				result = -13;
			}
			else
			{
				int faction = client.ClientData.Faction;
				if (faction <= 0)
				{
					result = -4;
				}
				else
				{
					BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, faction, client.ServerId);
					if (bangHuiDetailData == null)
					{
						result = -4;
					}
					else if (!this.IsAllyOpen(bangHuiDetailData.QiLevel))
					{
						result = -5;
					}
					else if (bangHuiDetailData.BZRoleID != client.ClientData.RoleID)
					{
						result = -7;
					}
					else
					{
						int num = 0;
						if (operateType == 1)
						{
							int num2 = AllyClient.getInstance().AllyCount(faction);
							int num3 = AllyClient.getInstance().AllyRequestCount(faction);
							if (num2 > 0 && this.IsAllyMax(num2))
							{
								return -11;
							}
							num = num2 + num3;
							if (num > 0 && this.IsAllyMax(num))
							{
								return -11;
							}
						}
						EAlly eally = AllyClient.getInstance().HAllyOperate(faction, targetID, operateType);
						if (eally == 12)
						{
							client.sendCmd(1048, (num + 1).ToString(), false);
						}
						result = eally;
					}
				}
			}
			return result;
		}

		private List<AllyLogData> GetAllyLogData(GameClient client)
		{
			List<AllyLogData> list = new List<AllyLogData>();
			int faction = client.ClientData.Faction;
			List<AllyLogData> result;
			if (faction <= 0)
			{
				result = list;
			}
			else
			{
				BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, faction, client.ServerId);
				if (bangHuiDetailData == null || !this.IsAllyOpen(bangHuiDetailData.QiLevel))
				{
					result = list;
				}
				else
				{
					client.AllyTip[1] = 0;
					if (client.AllyTip[0] <= 0)
					{
						client._IconStateMgr.AddFlushIconState(14111, false);
					}
					client._IconStateMgr.AddFlushIconState(14113, false);
					client._IconStateMgr.SendIconStateToClient(client);
					result = this.DBAllyLogData(faction, client.ServerId);
				}
			}
			return result;
		}

		public List<AllyLogData> DBAllyLogData(int unionID, int serverID)
		{
			List<AllyLogData> list = Global.sendToDB<List<AllyLogData>, int>(13122, unionID, serverID);
			if (list == null)
			{
				list = new List<AllyLogData>();
			}
			return list;
		}

		public bool DBAllyLogAdd(AllyLogData logData, int serverID)
		{
			return Global.sendToDB<bool, AllyLogData>(13123, logData, serverID);
		}

		public void UnionAllyInit(GameClient client)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				lock (AllyClient.getInstance()._Mutex)
				{
					int faction = client.ClientData.Faction;
					int serverId = client.ServerId;
					bool isKuaFuLogin = client.ClientSocket.IsKuaFuLogin;
					if (faction > 0)
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, faction, serverId);
						if (bangHuiDetailData != null && this.IsAllyOpen(bangHuiDetailData.QiLevel))
						{
							EAlly eally = AllyClient.getInstance().HUnionAllyInit(faction, isKuaFuLogin);
							if (eally == -18)
							{
								this.UnionDataChange(faction, serverId, false, 0);
							}
							else if (eally != 50)
							{
								LogManager.WriteLog(2, string.Format("战盟结盟：数据初始化失败 id={0}", eally), null, true);
							}
							List<AllyData> list = AllyClient.getInstance().HAllyDataList(faction, 1);
							if (list != null && list.Count > 0)
							{
								client.ClientData.AllyList = list;
							}
						}
					}
				}
			}
		}

		public bool UnionIsAlly(GameClient client, int targetID)
		{
			bool result;
			lock (AllyClient.getInstance()._Mutex)
			{
				if (client.ClientData.AllyList == null || client.ClientData.AllyList.Count <= 0)
				{
					result = false;
				}
				else
				{
					AllyData allyData = client.ClientData.AllyList.Find((AllyData data) => data.UnionID == targetID);
					bool flag2 = this.IsAllyMap(client.ClientData.MapCode);
					if (allyData != null && flag2)
					{
						result = true;
					}
					else
					{
						result = false;
					}
				}
			}
			return result;
		}

		public void UnionDataChange(int unionID, int serverID, bool isDel = false, int unionLevel = 0)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				if (unionID > 0)
				{
					if (isDel)
					{
						if (this.IsAllyOpen(unionLevel))
						{
							EAlly eally = AllyClient.getInstance().HUnionDel(unionID);
							if (eally != 50)
							{
								LogManager.WriteLog(2, string.Format("战盟结盟：战盟{0}解散失败 id={1}", unionID, eally), null, true);
							}
						}
					}
					else
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, unionID, serverID);
						if (bangHuiDetailData != null && this.IsAllyOpen(bangHuiDetailData.QiLevel))
						{
							AllyData allyData = new AllyData();
							allyData.UnionID = bangHuiDetailData.BHID;
							allyData.UnionZoneID = bangHuiDetailData.ZoneID;
							allyData.UnionName = bangHuiDetailData.BHName;
							allyData.UnionLevel = bangHuiDetailData.QiLevel;
							allyData.UnionNum = bangHuiDetailData.TotalNum;
							allyData.LeaderID = bangHuiDetailData.BZRoleID;
							allyData.LeaderName = bangHuiDetailData.BZRoleName;
							SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(allyData.LeaderID);
							if (null != safeClientDataFromLocalOrDB)
							{
								allyData.LeaderZoneID = safeClientDataFromLocalOrDB.ZoneID;
							}
							else
							{
								allyData.LeaderZoneID = bangHuiDetailData.ZoneID;
							}
							EAlly eally = AllyClient.getInstance().HUnionDataChange(allyData);
							if (eally != 50)
							{
								LogManager.WriteLog(2, string.Format("战盟结盟：战盟数据变更失败 id={0}", eally), null, true);
							}
						}
					}
				}
			}
		}

		public void UnionLeaderChangName(int roleId, string oldName, string newName)
		{
			if (!KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				if (!string.IsNullOrEmpty(oldName) && !string.IsNullOrEmpty(newName))
				{
					SafeClientData safeClientDataFromLocalOrDB = Global.GetSafeClientDataFromLocalOrDB(roleId);
					if (safeClientDataFromLocalOrDB != null && safeClientDataFromLocalOrDB.Faction > 0)
					{
						BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(-1, safeClientDataFromLocalOrDB.Faction, GameManager.ServerId);
						if (roleId == bangHuiDetailData.BZRoleID)
						{
							this.UnionDataChange(safeClientDataFromLocalOrDB.Faction, GameManager.ServerId, false, 0);
						}
					}
				}
			}
		}

		public bool IsAllyOpen(int unionLevel)
		{
			bool result;
			if (KuaFuManager.KuaFuWorldKuaFuGameServer)
			{
				result = false;
			}
			else
			{
				int num = (int)GameManager.systemParamsList.GetParamValueIntByName("AlignZhanMengLevel", -1);
				result = (unionLevel >= num);
			}
			return result;
		}

		private bool IsAllyMax(int numNow)
		{
			int num = (int)GameManager.systemParamsList.GetParamValueIntByName("AlignNum", -1);
			return numNow >= num;
		}

		private int AllyCostMoney
		{
			get
			{
				return (int)GameManager.systemParamsList.GetParamValueIntByName("AlignCostMoney", -1);
			}
		}

		private bool UnionMoneyIsMore(int myMoney)
		{
			int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("ZhanMengZiJin", ',');
			return paramValueIntArrayByName != null && myMoney - this.AllyCostMoney > paramValueIntArrayByName[0];
		}

		private int[] AllyMapArr
		{
			get
			{
				return GameManager.systemParamsList.GetParamValueIntArrayByName("AlignMap", ',');
			}
		}

		public bool IsAllyMap(int mapID)
		{
			return this.AllyMapArr.Contains(mapID);
		}

		private const int ALLY_LOG_MAX = 20;

		public const int _sceneType = 10004;

		public object _mutex = new object();

		private static AllyManager instance = new AllyManager();
	}
}
