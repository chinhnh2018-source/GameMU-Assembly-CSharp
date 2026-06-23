using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DailyJingMaiData
	{
		[ProtoMember(1)]
		public string JmTime = string.Empty;

		[ProtoMember(2)]
		public int JmNum;
	}
}
