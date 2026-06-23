using System;
using UnityEngine;

public class UIPanelControl : MonoBehaviour
{
	private void Start()
	{
		if (null == this._Panel)
		{
			this._Panel = base.GetComponent<UIPanel>();
		}
		if (null == this._ListBox)
		{
			this._ListBox = base.GetComponentInChildren<ListBox>();
		}
		if (null != this._ListBox)
		{
			this._ListBox.ChildHasChange = this.PanelChildHaveChange();
		}
		if (null == this._UITable)
		{
			this._UITable = base.GetComponentInChildren<UITable>();
		}
		if (null != this._UITable)
		{
			this._UITable.onReposition = new UITable.OnReposition(this.OnReposition);
		}
	}

	private void OnReposition()
	{
		UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren != null)
		{
			foreach (UIWidget uiwidget in componentsInChildren)
			{
				byte b = 0;
				if (null != uiwidget.panel && !uiwidget.panel.Equals(this._Panel))
				{
					Object.Destroy(uiwidget.panel);
					b = 1;
				}
				if (b == 1)
				{
					uiwidget.panel = uiwidget.panel;
				}
			}
		}
	}

	private ListBox.VoidDelegate PanelChildHaveChange()
	{
		UIWidget[] componentsInChildren = base.GetComponentsInChildren<UIWidget>();
		if (componentsInChildren != null)
		{
			for (int i = 0; i < componentsInChildren.Length; i++)
			{
				byte b = 0;
				if (null != componentsInChildren[i].panel && !componentsInChildren[i].panel.Equals(this._Panel))
				{
					Object.Destroy(componentsInChildren[i].panel);
					b = 1;
				}
				if (b == 1)
				{
					componentsInChildren[i].panel = componentsInChildren[i].panel;
				}
			}
		}
		return null;
	}

	[SerializeField]
	private UIPanel _Panel;

	[SerializeField]
	private ListBox _ListBox;

	[SerializeField]
	private UITable _UITable;

	private Vector4 mClipParamters = Vector4.zero;

	private float mSoftness;
}
