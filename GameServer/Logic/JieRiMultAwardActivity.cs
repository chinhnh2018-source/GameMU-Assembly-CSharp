using System;
using System.Collections.Generic;

namespace GameServer.Logic
{
	public class JieRiMultAwardActivity : Activity
	{
		public JieRiMultConfig GetConfig(int type)
		{
			JieRiMultConfig result = null;
			if (this.activityDict.ContainsKey(type))
			{
				result = this.activityDict[type];
			}
			return result;
		}

		public Dictionary<int, JieRiMultConfig> activityDict = new Dictionary<int, JieRiMultConfig>();
	}
}
