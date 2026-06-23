using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiLingDiInfoData
	{
		[ProtoMember(1)]
		public int LingDiID;

		[ProtoMember(2)]
		public int BHID;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public string BHName = string.Empty;

		[ProtoMember(5)]
		public int LingDiTax;

		[ProtoMember(6)]
		public int TakeDayID;

		[ProtoMember(7)]
		public int TakeDayNum;

		[ProtoMember(8)]
		public int YestodayTax;

		[ProtoMember(9)]
		public int TaxDayID;

		[ProtoMember(10)]
		public int TodayTax;

		[ProtoMember(11)]
		public int TotalTax;

		[ProtoMember(12)]
		public string WarRequest = string.Empty;

		[ProtoMember(13)]
		public int AwardFetchDay;
	}
}
