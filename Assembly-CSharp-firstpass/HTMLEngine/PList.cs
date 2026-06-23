using System;
using System.Collections.Generic;

namespace HTMLEngine
{
	internal class PList<T> : PoolableObject
	{
		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
			this.list.Clear();
		}

		public int Count
		{
			get
			{
				return this.list.Count;
			}
		}

		public void Add(T value)
		{
			this.list.Add(value);
		}

		public IEnumerable<T> Items
		{
			get
			{
				return this.list;
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			return this.list.GetEnumerator();
		}

		public T this[int index]
		{
			get
			{
				return this.list[index];
			}
			set
			{
				this.list[index] = value;
			}
		}

		public override string ToString()
		{
			string result;
			using (PStringBuilder pstringBuilder = OP<PStringBuilder>.Acquire())
			{
				for (int i = 0; i < this.list.Count; i++)
				{
					T t = this.list[i];
					pstringBuilder.Append('[');
					pstringBuilder.Append((!object.Equals(t, default(T))) ? t.ToString() : "null");
					pstringBuilder.Append(']');
					if (i < this.list.Count - 1)
					{
						pstringBuilder.Append(',');
					}
				}
				result = pstringBuilder.ToString();
			}
			return result;
		}

		protected readonly List<T> list = new List<T>();
	}
}
