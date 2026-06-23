using System;
using HSGameEngine.GameEngine.SilverLight;
using HSGameEngine.GameFramework.Logic;
using UnityEngine;

public class TencentLogin : UserControl
{
	protected override void InitializeComponent()
	{
		base.InitializeComponent();
		this.btnQQ.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnQQ_Click);
		this.btnWeixin.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnWeixin_Click);
		this.btnOtherQQ.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnQQ_Click);
		this.ShowOtherLogin.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnShowOthter);
		this.btnLuntan.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnLuntan_Click);
		this.CloseOtherUI.MouseLeftButtonUp = new MouseLeftButtonUpEventHandler(this.BtnCloseOther);
		if (PlatSDKMgr.PlatName.Equals("QQ"))
		{
			this.btnLuntan.gameObject.SetActive(false);
		}
		else
		{
			this.btnLuntan.gameObject.SetActive(true);
		}
		this.ShowOtherUI(false);
	}

	private void BtnQQ_Click(object sender, MouseEvent e)
	{
		this.ShowOtherUI(false);
		PlatSDKMgr.Login(base.gameObject, "QQ");
	}

	private void BtnWeixin_Click(object sender, MouseEvent e)
	{
		this.ShowOtherUI(false);
		PlatSDKMgr.Login(base.gameObject, "WX");
	}

	private void BtnLuntan_Click(object sender, MouseEvent e)
	{
		Application.OpenURL("http://bbs.g.qq.com/forum.php?mod=forumdisplay&fid=52086&ADTAG=game.app.qmqj");
	}

	private void BtnShowOthter(object sender, MouseEvent e)
	{
		this.ShowOtherUI(true);
	}

	private void BtnCloseOther(object seneder, MouseEvent e)
	{
		this.ShowOtherUI(false);
	}

	private void ShowOtherUI(bool show)
	{
		this.SelectOtherUI.SetActive(show);
	}

	public GButton btnQQ;

	public GButton btnWeixin;

	public GButton btnOtherQQ;

	public GButton btnLuntan;

	public GButton ShowOtherLogin;

	public GameObject SelectOtherUI;

	public GButton CloseOtherUI;

	public DPSelectedItemEventHandler DPSelectedItem;
}
