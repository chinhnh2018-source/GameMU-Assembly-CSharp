using System;
using UnityEngine;

namespace Xft
{
	public class ColorInverseEvent : CameraEffectEvent
	{
		public ColorInverseEvent(XftEventComponent owner) : base(CameraEffectEvent.EType.ColorInverse, owner)
		{
		}

		public override void ToggleCameraComponent(bool flag)
		{
			this.m_inverseComp = base.MyCamera.gameObject.GetComponent<XftColorInverse>();
			if (this.m_inverseComp == null)
			{
				this.m_inverseComp = base.MyCamera.gameObject.AddComponent<XftColorInverse>();
			}
			this.m_inverseComp.Init(this.m_owner.ColorInverseShader);
			this.m_inverseComp.enabled = flag;
		}

		public override void Initialize()
		{
			this.ToggleCameraComponent(false);
			this.m_supported = this.m_inverseComp.CheckSupport();
			if (!this.m_supported)
			{
				Debug.LogWarning("can't support Image Effect: ColorInverse on this device!");
			}
		}

		public override void Reset()
		{
			base.Reset();
			this.m_inverseComp.enabled = false;
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
			float strength = this.m_owner.CIStrengthCurve.Evaluate(this.m_elapsedTime);
			this.m_inverseComp.Strength = strength;
			this.m_inverseComp.enabled = true;
		}

		protected XftColorInverse m_inverseComp;

		protected bool m_supported = true;
	}
}
