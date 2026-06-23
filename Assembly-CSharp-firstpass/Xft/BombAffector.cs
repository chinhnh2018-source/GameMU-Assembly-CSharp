using System;
using UnityEngine;

namespace Xft
{
	public class BombAffector : Affector
	{
		public BombAffector(Transform obj, BOMBTYPE gtype, MAGTYPE mtype, BOMBDECAYTYPE dtype, float mag, AnimationCurve curve, float decay, Vector3 axis, EffectNode node) : base(node, AFFECTORTYPE.BombAffector)
		{
			this.BombType = gtype;
			this.MType = mtype;
			this.DecayType = dtype;
			this.Magnitude = mag;
			this.MagCurve = curve;
			this.Decay = decay;
			this.BombAxis = axis;
			this.BombAxis.Normalize();
			this.BombObj = obj;
		}

		public override void Reset()
		{
			this.ElapsedTime = 0f;
		}

		public override void Update(float deltaTime)
		{
			deltaTime = 0.01666f;
			float num;
			if (this.MType == MAGTYPE.Fixed)
			{
				num = this.Magnitude;
			}
			else
			{
				num = this.MagCurve.Evaluate(this.Node.GetElapsedTime());
			}
			Vector3 vector = this.BombObj.rotation * this.BombAxis;
			Vector3 vector2 = this.Node.GetOriginalPos() - this.BombObj.position;
			float num2 = vector2.magnitude;
			Vector3 vector3 = Vector3.zero;
			if (this.DecayType == BOMBDECAYTYPE.None || num2 <= this.Decay)
			{
				switch (this.BombType)
				{
				case BOMBTYPE.Planar:
					num2 = Vector3.Dot(vector, vector2);
					if (num2 < 0f)
					{
						num2 = -num2;
						vector3 = -vector;
					}
					else
					{
						vector3 = vector;
					}
					break;
				case BOMBTYPE.Spherical:
					vector3 = vector2 / num2;
					break;
				case BOMBTYPE.Cylindrical:
					num2 = Vector3.Dot(vector, vector2);
					vector3 = vector2 - num2 * vector;
					num2 = vector3.magnitude;
					if (num2 != 0f)
					{
						vector3 /= num2;
					}
					break;
				default:
					Debug.LogError("wrong bomb type!");
					break;
				}
				float num3 = 1f;
				if (this.DecayType == BOMBDECAYTYPE.Linear)
				{
					num3 = (this.Decay - num2) / this.Decay;
				}
				else if (this.DecayType == BOMBDECAYTYPE.Exponential)
				{
					num3 = Mathf.Exp(-num2 / this.Decay);
				}
				this.ElapsedTime += deltaTime;
				num /= this.ElapsedTime * this.ElapsedTime;
				this.Node.Velocity += num3 * num * deltaTime * vector3;
			}
		}

		protected BOMBTYPE BombType;

		protected MAGTYPE MType;

		protected BOMBDECAYTYPE DecayType;

		protected float Magnitude;

		protected AnimationCurve MagCurve;

		protected float Decay;

		protected Vector3 BombAxis;

		protected Transform BombObj;

		protected float ElapsedTime;
	}
}
