using System;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

public class UnityNetworkUSpeakSender : MonoBehaviour, ISpeechDataHandler
{
	private void Start()
	{
		if (!base.GetComponent<NetworkView>().isMine)
		{
			USpeaker.Get(this).SpeakerMode = SpeakerMode.Remote;
		}
	}

	public void USpeakOnSerializeAudio(byte[] data)
	{
		base.GetComponent<NetworkView>().RPC("vc", 2, new object[]
		{
			data
		});
	}

	public void USpeakInitializeSettings(int data)
	{
		base.GetComponent<NetworkView>().RPC("init", 6, new object[]
		{
			data
		});
	}

	[RPC]
	private void vc(byte[] data)
	{
		USpeaker.Get(this).ReceiveAudio(data);
	}

	[RPC]
	private void init(int data)
	{
		USpeaker.Get(this).InitializeSettings(data);
	}
}
