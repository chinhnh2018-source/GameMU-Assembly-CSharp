using System;
using GameServer.Logic;
using GameServer.Logic.JingJiChang;
using Server.Data;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiDetailCmdProcessor : ICmdProcessor
	{
		private JingJiDetailCmdProcessor()
		{
		}

		public static JingJiDetailCmdProcessor getInstance()
		{
			return JingJiDetailCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int requestType = Convert.ToInt32(cmdParams[1]);
			JingJiDetailData detailData = JingJiChangManager.getInstance().getDetailData(client, requestType);
			client.sendCmd<JingJiDetailData>(578, detailData, false);
			return true;
		}

		private static JingJiDetailCmdProcessor instance = new JingJiDetailCmdProcessor();
	}
}
