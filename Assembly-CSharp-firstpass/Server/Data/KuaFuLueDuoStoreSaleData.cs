using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoStoreSaleData
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public int Purchase;
	}
}
