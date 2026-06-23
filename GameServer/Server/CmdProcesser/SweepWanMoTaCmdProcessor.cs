using System;
using GameServer.Logic;
using GameServer.Logic.WanMota;

namespace GameServer.Server.CmdProcesser
{
	public class SweepWanMoTaCmdProcessor : ICmdProcessor
	{
		private SweepWanMoTaCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(616, 2, this);
		}

		public static SweepWanMoTaCmdProcessor getInstance()
		{
			return SweepWanMoTaCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdId = 616;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			int num2 = Global.SafeConvertToInt32(cmdParams[1]);
			bool result;
			if (client.ClientData.WanMoTaProp.nPassLayerCount < SweepWanMotaManager.nSweepReqMinLayerOrder)
			{
				string cmdData = string.Format("{0}:{1}", -2, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else if (0 == client.ClientData.WanMoTaProp.nSweepLayer)
			{
				string cmdData = string.Format("{0}:{1}", -4, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else
			{
				string cmdData;
				if (client.ClientData.WanMoTaProp.nSweepLayer > 0)
				{
					SweepWanMotaManager.SweepContinue(client);
				}
				else
				{
					if (SweepWanMotaManager.GetSweepCount(client) >= SweepWanMotaManager.nWanMoTaMaxSweepNum)
					{
						cmdData = string.Format("{0}:{1}", -1, num);
						client.sendCmd(cmdId, cmdData, false);
						return true;
					}
					SweepWanMotaManager.SweepBegin(client);
				}
				cmdData = string.Format("{0}:{1}", 0, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			return result;
		}

		private static SweepWanMoTaCmdProcessor instance = new SweepWanMoTaCmdProcessor();
	}
}
