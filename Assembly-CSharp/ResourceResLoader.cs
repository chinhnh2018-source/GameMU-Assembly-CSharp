using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ResourceResLoader
{
	public ResourceResLoader(ResourceLoadData data, OnResourceLoadComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		string modelResNameByID = Global.GetModelResNameByID(this.mData.modelID);
		if (string.IsNullOrEmpty(modelResNameByID))
		{
			return;
		}
		this.bundleID = MuAssetManager.GetBundleID("UIModel", modelResNameByID);
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
			Animation componentInChildren = go.GetComponentInChildren<Animation>();
			if (null != componentInChildren && null != componentInChildren["Stand"])
			{
				componentInChildren.wrapMode = 2;
				componentInChildren.CrossFade("Stand");
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

	private OnResourceLoadComplete mOnComplete;

	private ResourceLoadData mData;

	private string bundleID;
}
