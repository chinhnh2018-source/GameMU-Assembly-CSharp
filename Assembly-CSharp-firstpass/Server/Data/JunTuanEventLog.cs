using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanEventLog
	{
		[ProtoMember(1)]
		public int EventType;

		[ProtoMember(2)]
		public DateTime Time;

		[ProtoMember(3)]
		public string Message;
	}
}
