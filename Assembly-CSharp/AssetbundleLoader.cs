using System;
using HSGameEngine.GameEngine.AssetManagement;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class AssetbundleLoader : TTMonoBehaviour
{
	private void CoroutineException()
	{
		if (this.Complete != null)
		{
			this.Complete(this, null);
		}
		if (this.AutoDestroySelf)
		{
			Object.Destroy(this);
		}
	}

	private void Start()
	{
		MuAssetManager.Instance.BeginInstantiate(this.BundleID, new Action<GameObject>(this.LoadComplete), CacheType.CacheAutoRelease, CacheType.CacheAutoRelease, CacheType.NotCache);
	}

	private void LoadComplete(GameObject obj)
	{
		if (string.IsNullOrEmpty(this.BundleID))
		{
			if (this.Complete != null)
			{
				this.Complete(this, null);
			}
			if (this.LoadFail != null)
			{
				this.LoadFail(this, null);
			}
			return;
		}
		if (null != obj)
		{
			if (!string.IsNullOrEmpty(this.OwnerName) || this.TriggerType >= 0)
			{
				U3DUtils.SetEffectManagerParams(obj, this.OwnerName, this.TriggerType);
			}
			if (this == null)
			{
				ManagedObject component = obj.GetComponent<ManagedObject>();
				if (component)
				{
					component.ManualRelease();
				}
				else
				{
					Object.Destroy(obj);
				}
				return;
			}
			if (this.Complete != null)
			{
				this.Complete(this, obj);
				if (this.AutoDestroySelf)
				{
					Object.Destroy(this);
				}
			}
			else
			{
				Transform transform = obj.transform;
				ManagedObject component2 = obj.GetComponent<ManagedObject>();
				Vector3 localPosition;
				Quaternion localRotation;
				Vector3 localScale;
				if (null != component2)
				{
					localPosition = component2.InitPos;
					localRotation = component2.InitRotation;
					localScale = component2.InitScale;
				}
				else
				{
					localPosition = transform.localPosition;
					localRotation = transform.localRotation;
					localScale = transform.localScale;
				}
				if (transform.parent == null)
				{
					transform.parent = base.transform;
				}
				transform.localPosition = localPosition;
				transform.localRotation = localRotation;
				transform.localScale = localScale;
				if (this.ToLayer >= 0)
				{
					U3DUtils.ReplaceLayerInChildren(obj, this.ToLayer, null);
				}
				else
				{
					U3DUtils.ReplaceLayerInChildren(obj, base.gameObject.layer, null);
				}
				if (this.ToReplaceEffectShader)
				{
					U3DUtils.ReplaceEffectShader(obj);
				}
				if (this.LoadOK != null)
				{
					this.LoadOK(this, obj);
				}
				if (obj.layer == LayerMask.NameToLayer("MUUI"))
				{
					this.SetParticleSize(obj);
				}
				if (this.AutoDestroySelf)
				{
					Object.Destroy(this);
				}
			}
		}
		else if (this.LoadFail != null)
		{
			this.LoadFail(this, null);
		}
		this.bCompleted = true;
	}

	private void SetParticleSize(GameObject go)
	{
		if (go != null)
		{
			ParticleSystem[] componentsInChildren = go.GetComponentsInChildren<ParticleSystem>();
			if (componentsInChildren != null && componentsInChildren.Length > 0)
			{
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					if (componentsInChildren[i] != null)
					{
						componentsInChildren[i].startSize = this.ParticleScale * componentsInChildren[i].startSize;
					}
				}
			}
		}
	}

	private const bool OpenOrCloseCache = true;

	public string BundleID;

	public bool ToReplaceEffectShader;

	public AssetbundleLoaderComplete Complete;

	public AssetbundleLoaderComplete LoadOK;

	public AssetbundleLoaderComplete LoadFail;

	public string OwnerName;

	public int TriggerType = -1;

	public int ToLayer = -1;

	public bool AutoDestroySelf;

	public bool KeepOwnRenderQueue;

	public float ParticleScale = 1f;

	public bool ForceSyncfLoad = true;

	public bool IsCache;

	private bool bCompleted;
}
