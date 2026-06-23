using System;
using System.Collections.Generic;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class CamerasSwitch : MonoBehaviour
{
	private void Start()
	{
		this.MainCamera = Global.MainCamera;
		this.AddCameraToDict(0, this.MainCamera);
		this.AddCameraToDict(1, this.TargetCamera);
		this.AddCameraToDict(2, this.SelfCamera);
		this.SelfCameraTransform = this.SelfCamera.transform;
		this.SelfCamera.CopyFrom(this.MainCamera);
		int num = this.SelfCamera.cullingMask;
		num &= ~(1 << LayerMask.NameToLayer("TransparentFX"));
		num |= 1 << LayerMask.NameToLayer("SelfCamera");
		this.SelfCamera.cullingMask = num;
		this.SelfCamera.far = 50f;
		this.TargetCameraTransform = this.TargetCamera.transform;
		this.TargetCamera.CopyFrom(this.MainCamera);
		num = this.SelfCamera.cullingMask;
		num &= ~(1 << LayerMask.NameToLayer("TransparentFX"));
		num |= 1 << LayerMask.NameToLayer("TargetCamera");
		this.TargetCamera.cullingMask = num;
		this.TargetCamera.far = 50f;
		if (null != this.SelfCamera)
		{
			this.SelfCamera.enabled = false;
		}
		if (null != this.TargetCamera)
		{
			this.TargetCamera.enabled = false;
		}
	}

	public void SetSelfTransform(Transform trans, Vector3 meshSize, float heightOffset = 0.8f, float lookScale = 0.75f, float distScale = 3.5f)
	{
		Vector3 vector = trans.TransformDirection(Vector3.forward) * distScale;
		vector..ctor(vector.x, vector.y + meshSize.y + heightOffset, vector.z);
		vector += trans.position;
		Vector3 selfLookPos;
		selfLookPos..ctor(trans.position.x, trans.position.y + meshSize.y - meshSize.y * (1f - lookScale), trans.position.z);
		this._SelfCameraPos = vector;
		this._SelfLookPos = selfLookPos;
	}

	public void SetTargetTransform(Transform trans, Vector3 meshSize, float heightOffset = 0.8f, float lookScale = 0.75f, float distScale = 3f, float extSubHeight = 0f)
	{
		Vector3 vector = trans.TransformDirection(Vector3.forward) * distScale;
		vector..ctor(vector.x, vector.y + meshSize.y + heightOffset, vector.z);
		vector += trans.position;
		Vector3 targetLookPos;
		targetLookPos..ctor(trans.position.x, trans.position.y + meshSize.y * lookScale - extSubHeight, trans.position.z);
		this._TargetCameraPos = vector;
		this._TargetLookPos = targetLookPos;
		this._TargetTransform = trans;
	}

	public void SetPositions(Vector3 selfCameraPos, Vector3 selfLookPos, Vector3 targetCameraPos, Vector3 targetLookPos)
	{
		this._SelfCameraPos = selfCameraPos;
		this._SelfLookPos = selfLookPos;
		this._TargetCameraPos = targetCameraPos;
		this._TargetLookPos = targetLookPos;
	}

	public void SwitchCamera(CamerasSwitch.CamerasMode mode, CamerasSwitch.CamerasType cameraID, CamerasSwitch.TweenOfAfterPushMode tweenOfAfterPushMode = CamerasSwitch.TweenOfAfterPushMode.None, float pushTime = 3f, float lookTime = 1f, bool orienttopath = false, bool addSpritesLayer = false, CameraMoveCompleteNotifyHander notify = null)
	{
		foreach (KeyValuePair<int, Camera> keyValuePair in this.CamerasDict)
		{
			int key = keyValuePair.Key;
			if (key != (int)cameraID)
			{
				this.CamerasDict[key].enabled = false;
			}
			else
			{
				this.CamerasDict[key].enabled = true;
				this.CurrentCamera = this.CamerasDict[key];
			}
		}
		if (null != this.CurrentCamera)
		{
			if (mode == CamerasSwitch.CamerasMode.ScenceMode)
			{
				Global.IsMainCamera = true;
				this.IsRotate = false;
				this.RotateAngle = 0f;
				this._TweenOfAfterPushMode = CamerasSwitch.TweenOfAfterPushMode.None;
				iTween.Stop();
				return;
			}
			Global.IsMainCamera = false;
			if (mode == CamerasSwitch.CamerasMode.NpcMode)
			{
				if (cameraID == CamerasSwitch.CamerasType.TargetCamera)
				{
					this.LookatTarget(this.TargetCameraTransform, this._TargetCameraPos, this._TargetLookPos);
				}
				else if (cameraID == CamerasSwitch.CamerasType.SelfCamera)
				{
					this.LookatTarget(this.SelfCameraTransform, this._SelfCameraPos, this._SelfLookPos);
				}
			}
			else if (mode == CamerasSwitch.CamerasMode.PushMode)
			{
				this._TweenOfAfterPushMode = tweenOfAfterPushMode;
				this.IsRotate = false;
				this.RotateAngle = 0f;
				this.PushTime = pushTime;
				this.LookTime = lookTime;
				this.Orienttopath = orienttopath;
				this.CameraMoveCompleteNotify = notify;
				this.TweenToTarget(this.TargetCameraTransform, this._TargetCameraPos, this._TargetLookPos);
			}
			else if (mode == CamerasSwitch.CamerasMode.TrackMode)
			{
				this._TweenOfAfterPushMode = tweenOfAfterPushMode;
			}
		}
	}

	public void AddSpriteLayer(bool addSpritesLayer)
	{
		if (addSpritesLayer)
		{
			this.TargetCamera.cullingMask |= 1 << LayerMask.NameToLayer("Sprites");
			this.SelfCamera.cullingMask |= 1 << LayerMask.NameToLayer("Sprites");
		}
		else
		{
			this.TargetCamera.cullingMask &= ~(1 << LayerMask.NameToLayer("Sprites"));
			this.SelfCamera.cullingMask |= 1 << LayerMask.NameToLayer("Sprites");
		}
	}

	private void TweenToTarget(Transform tran, Vector3 cameraPos, Vector3 lookPos)
	{
		iTween.MoveTo(this.CurrentCamera.gameObject, iTween.Hash(new object[]
		{
			"position",
			cameraPos,
			"time",
			this.PushTime,
			"orienttopath",
			this.Orienttopath,
			"looktarget",
			lookPos,
			"looktime",
			this.LookTime,
			"oncomplete",
			"OnTweenToTartetComplete",
			"oncompletetarget",
			base.gameObject
		}));
	}

	public void OnTweenToTartetComplete()
	{
		if (this._TweenOfAfterPushMode == CamerasSwitch.TweenOfAfterPushMode.Rotate)
		{
			this.IsRotate = true;
		}
		else if (this.CameraMoveCompleteNotify != null)
		{
			this.CameraMoveCompleteNotify(this, EventArgs.Empty);
		}
	}

	public void OnPushComplete()
	{
	}

	private void Update()
	{
		if (this.IsRotate)
		{
			if (this.RotateAngle < 360f)
			{
				Vector3 targetLookPos = this._TargetLookPos;
				this.CurrentCamera.transform.RotateAround(targetLookPos, Vector3.up, -this.RotateFactor * Time.deltaTime);
				this.RotateAngle += this.RotateFactor * Time.deltaTime;
			}
			else
			{
				this.IsRotate = false;
				if (this.CameraMoveCompleteNotify != null)
				{
					this.CameraMoveCompleteNotify(this, EventArgs.Empty);
				}
			}
		}
	}

	private void LookatTarget(Transform tran, Vector3 cameraPos, Vector3 lookPos)
	{
		tran.position = cameraPos;
		Vector3 vector = lookPos - cameraPos;
		tran.rotation = Quaternion.LookRotation(vector, Vector3.up);
	}

	private void AddCameraToDict(int id, Camera cam)
	{
		if (!this.CamerasDict.ContainsKey(id))
		{
			this.CamerasDict.Add(id, cam);
		}
	}

	public Camera MainCamera;

	public Camera SelfCamera;

	public Camera TargetCamera;

	public AnimationManager _AnimationMgr;

	private Dictionary<int, Camera> CamerasDict = new Dictionary<int, Camera>();

	private Camera CurrentCamera;

	private Transform SelfCameraTransform;

	private Transform TargetCameraTransform;

	private Vector3 _SelfCameraPos = Vector3.zero;

	private Vector3 _SelfLookPos = Vector3.zero;

	private Vector3 _TargetCameraPos = Vector3.zero;

	private Vector3 _TargetLookPos = Vector3.zero;

	private Transform _TargetTransform;

	private CamerasSwitch.TweenOfAfterPushMode _TweenOfAfterPushMode = CamerasSwitch.TweenOfAfterPushMode.None;

	private float PushTime = 3f;

	private float LookTime = 3f;

	private bool Orienttopath;

	private CameraMoveCompleteNotifyHander CameraMoveCompleteNotify;

	private bool IsRotate;

	private float RotateAngle;

	private float RotateFactor = 180f;

	public enum CamerasType
	{
		MainCamera,
		TargetCamera,
		SelfCamera
	}

	public enum CamerasMode
	{
		ScenceMode,
		NpcMode,
		PushMode,
		TrackMode
	}

	public enum TweenOfAfterPushMode
	{
		None = -1,
		Track,
		Rotate
	}
}
