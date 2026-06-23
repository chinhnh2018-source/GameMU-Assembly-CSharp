using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CameraControllerDrag : MonoBehaviour
{
	public void SetCaneraProperty(Vector3 Pos, Quaternion Rotation, Vector3 scale)
	{
		if (null != this.Cam)
		{
			this.Cam.transform.localScale = scale;
			this.Cam.transform.localRotation = Rotation;
			if (Vector3.zero != Pos)
			{
				this.Cam.transform.localPosition = Pos;
			}
			else
			{
				if (null == this._Trans)
				{
					this._Trans = base.transform;
				}
				Vector3 position = this._Trans.position;
				position.y = this._Trans.position.y + this.CamHeight;
				position.z = this._Trans.position.z - this.CamHeightPushback;
				this.Cam.transform.position = position;
			}
		}
	}

	public void SetCamPositionRec(Vector4 rect)
	{
		this.CamPositionRect.MaxX = rect.x;
		this.CamPositionRect.MaxY = rect.y;
		this.CamPositionRect.MinX = rect.z;
		this.CamPositionRect.MinY = rect.w;
	}

	public Vector3 CameraPos
	{
		get
		{
			if (null == this._CameraTrans)
			{
				return Vector3.zero;
			}
			return this._CameraTrans.localPosition;
		}
	}

	private void Start()
	{
		this.TouchRect = new Rect((float)Screen.width / 5f, (float)Screen.height / 5f, (float)Screen.width / 5f * 3f, (float)Screen.height / 5f * 3f);
		if (this.Cam)
		{
			this.Cam.transform.parent = null;
		}
		if (this.Cam != null)
		{
			GameObject gameObject = GameObject.Find("CameraParams");
			float farClipPlane = 45f;
			if (gameObject != null)
			{
				farClipPlane = gameObject.GetComponent<Camera>().farClipPlane;
			}
			this.Cam.GetComponent<Camera>().farClipPlane = farClipPlane;
		}
		if (null == this.Cam)
		{
			U3DUtils.AddCameraController(null);
			return;
		}
		if (null == this._Trans)
		{
			this._Trans = base.transform;
		}
		if (null == this._CameraTrans)
		{
			this._CameraTrans = this.Cam.transform;
		}
		if (null == this._Trans || null == this._CameraTrans)
		{
			return;
		}
		Vector3 position = this._Trans.position;
		position.y = this._Trans.position.y + this.CamHeight;
		position.z = this._Trans.position.z - this.CamHeightPushback;
		this._CameraTrans.position = position;
		if (this.RoleDragScreen != null)
		{
			this.RoleDragScreen(this, new DPSelectedItemEventArgs());
		}
		int[] systemParamIntArrayByName = ConfigSystemParam.GetSystemParamIntArrayByName("CrusadeMap", ',');
		if (systemParamIntArrayByName != null && 6 <= systemParamIntArrayByName.Length)
		{
			this.SpeedEx = (float)systemParamIntArrayByName[4] / 100f;
		}
		else
		{
			this.SpeedEx = 0.2f;
		}
	}

	private void OnEnable()
	{
		CameraShake.Instance.controllerCount++;
	}

	private void OnDisable()
	{
		CameraShake.Instance.controllerCount--;
	}

	private void OnDestroy()
	{
		if (base.enabled)
		{
			CameraShake.Instance.controllerCount--;
		}
	}

	private void Update()
	{
		if (null == this.Cam)
		{
			U3DUtils.AddCameraController(null);
			return;
		}
		if (null == this._Trans)
		{
			this._Trans = base.transform;
		}
		if (null == this._CameraTrans)
		{
			this._CameraTrans = this.Cam.transform;
		}
		if (null == this._Trans || null == this._CameraTrans)
		{
			return;
		}
		this.UpdateCameraPositionVars();
	}

	private void UpdateCameraPositionVars()
	{
		if (Input.touchCount == 1 && Input.GetTouch(0).phase == 1)
		{
			Vector2 position = Input.GetTouch(0).position;
			if (this.TouchRect.Contains(position))
			{
				Vector2 deltaPosition = Input.GetTouch(0).deltaPosition;
				Vector3 vector = this._CameraTrans.position;
				float num = -deltaPosition.x * 0.1f * this.SpeedEx;
				float num2 = -deltaPosition.y * 0.1f * this.SpeedEx;
				bool flag = this.RestrictWithinBounds(vector.x + num, vector.z + num2);
				if (flag)
				{
					Vector3 vector2;
					vector2..ctor(num, 0f, num2);
					vector += vector2;
					this._CameraTrans.position = vector;
				}
			}
			if (this.RoleDragScreen != null)
			{
				this.RoleDragScreen(this, new DPSelectedItemEventArgs());
			}
		}
	}

	public bool RestrictWithinBounds(float x, float y)
	{
		return x >= this.CamPositionRect.MinX && x <= this.CamPositionRect.MaxX && y >= this.CamPositionRect.MinY && y <= this.CamPositionRect.MaxY;
	}

	public const float speed = 0.1f;

	public GameObject Cam;

	private RectBound CamPositionRect = new RectBound(-1000f, -1000f, 1000f, 1000f);

	public float CamHeight = 12f;

	public float CamHeightPushback = 8f;

	public float CamHeightPushleft = 8f;

	private Transform _Trans;

	private Transform _CameraTrans;

	private Rect TouchRect = new Rect(0f, 0f, 0f, 0f);

	private float SpeedEx = 0.05f;

	public DPSelectedItemEventHandler RoleDragScreen;
}
