using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleArenaRoleStateData
	{
		[ProtoMember(1)]
		public int RoleId;

		[ProtoMember(2)]
		public int MatchState;
	}
}
