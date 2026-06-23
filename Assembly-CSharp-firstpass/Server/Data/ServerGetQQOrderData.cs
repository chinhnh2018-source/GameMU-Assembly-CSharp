using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerGetQQOrderData
	{
		[ProtoMember(1)]
		public string strExchangeOrder = string.Empty;

		[ProtoMember(2)]
		public string strURLParam = string.Empty;

		[ProtoMember(3)]
		public string strToken = string.Empty;

		[ProtoMember(4)]
		public long lTime;

		[ProtoMember(5)]
		public string strMD5;
	}
}
