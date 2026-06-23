using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientServerListDataEx
	{
		[ProtoMember(1)]
		public long Time;

		[ProtoMember(2)]
		public string Md5 = string.Empty;

		[ProtoMember(3)]
		public string ServerId;

		[ProtoMember(4)]
		public string UserId;
	}
}
