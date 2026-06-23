using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientGetQQOrderData
	{
		[ProtoMember(1)]
		public string strPlatform;

		[ProtoMember(2)]
		public string strOpenid;

		[ProtoMember(3)]
		public string strOpenkey;

		[ProtoMember(4)]
		public string strPayToken;

		[ProtoMember(5)]
		public string strPayitem;

		[ProtoMember(6)]
		public string strGoodsMeta;

		[ProtoMember(7)]
		public string strGoodsURL;

		[ProtoMember(8)]
		public string strPF;

		[ProtoMember(9)]
		public string strPFKey;

		[ProtoMember(10)]
		public string strMoney;

		[ProtoMember(11)]
		public int nServerID;

		[ProtoMember(12)]
		public long lTime;

		[ProtoMember(13)]
		public string strMD5;
	}
}
