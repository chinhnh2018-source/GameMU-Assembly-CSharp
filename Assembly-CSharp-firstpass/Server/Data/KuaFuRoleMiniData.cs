using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class KuaFuRoleMiniData
	{
		[ProtoMember(1)]
		public int RoleId;

		[ProtoMember(2)]
		public int ZoneId;

		[ProtoMember(3)]
		public string RoleName;
	}
}
