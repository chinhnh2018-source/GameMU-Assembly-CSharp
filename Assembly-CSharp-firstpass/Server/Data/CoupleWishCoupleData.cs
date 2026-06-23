using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleWishCoupleData
	{
		[ProtoMember(1)]
		public int DbCoupleId;

		[ProtoMember(2)]
		public KuaFuRoleMiniData Man;

		[ProtoMember(3)]
		public RoleData4Selector ManSelector;

		[ProtoMember(4)]
		public KuaFuRoleMiniData Wife;

		[ProtoMember(5)]
		public RoleData4Selector WifeSelector;

		[ProtoMember(6)]
		public int BeWishedNum;

		[ProtoMember(7)]
		public int Rank;
	}
}
