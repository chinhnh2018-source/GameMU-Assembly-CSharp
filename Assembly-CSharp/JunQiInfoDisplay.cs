using System;
using UnityEngine;

public class JunQiInfoDisplay : MonoBehaviour
{
	private void Start()
	{
		if (JunQiInfoDisplay.Prefab == null)
		{
			JunQiInfoDisplay.Prefab = (Resources.Load("Prefabs/FollowInfo/MonsterInfo") as GameObject);
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
		if (null == JunQiInfoDisplay.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		this.NGUIChildObject = NGUITools.AddChild(HUDTextRoot.go, JunQiInfoDisplay.Prefab);
		this.NGUIChildObject.AddComponent<UIFollowTarget>().target = this.Target;
		this.JunQiName = this.NGUIChildObject.transform.Find("Label_MonsterName").gameObject.GetComponent<UILabel>();
		this.JunQiName.text = this.JunQiNameText;
		this.JunQiName.color = new Color(0f, 1f, 0f, 1f);
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		Object.Destroy(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.JunQiName = null;
	}

	private static GameObject Prefab;

	public Transform Target;

	public string JunQiNameText;

	private GameObject NGUIChildObject;

	private UILabel JunQiName;
}
