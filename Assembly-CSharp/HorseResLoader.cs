using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class HorseResLoader
{
	public HorseResLoader(HorseLoaderData data, OnHorserLoaderComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		if (this.mData == null || string.IsNullOrEmpty(this.mData.resName))
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, null);
			}
			return;
		}
		this.URL = MuAssetManager.GetBundleID("Horse", this.mData.resName);
		MuAssetManager.Instance.BeginInstantiate(this.URL, new Action<GameObject>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	private void LoadComplete(GameObject go)
	{
		if (null == go)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, null);
			}
			return;
		}
		U3DUtils.SetToReplaceMaterialsDict(go, this.toReplaceDict, Global.GetGoodsShaderIDsByID(this.mData.GoodsID, this.mData.HorseLevel));
		U3DUtils.ReplaceMaterials(go, this.toReplaceDict);
		go.transform.localPosition = Vector3.zero;
		go.transform.localRotation = Quaternion.identity;
		if (this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, go);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.URL, new Action<GameObject>(this.LoadComplete));
	}

	private OnHorserLoaderComplete mOnComplete;

	private HorseLoaderData mData;

	private string URL = string.Empty;

	private Dictionary<string, int> toReplaceDict = new Dictionary<string, int>();
}
