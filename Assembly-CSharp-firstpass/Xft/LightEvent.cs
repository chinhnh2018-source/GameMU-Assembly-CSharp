using System;
using UnityEngine;

namespace Xft
{
	public class LightEvent : XftEvent
	{
		public LightEvent(XftEventComponent owner) : base(XEventType.Light, owner)
		{
		}

		public override void Initialize()
		{
			if (this.m_owner.LightComp == null)
			{
				Debug.LogWarning("you should assign a light source to Light Event to use it!");
				return;
			}
			this.m_elapsedTime = 0f;
			XffectComponent.SetActive(this.m_owner.LightComp.gameObject, false);
		}

		public override void Reset()
		{
			base.Reset();
			if (this.m_owner.LightComp == null)
			{
				return;
			}
			XffectComponent.SetActive(this.m_owner.LightComp.gameObject, false);
		}

		public override void OnBegin()
		{
			base.OnBegin();
			if (this.m_owner.LightComp != null)
			{
				XffectComponent.SetActive(this.m_owner.LightComp.gameObject, true);
			}
		}

		public override void Update(float deltaTime)
		{
			if (this.m_owner.LightComp == null)
			{
				return;
			}
			this.m_elapsedTime += deltaTime;
			float intensity;
			if (this.m_owner.LightIntensityType == MAGTYPE.Curve && this.m_owner.LightIntensityCurve != null)
			{
				intensity = this.m_owner.LightIntensityCurve.Evaluate(this.m_elapsedTime - this.m_owner.StartTime);
			}
			else
			{
				intensity = this.m_owner.LightIntensity;
			}
			this.m_owner.LightComp.intensity = intensity;
			float range;
			if (this.m_owner.LightRangeType == MAGTYPE.Curve && this.m_owner.LightRangeCurve != null)
			{
				range = this.m_owner.LightRangeCurve.Evaluate(this.m_elapsedTime - this.m_owner.StartTime);
			}
			else
			{
				range = this.m_owner.LightRange;
			}
			this.m_owner.LightComp.range = range;
		}
	}
}
