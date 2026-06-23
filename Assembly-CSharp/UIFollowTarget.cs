using System;
using HSGameEngine.GameEngine.Logic;
using UnityEngine;

public class UIFollowTarget : MonoBehaviour
{
	public Transform target
	{
		get
		{
			return this._target;
		}
		set
		{
			this._target = value;
			this.ForceUpdata = 1;
			this.initHangPoint = false;
			Vector3 vector = this.HangPointLocalPosition;
		}
	}

	public float HorseHeght
	{
		set
		{
			this._HorseHeght = value;
			this.ForceUpdata = 1;
			this.initHangPoint = false;
			Vector3 vector = this.HangPointLocalPosition;
		}
	}

	public float BianShenHeight
	{
		set
		{
			this._BianShenHeight = value;
			this.initHangPoint = false;
			Vector3 vector = this.HangPointLocalPosition;
		}
	}

	private Vector3 HangPointLocalPosition
	{
		get
		{
			if (!this.initHangPoint)
			{
				Renderer renderer = this.target.GetComponentInChildren<Renderer>();
				if (renderer)
				{
					this.hangPointLocalPosition = new Vector3(0f, Mathf.Clamp(renderer.bounds.center.y + renderer.bounds.extents.y - this.target.position.y, 1.6f, 2.2f), 0f);
				}
				else
				{
					MeshFilter componentInChildren = this.target.GetComponentInChildren<MeshFilter>();
					if (componentInChildren)
					{
						this.hangPointLocalPosition = new Vector3(0f, Mathf.Clamp(componentInChildren.sharedMesh.bounds.center.y + componentInChildren.sharedMesh.bounds.extents.y, 1.6f, 2.2f), 0f);
					}
				}
				this.hangPointLocalPosition.y = this.hangPointLocalPosition.y + this._BianShenHeight;
				if (this.target.name.Contains("Horse_"))
				{
					for (int i = 0; i < this.target.childCount; i++)
					{
						Transform child = this.target.GetChild(i);
						if (null != child)
						{
							renderer = child.GetComponent<Renderer>();
							if (null != renderer)
							{
								break;
							}
						}
					}
					Vector3 extents;
					extents..ctor(0f, 1.3f, 0f);
					if (renderer)
					{
						extents = renderer.bounds.extents;
					}
					else
					{
						MeshFilter componentInChildren2 = this.target.GetComponentInChildren<MeshFilter>();
						if (componentInChildren2)
						{
							extents = componentInChildren2.sharedMesh.bounds.extents;
						}
					}
					this.hangPointLocalPosition = new Vector3(0f, extents.y + this._HorseHeght, 0f);
				}
				this.initHangPoint = true;
			}
			return this.hangPointLocalPosition;
		}
	}

	private bool IsActive
	{
		set
		{
			if (this.isActive == value)
			{
				return;
			}
			this.isActive = value;
			this.mTrans.localPosition = new Vector3(0f, 1000f, 0f);
		}
	}

	private void Awake()
	{
		this.mTrans = base.transform;
	}

	private void Start()
	{
		if (this.target != null)
		{
			if (this.gameCamera == null)
			{
				this.gameCamera = Global.MainCamera;
				this.mGameCameraTrans = this.gameCamera.transform;
			}
			if (this.uiCamera == null)
			{
				this.uiCamera = Global.UICamera;
			}
		}
		else
		{
			Debug.LogError("Expected to have 'target' set to a valid transform", this);
		}
	}

	private void Update()
	{
		if (!this.target)
		{
			return;
		}
		if (this.ForceUpdata == 0)
		{
			if (this.mLastTargetPos == this.target.position && this.mLastCameraPos == this.mGameCameraTrans.position)
			{
				return;
			}
			Vector3 vector = this.target.position - LeaderInfo.LeaderPos;
			if (Mathf.Abs(vector.x) > 8f || Mathf.Abs(vector.z) > 8f)
			{
				this.IsActive = false;
				return;
			}
			this.IsActive = true;
			this.mLastTargetPos = this.target.position;
			this.mLastCameraPos = this.mGameCameraTrans.position;
			Vector3 vector2 = this.target.position + this.HangPointLocalPosition;
			vector2 = this.gameCamera.WorldToScreenPoint(vector2);
			if (this.mLastUIPos != vector2)
			{
				this.mLastUIPos = vector2;
				this.mTrans.position = this.uiCamera.ScreenToWorldPoint(vector2);
				vector2 = this.mTrans.localPosition;
				vector2.z = 0f;
				vector2.y += (float)Screen.height * 0.02f;
				this.mTrans.localPosition = vector2;
			}
		}
		else
		{
			this.ForceUpdata = 0;
			this.IsActive = true;
			this.mLastTargetPos = this.target.position;
			this.mLastCameraPos = this.mGameCameraTrans.position;
			Vector3 vector3 = this.target.position + this.HangPointLocalPosition;
			vector3 = this.gameCamera.WorldToScreenPoint(vector3);
			vector3.y += (float)Screen.height * 0.02f;
			if (this.mLastUIPos != vector3)
			{
				this.mLastUIPos = vector3;
				this.mTrans.position = this.uiCamera.ScreenToWorldPoint(vector3);
				vector3 = this.mTrans.localPosition;
				vector3.z = 0f;
				this.mTrans.localPosition = vector3;
			}
		}
	}

	private const float pixelOffetPercentage = 0.02f;

	private byte ForceUpdata;

	private Transform _target;

	public Camera gameCamera;

	public Camera uiCamera;

	private float _HorseHeght;

	private float _BianShenHeight;

	private Transform mTrans;

	private Transform mGameCameraTrans;

	private Vector3 mLastTargetPos;

	private Vector3 mLastCameraPos;

	private Vector3 mLastUIPos;

	private bool initHangPoint;

	private Vector3 hangPointLocalPosition = Vector3.zero;

	private bool isActive = true;
}
