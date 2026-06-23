using System;

public class JunxianItem : UserControl
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

	public string NeedGoods
	{
		get
		{
			return this.needGoods;
		}
		set
		{
			this.needGoods = value;
		}
	}

	public ShowNetImage Icon;

	public UISprite Bkg;

	public UISprite Selected;

	private int level;

	private string needGoods = string.Empty;
}
