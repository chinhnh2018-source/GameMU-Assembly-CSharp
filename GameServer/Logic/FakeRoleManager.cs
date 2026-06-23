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
	public static class FakeRoleManager
	{
		private static FakeRoleItem AddFakeRole(SafeClientData clientData, FakeRoleTypes fakeRoleType)
		{
			FakeRoleItem fakeRoleItem = new FakeRoleItem
			{
				FakeRoleID = (int)GameManager.FakeRoleIDMgr.GetNewID(),
				FakeRoleType = (int)fakeRoleType,
				MyRoleDataMini = Global.ClientDataToRoleDataMini(clientData)
			};
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				FakeRoleManager._ID2FakeRoleDict[fakeRoleItem.FakeRoleID] = fakeRoleItem;
			}
			string key = string.Format("{0}_{1}", fakeRoleItem.MyRoleDataMini.RoleID, (int)fakeRoleType);
			lock (FakeRoleManager._RoleIDType2FakeRoleDict)
			{
				FakeRoleManager._RoleIDType2FakeRoleDict[key] = fakeRoleItem;
			}
			return fakeRoleItem;
		}

		private static FakeRoleItem AddFakeRole(RoleData4Selector clientData, FakeRoleTypes fakeRoleType)
		{
			FakeRoleItem result;
			if (null == clientData)
			{
				result = null;
			}
			else
			{
				FakeRoleItem fakeRoleItem = new FakeRoleItem
				{
					FakeRoleID = (int)GameManager.FakeRoleIDMgr.GetNewID(),
					FakeRoleType = (int)fakeRoleType,
					MyRoleDataMini = RoleManager.getInstance().GetRoleDataMiniFromRoleData4Selector(clientData)
				};
				lock (FakeRoleManager._ID2FakeRoleDict)
				{
					FakeRoleManager._ID2FakeRoleDict[fakeRoleItem.FakeRoleID] = fakeRoleItem;
				}
				string key = string.Format("{0}_{1}", fakeRoleItem.MyRoleDataMini.RoleID, (int)fakeRoleType);
				lock (FakeRoleManager._RoleIDType2FakeRoleDict)
				{
					FakeRoleManager._RoleIDType2FakeRoleDict[key] = fakeRoleItem;
				}
				result = fakeRoleItem;
			}
			return result;
		}

		public static FakeRoleItem FindFakeRoleByID(int FakeRoleID)
		{
			FakeRoleItem result = null;
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				FakeRoleManager._ID2FakeRoleDict.TryGetValue(FakeRoleID, out result);
			}
			return result;
		}

		public static FakeRoleItem FindFakeRoleByRoleIDType(int roleID, FakeRoleTypes fakeRoleType)
		{
			FakeRoleItem result = null;
			string key = string.Format("{0}_{1}", roleID, (int)fakeRoleType);
			lock (FakeRoleManager._RoleIDType2FakeRoleDict)
			{
				FakeRoleManager._RoleIDType2FakeRoleDict.TryGetValue(key, out result);
			}
			return result;
		}

		private static void RemoveFakeRole(int FakeRoleID)
		{
			FakeRoleItem fakeRoleItem = null;
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				FakeRoleManager._ID2FakeRoleDict.TryGetValue(FakeRoleID, out fakeRoleItem);
				if (null != fakeRoleItem)
				{
					FakeRoleManager._ID2FakeRoleDict.Remove(fakeRoleItem.FakeRoleID);
				}
			}
			if (null != fakeRoleItem)
			{
				string key = string.Format("{0}_{1}", fakeRoleItem.MyRoleDataMini.RoleID, fakeRoleItem.FakeRoleType);
				lock (FakeRoleManager._RoleIDType2FakeRoleDict)
				{
					FakeRoleManager._RoleIDType2FakeRoleDict.Remove(key);
				}
			}
		}

		private static void RemoveFakeRoleByRoleIDType(int roleID, FakeRoleTypes fakeRoleType)
		{
			FakeRoleItem fakeRoleItem = null;
			string key = string.Format("{0}_{1}", roleID, (int)fakeRoleType);
			lock (FakeRoleManager._RoleIDType2FakeRoleDict)
			{
				FakeRoleManager._RoleIDType2FakeRoleDict.TryGetValue(key, out fakeRoleItem);
				if (null != fakeRoleItem)
				{
					FakeRoleManager._RoleIDType2FakeRoleDict.Remove(key);
				}
			}
			if (null != fakeRoleItem)
			{
				lock (FakeRoleManager._ID2FakeRoleDict)
				{
					FakeRoleManager._ID2FakeRoleDict.Remove(fakeRoleItem.FakeRoleID);
				}
			}
		}

		private static List<FakeRoleItem> RemoveFakeRoleByType(FakeRoleTypes fakeRoleType)
		{
			List<FakeRoleItem> list = new List<FakeRoleItem>();
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				foreach (FakeRoleItem fakeRoleItem in FakeRoleManager._ID2FakeRoleDict.Values)
				{
					if (fakeRoleItem.FakeRoleType == (int)fakeRoleType)
					{
						list.Add(fakeRoleItem);
					}
				}
				foreach (FakeRoleItem fakeRoleItem in list)
				{
					FakeRoleManager._ID2FakeRoleDict.Remove(fakeRoleItem.FakeRoleID);
				}
			}
			lock (FakeRoleManager._RoleIDType2FakeRoleDict)
			{
				foreach (FakeRoleItem fakeRoleItem in list)
				{
					string key = string.Format("{0}_{1}", fakeRoleItem.MyRoleDataMini.RoleID, fakeRoleItem.FakeRoleType);
					FakeRoleManager._RoleIDType2FakeRoleDict.Remove(key);
				}
			}
			return list;
		}

		private static List<FakeRoleItem> GetFakeRoleListByType(FakeRoleTypes fakeRoleType)
		{
			List<FakeRoleItem> list = new List<FakeRoleItem>();
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				foreach (FakeRoleItem fakeRoleItem in FakeRoleManager._ID2FakeRoleDict.Values)
				{
					if (fakeRoleItem.FakeRoleType == (int)fakeRoleType)
					{
						list.Add(fakeRoleItem);
					}
				}
			}
			return list;
		}

		public static int ProcessNewFakeRole(SafeClientData clientData, int mapCode, FakeRoleTypes fakeRoleType, int direction = -1, int toPosX = -1, int toPosY = -1, int ToExtensionID = -1)
		{
			int result;
			if (mapCode <= 0 || !GameManager.MapGridMgr.DictGrids.ContainsKey(mapCode))
			{
				LogManager.WriteLog(2, string.Format("为RoleID离线挂机时失败, MapCode={0}, RoleID={1}", clientData.MapCode, clientData.RoleID), null, true);
				result = -1;
			}
			else
			{
				FakeRoleManager.RemoveFakeRoleByRoleIDType(clientData.RoleID, fakeRoleType);
				FakeRoleItem fakeRoleItem = FakeRoleManager.AddFakeRole(clientData, fakeRoleType);
				if (null == fakeRoleItem)
				{
					LogManager.WriteLog(2, string.Format("为RoleID生成假人对象时失败, MapCode={0}, RoleID={1}", clientData.MapCode, clientData.RoleID), null, true);
					result = -1;
				}
				else
				{
					fakeRoleItem.MyRoleDataMini.MapCode = mapCode;
					if (toPosX >= 0 && toPosY >= 0)
					{
						fakeRoleItem.MyRoleDataMini.PosX = toPosX;
						fakeRoleItem.MyRoleDataMini.PosY = toPosY;
					}
					if (direction >= 0)
					{
						fakeRoleItem.MyRoleDataMini.RoleDirection = direction;
					}
					if (ToExtensionID >= 0)
					{
						fakeRoleItem.ToExtensionID = ToExtensionID;
					}
					if (FakeRoleTypes.LiXianGuaJi == fakeRoleType)
					{
						if (clientData.OfflineMarketState <= 0)
						{
							fakeRoleItem.MyRoleDataMini.StallName = "";
						}
					}
					if (FakeRoleTypes.DiaoXiang2 == fakeRoleType)
					{
						if (fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams == null || fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Count <= 0)
						{
							int value = 0;
							foreach (FashionData fashionData in FashionManager.getInstance().RuntimeData.FashingDict.Values)
							{
								if (fashionData.Type == 1)
								{
									value = fashionData.ID;
									break;
								}
							}
							if (null == fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams)
							{
								fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams = new List<int>();
							}
							for (int i = fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Count; i < 53; i++)
							{
								fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Add(0);
							}
							fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams[26] = value;
						}
					}
					if (FakeRoleTypes.BangHuiMatchBZ == fakeRoleType || FakeRoleTypes.CompDaLingZhu_1 == fakeRoleType || FakeRoleTypes.CompDaLingZhu_2 == fakeRoleType || FakeRoleTypes.CompDaLingZhu_3 == fakeRoleType)
					{
						int value = 0;
						foreach (FashionData fashionData in FashionManager.getInstance().RuntimeData.FashingDict.Values)
						{
							if (fashionData.Type == 4)
							{
								value = fashionData.ID;
								break;
							}
						}
						if (fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams == null || fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Count <= 0)
						{
							if (null == fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams)
							{
								fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams = new List<int>();
							}
							for (int i = fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Count; i < 53; i++)
							{
								fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Add(0);
							}
							fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams[26] = value;
						}
						else if (53 == fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams.Count)
						{
							fakeRoleItem.MyRoleDataMini.RoleCommonUseIntPamams[26] = value;
						}
					}
					fakeRoleItem.MyRoleDataMini.LifeV = Math.Max(1, clientData.LifeV);
					fakeRoleItem.MyRoleDataMini.MagicV = Math.Max(1, clientData.MagicV);
					GameManager.MapGridMgr.DictGrids[fakeRoleItem.MyRoleDataMini.MapCode].MoveObject(-1, -1, fakeRoleItem.MyRoleDataMini.PosX, fakeRoleItem.MyRoleDataMini.PosY, fakeRoleItem);
					result = fakeRoleItem.FakeRoleID;
				}
			}
			return result;
		}

		public static int ProcessNewFakeRole(RoleData4Selector clientData, int mapCode, FakeRoleTypes fakeRoleType, int direction = -1, int toPosX = -1, int toPosY = -1, int ToExtensionID = -1)
		{
			int result;
			if (mapCode <= 0 || !GameManager.MapGridMgr.DictGrids.ContainsKey(mapCode))
			{
				LogManager.WriteLog(2, string.Format("生成雕像目标地图不存在, MapCode={0}, RoleID={1}", mapCode, clientData.RoleID), null, true);
				result = -1;
			}
			else
			{
				FakeRoleManager.RemoveFakeRoleByRoleIDType(clientData.RoleID, fakeRoleType);
				FakeRoleItem fakeRoleItem = FakeRoleManager.AddFakeRole(clientData, fakeRoleType);
				if (null == fakeRoleItem)
				{
					LogManager.WriteLog(2, string.Format("为RoleID生成假人对象时失败, MapCode={0}, RoleID={1}", mapCode, clientData.RoleID), null, true);
					result = -1;
				}
				else
				{
					fakeRoleItem.MyRoleDataMini.MapCode = mapCode;
					if (toPosX >= 0 && toPosY >= 0)
					{
						fakeRoleItem.MyRoleDataMini.PosX = toPosX;
						fakeRoleItem.MyRoleDataMini.PosY = toPosY;
					}
					if (direction >= 0)
					{
						fakeRoleItem.MyRoleDataMini.RoleDirection = direction;
					}
					if (ToExtensionID >= 0)
					{
						fakeRoleItem.ToExtensionID = ToExtensionID;
					}
					GameManager.MapGridMgr.DictGrids[fakeRoleItem.MyRoleDataMini.MapCode].MoveObject(-1, -1, fakeRoleItem.MyRoleDataMini.PosX, fakeRoleItem.MyRoleDataMini.PosY, fakeRoleItem);
					result = fakeRoleItem.FakeRoleID;
				}
			}
			return result;
		}

		public static void ProcessDelFakeRoleByType(FakeRoleTypes fakeRoleType, bool bBroadcastDelMsg = false)
		{
			List<FakeRoleItem> fakeRoleListByType = FakeRoleManager.GetFakeRoleListByType(fakeRoleType);
			foreach (FakeRoleItem fakeRoleItem in fakeRoleListByType)
			{
				FakeRoleManager.ProcessDelFakeRole(fakeRoleItem.FakeRoleID, bBroadcastDelMsg);
			}
		}

		public static void ProcessDelFakeRole(int FakeRoleID, bool bBroadcastDelMsg = false)
		{
			FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(FakeRoleID);
			if (null != fakeRoleItem)
			{
				FakeRoleManager.RemoveFakeRole(FakeRoleID);
				GameManager.MapGridMgr.DictGrids[fakeRoleItem.MyRoleDataMini.MapCode].RemoveObject(fakeRoleItem);
				if (bBroadcastDelMsg)
				{
					GameManager.ClientMgr.NotifyAllDelFakeRole(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, fakeRoleItem);
				}
			}
		}

		public static void ProcessFakeRoleGoBack(int FakeRoleID)
		{
			FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByID(FakeRoleID);
			if (null != fakeRoleItem)
			{
				GameManager.ClientMgr.NotifyAllDelFakeRole(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, fakeRoleItem);
				int currentMapCode = fakeRoleItem.CurrentMapCode;
				GameMap gameMap = null;
				if (GameManager.MapMgr.DictMaps.TryGetValue(currentMapCode, out gameMap))
				{
					int defaultBirthPosX = gameMap.DefaultBirthPosX;
					int defaultBirthPosY = gameMap.DefaultBirthPosY;
					int birthRadius = gameMap.BirthRadius;
					Point mapPoint = Global.GetMapPoint(ObjectTypes.OT_FAKEROLE, currentMapCode, defaultBirthPosX, defaultBirthPosY, birthRadius);
					int num = (int)mapPoint.X;
					int num2 = (int)mapPoint.Y;
					int posX = fakeRoleItem.MyRoleDataMini.PosX;
					int posY = fakeRoleItem.MyRoleDataMini.PosY;
					fakeRoleItem.MyRoleDataMini.PosX = num;
					fakeRoleItem.MyRoleDataMini.PosY = num2;
					fakeRoleItem.MyRoleDataMini.LifeV = fakeRoleItem.MyRoleDataMini.MaxLifeV;
					GameManager.MapGridMgr.DictGrids[currentMapCode].MoveObject(posX, posY, num, num2, fakeRoleItem);
				}
			}
		}

		public static void ProcessDelFakeRole(int roleID, FakeRoleTypes fakeRoleType)
		{
			FakeRoleItem fakeRoleItem = FakeRoleManager.FindFakeRoleByRoleIDType(roleID, fakeRoleType);
			if (null != fakeRoleItem)
			{
				FakeRoleManager.RemoveFakeRole(fakeRoleItem.FakeRoleID);
				GameManager.MapGridMgr.DictGrids[fakeRoleItem.MyRoleDataMini.MapCode].RemoveObject(fakeRoleItem);
			}
		}

		public static void NotifyOthersShowFakeRole(SocketListener sl, TCPOutPacketPool pool, FakeRoleItem FakeRoleItem)
		{
			if (null != FakeRoleItem)
			{
				GameManager.MapGridMgr.DictGrids[FakeRoleItem.MyRoleDataMini.MapCode].MoveObject(-1, -1, FakeRoleItem.MyRoleDataMini.PosX, FakeRoleItem.MyRoleDataMini.PosY, FakeRoleItem);
			}
		}

		public static void NotifyOthersHideFakeRole(SocketListener sl, TCPOutPacketPool pool, FakeRoleItem FakeRoleItem)
		{
			if (null != FakeRoleItem)
			{
				GameManager.MapGridMgr.DictGrids[FakeRoleItem.MyRoleDataMini.MapCode].RemoveObject(FakeRoleItem);
			}
		}

		private static bool ProcessFakeRoleDead(SocketListener sl, TCPOutPacketPool pool, long nowTicks, FakeRoleItem fakeRoleItem)
		{
			bool result;
			if (fakeRoleItem.CurrentLifeV > 0)
			{
				result = false;
			}
			else
			{
				long num = nowTicks - fakeRoleItem.FakeRoleDeadTicks;
				if (num < 2000L)
				{
					result = false;
				}
				else
				{
					FakeRoleManager.ProcessFakeRoleGoBack(fakeRoleItem.FakeRoleID);
					result = true;
				}
			}
			return result;
		}

		public static void ProcessAllFakeRoleItems(SocketListener sl, TCPOutPacketPool pool)
		{
			List<FakeRoleItem> list = new List<FakeRoleItem>();
			lock (FakeRoleManager._ID2FakeRoleDict)
			{
				foreach (FakeRoleItem item in FakeRoleManager._ID2FakeRoleDict.Values)
				{
					list.Add(item);
				}
			}
			long nowTicks = TimeUtil.NOW();
			for (int i = 0; i < list.Count; i++)
			{
				FakeRoleItem fakeRoleItem = list[i];
				if (FakeRoleManager.ProcessFakeRoleDead(sl, pool, nowTicks, fakeRoleItem))
				{
				}
			}
		}

		public static void SendMySelfFakeRoleItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList, int totalRoleAndMonsterNum)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					FakeRoleItem fakeRoleItem = objsList[i] as FakeRoleItem;
					if (null != fakeRoleItem)
					{
						if (GameManager.TestGameShowFakeRoleForUser || (fakeRoleItem.FakeRoleType != 1 && fakeRoleItem.FakeRoleType != 2))
						{
							if (fakeRoleItem.CurrentLifeV > 0)
							{
								if (totalRoleAndMonsterNum >= 30)
								{
									if (fakeRoleItem.FakeRoleType == 2)
									{
										goto IL_A0;
									}
								}
								GameManager.ClientMgr.NotifyMySelfNewFakeRole(sl, pool, client, fakeRoleItem);
							}
						}
					}
					IL_A0:;
				}
			}
		}

		public static void DelMySelfFakeRoleItems(SocketListener sl, TCPOutPacketPool pool, GameClient client, List<object> objsList)
		{
			if (null != objsList)
			{
				for (int i = 0; i < objsList.Count; i++)
				{
					FakeRoleItem fakeRoleItem = objsList[i] as FakeRoleItem;
					if (null != fakeRoleItem)
					{
						if (GameManager.TestGameShowFakeRoleForUser || (fakeRoleItem.FakeRoleType != 1 && fakeRoleItem.FakeRoleType != 2))
						{
							GameManager.ClientMgr.NotifyMySelfDelFakeRole(sl, pool, client, fakeRoleItem.FakeRoleID);
						}
					}
				}
			}
		}

		public static void LookupEnemiesInCircle(GameClient client, int mapCode, int toX, int toY, int radius, List<object> enemiesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(toX, toY, radius);
			if (null != list)
			{
				Point center = new Point((double)toX, (double)toY);
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is FakeRoleItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as FakeRoleItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as FakeRoleItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as FakeRoleItem).MyRoleDataMini.PosX, (double)(list[i] as FakeRoleItem).MyRoleDataMini.PosY);
								if (Global.InCircle(target, center, (double)radius))
								{
									enemiesList.Add(list[i] as FakeRoleItem);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<int> enemiesList, double angle, bool near180)
		{
			List<object> list = new List<object>();
			FakeRoleManager.LookupEnemiesInCircleByAngle(client, direction, mapCode, toX, toY, radius, list, angle, near180);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as FakeRoleItem).FakeRoleID);
			}
		}

		public static void LookupEnemiesInCircleByAngle(GameClient client, int direction, int mapCode, int toX, int toY, int radius, List<object> enemiesList, double angle, bool near180)
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
					if (list[i] is FakeRoleItem)
					{
						if (client == null || Global.IsOpposition(client, list[i] as FakeRoleItem))
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as FakeRoleItem).CopyMapID)
							{
								Point target = new Point((double)(list[i] as FakeRoleItem).MyRoleDataMini.PosX, (double)(list[i] as FakeRoleItem).MyRoleDataMini.PosY);
								if (Global.InCircleByAngle(target, center, (double)radius, loAngle, hiAngle))
								{
									enemiesList.Add(list[i]);
								}
							}
						}
					}
				}
			}
		}

		public static void LookupRolesInSquare(GameClient client, int mapCode, int radius, int nWidth, List<object> rolesList)
		{
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[mapCode];
			List<object> list = mapGrid.FindObjects(client.ClientData.PosX, client.ClientData.PosY, radius);
			if (null != list)
			{
				Point center = new Point((double)client.ClientData.PosX, (double)client.ClientData.PosY);
				Point apointInCircle = Global.GetAPointInCircle(center, radius, client.ClientData.RoleYAngle);
				int num = (int)apointInCircle.X;
				int num2 = (int)apointInCircle.Y;
				Point center2 = default(Point);
				center2.X = (double)((client.ClientData.PosX + num) / 2);
				center2.Y = (double)((client.ClientData.PosY + num2) / 2);
				int directionX = num - client.ClientData.PosX;
				int directionY = num2 - client.ClientData.PosY;
				for (int i = 0; i < list.Count; i++)
				{
					if (list[i] is FakeRoleItem)
					{
						if ((list[i] as FakeRoleItem).CurrentLifeV > 0)
						{
							if (client == null || client.ClientData.CopyMapID == (list[i] as FakeRoleItem).CopyMapID)
							{
								Point target = new Point((list[i] as FakeRoleItem).CurrentPos.X, (list[i] as FakeRoleItem).CurrentPos.Y);
								if (Global.InSquare(center2, target, radius, nWidth, directionX, directionY))
								{
									rolesList.Add(list[i]);
								}
								else if (Global.InCircle(target, center, 100.0))
								{
									rolesList.Add(list[i]);
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
					if (list[i] is FakeRoleItem)
					{
						if (attacker == null || attacker.CurrentCopyMapID == (list[i] as FakeRoleItem).CopyMapID)
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
			FakeRoleManager.LookupEnemiesAtGridXY(attacker, (int)gridPointByDirection.X, (int)gridPointByDirection.Y, enemiesList);
		}

		public static void LookupAttackEnemyIDs(IObject attacker, int direction, List<int> enemiesList)
		{
			List<object> list = new List<object>();
			FakeRoleManager.LookupAttackEnemies(attacker, direction, list);
			for (int i = 0; i < list.Count; i++)
			{
				enemiesList.Add((list[i] as FakeRoleItem).FakeRoleID);
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
					FakeRoleManager.LookupEnemiesAtGridXY(obj, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y, enemiesList);
				}
			}
		}

		public static bool CanAttack(FakeRoleItem enemy)
		{
			return !GameManager.TestGameShowFakeRoleForUser && null != enemy && enemy.GetFakeRoleData().FakeRoleType == 2;
		}

		public static int NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, FakeRoleItem enemy, int burst, int injure, double injurePercent, int attackType, bool forceBurst, int addInjure, double attackPercent, int addAttackMin, int addAttackMax, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			int result = 0;
			if ((enemy as FakeRoleItem).CurrentLifeV > 0)
			{
				injure = 1000;
				injure = (int)((double)injure * injurePercent);
				result = injure;
				(enemy as FakeRoleItem).CurrentLifeV -= injure;
				(enemy as FakeRoleItem).CurrentLifeV = Global.GMax((enemy as FakeRoleItem).CurrentLifeV, 0);
				int currentLifeV = (enemy as FakeRoleItem).CurrentLifeV;
				(enemy as FakeRoleItem).AttackedRoleID = client.ClientData.RoleID;
				GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
				(enemy as FakeRoleItem).AddAttacker(client.ClientData.RoleID, Global.GMax(0, injure));
				GameManager.SystemServerEvents.AddEvent(string.Format("假人减血, Injure={0}, Life={1}", injure, currentLifeV), EventLevels.Debug);
				if ((enemy as FakeRoleItem).CurrentLifeV <= 0)
				{
					GameManager.SystemServerEvents.AddEvent(string.Format("假人死亡, roleID={0}", (enemy as FakeRoleItem).FakeRoleID), EventLevels.Debug);
					FakeRoleManager.ProcessFakeRoleDead(sl, pool, client, enemy as FakeRoleItem);
				}
				Point hitToGrid = new Point(-1.0, -1.0);
				if (nHitFlyDistance > 0)
				{
					MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
					int num = nHitFlyDistance * 100 / mapGrid.MapGridWidth;
					if (num > 0)
					{
						hitToGrid = ChuanQiUtils.HitFly(client, enemy, num);
					}
				}
				if ((enemy as FakeRoleItem).AttackedRoleID >= 0 && (enemy as FakeRoleItem).AttackedRoleID != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient((enemy as FakeRoleItem).AttackedRoleID);
					if (null != gameClient)
					{
						GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (enemy as FakeRoleItem).FakeRoleID, 0, 0, (double)currentLifeV, gameClient.ClientData.Level, hitToGrid, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, enemy.FakeRoleID, 0, 0, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					}
				}
				GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (enemy as FakeRoleItem).FakeRoleID, burst, injure, (double)currentLifeV, client.ClientData.Level, hitToGrid, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, enemy.FakeRoleID, burst, injure, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
				if (!client.ClientData.DisableChangeRolePurpleName)
				{
					GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
				}
			}
			return result;
		}

		public static void NotifyInjured(SocketListener sl, TCPOutPacketPool pool, GameClient client, int roleID, int enemy, int enemyX, int enemyY, int burst, int injure, double attackPercent, int addAttack, double baseRate = 1.0, int addVlue = 0, int nHitFlyDistance = 0)
		{
			object obj = FakeRoleManager.FindFakeRoleByID(enemy);
			if (null != obj)
			{
				if ((obj as FakeRoleItem).CurrentLifeV > 0)
				{
					injure = 10000;
					(obj as FakeRoleItem).CurrentLifeV -= injure;
					(obj as FakeRoleItem).CurrentLifeV = Global.GMax((obj as FakeRoleItem).CurrentLifeV, 0);
					int currentLifeV = (obj as FakeRoleItem).CurrentLifeV;
					(obj as FakeRoleItem).AttackedRoleID = client.ClientData.RoleID;
					GameManager.ClientMgr.SpriteInjure2Blood(sl, pool, client, injure);
					GameManager.SystemServerEvents.AddEvent(string.Format("假人减血, Injure={0}, Life={1}", injure, currentLifeV), EventLevels.Debug);
					if ((obj as FakeRoleItem).CurrentLifeV <= 0)
					{
						GameManager.SystemServerEvents.AddEvent(string.Format("假人死亡, roleID={0}", (obj as FakeRoleItem).FakeRoleID), EventLevels.Debug);
						FakeRoleManager.ProcessFakeRoleDead(sl, pool, client, obj as FakeRoleItem);
					}
					int attackerFromList = (obj as FakeRoleItem).GetAttackerFromList();
					if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
					{
						GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
						if (null != gameClient)
						{
							GameManager.ClientMgr.NotifySpriteInjured(sl, pool, gameClient, gameClient.ClientData.MapCode, gameClient.ClientData.RoleID, (obj as FakeRoleItem).FakeRoleID, 0, 0, (double)currentLifeV, gameClient.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
							ClientManager.NotifySelfEnemyInjured(sl, pool, gameClient, gameClient.ClientData.RoleID, (obj as FakeRoleItem).FakeRoleID, 0, 0, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
						}
					}
					GameManager.ClientMgr.NotifySpriteInjured(sl, pool, client, client.ClientData.MapCode, client.ClientData.RoleID, (obj as FakeRoleItem).FakeRoleID, burst, injure, (double)currentLifeV, client.ClientData.Level, new Point(-1.0, -1.0), 0, EMerlinSecretAttrType.EMSAT_None, 0);
					ClientManager.NotifySelfEnemyInjured(sl, pool, client, client.ClientData.RoleID, (obj as FakeRoleItem).FakeRoleID, burst, injure, (double)currentLifeV, 0L, 0, EMerlinSecretAttrType.EMSAT_None, 0);
					if (!client.ClientData.DisableChangeRolePurpleName)
					{
						GameManager.ClientMgr.ForceChangeRolePurpleName2(sl, pool, client);
					}
				}
			}
		}

		private static void ProcessFakeRoleDead(SocketListener sl, TCPOutPacketPool pool, GameClient client, FakeRoleItem fakeRoleItem)
		{
			if (!fakeRoleItem.HandledDead)
			{
				fakeRoleItem.HandledDead = true;
				fakeRoleItem.FakeRoleDeadTicks = TimeUtil.NOW();
				int attackerFromList = fakeRoleItem.GetAttackerFromList();
				if (attackerFromList >= 0 && attackerFromList != client.ClientData.RoleID)
				{
					GameClient gameClient = GameManager.ClientMgr.FindClient(attackerFromList);
					if (null != gameClient)
					{
						client = gameClient;
					}
				}
			}
		}

		private static Dictionary<int, FakeRoleItem> _ID2FakeRoleDict = new Dictionary<int, FakeRoleItem>();

		private static Dictionary<string, FakeRoleItem> _RoleIDType2FakeRoleDict = new Dictionary<string, FakeRoleItem>();
	}
}
