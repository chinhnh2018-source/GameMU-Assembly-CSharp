using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class PlayRoleActions : MonoBehaviour
{
	public int Occupation { get; set; }

	private void Start()
	{
		this.Occupation = Global.CalcOriginalOccupationID(this.Occupation);
		this._Animation = base.GetComponent<Animation>();
		if (this._Animation != null)
		{
			if (base.GetComponent<Animation>()[this.StandName] == null)
			{
				base.GetComponent<Animation>().AddClip(Resources.Load(string.Concat(new object[]
				{
					"Prefabs/Skeleton/Anim/",
					this.Occupation,
					"/",
					this.StandName
				})) as AnimationClip, this.StandName);
			}
			if (base.GetComponent<Animation>()[this.AttackName] == null)
			{
				base.GetComponent<Animation>().AddClip(Resources.Load(string.Concat(new object[]
				{
					"Prefabs/Skeleton/Anim/",
					this.Occupation,
					"/",
					this.AttackName
				})) as AnimationClip, this.AttackName);
			}
			if (base.GetComponent<Animation>().clip == null)
			{
				base.GetComponent<Animation>().clip = base.GetComponent<Animation>().GetClip(this.StandName);
				base.GetComponent<Animation>().clip.wrapMode = 2;
			}
		}
		this._Animator = base.GetComponent<Animator>();
		if (this._Animator != null)
		{
			if (Global.GetClip(this._Animator, this.StandName) == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"没有这个动画" + this.StandName
				});
				return;
			}
			if (Global.GetClip(this._Animator, this.AttackName) == null)
			{
				MUDebug.LogError<string>(new string[]
				{
					"没有这个动画" + this.AttackName
				});
				return;
			}
			this.PlayAgain();
		}
		else if (null != this._Animation)
		{
			this.PlayAnimationAgain();
		}
	}

	private void OnDrag(Vector2 delta)
	{
		base.transform.Rotate((delta.x <= 0f) ? Vector3.up : Vector3.down, Mathf.Abs(delta.x));
	}

	private IEnumerator PlayQueue(float time, string name)
	{
		yield return new WaitForSeconds(time);
		if (null != Global.GetClip(this._Animator, name))
		{
			Global.PlayAnimatorClip(this._Animator, name);
		}
		yield break;
	}

	public void PlayAnimationAgain()
	{
		if (null != this._Animation)
		{
			if (this.IsPlayAttack)
			{
				if (null != this._Animation[this.AttackName])
				{
					this._Animation[this.AttackName].wrapMode = 1;
					this._Animation.CrossFade(this.AttackName);
				}
				if (null != this._Animation[this.StandName])
				{
					this._Animation[this.StandName].wrapMode = 2;
					this._Animation.CrossFadeQueued(this.StandName, 0.5f);
				}
			}
			else
			{
				this._Animation.CrossFade(this.AttackName);
			}
		}
	}

	public void PlayAgain()
	{
		if (null != this._Animator)
		{
			if (this.IsPlayAttack)
			{
				AnimationClip clip = Global.GetClip(this._Animator, this.AttackName);
				if (null != clip)
				{
					Global.PlayAnimatorClip(this._Animator, this.AttackName);
				}
				base.StartCoroutine(this.PlayQueue(Global.GetClipLength(this._Animator, this.AttackName), this.StandName));
			}
			else if (null != Global.GetClip(this._Animator, this.StandName))
			{
				Global.PlayAnimatorClip(this._Animator, this.StandName);
			}
		}
	}

	public void StopAttackAnimation()
	{
		this.IsPlayAttack = false;
		if (null != this._Animator && null != Global.GetClip(this._Animator, this.StandName))
		{
			Global.PlayAnimatorClip(this._Animator, this.StandName);
		}
	}

	public void PlayAnim(string animName, WrapMode mode = 1)
	{
		if (null != this._Animator)
		{
			Global.PlayAnimatorClip(this._Animator, animName);
		}
	}

	public bool IsPlayAnim(string name)
	{
		return null != this._Animator && this._Animator.GetCurrentAnimatorStateInfo(0).IsName(name);
	}

	private void OnEnable()
	{
		if (null != this._Animation)
		{
			this.PlayAnimationAgain();
		}
		if (null != this._Animator)
		{
			this.PlayAgain();
		}
	}

	private void HurtCount()
	{
	}

	public string StandName = "KStand";

	public string AttackName = "KAttack";

	private Animator _Animator;

	private Animation _Animation;

	private bool IsPlayAttack = true;
}
