using System;
using ProtoBuf;
using Server.Data;

namespace Tmsk.Contract.KuaFuData
{
	[ProtoContract]
	public class KFCompRoleData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int ZoneID;

		[ProtoMember(3)]
		public int ZhiWu;

		[ProtoMember(4)]
		public int CompType;

		[ProtoMember(5)]
		public int JunXian;

		[ProtoMember(6)]
		public RoleData4Selector RoleData4Selector;

		public long BossDamage;
	}
}
