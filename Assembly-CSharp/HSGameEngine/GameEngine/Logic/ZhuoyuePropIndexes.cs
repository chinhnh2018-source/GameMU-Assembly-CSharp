using System;

namespace HSGameEngine.GameEngine.Logic
{
	public static class ZhuoyuePropIndexes
	{
		public const int FatalAttack = 0;

		public const int MaxAttack = 1;

		public const int MaxMAttack = 2;

		public const int IncreasePhyAttack = 3;

		public const int IncreaseMagAttack = 4;

		public const int AttackSpeed = 5;

		public const int LifeRecoverKillMonster = 6;

		public const int MagicRecoverKillMonster = 7;

		public const int MaxLifePercent = 8;

		public const int MaxMagicPercent = 9;

		public const int DamageDecrease = 10;

		public const int DamageThornPercent = 11;

		public const int Dodge = 12;

		public const int DropMoneyKillMonster = 13;

		public const int IgnoreDefensePercent = 14;

		public const int LifeRecoverPercent = 15;

		public const int MagicRecoverPercent = 16;

		public const int LuckyAttackPercent = 17;

		public const int FatalAttackPercent = 18;

		public const int HitVPercent = 19;

		public const int MaxLifeUpPercent = 20;

		public const int AddAttackInjurePercent = 21;

		public const int SubAttackInjurePercent = 22;

		public const int DoubleAttackPercent = 23;

		public const int MaxAttackPercent = 24;

		public const int DodgePercent = 25;

		public const int IgnoreDefensePercentNew = 26;

		public const int DefensePercent = 27;

		public const int DamageThornPercentNew = 28;

		public const int CounteractLuckyAttack = 29;

		public const int CounteractFatalAttack = 30;

		public const int CounteractDoubleAttack = 31;

		public static readonly string[] ZhuoyuePropIndexChineseNames = new string[]
		{
			Global.GetLang("卓越一击几率"),
			Global.GetLang("攻击力"),
			Global.GetLang("攻击力"),
			Global.GetLang("攻击力提升"),
			Global.GetLang("攻击力提升"),
			Global.GetLang("伤害加成"),
			Global.GetLang("命中值"),
			Global.GetLang("无视防御比例"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("防御力"),
			Global.GetLang("伤害减少"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("闪避值"),
			Global.GetLang("防御力"),
			Global.GetLang("无视防御几率"),
			Global.GetLang("生命完全恢复几率"),
			Global.GetLang("魔法完全恢复几率"),
			Global.GetLang("幸运一击几率"),
			Global.GetLang("卓越一击几率"),
			Global.GetLang("命中值"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("伤害加成"),
			Global.GetLang("伤害减少"),
			Global.GetLang("致命一击几率"),
			Global.GetLang("攻击力"),
			Global.GetLang("闪避值"),
			Global.GetLang("无视防御几率"),
			Global.GetLang("防御力"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("抵抗幸运一击率"),
			Global.GetLang("抵抗卓越一击率"),
			Global.GetLang("抵抗致命一击率")
		};

		public static readonly int[] ZhuoyuePropIndexPercents = new int[]
		{
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			1
		};

		public static readonly int[] ZhuoyuePropIndexValues = new int[]
		{
			2,
			20,
			20,
			2,
			2,
			2,
			2,
			2,
			2,
			20,
			2,
			2,
			2,
			2,
			3,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5,
			5
		};
	}
}
