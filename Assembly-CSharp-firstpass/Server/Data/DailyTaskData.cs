using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DailyTaskData
	{
		[ProtoMember(1)]
		public int HuanID;

		[ProtoMember(2)]
		public string RecTime = string.Empty;

		[ProtoMember(3)]
		public int RecNum;

		[ProtoMember(4)]
		public int TaskClass;

		[ProtoMember(5)]
		public int ExtDayID;

		[ProtoMember(6)]
		public int ExtNum;
	}
}
