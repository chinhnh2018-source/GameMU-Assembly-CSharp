using System;
using UnityEngine;

namespace Xft
{
	public class RotateAffector : Affector
	{
		public RotateAffector(RSTYPE type, EffectNode node) : base(node, AFFECTORTYPE.RotateAffector)
		{
			this.RType = type;
		}

		public RotateAffector(float delta, EffectNode node) : base(node, AFFECTORTYPE.RotateAffector)
		{
			this.RType = RSTYPE.SIMPLE;
			this.Delta = delta;
		}

		public override void Reset()
		{
			this.IsFirst = true;
		}

		public override void Update(float deltaTime)
		{
			if (this.IsFirst)
			{
				if (this.RType == RSTYPE.RANDOM)
				{
					this.Delta = Random.Range(this.Node.Owner.RotateSpeedMin, this.Node.Owner.RotateSpeedMax);
				}
				else
				{
					this.Delta = this.Node.Owner.DeltaRot;
				}
				this.IsFirst = false;
			}
			float elapsedTime = this.Node.GetElapsedTime();
			if (this.RType == RSTYPE.CURVE)
			{
				this.Node.RotateAngle = (float)((int)this.Node.Owner.RotateCurve.Evaluate(elapsedTime));
			}
			else if (this.RType == RSTYPE.SIMPLE)
			{
				float rotateAngle = this.Node.RotateAngle + this.Delta * deltaTime;
				this.Node.RotateAngle = rotateAngle;
			}
			else if (this.RType == RSTYPE.RANDOM)
			{
				this.Node.RotateAngle = this.Node.RotateAngle + this.Delta * deltaTime;
			}
			else
			{
				float num = this.Node.Owner.RotateCurveTime;
				if (num < 0f)
				{
					num = this.Node.GetLifeTime();
				}
				float num2 = elapsedTime / num;
				if (num2 > 1f)
				{
					if (this.Node.Owner.RotateCurveWrap == WRAP_TYPE.CLAMP)
					{
						num2 = 1f;
					}
					else if (this.Node.Owner.RotateCurveWrap == WRAP_TYPE.LOOP)
					{
						int num3 = Mathf.FloorToInt(num2);
						num2 -= (float)num3;
					}
					else
					{
						int num4 = Mathf.CeilToInt(num2);
						int num5 = Mathf.FloorToInt(num2);
						if (num4 % 2 == 0)
						{
							num2 = (float)num4 - num2;
						}
						else
						{
							num2 -= (float)num5;
						}
					}
				}
				this.Node.RotateAngle = (float)((int)(this.Node.Owner.RotateCurve01.Evaluate(num2) * this.Node.Owner.RotateCurveMaxValue));
			}
		}

		protected RSTYPE RType;

		protected float Delta;

		protected bool IsFirst = true;
	}
}
