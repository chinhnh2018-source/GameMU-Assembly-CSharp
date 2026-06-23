using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class MallSaleData
	{
		[ProtoMember(1)]
		public string MallXmlString = string.Empty;

		[ProtoMember(2)]
		public string MallTabXmlString = string.Empty;

		[ProtoMember(3)]
		public string QiangGouXmlString = string.Empty;
	}
}
