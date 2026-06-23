using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientGetOrderData
	{
		[ProtoMember(1)]
		public string strPlatform;

		[ProtoMember(2)]
		public string strUserID;

		[ProtoMember(3)]
		public string strMoney;

		[ProtoMember(4)]
		public int nServerID;

		[ProtoMember(5)]
		public long lTime;

		[ProtoMember(6)]
		public string roleName;

		[ProtoMember(7)]
		public int roleId;

		[ProtoMember(8)]
		public string strMD5;

		[ProtoMember(9)]
		public string ZhiGouID = "0";

		[ProtoMember(10)]
		public string ProductId = string.Empty;

		[ProtoMember(11)]
		public string strAppId = string.Empty;

		[ProtoMember(12)]
		public string tmskchannel = string.Empty;

		[ProtoMember(13)]
		public string roleInfo = string.Empty;

		[ProtoMember(14)]
		public string deviceInfo = string.Empty;

		[ProtoMember(15)]
		public string firstId = string.Empty;
	}
}
