using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class QingGongYanInfo
	{
		public bool IfBanTime(DateTime time)
		{
			int num = (int)time.DayOfWeek;
			if (num == 0)
			{
				num = 7;
			}
			foreach (string text in this.ProhibitedTimeList)
			{
				string[] array = this.ProhibitedTimeList[0].Split(new char[]
				{
					','
				});
				if (Convert.ToInt32(array[0]) == num)
				{
					DateTime t = DateTime.Parse(array[1]);
					DateTime t2 = DateTime.Parse(array[2]);
					if (time >= t && time <= t2)
					{
						return true;
					}
				}
			}
			return false;
		}

		public int Index;

		public int NpcID;

		public int MapCode;

		public int X;

		public int Y;

		public int Direction;

		public List<string> ProhibitedTimeList = new List<string>();

		public string BeginTime;

		public string OverTime;

		public int FunctionID;

		public int HoldBindJinBi;

		public int TotalNum;

		public int SingleNum;

		public int JoinBindJinBi;

		public int ExpAward;

		public int XingHunAward;

		public int ZhanGongAward;

		public int ZuanShiCoe;
	}
}
