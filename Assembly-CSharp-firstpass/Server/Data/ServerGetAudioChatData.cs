using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerGetAudioChatData
	{
		[ProtoMember(1)]
		public byte[] arrAudioChat;
	}
}
