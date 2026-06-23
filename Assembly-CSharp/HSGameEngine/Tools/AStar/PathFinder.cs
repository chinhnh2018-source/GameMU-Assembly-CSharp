using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	public class PathFinder : IPathFinder
	{
		public PathFinder(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
		}

		public event PathFinderDebugHandler PathFinderDebug;

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
			PathFinderNode pathFinderNode;
			pathFinderNode.G = 0;
			pathFinderNode.H = this.mHEstimate;
			pathFinderNode.F = pathFinderNode.G + pathFinderNode.H;
			pathFinderNode.X = start.X;
			pathFinderNode.Y = start.Y;
			pathFinderNode.PX = pathFinderNode.X;
			pathFinderNode.PY = pathFinderNode.Y;
			this.mOpen.Push(pathFinderNode);
			while (this.mOpen.Count > 0 && !this.mStop)
			{
				pathFinderNode = this.mOpen.Pop();
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, pathFinderNode.X, pathFinderNode.Y, PathFinderNodeType.Current, -1, -1);
				}
				if (pathFinderNode.X == end.X && pathFinderNode.Y == end.Y)
				{
					this.mClose.Add(pathFinderNode);
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
					this.mHoriz = pathFinderNode.X - pathFinderNode.PX;
				}
				for (int i = 0; i < ((!this.mDiagonals) ? 4 : 8); i++)
				{
					PathFinderNode item;
					item.X = pathFinderNode.X + (int)array[i, 0];
					item.Y = pathFinderNode.Y + (int)array[i, 1];
					if (item.X >= 0 && item.Y >= 0 && item.X < upperBound && item.Y < upperBound2)
					{
						int num;
						if (this.mHeavyDiagonals && i > 3)
						{
							num = pathFinderNode.G + (int)((double)this.mGrid[item.X, item.Y] * 2.41);
						}
						else
						{
							num = pathFinderNode.G + (int)this.mGrid[item.X, item.Y];
						}
						if (num != pathFinderNode.G)
						{
							if (this.mPunishChangeDirection)
							{
								if (item.X - pathFinderNode.X != 0 && this.mHoriz == 0)
								{
									num += 20;
								}
								if (item.Y - pathFinderNode.Y != 0 && this.mHoriz != 0)
								{
									num += 20;
								}
							}
							int num2 = -1;
							for (int j = 0; j < this.mOpen.Count; j++)
							{
								if (this.mOpen[j].X == item.X && this.mOpen[j].Y == item.Y)
								{
									num2 = j;
									break;
								}
							}
							if (num2 == -1 || this.mOpen[num2].G > num)
							{
								int num3 = -1;
								for (int k = 0; k < this.mClose.Count; k++)
								{
									if (this.mClose[k].X == item.X && this.mClose[k].Y == item.Y)
									{
										num3 = k;
										break;
									}
								}
								if (num3 == -1 || (!this.mReopenCloseNodes && this.mClose[num3].G > num))
								{
									item.PX = pathFinderNode.X;
									item.PY = pathFinderNode.Y;
									item.G = num;
									switch (this.mFormula)
									{
									default:
										item.H = this.mHEstimate * (Math.Abs(item.X - end.X) + Math.Abs(item.Y - end.Y));
										break;
									case HeuristicFormula.MaxDXDY:
										item.H = this.mHEstimate * Math.Max(Math.Abs(item.X - end.X), Math.Abs(item.Y - end.Y));
										break;
									case HeuristicFormula.DiagonalShortCut:
									{
										int num4 = Math.Min(Math.Abs(item.X - end.X), Math.Abs(item.Y - end.Y));
										int num5 = Math.Abs(item.X - end.X) + Math.Abs(item.Y - end.Y);
										item.H = this.mHEstimate * 2 * num4 + this.mHEstimate * (num5 - 2 * num4);
										break;
									}
									case HeuristicFormula.Euclidean:
										item.H = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)(item.X - end.X), 2.0) + Math.Pow((double)(item.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.EuclideanNoSQR:
										item.H = (int)((double)this.mHEstimate * (Math.Pow((double)(item.X - end.X), 2.0) + Math.Pow((double)(item.Y - end.Y), 2.0)));
										break;
									case HeuristicFormula.Custom1:
									{
										Point2D point2D = new Point2D(Math.Abs(end.X - item.X), Math.Abs(end.Y - item.Y));
										int num6 = Math.Abs(point2D.X - point2D.Y);
										int num7 = Math.Abs((point2D.X + point2D.Y - num6) / 2);
										item.H = this.mHEstimate * (num7 + num6 + point2D.X + point2D.Y);
										break;
									}
									}
									if (this.mTieBreaker)
									{
										int num8 = pathFinderNode.X - end.X;
										int num9 = pathFinderNode.Y - end.Y;
										int num10 = start.X - end.X;
										int num11 = start.Y - end.Y;
										int num12 = Math.Abs(num8 * num11 - num10 * num9);
										item.H = (int)((double)item.H + (double)num12 * 0.001);
									}
									item.F = item.G + item.H;
									if (this.mDebugProgress && this.PathFinderDebug != null)
									{
										this.PathFinderDebug(pathFinderNode.X, pathFinderNode.Y, item.X, item.Y, PathFinderNodeType.Open, item.F, item.G);
									}
									this.mOpen.Push(item);
								}
							}
						}
					}
				}
				this.mClose.Add(pathFinderNode);
				if (this.mDebugProgress && this.PathFinderDebug != null)
				{
					this.PathFinderDebug(0, 0, pathFinderNode.X, pathFinderNode.Y, PathFinderNodeType.Close, pathFinderNode.F, pathFinderNode.G);
				}
			}
			if (flag)
			{
				PathFinderNode pathFinderNode2 = this.mClose[this.mClose.Count - 1];
				for (int l = this.mClose.Count - 1; l >= 0; l--)
				{
					if ((pathFinderNode2.PX == this.mClose[l].X && pathFinderNode2.PY == this.mClose[l].Y) || l == this.mClose.Count - 1)
					{
						if (this.mDebugFoundPath && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(pathFinderNode2.X, pathFinderNode2.Y, this.mClose[l].X, this.mClose[l].Y, PathFinderNodeType.Path, this.mClose[l].F, this.mClose[l].G);
						}
						pathFinderNode2 = this.mClose[l];
					}
					else
					{
						this.mClose.RemoveAt(l);
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
			if (_floydPath == null)
			{
				return null;
			}
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
			return _floydPath;
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

		private byte[,] mGrid;

		private PriorityQueueB<PathFinderNode> mOpen = new PriorityQueueB<PathFinderNode>(new PathFinder.ComparePFNode());

		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		private bool mStop;

		private bool mStopped = true;

		private int mHoriz;

		private HeuristicFormula mFormula = HeuristicFormula.Manhattan;

		private bool mDiagonals = true;

		private int mHEstimate = 2;

		private bool mPunishChangeDirection;

		private bool mReopenCloseNodes;

		private bool mTieBreaker;

		private bool mHeavyDiagonals;

		private int mSearchLimit = 2000;

		private double mCompletedTime;

		private bool mDebugProgress;

		private bool mDebugFoundPath;

		internal class ComparePFNode : IComparer<PathFinderNode>
		{
			public int Compare(PathFinderNode x, PathFinderNode y)
			{
				if (x.F > y.F)
				{
					return 1;
				}
				if (x.F < y.F)
				{
					return -1;
				}
				return 0;
			}
		}
	}
}
