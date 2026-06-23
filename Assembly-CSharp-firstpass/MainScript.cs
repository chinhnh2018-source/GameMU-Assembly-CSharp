using System;
using UnityEngine;

public class MainScript : MonoBehaviour
{
	private void Start()
	{
	}

	private void OnGUI()
	{
		if (GUILayout.Button("Begin Clip Record", new GUILayoutOption[]
		{
			GUILayout.Height(200f)
		}))
		{
			this.Speaker.StartRecord();
		}
		if (GUILayout.Button("Play Clip Record", new GUILayoutOption[]
		{
			GUILayout.Height(200f)
		}))
		{
		}
		if (GUILayout.Button("Clip Record Quality", new GUILayoutOption[]
		{
			GUILayout.Height(200f)
		}))
		{
			if (this.Speaker.BandwidthMode == BandMode.Narrow)
			{
				this.Speaker.BandwidthMode = BandMode.Wide;
			}
			else
			{
				this.Speaker.BandwidthMode = BandMode.Narrow;
			}
		}
	}

	public USpeaker Speaker;

	public USpeakerOut SpeakerOut;
}
