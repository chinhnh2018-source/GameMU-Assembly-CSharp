using System;
using HSGameEngine.GameEngine.Sprite;
using UnityEngine;

public class NpcStateBase
{
	public NpcStateBase(GSprite sprite)
	{
		this.mSprite = sprite;
	}

	public virtual SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.None;
		}
	}

	public virtual void EnterState()
	{
	}

	public virtual void Update()
	{
	}

	public virtual void ExitState()
	{
	}

	public bool HaveClip(Animator anim, string clipName)
	{
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			AnimationClip[] animationClips = anim.runtimeAnimatorController.animationClips;
			for (int i = 0; i < animationClips.Length; i++)
			{
				if (animationClips[i].name == clipName)
				{
					return true;
				}
			}
		}
		return false;
	}

	public float GetClipLength(Animator anim, string clipName)
	{
		if (anim != null && anim.runtimeAnimatorController != null)
		{
			float num = anim.GetFloat(clipName + "Time");
			if (num <= 0f)
			{
				num = 1f;
			}
			return num;
		}
		return 1f;
	}

	protected GSprite mSprite;
}
