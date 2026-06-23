using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QiZhenGeItemData
	{
		[ProtoMember(1)]
		public int ItemID;

		[ProtoMember(2)]
		public int GoodsID;

		[ProtoMember(3)]
		public int OrigPrice;

		[ProtoMember(4)]
		public int Price;

		[ProtoMember(5)]
		public string Description = string.Empty;

		[ProtoMember(6)]
		public int BaseProbability;

		[ProtoMember(7)]
		public int SelfProbability;
	}
}
