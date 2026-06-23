using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TeamMemberData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public int RoleSex;

		[ProtoMember(4)]
		public int Level;

		[ProtoMember(5)]
		public int Occupation;

		[ProtoMember(6)]
		public int RolePic;

		[ProtoMember(7)]
		public int MapCode;

		[ProtoMember(8)]
		public int OnlineState;

		[ProtoMember(9)]
		public int MaxLifeV;

		[ProtoMember(10)]
		public int CurrentLifeV;

		[ProtoMember(11)]
		public int MaxMagicV;

		[ProtoMember(12)]
		public int CurrentMagicV;

		[ProtoMember(13)]
		public int PosX;

		[ProtoMember(14)]
		public int PosY;

		[ProtoMember(15)]
		public int CombatForce;

		[ProtoMember(16)]
		public int ChangeLifeLev;
	}
}
