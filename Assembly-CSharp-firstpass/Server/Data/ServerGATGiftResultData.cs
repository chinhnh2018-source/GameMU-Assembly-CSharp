using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerGATGiftResultData
	{
		[ProtoMember(1)]
		public string strLongGiftKey;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5;
	}
}
