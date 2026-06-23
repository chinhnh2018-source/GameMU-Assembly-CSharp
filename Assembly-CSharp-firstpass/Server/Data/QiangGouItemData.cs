using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QiangGouItemData
	{
		[ProtoMember(1)]
		public int QiangGouID;

		[ProtoMember(2)]
		public int Group;

		[ProtoMember(3)]
		public int ItemID;

		[ProtoMember(4)]
		public int GoodsID;

		[ProtoMember(5)]
		public string StartTime = string.Empty;

		[ProtoMember(6)]
		public string EndTime = string.Empty;

		[ProtoMember(7)]
		public int IsTimeOver;

		[ProtoMember(8)]
		public int SinglePurchase;

		[ProtoMember(9)]
		public int FullPurchase;

		[ProtoMember(10)]
		public int FullHasPurchase;

		[ProtoMember(11)]
		public int SingleHasPurchase;

		[ProtoMember(12)]
		public int CurrentRoleID;

		[ProtoMember(13)]
		public int DaysTime;

		[ProtoMember(14)]
		public int Price;

		[ProtoMember(15)]
		public int Random;

		[ProtoMember(16)]
		public int OrigPrice;
	}
}
