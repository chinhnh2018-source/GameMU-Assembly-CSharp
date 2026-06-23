using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Sprite;

public class NpcStateController
{
	public NpcStateController(GSprite sprite)
	{
		this.mSprite = sprite;
		this.mStates = new Dictionary<int, NpcStateBase>();
		this.mStates.Add(0, new NpcStandState(sprite));
	}

	public bool SetState(NPCAnimState state)
	{
		this.mStates[(int)state].EnterState();
		this.current = this.mStates[(int)state];
		return true;
	}

	public void Update()
	{
		if (this.current != null)
		{
			this.current.Update();
		}
	}

	private Dictionary<int, NpcStateBase> mStates;

	protected GSprite mSprite;

	private NpcStateBase current;
}
