using System;

namespace Xft
{
	public class CameraEffectEvent : XftEvent
	{
		public CameraEffectEvent(CameraEffectEvent.EType etype, XftEventComponent owner) : base(XEventType.CameraEffect, owner)
		{
			this.m_effectType = etype;
		}

		public virtual void ToggleCameraComponent(bool flag)
		{
		}

		protected CameraEffectEvent.EType m_effectType;

		public enum EType
		{
			RadialBlur,
			RadialBlurMask,
			Glow,
			GlowPerObj,
			ColorInverse,
			Glitch
		}
	}
}
