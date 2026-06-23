using System;
using System.Collections.Generic;
using HSGameEngine.GameFramework.Logic;

namespace HSGameEngine.GameEngine.Logic
{
	public class ActivityCustomTipManager
	{
		public static void RegActivityTipListener(int type, CustomeActivityTipEventHandler handler)
		{
			List<CustomeActivityTipEventHandler> list = null;
			if (!ActivityCustomTipManager.ActivityTipItemDict.TryGetValue(type, ref list))
			{
				list = new List<CustomeActivityTipEventHandler>();
				ActivityCustomTipManager.ActivityTipItemDict[type] = list;
			}
			ActivityTipItem activityTipItem = ActivityTipManager.GetActivityTipItem(type);
			if (activityTipItem != null)
			{
				handler(type, activityTipItem.IsActive);
			}
			if (list.IndexOf(handler) >= 0)
			{
				return;
			}
			list.Add(handler);
		}

		public static void UnRegActivityTipListener(int type, CustomeActivityTipEventHandler handler)
		{
			List<CustomeActivityTipEventHandler> list = null;
			if (!ActivityCustomTipManager.ActivityTipItemDict.TryGetValue(type, ref list))
			{
				return;
			}
			if (list.IndexOf(handler) >= 0)
			{
				list.Remove(handler);
			}
		}

		public static void ExcuteActivityEvent(int type, bool beActive)
		{
			List<CustomeActivityTipEventHandler> list = null;
			if (!ActivityCustomTipManager.ActivityTipItemDict.TryGetValue(type, ref list))
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				try
				{
					list[i](type, beActive);
				}
				catch (Exception ex)
				{
					MUDebug.LogError<string>(new string[]
					{
						ex.Message
					});
				}
			}
		}

		private static Dictionary<int, List<CustomeActivityTipEventHandler>> ActivityTipItemDict = new Dictionary<int, List<CustomeActivityTipEventHandler>>();
	}
}
