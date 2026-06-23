using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientPostAudioChatData
	{
		[ProtoMember(1)]
		public int iTargetNum = 1;

		[ProtoMember(2)]
		public byte[] arrAudioChat;

		[ProtoMember(3)]
		public long lTime;

		[ProtoMember(4)]
		public string strMD5 = string.Empty;
	}
}
