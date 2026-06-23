using System;
using Server.Data;

public class SynthesizeItem : UserControl
{
	protected override void InitializeComponent()
	{
		this.NameText.textColor = 65280U;
		this.InfoText.textColor = 15917757U;
		this.InfoText.Pivot = 3;
		this.InfoText.X = -60.0;
		this.InfoText.Y = -9.0;
		this.InfoText.MaxWidth = 200.0;
	}

	public UISprite Bak;

	public TextBlock NameText;

	public TextBlock InfoText;

	public GGoodIcon gIcon;

	public UISprite select;

	public string IDText = string.Empty;

	public GoodsData goodsData;

	public string sTag = string.Empty;
}
