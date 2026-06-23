using System;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class ChengJiuTypeNewItem : UserControl
{
	protected override void InitializeComponent()
	{
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject s)
		{
			if (this.MouseLeftButtonUp != null)
			{
				this.MouseLeftButtonUp(this, new DPSelectedItemEventArgs
				{
					ID = -1
				});
			}
		};
	}

	public string NameText
	{
		set
		{
			this.name.Text = value;
		}
	}

	public string imageUrl
	{
		set
		{
			this.image.URL = value;
			this.image.ForceShow();
		}
	}

	public UISprite Bak;

	public ShowNetImage image;

	public TextBlock name;

	public string typeID = "-1";

	public DPSelectedItemBoolEventHandler MouseLeftButtonUp;
}
