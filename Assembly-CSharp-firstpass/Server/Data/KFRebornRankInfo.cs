using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KFRebornRankInfo
	{
		[ProtoMember(1)]
		public int Key;

		[ProtoMember(2)]
		public int Value;

		[ProtoMember(3)]
		public string Param1 = string.Empty;

		[ProtoMember(4)]
		public string Param2 = string.Empty;

		[ProtoMember(5)]
		public int PtID;
	}
}
