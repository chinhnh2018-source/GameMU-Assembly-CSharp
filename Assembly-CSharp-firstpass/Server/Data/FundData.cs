using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FundData
	{
		[ProtoMember(1, IsRequired = true)]
		public bool IsOpen;

		[ProtoMember(2, IsRequired = true)]
		public int State;

		[ProtoMember(3, IsRequired = true)]
		public int FundType;

		[ProtoMember(4, IsRequired = true)]
		public Dictionary<int, FundItem> FundDic = new Dictionary<int, FundItem>();
	}
}
