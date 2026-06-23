using System;
using HSGameEngine.GameEngine.Sprite;

public class MonsterRunState : SpriteStateBase
{
	public MonsterRunState(GSprite monster) : base(monster)
	{
	}

	public override SpriteAnimState State
	{
		get
		{
			return SpriteAnimState.Run;
		}
	}

	public override void EnterState()
	{
		this.mSprite.AnimatorComponent.SetBool("Run", true);
	}

	public override void Update()
	{
		base.Update();
	}

	public override void ExitState()
	{
		this.mSprite.AnimatorComponent.SetBool("Run", false);
		base.ExitState();
	}
}
