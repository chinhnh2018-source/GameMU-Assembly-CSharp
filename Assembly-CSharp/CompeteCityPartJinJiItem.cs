using System;
using HSGameEngine.GameEngine.Logic;

public class CompeteCityPartJinJiItem : UserControl
{
	public int Rank
	{
		set
		{
			if (value > 3)
			{
				this.paiMing.gameObject.SetActive(false);
				this.PaiMing.text = Global.GetColorStringForNGUIText(new object[]
				{
					"fdf7dd",
					value
				});
			}
			else
			{
				this.paiMing.gameObject.SetActive(true);
				this.paiMing.spriteName = value.ToString();
			}
		}
	}

	public string SetName
	{
		set
		{
			this.Name.text = Global.GetColorStringForNGUIText(new object[]
			{
				"6F9CE0",
				value
			});
		}
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public UILabel PaiMing;

	public new UILabel Name;

	public UILabel DengJi;

	public UILabel Zhanli;

	public UILabel YongShi;

	public UISprite paiMing;
}
