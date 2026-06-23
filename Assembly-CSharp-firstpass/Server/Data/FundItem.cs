using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FundItem
	{
		[ProtoMember(1, IsRequired = true)]
		public int FundType;

		[ProtoMember(2, IsRequired = true)]
		public int BuyType;

		[ProtoMember(3, IsRequired = true)]
		public DateTime BuyTime = DateTime.MinValue;

		[ProtoMember(4, IsRequired = true)]
		public int FundID;

		[ProtoMember(5, IsRequired = true)]
		public int AwardID;

		[ProtoMember(6, IsRequired = true)]
		public int AwardType;

		[ProtoMember(7, IsRequired = true)]
		public int Value1;

		[ProtoMember(8, IsRequired = true)]
		public int Value2;
	}
}
