using System;
using Server.Data;

namespace HSGameEngine.GameEngine.Logic
{
	public class GoodsMovingEndEventArgs : EventArgs
	{
		public int ListBoxType { get; set; }

		public GoodsData GoodsDataItem { get; set; }

		public object ClickMouseButtonEventArgs { get; set; }

		public bool Handled { get; set; }
	}
}
