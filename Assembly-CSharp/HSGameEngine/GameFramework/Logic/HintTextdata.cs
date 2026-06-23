using System;
using HSGameEngine.GameEngine.Logic;

namespace HSGameEngine.GameFramework.Logic
{
	public class HintTextdata
	{
		public HintTextdata(string msg)
		{
			this.Msg = msg;
			this.Ticks = Global.GetCorrectLocalTime();
		}

		public long Ticks;

		public string Msg;
	}
}
