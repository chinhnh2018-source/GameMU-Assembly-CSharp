using System;
using GameServer.Logic;

namespace GameServer.Server.CmdProcesser
{
	public class WashPropsCmdProcessor : ICmdProcessor
	{
		public WashPropsCmdProcessor(TCPGameServerCmds cmdID)
		{
			this.CmdID = cmdID;
		}

		public static WashPropsCmdProcessor getInstance(TCPGameServerCmds cmdID)
		{
			return new WashPropsCmdProcessor(cmdID);
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdID = (int)this.CmdID;
			bool result;
			if (this.CmdID == TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS)
			{
				int dbid = Global.SafeConvertToInt32(cmdParams[1]);
				int washIndex = Global.SafeConvertToInt32(cmdParams[2]);
				bool firstUseBinding = Global.SafeConvertToInt32(cmdParams[3]) > 0;
				int num = Global.SafeConvertToInt32(cmdParams[4]);
				result = WashPropsManager.WashProps(client, dbid, washIndex, firstUseBinding, num);
			}
			else if (this.CmdID == TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPSINHERIT)
			{
				int leftGoodsDbID = Global.SafeConvertToInt32(cmdParams[1]);
				int rightGoodsDbID = Global.SafeConvertToInt32(cmdParams[2]);
				int num = Global.SafeConvertToInt32(cmdParams[3]);
				result = WashPropsManager.WashPropsInherit(client, leftGoodsDbID, rightGoodsDbID, num);
			}
			else
			{
				result = true;
			}
			return result;
		}

		private TCPGameServerCmds CmdID = TCPGameServerCmds.CMD_SPR_EXEC_WASHPROPS;
	}
}
