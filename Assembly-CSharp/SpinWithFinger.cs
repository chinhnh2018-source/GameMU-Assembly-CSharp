using System;
using UnityEngine;

public class SpinWithFinger : MonoBehaviour
{
	private void Start()
	{
		this.mTrans = base.transform;
		if (this.target == null)
		{
			this.target = this.mTrans;
		}
	}

	private void Update()
	{
		if (Input.touchCount == 1)
		{
			this.t = Input.GetTouch(0);
			TouchPhase phase = this.t.phase;
			if (phase != 1)
			{
				if (phase == 2)
				{
					this.OnDrag(this.t.deltaPosition * 3f);
				}
			}
			else
			{
				this.OnDrag(this.t.deltaPosition * 3f);
			}
		}
	}

	private void OnDrag(Vector2 delta)
	{
		this.target.localRotation = Quaternion.Euler(0f, -0.5f * delta.x * this.speed, 0f) * this.target.localRotation;
	}

	public Transform target;

	public float speed = 2f;

	private Transform mTrans;

	private float x;

	private float y;

	private Touch t;
}
