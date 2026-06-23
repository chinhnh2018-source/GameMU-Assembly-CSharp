using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangGongHistData
	{
		[ProtoMember(1)]
		public int ZoneID;

		[ProtoMember(2)]
		public int RoleID;

		[ProtoMember(3)]
		public string RoleName = string.Empty;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int RoleLevel;

		[ProtoMember(6)]
		public int BHZhiWu;

		[ProtoMember(7)]
		public string BHChengHao = string.Empty;

		[ProtoMember(8)]
		public int Goods1Num;

		[ProtoMember(9)]
		public int Goods2Num;

		[ProtoMember(10)]
		public int Goods3Num;

		[ProtoMember(11)]
		public int Goods4Num;

		[ProtoMember(12)]
		public int Goods5Num;

		[ProtoMember(13)]
		public int TongQian;

		[ProtoMember(14)]
		public int BangGong;
	}
}
