using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class KFBoCaoHistoryData
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
		public int WinNo;

		[ProtoMember(7)]
		public long DataPeriods;

		[ProtoMember(8)]
		public int ServerID;

		[ProtoMember(9)]
		public string OpenData;
	}
}
