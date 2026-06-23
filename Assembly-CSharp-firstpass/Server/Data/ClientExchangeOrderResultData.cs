using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientExchangeOrderResultData
	{
		[ProtoMember(1)]
		public string strPlatform;

		[ProtoMember(2)]
		public string strUserID;

		[ProtoMember(3)]
		public int nMoney;

		[ProtoMember(4)]
		public string strExchangeOrder;

		[ProtoMember(5)]
		public int nServerID;

		[ProtoMember(6)]
		public long lTime;

		[ProtoMember(7)]
		public byte[] recipent_data;

		[ProtoMember(8)]
		public string strMD5;

		[ProtoMember(9)]
		public string strIOSVer;

		[ProtoMember(10)]
		public string strTransactionId;

		[ProtoMember(11)]
		public string strNSLocaleCountryCode = string.Empty;

		[ProtoMember(12)]
		public string strNSLocaleCurrencyCode = string.Empty;

		[ProtoMember(13)]
		public string VersionCode = string.Empty;

		[ProtoMember(14)]
		public string FirstId = string.Empty;
	}
}
