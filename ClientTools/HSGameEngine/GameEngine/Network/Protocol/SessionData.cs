using System;

namespace HSGameEngine.GameEngine.Network.Protocol
{
	public class SessionData
	{
		public const short MinOffsetCmdId = 100;

		public static ushort CmdOffset = 0;

		public static short OffsetChgCmdId = 21;

		public static short GenerateKeyCmdId = 24;
	}
}
