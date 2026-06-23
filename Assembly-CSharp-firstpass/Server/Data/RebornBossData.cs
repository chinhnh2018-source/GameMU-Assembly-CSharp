using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class RebornBossData
	{
		[ProtoMember(1)]
		public int ExtensionID;

		[ProtoMember(2)]
		public string NextTime = string.Empty;

		[ProtoMember(3)]
		public int AwardExtensionID;

		[ProtoMember(4)]
		public int RankNum;

		[ProtoMember(5)]
		public int BossKill;
	}
}
