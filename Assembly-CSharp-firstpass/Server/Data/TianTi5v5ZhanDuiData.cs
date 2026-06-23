using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class TianTi5v5ZhanDuiData
	{
		[ProtoMember(1)]
		public int ZhanDuiID;

		[ProtoMember(2)]
		public string XuanYan;

		[ProtoMember(3)]
		public string ZhanDuiName;

		[ProtoMember(4)]
		public int LeaderRoleID;

		[ProtoMember(5)]
		public int DuanWeiId;

		[ProtoMember(6)]
		public int DuanWeiJiFen;

		[ProtoMember(7)]
		public int DuanWeiRank;

		[ProtoMember(8)]
		public long ZhanDouLi;

		[ProtoMember(9)]
		public int LianSheng;

		[ProtoMember(10)]
		public int SuccessCount;

		[ProtoMember(11)]
		public int FightCount;

		[ProtoMember(12)]
		public int MonthDuanWeiRank;

		[ProtoMember(13)]
		public List<TianTi5v5ZhanDuiRoleData> teamerList = new List<TianTi5v5ZhanDuiRoleData>();

		[ProtoMember(14)]
		public string TeamerRidList;

		[ProtoMember(15)]
		public DateTime LastFightTime;

		[ProtoMember(16)]
		public string LeaderRoleName;
	}
}
