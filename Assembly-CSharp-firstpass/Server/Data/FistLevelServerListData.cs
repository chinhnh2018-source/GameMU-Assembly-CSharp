using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class FistLevelServerListData
	{
		[ProtoMember(1)]
		public List<SecondLevelServerListData> listServerData = new List<SecondLevelServerListData>();

		[ProtoMember(2)]
		public string strFirstLevelServerName = string.Empty;

		[ProtoMember(3)]
		public int nFirstLevelServerID;
	}
}
