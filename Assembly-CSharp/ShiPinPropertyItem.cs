using System;
using UnityEngine;

public class ShiPinPropertyItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this._Label.text = string.Empty;
		this._LabelNextLeve.text = string.Empty;
		this._UpObj.SetActive(false);
		this._Label.transform.localPosition = new Vector3(-140f, 0f, 0f);
		this._LabelNextLeve.transform.localPosition = new Vector3(115f, 0f, 0f);
		this._UpObj.transform.localPosition = new Vector3(100f, 0f, 0f);
	}

	public string Label1
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				this._Label.text = value;
			}
		}
	}

	public string Label2
	{
		set
		{
			if (!string.IsNullOrEmpty(value))
			{
				this._LabelNextLeve.text = value;
			}
		}
	}

	public bool bShowUp
	{
		set
		{
			NGUITools.SetActive(this._UpObj, value);
		}
	}

	public bool DelectPanel
	{
		set
		{
			if (value)
			{
				UIPanel component = base.GetComponent<UIPanel>();
				if (null != component)
				{
					Object.Destroy(component);
				}
			}
		}
	}

	public UIDraggablePanel DraggablePanel
	{
		set
		{
			if (null == this._UIDragPanelContents)
			{
				this._UIDragPanelContents = base.transform.GetComponent<UIDragPanelContents>();
				if (null == this._UIDragPanelContents)
				{
					this._UIDragPanelContents = base.gameObject.AddComponent<UIDragPanelContents>();
				}
				this._UIDragPanelContents.draggablePanel = value;
			}
		}
	}

	public UILabel _Label;

	public UILabel _LabelNextLeve;

	public GameObject _UpObj;

	private UIDragPanelContents _UIDragPanelContents;
}
