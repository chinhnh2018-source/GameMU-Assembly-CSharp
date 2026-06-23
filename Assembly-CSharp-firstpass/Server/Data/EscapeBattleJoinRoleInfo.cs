using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EscapeBattleJoinRoleInfo
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
		public int CombatForce;

		[ProtoMember(6)]
		public bool Join;

		[ProtoMember(7)]
		public bool IsLeader;
	}
}
