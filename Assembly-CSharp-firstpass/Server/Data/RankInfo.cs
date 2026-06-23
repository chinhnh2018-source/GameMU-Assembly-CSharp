using System;

namespace Server.Data
{
	public class RankInfo
	{
		public int RankType;

		public int RankMax;

		public int RankTopCount;

		public int RankRoleCount;

		public int RankListType;

		public int RankRefreshSpanType;

		public DateTime RankRefreshTime = DateTime.MaxValue;

		public int RankRefreshSecondTick;
	}
}
