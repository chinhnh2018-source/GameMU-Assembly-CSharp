using System;
using System.Collections.Generic;
using GameServer.Logic;
using Server.Data;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	public class JingJiChallengeInfoCmdProcessor : ICmdProcessor
	{
		private JingJiChallengeInfoCmdProcessor()
		{
		}

		public static JingJiChallengeInfoCmdProcessor getInstance()
		{
			return JingJiChallengeInfoCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			bool result;
			if (client.ClientData.CurrentLifeV <= 0 || client.ClientData.CurrentAction == 12)
			{
				result = true;
			}
			else
			{
				int num = Convert.ToInt32(cmdParams[1]);
				int roleID = client.ClientData.RoleID;
				List<JingJiChallengeInfoData> cmdData = Global.sendToDB<List<JingJiChallengeInfoData>, byte[]>(10146, DataHelper.ObjectToBytes<int[]>(new int[]
				{
					roleID,
					num
				}), client.ServerId);
				client.sendCmd<List<JingJiChallengeInfoData>>(582, cmdData, false);
				result = true;
			}
			return result;
		}

		private static JingJiChallengeInfoCmdProcessor instance = new JingJiChallengeInfoCmdProcessor();
	}
}
