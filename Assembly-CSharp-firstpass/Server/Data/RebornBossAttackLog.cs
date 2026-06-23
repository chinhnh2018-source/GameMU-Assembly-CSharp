using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RebornBossAttackLog
	{
		[ProtoMember(1)]
		public int PtID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string Param;

		[ProtoMember(4)]
		public int DamagePct;

		[ProtoMember(5)]
		public string RoleName;
	}
}
