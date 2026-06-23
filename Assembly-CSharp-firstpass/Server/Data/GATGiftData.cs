using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GATGiftData
	{
		[ProtoMember(1)]
		public string strShortGiftKey;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5;
	}
}
