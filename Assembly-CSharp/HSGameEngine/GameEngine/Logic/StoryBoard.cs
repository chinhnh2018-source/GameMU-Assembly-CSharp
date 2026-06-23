using System;
using System.Collections.Generic;
using System.Linq;
using HSGameEngine.GameEngine.Interface;
using HSGameEngine.GameEngine.Sprite;
using Server.Tools.AStarEx;
using UnityEngine;

namespace HSGameEngine.GameEngine.Logic
{
	public class StoryBoard
	{
		public StoryBoard(string name)
		{
			this._Name = name;
		}

		public event StoryBoardCompleteEventHandler Completed;

		public static bool ContainStoryBoard(string name)
		{
			return StoryBoard.storyBoardDict.ContainsKey(name);
		}

		public static StoryBoard FindStoryBoard(string name)
		{
			StoryBoard result = null;
			StoryBoard.storyBoardDict.TryGetValue(name, ref result);
			return result;
		}

		public static void RemoveStoryBoard(string name)
		{
			StoryBoard storyBoard = null;
			StoryBoard.storyBoardDict.TryGetValue(name, ref storyBoard);
			if (storyBoard != null)
			{
				storyBoard.Completed = null;
				storyBoard.Clear();
			}
		}

		public static int StopStoryBoard(string name, int stopIndex = -1)
		{
			StoryBoard storyBoard = null;
			StoryBoard.storyBoardDict.TryGetValue(name, ref storyBoard);
			if (storyBoard != null)
			{
				return storyBoard.StopOnNextGrid(stopIndex);
			}
			return -1;
		}

		public static void ClearStoryBoard()
		{
			List<StoryBoard> list = Enumerable.ToList<StoryBoard>(StoryBoard.storyBoardDict.Values);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				StoryBoard storyBoard = list[i];
				if (storyBoard != null)
				{
					storyBoard.Completed = null;
					storyBoard.Clear();
				}
				i++;
			}
			StoryBoard.storyBoardDict.Clear();
		}

		public static void runStoryBoards(bool shiftIsPressed)
		{
			try
			{
				long currentTicks = TmskTime.CurMills();
				List<StoryBoard> list = Enumerable.ToList<StoryBoard>(StoryBoard.storyBoardDict.Values);
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					StoryBoard storyBoard = list[i];
					if (storyBoard != null)
					{
						storyBoard.Run(currentTicks, shiftIsPressed);
					}
					i++;
				}
			}
			catch (Exception ex)
			{
				MUDebug.LogException(ex);
			}
		}

		public object Tag
		{
			get
			{
				return this._Tag;
			}
			set
			{
				this._Tag = value;
			}
		}

		public string Name
		{
			get
			{
				return this._Name;
			}
		}

		public void Binding()
		{
			StoryBoard.storyBoardDict.Add(this._Name, this);
		}

		public void Clear()
		{
			if (this._Name != null)
			{
				StoryBoard.storyBoardDict.Remove(this._Name);
			}
		}

		public double MovingSpeedPerSec
		{
			get
			{
				return this._MovingSpeedPerSec;
			}
			set
			{
				this._MovingSpeedPerSec = value;
			}
		}

		public bool Start(IObject obj, List<ANode> path, int cellSizeX, int cellSizeY, int destX, int destY, long elapsedTicks = 0L, bool noAction = false)
		{
			if (this._Started)
			{
				return false;
			}
			if (path.Count <= 0)
			{
				return false;
			}
			this._MovingObj = obj;
			this._CellSizeX = cellSizeX;
			this._CellSizeY = cellSizeY;
			this._PathIndex = 0;
			this._LastRunTicks = TmskTime.CurMills() - elapsedTicks;
			this._LastTargetX = (double)this._MovingObj.cx;
			this._LastTargetY = (double)this._MovingObj.cy;
			this._CurrentX = (double)this._MovingObj.cx;
			this._CurrentY = (double)this._MovingObj.cy;
			this._Path = path;
			this._CompletedState = false;
			this._Started = true;
			this._Stopped = false;
			this._DestX = destX;
			this._DestY = destY;
			this._NoAction = noAction;
			return true;
		}

		public int GetCurPathIndex()
		{
			return this._PathIndex;
		}

		public int StopOnNextGrid(int stopIndex = -1)
		{
			if (this._CompletedState)
			{
				return -1;
			}
			if (this._Path == null)
			{
				return -1;
			}
			if (stopIndex >= 0)
			{
				if (stopIndex < this._Path.Count)
				{
					this._Path.RemoveAt(stopIndex);
				}
				return stopIndex;
			}
			if (this._PathIndex >= this._Path.Count - 1)
			{
				this._Stopped = true;
				return this._Path.Count - 1;
			}
			this._Path.RemoveRange(this._PathIndex, this._Path.Count - this._PathIndex);
			this._Stopped = true;
			return this._PathIndex;
		}

		public List<ANode> Path
		{
			get
			{
				return this._Path;
			}
		}

		public bool IsStopped()
		{
			return this._Stopped;
		}

		public bool NoAction
		{
			get
			{
				return this._NoAction;
			}
		}

		public void Run(long currentTicks, bool shiftIsPressed)
		{
			if (!this._Started)
			{
				return;
			}
			if (this._CompletedState)
			{
				return;
			}
			if (currentTicks < this._LastRunTicks)
			{
				return;
			}
			long num = currentTicks - this._LastRunTicks;
			this._LastRunTicks = currentTicks;
			double num2 = (double)num / 1000.0;
			double toMoveDist = num2 * this._MovingSpeedPerSec * this._MovingObj.MoveSpeed;
			if (this.StepMove(toMoveDist))
			{
				this._CompletedState = true;
				if (this.Completed != null)
				{
					this.Completed(this, null);
				}
			}
		}

		private int GetNeedTicks(bool needWalking, int dir)
		{
			if (this._MovingObj.SpriteType == GSpriteTypes.Monster)
			{
				needWalking = true;
			}
			int num = (!needWalking) ? 125 : 225;
			if (dir == 0 || dir == 2 || dir == 4 || dir == 6)
			{
				return (int)((float)num / 1.414213f);
			}
			return num;
		}

		private int CalcDirection(double x1, double y1, double x2, double y2)
		{
			if (x1 == x2)
			{
				if (y2 > y1)
				{
					return 0;
				}
				return 4;
			}
			else if (y1 == y2)
			{
				if (x2 > x1)
				{
					return 2;
				}
				return 6;
			}
			else
			{
				if (x1 + 1.0 == x2 && y1 - 1.0 == y2)
				{
					return 3;
				}
				if (x1 + 1.0 == x2 && y1 + 1.0 == y2)
				{
					return 1;
				}
				if (x1 - 1.0 == x2 && y1 + 1.0 == y2)
				{
					return 7;
				}
				if (x1 - 1.0 == x2 && y1 - 1.0 == y2)
				{
					return 5;
				}
				return 0;
			}
		}

		private bool StepMove(double toMoveDist)
		{
			if (this._Stopped)
			{
				return true;
			}
			this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
			if (this._PathIndex < 0)
			{
				MUDebug.LogError<string>(new string[]
				{
					string.Concat(new object[]
					{
						"StepMove, _Path.Count=",
						this._Path.Count,
						", _PathIndex=",
						this._PathIndex
					})
				});
				return true;
			}
			int num = this._Path[this._PathIndex].x * this._CellSizeX + this._CellSizeX / 2;
			int num2 = this._Path[this._PathIndex].y * this._CellSizeY + this._CellSizeY / 2;
			int num3 = (int)this.GetDirectionByTan((double)num, (double)num2, this._LastTargetX, this._LastTargetY);
			int num4 = num - (int)this._LastTargetX;
			int num5 = num2 - (int)this._LastTargetY;
			double num6 = Math.Sqrt((double)(num4 * num4 + num5 * num5));
			bool flag = false;
			if (this._Path.Count <= 1)
			{
				flag = true;
			}
			if (this._NoAction)
			{
				flag = true;
			}
			if (this._MovingObj.SpriteType == GSpriteTypes.Leader || this._MovingObj.SpriteType == GSpriteTypes.Other)
			{
				if (flag || Global.IsInSafeRegion)
				{
					flag = true;
					if (!this._NoAction)
					{
						this._MovingObj.Action = GActions.Walk;
					}
				}
				else if (!this._NoAction)
				{
					this._MovingObj.Action = GActions.Run;
				}
			}
			if (flag)
			{
				if (!this._NoAction)
				{
					toMoveDist *= 0.8;
				}
				else
				{
					toMoveDist *= this.SimpleMoveSpeedRate;
				}
			}
			double num7 = (num6 >= toMoveDist) ? toMoveDist : num6;
			double num8 = Math.Atan2((double)num5, (double)num4);
			double num9 = num7 * Math.Cos(num8);
			double num10 = num7 * Math.Sin(num8);
			this._CurrentX += num9;
			this._CurrentY += num10;
			int direction = (this._MovingObj as GSprite).GetDirection();
			if (this._MovingObj.SpriteType == GSpriteTypes.Monster && this._MovingObj.MonsterType == MonsterTypes.JUSTMOVE)
			{
				if (num3 != direction && (Mathf.Abs(num4) >= this._CellSizeX / 2 || Mathf.Abs(num5) >= this._CellSizeX / 2) && !this._NoAction)
				{
					this._MovingObj.Direction = num3;
				}
			}
			else
			{
				if (num3 != direction && !this._NoAction)
				{
					this._MovingObj.Direction = num3;
				}
				if ((Mathf.Abs(num4) >= this._CellSizeX / 2 || Mathf.Abs(num5) >= this._CellSizeX / 2) && !this._NoAction)
				{
					this._MovingObj.Rotation = Quaternion.LookRotation(new Vector3((float)num4 / 100f, 0f, (float)num5 / 100f), Vector3.up);
				}
			}
			if (num6 >= toMoveDist)
			{
				this._MovingObj.cx = (int)this._CurrentX;
				this._MovingObj.cy = (int)this._CurrentY;
				this._LastTargetX = this._CurrentX;
				this._LastTargetY = this._CurrentY;
			}
			else
			{
				this._PathIndex++;
				if (this._PathIndex >= this._Path.Count)
				{
					this._MovingObj.cx = num;
					this._MovingObj.cy = num2;
					return true;
				}
				this._LastTargetX = (double)num;
				this._LastTargetY = (double)num2;
				toMoveDist -= num6;
				this.StepMove(toMoveDist);
			}
			return false;
		}

		private double GetDirectionByTan(double targetX, double targetY, double currentX, double currentY)
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

		private const float DiagCost = 1.414213f;

		private static Dictionary<string, StoryBoard> storyBoardDict = new Dictionary<string, StoryBoard>();

		private object _Tag;

		private string _Name;

		private int _PathIndex;

		private double _LastTargetX;

		private double _LastTargetY;

		private double _CurrentX;

		private double _CurrentY;

		private int _CellSizeX = 100;

		private int _CellSizeY = 100;

		private List<ANode> _Path;

		private long _LastRunTicks;

		private double _MovingSpeedPerSec = 500.0;

		private IObject _MovingObj;

		private bool _Started;

		private bool _CompletedState;

		private bool _Stopped;

		private int _DestX = -1;

		private int _DestY = -1;

		private bool _NoAction;

		public double SimpleMoveSpeedRate = 0.25;
	}
}
