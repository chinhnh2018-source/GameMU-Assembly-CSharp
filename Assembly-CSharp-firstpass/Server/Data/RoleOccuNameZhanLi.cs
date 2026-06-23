using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleOccuNameZhanLi
	{
		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public int Occupation;

		[ProtoMember(4)]
		public int ZhanLi;
	}
}
