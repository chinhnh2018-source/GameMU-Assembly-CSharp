using System;
using System.Collections.Generic;
using UnityEngine;

public class Console : MonoBehaviour
{
	private void OnEnable()
	{
		Application.RegisterLogCallback(new Application.LogCallback(this.HandleLog));
	}

	private void OnDisable()
	{
		Application.RegisterLogCallback(null);
	}

	private void Start()
	{
		Console.MyConsole = this;
	}

	private void OnGUI()
	{
		if (!this.show)
		{
			return;
		}
		this.windowRect = GUILayout.Window(123456, this.windowRect, new GUI.WindowFunction(this.ConsoleWindow), "Console", new GUILayoutOption[0]);
	}

	private void ConsoleWindow(int windowID)
	{
		GUI.skin.label.fontSize = 15;
		this.scrollPos = GUILayout.BeginScrollView(this.scrollPos, new GUILayoutOption[0]);
		for (int i = 0; i < this.entries.Count; i++)
		{
			Console.ConsoleMessage consoleMessage = this.entries[i];
			if (!this.collapse || i <= 0 || !(consoleMessage.message == this.entries[i - 1].message))
			{
				switch (consoleMessage.type)
				{
				case 0:
				case 4:
					GUI.contentColor = Color.red;
					break;
				case 1:
				case 3:
					goto IL_BF;
				case 2:
					GUI.contentColor = Color.yellow;
					break;
				default:
					goto IL_BF;
				}
				IL_CE:
				GUILayout.Label(consoleMessage.message, new GUILayoutOption[0]);
				if (consoleMessage.type == null || consoleMessage.type == 4)
				{
					GUILayout.Label("-------", new GUILayoutOption[0]);
					GUILayout.Label(consoleMessage.stackTrace, new GUILayoutOption[0]);
					GUILayout.Label("-------", new GUILayoutOption[0]);
					goto IL_12B;
				}
				goto IL_12B;
				IL_BF:
				GUI.contentColor = Color.white;
				goto IL_CE;
			}
			IL_12B:;
		}
		GUI.contentColor = Color.white;
		GUILayout.EndScrollView();
		GUILayout.BeginHorizontal(new GUILayoutOption[0]);
		if (GUILayout.Button(this.clearLabel, new GUILayoutOption[0]))
		{
			this.entries.Clear();
		}
		this.collapse = GUILayout.Toggle(this.collapse, this.collapseLabel, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(false)
		});
		GUILayout.EndHorizontal();
		GUI.DragWindow(new Rect(0f, 0f, 10000f, 20f));
	}

	private void HandleLog(string message, string stackTrace, LogType type)
	{
		Console.ConsoleMessage consoleMessage = new Console.ConsoleMessage(message, stackTrace, type);
		this.entries.Add(consoleMessage);
		if (this.entries.Count > 100)
		{
			this.entries.RemoveRange(0, 100);
		}
	}

	private const int margin = 20;

	public static readonly Version version = new Version(1, 0);

	private List<Console.ConsoleMessage> entries = new List<Console.ConsoleMessage>();

	private Vector2 scrollPos;

	public bool show;

	private bool collapse;

	public static Console MyConsole = null;

	private Rect windowRect = new Rect(20f, 20f, (float)(Screen.width - 40), (float)(Screen.height - 80));

	private GUIContent clearLabel = new GUIContent("Clear", "Clear the contents of the console.");

	private GUIContent collapseLabel = new GUIContent("Collapse", "Hide repeated messages.");

	private struct ConsoleMessage
	{
		public ConsoleMessage(string message, string stackTrace, LogType type)
		{
			this.message = message;
			this.stackTrace = stackTrace;
			this.type = type;
		}

		public readonly string message;

		public readonly string stackTrace;

		public readonly LogType type;
	}
}
