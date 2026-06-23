using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AllyData
	{
		[ProtoMember(1)]
		public int UnionID;

		[ProtoMember(2)]
		public int UnionZoneID;

		[ProtoMember(3)]
		public string UnionName = string.Empty;

		[ProtoMember(4)]
		public int UnionLevel;

		[ProtoMember(5)]
		public int UnionNum;

		[ProtoMember(6)]
		public int LeaderID;

		[ProtoMember(7)]
		public int LeaderZoneID;

		[ProtoMember(8)]
		public string LeaderName = string.Empty;

		[ProtoMember(9)]
		public DateTime LogTime = DateTime.MinValue;

		[ProtoMember(10)]
		public int LogState;
	}
}
