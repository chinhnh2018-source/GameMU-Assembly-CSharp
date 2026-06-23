using System;
using HSGameEngine.GameEngine.Logic;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;

public class SecondPasswordOptions : UserControl
{
	private void InitTextInPrefabs()
	{
		this.Title.text = Global.GetLang("二级密码");
		this.ChangeSecPassBtn.Text = Global.GetLang("修改二级密码");
		this.CancelSecPassBtn.Text = Global.GetLang("取消二级密码");
	}

	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.InitTextInPrefabs();
		this.ChangeSecPassBtn.MouseLeftButtonUp = delegate(object o, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).ShowChangeSecondPasswordWindow();
			(Super.GData.PlayZoneRoot as PlayZone).CloseSecondPasswordOptionsWindow();
		};
		this.CancelSecPassBtn.MouseLeftButtonUp = delegate(object o, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).ShowCancelSecondPasswordWindow();
			(Super.GData.PlayZoneRoot as PlayZone).CloseSecondPasswordOptionsWindow();
		};
		this.Close.MouseLeftButtonUp = delegate(object sender, MouseEvent e)
		{
			(Super.GData.PlayZoneRoot as PlayZone).CloseSecondPasswordOptionsWindow();
		};
	}

	public GButton ChangeSecPassBtn;

	public GButton CancelSecPassBtn;

	public GButton Close;

	public UILabel Title;
}
