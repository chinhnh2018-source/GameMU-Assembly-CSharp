using System;
using UnityEngine;

public class FlyDragon : ManualUpdateBehaviour
{
	private void Start()
	{
		this.bounceRange = this.range * 0.7f;
		this.bounceCenter = base.transform.position + base.transform.forward * this.range * 0.3f;
		this.targetDir = new Vector2(base.transform.forward.x, base.transform.forward.z);
	}

	public override void ManualUpdate()
	{
		if (this.DirectionDifference(this.targetDir) > 0.9f)
		{
			this.SetNewDir();
		}
		base.transform.position += base.transform.forward * (this.speed * Time.deltaTime);
		base.transform.RotateAroundLocal(Vector3.up, Time.deltaTime * this.turnRate);
	}

	private float DirectionDifference(Vector2 targetDir)
	{
		return targetDir.x * base.transform.forward.x + targetDir.y * base.transform.forward.z;
	}

	private void SetNewDir()
	{
		Vector2 vector = this.CalculateRandomDir();
		Vector2 vector2 = this.CalculateBounceDir();
		this.targetDir = (vector + vector2).normalized;
		if (Vector2.Angle(this.targetDir, base.transform.forward) > 10f)
		{
			bool flag = this.targetDir.x * base.transform.forward.z - this.targetDir.y * base.transform.forward.x > 0f;
			this.turnRate = ((!flag) ? (-this.maxTurnRate) : this.maxTurnRate);
		}
		else
		{
			this.turnRate = 0f;
		}
	}

	private Vector2 CalculateRandomDir()
	{
		if (Time.time - this.timeStamp > this.randomInterval)
		{
			this.timeStamp = Time.time;
			this.lastRandomDir = (((Random.value <= 0.5f) ? (-base.transform.right) : base.transform.right) + base.transform.forward * Random.value * this.turnScale).normalized;
			return new Vector2(this.lastRandomDir.x, this.lastRandomDir.z);
		}
		return new Vector2(this.lastRandomDir.x, this.lastRandomDir.z);
	}

	private Vector2 CalculateBounceDir()
	{
		Vector2 vector;
		vector..ctor(this.bounceCenter.x - base.transform.position.x, this.bounceCenter.z - base.transform.position.z);
		if (Mathf.Abs(vector.x) < 0.5f && Mathf.Abs(vector.y) < 0.5f)
		{
			return Vector2.zero;
		}
		float magnitude = vector.magnitude;
		float num = this.bounceRange - magnitude;
		float num2 = Mathf.InverseLerp(1f, 0f, num);
		Vector2 result = Vector2.zero;
		if (magnitude != 0f)
		{
			result = vector * (num2 * 2f / magnitude);
		}
		return result;
	}

	public float speed = 5f;

	public float maxTurnRate = 1f;

	public float range = 10f;

	public float turnScale = 1f;

	private float turnRate;

	private Vector2 targetDir;

	private Vector3 lastRandomDir;

	private Vector3 bounceCenter;

	private float bounceRange;

	private float randomInterval = 0.5f;

	private float timeStamp;
}
