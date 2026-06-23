using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class FxPlayController : MonoBehaviour
{
	private void Awake()
	{
		this.InitSeachComponent();
	}

	private void OnEnable()
	{
		if (this.mMonoLst != null)
		{
			for (int i = 0; i < this.mMonoLst.Count; i++)
			{
				if (null != this.mMonoLst[i])
				{
					this.mMonoLst[i].enabled = true;
				}
			}
		}
		if (this.mFxAnimatorList != null)
		{
			for (int j = 0; j < this.mFxAnimatorList.Count; j++)
			{
				Animator animator = this.mFxAnimatorList[j];
				if (animator)
				{
					animator.enabled = true;
					if (animator.runtimeAnimatorController)
					{
						animator.Play(animator.GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
					}
				}
			}
		}
		if (this.mFxAnimationList != null)
		{
			for (int k = 0; k < this.mFxAnimationList.Count; k++)
			{
				Animation animation = this.mFxAnimationList[k];
				if (animation)
				{
					animation.enabled = true;
					animation.Play();
				}
			}
		}
		if (this.mParticleLst != null)
		{
			for (int l = 0; l < this.mParticleLst.Count; l++)
			{
				ParticleSystem particleSystem = this.mParticleLst[l];
				if (particleSystem)
				{
					if (!LGQualityManager.ParticleEnabled && !this.isLeader)
					{
						particleSystem.gameObject.SetActive(false);
					}
					if (LGQualityManager.ParticleEnabled || this.isLeader)
					{
						particleSystem.Play();
					}
				}
			}
		}
		if (this.mRendererLst != null)
		{
			for (int m = 0; m < this.mRendererLst.Count; m++)
			{
				if (null != this.mRendererLst[m])
				{
					this.mRendererLst[m].enabled = true;
				}
			}
		}
		if (this.mCameraLst != null)
		{
			for (int n = 0; n < this.mCameraLst.Count; n++)
			{
				if (null != this.mCameraLst[n])
				{
					this.mCameraLst[n].enabled = true;
				}
			}
		}
		if (this.mAudioSourceLst != null)
		{
			for (int num = 0; num < this.mAudioSourceLst.Count; num++)
			{
				if (null != this.mAudioSourceLst[num])
				{
					this.mAudioSourceLst[num].enabled = true;
				}
			}
		}
		if (this.duration > 0f)
		{
			this.mCurrentWait = base.StartCoroutine(this.WaitForFxEnd());
		}
	}

	public void ManualEnable()
	{
		this.OnEnable();
	}

	public void isLeaderParticleSystem(bool isLe)
	{
		this.isLeader = true;
		FxDelayActiveObj[] componentsInChildren = base.GetComponentsInChildren<FxDelayActiveObj>(true);
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			componentsInChildren[i].ParticleisLeader = true;
		}
	}

	public void SetDuration(float t)
	{
		if (this.mCurrentWait != null)
		{
			base.StopCoroutine(this.mCurrentWait);
		}
		this.duration = t;
		if (this.duration > 0f)
		{
			this.mCurrentWait = base.StartCoroutine(this.WaitForFxEnd());
		}
	}

	private IEnumerator WaitForFxEnd()
	{
		yield return new WaitForSeconds(this.duration);
		if (this.autoDestory)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			ManagedObject managed = base.gameObject.GetComponent<ManagedObject>();
			if (managed)
			{
				managed.ManualRelease();
			}
			else
			{
				base.enabled = false;
			}
		}
		yield break;
	}

	private void OnDisable()
	{
		if (this.isLimitedFx)
		{
			FxResLoader.PlayerMagicCount--;
		}
		this.mCurrentWait = null;
		if (this.onEnd != null)
		{
			this.onEnd(this, base.gameObject);
		}
		if (this.mFxAnimatorList != null)
		{
			for (int i = 0; i < this.mFxAnimatorList.Count; i++)
			{
				if (null != this.mFxAnimatorList[i])
				{
					Animator animator = this.mFxAnimatorList[i];
					animator.enabled = false;
				}
			}
		}
		if (this.mFxAnimationList != null)
		{
			for (int j = 0; j < this.mFxAnimationList.Count; j++)
			{
				if (null != this.mFxAnimationList[j])
				{
					Animation animation = this.mFxAnimationList[j];
					animation.enabled = false;
				}
			}
		}
		if (this.mMonoLst != null)
		{
			for (int k = 0; k < this.mMonoLst.Count; k++)
			{
				if (null != this.mMonoLst[k] && this != this.mMonoLst[k])
				{
					this.mMonoLst[k].enabled = false;
				}
			}
		}
		if (this.mParticleLst != null)
		{
			for (int l = 0; l < this.mParticleLst.Count; l++)
			{
				ParticleSystem particleSystem = this.mParticleLst[l];
				if (particleSystem)
				{
					if (!LGQualityManager.ParticleEnabled && !this.isLeader)
					{
						particleSystem.gameObject.SetActive(false);
					}
					else
					{
						particleSystem.Stop(true);
					}
				}
			}
		}
		if (this.mRendererLst != null)
		{
			for (int m = 0; m < this.mRendererLst.Count; m++)
			{
				if (null != this.mRendererLst[m])
				{
					this.mRendererLst[m].enabled = false;
				}
			}
		}
		if (this.mCameraLst != null)
		{
			for (int n = 0; n < this.mCameraLst.Count; n++)
			{
				if (null != this.mCameraLst[n])
				{
					this.mCameraLst[n].enabled = false;
				}
			}
		}
		if (this.mAudioSourceLst != null)
		{
			for (int num = 0; num < this.mAudioSourceLst.Count; num++)
			{
				if (null != this.mAudioSourceLst[num])
				{
					this.mAudioSourceLst[num].enabled = false;
				}
			}
		}
		this.isLeader = false;
	}

	private void OnDestroy()
	{
		if (this.onDestroy != null)
		{
			this.onDestroy(this, base.gameObject);
		}
	}

	private void InitSeachComponent()
	{
		if (this.isComponentInit)
		{
			return;
		}
		this.mFxAnimatorList = new List<Animator>();
		this.mFxAnimationList = new List<Animation>();
		this.mMonoLst = new List<MonoBehaviour>();
		this.mRendererLst = new List<Renderer>();
		this.mParticleLst = new List<ParticleSystem>();
		this.delayObj = new List<GameObject>();
		this.mCameraLst = new List<Camera>();
		this.mAudioSourceLst = new List<AudioSource>();
		this.InitSeachComponent(base.transform);
		this.delayObj.Clear();
	}

	private void InitSeachComponent(Transform trans)
	{
		FxDelayActiveObj component = trans.GetComponent<FxDelayActiveObj>();
		if (null != component)
		{
			this.delayObj.Add(component.toBeActiveGameObj);
			this.mMonoLst.Add(component);
		}
		if (this.delayObj.Contains(trans.gameObject))
		{
			return;
		}
		Animator[] components = trans.GetComponents<Animator>();
		if (components != null)
		{
			this.mFxAnimatorList.AddRange(components);
		}
		Animation[] components2 = trans.GetComponents<Animation>();
		if (components2 != null)
		{
			this.mFxAnimationList.AddRange(components2);
		}
		MonoBehaviour[] components3 = trans.GetComponents<MonoBehaviour>();
		if (components3 != null)
		{
			this.mMonoLst.AddRange(components3);
		}
		Renderer[] components4 = trans.GetComponents<Renderer>();
		if (components4 != null)
		{
			this.mRendererLst.AddRange(components4);
		}
		ParticleSystem[] components5 = trans.GetComponents<ParticleSystem>();
		if (components5 != null)
		{
			this.mParticleLst.AddRange(components5);
		}
		Camera[] components6 = trans.GetComponents<Camera>();
		if (components6 != null)
		{
			this.mCameraLst.AddRange(components6);
		}
		AudioSource[] components7 = trans.GetComponents<AudioSource>();
		if (components7 != null)
		{
			this.mAudioSourceLst.AddRange(components7);
		}
		for (int i = 0; i < trans.GetChildCount(); i++)
		{
			Transform child = trans.GetChild(i);
			this.InitSeachComponent(child);
		}
	}

	public float duration;

	public bool autoDestory;

	public Animator[] mAnimatorList;

	public Animation[] mAnimationList;

	public ParticleSystem[] mParticleList;

	private List<Animator> mFxAnimatorList;

	private List<Animation> mFxAnimationList;

	private List<MonoBehaviour> mMonoLst;

	private List<Renderer> mRendererLst;

	private List<ParticleSystem> mParticleLst;

	private List<Camera> mCameraLst;

	private List<AudioSource> mAudioSourceLst;

	private Coroutine mCurrentWait;

	public EndEventHandler onEnd;

	public EndEventHandler onDestroy;

	public bool isLimitedFx;

	private bool isLeader;

	private bool isComponentInit;

	private List<GameObject> delayObj;
}
