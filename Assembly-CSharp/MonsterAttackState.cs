using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class MonsterAttackState : SpriteStateBase
{
	public MonsterAttackState(GSprite monster) : base(monster)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Attack;
		}
	}

	public override void EnterState()
	{
		this.PlayAttackAction();
		if (this.mSprite.Action == GActions.Attack)
		{
			this.mSprite.PlayMonsterAttackSound();
		}
		this.mSprite.DoMagicDecoration();
	}

	public override void Update()
	{
		if (this.mSprite.AnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName(this.mAttackName) && this.mSprite.AnimatorComponent.isInitialized)
		{
			this.mSprite.AnimatorComponent.SetBool(this.mAttackName, false);
		}
		if (Time.time > this.mAttackEndTime)
		{
			this.mSprite.AnimationController.SetState(SpriteAnimState.Stand);
		}
	}

	public override void ExitState()
	{
		this.mAttackName = null;
	}

	private string GetAttackName()
	{
		string text;
		if (this.mSprite.Action != GActions.Magic || this.mSprite.CurrentMagic < 0)
		{
			WrapMode wrapMode = 2;
			text = Global.GetActionName(this.mSprite, this.mSprite.Action, this.mSprite.IsInSafeRegion, out wrapMode);
			if (this.mSprite.Action == GActions.Attack)
			{
				text = Global.GetMonsterActionName(this.mSprite.CurrentMagic, out wrapMode);
			}
		}
		else
		{
			text = ConfigMagicInfos.GetSkillActionName(this.mSprite.CurrentMagic);
			if (text.CompareTo("dcj") == 0)
			{
				text = "Ndcj";
				if (this.mSprite.WeaponState == WeaponStates.G)
				{
					text = "Gdcj";
				}
			}
			else if (text.CompareTo("wcj") == 0)
			{
				text = "Nwcj";
				if (this.mSprite.WeaponState == WeaponStates.G)
				{
					text = "Gwcj";
				}
			}
			else if (text.CompareTo("zl") == 0)
			{
				text = "Nzl";
				if (this.mSprite.WeaponState == WeaponStates.G)
				{
					text = "Gzl";
				}
			}
			else if (text.CompareTo("ctj") == 0)
			{
				text = "Nctj";
				if (this.mSprite.WeaponState == WeaponStates.G)
				{
					text = "Gctj";
				}
			}
			else if (text.Contains("bfj"))
			{
				if (this.mSprite.WeaponState == WeaponStates.G)
				{
					text = "G" + text;
				}
				else
				{
					text = "N" + text;
				}
			}
		}
		return text;
	}

	private void PlayAttackAction()
	{
		if (!string.IsNullOrEmpty(this.mAttackName))
		{
			this.mSprite.AnimatorComponent.SetBool(this.mAttackName, false);
		}
		this.mAttackName = this.GetAttackName();
		if (!this.mSprite.HasClip(this.mAttackName))
		{
			this.mSprite.AnimationController.SetState(SpriteAnimState.Stand);
			return;
		}
		this.mSprite.AnimatorComponent.SetBool("Stand", false);
		this.mSprite.AnimatorComponent.SetBool("Walk", false);
		this.mSprite.AnimatorComponent.SetBool(this.mAttackName, true);
		this.mSprite.AnimatorComponent.Play(this.mAttackName, 0, 0f);
		if (this.mSprite.AnimatorComponent.speed != 0f)
		{
			this.mAttackEndTime = Time.time + this.mSprite.GetClipLength(this.mAttackName) / this.mSprite.AnimatorComponent.speed - 0.1f;
		}
	}

	private string mAttackName;

	private float mAttackEndTime;
}
