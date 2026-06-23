using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZtBuffServerListData
	{
		[ProtoMember(1)]
		public List<FistLevelServerListData> listServerData = new List<FistLevelServerListData>();

		[ProtoMember(2)]
		public bool IsAllPause;

		[ProtoMember(3)]
		public int ServerCount;

		[ProtoMember(4)]
		public string strMaintainTxt;

		[ProtoMember(5)]
		public string strMaintainStarTime;

		[ProtoMember(6)]
		public string strMaintainTerminalTime;
	}
}
