using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
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

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] array = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int challengerId = array[0];
			int beChallengerId = array[1];
			int beChallengerRanking = array[2];
			JingJiBeChallengeData cmdData = JingJiChangManager.getInstance().requestChallenge(challengerId, beChallengerId, beChallengerRanking);
			client.sendCmd<JingJiBeChallengeData>(10143, cmdData);
		}

		private static JingJiRequestChallengeCmdProcessor instance = new JingJiRequestChallengeCmdProcessor();
	}
}
