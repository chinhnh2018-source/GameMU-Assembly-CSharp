using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CoupleArenaPaiHangData
	{
		[ProtoMember(1)]
		public List<CoupleArenaCoupleJingJiData> PaiHang;
	}
}
