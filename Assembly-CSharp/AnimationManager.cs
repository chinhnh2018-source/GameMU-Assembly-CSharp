using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class AnimationManager : ManualUpdateBehaviour
{
	public Animator Animator
	{
		get
		{
			if (null == this.mAnimator)
			{
				this.mAnimator = base.GetComponent<Animator>();
			}
			return this.mAnimator;
		}
		set
		{
			this.mAnimator = value;
		}
	}

	public static void InitAnimData(int occupation, GameObject obj)
	{
	}

	private void Start()
	{
		AnimationClip clip = Global.GetClip(this.Animator, this.AnimationName);
		if (clip == null)
		{
			if (this.IsAllRole())
			{
				int occupation = Global.CalcOriginalOccupationID(this.gSprite.Occupation);
				this.AnimationName = this.GetStandAnimationNameByOccupation(occupation);
			}
			else
			{
				this.AnimationName = "Stand";
			}
			clip = Global.GetClip(this.Animator, this.AnimationName);
		}
		if (null != clip)
		{
			Global.PlayAnimatorClip(this.Animator, this.AnimationName);
		}
	}

	public override void ManualUpdate()
	{
		this.UpdateHorseAnimation();
		if (string.IsNullOrEmpty(this.AnimationName))
		{
			return;
		}
		if (null == this.Animator)
		{
			return;
		}
		float num = Time.time - this.mClipStartTime;
		if (this.isFirstChangeAction || this.mClipLength - num <= this.mClipLength * this.CrossFadeTime)
		{
			if (this.isFirstChangeAction)
			{
				this.isFirstChangeAction = false;
			}
			if (this.mWrapMode == 1)
			{
				if (this.TotalAnimationCount > 0)
				{
					if (this.TotalAnimationCount == 1 && this.EndAnimation != null)
					{
						this.TotalAnimationCount = int.MaxValue;
						this.isLeiTingLieShan = false;
						this.EndAnimation(this, EventArgs.Empty);
						if (this.AnimationName.Equals("ltls"))
						{
							this.CrossFadeTime = this.tempCrossFadeTime;
						}
					}
					return;
				}
				if (this.PrepareAnimation != null)
				{
					this.PrepareAnimation(this, EventArgs.Empty);
				}
				if (this.PlayingAnimation != null)
				{
					this.PlayingAnimation(this, EventArgs.Empty);
				}
			}
			else if (this.OnceLoop)
			{
				if (this.PrepareAnimation != null)
				{
					this.PrepareAnimation(this, EventArgs.Empty);
				}
				if (this.PlayingAnimation != null)
				{
					this.PlayingAnimation(this, EventArgs.Empty);
				}
			}
			this.TotalAnimationCount++;
			float num2 = 0f;
			if (this.IsAllRole())
			{
				num2 = 0.15f;
			}
			string animationName = this.AnimationName;
			this.Animator.CrossFade(animationName, num2);
		}
		else if (this.OnceLoop && this.mClipLength - num <= this.mClipLength * this.CrossFadeTime)
		{
			if (this.EndAnimation != null)
			{
				this.EndAnimation(this, EventArgs.Empty);
			}
			if (this.PrepareAnimation != null)
			{
				this.PrepareAnimation(this, EventArgs.Empty);
			}
			if (this.PlayingAnimation != null)
			{
				this.PlayingAnimation(this, EventArgs.Empty);
			}
		}
	}

	protected override void OnDisable()
	{
		base.OnDisable();
		this.gSprite = null;
		this.OnceLoop = false;
		this.mClipStartTime = 0f;
		this.PrepareAnimation = null;
		this.PlayingAnimation = null;
		this.EndAnimation = null;
	}

	public void ChangeAnimation(string animationName, WrapMode wrapMode = 2, bool onceLoop = false, GSprite gSprite = null, float animTime = 0f)
	{
		if (this.IsAllRole())
		{
			this.CrossFadeTime = 0.15f;
			if (this.gSprite.Action == GActions.Magic && animationName.Equals("zkb1"))
			{
				this.CrossFadeTime = 0f;
			}
		}
		if (animationName.Equals("ltls"))
		{
			this.isLeiTingLieShan = true;
		}
		base.CancelInvoke("RepeatAnimation");
		this.AnimationName = animationName;
		if (null == this.Animator)
		{
			return;
		}
		AnimationClip clip = Global.GetClip(this.Animator, this.AnimationName);
		if (clip == null)
		{
			if (this.IsAllRole())
			{
				this.AnimationName = this.GetStandAnimationNameByOccupation(this.gSprite.Occupation);
			}
			else
			{
				this.AnimationName = "Stand";
			}
			clip = Global.GetClip(this.Animator, this.AnimationName);
		}
		if (clip != null)
		{
			this.mClipLength = Global.GetClipLength(this.Animator, this.AnimationName);
			this.mClipStartTime = Time.time;
			this.isFirstChangeAction = true;
			Global.PlayAnimatorClip(this.Animator, this.AnimationName);
		}
		if (wrapMode == 2)
		{
			GSprite gsprite = gSprite;
			if (gsprite == null)
			{
				gsprite = this.gSprite;
			}
			if (gsprite != null && gsprite.SpriteType == GSpriteTypes.Leader && gsprite.Action == GActions.Magic)
			{
				wrapMode = 1;
			}
		}
		this.mWrapMode = wrapMode;
		this.TotalAnimationCount = 0;
		this.OnceLoop = onceLoop;
	}

	private string GetStandAnimationNameByOccupation(int Occupation)
	{
		switch (Occupation)
		{
		case 0:
			return "SStand";
		case 1:
			return "KStand";
		case 2:
			return "NStand";
		case 3:
			return "FSStand";
		case 4:
		case 5:
			return "SStand";
		default:
			return string.Empty;
		}
	}

	public string GetAnimationName()
	{
		return this.AnimationName;
	}

	public bool IsLeiTingLieShan()
	{
		return this.isLeiTingLieShan;
	}

	public void DelayChangeAnimation(string animationName, WrapMode wrapMode = 2, float delayTime = 1f)
	{
		this.DelayAnimationName = animationName;
		this.DelayWrapMode = wrapMode;
		base.CancelInvoke("RepeatAnimation");
		base.InvokeRepeating("RepeatAnimation", delayTime, 0f);
	}

	private void RepeatAnimation()
	{
		base.CancelInvoke("RepeatAnimation");
		this.ChangeAnimation(this.DelayAnimationName, this.DelayWrapMode, false, null, 0f);
	}

	public bool IsContinuePlay(GActions gActions)
	{
		if (gActions == GActions.Attack || gActions == GActions.Magic)
		{
			float num = Time.time - this.mClipStartTime;
			return this.mClipLength - num <= this.mClipLength * 0.5f;
		}
		return true;
	}

	private bool IsRole()
	{
		return this.gSprite != null && this.gSprite.SpriteType == GSpriteTypes.Leader;
	}

	private bool IsAllRole()
	{
		return this.gSprite != null && (this.gSprite.SpriteType == GSpriteTypes.Leader || this.gSprite.SpriteType == GSpriteTypes.FakeRole || this.gSprite.SpriteType == GSpriteTypes.Other);
	}

	private void HurtCount(float p)
	{
	}

	public void ChangeHorseAction(GActions action)
	{
		this.HorseAnimationName = this.GetHorseAnimationName(action);
		if (this.gSprite != null)
		{
			Animator horseAnimator = this.gSprite.HorseAnimator;
			if (null != horseAnimator)
			{
				AnimationClip clip = Global.GetClip(horseAnimator, this.HorseAnimationName);
				if (null == clip)
				{
					this.HorseAnimationName = this.GetHorseAnimationName(GActions.Stand);
					clip = Global.GetClip(horseAnimator, this.HorseAnimationName);
				}
				if (clip != null)
				{
					this.mHorseClipLength = Global.GetClipLength(horseAnimator, this.HorseAnimationName);
					this.mHorseClipStartTime = Time.time;
					Global.PlayAnimatorClip(horseAnimator, this.HorseAnimationName);
				}
			}
		}
	}

	private void UpdateHorseAnimation()
	{
		if (this.gSprite == null)
		{
			return;
		}
		if (!this.gSprite.OnHorseEX)
		{
			return;
		}
		Animator horseAnimator = this.gSprite.HorseAnimator;
		if (null != horseAnimator)
		{
			float num = Time.time - this.mHorseClipStartTime;
			if (this.mHorseClipLength - num <= this.mHorseClipLength * this.CrossFadeTime)
			{
				float num2 = 0f;
				if (this.IsAllRole())
				{
					num2 = 0.15f;
				}
				horseAnimator.CrossFade(this.HorseAnimationName, num2);
			}
		}
	}

	private string GetHorseAnimationName(GActions action)
	{
		if (action == GActions.Run)
		{
			return "Run";
		}
		if (action == GActions.Walk)
		{
			return "Walk";
		}
		if (action == GActions.Stand)
		{
			return "Stand";
		}
		return "Walk";
	}

	public static Dictionary<int, Dictionary<string, float[]>> animDic = new Dictionary<int, Dictionary<string, float[]>>();

	public string AnimationName = "Stand";

	public string HorseAnimationName = "Stand";

	private int TotalAnimationCount;

	private bool OnceLoop;

	private bool isLeiTingLieShan;

	public float CrossFadeTime = 0.15f;

	public GSprite gSprite;

	private Animator mAnimator;

	private float mHorseClipStartTime;

	private float mHorseClipLength;

	private WrapMode mHorseWrapMode;

	private float mClipStartTime;

	private float mClipLength;

	private WrapMode mWrapMode;

	private bool isFirstChangeAction;

	public AnimationChangeEventHandler PrepareAnimation;

	public AnimationChangeEventHandler PlayingAnimation;

	public AnimationChangeEventHandler EndAnimation;

	private string DelayAnimationName = "Stand";

	private WrapMode DelayWrapMode = 1;

	private float tempCrossFadeTime;
}
