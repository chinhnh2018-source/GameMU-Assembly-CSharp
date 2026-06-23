using System;
using System.Collections.Generic;
using UnityEngine;

public class ManagedObject : MonoBehaviour
{
	public void SaveMaterial(GameObject obj)
	{
		if (this.OldMaterial == null)
		{
			this.OldMaterial = new Dictionary<Renderer, Material[]>();
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				this.OldMaterial.Add(componentsInChildren[i], componentsInChildren[i].sharedMaterials);
			}
		}
	}

	public void RevertMaterial(GameObject obj)
	{
		if (this.OldMaterial != null)
		{
			Renderer[] componentsInChildren = obj.GetComponentsInChildren<Renderer>(true);
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				Material[] array = null;
				this.OldMaterial.TryGetValue(componentsInChildren[i], ref array);
				if (array != null)
				{
					componentsInChildren[i].sharedMaterials = array;
				}
			}
		}
	}

	public void SaveLocalScale()
	{
		this.scale = base.transform.localScale;
	}

	public void RevertLocalScale()
	{
		if (this.scale != Vector3.zero)
		{
			base.transform.localScale = this.scale;
		}
	}

	public void ReleaseRef()
	{
		base.gameObject.SetActive(false);
		if (this.onDisabled != null)
		{
			this.onDisabled.Invoke(this);
		}
	}

	private void Awake()
	{
		this.fxCtl = base.transform.GetComponentInChildren<FxPlayController>();
		if (!(null != this.fxCtl))
		{
			this.mMonoArr = base.GetComponentsInChildren<MonoBehaviour>(true);
			this.mRendererArr = base.GetComponentsInChildren<Renderer>(true);
			this.mParticleArr = base.GetComponentsInChildren<ParticleSystem>(true);
			this.mAnimator = base.GetComponentsInChildren<Animator>(true);
			this.mAnimation = base.GetComponentsInChildren<Animation>(true);
		}
		if (this.isNewInstance)
		{
			this.SaveLocalScale();
		}
	}

	private void OnDisable()
	{
	}

	private void OnDestroy()
	{
		if (this.onDestroyed != null)
		{
			this.onDestroyed.Invoke(this);
		}
	}

	public void ManualEnable()
	{
		if (null != this.fxCtl)
		{
			this.fxCtl.enabled = true;
		}
		else
		{
			if (this.mMonoArr != null)
			{
				for (int i = 0; i < this.mMonoArr.Length; i++)
				{
					if (null != this.mMonoArr[i])
					{
						this.mMonoArr[i].enabled = true;
					}
				}
			}
			if (this.mRendererArr != null)
			{
				for (int j = 0; j < this.mRendererArr.Length; j++)
				{
					if (null != this.mRendererArr[j])
					{
						this.mRendererArr[j].enabled = true;
					}
				}
			}
			if (this.mParticleArr != null)
			{
				for (int k = 0; k < this.mParticleArr.Length; k++)
				{
					if (null != this.mParticleArr[k])
					{
						this.mParticleArr[k].Play(true);
					}
				}
			}
		}
	}

	public void ManualRelease()
	{
		if (null != base.transform.parent && base.transform.gameObject.activeInHierarchy)
		{
			base.transform.SetParent(null);
		}
		base.transform.position = new Vector3(-2000f, -2000f, 0f);
		if (null != this.fxCtl)
		{
			this.fxCtl.enabled = false;
		}
		else
		{
			if (this.mMonoArr != null)
			{
				for (int i = 0; i < this.mMonoArr.Length; i++)
				{
					if (this != this.mMonoArr[i] && null != this.mMonoArr[i])
					{
						this.mMonoArr[i].enabled = false;
					}
				}
			}
			if (this.mRendererArr != null)
			{
				for (int j = 0; j < this.mRendererArr.Length; j++)
				{
					if (this.mRendererArr[j] != null)
					{
						this.mRendererArr[j].enabled = false;
					}
				}
			}
			if (this.mParticleArr != null)
			{
				for (int k = 0; k < this.mParticleArr.Length; k++)
				{
					if (this.mParticleArr[k] != null)
					{
						this.mParticleArr[k].Stop(true);
					}
				}
			}
		}
		if (this.onDisabled != null)
		{
			this.onDisabled.Invoke(this);
			this.RevertMaterial(base.gameObject);
			this.RevertLocalScale();
		}
	}

	public int instanceID;

	public string resURL;

	public bool isNewInstance = true;

	public Action<ManagedObject> onDisabled;

	public Action<ManagedObject> onDestroyed;

	private Dictionary<Renderer, Material[]> OldMaterial;

	public Vector3 scale = Vector3.zero;

	public bool isSkillManage;

	public FxPlayController fxCtl;

	private MonoBehaviour[] mMonoArr;

	private Renderer[] mRendererArr;

	private ParticleSystem[] mParticleArr;

	private Animator[] mAnimator;

	private Animation[] mAnimation;

	public Vector3 InitPos = Vector3.zero;

	public Quaternion InitRotation = Quaternion.Euler(Vector3.zero);

	public Vector3 InitScale = Vector3.one;

	public bool isCacheObject;
}
