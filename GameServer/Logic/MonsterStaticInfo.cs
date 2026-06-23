using System;

namespace GameServer.Logic
{
	public class MonsterStaticInfo
	{
		public string VSName { get; set; }

		public int ExtensionID { get; set; }

		public int VLevel { get; set; }

		public int VExperience { get; set; }

		public int VMoney { get; set; }

		public double VLifeMax { get; set; }

		public double VManaMax { get; set; }

		public int ToOccupation { get; set; }

		public int[] SpriteSpeedTickList { get; set; }

		public int[] EachActionFrameRange { get; set; }

		public int[] EffectiveFrame { get; set; }

		public int SeekRange { get; set; }

		public int EquipmentBody { get; set; }

		public int EquipmentWeapon { get; set; }

		public int MinAttack { get; set; }

		public int MaxAttack { get; set; }

		public int Defense { get; set; }

		public int MDefense { get; set; }

		public double HitV { get; set; }

		public double Dodge { get; set; }

		public double RecoverLifeV { get; set; }

		public double RecoverMagicV { get; set; }

		public double MonsterDamageThornPercent { get; set; }

		public double MonsterDamageThorn { get; set; }

		public double MonsterSubAttackInjurePercent { get; set; }

		public double MonsterSubAttackInjure { get; set; }

		public double MonsterIgnoreDefensePercent { get; set; }

		public double MonsterIgnoreDefenseRate { get; set; }

		public double MonsterLucky { get; set; }

		public double MonsterFatalAttack { get; set; }

		public double MonsterDoubleAttack { get; set; }

		public int FallGoodsPackID { get; set; }

		public int AttackType { get; set; }

		public int BattlePersonalJiFen { get; set; }

		public int BattleZhenYingJiFen { get; set; }

		public int DaimonSquareJiFen { get; set; }

		public int BloodCastJiFen { get; set; }

		public int WolfScore { get; set; }

		public int FallBelongTo { get; set; }

		public int[] SkillIDs { get; set; }

		public int Camp { get; set; }

		public int AIID { get; set; }

		public int ChangeLifeCount { get; set; }

		public MonsterStaticInfo Clone()
		{
			MonsterStaticInfo monsterStaticInfo = new MonsterStaticInfo();
			monsterStaticInfo.VSName = this.VSName;
			monsterStaticInfo.ExtensionID = this.ExtensionID;
			monsterStaticInfo.VLevel = this.VLevel;
			monsterStaticInfo.VExperience = this.VExperience;
			monsterStaticInfo.RebornExp = this.VExperience;
			monsterStaticInfo.ExtProps = this.ExtProps;
			monsterStaticInfo.VMoney = this.VMoney;
			monsterStaticInfo.VLifeMax = this.VLifeMax;
			monsterStaticInfo.VManaMax = this.VManaMax;
			monsterStaticInfo.ToOccupation = this.ToOccupation;
			if (null != this.SpriteSpeedTickList)
			{
				monsterStaticInfo.SpriteSpeedTickList = new int[this.SpriteSpeedTickList.Length];
				this.SpriteSpeedTickList.CopyTo(monsterStaticInfo.SpriteSpeedTickList, 0);
			}
			if (null != this.EachActionFrameRange)
			{
				monsterStaticInfo.EachActionFrameRange = new int[this.EachActionFrameRange.Length];
				this.EachActionFrameRange.CopyTo(monsterStaticInfo.EachActionFrameRange, 0);
			}
			if (null != this.EffectiveFrame)
			{
				monsterStaticInfo.EffectiveFrame = new int[this.EffectiveFrame.Length];
				this.EffectiveFrame.CopyTo(monsterStaticInfo.EffectiveFrame, 0);
			}
			monsterStaticInfo.SeekRange = this.SeekRange;
			monsterStaticInfo.EquipmentBody = this.EquipmentBody;
			monsterStaticInfo.EquipmentWeapon = this.EquipmentWeapon;
			monsterStaticInfo.MinAttack = this.MinAttack;
			monsterStaticInfo.MaxAttack = this.MaxAttack;
			monsterStaticInfo.Defense = this.Defense;
			monsterStaticInfo.MDefense = this.MDefense;
			monsterStaticInfo.HitV = this.HitV;
			monsterStaticInfo.Dodge = this.Dodge;
			monsterStaticInfo.RecoverLifeV = this.RecoverLifeV;
			monsterStaticInfo.RecoverMagicV = this.RecoverMagicV;
			monsterStaticInfo.MonsterDamageThornPercent = this.MonsterDamageThornPercent;
			monsterStaticInfo.MonsterDamageThorn = this.MonsterDamageThorn;
			monsterStaticInfo.MonsterSubAttackInjurePercent = this.MonsterSubAttackInjurePercent;
			monsterStaticInfo.MonsterSubAttackInjure = this.MonsterSubAttackInjure;
			monsterStaticInfo.MonsterIgnoreDefensePercent = this.MonsterIgnoreDefensePercent;
			monsterStaticInfo.MonsterIgnoreDefenseRate = this.MonsterIgnoreDefenseRate;
			monsterStaticInfo.MonsterLucky = this.MonsterLucky;
			monsterStaticInfo.MonsterFatalAttack = this.MonsterFatalAttack;
			monsterStaticInfo.MonsterDoubleAttack = this.MonsterDoubleAttack;
			monsterStaticInfo.FallGoodsPackID = this.FallGoodsPackID;
			monsterStaticInfo.AttackType = this.AttackType;
			monsterStaticInfo.BattlePersonalJiFen = this.BattlePersonalJiFen;
			monsterStaticInfo.BattleZhenYingJiFen = this.BattleZhenYingJiFen;
			monsterStaticInfo.DaimonSquareJiFen = this.DaimonSquareJiFen;
			monsterStaticInfo.BloodCastJiFen = this.BloodCastJiFen;
			monsterStaticInfo.FallBelongTo = this.FallBelongTo;
			if (null != this.SkillIDs)
			{
				monsterStaticInfo.SkillIDs = new int[this.SkillIDs.Length];
				this.SkillIDs.CopyTo(monsterStaticInfo.SkillIDs, 0);
			}
			monsterStaticInfo.Camp = this.Camp;
			monsterStaticInfo.AIID = this.AIID;
			monsterStaticInfo.ChangeLifeCount = this.ChangeLifeCount;
			if (this.ExtProps != null)
			{
				monsterStaticInfo.ExtProps = new double[177];
				Array.Copy(this.ExtProps, monsterStaticInfo.ExtProps, 177);
			}
			return monsterStaticInfo;
		}

		public int RebornExp;

		public double[] ExtProps;
	}
}
