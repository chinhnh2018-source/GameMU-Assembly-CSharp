using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class SearchTeamData
	{
		[ProtoMember(1)]
		public int StartIndex;

		[ProtoMember(2)]
		public int TotalTeamsCount;

		[ProtoMember(3)]
		public int PageTeamsCount;

		[ProtoMember(4)]
		public List<TeamData> TeamDataList;
	}
}
