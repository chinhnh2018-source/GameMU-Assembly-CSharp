using System;
using UnityEngine;

namespace Xft
{
	public class TurbulenceFieldAffector : Affector
	{
		public TurbulenceFieldAffector(Transform obj, MAGTYPE mtype, float mag, AnimationCurve curve, float atten, bool useMax, float maxDist, EffectNode node) : base(node, AFFECTORTYPE.TurbulenceAffector)
		{
			this.TurbulenceObj = obj;
			this.MType = mtype;
			this.Magnitude = mag;
			this.MagCurve = curve;
			this.UseMaxDistance = useMax;
			this.MaxDistance = maxDist;
			this.MaxDistanceSqr = this.MaxDistance * this.MaxDistance;
		}

		protected void UpateNoAttenuation(float deltaTime)
		{
			float sqrMagnitude = (this.Node.GetOriginalPos() - this.TurbulenceObj.position).sqrMagnitude;
			float num;
			if (this.MType == MAGTYPE.Fixed)
			{
				num = this.Magnitude;
			}
			else
			{
				num = this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			if (!this.UseMaxDistance || sqrMagnitude <= this.MaxDistanceSqr)
			{
				Vector3 vector;
				vector.x = Random.Range(-1f, 1f);
				vector.y = Random.Range(-1f, 1f);
				vector.z = Random.Range(-1f, 1f);
				vector *= num;
				this.Node.Velocity += vector;
			}
		}

		public override void Update(float deltaTime)
		{
			if ((double)this.Attenuation < 1E-06)
			{
				this.UpateNoAttenuation(deltaTime);
				return;
			}
			float magnitude = (this.Node.GetOriginalPos() - this.TurbulenceObj.position).magnitude;
			float num;
			if (this.MType == MAGTYPE.Fixed)
			{
				num = this.Magnitude;
			}
			else
			{
				num = this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			if (!this.UseMaxDistance || magnitude <= this.MaxDistance)
			{
				Vector3 vector;
				vector.x = Random.Range(-1f, 1f);
				vector.y = Random.Range(-1f, 1f);
				vector.z = Random.Range(-1f, 1f);
				vector *= num / (1f + magnitude * this.Attenuation);
				this.Node.Velocity += vector;
			}
		}

		protected MAGTYPE MType;

		protected float Magnitude;

		protected AnimationCurve MagCurve;

		protected Transform TurbulenceObj;

		protected float Attenuation;

		protected bool UseMaxDistance;

		protected float MaxDistance;

		protected float MaxDistanceSqr;
	}
}
