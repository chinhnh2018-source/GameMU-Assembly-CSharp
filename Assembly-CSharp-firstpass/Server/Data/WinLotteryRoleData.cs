using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class WinLotteryRoleData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public int BuyNum;

		[ProtoMember(5)]
		public long WinMoney;

		[ProtoMember(6)]
		public int AwardNo;
	}
}
