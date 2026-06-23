using System;
using HSGameEngine.GameEngine.Logic;

public class DuiHuanItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.duihuanBtn.Text = Global.GetLang("立即兑换");
	}

	protected override void InitializeComponent()
	{
		this.InitTextInPrefabs();
	}

	public GButton duihuanBtn;

	public UISprite jiaSprite;

	public GGoodIcon TargetGoods;

	public GGoodIcon SourceGoods;

	public GGoodIcon SourceGoods2;

	public int itemID;

	public int iDayDuiHuanNum;

	public int iDayTotalDuiHuanNum;

	public string strNeedGoods;

	public TextBlock xianzhiText;
}
