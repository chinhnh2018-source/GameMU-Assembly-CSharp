using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OlympicsShopData
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public int DayID;

		[ProtoMember(3)]
		public GoodsData Goods;

		[ProtoMember(4)]
		public int Price;

		[ProtoMember(5)]
		public int NumSingl;

		[ProtoMember(6)]
		public int NumFull;

		[ProtoMember(7)]
		public int NumSingleBuy;

		[ProtoMember(8)]
		public int NumFullBuy = -1;
	}
}
