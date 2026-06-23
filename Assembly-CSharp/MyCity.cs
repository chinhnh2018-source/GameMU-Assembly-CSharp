using System;
using UnityEngine;

public class MyCity : UserControl
{
	public int Level
	{
		get
		{
			return this.level;
		}
		set
		{
			this.level = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject e)
		{
			if (this.isOtherCity)
			{
				PlayZone.GlobalPlayZone.OpenWolfSoulFieldCityInfoWindow(this.Level, true, false, null);
			}
			else
			{
				PlayZone.GlobalPlayZone.OpenWolfSoulFieldCityInfoWindow(this.Level, false, false, null);
			}
		};
	}

	public UISprite Cloud;

	public UISprite State;

	public new UILabel Name;

	public GameObject Fire;

	public bool isOtherCity;

	private int level;
}
