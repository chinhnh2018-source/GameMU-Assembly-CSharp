using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiMatchRankInfo
	{
		[ProtoMember(1)]
		public int Value;

		[ProtoMember(2)]
		public string Param1 = string.Empty;

		[ProtoMember(3)]
		public string Param2 = string.Empty;

		[ProtoMember(4)]
		public int Key;

		[NonSerialized]
		public int RankValue;
	}
}
