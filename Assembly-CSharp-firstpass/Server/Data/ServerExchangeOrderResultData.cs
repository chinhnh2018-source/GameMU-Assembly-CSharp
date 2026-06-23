using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerExchangeOrderResultData
	{
		[ProtoMember(1)]
		public string strState = string.Empty;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5;
	}
}
