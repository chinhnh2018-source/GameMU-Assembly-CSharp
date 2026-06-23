using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiGetBuffCmdProcessor : ICmdProcessor
	{
		private JingJiGetBuffCmdProcessor()
		{
		}

		public static JingJiGetBuffCmdProcessor getInstance()
		{
			return JingJiGetBuffCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[1]);
			int cmdData;
			if (num != 0 && num != 1)
			{
				cmdData = ResultCode.Illegal;
			}
			else
			{
				cmdData = JingJiChangManager.getInstance().activeJunXianBuff(client, num == 1);
			}
			client.sendCmd<int>(585, cmdData, false);
			return true;
		}

		private static JingJiGetBuffCmdProcessor instance = new JingJiGetBuffCmdProcessor();
	}
}
