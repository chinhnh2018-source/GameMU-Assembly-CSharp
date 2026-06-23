using System;
using System.Collections.Generic;

namespace Server.Tools.AStarEx
{
	public class AStar
	{
		public List<ANode> find(NodeGrid grid)
		{
			if (this.findPath(grid))
			{
				return this._path;
			}
			return null;
		}

		public bool findPath(NodeGrid grid)
		{
			this._grid = grid;
			if (this._open == null)
			{
				this._open = new BinaryStack("f");
			}
			else
			{
				this._open.ClearAll();
			}
			grid.Clear();
			this._open._nodeGrid = grid;
			this._startNodeX = this._grid.startNodeX;
			this._startNodeY = this._grid.startNodeY;
			this._endNodeX = this._grid.endNodeX;
			this._endNodeY = this._grid.endNodeY;
			this._grid.Nodes[this._startNodeX, this._startNodeY].g = 0f;
			this._grid.Nodes[this._startNodeX, this._startNodeY].h = this.diagonal(this._startNodeX, this._startNodeY);
			this._grid.Nodes[this._startNodeX, this._startNodeY].f = this._grid.Nodes[this._startNodeX, this._startNodeY].g + this._grid.Nodes[this._startNodeX, this._startNodeY].h;
			return this.search();
		}

		public bool search()
		{
			try
			{
				long ticks = DateTime.Now.Ticks;
				int num = ANode.GetGUID(this._startNodeX, this._startNodeY);
				int guid = ANode.GetGUID(this._endNodeX, this._endNodeY);
				num = ANode.GetGUID(88, 159);
				guid = ANode.GetGUID(81, 160);
				int num2 = 0;
				int guid_X;
				int guid_Y;
				while (num != guid)
				{
					guid_X = ANode.GetGUID_X(num);
					guid_Y = ANode.GetGUID_Y(num);
					int num3 = (0 <= guid_X - 1) ? (guid_X - 1) : 0;
					int num4 = (this._grid.numCols - 1 >= guid_X + 1) ? (guid_X + 1) : (this._grid.numCols - 1);
					int num5 = (0 <= guid_Y - 1) ? (guid_Y - 1) : 0;
					int num6 = (this._grid.numRows - 1 >= guid_Y + 1) ? (guid_Y + 1) : (this._grid.numRows - 1);
					for (int i = num3; i <= num4; i++)
					{
						for (int j = num5; j <= num6; j++)
						{
							int guid2 = ANode.GetGUID(i, j);
							int num7 = i;
							int num8 = j;
							bool flag = this._grid.isWalkable(num7, num8);
							if (guid2 != num && flag && this._grid.isDiagonalWalkable(num, guid2))
							{
								float num9 = this._straightCost;
								if (guid_X != num7 && guid_Y != num8)
								{
									num9 = this._diagCost;
								}
								float g = this._grid.Nodes[guid_X, guid_Y].g;
								float num10 = g + num9 * 1f;
								float num11 = this.diagonal(num7, num8);
								float num12 = num10 + num11;
								int num13 = this._open.indexOf(guid2);
								bool flag2 = num13 != -1;
								int num14 = this.IndexOfClose(num7, num8);
								if (flag2 || num14 != -1)
								{
									if (this._grid.Nodes[num7, num8].f > num12)
									{
										this._grid.Nodes[num7, num8].f = num12;
										this._grid.Nodes[num7, num8].g = num10;
										this._grid.Nodes[num7, num8].h = num11;
										this._grid.Nodes[num7, num8].parentX = guid_X;
										this._grid.Nodes[num7, num8].parentY = guid_Y;
										if (flag2)
										{
											this._open.updateNode(num13, guid2);
										}
									}
								}
								else
								{
									this._grid.Nodes[num7, num8].f = num12;
									this._grid.Nodes[num7, num8].g = num10;
									this._grid.Nodes[num7, num8].h = num11;
									this._grid.Nodes[num7, num8].parentX = guid_X;
									this._grid.Nodes[num7, num8].parentY = guid_Y;
									this._open.push(guid2);
								}
							}
						}
					}
					this._grid.Nodes[guid_X, guid_Y].ClosedStatus = 1;
					num2++;
					if (this._open.getLength() == 0)
					{
						long num15 = (DateTime.Now.Ticks - ticks) / 10000L;
						string text = string.Format("start({0}, {1}), to({2}, {3})", new object[]
						{
							this._startNodeX,
							this._startNodeY,
							this._endNodeX,
							this._endNodeY
						});
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
				MUDebug.LogException(ex);
			}
			return true;
		}

		private int IndexOfClose(int nodex, int nodey)
		{
			return (this._grid.Nodes[nodex, nodey].ClosedStatus <= 0) ? -1 : 0;
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
		}

		private bool isDiagonalWalkable(int node1, int node2)
		{
			return this._grid.isDiagonalWalkable(node1, node2);
		}

		private float diagonal(int nodex, int nodey)
		{
			float num = (float)((nodex - this._endNodeX >= 0) ? (nodex - this._endNodeX) : (this._endNodeX - nodex));
			float num2 = num;
			float num3 = (num >= num2) ? num2 : num;
			float num4 = num + num2;
			return this._diagCost * num3 + this._straightCost * (num4 - 2f * num3);
		}

		public List<ANode> path
		{
			get
			{
				return this._path;
			}
		}

		public const float costMultiplier = 1f;

		private BinaryStack _open;

		private NodeGrid _grid;

		private int _endNodeX;

		private int _endNodeY;

		private int _startNodeX;

		private int _startNodeY;

		private List<ANode> _path;

		private float _straightCost = 1f;

		private float _diagCost = 1.41421354f;
	}
}
