using System;

namespace HSGameEngine.GameEngine.SilverLight
{
	public class KeyboardEvent
	{
		public KeyboardEvent(string type, uint charCode = 0U, uint keyCode = 0U, uint keyLocation = 0U, bool ctrlKey = false, bool altKey = false, bool shiftKey = false)
		{
			this.mCharCode = charCode;
			this.mKeyCode = keyCode;
			this.mKeyLocation = keyLocation;
			this.mCtrlKey = ctrlKey;
			this.mAltKey = altKey;
			this.mShiftKey = shiftKey;
		}

		public void update(uint charCode = 0U, uint keyCode = 0U, uint keyLocation = 0U, bool ctrlKey = false, bool altKey = false, bool shiftKey = false)
		{
			this.mCharCode = charCode;
			this.mKeyCode = keyCode;
			this.mKeyLocation = keyLocation;
			this.mCtrlKey = ctrlKey;
			this.mAltKey = altKey;
			this.mShiftKey = shiftKey;
		}

		public uint charCode
		{
			get
			{
				return this.mCharCode;
			}
		}

		public uint keyCode
		{
			get
			{
				return this.mKeyCode;
			}
		}

		public uint keyLocation
		{
			get
			{
				return this.mKeyLocation;
			}
		}

		public bool altKey
		{
			get
			{
				return this.mAltKey;
			}
		}

		public bool ctrlKey
		{
			get
			{
				return this.mCtrlKey;
			}
		}

		public bool shiftKey
		{
			get
			{
				return this.mShiftKey;
			}
		}

		public void prevtDefault()
		{
		}

		public static string KEY_UP = "keyUp";

		public static string KEY_DOWN = "keyDown";

		private uint mCharCode;

		private uint mKeyCode;

		private uint mKeyLocation;

		private bool mAltKey;

		private bool mCtrlKey;

		private bool mShiftKey;
	}
}
