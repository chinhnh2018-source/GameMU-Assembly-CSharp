using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuLueDuoAwardsData
	{
		[ProtoMember(1)]
		public int type;

		[ProtoMember(2)]
		public int ZiYuan;

		[ProtoMember(3)]
		public int JiFen;

		[ProtoMember(4)]
		public long Exp;

		[ProtoMember(5)]
		public int JueXing;

		[ProtoMember(6)]
		public int BindJinBi;
	}
}
