using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengBaUnionGroupData
	{
		[ProtoMember(1)]
		public int UnionGroup;

		[ProtoMember(2)]
		public List<ZhengBaSupportAnalysisData> SupportDatas;

		[ProtoMember(3)]
		public List<ZhengBaSupportLog> SupportLogs;

		[ProtoMember(4)]
		public List<ZhengBaSupportFlagData> SupportFlags;

		[ProtoMember(5)]
		public int WinZhengBaPoint;
	}
}
