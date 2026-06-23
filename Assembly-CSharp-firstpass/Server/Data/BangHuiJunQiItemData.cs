using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiJunQiItemData
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string QiName = string.Empty;

		[ProtoMember(3)]
		public int QiLevel;
	}
}
