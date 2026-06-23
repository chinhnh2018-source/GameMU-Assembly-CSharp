using System;
using UnityEngine;

namespace Xft
{
	public class AirFieldAffector : Affector
	{
		public AirFieldAffector(Transform airObj, Vector3 dir, MAGTYPE mtype, float mag, AnimationCurve curve, float atten, bool useMaxdist, float maxDist, bool enableSpread, float spread, float inhV, bool inhRot, EffectNode node) : base(node, AFFECTORTYPE.AirFieldAffector)
		{
			this.AirObj = airObj;
			this.Direction = dir.normalized;
			this.MType = mtype;
			this.Magnitude = mag;
			this.MagCurve = curve;
			this.Attenuation = atten;
			this.UseMaxDistance = useMaxdist;
			this.MaxDistance = maxDist;
			this.MaxDistanceSqr = this.MaxDistance * this.MaxDistance;
			this.EnableSpread = enableSpread;
			this.Spread = spread;
			this.InheritVelocity = inhV;
			this.InheritRotation = inhRot;
			this.LastFieldPos = this.AirObj.position;
		}

		public override void Reset()
		{
			this.LastFieldPos = this.AirObj.position;
		}

		public override void Update(float deltaTime)
		{
			Vector3 vector;
			if (this.InheritRotation)
			{
				vector = this.AirObj.rotation * this.Direction;
			}
			else
			{
				vector = this.Direction;
			}
			Vector3 vector2 = Vector3.zero;
			vector2 = (this.AirObj.position - this.LastFieldPos) * this.InheritVelocity / deltaTime;
			this.LastFieldPos = this.AirObj.position;
			float num;
			if (this.MType == MAGTYPE.Fixed)
			{
				num = this.Magnitude;
			}
			else
			{
				num = this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			vector2 += vector * num;
			float magnitude = vector2.magnitude;
			float num2 = (!this.EnableSpread) ? 0f : Mathf.Cos(1.57079637f * this.Spread);
			Vector3 vector3 = this.Node.GetOriginalPos() - this.AirObj.position;
			float sqrMagnitude = vector3.sqrMagnitude;
			if (!this.UseMaxDistance || sqrMagnitude < this.MaxDistanceSqr)
			{
				Vector3 vector4 = vector2;
				if (this.EnableSpread)
				{
					vector4 = vector3.normalized;
					if (Vector3.Dot(vector2, vector4) < num2)
					{
						return;
					}
					vector4 *= magnitude;
				}
				Vector3 vector5 = this.Node.Velocity;
				if (Vector3.Dot(vector4, vector5 - vector4) < 0f)
				{
					float num3 = deltaTime;
					if (this.UseMaxDistance && this.Attenuation < 1E-06f)
					{
						num3 *= Mathf.Pow(1f - Mathf.Sqrt(sqrMagnitude) / this.MaxDistance, this.Attenuation);
					}
					vector5 += vector4 * num3;
					this.Node.Velocity = vector5;
				}
			}
		}

		protected Transform AirObj;

		protected Vector3 Direction;

		protected MAGTYPE MType;

		protected float Magnitude;

		protected AnimationCurve MagCurve;

		protected float Attenuation;

		protected bool UseMaxDistance;

		protected float MaxDistance;

		protected float MaxDistanceSqr;

		protected bool EnableSpread;

		protected float Spread;

		protected float InheritVelocity;

		protected bool InheritRotation;

		protected Vector3 LastFieldPos;
	}
}
