using System;
using UnityEngine;

namespace Xft
{
	public class TimeScaleEvent : XftEvent
	{
		public TimeScaleEvent(XftEventComponent owner) : base(XEventType.TimeScale, owner)
		{
		}

		public override void Reset()
		{
			base.Reset();
			Time.timeScale = 1f;
		}

		public override void OnBegin()
		{
			base.OnBegin();
			Time.timeScale = this.m_owner.TimeScale;
		}

		public override void Update(float deltaTime)
		{
			this.m_elapsedTime += deltaTime;
			float elapsedTime = this.m_elapsedTime;
			if (elapsedTime / this.m_owner.TimeScale > this.m_owner.TimeScaleDuration)
			{
				Time.timeScale = 1f;
			}
		}
	}
}
