using System;
using System.Collections.Generic;
using GameServer.Core.Executor;

namespace GameServer.Logic.JingJiChang.FSM
{
	public class FinishStateMachine
	{
		public FinishStateMachine(GameClient player, Robot owner)
		{
			this.owner = owner;
			IFSMState value = new AttackState(player, owner, this);
			IFSMState value2 = new DeadState(owner, this);
			IFSMState value3 = new ReturnState(owner, this);
			IFSMState value4 = new NormalState(owner, this);
			this.states.Add(AIState.ATTACK, value);
			this.states.Add(AIState.DEAD, value2);
			this.states.Add(AIState.RETURN, value3);
			this.states.Add(AIState.NORMAL, value4);
			this.currentState = value4;
		}

		public void onUpdate()
		{
			long now = TimeUtil.NOW();
			this.currentState.onUpdate(now);
		}

		public void switchState(AIState state)
		{
			IFSMState ifsmstate = null;
			if (this.states.TryGetValue(state, out ifsmstate))
			{
				if (ifsmstate != this.currentState)
				{
					this.currentState.onEnd();
					this.currentState = ifsmstate;
					ifsmstate.onBegin();
				}
			}
		}

		private Dictionary<AIState, IFSMState> states = new Dictionary<AIState, IFSMState>();

		private Robot owner = null;

		private IFSMState currentState = null;
	}
}
