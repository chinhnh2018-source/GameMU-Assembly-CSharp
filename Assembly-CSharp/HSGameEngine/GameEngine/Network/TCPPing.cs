using System;
using HSGameEngine.GameEngine.Network.Tools;

namespace HSGameEngine.GameEngine.Network
{
	public class TCPPing
	{
		public static long GetPingTicks()
		{
			return TCPPing.AvgPingTicks;
		}

		public static void RecordSendCmd(int nID)
		{
			switch (nID)
			{
			case 112:
				TCPPing.PosCmdStartTicks = TimeManager.GetCorrectLocalTime();
				break;
			default:
				if (nID != 23)
				{
					if (nID == 107)
					{
						TCPPing.MoveCmdStartTicks = TimeManager.GetCorrectLocalTime();
					}
				}
				else
				{
					TCPPing.HeartCmdStartTicks = TimeManager.GetCorrectLocalTime();
				}
				break;
			case 114:
				TCPPing.ActionCmdStartTicks = TimeManager.GetCorrectLocalTime();
				break;
			}
		}

		public static void RecordRecCmd(int nID)
		{
			switch (nID)
			{
			case 112:
				TCPPing.AvgPingTicks = (TimeManager.GetCorrectLocalTime() - TCPPing.PosCmdStartTicks) / 2L;
				break;
			default:
				if (nID != 23)
				{
					if (nID == 107)
					{
						TCPPing.AvgPingTicks = (TimeManager.GetCorrectLocalTime() - TCPPing.MoveCmdStartTicks) / 2L;
					}
				}
				else
				{
					TCPPing.AvgPingTicks = (TimeManager.GetCorrectLocalTime() - TCPPing.HeartCmdStartTicks) / 2L;
				}
				break;
			case 114:
				TCPPing.AvgPingTicks = (TimeManager.GetCorrectLocalTime() - TCPPing.ActionCmdStartTicks) / 2L;
				break;
			}
		}

		private static long MoveCmdStartTicks;

		private static long PosCmdStartTicks;

		private static long HeartCmdStartTicks;

		private static long ActionCmdStartTicks;

		private static long AvgPingTicks;
	}
}
