using System;
using UnityEngine;

namespace Xft
{
	public class JetAffector : Affector
	{
		public JetAffector(float mag, MAGTYPE type, AnimationCurve curve, EffectNode node) : base(node, AFFECTORTYPE.JetAffector)
		{
			this.Mag = mag;
			this.MType = type;
			this.MagCurve = curve;
		}

		public override void Update(float deltaTime)
		{
			Vector3 vector = Vector3.zero;
			if (this.MType == MAGTYPE.Fixed)
			{
				vector = this.Node.Velocity.normalized * this.Mag * deltaTime;
			}
			else
			{
				vector = this.Node.Velocity.normalized * this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			Vector3 vector2 = this.Node.Velocity + vector;
			if (Vector3.Dot(vector2, this.Node.Velocity) <= 0f)
			{
				this.Node.Velocity = Vector3.zero;
				return;
			}
			this.Node.Velocity = vector2;
		}

		protected float Mag;

		protected MAGTYPE MType;

		protected AnimationCurve MagCurve;
	}
}
