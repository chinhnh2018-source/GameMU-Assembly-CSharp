using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SearchRoleData
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
		public int MapCode;

		[ProtoMember(7)]
		public int PosX;

		[ProtoMember(8)]
		public int PosY;

		[ProtoMember(9)]
		public int Faction;

		[ProtoMember(10)]
		public string BHName = string.Empty;

		[ProtoMember(11)]
		public int CombatForce;

		[ProtoMember(12)]
		public int ChangeLifeLev;
	}
}
