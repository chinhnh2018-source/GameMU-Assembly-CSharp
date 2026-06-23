using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class DuanWeiLoversPartListItem : UserControl
{
	public bool IsXuanZhong
	{
		set
		{
			this.xuanzhong.SetActive(value);
		}
	}

	public string SetManName
	{
		set
		{
			this.ManName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				value
			});
		}
	}

	public string SetWoManName
	{
		set
		{
			this.WoManName.text = Global.GetColorStringForNGUIText(new object[]
			{
				"f0f0f0",
				value
			});
		}
	}

	public int PaiMingNum
	{
		set
		{
			if (value > 3)
			{
				this.PaiMingLab.text = Global.GetColorStringForNGUIText(new object[]
				{
					"f0f0f0",
					string.Format(Global.GetLang("第{0}名"), value)
				});
				this.PaiMing.gameObject.SetActive(false);
			}
			else
			{
				this.PaiMing.spriteName = this.GetPaiMing(value);
				this.PaiMingLab.gameObject.SetActive(false);
			}
		}
	}

	private string GetPaiMing(int pm)
	{
		string result = string.Empty;
		if (pm == 1)
		{
			result = "One";
		}
		else if (pm == 2)
		{
			result = "Two";
		}
		else if (pm == 3)
		{
			result = "Three";
		}
		return result;
	}

	public DPSelectedItemEventHandler ObjHandler;

	public UILabel ManName;

	public UILabel WoManName;

	public UILabel PaiMingLab;

	public GameObject xuanzhong;

	public UISprite PaiMing;

	public int num;
}
