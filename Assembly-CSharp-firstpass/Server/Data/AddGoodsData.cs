using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AddGoodsData
	{
		[ProtoMember(1)]
		public int roleID;

		[ProtoMember(2)]
		public int id;

		[ProtoMember(3)]
		public int goodsID;

		[ProtoMember(4)]
		public int forgeLevel;

		[ProtoMember(5)]
		public int quality;

		[ProtoMember(6)]
		public int goodsNum;

		[ProtoMember(7)]
		public int binding;

		[ProtoMember(8)]
		public int site;

		[ProtoMember(9)]
		public string jewellist = string.Empty;

		[ProtoMember(10)]
		public int newHint;

		[ProtoMember(11)]
		public string newEndTime = string.Empty;

		[ProtoMember(12)]
		public int addPropIndex;

		[ProtoMember(13)]
		public int bornIndex;

		[ProtoMember(14)]
		public int lucky;

		[ProtoMember(15)]
		public int strong;

		[ProtoMember(16)]
		public int ExcellenceProperty;

		[ProtoMember(17)]
		public int nAppendPropLev;

		[ProtoMember(18)]
		public int ChangeLifeLevForEquip;

		[ProtoMember(19)]
		public int bagIndex;

		[ProtoMember(20)]
		public List<int> washProps;

		[ProtoMember(21)]
		public List<int> ElementhrtsProps;

		[ProtoMember(22)]
		public int juHunLevel;

		[ProtoMember(23)]
		public int InsureCount;

		[ProtoMember(24)]
		public int PackUp;

		[ProtoMember(25)]
		public string prop;
	}
}
