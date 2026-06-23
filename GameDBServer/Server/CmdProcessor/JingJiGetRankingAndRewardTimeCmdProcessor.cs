using System;
using GameDBServer.Logic;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class JingJiGetRankingAndRewardTimeCmdProcessor : ICmdProcessor
	{
		private JingJiGetRankingAndRewardTimeCmdProcessor()
		{
		}

		public static JingJiGetRankingAndRewardTimeCmdProcessor getInstance()
		{
			return JingJiGetRankingAndRewardTimeCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			int num = -2;
			long num2 = 0L;
			JingJiChangManager.getInstance().getRankingAndNextRewardTimeById(roleId, out num, out num2);
			long[] cmdData = new long[]
			{
				(long)num,
				num2
			};
			client.sendCmd<long[]>(10148, cmdData);
		}

		private static JingJiGetRankingAndRewardTimeCmdProcessor instance = new JingJiGetRankingAndRewardTimeCmdProcessor();
	}
}
