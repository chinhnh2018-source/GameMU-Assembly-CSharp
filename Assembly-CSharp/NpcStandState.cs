using System;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class NpcStandState : NpcStateBase
{
	public NpcStandState(GSprite sprite) : base(sprite)
	{
	}

	private void HasClipRelax()
	{
		this.bHasClipRelax = base.HaveClip(this.mSprite.AnimatorComponent, "Relax");
	}

	public override void EnterState()
	{
		if (null == this.mSprite.AnimatorComponent)
		{
			return;
		}
		this.HasClipRelax();
		if (this.bHasClipRelax)
		{
			this.mSprite.AnimatorComponent.SetBool("Relax", false);
			if (this.mSprite.AnimatorComponent.speed != 0f)
			{
				this.clipTime = base.GetClipLength(this.mSprite.AnimatorComponent, "Relax") / this.mSprite.AnimatorComponent.speed - 0.1f;
			}
			this.mEndTime = this.clipTime + Time.time;
		}
		this.mSprite.AnimatorComponent.SetBool("Stand", true);
		base.EnterState();
	}

	public override void ExitState()
	{
		if (null == this.mSprite.AnimatorComponent)
		{
			return;
		}
		if (this.bHasClipRelax)
		{
			this.mSprite.AnimatorComponent.SetBool("Relax", false);
		}
		base.ExitState();
	}

	public override void Update()
	{
		if (null == this.mSprite.AnimatorComponent)
		{
			return;
		}
		if (!this.bHasClipRelax)
		{
			return;
		}
		if (Time.time > this.mEndTime)
		{
			if (Random.Range(0, 3) == 0 && this.bHasClipRelax)
			{
				if (this.mSprite.AnimatorComponent.GetCurrentAnimatorStateInfo(0).IsName("Relax"))
				{
					this.mSprite.AnimatorComponent.SetBool("Relax", false);
					this.mSprite.AnimatorComponent.SetBool("Stand", true);
				}
				else
				{
					this.mSprite.AnimatorComponent.SetBool("Stand", false);
					this.mSprite.AnimatorComponent.SetBool("Relax", true);
				}
			}
			else
			{
				if (this.bHasClipRelax)
				{
					this.mSprite.AnimatorComponent.SetBool("Relax", false);
				}
				this.mSprite.AnimatorComponent.SetBool("Stand", true);
			}
			this.mEndTime = this.clipTime + Time.time;
		}
		base.Update();
	}

	private float mEndTime;

	private float clipTime;

	private bool bHasClipRelax;
}
