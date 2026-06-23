using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	public class ZorkBattleMonsterConfig
	{
		public int RandomBuffID()
		{
			if (this.BossBuffRoundList == null || this.BossBuffRoundList.Count == 0)
			{
				this.BossBuffRoundList = new List<int>(this.BossBuffRound);
				this.BossBuffGroupList = new List<int>(this.BossBuffGroup);
			}
			int result = 0;
			int randomNumber = Global.GetRandomNumber(0, this.BossBuffRoundList.Sum());
			int num = 0;
			for (int i = 0; i < this.BossBuffRoundList.Count; i++)
			{
				num += this.BossBuffRound[i];
				if (randomNumber < num)
				{
					result = this.BossBuffGroupList[i];
					this.BossBuffRoundList.RemoveAt(i);
					this.BossBuffGroupList.RemoveAt(i);
					break;
				}
			}
			return result;
		}

		public ZorkBattleMonsterConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleMonsterConfig;
		}

		public int ID;

		public int GroupID;

		public ZorkBattleArmyType ArmyType;

		public int MonsterId;

		public int MonsterNum;

		public int MonsterDropBuffId;

		public int BuffEffictTime;

		public int RewardIntegral;

		public double BossBlood;

		public int BuffRefreshTime;

		public int[] BossBuffGroup;

		public int[] BossBuffRound;

		public AwardsItemList BossKillAwardsItemList = new AwardsItemList();

		public List<int> BossBuffRoundList;

		public List<int> BossBuffGroupList;
	}
}
