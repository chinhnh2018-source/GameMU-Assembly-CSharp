using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class IOSClientGetOrderData
	{
		[ProtoMember(1)]
		public string strPlatform = string.Empty;

		[ProtoMember(2)]
		public string strUserID = string.Empty;

		[ProtoMember(3)]
		public string strMoney = string.Empty;

		[ProtoMember(4)]
		public int nServerID = 1;

		[ProtoMember(5)]
		public long lTime;

		[ProtoMember(6)]
		public string strMD5 = string.Empty;

		[ProtoMember(7)]
		public string NSLocaleCountryCode = string.Empty;

		[ProtoMember(8)]
		public string NSLocaleIdentifier = string.Empty;

		[ProtoMember(9)]
		public string NSLocaleLanguageCode = string.Empty;

		[ProtoMember(10)]
		public string NSLocaleCurrencySymbol = string.Empty;

		[ProtoMember(11)]
		public string NSLocaleCurrencyCode = string.Empty;

		[ProtoMember(12)]
		public string NSLocaleCollatorIdentifier = string.Empty;

		[ProtoMember(13)]
		public string Md5IdxValue = string.Empty;

		[ProtoMember(14)]
		public string Md5IdyValue = string.Empty;

		[ProtoMember(15)]
		public string IdxEqual = string.Empty;

		[ProtoMember(16)]
		public string NotUseAgent = string.Empty;

		[ProtoMember(17)]
		public string VersionCode = string.Empty;

		[ProtoMember(18)]
		public string roleName;

		[ProtoMember(19)]
		public int roleId;

		[ProtoMember(20)]
		public string iOSVersion;

		[ProtoMember(21)]
		public string ZhiGouID;

		[ProtoMember(22)]
		public string Idfa;

		[ProtoMember(23)]
		public int FirstId;
	}
}
