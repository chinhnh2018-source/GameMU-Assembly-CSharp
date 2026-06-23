using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class PrisonMainData
	{
		[ProtoMember(1)]
		public YaoSaiRoleDataMini roleData;

		[ProtoMember(2)]
		public int MineFuLuState;

		[ProtoMember(3)]
		public long RevoltCD;

		[ProtoMember(4)]
		public int JieJiuCount;

		[ProtoMember(5)]
		public int ZhengFuCount;

		[ProtoMember(6)]
		public int ZhengFuLeftCount;

		[ProtoMember(7)]
		public int LaoDongCount;

		[ProtoMember(8)]
		public List<FuLuState> FuLuData;
	}
}
