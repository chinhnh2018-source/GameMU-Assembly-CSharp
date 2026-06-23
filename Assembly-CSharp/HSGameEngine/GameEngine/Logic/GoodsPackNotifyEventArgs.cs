using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class GoodsPackNotifyEventArgs : EventArgs
	{
		public int OwnerRoleID { get; set; }

		public int AutoID { get; set; }

		public int GoodsPackID { get; set; }

		public long ProduceTicks { get; set; }
	}
}
