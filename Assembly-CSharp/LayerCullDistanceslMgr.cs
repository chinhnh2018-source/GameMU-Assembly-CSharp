using System;
using UnityEngine;

public class LayerCullDistanceslMgr : MonoBehaviour
{
	private void Start()
	{
		LayerCullDistanceslMgr.SetCameraLayerDistance(base.GetComponent<Camera>(), 1000f);
	}

	public static void SetCameraLayerDistance(Camera camera, float defaultLayer = 45f)
	{
		float[] array = new float[32];
		int num = 0;
		array[num++] = defaultLayer;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 1000f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 30f;
		array[num++] = 50f;
		array[num++] = 30f;
		array[num++] = 50f;
		array[num++] = 30f;
		array[num++] = 50f;
		array[num++] = 25f;
		array[num++] = 25f;
		array[num++] = 25f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 25f;
		array[num++] = 50f;
		array[num++] = 25f;
		array[num++] = 25f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		array[num++] = 50f;
		camera.layerCullDistances = array;
	}
}
