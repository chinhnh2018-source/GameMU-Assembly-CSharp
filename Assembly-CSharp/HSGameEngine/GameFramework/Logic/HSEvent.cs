using System;

namespace HSGameEngine.GameFramework.Logic
{
	public class HSEvent
	{
		public string type
		{
			get
			{
				return this._type;
			}
			set
			{
				this._type = value;
			}
		}

		public const string ADDED_TO_STAGE = "ADDED_TO_STAGE";

		private string _type = string.Empty;
	}
}
