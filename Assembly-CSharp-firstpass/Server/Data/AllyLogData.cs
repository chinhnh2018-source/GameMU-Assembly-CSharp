using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AllyLogData
	{
		[ProtoMember(1)]
		public int UnionID;

		[ProtoMember(2)]
		public int UnionZoneID;

		[ProtoMember(3)]
		public string UnionName = string.Empty;

		[ProtoMember(4)]
		public int MyUnionID;

		[ProtoMember(5)]
		public DateTime LogTime = DateTime.MinValue;

		[ProtoMember(6)]
		public int LogState;
	}
}
