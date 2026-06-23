using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZorkBattleSideScore
	{
		[ProtoMember(1)]
		public List<ZorkBattleRoleInfo> ZorkBattleRoleList = new List<ZorkBattleRoleInfo>();

		[ProtoMember(2)]
		public Dictionary<int, string> MosterNextTimeDict = new Dictionary<int, string>();

		[ProtoMember(3)]
		public List<ZorkBattleTeamInfo> ZorkBattleTeamList = new List<ZorkBattleTeamInfo>();

		[ProtoMember(4)]
		public int BossBuffID;
	}
}
