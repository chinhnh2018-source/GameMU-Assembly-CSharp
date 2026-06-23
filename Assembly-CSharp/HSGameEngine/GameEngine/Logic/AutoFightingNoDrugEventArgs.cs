using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class AutoFightingNoDrugEventArgs : EventArgs
	{
		public int DrugType { get; set; }

		public bool isAutoFight { get; set; }

		public bool autoGoBack { get; set; }

		public int state { get; set; }

		public int AutoID { get; set; }

		public int RoleID { get; set; }
	}
}
