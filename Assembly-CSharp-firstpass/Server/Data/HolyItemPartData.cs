using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class HolyItemPartData
	{
		[ProtoMember(1)]
		public sbyte m_sSuit;

		[ProtoMember(2)]
		public int m_nSlice;
	}
}
