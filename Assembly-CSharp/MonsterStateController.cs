using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Sprite;

public class MonsterStateController
{
	public MonsterStateController(GSprite sprite)
	{
		this.mSprite = sprite;
		this.mStates = new Dictionary<int, SpriteStateBase>();
		this.mStates.Add(2, new MonsterAttackState(this.mSprite));
		this.mStates.Add(0, new MonsterStandState(this.mSprite));
		this.mStates.Add(1, new MonsterWalkState(this.mSprite));
		this.mStates.Add(5, new MonsterRunState(this.mSprite));
		this.mStates.Add(4, new MonsterDeathState(this.mSprite));
		this.mStates.Add(3, new MonsterInjuredState(this.mSprite));
	}

	public void SetState(SpriteAnimState state)
	{
		if ((this.mCurrentState != null && this.mCurrentState.State == SpriteAnimState.Death) || this.mTargetState == SpriteAnimState.Death)
		{
			return;
		}
		this.mTargetState = state;
	}

	public void ExitState()
	{
		if (!this.mSprite.AnimatorComponent)
		{
			return;
		}
		if (this.mCurrentState != null)
		{
			this.mCurrentState.ExitState();
		}
	}

	private void InitController()
	{
		if (null != this.mSprite.AnimatorComponent)
		{
			if (this.mSprite.AnimatorComponent.GetBool("Die"))
			{
				this.mSprite.AnimatorComponent.SetBool("Die", false);
			}
			this.mSprite.AnimatorComponent.Play("Stand", 0, 0f);
			this.initFinish = true;
		}
	}

	public void Update()
	{
		if (!this.mSprite.AnimatorComponent)
		{
			this.mCurrentState = null;
			return;
		}
		if (!this.initFinish)
		{
			this.InitController();
		}
		if (this.mCurrentState != null)
		{
			this.mCurrentState.Update();
		}
		if (this.mTargetState == SpriteAnimState.Injured)
		{
			if (this.mCurrentState == null || (this.mCurrentState.State != SpriteAnimState.Death && this.mCurrentState.State != SpriteAnimState.Attack))
			{
				if (this.mCurrentState != null)
				{
					this.mCurrentState.ExitState();
				}
				this.mCurrentState = this.mStates[3];
				this.mCurrentState.EnterState();
			}
			this.mTargetState = SpriteAnimState.None;
		}
		else if (this.mTargetState != SpriteAnimState.None && (this.mCurrentState == null || this.mTargetState != this.mCurrentState.State))
		{
			if (this.mCurrentState != null)
			{
				this.mCurrentState.ExitState();
			}
			SpriteStateBase spriteStateBase;
			if (!this.mStates.TryGetValue((int)this.mTargetState, ref spriteStateBase))
			{
				MUDebug.LogError<string>(new string[]
				{
					"Monster Animation State '" + this.mTargetState + "' is not defined"
				});
				spriteStateBase = this.mStates[0];
			}
			this.mCurrentState = spriteStateBase;
			this.mCurrentState.EnterState();
			this.mTargetState = SpriteAnimState.None;
		}
	}

	private Dictionary<int, SpriteStateBase> mStates;

	private SpriteStateBase mCurrentState;

	private SpriteAnimState mTargetState = SpriteAnimState.None;

	private GSprite mSprite;

	private bool initFinish;
}
