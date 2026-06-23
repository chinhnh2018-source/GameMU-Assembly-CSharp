using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class EscapeBattleRankInfo
	{
		[ProtoMember(1)]
		public List<KFEscapeRankInfo> rankInfo2Client = new List<KFEscapeRankInfo>();

		[ProtoMember(2)]
		public KFEscapeRankInfo myZhanDuiRankInfo = new KFEscapeRankInfo();

		[ProtoMember(3)]
		public int SelfRank;
	}
}
