using System;

namespace UniLua
{
	internal class UndumpException : Exception
	{
		public UndumpException(string why)
		{
			this.Why = why;
		}

		public string Why;
	}
}
