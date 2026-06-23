using System;
using System.Collections.Generic;
using HSGameEngine.Drawing;
using HSGameEngine.GameEngine.Data;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.Scene;
using HSGameEngine.GameEngine.Sprite;
using Server.Tools;

namespace HSGameEngine.GameEngine.Logic
{
	public static class ChuanQiUtils
	{
		public static List<Point> GetGridList(Point p, int nDir, int nWalk, bool ignoreObs = true, GSprite sprite = null)
		{
			GMapData currentMapData = Global.CurrentMapData;
			List<Point> list = new List<Point>();
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return list;
			}
			int num = p.X / currentMapData.GridSizeX;
			int num2 = p.Y / currentMapData.GridSizeY;
			int num3 = num;
			int num4 = num2;
			int num5 = num3;
			int num6 = num4;
			for (int i = 0; i < nWalk; i++)
			{
				switch (nDir)
				{
				case 0:
					num6++;
					break;
				case 1:
					num5++;
					num6++;
					break;
				case 2:
					num5++;
					break;
				case 3:
					num5++;
					num6--;
					break;
				case 4:
					num6--;
					break;
				case 5:
					num5--;
					num6--;
					break;
				case 6:
					num5--;
					break;
				case 7:
					num5--;
					num6++;
					break;
				}
				if (!ignoreObs && sprite != null && !ChuanQiUtils.CanMove(sprite, num5, num6))
				{
					break;
				}
				list.Add(new Point(num5, num6));
			}
			return list;
		}

		public static void WalkNextPos1(int nCurrX, int nCurrY, int nDir, out int nX, out int nY)
		{
			nX = 0;
			nY = 0;
			nX = nCurrX;
			nY = nCurrY;
			switch (nDir)
			{
			case 0:
				nX = nCurrX;
				nY = nCurrY + 1;
				break;
			case 1:
				nX = nCurrX + 1;
				nY = nCurrY + 1;
				break;
			case 2:
				nX = nCurrX + 1;
				nY = nCurrY;
				break;
			case 3:
				nX = nCurrX + 1;
				nY = nCurrY - 1;
				break;
			case 4:
				nX = nCurrX;
				nY = nCurrY - 1;
				break;
			case 5:
				nX = nCurrX - 1;
				nY = nCurrY - 1;
				break;
			case 6:
				nX = nCurrX - 1;
				nY = nCurrY;
				break;
			case 7:
				nX = nCurrX - 1;
				nY = nCurrY + 1;
				break;
			}
		}

		public static void WalkNextPos(GSprite obj, int nDir, out int nX, out int nY)
		{
			nX = 0;
			nY = 0;
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return;
			}
			int nCurrX = obj.cx / currentMapData.GridSizeX;
			int nCurrY = obj.cy / currentMapData.GridSizeY;
			ChuanQiUtils.WalkNextPos1(nCurrX, nCurrY, nDir, out nX, out nY);
		}

		public static bool WalkTo(GScene scene, GSprite obj, int nDir, int extAction, Point toPos)
		{
			int num = 0;
			int num2 = 0;
			ChuanQiUtils.WalkNextPos(obj, nDir, out num, out num2);
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num3 = obj.cx / currentMapData.GridSizeX;
			int num4 = obj.cy / currentMapData.GridSizeY;
			int num5 = num3;
			int num6 = num4;
			string text = StringUtil.substitute("{0}_{1}|{2}_{3}", new object[]
			{
				num5,
				num6,
				num,
				num2
			});
			bool flag = ChuanQiUtils.WalkXY(obj, num, num2, nDir, text);
			if (flag)
			{
				ChuanQiUtils.StartStoryboard(scene, obj, text, num5, num6, num, num2, nDir, 1, extAction, toPos);
			}
			return flag;
		}

		public static bool RunTo(GScene scene, GSprite obj, int nDir, int extAction, Point toPos, bool ignoreObs = false)
		{
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num = obj.cx / currentMapData.GridSizeX;
			int num2 = obj.cy / currentMapData.GridSizeY;
			int num3 = num;
			int num4 = num2;
			int num5 = num3;
			int num6 = num4;
			int num7 = 2;
			string text = StringUtil.substitute("{0}_{1}", new object[]
			{
				num3,
				num4
			});
			for (int i = 0; i < num7; i++)
			{
				switch (nDir)
				{
				case 0:
					num6++;
					break;
				case 1:
					num5++;
					num6++;
					break;
				case 2:
					num5++;
					break;
				case 3:
					num5++;
					num6--;
					break;
				case 4:
					num6--;
					break;
				case 5:
					num5--;
					num6--;
					break;
				case 6:
					num5--;
					break;
				case 7:
					num5--;
					num6++;
					break;
				}
				if (!ignoreObs && !ChuanQiUtils.CanMove(obj, num5, num6))
				{
					return false;
				}
				text += StringUtil.substitute("|{0}_{1}", new object[]
				{
					num5,
					num6
				});
			}
			bool flag = ChuanQiUtils.RunXY(obj, num5, num6, nDir, text);
			if (flag)
			{
				ChuanQiUtils.StartStoryboard(scene, obj, text, num3, num4, num5, num6, nDir, 2, extAction, toPos);
			}
			return flag;
		}

		private static bool WalkXY(GSprite obj, int nX, int nY, int nDir, string pathStr)
		{
			if (!ChuanQiUtils.CanMove(obj, nX, nY))
			{
				return false;
			}
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num = obj.cx / currentMapData.GridSizeX;
			int num2 = obj.cy / currentMapData.GridSizeY;
			int oldGridX = num;
			int oldGridY = num2;
			MapGrid mapGrid = currentMapData._MapGrid;
			return mapGrid.MoveObjectEx(oldGridX, oldGridY, nX, nY, obj);
		}

		private static bool RunXY(GSprite obj, int nX, int nY, int nDir, string pathStr)
		{
			GMapData currentMapData = Global.CurrentMapData;
			if (currentMapData.GridSizeX == 0 || currentMapData.GridSizeY == 0)
			{
				return false;
			}
			int num = obj.cx / currentMapData.GridSizeX;
			int num2 = obj.cy / currentMapData.GridSizeY;
			int oldGridX = num;
			int oldGridY = num2;
			MapGrid mapGrid = currentMapData._MapGrid;
			return mapGrid.MoveObjectEx(oldGridX, oldGridY, nX, nY, obj);
		}

		public static bool CanMove(GSprite obj, int nX, int nY)
		{
			GMapData currentMapData = Global.CurrentMapData;
			return !Global.OnObstructionByGrid(new Point(nX, nY), currentMapData) && currentMapData._MapGrid.CanMove(0, nX, nY, 0, 0);
		}

		private static void StartStoryboard(GScene scene, GSprite obj, string pathString, int nSrcGridX, int nSrcGridY, int nDestGridX, int nDestGridY, int direction, int action, int extAction, Point toPos)
		{
			scene.StartStoryboard(obj, pathString, nSrcGridX, nSrcGridY, nDestGridX, nDestGridY, direction, action, extAction, toPos);
			GameInstance.Game.SpriteMoveTo(obj.Coordinate, toPos, action, extAction);
		}
	}
}
