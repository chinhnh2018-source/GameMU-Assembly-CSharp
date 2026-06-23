using System;
using UnityEngine;

public class tailtransformer : MonoBehaviour
{
	private void Start()
	{
		base.Invoke("DestoryGameObj", this.LifeTime);
	}

	private void Update()
	{
		if (base.transform == null)
		{
			return;
		}
		if (Time.time >= this.timetemp + this.redirectRate)
		{
			this.target = new Vector3(Random.Range(this.DirectionRandomMin.x, this.DirectionRandomMax.x), Random.Range(this.DirectionRandomMin.y, this.DirectionRandomMax.y), Random.Range(this.DirectionRandomMin.z, this.DirectionRandomMax.z));
			this.timetemp = Time.time;
		}
		Quaternion quaternion = Quaternion.LookRotation(this.target - base.transform.position);
		float num = Mathf.Min(10f * Time.deltaTime, 1f);
		base.transform.rotation = Quaternion.Lerp(base.transform.rotation, quaternion, num);
		base.transform.position += base.transform.forward * this.Speed * Time.deltaTime;
	}

	private void DestoryGameObj()
	{
		if (base.IsInvoking("DestoryGameObj"))
		{
			base.CancelInvoke("DestoryGameObj");
		}
		Object.Destroy(base.gameObject);
	}

	private Vector3 target;

	public Vector3 DirectionRandomMax;

	public Vector3 DirectionRandomMin;

	public float Speed = 1f;

	private float timetemp;

	public float redirectRate = 0.5f;

	public float LifeTime = 1f;
}
