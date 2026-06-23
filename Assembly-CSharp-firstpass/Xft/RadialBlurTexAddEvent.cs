using System;
using UnityEngine;

namespace Xft
{
	public class RadialBlurTexAddEvent : CameraEffectEvent
	{
		public RadialBlurTexAddEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.RadialBlurMask, owner)
		{
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_radialBlurComp = base.MyCamera.gameObject.GetComponent<XftRadialBlurTexAdd>();
			if (this.m_radialBlurComp == null)
			{
				this.m_radialBlurComp = base.MyCamera.gameObject.AddComponent<XftRadialBlurTexAdd>();
			}
			this.m_radialBlurComp.Init(this.m_owner.RadialBlurTexAddShader);
			this.m_radialBlurComp.enabled = flag;
		}

		public override void Initialize()
		{
			this.ToggleCameraComponent(false);
			this.m_supported = this.m_radialBlurComp.CheckSupport();
			if (!this.m_supported)
			{
				Debug.LogWarning("can't support Image Effect: Radial Blur Mask on this device!");
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.m_radialBlurComp.enabled = false;
			this.m_elapsedTime = 0f;
			base.ResetMyCamera();
		}

		public override void Update(float deltaTime)
		{
			if (!this.m_supported)
			{
				this.m_owner.enabled = false;
				return;
			}
			this.m_elapsedTime += deltaTime;
			this.m_radialBlurComp.enabled = true;
			this.m_radialBlurComp.SampleDist = this.m_owner.RBMaskSampleDist;
			float sampleStrength;
			if (this.m_owner.RBMaskStrengthType == MAGTYPE.Fixed)
			{
				sampleStrength = this.m_owner.RBMaskSampleStrength;
			}
			else
			{
				sampleStrength = this.m_owner.RBMaskSampleStrengthCurve.Evaluate(this.m_elapsedTime);
			}
			this.m_radialBlurComp.SampleStrength = sampleStrength;
			this.m_radialBlurComp.Mask = this.m_owner.RadialBlurMask;
		}

		protected XftRadialBlurTexAdd m_radialBlurComp;

		protected bool m_supported = true;
	}
}
