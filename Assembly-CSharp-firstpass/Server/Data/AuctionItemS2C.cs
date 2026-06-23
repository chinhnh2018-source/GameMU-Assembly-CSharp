using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AuctionItemS2C
	{
		[ProtoMember(1)]
		public int BuyRoleId;

		[ProtoMember(2)]
		public long Price;

		[ProtoMember(3)]
		public long LastTime;

		[ProtoMember(4)]
		public string AuctionItemKey;

		[ProtoMember(5)]
		public GoodsData Goods;

		[ProtoMember(6)]
		public long MaxPrice;

		[ProtoMember(7)]
		public long UnitPrice;
	}
}
