using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class ShouHuChongResLoader
{
	public ShouHuChongResLoader(ShouHuChongLoadData data, OnShouHuChongLoadComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		GameObject go = null;
		if (this.mData.data == null || this.mData.data.Using <= 0)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, go);
			}
			return;
		}
		string goods3DResNameByID = Global.GetGoods3DResNameByID(this.mData.data.GoodsID, -1);
		if (string.IsNullOrEmpty(goods3DResNameByID))
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, go);
			}
			return;
		}
		this.bundleID = MuAssetManager.GetBundleID("Equip", goods3DResNameByID);
		MuAssetManager.Instance.BeginInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete), CacheType.NotCache, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	public void OnLoadComplete(GameObject go)
	{
		if (null == go)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, go);
			}
			MUDebug.LogError<string>(new string[]
			{
				string.Concat(new object[]
				{
					"越南东南亚英文，宠物模型资源不存在！或模型资源有问题,请检查：",
					this.bundleID,
					";ID=",
					this.mData.data.GoodsID
				})
			});
			return;
		}
		if (go != null)
		{
			if (!this.mData.HideGameEffect)
			{
				U3DUtils.ReplaceAlphaMaterials(go);
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
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南东南亚英文宠物资源已加载：",
				this.bundleID,
				";ID=",
				this.mData.data.GoodsID
			})
		});
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.bundleID, new Action<GameObject>(this.OnLoadComplete));
	}

	private OnShouHuChongLoadComplete mOnComplete;

	private ShouHuChongLoadData mData;

	private string bundleID;
}
