using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BossData
	{
		[ProtoMember(1)]
		public int MonsterID;

		[ProtoMember(2)]
		public int ExtensionID;

		[ProtoMember(3)]
		public string KillMonsterName = string.Empty;

		[ProtoMember(4)]
		public int KillerOnline;

		[ProtoMember(5)]
		public string NextTime = string.Empty;
	}
}
