using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompBattleSelfScore
	{
		[ProtoMember(1)]
		public int RankNum;

		[ProtoMember(2)]
		public int AwardID;

		[ProtoMember(3)]
		public List<CompRankInfo> rankInfo2Client = new List<CompRankInfo>();
	}
}
