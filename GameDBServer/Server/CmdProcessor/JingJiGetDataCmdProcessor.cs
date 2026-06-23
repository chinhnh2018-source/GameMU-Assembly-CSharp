using System;
using GameDBServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameDBServer.Server.CmdProcessor
{
	public class JingJiGetDataCmdProcessor : ICmdProcessor
	{
		private JingJiGetDataCmdProcessor()
		{
		}

		public static JingJiGetDataCmdProcessor getInstance()
		{
			return JingJiGetDataCmdProcessor.instance;
		}

		public void processCmd(GameServerClient client, int nID, byte[] cmdParams, int count)
		{
			int roleId = DataHelper.BytesToObject<int>(cmdParams, 0, count);
			PlayerJingJiData playerJingJiDataById = JingJiChangManager.getInstance().getPlayerJingJiDataById(roleId);
			client.sendCmd<PlayerJingJiData>(10140, playerJingJiDataById);
		}

		private static JingJiGetDataCmdProcessor instance = new JingJiGetDataCmdProcessor();
	}
}
