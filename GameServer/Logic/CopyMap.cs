using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class CopyMap
	{
		public int CopyMapID { get; set; }

		public int FuBenSeqID { get; set; }

		public int MapCode { get; set; }

		public int FubenMapID { get; set; }

		public MapTypes CopyMapType { get; set; }

		public long InitTicks { get; set; }

		public bool bStoryCopyMapFinishStatus { get; set; }

		public void SetKilledNormalDict(int monsterID)
		{
			lock (this)
			{
				if (-1 != monsterID)
				{
					this._KilledNormalDict[monsterID] = true;
				}
				else
				{
					this._KilledNormalDict.Clear();
				}
			}
		}

		public void ClearKilledNormalDict()
		{
			lock (this)
			{
				this._KilledNormalDict.Clear();
			}
		}

		public int KilledNormalNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledNormalDict.Count;
				}
				return count;
			}
		}

		public void SetKilledDynamicMonsterDict(long uniqueID)
		{
			lock (this)
			{
				if (-1L != uniqueID)
				{
					this._KilledDynamicMonsterDict[uniqueID] = true;
				}
				else
				{
					this._KilledDynamicMonsterDict.Clear();
				}
			}
		}

		public void ClearKilledDynamicMonsterDict()
		{
			lock (this)
			{
				this._KilledDynamicMonsterDict.Clear();
			}
		}

		public int KilledDynamicMonsterNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledDynamicMonsterDict.Count;
				}
				return count;
			}
		}

		public void SetKilledBossDict(int monsterID)
		{
			lock (this)
			{
				if (-1 != monsterID)
				{
					this._KilledBossDict[monsterID] = true;
				}
				else
				{
					this._KilledBossDict.Clear();
				}
			}
		}

		public void ClearKilledBossDict()
		{
			lock (this)
			{
				this._KilledBossDict.Clear();
			}
		}

		public int KilledBossNum
		{
			get
			{
				int count;
				lock (this)
				{
					count = this._KilledBossDict.Count;
				}
				return count;
			}
		}

		public int FreshPlayerCreateGateFlag { get; set; }

		public int FreshPlayerKillMonsterACount { get; set; }

		public int FreshPlayerKillMonsterBCount { get; set; }

		public bool HaveBirthShuiJingGuan { get; set; }

		public bool ExecEnterMapLuaFile { get; set; }

		public long CanRemoveTicks { get; private set; }

		public void SetRemoveTicks(long ticks)
		{
			this.CanRemoveTicks = ticks;
		}

		public void AddGameClient(GameClient client)
		{
			lock (this._ClientsList)
			{
				this._ClientsList.Add(client);
			}
		}

		public void RemoveGameClient(GameClient client)
		{
			long lastLeaveClientTicks = TimeUtil.NOW();
			lock (this._ClientsList)
			{
				this._ClientsList.Remove(client);
				this.LastLeaveClientTicks = lastLeaveClientTicks;
			}
		}

		public List<GameClient> GetClientsList()
		{
			List<GameClient> result = null;
			lock (this._ClientsList)
			{
				result = this._ClientsList.GetRange(0, this._ClientsList.Count);
			}
			return result;
		}

		public List<object> GetClientsList2()
		{
			List<object> list = new List<object>(10);
			lock (this._ClientsList)
			{
				foreach (GameClient item in this._ClientsList)
				{
					list.Add(item);
				}
			}
			return list;
		}

		public long GetLastLeaveClientTicks()
		{
			long lastLeaveClientTicks;
			lock (this._ClientsList)
			{
				lastLeaveClientTicks = this.LastLeaveClientTicks;
			}
			return lastLeaveClientTicks;
		}

		public int GetGameClientCount()
		{
			int count;
			lock (this._ClientsList)
			{
				count = this._ClientsList.Count;
			}
			return count;
		}

		public bool IsInitMonster
		{
			get
			{
				bool isInitMonster;
				lock (this)
				{
					isInitMonster = this._IsInitMonster;
				}
				return isInitMonster;
			}
			set
			{
				lock (this)
				{
					this._IsInitMonster = value;
				}
			}
		}

		public void AddGuangMuEvent(int guangMuID, int show)
		{
			MapAIEvent item = new MapAIEvent
			{
				GuangMuID = guangMuID,
				Show = show
			};
			lock (this.EventQueue)
			{
				this.EventQueue.Add(item);
			}
		}

		public bool bNeedRemove = false;

		public int TotalNormalNum = 0;

		private Dictionary<int, bool> _KilledNormalDict = new Dictionary<int, bool>();

		public int TotalDynamicMonsterNum = 0;

		private Dictionary<long, bool> _KilledDynamicMonsterDict = new Dictionary<long, bool>();

		public int TotalBossNum = 0;

		private Dictionary<int, bool> _KilledBossDict = new Dictionary<int, bool>();

		public bool CopyMapPassAwardFlag = false;

		public bool IsKuaFuCopy = false;

		public bool CustomPassAwards;

		private List<GameClient> _ClientsList = new List<GameClient>();

		private long LastLeaveClientTicks = 0L;

		private bool _IsInitMonster = false;

		public List<MapAIEvent> EventQueue = new List<MapAIEvent>();
	}
}
