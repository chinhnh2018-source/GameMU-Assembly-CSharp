using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows;
using System.Xml.Linq;
using GameServer.Core.Executor;
using GameServer.Core.GameEvent;
using GameServer.Core.GameEvent.EventOjectImpl;
using GameServer.Interface;
using GameServer.Logic.ActivityNew;
using GameServer.Logic.ActivityNew.SevenDay;
using GameServer.Logic.CheatGuard;
using GameServer.Logic.FluorescentGem;
using GameServer.Logic.JingJiChang;
using GameServer.Logic.LiXianBaiTan;
using GameServer.Logic.LiXianGuaJi;
using GameServer.Logic.LoginWaiting;
using GameServer.Logic.Marriage.CoupleArena;
using GameServer.Logic.Olympics;
using GameServer.Logic.OnePiece;
using GameServer.Logic.Reborn;
using GameServer.Logic.SecondPassword;
using GameServer.Logic.Spread;
using GameServer.Logic.Tarot;
using GameServer.Logic.UnionAlly;
using GameServer.Logic.UnionPalace;
using GameServer.Logic.UserReturn;
using GameServer.Logic.WanMota;
using GameServer.Logic.YueKa;
using GameServer.Server;
using KF.Client;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;
using Server.Tools.Pattern;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class ClientManager
	{
		public int GetMaxClientCount()
		{
			return 2000;
		}

		public void initialize(IEnumerable<XElement> mapItems)
		{
			this.Container.initialize(mapItems);
			for (int i = 0; i < 2000; i++)
			{
				this._ArrayClients[i] = null;
				this._FreeClientList.Add(i);
			}
		}

		public bool AddClient(GameClient client)
		{
			try
			{
				GameClient gameClient = this.FindClient(client.ClientData.RoleID);
				if (null != gameClient)
				{
					if (gameClient.ClientData.ClosingClientStep <= 0)
					{
						return false;
					}
					this.RemoveClient(gameClient);
				}
				int num = -1;
				lock (this._FreeClientList)
				{
					if (this._FreeClientList == null || this._FreeClientList.Count <= 0)
					{
						LogManager.WriteLog(2, string.Format("ClientManager::AddClient _FreeClientList.Count <= 0", new object[0]), null, true);
						return false;
					}
					num = this._FreeClientList[0];
					this._FreeClientList.RemoveAt(0);
				}
				this._ArrayClients[num] = client;
				client.ClientSocket.Nid = num;
				lock (this._DictClientNids)
				{
					this._DictClientNids[client.ClientData.RoleID] = num;
				}
				this.AddClientToContainer(client);
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(2, string.Format("ClientManager::AddClient ==>{0}", ex.ToString()), null, true);
				return false;
			}
			return true;
		}

		public void AddClientToContainer(GameClient client)
		{
			this.Container.AddObject(client.ClientData.RoleID, client.ClientData.MapCode, client);
		}

		public void RemoveClient(GameClient client)
		{
			try
			{
				int num = this.FindClientNid(client.ClientData.RoleID);
				if (num != client.ClientSocket.Nid)
				{
					LogManager.WriteLog(2, string.Format("ClientManager::RemoveClient nNid={0}, client.ClientSocket.Nid={1]", num, client.ClientSocket.Nid), null, true);
				}
			}
			catch (Exception ex)
			{
			}
			lock (this._DictClientNids)
			{
				try
				{
					this._DictClientNids.Remove(client.ClientData.RoleID);
				}
				catch (Exception ex2)
				{
					LogManager.WriteException(ex2.ToString());
					try
					{
						this._DictClientNids.Remove(client.ClientData.RoleID);
					}
					catch (Exception ex3)
					{
						LogManager.WriteException(string.Format("try agin:{0}", ex3.ToString()));
					}
				}
			}
			if (client.ClientSocket.Nid >= 0 && client.ClientSocket.Nid < 2000)
			{
				this._ArrayClients[client.ClientSocket.Nid] = null;
				lock (this._FreeClientList)
				{
					this._FreeClientList.Add(client.ClientSocket.Nid);
				}
			}
			else
			{
				LogManager.WriteLog(2, string.Format("ClientManager::RemoveClient nid={0} out range", client.ClientSocket.Nid), null, true);
			}
			client.ClientSocket.Nid = -1;
			this.RemoveClientFromContainer(client);
		}

		public void RemoveClientFromContainer(GameClient client)
		{
			GameMap gameMap = null;
			if (!GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap) || null == gameMap)
			{
				LogManager.WriteLog(2, "RemoveClientFromContainer 错误的地图编号：" + client.ClientData.MapCode, null, true);
			}
			else
			{
				bool flag = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].RemoveObject(client);
				if (!this.Container.RemoveObject(client.ClientData.RoleID, client.ClientData.MapCode, client) || !flag)
				{
					foreach (int num in GameManager.MapMgr.DictMaps.Keys)
					{
						GameManager.MapGridMgr.DictGrids[num].RemoveObject(client);
						this.Container.RemoveObject(client.ClientData.RoleID, num, client);
					}
				}
			}
		}

		public int FindClientNid(int RoleID)
		{
			int result = -1;
			lock (this._DictClientNids)
			{
				if (!this._DictClientNids.TryGetValue(RoleID, out result))
				{
					return -1;
				}
			}
			return result;
		}

		public GameClient FindClientByNid(int nNid)
		{
			GameClient result;
			if (nNid < 0 || nNid >= 2000)
			{
				result = null;
			}
			else
			{
				result = this._ArrayClients[nNid];
			}
			return result;
		}

		public GameClient FindClient(TMSKSocket socket)
		{
			GameClient result;
			if (null == socket)
			{
				result = null;
			}
			else
			{
				result = this.FindClientByNid(socket.Nid);
			}
			return result;
		}

		public GameClient FindClient(int roleID)
		{
			int nNid = this.FindClientNid(roleID);
			return this.FindClientByNid(nNid);
		}

		public bool ClientExists(GameClient client)
		{
			object obj = null;
			lock (this.Container.ObjectDict)
			{
				this.Container.ObjectDict.TryGetValue(client.ClientData.RoleID, out obj);
			}
			return null != obj;
		}

		public GameClient GetNextClient(ref int nNid, bool loading = false)
		{
			GameClient result;
			if (nNid < 0 || nNid >= 2000)
			{
				result = null;
			}
			else
			{
				GameClient gameClient = null;
				while (nNid < 2000)
				{
					if (null != this._ArrayClients[nNid])
					{
						gameClient = this._ArrayClients[nNid];
						if (!loading)
						{
							if (gameClient.ClientData.FirstPlayStart || gameClient.ClientData.ClosingClientStep != 0)
							{
								goto IL_71;
							}
						}
						nNid++;
						break;
					}
					IL_71:
					nNid++;
				}
				result = gameClient;
			}
			return result;
		}

		public List<object> GetMapClients(int mapCode)
		{
			return this.Container.GetObjectsByMap(mapCode);
		}

		public List<GameClient> GetMapGameClients(int mapCode)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			List<GameClient> list = new List<GameClient>();
			if (null != objectsByMap)
			{
				foreach (object obj in objectsByMap)
				{
					GameClient gameClient = obj as GameClient;
					if (null != gameClient)
					{
						list.Add(gameClient);
					}
				}
			}
			return list;
		}

		public List<GameClient> GetMapAliveClients(int mapCode)
		{
			List<GameClient> list = new List<GameClient>();
			List<object> mapClients = this.GetMapClients(mapCode);
			List<GameClient> result;
			if (null == mapClients)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null && gameClient.ClientData.CurrentLifeV > 0)
					{
						list.Add(gameClient);
					}
				}
				result = list;
			}
			return result;
		}

		public List<GameClient> GetMapAliveClientsEx(int mapCode, bool writeLog = true)
		{
			List<GameClient> list = new List<GameClient>();
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			List<GameClient> result;
			if (null == objectsByMap)
			{
				result = list;
			}
			else
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					GameClient gameClient = objectsByMap[i] as GameClient;
					if (gameClient != null && gameClient.ClientData.CurrentLifeV > 0)
					{
						bool flag = false;
						if (!gameClient.ClientData.WaitingNotifyChangeMap && !gameClient.ClientData.WaitingForChangeMap)
						{
							if (gameClient.ClientData.MapCode == mapCode && Global.IsPosReachable(mapCode, gameClient.ClientData.PosX, gameClient.ClientData.PosY))
							{
								flag = true;
								list.Add(gameClient);
							}
							else
							{
								LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数client.MapCode={0};maCode={1};", gameClient.ClientData.MapCode, mapCode), null, true);
							}
							if (writeLog && !flag)
							{
								string text = string.Format("存活玩家坐标非法:{6}({7}) mapCode:{0},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode,
									gameClient.ClientData.MapCode,
									gameClient.ClientData.WaitingNotifyChangeMap,
									gameClient.ClientData.WaitingForChangeMap,
									gameClient.ClientData.PosX,
									gameClient.ClientData.PosY,
									gameClient.ClientData.RoleID,
									gameClient.ClientData.RoleName
								});
								LogManager.WriteLog(2, text, null, true);
							}
						}
						else
						{
							LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数 WaitingNotifyChangeMap={0};WaitingForChangeMap={1}", gameClient.ClientData.WaitingNotifyChangeMap, gameClient.ClientData.WaitingForChangeMap), null, true);
						}
					}
					else if (null != gameClient)
					{
						LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数角色生命值:{0}", gameClient.ClientData.CurrentLifeV), null, true);
					}
				}
				result = list;
			}
			return result;
		}

		public int GetMapAliveClientCountEx(int mapCode)
		{
			int num = 0;
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			int result;
			if (null == objectsByMap)
			{
				result = num;
			}
			else
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					GameClient gameClient = objectsByMap[i] as GameClient;
					if (gameClient != null && gameClient.ClientData.CurrentLifeV > 0)
					{
						if (!gameClient.ClientData.WaitingNotifyChangeMap && !gameClient.ClientData.WaitingForChangeMap)
						{
							if (gameClient.ClientData.MapCode == mapCode && !Global.InOnlyObsByXY(ObjectTypes.OT_CLIENT, mapCode, gameClient.ClientData.PosX, gameClient.ClientData.PosY))
							{
								num++;
							}
							else
							{
								LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数client.MapCode={0};maCode={1};", gameClient.ClientData.MapCode, mapCode), null, true);
								string text = string.Format("玩家坐标:{6}({7}) mapCode:{0},clientMapCode{1}:,WaitingNotifyChangeMap:{2},WaitingForChangeMap:{3},PosX:{4},PosY{5}", new object[]
								{
									mapCode,
									gameClient.ClientData.MapCode,
									gameClient.ClientData.WaitingNotifyChangeMap,
									gameClient.ClientData.WaitingForChangeMap,
									gameClient.ClientData.PosX,
									gameClient.ClientData.PosY,
									gameClient.ClientData.RoleID,
									gameClient.ClientData.RoleName
								});
								LogManager.WriteLog(2, text, null, true);
							}
						}
						else
						{
							LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数 WaitingNotifyChangeMap={0};WaitingForChangeMap={1}", gameClient.ClientData.WaitingNotifyChangeMap, gameClient.ClientData.WaitingForChangeMap), null, true);
						}
					}
					else if (null != gameClient)
					{
						LogManager.WriteLog(2, string.Format("ArenaBattleManager::PK之王活动中统计剩余人数角色生命值:{0}", gameClient.ClientData.CurrentLifeV), null, true);
					}
				}
				result = num;
			}
			return result;
		}

		public int GetMapClientsCount(int mapCode)
		{
			return this.Container.GetObjectsCountByMap(mapCode);
		}

		public int GetClientCount()
		{
			int num = 0;
			lock (this._FreeClientList)
			{
				num = this._FreeClientList.Count;
			}
			return 2000 - num;
		}

		public int GetClientCountFromDict()
		{
			int result = 0;
			lock (this._DictClientNids)
			{
				result = this._DictClientNids.Count;
			}
			return result;
		}

		public string GetAllMapRoleNumStr()
		{
			return this.Container.GetAllMapRoleNumStr();
		}

		public GameClient GetFirstClient()
		{
			GameClient result = null;
			lock (this._DictClientNids)
			{
				if (this._DictClientNids.Count > 0)
				{
					using (Dictionary<int, int>.Enumerator enumerator = this._DictClientNids.GetEnumerator())
					{
						if (enumerator.MoveNext())
						{
							KeyValuePair<int, int> keyValuePair = enumerator.Current;
							return this.FindClientByNid(keyValuePair.Value);
						}
					}
				}
			}
			return result;
		}

		public GameClient GetRandomClient()
		{
			lock (this._DictClientNids)
			{
				if (this._DictClientNids.Count > 0)
				{
					int[] array = new int[2000];
					this._DictClientNids.Values.CopyTo(array, 0);
					int randomNumber = Global.GetRandomNumber(0, this._DictClientNids.Count);
					return this.FindClientByNid(array[randomNumber]);
				}
			}
			return null;
		}

		public void PushBackTcpOutPacket(TCPOutPacket tcpOutPacket)
		{
			if (null != tcpOutPacket)
			{
				Global._TCPManager.TcpOutPacketPool.Push(tcpOutPacket);
			}
		}

		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, byte[] bytesData, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpoutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient gameClient = objsList[i] as GameClient;
							if (null != gameClient)
							{
								if (!gameClient.LogoutState)
								{
									if (null == tcpoutPacket)
									{
										tcpoutPacket = pool.Pop();
										tcpoutPacket.PacketCmdID = (ushort)cmdID;
										tcpoutPacket.FinalWriteData(bytesData, 0, bytesData.Length);
									}
									if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpoutPacket, false))
									{
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpoutPacket);
				}
			}
		}

		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, string strCmd, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpoutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient gameClient = objsList[i] as GameClient;
							if (null != gameClient)
							{
								if (!gameClient.LogoutState)
								{
									if (null == tcpoutPacket)
									{
										tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, cmdID);
									}
									if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpoutPacket, false))
									{
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpoutPacket);
				}
			}
		}

		public void SendToClients<T>(SocketListener sl, TCPOutPacketPool pool, object self, List<object> objsList, T scData, int cmdID)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							if (objsList[i] is GameClient)
							{
								if (!(objsList[i] as GameClient).LogoutState)
								{
									(objsList[i] as GameClient).sendCmd<T>(cmdID, scData, false);
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		public void SendToClients<T1, T2>(SocketListener sl, TCPOutPacketPool pool, object self, List<T1> objsList, T2 data, int cmdID, int hideFlag, int includeRoleId)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpoutPacket = null;
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						if (self == null || self != objsList[i])
						{
							GameClient gameClient = objsList[i] as GameClient;
							if (null != gameClient)
							{
								if (gameClient.ClientData.RoleID == includeRoleId || (gameClient.ClientEffectHideFlag1 & hideFlag) <= 0)
								{
									if (!gameClient.LogoutState)
									{
										if (null == tcpoutPacket)
										{
											if (data is string)
											{
												tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as string, cmdID);
											}
											else
											{
												if (!(data is byte[]))
												{
													break;
												}
												tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as byte[], cmdID);
											}
										}
										if (!sl.SendData((objsList[i] as GameClient).ClientSocket, tcpoutPacket, false))
										{
										}
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpoutPacket);
				}
			}
		}

		public void SendToClients(SocketListener sl, TCPOutPacketPool pool, object self, object obj, List<object> objsList, byte[] bytesData, byte[] bytesData2, int cmdID, bool sendIfHide)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesData, 0, bytesData.Length, cmdID);
				TCPOutPacket tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, bytesData2, 0, bytesData2.Length, cmdID);
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient gameClient = objsList[i] as GameClient;
						if (null != gameClient)
						{
							if (!gameClient.LogoutState)
							{
								if (gameClient == self || gameClient == obj || gameClient.ClientEffectHideFlag1 <= 0)
								{
									sl.SendData(gameClient.ClientSocket, tcpOutPacket, false);
								}
								else if (sendIfHide)
								{
									sl.SendData(gameClient.ClientSocket, tcpOutPacket2, false);
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
					this.PushBackTcpOutPacket(tcpOutPacket2);
				}
			}
		}

		public void SendToClients<T1, T2>(SocketListener sl, TCPOutPacketPool pool, object self, object obj, List<T1> objsList, T2 data, T2 data2, int cmdID, int hideFlag, int includeRoleId)
		{
			if (null != objsList)
			{
				TCPOutPacket tcpOutPacket = null;
				TCPOutPacket tcpOutPacket2 = null;
				if (data is string)
				{
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as string, cmdID);
					tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, data2 as string, cmdID);
				}
				else
				{
					if (!(data is byte[]))
					{
						return;
					}
					tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data as byte[], cmdID);
					tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, data2 as byte[], cmdID);
				}
				try
				{
					for (int i = 0; i < objsList.Count; i++)
					{
						GameClient gameClient = objsList[i] as GameClient;
						if (null != gameClient)
						{
							if (gameClient.ClientData.RoleID == includeRoleId || (gameClient.ClientEffectHideFlag1 & hideFlag) <= 0)
							{
								if (!gameClient.LogoutState)
								{
									if (gameClient == self || gameClient == obj || gameClient.ClientEffectHideFlag1 <= 0)
									{
										sl.SendData(gameClient.ClientSocket, tcpOutPacket, false);
									}
									else
									{
										sl.SendData(gameClient.ClientSocket, tcpOutPacket2, false);
									}
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpOutPacket);
					this.PushBackTcpOutPacket(tcpOutPacket2);
				}
			}
		}

		public void SendToClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strCmd, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strCmd, cmdID);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public void SendToClient(GameClient client, string strCmd, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, strCmd, cmdID);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public void SendToClient(GameClient client, byte[] buffer, int cmdID)
		{
			if (null != client)
			{
				if (!client.LogoutState)
				{
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, buffer, 0, buffer.Length, cmdID);
					if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public void NotifyClientOpenWindow(GameClient client, int windowType, string strParams)
		{
			string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, windowType, strParams);
			this.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, strCmd, 419);
		}

		public void NotifyOthersIamComing(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, int cmd)
		{
			if (null != objsList)
			{
				RoleData instance = Global.ClientToRoleData2(client);
				byte[] bytesData = DataHelper.ObjectToBytes<RoleData>(instance);
				this.SendToClients(sl, pool, client, objsList, bytesData, cmd);
			}
		}

		public int NotifySelfOnlineOthers(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, int cmd)
		{
			int result;
			if (null == objsList)
			{
				result = 0;
			}
			else
			{
				int num = 0;
				int num2 = 0;
				while (num2 < objsList.Count && num2 < 30)
				{
					if (objsList[num2] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[num2] as GameClient).ClientData.RoleID)
						{
							TCPOutPacket tcpOutPacket;
							if (1 == GameManager.RoleDataMiniMode)
							{
								RoleDataMini roleDataMini = Global.ClientToRoleDataMini(objsList[num2] as GameClient);
								roleDataMini.BufferMiniInfo = Global.GetBufferMiniList(objsList[num2] as GameClient);
								tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDataMini>(roleDataMini, pool, cmd);
							}
							else
							{
								RoleData instance = Global.ClientToRoleData2(objsList[num2] as GameClient);
								tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(instance, pool, cmd);
							}
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
								break;
							}
							num++;
						}
					}
					num2++;
				}
				result = num;
			}
			return result;
		}

		public void NotifySelfOnline(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int cmd)
		{
			if (null != otherClient)
			{
				RoleData instance = Global.ClientToRoleData2(otherClient);
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(instance, pool, cmd);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifySelfOnlineData(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int cmd)
		{
			if (null != otherClient)
			{
				RoleData instance = Global.ClientToRoleData2(otherClient);
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(instance, pool, cmd);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifySelfOtherData(SocketListener sl, TCPOutPacketPool pool, GameClient client, RoleDataEx roleDataEx, int cmd)
		{
			RoleData instance = null;
			if (null != roleDataEx)
			{
				instance = Global.RoleDataExToRoleData(roleDataEx);
			}
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleData>(instance, pool, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMyselfOtherLoadAlready(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int mapCode, long startMoveTicks, int currentX, int currentY, int currentDirection, int action, int toX, int toY, double moveCost = 1.0, int extAction = 0, int currentPathIndex = 0)
		{
			GameClient gameClient = this.FindClient(otherRoleID);
			LoadAlreadyData instance = new LoadAlreadyData
			{
				RoleID = otherRoleID,
				MapCode = mapCode,
				StartMoveTicks = startMoveTicks,
				CurrentX = currentX,
				CurrentY = currentY,
				CurrentDirection = currentDirection,
				Action = action,
				ToX = toX,
				ToY = toY,
				MoveCost = moveCost,
				ExtAction = extAction,
				PathString = ((gameClient != null) ? gameClient.ClientData.RolePathString : ""),
				CurrentPathIndex = currentPathIndex
			};
			byte[] array = DataHelper.ObjectToBytes<LoadAlreadyData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 209);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMyselfOtherMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int mapCode, int action, long startMoveTicks, int fromX, int fromY, int toX, int toY, int cmd, double moveCost = 1.0, int extAction = 0)
		{
			GameClient gameClient = this.FindClient(otherRoleID);
			byte[] array = DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
			{
				roleID = otherRoleID,
				mapCode = mapCode,
				action = action,
				toX = toX,
				toY = toY,
				moveCost = moveCost,
				extAction = extAction,
				fromX = fromX,
				fromY = fromY,
				startMoveTicks = startMoveTicks,
				pathString = ((gameClient != null) ? gameClient.ClientData.RolePathString : "")
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMyselfOthersMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.CurrentAction == 1 || (objsList[i] as GameClient).ClientData.CurrentAction == 2)
							{
								GameManager.ClientMgr.NotifyMyselfOtherMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (objsList[i] as GameClient).ClientData.RoleID, (objsList[i] as GameClient).ClientData.MapCode, (objsList[i] as GameClient).ClientData.CurrentAction, Global.GetClientStartMoveTicks(objsList[i] as GameClient), (objsList[i] as GameClient).ClientData.PosX, (objsList[i] as GameClient).ClientData.PosY, (int)(objsList[i] as GameClient).ClientData.DestPoint.X, (int)(objsList[i] as GameClient).ClientData.DestPoint.Y, 107, (objsList[i] as GameClient).ClientData.MoveSpeed, 0);
							}
						}
					}
				}
			}
		}

		public void NotifyOthersMyMoving(SocketListener sl, TCPOutPacketPool pool, SpriteNotifyOtherMoveData moveData, GameClient client, int cmd, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				this.SendToClients(sl, pool, client, objsList, DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(moveData), cmd);
			}
		}

		public void NotifyOthersMyMovingEnd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int mapCode, int action, int toX, int toY, int direction, int tryRun, bool sendToSelf, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				SCMoveEnd scData = new SCMoveEnd(client.ClientData.RoleID, mapCode, action, toX, toY, direction, tryRun, 0L);
				this.SendToClients<SCMoveEnd>(sl, pool, sendToSelf ? null : client, objsList, scData, 108);
			}
		}

		public void NotifyOthersStopMyMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int stopIndex, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, stopIndex);
				this.SendToClients(sl, pool, client, objsList, strCmd, 411);
			}
		}

		public bool NotifyOthersToMoving(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, long startMoveTicks, int currentX, int currentY, int action, int toX, int toY, int cmd, double moveCost = 1.0, string pathString = "", List<object> objsList = null)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, currentX, currentY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
				{
					roleID = roleID,
					mapCode = mapCode,
					action = action,
					toX = toX,
					toY = toY,
					moveCost = moveCost,
					extAction = 0,
					fromX = currentX,
					fromY = currentY,
					startMoveTicks = startMoveTicks,
					pathString = pathString
				}), cmd);
				result = true;
			}
			return result;
		}

		public void NotifyMyselfMonsterLoadAlready(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID, int mapCode, long startMoveTicks, int currentX, int currentY, int currentDirection, int action, int toX, int toY, double moveCost = 1.0, int extAction = 0, string pathString = "", int currentPathIndex = 0)
		{
			LoadAlreadyData instance = new LoadAlreadyData
			{
				RoleID = monsterID,
				MapCode = mapCode,
				StartMoveTicks = startMoveTicks,
				CurrentX = currentX,
				CurrentY = currentY,
				CurrentDirection = currentDirection,
				Action = action,
				ToX = toX,
				ToY = toY,
				MoveCost = moveCost,
				ExtAction = extAction,
				PathString = pathString,
				CurrentPathIndex = currentPathIndex
			};
			byte[] array = DataHelper.ObjectToBytes<LoadAlreadyData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 209);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMyselfMonsterMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID, int mapCode, int action, long startMoveTicks, int fromX, int fromY, int toX, int toY, int cmd, double moveCost = 1.0, int extAction = 0, string pathString = "")
		{
			byte[] array = DataHelper.ObjectToBytes<SpriteNotifyOtherMoveData>(new SpriteNotifyOtherMoveData
			{
				roleID = monsterID,
				mapCode = mapCode,
				action = action,
				toX = toX,
				toY = toY,
				moveCost = moveCost,
				extAction = extAction,
				fromX = fromX,
				fromY = fromY,
				startMoveTicks = startMoveTicks,
				pathString = pathString
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, cmd);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMyselfMonstersMoving(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if ((objsList[i] as Monster).SafeAction == GActions.Walk || (objsList[i] as Monster).SafeAction == GActions.Run)
						{
							Monster monster = objsList[i] as Monster;
							GameManager.ClientMgr.NotifyMyselfMonsterMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (objsList[i] as Monster).RoleID, (objsList[i] as Monster).MonsterZoneNode.MapCode, (int)(objsList[i] as Monster).SafeAction, Global.GetMonsterStartMoveTicks(objsList[i] as Monster), (int)(objsList[i] as Monster).SafeCoordinate.X, (int)(objsList[i] as Monster).SafeCoordinate.Y, (int)(objsList[i] as Monster).DestPoint.X, (int)(objsList[i] as Monster).DestPoint.Y, 107, (objsList[i] as Monster).MoveSpeed, 0, "");
						}
					}
				}
			}
		}

		public void NotifyOthersMyAction(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int mapCode, int direction, int action, int x, int y, int targetX, int targetY, int yAngle, int moveToX, int moveToY, int cmd)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				this.SendToClients(sl, pool, null, all9Clients, DataHelper.ObjectToBytes<SpriteActionData>(new SpriteActionData
				{
					roleID = roleID,
					mapCode = mapCode,
					direction = direction,
					action = action,
					toX = x,
					toY = y,
					targetX = targetX,
					targetY = targetY,
					yAngle = yAngle,
					moveToX = moveToX,
					moveToY = moveToY
				}), cmd);
			}
		}

		public void NotifyOthersMyAction(SocketListener sl, TCPOutPacketPool pool, GameClient client, SpriteActionData cmdData, int cmd)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				this.SendToClients(sl, pool, null, all9Clients, DataHelper.ObjectToBytes<SpriteActionData>(cmdData), cmd);
			}
		}

		public void NotifyOthersDoAction(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int direction, int action, int x, int y, int targetX, int targetY, int cmd, List<object> objsList)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, x, y, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteActionData>(new SpriteActionData
				{
					roleID = roleID,
					mapCode = mapCode,
					direction = direction,
					action = action,
					toX = x,
					toY = y,
					targetX = targetX,
					targetY = targetY,
					yAngle = -1,
					moveToX = 0,
					moveToY = 0
				}), cmd);
			}
		}

		public void NotifyOthersChangeAngle(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int direction, int yAngle, int cmd)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", roleID, direction, yAngle);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, cmd);
			}
		}

		public void NotifyOthersMagicCode(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int roleID, int mapCode, int magicCode, int cmd)
		{
			List<object> all9Clients = Global.GetAll9Clients(attacker);
			if (null != all9Clients)
			{
				this.SendToClients(sl, pool, attacker, all9Clients, DataHelper.ObjectToBytes<SpriteMagicCodeData>(new SpriteMagicCodeData
				{
					roleID = roleID,
					mapCode = mapCode,
					magicCode = magicCode
				}), cmd);
			}
		}

		public void NotifySpriteHited(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int enemy, int enemyX, int enemyY, int magicCode)
		{
			List<object> all9Clients = Global.GetAll9Clients(attacker);
			if (null != all9Clients)
			{
				SpriteHitedData spriteHitedData = new SpriteHitedData();
				spriteHitedData.roleId = attacker.GetObjectID();
				spriteHitedData.enemy = enemy;
				spriteHitedData.magicCode = magicCode;
				if (enemy < 0)
				{
					spriteHitedData.enemyX = enemyX;
					spriteHitedData.enemyY = enemyY;
				}
				if (!GameManager.FlagEnableHideFlags || !GameManager.HideFlagsMapDict.ContainsKey(attacker.CurrentMapCode))
				{
					this.SendToClients(sl, pool, null, all9Clients, DataHelper.ObjectToBytes<SpriteHitedData>(spriteHitedData), 155);
				}
				else
				{
					GSpriteTypes spriteType = Global.GetSpriteType((uint)enemy);
					GameClient gameClient = attacker as GameClient;
					if (null != gameClient)
					{
						gameClient.sendCmd<SpriteHitedData>(155, spriteHitedData, false);
					}
					if (spriteType == GSpriteTypes.Other && (gameClient != null || GameManager.FlagHideFlagsType == 0))
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, all9Clients, DataHelper.ObjectToBytes<SpriteHitedData>(spriteHitedData), 155, 1, enemy);
					}
				}
				this.AddDelayDecoToMap(attacker, magicCode, attacker.CurrentMapCode, attacker.CurrentCopyMapID, enemyX, enemyY);
			}
		}

		public void AddDelayDecoToMap(IObject attacker, int magicCode, int mapCode, int copyMapID, int posX, int posY)
		{
			SystemXmlItem systemXmlItem = null;
			if (GameManager.SystemMagicsMgr.SystemXmlItemDict.TryGetValue(magicCode, out systemXmlItem))
			{
				if (systemXmlItem.GetIntValue("DelayDecoToMap", -1) > 0)
				{
					string stringValue = systemXmlItem.GetStringValue("MagicTime");
					if (!string.IsNullOrEmpty(stringValue))
					{
						string[] array = stringValue.Split(new char[]
						{
							','
						});
						if (array.Length > 0)
						{
							SkillData skillData = null;
							if (attacker is GameClient)
							{
								skillData = Global.GetSkillDataByID(attacker as GameClient, magicCode);
							}
							int num = (skillData == null) ? 0 : (skillData.SkillLevel - 1);
							num = Math.Min(num, array.Length - 1);
							int num2 = Global.SafeConvertToInt32(array[num]);
							if (num2 > 0)
							{
								int intValue = systemXmlItem.GetIntValue("DelayDecoration", -1);
								if (intValue > 0)
								{
									if (1 == systemXmlItem.GetIntValue("DelayDecoToMap", -1))
									{
										GameMap gameMap = GameManager.MapMgr.DictMaps[mapCode];
										Point item = new Point((double)(posX / gameMap.MapGridWidth), (double)(posY / gameMap.MapGridHeight));
										List<Point> list = new List<Point>();
										list.Add(item);
										list.Add(new Point(item.X, item.Y - 1.0));
										list.Add(new Point(item.X + 1.0, item.Y));
										list.Add(new Point(item.X, item.Y + 1.0));
										list.Add(new Point(item.X - 1.0, item.Y));
										for (int i = 0; i < list.Count; i++)
										{
											if (!Global.InOnlyObs(ObjectTypes.OT_CLIENT, mapCode, (int)list[i].X, (int)list[i].Y))
											{
												Point pos = new Point(list[i].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), list[i].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
												DecorationManager.AddDecoToMap(mapCode, copyMapID, pos, intValue, num2 * 1000, 2000, true);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifyOthersRealive(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int posX, int posY, int direction)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				MonsterRealiveData instance = new MonsterRealiveData
				{
					RoleID = roleID,
					PosX = posX,
					PosY = posY,
					Direction = direction
				};
				byte[] bytesData = DataHelper.ObjectToBytes<MonsterRealiveData>(instance);
				this.SendToClients(sl, pool, client, all9Clients, bytesData, 119);
				this.NotifyTeamRealive(sl, pool, roleID, posX, posY, direction);
			}
		}

		public void NotifyTeamRealive(SocketListener sl, TCPOutPacketPool pool, int roleID, int posX, int posY, int direction)
		{
			GameClient gameClient = this.FindClient(roleID);
			if (null != gameClient)
			{
				if (gameClient.ClientData.TeamID > 0)
				{
					TeamData teamData = GameManager.TeamMgr.FindData(gameClient.ClientData.TeamID);
					if (null != teamData)
					{
						List<int> list = new List<int>();
						lock (teamData)
						{
							for (int i = 0; i < teamData.TeamRoles.Count; i++)
							{
								if (roleID != teamData.TeamRoles[i].RoleID)
								{
									list.Add(teamData.TeamRoles[i].RoleID);
								}
							}
						}
						TCPOutPacket tcpoutPacket = null;
						try
						{
							for (int i = 0; i < list.Count; i++)
							{
								GameClient gameClient2 = this.FindClient(list[i]);
								if (null != gameClient2)
								{
									if (null == tcpoutPacket)
									{
										MonsterRealiveData instance = new MonsterRealiveData
										{
											RoleID = roleID,
											PosX = posX,
											PosY = posY,
											Direction = direction
										};
										byte[] array = DataHelper.ObjectToBytes<MonsterRealiveData>(instance);
										tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 119);
									}
									if (!sl.SendData(gameClient2.ClientSocket, tcpoutPacket, false))
									{
									}
								}
							}
						}
						finally
						{
							this.PushBackTcpOutPacket(tcpoutPacket);
						}
					}
				}
			}
		}

		public void NotifyMySelfRealive(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int posX, int posY, int direction)
		{
			MonsterRealiveData instance = new MonsterRealiveData
			{
				RoleID = roleID,
				PosX = posX,
				PosY = posY,
				Direction = direction
			};
			byte[] array = DataHelper.ObjectToBytes<MonsterRealiveData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 119);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMonsterRealive(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int posX, int posY, int direction, List<object> objsList)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, posX, posY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				MonsterRealiveData instance = new MonsterRealiveData
				{
					RoleID = roleID,
					PosX = posX,
					PosY = posY,
					Direction = direction
				};
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<MonsterRealiveData>(instance), 119);
			}
		}

		public void NotifyOthersLeave(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, 1);
				this.SendToClients(sl, pool, null, objsList, strCmd, 127);
			}
		}

		public void NotifyOthersMonsterLeave(SocketListener sl, TCPOutPacketPool pool, Monster monster, List<object> objsList)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", monster.RoleID, 2);
				this.SendToClients(sl, pool, null, objsList, strCmd, 127);
			}
		}

		public void NotifyMyselfLeaveOthers(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							string data = string.Format("{0}:{1}", (objsList[i] as GameClient).ClientData.RoleID, 1);
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 127);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
								break;
							}
						}
					}
				}
			}
		}

		public void NotifyMyselfLeaveMonsters(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is Monster)
					{
						if (!this.NotifyMyselfLeaveMonsterByID(sl, pool, client, (objsList[i] as Monster).RoleID))
						{
							break;
						}
					}
				}
			}
		}

		public bool NotifyMyselfLeaveMonsterByID(SocketListener sl, TCPOutPacketPool pool, GameClient client, int monsterID)
		{
			string data = string.Format("{0}:{1}", monsterID, 2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 127);
			return sl.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		public void JugeSpriteDead(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.CurrentLifeV <= 0)
			{
				GameManager.SystemServerEvents.AddEvent(string.Format("角色强制死亡, roleID={0}({1}), Life={2}", client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.CurrentLifeV), EventLevels.Debug);
				this.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, -1, client.ClientData.RoleID, 0, 0, 0.0, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
			}
		}

		public void NotifySelfLifeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			byte[] buffer = DataHelper.ObjectToBytes<SpriteLifeChangeData>(new SpriteLifeChangeData
			{
				roleID = client.ClientData.RoleID,
				lifeV = client.ClientData.LifeV,
				magicV = client.ClientData.MagicV,
				currentLifeV = client.ClientData.CurrentLifeV,
				currentMagicV = client.ClientData.CurrentMagicV
			});
			this.SendToClient(client, buffer, 164);
		}

		public bool NotifyOthersRelife(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int roleID, int x, int y, int direction, double lifeV, double magicV, int cmd, List<object> objsList, int force = 0)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, x, y, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (null == objsList)
			{
				result = true;
			}
			else
			{
				SpriteRelifeData spriteRelifeData = new SpriteRelifeData();
				spriteRelifeData.roleID = roleID;
				spriteRelifeData.direction = direction;
				spriteRelifeData.lifeV = lifeV;
				spriteRelifeData.magicV = magicV;
				spriteRelifeData.force = force;
				if (!GameManager.FlagEnableHideFlags)
				{
					spriteRelifeData.x = x;
					spriteRelifeData.y = y;
				}
				this.SendToClients(sl, pool, null, objsList, DataHelper.ObjectToBytes<SpriteRelifeData>(spriteRelifeData), cmd);
				result = true;
			}
			return result;
		}

		public void UserFullLife(GameClient client, string reason, bool allSend = true)
		{
			RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, reason);
			roleRelifeLog.hpModify = true;
			roleRelifeLog.mpModify = true;
			roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
			roleRelifeLog.oldMp = client.ClientData.CurrentMagicV;
			client.ClientData.CurrentLifeV = client.ClientData.LifeV;
			client.ClientData.CurrentMagicV = client.ClientData.MagicV;
			roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
			roleRelifeLog.newMp = client.ClientData.CurrentMagicV;
			SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
			GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, allSend, false, 7);
		}

		public void NotifyOthersLifeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client, bool allSend = true, bool resetMax = false, int flags = 7)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				client.ClientData.LifeV = (int)RoleAlgorithm.GetMaxLifeV(client);
				client.ClientData.MagicV = (int)RoleAlgorithm.GetMaxMagicV(client);
				if (!resetMax)
				{
					client.ClientData.CurrentLifeV = Global.GMin(client.ClientData.CurrentLifeV, client.ClientData.LifeV);
					client.ClientData.CurrentMagicV = Global.GMin(client.ClientData.CurrentMagicV, client.ClientData.MagicV);
					client.ClientData.CurrentArmorV = Global.GMin(client.ClientData.CurrentArmorV, client.ClientData.ArmorV);
				}
				else
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					client.ClientData.CurrentArmorV = client.ClientData.ArmorV;
				}
				SpriteLifeChangeData spriteLifeChangeData = new SpriteLifeChangeData();
				spriteLifeChangeData.roleID = client.ClientData.RoleID;
				if ((flags & 1) != 0)
				{
					spriteLifeChangeData.lifeV = client.ClientData.LifeV;
					spriteLifeChangeData.currentLifeV = client.ClientData.CurrentLifeV;
				}
				if ((flags & 2) != 0)
				{
					spriteLifeChangeData.magicV = client.ClientData.MagicV;
					spriteLifeChangeData.currentMagicV = client.ClientData.CurrentMagicV;
				}
				if ((flags & 4) != 0)
				{
					spriteLifeChangeData.ArmorV = (long)client.ClientData.ArmorV;
					spriteLifeChangeData.currentArmorV = (long)client.ClientData.CurrentArmorV;
				}
				byte[] array = DataHelper.ObjectToBytes<SpriteLifeChangeData>(spriteLifeChangeData);
				if (!allSend)
				{
					if (null != client)
					{
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 164);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
				else
				{
					List<object> all9Clients = Global.GetAll9Clients(client);
					if (null != all9Clients)
					{
						for (int i = all9Clients.Count - 1; i >= 0; i--)
						{
							GameClient gameClient = all9Clients[i] as GameClient;
							if (gameClient != null && gameClient != client)
							{
								if (gameClient.ClientData.ChangeLifeCount >= Data.OpChangeLifeCount && gameClient.ClientData.CurrentLifeV * 8 < gameClient.ClientData.LifeV)
								{
									all9Clients.RemoveAt(i);
								}
							}
						}
						this.SendToClients(sl, pool, null, all9Clients, array, 164);
					}
				}
			}
		}

		public void NotifyOthersGoBack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toPosX = -1, int toPosY = -1, int direction = -1)
		{
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							client.ClientData.MapCode,
							toPosX,
							toPosY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastChangeMapTicks = TimeUtil.NOW();
			int defaultBirthPosX = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosX;
			int defaultBirthPosY = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosY;
			int birthRadius = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].BirthRadius;
			int num = toPosX;
			int num2 = toPosY;
			if (-1 == num || -1 == num2)
			{
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
				num = (int)mapPoint.X;
				num2 = (int)mapPoint.Y;
			}
			if (direction >= 0)
			{
				client.ClientData.RoleDirection = direction;
			}
			GameManager.ClientMgr.ChangePosition(sl, pool, client, num, num2, direction, 159, 0);
		}

		public void NotifyOthersChangeEquip(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int refreshNow, WingData usingWinData = null)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				ChangeEquipData instance = new ChangeEquipData
				{
					RoleID = client.ClientData.RoleID,
					EquipGoodsData = goodsData,
					UsingWinData = usingWinData
				};
				byte[] bytesData = DataHelper.ObjectToBytes<ChangeEquipData>(instance);
				this.SendToClients(sl, pool, null, all9Clients, bytesData, 137);
			}
		}

		public void NotifyOthersRebornEquipChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowEquip);
				GameManager.ClientMgr.SendToClients(sl, pool, null, all9Clients, strCmd, 2052);
			}
		}

		public void NotifyOthersRebornModelChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.RebornShowModel);
				GameManager.ClientMgr.SendToClients(sl, pool, null, all9Clients, strCmd, 2061);
			}
		}

		public void NotifyOthersPKModeChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.PKMode);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 149);
			}
		}

		public void Logout(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			try
			{
				GameManager.systemGMCommands.OnClientLogout(client);
				GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutEventObject(client));
				client.TimedActionMgr.RemoveItem(0);
				Global.SystemKillSummonMonster(client, -1);
				ChengJiuManager.SaveKilledMonsterNumToDB(client, true);
				OnePieceManager.getInstance().HandleRoleLogout(client);
				if (null != client.ClientData.WanMoTaSweeping)
				{
					client.ClientData.WanMoTaSweeping.StopSweeping();
				}
				Global.ProcessDBCmdByTicks(client, true);
				Global.ProcessDBSkillCmdByTicks(client, true);
				Global.ProcessDBRoleParamCmdByTicks(client, true);
				Global.ProcessDBEquipStrongCmdByTicks(client, true);
				Global.UpdateAllDBBufferData(client);
				Global.UpdateHuoDongDBCommand(pool, client);
				GameManager.BattleMgr.LeaveBattleMap(client, true);
				GameManager.ArenaBattleMgr.LeaveArenaBattleMap(client);
				SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
				KuaFuManager.getInstance().OnLeaveScene(client, mapSceneType, false);
				KuaFuManager.getInstance().OnLogout(client);
				if (!client.ClientSocket.IsKuaFuLogin && client.ClientData.SaleGoodsDataList.Count > 0)
				{
					SaleRoleManager.RemoveSaleRoleItem(client.ClientData.RoleID);
					SaleGoodsManager.RemoveSaleGoodsItems(client);
					this.SaleGoodsToOfflineSale(client);
				}
				long nowTicks = TimeUtil.NOW();
				Global.ResetMeditateTime(client, nowTicks, false);
				GameManager.DBCmdMgr.AddDBCmd(10032, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TotalOnlineSecs, client.ClientData.AntiAddictionSecs), null, client.ServerId);
				this.RemoveClient(client);
				if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
				{
					GameManager.BloodCastleCopySceneMgr.LogOutWhenInBloodCastleCopyScene(client);
				}
				if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
				{
					GameManager.DaimonSquareCopySceneMgr.LogOutWhenInDaimonSquareCopyMap(client);
				}
				if (client.ClientData.IsFlashPlayer == 1)
				{
					Global.ProcessLogOutWhenINFreshPlayerStatus(client);
				}
				if (client.ClientData.bIsInAngelTempleMap)
				{
					GameManager.AngelTempleMgr.LeaveAngelTempleScene(client, true);
				}
				List<object> all9GridObjects = Global.GetAll9GridObjects(client);
				Global.GameClientHandleOldObjs(client, all9GridObjects);
				client.ClearVisibleObjects(true);
				Global.ClearCopyMap(client, true);
				GameManager.GoodsPackMgr.UnLockGoodsPackItem(client);
				Global.QuitFromTeam(client);
				if (client.ClientData.DJRoomID > 0)
				{
					if (client.ClientData.DJRoomTeamID > 0)
					{
						if (GameManager.ClientMgr.DestroyDianJiangRoom(sl, pool, client) < 0)
						{
							GameManager.ClientMgr.LeaveDianJiangRoom(sl, pool, client);
						}
						if (MapTypes.DianJiangCopy == Global.GetMapType(client.ClientData.MapCode))
						{
							GameManager.DJRoomMgr.SetRoomRolesDataRoleState(client.ClientData.DJRoomID, client.ClientData.RoleID, 2);
						}
					}
					else
					{
						this.ViewerLeaveDianJiangRoom(sl, pool, client);
					}
					client.ClientData.DJRoomID = -1;
					client.ClientData.DJRoomTeamID = -1;
					client.ClientData.HideSelf = 0;
				}
				RoleName2IDs.RemoveRoleName(Global.FormatRoleName(client, client.ClientData.RoleName));
				this.ProcessExchangeData(sl, pool, client);
				if (client.ClientData.CurrentLifeV <= 0)
				{
					client.ClientData.MapCode = -1;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
					client.ClientData.ReportPosTicks = 0L;
				}
				if (mapSceneType == 21)
				{
					client.ClientData.MapCode = -1;
					client.ClientData.PosX = -1;
					client.ClientData.PosY = -1;
					client.ClientData.ReportPosTicks = 0L;
				}
				if (Global.CanRecordPos(client))
				{
					GameManager.DBCmdMgr.AddDBCmd(10001, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.MapCode,
						client.ClientData.RoleDirection,
						client.ClientData.PosX,
						client.ClientData.PosY
					}), null, client.ServerId);
				}
				GameManager.DBCmdMgr.AddDBCmd(10017, string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					GameManager.ServerLineID,
					Global.GetSocketRemoteIP(client, false),
					client.ClientData.OnlineActiveVal,
					TimeUtil.NOW()
				}), null, client.ServerId);
				EventLogManager.AddRoleLogoutEvent(client);
				Global.UpdateRoleParamsInfo(client);
				Global.UpdateRoleDayActivityInfo(client, 0);
				DailyActiveManager.SaveRoleDailyActiveData(client);
				ChengJiuManager.SaveRoleChengJiuData(client);
				MarryLogic.ApplyLogoutClear(client);
				RobotTaskValidator.getInstance().RobotDataReset(client);
				GameManager.loginWaitLogic.AddToAllow(client.strUserID, GameManager.loginWaitLogic.GetConfig(LoginWaitLogic.UserType.Normal, LoginWaitLogic.ConfigType.LogouAllowMSeconds));
				SecondPasswordManager.OnUsrLogout(client.strUserID);
				SingletonTemplate<SpeedUpTickCheck>.Instance().OnLogout(client);
				SingletonTemplate<CoupleArenaManager>.Instance().OnClientLogout(client);
				try
				{
					string ip = RobotTaskValidator.getInstance().GetIp(client);
					string text = string.Format("logout server={0} account={1} player={2} dev_id={3} exp={4}", new object[]
					{
						GameManager.ServerId,
						client.strUserID,
						client.ClientData.RoleID,
						string.IsNullOrEmpty(client.deviceID) ? "" : client.deviceID,
						ip
					});
					LogManager.WriteLog(5, text, null, true);
				}
				catch
				{
				}
				client.LogoutState = true;
				GlobalEventSource.getInstance().fireEvent(new PlayerLogoutFinishEventObject(client));
				GlobalEventSource.getInstance().fireEvent(new PlayerLeaveFuBenEventObject(client));
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			finally
			{
				client.ClientData.SceneType = 9999;
				client.ClientData.SceneMapCode = 0;
			}
		}

		private void SaleGoodsToOfflineSale(GameClient client)
		{
			if (client.ClientSocket.ClientKuaFuServerLoginData.RoleId <= 0 && !client.CheckCheatData.IsKickedRole)
			{
				LiXianBaiTanManager.AddLiXianSaleGoodsItems(client, -1);
			}
		}

		private void ProcessFakeRoleForLiXianGuaJi(GameClient client)
		{
			if (client.ClientSocket.ClientKuaFuServerLoginData.RoleId <= 0 && !client.CheckCheatData.IsKickedRole)
			{
				int fakeRoleID = 0;
				if (GameManager.FlagLiXianGuaJi > 0)
				{
					fakeRoleID = FakeRoleManager.ProcessNewFakeRole(client.ClientData, client.ClientData.MapCode, FakeRoleTypes.LiXianGuaJi, -1, client.ClientData.PosX, client.ClientData.PosY, 0);
				}
				LiXianGuaJiManager.AddLiXianGuaJiRole(client, fakeRoleID);
			}
		}

		public void NotifyUpdateTask(SocketListener sl, TCPOutPacketPool pool, GameClient client, int dbID, int taskID, int taskVal1, int taskVal2, int taskFocus, long chengjiuValue)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				dbID,
				taskID,
				taskVal1,
				taskVal2,
				taskFocus,
				chengjiuValue
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 139);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyUpdateNPCTaskSate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int npcID, int state)
		{
			string data = string.Format("{0}:{1}", npcID, state);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 151);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyNPCTaskStateList(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<NPCTaskState> npcTaskStatList)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<NPCTaskState>>(npcTaskStatList, pool, 152);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool GiveFirstTask(TCPManager tcpMgr, TMSKSocket socket, TCPClientPool tcpClientPool, TCPOutPacketPool pool, TCPRandKey tcpRandKey, GameClient client, bool bNeedTakeStartTask)
		{
			bool result;
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，无法给与新手任务", new object[0]), null, true);
				result = false;
			}
			else
			{
				int roleID = client.ClientData.RoleID;
				try
				{
					if (Global.GetTaskData(client, MagicSwordData.InitTaskID) == null && GameManager.MagicSwordMgr.IsFirstLoginMagicSword(client, MagicSwordData.InitChangeLifeCount))
					{
						int num = GameManager.ClientMgr.AutoCompletionTaskByTaskID(tcpMgr, tcpClientPool, pool, tcpRandKey, client, MagicSwordData.InitPrevTaskID);
						if (num != 0)
						{
							LogManager.WriteLog(2, string.Format("魔剑士任务初始化失败，无法创建魔剑士, RoleID={0}", roleID), null, true);
							return false;
						}
						client.ClientData.MainTaskID = MagicSwordData.InitPrevTaskID;
						client.ClientData.MapCode = MagicSwordData.InitMapID;
						TCPOutPacket tcpoutPacket = null;
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, roleID, MagicSwordData.InitTaskID, MagicSwordData.InitTaskNpcID, out tcpoutPacket);
					}
					else if (Global.GetTaskData(client, SummonerData.InitTaskID) == null && GameManager.SummonerMgr.IsFirstLoginSummoner(client, SummonerData.InitChangeLifeCount))
					{
						int num = GameManager.ClientMgr.AutoCompletionTaskByTaskID(tcpMgr, tcpClientPool, pool, tcpRandKey, client, SummonerData.InitPrevTaskID);
						if (num != 0)
						{
							LogManager.WriteLog(2, string.Format("召唤师任务初始化失败，无法创建召唤师, RoleID={0}", roleID), null, true);
							return false;
						}
						client.ClientData.MainTaskID = SummonerData.InitPrevTaskID;
						client.ClientData.MapCode = SummonerData.InitMapID;
						TCPOutPacket tcpoutPacket = null;
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, roleID, SummonerData.InitTaskID, SummonerData.InitTaskNpcID, out tcpoutPacket);
					}
					else if (bNeedTakeStartTask && Global.GetTaskData(client, 1000) == null && !GameManager.MagicSwordMgr.IsMagicSword(client))
					{
						client.ClientData.MainTaskID = 106;
						TCPOutPacket tcpoutPacket = null;
						Global.AddOldTask(client, 106);
						Global.TakeNewTask(tcpMgr, socket, tcpClientPool, tcpRandKey, pool, 125, client, roleID, 1000, 60900, out tcpoutPacket);
					}
					return true;
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result = false;
			}
			return result;
		}

		public EquipPropsData GetEquipPropsStr(GameClient client)
		{
			client.propsCacheModule.ResetAllProps();
			AdvanceBufferPropsMgr.DoSpriteBuffers(client);
			double minAttackV = RoleAlgorithm.GetMinAttackV(client);
			double maxAttackV = RoleAlgorithm.GetMaxAttackV(client);
			double minADefenseV = RoleAlgorithm.GetMinADefenseV(client);
			double maxADefenseV = RoleAlgorithm.GetMaxADefenseV(client);
			double minMagicAttackV = RoleAlgorithm.GetMinMagicAttackV(client);
			double maxMagicAttackV = RoleAlgorithm.GetMaxMagicAttackV(client);
			double minMDefenseV = RoleAlgorithm.GetMinMDefenseV(client);
			double maxMDefenseV = RoleAlgorithm.GetMaxMDefenseV(client);
			double hitV = RoleAlgorithm.GetHitV(client);
			double dodgeV = RoleAlgorithm.GetDodgeV(client);
			double addAttackInjureValue = RoleAlgorithm.GetAddAttackInjureValue(client);
			double decreaseInjureValue = RoleAlgorithm.GetDecreaseInjureValue(client);
			double maxLifeV = RoleAlgorithm.GetMaxLifeV(client);
			double maxMagicV = RoleAlgorithm.GetMaxMagicV(client);
			double lifeStealV = RoleAlgorithm.GetLifeStealV(client);
			double num = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Fire);
			double num2 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Water);
			double num3 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Lightning);
			double num4 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Soil);
			double num5 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Ice);
			double num6 = (double)GameManager.ElementsAttackMgr.GetElementAttack(client, EElementDamageType.EEDT_Wind);
			double extProp = RoleAlgorithm.GetExtProp(client, 122);
			double extProp2 = RoleAlgorithm.GetExtProp(client, 123);
			double extProp3 = RoleAlgorithm.GetExtProp(client, 129);
			double extProp4 = RoleAlgorithm.GetExtProp(client, 130);
			double extProp5 = RoleAlgorithm.GetExtProp(client, 136);
			double extProp6 = RoleAlgorithm.GetExtProp(client, 137);
			double extProp7 = RoleAlgorithm.GetExtProp(client, 143);
			double extProp8 = RoleAlgorithm.GetExtProp(client, 144);
			double extProp9 = RoleAlgorithm.GetExtProp(client, 150);
			double extProp10 = RoleAlgorithm.GetExtProp(client, 151);
			CombatForceInfo combatForceInfo = Data.CombatForceDataInfo[1];
			int num7 = (int)RoleAlgorithm.GetExtProp(client, 119);
			double extProp11 = RoleAlgorithm.GetExtProp(client, 101);
			if (combatForceInfo != null)
			{
				double num8 = (minAttackV / combatForceInfo.MinPhysicsAttackModulus + maxAttackV / combatForceInfo.MaxPhysicsAttackModulus) / 2.0 + (minADefenseV / combatForceInfo.MinPhysicsDefenseModulus + maxADefenseV / combatForceInfo.MaxPhysicsDefenseModulus) / 2.0 + (minMagicAttackV / combatForceInfo.MinMagicAttackModulus + maxMagicAttackV / combatForceInfo.MaxMagicAttackModulus) / 2.0 + (minMDefenseV / combatForceInfo.MinMagicDefenseModulus + maxMDefenseV / combatForceInfo.MaxMagicDefenseModulus) / 2.0 + addAttackInjureValue / combatForceInfo.AddAttackInjureModulus + decreaseInjureValue / combatForceInfo.DecreaseInjureModulus + hitV / combatForceInfo.HitValueModulus + dodgeV / combatForceInfo.DodgeModulus + maxLifeV / combatForceInfo.MaxHPModulus + maxMagicV / combatForceInfo.MaxMPModulus + lifeStealV / combatForceInfo.LifeStealModulus + (double)num7 / combatForceInfo.ArmorMax;
				num8 += num / combatForceInfo.FireAttack + num2 / combatForceInfo.WaterAttack + num3 / combatForceInfo.LightningAttack + num4 / combatForceInfo.SoilAttack + num5 / combatForceInfo.IceAttack + num6 / combatForceInfo.WindAttack;
				num8 += extProp / combatForceInfo.HolyAttack + extProp2 / combatForceInfo.HolyDefense + extProp3 / combatForceInfo.ShadowAttack + extProp4 / combatForceInfo.ShadowDefense + extProp5 / combatForceInfo.NatureAttack + extProp6 / combatForceInfo.NatureDefense + extProp7 / combatForceInfo.ChaosAttack + extProp8 / combatForceInfo.ChaosDefense + extProp9 / combatForceInfo.IncubusAttack + extProp10 / combatForceInfo.IncubusDefense;
				client.ClientData.CombatForce = (int)num8;
				HuodongCachingMgr.ProcessCombatGift(client, false);
			}
			RebornManager.getInstance().CalculateCombatForce(client);
			int timeAddProp = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPStrength);
			int timeAddProp2 = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPIntelligsence);
			int timeAddProp3 = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPDexterity);
			int timeAddProp4 = DBRoleBufferManager.GetTimeAddProp(client, BufferItemTypes.ADDTEMPConstitution);
			int num9 = (int)RoleAlgorithm.GetStrength(client, false);
			int num10 = (int)RoleAlgorithm.GetIntelligence(client, false);
			int num11 = (int)RoleAlgorithm.GetDexterity(client, false);
			int num12 = (int)RoleAlgorithm.GetConstitution(client, false);
			int num13 = num9 + num10 + num11 + num12;
			EquipPropsData equipPropsData = new EquipPropsData
			{
				RoleID = client.ClientData.RoleID,
				Strength = (double)(num9 + timeAddProp),
				Intelligence = (double)(num10 + timeAddProp2),
				Dexterity = (double)(num11 + timeAddProp3),
				Constitution = (double)(num12 + timeAddProp4),
				MinAttack = minAttackV,
				MaxAttack = maxAttackV,
				MinDefense = minADefenseV,
				MaxDefense = maxADefenseV,
				MagicSkillIncrease = RoleAlgorithm.GetMagicSkillIncrease(client),
				MinMAttack = minMagicAttackV,
				MaxMAttack = maxMagicAttackV,
				MinMDefense = minMDefenseV,
				MaxMDefense = maxMDefenseV,
				PhySkillIncrease = RoleAlgorithm.GetPhySkillIncrease(client),
				MaxHP = maxLifeV,
				MaxMP = maxMagicV,
				AttackSpeed = RoleAlgorithm.GetAttackSpeed(client),
				Hit = hitV,
				Dodge = dodgeV,
				TotalPropPoint = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint"),
				ChangeLifeCount = client.ClientData.ChangeLifeCount,
				CombatForce = client.ClientData.CombatForce,
				TEMPStrength = Global.GetRoleParamsInt32FromDB(client, "PropStrengthChangeless"),
				TEMPIntelligsence = Global.GetRoleParamsInt32FromDB(client, "PropIntelligenceChangeless"),
				TEMPDexterity = Global.GetRoleParamsInt32FromDB(client, "PropDexterityChangeless"),
				TEMPConstitution = Global.GetRoleParamsInt32FromDB(client, "PropConstitutionChangeless"),
				Toughness = extProp11,
				ArmorMax = num7,
				RebornCombatForce = client.ClientData.RebornCombatForce
			};
			equipPropsData.TotalPropPoint += timeAddProp + timeAddProp2 + timeAddProp3 + timeAddProp4;
			equipPropsData.TotalPropPoint += (int)client.ClientData.PropsCacheManager.GetBaseProp(0) + (int)client.ClientData.PropsCacheManager.GetBaseProp(1) + (int)client.ClientData.PropsCacheManager.GetBaseProp(2) + (int)client.ClientData.PropsCacheManager.GetBaseProp(3);
			Global.CalcAttackType(client);
			if (client.ClientData.Occupation == 3)
			{
				int num14 = (client.ClientData.AttackType == 0) ? 1 : 2;
				if (num14 != client.ClientData.SubOccupation)
				{
					client.ClientData.SubOccupation = num14;
					Global.SaveRoleParamsInt32ValueToDB(client, "10213", num14, true);
					string cmdData = string.Format("{0}:{1}", client.ClientData.RoleID, num14);
					client.sendOthersCmd(1270, cmdData);
				}
				client.ClientData.OccupationIndex = client.ClientData.Occupation + num14 - 1;
			}
			else
			{
				client.ClientData.OccupationIndex = client.ClientData.Occupation;
			}
			return equipPropsData;
		}

		public void NotifyUpdateEquipProps(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				EquipPropsData equipPropsStr = this.GetEquipPropsStr(client);
				byte[] array = DataHelper.ObjectToBytes<EquipPropsData>(equipPropsStr);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 126);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
				if (client.ClientData.CombatForce != client.ClientData.LastNotifyCombatForce)
				{
					client.ClientData.LastNotifyCombatForce = client.ClientData.CombatForce;
					this.NotifyTeamCHGZhanLi(sl, pool, client);
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.CombatChange));
				}
			}
		}

		public void NotifyUpdateEscapeBattleProps(SocketListener sl, TCPOutPacketPool pool, GameClient client, EscapeBattlePropNotify ebProp)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				byte[] array = DataHelper.ObjectToBytes<EscapeBattlePropNotify>(ebProp);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 2121);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyUpdateEquipProps(GameClient client)
		{
			this.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
		}

		public void NotifyUpdateWeights(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
		}

		public void NotifyUpdateEquipProps(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			EquipPropsData equipPropsStr = this.GetEquipPropsStr(otherClient);
			byte[] array = DataHelper.ObjectToBytes<EquipPropsData>(equipPropsStr);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 126);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void AddSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double lifeV, string reason)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				if (c.ClientData.CurrentLifeV < c.ClientData.LifeV)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, reason);
					roleRelifeLog.hpModify = true;
					roleRelifeLog.oldHp = c.ClientData.CurrentLifeV;
					c.ClientData.CurrentLifeV = (int)Global.GMin((double)c.ClientData.LifeV, (double)c.ClientData.CurrentLifeV + lifeV);
					roleRelifeLog.newHp = c.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
					List<object> all9Clients = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public void SubSpriteLifeV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double lifeV)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				c.ClientData.CurrentLifeV = (int)Global.GMax(0.0, (double)c.ClientData.CurrentLifeV - lifeV);
				List<object> all9Clients = Global.GetAll9Clients(c);
				GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, all9Clients, 0);
			}
		}

		public void AddSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double magicV, string reason)
		{
			if (c.ClientData.CurrentLifeV > 0)
			{
				if (c.ClientData.CurrentMagicV < c.ClientData.MagicV)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, reason);
					roleRelifeLog.mpModify = true;
					roleRelifeLog.oldMp = c.ClientData.CurrentMagicV;
					c.ClientData.CurrentMagicV = (int)Global.GMin((double)c.ClientData.MagicV, (double)c.ClientData.CurrentMagicV + magicV);
					roleRelifeLog.newMp = c.ClientData.CurrentMagicV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
					List<object> all9Clients = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public void SubSpriteMagicV(SocketListener sl, TCPOutPacketPool pool, GameClient c, double magicV)
		{
			if (c.ClientData.IsFlashPlayer != 1 || c.ClientData.MapCode != 6090)
			{
				if (c.ClientData.CurrentLifeV > 0)
				{
					c.ClientData.CurrentMagicV = (int)Global.GMax(0.0, (double)c.ClientData.CurrentMagicV - magicV);
					List<object> all9Clients = Global.GetAll9Clients(c);
					GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, all9Clients, 0);
				}
			}
		}

		public void NotifyPetCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int petType, int extTag1, string extTag2, List<object> objsList)
		{
			string text = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				petType,
				extTag1,
				extTag2
			});
			if (null == objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, 184);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, text, 184);
			}
		}

		public void RemoveRolePet(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, bool notifySelf)
		{
			if (client.ClientData.PetDbID > 0 && client.ClientData.PetRoleID > 0)
			{
				PetData petDataByDbID = Global.GetPetDataByDbID(client, client.ClientData.PetDbID);
				if (null != petDataByDbID)
				{
					GameManager.ClientMgr.NotifyPetCmd(sl, pool, client, 0, 2, client.ClientData.PetRoleID, "", objsList);
				}
			}
		}

		public void NotifySelfPetShow(GameClient client)
		{
			if (client.ClientData.PetDbID > 0)
			{
				PetData petDataByDbID = Global.GetPetDataByDbID(client, client.ClientData.PetDbID);
				if (null != petDataByDbID)
				{
					if (client.ClientData.PetRoleID <= 0)
					{
						if (!Global.IsPetDead(petDataByDbID))
						{
							client.ClientData.PetRoleID = (int)GameManager.PetIDMgr.GetNewID();
							Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, 150);
							client.ClientData.PetPosX = (int)mapPoint.X;
							client.ClientData.PetPosY = (int)mapPoint.Y;
							client.ClientData.ReportPetPosTicks = 0L;
						}
					}
					if (client.ClientData.PetRoleID > 0)
					{
						double directionByTan = Global.GetDirectionByTan((double)client.ClientData.PosX, (double)client.ClientData.PosY, (double)client.ClientData.PetPosX, (double)client.ClientData.PetPosY);
						string extTag = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
						{
							client.ClientData.PetRoleID,
							petDataByDbID.PetName,
							petDataByDbID.Level,
							petDataByDbID.PetID,
							client.ClientData.PetPosX,
							client.ClientData.PetPosY,
							(int)directionByTan
						});
						GameManager.ClientMgr.NotifyPetCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, petDataByDbID.DbID, extTag, null);
					}
				}
			}
		}

		public void NotifyOthersMyPetHide(GameClient client)
		{
			if (client.ClientData.PetRoleID > 0)
			{
				List<object> all9Clients = Global.GetAll9Clients(client);
				GameManager.ClientMgr.NotifyPetCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 2, client.ClientData.PetRoleID, "", all9Clients);
			}
		}

		public void NotifyMySelfOnlineOtherPet(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (client.ClientData.RoleID != otherClient.ClientData.RoleID)
				{
					if (otherClient.ClientData.PetDbID > 0 && otherClient.ClientData.PetRoleID > 0)
					{
						PetData petDataByDbID = Global.GetPetDataByDbID(otherClient, otherClient.ClientData.PetDbID);
						if (null != petDataByDbID)
						{
							Point point = new Point((double)otherClient.ClientData.PetPosX, (double)otherClient.ClientData.PetPosY);
							double directionByTan = Global.GetDirectionByTan((double)otherClient.ClientData.PosX, (double)otherClient.ClientData.PosY, point.X, point.Y);
							string text = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
							{
								otherClient.ClientData.PetRoleID,
								petDataByDbID.PetName,
								petDataByDbID.Level,
								petDataByDbID.PetID,
								(int)point.X,
								(int)point.Y,
								(int)directionByTan
							});
							string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
							{
								0,
								otherClient.ClientData.RoleID,
								1,
								otherClient.ClientData.PetRoleID,
								text
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 184);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		public void NotifyMySelfOnlineOtherPets(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.PetDbID > 0 && (objsList[i] as GameClient).ClientData.PetRoleID > 0)
							{
								PetData petDataByDbID = Global.GetPetDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.PetDbID);
								if (null != petDataByDbID)
								{
									Point point = new Point((double)(objsList[i] as GameClient).ClientData.PetPosX, (double)(objsList[i] as GameClient).ClientData.PetPosY);
									double directionByTan = Global.GetDirectionByTan((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY, point.X, point.Y);
									string text = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
									{
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petDataByDbID.PetName,
										petDataByDbID.Level,
										petDataByDbID.PetID,
										(int)point.X,
										(int)point.Y,
										(int)directionByTan
									});
									string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										1,
										(objsList[i] as GameClient).ClientData.PetRoleID,
										text
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 184);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifySelfPetsOfflines(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.PetDbID > 0 && (objsList[i] as GameClient).ClientData.PetRoleID > 0)
							{
								PetData petDataByDbID = Global.GetPetDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.PetDbID);
								if (null != petDataByDbID)
								{
									Point point = new Point((double)(objsList[i] as GameClient).ClientData.PetPosX, (double)(objsList[i] as GameClient).ClientData.PetPosY);
									double directionByTan = Global.GetDirectionByTan((double)(objsList[i] as GameClient).ClientData.PosX, (double)(objsList[i] as GameClient).ClientData.PosY, point.X, point.Y);
									string text = string.Format("{0}${1}${2}${3}${4}${5}${6}", new object[]
									{
										(objsList[i] as GameClient).ClientData.PetRoleID,
										petDataByDbID.PetName,
										petDataByDbID.Level,
										petDataByDbID.PetID,
										(int)point.X,
										(int)point.Y,
										(int)directionByTan
									});
									string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										2,
										(objsList[i] as GameClient).ClientData.PetRoleID,
										text
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 184);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifyHorseCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int horseType, int horseDbID, int horseID, int horseBodyID, List<object> objsList)
		{
			string text = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				status,
				client.ClientData.RoleID,
				horseType,
				horseDbID,
				horseID,
				horseBodyID
			});
			if (null == objsList)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, text, 183);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				this.SendToClients(sl, pool, null, objsList, text, 183);
			}
		}

		public void NotifySelfOnHorse(GameClient client)
		{
			if (client.ClientData.HorseDbID > 0)
			{
				HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, client.ClientData.HorseDbID);
				if (null != horseDataByDbID)
				{
					client.ClientData.RoleHorseJiFen = Global.CalcHorsePropsJiFen(horseDataByDbID);
					GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.BodyID, null);
				}
			}
		}

		public void NotifySelfOtherHorse(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient)
		{
			if (null != otherClient)
			{
				if (client.ClientData.RoleID != otherClient.ClientData.RoleID)
				{
					if (otherClient.ClientData.HorseDbID > 0)
					{
						HorseData horseDataByDbID = Global.GetHorseDataByDbID(otherClient, otherClient.ClientData.HorseDbID);
						if (null != horseDataByDbID)
						{
							string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
							{
								0,
								otherClient.ClientData.RoleID,
								1,
								horseDataByDbID.DbID,
								horseDataByDbID.HorseID,
								horseDataByDbID.BodyID
							});
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 183);
							if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		public void NotifySelfOthersHorse(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is GameClient)
					{
						if (client.ClientData.RoleID != (objsList[i] as GameClient).ClientData.RoleID)
						{
							if ((objsList[i] as GameClient).ClientData.HorseDbID > 0)
							{
								HorseData horseDataByDbID = Global.GetHorseDataByDbID(objsList[i] as GameClient, (objsList[i] as GameClient).ClientData.HorseDbID);
								if (null != horseDataByDbID)
								{
									string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
									{
										0,
										(objsList[i] as GameClient).ClientData.RoleID,
										1,
										horseDataByDbID.DbID,
										horseDataByDbID.HorseID,
										horseDataByDbID.BodyID
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 183);
									if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
									{
										break;
									}
								}
							}
						}
					}
				}
			}
		}

		public void JugeTempHorseID(GameClient client)
		{
			if (client.ClientData.StartTempHorseIDTicks > 0L)
			{
				if (client.ClientData.TempHorseID > 0)
				{
					long num = TimeUtil.NOW();
					if (num - client.ClientData.StartTempHorseIDTicks >= 180000L)
					{
						int tempHorseID = client.ClientData.TempHorseID;
						client.ClientData.StartTempHorseIDTicks = 0L;
						client.ClientData.TempHorseID = 0;
						if (client.ClientData.HorseDbID > 0)
						{
							HorseData horseDataByDbID = Global.GetHorseDataByDbID(client, client.ClientData.HorseDbID);
							if (null != horseDataByDbID)
							{
								string horseNameByID = Global.GetHorseNameByID(tempHorseID);
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(52, new object[0]), new object[]
								{
									horseNameByID
								}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
								List<object> all9Clients = Global.GetAll9Clients(client);
								if (null != all9Clients)
								{
									GameManager.ClientMgr.NotifyHorseCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 0, 1, horseDataByDbID.DbID, horseDataByDbID.HorseID, horseDataByDbID.BodyID, all9Clients);
								}
							}
						}
					}
				}
			}
		}

		public void NotifyJingMaiListCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			TCPOutPacket tcpOutPacket = null;
			List<JingMaiData> jingMaiDataList = client.ClientData.JingMaiDataList;
			if (null != jingMaiDataList)
			{
				lock (jingMaiDataList)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 206);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 206);
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyJingMaiInfoCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			TCPOutPacket tcpOutPacket = null;
			Dictionary<string, int> jingMaiPropsDict = client.ClientData.JingMaiPropsDict;
			if (null != jingMaiPropsDict)
			{
				lock (jingMaiPropsDict)
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, int>>(jingMaiPropsDict, pool, 218);
				}
			}
			else
			{
				tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<string, int>>(jingMaiPropsDict, pool, 218);
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyOtherJingMaiListCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID)
		{
			TCPOutPacket tcpOutPacket = null;
			GameClient gameClient = this.FindClient(otherRoleID);
			if (null != gameClient)
			{
				List<JingMaiData> jingMaiDataList = gameClient.ClientData.JingMaiDataList;
				if (null != jingMaiDataList)
				{
					lock (jingMaiDataList)
					{
						tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 208);
					}
				}
				else
				{
					tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<JingMaiData>>(jingMaiDataList, pool, 208);
				}
			}
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyEndChongXueCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}", client.ClientData.RoleID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 280);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyJingMaiResult(GameClient client, int retCode, int jingMaiID, int jingMaiLevel)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				retCode,
				client.ClientData.JingMaiBodyLevel,
				jingMaiID,
				jingMaiLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 207);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool RemoveOldestEnemy(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client)
		{
			int friendCountByType = Global.GetFriendCountByType(client, 2);
			bool result;
			if (friendCountByType < 20)
			{
				result = true;
			}
			else
			{
				FriendData friendData = Global.FindFirstFriendDataByType(client, 2);
				result = (null == friendData || GameManager.ClientMgr.RemoveFriend(tcpMgr, tcpClientPool, pool, client, friendData.DbID));
			}
			return result;
		}

		public void AddToEnemyList(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int killedRoleID)
		{
			if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
			{
				GameClient gameClient = this.FindClient(killedRoleID);
				if (null != gameClient)
				{
					if (this.RemoveOldestEnemy(tcpMgr, tcpClientPool, pool, gameClient))
					{
						int dbID = -1;
						FriendData friendData = Global.FindFriendData(gameClient, client.ClientData.RoleID);
						if (null != friendData)
						{
							dbID = friendData.DbID;
						}
						int friendCountByType = Global.GetFriendCountByType(gameClient, 2);
						if (friendCountByType >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, gameClient, StringUtil.substitute(GLang.GetLang(53, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
						}
						else if (friendData == null || (friendData.FriendType != 0 && friendData.FriendType != 2))
						{
							this.AddFriend(tcpMgr, tcpClientPool, pool, gameClient, dbID, client.ClientData.RoleID, Global.FormatRoleName(client, client.ClientData.RoleName), 2);
						}
					}
				}
			}
		}

		public bool RemoveFriend(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID)
		{
			bool result = false;
			try
			{
				string s = string.Format("{0}:{1}", dbID, client.ClientData.RoleID);
				byte[] bytes = new UTF8Encoding().GetBytes(s);
				TCPOutPacket tcpoutPacket = null;
				TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, null, pool, 144, bytes, bytes.Length, out tcpoutPacket, client.ServerId);
				if (TCPProcessCmdResults.RESULT_FAILED != tcpprocessCmdResults)
				{
					string @string = new UTF8Encoding().GetString(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
					string[] array = @string.Split(new char[]
					{
						':'
					});
					if (array.Length == 3 && Convert.ToInt32(array[2]) >= 0)
					{
						Global.RemoveFriendData(client, dbID);
					}
					result = true;
				}
				if (!tcpMgr.MySocketListener.SendData(client.ClientSocket, tcpoutPacket, true))
				{
				}
				return result;
			}
			catch (Exception e)
			{
				DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
			}
			return result;
		}

		public bool AddFriend(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int otherRoleID, string otherRoleName, int friendType)
		{
			bool flag = false;
			bool result;
			if (client.ClientSocket.IsKuaFuLogin)
			{
				result = false;
			}
			else if (friendType == 2 && otherRoleID == client.ClientData.RoleID)
			{
				result = false;
			}
			else
			{
				try
				{
					FriendData friendData;
					if (otherRoleID > 0)
					{
						friendData = Global.FindFriendData(client, otherRoleID);
						if (null != friendData)
						{
							if (friendData.FriendType == friendType)
							{
								return flag;
							}
						}
					}
					int friendCountByType = Global.GetFriendCountByType(client, friendType);
					if (0 == friendType)
					{
						if (friendCountByType >= 50)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(54, new object[0]), new object[]
							{
								50
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return flag;
						}
					}
					else if (1 == friendType)
					{
						if (friendCountByType >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(55, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return flag;
						}
					}
					else if (2 == friendType)
					{
						if (friendCountByType >= 20)
						{
							GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, client, StringUtil.substitute(GLang.GetLang(53, new object[0]), new object[]
							{
								20
							}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
							return flag;
						}
					}
					string s = string.Format("{0}:{1}:{2}:{3}", new object[]
					{
						dbID,
						client.ClientData.RoleID,
						otherRoleName,
						friendType
					});
					byte[] bytes = new UTF8Encoding().GetBytes(s);
					TCPOutPacket tcpoutPacket = null;
					TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, null, pool, 143, bytes, bytes.Length, out tcpoutPacket, client.ServerId);
					if (null == tcpoutPacket)
					{
						return flag;
					}
					friendData = DataHelper.BytesToObject<FriendData>(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
					if (friendData != null && friendData.DbID >= 0)
					{
						flag = true;
						Global.RemoveFriendData(client, friendData.DbID);
						Global.AddFriendData(client, friendData);
						if (0 == friendType)
						{
							friendCountByType = Global.GetFriendCountByType(client, friendType);
							if (1 == friendCountByType)
							{
								ChengJiuManager.OnFirstAddFriend(client);
							}
							ProcessTask.ProcessRoleTaskVal(client, TaskTypes.FreindNum, friendCountByType);
						}
						GameClient gameClient = GameManager.ClientMgr.FindClient(friendData.OtherRoleID);
						if (null != gameClient)
						{
							if (friendData.FriendType == 0)
							{
								string lang = GLang.GetLang(56, new object[0]);
								GameManager.ClientMgr.NotifyImportantMsg(tcpMgr.MySocketListener, pool, gameClient, StringUtil.substitute(GLang.GetLang(59, new object[0]), new object[]
								{
									Global.FormatRoleName(client, client.ClientData.RoleName),
									lang
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.ErrAndBox, 0);
							}
						}
					}
					if (!tcpMgr.MySocketListener.SendData(client.ClientSocket, tcpoutPacket, true))
					{
					}
					return flag;
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result = flag;
			}
			return result;
		}

		public void NotifyDianJiangData(SocketListener sl, TCPOutPacketPool pool, DJRoomData roomData)
		{
			if (null != roomData)
			{
				byte[] array = null;
				lock (roomData)
				{
					array = DataHelper.ObjectToBytes<DJRoomData>(roomData);
				}
				if (array != null && array.Length > 0)
				{
					int num = 0;
					GameClient nextClient;
					while ((nextClient = this.GetNextClient(ref num, false)) != null)
					{
						if (nextClient.ClientData.ViewDJRoomDlg)
						{
							TCPOutPacket tcpoutPacket = pool.Pop();
							tcpoutPacket.PacketCmdID = 186;
							tcpoutPacket.FinalWriteData(array, 0, array.Length);
							if (!sl.SendData(nextClient.ClientSocket, tcpoutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		public void NotifyDJRoomRolesData(SocketListener sl, TCPOutPacketPool pool, DJRoomRolesData djRoomRolesData)
		{
			if (null != djRoomRolesData)
			{
				lock (djRoomRolesData)
				{
					byte[] array = DataHelper.ObjectToBytes<DJRoomRolesData>(djRoomRolesData);
					for (int i = 0; i < djRoomRolesData.Team1.Count; i++)
					{
						GameClient gameClient = this.FindClient(djRoomRolesData.Team1[i].RoleID);
						if (null != gameClient)
						{
							TCPOutPacket tcpoutPacket = pool.Pop();
							tcpoutPacket.PacketCmdID = 187;
							tcpoutPacket.FinalWriteData(array, 0, array.Length);
							if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, true))
							{
							}
						}
					}
					for (int i = 0; i < djRoomRolesData.Team2.Count; i++)
					{
						GameClient gameClient = this.FindClient(djRoomRolesData.Team2[i].RoleID);
						if (null != gameClient)
						{
							TCPOutPacket tcpoutPacket = pool.Pop();
							tcpoutPacket.PacketCmdID = 187;
							tcpoutPacket.FinalWriteData(array, 0, array.Length);
							if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, true))
							{
							}
						}
					}
				}
			}
		}

		public void NotifyDianJiangCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int djCmdType, int extTag1, string extTag2, bool allSend = false)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				djCmdType,
				extTag1,
				extTag2
			});
			if (!allSend)
			{
				TCPOutPacket tcpoutPacket = null;
				tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 188);
				if (!sl.SendData(client.ClientSocket, tcpoutPacket, true))
				{
				}
			}
			else
			{
				int num = 0;
				client = null;
				TCPOutPacket tcpoutPacket = null;
				try
				{
					while ((client = this.GetNextClient(ref num, false)) != null)
					{
						if (client.ClientData.ViewDJRoomDlg)
						{
							if (null == tcpoutPacket)
							{
								tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 188);
							}
							if (!sl.SendData(client.ClientSocket, tcpoutPacket, false))
							{
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpoutPacket);
				}
			}
		}

		public int DestroyDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djroomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djroomData)
				{
					result = -2;
				}
				else if (djroomData.CreateRoleID != client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djroomData)
					{
						if (djroomData.PKState > 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djroomRolesData)
					{
						result = -5;
					}
					else
					{
						int djroomID = client.ClientData.DJRoomID;
						GameManager.DJRoomMgr.RemoveRoomData(djroomID);
						GameManager.DJRoomMgr.RemoveRoomRolesData(djroomID);
						lock (djroomRolesData)
						{
							djroomRolesData.Removed = 1;
							for (int i = 0; i < djroomRolesData.Team1.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
								if (null != gameClient)
								{
									gameClient.ClientData.DJRoomID = -1;
									gameClient.ClientData.DJRoomTeamID = -1;
									gameClient.ClientData.HideSelf = 0;
								}
							}
							for (int i = 0; i < djroomRolesData.Team2.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
								if (null != gameClient)
								{
									gameClient.ClientData.DJRoomID = -1;
									gameClient.ClientData.DJRoomTeamID = -1;
									gameClient.ClientData.HideSelf = 0;
								}
							}
						}
						GameManager.ClientMgr.NotifyDianJiangCmd(sl, pool, client, 0, 2, djroomID, "", true);
						result = 0;
					}
				}
			}
			return result;
		}

		public int LeaveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djroomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djroomData)
				{
					result = -2;
				}
				else if (djroomData.CreateRoleID == client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djroomData)
					{
						if (djroomData.PKState > 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djroomRolesData)
					{
						result = -5;
					}
					else
					{
						int djroomID = client.ClientData.DJRoomID;
						bool flag2 = false;
						lock (djroomRolesData)
						{
							if (djroomRolesData.Removed > 0)
							{
								return -6;
							}
							if (djroomRolesData.Locked > 0)
							{
								return -7;
							}
							for (int i = 0; i < djroomRolesData.Team1.Count; i++)
							{
								if (client.ClientData.RoleID == djroomRolesData.Team1[i].RoleID)
								{
									flag2 = true;
									djroomRolesData.Team1.RemoveAt(i);
									break;
								}
							}
							if (!flag2)
							{
								for (int i = 0; i < djroomRolesData.Team2.Count; i++)
								{
									if (client.ClientData.RoleID == djroomRolesData.Team2[i].RoleID)
									{
										flag2 = true;
										djroomRolesData.Team2.RemoveAt(i);
										break;
									}
								}
							}
							djroomRolesData.TeamStates.Remove(client.ClientData.RoleID);
							djroomRolesData.RoleStates.Remove(client.ClientData.RoleID);
						}
						if (flag2)
						{
							lock (djroomData)
							{
								djroomData.PKRoleNum--;
							}
						}
						client.ClientData.DJRoomID = -1;
						client.ClientData.DJRoomTeamID = -1;
						client.ClientData.HideSelf = 0;
						GameManager.ClientMgr.NotifyDianJiangData(sl, pool, djroomData);
						GameManager.ClientMgr.NotifyDJRoomRolesData(sl, pool, djroomRolesData);
						GameManager.ClientMgr.NotifyDianJiangCmd(sl, pool, client, 0, 4, djroomID, Global.FormatRoleName(client, client.ClientData.RoleName), true);
						result = 0;
					}
				}
			}
			return result;
		}

		public int ViewerLeaveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else if (client.ClientData.DJRoomTeamID > 0)
			{
				result = -100;
			}
			else
			{
				DJRoomData djroomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djroomData)
				{
					result = -2;
				}
				else
				{
					DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djroomRolesData)
					{
						result = -3;
					}
					else
					{
						int djroomID = client.ClientData.DJRoomID;
						bool flag = false;
						lock (djroomRolesData)
						{
							if (null != djroomRolesData.ViewRoles)
							{
								for (int i = 0; i < djroomRolesData.ViewRoles.Count; i++)
								{
									if (client.ClientData.RoleID == djroomRolesData.ViewRoles[i].RoleID)
									{
										flag = true;
										djroomRolesData.ViewRoles.RemoveAt(i);
										break;
									}
								}
							}
						}
						if (flag)
						{
							lock (djroomData)
							{
								djroomData.ViewRoleNum--;
							}
						}
						client.ClientData.DJRoomID = -1;
						client.ClientData.DJRoomTeamID = -1;
						client.ClientData.HideSelf = 0;
						GameManager.ClientMgr.NotifyDianJiangData(sl, pool, djroomData);
						result = 0;
					}
				}
			}
			return result;
		}

		public int TransportDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			int result;
			if (client.ClientData.DJRoomID <= 0)
			{
				result = -1;
			}
			else
			{
				DJRoomData djroomData = GameManager.DJRoomMgr.FindRoomData(client.ClientData.DJRoomID);
				if (null == djroomData)
				{
					result = -2;
				}
				else if (djroomData.CreateRoleID != client.ClientData.RoleID)
				{
					result = -3;
				}
				else
				{
					lock (djroomData)
					{
						if (djroomData.PKState <= 0)
						{
							return -4;
						}
					}
					DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(client.ClientData.DJRoomID);
					if (null == djroomRolesData)
					{
						result = -5;
					}
					else
					{
						lock (djroomRolesData)
						{
							if (djroomRolesData.Locked <= 0)
							{
								return -6;
							}
							for (int i = 0; i < djroomRolesData.Team1.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, Global.DianJiangTaiMapCode, -1, -1, -1, 0);
								}
							}
							for (int i = 0; i < djroomRolesData.Team2.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, Global.DianJiangTaiMapCode, -1, -1, -1, 0);
								}
							}
						}
						result = 0;
					}
				}
			}
			return result;
		}

		public void NotifyDianJiangFightCmd(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData, int djCmdType, string extTag2, GameClient toClient = null)
		{
			if (null != djRoomData)
			{
				string data = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					djRoomData.RoomID,
					djCmdType,
					0,
					extTag2
				});
				DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djroomRolesData)
				{
					TCPOutPacket tcpoutPacket = null;
					if (null != toClient)
					{
						tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 189);
						if (!sl.SendData(toClient.ClientSocket, tcpoutPacket, true))
						{
						}
					}
					else
					{
						lock (djroomRolesData)
						{
							tcpoutPacket = null;
							try
							{
								for (int i = 0; i < djroomRolesData.Team1.Count; i++)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
									if (null != gameClient)
									{
										if (null == tcpoutPacket)
										{
											tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 189);
										}
										if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
										{
										}
									}
								}
								for (int i = 0; i < djroomRolesData.Team2.Count; i++)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
									if (null != gameClient)
									{
										if (null == tcpoutPacket)
										{
											tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 189);
										}
										if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
										{
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpoutPacket);
							}
							tcpoutPacket = null;
							try
							{
								if (null != djroomRolesData.ViewRoles)
								{
									data = string.Format("{0}:{1}:{2}:{3}", new object[]
									{
										djRoomData.RoomID,
										djCmdType,
										1,
										extTag2
									});
									for (int i = 0; i < djroomRolesData.ViewRoles.Count; i++)
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.ViewRoles[i].RoleID);
										if (null != gameClient)
										{
											if (null == tcpoutPacket)
											{
												tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 189);
											}
											if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
											{
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpoutPacket);
							}
						}
					}
				}
			}
		}

		public void NotifyDJFightRoomLeaveMsg(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData)
		{
			if (null != djRoomData)
			{
				DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djroomRolesData)
				{
					lock (djroomRolesData)
					{
						for (int i = 0; i < djroomRolesData.Team1.Count; i++)
						{
							int num = 0;
							djroomRolesData.RoleStates.TryGetValue(djroomRolesData.Team1[i].RoleID, out num);
							if (1 == num)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.LastMapCode, gameClient.ClientData.LastPosX, gameClient.ClientData.LastPosY, -1, 0);
								}
							}
						}
						for (int i = 0; i < djroomRolesData.Team2.Count; i++)
						{
							int num = 0;
							djroomRolesData.RoleStates.TryGetValue(djroomRolesData.Team2[i].RoleID, out num);
							if (1 == num)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.LastMapCode, gameClient.ClientData.LastPosX, gameClient.ClientData.LastPosY, -1, 0);
								}
							}
						}
						if (null != djroomRolesData.ViewRoles)
						{
							for (int i = 0; i < djroomRolesData.ViewRoles.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.ViewRoles[i].RoleID);
								if (null != gameClient)
								{
									GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.LastMapCode, gameClient.ClientData.LastPosX, gameClient.ClientData.LastPosY, -1, 0);
								}
							}
						}
					}
				}
			}
		}

		public void NotifyDianJiangRoomRolesPoint(SocketListener sl, TCPOutPacketPool pool, DJRoomRolesPoint djRoomRolesPoint)
		{
			if (null != djRoomRolesPoint)
			{
				DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomRolesPoint.RoomID);
				if (null != djroomRolesData)
				{
					byte[] array = DataHelper.ObjectToBytes<DJRoomRolesPoint>(djRoomRolesPoint);
					if (null != array)
					{
						TCPOutPacket tcpoutPacket = null;
						lock (djroomRolesData)
						{
							try
							{
								for (int i = 0; i < djroomRolesData.Team1.Count; i++)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
									if (null != gameClient)
									{
										if (null == tcpoutPacket)
										{
											tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 190);
										}
										if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
										{
										}
									}
								}
								for (int i = 0; i < djroomRolesData.Team2.Count; i++)
								{
									GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
									if (null != gameClient)
									{
										if (null == tcpoutPacket)
										{
											tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 190);
										}
										if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
										{
										}
									}
								}
								if (null != djroomRolesData.ViewRoles)
								{
									for (int i = 0; i < djroomRolesData.ViewRoles.Count; i++)
									{
										GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.ViewRoles[i].RoleID);
										if (null != gameClient)
										{
											if (null == tcpoutPacket)
											{
												tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 190);
											}
											if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
											{
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpoutPacket);
							}
						}
					}
				}
			}
		}

		public void RemoveDianJiangRoom(SocketListener sl, TCPOutPacketPool pool, DJRoomData djRoomData)
		{
			if (null != djRoomData)
			{
				DJRoomRolesData djroomRolesData = GameManager.DJRoomMgr.FindRoomRolesData(djRoomData.RoomID);
				if (null != djroomRolesData)
				{
					int roomID = djRoomData.RoomID;
					GameManager.DJRoomMgr.RemoveRoomData(roomID);
					GameManager.DJRoomMgr.RemoveRoomRolesData(roomID);
					lock (djroomRolesData)
					{
						djroomRolesData.Removed = 1;
						for (int i = 0; i < djroomRolesData.Team1.Count; i++)
						{
							int num = 0;
							djroomRolesData.RoleStates.TryGetValue(djroomRolesData.Team1[i].RoleID, out num);
							if (1 == num)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team1[i].RoleID);
								if (null != gameClient)
								{
									gameClient.ClientData.DJRoomID = -1;
									gameClient.ClientData.DJRoomTeamID = -1;
									gameClient.ClientData.HideSelf = 0;
								}
							}
						}
						for (int i = 0; i < djroomRolesData.Team2.Count; i++)
						{
							int num = 0;
							djroomRolesData.RoleStates.TryGetValue(djroomRolesData.Team2[i].RoleID, out num);
							if (1 == num)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.Team2[i].RoleID);
								if (null != gameClient)
								{
									gameClient.ClientData.DJRoomID = -1;
									gameClient.ClientData.DJRoomTeamID = -1;
									gameClient.ClientData.HideSelf = 0;
								}
							}
						}
						if (null != djroomRolesData.ViewRoles)
						{
							for (int i = 0; i < djroomRolesData.ViewRoles.Count; i++)
							{
								GameClient gameClient = GameManager.ClientMgr.FindClient(djroomRolesData.ViewRoles[i].RoleID);
								if (null != gameClient)
								{
									gameClient.ClientData.DJRoomID = -1;
									gameClient.ClientData.DJRoomTeamID = -1;
									gameClient.ClientData.HideSelf = 0;
								}
							}
						}
					}
					string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						0,
						-1,
						2,
						roomID,
						"noHint"
					});
					int num2 = 0;
					TCPOutPacket tcpoutPacket = null;
					try
					{
						GameClient nextClient;
						while ((nextClient = this.GetNextClient(ref num2, false)) != null)
						{
							if (nextClient.ClientData.ViewDJRoomDlg)
							{
								if (null == tcpoutPacket)
								{
									tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 188);
								}
								if (!sl.SendData(nextClient.ClientSocket, tcpoutPacket, false))
								{
								}
							}
						}
					}
					finally
					{
						this.PushBackTcpOutPacket(tcpoutPacket);
					}
				}
			}
		}

		public void NotifyArenaBattleCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int battleType, int extTag1, int leftSecs)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				battleType,
				extTag1,
				leftSecs
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 415);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllArenaBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int minLevel, int battleType, int extTag1, int leftSecs)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Level >= minLevel)
				{
					if (!nextClient.ClientSocket.IsKuaFuLogin)
					{
						string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							0,
							nextClient.ClientData.RoleID,
							battleType,
							extTag1,
							leftSecs
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 415);
						if (!sl.SendData(nextClient.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		public void NotifyArenaBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int battleType, int extTag1, int leftSecs)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					battleType,
					extTag1,
					leftSecs
				});
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, 415);
			}
		}

		public void NotifyArenaBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, int roleNumKilled, int roleNumOnStart, int rowNumNow)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(GameManager.ArenaBattleMgr.BattleMapCode);
			if (null != objectsByMap)
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					GameClient gameClient = objectsByMap[i] as GameClient;
					if (null != gameClient)
					{
						string strCmd = string.Format("{0}:{1}", gameClient.ClientData.KingOfPkCurrentPoint, rowNumNow);
						this.SendToClient(sl, pool, gameClient, strCmd, 416);
					}
				}
			}
		}

		public void NotifyBattleCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int battleType, int extTag1, int leftSecs)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				status,
				client.ClientData.RoleID,
				battleType,
				extTag1,
				leftSecs
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 179);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int minLevel, int battleType, int extTag1, int leftSecs)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Level >= minLevel)
				{
					if (!nextClient.ClientSocket.IsKuaFuLogin)
					{
						string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							0,
							nextClient.ClientData.RoleID,
							battleType,
							extTag1,
							leftSecs
						});
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 179);
						if (!sl.SendData(nextClient.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		public void NotifyBattleInviteMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int battleType, int extTag1, int leftSecs)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					0,
					-1,
					battleType,
					extTag1,
					leftSecs
				});
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, 179);
			}
		}

		public void BattleBeginForceLeaveg(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					GameClient gameClient = objectsByMap[i] as GameClient;
					if (null != gameClient)
					{
						this.Container.RemoveObject(gameClient.ClientData.RoleID, mapCode, gameClient);
					}
				}
			}
		}

		public void NotifyBattleLeaveMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					GameClient gameClient = objectsByMap[i] as GameClient;
					if (null != gameClient)
					{
						int num = GameManager.MainMapCode;
						int maxX = -1;
						int mapY = -1;
						if (gameClient.ClientData.LastMapCode != -1 && gameClient.ClientData.LastPosX != -1 && gameClient.ClientData.LastPosY != -1)
						{
							if (MapTypes.Normal == Global.GetMapType(gameClient.ClientData.LastMapCode))
							{
								num = gameClient.ClientData.LastMapCode;
								maxX = gameClient.ClientData.LastPosX;
								mapY = gameClient.ClientData.LastPosY;
							}
						}
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(num, out gameMap))
						{
							GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, num, maxX, mapY, -1, 0);
						}
					}
				}
			}
		}

		public void NotifyBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int tangJiFen, int suiJiFen)
		{
			int battleMaxPointNow = BattleManager.BattleMaxPointNow;
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.BattleKilledNum,
				battleMaxPointNow,
				tangJiFen,
				suiJiFen
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 282);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyBattleKilledNumCmd(SocketListener sl, TCPOutPacketPool pool, int suiJiFen, int tangJiFen)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(GameManager.BattleMgr.BattleMapCode);
			if (null != objectsByMap)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					-1,
					-1,
					-1,
					tangJiFen,
					suiJiFen
				});
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, 282);
			}
		}

		public void NotifyRoleBattleNameInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 283);
			}
		}

		public void ProcessRoleBattleNameInfoTimeOut(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.BattleNameIndex > 0)
			{
				long num = TimeUtil.NOW();
				if (num - client.ClientData.BattleNameStart >= Global.MaxBattleNameTicks)
				{
					client.ClientData.BattleNameIndex = 0;
					GameManager.DBCmdMgr.AddDBCmd(10059, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BattleNameStart, client.ClientData.BattleNameIndex), null, client.ServerId);
					GameManager.ClientMgr.NotifyRoleBattleNameInfo(sl, pool, client);
				}
			}
		}

		public void NotifyRoleBattleRoleInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, int startTotalRoleNum, int allKilledRoleNum)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = string.Format("{0}:{1}", startTotalRoleNum, allKilledRoleNum);
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, 285);
			}
		}

		public void NotifyRoleBattleEndInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, List<BattleEndRoleItem> endRoleItemList)
		{
			if (endRoleItemList.Count > 0)
			{
				List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
				if (null != objectsByMap)
				{
					byte[] bytesData = DataHelper.ObjectToBytes<List<BattleEndRoleItem>>(endRoleItemList);
					this.SendToClients(sl, pool, null, objectsByMap, bytesData, 286);
				}
			}
		}

		public void NotifyRoleBattleSideInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BattleWhichSide);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 359);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyRoleBattlePlayerSideNumberEndInfo(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nNum1, int nNum2)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = string.Format("{0}:{1}", nNum1, nNum2);
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, 547);
			}
		}

		public void NotifyAutoFightCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int fightType, int extTag1)
		{
			SCAutoFight cmdData = new SCAutoFight(status, client.ClientData.RoleID, fightType, extTag1);
			client.sendCmd<SCAutoFight>(182, cmdData, false);
		}

		public void NotifyTeamCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int teamType, int extTag1, string extTag2, int nOccu = -1, int nLev = -1, int nChangeLife = -1)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
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
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 176);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyTeamData(SocketListener sl, TCPOutPacketPool pool, TeamData td)
		{
			if (null != td)
			{
				lock (td)
				{
					byte[] array = DataHelper.ObjectToBytes<TeamData>(td);
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient gameClient = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != gameClient)
						{
							TCPOutPacket tcpoutPacket = pool.Pop();
							tcpoutPacket.PacketCmdID = 177;
							tcpoutPacket.FinalWriteData(array, 0, array.Length);
							if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, true))
							{
								break;
							}
						}
					}
				}
			}
		}

		public void NotifyOthersTeamIDChanged(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TeamID, Global.GetGameClientTeamLeaderID(client.ClientData));
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 178);
			}
		}

		public void NotifyOthersTeamDestroy(SocketListener sl, TCPOutPacketPool pool, GameClient client, TeamData td)
		{
			if (null != td)
			{
				lock (td)
				{
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient gameClient = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != gameClient)
						{
							if (client != gameClient)
							{
								gameClient.ClientData.TeamID = 0;
								GameManager.TeamMgr.RemoveRoleID2TeamID(gameClient.ClientData.RoleID);
								this.NotifyOthersTeamIDChanged(sl, pool, gameClient);
							}
						}
					}
				}
			}
		}

		public void NotifyTeamUpLevel(SocketListener sl, TCPOutPacketPool pool, GameClient client, bool zhuanShengChanged = false)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != teamData)
				{
					lock (teamData)
					{
						for (int i = 0; i < teamData.TeamRoles.Count; i++)
						{
							GameClient gameClient = this.FindClient(teamData.TeamRoles[i].RoleID);
							if (null != gameClient)
							{
								if (teamData.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									teamData.TeamRoles[i].Level = client.ClientData.Level;
									teamData.TeamRoles[i].ChangeLifeLev = client.ClientData.ChangeLifeCount;
								}
								string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Level, client.ClientData.ChangeLifeCount);
								TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 288);
								if (!sl.SendData(gameClient.ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		public void NotifySelfChgZhanLi(GameClient client, int ZhanLi)
		{
			client.sendCmd<int>(675, ZhanLi, false);
		}

		public void NotifyTeamCHGZhanLi(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != teamData)
				{
					lock (teamData)
					{
						string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.CombatForce);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 674);
						for (int i = 0; i < teamData.TeamRoles.Count; i++)
						{
							GameClient gameClient = this.FindClient(teamData.TeamRoles[i].RoleID);
							if (null != gameClient)
							{
								if (teamData.TeamRoles[i].RoleID == client.ClientData.RoleID)
								{
									teamData.TeamRoles[i].CombatForce = client.ClientData.CombatForce;
								}
								if (gameClient.CodeRevision >= 1)
								{
									if (!sl.SendData(gameClient.ClientSocket, tcpOutPacket, false))
									{
									}
								}
							}
						}
						this.PushBackTcpOutPacket(tcpOutPacket);
					}
				}
			}
		}

		public void NotifyGoodsExchangeCmd(SocketListener sl, TCPOutPacketPool pool, int roleID, int otherRoleID, GameClient client, GameClient otherClient, int status, int exchangeType)
		{
			string data = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				status,
				roleID,
				otherRoleID,
				exchangeType
			});
			if (null != client)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 170);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			if (null != otherClient)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 170);
				if (!sl.SendData(otherClient.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyGoodsExchangeData(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, ExchangeData ed)
		{
			byte[] array = null;
			lock (ed)
			{
				array = DataHelper.ObjectToBytes<ExchangeData>(ed);
			}
			TCPOutPacket tcpoutPacket = pool.Pop();
			tcpoutPacket.PacketCmdID = 171;
			tcpoutPacket.FinalWriteData(array, 0, array.Length);
			if (!sl.SendData(client.ClientSocket, tcpoutPacket, true))
			{
			}
			tcpoutPacket = pool.Pop();
			tcpoutPacket.PacketCmdID = 171;
			tcpoutPacket.FinalWriteData(array, 0, array.Length);
			if (!sl.SendData(otherClient.ClientSocket, tcpoutPacket, true))
			{
			}
		}

		private void ProcessExchangeData(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.ExchangeID > 0)
			{
				ExchangeData exchangeData = GameManager.GoodsExchangeMgr.FindData(client.ClientData.ExchangeID);
				if (null != exchangeData)
				{
					int num = (exchangeData.RequestRoleID == client.ClientData.RoleID) ? exchangeData.AgreeRoleID : exchangeData.RequestRoleID;
					GameClient gameClient = GameManager.ClientMgr.FindClient(num);
					if (null != gameClient)
					{
						if (gameClient.ClientData.ExchangeID > 0 && gameClient.ClientData.ExchangeID == client.ClientData.ExchangeID)
						{
							GameManager.GoodsExchangeMgr.RemoveData(client.ClientData.ExchangeID);
							Global.RestoreExchangeData(gameClient, exchangeData);
							gameClient.ClientData.ExchangeID = 0;
							gameClient.ClientData.ExchangeTicks = 0L;
							GameManager.ClientMgr.NotifyGoodsExchangeCmd(sl, pool, client.ClientData.RoleID, num, null, gameClient, client.ClientData.ExchangeID, 4);
						}
					}
				}
			}
		}

		public void NotifyMySelfNewGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int ownerRoleID, string ownerRoleName, int autoID, int goodsPackID, int mapCode, int toX, int toY, int goodsID, int goodsNum, long productTicks, int teamID, string teamRoleIDs, int lucky, int excellenceInfo, int appendPropLev, int forge_Level)
		{
			NewGoodsPackData instance = new NewGoodsPackData
			{
				ownerRoleID = ownerRoleID,
				ownerRoleName = ownerRoleName,
				autoID = autoID,
				goodsPackID = goodsPackID,
				mapCode = mapCode,
				toX = toX,
				toY = toY,
				goodsID = goodsID,
				goodsNum = goodsNum,
				productTicks = productTicks,
				teamID = (long)teamID,
				teamRoleIDs = teamRoleIDs,
				lucky = lucky,
				excellenceInfo = excellenceInfo,
				appendPropLev = appendPropLev,
				forge_Level = forge_Level
			};
			byte[] array = DataHelper.ObjectToBytes<NewGoodsPackData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 145);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySelfGetThing(SocketListener sl, TCPOutPacketPool pool, GameClient client, int goodsDbID)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, goodsDbID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 148);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyOthersDelGoodsPack(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int mapCode, int autoID, int toRoleID)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", autoID, toRoleID);
				this.SendToClients(sl, pool, null, objsList, strCmd, 146);
			}
		}

		public void NotifyMySelfDelGoodsPack(SocketListener sl, TCPOutPacketPool pool, GameClient client, int autoID)
		{
			string data = string.Format("{0}:{1}", autoID, -1);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 146);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public static void NotifySelfEnemyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int burst, int injure, double enemyLife, long newExperience, int nMerlinInjure = 0, EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None, int armorV_p1 = 0)
		{
			SpriteAttackResultData spriteAttackResultData = new SpriteAttackResultData();
			spriteAttackResultData.enemy = enemy;
			spriteAttackResultData.burst = burst;
			spriteAttackResultData.injure = injure;
			spriteAttackResultData.enemyLife = enemyLife;
			spriteAttackResultData.newExperience = newExperience;
			spriteAttackResultData.currentExperience = client.ClientData.Experience;
			spriteAttackResultData.newLevel = client.ClientData.Level;
			spriteAttackResultData.armorV_p1 = armorV_p1;
			if (nMerlinInjure > 0)
			{
				spriteAttackResultData.MerlinInjuer = nMerlinInjure;
				spriteAttackResultData.MerlinType = (int)eMerlinType;
			}
			byte[] array = DataHelper.ObjectToBytes<SpriteAttackResultData>(spriteAttackResultData);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 117);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		private IObject GetInjuredObject(int mapCode, int injuredRoleID)
		{
			IObject result = null;
			GSpriteTypes spriteType = Global.GetSpriteType((uint)injuredRoleID);
			if (spriteType == GSpriteTypes.Monster)
			{
				Monster monster = GameManager.MonsterMgr.FindMonster(mapCode, injuredRoleID);
				if (null != monster)
				{
					result = monster;
				}
			}
			else if (spriteType == GSpriteTypes.BiaoChe)
			{
				BiaoCheManager.FindBiaoCheByRoleID(injuredRoleID);
			}
			else
			{
				if (spriteType == GSpriteTypes.JunQi)
				{
					return JunQiManager.FindJunQiByID(injuredRoleID);
				}
				GameClient gameClient = GameManager.ClientMgr.FindClient(injuredRoleID);
				if (null != gameClient)
				{
					result = gameClient;
				}
			}
			return result;
		}

		public void NotifySpriteInjured(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int mapCode, int attackerRoleID, int injuredRoleID, int burst, int injure, double injuredRoleLife, int attackerLevel, Point hitToGrid, int nMerlinInjure = 0, EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None, int armorV_p1 = 0)
		{
			if (hitToGrid.X < 0.0 || hitToGrid.Y < 0.0)
			{
				hitToGrid.X = 0.0;
				hitToGrid.Y = 0.0;
			}
			IObject injuredObject = this.GetInjuredObject(mapCode, injuredRoleID);
			if (null != injuredObject)
			{
				List<object> list = Global.GetAll9Clients(attacker);
				if (null == list)
				{
					list = new List<object>();
				}
				if (GameManager.FlagHideFlagsType == -1 || !GameManager.HideFlagsMapDict.ContainsKey(mapCode))
				{
					if (list.IndexOf(injuredObject) < 0)
					{
						list.Add(injuredObject);
					}
					int injuredRoleMagic = 0;
					int injuredRoleMaxMagicV = 0;
					int injuredRoleMaxLifeV = 0;
					GameClient gameClient = this.FindClient(injuredRoleID);
					if (null != gameClient)
					{
						injuredRoleMagic = gameClient.ClientData.CurrentMagicV;
						injuredRoleMaxMagicV = gameClient.ClientData.MagicV;
						injuredRoleMaxLifeV = gameClient.ClientData.LifeV;
					}
					SpriteInjuredData spriteInjuredData = new SpriteInjuredData();
					spriteInjuredData.attackerRoleID = attackerRoleID;
					spriteInjuredData.injuredRoleID = injuredRoleID;
					spriteInjuredData.burst = burst;
					spriteInjuredData.injure = injure;
					spriteInjuredData.injuredRoleLife = (long)injuredRoleLife;
					spriteInjuredData.attackerLevel = attackerLevel;
					spriteInjuredData.injuredRoleMaxLifeV = injuredRoleMaxLifeV;
					spriteInjuredData.injuredRoleMagic = injuredRoleMagic;
					spriteInjuredData.injuredRoleMaxMagicV = injuredRoleMaxMagicV;
					spriteInjuredData.hitToGridX = (int)hitToGrid.X;
					spriteInjuredData.hitToGridY = (int)hitToGrid.Y;
					spriteInjuredData.MerlinInjuer = nMerlinInjure;
					spriteInjuredData.MerlinType = (int)((sbyte)eMerlinType);
					spriteInjuredData.armorV_p1 = armorV_p1;
					byte[] array = DataHelper.ObjectToBytes<SpriteInjuredData>(spriteInjuredData);
					spriteInjuredData.burst = 999;
					spriteInjuredData.MerlinType = 999;
					byte[] array2 = DataHelper.ObjectToBytes<SpriteInjuredData>(spriteInjuredData);
					this.SendToClients(sl, pool, attacker, injuredObject, list, array, array2, 118, injure != 0);
					if (injure != 0)
					{
						this.NotifySpriteTeamInjured(sl, pool, injuredRoleID, array, array2, mapCode);
					}
				}
				else
				{
					SpriteInjuredData spriteInjuredData = new SpriteInjuredData();
					spriteInjuredData.injuredRoleID = injuredRoleID;
					spriteInjuredData.injuredRoleLife = (long)injuredRoleLife;
					spriteInjuredData.burst = burst;
					spriteInjuredData.injure = injure;
					spriteInjuredData.armorV_p1 = armorV_p1;
					if (hitToGrid.X > 0.0 || hitToGrid.Y > 0.0)
					{
						spriteInjuredData.hitToGridX = (int)hitToGrid.X;
						spriteInjuredData.hitToGridY = (int)hitToGrid.Y;
						spriteInjuredData.attackerRoleID = attackerRoleID;
					}
					if (nMerlinInjure > 0)
					{
						spriteInjuredData.MerlinInjuer = nMerlinInjure;
						spriteInjuredData.MerlinType = (int)((sbyte)eMerlinType);
					}
					if (injuredObject != null && injuredObject.ObjectType == ObjectTypes.OT_CLIENT)
					{
						if (list.IndexOf(injuredObject) < 0)
						{
							list.Add(injuredObject);
						}
					}
					bool flag = injuredRoleLife <= 0.0;
					if (flag)
					{
						spriteInjuredData.attackerRoleID = attackerRoleID;
					}
					byte[] array = DataHelper.ObjectToBytes<SpriteInjuredData>(spriteInjuredData);
					spriteInjuredData.burst = 999;
					spriteInjuredData.MerlinType = 999;
					byte[] array2 = DataHelper.ObjectToBytes<SpriteInjuredData>(spriteInjuredData);
					if (flag)
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, injuredObject, list, array, array2, 118, 1, injuredRoleID);
					}
					else
					{
						this.SendToClients<object, byte[]>(sl, pool, attacker, injuredObject, list, array, array2, 118, 1, injuredRoleID);
					}
					if (injure != 0)
					{
						this.NotifySpriteTeamInjured(sl, pool, injuredRoleID, array, array2, mapCode);
					}
				}
			}
		}

		public void NotifySpriteTeamInjured(SocketListener sl, TCPOutPacketPool pool, int injuredRoleID, byte[] bytesCmd, byte[] bytesCmd2, int mapCode)
		{
			GameClient gameClient = this.FindClient(injuredRoleID);
			if (null != gameClient)
			{
				ZorkBattleManager.getInstance().NotifySpriteInjured(gameClient);
				EscapeBattleManager.getInstance().NotifySpriteInjured(gameClient);
				if (gameClient.ClientData.TeamID > 0)
				{
					TeamData teamData = GameManager.TeamMgr.FindData(gameClient.ClientData.TeamID);
					if (null != teamData)
					{
						List<int> list = new List<int>();
						lock (teamData)
						{
							for (int i = 0; i < teamData.TeamRoles.Count; i++)
							{
								if (injuredRoleID != teamData.TeamRoles[i].RoleID)
								{
									list.Add(teamData.TeamRoles[i].RoleID);
								}
							}
						}
						if (list.Count != 0)
						{
							TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd, 0, bytesCmd.Length, 118);
							TCPOutPacket tcpOutPacket2 = TCPOutPacket.MakeTCPOutPacket(pool, bytesCmd2, 0, bytesCmd2.Length, 118);
							try
							{
								for (int i = 0; i < list.Count; i++)
								{
									GameClient gameClient2 = this.FindClient(list[i]);
									if (null != gameClient2)
									{
										if (gameClient2.ClientData.MapCode == mapCode)
										{
											if (gameClient2.ClientEffectHideFlag1 <= 0)
											{
												sl.SendData(gameClient2.ClientSocket, tcpOutPacket, false);
											}
											else
											{
												sl.SendData(gameClient2.ClientSocket, tcpOutPacket2, false);
											}
										}
									}
								}
							}
							finally
							{
								this.PushBackTcpOutPacket(tcpOutPacket);
								this.PushBackTcpOutPacket(tcpOutPacket2);
							}
						}
					}
				}
			}
		}

		public void NotifyAllImportantMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0, int minZhuanSheng = 0, int minLevel = 0, int maxZhuanSheng = 100, int maxLevel = 100)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (client == null || nextClient != client)
				{
					if (nextClient == null || Global.GetUnionLevel(nextClient, false) >= Global.GetUnionLevel(minZhuanSheng, minLevel, false))
					{
						if (nextClient == null || Global.GetUnionLevel(nextClient, false) <= Global.GetUnionLevel(maxZhuanSheng, maxLevel, false))
						{
							this.NotifyImportantMsg(sl, pool, nextClient, msgText, typeIndex, showGameInfoType, errCode);
						}
					}
				}
			}
		}

		public void NotifyBangHuiImportantMsg(SocketListener sl, TCPOutPacketPool pool, int faction, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (faction == nextClient.ClientData.Faction)
				{
					this.NotifyImportantMsg(sl, pool, nextClient, msgText, typeIndex, showGameInfoType, errCode);
				}
			}
		}

		public void SendBangHuiCmd<T>(int faction, int cmdId, T cmdData, bool excludeKuaFu = true, bool normalMapNoly = false)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (faction == nextClient.ClientData.Faction)
				{
					if (!excludeKuaFu || !nextClient.ClientSocket.IsKuaFuLogin)
					{
						if (!normalMapNoly || Global.GetMapSceneType(nextClient.ClientData.MapCode) == 0)
						{
							nextClient.sendCmd<T>(cmdId, cmdData, false);
						}
					}
				}
			}
		}

		public void NotifyImportantMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			msgText = msgText.Replace(":", "``");
			string data = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				(int)showGameInfoType,
				(int)typeIndex,
				msgText,
				errCode
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 194);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyUseGoodsResult(SocketListener sl, TCPOutPacketPool pool, GameClient client, int goodsID, int useNum)
		{
			string data = string.Format("{0}:{1}", goodsID, useNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 689);
			sl.SendData(client.ClientSocket, tcpOutPacket, true);
		}

		public void NotifyImportantMsg(GameClient client, string msgText, GameInfoTypeIndexes typeIndex, ShowGameInfoTypes showGameInfoType, int errCode = 0)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msgText, typeIndex, showGameInfoType, errCode);
		}

		public void NotifyImportantMsgWithGoods(GameClient client, MsgWithGoodsType idx, ShowGameInfoTypes showType, List<GoodsData> goodsDataList, string param1, Dictionary<int, List<GoodsData>> goodsDic = null)
		{
			NotifyMsgWithGoodsData instance = new NotifyMsgWithGoodsData
			{
				index = (int)idx,
				type = (int)showType,
				goodsDataList = goodsDataList,
				param1 = param1,
				goodsDic = goodsDic
			};
			byte[] array = DataHelper.ObjectToBytes<NotifyMsgWithGoodsData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, array, 0, array.Length, 3001);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyGetAwardMsg(GameClient client, RoleAwardMsg type, string notifyParam)
		{
			List<GoodsData> awardRecord = client.ClientData.GetAwardRecord(type);
			if (awardRecord != null && awardRecord.Count != 0)
			{
				GameManager.ClientMgr.NotifyImportantMsgWithGoods(client, MsgWithGoodsType.GoodsAwards, ShowGameInfoTypes.OnlyChatBox, awardRecord, notifyParam, null);
				client.ClientData.ClearAwardRecord(type);
			}
		}

		public void NotifyAddExpMsg(GameClient client, long addExp)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format(GLang.GetLang(60, new object[0]), addExp), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
		}

		public void NotifyAddJinBiMsg(GameClient client, int addJinBi)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(61, new object[0]), new object[]
			{
				addJinBi
			}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 27);
		}

		public void NotifyHintMsg(GameClient client, string msg)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
		}

		public void NotifyHintMsgDelay(GameClient client, string msg)
		{
			msg = msg.Replace(":", "``");
			string cmdData = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				4,
				1,
				msg,
				0
			});
			client.sendCmd(194, cmdData, false);
		}

		public void NotifyCopyMapHintMsg(GameClient client, string msg)
		{
			GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, msg, GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
		}

		public void NotifyGMAuthCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int auth)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, auth);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 211);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllBulletinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, BulletinMsgData bulletinMsgData, int minZhuanSheng = 0, int minLevel = 0)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (client == null || nextClient != client)
				{
					if (Global.GetUnionLevel(nextClient, false) >= Global.GetUnionLevel(minZhuanSheng, minLevel, false))
					{
						if (!nextClient.ClientSocket.IsKuaFuLogin)
						{
							this.NotifyBulletinMsg(sl, pool, nextClient, bulletinMsgData);
						}
					}
				}
			}
		}

		public void NotifyBulletinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, BulletinMsgData bulletinMsgData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BulletinMsgData>(bulletinMsgData, pool, 210);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void ChangeRolePKValueAndPKPoint(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy)
		{
			if (client != enemy)
			{
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
				{
					if (!WangChengManager.IsInCityWarBattling(client))
					{
						if (!MoYuLongXue.InMoYuMap(client.ClientData.MapCode))
						{
							if (!ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
							{
								GameMap gameMap = null;
								if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
								{
									if (0 == gameMap.PKMode)
									{
										client.ClientData.PKValue = client.ClientData.PKValue + 1;
										int nameColorIndexByPKPoints = Global.GetNameColorIndexByPKPoints(enemy.ClientData.PKPoint);
										if (nameColorIndexByPKPoints < 2)
										{
											if (!Global.IsPurpleName(enemy))
											{
												client.ClientData.PKPoint = Global.GMin(Global.MaxPKPointValue, client.ClientData.PKPoint + Global.PKValueEqPKPoints);
											}
										}
										else if (Global.IsRedName(client))
										{
											if (Global.AddToTodayRoleKillRoleSet(client.ClientData.RoleID, enemy.ClientData.RoleID))
											{
												client.ClientData.PKPoint = Global.GMax(0, client.ClientData.PKPoint - Global.PKValueEqPKPoints / 2);
											}
										}
										Global.ProcessRedNamePunishForDebuff(client);
										List<object> all9Clients = Global.GetAll9Clients(client);
										if (null != all9Clients)
										{
											string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint);
											this.SendToClients(sl, pool, null, all9Clients, strCmd, 150);
										}
									}
								}
							}
						}
					}
				}
			}
		}

		public void SetRolePKValuePoint(SocketListener sl, TCPOutPacketPool pool, GameClient client, int pkValue, int pkPoint, bool writeToDB = true)
		{
			client.ClientData.PKValue = pkValue;
			client.ClientData.PKPoint = pkPoint;
			Global.ProcessRedNamePunishForDebuff(client);
			if (writeToDB)
			{
				GameManager.DBCmdMgr.AddDBCmd(10009, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10009, nowTicks);
			}
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.PKValue, client.ClientData.PKPoint);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 150);
			}
		}

		public void ChangeRolePurpleName(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy)
		{
			if (client != enemy)
			{
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode && client.ClientData.MapCode != GameManager.ArenaBattleMgr.BattleMapCode)
				{
					if (enemy.ClientData.PKPoint < Global.MinRedNamePKPoints)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(client.ClientData.MapCode, out gameMap))
						{
							if (0 == gameMap.PKMode)
							{
								client.ClientData.StartPurpleNameTicks = TimeUtil.NOW();
								List<object> all9Clients = Global.GetAll9Clients(client);
								if (null != all9Clients)
								{
									string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
									this.SendToClients(sl, pool, null, all9Clients, strCmd, 265);
								}
							}
						}
					}
				}
			}
		}

		public void ForceChangeRolePurpleName2(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (!Global.IsPurpleName(client))
			{
				client.ClientData.StartPurpleNameTicks = TimeUtil.NOW();
				List<object> all9Clients = Global.GetAll9Clients(client);
				if (null != all9Clients)
				{
					string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
					this.SendToClients(sl, pool, null, all9Clients, strCmd, 265);
				}
			}
		}

		public void BroadcastRolePurpleName(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (client.ClientData.StartPurpleNameTicks > 0L)
			{
				if (!Global.IsPurpleName(client))
				{
					client.ClientData.StartPurpleNameTicks = 0L;
					List<object> all9Clients = Global.GetAll9Clients(client);
					if (null != all9Clients)
					{
						string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StartPurpleNameTicks);
						this.SendToClients(sl, pool, null, all9Clients, strCmd, 265);
					}
				}
			}
		}

		public void NotifyAllChatMsg(SocketListener sl, TCPOutPacketPool pool, string cmdText, GameClient sender = null)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (sender != null && nextClient.ClientSocket.IsKuaFuLogin)
				{
					if (sender.ClientData.MapCode == nextClient.ClientData.MapCode && sender.ClientData.CopyMapID == nextClient.ClientData.CopyMapID)
					{
						this.SendChatMessage(sl, pool, nextClient, cmdText);
					}
				}
				else
				{
					this.SendChatMessage(sl, pool, nextClient, cmdText);
				}
			}
		}

		public void NotifyFactionChatMsg(SocketListener sl, TCPOutPacketPool pool, int faction, string cmdText, GameClient sender = null)
		{
			if (faction > 0)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					if (nextClient.ClientData.Faction == faction)
					{
						if (sender != null && nextClient.ClientSocket.IsKuaFuLogin)
						{
							if (sender.SceneGameId == nextClient.SceneGameId)
							{
								this.SendChatMessage(sl, pool, nextClient, cmdText);
							}
						}
						else
						{
							this.SendChatMessage(sl, pool, nextClient, cmdText);
						}
					}
				}
			}
		}

		public void NotifyTeamChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.TeamID > 0)
			{
				TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != teamData)
				{
					List<int> list = new List<int>();
					lock (teamData)
					{
						for (int i = 0; i < teamData.TeamRoles.Count; i++)
						{
							list.Add(teamData.TeamRoles[i].RoleID);
						}
					}
					if (list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							GameClient gameClient = this.FindClient(list[i]);
							if (null != gameClient)
							{
								this.SendChatMessage(sl, pool, gameClient, cmdText);
							}
						}
					}
				}
			}
		}

		public void NotifyMapChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			List<object> list = new List<object>();
			this.LookupRolesInCircle(null, client.ClientData.MapCode, client.ClientData.PosX, client.ClientData.PosY, 1000, list);
			if (list.Count > 0)
			{
				this.SendToClients(sl, pool, null, list, cmdText, 157);
			}
		}

		public void NotifyCopyMapChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
				if (null != copyMap)
				{
					List<object> clientsList = copyMap.GetClientsList2();
					if (clientsList != null && clientsList.Count > 0)
					{
						this.SendToClients(sl, pool, null, clientsList, cmdText, 157);
					}
				}
			}
		}

		public void NotifyBattleSideChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.BattleWhichSide > 0)
			{
				if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
				{
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
					if (null != copyMap)
					{
						List<object> clientsList = copyMap.GetClientsList2();
						if (clientsList != null && clientsList.Count > 0)
						{
							foreach (object obj in clientsList)
							{
								GameClient gameClient = obj as GameClient;
								if (null != gameClient)
								{
									if (client.ClientData.BattleWhichSide == gameClient.ClientData.BattleWhichSide)
									{
										gameClient.sendCmd(157, cmdText, false);
									}
								}
							}
						}
					}
				}
				else
				{
					List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(client.ClientData.MapCode);
					foreach (GameClient gameClient in mapGameClients)
					{
						if (null != gameClient)
						{
							if (client.ClientData.BattleWhichSide == gameClient.ClientData.BattleWhichSide)
							{
								gameClient.sendCmd(157, cmdText, false);
							}
						}
					}
				}
			}
		}

		public void NotifyZhanDuiChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			if (client.ClientData.ZhanDuiID > 0)
			{
				if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
				{
					CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(client.ClientData.MapCode, client.ClientData.FuBenSeqID);
					if (null != copyMap)
					{
						List<object> clientsList = copyMap.GetClientsList2();
						if (clientsList != null && clientsList.Count > 0)
						{
							foreach (object obj in clientsList)
							{
								GameClient gameClient = obj as GameClient;
								if (null != gameClient)
								{
									if (client.ClientData.ZhanDuiID == gameClient.ClientData.ZhanDuiID)
									{
										gameClient.sendCmd(157, cmdText, false);
									}
								}
							}
						}
					}
				}
				else
				{
					List<GameClient> mapGameClients = GameManager.ClientMgr.GetMapGameClients(client.ClientData.MapCode);
					foreach (GameClient gameClient in mapGameClients)
					{
						if (null != gameClient)
						{
							if (client.ClientData.ZhanDuiID == gameClient.ClientData.ZhanDuiID)
							{
								gameClient.sendCmd(157, cmdText, false);
							}
						}
					}
				}
			}
		}

		public bool NotifyClientChatMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, int fromRoleID, string fromRoleName, string toRoleName, int index, string textMsg, string chatType)
		{
			bool result = true;
			GameClient gameClient = null;
			int num = RoleName2IDs.FindRoleIDByName(toRoleName, false);
			if (-1 == num)
			{
				result = false;
			}
			else
			{
				gameClient = this.FindClient(num);
				if (null == gameClient)
				{
					result = false;
				}
				else if (Global.InFriendsBlackList(gameClient, fromRoleID))
				{
					gameClient = null;
				}
			}
			string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				fromRoleID,
				fromRoleName,
				0,
				toRoleName,
				index,
				textMsg,
				chatType
			});
			if (null != client)
			{
				this.SendChatMessage(sl, pool, client, cmdText);
			}
			if (null != gameClient)
			{
				this.SendChatMessage(sl, pool, gameClient, cmdText);
			}
			else
			{
				string textMsg2 = string.Format(GLang.GetLang(62, new object[0]), toRoleName);
				GameManager.ClientMgr.SendSystemChatMessageToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, textMsg2);
			}
			return result;
		}

		public void SendSystemChatMessageToClient(SocketListener sl, TCPOutPacketPool pool, GameClient client, string textMsg)
		{
			if (null != client)
			{
				string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					-1,
					"",
					0,
					"",
					0,
					textMsg,
					0
				});
				this.SendChatMessage(sl, pool, client, cmdText);
			}
		}

		public void SendSystemChatMessageToClients(SocketListener sl, TCPOutPacketPool pool, string textMsg)
		{
			string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				-1,
				"",
				0,
				"",
				0,
				textMsg
			});
			this.NotifyAllChatMsg(sl, pool, cmdText, null);
		}

		public void SendChatMessage(SocketListener sl, TCPOutPacketPool pool, GameClient client, string cmdText)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, cmdText, 157);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void HandleTransferChatMsg()
		{
			long num = TimeUtil.NOW();
			if (num - this.LastTransferTicks >= 5000L)
			{
				this.LastTransferTicks = num;
				TCPOutPacket tcpoutPacket = null;
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					GameManager.ServerLineID,
					GameManager.ClientMgr.GetClientCount(),
					Global.SendServerHeartCount,
					GetMapcodeOnlineNumManager.CountMapIDOnlineNum()
				});
				Global.SendServerHeartCount++;
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer2(Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, 10018, strcmd, out tcpoutPacket, 0);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					LogManager.WriteLog(2, string.Format("处理转发消息时，连接DBServer获取消息列表失败", new object[0]), null, true);
				}
				else if (null != tcpoutPacket)
				{
					List<string> list = DataHelper.BytesToObject<List<string>>(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
					Global._TCPManager.TcpOutPacketPool.Push(tcpoutPacket);
					if (list != null && list.Count > 0)
					{
						for (int i = 0; i < list.Count; i++)
						{
							this.TransferChatMsg(list[i]);
						}
					}
				}
			}
		}

		public void TransferChatMsg(string chatMsg)
		{
			try
			{
				string[] array = chatMsg.Split(new char[]
				{
					':'
				});
				if (array.Length == 9)
				{
					int num = Convert.ToInt32(array[0]);
					string text = array[1];
					int num2 = Convert.ToInt32(array[2]);
					string text2 = array[3];
					int num3 = Convert.ToInt32(array[4]);
					string text3 = array[5];
					string text4 = array[6];
					int faction = Convert.ToInt32(array[7]);
					int num4 = Convert.ToInt32(array[8]);
					if (num4 != GameManager.ServerLineID)
					{
						if (!GameManager.systemGMCommands.ProcessChatMessage(null, null, text3, true))
						{
							if (num3 == 1)
							{
							}
							if (num3 == 2)
							{
								string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									num,
									text,
									0,
									text2,
									num3,
									text3,
									text4
								});
								GameManager.ClientMgr.NotifyAllChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, cmdText, null);
							}
							else if (num3 == 3)
							{
								string cmdText = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									num,
									text,
									0,
									text2,
									num3,
									text3,
									text4
								});
								GameManager.ClientMgr.NotifyFactionChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, faction, cmdText, null);
							}
							else if (num3 != 4)
							{
								if (num3 == 5)
								{
									GameManager.ClientMgr.NotifyClientChatMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, num, text, text2, num3, text3, text4);
								}
							}
						}
					}
				}
			}
			catch (Exception ex)
			{
				LogManager.WriteLog(1000, "处理GM(后台)指令异常,msg=" + ex.Message, ex, true);
			}
		}

		public void LogBatterEnemy(GameClient attacker, GameClient victim)
		{
			attacker.ClientData.RoleIDAttackebByMyself = victim.ClientData.RoleID;
			victim.ClientData.RoleIDAttackMe = attacker.ClientData.RoleID;
			GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(attacker, victim);
			GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(victim, attacker);
		}

		public int NotifyOtherInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient enemy, int burst, int injure, double injurePercnet, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, bool dontEffectDSHide = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			if ((enemy as GameClient).ClientData.CurrentLifeV > 0)
			{
				this.LogBatterEnemy(client, enemy);
				if (injure <= 0)
				{
					if (0 == attackType)
					{
						RoleAlgorithm.AttackEnemy(client, enemy as GameClient, forceBurst, injurePercnet, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
					}
					else if (1 == attackType)
					{
						RoleAlgorithm.MAttackEnemy(client, enemy as GameClient, forceBurst, injurePercnet, addInjure, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
					}
				}
				bool flag = false;
				if (injure > 0)
				{
					RoleRelifeLog roleRelifeLog = new RoleRelifeLog(client.ClientData.RoleID, client.ClientData.RoleName, client.ClientData.MapCode, string.Format("打人rid={0},rname={1}击中恢复", enemy.ClientData.RoleID, enemy.ClientData.RoleName));
					int num = (int)RoleAlgorithm.GetLifeStealV(client);
					if (num > 0 && client.ClientData.CurrentLifeV > 0 && client.ClientData.CurrentLifeV < client.ClientData.LifeV)
					{
						roleRelifeLog.hpModify = true;
						roleRelifeLog.oldHp = client.ClientData.CurrentLifeV;
						flag = true;
						client.ClientData.CurrentLifeV += num;
					}
					if (client.ClientData.CurrentLifeV > client.ClientData.LifeV)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					}
					roleRelifeLog.newHp = client.ClientData.CurrentLifeV;
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
					injure = this.InjureToEnemy(sl, pool, enemy, injure, attackType, ignoreDefenseAndDodge, skillLevel);
					injure = DBRoleBufferManager.ProcessAntiRole(client, enemy as GameClient, injure);
					injure /= 2;
				}
				if (enemy.buffManager.IsBuffEnabled(113))
				{
					BuffItemData buffItemData = enemy.buffManager.GetBuffItemData(113);
					injure = (int)((double)injure * (1.0 - buffItemData.buffValEx));
				}
				if (48 == Global.GetMapSceneType(client.ClientData.MapCode))
				{
					injure = CompManager.getInstance().FilterCompEnemyInjure(client, enemy, injure);
				}
				MapSettingItem mapSettingInfo = Data.GetMapSettingInfo(client.ClientData.MapCode);
				injure = (int)((double)injure * mapSettingInfo.NormalHuntNum);
				EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
				int num2 = GameManager.MerlinInjureMgr.CalcMerlinInjure(client, enemy, injure, ref eMerlinType);
				int num3 = RoleAlgorithm.CallAttackArmor(client, enemy, ref injure, ref num2);
				int num4 = RebornManager.getInstance().CalcRebornInjure(client, enemy, injurePercnet, baseRate, ref burst);
				injure += (int)((double)num4 * mapSettingInfo.RebornHuntNum);
				if (injure > 0)
				{
					enemy.buffManager.SetStatusBuff(114, TimeUtil.NOW(), Data.FightStateTime, 0L);
				}
				if (!GameManager.TestGamePerformanceMode || !GameManager.TestGamePerformanceLockLifeV)
				{
					(enemy as GameClient).ClientData.CurrentLifeV -= Global.GMax(0, injure + num2);
				}
				(enemy as GameClient).ClientData.CurrentLifeV = Global.GMax((enemy as GameClient).ClientData.CurrentLifeV, 0);
				if (client.ClientData.ExcellenceProp[15] > 0.0)
				{
					int randomNumber = Global.GetRandomNumber(0, 101);
					if ((double)randomNumber <= client.ClientData.ExcellenceProp[15] * 100.0)
					{
						client.ClientData.CurrentLifeV = client.ClientData.LifeV;
						flag = true;
					}
				}
				if (client.ClientData.ExcellenceProp[16] > 0.0)
				{
					int randomNumber = Global.GetRandomNumber(0, 101);
					if ((double)randomNumber <= client.ClientData.ExcellenceProp[16] * 100.0)
					{
						client.ClientData.CurrentMagicV = client.ClientData.MagicV;
						flag = true;
					}
				}
				int currentLifeV = (enemy as GameClient).ClientData.CurrentLifeV;
				(enemy as GameClient).UsingEquipMgr.InjuredSomebody(enemy as GameClient);
				this.SpriteInjure2Blood(sl, pool, client, injure);
				Point hitToGrid = new Point(-1.0, -1.0);
				if (nHitFlyDistance > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int num5 = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
					if (num5 > 0)
					{
						hitToGrid = ChuanQiUtils.HitFly(client, enemy, num5);
					}
				}
				this.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as GameClient).ClientData.RoleID, burst, injure, (double)currentLifeV, client.ClientData.Level, hitToGrid, num2, eMerlinType, num3 + 1);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.ClientData.RoleID, burst, injure, (double)currentLifeV, 0L, num2, eMerlinType, num3 + 1);
				if (!dontEffectDSHide)
				{
					if (client.ClientData.DSHideStart > 0L)
					{
						Global.RemoveBufferData(client, 41);
						client.ClientData.DSHideStart = 0L;
						GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					}
				}
				if (enemy.ClientData.DSHideStart > 0L)
				{
					Global.RemoveBufferData(enemy, 41);
					enemy.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, enemy);
				}
				if (currentLifeV <= 0)
				{
					Global.ProcessRoleDieForRoleAttack(sl, pool, client, enemy as GameClient);
				}
				GameManager.ClientMgr.ChangeRolePurpleName(sl, pool, client, enemy);
				Global.ProcessDamageThorn(sl, pool, client, enemy, injure);
				if (injure > 0)
				{
					enemy.passiveSkillModule.OnInjured(enemy);
					ZuoQiManager.getInstance().RoleDisMount(enemy, true);
				}
				if (flag)
				{
					GameManager.ClientMgr.NotifyOthersLifeChanged(sl, pool, client, true, false, 7);
				}
				GameManager.damageMonitor.Out(client);
			}
			return injure;
		}

		public void NotifyOtherInjured(SocketListener sl, TCPOutPacketPool pool, Monster monster, GameClient enemy, int burst, int injure, double injurePercnet, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, int skillLevel, double skillBaseAddPercent, double skillUpAddPercent, bool ignoreDefenseAndDodge = false, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0, int magicCode = 0, double shenShiInjurePercent = 0.0)
		{
			if (null != enemy)
			{
				if (enemy.ClientData.CurrentLifeV > 0)
				{
					enemy.ClientData.RoleIDAttackMe = monster.RoleID;
					GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(enemy, monster);
					GameManager.MonsterMgr.PetAttackMasterTargetTriggerEvent(monster, enemy);
					if (injure <= 0)
					{
						if (0 == attackType)
						{
							RoleAlgorithm.AttackEnemy(monster, enemy as GameClient, false, 1.0, 0, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
						else if (1 == attackType)
						{
							RoleAlgorithm.MAttackEnemy(monster, enemy as GameClient, false, 1.0, 0, attackPercent, addAttackMin, addAttackMax, out burst, out injure, ignoreDefenseAndDodge, baseRate, addVlue, magicCode, shenShiInjurePercent);
						}
					}
					if (injure > 0)
					{
						injure = this.InjureToEnemy(sl, pool, enemy as GameClient, injure, attackType, ignoreDefenseAndDodge, skillLevel);
						injure = (int)((double)injure * injurePercnet);
					}
					MapSettingItem mapSettingInfo = Data.GetMapSettingInfo(enemy.ClientData.MapCode);
					injure = (int)((double)injure * mapSettingInfo.NormalHuntNum);
					EMerlinSecretAttrType eMerlinType = EMerlinSecretAttrType.EMSAT_None;
					int num = GameManager.MerlinInjureMgr.CalcMerlinInjure(monster, enemy, injure, ref eMerlinType);
					int num2 = RoleAlgorithm.CallAttackArmor(monster, enemy, ref injure, ref num);
					int num3 = RebornManager.getInstance().CalcRebornInjure(monster, enemy, injurePercnet, baseRate, ref burst);
					injure += (int)((double)num3 * mapSettingInfo.RebornHuntNum);
					injure = Global.GMax(0, injure + num);
					if (enemy.buffManager.IsBuffEnabled(113))
					{
						BuffItemData buffItemData = enemy.buffManager.GetBuffItemData(113);
						injure = (int)((double)injure * (1.0 - buffItemData.buffValEx));
					}
					if (48 == Global.GetMapSceneType(enemy.ClientData.MapCode))
					{
						if (null != monster.OwnerClient)
						{
							injure = CompManager.getInstance().FilterCompEnemyInjure(monster.OwnerClient, enemy, injure);
						}
					}
					if (injure > 0)
					{
						enemy.buffManager.SetStatusBuff(114, TimeUtil.NOW(), Data.FightStateTime, 0L);
					}
					if (!GameManager.TestGamePerformanceMode || !GameManager.TestGamePerformanceLockLifeV)
					{
						(enemy as GameClient).ClientData.CurrentLifeV -= injure;
					}
					(enemy as GameClient).ClientData.CurrentLifeV = Global.GMax((enemy as GameClient).ClientData.CurrentLifeV, 0);
					int currentLifeV = (enemy as GameClient).ClientData.CurrentLifeV;
					(enemy as GameClient).UsingEquipMgr.InjuredSomebody(enemy as GameClient);
					if (currentLifeV <= 0)
					{
						if (null == monster.OwnerClient)
						{
							Global.ProcessRoleDieForMonsterAttack(sl, pool, monster, enemy);
						}
						else
						{
							Global.ProcessRoleDieForRoleAttack(sl, pool, monster.OwnerClient, enemy);
						}
					}
					Point hitToGrid = new Point(-1.0, -1.0);
					if (nHitFlyDistance > 0)
					{
						MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[(enemy as GameClient).ClientData.MapCode];
						int num4 = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
						if (num4 > 0)
						{
							hitToGrid = ChuanQiUtils.MonsterHitFly(monster, enemy, num4);
						}
					}
					if (injure > 0)
					{
						enemy.passiveSkillModule.OnInjured(enemy);
					}
					this.NotifySpriteInjured(sl, pool, enemy as GameClient, monster.MonsterZoneNode.MapCode, monster.RoleID, (enemy as GameClient).ClientData.RoleID, burst, injure, (double)currentLifeV, monster.MonsterInfo.VLevel, hitToGrid, num, eMerlinType, num2 + 1);
					Global.ProcessDamageThorn(sl, pool, monster, enemy as GameClient, injure);
				}
			}
		}

		public void SeekSpriteToLock(Monster monster)
		{
			if (monster.MonsterType == 1001)
			{
			}
			if (monster.MonsterInfo.SeekRange <= 0)
			{
				monster.VisibleItemList = null;
			}
			else if (monster.VLife <= 0.0)
			{
				monster.VisibleItemList = null;
			}
			else if (!MonsterManager.CanMonsterSeekRange(monster))
			{
				monster.VisibleItemList = null;
			}
			else
			{
				int num = (2 * (Global.MaxCache9XGridNum - 1) + 1) * (2 * (Global.MaxCache9YGridNum - 1) + 1);
				Point currentGrid = monster.CurrentGrid;
				int num2 = (int)currentGrid.X;
				int num3 = (int)currentGrid.Y;
				List<Point> searchTableList = SearchTable.GetSearchTableList();
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[monster.MonsterZoneNode.MapCode];
				monster.VisibleItemList = new List<VisibleItem>();
				int num4 = 0;
				while (num4 < num && num4 < searchTableList.Count)
				{
					int gridX = num2 + (int)searchTableList[num4].X;
					int gridY = num3 + (int)searchTableList[num4].Y;
					List<object> list = mapGrid.FindObjects(gridX, gridY);
					if (null != list)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (null != list[i] as IObject)
							{
								if (monster.GetObjectID() != (list[i] as IObject).GetObjectID())
								{
									if (monster.CopyMapID > 0)
									{
										if (monster.CopyMapID != (list[i] as IObject).CurrentCopyMapID)
										{
											goto IL_2C4;
										}
									}
									if (list[i] is GameClient)
									{
										if (!Global.RoleIsVisible(list[i] as GameClient))
										{
											goto IL_2C4;
										}
									}
									else if (list[i] is Monster)
									{
										if (1001 != monster.MonsterType)
										{
											if ((list[i] as Monster).MonsterType <= 901)
											{
												if ((list[i] as Monster).Camp <= 0)
												{
													goto IL_2C4;
												}
											}
										}
									}
									monster.VisibleItemList.Add(new VisibleItem
									{
										ItemType = (list[i] as IObject).ObjectType,
										ItemID = (list[i] as IObject).GetObjectID()
									});
								}
							}
							IL_2C4:;
						}
					}
					num4++;
				}
			}
		}

		public Point SeekMonsterPosition(GameClient client, int centerX, int centerY, int radiusGridNum, out int totalMonsterNum)
		{
			totalMonsterNum = 0;
			Point point = new Point((double)centerX, (double)centerY);
			List<object> objectsByMap = GameManager.MonsterMgr.GetObjectsByMap(client.ClientData.MapCode);
			Point result;
			if (null == objectsByMap)
			{
				result = point;
			}
			else
			{
				int num = radiusGridNum * 64;
				int num2 = int.MaxValue;
				Monster monster = null;
				int i = 0;
				while (i < objectsByMap.Count)
				{
					if (objectsByMap[i] is Monster)
					{
						Monster monster2 = objectsByMap[i] as Monster;
						if (monster2.VLife > 0.0 && monster2.Alive)
						{
							if (1201 != monster2.MonsterType)
							{
								if (Global.IsOpposition(client, monster2))
								{
									if (monster2.CopyMapID > 0)
									{
										if (monster2.CopyMapID != client.ClientData.CopyMapID)
										{
											goto IL_154;
										}
									}
									if (Global.InCircle(monster2.SafeCoordinate, point, (double)num))
									{
										totalMonsterNum++;
										int num3 = (int)Global.GetTwoPointDistance(point, monster2.SafeCoordinate);
										if (num3 < num2)
										{
											num2 = num3;
											monster = monster2;
										}
									}
								}
							}
						}
					}
					IL_154:
					i++;
					continue;
					goto IL_154;
				}
				if (null != monster)
				{
					result = new Point(monster.SafeCoordinate.X, monster.SafeCoordinate.Y);
				}
				else
				{
					result = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				}
			}
			return result;
		}

		public void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			this.LookupEnemiesInCircle(client, mapCode, toX, toY, radius, list, -1);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as GameClient).ClientData.RoleID);
			}
		}

		public void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> enemiesList, int nTargetType = -1)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (client == null || client.ClientData.RoleID != (list[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || Global.IsOpposition(client, list[i] as GameClient) || nTargetType == 2)
							{
								if (client == null || client.ClientData.CopyMapID == (list[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
									if (Global.InCircle(target, center, (double)radius))
									{
										enemiesList.Add(list[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircle(int mapCode, int copyMapCode, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (copyMapCode == (list[i] as GameClient).ClientData.CopyMapID)
						{
							Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
							if (Global.InCircle(target, center, (double)radius))
							{
								enemiesList.Add(list[i]);
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> list = new List<object>();
			this.LookupEnemiesInCircleByAngle(client, direction, mapCode, toX, toY, radius, list, angle, near180);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as GameClient).ClientData.RoleID);
			}
		}

		public void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeAngle((double)client.ClientData.RoleYAngle, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (client.ClientData.RoleID != (list[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || Global.IsOpposition(client, list[i] as GameClient))
							{
								if (client == null || client.ClientData.CopyMapID == (list[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
									if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
									{
										enemiesList.Add(list[i]);
									}
									else if (Global.InCircle(target, center, 100.0))
									{
										enemiesList.Add(list[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapCode, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> list = new List<object>();
			this.LookupEnemiesInCircleByAngle(direction, mapCode, copyMapCode, toX, toY, radius, list, angle, near180);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as GameClient).ClientData.RoleID);
			}
		}

		public void LookupEnemiesInCircleByAngle(int direction, int mapCode, int copyMapCode, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				double num = 0.0;
				double num2 = 0.0;
				Global.GetAngleRangeByDirection(direction, 360.0, out num, out num2);
				int num3 = 100;
				if (JingJiChangManager.getInstance().IsJingJiChangMap(mapCode))
				{
					num3 = 200;
				}
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (copyMapCode == (list[i] as GameClient).ClientData.CopyMapID)
						{
							Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
							if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
							{
								enemiesList.Add(list[i]);
							}
							else if (Global.InCircle(target, center, (double)num3))
							{
								enemiesList.Add(list[i]);
							}
						}
					}
				}
			}
		}

		public void LookupRolesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (client == null || client.ClientData.RoleID != (list[i] as GameClient).ClientData.RoleID)
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as GameClient).ClientData.CopyMapID)
							{
								Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
								if (Global.InCircle(target, center, (double)radius))
								{
									rolesList.Add(list[i]);
								}
							}
						}
					}
				}
			}
		}

		public void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(client.ClientData.PosX, client.ClientData.PosY, radius);
			if (null != list)
			{
				Point center = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				Point apointInCircle = Global.GetAPointInCircle(center, radius, client.ClientData.RoleYAngle);
				int num = (int)apointInCircle.X;
				int num2 = (int)apointInCircle.Y;
				Point center2 = default(Point);
				center2.X = (double)((client.ClientData.PosX + num) / 2);
				center2.Y = (double)((client.ClientData.PosY + num2) / 2);
				int directionX = num - client.ClientData.PosX;
				int directionY = num2 - client.ClientData.PosY;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if ((list[i] as GameClient).ClientData.LifeV > 0)
						{
							if (client == null || client.ClientData.RoleID != (list[i] as GameClient).ClientData.RoleID)
							{
								if (client == null || client.ClientData.CopyMapID == (list[i] as GameClient).ClientData.CopyMapID)
								{
									Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
									if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
									{
										rolesList.Add(list[i]);
									}
									else if (Global.InCircle(target, center, 100.0))
									{
										rolesList.Add(list[i]);
									}
								}
							}
						}
					}
				}
			}
		}

		public void LookupRolesInSquare(int mapCode, int copyMapId, int srcX, int srcY, int toX, int toY, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(srcX, srcY, radius);
			if (null != list)
			{
				Point center = new Point((double)srcX, (double)srcY);
				Point center2 = default(Point);
				center2.X = (double)((srcX + toX) / 2);
				center2.Y = (double)((srcY + toY) / 2);
				int directionX = toX - srcX;
				int directionY = toY - srcY;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if ((list[i] as GameClient).ClientData.LifeV > 0)
						{
							if (copyMapId == (list[i] as GameClient).ClientData.CopyMapID)
							{
								Point target = new Point((double)(list[i] as GameClient).ClientData.PosX, (double)(list[i] as GameClient).ClientData.PosY);
								if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
								{
									rolesList.Add(list[i]);
								}
								else if (Global.InCircle(target, center, 100.0))
								{
									rolesList.Add(list[i]);
								}
							}
						}
					}
				}
			}
		}

		public void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			List<object> list = mapGrid.FindObjects(gridX, gridY);
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (list[i] as GameClient).ClientData.CopyMapID)
						{
							enemiesList.Add(list[i]);
						}
					}
				}
			}
		}

		public void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			Point currentGrid = attacker.CurrentGrid;
			int gridX = (int)currentGrid.X;
			int gridY = (int)currentGrid.Y;
			Point gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY);
			this.LookupEnemiesAtGridXY(attacker, (int)gridPointByDirection.X, (int)gridPointByDirection.Y, enemiesList);
		}

		public void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			this.LookupAttackEnemies(attacker, direction, list);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as GameClient).ClientData.RoleID);
			}
		}

		public void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridPointByDirection.Count > 0)
			{
				for (int i = 0; i < gridPointByDirection.Count; i++)
				{
					this.LookupEnemiesAtGridXY(obj, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, enemiesList);
				}
			}
		}

		public int InjureToEnemy(SocketListener sl, TCPOutPacketPool pool, GameClient enemy, int injured, int attackType, bool ignoreDefenseAndDodge, int skillLevel)
		{
			double num = 0.0;
			num += enemy.RoleMagicHelper.GetSubInjure();
			num += enemy.RoleMagicHelper.MU_GetSubInjure2();
			injured = (int)((double)injured - num);
			injured = (int)((double)injured * (1.0 - enemy.RoleMagicHelper.GetSubInjure1()) * (1.0 - enemy.RoleMagicHelper.MU_GetSubInjure1()));
			int result;
			if (injured <= 0)
			{
				result = 0;
			}
			else
			{
				double num2 = enemy.RoleMagicHelper.GetInjure2Magic();
				if (num2 > 0.0)
				{
					double num3 = num2 * (double)injured;
					num3 = Global.GMin(num3, (double)injured);
					num3 = Global.GMin((double)enemy.ClientData.CurrentMagicV, num3);
					injured -= (int)num3;
					this.SubSpriteMagicV(sl, pool, enemy, num3);
				}
				double num4 = enemy.RoleMagicHelper.GetNewInjure2Magic();
				if (num4 > 0.0)
				{
					num4 = Global.GMin(num4, (double)injured);
					num4 = Global.GMin((double)enemy.ClientData.CurrentMagicV, num4);
					injured -= (int)num4;
					this.SubSpriteMagicV(sl, pool, enemy, num4);
				}
				num2 = enemy.RoleMagicHelper.GetNewInjure2Magic3();
				if (num2 > 0.0)
				{
					double num3 = num2 * (double)injured;
					num3 = Global.GMin(num3, (double)injured);
					num3 = Global.GMin((double)enemy.ClientData.CurrentMagicV, num3);
					injured -= (int)num3;
					this.SubSpriteMagicV(sl, pool, enemy, num3);
				}
				num2 = enemy.RoleMagicHelper.GetNewMagicSubInjure();
				if (num2 > 0.0)
				{
					if (0 == attackType)
					{
						if (ignoreDefenseAndDodge)
						{
							skillLevel = Math.Min(skillLevel, ClientManager.IgnoreDefenseAndDogeSubPercent.Length - 1);
							skillLevel = Math.Max(0, skillLevel);
							num2 = Math.Min(num2, ClientManager.IgnoreDefenseAndDogeSubPercent[skillLevel]);
						}
					}
					double num3 = num2 * (double)injured;
					num3 = Global.GMin(num3, (double)injured);
					num3 = Global.GMin((double)enemy.ClientData.CurrentMagicV, num3);
					injured -= (int)num3;
				}
				injured = DBRoleBufferManager.ProcessHuZhaoSubLifeV(enemy, Math.Max(0, injured));
				injured = DBRoleBufferManager.ProcessWuDiHuZhaoNoInjured(enemy, Math.Max(0, injured));
				result = Math.Max(0, injured);
			}
			return result;
		}

		public void SpriteInjure2Blood(SocketListener sl, TCPOutPacketPool pool, GameClient client, int injured)
		{
			double injure2Life = client.RoleMagicHelper.GetInjure2Life();
			if (0.0 < injure2Life)
			{
				injured = (int)((double)injured * injure2Life);
				this.AddSpriteLifeV(sl, pool, client, (double)injured, "击中恢复");
			}
		}

		public bool NotifyChangeMap(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toMapCode, int maxX = -1, int mapY = -1, int direction = -1, int relife = 0)
		{
			bool flag = KuaFuManager.getInstance().IsKuaFuMap(toMapCode);
			if (client.CheckCheatData.GmGotoShadowMapCode != toMapCode && client.ClientData.WaitingChangeMapToMapCode != toMapCode)
			{
				if (client.ClientSocket.IsKuaFuLogin && client.ClientData.KuaFuChangeMapCode != toMapCode)
				{
					KuaFuManager.getInstance().GotoLastMap(client);
					return true;
				}
				if (client.ClientSocket.IsKuaFuLogin != flag)
				{
					LogManager.WriteLog(2, string.Format("GotoMap denied, mapCode={0},IsKuaFuLogin={1}", toMapCode, client.ClientSocket.IsKuaFuLogin), null, true);
					return false;
				}
				if (LingDiCaiJiManager.getInstance().GetLingDiType(toMapCode) != 2)
				{
					return false;
				}
			}
			client.ClientData.WaitingNotifyChangeMap = true;
			client.ClientData.WaitingChangeMapToMapCode = toMapCode;
			client.ClientData.WaitingChangeMapToPosX = maxX;
			client.ClientData.WaitingChangeMapToPosY = mapY;
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastNotifyChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							toMapCode,
							maxX,
							mapY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastNotifyChangeMapTicks = TimeUtil.NOW();
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
			{
				client.ClientData.RoleID,
				toMapCode,
				maxX,
				mapY,
				direction,
				relife
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 160);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
			return true;
		}

		public bool ChangeMap(SocketListener sl, TCPOutPacketPool pool, GameClient client, int teleport, int toMapCode, int toMapX, int toMapY, int toMapDirection, int nID)
		{
			if ("1" == GameManager.GameConfigMgr.GetGameConfigItemStr("log-changmap", "0"))
			{
				if (client.ClientData.LastChangeMapTicks >= TimeUtil.NOW() - 12000L)
				{
					try
					{
						DataHelper.WriteStackTraceLog(string.Format("地图传送频繁,记录堆栈信息备查 role={3}({4}) toMapCode={0} pt=({1},{2})", new object[]
						{
							toMapCode,
							toMapX,
							toMapY,
							client.ClientData.RoleName,
							client.ClientData.RoleID
						}));
					}
					catch (Exception)
					{
					}
				}
			}
			client.ClientData.LastChangeMapTicks = TimeUtil.NOW();
			client.ClientData.SceneType = 9999;
			client.ClientData.SceneMapCode = 0;
			GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
			if (toMapCode > 0)
			{
				GameMap gameMap = GameManager.MapMgr.GetGameMap(toMapCode);
				if (null != gameMap)
				{
					if (!gameMap.CanMove(toMapX / gameMap.MapGridWidth, toMapY / gameMap.MapGridHeight))
					{
						toMapX = -1;
						toMapY = -1;
					}
				}
				else
				{
					toMapCode = -1;
				}
			}
			if (teleport >= 0)
			{
				Global.HandleBiaoCheChangMap(client, toMapCode, toMapX, toMapY, toMapDirection);
			}
			List<object> all9Clients = Global.GetAll9Clients(client);
			GameManager.ClientMgr.NotifyOthersLeave(sl, pool, client, all9Clients);
			if (client.ClientData.MapCode != toMapCode)
			{
				GlobalEventSource4Scene.getInstance().fireEvent(new EventObjectEx(65, new object[]
				{
					client,
					client.ClientData.MapCode,
					toMapCode
				}), 10000);
			}
			if (client.ClientData.CopyMapID > 0 && client.ClientData.FuBenSeqID > 0)
			{
				if (!client.ClientSocket.IsKuaFuLogin || Global.GetMapSceneType(client.ClientData.MapCode) != Global.GetMapSceneType(toMapCode))
				{
					int num = FuBenManager.FindFuBenIDByMapCode(toMapCode);
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(client.ClientData.FuBenSeqID);
					if (fuBenInfoItem != null && num != fuBenInfoItem.FuBenID)
					{
						GlobalEventSource.getInstance().fireEvent(new PlayerLeaveFuBenEventObject(client));
						SceneUIClasses mapSceneType = Global.GetMapSceneType(client.ClientData.MapCode);
						KuaFuManager.getInstance().OnLeaveScene(client, mapSceneType, false);
					}
					if (-1 == num)
					{
						if (GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(client.ClientData.FuBenID))
						{
							GameManager.BloodCastleCopySceneMgr.LeaveBloodCastCopyScene(client, true);
						}
						else if (GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(client.ClientData.FuBenID))
						{
							GameManager.DaimonSquareCopySceneMgr.LeaveDaimonSquareCopyScene(client, true);
						}
						client.ClientData.FuBenSeqID = -1;
						client.ClientData.FuBenID = -1;
						FuBenManager.RemoveFuBenSeqID(client.ClientData.RoleID);
					}
					else if (null != fuBenInfoItem)
					{
						if (num != fuBenInfoItem.FuBenID)
						{
							LogManager.WriteLog(1, string.Format("外挂利用直接切换地图的操作，进行副本地图切换: Cmd={0}, RoleID={1}, 关闭连接", (TCPGameServerCmds)nID, client.ClientData.RoleID), null, true);
							return false;
						}
					}
				}
			}
			if (ZhuanShengShiLian.IsZhuanShengShiLianCopyScene(client.ClientData.MapCode))
			{
				Global.QuitFromTeam(client);
			}
			Global.ClearCopyMap(client, false);
			GameManager.ClientMgr.RemoveClientFromContainer(client);
			if (toMapX <= 0 || toMapY <= 0)
			{
				int defaultBirthPosX = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosX;
				int defaultBirthPosY = GameManager.MapMgr.DictMaps[toMapCode].DefaultBirthPosY;
				int birthRadius = GameManager.MapMgr.DictMaps[toMapCode].BirthRadius;
				if (Global.IsHuangChengMapCode(toMapCode))
				{
					Global.GetHuangChengMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref birthRadius);
				}
				else if (toMapCode == GameManager.BattleMgr.BattleMapCode)
				{
					Global.GetLastBattleSideInfo(client);
					Global.GetBattleMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref birthRadius);
				}
				Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, toMapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
				toMapX = (int)mapPoint.X;
				toMapY = (int)mapPoint.Y;
			}
			if (client.ClientData.MapCode == GameManager.BattleMgr.BattleMapCode && toMapCode != GameManager.BattleMgr.BattleMapCode)
			{
				GameManager.BattleMgr.LeaveBattleMap(client, true);
			}
			else if (client.ClientData.MapCode == GameManager.ArenaBattleMgr.BattleMapCode && toMapCode != GameManager.ArenaBattleMgr.BattleMapCode)
			{
				GameManager.ArenaBattleMgr.LeaveArenaBattleMap(client);
			}
			else if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhenMap(client.ClientData.MapCode))
			{
				LuoLanFaZhenCopySceneManager.OnLeaveFubenMap(client, toMapCode);
			}
			else if (JingJiChangManager.getInstance().IsJingJiChangMap(client.ClientData.MapCode))
			{
				this.UserFullLife(client, "离开竞技场", false);
			}
			else if (client.ClientData.MapCode == GameManager.AngelTempleMgr.m_AngelTempleData.MapCode && toMapCode != GameManager.AngelTempleMgr.m_AngelTempleData.MapCode)
			{
				GameManager.AngelTempleMgr.LeaveAngelTempleScene(client, false);
			}
			Global.RemoveBufferData(client, 85);
			Global.RemoveBufferData(client, 86);
			Global.ProcessLeaveGuMuMap(client);
			if (!WanMotaCopySceneManager.IsWanMoTaMapCode(client.ClientData.MapCode) && 44 != Global.GetMapSceneType(client.ClientData.MapCode))
			{
				client.ClientData.LastMapCode = client.ClientData.MapCode;
				client.ClientData.LastPosX = client.ClientData.PosX;
				client.ClientData.LastPosY = client.ClientData.PosY;
			}
			client.ClientData.WaitingForChangeMap = true;
			int mapCode = client.ClientData.MapCode;
			client.ClientData.MapCode = toMapCode;
			client.ClientData.PosX = toMapX;
			client.ClientData.PosY = toMapY;
			client.ClientData.ReportPosTicks = 0L;
			client.ClientData.CurrentAction = 0;
			client.ClearVisibleObjects(true);
			client.ClientData.DestPoint = new Point(-1.0, -1.0);
			if (toMapDirection > 0)
			{
				client.ClientData.RoleDirection = toMapDirection;
			}
			else
			{
				toMapDirection = client.ClientData.RoleDirection;
			}
			Global.InitCopyMap(client);
			GameManager.ClientMgr.AddClientToContainer(client);
			SceneUIClasses mapSceneType2 = Global.GetMapSceneType(mapCode);
			if (mapSceneType2 == 21)
			{
				client.ClientData.ShuiJingHuanJingTicks = TimeUtil.NOW() * 10000L;
			}
			bool result;
			if (!GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].MoveObject(-1, -1, client.ClientData.PosX, client.ClientData.PosY, client))
			{
				LogManager.WriteLog(1, string.Format("精灵移动超出了地图边界: Cmd={0}, RoleID={1}, 关闭连接", (TCPGameServerCmds)nID, client.ClientData.RoleID), null, true);
				result = false;
			}
			else
			{
				TeamData teamData = GameManager.TeamMgr.FindData(client.ClientData.TeamID);
				if (null != teamData)
				{
					if (null != teamData.TeamRoles)
					{
						for (int i = 0; i < teamData.TeamRoles.Count; i++)
						{
							if (teamData.TeamRoles[i].RoleID == client.ClientData.RoleID)
							{
								teamData.TeamRoles[i].MapCode = toMapCode;
								break;
							}
						}
						GameManager.ClientMgr.NotifyTeamData(sl, pool, teamData);
					}
				}
				Global.BroadcastEnterLaoFangHint(client, toMapCode);
				if (client.ClientData.MapCode != GameManager.BattleMgr.BattleMapCode)
				{
					if (LuoLanFaZhenCopySceneManager.IsLuoLanFaZhenMap(client.ClientData.MapCode))
					{
						LuoLanFaZhenCopySceneManager.OnEnterFubenMap(client, mapCode, false);
					}
				}
				Global.ProcessLimitFuBenMapNotifyMsg(client);
				client.ClearChangeGrid();
				Global.AddMapEvent(client);
				ClientCmdCheck.RecordClientPosition(client);
				client.CheckCheatData.LastNotifyLeaveGuMuTick = 0L;
				Monster petMonsterByMonsterByType = Global.GetPetMonsterByMonsterByType(client, MonsterTypes.DSPetMonster);
				if (null != petMonsterByMonsterByType)
				{
					Global.SystemKillSummonMonster(client, MonsterTypes.DSPetMonster);
					GameManager.LuaMgr.CallMonstersForGameClient(client, petMonsterByMonsterByType.MonsterInfo.ExtensionID, petMonsterByMonsterByType.CurrentMagicLevel, petMonsterByMonsterByType.SurvivalTime / 1000, 1001, 1);
				}
				SCMapChange cmdData = new SCMapChange(client.ClientData.RoleID, teleport, toMapCode, toMapX, toMapY, toMapDirection, 0);
				client.sendCmd<SCMapChange>(123, cmdData, false);
				result = true;
			}
			return result;
		}

		public bool ChangePosition(SocketListener sl, TCPOutPacketPool pool, GameClient client, int toMapX, int toMapY, int toMapDirection, int nID, int animation = 0)
		{
			if (2 != animation)
			{
				GameManager.ClientMgr.StopClientStoryboard(client, 0L, -1, -1);
				if (toMapX <= 0 || toMapY <= 0)
				{
					int defaultBirthPosX = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosX;
					int defaultBirthPosY = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].DefaultBirthPosY;
					int birthRadius = GameManager.MapMgr.DictMaps[client.ClientData.MapCode].BirthRadius;
					Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
					toMapX = (int)mapPoint.X;
					toMapY = (int)mapPoint.Y;
				}
				int posX = client.ClientData.PosX;
				int posY = client.ClientData.PosY;
				client.ClientData.PosX = toMapX;
				client.ClientData.PosY = toMapY;
				client.ClientData.ReportPosTicks = 0L;
				if (toMapDirection > 0)
				{
					client.ClientData.RoleDirection = toMapDirection;
				}
				if (!GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode].MoveObject(-1, -1, client.ClientData.PosX, client.ClientData.PosY, client))
				{
					client.ClientData.PosX = posX;
					client.ClientData.PosY = posY;
					client.ClientData.ReportPosTicks = 0L;
				}
				ClientManager.DoSpriteMapGridMove(client, 0);
				ClientCmdCheck.RecordClientPosition(client);
			}
			List<object> all9Clients = Global.GetAll9Clients(client);
			bool result;
			if (null == all9Clients)
			{
				result = true;
			}
			else
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					toMapX,
					toMapY,
					toMapDirection,
					animation
				});
				this.SendToClients(sl, pool, null, all9Clients, strCmd, nID);
				result = true;
			}
			return result;
		}

		public bool ChangePosition2(SocketListener sl, TCPOutPacketPool pool, IObject obj, int roleID, int mapCode, int copyMapID, int toMapX, int toMapY, int toMapDirection, List<object> objsList)
		{
			int cmdID = 159;
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, toMapX, toMapY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			bool result;
			if (objsList == null)
			{
				result = true;
			}
			else
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					roleID,
					toMapX,
					toMapY,
					toMapDirection
				});
				this.SendToClients(sl, pool, null, objsList, strCmd, cmdID);
				result = true;
			}
			return result;
		}

		public void NotifySelfAddGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int nNewHint, int isPackUp = 0)
		{
			AddGoodsData instance = new AddGoodsData
			{
				roleID = client.ClientData.RoleID,
				id = goodsData.Id,
				goodsID = goodsData.GoodsID,
				forgeLevel = goodsData.Forge_level,
				quality = goodsData.Quality,
				goodsNum = goodsData.GCount,
				binding = goodsData.Binding,
				site = goodsData.Site,
				jewellist = goodsData.Jewellist,
				newHint = nNewHint,
				newEndTime = goodsData.Endtime,
				addPropIndex = goodsData.AddPropIndex,
				bornIndex = goodsData.BornIndex,
				lucky = goodsData.Lucky,
				strong = goodsData.Strong,
				ExcellenceProperty = goodsData.ExcellenceInfo,
				nAppendPropLev = goodsData.AppendPropLev,
				ChangeLifeLevForEquip = goodsData.ChangeLifeLevForEquip,
				bagIndex = goodsData.BagIndex,
				washProps = goodsData.WashProps,
				ElementhrtsProps = goodsData.ElementhrtsProps,
				juHunLevel = goodsData.JuHunID,
				PackUp = isPackUp
			};
			byte[] array = DataHelper.ObjectToBytes<AddGoodsData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 130);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySelfAddGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int id, int goodsID, int forgeLevel, int quality, int goodsNum, int binding, int site, string jewellist, int newHint, string newEndTime, int addPropIndex, int bornIndex, int lucky, int strong, int ExcellenceProperty, int nAppendPropLev, int ChangeLifeLevForEquip = 0, int bagIndex = 0, List<int> washProps = null, List<int> elementhrtsProps = null, int juHun_level = 0, string prop = "")
		{
			newEndTime = newEndTime.Replace(":", "$");
			AddGoodsData instance = new AddGoodsData
			{
				roleID = client.ClientData.RoleID,
				id = id,
				goodsID = goodsID,
				forgeLevel = forgeLevel,
				quality = quality,
				goodsNum = goodsNum,
				binding = binding,
				site = site,
				jewellist = jewellist,
				newHint = newHint,
				newEndTime = newEndTime,
				addPropIndex = addPropIndex,
				bornIndex = bornIndex,
				lucky = lucky,
				strong = strong,
				ExcellenceProperty = ExcellenceProperty,
				nAppendPropLev = nAppendPropLev,
				ChangeLifeLevForEquip = ChangeLifeLevForEquip,
				bagIndex = bagIndex,
				washProps = washProps,
				ElementhrtsProps = elementhrtsProps,
				juHunLevel = juHun_level,
				prop = prop
			};
			byte[] array = DataHelper.ObjectToBytes<AddGoodsData>(instance);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, array, 0, array.Length, 130);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyModGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, int modType, int id, int isusing, int site, int gcount, int bagIndex, int newHint)
		{
			SCModGoods cmdData = new SCModGoods(0, modType, id, isusing, site, gcount, bagIndex, newHint);
			client.sendCmd<SCModGoods>(131, cmdData, false);
		}

		public void NotifyMoveGoods(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData gd, int moveType)
		{
			if (0 == moveType)
			{
				string data = string.Format("{0}:{1}", client.ClientData.RoleID, gd.Id);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 172);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
			else
			{
				GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 0, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
			}
		}

		public void NotifyGoodsInfo(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData gd)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, gd.Id, gd.Lucky);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 426);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, bool usingGoods, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, 1, usingGoods, dontCalcLimitNum);
		}

		public bool NotifyUseGoodsByDbId(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int useCount, bool usingGoods, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, useCount, usingGoods, dontCalcLimitNum);
		}

		public bool NotifyUseGoodsByDbId(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int dbID, int useCount, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			GoodsData goodsData = RebornEquip.GetRebornGoodsByDbID(client, dbID);
			if (null == goodsData)
			{
				goodsData = Global.GetGoodsByDbID(client, dbID);
				if (null != goodsData)
				{
				}
			}
			return this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData.GoodsID, useCount, usingGoods, out usedBinding, out usedTimeLimited, dontCalcLimitNum);
		}

		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int useCount, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool result = false;
			lock (client.ClientData.GoodsDataList)
			{
				if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
				{
					return result;
				}
				if (!usedBinding)
				{
					usedBinding = (goodsData.Binding > 0);
				}
				if (!usedTimeLimited)
				{
					usedTimeLimited = Global.IsTimeLimitGoods(goodsData);
				}
				result = this.NotifyUseGoods(sl, tcpClientPool, pool, client, goodsData, useCount, usingGoods, dontCalcLimitNum);
			}
			return result;
		}

		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData, int subNum, bool usingGoods, bool dontCalcLimitNum = false)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else
			{
				if (!dontCalcLimitNum)
				{
					if (!Global.HasEnoughGoodsDayUseNum(client, goodsData.GoodsID, subNum))
					{
						return false;
					}
				}
				if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
				{
					result = false;
				}
				else if (goodsData.GCount <= 0)
				{
					result = false;
				}
				else if (goodsData.GCount < subNum)
				{
					result = false;
				}
				else if (subNum <= 0)
				{
					result = false;
				}
				else
				{
					List<MagicActionItem> list = null;
					int categoriy = 0;
					if (usingGoods)
					{
						int num = UsingGoods.ProcessUsingGoodsVerify(client, goodsData.GoodsID, goodsData.Binding, out list, out categoriy, subNum);
						if (num < 0)
						{
							return false;
						}
						if (num == 0)
						{
							for (int i = 0; i < list.Count; i++)
							{
								if (list[i].MagicActionID == MagicActionIDs.UP_LEVEL)
								{
									int num2 = 0;
									int num3 = (int)list[i].MagicActionParams[0];
									bool flag = true;
									if (num3 > 0)
									{
										if (client.ClientData.ChangeLifeCount > GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
										{
											flag = false;
										}
										else if (client.ClientData.ChangeLifeCount == GameManager.ChangeLifeMgr.m_MaxChangeLifeCount)
										{
											ChangeLifeDataInfo changeLifeDataInfo = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, 0);
											if (changeLifeDataInfo == null)
											{
												flag = false;
											}
											else
											{
												num2 = changeLifeDataInfo.NeedLevel;
												if (client.ClientData.Level >= num2)
												{
													flag = false;
												}
											}
										}
										else
										{
											ChangeLifeDataInfo changeLifeDataInfo = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, client.ClientData.ChangeLifeCount + 1);
											if (changeLifeDataInfo == null)
											{
												flag = false;
											}
											else
											{
												num2 = changeLifeDataInfo.NeedLevel;
												if (client.ClientData.Level >= num2)
												{
													flag = false;
												}
											}
										}
										if (!flag)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(64, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
										if (client.ClientData.Level + num3 > num2)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(65, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
										if (client.ClientData.CurrentLifeV <= 0)
										{
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(66, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 40);
											return false;
										}
									}
								}
								else if (list[i].MagicActionID == MagicActionIDs.ADD_GOODWILL)
								{
									if (!MarriageOtherLogic.getInstance().CanAddMarriageGoodWill(client))
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(67, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										return false;
									}
								}
								else if (list[i].MagicActionID == MagicActionIDs.MU_GETSHIZHUANG)
								{
									int nFashionID = (int)list[i].MagicActionParams[0];
									if (!FashionManager.getInstance().FashionCanAdd(client, nFashionID))
									{
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(68, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
										return false;
									}
								}
							}
						}
						else if (num == 1)
						{
							usingGoods = false;
						}
					}
					int num4 = goodsData.GCount;
					num4 = goodsData.GCount - subNum;
					string[] array = null;
					string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
					{
						client.ClientData.RoleID,
						goodsData.Id,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						num4,
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*",
						"*"
					});
					TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array, client.ServerId);
					if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
					{
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-1, goodsData.Id, num4), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = false;
					}
					else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
					{
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-2, goodsData.Id, num4), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = false;
					}
					else
					{
						if (num4 > 0)
						{
							goodsData.GCount = num4;
						}
						else if (3000 == goodsData.Site || 3001 == goodsData.Site)
						{
							goodsData.GCount = 0;
							ElementhrtsManager.RemoveElementhrtsData(client, goodsData);
						}
						else if (7000 == goodsData.Site)
						{
							goodsData.GCount = 0;
							GameManager.FluorescentGemMgr.RemoveFluorescentGemData(client, goodsData);
						}
						else if (8000 == goodsData.Site)
						{
							goodsData.GCount = 0;
							SingletonTemplate<SoulStoneManager>.Instance().RemoveSoulStoneGoods(client, goodsData, goodsData.Site);
						}
						else if (11000 == goodsData.Site)
						{
							goodsData.GCount = num4;
							ShenShiManager.UpdateFuWenGoodsData(client, goodsData);
						}
						else if (goodsData.Site == 16000)
						{
							goodsData.GCount = 0;
							MountHolyStampManager.RemoveGoodsData(client, goodsData);
						}
						else
						{
							goodsData.GCount = 0;
							if (RebornEquip.IsRebornType(goodsData.GoodsID))
							{
								RebornEquip.RemoveGoodsData(client, goodsData);
							}
							else
							{
								Global.RemoveGoodsData(client, goodsData);
							}
						}
						if (usingGoods)
						{
							UsingGoods.ProcessUsingGoods(client, goodsData.GoodsID, goodsData.Binding, list, categoriy, subNum);
						}
						if (!dontCalcLimitNum)
						{
							Global.AddGoodsLimitNum(client, goodsData.GoodsID, subNum);
						}
						Global.ModRoleGoodsEvent(client, goodsData, -subNum, "物品使用", false);
						EventLogManager.AddGoodsEvent(client, OpTypes.AddOrSub, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, -subNum, goodsData.GCount, "物品使用");
						SevenDayGoalEventObject sevenDayGoalEventObject = SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.UseGoodsCount);
						sevenDayGoalEventObject.Arg1 = goodsData.GoodsID;
						sevenDayGoalEventObject.Arg2 = subNum;
						GlobalEventSource.getInstance().fireEvent(sevenDayGoalEventObject);
						TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(0, goodsData.Id, num4), pool, 158);
						if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
						{
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool NotifyUseGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool flag = false;
			int num = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (!usedBinding)
							{
								usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
							}
							if (!usedTimeLimited)
							{
								usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
							}
							int gcount = client.ClientData.GoodsDataList[i].GCount;
							int num2 = Global.GMin(gcount, totalNum - num);
							flag = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], num2, usingGoods, dontCalcLimitNum);
							if (!flag)
							{
								break;
							}
							num += num2;
							if (num >= totalNum)
							{
								break;
							}
							if (num2 >= gcount)
							{
								i--;
							}
						}
					}
					IL_160:
					i++;
					continue;
					goto IL_160;
				}
			}
			return flag;
		}

		public bool NotifyUseBindGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool flag = false;
			int num = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (client.ClientData.GoodsDataList[i].Binding >= 1)
							{
								if (!usedBinding)
								{
									usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
								}
								if (!usedTimeLimited)
								{
									usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
								}
								int gcount = client.ClientData.GoodsDataList[i].GCount;
								int num2 = Global.GMin(gcount, totalNum - num);
								flag = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], num2, usingGoods, dontCalcLimitNum);
								if (!flag)
								{
									break;
								}
								num += num2;
								if (num >= totalNum)
								{
									break;
								}
								if (num2 >= gcount)
								{
									i--;
								}
							}
						}
					}
					IL_189:
					i++;
					continue;
					goto IL_189;
				}
			}
			return flag;
		}

		public bool NotifyUseNotBindGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int goodsID, int totalNum, bool usingGoods, out bool usedBinding, out bool usedTimeLimited, bool dontCalcLimitNum = false)
		{
			usedBinding = false;
			usedTimeLimited = false;
			bool flag = false;
			int num = 0;
			lock (client.ClientData.GoodsDataList)
			{
				int i = 0;
				while (i < client.ClientData.GoodsDataList.Count)
				{
					if (client.ClientData.GoodsDataList[i].GoodsID == goodsID)
					{
						if (!Global.IsGoodsTimeOver(client.ClientData.GoodsDataList[i]) && !Global.IsGoodsNotReachStartTime(client.ClientData.GoodsDataList[i]))
						{
							if (client.ClientData.GoodsDataList[i].Binding <= 0)
							{
								if (!usedBinding)
								{
									usedBinding = (client.ClientData.GoodsDataList[i].Binding > 0);
								}
								if (!usedTimeLimited)
								{
									usedTimeLimited = Global.IsTimeLimitGoods(client.ClientData.GoodsDataList[i]);
								}
								int gcount = client.ClientData.GoodsDataList[i].GCount;
								int num2 = Global.GMin(gcount, totalNum - num);
								flag = this.NotifyUseGoods(sl, tcpClientPool, pool, client, client.ClientData.GoodsDataList[i], num2, usingGoods, dontCalcLimitNum);
								if (!flag)
								{
									break;
								}
								num += num2;
								if (num >= totalNum)
								{
									break;
								}
								if (num2 >= gcount)
								{
									i--;
								}
							}
						}
					}
					IL_189:
					i++;
					continue;
					goto IL_189;
				}
			}
			return flag;
		}

		public bool FallRoleGoods(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, GoodsData goodsData)
		{
			bool result;
			if (null == goodsData)
			{
				result = false;
			}
			else if (Global.IsGoodsTimeOver(goodsData) || Global.IsGoodsNotReachStartTime(goodsData))
			{
				result = false;
			}
			else if (goodsData.GCount <= 0)
			{
				result = false;
			}
			else
			{
				int num = goodsData.GCount;
				int num2 = 1;
				if (Global.GetGoodsDefaultCount(goodsData.GoodsID) > 1)
				{
					num2 = goodsData.GCount;
				}
				num = goodsData.GCount - num2;
				string[] array = null;
				string strcmd = Global.FormatUpdateDBGoodsStr(new object[]
				{
					client.ClientData.RoleID,
					goodsData.Id,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					num,
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*",
					"*"
				});
				TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array, client.ServerId);
				if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-1, goodsData.Id, num), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = false;
				}
				else if (array.Length <= 0 || Convert.ToInt32(array[1]) < 0)
				{
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(-2, goodsData.Id, num), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = false;
				}
				else
				{
					if (num > 0)
					{
						goodsData.GCount = num;
					}
					else
					{
						goodsData.GCount = 0;
						Global.RemoveGoodsData(client, goodsData);
					}
					Global.ModRoleGoodsEvent(client, goodsData, -num2, "物品掉落", false);
					EventLogManager.AddGoodsEvent(client, OpTypes.AddOrSub, OpTags.None, goodsData.GoodsID, (long)goodsData.Id, -num2, goodsData.GCount, "物品掉落");
					TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SC_SprUseGoods>(new SC_SprUseGoods(0, goodsData.Id, num), pool, 158);
					if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
					{
					}
					result = true;
				}
			}
			return result;
		}

		public void NotifySelfPropertyValue(GameClient client, int moneyType, long value)
		{
			client.sendCmd<SCPropertyChange>(719, new SCPropertyChange
			{
				RoleID = client.ClientData.RoleID,
				MoneyType = moneyType,
				Value = value
			}, false);
		}

		public void NotifyOthersPropertyValue(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			throw new NotImplementedException();
		}

		public void NotifySelfMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Money1, client.ClientData.Money2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 138);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddMoney1(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addMoney, string strFrom, bool writeToDB = true)
		{
			int money = client.ClientData.Money1;
			if (addMoney > 0)
			{
				if (money >= 1000000000)
				{
					return this.AddUserStoreMoney(sl, tcpClientPool, pool, client, (long)addMoney, strFrom, false);
				}
				if (money + addMoney > 1000000000)
				{
					long addMoney2 = (long)money + (long)addMoney - 1000000000L;
					addMoney = Global.GMax(0, 1000000000 - money);
					this.AddUserStoreMoney(sl, tcpClientPool, pool, client, addMoney2, strFrom, false);
				}
			}
			else if ((long)money + (long)addMoney < -2147483648L)
			{
				long num = client.ClientData.StoreMoney + (long)money + (long)addMoney;
				if (num < -2147483648L)
				{
					return false;
				}
				long addMoney3 = (long)addMoney - (-2147483648L - (long)money);
				addMoney = int.MinValue - money;
				this.AddUserStoreMoney(sl, tcpClientPool, pool, client, addMoney3, strFrom, true);
			}
			bool result;
			if (0 == addMoney)
			{
				result = true;
			}
			else
			{
				if (writeToDB)
				{
					string cmdText = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Money1 + addMoney);
					GameManager.DBCmdMgr.AddDBCmd(10004, cmdText, null, client.ServerId);
					long nowTicks = TimeUtil.NOW();
					Global.SetLastDBCmdTicks(client, 10004, nowTicks);
				}
				client.ClientData.Money1 = client.ClientData.Money1 + addMoney;
				GameManager.ClientMgr.NotifySelfMoneyChange(sl, pool, client);
				EventLogManager.AddResourceEvent(client, MoneyTypes.TongQian, (long)addMoney, (long)client.ClientData.Money1, strFrom);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑金", strFrom, "系统", client.ClientData.RoleName, "增加", addMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.Money1, client.ServerId, null);
				}
				GameManager.SystemServerEvents.AddEvent(string.Format("角色添加金钱, roleID={0}({1}), Money={2}, addMoney={3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.RoleName,
					client.ClientData.Money1,
					addMoney
				}), EventLevels.Record);
				result = true;
			}
			return result;
		}

		public bool AddMoney1(GameClient client, int addMoney, string strFrom, bool writeToDB = true)
		{
			return this.AddMoney1(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addMoney, strFrom, writeToDB);
		}

		public bool SubMoney1(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, string strFrom)
		{
			if (client.ClientData.Money1 - subMoney < 0)
			{
				subMoney = client.ClientData.Money1;
			}
			string cmdText = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Money1 - subMoney);
			GameManager.DBCmdMgr.AddDBCmd(10004, cmdText, null, client.ServerId);
			long nowTicks = TimeUtil.NOW();
			Global.SetLastDBCmdTicks(client, 10004, nowTicks);
			client.ClientData.Money1 = client.ClientData.Money1 - subMoney;
			GameManager.ClientMgr.NotifySelfMoneyChange(sl, pool, client);
			if (0 != subMoney)
			{
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑金", strFrom, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.Money1, client.ServerId, null);
			}
			GameManager.SystemServerEvents.AddEvent(string.Format("角色扣除金钱, roleID={0}({1}), Money={2}, subMoney={3}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.RoleName,
				client.ClientData.Money1,
				subMoney
			}), EventLevels.Record);
			EventLogManager.AddResourceEvent(client, MoneyTypes.TongQian, (long)(-(long)subMoney), (long)client.ClientData.Money1, strFrom);
			return true;
		}

		public void NotifySelfUserMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.UserMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 168);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySelfUserMoneyChange(GameClient client)
		{
			client.sendCmd(168, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.UserMoney), false);
		}

		public bool AddUserMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addMoney, string msg, ActivityTypes result = ActivityTypes.None, string param = "")
		{
			lock (client.ClientData.UserMoneyMutex)
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					addMoney,
					(int)result,
					param
				});
				string[] array = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				if (null == array)
				{
					return false;
				}
				if (array.Length != 3)
				{
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(array[1]);
				int num = Convert.ToInt32(array[2]);
				if (num > 0)
				{
					Global.ProcessVipLevelUp(client);
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)addMoney, (long)client.ClientData.UserMoney, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", client.ClientData.RoleName, "增加", addMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(sl, pool, client);
			return true;
		}

		public bool AddOfflineUserMoney(TCPClientPool tcpClientPool, TCPOutPacketPool pool, int otherRoleID, string roleName, int addMoney, string msg, int zoneid, string userid)
		{
			string strcmd = string.Format("{0}:{1}", otherRoleID, addMoney);
			string[] array = Global.ExecuteDBCmd(10011, strcmd, 0);
			bool result;
			if (null == array)
			{
				result = false;
			}
			else if (array.Length != 3)
			{
				result = false;
			}
			else if (Convert.ToInt32(array[1]) < 0)
			{
				result = false;
			}
			else
			{
				int zoneIDByRoleID = this.GetZoneIDByRoleID(otherRoleID);
				EventLogManager.AddResourceEvent(userid, zoneIDByRoleID, otherRoleID, MoneyTypes.YuanBao, (long)addMoney, -1L, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", roleName, "增加", addMoney, zoneid, userid, Convert.ToInt32(array[1]), 0, null);
				}
				result = true;
			}
			return result;
		}

		public bool SubUserMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, string msg, bool bIsAddVipExp = true, bool isAddFund = true, bool isGM = false, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (DaiBiSySType.None != SysType)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, subMoney, SysType, msg, false))
				{
					return true;
				}
			}
			lock (client.ClientData.UserMoneyMutex)
			{
				subMoney = Math.Abs(subMoney);
				if (client.ClientData.UserMoney < subMoney && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.UserMoney - (long)subMoney < -2147483648L)
				{
					return false;
				}
				int userMoney = client.ClientData.UserMoney;
				client.ClientData.UserMoney -= subMoney;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subMoney);
				string[] array = null;
				try
				{
					array = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, string.Format("CMD_DB_UPDATEUSERMONEY_CMD Faild", new object[0]));
					return false;
				}
				if (null == array)
				{
					return false;
				}
				if (array.Length != 3)
				{
					client.ClientData.UserMoney = userMoney;
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					client.ClientData.UserMoney = userMoney;
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(array[1]);
				client._IconStateMgr.FlushUsedMoneyconState(client);
				client._IconStateMgr.CheckJieRiActivity(client, false);
				client._IconStateMgr.SendIconStateToClient(client);
				if (bIsAddVipExp)
				{
					Global.SaveConsumeLog(client, subMoney);
					if (isAddFund)
					{
						FundManager.FundMoneyCost(client, subMoney);
					}
					SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
					if (specialActivity != null)
					{
						specialActivity.MoneyConst(client, subMoney);
					}
					EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
					if (everydayActivity != null)
					{
						everydayActivity.MoneyConst(client, subMoney);
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPriorityActivity != null)
					{
						specPriorityActivity.MoneyConst(client, subMoney);
					}
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)(-(long)subMoney), (long)client.ClientData.UserMoney, msg);
				if (0 != subMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(sl, pool, client);
			return true;
		}

		public bool SubUserMoney(GameClient client, int subMoney, string msg, bool savedb = true, bool bIsAddVipExp = true, bool isAddFund = true, bool isExpense = true, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (DaiBiSySType.None != SysType)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, subMoney, SysType, msg, false))
				{
					return true;
				}
			}
			lock (client.ClientData.UserMoneyMutex)
			{
				subMoney = Math.Abs(subMoney);
				if (client.ClientData.UserMoney < subMoney)
				{
					return false;
				}
				client.ClientData.UserMoney -= subMoney;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subMoney);
				string[] array = Global.ExecuteDBCmd(10011, strcmd, client.ServerId);
				if (null == array)
				{
					return false;
				}
				if (array.Length != 3)
				{
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					return false;
				}
				client.ClientData.UserMoney = Convert.ToInt32(array[1]);
				if (isExpense)
				{
					client._IconStateMgr.FlushUsedMoneyconState(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
				if (savedb)
				{
					Global.SaveConsumeLog(client, subMoney);
					if (isAddFund)
					{
						FundManager.FundMoneyCost(client, subMoney);
					}
					SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
					if (specialActivity != null)
					{
						specialActivity.MoneyConst(client, subMoney);
					}
					EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
					if (everydayActivity != null)
					{
						everydayActivity.MoneyConst(client, subMoney);
					}
					SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
					if (specPriorityActivity != null)
					{
						specPriorityActivity.MoneyConst(client, subMoney);
					}
				}
				EventLogManager.AddResourceEvent(client, MoneyTypes.YuanBao, (long)(-(long)subMoney), (long)client.ClientData.UserMoney, msg);
				if (0 != subMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, client.ClientData.RoleName, "系统", "减少", subMoney, client.ClientData.ZoneID, client.strUserID, client.ClientData.UserMoney, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserMoneyChange(client);
			return true;
		}

		public RoleBaseInfo QueryRoleBaseInfoFromDB(int roleID, int serverID = -1)
		{
			RoleBaseInfo roleBaseInfo = Global.sendToDB<RoleBaseInfo, string>(3004, roleID.ToString(), serverID);
			RoleBaseInfo result;
			if (roleBaseInfo != null && roleBaseInfo.RoleID != roleID)
			{
				result = null;
			}
			else
			{
				result = roleBaseInfo;
			}
			return result;
		}

		public int QueryTotaoChongZhiMoney(GameClient client)
		{
			string userID = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			int zoneID = client.ClientData.ZoneID;
			return this.QueryTotaoChongZhiMoney(userID, zoneID, client.ServerId);
		}

		public int QueryTotaoChongZhiMoney(string userID, int zoneID, int ServerId)
		{
			string strcmd = string.Format("{0}:{1}", userID, zoneID);
			string[] array = Global.ExecuteDBCmd(10083, strcmd, ServerId);
			int result;
			if (null == array)
			{
				result = 0;
			}
			else if (array.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(array[0]);
			}
			return result;
		}

		public int[] QueryUserIdValue(string userID, int ServerId)
		{
			string cmd = string.Format("{0}", userID);
			return Global.sendToDB<int[], string>(13001, cmd, ServerId);
		}

		public int QueryTotaoChongZhiMoneyToday(GameClient client)
		{
			string arg = GameManager.OnlineUserSession.FindUserID(client.ClientSocket);
			int zoneID = client.ClientData.ZoneID;
			string strcmd = string.Format("{0}:{1}", arg, zoneID);
			string[] array = Global.ExecuteDBCmd(10120, strcmd, client.ServerId);
			int result;
			if (null == array)
			{
				result = 0;
			}
			else if (array.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(array[0]);
			}
			return result;
		}

		public int QueryTotalChongZhiMoneyPeriod(GameClient client, DateTime fromDate, DateTime toDate)
		{
			string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, fromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), toDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] array = Global.ExecuteDBCmd(13177, strcmd, client.ServerId);
			int result;
			if (null == array)
			{
				result = 0;
			}
			else if (array.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(array[0]);
			}
			return result;
		}

		public int QueryTotalChongZhiMoneyPeriod(int RoleID, DateTime fromDate, DateTime toDate)
		{
			string strcmd = string.Format("{0}:{1}:{2}", RoleID, fromDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'), toDate.ToString("yyyy-MM-dd HH:mm:ss").Replace(':', '$'));
			string[] array = Global.ExecuteDBCmd(13177, strcmd, 0);
			int result;
			if (null == array)
			{
				result = 0;
			}
			else if (array.Length != 1)
			{
				result = 0;
			}
			else
			{
				result = Global.SafeConvertToInt32(array[0]);
			}
			return result;
		}

		public int GetZoneIDByRoleID(int roleID)
		{
			string strcmd = string.Format("{0}", roleID);
			string[] array = Global.ExecuteDBCmd(30010, strcmd, 0);
			int result;
			if (null == array)
			{
				result = -1;
			}
			else if (array.Length != 2)
			{
				result = -1;
			}
			else
			{
				result = Convert.ToInt32(array[1]);
			}
			return result;
		}

		public string GetUserIDByRoleID(int roleID)
		{
			string strcmd = string.Format("{0}", roleID);
			string[] array = Global.ExecuteDBCmd(30010, strcmd, 0);
			string result;
			if (null == array)
			{
				result = "";
			}
			else if (array.Length != 2)
			{
				result = "";
			}
			else
			{
				result = array[0];
			}
			return result;
		}

		public bool AddUserMoneyOffLine(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int roleID, int addMoney, string msg, int zoneid, string userid)
		{
			string strcmd = string.Format("{0}:{1}", roleID, addMoney);
			string[] array = Global.ExecuteDBCmd(10011, strcmd, 0);
			bool result;
			if (null == array)
			{
				result = false;
			}
			else if (array.Length != 3)
			{
				result = false;
			}
			else if (Convert.ToInt32(array[1]) < 0)
			{
				result = false;
			}
			else
			{
				int zoneIDByRoleID = this.GetZoneIDByRoleID(roleID);
				string userIDByRoleID = this.GetUserIDByRoleID(roleID);
				EventLogManager.AddResourceEvent(userIDByRoleID, zoneIDByRoleID, roleID, MoneyTypes.YuanBao, (long)addMoney, -1L, msg);
				if (0 != addMoney)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "钻石", msg, "系统", string.Concat(roleID), "增加", addMoney, zoneid, userid, Convert.ToInt32(array[1]), 0, null);
				}
				result = true;
			}
			return result;
		}

		public void NotifySelfUserGoldChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.Gold);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 397);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddUserGold(GameClient client, int addGold, string strFrom)
		{
			return this.AddUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addGold, strFrom);
		}

		public bool AddUserGold(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addGold, string strFrom = "")
		{
			int gold = client.ClientData.Gold;
			lock (client.ClientData.GoldMutex)
			{
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, addGold);
				string[] array = Global.ExecuteDBCmd(10095, strcmd, client.ServerId);
				if (null == array)
				{
					return false;
				}
				if (array.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					return false;
				}
				client.ClientData.Gold = Convert.ToInt32(array[1]);
				if (0 != addGold)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", strFrom, client.ClientData.RoleName, "系统", "增加", addGold, client.ClientData.ZoneID, client.strUserID, client.ClientData.Gold, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserGoldChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.BindYuanBao, (long)addGold, (long)client.ClientData.Gold, strFrom);
			return true;
		}

		public bool AddUserGoldOffLine(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, int roleID, int addGold, string strFrom = "", string strUserID = "")
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
			bool result;
			if (null != gameClient)
			{
				result = this.AddUserGold(sl, tcpClientPool, pool, gameClient, addGold, strFrom);
			}
			else
			{
				string strcmd = string.Format("{0}:{1}", roleID, addGold);
				string[] array = Global.ExecuteDBCmd(10095, strcmd, 0);
				if (null == array)
				{
					result = false;
				}
				else if (array.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(array[1]) < 0)
				{
					result = false;
				}
				else
				{
					if (0 != addGold)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", strFrom, string.Concat(roleID), "系统", "增加", addGold, 0, strUserID, Convert.ToInt32(array[1]), 0, null);
					}
					EventLogManager.AddResourceEvent(this.GetUserIDByRoleID(roleID), this.GetZoneIDByRoleID(roleID), roleID, MoneyTypes.BindYuanBao, (long)addGold, -1L, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool SubUserGold(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subGold, string msg = "无", bool isGM = false)
		{
			int gold = client.ClientData.Gold;
			lock (client.ClientData.GoldMutex)
			{
				if (client.ClientData.Gold < subGold && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.Gold - (long)subGold < -2147483648L)
				{
					return false;
				}
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subGold);
				string[] array = Global.ExecuteDBCmd(10095, strcmd, client.ServerId);
				if (null == array)
				{
					return false;
				}
				if (array.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					return false;
				}
				client.ClientData.Gold = Convert.ToInt32(array[1]);
				EventLogManager.AddResourceEvent(client, MoneyTypes.BindYuanBao, (long)(-(long)subGold), (long)client.ClientData.Gold, msg);
				if (0 != subGold)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "绑定钻石", msg, client.ClientData.RoleName, "系统", "减少", subGold, client.ClientData.ZoneID, client.strUserID, client.ClientData.Gold, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserGoldChange(sl, pool, client);
			return true;
		}

		public bool SubUserGold(GameClient client, int subGold, string msg = "无")
		{
			return this.SubUserGold(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, subGold, msg, false);
		}

		public void NotifySelfUserYinLiangChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.YinLiang);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 169);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int addYinLiang, string strFrom, bool isGM = false)
		{
			int yinLiang = client.ClientData.YinLiang;
			lock (client.ClientData.YinLiangMutex)
			{
				if (addYinLiang > 0)
				{
					if (yinLiang >= 1000000000)
					{
						return this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, (long)addYinLiang, strFrom, false);
					}
					if (yinLiang + addYinLiang > 1000000000)
					{
						long addYinLiang2 = (long)yinLiang + (long)addYinLiang - 1000000000L;
						addYinLiang = Global.GMax(0, 1000000000 - yinLiang);
						this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, addYinLiang2, strFrom, false);
					}
				}
				else if ((long)yinLiang + (long)addYinLiang < -2147483648L)
				{
					long num = client.ClientData.StoreYinLiang + (long)yinLiang + (long)addYinLiang;
					if (num < -2147483648L)
					{
						return false;
					}
					long addYinLiang3 = (long)addYinLiang - (-2147483648L - (long)yinLiang);
					addYinLiang = int.MinValue - yinLiang;
					this.AddUserStoreYinLiang(sl, tcpClientPool, pool, client, addYinLiang3, strFrom, isGM);
				}
				if (0 == addYinLiang)
				{
					return true;
				}
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, addYinLiang);
				string[] array = Global.ExecuteDBCmd(10012, strcmd, client.ServerId);
				if (null == array)
				{
					return false;
				}
				if (array.Length != 2)
				{
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					return false;
				}
				client.ClientData.YinLiang = Convert.ToInt32(array[1]);
				if (0 != addYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, "系统", client.ClientData.RoleName, "增加", addYinLiang, client.ClientData.ZoneID, client.strUserID, client.ClientData.YinLiang, client.ServerId, null);
				}
			}
			if (addYinLiang > 0)
			{
				ChengJiuManager.OnTongQianIncrease(client);
			}
			GameManager.ClientMgr.NotifySelfUserYinLiangChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.YinLiang, (long)addYinLiang, (long)client.ClientData.YinLiang, strFrom);
			return true;
		}

		public bool AddUserYinLiang(GameClient client, int addYinLiang, string strFrom)
		{
			return this.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, addYinLiang, strFrom, false);
		}

		public bool AddOfflineUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, string userID, int roleID, string roleName, int addYinLiang, string strFrom, int zoneid)
		{
			string strcmd = string.Format("{0}:{1}", roleID, addYinLiang);
			string[] array = Global.ExecuteDBCmd(10012, strcmd, 0);
			bool result;
			if (null == array)
			{
				result = false;
			}
			else if (array.Length != 2)
			{
				result = false;
			}
			else if (Convert.ToInt32(array[1]) < 0)
			{
				result = false;
			}
			else
			{
				if (0 != addYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, "系统", string.Concat(roleID), "增加", addYinLiang, zoneid, userID, Convert.ToInt32(array[1]), 0, null);
				}
				int zoneIDByRoleID = this.GetZoneIDByRoleID(roleID);
				EventLogManager.AddResourceEvent(userID, zoneIDByRoleID, roleID, MoneyTypes.YinLiang, (long)addYinLiang, -1L, strFrom);
				result = true;
			}
			return result;
		}

		public bool SubUserYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subYinLiang, string strFrom, bool isGM = false)
		{
			int yinLiang = client.ClientData.YinLiang;
			lock (client.ClientData.YinLiangMutex)
			{
				if (client.ClientData.YinLiang < subYinLiang && !isGM)
				{
					return false;
				}
				if ((long)client.ClientData.YinLiang - (long)subYinLiang < -2147483648L)
				{
					return false;
				}
				int yinLiang2 = client.ClientData.YinLiang;
				client.ClientData.YinLiang -= subYinLiang;
				string strcmd = string.Format("{0}:{1}", client.ClientData.RoleID, -subYinLiang);
				string[] array = null;
				try
				{
					array = Global.ExecuteDBCmd(10012, strcmd, client.ServerId);
				}
				catch (Exception ex)
				{
					DataHelper.WriteExceptionLogEx(ex, string.Format("CMD_DB_UPDATEUSERYINLIANG_CMD Faild", new object[0]));
					return false;
				}
				if (null == array)
				{
					return false;
				}
				if (array.Length != 2)
				{
					client.ClientData.YinLiang = yinLiang2;
					return false;
				}
				if (Convert.ToInt32(array[1]) < 0)
				{
					client.ClientData.YinLiang = yinLiang2;
					return false;
				}
				client.ClientData.YinLiang = Convert.ToInt32(array[1]);
				if (0 != subYinLiang)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "金币", strFrom, client.ClientData.RoleName, "系统", "减少", subYinLiang, client.ClientData.ZoneID, client.strUserID, client.ClientData.YinLiang, client.ServerId, null);
				}
			}
			GameManager.ClientMgr.NotifySelfUserYinLiangChange(sl, pool, client);
			EventLogManager.AddResourceEvent(client, MoneyTypes.YinLiang, (long)(-(long)subYinLiang), (long)client.ClientData.YinLiang, strFrom);
			return true;
		}

		public bool MoveGoodsDataToOtherRole(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GoodsData gd, GameClient fromClient, GameClient toClient, bool bAddToTarget = true)
		{
			string[] array = null;
			string strcmd;
			if (!RebornEquip.IsRebornType(gd.GoodsID))
			{
				strcmd = string.Format("{0}:{1}:{2}", toClient.ClientData.RoleID, fromClient.ClientData.RoleID, gd.Id);
			}
			else
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					toClient.ClientData.RoleID,
					fromClient.ClientData.RoleID,
					gd.Id,
					15000
				});
			}
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10013, strcmd, out array, 0);
			bool result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				result = false;
			}
			else if (array.Length < 4 || Convert.ToInt32(array[3]) < 0)
			{
				result = false;
			}
			else
			{
				Global.AddRoleGoodsEvent(fromClient, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, -gd.GCount, "物品转给别人", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
				EventLogManager.AddGoodsEvent(fromClient, OpTypes.AddOrSub, OpTags.None, gd.GoodsID, (long)gd.Id, -gd.GCount, 0, "物品转给别人");
				GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "物品转给别人(在线)", fromClient.ClientData.RoleName, toClient.ClientData.RoleName, "移动", -gd.GCount, fromClient.ClientData.ZoneID, toClient.strUserID, -1, 0, gd);
				if (bAddToTarget)
				{
					if (!RebornEquip.IsRebornType(gd.GoodsID))
					{
						string[] array2 = null;
						gd.BagIndex = Global.GetIdleSlotOfBagGoods(toClient);
						strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							toClient.ClientData.RoleID,
							gd.Id,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							gd.BagIndex,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array2, 0);
						Global.AddGoodsData(toClient, gd);
						ZuoQiManager.ProcessZuoQiTuJian(toClient, gd.GoodsID);
					}
					else
					{
						string[] array2 = null;
						gd.BagIndex = RebornEquip.GetIdleSlotOfRebornGoods(toClient);
						strcmd = Global.FormatUpdateDBGoodsStr(new object[]
						{
							toClient.ClientData.RoleID,
							gd.Id,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							gd.BagIndex,
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*",
							"*"
						});
						Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array2, 0);
						RebornEquip.AddGoodsData(toClient, gd);
					}
					Global.AddRoleGoodsEvent(toClient, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, gd.GCount, "得到他人物品", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
					EventLogManager.AddGoodsEvent(toClient, OpTypes.AddOrSub, OpTags.None, gd.GoodsID, (long)gd.Id, gd.GCount, gd.GCount, "得到他人物品");
					GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "得到他人物品(在线)", fromClient.ClientData.RoleName, toClient.ClientData.RoleName, "移动", gd.GCount, toClient.ClientData.ZoneID, toClient.strUserID, -1, 0, gd);
					ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, -1, -1, gd.GoodsID, TaskTypes.BuySomething, null, 0, -1L, null);
					GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, toClient, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 1, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
				}
				result = true;
			}
			return result;
		}

		public bool MoveGoodsDataToOfflineRole(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GoodsData gd, string fromUserID, int fromRoleID, string fromRoleName, int fromRoleLevel, string toUserID, int toRoleID, string toRoleName, int toRoleLevel, bool bAddToTarget, int zoneid)
		{
			string[] array = null;
			string strcmd;
			if (!RebornEquip.IsRebornType(gd.GoodsID))
			{
				strcmd = string.Format("{0}:{1}:{2}", toRoleID, fromRoleID, gd.Id);
			}
			else
			{
				strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					toRoleID,
					fromRoleID,
					gd.Id,
					15000
				});
			}
			TCPProcessCmdResults tcpprocessCmdResults = Global.RequestToDBServer(tcpClientPool, pool, 10013, strcmd, out array, 0);
			bool result;
			if (tcpprocessCmdResults == TCPProcessCmdResults.RESULT_FAILED)
			{
				LogManager.WriteLog(3, string.Format("向DB请求转移物品时失败{0}->{1}", fromRoleName, toRoleName), null, true);
				result = false;
			}
			else if (array.Length < 4 || Convert.ToInt32(array[3]) < 0)
			{
				LogManager.WriteLog(3, string.Format("向DB请求转移物品时失败{0}->{1},错误码{2}", fromRoleName, toRoleName, array[3]), null, true);
				result = false;
			}
			else
			{
				Global.AddRoleGoodsEvent(fromUserID, fromRoleID, fromRoleName, fromRoleLevel, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, -gd.GCount, "物品转给别人", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
				GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "物品转给别人(离线)", fromRoleName, toRoleName, "移动", -gd.GCount, zoneid, fromUserID, -1, 0, gd);
				if (bAddToTarget)
				{
					Global.AddRoleGoodsEvent(toUserID, toRoleID, toRoleName, toRoleLevel, gd.Id, gd.GoodsID, gd.GCount, gd.Binding, gd.Quality, gd.Forge_level, gd.Jewellist, gd.Site, gd.Endtime, gd.GCount, "得到他人物品", gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip);
					GameManager.logDBCmdMgr.AddDBLogInfo(gd.Id, Global.ModifyGoodsLogName(gd), "得到他人物品(离线)", fromRoleName, toRoleName, "移动", gd.GCount, zoneid, toUserID, -1, 0, gd);
					GameClient gameClient = GameManager.ClientMgr.FindClient(toRoleID);
					if (null != gameClient)
					{
						if (!RebornEquip.IsRebornType(gd.GoodsID))
						{
							string[] array2 = null;
							gd.BagIndex = Global.GetIdleSlotOfBagGoods(gameClient);
							strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								toRoleID,
								gd.Id,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								gd.BagIndex,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array2, 0);
							Global.AddGoodsData(gameClient, gd);
							ZuoQiManager.ProcessZuoQiTuJian(gameClient, gd.GoodsID);
						}
						else
						{
							string[] array2 = null;
							gd.BagIndex = RebornEquip.GetIdleSlotOfRebornGoods(gameClient);
							strcmd = Global.FormatUpdateDBGoodsStr(new object[]
							{
								gameClient.ClientData.RoleID,
								gd.Id,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								gd.BagIndex,
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*",
								"*"
							});
							Global.RequestToDBServer(tcpClientPool, pool, 10006, strcmd, out array2, 0);
							RebornEquip.AddGoodsData(gameClient, gd);
						}
						ProcessTask.Process(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, -1, -1, gd.GoodsID, TaskTypes.BuySomething, null, 0, -1L, null);
						GameManager.ClientMgr.NotifySelfAddGoods(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gd.Id, gd.GoodsID, gd.Forge_level, gd.Quality, gd.GCount, gd.Binding, gd.Site, gd.Jewellist, 1, gd.Endtime, gd.AddPropIndex, gd.BornIndex, gd.Lucky, gd.Strong, gd.ExcellenceInfo, gd.AppendPropLev, gd.ChangeLifeLevForEquip, gd.BagIndex, gd.WashProps, null, 0, "");
					}
				}
				result = true;
			}
			return result;
		}

		public void NotifyGoodsStallCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int status, int stallType)
		{
			string data = string.Format("{0}:{1}:{2}", status, client.ClientData.RoleID, stallType);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 173);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyGoodsStallData(SocketListener sl, TCPOutPacketPool pool, GameClient client, StallData sd)
		{
			byte[] array = null;
			lock (sd)
			{
				array = DataHelper.ObjectToBytes<StallData>(sd);
			}
			TCPOutPacket tcpoutPacket = pool.Pop();
			tcpoutPacket.PacketCmdID = 174;
			tcpoutPacket.FinalWriteData(array, 0, array.Length);
			if (!sl.SendData(client.ClientSocket, tcpoutPacket, true))
			{
			}
		}

		public void NotifySpriteStartStall(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			if (null != client.ClientData.StallDataItem)
			{
				List<object> all9Clients = Global.GetAll9Clients(client);
				if (null != all9Clients)
				{
					string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StallDataItem.StallName);
					this.SendToClients(sl, pool, null, all9Clients, strCmd, 175);
				}
			}
		}

		public void NotifySpriteMarketBuy(SocketListener sl, TCPOutPacketPool pool, GameClient client, GameClient otherClient, int result, int buyType, int goodsDbID, int goodsID, int nID = 226)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				result,
				buyType,
				client.ClientData.RoleID,
				(otherClient != null) ? otherClient.ClientData.RoleID : -1,
				(otherClient != null) ? Global.FormatRoleName(otherClient, otherClient.ClientData.RoleName) : "",
				goodsDbID,
				goodsID
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySpriteMarketBuy2(SocketListener sl, TCPOutPacketPool pool, GameClient client, int otherRoleID, int result, int buyType, int goodsDbID, int goodsID, int otherRoleZoneID, string otherRoleName, int nID = 226)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
			{
				result,
				buyType,
				client.ClientData.RoleID,
				otherRoleID,
				Global.FormatRoleName3(otherRoleID, otherRoleName),
				goodsDbID,
				goodsID
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, nID);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySpriteMarketName(SocketListener sl, TCPOutPacketPool pool, GameClient client, string marketName, int offlineMarket)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, marketName, offlineMarket);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 591);
			}
		}

		public void RemoveCoolDown(SocketListener sl, TCPOutPacketPool pool, GameClient client, int type, int code)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, type, code);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 165);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyUpdateInterPowerCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int hintUser = 1)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.InterPower, hintUser);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 192);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddInterPower(GameClient client, int addInterPower, bool enableFilter = false, bool writeToDB = true)
		{
			bool result;
			if (client.ClientData.InterPower >= 30000)
			{
				result = false;
			}
			else
			{
				if (enableFilter)
				{
					addInterPower = Global.FilterValue(client, addInterPower);
				}
				if (addInterPower <= 0)
				{
					result = false;
				}
				else
				{
					int interPower = client.ClientData.InterPower;
					client.ClientData.InterPower = client.ClientData.InterPower + addInterPower;
					client.ClientData.InterPower = Global.GMin(client.ClientData.InterPower, 30000);
					if (client.ClientData.InterPower > interPower)
					{
						if (writeToDB)
						{
							GameManager.DBCmdMgr.AddDBCmd(10003, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.InterPower), null, client.ServerId);
							long nowTicks = TimeUtil.NOW();
							Global.SetLastDBCmdTicks(client, 10003, nowTicks);
						}
						this.NotifyUpdateInterPowerCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1);
						GameManager.ClientMgr.UpdateRoleDailyData_LingLi(client, client.ClientData.InterPower - interPower);
					}
					result = true;
				}
			}
			return result;
		}

		public bool SubInterPower(GameClient client, int subInterPower)
		{
			if (subInterPower > 0)
			{
				client.ClientData.InterPower = Global.GMax(client.ClientData.InterPower - subInterPower, 0);
				client.ClientData.InterPower = Global.GMin(client.ClientData.InterPower, 30000);
				GameManager.DBCmdMgr.AddDBCmd(10003, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.InterPower), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10003, nowTicks);
				this.NotifyUpdateInterPowerCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, 1);
				DBRoleBufferManager.ProcessLingLiVReserve(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
			}
			return true;
		}

		private void UpdateRoleOnlineTimes(GameClient client, long addTicks)
		{
			this.UpdateRoleOnlineTimesForKorea(client, addTicks);
			if (!client.ClientData.FirstPlayStart)
			{
				if (client.ClientData.ForceShenFenZheng)
				{
					client.ClientData.ForceShenFenZheng = false;
					GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(69, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 17);
				}
				else
				{
					int num = client.ClientData.TotalOnlineSecs / 3600;
					client.ClientData.TotalOnlineSecs += Math.Max(0, (int)(addTicks / 1000L));
					if (client.ClientData.BagNum < Global.MaxBagGridNum)
					{
						client.ClientData.OpenGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "OpenGridTick", client.ClientData.OpenGridTime, false);
					}
					if (client.ClientData.RebornBagNum < Global.MaxBagGridNum)
					{
						client.ClientData.OpenRebornBagTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "10248", client.ClientData.OpenRebornBagTime, false);
					}
					if (client.ClientData.MyPortableBagData.ExtGridNum < Global.ExtraBagGridPriceStartPos)
					{
						client.ClientData.OpenPortableGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "OpenPortableGridTick", client.ClientData.OpenPortableGridTime, false);
					}
					if (client.ClientData.RebornGirdData.ExtGridNum < Global.ExtraBagGridPriceStartPos)
					{
						client.ClientData.OpenRebornGridTime += Math.Max(0, (int)(addTicks / 1000L));
						Global.SaveRoleParamsInt32ValueToDB(client, "10247", client.ClientData.OpenRebornGridTime, false);
					}
					GlobalEventSource.getInstance().fireEvent(new PlayerOnlineEventObject(client));
					int num2 = client.ClientData.TotalOnlineSecs / 3600;
					if (num != num2)
					{
						HuodongCachingMgr.ProcessKaiFuGiftAward(client);
					}
					int num3 = client.ClientData.AntiAddictionSecs / 3600;
					client.ClientData.AntiAddictionSecs += Math.Max(0, (int)(addTicks / 1000L));
					int num4 = client.ClientData.AntiAddictionSecs / 3600;
					int month = TimeUtil.NowDateTime().Month;
					if (client.ClientData.MyHuodongData.CurMID == month.ToString())
					{
						client.ClientData.MyHuodongData.CurMTime += Math.Max(0, (int)(addTicks / 1000L));
					}
					else
					{
						client.ClientData.MyHuodongData.OnlineGiftState = 0;
						client.ClientData.MyHuodongData.CurMID = month.ToString();
						client.ClientData.MyHuodongData.LastMTime = client.ClientData.MyHuodongData.CurMTime;
						client.ClientData.MyHuodongData.CurMTime = 0;
						client.ClientData.MyHuodongData.CurMTime += Math.Max(0, (int)(addTicks / 1000L));
					}
					DailyActiveManager.ProcessOnlineForDailyActive(client);
					client._IconStateMgr.CheckJingJiChangJiangLi(client);
					client._IconStateMgr.CheckFuMeiRiZaiXian(client);
					client._IconStateMgr.SendIconStateToClient(client);
					if (!("1" != GameManager.GameConfigMgr.GetGameConfigItemStr("anti-addiction", "1")))
					{
						if (!this.UpdateRoleOnlineTimesForTengXun(client))
						{
							int num5 = GameManager.OnlineUserSession.FindUserAdult(client.ClientSocket);
							if (num5 <= 0)
							{
								if (num3 < 1 && num4 >= 1)
								{
									BulletinMsgData bulletinMsgData = new BulletinMsgData
									{
										MsgID = "one-hour-hint-addiction",
										PlayMinutes = -1,
										ToPlayNum = -1,
										BulletinText = GLang.GetLang(70, new object[0]),
										BulletinTicks = TimeUtil.NOW(),
										playingNum = 0
									};
									this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
								}
								if (num3 < 2 && num4 >= 2)
								{
									BulletinMsgData bulletinMsgData = new BulletinMsgData
									{
										MsgID = "two-hour-hint-addiction",
										PlayMinutes = -1,
										ToPlayNum = -1,
										BulletinText = GLang.GetLang(71, new object[0]),
										BulletinTicks = TimeUtil.NOW(),
										playingNum = 0
									};
									this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
								}
								int antiAddictionTimeType = Global.GetAntiAddictionTimeType(client);
								if (antiAddictionTimeType != client.ClientData.AntiAddictionTimeType)
								{
									client.ClientData.AntiAddictionTimeType = antiAddictionTimeType;
									if (1 == client.ClientData.AntiAddictionTimeType)
									{
										BulletinMsgData bulletinMsgData;
										if ("0" == GameManager.GameConfigMgr.GetGameConfigItemStr("force-add-shenfenzheng", "1"))
										{
											bulletinMsgData = new BulletinMsgData
											{
												MsgID = "anti-addiction",
												PlayMinutes = -1,
												ToPlayNum = -1,
												BulletinText = GLang.GetLang(72, new object[0]),
												BulletinTicks = TimeUtil.NOW(),
												playingNum = 0
											};
											GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(73, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 14);
										}
										else
										{
											bulletinMsgData = new BulletinMsgData
											{
												MsgID = "anti-addiction",
												PlayMinutes = -1,
												ToPlayNum = -1,
												BulletinText = GLang.GetLang(74, new object[0]),
												BulletinTicks = TimeUtil.NOW(),
												playingNum = 0
											};
											client.ClientData.ForceShenFenZheng = true;
										}
										this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
									}
									else if (2 == client.ClientData.AntiAddictionTimeType)
									{
										BulletinMsgData bulletinMsgData = new BulletinMsgData
										{
											MsgID = "anti-addiction",
											PlayMinutes = -1,
											ToPlayNum = -1,
											BulletinText = GLang.GetLang(75, new object[0]),
											BulletinTicks = TimeUtil.NOW(),
											playingNum = 0
										};
										this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
										GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(76, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 14);
									}
								}
							}
						}
					}
				}
			}
		}

		private void UpdateRoleOnlineTimesForKorea(GameClient client, long addTicks)
		{
			if (!client.ClientData.FirstPlayStart)
			{
				if (!("korea" != GameManager.GameConfigMgr.GetGameConfigItemStr("country", "")))
				{
					int num = client.ClientData.ThisTimeOnlineSecs / 3600;
					client.ClientData.ThisTimeOnlineSecs += Math.Max(0, (int)(addTicks / 1000L));
					int num2 = client.ClientData.ThisTimeOnlineSecs / 3600;
					if (num != num2)
					{
						BulletinMsgData bulletinMsgData = new BulletinMsgData
						{
							MsgID = "this-time-every-one-hour-hint-addiction",
							PlayMinutes = -1,
							ToPlayNum = -1,
							BulletinText = string.Format(GLang.GetLang(77, new object[0]), num2),
							BulletinTicks = TimeUtil.NOW(),
							playingNum = 0
						};
						this.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
					}
				}
			}
		}

		private bool UpdateRoleOnlineTimesForTengXun(GameClient client)
		{
			bool result;
			if ("TengXun" != GameManager.GameConfigMgr.GetGameConfigItemStr("pingtainame", ""))
			{
				result = false;
			}
			else if (client.ClientData.TengXunFCMRate >= 1.0)
			{
				result = true;
			}
			else
			{
				int antiAddictionTimeType_TengXun = Global.GetAntiAddictionTimeType_TengXun(client);
				if (antiAddictionTimeType_TengXun == client.ClientData.AntiAddictionTimeType)
				{
					result = true;
				}
				else
				{
					client.ClientData.AntiAddictionTimeType = antiAddictionTimeType_TengXun;
					result = true;
				}
			}
			return result;
		}

		public void NotifySelfDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
			{
				client.ClientData.RoleID,
				decoID,
				decoType,
				toBody,
				toX,
				toY,
				shakeMap,
				toX1,
				toY1,
				moveTicks,
				alphaTicks
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 229);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyOthersMyDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			if (null == objsList)
			{
				objsList = Global.GetAll9Clients(client);
			}
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
				{
					client.ClientData.RoleID,
					decoID,
					decoType,
					toBody,
					toX,
					toY,
					shakeMap,
					toX1,
					toY1,
					moveTicks,
					alphaTicks
				});
				this.SendToClients(sl, pool, null, objsList, strCmd, 229);
			}
		}

		public void NotifyOthersMyDeco(SocketListener sl, TCPOutPacketPool pool, IObject obj, int mapCode, int copyMapID, int decoID, int decoType, int toBody, int toX, int toY, int shakeMap, int toX1, int toY1, int moveTicks, int alphaTicks, List<object> objsList = null)
		{
			if (null == objsList)
			{
				if (null == obj)
				{
					objsList = Global.GetAll9Clients2(mapCode, toX, toY, copyMapID);
				}
				else
				{
					objsList = Global.GetAll9Clients(obj);
				}
			}
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
				{
					-1,
					decoID,
					decoType,
					toBody,
					toX,
					toY,
					shakeMap,
					toX1,
					toY1,
					moveTicks,
					alphaTicks
				});
				this.SendToClients(sl, pool, null, objsList, strCmd, 229);
			}
		}

		public void NotifyBufferData(GameClient client, BufferData bufferData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BufferData>(bufferData, Global._TCPManager.TcpOutPacketPool, 230);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool VertifyBuffFashion(int bufferID)
		{
			return bufferID == 39 || bufferID == 111 || bufferID == 10013 || bufferID == 103 || bufferID == 10020 || bufferID == 10022 || bufferID == 10023 || bufferID == 10012 || bufferID == 10011 || bufferID == 10010 || bufferID == 10009 || bufferID == 10008 || bufferID == 10007 || bufferID == 10001 || bufferID == 10002 || bufferID == 10003 || bufferID == 10004 || bufferID == 9000 || bufferID == 9001 || bufferID == 9002 || bufferID == 9003 || bufferID == 9004 || bufferID == 9005 || bufferID == 9006 || bufferID == 9007 || bufferID == 9008 || bufferID == 9009 || bufferID == 9010 || bufferID == 9011 || bufferID == 9012 || bufferID == 9051 || bufferID == 9052;
		}

		public void NotifyOtherBufferData(IObject self, BufferData bufferData)
		{
			OtherBufferData otherBufferData = new OtherBufferData
			{
				BufferID = bufferData.BufferID,
				BufferVal = bufferData.BufferVal,
				BufferType = bufferData.BufferType,
				BufferSecs = bufferData.BufferSecs,
				StartTime = bufferData.StartTime
			};
			ObjectTypes objectType = self.ObjectType;
			switch (objectType)
			{
			case ObjectTypes.OT_CLIENT:
				otherBufferData.RoleID = (self as GameClient).ClientData.RoleID;
				break;
			case ObjectTypes.OT_MONSTER:
				otherBufferData.RoleID = (self as Monster).RoleID;
				break;
			default:
				switch (objectType)
				{
				case ObjectTypes.OT_NPC:
					otherBufferData.RoleID = (self as NPC).NpcID;
					goto IL_D0;
				case ObjectTypes.OT_FAKEROLE:
					otherBufferData.RoleID = (self as FakeRoleItem).FakeRoleID;
					goto IL_D0;
				}
				return;
			}
			IL_D0:
			byte[] buffer = DataHelper.ObjectToBytes<OtherBufferData>(otherBufferData);
			List<object> list = Global.GetAll9Clients(self);
			if (null == list)
			{
				list = new List<object>();
			}
			if (list.IndexOf(self) < 0)
			{
				list.Add(self);
			}
			foreach (object obj in list)
			{
				GameClient gameClient = obj as GameClient;
				if (gameClient != null && gameClient.CodeRevision >= 2)
				{
					this.SendToClient(gameClient, buffer, 676);
				}
			}
		}

		public void NotifySelfExperience(SocketListener sl, TCPOutPacketPool pool, GameClient client, long newExperience)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.Experience,
				client.ClientData.Level,
				newExperience,
				client.ClientData.ChangeLifeCount
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 141);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void ProcessRoleExperience(GameClient client, long experience, bool enableFilter = true, bool writeToDB = true, bool checkDead = false, string strFrom = "none")
		{
			if (client.ClientData.HideGM <= 0)
			{
				if (!checkDead || client.ClientData.CurrentLifeV > 0)
				{
					if (experience > 0L)
					{
						if (enableFilter)
						{
							experience = Global.FilterValue(client, experience);
						}
						if (experience > 0L)
						{
							long experience2 = client.ClientData.Experience;
							int unionLevel = Global.GetUnionLevel2(client);
							EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.Awards, MoneyTypes.Exp, experience, -1L, strFrom);
							int level = client.ClientData.Level;
							Global.EarnExperience(client, experience);
							if (writeToDB || level != client.ClientData.Level)
							{
								int num = Global.CalcOriginalOccupationID(client.ClientData.Occupation);
								ChangeLifeAddPointInfo changeLifeAddPointInfo = null;
								if (!Data.ChangeLifeAddPointInfoList.TryGetValue(client.ClientData.ChangeLifeCount, out changeLifeAddPointInfo) || changeLifeAddPointInfo == null)
								{
									return;
								}
								lock (client.ClientData.PropPointMutex)
								{
									int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "TotalPropPoint");
									int num2 = client.ClientData.Level - level;
									int num3 = num2 * changeLifeAddPointInfo.AddPoint + roleParamsInt32FromDB;
									client.ClientData.TotalPropPoint = num3;
									Global.SaveRoleParamsInt32ValueToDB(client, "TotalPropPoint", num3, true);
								}
								GameManager.DBCmdMgr.AddDBCmd(10002, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Level, client.ClientData.Experience), null, client.ServerId);
								long nowTicks = TimeUtil.NOW();
								Global.SetLastDBCmdTicks(client, 10002, nowTicks);
							}
							if (level != client.ClientData.Level)
							{
								GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
								GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, true, 7);
								GameManager.ClientMgr.NotifyTeamUpLevel(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, false);
								if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									Global.AutoLearnSkills(client);
								}
								if (client.ClientData.IsFlashPlayer != 1 && client.ClientData.MapCode != 6090)
								{
									ChengJiuManager.OnRoleLevelUp(client);
								}
								HuodongCachingMgr.ProcessKaiFuGiftAward(client);
								HuodongCachingMgr.ProcessUpLevelAward4_60Level_100Level(client, level, client.ClientData.Level);
								WorldLevelManager.getInstance().UpddateWorldLevelBuff(client);
								GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.RoleLevelUp));
								SpreadManager.getInstance().SpreadIsLevel(client);
								SingletonTemplate<TradeBlackManager>.Instance().UpdateObjectExtData(client);
								EventLogManager.AddRoleUpgradeEvent(client, experience, experience2, unionLevel, strFrom);
								if (client._IconStateMgr.CheckReborn(client))
								{
									client._IconStateMgr.SendIconStateToClient(client);
								}
							}
							GameManager.ClientMgr.UpdateRoleDailyData_Exp(client, experience);
							GameManager.ClientMgr.NotifySelfExperience(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, experience);
						}
					}
				}
			}
		}

		public void AddOnlieRoleExperience(GameClient client, int addPercent)
		{
			long num = 0L;
			if (client.ClientData.Level < Data.LevelUpExperienceList.Length - 1)
			{
				num = Data.LevelUpExperienceList[client.ClientData.Level + 1];
			}
			if (num > 0L)
			{
				int num2 = (int)((double)num * ((double)addPercent / 100.0));
				this.ProcessRoleExperience(client, (long)num2, false, false, false, "none");
			}
		}

		public void AddAllOnlieRoleExperience(int addPercent)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep <= 0)
				{
					this.AddOnlieRoleExperience(nextClient, addPercent);
				}
			}
		}

		public long GetCurRoleLvUpNeedExp(GameClient client)
		{
			long result;
			if (client == null)
			{
				result = 0L;
			}
			else if (client.ClientData.Level >= Data.LevelUpExperienceList.Length - 1)
			{
				result = 0L;
			}
			else
			{
				long num = Data.LevelUpExperienceList[client.ClientData.Level];
				if (client.ClientData.ChangeLifeCount > 0)
				{
					ChangeLifeDataInfo changeLifeDataInfo = GameManager.ChangeLifeMgr.GetChangeLifeDataInfo(client, 0);
					if (changeLifeDataInfo != null && changeLifeDataInfo.ExpProportion > 0L)
					{
						num *= changeLifeDataInfo.ExpProportion;
					}
				}
				result = num;
			}
			return result;
		}

		public int AutoCompletionTaskByTaskID(TCPManager tcpMgr, TCPClientPool tcpClientPool, TCPOutPacketPool pool, TCPRandKey tcpRandKey, GameClient client, int nDestTaskID)
		{
			int result;
			if (null == client)
			{
				LogManager.WriteLog(2, string.Format("client不存在，服务器无法完成某任务与之前所有任务", new object[0]), null, true);
				result = -1;
			}
			else
			{
				try
				{
					int roleID = client.ClientData.RoleID;
					List<int> list = new List<int>();
					foreach (KeyValuePair<int, SystemXmlItem> keyValuePair in GameManager.SystemTasksMgr.SystemXmlItemDict)
					{
						SystemXmlItem value = keyValuePair.Value;
						int key = keyValuePair.Key;
						if (key > nDestTaskID)
						{
							break;
						}
						if (key > 0)
						{
							Global.AddOldTask(client, key);
							Global.AddRoleTaskEvent(client, key);
							ChengJiuManager.ProcessCompleteMainTaskForChengJiu(client, key);
							Global.UpdateTaskZhangJieProp(client, key, false);
							list.Add(key);
						}
					}
					list.Sort();
					list.Insert(0, roleID);
					byte[] array = DataHelper.ObjectToBytes<List<int>>(list);
					TCPOutPacket tcpoutPacket = null;
					TCPProcessCmdResults tcpprocessCmdResults = Global.TransferRequestToDBServer(tcpMgr, client.ClientSocket, tcpClientPool, tcpRandKey, pool, 10180, array, array.Length, out tcpoutPacket, client.ServerId);
					if (tcpprocessCmdResults != TCPProcessCmdResults.RESULT_DATA || null == tcpoutPacket)
					{
						return -1;
					}
					string @string = new UTF8Encoding().GetString(tcpoutPacket.GetPacketBytes(), 6, tcpoutPacket.PacketDataSize - 6);
					Global.PushBackTcpOutPacket(tcpoutPacket);
					string[] array2 = @string.Split(new char[]
					{
						':'
					});
					if (array2.Length != 1)
					{
						return -1;
					}
					return int.Parse(array2[0]);
				}
				catch (Exception e)
				{
					DataHelper.WriteFormatExceptionLog(e, Global.GetDebugHelperInfo(client.ClientSocket), false, false);
				}
				result = -1;
			}
			return result;
		}

		public void SearchRolesByStr(GameClient client, string roleName, int startIndex)
		{
			int num = startIndex;
			int num2 = 0;
			List<SearchRoleData> list = new List<SearchRoleData>();
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (-1 != nextClient.ClientData.RoleName.IndexOf(roleName))
				{
					list.Add(new SearchRoleData
					{
						RoleID = nextClient.ClientData.RoleID,
						RoleName = Global.FormatRoleName(nextClient, nextClient.ClientData.RoleName),
						RoleSex = nextClient.ClientData.RoleSex,
						Level = nextClient.ClientData.Level,
						Occupation = nextClient.ClientData.Occupation,
						MapCode = nextClient.ClientData.MapCode,
						PosX = nextClient.ClientData.PosX,
						PosY = nextClient.ClientData.PosY,
						ChangeLifeLev = nextClient.ClientData.ChangeLifeCount
					});
					num2++;
					if (num2 >= 10)
					{
						break;
					}
				}
			}
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<SearchRoleData>>(list, Global._TCPManager.TcpOutPacketPool, 232);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void ListMapRoles(GameClient client, int startIndex)
		{
			ListRolesData listRolesData = new ListRolesData
			{
				StartIndex = startIndex,
				TotalRolesCount = 0,
				PageRolesCount = 10,
				SearchRoleDataList = new List<SearchRoleData>()
			};
			List<SearchRoleData> searchRoleDataList = listRolesData.SearchRoleDataList;
			List<object> list = this.GetMapClients(client.ClientData.MapCode);
			list = Global.FilterHideObjsList(list);
			if (list == null || list.Count <= 0)
			{
				this.SendListRolesDataResult(client, listRolesData);
			}
			else
			{
				List<GameClient> list2 = new List<GameClient>();
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is GameClient)
					{
						if ((list[i] as GameClient).ClientData.TeamID <= 0)
						{
							list2.Add(list[i] as GameClient);
						}
					}
				}
				listRolesData.TotalRolesCount = list2.Count;
				if (listRolesData.TotalRolesCount <= 0)
				{
					this.SendListRolesDataResult(client, listRolesData);
				}
				else
				{
					if (startIndex >= list2.Count)
					{
						startIndex = 0;
					}
					int num = 0;
					for (int i = 0; i < list2.Count; i++)
					{
						if (i >= startIndex)
						{
							GameClient gameClient = list2[i];
							searchRoleDataList.Add(new SearchRoleData
							{
								RoleID = gameClient.ClientData.RoleID,
								RoleName = Global.FormatRoleName(gameClient, gameClient.ClientData.RoleName),
								RoleSex = gameClient.ClientData.RoleSex,
								Level = gameClient.ClientData.Level,
								Occupation = gameClient.ClientData.Occupation,
								MapCode = gameClient.ClientData.MapCode,
								PosX = gameClient.ClientData.PosX,
								PosY = gameClient.ClientData.PosY,
								CombatForce = gameClient.ClientData.CombatForce,
								ChangeLifeLev = gameClient.ClientData.ChangeLifeCount
							});
							num++;
							if (num >= 10)
							{
								break;
							}
						}
					}
					this.SendListRolesDataResult(client, listRolesData);
				}
			}
		}

		private void SendListRolesDataResult(GameClient client, ListRolesData listRolesData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<ListRolesData>(listRolesData, Global._TCPManager.TcpOutPacketPool, 233);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void ListAllTeams(GameClient client, int startIndex)
		{
			SearchTeamData searchTeamData = new SearchTeamData
			{
				StartIndex = startIndex,
				TotalTeamsCount = 0,
				PageTeamsCount = 10,
				TeamDataList = null
			};
			searchTeamData.TotalTeamsCount = GameManager.TeamMgr.GetTotalDataCount();
			if (searchTeamData.TotalTeamsCount <= 0)
			{
				this.SendListTeamsDataResult(client, searchTeamData);
			}
			else
			{
				if (startIndex >= searchTeamData.TotalTeamsCount)
				{
					startIndex = 0;
				}
				searchTeamData.TeamDataList = GameManager.TeamMgr.GetTeamDataList(startIndex, searchTeamData.PageTeamsCount);
				this.SendListTeamsDataResult(client, searchTeamData);
			}
		}

		private void SendListTeamsDataResult(GameClient client, SearchTeamData searchTeamData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<SearchTeamData>(searchTeamData, Global._TCPManager.TcpOutPacketPool, 234);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyDailyTaskData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<DailyTaskData>>(client.ClientData.MyDailyTaskDataList, Global._TCPManager.TcpOutPacketPool, 236);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyFuBenData(GameClient client, FuBenData fuBenData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FuBenData>(fuBenData, Global._TCPManager.TcpOutPacketPool, 252);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyFuBenBeginInfo(GameClient client)
		{
			if (client.ClientData.IsFlashPlayer == 1 && client.ClientData.MapCode == 6090)
			{
				string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					client.ClientData.RoleID,
					-1,
					TimeUtil.NOW(),
					0,
					1,
					0,
					1,
					1,
					1
				});
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 260);
				if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
				{
					return;
				}
			}
			int num = FuBenManager.FindFuBenSeqIDByRoleID(client.ClientData.RoleID);
			if (num > 0)
			{
				int copyMapID = client.ClientData.CopyMapID;
				if (copyMapID > 0)
				{
					int num2 = FuBenManager.FindFuBenIDByMapCode(client.ClientData.MapCode);
					if (num2 > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(num);
						if (null != fuBenInfoItem)
						{
							CopyMap copyMap = GameManager.CopyMapMgr.FindCopyMap(copyMapID);
							if (null != copyMap)
							{
								if (Global.IsStoryCopyMapScene(client.ClientData.MapCode))
								{
									SystemXmlItem systemXmlItem = null;
									if (GameManager.systemFuBenMgr.SystemXmlItemDict.TryGetValue(copyMap.FubenMapID, out systemXmlItem) && systemXmlItem != null)
									{
										int intValue = systemXmlItem.GetIntValue("BossID", -1);
										int mapMonsterNum = GameManager.MonsterZoneMgr.GetMapMonsterNum(client.ClientData.MapCode, intValue);
										if (mapMonsterNum == 0)
										{
											Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 1);
										}
										else
										{
											Global.NotifyClientStoryCopyMapInfo(copyMap.CopyMapID, 2);
										}
									}
								}
								long startTicks = fuBenInfoItem.StartTicks;
								long endTicks = fuBenInfoItem.EndTicks;
								int killedNormalNum = copyMap.KilledNormalNum;
								int totalNormalNum = copyMap.TotalNormalNum;
								int killedBossNum = copyMap.KilledBossNum;
								int totalBossNum = copyMap.TotalBossNum;
								string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
								{
									client.ClientData.RoleID,
									num2,
									startTicks,
									endTicks,
									killedNormalNum,
									totalNormalNum,
									killedBossNum,
									totalBossNum
								});
								TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 260);
								if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
								{
								}
							}
						}
					}
				}
			}
		}

		public void NotifyAllFuBenBeginInfo(CopyMap copyMap, int roleId, bool allKilled)
		{
			int fuBenSeqID = copyMap.FuBenSeqID;
			if (fuBenSeqID > 0)
			{
				int copyMapID = copyMap.CopyMapID;
				if (copyMapID > 0)
				{
					int num = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
					if (num > 0)
					{
						FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(fuBenSeqID);
						if (null != fuBenInfoItem)
						{
							long startTicks = fuBenInfoItem.StartTicks;
							long endTicks = fuBenInfoItem.EndTicks;
							int num2 = copyMap.KilledNormalNum;
							int totalNormalNum = copyMap.TotalNormalNum;
							if (allKilled)
							{
								num2 = totalNormalNum;
							}
							int num3 = copyMap.KilledBossNum;
							int totalBossNum = copyMap.TotalBossNum;
							if (allKilled)
							{
								num3 = totalBossNum;
							}
							string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
							{
								roleId,
								num,
								startTicks,
								endTicks,
								num2,
								totalNormalNum,
								num3,
								totalBossNum
							});
							List<object> list = this.GetMapClients(copyMap.MapCode);
							if (null != list)
							{
								list = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, list, false);
								this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, strCmd, 260);
							}
						}
					}
				}
			}
		}

		public void NotifyAllMapFuBenBeginInfo(CopyMap copyMap, int roleId, bool allKilled)
		{
			FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
			if (null != fuBenInfoItem)
			{
				long startTicks = fuBenInfoItem.StartTicks;
				long endTicks = fuBenInfoItem.EndTicks;
				int num = copyMap.KilledNormalNum;
				int totalNormalNum = copyMap.TotalNormalNum;
				if (allKilled)
				{
					num = totalNormalNum;
				}
				int num2 = copyMap.KilledBossNum;
				int totalBossNum = copyMap.TotalBossNum;
				if (allKilled)
				{
					num2 = totalBossNum;
				}
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
				{
					roleId,
					copyMap.FubenMapID,
					startTicks,
					endTicks,
					num,
					totalNormalNum,
					num2,
					totalBossNum
				});
				List<object> list = new List<object>();
				List<int> list2 = FuBenManager.FindMapCodeListByFuBenID(copyMap.FubenMapID);
				if (null != list2)
				{
					foreach (int mapCode in list2)
					{
						int num3 = GameManager.CopyMapMgr.FindCopyID(copyMap.FuBenSeqID, mapCode);
						if (num3 >= 0)
						{
							CopyMap copyMap2 = GameManager.CopyMapMgr.FindCopyMap(num3);
							if (null != copyMap2)
							{
								list.AddRange(copyMap2.GetClientsList());
							}
						}
					}
				}
				if (0 != list.Count)
				{
					this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, strCmd, 260);
				}
			}
		}

		public void NotifyAllFuBenTongGuanJiangLi(CopyMap copyMap, byte[] bytesData)
		{
			int copyMapID = copyMap.CopyMapID;
			if (copyMapID > 0)
			{
				int num = FuBenManager.FindFuBenIDByMapCode(copyMap.MapCode);
				if (num > 0)
				{
					FuBenInfoItem fuBenInfoItem = FuBenManager.FindFuBenInfoBySeqID(copyMap.FuBenSeqID);
					if (null != fuBenInfoItem)
					{
						List<object> list = this.GetMapClients(copyMap.MapCode);
						if (null != list)
						{
							list = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, list, false);
							this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, bytesData, 521);
						}
					}
				}
			}
		}

		public void NotifyAllFuBenMonstersNum(CopyMap copyMap, bool allKilled)
		{
			if (!GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(copyMap.FubenMapID))
			{
				int fuBenSeqID = copyMap.FuBenSeqID;
				if (fuBenSeqID > 0)
				{
					int copyMapID = copyMap.CopyMapID;
					if (copyMapID > 0)
					{
						int num = copyMap.KilledNormalNum;
						int totalNormalNum = copyMap.TotalNormalNum;
						if (allKilled)
						{
							num = totalNormalNum;
						}
						int num2 = copyMap.KilledBossNum;
						int totalBossNum = copyMap.TotalBossNum;
						if (allKilled)
						{
							num2 = totalBossNum;
						}
						string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
						{
							copyMap.GetGameClientCount(),
							num,
							totalNormalNum,
							num2,
							totalBossNum
						});
						List<object> list = this.GetMapClients(copyMap.MapCode);
						if (null != list)
						{
							list = Global.ConvertObjsList(copyMap.MapCode, copyMap.CopyMapID, list, false);
							this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, strCmd, 261);
						}
					}
				}
			}
		}

		public void NotifyDailyJingMaiData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DailyJingMaiData>(client.ClientData.MyDailyJingMaiData, Global._TCPManager.TcpOutPacketPool, 237);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyOtherJingMaiExp(GameClient client)
		{
			int leftAddJingMaiExpNum = Global.GetLeftAddJingMaiExpNum(client);
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TotalJingMaiExp, leftAddJingMaiExpNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 256);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySelfAddSkill(SocketListener sl, TCPOutPacketPool pool, GameClient client, int skillDbID, int skillID, int skillLevel)
		{
			string data = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				skillDbID,
				skillID,
				skillLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 217);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void AddNumSkill(GameClient client, SkillData skillData, int addNum, bool writeToDB = true)
		{
			if (addNum != 0)
			{
				int num;
				if (skillData.DbID < 0)
				{
					num = client.ClientData.DefaultSkillUseNum;
				}
				else
				{
					num = skillData.UsedNum;
				}
				SystemXmlItem magicCacheItem = MagicsCacheManager.GetMagicCacheItem(client.ClientData.Occupation, skillData.SkillID, skillData.SkillLevel);
				if (addNum > 0)
				{
					if (magicCacheItem == null)
					{
						return;
					}
					if (magicCacheItem.GetIntValue("ShuLianDu", -1) <= num)
					{
						if (skillData.DbID < 0)
						{
							client.ClientData.DefaultSkillUseNum = magicCacheItem.GetIntValue("ShuLianDu", -1);
						}
						else
						{
							skillData.UsedNum = magicCacheItem.GetIntValue("ShuLianDu", -1);
						}
						return;
					}
				}
				int num2;
				if (skillData.DbID < 0)
				{
					client.ClientData.DefaultSkillUseNum += addNum;
					if (client.ClientData.DefaultSkillUseNum < 0)
					{
						client.ClientData.DefaultSkillUseNum = 0;
					}
					Global.SaveRoleParamsInt32ValueToDB(client, "DefaultSkillUseNum", client.ClientData.DefaultSkillUseNum, false);
					num2 = client.ClientData.DefaultSkillUseNum;
				}
				else
				{
					skillData.UsedNum += addNum;
					if (skillData.UsedNum < 0)
					{
						skillData.UsedNum = 0;
					}
					this.UpdateSkillInfo(client, skillData, writeToDB);
					num2 = skillData.UsedNum;
				}
				if (magicCacheItem != null && num2 >= magicCacheItem.GetIntValue("ShuLianDu", -1))
				{
					GameManager.ClientMgr.NotifySkillUsedNumFull(client, skillData);
				}
			}
		}

		public void UpdateSkillInfo(GameClient client, SkillData skillData, bool writeToDB = true)
		{
			if (writeToDB)
			{
				Global.SetLastDBSkillCmdTicks(client, skillData.SkillID, 0L);
				GameManager.DBCmdMgr.AddDBCmd(10037, string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					skillData.DbID,
					skillData.SkillLevel,
					skillData.UsedNum
				}), null, client.ServerId);
			}
			else
			{
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBSkillCmdTicks(client, skillData.SkillID, nowTicks);
			}
		}

		public void NotifySkillUsedNumFull(GameClient client, SkillData skillData)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				skillData.DbID,
				skillData.SkillID,
				skillData.UsedNum,
				skillData.SkillLevel
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 258);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySkillCDTime(GameClient client, int skillid, long cdtime, bool waitEnterScene = false)
		{
			client.sendCmd(691, string.Format("{0}:{1}:{2}", client.ClientData.RoleID, skillid, cdtime), waitEnterScene);
		}

		public void NotifyPortableBagData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<PortableBagData>(client.ClientData.MyPortableBagData, Global._TCPManager.TcpOutPacketPool, 241);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyHuodongData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HuodongData>(client.ClientData.MyHuodongData, Global._TCPManager.TcpOutPacketPool, 245);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyGetCombatGiftData(GameClient client)
		{
		}

		public void NotifyGetLevelUpGiftData(GameClient client, int newLevel)
		{
			GameManager.ClientMgr.SendToClient(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, string.Format("{0}:{1}", client.ClientData.RoleID, newLevel), 445);
		}

		public void NotifyAllChangeHuoDongID(int bigAwardID, int songLiID)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (!nextClient.ClientSocket.IsKuaFuLogin)
				{
					string data = string.Format("{0}:{1}:{2}", nextClient.ClientData.RoleID, bigAwardID, songLiID);
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 251);
					if (!Global._TCPManager.MySocketListener.SendData(nextClient.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public void NotifyTeamMemberFuBenEnterMsg(GameClient client, int leaderRoleID, int fuBenID, int fuBenSeqID)
		{
			string data = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				client.ClientData.RoleID,
				leaderRoleID,
				fuBenID,
				fuBenSeqID
			});
			client.ClientData.NotifyFuBenID = fuBenID;
			client.ClientData.NotifyFuBenSeqID = fuBenSeqID;
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 254);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyTeamFuBenEnterMsg(List<int> roleIDsList, int minLevel, int maxLevel, int leaderMapCode, int leaderRoleID, int fuBenID, int fuBenSeqID, int enterNumber, int maxFinishNum, bool igoreNumLimit = false)
		{
			if (roleIDsList != null && roleIDsList.Count > 0)
			{
				for (int i = 0; i < roleIDsList.Count; i++)
				{
					GameClient gameClient = this.FindClient(roleIDsList[i]);
					if (null != gameClient)
					{
						if (gameClient.ClientData.MapCode == leaderMapCode)
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
										goto IL_E7;
									}
								}
								this.NotifyTeamMemberFuBenEnterMsg(gameClient, leaderRoleID, fuBenID, fuBenSeqID);
							}
						}
					}
					IL_E7:;
				}
			}
		}

		public void InitLianZhanBuff(GameClient client)
		{
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 122);
			if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				if (Global.CanMapUseBuffer(client.ClientData.GetRoleData().MapCode, 122))
				{
					LianZhanConfig lianZhanBufferVal = Global.GetLianZhanBufferVal((int)bufferDataByID.BufferVal);
					if (lianZhanBufferVal != null)
					{
						client.ClientData.LianZhanExpRate = lianZhanBufferVal.RebornExp;
					}
				}
			}
		}

		public double GetLianZhanExpRate(GameClient client)
		{
			double result = 0.0;
			BufferData bufferDataByID = Global.GetBufferDataByID(client, 122);
			if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
			{
				int lianZhanNum = (int)bufferDataByID.BufferVal;
				LianZhanConfig lianZhanBufferVal = Global.GetLianZhanBufferVal(lianZhanNum);
				if (null != lianZhanBufferVal)
				{
					result = lianZhanBufferVal.RebornExp;
				}
			}
			return result;
		}

		public void ChangeRoleLianZhan(SocketListener sl, TCPOutPacketPool pool, GameClient client, Monster monster, int addNum = 1)
		{
			if (null != client)
			{
				if (Data.IsLianZhanMap(client.ClientData.MapCode))
				{
					int tempLianZhan = client.ClientData.TempLianZhan;
					if (Global.CanContinueLianZhan(client) || addNum > 1)
					{
						client.ClientData.TempLianZhan = client.ClientData.TempLianZhan + addNum;
						if (client.ClientData.TempLianZhan % Data.MinLianZhanNum == 0)
						{
							LianZhanConfig lianZhanBufferVal = Global.GetLianZhanBufferVal(client.ClientData.TempLianZhan);
							if (null != lianZhanBufferVal)
							{
								int num = 0;
								BufferData bufferDataByID = Global.GetBufferDataByID(client, 122);
								if (bufferDataByID != null && !Global.IsBufferDataOver(bufferDataByID, 0L))
								{
									num = (int)bufferDataByID.BufferVal;
								}
								if (lianZhanBufferVal.Num > num)
								{
									Global.UpdateBufferData(client, BufferItemTypes.LianZhanBuff, new double[]
									{
										(double)lianZhanBufferVal.Time,
										(double)((long)lianZhanBufferVal.Num + ((long)lianZhanBufferVal.GoodsID << 32))
									}, 0, true);
									Global.AddLianZhanEvent(client, client.ClientData.TempLianZhan);
								}
								else if (lianZhanBufferVal != null && lianZhanBufferVal.Num == num && TimeUtil.NOW() + (long)(lianZhanBufferVal.Time * 1000) > bufferDataByID.StartTime + (long)(bufferDataByID.BufferSecs * 1000))
								{
									Global.UpdateBufferData(client, BufferItemTypes.LianZhanBuff, new double[]
									{
										(double)lianZhanBufferVal.Time,
										(double)((long)lianZhanBufferVal.Num + ((long)lianZhanBufferVal.GoodsID << 32))
									}, 0, true);
									Global.AddLianZhanEvent(client, client.ClientData.TempLianZhan);
								}
							}
						}
					}
					else
					{
						client.ClientData.TempLianZhan = addNum;
					}
					client.ClientData.StartLianZhanTicks = TimeUtil.NOW();
					client.ClientData.WaitingLianZhanMS = (long)(Data.LianZhanContinueTime(client.ClientData.TempLianZhan) * 1000);
					RebornManager.getInstance().ProcessLianZhan(client);
					string cmdData = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.TempLianZhan, client.ClientData.StartLianZhanTicks + client.ClientData.WaitingLianZhanMS);
					client.sendCmd(266, cmdData, false);
				}
			}
		}

		public void UpdateRoleDailyData_Exp(GameClient client, long newExperience)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.ExpDayID)
			{
				client.ClientData.MyRoleDailyData.TodayExp += (int)newExperience;
			}
			else
			{
				client.ClientData.MyRoleDailyData.ExpDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.TodayExp = (int)newExperience;
			}
		}

		public void UpdateRoleDailyData_LingLi(GameClient client, int newLingLi)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.LingLiDayID)
			{
				client.ClientData.MyRoleDailyData.TodayLingLi += newLingLi;
			}
			else
			{
				client.ClientData.MyRoleDailyData.LingLiDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.TodayLingLi = newLingLi;
			}
		}

		public void UpdateRoleDailyData_KillBoss(GameClient client, int newKillBoss)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.KillBossDayID)
			{
				client.ClientData.MyRoleDailyData.TodayKillBoss += newKillBoss;
			}
			else
			{
				client.ClientData.MyRoleDailyData.KillBossDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.TodayKillBoss = newKillBoss;
			}
		}

		public void UpdateRoleDailyData_FuBenNum(GameClient client, int newFuBenNum, int nLev, bool bActiveChenJiu = true)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.FuBenDayID)
			{
				client.ClientData.MyRoleDailyData.TodayFuBenNum += newFuBenNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.FuBenDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.TodayFuBenNum = newFuBenNum;
			}
			DailyActiveManager.ProcessCompleteCopyMapForDailyActive(client, nLev, 1);
			if (bActiveChenJiu)
			{
				ChengJiuManager.ProcessCompleteCopyMapForChengJiu(client, nLev, 1);
			}
		}

		public long GetRoleDailyData_RebornExp(GameClient client, MoneyTypes types)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.RebornExpDayID)
			{
				if (types == MoneyTypes.RebornExpMonster)
				{
					return (long)client.ClientData.MyRoleDailyData.RebornExpMonster;
				}
				if (types == MoneyTypes.RebornExpSale)
				{
					return (long)client.ClientData.MyRoleDailyData.RebornExpSale;
				}
			}
			else
			{
				client.ClientData.MyRoleDailyData.RebornExpMonster = 0;
				client.ClientData.MyRoleDailyData.RebornExpSale = 0;
				client.ClientData.MyRoleDailyData.RebornExpDayID = dayOfYear;
			}
			return 0L;
		}

		public void UpdateRoleDailyData_RebornExp(GameClient client, MoneyTypes types, long newExperience)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.RebornExpDayID)
			{
				if (types == MoneyTypes.RebornExpMonster)
				{
					client.ClientData.MyRoleDailyData.RebornExpMonster += (int)newExperience;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					client.ClientData.MyRoleDailyData.RebornExpSale += (int)newExperience;
				}
			}
			else
			{
				client.ClientData.MyRoleDailyData.RebornExpMonster = 0;
				client.ClientData.MyRoleDailyData.RebornExpSale = 0;
				client.ClientData.MyRoleDailyData.RebornExpDayID = dayOfYear;
				if (types == MoneyTypes.RebornExpMonster)
				{
					client.ClientData.MyRoleDailyData.RebornExpMonster = (int)newExperience;
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					client.ClientData.MyRoleDailyData.RebornExpSale = (int)newExperience;
				}
			}
		}

		public void UpdateRoleDailyData_WuXingNum(GameClient client, int newWuXingNum)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.WuXingDayID)
			{
				client.ClientData.MyRoleDailyData.WuXingNum += newWuXingNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.WuXingDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.WuXingNum = newWuXingNum;
			}
		}

		public void UpdateRoleDailyData_SweepNum(GameClient client, int newWuXingNum)
		{
			if (null == client.ClientData.MyRoleDailyData)
			{
				client.ClientData.MyRoleDailyData = new RoleDailyData();
			}
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear == client.ClientData.MyRoleDailyData.WuXingDayID)
			{
				client.ClientData.MyRoleDailyData.WuXingNum += newWuXingNum;
			}
			else
			{
				client.ClientData.MyRoleDailyData.WuXingDayID = dayOfYear;
				client.ClientData.MyRoleDailyData.WuXingNum = newWuXingNum;
			}
		}

		public void NotifyRoleDailyData(GameClient client)
		{
			RoleDailyData myRoleDailyData = client.ClientData.MyRoleDailyData;
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<RoleDailyData>(myRoleDailyData, Global._TCPManager.TcpOutPacketPool, 267);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void UpdateKillBoss(GameClient client, int killBossNum, Monster monster, bool writeToDB = false)
		{
			if (401 == monster.MonsterType)
			{
				int[] paramValueIntArrayByName = GameManager.systemParamsList.GetParamValueIntArrayByName("NotTuMo", ',');
				if (paramValueIntArrayByName != null && paramValueIntArrayByName.Length > 0)
				{
					for (int i = 0; i < paramValueIntArrayByName.Length; i++)
					{
						if (monster.MonsterInfo.ExtensionID == paramValueIntArrayByName[i])
						{
							return;
						}
					}
				}
				client.ClientData.KillBoss += killBossNum;
				this.UpdateRoleDailyData_KillBoss(client, killBossNum);
				if (writeToDB)
				{
					GameManager.DBCmdMgr.AddDBCmd(10055, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.KillBoss), null, client.ServerId);
					long nowTicks = TimeUtil.NOW();
					Global.SetLastDBCmdTicks(client, 10055, nowTicks);
				}
			}
		}

		public void UpdateBattleNum(GameClient client, int addNum, bool writeToDB = false)
		{
			client.ClientData.BattleNum += addNum;
			if (writeToDB)
			{
				GameManager.DBCmdMgr.AddDBCmd(10064, string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.BattleNum), null, client.ServerId);
				long nowTicks = TimeUtil.NOW();
				Global.SetLastDBCmdTicks(client, 10064, nowTicks);
			}
		}

		public void ChangeRoleHeroIndex(SocketListener sl, TCPOutPacketPool pool, GameClient client, int heroIndex, bool force = false)
		{
			if (!force)
			{
				if (heroIndex <= 0)
				{
					return;
				}
				int heroIndex2 = client.ClientData.HeroIndex;
				if (heroIndex <= heroIndex2)
				{
					Global.BroadcastHeroMapOk(client, heroIndex, false);
					return;
				}
			}
			client.ClientData.HeroIndex = Math.Min(13, heroIndex);
			Global.BroadcastHeroMapOk(client, heroIndex, true);
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.HeroIndex);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 290);
			}
		}

		public void NotifyBossInfoDictData(GameClient client)
		{
			Dictionary<int, BossData> bossDictData = MonsterBossManager.GetBossDictData();
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<Dictionary<int, BossData>>(bossDictData, Global._TCPManager.TcpOutPacketPool, 268);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyYaBiaoData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<YaBiaoData>(client.ClientData.MyYaBiaoData, Global._TCPManager.TcpOutPacketPool, 270);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyOtherBiaoCheLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int biaoCheID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", biaoCheID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strCmd, 279);
			}
		}

		public void NotifyMySelfNewBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem biaoCheItem)
		{
			BiaoCheData instance = Global.BiaoCheItem2BiaoCheData(biaoCheItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<BiaoCheData>(instance, pool, 276);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMySelfDelBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, int biaoCheID)
		{
			string data = string.Format("{0}", biaoCheID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 277);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllPopupWinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (client == null || nextClient != client)
				{
					if (!nextClient.ClientSocket.IsKuaFuLogin)
					{
						this.NotifyPopupWinMsg(sl, pool, nextClient, strcmd);
					}
				}
			}
		}

		public void NotifyPopupWinMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 284);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		private void ChangeDayLoginNum(GameClient client)
		{
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (dayOfYear < client.ClientData.LoginDayID && Math.Abs(dayOfYear - client.ClientData.LoginDayID) < 2)
			{
				LogManager.WriteLog(2, string.Format("玩家退后登陆了！！rid={0}, rname={1}", client.ClientData.RoleID, client.ClientData.RoleName), null, true);
			}
			else if (dayOfYear != client.ClientData.LoginDayID)
			{
				client.ClientData.LoginDayID = dayOfYear;
				HuodongCachingMgr.OnJieriRoleLogin(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID), false);
				ChengJiuManager.OnRoleLogin(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID));
				HuodongCachingMgr.ProcessDayOnlineSecs(client, Global.SafeConvertToInt32(client.ClientData.MyHuodongData.LastDayID));
				HuodongCachingMgr.ResetRegressActiveOpen();
				UserRegressActiveManager.getInstance().RoleOnlineHandler(client);
				bool flag = Global.UpdateWeekLoginNum(client);
				flag |= Global.UpdateLimitTimeLoginNum(client);
				if (flag)
				{
					GameManager.ClientMgr.NotifyHuodongData(client);
				}
				Global.UpdateHuoDongDBCommand(Global._TCPManager.TcpOutPacketPool, client);
				Global.GiveGuMuTimeLimitAward(client);
				Global.InitRoleDailyTaskData(client, true);
				CompManager.getInstance().HandleCompTaskSomething(client, false);
				CaiJiLogic.InitRoleDailyCaiJiData(client, false, true);
				HuanYingSiYuanManager.getInstance().InitRoleDailyHYSYData(client);
				Global.ProcessUpdateFuBenData(client);
				CGetOldResourceManager.InitRoleOldResourceInfo(client, true);
				Global.UpdateHeFuLoginFlag(client);
				Global.UpdateHeFuTotalLoginFlag(client);
				if (client._IconStateMgr.CheckHeFuActivity(client) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client) || client._IconStateMgr.CheckPaiHangState(client) || client._IconStateMgr.CheckJieRiPCKingEveryDay(client) || client._IconStateMgr.CheckSpecPriorityActivity(client))
				{
					client._IconStateMgr.SendIconStateToClient(client);
				}
				YueKaManager.UpdateNewDay(client);
				UserReturnManager.getInstance().initUserReturnData(client);
				OlympicsManager.getInstance().CheckOlympicsOpenState(TimeUtil.NOW(), true);
				OlympicsManager.getInstance().CheckTip(client);
				FundManager.initFundData(client);
				MarriageOtherLogic.getInstance().ChangeDayUpdate(client, true);
				MarryPartyLogic.getInstance().MarryPartyJoinListClear(client, true);
				Global.UpdateRoleLoginRecord(client);
				JieriGiveActivity jieriGiveActivity = HuodongCachingMgr.GetJieriGiveActivity();
				if (jieriGiveActivity != null)
				{
					jieriGiveActivity.UpdateNewDay(client);
				}
				JieriRecvActivity jieriRecvActivity = HuodongCachingMgr.GetJieriRecvActivity();
				if (jieriRecvActivity != null)
				{
					jieriRecvActivity.UpdateNewDay(client);
				}
				client.sendCmd(833, string.Format("{0}", TimeUtil.NOW() * 10000L), false);
				SingletonTemplate<SevenDayActivityMgr>.Instance().OnNewDay(client);
				SingletonTemplate<ZhengBaManager>.Instance().OnNewDay(client);
				SpecPlatFuLiManager.getInstance().OnNewDay(client);
				RebornManager.getInstance().OnLogin(client, false);
				GameManager.ClientMgr.ModifyRebornEquipHoleValue(client, -Global.GetRoleParamsInt32FromDB(client, "10255"), "跨天重生槽免费次数重置", true, true, false);
			}
		}

		public void ChangeAllThingAddPropIndexs(GameClient client)
		{
			string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				client.ClientData.RoleID,
				client.ClientData.AllQualityIndex,
				client.ClientData.AllForgeLevelIndex,
				client.ClientData.AllJewelLevelIndex,
				client.ClientData.AllZhuoYueNum
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 292);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllChangeHalfYinLiangPeriod(int halfYinLiangPeriod)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				string data = string.Format("{0}:{1}", nextClient.ClientData.RoleID, halfYinLiangPeriod);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 293);
				if (!Global._TCPManager.MySocketListener.SendData(nextClient.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void ChangeBangHuiName(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.Faction,
					client.ClientData.BHName,
					client.ClientData.BHZhiWu
				});
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 296);
			}
		}

		public void ChangeBangHuiZhiWu(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, client.ClientData.BHZhiWu);
				if (client.ClientData.BHZhiWu > 0)
				{
					string text = "";
					if (client.ClientData.BHZhiWu == 1)
					{
						text = GLang.GetLang(78, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 2)
					{
						text = GLang.GetLang(79, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 3)
					{
						text = GLang.GetLang(80, new object[0]);
					}
					else if (client.ClientData.BHZhiWu == 4)
					{
						text = GLang.GetLang(81, new object[0]);
					}
					Global.BroadcastBangHuiMsg(client.ClientData.RoleID, client.ClientData.Faction, StringUtil.substitute(GLang.GetLang(82, new object[0]), new object[]
					{
						client.ClientData.RoleName,
						text
					}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				}
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 329);
			}
		}

		public void NotifyOnlineBangHuiMgrRoleApplyMsg(int roleID, string roleName, int bhid, string bhName, string roleList)
		{
			if (!string.IsNullOrEmpty(roleList))
			{
				string[] array = roleList.Split(new char[]
				{
					','
				});
				if (array != null && array.Length > 0)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(roleID);
					if (gameClient != null)
					{
						for (int i = 0; i < array.Length; i++)
						{
							int num = Global.SafeConvertToInt32(array[i]);
							if (num > 0)
							{
								GameClient gameClient2 = GameManager.ClientMgr.FindClient(num);
								if (null != gameClient2)
								{
									string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
									{
										roleID,
										roleName,
										gameClient.ClientData.Occupation,
										gameClient.ClientData.Level,
										gameClient.ClientData.ChangeLifeCount,
										bhid,
										bhName
									});
									TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 301);
									if (!Global._TCPManager.MySocketListener.SendData(gameClient2.ClientSocket, tcpOutPacket, true))
									{
									}
								}
							}
						}
					}
				}
			}
		}

		public void NotifyInviteToBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int inviteRoleID, string inviteRoleName, int bhid, string bhName, int nChangelifeLev)
		{
			if (Global.GetUnionLevel(otherClient, false) >= Global.JoinBangHuiNeedLevel)
			{
				string data = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					inviteRoleID,
					inviteRoleName,
					bhid,
					bhName,
					nChangelifeLev
				});
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 307);
				if (!sl.SendData(otherClient.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyJoinBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int bhid, string bhName)
		{
			if (otherClient.ClientData.Faction <= 0)
			{
				otherClient.ClientData.Faction = bhid;
				otherClient.ClientData.BHName = bhName;
				otherClient.ClientData.BHZhiWu = 0;
				GameManager.ClientMgr.ChangeBangHuiName(sl, pool, otherClient);
				GlobalEventSource4Scene.getInstance().fireEvent(new PostBangHuiChangeEventObject(otherClient, bhid), 10000);
				Global.SaveRoleParamsInt32ValueToDB(otherClient, "EnterBangHuiUnixSecs", DataHelper.UnixSecondsNow(), true);
				Global.SaveRoleParamsDateTimeToDB(otherClient, "10182", TimeUtil.NowDateTime(), true);
				int junQiLevelByBHID = JunQiManager.GetJunQiLevelByBHID(otherClient.ClientData.Faction);
				Global.UpdateBufferData(otherClient, BufferItemTypes.JunQi, new double[]
				{
					(double)junQiLevelByBHID - 1.0
				}, 1, true);
				Global.BroadcastBangHuiMsg(otherClient.ClientData.RoleID, bhid, StringUtil.substitute(GLang.GetLang(83, new object[0]), new object[]
				{
					otherClient.ClientData.RoleName,
					otherClient.ClientData.BHName
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				ChengJiuManager.OnFirstInFaction(otherClient);
				UnionPalaceManager.initSetUnionPalaceProps(otherClient, true);
				Global.UpdateChengHaoBuff(otherClient);
				otherClient._IconStateMgr.CheckGuildIcon(otherClient, false);
			}
		}

		public void NotifyLeaveBangHui(SocketListener sl, TCPOutPacketPool pool, GameClient otherClient, int bhid, string bhName, int leaveType)
		{
			if (otherClient.ClientData.Faction > 0)
			{
				MoYuLongXue.OnClientLeaveBangHui(bhid, otherClient.ClientData.RoleID);
				Global.BroadcastBangHuiMsg(otherClient.ClientData.RoleID, bhid, StringUtil.substitute(GLang.GetLang(84, new object[0]), new object[]
				{
					otherClient.ClientData.RoleName,
					(leaveType <= 0) ? GLang.GetLang(85, new object[0]) : GLang.GetLang(86, new object[0]),
					bhName
				}), true, GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox);
				otherClient.ClientData.Faction = 0;
				otherClient.ClientData.BHName = "";
				otherClient.ClientData.BHZhiWu = 0;
				GameManager.ClientMgr.ChangeBangHuiName(sl, pool, otherClient);
				GlobalEventSource4Scene.getInstance().fireEvent(new PostBangHuiChangeEventObject(otherClient, bhid), 10000);
				Global.RemoveBufferData(otherClient, 16);
				UnionPalaceManager.initSetUnionPalaceProps(otherClient, true);
				GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient);
				GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, true, false, 7);
				Global.UpdateChengHaoBuff(otherClient);
			}
		}

		public void NotifyBangHuiDestroy(int retCode, int roleID, int bhid)
		{
			MoYuLongXue.OnBangHuiDestroy(bhid);
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Faction == bhid)
				{
					if (!nextClient.ClientSocket.IsKuaFuLogin)
					{
						nextClient.ClientData.Faction = 0;
						nextClient.ClientData.BHName = "";
						nextClient.ClientData.BHZhiWu = 0;
						string data = string.Format("{0}:{1}:{2}", retCode, roleID, bhid);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 305);
						if (!Global._TCPManager.MySocketListener.SendData(nextClient.ClientSocket, tcpOutPacket, true))
						{
						}
						Global.RemoveBufferData(nextClient, 16);
						UnionPalaceManager.initSetUnionPalaceProps(nextClient, true);
						nextClient.ClientData.AllyList = null;
						GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient);
						GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, nextClient, true, false, 7);
					}
				}
			}
		}

		public void NotifyBangHuiUpLevel(int bhid, int serverID, int level, bool isKF)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Faction == bhid && !nextClient.ClientSocket.IsKuaFuLogin)
				{
					UnionPalaceManager.initSetUnionPalaceProps(nextClient, true);
				}
			}
			if (AllyManager.getInstance().IsAllyOpen(level))
			{
				AllyManager.getInstance().UnionDataChange(bhid, serverID, false, 0);
			}
		}

		public void NotifyBangHuiChangeName(int bhid, string newName)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Faction == bhid)
				{
					if (!nextClient.ClientSocket.IsKuaFuLogin)
					{
						nextClient.ClientData.BHName = newName;
						string data = string.Format("{0}:{1}", bhid, newName);
						TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 1315);
						if (!Global._TCPManager.MySocketListener.SendData(nextClient.ClientSocket, tcpOutPacket, true))
						{
						}
					}
				}
			}
		}

		public void NotifyRefuseApplyToBHMember(GameClient otherClient, string bhRoleName, string bhName)
		{
			if (otherClient.ClientData.Faction <= 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(87, new object[0]), new object[]
				{
					bhRoleName,
					bhName
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
		}

		public void NotifyRefuseInviteToBHMember(GameClient otherClient, string bhRoleName, string bhName)
		{
			if (otherClient.ClientData.Faction > 0)
			{
				GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, otherClient, StringUtil.substitute(GLang.GetLang(88, new object[0]), new object[]
				{
					bhRoleName,
					bhName
				}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
			}
		}

		public void NotifySelfBangGongChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.BangGong, client.ClientData.BGMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 316);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddBangGong(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, ref int addBangGong, AddBangGongTypes addBangGongType, int nBangGongLimit = 0)
		{
			int bangGong = client.ClientData.BangGong;
			int dayOfYear = TimeUtil.NowDateTime().DayOfYear;
			if (client.ClientData.BGDayID1 != dayOfYear)
			{
				client.ClientData.BGMoney = 0;
				client.ClientData.BGDayID1 = dayOfYear;
			}
			if (client.ClientData.BGDayID2 != dayOfYear)
			{
				client.ClientData.BGGoods = 0;
				client.ClientData.BGDayID2 = dayOfYear;
			}
			if (AddBangGongTypes.BGGold == addBangGongType)
			{
				int bgmoney = client.ClientData.BGMoney;
				client.ClientData.BGMoney = Global.GMin(client.ClientData.BGMoney + addBangGong, nBangGongLimit);
				addBangGong = client.ClientData.BGMoney - bgmoney;
			}
			else if (AddBangGongTypes.BGGoods == addBangGongType)
			{
				int bggoods = client.ClientData.BGGoods;
				client.ClientData.BGGoods = Global.GMin(client.ClientData.BGGoods + addBangGong, nBangGongLimit);
				addBangGong = client.ClientData.BGGoods - bggoods;
			}
			bool result;
			if (0 == addBangGong)
			{
				result = true;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.BGDayID1,
					client.ClientData.BGMoney,
					client.ClientData.BGDayID2,
					client.ClientData.BGGoods,
					addBangGong
				});
				string[] array = Global.ExecuteDBCmd(10071, strcmd, client.ServerId);
				if (null == array)
				{
					result = false;
				}
				else if (array.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(array[1]) < 0)
				{
					result = false;
				}
				else
				{
					client.ClientData.BangGong = Convert.ToInt32(array[1]);
					ChengJiuManager.OnRoleGuildChengJiu(client);
					GameManager.ClientMgr.NotifySelfBangGongChange(sl, pool, client);
					Global.AddRoleBangGongEvent(client, bangGong);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BangGong, (long)addBangGong, (long)client.ClientData.BangGong, addBangGongType.ToString());
					result = true;
				}
			}
			return result;
		}

		public bool AddBangGong(GameClient client, ref int addBangGong, AddBangGongTypes addBangGongType, int nBangGongLimit = 0)
		{
			return this.AddBangGong(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, ref addBangGong, addBangGongType, nBangGongLimit);
		}

		public bool SubUserBangGong(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subBangGong)
		{
			int bangGong = client.ClientData.BangGong;
			bool result;
			if (client.ClientData.BangGong < subBangGong)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", new object[]
				{
					client.ClientData.RoleID,
					client.ClientData.BGDayID1,
					client.ClientData.BGMoney,
					client.ClientData.BGDayID2,
					client.ClientData.BGGoods,
					-subBangGong
				});
				string[] array = Global.ExecuteDBCmd(10071, strcmd, client.ServerId);
				if (null == array)
				{
					result = false;
				}
				else if (array.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(array[1]) < 0)
				{
					result = false;
				}
				else
				{
					client.ClientData.BangGong = Convert.ToInt32(array[1]);
					GameManager.ClientMgr.NotifySelfBangGongChange(sl, pool, client);
					Global.AddRoleBangGongEvent(client, bangGong);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BangGong, (long)(-(long)subBangGong), (long)client.ClientData.BangGong, "none");
					result = true;
				}
			}
			return result;
		}

		public bool AddBangHuiTongQian(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int bhid, int addMoney)
		{
			int num = (client == null) ? 0 : client.ClientData.RoleID;
			int serverId = (client == null) ? GameManager.ServerId : client.ServerId;
			string strcmd = string.Format("{0}:{1}:{2}", num, bhid, addMoney);
			string[] array = Global.ExecuteDBCmd(10077, strcmd, serverId);
			bool result;
			if (null == array)
			{
				result = false;
			}
			else if (array.Length != 2)
			{
				result = false;
			}
			else if (Convert.ToInt32(array[0]) < 0)
			{
				result = false;
			}
			else
			{
				if (null != client)
				{
					GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, bhid);
				}
				result = true;
			}
			return result;
		}

		public bool SubBangHuiTongQian(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int subMoney, out int bhZoneID)
		{
			bhZoneID = 0;
			bool result;
			if (client.ClientData.Faction <= 0)
			{
				result = false;
			}
			else
			{
				string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, client.ClientData.Faction, subMoney);
				string[] array = Global.ExecuteDBCmd(10072, strcmd, client.ServerId);
				if (null == array)
				{
					result = false;
				}
				else if (array.Length != 2)
				{
					result = false;
				}
				else if (Convert.ToInt32(array[0]) < 0)
				{
					result = false;
				}
				else
				{
					bhZoneID = Global.SafeConvertToInt32(array[1]);
					GameManager.ClientMgr.NotifyBangHuiZiJinChanged(client, client.ClientData.Faction);
					result = true;
				}
			}
			return result;
		}

		public void NotifyBangHuiZiJinChanged(GameClient client, int bhid)
		{
			int roleID = client.ClientData.RoleID;
			BangHuiDetailData bangHuiDetailData = Global.GetBangHuiDetailData(roleID, bhid, 0);
			if (null != bangHuiDetailData)
			{
				if (roleID != bangHuiDetailData.BZRoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(bangHuiDetailData.BZRoleID);
					if (null != gameClient)
					{
						gameClient.sendCmd(709, string.Format("{0}:{1}", bhid, bangHuiDetailData.TotalMoney), false);
					}
				}
				if (client.ClientData.Faction == bhid)
				{
					client.sendCmd(709, string.Format("{0}:{1}", bhid, bangHuiDetailData.TotalMoney), false);
				}
			}
		}

		public void NotifyOtherJunQiLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int junQiID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", junQiID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strCmd, 320);
			}
		}

		public void NotifyMySelfNewJunQi(SocketListener sl, TCPOutPacketPool pool, GameClient client, JunQiItem junQiItem)
		{
			JunQiData instance = Global.JunQiItem2JunQiData(junQiItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<JunQiData>(instance, pool, 321);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMySelfDelJunQi(SocketListener sl, TCPOutPacketPool pool, GameClient client, int junQiID)
		{
			string data = string.Format("{0}", junQiID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 322);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyLingDiForBHMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 323);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllLingDiForBHMsg(SocketListener sl, TCPOutPacketPool pool, int lingDiID, int bhid, int zoneID, string bhName, int tax)
		{
			string strcmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
			{
				lingDiID,
				bhid,
				zoneID,
				bhName,
				tax
			});
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (!nextClient.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyLingDiForBHMsg(sl, pool, nextClient, strcmd);
				}
			}
		}

		public void NotifyAllLuoLanChengZhanRequestInfoList(List<LuoLanChengZhanRequestInfoEx> list)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (!nextClient.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyLuoLanChengZhanRequestInfoList(nextClient, list);
				}
			}
		}

		public void NotifyLuoLanChengZhanRequestInfoList(GameClient client, List<LuoLanChengZhanRequestInfoEx> list)
		{
			client.sendCmd<List<LuoLanChengZhanRequestInfoEx>>(708, list, false);
		}

		public void HandleBHJunQiUpLevel(int bhid, int junQiLevel)
		{
			double[] actionParams = new double[]
			{
				(double)junQiLevel - 1.0
			};
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.Faction == bhid)
				{
					Global.UpdateBufferData(nextClient, BufferItemTypes.JunQi, actionParams, 1, true);
				}
			}
		}

		public void NotifyOtherFakeRoleLifeV(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int FakeRoleID, int currentLifeV)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}:{1}", FakeRoleID, currentLifeV);
				this.SendToClients(sl, pool, null, objsList, strCmd, 588);
			}
		}

		public void NotifyMySelfNewFakeRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, FakeRoleItem FakeRoleItem)
		{
			FakeRoleData instance = Global.FakeRoleItem2FakeRoleData(FakeRoleItem);
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<FakeRoleData>(instance, pool, 589);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMySelfDelFakeRole(SocketListener sl, TCPOutPacketPool pool, GameClient client, int FakeRoleID)
		{
			string data = string.Format("{0}", FakeRoleID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 590);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllDelFakeRole(SocketListener sl, TCPOutPacketPool pool, FakeRoleItem fakeRoleItem)
		{
			List<object> all9Clients = Global.GetAll9Clients(fakeRoleItem);
			string strCmd = string.Format("{0}", fakeRoleItem.FakeRoleID);
			this.SendToClients(sl, pool, null, all9Clients, strCmd, 590);
		}

		public void NotifyChgHuangDiRoleIDMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string strcmd)
		{
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, strcmd, 324);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllChgHuangDiRoleIDMsg(SocketListener sl, TCPOutPacketPool pool, int oldHuangDiRoleID, int huangDiRoleID)
		{
			string strcmd = string.Format("{0}:{1}", oldHuangDiRoleID, huangDiRoleID);
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (!nextClient.ClientSocket.IsKuaFuLogin)
				{
					this.NotifyChgHuangDiRoleIDMsg(sl, pool, nextClient, strcmd);
				}
			}
		}

		public void NotifyInviteAddHuangFei(GameClient client, int otherRoleID, string otherRoleName, int randNum)
		{
			string data = string.Format("{0}:{1}:{2}", otherRoleID, otherRoleName, randNum);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 351);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyChgHuangHou(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.HuangHou);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 347);
			}
		}

		public void NotifyLingDiMapInfoData(GameClient client, LingDiMapInfoData lingDiMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<LingDiMapInfoData>(lingDiMapInfoData, Global._TCPManager.TcpOutPacketPool, 348);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllLingDiMapInfoData(int mapCode, LingDiMapInfoData lingDiMapInfoData)
		{
			List<object> list = this.GetMapClients(mapCode);
			if (null != list)
			{
				list = Global.ConvertObjsList(mapCode, -1, list, false);
				byte[] bytesData = DataHelper.ObjectToBytes<LingDiMapInfoData>(lingDiMapInfoData);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, bytesData, 348);
			}
		}

		public void NotifyHuangChengMapInfoData(GameClient client, HuangChengMapInfoData huangChengMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<HuangChengMapInfoData>(huangChengMapInfoData, Global._TCPManager.TcpOutPacketPool, 349);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllHuangChengMapInfoData(int mapCode, HuangChengMapInfoData huangChengMapInfoData)
		{
			List<object> list = this.GetMapClients(mapCode);
			if (null != list)
			{
				list = Global.ConvertObjsList(mapCode, -1, list, false);
				byte[] bytesData = DataHelper.ObjectToBytes<HuangChengMapInfoData>(huangChengMapInfoData);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, bytesData, 349);
			}
		}

		public void NotifyWangChengMapInfoData(GameClient client, WangChengMapInfoData wangChengMapInfoData)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<WangChengMapInfoData>(wangChengMapInfoData, Global._TCPManager.TcpOutPacketPool, 454);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllWangChengMapInfoData(WangChengMapInfoData wangChengMapInfoData)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				this.NotifyWangChengMapInfoData(nextClient, wangChengMapInfoData);
			}
		}

		public bool AddLingDiTaxMoney(int bhid, int lingDiID, int addMoney)
		{
			string strcmd = string.Format("{0}:{1}:{2}", bhid, lingDiID, addMoney);
			string[] array = Global.ExecuteDBCmd(350, strcmd, 0);
			return null != array && array.Length == 4 && Convert.ToInt32(array[0]) >= 0;
		}

		public void NotifySelfSuiTangBattleAward(SocketListener sl, TCPOutPacketPool pool, GameClient client, int nPoint1, int nPoint2, long experience, int bindYuanBao, int chengJiu, bool bIsSuccess, int paiMing, string awardsGoods)
		{
			int battleKilledNum = client.ClientData.BattleKilledNum;
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}", new object[]
			{
				client.ClientData.RoleID,
				bIsSuccess,
				nPoint1,
				nPoint2,
				battleKilledNum,
				experience,
				chengJiu,
				bindYuanBao,
				paiMing,
				awardsGoods
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 360);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool NotifyLastUserMail(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, int mailID)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, mailID);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 369);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
			return true;
		}

		public void SendMailWhenPacketFull(GameClient client, List<GoodsData> awardsItemList, string sContent, string sSubject)
		{
			int num = awardsItemList.Count / 5;
			int num2 = awardsItemList.Count % 5;
			int num3 = 0;
			if (num > 0)
			{
				for (int i = 0; i < num; i++)
				{
					List<GoodsData> list = new List<GoodsData>();
					for (int j = 0; j < 5; j++)
					{
						list.Add(awardsItemList[num3]);
						num3++;
					}
					Global.UseMailGivePlayerAward2(client, list, sContent, sSubject, 0, 0, 0);
				}
			}
			if (num2 > 0)
			{
				List<GoodsData> list2 = new List<GoodsData>();
				for (int i = 0; i < num2; i++)
				{
					list2.Add(awardsItemList[num3]);
					num3++;
				}
				Global.UseMailGivePlayerAward2(client, list2, sContent, sSubject, 0, 0, 0);
			}
		}

		public void NotifyVipDailyData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<List<VipDailyData>>(client.ClientData.VipDailyDataList, Global._TCPManager.TcpOutPacketPool, 389);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyYangGongBKAwardDailyData(GameClient client)
		{
			TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<YangGongBKDailyJiFenData>(client.ClientData.YangGongBKDailyJiFen, Global._TCPManager.TcpOutPacketPool, 392);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyAllShengXiaoGuessStateMsg(SocketListener sl, TCPOutPacketPool pool, int shengXiaoGuessState, int extraParams, int minLevel, int preGuessResult)
		{
			string data = string.Format("{0}:{1}:{2}", shengXiaoGuessState, extraParams, preGuessResult);
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(GameManager.ShengXiaoGuessMgr.GuessMapCode);
			if (null != mapClients)
			{
				TCPOutPacket tcpoutPacket = null;
				try
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							if (gameClient.ClientData.Level >= minLevel)
							{
								if (null == tcpoutPacket)
								{
									tcpoutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 398);
								}
								if (!sl.SendData(gameClient.ClientSocket, tcpoutPacket, false))
								{
								}
							}
						}
					}
				}
				finally
				{
					this.PushBackTcpOutPacket(tcpoutPacket);
				}
			}
		}

		public void NotifyShengXiaoGuessResultMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, string sResult)
		{
			if (null != client)
			{
				string data = string.Format("{0}:{1}", client.ClientData.RoleID, sResult);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 399);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyClientShengXiaoGuessStateMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, int shengXiaoGuessState, int extraParams, int minLevel, int preGuessResult)
		{
			if (client != null && client.ClientData.Level >= minLevel)
			{
				string data = string.Format("{0}:{1}:{2}", shengXiaoGuessState, extraParams, preGuessResult);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 398);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyMySelfNewNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, NPC npc)
		{
			if (npc != null && null != npc.RoleBufferData)
			{
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, npc.RoleBufferData, 0, npc.RoleBufferData.Length, 406);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyMySelfNewNPCBy9Grid(SocketListener sl, TCPOutPacketPool pool, NPC npc)
		{
			if (npc != null && null != npc.RoleBufferData)
			{
				List<object> all9GridObjects = Global.GetAll9GridObjects(npc);
				for (int i = 0; i < all9GridObjects.Count; i++)
				{
					if (all9GridObjects[i] is GameClient)
					{
						this.NotifyMySelfNewNPC(sl, pool, all9GridObjects[i] as GameClient, npc);
					}
				}
			}
		}

		public void NotifyMySelfDelNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, int mapCode, int npcID)
		{
			string data = string.Format("{0}:{1}", npcID, mapCode);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 407);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyMySelfDelNPC(SocketListener sl, TCPOutPacketPool pool, GameClient client, NPC npc)
		{
			this.NotifyMySelfDelNPC(sl, pool, client, npc.MapCode, npc.NpcID);
		}

		public void NotifyMySelfDelNPCBy9Grid(SocketListener sl, TCPOutPacketPool pool, NPC npc)
		{
			List<object> all9GridObjects = Global.GetAll9GridObjects(npc);
			for (int i = 0; i < all9GridObjects.Count; i++)
			{
				if (all9GridObjects[i] is GameClient)
				{
					this.NotifyMySelfDelNPC(sl, pool, all9GridObjects[i] as GameClient, npc.MapCode, npc.NpcID);
				}
			}
		}

		private bool TryDirectMove(GameClient client, long startMoveTicks, List<Point> path)
		{
			int num = (int)path[path.Count - 1].X;
			int num2 = (int)path[path.Count - 1].Y;
			bool result;
			if (Global.GetTwoPointDistance(client.CurrentGrid, new Point((double)num, (double)num2)) >= 3.0)
			{
				result = false;
			}
			else if (path.Count > 2)
			{
				result = false;
			}
			else
			{
				for (int i = 0; i < path.Count; i++)
				{
					Point currentGrid = client.CurrentGrid;
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int num3 = (int)path[i].X;
					int num4 = (int)path[i].Y;
					if (num3 != (int)currentGrid.X || num4 != (int)currentGrid.Y)
					{
						if (Global.InObsByGridXY(ObjectTypes.OT_CLIENT, client.ClientData.MapCode, num3, num4, 0, 0))
						{
							int roleDirection = client.ClientData.RoleDirection;
							int tryRun = 0;
							GameManager.ClientMgr.NotifyOthersMyMovingEnd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.MapCode, 0, client.ClientData.PosX, client.ClientData.PosY, roleDirection, tryRun, true, null);
							break;
						}
						int num5 = num3 * mapGrid.MapGridWidth + mapGrid.MapGridWidth / 2;
						int num6 = num4 * mapGrid.MapGridHeight + mapGrid.MapGridHeight / 2;
						client.ClientData.PosX = num5;
						client.ClientData.PosY = num6;
						mapGrid.MoveObject(-1, -1, num5, num6, client);
						client.ClientData.CurrentAction = 0;
					}
				}
				result = true;
			}
			return result;
		}

		public bool StartClientStoryboard(GameClient client, long startMoveTicks, List<Point> path, bool stepMove)
		{
			StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			bool result;
			if (path.Count <= 1)
			{
				result = false;
			}
			else
			{
				path.RemoveAt(0);
				if (this.TryDirectMove(client, startMoveTicks, path))
				{
					ClientManager.DoSpriteMapGridMove(client, 1);
					result = true;
				}
				else if (stepMove)
				{
					result = false;
				}
				else
				{
					StoryBoard4Client storyBoard4Client = new StoryBoard4Client(client.ClientData.RoleID);
					storyBoard4Client.Completed = new StoryBoard4Client.CompletedDelegateHandle(this.Move_Completed);
					GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
					long num = TimeUtil.NOW() * 10000L - 621356256000000000L;
					num /= 10000L;
					startMoveTicks -= 62135625600000L;
					long elapsedTicks = 0L;
					storyBoard4Client.Start(client, path, gameMap.MapGridWidth, gameMap.MapGridHeight, elapsedTicks);
					storyBoard4Client.Binding();
					result = true;
				}
			}
			return result;
		}

		public void Move_Completed(object sender, EventArgs e)
		{
			StoryBoard4Client storyBoard4Client = sender as StoryBoard4Client;
			StoryBoard4Client.RemoveStoryBoard(storyBoard4Client.RoleID);
			if (storyBoard4Client.IsStopped())
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(storyBoard4Client.RoleID);
				if (null != gameClient)
				{
					GameMap gameMap = GameManager.MapMgr.DictMaps[gameClient.ClientData.MapCode];
					int num = gameMap.CorrectWidthPointToGridPoint(gameClient.ClientData.PosX);
					int num2 = gameMap.CorrectHeightPointToGridPoint(gameClient.ClientData.PosY);
					gameClient.ClientData.PosX = num;
					gameClient.ClientData.PosY = num2;
					gameClient.ClientData.CurrentAction = 0;
					int roleDirection = gameClient.ClientData.RoleDirection;
					int tryRun = 1;
					GameManager.ClientMgr.NotifyOthersMyMovingEnd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, gameClient.ClientData.MapCode, 0, num, num2, roleDirection, tryRun, true, null);
					ClientManager.DoSpriteMapGridMove(gameClient, 0);
				}
			}
			else
			{
				GameClient gameClient = GameManager.ClientMgr.FindClient(storyBoard4Client.RoleID);
				if (null != gameClient)
				{
					gameClient.ClientData.CurrentAction = 0;
					gameClient.ClientData.MoveSpeed = 1.0;
					gameClient.ClientData.DestPoint = new Point((double)gameClient.ClientData.PosX, (double)gameClient.ClientData.PosY);
					ClientManager.DoSpriteMapGridMove(gameClient, 0);
				}
			}
		}

		public bool StopClientStoryboard(GameClient client, long clientTicks = 0L, int posX = -1, int posY = -1)
		{
			if (clientTicks > 0L)
			{
				StoryBoard4Client storyBoard4Client = StoryBoard4Client.StopStoryBoard(client.ClientData.RoleID, clientTicks);
				if (storyBoard4Client != null && posX >= 0)
				{
					if (Math.Abs(storyBoard4Client.LastPoint.X - (double)posX) + Math.Abs(storyBoard4Client.LastPoint.Y - (double)posY) >= 300.0)
					{
						ClientCmdCheck.ResetClientPosition(client, storyBoard4Client.CurrentX, storyBoard4Client.CurrentY);
						return false;
					}
				}
			}
			else
			{
				StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			}
			return true;
		}

		public void StopClientStoryboard(GameClient client, int stopIndex)
		{
			if (stopIndex > 0)
			{
				StoryBoard4Client.StopStoryBoard(client.ClientData.RoleID, stopIndex);
			}
			else
			{
				StoryBoard4Client.RemoveStoryBoard(client.ClientData.RoleID);
			}
		}

		public bool GetClientStoryboardLastPoint(GameClient client, out Point lastPoint)
		{
			lastPoint = new Point(0.0, 0.0);
			StoryBoard4Client storyBoard4Client = StoryBoard4Client.FindStoryBoard(client.ClientData.RoleID);
			bool result;
			if (null != storyBoard4Client)
			{
				lastPoint = storyBoard4Client.LastPoint;
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public bool AddEquipStrong(GameClient client, GoodsData goodsData, int subStrong)
		{
			int equipGoodsMaxStrong = Global.GetEquipGoodsMaxStrong(goodsData.GoodsID);
			bool result;
			if (goodsData.Strong >= equipGoodsMaxStrong)
			{
				result = false;
			}
			else
			{
				int strong = goodsData.Strong;
				int num = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
				goodsData.Strong = Math.Min(goodsData.Strong + subStrong, equipGoodsMaxStrong);
				int num2 = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
				bool flag = false;
				if (num != num2)
				{
					if (goodsData.Strong < equipGoodsMaxStrong)
					{
						Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
					}
					else
					{
						Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW() - 7200000L, false);
					}
					this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
					flag = true;
				}
				else
				{
					Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
				}
				if (strong < equipGoodsMaxStrong && goodsData.Strong >= equipGoodsMaxStrong)
				{
					if (!flag)
					{
						Global.UpdateEquipStrong(client, goodsData);
						this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
					}
					Global.RefreshEquipPropAndNotify(client);
				}
				result = true;
			}
			return result;
		}

		public int SubEquipStrong(GameClient client, GoodsData goodsData, int subStrong)
		{
			int num = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
			goodsData.Strong = Math.Max(0, goodsData.Strong - subStrong);
			int num2 = goodsData.Strong / Global.MaxNotifyEquipStrongValue;
			if (num != num2)
			{
				Global.UpdateEquipStrong(client, goodsData);
				this.NotifyMySelfEquipStrong(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, goodsData);
			}
			else
			{
				Global.SetLastDBEquipStrongCmdTicks(client, goodsData.Id, TimeUtil.NOW(), false);
			}
			return num2;
		}

		public void NotifyMySelfEquipStrong(SocketListener sl, TCPOutPacketPool pool, GameClient client, GoodsData goodsData)
		{
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, goodsData.Id, goodsData.Strong);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 412);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyDSHideCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.DSHideStart);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 422);
			}
		}

		public void CheckDSHideState(GameClient client)
		{
			if (client.ClientData.DSHideStart > 0L)
			{
				long num = TimeUtil.NOW();
				if (num >= client.ClientData.DSHideStart)
				{
					Global.RemoveBufferData(client, 41);
					client.ClientData.DSHideStart = 0L;
					GameManager.ClientMgr.NotifyDSHideCmd(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
				}
			}
		}

		public void NotifyRoleStatusCmd(SocketListener sl, TCPOutPacketPool pool, GameClient client, int statusID, long startTicks, int slotSeconds, double tag = 0.0)
		{
			switch (statusID)
			{
			case 2:
			case 3:
			case 8:
				ClientCmdCheck.MoveSpeedChange(client, tag);
				ClientCmdCheck.ClientAction(client, 0L, (long)(slotSeconds * 1000));
				break;
			case 4:
			case 11:
			case 12:
			case 14:
				ClientCmdCheck.MoveSpeedChange(client, tag);
				break;
			}
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					client.ClientData.RoleID,
					statusID,
					startTicks,
					slotSeconds,
					tag
				});
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 456);
			}
		}

		public void NotifyMonsterStatusCmd(SocketListener sl, TCPOutPacketPool pool, Monster monster, int statusID, long startTicks, int slotSeconds, double tag = 0.0)
		{
			List<object> all9Clients = Global.GetAll9Clients(monster);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
				{
					monster.RoleID,
					statusID,
					startTicks,
					slotSeconds,
					tag
				});
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 456);
			}
		}

		public void NotifyMySelfNewDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, Decoration deco)
		{
			if (null != deco)
			{
				DecorationData instance = new DecorationData
				{
					AutoID = deco.AutoID,
					DecoID = deco.DecoID,
					MapCode = deco.MapCode,
					PosX = (int)deco.Pos.X,
					PosY = (int)deco.Pos.Y,
					StartTicks = deco.StartTicks,
					MaxLiveTicks = deco.MaxLiveTicks,
					AlphaTicks = deco.AlphaTicks
				};
				TCPOutPacket tcpOutPacket = DataHelper.ObjectToTCPOutPacket<DecorationData>(instance, Global._TCPManager.TcpOutPacketPool, 423);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyMySelfDelDeco(SocketListener sl, TCPOutPacketPool pool, GameClient client, Decoration deco)
		{
			if (null != deco)
			{
				string data = string.Format("{0}", deco.AutoID);
				TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 424);
				if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
				{
				}
			}
		}

		public void NotifyOthersDelDeco(SocketListener sl, TCPOutPacketPool pool, List<object> objsList, int mapCode, int autoID)
		{
			if (null != objsList)
			{
				string strCmd = string.Format("{0}", autoID);
				this.SendToClients(sl, pool, null, objsList, strCmd, 424);
			}
		}

		public bool ModifyFuWenZhiChenPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)client.ClientData.FuWenZhiChen;
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.FuWenZhiChen = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "符文之尘", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.FuWenZhiChen, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10187", client.ClientData.FuWenZhiChen, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.FuWenZhiChen, (long)addValue, (long)client.ClientData.FuWenZhiChen, strFrom);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.FuWenZhiChen, client.ClientData.FuWenZhiChen);
					}
					result = true;
				}
			}
			return result;
		}

		public void ModifyShenLiJingHuaPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.ShenLiJingHuaPoints += addValue;
				client.ClientData.ShenLiJingHuaPoints = Math.Max(client.ClientData.ShenLiJingHuaPoints, 0);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神力精华", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ShenLiJingHuaPoints, client.ServerId, null);
				Global.SaveRoleParamsInt32ValueToDB(client, "10157", client.ClientData.ShenLiJingHuaPoints, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenLiJingHua, (long)addValue, (long)client.ClientData.ShenLiJingHuaPoints, strFrom);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenLiJingHua, client.ClientData.ShenLiJingHuaPoints);
				}
			}
		}

		public void ModifyChengJiuPointsValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.ChengJiuPoints += addValue;
				client.ClientData.ChengJiuPoints = Math.Max(client.ClientData.ChengJiuPoints, 0);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ChengJiuPoints, client.ServerId, null);
				ChengJiuManager.ModifyChengJiuExtraData(client, (uint)client.ClientData.ChengJiuPoints, ChengJiuExtraDataField.ChengJiuPoints, true);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ChengJiu, (long)addValue, (long)client.ClientData.ChengJiuPoints, strFrom);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiu, client.ClientData.ChengJiuPoints);
				}
				client._IconStateMgr.CheckChengJiuUpLevelState(client);
			}
		}

		public int GetChengJiuPointsValue(GameClient client)
		{
			client.ClientData.ChengJiuPoints = (int)ChengJiuManager.GetChengJiuExtraDataByField(client, ChengJiuExtraDataField.ChengJiuPoints);
			return client.ClientData.ChengJiuPoints;
		}

		public int SetChengJiuLevelValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			client.ClientData.ChengJiuLevel = ChengJiuManager.GetChengJiuLevel(client);
			client.ClientData.ChengJiuLevel += addValue;
			GameManager.logDBCmdMgr.AddDBLogInfo(-1, "成就等级", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.ChengJiuLevel, client.ServerId, null);
			ChengJiuManager.SetChengJiuLevel(client, client.ClientData.ChengJiuLevel, true);
			if (notifyClient)
			{
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ChengJiuLevel, client.ClientData.ChengJiuLevel);
			}
			return client.ClientData.ChengJiuLevel;
		}

		public void ModifyZhuangBeiJiFenValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetZhuangBeiJiFenValue(client) + addValue;
				this.SaveZhuangBeiJiFenValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhuangBeiJiFen, num);
				}
			}
		}

		public void SaveZhuangBeiJiFenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhuangBeiJiFen", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZhuangBeiJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhuangBeiJiFen", "2020-12-12 12:12:12");
		}

		public void ModifyLieShaValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetLieShaValue(client) + addValue;
				this.SaveLieShaValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.LieShaZhi, num);
				}
			}
		}

		public void SaveLieShaValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "LieShaZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetLieShaValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "LieShaZhi", "2020-12-12 12:12:12");
		}

		public void ModifyWuXingValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true, bool doChangeWuXueLevel = true)
		{
			if (0 != addValue)
			{
				int num = this.GetWuXingValue(client) + addValue;
				this.SaveWuXingValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.WuXingZhi, num);
				}
				if (doChangeWuXueLevel)
				{
					if (addValue > 0)
					{
						Global.TryToActivateSpecialWuXueLevel(client);
					}
					else
					{
						Global.TryToDeActivateSpecialWuXueLevel(client);
					}
				}
			}
		}

		public void SaveWuXingValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WuXingZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetWuXingValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WuXingZhi", "2020-12-12 12:12:12");
		}

		public void ModifyZhenQiValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetZhenQiValue(client) + addValue;
				this.SaveZhenQiValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhenQiZhi, num);
				}
			}
		}

		public void SaveZhenQiValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhenQiZhi", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZhenQiValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhenQiZhi", "2020-12-12 12:12:12");
		}

		public void ModifyStarSoulValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.StarSoul += addValue;
				if (client.ClientData.StarSoul < 0)
				{
					client.ClientData.StarSoul = 0;
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "星魂", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.StarSoul, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.XingHun, (long)addValue, (long)client.ClientData.StarSoul, strFrom);
				Global.SaveRoleParamsInt32ValueToDB(client, "StarSoul", client.ClientData.StarSoul, true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.StarSoulValue, client.ClientData.StarSoul);
				}
			}
		}

		public void ModifyPetJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = Convert.ToInt32(Global.GetRoleParamByName(client, "PetJiFen")) + addValue;
				Global.UpdateRoleParamByName(client, "PetJiFen", num.ToString(), true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.PetJiFen, num);
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "精灵积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
			}
		}

		public bool ModifyYuanSuFenMoValue(GameClient client, int addValue, string strFrom, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "ElementPowder");
				long num = (long)roleParamsInt32FromDB;
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					if (num3 == roleParamsInt32FromDB)
					{
						result = true;
					}
					else
					{
						addValue = num3 - roleParamsInt32FromDB;
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "元素粉末", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.YuanSuFenMo, (long)addValue, (long)num3, strFrom);
						Global.SaveRoleParamsInt32ValueToDB(client, "ElementPowder", num3, true);
						if (notifyClient)
						{
							GameManager.ClientMgr.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.YuansuFenmo, num3);
						}
						result = true;
					}
				}
			}
			return result;
		}

		public bool ModifyMUMoHeValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetMUMoHeValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔核", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MUMoHe, (long)addValue, (long)num3, strFrom);
					this.SaveMUMoHeValue(client, num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.MUMoHe, num3);
					}
					result = true;
				}
			}
			return result;
		}

		public void SaveMUMoHeValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "MUMoHe", nValue, writeToDB);
		}

		public int GetMUMoHeValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "MUMoHe");
		}

		public bool ModifyTianDiJingYuanValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetTianDiJingYuanValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔晶", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JingYuanZhi, (long)addValue, (long)num3, strFrom);
					this.SaveTianDiJingYuanValue(client, num3, true);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TianDiJingYuan, num3);
					}
					GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.MoJingCntInBag));
					result = true;
				}
			}
			return result;
		}

		public void SaveTianDiJingYuanValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "TianDiJingYuan", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetTianDiJingYuanValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "TianDiJingYuan", "2020-12-12 12:12:12");
		}

		public bool ModifyZaiZaoValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetZaiZaoValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "再造点", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZaiZao, (long)addValue, (long)num3, strFrom);
					this.SaveZaiZaoValue(client, num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZaiZaoPoint, num3);
					}
					result = true;
				}
			}
			return result;
		}

		public void SaveZaiZaoValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZaiZaoPoint", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZaiZaoValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZaiZaoPoint", "2020-12-12 12:12:12");
		}

		public void ModifyShiLianLingValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetShiLianLingValue(client) + addValue;
				this.SaveShiLianLingValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShiLianLing, num);
				}
			}
		}

		public void SaveShiLianLingValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShiLianLing", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetShiLianLingValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShiLianLing", "2020-12-12 12:12:12");
		}

		public void SaveJingMaiLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "JingMaiLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetJingMaiLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "JingMaiLevel", "2020-12-12 12:12:12");
		}

		public void SaveWuXueLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WuXueLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetWuXueLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WuXueLevel", "2020-12-12 12:12:12");
		}

		public void ModifyZuanHuangLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetZuanHuangLevelValue(client) + addValue;
				this.SaveZuanHuangLevelValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZuanHuangLevel, num);
				}
			}
		}

		public void SaveZuanHuangLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZuanHuangLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZuanHuangLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZuanHuangLevel", "2020-12-12 12:12:12");
		}

		public void ModifySystemOpenValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (addValue >= 0 && addValue <= 31)
			{
				int num = this.GetSystemOpenValue(client) | 1 << addValue;
				this.SaveSystemOpenValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.SystemOpenValue, num);
				}
			}
		}

		public int GetScoreBoxState(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ScoreBoxState", "2020-12-12 12:12:12");
		}

		public void ModifyScoreBoxState(GameClient client, int nOpen)
		{
			if (nOpen >= 0 && nOpen <= 2)
			{
				Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ScoreBoxState", nOpen, true, "2020-12-12 12:12:12");
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ScoreState, nOpen);
			}
		}

		public void SaveSystemOpenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "SystemOpenValue", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetSystemOpenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "SystemOpenValue", "2020-12-12 12:12:12");
		}

		public void ModifyJunGongValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetJunGongValue(client) + addValue;
				this.SaveJunGongValue(client, num, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JunGongZhi, (long)addValue, (long)num, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.JunGong, num);
				}
			}
		}

		public void SaveJunGongValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "JunGong", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetJunGongValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "JunGong", "2020-12-12 12:12:12");
		}

		public void ModifyKaiFuOnlineDayID(GameClient client, int dayID, bool writeToDB = false, bool notifyClient = true)
		{
			if (dayID >= 1 && dayID <= 7)
			{
				this.SaveKaiFuOnlineDayID(client, dayID, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.KaiFuOnlineDayID, dayID);
				}
			}
		}

		public void SaveKaiFuOnlineDayID(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "KaiFuOnlineDayID", nValue, writeToDB);
		}

		public int GetKaiFuOnlineDayID(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "KaiFuOnlineDayID");
		}

		public void ModifyTo60or100ID(GameClient client, int nID, bool writeToDB = false, bool notifyClient = true)
		{
			this.SaveTo60or100ID(client, nID, writeToDB);
			if (notifyClient)
			{
				this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.To60or100, nID);
			}
		}

		public void SaveTo60or100ID(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "To60or100", nValue, writeToDB);
		}

		public int GetTo60or100ID(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "To60or100");
		}

		public void ModifyTreasureJiFenValue(GameClient client, int addValue, string strFrom, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetTreasureJiFen(client) + addValue;
				if (addValue > 0)
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "藏宝积分", strFrom, "系统", client.ClientData.RoleName, "增加", addValue, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
				}
				else
				{
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "藏宝积分", strFrom, client.ClientData.RoleName, "系统", "减少", addValue, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
				}
				this.SaveTreasureJiFenValue(client, num, true);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BaoZangJiFen, (long)addValue, (long)num, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TreasureJiFen, num);
				}
			}
		}

		public void ModifyTreasureXueZuanValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetTreasureXueZuan(client) + addValue;
				this.SaveTreasureXueZuanValue(client, num, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BaoZangXueZuan, (long)addValue, (long)num, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.TreasureXueZuan, num);
				}
			}
		}

		public int GetTreasureJiFen(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "TreasureJiFen");
		}

		public void SaveTreasureJiFenValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "TreasureJiFen", nValue, writeToDB);
		}

		public int GetTreasureXueZuan(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "TreasureXueZuan");
		}

		public void SaveTreasureXueZuanValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "TreasureXueZuan", nValue, writeToDB);
		}

		public void ModifyZhanHunValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetZhanHunValue(client) + addValue;
				this.SaveZhanHunValue(client, num, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZhanHun, (long)addValue, (long)num, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhanHun, num);
				}
			}
		}

		public bool ModifyTianTiRongYaoValue(GameClient client, int addValue, string strFrom, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				client.ClientData.TianTiData.RongYao += addValue;
				RoleAttributeValueData cmdData = new RoleAttributeValueData
				{
					RoleAttribyteType = 0,
					Targetvalue = client.ClientData.TianTiData.RongYao,
					AddVAlue = addValue
				};
				Global.sendToDB<int, int[]>(10202, new int[]
				{
					client.ClientData.RoleID,
					client.ClientData.TianTiData.RongYao
				}, client.ServerId);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "荣耀", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, client.ClientData.TianTiData.RongYao, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TianTiRongYao, (long)addValue, (long)client.ClientData.TianTiData.RongYao, strFrom);
				if (notifyClient)
				{
					client.sendCmd<RoleAttributeValueData>(968, cmdData, false);
				}
			}
			return true;
		}

		public void SaveZhanHunValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhanHun", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZhanHunValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhanHun", "2020-12-12 12:12:12");
		}

		public void ModifyRongYuValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetRongYuValue(client) + addValue;
				this.SaveRongYuValue(client, num, writeToDB);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RongYu, (long)addValue, (long)num, "none");
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.RongYu, num);
				}
			}
		}

		public void SaveRongYuValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "RongYu", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetRongYuValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "RongYu", "2020-12-12 12:12:12");
		}

		public void ModifyZhanHunLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetZhanHunLevelValue(client) + addValue;
				this.SaveZhanHunLevelValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhanHunLevel, num);
				}
				Global.ActiveZhanHunBuffer(client, true);
			}
		}

		public void SaveZhanHunLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ZhanHunLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetZhanHunLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ZhanHunLevel", "2020-12-12 12:12:12");
		}

		public void ModifyRongYuLevelValue(GameClient client, int addValue, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetRongYuLevelValue(client) + addValue;
				this.SaveRongYuLevelValue(client, num, writeToDB);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.RongYuLevel, num);
				}
			}
		}

		public void SaveRongYuLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "RongYuLevel", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetRongYuLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "RongYuLevel", "2020-12-12 12:12:12");
		}

		public void ModifyShengWangValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetShengWangValue(client) + addValue;
				this.SaveShengWangValue(client, num, true);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWang, num);
				}
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShengWang, (long)addValue, (long)num, strFrom);
				if (addValue > 0)
				{
					client._IconStateMgr.CheckJingJiChangJunXian(client);
					client._IconStateMgr.SendIconStateToClient(client);
				}
			}
		}

		public void ModifyLangHunFenMoValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetLangHunFenMoValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "LangHunFenMo", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.LangHunFenMo, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "狼魂粉末", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.LangHunFenMo, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public int GetLangHunFenMoValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "LangHunFenMo");
		}

		public bool ModifyOrnamentCharmPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetOrnamentCharmPointValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10153", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.OrnamentCharmPoint, num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魅力点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.OrnamentCharmPoint, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetOrnamentCharmPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10153");
		}

		public bool ModifyBHMatchGuessJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetBHMatchGuessJiFenValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10190", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.BHMatchGuessJiFen, num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "战盟联赛竞猜点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.BHMatchGuessJiFen, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetBHMatchGuessJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10190");
		}

		public bool ModifyEraDonateValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetEraDonateValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					Global.SaveRoleParamsInt32ValueToDB(client, "10196", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.EraDonate, num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "纪元贡献点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXingZhiChen, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetEraDonateValue(GameClient client)
		{
			int result;
			if (0 == client.ClientData.JunTuanId)
			{
				result = 0;
			}
			else
			{
				int currentEraID = JunTuanClient.getInstance().GetCurrentEraID();
				if (currentEraID <= 0)
				{
					result = 0;
				}
				else
				{
					int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10197");
					int roleParamsInt32FromDB2 = Global.GetRoleParamsInt32FromDB(client, "10195");
					if (currentEraID != roleParamsInt32FromDB2 || roleParamsInt32FromDB != client.ClientData.JunTuanId)
					{
						Global.SaveRoleParamsInt32ValueToDB(client, "10197", client.ClientData.JunTuanId, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10195", currentEraID, true);
						Global.SaveRoleParamsInt32ValueToDB(client, "10196", 0, true);
					}
					result = Global.GetRoleParamsInt32FromDB(client, "10196");
				}
			}
			return result;
		}

		public int GetEraDonateValueOffline(int rid)
		{
			return Global.SafeConvertToInt32(Global.GetRoleParamsFromDBByRoleID(rid, "10196", 0));
		}

		public long GetRebornExpMaxAddValue(GameClient client, MoneyTypes types)
		{
			long result;
			if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = 0L;
			}
			else
			{
				int offsetDay = TimeUtil.GetOffsetDay(TimeUtil.NowDateTime());
				int roleParamsInt32FromDB = Global.GetRoleParamsInt32FromDB(client, "10243");
				if (roleParamsInt32FromDB != offsetDay)
				{
					Global.SaveRoleParamsInt64ValueToDB(client, "10244", 0L, true);
					Global.SaveRoleParamsInt64ValueToDB(client, "10245", 0L, true);
					Global.SaveRoleParamsInt32ValueToDB(client, "10243", offsetDay, true);
				}
				long num = 0L;
				if (types == MoneyTypes.RebornExpMonster)
				{
					num = Global.GetRoleParamsInt64FromDB(client, "10244");
				}
				else if (types == MoneyTypes.RebornExpSale)
				{
					num = Global.GetRoleParamsInt64FromDB(client, "10245");
				}
				if (roleParamsInt32FromDB != offsetDay)
				{
					MoneyTypes moneyType = (types == MoneyTypes.RebornExpMonster) ? MoneyTypes.RebornExpMonsterMax : MoneyTypes.RebornExpSaleMax;
					this.NotifySelfPropertyValue(client, (int)types, RebornManager.getInstance().GetRebornExpMaxValueLeft(client, types));
					this.NotifySelfPropertyValue(client, (int)moneyType, RebornManager.getInstance().GetRebornExpMaxValue(client, types));
				}
				result = num;
			}
			return result;
		}

		public bool ModifyRebornExpMaxAddValue(GameClient client, long addValue, string strFrom, MoneyTypes types, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (types != MoneyTypes.RebornExpMonster && types != MoneyTypes.RebornExpSale)
			{
				result = false;
			}
			else
			{
				long rebornExpMaxAddValue = this.GetRebornExpMaxAddValue(client, types);
				long num = rebornExpMaxAddValue + addValue;
				if (isGM && (num > 9223372036854775807L || num < -9223372036854775808L))
				{
					result = false;
				}
				else
				{
					long num2 = (long)((int)Global.Clamp(num, long.MinValue, long.MaxValue));
					if (rebornExpMaxAddValue != num2)
					{
						if (types == MoneyTypes.RebornExpMonster)
						{
							Global.SaveRoleParamsInt64ValueToDB(client, "10244", num2, writeToDB);
						}
						else if (types == MoneyTypes.RebornExpSale)
						{
							Global.SaveRoleParamsInt64ValueToDB(client, "10245", num2, writeToDB);
						}
					}
					long rebornExpMaxValueLeft = RebornManager.getInstance().GetRebornExpMaxValueLeft(client, types);
					client.ClientData.MoneyData[(int)types] = rebornExpMaxValueLeft;
					MoneyTypes moneyTypes = (types == MoneyTypes.RebornExpMonster) ? MoneyTypes.RebornExpMonsterMax : MoneyTypes.RebornExpSaleMax;
					long rebornExpMaxValue = RebornManager.getInstance().GetRebornExpMaxValue(client, types);
					if (client.ClientData.MoneyData[(int)moneyTypes] != rebornExpMaxValue)
					{
						client.ClientData.MoneyData[(int)moneyTypes] = rebornExpMaxValue;
						if (notifyClient)
						{
							this.NotifySelfPropertyValue(client, (int)moneyTypes, rebornExpMaxValue);
						}
					}
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, (int)types, rebornExpMaxValueLeft);
					}
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, types, addValue, num2, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetCompType(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10203");
		}

		public void SetCompType(GameClient client, int compType)
		{
			Global.SaveRoleParamsInt32ValueToDB(client, "10203", compType, true);
		}

		public int GetCompBattleJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10159");
		}

		public bool ModifyCompBattleJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetCompBattleJiFenValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[143] = (long)num3;
					Global.SaveRoleParamsInt32ValueToDB(client, "10159", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 143, (long)num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力战积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompBattleJiFen, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetCompMineJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10215");
		}

		public bool ModifyCompMineJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetCompMineJiFenValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[145] = (long)num3;
					Global.SaveRoleParamsInt32ValueToDB(client, "10215", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 145, (long)num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力矿洞积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompMineJiFen, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public int GetCompDonateValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10204");
		}

		public bool ModifyCompDonateValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)this.GetCompDonateValue(client);
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					client.ClientData.MoneyData[138] = (long)num3;
					Global.SaveRoleParamsInt32ValueToDB(client, "10204", num3, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 138, (long)num3);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "势力争霸贡献度", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.CompDonate, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public void ModifyShenJiPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetShenJiPointValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "10172", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiPoint, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiPoints, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public bool ModifyAlchemyElementValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool isGM = false)
		{
			bool result;
			if (client == null)
			{
				result = false;
			}
			else if (!GlobalNew.IsGongNengOpened(client, 90, false))
			{
				result = false;
			}
			else if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)client.ClientData.AlchemyInfo.BaseData.Element;
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					int num3 = (int)Global.Clamp(num2, -2147483648L, 2147483647L);
					client.ClientData.AlchemyInfo.BaseData.Element = num3;
					if (writeToDB)
					{
						AlchemyManager.getInstance().UpdateAlchemyDataDB(client);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炼金元素值", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num3, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.AlchemyElement, (long)addValue, (long)num3, strFrom);
					result = true;
				}
			}
			return result;
		}

		public void ModifyShenJiJiFenValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetShenJiJiFenValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "10173", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiJiFen, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹积分", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiJiFen, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public void ModifyShenJiJiFenAddValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetShenJiJiFenAddValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "10174", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShenJiJiFenAdd, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "神迹积分注入", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ShenJiJiFenAdd, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public int GetShenJiPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10172");
		}

		public int GetShenJiJiFenValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10173");
		}

		public int GetShenJiJiFenAddValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10174");
		}

		public void ModifyKingOfBattlePointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetKingOfBattlePointValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "10150", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.KingOfBattlePoint, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "王者争霸点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.KingOfBattlePoint, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public int GetKingOfBattlePointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "10150");
		}

		public bool ModifyMoBiValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.MoBi + addValue);
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.MoBi = (int)num;
					this.NotifySelfPropertyValue(client, 141, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10212", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔币", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MoBi, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyLuckStarValue(GameClient client, int addValue, string strFrom, bool isGM = false, DaiBiSySType SysType = DaiBiSySType.None)
		{
			if (SysType != DaiBiSySType.None && addValue < 0)
			{
				if (HuanLeDaiBiManager.GetInstance().UseReplaceMoney(client, Math.Abs(addValue), SysType, strFrom, true))
				{
					return true;
				}
			}
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.LuckStar + addValue);
				if (client == null || (num < 0L && !isGM))
				{
					result = false;
				}
				else
				{
					client.ClientData.LuckStar = (int)num;
					this.NotifySelfPropertyValue(client, 163, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10224", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "幸运之星", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.LuckStar, (long)addValue, num, strFrom);
					LogManager.WriteLog(0, string.Format("[ljl_幸运之星]{0}", string.Format("幸运之星 strFrom={0},val={1},type={2},id={3},name={4}", new object[]
					{
						strFrom,
						addValue,
						SysType,
						client.ClientData.RoleID,
						client.ClientData.RoleName
					})), null, true);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyTeamRongYaoValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.TeamRongYao + addValue);
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.TeamRongYao = (int)num;
					this.NotifySelfPropertyValue(client, 160, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10218", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "组队荣耀", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TeamRongYao, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyTeamPointValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.TeamPoint + addValue);
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.TeamPoint = (int)num;
					this.NotifySelfPropertyValue(client, 162, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10223", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "竞技点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.TeamPoint, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyMoBiValueOffline(int rid, string roleName, int zoneId, string userId, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = 0L;
				string roleParamsFromDBByRoleID = Global.GetRoleParamsFromDBByRoleID(rid, "10212", 0);
				if (!string.IsNullOrEmpty(roleParamsFromDBByRoleID) && long.TryParse(roleParamsFromDBByRoleID, out num))
				{
					num += (long)addValue;
				}
				else
				{
					num = (long)addValue;
				}
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					GameManager.DBCmdMgr.AddDBCmd(10100, string.Format("{0}:{1}:{2}", rid, "10212", num), null, GameManager.ServerId);
					Global.UpdateRoleParamByNameOffline(rid, "10212", num.ToString(), GameManager.ServerId);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魔币", strFrom, "系统", roleName, "修改", addValue, zoneId, userId, (int)num, GameManager.ServerId, null);
					EventLogManager.AddMoneyEvent(GameManager.ServerId, zoneId, userId, (long)rid, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MoBi, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyKuaFuLueDuoBuyNumAndDayID(GameClient client, int buyNum, int dayID, string strFrom)
		{
			bool result;
			if (client == null)
			{
				result = true;
			}
			else
			{
				long num = (long)(buyNum * 100000000 + dayID);
				client.ClientData.KuaFuLueDuoEnterNumBuyNum = buyNum;
				client.ClientData.KuaFuLueDuoEnterNumDayID = dayID;
				this.NotifySelfPropertyValue(client, 135, (long)buyNum);
				Global.SaveRoleParamsInt64ValueToDB(client, "10201", num, true);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "跨服掠夺进入次数", strFrom, "系统", client.ClientData.RoleName, "更新", 0, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
				EventLogManager.AddMoneyEvent(client, OpTypes.Trace, OpTags.Trace, MoneyTypes.KuaFuLueDuoEnterNumBuyNum, 0L, (long)buyNum, strFrom);
				result = true;
			}
			return result;
		}

		public bool ModifyKuaFuLueDuoEnterNum(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.KuaFuLueDuoEnterNum + addValue);
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.KuaFuLueDuoEnterNum = (int)num;
					this.NotifySelfPropertyValue(client, 134, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10200", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "跨服掠夺进入次数 ", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.KuaFuLueDuoEnterNum, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyJueXingValue(GameClient client, int addValue, string strFrom, bool isGM = false)
		{
			bool result;
			if (client == null || 0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)(client.ClientData.JueXingPoint + addValue);
				if (num < 0L && !isGM)
				{
					result = false;
				}
				else
				{
					client.ClientData.JueXingPoint = (int)num;
					this.NotifySelfPropertyValue(client, 132, num);
					Global.SaveRoleParamsInt64ValueToDB(client, "10198", num, true);
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "觉醒点数", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXing, (long)addValue, num, strFrom);
					result = true;
				}
			}
			return result;
		}

		public bool ModifyJueXingZhiChenValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long jueXingZhiChen = client.ClientData.JueXingZhiChen;
				long num = jueXingZhiChen + (long)addValue;
				if (isGM && (num > 2147483647L || num < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.JueXingZhiChen = (long)((int)Global.Clamp(num, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "觉醒之尘", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.JueXingZhiChen, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10194", (int)client.ClientData.JueXingZhiChen, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.JueXingZhiChen, (long)addValue, client.ClientData.JueXingZhiChen, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 133, client.ClientData.JueXingZhiChen);
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyYuanSuJueXingShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long yuanSuJueXingShi = client.ClientData.YuanSuJueXingShi;
				long num = yuanSuJueXingShi + (long)addValue;
				if (isGM && (num > 2147483647L || num < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.YuanSuJueXingShi = (long)((int)Global.Clamp(num, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "元素觉醒石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.YuanSuJueXingShi, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10214", (int)client.ClientData.YuanSuJueXingShi, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.YuanSuJueXingShi, (long)addValue, client.ClientData.YuanSuJueXingShi, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 144, client.ClientData.YuanSuJueXingShi);
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyHunJingValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long hunJing = client.ClientData.HunJing;
				long num = hunJing + (long)addValue;
				if (isGM && (num > 2147483647L || num < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.HunJing = (long)((int)Global.Clamp(num, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "魂晶", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.HunJing, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10208", (int)client.ClientData.HunJing, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.HunJing, (long)addValue, client.ClientData.HunJing, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 139, client.ClientData.HunJing);
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyMountPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long mountPoint = client.ClientData.MountPoint;
				long num = mountPoint + (long)addValue;
				if (isGM && (num > 2147483647L || num < -2147483648L))
				{
					result = false;
				}
				else
				{
					client.ClientData.MountPoint = (long)((int)Global.Clamp(num, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "坐骑积分", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.MountPoint, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10209", (int)client.ClientData.MountPoint, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.MountPoint, (long)addValue, client.ClientData.MountPoint, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 140, client.ClientData.MountPoint);
					}
					result = true;
				}
			}
			return result;
		}

		public void ModifyZhengBaPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (client != null)
			{
				if (0 != addValue)
				{
					long num = (long)this.GetZhengBaPointValue(client) + (long)addValue;
					num = Math.Min(num, 2147483647L);
					int num2 = (int)num;
					Global.SaveRoleParamsInt32ValueToDB(client, "ZhengBaPoint", num2, writeToDB);
					if (notifyClient)
					{
						this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ZhengBaPoint, num2);
					}
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "争霸点", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num2, client.ServerId, null);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.ZhengBaPoint, (long)addValue, (long)num2, strFrom);
				}
			}
		}

		public int GetZhengBaPointValue(GameClient client)
		{
			return Global.GetRoleParamsInt32FromDB(client, "ZhengBaPoint");
		}

		public void SaveWanMoTaPassLayerValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "WanMoTaCurrLayerOrder", nValue, true, "2020-12-12 12:12:12");
		}

		public int GetWanMoTaPassLayerValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "WanMoTaCurrLayerOrder", "2020-12-12 12:12:12");
		}

		public void SaveShengWangValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWang", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetShengWangValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShengWang", "2020-12-12 12:12:12");
		}

		public void ModifyShengWangLevelValue(GameClient client, int addValue, string strFrom, bool writeToDB = false, bool notifyClient = true)
		{
			if (0 != addValue)
			{
				int num = this.GetShengWangLevelValue(client) + addValue;
				this.SaveShengWangLevelValue(client, num, writeToDB);
				GameManager.logDBCmdMgr.AddDBLogInfo(-1, "声望等级", strFrom, "系统", client.ClientData.RoleName, "修改", addValue, client.ClientData.ZoneID, client.strUserID, num, client.ServerId, null);
				ChengJiuManager.OnRoleJunXianChengJiu(client);
				if (notifyClient)
				{
					this.NotifySelfParamsValueChange(client, RoleCommonUseIntParamsIndexs.ShengWangLevel, num);
				}
				GlobalEventSource.getInstance().fireEvent(SevenDayGoalEvPool.Alloc(client, ESevenDayGoalFuncType.JunXianLevel));
			}
		}

		public void SaveShengWangLevelValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "ShengWangLevel", nValue, writeToDB, "2020-12-12 12:12:12");
			if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriMilitaryRank) || client._IconStateMgr.CheckSpecialActivity(client) || client._IconStateMgr.CheckEverydayActivity(client))
			{
				client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
				client._IconStateMgr.SendIconStateToClient(client);
			}
		}

		public int GetShengWangLevelValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "ShengWangLevel", "2020-12-12 12:12:12");
		}

		public void NotifySelfParamsValueChange(GameClient client, RoleCommonUseIntParamsIndexs index, int value)
		{
			string strCmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, (int)index, value);
			this.SendToClient(client, strCmd, 427);
		}

		public void ModifyLiXianBaiTanTicksValue(GameClient client, int addValue, bool writeToDB = false)
		{
			if (0 != addValue)
			{
				int nValue = this.GetLiXianBaiTanTicksValue(client) + addValue;
				this.SaveLiXianBaiTanTicksValue(client, nValue, writeToDB);
			}
		}

		public void SaveLiXianBaiTanTicksValue(GameClient client, int nValue, bool writeToDB = false)
		{
			Global.SaveRoleParamsInt32ValueWithTimeStampToDB(client, "LiXianBaiTanTicks", nValue, writeToDB, "2020-12-12 12:12:12");
		}

		public int GetLiXianBaiTanTicksValue(GameClient client)
		{
			return Global.GetRoleParamsInt32ValueWithTimeStampFromDB(client, "LiXianBaiTanTicks", "2020-12-12 12:12:12");
		}

		public void SendGameEffect(GameClient client, string effectName, int lifeTicks, GameEffectAlignModes alignMode = GameEffectAlignModes.None, string mp3Name = "")
		{
			string data = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				effectName,
				lifeTicks,
				(int)alignMode,
				mp3Name
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 437);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void BroadCastGameEffect(int mapCode, int copyMapID, string effectName, int lifeTicks, GameEffectAlignModes alignMode = GameEffectAlignModes.None, string mp3Name = "")
		{
			string strCmd = string.Format("{0}:{1}:{2}:{3}", new object[]
			{
				effectName,
				lifeTicks,
				(int)alignMode,
				mp3Name
			});
			List<object> list = this.GetMapClients(mapCode);
			if (null != list)
			{
				list = Global.ConvertObjsList(mapCode, copyMapID, list, false);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, strCmd, 437);
			}
		}

		public void BroadcastJieriChengHao(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.JieriChengHao);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 477);
			}
		}

		public void NotifyZaJinDanKAwardDailyData(GameClient client)
		{
			int zaJinDanJifen = Global.GetZaJinDanJifen(client);
			int zaJinDanJiFenBits = Global.GetZaJinDanJiFenBits(client);
			string data = string.Format("{0}:{1}", zaJinDanJifen, zaJinDanJiFenBits);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 499);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySpriteMeditate(SocketListener sl, TCPOutPacketPool pool, GameClient client, int meditate)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			if (null != all9Clients)
			{
				string strCmd = string.Format("{0}:{1}", client.ClientData.RoleID, meditate);
				this.SendToClients(sl, pool, null, all9Clients, strCmd, 600);
			}
		}

		public void NotifyGetMeditateAward(GameClient client)
		{
			int num = Global.GetRoleParamsInt32FromDB(client, "MeditateTime") / 1000;
			int num2 = 0;
			int num3 = num + num2;
			if (num3 >= Data.NotifyLiXianAwardMin * 60 || num3 * 1000 >= 43200000)
			{
				this.NotifyImportantMsgWithGoods(client, MsgWithGoodsType.NeedQueryMeditateInfo, ShowGameInfoTypes.OnlyChatBox, null, "", null);
			}
		}

		public void NotifyMeditateTime(GameClient client)
		{
			int num = Global.GetRoleParamsInt32FromDB(client, "MeditateTime") / 1000;
			int num2 = 0;
			string data = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, num, num2);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 550);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifyPlayBossAnimation(GameClient client, int monsterID, int mapCode, int toX, int toY, int effectX, int effectY)
		{
			long num = TimeUtil.NOW();
			int bossAnimationCheckCode = Global.GetBossAnimationCheckCode(monsterID, mapCode, toX, toY, effectX, effectY, num);
			string data = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
			{
				client.ClientData.RoleID,
				monsterID,
				mapCode,
				toX,
				toY,
				effectX,
				effectY,
				num,
				bossAnimationCheckCode
			});
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 639);
			if (!Global._TCPManager.MySocketListener.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySpriteExtensionPropsHited(SocketListener sl, TCPOutPacketPool pool, IObject attacker, int enemy, int enemyX, int enemyY, int extensionPropID)
		{
			List<object> all9Clients = Global.GetAll9Clients(attacker);
			if (null != all9Clients)
			{
				this.SendToClients(sl, pool, null, all9Clients, DataHelper.ObjectToBytes<SpriteExtensionPropsHitedData>(new SpriteExtensionPropsHitedData
				{
					roleId = attacker.GetObjectID(),
					enemy = enemy,
					enemyX = enemyX,
					enemyY = enemyY,
					ExtensionPropID = extensionPropID
				}), 644);
			}
		}

		public IEnumerable<GameClient> GetAllClients(bool includeKuaFu = true)
		{
			int index = 0;
			GameClient client = null;
			while ((client = this.GetNextClient(ref index, false)) != null)
			{
				if (client.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !client.ClientSocket.IsKuaFuLogin)
					{
						yield return client;
					}
				}
			}
			yield break;
		}

		public void BroadcastServerCmd(int cmdId, string data, bool includeKuaFu = false)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !nextClient.ClientSocket.IsKuaFuLogin)
					{
						nextClient.sendCmd(cmdId, data, false);
					}
				}
			}
		}

		public void BroadcastServerCmd<T>(int cmdId, T data, bool includeKuaFu = false)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !nextClient.ClientSocket.IsKuaFuLogin)
					{
						nextClient.sendCmd<T>(cmdId, data, false);
					}
				}
			}
		}

		public void BroadcastOthersCmdData<T>(GameClient client, int cmdId, T data, bool includeKuaFu = true)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			foreach (object obj in all9Clients)
			{
				GameClient gameClient = obj as GameClient;
				if (gameClient != null && gameClient.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !gameClient.ClientSocket.IsKuaFuLogin)
					{
						gameClient.sendCmd<T>(cmdId, data, false);
					}
				}
			}
		}

		public void BroadcastOthersCmdData(GameClient client, int cmdId, string data, bool includeKuaFu = true)
		{
			List<object> all9Clients = Global.GetAll9Clients(client);
			foreach (object obj in all9Clients)
			{
				GameClient gameClient = obj as GameClient;
				if (gameClient != null && gameClient.ClientData.ClosingClientStep == 0)
				{
					if (includeKuaFu || !gameClient.ClientSocket.IsKuaFuLogin)
					{
						gameClient.sendCmd(cmdId, data, false);
					}
				}
			}
		}

		public void BroadSpecialHintText(int mapCode, int copyMapID, string text)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				List<object> list = new List<object>();
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CopyMapID == copyMapID)
						{
							list.Add(gameClient);
						}
					}
				}
				text = text.Replace(":", " ");
				string strCmd = string.Format("{0}", text);
				this.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, list, strCmd, 666);
			}
		}

		public void BroadSpecialMapAIEvent(int mapCode, int copyMapID, int guangMuID, int show)
		{
			string cmdData = string.Format("{0}:{1}", guangMuID, show);
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CopyMapID == copyMapID)
						{
							gameClient.sendCmd(667, cmdData, false);
						}
					}
				}
			}
		}

		public void BroadSpecialMapMessage(int cmdID, string strcmd, int mapCode, int copyMapID)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CopyMapID == copyMapID)
						{
							gameClient.sendCmd(cmdID, strcmd, false);
						}
					}
				}
			}
		}

		public void BroadSpecialMapMessage(TCPOutPacket tcpOutPacket, int mapCode, int copyMapID, bool pushBack = true)
		{
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count > 0)
			{
				for (int i = 0; i < mapClients.Count; i++)
				{
					GameClient gameClient = mapClients[i] as GameClient;
					if (gameClient != null)
					{
						if (gameClient.ClientData.CopyMapID == copyMapID)
						{
							gameClient.sendCmd(tcpOutPacket, false);
						}
					}
				}
				if (pushBack)
				{
					Global.PushBackTcpOutPacket(tcpOutPacket);
				}
			}
		}

		public void BroadSpecialCopyMapMessageStr(int cmdID, string strcmd, CopyMap copyMap, bool insertRoleID = false)
		{
			List<GameClient> clientsList = copyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null)
					{
						if (gameClient.ClientData.CopyMapID == copyMap.CopyMapID)
						{
							if (insertRoleID)
							{
								gameClient.sendCmd(cmdID, strcmd.Insert(0, string.Format("{0}:", gameClient.ClientData.RoleID)), false);
							}
							else
							{
								gameClient.sendCmd(cmdID, strcmd, false);
							}
						}
					}
				}
			}
		}

		public void BroadSpecialCopyMapMessage<T>(int cmdID, T data, CopyMap copyMap)
		{
			List<GameClient> clientsList = copyMap.GetClientsList();
			if (clientsList != null && clientsList.Count > 0)
			{
				for (int i = 0; i < clientsList.Count; i++)
				{
					GameClient gameClient = clientsList[i];
					if (gameClient != null && gameClient.ClientData.CopyMapID == copyMap.CopyMapID)
					{
						gameClient.sendCmd<T>(cmdID, data, false);
					}
				}
			}
		}

		public void BroadSpecialCopyMapMessage(int cmdID, string strcmd, List<GameClient> objsList, bool insertRoleID = false)
		{
			if (objsList != null && objsList.Count > 0)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					GameClient gameClient = objsList[i];
					if (gameClient != null)
					{
						if (insertRoleID)
						{
							gameClient.sendCmd(cmdID, strcmd.Insert(0, string.Format("{0}:", gameClient.ClientData.RoleID)), false);
						}
						else
						{
							gameClient.sendCmd(cmdID, strcmd, false);
						}
					}
				}
			}
		}

		public void BroadZhanDuiMessage<T>(int cmdID, T data, int zhanDuiID)
		{
			if (zhanDuiID != 0)
			{
				foreach (GameClient gameClient in this.GetAllClients(false))
				{
					if (gameClient != null && gameClient.ClientData.ZhanDuiID == zhanDuiID)
					{
						gameClient.sendCmd<T>(cmdID, data, false);
					}
				}
			}
		}

		public void BroadZhanDuiMessage(int cmdID, string strcmd, int zhanDuiID)
		{
			if (zhanDuiID != 0)
			{
				foreach (GameClient gameClient in this.GetAllClients(false))
				{
					if (gameClient != null && gameClient.ClientData.ZhanDuiID == zhanDuiID)
					{
						gameClient.sendCmd(cmdID, strcmd, false);
					}
				}
			}
		}

		public void BroadSpecialCopyMapHintMsg(CopyMap copymap, string msg)
		{
			try
			{
				msg = msg.Replace(":", "``");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					4,
					1,
					msg,
					0
				});
				this.BroadSpecialCopyMapMessageStr(194, strcmd, copymap, false);
			}
			catch
			{
			}
		}

		public void BroadSpecialCopyMapMsg(CopyMap copymap, string msg, ShowGameInfoTypes showGameInfoType = ShowGameInfoTypes.OnlySysHint, GameInfoTypeIndexes infoType = GameInfoTypeIndexes.Hot, int error = 0)
		{
			try
			{
				msg = msg.Replace(":", "``");
				string strcmd = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					(int)showGameInfoType,
					(int)infoType,
					msg,
					error
				});
				this.BroadSpecialCopyMapMessageStr(194, strcmd, copymap, false);
			}
			catch
			{
			}
		}

		public void NotifyChangMap2NormalMap(GameClient client)
		{
			if (Global.CanChangeMap(client, client.ClientData.LastMapCode, client.ClientData.LastPosX, client.ClientData.LastPosY, true))
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.LastMapCode, client.ClientData.LastPosX, client.ClientData.LastPosY, -1, 0);
			}
			else
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, GameManager.MainMapCode, -1, -1, -1, 0);
			}
		}

		private void DoSpriteLifeMagic(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.LastLifeMagicTick;
			if (num2 >= 10000L)
			{
				c.LastLifeMagicTick = num;
				RoleRelifeLog roleRelifeLog = new RoleRelifeLog(c.ClientData.RoleID, c.ClientData.RoleName, c.ClientData.MapCode, "自然恢复补血补蓝");
				if (c.ClientData.CurrentLifeV > 0)
				{
					bool flag = false;
					if (c.ClientData.CurrentLifeV < c.ClientData.LifeV)
					{
						flag = true;
						roleRelifeLog.hpModify = true;
						roleRelifeLog.oldHp = c.ClientData.CurrentLifeV;
						double num3 = RoleAlgorithm.GetLifeRecoverValPercentV(c);
						double num4 = num3 * (double)c.ClientData.LifeV;
						num4 *= 1.0 + RoleAlgorithm.GetLifeRecoverAddPercentV(c) + DBRoleBufferManager.ProcessHuZhaoRecoverPercent(c) + RoleAlgorithm.GetLifeRecoverAddPercentOnlySandR(c);
						num4 += (double)c.ClientData.CurrentLifeV;
						c.ClientData.CurrentLifeV = (int)Global.GMin((double)c.ClientData.LifeV, num4);
						roleRelifeLog.newHp = c.ClientData.CurrentLifeV;
					}
					if (c.ClientData.CurrentMagicV < c.ClientData.MagicV)
					{
						flag = true;
						roleRelifeLog.mpModify = true;
						roleRelifeLog.oldMp = c.ClientData.CurrentMagicV;
						double num3 = RoleAlgorithm.GetMagicRecoverValPercentV(c);
						double num5 = num3 * (double)c.ClientData.MagicV;
						num5 *= 1.0 + RoleAlgorithm.GetMagicRecoverAddPercentV(c) + RoleAlgorithm.GetMagicRecoverAddPercentOnlySandR(c);
						num5 += (double)c.ClientData.CurrentMagicV;
						c.ClientData.CurrentMagicV = (int)Global.GMin((double)c.ClientData.MagicV, num5);
						roleRelifeLog.newMp = c.ClientData.CurrentMagicV;
					}
					if (flag)
					{
						List<object> all9Clients = Global.GetAll9Clients(c);
						GameManager.ClientMgr.NotifyOthersRelife(sl, pool, c, c.ClientData.MapCode, c.ClientData.CopyMapID, c.ClientData.RoleID, c.ClientData.PosX, c.ClientData.PosY, c.ClientData.RoleDirection, (double)c.ClientData.CurrentLifeV, (double)c.ClientData.CurrentMagicV, 120, all9Clients, 0);
					}
					SingletonTemplate<MonsterAttackerLogManager>.Instance().AddRoleRelifeLog(roleRelifeLog);
				}
			}
		}

		private void DoSpriteHeart(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			c.ClientData.DayOnlineSecond = c.ClientData.BakDayOnlineSecond + (int)((TimeUtil.NOW() - c.ClientData.DayOnlineRecSecond) / 1000L);
			int num = Global.GetRoleParamsInt32FromDB(c, "10167");
			int num2 = (num > 10) ? 10 : 0;
			num %= 10;
			if (num == 1)
			{
				if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(c.ClientData.RoleID, 0))
				{
					Global.SaveRoleParamsInt32ValueToDB(c, "10167", 2 + num2, true);
				}
			}
			else if (num == 2 && c.ClientData.DayOnlineSecond >= 2400)
			{
				if (WebOldPlayerManager.getInstance().ChouJiangAddCheck(c.ClientData.RoleID, 3))
				{
					Global.SaveRoleParamsInt32ValueToDB(c, "10167", 3 + num2, true);
				}
			}
		}

		private void DoSpriteDBHeart(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.ClientData.LastDBHeartTicks;
			if (num2 >= 10000L)
			{
				long num3 = 0L;
				Math.DivRem(num2, 1000L, out num3);
				num2 -= num3;
				num -= num3;
				c.ClientData.LastDBHeartTicks = num;
				this.UpdateRoleOnlineTimes(c, num2);
				c._IconStateMgr.CheckFreeZhuanPanChouState(c);
				c._IconStateMgr.SendIconStateToClient(c);
			}
		}

		private void DoSpriteAutoFight(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long ticks = TimeUtil.NOW();
			c.AutoGetThingsOnAutoFight(ticks);
		}

		private void DoSpriteSitExp(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.ClientData.LastSiteExpTicks;
			if (num2 >= 60000L)
			{
				c.ClientData.LastSiteExpTicks = num;
				bool flag = false;
				if (c.ClientData.MapCode == GameManager.MainMapCode)
				{
					GameMap gameMap = null;
					if (GameManager.MapMgr.DictMaps.TryGetValue(c.ClientData.MapCode, out gameMap))
					{
						flag = gameMap.InSafeRegionList(c.CurrentGrid);
					}
				}
				if (flag)
				{
					double num3 = 0.0;
					long num4 = DBRoleBufferManager.ProcessErGuoTouGiveExperience(c, num2, out num3);
					if (num4 > 0L)
					{
						RoleSitExpItem roleSitExpItem = null;
						if (c.ClientData.Level < Data.RoleSitExpList.Length)
						{
							roleSitExpItem = Data.RoleSitExpList[c.ClientData.Level];
						}
						if (null != roleSitExpItem)
						{
							int num5 = roleSitExpItem.Experience;
							double num6 = 1.0;
							if (SpecailTimeManager.JugeIsDoulbeKaoHuo())
							{
								num6 += 1.0;
							}
							num6 += Global.ProcessTeamZhuFuExperience(c);
							num6 += num3;
							num5 = (int)((double)num5 * num6);
							GameManager.ClientMgr.ProcessRoleExperience(c, (long)num5, true, false, true, "none");
							GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, StringUtil.substitute(GLang.GetLang(89, new object[0]), new object[]
							{
								num5,
								num4 / 60L,
								num4 % 60L
							}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
						}
					}
				}
			}
		}

		private void DoSpriteSubPKPoint(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			long num = TimeUtil.NOW();
			if (c.ClientData.LastSiteSubPKPointTicks == 0L)
			{
				c.ClientData.LastSiteSubPKPointTicks = num;
			}
			else
			{
				long num2 = num - c.ClientData.LastSiteSubPKPointTicks;
				if (num2 >= 60000L)
				{
					c.ClientData.LastSiteSubPKPointTicks = num;
					if (c.ClientData.PKPoint > 0)
					{
						int pkpoint = c.ClientData.PKPoint;
						c.ClientData.PKPoint = Global.GMax(c.ClientData.PKPoint - 10, 0);
						c.ClientData.TmpPKPoint += 10;
						if (pkpoint != c.ClientData.PKPoint)
						{
							if (c.ClientData.TmpPKPoint >= 60)
							{
								this.SetRolePKValuePoint(sl, pool, c, c.ClientData.PKValue, c.ClientData.PKPoint, true);
								c.ClientData.TmpPKPoint = 0;
							}
							else
							{
								this.SetRolePKValuePoint(sl, pool, c, c.ClientData.PKValue, c.ClientData.PKPoint, false);
							}
						}
					}
				}
			}
		}

		private void DoSpriteBuffers(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			DBRoleBufferManager.ProcessAutoGiveExperience(c);
			DBRoleBufferManager.RemoveUpLifeLimitStatus(c);
			DBRoleBufferManager.RemoveAttackBuffer(c);
			DBRoleBufferManager.RemoveDefenseBuffer(c);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddDefense);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddMDefense);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddMAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.TimeAddDSAttack);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.PKKingBuffer);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeShiDuNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddLifeNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddDefenseNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.DSTimeAddMDefenseNoShow);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI1);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI2);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.MU_LUOLANCHENGZHAN_QIZHI3);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPStrength);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPIntelligsence);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPDexterity);
			DBRoleBufferManager.RefreshTimePropBuffer(c, BufferItemTypes.ADDTEMPConstitution);
			DBRoleBufferManager.ProcessTimeAddLifeMagic(c);
			DBRoleBufferManager.ProcessTimeAddLifeNoShow(c);
			DBRoleBufferManager.ProcessTimeAddMagicNoShow(c);
			DBRoleBufferManager.ProcessDSTimeAddLifeNoShow(c);
			DBRoleBufferManager.ProcessDSTimeSubLifeNoShow(c);
			DBRoleBufferManager.ProcessAllTimeSubLifeNoShow(c);
			AdvanceBufferPropsMgr.DoSpriteBuffers(c);
			if (GlobalNew.IsGongNengOpened(c, 79, false))
			{
				TarotManager.getInstance().RemoveTarotKingData(c);
			}
			long num = TimeUtil.NOW();
			long num2 = num - c.ClientData.LastProcessBufferTicks;
			if (num2 >= 60000L)
			{
				c.ClientData.LastProcessBufferTicks = num;
				Global.RefreshJieriChengHao(c);
			}
		}

		private void DoSpriteMapLimitTimes(SocketListener sl, TCPOutPacketPool pool, GameClient c)
		{
			DateTime dateTime = TimeUtil.NowDateTime();
			long num = dateTime.Ticks / 10000L;
			long num2 = num - c.ClientData.LastProcessMapLimitTimesTicks;
			if (c.ClientData.EventLastMapCode != c.ClientData.MapCode)
			{
				GlobalEventSource.getInstance().fireEvent(new OnClientMapChangedEventObject(c, c.ClientData.EventLastMapCode, c.ClientData.MapCode));
				c.ClientData.EventLastMapCode = c.ClientData.MapCode;
			}
			if (num2 >= 60000L)
			{
				int elapsedSecs = (int)((num - c.ClientData.LastProcessMapLimitTimesTicks) / 1000L);
				c.ClientData.LastProcessMapLimitTimesTicks = num;
				if (!Global.CanMapInLimitTimes(c.ClientData.MapCode, dateTime))
				{
					GameManager.ClientMgr.NotifyImportantMsg(sl, pool, c, StringUtil.substitute(GLang.GetLang(90, new object[0]), new object[]
					{
						Global.GetMapName(c.ClientData.MapCode)
					}), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, c, GameManager.MainMapCode, -1, -1, -1, 0);
				}
				Global.ProcessDayLimitSecsByClient(c, elapsedSecs);
			}
		}

		private static void DoSpriteMapTimeLimit(GameClient client)
		{
			long num = TimeUtil.NOW();
			long num2 = num - client.ClientData.LastMapLimitUpdateTicks;
			if (num2 >= 3000L)
			{
				Global.ProcessMingJieMapTimeLimit(client, num2);
				Global.ProcessGuMuMapTimeLimit(client, num2);
				client.ClientData.LastMapLimitUpdateTicks = num;
			}
		}

		private static void DoSpriteHintToUpdateClient(GameClient client)
		{
			long num = TimeUtil.NOW();
			long num2 = num - client.ClientData.LastHintToUpdateClientTicks;
			if (num2 >= 60000L)
			{
				client.ClientData.LastHintToUpdateClientTicks = num;
				int gameConfigItemInt = GameManager.GameConfigMgr.GetGameConfigItemInt("hint-appver", 0);
				if (client.MainExeVer > 0 && client.MainExeVer < gameConfigItemInt)
				{
					string msgID = "1";
					int playMinutes = 1;
					int toPlayNum = 1;
					string lang = GLang.GetLang(91, new object[0]);
					BulletinMsgData bulletinMsgData = new BulletinMsgData
					{
						MsgID = msgID,
						PlayMinutes = playMinutes,
						ToPlayNum = toPlayNum,
						BulletinText = lang,
						BulletinTicks = TimeUtil.NOW(),
						MsgType = 0
					};
					GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
				}
				else if (client.ResVer > 0)
				{
					int gameConfigItemInt2 = GameManager.GameConfigMgr.GetGameConfigItemInt("hint-resver", 0);
					if (client.ResVer < gameConfigItemInt2)
					{
						string msgID = "1";
						int playMinutes = 1;
						int toPlayNum = 1;
						string lang = GLang.GetLang(92, new object[0]);
						BulletinMsgData bulletinMsgData = new BulletinMsgData
						{
							MsgID = msgID,
							PlayMinutes = playMinutes,
							ToPlayNum = toPlayNum,
							BulletinText = lang,
							BulletinTicks = TimeUtil.NOW(),
							MsgType = 0
						};
						GameManager.ClientMgr.NotifyBulletinMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, bulletinMsgData);
					}
				}
			}
		}

		private static void DoSpriteGoodsTimeLimit(GameClient client)
		{
			bool flag = false;
			bool flag2 = false;
			long num = TimeUtil.NOW();
			long num2 = num - client.ClientData.LastGoodsLimitUpdateTicks;
			long num3 = num - client.ClientData.LastFashionLimitUpdateTicks;
			List<GoodsData> list = null;
			if (num3 >= 3000L)
			{
				list = Global.GetFashionTimeExpired(client);
				flag = true;
			}
			if (num2 >= 30000L)
			{
				List<GoodsData> goodsTimeExpired = Global.GetGoodsTimeExpired(client);
				if (goodsTimeExpired != null)
				{
					if (list == null)
					{
						list = goodsTimeExpired;
					}
					else
					{
						list.AddRange(Global.GetGoodsTimeExpired(client));
					}
				}
				flag2 = true;
			}
			if (list != null && list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					GoodsData goodsData = list[i];
					if (Global.DestroyGoods(client, goodsData))
					{
						Global.SendMail(client, GLang.GetLang(93, new object[0]), string.Format(GLang.GetLang(94, new object[0]), Global.GetGoodsNameByID(goodsData.GoodsID)));
					}
				}
			}
			if (flag2)
			{
				client.ClientData.LastGoodsLimitUpdateTicks = num;
			}
			if (flag)
			{
				client.ClientData.LastFashionLimitUpdateTicks = num;
			}
		}

		public static void DoSpriteMapGridMove(GameClient client, int slot = 0)
		{
			long num = TimeUtil.NOW();
			lock (client.Current9GridMutex)
			{
				long num2 = client.LastRefresh9GridObjectsTicks[slot];
				if (num >= num2)
				{
					client.LastRefresh9GridObjectsTicks[slot] = num + client.CurrentSlotTicks;
					Global.GameClientMoveGrid(client);
				}
			}
		}

		private void DoSpriteMeditateTime(GameClient c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.ClientData.MeditateTicks;
			if (num2 >= 10000L)
			{
				SceneUIClasses mapSceneType = Global.GetMapSceneType(c.ClientData.MapCode);
				if (54 != mapSceneType)
				{
					if (c.ClientData.StartMeditate <= 0)
					{
						if (c.ClientData.LastMovePosTicks == 0L || c.ClientData.Last10sPosX != c.ClientData.PosX || c.ClientData.Last10sPosY != c.ClientData.PosY)
						{
							c.ClientData.Last10sPosX = c.ClientData.PosX;
							c.ClientData.Last10sPosY = c.ClientData.PosY;
							c.ClientData.LastMovePosTicks = num;
						}
						else if (GlobalNew.IsGongNengOpened(c, 14, false))
						{
							if (num - c.ClientData.LastMovePosTicks > 120000L)
							{
								Global.StartMeditate(c);
								num2 = 60000L;
							}
						}
					}
					if (num2 >= 60000L)
					{
						c.ClientData.MeditateTicks = num;
						if (c.ClientData.StartMeditate > 0)
						{
							Global.UpdateMeditateTime(c, num);
							int meditateTime = c.ClientData.MeditateTime;
							if (1 == c.ClientData.StartMeditate)
							{
								if (c.ClientData.GiveMeditateGoodsInterval == 0)
								{
									c.ClientData.GiveMeditateGoodsInterval = Global.GetMingXiangGoodsInterval(c);
								}
								if (GoodsUtil.GetMeditateBagGoodsCnt(c) < Data.OfflineRW_ItemLimit && (long)meditateTime >= c.ClientData.GiveMeditateAwardOffsetTicks + (long)c.ClientData.GiveMeditateGoodsInterval)
								{
									int num3 = (int)(((long)meditateTime - c.ClientData.GiveMeditateAwardOffsetTicks) / (long)c.ClientData.GiveMeditateGoodsInterval);
									for (int i = 0; i < num3; i++)
									{
										if (GoodsUtil.GetMeditateBagGoodsCnt(c) >= Data.OfflineRW_ItemLimit)
										{
											break;
										}
										GoodsUtil.GiveOneMeditateGood(c);
									}
									if (num3 > 1)
									{
										LogManager.WriteLog(1000, string.Format("角色冥想背包本次添加物品个数为  {2},角色ID = {0} ，角色roleid = {1}, 个数异常。", c.strUserID, c.ClientData.RoleID, num3), null, false);
									}
									c.ClientData.GiveMeditateAwardOffsetTicks = GoodsUtil.GetLastGiveMeditateTime(c);
									c.ClientData.GiveMeditateGoodsInterval = Global.GetMingXiangGoodsInterval(c);
								}
							}
							GameManager.ClientMgr.NotifyMeditateTime(c);
							if (c._IconStateMgr.CheckShenYouAwardIcon(c))
							{
								c._IconStateMgr.SendIconStateToClient(c);
							}
						}
					}
				}
			}
		}

		private void DoSpriteDeadTime(GameClient c)
		{
			long num = TimeUtil.NOW();
			long num2 = num - c.ClientData.LastProcessDeadTicks;
			if (c.ClientData.CurrentLifeV <= 0 && num2 >= 3000L)
			{
				c.ClientData.LastProcessDeadTicks = num;
				this.ProcessSpriteDead(c, num);
			}
		}

		private void ProcessSpriteDead(GameClient client, long nowTicks)
		{
			int num = -1;
			int num2 = -1;
			if (2 == Global.GetRoleReliveType(client) || 3 == Global.GetRoleReliveType(client))
			{
				long num3 = nowTicks - client.ClientData.LastRoleDeadTicks;
				if (num3 / 1000L < (long)(Global.GetRoleReliveWaitingSecs(client) + 3000))
				{
					return;
				}
			}
			else if (4 == Global.GetRoleReliveType(client))
			{
				long num3 = TimeUtil.NOW() - client.ClientData.LastRoleDeadTicks;
				if (num3 / 1000L < (long)(Global.GetRoleReliveWaitingSecs(client) + 3000))
				{
					return;
				}
				num = -1;
				num2 = -1;
			}
			else if (0 == Global.GetRoleReliveType(client))
			{
				if (nowTicks - client.ClientData.LastRoleDeadTicks < 35000L)
				{
					return;
				}
			}
			else
			{
				if (1 != Global.GetRoleReliveType(client))
				{
					return;
				}
				if (nowTicks - client.ClientData.LastRoleDeadTicks < 5000L)
				{
					return;
				}
			}
			if (Global.IsHuangChengMapCode(client.ClientData.MapCode) || Global.IsHuangGongMapCode(client.ClientData.MapCode))
			{
				num = -1;
				num2 = -1;
			}
			if (Global.IsBattleMap(client))
			{
				int num4 = GameManager.BattleMgr.BattleMapCode;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					int defaultBirthPosX = gameMap.DefaultBirthPosX;
					int defaultBirthPosY = gameMap.DefaultBirthPosY;
					int birthRadius = gameMap.BirthRadius;
					Global.GetBattleMapPos(client, ref defaultBirthPosX, ref defaultBirthPosY, ref birthRadius);
					Point point = Global.GetMapPoint(ObjectTypes.OT_CLIENT, num4, defaultBirthPosX, defaultBirthPosY, birthRadius);
					num = (int)point.X;
					num2 = (int)point.Y;
					Global.ClientRealive(client, num, num2, client.ClientData.RoleDirection);
				}
			}
			else if (Global.IsLingDiZhanMapCode(client))
			{
				int num4 = client.ClientData.MapCode;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
				{
					client.ClientData.CurrentLifeV = client.ClientData.LifeV;
					client.ClientData.CurrentMagicV = client.ClientData.MagicV;
					Point point = Global.GetRandomPoint(ObjectTypes.OT_CLIENT, num4);
					num = (int)point.X;
					num2 = (int)point.Y;
					Global.ClientRealive(client, num, num2, client.ClientData.RoleDirection);
				}
			}
			else
			{
				if (GameManager.ArenaBattleMgr.IsInArenaBattle(client))
				{
					num = -1;
					num2 = -1;
				}
				if (num == -1 || num2 == -1)
				{
					int num4 = Global.GetMapRealiveInfoByCode(client.ClientData.MapCode);
					if (num4 <= -1)
					{
						num4 = GameManager.MainMapCode;
					}
					else if (num4 == 0 || GameManager.ArenaBattleMgr.IsInArenaBattle(client))
					{
						num4 = GameManager.MainMapCode;
					}
					else if (num4 == 1)
					{
						num4 = client.ClientData.MapCode;
					}
					if (num4 >= 0)
					{
						GameMap gameMap = null;
						if (GameManager.MapMgr.DictMaps.TryGetValue(num4, out gameMap))
						{
							int defaultBirthPosX = GameManager.MapMgr.DictMaps[num4].DefaultBirthPosX;
							int defaultBirthPosY = GameManager.MapMgr.DictMaps[num4].DefaultBirthPosY;
							int birthRadius = GameManager.MapMgr.DictMaps[num4].BirthRadius;
							Point point = Global.GetMapPoint(ObjectTypes.OT_CLIENT, num4, defaultBirthPosX, defaultBirthPosY, birthRadius);
							num = (int)point.X;
							num2 = (int)point.Y;
							client.ClientData.CurrentLifeV = client.ClientData.LifeV;
							client.ClientData.CurrentMagicV = client.ClientData.MagicV;
							client.ClientData.MoveAndActionNum = 0;
							GameManager.ClientMgr.NotifyTeamRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client.ClientData.RoleID, num, num2, client.ClientData.RoleDirection);
							if (num4 != client.ClientData.MapCode)
							{
								GameManager.ClientMgr.NotifyMySelfRealive(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, client.ClientData.RoleID, client.ClientData.PosX, client.ClientData.PosY, client.ClientData.RoleDirection);
								GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num4, num, num2, -1, 1);
							}
							else
							{
								Global.ClientRealive(client, num, num2, client.ClientData.RoleDirection);
							}
						}
					}
				}
			}
		}

		public void DoSpriteWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteHeart(sl, pool, client);
			this.DoSpriteAutoFight(sl, pool, client);
			this.DoSpriteSitExp(sl, pool, client);
			this.DoSpriteSubPKPoint(sl, pool, client);
			this.BroadcastRolePurpleName(sl, pool, client);
			Global.ProcessQueueCmds(client);
			this.ProcessRoleBattleNameInfoTimeOut(sl, pool, client);
			this.JugeTempHorseID(client);
			this.ChangeDayLoginNum(client);
			Global.ProcessClientHeart(client);
			this.DoSpriteMapLimitTimes(sl, pool, client);
			ClientManager.DoSpriteMapTimeLimit(client);
			ClientManager.DoSpriteHintToUpdateClient(client);
			ClientManager.DoSpriteGoodsTimeLimit(client);
			this.DoSpriteMeditateTime(client);
			client._IconStateMgr.DoSpriteIconTicks(client);
			GroupMailManager.CheckRoleGroupMail(client);
			RobotTaskValidator.getInstance().KickTimeout(client);
			GameManager.MerlinMagicBookMgr.DoMerlinSecretTime(client);
			SingletonTemplate<GetInterestingDataMgr>.Instance().Update(client);
			ClientCmdCheck.WriteLifeLogs(client);
		}

		public void DoSpriteBackgourndWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep <= 0 && !nextClient.ClientData.FirstPlayStart)
				{
					this.DoSpriteWorks(sl, pool, nextClient);
				}
			}
		}

		public void DoBuffersWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteLifeMagic(sl, pool, client);
			this.DoSpriteBuffers(sl, pool, client);
			this.CheckDSHideState(client);
			client.OneSecsTimerEventObject.Client = client;
			client.OneSecsTimerEventObject.NowTicks = TimeUtil.NOW();
			client.OneSecsTimerEventObject.DeltaTicks = client.OneSecsTimerEventObject.NowTicks - client.OneSecsTimerEventObject.LastRunTicks;
			if (client.OneSecsTimerEventObject.DeltaTicks < 60000L)
			{
				GlobalEventSource.getInstance().fireEvent(client.OneSecsTimerEventObject);
			}
			client.OneSecsTimerEventObject.LastRunTicks = client.OneSecsTimerEventObject.NowTicks;
		}

		public void DoBuffersExtension(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			long nowTicks = TimeUtil.NOW();
			SpriteAttack.ExecMagicsManyTimeDmageQueueEx(client);
			client.buffManager.UpdateByTime(client, nowTicks);
			if (client.bufferPropsManager.TimerUpdateProps(nowTicks, false))
			{
				client.delayExecModule.SetDelayExecProc(new DelayExecProcIds[]
				{
					2
				});
			}
			client.TimedActionMgr.Run(nowTicks);
			client.delayExecModule.ExecDelayProcs(client);
			SpriteMagicHelper.ExecuteAllItems(client);
		}

		public void DoDBWorks(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			this.DoSpriteDBHeart(sl, pool, client);
			Global.ProcessDBCmdByTicks(client, false);
			Global.ProcessDBSkillCmdByTicks(client, false);
			Global.ProcessDBRoleParamCmdByTicks(client, false);
			Global.ProcessDBEquipStrongCmdByTicks(client, false);
		}

		public void DoSpriteBuffersWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep <= 0)
				{
					this.DoBuffersWorks(sl, pool, nextClient);
				}
			}
		}

		public void DoSpriteExtensionWork(SocketListener sl, TCPOutPacketPool pool, int nThead, int nMaxThread)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nMaxThread > 0)
				{
					if (nextClient.ClientData.RoleID % nMaxThread != nThead)
					{
						continue;
					}
				}
				if (nextClient.ClientData.ClosingClientStep <= 0)
				{
					this.DoBuffersExtension(sl, pool, nextClient);
				}
			}
		}

		public void DoSpriteExtensionWorkByPerMap(int mapCode = -1, int subMapCode = -1)
		{
			SocketListener mySocketListener = Global._TCPManager.MySocketListener;
			TCPOutPacketPool tcpOutPacketPool = Global._TCPManager.TcpOutPacketPool;
			List<object> mapClients = GameManager.ClientMgr.GetMapClients(mapCode);
			if (mapClients != null && mapClients.Count != 0)
			{
				foreach (object obj in mapClients)
				{
					if (null != obj)
					{
						GameClient gameClient = obj as GameClient;
						if (null != gameClient)
						{
							if (subMapCode < 0 || gameClient.ClientData.CopyMapID == subMapCode)
							{
								if (gameClient.ClientData.ClosingClientStep <= 0)
								{
									this.DoBuffersExtension(mySocketListener, tcpOutPacketPool, gameClient);
								}
							}
						}
					}
				}
			}
		}

		public void DoSpriteDBWork(SocketListener sl, TCPOutPacketPool pool)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.ClosingClientStep <= 0)
				{
					this.DoDBWorks(sl, pool, nextClient);
				}
			}
		}

		public void DoSpritesMapGridMove(int nThead)
		{
			if (GameManager.Update9GridUsingPosition != 2)
			{
				int num = 0;
				int num2 = 0;
				long num3 = TimeUtil.NOW() - 3000L;
				List<int> list = new List<int>();
				List<int> list2 = new List<int>();
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					int vipLevel = nextClient.ClientData.VipLevel;
					int mainTaskID = nextClient.ClientData.MainTaskID;
					if (nThead == 0)
					{
						list.Add(vipLevel);
						list2.Add(mainTaskID);
					}
					if (nextClient.ClientData.ServerPosTicks >= num3)
					{
						if (nextClient.ClientData.RoleID % Program.MaxGird9UpdateWorkersNum == nThead)
						{
							if (mainTaskID < this.MinMainTaskForDoSpriteMapGridMove && vipLevel < this.MinVipLevelForDoSpriteMapGridMove)
							{
								nextClient.CurrentSlotTicks = 2000L;
							}
							else
							{
								nextClient.CurrentSlotTicks = (long)GameManager.MaxSlotOnUpdate9GridsTicks;
								ClientManager.DoSpriteMapGridMove(nextClient, 1);
								if (GameManager.MaxSleepOnDoMapGridMoveTicks > 0 && num2++ % 5 == 0)
								{
									Thread.Sleep(GameManager.MaxSleepOnDoMapGridMoveTicks);
								}
							}
						}
					}
				}
				if (nThead == 0)
				{
					list.Sort();
					int num4 = list.Count - 1;
					int num5 = 0;
					while (num4 >= 0 && num5 < 160)
					{
						if (num5 == 159)
						{
							this.MinVipLevelForDoSpriteMapGridMove = list[num4];
						}
						else
						{
							this.MinVipLevelForDoSpriteMapGridMove = list[num4] + 1;
						}
						num4--;
						num5++;
					}
					list2.Sort();
					num4 = list2.Count - 1;
					num5 = 0;
					while (num4 >= 0 && num5 < 160)
					{
						if (num5 < 159)
						{
							this.MinMainTaskForDoSpriteMapGridMove = list2[num4];
						}
						else
						{
							this.MinMainTaskForDoSpriteMapGridMove = list2[num4] + 1;
						}
						num4--;
						num5++;
					}
				}
			}
		}

		public void DoSpritesMapGridMoveNewMode(int nThead)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (nextClient.ClientData.RoleID % Program.MaxGird9UpdateWorkersNum == nThead)
				{
					Global.GameClientMoveGrid(nextClient);
				}
			}
		}

		public void NotifyBloodCastleMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = "";
				if (nCmdID == 517)
				{
					strCmd = string.Format("{0}:{1}", mapCode, nTimer);
				}
				else if (nCmdID == 533)
				{
					strCmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, nCmdID);
			}
		}

		public void NotifyBloodCastleCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0, GameClient client = null)
		{
			List<GameClient> clientsList = mapInfo.GetClientsList();
			if (null != clientsList)
			{
				string strCmd = "";
				if (nCmdID == 517)
				{
					strCmd = string.Format("{0}:{1}", mapInfo.FubenMapID, nTimer);
				}
				else if (nCmdID == 533)
				{
					strCmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				else if (nCmdID == 531)
				{
					BloodCastleDataInfo bloodCastleDataInfo = null;
					if (!Data.BloodCastleDataInfoList.TryGetValue(mapInfo.FubenMapID, out bloodCastleDataInfo) || bloodCastleDataInfo == null)
					{
						return;
					}
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}", new object[]
					{
						mapInfo.FubenMapID,
						nTimer,
						bloodCastleDataInfo.NeedKillMonster1Num,
						1,
						bloodCastleDataInfo.NeedKillMonster2Num,
						1,
						1,
						1
					});
				}
				for (int i = 0; i < clientsList.Count; i++)
				{
					if (clientsList[i] != client)
					{
						this.SendToClient(sl, pool, clientsList[i], strCmd, nCmdID);
					}
				}
				if (null != client)
				{
					this.SendToClient(sl, pool, client, strCmd, nCmdID);
				}
			}
		}

		public void NotifyBloodCastleCopySceneMsgEndFight(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, BloodCastleScene bcTmp, int nCmdID, int nTimer, int nTimeAward)
		{
			BloodCastleDataInfo bloodCastleDataInfo = null;
			if (Data.BloodCastleDataInfoList.TryGetValue(mapInfo.FubenMapID, out bloodCastleDataInfo))
			{
				if (bcTmp != null && bloodCastleDataInfo != null)
				{
					bcTmp.m_bEndFlag = true;
					List<GameClient> clientsList = mapInfo.GetClientsList();
					if (null != clientsList)
					{
						for (int i = 0; i < clientsList.Count; i++)
						{
							GameClient gameClient = clientsList[i];
							if (gameClient.ClientData.FuBenID <= 0 || GameManager.BloodCastleCopySceneMgr.IsBloodCastleCopyScene(gameClient.ClientData.FuBenID))
							{
								string text = null;
								string text2 = null;
								gameClient.ClientData.BloodCastleAwardPoint += nTimeAward;
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "BloodCastlePlayerPoint", gameClient.ClientData.BloodCastleAwardPoint, true);
								if (gameClient.ClientData.RoleID == bcTmp.m_nRoleID)
								{
									for (int j = 0; j < bloodCastleDataInfo.AwardItem1.Length; j++)
									{
										text += bloodCastleDataInfo.AwardItem1[j];
										if (j != bloodCastleDataInfo.AwardItem1.Length - 1)
										{
											text += "|";
										}
									}
								}
								for (int k = 0; k < bloodCastleDataInfo.AwardItem2.Length; k++)
								{
									text2 += bloodCastleDataInfo.AwardItem2[k];
									if (k != bloodCastleDataInfo.AwardItem2.Length - 1)
									{
										text2 += "|";
									}
								}
								int num = 0;
								if (bcTmp.m_bIsFinishTask)
								{
									num = 1;
								}
								Global.SaveRoleParamsInt32ValueToDB(gameClient, "BloodCastleSceneFinishFlag", num, true);
								string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
								{
									nTimer,
									num,
									gameClient.ClientData.BloodCastleAwardPoint,
									Global.CalcExpForRoleScore(gameClient.ClientData.BloodCastleAwardPoint, bloodCastleDataInfo.ExpModulus),
									gameClient.ClientData.BloodCastleAwardPoint * bloodCastleDataInfo.MoneyModulus,
									text,
									text2
								});
								GameManager.ClientMgr.SendToClient(gameClient, strCmd, nCmdID);
							}
						}
					}
				}
			}
		}

		public void NotifyDaimonSquareMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, int nSection, int nTimer, int nWave, int nNum, int nPlayerNum)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = "";
				if (nCmdID == 537)
				{
					strCmd = string.Format("{0}:{1}", nWave, nNum);
				}
				else if (nCmdID == 536)
				{
					strCmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 546)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, nCmdID);
			}
		}

		public void NotifyDaimonSquareCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0)
		{
			List<GameClient> clientsList = mapInfo.GetClientsList();
			if (null != clientsList)
			{
				string strCmd = "";
				if (nCmdID == 546)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				for (int i = 0; i < clientsList.Count; i++)
				{
					this.SendToClient(sl, pool, clientsList[i], strCmd, nCmdID);
				}
			}
		}

		public void NotifyDaimonSquareCopySceneMsg(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, int nCmdID, int nSection, int nTimer, int nWave, int nNum, int nPlayerNum)
		{
			List<GameClient> clientsList = mapInfo.GetClientsList();
			if (null != clientsList)
			{
				string strCmd = "";
				if (nCmdID == 537)
				{
					strCmd = string.Format("{0}:{1}", nWave, nNum);
				}
				else if (nCmdID == 536)
				{
					strCmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 546)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				for (int i = 0; i < clientsList.Count; i++)
				{
					this.SendToClient(sl, pool, clientsList[i], strCmd, nCmdID);
				}
			}
		}

		public void NotifyDaimonSquareCopySceneMsgEndFight(SocketListener sl, TCPOutPacketPool pool, CopyMap mapInfo, DaimonSquareScene dsInfo, int nCmdID, int nTimeAward)
		{
			DaimonSquareDataInfo daimonSquareDataInfo = null;
			if (Data.DaimonSquareDataInfoList.TryGetValue(mapInfo.FubenMapID, out daimonSquareDataInfo))
			{
				if (dsInfo != null && daimonSquareDataInfo != null)
				{
					dsInfo.m_bEndFlag = true;
					List<GameClient> clientsList = mapInfo.GetClientsList();
					if (null != clientsList)
					{
						for (int i = 0; i < clientsList.Count; i++)
						{
							if (clientsList[i] != null)
							{
								GameClient gameClient = clientsList[i];
								if (gameClient.ClientData.FuBenID <= 0 || GameManager.DaimonSquareCopySceneMgr.IsDaimonSquareCopyScene(gameClient.ClientData.FuBenID))
								{
									string text = null;
									gameClient.ClientData.DaimonSquarePoint += nTimeAward;
									Global.SaveRoleParamsInt32ValueToDB(gameClient, "DaimonSquarePlayerPoint", gameClient.ClientData.DaimonSquarePoint, true);
									for (int j = 0; j < daimonSquareDataInfo.AwardItem.Length; j++)
									{
										text += daimonSquareDataInfo.AwardItem[j];
										if (j != daimonSquareDataInfo.AwardItem.Length - 1)
										{
											text += "|";
										}
									}
									int num = 0;
									if (dsInfo.m_bIsFinishTask)
									{
										num = 1;
									}
									string strCmd = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
									{
										num,
										gameClient.ClientData.DaimonSquarePoint,
										Global.CalcExpForRoleScore(gameClient.ClientData.DaimonSquarePoint, daimonSquareDataInfo.ExpModulus),
										gameClient.ClientData.DaimonSquarePoint * daimonSquareDataInfo.MoneyModulus,
										text
									});
									GameManager.ClientMgr.SendToClient(gameClient, strCmd, nCmdID);
								}
							}
						}
					}
				}
			}
		}

		public void NotifyAngelTempleMsg(SocketListener sl, TCPOutPacketPool pool, int mapCode, int nCmdID, AngelTemplePointInfo[] array, int nSection, int nTimer = 0, int nValue = 0, int nType = 0, int nPlayerNum = 0, double nBossHP = 0.0)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				string strCmd = "";
				if (nCmdID == 570)
				{
					strCmd = string.Format("{0}:{1}", nSection, nTimer);
				}
				else if (nCmdID == 533)
				{
					strCmd = string.Format("{0}:{1}", nValue, nType);
				}
				else if (nCmdID == 545)
				{
					strCmd = string.Format("{0}", nPlayerNum);
				}
				else if (nCmdID == 572)
				{
					double num = Math.Round((double)array[0].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string roleName = array[0].m_RoleName;
					double num2 = Math.Round((double)array[1].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string roleName2 = array[1].m_RoleName;
					double num3 = Math.Round((double)array[2].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string roleName3 = array[2].m_RoleName;
					double num4 = Math.Round((double)array[3].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string roleName4 = array[3].m_RoleName;
					double num5 = Math.Round((double)array[4].m_DamagePoint / (double)GameManager.AngelTempleMgr.m_BossHP, 2);
					string roleName5 = array[4].m_RoleName;
					strCmd = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}:{9}:{10}", new object[]
					{
						Math.Round(nBossHP / (double)GameManager.AngelTempleMgr.m_BossHP, 2),
						roleName,
						num,
						roleName2,
						num2,
						roleName3,
						num3,
						roleName4,
						num4,
						roleName5,
						num5
					});
				}
				this.SendToClients(sl, pool, null, objectsByMap, strCmd, nCmdID);
			}
		}

		public void NotifyAngelTempleMsgBossDisappear(SocketListener sl, TCPOutPacketPool pool, int mapCode)
		{
			List<object> objectsByMap = this.Container.GetObjectsByMap(mapCode);
			if (null != objectsByMap)
			{
				for (int i = 0; i < objectsByMap.Count; i++)
				{
					if (objectsByMap[i] is GameClient)
					{
						GameClient client = objectsByMap[i] as GameClient;
						GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, StringUtil.substitute(GLang.GetLang(95, new object[0]), new object[0]), GameInfoTypeIndexes.Error, ShowGameInfoTypes.ErrAndBox, 0);
					}
				}
			}
		}

		public void NotifyTeamMemberMsg(SocketListener sl, TCPOutPacketPool pool, GameClient client, TeamData td, TeamCmds nCmd)
		{
			if (null != td)
			{
				lock (td)
				{
					for (int i = 0; i < td.TeamRoles.Count; i++)
					{
						GameClient gameClient = this.FindClient(td.TeamRoles[i].RoleID);
						if (null != gameClient)
						{
							if (nCmd == TeamCmds.Quit)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(96, new object[0]), new object[]
								{
									client.ClientData.RoleName
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
							}
							else if (nCmd == TeamCmds.AgreeApply)
							{
								GameManager.ClientMgr.NotifyImportantMsg(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, StringUtil.substitute(GLang.GetLang(97, new object[0]), new object[]
								{
									client.ClientData.RoleName
								}), GameInfoTypeIndexes.Normal, ShowGameInfoTypes.OnlyChatBox, 0);
							}
						}
					}
				}
			}
		}

		public List<object> GetPlayerByMap(GameClient client)
		{
			return this.Container.GetObjectsByMap(client.ClientData.MapCode);
		}

		public void NotifySelfUserStoreYinLiangChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StoreYinLiang);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 763);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public void NotifySelfUserStoreMoneyChange(SocketListener sl, TCPOutPacketPool pool, GameClient client)
		{
			string data = string.Format("{0}:{1}", client.ClientData.RoleID, client.ClientData.StoreMoney);
			TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(pool, data, 764);
			if (!sl.SendData(client.ClientSocket, tcpOutPacket, true))
			{
			}
		}

		public bool AddUserStoreYinLiang(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, long addYinLiang, string strFrom, bool isGM = false)
		{
			bool result;
			if (0L == addYinLiang)
			{
				result = true;
			}
			else
			{
				long storeYinLiang = client.ClientData.StoreYinLiang;
				lock (client.ClientData.StoreYinLiangMutex)
				{
					if (addYinLiang < 0L && !isGM && storeYinLiang < Math.Abs(addYinLiang))
					{
						return false;
					}
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, addYinLiang, isGM ? 1 : 0);
					string[] array = Global.ExecuteDBCmd(10173, strcmd, client.ServerId);
					if (null == array)
					{
						return false;
					}
					if (array.Length != 2)
					{
						return false;
					}
					if (Convert.ToInt64(array[0]) < 0L)
					{
						return false;
					}
					client.ClientData.StoreYinLiang = Convert.ToInt64(array[1]);
					GameManager.ClientMgr.NotifySelfUserStoreYinLiangChange(sl, pool, client);
					if (0L != addYinLiang)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "仓库金币", strFrom, "系统", client.ClientData.RoleName, "增加", (int)addYinLiang, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.StoreYinLiang, client.ServerId, null);
						EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.StoreYinLiang, addYinLiang, client.ClientData.StoreYinLiang, strFrom);
					}
				}
				Global.AddRoleStoreYinLiangEvent(client, storeYinLiang);
				result = true;
			}
			return result;
		}

		public bool AddUserStoreMoney(SocketListener sl, TCPClientPool tcpClientPool, TCPOutPacketPool pool, GameClient client, long addMoney, string strFrom, bool isGM = false)
		{
			bool result;
			if (0L == addMoney)
			{
				result = true;
			}
			else
			{
				long storeMoney = client.ClientData.StoreMoney;
				lock (client.ClientData.StoreMoneyMutex)
				{
					if (addMoney < 0L && !isGM && storeMoney < Math.Abs(addMoney))
					{
						return false;
					}
					string strcmd = string.Format("{0}:{1}:{2}", client.ClientData.RoleID, addMoney, isGM ? 1 : 0);
					string[] array = Global.ExecuteDBCmd(10174, strcmd, client.ServerId);
					if (null == array)
					{
						return false;
					}
					if (array.Length != 2)
					{
						return false;
					}
					if (Convert.ToInt64(array[0]) < 0L)
					{
						return false;
					}
					client.ClientData.StoreMoney = Convert.ToInt64(array[1]);
					if (0L != addMoney)
					{
						GameManager.logDBCmdMgr.AddDBLogInfo(-1, "仓库绑定金币", strFrom, "系统", client.ClientData.RoleName, "增加", (int)addMoney, client.ClientData.ZoneID, client.strUserID, (int)client.ClientData.StoreMoney, client.ServerId, null);
					}
				}
				GameManager.ClientMgr.NotifySelfUserStoreMoneyChange(sl, pool, client);
				Global.AddRoleStoreMoneyEvent(client, storeMoney);
				result = true;
			}
			return result;
		}

		public void NotifyAllActivityState(int type, int state, string activityTimeBegin = "", string activityTimeEnd = "", int activityID = 0)
		{
			int num = 0;
			GameClient nextClient;
			while ((nextClient = this.GetNextClient(ref num, false)) != null)
			{
				if (type != 10 || !nextClient.ClientSocket.IsKuaFuLogin)
				{
					string data = string.Format("{0}:{1}:{2}:{3}:{4}", new object[]
					{
						type,
						state,
						activityTimeBegin,
						activityTimeEnd,
						activityID
					});
					TCPOutPacket tcpOutPacket = TCPOutPacket.MakeTCPOutPacket(Global._TCPManager.TcpOutPacketPool, data, 770);
					if (!Global._TCPManager.MySocketListener.SendData(nextClient.ClientSocket, tcpOutPacket, true))
					{
					}
				}
			}
		}

		public void NotifyAllOneDollarChongZhiState()
		{
			OneDollarChongZhi oneDollarChongZhiActivity = HuodongCachingMgr.GetOneDollarChongZhiActivity();
			if (null != oneDollarChongZhiActivity)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					oneDollarChongZhiActivity.OnRoleLogin(nextClient);
				}
			}
		}

		public void NotifyAllInputFanLiNewState()
		{
			InputFanLiNew inputFanLiNewActivity = HuodongCachingMgr.GetInputFanLiNewActivity();
			if (null != inputFanLiNewActivity)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					inputFanLiNewActivity.OnRoleLogin(nextClient);
				}
			}
		}

		public void NotifyAllRegressActiveOpenState()
		{
			RegressActiveOpen regressActiveOpen = HuodongCachingMgr.GetRegressActiveOpen();
			if (null != regressActiveOpen)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					regressActiveOpen.OnRoleLogin(nextClient);
				}
			}
		}

		public void NotifyAllRegressActiveSignGiftState()
		{
			RegressActiveSignGift regressActiveSignGift = HuodongCachingMgr.GetRegressActiveSignGift();
			if (null == regressActiveSignGift)
			{
			}
		}

		public void NotifyAllRegressActiveTotalRechargeState()
		{
			RegressActiveTotalRecharge regressActiveTotalRecharge = HuodongCachingMgr.GetRegressActiveTotalRecharge();
			if (null == regressActiveTotalRecharge)
			{
			}
		}

		public void NotifyAllRegressActiveDayBuyState()
		{
			RegressActiveDayBuy regressActiveDayBuy = HuodongCachingMgr.GetRegressActiveDayBuy();
			if (null == regressActiveDayBuy)
			{
			}
		}

		public void NotifyAllRegressActiveStoreState()
		{
			RegressActiveStore regressActiveStore = HuodongCachingMgr.GetRegressActiveStore();
			if (null == regressActiveStore)
			{
			}
		}

		public void ReGenerateSpecActGroup()
		{
			SpecialActivity specialActivity = HuodongCachingMgr.GetSpecialActivity();
			if (null != specialActivity)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					specialActivity.OnRoleLogin(nextClient, false);
					if (nextClient._IconStateMgr.CheckSpecialActivity(nextClient))
					{
						nextClient._IconStateMgr.SendIconStateToClient(nextClient);
					}
				}
			}
		}

		public void ReGenerateEverydayActGroup()
		{
			EverydayActivity everydayActivity = HuodongCachingMgr.GetEverydayActivity();
			if (null != everydayActivity)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					everydayActivity.OnRoleLogin(nextClient);
					if (nextClient._IconStateMgr.CheckEverydayActivity(nextClient))
					{
						nextClient._IconStateMgr.SendIconStateToClient(nextClient);
					}
				}
			}
		}

		public void ReGenerateSpecPriorityActGroup()
		{
			SpecPriorityActivity specPriorityActivity = HuodongCachingMgr.GetSpecPriorityActivity();
			if (null != specPriorityActivity)
			{
				int num = 0;
				GameClient nextClient;
				while ((nextClient = this.GetNextClient(ref num, false)) != null)
				{
					specPriorityActivity.OnRoleLogin(nextClient, false);
					if (nextClient._IconStateMgr.CheckSpecPriorityActivity(nextClient))
					{
						nextClient._IconStateMgr.SendIconStateToClient(nextClient);
					}
				}
			}
		}

		public bool ModifyFuMoLingShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10217");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "附魔灵石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10217", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.FuMoMoney, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 146, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornYinJiPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10246");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生印记点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10246", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornLevelUpPoint, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 151, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornCuiLianPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10249");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶淬炼点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10249", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornCuiLian, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 152, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornDuanZaoPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10250");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶锻造点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10250", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornDuanZao, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 153, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornNiePanPointValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10251");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生进阶涅槃点", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10251", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornNiePan, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 154, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornFengYinJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10252");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "封印晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10252", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornFengYin, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 155, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornChongShengJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10253");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10253", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornChongSheng, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 156, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornXuanCaiJinShiValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10254");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "炫彩晶石", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10254", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornXuanCai, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 157, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		public bool ModifyRebornEquipHoleValue(GameClient client, int addValue, string strFrom, bool writeToDB = true, bool notifyClient = true, bool isGM = false)
		{
			bool result;
			if (0 == addValue)
			{
				result = true;
			}
			else
			{
				long num = (long)Global.GetRoleParamsInt32FromDB(client, "10255");
				long num2 = num + (long)addValue;
				if (isGM && (num2 > 2147483647L || num2 < -2147483648L))
				{
					result = false;
				}
				else
				{
					num2 = (long)((int)Global.Clamp(num2, -2147483648L, 2147483647L));
					GameManager.logDBCmdMgr.AddDBLogInfo(-1, "重生装备槽灌注次数", strFrom, client.ClientData.RoleName, "系统", "修改", addValue, client.ClientData.ZoneID, client.strUserID, (int)num2, client.ServerId, null);
					Global.SaveRoleParamsInt32ValueToDB(client, "10255", (int)num2, writeToDB);
					EventLogManager.AddMoneyEvent(client, OpTypes.AddOrSub, OpTags.None, MoneyTypes.RebornEquipHole, (long)addValue, num2, strFrom);
					if (notifyClient)
					{
						this.NotifySelfPropertyValue(client, 161, (long)((int)num2));
					}
					result = true;
				}
			}
			return result;
		}

		private const int MAX_CLIENT_COUNT = 2000;

		public const long Before1970Ticks = 62135625600000L;

		private GameClient[] _ArrayClients = new GameClient[2000];

		private Dictionary<int, int> _DictClientNids = new Dictionary<int, int>(2000);

		private List<int> _FreeClientList = new List<int>(2000);

		private SpriteContainer Container = new SpriteContainer();

		private long LastTransferTicks = 0L;

		private static double[] IgnoreDefenseAndDogeSubPercent = new double[]
		{
			0.05,
			0.1,
			0.2
		};

		private int MinVipLevelForDoSpriteMapGridMove = 10;

		private int MinMainTaskForDoSpriteMapGridMove = 5000;
	}
}
