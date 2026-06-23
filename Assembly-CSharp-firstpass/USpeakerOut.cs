using System;
using System.Collections.Generic;
using MoPhoGames.USpeak.Core;
using UnityEngine;

[AddComponentMenu("USpeak/USpeakerOut")]
public class USpeakerOut : MonoBehaviour
{
	public void PlayRecord(List<AudioClip> playBuffer, int audioFrequency)
	{
		List<float> list = new List<float>();
		foreach (AudioClip audioClip in playBuffer)
		{
			float[] array = new float[audioClip.samples];
			audioClip.GetData(array, 0);
			Object.DestroyImmediate(audioClip);
			list.AddRange(array);
		}
		if (list.Count > 0)
		{
			AudioClip audioClip2 = AudioClip.Create("clip", list.Count, 1, audioFrequency, false, false);
			audioClip2.SetData(list.ToArray(), 0);
			USpeakAudioManager.PlayClipAtPoint(audioClip2, base.transform.position, 0UL, false, false);
		}
		playBuffer.Clear();
	}

	public AudioSource PlayRecord(List<float> temp_stitch, int audioFrequency)
	{
		AudioSource result = null;
		if (temp_stitch.Count > 0)
		{
			AudioClip audioClip = AudioClip.Create("clip", temp_stitch.Count, 1, audioFrequency, false, false);
			audioClip.SetData(temp_stitch.ToArray(), 0);
			result = USpeakAudioManager.PlayClipAtPoint(audioClip, base.transform.position, 0UL, false, false);
		}
		return result;
	}

	public List<float> GetClipData(List<AudioClip> playBuffer, ref float clipLength, int audioFrequency)
	{
		List<float> list = new List<float>();
		foreach (AudioClip audioClip in playBuffer)
		{
			float[] array = new float[audioClip.samples];
			audioClip.GetData(array, 0);
			Object.DestroyImmediate(audioClip);
			list.AddRange(array);
		}
		playBuffer.Clear();
		clipLength = (float)list.Count / (float)audioFrequency;
		return list;
	}

	public static List<float> GetClipDataLength(List<AudioClip> playBuffer, ref float clipLength, int audioFrequency)
	{
		List<float> list = new List<float>();
		foreach (AudioClip audioClip in playBuffer)
		{
			float[] array = new float[audioClip.samples];
			audioClip.GetData(array, 0);
			Object.DestroyImmediate(audioClip);
			list.AddRange(array);
		}
		playBuffer.Clear();
		clipLength = (float)list.Count / (float)audioFrequency;
		return list;
	}
}
