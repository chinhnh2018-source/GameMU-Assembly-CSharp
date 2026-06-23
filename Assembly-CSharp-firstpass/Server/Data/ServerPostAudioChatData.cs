using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerPostAudioChatData
	{
		[ProtoMember(1)]
		public string iAudioChatOrder = string.Empty;
	}
}
