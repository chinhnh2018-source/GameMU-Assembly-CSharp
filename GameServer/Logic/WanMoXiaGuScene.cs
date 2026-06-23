using System;
using System.Collections.Generic;
using Tmsk.Contract;

namespace GameServer.Logic
{
	public class WanMoXiaGuScene
	{
		public bool AddKilledMonster(Monster monster)
		{
			bool result = false;
			lock (this.KilledMonsterHashSet)
			{
				if (!this.KilledMonsterHashSet.Contains(monster.UniqueID))
				{
					this.KilledMonsterHashSet.Add(monster.UniqueID);
					result = true;
				}
			}
			return result;
		}

		public void CleanAllInfo()
		{
		}

		public CopyMap CopyMapInfo;

		public int MapID;

		public int CopyMapID;

		public int FuBenSeqId;

		public long GameId;

		public int PlayerCount;

		public WanMoXiaGuScoreData ScoreData = new WanMoXiaGuScoreData();

		public GameSceneStatuses SceneStatus = 0;

		public long PrepareTime;

		public long BeginTime;

		public long EndTime;

		public long LeaveTime;

		public GameSceneStateTimeData StateTimeData = new GameSceneStateTimeData();

		public double BossLifePercent;

		public bool MonsterCreated;

		public int MonsterCount;

		public Monster Boss;

		public long NextRelifeTicks;

		public int Success;

		public WanMoXiaGuMonsterConfigInfo ZuoQiInfo;

		public HashSet<long> KilledMonsterHashSet = new HashSet<long>();
	}
}
