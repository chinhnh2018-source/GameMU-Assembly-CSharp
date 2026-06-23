using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DJRoomData
	{
		[ProtoMember(1)]
		public int RoomID;

		[ProtoMember(2)]
		public int CreateRoleID;

		[ProtoMember(3)]
		public string CreateRoleName = string.Empty;

		[ProtoMember(4)]
		public string RoomName = string.Empty;

		[ProtoMember(5)]
		public int VSMode;

		[ProtoMember(6)]
		public int PKState;

		[ProtoMember(7)]
		public int PKRoleNum;

		[ProtoMember(8)]
		public int ViewRoleNum;

		[ProtoMember(9)]
		public long StartFightTicks;

		[ProtoMember(10)]
		public int DJFightState;
	}
}
