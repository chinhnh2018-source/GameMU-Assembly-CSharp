using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class RoleDailyData
	{
		[ProtoMember(1)]
		public int ExpDayID;

		[ProtoMember(2)]
		public int TodayExp;

		[ProtoMember(3)]
		public int LingLiDayID;

		[ProtoMember(4)]
		public int TodayLingLi;

		[ProtoMember(5)]
		public int KillBossDayID;

		[ProtoMember(6)]
		public int TodayKillBoss;

		[ProtoMember(7)]
		public int FuBenDayID;

		[ProtoMember(8)]
		public int TodayFuBenNum;

		[ProtoMember(9)]
		public int WuXingDayID;

		[ProtoMember(10)]
		public int WuXingNum;
	}
}
