using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengBaSupportLog
	{
		[ProtoMember(1)]
		public int FromRoleID;

		[ProtoMember(2)]
		public int FromZoneID;

		[ProtoMember(3)]
		public string FromRoleName;

		[ProtoMember(4)]
		public int SupportType;

		[ProtoMember(5)]
		public int ToUnionGroup;

		[ProtoMember(6)]
		public int ToGroup;

		[ProtoMember(7)]
		public DateTime Time;
	}
}
