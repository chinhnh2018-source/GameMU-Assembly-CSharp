using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class BossHomeManager
	{
		public void HeartBeatBossHomeScene()
		{
			int mapClientsCount = GameManager.ClientMgr.GetMapClientsCount(Data.BosshomeData.MapID);
			if (mapClientsCount > 0)
			{
				List<object> mapClients = GameManager.ClientMgr.GetMapClients(Data.BosshomeData.MapID);
				if (mapClients != null)
				{
					for (int i = 0; i < mapClients.Count; i++)
					{
						GameClient gameClient = mapClients[i] as GameClient;
						if (gameClient != null)
						{
							if (gameClient.ClientData.MapCode == Data.BosshomeData.MapID)
							{
								this.SubDiamond(gameClient);
							}
						}
					}
				}
			}
		}

		public void SubDiamond(GameClient client)
		{
			if (0 != Data.BosshomeData.OneMinuteNeedDiamond)
			{
				long num = TimeUtil.NOW();
				if (num >= this.m_SubMoneyTick + (long)this.m_SubMoneyInterval)
				{
					this.m_SubMoneyTick = num;
					if (!GameManager.ClientMgr.SubUserMoney(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, Data.BosshomeData.OneMinuteNeedDiamond, "BOSS之家扣除", true, true, false, DaiBiSySType.None))
					{
						this.KickOutScene(client);
					}
				}
			}
		}

		public void KickOutScene(GameClient client)
		{
			int num = GameManager.MainMapCode;
			int maxX = -1;
			int mapY = -1;
			if (MapTypes.Normal == Global.GetMapType(client.ClientData.LastMapCode))
			{
				if (GameManager.BattleMgr.BattleMapCode != client.ClientData.LastMapCode || GameManager.ArenaBattleMgr.BattleMapCode != client.ClientData.LastMapCode)
				{
					num = client.ClientData.LastMapCode;
					maxX = client.ClientData.LastPosX;
					mapY = client.ClientData.LastPosY;
				}
			}
			GameMap gameMap = null;
			if (GameManager.MapMgr.DictMaps.TryGetValue(num, out gameMap))
			{
				GameManager.ClientMgr.NotifyChangeMap(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, num, maxX, mapY, -1, 0);
			}
		}

		public int m_SubMoneyInterval = 60000;

		public long m_SubMoneyTick = 0L;
	}
}
