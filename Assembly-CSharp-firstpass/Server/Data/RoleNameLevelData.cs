using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleNameLevelData
	{
		[ProtoMember(1)]
		public int ZhuanSheng;

		[ProtoMember(2)]
		public int Level;

		[ProtoMember(3)]
		public string RoleName;

		[ProtoMember(4)]
		public bool ZhiWu;

		[ProtoMember(5)]
		public int Occupation;
	}
}
