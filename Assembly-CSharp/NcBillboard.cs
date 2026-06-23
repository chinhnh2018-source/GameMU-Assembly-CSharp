using System;
using UnityEngine;

public class NcBillboard : NcEffectBehaviour
{
	private void Awake()
	{
	}

	private void OnEnable()
	{
		this.UpdateBillboard();
	}

	public void UpdateBillboard()
	{
		this.m_fRndValue = Random.Range(0f, 360f);
		if (base.enabled)
		{
			this.Update();
		}
	}

	private void Start()
	{
		this.m_qOiginal = base.transform.rotation;
	}

	private void Update()
	{
		if (Camera.main == null)
		{
			return;
		}
		Vector3 vector;
		if (this.m_bFixedObjectUp)
		{
			vector = base.transform.up;
		}
		else
		{
			vector = Camera.main.transform.rotation * Vector3.up;
		}
		if (this.m_bCameraLookAt)
		{
			base.transform.LookAt(Camera.main.transform, vector);
		}
		else
		{
			base.transform.LookAt(base.transform.position + Camera.main.transform.rotation * Vector3.back, vector);
		}
		switch (this.m_FrontAxis)
		{
		case NcBillboard.AXIS_TYPE.AXIS_BACK:
			base.transform.Rotate(base.transform.up, 180f, 0);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_RIGHT:
			base.transform.Rotate(base.transform.up, 270f, 0);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_LEFT:
			base.transform.Rotate(base.transform.up, 90f, 0);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_UP:
			base.transform.Rotate(base.transform.right, 90f, 0);
			break;
		case NcBillboard.AXIS_TYPE.AXIS_DOWN:
			base.transform.Rotate(base.transform.right, 270f, 0);
			break;
		}
		if (this.m_bFixedStand)
		{
			base.transform.rotation = Quaternion.Euler(new Vector3(0f, base.transform.rotation.eulerAngles.y, base.transform.rotation.eulerAngles.z));
		}
		if (this.m_RatationMode == NcBillboard.ROTATION.RND)
		{
			base.transform.localRotation *= Quaternion.Euler((this.m_RatationAxis != NcBillboard.AXIS.X) ? 0f : this.m_fRndValue, (this.m_RatationAxis != NcBillboard.AXIS.Y) ? 0f : this.m_fRndValue, (this.m_RatationAxis != NcBillboard.AXIS.Z) ? 0f : this.m_fRndValue);
		}
		if (this.m_RatationMode == NcBillboard.ROTATION.ROTATE)
		{
			float num = this.m_fTotalRotationValue + NcEffectBehaviour.GetEngineDeltaTime() * this.m_fRotationValue;
			base.transform.Rotate((this.m_RatationAxis != NcBillboard.AXIS.X) ? 0f : num, (this.m_RatationAxis != NcBillboard.AXIS.Y) ? 0f : num, (this.m_RatationAxis != NcBillboard.AXIS.Z) ? 0f : num, 1);
			this.m_fTotalRotationValue = num;
		}
	}

	public bool m_bCameraLookAt;

	public bool m_bFixedObjectUp;

	public bool m_bFixedStand;

	public NcBillboard.AXIS_TYPE m_FrontAxis;

	public NcBillboard.ROTATION m_RatationMode;

	public NcBillboard.AXIS m_RatationAxis = NcBillboard.AXIS.Z;

	public float m_fRotationValue = 180f;

	protected float m_fRndValue;

	protected float m_fTotalRotationValue;

	protected Quaternion m_qOiginal;

	public enum AXIS_TYPE
	{
		AXIS_FORWARD,
		AXIS_BACK,
		AXIS_RIGHT,
		AXIS_LEFT,
		AXIS_UP,
		AXIS_DOWN
	}

	public enum ROTATION
	{
		NONE,
		RND,
		ROTATE
	}

	public enum AXIS
	{
		X,
		Y,
		Z
	}
}
