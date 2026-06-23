using System;
using System.Collections.Generic;
using Server.Data;

namespace HSGameEngine.GameFramework.Logic
{
	public class XuanFuServerData
	{
		public ZtBuffServerInfo RecommendServer;

		public ZtBuffServerInfo LastServer;

		public List<ZtBuffServerInfo> RecordServerInfos;

		public List<ZtBuffServerInfo> RecommendServerInfos;

		public ZtBuffServerListData ServerListData;

		public int ClientInfo;
	}
}
