using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SaleRoleData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int RoleLevel;

		[ProtoMember(4)]
		public int SaleGoodsNum;
	}
}
