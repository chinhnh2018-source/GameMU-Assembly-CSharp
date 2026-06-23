using System;
using UnityEngine;

public class JingLingPropInF : UserControl
{
	public UIDraggablePanel MDragPanel
	{
		set
		{
			if (null == this.mDragPanel)
			{
				this.mDragPanel = base.GetComponent<UIDragPanelContents>();
			}
			if (null == this.mDragPanel)
			{
				this.mDragPanel = base.gameObject.AddComponent<UIDragPanelContents>();
			}
			this.mDragPanel.draggablePanel = value;
			UIPanel component = base.GetComponent<UIPanel>();
			if (null != component)
			{
				Object.Destroy(component);
			}
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		try
		{
			this.LabelPropInfValue1.pivot = 5;
			this.LabelPropInfValue1.transform.localPosition = new Vector3(160f, 0f, -1f);
			this.SpUp.transform.localPosition = new Vector3(105f, this.SpUp.transform.localPosition.y, this.SpUp.transform.localPosition.z);
		}
		catch
		{
			MUDebug.LogError<string>(new string[]
			{
				"越南东南亚英文报空！Please check it"
			});
		}
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
	}

	public UILabel LabelPropInfName;

	public UISprite SpUp;

	public UILabel LabelPropInfValue1;

	private UIDragPanelContents mDragPanel;
}
