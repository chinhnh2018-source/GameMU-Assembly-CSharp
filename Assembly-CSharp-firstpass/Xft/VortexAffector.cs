using System;
using UnityEngine;

namespace Xft
{
	public class VortexAffector : Affector
	{
		public VortexAffector(Transform obj, MAGTYPE mtype, float mag, AnimationCurve vortexCurve, Vector3 dir, bool inhRot, EffectNode node) : base(node, AFFECTORTYPE.VortexAffector)
		{
			this.VortexCurve = vortexCurve;
			this.Direction = dir;
			this.InheritRotation = inhRot;
			this.VortexObj = obj;
			this.MType = mtype;
			this.Magnitude = mag;
			if (node.Owner.IsRandomVortexDir)
			{
				this.Direction.x = Random.Range(-1f, 1f);
				this.Direction.y = Random.Range(-1f, 1f);
				this.Direction.z = Random.Range(-1f, 1f);
			}
			this.Direction.Normalize();
			this.IsFirst = true;
		}

		public override void Reset()
		{
			this.IsFirst = true;
			this.OriginalRadius = 0f;
		}

		public override void Update(float deltaTime)
		{
			Vector3 vector = this.Node.GetOriginalPos() - this.VortexObj.position;
			Vector3 vector2 = this.Direction;
			if (this.InheritRotation)
			{
				vector2 = this.Node.Owner.ClientTransform.rotation * vector2;
			}
			if (this.IsFirst)
			{
				this.IsFirst = false;
				this.OriginalRadius = (vector - Vector3.Project(vector, vector2)).magnitude;
			}
			float sqrMagnitude = vector.sqrMagnitude;
			if (sqrMagnitude < 1E-06f)
			{
				return;
			}
			if (!this.Node.Owner.UseVortexMaxDistance || sqrMagnitude <= this.Node.Owner.VortexMaxDistance * this.Node.Owner.VortexMaxDistance)
			{
				float num = Vector3.Dot(vector2, vector);
				vector -= num * vector2;
				Vector3 vector3 = Vector3.zero;
				if (vector == Vector3.zero)
				{
					vector3 = vector;
				}
				else
				{
					vector3 = Vector3.Cross(vector2, vector).normalized;
				}
				float elapsedTime = this.Node.GetElapsedTime();
				float num2;
				if (this.MType == MAGTYPE.Curve)
				{
					num2 = this.VortexCurve.Evaluate(elapsedTime);
				}
				else
				{
					num2 = this.Magnitude;
				}
				if (this.Node.Owner.VortexAttenuation < 0.0001f)
				{
					vector3 *= num2 * deltaTime;
				}
				else
				{
					vector3 *= num2 * deltaTime / Mathf.Pow(Mathf.Sqrt(sqrMagnitude), this.Node.Owner.VortexAttenuation);
				}
				if (this.Node.Owner.IsVortexAccelerate)
				{
					this.Node.Velocity += vector3;
				}
				else if (this.Node.Owner.IsFixedCircle)
				{
					Vector3 vector4 = this.Node.GetOriginalPos() + vector3 - this.VortexObj.position;
					Vector3 vector5 = Vector3.Project(vector4, vector2);
					Vector3 vector6 = vector4 - vector5;
					if (this.Node.Owner.SyncClient)
					{
						this.Node.Position = vector6.normalized * this.OriginalRadius + vector5;
					}
					else
					{
						this.Node.Position = this.Node.GetRealClientPos() + vector6.normalized * this.OriginalRadius + vector5;
					}
				}
				else
				{
					this.Node.Position += vector3;
				}
			}
		}

		private AnimationCurve VortexCurve;

		protected Vector3 Direction;

		protected Transform VortexObj;

		protected MAGTYPE MType;

		protected bool InheritRotation;

		private float Magnitude;

		protected bool IsFirst = true;

		protected float OriginalRadius;
	}
}
