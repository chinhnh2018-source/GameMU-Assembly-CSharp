using System;
using System.Collections;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ChildResLoader
{
	public ChildResLoader(ChildResLoaderData data, OnLoadChildResComplete completeCallback)
	{
		this.mData = data;
		this.mOnCompleted = completeCallback;
		MuAssetManager.Instance.BeginInstantiate(data.bundleID, new Action<GameObject>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	public IEnumerator LoadCoroutine
	{
		get
		{
			return this.mLoadCoroutine;
		}
		set
		{
			this.mLoadCoroutine = value;
		}
	}

	public bool Stopped
	{
		get
		{
			return this.mStopped;
		}
		set
		{
			this.mStopped = value;
		}
	}

	private void LoadComplete(GameObject go)
	{
		if (null == go || this.mData == null || this.mData.parent == null)
		{
			if (this.mOnCompleted != null)
			{
				this.mOnCompleted(this.mData, go);
			}
			return;
		}
		Transform transform = go.transform;
		ManagedObject component = go.GetComponent<ManagedObject>();
		Vector3 vector = transform.localPosition;
		Quaternion localRotation = transform.localRotation;
		Vector3 localScale = transform.localScale;
		if (null != component && component.isCacheObject)
		{
			vector = component.InitPos;
			localRotation = component.InitRotation;
			localScale = component.InitScale;
		}
		transform.parent = this.mData.parent.transform;
		transform.localPosition = this.mData.localPosition;
		transform.localRotation = localRotation;
		transform.localScale = localScale;
		U3DUtils.ReplaceLayerInChildren(go, (this.mData.layer >= 0) ? this.mData.layer : this.mData.parent.layer, null);
		if (go.layer == LayerMask.NameToLayer("MUUI"))
		{
			ParticleSystem[] componentsInChildren = go.GetComponentsInChildren<ParticleSystem>();
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				if (componentsInChildren[i])
				{
					componentsInChildren[i].startSize = this.mData.scale * componentsInChildren[i].startSize;
					componentsInChildren[i].startSpeed = this.mData.scale * componentsInChildren[i].startSpeed;
				}
			}
		}
		if (this.mOnCompleted != null)
		{
			this.mOnCompleted(this.mData, go);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.mData.bundleID, new Action<GameObject>(this.LoadComplete));
	}

	private IEnumerator mLoadCoroutine;

	private bool mStopped;

	private ChildResLoaderData mData;

	private OnLoadChildResComplete mOnCompleted;
}
