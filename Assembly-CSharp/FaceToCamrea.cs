using System;
using System.Collections.Generic;
using UnityEngine;

public class FaceToCamrea : MonoBehaviour
{
	private void Start()
	{
		this.orgDistance = Vector3.Distance(this.enemy.transform.position, base.transform.position);
		this.orgScaleZ = new List<float>();
		this.child = new List<Transform>();
		foreach (object obj in base.transform)
		{
			Transform transform = (Transform)obj;
			this.child.Add(transform);
			this.orgScaleZ.Add(transform.localScale.z);
		}
	}

	private void Update()
	{
		if (this.mainCamera != null)
		{
			for (int i = 0; i < this.child.Count; i++)
			{
				if (this.AutoFaceToCamera)
				{
					base.transform.rotation = Quaternion.LookRotation(this.enemy.transform.position - base.transform.position, base.transform.position - this.mainCamera.transform.position);
				}
				else
				{
					base.transform.rotation = Quaternion.LookRotation(this.enemy.transform.position - base.transform.position);
				}
				if (this.AutoStretch && this.orgDistance != 0f)
				{
					this.child[i].localScale = new Vector3(this.child[i].localScale.x, this.child[i].localScale.y, this.orgScaleZ[i] * (Vector3.Distance(this.enemy.transform.position, base.transform.position) / this.orgDistance));
				}
			}
		}
	}

	public GameObject mainCamera;

	public GameObject enemy;

	public bool AutoFaceToCamera = true;

	public bool AutoStretch = true;

	private List<float> orgScaleZ;

	private float orgDistance;

	private List<Transform> child;
}
