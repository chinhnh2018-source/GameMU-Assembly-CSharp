using System;
using System.Collections.Generic;

namespace HSGameEngine.Tools.AStar
{
	public class PriorityQueueB<T> : IPriorityQueue<T>
	{
		public PriorityQueueB()
		{
			this.mComparer = Comparer<T>.Default;
		}

		public PriorityQueueB(IComparer<T> comparer)
		{
			this.mComparer = comparer;
		}

		public PriorityQueueB(IComparer<T> comparer, int capacity)
		{
			this.mComparer = comparer;
			this.InnerList.Capacity = capacity;
		}

		protected void SwitchElements(int i, int j)
		{
			T value = this.InnerList[i];
			this.InnerList[i] = this.InnerList[j];
			this.InnerList[j] = value;
		}

		protected virtual int OnCompare(int i, int j)
		{
			return this.mComparer.Compare(this.InnerList[i], this.InnerList[j]);
		}

		public int Push(T item)
		{
			int num = this.InnerList.Count;
			this.InnerList.Add(item);
			while (num != 0)
			{
				int num2 = (num - 1) / 2;
				if (this.OnCompare(num, num2) >= 0)
				{
					return num;
				}
				this.SwitchElements(num, num2);
				num = num2;
			}
			return num;
		}

		public T Pop()
		{
			T result = this.InnerList[0];
			int num = 0;
			this.InnerList[0] = this.InnerList[this.InnerList.Count - 1];
			this.InnerList.RemoveAt(this.InnerList.Count - 1);
			for (;;)
			{
				int num2 = num;
				int num3 = 2 * num + 1;
				int num4 = 2 * num + 2;
				if (this.InnerList.Count > num3 && this.OnCompare(num, num3) > 0)
				{
					num = num3;
				}
				if (this.InnerList.Count > num4 && this.OnCompare(num, num4) > 0)
				{
					num = num4;
				}
				if (num == num2)
				{
					break;
				}
				this.SwitchElements(num, num2);
			}
			return result;
		}

		public void Update(int i)
		{
			int num2;
			for (int num = i; num != 0; num = num2)
			{
				num2 = (num - 1) / 2;
				if (this.OnCompare(num, num2) >= 0)
				{
					IL_44:
					if (num >= i)
					{
						for (;;)
						{
							int num3 = num;
							int num4 = 2 * num + 1;
							num2 = 2 * num + 2;
							if (this.InnerList.Count > num4 && this.OnCompare(num, num4) > 0)
							{
								num = num4;
							}
							if (this.InnerList.Count > num2 && this.OnCompare(num, num2) > 0)
							{
								num = num2;
							}
							if (num == num3)
							{
								break;
							}
							this.SwitchElements(num, num3);
						}
					}
					return;
				}
				this.SwitchElements(num, num2);
			}
			goto IL_44;
		}

		public T Peek()
		{
			T result;
			if (this.InnerList.Count > 0)
			{
				result = this.InnerList[0];
			}
			else
			{
				result = default(T);
			}
			return result;
		}

		public void Clear()
		{
			this.InnerList.Clear();
		}

		public int Count
		{
			get
			{
				return this.InnerList.Count;
			}
		}

		public void RemoveLocation(T item)
		{
			int num = -1;
			for (int i = 0; i < this.InnerList.Count; i++)
			{
				if (this.mComparer.Compare(this.InnerList[i], item) == 0)
				{
					num = i;
				}
			}
			if (num != -1)
			{
				this.InnerList.RemoveAt(num);
			}
		}

		public T this[int index]
		{
			get
			{
				return this.InnerList[index];
			}
			set
			{
				this.InnerList[index] = value;
				this.Update(index);
			}
		}

		protected List<T> InnerList = new List<T>();

		protected IComparer<T> mComparer;
	}
}
