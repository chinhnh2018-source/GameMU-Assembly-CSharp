using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class NextStepEventArgs : EventArgs
	{
		public int StepType { get; set; }

		public int ID;

		public object Tag;
	}
}
