using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BuyBoCaiResult
	{
		[ProtoMember(1)]
		public List<BoCaiBuyItem> ItemList;

		[ProtoMember(2)]
		public int Info;

		[ProtoMember(3)]
		public int BocaiType;
	}
}
