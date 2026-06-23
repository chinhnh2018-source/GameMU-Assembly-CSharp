using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PrestigeMedalData
	{
		public override string ToString()
		{
			return string.Concat(new object[]
			{
				"RoleID, ",
				this.RoleID,
				", MedalID, ",
				this.MedalID,
				", LifeAdd, ",
				this.LifeAdd,
				", AttackAdd, ",
				this.AttackAdd,
				", DefenseAdd, ",
				this.DefenseAdd,
				", HitAdd, ",
				this.HitAdd,
				", Prestige, ",
				this.Prestige,
				", Diamond, ",
				this.Diamond,
				", BurstType, ",
				this.BurstType,
				", UpResultType, ",
				this.UpResultType
			});
		}

		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int MedalID;

		[ProtoMember(3)]
		public int LifeAdd;

		[ProtoMember(4)]
		public int AttackAdd;

		[ProtoMember(5)]
		public int DefenseAdd;

		[ProtoMember(6)]
		public int HitAdd;

		[ProtoMember(7)]
		public int Prestige;

		[ProtoMember(8)]
		public int Diamond;

		[ProtoMember(9)]
		public int BurstType;

		[ProtoMember(10)]
		public int UpResultType;

		[ProtoMember(11)]
		public int PrestigeLeft;
	}
}
