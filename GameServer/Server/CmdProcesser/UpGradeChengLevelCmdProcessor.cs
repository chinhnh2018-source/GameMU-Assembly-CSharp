using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	public class UpGradeChengLevelCmdProcessor : ICmdProcessor
	{
		public UpGradeChengLevelCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		public static UpGradeChengLevelCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new UpGradeChengLevelCmdProcessor(cmdID);
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdID = (int)this.CmdID;
			bool result;
			if (this.CmdID == TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU)
			{
				int num = Global.SafeConvertToInt32(cmdParams[0]);
				int nChengJiuLevel = Global.SafeConvertToInt32(cmdParams[1]);
				int num2 = ChengJiuManager.TryToActiveNewChengJiuBuffer(client, true, nChengJiuLevel);
				string cmdData = string.Format("{0}:{1}", num, num2);
				client.sendCmd(cmdID, cmdData, false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_UPGRADE_CHENGJIU;
	}
}
