using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BoCaiShopInfo
	{
		[ProtoMember(1)]
		public List<SelfBuyInfo> ItemList;

		[ProtoMember(2)]
		public int Info;
	}
}
