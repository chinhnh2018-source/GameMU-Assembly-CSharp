using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhuanPanChouJiangData
	{
		[ProtoMember(1)]
		public int Result;

		[ProtoMember(2)]
		public ZhuanPanItem GoodsItem;

		[ProtoMember(3)]
		public DateTime FreeTime;

		[ProtoMember(4)]
		public int LeftFuLiCount;
	}
}
