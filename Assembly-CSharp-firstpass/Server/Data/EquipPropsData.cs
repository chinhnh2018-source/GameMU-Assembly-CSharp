using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EquipPropsData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public double Strength;

		[ProtoMember(3)]
		public double Intelligence;

		[ProtoMember(4)]
		public double Dexterity;

		[ProtoMember(5)]
		public double Constitution;

		[ProtoMember(6)]
		public double MinAttack;

		[ProtoMember(7)]
		public double MaxAttack;

		[ProtoMember(8)]
		public double MinDefense;

		[ProtoMember(9)]
		public double MaxDefense;

		[ProtoMember(10)]
		public double MagicSkillIncrease;

		[ProtoMember(11)]
		public double MinMAttack;

		[ProtoMember(12)]
		public double MaxMAttack;

		[ProtoMember(13)]
		public double MinMDefense;

		[ProtoMember(14)]
		public double MaxMDefense;

		[ProtoMember(15)]
		public double PhySkillIncrease;

		[ProtoMember(16)]
		public double MaxHP;

		[ProtoMember(17)]
		public double MaxMP;

		[ProtoMember(18)]
		public double AttackSpeed;

		[ProtoMember(19)]
		public double Hit;

		[ProtoMember(20)]
		public double Dodge;

		[ProtoMember(21)]
		public int TotalPropPoint;

		[ProtoMember(22)]
		public int ChangeLifeCount;

		[ProtoMember(23)]
		public int CombatForce;

		[ProtoMember(24)]
		public int TEMPStrength;

		[ProtoMember(25)]
		public int TEMPIntelligsence;

		[ProtoMember(26)]
		public int TEMPDexterity;

		[ProtoMember(27)]
		public int TEMPConstitution;

		[ProtoMember(28)]
		public double Toughness;

		[ProtoMember(29)]
		public int ArmorMax;

		[ProtoMember(30)]
		public int RebornCombatForce;
	}
}
