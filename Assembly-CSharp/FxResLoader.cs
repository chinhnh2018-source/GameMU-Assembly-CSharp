using System;
using System.Collections;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class FxResLoader
{
	public FxResLoader(FxResLoaderData data, OnLoadFxResComplete completeCallback)
	{
		if (data.isLimited && FxResLoader.PlayerMagicCount >= LGQualityManager.MaxSkillEffectCount)
		{
			return;
		}
		this.mData = data;
		this.mOnCompleted = completeCallback;
		this.URL = MuAssetManager.GetBundleID("Decoration", this.mData.assetName);
		MuAssetManager.Instance.BeginInstantiate(this.URL, new Action<GameObject>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
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
		if (null == go)
		{
			if (this.mOnCompleted != null)
			{
				this.mOnCompleted(this.mData, null);
			}
			return;
		}
		FxPlayController component = go.GetComponent<FxPlayController>();
		if (this.mData.isLimited)
		{
			FxResLoader.PlayerMagicCount++;
		}
		if (component != null)
		{
			component.isLimitedFx = this.mData.isLimited;
		}
		if (this.mData.parent)
		{
			if (!this.mData.parent.gameObject.active)
			{
				ManagedObject component2 = go.GetComponent<ManagedObject>();
				if (null != component2)
				{
					component2.ManualRelease();
					if (this.mOnCompleted != null)
					{
						this.mOnCompleted(this.mData, null);
					}
					return;
				}
			}
			go.transform.parent = this.mData.parent;
		}
		go.transform.localPosition = this.mData.localPosition;
		go.transform.localRotation = this.mData.localRotation;
		if (this.mData.autoSetEmptyParent)
		{
			go.transform.parent = null;
		}
		ManagedObject component3 = go.GetComponent<ManagedObject>();
		if (!(null != component3) || component3.isNewInstance)
		{
			this.OnInstantiateNewFx(go);
		}
		if (this.mOnCompleted != null)
		{
			this.mOnCompleted(this.mData, go);
		}
	}

	private void OnInstantiateNewFx(GameObject fx)
	{
		FxPlayController fxPlayController = fx.GetComponent<FxPlayController>();
		if (!fxPlayController)
		{
			fxPlayController = fx.AddComponent<FxPlayController>();
		}
		FxDelayActiveObj component = fx.GetComponent<FxDelayActiveObj>();
		if (component && component.toBeActiveGameObj == fx)
		{
			fxPlayController.duration = component.delayDisactiveTime;
			component.delayDisactiveTime = 0f;
		}
		Renderer[] componentsInChildren = fx.GetComponentsInChildren<Renderer>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			for (int j = 0; j < componentsInChildren[i].sharedMaterials.Length; j++)
			{
				if (componentsInChildren[i] != null && componentsInChildren[i].sharedMaterials != null && componentsInChildren[i].sharedMaterials[j] != null)
				{
					int renderQueue = componentsInChildren[i].sharedMaterials[j].renderQueue;
					componentsInChildren[i].sharedMaterials[j].shader = Shader.Find(componentsInChildren[i].sharedMaterials[j].shader.name);
					if (renderQueue > 3000)
					{
						componentsInChildren[i].sharedMaterials[j].renderQueue = renderQueue;
					}
				}
			}
		}
		NetAudioSource netAudioSource = fx.AddComponent<NetAudioSource>();
		if (!string.IsNullOrEmpty(this.mData.soundName))
		{
			netAudioSource.URL = "Audio/Deco/" + this.mData.soundName;
		}
	}

	public void Stop()
	{
		MuAssetManager.Instance.StopInstantiate(this.URL, new Action<GameObject>(this.LoadComplete));
	}

	public static int PlayerMagicCount;

	private IEnumerator mLoadCoroutine;

	private bool mStopped;

	private string URL;

	private FxResLoaderData mData;

	private OnLoadFxResComplete mOnCompleted;
}
