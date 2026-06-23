using System;
using UnityEngine;

public class GoodsPackInfoDisplay : MonoBehaviour
{
	public Color GoodsNameColor
	{
		set
		{
			this._GoodsNameColor = value;
			if (null != this.GoodsName)
			{
				this.GoodsName.color = this._GoodsNameColor;
			}
		}
	}

	private void Start()
	{
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
		if (null == this.Prefab)
		{
			return;
		}
		if (null == HUDTextRoot.go)
		{
			return;
		}
		this.NGUIChildObject = NGUITools.AddChild(HUDTextRoot.go, this.Prefab);
		this.NGUIChildObject.AddComponent<UIFollowTarget>().target = this.Target;
		this.GoodsName = this.NGUIChildObject.transform.Find("Label_GoodsName").gameObject.GetComponent<UILabel>();
		this.GoodsName.text = this.GoodsNameText;
		this.GoodsNameColor = this._GoodsNameColor;
	}

	private void HideDisplayInfo()
	{
		if (null == this.NGUIChildObject)
		{
			return;
		}
		Object.Destroy(this.NGUIChildObject);
		this.NGUIChildObject = null;
		this.GoodsName = null;
	}

	public GameObject Prefab;

	public Transform Target;

	public string GoodsNameText;

	private Color _GoodsNameColor = Color.white;

	private GameObject NGUIChildObject;

	private UILabel GoodsName;
}
