using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DecorationData
	{
		[ProtoMember(1)]
		public int AutoID;

		[ProtoMember(2)]
		public int DecoID;

		[ProtoMember(3)]
		public int PosX;

		[ProtoMember(4)]
		public int PosY;

		[ProtoMember(5)]
		public int MapCode = -1;

		[ProtoMember(6)]
		public long StartTicks;

		[ProtoMember(7)]
		public int MaxLiveTicks;

		[ProtoMember(8)]
		public int AlphaTicks;
	}
}
