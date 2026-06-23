using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FuLuState
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string Name;

		[ProtoMember(3)]
		public int Level;

		[ProtoMember(4)]
		public int ChangeLevel;

		[ProtoMember(5)]
		public int ZoneID;

		[ProtoMember(6)]
		public int LaoDongState;

		[ProtoMember(7)]
		public long LaoDongTime;

		[ProtoMember(8)]
		public int Occupation;

		[ProtoMember(9)]
		public int RoleSex;
	}
}
