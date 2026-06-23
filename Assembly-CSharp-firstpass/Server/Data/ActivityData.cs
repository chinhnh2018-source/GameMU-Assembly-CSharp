using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ActivityData
	{
		[ProtoMember(1)]
		public int ActivityType;

		[ProtoMember(2)]
		public bool ActivityIsOpen;

		[ProtoMember(3)]
		public DateTime TimeBegin = DateTime.MinValue;

		[ProtoMember(4)]
		public DateTime TimeEnd = DateTime.MinValue;

		[ProtoMember(5)]
		public DateTime TimeAwardBegin = DateTime.MinValue;

		[ProtoMember(6)]
		public DateTime TimeAwardEnd = DateTime.MinValue;
	}
}
