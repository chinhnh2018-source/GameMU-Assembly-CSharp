using System;
using UnityEngine;

public class Spawner : MonoBehaviour
{
	private void Start()
	{
		this.timetemp = Time.time;
	}

	private void Update()
	{
		if (Time.time > this.timetemp + this.SpawnRate)
		{
			if (this.ObjectSpawn && this.objcount < this.LimitObject)
			{
				Vector3 vector;
				vector..ctor(Random.Range(-this.PositionRandomSize.x, this.PositionRandomSize.x), Random.Range(-this.PositionRandomSize.y, this.PositionRandomSize.y), Random.Range(-this.PositionRandomSize.z, this.PositionRandomSize.z));
				Object @object = Object.Instantiate(this.ObjectSpawn, base.transform.position + vector + this.PositionOffset, base.transform.rotation);
				this.objcount++;
				Object.Destroy(@object, this.LifeTimeObject);
			}
			this.timetemp = Time.time;
		}
	}

	public GameObject ObjectSpawn;

	public float SpawnRate;

	public float LifeTimeObject = 1f;

	public int LimitObject = 3;

	private float timetemp;

	private int objcount;

	public Vector3 PositionRandomSize;

	public Vector3 PositionOffset;
}
