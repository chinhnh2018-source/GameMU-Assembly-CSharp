using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class YaoSaiData
	{
		[ProtoMember(1)]
		public int state;

		[ProtoMember(2)]
		public YaoSaiRoleDataMini Mine;

		[ProtoMember(3)]
		public YaoSaiRoleDataMini Master;

		[ProtoMember(4)]
		public int zhenfuCount;

		[ProtoMember(5)]
		public int zhenfuLeftCount;

		[ProtoMember(6)]
		public int jiejiuCount;

		[ProtoMember(7)]
		public List<FuLuState> FuLuList;
	}
}
