using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerAppealData
	{
		[ProtoMember(1)]
		public string strState = string.Empty;

		[ProtoMember(2)]
		public long lTime;

		[ProtoMember(3)]
		public string strResult = string.Empty;

		[ProtoMember(4)]
		public string strMD5;
	}
}
