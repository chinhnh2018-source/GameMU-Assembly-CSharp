using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ChengJiuData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public long ChengJiuPoints;

		[ProtoMember(3)]
		public long TotalKilledMonsterNum;

		[ProtoMember(4)]
		public long TotalLoginNum;

		[ProtoMember(5)]
		public int ContinueLoginNum;

		[ProtoMember(6)]
		public List<ushort> ChengJiuFlags;

		[ProtoMember(7)]
		public int NowCompletedChengJiu;

		[ProtoMember(8)]
		public long TotalKilledBossNum;

		[ProtoMember(9)]
		public long CompleteNormalCopyMapCount;

		[ProtoMember(10)]
		public long CompleteHardCopyMapCount;

		[ProtoMember(11)]
		public long CompleteDifficltCopyMapCount;

		[ProtoMember(12)]
		public long GuildChengJiu;

		[ProtoMember(13)]
		public long JunXianChengJiu;
	}
}
