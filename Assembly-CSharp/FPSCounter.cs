using System;
using System.Collections;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.JavaPlugins;
using UnityEngine;

[AddComponentMenu("Utilities/HUDFPS")]
public class FPSCounter : TTMonoBehaviour
{
	private void Start()
	{
		this.lastFrameCount = Time.frameCount;
		this.lastTouchTimer = Time.time;
		this.startRect = new Rect((float)(Screen.width / 2) - 37f, 10f, 200f, 100f);
		base.StartCoroutine<bool>(this.FPS());
		FPSCounter.CountMemory = SystemInfo.systemMemorySize;
		FPSCounter.MemorySync();
	}

	private IEnumerator FPS()
	{
		for (;;)
		{
			float t = Time.time;
			int fc = Time.frameCount;
			FPSCounter.fps = (float)(fc - this.lastFrameCount) / (t - this.lastTouchTimer);
			if (FPSCounter.showFps)
			{
				this.sFPS = FPSCounter.fps.ToString("f" + Mathf.Clamp(this.nbDecimal, 0, 10));
				this.color = ((FPSCounter.fps < 30f) ? ((FPSCounter.fps <= 10f) ? Color.red : Color.yellow) : Color.green);
			}
			this.lastFrameCount = fc;
			this.lastTouchTimer = t;
			yield return new WaitForSeconds(this.frequency);
		}
		yield break;
	}

	public static void MemorySync()
	{
		FPSCounter.ResidualMemory = (int)QMQJJava.GetLeftMemery();
	}

	private void DoMyWindow(int windowID)
	{
		string text = string.Empty;
		int num = TCPClient.snTotalSendCount;
		if (TCPClient.snTotalSendCount > 100000)
		{
			num /= 1000;
			text = " K";
		}
		string text2 = string.Empty;
		int num2 = TCPClient.snTotalRecvCount;
		if (TCPClient.snTotalRecvCount > 100000)
		{
			num2 /= 1000;
			text2 = " K";
		}
		GUI.Label(new Rect(0f, -15f, this.startRect.width, this.startRect.height), this.sFPS + " FPS", this.style);
		GUI.Label(new Rect(0f, -5f, this.startRect.width, this.startRect.height), "up: " + num + text, this.style);
		GUI.Label(new Rect(0f, 5f, this.startRect.width, this.startRect.height), "down: " + num2 + text2, this.style);
		string text3 = string.Concat(new object[]
		{
			QMQJJava.GetLeftMemery(),
			"MB/",
			SystemInfo.systemMemorySize,
			"MB ",
			FPSCounter.ResidualMemory
		});
		GUI.Label(new Rect(0f, 15f, this.startRect.width, this.startRect.height), "memory: " + text3, this.style);
		DateTime correctDateTime = Global.GetCorrectDateTime();
		GUI.Label(new Rect(0f, 25f, this.startRect.width, this.startRect.height), "gm" + correctDateTime.ToString(), this.style);
		GUI.Label(new Rect(0f, 35f, this.startRect.width, this.startRect.height), correctDateTime.GetDateTimeFormats('r')[0].ToString(), this.style);
		if (this.allowDrag)
		{
			GUI.DragWindow(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height));
		}
	}

	public Rect startRect;

	public bool updateColor = true;

	public bool allowDrag = true;

	public float frequency = 2f;

	public int nbDecimal = 1;

	private Color color = Color.white;

	private string sFPS = string.Empty;

	private GUIStyle style;

	public static bool showFps;

	public static float fps = 40f;

	public static float _netTraffic;

	public static int ResidualMemory = -1;

	public static int CountMemory;

	private int lastFrameCount;

	private float lastTouchTimer;
}
