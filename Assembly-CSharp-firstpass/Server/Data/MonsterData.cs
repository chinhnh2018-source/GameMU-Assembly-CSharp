using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MonsterData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int RoleSex;

		[ProtoMember(4)]
		public int Level = 1;

		[ProtoMember(5)]
		public int Experience;

		[ProtoMember(6)]
		public int PosX;

		[ProtoMember(7)]
		public int PosY;

		[ProtoMember(8)]
		public int RoleDirection;

		[ProtoMember(9)]
		public double LifeV;

		[ProtoMember(10)]
		public double MaxLifeV;

		[ProtoMember(11)]
		public double MagicV;

		[ProtoMember(12)]
		public double MaxMagicV;

		[ProtoMember(13)]
		public int EquipmentBody;

		[ProtoMember(14)]
		public int ExtensionID;

		[ProtoMember(15)]
		public int MonsterType;

		[ProtoMember(16)]
		public int MasterRoleID;

		[ProtoMember(17)]
		public ushort AiControlType = 1;

		[ProtoMember(18)]
		public string AnimalSound = string.Empty;

		[ProtoMember(19)]
		public int MonsterLevel;

		[ProtoMember(20)]
		public long ZhongDuStart;

		[ProtoMember(21)]
		public int ZhongDuSeconds;

		[ProtoMember(22)]
		public long FaintStart;

		[ProtoMember(23)]
		public int FaintSeconds;

		[ProtoMember(24)]
		public int BattleWitchSide;

		[ProtoMember(25)]
		public long BirthTicks;
	}
}
