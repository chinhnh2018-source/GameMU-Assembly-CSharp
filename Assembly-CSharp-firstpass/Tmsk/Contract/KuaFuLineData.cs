using System;
using ProtoBuf;

namespace Tmsk.Contract
{
	[ProtoContract]
	public class KuaFuLineData
	{
		[ProtoMember(1)]
		public int LineID;

		[ProtoMember(2)]
		public int State;

		[ProtoMember(3)]
		public int OnlineCount;

		[ProtoMember(4)]
		public int MaxOnlineCount;

		[ProtoMember(5)]
		public int ServerId;

		[ProtoMember(6)]
		public int MapCode;
	}
}
