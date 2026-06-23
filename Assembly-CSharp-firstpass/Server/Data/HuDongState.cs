using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HuDongState
	{
		[ProtoMember(1)]
		public int ID;

		[ProtoMember(2)]
		public string Name1;

		[ProtoMember(3)]
		public string Name2;

		[ProtoMember(4)]
		public int JiangLiType;

		[ProtoMember(5)]
		public int JiangLiCount;
	}
}
