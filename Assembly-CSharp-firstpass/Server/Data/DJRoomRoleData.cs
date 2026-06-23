using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DJRoomRoleData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int DJPoint;

		[ProtoMember(5)]
		public int DJTotal;

		[ProtoMember(6)]
		public int DJWincnt;
	}
}
