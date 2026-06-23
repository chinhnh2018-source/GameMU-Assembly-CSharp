using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LineData
	{
		[ProtoMember(1)]
		public int LineID;

		[ProtoMember(2)]
		public string GameServerIP = string.Empty;

		[ProtoMember(3)]
		public int GameServerPort;

		[ProtoMember(4)]
		public int OnlineCount;
	}
}
