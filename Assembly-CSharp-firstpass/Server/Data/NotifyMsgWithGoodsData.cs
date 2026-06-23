using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class NotifyMsgWithGoodsData
	{
		[ProtoMember(1)]
		public int index;

		[ProtoMember(2)]
		public int type;

		[ProtoMember(3)]
		public List<GoodsData> goodsDataList;

		[ProtoMember(4)]
		public string param1 = string.Empty;

		[ProtoMember(5)]
		public Dictionary<int, List<GoodsData>> goodsDic;
	}
}
