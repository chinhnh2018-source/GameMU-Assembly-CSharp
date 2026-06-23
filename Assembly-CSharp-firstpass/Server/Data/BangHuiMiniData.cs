using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiMiniData
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string BHName = string.Empty;

		[ProtoMember(3)]
		public int ZoneID;
	}
}
