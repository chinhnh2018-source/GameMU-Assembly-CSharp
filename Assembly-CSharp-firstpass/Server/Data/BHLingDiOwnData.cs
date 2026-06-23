using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BHLingDiOwnData
	{
		[ProtoMember(1)]
		public int LingDiID;

		[ProtoMember(2)]
		public int ZoneID;

		[ProtoMember(3)]
		public int BHID;

		[ProtoMember(4)]
		public string BHName = string.Empty;

		[ProtoMember(5)]
		public string BangQiName = string.Empty;

		[ProtoMember(6)]
		public int BangQiLevel;
	}
}
