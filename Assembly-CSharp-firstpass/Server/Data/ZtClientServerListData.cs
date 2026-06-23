using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZtClientServerListData
	{
		[ProtoMember(1)]
		public long lTime;

		[ProtoMember(2)]
		public string strMD5 = string.Empty;

		[ProtoMember(3)]
		public string strUID = string.Empty;
	}
}
