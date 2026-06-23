using System;
using UnityEngine;

public class SetShadowMap : MonoBehaviour
{
	private void Awake()
	{
		if (this.shadowMaps != null && this.shadowMaps.Length > 0)
		{
			LightmapSettings.lightmapsMode = 1;
			LightmapData[] array = new LightmapData[LightmapSettings.lightmaps.Length];
			for (int i = 0; i < array.Length; i++)
			{
				array[i] = new LightmapData
				{
					lightmapFar = LightmapSettings.lightmaps[i].lightmapFar,
					lightmapNear = ((i >= this.shadowMaps.Length) ? null : this.shadowMaps[i])
				};
			}
			LightmapSettings.lightmaps = array;
		}
		if (this.useCustomAmbientColor)
		{
			this.originAmbient = RenderSettings.ambientLight;
			RenderSettings.ambientLight = this.ambientColor;
		}
	}

	private void OnDestroy()
	{
		RenderSettings.ambientLight = this.originAmbient;
	}

	public Texture2D[] shadowMaps;

	public bool useCustomAmbientColor;

	public Color ambientColor;

	private Color originAmbient;
}
