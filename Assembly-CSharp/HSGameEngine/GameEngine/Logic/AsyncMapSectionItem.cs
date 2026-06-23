using System;

namespace HSGameEngine.GameEngine.Logic
{
	public class AsyncMapSectionItem
	{
		public int Index
		{
			get
			{
				return this._Index;
			}
			set
			{
				this._Index = value;
			}
		}

		public int X
		{
			get
			{
				return this._X;
			}
			set
			{
				this._X = value;
			}
		}

		public int Y
		{
			get
			{
				return this._Y;
			}
			set
			{
				this._Y = value;
			}
		}

		private int _Index;

		private int _X;

		private int _Y;
	}
}
