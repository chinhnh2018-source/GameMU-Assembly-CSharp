using System;
using System.Collections.Generic;
using UnityEngine;

namespace MoPhoGames.USpeak.Core
{
	public class USpeakAudioManager
	{
		public static AudioSource PlayClipAtPoint(AudioClip clip, Vector3 position, ulong delay, bool calcPan = false, bool usePooledClips = true)
		{
			AudioSource audioSource = USpeakAudioManager.GetAudioSource();
			audioSource.transform.position = position;
			if (usePooledClips)
			{
				if (audioSource.clip != null)
				{
					USpeakAudioManager.PoolAudioClip(audioSource.clip);
				}
			}
			else if (audioSource.clip != null)
			{
				Object.DestroyImmediate(audioSource.clip);
			}
			audioSource.clip = clip;
			if (calcPan)
			{
				audioSource.panStereo = -Vector3.Dot(Vector3.Cross(Camera.main.transform.forward, Vector3.up).normalized, (position - Camera.main.transform.position).normalized);
			}
			audioSource.PlayDelayed(delay * 44100f);
			return audioSource;
		}

		public static void StopAll()
		{
			foreach (AudioSource audioSource in USpeakAudioManager.audioPool)
			{
				audioSource.Stop();
			}
		}

		public static AudioClip GetOrCreateAudioClip(int lenSamples, int channels, int frequency, bool threeD)
		{
			AudioClip audioClip = USpeakAudioManager.ReturnPooledAudioClip(lenSamples, channels);
			if (audioClip != null)
			{
				USpeakAudioManager.RemoveFromPool(audioClip);
				return audioClip;
			}
			return USpeakAudioManager.CreateAudioClip(lenSamples, channels, frequency, threeD);
		}

		public static AudioClip CreateAudioClip(int lenSamples, int channels, int frequency, bool threeD)
		{
			return AudioClip.Create("clip", lenSamples / channels, channels, frequency, threeD, false);
		}

		public static void RemoveFromPool(AudioClip clip)
		{
			USpeakAudioManager.clipPool.Remove(clip);
		}

		public static AudioClip ReturnPooledAudioClip(int samples, int channels)
		{
			foreach (AudioClip audioClip in USpeakAudioManager.clipPool)
			{
				if (audioClip.channels == channels && audioClip.samples == samples / channels)
				{
					return audioClip;
				}
			}
			return null;
		}

		public static void PoolAudioClip(AudioClip clip)
		{
			USpeakAudioManager.clipPool.Add(clip);
		}

		private static AudioSource GetAudioSource()
		{
			AudioSource audioSource = USpeakAudioManager.FindInactiveAudioFromPool();
			if (audioSource == null)
			{
				audioSource = new GameObject
				{
					hideFlags = 1
				}.AddComponent<AudioSource>();
				USpeakAudioManager.audioPool.Add(audioSource);
			}
			return audioSource;
		}

		private static AudioSource FindInactiveAudioFromPool()
		{
			USpeakAudioManager.Cleanup();
			foreach (AudioSource audioSource in USpeakAudioManager.audioPool)
			{
				if (!audioSource.isPlaying)
				{
					return audioSource;
				}
			}
			return null;
		}

		private static void Cleanup()
		{
			USpeakAudioManager.audioPool.RemoveAll((AudioSource src) => src == null);
		}

		private static List<AudioSource> audioPool = new List<AudioSource>();

		private static List<AudioClip> clipPool = new List<AudioClip>();
	}
}
