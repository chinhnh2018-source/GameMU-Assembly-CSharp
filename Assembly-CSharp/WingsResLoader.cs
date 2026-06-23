using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class WingsResLoader
{
	public WingsResLoader(WingsLoadData data, OnWingsLoadComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		if (this.mData.data == null || this.mData.data.Using <= 0)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(data, null);
			}
			return;
		}
		string goods3DResNameByID = Global.GetGoods3DResNameByID(this.mData.data.GoodsID, -1);
		if (string.IsNullOrEmpty(goods3DResNameByID))
		{
			return;
		}
		this.URL = MuAssetManager.GetBundleID(this.mData.Path, goods3DResNameByID);
		MuAssetManager.Instance.BeginInstantiate(this.URL, new Action<GameObject>(this.OnLoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	public WingsLoadData GetWingsLoadData
	{
		get
		{
			return this.mData;
		}
	}

	private void OnLoadComplete(GameObject go)
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
					"越南东南亚英文，翅膀模型资源不存在！或翅膀模型资源有问题,请检查：",
					this.URL,
					";ID=",
					this.mData.data.GoodsID
				})
			});
			return;
		}
		if (go)
		{
			if (!this.mData.HideGameEffect)
			{
				Global.AddSpecialGameObjects4Goods(go, this.mData.data.GoodsID, this.mData.ToLayer, this.mData.scale, null);
				U3DUtils.ReplaceAlphaMaterials(go);
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
		MUDebug.Log<string>(new string[]
		{
			string.Concat(new object[]
			{
				"越南东南亚英文翅膀资源已加载：",
				this.URL,
				";ID=",
				this.mData.data.GoodsID
			})
		});
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.URL, new Action<GameObject>(this.OnLoadComplete));
	}

	private OnWingsLoadComplete mOnComplete;

	private WingsLoadData mData;

	private string URL;
}
