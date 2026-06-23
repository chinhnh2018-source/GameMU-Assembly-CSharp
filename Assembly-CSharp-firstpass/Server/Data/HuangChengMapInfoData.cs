using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HuangChengMapInfoData
	{
		[ProtoMember(1)]
		public long FightingEndTime;

		[ProtoMember(2)]
		public int HuangDiRoleID;

		[ProtoMember(3)]
		public string HuangDiRoleName = string.Empty;

		[ProtoMember(4)]
		public string HuangDiBHName = string.Empty;

		[ProtoMember(5)]
		public int FightingState;

		[ProtoMember(6)]
		public string NextBattleTime = string.Empty;

		[ProtoMember(7)]
		public int WangZuBHid = -1;
	}
}
