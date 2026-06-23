using System;
using HSGameEngine.GameEngine.Teleport;

namespace HSGameEngine.GameEngine.Logic
{
	public class MapConversionByTeleportCodeEventArgs : EventArgs
	{
		public GTeleport Teleport { get; set; }
	}
}
