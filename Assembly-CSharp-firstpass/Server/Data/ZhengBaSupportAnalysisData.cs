using System;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhengBaSupportAnalysisData
	{
		[ProtoMember(1)]
		public int UnionGroup;

		[ProtoMember(2)]
		public int Group;

		[ProtoMember(3)]
		public int TotalSupport;

		[ProtoMember(4)]
		public int TotalOppose;

		[ProtoMember(5)]
		public int TotalYaZhu;
	}
}
