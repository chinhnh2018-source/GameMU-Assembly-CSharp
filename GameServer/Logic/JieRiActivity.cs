using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class JieRiActivity : Activity
	{
		public override bool InActivityTime()
		{
			DateTime jieriStartDay = Global.GetJieriStartDay();
			DateTime t = TimeUtil.NowDateTime();
			return t >= jieriStartDay && t <= jieriStartDay.AddDays((double)Global.GetJieriDaysNum()) && base.InActivityTime();
		}

		public override bool InAwardTime()
		{
			DateTime jieriStartDay = Global.GetJieriStartDay();
			DateTime t = TimeUtil.NowDateTime();
			return t >= jieriStartDay && t <= jieriStartDay.AddDays((double)Global.GetJieriDaysNum()) && base.InAwardTime();
		}

		public new bool CanGiveAward()
		{
			return this.InAwardTime();
		}
	}
}
