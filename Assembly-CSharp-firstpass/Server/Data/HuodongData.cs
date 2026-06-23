using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HuodongData
	{
		[ProtoMember(1)]
		public string LastWeekID = string.Empty;

		[ProtoMember(2)]
		public string LastDayID = string.Empty;

		[ProtoMember(3)]
		public int LoginNum;

		[ProtoMember(4)]
		public int NewStep;

		[ProtoMember(5)]
		public long StepTime;

		[ProtoMember(6)]
		public int LastMTime;

		[ProtoMember(7)]
		public string CurMID = string.Empty;

		[ProtoMember(8)]
		public int CurMTime;

		[ProtoMember(9)]
		public int SongLiID;

		[ProtoMember(10)]
		public int LoginGiftState;

		[ProtoMember(11)]
		public int OnlineGiftState;

		[ProtoMember(12)]
		public int LastLimitTimeHuoDongID;

		[ProtoMember(13)]
		public int LastLimitTimeDayID;

		[ProtoMember(14)]
		public int LimitTimeLoginNum;

		[ProtoMember(15)]
		public int LimitTimeGiftState;

		[ProtoMember(16)]
		public int EveryDayOnLineAwardStep;

		[ProtoMember(17)]
		public int GetEveryDayOnLineAwardDayID;

		[ProtoMember(18)]
		public int SeriesLoginGetAwardStep;

		[ProtoMember(19)]
		public int SeriesLoginAwardDayID;

		[ProtoMember(20)]
		public string SeriesLoginAwardGoodsID = string.Empty;

		[ProtoMember(21)]
		public string EveryDayOnLineAwardGoodsID = string.Empty;
	}
}
