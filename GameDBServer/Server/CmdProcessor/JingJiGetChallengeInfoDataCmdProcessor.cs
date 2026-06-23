using System;
using System.Collections.Generic;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class JingJiGetChallengeInfoDataCmdProcessor : ICmdProcessor
	{
		private JingJiGetChallengeInfoDataCmdProcessor()
		{
		}

		public static JingJiGetChallengeInfoDataCmdProcessor getInstance()
		{
			return JingJiGetChallengeInfoDataCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int[] array = DataHelper.BytesToObject<int[]>(cmdParams, 0, count);
			int roleId = array[0];
			int pageIndex = array[1];
			List<JingJiChallengeInfoData> challengeInfoDataList = JingJiChangManager.getInstance().getChallengeInfoDataList(roleId, pageIndex);
			client.sendCmd<List<JingJiChallengeInfoData>>(10146, challengeInfoDataList);
		}

		private static JingJiGetChallengeInfoDataCmdProcessor instance = new JingJiGetChallengeInfoDataCmdProcessor();
	}
}
