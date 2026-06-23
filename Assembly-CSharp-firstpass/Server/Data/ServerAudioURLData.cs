using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerAudioURLData
	{
		[ProtoMember(1)]
		public string strAudioURL = string.Empty;
	}
}
