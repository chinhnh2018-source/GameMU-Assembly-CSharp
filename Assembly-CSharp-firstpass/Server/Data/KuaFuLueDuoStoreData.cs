using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoStoreData
	{
		[ProtoMember(1)]
		public DateTime LastRefTime;

		[ProtoMember(2)]
		public List<KuaFuLueDuoStoreSaleData> SaleList;
	}
}
