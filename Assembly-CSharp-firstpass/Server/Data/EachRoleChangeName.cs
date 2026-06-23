using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EachRoleChangeName
	{
		[ProtoMember(1)]
		public int RoleId;

		[ProtoMember(2)]
		public int LeftFreeTimes;

		[ProtoMember(3)]
		public int AlreadyZuanShiTimes;
	}
}
