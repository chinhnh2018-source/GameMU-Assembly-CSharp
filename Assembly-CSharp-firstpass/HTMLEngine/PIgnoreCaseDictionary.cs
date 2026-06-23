using System;
using System.Collections.Generic;

namespace HTMLEngine
{
	internal class PIgnoreCaseDictionary<V> : PoolableObject
	{
		internal override void OnAcquire()
		{
		}

		internal override void OnRelease()
		{
			this.dict.Clear();
		}

		public void Clear()
		{
			this.dict.Clear();
		}

		public int Count
		{
			get
			{
				return this.dict.Count;
			}
		}

		public IEnumerable<string> Keys
		{
			get
			{
				return this.dict.Keys;
			}
		}

		public IEnumerable<V> Values
		{
			get
			{
				return this.dict.Values;
			}
		}

		public V this[string key]
		{
			get
			{
				V v;
				return (!this.dict.TryGetValue(key, ref v)) ? default(V) : v;
			}
			set
			{
				this.dict[key] = value;
			}
		}

		private readonly Dictionary<string, V> dict = new Dictionary<string, V>();
	}
}
