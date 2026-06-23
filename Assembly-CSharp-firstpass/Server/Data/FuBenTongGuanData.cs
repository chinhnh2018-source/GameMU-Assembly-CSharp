using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FuBenTongGuanData
	{
		[ProtoMember(1)]
		public int FuBenID;

		[ProtoMember(2)]
		public int TotalScore;

		[ProtoMember(3)]
		public int KillNum;

		[ProtoMember(4)]
		public int KillScore;

		[ProtoMember(5)]
		public int MaxKillScore;

		[ProtoMember(6)]
		public int UsedSecs;

		[ProtoMember(7)]
		public int TimeScore;

		[ProtoMember(8)]
		public int MaxTimeScore;

		[ProtoMember(9)]
		public int DieCount;

		[ProtoMember(10)]
		public int DieScore;

		[ProtoMember(11)]
		public int MaxDieScore;

		[ProtoMember(12)]
		public List<int> GoodsIDList;

		[ProtoMember(13)]
		public int AwardExp;

		[ProtoMember(14)]
		public int AwardJinBi;

		[ProtoMember(15)]
		public int AwardXingHun;

		[ProtoMember(16)]
		public int ResultMark;

		[ProtoMember(17)]
		public int MapCode;

		[ProtoMember(18)]
		public int AwardZhanGong;

		[ProtoMember(19)]
		public double AwardRate = 1.0;

		[ProtoMember(20)]
		public int TreasureEventID;

		[ProtoMember(21)]
		public int AwardMoJing;
	}
}
