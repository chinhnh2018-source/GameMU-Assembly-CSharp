using System;

namespace HTMLEngine
{
	public abstract class HtLogger
	{
		public abstract void Log(HtLogLevel level, string message);
	}
}
