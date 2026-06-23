using System;
using UnityEngine;

namespace Xft
{
	public class WaveRandom
	{
		public void Reset()
		{
			this.seeds[0] = Random.Range(1f, 2f);
			this.seeds[1] = Random.Range(1f, 2f);
			this.seeds[2] = Random.Range(1f, 2f);
			this.seed = 0;
		}

		public Vector3 GetRandom(float minAmp, float maxAmp, float minRand, float maxRand, int len)
		{
			float num = maxAmp - minAmp;
			this.seed++;
			if (this.seed >= len)
			{
				this.seed = 0;
			}
			float num2 = 3.14159274f / (float)len * (float)this.seed;
			float num3 = 1.27323949f * num2 - 0.405284733f * num2 * num2;
			float num4 = minAmp + num3 * num;
			float num5 = 6.28318548f;
			for (int i = 0; i < 3; i++)
			{
				if (this.seeds[i] >= 1f)
				{
					this.seeds[i] = this.seeds[i] - 1f;
					this.dSeeds[i] = Random.Range(minRand, maxRand);
				}
				this.seeds[i] += this.dSeeds[i];
				num2 = this.seeds[i] * num5;
				if (num2 > 3.14159274f)
				{
					num2 -= num5;
				}
				if (num2 < 0f)
				{
					num3 = 1.27323949f * num2 + 0.405284733f * num2 * num2;
				}
				else
				{
					num3 = 1.27323949f * num2 - 0.405284733f * num2 * num2;
				}
				this.disp[i] = num3 * num4;
			}
			return this.disp;
		}

		protected int seed;

		protected float[] dSeeds = new float[3];

		protected float[] seeds = new float[3];

		protected Vector3 disp = Vector3.zero;
	}
}
