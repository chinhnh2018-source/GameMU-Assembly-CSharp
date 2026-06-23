using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiRequestChallengeCmdProcessor : ICmdProcessor
	{
		private JingJiRequestChallengeCmdProcessor()
		{
		}

		public static JingJiRequestChallengeCmdProcessor getInstance()
		{
			return JingJiRequestChallengeCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num = Convert.ToInt32(cmdParams[0]);
			int num2 = Convert.ToInt32(cmdParams[1]);
			int num3 = Convert.ToInt32(cmdParams[2]);
			int num4 = Convert.ToInt32(cmdParams[3]);
			int cmdData = 0;
			bool result;
			if (num2 < 0 || num3 < 1 || num3 > JingJiChangConstants.RankingListMaxNum || (num4 != JingJiChangConstants.Enter_Type_Free && num4 != JingJiChangConstants.Enter_Type_Vip))
			{
				client.sendCmd<int>(579, cmdData, false);
				result = true;
			}
			else
			{
				cmdData = JingJiChangManager.getInstance().requestChallenge(client, num2, num3, num4);
				client.sendCmd<int>(579, cmdData, false);
				result = true;
			}
			return result;
		}

		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
