using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZtBuffServerListDataEx
	{
		[ProtoMember(1)]
		public List<ZtBuffServerInfo> ListServerData = new List<ZtBuffServerInfo>();

		[ProtoMember(2)]
		public List<ZtBuffServerInfo> RecommendListServerData = new List<ZtBuffServerInfo>();

		[ProtoMember(3)]
		public int ClientInfo;
	}
}
