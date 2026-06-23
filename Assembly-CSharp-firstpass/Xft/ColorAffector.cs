using System;
using UnityEngine;

namespace Xft
{
	public class ColorAffector : Affector
	{
		public ColorAffector(EffectLayer owner, EffectNode node) : base(node, AFFECTORTYPE.ColorAffector)
		{
			this.mOwner = owner;
			if (owner.ColorGradualTimeLength < 0f)
			{
				this.IsNodeLife = true;
			}
			if (this.mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
			{
				this.mRandomKey = Random.Range(0f, 1f);
			}
		}

		public override void Reset()
		{
			this.ElapsedTime = 0f;
			if (this.IsNodeLife && this.mOwner.IsNodeLifeLoop)
			{
				Debug.LogWarning("invalid color gradual time, loop node can't be gradient by 'gradual time':" + this.mOwner.ColorGradualTimeLength);
			}
			if (this.mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
			{
				this.mRandomKey = Random.Range(0f, 1f);
			}
		}

		public override void Update(float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			float num = this.mOwner.ColorGradualTimeLength;
			if (this.IsNodeLife)
			{
				num = this.Node.GetLifeTime();
			}
			if (num <= 0f)
			{
				return;
			}
			float num2;
			if (this.mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.CURVE)
			{
				num2 = this.mOwner.ColorGradualCurve.Evaluate(this.ElapsedTime);
			}
			else
			{
				num2 = this.ElapsedTime / num;
			}
			if (this.mOwner.ColorChangeType == COLOR_CHANGE_TYPE.Random)
			{
				num2 = this.mRandomKey;
			}
			if (num2 > 1f)
			{
				if (this.mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.CLAMP)
				{
					this.Node.Color = this.mOwner.ColorParam.GetGradientColor(1f);
					return;
				}
				if (this.mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.LOOP)
				{
					this.ElapsedTime = 0f;
					return;
				}
				if (this.mOwner.ColorGradualType == COLOR_GRADUAL_TYPE.REVERSE)
				{
					int num3 = Mathf.CeilToInt(num2);
					int num4 = Mathf.FloorToInt(num2);
					if (num3 % 2 == 0)
					{
						num2 = (float)num3 - num2;
					}
					else
					{
						num2 -= (float)num4;
					}
				}
			}
			this.Node.Color = this.mOwner.ColorParam.GetGradientColor(num2);
		}

		protected float ElapsedTime;

		protected bool IsNodeLife;

		protected EffectLayer mOwner;

		protected float mRandomKey;
	}
}
