using System;
using GameServer.Logic;
using GameServer.Logic.MUWings;

namespace GameServer.Server.CmdProcesser
{
	public class WingOffOnCmdProcessor : ICmdProcessor
	{
		private WingOffOnCmdProcessor()
		{
			WingStarCacheManager.LoadWingStarItems();
			WingPropsCacheManager.LoadWingPropsItems();
			TCPCmdDispatcher.getInstance().registerProcessor(610, 1, this);
		}

		public static WingOffOnCmdProcessor getInstance()
		{
			return WingOffOnCmdProcessor.instance;
		}

		public bool processCmd(GameClient client, string[] cmdParams)
		{
			int cmdId = 610;
			int num = Global.SafeConvertToInt32(cmdParams[0]);
			bool result;
			if (null == client.ClientData.MyWingData)
			{
				string cmdData = string.Format("{0}:{1}", -3, num);
				client.sendCmd(cmdId, cmdData, false);
				result = true;
			}
			else
			{
				int num2 = MUWingsManager.WingOnOffDBCommand(client, client.ClientData.MyWingData.DbID, (client.ClientData.MyWingData.Using == 0) ? 1 : 0);
				if (num2 < 0)
				{
					string cmdData = string.Format("{0}:{1}", -3, num);
					client.sendCmd(cmdId, cmdData, false);
				}
				else
				{
					string cmdData = string.Format("{0}:{1}", 0, num);
					client.sendCmd(cmdId, cmdData, false);
					client.ClientData.MyWingData.Using = ((client.ClientData.MyWingData.Using == 0) ? 1 : 0);
					MUWingsManager.UpdateWingDataProps(client, client.ClientData.MyWingData.Using == 1);
					LingYuManager.UpdateLingYuProps(client);
					ZhuLingZhuHunManager.UpdateZhuLingZhuHunProps(client);
					GameManager.ClientMgr.NotifyUpdateEquipProps(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client);
					GameManager.ClientMgr.NotifyOthersLifeChanged(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, true, false, 7);
					GameManager.ClientMgr.NotifyOthersChangeEquip(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, null, 1, client.ClientData.MyWingData);
					if (client._IconStateMgr.CheckJieRiFanLi(client, ActivityTypes.JieriWing))
					{
						client._IconStateMgr.AddFlushIconState(14000, client._IconStateMgr.IsAnyJieRiTipActived());
						client._IconStateMgr.SendIconStateToClient(client);
					}
				}
				result = true;
			}
			return result;
		}

		private static WingOffOnCmdProcessor instance = new WingOffOnCmdProcessor();
	}
}
