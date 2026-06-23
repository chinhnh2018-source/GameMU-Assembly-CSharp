using System;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class JieRiMultConfig
	{
		public double GetMult()
		{
			double result;
			if (this.Effective == 0)
			{
				result = 0.0;
			}
			else
			{
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null == jieRiMultAwardActivity)
				{
					result = 0.0;
				}
				else if (!jieRiMultAwardActivity.InActivityTime())
				{
					result = 0.0;
				}
				else if (!this.InActivityTime())
				{
					result = 0.0;
				}
				else if (this.Multiplying < 1.0)
				{
					result = 0.0;
				}
				else
				{
					result = this.Multiplying;
				}
			}
			return result;
		}

		public bool InActivityTime()
		{
			JieriActivityConfig jieriActivityConfig = HuodongCachingMgr.GetJieriActivityConfig();
			bool result;
			if (null == jieriActivityConfig)
			{
				result = false;
			}
			else if (!jieriActivityConfig.InList(41))
			{
				result = false;
			}
			else
			{
				JieRiMultAwardActivity jieRiMultAwardActivity = HuodongCachingMgr.GetJieRiMultAwardActivity();
				if (null == jieRiMultAwardActivity)
				{
					result = false;
				}
				else if (!jieRiMultAwardActivity.InActivityTime())
				{
					result = false;
				}
				else
				{
					DateTime t = DateTime.Parse(this.StartDate);
					DateTime t2 = DateTime.Parse(this.EndDate);
					result = (TimeUtil.NowDateTime() >= t && TimeUtil.NowDateTime() <= t2);
				}
			}
			return result;
		}

		public int index;

		public int type;

		public double Multiplying;

		public int Effective;

		public string StartDate;

		public string EndDate;
	}
}
