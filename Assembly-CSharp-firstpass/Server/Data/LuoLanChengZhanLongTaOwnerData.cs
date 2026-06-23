using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LuoLanChengZhanLongTaOwnerData
	{
		[ProtoMember(1)]
		public string OwnerBHName = string.Empty;

		[ProtoMember(2)]
		public int OwnerBHid = -1;
	}
}
