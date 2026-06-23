using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ListRolesData
	{
		[ProtoMember(1)]
		public int StartIndex;

		[ProtoMember(2)]
		public int TotalRolesCount;

		[ProtoMember(3)]
		public int PageRolesCount;

		[ProtoMember(4)]
		public List<SearchRoleData> SearchRoleDataList;
	}
}
