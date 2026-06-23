using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiBagData
	{
		[ProtoMember(1)]
		public int Goods1Num;

		[ProtoMember(2)]
		public int Goods2Num;

		[ProtoMember(3)]
		public int Goods3Num;

		[ProtoMember(4)]
		public int Goods4Num;

		[ProtoMember(5)]
		public int Goods5Num;

		[ProtoMember(6)]
		public int TongQian;

		[ProtoMember(7)]
		public List<BangGongHistData> BbangGongHistList;
	}
}
