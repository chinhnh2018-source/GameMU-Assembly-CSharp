using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class WingsLingyuResLoader
{
	public WingsLingyuResLoader(WingsLingYuLoadData data, OnWingsLingYuLoadComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		if (string.IsNullOrEmpty(this.mData.resName))
		{
			return;
		}
		this.bundleID = MuAssetManager.GetBundleID(this.mData.path, this.mData.resName);
		MuAssetManager.Instance.BeginInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	private void OnLoadComplete(GameObject go)
	{
		if (null == go)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, go);
			}
			return;
		}
		if (go)
		{
			if (!this.mData.HideGameEffect)
			{
			}
			Animation component = go.GetComponent<Animation>();
			if (null != component)
			{
				component.wrapMode = 2;
				component.CrossFade("Stand");
			}
			if (this.mData.ReplaceChildLayer)
			{
				U3DUtils.ReplaceLayerInChildren(go, this.mData.ToLayer, null);
			}
		}
		if (this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, go);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete));
	}

	private OnWingsLingYuLoadComplete mOnComplete;

	private WingsLingYuLoadData mData;

	private string bundleID;
}
