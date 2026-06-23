using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class BangHuiListData
	{
		[ProtoMember(1)]
		public int TotalBangHuiItemNum;

		[ProtoMember(2)]
		public List<BangHuiItemData> BangHuiItemDataList;
	}
}
