using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LuoLanChengZhanResultInfo
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string BHName = string.Empty;

		[ProtoMember(3)]
		public long ExpAward;

		[ProtoMember(4)]
		public int ZhanGongAward;

		[ProtoMember(5)]
		public int ZhanMengZiJin;
	}
}
