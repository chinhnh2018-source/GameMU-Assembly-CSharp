using System;
using UnityEngine;

public class PaddingObject : UserControl
{
	public Vector3 Size
	{
		get
		{
			return this._Size;
		}
		set
		{
			this._Size = value;
			this.Texture.transform.localScale = value;
			base.GetComponent<BoxCollider>().size = this._Size;
		}
	}

	public void Init()
	{
		Transform parent = base.transform.parent;
		ListBox listBox = NGUITools.FindInParents<ListBox>(base.gameObject);
		if (null != listBox)
		{
			parent = listBox.transform.parent;
		}
		UIDragObject uidragObject = base.gameObject.GetComponent<UIDragObject>();
		if (null == uidragObject)
		{
			uidragObject = base.gameObject.AddComponent<UIDragObject>();
		}
		uidragObject.scale = Vector3.right;
		uidragObject.target = base.transform.parent;
		uidragObject.restrictWithinPanel = true;
	}

	[SerializeField]
	private UITexture Texture;

	private Vector3 _Size;
}
