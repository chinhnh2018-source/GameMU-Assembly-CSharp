using System;

public class PKLoversPartZhanBaoList : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
	}

	public string Miaoshu
	{
		set
		{
			this.ZhanBao.text = value;
		}
	}

	public UILabel ZhanBao;
}
