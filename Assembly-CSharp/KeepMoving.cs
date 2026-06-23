using System;
using System.Collections;
using UnityEngine;

public class KeepMoving : MonoBehaviour
{
	private void Start()
	{
		this.playing = true;
		this.startPoint = base.transform.position;
		base.StartCoroutine(this.DecideTurnRate());
	}

	private void Update()
	{
		this.randomSpeed = Random.Range(this.speedMin, this.speedMax);
		this.outOfRange = (Vector3.Distance(this.startPoint, base.transform.localPosition) > this.rangeRadius - 1f);
		if (this.outOfRange)
		{
			this.inCircle++;
			if (this.inCircle > 60)
			{
				Vector3 vector = this.startPoint - base.transform.localPosition;
				float num = Vector3.Angle(base.transform.forward, vector);
				base.transform.position += base.transform.forward * (this.randomSpeed * Time.deltaTime);
				if (num > 45f)
				{
					base.transform.RotateAroundLocal(Vector3.up, Time.deltaTime * 5f);
				}
			}
			else
			{
				base.transform.position += base.transform.forward * (this.randomSpeed * Time.deltaTime);
				base.transform.RotateAroundLocal(Vector3.up, Time.deltaTime * 5f);
			}
		}
		else
		{
			if (this.inCircle > 0)
			{
				this.inCircle--;
			}
			base.transform.position += base.transform.forward * (this.randomSpeed * Time.deltaTime);
			base.transform.RotateAroundLocal(Vector3.up, Time.deltaTime * this.turnRate);
		}
	}

	private IEnumerator DecideTurnRate()
	{
		this.turnTimeLength = 0.1f;
		while (base.enabled)
		{
			yield return new WaitForSeconds(this.turnTimeLength);
			this.turnTimeLength = Random.Range(this.minTurnLength, this.maxTurnLength);
			this.turnRate = Random.Range(-this.maxTurnRate, this.maxTurnRate);
		}
		yield break;
	}

	public void OnDrawGizmos()
	{
		if (!this.playing)
		{
			this.startPoint = base.transform.position;
		}
		Gizmos.color = Color.yellow;
		Gizmos.DrawWireSphere(this.startPoint, this.rangeRadius);
	}

	public float speedMax = 10f;

	public float speedMin = 1f;

	public float maxTurnRate = 3f;

	public float minTurnLength = 0.5f;

	public float maxTurnLength = 5f;

	private float turnRate;

	private float turnTimeLength;

	public float rangeRadius = 20f;

	private Vector3 startPoint;

	private bool outOfRange;

	private bool playing;

	private int inCircle;

	private float randomSpeed;
}
