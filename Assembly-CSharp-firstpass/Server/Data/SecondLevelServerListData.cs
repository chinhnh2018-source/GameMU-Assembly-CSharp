using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SecondLevelServerListData
	{
		[ProtoMember(1)]
		public List<ZtBuffServerInfo> listServerData = new List<ZtBuffServerInfo>();

		[ProtoMember(2)]
		public string strSecondtLevelServerName = string.Empty;

		[ProtoMember(3)]
		public int nSecondLevelServerID;

		[ProtoMember(4)]
		public int nFirstLevelServerID;
	}
}
