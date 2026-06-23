using System;
using System.Collections.Generic;
using ProtoBuf;

namespace HSGameEngine.GameFramework.Logic
{
	[ProtoContract]
	public class CopyWolfScoreData
	{
		[ProtoMember(1)]
		public int Wave;

		[ProtoMember(2)]
		public long EndTime;

		[ProtoMember(3)]
		public int FortLifeNow;

		[ProtoMember(4)]
		public int FortLifeMax;

		[ProtoMember(5)]
		public Dictionary<int, int> RoleMonsterScore = new Dictionary<int, int>();

		[ProtoMember(6)]
		public int MonsterCount;
	}
}
