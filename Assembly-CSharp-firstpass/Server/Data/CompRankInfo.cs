using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CompRankInfo
	{
		[ProtoMember(1)]
		public int Key;

		[ProtoMember(2)]
		public int Value;

		[ProtoMember(3)]
		public string Param1 = string.Empty;
	}
}
