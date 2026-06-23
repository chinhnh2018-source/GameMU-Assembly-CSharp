using System;
using UnityEngine;

namespace Xft
{
	public class UVAffector : Affector
	{
		public UVAffector(UVAnimation frame, float time, EffectNode node, bool randomStart) : base(node, AFFECTORTYPE.UVAffector)
		{
			this.Frames = frame;
			this.UVTime = time;
			this.RandomStartFrame = randomStart;
			if (this.RandomStartFrame)
			{
				this.Frames.curFrame = Random.Range(0, this.Frames.frames.Length - 1);
			}
		}

		public override void Reset()
		{
			this.ElapsedTime = 0f;
			this.FirstUpdate = true;
			this.Frames.curFrame = 0;
			this.Frames.numLoops = 0;
			if (this.RandomStartFrame)
			{
				this.Frames.curFrame = Random.Range(0, this.Frames.frames.Length - 1);
			}
		}

		public override void Update(float deltaTime)
		{
			this.ElapsedTime += deltaTime;
			float num;
			if (this.UVTime <= 0f)
			{
				num = this.Node.GetLifeTime() / (float)this.Frames.frames.Length;
			}
			else
			{
				num = this.UVTime / (float)this.Frames.frames.Length;
			}
			if (this.ElapsedTime >= num || this.FirstUpdate)
			{
				Vector2 zero = Vector2.zero;
				Vector2 zero2 = Vector2.zero;
				this.Frames.GetNextFrame(ref zero, ref zero2);
				if (this.Node.Owner.RenderType == 2 || this.Node.Owner.RenderType == 3)
				{
					zero.y = 1f - zero.y;
					zero2.y = -zero2.y;
				}
				this.Node.LowerLeftUV = zero;
				this.Node.UVDimensions = zero2;
				this.ElapsedTime -= num;
			}
			this.FirstUpdate = false;
		}

		protected UVAnimation Frames;

		protected float ElapsedTime;

		protected float UVTime;

		protected bool RandomStartFrame;

		protected bool FirstUpdate = true;
	}
}
