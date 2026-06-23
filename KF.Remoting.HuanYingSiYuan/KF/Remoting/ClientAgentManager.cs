using System;
using System.Collections.Generic;
using System.ServiceModel;
using KF.Contract;
using KF.Contract.Data;
using KF.Remoting.Data;
using Server.Tools;

namespace KF.Remoting
{
	internal class ClientAgentManager
	{
		public static ClientAgentManager Instance()
		{
			return ClientAgentManager._AgentMgr;
		}

		private ClientAgentManager()
		{
		}

		public bool IsAgentAlive(int serverId)
		{
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent) && clientAgent.IsAlive)
				{
					return true;
				}
			}
			return false;
		}

		public bool ExistAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = this.ServerId2ClientAgent.ContainsKey(serverId);
			}
			return result;
		}

		public bool IsAnyKfAgentAlive()
		{
			lock (this.Mutex)
			{
				foreach (int serverId in this.AutoKfServerId)
				{
					if (this.IsAgentAlive(serverId))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool IsKfAgent(int serverId)
		{
			bool result;
			lock (this.Mutex)
			{
				result = (this.AllKfServerId.Contains(serverId) && this.ExistAgent(serverId));
			}
			return result;
		}

		public void SetAllKfServerId(HashSet<int> existKfIds)
		{
			lock (this.Mutex)
			{
				this.AllKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId = new HashSet<int>(existKfIds);
				this.AutoKfServerId.ExceptWith(KuaFuServerManager.SpecialLineDict.Keys);
			}
		}

		public ClientAgent GetCurrentClientAgent(int serverId)
		{
			ClientAgent clientAgent;
			ClientAgent result;
			if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent))
			{
				result = clientAgent;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public void OnConnectionClose(object sender, EventArgs args)
		{
			if (null != OperationContext.Current)
			{
				string sessionId = OperationContext.Current.SessionId;
				lock (this.Mutex)
				{
					ClientAgent clientAgent;
					if (this.SessionId2ClientAgent.TryGetValue(sessionId, out clientAgent))
					{
						clientAgent.RemoveSession(sessionId);
					}
				}
			}
		}

		public int InitializeClient(KuaFuClientContext clientInfo, out bool bFistInit)
		{
			bFistInit = false;
			if (KuaFuServerManager.LimitIP)
			{
				bool flag = true;
				KuaFuServerInfo kuaFuServerInfo = KuaFuServerManager.GetKuaFuServerInfo(clientInfo.ServerId);
				if (null == kuaFuServerInfo)
				{
					LogManager.WriteLog(2, string.Format("非法连接,无效的服务器编号#serverid={0},ip={1},gametype={2}", clientInfo.ServerId, clientInfo.Token, clientInfo.GameType), null, true);
					return -1;
				}
				if (kuaFuServerInfo != null && !string.IsNullOrEmpty(clientInfo.Token))
				{
					if (!string.IsNullOrEmpty(kuaFuServerInfo.LanIp) && clientInfo.Token.Contains(kuaFuServerInfo.LanIp))
					{
						flag = false;
					}
					else if (clientInfo.Token.Contains(kuaFuServerInfo.Ip))
					{
						flag = false;
					}
				}
				if (flag)
				{
					LogManager.WriteLog(2, string.Format("非法连接#serverid={0},ip={1},ip={2},lanip={3},gametype={4}", new object[]
					{
						clientInfo.ServerId,
						clientInfo.Token,
						kuaFuServerInfo.Ip,
						kuaFuServerInfo.LanIp,
						clientInfo.GameType
					}), null, true);
					return -1;
				}
			}
			int clientId;
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (!this.ServerId2ClientAgent.TryGetValue(clientInfo.ServerId, out clientAgent))
				{
					LogManager.WriteLog(0, string.Format("InitializeClient服务器连接1.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [Service首次连接过来]", new object[]
					{
						clientInfo.ServerId,
						clientInfo.ClientId,
						clientInfo.Token,
						clientInfo.GameType
					}), null, true);
					bFistInit = true;
					clientInfo.ClientId = KuaFuServerManager.GetUniqueClientId();
					clientAgent = new ClientAgent(clientInfo);
					this.ServerId2ClientAgent[clientInfo.ServerId] = clientAgent;
				}
				else if (clientInfo.Token != clientAgent.ClientInfo.Token)
				{
					if (clientInfo.ClientId == clientAgent.ClientInfo.ClientId)
					{
						LogManager.WriteLog(0, string.Format("InitializeClient服务器IP变化.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							clientInfo.GameType
						}), null, true);
					}
					else
					{
						if (clientAgent.IsAlive)
						{
							LogManager.WriteLog(0, string.Format("InitializeClient服务器ID重复,禁止连接.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
							{
								clientInfo.ServerId,
								clientInfo.ClientId,
								clientInfo.Token,
								clientInfo.GameType
							}), null, true);
							return -11002;
						}
						bFistInit = true;
						clientInfo.ClientId = KuaFuServerManager.GetUniqueClientId();
						clientAgent.Reset(clientInfo);
						LogManager.WriteLog(0, string.Format("InitializeClient服务器IP变化.ServerId:{0},ClientId:{1},info:{2},GameType:{3}", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							clientInfo.GameType
						}), null, true);
					}
				}
				else if (clientInfo.ClientId != clientAgent.ClientInfo.ClientId)
				{
					if (clientInfo.ClientId <= 0)
					{
						clientInfo.ClientId = clientAgent.ClientInfo.ClientId;
						clientAgent.Reset(clientInfo);
						LogManager.WriteLog(0, string.Format("InitializeClient服务器重连.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [首次连接过来]", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							clientInfo.GameType
						}), null, true);
					}
					else
					{
						LogManager.WriteLog(0, string.Format("InitializeClient服务器重连.ServerId:{0},ClientId:{1},info:{2},GameType:{3} [非首次连接过来]", new object[]
						{
							clientInfo.ServerId,
							clientInfo.ClientId,
							clientInfo.Token,
							clientInfo.GameType
						}), null, true);
					}
				}
				else if (!Global.TestMode && !clientAgent.IsAlive)
				{
					LogManager.WriteLog(0, string.Format("InitializeClient服务器连接和上次心跳时间间隔过长.ServerId:{0},ClientId:{1},info:{2},GameType:{3},time={4}", new object[]
					{
						clientInfo.ServerId,
						clientInfo.ClientId,
						clientInfo.Token,
						clientInfo.GameType,
						clientAgent.DeltaTime
					}), null, true);
				}
				if (clientAgent != null)
				{
					clientInfo.ClientId = clientAgent.ClientInfo.ClientId;
					clientAgent.ClientHeartTick();
					clientAgent.TryInitGameType(clientInfo.GameType);
				}
				clientId = clientInfo.ClientId;
			}
			return clientId;
		}

		public int InitializeClient(KuaFuClientContext clientInfo)
		{
			bool flag = false;
			return this.InitializeClient(clientInfo, out flag);
		}

		public void BroadCastAsyncEvent(GameTypes gameType, AsyncDataItem[] evItems)
		{
			if (evItems != null && evItems.Length > 0)
			{
				lock (this.Mutex)
				{
					for (int i = 0; i < evItems.Length; i++)
					{
						this.BroadCastAsyncEvent(gameType, evItems[i], 0);
					}
				}
			}
		}

		public void BroadCastAsyncEvent(GameTypes gameType, AsyncDataItem evItem, int srcServerId = 0)
		{
			if (evItem != null)
			{
				lock (this.Mutex)
				{
					foreach (int num in this.ServerId2ClientAgent.Keys)
					{
						if (num != srcServerId)
						{
							this.PostAsyncEvent(num, gameType, evItem);
						}
					}
				}
			}
		}

		public void KFBroadCastAsyncEvent(GameTypes gameType, AsyncDataItem evItem)
		{
			if (evItem != null)
			{
				lock (this.Mutex)
				{
					foreach (int serverId in this.AllKfServerId)
					{
						this.PostAsyncEvent(serverId, gameType, evItem);
					}
				}
			}
		}

		public void PostAsyncEvent(int ServerId, GameTypes gameType, AsyncDataItem evItem)
		{
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(ServerId, out clientAgent))
				{
					clientAgent.PostAsyncEvent(gameType, evItem);
				}
			}
		}

		public AsyncDataItem[] PickAsyncEvent(int serverId, GameTypes gameType)
		{
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent))
				{
					return clientAgent.PickAsyncEvent(gameType);
				}
			}
			return null;
		}

		public void BroadCastMsg(KFCallMsg msg, int srcServerId = 0)
		{
			lock (this.Mutex)
			{
				foreach (int num in this.ServerId2ClientAgent.Keys)
				{
					if (num != srcServerId)
					{
						this.SendMsg(num, msg);
					}
				}
			}
		}

		public void KFBroadCastMsg(KFCallMsg msg)
		{
			if (msg != null)
			{
				lock (this.Mutex)
				{
					foreach (int serverId in this.AllKfServerId)
					{
						this.SendMsg(serverId, msg);
					}
				}
			}
		}

		public void SendMsg(int serverId, KFCallMsg msg)
		{
			lock (this.Mutex)
			{
				lock (this.Mutex)
				{
					ClientAgent clientAgent;
					if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent))
					{
						clientAgent.PostAsyncEvent(msg);
					}
				}
			}
		}

		public void SendAsyncKuaFuMsg()
		{
			lock (this.Mutex)
			{
				foreach (ClientAgent clientAgent in this.ServerId2ClientAgent.Values)
				{
					int num = 0;
					if (clientAgent.KFCallMsg != null && clientAgent.AgentData.MsgQueue.Count > 0)
					{
						Queue<KFCallMsg> msgQueue = clientAgent.AgentData.MsgQueue;
						do
						{
							try
							{
								KFCallMsg kfcallMsg = msgQueue.Peek();
								if (!clientAgent.KFCallMsg(kfcallMsg))
								{
									break;
								}
								msgQueue.Dequeue();
							}
							catch (Exception ex)
							{
								LogManager.WriteException(ex.ToString());
								break;
							}
						}
						while (msgQueue.Count > 0 && num++ < 2000);
					}
				}
			}
		}

		public bool AssginKfFuben(GameTypes gameType, long gameId, int roleNum, out int kfSrvId)
		{
			kfSrvId = 0;
			lock (this.Mutex)
			{
				long num = long.MaxValue;
				ClientAgent clientAgent = null;
				foreach (int key in this.AutoKfServerId)
				{
					ClientAgent clientAgent2 = null;
					if (this.ServerId2ClientAgent.TryGetValue(key, out clientAgent2) && clientAgent2.IsAlive && clientAgent2.TotalRolePayload < num)
					{
						num = clientAgent2.TotalRolePayload;
						clientAgent = clientAgent2;
					}
				}
				if (clientAgent != null)
				{
					clientAgent.AssginKfFuben(gameType, gameId, roleNum);
					kfSrvId = clientAgent.ClientInfo.ServerId;
					return true;
				}
			}
			return false;
		}

		public bool SpecialKfFuben(GameTypes gameType, long gameId, int roleNum, out int kfSrvId)
		{
			kfSrvId = 0;
			ClientAgent clientAgent = null;
			int specialLineId = KuaFuServerManager.GetSpecialLineId(gameType);
			bool result;
			if (specialLineId <= 0)
			{
				kfSrvId = -3;
				result = false;
			}
			else
			{
				lock (this.Mutex)
				{
					ClientAgent clientAgent2 = null;
					if (this.ServerId2ClientAgent.TryGetValue(specialLineId, out clientAgent2) && clientAgent2.IsAlive)
					{
						clientAgent = clientAgent2;
					}
					else
					{
						kfSrvId = -100;
					}
					if (clientAgent != null)
					{
						clientAgent.AssginKfFuben(gameType, gameId, roleNum);
						kfSrvId = clientAgent.ClientInfo.ServerId;
						return true;
					}
				}
				result = false;
			}
			return result;
		}

		public void RemoveKfFuben(GameTypes gameType, int kfSrvId, long gameId)
		{
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(kfSrvId, out clientAgent))
				{
					clientAgent.RemoveKfFuben(gameType, gameId);
				}
			}
		}

		public void SetMainlinePayload(int serverId, int payload)
		{
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent))
				{
					clientAgent.SetMainlinePayload(payload);
				}
			}
		}

		public void SetGameTypeLoad(GameTypes gameType, int signUpCount, int startCount)
		{
			lock (this.Mutex)
			{
				GameTypeStaticsData gameTypeStaticsData = null;
				if (!this.GameTypeLoadDict.TryGetValue(gameType, out gameTypeStaticsData))
				{
					gameTypeStaticsData = new GameTypeStaticsData();
					this.GameTypeLoadDict[gameType] = gameTypeStaticsData;
				}
				gameTypeStaticsData.SingUpRoleCount = signUpCount;
				gameTypeStaticsData.StartGameRoleCount = startCount;
			}
		}

		public void GetServerState(int serverId, out int state, out int load)
		{
			state = 0;
			load = 0;
			lock (this.Mutex)
			{
				ClientAgent clientAgent = null;
				if (this.ServerId2ClientAgent.TryGetValue(serverId, out clientAgent))
				{
					if (clientAgent.IsAlive)
					{
						state = 1;
						load = (int)clientAgent.TotalRolePayload;
					}
				}
			}
		}

		public Dictionary<int, GameTypeStaticsData> GetGameTypeStatics()
		{
			Dictionary<int, GameTypeStaticsData> dictionary = new Dictionary<int, GameTypeStaticsData>();
			lock (this.Mutex)
			{
				foreach (int key in this.AllKfServerId)
				{
					ClientAgent clientAgent = null;
					if (this.ServerId2ClientAgent.TryGetValue(key, out clientAgent))
					{
						foreach (KeyValuePair<int, GameTypeStaticsData> keyValuePair in clientAgent.GetGameTypeStatics())
						{
							GameTypeStaticsData gameTypeStaticsData = null;
							if (!dictionary.TryGetValue(keyValuePair.Key, out gameTypeStaticsData))
							{
								gameTypeStaticsData = new GameTypeStaticsData();
								dictionary[keyValuePair.Key] = gameTypeStaticsData;
							}
							gameTypeStaticsData.ServerAlived += keyValuePair.Value.ServerAlived;
							gameTypeStaticsData.FuBenAlived += keyValuePair.Value.FuBenAlived;
							gameTypeStaticsData.SingUpRoleCount += keyValuePair.Value.SingUpRoleCount;
							gameTypeStaticsData.StartGameRoleCount += keyValuePair.Value.StartGameRoleCount;
						}
					}
				}
				foreach (KeyValuePair<int, GameTypeStaticsData> keyValuePair2 in this.GameTypeLoadDict)
				{
					if (dictionary.ContainsKey(keyValuePair2.Key))
					{
						dictionary[keyValuePair2.Key].SingUpRoleCount = keyValuePair2.Value.SingUpRoleCount;
						dictionary[keyValuePair2.Key].StartGameRoleCount = keyValuePair2.Value.StartGameRoleCount;
					}
				}
			}
			return dictionary;
		}

		private static ClientAgentManager _AgentMgr = new ClientAgentManager();

		private object Mutex = new object();

		private Dictionary<int, ClientAgent> ServerId2ClientAgent = new Dictionary<int, ClientAgent>();

		private Dictionary<string, ClientAgent> SessionId2ClientAgent = new Dictionary<string, ClientAgent>();

		private HashSet<int> AllKfServerId = new HashSet<int>();

		private HashSet<int> AutoKfServerId = new HashSet<int>();

		private Dictionary<int, GameTypeStaticsData> GameTypeLoadDict = new Dictionary<int, GameTypeStaticsData>();
	}
}
