using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoBangHuiJingJiaData
	{
		[ProtoMember(1)]
		public int ServerId;

		[ProtoMember(2)]
		public int BhId;

		[ProtoMember(3)]
		public int ZoneId;

		[ProtoMember(4)]
		public string BhName;

		[ProtoMember(5)]
		public int ZiJin;
	}
}
