using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AchievementRuneData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int RuneID;

		[ProtoMember(3)]
		public int LifeAdd;

		[ProtoMember(4)]
		public int AttackAdd;

		[ProtoMember(5)]
		public int DefenseAdd;

		[ProtoMember(6)]
		public int DodgeAdd;

		[ProtoMember(7)]
		public int Achievement;

		[ProtoMember(8)]
		public int Diamond;

		[ProtoMember(9)]
		public int BurstType;

		[ProtoMember(10)]
		public int UpResultType;

		[ProtoMember(11)]
		public int AchievementLeft;
	}
}
