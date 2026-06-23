using System;
using Server.Data;

namespace HSGameEngine.GameEngine.Logic
{
	public class HorseSkillData
	{
		public HorseSkillData(GoodsData horseData)
		{
			if (horseData == null)
			{
				return;
			}
			HorseAdvancedVO horseAdvancedVOByID = IConfigbase<ConfigRidePet>.Instance.GetHorseAdvancedVOByID(horseData.GoodsID, horseData.Forge_level + 1);
			if (horseAdvancedVOByID != null)
			{
				this.mSkillID = horseAdvancedVOByID.SkillID;
				this.mSkillLevel = horseData.Forge_level + 1;
			}
		}

		public int SkillID
		{
			get
			{
				return this.mSkillID;
			}
		}

		public int SkillLevel
		{
			get
			{
				return this.mSkillLevel;
			}
		}

		private int mSkillID;

		private int mSkillLevel;
	}
}
