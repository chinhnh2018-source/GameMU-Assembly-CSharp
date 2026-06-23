using System;
using System.Collections.Generic;
using System.Linq;

namespace GameServer.Logic
{
	public class ZorkBattleArmyConfig
	{
		public int RandomGroupID()
		{
			if (this.ArmyGroupRoundList == null || this.ArmyGroupRoundList.Count == 0)
			{
				this.ArmyGroupRoundList = new List<int>(this.ArmyGroupRound);
				this.GuardGroupIDList = new List<int>(this.GuardGroupID);
			}
			int result = 0;
			int randomNumber = Global.GetRandomNumber(0, this.ArmyGroupRoundList.Sum());
			int num = 0;
			for (int i = 0; i < this.ArmyGroupRoundList.Count; i++)
			{
				num += this.ArmyGroupRoundList[i];
				if (randomNumber < num)
				{
					result = this.GuardGroupIDList[i];
					this.ArmyGroupRoundList.RemoveAt(i);
					this.GuardGroupIDList.RemoveAt(i);
					break;
				}
			}
			return result;
		}

		public ZorkBattleArmyConfig Clone()
		{
			return base.MemberwiseClone() as ZorkBattleArmyConfig;
		}

		public int ID;

		public int PosX;

		public int PosY;

		public int PursuitRadius;

		public int Range;

		public ZorkBattleArmyType ArmyType;

		public int[] ArmyGroupRound;

		public int[] GuardGroupID;

		public int FirstArmyTime;

		public int NextArmyRefresTime;

		public List<int> ArmyGroupRoundList;

		public List<int> GuardGroupIDList;

		public int MonsterDeadNum;
	}
}
