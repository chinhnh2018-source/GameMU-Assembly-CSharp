using System;
using HSGameEngine.GameEngine.Sprite;

public class MonsterStandState : SpriteStateBase
{
	public MonsterStandState(GSprite monster) : base(monster)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Stand;
		}
	}

	public override void EnterState()
	{
		this.mSprite.AnimatorComponent.SetBool("Stand", true);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ExitState()
	{
		this.mSprite.AnimatorComponent.SetBool("Stand", false);
		base.ExitState();
	}
}
