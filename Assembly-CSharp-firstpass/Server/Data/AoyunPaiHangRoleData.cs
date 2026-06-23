using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AoyunPaiHangRoleData
	{
		[ProtoMember(1)]
		public int ZoneId;

		[ProtoMember(2)]
		public int RoleId;

		[ProtoMember(3)]
		public string RoleName;

		[ProtoMember(4)]
		public int RolePoint;

		[ProtoMember(5)]
		public int RoleLastPoint;

		[ProtoMember(6)]
		public int RoleRank;
	}
}
