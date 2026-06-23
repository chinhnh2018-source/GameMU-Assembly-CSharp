using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KaiFuOnlineAwardData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int ZoneID;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public int DayID;

		[ProtoMember(5)]
		public int TotalRoleNum;
	}
}
