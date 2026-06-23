using System;
using UnityEngine;

public class HeadDisplayText : MonoBehaviour
{
	private void Start()
	{
		if (HeadDisplayText.Prefab == null)
		{
			HeadDisplayText.Prefab = (Resources.Load("Prefabs/HUD/HUDTextCustom") as GameObject);
		}
		this.ShowDisplayInfo();
	}

	private void OnBecameVisible()
	{
		this.ShowDisplayInfo();
	}

	private void OnBecameInvisible()
	{
		this.HideDisplayInfo();
	}

	private void OnDestroy()
	{
		this.HideDisplayInfo();
	}

	private void ShowDisplayInfo()
	{
		if (null == HeadDisplayText.Prefab)
		{
			return;
		}
		if (null == this.Target)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		if (HeadDisplayText.NGUIChildObject == null)
		{
			HeadDisplayText.NGUIChildObject = NGUITools.AddChild(HUDTextRoot.go, HeadDisplayText.Prefab);
		}
		if (HeadDisplayText.HudText == null)
		{
			HeadDisplayText.HudText = HeadDisplayText.NGUIChildObject.GetComponentInChildren<HUDTextCustom>();
		}
	}

	private void HideDisplayInfo()
	{
	}

	public void Add(object obj, Color c, float stayDuration, float offsetX = 0f, float fontSize = 0f, HUDTextCustom.TextType textType = HUDTextCustom.TextType.Normal)
	{
		if (null == HeadDisplayText.HudText)
		{
			this.ShowDisplayInfo();
			if (null == HeadDisplayText.HudText)
			{
				return;
			}
		}
		HeadDisplayText.HudText.Add(obj, this.Target.position, c, stayDuration, offsetX, fontSize, textType);
	}

	private static GameObject Prefab;

	private static GameObject NGUIChildObject;

	private static HUDTextCustom HudText;

	public Transform Target;
}
