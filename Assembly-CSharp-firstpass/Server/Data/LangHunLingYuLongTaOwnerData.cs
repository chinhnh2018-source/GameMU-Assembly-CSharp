using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LangHunLingYuLongTaOwnerData
	{
		[ProtoMember(1)]
		public string OwnerBHName = string.Empty;

		[ProtoMember(2)]
		public int OwnerBHid;

		[ProtoMember(3)]
		public int OwnerBHZoneId;
	}
}
