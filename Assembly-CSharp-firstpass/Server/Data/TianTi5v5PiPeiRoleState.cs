using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TianTi5v5PiPeiRoleState
	{
		[ProtoMember(1)]
		public string RoleName;

		[ProtoMember(2)]
		public int Occupation;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int RoleID;
	}
}
