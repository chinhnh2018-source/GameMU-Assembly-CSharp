using System;
using UnityEngine;

namespace Xft
{
	public class RadialBlurEvent : CameraEffectEvent
	{
		public RadialBlurEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.RadialBlur, owner)
		{
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_radialBlurComp = base.MyCamera.gameObject.GetComponent<XftRadialBlur>();
			if (this.m_radialBlurComp == null)
			{
				this.m_radialBlurComp = base.MyCamera.gameObject.AddComponent<XftRadialBlur>();
			}
			this.m_radialBlurComp.Init(this.m_owner.RadialBlurShader);
			this.m_radialBlurComp.enabled = flag;
		}

		public override void Initialize()
		{
			this.ToggleCameraComponent(false);
			this.m_supported = this.m_radialBlurComp.CheckSupport();
			if (!this.m_supported)
			{
				Debug.LogWarning("can't support Image Effect: Radial Blur on this device!");
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.m_radialBlurComp.enabled = false;
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
			Vector3 center = base.MyCamera.WorldToScreenPoint(this.m_owner.RadialBlurObj.position);
			this.m_radialBlurComp.Center = center;
			float num;
			if (this.m_owner.RBStrengthType == MAGTYPE.Fixed)
			{
				num = this.m_owner.RBSampleStrength;
			}
			else
			{
				num = this.m_owner.RBSampleStrengthCurve.Evaluate(this.m_elapsedTime);
			}
			Mathf.Clamp(num, 0.05f, 99f);
			this.m_radialBlurComp.SampleStrength = num;
			this.m_radialBlurComp.SampleDist = this.m_owner.RBSampleDist;
		}

		protected XftRadialBlur m_radialBlurComp;

		protected bool m_supported = true;
	}
}
