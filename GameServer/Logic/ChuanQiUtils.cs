using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using GameServer.Interface;
using Server.Tools;

namespace GameServer.Logic
{
	public class ChuanQiUtils
	{
		public static void TurnTo(IObject obj, Dircetions nDir)
		{
			if (nDir != obj.CurrentDir)
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[obj.CurrentMapCode];
				Point currentGrid = obj.CurrentGrid;
				int x = (int)((double)gameMap.MapGridWidth * currentGrid.X + (double)(gameMap.MapGridWidth / 2));
				int y = (int)((double)gameMap.MapGridHeight * currentGrid.Y + (double)(gameMap.MapGridHeight / 2));
				List<object> all9Clients = Global.GetAll9Clients(obj);
				GameManager.ClientMgr.NotifyOthersDoAction(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj, obj.CurrentMapCode, obj.CurrentCopyMapID, obj.GetObjectID(), (int)nDir, 0, x, y, 0, 0, 114, all9Clients);
				if (obj is Monster)
				{
					Monster monster = obj as Monster;
					monster.DestPoint = new Point(-1.0, -1.0);
					Global.RemoveStoryboard(monster.Name);
					monster.Direction = (double)nDir;
					monster.Action = GActions.Stand;
				}
			}
		}

		protected static void WalkNextPos(IObject obj, Dircetions nDir, out int nX, out int nY)
		{
			Point currentGrid = obj.CurrentGrid;
			int num = (int)currentGrid.X;
			int num2 = (int)currentGrid.Y;
			nX = num;
			nY = num2;
			switch (nDir)
			{
			case Dircetions.DR_UP:
				nX = num;
				nY = num2 + 1;
				break;
			case Dircetions.DR_UPRIGHT:
				nX = num + 1;
				nY = num2 + 1;
				break;
			case Dircetions.DR_RIGHT:
				nX = num + 1;
				nY = num2;
				break;
			case Dircetions.DR_DOWNRIGHT:
				nX = num + 1;
				nY = num2 - 1;
				break;
			case Dircetions.DR_DOWN:
				nX = num;
				nY = num2 - 1;
				break;
			case Dircetions.DR_DOWNLEFT:
				nX = num - 1;
				nY = num2 - 1;
				break;
			case Dircetions.DR_LEFT:
				nX = num - 1;
				nY = num2;
				break;
			case Dircetions.DR_UPLEFT:
				nX = num - 1;
				nY = num2 + 1;
				break;
			}
		}

		public static bool WalkTo(IObject obj, Dircetions nDir)
		{
			if (obj is Monster)
			{
				if (obj is Monster && (obj as Monster).IsMonsterDongJie())
				{
					return false;
				}
				long num = TimeUtil.NOW();
				if (obj is Monster && num - (obj as Monster).LastActionTick <= (long)(obj as Monster).GetMovingNeedTick())
				{
					return false;
				}
			}
			int num2;
			int num3;
			ChuanQiUtils.WalkNextPos(obj, nDir, out num2, out num3);
			Point currentGrid = obj.CurrentGrid;
			int num4 = (int)currentGrid.X;
			int num5 = (int)currentGrid.Y;
			string pathStr = string.Format("{0}_{1}|{2}_{3}", new object[]
			{
				num4,
				num5,
				num2,
				num3
			});
			bool flag = ChuanQiUtils.WalkXY(obj, num2, num3, nDir, pathStr);
			if (flag)
			{
			}
			return flag;
		}

		public static bool RunTo1(IObject obj, Dircetions nDir)
		{
			Point currentGrid = obj.CurrentGrid;
			int num = (int)currentGrid.X;
			int num2 = (int)currentGrid.Y;
			int num3 = num;
			int num4 = num2;
			int num5 = 2;
			string text = string.Format("{0}_{1}", num, num2);
			for (int i = 0; i < num5; i++)
			{
				switch (nDir)
				{
				case Dircetions.DR_UP:
					num4++;
					break;
				case Dircetions.DR_UPRIGHT:
					num3++;
					num4++;
					break;
				case Dircetions.DR_RIGHT:
					num3++;
					break;
				case Dircetions.DR_DOWNRIGHT:
					num3++;
					num4--;
					break;
				case Dircetions.DR_DOWN:
					num4--;
					break;
				case Dircetions.DR_DOWNLEFT:
					num3--;
					num4--;
					break;
				case Dircetions.DR_LEFT:
					num3--;
					break;
				case Dircetions.DR_UPLEFT:
					num3--;
					num4++;
					break;
				}
				if (!ChuanQiUtils.CanMove(obj, num3, num4))
				{
					return false;
				}
				text += string.Format("|{0}_{1}", num3, num4);
			}
			return ChuanQiUtils.RunXY1(obj, num3, num4, nDir, text);
		}

		public static bool RunTo(IObject obj, Dircetions nDir)
		{
			Point currentGrid = obj.CurrentGrid;
			int num = (int)currentGrid.X;
			int num2 = (int)currentGrid.Y;
			int num3 = num;
			int num4 = num2;
			int num5 = 2;
			string text = string.Format("{0}_{1}", num, num2);
			for (int i = 0; i < num5; i++)
			{
				switch (nDir)
				{
				case Dircetions.DR_UP:
					num4++;
					break;
				case Dircetions.DR_UPRIGHT:
					num3++;
					num4++;
					break;
				case Dircetions.DR_RIGHT:
					num3++;
					break;
				case Dircetions.DR_DOWNRIGHT:
					num3++;
					num4--;
					break;
				case Dircetions.DR_DOWN:
					num4--;
					break;
				case Dircetions.DR_DOWNLEFT:
					num3--;
					num4--;
					break;
				case Dircetions.DR_LEFT:
					num3--;
					break;
				case Dircetions.DR_UPLEFT:
					num3--;
					num4++;
					break;
				}
				if (!ChuanQiUtils.CanMove(obj, num3, num4))
				{
					return false;
				}
				text += string.Format("|{0}_{1}", num3, num4);
			}
			return ChuanQiUtils.RunXY(obj, num3, num4, nDir, text);
		}

		protected static bool WalkXY(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			bool result;
			if (!ChuanQiUtils.CanMove(obj, nX, nY))
			{
				result = false;
			}
			else
			{
				Point currentGrid = obj.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
				if (mapGrid.MoveObjectEx(num, num2, nX, nY, obj))
				{
					ChuanQiUtils.NotifyOthersMyMoving(obj, pathStr, num, num2, nX, nY, nDir);
					obj.CurrentGrid = new Point((double)nX, (double)nY);
					obj.CurrentDir = nDir;
					ChuanQiUtils.Notify9Grid(obj, false);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		protected static bool RunXY1(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			bool result;
			if (!ChuanQiUtils.CanMove(obj, nX, nY))
			{
				result = false;
			}
			else
			{
				Point currentGrid = obj.CurrentGrid;
				int num = (int)currentGrid.X;
				int num2 = (int)currentGrid.Y;
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
				if (mapGrid.MoveObjectEx(num, num2, nX, nY, obj))
				{
					ChuanQiUtils.NotifyOthersMyMoving1(obj, pathStr, num, num2, nX, nY, nDir);
					obj.CurrentGrid = new Point((double)nX, (double)nY);
					obj.CurrentDir = nDir;
					ChuanQiUtils.Notify9Grid(obj, false);
					result = true;
				}
				else
				{
					result = false;
				}
			}
			return result;
		}

		protected static bool RunXY(IObject obj, int nX, int nY, Dircetions nDir, string pathStr)
		{
			Point currentGrid = obj.CurrentGrid;
			int num = (int)currentGrid.X;
			int num2 = (int)currentGrid.Y;
			MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			bool result;
			if (mapGrid.MoveObjectEx(num, num2, nX, nY, obj))
			{
				ChuanQiUtils.NotifyOthersMyMoving(obj, pathStr, num, num2, nX, nY, nDir);
				obj.CurrentGrid = new Point((double)nX, (double)nY);
				obj.CurrentDir = nDir;
				ChuanQiUtils.Notify9Grid(obj, false);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool TransportTo(IObject obj, int nX, int nY, Dircetions nDir, int oldMapCode, string pathStr = "")
		{
			Point currentGrid = obj.CurrentGrid;
			int oldGridX = (int)currentGrid.X;
			int oldGridY = (int)currentGrid.Y;
			if (oldMapCode > 0 && oldMapCode != obj.CurrentMapCode)
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[oldMapCode];
				if (mapGrid != null)
				{
					mapGrid.RemoveObject(obj);
				}
				oldGridX = -1;
				oldGridY = -1;
			}
			MapGrid mapGrid2 = GameManager.MapGridMgr.DictGrids[obj.CurrentMapCode];
			bool result;
			if (mapGrid2.MoveObjectEx(oldGridX, oldGridY, nX, nY, obj))
			{
				obj.CurrentGrid = new Point((double)nX, (double)nY);
				obj.CurrentDir = nDir;
				if (obj is Monster && (obj as Monster).MonsterType == 1001)
				{
					GameManager.MonsterMgr.ChangePosition(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, obj as Monster, (int)obj.CurrentPos.X, (int)obj.CurrentPos.Y, (int)nDir, 159, 0);
				}
				ChuanQiUtils.Notify9Grid(obj, true);
				result = true;
			}
			else
			{
				result = false;
			}
			return result;
		}

		public static bool CanMonsterMoveOnCopyMap(Monster monster, int nX, int nY)
		{
			bool result;
			if (monster.CopyMapID <= 0)
			{
				result = false;
			}
			else if (Global.InOnlyObs(monster.ObjectType, monster.CurrentMapCode, nX, nY))
			{
				result = false;
			}
			else
			{
				MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[monster.CurrentMapCode];
				if (mapGrid.CanMove(monster.ObjectType, nX, nY, 0, 0))
				{
					result = true;
				}
				else
				{
					bool flag = true;
					List<object> list = mapGrid.FindObjects(nX, nY);
					if (null != list)
					{
						for (int i = 0; i < list.Count; i++)
						{
							if (list[i] != monster)
							{
								if (list[i] is GameClient && (list[i] as GameClient).CurrentCopyMapID == monster.CopyMapID)
								{
									flag = false;
									break;
								}
								if (list[i] is NPC)
								{
									flag = false;
									break;
								}
								if (list[i] is Monster && (list[i] as Monster).CopyMapID == monster.CopyMapID)
								{
									flag = false;
									break;
								}
							}
						}
					}
					result = flag;
				}
			}
			return result;
		}

		public static bool CanMove(IObject obj, int nX, int nY)
		{
			bool result;
			if (obj is Monster && (obj as Monster).CopyMapID > 0)
			{
				Monster monster = obj as Monster;
				result = (monster.CurrentMapCode == 5100 || (1502 == monster.MonsterType && monster.Tag is CompMineTruckConfig) || ChuanQiUtils.CanMonsterMoveOnCopyMap(monster, nX, nY));
			}
			else
			{
				result = !Global.InObsByGridXY(obj.ObjectType, obj.CurrentMapCode, nX, nY, 0, 0);
			}
			return result;
		}

		private static void Notify9Grid(IObject obj, bool force = false)
		{
			if (obj is Monster)
			{
			}
		}

		private static void NotifyOthersMyMoving(IObject obj, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, Dircetions direction)
		{
			if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
					monster.Direction = (double)direction;
					monster.Action = GActions.Walk;
					GameMap gameMap = GameManager.MapMgr.DictMaps[monster.MonsterZoneNode.MapCode];
					int currentX = gameMap.MapGridWidth * nSrcGridX + gameMap.MapGridWidth / 2;
					int currentY = gameMap.MapGridHeight * nSrcGridY + gameMap.MapGridHeight / 2;
					int toX = gameMap.MapGridWidth * nDestGridX + gameMap.MapGridWidth / 2;
					int toY = gameMap.MapGridHeight * nDestGridY + gameMap.MapGridHeight / 2;
					string pathString2 = DataHelper.ZipStringToBase64(pathString);
					GameManager.ClientMgr.NotifyOthersToMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, Global.GetMonsterStartMoveTicks(monster), currentX, currentY, 1, toX, toY, 107, monster.MoveSpeed, pathString2, null);
				}
			}
		}

		private static void NotifyOthersMyMoving1(IObject obj, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, Dircetions direction)
		{
			if (obj is Monster)
			{
				Monster monster = obj as Monster;
				if (null != monster)
				{
					monster.Direction = (double)direction;
					monster.Action = GActions.Run;
					GameMap gameMap = GameManager.MapMgr.DictMaps[monster.MonsterZoneNode.MapCode];
					int currentX = gameMap.MapGridWidth * nSrcGridX + gameMap.MapGridWidth / 2;
					int currentY = gameMap.MapGridHeight * nSrcGridY + gameMap.MapGridHeight / 2;
					int toX = gameMap.MapGridWidth * nDestGridX + gameMap.MapGridWidth / 2;
					int toY = gameMap.MapGridHeight * nDestGridY + gameMap.MapGridHeight / 2;
					string pathString2 = DataHelper.ZipStringToBase64(pathString);
					GameManager.ClientMgr.NotifyOthersToMoving(Global._TCPManager.MySocketListener, Global._TCPManager.TcpOutPacketPool, monster, monster.MonsterZoneNode.MapCode, monster.CopyMapID, monster.RoleID, Global.GetMonsterStartMoveTicks(monster), currentX, currentY, 2, toX, toY, 107, monster.MoveSpeed, pathString2, null);
				}
			}
		}

		public static Point HitFly(GameClient client, IObject enemy, int gridNum)
		{
			bool flag = false;
			if (enemy is Monster)
			{
				flag = ((enemy as Monster).VLife <= 0.0);
				if (101 != (enemy as Monster).MonsterType && 1001 != (enemy as Monster).MonsterType && 1801 != (enemy as Monster).MonsterType)
				{
					return new Point(-1.0, -1.0);
				}
			}
			Point currentGrid = enemy.CurrentGrid;
			Point currentGrid2 = client.CurrentGrid;
			int direction = (int)Global.GetDirectionByAspect((int)currentGrid.X, (int)currentGrid.Y, (int)currentGrid2.X, (int)currentGrid2.Y);
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, (int)currentGrid.X, (int)currentGrid.Y, gridNum);
			Point result;
			if (null == gridPointByDirection)
			{
				result = new Point(-1.0, -1.0);
			}
			else
			{
				if (!flag)
				{
					for (int i = 0; i < gridPointByDirection.Count; i++)
					{
						if (Global.InOnlyObs(enemy.ObjectType, client.ClientData.MapCode, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y))
						{
							gridPointByDirection.RemoveRange(i, gridPointByDirection.Count - i);
							break;
						}
					}
					if (gridPointByDirection.Count <= 0)
					{
						return new Point(-1.0, -1.0);
					}
				}
				Point point = gridPointByDirection[gridPointByDirection.Count - 1];
				if (!ChuanQiUtils.TransportTo(enemy, (int)point.X, (int)point.Y, enemy.CurrentDir, enemy.CurrentMapCode, ""))
				{
					result = new Point(-1.0, -1.0);
				}
				else
				{
					result = point;
				}
			}
			return result;
		}

		public static Point MonsterHitFly(Monster attacker, GameClient injurer, int gridNum)
		{
			bool flag = false;
			Point currentGrid = attacker.CurrentGrid;
			Point currentGrid2 = injurer.CurrentGrid;
			int direction = (int)Global.GetDirectionByAspect((int)currentGrid2.X, (int)currentGrid2.Y, (int)currentGrid.X, (int)currentGrid.Y);
			List<Point> gridPointByDirection = Global.GetGridPointByDirection(direction, (int)currentGrid2.X, (int)currentGrid2.Y, gridNum);
			Point result;
			if (null == gridPointByDirection)
			{
				result = new Point(-1.0, -1.0);
			}
			else
			{
				if (!flag)
				{
					for (int i = 0; i < gridPointByDirection.Count; i++)
					{
						if (Global.InOnlyObs(attacker.ObjectType, injurer.ClientData.MapCode, (int)gridPointByDirection[i].X, (int)gridPointByDirection[i].Y))
						{
							gridPointByDirection.RemoveRange(i, gridPointByDirection.Count - i);
							break;
						}
					}
					if (gridPointByDirection.Count <= 0)
					{
						return new Point(-1.0, -1.0);
					}
				}
				Point point = gridPointByDirection[gridPointByDirection.Count - 1];
				if (!ChuanQiUtils.TransportTo(injurer, (int)point.X, (int)point.Y, injurer.CurrentDir, injurer.CurrentMapCode, ""))
				{
					result = new Point(-1.0, -1.0);
				}
				else
				{
					result = point;
				}
			}
			return result;
		}
	}
}
