using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengDuoScoreInfo
	{
		[ProtoMember(1)]
		public List<ZhengDuoScoreData> ScoreRank;

		[ProtoMember(2)]
		public int Step;
	}
}
