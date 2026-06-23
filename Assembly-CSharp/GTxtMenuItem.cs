using System;
using UnityEngine;

public class GTxtMenuItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.BakTrans = this.Bak.transform;
		UIEventListener.Get(base.gameObject).onClick = new UIEventListener.VoidDelegate(this.SetTab);
	}

	public bool BakVisible
	{
		set
		{
			if (value)
			{
				this.Bak.spriteName = "menuItem_bak";
			}
			else
			{
				this.Bak.spriteName = "none";
			}
		}
	}

	public Vector3 BakSize
	{
		set
		{
			this.BakTrans.localScale = value;
			this.BakCollider.size = value;
		}
	}

	public int MenuItemID
	{
		get
		{
			return this._MenuItemID;
		}
		set
		{
			this._MenuItemID = value;
		}
	}

	public string MenuItemText
	{
		get
		{
			return this.txtItemText.Text;
		}
		set
		{
			this.txtItemText.Text = value;
		}
	}

	private void SetTab(object sender)
	{
		if (this.MenuItemClick != null)
		{
			this.MenuItemClick.Invoke(this, null);
		}
	}

	public UISprite Bak;

	public TextBlock txtItemText;

	public BoxCollider BakCollider;

	private Transform BakTrans;

	private int _MenuItemID;

	public EventHandler MenuItemClick;
}
