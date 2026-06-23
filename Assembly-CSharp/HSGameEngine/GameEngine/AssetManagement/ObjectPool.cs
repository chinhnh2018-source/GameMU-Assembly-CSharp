using System;
using System.Collections.Generic;
using UnityEngine;

namespace HSGameEngine.GameEngine.AssetManagement
{
	public class ObjectPool
	{
		public ObjectPool(CachedObject res, int max = 5)
		{
			this.mRes = res;
			this.mActiveList = new Dictionary<int, GameObject>();
			this.mDisactiveList = new List<KeyValuePair<GameObject, int>>(8);
			this.mMaxCount = max;
			this.mActiveCount = 0;
		}

		internal Object Resource
		{
			get
			{
				return this.mRes.res;
			}
		}

		internal int ActiveCount
		{
			get
			{
				return this.mActiveCount;
			}
		}

		internal float UnloadTime
		{
			get
			{
				return this.mUnloadTime;
			}
			set
			{
				this.mUnloadTime = value;
			}
		}

		internal GameObject Instantiate(bool force = false, string url = "")
		{
			if (!force && this.mActiveList.Count > this.mMaxCount)
			{
				return null;
			}
			GameObject gameObject = null;
			if (this.mDisactiveList.Count > 0)
			{
				for (int i = this.mDisactiveList.Count - 1; i >= 0; i--)
				{
					if (this.mDisactiveList[i].Value != Time.frameCount)
					{
						gameObject = this.mDisactiveList[i].Key;
						this.mDisactiveList.RemoveAt(i);
						ManagedObject component = gameObject.GetComponent<ManagedObject>();
						component.isNewInstance = false;
						gameObject.SetActive(true);
						component.ManualEnable();
						break;
					}
				}
			}
			if (!gameObject)
			{
				if (this.mRes == null || this.mRes.res == null)
				{
					return null;
				}
				this.mRes.Lock();
				gameObject = (Object.Instantiate(this.mRes.res) as GameObject);
				ManagedObject managedObject = gameObject.AddComponent<ManagedObject>();
				managedObject.InitPos = gameObject.transform.localPosition;
				managedObject.InitRotation = gameObject.transform.rotation;
				managedObject.InitScale = gameObject.transform.localScale;
				managedObject.instanceID = gameObject.GetInstanceID();
				ManagedObject managedObject2 = managedObject;
				managedObject2.onDestroyed = (Action<ManagedObject>)Delegate.Combine(managedObject2.onDestroyed, new Action<ManagedObject>(this.OnObjectDestroyed));
				ManagedObject managedObject3 = managedObject;
				managedObject3.onDisabled = (Action<ManagedObject>)Delegate.Combine(managedObject3.onDisabled, new Action<ManagedObject>(this.OnObjectDisactive));
				ManagedObject managedObject4 = managedObject;
				managedObject4.onDestroyed = (Action<ManagedObject>)Delegate.Combine(managedObject4.onDestroyed, new Action<ManagedObject>(this.mRes.OnResInstanceDestroyed));
				managedObject.isCacheObject = true;
				managedObject.resURL = url;
			}
			if (null != gameObject)
			{
				FxPlayController componentInChildren = gameObject.GetComponentInChildren<FxPlayController>();
				if (null != componentInChildren)
				{
					componentInChildren.enabled = true;
				}
				this.AddActice(gameObject);
			}
			return gameObject;
		}

		internal bool Instantiate(Action<GameObject> endInstantiate = null, bool force = false, string url = "")
		{
			bool result = false;
			if (!force && this.mActiveList.Count > this.mMaxCount)
			{
				return result;
			}
			GameObject gameObject = null;
			if (this.mDisactiveList.Count > 0)
			{
				for (int i = this.mDisactiveList.Count - 1; i >= 0; i--)
				{
					if (this.mDisactiveList[i].Value != Time.frameCount)
					{
						gameObject = this.mDisactiveList[i].Key;
						this.mDisactiveList.RemoveAt(i);
						ManagedObject component = gameObject.GetComponent<ManagedObject>();
						component.isNewInstance = false;
						gameObject.SetActive(true);
						component.enabled = true;
						component.ManualEnable();
						result = true;
						break;
					}
				}
			}
			if (!gameObject)
			{
				this.mRes.Lock();
				gameObject = (Object.Instantiate(this.mRes.res) as GameObject);
				ManagedObject managedObject = gameObject.AddComponent<ManagedObject>();
				managedObject.InitPos = gameObject.transform.localPosition;
				managedObject.InitRotation = gameObject.transform.rotation;
				managedObject.InitScale = gameObject.transform.localScale;
				managedObject.instanceID = gameObject.GetInstanceID();
				ManagedObject managedObject2 = managedObject;
				managedObject2.onDestroyed = (Action<ManagedObject>)Delegate.Combine(managedObject2.onDestroyed, new Action<ManagedObject>(this.OnObjectDestroyed));
				ManagedObject managedObject3 = managedObject;
				managedObject3.onDisabled = (Action<ManagedObject>)Delegate.Combine(managedObject3.onDisabled, new Action<ManagedObject>(this.OnObjectDisactive));
				ManagedObject managedObject4 = managedObject;
				managedObject4.onDestroyed = (Action<ManagedObject>)Delegate.Combine(managedObject4.onDestroyed, new Action<ManagedObject>(this.mRes.OnResInstanceDestroyed));
				managedObject.resURL = url;
				managedObject.isCacheObject = true;
				result = false;
			}
			if (null != gameObject)
			{
				FxPlayController componentInChildren = gameObject.GetComponentInChildren<FxPlayController>();
				if (null != componentInChildren)
				{
					componentInChildren.enabled = true;
				}
			}
			if (endInstantiate != null)
			{
				endInstantiate.Invoke(gameObject);
			}
			if (null != gameObject)
			{
				this.AddActice(gameObject);
			}
			return result;
		}

		internal void AddActice(GameObject go)
		{
			this.mActiveList.Add(go.GetInstanceID(), go);
			this.mActiveCount++;
		}

		internal void OnObjectDisactive(ManagedObject mo)
		{
			if (this.mActiveList.ContainsKey(mo.instanceID))
			{
				this.mActiveList.Remove(mo.instanceID);
				this.mDisactiveList.Add(new KeyValuePair<GameObject, int>(mo.gameObject, Time.frameCount));
				this.mActiveCount--;
				if (this.mActiveCount <= 0)
				{
					this.mUnloadTime = Time.time + ObjectPool.ObjectCacheTime;
				}
			}
		}

		internal void OnObjectDestroyed(ManagedObject mo)
		{
			if (this.mActiveList.ContainsKey(mo.instanceID))
			{
				this.mActiveList.Remove(mo.instanceID);
				this.mActiveCount--;
				if (this.mActiveCount <= 0)
				{
					this.mUnloadTime = Time.time + ObjectPool.ObjectCacheTime;
				}
			}
			else
			{
				for (int i = 0; i < this.mDisactiveList.Count; i++)
				{
					if (!this.mDisactiveList[i].Key || this.mDisactiveList[i].Key.GetInstanceID() == mo.instanceID)
					{
						this.mDisactiveList.RemoveAt(i);
					}
				}
			}
		}

		internal void ClearPool()
		{
			if (this.mActiveList.Count > 0)
			{
				MUDebug.LogError<string>(new string[]
				{
					"Clear an active Pool " + this.mRes.res.name
				});
				return;
			}
			for (int i = 0; i < this.mDisactiveList.Count; i++)
			{
				Object.Destroy(this.mDisactiveList[i].Key);
			}
			this.mDisactiveList.Clear();
			this.mRes = null;
		}

		internal void ForceClearDisactiveList()
		{
			for (int i = 0; i < this.mDisactiveList.Count; i++)
			{
				Object.Destroy(this.mDisactiveList[i].Key);
			}
			this.mDisactiveList.Clear();
			if (this.mActiveList.Count <= 0)
			{
				this.mRes = null;
			}
		}

		internal GameObject LockOne(bool force = false)
		{
			if (!force && this.mActiveList.Count > this.mMaxCount)
			{
				return null;
			}
			GameObject gameObject = null;
			if (this.mDisactiveList.Count > 0)
			{
				for (int i = this.mDisactiveList.Count - 1; i >= 0; i--)
				{
					if (this.mDisactiveList[i].Value != Time.frameCount)
					{
						gameObject = this.mDisactiveList[i].Key;
						this.mDisactiveList.RemoveAt(i);
						this.AddActice(gameObject);
						break;
					}
				}
			}
			return gameObject;
		}

		internal void UnLockOne(GameObject go)
		{
			if (this.mActiveList.ContainsKey(go.GetInstanceID()))
			{
				this.mActiveList.Remove(go.GetInstanceID());
				this.mDisactiveList.Add(new KeyValuePair<GameObject, int>(go.gameObject, Time.frameCount));
				this.mActiveCount--;
				if (this.mActiveCount <= 0)
				{
					this.mUnloadTime = Time.time + ObjectPool.ObjectCacheTime;
				}
			}
		}

		private static readonly float ObjectCacheTime = 10f;

		public bool isNotUnload;

		private CachedObject mRes;

		private Dictionary<int, GameObject> mActiveList;

		private List<KeyValuePair<GameObject, int>> mDisactiveList;

		private int mMaxCount;

		private int mActiveCount;

		private float mUnloadTime;
	}
}
