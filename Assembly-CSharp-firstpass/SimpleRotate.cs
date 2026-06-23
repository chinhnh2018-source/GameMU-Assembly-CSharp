using System;
using UnityEngine;

public class SimpleRotate : MonoBehaviour
{
	private void Start()
	{
		this.OriAngleX = base.transform.rotation.x;
		this.OriAngleY = base.transform.rotation.y;
	}

	private void Update()
	{
		if (this.RotateX)
		{
			this.OriAngleX += Time.deltaTime * this.RotateSpeed;
		}
		if (this.RotateY)
		{
			this.OriAngleY -= Time.deltaTime * this.RotateSpeed;
		}
		base.transform.rotation = Quaternion.Euler(this.OriAngleX, this.OriAngleY, base.transform.rotation.z);
	}

	protected float OriAngleX;

	protected float OriAngleY;

	public float RotateSpeed = 20f;

	public bool RotateX = true;

	public bool RotateY;
}
