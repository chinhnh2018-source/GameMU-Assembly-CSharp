using System;

namespace HTMLEngine
{
	public abstract class PoolableObject : IDisposable
	{
		internal void SetPoolHandler(ObjectPoolHandler handler)
		{
			this._handler = handler;
		}

		internal abstract void OnAcquire();

		internal abstract void OnRelease();

		public void Dispose()
		{
			if (this._handler != null)
			{
				this._handler(this);
			}
		}

		~PoolableObject()
		{
			if (this._handler != null)
			{
				HtEngine.Log(HtLogLevel.Warning, "Poolable object is not disposed: " + base.GetType(), new object[0]);
			}
		}

		private ObjectPoolHandler _handler;
	}
}
