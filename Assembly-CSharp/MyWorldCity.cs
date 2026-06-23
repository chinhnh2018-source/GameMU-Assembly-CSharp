using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;
using UnityEngine;

public class MyWorldCity : UserControl
{
	public int ID
	{
		get
		{
			return this.id;
		}
		set
		{
			this.id = value;
		}
	}

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

	public string CityName
	{
		get
		{
			return this.cityName;
		}
		set
		{
			this.cityName = value;
			string text = "fef5e5";
			if (this.Level == 10)
			{
				text = "a03eb9";
			}
			else if (this.Level == 9)
			{
				text = "0187ef";
			}
			else if (this.Level == 8)
			{
				text = "3eb431";
			}
			this.name.text = Global.GetColorStringForNGUIText(new object[]
			{
				string.Format("{0}", text),
				this.cityName
			});
		}
	}

	public LangHunLingYuCityData LangHunlingyuCityData
	{
		get
		{
			return this.langhunlingyuCityData;
		}
		set
		{
			this.langhunlingyuCityData = value;
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		UIEventListener.Get(base.gameObject).onClick = delegate(GameObject e)
		{
			PlayZone.GlobalPlayZone.OpenWolfSoulFieldCityInfoWindow(this.Level, false, true, this.LangHunlingyuCityData);
		};
	}

	public UILabel name;

	private int id = 1;

	private int level;

	private string cityName;

	private LangHunLingYuCityData langhunlingyuCityData;
}
