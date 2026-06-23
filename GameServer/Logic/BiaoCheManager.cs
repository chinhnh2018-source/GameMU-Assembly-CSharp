using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Data;
using Server.Protocol;
using Server.TCP;
using Server.Tools;

namespace GameServer.Logic
{
	public class BiaoCheManager
	{
		private static BiaoCheItem AddBiaoChe(GameClient client, int yaBiaoID)
		{
			SystemXmlItem systemXmlItem = null;
			BiaoCheItem result;
			if (!GameManager.systemYaBiaoMgr.SystemXmlItemDict.TryGetValue(yaBiaoID, out systemXmlItem))
			{
				result = null;
			}
			else
			{
				BiaoCheItem biaoCheItem = new BiaoCheItem
				{
					OwnerRoleID = client.ClientData.RoleID,
					OwnerRoleName = Global.FormatRoleName(client, client.ClientData.RoleName),
					BiaoCheID = (int)GameManager.BiaoCheIDMgr.GetNewID(),
					BiaoCheName = Global.GetYaBiaoName(yaBiaoID),
					YaBiaoID = yaBiaoID,
					MapCode = client.ClientData.MapCode,
					PosX = client.ClientData.PosX,
					PosY = client.ClientData.PosY,
					Direction = client.ClientData.RoleDirection,
					LifeV = systemXmlItem.GetIntValue("Lifev", -1),
					StartTime = TimeUtil.NOW(),
					CurrentLifeV = systemXmlItem.GetIntValue("Lifev", -1),
					CutLifeV = systemXmlItem.GetIntValue("CutLifeV", -1),
					BodyCode = systemXmlItem.GetIntValue("BodyCode", -1),
					PicCode = systemXmlItem.GetIntValue("PicCode", -1),
					DestNPC = systemXmlItem.GetIntValue("DestNPC", -1),
					MinLevel = systemXmlItem.GetIntValue("MinLevel", -1),
					MaxLevel = systemXmlItem.GetIntValue("MaxLevel", -1)
				};
				lock (BiaoCheManager._RoleID2BiaoCheDict)
				{
					BiaoCheManager._RoleID2BiaoCheDict[biaoCheItem.OwnerRoleID] = biaoCheItem;
				}
				lock (BiaoCheManager._ID2BiaoCheDict)
				{
					BiaoCheManager._ID2BiaoCheDict[biaoCheItem.BiaoCheID] = biaoCheItem;
				}
				result = biaoCheItem;
			}
			return result;
		}

		public static BiaoCheItem FindBiaoCheByRoleID(int roleID)
		{
			BiaoCheItem result = null;
			lock (BiaoCheManager._RoleID2BiaoCheDict)
			{
				BiaoCheManager._RoleID2BiaoCheDict.TryGetValue(roleID, out result);
			}
			return result;
		}

		public static BiaoCheItem FindBiaoCheByID(int biaoCheID)
		{
			BiaoCheItem result = null;
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				BiaoCheManager._ID2BiaoCheDict.TryGetValue(biaoCheID, out result);
			}
			return result;
		}

		private static void RemoveBiaoChe(int biaoCheID)
		{
			BiaoCheItem biaoCheItem = null;
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				BiaoCheManager._ID2BiaoCheDict.TryGetValue(biaoCheID, out biaoCheItem);
				if (null != biaoCheItem)
				{
					BiaoCheManager._ID2BiaoCheDict.Remove(biaoCheItem.BiaoCheID);
				}
			}
			lock (BiaoCheManager._RoleID2BiaoCheDict)
			{
				if (null != biaoCheItem)
				{
					BiaoCheManager._RoleID2BiaoCheDict.Remove(biaoCheItem.OwnerRoleID);
				}
			}
		}

		public static void ProcessNewBiaoChe(SocketListener sl, TCPOutPacketPool pool, GameClient client, int yaBiaoID)
		{
			BiaoCheItem biaoCheItem = BiaoCheManager.AddBiaoChe(client, yaBiaoID);
			if (null == biaoCheItem)
			{
				LogManager.WriteLog(2, string.Format("为RoleID生成镖车对象时失败, Client={0}, RoleID={1}, YaBiaoID={2}", Global.GetSocketRemoteEndPoint(client.ClientSocket, false), client.ClientData.RoleID, yaBiaoID), null, true);
			}
			else
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].MoveObject(-1, -1, biaoCheItem.PosX, biaoCheItem.PosY, biaoCheItem);
			}
		}

		public static void ProcessDelBiaoChe(SocketListener sl, TCPOutPacketPool pool, int biaoCheID)
		{
			BiaoCheItem biaoCheItem = BiaoCheManager.FindBiaoCheByID(biaoCheID);
			if (null != biaoCheItem)
			{
				BiaoCheManager.RemoveBiaoChe(biaoCheID);
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].RemoveObject(biaoCheItem);
			}
		}

		public static void NotifyOthersShowBiaoChe(SocketListener sl, TCPOutPacketPool pool, BiaoCheItem biaoCheItem)
		{
			if (null != biaoCheItem)
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].MoveObject(-1, -1, biaoCheItem.PosX, biaoCheItem.PosY, biaoCheItem);
			}
		}

		public static void NotifyOthersHideBiaoChe(SocketListener sl, TCPOutPacketPool pool, BiaoCheItem biaoCheItem)
		{
			if (null != biaoCheItem)
			{
				GameManager.MapGridMgr.DictGrids[biaoCheItem.MapCode].RemoveObject(biaoCheItem);
			}
		}

		private static bool ProcessBiaoCheOverTime(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			bool result;
			if (nowTicks - biaoCheItem.StartTime < (long)Global.MaxYaBiaoTicks)
			{
				result = false;
			}
			else
			{
				BiaoCheManager.ProcessDelBiaoChe(sl, pool, biaoCheItem.BiaoCheID);
				result = true;
			}
			return result;
		}

		private static bool ProcessBiaoCheDead(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			bool result;
			if (biaoCheItem.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				long num = nowTicks - biaoCheItem.BiaoCheDeadTicks;
				if (num < 2000L)
				{
					result = false;
				}
				else
				{
					BiaoCheManager.ProcessDelBiaoChe(sl, pool, biaoCheItem.BiaoCheID);
					result = true;
				}
			}
			return result;
		}

		private static void ProcessBiaoCheAddLife(SocketListener sl, TCPOutPacketPool pool, long nowTicks, BiaoCheItem biaoCheItem)
		{
			long num = nowTicks - biaoCheItem.LastLifeMagicTick;
			if (num >= 5000L)
			{
				biaoCheItem.LastLifeMagicTick = nowTicks;
				if (biaoCheItem.CurrentLifeV > 0)
				{
					if (biaoCheItem.CurrentLifeV < biaoCheItem.LifeV)
					{
						double num2 = (double)biaoCheItem.CutLifeV;
						num2 += (double)biaoCheItem.CurrentLifeV;
						if (biaoCheItem.CurrentLifeV > 0)
						{
							biaoCheItem.CurrentLifeV = (int)Global.GMin((double)biaoCheItem.LifeV, num2);
							List<object> all9Clients = Global.GetAll9Clients(biaoCheItem);
							GameManager.ClientMgr.NotifyOtherBiaoCheLifeV(sl, pool, all9Clients, biaoCheItem.BiaoCheID, biaoCheItem.CurrentLifeV);
						}
					}
				}
			}
		}

		public static void ProcessAllBiaoCheItems(SocketListener sl, TCPOutPacketPool pool)
		{
			List<BiaoCheItem> list = new List<BiaoCheItem>();
			lock (BiaoCheManager._ID2BiaoCheDict)
			{
				foreach (BiaoCheItem item in BiaoCheManager._ID2BiaoCheDict.Values)
				{
					list.Add(item);
				}
			}
			long nowTicks = TimeUtil.NOW();
			for (int i = 0; i < list.Count; i++)
			{
				BiaoCheItem biaoCheItem = list[i];
				if (!BiaoCheManager.ProcessBiaoCheOverTime(sl, pool, nowTicks, biaoCheItem))
				{
					if (!BiaoCheManager.ProcessBiaoCheDead(sl, pool, nowTicks, biaoCheItem))
					{
						BiaoCheManager.ProcessBiaoCheAddLife(sl, pool, nowTicks, biaoCheItem);
					}
				}
			}
		}

		public static void SendMySelfBiaoCheItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is BiaoCheItem)
					{
						if ((objsList[i] as BiaoCheItem).CurrentLifeV > 0)
						{
							BiaoCheItem biaoCheItem = objsList[i] as BiaoCheItem;
							GameManager.ClientMgr.NotifyMySelfNewBiaoChe(sl, pool, client, biaoCheItem);
						}
					}
				}
			}
		}

		public static void DelMySelfBiaoCheItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					if (objsList[i] is BiaoCheItem)
					{
						BiaoCheItem biaoCheItem = objsList[i] as BiaoCheItem;
						GameManager.ClientMgr.NotifyMySelfDelBiaoChe(sl, pool, client, biaoCheItem.BiaoCheID);
					}
				}
			}
		}

		public static void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<int> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is BiaoCheItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as BiaoCheItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as BiaoCheItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as BiaoCheItem).PosX, (double)(list[i] as BiaoCheItem).PosY);
								if (Global.InCircle(target, center, (double)radius))
								{
									enemiesList.Add((list[i] as BiaoCheItem).BiaoCheID);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<BiaoCheItem> enemiesList, double angle)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				double loAngle = 0.0;
				double hiAngle = 0.0;
				Global.GetAngleRangeByDirection(direction, angle, out loAngle, out hiAngle);
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is BiaoCheItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as BiaoCheItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as BiaoCheItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as BiaoCheItem).PosX, (double)(list[i] as BiaoCheItem).PosY);
								if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
								{
									enemiesList.Add(list[i] as BiaoCheItem);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupEnemiesAtGridXY(IObject attacker, int gridX, int gridY, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			List<object> list = mapGrid.FindObjects(gridX, gridY);
			if (null != list)
			{
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is BiaoCheItem)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (list[i] as BiaoCheItem).CopyMapID)
						{
							enemiesList.Add(list[i]);
						}
					}
				}
			}
		}

		public static void LookupAttackEnemies(IObject attacker, int direction, List<object> enemiesList)
		{
			int currentMapCode = attacker.CurrentMapCode;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[currentMapCode];
			Point currentGrid = attacker.CurrentGrid;
			int gridX = (int)currentGrid.X;
			int gridY = (int)currentGrid.Y;
			Point gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY);
			BiaoCheManager.LookupEnemiesAtGridXY(attacker, (int)gridPointByDirection.X, (int)gridPointByDirection.Y, enemiesList);
		}

		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			BiaoCheManager.LookupAttackEnemies(attacker, direction, list);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as BiaoCheItem).BiaoCheID);
			}
		}

		public static void LookupRangeAttackEnemies(IObject obj, int toX, int toY, int direction, string rangeMode, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			int gridX = toX / mapGrid.MapGridWidth;
			int gridY = toY / mapGrid.MapGridHeight;
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, gridX, gridY, rangeMode, true);
			if (gridPointByDirection.Count > 0)
			{
				for (int i = 0; i < gridPointByDirection.Count; i++)
				{
					BiaoCheManager.LookupEnemiesAtGridXY(obj, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, enemiesList);
				}
			}
		}

		public static bool CanAttack(GameClient client, BiaoCheItem enemy)
		{
			bool result;
			if (null == enemy)
			{
				result = false;
			}
			else if (enemy.YaBiaoID == BiaoCheManager.NotAttackYaBiaoID)
			{
				result = false;
			}
			else
			{
				int num = (enemy.MaxLevel < 0) ? 1000 : enemy.MaxLevel;
				result = (client.ClientData.Level <= num);
			}
			return result;
		}

		public static int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int result = 0;
			if ((enemy as BiaoCheItem).CurrentLifeV > 0)
			{
				injure = (enemy as BiaoCheItem).CutLifeV;
				injure = (int)((double)injure * baseRate + (double)addVlue);
				injure = (int)((double)injure * injurePercent);
				result = injure;
				(enemy as BiaoCheItem).CurrentLifeV -= injure;
				(enemy as BiaoCheItem).CurrentLifeV = Global.GMax((enemy as BiaoCheItem).CurrentLifeV, 0);
				double num = (double)(enemy as BiaoCheItem).CurrentLifeV;
				(enemy as BiaoCheItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				GameManager.SystemServerEvents.AddEvent(string.Format("镖车减血, Injure={0}, Life={1}", injure, num), EventLevels.Debug);
				if ((enemy as BiaoCheItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("镖车死亡, roleID={0}", (enemy as BiaoCheItem).BiaoCheID), EventLevels.Debug);
					BiaoCheManager.ProcessBiaoCheDead(sl, pool, client, enemy as BiaoCheItem);
				}
				if ((enemy as BiaoCheItem).AttackedRoleID >= 0 && (enemy as BiaoCheItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient((enemy as BiaoCheItem).AttackedRoleID);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (enemy as BiaoCheItem).BiaoCheID, 0, 0, num, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, enemy.BiaoCheID, 0, 0, num, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as BiaoCheItem).BiaoCheID, burst, injure, num, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.BiaoCheID, burst, injure, num, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return result;
		}

		public static void NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int enemyX, int enemyY, int burst, int injure, double attackPercent, int addAttack, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			object obj = BiaoCheManager.FindBiaoCheByID(enemy);
			if (null != obj)
			{
				if ((obj as BiaoCheItem).CurrentLifeV > 0)
				{
					injure = (obj as BiaoCheItem).CutLifeV;
					injure = (int)((double)injure * baseRate + (double)addVlue);
					(obj as BiaoCheItem).CurrentLifeV -= injure;
					(obj as BiaoCheItem).CurrentLifeV = Global.GMax((obj as BiaoCheItem).CurrentLifeV, 0);
					double num = (double)(obj as BiaoCheItem).CurrentLifeV;
					(obj as BiaoCheItem).AttackedRoleID = client.ClientData.RoleID;
					GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
					GameManager.SystemServerEvents.AddEvent(string.Format("镖车减血, Injure={0}, Life={1}", injure, num), EventLevels.Debug);
					if ((obj as BiaoCheItem).CurrentLifeV <= 0)
					{
						GameManager.SystemServerEvents.AddEvent(string.Format("镖车死亡, roleID={0}", (obj as BiaoCheItem).BiaoCheID), EventLevels.Debug);
						BiaoCheManager.ProcessBiaoCheDead(sl, pool, client, obj as BiaoCheItem);
					}
					if ((obj as BiaoCheItem).AttackedRoleID >= 0 && (obj as BiaoCheItem).AttackedRoleID != client.ClientData.RoleID)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient((obj as BiaoCheItem).AttackedRoleID);
						if (null != gameClient)
						{
							GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, 0, 0, num, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, 0, 0, num, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, burst, injure, num, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
					ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, (obj as BiaoCheItem).BiaoCheID, burst, injure, num, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					if (!client.ClientData.DisableChangeRolePurpleName)
					{
						GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
					}
				}
			}
		}

		private static void ProcessBiaoCheDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, BiaoCheItem biaoCheItem)
		{
			if (!biaoCheItem.HandledDead)
			{
				biaoCheItem.HandledDead = true;
				biaoCheItem.BiaoCheDeadTicks = TimeUtil.NOW();
				GameClient gameClient;
				if (biaoCheItem.AttackedRoleID >= 0 && biaoCheItem.AttackedRoleID != client.ClientData.RoleID)
				{
					gameClient = GameManager.ClientMgr.FindClient(biaoCheItem.AttackedRoleID);
					if (null != gameClient)
					{
						client = gameClient;
					}
				}
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				Global.GetYaBiaoReward(biaoCheItem.YaBiaoID, out num, out num2, out num3);
				num /= 2;
				num2 = 0;
				int num4 = Global.IncTotayJieBiaoNum(client);
				if (num2 > 0)
				{
					GameManager.ClientMgr.ProcessRoleExperience(client, (long)num2, true, false, false, "none");
				}
				if (num > 0)
				{
					num = Global.FilterValue(client, num);
					GameManager.ClientMgr.AddUserYinLiang(Global._TCPManager.MySocketListener, Global._TCPManager.tcpClientPool, Global._TCPManager.TcpOutPacketPool, client, num, "押镖奖励", false);
					GameManager.SystemServerEvents.AddEvent(string.Format("角色获取银两, roleID={0}({1}), Money={2}, newMoney={3}", new object[]
					{
						client.ClientData.RoleID,
						client.ClientData.RoleName,
						client.ClientData.YinLiang,
						num
					}), EventLevels.Record);
				}
				GameManager.DBCmdMgr.AddDBCmd(10058, string.Format("{0}:{1}", biaoCheItem.OwnerRoleID, 1), null, client.ServerId);
				gameClient = GameManager.ClientMgr.FindClient(biaoCheItem.OwnerRoleID);
				if (null != gameClient)
				{
					if (null != gameClient.ClientData.MyYaBiaoData)
					{
						gameClient.ClientData.MyYaBiaoData.State = 1;
						GameManager.ClientMgr.NotifyYaBiaoData(gameClient);
					}
				}
				else
				{
					string text = string.Format("-setybstate2 {0} 1", biaoCheItem.OwnerRoleID);
					GameManager.DBCmdMgr.AddDBCmd(157, string.Format("{0}:{1}:{2}:{3}:{4}:{5}:{6}:{7}:{8}", new object[]
					{
						client.ClientData.RoleID,
						"",
						0,
						"",
						0,
						text,
						0,
						0,
						-1
					}), null, 0);
				}
				Global.BroadcastKillBiaoCheHint(client, biaoCheItem);
			}
		}

		public static int NotAttackYaBiaoID = -1;

		private static Dictionary<int, BiaoCheItem> _RoleID2BiaoCheDict = new Dictionary<int, BiaoCheItem>();

		private static Dictionary<int, BiaoCheItem> _ID2BiaoCheDict = new Dictionary<int, BiaoCheItem>();
	}
}
