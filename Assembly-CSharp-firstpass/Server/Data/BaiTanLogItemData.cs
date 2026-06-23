using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BaiTanLogItemData
	{
		[ProtoMember(1)]
		public int rid;

		[ProtoMember(2)]
		public int OtherRoleID;

		[ProtoMember(3)]
		public string OtherRName = string.Empty;

		[ProtoMember(4)]
		public int GoodsID;

		[ProtoMember(5)]
		public int GoodsNum;

		[ProtoMember(6)]
		public int ForgeLevel;

		[ProtoMember(7)]
		public int TotalPrice;

		[ProtoMember(8)]
		public int LeftYuanBao;

		[ProtoMember(9)]
		public string BuyTime = string.Empty;

		[ProtoMember(10)]
		public int YinLiang;

		[ProtoMember(11)]
		public int LeftYinLiang;

		[ProtoMember(12)]
		public int Tax;

		[ProtoMember(13)]
		public int Excellenceinfo;

		[ProtoMember(14)]
		public string Washprops = string.Empty;
	}
}
