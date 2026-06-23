using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FakeRoleData
	{
		[ProtoMember(1)]
		public int FakeRoleID;

		[ProtoMember(2)]
		public int FakeRoleType;

		[ProtoMember(3)]
		public int ToExtensionID;

		[ProtoMember(4)]
		public RoleDataMini MyRoleDataMini;
	}
}
