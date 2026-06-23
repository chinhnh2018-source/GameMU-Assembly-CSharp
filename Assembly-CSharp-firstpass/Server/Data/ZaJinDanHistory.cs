using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZaJinDanHistory
	{
		[ProtoMember(1)]
		public int RoleID;

		[ProtoMember(2)]
		public string RoleName = string.Empty;

		[ProtoMember(3)]
		public int TimesSelected;

		[ProtoMember(4)]
		public int UsedYuanBao;

		[ProtoMember(5)]
		public int UsedJinDan;

		[ProtoMember(6)]
		public int GainGoodsId;

		[ProtoMember(7)]
		public int GainGoodsNum;

		[ProtoMember(8)]
		public int GainGold;

		[ProtoMember(9)]
		public int GainYinLiang;

		[ProtoMember(10)]
		public int GainExp;

		[ProtoMember(11)]
		public string GoodPorp = string.Empty;

		[ProtoMember(12)]
		public string OperationTime = string.Empty;
	}
}
