using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoPhoGames.USpeak.Core;
using MoPhoGames.USpeak.Interface;
using UnityEngine;

[AddComponentMenu("USpeak/USpeaker")]
public class USpeaker : ManualUpdateBehaviour
{
	public bool IsTalking
	{
		get
		{
			return this.RecordStarted;
		}
	}

	public int audioFrequency
	{
		get
		{
			if (this.recFreq == 0)
			{
				BandMode bandwidthMode = this.BandwidthMode;
				if (bandwidthMode != BandMode.Narrow)
				{
					if (bandwidthMode != BandMode.Wide)
					{
						this.recFreq = USpeaker.VoiceWide;
					}
					else
					{
						this.recFreq = USpeaker.VoiceWide;
					}
				}
				else
				{
					this.recFreq = USpeaker.VoiceWide;
				}
			}
			return this.recFreq;
		}
	}

	public void SetInputDevice(int deviceID)
	{
		USpeaker.InputDeviceID = deviceID;
	}

	public static USpeaker Get(Object source)
	{
		if (source is GameObject)
		{
			return (source as GameObject).GetComponent<USpeaker>();
		}
		if (source is Transform)
		{
			return (source as Transform).GetComponent<USpeaker>();
		}
		if (source is Component)
		{
			return (source as Component).GetComponent<USpeaker>();
		}
		return null;
	}

	public void GetInputHandler()
	{
		this.talkController = (IUSpeakTalkController)this.FindInputHandler();
	}

	public void DrawTalkControllerUI()
	{
		if (this.talkController != null)
		{
			this.talkController.OnInspectorGUI();
		}
		else
		{
			GUILayout.Label("No component available which implements IUSpeakTalkController\nReverting to default behavior - data is always sent", new GUILayoutOption[0]);
		}
	}

	public List<AudioClip> ReceiveAudio(byte[] data)
	{
		List<AudioClip> list = new List<AudioClip>();
		if (this.settings == null)
		{
			Debug.LogWarning("Trying to receive remote audio data without calling InitializeSettings!\nIncoming packet will be ignored");
			this.UpdateSettings();
		}
		if (USpeaker.MuteAll || this.Mute)
		{
			return null;
		}
		byte[] array;
		for (int i = 0; i < data.Length; i += array.Length)
		{
			int num = BitConverter.ToInt32(data, i);
			array = new byte[num + 6];
			Array.Copy(data, i, array, 0, array.Length);
			USpeakFrameContainer uspeakFrameContainer = default(USpeakFrameContainer);
			uspeakFrameContainer.LoadFrom(array);
			list.Add(USpeakAudioClipCompressor.DecompressAudioClip(uspeakFrameContainer.encodedData, (int)uspeakFrameContainer.Samples, 1, false, this.settings.bandMode, USpeaker.RemoteGain));
		}
		return list;
	}

	public void InitializeSettings(int data)
	{
		MonoBehaviour.print("Settings changed");
		this.settings = new USpeakSettingsData((byte)data);
	}

	public void StartRecord()
	{
		if (this.recording != null)
		{
			Object.DestroyImmediate(this.recording);
			this.recording = null;
		}
		Microphone.End(null);
		this.recording = Microphone.Start(this.currentDeviceName, false, (int)this.SendTimeRange + 1, this.audioFrequency);
		this.RecordStarted = true;
		this.sendTimer = 0f;
		this.lastRecoredTimer = 0;
		this.lastReadPos = 0;
		this.EncodeBytes = null;
	}

	public void EndRecord()
	{
		this.sendTimer = 0f;
		this.lastRecoredTimer = 0;
		this.RecordStarted = false;
		this.CalculateRecord();
		Microphone.End(null);
	}

	private void CalculateRecord()
	{
		this.EncodeBytes = this.GetEncodeBytes();
	}

	private List<byte> GetEncodeBytes()
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<byte> list = new List<byte>();
		foreach (USpeakFrameContainer uspeakFrameContainer in this.sendBuffer)
		{
			list.AddRange(uspeakFrameContainer.ToByteArray());
		}
		this.sendBuffer.Clear();
		USpeaker.CompressLength = Time.realtimeSinceStartup - realtimeSinceStartup;
		return list;
	}

	public List<AudioClip> GetDecodeBytes(byte[] encodeBytes)
	{
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		List<AudioClip> result = new List<AudioClip>();
		if (encodeBytes.Length > 0)
		{
			result = this.ReceiveAudio(encodeBytes);
		}
		USpeaker.DecompressionLength = Time.realtimeSinceStartup - realtimeSinceStartup;
		return result;
	}

	private void Awake()
	{
		USpeaker.USpeakerList.Add(this);
	}

	private void OnDestroy()
	{
		USpeaker.USpeakerList.Remove(this);
	}

	private IEnumerator Start()
	{
		yield return null;
		this.audioHandler = (ISpeechDataHandler)this.FindSpeechHandler();
		this.talkController = (IUSpeakTalkController)this.FindInputHandler();
		if (this.audioHandler == null)
		{
			Debug.LogError("USpeaker requires a component which implements the ISpeechDataHandler interface");
			yield break;
		}
		if (this.SpeakerMode == SpeakerMode.Remote)
		{
			yield break;
		}
		if (Microphone.devices.Length == 0)
		{
			Debug.LogWarning("Failed to find a recording device");
			yield break;
		}
		this.UpdateSettings();
		this.currentDeviceName = Microphone.devices[USpeaker.InputDeviceID];
		yield break;
	}

	public bool GetMicrophone()
	{
		if (!Application.HasUserAuthorization(2))
		{
			Application.RequestUserAuthorization(2);
			Debug.LogError("Failed to start recording - user has denied microphone access");
			return false;
		}
		if (Microphone.devices.Length == 0)
		{
			Debug.LogWarning("Failed to find a recording device");
			return false;
		}
		return true;
	}

	private new void OnEnable()
	{
		base.OnEnable();
		if (this.audioHandler == null)
		{
			this.audioHandler = (ISpeechDataHandler)this.FindSpeechHandler();
		}
	}

	public override void ManualUpdate()
	{
		if (this.audioHandler == null)
		{
			return;
		}
		if (!this.RecordStarted)
		{
			return;
		}
		if (Microphone.devices.Length == 0)
		{
			return;
		}
		if (this.RecordStarted && Microphone.devices[Mathf.Min(USpeaker.InputDeviceID, Microphone.devices.Length - 1)] != this.currentDeviceName)
		{
			this.currentDeviceName = Microphone.devices[Mathf.Min(USpeaker.InputDeviceID, Microphone.devices.Length - 1)];
			this.recording = Microphone.Start(this.currentDeviceName, false, (int)this.SendTimeRange + 1, this.audioFrequency);
			this.lastReadPos = 0;
		}
		int position = Microphone.GetPosition(null);
		if (position <= this.overlap)
		{
			return;
		}
		try
		{
			int num = position - this.lastReadPos;
			if (num > 1 && this.RecordStarted)
			{
				float[] array = new float[num - 1];
				this.recording.GetData(array, this.lastReadPos);
				this.OnAudioAvailable(array);
			}
			this.lastReadPos = position;
		}
		catch (Exception)
		{
			Debug.LogError("Exception");
		}
		this.ProcessPendingEncodeBuffer();
		this.sendTimer += Time.deltaTime;
		if (this.sendTimer - (float)this.lastRecoredTimer > 1f)
		{
			this.lastRecoredTimer = (int)this.sendTimer;
			if (this.TimeEachSecondInstance != null)
			{
				this.TimeEachSecondInstance(this.lastRecoredTimer);
			}
		}
		if (this.sendTimer >= this.SendTimeRange && this.RecordStarted && this.TimeOutDelegateInstance != null)
		{
			this.TimeOutDelegateInstance();
		}
	}

	public float Volume
	{
		get
		{
			int num = 128;
			float num2 = 0f;
			float[] array = new float[num];
			int num3 = Microphone.GetPosition(null) - (num + 1);
			if (num3 < 0)
			{
				return 0f;
			}
			this.recording.GetData(array, num3);
			for (int i = 0; i < num; i++)
			{
				float num4 = array[i] * array[i];
				if (num2 < num4)
				{
					num2 = num4;
				}
			}
			return num2 * 100f;
		}
	}

	private void UpdateSettings()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		this.settings = new USpeakSettingsData();
		this.settings.bandMode = this.BandwidthMode;
		this.settings.Is3D = this.Is3D;
	}

	private Component FindSpeechHandler()
	{
		Component[] componentsInChildren = base.GetComponentsInChildren<Component>(true);
		foreach (Component component in componentsInChildren)
		{
			if (component is ISpeechDataHandler)
			{
				return component;
			}
		}
		return null;
	}

	private Component FindInputHandler()
	{
		Component[] componentsInChildren = base.GetComponentsInChildren<Component>(true);
		foreach (Component component in componentsInChildren)
		{
			if (component is IUSpeakTalkController)
			{
				return component;
			}
		}
		return null;
	}

	private void OnAudioAvailable(float[] pcmData)
	{
		if (this.UseVAD && !this.CheckVAD(pcmData))
		{
			return;
		}
		int size = 640;
		List<float[]> list = this.SplitArray(pcmData, size);
		foreach (float[] array in list)
		{
			this.pendingEncode.Add(array);
		}
	}

	private List<float[]> SplitArray(float[] array, int size)
	{
		List<float[]> list = new List<float[]>();
		float[] array2;
		for (int i = 0; i < array.Length; i += array2.Length)
		{
			array2 = Enumerable.ToArray<float>(Enumerable.Take<float>(Enumerable.Skip<float>(array, i), size));
			list.Add(array2);
		}
		return list;
	}

	private void ProcessPendingEncodeBuffer()
	{
		int num = 10;
		float num2 = (float)num / 1000f;
		float realtimeSinceStartup = Time.realtimeSinceStartup;
		while (Time.realtimeSinceStartup <= realtimeSinceStartup + num2 && this.pendingEncode.Count > 0)
		{
			float[] pcm = this.pendingEncode[0];
			this.pendingEncode.RemoveAt(0);
			this.ProcessPendingEncode(pcm);
		}
	}

	private void ProcessPendingEncode(float[] pcm)
	{
		int num;
		byte[] encodedData = USpeakAudioClipCompressor.CompressAudioData(pcm, 1, out num, this.BandwidthMode, USpeaker.LocalGain);
		USpeakFrameContainer uspeakFrameContainer = default(USpeakFrameContainer);
		uspeakFrameContainer.Samples = (ushort)num;
		uspeakFrameContainer.encodedData = encodedData;
		this.sendBuffer.Add(uspeakFrameContainer);
	}

	private int CalculateSamplesRead(int readPos)
	{
		if (readPos >= this.lastReadPos)
		{
			return readPos - this.lastReadPos;
		}
		return this.audioFrequency * 10 - this.lastReadPos + readPos;
	}

	private float[] normalize(float[] samples, float magnitude)
	{
		float[] array = new float[samples.Length];
		for (int i = 0; i < samples.Length; i++)
		{
			array[i] = samples[i] / magnitude;
		}
		return array;
	}

	private float amplitude(float[] x)
	{
		float num = 0f;
		for (int i = 0; i < x.Length; i++)
		{
			num = Mathf.Max(num, Mathf.Abs(x[i]));
		}
		return num;
	}

	private bool CheckVAD(float[] samples)
	{
		if (Time.realtimeSinceStartup < this.lastVTime + this.vadHangover)
		{
			return true;
		}
		float num = 0f;
		foreach (float num2 in samples)
		{
			num = Mathf.Max(num, Mathf.Abs(num2));
		}
		bool flag = num >= 0.005f;
		if (flag)
		{
			this.lastVTime = Time.realtimeSinceStartup;
		}
		return flag;
	}

	public byte[] ReceiveAudioByte(byte[] data)
	{
		List<byte> list = new List<byte>();
		byte[] array;
		for (int i = 0; i < data.Length; i += array.Length)
		{
			int num = BitConverter.ToInt32(data, i);
			array = new byte[num + 6];
			Array.Copy(data, i, array, 0, array.Length);
			USpeakFrameContainer uspeakFrameContainer = default(USpeakFrameContainer);
			uspeakFrameContainer.LoadFrom(array);
			list.AddRange(Enumerable.ToList<byte>(USpeakAudioClipCompressor.DecompressAudioClipbyte(uspeakFrameContainer.encodedData, (int)uspeakFrameContainer.Samples, 1, false, this.settings.bandMode, USpeaker.RemoteGain)));
		}
		return list.ToArray();
	}

	public static float RemoteGain = 1f;

	public static float LocalGain = 1f;

	public static bool MuteAll = false;

	public static List<USpeaker> USpeakerList = new List<USpeaker>();

	private static int InputDeviceID = 0;

	public USpeaker.TimeOutDelegate TimeOutDelegateInstance;

	public USpeaker.TimeEachSecondUpdateDelegate TimeEachSecondInstance;

	public SpeakerMode SpeakerMode;

	public BandMode BandwidthMode;

	public static int VoiceWide = 8000;

	public float SendRate = 16f;

	public SendBehavior SendingMode = SendBehavior.RecordThenSend;

	public bool UseVAD;

	public bool Is3D;

	public bool AskPermission = true;

	public float SendTimeRange = 30f;

	public bool Mute;

	public float PlayBufferSize = 1f;

	private int recFreq;

	private AudioClip recording;

	private int lastReadPos;

	private List<AudioClip> playBuffer = new List<AudioClip>();

	private float sendTimer;

	private int lastRecoredTimer;

	private List<USpeakFrameContainer> sendBuffer = new List<USpeakFrameContainer>();

	private List<byte> tempSendBytes = new List<byte>();

	private ISpeechDataHandler audioHandler;

	private IUSpeakTalkController talkController;

	private int overlap;

	private USpeakSettingsData settings;

	private string currentDeviceName = string.Empty;

	private float vadHangover = 0.5f;

	private float lastVTime;

	private List<float[]> pendingEncode = new List<float[]>();

	private bool RecordStarted;

	public List<byte> EncodeBytes;

	public static float CompressLength = 0f;

	public static float DecompressionLength = 0f;

	public delegate void TimeOutDelegate();

	public delegate void TimeEachSecondUpdateDelegate(int remainTime);
}
