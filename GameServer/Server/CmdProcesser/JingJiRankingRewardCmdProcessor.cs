using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiRankingRewardCmdProcessor : ICmdProcessor
	{
		private JingJiRankingRewardCmdProcessor()
		{
		}

		public static JingJiRankingRewardCmdProcessor getInstance()
		{
			return JingJiRankingRewardCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int num;
			long num2;
			JingJiChangManager.getInstance().rankingReward(client, out num, out num2);
			client.sendCmd<long[]>(583, new long[]
			{
				(long)num,
				num2
			}, false);
			return true;
		}

		private static JingJiRankingRewardCmdProcessor instance = new JingJiRankingRewardCmdProcessor();
	}
}
