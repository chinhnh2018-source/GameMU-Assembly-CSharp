using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleWishYanHuiData
	{
		[ProtoMember(1)]
		public KuaFuRoleMiniData Man;

		[ProtoMember(2)]
		public KuaFuRoleMiniData Wife;

		[ProtoMember(3, IsRequired = true)]
		public int TotalJoinNum;

		[ProtoMember(4, IsRequired = true)]
		public int MyJoinNum;

		[ProtoMember(5, IsRequired = true)]
		public int DbCoupleId;
	}
}
