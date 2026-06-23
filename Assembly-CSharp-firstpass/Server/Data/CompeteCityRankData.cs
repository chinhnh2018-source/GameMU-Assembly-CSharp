using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompeteCityRankData
	{
		[ProtoMember(1)]
		public List<ZhengDuoRankData> RankList;

		[ProtoMember(2)]
		public int UsedMillisecond;
	}
}
