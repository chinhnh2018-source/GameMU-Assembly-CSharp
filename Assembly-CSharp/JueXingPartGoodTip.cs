using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using Server.Tools;

public class JueXingPartGoodTip : UserControl
{
	private void InitTextInPrefabs()
	{
		this.GoodIcon.isAutoSize = true;
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite15.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite2.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.BindingSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.EndTimeSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.NoUseSprite.gameObject, false);
		NGUITools.SetActive(this.GoodIcon.TeXiao.gameObject, false);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs
			{
				ID = 0
			});
		};
	}

	public void InitGoods(GoodVO goodVO)
	{
		GGoodTips.LastGoodsID = goodVO.ID;
		this.lblTitle.text = Global.GetColorStringForNGUIText(new object[]
		{
			goodVO.GoodsColor,
			goodVO.Title
		});
		string url = StringUtil.substitute("NetImages/GameRes/Images/Goods/{0}.png", new object[]
		{
			Super.GetIconCode(goodVO)
		});
		this.GoodIcon.GoodImg.URL = url;
		int categoriy = goodVO.Categoriy;
		this.lblType.text = Global.GetGoodsType(categoriy);
		this.GoodIcon.BackgroundSprite1.spriteName = "bagGrid4_bak";
		NGUITools.SetActive(this.GoodIcon.BackgroundSprite1.gameObject, true);
		this.lblDescription.text = goodVO.Description;
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblDescription;

	public UILabel lblType;

	public GGoodIcon GoodIcon;

	public GButton btnClose;
}
