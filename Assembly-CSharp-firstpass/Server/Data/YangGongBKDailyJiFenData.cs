using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class YangGongBKDailyJiFenData
	{
		[ProtoMember(1)]
		public int DayID;

		[ProtoMember(2)]
		public int JiFen;

		[ProtoMember(3)]
		public long AwardHistory;
	}
}
