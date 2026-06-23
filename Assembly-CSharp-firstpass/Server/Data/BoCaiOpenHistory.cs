using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BoCaiOpenHistory
	{
		[ProtoMember(1)]
		public long DataPeriods;

		[ProtoMember(2)]
		public string OpenValue;
	}
}
