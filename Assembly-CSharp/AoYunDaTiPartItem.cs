using System;
using HSGameEngine.GameEngine.Logic;

public class AoYunDaTiPartItem : UserControl
{
	public int SetPaiMing
	{
		set
		{
			if (value > 3)
			{
				this.PaiMing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					value
				});
				this.Paihang.gameObject.SetActive(false);
			}
			else if (value >= 1 && value <= 3)
			{
				this.PaiMing.text = string.Empty;
				this.Paihang.spriteName = value.ToString();
			}
		}
	}

	public string SetName
	{
		set
		{
			this.Name.text = value;
		}
	}

	public int SetJiFen
	{
		set
		{
			this.JiFen.text = value.ToString();
		}
	}

	public UISprite Paihang;

	public UILabel PaiMing;

	public new UILabel Name;

	public UILabel JiFen;
}
