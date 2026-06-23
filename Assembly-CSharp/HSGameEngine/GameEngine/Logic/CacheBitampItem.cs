using System;
using System.IO;
using HSGameEngine.GameEngine.Network.Tools;

namespace HSGameEngine.GameEngine.Logic
{
	public class CacheBitampItem
	{
		public long LastAccessTick
		{
			get
			{
				return this._LastAccessTick;
			}
		}

		public Stream[] BitmapList
		{
			get
			{
				this._LastAccessTick = TimeManager.GetCorrectLocalTime();
				return this._BitmapList;
			}
			set
			{
				this._BitmapList = value;
			}
		}

		private long _LastAccessTick;

		private Stream[] _BitmapList;
	}
}
