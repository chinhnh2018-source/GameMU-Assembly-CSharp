using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HolyItemData
	{
		[ProtoMember(1)]
		public sbyte m_sType;

		[ProtoMember(2)]
		public Dictionary<sbyte, HolyItemPartData> m_PartArray;
	}
}
