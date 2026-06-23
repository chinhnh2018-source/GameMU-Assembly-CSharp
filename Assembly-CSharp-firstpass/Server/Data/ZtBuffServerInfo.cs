using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZtBuffServerInfo
	{
		[ProtoMember(1)]
		public int nServerOrder;

		[ProtoMember(2)]
		public int nServerID;

		[ProtoMember(3)]
		public int nOnlineNum;

		[ProtoMember(4)]
		public List<int> listMapOnline = new List<int>();

		[ProtoMember(5)]
		public string strServerName = string.Empty;

		[ProtoMember(6)]
		public string strStartTime = string.Empty;

		[ProtoMember(7)]
		public int nStatus;

		[ProtoMember(8)]
		public string strURL;

		[ProtoMember(9)]
		public int nServerPort;

		[ProtoMember(10)]
		public string strMaintainTxt;

		[ProtoMember(11)]
		public string strMaintainStarTime;

		[ProtoMember(12)]
		public string strMaintainTerminalTime;

		[ProtoMember(13)]
		public int nFirstLevelServerID;

		[ProtoMember(14)]
		public int nSecondLevelServerID;

		[ProtoMember(15)]
		public int nZoneID;
	}
}
