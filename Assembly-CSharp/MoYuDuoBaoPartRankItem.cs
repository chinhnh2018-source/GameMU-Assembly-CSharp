using System;
using HSGameEngine.GameEngine.Logic;

public class MoYuDuoBaoPartRankItem : UserControl
{
	private void InitTextInPrefabs()
	{
		this.lblContent1.text = Global.GetLang(string.Empty);
		this.lblContent2.text = Global.GetLang(string.Empty);
		this.lblContent3.text = Global.GetLang(string.Empty);
		this.lblContent4.text = Global.GetLang(string.Empty);
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	public void InitInfo(int rank, string content2, string content3, string content4)
	{
		this.lblContent1.text = rank.ToString();
		this.lblContent2.text = content2;
		this.lblContent3.text = content3;
		this.lblContent4.text = content4;
	}

	public UILabel lblContent1;

	public UILabel lblContent2;

	public UILabel lblContent3;

	public UILabel lblContent4;
}
