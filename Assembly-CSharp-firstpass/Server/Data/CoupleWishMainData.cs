using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleWishMainData
	{
		[ProtoMember(1)]
		public List<CoupleWishCoupleData> RankList;

		[ProtoMember(2)]
		public int MyCoupleRank;

		[ProtoMember(3)]
		public int MyCoupleBeWishNum;

		[ProtoMember(4)]
		public int CanGetAwardId;

		[ProtoMember(5)]
		public RoleData4Selector MyCoupleManSelector;

		[ProtoMember(6)]
		public RoleData4Selector MyCoupleWifeSelector;
	}
}
