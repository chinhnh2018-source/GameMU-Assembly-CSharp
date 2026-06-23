using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ServerListData
	{
		[ProtoMember(1)]
		public int RetCode;

		[ProtoMember(2)]
		public int RolesCount;

		[ProtoMember(3)]
		public List<LineData> LineDataList;
	}
}
