using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class HelpPart : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitPrefabText()
	{
		this.m_ContentLabel0.lineWidth = 415;
	}

	private void InitTexture()
	{
	}

	private void InitHandler()
	{
		this.m_ColseBtn.MouseLeftButtonUp = delegate(object e, MouseEvent s)
		{
			if (this.ClseoHander != null)
			{
				this.ClseoHander(e, new DPSelectedItemEventArgs());
			}
		};
	}

	public void SetContent(string Content0, string Content1, float TitleYAddValue = 0f)
	{
		this.m_ContentLabel.text = Content0;
		this.m_ContentLabel0.text = Content1;
		if (TitleYAddValue != 0f)
		{
			Vector3 localPosition = this.m_ContentLabel.transform.localPosition;
			localPosition.y += TitleYAddValue;
			this.m_ContentLabel.transform.localPosition = localPosition;
		}
		TextBlock component = this.m_ContentLabel0.GetComponent<TextBlock>();
		if (null != component)
		{
			BoxCollider component2 = this.m_ContentLabel0.transform.parent.GetComponent<BoxCollider>();
			if (null != component2)
			{
				component2.size = new Vector3(component2.size.x, (float)component.ActualHeight + 200f, component2.size.z);
				component2.center = new Vector3(component2.center.x, -(float)component.ActualHeight / 2f, component2.center.z);
			}
		}
	}

	public GButton m_ColseBtn;

	public UILabel m_ContentLabel;

	public UILabel m_ContentLabel0;

	public DPSelectedItemEventHandler ClseoHander;
}
