using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	internal class Timer
	{
		public Timer(int time = 0)
		{
		}

		public bool running { get; set; }

		public void start()
		{
		}

		public void stop()
		{
		}

		public bool addEventListener(string id, Timer.TimerEventHnadler timeEventHandler)
		{
			return true;
		}

		public bool removeEventListener(string id, Timer.TimerEventHnadler timeEventHandler)
		{
			return true;
		}

		public Timer.TimerEventHnadler timeEventHandler;

		public delegate void TimerEventHnadler(EventArgs e);
	}
}
