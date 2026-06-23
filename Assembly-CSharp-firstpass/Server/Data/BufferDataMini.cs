using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BufferDataMini
	{
		[ProtoMember(1)]
		public int BufferID;

		[ProtoMember(2)]
		public long StartTime;

		[ProtoMember(3)]
		public int BufferSecs;

		[ProtoMember(4)]
		public long BufferVal;

		[ProtoMember(5)]
		public int BufferType;
	}
}
