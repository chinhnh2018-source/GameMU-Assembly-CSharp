using System;
using UnityEngine;

namespace Xft
{
	public class ScaleAffector : Affector
	{
		public ScaleAffector(RSTYPE type, EffectNode node) : base(node, AFFECTORTYPE.ScaleAffector)
		{
			this.SType = type;
		}

		public ScaleAffector(float x, float y, EffectNode node) : base(node, AFFECTORTYPE.ScaleAffector)
		{
			this.SType = RSTYPE.SIMPLE;
			this.DeltaX = x;
			this.DeltaY = y;
		}

		public override void Reset()
		{
			this.IsFirst = true;
		}

		public override void Update(float deltaTime)
		{
			if (this.IsFirst)
			{
				if (this.SType == RSTYPE.RANDOM)
				{
					this.DeltaX = Random.Range(this.Node.Owner.DeltaScaleX, this.Node.Owner.DeltaScaleXMax);
					this.DeltaY = Random.Range(this.Node.Owner.DeltaScaleY, this.Node.Owner.DeltaScaleYMax);
				}
				else
				{
					this.DeltaX = this.Node.Owner.DeltaScaleX;
					this.DeltaY = this.Node.Owner.DeltaScaleY;
				}
				this.IsFirst = false;
			}
			float elapsedTime = this.Node.GetElapsedTime();
			if (this.SType == RSTYPE.CURVE)
			{
				if (this.Node.Owner.UseSameScaleCurve)
				{
					float num = this.Node.Owner.ScaleXCurve.Evaluate(elapsedTime);
					this.Node.Scale.x = num;
					this.Node.Scale.y = num;
				}
				else
				{
					this.Node.Scale.x = this.Node.Owner.ScaleXCurve.Evaluate(elapsedTime);
					this.Node.Scale.y = this.Node.Owner.ScaleYCurve.Evaluate(elapsedTime);
				}
			}
			else if (this.SType == RSTYPE.RANDOM)
			{
				float num2 = this.Node.Scale.x + this.DeltaX * deltaTime;
				float num3 = this.Node.Scale.y + this.DeltaY * deltaTime;
				if (num2 > 0f)
				{
					this.Node.Scale.x = num2;
				}
				if (num3 > 0f)
				{
					this.Node.Scale.y = num3;
				}
			}
			else if (this.SType == RSTYPE.CURVE01)
			{
				float num4 = this.Node.Owner.ScaleCurveTime;
				if (num4 < 0f)
				{
					num4 = this.Node.GetLifeTime();
				}
				float num5 = elapsedTime / num4;
				if (num5 > 1f)
				{
					if (this.Node.Owner.ScaleWrapMode == WRAP_TYPE.CLAMP)
					{
						num5 = 1f;
					}
					else if (this.Node.Owner.ScaleWrapMode == WRAP_TYPE.LOOP)
					{
						int num6 = Mathf.FloorToInt(num5);
						num5 -= (float)num6;
					}
					else
					{
						int num7 = Mathf.CeilToInt(num5);
						int num8 = Mathf.FloorToInt(num5);
						if (num7 % 2 == 0)
						{
							num5 = (float)num7 - num5;
						}
						else
						{
							num5 -= (float)num8;
						}
					}
				}
				if (this.Node.Owner.UseSameScaleCurve)
				{
					float num9 = this.Node.Owner.ScaleXCurveNew.Evaluate(num5);
					num9 *= this.Node.Owner.MaxScaleCalue;
					this.Node.Scale.x = num9;
					this.Node.Scale.y = num9;
				}
				else
				{
					this.Node.Scale.x = this.Node.Owner.ScaleXCurveNew.Evaluate(num5) * this.Node.Owner.MaxScaleCalue;
					this.Node.Scale.y = this.Node.Owner.ScaleYCurveNew.Evaluate(num5) * this.Node.Owner.MaxScaleCalue;
				}
			}
			else if (this.SType == RSTYPE.SIMPLE)
			{
				float num10 = this.Node.Scale.x + this.DeltaX * deltaTime;
				float num11 = this.Node.Scale.y + this.DeltaY * deltaTime;
				if (num10 * this.Node.Scale.x > 0f)
				{
					this.Node.Scale.x = num10;
				}
				if (num11 * this.Node.Scale.y > 0f)
				{
					this.Node.Scale.y = num11;
				}
			}
		}

		protected RSTYPE SType;

		protected float DeltaX;

		protected float DeltaY;

		protected bool IsFirst = true;
	}
}
