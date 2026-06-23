using System;

namespace HSGameEngine.GameEngine.Logic
{
	public static class ExtPropIndexes
	{
		public static int GetPropIndex(string propName)
		{
			return Array.FindIndex<string>(ExtPropIndexes.ExtPropIndexNames, (string x) => 0 == string.Compare(x, propName, true));
		}

		public const int Strong = 0;

		public const int AttackSpeed = 1;

		public const int MoveSpeed = 2;

		public const int MinDefense = 3;

		public const int MaxDefense = 4;

		public const int MinMDefense = 5;

		public const int MaxMDefense = 6;

		public const int MinAttack = 7;

		public const int MaxAttack = 8;

		public const int MinMAttack = 9;

		public const int MaxMAttack = 10;

		public const int IncreasePhyAttack = 11;

		public const int IncreaseMagAttack = 12;

		public const int MaxLifeV = 13;

		public const int MaxLifePercent = 14;

		public const int MaxMagicV = 15;

		public const int MaxMagicPercent = 16;

		public const int Lucky = 17;

		public const int HitV = 18;

		public const int Dodge = 19;

		public const int LifeRecoverPercent = 20;

		public const int MagicRecoverPercent = 21;

		public const int LifeRecover = 22;

		public const int MagicRecover = 23;

		public const int SubAttackInjurePercent = 24;

		public const int SubAttackInjure = 25;

		public const int AddAttackInjurePercent = 26;

		public const int AddAttackInjure = 27;

		public const int IgnoreDefensePercent = 28;

		public const int DamageThornPercent = 29;

		public const int DamageThorn = 30;

		public const int PhySkillIncreasePercent = 31;

		public const int PhySkillIncrease = 32;

		public const int MagicSkillIncreasePercent = 33;

		public const int MagicSkillIncrease = 34;

		public const int FatalAttack = 35;

		public const int DoubleAttack = 36;

		public const int DecreaseInjurePercent = 37;

		public const int DecreaseInjureValue = 38;

		public const int CounteractInjurePercent = 39;

		public const int CounteractInjureValue = 40;

		public const int IgnoreDefenseRate = 41;

		public const int IncreasePhyDefense = 42;

		public const int IncreaseMagDefense = 43;

		public const int LifeSteal = 44;

		public const int AddAttack = 45;

		public const int AddDefense = 46;

		public const int StateDingShen = 47;

		public const int StateMoveSpeed = 48;

		public const int StateJiTui = 49;

		public const int StateHunMi = 50;

		public const int DeLucky = 51;

		public const int DeFatalAttack = 52;

		public const int DeDoubleAttack = 53;

		public const int HitPercent = 54;

		public const int DodgePercent = 55;

		public const int FrozenPercent = 56;

		public const int PalsyPercent = 57;

		public const int SpeedDownPercent = 58;

		public const int BlowPercent = 59;

		public const int AutoRevivePercent = 60;

		public const int SavagePercent = 61;

		public const int ColdPercent = 62;

		public const int RuthlessPercent = 63;

		public const int DeSavagePercent = 64;

		public const int DeColdPercent = 65;

		public const int DeRuthlessPercent = 66;

		public const int LifeStealPercent = 67;

		public const int Potion = 68;

		public const int FireAttack = 69;

		public const int WaterAttack = 70;

		public const int LightningAttack = 71;

		public const int SoilAttack = 72;

		public const int IceAttack = 73;

		public const int WindAttack = 74;

		public const int FirePenetration = 75;

		public const int WaterPenetration = 76;

		public const int LightningPenetration = 77;

		public const int SoilPenetration = 78;

		public const int IcePenetration = 79;

		public const int WindPenetration = 80;

		public const int DeFirePenetration = 81;

		public const int DeWaterPenetration = 82;

		public const int DeLightningPenetration = 83;

		public const int DeSoilPenetration = 84;

		public const int DeIcePenetration = 85;

		public const int DeWindPenetration = 86;

		public const int Holywater = 87;

		public const int RecoverLifeV = 88;

		public const int RecoverMagicV = 89;

		public const int Fatalhurt = 90;

		public const int AddAttackPercent = 91;

		public const int AddDefensePercent = 92;

		public const int InjurePenetrationPercent = 93;

		public const int ElementInjurePercent = 94;

		public const int IgnorePhyAttackPercent = 95;

		public const int IgnoreMagyAttackPercent = 96;

		public const int DeFrozenPercent = 97;

		public const int DePalsyPercent = 98;

		public const int DeSpeedDownPercent = 99;

		public const int DeBlowPercent = 100;

		public const int Toughness = 101;

		public const int SPAttackInjurePercent = 102;

		public const int AttackInjurePercent = 103;

		public const int ElementAttackInjurePercent = 104;

		public const int WeaponEffect = 105;

		public const int FireEnhance = 106;

		public const int WaterEnhance = 107;

		public const int LightningEnhance = 108;

		public const int SoilEnhance = 109;

		public const int IceEnhance = 110;

		public const int WindEnhance = 111;

		public const int FireReduce = 112;

		public const int WaterReduce = 113;

		public const int LightningReduce = 114;

		public const int SoilReduce = 115;

		public const int IceReduce = 116;

		public const int WindReduce = 117;

		public const int ElementPenetration = 118;

		public const int ArmorUp = 119;

		public const int Absorb = 120;

		public const int Reply = 121;

		public const int HolyAttack = 122;

		public const int HolyDefense = 123;

		public const int HolyPenetratePercent = 124;

		public const int HolyAbsorbPercent = 125;

		public const int HolyWeakPercent = 126;

		public const int HolyDoubleAttackPercent = 127;

		public const int HolyDoubleAttackInjure = 128;

		public const int ShadowAttack = 129;

		public const int ShadowDefense = 130;

		public const int ShadowPenetratePercent = 131;

		public const int ShadowAbsorbPercent = 132;

		public const int ShadowWeakPercent = 133;

		public const int ShadowDoubleAttackPercent = 134;

		public const int ShadowDoubleAttackInjure = 135;

		public const int NatureAttack = 136;

		public const int NatureDefense = 137;

		public const int NaturePenetratePercent = 138;

		public const int NatureAbsorbPercent = 139;

		public const int NatureWeakPercent = 140;

		public const int NatureDoubleAttackPercent = 141;

		public const int NatureDoubleAttackInjure = 142;

		public const int ChaosAttack = 143;

		public const int ChaosDefense = 144;

		public const int ChaosPenetratePercent = 145;

		public const int ChaosAbsorbPercent = 146;

		public const int ChaosWeakPercent = 147;

		public const int ChaosDoubleAttackPercent = 148;

		public const int ChaosDoubleAttackInjure = 149;

		public const int IncubusAttack = 150;

		public const int IncubusDefense = 151;

		public const int IncubusPenetratePercent = 152;

		public const int IncubusAbsorbPercent = 153;

		public const int IncubusWeakPercent = 154;

		public const int IncubusDoubleAttackPercent = 155;

		public const int IncubusDoubleAttackInjure = 156;

		public const int RebronAttack = 157;

		public const int RebronDefense = 158;

		public const int RebronPenetratePercent = 159;

		public const int RebronAbsorbPercent = 160;

		public const int RebronWeakPercent = 161;

		public const int RebronDoubleAttackPercent = 162;

		public const int RebronDoubleAttackInjure = 163;

		public const int IgnoreDamageThornPercent = 164;

		public const int HolyRebornDoubleAttackResistance = 165;

		public const int HolyRebornDoubleAttackInjureResistance = 166;

		public const int ShadowRebornDoubleAttackResistance = 167;

		public const int ShadowRebornDoubleAttackInjureResistance = 168;

		public const int NatureRebornDoubleAttackResistance = 169;

		public const int NatureRebornDoubleAttackInjureResistance = 170;

		public const int ChaosRebornDoubleAttackResistance = 171;

		public const int ChaosRebornDoubleAttackInjureResistance = 172;

		public const int IncubusRebornDoubleAttackResistance = 173;

		public const int IncubusRebornDoubleAttackInjureResistance = 174;

		public const int RebornDoubleAttackResistance = 175;

		public const int RebornDoubleAttackInjureResistance = 176;

		public const int Max = 177;

		public const int Weight = 0;

		public const int MinDSAttack = 9;

		public const int MaxDSAttack = 10;

		public const int Curse = 255;

		public const int MagicDodgePercent = 255;

		public const int PoisoningReoverPercent = 255;

		public const int PoisoningDodge = 255;

		public const int SubMAttackInjurePercent = 255;

		public const int IgnoreMDefensePercent = 255;

		public static readonly string[] ExtPropIndexNames = new string[]
		{
			"strong",
			"accackspeed",
			"speed",
			"defense",
			"maxdefense",
			"mdefense",
			"maxmdefense",
			"attack",
			"maxattack",
			"mattack",
			"maxmattack",
			"increasephyattack",
			"increasemagattack",
			"maxlifev",
			"maxlifepercent",
			"maxmagicv",
			"maxmagicpercent",
			"lucky",
			"hitv",
			"dodge",
			"liferecoverpercent",
			"magicrecoverpercent",
			"liferecover",
			"magicrecover",
			"subattackinjurepercent",
			"subattackinjure",
			"addattackinjurepercent",
			"addattackinjure",
			"ignoredefensepercent",
			"damagethornpercent",
			"damagethorn",
			"physkillincreasepercent",
			"physkillincrease",
			"magicskillincreasepercent",
			"magicskillincrease",
			"fatalattack",
			"doubleattack",
			"decreaseinjurepercent",
			"decreaseinjurevalue",
			"counteractinjurepercent",
			"counteractinjurevalue",
			"ignoredefenserate",
			"increasephydefense",
			"increasemagdefense",
			"lifesteal",
			"addattack",
			"adddefense",
			"hitpercent",
			"dodgepercent",
			"savagepercent",
			"coldpercent",
			"ruthlesspercent",
			"desavagepercent",
			"decoldpercent",
			"deruthlesspercent",
			"lifestealpercent",
			"potion",
			"fireattack",
			"waterattack",
			"lightningattack",
			"soilattack",
			"iceattack",
			"windattack",
			"firepenetration",
			"waterpenetration",
			"lightningpenetration",
			"soilpenetration",
			"icepenetration",
			"windpenetration",
			"defirepenetration",
			"dewaterpenetration",
			"delightningpenetration",
			"desoilpenetration",
			"deicepenetration",
			"dewindpenetration",
			"holywater",
			"recoverlifev",
			"recovermagicv",
			"fatalhurt",
			"addattackpercent",
			"adddefensepercent",
			"injurepenetrationpercent",
			"elementinjurepercent",
			"ignorephyattackpercent",
			"ignoremagyattackpercent"
		};

		public static readonly string[] ChineseNames = new string[]
		{
			Global.GetLang("耐久"),
			Global.GetLang("攻击速度"),
			Global.GetLang("移动速度"),
			Global.GetLang("最小物防"),
			Global.GetLang("最大物防"),
			Global.GetLang("最小魔防"),
			Global.GetLang("最大魔防"),
			Global.GetLang("最小物攻"),
			Global.GetLang("最大物攻"),
			Global.GetLang("最小魔攻"),
			Global.GetLang("最大魔攻"),
			Global.GetLang("物理攻击提升"),
			Global.GetLang("魔法攻击提升"),
			Global.GetLang("生命上限"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("魔法上限"),
			Global.GetLang("魔法上限加成"),
			Global.GetLang("幸       运"),
			Global.GetLang("命       中"),
			Global.GetLang("闪       避"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害加成"),
			Global.GetLang("附加伤害"),
			Global.GetLang("无视防御概率"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("物理技能增幅"),
			Global.GetLang("物理技能增幅"),
			Global.GetLang("魔法技能增幅"),
			Global.GetLang("魔法技能增幅"),
			Global.GetLang("卓越一击概率"),
			Global.GetLang("双倍一击概率"),
			Global.GetLang("伤害减少"),
			Global.GetLang("抵挡伤害"),
			Global.GetLang("伤害抵挡"),
			Global.GetLang("伤害抵挡"),
			Global.GetLang("无视防御比例"),
			Global.GetLang("物理防御提升"),
			Global.GetLang("魔法防御提升"),
			Global.GetLang("击中恢复"),
			Global.GetLang("攻击力\u3000"),
			Global.GetLang("防御力\u3000"),
			Global.GetLang("定身状态加成"),
			Global.GetLang("速度改变状态"),
			Global.GetLang("击退状态"),
			Global.GetLang("昏迷状态"),
			Global.GetLang("抵抗幸运一击"),
			Global.GetLang("抵抗卓越一击"),
			Global.GetLang("抵抗双倍一击"),
			Global.GetLang("增加命中百分比"),
			Global.GetLang("增加闪避百分比"),
			Global.GetLang("冰冻几率"),
			Global.GetLang("麻痹几率"),
			Global.GetLang("减速几率"),
			Global.GetLang("重击几率"),
			Global.GetLang("自动重生几率"),
			Global.GetLang("野蛮一击"),
			Global.GetLang("冷血一击"),
			Global.GetLang("无情一击"),
			Global.GetLang("抵抗野蛮一击"),
			Global.GetLang("抵抗冷血一击"),
			Global.GetLang("抵抗无情一击"),
			Global.GetLang("击中恢复百分比"),
			Global.GetLang("药水效果"),
			Global.GetLang("火系伤害"),
			Global.GetLang("水系伤害"),
			Global.GetLang("雷系伤害"),
			Global.GetLang("土系伤害"),
			Global.GetLang("冰系伤害"),
			Global.GetLang("风系伤害"),
			Global.GetLang("火系伤害穿透"),
			Global.GetLang("水系伤害穿透"),
			Global.GetLang("雷系伤害穿透"),
			Global.GetLang("土系伤害穿透"),
			Global.GetLang("冰系伤害穿透"),
			Global.GetLang("风系伤害穿透"),
			Global.GetLang("火系抵抗"),
			Global.GetLang("水系抵抗"),
			Global.GetLang("雷系抵抗"),
			Global.GetLang("土系抵抗"),
			Global.GetLang("冰系抵抗"),
			Global.GetLang("风系抵抗"),
			Global.GetLang("圣水效果"),
			Global.GetLang("自动恢复生命效果"),
			Global.GetLang("自动恢复魔法效果"),
			Global.GetLang("卓越伤害提升"),
			Global.GetLang("攻击力提升"),
			Global.GetLang("防御力提升"),
			Global.GetLang("伤害穿透"),
			Global.GetLang("元素伤害加成"),
			Global.GetLang("物理免疫几率"),
			Global.GetLang("魔法免疫几率")
		};

		public static readonly string[] ExtPropIndexChineseNames = new string[]
		{
			Global.GetLang("耐久"),
			Global.GetLang("攻击速度"),
			Global.GetLang("移动速度"),
			Global.GetLang("物理防御"),
			Global.GetLang("最大物防"),
			Global.GetLang("魔法防御"),
			Global.GetLang("最大魔防"),
			Global.GetLang("物理攻击"),
			Global.GetLang("最大物攻"),
			Global.GetLang("魔法攻击"),
			Global.GetLang("最大魔攻"),
			Global.GetLang("物理攻击提升"),
			Global.GetLang("魔法攻击提升"),
			Global.GetLang("生命上限"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("魔法上限"),
			Global.GetLang("魔法上限加成"),
			Global.GetLang("幸        运"),
			Global.GetLang("命        中"),
			Global.GetLang("闪        避"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害加成"),
			Global.GetLang("附加伤害"),
			Global.GetLang("无视防御概率"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("物理技能增幅"),
			Global.GetLang("物理技能增幅"),
			Global.GetLang("魔法技能增幅"),
			Global.GetLang("魔法技能增幅"),
			Global.GetLang("卓越一击概率"),
			Global.GetLang("双倍一击概率"),
			Global.GetLang("伤害减少"),
			Global.GetLang("抵挡伤害"),
			Global.GetLang("伤害抵挡"),
			Global.GetLang("伤害抵挡"),
			Global.GetLang("无视防御比例"),
			Global.GetLang("物理防御提升"),
			Global.GetLang("魔法防御提升"),
			Global.GetLang("击中恢复"),
			Global.GetLang("攻击力\u3000"),
			Global.GetLang("防御力\u3000"),
			Global.GetLang("定身状态加成"),
			Global.GetLang("速度改变状态"),
			Global.GetLang("击退状态"),
			Global.GetLang("昏迷状态"),
			Global.GetLang("抵抗幸运一击"),
			Global.GetLang("抵抗卓越一击"),
			Global.GetLang("抵抗双倍一击"),
			Global.GetLang("增加命中百分比"),
			Global.GetLang("增加闪避百分比"),
			Global.GetLang("冰冻几率"),
			Global.GetLang("麻痹几率"),
			Global.GetLang("减速几率"),
			Global.GetLang("重击几率"),
			Global.GetLang("自动重生几率"),
			Global.GetLang("野蛮一击"),
			Global.GetLang("冷血一击"),
			Global.GetLang("无情一击"),
			Global.GetLang("抵抗野蛮一击"),
			Global.GetLang("抵抗冷血一击"),
			Global.GetLang("抵抗无情一击"),
			Global.GetLang("击中恢复百分比"),
			Global.GetLang("药水效果"),
			Global.GetLang("火系伤害"),
			Global.GetLang("水系伤害"),
			Global.GetLang("雷系伤害"),
			Global.GetLang("土系伤害"),
			Global.GetLang("冰系伤害"),
			Global.GetLang("风系伤害"),
			Global.GetLang("火系伤害穿透"),
			Global.GetLang("水系伤害穿透"),
			Global.GetLang("雷系伤害穿透"),
			Global.GetLang("土系伤害穿透"),
			Global.GetLang("冰系伤害穿透"),
			Global.GetLang("风系伤害穿透"),
			Global.GetLang("火系抵抗"),
			Global.GetLang("水系抵抗"),
			Global.GetLang("雷系抵抗"),
			Global.GetLang("土系抵抗"),
			Global.GetLang("冰系抵抗"),
			Global.GetLang("风系抵抗"),
			Global.GetLang("圣水效果"),
			Global.GetLang("自动恢复生命效果"),
			Global.GetLang("自动恢复魔法效果"),
			Global.GetLang("卓越伤害提升"),
			Global.GetLang("攻击力提升"),
			Global.GetLang("防御力提升"),
			Global.GetLang("伤害穿透"),
			Global.GetLang("元素伤害加成"),
			Global.GetLang("物理免疫几率"),
			Global.GetLang("魔法免疫几率"),
			Global.GetLang("抵抗冰冻几率"),
			Global.GetLang("抵抗麻痹几率"),
			Global.GetLang("抵抗减速几率"),
			Global.GetLang("抵抗重击几率"),
			Global.GetLang("韧性"),
			Global.GetLang("特殊效果伤害减免"),
			Global.GetLang("伤害减免"),
			Global.GetLang("元素伤害减免"),
			Global.GetLang("武器大师属性"),
			Global.GetLang("火系效果增强"),
			Global.GetLang("水系效果增强"),
			Global.GetLang("雷系效果增强"),
			Global.GetLang("土系效果增强"),
			Global.GetLang("冰系效果增强"),
			Global.GetLang("风系效果增强"),
			Global.GetLang("火伤效果减免"),
			Global.GetLang("水伤效果减免"),
			Global.GetLang("雷伤效果减免"),
			Global.GetLang("土伤效果减免"),
			Global.GetLang("冰伤效果减免"),
			Global.GetLang("风伤效果减免"),
			Global.GetLang("元素穿透"),
			Global.GetLang("ArmorUp"),
			Global.GetLang("吸收"),
			Global.GetLang("反弹"),
			Global.GetLang("神圣攻击"),
			Global.GetLang("神圣防御"),
			Global.GetLang("神圣伤害穿刺"),
			Global.GetLang("神圣伤害吸收"),
			Global.GetLang("神圣易伤"),
			Global.GetLang("神圣暴击率"),
			Global.GetLang("神圣暴击伤害"),
			Global.GetLang("暗影攻击"),
			Global.GetLang("暗影防御"),
			Global.GetLang("暗影伤害穿刺"),
			Global.GetLang("暗影伤害吸收"),
			Global.GetLang("暗影易伤"),
			Global.GetLang("暗影暴击率"),
			Global.GetLang("暗影暴击伤害"),
			Global.GetLang("自然攻击"),
			Global.GetLang("自然防御"),
			Global.GetLang("自然伤害穿刺"),
			Global.GetLang("自然伤害吸收"),
			Global.GetLang("自然易伤"),
			Global.GetLang("自然暴击率"),
			Global.GetLang("自然暴击伤害"),
			Global.GetLang("混沌攻击"),
			Global.GetLang("混沌防御"),
			Global.GetLang("混沌伤害穿刺"),
			Global.GetLang("混沌伤害吸收"),
			Global.GetLang("混沌易伤"),
			Global.GetLang("混沌暴击率"),
			Global.GetLang("混沌暴击伤害"),
			Global.GetLang("梦魇攻击"),
			Global.GetLang("梦魇防御"),
			Global.GetLang("梦魇伤害穿刺"),
			Global.GetLang("梦魇伤害吸收"),
			Global.GetLang("梦魇易伤"),
			Global.GetLang("梦魇暴击率"),
			Global.GetLang("梦魇暴击伤害"),
			Global.GetLang("全系重生攻击"),
			Global.GetLang("全系重生防御"),
			Global.GetLang("全系重生伤害穿刺"),
			Global.GetLang("全系重生伤害吸收"),
			Global.GetLang("全系重生易伤"),
			Global.GetLang("全系重生暴击率"),
			Global.GetLang("全系重生暴击伤害")
		};

		public static readonly int[] ExtPropIndexPercents = new int[]
		{
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			0,
			1,
			0,
			1,
			0,
			0,
			0,
			1,
			1,
			0,
			0,
			1,
			0,
			1,
			0,
			1,
			1,
			0,
			1,
			0,
			1,
			0,
			1,
			1,
			1,
			0,
			1,
			0,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			0,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
			0,
			0,
			0,
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
			0,
			0,
			0,
			0,
			1,
			1,
			0,
			0,
			0,
			0,
			1,
			1,
			1,
			1,
			0,
			0,
			0,
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
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1,
			0,
			0,
			1,
			1,
			1,
			1,
			1
		};

		public static int[] ExtPropIndexShows = new int[]
		{
			1,
			2,
			3,
			7,
			11,
			13,
			14,
			15,
			16
		};

		public static readonly string[] ExtPropIndexBuffNames = new string[]
		{
			Global.GetLang("耐久"),
			Global.GetLang("攻击速度提升"),
			Global.GetLang("移动速度提升"),
			Global.GetLang("防御提升"),
			Global.GetLang("最大物防"),
			Global.GetLang("魔法防御提升"),
			Global.GetLang("最大魔防"),
			Global.GetLang("攻击提升"),
			Global.GetLang("最大物攻"),
			Global.GetLang("魔法攻击提升"),
			Global.GetLang("最大魔攻"),
			Global.GetLang("攻击提升"),
			Global.GetLang("魔法攻击提升"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("魔法上限加成"),
			Global.GetLang("魔法上限加成"),
			Global.GetLang("幸运提升"),
			Global.GetLang("命中提升"),
			Global.GetLang("闪避提升"),
			Global.GetLang("生命恢复提升"),
			Global.GetLang("魔法恢复提升"),
			Global.GetLang("生命恢复提升"),
			Global.GetLang("魔法恢复提升"),
			Global.GetLang("伤害吸收提升"),
			Global.GetLang("伤害吸收提升"),
			Global.GetLang("伤害加成提升"),
			Global.GetLang("伤害加成提升"),
			Global.GetLang("无视防御概率提升"),
			Global.GetLang("伤害反弹提升"),
			Global.GetLang("伤害反弹提升"),
			Global.GetLang("物理技能增幅提升"),
			Global.GetLang("物理技能增幅提升"),
			Global.GetLang("魔法技能增幅提升"),
			Global.GetLang("魔法技能增幅提升"),
			Global.GetLang("卓越一击概率提升"),
			Global.GetLang("双倍一击概率提升"),
			Global.GetLang("伤害减少提升"),
			Global.GetLang("伤害减少提升"),
			Global.GetLang("伤害抵挡提升"),
			Global.GetLang("伤害抵挡提升"),
			Global.GetLang("无视防御比例提升"),
			Global.GetLang("物理防御提升"),
			Global.GetLang("魔法防御提升")
		};

		public static readonly string[] ShengWuIndexNames = new string[]
		{
			"Constitution",
			"Dexterity",
			"Intelligence",
			"Strength",
			"Strong",
			"AttackSpeed",
			"MoveSpeed",
			"MinDefense",
			"MaxDefense",
			"MinMDefense",
			"MaxMDefense",
			"MinAttack",
			"MaxAttack",
			"MinMAttack",
			"MaxMAttack",
			"IncreasePhyAttack",
			"IncreaseMagAttack",
			"MaxLifeV",
			"MaxLifePercent",
			"MaxMagicV",
			"MaxMagicPercent",
			"Lucky",
			"HitV",
			"Dodge",
			"LifeRecoverPercent",
			"MagicRecoverPercent",
			"LifeRecover",
			"MagicRecover",
			"SubAttackInjurePercent",
			"SubAttackInjure",
			"AddAttackInjurePercent",
			"AddAttackInjure",
			"IgnoreDefensePercent",
			"DamageThornPercent",
			"DamageThorn",
			"PhySkillIncreasePercent",
			"PhySkillIncrease",
			"MagicSkillIncreasePercent",
			"MagicSkillIncrease",
			"FatalAttack",
			"DoubleAttack",
			"DecreaseInjurePercent",
			"DecreaseInjureValue",
			"CounteractInjurePercent",
			"CounteractInjureValue",
			"IgnoreDefenseRate",
			"IncreasePhyDefense",
			"IncreaseMagDefense",
			"LifeSteal",
			"AddAttack",
			"AddDefense",
			"StateDingShen",
			"StateMoveSpeed",
			"StateJiTui",
			"StateHunMi",
			"DeLucky",
			"DeFatalAttack",
			"DeDoubleAttack",
			"HitPercent",
			"DodgePercent",
			"FrozenPercent",
			"PalsyPercent",
			"SpeedDownPercent",
			"BlowPercent",
			"AutoRevivePercent",
			"SavagePercent",
			"ColdPercent",
			"RuthlessPercent",
			"DeSavagePercent",
			"DeColdPercent",
			"DeRuthlessPercent",
			"LifeStealPercent",
			"Potion",
			"FireAttack",
			"WaterAttack",
			"LightningAttack",
			"SoilAttack",
			"IceAttack",
			"WindAttack",
			"FirePenetration",
			"WaterPenetration",
			"LightningPenetration",
			"SoilPenetration",
			"IcePenetration",
			"WindPenetration",
			"DeFirePenetration",
			"DeWaterPenetration",
			"DeLightningPenetration",
			"DeSoilPenetration",
			"DeIcePenetration",
			"DeWindPenetration",
			"HolyWater",
			"RecoverLifeV",
			"RecoverMagicV",
			"FatalHurt"
		};

		public static readonly string[] ShengWuChineseNames = new string[]
		{
			Global.GetLang("体        力"),
			Global.GetLang("敏        捷"),
			Global.GetLang("智        力"),
			Global.GetLang("力        量"),
			Global.GetLang("耐        久"),
			Global.GetLang("攻击速度"),
			Global.GetLang("移动速度"),
			Global.GetLang("最小物防"),
			Global.GetLang("最大物防"),
			Global.GetLang("最小魔防"),
			Global.GetLang("最大魔防"),
			Global.GetLang("最小物攻"),
			Global.GetLang("最大物攻"),
			Global.GetLang("最小魔攻"),
			Global.GetLang("最大魔攻"),
			Global.GetLang("物理攻击"),
			Global.GetLang("魔法攻击"),
			Global.GetLang("生命上限"),
			Global.GetLang("生命上限加成"),
			Global.GetLang("魔法上限"),
			Global.GetLang("魔法上限"),
			Global.GetLang("幸        运"),
			Global.GetLang("命        中"),
			Global.GetLang("闪        避"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("生命恢复"),
			Global.GetLang("魔法恢复"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害吸收"),
			Global.GetLang("伤害加成"),
			Global.GetLang("附加伤害"),
			Global.GetLang("无视防御几率"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("伤害反弹"),
			Global.GetLang("物理技能伤害"),
			Global.GetLang("物理技能伤害"),
			Global.GetLang("魔法技能伤害"),
			Global.GetLang("魔法技能伤害"),
			Global.GetLang("卓越一击几率"),
			Global.GetLang("致命一击几率"),
			Global.GetLang("伤害减少"),
			Global.GetLang("伤害减少"),
			Global.GetLang("抵挡伤害"),
			Global.GetLang("抵挡伤害"),
			Global.GetLang("无视防御的比例"),
			Global.GetLang("物理防御"),
			Global.GetLang("魔法防御"),
			Global.GetLang("击中恢复"),
			Global.GetLang("攻  击  力"),
			Global.GetLang("防  御  力"),
			Global.GetLang("定身状态"),
			Global.GetLang("速度改变"),
			Global.GetLang("击退状态"),
			Global.GetLang("昏迷状态"),
			Global.GetLang("抵抗幸运一击"),
			Global.GetLang("抵抗卓越一击"),
			Global.GetLang("抵抗双倍一击"),
			Global.GetLang("命中提升"),
			Global.GetLang("闪避提升"),
			Global.GetLang("冰冻几率"),
			Global.GetLang("麻痹几率"),
			Global.GetLang("减速几率"),
			Global.GetLang("重击几率"),
			Global.GetLang("自动重生几率"),
			Global.GetLang("野蛮一击几率"),
			Global.GetLang("冷血一击几率"),
			Global.GetLang("无情一击几率"),
			Global.GetLang("抵抗野蛮几率"),
			Global.GetLang("抵抗冷血几率"),
			Global.GetLang("抵抗无情几率"),
			Global.GetLang("击中恢复"),
			Global.GetLang("药水效果"),
			Global.GetLang("火系伤害"),
			Global.GetLang("水系伤害"),
			Global.GetLang("雷系伤害"),
			Global.GetLang("土系伤害"),
			Global.GetLang("冰系伤害"),
			Global.GetLang("风系伤害"),
			Global.GetLang("火伤穿透"),
			Global.GetLang("水伤穿透"),
			Global.GetLang("雷伤穿透"),
			Global.GetLang("土伤穿透"),
			Global.GetLang("冰伤穿透"),
			Global.GetLang("风伤穿透"),
			Global.GetLang("火系抗性"),
			Global.GetLang("水系抗性"),
			Global.GetLang("雷系抗性"),
			Global.GetLang("土系抗性"),
			Global.GetLang("冰系抗性"),
			Global.GetLang("风系抗性"),
			Global.GetLang("圣水效果"),
			Global.GetLang("自动恢复生命效果"),
			Global.GetLang("自动恢复魔法效果"),
			Global.GetLang("卓越伤害提升")
		};
	}
}
