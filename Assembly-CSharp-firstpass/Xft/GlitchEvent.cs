using System;
using UnityEngine;

namespace Xft
{
	public class GlitchEvent : CameraEffectEvent
	{
		public GlitchEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.Glitch, owner)
		{
			this.m_random = new WaveRandom();
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_glitchComp = base.MyCamera.gameObject.GetComponent<XftGlitch>();
			if (this.m_glitchComp == null)
			{
				this.m_glitchComp = base.MyCamera.gameObject.AddComponent<XftGlitch>();
			}
			this.m_glitchComp.Init(this.m_owner.GlitchShader, this.m_owner.GlitchMask);
			this.m_glitchComp.enabled = flag;
		}

		public override void Initialize()
		{
			this.ToggleCameraComponent(false);
			this.m_supported = this.m_glitchComp.CheckSupport();
			if (!this.m_supported)
			{
				Debug.LogWarning("can't support Image Effect: Glitch on this device!");
			}
			this.m_random.Reset();
		}

		public override void Reset()
		{
			base.Reset();
			this.m_glitchComp.enabled = false;
			this.m_elapsedTime = 0f;
			base.ResetMyCamera();
			this.m_random.Reset();
		}

		public override void Update(float deltaTime)
		{
			if (!this.m_supported)
			{
				this.m_owner.enabled = false;
				return;
			}
			this.m_elapsedTime += deltaTime;
			this.m_glitchComp.Offset = this.m_random.GetRandom(this.m_owner.MinAmp, this.m_owner.MaxAmp, this.m_owner.MinRand, this.m_owner.MaxRand, this.m_owner.WaveLen);
			this.m_glitchComp.enabled = true;
		}

		protected XftGlitch m_glitchComp;

		protected bool m_supported = true;

		protected WaveRandom m_random;
	}
}
