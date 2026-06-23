using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FluorescentGemData
	{
		[ProtoMember(1)]
		public Dictionary<int, Dictionary<int, GoodsData>> GemInstalList = new Dictionary<int, Dictionary<int, GoodsData>>();

		[ProtoMember(2)]
		public Dictionary<int, GoodsData> GemStoreList = new Dictionary<int, GoodsData>();
	}
}
