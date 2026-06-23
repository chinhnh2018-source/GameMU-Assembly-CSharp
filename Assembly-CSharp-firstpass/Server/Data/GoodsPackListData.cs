using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GoodsPackListData
	{
		[ProtoMember(1)]
		public int AutoID = -1;

		[ProtoMember(2)]
		public List<GoodsData> GoodsDataList;

		[ProtoMember(3)]
		public int OpenState = -1;

		[ProtoMember(4)]
		public int RetError;

		[ProtoMember(5)]
		public long LeftTicks;

		[ProtoMember(6)]
		public long PackTicks = -1L;
	}
}
