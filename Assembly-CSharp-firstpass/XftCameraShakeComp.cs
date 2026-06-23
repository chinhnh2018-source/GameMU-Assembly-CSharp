using System;
using UnityEngine;
using Xft;

[ExecuteInEditMode]
public class XftCameraShakeComp : MonoBehaviour
{
	public XftEventComponent Client
	{
		get
		{
			return this.m_client;
		}
	}

	private void Awake()
	{
		base.enabled = false;
	}

	public void Init()
	{
		this.PositionSpring = new Spring(base.transform, Spring.TransformType.Position);
		this.PositionSpring.MinVelocity = 1E-05f;
		this.RotationSpring = new Spring(base.transform, Spring.TransformType.Rotation);
		this.RotationSpring.MinVelocity = 1E-05f;
	}

	public void Reset(XftEventComponent client)
	{
		if (!(this.m_client != null) || !this.CheckDone())
		{
		}
		this.m_client = client;
		this.PositionSpring.Stiffness = new Vector3(this.m_client.PositionStifness, this.m_client.PositionStifness, this.m_client.PositionStifness);
		this.PositionSpring.Damping = Vector3.one - new Vector3(this.m_client.PositionDamping, this.m_client.PositionDamping, this.m_client.PositionDamping);
		this.RotationSpring.Stiffness = new Vector3(this.m_client.RotationStiffness, this.m_client.RotationStiffness, this.m_client.RotationStiffness);
		this.RotationSpring.Damping = Vector3.one - new Vector3(this.m_client.RotationDamping, this.m_client.RotationDamping, this.m_client.RotationDamping);
		this.m_elapsedTime = 0f;
		this.PositionSpring.RefreshTransformType();
		this.RotationSpring.RefreshTransformType();
		this.m_earthQuakeTimeTemp = this.m_client.EarthQuakeTime;
	}

	private void UpdateEarthQuake()
	{
		if (this.m_client == null || !this.m_client.UseEarthQuake || this.m_earthQuakeTimeTemp <= 0f || !this.EarthQuakeToggled || this.m_elapsedTime > this.m_client.EarthQuakeTime)
		{
			return;
		}
		this.m_elapsedTime += Time.deltaTime;
		this.m_earthQuakeTimeTemp -= 0.0166f * (60f * Time.deltaTime);
		float num;
		if (this.m_client.EarthQuakeMagTye == MAGTYPE.Fixed)
		{
			num = this.m_client.EarthQuakeMagnitude;
		}
		else
		{
			num = this.m_client.EarthQuakeMagCurve.Evaluate(this.m_elapsedTime);
		}
		Vector3 force = Vector3.Scale(XftSmoothRandom.GetVector3Centered(1f), new Vector3(num, 0f, num)) * Mathf.Min(this.m_earthQuakeTimeTemp, 1f);
		float num2 = 0f;
		if (Random.value < 0.3f)
		{
			num2 = Random.Range(0f, num * 0.35f) * Mathf.Min(this.m_earthQuakeTimeTemp, 1f);
			if (this.PositionSpring.State.y >= this.PositionSpring.RestState.y)
			{
				num2 = -num2;
			}
		}
		this.PositionSpring.AddForce(force);
		this.RotationSpring.AddForce(new Vector3(0f, 0f, -force.x * 2f) * this.m_client.EarthQuakeCameraRollFactor);
		this.PositionSpring.AddForce(new Vector3(0f, num2, 0f));
	}

	public bool CheckDone()
	{
		return this.PositionSpring.Done && this.RotationSpring.Done;
	}

	private void Update()
	{
		if (this.PositionSpring == null || this.RotationSpring == null)
		{
			return;
		}
		this.UpdateEarthQuake();
		if (this.CheckDone())
		{
			base.enabled = false;
			return;
		}
		this.PositionSpring.FixedUpdate();
		this.RotationSpring.FixedUpdate();
	}

	public Spring PositionSpring;

	public Spring RotationSpring;

	protected XftEventComponent m_client;

	protected float m_elapsedTime;

	public bool EarthQuakeToggled;

	protected float m_earthQuakeTimeTemp;
}
