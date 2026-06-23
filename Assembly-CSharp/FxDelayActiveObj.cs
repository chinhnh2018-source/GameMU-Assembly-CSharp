using System;
using System.Collections;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class FxDelayActiveObj : MonoBehaviour
{
	private void Awake()
	{
		if (this.toBeActiveGameObj == null)
		{
			if (base.transform.parent != null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"toBeActiveGameObj is Null,parent ===" + base.transform.parent.name
				});
			}
			this.toBeActiveGameObj = base.transform.gameObject;
		}
		this.childParticles = this.toBeActiveGameObj.GetComponentsInChildren<ParticleSystem>();
	}

	private void OnEnable()
	{
		if (!this.isInit)
		{
			this.initPos.x = this.toBeActiveGameObj.transform.localPosition.x;
			this.initPos.y = this.toBeActiveGameObj.transform.localPosition.y;
			this.initPos.z = this.toBeActiveGameObj.transform.localPosition.z;
			this.childParticles = this.toBeActiveGameObj.GetComponentsInChildren<ParticleSystem>();
			this.isInit = true;
		}
		if (this.childParticles != null)
		{
			for (int i = 0; i < this.childParticles.Length; i++)
			{
				if (this.childParticles[i].gameObject == this.toBeActiveGameObj && !LGQualityManager.ParticleEnabled)
				{
					return;
				}
			}
		}
		if (this.delayTime > 0f)
		{
			this.StopAnimator();
			base.StartCoroutine(this.WaitForActive());
		}
		else
		{
			this.PlayAnimator();
			if (!LGQualityManager.ParticleEnabled && this.childParticles != null)
			{
				for (int j = 0; j < this.childParticles.Length; j++)
				{
					this.childParticles[j].gameObject.SetActive(false);
				}
			}
		}
		if (this.delayDisactiveTime > 0f)
		{
			base.StartCoroutine(this.WaitForDisactive());
		}
	}

	private void OnDisable()
	{
		this.StopAnimator();
		base.StopAllCoroutines();
	}

	private IEnumerator WaitForActive()
	{
		yield return new WaitForSeconds(this.delayTime);
		this.PlayAnimator();
		if (this.childParticles != null)
		{
			for (int i = 0; i < this.childParticles.Length; i++)
			{
				if (this.ParticleisLeader)
				{
					this.childParticles[i].gameObject.SetActive(true);
				}
				else
				{
					this.childParticles[i].gameObject.SetActive(LGQualityManager.ParticleEnabled);
				}
			}
		}
		yield break;
	}

	private IEnumerator WaitForDisactive()
	{
		yield return new WaitForSeconds(this.delayDisactiveTime);
		this.StopAnimator();
		yield break;
	}

	private void PlayAnimator()
	{
		this.toBeActiveGameObj.transform.localPosition = this.initPos;
		this.InitSeachComponent();
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
		if (this.mRendererLst != null)
		{
			for (int j = 0; j < this.mRendererLst.Count; j++)
			{
				if (null != this.mRendererLst[j])
				{
					this.mRendererLst[j].enabled = true;
				}
			}
		}
		if (this.mParticleLst != null)
		{
			for (int k = 0; k < this.mParticleLst.Count; k++)
			{
				if (null != this.mParticleLst[k])
				{
					ParticleSystem particleSystem = this.mParticleLst[k];
					if (particleSystem)
					{
						if (!LGQualityManager.ParticleEnabled && !this.ParticleisLeader)
						{
							particleSystem.gameObject.SetActive(false);
						}
						if (LGQualityManager.ParticleEnabled || this.ParticleisLeader)
						{
							particleSystem.Play();
						}
					}
				}
			}
		}
		if (this.mFxAnimatorList != null)
		{
			for (int l = 0; l < this.mFxAnimatorList.Count; l++)
			{
				if (null != this.mFxAnimatorList[l])
				{
					this.mFxAnimatorList[l].enabled = true;
					this.mFxAnimatorList[l].Play(this.mFxAnimatorList[l].GetCurrentAnimatorStateInfo(0).fullPathHash, 0, 0f);
				}
			}
		}
		if (this.mFxAnimationList != null)
		{
			for (int m = 0; m < this.mFxAnimationList.Count; m++)
			{
				if (null != this.mFxAnimationList[m])
				{
					this.mFxAnimationList[m].enabled = true;
					this.mFxAnimationList[m].Play();
				}
			}
		}
	}

	public void StopAnimator()
	{
		this.InitSeachComponent();
		if (this.mDelayLst != null)
		{
			for (int i = 0; i < this.mDelayLst.Count; i++)
			{
				if (null != this.mDelayLst[i])
				{
					this.mDelayLst[i].StopAnimator();
				}
			}
		}
		if (this.mMonoLst != null)
		{
			for (int j = 0; j < this.mMonoLst.Count; j++)
			{
				if (null != this.mMonoLst[j])
				{
					this.mMonoLst[j].enabled = false;
				}
			}
		}
		if (this.mFxAnimatorList != null)
		{
			for (int k = 0; k < this.mFxAnimatorList.Count; k++)
			{
				if (null != this.mFxAnimatorList[k])
				{
					this.mFxAnimatorList[k].enabled = false;
				}
			}
		}
		if (this.mFxAnimationList != null)
		{
			for (int l = 0; l < this.mFxAnimationList.Count; l++)
			{
				if (null != this.mFxAnimationList[l])
				{
					this.mFxAnimationList[l].Stop();
					this.mFxAnimationList[l].enabled = false;
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
		if (this.mParticleLst != null)
		{
			for (int n = 0; n < this.mParticleLst.Count; n++)
			{
				if (null != this.mParticleLst[n])
				{
					ParticleSystem particleSystem = this.mParticleLst[n];
					if (particleSystem)
					{
						if (!LGQualityManager.ParticleEnabled && !this.ParticleisLeader)
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
		}
		this.ParticleisLeader = false;
	}

	private void InitSeachComponent()
	{
		if (this.isComponentInit)
		{
			this.isComponentInit = true;
			return;
		}
		this.mFxAnimatorList = new List<Animator>();
		this.mFxAnimationList = new List<Animation>();
		this.mMonoLst = new List<MonoBehaviour>();
		this.mRendererLst = new List<Renderer>();
		this.mParticleLst = new List<ParticleSystem>();
		this.mDelayLst = new List<FxDelayActiveObj>();
		this.delayObj = new List<GameObject>();
		this.InitSeachComponent(this.toBeActiveGameObj.transform);
		this.delayObj.Clear();
	}

	private void InitSeachComponent(Transform trans)
	{
		FxDelayActiveObj component = trans.GetComponent<FxDelayActiveObj>();
		if (null != component && this != component)
		{
			this.delayObj.Add(component.toBeActiveGameObj);
			this.mMonoLst.Add(component);
			this.mDelayLst.Add(component);
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
		for (int i = 0; i < trans.GetChildCount(); i++)
		{
			Transform child = trans.GetChild(i);
			this.InitSeachComponent(child);
		}
	}

	public float delayTime;

	public float delayDisactiveTime;

	public GameObject toBeActiveGameObj;

	private ParticleSystem[] childParticles;

	private List<Animator> mFxAnimatorList;

	private List<Animation> mFxAnimationList;

	private List<MonoBehaviour> mMonoLst;

	private List<Renderer> mRendererLst;

	private List<ParticleSystem> mParticleLst;

	private List<FxDelayActiveObj> mDelayLst;

	private Vector3 initPos = default(Vector3);

	private bool isInit;

	public bool ParticleisLeader;

	private bool isComponentInit;

	private List<GameObject> delayObj;
}
