using System;
using UnityEngine;

public class TeleportInfoDisplay : MonoBehaviour
{
	private void Start()
	{
		if (TeleportInfoDisplay.Prefab == null)
		{
			TeleportInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/TeleportInfo") as GameObject);
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
		if (null != this.NGUIChildObject)
		{
			return;
		}
		if (null == TeleportInfoDisplay.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		this.NGUIChildObject = NGUITools.AddChild(HUDTextRoot.go, TeleportInfoDisplay.Prefab);
		this.NGUIChildObject.AddComponent<UIFollowTarget>().target = this.Target;
		this.TeleName = this.NGUIChildObject.transform.Find("Label_TeleName").gameObject.GetComponent<UILabel>();
		this.TeleName.text = this.TeleNameText;
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		Object.Destroy(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.TeleName = null;
	}

	private static GameObject Prefab;

	public Transform Target;

	public string TeleNameText;

	private GameObject NGUIChildObject;

	private UILabel TeleName;
}
