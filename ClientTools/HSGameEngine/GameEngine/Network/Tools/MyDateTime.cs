using System;

namespace HSGameEngine.GameEngine.Network.Tools
{
	public class MyDateTime
	{
		public static void Init()
		{
			MyDateTime.xe644b932ff70e893 = DateTime.Now;
			MyDateTime.xee708fd343752bbc = Environment.TickCount;
		}

		public static DateTime Now()
		{
			int num = Environment.TickCount - MyDateTime.xee708fd343752bbc;
			return MyDateTime.xe644b932ff70e893.AddMilliseconds((double)num);
		}

		public static long Before1970Ticks = 621356256000000000L;

		private static DateTime xe644b932ff70e893;

		private static int xee708fd343752bbc = 0;
	}
}
