using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class SpriteNotifyEventArgs : EventArgs
	{
		public int RoleID { get; set; }

		public GSpriteTypes SpriteType { get; set; }

		public bool ShowDlg { get; set; }

		public int ExtensionID { get; set; }
	}
}
