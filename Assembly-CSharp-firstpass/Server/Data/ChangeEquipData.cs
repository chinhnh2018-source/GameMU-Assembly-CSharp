using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ChangeEquipData
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public GoodsData EquipGoodsData;

		[ProtoMember(3)]
		public WingData UsingWinData;
	}
}
