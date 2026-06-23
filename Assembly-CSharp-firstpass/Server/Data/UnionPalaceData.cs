using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class UnionPalaceData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int StatueID;

		[ProtoMember(3)]
		public int StatueLevel;

		[ProtoMember(4)]
		public int LifeAdd;

		[ProtoMember(5)]
		public int AttackAdd;

		[ProtoMember(6)]
		public int DefenseAdd;

		[ProtoMember(7)]
		public int AttackInjureAdd;

		[ProtoMember(8)]
		public int ZhanGongNeed;

		[ProtoMember(9)]
		public int BurstType;

		[ProtoMember(10)]
		public int ResultType;

		[ProtoMember(11)]
		public int ZhanGongLeft;

		[ProtoMember(12)]
		public int UnionLevel;

		[ProtoMember(13)]
		public int StatueType;
	}
}
