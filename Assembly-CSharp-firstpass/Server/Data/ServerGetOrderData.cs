using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerGetOrderData
	{
		[ProtoMember(1)]
		public string strExchangeOrder = string.Empty;

		[ProtoMember(2)]
		public string strExtParam = string.Empty;

		[ProtoMember(3)]
		public long lTime;

		[ProtoMember(4)]
		public string strMD5;
	}
}
