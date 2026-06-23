using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class QiangGouBuyItemData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int GoodsID;

		[ProtoMember(4)]
		public int GoodsNum;

		[ProtoMember(3)]
		public int QiangGouID;
	}
}
