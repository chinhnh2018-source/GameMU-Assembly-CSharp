using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	public class PathFinder : IPathFinder
	{
		public event PathFinderDebugHandler PathFinderDebug;

		public PathFinder(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
		}

		public bool Stopped
		{
			get
			{
				return this.mStopped;
			}
		}

		public HeuristicFormula Formula
		{
			get
			{
				return this.mFormula;
			}
			set
			{
				this.mFormula = value;
			}
		}

		public bool Diagonals
		{
			get
			{
				return this.mDiagonals;
			}
			set
			{
				this.mDiagonals = value;
			}
		}

		public bool HeavyDiagonals
		{
			get
			{
				return this.mHeavyDiagonals;
			}
			set
			{
				this.mHeavyDiagonals = value;
			}
		}

		public int HeuristicEstimate
		{
			get
			{
				return this.mHEstimate;
			}
			set
			{
				this.mHEstimate = value;
			}
		}

		public bool PunishChangeDirection
		{
			get
			{
				return this.mPunishChangeDirection;
			}
			set
			{
				this.mPunishChangeDirection = value;
			}
		}

		public bool ReopenCloseNodes
		{
			get
			{
				return this.mReopenCloseNodes;
			}
			set
			{
				this.mReopenCloseNodes = value;
			}
		}

		public bool TieBreaker
		{
			get
			{
				return this.mTieBreaker;
			}
			set
			{
				this.mTieBreaker = value;
			}
		}

		public int SearchLimit
		{
			get
			{
				return this.mSearchLimit;
			}
			set
			{
				this.mSearchLimit = value;
			}
		}

		public double CompletedTime
		{
			get
			{
				return this.mCompletedTime;
			}
			set
			{
				this.mCompletedTime = value;
			}
		}

		public bool DebugProgress
		{
			get
			{
				return this.mDebugProgress;
			}
			set
			{
				this.mDebugProgress = value;
			}
		}

		public bool DebugFoundPath
		{
			get
			{
				return this.mDebugFoundPath;
			}
			set
			{
				this.mDebugFoundPath = value;
			}
		}

		public void FindPathStop()
		{
			this.mStop = true;
		}

		public List<PathFinderNode> FindPath(Point2D start, Point2D end)
		{
			bool flag = false;
			int upperBound = this.mGrid.GetUpperBound(0);
			int upperBound2 = this.mGrid.GetUpperBound(1);
			this.mStop = false;
			this.mStopped = false;
			this.mOpen.Clear();
			this.mClose.Clear();
			if (this.mDebugProgress && this.PathFinderDebug != null)
			{
				this.PathFinderDebug(0, 0, start.X, start.Y, PathFinderNodeType.Start, -1, -1);
			}
			if (this.mDebugProgress && this.PathFinderDebug != null)
			{
				this.PathFinderDebug(0, 0, end.X, end.Y, PathFinderNodeType.End, -1, -1);
			}
			sbyte[,] array;
			if (this.mDiagonals)
			{
				array = new sbyte[,]
				{
					{
						0,
						-1
					},
					{
						1,
						0
					},
					{
						0,
						1
					},
					{
						-1,
						0
					},
					{
						1,
						-1
					},
					{
						1,
						1
					},
					{
						-1,
						1
					},
					{
						-1,
						-1
					}
				};
			}
			else
			{
				array = new sbyte[,]
				{
					{
						0,
						-1
					},
					{
						1,
						0
					},
					{
						0,
						1
					},
					{
						-1,
						0
					}
				};
			}
			PathFinderNode item;
			item.G = 0;
			item.H = this.mHEstimate;
			item.F = item.G + item.H;
			item.X = start.X;
			item.Y = start.Y;
			item.PX = item.X;
			item.PY = item.Y;
			this.mOpen.Push(item);
			while (this.mOpen.Count > 0 && !this.mStop)
			{
				item = this.mOpen.Pop();
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, item.X, item.Y, PathFinderNodeType.Current, -1, -1);
				}
				if (item.X == end.X && item.Y == end.Y)
				{
					this.mClose.Add(item);
					flag = true;
					break;
				}
				if (this.mClose.Count > this.mSearchLimit)
				{
					this.mStopped = true;
					return null;
				}
				if (this.mPunishChangeDirection)
				{
					this.mHoriz = item.X - item.PX;
				}
				for (int i = 0; i < (this.mDiagonals ? 8 : 4); i++)
				{
					PathFinderNode item2;
					item2.X = item.X + (int)array[i, 0];
					item2.Y = item.Y + (int)array[i, 1];
					if (item2.X >= 0 && item2.Y >= 0 && item2.X < upperBound && item2.Y < upperBound2)
					{
						int num;
						if (this.mHeavyDiagonals && i > 3)
						{
							num = item.G + (int)((double)this.mGrid[item2.X, item2.Y] * 2.41);
						}
						else
						{
							num = item.G + (int)this.mGrid[item2.X, item2.Y];
						}
						if (num != item.G)
						{
							if (this.mPunishChangeDirection)
							{
								if (item2.X - item.X != 0)
								{
									if (this.mHoriz == 0)
									{
										num += 20;
									}
								}
								if (item2.Y - item.Y != 0)
								{
									if (this.mHoriz != 0)
									{
										num += 20;
									}
								}
							}
							int num2 = -1;
							for (int j = 0; j < this.mOpen.Count; j++)
							{
								if (this.mOpen[j].X == item2.X && this.mOpen[j].Y == item2.Y)
								{
									num2 = j;
									break;
								}
							}
							if (num2 == -1 || this.mOpen[num2].G > num)
							{
								int num3 = -1;
								for (int j = 0; j < this.mClose.Count; j++)
								{
									if (this.mClose[j].X == item2.X && this.mClose[j].Y == item2.Y)
									{
										num3 = j;
										break;
									}
								}
								if (num3 == -1 || (!this.mReopenCloseNodes && this.mClose[num3].G > num))
								{
									item2.PX = item.X;
									item2.PY = item.Y;
									item2.G = num;
									switch (this.mFormula)
									{
									default:
										item2.H = this.mHEstimate * (Math.Abs(item2.X - end.X) + Math.Abs(item2.Y - end.Y));
										break;
									case HeuristicFormula.MaxDXDY:
										item2.H = this.mHEstimate * Math.Max(Math.Abs(item2.X - end.X), Math.Abs(item2.Y - end.Y));
										break;
									case HeuristicFormula.DiagonalShortCut:
									{
										int num4 = Math.Min(Math.Abs(item2.X - end.X), Math.Abs(item2.Y - end.Y));
										int num5 = Math.Abs(item2.X - end.X) + Math.Abs(item2.Y - end.Y);
										item2.H = this.mHEstimate * 2 * num4 + this.mHEstimate * (num5 - 2 * num4);
										break;
									}
									case HeuristicFormula.Euclidean:
										item2.H = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)(item2.X - end.X), 2.0) + Math.Pow((double)(item2.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.EuclideanNoSQR:
										item2.H = (int)((double)this.mHEstimate * (Math.Pow((double)(item2.X - end.X), 2.0) + Math.Pow((double)(item2.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.Custom1:
									{
										Point2D point2D = new Point2D(Math.Abs(end.X - item2.X), Math.Abs(end.Y - item2.Y));
										int num6 = Math.Abs(point2D.X - point2D.Y);
										int num7 = Math.Abs((point2D.X + point2D.Y - num6) / 2);
										item2.H = this.mHEstimate * (num7 + num6 + point2D.X + point2D.Y);
										break;
									}
									}
									if (this.mTieBreaker)
									{
										int num8 = item.X - end.X;
										int num9 = item.Y - end.Y;
										int num10 = start.X - end.X;
										int num11 = start.Y - end.Y;
										int num12 = Math.Abs(num8 * num11 - num10 * num9);
										item2.H = (int)((double)item2.H + (double)num12 * 0.001);
									}
									item2.F = item2.G + item2.H;
									if (this.mDebugProgress && this.PathFinderDebug != null)
									{
										this.PathFinderDebug(item.X, item.Y, item2.X, item2.Y, PathFinderNodeType.Open, item2.F, item2.G);
									}
									this.mOpen.Push(item2);
								}
							}
						}
					}
				}
				this.mClose.Add(item);
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, item.X, item.Y, PathFinderNodeType.Close, item.F, item.G);
				}
			}
			if (flag)
			{
				PathFinderNode pathFinderNode = this.mClose[this.mClose.Count - 1];
				for (int i = this.mClose.Count - 1; i >= 0; i--)
				{
					if ((pathFinderNode.PX == this.mClose[i].X && pathFinderNode.PY == this.mClose[i].Y) || i == this.mClose.Count - 1)
					{
						if (this.mDebugFoundPath && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(pathFinderNode.X, pathFinderNode.Y, this.mClose[i].X, this.mClose[i].Y, PathFinderNodeType.Path, this.mClose[i].F, this.mClose[i].G);
						}
						pathFinderNode = this.mClose[i];
					}
					else
					{
						this.mClose.RemoveAt(i);
					}
				}
				this.mStopped = true;
				return this.mClose;
			}
			this.mStopped = true;
			return null;
		}

		public List<PathFinderNode> Floyd(List<PathFinderNode> _floydPath)
		{
			List<PathFinderNode> result;
			if (null == _floydPath)
			{
				result = null;
			}
			else
			{
				_floydPath = this.ReverseList(_floydPath);
				int count = _floydPath.Count;
				if (count > 2)
				{
					PathFinderNode target = default(PathFinderNode);
					PathFinderNode target2 = default(PathFinderNode);
					this.FloydVector(target, _floydPath[count - 1], _floydPath[count - 2]);
					for (int i = _floydPath.Count - 3; i >= 0; i--)
					{
						this.FloydVector(target2, _floydPath[i + 1], _floydPath[i]);
						if (target.PX == target2.PX && target.PY == target2.PY)
						{
							_floydPath.RemoveAt(i + 1);
						}
						else
						{
							target.PX = target2.PX;
							target.PY = target2.PY;
						}
					}
				}
				_floydPath = this.ReverseList(_floydPath);
				result = _floydPath;
			}
			return result;
		}

		private List<PathFinderNode> ReverseList(List<PathFinderNode> floydPath)
		{
			List<PathFinderNode> list = new List<PathFinderNode>();
			for (int i = floydPath.Count - 1; i >= 0; i--)
			{
				list.Add(floydPath[i]);
			}
			return list;
		}

		private void FloydVector(PathFinderNode target, PathFinderNode n1, PathFinderNode n2)
		{
			target.PX = n1.PX - n2.PX;
			target.PY = n1.PY - n2.PY;
		}

		private byte[,] mGrid = null;

		private PriorityQueueB<PathFinderNode> mOpen = new PriorityQueueB<PathFinderNode>(new PathFinder.ComparePFNode());

		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		private bool mStop = false;

		private bool mStopped = true;

		private int mHoriz = 0;

		private HeuristicFormula mFormula = HeuristicFormula.Manhattan;

		private bool mDiagonals = true;

		private int mHEstimate = 2;

		private bool mPunishChangeDirection = false;

		private bool mReopenCloseNodes = false;

		private bool mTieBreaker = false;

		private bool mHeavyDiagonals = false;

		private int mSearchLimit = 2000;

		private double mCompletedTime = 0.0;

		private bool mDebugProgress = false;

		private bool mDebugFoundPath = false;

		internal class ComparePFNode : IComparer<PathFinderNode>
		{
			public int Compare(PathFinderNode x, PathFinderNode y)
			{
				int result;
				if (x.F > y.F)
				{
					result = 1;
				}
				else if (x.F < y.F)
				{
					result = -1;
				}
				else
				{
					result = 0;
				}
				return result;
			}
		}
	}
}
