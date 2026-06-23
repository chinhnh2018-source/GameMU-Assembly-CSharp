using System;
using UnityEngine;

namespace Xft
{
	public class GlowEvent : CameraEffectEvent
	{
		public GlowEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.Glow, owner)
		{
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_glowComp = base.MyCamera.gameObject.GetComponent<XftGlow>();
			if (this.m_glowComp == null)
			{
				this.m_glowComp = base.MyCamera.gameObject.AddComponent<XftGlow>();
			}
			this.m_glowComp.Init(this.m_owner);
			this.m_glowComp.enabled = flag;
		}

		public override void Initialize()
		{
			this.ToggleCameraComponent(false);
			this.m_elapsedTime = 0f;
			this.m_supported = this.m_glowComp.CheckSupport();
			if (!this.m_supported)
			{
				Debug.LogWarning("can't support Image Effect: Glow on this device!");
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.m_glowComp.enabled = false;
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
			Color glowTint = Color.clear;
			float num = this.m_owner.ColorCurve.Evaluate(this.m_elapsedTime);
			glowTint = Color.Lerp(this.m_owner.GlowColorStart, this.m_owner.GlowColorEnd, num);
			this.m_glowComp.glowTint = glowTint;
			this.m_glowComp.enabled = true;
		}

		protected XftGlow m_glowComp;

		protected bool m_supported = true;
	}
}
