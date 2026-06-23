using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
{
	[ProtoContract]
	public class CopyTeamData
	{
		public CopyTeamData SimpleClone()
		{
			return new CopyTeamData
			{
				TeamID = this.TeamID,
				LeaderRoleID = this.LeaderRoleID,
				StartTime = this.StartTime,
				GetThingOpt = this.GetThingOpt,
				SceneIndex = this.SceneIndex,
				FuBenSeqID = this.FuBenSeqID,
				MinZhanLi = this.MinZhanLi,
				AutoStart = this.AutoStart,
				TeamRoles = null,
				MemberCount = this.MemberCount,
				TeamName = this.TeamName,
				KFServerId = this.KFServerId,
				AutoKick = this.AutoKick
			};
		}

		[ProtoMember(1)]
		public long TeamID;

		[ProtoMember(2)]
		public int LeaderRoleID;

		[ProtoMember(3)]
		public List<CopyTeamMemberData> TeamRoles;

		[ProtoMember(4)]
		public long StartTime;

		[ProtoMember(5)]
		public int GetThingOpt;

		[ProtoMember(6)]
		public int SceneIndex;

		[ProtoMember(7)]
		public int FuBenSeqID;

		[ProtoMember(8)]
		public int MinZhanLi;

		[ProtoMember(9)]
		public bool AutoStart;

		[ProtoMember(10)]
		public int MemberCount;

		[ProtoMember(11)]
		public string TeamName;

		[ProtoMember(12)]
		public int KFServerId;

		[ProtoMember(15)]
		public int AutoKick;
	}
}
