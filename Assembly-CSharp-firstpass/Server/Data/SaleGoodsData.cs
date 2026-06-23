using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SaleGoodsData
	{
		[ProtoMember(1)]
		public int GoodsDbID;

		[ProtoMember(2)]
		public GoodsData SalingGoodsData;

		[ProtoMember(3)]
		public int RoleID;

		[ProtoMember(4)]
		public string RoleName = string.Empty;

		[ProtoMember(5)]
		public int RoleLevel;
	}
}
