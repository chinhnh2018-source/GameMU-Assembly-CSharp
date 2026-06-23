using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientGetAudioChatData
	{
		[ProtoMember(1)]
		public string iAudioChatOrder = string.Empty;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5 = string.Empty;
	}
}
