using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientAudioURLData
	{
		[ProtoMember(1)]
		public string strPlatform = string.Empty;

		[ProtoMember(2)]
		public int iServerID = 1;

		[ProtoMember(3)]
		public long lTime;

		[ProtoMember(4)]
		public string strMD5 = string.Empty;
	}
}
