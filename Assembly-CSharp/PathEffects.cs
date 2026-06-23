using System;
using UnityEngine;

public class PathEffects : MonoBehaviour
{
	private void Awake()
	{
	}

	private void OnEnable()
	{
	}

	public Vector3 VertVector(Vector3 axis)
	{
		Vector3 vector = this.m_StartVector;
		while (vector == axis.normalized || vector.sqrMagnitude == 0f)
		{
			vector[Random.Range(0, 3)] = 1f;
			vector = Vector3.Cross(vector, axis);
		}
		return vector.normalized;
	}

	private void Start()
	{
		if (this.centerObj == null && base.transform.parent != null)
		{
			this.centerObj = base.transform.parent.gameObject;
		}
		if (this.centerObj == null)
		{
			this.centerObj = base.gameObject;
		}
		this.m_StartPos = this.center + this.VertVector(this.m_axis) * this.m_Radius;
		this.m_StartRotation = base.transform.localRotation;
		base.transform.localPosition = this.m_StartPos;
	}

	private void Update()
	{
		this.m_Time += Time.deltaTime;
		if (this.m_Period > 0f && this.m_Time > this.m_Period)
		{
			if (this.m_AutoRepeat)
			{
				this.m_Time = 0f;
				base.transform.localPosition = this.m_StartPos;
				base.transform.localRotation = this.m_StartRotation;
			}
			else
			{
				base.gameObject.SetActive(false);
			}
		}
		float num;
		if (this.m_Period > 0f)
		{
			num = this.m_MoveSpeed * this.m_Height / this.m_Period;
		}
		else
		{
			num = this.m_MoveSpeed;
		}
		if (num != 0f)
		{
			base.transform.Translate(this.m_axis * num * Time.deltaTime);
		}
		float num2;
		if (this.m_RoundPeriod > 0f)
		{
			num2 = 360f * this.m_RoundSpeed / this.m_RoundPeriod;
		}
		else
		{
			num2 = 360f * this.m_RoundSpeed / (this.m_Radius * 2f * 3.14159274f);
		}
		base.transform.RotateAround(this.center + this.centerObj.transform.position, this.m_axis, num2 * Time.deltaTime);
	}

	public GameObject centerObj;

	public Vector3 center = new Vector3(0f, 0f, 0f);

	public Vector3 m_axis = Vector3.up;

	public Vector3 m_StartVector = Vector3.forward;

	public float m_Height = 8f;

	public float m_Radius = 2f;

	public bool m_AutoRepeat = true;

	public float m_RoundPeriod = 2f;

	public float m_Period = 5f;

	public float m_RoundSpeed = 1f;

	public float m_MoveSpeed = 1f;

	protected Vector3 m_StartPos = Vector3.zero;

	protected Quaternion m_StartRotation = Quaternion.identity;

	private float m_Time;
}
