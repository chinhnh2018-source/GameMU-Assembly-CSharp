using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OlympicsRankData
	{
		[ProtoMember(1)]
		public int Rank;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string RoleName;

		[ProtoMember(4)]
		public int Grade;

		[ProtoMember(5)]
		public RoleDataEx RoleData;
	}
}
