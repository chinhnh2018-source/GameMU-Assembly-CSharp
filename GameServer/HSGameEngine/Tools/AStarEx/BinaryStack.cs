using System;
using System.Collections.Generic;
using System.Linq;

namespace HSGameEngine.Tools.AStarEx
{
	public class BinaryStack
	{
		public BinaryStack(string compareValue = "f")
		{
			this._data = new List<long>(1000);
			this._dict = new Dictionary<long, int>(1000);
		}

		public void push(long guid)
		{
			this._data.Add(guid);
			this._dict[guid] = this._data.Count - 1;
			int count = this._data.Count;
			if (count > 1)
			{
				int num = count;
				int num2 = num / 2 - 1;
				while (this.compareTwoNodes(guid, this._data[num2]))
				{
					long num3 = this._data[num2];
					this._data[num2] = guid;
					this._dict[guid] = num2;
					this._data[num - 1] = num3;
					this._dict[num3] = num - 1;
					num /= 2;
					num2 = num / 2 - 1;
					if (num2 < 0)
					{
						break;
					}
				}
			}
		}

		public long shift()
		{
			long num = this._data.ElementAt(0);
			this._data.RemoveAt(0);
			this._dict.Remove(num);
			int count = this._data.Count;
			if (count > 1)
			{
				long num2 = this._data.ElementAt(this._data.Count - 1);
				this._data.RemoveAt(this._data.Count - 1);
				this._data.Insert(0, num2);
				this._dict[num2] = 0;
				int num3 = 0;
				for (int i = (num3 + 1) * 2 - 1; i < count; i = (num3 + 1) * 2 - 1)
				{
					int num4;
					if (i + 1 == count)
					{
						num4 = i;
					}
					else
					{
						num4 = (this.compareTwoNodes(this._data[i], this._data[i + 1]) ? i : (i + 1));
					}
					if (num4 < 0)
					{
						break;
					}
					if (!this.compareTwoNodes(this._data[num4], num2))
					{
						break;
					}
					long num5 = this._data[num4];
					this._data[num4] = num2;
					this._dict[num2] = num4;
					this._data[num3] = num5;
					this._dict[num5] = num3;
					num3 = num4;
				}
			}
			return num;
		}

		public void updateNode(long node)
		{
			int num = this.indexOf(node);
			if (num >= 0)
			{
				int num2 = num + 1;
				int num3 = num2 / 2 - 1;
				if (num3 >= 0)
				{
					while (this.compareTwoNodes(node, this._data[num3]))
					{
						long num4 = this._data[num3];
						this._data[num3] = node;
						this._dict[node] = num3;
						this._data[num2 - 1] = num4;
						this._dict[num4] = num2 - 1;
						num2 /= 2;
						num3 = num2 / 2 - 1;
						if (num3 < 0)
						{
							break;
						}
					}
				}
			}
		}

		public int indexOf(long node)
		{
			int num = -1;
			int result;
			if (this._dict.TryGetValue(node, out num))
			{
				result = num;
			}
			else
			{
				result = -1;
			}
			return result;
		}

		public int getLength()
		{
			return this._data.Count;
		}

		private bool compareTwoNodes(long node1, long node2)
		{
			double f = this._nodeGrid.Nodes[ANode.GetGUID_X(node1), ANode.GetGUID_Y(node1)].f;
			double f2 = this._nodeGrid.Nodes[ANode.GetGUID_X(node2), ANode.GetGUID_Y(node2)].f;
			return f < f2;
		}

		public string toString()
		{
			string text = "";
			int count = this._data.Count;
			for (int i = 0; i < count; i++)
			{
				double f = this._nodeGrid.Nodes[ANode.GetGUID_X(this._data[i]), ANode.GetGUID_Y(this._data[i])].f;
				text += f;
				if (i < count - 1)
				{
					text += ",";
				}
			}
			return text;
		}

		public void ClearAll()
		{
			this._data.Clear();
			this._dict.Clear();
		}

		public NodeGrid _nodeGrid = null;

		public List<long> _data = null;

		private Dictionary<long, int> _dict = null;
	}
}
