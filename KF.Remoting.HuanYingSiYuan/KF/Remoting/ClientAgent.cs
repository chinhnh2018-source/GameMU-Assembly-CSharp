using System;
using System.Collections.Generic;
using System.Linq;
using AutoCSer.Net.TcpServer;
using KF.Contract;
using KF.Contract.Data;
using KF.Contract.Interface;
using KF.Remoting.Data;

namespace KF.Remoting
{
	internal sealed class ClientAgent
	{
		public KuaFuClientContext ClientInfo { get; private set; }

		public bool IsAlive
		{
			get
			{
				return this.MaxActiveTicks > Global.NowTime.Ticks;
			}
		}

		public long DeltaTime
		{
			get
			{
				return (Global.NowTime.Ticks - this.MaxActiveTicks) / 10000000L;
			}
		}

		public bool IsDead
		{
			get
			{
				return this.MaxDeadTicks < Global.NowTime.Ticks;
			}
		}

		public long TotalRolePayload
		{
			get
			{
				return this.TotalFubenRolePayLoad + this.TotalMainlineRolePayLoad;
			}
		}

		public long TotalFubenRolePayLoad { get; private set; }

		public long TotalMainlineRolePayLoad { get; private set; }

		public ClientAgent(KuaFuClientContext clientInfo)
		{
			this.ClientInfo = clientInfo;
			this.ClientHeartTick();
		}

		public void Reset(KuaFuClientContext clientInfo)
		{
			lock (this.Mutex)
			{
				this.ClientInfo = clientInfo;
				this.ClientHeartTick();
				this.AlivedGameDict.Clear();
				this.TotalFubenRolePayLoad = 0L;
				this.TotalMainlineRolePayLoad = 0L;
			}
		}

		public void AddSession(string sessionId, int gameType, IKuaFuClient callback)
		{
			lock (this.Mutex)
			{
				this.SessionId2GameTypeDict[sessionId] = gameType;
				this.KuaFuClientDict[gameType] = callback;
			}
		}

		public void RemoveSession(string sessionId)
		{
			lock (this.Mutex)
			{
				int key;
				if (this.SessionId2GameTypeDict.TryGetValue(sessionId, out key))
				{
					this.KuaFuClientDict.Remove(key);
					this.SessionId2GameTypeDict.Remove(sessionId);
				}
			}
		}

		public Dictionary<int, GameTypeStaticsData> GetGameTypeStatics()
		{
			Dictionary<int, GameTypeStaticsData> dictionary = new Dictionary<int, GameTypeStaticsData>();
			if (this.IsAlive)
			{
				lock (this.Mutex)
				{
					foreach (KeyValuePair<int, Dictionary<long, int>> keyValuePair in this.AlivedGameDict)
					{
						GameTypeStaticsData gameTypeStaticsData = null;
						if (!dictionary.TryGetValue(keyValuePair.Key, out gameTypeStaticsData))
						{
							gameTypeStaticsData = new GameTypeStaticsData();
							gameTypeStaticsData.ServerAlived = 1;
							dictionary[keyValuePair.Key] = gameTypeStaticsData;
						}
						gameTypeStaticsData.FuBenAlived = keyValuePair.Value.Count;
						gameTypeStaticsData.SingUpRoleCount = 0;
						gameTypeStaticsData.StartGameRoleCount = keyValuePair.Value.ToList<KeyValuePair<long, int>>().Sum((KeyValuePair<long, int> c) => c.Value);
					}
					if (this.TotalMainlineRolePayLoad > 0L)
					{
						dictionary.Add(7, new GameTypeStaticsData
						{
							ServerAlived = 1,
							StartGameRoleCount = (int)this.TotalMainlineRolePayLoad
						});
					}
				}
			}
			return dictionary;
		}

		public void SetMainlinePayload(int payload)
		{
			lock (this.Mutex)
			{
				this.TotalMainlineRolePayLoad = (long)payload;
			}
		}

		public void AssginKfFuben(GameTypes gameType, long gameId, int roleNum)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dictionary = null;
				if (!this.AlivedGameDict.TryGetValue(gameType, out dictionary))
				{
					dictionary = new Dictionary<long, int>();
					this.AlivedGameDict[gameType] = dictionary;
				}
				dictionary.Add(gameId, roleNum);
				this.TotalFubenRolePayLoad += (long)roleNum;
			}
		}

		public void RemoveKfFuben(GameTypes gameType, long gameId)
		{
			lock (this.Mutex)
			{
				Dictionary<long, int> dictionary = null;
				if (this.AlivedGameDict.TryGetValue(gameType, out dictionary) && dictionary.ContainsKey(gameId))
				{
					this.TotalFubenRolePayLoad -= (long)dictionary[gameId];
					dictionary.Remove(gameId);
				}
			}
		}

		public void ClientHeartTick()
		{
			this.MaxActiveTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerActiveTicks).Ticks;
			this.MaxDeadTicks = Global.NowTime.AddSeconds((double)Consts.RenewServerDeadTicks).Ticks;
		}

		public void PostAsyncEvent(GameTypes gameType, AsyncDataItem evItem)
		{
			lock (this.Mutex)
			{
				Queue<AsyncDataItem> queue = null;
				if (!this.EvItemOfGameType.TryGetValue(gameType, out queue))
				{
					queue = new Queue<AsyncDataItem>();
					this.EvItemOfGameType[gameType] = queue;
				}
				queue.Enqueue(evItem);
				while (queue.Count > 100000)
				{
					queue.Dequeue();
				}
			}
		}

		public void PostAsyncEvent(KFCallMsg evItem)
		{
			lock (this.Mutex)
			{
				Queue<KFCallMsg> msgQueue = this.AgentData.MsgQueue;
				msgQueue.Enqueue(evItem);
				while (msgQueue.Count > 100000)
				{
					msgQueue.Dequeue();
				}
			}
		}

		public AsyncDataItem[] PickAsyncEvent(GameTypes gameType)
		{
			lock (this.Mutex)
			{
				this.ClientHeartTick();
				Queue<AsyncDataItem> queue = null;
				if (this.EvItemOfGameType.TryGetValue(gameType, out queue))
				{
					int num = Math.Min(queue.Count, KuaFuServerManager.MaxGetAsyncItemDataCount);
					if (num > 0)
					{
						AsyncDataItem[] array = new AsyncDataItem[num];
						for (int i = 0; i < num; i++)
						{
							array[i] = queue.Dequeue();
						}
						return array;
					}
				}
			}
			return null;
		}

		public void TryInitGameType(int gameType)
		{
			lock (this.Mutex)
			{
				if (!this.AlivedGameDict.ContainsKey(gameType))
				{
					this.AlivedGameDict[gameType] = new Dictionary<long, int>();
				}
			}
		}

		private const int MaxCachedAsyncDataItemCount = 100000;

		private object Mutex = new object();

		public AgentDataObj AgentData = new AgentDataObj();

		public Func<ReturnValue<KFCallMsg>, bool> KFCallMsg;

		private Dictionary<int, IKuaFuClient> KuaFuClientDict = new Dictionary<int, IKuaFuClient>();

		private Dictionary<string, int> SessionId2GameTypeDict = new Dictionary<string, int>();

		private long MaxActiveTicks = 0L;

		private long MaxDeadTicks = 0L;

		private Dictionary<int, Queue<AsyncDataItem>> EvItemOfGameType = new Dictionary<int, Queue<AsyncDataItem>>();

		private Dictionary<int, Dictionary<long, int>> AlivedGameDict = new Dictionary<int, Dictionary<long, int>>();
	}
}
