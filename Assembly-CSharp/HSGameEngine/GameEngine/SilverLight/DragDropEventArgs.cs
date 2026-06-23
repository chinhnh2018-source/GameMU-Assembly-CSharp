using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class DragDropEventArgs : EventArgs
	{
		public bool Cancel { get; set; }

		public object DragSource;

		public object Data;

		public int stageX;

		public int stageY;
	}
}
