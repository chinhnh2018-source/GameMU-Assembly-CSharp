using System;
using HSGameEngine.GameEngine.Sprite;

public class SpriteStateBase
{
	public SpriteStateBase(GSprite sprite)
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

	protected GSprite mSprite;
}
