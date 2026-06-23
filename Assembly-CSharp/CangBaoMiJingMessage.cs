using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class CangBaoMiJingMessage : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitPrefabText();
		this.InitTexture();
		this.InitHandler();
	}

	private void InitHandler()
	{
		UIEventListener.Get(this.m_MaskSp.gameObject).onClick = delegate(GameObject e)
		{
			this.Closehandler(e, new DPSelectedItemEventArgs());
		};
	}

	private void InitPrefabText()
	{
	}

	private void InitTexture()
	{
		if (null != this.m_MaskSp)
		{
			this.m_MaskSp.enabled = false;
		}
	}

	public void RefreshMessage(string Tips, string Inf)
	{
		if (null != this.m_Label_1)
		{
			this.m_Label_1.text = Global.GetColorStringForNGUIText(new object[]
			{
				"fac60d",
				Tips
			});
		}
		if (null != this.m_Label_2)
		{
			this.m_Label_2.text = Global.GetColorStringForNGUIText(new object[]
			{
				"feedc5",
				Inf
			});
		}
	}

	public UISprite m_MaskSp;

	public UILabel m_Label_1;

	public UILabel m_Label_2;

	public DPSelectedItemEventHandler Closehandler;
}
