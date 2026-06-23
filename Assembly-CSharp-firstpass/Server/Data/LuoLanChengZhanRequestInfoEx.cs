using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LuoLanChengZhanRequestInfoEx
	{
		[ProtoMember(1)]
		public int Site;

		[ProtoMember(2)]
		public int BHID;

		[ProtoMember(3)]
		public int BidMoney;

		[ProtoMember(4)]
		public string BHName = string.Empty;
	}
}
