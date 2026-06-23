using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiStartFightCmdProcessor : ICmdProcessor
	{
		private JingJiStartFightCmdProcessor()
		{
		}

		public static JingJiStartFightCmdProcessor getInstance()
		{
			return JingJiStartFightCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdId = 634;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (-1 == JingJiChangManager.getInstance().JingJiChangStartFight(client))
			{
				string cmdData = string.Format("{0}:{1}", -1, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else
			{
				string cmdData = string.Format("{0}:{1}", 0, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			return result;
		}

		private static JingJiStartFightCmdProcessor instance = new JingJiStartFightCmdProcessor();
	}
}
