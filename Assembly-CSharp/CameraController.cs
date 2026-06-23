using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
	public Transform TagterTrans
	{
		set
		{
			if (null == value)
			{
				this._Trans = base.transform;
			}
			else
			{
				this._Trans = value;
				if (null == this.Cam)
				{
					U3DUtils.AddPlayerController(null);
					return;
				}
				this.UpdateCameraPosition();
			}
		}
	}

	private void Start()
	{
		this.TouchRect = new Rect((float)Screen.width / 5f, (float)Screen.height / 5f, (float)Screen.width / 5f * 3f, (float)Screen.height / 5f * 3f);
		if (this.CamHeightPushleft != 0f)
		{
			this.HeightDivLeft = this.CamHeight / this.CamHeightPushleft;
		}
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
			U3DUtils.AddPlayerController(null);
			return;
		}
		this.UpdateCameraPosition();
		this.UpdateCameraPositionVars();
	}

	private void UpdateCameraPosition()
	{
		if (null == this._Trans)
		{
			this._Trans = base.transform;
		}
		if (null == this._CameraTrans)
		{
			this._CameraTrans = this.Cam.transform;
		}
		if (null == this._CameraTrans)
		{
			return;
		}
		Vector3 position = this._Trans.position;
		if (null == this._Trans)
		{
			Vector3 leaderPos = LeaderInfo.LeaderPos;
			position.y = leaderPos.y + this.CamHeight;
			position.x = leaderPos.x - this.CamHeightPushleft;
			position.z = leaderPos.z - this.CamHeightPushback;
		}
		else
		{
			position.y = this._Trans.position.y + this.CamHeight;
			position.x = this._Trans.position.x - this.CamHeightPushleft;
			position.z = this._Trans.position.z - this.CamHeightPushback;
		}
		this._CameraTrans.position = position;
	}

	private void UpdateCameraPositionVars()
	{
		if (Input.touchCount > 1 && (Input.GetTouch(0).phase == 1 || Input.GetTouch(1).phase == 1))
		{
			Vector2 position = Input.GetTouch(0).position;
			Vector2 position2 = Input.GetTouch(1).position;
			if (this.TouchRect.Contains(position) && this.TouchRect.Contains(position2))
			{
				if (this.isEnlarge(this.oldPosition1, this.oldPosition2, position, position2))
				{
					if (this.CamHeight > 6f)
					{
						this.CamHeight -= 0.5f;
						if (this.HeightDivLeft != 0f)
						{
							this.CamHeightPushback = this.CamHeight / this.HeightDivLeft;
						}
						this.CamHeightPushleft = this.CamHeightPushback;
						Camera.main.farClipPlane -= 1f;
					}
				}
				else if (this.CamHeight < 13f)
				{
					this.CamHeight += 0.5f;
					if (this.HeightDivLeft != 0f)
					{
						this.CamHeightPushback = this.CamHeight / this.HeightDivLeft;
					}
					this.CamHeightPushleft = this.CamHeightPushback;
					Camera.main.farClipPlane += 1f;
				}
				this.oldPosition1 = position;
				this.oldPosition2 = position2;
			}
		}
	}

	private bool isEnlarge(Vector2 oP1, Vector2 oP2, Vector2 nP1, Vector2 nP2)
	{
		float num = Mathf.Sqrt((oP1.x - oP2.x) * (oP1.x - oP2.x) + (oP1.y - oP2.y) * (oP1.y - oP2.y));
		float num2 = Mathf.Sqrt((nP1.x - nP2.x) * (nP1.x - nP2.x) + (nP1.y - nP2.y) * (nP1.y - nP2.y));
		return num < num2;
	}

	public GameObject Cam;

	public float CamHeight = 12f;

	public float CamHeightPushback = 8f;

	public float CamHeightPushleft = 8f;

	private Transform _Trans;

	private Transform _CameraTrans;

	private float HeightDivLeft = 1f;

	private Rect TouchRect = new Rect(0f, 0f, 0f, 0f);

	private Vector2 oldPosition1;

	private Vector2 oldPosition2;
}
