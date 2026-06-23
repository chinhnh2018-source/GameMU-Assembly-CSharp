using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

namespace HSGameEngine.GameEngine.AssetManagement
{
	public class MuAssetManager : ManualUpdateBehaviour
	{
		public static MuAssetManager Instance
		{
			get
			{
				return MuAssetManager.instance;
			}
		}

		private void Awake()
		{
			MuAssetManager.instance = this;
		}

		public void PreLoadResource(string bundleID, CacheType cacheResource = CacheType.CacheNotRelease)
		{
			CachedObject cachedObject;
			if (this.mCachedResources.TryGetValue(bundleID, ref cachedObject))
			{
				if (cacheResource == CacheType.CacheNotRelease)
				{
					cachedObject.isNotRelease = true;
				}
			}
			else
			{
				MuAssetManager.RequestBase requestBase;
				if (this.mAssetResquests.TryGetValue(bundleID, ref requestBase))
				{
					return;
				}
				MuAssetManager.GameObjectRequest gameObjectRequest = new MuAssetManager.GameObjectRequest(bundleID, CacheType.NotCache, cacheResource, CacheType.NotCache, 0);
				this.mRequestQueue.Enqueue(gameObjectRequest);
				this.mAssetResquests.Add(bundleID, gameObjectRequest);
			}
		}

		public void UnLoadPerResource(string url)
		{
			CachedObject cachedObject;
			if (this.mCachedResources.TryGetValue(url, ref cachedObject))
			{
				cachedObject.isNotRelease = false;
			}
		}

		public void BeginInstantiate(string bundleID, Action<GameObject> endInstantiate, CacheType cacheObject = CacheType.NotCache, CacheType cacheResource = CacheType.CacheAutoRelease, CacheType cacheBundle = CacheType.NotCache)
		{
			if (string.IsNullOrEmpty(bundleID))
			{
				if (endInstantiate != null)
				{
					endInstantiate.Invoke(null);
				}
				return;
			}
			ObjectPool objectPool;
			if (this.CachedPools.TryGetValue(bundleID, ref objectPool))
			{
				objectPool.Instantiate(endInstantiate, true, bundleID);
				return;
			}
			CachedObject res;
			if (this.mCachedResources.TryGetValue(bundleID, ref res))
			{
				GameObject gameObject = this.InstantiateGameObject(bundleID, res, 999, cacheObject, true);
				if (endInstantiate != null)
				{
					endInstantiate.Invoke(gameObject);
				}
				return;
			}
			MuAssetManager.RequestBase requestBase;
			if (this.mAssetResquests.TryGetValue(bundleID, ref requestBase))
			{
				MuAssetManager.GameObjectRequest gameObjectRequest = requestBase as MuAssetManager.GameObjectRequest;
				if (gameObjectRequest != null)
				{
					gameObjectRequest.AddInstantiateCallback(endInstantiate);
					gameObjectRequest.CacheObject = cacheObject;
					gameObjectRequest.CacheBundle = cacheBundle;
					gameObjectRequest.CacheRes = cacheResource;
				}
				return;
			}
			MuAssetManager.GameObjectRequest gameObjectRequest2 = new MuAssetManager.GameObjectRequest(bundleID, cacheBundle, cacheResource, cacheObject, 999);
			gameObjectRequest2.AddInstantiateCallback(endInstantiate);
			this.mRequestQueue.Enqueue(gameObjectRequest2);
			this.mAssetResquests.Add(bundleID, gameObjectRequest2);
		}

		public void StopInstantiate(string bundleID, Action<GameObject> endInstantiate)
		{
			if (string.IsNullOrEmpty(bundleID))
			{
				return;
			}
			MuAssetManager.RequestBase requestBase;
			if (this.mAssetResquests.TryGetValue(bundleID, ref requestBase))
			{
				MuAssetManager.GameObjectRequest gameObjectRequest = requestBase as MuAssetManager.GameObjectRequest;
				if (gameObjectRequest != null)
				{
					gameObjectRequest.RemoveInstantiateCallback(endInstantiate);
				}
			}
		}

		public int BeginInstantiate(string[] bundleIDs, Action<GameObject[]> endInstantiate, CacheType cacheObject = CacheType.NotCache, CacheType cacheResource = CacheType.CacheAutoRelease, CacheType cacheBundle = CacheType.NotCache)
		{
			MuAssetManager.GameObjectListRequest gameObjectListRequest = new MuAssetManager.GameObjectListRequest(bundleIDs, endInstantiate, new Action<MuAssetManager.GameObjectListRequest>(this.OnSingleAssetFromListLoaded), true, 999, cacheObject);
			this.mListRequests.Add(gameObjectListRequest.requestID, gameObjectListRequest);
			for (int i = 0; i < bundleIDs.Length; i++)
			{
				if (!string.IsNullOrEmpty(bundleIDs[i]))
				{
					ObjectPool objectPool;
					if (this.CachedPools.TryGetValue(bundleIDs[i], ref objectPool))
					{
						GameObject gameObject = objectPool.LockOne(true);
						if (null != gameObject)
						{
							gameObjectListRequest.OnEndLoaded(bundleIDs[i], gameObject);
							goto IL_13B;
						}
					}
					CachedObject cachedObject;
					MuAssetManager.RequestBase requestBase;
					if (this.mCachedResources.TryGetValue(bundleIDs[i], ref cachedObject))
					{
						cachedObject.Lock();
						gameObjectListRequest.OnEndLoaded(bundleIDs[i]);
					}
					else if (!this.mAssetResquests.TryGetValue(bundleIDs[i], ref requestBase))
					{
						MuAssetManager.GameObjectRequest gameObjectRequest = new MuAssetManager.GameObjectRequest(bundleIDs[i], cacheBundle, cacheResource, cacheObject, 999);
						this.mRequestQueue.Enqueue(gameObjectRequest);
						this.mAssetResquests.Add(bundleIDs[i], gameObjectRequest);
						gameObjectRequest.AddLoadCallback(new Action<string>(gameObjectListRequest.OnEndLoaded));
					}
					else
					{
						MuAssetManager.GameObjectRequest gameObjectRequest2 = requestBase as MuAssetManager.GameObjectRequest;
						if (gameObjectRequest2 != null)
						{
							gameObjectRequest2.CacheBundle = cacheBundle;
							gameObjectRequest2.CacheRes = cacheResource;
							gameObjectRequest2.AddLoadCallback(new Action<string>(gameObjectListRequest.OnEndLoaded));
						}
					}
				}
				IL_13B:;
			}
			return gameObjectListRequest.requestID;
		}

		public void StopInstantiate(int listRequstID)
		{
			MuAssetManager.GameObjectListRequest gameObjectListRequest;
			if (!this.mListRequests.TryGetValue(listRequstID, ref gameObjectListRequest))
			{
				return;
			}
			gameObjectListRequest.OnAllInstantiateFinish = null;
			gameObjectListRequest.OnSingleJobFinish = null;
		}

		private void OnSingleAssetFromListLoaded(MuAssetManager.GameObjectListRequest assetList)
		{
			GameObject[] array = new GameObject[assetList.mUrls.Length];
			for (int i = 0; i < assetList.mUrls.Length; i++)
			{
				CachedObject cachedObject;
				if (string.IsNullOrEmpty(assetList.mUrls[i].URL))
				{
					array[i] = null;
				}
				else if (null != assetList.mUrls[i].Object)
				{
					ObjectPool objectPool;
					if (this.CachedPools.TryGetValue(assetList.mUrls[i].URL, ref objectPool))
					{
						objectPool.UnLockOne(assetList.mUrls[i].Object);
						if (assetList.OnAllInstantiateFinish != null)
						{
							array[i] = objectPool.Instantiate(false, assetList.mUrls[i].URL);
						}
					}
				}
				else if (this.mCachedResources.TryGetValue(assetList.mUrls[i].URL, ref cachedObject))
				{
					cachedObject.Unlock();
					if (assetList.OnAllInstantiateFinish != null)
					{
						array[i] = this.InstantiateGameObject(assetList.mUrls[i].URL, cachedObject, assetList.mMaxActive, assetList.mcacheObject, false);
					}
				}
			}
			if (assetList.OnAllInstantiateFinish != null)
			{
				assetList.OnAllInstantiateFinish.Invoke(array);
			}
			this.mListRequests.Remove(assetList.requestID);
		}

		public int PreLoadLeader(string[] url, Action<string[]> endInstantiate)
		{
			MuAssetManager.GameObjectListRequest gameObjectListRequest = new MuAssetManager.GameObjectListRequest(url, endInstantiate, new Action<MuAssetManager.GameObjectListRequest>(this.OnLeaderLoaded));
			this.mListRequests.Add(gameObjectListRequest.requestID, gameObjectListRequest);
			for (int i = 0; i < url.Length; i++)
			{
				if (!string.IsNullOrEmpty(url[i]))
				{
					CachedObject cachedObject;
					MuAssetManager.RequestBase requestBase;
					if (this.mCachedResources.TryGetValue(url[i], ref cachedObject))
					{
						cachedObject.Lock();
						gameObjectListRequest.OnEndLoaded(url[i]);
					}
					else if (!this.mAssetResquests.TryGetValue(url[i], ref requestBase))
					{
						MuAssetManager.GameObjectRequest gameObjectRequest = new MuAssetManager.GameObjectRequest(url[i], CacheType.NotCache, CacheType.CacheAutoRelease, CacheType.NotCache, 0);
						this.mRequestQueue.Enqueue(gameObjectRequest);
						this.mAssetResquests.Add(url[i], gameObjectRequest);
						gameObjectRequest.AddLoadCallback(new Action<string>(gameObjectListRequest.OnEndLoaded));
					}
					else
					{
						MuAssetManager.GameObjectRequest gameObjectRequest2 = requestBase as MuAssetManager.GameObjectRequest;
						if (gameObjectRequest2 != null)
						{
							gameObjectRequest2.CacheBundle = CacheType.NotCache;
							gameObjectRequest2.CacheRes = CacheType.CacheAutoRelease;
							gameObjectRequest2.AddLoadCallback(new Action<string>(gameObjectListRequest.OnEndLoaded));
						}
					}
				}
			}
			return gameObjectListRequest.requestID;
		}

		public void StopPreLoadLeader(int listRequstID)
		{
			MuAssetManager.GameObjectListRequest gameObjectListRequest;
			if (!this.mListRequests.TryGetValue(listRequstID, ref gameObjectListRequest))
			{
				return;
			}
			gameObjectListRequest.OnAllInstantiateFinish = null;
			gameObjectListRequest.OnSingleJobFinish = null;
		}

		public GameObject InstantiateLeaderGameObject(string resID)
		{
			if (string.IsNullOrEmpty(resID))
			{
				return null;
			}
			CachedObject cachedObject;
			if (this.mCachedResources.TryGetValue(resID, ref cachedObject))
			{
				return (GameObject)Object.Instantiate(cachedObject.res);
			}
			return null;
		}

		private void OnLeaderLoaded(MuAssetManager.GameObjectListRequest assetList)
		{
			string[] array = new string[assetList.mUrls.Length];
			for (int i = 0; i < assetList.mUrls.Length; i++)
			{
				array[i] = assetList.mUrls[i].URL;
				if (!string.IsNullOrEmpty(assetList.mUrls[i].URL))
				{
					CachedObject cachedObject;
					if (this.mCachedResources.TryGetValue(assetList.mUrls[i].URL, ref cachedObject))
					{
						cachedObject.Unlock();
					}
				}
			}
			if (assetList.OnAllLoadFinish != null)
			{
				assetList.OnAllLoadFinish.Invoke(array);
			}
			this.mListRequests.Remove(assetList.requestID);
		}

		public ObjectPool GetObjectPool(string AssetID)
		{
			ObjectPool result = null;
			this.CachedPools.TryGetValue(AssetID, ref result);
			return result;
		}

		public bool AddToObjectPool(string AssetID, ObjectPool PoolObject)
		{
			ObjectPool objectPool = null;
			if (this.CachedPools.TryGetValue(AssetID, ref objectPool))
			{
				return false;
			}
			this.CachedPools.Add(AssetID, PoolObject);
			return true;
		}

		public void BeginLoadAsset(string url, Action<Object> endInstantiate, GameObject host)
		{
			if (!host || string.IsNullOrEmpty(url))
			{
				if (endInstantiate != null)
				{
					endInstantiate.Invoke(null);
				}
				return;
			}
			CachedObject res;
			if (this.mCachedResources.TryGetValue(url, ref res))
			{
				Object @object = this.InstantiateAsset(url, res, host);
				if (endInstantiate != null)
				{
					endInstantiate.Invoke(@object);
				}
				return;
			}
			MuAssetManager.RequestBase requestBase;
			if (this.mAssetResquests.TryGetValue(url, ref requestBase))
			{
				MuAssetManager.AssetRequest assetRequest = requestBase as MuAssetManager.AssetRequest;
				if (assetRequest != null)
				{
					assetRequest.AddInstantiateCallback(host, endInstantiate);
					assetRequest.CacheBundle = CacheType.NotCache;
					assetRequest.CacheRes = CacheType.CacheAutoRelease;
				}
				return;
			}
			MuAssetManager.AssetRequest assetRequest2 = new MuAssetManager.AssetRequest(url, CacheType.NotCache, CacheType.CacheAutoRelease);
			assetRequest2.AddInstantiateCallback(host, endInstantiate);
			this.mRequestQueue.Enqueue(assetRequest2);
			this.mAssetResquests.Add(url, assetRequest2);
		}

		public void StopLoadAsset(string url, Action<Object> endInstantiate)
		{
			if (string.IsNullOrEmpty(url))
			{
				return;
			}
			MuAssetManager.RequestBase requestBase;
			if (this.mAssetResquests.TryGetValue(url, ref requestBase))
			{
				MuAssetManager.AssetRequest assetRequest = requestBase as MuAssetManager.AssetRequest;
				if (assetRequest != null)
				{
					assetRequest.RemoveInstantiateCallback(endInstantiate);
				}
			}
		}

		public void AddPermenentSharedAssets(Object[] assets)
		{
			for (int i = 0; i < assets.Length; i++)
			{
				this.PermenentSharedAssets.Add(assets[i], 0);
			}
		}

		public void AddSharedAssets(Object[] assets)
		{
			foreach (Object @object in assets)
			{
				if (@object is Mesh || @object is Texture)
				{
					if (this.SharedAssets.ContainsKey(@object))
					{
						Dictionary<Object, int> sharedAssets;
						Dictionary<Object, int> dictionary = sharedAssets = this.SharedAssets;
						Object object3;
						Object object2 = object3 = @object;
						int num = sharedAssets[object3];
						dictionary[object2] = num + 1;
					}
					else
					{
						this.SharedAssets.Add(@object, 1);
					}
				}
			}
		}

		private void ReleaseResource(Object obj)
		{
			if (null == obj)
			{
				return;
			}
			if (typeof(GameObject) == obj.GetType())
			{
				SkinnedMeshRenderer[] componentsInChildren = ((GameObject)obj).gameObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					this.ReleaseAssets(componentsInChildren[i].sharedMesh);
					if (componentsInChildren[i].sharedMaterials != null)
					{
						this.ReleaseMaterial(componentsInChildren[i].sharedMaterials);
					}
					this.ReleaseAssets(componentsInChildren[i]);
				}
				MeshRenderer[] componentsInChildren2 = ((GameObject)obj).gameObject.GetComponentsInChildren<MeshRenderer>(true);
				for (int j = 0; j < componentsInChildren2.Length; j++)
				{
					if (componentsInChildren2[j].sharedMaterials != null)
					{
						this.ReleaseMaterial(componentsInChildren2[j].sharedMaterials);
					}
					this.ReleaseAssets(componentsInChildren2[j]);
				}
				MeshFilter[] componentsInChildren3 = ((GameObject)obj).gameObject.GetComponentsInChildren<MeshFilter>(true);
				for (int k = 0; k < componentsInChildren3.Length; k++)
				{
					this.ReleaseAssets(componentsInChildren3[k].sharedMesh);
				}
				Renderer[] componentsInChildren4 = ((GameObject)obj).gameObject.GetComponentsInChildren<Renderer>(true);
				for (int l = 0; l < componentsInChildren4.Length; l++)
				{
					this.ReleaseMaterial(componentsInChildren4[l].sharedMaterials);
				}
				Animator[] componentsInChildren5 = ((GameObject)obj).gameObject.GetComponentsInChildren<Animator>(true);
				for (int m = 0; m < componentsInChildren5.Length; m++)
				{
					if (!(null == componentsInChildren5[m].runtimeAnimatorController))
					{
						if (componentsInChildren5[m].runtimeAnimatorController.animationClips == null)
						{
							this.ReleaseAssets(componentsInChildren5[m].runtimeAnimatorController);
						}
						else
						{
							AnimationClip[] animationClips = componentsInChildren5[m].runtimeAnimatorController.animationClips;
							for (int n = 0; n < animationClips.Length; n++)
							{
								this.ReleaseAssets(animationClips[n]);
							}
							this.ReleaseAssets(componentsInChildren5[m].runtimeAnimatorController);
							this.ReleaseAssets(componentsInChildren5[m]);
						}
					}
				}
				Animation[] componentsInChildren6 = ((GameObject)obj).gameObject.GetComponentsInChildren<Animation>(true);
				for (int num = 0; num < componentsInChildren6.Length; num++)
				{
					this.ReleaseAssets(componentsInChildren6[num]);
				}
				ParticleSystem[] componentsInChildren7 = ((GameObject)obj).gameObject.GetComponentsInChildren<ParticleSystem>(true);
				for (int num2 = 0; num2 < componentsInChildren7.Length; num2++)
				{
					this.ReleaseAssets(componentsInChildren7[num2]);
				}
			}
		}

		private void ReleaseMaterial(Material[] m)
		{
			if (m == null)
			{
				return;
			}
			for (int i = 0; i < m.Length; i++)
			{
				if (null != m[i])
				{
					if (m[i].shader.name.Contains("PlayerCharacter"))
					{
						this.ReleaseAssets(m[i].mainTexture);
						this.ReleaseAssets(m[i].GetTexture("_MaskTex"));
						this.ReleaseAssets(m[i].GetTexture("_EmissionTex"));
						this.ReleaseAssets(m[i].GetTexture("_EmissionMask"));
						this.ReleaseAssets(m[i].GetTexture("_Cube"));
					}
					else if (m[i].shader.name.Contains("Tint Particle"))
					{
						this.ReleaseAssets(m[i].mainTexture);
						this.ReleaseAssets(m[i].GetTexture("_Mask"));
						this.ReleaseAssets(m[i].GetTexture("_DissolveTex"));
					}
					else if (m[i].shader.name.Contains("Mobile Specular Standard"))
					{
						this.ReleaseAssets(m[i].mainTexture);
						this.ReleaseAssets(m[i].GetTexture("_SpecGlossMap"));
						this.ReleaseAssets(m[i].GetTexture("_BumpMap"));
					}
					else if (m[i].shader.name.Contains("Distort-Normal"))
					{
						this.ReleaseAssets(m[i].GetTexture("_MainTex"));
						this.ReleaseAssets(m[i].GetTexture("_BumpMap"));
					}
					else if (m[i].shader.name.Contains("Artist/Effect/Distort-Noise"))
					{
						this.ReleaseAssets(m[i].GetTexture("_NoiseTex"));
					}
					else
					{
						if (!m[i].shader.name.Contains("Artist/NPCCharacter"))
						{
							MUDebug.LogError<string>(new string[]
							{
								"Unhandled Shader Asset Release : " + m[i].shader.name
							});
						}
						this.ReleaseAssets(m[i].mainTexture);
					}
				}
				this.ReleaseAssets(m[i]);
			}
		}

		private void ReleaseAssets(Object asset)
		{
			if (!asset)
			{
				return;
			}
			if (this.PermenentSharedAssets.ContainsKey(asset))
			{
				return;
			}
			if (this.SharedAssets.ContainsKey(asset))
			{
				Dictionary<Object, int> sharedAssets;
				Dictionary<Object, int> dictionary = sharedAssets = this.SharedAssets;
				Object object2;
				Object @object = object2 = asset;
				int num = sharedAssets[object2];
				dictionary[@object] = num - 1;
				if (this.SharedAssets[asset] <= 0)
				{
					this.SharedAssets.Remove(asset);
					Object.DestroyImmediate(asset, true);
					asset = null;
				}
			}
			else
			{
				Object.DestroyImmediate(asset, true);
				asset = null;
			}
		}

		private void ClearnUnusePoolObject()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ObjectPool> keyValuePair in this.CachedPools)
			{
				ObjectPool value = keyValuePair.Value;
				if (value.ActiveCount <= 0 && value.UnloadTime <= Time.time && !value.isNotUnload)
				{
					List<string> list2 = list;
					Dictionary<string, ObjectPool>.Enumerator enumerator;
					KeyValuePair<string, ObjectPool> keyValuePair2 = enumerator.Current;
					list2.Add(keyValuePair2.Key);
					value.ClearPool();
				}
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.CachedPools.Remove(list[i]);
				}
				list.Clear();
			}
		}

		private bool CleanUnusedResource()
		{
			foreach (KeyValuePair<string, CachedObject> keyValuePair in this.mCachedResources)
			{
				CachedObject value = keyValuePair.Value;
				if (value.refCount <= 0 && value.unloadTime <= Time.time && !value.isNotRelease)
				{
					value.res = null;
					List<string> list = this.mDeleteKeys;
					Dictionary<string, CachedObject>.Enumerator enumerator;
					KeyValuePair<string, CachedObject> keyValuePair2 = enumerator.Current;
					list.Add(keyValuePair2.Key);
				}
			}
			if (this.mDeleteKeys.Count > 0)
			{
				for (int i = 0; i < this.mDeleteKeys.Count; i++)
				{
					CachedObject cachedObject = null;
					if (this.mCachedResources.TryGetValue(this.mDeleteKeys[i], ref cachedObject))
					{
						this.ReleaseResource(cachedObject.res);
					}
					this.mCachedResources.Remove(this.mDeleteKeys[i]);
				}
				this.mDeleteKeys.Clear();
				return true;
			}
			return false;
		}

		private IEnumerator UnloadUnusedResource()
		{
			if (this.CleanUnusedResource())
			{
				yield return Resources.UnloadUnusedAssets();
			}
			this.mUnloadTime = Time.time + MuAssetManager.UnloadResInterval;
			yield break;
		}

		public void UnLoadAllUnuseResource()
		{
			List<string> list = new List<string>();
			foreach (KeyValuePair<string, ObjectPool> keyValuePair in this.CachedPools)
			{
				ObjectPool value = keyValuePair.Value;
				if (value.ActiveCount <= 0)
				{
					List<string> list2 = list;
					Dictionary<string, ObjectPool>.Enumerator enumerator;
					KeyValuePair<string, ObjectPool> keyValuePair2 = enumerator.Current;
					list2.Add(keyValuePair2.Key);
					value.ClearPool();
				}
				else
				{
					value.ForceClearDisactiveList();
				}
			}
			if (list.Count > 0)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.CachedPools.Remove(list[i]);
				}
				list.Clear();
			}
			if (this.CleanUnusedResource())
			{
				Resources.UnloadUnusedAssets();
			}
		}

		private IEnumerator DoAssetRequest(MuAssetManager.RequestBase request)
		{
			Object resource = null;
			MuAssetManager.SharedBundle sharedBundle = null;
			CDNAssetBundle cdnSharedBundle = null;
			WaitForCDNAsset cdnRequest = null;
			CachedObject cachedRes;
			if (this.mCachedResources.TryGetValue(request.BundleID, ref cachedRes))
			{
				resource = cachedRes.res;
			}
			else
			{
				bool loadSharedBundleSucces = true;
				if (!string.IsNullOrEmpty(request.SharedBundleID))
				{
					if (this.mSharedAssetbundles.TryGetValue(request.SharedBundleID, ref sharedBundle))
					{
						sharedBundle.reference++;
					}
					else
					{
						string sharedPath = request.SharedBundleID + ".unity3d";
						string sharedURL = PathUtils.WebPath(sharedPath);
						WWW sharedWWW = new WWW(sharedURL);
						yield return sharedWWW;
						if (this.mSharedAssetbundles.TryGetValue(request.SharedBundleID, ref sharedBundle))
						{
							sharedBundle.reference++;
						}
						else if (!string.IsNullOrEmpty(sharedWWW.error))
						{
							WaitForCDNAsset cdnRequestShar = new WaitForCDNAsset(sharedPath);
							yield return base.StartCoroutine(cdnRequestShar);
							if (!string.IsNullOrEmpty(cdnRequestShar.error) || !cdnRequestShar.assetBundle)
							{
								MUDebug.LogError<string>(new string[]
								{
									cdnRequestShar.error + "  sharedPath " + sharedPath
								});
								loadSharedBundleSucces = false;
							}
							else
							{
								cdnSharedBundle = cdnRequestShar.cdnAssetBundle;
								sharedBundle = new MuAssetManager.SharedBundle
								{
									id = request.SharedBundleID,
									bundle = cdnRequestShar.assetBundle,
									reference = 1
								};
								this.mSharedAssetbundles.Add(request.SharedBundleID, sharedBundle);
								this.AddSharedAssets(sharedBundle.bundle.LoadAllAssets());
							}
						}
						else
						{
							sharedBundle = new MuAssetManager.SharedBundle
							{
								id = request.SharedBundleID,
								bundle = sharedWWW.assetBundle,
								reference = 1
							};
							this.mSharedAssetbundles.Add(request.SharedBundleID, sharedBundle);
							this.AddSharedAssets(sharedBundle.bundle.LoadAllAssets());
						}
					}
				}
				if (loadSharedBundleSucces)
				{
					MuAssetManager.CachedBundle bundleData;
					if (this.mCachedBundles.TryGetValue(request.BundleID, ref bundleData) && bundleData.bundle != null)
					{
						bundleData.Lock();
						AssetBundleRequest abr = bundleData.bundle.LoadAssetAsync(request.AssetName, typeof(Object));
						yield return abr;
						bundleData.Unlock();
						resource = abr.asset;
					}
					else
					{
						string path = request.BundleID + ".unity3d";
						string url = PathUtils.WebPath(path);
						WWW www = new WWW(url);
						yield return www;
						if (string.IsNullOrEmpty(www.error))
						{
							AssetBundle bundle = www.assetBundle;
							bundleData = new MuAssetManager.CachedBundle
							{
								bundle = bundle,
								isCached = request.CacheBundle
							};
							this.mCachedBundles.Add(request.BundleID, bundleData);
							bundleData.Lock();
							AssetBundleRequest abr2 = bundle.LoadAssetAsync(request.AssetName, typeof(Object));
							yield return abr2;
							bundleData.Unlock();
							resource = abr2.asset;
						}
						else
						{
							cdnRequest = new WaitForCDNAsset(path);
							yield return base.StartCoroutine(cdnRequest);
							if (!string.IsNullOrEmpty(cdnRequest.error) || !cdnRequest.assetBundle)
							{
								MUDebug.LogError<string>(new string[]
								{
									cdnRequest.error + "  path " + path
								});
								cdnRequest = null;
							}
							else
							{
								AssetBundle bundle2 = cdnRequest.assetBundle;
								bundle2 = cdnRequest.assetBundle;
								bundleData = new MuAssetManager.CachedBundle
								{
									bundle = bundle2,
									isCached = request.CacheBundle
								};
								this.mCachedBundles.Add(request.BundleID, bundleData);
								bundleData.Lock();
								AssetBundleRequest abr3 = bundle2.LoadAssetAsync(request.AssetName, typeof(Object));
								yield return abr3;
								bundleData.Unlock();
								resource = abr3.asset;
							}
						}
					}
				}
			}
			if (cachedRes == null && resource)
			{
				cachedRes = new CachedObject
				{
					res = resource,
					refCount = 0,
					unloadTime = Time.time + 1f,
					isCached = request.CacheRes
				};
				this.mCachedResources.Add(request.BundleID, cachedRes);
			}
			this.mAssetResquests.Remove(request.BundleID);
			this.mActiveResquests.Remove(request.BundleID);
			try
			{
				MuAssetManager.GameObjectRequest gr = request as MuAssetManager.GameObjectRequest;
				MuAssetManager.AssetRequest ar = request as MuAssetManager.AssetRequest;
				if (gr != null)
				{
					List<Action<GameObject>>.Enumerator itr = gr.InstantiateItr;
					while (itr.MoveNext())
					{
						GameObject g = null;
						if (cachedRes != null)
						{
							g = this.InstantiateGameObject(gr.BundleID, cachedRes, gr.MaxActive, gr.CacheObject, false);
						}
						itr.Current.Invoke(g);
					}
					List<Action<string>>.Enumerator loaderItr = gr.LoadItr;
					while (loaderItr.MoveNext())
					{
						if (cachedRes != null)
						{
							cachedRes.Lock();
						}
						loaderItr.Current.Invoke(gr.BundleID);
					}
				}
				else
				{
					Dictionary<Action<Object>, GameObject>.Enumerator itr2 = ar.InstantiateItr;
					while (itr2.MoveNext())
					{
						Object asset = null;
						if (cachedRes != null)
						{
							string bundleID = ar.BundleID;
							CachedObject res = cachedRes;
							KeyValuePair<Action<Object>, GameObject> keyValuePair = itr2.Current;
							asset = this.InstantiateAsset(bundleID, res, keyValuePair.Value);
						}
						KeyValuePair<Action<Object>, GameObject> keyValuePair2 = itr2.Current;
						keyValuePair2.Key.Invoke(asset);
					}
				}
			}
			catch (Exception ex2)
			{
				Exception ex = ex2;
				MUDebug.LogException(ex);
			}
			finally
			{
				if (cdnRequest != null && cdnRequest.cdnAssetBundle != null)
				{
					cdnRequest.cdnAssetBundle.Release();
				}
				if (sharedBundle != null)
				{
					sharedBundle.reference--;
					if (sharedBundle.reference <= 0)
					{
						if (cdnSharedBundle != null)
						{
							cdnSharedBundle.Release();
						}
						this.mSharedAssetbundles.Remove(sharedBundle.id);
						if (sharedBundle.bundle)
						{
							sharedBundle.bundle.Unload(false);
						}
					}
				}
			}
			yield break;
		}

		public override void ManualUpdate()
		{
			while (this.mRequestQueue.Count > 0 && this.mActiveResquests.Count < MuAssetManager.MaxConcurrency)
			{
				MuAssetManager.RequestBase requestBase = this.mRequestQueue.Dequeue();
				this.mActiveResquests.Add(requestBase.BundleID, requestBase);
				base.StartCoroutine(this.DoAssetRequest(requestBase));
			}
			foreach (KeyValuePair<string, MuAssetManager.CachedBundle> keyValuePair in this.mCachedBundles)
			{
				MuAssetManager.CachedBundle value = keyValuePair.Value;
				if (value.refCount <= 0 && value.unloadTime < Time.time)
				{
					if (value.bundle != null)
					{
						value.bundle.Unload(false);
					}
					List<string> list = this.mDeleteKeys;
					Dictionary<string, MuAssetManager.CachedBundle>.Enumerator enumerator;
					KeyValuePair<string, MuAssetManager.CachedBundle> keyValuePair2 = enumerator.Current;
					list.Add(keyValuePair2.Key);
				}
			}
			if (this.mDeleteKeys.Count > 0)
			{
				for (int i = 0; i < this.mDeleteKeys.Count; i++)
				{
					this.mCachedBundles.Remove(this.mDeleteKeys[i]);
				}
				this.mDeleteKeys.Clear();
			}
			if (this.mUnloadObjectTime < Time.time)
			{
				this.mUnloadObjectTime = Time.time + MuAssetManager.ObjectCacheTime;
				this.ClearnUnusePoolObject();
				this.CleanUnusedResource();
			}
			if (this.mUnloadTime < Time.time && this.mActiveResquests.Count == 0)
			{
				this.mUnloadTime = Time.time + MuAssetManager.CoroutineSafeLockTime;
				Resources.UnloadUnusedAssets();
			}
		}

		private GameObject InstantiateGameObject(string resID, CachedObject res, int maxActive, CacheType cacheObject = CacheType.CacheAutoRelease, bool force = false)
		{
			ObjectPool objectPool = null;
			GameObject gameObject;
			if (cacheObject != CacheType.NotCache)
			{
				if (!this.CachedPools.TryGetValue(resID, ref objectPool))
				{
					objectPool = new ObjectPool(res, maxActive);
					if (cacheObject == CacheType.CacheNotRelease)
					{
						objectPool.isNotUnload = true;
					}
					this.CachedPools.Add(resID, objectPool);
				}
				gameObject = objectPool.Instantiate(force, resID);
			}
			else
			{
				res.Lock();
				gameObject = (Object.Instantiate(res.res) as GameObject);
				ManagedObject managedObject = gameObject.AddComponent<ManagedObject>();
				managedObject.instanceID = gameObject.GetInstanceID();
				managedObject.resURL = resID;
				managedObject.onDestroyed = new Action<ManagedObject>(res.OnResInstanceDestroyed);
			}
			return gameObject;
		}

		private Object InstantiateAsset(string resID, CachedObject res, GameObject host)
		{
			if (!host)
			{
				return null;
			}
			res.Lock();
			Object res2 = res.res;
			MuAsset muAsset = host.GetComponent<MuAsset>();
			if (null == muAsset)
			{
				muAsset = host.AddComponent<MuAsset>();
				muAsset.onDeleteOne = new Action<string>(this.OnAssetResDestroyOne);
				muAsset.onDestroy = new Action<List<string>>(this.OnAssetResDestroy);
			}
			muAsset.assetIDs.Add(resID);
			return res2;
		}

		private void OnAssetResDestroyOne(string assetID)
		{
			if (string.IsNullOrEmpty(assetID))
			{
				return;
			}
			CachedObject cachedObject;
			if (this.mCachedResources.TryGetValue(assetID, ref cachedObject))
			{
				cachedObject.Unlock();
				return;
			}
		}

		private void OnAssetResDestroy(List<string> assetIDs)
		{
			if (assetIDs == null)
			{
				return;
			}
			for (int i = 0; i < assetIDs.Count; i++)
			{
				CachedObject cachedObject;
				if (this.mCachedResources.TryGetValue(assetIDs[i], ref cachedObject))
				{
					cachedObject.Unlock();
					return;
				}
			}
		}

		private static string StreamingAssetPath
		{
			get
			{
				if (MuAssetManager.streamingAssetPath == null)
				{
					MuAssetManager.streamingAssetPath = Application.streamingAssetsPath + "/";
				}
				return MuAssetManager.streamingAssetPath;
			}
		}

		public static string GetUrl(string bundleID)
		{
			return new StringBuilder(MuAssetManager.StreamingAssetPath, 128).Append(bundleID).Append(".unity3d").ToString();
		}

		public static string GetAssetNameFromURL(string url)
		{
			int num = url.LastIndexOf('/');
			int num2 = url.LastIndexOf('.');
			if (num2 < 0)
			{
				num2 = url.Length;
			}
			return url.Substring(num + 1, num2 - num - 1);
		}

		public static string GetBundleID(string folder, string name)
		{
			if (name.EndsWith(".unity3d"))
			{
				if (folder.EndsWith("/"))
				{
					return new StringBuilder(folder, 32).Append(name, 0, name.Length - 8).ToString();
				}
				return new StringBuilder(folder, 32).Append('/').Append(name, 0, name.Length - 8).ToString();
			}
			else
			{
				if (folder.EndsWith("/"))
				{
					return new StringBuilder(folder, 32).Append(name).ToString();
				}
				return new StringBuilder(folder, 32).Append('/').Append(name).ToString();
			}
		}

		private static readonly int MaxConcurrency = 3;

		private static readonly float CoroutineSafeLockTime = 120f;

		private static readonly float CachedTime = 10f;

		private static readonly float UnloadResInterval = 20f;

		private static readonly float ObjectCacheTime = 10f;

		private static MuAssetManager instance;

		private Dictionary<string, MuAssetManager.RequestBase> mAssetResquests = new Dictionary<string, MuAssetManager.RequestBase>();

		private Dictionary<string, MuAssetManager.RequestBase> mActiveResquests = new Dictionary<string, MuAssetManager.RequestBase>();

		private Queue<MuAssetManager.RequestBase> mRequestQueue = new Queue<MuAssetManager.RequestBase>();

		private Dictionary<int, MuAssetManager.GameObjectListRequest> mListRequests = new Dictionary<int, MuAssetManager.GameObjectListRequest>();

		private Dictionary<string, MuAssetManager.CachedBundle> mCachedBundles = new Dictionary<string, MuAssetManager.CachedBundle>();

		private Dictionary<string, MuAssetManager.SharedBundle> mSharedAssetbundles = new Dictionary<string, MuAssetManager.SharedBundle>();

		private Dictionary<string, CachedObject> mCachedResources = new Dictionary<string, CachedObject>();

		private Dictionary<string, ObjectPool> CachedPools = new Dictionary<string, ObjectPool>();

		private List<string> mDeleteKeys = new List<string>();

		private float mUnloadTime;

		private float mUnloadObjectTime;

		private Dictionary<Object, int> PermenentSharedAssets = new Dictionary<Object, int>();

		private Dictionary<Object, int> SharedAssets = new Dictionary<Object, int>();

		private static string streamingAssetPath;

		private class CachedBundle
		{
			public void Lock()
			{
				this.refCount++;
			}

			public void Unlock()
			{
				this.refCount--;
				if (this.refCount <= 0)
				{
					if (this.isCached == CacheType.NotCache)
					{
						this.unloadTime = Time.time + 1f;
					}
					else
					{
						this.unloadTime = Time.time + MuAssetManager.CachedTime;
					}
				}
			}

			public AssetBundle bundle;

			public float unloadTime;

			public int refCount;

			public CacheType isCached;
		}

		private class SharedBundle
		{
			public string id;

			public AssetBundle bundle;

			public int reference;
		}

		private class RequestBase
		{
			protected RequestBase(string bundleID, CacheType cacheBundle, CacheType cacheRes, CacheType cacheObject, int maxActive)
			{
				this.mBundleID = bundleID;
				this.mIscacheObject = cacheObject;
				this.mIsCacheBundle = cacheBundle;
				this.mIsCacheRes = cacheRes;
				this.mMaxActive = maxActive;
			}

			public string BundleID
			{
				get
				{
					return this.mBundleID;
				}
			}

			public string SharedBundleID
			{
				get
				{
					return this.mSharedBundleID;
				}
			}

			public string AssetName
			{
				get
				{
					return this.mAssetName;
				}
			}

			public CacheType CacheObject
			{
				get
				{
					return this.mIscacheObject;
				}
				set
				{
					this.mIscacheObject = value;
				}
			}

			public CacheType CacheBundle
			{
				get
				{
					return this.mIsCacheBundle;
				}
				set
				{
					this.mIsCacheBundle = value;
				}
			}

			public CacheType CacheRes
			{
				get
				{
					return this.mIsCacheRes;
				}
				set
				{
					this.mIsCacheRes = value;
				}
			}

			public int MaxActive
			{
				get
				{
					return this.mMaxActive;
				}
				set
				{
					this.mMaxActive = value;
				}
			}

			private string mBundleID;

			protected string mSharedBundleID;

			protected string mAssetName;

			private CacheType mIscacheObject;

			private CacheType mIsCacheBundle;

			private CacheType mIsCacheRes;

			private int mMaxActive;
		}

		private class GameObjectRequest : MuAssetManager.RequestBase
		{
			public GameObjectRequest(string bundleID, CacheType cacheBundle, CacheType cacheRes, CacheType cacheObject, int maxActive) : base(bundleID, cacheBundle, cacheRes, cacheObject, maxActive)
			{
				if (string.IsNullOrEmpty(bundleID))
				{
					return;
				}
				this.mAssetName = MuAssetManager.GetAssetNameFromURL(bundleID);
				string text;
				if (ConfigBundleDependencies.TryGetDependencies(this.mAssetName, out text))
				{
					this.mSharedBundleID = bundleID.Replace(this.mAssetName, text);
				}
			}

			public List<Action<GameObject>>.Enumerator InstantiateItr
			{
				get
				{
					return this.instantiateCallbacks.GetEnumerator();
				}
			}

			public List<Action<string>>.Enumerator LoadItr
			{
				get
				{
					return this.loadCallbacks.GetEnumerator();
				}
			}

			public void AddInstantiateCallback(Action<GameObject> callback)
			{
				this.instantiateCallbacks.Add(callback);
			}

			public void RemoveInstantiateCallback(Action<GameObject> callback)
			{
				this.instantiateCallbacks.Remove(callback);
			}

			public void AddLoadCallback(Action<string> callback)
			{
				this.loadCallbacks.Add(callback);
			}

			public void RemoveLoadCallback(Action<string> callback)
			{
				this.loadCallbacks.Remove(callback);
			}

			private List<Action<GameObject>> instantiateCallbacks = new List<Action<GameObject>>();

			private List<Action<string>> loadCallbacks = new List<Action<string>>();
		}

		private class AssetRequest : MuAssetManager.RequestBase
		{
			public AssetRequest(string bundleID, CacheType cacheBundle, CacheType cacheRes) : base(bundleID, cacheBundle, cacheRes, CacheType.NotCache, 0)
			{
				this.mAssetName = MuAssetManager.GetAssetNameFromURL(bundleID);
			}

			public Dictionary<Action<Object>, GameObject>.Enumerator InstantiateItr
			{
				get
				{
					return this.instantiateCallbacks.GetEnumerator();
				}
			}

			public void AddInstantiateCallback(GameObject host, Action<Object> callback)
			{
				this.instantiateCallbacks.Add(callback, host);
			}

			public void RemoveInstantiateCallback(Action<Object> callback)
			{
				this.instantiateCallbacks.Remove(callback);
			}

			private Dictionary<Action<Object>, GameObject> instantiateCallbacks = new Dictionary<Action<Object>, GameObject>();
		}

		private class GameObjectListRequest
		{
			public GameObjectListRequest(string[] urls, Action<GameObject[]> onAllRequestFinish, Action<MuAssetManager.GameObjectListRequest> onSingleJobFinish, bool force = false, int MaxActive = 5, CacheType cacheObject = CacheType.CacheAutoRelease)
			{
				this.requestID = MuAssetManager.GameObjectListRequest.RequestID++;
				this.mforce = force;
				this.mMaxActive = MaxActive;
				this.mcacheObject = cacheObject;
				this.mUrls = new MuAssetManager.GameObjectListRequest.RequestObj[urls.Length];
				this.remainRequestCount = urls.Length;
				this.OnAllInstantiateFinish = onAllRequestFinish;
				this.OnSingleJobFinish = onSingleJobFinish;
				for (int i = 0; i < urls.Length; i++)
				{
					if (string.IsNullOrEmpty(urls[i]))
					{
						this.remainRequestCount--;
					}
					this.mUrls[i].URL = urls[i];
					this.mUrls[i].Object = null;
				}
				if (this.remainRequestCount <= 0 && this.OnSingleJobFinish != null)
				{
					this.OnSingleJobFinish.Invoke(this);
				}
			}

			public GameObjectListRequest(string[] urls, Action<string[]> onAllRequestFinish, Action<MuAssetManager.GameObjectListRequest> onSingleJobFinish)
			{
				this.requestID = MuAssetManager.GameObjectListRequest.RequestID++;
				this.mUrls = new MuAssetManager.GameObjectListRequest.RequestObj[urls.Length];
				this.remainRequestCount = urls.Length;
				this.OnAllLoadFinish = onAllRequestFinish;
				this.OnSingleJobFinish = onSingleJobFinish;
				for (int i = 0; i < urls.Length; i++)
				{
					if (string.IsNullOrEmpty(urls[i]))
					{
						this.remainRequestCount--;
					}
					this.mUrls[i].URL = urls[i];
					this.mUrls[i].Object = null;
				}
				if (this.remainRequestCount <= 0 && this.OnSingleJobFinish != null)
				{
					this.OnSingleJobFinish.Invoke(this);
				}
			}

			public void OnEndLoaded(string url, GameObject go)
			{
				if (null == go || string.IsNullOrEmpty(url))
				{
					return;
				}
				for (int i = 0; i < this.mUrls.Length; i++)
				{
					if (!string.IsNullOrEmpty(this.mUrls[i].URL) && this.mUrls[i].URL.Equals(url) && this.mUrls[i].Object == null)
					{
						this.mUrls[i].Object = go;
						this.remainRequestCount--;
						break;
					}
				}
				if (this.remainRequestCount > 0)
				{
					return;
				}
				if (this.OnSingleJobFinish != null)
				{
					this.OnSingleJobFinish.Invoke(this);
				}
			}

			public void OnEndLoaded(string url)
			{
				if (string.IsNullOrEmpty(url))
				{
					return;
				}
				for (int i = 0; i < this.mUrls.Length; i++)
				{
					if (!string.IsNullOrEmpty(this.mUrls[i].URL) && this.mUrls[i].URL.Equals(url))
					{
						this.remainRequestCount--;
						break;
					}
				}
				if (this.remainRequestCount > 0)
				{
					return;
				}
				if (this.OnSingleJobFinish != null)
				{
					this.OnSingleJobFinish.Invoke(this);
				}
			}

			private static int RequestID;

			public int requestID;

			public bool mforce;

			public int mMaxActive;

			public CacheType mcacheObject;

			public MuAssetManager.GameObjectListRequest.RequestObj[] mUrls;

			public Action<GameObject[]> OnAllInstantiateFinish;

			public Action<string[]> OnAllLoadFinish;

			public Action<MuAssetManager.GameObjectListRequest> OnSingleJobFinish;

			private int remainRequestCount;

			public struct RequestObj
			{
				public string URL;

				public GameObject Object;
			}
		}
	}
}
