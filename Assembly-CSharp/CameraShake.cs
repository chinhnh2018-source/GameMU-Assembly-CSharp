using System;
using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
	public static CameraShake Instance
	{
		get
		{
			if (!CameraShake._instance && Camera.main)
			{
				CameraShake._instance = Camera.main.gameObject.AddComponent<CameraShake>();
			}
			return CameraShake._instance;
		}
	}

	public static CameraShake GetInstance(Camera camera)
	{
		CameraShake cameraShake = null;
		if (camera)
		{
			cameraShake = camera.gameObject.GetComponent<CameraShake>();
			if (!cameraShake)
			{
				cameraShake = camera.gameObject.AddComponent<CameraShake>();
			}
		}
		return cameraShake;
	}

	private void Awake()
	{
		this.mTransform = base.transform;
	}

	private void OnEnable()
	{
		base.StartCoroutine(this.Shake());
	}

	private void Update()
	{
		if (this.mShakeOffset != Vector3.zero)
		{
			if (this.mTransform.position != this.mLastShakePosition)
			{
				this.mShakeAnchor = this.mTransform.position;
			}
			this.mLastShakePosition = this.mShakeAnchor + this.mShakeOffset;
			this.mTransform.position = this.mLastShakePosition;
		}
	}

	private IEnumerator Shake()
	{
		while (base.enabled && Time.time < this.mShakeEndTime && !CameraShake.DisableCameraShake)
		{
			this.mShakeOffset = this.mTransform.TransformDirection(Random.Range(-this.maxShakeX, this.maxShakeX), Random.Range(-this.maxShakeY, this.maxShakeY), 0f) * 20f;
			yield return new WaitForSeconds(this.shakeInterval);
		}
		base.enabled = false;
		if (this.mTransform.position == this.mLastShakePosition)
		{
			this.mTransform.position = this.mShakeAnchor;
		}
		this.mShakeOffset = Vector3.zero;
		yield break;
	}

	public void ShakeForSeconds(float time, float interval, float x, float y)
	{
		if (CameraShake.DisableCameraShake)
		{
			return;
		}
		if (base.enabled && x < this.maxShakeX)
		{
			return;
		}
		this.maxShakeX = x;
		this.maxShakeY = y;
		this.shakeInterval = interval;
		this.mShakeEndTime = Time.time + time;
		if (!base.enabled)
		{
			base.enabled = true;
		}
	}

	public void StopShake()
	{
		base.enabled = false;
		this.mShakeOffset = Vector3.zero;
		base.StopAllCoroutines();
	}

	public int controllerCount { get; set; }

	public Vector3 OriginPosition { get; set; }

	private float shakeInterval = 0.1f;

	private float maxShakeX = 0.02f;

	private float maxShakeY = 0.02f;

	private Transform mTransform;

	private Vector3 mShakeOffset;

	private Vector3 mLastShakePosition;

	private Vector3 mShakeAnchor;

	private float mShakeEndTime;

	public static bool DisableCameraShake;

	private static CameraShake _instance;
}
