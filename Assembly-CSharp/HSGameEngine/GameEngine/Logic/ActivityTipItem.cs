using System;
using System.Collections.Generic;
using HSGameEngine.GameFramework.Logic;

namespace HSGameEngine.GameEngine.Logic
{
	public class ActivityTipItem
	{
		public int type;

		public int ActiveChildCount;

		public ActivityTipItem Parent;

		public List<ActivityTipItem> Children = new List<ActivityTipItem>();

		public bool IsActive;

		public bool TaskRelative;

		public bool LevelRelative;

		public bool VIPRalative;

		public bool TimeRelative;

		public bool Hide;

		public int NeedTaskID;

		public int NeedLevel;

		public int NeedVIPLevel;

		public List<DateTime[]> TimeList;

		public ActivityTipEventHandler Handler;
	}
}
