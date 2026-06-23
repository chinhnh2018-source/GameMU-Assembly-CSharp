using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientPostPushIDData
	{
		[ProtoMember(1)]
		public string strPushID;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5 = string.Empty;
	}
}
