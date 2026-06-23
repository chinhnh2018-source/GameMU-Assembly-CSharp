using System;
using System.Collections.Generic;

namespace HSGameEngine.GameEngine.SilverLight
{
	public static class DispatcherTimerDriver
	{
		public static void AddTimer(DispatcherTimer timer)
		{
			DispatcherTimerDriver.TimersList.Add(timer);
		}

		public static void RemoveTimer(DispatcherTimer timer)
		{
			DispatcherTimerDriver.TimersList.Remove(timer);
		}

		public static bool RemoveTimer(string name)
		{
			bool result = false;
			int count = DispatcherTimerDriver.TimersList.Count;
			for (int i = count - 1; i >= 0; i--)
			{
				DispatcherTimer dispatcherTimer = DispatcherTimerDriver.TimersList[i];
				if (dispatcherTimer.Name == name)
				{
					DispatcherTimerDriver.TimersList.Remove(dispatcherTimer);
					result = true;
				}
			}
			return result;
		}

		public static void ClearAll()
		{
			DispatcherTimerDriver.TimersList.Clear();
		}

		public static void ExecuteTimers()
		{
			int count = DispatcherTimerDriver.TimersList.Count;
			for (int i = 0; i < count; i++)
			{
				try
				{
					DispatcherTimerDriver.TimersList[i].ExecuteTimer();
				}
				catch (Exception ex)
				{
					MUDebug.LogException(ex);
				}
			}
		}

		private static List<DispatcherTimer> TimersList = new List<DispatcherTimer>();
	}
}
