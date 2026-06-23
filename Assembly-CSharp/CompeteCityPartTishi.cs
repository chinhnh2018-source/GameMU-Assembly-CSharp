using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.Network;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class CompeteCityPartTishi : UserControl
{
	private void InitTextInPrefabs()
	{
		this.BtnJoin.Label.text = Global.GetLang("立即进入");
		this.BtnQuXiao.Label.text = Global.GetLang("取消");
		this.Miaoshu.text = Global.GetLang("争夺之地已开启，战盟BOSS争夺等你来战!");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.BtnJoin.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			GameInstance.Game.SendCompeteCityEnterGameData();
			this.CloseHandler(this, new DPSelectedItemEventArgs());
			Super.ShowNetWaiting(null);
		};
		this.BtnQuXiao.MouseLeftButtonUp = delegate(object s, MouseEvent e)
		{
			this.CloseHandler(this, new DPSelectedItemEventArgs());
		};
	}

	public DPSelectedItemEventHandler CloseHandler;

	public GButton BtnJoin;

	public GButton BtnQuXiao;

	public UILabel Miaoshu;
}
