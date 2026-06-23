using System;
using System.Collections.Generic;
using ProtoBuf;

namespace Server.Data
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
			if (this.TeamDataList != null)
			{
				copySearchTeamData.TeamDataList = new List<CopyTeamData>();
				for (int i = 0; i < this.TeamDataList.Count; i++)
				{
					copySearchTeamData.TeamDataList.Add(this.TeamDataList[i].SimpleClone());
				}
			}
			return copySearchTeamData;
		}

		[ProtoMember(1)]
		public int StartIndex;

		[ProtoMember(2)]
		public int TotalTeamsCount;

		[ProtoMember(3)]
		public int PageTeamsCount;

		[ProtoMember(4)]
		public List<CopyTeamData> TeamDataList;
	}
}
