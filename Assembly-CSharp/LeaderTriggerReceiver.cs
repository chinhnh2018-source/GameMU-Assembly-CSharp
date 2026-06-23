using System;
using System.Collections.Generic;
using UnityEngine;

public class LeaderTriggerReceiver : MonoBehaviour
{
	private void Awake()
	{
		Collider[] componentsInChildren = base.GetComponentsInChildren<Collider>();
		for (int i = 0; i < componentsInChildren.Length; i++)
		{
			LeaderTriggerSender leaderTriggerSender = componentsInChildren[i].gameObject.AddComponent<LeaderTriggerSender>();
			leaderTriggerSender.receiver = this;
		}
		if (this.hideObjects != null)
		{
			for (int j = 0; j < this.hideObjects.Length; j++)
			{
				this.hideRenderers.AddRange(this.hideObjects[j].GetComponentsInChildren<Renderer>());
			}
		}
	}

	public void OnLeaderEnter()
	{
		if (this.mEnteredTriggerCount == 0)
		{
			for (int i = 0; i < this.hideRenderers.Count; i++)
			{
				this.hideRenderers[i].enabled = false;
			}
		}
		this.mEnteredTriggerCount++;
	}

	public void OnLeaderLeave()
	{
		this.mEnteredTriggerCount--;
		if (this.mEnteredTriggerCount == 0)
		{
			for (int i = 0; i < this.hideRenderers.Count; i++)
			{
				this.hideRenderers[i].enabled = true;
			}
		}
	}

	public GameObject[] hideObjects;

	private int mEnteredTriggerCount;

	private List<Renderer> hideRenderers = new List<Renderer>();
}
