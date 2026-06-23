using System;
using UnityEngine;

public class AnimationEventTrigger : MonoBehaviour
{
	private void Start()
	{
		this.AddAnimEvent(this.animClip.length / (float)this.totalFrame * (float)this.eventFrame, "ExecuteEvent");
	}

	private void AddAnimEvent(float frame, string functionName)
	{
		if (this.animClip != null)
		{
			AnimationEvent animationEvent = new AnimationEvent();
			animationEvent.functionName = functionName;
			animationEvent.time = frame;
			this.animClip.AddEvent(animationEvent);
		}
	}

	private void ExecuteEvent()
	{
		if (this.executeObj != null)
		{
			this.executeObj.SetActive(true);
			ParticleSize component = base.GetComponent<ParticleSize>();
			if (component != null && !this.isParticleSize)
			{
				this.isParticleSize = true;
				component.SetParticleSize();
			}
		}
	}

	public AnimationClip animClip;

	public int totalFrame;

	public int eventFrame;

	public GameObject executeObj;

	private bool isParticleSize;
}
