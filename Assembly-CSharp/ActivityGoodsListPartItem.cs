using System;
using HSGameEngine.GameEngine.Logic;
using Server.Data;

public class ActivityGoodsListPartItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		if (null != this._Icon)
		{
			this._Icon.EnableEvent = false;
		}
		this._bak.URL = Global.GetGameResImageString("ActivityGoodsListPartItem_bak.png");
		this._Icon.Width = 78.0;
		this._Icon.Height = 78.0;
	}

	public void Init()
	{
		this._Spliter.gameObject.SetActive(true);
	}

	public UISprite _Spliter;

	public ShowNetImage _bak;

	public TextBlock _Label;

	public GGoodIcon _Icon;

	public int GoodsID;

	public GoodsData GoodsData;
}
