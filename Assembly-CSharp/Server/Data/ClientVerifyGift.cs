using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientVerifyGift
	{
		[ProtoMember(1)]
		public string codeno = string.Empty;

		[ProtoMember(2)]
		public int zoneid;

		[ProtoMember(3)]
		public int rid;

		[ProtoMember(4)]
		public string uid;

		[ProtoMember(5)]
		public int ptid;

		[ProtoMember(6)]
		public string channel = string.Empty;

		[ProtoMember(7)]
		public long lTime;

		[ProtoMember(8)]
		public string token = string.Empty;

		[ProtoMember(9)]
		public int appid;

		[ProtoMember(10)]
		public int themeid;
	}
}
