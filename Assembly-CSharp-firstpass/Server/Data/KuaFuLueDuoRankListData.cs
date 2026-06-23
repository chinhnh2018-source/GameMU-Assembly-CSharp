using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KuaFuLueDuoRankListData
	{
		[ProtoMember(1)]
		public long Age;

		[ProtoMember(2)]
		public Dictionary<int, List<KuaFuLueDuoRankInfo>> ListDict = new Dictionary<int, List<KuaFuLueDuoRankInfo>>();

		[ProtoMember(3)]
		public Dictionary<int, KuaFuLueDuoRankInfo> LastInfoDict;

		[ProtoMember(4)]
		public Dictionary<int, KuaFuLueDuoRankInfo> SelfInfoDict;
	}
}
