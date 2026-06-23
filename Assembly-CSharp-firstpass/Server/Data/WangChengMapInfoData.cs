using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class WangChengMapInfoData
	{
		[ProtoMember(1)]
		public long FightingEndTime;

		[ProtoMember(2)]
		public int FightingState;

		[ProtoMember(3)]
		public string NextBattleTime = string.Empty;

		[ProtoMember(4)]
		public string WangZuBHName = string.Empty;

		[ProtoMember(5)]
		public int WangZuBHid = -1;
	}
}
