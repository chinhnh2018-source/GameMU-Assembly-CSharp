using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TweenCamera : MonoBehaviour
{
	private void Awake()
	{
		this.mAnimation = base.GetComponent<Animation>();
		this.mAnimation.clip.AddEvent(new AnimationEvent
		{
			time = ((this.startTime != 0f) ? this.startTime : this.mAnimation.clip.length),
			functionName = "OnAnimationEnd"
		});
		this.modelCameraTr = base.transform.Find("Camera001/Camera");
		Animation component = this.modelCameraTr.GetComponent<Animation>();
		if (component)
		{
			this.cameraAnimClip = component.clip;
		}
	}

	private void OnEnable()
	{
		this.mCamera = ((!Global.MainCamera) ? Camera.main : Global.MainCamera);
		if (this.cameraAnimClip)
		{
			Animation animation = this.mCamera.GetComponent<Animation>();
			if (!animation)
			{
				animation = this.mCamera.gameObject.AddComponent<Animation>();
			}
			animation.AddClip(this.cameraAnimClip, this.cameraAnimClip.name);
			animation.Play(this.cameraAnimClip.name);
		}
		this.mCameraTrans = this.mCamera.transform;
		this.mTargetTrans = this.target.transform;
		this.mTargetFov = this.target.fieldOfView;
		this.mCamera.transform.parent = base.transform.Find("Camera001").transform;
		this.mCamera.backgroundColor = this.target.backgroundColor;
		base.StartCoroutine(this.ReSetMainCameraPosition());
	}

	private void OnDisable()
	{
		this.OnAnimationEnd();
	}

	private IEnumerator ReSetMainCameraPosition()
	{
		yield return null;
		this.mCameraTrans.localPosition = this.modelCameraTr.localPosition;
		this.mCameraTrans.localRotation = this.modelCameraTr.localRotation;
		this.mCameraTrans.localScale = this.modelCameraTr.localScale;
		this.mCamera.fieldOfView = this.modelCameraTr.GetComponent<Camera>().fieldOfView;
		yield break;
	}

	private void OnAnimationEnd()
	{
		if (this.mCamera)
		{
			Animation component = this.mCamera.GetComponent<Animation>();
			if (component)
			{
				component.Stop();
				if (this.cameraAnimClip && component.GetClipCount() > 0)
				{
					component.RemoveClip(this.cameraAnimClip.name);
				}
			}
		}
		this.mTween = true;
		this.mStartTime = Time.time;
		this.mStartPos = this.mCameraTrans.position;
		if (this.mCamera)
		{
			this.mStartFOV = this.mCamera.fieldOfView;
		}
		this.mStartRotation = this.mCameraTrans.rotation;
		this.mCameraTrans.parent = null;
		if (Global.MainCamera)
		{
			Object.DontDestroyOnLoad(Global.MainCamera.gameObject);
		}
		if (this.DPSelectedItem != null)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = -2,
				IDType = 0
			});
		}
	}

	private void LateUpdate()
	{
		if (!this.mTween)
		{
			if (this.mCameraTrans.parent != null)
			{
				this.mCameraTrans.localPosition = this.modelCameraTr.localPosition;
				this.mCameraTrans.localRotation = this.modelCameraTr.localRotation;
				this.mCameraTrans.localScale = this.modelCameraTr.localScale;
			}
			return;
		}
		if (this.time != 0f)
		{
			float num = this.curve.Evaluate((Time.time - this.mStartTime) / this.time);
			if (num >= 1f)
			{
				this.mCameraTrans.position = this.mTargetTrans.position;
				this.mCameraTrans.rotation = this.mTargetTrans.rotation;
				this.mCamera.fieldOfView = this.mTargetFov;
				this.mTween = false;
			}
			else
			{
				this.mCameraTrans.position = Vector3.Lerp(this.mStartPos, this.mTargetTrans.position, num);
				this.mCameraTrans.rotation = Quaternion.Slerp(this.mStartRotation, this.mTargetTrans.rotation, num);
				this.mCamera.fieldOfView = Mathf.Lerp(this.mStartFOV, this.mTargetFov, num);
			}
		}
	}

	public float time;

	public float startTime;

	public AnimationCurve curve;

	public Camera target;

	private Transform mTargetTrans;

	private float mTargetFov;

	private Animation mAnimation;

	private Transform mCameraTrans;

	private Camera mCamera;

	private float mStartTime;

	private bool mTween;

	private Transform modelCameraTr;

	private AnimationClip cameraAnimClip;

	private Vector3 mStartPos;

	private float mStartFOV;

	private Quaternion mStartRotation;

	public DPSelectedItemEventHandler DPSelectedItem;
}
