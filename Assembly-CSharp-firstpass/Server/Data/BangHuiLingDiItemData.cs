using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiLingDiItemData
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
		public string WarRequest = string.Empty;

		[ProtoMember(7)]
		public int AwardFetchDay = -1;
	}
}
