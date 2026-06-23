using System;
using UnityEngine;

namespace Xft
{
	public class GlowPerObjEvent : CameraEffectEvent
	{
		public GlowPerObjEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.GlowPerObj, owner)
		{
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_glowComp = base.MyCamera.gameObject.GetComponent<XftGlowPerObj>();
			if (this.m_glowComp == null)
			{
				this.m_glowComp = base.MyCamera.gameObject.AddComponent<XftGlowPerObj>();
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
			this.ToggleCameraComponent(false);
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

		protected XftGlowPerObj m_glowComp;

		protected bool m_supported = true;
	}
}
