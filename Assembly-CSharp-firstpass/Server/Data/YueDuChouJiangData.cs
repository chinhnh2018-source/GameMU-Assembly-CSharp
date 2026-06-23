using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class YueDuChouJiangData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int GainGoodsId;

		[ProtoMember(4)]
		public int GainGoodsNum;

		[ProtoMember(5)]
		public int GainGold;

		[ProtoMember(6)]
		public int GainYinLiang;

		[ProtoMember(7)]
		public int GainExp;

		[ProtoMember(8)]
		public string OperationTime = string.Empty;
	}
}
