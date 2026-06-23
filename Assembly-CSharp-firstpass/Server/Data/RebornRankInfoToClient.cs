using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RebornRankInfoToClient
	{
		[ProtoMember(1)]
		public int RankType;

		[ProtoMember(2)]
		public List<KFRebornRankInfo> rankList = new List<KFRebornRankInfo>();
	}
}
