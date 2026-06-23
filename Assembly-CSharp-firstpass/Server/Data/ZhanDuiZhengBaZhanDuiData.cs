using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class ZhanDuiZhengBaZhanDuiData
	{
		[ProtoMember(1)]
		public int Grade;

		[ProtoMember(2)]
		public int Group;

		[ProtoMember(3)]
		public int State;

		[ProtoMember(4)]
		public int ZhanDuiID;

		[ProtoMember(5)]
		public int ZoneId;

		[ProtoMember(6)]
		public string ZhanDuiName;

		[ProtoMember(7)]
		public int DuanWeiId;

		[ProtoMember(8)]
		public int DuanWeiRank;

		[ProtoMember(9)]
		public int ZhanLi;

		[ProtoMember(11)]
		public List<RoleOccuNameZhanLi> MemberList;
	}
}
