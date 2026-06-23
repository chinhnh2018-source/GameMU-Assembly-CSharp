using System;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class DispatcherTimer : IDisposable
	{
		public DispatcherTimer(string name)
		{
			this._Name = name;
			DispatcherTimerDriver.AddTimer(this);
			this._beDisposed = false;
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				this._Name = value;
			}
		}

		public TimeSpan Interval
		{
			get
			{
				return this._Interval;
			}
			set
			{
				this._Interval = value;
			}
		}

		public void Start()
		{
			this._LastTicks = DateTime.Now.Ticks;
		}

		public void Stop()
		{
		}

		public void Dispose()
		{
			if (!this._beDisposed)
			{
				DispatcherTimerDriver.RemoveTimer(this);
				this._beDisposed = true;
			}
		}

		public void ExecuteTimer()
		{
			long ticks = DateTime.Now.Ticks;
			if (ticks - this._LastTicks < this._Interval.Ticks)
			{
				return;
			}
			this._LastTicks = ticks;
			if (this.Tick != null)
			{
				this.Tick(this, EventArgs.Empty);
			}
		}

		~DispatcherTimer()
		{
			this.Dispose();
		}

		private bool _beDisposed;

		public DispatcherTimerEventHandler Tick;

		private string _Name = Global.GetLang("未知");

		private long _LastTicks;

		private TimeSpan _Interval = TimeSpan.Zero;
	}
}
