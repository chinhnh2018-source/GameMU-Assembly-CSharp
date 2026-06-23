using System;
using System.Collections.Generic;

namespace HTMLEngine
{
	public sealed class OP<T> where T : PoolableObject, new()
	{
		private OP(int capacity)
		{
			if (capacity < 1)
			{
				capacity = 1;
			}
			this._returnHandler = new ObjectPoolHandler(this.ReturnObject);
			this._pool = new Queue<T>();
			for (int i = 0; i < capacity; i++)
			{
				T t = OP<T>.CreateInstance();
				this._pool.Enqueue(t);
			}
			this._capacity = capacity;
		}

		private static T CreateInstance()
		{
			return Activator.CreateInstance<T>();
		}

		public static T Acquire()
		{
			return OP<T>.Instance.AcquireInternal();
		}

		private T AcquireInternal()
		{
			if (this._pool.Count == 0)
			{
				for (int i = 0; i < this._capacity; i++)
				{
					T t = OP<T>.CreateInstance();
					this._pool.Enqueue(t);
				}
				this._capacity *= 2;
			}
			T result = this._pool.Dequeue();
			result.SetPoolHandler(this._returnHandler);
			result.OnAcquire();
			return result;
		}

		private void ReturnObject(PoolableObject obj)
		{
			obj.SetPoolHandler(null);
			obj.OnRelease();
			this._pool.Enqueue((T)((object)obj));
		}

		public int Count
		{
			get
			{
				return this._pool.Count;
			}
		}

		public static readonly OP<T> Instance = new OP<T>(16);

		private readonly Queue<T> _pool;

		private int _capacity;

		private readonly ObjectPoolHandler _returnHandler;
	}
}
