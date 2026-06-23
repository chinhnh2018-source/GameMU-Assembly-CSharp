using System;
using System.Collections.Generic;
using System.Linq;

namespace Server.Tools.AStarEx
{
	public class BinaryStack
	{
		public BinaryStack(string compareValue = "f")
		{
			this._data = new List<int>(2000);
			this._dict = new Dictionary<int, int>(2000);
		}

		public void push(int guid)
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
					int num3 = this._data[num2];
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

		public int shift()
		{
			int num = Enumerable.ElementAt<int>(this._data, 0);
			this._data.RemoveAt(0);
			this._dict.Remove(num);
			int count = this._data.Count;
			if (count > 1)
			{
				int num2 = Enumerable.ElementAt<int>(this._data, this._data.Count - 1);
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
						num4 = ((!this.compareTwoNodes(this._data[i], this._data[i + 1])) ? (i + 1) : i);
					}
					if (num4 < 0)
					{
						break;
					}
					if (!this.compareTwoNodes(this._data[num4], num2))
					{
						break;
					}
					int num5 = this._data[num4];
					this._data[num4] = num2;
					this._dict[num2] = num4;
					this._data[num3] = num5;
					this._dict[num5] = num3;
					num3 = num4;
				}
			}
			return num;
		}

		public void updateNode(int indexObj, int node)
		{
			int num = indexObj + 1;
			int num2 = num / 2 - 1;
			if (num2 < 0)
			{
				return;
			}
			while (this.compareTwoNodes(node, this._data[num2]))
			{
				int num3 = this._data[num2];
				this._data[num2] = node;
				this._dict[node] = num2;
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

		public int indexOf(int node)
		{
			int result = -1;
			if (this._dict.TryGetValue(node, ref result))
			{
				return result;
			}
			return -1;
		}

		public int getLength()
		{
			return this._data.Count;
		}

		private bool compareTwoNodes(int node1, int node2)
		{
			float f = this._nodeGrid.Nodes[ANode.GetGUID_X(node1), ANode.GetGUID_Y(node1)].f;
			float f2 = this._nodeGrid.Nodes[ANode.GetGUID_X(node2), ANode.GetGUID_Y(node2)].f;
			return f < f2;
		}

		public string toString()
		{
			string text = string.Empty;
			int count = this._data.Count;
			for (int i = 0; i < count; i++)
			{
				double num = (double)this._nodeGrid.Nodes[ANode.GetGUID_X(this._data[i]), ANode.GetGUID_Y(this._data[i])].f;
				text += num;
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

		public NodeGrid _nodeGrid;

		public List<int> _data;

		private Dictionary<int, int> _dict;
	}
}
