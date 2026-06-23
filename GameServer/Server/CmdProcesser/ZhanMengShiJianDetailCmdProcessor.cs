using System;
using System.Collections.Generic;
using GameServer.Logic;
using GameServer.Logic.BangHui.ZhanMengShiJian;
using Server.Tools;

namespace GameServer.Server.CmdProcesser
{
	public class ZhanMengShiJianDetailCmdProcessor : ICmdProcessor
	{
		private ZhanMengShiJianDetailCmdProcessor()
		{
		}

		public static ZhanMengShiJianDetailCmdProcessor getInstance()
		{
			return ZhanMengShiJianDetailCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int faction = client.ClientData.Faction;
			int num = Convert.ToInt32(cmdParams[1]);
			byte[] cmd = DataHelper.ObjectToBytes<int[]>(new int[]
			{
				faction,
				num
			});
			List<ZhanMengShiJianData> cmdData = Global.sendToDB<List<ZhanMengShiJianData>, byte[]>(10139, cmd, client.ServerId);
			client.sendCmd<List<ZhanMengShiJianData>>(566, cmdData, false);
			return true;
		}

		private static ZhanMengShiJianDetailCmdProcessor instance = new ZhanMengShiJianDetailCmdProcessor();
	}
}
