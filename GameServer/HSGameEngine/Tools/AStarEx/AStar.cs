using System;
using System.Collections.Generic;
using System.Diagnostics;
using Server.Tools;

namespace HSGameEngine.Tools.AStarEx
{
	public class AStar
	{
		public List<ANode> find(NodeGrid grid)
		{
			List<ANode> result;
			if (this.findPath(grid))
			{
				result = this._path;
			}
			else
			{
				result = null;
			}
			return result;
		}

		public bool findPath(NodeGrid grid)
		{
			this._grid = grid;
			if (null == this._open)
			{
				this._open = new BinaryStack("f");
			}
			else
			{
				this._open.ClearAll();
			}
			grid.Clear();
			this._open._nodeGrid = grid;
			if (null == this._closed)
			{
				this._closed = new Dictionary<long, bool>(1000);
			}
			else
			{
				this._closed.Clear();
			}
			this._startNodeX = this._grid.startNodeX;
			this._startNodeY = this._grid.startNodeY;
			this._endNodeX = this._grid.endNodeX;
			this._endNodeY = this._grid.endNodeY;
			this._grid.Nodes[this._startNodeX, this._startNodeY].g = 0.0;
			this._grid.Nodes[this._startNodeX, this._startNodeY].h = this.diagonal(this._startNodeX, this._startNodeY);
			this._grid.Nodes[this._startNodeX, this._startNodeY].f = this._grid.Nodes[this._startNodeX, this._startNodeY].g + this._grid.Nodes[this._startNodeX, this._startNodeY].h;
			return this.search();
		}

		public bool search()
		{
			try
			{
				long num = ANode.GetGUID(this._startNodeX, this._startNodeY);
				long guid = ANode.GetGUID(this._endNodeX, this._endNodeY);
				int guid_X;
				int guid_Y;
				while (num != guid)
				{
					guid_X = ANode.GetGUID_X(num);
					guid_Y = ANode.GetGUID_Y(num);
					int num2 = (0 > guid_X - 1) ? 0 : (guid_X - 1);
					int num3 = (this._grid.numCols - 1 < guid_X + 1) ? (this._grid.numCols - 1) : (guid_X + 1);
					int num4 = (0 > guid_Y - 1) ? 0 : (guid_Y - 1);
					int num5 = (this._grid.numRows - 1 < guid_Y + 1) ? (this._grid.numRows - 1) : (guid_Y + 1);
					for (int i = num2; i <= num3; i++)
					{
						for (int j = num4; j <= num5; j++)
						{
							if (this._open.getLength() > AStar.MaxOpenNodeCount)
							{
								LogManager.WriteLog(1, string.Format("AStar:search()待检测的点太多，没必要再寻路: start({0}, {1}), to({2}, {3}), MaxOpenNodeCount={4}", new object[]
								{
									this._startNodeX,
									this._startNodeY,
									this._endNodeX,
									this._endNodeY,
									AStar.MaxOpenNodeCount
								}), null, true);
								return false;
							}
							long guid2 = ANode.GetGUID(i, j);
							int num6 = i;
							int num7 = j;
							bool flag = this._grid.isWalkable(num6, num7);
							if (guid2 != num && flag && this._grid.isDiagonalWalkable(num, guid2))
							{
								double num8 = this._straightCost;
								if (guid_X != num6 && guid_Y != num7)
								{
									num8 = this._diagCost;
								}
								double g = this._grid.Nodes[guid_X, guid_Y].g;
								double num9 = g + num8 * 1.0;
								double num10 = this.diagonal(num6, num7);
								double num11 = num9 + num10;
								bool flag2 = this._open.indexOf(guid2) != -1;
								int num12 = this.IndexOfClose(guid2);
								if (flag2 || num12 != -1)
								{
									if (this._grid.Nodes[num6, num7].f > num11)
									{
										this._grid.Nodes[num6, num7].f = num11;
										this._grid.Nodes[num6, num7].g = num9;
										this._grid.Nodes[num6, num7].h = num10;
										this._grid.Nodes[num6, num7].parentX = guid_X;
										this._grid.Nodes[num6, num7].parentY = guid_Y;
										if (flag2)
										{
											this._open.updateNode(guid2);
										}
									}
								}
								else
								{
									this._grid.Nodes[num6, num7].f = num11;
									this._grid.Nodes[num6, num7].g = num9;
									this._grid.Nodes[num6, num7].h = num10;
									this._grid.Nodes[num6, num7].parentX = guid_X;
									this._grid.Nodes[num6, num7].parentY = guid_Y;
									this._open.push(guid2);
								}
							}
						}
					}
					this._closed[num] = true;
					if (this._open.getLength() == 0)
					{
						return false;
					}
					num = this._open.shift();
				}
				guid_X = ANode.GetGUID_X(num);
				guid_Y = ANode.GetGUID_Y(num);
				this._endNodeX = guid_X;
				this._endNodeY = guid_Y;
				this.buildPath();
			}
			catch (Exception ex)
			{
				Debug.WriteLine(ex.Message);
			}
			return true;
		}

		private int IndexOfClose(long node)
		{
			return this._closed.ContainsKey(node) ? 0 : -1;
		}

		private void buildPath()
		{
			this._path = new List<ANode>();
			ANode anode = new ANode(this._endNodeX, this._endNodeY);
			this._path.Add(anode);
			int num = 0;
			while (anode.x != this._startNodeX || anode.y != this._startNodeY)
			{
				int parentX = this._grid.Nodes[anode.x, anode.y].parentX;
				int parentY = this._grid.Nodes[anode.x, anode.y].parentY;
				anode = new ANode(parentX, parentY);
				this._path.Insert(0, anode);
				num++;
			}
			Debug.WriteLine(string.Format("Find Path count={0}", num));
		}

		private bool isDiagonalWalkable(long node1, long node2)
		{
			return this._grid.isDiagonalWalkable(node1, node2);
		}

		private double diagonal(int nodex, int nodey)
		{
			double num = (double)((nodex - this._endNodeX < 0) ? (this._endNodeX - nodex) : (nodex - this._endNodeX));
			double num2 = (double)((nodey - this._endNodeY < 0) ? (this._endNodeY - nodey) : (nodey - this._endNodeY));
			double num3 = (num < num2) ? num : num2;
			double num4 = num + num2;
			return this._diagCost * num3 + this._straightCost * (num4 - 2.0 * num3);
		}

		public List<ANode> path
		{
			get
			{
				return this._path;
			}
		}

		public const double costMultiplier = 1.0;

		private BinaryStack _open;

		private Dictionary<long, bool> _closed;

		private NodeGrid _grid;

		private int _endNodeX;

		private int _endNodeY;

		private int _startNodeX;

		private int _startNodeY;

		private List<ANode> _path;

		private double _straightCost = 1.0;

		private double _diagCost = 1.4142135623730951;

		public static int MaxOpenNodeCount = 200;
	}
}
