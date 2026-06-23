using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LevelupDiamondObject
	{
		[ProtoMember(1)]
		public int status;

		[ProtoMember(2)]
		public int flag;

		[ProtoMember(3)]
		public GoodsData newGoods;
	}
}
