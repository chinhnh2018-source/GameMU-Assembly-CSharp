using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerVerifySIDDataToken
	{
		[ProtoMember(1)]
		public string strPlatformUserID = string.Empty;

		[ProtoMember(2)]
		public string strAccountName = string.Empty;

		[ProtoMember(3)]
		public long lTime;

		[ProtoMember(4)]
		public string strCM = "1";

		[ProtoMember(5)]
		public string strToken = string.Empty;

		[ProtoMember(6)]
		public string strPlatformToken = string.Empty;
	}
}
