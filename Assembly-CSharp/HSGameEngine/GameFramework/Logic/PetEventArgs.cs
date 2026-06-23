using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class PetEventArgs : EventArgs
	{
		public int StepType { get; set; }

		public int ID;
	}
}
