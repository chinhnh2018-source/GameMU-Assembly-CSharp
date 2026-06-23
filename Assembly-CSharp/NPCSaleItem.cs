using System;

public class NPCSaleItem : UserControl
{
	protected override void InitializeComponent()
	{
	}

	public int GoodsID
	{
		get
		{
			return this._GoodsID;
		}
		set
		{
			this._GoodsID = value;
		}
	}

	public GGoodIcon goodsIcon
	{
		get
		{
			return this.gIcon;
		}
		set
		{
			this.Icon.Add(value.gameObject);
			this.gIcon = value;
		}
	}

	public bool SetSelectStat
	{
		set
		{
			this.SelectedRect.Visibility = value;
		}
	}

	public GGoodIcon Icon;

	public TextBlock ItemName;

	public TextBlock MoneyText;

	public SpriteSL SelectedRect;

	public UISprite MoneyTeypImg;

	public GGoodIcon gIcon;

	private int _GoodsID;
}
