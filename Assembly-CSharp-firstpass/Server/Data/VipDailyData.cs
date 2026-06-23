using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class VipDailyData
	{
		[ProtoMember(1)]
		public int PriorityType;

		[ProtoMember(2)]
		public int DayID;

		[ProtoMember(3)]
		public int UsedTimes;
	}
}
