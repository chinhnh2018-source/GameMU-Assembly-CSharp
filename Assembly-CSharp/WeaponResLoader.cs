using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class WeaponResLoader
{
	public WeaponResLoader(WeaponLoadData data, OnWeaponLoadComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		if (this.mData.weaponList == null || this.mData.weaponList.Count <= 0)
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, new List<GameObject>());
			}
			return;
		}
		if (null != this.mData.parent)
		{
			string[] array = new string[this.mData.weaponList.Count];
			this.weaponData = new List<GoodsData>();
			for (int i = 0; i < this.mData.weaponList.Count; i++)
			{
				array[i] = string.Empty;
				if (this.mData.weaponList[i].Using > 0)
				{
					string goods3DResNameByID = Global.GetGoods3DResNameByID(this.mData.weaponList[i].GoodsID, this.mData.occupation);
					if (!string.IsNullOrEmpty(goods3DResNameByID))
					{
						string bundleID = MuAssetManager.GetBundleID("Equip", goods3DResNameByID);
						this.weaponData.Add(this.mData.weaponList[i]);
						array[i] = bundleID;
					}
				}
			}
			this.requestID = MuAssetManager.Instance.BeginInstantiate(array, new Action<GameObject[]>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
		}
	}

	private void LoadComplete(GameObject[] go)
	{
		if (this.mData.parent == null)
		{
			for (int i = 0; i < go.Length; i++)
			{
				if (go[i])
				{
					Object.Destroy(go[i]);
				}
			}
			return;
		}
		List<GameObject> list = new List<GameObject>();
		for (int j = 0; j < go.Length; j++)
		{
			GameObject gameObject = go[j];
			GoodsData goodsData = this.mData.weaponList[j];
			if (!this.mData.HideGameEffect && gameObject != null)
			{
				Global.AddSpecialGameObjects4Goods(gameObject, goodsData.GoodsID, this.mData.ToLayer, this.mData.Scale, null);
				U3DUtils.ReplaceMaterials(gameObject, Global.GetGoodsShaderIDsByID(goodsData.GoodsID, goodsData.Forge_level), false);
				Dictionary<string, int> dict = new Dictionary<string, int>();
				U3DUtils.SetZhuoYueDict(gameObject, dict, goodsData.ExcellenceInfo);
			}
			if (gameObject != null)
			{
				list.Add(gameObject);
				if (this.mData.ReplaceChildLayer)
				{
					U3DUtils.ReplaceLayerInChildren(gameObject, this.mData.ToLayer, null);
				}
			}
		}
		if (this.mData.parent && this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, list);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.requestID);
	}

	private OnWeaponLoadComplete mOnComplete;

	private WeaponLoadData mData;

	private List<GoodsData> weaponData;

	private int requestID;
}
