using System;
using UnityEngine;

namespace HSGameEngine.GameEngine.AssetManagement
{
	public class CachedObject
	{
		public CacheType isCached
		{
			get
			{
				return this.misCached;
			}
			set
			{
				this.misCached = value;
				if (this.misCached == CacheType.CacheNotRelease)
				{
					this.isNotRelease = true;
				}
			}
		}

		public void Lock()
		{
			this.refCount++;
		}

		public void Unlock()
		{
			this.refCount--;
			if (this.refCount <= 0)
			{
				if (this.isCached == CacheType.NotCache)
				{
					this.unloadTime = Time.time;
				}
				else
				{
					this.unloadTime = Time.time + CachedObject.CachedTime;
				}
			}
		}

		internal void OnResInstanceDestroyed(ManagedObject manageObj)
		{
			this.Unlock();
		}

		private static readonly float CachedTime = 10f;

		public Object res;

		public int refCount;

		public float unloadTime;

		public bool isNotRelease;

		private CacheType misCached = CacheType.CacheAutoRelease;
	}
}
