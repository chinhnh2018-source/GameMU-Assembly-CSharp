using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiItemData
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string BHName = string.Empty;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public int BZRoleID;

		[ProtoMember(5)]
		public string BZRoleName = string.Empty;

		[ProtoMember(6)]
		public int BZOccupation;

		[ProtoMember(7)]
		public int TotalNum;

		[ProtoMember(8)]
		public int TotalLevel;

		[ProtoMember(9)]
		public int QiLevel;

		[ProtoMember(10)]
		public int IsVerfiy;

		[ProtoMember(11)]
		public int TotalCombatForce;
	}
}
