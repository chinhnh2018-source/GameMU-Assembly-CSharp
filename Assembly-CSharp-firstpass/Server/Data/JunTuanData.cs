using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract, Serializable]
	public class JunTuanData
	{
		[ProtoMember(1)]
		public int JunTuanId;

		[ProtoMember(2)]
		public string JunTuanName;

		[ProtoMember(3)]
		public int LeaderZoneId;

		[ProtoMember(4)]
		public string LeaderName;

		[ProtoMember(5)]
		public int BangHuiNum;

		[ProtoMember(6)]
		public int LingDi;

		[ProtoMember(7)]
		public int Point;

		[ProtoMember(8)]
		public int WeekPoint;

		[ProtoMember(9)]
		public int WeekRank;

		[ProtoMember(11)]
		public string Bulletin;

		[ProtoMember(12)]
		public int RequestCount;

		[NonSerialized]
		public int PointCostDay;
	}
}
