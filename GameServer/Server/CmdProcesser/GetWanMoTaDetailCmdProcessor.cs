using System;
using GameServer.Logic;
using GameServer.Logic.WanMota;
using Server.Data;
using Server.Tools;
using Server.Tools.Pattern;

namespace GameServer.Server.CmdProcesser
{
	public class GetWanMoTaDetailCmdProcessor : ICmdProcessor
	{
		private GetWanMoTaDetailCmdProcessor()
		{
			TCPCmdDispatcher.getInstance().registerProcessor(618, 1, this);
		}

		public static GetWanMoTaDetailCmdProcessor getInstance()
		{
			return GetWanMoTaDetailCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdId = 618;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			WanMotaInfo wanMoTaDetail = WanMotaCopySceneManager.GetWanMoTaDetail(client, false);
			bool result;
			if (null == wanMoTaDetail)
			{
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					-1,
					num,
					0,
					0,
					0,
					0,
					0
				});
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else
			{
				if (wanMoTaDetail.nPassLayerCount != client.ClientData.WanMoTaNextLayerOrder)
				{
					LogManager.WriteLog(2, string.Format("角色roleid={0} 万魔塔层数不一致 nPassLayerCount={1}, WanMoTaNextLayerOrder={2}", client.ClientData.RoleID, wanMoTaDetail.nPassLayerCount, client.ClientData.WanMoTaNextLayerOrder), null, true);
					client.ClientData.WanMoTaNextLayerOrder = wanMoTaDetail.nPassLayerCount;
					GameManager.ClientMgr.SaveWanMoTaPassLayerValue(client, wanMoTaDetail.nPassLayerCount, true);
				}
				string cmdData = string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}", new object[]
				{
					0,
					num,
					wanMoTaDetail.nPassLayerCount,
					wanMoTaDetail.nTopPassLayerCount,
					SweepWanMotaManager.GetSweepCount(client),
					wanMoTaDetail.nSweepLayer,
					WanMotaCopySceneManager.WanmotaIsSweeping(client)
				});
				SingletonTemplate<WanMoTaTopLayerManager>.Instance().CheckNeedUpdate(wanMoTaDetail.nTopPassLayerCount);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			return result;
		}

		private static GetWanMoTaDetailCmdProcessor instance = new GetWanMoTaDetailCmdProcessor();
	}
}
