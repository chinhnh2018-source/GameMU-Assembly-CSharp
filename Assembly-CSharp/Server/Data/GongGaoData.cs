using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class GongGaoData
	{
		[ProtoMember(1)]
		public int nHaveGongGao;

		[ProtoMember(2)]
		public int nLianXuLoginReward;

		[ProtoMember(3)]
		public int nLeiJiLoginReward;

		[ProtoMember(4)]
		public string strGongGaoInfo = string.Empty;
	}
}
