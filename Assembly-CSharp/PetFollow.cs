using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class PetFollow : MonoBehaviour
{
	private void Awake()
	{
		this.myTransform = base.transform;
		this.petEventArgs = new PetEventArgs();
	}

	private void Update()
	{
		if (this.target == null)
		{
			return;
		}
		this.targetPosition = this.target.localPosition + this.target.forward * this.offsetX + this.target.up * this.offsetY + this.target.right * this.offsetZ;
		float num = Vector3.Distance(this.myTransform.localPosition, this.targetPosition);
		if (num > this.stopRange)
		{
			this.myTransform.localRotation = Quaternion.Slerp(this.myTransform.localRotation, this.target.localRotation, 0.05f);
			this.myTransform.localPosition = Vector3.Lerp(this.myTransform.localPosition, this.targetPosition, 0.05f);
		}
		if (num > this.ActionRange)
		{
			if (this.PetItemEvent != null)
			{
				this.petEventArgs.StepType = 1;
				this.PetItemEvent(this, this.petEventArgs);
			}
		}
		else if (this.PetItemEvent != null)
		{
			this.petEventArgs.StepType = 2;
			this.PetItemEvent(this, this.petEventArgs);
		}
	}

	public Transform target;

	public float stopRange;

	public float offsetX;

	public float offsetY;

	public float offsetZ;

	public float ActionRange = 0.4f;

	private Vector3 targetPosition = Vector3.zero;

	public PetItemEventHandler PetItemEvent;

	private PetEventArgs petEventArgs;

	private Transform myTransform;
}
