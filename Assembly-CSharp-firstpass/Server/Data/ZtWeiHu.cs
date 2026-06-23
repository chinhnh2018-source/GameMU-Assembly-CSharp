using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	public class ZtWeiHu
	{
		[ProtoMember(1)]
		public bool IsAllPause;

		[ProtoMember(2)]
		public List<FistLevelServerListData> FirstWeiHuList = new List<FistLevelServerListData>();

		[ProtoMember(3)]
		public string MaintainTxt = string.Empty;

		[ProtoMember(4)]
		public string MaintainStarTime = string.Empty;

		[ProtoMember(5)]
		public string MaintainTerminalTime = string.Empty;
	}
}
