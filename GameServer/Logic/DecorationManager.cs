using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using Server.Protocol;
using Server.TCP;

namespace GameServer.Logic
{
	public class DecorationManager
	{
		public static bool AddDecoToMap(int mapCode, int copyMapID, Point pos, int decoID, int maxLiveTicks, int alphaTicks, bool notifyClients)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			bool result;
			if (null == mapGrid)
			{
				result = false;
			}
			else
			{
				Decoration decoration = new Decoration
				{
					AutoID = DecorationManager.AutoDecoID++,
					DecoID = decoID,
					MapCode = mapCode,
					CopyMapID = copyMapID,
					Pos = pos,
					StartTicks = TimeUtil.NOW(),
					MaxLiveTicks = maxLiveTicks,
					AlphaTicks = alphaTicks
				};
				lock (DecorationManager.DictDecos)
				{
					DecorationManager.DictDecos[decoration.AutoID] = decoration;
				}
				mapGrid.MoveObject(-1, -1, (int)pos.X, (int)pos.Y, decoration);
				if (notifyClients)
				{
					DecorationManager.NotifyNearClientsToAddSelf(decoration);
				}
				result = false;
			}
			return result;
		}

		public static Decoration GetDecoration(int mapCode, int copyMapID, Point pos, int decoID, int maxLiveTicks, int alphaTicks)
		{
			return new Decoration
			{
				AutoID = DecorationManager.AutoDecoID++,
				DecoID = decoID,
				MapCode = mapCode,
				CopyMapID = copyMapID,
				Pos = pos,
				StartTicks = TimeUtil.NOW(),
				MaxLiveTicks = maxLiveTicks,
				AlphaTicks = alphaTicks
			};
		}

		public static Decoration FindDeco(int autoID)
		{
			Decoration result = null;
			lock (DecorationManager.DictDecos)
			{
				if (DecorationManager.DictDecos.TryGetValue(autoID, out result))
				{
					DecorationManager.DictDecos.Remove(autoID);
				}
			}
			return result;
		}

		public static void RemoveDeco(int autoID)
		{
			Decoration decoration = null;
			lock (DecorationManager.DictDecos)
			{
				if (DecorationManager.DictDecos.TryGetValue(autoID, out decoration))
				{
					DecorationManager.DictDecos.Remove(autoID);
				}
			}
			if (null != decoration)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[decoration.MapCode];
				if (null != mapGrid)
				{
					mapGrid.RemoveObject(decoration);
				}
			}
		}

		protected static void NotifyNearClientsToAddSelf(Decoration deco)
		{
			List<object> all9Clients = Global.GetAll9Clients(deco);
			if (null != all9Clients)
			{
				GameClient gameClient = null;
				for (int i = 0; i < all9Clients.Count; i++)
				{
					gameClient = (all9Clients[i] as GameClient);
					if (null != gameClient)
					{
						lock (gameClient.ClientData.VisibleGrid9Objects)
						{
							gameClient.ClientData.VisibleGrid9Objects[gameClient] = 1;
						}
						GameManager.ClientMgr.NotifyMySelfNewDeco(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, gameClient, deco);
					}
				}
			}
		}

		public static void SendMySelfDecos(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					Decoration decoration = objsList[i] as Decoration;
					if (null != decoration)
					{
						GameManager.ClientMgr.NotifyMySelfNewDeco(sl, pool, client, decoration);
					}
				}
			}
		}

		public static void DelMySelfDecos(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					Decoration decoration = objsList[i] as Decoration;
					if (null != decoration)
					{
						GameManager.ClientMgr.NotifyMySelfDelDeco(sl, pool, client, decoration);
					}
				}
			}
		}

		public static void ProcessAllDecos(SocketListener sl, TCPOutPacketPool pool)
		{
			List<Decoration> list = new List<Decoration>();
			lock (DecorationManager.DictDecos)
			{
				foreach (Decoration item in DecorationManager.DictDecos.Values)
				{
					list.Add(item);
				}
			}
			long num = TimeUtil.NOW();
			Decoration decoration = null;
			for (int i = 0; i < list.Count; i++)
			{
				decoration = list[i];
				if (decoration.MaxLiveTicks > 0)
				{
					if (num - decoration.StartTicks >= (long)decoration.MaxLiveTicks)
					{
						lock (DecorationManager.DictDecos)
						{
							DecorationManager.DictDecos.Remove(decoration.AutoID);
						}
						GameManager.MapGridMgr.DictGrids[decoration.MapCode].RemoveObject(decoration);
						List<object> all9Clients = Global.GetAll9Clients(decoration);
						GameManager.ClientMgr.NotifyOthersDelDeco(sl, pool, all9Clients, decoration.MapCode, decoration.AutoID);
					}
				}
			}
		}

		public static int AutoDecoID = 1;

		public static Dictionary<int, Decoration> DictDecos = new Dictionary<int, Decoration>();
	}
}
