using System;

public class RiChangPaTaItem : UserControl
{
	public bool SelectStat
	{
		get
		{
			return this.SelectBak.Visibility;
		}
		set
		{
			this.SelectBak.Visibility = value;
		}
	}

	public RiChangHuoDongData Data
	{
		get
		{
			return this.riChangHuoDongData;
		}
		set
		{
			this.riChangHuoDongData = value;
		}
	}

	public int Index
	{
		get
		{
			return this.index;
		}
		set
		{
			this.index = value;
		}
	}

	public UISprite Bak;

	public SpriteSL SelectBak;

	public TextBlock TxtName;

	private int index;

	private RiChangHuoDongData riChangHuoDongData;
}
