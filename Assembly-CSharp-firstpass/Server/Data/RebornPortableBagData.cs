using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RebornPortableBagData
	{
		[ProtoMember(1)]
		public int ExtGridNum;

		[ProtoMember(2)]
		public int GoodsUsedGridNum;
	}
}
