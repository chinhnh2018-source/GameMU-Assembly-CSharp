using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SaleGoodsSearchResultData
	{
		[ProtoMember(1)]
		public int Type;

		[ProtoMember(2)]
		public int ID;

		[ProtoMember(3)]
		public int MoneyFlags;

		[ProtoMember(4)]
		public int ColorFlags;

		[ProtoMember(5)]
		public int OrderByMoney;

		[ProtoMember(6)]
		public int StartIndex;

		[ProtoMember(7)]
		public int TotalCount;

		[ProtoMember(8)]
		public List<SaleGoodsData> saleGoodsDataList;
	}
}
