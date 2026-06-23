using System;
using System.Collections;
using UnityEngine;

public class InvokeCameraShake : MonoBehaviour
{
	private IEnumerator Start()
	{
		yield return new WaitForSeconds(this.startShakeTime);
		CameraShake Instance = CameraShake.Instance;
		if (null != Instance)
		{
			Instance.ShakeForSeconds(this.shakeTimeLength, this.shakeFreq, this.shakeX, this.shakeY);
		}
		yield break;
	}

	public float startShakeTime;

	public float shakeTimeLength = 0.4f;

	public float shakeFreq = 0.01f;

	public float shakeX = 0.02f;

	public float shakeY = 0.02f;
}
