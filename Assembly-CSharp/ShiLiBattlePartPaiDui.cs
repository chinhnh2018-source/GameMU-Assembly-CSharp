using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class ShiLiBattlePartPaiDui : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblTitle.text = Global.GetLang("提示");
		this.lblContent1.text = Global.GetLang("当前战场人数已满，需要排队进入！");
		this.lblContent2.text = Global.GetLang("排队人数 : ");
		this.btnCancel.Text = Global.GetLang("取消");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.btnClose.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
		this.btnCancel.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public void ResetNum(int num)
	{
		this.lblNum.text = num.ToString();
	}

	public DPSelectedItemEventHandler CloseHandler;

	public UILabel lblTitle;

	public UILabel lblContent1;

	public UILabel lblContent2;

	public UILabel lblNum;

	public GButton btnClose;

	public GButton btnCancel;
}
