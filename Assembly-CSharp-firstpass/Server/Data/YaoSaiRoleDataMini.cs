using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class YaoSaiRoleDataMini
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int ChangeLevel;

		[ProtoMember(5)]
		public int ZoneID;

		[ProtoMember(6)]
		public int Occupation;

		[ProtoMember(7)]
		public int RoleSex;

		[ProtoMember(8)]
		public int CombatForce;
	}
}
