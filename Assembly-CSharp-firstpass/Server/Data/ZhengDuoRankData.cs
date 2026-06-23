using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengDuoRankData
	{
		[ProtoMember(1)]
		public int Rank1;

		[ProtoMember(2)]
		public int Rank2;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int Bhid;

		[ProtoMember(5)]
		public int ZoneId;

		[ProtoMember(6)]
		public string BhName;

		[ProtoMember(7)]
		public int BhLevel;

		[ProtoMember(8)]
		public long ZhanLi;

		[ProtoMember(9)]
		public int UsedMillisecond;

		[ProtoMember(10)]
		public int ServerID;

		[ProtoMember(11)]
		public int Week;

		[ProtoMember(12)]
		public int Lose;
	}
}
