using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KuaFuLueDuoRankInfo
	{
		[ProtoMember(1)]
		public int Value;

		[ProtoMember(2)]
		public string Param1 = string.Empty;

		[ProtoMember(4)]
		public int Key;

		[NonSerialized]
		public int RankValue;
	}
}
