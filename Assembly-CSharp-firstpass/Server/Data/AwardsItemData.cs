using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class AwardsItemData
	{
		[ProtoMember(1)]
		public int Occupation;

		[ProtoMember(2)]
		public int GoodsID;

		[ProtoMember(3)]
		public int GoodsNum;

		[ProtoMember(4)]
		public int Binding;

		[ProtoMember(5)]
		public int Level;

		[ProtoMember(6)]
		public int Quality;

		[ProtoMember(7)]
		public string EndTime = string.Empty;

		[ProtoMember(8)]
		public int BornIndex;

		[ProtoMember(9)]
		public int RoleSex;

		[ProtoMember(10)]
		public int AppendLev;

		[ProtoMember(11)]
		public int IsHaveLuckyProp;

		[ProtoMember(12)]
		public int ExcellencePorpValue;
	}
}
