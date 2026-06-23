using System;
using HSGameEngine.GameEngine.Logic;

public class HefuChongzhiFanliItem : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
	}

	private void InitTextInPrefabs()
	{
		this.labelDi.text = Global.GetLang("第");
		this.labelMing.text = Global.GetLang("名");
	}

	public TextBlock rank;

	public TextBlock precent;

	public TextBlock roleName;

	public TextBlock labelDi;

	public TextBlock labelMing;
}
