using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ClientAppealData
	{
		[ProtoMember(1)]
		public string strUserID;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strMD5;
	}
}
