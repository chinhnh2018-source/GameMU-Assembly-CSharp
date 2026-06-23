using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Network.Tools;

namespace HSGameEngine.GameEngine.Logic
{
	public class ActionCoolDownMgr
	{
		public static void AddAction(string actionName)
		{
			long correctLocalTime = TimeManager.GetCorrectLocalTime();
			ActionCoolDownMgr._ActionDict[actionName] = correctLocalTime;
		}

		public static bool FindAction(string actionName, long maxTicks)
		{
			long num = 0L;
			return ActionCoolDownMgr._ActionDict.TryGetValue(actionName, ref num) && num + maxTicks >= TimeManager.GetCorrectLocalTime();
		}

		private static Dictionary<string, long> _ActionDict = new Dictionary<string, long>();
	}
}
