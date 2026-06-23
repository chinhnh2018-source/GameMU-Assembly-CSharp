using System;
using System.Collections;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class MonsterNPCResLoader
{
	public MonsterNPCResLoader(MonsterNPCLoaderData data, OnLoadMonsterNPCComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		if (string.IsNullOrEmpty(this.mData.resName))
		{
			if (this.mOnComplete != null)
			{
				this.mOnComplete(data, null);
			}
			return;
		}
		string[] array = new string[3];
		if (this.mData.spriteType == GSpriteTypes.Monster)
		{
			array[0] = MuAssetManager.GetBundleID("Monster", this.mData.resName);
		}
		else if (this.mData.spriteType == GSpriteTypes.NPC)
		{
			array[0] = MuAssetManager.GetBundleID("NPC", this.mData.resName);
		}
		string name = string.Empty;
		if (this.mData.rightWeaponID > 0)
		{
			name = Global.GetGoods3DResNameByID(this.mData.rightWeaponID, -1);
			array[1] = MuAssetManager.GetBundleID("Equip", name);
		}
		if (this.mData.leftWeaponID > 0)
		{
			name = Global.GetGoods3DResNameByID(this.mData.leftWeaponID, -1);
			array[2] = MuAssetManager.GetBundleID("Equip", name);
		}
		if (this.mData.spriteType == GSpriteTypes.Monster)
		{
			this.requestID = MuAssetManager.Instance.BeginInstantiate(array, new Action<GameObject[]>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
		}
		else if (this.mData.spriteType == GSpriteTypes.NPC)
		{
			this.requestID = MuAssetManager.Instance.BeginInstantiate(array, new Action<GameObject[]>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
		}
	}

	public IEnumerator Load()
	{
		return null;
	}

	private void LoadComplete(GameObject[] go)
	{
		if (go.Length == 0 || null == go[0])
		{
			if (go[1])
			{
				Object.Destroy(go[1]);
			}
			if (go[2])
			{
				Object.Destroy(go[2]);
			}
			if (this.mOnComplete != null)
			{
				this.mOnComplete(this.mData, null);
			}
		}
		GameObject gameObject = go[0];
		GameObject gameObject2 = go[1];
		GameObject gameObject3 = go[2];
		if (null != gameObject2 && gameObject != null)
		{
			if (!this.mData.hideEffect)
			{
				if (this.mData.rightShaderID > 0)
				{
					U3DUtils.ReplaceMaterials(gameObject2, this.mData.rightShaderID, true);
				}
				Global.AddSpecialGameObjects4Goods(gameObject2, this.mData.rightWeaponID, this.mData.layer, this.mData.scale, null);
			}
			string theName = "youshou";
			GameObject gameObject4 = U3DUtils.FindGameObjectByName(gameObject, theName);
			if (null != gameObject4)
			{
				U3DUtils.AddChild(gameObject4, gameObject2, true);
			}
			else
			{
				Object.Destroy(gameObject2);
			}
		}
		if (null != gameObject3 && gameObject != null)
		{
			if (!this.mData.hideEffect)
			{
				if (this.mData.leftShaderID > 0)
				{
					U3DUtils.ReplaceMaterials(gameObject3, this.mData.leftShaderID, true);
				}
				Global.AddSpecialGameObjects4Goods(gameObject3, this.mData.leftWeaponID, this.mData.layer, this.mData.scale, null);
			}
			string theName2 = "zuoshou";
			if (Global.GetCategoriyByGoodsID(this.mData.leftWeaponID) == 18)
			{
				theName2 = "dun";
			}
			GameObject gameObject5 = U3DUtils.FindGameObjectByName(gameObject, theName2);
			if (null != gameObject5)
			{
				U3DUtils.AddChild(gameObject5, gameObject3, true);
			}
			else
			{
				Object.Destroy(gameObject3);
			}
		}
		if (this.mData.ReplaceChildLayer && gameObject != null)
		{
			U3DUtils.ReplaceLayerInChildren(gameObject, this.mData.layer, null);
		}
		if (this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, gameObject);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.requestID);
	}

	private OnLoadMonsterNPCComplete mOnComplete;

	public MonsterNPCLoaderData mData;

	private int requestID;
}
