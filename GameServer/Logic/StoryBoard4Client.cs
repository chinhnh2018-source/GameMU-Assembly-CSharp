using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;

namespace GameServer.Logic
{
	public class StoryBoard4Client
	{
		public static StoryBoard4Client FindStoryBoard(int roleID)
		{
			StoryBoard4Client result = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out result);
			}
			return result;
		}

		public static void RemoveStoryBoard(int roleID)
		{
			StoryBoard4Client storyBoard4Client = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBoard4Client))
				{
					StoryBoard4Client.StoryBoardDict.Remove(roleID);
					if (null != storyBoard4Client)
					{
						storyBoard4Client.Completed = null;
					}
				}
			}
		}

		public static StoryBoard4Client StopStoryBoard(int roleID, long clientTicks)
		{
			StoryBoard4Client storyBoard4Client = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBoard4Client))
				{
					return null;
				}
				StoryBoard4Client.StoryBoardDict.Remove(roleID);
			}
			if (null != storyBoard4Client)
			{
				storyBoard4Client.Run(clientTicks);
			}
			return storyBoard4Client;
		}

		public static void StopStoryBoard(int roleID, int stopIndex)
		{
			StoryBoard4Client storyBoard4Client = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBoard4Client))
				{
					return;
				}
			}
			if (null != storyBoard4Client)
			{
				storyBoard4Client.StopOnNextGrid(stopIndex);
			}
		}

		public static int GetStoryBoardPathIndex(int roleID)
		{
			StoryBoard4Client storyBoard4Client = null;
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.TryGetValue(roleID, out storyBoard4Client))
				{
					return 0;
				}
			}
			int result;
			if (null != storyBoard4Client)
			{
				result = storyBoard4Client.GetStoryBoardPathIndex();
			}
			else
			{
				result = 0;
			}
			return result;
		}

		public static void ClearStoryBoard()
		{
			List<StoryBoard4Client> list = new List<StoryBoard4Client>();
			lock (StoryBoard4Client.StoryBoardDict)
			{
				foreach (StoryBoard4Client storyBoard4Client in StoryBoard4Client.StoryBoardDict.Values)
				{
					list.Add(storyBoard4Client);
				}
				StoryBoard4Client.StoryBoardDict.Clear();
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoard4Client storyBoard4Client = list[i];
				if (null != storyBoard4Client)
				{
					storyBoard4Client.Completed = null;
					storyBoard4Client.Clear();
				}
			}
		}

		private static long getMyTimer()
		{
			return TimeUtil.NOW();
		}

		public static void runStoryBoards()
		{
			long myTimer = StoryBoard4Client.getMyTimer();
			StoryBoard4Client.LastRunStoryTicks = myTimer;
			List<StoryBoard4Client> list = new List<StoryBoard4Client>();
			lock (StoryBoard4Client.StoryBoardDict)
			{
				foreach (StoryBoard4Client storyBoard4Client in StoryBoard4Client.StoryBoardDict.Values)
				{
					list.Add(storyBoard4Client);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoard4Client storyBoard4Client = list[i];
				if (null != storyBoard4Client)
				{
					storyBoard4Client.Run(myTimer);
				}
			}
		}

		public event StoryBoard4Client.CompletedDelegateHandle _Completed = null;

		public StoryBoard4Client.CompletedDelegateHandle Completed
		{
			get
			{
				return this._Completed;
			}
			set
			{
				this._Completed = value;
			}
		}

		public StoryBoard4Client(int roleID)
		{
			this._RoleID = roleID;
		}

		public int RoleID
		{
			get
			{
				return this._RoleID;
			}
		}

		public void Binding()
		{
			lock (StoryBoard4Client.StoryBoardDict)
			{
				if (!StoryBoard4Client.StoryBoardDict.ContainsKey(this._RoleID))
				{
					StoryBoard4Client.StoryBoardDict.Add(this._RoleID, this);
				}
			}
		}

		public void UnBinding()
		{
			this.Clear();
		}

		public void Clear()
		{
			if (-1 != this._RoleID)
			{
				StoryBoard4Client.RemoveStoryBoard(this._RoleID);
			}
		}

		public int CurrentX
		{
			get
			{
				return this._LastTargetX;
			}
		}

		public int CurrentY
		{
			get
			{
				return this._LastTargetY;
			}
		}

		public bool Start(GameClient client, List<Point> path, int cellSizeX, int cellSizeY, long elapsedTicks)
		{
			bool result;
			lock (this.mutex)
			{
				if (this._Started)
				{
					result = false;
				}
				else
				{
					this._CellSizeX = cellSizeX;
					this._CellSizeY = cellSizeY;
					this._PathIndex = 0;
					this._LastRunTicks = StoryBoard4Client.getMyTimer() - elapsedTicks;
					this._LastTargetX = client.ClientData.PosX;
					this._LastTargetY = client.ClientData.PosY;
					this._CurrentX = (double)client.ClientData.PosX;
					this._CurrentY = (double)client.ClientData.PosY;
					this._Path = path;
					this._CompletedState = false;
					this._Started = true;
					this._Stopped = false;
					this._LastStopIndex = -1;
					this._FirstPoint = new Point(this._Path[0].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[0].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
					if (this._Path.Count <= 0)
					{
						this._LastPoint = this._FirstPoint;
					}
					else
					{
						this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
					}
					result = true;
				}
			}
			return result;
		}

		private void StopOnNextGrid(int stopIndex)
		{
			lock (this.mutex)
			{
				if (!this._CompletedState)
				{
					if (stopIndex >= 0)
					{
						if (stopIndex < this._Path.Count)
						{
							this._Path.RemoveRange(stopIndex, this._Path.Count - stopIndex);
							if (this._Path.Count <= 0)
							{
								this._LastPoint = this._FirstPoint;
							}
							else
							{
								this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
							}
						}
					}
				}
			}
		}

		public bool IsStopped()
		{
			bool stopped;
			lock (this.mutex)
			{
				stopped = this._Stopped;
			}
			return stopped;
		}

		public int GetStoryBoardPathIndex()
		{
			int pathIndex;
			lock (this.mutex)
			{
				pathIndex = this._PathIndex;
			}
			return pathIndex;
		}

		public Point LastPoint
		{
			get
			{
				return this._LastPoint;
			}
		}

		public void Run(long currentTicks)
		{
			lock (this.mutex)
			{
				if (this._Started)
				{
					if (!this._CompletedState)
					{
						long num = currentTicks - this._LastRunTicks;
						this._LastRunTicks = currentTicks;
						GameClient client = GameManager.ClientMgr.FindClient(this._RoleID);
						double num2 = (double)num / 1000.0;
						double toMoveDist = num2 * this._MovingSpeedPerSec * this.GetClientMoveSpeed(client);
						if (this.StepMove(toMoveDist, client))
						{
							this._CompletedState = true;
							if (null != this._Completed)
							{
								this._Completed(this, null);
							}
						}
					}
				}
			}
		}

		private double GetClientMoveSpeed(GameClient client)
		{
			double result;
			if (null != client)
			{
				result = Math.Max(0.1, Math.Min(2.5, client.ClientData.MoveSpeed));
			}
			else
			{
				result = 1.0;
			}
			return result;
		}

		private static long GetNeedTicks(bool needWalking, int dir)
		{
			int num = needWalking ? 225 : 125;
			long result;
			if (dir == 0 || 2 == dir || 4 == dir || 6 == dir)
			{
				result = (long)((int)((float)num / 1.414213f));
			}
			else
			{
				result = (long)num;
			}
			return result;
		}

		private static int CalcDirection(int x1, int y1, int x2, int y2)
		{
			int result;
			if (x1 == x2)
			{
				if (y2 > y1)
				{
					result = 0;
				}
				else
				{
					result = 4;
				}
			}
			else if (y1 == y2)
			{
				if (x2 > x1)
				{
					result = 2;
				}
				else
				{
					result = 6;
				}
			}
			else if (x1 + 1 == x2 && y1 - 1 == y2)
			{
				result = 3;
			}
			else if (x1 + 1 == x2 && y1 + 1 == y2)
			{
				result = 1;
			}
			else if (x1 - 1 == x2 && y1 + 1 == y2)
			{
				result = 7;
			}
			else if (x1 - 1 == x2 && y1 - 1 == y2)
			{
				result = 5;
			}
			else
			{
				result = 0;
			}
			return result;
		}

		private bool StepMove(double toMoveDist, GameClient client)
		{
			StoryBoard4Client storyBoard4Client = StoryBoard4Client.FindStoryBoard(this._RoleID);
			bool result;
			if (null == storyBoard4Client)
			{
				result = false;
			}
			else
			{
				lock (this.mutex)
				{
					this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
					if (!this.DetectNextGrid())
					{
						result = true;
					}
					else
					{
						double num = this._Path[this._PathIndex].X * (double)this._CellSizeX + (double)this._CellSizeX / 2.0;
						double num2 = this._Path[this._PathIndex].Y * (double)this._CellSizeY + (double)this._CellSizeY / 2.0;
						int num3 = (int)StoryBoard4Client.GetDirectionByTan(num, num2, (double)this._LastTargetX, (double)this._LastTargetY);
						double num4 = num - (double)this._LastTargetX;
						double num5 = num2 - (double)this._LastTargetY;
						double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
						bool flag2 = false;
						if (this._Path.Count <= 1)
						{
							flag2 = true;
						}
						if (null != client)
						{
							GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
							if (gameMap.InSafeRegionList(this._Path[this._PathIndex]))
							{
								flag2 = true;
							}
						}
						int currentAction = flag2 ? 1 : 2;
						if (flag2)
						{
							toMoveDist *= 0.8;
						}
						double num7 = (num6 < toMoveDist) ? num6 : toMoveDist;
						double num8 = Math.Atan2(num5, num4);
						double num9 = num7 * Math.Cos(num8);
						double num10 = num7 * Math.Sin(num8);
						this._CurrentX += num9;
						this._CurrentY += num10;
						if (null != client)
						{
							client.ClientData.CurrentAction = currentAction;
							if (num3 != client.ClientData.RoleDirection)
							{
								client.ClientData.RoleDirection = num3;
							}
						}
						if (num6 >= toMoveDist)
						{
							if (null != client)
							{
								GameMap gameMap = GameManager.MapMgr.DictMaps[client.ClientData.MapCode];
								int num11 = client.ClientData.PosX / gameMap.MapGridWidth;
								int num12 = client.ClientData.PosY / gameMap.MapGridHeight;
								client.ClientData.PosX = (int)this._CurrentX;
								client.ClientData.PosY = (int)this._CurrentY;
								int num13 = client.ClientData.PosX / gameMap.MapGridWidth;
								int num14 = client.ClientData.PosY / gameMap.MapGridHeight;
								if (num11 != num13 || num12 != num14)
								{
									MapGrid mapGrid = GameManager.MapGridMgr.DictGrids[client.ClientData.MapCode];
									mapGrid.MoveObjectEx(num11, num12, num13, num14, client);
								}
							}
							this._LastTargetX = (int)this._CurrentX;
							this._LastTargetY = (int)this._CurrentY;
						}
						else
						{
							this._PathIndex++;
							if (this._PathIndex >= this._Path.Count)
							{
								if (null != client)
								{
									client.ClientData.PosX = (int)num;
									client.ClientData.PosY = (int)num2;
								}
								return true;
							}
							this._LastTargetX = (int)num;
							this._LastTargetY = (int)num2;
							toMoveDist -= num6;
							this.StepMove(toMoveDist, client);
						}
						result = false;
					}
				}
			}
			return result;
		}

		private static double GetDirectionByTan(double targetX, double targetY, double currentX, double currentY)
		{
			int num = 0;
			if (targetX < currentX)
			{
				if (targetY < currentY)
				{
					num = 5;
				}
				else if (targetY == currentY)
				{
					num = 6;
				}
				else if (targetY > currentY)
				{
					num = 7;
				}
			}
			else if (targetX == currentX)
			{
				if (targetY < currentY)
				{
					num = 4;
				}
				else if (targetY > currentY)
				{
					num = 0;
				}
			}
			else if (targetX > currentX)
			{
				if (targetY < currentY)
				{
					num = 3;
				}
				else if (targetY == currentY)
				{
					num = 2;
				}
				else if (targetY > currentY)
				{
					num = 1;
				}
			}
			return (double)num;
		}

		private bool DetectNextGrid()
		{
			bool result;
			if (this._PathIndex <= this._LastStopIndex)
			{
				result = true;
			}
			else if (this.CanMoveToNext())
			{
				result = true;
			}
			else
			{
				this._LastStopIndex = this._PathIndex;
				this._Path.RemoveRange(this._PathIndex, this._Path.Count - this._PathIndex);
				if (this._Path.Count <= 0)
				{
					this._LastPoint = this._FirstPoint;
				}
				else
				{
					this._LastPoint = new Point(this._Path[this._Path.Count - 1].X * (double)this._CellSizeX + (double)(this._CellSizeX / 2), this._Path[this._Path.Count - 1].Y * (double)this._CellSizeY + (double)(this._CellSizeY / 2));
				}
				this._Stopped = true;
				result = false;
			}
			return result;
		}

		private bool CanMoveToNext()
		{
			GameClient gameClient = GameManager.ClientMgr.FindClient(this._RoleID);
			bool result;
			if (null == gameClient)
			{
				result = false;
			}
			else
			{
				this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
				GameMap gameMap = GameManager.MapMgr.DictMaps[gameClient.ClientData.MapCode];
				int num = gameClient.ClientData.PosX / gameMap.MapGridWidth;
				int num2 = gameClient.ClientData.PosY / gameMap.MapGridHeight;
				result = ((num == (int)this._Path[this._PathIndex].X && num2 == (int)this._Path[this._PathIndex].Y) || !Global.InObsByGridXY(ObjectTypes.OT_CLIENT, gameClient.ClientData.MapCode, (int)this._Path[this._PathIndex].X, (int)this._Path[this._PathIndex].Y, 0, 0));
			}
			return result;
		}

		private const float DiagCost = 1.414213f;

		private static Dictionary<int, StoryBoard4Client> StoryBoardDict = new Dictionary<int, StoryBoard4Client>();

		private static long LastRunStoryTicks = 0L;

		private int _RoleID = -1;

		private object mutex = new object();

		private int _PathIndex = 0;

		private int _LastTargetX = 0;

		private int _LastTargetY = 0;

		private double _CurrentX = 0.0;

		private double _CurrentY = 0.0;

		private int _CellSizeX = GameManager.MapGridWidth;

		private int _CellSizeY = GameManager.MapGridHeight;

		private List<Point> _Path = null;

		private long _LastRunTicks = 0L;

		private bool _Started = false;

		private bool _CompletedState = false;

		private bool _Stopped = false;

		private int _LastStopIndex = -1;

		private Point _FirstPoint = new Point(0.0, 0.0);

		private Point _LastPoint = new Point(0.0, 0.0);

		private double _MovingSpeedPerSec = 500.0;

		public delegate void CompletedDelegateHandle(object sender, EventArgs e);
	}
}
