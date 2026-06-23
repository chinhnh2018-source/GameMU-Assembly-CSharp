using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class JueXingShiData
	{
		[ProtoMember(1)]
		public int AttackEquip;

		[ProtoMember(2)]
		public int DefenseEquip;

		[ProtoMember(3)]
		public List<TaoZhuangData> TaoZhuangList;

		[ProtoMember(4)]
		public int JueXingJie;

		[ProtoMember(5)]
		public int JueXingJi;
	}
}
