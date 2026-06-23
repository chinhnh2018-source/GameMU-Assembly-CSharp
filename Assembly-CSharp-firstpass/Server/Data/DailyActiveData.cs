using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class DailyActiveData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public long DailyActiveValues;

		[ProtoMember(3)]
		public long TotalKilledMonsterCount;

		[ProtoMember(4)]
		public long DailyActiveTotalLoginCount;

		[ProtoMember(5)]
		public int DailyActiveOnLineTimer;

		[ProtoMember(6)]
		public List<ushort> DailyActiveInforFlags;

		[ProtoMember(7)]
		public int NowCompletedDailyActiveID;

		[ProtoMember(8)]
		public long TotalKilledBossCount;

		[ProtoMember(9)]
		public int PassNormalCopySceneNum;

		[ProtoMember(10)]
		public int PassHardCopySceneNum;

		[ProtoMember(11)]
		public int PassDifficultCopySceneNum;

		[ProtoMember(12)]
		public int BuyItemInMall;

		[ProtoMember(13)]
		public int CompleteDailyTaskCount;

		[ProtoMember(14)]
		public int CompleteBloodCastleCount;

		[ProtoMember(15)]
		public int CompleteDaimonSquareCount;

		[ProtoMember(16)]
		public int CompleteBattleCount;

		[ProtoMember(17)]
		public int EquipForge;

		[ProtoMember(18)]
		public int EquipAppend;

		[ProtoMember(19)]
		public int ChangeLife;

		[ProtoMember(20)]
		public int MergeFruit;

		[ProtoMember(21)]
		public int GetAwardFlag;
	}
}
