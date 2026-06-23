using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LingDiMapInfoData
	{
		[ProtoMember(1)]
		public long FightingEndTime;

		[ProtoMember(2)]
		public long FightingStartTime;

		[ProtoMember(3)]
		public Dictionary<int, string> BHNameDict;
	}
}
