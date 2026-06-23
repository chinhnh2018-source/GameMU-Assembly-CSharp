using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerPostPushIDData
	{
		[ProtoMember(1)]
		public long iPushIDChatOrder;
	}
}
