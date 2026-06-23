using System;
using UnityEngine;

public class DebugInfo : MonoBehaviour
{
	public static DebugInfo Instance
	{
		get
		{
			return DebugInfo._Instance;
		}
	}

	private void Awake()
	{
		DebugInfo._Instance = this;
	}

	private static DebugInfo _Instance;

	public bool ShowZhangAiWu;

	public bool ShowAnQuanQu;
}
