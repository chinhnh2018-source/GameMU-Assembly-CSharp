using System;

namespace Xft
{
	public class Affector
	{
		public Affector(EffectNode node, AFFECTORTYPE type)
		{
			this.Node = node;
			this.Type = type;
		}

		public virtual void Update(float deltaTime)
		{
		}

		public virtual void Reset()
		{
		}

		public EffectNode Node;

		public AFFECTORTYPE Type;
	}
}
