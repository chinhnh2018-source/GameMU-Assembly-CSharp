using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class YaBiaoData
	{
		[ProtoMember(1)]
		public int YaBiaoID;

		[ProtoMember(2)]
		public long StartTime;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int LineID;

		[ProtoMember(5)]
		public int TouBao;

		[ProtoMember(6)]
		public int YaBiaoDayID;

		[ProtoMember(7)]
		public int YaBiaoNum;

		[ProtoMember(8)]
		public int TakeGoods;
	}
}
