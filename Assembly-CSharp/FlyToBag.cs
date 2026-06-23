using System;
using UnityEngine;

public class FlyToBag : MonoBehaviour
{
	public void StartMove()
	{
		this.start = new Vector3(-base.transform.localPosition.x, -base.transform.localPosition.y + 10f, -base.transform.localPosition.z);
		this.dir = this.start.normalized;
		this.Updir = new Vector3(-this.dir.y, this.dir.x, 0f);
		this.Len = Vector3.Distance(this.start, Vector3.zero);
		if (this.time != 0f)
		{
			this.XSpeed = this.Len / this.time;
			this.a = this.XSpeed / this.time * 2f;
		}
		this.YSpeed = this.XSpeed;
		this.isMove = true;
	}

	private void Update()
	{
		if (!this.isMove)
		{
			return;
		}
		this.time -= Time.deltaTime;
		if (this.time > 0f)
		{
			this.YSpeed -= Time.deltaTime * this.a;
			base.transform.localPosition += this.dir * Time.deltaTime * this.XSpeed;
			base.transform.localPosition += this.Updir * Time.deltaTime * this.YSpeed;
		}
		else
		{
			base.transform.localPosition = new Vector3(0f, 20f, -10f);
			this.ts.Play(true);
			this.isMove = false;
			this.isMiss = false;
			base.Invoke("MoveEnd", this.ts.duration);
		}
	}

	public void MoveEnd()
	{
		if (this.isMiss)
		{
			Object.Destroy(base.gameObject);
		}
		else
		{
			this.isMiss = true;
			this.ts = UITweener.Begin<TweenScale>(this.ts.gameObject, 0.3f);
			this.ts.to = new Vector3(1f, 1f, 1f);
			base.Invoke("MoveEnd", this.ts.duration);
			this.ts.Play(true);
			this.ta.Play(true);
		}
	}

	public TweenScale ts;

	public TweenAlpha ta;

	public float time = 2f;

	private float a;

	private float XSpeed;

	private float YSpeed;

	private Vector3 start;

	private Vector3 dir;

	private Vector3 Updir;

	private bool isMove;

	private float Len;

	private bool isMiss;
}
