using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.U3D;
using Server.Data;
using UnityEngine;

public class RoleResLoader
{
	public RoleResLoader(RoleLoaderData data, OnLoadRoleComplete completeCallback)
	{
		this.mData = data;
		this.mOnComplete = completeCallback;
		this.partsList.Clear();
		this.toReplaceDict.Clear();
		this.toZhuoYueDict.Clear();
		if (this.mData.IsChangeBody)
		{
			this.bundleIDs = new List<string>();
			string monster3DResNameByID = ConfigMonsters.GetMonster3DResNameByID(this.mData.ChangeBodyID);
			string bundleID = MuAssetManager.GetBundleID("Monster", monster3DResNameByID);
			this.bundleIDs.Add(bundleID);
			this.requestID = MuAssetManager.Instance.PreLoadLeader(this.bundleIDs.ToArray(), new Action<string[]>(this.OnLoadBianShenComplete));
			return;
		}
		if (this.mData.GoodsDataList != null && this.mData.GoodsDataList.Count > 0)
		{
			this.bundleIDs = new List<string>();
			for (int i = 0; i < this.mData.GoodsDataList.Count; i++)
			{
				GoodsData goodsData = this.mData.GoodsDataList[i];
				if (goodsData != null && goodsData.Using > 0)
				{
					int num = Global.GetGoodsCatetoriy(goodsData.GoodsID);
					if ((num >= 0 && num <= 4 && num != 26) || (num >= 30 && num <= 38))
					{
						string goods3DResNameByID = Global.GetGoods3DResNameByID(goodsData.GoodsID, (this.mData.LoadRebitrhEquit != 1) ? -1 : this.mData.Occupation);
						if (!string.IsNullOrEmpty(goods3DResNameByID))
						{
							string bundleID2 = MuAssetManager.GetBundleID("Equip", goods3DResNameByID);
							if (!string.IsNullOrEmpty(bundleID2))
							{
								num = Global.CheckCategoriy(num);
								this.GoDict[num] = new RoleLoaderResult
								{
									Url = bundleID2,
									MyGoodsData = goodsData
								};
								this.bundleIDs.Add(bundleID2);
							}
						}
					}
				}
			}
			this.requestID = MuAssetManager.Instance.PreLoadLeader(this.bundleIDs.ToArray(), new Action<string[]>(this.OnLoadComplete));
		}
		else
		{
			this.OnLoadComplete(null);
		}
	}

	public void OnLoadComplete(string[] urls)
	{
		for (int i = 0; i < 5; i++)
		{
			int itemCategoriyByBodyPartID = Global.GetItemCategoriyByBodyPartID(i);
			if (this.GoDict.ContainsKey(itemCategoriyByBodyPartID))
			{
				RoleLoaderResult roleLoaderResult = this.GoDict[itemCategoriyByBodyPartID];
				GameObject gameObject = MuAssetManager.Instance.InstantiateLeaderGameObject(roleLoaderResult.Url);
				if (!this.mData.HideGameEffect && gameObject != null)
				{
					Global.AddSpecialGameObjects4Goods(gameObject, roleLoaderResult.MyGoodsData.GoodsID, this.mData.ToLayer, this.mData.Scale, null);
					U3DUtils.SetToReplaceMaterialsDict(gameObject, this.toReplaceDict, Global.GetGoodsShaderIDsByID(roleLoaderResult.MyGoodsData.GoodsID, roleLoaderResult.MyGoodsData.Forge_level));
					U3DUtils.SetZhuoYueDict(gameObject, this.toZhuoYueDict, roleLoaderResult.MyGoodsData.ExcellenceInfo);
				}
				if (gameObject != null)
				{
					this.partsList.Add(gameObject);
				}
				else
				{
					gameObject = U3DUtils.LoadNudeModelByName(this.mData.DefaultPartNames[i]);
					if (gameObject != null)
					{
						this.partsList.Add(gameObject);
					}
				}
			}
			else
			{
				GameObject gameObject = U3DUtils.LoadNudeModelByName(this.mData.DefaultPartNames[i]);
				if (gameObject != null)
				{
					this.partsList.Add(gameObject);
				}
			}
		}
		if (this.skeleton == null)
		{
			this.skeleton = U3DUtils.LoadSkeletonByName(this.mData.SkeletonName, false);
		}
		GameObject gameObject2 = null;
		if (this.skeleton != null)
		{
			this.MergeRoleObject(this.mData.SkeletonName, this.skeleton, this.partsList, new Vector3(0f, 1f, 0f), 0.01f, 2f);
			this.skeleton.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = true;
			gameObject2 = this.skeleton;
			if (!this.mData.HideGameEffect)
			{
				U3DUtils.ReplaceMaterials(gameObject2, this.toReplaceDict);
			}
			else if (gameObject2.GetComponent<LoadRoleShaderAgain>() == null)
			{
				gameObject2.AddComponent<LoadRoleShaderAgain>();
			}
			if (this.mData.ReplaceChildLayer)
			{
				U3DUtils.ReplaceLayerInChildren(gameObject2, this.mData.ToLayer, null);
			}
		}
		if (this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, gameObject2);
		}
	}

	public void OnLoadBianShenComplete(string[] urls)
	{
		GameObject gameObject = MuAssetManager.Instance.InstantiateLeaderGameObject(urls[0]);
		try
		{
			Renderer[] componentsInChildren = gameObject.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
				{
					if (componentsInChildren[i].sharedMaterials[j] != null)
					{
						int renderQueue = componentsInChildren[i].sharedMaterials[j].renderQueue;
						componentsInChildren[i].sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterials[j].shader.name);
						componentsInChildren[i].sharedMaterials[j].renderQueue = ((renderQueue != 2000) ? renderQueue : -1);
					}
					else
					{
						MUDebug.LogError<string>(new string[]
						{
							"sharedMaterial is null : " + componentsInChildren[i].name
						});
					}
				}
			}
		}
		catch (Exception ex)
		{
			MUDebug.LogException(ex);
		}
		if (this.mOnComplete != null)
		{
			this.mOnComplete(this.mData, gameObject);
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopPreLoadLeader(this.requestID);
		if (this.partsList != null && this.partsList.Count > 0)
		{
			for (int i = this.partsList.Count - 1; i >= 0; i--)
			{
				GameObject gameObject = this.partsList[i];
				Object.Destroy(gameObject);
				this.partsList.Remove(gameObject);
			}
			this.partsList.Clear();
		}
		if (this.GoDict != null && this.GoDict.Count > 0)
		{
			this.GoDict.Clear();
		}
	}

	public void MergeRoleObject(string skeletonName, GameObject skeleton, List<GameObject> partsList, Vector3 ccCenter, float ccRadius, float ccHeight)
	{
		skeleton.AddComponent<SkinnedMeshRenderer>();
		MeshHelper.MergeMeshes(skeletonName, skeleton, partsList);
	}

	private OnLoadRoleComplete mOnComplete;

	private RoleLoaderData mData;

	private Dictionary<int, RoleLoaderResult> GoDict = new Dictionary<int, RoleLoaderResult>();

	private List<GameObject> partsList = new List<GameObject>();

	private List<string> bundleIDs;

	private GameObject skeleton;

	private int requestID;

	private Dictionary<string, int> toReplaceDict = new Dictionary<string, int>();

	private Dictionary<string, int> toZhuoYueDict = new Dictionary<string, int>();
}
