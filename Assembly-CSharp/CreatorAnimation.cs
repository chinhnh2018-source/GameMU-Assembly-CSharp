using System;
using UnityEngine;

public class CreatorAnimation : ManualUpdateBehaviour
{
	private void Start()
	{
		base.GetComponent<Animation>()["Stand2"].wrapMode = 2;
		AnimationEvent animationEvent = new AnimationEvent();
		animationEvent.functionName = "Fly1Finish";
		animationEvent.time = base.GetComponent<Animation>()["Fly1"].length;
		base.GetComponent<Animation>().GetClip("Fly1").AddEvent(animationEvent);
		base.GetComponent<Animation>().Play("Fly1");
	}

	public override void ManualUpdate()
	{
		base.transform.position = Vector3.MoveTowards(base.transform.position, this.poisition, Time.deltaTime * this.speed);
	}

	private void Fly1Finish()
	{
		if (!this.isFinish)
		{
			this.isFinish = true;
			base.GetComponent<Animation>().PlayQueued("Fly2");
			base.GetComponent<Animation>().PlayQueued("Stand2");
		}
	}

	public Vector3 poisition;

	public float speed;

	private bool isFinish;
}
