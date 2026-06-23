using System;
using UnityEngine;

namespace UnityStandardAssets.Utility
{
	[RequireComponent(typeof(GUIText))]
	public class FPSCounter : MonoBehaviour
	{
		private void Start()
		{
			this.m_FpsNextPeriod = Time.realtimeSinceStartup + 0.5f;
			this.m_GuiText = base.GetComponent<GUIText>();
		}

		private void Update()
		{
			this.m_FpsAccumulator++;
			if (Time.realtimeSinceStartup > this.m_FpsNextPeriod)
			{
				this.m_CurrentFps = (int)((float)this.m_FpsAccumulator / 0.5f);
				this.m_FpsAccumulator = 0;
				this.m_FpsNextPeriod += 0.5f;
				this.m_GuiText.text = string.Format("{0} FPS", this.m_CurrentFps);
			}
		}

		private const float fpsMeasurePeriod = 0.5f;

		private const string display = "{0} FPS";

		private int m_FpsAccumulator;

		private float m_FpsNextPeriod;

		private int m_CurrentFps;

		private GUIText m_GuiText;
	}
}
