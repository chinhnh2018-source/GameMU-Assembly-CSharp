using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using HSGameEngine.Tools.AStarEx;
using Server.Tools;

namespace GameServer.Logic
{
	public class MonsterMoving
	{
		public bool _LinearMove(Monster sprite, Point p, int action)
		{
			long num = TimeUtil.NOW();
			sprite.DestPoint = p;
			bool result = this.AStarMove(sprite, p, action);
			long num2 = TimeUtil.NOW();
			if (num2 > num + 100L)
			{
				LogManager.WriteLog(2, string.Format("_LinearMove 消耗:{0}毫秒, start({1}, {2}), to({3}, {4}), mapID={5}", new object[]
				{
					num2 - num,
					sprite.Coordinate.X,
					sprite.Coordinate.Y,
					p.X,
					p.Y,
					sprite.MonsterZoneNode.MapCode
				}), null, true);
			}
			return result;
		}

		public bool FindLinearNoObsMaxPoint(GameMap gameMap, Monster sprite, Point p, out Point maxPoint)
		{
			List<ANode> list = new List<ANode>();
			Global.Bresenham(list, (int)(sprite.Coordinate.X / (double)gameMap.MapGridWidth), (int)(sprite.Coordinate.Y / (double)gameMap.MapGridHeight), (int)(p.X / (double)gameMap.MapGridWidth), (int)(p.Y / (double)gameMap.MapGridHeight), gameMap.MyNodeGrid);
			bool result;
			if (list.Count > 1)
			{
				maxPoint = new Point((double)(list[list.Count - 1].x * gameMap.MapGridWidth + gameMap.MapGridWidth / 2), (double)(list[list.Count - 1].y * gameMap.MapGridHeight + gameMap.MapGridHeight / 2));
				list.Clear();
				result = true;
			}
			else
			{
				maxPoint = new Point(0.0, 0.0);
				result = false;
			}
			return result;
		}

		protected double CalcDirection(Point op, Point ep)
		{
			return Global.GetDirectionByTan(ep.X, ep.Y, op.X, op.Y);
		}

		private bool AStarMove(Monster sprite, Point p, int action)
		{
			Point coordinate = sprite.Coordinate;
			Point point = new Point
			{
				X = coordinate.X / 20.0,
				Y = coordinate.Y / 20.0
			};
			Point point2 = new Point
			{
				X = p.X / 20.0,
				Y = p.Y / 20.0
			};
			bool result;
			if (point.X == point2.X && point.Y == point2.Y)
			{
				result = true;
			}
			else
			{
				GameMap gameMap = GameManager.MapMgr.DictMaps[sprite.MonsterZoneNode.MapCode];
				if (point != point2)
				{
					List<ANode> list = null;
					gameMap.MyNodeGrid.setStartNode((int)point.X, (int)point.Y);
					gameMap.MyNodeGrid.setEndNode((int)point2.X, (int)point2.Y);
					try
					{
						list = gameMap.MyAStarFinder.find(gameMap.MyNodeGrid);
					}
					catch (Exception)
					{
						sprite.DestPoint = new Point(-1.0, -1.0);
						LogManager.WriteLog(2, string.Format("AStar怪物寻路失败, ExtenstionID={0}, Start=({1},{2}), End=({3},{4}), fixedObstruction=({5},{6})", new object[]
						{
							sprite.MonsterInfo.ExtensionID,
							(int)point.X,
							(int)point.Y,
							(int)point2.X,
							(int)point2.Y,
							gameMap.MyNodeGrid.numCols,
							gameMap.MyNodeGrid.numRows
						}), null, true);
						return false;
					}
					if (list == null || list.Count <= 1)
					{
						Point point3;
						if (this.FindLinearNoObsMaxPoint(gameMap, sprite, p, out point3))
						{
							list = null;
							point2 = new Point
							{
								X = point3.X / (double)gameMap.MapGridWidth,
								Y = point3.Y / (double)gameMap.MapGridHeight
							};
							p = point3;
							gameMap.MyNodeGrid.setStartNode((int)point.X, (int)point.Y);
							gameMap.MyNodeGrid.setEndNode((int)point2.X, (int)point2.Y);
							list = gameMap.MyAStarFinder.find(gameMap.MyNodeGrid);
						}
					}
					if (list == null || list.Count <= 1)
					{
						sprite.DestPoint = new Point(-1.0, -1.0);
						sprite.Action = GActions.Stand;
						Global.RemoveStoryboard(sprite.Name);
						return false;
					}
					sprite.Destination = p;
					double num = (double)Data.RunUnitCost;
					num /= sprite.MoveSpeed;
					num = 20.0 / num * (double)Global.MovingFrameRate;
					num *= 0.5;
					StoryBoardEx.RemoveStoryBoard(sprite.Name);
					StoryBoardEx storyBoardEx = new StoryBoardEx(sprite.Name);
					storyBoardEx.Completed = new StoryBoardEx.CompletedDelegateHandle(this.Move_Completed);
					Point ep = new Point((double)(list[0].x * gameMap.MapGridWidth), (double)(list[0].y * gameMap.MapGridHeight));
					sprite.Direction = this.CalcDirection(sprite.Coordinate, ep);
					sprite.Action = (GActions)action;
					storyBoardEx.Binding();
					sprite.FirstStoryMove = true;
					storyBoardEx.Start(sprite, list, num, 20);
				}
				result = true;
			}
			return result;
		}

		private void Move_Completed(object sender, EventArgs e)
		{
			Global.RemoveStoryboard((sender as StoryBoardEx).Name);
		}

		public double CalcDirection(Monster sprite, Point p)
		{
			return Global.GetDirectionByTan(p.X, p.Y, sprite.Coordinate.X, sprite.Coordinate.Y);
		}

		public void ChangeDirection(Monster sprite, double direction)
		{
			if (sprite.Direction != direction)
			{
				sprite.Direction = direction;
			}
		}

		public double ChangeDirection(Monster sprite, Point p)
		{
			double directionByTan = Global.GetDirectionByTan(p.X, p.Y, sprite.Coordinate.X, sprite.Coordinate.Y);
			if (sprite.Direction != directionByTan)
			{
				sprite.Direction = directionByTan;
			}
			return directionByTan;
		}
	}
}
