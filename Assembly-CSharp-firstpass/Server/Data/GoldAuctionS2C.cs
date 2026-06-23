using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GoldAuctionS2C
	{
		[ProtoMember(1)]
		public List<AuctionItemS2C> ItemList;

		[ProtoMember(2)]
		public int Info;

		[ProtoMember(3)]
		public int TotalCount;

		[ProtoMember(4)]
		public int CurrentPage;
	}
}
