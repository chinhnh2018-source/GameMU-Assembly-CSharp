using System;
using UnityEngine;

public class UnderWaterEffect : MonoBehaviour
{
	private void OnTriggerStay(Collider hit)
	{
		if (hit.gameObject.CompareTag("WaterColler"))
		{
			RenderSettings.fog = true;
			RenderSettings.fogColor = new Color(0f, 0.4f, 0.7f, 0.6f);
			RenderSettings.fogDensity = 0.04f;
			RenderSettings.skybox = this.noSkybox;
		}
	}

	private void OnTriggerExit(Collider hit)
	{
		if (hit.gameObject.CompareTag("WaterColler"))
		{
			RenderSettings.fog = this.defaultFog;
			RenderSettings.fogColor = this.defaultFogColor;
			RenderSettings.fogDensity = this.defaultFogDensity;
			RenderSettings.skybox = this.defaultSkybox;
		}
	}

	private bool defaultFog = RenderSettings.fog;

	private Color defaultFogColor = RenderSettings.fogColor;

	private float defaultFogDensity = RenderSettings.fogDensity;

	private Material defaultSkybox = RenderSettings.skybox;

	private Material noSkybox;
}
