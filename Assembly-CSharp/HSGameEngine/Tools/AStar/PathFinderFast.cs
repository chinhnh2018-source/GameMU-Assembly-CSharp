using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using HSGameEngine.Drawing;
using Server.Tools.AStarEx;

namespace HSGameEngine.Tools.AStar
{
	public class PathFinderFast : IPathFinder
	{
		public PathFinderFast(byte[,] grid)
		{
			if (grid == null)
			{
				throw new Exception("Grid cannot be null");
			}
			this.mGrid = grid;
			this.mGridX = (ushort)(this.mGrid.GetUpperBound(0) + 1);
			this.mGridY = (ushort)(this.mGrid.GetUpperBound(1) + 1);
			this.mGridXMinus1 = this.mGridX - 1;
			this.mGridYLog2 = (ushort)Math.Log((double)this.mGridY, 2.0);
			if (Math.Log((double)this.mGridX, 2.0) != (double)((int)Math.Log((double)this.mGridX, 2.0)) || Math.Log((double)this.mGridY, 2.0) != (double)((int)Math.Log((double)this.mGridY, 2.0)))
			{
				throw new Exception("Invalid Grid, size in X and Y must be power of 2");
			}
			if (PathFinderFast.mCalcGrid == null || PathFinderFast.mCalcGrid.Length < (int)(this.mGridX * this.mGridY))
			{
				PathFinderFast.mCalcGrid = new PathFinderFast.PathFinderNodeFast[(int)(this.mGridX * this.mGridY)];
			}
			this.mOpen = new PriorityQueueB<int>(new PathFinderFast.ComparePFNodeMatrix(PathFinderFast.mCalcGrid));
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
				if (this.mDiagonals)
				{
					this.mDirection = new sbyte[,]
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
					this.mDirection = new sbyte[,]
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

		public int[,] Punish
		{
			get
			{
				return this.mPunish;
			}
			set
			{
				this.mPunish = value;
			}
		}

		public int MaxNum
		{
			get
			{
				return this.mMaxNum;
			}
			set
			{
				this.mMaxNum = value;
			}
		}

		public bool EnablePunish
		{
			get
			{
				return this.mEnablePunish;
			}
			set
			{
				this.mEnablePunish = value;
			}
		}

		public void FindPathStop()
		{
			this.mStop = true;
		}

		public List<PathFinderNode> FindPath(Point start, Point end)
		{
			return this.FindPath(new Point2D(start.X, start.Y), new Point2D(end.X, end.Y));
		}

		private int GetPunishNum(int x, int y)
		{
			if (!this.mEnablePunish)
			{
				return 0;
			}
			if (this.mPunish == null)
			{
				return 0;
			}
			return this.mMaxNum - Math.Min(this.mPunish[x, y], 3);
		}

		public List<PathFinderNode> FindPath(Point2D start, Point2D end)
		{
			List<PathFinderNode> result;
			lock (this)
			{
				long ticks = DateTime.Now.Ticks;
				Array.Clear(PathFinderFast.mCalcGrid, 0, PathFinderFast.mCalcGrid.Length);
				this.mFound = false;
				this.mStop = false;
				this.mStopped = false;
				this.mCloseNodeCounter = 0;
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
				this.mLocation = (start.Y << (int)this.mGridYLog2) + start.X;
				this.mEndLocation = (end.Y << (int)this.mGridYLog2) + end.X;
				PathFinderFast.mCalcGrid[this.mLocation].G = 0;
				PathFinderFast.mCalcGrid[this.mLocation].F = this.mHEstimate;
				PathFinderFast.mCalcGrid[this.mLocation].PX = (ushort)start.X;
				PathFinderFast.mCalcGrid[this.mLocation].PY = (ushort)start.Y;
				PathFinderFast.mCalcGrid[this.mLocation].Status = this.mOpenNodeValue;
				this.mOpen.Push(this.mLocation);
				while (this.mOpen.Count > 0 && !this.mStop)
				{
					this.mLocation = this.mOpen.Pop();
					if (PathFinderFast.mCalcGrid[this.mLocation].Status != this.mCloseNodeValue)
					{
						this.mLocationX = (ushort)(this.mLocation & (int)this.mGridXMinus1);
						this.mLocationY = (ushort)(this.mLocation >> (int)this.mGridYLog2);
						if (this.mDebugProgress && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(0, 0, this.mLocation & (int)this.mGridXMinus1, this.mLocation >> (int)this.mGridYLog2, PathFinderNodeType.Current, -1, -1);
						}
						if (this.mLocation == this.mEndLocation)
						{
							PathFinderFast.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
							this.mFound = true;
							break;
						}
						if (this.mCloseNodeCounter > this.mSearchLimit)
						{
							this.mStopped = true;
							return null;
						}
						if (this.mPunishChangeDirection)
						{
							this.mHoriz = (int)(this.mLocationX - PathFinderFast.mCalcGrid[this.mLocation].PX);
						}
						for (int i = 0; i < ((!this.mDiagonals) ? 4 : 8); i++)
						{
							this.mNewLocationX = (ushort)((int)this.mLocationX + (int)this.mDirection[i, 0]);
							this.mNewLocationY = (ushort)((int)this.mLocationY + (int)this.mDirection[i, 1]);
							this.mNewLocation = ((int)this.mNewLocationY << (int)this.mGridYLog2) + (int)this.mNewLocationX;
							if (this.mNewLocationX < this.mGridX && this.mNewLocationY < this.mGridY)
							{
								if (PathFinderFast.mCalcGrid[this.mNewLocation].Status != this.mCloseNodeValue || this.mReopenCloseNodes)
								{
									if (this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY] != 0)
									{
										if (this.mHeavyDiagonals && i > 3)
										{
											this.mNewG = PathFinderFast.mCalcGrid[this.mLocation].G + (int)((double)this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY] * 2.41);
										}
										else
										{
											this.mNewG = PathFinderFast.mCalcGrid[this.mLocation].G + (int)this.mGrid[(int)this.mNewLocationX, (int)this.mNewLocationY];
										}
										if (this.mPunishChangeDirection)
										{
											if (this.mNewLocationX - this.mLocationX != 0 && this.mHoriz == 0)
											{
												this.mNewG += Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
											}
											if (this.mNewLocationY - this.mLocationY != 0 && this.mHoriz != 0)
											{
												this.mNewG += Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
											}
										}
										this.mNewG += this.GetPunishNum((int)this.mNewLocationX, (int)this.mNewLocationY);
										if ((PathFinderFast.mCalcGrid[this.mNewLocation].Status != this.mOpenNodeValue && PathFinderFast.mCalcGrid[this.mNewLocation].Status != this.mCloseNodeValue) || PathFinderFast.mCalcGrid[this.mNewLocation].G > this.mNewG)
										{
											PathFinderFast.mCalcGrid[this.mNewLocation].PX = this.mLocationX;
											PathFinderFast.mCalcGrid[this.mNewLocation].PY = this.mLocationY;
											PathFinderFast.mCalcGrid[this.mNewLocation].G = this.mNewG;
											switch (this.mFormula)
											{
											default:
												this.mH = this.mHEstimate * (Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y));
												break;
											case HeuristicFormula.MaxDXDY:
												this.mH = this.mHEstimate * Math.Max(Math.Abs((int)this.mNewLocationX - end.X), Math.Abs((int)this.mNewLocationY - end.Y));
												break;
											case HeuristicFormula.DiagonalShortCut:
											{
												int num = Math.Min(Math.Abs((int)this.mNewLocationX - end.X), Math.Abs((int)this.mNewLocationY - end.Y));
												int num2 = Math.Abs((int)this.mNewLocationX - end.X) + Math.Abs((int)this.mNewLocationY - end.Y);
												this.mH = this.mHEstimate * 2 * num + this.mHEstimate * (num2 - 2 * num);
												break;
											}
											case HeuristicFormula.Euclidean:
												this.mH = (int)((double)this.mHEstimate * Math.Sqrt(Math.Pow((double)((int)this.mNewLocationY - end.X), 2.0) + Math.Pow((double)((int)this.mNewLocationY - end.Y), 2.0)));
												break;
											case HeuristicFormula.EuclideanNoSQR:
												this.mH = (int)((double)this.mHEstimate * (Math.Pow((double)((int)this.mNewLocationX - end.X), 2.0) + Math.Pow((double)((int)this.mNewLocationY - end.Y), 2.0)));
												break;
											case HeuristicFormula.Custom1:
											{
												Point2D point2D = new Point2D(Math.Abs(end.X - (int)this.mNewLocationX), Math.Abs(end.Y - (int)this.mNewLocationY));
												int num3 = Math.Abs(point2D.X - point2D.Y);
												int num4 = Math.Abs((point2D.X + point2D.Y - num3) / 2);
												this.mH = this.mHEstimate * (num4 + num3 + point2D.X + point2D.Y);
												break;
											}
											}
											if (this.mTieBreaker)
											{
												int num5 = (int)this.mLocationX - end.X;
												int num6 = (int)this.mLocationY - end.Y;
												int num7 = start.X - end.X;
												int num8 = start.Y - end.Y;
												int num9 = Math.Abs(num5 * num8 - num7 * num6);
												this.mH = (int)((double)this.mH + (double)num9 * 0.001);
											}
											PathFinderFast.mCalcGrid[this.mNewLocation].F = this.mNewG + this.mH;
											if (this.mDebugProgress && this.PathFinderDebug != null)
											{
												this.PathFinderDebug((int)this.mLocationX, (int)this.mLocationY, (int)this.mNewLocationX, (int)this.mNewLocationY, PathFinderNodeType.Open, PathFinderFast.mCalcGrid[this.mNewLocation].F, PathFinderFast.mCalcGrid[this.mNewLocation].G);
											}
											this.mOpen.Push(this.mNewLocation);
											PathFinderFast.mCalcGrid[this.mNewLocation].Status = this.mOpenNodeValue;
										}
									}
								}
							}
						}
						this.mCloseNodeCounter++;
						PathFinderFast.mCalcGrid[this.mLocation].Status = this.mCloseNodeValue;
						if (this.mDebugProgress && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(0, 0, (int)this.mLocationX, (int)this.mLocationY, PathFinderNodeType.Close, PathFinderFast.mCalcGrid[this.mLocation].F, PathFinderFast.mCalcGrid[this.mLocation].G);
						}
					}
				}
				long num10 = (DateTime.Now.Ticks - ticks) / 10000L;
				if (this.mFound)
				{
					this.mClose.Clear();
					int num11 = end.X;
					int num12 = end.Y;
					PathFinderFast.PathFinderNodeFast pathFinderNodeFast = PathFinderFast.mCalcGrid[(end.Y << (int)this.mGridYLog2) + end.X];
					PathFinderNode pathFinderNode;
					pathFinderNode.F = pathFinderNodeFast.F;
					pathFinderNode.G = pathFinderNodeFast.G;
					pathFinderNode.H = 0;
					pathFinderNode.PX = (int)pathFinderNodeFast.PX;
					pathFinderNode.PY = (int)pathFinderNodeFast.PY;
					pathFinderNode.X = end.X;
					pathFinderNode.Y = end.Y;
					while (pathFinderNode.X != pathFinderNode.PX || pathFinderNode.Y != pathFinderNode.PY)
					{
						this.mClose.Add(pathFinderNode);
						if (this.mDebugFoundPath && this.PathFinderDebug != null)
						{
							this.PathFinderDebug(pathFinderNode.PX, pathFinderNode.PY, pathFinderNode.X, pathFinderNode.Y, PathFinderNodeType.Path, pathFinderNode.F, pathFinderNode.G);
						}
						num11 = pathFinderNode.PX;
						num12 = pathFinderNode.PY;
						pathFinderNodeFast = PathFinderFast.mCalcGrid[(num12 << (int)this.mGridYLog2) + num11];
						pathFinderNode.F = pathFinderNodeFast.F;
						pathFinderNode.G = pathFinderNodeFast.G;
						pathFinderNode.H = 0;
						pathFinderNode.PX = (int)pathFinderNodeFast.PX;
						pathFinderNode.PY = (int)pathFinderNodeFast.PY;
						pathFinderNode.X = num11;
						pathFinderNode.Y = num12;
					}
					this.mClose.Add(pathFinderNode);
					if (this.mDebugFoundPath && this.PathFinderDebug != null)
					{
						this.PathFinderDebug(pathFinderNode.PX, pathFinderNode.PY, pathFinderNode.X, pathFinderNode.Y, PathFinderNodeType.Path, pathFinderNode.F, pathFinderNode.G);
					}
					this.mClose = PathFinderFast.Floyd(this.mClose, ref this.mGrid);
					this.mStopped = true;
					result = this.mClose;
				}
				else
				{
					this.mStopped = true;
					result = null;
				}
			}
			return result;
		}

		public static List<PathFinderNode> Floyd(List<PathFinderNode> _floydPath, ref byte[,] grid)
		{
			if (_floydPath == null || _floydPath.Count <= 0)
			{
				return null;
			}
			_floydPath = PathFinderFast.ReverseList(_floydPath);
			int count = _floydPath.Count;
			if (count > 2)
			{
				PathFinderNode pathFinderNode = default(PathFinderNode);
				PathFinderNode pathFinderNode2 = default(PathFinderNode);
				PathFinderFast.FloydVector(ref pathFinderNode, _floydPath[count - 1], _floydPath[count - 2]);
				for (int i = _floydPath.Count - 3; i >= 0; i--)
				{
					PathFinderFast.FloydVector(ref pathFinderNode2, _floydPath[i + 1], _floydPath[i]);
					if (pathFinderNode.X == pathFinderNode2.X && pathFinderNode.Y == pathFinderNode2.Y)
					{
						_floydPath.RemoveAt(i + 1);
					}
					else
					{
						pathFinderNode.X = pathFinderNode2.X;
						pathFinderNode.Y = pathFinderNode2.Y;
					}
				}
			}
			count = _floydPath.Count;
			for (int j = count - 1; j >= 0; j--)
			{
				for (int k = 0; k <= j - 2; k++)
				{
					if (!PathFinderFast.hasBarrier(ref grid, _floydPath[j].X, _floydPath[j].Y, _floydPath[k].X, _floydPath[k].Y))
					{
						for (int l = j - 1; l > k; l--)
						{
							_floydPath.RemoveAt(l);
						}
						j = k;
						count = _floydPath.Count;
						break;
					}
				}
			}
			return _floydPath;
		}

		private static List<PathFinderNode> ReverseList(List<PathFinderNode> floydPath)
		{
			floydPath.Reverse(0, floydPath.Count);
			return floydPath;
		}

		private static void FloydVector(ref PathFinderNode target, PathFinderNode n1, PathFinderNode n2)
		{
			target.X = n1.X - n2.X;
			target.Y = n1.Y - n2.Y;
		}

		public static bool hasBarrier(ref byte[,] grid, int startX, int startY, int endX, int endY)
		{
			if (startX == endX && startY == endY)
			{
				return false;
			}
			if (grid[endX, endY] == 0)
			{
				return true;
			}
			PointF ponit = new PointF((float)startX + 0.5f, (float)startY + 0.5f);
			PointF point = new PointF((float)endX + 0.5f, (float)endY + 0.5f);
			float num = (float)Math.Abs(endX - startX);
			float num2 = (float)Math.Abs(endY - startY);
			bool flag = num > num2;
			if (flag)
			{
				CalcLineHandler lineFunc = MathUtilX.getLineFunc(ponit, point, 0);
				int num3 = Math.Min(startX, endX);
				int num4 = Math.Max(startX, endX);
				for (float num5 = (float)num3; num5 <= (float)num4; num5 += 1f)
				{
					if (num5 == (float)num3)
					{
						num5 += 0.5f;
					}
					float yPos = lineFunc(num5);
					List<ANode> nodesUnderPoint = PathFinderFast.getNodesUnderPoint(num5, yPos);
					for (int i = 0; i < nodesUnderPoint.Count; i++)
					{
						ANode anode = nodesUnderPoint[i];
						if (grid[anode.x, anode.y] == 0)
						{
							return true;
						}
					}
					if (num5 == (float)num3 + 0.5f)
					{
						num5 -= 0.5f;
					}
				}
			}
			else
			{
				CalcLineHandler lineFunc = MathUtilX.getLineFunc(ponit, point, 1);
				int num3 = Math.Min(startY, endY);
				int num4 = Math.Max(startY, endY);
				for (float num5 = (float)num3; num5 <= (float)num4; num5 += 1f)
				{
					if (num5 == (float)num3)
					{
						num5 += 0.5f;
					}
					float xPos = lineFunc(num5);
					List<ANode> nodesUnderPoint = PathFinderFast.getNodesUnderPoint(xPos, num5);
					for (int j = 0; j < nodesUnderPoint.Count; j++)
					{
						ANode anode = nodesUnderPoint[j];
						if (grid[anode.x, anode.y] == 0)
						{
							return true;
						}
					}
					if (num5 == (float)num3 + 0.5f)
					{
						num5 -= 0.5f;
					}
				}
			}
			return false;
		}

		public static List<ANode> getNodesUnderPoint(float xPos, float yPos)
		{
			List<ANode> list = new List<ANode>();
			bool flag = xPos % 1f == 0f;
			bool flag2 = yPos % 1f == 0f;
			if (flag && flag2)
			{
				list.Add(new ANode((int)xPos - 1, (int)yPos - 1));
				list.Add(new ANode((int)xPos, (int)yPos - 1));
				list.Add(new ANode((int)xPos - 1, (int)yPos));
				list.Add(new ANode((int)xPos, (int)yPos));
			}
			else if (flag && !flag2)
			{
				list.Add(new ANode((int)xPos - 1, (int)yPos));
				list.Add(new ANode((int)xPos, (int)yPos));
			}
			else if (!flag && flag2)
			{
				list.Add(new ANode((int)xPos, (int)yPos - 1));
				list.Add(new ANode((int)xPos, (int)yPos));
			}
			else
			{
				list.Add(new ANode((int)xPos, (int)yPos));
			}
			return list;
		}

		private byte[,] mGrid;

		private PriorityQueueB<int> mOpen;

		private List<PathFinderNode> mClose = new List<PathFinderNode>();

		private bool mStop;

		private bool mStopped = true;

		private int mHoriz;

		private HeuristicFormula mFormula = HeuristicFormula.DiagonalShortCut;

		private bool mDiagonals = true;

		private int mHEstimate = 2;

		private bool mPunishChangeDirection;

		private bool mReopenCloseNodes = true;

		private bool mTieBreaker;

		private bool mHeavyDiagonals;

		private int mSearchLimit = 2000;

		private double mCompletedTime;

		private bool mDebugProgress;

		private bool mDebugFoundPath;

		private static PathFinderFast.PathFinderNodeFast[] mCalcGrid;

		private byte mOpenNodeValue = 1;

		private byte mCloseNodeValue = 2;

		private int[,] mPunish;

		private int mMaxNum;

		private bool mEnablePunish;

		private int mH;

		private int mLocation;

		private int mNewLocation;

		private ushort mLocationX;

		private ushort mLocationY;

		private ushort mNewLocationX;

		private ushort mNewLocationY;

		private int mCloseNodeCounter;

		private ushort mGridX;

		private ushort mGridY;

		private ushort mGridXMinus1;

		private ushort mGridYLog2;

		private bool mFound;

		private sbyte[,] mDirection = new sbyte[,]
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

		private int mEndLocation;

		private int mNewG;

		[StructLayout(0, Pack = 1)]
		internal struct PathFinderNodeFast
		{
			public int F;

			public int G;

			public ushort PX;

			public ushort PY;

			public byte Status;
		}

		internal class ComparePFNodeMatrix : IComparer<int>
		{
			public ComparePFNodeMatrix(PathFinderFast.PathFinderNodeFast[] matrix)
			{
				this.mMatrix = matrix;
			}

			public int Compare(int a, int b)
			{
				if (this.mMatrix[a].F > this.mMatrix[b].F)
				{
					return 1;
				}
				if (this.mMatrix[a].F < this.mMatrix[b].F)
				{
					return -1;
				}
				return 0;
			}

			private PathFinderFast.PathFinderNodeFast[] mMatrix;
		}
	}
}
