using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class ExecWordsFilterEventArgs : EventArgs
	{
		public int ret { get; set; }

		public int is_lost { get; set; }

		public int is_dirty { get; set; }

		public string msg { get; set; }
	}
}
