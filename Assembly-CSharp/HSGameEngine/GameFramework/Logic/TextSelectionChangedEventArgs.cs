using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class TextSelectionChangedEventArgs : ObjectClickEvent
	{
		public TextSelectionChangedEventArgs() : base(string.Empty, false, false)
		{
		}

		public string Text
		{
			get
			{
				return this._Text;
			}
			set
			{
				this._Text = value;
			}
		}

		private string _Text = string.Empty;
	}
}
