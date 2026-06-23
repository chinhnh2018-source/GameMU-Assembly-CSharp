using System;
using UnityEngine;

namespace Xft
{
	public class GravityAffector : Affector
	{
		public GravityAffector(Transform obj, GAFTTYPE gtype, MAGTYPE mtype, bool isacc, Vector3 dir, float mag, AnimationCurve curve, EffectNode node) : base(node, AFFECTORTYPE.GravityAffector)
		{
			this.GType = gtype;
			this.MType = mtype;
			this.Magnitude = mag;
			this.MagCurve = curve;
			this.Dir = dir;
			this.Dir.Normalize();
			this.GravityObj = obj;
			this.IsAccelerate = isacc;
		}

		public void SetAttraction(Transform goal)
		{
			this.GravityObj = goal;
		}

		public override void Update(float deltaTime)
		{
			float num;
			if (this.MType == MAGTYPE.Fixed)
			{
				num = this.Magnitude;
			}
			else
			{
				num = this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			if (this.GType == GAFTTYPE.Planar)
			{
				Vector3 vector = this.Node.Owner.ClientTransform.rotation * this.Dir;
				if (this.IsAccelerate)
				{
					this.Node.Velocity += vector * num * deltaTime;
				}
				else
				{
					this.Node.Position += vector * num * deltaTime;
				}
			}
			else if (this.GType == GAFTTYPE.Spherical)
			{
				Vector3 vector2 = this.GravityObj.position - this.Node.GetOriginalPos();
				if (this.IsAccelerate)
				{
					this.Node.Velocity += vector2 * num * deltaTime;
				}
				else
				{
					this.Node.Position += vector2.normalized * num * deltaTime;
				}
			}
		}

		protected GAFTTYPE GType;

		protected MAGTYPE MType;

		protected float Magnitude;

		protected AnimationCurve MagCurve;

		protected Vector3 Dir;

		protected Transform GravityObj;

		protected bool IsAccelerate = true;
	}
}
