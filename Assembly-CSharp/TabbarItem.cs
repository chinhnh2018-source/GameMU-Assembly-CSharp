using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TabbarItem : UserControl
{
	private void InitTextInPrefabs()
	{
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				ID = this.ID
			});
		};
	}

	public TabbarItemState tabbarItemState
	{
		get
		{
			return this._tabbarItemState;
		}
		set
		{
			this._tabbarItemState = value;
			this.SetTabbarState();
			this.SetAdditiveSpriteState1();
			this.SetAdditiveSpriteState2();
		}
	}

	public string barName
	{
		set
		{
			this._barName = value;
			if (null != this.tabbarItem)
			{
				this.tabbarItem.Text = this._barName;
			}
		}
	}

	private void SetTabbarState()
	{
		if (null == this.tabbarItem)
		{
			return;
		}
		Color textColor = Color.white;
		bool pressed = false;
		bool flag = false;
		switch (this.tabbarItemState)
		{
		case TabbarItemState.Normal:
			textColor = NGUIMath.HexToColorEx(10323559U);
			pressed = false;
			flag = true;
			break;
		case TabbarItemState.Hover:
			textColor = NGUIMath.HexToColorEx(15790320U);
			pressed = true;
			flag = true;
			break;
		case TabbarItemState.Disabled:
			textColor = Color.gray;
			pressed = false;
			flag = false;
			break;
		}
		this.tabbarItem.Pressed = pressed;
		this.tabbarItem.isEnabled = flag;
		this.tabbarItem.TextColor = textColor;
		this.tabbarItem.Refresh();
		BoxCollider component = base.GetComponent<BoxCollider>();
		if (null != component)
		{
			component.enabled = (this.triggerWhenDisable || flag);
		}
	}

	private void SetAdditiveSpriteState1()
	{
		if (null == this.additiveDecoration1)
		{
			return;
		}
		int num = this.additiveDecoration1.spriteName.LastIndexOf('_');
		string text = this.additiveDecoration1.spriteName.Substring(0, num + 1);
		switch (this.tabbarItemState)
		{
		case TabbarItemState.Normal:
			text += "normal";
			break;
		case TabbarItemState.Hover:
			text += "highlight";
			break;
		case TabbarItemState.Disabled:
			text += "disable";
			break;
		}
		this.additiveDecoration1.spriteName = text;
	}

	private void SetAdditiveSpriteState2()
	{
		if (null == this.additiveDecoration2)
		{
			return;
		}
		bool active = true;
		switch (this.tabbarItemState)
		{
		case TabbarItemState.Normal:
		case TabbarItemState.Hover:
			active = false;
			break;
		case TabbarItemState.Disabled:
			active = true;
			break;
		}
		this.additiveDecoration2.gameObject.SetActive(active);
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public GButton tabbarItem;

	public UISprite additiveDecoration1;

	public UISprite additiveDecoration2;

	public int ID;

	public bool triggerWhenDisable;

	private TabbarItemState _tabbarItemState;

	private string _barName;
}
