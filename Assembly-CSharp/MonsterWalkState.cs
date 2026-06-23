using System;
using HSGameEngine.GameEngine.Sprite;

public class MonsterWalkState : SpriteStateBase
{
	public MonsterWalkState(GSprite monster) : base(monster)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Walk;
		}
	}

	public override void EnterState()
	{
		this.mSprite.AnimatorComponent.SetBool("Walk", true);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ExitState()
	{
		this.mSprite.AnimatorComponent.SetBool("Walk", false);
		base.ExitState();
	}
}
