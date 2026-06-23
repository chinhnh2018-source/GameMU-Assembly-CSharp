using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DJRoleInfoData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int Occupation;

		[ProtoMember(5)]
		public int OnlineState;
	}
}
