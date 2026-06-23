using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KuaFuLueDuoRankListCmdData
	{
		[ProtoMember(1)]
		public long Age;

		[ProtoMember(2)]
		public int RankType;

		[ProtoMember(3)]
		public List<KuaFuLueDuoRankInfo> ListRankList = new List<KuaFuLueDuoRankInfo>();

		[ProtoMember(4)]
		public KuaFuLueDuoRankInfo LastData;

		[ProtoMember(5)]
		public KuaFuLueDuoRankInfo SelfData;
	}
}
