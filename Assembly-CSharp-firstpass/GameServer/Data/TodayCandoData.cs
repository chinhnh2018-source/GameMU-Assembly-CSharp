using System;
using ProtoBuf;

namespace GameServer.Data
{
	[ProtoContract]
	public class TodayCandoData
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public int LeftCount;
	}
}
