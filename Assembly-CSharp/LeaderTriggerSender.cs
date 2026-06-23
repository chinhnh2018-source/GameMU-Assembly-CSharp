using System;
using UnityEngine;

public class LeaderTriggerSender : MonoBehaviour
{
	private void OnTriggerEnter(Collider other)
	{
		if (other.transform.name.StartsWith("Leader"))
		{
			this.receiver.OnLeaderEnter();
		}
	}

	private void OnTriggerExit(Collider other)
	{
		if (other.transform.name.StartsWith("Leader"))
		{
			this.receiver.OnLeaderLeave();
		}
	}

	public LeaderTriggerReceiver receiver;
}
