using System;
using System.Collections.Generic;
using System.Windows;
using GameServer.Core.Executor;
using HSGameEngine.Tools.AStarEx;

namespace GameServer.Logic
{
	public class StoryBoardEx
	{
		public static bool ContainStoryBoard(string name)
		{
			return StoryBoardEx.StoryBoardDict.ContainsKey(name);
		}

		public static StoryBoardEx FindStoryBoard(string name)
		{
			StoryBoardEx result = null;
			StoryBoardEx.StoryBoardDict.TryGetValue(name, out result);
			return result;
		}

		public static void RemoveStoryBoard(string name)
		{
			StoryBoardEx storyBoardEx = StoryBoardEx.FindStoryBoard(name);
			if (null != storyBoardEx)
			{
				storyBoardEx.Completed = null;
				storyBoardEx.Clear();
			}
		}

		public static void ClearStoryBoard()
		{
			foreach (StoryBoardEx storyBoardEx in StoryBoardEx.StoryBoardDict.Values)
			{
				if (null != storyBoardEx)
				{
					storyBoardEx.Completed = null;
					storyBoardEx.Clear();
				}
			}
			StoryBoardEx.StoryBoardDict.Clear();
		}

		private static long getMyTimer()
		{
			return TimeUtil.NOW();
		}

		public static void runStoryBoards()
		{
			long myTimer = StoryBoardEx.getMyTimer();
			StoryBoardEx.LastRunStoryTicks = myTimer;
			List<StoryBoardEx> list = new List<StoryBoardEx>();
			foreach (StoryBoardEx storyBoardEx in StoryBoardEx.StoryBoardDict.Values)
			{
				StoryBoardEx storyBoardEx;
				list.Add(storyBoardEx);
			}
			for (int i = 0; i < list.Count; i++)
			{
				StoryBoardEx storyBoardEx = list[i];
				if (null != storyBoardEx)
				{
					storyBoardEx.Run(myTimer);
				}
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

		public event StoryBoardEx.CompletedDelegateHandle _Completed = null;

		public StoryBoardEx.CompletedDelegateHandle Completed
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

		public StoryBoardEx(string name)
		{
			this._Name = name;
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
			if (!StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Add(this._Name, this);
			}
		}

		public void Clear()
		{
			if (this._Name != null && StoryBoardEx.StoryBoardDict.ContainsKey(this._Name))
			{
				StoryBoardEx.StoryBoardDict.Remove(this._Name);
			}
		}

		public double OrigMovingSpeedPerFrame
		{
			get
			{
				return this._OrigMovingSpeedPerFrame;
			}
			set
			{
				this._OrigMovingSpeedPerFrame = value;
			}
		}

		public double MovingSpeedPerFrame
		{
			get
			{
				return this._MovingSpeedPerFrame;
			}
			set
			{
				this._MovingSpeedPerFrame = value;
			}
		}

		public bool Start(Monster obj, List<ANode> path, double movingSpeedPerFrame, int cellSize)
		{
			bool result;
			if (this._Started)
			{
				result = false;
			}
			else
			{
				this._OrigMovingSpeedPerFrame = movingSpeedPerFrame;
				this._MovingSpeedPerFrame = movingSpeedPerFrame;
				this._MovingObj = obj;
				this._LastRunTicks = StoryBoardEx.getMyTimer();
				this._CellSize = cellSize;
				this._PathIndex = 0;
				this._Path = path;
				this._CompletedState = false;
				this._Started = true;
				result = true;
			}
			return result;
		}

		public void Run(long currentTicks)
		{
			if (this._Started)
			{
				if (!this._CompletedState)
				{
					long num = currentTicks - this._LastRunTicks;
					this._LastRunTicks = currentTicks;
					double num2 = (double)(1000f / (float)Global.MovingFrameRate);
					double num3 = (double)num / num2;
					double toMoveDist = num3 * this._MovingSpeedPerFrame;
					if (this.StepMove(toMoveDist))
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

		private bool StepMove(double toMoveDist)
		{
			this._PathIndex = Math.Min(this._PathIndex, this._Path.Count - 1);
			double num = (double)(this._Path[this._PathIndex].x * this._CellSize) + (double)this._CellSize / 2.0;
			double num2 = (double)(this._Path[this._PathIndex].y * this._CellSize) + (double)this._CellSize / 2.0;
			double num3 = num - this._MovingObj.SafeCoordinate.X;
			double num4 = num2 - this._MovingObj.SafeCoordinate.Y;
			double num5 = Math.Sqrt(num3 * num3 + num4 * num4);
			double num6 = (num5 < toMoveDist) ? num5 : toMoveDist;
			double num7 = Math.Atan2(num4, num3);
			double num8 = num6 * Math.Cos(num7);
			double num9 = num6 * Math.Sin(num7);
			this._MovingObj.Coordinate = new Point(this._MovingObj.SafeCoordinate.X + num8, this._MovingObj.SafeCoordinate.Y + num9);
			if ((long)num != (long)this._MovingObj.SafeCoordinate.X || (long)num2 != (long)this._MovingObj.SafeCoordinate.Y)
			{
				int num10 = (int)Global.GetDirectionByTan(num, num2, this._MovingObj.SafeCoordinate.X, this._MovingObj.SafeCoordinate.Y);
				if ((double)num10 != this._MovingObj.Direction)
				{
					this._MovingObj.Direction = (double)num10;
				}
			}
			if (num5 < toMoveDist)
			{
				this._PathIndex++;
				if (this._PathIndex >= this._Path.Count)
				{
					this._MovingObj.Coordinate = new Point(num, num2);
					return true;
				}
				toMoveDist -= num5;
				this.StepMove(toMoveDist);
			}
			return false;
		}

		private static Dictionary<string, StoryBoardEx> StoryBoardDict = new Dictionary<string, StoryBoardEx>();

		private static long LastRunStoryTicks = 0L;

		private object _Tag = null;

		private string _Name = null;

		private int _PathIndex = 0;

		private int _CellSize = GameManager.MapGridWidth;

		private List<ANode> _Path = null;

		private long _LastRunTicks = 0L;

		private double _OrigMovingSpeedPerFrame = 0.0;

		private double _MovingSpeedPerFrame = 0.0;

		private Monster _MovingObj = null;

		private bool _Started = false;

		private bool _CompletedState = false;

		public delegate void CompletedDelegateHandle(object sender, EventArgs e);
	}
}
