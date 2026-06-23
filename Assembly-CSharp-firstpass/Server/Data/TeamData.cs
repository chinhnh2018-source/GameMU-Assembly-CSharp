using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TeamData
	{
		[ProtoMember(1)]
		public int TeamID;

		[ProtoMember(2)]
		public int LeaderRoleID;

		[ProtoMember(3)]
		public List<TeamMemberData> TeamRoles;

		[ProtoMember(4)]
		public long AddDateTime;

		[ProtoMember(5)]
		public int GetThingOpt;

		[ProtoMember(6)]
		public int PosX;

		[ProtoMember(7)]
		public int PosY;
	}
}
