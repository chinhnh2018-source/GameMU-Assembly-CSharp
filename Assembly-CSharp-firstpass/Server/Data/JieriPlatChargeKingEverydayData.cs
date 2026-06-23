using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class JieriPlatChargeKingEverydayData
	{
		[ProtoMember(1)]
		public long hasgettimes;

		[ProtoMember(2)]
		public Dictionary<int, List<InputKingPaiHangData>> PaiHangDict;
	}
}
