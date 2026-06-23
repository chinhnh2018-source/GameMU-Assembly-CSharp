using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TianTi5v5ZhanDuiRoleData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public int RoleOcc;

		[ProtoMember(5)]
		public string RoleName = string.Empty;

		[ProtoMember(6)]
		public long ZhanLi;

		[ProtoMember(7)]
		public int ZhuanSheng;

		[ProtoMember(8)]
		public int Level;

		[ProtoMember(9)]
		public byte[] ModelData;

		[ProtoMember(10)]
		public int OnlineState;

		[ProtoMember(11)]
		public byte[] PlayerJingJiMirrorData;

		[ProtoMember(12)]
		public int ZoneID;

		[ProtoMember(13)]
		public int TodayFightCount;

		[ProtoMember(14)]
		public int LastFightDayId;

		[ProtoMember(15)]
		public int MonthFigntCount;

		[ProtoMember(16)]
		public int MonthAwardsFlags;

		[ProtoMember(17)]
		public DateTime FetchMonthDuanWeiRankAwardsTime;
	}
}
