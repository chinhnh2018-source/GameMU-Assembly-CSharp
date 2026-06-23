using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data.OldCopyTeam
{
	[ProtoContract]
	public class CopySearchTeamData
	{
		public CopySearchTeamData SimpleClone()
		{
			CopySearchTeamData copySearchTeamData = new CopySearchTeamData();
			copySearchTeamData.PageTeamsCount = this.PageTeamsCount;
			copySearchTeamData.StartIndex = this.StartIndex;
			copySearchTeamData.TotalTeamsCount = this.TotalTeamsCount;
			if (null != this.TeamDataList)
			{
				copySearchTeamData.TeamDataList = new List<CopyTeamData>();
				foreach (CopyTeamData copyTeamData in this.TeamDataList)
				{
					copySearchTeamData.TeamDataList.Add(copyTeamData.SimpleClone());
				}
			}
			return copySearchTeamData;
		}

		[ProtoMember(1)]
		public int StartIndex = 0;

		[ProtoMember(2)]
		public int TotalTeamsCount = 0;

		[ProtoMember(3)]
		public int PageTeamsCount = 0;

		[ProtoMember(4)]
		public List<CopyTeamData> TeamDataList = null;
	}
}
