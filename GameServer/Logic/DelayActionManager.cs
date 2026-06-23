using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	internal class DelayActionManager
	{
		public static void AddDelayAction(DelayAction action)
		{
			lock (DelayActionManager.m_Actions)
			{
				DelayActionManager.m_Actions.Add(action);
			}
		}

		public static void RemoveDelayAction(DelayAction action)
		{
			lock (DelayActionManager.m_Actions)
			{
				DelayActionManager.m_Actions.Remove(action);
			}
		}

		public static void StartAction(DelayAction action)
		{
			DelayActionType delayActionType = action.m_DelayActionType;
			DelayActionType delayActionType2 = delayActionType;
			if (delayActionType2 == DelayActionType.DA_BLINK)
			{
				int num = action.m_Params[0];
				GameClient client = action.m_Client;
				int num2 = num * 100;
				GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
				int roleDirection = client.ClientData.RoleDirection;
				Point point = client.CurrentGrid;
				int num3 = num2 / gameMap.MapGridWidth;
				int num4 = num3;
				List<Point> gridPointByDirection = Global.GetGridPointByDirection(roleDirection, (int)point.X, (int)point.Y, num3);
				byte b = 0;
				b |= 1;
				b |= 2;
				for (int i = 0; i < gridPointByDirection.Count; i++)
				{
					if (Global.InObsByGridXY(client.ObjectType, client.ClientData.MapCode, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, 0, b))
					{
						break;
					}
					num3--;
				}
				if (num3 < num4)
				{
					point = gridPointByDirection[num4 - num3 - 1];
				}
				Point point2 = point;
				if (!Global.CanQueueMoveObject(client, roleDirection, (int)point.X, (int)point.Y, num3, num3, b, out point2, false))
				{
					Point point3 = new Point(point2.X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), point2.Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
					GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (int)point3.X, (int)point3.Y, client.ClientData.RoleDirection, 159, 3);
				}
				else
				{
					Point point3 = new Point(gridPointByDirection[gridPointByDirection.Count - 1].X * (double)gameMap.MapGridWidth + (double)(gameMap.MapGridWidth / 2), gridPointByDirection[gridPointByDirection.Count - 1].Y * (double)gameMap.MapGridHeight + (double)(gameMap.MapGridHeight / 2));
					GameManager.ClientMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, client, (int)point3.X, (int)point3.Y, client.ClientData.RoleDirection, 159, 3);
				}
				List<object> all9Clients = Global.GetAll9Clients(client);
				string strCmd = string.Format("{0}", client.ClientData.RoleID);
				GameManager.ClientMgr.SendToClients(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, null, all9Clients, strCmd, 511);
				DelayActionManager.RemoveDelayAction(action);
			}
		}

		public static void HeartBeatDelayAction()
		{
			for (int i = 0; i < DelayActionManager.m_Actions.Count; i++)
			{
				long num = TimeUtil.NOW();
				DelayAction delayAction = DelayActionManager.m_Actions[i];
				long startTime = delayAction.m_StartTime;
				long delayTime = delayAction.m_DelayTime;
				if (num - startTime > delayTime)
				{
					DelayActionManager.StartAction(delayAction);
				}
			}
		}

		private static List<DelayAction> m_Actions = new List<DelayAction>();
	}
}
