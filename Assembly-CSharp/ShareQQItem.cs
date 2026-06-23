using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ShareQQItem : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(this.Img.gameObject).onClick = delegate(GameObject s)
		{
			this.DPSelectedItem(this, new DPSelectedItemEventArgs
			{
				IDType = 0,
				ID = this.ItemID
			});
		};
	}

	public string ImgUrl
	{
		set
		{
			this.Img.URL = string.Format("NetImages/GameRes/Images/QQ/{0}.png", value);
		}
	}

	public new string Name
	{
		set
		{
			this.TextName.Text = value;
		}
	}

	public int ItemID
	{
		get
		{
			return this._ItemID;
		}
		set
		{
			this._ItemID = value;
		}
	}

	public DPSelectedItemEventHandler DPSelectedItem;

	public ShowNetImage Img;

	public TextBlock TextName;

	private int _ItemID = -1;
}
