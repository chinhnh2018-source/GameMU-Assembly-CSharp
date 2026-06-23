using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SelfBuyInfo
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public string WuPinID;

		[ProtoMember(3)]
		public int BuyNum;
	}
}
