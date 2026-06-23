using System;
using UnityEngine;

namespace Xft
{
	public class UVRotAffector : Affector
	{
		public UVRotAffector(float rotXSpeed, float rotYSpeed, EffectNode node) : base(node, AFFECTORTYPE.UVRotAffector)
		{
			this.RotXSpeed = rotXSpeed;
			this.RotYSpeed = rotYSpeed;
		}

		public override void Reset()
		{
			this.FirstUpdate = true;
		}

		public override void Update(float deltaTime)
		{
			if (this.FirstUpdate)
			{
				if (this.Node.Owner.RandomStartFrame)
				{
					EffectNode node = this.Node;
					node.LowerLeftUV.x = node.LowerLeftUV.x + Random.Range(-1f, 1f);
					EffectNode node2 = this.Node;
					node2.LowerLeftUV.y = node2.LowerLeftUV.y + Random.Range(-1f, 1f);
				}
				this.FirstUpdate = false;
			}
			Vector2 lowerLeftUV = this.Node.LowerLeftUV;
			lowerLeftUV.x += this.RotXSpeed * deltaTime;
			lowerLeftUV.y += this.RotYSpeed * deltaTime;
			this.Node.LowerLeftUV = lowerLeftUV;
		}

		protected float RotXSpeed;

		protected float RotYSpeed;

		protected bool FirstUpdate = true;
	}
}
