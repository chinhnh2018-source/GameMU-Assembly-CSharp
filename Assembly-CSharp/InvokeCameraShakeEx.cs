using System;
using System.Collections;
using UnityEngine;

public class InvokeCameraShakeEx : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this.startShakeTime);
		CameraShake Instance = null;
		if (this.cameraEffect != null)
		{
			Instance = CameraShake.GetInstance(this.cameraEffect);
		}
		else
		{
			Instance = CameraShake.Instance;
		}
		if (null != Instance)
		{
			Instance.ShakeForSeconds(this.shakeTimeLength, this.shakeFreq, this.shakeX, this.shakeY);
		}
		Object.Destroy(this);
		yield break;
	}

	public float startShakeTime;

	public float shakeTimeLength = 0.4f;

	public float shakeFreq = 0.01f;

	public float shakeX = 0.02f;

	public float shakeY = 0.02f;

	public Camera cameraEffect;
}
