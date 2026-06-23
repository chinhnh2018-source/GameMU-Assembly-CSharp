using System;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class MonsterInjuredState : SpriteStateBase
{
	public MonsterInjuredState(GSprite sprite) : base(sprite)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Injured;
		}
	}

	public override void EnterState()
	{
		if (!this.mSprite.HasClip(this.injureName))
		{
			this.mSprite.AnimationController.SetState(SpriteAnimState.Stand);
			return;
		}
		if (this.mSprite.AnimatorComponent.speed != 0f)
		{
			this.mAnimEndTime = Time.time + this.mSprite.GetClipLength(this.injureName) / this.mSprite.AnimatorComponent.speed - 0.1f;
		}
		this.mSprite.AnimatorComponent.SetBool(this.injureName, true);
		this.mSprite.PlayMonsterHitSound();
	}

	public override void Update()
	{
		if (Time.time >= this.mAnimEndTime)
		{
			this.mSprite.AnimationController.SetState(SpriteAnimState.Stand);
		}
	}

	public override void ExitState()
	{
		if (this.mSprite.HasClip(this.injureName))
		{
			this.mSprite.AnimatorComponent.SetBool(this.injureName, false);
		}
		base.ExitState();
	}

	private float mAnimEndTime;

	private string injureName = "Hit";
}
