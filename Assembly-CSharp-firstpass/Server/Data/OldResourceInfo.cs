using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class OldResourceInfo
	{
		[ProtoMember(1)]
		public int type = 1;

		[ProtoMember(2)]
		public int exp;

		[ProtoMember(3)]
		public int bandmoney;

		[ProtoMember(4)]
		public int mojing;

		[ProtoMember(5)]
		public int chengjiu;

		[ProtoMember(6)]
		public int shengwang;

		[ProtoMember(7)]
		public int zhangong;

		[ProtoMember(8)]
		public int leftCount;

		[ProtoMember(9)]
		public int roleId;

		[ProtoMember(10)]
		public int bandDiamond;

		[ProtoMember(11)]
		public int xinghun;

		[ProtoMember(12)]
		public int yuanSuFenMo;
	}
}
