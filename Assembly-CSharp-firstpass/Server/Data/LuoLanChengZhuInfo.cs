using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class LuoLanChengZhuInfo
	{
		[ProtoMember(1)]
		public int BHID;

		[ProtoMember(2)]
		public string BHName = string.Empty;

		[ProtoMember(3)]
		public int ZoneID;

		[ProtoMember(4)]
		public List<int> ZhiWuList = new List<int>();

		[ProtoMember(5)]
		public List<RoleData4Selector> RoleInfoList = new List<RoleData4Selector>();

		[ProtoMember(6)]
		public bool isGetReward;
	}
}
